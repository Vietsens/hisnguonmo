/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.Controls.Session;
//using HIS.Desktop.Plugins.CareSlipList.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using MPS.ADO;
using HIS.Desktop.Plugins.CareSlipList.Popup;
using AutoMapper;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
namespace HIS.Desktop.Plugins.CareSlipList.CareSlipList
{
    public partial class FormCareSlipList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS_CARE _hisCare { get; set; }
        List<HIS_CARE> _listCare { get; set; }
        List<HisCareCheck> careCheckProcessing;
        List<HisCareCheck> careChecks;
        V_HIS_CARE_SUM printCareSum;
        //List<MPS.ADO.CareViewPrintADO> lstCareViewPrintADO { get; set; }
        //List<MPS.ADO.CareDetailViewPrintADO> lstCareDetailViewPrintADO { get; set; }
        internal V_HIS_TREATMENT currentHisTreatment { get; set; }
        internal List<HIS_AWARENESS> lstHisAwareness = new List<HIS_AWARENESS>();
        internal List<HIS_CARE_TYPE> lstHisCareType = new List<HIS_CARE_TYPE>();
        //List<HIS_CARE> careByTreatmentHasIcd { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM _treatmentBedRoom { get; set; }
        long treatmentID = 0;
        List<HIS_CARE> hiscare = new List<HIS_CARE>();
        #endregion

        #region Contructor

