/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 * All rights reserved.
 * MIMS Drug Pregnancy / Drug Lactation — PatientProfile cho bệnh nhân nữ
 * (checklist "Phụ nữ mang thai" / "Phụ nữ cho con bú" — bảng HIS_MIMS_PATIENT_PROFILE).
 */
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>
        /// Bản ghi trạng thái PN mang thai / cho con bú của bệnh nhân (prefetch async khi mở form).
        /// </summary>
        MimsPatientProfileRecord mimsPatientProfileRecord;

        /// <summary>
        /// true = đã nạp xong bản ghi (kể cả kết quả null — bệnh nhân chưa được đánh dấu).
        /// </summary>
        bool isMimsPatientProfileLoaded = false;

        /// <summary>
        /// Prefetch bất đồng bộ trạng thái mang thai / cho con bú của bệnh nhân —
        /// chỉ chạy khi config bật + dùng MIMS + bệnh nhân nữ; không chặn UI mở form.
        /// </summary>
        private void PrefetchMimsPatientProfile()
        {
            try
            {
                if (HisConfigCFG.IsCheckMimsPregnancyLactation != "1"
                    || HisConfigCFG.ConnectDrugInterventionInfo != "2")
                    return;
                if (this.currentTreatmentWithPatientType == null
                    || this.currentTreatmentWithPatientType.TDL_PATIENT_GENDER_ID != IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                {
                    this.isMimsPatientProfileLoaded = true;
                    return;
                }

                long patientId = this.currentTreatmentWithPatientType.PATIENT_ID;
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        this.mimsPatientProfileRecord = MimsPatientProfileWorker.GetByPatientId(patientId);
                        this.isMimsPatientProfileLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Build PatientProfile gửi MIMS. Trả về null (giữ nguyên request MIMS như hiện tại) khi:
        /// config tắt / bệnh nhân nam / bệnh nhân nữ không tick mang thai lẫn cho con bú.
        /// </summary>
        private MimsPatientProfile BuildMimsPatientProfile()
        {
            try
            {
                if (HisConfigCFG.IsCheckMimsPregnancyLactation != "1")
                    return null;
                if (this.currentTreatmentWithPatientType == null
                    || this.currentTreatmentWithPatientType.TDL_PATIENT_GENDER_ID != IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    return null;

                if (!this.isMimsPatientProfileLoaded && this.mimsPatientProfileRecord == null)
                {
                    // Prefetch chưa kịp hoàn thành — lấy đồng bộ 1 lần (API nội bộ, nhanh)
                    this.mimsPatientProfileRecord = MimsPatientProfileWorker.GetByPatientId(this.currentTreatmentWithPatientType.PATIENT_ID);
                    this.isMimsPatientProfileLoaded = true;
                }

                int? ageYear = CalculateAgeYearForMims(this.currentTreatmentWithPatientType.TDL_PATIENT_DOB);
                return MimsPatientProfileWorker.ToRequestProfile(this.mimsPatientProfileRecord, "F", ageYear);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Tính tuổi (năm) từ DOB dạng long yyyyMMddHHmmss cho khối Age của MIMS.
        /// </summary>
        private static int? CalculateAgeYearForMims(long dob)
        {
            try
            {
                var dobDateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                if (dobDateTime == null)
                    return null;
                int age = DateTime.Now.Year - dobDateTime.Value.Year;
                if (DateTime.Now < dobDateTime.Value.AddYears(age)) age--;
                if (age < 0) age = 0;
                return age;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
