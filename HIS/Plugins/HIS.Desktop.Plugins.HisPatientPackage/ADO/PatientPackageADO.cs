/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisPatientPackage.ADO
{
    /// <summary>
    /// Mở rộng V_HIS_PATIENT_PACKAGE với các cột hiển thị đã tính trước (pre-compute)
    /// để KHÔNG tính toán nặng trong CustomUnboundColumnData (xem performance.md).
    /// </summary>
    public class PatientPackageADO : V_HIS_PATIENT_PACKAGE
    {
        /// <summary>Số thứ tự hiển thị trên grid (tính theo trang).</summary>
        public int STT { get; set; }

        // TODO: View V_HIS_PATIENT_PACKAGE trong checkout hiện tại CHƯA có PACKAGE_NAME/STATUS_CODE
        // (chỉ có ở bảng nền HIS_PATIENT_PACKAGE). Khai báo tạm ở ADO để compile/test.
        // Khi backend bổ sung 2 cột này vào view -> bỏ 2 property dưới (hoặc thêm 'new').
        /// <summary>Tên gói (tạm khai báo ở ADO khi view chưa có PACKAGE_NAME).</summary>
        public string PACKAGE_NAME { get; set; }

        /// <summary>Mã trạng thái gói (tạm khai báo ở ADO khi view chưa có STATUS_CODE).</summary>
        public string STATUS_CODE { get; set; }

        /// <summary>Tên giới tính (resolve từ HIS_GENDER theo PATIENT_GENDER_ID).</summary>
        public string GenderName { get; set; }

        /// <summary>Ngày sinh hiển thị (dd/MM/yyyy hoặc năm).</summary>
        public string DobDisplay { get; set; }

        /// <summary>Tên trạng thái hiển thị (Đăng ký / Đang sử dụng / Đã khóa).</summary>
        public string StatusName { get; set; }

        /// <summary>Thời gian tạo hiển thị.</summary>
        public string CreateTimeStr { get; set; }

        /// <summary>Thời gian sửa hiển thị.</summary>
        public string ModifyTimeStr { get; set; }
    }
}