        public FormCareSlipList(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_BED_ROOM treatmentBedRoom)
		:base(module)
        {
            InitializeComponent();
            try
            {
                //this.treatmentId = treatmentId;
                this._treatmentBedRoom = treatmentBedRoom;
                this.treatmentID = treatmentBedRoom.TREATMENT_ID;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public FormCareSlipList(Inventec.Desktop.Common.Modules.Module module, long treatmentID)
		:base(module)
        {
            InitializeComponent();
            try
            {
                //this.treatmentId = treatmentId;
                //this._treatmentBedRoom = treatmentBedRoom;
                this.treatmentID = treatmentID;
                this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Grid Control Care
        private void fillDataToGridControl()
        {
            try
            {
                gridControlCare.DataSource = null;
                CommonParam param = new CommonParam();
                HisCareFilter filter = new HisCareFilter();
                filter.TREATMENT_ID = treatmentID;
                //bool success = false;
                var cares = new BackendAdapter(param).Get<List<HIS_CARE>>(ApiConsumer.HisRequestUriStore.HIS_CARE_GET, ApiConsumers.MosConsumer, filter, param);
                careChecks = new List<HisCareCheck>();

                foreach (var item in cares)
                {
                    MPS.ADO.HisCareCheck serviceCareReq = new MPS.ADO.HisCareCheck();
                    Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_CARE, MPS.ADO.HisCareCheck>();
                    serviceCareReq = Mapper.Map<MOS.EFMODEL.DataModels.HIS_CARE, MPS.ADO.HisCareCheck>(item);
                    careChecks.Add(serviceCareReq);
                }
                gridControlCare.DataSource = careChecks;
                fillDataToGridCareSum();
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewCare_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_CARE data = (MOS.EFMODEL.DataModels.HIS_CARE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXECUTE_LOGINNAME_USERNAME")
                        {
                            e.Value = data.EXECUTE_LOGINNAME + " - " + data.EXECUTE_USERNAME;
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "HAS_TEST_DISPLAY")
                        {
                            e.Value = (data.HAS_TEST ?? 0) == 1 ? "X" : "";
                        }
                        else if (e.Column.FieldName == "HAS_MEDICINE_DISPLAY")
                        {
                            e.Value = (data.HAS_MEDICINE ?? 0) == 1 ? "X" : "";
                        }
                        else if (e.Column.FieldName == "HAS_ADD_MEDICINE_DISPLAY")
                        {
                            e.Value = (data.HAS_ADD_MEDICINE ?? 0) == 1 ? "X" : "";
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCare_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //long careSumId = 0;
                    long careSumId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewCare.GetRowCellValue(e.RowHandle, "CARE_SUM_ID") ?? 0).ToString());
                    var creator = gridViewCare.GetRowCellValue(e.RowHandle, "CREATOR");
                    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "checkSum")
                    {
                        if (careSumId == 0)
                        {
                            e.RepositoryItem = checkE;
                        }
                        else
                        {
                            e.RepositoryItem = checkD;
                        }
                    }
                    if (e.Column.FieldName == "btnEdit")
                    {
                        if (careSumId == 0)
                        {
                            e.RepositoryItem = btnEditE;
                        }
                        else
                        {
                            e.RepositoryItem = btnEditD;
                        }
                    }
                    if (e.Column.FieldName == "btnDelete")
                    {
                        if (careSumId == 0 && loginName.Equals(creator))
                        {
                            e.RepositoryItem = btnDeleteE;
                        }
                        else
                        {
                            e.RepositoryItem = btnDeleteD;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Grid control CareSum
        private void fillDataToGridCareSum()
        {
            CommonParam param = new CommonParam();
            try
            {

                HisCareSumFilter filter = new HisCareSumFilter();
                filter.TREATMENT_ID = treatmentID;
                var rs = new BackendAdapter(param).Get<List<V_HIS_CARE_SUM>>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (rs != null)
                {
                    gridControlCareSum.BeginUpdate();
                    gridControlCareSum.DataSource = rs;
                    gridControlCareSum.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewCareSum_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_CARE_SUM data = (MOS.EFMODEL.DataModels.V_HIS_CARE_SUM)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "ICD_MAIN")
                        {
                            if (String.IsNullOrEmpty(data.ICD_MAIN_TEXT))
                            {
                                e.Value = data.ICD_NAME;
                            }
                            else
                            {
                                e.Value = data.ICD_MAIN_TEXT;
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewCareSum_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {

        }

        private void gridControlCareSum_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Event

        private void FormCareSlipList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Init();
                //fillDataToGridCareSum();
                WaitingManager.Hide();
                //label1.Text = treatmentID.ToString();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }


        private void btnSum_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                careCheckProcessing = new List<HisCareCheck>();
                if (careChecks != null && careChecks.Count > 0)
                {
                    careCheckProcessing = careChecks.Where(o => o.checkSum == true).ToList();
                    if (careCheckProcessing != null && careCheckProcessing.Count > 0)
                    {
                        FormCareSum frmCareSum = new FormCareSum(fillDataToGridControl, treatmentID);
                        frmCareSum.careCheckProcessing = this.careCheckProcessing;
                        frmCareSum.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Bạn chưa chọn dịch vụ nào");
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEditE_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                var row = (HIS_CARE)gridViewCare.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.CareCreate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.CareCreate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Hide();

                    }
                    else
                    {
                        MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
                fillDataToGridControl();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteE_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Test");
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {

                WaitingManager.Show();
                HisCareCheck row = (HisCareCheck)gridViewCare.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (row != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_CARE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, row, null);
                        if (success == true)
                        {
                            //gridViewCare.DeleteRow(gridViewCare.FocusedRowHandle);
                            //ExaminationProcessor.RefeshDataAfterAssignSuccess(moduleType, false);
                            Init();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        ResultManager.ShowMessage(param, success);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
                Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetMessage(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat);
            }
        }

        private void btnDeleteSum_Click(object sender, EventArgs e)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_CARE_SUM)gridViewCareSum.GetFocusedRow();
                if (row != null)
                {
                    result = new BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_CARE_SUM_DELETE, ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                    if (result == true)
                    {
                        Init();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                ResultManager.ShowMessage(param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void btnEditSum_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_CARE_SUM)gridViewCareSum.GetFocusedRow();
                FormCareSum frmCareSum = new FormCareSum(row, fillDataToGridCareSum);
                //frmCareSum. = this;
                frmCareSum.ShowDialog();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrintSum_Click(object sender, EventArgs e)
        {
            try
            {
                printCareSum = null;
                printCareSum = new V_HIS_CARE_SUM();
                printCareSum = (V_HIS_CARE_SUM)gridViewCareSum.GetFocusedRow();
                onClickPrintCare();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        public void Init()
        {
            CommonParam param = new CommonParam();
            try
            {
                fillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion



    }

}
