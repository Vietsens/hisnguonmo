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

                // Labels auto-size their HEIGHT to the text -> adapts to larger fonts / high DPI on
                // other machines (no clipping). Widths, positions and the dialog size are computed in
                // LayoutComplexity (runs after the handle exists, so the font scale is known).
                lblNewPassWarning.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
                lblNewPassWarning.Visible = false;

                lblRequireHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
                lblRequireHint.Text = Process.MessageUtil.GetMessage(Message.Message.Enum.YeuCauDoPhucTapMatKhau);
                lblRequireHint.Visible = true;
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

                // Scale everything by the runtime font/DPI ratio vs the design metrics so the dialog
                // adapts to larger fonts on other machines (text never clipped).
                System.Drawing.SizeF cur = this.CurrentAutoScaleDimensions;
                System.Drawing.SizeF des = this.AutoScaleDimensions;
                float baseW = des.Width > 0.1f ? des.Width : 6f;
                float baseH = des.Height > 0.1f ? des.Height : 13f;
                float sx = cur.Width > 0.1f ? cur.Width / baseW : 1f;
                float sy = cur.Height > 0.1f ? cur.Height / baseH : 1f;

                int margin = (int)System.Math.Round(SideMargin * sx);
                int gap = (int)System.Math.Round(8 * sy);
                int formW = (int)System.Math.Round(UcWidthOn * sx);
                int iconRoom = (int)System.Math.Round(22 * sx); // room for the DXErrorProvider icon

                // Input fields: from their (already font-scaled) left edge to the right margin.
                int fieldW = formW - txtNewPass.Left - margin - iconRoom;
                if (fieldW > 60)
                {
                    txtPreviousPass.Width = fieldW;
                    txtNewPass.Width = fieldW;
                    txtRetypePass.Width = fieldW;
                }

                // Full-width labels; height auto-fits the text at the current font.
                lblNewPassWarning.Left = margin;
                lblNewPassWarning.Width = formW - 2 * margin;
                lblRequireHint.Left = margin;
                lblRequireHint.Width = formW - 2 * margin;

                int retypeTop;
                if (showWarning)
                {
                    lblNewPassWarning.Top = txtNewPass.Bottom + (int)System.Math.Round(6 * sy);
                    lblNewPassWarning.Visible = true;
                    retypeTop = lblNewPassWarning.Bottom + gap;
                }
                else
                {
                    lblNewPassWarning.Visible = false;
                    retypeTop = txtNewPass.Bottom + gap;
                }

                txtRetypePass.Top = retypeTop;
                lblRetypePass.Top = retypeTop + (int)System.Math.Round(3 * sy);

                lblRequireHint.Top = txtRetypePass.Bottom + (int)System.Math.Round(6 * sy);

                int buttonTop = lblRequireHint.Bottom + (int)System.Math.Round(12 * sy);
                int btnW = (formW - 3 * margin) / 2;
                int btnH = (int)System.Math.Round(38 * sy);
                btnSave.SetBounds(margin, buttonTop, btnW, btnH);
                btnRefresh.SetBounds(2 * margin + btnW, buttonTop, btnW, btnH);

                int neededHeight = btnSave.Bottom + margin;
                System.Windows.Forms.Form host = this.FindForm();
                if (host != null && (host.ClientSize.Width != formW || host.ClientSize.Height != neededHeight))
                {
                    host.ClientSize = new System.Drawing.Size(formW, neededHeight);
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
