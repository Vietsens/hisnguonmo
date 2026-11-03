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
    /// 1 bản ghi API /api/v1/birth-certificates-01 (đặc tả KDL-YT-API-POST mục 12.3, 30 trường).
    /// Tên thuộc tính = tên trường của kho (phân biệt hoa thường). Trường kiểu Số khai báo long?.
    /// </summary>
    public class KdlGcs01Data
    {
        public string TEN_CSKBCB { get; set; }
        public string MA_CSKBCB { get; set; }
        public string MA_GCS { get; set; }
        public string LAN_CAP { get; set; }
        public string HOTEN_ME { get; set; }
        public string NGAYSINH_ME { get; set; }
        public string MA_DANTOC_ME { get; set; }
        public long? LOAI_GIAYTO_ME { get; set; }
        public string SO_DDCN_ME { get; set; }
        public string NOI_CU_TRU_ME { get; set; }
        public string MATINH_CU_TRU { get; set; }
        public string MAXA_CU_TRU { get; set; }
        // Đặc tả ghi kiểu Số(10) nhưng gửi chuỗi để giữ số 0 ở đầu mã BHXH
        public string MA_BHXH_ME { get; set; }
        public string MA_THE_ME { get; set; }
        public string NGAY_SINH_CON { get; set; }
        public string NOI_SINH_CON { get; set; }
        public long? SOCON_LANNAY { get; set; }
        public long? GIOI_TINH_CON { get; set; }
        public string CAN_NANG_CON { get; set; }
        public string MA_THE_TAM { get; set; }
        public string TEN_CON { get; set; }
        public string GHI_CHU { get; set; }
        public long? SINHCON_PHAUTHUAT { get; set; }
        public long? SINHCON_DUOI32TUAN { get; set; }
        public long? SINHCON_PHAUTHUAT_DUOI32TUAN { get; set; }
        public string TINH_TRANG_CON { get; set; }
        public string NGAY_CS { get; set; }
        public string THAN_NHAN { get; set; }
        public string NGUOI_DO_DE { get; set; }
        public string THU_TRUONG_DVI { get; set; }
    }
}
