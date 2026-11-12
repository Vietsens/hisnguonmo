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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000525.PDO
{
    /// <summary>
    /// Một câu hỏi tiền sử bệnh, tật của bản thân (câu 1–21 của Mẫu 06) — theo danh mục bệnh
    /// "KSK lái xe định kỳ", sắp theo thứ tự câu trên giấy.
    /// </summary>
    public class Mps000525DiseaseADO
    {
        /// <summary>Số thứ tự câu trên giấy (1..21).</summary>
        public int STT { get; set; }
        public string DISEASE_TYPE_NAME { get; set; }
        /// <summary>"1" = Có, "0" = Không, null = chưa trả lời.</summary>
        public string IS_YES_NO { get; set; }
    }

    /// <summary>
    /// PDO Giấy khám sức khỏe định kỳ đối với người hành nghề lái xe ô tô — Mẫu 06, Thông tư 25/2026/TT-BYT
    /// (bảng HIS_KSK_PRO_DRIVER). Khuôn theo Mps000520: thể lực lấy HIS_DHST qua DHST_ID; người kết luận
    /// + ngày kết luận ở HIS_KSK_GENERAL.
    /// </summary>
    public partial class Mps000525PDO : RDOBase
    {
        public HIS_KSK_PRO_DRIVER HisKskProDriver { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        /// <summary>Người kết luận (CONCLUDER_*) + ngày kết luận (CONCLUSION_TIME) của lượt khám.</summary>
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public V_HIS_TREATMENT_4 treatment { get; set; }
        /// <summary>Y lệnh KSK (entity HIS_SERVICE_REQ) — tùy chọn; processor đổ key prefix SREQ_.</summary>
        public HIS_SERVICE_REQ KskServiceReq { get; set; }
        /// <summary>Bệnh nhân (HIS_PATIENT) — tùy chọn; processor đổ key prefix PATIENT_.</summary>
        public HIS_PATIENT KskPatient { get; set; }
        /// <summary>Đối tượng — tên các mục đã chọn (plugin đổi mã KSK_PATIENT_TYPES sang tên).</summary>
        public string KskPatientTypesName { get; set; }
        /// <summary>Nguồn chi trả — tên (plugin đổi mã KSK_PAY_SOURCE sang tên).</summary>
        public string KskPaySourceName { get; set; }
        /// <summary>Tên hạng giấy phép lái xe (LICENSE_CLASS_ID).</summary>
        public string LicenseClassName { get; set; }
        /// <summary>21 câu tiền sử bản thân đã sắp theo thứ tự câu.</summary>
        public List<Mps000525DiseaseADO> Diseases { get; set; }
        /// <summary>Danh mục phân loại sức khỏe — để đổi mã phân loại (ID) sang mức I–V khi đánh dấu ô.</summary>
        public List<HIS_HEALTH_EXAM_RANK> HealthExamRanks { get; set; }
        /// <summary>Tài khoản -> họ tên (bác sĩ từng mục, người thực hiện cận lâm sàng).</summary>
        public Dictionary<string, string> EmployeeNames { get; set; }

        public Mps000525PDO(
            HIS_KSK_PRO_DRIVER HisKskProDriver,
            V_HIS_SERVICE_REQ HisServiceReq,
            HIS_DHST HisDhst,
            HIS_KSK_GENERAL HisKskGeneral,
            V_HIS_TREATMENT_4 treatment
            )
        {
            try
            {
                this.HisKskProDriver = HisKskProDriver;
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
