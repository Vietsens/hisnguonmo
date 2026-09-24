namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// 1 dòng grid dịch vụ (tương tự HisSereServADO của "Sửa chỉ định dịch vụ"), gộp trên toàn bộ hồ sơ đã chọn.
    /// IsChecked: true = tất cả BN có dịch vụ; null = một phần BN có (chưa thay đổi); false = không BN nào có.
    /// </summary>
    public class ServiceRowADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }

        /// <summary>Nhóm dịch vụ KSK dùng khi thêm (null: dịch vụ không thuộc nhóm KSK nào của hợp đồng)</summary>
        public long? KSK_ID { get; set; }
        public decimal? AMOUNT { get; set; }
        /// <summary>Đơn giá theo nhóm KSK (đã gồm VAT)</summary>
        public decimal? PRICE { get; set; }

        /// <summary>Số hồ sơ đang có dịch vụ</summary>
        public int PatientCount { get; set; }
        public string PatientCountDisplay { get; set; }
        /// <summary>Số hồ sơ đã thực hiện dịch vụ</summary>
        public int ExecutedCount { get; set; }

        /// <summary>Phòng thực hiện hiện tại nếu duy nhất (null khi nhiều phòng hoặc chưa có)</summary>
        public long? OriginalRoomId { get; set; }
        /// <summary>Tên các phòng hiện tại (hiển thị khi nhiều phòng)</summary>
        public string CurrentRoomDisplay { get; set; }
        /// <summary>Phòng thực hiện (sửa trên grid)</summary>
        public long? RoomId { get; set; }
        public bool IsRoomChanged { get; set; }

        public bool? IsChecked { get; set; }
        /// <summary>Có ít nhất 1 hồ sơ đang có dịch vụ</summary>
        public bool IsExisting { get; set; }
        /// <summary>Chỉ một phần hồ sơ có dịch vụ</summary>
        public bool IsPartial { get; set; }
    }
}
