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
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Validtion
{
    class InstructionTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtInstructionTime;
        internal DevExpress.XtraEditors.DateEdit dtStartTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            long? startTime = null;
            long instructionTime = 0;
            try
            {
                if (dtInstructionTime == null)
                    return valid;
                if (String.IsNullOrEmpty(dtInstructionTime.Text))
                {
                    ErrorText = ResourceMessage.GiaTriBatBuocNhap;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }

                if (dtStartTime.EditValue != null && dtStartTime.DateTime != DateTime.MinValue)
                    startTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtStartTime.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (dtInstructionTime.EditValue != null && dtInstructionTime.DateTime != DateTime.MinValue)
                    instructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtInstructionTime.EditValue).ToString("yyyyMMddHHmm") + "00");

                if (startTime < instructionTime)
                {
                    ErrorText = ResourceMessage.ThoiGianYLenhBeHonThoiGianBatDau;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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
