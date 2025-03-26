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
using IMSys.DbConfig.HIS_RS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Common.Treatment
{
    public class PatientTypeEnum
    {
        public enum TYPE
        {
            BHYT,
            THU_PHI
        }
    }

    public static class Calculation
    {
        /// <summary>
        /// -- 1. Các TH sau tính ngày điều trị = ngày ra - ngày vào + 1:
        //+ Kết quả điều trị: không thay đổi, xử trí xin về
        //+ Kết quả điều trị: không thay đổi, xử trí chuyển viện
        //+ Kết quả điều trị: không thay đổi, xử trí tử xong
        //+ Kết quả điều trị: nặng hơn, xử trí xin về
        //+ Kết quả điều trị: nặng hơn, xử trí chuyển viện
        //+ Kết quả điều trị: nặng hơn, xử trí tử xong
        //-- 2. Các trường hợp còn lại tính ngày điều trị = ngày ra - ngày vào
        //-- 3. TH bn vào viện cùng 1 ngày có thời gian điều trị> 4h vẫn tính là 1 ngày điều trị như hiện tại.
        /// </summary>
        /// <param name="timeIn"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static long? DayOfTreatment(long? timeIn, long? timeOut, long? treatmentEndTypeId, long? treatmentResultId, PatientTypeEnum.TYPE patientTypeEnum)
        {
            long? result = null;
            try
            {
                if (!timeIn.HasValue || !timeOut.HasValue || !treatmentEndTypeId.HasValue
                    || !treatmentResultId.HasValue || timeIn > timeOut)
                    return result;

                DateTime dtIn = TimeNumberToSystemDateTime(timeIn.Value) ?? DateTime.Now;
                DateTime dtOut = TimeNumberToSystemDateTime(timeOut.Value) ?? DateTime.Now;
                TimeSpan ts = new TimeSpan();
                ts = (TimeSpan)(dtOut - dtIn);

                //Cung 1 ngay
                if (timeIn.Value.ToString().Substring(0, 8) == timeOut.Value.ToString().Substring(0, 8))
                {
                    if (ts.TotalMinutes <= 1440 && ts.TotalMinutes > 240)
                    {
                        result = 1;
                    }
                    else if (ts.TotalMinutes <= 240)
                    {
                        result = 0;
                    }
                }
                else if (ts.TotalMinutes < 1440) //Khac 1 ngay
                {
                    result = 1;
                }
                else
                {
                    //Nếu thời gian vào trước ngày 15/07/2018. Số ngày điều trị tính theo thông tư 37
                    //Nếu thười gian vào sau ngày 15/07/2018. Số ngày điều trị tính theo thông tư 15
                    if (timeIn.Value < 20180715000000 || patientTypeEnum == PatientTypeEnum.TYPE.THU_PHI)
                    {
                        result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays + 1;
                    }
                    else if (patientTypeEnum == PatientTypeEnum.TYPE.BHYT)
                    {
                        if (treatmentEndTypeId.Value == HIS_TREATMENT_END_TYPE.ID__CHUYEN
                            || treatmentResultId.Value == HIS_TREATMENT_RESULT.ID__KTD
                            || treatmentResultId.Value == HIS_TREATMENT_RESULT.ID__NANG
                            || treatmentResultId.Value == HIS_TREATMENT_RESULT.ID__CHET)
                        {
                            result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays + 1;
                        }
                        else
                            result = (int)((TimeSpan)(dtOut.Date - dtIn.Date)).TotalDays;
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
            }
            return result;
        }

        private static System.DateTime? TimeNumberToSystemDateTime(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    result = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Bệnh nhân điều trị nội trú, điều trị ngoại trú
        /// có thời gian ra - thời gian vào lớn hơn 24h sẽ tính bằng công thức: ngày ra - ngày vào + 1
        /// Bệnh nhân điều trị nội trú có thời gian ra - thời gian vào nhỏ hơn hoặc bằng 24h thì số ngày điều trị = 1
        /// Ngoài các trường hợp trên sẽ không có thông tin số ngày
        /// </summary>
        /// <param name="in_time">Thời gian vào viện</param>
        /// <param name="clinical_in_time">Thời gian vào điều trị</param>
        /// <param name="out_time">Thời gian ra viện</param>
        /// <param name="treatment_type_id">Đối tượng bệnh nhân</param>
        /// <returns></returns>
        public static long? DayOfTreatment6556(long in_time, long? clinical_in_time, long? out_time, long treatment_type_id)
        {
            long? result = null;
            try
            {
                if (out_time.HasValue)
                {
                    if (clinical_in_time.HasValue)
                    {
                        in_time = clinical_in_time.Value;
                    }

                    System.DateTime? dateBefore = TimeNumberToSystemDateTime(in_time);
                    System.DateTime? dateAfter = TimeNumberToSystemDateTime(out_time.Value);
                    if (dateBefore != null && dateAfter != null)
                    {
                        TimeSpan difference = dateAfter.Value - dateBefore.Value;

                        //Lớn hơn 24h thì ngày ra - ngày vào + 1;
                        if ((difference.Days > 1 || (difference.Days == 1 && (difference.Hours >= 1 || difference.Minutes >= 1 || difference.Seconds >= 1))) && treatment_type_id != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            result = (int)((TimeSpan)(dateAfter.Value.Date - dateBefore.Value.Date)).TotalDays + 1;
                        }
                        else if (treatment_type_id == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                        {
                            result = 1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
