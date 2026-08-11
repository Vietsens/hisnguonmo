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
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.UC.ExamTreatmentFinish.Base
{
    /// <summary>
    /// PT-53438: Chan hen kham khi benh an ngoai tru cua benh nhan man tinh da qua thoi han 1 nam.
    /// Chi kiem tra khi cau hinh MOS.HIS_TREATMENT.IS_BLOCK_APPOINTMENT_WHEN_OUT_PATIENT_MEDI_RECORD_OVER_ONE_YEAR = 1.
    /// </summary>
    internal class AppointmentMediRecordExpiryWorker
    {
        private const string URI_HIS_MEDI_RECORD_GET = "api/HisMediRecord/Get";
        private const string URI_HIS_TREATMENT_GET = "api/HisTreatment/Get";

        /// <summary>
        /// Kiem tra lan hen kham co bi chan vi benh an ngoai tru qua 1 nam hay khong.
        /// Tra ve true = CHAN. Moi truong hop khong du dieu kien / thieu du lieu deu tra ve false (khong chan).
        /// </summary>
        /// <param name="treatment">Luot dieu tri dang xu ly</param>
        /// <param name="mediRecordCache">Benh an da lay truoc do (neu co) — dung lai de khong goi API lap</param>
        internal static bool IsBlockedByExpiredOutPatientMediRecord(HIS_TREATMENT treatment, long treatmentId, ref HIS_MEDI_RECORD mediRecordCache)
        {
            bool result = false;
            try
            {
                // R1: cau hinh khac 1 -> thoat som, khong truy van du lieu
                if (!Config.HisConfig.IsBlockAppointmentWhenOutPatientMediRecordOverOneYear)
                {
                    Inventec.Common.Logging.LogSystem.Debug("PT53438.CheckAppointmentExpiry____Cau hinh MOS.HIS_TREATMENT.IS_BLOCK_APPOINTMENT_WHEN_OUT_PATIENT_MEDI_RECORD_OVER_ONE_YEAR khac 1 ==> bo qua kiem tra.");
                    return false;
                }

                long currentTreatmentId = (treatment != null && treatment.ID > 0) ? treatment.ID : treatmentId;
                long? mediRecordId = treatment != null ? treatment.MEDI_RECORD_ID : null;
                long? treatmentTypeId = treatment != null ? treatment.TDL_TREATMENT_TYPE_ID : null;

                // Man hinh khong giu san doi tuong luot dieu tri -> nap lai theo ma luot dieu tri
                if ((!mediRecordId.HasValue || mediRecordId.Value <= 0) && currentTreatmentId > 0)
                {
                    HIS_TREATMENT treatmentFromApi = GetTreatment(currentTreatmentId);
                    if (treatmentFromApi != null)
                    {
                        mediRecordId = treatmentFromApi.MEDI_RECORD_ID;
                        treatmentTypeId = treatmentFromApi.TDL_TREATMENT_TYPE_ID;
                    }
                }

                // Muc 3.5: chi ap dung cho benh nhan ngoai tru -> loai tru luot dieu tri noi tru
                if (treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                {
                    Inventec.Common.Logging.LogSystem.Debug("PT53438.CheckAppointmentExpiry____Luot dieu tri noi tru ==> khong chan. TREATMENT_ID="
                        + currentTreatmentId);
                    return false;
                }

                if (!mediRecordId.HasValue || mediRecordId.Value <= 0)
                {
                    // R2: luot dieu tri khong gan benh an -> khong phai ngoai tru man tinh -> khong chan
                    Inventec.Common.Logging.LogSystem.Debug("PT53438.CheckAppointmentExpiry____Luot dieu tri khong gan benh an ==> khong chan. TREATMENT_ID="
                        + currentTreatmentId);
                    return false;
                }

                HIS_MEDI_RECORD mediRecord = GetMediRecord(mediRecordId.Value, ref mediRecordCache);
                if (mediRecord == null)
                {
                    // R9: khong lay duoc benh an -> khong chan
                    Inventec.Common.Logging.LogSystem.Warn("PT53438.CheckAppointmentExpiry____Khong lay duoc thong tin benh an. MEDI_RECORD_ID=" + mediRecordId.Value);
                    return false;
                }

                // R3 + R4: han = thoi diem tao benh an + 1 nam, so sanh theo ngay
                DateTime? createTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(mediRecord.CREATE_TIME ?? 0);
                if (!createTime.HasValue || createTime.Value == DateTime.MinValue)
                {
                    // R9: khong xac dinh duoc thoi diem tao benh an -> khong chan
                    Inventec.Common.Logging.LogSystem.Warn("PT53438.CheckAppointmentExpiry____Benh an khong co thoi diem tao ==> khong chan. MEDI_RECORD_ID=" + mediRecord.ID);
                    return false;
                }

                DateTime expiryDate = createTime.Value.Date.AddYears(1);
                DateTime today = DateTime.Now.Date; // R5: so voi ngay thuc hien xu tri

                result = today > expiryDate;

                Inventec.Common.Logging.LogSystem.Debug("PT53438.CheckAppointmentExpiry____"
                    + "MEDI_RECORD_ID=" + mediRecord.ID
                    + ", PROGRAM_ID=" + (mediRecord.PROGRAM_ID.HasValue ? mediRecord.PROGRAM_ID.Value.ToString() : "null")
                    + ", CREATE_TIME=" + (mediRecord.CREATE_TIME.HasValue ? mediRecord.CREATE_TIME.Value.ToString() : "null")
                    + ", ExpiryDate=" + expiryDate.ToString("dd/MM/yyyy")
                    + ", Today=" + today.ToString("dd/MM/yyyy")
                    + ", IsBlocked=" + result);
            }
            catch (Exception ex)
            {
                // R9: loi ky thuat -> khong chan, chi ghi log
                result = false;
                Inventec.Common.Logging.LogSystem.Error("PT53438.CheckAppointmentExpiry____Loi ky thuat ==> khong chan.", ex);
            }
            return result;
        }

        /// <summary>
        /// Nap lai luot dieu tri theo ma — dung khi man hinh khong giu san doi tuong HIS_TREATMENT.
        /// </summary>
        private static HIS_TREATMENT GetTreatment(long treatmentId)
        {
            HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                List<HIS_TREATMENT> treatments = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>(URI_HIS_TREATMENT_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                result = (treatments != null && treatments.Count > 0) ? treatments[0] : null;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error("PT53438.GetTreatment____Loi nap luot dieu tri.", ex);
            }
            return result;
        }

        /// <summary>
        /// Lay benh an theo ma benh an cua luot dieu tri. Uu tien du lieu da co san de khong goi API lap.
        /// </summary>
        private static HIS_MEDI_RECORD GetMediRecord(long mediRecordId, ref HIS_MEDI_RECORD mediRecordCache)
        {
            try
            {
                if (mediRecordCache != null && mediRecordCache.ID == mediRecordId)
                {
                    return mediRecordCache;
                }

                CommonParam param = new CommonParam();
                HisMediRecordFilter filter = new HisMediRecordFilter();
                filter.ID = mediRecordId;
                List<HIS_MEDI_RECORD> mediRecords = new Inventec.Common.Adapter.BackendAdapter(param)
                    .Get<List<HIS_MEDI_RECORD>>(URI_HIS_MEDI_RECORD_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                mediRecordCache = (mediRecords != null && mediRecords.Count > 0) ? mediRecords[0] : null;
            }
            catch (Exception ex)
            {
                mediRecordCache = null;
                Inventec.Common.Logging.LogSystem.Error("PT53438.GetMediRecord____Loi lay benh an.", ex);
            }
            return mediRecordCache;
        }

    }
}
