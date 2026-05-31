/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Mã trạng thái HIỂN THỊ của gói bệnh nhân theo bảng màn 6.2 (4 trạng thái).
    /// Lưu ý: bảng nền HIS_PATIENT_PACKAGE.STATUS_CODE (theo §3.1) chỉ có 3 mã gốc
    /// (REGISTERED/IN_USE/LOCKED); 4 trạng thái hiển thị được suy ra thêm từ tiền hoàn/hủy.
    /// Việc map mã gốc -> mã hiển thị nằm ở MapDisplayStatus() (UcHisPatientPackage___Grid.cs) — cần backend chốt.
    /// </summary>
    internal static class PatientPackageStatusCode
    {
        /// <summary>Chờ thanh toán — gói mới đăng ký, chưa thu tiền.</summary>
        internal const string WAITING_PAYMENT = "WAITING_PAYMENT";

        /// <summary>Đã thanh toán — đã thu tiền, đang sử dụng.</summary>
        internal const string PAID = "PAID";

        /// <summary>Đã hoàn tiền — đã hoàn ứng.</summary>
        internal const string REFUNDED = "REFUNDED";

        /// <summary>Đã hủy — gói bị hủy/khóa.</summary>
        internal const string CANCELED = "CANCELED";

        // === RAW STATUS_CODE trong HIS_PATIENT_PACKAGE (theo §3.1) — 3 mã gốc ===
        /// <summary>Mã raw trong DB: đã đăng ký, chưa thu tiền.</summary>
        internal const string RAW_REGISTERED = "REGISTERED";
        /// <summary>Mã raw trong DB: đang sử dụng (đã thanh toán).</summary>
        internal const string RAW_IN_USE = "IN_USE";
        /// <summary>Mã raw trong DB: đã khóa/hủy.</summary>
        internal const string RAW_LOCKED = "LOCKED";

        /// <summary>
        /// Map mã HIỂN THỊ (UI) -> mã RAW (DB). DÙNG khi gửi entity về backend (Lock, Update, ...)
        /// để tránh backend nhận giá trị "WAITING_PAYMENT"/"PAID"/... (không khớp WHERE clause).
        /// </summary>
        internal static string ToRaw(string displayCode)
        {
            if (string.IsNullOrEmpty(displayCode)) return displayCode;
            switch (displayCode)
            {
                case WAITING_PAYMENT: return RAW_REGISTERED;
                case PAID:            return RAW_IN_USE;
                case REFUNDED:        return RAW_IN_USE;  // REFUNDED là trạng thái suy luận từ TOTAL_REFUNDED; nền vẫn IN_USE
                case CANCELED:        return RAW_LOCKED;
                default: return displayCode;  // nếu đã là raw thì giữ nguyên
            }
        }
    }

    /// <summary>Các hành động dòng trong grid Danh sách gói — quyết định ẩn/hiện nút theo trạng thái.</summary>
    internal enum PatientPackageRowAction
    {
        /// <summary>Sửa gói (mở màn 6.1).</summary>
        Edit = 1,
        /// <summary>Xóa gói (kèm chi tiết).</summary>
        Delete = 2,
        /// <summary>In phiếu thông tin gói.</summary>
        Print = 3,
        /// <summary>Thanh toán (mở Thanh toán khác).</summary>
        Pay = 4,
        /// <summary>Hoàn tiền (mở Hoàn ứng dịch vụ).</summary>
        Refund = 5,
        /// <summary>Khóa/Mở khóa gói.</summary>
        Lock = 6
    }
}
