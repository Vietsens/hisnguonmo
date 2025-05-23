﻿/* IVT
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

namespace HIS.Desktop.Plugins.TreatmentIcdEdit.Validation
{
    class GridLookupReasonEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cbo;
        internal DevExpress.XtraEditors.TextEdit txt;
        internal bool IsRequired;
        internal int MaxLengthCode;
        internal int MaxLengthName;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cbo == null) return valid;

                if (IsRequired && (cbo.EditValue == null || String.IsNullOrWhiteSpace(cbo.EditValue.ToString()) || string.IsNullOrEmpty(txt.Text.Trim())))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                if(!string.IsNullOrEmpty(txt.Text.Trim()) && Inventec.Common.String.CountVi.Count(txt.Text.Trim()) > MaxLengthCode)
                {
                    this.ErrorText = "Mã vượt ký tự cho phép, " + MaxLengthCode + " ký tự";
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                if (!string.IsNullOrEmpty(cbo.Text.Trim()) && Inventec.Common.String.CountVi.Count(cbo.Text.Trim()) > MaxLengthName)
                {
                    this.ErrorText = "Tên vượt ký tự cho phép, " + MaxLengthName + " ký tự";
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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
