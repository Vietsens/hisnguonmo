namespace HIS.Desktop.Plugins.KskServiceEditList.ADO
{
    /// <summary>
    /// Phòng thực hiện được dịch vụ (từ V_HIS_SERVICE_ROOM)
    /// </summary>
    public class RoomADO
    {
        /// <summary>= ROOM_ID</summary>
        public long ID { get; set; }
        public string ROOM_CODE { get; set; }
        public string ROOM_NAME { get; set; }
    }
}
