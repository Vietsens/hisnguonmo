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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqSampleInfo.Validation
{
    class SampleTimeValidationRule : ValidationRule
    {
        //internal long intructionTime;
        internal DevExpress.XtraEditors.DateEdit dtSampleTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtSampleTime == null) return valid;
                if (dtSampleTime.EditValue == null || dtSampleTime.DateTime == DateTime.MinValue)
                {
                    ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = ErrorType.Warning;
                    return valid;
                }
                //long time = Convert.ToInt64(dtSampleTime.DateTime.ToString("yyyyMMddHHmmss"));
                //if (time < intructionTime)
                //{
                //    string s = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime);
                //    ErrorText = String.Format("Thời gian duyệt mẫu không được nhỏ hơn thời gian y lệnh: {0}", s);
                //    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                //    return valid;
                //}
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
