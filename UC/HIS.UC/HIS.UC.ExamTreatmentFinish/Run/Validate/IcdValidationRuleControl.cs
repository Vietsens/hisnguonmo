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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.ExamTreatmentFinish.Run.Validate
{
    class IcdValidationRuleControl : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtIcdCode;
        internal DevExpress.XtraEditors.TextEdit txtMainText;
        internal DevExpress.XtraEditors.GridLookUpEdit btnBenhChinh;
        internal DevExpress.XtraEditors.CheckEdit chkCheck;
        internal int? maxLengthCode;
        internal int? maxLengthText;
        internal bool IsObligatoryTranferMediOrg;
        internal bool IsRequired = true;
        internal List<HIS_ICD> icds;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtIcdCode == null
                    || btnBenhChinh == null
                    || txtMainText == null
                    || chkCheck == null)
                    return valid;

                if (maxLengthCode != null)
                {
                    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtIcdCode.Text.Trim(), maxLengthCode ?? 0))
                    {
                        this.ErrorText = "Mã bệnh chính vượt quá ký tự cho phép";
                        return valid;
                    }
                }

                if (IsRequired && !String.IsNullOrEmpty(txtIcdCode.Text.Trim()) && !icds.Exists(o=>o.ICD_CODE.Equals(txtIcdCode.Text.Trim())))
                {
                    this.ErrorText = "Bạn nhập mã chẩn đoán không đúng. Vui lòng kiểm tra lại";
                    return valid;
                }


                if (chkCheck.Checked)
                {
                    if (maxLengthText != null)
                    {
                        if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtMainText.Text.Trim(), maxLengthText ?? 0))
                        {
                            this.ErrorText = "Tên bệnh chính vượt quá ký tự cho phép";
                            return valid;
                        }
                    }
                    if (IsObligatoryTranferMediOrg)
                    {
                        if (IsRequired && string.IsNullOrEmpty(txtMainText.Text))
                            return valid;
                    }
                    else if (IsRequired && (string.IsNullOrEmpty(txtIcdCode.Text)
                        || string.IsNullOrEmpty(txtMainText.Text)))
                        return valid;
                }
                else
                {
                    if (IsRequired && (string.IsNullOrEmpty(txtIcdCode.Text)
                        || btnBenhChinh.EditValue == null))
                        return valid;
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
