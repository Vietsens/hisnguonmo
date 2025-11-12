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
using ACS.SDO;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.Validate.ValidateRule
{
    public class ValidateStartTime : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtpStartTime;
        internal V_HIS_SERVICE_REQ HisServiceReqView;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtpStartTime == null) return valid;
                if (HisServiceReqView == null) return valid;
                var startTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtpStartTime.DateTime);
                var finishTime = HisServiceReqView.FINISH_TIME;
                var instructionTime = HisServiceReqView.INTRUCTION_TIME;
                var currentTime = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam()).LocalTime;
                if (finishTime != null && startTime >= finishTime)
                {
                    this.ErrorText = "Thời gian bắt đầu không được trùng hoặc lớn hơn thời gian kết thúc y lệnh "
                        + Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(finishTime.Value).Value.ToString("HH:mm dd/MM/yyyy");
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (instructionTime != 0 && startTime < instructionTime)
                {
                    this.ErrorText = "Thời gian bắt đầu không được nhỏ hơn thời gian y lệnh "
                        + Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(instructionTime).Value.ToString("HH:mm dd/MM/yyyy");
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (startTime > currentTime)
                {
                    this.ErrorText = "Thời gian bắt đầu không được lớn hơn thời gian hiện tại "
                        + Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentTime).Value.ToString("HH:mm dd/MM/yyyy");
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
    }
}
