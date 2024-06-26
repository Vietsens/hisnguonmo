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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Resources;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Worker
{
    internal class MedicineAgeWorker
    {
        /// <summary>
        /// Bổ sung chặn ở chức năng kê đơn (kê đơn ngoại trú, nội trú, YHCT):
        ///- Khi bổ sung thuốc, nếu tuổi BN nằm ngoài khoảng tuổi được cấu hình của thuốc thì hiển thị thông báo, ko cho bổ sung:
        ///"Thuốc AAA chỉ định dùng cho bệnh nhân từ XX tuổi đến YY tuổi" 
        ///nếu thuốc chỉ cấu hình tuổi từ thì hiển thị: "Thuốc AAA chỉ định dùng cho bệnh nhân từ XX tuổi" 
        ///nếu thuốc chỉ cấu hình tuổi đến thì hiển thị: "Thuốc AAA chỉ định dùng cho bệnh nhân chưa quá YY tuổi"
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        internal static bool ValidThuocCoGioiHanTuoi(long serviceId, long patientDob)
        {
            bool valid = true;
            try
            {
                string messErr = "";
                valid = ValidThuocCoGioiHanTuoi(serviceId, patientDob, ref messErr, true);
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        internal static bool ValidThuocCoGioiHanTuoi(long serviceId, long patientDob, ref string messErr, bool isShowAlert)
        {
            bool valid = true;
            try
            {
                int age = 0;
                var sv = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault();
                var medi = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                long ageMin = (long)(sv.AGE_FROM ?? 0);
                long ageMax = (long)(sv.AGE_TO ?? 0);
                if (sv != null && medi != null && (ageMin > 0 || ageMax > 0))
                {
                    System.DateTime dtPatientDob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDob).Value;
                    TimeSpan diff__Day = (DateTime.Now.Date - dtPatientDob.Date);
                    long tongsogiay = diff__Day.Ticks;
                    System.DateTime newDate = new System.DateTime(tongsogiay);
                    int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                    if (month <= 12)
                    {
                        age = 1;
                    }
                    else
                    {
                        age = System.DateTime.Now.Year - dtPatientDob.Year;
                    }
                    string tuoi_thangtuoi_min = (ageMin > 0 ? (ageMin >= 72 ? "tuổi" : "tháng tuổi") : "");
                    string tuoi_thangtuoi_max = (ageMax > 0 ? (ageMax >= 72 ? "tuổi" : "tháng tuổi") : "");
                    string ageMinShow = (ageMin > 0 ? (ageMin >= 72 ? (long)(ageMin / 12) : ageMin) : 0) + "";
                    string ageMaxShow = (ageMax > 0 ? (ageMax >= 72 ? (long)(ageMax / 12) : ageMax) : 0) + "";
                    string ageMinPlus = (ageMin > 0 ? (ageMin >= 72 ? ageMin % 12 == 0 ? "" : " " + (ageMin % 12) + " tháng" : "") : "") + "";
                    string ageMaxPlus = (ageMax > 0 ? (ageMax >= 72 ? ageMax % 12 == 0 ? "" : " " + (ageMax % 12) + " tháng" : "") : "") + "";

                    if (ageMin > 0 && ageMax > 0 && (ageMin > month || ageMax < month))
                    {
                        messErr = String.Format(ResourceMessage.ThuocChiDinhDungChoBenhNhanTuXDenYTuoi, medi.MEDICINE_TYPE_NAME, ageMinShow, tuoi_thangtuoi_min + ageMinPlus, ageMaxShow, tuoi_thangtuoi_max + ageMaxPlus);
                        valid = false;
                    }
                    else if (ageMin > 0 && ageMax <= 0 && ageMin > month)
                    {
                        messErr = String.Format(ResourceMessage.ThuocChiDinhDungChoBenhNhanTuXTuoi, medi.MEDICINE_TYPE_NAME, ageMinShow, tuoi_thangtuoi_max + ageMaxPlus);
                        valid = false;
                    }
                    else if (ageMax > 0 && ageMin <= 0 && ageMax < month)
                    {
                        messErr = String.Format(ResourceMessage.ThuocChiDinhDungChoBenhNhanChuaQuaYTuoi, medi.MEDICINE_TYPE_NAME, ageMaxShow, tuoi_thangtuoi_min + ageMinPlus);
                        valid = false;
                    }
                }
                if (!valid)
                {
                    if (isShowAlert)
                        MessageManager.Show(messErr);
                    Inventec.Common.Logging.LogSystem.Debug("ValidThuocCoGioiHanTuoi khong hop le____" + messErr + "____PatientAge:" + age);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

    }
}
