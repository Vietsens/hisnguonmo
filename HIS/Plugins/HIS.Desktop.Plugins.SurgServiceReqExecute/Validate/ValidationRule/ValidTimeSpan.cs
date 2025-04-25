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
using HIS.Desktop.Plugins.SurgServiceReqExecute.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Validate.ValidationRule
{
    class ValidTimeSpan : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dateFromEdit;
        internal DevExpress.XtraEditors.DateEdit dateToEdit;
        internal DevExpress.XtraLayout.LayoutControlItem lciDate;
        internal DevExpress.XtraEditors.Controls.CalendarControl calendarControl;
        internal DevExpress.XtraLayout.LayoutControlItem lciCa;
        internal DevExpress.XtraEditors.TimeSpanEdit timeSpanEdit;
        internal long? inTime;
        internal long? outTime;
        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                if (timeSpanEdit.EditValue == null)
                {
                    this.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                    return false;
                }
                //if (lciDate.Visible)
                //{
                //    string dteFrom = dateFromEdit.DateTime.ToString("yyyyMMdd");
                //    string dteTo = dateToEdit.DateTime.ToString("yyyyMMdd");
                //    if (inTime.HasValue && Int64.Parse(dteFrom + (DateTime.Today.Date + timeSpanEdit.TimeSpan).ToString("HHmm")) < Int64.Parse(inTime.ToString().Substring(0, 12)))
                //    {
                //        this.ErrorText = "Thời gian nhỏ hơn thời gian vào viện";
                //        this.ErrorType = ErrorType.Warning;
                //        return false;
                //    }

                //    if (outTime.HasValue && Int64.Parse(dteTo + (DateTime.Today.Date + timeSpanEdit.TimeSpan).ToString("HHmm")) > Int64.Parse(outTime.ToString().Substring(0, 12)))
                //    {
                //        this.ErrorText = "Thời gian lớn hơn thời gian ra viện";
                //        this.ErrorType = ErrorType.Warning;
                //        return false;
                //    }
                //}

                if(lciCa.Visible)
                {
                    if (inTime.HasValue && Int64.Parse(calendarControl.DateTime.ToString("yyyyMMdd") + (DateTime.Today.Date + timeSpanEdit.TimeSpan).ToString("HHmm")) < Int64.Parse(inTime.ToString().Substring(0, 12)))
                    {
                        this.ErrorText = "Thời gian nhỏ hơn thời gian vào viện";
                        this.ErrorType = ErrorType.Warning;
                        return false;
                    }

                    if (outTime.HasValue && Int64.Parse(calendarControl.DateTime.ToString("yyyyMMdd") + (DateTime.Today.Date + timeSpanEdit.TimeSpan).ToString("HHmm")) > Int64.Parse(outTime.ToString().Substring(0, 12)))
                    {
                        this.ErrorText = "Thời gian lớn hơn thời gian ra viện";
                        this.ErrorType = ErrorType.Warning;
                        return false;
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
