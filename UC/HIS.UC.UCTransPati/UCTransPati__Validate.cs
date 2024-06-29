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
using HIS.UC.UCTransPati.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.UC.Icd;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;

namespace HIS.UC.UCTransPati
{
    public partial class UCTransPati : UserControl
    {
        #region Outside Validate

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {

                this.positionHandleControl = -1;
                if (!this.dxValidationProviderControl.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }
                valid = (bool)icdProcessor.ValidationIcd(this.ucIcd) && valid;
                valid = (bool)SubIcdProcessor.GetValidate(this.ucSubIcd) && valid;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        public void ResetRequiredField(bool _isValidate)
        {
            try
            {
                if (_isValidate == false)
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        this.dxValidationProviderControl.RemoveControlError(invalidControls[i]);
                    }
                    this.dxErrorProviderControl.ClearErrors();
                    this.ResetValidate();
                }
                else
                {
                    this.SetValidate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ResetRequiredField(bool _isValidate, bool _isValidateAll)
        {
            try
            {
                if (_isValidate == false)
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        this.dxValidationProviderControl.RemoveControlError(invalidControls[i]);
                    }
                    this.dxErrorProviderControl.ClearErrors();
                    this.ResetValidate();
                }
                else
                {
                    this.SetValidate(_isValidateAll);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void SetValidate(bool isValidAll = false)
        {
            try
            {
                this.ValidateNoiChuyenDen();


                if (isValidAll)
                {
                    ((UCIcd)this.ucIcd).ValidationICD(10, 500, true);
                    this.ValidationSingleControl(dtFromTime, dxValidationProviderControl);
                    this.ValidationSingleControl(dtToTime, dxValidationProviderControl);
                    this.ValidationSingleControl(txtInCode, dxValidationProviderControl);
                    this.ValidationSingleControl(cboChuyenTuyen, dxValidationProviderControl);
                    this.ValidateLookupWithTextEdit(cboHinhThucChuyen, txtMaHinhThucChuyen, dxValidationProviderControl);
                    this.ValidateLookupWithTextEdit(cboLyDoChuyen, txtMaLyDoChuyen, dxValidationProviderControl);

                    this.lciFordtFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciFordtToTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciSoChuyenVien.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciChuyenTuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciHinhThucChuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    this.lciLyDoChuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
                else
                {
                    ((UCIcd)this.ucIcd).ValidationICD(10, 500, IsObligatoryTranferMediOrg);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetValidate()
        {
            try
            {
                dxValidationProviderControl.SetValidationRule(txtMaNoiChuyenDen, null);
                ((UCIcd)this.ucIcd).ResetValidationICD();

                dxValidationProviderControl.SetValidationRule(dtFromTime, null);
                dxValidationProviderControl.SetValidationRule(dtToTime, null);
                dxValidationProviderControl.SetValidationRule(txtInCode, null);
                dxValidationProviderControl.SetValidationRule(cboChuyenTuyen, null);
                dxValidationProviderControl.SetValidationRule(txtMaHinhThucChuyen, null);
                dxValidationProviderControl.SetValidationRule(txtMaLyDoChuyen, null);
                dxValidationProviderControl.SetValidationRule(cboHinhThucChuyen, null);
                dxValidationProviderControl.SetValidationRule(cboLyDoChuyen, null);
                dxValidationProviderControl.SetValidationRule(txtMaNoiChuyenDen, null);

                this.lciFordtFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciFordtToTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciSoChuyenVien.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciChuyenTuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciHinhThucChuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                this.lciLyDoChuyen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateNoiChuyenDen()
        {
            Valid_NoiChuyenDen_Control validNoiChuyenDen = new Valid_NoiChuyenDen_Control();
            validNoiChuyenDen.cboNoiChuyenDen = this.cboNoiChuyenDen;
            validNoiChuyenDen.txtMaNoiChuyenDen = this.txtMaNoiChuyenDen;
            validNoiChuyenDen.ErrorText = MessageUtil.GetMessage(His.UC.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validNoiChuyenDen.ErrorType = ErrorType.Warning;
            dxValidationProviderControl.SetValidationRule(txtMaNoiChuyenDen, validNoiChuyenDen);
            this.lciNoiChuyenDen.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
        }

        private void EnableControl(bool _isEnable)
        {
            this.lciBenhChinh.Enabled = _isEnable;
            this.lciChuyenTuyen.Enabled = _isEnable;
            this.lciHinhThucChuyen.Enabled = _isEnable;
            this.lciLyDoChuyen.Enabled = _isEnable;
            this.lciNoiChuyenDen.Enabled = _isEnable;
            this.lciSoChuyenVien.Enabled = _isEnable;
            this.RefreshUserControl();
        }

        private void DisableSomeControlByKeyConfig(bool _isEnable)
        {
            try
            {
                if (_isEnable == true)
                {
                    lciChuyenTuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciHinhThucChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciLyDoChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCboHinhThucChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCboLyDoChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciChuyenTuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciHinhThucChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciLyDoChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCboHinhThucChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCboLyDoChuyen.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
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

        protected void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
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

        protected void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
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
    }
}
