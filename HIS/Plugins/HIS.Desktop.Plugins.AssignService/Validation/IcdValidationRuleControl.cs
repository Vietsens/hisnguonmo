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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.Validation
{
    class IcdValidationRuleControl : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtIcdCode;
        internal DevExpress.XtraEditors.TextEdit txtMainText;
        internal Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn btnBenhChinh;
        internal DevExpress.XtraEditors.CheckEdit chkCheck;
        internal int? maxLengthCode;
        internal int? maxLengthText;
        internal bool IsObligatoryTranferMediOrg;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtIcdCode == null
                    || btnBenhChinh == null
                    || txtMainText == null
                    || chkCheck == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("txtIcdCode == null || btnBenhChinh == null || txtMainText == null                   || chkCheck == null");
                    return valid;
                }

                if (maxLengthCode != null)
                {
                    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtIcdCode.Text.Trim(), maxLengthCode ?? 0))
                    {
                        this.ErrorText = "Mã bệnh chính vượt quá ký tự cho phép";
                        Inventec.Common.Logging.LogSystem.Debug("Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtIcdCode.Text.Trim(), maxLengthCode ?? 0)");
                        return valid;
                    }
                }

                if (!String.IsNullOrEmpty(this.ErrorText)
                    && this.ErrorText != Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc))
                {
                    this.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                    Inventec.Common.Logging.LogSystem.Debug("!String.IsNullOrEmpty(this.ErrorText) && txtIcdCode.ErrorText != Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc)");
                    return valid;
                }


                if (chkCheck.Checked)
                {
                    if (maxLengthText != null)
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtMainText.Text.Trim(), maxLengthText ?? 0))
                        {
                            this.ErrorText = "Tên bệnh chính vượt quá ký tự cho phép";
                            Inventec.Common.Logging.LogSystem.Debug("Tên bệnh chính vượt quá ký tự cho phép");
                            return valid;
                        }
                    }
                    if (IsObligatoryTranferMediOrg)
                    {
                        if (string.IsNullOrEmpty(txtMainText.Text))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("string.IsNullOrEmpty(txtMainText.Text)");
                            return valid;
                        }
                    }
                    else if (string.IsNullOrEmpty(txtIcdCode.Text)
                        || string.IsNullOrEmpty(txtMainText.Text))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("string.IsNullOrEmpty(txtIcdCode.Text) || string.IsNullOrEmpty(txtMainText.Text)");
                        return valid;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(txtIcdCode.Text)
                        || btnBenhChinh.EditValue == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("string.IsNullOrEmpty(txtIcdCode.Text) || btnBenhChinh.EditValue == null");
                        return valid;
                    }
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
