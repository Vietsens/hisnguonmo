namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// Dịch vụ trong nhóm dịch vụ KSK (HIS_KSK_SERVICE) cho combo chọn dịch vụ thêm
    /// </summary>
    public class KskServiceADO
    {
        /// <summary>= SERVICE_ID (value member của combo)</summary>
        public long ID { get; set; }
        public long KSK_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long ROOM_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
    }
}
