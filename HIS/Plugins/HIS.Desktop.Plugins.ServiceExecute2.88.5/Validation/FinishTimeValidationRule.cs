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

namespace HIS.Desktop.Plugins.ServiceExecute.ValidationRule
{
    class FinishTimeValidationRule :
DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit startTime;
        internal DevExpress.XtraEditors.DateEdit finishTime;
        internal long treatmentOutTime;
        internal long instructionTime;
        internal bool keyCheck;//#19893
        internal bool keyCheckStatsTime;//#20201

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                if (finishTime.EditValue == null)
                    return valid;
                List<string> errMess = new List<string>();
                long timeFinish = finishTime.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(finishTime.DateTime.ToString("yyyyMMddHHmm") + "00") : 0;
                if (startTime.EditValue != null && finishTime.EditValue != null && startTime.DateTime > finishTime.DateTime)
                {
                    errMess.Add(ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianBatDau);
                    valid = false;
                }
                //#20113
                if (!keyCheckStatsTime)
                {
                    if (finishTime.EditValue != null && finishTime.DateTime > DateTime.Now)
                    {
                        errMess.Add(ResourceMessage.ThoiGianKetThucKhongDuocLonHonThoiGianHeThong);
                        valid = false;
                    }
                }
                if (!keyCheck)
                    if (timeFinish < instructionTime)
                    {
                        // errMess.Add(String.Format(ResourceMessage.ThoiGianKetThucThoiGianVaoVien));
                        errMess.Add(String.Format(ResourceMessage.ThoiGianKetThucKhongDuocNhoHonThoiGianYLenh));
                        valid = false;
                    }
                if (treatmentOutTime > 0 && timeFinish > treatmentOutTime)
                {
                    errMess.Add(String.Format(ResourceMessage.ThoiGianKetThucThoiGianRaVien));
                    valid = false;
                }
                if (!valid)
                {
                    this.ErrorText = String.Join(";", errMess);
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
