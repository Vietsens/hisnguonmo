namespace HIS.Desktop.Plugins.KskSyncList.ADO
{
    /// <summary>
    /// Trạng thái cấu hình đẩy dữ liệu liên thông KSK (lưu local qua ControlState theo key btnSettings).
    /// SyncByt = Liên thông KSK BYT (2062/QĐ-BYT); SyncHssk = Liên thông HSSK (2062/QĐ-BYT);
    /// SyncHoc = Liên thông HOC → TTYTQG (2062/QĐ-BYT); SyncHcc = Liên thông HCC (Health Care Center).
    /// </summary>
    public class KskSyncTargetADO
    {
        public bool SyncByt { get; set; }
        public bool SyncHssk { get; set; }
        public bool SyncHoc { get; set; }
        public bool SyncHcc { get; set; }
    }
}
