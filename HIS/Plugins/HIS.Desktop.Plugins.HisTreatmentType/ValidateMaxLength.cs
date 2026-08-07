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

namespace HIS.Desktop.Plugins.TreatmentType
{
    public class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt;

        /// <summary>
        /// PT-48590 R13: gioi han do dai tinh theo BYTE (cot khai theo byte, ky tu tieng Viet
        /// co dau chiem nhieu hon 1 byte). Mac dinh 5 de giu nguyen hanh vi cu cua o Tien to ma ket thuc.
        /// </summary>
        internal int maxByte = 5;

        /// <summary>Cau bao loi lay tu tep ngon ngu cua plugin, khong viet thang tieng Viet.</summary>
        internal string errorMessage;

        /// <summary>Kiem tra bat buoc nhap truoc khi kiem tra do dai (mot control chi nhan mot rule).</summary>
        internal bool isRequired;

        internal string requiredMessage;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (this.isRequired && string.IsNullOrWhiteSpace(txt.Text))
                {
                    this.ErrorText = this.requiredMessage;
                    return valid;
                }
                if (!string.IsNullOrEmpty(txt.Text) && Inventec.Common.String.CountVi.Count(txt.Text) > this.maxByte)
                {
                    this.ErrorText = string.IsNullOrEmpty(this.errorMessage)
                        ? string.Format("Chỉ được nhập tối đa {0} byte", this.maxByte)
                        : string.Format(this.errorMessage, this.maxByte);
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
