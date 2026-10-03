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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000520.PDO
{
    /// <summary>
    /// PDO Giấy khám sức khỏe tâm thần — Mẫu số 04, Phụ lục XXIV Thông tư 25/2026/TT-BYT (bảng HIS_KSK_MENTAL).
    /// Khuôn theo Mps000516: thể lực lấy HIS_DHST qua DHST_ID; bác sĩ khám + ngày kết luận ở HIS_KSK_GENERAL.
    /// </summary>
    public partial class Mps000520PDO : RDOBase
    {
        public HIS_KSK_MENTAL HisKskMental { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        /// <summary>Bác sĩ khám (CONCLUDER_*) + ngày kết luận (CONCLUSION_TIME) của lượt khám.</summary>
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public V_HIS_TREATMENT_4 treatment { get; set; }
        /// <summary>Y lệnh KSK (entity HIS_SERVICE_REQ) — tùy chọn; processor đổ key prefix SREQ_.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (HIS_PATIENT) — tùy chọn; processor đổ key prefix PATIENT_ + giấy tờ, nhóm máu, nơi ở.</summary>
        public HIS_PATIENT KskPatient { get; set; }
        /// <summary>Mục I.7 Đối tượng — tên các mục đã chọn (plugin đổi mã KSK_PATIENT_TYPES sang tên).</summary>
        public string KskPatientTypesName { get; set; }
        /// <summary>Mục I.8 Nguồn chi trả — tên (plugin đổi mã KSK_PAY_SOURCE sang tên).</summary>
        public string KskPaySourceName { get; set; }

        public Mps000520PDO(
            HIS_KSK_MENTAL HisKskMental,
            V_HIS_SERVICE_REQ HisServiceReq,
            HIS_DHST HisDhst,
            HIS_KSK_GENERAL HisKskGeneral,
            V_HIS_TREATMENT_4 treatment
            )
        {
            try
            {
                this.HisKskMental = HisKskMental;
                this.HisServiceReq = HisServiceReq;
                this.HisDhst = HisDhst;
                this.HisKskGeneral = HisKskGeneral;
                this.treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
