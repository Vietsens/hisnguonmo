/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.UC.PatientPackagePicker.ADO
{
    /// <summary>
    /// Ket qua tra ra cua popup chon dich vu trong goi.
    /// Wrap V_HIS_PATIENT_PACKAGE_DT + so luong su dung lan nay
    /// + tham chieu goi cha (HIS_PATIENT_PACKAGE) de caller ghi
    /// PATIENT_PACKAGE_ID vao HIS_SERE_SERV.
    /// </summary>
    public class SelectedPatientPackageServiceADO
    {
        public HIS_PATIENT_PACKAGE PatientPackage { get; set; }
        public V_HIS_PATIENT_PACKAGE_DT PatientPackageDetail { get; set; }

        /// <summary>So luong su dung lan nay (mac dinh = 1 tren UI).</summary>
        public decimal AmountThisTime { get; set; }
    }
}
