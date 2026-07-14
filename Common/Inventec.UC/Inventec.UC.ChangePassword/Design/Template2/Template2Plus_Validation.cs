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

        #region Password complexity (BR01)

        /// <summary>
        /// Rearranges the layout for the complexity feature and shows the requirement hint.
        /// Only runs when config MOS.ACS_USER.PasswordComplexity.Require is enabled; when
        /// disabled the control keeps its original compact layout and the extra labels stay hidden.
        /// </summary>
        private void ApplyPasswordComplexityLayout()
        {
            try
            {
                if (!isRequirePasswordComplexity) return;

                // Warning box right below the "new password" field.
                lblNewPassWarning.Location = new System.Drawing.Point(txtNewPass.Left, txtNewPass.Bottom + 6);
                lblNewPassWarning.Size = new System.Drawing.Size(200, 96);
                lblNewPassWarning.Visible = false;

                // Retype field pushed below the warning box.
                int retypeTop = lblNewPassWarning.Bottom + 10;
                lblRetypePass.Location = new System.Drawing.Point(lblRetypePass.Left, retypeTop + 3);
                txtRetypePass.Location = new System.Drawing.Point(txtRetypePass.Left, retypeTop);

                // Static requirement hint below the retype field.
                lblRequireHint.Location = new System.Drawing.Point(txtRetypePass.Left, txtRetypePass.Bottom + 6);
                lblRequireHint.Size = new System.Drawing.Size(200, 48);
                lblRequireHint.Text = Process.MessageUtil.GetMessage(Message.Message.Enum.YeuCauDoPhucTapMatKhau);
                lblRequireHint.Visible = true;

                // Buttons pushed to the bottom.
                int buttonTop = lblRequireHint.Bottom + 10;
                btnSave.Location = new System.Drawing.Point(btnSave.Left, buttonTop);
                btnRefresh.Location = new System.Drawing.Point(btnRefresh.Left, buttonTop);

                this.Size = new System.Drawing.Size(352, btnSave.Bottom + 10);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Live-updates the warning icon and the inline warning box whenever the new password changes.
        /// </summary>
        private void UpdatePasswordComplexityState()
        {
            try
            {
                if (!isRequirePasswordComplexity) return;

                List<string> missing = GetUnmetPasswordConditions(txtNewPass.Text);
                if (missing == null || missing.Count == 0)
                {
                    dxErrorProvider1.SetError(txtNewPass, "");
                    lblNewPassWarning.Text = "";
                    lblNewPassWarning.Visible = false;
                }
                else
                {
                    string content = Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauChuaDatChuanConThieu)
                        + "\r\n• " + string.Join("\r\n• ", missing);
                    dxErrorProvider1.SetError(txtNewPass, content, ErrorType.Critical);
                    lblNewPassWarning.Text = content;
                    lblNewPassWarning.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Returns the list of complexity conditions (BR01) the given password does not satisfy.
        /// Empty when the password is valid or empty (empty is handled by the required-field rule).
        /// Special character = any character that is neither a letter nor a digit.
        /// </summary>
        private List<string> GetUnmetPasswordConditions(string password)
        {
            List<string> missing = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(password)) return missing;

                if (password.Length < 8)
                    missing.Add(Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauChuaDu8KyTu));
                if (!password.Any(char.IsUpper))
                    missing.Add(Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauThieuChuInHoa));
                if (!password.Any(char.IsLower))
                    missing.Add(Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauThieuChuThuong));
                if (!password.Any(char.IsDigit))
                    missing.Add(Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauThieuChuSo));
                if (!password.Any(c => !char.IsLetterOrDigit(c)))
                    missing.Add(Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauThieuKyTuDacBiet));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return missing;
        }

        /// <summary>
        /// True when the complexity feature is off or the new password satisfies every BR01 condition.
        /// </summary>
        private bool IsPasswordComplexityValid()
        {
            if (!isRequirePasswordComplexity) return true;
            return GetUnmetPasswordConditions(txtNewPass.Text).Count == 0;
        }

        #endregion
    }
}
