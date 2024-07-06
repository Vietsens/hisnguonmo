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
namespace His.UC.LibraryMessage
{
    public partial class Message
    {
        public enum Enum
        {
            HeThongTBKQXLYCCuaFrontendThanhCong,
            HeThongTBKQXLYCCuaFrontendThatBai,
            HeThongTBXuatHienExceptionChuaKiemDuocSoat,
            TaiKhoanKhongCoQuyenThucHienChucNang,
            NguoiDungChuaNhapTaiKhoanDeDangNhap,
            NguoiDungChuaNhapMatKhauDeDangNhap,
            NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac,
            TieuDeCuaSoThongBaoLaThongBao,
            TieuDeCuaSoThongBaoLaCanhBao,
            TieuDeCuaSoThongBaoLaLoi,
            PhanMemKhongKetNoiDuocToiMayChuHeThong,
            HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong,
            NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap,
            HeThongThongBaoMoTaChoWaitDialogForm,
            HeThongThongBaoTieuDeChoWaitDialogForm,
            NguoiDungNhapTuoiBenhNhanKhongHopLe,
            NguoiDungNhapThangSinhLonHonHienTai,
            NguoiDungNhapNgaySinhLonHonHienTai,
            NguoiDungNhapNamSinhKhongHopLe,
            NguoiDungNhapThangSinhKhongHopLe,
            NguoiDungNhapNgaySinhKhongHopLe,
            NguoiDungNhapThangBatDauCoHieuLucTheBHYTKhongHopLe,
            NguoiDungNhapNgayBatDauCoHieuLucTheBHYTKhongHopLe,
            NguoiDungNhapNamHetHieuLucTheBHYTKhongHopLe,
            NguoiDungNhapThangHetHieuLucTheBHYTKhongHopLe,
            NguoiDungNhapNgayHetHieuLucTheBHYTKhongHopLe,
            NguoiDungNhapInPhieuDangKyYeuCauDichVuKhamKhongCoDuLieuDangKyKham,
            NguoiDungNhapSoPhieuDeTaoPhieuThuChiKhongHopLe,
            NguoiDungNhapPhieuTuDeTaoPhieuThuChiKhongHopLe,
            HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong,
            HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong,
            HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
            HeThongTBCuaSoThongBaoBanCoMuonDuyetKhoaDuLieuKhong,
            HeThongTBCuaSoThongBaoBanCoMuonBoDuyetKhoaDuLieuKhong,
            HeThongTBCuaSoThongBaoBanCoMuonQuyetToanDuLieuKhong,
            NguoiDungNhapNamKhongHopLe,
            NguoiDungNhapThangKhongHopLe,
            NguoiDungNhapNgayKhongHopLe,
            NguoiDungNhapTruongHopBHYTKhongChiTraChiPhiVuiLongXemLaiCacDuLieuLienQuan,
            NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong,
            TieuDeThongTinHienThiPhanTrang,
            HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai,
            HeThongTBKetNoiDenMayChuTot,
            HeThongTBKetNoiDenMayChuKhongTot,
            HeThongTBKetNoiDenMayChuKhongOnDinh,
            HeThongTBKetNoiDenMayChuThatBai,
            HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong,
            HeThongTBTruongDuLieuBatBuocPhaiNhap,
            HeThongTBDuLieuNhapVaoKhongHopLe,
            NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh,
            ChucNangDangPhatTrienVuiLongThuLaiSau,
            ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho,
            ChiDinhThuoc_KhongCoDoiTuongThanhToan,
            ChiDinhDichVu_KhongCoDoiTuongThanhToan,
            ChiDinhDichVuKham_ChuaCoYeuCauChiDinhDichVu,
            ChiDinhDichVuKham_ChuaChonPhongXuLy,
            HuyKetThucYeuCau_KhongDungNguoiTraKetQua,
            HuyKetThucYeuCau_HoSoDieuTriDaKetThuc,
            XoaYeuCauChiDinhDichVu_KhongDungNguoiYeuCau,
            XoaYeuCauChiDinhDichVu_KhongDungPhongYeuCau,
            XoaYeuCauChiDinhDichVu_HoSoDieuTriDangDong,

            NguoiDungNhapSoTheBHYTKhongHopLe,
            NguoiDungNhapHanTheTuPhaiNhoHonHanTheDen,
            NguoiDungNhapHanTheBhytDaHetHanSuDung,

            NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec,
            NguoiDungNhapDuLieuKhongHopLe,
            TruongDuLieuBatBuoc,
            HeThongTBBanQuyenKhongHopLe,
            ThieuTruongDuLieuBatBuoc,
            HeThongTBCuaSoThongBaoBanChuaLuuLaiDuLieu,
            XuatTraThuoc_ChuaChonThuocVatTuDeTra,
            HeThongThongBaoThoiGianDenBeHonThoiGianTu,
            NguoiDungNhapNgayPhaiNhoHonNgayHienTai,
            DungLuongFileDinhKemQuaLon,

            ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc,
            ExecuteRoom_DichVuDaCoDichVuDinhKemKhongChoPhepHuyBatDau,
            HeThongTBCuaSoThongBaoBanCoMuonGuiLaiYeuCau,
            NguoiDungNhapSoLonThuocKeDuoiMucCanhBaoTonKho,
            His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop,
            His_UCHein__TheBHYTChuaDenHanSuDung,
            His_UCHein__TheBHYTDaHetHanSuDung,
            His_UCHein__SoTheBHYTNamTrongDanhSachDenVuiLongKiemTraLai
        }
    }
}
