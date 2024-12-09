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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core.RelaytionType;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using HIS.UC.Sick.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.Sick
{
    public partial class UCSick : UserControl
    {
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment;
        private List<V_HIS_DOCUMENT_BOOK> lstDocumentBook = null;
        private DelegateNextFocus NextFocus;
        private int positionHandle = -1;
        bool IsDuongThai;
        long? workPlaceId;
        string SocialNumber;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        HIS_PATIENT Patient { get; set; }
        public UCSick()
        {
            InitializeComponent();
        }

        public UCSick(ADO.SickInitADO data)
            : this()
        {
            try
            {
                InitComboUser();
                if (data != null)
                {
                    this.hisTreatment = data.CurrentHisTreatment;
                    this.currentModule = data.currentModule;
                    this.IsDuongThai = data.IsDuongThai;
                    this.NextFocus = data.DelegateNextFocus;
                    this.lstDocumentBook = data.ListDocumentBook;
                    this.workPlaceId = data.WorkPlaceId;
                    FillDataToForm(this.hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCSick_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.IsDuongThai)
                {
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciBtnBaby.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciDocumentBook.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem13.AppearanceItemCaption.ForeColor = Color.Black;
                    layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciTreatmentMethod.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lcidtePregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutControlItem6.TextSize = new Size(100, 20);
                    layoutControlItem6.TextToControlDistance = 5;
                    dxValidationProvider1.SetValidationRule(memTreatmentMethod, null);
                }
                else
                {
                    layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciBtnBaby.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciDocumentBook.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.LoadComboDocumentBook();
                    layoutControlItem13.AppearanceItemCaption.ForeColor = Color.Maroon;
                    layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ValidateBHXH();
                    lcidtePregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciTreatmentMethod.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciPregnancyTerminationReason.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    lciTreatmentMethod.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    lciIsPregnancyTermination.MinSize = new Size(lciIsPregnancyTermination.MinSize.Width, 50);
                    lciTreatmentMethod.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidationSingleControl(memTreatmentMethod, dxValidationProvider1, null, null);
                }

                LoadComboGender();
                InitComboRelaytionType();
                LoadComboEthnic();
                InitComboWorkPlace();
                LoadKeysFromlanguage();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateBHXH()
        {
            try
            {
                ValidateNullText txt = new ValidateNullText();
                txt.txt = txtSocialInsuranceNumber;
                dxValidationProvider1.SetValidationRule(txtSocialInsuranceNumber, txt);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FocusWhileSelectedUser()
        {
            try
            {
                spinSickLeaveDay.Focus();
                spinSickLeaveDay.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.LanguageUCSick, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        internal bool ValidateControl()
        {
            return dxValidationProvider1.Validate();
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(cboWorkPlace, dxValidationProvider1, GetErrorValidWorkPlace, IsValidWorkPlace);
                ValidateGridLookupWithTextEdit(cboUser, txtLoginName, dxValidationProvider1);
                ValidationMaxLength(txtRelativeName, 100);
                ValidMaxLengthRequired(false, memPregnancyTerminationReason, 1000, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidMaxLengthRequired(bool IsRequied, BaseControl control, int maxlength, string tooltip)
        {
            try
            {
                ValidateMaxLength valid = new ValidateMaxLength();
                valid.tooltip = tooltip;
                valid.IsRequired = IsRequied;
                valid.txt = control;
                valid.maxLength = maxlength;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string GetErrorValidWorkPlace()
        {
            string errorText = "";
            if (!IsValidWorkPlace())
            {
                errorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            }
            else if (!String.IsNullOrEmpty(txtWorkPlace.Text) && Encoding.UTF8.GetByteCount(txtWorkPlace.Text) > 100)
            {
                errorText = "Vượt quá độ dài cho phép (" + 100 + ")";
            }
            return errorText;
        }

        bool IsValidWorkPlace()
        {
            bool valid = true;
            try
            {
                valid = (cboWorkPlace.EditValue != null || !String.IsNullOrEmpty(txtWorkPlace.Text.Trim()));
                Inventec.Common.Logging.LogSystem.Debug("IsValidWorkPlace:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        private void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, GetMessageErrorValidControl getMessageErrorValidControl, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isUseOnlyCustomValidControl = true;
                    validRule.isValidControl = isValidControl;
                }
                if (getMessageErrorValidControl != null)
                    validRule.ErrorText = getMessageErrorValidControl();
                else
                    validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(Control control, int? maxLength)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboGender()
        {
            try
            {
                var gender = BackendDataWorker.Get<HIS_GENDER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(Res_GridLookUpEdit, gender, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitComboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS.EFMODEL.DataModels.ACS_USER>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);
                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS.EFMODEL.DataModels.ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

                //Nguoi chi dinh
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboUser, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboDocumentBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_CODE", "", 80, 1, true));
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_NAME", "", 170, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_BOOK_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboDocumentBook, lstDocumentBook, controlEditorADO);
                if (lstDocumentBook != null && lstDocumentBook.Count > 0)
                {
                    if (this.hisTreatment.DOCUMENT_BOOK_ID.HasValue && lstDocumentBook.Any(a => a.ID == this.hisTreatment.DOCUMENT_BOOK_ID.Value))
                    {
                        cboDocumentBook.EditValue = this.hisTreatment.DOCUMENT_BOOK_ID.Value;
                    }
                    else if (lstDocumentBook.Count == 1)
                    {
                        cboDocumentBook.EditValue = lstDocumentBook[0].ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRelaytionType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Name", columnInfos, false, 250);
                ControlEditorLoader.Load(cboRelativeType, RelaytionTypeDataWorker.RelaytionTypeADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboWorkPlace()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboWorkPlace, BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboEthnic()
        {
            try
            {
                var ethnic = BackendDataWorker.Get<SDA_ETHNIC>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ETHNIC_CODE", columnInfos, false, 150);
                ControlEditorLoader.Load(Res_ComboEthnic, ethnic, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToForm(HIS_TREATMENT currentTreatmentFinishSDO)
        {
            try
            {
                if (currentTreatmentFinishSDO != null)
                {
                    spinSickLeaveDay.EditValue = currentTreatmentFinishSDO.SICK_LEAVE_DAY;
                    txtRelativeName.Text = currentTreatmentFinishSDO.TDL_PATIENT_RELATIVE_NAME;
                    cboRelativeType.EditValue = currentTreatmentFinishSDO.TDL_PATIENT_RELATIVE_TYPE;
                    txtExtraEndCode.Text = currentTreatmentFinishSDO.EXTRA_END_CODE;

                    txtLoginName.Text = currentTreatmentFinishSDO.SICK_LOGINNAME;
                    cboUser.EditValue = currentTreatmentFinishSDO.SICK_LOGINNAME;
                    txtWorkPlace.Text = currentTreatmentFinishSDO.TDL_PATIENT_WORK_PLACE;
                    txtEndTypeExtNote.Text = currentTreatmentFinishSDO.END_TYPE_EXT_NOTE;
                    if (currentTreatmentFinishSDO.SICK_LEAVE_FROM.HasValue)
                    {
                        dtSickLeaveFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.SICK_LEAVE_FROM.Value) ?? new DateTime();
                    }
                    else
                    {
                        dtSickLeaveFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.IN_TIME) ?? new DateTime();
                    }

                    if (currentTreatmentFinishSDO.SICK_LEAVE_TO.HasValue)
                    {
                        dtSickLeaveToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.SICK_LEAVE_TO.Value) ?? new DateTime();
                    }
                    else
                    {
                        dtSickLeaveToTime.EditValue = null;
                    }

                    MOS.Filter.HisPatientFilter patientFilter = new MOS.Filter.HisPatientFilter();
                    patientFilter.ID = currentTreatmentFinishSDO.PATIENT_ID;
                    Patient = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null).FirstOrDefault();

                    if (Patient != null)
                    {
                        SocialNumber = Patient.SOCIAL_INSURANCE_NUMBER;
                    }

                    if (this.workPlaceId.HasValue && this.workPlaceId.Value > 0)
                    {
                        cboWorkPlace.EditValue = this.workPlaceId;
                    }
                    else
                    {
                        if (Patient != null)
                        {
                            cboWorkPlace.EditValue = Patient.WORK_PLACE_ID;
                        }
                    }

                    txtHeinCard.Text = currentTreatmentFinishSDO.TDL_HEIN_CARD_NUMBER;
                    if (currentTreatmentFinishSDO.TDL_PATIENT_TYPE_ID == Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        txtHeinCard.Enabled = false;
                    }
                    else
                    {
                        txtHeinCard.Enabled = true;
                    }
                    txtSocialInsuranceNumber.Text = currentTreatmentFinishSDO.TDL_SOCIAL_INSURANCE_NUMBER ?? (!string.IsNullOrEmpty(currentTreatmentFinishSDO.TDL_HEIN_CARD_NUMBER) && currentTreatmentFinishSDO.TDL_HEIN_CARD_NUMBER.Length >= 10 ? currentTreatmentFinishSDO.TDL_HEIN_CARD_NUMBER.Substring(currentTreatmentFinishSDO.TDL_HEIN_CARD_NUMBER.Length - 10, 10) : null);
                    chkIsPregnancyTermination.Checked = currentTreatmentFinishSDO.IS_PREGNANCY_TERMINATION == 1;
                    txtGestationAge.Text = currentTreatmentFinishSDO.GESTATIONAL_AGE != null ? currentTreatmentFinishSDO.GESTATIONAL_AGE.ToString() : "";
                    memPregnancyTerminationReason.Text = currentTreatmentFinishSDO.PREGNANCY_TERMINATION_REASON;
                    memTreatmentMethod.Text = currentTreatmentFinishSDO.TREATMENT_METHOD;
                    if (currentTreatmentFinishSDO.PREGNANCY_TERMINATION_TIME != null)
                        dtePregnancyTermination.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTreatmentFinishSDO.PREGNANCY_TERMINATION_TIME ?? 0) ?? DateTime.Now;
                    txtMother.Text = currentTreatmentFinishSDO.TDL_PATIENT_MOTHER_NAME;
                    txtFather.Text = currentTreatmentFinishSDO.TDL_PATIENT_FATHER_NAME;
                }
                else
                {
                    spinSickLeaveDay.EditValue = null;
                    dtSickLeaveFromTime.EditValue = null;
                    dtSickLeaveToTime.EditValue = null;
                    txtRelativeName.Text = "";
                    cboRelativeType.EditValue = null;
                    txtExtraEndCode.Text = "";
                    cboWorkPlace.EditValue = null;
                    txtWorkPlace.Text = "";
                    txtLoginName.Text = "";
                    cboUser.EditValue = null;
                    chkIsPregnancyTermination.Checked = false;
                    txtGestationAge.Text = "";
                    memPregnancyTerminationReason.Text = null;
                    memTreatmentMethod.Text = null;
                    dtePregnancyTermination.EditValue = null;
                    txtMother.Text = null;
                    txtFather.Text = null;
                }

                LoadBabyInfor(this.hisTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                spinSickLeaveDay.Focus();
                spinSickLeaveDay.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void Reload(MOS.EFMODEL.DataModels.HIS_TREATMENT input)
        {
            try
            {
                if (input != null)
                {
                    txtExtraEndCode.Text = input.EXTRA_END_CODE;
                }
                else
                {
                    txtExtraEndCode.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReadOnly(bool isReadOnly)
        {
            try
            {
                spinSickLeaveDay.ReadOnly = isReadOnly;
                dtSickLeaveFromTime.ReadOnly = isReadOnly;
                dtSickLeaveToTime.ReadOnly = isReadOnly;
                txtRelativeName.ReadOnly = isReadOnly;
                cboRelativeType.ReadOnly = isReadOnly;
                cboWorkPlace.ReadOnly = isReadOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetValue(object data)
        {
            try
            {
                if (data is ADO.SickInitADO)
                {

                }
                else
                {
                    FillDataToForm(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal object GetValue()
        {
            object result = null;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return null;

                MOS.SDO.HisTreatmentFinishSDO outPut = new MOS.SDO.HisTreatmentFinishSDO();

                outPut.TreatmentId = hisTreatment.ID;

                if (spinSickLeaveDay.EditValue != null)
                {
                    outPut.SickLeaveDay = Inventec.Common.TypeConvert.Parse.ToDecimal(spinSickLeaveDay.EditValue.ToString());
                }
                else
                    outPut.SickLeaveDay = null;

                if (dtSickLeaveFromTime.EditValue != null)
                {
                    outPut.SickLeaveFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSickLeaveFromTime.DateTime);
                }
                else
                {
                    outPut.SickLeaveFrom = null;
                }
                if (dtSickLeaveToTime.EditValue != null)
                {
                    outPut.SickLeaveTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSickLeaveToTime.DateTime);
                }
                else
                {
                    outPut.SickLeaveTo = null;
                }

                outPut.PatientRelativeName = txtRelativeName.Text;
                outPut.PatientRelativeType = cboRelativeType.Text;
                outPut.PatientWorkPlace = txtWorkPlace.Text;
                outPut.SickLoginname = txtLoginName.Text;
                outPut.SickUsername = cboUser.Text;

                if (cboWorkPlace.EditValue != null)
                {
                    outPut.WorkPlaceId = Convert.ToInt64(cboWorkPlace.EditValue);
                }
                else
                {
                    outPut.WorkPlaceId = null;
                }

                if (!this.IsDuongThai)
                {
                    if (cboDocumentBook.EditValue != null)
                    {
                        outPut.DocumentBookId = Convert.ToInt64(cboDocumentBook.EditValue);
                    }
                    else
                    {
                        outPut.DocumentBookId = null;
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtHeinCard.Text))
                {
                    string heinCardNumber = this.txtHeinCard.Text;
                    heinCardNumber = HeinCardHelper.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    if (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber))
                    {
                        outPut.SickHeinCardNumber = heinCardNumber;
                    }
                }
                outPut.SocialInsuranceNumber = txtSocialInsuranceNumber.Text.ToString().Trim();
                outPut.EndTypeExtNote = txtEndTypeExtNote.Text.Trim();
                outPut.TreatmentMethod = memTreatmentMethod.Text.Trim();
                outPut.IsPregnancyTermination = chkIsPregnancyTermination.Checked;
                outPut.PregnancyTerminationReason = memPregnancyTerminationReason.Text.Trim();
                if (!string.IsNullOrEmpty(txtGestationAge.Text.Trim()))
                    outPut.GestationalAge = Int64.Parse(txtGestationAge.Text.Trim());
                else
                    outPut.GestationalAge = null;
                if (dtePregnancyTermination.EditValue != null && dtePregnancyTermination.DateTime != DateTime.MinValue)
                    outPut.PregnancyTerminationTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtePregnancyTermination.DateTime);
                outPut.MotherName = txtMother.Text.Trim();
                outPut.FatherName = txtFather.Text.Trim();
                result = outPut;
                Inventec.Common.Logging.LogSystem.Debug("UCSick.GetValue____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => outPut), outPut));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

        private void gridViewBaby_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_BABY pData = (V_HIS_BABY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "BORN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.BORN_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Res_DeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //var row = (BabyADO)gridViewBaby.GetFocusedRow();
                //if (row != null)
                //{
                //    babyAdos = (List<BabyADO>)gridControlBaby.DataSource;
                //    babyAdos.Remove(row);
                //    gridControlBaby.BeginUpdate();
                //    gridControlBaby.DataSource = babyAdos;
                //    gridControlBaby.EndUpdate();
                //}
                //babyAdos.Remove
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDayNum()
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue != null && dtSickLeaveFromTime.DateTime != DateTime.MinValue
                    && dtSickLeaveToTime.EditValue != null && dtSickLeaveToTime.DateTime != DateTime.MinValue
                    && dtSickLeaveFromTime.DateTime.Date <= dtSickLeaveToTime.DateTime.Date)
                {
                    TimeSpan ts = (TimeSpan)(dtSickLeaveToTime.DateTime.Date - dtSickLeaveFromTime.DateTime.Date);
                    spinSickLeaveDay.Value = ts.Days + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateFrom()
        {
            try
            {
                if (dtSickLeaveToTime.EditValue != null && dtSickLeaveToTime.DateTime != DateTime.MinValue && spinSickLeaveDay.EditValue != null)
                {
                    dtSickLeaveFromTime.DateTime = dtSickLeaveToTime.DateTime.AddDays((double)(-spinSickLeaveDay.Value + 1));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateTo()
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue != null && dtSickLeaveFromTime.DateTime != DateTime.MinValue && spinSickLeaveDay.EditValue != null)
                {
                    dtSickLeaveToTime.DateTime = dtSickLeaveFromTime.DateTime.AddDays((double)(spinSickLeaveDay.Value - 1));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Res_Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //babyAdos.Add(new BabyADO());
                //gridControlBaby.BeginUpdate();
                //gridControlBaby.DataSource = babyAdos;
                //gridControlBaby.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCard_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string heinCardNumber = this.txtHeinCard.Text;
                    heinCardNumber = HeinCardHelper.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());

                    bool valid = true;
                    valid = valid && new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber);
                    if (valid && !String.IsNullOrEmpty(heinCardNumber))
                    {
                        this.dxErrorProvider1.ClearErrors();
                    }
                    else
                    {
                        this.dxErrorProvider1.SetError(this.txtHeinCard, "Người dùng nhập số thẻ BHYT không hợp lệ");
                    }
                    txtSocialInsuranceNumber.Focus();
                    txtSocialInsuranceNumber.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCard_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = "Người dùng nhập số thẻ BHYT không hợp lệ";
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinCard_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void CallModule(string moduleLink, List<object> data)
        {
            try
            {
                CallModule callModule = new CallModule(moduleLink, this.currentModule.RoomId, currentModule.RoomTypeId, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinSickLeaveDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue != null)
                {
                    this.CalculateDateTo();
                }
                else
                {
                    this.CalculateDateFrom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSickLeaveDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtSickLeaveFromTime.Focus();
                    dtSickLeaveFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSickLeaveFromTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtSickLeaveToTime.Focus();
                    dtSickLeaveToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSickLeaveFromTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSickLeaveToTime.EditValue == null || dtSickLeaveToTime.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateTo();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickLeaveFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtSickLeaveToTime.Focus();
                    dtSickLeaveToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSickLeaveToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtRelativeName.Focus();
                    txtRelativeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSickLeaveToTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtSickLeaveFromTime.EditValue == null || dtSickLeaveFromTime.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateFrom();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSickLeaveToTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRelativeName.Focus();
                    txtRelativeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRelativeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRelativeType.Focus();
                    cboRelativeType.SelectAll();
                    cboRelativeType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRelativeType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboWorkPlace.Focus();
                    cboWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRelativeType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboWorkPlace.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboWorkPlace.EditValue = null;
                    cboWorkPlace.Properties.Buttons[0].Visible = false;
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    List<object> sendObj = new List<object>() { this.currentModule };
                    CallModule("HIS.Desktop.Plugins.HisWorkPlace", sendObj);
                    InitComboWorkPlace();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboWorkPlace.EditValue != null)
                {
                    cboWorkPlace.Properties.Buttons[0].Visible = true;
                }
                else
                {
                    cboWorkPlace.Properties.Buttons[0].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtWorkPlace.Focus();
                    txtWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtWorkPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.NextFocus != null)
                    {
                        this.NextFocus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboUser.EditValue = null;
                        this.FocusShowpopup(cboUser, true);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboUser.EditValue = searchResult[0].LOGINNAME;
                            this.txtLoginName.Text = searchResult[0].LOGINNAME;
                            this.FocusWhileSelectedUser();
                        }
                        else
                        {
                            this.cboUser.EditValue = null;
                            this.FocusShowpopup(cboUser, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }

                    this.FocusWhileSelectedUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboUser.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.FocusWhileSelectedUser();
                            this.txtLoginName.Text = data.LOGINNAME;
                        }
                    }
                }
                else
                {
                    this.cboUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentBook_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDocumentBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnBaby_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfantInformation").FirstOrDefault();
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(hisTreatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule != null ? this.currentModule.RoomId : 0, this.currentModule != null ? this.currentModule.RoomTypeId : 0), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    LoadBabyInfor(this.hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBabyInfor(HIS_TREATMENT hisTreatment)
        {
            try
            {
                List<V_HIS_BABY> babes = new List<V_HIS_BABY>();
                if (hisTreatment != null)
                {
                    CommonParam param = new CommonParam();
                    HisBabyViewFilter filter = new HisBabyViewFilter();
                    filter.TREATMENT_ID = hisTreatment.ID;
                    babes = new BackendAdapter(param).Get<List<V_HIS_BABY>>("api/HisBaby/GetView", ApiConsumers.MosConsumer, filter, param);
                }

                gridControlBaby.DataSource = babes;
                gridControlBaby.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void txtHeinCard_Leave(object sender, EventArgs e)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(txtHeinCard.Text))
                {
                    string heinCardNumber = this.txtHeinCard.Text;
                    heinCardNumber = HeinCardHelper.TrimHeinCardNumber(heinCardNumber.Replace(" ", "").Replace("  ", "").ToUpper().Trim());
                    if (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().IsValidHeinCardNumber(heinCardNumber))
                    {
                        txtSocialInsuranceNumber.Text = heinCardNumber.Substring(heinCardNumber.Length - 10, 10);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGestationAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPregnancyTermination_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsPregnancyTermination.Checked)
                {
                    lcidtePregnancyTermination.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Maroon;
                    dtePregnancyTermination.ReadOnly = false;
                    memPregnancyTerminationReason.ReadOnly = false;
                    lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidMaxLengthRequired(true, memPregnancyTerminationReason, 1000, "Bắt buộc nhập thông tin lý do đình chỉ trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, txtGestationAge, 0, "Bắt buộc nhập thông tin tuổi thai trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, dtePregnancyTermination, 0, "Bắt buộc nhập thông tin thời gian đình chỉ trong trường hợp đình chỉ thai nghén");
                }
                else
                {
                    lcidtePregnancyTermination.AppearanceItemCaption.ForeColor = Color.Black;
                    lciGestationalAge.OptionsToolTip.ToolTip = null;
                    lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Black;
                    dtePregnancyTermination.EditValue = null;
                    dtePregnancyTermination.ReadOnly = true;
                    memPregnancyTerminationReason.Text = null;
                    memPregnancyTerminationReason.ReadOnly = true;
                    lciPregnancyTerminationReason.OptionsToolTip.ToolTip = null;
                    lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.SetValidationRule(memPregnancyTerminationReason, null);
                    dxValidationProvider1.SetValidationRule(txtGestationAge, null);
                    dxValidationProvider1.SetValidationRule(dtePregnancyTermination, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboDocumentBook.EditValue != null)
                {
                    txtLoginName.Focus();
                    txtLoginName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGestationAge_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!memPregnancyTerminationReason.ReadOnly)
                    {
                        memPregnancyTerminationReason.Focus();
                        memPregnancyTerminationReason.SelectAll();
                    }
                    else
                    {
                        memTreatmentMethod.Focus();
                        memTreatmentMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memTreatmentMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEndTypeExtNote.Focus();
                    txtEndTypeExtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void memPregnancyTerminationReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    memTreatmentMethod.Focus();
                    memTreatmentMethod.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCheckBhxhCode_Click(object sender, EventArgs e)
        {

            try
            {
                if (!dxValidationProvider1.Validate(txtSocialInsuranceNumber) || Patient == null)
                    return;
                CheckBhxh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<string> connectInfors = new List<string>();
        string api = "";
        string nameCb = "";
        string cccdCb = "";
        private HIS_EMPLOYEE GetEmployee(string username)
        {
            HIS_EMPLOYEE result = new HIS_EMPLOYEE();
            try
            {
                var rs = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(s => s.LOGINNAME == username).FirstOrDefault();
                if (rs != null)
                {
                    result = rs;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
        private async Task CheckBhxh()
        {
            ResultHistoryLDO rsData = null;
            try
            {
                BHXHLoginCFG.LoadConfig();
                CommonParam param = new CommonParam();
                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = txtSocialInsuranceNumber.Text.Trim();
                checkHistoryLDO.ngaySinh = Patient.IS_HAS_NOT_DAY_DOB == 1 ? Patient.DOB.ToString().Substring(0,4) :((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Patient.DOB) ?? DateTime.MinValue).ToString("dd/MM/yyyy"));
                checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(Patient.VIR_PATIENT_NAME.ToLower());
                checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? Patient.VIR_PATIENT_NAME.ToLower() : checkHistoryLDO.hoTen);
                string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = GetEmployee(username);
                try
                {
                    LogSystem.Debug("Get value config check cong BHXH: ");
                    string connect_infor = HisConfigs.Get<string>("HIS.CHECK_HEIN_CARD.BHXH__API");
                    if (!string.IsNullOrEmpty(connect_infor))
                    {
                        connectInfors = connect_infor.Split('|').ToList();
                        api = connectInfors.Count > 0 ? connectInfors[0] : string.Empty;

                        nameCb = connectInfors.Count > 1 && !string.IsNullOrEmpty(connectInfors[1]) ? connectInfors[1] : employee.TDL_USERNAME;
                        cccdCb = connectInfors.Count > 2 && !string.IsNullOrEmpty(connectInfors[2]) ? connectInfors[2] : employee.IDENTIFICATION_NUMBER;

                        LogSystem.Debug("BHXHLoginCFG.OFFICERNAME: " + connectInfors[1]);
                        LogSystem.Debug("BHXHLoginCFG.CCCDOFFICER: " + connectInfors[2]);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                apiInsuranceExpertise.ApiEgw = api;
                checkHistoryLDO.hoTenCb = nameCb;
                checkHistoryLDO.cccdCb = cccdCb;

                Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkHistoryLDO), checkHistoryLDO));
                    rsData = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));
                    if (rsData != null)
                    {
                        if (rsData.maKetQua == "000" || rsData.maKetQua == "001" || rsData.maKetQua == "002" || rsData.maKetQua == "004")
                        {
                            txtHeinCard.Text = rsData.maThe;
                            if (!string.IsNullOrEmpty(rsData.ngaySinh) && checkHistoryLDO.ngaySinh != rsData.ngaySinh && Patient != null && DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn cập nhật lại ngày sinh của bệnh nhân không?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                WaitingManager.Show();
                                var splDob = rsData.ngaySinh.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (splDob.Count > 2)
                                {
                                    Patient.DOB = Int64.Parse(splDob[2] + splDob[1] + splDob[0] + "000000");
                                    Patient.IS_HAS_NOT_DAY_DOB = null;
                                }
                                else
                                {
                                    Patient.DOB = Int64.Parse(splDob[0] + "0101000000");
                                    Patient.IS_HAS_NOT_DAY_DOB = 1;

                                }
                                HisPatientUpdateSDO sdo = new HisPatientUpdateSDO();
                                sdo.HisPatient = Patient;
                                sdo.TreatmentId = hisTreatment.ID;
                                var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, resultData != null);
                            }
                        }
                        else if (!string.IsNullOrEmpty(rsData.ghiChu))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(rsData.ghiChu, "Thông báo");
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
