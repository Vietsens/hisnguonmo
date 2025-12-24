using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.MchTreatmentExamService.Validate;
using System;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MainForm
{
    public partial class UCMchTreatmentExamService : HIS.Desktop.Utility.FormBase
    {
        private DXValidationProvider dxValidationProvider;
        private int positionHandleControl;

        #region Init Validation

        /// <summary>
        /// Khởi tạo validation cho tất cả các tab
        /// </summary>
        private void InitValidation()
        {
            try
            {
                dxValidationProvider = new DXValidationProvider();
                dxValidationProvider.ValidationMode = ValidationMode.Manual;
                dxValidationProvider.ValidationFailed += DxValidationProvider_ValidationFailed;
                // Tab 1: Sàng lọc
                InitValidationTab1();

                // Tab 2: Khám thai
                InitValidationTab2();

                // Tab 3: Sinh đẻ
                InitValidationTab3();

                // Tab 4: Tránh thai
                InitValidationTab4();

                // Tab 5: Phá thai
                InitValidationTab5();
                
                // Đăng ký sự kiện thay đổi dữ liệu để clear error
                RegisterClearErrorEvents();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        #endregion

        #region Tab 1 - Validation

        private void InitValidationTab1()
        {
            try
            {
                // Validation Ngày khám
                DateTimeValidationRule dteDateValidation1 = new DateTimeValidationRule();
                dteDateValidation1.dte = dteExam1;
                dteDateValidation1.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteExam1, dteDateValidation1);

                // Validation Người khám
                GridLookupValidationRule userValidation1 = new GridLookupValidationRule();
                userValidation1.gridLookUpEdit = cboUser1;
                userValidation1.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboUser1, userValidation1);

                // Validation Trình độ
                GridLookupValidationRule diplomaValidation1 = new GridLookupValidationRule();
                diplomaValidation1.gridLookUpEdit = cboDiploma1;
                diplomaValidation1.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboDiploma1, diplomaValidation1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab 2 - Validation

        private void InitValidationTab2()
        {
            try
            {
                // Validation Ngày khám
                DateTimeValidationRule dteDateValidation2 = new DateTimeValidationRule();
                dteDateValidation2.dte = dteExam2;
                dteDateValidation2.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteExam2, dteDateValidation2);

                // Validation Người khám
                GridLookupValidationRule userValidation2 = new GridLookupValidationRule();
                userValidation2.gridLookUpEdit = cboUser2;
                userValidation2.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboUser2, userValidation2);

                // Validation Trình độ
                GridLookupValidationRule diplomaValidation2 = new GridLookupValidationRule();
                diplomaValidation2.gridLookUpEdit = cboDiploma2;
                diplomaValidation2.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboDiploma2, diplomaValidation2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab 3 - Validation

        private void InitValidationTab3()
        {
            try
            {
                // Validation Ngày khám
                DateTimeValidationRule dteDateValidation3 = new DateTimeValidationRule();
                dteDateValidation3.dte = dteExam3;
                dteDateValidation3.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteExam3, dteDateValidation3);

                // Validation Người khám
                GridLookupValidationRule userValidation3 = new GridLookupValidationRule();
                userValidation3.gridLookUpEdit = cboUser3;
                userValidation3.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboUser3, userValidation3);

                // Validation Trình độ
                GridLookupValidationRule diplomaValidation3 = new GridLookupValidationRule();
                diplomaValidation3.gridLookUpEdit = cboDiploma3;
                diplomaValidation3.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboDiploma3, diplomaValidation3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab 4 - Validation

        private void InitValidationTab4()
        {
            try
            {
                // Validation Ngày khám
                DateTimeValidationRule dteDateValidation4 = new DateTimeValidationRule();
                dteDateValidation4.dte = dteExam4;
                dteDateValidation4.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteExam4, dteDateValidation4);

                // Validation Người khám
                GridLookupValidationRule userValidation4 = new GridLookupValidationRule();
                userValidation4.gridLookUpEdit = cboUser4;
                userValidation4.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboUser4, userValidation4);

                // Validation Trình độ
                GridLookupValidationRule diplomaValidation4 = new GridLookupValidationRule();
                diplomaValidation4.gridLookUpEdit = cboDiploma4;
                diplomaValidation4.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboDiploma4, diplomaValidation4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Tab 5 - Validation

        private void InitValidationTab5()
        {
            try
            {
                // Validation Ngày khám
                DateTimeValidationRule dteDateValidation5 = new DateTimeValidationRule();
                dteDateValidation5.dte = dteExam5;
                dteDateValidation5.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dteExam5, dteDateValidation5);

                // Validation Người khám
                GridLookupValidationRule userValidation5 = new GridLookupValidationRule();
                userValidation5.gridLookUpEdit = cboUser5;
                userValidation5.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboUser5, userValidation5);

                // Validation Trình độ
                GridLookupValidationRule diplomaValidation5 = new GridLookupValidationRule();
                diplomaValidation5.gridLookUpEdit = cboDiploma5;
                diplomaValidation5.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboDiploma5, diplomaValidation5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Register Clear Error Events

        private void RegisterClearErrorEvents()
        {
            try
            {
                // Tab 1
                if (dteExam1 != null) dteExam1.EditValueChanged += Control_EditValueChanged;
                if (cboUser1 != null) cboUser1.EditValueChanged += Control_EditValueChanged;
                if (cboDiploma1 != null) cboDiploma1.EditValueChanged += Control_EditValueChanged;

                // Tab 2
                if (dteExam2 != null) dteExam2.EditValueChanged += Control_EditValueChanged;
                if (cboUser2 != null) cboUser2.EditValueChanged += Control_EditValueChanged;
                if (cboDiploma2 != null) cboDiploma2.EditValueChanged += Control_EditValueChanged;

                // Tab 3
                if (dteExam3 != null) dteExam3.EditValueChanged += Control_EditValueChanged;
                if (cboUser3 != null) cboUser3.EditValueChanged += Control_EditValueChanged;
                if (cboDiploma3 != null) cboDiploma3.EditValueChanged += Control_EditValueChanged;

                // Tab 4
                if (dteExam4 != null) dteExam4.EditValueChanged += Control_EditValueChanged;
                if (cboUser4 != null) cboUser4.EditValueChanged += Control_EditValueChanged;
                if (cboDiploma4 != null) cboDiploma4.EditValueChanged += Control_EditValueChanged;

                // Tab 5
                if (dteExam5 != null) dteExam5.EditValueChanged += Control_EditValueChanged;
                if (cboUser5 != null) cboUser5.EditValueChanged += Control_EditValueChanged;
                if (cboDiploma5 != null) cboDiploma5.EditValueChanged += Control_EditValueChanged;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Control_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dxValidationProvider != null && sender is DevExpress.XtraEditors.BaseEdit)
                {
                    dxValidationProvider.RemoveControlError(sender as DevExpress.XtraEditors.BaseEdit);
                }
                var cbo = sender as DevExpress.XtraEditors.GridLookUpEdit;
                if(cbo != null && cbo.Name == cboDiploma3.Name)
                {
                    txtDeliveryAssistant3.Text = cboDiploma3.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Validate Methods

        /// <summary>
        /// Validate tab hiện tại dựa vào tab đang được chọn
        /// </summary>
        private bool ValidateCurrentTab()
        {
            bool valid = false;
            try
            {
                if (xtraTabControl1 != null && xtraTabControl1.SelectedTabPage != null)
                {
                    int selectedTabIndex = xtraTabControl1.TabPages.IndexOf(xtraTabControl1.SelectedTabPage);
                    valid = ValidateTab(selectedTabIndex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        /// <summary>
        /// Validate theo từng tab
        /// </summary>
        private bool ValidateTab(int tabIndex)
        {
            bool valid = false;
            try
            {
                switch (tabIndex)
                {
                    case 0: // Tab 1
                        valid = ValidateTab1();
                        break;
                    case 1: // Tab 2
                        valid = ValidateTab2();
                        break;
                    case 2: // Tab 3
                        valid = ValidateTab3();
                        break;
                    case 3: // Tab 4
                        valid = ValidateTab4();
                        break;
                    case 4: // Tab 5
                        valid = ValidateTab5();
                        break;
                    default:
                        valid = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidateTab1()
        {
            bool valid = true;
            try
            {
                if (dxValidationProvider == null) return true;

                valid = valid && dxValidationProvider.Validate(dteExam1);
                valid = valid && dxValidationProvider.Validate(cboUser1);
                valid = valid && dxValidationProvider.Validate(cboDiploma1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidateTab2()
        {
            bool valid = true;
            try
            {
                if (dxValidationProvider == null) return true;

                valid = valid && dxValidationProvider.Validate(dteExam2);
                valid = valid && dxValidationProvider.Validate(cboUser2);
                valid = valid && dxValidationProvider.Validate(cboDiploma2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidateTab3()
        {
            bool valid = true;
            try
            {
                if (dxValidationProvider == null) return true;

                valid = valid && dxValidationProvider.Validate(dteExam3);
                valid = valid && dxValidationProvider.Validate(cboUser3);
                valid = valid && dxValidationProvider.Validate(cboDiploma3);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidateTab4()
        {
            bool valid = true;
            try
            {
                if (dxValidationProvider == null) return true;

                valid = valid && dxValidationProvider.Validate(dteExam4);
                valid = valid && dxValidationProvider.Validate(cboUser4);
                valid = valid && dxValidationProvider.Validate(cboDiploma4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private bool ValidateTab5()
        {
            bool valid = true;
            try
            {
                if (dxValidationProvider == null) return true;

                valid = valid && dxValidationProvider.Validate(dteExam5);
                valid = valid && dxValidationProvider.Validate(cboUser5);
                valid = valid && dxValidationProvider.Validate(cboDiploma5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        #endregion
    }
}
