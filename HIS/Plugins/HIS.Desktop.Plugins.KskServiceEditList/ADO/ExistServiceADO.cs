namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// 1 dòng grid "Dịch vụ hiện có": gộp theo SERVICE_ID trên toàn bộ hồ sơ đã chọn
    /// </summary>
    public class ExistServiceADO
    {
        public long SERVICE_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        /// <summary>Số hồ sơ có dịch vụ</summary>
        public int PatientCount { get; set; }
        /// <summary>"x/N"</summary>
        public string PatientCountDisplay { get; set; }
        /// <summary>Số hồ sơ đã thực hiện dịch vụ (không xóa/đổi phòng được)</summary>
        public int ExecutedCount { get; set; }
        /// <summary>Tên phòng nếu duy nhất, ngược lại "Nhiều phòng"</summary>
        public string CurrentRoomDisplay { get; set; }
        /// <summary>Danh sách phòng đầy đủ (tooltip)</summary>
        public string CurrentRoomTooltip { get; set; }
        public bool IsDelete { get; set; }
        public long? NewRoomId { get; set; }
    }
}
