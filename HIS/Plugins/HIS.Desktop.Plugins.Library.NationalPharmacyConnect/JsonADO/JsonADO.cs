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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.NationalPharmacyConnect.JsonADO
{
    public enum ThaoTac
    {
        TaoMoi = 1,
        CapNhat,
        Xoa
    }

    public class TaiKhoan
    {
        [JsonProperty("usr")]
        public string TenDangNhap { get; set; }
        [JsonProperty("pwd")]
        public string MatKhau { get; set; }
    }

    public class DangNhap
    {
        [JsonProperty("code")]
        public string Ma { get; set; }
        [JsonProperty("message")]
        public string TinNhan { get; set; }
        [JsonProperty("data")]
        public ThongTinDangNhap ThongTinDangNhap { get; set; }
    }

    public class ThongTinDangNhap
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class DonThuoc
    {
        [JsonProperty("ma_don_thuoc_co_so_kcb")]
        public string MaDonThuocCSKCB { get; set; }
        [JsonProperty("thong_tin_don_vi")]
        public ThongTinDonVi ThongTinDonVi { get; set; }
        [JsonProperty("thong_tin_benh_nhan")]
        public ThongTinBenhNhan ThongTinBenhNhan { get; set; }
        [JsonProperty("thong_tin_benh")]
        public ThongTinBenh ThongTinBenh { get; set; }
        [JsonProperty("thong_tin_don_thuoc")]
        public List<ThongTinDonThuoc> ThongTinDonThuoc { get; set; }
        [JsonProperty("ngay_ke_don")]
        public string NgayKeDon { get; set; }
        [JsonProperty("nguoi_ke_don")]
        public string TenNguoiKeDon { get; set; }
    }

    public class ThongTinDonVi
    {
        [JsonProperty("ma_co_so_kcb")]
        public string MaCSKCB { get; set; }
        [JsonProperty("ten_co_so_kcb")]
        public string TenCSKCB { get; set; }
    }

    public class ThongTinBenhNhan
    {
        [JsonProperty("ma_benh_nhan")]
        public string MaBenhNhan { get; set; }
        [JsonProperty("ho_ten")]
        public string HoTen { get; set; }
        [JsonProperty("tuoi")]
        public int Tuoi { get; set; }
        [JsonProperty("gioi_tinh")]
        public int GioiTinh { get; set; }
        [JsonProperty("can_nang")]
        public int CanNang { get; set; }
        [JsonProperty("chieu_cao")]
        public int ChieuCao { get; set; }
        [JsonProperty("dia_chi")]
        public string DiaChi { get; set; }
    }

    public class ThongTinBenh
    {
        [JsonProperty("ma_benh")]
        public string MaBenh { get; set; }
        [JsonProperty("ten_benh")]
        public string TenBenh { get; set; }
    }

    public class ThongTinDonThuoc
    {
        [JsonProperty("ma_thuoc")]
        public string MaThuoc { get; set; }
        [JsonProperty("ten_thuoc")]
        public string TenThuoc { get; set; }
        [JsonProperty("don_vi_tinh")]
        public string DonViTinh { get; set; }
        [JsonProperty("ham_luong")]
        public string HamLuong { get; set; }
        [JsonProperty("lieu_dung")]
        public string LieuDung { get; set; }
        [JsonProperty("duong_dung")]
        public string DuongDung { get; set; }
        [JsonProperty("so_dang_ky")]
        public string SoDangKy { get; set; }
        [JsonProperty("so_luong")]
        public int SoLuong { get; set; }
        //public decimal so_luong { get; set; }
    }

    public class DonThuocQuocGia
    {
        [JsonProperty("code")]
        public string Ma { get; set; }
        [JsonProperty("mess")]
        public string TinNhan { get; set; }
        [JsonProperty("ma_don_thuoc_quoc_gia")]
        public string MaDonThuocQuocGia { get; set; }
    }

    public class HoaDon
    {
        [JsonProperty("ma_hoa_don")]
        public string MaHoaDon { get; set; }
        [JsonProperty("ma_co_so")]
        public string MaCoSo { get; set; }
        [JsonProperty("ma_don_thuoc_quoc_gia")]
        public string MaDonThuocQuocGia { get; set; }
        [JsonProperty("ngay_ban")]
        public string NgayBan { get; set; }
        [JsonProperty("ho_ten_nguoi_ban")]
        public string HoTenNguoiBan { get; set; }
        [JsonProperty("ho_ten_khach_hang")]
        public string HoTenKhachHang { get; set; }
        [JsonProperty("hoa_don_chi_tiet")]
        public List<ChiTietHoaDon> ChiTietHoaDon { get; set; }
    }

    public class ChiTietHoaDon
    {
        [JsonProperty("ma_thuoc")]
        public string MaThuoc { get; set; }
        [JsonProperty("ten_thuoc")]
        public string TenThuoc { get; set; }
        [JsonProperty("so_lo")]
        public string SoLo { get; set; }
        [JsonProperty("ngay_san_xuat")]
        public string NgaySanXuat { get; set; }
        [JsonProperty("han_dung")]
        public string HanDung { get; set; }
        [JsonProperty("don_vi_tinh")]
        public string DonViTinh { get; set; }
        [JsonProperty("ham_luong")]
        public string HamLuong { get; set; }
        [JsonProperty("so_luong")]
        public int SoLuong { get; set; }
        //public decimal so_luong { get; set; }
        [JsonProperty("don_gia")]
        public int DonGia { get; set; }
        //public decimal don_gia { get; set; }
        [JsonProperty("thanh_tien")]
        public int ThanhTien { get; set; }
        //public decimal thanh_tien { get; set; }
        [JsonProperty("ty_le_quy_doi")]
        public int TyLeQuyDoi { get; set; }
        [JsonProperty("lieu_dung")]
        public string LieuDung { get; set; }
        [JsonProperty("duong_dung")]
        public string DuongDung { get; set; }
        [JsonProperty("so_dang_ky")]
        public string SoDangKy { get; set; }
    }

    public class HoaDonQuocGia
    {
        [JsonProperty("code")]
        public string Ma { get; set; }
        [JsonProperty("mess")]
        public string TinNhan { get; set; }
        [JsonProperty("ma_hoa_don_quoc_gia")]
        public string MaHoaDonQuocGia { get; set; }
    }

    public class PhieuNhap
    {
        [JsonProperty("ma_phieu")]
        public string MaPhieu { get; set; }
        [JsonProperty("ma_co_so")]
        public string MaCoSo { get; set; }
        [JsonProperty("ngay_nhap")]
        public string NgayNhap { get; set; }
        [JsonProperty("loai_phieu_nhap")]
        public int LoaiPhieuNhap { get; set; }
        [JsonProperty("ghi_chu")]
        public string GhiChu { get; set; }
        [JsonProperty("ten_co_so_cung_cap")]
        public string TenCoSoCungCap { get; set; }
        [JsonProperty("chi_tiet")]
        public List<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
    }

    public class ChiTietPhieuNhap
    {
        [JsonProperty("ma_thuoc")]
        public string MaThuoc { get; set; }
        [JsonProperty("ten_thuoc")]
        public string TenThuoc { get; set; }
        [JsonProperty("so_lo")]
        public string SoLo { get; set; }
        [JsonProperty("ngay_san_xuat")]
        public string NgaySanXuat { get; set; }
        [JsonProperty("han_dung")]
        public string HanDung { get; set; }
        [JsonProperty("so_dklh")]
        public string So_Dklh { get; set; }
        [JsonProperty("so_luong")]
        public int SoLuong { get; set; }
        //public decimal so_luong { get; set; }
        [JsonProperty("don_gia")]
        public int DonGia { get; set; }
        //public decimal don_gia { get; set; }
        [JsonProperty("don_vi_tinh")]
        public string DonViTinh { get; set; }
    }

    public class PhieuNhapQuocGia
    {
        [JsonProperty("code")]
        public string Ma { get; set; }
        [JsonProperty("message")]
        public string TinNhan { get; set; }
        [JsonProperty("ma_phieu_nhap_quoc_gia")]
        public string MaPhieuNhapQuocGia { get; set; }
    }

    public class PhieuXuat
    {
        [JsonProperty("ma_phieu")]
        public string MaPhieu { get; set; }
        [JsonProperty("ma_co_so")]
        public string MaCoSo { get; set; }
        [JsonProperty("ngay_xuat")]
        public string NgayXuat { get; set; }
        [JsonProperty("loai_phieu_xuat")]
        public int LoaiPhieuXuat { get; set; }
        [JsonProperty("ghi_chu")]
        public string GhiChu { get; set; }
        [JsonProperty("ten_co_so_nhan")]
        public string TenCoSoNhan { get; set; }
        [JsonProperty("chi_tiet")]
        public List<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
    }

    public class ChiTietPhieuXuat
    {
        [JsonProperty("ma_thuoc")]
        public string MaThuoc { get; set; }
        [JsonProperty("ten_thuoc")]
        public string TenThuoc { get; set; }
        [JsonProperty("so_lo")]
        public string SoLo { get; set; }
        [JsonProperty("ngay_san_xuat")]
        public string NgaySanXuat { get; set; }
        [JsonProperty("han_dung")]
        public string HanDung { get; set; }
        [JsonProperty("so_dklh")]
        public string So_Dklh { get; set; }
        [JsonProperty("so_luong")]
        public int SoLuong { get; set; }
        //public decimal so_luong { get; set; }
        [JsonProperty("don_gia")]
        public int DonGia { get; set; }
        //public decimal don_gia { get; set; }
        [JsonProperty("don_vi_tinh")]
        public string DonViTinh { get; set; }
    }

    public class PhieuXuatQuocGia
    {
        [JsonProperty("code")]
        public string Ma { get; set; }
        [JsonProperty("message")]
        public string TinNhan { get; set; }
        [JsonProperty("ma_phieu_xuat_quoc_gia")]
        public string MaPhieuXuatQuocGia { get; set; }
    }
}
