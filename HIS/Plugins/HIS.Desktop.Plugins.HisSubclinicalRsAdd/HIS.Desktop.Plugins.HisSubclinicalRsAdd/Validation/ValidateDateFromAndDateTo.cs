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
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisSubclinicalRsAdd.Validtion
{
    public class ValidateDateFromAndDateTo : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ComboBoxEdit cboFrom;
        internal DevExpress.XtraEditors.ComboBoxEdit cboTo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboFrom == null || cboFrom == null) return valid;

                if (cboFrom.SelectedIndex < 0 || cboTo.SelectedIndex < 0)
                {
                    valid = true;
                    return valid;
                }

                long timeFrom = ConvertTimeStringToLong((cboFrom.EditValue ?? "0").ToString());
                long timeTo = ConvertTimeStringToLong((cboTo.EditValue ?? "0").ToString());
                if (timeFrom > timeTo)
                {
                    this.ErrorText = "Ngày chỉ định từ bé hơn hoặc bằng ngày chỉ định đến";
                    this.ErrorType = ErrorType.Warning;
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

        private long ConvertTimeStringToLong(string time)
        {
            long result = 0;
            try
            {
                switch (time)
                {
                    case "Chủ nhật":
                        result = 1;
                        break;
                    case "Thứ 2":
                        result = 2;
                        break;
                    case "Thứ 3":
                        result = 3;
                        break;
                    case "Thứ 4":
                        result = 4;
                        break;
                    case "Thứ 5":
                        result = 5;
                        break;
                    case "Thứ 6":
                        result = 6;
                        break;
                    case "Thứ 7":
                        result = 7;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
