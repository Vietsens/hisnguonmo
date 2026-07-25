/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System;
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000519.PDO
{
    /// <summary>
    /// PDO Hồ sơ quản lý sức khỏe cá nhân theo QĐ 831.
    /// Đầu vào là các model object hiện có:
    ///   - HIS_KSK_PROFILE (hồ sơ: hành chính, tiền sử lúc sinh/nguy cơ/sản khoa, lý do khám...)
    ///   - HIS_KSK_GENERAL (khám lâm sàng + CLS + kết luận)
    ///   - HIS_SERVICE_REQ (y lệnh) + HIS_PATIENT (bệnh nhân)
    ///   - HIS_DHST (dấu hiệu sinh tồn)
    ///   - HIS_DISEASE_TYPE / HIS_DISEASE_DETAIL / HIS_DISEASE_DETAIL_RESULT (checklist tiền sử: bệnh tật/dị ứng/khuyết tật/gia đình)
    /// </summary>
    public partial class Mps000519PDO : RDOBase
    {
        public HIS_KSK_PROFILE HisKskProfile { get; set; }
        public HIS_KSK_GENERAL HisKskGeneral { get; set; }
        public HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_PATIENT HisPatient { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public List<HIS_DISEASE_TYPE> DiseaseTypes { get; set; }
        public List<HIS_DISEASE_DETAIL> DiseaseDetails { get; set; }
        public List<HIS_DISEASE_DETAIL_RESULT> DiseaseDetailResults { get; set; }
        /// <summary>Danh mục loại vắc xin (mục C) — HIS_VACCINE_TYPE (TYPE_VACCINE 1/2/3).</summary>
        public List<HIS_VACCINE_TYPE> VaccineTypes { get; set; }
        /// <summary>Dữ liệu tiêm chủng đã lưu của bệnh nhân — HIS_HEALTH_VACCINATION.</summary>
        public List<HIS_HEALTH_VACCINATION> HealthVaccinations { get; set; }
        /// <summary>Tùy chọn — dùng cho barcode/avatar (ADO Treatment).</summary>
        public V_HIS_TREATMENT_4 treatment { get; set; }

        public Mps000519PDO(
            HIS_KSK_PROFILE HisKskProfile,
            HIS_KSK_GENERAL HisKskGeneral,
            HIS_SERVICE_REQ HisServiceReq,
            HIS_PATIENT HisPatient,
            HIS_DHST HisDhst,
            List<HIS_DISEASE_TYPE> DiseaseTypes,
            List<HIS_DISEASE_DETAIL> DiseaseDetails,
            List<HIS_DISEASE_DETAIL_RESULT> DiseaseDetailResults,
            List<HIS_VACCINE_TYPE> VaccineTypes,
            List<HIS_HEALTH_VACCINATION> HealthVaccinations,
            V_HIS_TREATMENT_4 treatment)
        {
            try
            {
                this.HisKskProfile = HisKskProfile;
                this.HisKskGeneral = HisKskGeneral;
                this.HisServiceReq = HisServiceReq;
                this.HisPatient = HisPatient;
                this.HisDhst = HisDhst;
                this.DiseaseTypes = DiseaseTypes;
                this.DiseaseDetails = DiseaseDetails;
                this.DiseaseDetailResults = DiseaseDetailResults;
                this.VaccineTypes = VaccineTypes;
                this.HealthVaccinations = HealthVaccinations;
                this.treatment = treatment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
