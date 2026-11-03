/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */

namespace HIS.Desktop.Plugins.Library.KhoDuLieuYTe.Model
{
    /// <summary>
    /// 1 bản ghi API /api/v1/death-certificates (đặc tả KDL-YT-API-POST mục 14.3, 35 trường theo CV 3617).
    /// Tên thuộc tính = tên trường của kho (phân biệt hoa thường). Trường kiểu Số khai báo long?.
    /// </summary>
    public class KdlGbtData
    {
        public string MA_GBT { get; set; }
        public string MA_BN { get; set; }
        public string MA_HSBA { get; set; }
        public string HO_TEN { get; set; }
        public string NGAY_SINH { get; set; }
        public long? GIOI_TINH { get; set; }
        public string MA_THE { get; set; }
        public string MA_DANTOC { get; set; }
        public string MA_QUOCTICH { get; set; }
        public string DCHI_THUONGTRU { get; set; }
        public string MATINH_THUONGTRU { get; set; }
        public string MAHUYEN_THUONGTRU { get; set; }
        public string MAXA_THUONGTRU { get; set; }
        public string DCHI_HIENTAI { get; set; }
        public string MATINH_HIENTAI { get; set; }
        public string MAHUYEN_HIENTAI { get; set; }
        public string MAXA_HIENTAI { get; set; }
        public long? LOAI_GIAYTO { get; set; }
        public string SO_GIAYTO { get; set; }
        public string NGAY_CAP { get; set; }
        public string NOI_CAP { get; set; }
        public string NGAYGIO_VV { get; set; }
        public string NGAY_TV { get; set; }
        public long? TINH_TRANG_TV { get; set; }
        public string NGUYENNHAN_TV { get; set; }
        public string NGUOI_GHIGIAY { get; set; }
        public string NGUOI_THANTHICH { get; set; }
        public string TTRUONG_DVI { get; set; }
        public string SO_BAOTU { get; set; }
        public string QUYEN_SO { get; set; }
        public string NGAY_CAPGIAYBT { get; set; }
        public string SO_BAOTU_BD { get; set; }
        public string QUYEN_SO_BD { get; set; }
        public string MACSKCB { get; set; }
        public string DIACHI_CSKCB { get; set; }
    }
}
