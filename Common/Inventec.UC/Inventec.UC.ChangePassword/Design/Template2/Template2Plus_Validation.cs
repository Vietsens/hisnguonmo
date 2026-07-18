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

        private const int UcWidthOn = 360;
        private const int FieldWidthOn = 190;
        private const int SideMargin = 12;

        /// <summary>
        /// Sets the static properties for the complexity feature (field widths, auto-size labels,
        /// button bounds, requirement hint). Actual positioning + dialog sizing is done by
        /// LayoutComplexity (needs the host form, so it runs from the Load event / on change).
        /// Only runs when config MOS.ACS_USER.PasswordComplexity.Require is enabled; otherwise the
        /// control keeps its original compact layout and the extra labels stay hidden.
        /// </summary>
        private void ApplyPasswordComplexityLayout()
        {
            try
            {
                if (!isRequirePasswordComplexity) return;

                // Wider input fields for a less cramped look (ON mode only).
                txtPreviousPass.Width = FieldWidthOn;
                txtNewPass.Width = FieldWidthOn;
                txtRetypePass.Width = FieldWidthOn;

                // Warning banner: full width; height is set to fit its content in
                // UpdatePasswordComplexityState (deterministic, avoids auto-size timing issues).
                lblNewPassWarning.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
                lblNewPassWarning.Left = SideMargin;
                lblNewPassWarning.Width = UcWidthOn - 2 * SideMargin;
                lblNewPassWarning.Visible = false;

                // Requirement hint: full width (wraps to ~2 lines, never clipped), explicit fixed size
                // so it renders reliably (LabelControl auto-size can collapse height to 0).
                lblRequireHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
                lblRequireHint.Left = SideMargin;
                lblRequireHint.Size = new System.Drawing.Size(UcWidthOn - 2 * SideMargin, 38);
                lblRequireHint.Text = Process.MessageUtil.GetMessage(Message.Message.Enum.YeuCauDoPhucTapMatKhau);
                lblRequireHint.Visible = true;

                // Two equal buttons across the bottom.
                int btnWidth = (UcWidthOn - 3 * SideMargin) / 2;
                btnSave.SetBounds(SideMargin, btnSave.Top, btnWidth, 36);
                btnRefresh.SetBounds(2 * SideMargin + btnWidth, btnRefresh.Top, btnWidth, 36);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Positions the retype field, hint and buttons and resizes the host dialog to fit its
        /// content. When showWarning is false the warning banner is hidden and the retype block sits
        /// right under the "new password" field (no empty gap). When true, the banner is shown and
        /// everything below shifts down; the dialog grows/shrinks accordingly.
        /// </summary>
        private void LayoutComplexity(bool showWarning)
        {
            try
            {
                if (!isRequirePasswordComplexity) return;

                int gap = 8;
                int retypeTop;
                if (showWarning)
                {
                    lblNewPassWarning.Top = txtNewPass.Bottom + 6;
                    lblNewPassWarning.Visible = true;
                    retypeTop = lblNewPassWarning.Bottom + gap;
                }
                else
                {
                    lblNewPassWarning.Visible = false;
                    retypeTop = txtNewPass.Bottom + gap;
                }

                txtRetypePass.Top = retypeTop;
                lblRetypePass.Top = retypeTop + 3;

                lblRequireHint.Top = txtRetypePass.Bottom + 6;

                int buttonTop = lblRequireHint.Bottom + 12;
                btnSave.Top = buttonTop;
                btnRefresh.Top = buttonTop;

                int neededHeight = btnSave.Bottom + 12;
                System.Windows.Forms.Form host = this.FindForm();
                if (host != null && host.ClientSize.Height != neededHeight)
                {
                    host.ClientSize = new System.Drawing.Size(UcWidthOn, neededHeight);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Live-updates the warning icon + inline banner and reflows the dialog whenever the new
        /// password changes.
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
                    LayoutComplexity(false);
                }
                else
                {
                    string content = Process.MessageUtil.GetMessage(Message.Message.Enum.MatKhauChuaDatChuanConThieu)
                        + "\r\n• " + string.Join("\r\n• ", missing);
                    dxErrorProvider1.SetError(txtNewPass, content, ErrorType.Critical);
                    lblNewPassWarning.Text = content;
                    // Header line + one line per missing condition (+ padding/border).
                    lblNewPassWarning.Height = (1 + missing.Count) * 16 + 12;
                    LayoutComplexity(true);
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
