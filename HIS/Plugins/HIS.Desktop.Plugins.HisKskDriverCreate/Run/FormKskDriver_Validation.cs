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
using HIS.Desktop.Plugins.HisKskDriverCreate.Validation;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.Run
{
    public partial class FormKskDriver : FormBase
    {
        private void SetValidateForm()
        {
            try
            {
                ValidationControlMaxLength(this.dtTimeCccd, null, true);
                ValidationControlMaxLength(this.txtPlaceCccd, 30, true);
                ValidationControlMaxLength(this.dtConclusionTime, null, true);
                ValidationControlMaxLength(this.cboConclusion, null, true);
                ValidationControlMaxLength(this.cboLicenesClass, null, true);
                ValidationControlMaxLength(this.cboConclusionName, null, true); 
                ValidationControlMaxLength(this.txtReasonBadHeathly, 250, false);
                ValidationControlMaxLength(this.txtSickCondition, 250, false);
                //if (spConcentration.EditValue != null && chkMgKhi.Checked == false && chkMgMau.Checked == false)
                //{
                //    ValidationControlConcentration();
                //}
                ValidationSingleControl(this.txtCccdCmnd, dxValidationProvider1, GetErrorValidCccdCmnd, IsValidCccdCmndHc);
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsValidCccdCmndHc()
        {
            bool valid = true;
            try
            {
                valid = (!String.IsNullOrEmpty(this.txtCccdCmnd.Text.Trim()) && (this.txtCccdCmnd.Text.Trim().Length == 8 || this.txtCccdCmnd.Text.Trim().Length == 9 || this.txtCccdCmnd.Text.Trim().Length == 12));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        private string GetErrorValidCccdCmnd()
        {
            return Resources.ResourceMessage.DuLieuKhongDungDinhDang;
        }

        private void ValidationControlConcentration()
        {
            try
            {
                ValidateConcentration validate = new ValidateConcentration();
                validate.chkMgKhi = this.chkMgKhi;
                validate.chkMgMau = this.chkMgMau;
                validate.spConcentration = this.spConcentration;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.spConcentration, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequest = false)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
