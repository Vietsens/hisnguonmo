/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.ExecuteRoom.ADO
{
    /// <summary>
    /// DTO truyền lên API /api/HisServiceReq/Start khi bắt đầu xử lý y lệnh.
    /// Bổ sung SECRETARY_LOGINNAME + SECRETARY_USERNAME theo vCong42464.
    /// </summary>
    public class HisServiceReqStartSecretarySDO
    {
        public long ID { get; set; }
        public string SECRETARY_LOGINNAME { get; set; }
        public string SECRETARY_USERNAME { get; set; }
    }
}
