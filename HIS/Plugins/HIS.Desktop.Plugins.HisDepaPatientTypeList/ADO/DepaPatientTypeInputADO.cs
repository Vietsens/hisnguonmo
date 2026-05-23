using System.Collections.Generic;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.ADO
{
    /// <summary>
    /// Tham số đầu vào khi mở plugin HisDepaPatientTypeList từ plugin khác.
    /// Đóng gói SERVICE_ID + danh sách HIS_DEPA_PATIENT_TYPE hiện tại + cờ trạng thái.
    /// </summary>
    public class DepaPatientTypeInputADO
    {
        /// <summary>Mã dịch vụ (có thể null khi tạo mới chưa save lần đầu).</summary>
        public long? ServiceId { get; set; }

        /// <summary>Danh sách đã chọn ở phiên trước (form sẽ tích lại theo dữ liệu này).</summary>
        public List<HIS_DEPA_PATIENT_TYPE> DepaPatientTypes { get; set; }

        /// <summary>Đã gọi API lấy dữ liệu cũ từ DB hay chưa — tránh gọi lặp.</summary>
        public bool IsCalledApi { get; set; }

        /// <summary>User đã từng nhấn nút Chọn ít nhất 1 lần hay chưa.</summary>
        public bool IsClickPick { get; set; }

        public DepaPatientTypeInputADO()
        {
            DepaPatientTypes = new List<HIS_DEPA_PATIENT_TYPE>();
        }
    }
}
