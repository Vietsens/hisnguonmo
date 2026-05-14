/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Mã lý do hoàn ứng - dùng khi tra cứu HIS_REPAY_REASON theo REPAY_REASON_CODE.
 * Thêm mới mã "07" - "Nhập lại xuất bán" (việc 42727).
 */
namespace HIS.Desktop.Plugins.HisImportMestMedicine.Base
{
    internal class RepayReasonCode
    {
        // Mã lý do hoàn ứng "Nhập lại xuất bán" (record danh mục mới - việc 42727)
        internal const string NhapLaiXuatBan = "07";
    }
}
