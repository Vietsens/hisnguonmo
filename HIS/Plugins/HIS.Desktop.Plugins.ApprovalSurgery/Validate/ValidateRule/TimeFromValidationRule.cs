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

namespace HIS.Desktop.Plugins.ApprovalSurgery.CreateCalendar.ValidationRule
{
    class TimeFromValidationRule :
DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit timeFrom;
        internal DevExpress.XtraEditors.DateEdit timeTo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (timeFrom.EditValue == null)
                {
                    this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                    return valid;
                }

                if (timeTo.EditValue != null && timeFrom.DateTime > timeTo.DateTime)
                {
                    this.ErrorText = "Thời gian từ lớn hơn thời gian dự kiến đến";
                    return valid;
                }
                //if (timè.EditValue != null && startTime.DateTime > DateTime.Now)
                //{
                //    this.ErrorText = "Thời gian bắt đầu không được lớn hơn thời gian hiện tại";
                //    return valid;
                //}

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
