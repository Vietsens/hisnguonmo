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
using AutoMapper;
using DevExpress.XtraEditors.Controls;
//using DevExpress.Entity.Model.Metadata;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.AssignPrescription;
using HIS.Desktop.Plugins.MediReactSum.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000156.PDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediReactSum
{
    public partial class frmMediReactSum : HIS.Desktop.Utility.FormBase
    {
        IcdProcessor icdProcessor = null;
        UserControl ucIcd = null;

        V_HIS_MEDI_REACT_SUM currentMediReactSum = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId = 0;
        V_HIS_TREATMENT_2 treatment = null;
        private bool IsTreatmentList;
        private V_HIS_ROOM CurrentRoom;

        public frmMediReactSum(Inventec.Desktop.Common.Modules.Module module, long data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                this.treatmentId = data;
                //this.txtIcdSubCode.Enabled = false;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMediReactSum(Inventec.Desktop.Common.Modules.Module module, long data, bool isTreatmentList)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                this.treatmentId = data;
                this.IsTreatmentList = isTreatmentList;
                //this.txtIcdSubCode.Enabled = false;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                ado.DelegateNextFocus = TxtIcdSubCode;
                ado.Width = 374;
                ado.Height = 24;
                if (HisConfigCFG.AutoCheckIcd == "1")
                {
                    ado.AutoCheckIcd = true;
                }

                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.layoutControlUcIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void TxtIcdSubCode()
        {
            txtIcdSubCode.Focus();
            txtIcdSubCode.SelectAll();
        }

        private void frmMediReactSum_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    HisConfigCFG.LoadConfig();
                    InitUcIcd();
                    FillDataToGrid();
                    LoadTreatmentById();
                    ResetControlValue(true);
                    SetCaptionByLanguageKey();
                }
                CurrentRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId) ?? new V_HIS_ROOM();

                if (treatment.IS_PAUSE == 1 || IsTreatmentList)
                {
                    btnNew.Enabled = false;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentById()
        {
            try
            {
                HisTreatmentView2Filter treatFilter = new HisTreatmentView2Filter();
                treatFilter.ID = treatmentId;
                var listTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatFilter, null);
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    this.treatment = listTreatment.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                HisMediReactSumViewFilter MediReactSumViewFilter = new HisMediReactSumViewFilter();
                MediReactSumViewFilter.TREATMENT_ID = this.treatmentId;
                MediReactSumViewFilter.ORDER_DIRECTION = "DESC";
                MediReactSumViewFilter.ORDER_FIELD = "MODIFY_TIME";
                List<V_HIS_MEDI_REACT_SUM> listData = new List<V_HIS_MEDI_REACT_SUM>();
                listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_MEDI_REACT_SUM>>("api/HisMediReactSum/GetView", ApiConsumers.MosConsumer, MediReactSumViewFilter, new CommonParam());
                if (listData.Count > 0) currentMediReactSum = listData.First();
                gridControlMediReactSum.BeginUpdate();
                gridControlMediReactSum.DataSource = listData;
                gridControlMediReactSum.EndUpdate();
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValue(bool IsInit)
        {
            try
            {
                txtIcdText.Text = "";
                txtIcdSubCode.Text = "";
                this.currentMediReactSum = null;
                this.SetControlValueByMediReactSum(IsInit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetControlValueByMediReactSum(bool isInit)
        {
            try
            {

                icdProcessor.SetValue(ucIcd, null);
                icdProcessor.Reload(ucIcd, null);
                txtIcdText.Text = "";
                txtIcdSubCode.Text = "";
                if (this.currentMediReactSum != null)
                {
                    HIS.UC.Icd.ADO.IcdInputADO inputIcd = new HIS.UC.Icd.ADO.IcdInputADO();
                    inputIcd.ICD_CODE = this.currentMediReactSum.ICD_CODE;
                    inputIcd.ICD_NAME = this.currentMediReactSum.ICD_NAME;
                    this.icdProcessor.Reload(ucIcd, inputIcd);
                    txtIcdText.Text = this.currentMediReactSum.ICD_TEXT;
                    txtIcdSubCode.Text = this.currentMediReactSum.ICD_SUB_CODE;
                }
                else if (isInit)
                {
                    HIS.UC.Icd.ADO.IcdInputADO inputIcd = new HIS.UC.Icd.ADO.IcdInputADO();
                    inputIcd.ICD_CODE = this.treatment.ICD_CODE;
                    inputIcd.ICD_NAME = this.treatment.ICD_NAME;
                    this.icdProcessor.Reload(ucIcd, inputIcd);
                    txtIcdText.Text = this.treatment.ICD_TEXT;
                    txtIcdSubCode.Text = this.treatment.ICD_SUB_CODE;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                string strError = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                e.ErrorText = strError;
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdSubCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string currentValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                currentValue = currentValue.Trim();
                string strIcdNames = "";
                if (!String.IsNullOrEmpty(currentValue))
                {
                    string seperate = ";";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    List<string> arrWrongCodes = new List<string>();
                    string[] arrIcdExtraCodes = txtIcdSubCode.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format(ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            int startPositionWarm = 0;
                            int lenghtPositionWarm = txtIcdSubCode.Text.Length - 1;
                            if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            {
                                startPositionWarm = txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                                lenghtPositionWarm = arrWrongCodes[0].Length;
                            }
                            txtIcdSubCode.Focus();
                            txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                        }
                    }
                }
                SetCheckedIcdsToControl(txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text);
                txtIcdText.Text = processIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string processIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>().Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void gridViewMediReactSum_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_MEDI_REACT_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "ICD_MAIN")
                        {
                            try
                            {
                                e.Value = (!String.IsNullOrEmpty(data.ICD_NAME)) ? data.ICD_NAME : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediReactSum_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                var data = (V_HIS_MEDI_REACT_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    string departmentCode = (gridViewMediReactSum.GetRowCellValue(e.RowHandle, "DEPARTMENT_CODE") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = repositoryItemBtnDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteDisable;
                        }
                    }
                    else if (e.Column.FieldName == "VIEW_DETAIL")
                    {
                        if (!IsTreatmentList)
                        {
                            if (treatment.IS_PAUSE != 1 && departmentCode == CurrentRoom.DEPARTMENT_CODE)
                                e.RepositoryItem = repositoryItemBtnViewDetail;
                            else
                                e.RepositoryItem = repositoryItemBtnViewDetailDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnViewDetailDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediReactSum_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                this.currentMediReactSum = null;
                if (e.RowHandle >= 0)
                {
                    this.currentMediReactSum = (V_HIS_MEDI_REACT_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }

                SetControlValueByMediReactSum(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediReactSum_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                //this.currentMediReactSum = null;
                //if(gridViewMediReactSum.FocusedRowHandle >= 0)
                //		 {
                //		 this.currentMediReactSum = (V_HIS_MEDI_REACT_SUM)((IList)((BaseView)sender).DataSource)[gridViewMediReactSum.FocusedRowHandle];
                //		 }
                //SetControlValueByMediReactSum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled || this.treatmentId <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_MEDI_REACT_SUM data = new HIS_MEDI_REACT_SUM();
                data.TREATMENT_ID = this.treatmentId;
                if (this.currentMediReactSum != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDI_REACT_SUM>(data, this.currentMediReactSum);
                }
                var icdData = this.icdProcessor.GetValue(this.ucIcd);
                if (icdData != null && icdData is HIS.UC.Icd.ADO.IcdInputADO)
                {
                    data.ICD_CODE = ((HIS.UC.Icd.ADO.IcdInputADO)icdData).ICD_CODE;
                    data.ICD_NAME = ((HIS.UC.Icd.ADO.IcdInputADO)icdData).ICD_NAME;
                }
                data.ICD_TEXT = txtIcdText.Text;
                data.ICD_SUB_CODE = txtIcdSubCode.Text;
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                if (room != null)
                {
                    data.ROOM_ID = room.ID;
                    data.DEPARTMENT_ID = room.DEPARTMENT_ID;
                }

                if (this.currentMediReactSum != null)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Post<HIS_MEDI_REACT_SUM>("api/HisMediReactSum/Update", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Post<HIS_MEDI_REACT_SUM>("api/HisMediReactSum/Create", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                    }
                }
                if (success)
                {
                    FillDataToGrid();
                    ResetControlValue(false);
                    MessageManager.Show(this, param, success);
                }
                else MessageManager.Show(this, param, success);
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                ResetControlValue(false);
                ucIcd = null;
                icdProcessor.SetValue(ucIcd, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void bbtnRCNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnNew_Click(null, null);
        }

        private void bbtnRCChoiceIcd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    SecondaryIcdADO ado = new SecondaryIcdADO(RefreshChanDoanPhu, txtIcdSubCode.Text, txtIcdText.Text);
                    listArgs.Add(ado);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDataIcd(HIS_ICD icd)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshChanDoanPhu(string icdCodes, string icdNames)
        {
            try
            {
                if (!String.IsNullOrEmpty(icdNames))
                {
                    txtIcdText.Text = icdNames;
                }
                if (!String.IsNullOrEmpty(icdCodes))
                {
                    txtIcdSubCode.Text = icdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_HIS_MEDI_REACT_SUM)gridViewMediReactSum.GetFocusedRow();
                if (data != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        return;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisMediReactSum/Delete", ApiConsumers.MosConsumer, data.ID, param);
                    if (success)
                    {
                        FillDataToGrid();
                        ResetControlValue(false);
                        MessageManager.Show(this, param, success);
                    }
                    else
                        MessageManager.Show("Xử lý thất bại \n Lỗi khi xóa hoặc phản ứng đã thực hiện");
                    WaitingManager.Hide();
                    //param .Messages[0] = "Hello";

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuPhanUngThuoc_MPS000156, delegateRunPrintTemplte);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_MEDI_REACT_SUM)gridViewMediReactSum.GetFocusedRow();
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MediReactCreate").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MediReactCreate'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        listArgs.Add(row.ID);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                    }
                }
                else
                {
                    MessageManager.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.TaiKhoanKhongCoQuyenThucHienChucNang));
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegateRunPrintTemplte(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuPhanUngThuoc_MPS000156:
                            InPhieuPhanUngThuoc(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
            //List<ExeMediReactADO> expMestMediReact = new List<ExeMediReactADO>();
            //V_HIS_TREATMENT_2 currentTreatment = new V_HIS_TREATMENT_2();
            //V_HIS_ROOM thisRoom = BackendDataWorker.Get<V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
            //try
            //{
            //    var data = (V_HIS_MEDI_REACT_SUM)gridViewMediReactSum.GetFocusedRow();
            //    if (data == null)
            //    {
            //        return result;
            //    }
            //    WaitingManager.Show();
            //    HisMediReactViewFilter filter = new HisMediReactViewFilter();
            //    filter.MEDI_REACT_SUM_ID = data.ID;
            //    expMestMediReact = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<ExeMediReactADO>>("api/HisMediReact/GetView", ApiConsumers.MosConsumer, filter, null);
            //    HisTreatmentView2Filter treatmentView2filter = new HisTreatmentView2Filter();
            //    treatmentView2filter.ID = data.TREATMENT_ID;
            //    currentTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatmentView2filter, null).First();
            //    currentTreatment.ICD_MAIN_TEXT = data.ICD_NAME;
            //    List<ExeMediReactADO> expMestMediReact1 = (from r in expMestMediReact select new ExeMediReactADO(r)).ToList();
            //    MPS.Core.Mps000156.Mps000156RDO rdo = new MPS.Core.Mps000156.Mps000156RDO(expMestMediReact1, currentTreatment, thisRoom);
            //    WaitingManager.Hide();
            //    result = MPS.Printer.Run(printTypeCode, fileName, rdo);
            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //    result = false;
            //}
            //return result;

        }

        private void InPhieuPhanUngThuoc(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                List<ExeMediReactADO> expMestMediReact = new List<ExeMediReactADO>();
                V_HIS_TREATMENT_2 currentTreatment = new V_HIS_TREATMENT_2();
                V_HIS_ROOM thisRoom = BackendDataWorker.Get<V_HIS_ROOM>().First(o => o.ID == this.currentModule.RoomId);
                V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();

                var data = (V_HIS_MEDI_REACT_SUM)gridViewMediReactSum.GetFocusedRow();
                if (data == null)
                {
                    return;
                }
                WaitingManager.Show();
                HisMediReactViewFilter filter = new HisMediReactViewFilter();
                filter.MEDI_REACT_SUM_ID = data.ID;
                expMestMediReact = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<ExeMediReactADO>>("api/HisMediReact/GetView", ApiConsumers.MosConsumer, filter, null);
                HisTreatmentView2Filter treatmentView2filter = new HisTreatmentView2Filter();
                treatmentView2filter.ID = data.TREATMENT_ID;
                currentTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, treatmentView2filter, null).First();
                currentTreatment.ICD_NAME = data.ICD_NAME;
                List<MPS.Processor.Mps000156.PDO.ExeMediReactADO> expMestMediReact1 = (from r in expMestMediReact select new MPS.Processor.Mps000156.PDO.ExeMediReactADO(r)).ToList();

                if (expMestMediReact.Count() > 0)
                {
                    var executeTime = expMestMediReact.OrderByDescending(o => o.EXECUTE_TIME).FirstOrDefault().EXECUTE_TIME;
                    HisBedLogViewFilter bedLogViewFilter = new HisBedLogViewFilter();
                    bedLogViewFilter.TREATMENT_ID = data.TREATMENT_ID;
                    bedLogViewFilter.START_TIME_TO = executeTime;
                    var bedLogs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogViewFilter, null);
                    if (bedLogs != null && bedLogs.Count() > 0)
                    {
                        bedLogs = bedLogs.Where(o => o.FINISH_TIME == null || o.FINISH_TIME >= executeTime).ToList();
                        if (bedLogs != null && bedLogs.Count() > 0)
                        {
                            bedLog = bedLogs.OrderByDescending(o => o.START_TIME).FirstOrDefault();
                        }
                    }
                }


                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000156.PDO.Mps000156PDO rdo = new MPS.Processor.Mps000156.PDO.Mps000156PDO(expMestMediReact1, currentTreatment, thisRoom, bedLog);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData printdata = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(printdata);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
