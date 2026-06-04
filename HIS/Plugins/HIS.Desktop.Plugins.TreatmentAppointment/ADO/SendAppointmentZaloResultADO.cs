/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.TreatmentAppointment.ADO
{
    /// <summary>
    /// Response body trả về từ API SendAppointmentZalo.
    /// Tổng hợp kết quả gửi tin Zalo cho danh sách điều trị.
    /// </summary>
    public class SendAppointmentZaloResultADO
    {
        /// <summary>Tổng số điều trị gửi yêu cầu</summary>
        public int TotalRequested { get; set; }

        /// <summary>Số điều trị gửi thành công</summary>
        public int TotalSuccess { get; set; }

        /// <summary>Số điều trị gửi thất bại</summary>
        public int TotalFailed { get; set; }

        /// <summary>Chi tiết kết quả từng điều trị</summary>
        public List<SendAppointmentZaloResultItemADO> Details { get; set; }
    }

    /// <summary>
    /// Chi tiết kết quả gửi tin Zalo cho 1 điều trị.
    /// </summary>
    public class SendAppointmentZaloResultItemADO
    {
        /// <summary>ID điều trị</summary>
        public long TreatmentId { get; set; }

        /// <summary>Mã điều trị (hiển thị popup kết quả)</summary>
        public string TreatmentCode { get; set; }

        /// <summary>Tên bệnh nhân</summary>
        public string PatientName { get; set; }

        /// <summary>Số điện thoại gửi tin</summary>
        public string Phone { get; set; }

        /// <summary>Trạng thái gửi (true = thành công)</summary>
        public bool IsSuccess { get; set; }

        /// <summary>Thông báo lỗi nếu thất bại</summary>
        public string ErrorMessage { get; set; }
    }
}
