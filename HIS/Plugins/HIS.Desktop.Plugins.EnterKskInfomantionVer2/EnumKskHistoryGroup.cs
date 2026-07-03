/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2
{
    /// <summary>
    /// 5 nhóm tiền sử có gắn cụm chọn mã ICD trong KSK (theo yêu cầu R5).
    /// Mỗi nhóm chỉ lưu MỘT giá trị (mã + tên ICD) dùng chung cho cả lượt khám —
    /// hiển thị nhất quán trên mọi tab có nhóm tương ứng.
    /// (Khi BE bổ sung cột, map sang các cột tương ứng trong HIS_KSK_GENERAL.)
    /// </summary>
    public enum KskHistoryGroup
    {
        /// <summary>Tiền sử gia đình</summary>
        Family = 1,

        /// <summary>Tiền sử bản thân</summary>
        Personal = 2,

        /// <summary>Bệnh nghề nghiệp</summary>
        Occupational = 3,

        /// <summary>Sản khoa bất thường</summary>
        Obstetric = 4,

        /// <summary>Bệnh đang điều trị</summary>
        Treatment = 5
    }
}
