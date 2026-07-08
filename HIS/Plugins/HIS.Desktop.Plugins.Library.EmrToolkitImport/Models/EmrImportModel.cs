/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models
{
    /// <summary>
    /// Model dữ liệu gửi tới API MaHoaJson của EMRTOOLKIT — dùng cho mẫu phiếu
    /// "Giấy Chuyển Viện / Giấy Chuyển Tuyến" (IDMauPhieu cấu hình, mặc định 524).
    /// Tên thuộc tính + [JsonProperty] khớp đúng key tài liệu API IMPORT.
    /// Quy ước kiểu: int (mã/loại), DateTime? (ngày), string (còn lại).
    /// </summary>
    public class EmrImportModel
    {
        #region ----- Thông tin hành chính bệnh nhân -----

        /// <summary>Loại ký (0 = mặc định)</summary>
        [JsonProperty("LoaiKy")]
        public int LoaiKy { get; set; }

        /// <summary>Mã y tế</summary>
        [JsonProperty("MaYTe")]
        public string MaYTe { get; set; }

        /// <summary>Mã bệnh nhân</summary>
        [JsonProperty("MaBenhNhan")]
        public string MaBenhNhan { get; set; }

        /// <summary>Họ và tên bệnh nhân</summary>
        [JsonProperty("HoVaTenBenhNhan")]
        public string HoVaTenBenhNhan { get; set; }

        /// <summary>Ngày sinh</summary>
        [JsonProperty("NgaySinh")]
        public System.DateTime? NgaySinh { get; set; }

        /// <summary>Tuổi (chuỗi hiển thị)</summary>
        [JsonProperty("Tuoi")]
        public string Tuoi { get; set; }

        /// <summary>Giới tính (1 = Nam, ...)</summary>
        [JsonProperty("GioiTinh")]
        public int GioiTinh { get; set; }

        /// <summary>Số nhà</summary>
        [JsonProperty("SoNha")]
        public string SoNha { get; set; }

        /// <summary>Thôn/Phố</summary>
        [JsonProperty("ThonPho")]
        public string ThonPho { get; set; }

        /// <summary>Tên phường/xã</summary>
        [JsonProperty("TenPhuongXa")]
        public string TenPhuongXa { get; set; }

        /// <summary>Mã phường/xã</summary>
        [JsonProperty("MaPhuongXa")]
        public string MaPhuongXa { get; set; }

        /// <summary>Tên quận/huyện</summary>
        [JsonProperty("TenQuanHuyen")]
        public string TenQuanHuyen { get; set; }

        /// <summary>Mã quận/huyện</summary>
        [JsonProperty("MaQuanHuyen")]
        public string MaQuanHuyen { get; set; }

        /// <summary>Tên tỉnh/thành</summary>
        [JsonProperty("TenTinhThanh")]
        public string TenTinhThanh { get; set; }

        /// <summary>Mã tỉnh/thành</summary>
        [JsonProperty("MaTinhThanh")]
        public string MaTinhThanh { get; set; }

        /// <summary>Nơi làm việc</summary>
        [JsonProperty("NoiLamViec")]
        public string NoiLamViec { get; set; }

        /// <summary>Dân tộc</summary>
        [JsonProperty("DanToc")]
        public string DanToc { get; set; }

        /// <summary>Ngoại kiều / Quốc tịch</summary>
        [JsonProperty("NgoaiKieu")]
        public string NgoaiKieu { get; set; }

        /// <summary>Nghề nghiệp</summary>
        [JsonProperty("NgheNghiep")]
        public string NgheNghiep { get; set; }

        /// <summary>Địa chỉ đầy đủ</summary>
        [JsonProperty("DiaChi")]
        public string DiaChi { get; set; }

        /// <summary>Số CCCD/CMND</summary>
        [JsonProperty("CCCD")]
        public string CCCD { get; set; }

        #endregion

        #region ----- Thông tin BHYT -----

        /// <summary>Đối tượng (0 = mặc định)</summary>
        [JsonProperty("DoiTuong")]
        public int DoiTuong { get; set; }

        /// <summary>Số thẻ BHYT</summary>
        [JsonProperty("SoTheBHYT")]
        public string SoTheBHYT { get; set; }

        /// <summary>Số thẻ (hiển thị trên phiếu)</summary>
        [JsonProperty("SoThe")]
        public string SoThe { get; set; }

        /// <summary>Ngày hết hạn BHYT</summary>
        [JsonProperty("NgayHetHanBHYT")]
        public System.DateTime? NgayHetHanBHYT { get; set; }

        /// <summary>BHYT giá trị từ ngày</summary>
        [JsonProperty("BatDauBHYT")]
        public System.DateTime? BatDauBHYT { get; set; }

        /// <summary>BHYT giá trị đến ngày</summary>
        [JsonProperty("KetThucBHYT")]
        public System.DateTime? KetThucBHYT { get; set; }

        #endregion

        #region ----- Người nhà / liên lạc -----

        /// <summary>Họ tên + địa chỉ người nhà</summary>
        [JsonProperty("HoTenDiaChiNguoiNha")]
        public string HoTenDiaChiNguoiNha { get; set; }

        /// <summary>Số điện thoại người nhà</summary>
        [JsonProperty("SoDienThoaiNguoiNha")]
        public string SoDienThoaiNguoiNha { get; set; }

        /// <summary>Số điện thoại liên lạc</summary>
        [JsonProperty("SoDienThoaiLienLac")]
        public string SoDienThoaiLienLac { get; set; }

        #endregion

        #region ----- Thông tin điều trị / khoa phòng -----

        /// <summary>Mã khoa làm bệnh án</summary>
        [JsonProperty("MaKhoaLamBenhAn")]
        public string MaKhoaLamBenhAn { get; set; }

        /// <summary>Tên khoa làm bệnh án</summary>
        [JsonProperty("TenKhoaLamBenhAn")]
        public string TenKhoaLamBenhAn { get; set; }

        /// <summary>Buồng</summary>
        [JsonProperty("Buong")]
        public string Buong { get; set; }

        /// <summary>Giường</summary>
        [JsonProperty("Giuong")]
        public string Giuong { get; set; }

        /// <summary>Ngày vào viện</summary>
        [JsonProperty("NgayVaoVien")]
        public System.DateTime? NgayVaoVien { get; set; }

        /// <summary>Ngày ra viện</summary>
        [JsonProperty("NgayRaVien")]
        public System.DateTime? NgayRaVien { get; set; }

        /// <summary>Số nhập viện</summary>
        [JsonProperty("SoNhapVien")]
        public string SoNhapVien { get; set; }

        /// <summary>Mã cơ sở khám chữa bệnh</summary>
        [JsonProperty("MaCoSoKhamChuaBenh")]
        public string MaCoSoKhamChuaBenh { get; set; }

        /// <summary>Mã liên kết (Ma_LK)</summary>
        [JsonProperty("Ma_LK")]
        public string Ma_LK { get; set; }

        /// <summary>Mã liên kết (MA_LK)</summary>
        [JsonProperty("MA_LK")]
        public string MA_LK { get; set; }

        /// <summary>Đã được điều trị / khám tại</summary>
        [JsonProperty("DaDieuTriTai")]
        public string DaDieuTriTai { get; set; }

        /// <summary>Ngày bắt đầu điều trị (Từ ngày)</summary>
        [JsonProperty("NgayBDDieuTri")]
        public System.DateTime? NgayBDDieuTri { get; set; }

        /// <summary>Ngày kết thúc điều trị (Đến ngày)</summary>
        [JsonProperty("NgayKTDieuTri")]
        public System.DateTime? NgayKTDieuTri { get; set; }

        /// <summary>Số lưu trữ</summary>
        [JsonProperty("SoLuuTru")]
        public string SoLuuTru { get; set; }

        /// <summary>Kính gửi (cơ sở KCB tiếp nhận)</summary>
        [JsonProperty("KinhGui")]
        public string KinhGui { get; set; }

        /// <summary>Danh sách chuỗi ký số</summary>
        [JsonProperty("DanhSachChuoiKy")]
        public List<object> DanhSachChuoiKy { get; set; }

        /// <summary>ID (0 = tạo mới)</summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>Họ tên bệnh nhân (hiển thị)</summary>
        [JsonProperty("HoTenBN")]
        public string HoTenBN { get; set; }

        #endregion

        #region ----- Tóm tắt bệnh án -----

        /// <summary>Dấu hiệu lâm sàng</summary>
        [JsonProperty("DauHieuLamSan")]
        public string DauHieuLamSan { get; set; }

        /// <summary>Tình trạng lúc nhập viện</summary>
        [JsonProperty("TTLucNhapVien")]
        public string TTLucNhapVien { get; set; }

        /// <summary>Tri giác lúc nhập viện</summary>
        [JsonProperty("TriGiacLNV")]
        public string TriGiacLNV { get; set; }

        /// <summary>Mạch lúc nhập viện</summary>
        [JsonProperty("MachLNV")]
        public string MachLNV { get; set; }

        /// <summary>Huyết áp lúc nhập viện</summary>
        [JsonProperty("HuyetApLNV")]
        public string HuyetApLNV { get; set; }

        /// <summary>Nhịp thở lúc nhập viện</summary>
        [JsonProperty("NhipThoLNV")]
        public string NhipThoLNV { get; set; }

        /// <summary>SPO2 lúc nhập viện</summary>
        [JsonProperty("SPO2LNV")]
        public string SPO2LNV { get; set; }

        /// <summary>Các xét nghiệm</summary>
        [JsonProperty("CacXN")]
        public string CacXN { get; set; }

        /// <summary>Chẩn đoán</summary>
        [JsonProperty("ChanDoan")]
        public string ChanDoan { get; set; }

        /// <summary>Chẩn đoán (nội dung)</summary>
        [JsonProperty("ChanDoanND")]
        public string ChanDoanND { get; set; }

        /// <summary>Ngày chẩn đoán</summary>
        [JsonProperty("ChanDoanNgay")]
        public System.DateTime? ChanDoanNgay { get; set; }

        /// <summary>Phương pháp, thủ thuật, kỹ thuật, thuốc đã sử dụng (các thuốc khác)</summary>
        [JsonProperty("ThuocKhac")]
        public string ThuocKhac { get; set; }

        #endregion

        #region ----- Hỗ trợ điều trị (truyền dịch / vận mạch) -----

        /// <summary>Dịch truyền dạ dày (ml)</summary>
        [JsonProperty("DTDGml")]
        public string DTDGml { get; set; }

        /// <summary>Dịch truyền dạ dày (giờ)</summary>
        [JsonProperty("DTDGGio")]
        public string DTDGGio { get; set; }

        /// <summary>Dịch truyền dạ dày (ml/kg)</summary>
        [JsonProperty("DTDGmlkg")]
        public string DTDGmlkg { get; set; }

        /// <summary>Cao phân tử (ml)</summary>
        [JsonProperty("CaoPhanTuml")]
        public string CaoPhanTuml { get; set; }

        /// <summary>Cao phân tử (giờ)</summary>
        [JsonProperty("CaoPhanTuGio")]
        public string CaoPhanTuGio { get; set; }

        /// <summary>Cao phân tử (ml/kg)</summary>
        [JsonProperty("CaoPhanTumlkg")]
        public string CaoPhanTumlkg { get; set; }

        /// <summary>Hỗ trợ hô hấp</summary>
        [JsonProperty("HoTroHoHap")]
        public string HoTroHoHap { get; set; }

        /// <summary>Dobutamine từ</summary>
        [JsonProperty("DobutamineTu")]
        public string DobutamineTu { get; set; }

        /// <summary>Dobutamine đến</summary>
        [JsonProperty("DobutamineDen")]
        public string DobutamineDen { get; set; }

        /// <summary>Dobutamine (giờ)</summary>
        [JsonProperty("DobutamineGio")]
        public string DobutamineGio { get; set; }

        /// <summary>Dobutamine (ngày)</summary>
        [JsonProperty("DobutamineNgay")]
        public System.DateTime? DobutamineNgay { get; set; }

        /// <summary>Adrenaline từ</summary>
        [JsonProperty("AdrenalineTu")]
        public string AdrenalineTu { get; set; }

        /// <summary>Adrenaline đến</summary>
        [JsonProperty("AdrenalineDen")]
        public string AdrenalineDen { get; set; }

        /// <summary>Adrenaline (giờ)</summary>
        [JsonProperty("AdrenalineGio")]
        public string AdrenalineGio { get; set; }

        /// <summary>Adrenaline (ngày)</summary>
        [JsonProperty("AdrenalineNgay")]
        public System.DateTime? AdrenalineNgay { get; set; }

        /// <summary>Milrinone từ</summary>
        [JsonProperty("MilrinoneTu")]
        public string MilrinoneTu { get; set; }

        /// <summary>Milrinone đến</summary>
        [JsonProperty("MilrinoneDen")]
        public string MilrinoneDen { get; set; }

        /// <summary>Milrinone (giờ)</summary>
        [JsonProperty("MilrinoneGio")]
        public string MilrinoneGio { get; set; }

        /// <summary>Milrinone (ngày)</summary>
        [JsonProperty("MilrinoneNgay")]
        public System.DateTime? MilrinoneNgay { get; set; }

        /// <summary>IVIG lần 1 (giờ)</summary>
        [JsonProperty("IVIG1Gio")]
        public string IVIG1Gio { get; set; }

        /// <summary>IVIG lần 1 (ngày)</summary>
        [JsonProperty("IVIG1Ngay")]
        public System.DateTime? IVIG1Ngay { get; set; }

        /// <summary>IVIG lần 2 (giờ)</summary>
        [JsonProperty("IVIG2Gio")]
        public string IVIG2Gio { get; set; }

        /// <summary>IVIG lần 2 (ngày)</summary>
        [JsonProperty("IVIG2Ngay")]
        public System.DateTime? IVIG2Ngay { get; set; }

        /// <summary>Truyền tĩnh mạch (giờ)</summary>
        [JsonProperty("TTMGio")]
        public string TTMGio { get; set; }

        /// <summary>Truyền tĩnh mạch (phút)</summary>
        [JsonProperty("TTMPhut")]
        public string TTMPhut { get; set; }

        /// <summary>Truyền tĩnh mạch (ngày)</summary>
        [JsonProperty("TTMNgay")]
        public System.DateTime? TTMNgay { get; set; }

        /// <summary>Truyền tĩnh mạch (tổng)</summary>
        [JsonProperty("TTMTong")]
        public string TTMTong { get; set; }

        #endregion

        #region ----- Tình trạng lúc chuyển viện -----

        /// <summary>Mạch lúc chuyển viện</summary>
        [JsonProperty("MachTCV")]
        public string MachTCV { get; set; }

        /// <summary>Huyết áp (cmHg) lúc chuyển viện</summary>
        [JsonProperty("HuyetApcmHgTCV")]
        public string HuyetApcmHgTCV { get; set; }

        /// <summary>Nhịp thở lúc chuyển viện</summary>
        [JsonProperty("NhipThoTCV")]
        public string NhipThoTCV { get; set; }

        /// <summary>SPO2 lúc chuyển viện</summary>
        [JsonProperty("SPO2TCV")]
        public string SPO2TCV { get; set; }

        /// <summary>Dịch trong chuyển viện</summary>
        [JsonProperty("DichTrongCV")]
        public string DichTrongCV { get; set; }

        /// <summary>Dịch còn lại</summary>
        [JsonProperty("DichConLai")]
        public string DichConLai { get; set; }

        /// <summary>Tốc độ dịch trong chuyển viện</summary>
        [JsonProperty("TocDoDichTrongCV")]
        public string TocDoDichTrongCV { get; set; }

        /// <summary>Vận mạch</summary>
        [JsonProperty("VanMach")]
        public string VanMach { get; set; }

        /// <summary>Lý do chuyển viện / chuyển tuyến</summary>
        [JsonProperty("LiDoCV")]
        public string LiDoCV { get; set; }

        /// <summary>Số giấy chuyển viện</summary>
        [JsonProperty("SoGiayCV")]
        public string SoGiayCV { get; set; }

        /// <summary>Chuyển viện hồi (giờ)</summary>
        [JsonProperty("CVGio")]
        public string CVGio { get; set; }

        /// <summary>Chuyển viện hồi (phút)</summary>
        [JsonProperty("CVPhut")]
        public string CVPhut { get; set; }

        /// <summary>Ngày chuyển viện</summary>
        [JsonProperty("CVNgay")]
        public System.DateTime? CVNgay { get; set; }

        /// <summary>Phương tiện vận chuyển</summary>
        [JsonProperty("PTVanChuyen")]
        public string PTVanChuyen { get; set; }

        /// <summary>Họ tên người đưa đi (người hộ tống)</summary>
        [JsonProperty("HoTenNDD")]
        public string HoTenNDD { get; set; }

        /// <summary>Ngày đưa đi</summary>
        [JsonProperty("NgayDuaDi")]
        public System.DateTime? NgayDuaDi { get; set; }

        #endregion

        #region ----- Người ký / mẫu phiếu -----

        /// <summary>Họ tên bác sĩ điều trị</summary>
        [JsonProperty("HTBacSyDieuTri")]
        public string HTBacSyDieuTri { get; set; }

        /// <summary>Họ tên giám đốc bệnh viện / người có thẩm quyền chuyển tuyến</summary>
        [JsonProperty("HTGiamDocBV")]
        public string HTGiamDocBV { get; set; }

        /// <summary>ID mẫu phiếu (Giấy Chuyển Viện)</summary>
        [JsonProperty("IDMauPhieu")]
        public int IDMauPhieu { get; set; }

        /// <summary>Tên mẫu phiếu</summary>
        [JsonProperty("TenMauPhieu")]
        public string TenMauPhieu { get; set; }

        /// <summary>Mã quản lý</summary>
        [JsonProperty("MaQuanLy")]
        public int MaQuanLy { get; set; }

        #endregion
    }
}
