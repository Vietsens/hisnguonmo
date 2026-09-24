namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// 1 dòng grid "Dịch vụ thêm"
    /// </summary>
    public class AddServiceADO
    {
        public long KSK_ID { get; set; }
        public string KSK_NAME { get; set; }
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long ROOM_ID { get; set; }
        public string ROOM_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        /// <summary>Đơn giá theo nhóm dịch vụ KSK (đã gồm VAT), null = theo chính sách giá</summary>
        public decimal? PRICE { get; set; }
    }
}
