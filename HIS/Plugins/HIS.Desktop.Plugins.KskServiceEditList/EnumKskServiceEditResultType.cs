namespace HIS.Desktop.Plugins.KskServiceEditList
{
    /// <summary>
    /// Kết quả xử lý 1 thao tác — mapping với KskServiceEditResultSDO.ResultType (MOS.SDO.KskServiceEditResultType).
    /// </summary>
    public enum EnumKskServiceEditResultType
    {
        /// <summary>Chưa có kết quả (không dùng khi backend trả về)</summary>
        None = 0,

        /// <summary>Thành công</summary>
        Success = 1,

        /// <summary>Bỏ qua: đã có dịch vụ / không có dịch vụ / đã ở phòng mới</summary>
        Skipped = 2,

        /// <summary>Lỗi: không xử lý, hồ sơ được rollback</summary>
        Error = 3
    }

    /// <summary>
    /// Thao tác trên dịch vụ — mapping với KskServiceEditResultSDO.Action (MOS.SDO.KskServiceEditAction).
    /// </summary>
    public static class KskServiceEditAction
    {
        /// <summary>Thêm dịch vụ</summary>
        public const string ADD = "ADD";

        /// <summary>Xóa dịch vụ</summary>
        public const string DELETE = "DELETE";

        /// <summary>Đổi phòng thực hiện</summary>
        public const string CHANGE_ROOM = "CHANGE_ROOM";
    }
}
