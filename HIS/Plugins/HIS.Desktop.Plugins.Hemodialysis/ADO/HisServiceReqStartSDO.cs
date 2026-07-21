/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.Hemodialysis.ADO
{
    /// <summary>
    /// DTO gửi lên API /api/HisServiceReq/Start (backend nâng cấp theo vCong42464 nhận object,
    /// không còn nhận id vô hướng). Tên property phải trùng với backend để deserialize đúng.
    /// SECRETARY_* để trống vì chạy thận không có thư ký.
    /// </summary>
    public class HisServiceReqStartSDO
    {
        public long ID { get; set; }
        public string SECRETARY_LOGINNAME { get; set; }
        public string SECRETARY_USERNAME { get; set; }
    }
}
