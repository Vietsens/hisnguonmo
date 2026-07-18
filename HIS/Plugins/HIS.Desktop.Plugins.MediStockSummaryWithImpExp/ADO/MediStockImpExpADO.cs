/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Tồn kho nhập xuất tồn - DTO/Filter cho API GetWithImpExp
 */
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.MediStockSummaryWithImpExp.ADO
{
    /// <summary>
    /// Một dòng kết quả trả về từ API GetWithImpExp (Mety/Maty):
    /// tổng nhập, tổng xuất, tồn cuối kỳ theo loại thuốc/vật tư trong 1 kho.
    /// </summary>
    public class MediStockImpExpADO
    {
        public long? MEDI_STOCK_ID { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long? MATERIAL_TYPE_ID { get; set; }
        public decimal? AMOUNT { get; set; }              // Tồn hiện tại của loại tại kho — nguồn số liệu cột kho động (pivot)
        public decimal? TOTAL_IMP_QUANTITY { get; set; }  // Tổng nhập
        public decimal? TOTAL_EXP_QUANTITY { get; set; }  // Tổng xuất
        public decimal? CLOSE_QUANTITY { get; set; }      // Tồn cuối kỳ
    }

    /// <summary>
    /// Filter gửi lên API GetWithImpExp: danh sách kho + khoảng thời gian (yyyyMMddHHmmss).
    /// </summary>
    public class MediStockImpExpFilter
    {
        public List<long> MEDI_STOCK_IDs { get; set; }
        public long? FROM_TIME { get; set; }
        public long? TO_TIME { get; set; }
    }
}
