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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Plugins.DebateDiagnostic.ADO;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.DebateDiagnostic.UcDebateDetail
{
    public partial class UcOther : UserControl
    {
        private int positionHandleControl = -1;
        private bool IsOther;
        private long TreatmentId;
        private long RoomId;
        private long RoomTypeId;
        private HIS_SERVICE hisService;
        List<MedicineTypeADO> currentMedicineTypeAlls;
        List<ActiveIngredientADO> currentActiveIngredientAlls;
        //List<MedicineTypeADO> currentMedicineTypeSelecteds;
        //List<ActiveIngredientADO> currentActiveIngredientSelecteds;
        //qtcode
        internal Inventec.Desktop.Common.Modules.Module modules;
        //qtcode
        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;


        public UcOther(long treatmentId, long roomId, long roomTypeId, bool isOther, Inventec.Desktop.Common.Modules.Module modules)
        {
            InitializeComponent();
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
            this.IsOther = isOther;
            this.modules = modules;
        }
        public UcOther(long treatmentId, long roomId, long roomTypeId, bool isOther, HIS_SERVICE _hisService, Inventec.Desktop.Common.Modules.Module modules)
        {
            InitializeComponent();
            this.TreatmentId = treatmentId;
            this.RoomId = roomId;
            this.RoomTypeId = roomTypeId;
            this.IsOther = isOther;
            this.hisService = _hisService;

            this.modules = modules;
        }

        private void UcOther_Load(object sender, EventArgs e)
        {
            try
            {
                //InitGridCheckComboMedicineType();
                //LoadDataComboMedicineType();
                //InitGridCheckComboAcinIntegrate();
                //LoadDataComboAcinIntegrate();

                InitPopupMedicineType();
                InitPopupActiveIngredient();

                VisibilityControl();
                ValidationControl();
                UpdateCustomHeaderButtons(xtraTabControl1.SelectedTabPage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDebateDiagnostic(HIS_DEBATE hisDebate)
        {
            try
            {
                List<long> arrMetyIds = null;
                if (!String.IsNullOrEmpty(hisDebate.MEDICINE_TYPE_IDS))
                {
                    var arrMetys = hisDebate.MEDICINE_TYPE_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    arrMetyIds = (arrMetys != null && arrMetys.Count() > 0) ? arrMetys.Where(o => Inventec.Common.TypeConvert.Parse.ToInt64(o) > 0).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : null;
                    if (arrMetyIds != null && arrMetyIds.Count > 0)
                    {
                        if (currentMedicineTypeAlls == null || currentMedicineTypeAlls.Count == 0)
                        {
                            var _currentMedicineTypeAlls = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                            this.currentMedicineTypeAlls = _currentMedicineTypeAlls != null ? _currentMedicineTypeAlls.Where(o => o.IS_ACTIVE == 1).Select(o => new MedicineTypeADO(o)).ToList() : null;
                        }
                        this.isShowContainerMediMatyForChoose = true;

                        if (currentMedicineTypeAlls != null && currentMedicineTypeAlls.Count > 0)
                        {
                            currentMedicineTypeAlls.ForEach(o => o.IsChecked = arrMetyIds.Contains(o.ID));
                            ProcessDisplayMedicineTypeWithData();
                        }
                    }
                }
                if (!String.IsNullOrEmpty(hisDebate.ACTIVE_INGREDIENT_IDS))
                {
                    var arrAcIngrs = hisDebate.ACTIVE_INGREDIENT_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var arrAcIngrIds = (arrAcIngrs != null && arrAcIngrs.Count() > 0) ? arrAcIngrs.Where(o => Inventec.Common.TypeConvert.Parse.ToInt64(o) > 0).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : null;
                    if (arrAcIngrIds != null && arrAcIngrIds.Count > 0)
                    {
                        if (currentActiveIngredientAlls == null || currentActiveIngredientAlls.Count == 0)
                        {
                            var _currentActiveIngredientAlls = BackendDataWorker.Get<HIS_ACTIVE_INGREDIENT>();
                            this.currentActiveIngredientAlls = _currentActiveIngredientAlls != null ? _currentActiveIngredientAlls.Where(o => o.IS_ACTIVE == 1).Select(o => new ActiveIngredientADO(o)).ToList() : null;
                        }
                        if (currentActiveIngredientAlls != null && currentActiveIngredientAlls.Count > 0)
                        {
                            currentActiveIngredientAlls.ForEach(o => o.IsChecked = arrAcIngrIds.Contains(o.ID));
                            ProcessDisplayActiveIngredientWithData();
                        }
                    }
                }

                if ((arrMetyIds != null && arrMetyIds.Count == 1) || hisDebate.ID > 0)
                {
                    txtMedicineName.Text = hisDebate.MEDICINE_TYPE_NAME;
                    txtConcena.Text = hisDebate.MEDICINE_CONCENTRA;
                    txtUserManual.Text = hisDebate.MEDICINE_TUTORIAL;
                    txtUseForm.Text = hisDebate.MEDICINE_USE_FORM_NAME;
                    dtTimeUse.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisDebate.MEDICINE_USE_TIME ?? 0);
                }

                txtDiscussion.EditValue = hisDebate.DISCUSSION;
                txtRequestContent.Text = hisDebate.REQUEST_CONTENT;
                txtPathologicalHistory.Text = hisDebate.PATHOLOGICAL_HISTORY;
                txtHospitalizationState.Text = hisDebate.HOSPITALIZATION_STATE;
                txtBeforeDiagnostic.Text = hisDebate.BEFORE_DIAGNOSTIC;
                txtTreatmentTracking.Text = hisDebate.TREATMENT_TRACKING;
                txtDiagnostic.Text = hisDebate.DIAGNOSTIC;
                txtTreatmentMethod.Text = hisDebate.TREATMENT_METHOD;
                txtCareMethod.Text = hisDebate.CARE_METHOD;
                txtConclusion.Text = hisDebate.CONCLUSION;

                //qtcode
                if (IsOther)
                {
                    txtKetQuaCLS.Text = hisDebate.SUBCLINICAL_PROCESSES;
                }
                else
                {
                    txtKetQuaCLS2.Text = hisDebate.SUBCLINICAL_PROCESSES;
                }
                //qtcode
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisDebate), hisDebate));

                if (hisDebate.SERVICE_ID != null)
                {
                    HisServiceFilter filter = new HisServiceFilter();
                    filter.ID = hisDebate.SERVICE_ID;
                    var data = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter, null);
                    if (data != null && data.Count() > 0)
                    {
                        txtServiceCode.Text = data.FirstOrDefault().SERVICE_CODE;
                        txtServiceName.Text = data.FirstOrDefault().SERVICE_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDebateDiagnostic(HIS_SERVICE_REQ hisDebate)
        {
            try
            {
                txtPathologicalHistory.Text = hisDebate.PATHOLOGICAL_HISTORY;
                string hospitalizationState = hisDebate.FULL_EXAM + "\r\n" + hisDebate.PART_EXAM + "\r\n" + hisDebate.SUBCLINICAL;
                txtHospitalizationState.Text = hospitalizationState.Trim();
                txtTreatmentTracking.Text = hisDebate.PATHOLOGICAL_PROCESS;
                txtTreatmentMethod.Text = hisDebate.TREATMENT_INSTRUCTION;
                txtConclusion.Text = hisDebate.NEXT_TREATMENT_INSTRUCTION;

                if (!string.IsNullOrEmpty(hisDebate.ICD_CODE))
                {
                    var icd = Base.GlobalStore.HisIcds.Where(o => o.ICD_CODE == hisDebate.ICD_CODE).FirstOrDefault();
                    txtBeforeDiagnostic.Text = icd.ICD_NAME;
                    txtDiagnostic.Text = icd.ICD_NAME;
                }
                else
                {
                    txtBeforeDiagnostic.Text = "";
                    txtDiagnostic.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisibilityControl()
        {
            try
            {
                if (IsOther)
                {
                    lciForcboActiveIngredient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciForcboMedcineType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciMedicineName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciConcena.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciUseForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciTimeUse.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciUserManual.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciRequestContent.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciServiceCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciServiceName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    //qtcode
                    lciKetQuaCLS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciChonKQ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciKetQuaCLS2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciChonKQ2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciForcboActiveIngredient.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciForcboMedcineType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciMedicineName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciConcena.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciUseForm.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciTimeUse.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciUserManual.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    LciRequestContent.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciServiceCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciServiceName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    //qtcode
                    lciKetQuaCLS.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciChonKQ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    lciKetQuaCLS2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciChonKQ2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                if (hisService != null)
                {
                    txtServiceCode.Text = hisService.SERVICE_CODE;
                    txtServiceName.Text = hisService.SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ValidationControl()
        {
            try
            {
                ValidationControlMedicine(txtMedicineName, 500, !IsOther, IsValidControlMedicineNameOrActiveGredient);
                ValidationSingleControl(cboActiveIngredient, dxValidationProvider1, "", IsValidControlMedicineNameOrActiveGredient);
                //ValidationControlMaxLength(txtMedicineName, 500, false);
                ValidationControlMaxLength(txtUseForm, 100, false, dxValidationProvider1);
                ValidationControlMaxLength(txtConcena, 1000, false, dxValidationProvider1);
                ValidationControlMaxLength(txtUserManual, 2000, false, dxValidationProvider1);
                ValidationControlMaxLength(txtRequestContent, 1000, false, dxValidationProvider1);

                ValidationControlMaxLength(txtPathologicalHistory, 4000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtHospitalizationState, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtBeforeDiagnostic, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtTreatmentTracking, 4000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtDiscussion, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtDiagnostic, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtTreatmentMethod, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtCareMethod, 2000, false, dxValidationProviderControl);
                ValidationControlMaxLength(txtConclusion, 2000, false, dxValidationProviderControl);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void WarningValidMessage()
        {
            try
            {
                var invalidControls = dxValidationProviderControl.GetInvalidControls();
                List<string> listNameValid = new List<string>();
                if (invalidControls != null && invalidControls.Count > 0)
                {
                    List<MemoEdit> memoEdits = invalidControls.OfType<MemoEdit>().ToList();
                    if (memoEdits != null && memoEdits.Count > 0)
                    {
                        foreach (var item in memoEdits)
                        {
                            if (item.Name == txtPathologicalHistory.Name)
                                listNameValid.Add(xtraTabPage1.Text);
                            else if (item.Name == txtHospitalizationState.Name)
                                listNameValid.Add(xtraTabPage2.Text);
                            else if (item.Name == txtBeforeDiagnostic.Name)
                                listNameValid.Add(xtraTabPage3.Text);
                            else if (item.Name == txtTreatmentTracking.Name)
                                listNameValid.Add(xtraTabPage4.Text);
                            else if (item.Name == txtDiscussion.Name)
                                listNameValid.Add(xtraTabPage5.Text);
                            else if (item.Name == txtDiagnostic.Name)
                                listNameValid.Add(xtraTabPage6.Text);
                            else if (item.Name == txtTreatmentMethod.Name)
                                listNameValid.Add(xtraTabPage7.Text);
                            else if (item.Name == txtCareMethod.Name)
                                listNameValid.Add(xtraTabPage8.Text);
                            else if (item.Name == txtConclusion.Name)
                                listNameValid.Add(xtraTabPage9.Text);
                        }
                    }

                    string warning = String.Join(", ", listNameValid);
                    if (!String.IsNullOrEmpty(warning))
                    {
                        XtraMessageBox.Show(warning + " vượt quá ký tự cho phép.", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsValidControlMedicineNameOrActiveGredient()
        {
            bool valid = true;
            try
            {
                var currentActiveIngredientSelecteds = (currentActiveIngredientAlls != null && currentActiveIngredientAlls.Count > 0) ? currentActiveIngredientAlls.Where(o => o.IsChecked).ToList() : null;
                if ((String.IsNullOrEmpty(txtMedicineName.Text.Trim())) && (currentActiveIngredientSelecteds == null || currentActiveIngredientSelecteds.Count == 0) && !IsOther)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
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
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
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

        private void ValidationControlMedicine(BaseEdit control, int? maxLength, bool IsRequired, IsValidControl validEditorReference)
        {
            try
            {
                CustomControlValidationRule validate = new CustomControlValidationRule();
                validate.editor = control;
                validate.messageError = "Chọn loại là \"Hội chẩn thuốc\" bắt buộc 1 trong các trường sau phải có thông tin: Tên thuốc, Hoạt chất";
                validate.validEditorReference = validEditorReference;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMedicineName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConcena.SelectAll();
                    txtConcena.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtConcena_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUseForm.SelectAll();
                    txtUseForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUseForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtTimeUse.SelectAll();
                    dtTimeUse.ShowPopup();
                    dtTimeUse.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeUse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtUserManual.SelectAll();
                    txtUserManual.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUserManual_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory.Focus();
                    txtPathologicalHistory.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRequestContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPathologicalHistory.Focus();
                    txtPathologicalHistory.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region internal method
        internal bool ValidControl()
        {
            bool result = false;
            try
            {
                this.positionHandleControl = -1;
                result = !(dxValidationProvider1.Validate() && dxValidationProviderControl.Validate());
                WarningValidMessage();
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal void DisableControlItem()
        {
            try
            {
                txtPathologicalHistory.ReadOnly = true;
                txtBeforeDiagnostic.ReadOnly = true;
                txtHospitalizationState.ReadOnly = true;
                txtTreatmentTracking.ReadOnly = true;
                txtDiagnostic.ReadOnly = true;
                txtCareMethod.ReadOnly = true;
                txtTreatmentMethod.ReadOnly = true;
                txtConclusion.ReadOnly = true;
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void GetData(ref HIS_DEBATE saveData)
        {
            try
            {
                if (saveData == null) saveData = new HIS_DEBATE();

                var currentMedicineTypeSelecteds = (currentMedicineTypeAlls != null && currentMedicineTypeAlls.Count > 0) ? currentMedicineTypeAlls.Where(o => o.IsChecked).ToList() : null;
                if (currentMedicineTypeSelecteds != null && currentMedicineTypeSelecteds.Count > 0)
                    saveData.MEDICINE_TYPE_IDS = String.Join(",", currentMedicineTypeSelecteds.Select(o => o.ID));
                var currentActiveIngredientSelecteds = (currentActiveIngredientAlls != null && currentActiveIngredientAlls.Count > 0) ? currentActiveIngredientAlls.Where(o => o.IsChecked).ToList() : null;
                if (currentActiveIngredientSelecteds != null && currentActiveIngredientSelecteds.Count > 0)
                    saveData.ACTIVE_INGREDIENT_IDS = String.Join(",", currentActiveIngredientSelecteds.Select(o => o.ID));
                saveData.MEDICINE_TYPE_NAME = txtMedicineName.Text.Trim();
                saveData.MEDICINE_CONCENTRA = txtConcena.Text.Trim();
                saveData.MEDICINE_USE_FORM_NAME = txtUseForm.Text.Trim();
                saveData.MEDICINE_TUTORIAL = txtUserManual.Text.Trim();
                saveData.DISCUSSION = txtDiscussion.Text.Trim();
                saveData.BEFORE_DIAGNOSTIC = txtBeforeDiagnostic.Text.Trim();
                saveData.CARE_METHOD = txtCareMethod.Text.Trim();
                saveData.CONCLUSION = txtConclusion.Text.Trim();
                saveData.DIAGNOSTIC = txtDiagnostic.Text.Trim();
                saveData.HOSPITALIZATION_STATE = txtHospitalizationState.Text.Trim();
                saveData.PATHOLOGICAL_HISTORY = txtPathologicalHistory.Text.Trim();
                saveData.REQUEST_CONTENT = txtRequestContent.Text.Trim();
                saveData.TREATMENT_METHOD = txtTreatmentMethod.Text.Trim();
                saveData.TREATMENT_TRACKING = txtTreatmentTracking.Text.Trim();
                //qtcodeapi
                if (IsOther)
                {
                    saveData.SUBCLINICAL_PROCESSES = txtKetQuaCLS.Text;
                }
                else
                {
                    saveData.SUBCLINICAL_PROCESSES = txtKetQuaCLS2.Text;
                }

                //qtcode
                if (dtTimeUse.EditValue != null && dtTimeUse.DateTime != DateTime.MinValue)
                    saveData.MEDICINE_USE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtTimeUse.EditValue ?? "").ToString()).ToString("yyyyMMddHHmm") + "00");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetData(object data)
        {
            try
            {
                if (data != null)
                {
                    if (data.GetType() == typeof(HIS_DEBATE))
                    {
                        LoadDataDebateDiagnostic((HIS_DEBATE)data);
                    }
                    else if (data.GetType() == typeof(HIS_SERVICE_REQ))
                    {
                        LoadDataDebateDiagnostic((HIS_SERVICE_REQ)data);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void SetContent(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    txtDiscussion.Text = !string.IsNullOrEmpty(txtDiscussion.Text) ? txtDiscussion.Text + "\r\n" + data : data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        internal void SetDataMedicine(HIS_DEBATE data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.1");
                List<long> arrMetyIds = null;
                if (data.MEDICINE_TYPE_IDS != null)
                {
                    var arrMetys = data.MEDICINE_TYPE_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    arrMetyIds = (arrMetys != null && arrMetys.Count() > 0) ? arrMetys.Where(o => Inventec.Common.TypeConvert.Parse.ToInt64(o) > 0).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : null;
                    arrMetyIds = (arrMetyIds != null && arrMetyIds.Count > 0) ? arrMetyIds.Distinct().ToList() : null;
                    if (arrMetyIds != null && arrMetyIds.Count > 0)
                    {
                        this.isShowContainerMediMatyForChoose = true;
                        currentMedicineTypeAlls.ForEach(o => o.IsChecked = arrMetyIds.Contains(o.ID));
                        ProcessDisplayMedicineTypeWithData();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrMetyIds), arrMetyIds));
                }
                if (data.ACTIVE_INGREDIENT_IDS != null)
                {
                    var arrAcIngrs = data.ACTIVE_INGREDIENT_IDS.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var arrAcIngrIds = (arrAcIngrs != null && arrAcIngrs.Count() > 0) ? arrAcIngrs.Where(o => Inventec.Common.TypeConvert.Parse.ToInt64(o) > 0).Select(o => Inventec.Common.TypeConvert.Parse.ToInt64(o)).ToList() : null;
                    if (arrAcIngrIds != null && arrAcIngrIds.Count > 0 && currentActiveIngredientAlls != null && currentActiveIngredientAlls.Count > 0)
                    {
                        currentActiveIngredientAlls.ForEach(o => o.IsChecked = arrAcIngrIds.Contains(o.ID));
                        ProcessDisplayActiveIngredientWithData();
                    }
                }
                if ((arrMetyIds != null && arrMetyIds.Count == 1) || data.ID > 0)
                {
                    txtMedicineName.EditValue = data.MEDICINE_TYPE_NAME;
                    txtConcena.EditValue = data.MEDICINE_CONCENTRA;
                    txtUserManual.EditValue = data.MEDICINE_TUTORIAL;
                    txtUseForm.EditValue = data.MEDICINE_USE_FORM_NAME;
                    dtTimeUse.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.MEDICINE_USE_TIME ?? 0);
                }
                Inventec.Common.Logging.LogSystem.Debug("SetDataMedicine.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MedcineTypeEditValueChanged()
        {
            try
            {
                var currentMedicineTypeSelecteds = currentMedicineTypeAlls.Where(o => o.IsChecked).ToList();
                if (currentMedicineTypeSelecteds != null && currentMedicineTypeSelecteds.Count > 0 && currentActiveIngredientAlls != null && currentActiveIngredientAlls.Count > 0)
                {
                    txtMedicineName.Text = String.Join(",", currentMedicineTypeSelecteds.Select(o => o.MEDICINE_TYPE_NAME));
                    txtConcena.EditValue = "";//TODO
                    txtUserManual.EditValue = "";//TODO
                    txtUseForm.EditValue = "";//TODO
                    dtTimeUse.EditValue = null;//TODO

                    var metyAcins = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>();

                    List<ActiveIngredientADO> currentActiveIngredientSelecteds = null;
                    var metyAcinss = metyAcins != null ? metyAcins.Where(o => currentMedicineTypeSelecteds.Exists(t => t.ID == o.MEDICINE_TYPE_ID)).OrderByDescending(o => o.ACTIVE_INGREDIENT_ID).ToList() : null;
                    if (metyAcinss != null && metyAcinss.Count > 0)
                    {
                        currentActiveIngredientSelecteds = currentActiveIngredientAlls != null ? currentActiveIngredientAlls.Where(o => metyAcinss.Exists(t => t.ACTIVE_INGREDIENT_ID == o.ID)).ToList() : null;
                    }
                    currentActiveIngredientAlls.ForEach(o => o.IsChecked = (metyAcinss != null && metyAcinss.Count > 0 && metyAcinss.Exists(t => t.ACTIVE_INGREDIENT_ID == o.ID)));
                    cboActiveIngredient.Properties.Buttons[1].Visible = (currentActiveIngredientSelecteds != null && currentActiveIngredientSelecteds.Count > 0);
                    ProcessDisplayActiveIngredientWithData();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metyAcinss), metyAcinss));
                }
                else
                {
                    txtMedicineName.EditValue = "";
                    txtConcena.EditValue = "";
                    txtUserManual.EditValue = "";
                    txtUseForm.EditValue = "";
                    dtTimeUse.EditValue = null;//TODO

                    cboActiveIngredient.Text = "";
                    currentActiveIngredientAlls.ForEach(o => o.IsChecked = false);
                    cboActiveIngredient.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedcineType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
                {
                    Inventec.Common.Logging.LogSystem.Debug("cboMedcineType_ButtonClick.1");
                    ProcessShowpopupControlContainerMedcineType();
                    Inventec.Common.Logging.LogSystem.Debug("cboMedcineType_ButtonClick.2");
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboMedcineType.Text = null;
                    currentMedicineTypeAlls.ForEach(o => o.IsChecked = false);
                    //DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerMedicineType.Columns["IsChecked"];
                    //if (gridColumnCheck != null)
                    //{
                    //    gridColumnCheck.ImageAlignment = StringAlignment.Center;
                    //    gridColumnCheck.Image = this.imageCollection1.Images[0];
                    //}
                    cboMedcineType.Properties.Buttons[1].Visible = false;
                    txtMedicineName.Text = txtConcena.Text = txtUseForm.Text = txtUserManual.Text = "";
                    dtTimeUse.EditValue = null;

                    cboActiveIngredient.Text = null;
                    currentActiveIngredientAlls.ForEach(o => o.IsChecked = false);
                    cboActiveIngredient.Properties.Buttons[1].Visible = false;
                }
                //else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                //{
                //    this.txtRoomCode.Text = "";
                //    this.beditRoom.Text = "";
                //    this.roomExts.ForEach(o => o.IsChecked = false);
                //    this.beditRoom.Properties.Buttons[1].Visible = false;
                //    DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerRoom.Columns["IsChecked"];
                //    if (gridColumnCheck != null)
                //    {
                //        gridColumnCheck.ImageAlignment = StringAlignment.Center;
                //        gridColumnCheck.Image = this.imageCollection1.Images[0];
                //    }
                //    this.cboExamService.Properties.Buttons[1].Visible = false;
                //    this.cboExamService.EditValue = null;
                //    this.txtExamServiceCode.Text = "";
                //    FocusTotxtExamServiceCode();
                //    Inventec.Common.Logging.LogSystem.Debug("beditRoom_Properties_ButtonClick.3");
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboActiveIngredient_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
                {
                    Inventec.Common.Logging.LogSystem.Debug("cboActiveIngredient_ButtonClick.1");
                    ProcessShowpopupControlContainerActiveIngredient();
                    Inventec.Common.Logging.LogSystem.Debug("cboActiveIngredient_ButtonClick.2");
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboActiveIngredient.Text = null;
                    currentActiveIngredientAlls.ForEach(o => o.IsChecked = false);
                    cboActiveIngredient.Properties.Buttons[1].Visible = false;
                    //DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerActiveIngredient.Columns["IsChecked"];
                    //if (gridColumnCheck != null)
                    //{
                    //    gridColumnCheck.ImageAlignment = StringAlignment.Center;
                    //    gridColumnCheck.Image = this.imageCollection1.Images[0];
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedcineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboActiveIngredient.Focus();
                    cboActiveIngredient.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboActiveIngredient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMedicineName.Focus();
                    txtMedicineName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedcineType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    Inventec.Common.Logging.LogSystem.Debug("cboMedcineType_KeyDown.1");
                    ProcessShowpopupControlContainerMedcineType();
                    Inventec.Common.Logging.LogSystem.Debug("cboMedcineType_KeyDown.2");
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    cboMedcineType.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedcineType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(cboMedcineType.Text))
                {
                    cboMedcineType.Refresh();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("cboMedcineType.Text", cboMedcineType.Text) + Inventec.Common.Logging.LogUtil.TraceData("isShowContainerMediMatyForChoose", isShowContainerMediMatyForChoose) + Inventec.Common.Logging.LogUtil.TraceData("isShowContainerMediMaty", isShowContainerMediMaty) + Inventec.Common.Logging.LogUtil.TraceData("isShow", isShow));
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewContainerMedicineType.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewContainerMedicineType.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%'  OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%'", cboMedcineType.Text);
                        gridViewContainerMedicineType.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewContainerMedicineType.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewContainerMedicineType.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewContainerMedicineType.FocusedRowHandle = 0;
                        gridViewContainerMedicineType.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewContainerMedicineType.OptionsFind.HighlightFindResults = true;

                        if (isShow)
                        {
                            ProcessShowpopupControlContainerMedcineType();
                            isShow = false;
                        }

                        cboMedcineType.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewContainerMedicineType.ActiveFilter.Clear();
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMedicineType.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboActiveIngredient_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    Inventec.Common.Logging.LogSystem.Debug("cboActiveIngredient_KeyDown.1");
                    ProcessShowpopupControlContainerActiveIngredient();
                    Inventec.Common.Logging.LogSystem.Debug("cboActiveIngredient_KeyDown.2");
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    cboActiveIngredient.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboActiveIngredient_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(cboActiveIngredient.Text))
                {
                    cboActiveIngredient.Refresh();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("cboActiveIngredient.Text", cboActiveIngredient.Text) + Inventec.Common.Logging.LogUtil.TraceData("isShowContainerMediMatyForChoose", isShowContainerMediMatyForChoose) + Inventec.Common.Logging.LogUtil.TraceData("isShowContainerMediMaty", isShowContainerMediMaty) + Inventec.Common.Logging.LogUtil.TraceData("isShow", isShow));
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewContainerActiveIngredient.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewContainerActiveIngredient.ActiveFilterString = String.Format("[ACTIVE_INGREDIENT_NAME] Like '%{0}%' OR [ACTIVE_INGREDIENT_CODE] Like '%{0}%' OR [ACTIVE_INGREDIENT_NAME__UNSIGN] Like '%{0}%'", cboActiveIngredient.Text);
                        gridViewContainerActiveIngredient.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewContainerActiveIngredient.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewContainerActiveIngredient.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewContainerActiveIngredient.FocusedRowHandle = 0;
                        gridViewContainerActiveIngredient.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewContainerActiveIngredient.OptionsFind.HighlightFindResults = true;

                        if (isShow)
                        {
                            ProcessShowpopupControlContainerActiveIngredient();
                            isShow = false;
                        }

                        cboActiveIngredient.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewContainerActiveIngredient.ActiveFilter.Clear();
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerActiveIngredient.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessShowpopupControlContainerActiveIngredient()
        {
            int heightPlus = 0;
            Rectangle bounds = GetClientRectangle(this, ref heightPlus);
            Rectangle bounds1 = GetAllClientRectangle(this, ref heightPlus);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds1), bounds1) + "|bounds1.Height=" + bounds1.Height + "|popupHeight=" + popupHeight);
            if (bounds == null)
            {
                bounds = cboActiveIngredient.Bounds;
            }

            //xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
            if (bounds1.Height <= 768)
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
                }
                else
                    heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
            }
            else
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
                }
                else
                    heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
            }

            Rectangle buttonBounds = new Rectangle(cboActiveIngredient.Bounds.X + 10, bounds.Y + heightPlus, bounds.Width, bounds.Height);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("buttonBounds", buttonBounds)
                + Inventec.Common.Logging.LogUtil.TraceData("heightPlus", heightPlus));

            this.currentActiveIngredientAlls = (this.currentActiveIngredientAlls != null && this.currentActiveIngredientAlls.Count > 0) ? this.currentActiveIngredientAlls.OrderByDescending(o => o.IsChecked).ThenBy(o => o.ACTIVE_INGREDIENT_NAME).ToList() : null;

            gridViewContainerActiveIngredient.BeginUpdate();
            gridViewContainerActiveIngredient.GridControl.DataSource = this.currentActiveIngredientAlls;
            gridViewContainerActiveIngredient.EndUpdate();
            popupControlContainerActiveIngredient.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
            gridViewContainerActiveIngredient.Focus();
            gridViewContainerActiveIngredient.FocusedRowHandle = 0;
        }

        void ProcessShowpopupControlContainerMedcineType()
        {
            int heightPlus = 0;
            Rectangle bounds = GetClientRectangle(this, ref heightPlus);
            Rectangle bounds1 = GetAllClientRectangle(this, ref heightPlus);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds1), bounds1) + "|bounds1.Height=" + bounds1.Height + "|popupHeight=" + popupHeight);
            if (bounds == null)
            {
                bounds = cboMedcineType.Bounds;
            }

            //xử lý tính toán lại vị trí hiển thị popup tương đối phụ thuộc theo chiều cao của popup, kích thước màn hình, đối tượng bệnh nhân(bhyt/...)
            if (bounds1.Height <= 768)
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -125 : (bounds.Y > 410 ? (-262) : (bounds.Y < 230 ? (-bounds.Y - 227) : -276));
                }
                else
                    heightPlus = bounds.Y >= 650 ? -60 : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 27 : -78)));
            }
            else
            {
                if (popupHeight == 400)
                {
                    heightPlus = bounds.Y >= 650 ? -327 : (bounds.Y > 410 ? -260 : (bounds.Y < 230 ? (-bounds.Y - 225) : -608));
                }
                else
                    heightPlus = bounds.Y >= 650 ? (-122) : (bounds.Y > 410 ? -60 : ((bounds.Y < 230 ? -bounds.Y - 25 : -180)));
            }

            Rectangle buttonBounds = new Rectangle(cboMedcineType.Bounds.X + 10, bounds.Y + heightPlus, bounds.Width, bounds.Height);
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("buttonBounds", buttonBounds)
                + Inventec.Common.Logging.LogUtil.TraceData("heightPlus", heightPlus));

            this.currentMedicineTypeAlls = this.currentMedicineTypeAlls != null ? this.currentMedicineTypeAlls.OrderByDescending(o => o.IsChecked).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList() : null;
            gridViewContainerMedicineType.BeginUpdate();
            gridViewContainerMedicineType.GridControl.DataSource = this.currentMedicineTypeAlls;
            gridViewContainerMedicineType.EndUpdate();
            popupControlContainerMedicineType.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom));
            gridViewContainerMedicineType.Focus();
            gridViewContainerMedicineType.FocusedRowHandle = 0;
        }

        private Rectangle GetClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetClientRectangle:" + Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + "|control.name=" + control.Name + "|bounds.Y=" + bounds.Y);
                if (control.Parent != null && !(control is UserControl))
                {
                    heightPlus += bounds.Y;
                    return GetClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }

        private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetAllClientRectangle:" + Inventec.Common.Logging.LogUtil.GetMemberName(() => bounds), bounds) + "|control.name=" + control.Name + "|bounds.Y=" + bounds.Y);
                if (control.Parent != null)
                {
                    heightPlus += bounds.Y;
                    return GetAllClientRectangle(control.Parent, ref heightPlus);
                }
            }
            return bounds;
        }

        private void popupControlContainerMedicineType_CloseUp(object sender, EventArgs e)
        {
            try
            {
                bool isExistsCheckeds = this.currentMedicineTypeAlls.Any(o => o.IsChecked);
                this.cboMedcineType.Properties.Buttons[1].Visible = isExistsCheckeds;
                isShow = true;
                if (isExistsCheckeds)
                {
                    cboMedcineType.Text = String.Join(", ", currentMedicineTypeAlls.Where(o => o.IsChecked).Select(o => o.MEDICINE_TYPE_NAME));
                    //this.MedcineTypeEditValueChanged();
                    cboActiveIngredient.Focus();
                    cboActiveIngredient.SelectAll();
                }
                else
                {
                    try
                    {
                        cboActiveIngredient.Focus();
                        cboActiveIngredient.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerActiveIngredient_CloseUp(object sender, EventArgs e)
        {
            try
            {
                bool isExistsCheckeds = this.currentActiveIngredientAlls.Any(o => o.IsChecked);
                this.cboActiveIngredient.Properties.Buttons[1].Visible = isExistsCheckeds;
                isShow = true;
                if (isExistsCheckeds)
                {
                    cboActiveIngredient.Text = String.Join(", ", currentActiveIngredientAlls.Where(o => o.IsChecked).Select(o => o.ACTIVE_INGREDIENT_NAME));
                    txtMedicineName.Focus();
                    txtMedicineName.SelectAll();
                }
                else
                {
                    try
                    {
                        txtMedicineName.Focus();
                        txtMedicineName.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessDisplayMedicineTypeWithData()
        {
            StringBuilder sbText = new StringBuilder();
            StringBuilder sbCode = new StringBuilder();
            var currentMetys = this.currentMedicineTypeAlls.Where(o => o.IsChecked).ToList();
            if (currentMetys != null && currentMetys.Count > 0)
            {
                foreach (MedicineTypeADO rv in currentMetys)
                {
                    if (rv != null)
                    {
                        if (sbText.ToString().Length > 0) { sbText.Append(", "); }
                        sbText.Append(rv.MEDICINE_TYPE_NAME);
                    }
                }
            }
            this.cboMedcineType.Properties.Buttons[1].Visible = (currentMetys != null && currentMetys.Count > 0);
            isShowContainerMediMatyForChoose = true;
            this.cboMedcineType.Text = sbText.ToString();

            this.MedcineTypeEditValueChanged();
        }

        void ProcessDisplayActiveIngredientWithData()
        {
            StringBuilder sbText = new StringBuilder();
            StringBuilder sbCode = new StringBuilder();
            var currentIngrs = this.currentActiveIngredientAlls.Where(o => o.IsChecked).ToList();
            if (currentIngrs != null && currentIngrs.Count > 0)
            {
                foreach (ActiveIngredientADO rv in currentIngrs)
                {
                    if (rv != null)
                    {
                        if (sbText.ToString().Length > 0) { sbText.Append(", "); }
                        sbText.Append(rv.ACTIVE_INGREDIENT_NAME);
                    }
                }
            }
            this.cboActiveIngredient.Properties.Buttons[1].Visible = (currentIngrs != null && currentIngrs.Count > 0);
            isShowContainerMediMatyForChoose = true;
            this.cboActiveIngredient.Text = sbText.ToString();
        }

        private void gridControlContainerMedicineType_Click(object sender, EventArgs e)
        {
            try
            {
                var rawRoom = (MedicineTypeADO)this.gridViewContainerMedicineType.GetFocusedRow();
                if (rawRoom != null)
                {
                    rawRoom.IsChecked = !rawRoom.IsChecked;
                    gridControlContainerMedicineType.RefreshDataSource();
                    isShowContainerMediMaty = true;
                    ProcessDisplayMedicineTypeWithData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlContainerActiveIngredient_Click(object sender, EventArgs e)
        {
            try
            {
                var rawAcgr = (ActiveIngredientADO)this.gridViewContainerActiveIngredient.GetFocusedRow();
                if (rawAcgr != null)
                {
                    rawAcgr.IsChecked = !rawAcgr.IsChecked;
                    gridControlContainerActiveIngredient.RefreshDataSource();
                    isShowContainerMediMaty = true;
                    ProcessDisplayActiveIngredientWithData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewContainerMedicineType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.1");
                ColumnView View = (ColumnView)gridControlContainerMedicineType.FocusedView;
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewContainerMedicineType.IsEditing)
                        this.gridViewContainerMedicineType.CloseEditor();

                    if (this.gridViewContainerMedicineType.FocusedRowModified)
                        this.gridViewContainerMedicineType.UpdateCurrentRow();

                    var rawMety = (MedicineTypeADO)this.gridViewContainerMedicineType.GetFocusedRow();

                    Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown. gridViewContainerMedicineType.FocusedRowHandle=" + View.FocusedRowHandle + "|" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rawMety), rawMety));
                    if (rawMety != null && rawMety.IsChecked)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.1.1");
                        rawMety.IsChecked = false;
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.1.2");
                    }
                    else if (rawMety != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.1.3");
                        rawMety.IsChecked = true;
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.1.4");
                    }
                    gridControlContainerMedicineType.RefreshDataSource();
                    ProcessDisplayMedicineTypeWithData();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var rawMety = (MedicineTypeADO)this.gridViewContainerMedicineType.GetFocusedRow();
                    if ((currentMedicineTypeAlls != null && !currentMedicineTypeAlls.Any(o => o.IsChecked)) && rawMety != null)
                    {
                        rawMety.IsChecked = true;
                        gridControlContainerMedicineType.RefreshDataSource();
                        ProcessDisplayMedicineTypeWithData();
                    }
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    popupControlContainerMedicineType.HidePopup();
                }
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerMedicineType_KeyDown.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerMedicineType_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.Column.FieldName == "IsChecked" && hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("hi.InRowCell");
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            gridControlContainerMedicineType.RefreshDataSource();
                            ProcessDisplayMedicineTypeWithData();

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    //else if (hi.Column.FieldName == "IsChecked" && hi.InColumnPanel)
                    //{
                    //    Inventec.Common.Logging.LogSystem.Debug("hi.InColumnPanel");

                    //    statecheckColumn = !statecheckColumn;
                    //    this.SetCheckAllColumn(statecheckColumn);
                    //    var rawMety = (MedicineTypeADO)this.gridViewContainerMedicineType.GetFocusedRow();
                    //    long roomIdFocus = rawMety != null ? rawMety.ID : 0;
                    //    this.currentMedicineTypeAlls.ForEach(o => o.IsChecked = statecheckColumn);
                    //    var roomFocus = this.currentMedicineTypeAlls.FirstOrDefault(o => o.ID == roomIdFocus);
                    //    if (roomFocus != null)
                    //    {
                    //        roomFocus.IsChecked = !roomFocus.IsChecked;
                    //    }
                    //    gridControlContainerMedicineType.RefreshDataSource();
                    //    ProcessDisplayMedicineTypeWithData();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewContainerActiveIngredient_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.1");
                ColumnView View = (ColumnView)gridControlContainerActiveIngredient.FocusedView;
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewContainerActiveIngredient.IsEditing)
                        this.gridViewContainerActiveIngredient.CloseEditor();

                    if (this.gridViewContainerActiveIngredient.FocusedRowModified)
                        this.gridViewContainerActiveIngredient.UpdateCurrentRow();

                    var rawAcgr = (ActiveIngredientADO)this.gridViewContainerActiveIngredient.GetFocusedRow();

                    Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown. gridViewContainerActiveIngredient.FocusedRowHandle=" + View.FocusedRowHandle + "|" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rawAcgr), rawAcgr));
                    if (rawAcgr != null && rawAcgr.IsChecked)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.1.1");
                        rawAcgr.IsChecked = false;
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.1.2");
                    }
                    else if (rawAcgr != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.1.3");
                        rawAcgr.IsChecked = true;
                        Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.1.4");
                    }
                    gridControlContainerActiveIngredient.RefreshDataSource();
                    ProcessDisplayActiveIngredientWithData();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var rawAcgr = (ActiveIngredientADO)this.gridViewContainerActiveIngredient.GetFocusedRow();
                    if ((currentActiveIngredientAlls != null && !currentActiveIngredientAlls.Any(o => o.IsChecked)) && rawAcgr != null)
                    {
                        rawAcgr.IsChecked = true;
                        gridControlContainerActiveIngredient.RefreshDataSource();
                        ProcessDisplayActiveIngredientWithData();
                    }
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    popupControlContainerActiveIngredient.HidePopup();
                }
                Inventec.Common.Logging.LogSystem.Debug("gridViewContainerActiveIngredient_KeyDown.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerActiveIngredient_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.Column.FieldName == "IsChecked" && hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("hi.InRowCell");
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            gridControlContainerActiveIngredient.RefreshDataSource();
                            ProcessDisplayActiveIngredientWithData();

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    //else if (hi.Column.FieldName == "IsChecked" && hi.InColumnPanel)
                    //{
                    //    Inventec.Common.Logging.LogSystem.Debug("hi.InColumnPanel");

                    //    statecheckColumn = !statecheckColumn;
                    //    this.SetCheckAllColumn(statecheckColumn);
                    //    var rawMety = (MedicineTypeADO)this.gridViewContainerMedicineType.GetFocusedRow();
                    //    long roomIdFocus = rawMety != null ? rawMety.ID : 0;
                    //    this.currentMedicineTypeAlls.ForEach(o => o.IsChecked = statecheckColumn);
                    //    var roomFocus = this.currentMedicineTypeAlls.FirstOrDefault(o => o.ID == roomIdFocus);
                    //    if (roomFocus != null)
                    //    {
                    //        roomFocus.IsChecked = !roomFocus.IsChecked;
                    //    }
                    //    gridControlContainerMedicineType.RefreshDataSource();
                    //    ProcessDisplayMedicineTypeWithData();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChonKQ_Click(object sender, EventArgs e)
        {
            try
            {

                List<object> listArgs = new List<object>();
                listArgs.Add(this.TreatmentId);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)SelectDataResult);
                listArgs.Add(true);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ContentSubclinical", RoomId, RoomTypeId, listArgs);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        //Được gọi bởi module ContentSubclinical khi người dùng hoàn tất việc chọn kết quả.
        //Nhận dữ liệu và cập nhật vào txtKetQuaCLS.
        private void SelectDataResult(object data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("SelectDataResult: ", data));
                txtKetQuaCLS.Text = "";
                txtKetQuaCLS2.Text = "";
                if (data != null && data is List<HIS.Desktop.ADO.ContentSubclinicalADO>)
                {
                    List<HIS.Desktop.ADO.ContentSubclinicalADO> dienBien = data as List<HIS.Desktop.ADO.ContentSubclinicalADO>;
                    List<string> lstData = new List<string>();


                    var dbXetNghiem = dienBien.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();
                    var dbother = dienBien.Where(o => o.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList();

                    if (dbXetNghiem != null && dbXetNghiem.Count > 0)
                    {
                        var groupxn = dbXetNghiem.OrderBy(p => p.NUM_ORDER).GroupBy(s => s.SERVICE_ID).ToList();
                        foreach (var item in groupxn)
                        {
                            List<string> lstData1 = new List<string>();
                            List<HIS.Desktop.ADO.ContentSubclinicalADO> xn = item.ToList();
                            for (int i = 0; i < xn.Count(); i++)
                            {
                                if (i == 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(xn[i].TDL_SERVICE_NAME.ToLower()) && !string.IsNullOrWhiteSpace(xn[i].TEST_INDEX_NAME) && xn[i].TDL_SERVICE_NAME.ToLower() == xn[i].TEST_INDEX_NAME.ToLower())
                                    {
                                        lstData1.Add(string.Format("{0}: {1} {2}", xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                    }
                                    else
                                    {
                                        lstData1.Add(string.Format("{0}: {1} {2} {3} ; ", xn[i].TDL_SERVICE_NAME, xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                    }
                                }
                                else
                                {
                                    lstData1.Add(string.Format("{0} {1} {2}", xn[i].TEST_INDEX_NAME, xn[i].VALUE, xn[i].SERVICE_UNIT_NAME));
                                }
                            }
                            lstData.Add(string.Join("; ", lstData1));
                        }
                    }

                    if (dbother != null && dbother.Count > 0)
                    {
                        foreach (var item in dbother)
                        {
                            lstData.Add(string.Format("{0}:{1}", item.TDL_SERVICE_NAME, item.VALUE));
                        }
                    }

                    txtKetQuaCLS.Text = string.Join("\r\n", lstData);
                    txtKetQuaCLS2.Text = string.Join("\r\n", lstData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                //if (e.Page == xtraTabPage1 || e.Page == xtraTabPage2 || e.Page == xtraTabPage3)
                //{
                //    xtraTabControl1.CustomHeaderButtons[0].Enabled = false;
                //}
                //else if (e.Page == xtraTabPage4)
                //{
                //    xtraTabControl1.CustomHeaderButtons[0].Enabled = true;
                //}
                UpdateCustomHeaderButtons(e.Page);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void UpdateCustomHeaderButtons(DevExpress.XtraTab.XtraTabPage page)
        {
            if (page == xtraTabPage1 || page == xtraTabPage2 || page == xtraTabPage3)
            {
                xtraTabControl1.CustomHeaderButtons[0].Enabled = false;
            }
            else if (page == xtraTabPage4)
            {
                xtraTabControl1.CustomHeaderButtons[0].Enabled = true;
            }
        }

        private void xtraTabControl1_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            try
            {
                var clickedButton = e.Button;

                if (clickedButton.Tag != null)
                {
                    string tag = clickedButton.Tag.ToString();

                    if (tag == "Chondienbien")
                    {
                        frmDevelopmentCLS frm = new frmDevelopmentCLS(TreatmentId);
                        frm.ParentUcOther = this;
                        frm.ShowDialog(); 
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        public void SetTreatmentTrackingText(string text)
        {
            txtTreatmentTracking.Text = text;
        }
    }
}
