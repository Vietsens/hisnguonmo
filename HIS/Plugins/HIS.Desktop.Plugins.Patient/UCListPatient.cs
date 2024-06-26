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
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HID.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Patient
{
    public partial class UCListPatient : UserControlBase
    {
        internal const string KeyHid = "CONFIG_KEY__IS_USE_HID_SYNC";

        V_HIS_PATIENT patient;
        HID_PERSON selectPerson;
        string logginname;
        List<string> error;
        string datetime;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        List<PatientADO> listPatientImp;
        PatientADO patientImp;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        private string LoggingName = "";
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        string fileName = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.IO.Path.Combine("ModuleDesign", "HIS.Desktop.Plugins.Patient.gridViewPatientList.xml"));


        public UCListPatient(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;

        }

        private void gridPatientList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_PATIENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "DOB_DISPLAY")
                    {
                        try
                        {
                            string dob = (view.GetRowCellValue(e.ListSourceRowIndex, "DOB") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(dob));
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = dob.Substring(0, 4);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB_DISPLAY", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    //else if (e.Column.FieldName == "DOB_DISPLAY")
                    //{
                    //    try
                    //    {
                    //        string DOB = (view.GetRowCellValue(e.ListSourceRowIndex, "DOB") ?? "").ToString();
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(DOB));

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB", ex);
                    //    }
                    //}
                    else if (e.Column.FieldName == "IS_MALE_DISPLAY1")
                    {
                        try
                        {
                            string gender = (view.GetRowCellValue(e.ListSourceRowIndex, "GENDER_ID") ?? "").ToString();
                            if (gender == "1")
                                e.Value = "Nữ";
                            else if (gender == "2")
                                e.Value = "Nam";
                            else
                                e.Value = "Không xác định";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "SOCIAL_INSURANCE_NUMBER_STR")
                    {
                        try
                        {
                            string socialInsuranceNumber = (view.GetRowCellValue(e.ListSourceRowIndex, "SOCIAL_INSURANCE_NUMBER") ?? "").ToString();
                            if (!String.IsNullOrEmpty(socialInsuranceNumber))
                            {

                                e.Value = socialInsuranceNumber;

                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_HEIN_CARD_NUMBER_STR")
                    {
                        try
                        {
                            string heinCardNumber = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_HEIN_CARD_NUMBER") ?? "").ToString();
                            if (!String.IsNullOrEmpty(heinCardNumber))
                            {
                                e.Value = heinCardNumber;

                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }

                    gridViewPatientList.RefreshData();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rbtnHisPatient_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                try
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPatient").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisPatient'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + row.PATIENT_CODE, "Thông tin bệnh nhân - " + row.VIR_PATIENT_NAME + " (" + row.PATIENT_CODE + ")", (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                    }
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ButtonEditHeinCardInfo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                frmHeinCard frmHeinCard = new frmHeinCard(row.ID);
                frmHeinCard.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setDefaultControl()
        {
            try
            {

                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartDay() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtKeyWord.Text = "";
                txtMaYTeCode.Text = "";
                cboMaYTe.EditValue = null;
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                GridPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pageSize, gridControlPatientList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                setDefaultControl();
                //AnticipateListUCProcess.SearchAnticipateControl(this);
                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.Focus();
                btnSearch_Click(null, null);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS.Desktop.Plugins.Patient.Base.ADO_V_HIS_PATIENT>> apiResult = null;
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                SetFilter(ref filter);
                gridViewPatientList.BeginUpdate();

                gridViewPatientList.RefreshData();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<HIS.Desktop.Plugins.Patient.Base.ADO_V_HIS_PATIENT>>
                    (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                List<HIS.Desktop.Plugins.Patient.Base.ADO_V_HIS_PATIENT> data_ = new List<Base.ADO_V_HIS_PATIENT>();
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            try
                            {

                                if (item.GENDER_ID == 1)
                                    item.IS_MALE_DISPLAY1 = "Nữ";
                                else if (item.GENDER_ID == 2)
                                    item.IS_MALE_DISPLAY1 = "Nam";
                                else
                                    item.IS_MALE_DISPLAY1 = "Không xác định";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(ex);
                            }
                            /////////////////
                            try
                            {
                                item.DOB_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(item.DOB.ToString()));
                                if (item.IS_HAS_NOT_DAY_DOB == 1)
                                {
                                    item.DOB_DISPLAY = item.DOB.ToString().Substring(0, 4);
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DOB_DISPLAY", ex);
                            }
                            /////////////
                            try
                            {
                                if (!String.IsNullOrEmpty(item.SOCIAL_INSURANCE_NUMBER))
                                {

                                    item.SOCIAL_INSURANCE_NUMBER_STR = item.SOCIAL_INSURANCE_NUMBER;

                                }
                                else
                                {
                                    item.SOCIAL_INSURANCE_NUMBER_STR = "";
                                }

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                            /////
                            try
                            {
                                
                                if (!String.IsNullOrEmpty(item.TDL_HEIN_CARD_NUMBER))
                                {
                                    item.TDL_HEIN_CARD_NUMBER_STR = item.TDL_HEIN_CARD_NUMBER;

                                }
                                else
                                {
                                    item.TDL_HEIN_CARD_NUMBER_STR = "";
                                }

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                            //////////
                            try
                            {
                                
                                item.MODIFY_TIME_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(item.MODIFY_TIME.ToString()));

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                            ////
                            try
                            {
                              
                                item.CREATE_TIME_DISPLAY = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(item.CREATE_TIME.ToString()));

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                            data_.Add(item);
                        }

                        gridControlPatientList.DataSource = data_;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlPatientList.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridControlPatientList.RefreshDataSource();
                gridViewPatientList.RefreshData();
                gridViewPatientList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilter(ref MOS.Filter.HisPatientViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else if (!string.IsNullOrEmpty(txtMaYTeCode.Text))
                {
                    string code = txtMaYTeCode.Text.Trim();
                    if (code.Length < 9 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000}", Convert.ToInt64(code));
                        txtMaYTeCode.Text = code;
                    }
                    filter.PERSON_CODE__EXACT = code;
                }
                else
                {
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (cboMaYTe.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(cboMaYTe.EditValue.ToString()) == 1)
                        {
                            filter.HAS_PERSON_CODE = true;
                        }
                        else
                        {
                            filter.HAS_PERSON_CODE = false;
                        }
                    }

                    filter.PATIENT_NAME = txtPatientName.Text.Trim();

                    if (medistock == null)
                    {
                        filter.CREATOR = LoggingName;
                    }
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                    if (cboCSTheoDoi.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(cboCSTheoDoi.EditValue.ToString()) == 2)
                        {
                            filter.OWN_BRANCH_ID__CONTAINS = WorkPlace.GetBranchId();
                        }
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboCSTheoDoi.EditValue.ToString()) == 3)
                        {
                            filter.OWN_BRANCH_ID__NOT_CONTAINS = WorkPlace.GetBranchId();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPatientProgram_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    TreatmentHistoryADO currentInput = new TreatmentHistoryADO();
                    currentInput.patientId = row.ID;
                    currentInput.patient_code = row.PATIENT_CODE;
                    listArgs.Add(currentInput);
                    CallModule callModule = new CallModule(CallModule.TreatmentHistory, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGPatientEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    //List<object> listArgs = new List<object>();
                    //listArgs.Add(row);
                    //listArgs.Add((RefeshReference)Search);
                    //CallModule callModule = new CallModule(CallModule.PatientUpdate, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.PatientUpdate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add((DelegateSelectData)Search);
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("extenceInstance is null");
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void btnSearchKey()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public void Search(object data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (btnSearch.Enabled)
                        {
                            btnSearch.Focus();
                            btnSearch_Click(null, null);
                        }
                        HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                        rm.Print();
                    }));
                }
                else
                {
                    if (btnSearch.Enabled)
                    {
                        btnSearch.Focus();
                        btnSearch_Click(null, null);
                    }
                    HIS.Desktop.Plugins.Library.PrintPatientUpdate.PrintPatientUpdateProcessor rm = new Library.PrintPatientUpdate.PrintPatientUpdateProcessor(data, this.currentModule.RoomId);
                    rm.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void Import()
        {
            try
            {
                if (btnImportPatient.Enabled)
                {
                    btnImportPatient.Focus();
                    btnImportPatient_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void Refesh()
        {
            try
            {
                if (btnRefesh.Enabled)
                {
                    btnRefesh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Save()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave.Focus();
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtKeyWord.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnListReq_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    V_HIS_PATIENT currentInput = new V_HIS_PATIENT();
                    currentInput.ID = row.ID;
                    currentInput.PATIENT_CODE = row.PATIENT_CODE;
                    currentInput.VIR_PATIENT_NAME = row.VIR_PATIENT_NAME;
                    listArgs.Add(currentInput);
                    CallModule callModule = new CallModule(CallModule.ServiceReqList, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__UCListPatient = new ResourceManager("HIS.Desktop.Plugins.Patient.Resources.Lang", typeof(UCListPatient).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.btnDownload.Text = Inventec.Common.Resource.Get.Value("UCListPatient.btnDownload.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCListPatient.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.btnImportPatient.Text = Inventec.Common.Resource.Get.Value("UCListPatient.btnImportPatient.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn34.ToolTip = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn34.ToolTip", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn35.ToolTip = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn35.ToolTip", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn40.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.cboCSTheoDoi.Properties.NullText = Inventec.Common.Resource.Get.Value("UCListPatient.cboCSTheoDoi.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.txtPatientName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPatient.txtPatientName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPatient.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.txtMaYTeCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPatient.txtMaYTeCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.cboMaYTe.Properties.NullText = Inventec.Common.Resource.Get.Value("UCListPatient.cboMaYTe.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("UCListPatient.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCListPatient.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCListPatient.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCListPatient.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCListPatient.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControlItem15.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UCListPatient.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource__UCListPatient, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCListPatient_Load(object sender, EventArgs e)
        {
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(KeyHid) != "1")
                {
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                setDefaultControl();
                LoadComboMaYTe();
                FillDataToGrid();
                logginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SetCaptionByLanguageKey();
                btnSave.Enabled = false;
                if (System.IO.File.Exists(this.fileName))
                {
                    gridViewPatientList.RestoreLayoutFromXml(fileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboMaYTe()
        {
            try
            {
                List<MaYTeADO> data = new List<MaYTeADO>();
                data.Add(new MaYTeADO(1, "Có"));
                data.Add(new MaYTeADO(2, "Không"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboMaYTe, data, controlEditorADO);

                List<MaYTeADO> dataCS = new List<MaYTeADO>();
                dataCS.Add(new MaYTeADO(1, "Tất cả"));
                dataCS.Add(new MaYTeADO(2, "Đang theo dõi"));
                dataCS.Add(new MaYTeADO(3, "Chưa theo dõi"));

                List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
                columnInfos2.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO2 = new ControlEditorADO("Name", "ID", columnInfos2, false, 250);
                ControlEditorLoader.Load(cboCSTheoDoi, dataCS, controlEditorADO2);

                cboCSTheoDoi.EditValue = 2;//#15982
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnImportPatient_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //WaitingManager.Show();
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = false;
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        WaitingManager.Show();
                        var import = new Inventec.Common.ExcelImport.Import();
                        error = new List<string>();
                        if (import.ReadFileExcel(ofd.FileName))
                        {
                            var ImpPatientListProcessor = import.Get<PatientADO>(0);
                            if (ImpPatientListProcessor != null && ImpPatientListProcessor.Count > 0)
                            {
                                this.listPatientImp = new List<PatientADO>();
                                var allow = ImpPatientListProcessor.Where(o =>
                                    !string.IsNullOrEmpty(o.VIR_PATIENT_NAME)
                                    && !string.IsNullOrEmpty(o.dobDateTime)
                                    && !string.IsNullOrEmpty(o.GENDER_CODE)
                                    ).ToList();
                                if (allow != null && allow.Count > 0)
                                {
                                    addPatientToProcessList(allow);
                                    //addPatientToProcessList(ImpPatientListProcessor);
                                    if (error.Count == 0)
                                    {
                                        if (this.listPatientImp != null && this.listPatientImp.Count > 0)
                                        {
                                            gridControlPatientList.BeginUpdate();
                                            gridControlPatientList.DataSource = null;
                                            gridControlPatientList.DataSource = this.listPatientImp;
                                            gridControlPatientList.EndUpdate();
                                        }
                                    }
                                    else
                                    {
                                        string message = "";
                                        for (int i = 0; i < error.Count; i++)
                                        {
                                            message = message + ", " + error[i];
                                        }
                                        WaitingManager.Hide();
                                        DevExpress.XtraEditors.XtraMessageBox.Show(message);
                                    }

                                    WaitingManager.Hide();
                                    btnSave.Enabled = true;
                                }
                            }
                            else
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string convertDateStringToDate(string date)
        {
            string rs = "";
            try
            {
                date.Replace(" ", "");
                int idx = date.LastIndexOf("/");
                string year = date.Substring(idx + 1);
                string monthdate = date.Substring(0, idx);
                idx = monthdate.LastIndexOf("/");
                monthdate.Remove(idx);
                idx = monthdate.LastIndexOf("/");
                string month = monthdate.Substring(idx + 1);
                string dateStr = monthdate.Substring(0, idx);
                if (month.Length < 2)
                {
                    month = "0" + month;
                }
                if (dateStr.Length < 2)
                {
                    dateStr = "0" + dateStr;
                }
                datetime = year + month + dateStr;
                datetime.Replace("/", "");
                rs = datetime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
            return rs;
        }

        private void addPatientToProcessList(List<PatientADO> PatientImports)
        {
            try
            {
                if (PatientImports == null || PatientImports.Count == 0)
                    return;
                PatientImports = PatientImports.ToList();
                foreach (var patientImport in PatientImports)
                {
                    //Truong DL
                    patientImp = new PatientADO();

                    int idx = patientImport.VIR_PATIENT_NAME.Trim().LastIndexOf(" ");

                    if (idx > -1)
                    {

                        patientImp.FIRST_NAME = patientImport.VIR_PATIENT_NAME.Trim().Substring(idx).Trim();
                        patientImp.LAST_NAME = patientImport.VIR_PATIENT_NAME.Trim().Substring(0, idx).Trim();
                    }
                    else
                    {
                        patientImp.FIRST_NAME = patientImport.VIR_PATIENT_NAME.Trim();
                        patientImp.LAST_NAME = "";
                    }

                    if (patientImport.ADDRESS.Length <= 200)
                    {
                        patientImp.ADDRESS = patientImport.ADDRESS;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu địa chỉ của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.COMMUNE_NAME.Length <= 100)
                    {
                        patientImp.COMMUNE_NAME = patientImport.COMMUNE_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu tên xã của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.PATIENT_STORE_CODE.Length <= 20)
                    {
                        patientImp.PATIENT_STORE_CODE = patientImport.PATIENT_STORE_CODE;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu mã lưu trữ của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }


                    if (patientImport.DISTRICT_NAME.Length <= 100)
                    {
                        patientImp.DISTRICT_NAME = patientImport.DISTRICT_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu huyện/quận của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }


                    //patientImport.VIR_ADDRESS = patientImport.ADDRESS;


                    if (patientImport.EMAIL.Length <= 100)
                    {
                        patientImp.EMAIL = patientImport.EMAIL;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu email của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.ETHNIC_NAME.Length <= 100)
                    {
                        patientImp.ETHNIC_NAME = patientImport.ETHNIC_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu dân tộc của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    string Year;

                    if (patientImport.dobDateTime.Length == 4)
                    {
                        Year = patientImport.dobDateTime + "0101000000";
                        patientImp.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(Year);
                        patientImp.IS_HAS_NOT_DAY_DOB = 1;
                    }
                    else if (patientImport.dobDateTime.Length >= 8 && patientImport.dobDateTime.Length <= 10)
                    {
                        string date = convertDateStringToDate(patientImport.dobDateTime);
                        patientImp.DOB = Inventec.Common.TypeConvert.Parse.ToInt64(date + "000000");
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu ngày sinh của bệnh nhân {0} không đúng", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.NATIONAL_NAME.Length <= 100)
                    {
                        patientImp.NATIONAL_NAME = patientImport.NATIONAL_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu quốc tịch của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.PROVINCE_NAME.Length <= 100)
                    {
                        patientImp.PROVINCE_NAME = patientImport.PROVINCE_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu tỉnh của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.PHONE.Length <= 20)
                    {
                        patientImp.PHONE = patientImport.PHONE;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu điện thoại của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.RELATIVE_ADDRESS.Length <= 200)
                    {
                        patientImp.RELATIVE_ADDRESS = patientImport.RELATIVE_ADDRESS;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu thông tin liên hệ của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.RELATIVE_NAME.Length <= 100)
                    {
                        patientImp.RELATIVE_NAME = patientImport.RELATIVE_NAME;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu người nhà của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.RELATIVE_TYPE.Length <= 50)
                    {
                        patientImp.RELATIVE_TYPE = patientImport.RELATIVE_TYPE;
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu quan hệ của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }


                    patientImp.PATIENT_CODE = patientImport.PATIENT_CODE;

                    if (patientImport.VIR_PATIENT_NAME.Length <= 101)
                    {
                        patientImp.VIR_PATIENT_NAME = patientImport.VIR_PATIENT_NAME;
                        //patientImp
                    }
                    else
                    {
                        error.Add(string.Format("Dữ liệu tên bệnh nhân của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.CAREER_CODE != null)
                    {
                        if (patientImport.CAREER_CODE.Length <= 2)
                        {
                            var career = GlobalConfig.careers.FirstOrDefault(o => o.CAREER_CODE == patientImport.CAREER_CODE);
                            if (career != null)
                            {
                                patientImp.CAREER_ID = career != null ? career.ID : 0;
                                //patientImp.CAREER_CODE = career != null ? career.CAREER_CODE : "";
                                patientImp.CAREER_NAME = career != null ? career.CAREER_NAME : "";
                            }
                        }
                        else
                            error.Add(string.Format("Dữ liệu nghề nghiệp của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.GENDER_CODE != null)
                    {
                        if (patientImport.GENDER_CODE.Length <= 2)
                        {

                            var gender = GlobalConfig.genders.FirstOrDefault(o => o.GENDER_CODE == patientImport.GENDER_CODE);
                            if (gender != null)
                            {
                                patientImp.GENDER_ID = gender != null ? gender.ID : 0;
                                //patientImp.GENDER_CODE = gender != null ? gender.GENDER_CODE : "";
                            }
                        }
                        else
                            error.Add(string.Format("Dữ liệu giới tính của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }

                    if (patientImport.MILITARY_RANK_CODE != null)
                    {
                        if (patientImport.MILITARY_RANK_CODE.Length <= 6)
                        {
                            var militaryRank = GlobalConfig.militatys.FirstOrDefault(o => o.MILITARY_RANK_CODE == patientImport.MILITARY_RANK_CODE);
                            if (militaryRank != null)
                            {
                                patientImp.MILITARY_RANK_ID = militaryRank != null ? militaryRank.ID : 0;
                                //patientImp.MILITARY_RANK_CODE = militaryRank != null ? militaryRank.MILITARY_RANK_CODE : "";
                            }
                        }
                        else
                            error.Add(string.Format("Dữ liệu quân hàm của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));

                    }


                    if (patientImport.WORK_PLACE != null)
                    {
                        if (patientImport.WORK_PLACE.Length <= 200)
                        {
                            var workPlace = GlobalConfig.workPlaces.FirstOrDefault(o => o.WORK_PLACE_NAME == patientImport.WORK_PLACE);
                            if (workPlace != null)
                            {
                                patientImp.WORK_PLACE_ID = workPlace != null ? workPlace.ID : 0;
                                patientImp.WORK_PLACE = workPlace != null ? workPlace.WORK_PLACE_NAME : "";
                                patientImp.WORK_PLACE_NAME = workPlace != null ? workPlace.WORK_PLACE_NAME : "";
                            }
                        }
                        else
                            error.Add(string.Format("Dữ liệu nơi làm việc của bệnh nhân {0} độ dài vượt quá ký tự cho phép", patientImp.VIR_PATIENT_NAME));
                    }


                    this.listPatientImp.Insert(0, patientImp);


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var createList = new List<HIS_PATIENT>();
                WaitingManager.Show();
                if (listPatientImp != null && listPatientImp.Count > 0)
                {
                    foreach (var item in listPatientImp)
                    {
                        var create = new HIS_PATIENT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(create, item);
                        createList.Add(create);
                    }

                    var rs = new BackendAdapter(param).Post<List<HIS_PATIENT>>("api/HisPatient/CreateList", ApiConsumers.MosConsumer, createList, param);
                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                        WaitingManager.Hide();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreatePatientProgram_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();

                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row.ID);
                    CallModule callModule = new CallModule(CallModule.HisPatientProgram, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnPrintPatientCard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                patient = new V_HIS_PATIENT();
                patient = (V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_THE_BENH_NHAN;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal enum PrintTypeCare
        {
            IN_THE_BENH_NHAN,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_THE_BENH_NHAN:
                        richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinterCare);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000178":
                        LoadInTheBenhNhan(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadInTheBenhNhan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                if (patient != null)
                {
                    MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178PDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(patient);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                        }
                    }
                    else
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuot phai load SCN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private V_HIS_PATIENT _PatientShowPopup { get; set; }

        private PatientPopupMenuProcessor patientPopupMenuProcessor;

        private void gridViewPatientList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    int rowHandle = gridViewPatientList.GetVisibleRowHandle(hi.RowHandle);
                    this._PatientShowPopup = new V_HIS_PATIENT();
                    this._PatientShowPopup = (V_HIS_PATIENT)gridViewPatientList.GetRow(rowHandle);
                    gridViewPatientList.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridViewPatientList.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }

                    this.patientPopupMenuProcessor = new PatientPopupMenuProcessor(this._PatientShowPopup, this.PatientMouseRight_Click, barManager1);
                    this.patientPopupMenuProcessor.InitMenu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Btn_EvenLog_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    HIS.Desktop.ADO.KeyCodeADO ado = new ADO.KeyCodeADO();
                    ado.patientCode = row.PATIENT_CODE;
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "PATIENT_CODE: " + row.PATIENT_CODE);
                    listArgs.Add(dataInit3);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    CallModule callModule = new CallModule(CallModule.EventLog, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                }
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_PatientInfo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    listArgs.Add((DelegateSelectData)Search);
                    CallModule callModule = new CallModule(CallModule.PatientInfo, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                    FillDataToGrid();

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLayMaYTe_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<HIS_PATIENT> listPatient = new List<HIS_PATIENT>();
                var dataSource = (List<V_HIS_PATIENT>)gridControlPatientList.DataSource;
                if (dataSource != null && dataSource.Count > 0)
                {
                    dataSource = dataSource.Where(o => string.IsNullOrEmpty(o.PERSON_CODE)).ToList();
                    if (dataSource != null && dataSource.Count > 0)
                    {
                        CommonParam param = new CommonParam();

                        var result = new BackendAdapter(param).Post<bool>("api/HisPatient/TakePersonCode", ApiConsumers.MosConsumer, dataSource.Select(o => o.ID), param);

                        if (result)
                        {
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();

                        #region Hien thi message thong bao
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                        SessionManager.ProcessTokenLost(param);
                        #endregion

                        //        HisCardFilter hisCardFilter = new HisCardFilter();
                        //        hisCardFilter.PATIENT_IDs = dataSource.Select(o => o.ID).ToList();

                        //        var hisCard = new BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumers.MosConsumer, hisCardFilter, param);

                        //        foreach (var item in dataSource)
                        //        {
                        //            HIS_PATIENT patient = new HIS_PATIENT();

                        //            HidPersonFilter personFilter = new HidPersonFilter();

                        //            personFilter.ADDRESS = item.ADDRESS;
                        //            //personFilter.BHYT_NUMBER = item.ADDRESS;
                        //            personFilter.BLOOD_ABO_CODE = item.BLOOD_ABO_CODE;
                        //            personFilter.BLOOD_RH_CODE = item.BLOOD_RH_CODE;
                        //            personFilter.BORN_PROVINCE_CODE = item.BORN_PROVINCE_CODE;
                        //            personFilter.BORN_PROVINCE_NAME = item.BORN_PROVINCE_NAME;
                        //            personFilter.CAREER_NAME = item.CAREER_NAME;
                        //            personFilter.CCCD_DATE = item.CCCD_DATE;
                        //            personFilter.CCCD_NUMBER = item.CCCD_NUMBER;
                        //            personFilter.CCCD_PLACE = item.CCCD_PLACE;
                        //            personFilter.CMND_DATE = item.CMND_DATE;
                        //            personFilter.CMND_NUMBER = item.CMND_NUMBER;
                        //            personFilter.CMND_PLACE = item.CMND_PLACE;
                        //            personFilter.COMMUNE_NAME = item.COMMUNE_NAME;
                        //            personFilter.DISTRICT_NAME = item.DISTRICT_NAME;
                        //            personFilter.DOB = item.DOB;
                        //            personFilter.EMAIL = item.EMAIL;
                        //            personFilter.ETHNIC_NAME = item.ETHNIC_NAME;
                        //            //personFilter.FATHER_CODE = item.FATHER_CODE;
                        //            personFilter.FATHER_NAME = item.FATHER_NAME;
                        //            personFilter.FIRST_NAME = item.FIRST_NAME;
                        //            personFilter.GENDER_ID = item.GENDER_ID;
                        //            personFilter.HOUSEHOLD_CODE = item.HOUSEHOLD_CODE;
                        //            personFilter.HOUSEHOLD_RELATION_NAME = item.HOUSEHOLD_RELATION_NAME;
                        //            personFilter.HT_ADDRESS = item.HT_ADDRESS;
                        //            personFilter.HT_COMMUNE_NAME = item.HT_COMMUNE_NAME;
                        //            personFilter.HT_DISTRICT_NAME = item.HT_DISTRICT_NAME;
                        //            personFilter.HT_PROVINCE_NAME = item.HT_PROVINCE_NAME;
                        //            personFilter.IS_HAS_NOT_DAY_DOB = item.IS_HAS_NOT_DAY_DOB == 1 ? true : false;
                        //            personFilter.LAST_NAME = item.LAST_NAME;
                        //            personFilter.MOBILE = item.MOBILE;
                        //            //personFilter.MOTHER_CODE = item.MOTHER_CODE;
                        //            personFilter.MOTHER_NAME = item.MOTHER_NAME;
                        //            personFilter.NATIONAL_NAME = item.NATIONAL_NAME;
                        //            //personFilter.NICK_NAME = item.NICK_NAME;
                        //            personFilter.PERSON_CODE = item.PERSON_CODE;
                        //            personFilter.PROVINCE_NAME = item.PROVINCE_NAME;
                        //            personFilter.PHONE = item.PHONE;
                        //            personFilter.RELATIVE_ADDRESS = item.RELATIVE_ADDRESS;
                        //            personFilter.RELATIVE_CMND_NUMBER = item.RELATIVE_CMND_NUMBER;
                        //            personFilter.RELATIVE_MOBILE = item.RELATIVE_MOBILE;
                        //            personFilter.RELATIVE_NAME = item.RELATIVE_NAME;
                        //            personFilter.RELATIVE_PHONE = item.RELATIVE_PHONE;
                        //            personFilter.RELATIVE_TYPE = item.RELATIVE_TYPE;
                        //            personFilter.RELIGION_NAME = item.RELIGION_NAME;
                        //            personFilter.VIR_PERSON_NAME = item.VIR_PATIENT_NAME;

                        //            if (hisCard != null && hisCard.Count > 0)
                        //            {
                        //                var card = hisCard.Where(o => o.PATIENT_ID == item.ID).ToList();
                        //                if (card != null && card.Count > 0)
                        //                {
                        //                    card = card.OrderByDescending(o => o.CREATE_TIME).ToList();
                        //                    personFilter.CARD_CODE = card.FirstOrDefault().CARD_CODE;
                        //                }
                        //            }

                        //            var result = new BackendAdapter(param).Post<List<HID_PERSON>>("api/HidPerson/Get", ApiConsumers.HidConsumer, personFilter, param);

                        //            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(patient, item);

                        //            if (result != null && result.Count == 1)
                        //            {
                        //                patient.PERSON_CODE = result.FirstOrDefault().PERSON_CODE;
                        //            }
                        //            else
                        //            {
                        //                HID_PERSON person = new HID_PERSON();

                        //                person.ADDRESS = item.ADDRESS;
                        //                //person.BHYT_NUMBER = item.ADDRESS;
                        //                person.BLOOD_ABO_CODE = item.BLOOD_ABO_CODE;
                        //                person.BLOOD_RH_CODE = item.BLOOD_RH_CODE;
                        //                person.BORN_PROVINCE_CODE = item.BORN_PROVINCE_CODE;
                        //                person.BORN_PROVINCE_NAME = item.BORN_PROVINCE_NAME;
                        //                person.CAREER_NAME = item.CAREER_NAME;
                        //                person.CCCD_DATE = item.CCCD_DATE;
                        //                person.CCCD_NUMBER = item.CCCD_NUMBER;
                        //                person.CCCD_PLACE = item.CCCD_PLACE;
                        //                person.CMND_DATE = item.CMND_DATE;
                        //                person.CMND_NUMBER = item.CMND_NUMBER;
                        //                person.CMND_PLACE = item.CMND_PLACE;
                        //                person.COMMUNE_NAME = item.COMMUNE_NAME;
                        //                person.DISTRICT_NAME = item.DISTRICT_NAME;
                        //                person.DOB = item.DOB;
                        //                person.EMAIL = item.EMAIL;
                        //                person.ETHNIC_NAME = item.ETHNIC_NAME;
                        //                person.FATHER_NAME = item.FATHER_NAME;
                        //                person.FIRST_NAME = item.FIRST_NAME;
                        //                person.GENDER_ID = item.GENDER_ID;
                        //                person.HOUSEHOLD_CODE = item.HOUSEHOLD_CODE;
                        //                person.HOUSEHOLD_RELATION_NAME = item.HOUSEHOLD_RELATION_NAME;
                        //                person.HT_ADDRESS = item.HT_ADDRESS;
                        //                person.HT_COMMUNE_NAME = item.HT_COMMUNE_NAME;
                        //                person.HT_DISTRICT_NAME = item.HT_DISTRICT_NAME;
                        //                person.HT_PROVINCE_NAME = item.HT_PROVINCE_NAME;
                        //                person.IS_HAS_NOT_DAY_DOB = item.IS_HAS_NOT_DAY_DOB;
                        //                person.LAST_NAME = item.LAST_NAME;
                        //                person.MOBILE = item.MOBILE;
                        //                person.MOTHER_NAME = item.MOTHER_NAME;
                        //                person.NATIONAL_NAME = item.NATIONAL_NAME;
                        //                person.PERSON_CODE = item.PERSON_CODE;
                        //                person.PROVINCE_NAME = item.PROVINCE_NAME;
                        //                person.PHONE = item.PHONE;
                        //                person.RELATIVE_ADDRESS = item.RELATIVE_ADDRESS;
                        //                person.RELATIVE_CMND_NUMBER = item.RELATIVE_CMND_NUMBER;
                        //                person.RELATIVE_MOBILE = item.RELATIVE_MOBILE;
                        //                person.RELATIVE_NAME = item.RELATIVE_NAME;
                        //                person.RELATIVE_PHONE = item.RELATIVE_PHONE;
                        //                person.RELATIVE_TYPE = item.RELATIVE_TYPE;
                        //                person.RELIGION_NAME = item.RELIGION_NAME;
                        //                person.VIR_PERSON_NAME = item.VIR_PATIENT_NAME;

                        //                var dataCreate = new BackendAdapter(param).Post<HID_PERSON>("api/HidPerson/Create", ApiConsumers.HidConsumer, person, param);
                        //                if (dataCreate != null)
                        //                {
                        //                    patient.PERSON_CODE = dataCreate.PERSON_CODE;
                        //                }

                        //                //WaitingManager.Hide();
                        //                //frmChoosePerson frm = new frmChoosePerson((DelegateSelectData)DelegatePerson, result, item);
                        //                //frm.ShowDialog();

                        //                //if (this.selectPerson != null)
                        //                //{
                        //                //    patient.PERSON_CODE = this.selectPerson.PERSON_CODE;
                        //                //}
                        //            }

                        //            listPatient.Add(patient);
                        //        }

                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không có bệnh nhân chưa có mã y tế", "Thông báo");
                    }
                }


                //if (listPatient != null && listPatient.Count > 0)
                //{
                //    WaitingManager.Show();
                //    bool success = false;
                //    CommonParam param = new CommonParam();
                //    var resultPatient = new BackendAdapter(param).Post<List<HIS_PATIENT>>("api/HisPatient/UpdatePersonCode", ApiConsumers.MosConsumer, listPatient, param);

                //    if (resultPatient != null)
                //    {
                //        success = true;
                //        FillDataToGrid();
                //    }
                //    WaitingManager.Hide();

                //    #region Hien thi message thong bao
                //    MessageManager.Show(this.ParentForm, param, success);
                //    #endregion

                //    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                //    SessionManager.ProcessTokenLost(param);
                //    #endregion
                //}
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }

        }

        public void DelegatePerson(object data)
        {
            try
            {
                this.selectPerson = new HID_PERSON();
                if (data != null)
                {
                    this.selectPerson = (HID_PERSON)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_PATIENT.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_PATIENT";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaYTeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtMaYTeCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMaYTe_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMaYTe.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPatientCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtPatientCode.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCSTheoDoi_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCSTheoDoi.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Follow_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    HIS_PATIENT data = new HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(data, row);
                    string _OWN_BRANCH_IDS = data.OWN_BRANCH_IDS;
                    if (!string.IsNullOrEmpty(_OWN_BRANCH_IDS))
                    {
                        List<string> str = _OWN_BRANCH_IDS.Split(',').ToList();
                        str.Remove(WorkPlace.GetBranchId().ToString());
                        data.OWN_BRANCH_IDS = string.Join(",", str);
                    }
                    else
                    {
                        data.OWN_BRANCH_IDS = WorkPlace.GetBranchId().ToString();
                    }
                    CommonParam param = new CommonParam();
                    bool success = false;

                    var rs = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/Follow", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__UnFollow_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_PATIENT)gridViewPatientList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    HIS_PATIENT data = new HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(data, row);
                    string _OWN_BRANCH_IDS = data.OWN_BRANCH_IDS;
                    if (!string.IsNullOrEmpty(_OWN_BRANCH_IDS))
                    {
                        List<string> str = _OWN_BRANCH_IDS.Split(',').ToList();
                        str.Remove(WorkPlace.GetBranchId().ToString());
                        data.OWN_BRANCH_IDS = string.Join(",", str);
                    }

                    CommonParam param = new CommonParam();
                    bool success = false;

                    var rs = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/Unfollow", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatientList_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string _OWN_BRANCH_IDS = (gridViewPatientList.GetRowCellValue(e.RowHandle, "OWN_BRANCH_IDS") ?? "").ToString().Trim();

                    if (e.Column.FieldName == "FOLLOW_STR")
                    {
                        if (!string.IsNullOrEmpty(_OWN_BRANCH_IDS))
                        {
                            List<string> str = _OWN_BRANCH_IDS.Split(',').ToList();
                            string ss = str.FirstOrDefault(p => WorkPlace.GetBranchId() == Inventec.Common.TypeConvert.Parse.ToInt64(p.Trim()));
                            if (!string.IsNullOrEmpty(ss))
                                e.RepositoryItem = repositoryItemButton__UnFollow;
                            else
                                e.RepositoryItem = repositoryItemButton__Follow;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Follow;
                        }
                    }

                    string IS_CREATE_UUID_FAIL_STR = (gridViewPatientList.GetRowCellValue(e.RowHandle, "IS_CREATE_UUID_FAIL") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "IS_CREATE_UUID_FAIL_STR")
                    {
                        if (IS_CREATE_UUID_FAIL_STR != null && IS_CREATE_UUID_FAIL_STR == "1")
                            e.RepositoryItem = repositoryItemButton_CheckCreateUUIDFail;
                        else
                            e.RepositoryItem = repositoryItemButton_NoneCreateUUIDFail;
                    }

                    string IS_UPDATE_UUID_FAIL_STR = (gridViewPatientList.GetRowCellValue(e.RowHandle, "IS_UPDATE_UUID_FAIL") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "IS_UPDATE_UUID_FAIL_STR")
                    {
                        if (IS_UPDATE_UUID_FAIL_STR != null && IS_UPDATE_UUID_FAIL_STR == "1")
                            e.RepositoryItem = repositoryItemButton_CheckUpdateUUIDFail;
                        else
                            e.RepositoryItem = repositoryItemButton_NoneUpdateUUIDFail;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatientList_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"));
                }
                gridViewPatientList.SaveLayoutToXml(this.fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPatientList_ColumnPositionChanged(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, "ModuleDesign"));
                }
                gridViewPatientList.SaveLayoutToXml(this.fileName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Info == null && e.SelectedControl == this.gridControlPatientList)
        //        {
        //            string text1 = "";
        //            string text2 = "";
        //            DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlPatientList.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
        //            GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
        //            if (info.Column.FieldName == "IS_CREATE_UUID_FAIL_STR")
        //            {
        //                if (info.InRowCell)
        //                {
        //                    if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
        //                    {
        //                        this.lastColumn = info.Column;
        //                        this.lastRowHandle = info.RowHandle;
        //                        text1 = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_PATIENT_CHK_IS_CREATE_UUID_FAIL_STR", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

        //                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text1);
        //                    }
        //                    e.Info = lastInfo;
        //                }
        //            }
        //            if (info.Column.FieldName == "IS_UPDATE_UUID_FAIL_STR")
        //            {
        //                if (info.InRowCell)
        //                {
        //                    if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
        //                    {
        //                        this.lastColumn = info.Column;
        //                        this.lastRowHandle = info.RowHandle;
        //                        text2 = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LIST_PATIENT_CHK_IS_UPDATE_UUID_FAIL_STR", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

        //                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text2);
        //                    }
        //                    e.Info = lastInfo;
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
