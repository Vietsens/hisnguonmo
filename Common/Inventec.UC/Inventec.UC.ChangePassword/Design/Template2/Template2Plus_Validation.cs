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
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.UC.ChangePassword.Validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Design.Template2
{
    internal partial class Template2
    {
        #region Validation

        private void ValidControl()
        {
            try
            {
                ValidtxtOldPass();
                ValidtxtNewPass();
                ValidtxtRetypePass();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtOldPass()
        {
            try
            {
                OldPass__ValidationRule oldPassRule = new OldPass__ValidationRule();
                oldPassRule.txtOldPass = txtPreviousPass;
                oldPassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                oldPassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtPreviousPass, oldPassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtNewPass()
        {
            try
            {
                NewPass__ValidationRule newPassRule = new NewPass__ValidationRule();
                newPassRule.txtNewPass = txtNewPass;
                newPassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                newPassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtNewPass, newPassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidtxtRetypePass()
        {
            try
            {
                RetypePass__ValidationRule retypePassRule = new RetypePass__ValidationRule();
                retypePassRule.txtRetypePass = txtRetypePass;
                retypePassRule.txtNewPass = txtNewPass;
                //retypePassRule.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                retypePassRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtRetypePass, retypePassRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
