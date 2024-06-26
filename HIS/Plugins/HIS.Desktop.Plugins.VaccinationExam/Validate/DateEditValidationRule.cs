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

namespace HIS.Desktop.Plugins.VaccinationExam.Validate
{
    class DateEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtAppointmentTime;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtAppointmentTime == null) return valid;
                if (String.IsNullOrEmpty(dtAppointmentTime.Text) || dtAppointmentTime.EditValue == null)
                {
                    this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                    return valid;
                }

                if ((Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointmentTime.DateTime) ?? 0) < (Inventec.Common.DateTime.Get.EndDay() ?? 0))
                {
                    this.ErrorText = "Thời gian hẹn phải lớn hơn ngày hiện tại";
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
