using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignBed.Config;
using HIS.Desktop.Plugins.AssignBed.Validation;
using HIS.Desktop.Utility;
using HIS.UC.Icd.ADO;
using HIS.UC.PatientSelect;
using HIS.UC.SecondaryIcd;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignBed.AssignBed
{
    public partial class frmAssignBed : HIS.Desktop.Utility.FormBase
    {
        private void InitUC()
        {
            try
            {
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKey();
                this.isAutoCheckIcd = (HisConfigCFG.AutoCheckIcd == GlobalVariables.CommonStringTrue);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();

                UCIcdInit();
                UCIcdCauseInit();
                //UcDateInit();
                V_HIS_ROOM currentRoom = null;

                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                }
                if (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue && (currentRoom != null && currentRoom.IS_ALLOW_NO_ICD != 1))
                {
                    ValidationICD(10, 500, true);
                }
                else
                {
                    ValidationSingleControlWithMaxLength(txtIcdCode, false, 10);
                    ValidationSingleControlWithMaxLength(txtIcdMainText, false, 500);
                }
                ValidationSingleControlWithMaxLength(txtIcdCodeCause, false, 10);
                ValidationSingleControlWithMaxLength(txtIcdMainTextCause, false, 500);

                loadCauHinhIn();
                InitControlState();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ucIcdYhct
        private void InitUcIcdYhct()
        {
            try
            {
                icdYhctProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusOutYhct;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.LabelTextSize = 90;
                ado.Height = 24;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                ado.AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd") == "1";
                ado.LblIcdMain = "CĐ YHCT:";
                ado.ToolTipsIcdMain = "Chẩn đoán y học cổ truyền";
                ucIcdYhct = (UserControl)icdYhctProcessor.Run(ado);

                if (ucIcdYhct != null)
                {
                    this.pnIcdTranditional.Controls.Add(ucIcdYhct);
                    ucIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void NextForcusOutYhct()
        {
            try
            {
                if (subIcdYhctProcessor != null && ucSecondaryIcdYhct != null && ucSecondaryIcdYhct.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcdYhct);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdSubCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        subIcdYhctProcessor.FocusControl(ucSecondaryIcdYhct);
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCPatientSelect()
        {
            try
            {
                this.patientSelectProcessor = new PatientSelectProcessor();
                HIS.UC.PatientSelect.ADO.PatientSelectInitADO ado = new HIS.UC.PatientSelect.ADO.PatientSelectInitADO();
                ado.SelectedSingleRow = PatientSelectedChange;
                ado.IsInitForm = true;
                ado.RoomId = currentModule.RoomId;
                ado.TreatmentId = treatmentId;
                ado.IsShowColumnBedRoomName = true;
                //ado.IsAutoWidth = false;

                //ado.LanguageInputADO = new UC.PatientSelect.ADO.LanguageInputADO();
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColDob__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColDob__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColPatientName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColPatientName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColSTT__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColSTT__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColTreatmentCode__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColTreatmentCode__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColBedRoomName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColBedRoomName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //ado.LanguageInputADO.gridControlTreatmentBedRoom__grdColClassifyName__Caption = Inventec.Common.Resource.Get.Value("gridControlTreatmentBedRoom__grdColClassifyName__Caption", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ucPatientSelect = (UserControl)this.patientSelectProcessor.Run(ado);
                if (this.ucPatientSelect != null)
                {
                    this.pnlUCPanel.Controls.Clear();
                    this.pnlUCPanel.Controls.Add(this.ucPatientSelect);
                    this.ucPatientSelect.Dock = DockStyle.Fill;
                    this.patientSelectProcessor.LoadWithFilter(this.ucPatientSelect, workingAssignServiceADO.FilterFromBedRoomPartiral);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcdYhct()
        {
            try
            {
                var icdYhct = BackendDataWorker.Get<V_HIS_ICD>().Where(o => o.IS_TRADITIONAL == 1).ToList();
                subIcdYhctProcessor = new SecondaryIcdProcessor(new CommonParam(), icdYhct);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateGetIcdMain = GetIcdMainCodeYhct;
                ado.Width = 440;
                ado.TextSize = 80;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ YHCT phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán y học cổ truyền phụ";
                ado.TextNullValue = "Nhấn F1 để chọn bệnh";
                ado.ViewHisIcds = icdYhct;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcdYhct = (UserControl)subIcdYhctProcessor.Run(ado);

                if (ucSecondaryIcdYhct != null)
                {
                    this.pnSubIcdTranditional.Controls.Add(ucSecondaryIcdYhct);
                    ucSecondaryIcdYhct.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private string GetIcdMainCodeYhct()
        {
            string mainCode = "";
            try
            {
                if (this.icdYhctProcessor != null && this.ucIcdYhct != null)
                {
                    var icdValue = this.icdYhctProcessor.GetValue(this.ucIcdYhct);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        #endregion

        private void UCIcdInit()
        {
            try
            {
                DataToComboChuanDoanTD(cboIcds, this.currentIcds);
                chkEditIcd.Enabled = (HisConfigCFG.AutoCheckIcd != "2");

                //txtIcdCode.Focus();
                //txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DataToComboChuanDoanTD(CustomGridLookUpEditWithFilterMultiColumn cbo, List<HIS_ICD> data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);

                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(900, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 340;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCIcdCauseInit()
        {
            try
            {
                DataToComboChuanDoanTD(cboIcdsCause, this.currentIcds.Where(o => o.IS_CAUSE == 1)
                    .OrderBy(p => p.ICD_CODE).ToList());
                chkEditIcdCause.Enabled = (HisConfigCFG.AutoCheckIcd != "2");
                //txtIcdCode.Focus();
                //txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ValidationICD(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = Color.Maroon;

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCode;
                    icdMainRule.btnBenhChinh = cboIcds;
                    icdMainRule.txtMainText = txtIcdMainText;
                    icdMainRule.chkCheck = chkEditIcd;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProviderControl.SetValidationRule(txtIcdCode, icdMainRule);
                }
                else
                {
                    lciIcdText.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCode.ErrorText = "";
                    dxValidationProviderControl.RemoveControlError(txtIcdCode);
                    dxValidationProviderControl.SetValidationRule(txtIcdCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkPrint)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                            if (chkPrint.Checked)
                                chkPrintDocumentSigned.Checked = false;
                        }
                        else if (item.KEY == ControlStateConstant.chkSign)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == ControlStateConstant.chkPrintDocumentSigned)
                        {
                            chkPrintDocumentSigned.Checked = item.VALUE == "1";
                            if (chkPrintDocumentSigned.Checked)
                                chkPrint.Checked = false;
                        }

                        foreach (var phieu in lstLoaiPhieu)
                        {
                            if (item.KEY == phieu.ID)
                            {
                                phieu.Check = item.VALUE == "1";
                            }
                        }
                    }
                }
                //chkNotCheckService.Checked = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY_CHOOSING_WHEN_SEARCH) == "1";
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ValidationSingleControlWithMaxLength(Control control, bool isRequired, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule icdMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                icdMainRule.editor = control;
                icdMainRule.maxLength = maxLength;
                icdMainRule.IsRequired = isRequired;
                icdMainRule.ErrorType = ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(control, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (chkEditIcd.Checked)
                    outPut.ICD_NAME = txtIcdMainText.Text;
                else
                    outPut.ICD_NAME = cboIcds.Text;

                if (!String.IsNullOrEmpty(txtIcdCode.Text))
                {
                    outPut.ICD_CODE = txtIcdCode.Text;
                }

                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
