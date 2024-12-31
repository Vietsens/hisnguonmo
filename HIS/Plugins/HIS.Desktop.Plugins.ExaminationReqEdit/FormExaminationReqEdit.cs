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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Print;
using HIS.Desktop.Plugins.ExaminationReqEdit.Base;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.Filter;
using HIS.Desktop.Plugins.ExaminationReqEdit.Config;
using HIS.Desktop.Plugins.ExaminationReqEdit.Resources;

namespace HIS.Desktop.Plugins.ExaminationReqEdit
{
    public partial class FormExaminationReqEdit : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        System.Globalization.CultureInfo cultureLang;
        bool isPrintNow = false;
        HIS_SERVICE_REQ aExamServiceReq { get; set; }
        HIS_SERE_SERV aSereServ { get; set; }
        HIS_TREATMENT currentHisTreatment { get; set; }
        List<V_HIS_SERVICE_ROOM> currentServiceRoom { get; set; }
        List<V_HIS_SERVICE> currentExamServiceType { get; set; }

        long serviceReqId = 0;
        long roomID = -1;
        int positionHandleLeft = -1;
        short IS_TRUE = 1;
        #endregion

        #region Construct
        public FormExaminationReqEdit(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {

                InitializeComponent();

                this.Text = module.text;
                this.roomID = module.RoomId;

                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                aExamServiceReq = new HIS_SERVICE_REQ();
                aSereServ = new HIS_SERE_SERV();
                cboExamServiceType.Enabled = false;
                txtExamServiceTypeCode.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormExaminationReqEdit_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();

                HisConfigCFG.LoadConfig();

                LoadKeysFromlanguage();

                SetPrintTypeToMps();

                SetDefaultValueControl();

                FillDataToCbo();

                ValidExcuteRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__BTN_PRINT",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__BTN_SAVE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.btnSaveNPrint.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__BTN_SAVE_AND_PRINT",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciExamServiceType.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__LCI_EXAM_SERVICE_TYPE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciExecuteRoom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__LCI_EXECUTE_ROOM",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciStatus.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__LCI_STATUS",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__TXT_SERVICE_REQ_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__TXT_TREATMENT_CODE_NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.lciIntructionTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__LCI_INTRUCTION_TIME",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
                this.checkPriority.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_EXAMINATION_REQ_EDIT__CHECK_PRIORITY",
                    Resources.ResourceLanguageManager.LanguageFormExaminationReqEdit,
                    cultureLang);
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                this.txtServiceReqCode.Text = "";
                this.txtTreatmentCode.Text = "";
                this.txtExamServiceTypeCode.Text = "";
                this.cboExamServiceType.EditValue = "";
                this.txtExecuteRoomCode.Text = "";
                this.cboExecuteRoom.EditValue = "";
                this.labelName.Text = "";
                this.labelGender.Text = "";
                this.labelDOB.Text = "";
                this.labelAddress.Text = "";
                this.lblStatus.Text = "";
                this.dtIntructionTime.EditValue = null;
                this.checkPriority.Checked = false;
                btnPrint.Enabled = false;
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
                dxValidationProvider.RemoveControlError(txtExecuteRoomCode);

                currentExamServiceType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH).ToList();

