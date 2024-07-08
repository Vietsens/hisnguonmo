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

namespace His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule
{
    class TemplateHeinBHYT1__HNCode__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHNCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtHNCode == null) return valid;
                string hncode = txtHNCode.Text.Trim();
                if (!txtHNCode.Enabled || (txtHNCode.Enabled && (String.IsNullOrEmpty(hncode))))
                    return true;

                //if (hncode.Length != 9 && hncode.Length != 12)
                //{
                //    return valid;
                //}
                //else if (hncode.Length == 12 && !hncode.EndsWith("VCN"))
                //{
                //    return valid;
                //}
                //#18611
                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(hncode, 20))
                {
                    this.ErrorText = "Nhập quá kí tự cho phép (20)";
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
