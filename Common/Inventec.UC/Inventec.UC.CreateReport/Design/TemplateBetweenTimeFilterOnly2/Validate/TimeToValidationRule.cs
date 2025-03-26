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
using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2.Validate
{
    class TimeToValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit deTimeToDate;
        internal DevExpress.XtraEditors.DateEdit deTimeFromDate;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                valid = valid && (deTimeToDate != null) && (deTimeFromDate != null);
                if (valid)
                {
                    var dateTimeTo = GlobalStore.ConvertDateStringToSystemDate(deTimeToDate.Text);
                    var dataTimeFrom = GlobalStore.ConvertDateStringToSystemDate(deTimeFromDate.Text);

                    //if (dateTimeTo == null || dataTimeFrom == null)
                    //{
                    //    valid = false;
                    //}
                    if (dateTimeTo.Value == DateTime.MinValue || dataTimeFrom.Value == DateTime.MinValue)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = valid && (dateTimeTo != null && dateTimeTo.Value != DateTime.MinValue);
                        if (valid && dateTimeTo.Value < dataTimeFrom.Value)
                        {
                            valid = false;
                            this.ErrorText = Base.MessageUtil.GetMessage(Inventec.UC.CreateReport.MessageLang.Message.Enum.HeThongThongBaoThoiGianDenBeHonThoiGianTu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
