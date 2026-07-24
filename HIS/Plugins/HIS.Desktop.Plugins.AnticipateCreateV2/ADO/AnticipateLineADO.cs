/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong 52461 - Tạo dự trù v2: 1 dòng trong GRID DỰ TRÙ (bên dưới cây).
 * Người dùng nhập SL dự trù trên cây rồi Bổ sung (Ctrl A) → tạo AnticipateLineADO đưa xuống grid;
 * Lưu đọc từ danh sách các dòng này để build HIS_ANTICIPATE + chi tiết METY/MATY/BLTY.
 */
using System;

namespace HIS.Desktop.Plugins.AnticipateCreateV2.ADO
{
    /// <summary>Loại mặt hàng của dòng dự trù (phân nhánh khi build chi tiết + gom nhóm khi in).</summary>
    public static class AnticipateLineType
    {
        public const string THUOC = "THUOC";
        public const string VATTU = "VATTU";
        public const string MAU = "MAU";
    }

    /// <summary>1 dòng dự trù trong grid (đã Bổ sung từ cây).</summary>
    public class AnticipateLineADO
    {
        public long TypeId { get; set; }              // MEDICINE_TYPE_ID / MATERIAL_TYPE_ID / BLOOD_TYPE_ID
        public string Type { get; set; }              // AnticipateLineType.*
        public string Code { get; set; }
        public string Name { get; set; }
        public string ActiveIngrName { get; set; }
        public string Concentra { get; set; }
        public string UnitName { get; set; }
        public string ManufacturerName { get; set; }

        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public long? BidId { get; set; }
        public string BidName { get; set; }

        public decimal? BidAmount { get; set; }
        public decimal? BidRemain { get; set; }
        public decimal? OpenQuantity { get; set; }
        public decimal? NewImport { get; set; }
        public decimal? Used { get; set; }
        public decimal? CloseQuantity { get; set; }
        public decimal? MaxExport { get; set; }
        public int MaxExportMonth { get; set; }

        public decimal? ImpPrice { get; set; }        // Giá nhập (sửa được)
        public decimal? Amount { get; set; }          // SL dự trù (sửa được)
        public string Note { get; set; }              // Ghi chú (sửa được — lưu vào cột NOTE của METY/MATY/BLTY)
    }
}