                var executeRoomIsPauseEnclitic = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.IS_ACTIVE == 1 && (o.IS_PAUSE_ENCLITIC == null || o.IS_PAUSE_ENCLITIC != 1)).Select(s => s.ROOM_ID).ToList();

                var ActiveRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1).Select(s => s.ID).ToList();
                if (ActiveRoom != null && ActiveRoom.Count > 0)
                {
                    if (executeRoomIsPauseEnclitic != null && executeRoomIsPauseEnclitic.Count > 0)
                    {
                        ActiveRoom = ActiveRoom.Where(o => executeRoomIsPauseEnclitic.Contains(o)).ToList();
                    }
                    currentServiceRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL && ActiveRoom.Contains(o.ROOM_ID)).ToList();
                }
                else
                {
                    currentServiceRoom = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCbo()
        {
            try
            {
                WaitingManager.Show();
                LoadDataToCboExamServiceType();

                LoadDataToCbocboExecuteRoom();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataToCboExamServiceType()
        {
            try
            {
                cboExamServiceType.Properties.DataSource = currentExamServiceType;
                cboExamServiceType.Properties.DisplayMember = "SERVICE_NAME";
                cboExamServiceType.Properties.ValueMember = "ID";
                cboExamServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExamServiceType.Properties.ImmediatePopup = true;
                cboExamServiceType.ForceInitialize();
                cboExamServiceType.Properties.View.Columns.Clear();
                cboExamServiceType.Properties.PopupFormSize = new Size(350, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboExamServiceType.Properties.View.Columns.AddField("SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 0;
                aColumnCode.Width = 70;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboExamServiceType.Properties.View.Columns.AddField("SERVICE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 1;
                aColumnName.Width = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCbocboExecuteRoom()
        {
            try
            {
                cboExecuteRoom.Properties.DisplayMember = "ROOM_NAME";
                cboExecuteRoom.Properties.ValueMember = "ROOM_ID";
                cboExecuteRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboExecuteRoom.Properties.ImmediatePopup = true;
                cboExecuteRoom.ForceInitialize();
                cboExecuteRoom.Properties.View.Columns.Clear();
                cboExecuteRoom.Properties.PopupFormSize = new Size(350, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboExecuteRoom.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 0;
                aColumnCode.Width = 70;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboExecuteRoom.Properties.View.Columns.AddField("ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 1;
                aColumnName.Width = 280;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                dxValidationProvider.RemoveControlError(txtExecuteRoomCode);
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                SetFilter(ref filter);
                var apiresul = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiresul != null && apiresul.Count > 0)
                {
                    aExamServiceReq = apiresul.FirstOrDefault();
                    var stt = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().FirstOrDefault(o => o.ID == aExamServiceReq.SERVICE_REQ_STT_ID);
                    lblStatus.Text = stt != null ? stt.SERVICE_REQ_STT_NAME : "";
                    if (aExamServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && aExamServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                    {
                        dtIntructionTime.DateTime = DateTime.Now;
                    }
                    else if (aExamServiceReq.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH || (aExamServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH && aExamServiceReq.SERVICE_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL))
                    {
                        dtIntructionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(aExamServiceReq.INTRUCTION_TIME) ?? DateTime.Now;
                    }
                    labelName.Text = aExamServiceReq.TDL_PATIENT_NAME;
                    labelGender.Text = aExamServiceReq.TDL_PATIENT_GENDER_NAME;
                    labelDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(aExamServiceReq.TDL_PATIENT_DOB).ToString();
                    labelAddress.Text = aExamServiceReq.TDL_PATIENT_ADDRESS;

                    if (aExamServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    {
                        txtExecuteRoomCode.Enabled = false;
                        cboExecuteRoom.Enabled = false;
                        dtIntructionTime.Enabled = false;
                        checkPriority.Enabled = false;
                    }
                    else
                    {
                        txtExecuteRoomCode.Enabled = true;
                        cboExecuteRoom.Enabled = true;
                        dtIntructionTime.Enabled = true;
                        checkPriority.Enabled = true;
                    }

                    MOS.Filter.HisTreatmentFilter treatFilter = new MOS.Filter.HisTreatmentFilter();
                    treatFilter.ID = aExamServiceReq.TREATMENT_ID;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(Base.GlobalStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, treatFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (treatment != null && treatment.Count > 0)
                    {
                        currentHisTreatment = treatment.FirstOrDefault();
                    }

                    var executeRoom = currentServiceRoom.FirstOrDefault(o => o.ROOM_ID == aExamServiceReq.EXECUTE_ROOM_ID);

                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.SERVICE_REQ_ID = aExamServiceReq.ID;
                    var sereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(Base.GlobalStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (sereServ != null && sereServ.Count > 0)
                    {
                        aSereServ = sereServ.FirstOrDefault();
                        var serviceTpye = currentExamServiceType.Where(o => o.ID == aSereServ.SERVICE_ID).FirstOrDefault();
                        if (serviceTpye != null)
                        {
                            cboExamServiceType.EditValue = serviceTpye.ID;
                            txtExamServiceTypeCode.Text = serviceTpye.SERVICE_CODE;

                            cboExamServiceType.Properties.NullText = "";
                            cboExecuteRoom.Properties.NullText = "";

                            var rooms = currentServiceRoom.Where(o => o.SERVICE_ID == serviceTpye.ID).ToList();

                            cboExecuteRoom.Properties.DataSource = rooms;
                            if (executeRoom != null)
                            {
                                cboExecuteRoom.EditValue = executeRoom.ROOM_ID;
                                txtExecuteRoomCode.Text = executeRoom.ROOM_CODE;
                            }
                            else
                            {
                                cboExecuteRoom.EditValue = null;
                                txtExecuteRoomCode.Text = "";
                            }
                            serviceReqId = aExamServiceReq.ID;
                            btnPrint.Enabled = true;
                            btnSave.Enabled = true;
                            btnSaveNPrint.Enabled = true;
                        }
                        else
                        {
                            cboExamServiceType.Properties.NullText = aSereServ.TDL_SERVICE_NAME;
                            cboExamServiceType.EditValue = null;
                            txtExamServiceTypeCode.Text = aSereServ.TDL_SERVICE_CODE;

                            cboExecuteRoom.EditValue = null;
                            cboExecuteRoom.Properties.NullText = executeRoom.ROOM_NAME;
                            txtExecuteRoomCode.Text = executeRoom.ROOM_CODE;

                            txtExecuteRoomCode.Enabled = false;
                            cboExecuteRoom.Enabled = false;
                            dtIntructionTime.Enabled = false;
                            checkPriority.Enabled = false;
                            btnPrint.Enabled = false;
                            btnSave.Enabled = false;
                            btnSaveNPrint.Enabled = false;

                            MessageBox.Show(Resources.ResourceMessage.YeuCauKhongPhaiLaKham);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.MaYeuCauHoacMaDieuTriKhongHopLe);
                    lblStatus.Text = "";
                    cboExamServiceType.EditValue = "";
                    cboExecuteRoom.EditValue = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisServiceReqFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "CREATE_TIME";
                filter.ORDER_FIELD = "DESC";
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = code;
                }
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
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

        private void ValidExcuteRoom()
        {
            try
            {
                Validation.ExcuteRoomValidationRule excuteRoomRule = new Validation.ExcuteRoomValidationRule();
                excuteRoomRule.txtExecuteRoomCode = txtExecuteRoomCode;
                excuteRoomRule.cboExecuteRoom = cboExecuteRoom;
                excuteRoomRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                excuteRoomRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtExecuteRoomCode, excuteRoomRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleLeft == -1)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleLeft > edit.TabIndex)
                {
                    positionHandleLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region click button
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Message.WaitingManager.Show();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text) || !String.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    if (checkDigit(txtTreatmentCode.Text) || checkDigit(txtServiceReqCode.Text))
                        FillDataToControl();
                    else
                    {
                        Inventec.Desktop.Common.Message.WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.MaYeuCauHoacMaDieuTriKhongHopLe);
                    }
                }
                else
                {
                    lblStatus.Text = "";
                    cboExamServiceType.EditValue = "";
                    cboExecuteRoom.EditValue = "";
                    dtIntructionTime.EditValue = null;
                    checkPriority.Checked = false;
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandleLeft = -1;
                if (!dxValidationProvider.Validate()) return;
                if (CheckTime(currentHisTreatment)) return;
                if (CheckMaxReqBhytByDay()) return;
                if (aSereServ != null && aSereServ.ID > 0)
                {
                    // kiểm tra trạng thái của yêu cầu
                    if (this.aExamServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                    {
                        if (MessageBox.Show(ResourceMessage.YeuCauDangXuLy, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    WaitingManager.Show();
                    success = SaveProcess(ref param);
                    WaitingManager.Hide();
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private bool CheckTime(MOS.EFMODEL.DataModels.HIS_TREATMENT currentHisTreatment)
        {
            bool result = false;
            try
            {
                if (currentHisTreatment != null)
                {
                    var dt = this.dtIntructionTime.DateTime.ToString("yyyyMMddHHmmss");
                    if (currentHisTreatment.IN_TIME > Inventec.Common.TypeConvert.Parse.ToInt64(dt))
                    {
                        MessageBox.Show("thời gian chỉ định không được nhỏ hơn thời gian vào viện");
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }

        private bool CheckMaxReqBhytByDay()
        {
            bool result = false;
            long? OverExam = null;
            long executeRoomId = 0;
            try
            {
                if (cboExecuteRoom.EditValue != null)
                {
                    executeRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? "").ToString());
                    var executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == executeRoomId);
                    var warningCFG = HisConfigs.Get<string>(GlobalStore.CONFIG_KEY__WARNING_OVER_EXAM__BHYT);
                    if (warningCFG == "1" && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && this.currentHisTreatment.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        // lấy về số lượt khám
                        CommonParam param = new CommonParam();
                        HisSereServBhytOutpatientExamFilter hisSereServBhytOutpatientExamFilter = new HisSereServBhytOutpatientExamFilter();
                        hisSereServBhytOutpatientExamFilter.ROOM_IDs = new List<long>() { executeRoomId };
                        if (this.dtIntructionTime.EditValue != null && this.dtIntructionTime.DateTime != DateTime.MinValue)
                        {
                            hisSereServBhytOutpatientExamFilter.INTRUCTION_DATE = Convert.ToInt64(this.dtIntructionTime.DateTime.ToString("yyyyMMdd") + "000000");
                        }
                        var hisSereServBhytOutPatientExams = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>(GlobalStore.HIS_SERE_SERV__GET_SERE_SERV_BHYT_OUT_PATIENT_EXAM, ApiConsumer.ApiConsumers.MosConsumer, hisSereServBhytOutpatientExamFilter, param);
                        if (hisSereServBhytOutPatientExams != null && hisSereServBhytOutPatientExams.Count > 0)
                        {
                            OverExam = (hisSereServBhytOutPatientExams.Count - executeRoom.MAX_REQ_BHYT_BY_DAY ?? 0);
                            if (OverExam >= 0 && MessageBox.Show(String.Format("Phòng {0} đã vượt quá {1} lượt khám BHYT trong ngày. Bạn có muốn thực hiện không?", executeRoom.EXECUTE_ROOM_NAME, executeRoom.MAX_REQ_BHYT_BY_DAY ?? 0), "Cảnh báo",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool SaveProcess(ref CommonParam param)
        {
            bool result = false;
            try
            {
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ data = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();

                SetDataSave(ref data);
                var apiresult = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(Base.GlobalStore.HIS_SERVICE_REQ_CHANGEROOM, ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiresult != null)
                {
                    result = true;
                    serviceReqId = apiresult.ID;
                    btnPrint.Enabled = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetDataSave(ref MOS.EFMODEL.DataModels.HIS_SERVICE_REQ data)
        {
            try
            {
                data.REQUEST_ROOM_ID = roomID;
                data.ID = aSereServ.SERVICE_REQ_ID ?? 0;
                if (checkPriority.Checked == true) data.PRIORITY = Base.GlobalStore.HAS_PRIORITY;
                data.EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboExecuteRoom.EditValue.ToString());
                String dt = dtIntructionTime.DateTime.ToString("yyyyMMddHHmm");
                data.INTRUCTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dt + "00");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    this.isPrintNow = false;
                    PrintProcess();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveNPrint_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandleLeft = -1;
                if (!dxValidationProvider.Validate()) return;
                if (CheckMaxReqBhytByDay()) return;
                if (aSereServ != null && aSereServ.ID > 0)
                {
                    // kiểm tra trạng thái của yêu cầu
                    if (this.aExamServiceReq.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                    {
                        if (MessageBox.Show(ResourceMessage.YeuCauDangXuLy, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }
                    WaitingManager.Show();
                    success = SaveProcess(ref param);
                    if (success == true)
                    {
                        isPrintNow = true;
                        PrintProcess();
                    }
                    WaitingManager.Hide();
                }
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
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
                SetDefaultValueControl();
                btnSave.Enabled = true;
                btnSaveNPrint.Enabled = true;
                txtExecuteRoomCode.Enabled = true;
                cboExecuteRoom.Enabled = true;
                dtIntructionTime.Enabled = true;
                checkPriority.Enabled = true;
                cboExamServiceType.Properties.NullText = "";
                cboExecuteRoom.Properties.NullText = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Print
        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(HIS.Desktop.Print.PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqId != 0)
                {
                    WaitingManager.Show();
                    MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new MOS.Filter.HisServiceReqViewFilter();
                    serviceReqFilter.ID = serviceReqId;
                    var serviceReqResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>(ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (serviceReqResult != null && serviceReqResult.Count > 0)
                    {
                        V_HIS_SERVICE_REQ serviceReq = serviceReqResult.First();
                        //V_HIS_SERVICE_REQ serviceReq = new V_HIS_SERVICE_REQ();
                        //Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(serviceReq, serviceReqResult.FirstOrDefault());
                        //if (serviceReq == null)
                        //    throw new ArgumentNullException("serviceReq is null");

                        //var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.EXECUTE_ROOM_ID);
                        //if (room != null)
                        //{
                        //    serviceReq.EXECUTE_DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                        //    serviceReq.EXECUTE_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        //    serviceReq.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        //    serviceReq.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        //}

                        //var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == serviceReq.REQUEST_ROOM_ID);
                        //if (reqRoom != null)
                        //{
                        //    serviceReq.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                        //    serviceReq.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                        //    serviceReq.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                        //    serviceReq.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                        //}

                        var lstSereServ = new List<MPS.Processor.Mps000001.PDO.Mps000001_ListSereServs>();
                        var sereServ11 = GetSereServByServiceReqId(serviceReq.ID);
                        if (sereServ11 != null && sereServ11.Count > 0)
                        {
                            foreach (var item in sereServ11)
                            {
                                lstSereServ.Add(new MPS.Processor.Mps000001.PDO.Mps000001_ListSereServs(item));
                            }
                        }

                        var vHisBhyt = getPatientTypeAlter(serviceReq.TREATMENT_ID, 0);

                        //Mức hưởng BHYT
                        string ratio_text = "";
                        if (vHisBhyt != null)
                        {
                            string levelCode = Base.GlobalStore.HEIN_LEVEL_CODE__CURRENT;
                            ratio_text = GetDefaultHeinRatioForView(vHisBhyt.HEIN_CARD_NUMBER, vHisBhyt.HEIN_TREATMENT_TYPE_CODE, levelCode, vHisBhyt.RIGHT_ROUTE_CODE);

                        }

                        var hisTreatment = new HIS_TREATMENT();
                        MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                        treatmentFilter.ID = serviceReq.TREATMENT_ID;
                        var lstTreatment = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                        if (lstTreatment != null && lstTreatment.Count > 0)
                        {
                            hisTreatment = lstTreatment.FirstOrDefault();
                        }

                        var firstExam = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == hisTreatment.TDL_FIRST_EXAM_ROOM_ID);
                        string firstExamRoomName = firstExam != null ? firstExam.ROOM_NAME : "";
                        var patient = PrintGlobalStore.GetPatientById(serviceReq.TDL_PATIENT_ID);
                        var tranInReason = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == hisTreatment.TRANSFER_IN_REASON_ID);
                        var inroom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == hisTreatment.IN_ROOM_ID);
                        MPS.Processor.Mps000001.PDO.Mps000001ADO ado = new MPS.Processor.Mps000001.PDO.Mps000001ADO();
                        ado.firstExamRoomName = firstExamRoomName;
                        ado.ratio_text = ratio_text;
                        ado.TRANSFER_IN_REASON_NAME = tranInReason != null ? tranInReason.TRAN_PATI_REASON_NAME : "";
                        ado.IN_DEPARTMENT_NAME = inroom != null ? inroom.DEPARTMENT_NAME : "";
                        ado.IN_ROOM_NAME = inroom != null ? inroom.ROOM_NAME : "";

                        MPS.Processor.Mps000001.PDO.Mps000001PDO mps000001RDO = new MPS.Processor.Mps000001.PDO.Mps000001PDO(
                            serviceReq,
                            vHisBhyt,
                            patient,
                            lstSereServ,
                            hisTreatment,
                            ado);

                        string printerName = "";
                        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                        {
                            printerName = GlobalVariables.dicPrinter[printTypeCode];
                        }
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;

                        if (isPrintNow)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    WaitingManager.Hide();
                    isPrintNow = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER getPatientTypeAlter(long treatmentId, long instructTime)
        {
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new MOS.Filter.HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentId;
                if (instructTime > 0)
                    hisPTAlterFilter.InstructionTime = instructTime;
                else
                    hisPTAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHispatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(ApiConsumer.HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumer.ApiConsumers.MosConsumer, hisPTAlterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            }
            catch (Exception ex)
            {
                currentHispatientTypeAlter = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentHispatientTypeAlter;
        }

        private string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(long serviceReqId)
        {
            List<V_HIS_SERE_SERV> result = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.SERVICE_REQ_ID = serviceReqId;
                var result1 = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_12>>(Base.GlobalStore.HIS_SERE_SERV_GETVIEW_12, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (result1 != null && result1.Count > 0)
                {
                    result1 = result1.Where(o => o.IS_NO_EXECUTE != IS_TRUE).ToList();
                    result = new List<V_HIS_SERE_SERV>();
                    foreach (var item in result1)
                    {
                        V_HIS_SERE_SERV ss11 = new V_HIS_SERE_SERV();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(ss11, item);

                        var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss11.SERVICE_ID);
                        if (service != null)
                        {
                            ss11.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            ss11.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            ss11.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            ss11.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            ss11.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            ss11.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        }

                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                        if (room != null)
                        {
                            ss11.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                            ss11.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                            ss11.EXECUTE_DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                            ss11.EXECUTE_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        }
                        result.Add(ss11);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        #endregion

        #region enter
        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtServiceReqCode.Text))
                    {
                        btnSearch_Click(null, null);
                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExamServiceTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtExamServiceTypeCode.Text.Trim()))
                    {
                        string code = txtExamServiceTypeCode.Text.Trim().ToLower();
                        var listData = currentExamServiceType.Where(o => o.SERVICE_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SERVICE_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtExamServiceTypeCode.Text = result.First().SERVICE_CODE;
                            cboExamServiceType.EditValue = result.First().ID;
                            var data = currentServiceRoom.Where(o => o.SERVICE_ID == result.First().ID).ToList();
                            cboExecuteRoom.Properties.DataSource = data;
                            txtExecuteRoomCode.Focus();
                            txtExecuteRoomCode.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboExamServiceType.Focus();
                        cboExamServiceType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamServiceType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboExamServiceType.EditValue != null)
                    {
                        var service = currentExamServiceType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()));
                        if (service != null)
                        {
                            txtExamServiceTypeCode.Text = service.SERVICE_CODE;
                            var data = currentServiceRoom.Where(o => o.SERVICE_ID == service.ID).ToList();
                            cboExecuteRoom.Properties.DataSource = data;
                            txtExecuteRoomCode.Focus();
                            txtExecuteRoomCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamServiceType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var service = currentExamServiceType.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExamServiceType.EditValue ?? 0).ToString()));
                    if (service != null)
                    {
                        txtExamServiceTypeCode.Text = service.SERVICE_CODE;
                        var data = currentServiceRoom.Where(o => o.SERVICE_ID == service.ID).ToList();
                        cboExecuteRoom.Properties.DataSource = data;
                        txtExecuteRoomCode.Focus();
                        txtExecuteRoomCode.SelectAll();
                    }
                }
                if (e.KeyCode == Keys.Down)
                {
                    cboExamServiceType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExecuteRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtExecuteRoomCode.Text.Trim()))
                    {
                        string code = txtExecuteRoomCode.Text.Trim().ToLower();
                        var listData = currentServiceRoom.Where(o => o.ROOM_CODE.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ROOM_CODE.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 1)
                        {
                            showCbo = false;
                            txtExecuteRoomCode.Text = result.First().ROOM_CODE;
                            cboExecuteRoom.EditValue = result.First().ROOM_ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (showCbo)
                    {
                        cboExecuteRoom.Focus();
                        cboExecuteRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down) cboExecuteRoom.ShowPopup();
                else if (e.KeyCode == Keys.Enter)
                {
                    var ExecuteRoom = currentServiceRoom.FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                    if (ExecuteRoom != null)
                    {
                        txtExecuteRoomCode.Text = ExecuteRoom.ROOM_CODE;
                        dtIntructionTime.Focus();
                        dtIntructionTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    var ExecuteRoom = currentServiceRoom.FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExecuteRoom.EditValue ?? 0).ToString()));
                    if (ExecuteRoom != null)
                    {
                        txtExecuteRoomCode.Text = ExecuteRoom.ROOM_CODE;
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) checkPriority.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region shotcut
        private void barButtonItemPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled) btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtServiceReqCode.Focus();
                txtServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSaveNPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveNPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
