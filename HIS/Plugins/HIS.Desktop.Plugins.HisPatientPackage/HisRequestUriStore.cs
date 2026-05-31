/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Tập trung URI API cho module Gói dịch vụ bệnh nhân.
    /// Pattern: api/HisPatientPackage/{Action}. View danh sách: GetView (V_HIS_PATIENT_PACKAGE).
    /// </summary>
    internal class HisRequestUriStore
    {
        /// <summary>Lấy danh sách gói (view) — trả V_HIS_PATIENT_PACKAGE, có paging.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_GET_VIEW = "api/HisPatientPackage/GetView";

        /// <summary>Lấy chi tiết gói (view) theo PATIENT_PACKAGE_ID — V_HIS_PATIENT_PACKAGE_DT.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_DT_GET_VIEW = "api/HisPatientPackageDt/GetView";

        /// <summary>Xóa gói (kèm chi tiết). Backend chặn nếu gói đã thanh toán mà chưa hoàn hết.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_DELETE = "api/HisPatientPackage/Delete";

        /// <summary>Đổi trạng thái khóa/mở khóa gói — truyền nguyên entity HIS_PATIENT_PACKAGE.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_CHANGE_LOCK = "api/HisPatientPackage/ChangeLock";

        /// <summary>Lấy bản ghi gói (HIS_PATIENT_PACKAGE) theo ID — dùng khi cần data MỚI NHẤT để in.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_GET = "api/HisPatientPackage/Get";

        /// <summary>Lấy chi tiết gói (HIS_PATIENT_PACKAGE_DT) theo PATIENT_PACKAGE_ID — dùng khi in.</summary>
        internal const string MOSHIS_HIS_PATIENT_PACKAGE_DT_GET = "api/HisPatientPackageDt/Get";

        /// <summary>Lấy bệnh nhân (HIS_PATIENT) theo ID — dùng để in phiếu gói.</summary>
        internal const string MOSHIS_HIS_PATIENT_GET = "api/HisPatient/Get";
    }
}
