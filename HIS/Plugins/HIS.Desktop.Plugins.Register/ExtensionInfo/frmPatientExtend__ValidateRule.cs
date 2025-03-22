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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Register.ValidationRule;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.PatientExtend
{
    public partial class frmPatientExtend : HIS.Desktop.Utility.FormBase
    {
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtCmndNumber, dxValidationProviderControl, "Dữ liệu không đúng định dạng, CMND/CCCD phải 9 hoặc 12 ký tự", ValidCmnd);
                if (!patientInformation.IsNoCCCD && (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2"))
                {
                    layoutControlItem25.AppearanceItemCaption.ForeColor = Color.Maroon;
                    ValidateMaxlengthTextEdit(this.txtCmndNumber, 12, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlengthTextEdit(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali = false)
        {
            try
            {
                TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
                _rule.txtEdit = txtEdit;
                _rule.maxlength = maxlength;
                _rule.isVali = isVali;
                _rule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _rule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtEdit, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool ValidCmnd()
        {
            try
            {
                if (!String.IsNullOrEmpty(txtCmndNumber.Text) && (txtCmndNumber.Text.Trim().Length != 9 && txtCmndNumber.Text.Trim().Length != 12))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
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
                    validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
    }
}
