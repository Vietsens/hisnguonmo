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
        private string GetMessage(Enum enumBC)
        {
            string message = "";
            if (Language == LanguageEnum.Vietnamese)
            {
                switch (enumBC)
                {
                    case Enum.HeThongTBKQXLYCCuaFrontendThanhCong: message = MessageViResource.HeThongTBKQXLYCCuaFrontendThanhCong; break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThatBai: message = MessageViResource.HeThongTBKQXLYCCuaFrontendThatBai; break;
                    case Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat: message = MessageViResource.HeThongTBXuatHienExceptionChuaKiemDuocSoat; break;
                    case Enum.TaiKhoanKhongCoQuyenThucHienChucNang: message = MessageViResource.TaiKhoanKhongCoQuyenThucHienChucNang; break;
                    case Enum.NguoiDungChuaNhapTaiKhoanDeDangNhap: message = MessageViResource.NguoiDungChuaNhapTaiKhoanDeDangNhap; break;
                    case Enum.NguoiDungChuaNhapMatKhauDeDangNhap: message = MessageViResource.NguoiDungChuaNhapMatKhauDeDangNhap; break;
                    case Enum.TieuDeCuaSoThongBaoLaThongBao: message = MessageViResource.TieuDeCuaSoThongBaoLaThongBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaCanhBao: message = MessageViResource.TieuDeCuaSoThongBaoLaCanhBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaLoi: message = MessageViResource.TieuDeCuaSoThongBaoLaLoi; break;
                    case Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong: message = MessageViResource.PhanMemKhongKetNoiDuocToiMayChuHeThong; break;
                    case Enum.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong: message = MessageViResource.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong; break;
                    case Enum.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap: message = MessageViResource.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap; break;
                    case Enum.HeThongThongBaoMoTaChoWaitDialogForm: message = MessageViResource.HeThongThongBaoMoTaChoWaitDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogForm: message = MessageViResource.HeThongThongBaoTieuDeChoWaitDialogForm; break;
                    case Enum.NguoiDungNhapTuoiBenhNhanKhongHopLe: message = MessageViResource.NguoiDungNhapTuoiBenhNhanKhongHopLe; break;
                    case Enum.NguoiDungNhapThangSinhLonHonHienTai: message = MessageViResource.NguoiDungNhapThangSinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNgaySinhLonHonHienTai: message = MessageViResource.NguoiDungNhapNgaySinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNamSinhKhongHopLe: message = MessageViResource.NguoiDungNhapNamSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapThangSinhKhongHopLe: message = MessageViResource.NguoiDungNhapThangSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapNgaySinhKhongHopLe: message = MessageViResource.NguoiDungNhapNgaySinhKhongHopLe; break;
                    case Enum.NguoiDungNhapThangBatDauCoHieuLucTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapThangBatDauCoHieuLucTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayBatDauCoHieuLucTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapNgayBatDauCoHieuLucTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapNamHetHieuLucTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapNamHetHieuLucTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapThangHetHieuLucTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapThangHetHieuLucTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayHetHieuLucTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapNgayHetHieuLucTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapInPhieuDangKyYeuCauDichVuKhamKhongCoDuLieuDangKyKham: message = MessageViResource.NguoiDungNhapInPhieuDangKyYeuCauDichVuKhamKhongCoDuLieuDangKyKham; break;
                    case Enum.NguoiDungNhapPhieuTuDeTaoPhieuThuChiKhongHopLe: message = MessageViResource.NguoiDungNhapPhieuTuDeTaoPhieuThuChiKhongHopLe; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonDuyetKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonDuyetKhoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonBoDuyetKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonBoDuyetKhoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNamKhongHopLe: message = MessageViResource.NguoiDungNhapNamKhongHopLe; break;
                    case Enum.NguoiDungNhapThangKhongHopLe: message = MessageViResource.NguoiDungNhapThangKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayKhongHopLe: message = MessageViResource.NguoiDungNhapNgayKhongHopLe; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonQuyetToanDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonQuyetToanDuLieuKhong; break;
                    case Enum.NguoiDungNhapTruongHopBHYTKhongChiTraChiPhiVuiLongXemLaiCacDuLieuLienQuan: message = MessageViResource.NguoiDungNhapTruongHopBHYTKhongChiTraChiPhiVuiLongXemLaiCacDuLieuLienQuan; break;
                    case Enum.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong: message = MessageViResource.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong; break;
                    case Enum.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac: message = MessageViResource.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac; break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageViResource.TieuDeThongTinHienThiPhanTrang; break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageViResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai; break;
                    case Enum.HeThongTBKetNoiDenMayChuTot: message = MessageViResource.HeThongTBKetNoiDenMayChuTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongTot: message = MessageViResource.HeThongTBKetNoiDenMayChuKhongTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongOnDinh: message = MessageViResource.HeThongTBKetNoiDenMayChuKhongOnDinh; break;
                    case Enum.HeThongTBKetNoiDenMayChuThatBai: message = MessageViResource.HeThongTBKetNoiDenMayChuThatBai; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong; break;
                    case Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap: message = MessageViResource.HeThongTBTruongDuLieuBatBuocPhaiNhap; break;
                    case Enum.HeThongTBDuLieuNhapVaoKhongHopLe: message = MessageViResource.HeThongTBDuLieuNhapVaoKhongHopLe; break;
                    case Enum.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh: message = MessageViResource.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh; break;
                    case Enum.ChucNangDangPhatTrienVuiLongThuLaiSau: message = MessageViResource.ChucNangDangPhatTrienVuiLongThuLaiSau; break;
                    case Enum.ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho: message = MessageViResource.ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho; break;
                    case Enum.ChiDinhThuoc_KhongCoDoiTuongThanhToan: message = MessageViResource.ChiDinhThuoc_KhongCoDoiTuongThanhToan; break;
                    case Enum.ChiDinhDichVuKham_ChuaCoYeuCauChiDinhDichVu: message = MessageViResource.ChiDinhDichVuKham_ChuaCoYeuCauChiDinhDichVu; break;
                    case Enum.ChiDinhDichVuKham_ChuaChonPhongXuLy: message = MessageViResource.ChiDinhDichVuKham_ChuaChonPhongXuLy; break;
                    case Enum.XuatTraThuoc_ChuaChonThuocVatTuDeTra: message = MessageViResource.XuatTraThuoc_ChuaChonThuocVatTuDeTra; break;
                    case Enum.HuyKetThucYeuCau_KhongDungNguoiTraKetQua: message = MessageViResource.HuyKetThucYeuCau_KhongDungNguoiTraKetQua; break;
                    case Enum.HuyKetThucYeuCau_HoSoDieuTriDaKetThuc: message = MessageViResource.HuyKetThucYeuCau_HoSoDieuTriDaKetThuc; break;
                    case Enum.XoaYeuCauChiDinhDichVu_KhongDungNguoiYeuCau: message = MessageViResource.XoaYeuCauChiDinhDichVu_KhongDungNguoiYeuCau; break;
                    case Enum.XoaYeuCauChiDinhDichVu_KhongDungPhongYeuCau: message = MessageViResource.XoaYeuCauChiDinhDichVu_KhongDungPhongYeuCau; break;
                    case Enum.XoaYeuCauChiDinhDichVu_HoSoDieuTriDangDong: message = MessageViResource.XoaYeuCauChiDinhDichVu_HoSoDieuTriDangDong; break;
                    case Enum.ChiDinhDichVu_KhongCoDoiTuongThanhToan: message = MessageViResource.ChiDinhDichVu_KhongCoDoiTuongThanhToan; break;


                    case Enum.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec: message = MessageViResource.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec; break;

                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageViResource.HeThongTBBanQuyenKhongHopLe; break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageViResource.ThieuTruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapDuLieuKhongHopLe: message = MessageViResource.NguoiDungNhapDuLieuKhongHopLe; break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageViResource.TruongDuLieuBatBuoc; break;
                    case Enum.HeThongTBCuaSoThongBaoBanChuaLuuLaiDuLieu: message = MessageViResource.HeThongTBCuaSoThongBaoBanChuaLuuLaiDuLieu; break;
                    case Enum.HeThongThongBaoThoiGianDenBeHonThoiGianTu: message = MessageViResource.HeThongTBCuaSoThongBaoBanChuaLuuLaiDuLieu; break;
                    case Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai: message = MessageViResource.NguoiDungNhapNgayPhaiNhoHonNgayHienTai; break;
                    case Enum.DungLuongFileDinhKemQuaLon: message = MessageViResource.DungLuongFileDinhKemQuaLon; break;

                    case Enum.ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc: message = MessageViResource.ChamSoc_DuLieuChiTietKhongTheCungLoaiChamSoc; break;
                    case Enum.ExecuteRoom_DichVuDaCoDichVuDinhKemKhongChoPhepHuyBatDau: message = MessageViResource.ExecuteRoom_DichVuDaCoDichVuDinhKemKhongChoPhepHuyBatDau; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonGuiLaiYeuCau: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonGuiLaiYeuCau; break;
                    case Enum.NguoiDungNhapSoLonThuocKeDuoiMucCanhBaoTonKho: message = MessageViResource.NguoiDungNhapSoLonThuocKeDuoiMucCanhBaoTonKho; break;
                    case Enum.NguoiDungNhapSoTheBHYTKhongHopLe: message = MessageViResource.NguoiDungNhapSoTheBHYTKhongHopLe; break;
                    case Enum.NguoiDungNhapHanTheTuPhaiNhoHonHanTheDen: message = MessageViResource.NguoiDungNhapHanTheTuPhaiNhoHonHanTheDen; break;
                    case Enum.NguoiDungNhapHanTheBhytDaHetHanSuDung: message = MessageViResource.NguoiDungNhapHanTheBhytDaHetHanSuDung; break;
                    case Enum.His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop: message = MessageViResource.His_UCHein__MaDKKCBBDKhacVoiCuaVienNguoiDungPhaiChonTruongHop; break;
                    case Enum.His_UCHein__TheBHYTChuaDenHanSuDung: message = MessageViResource.His_UCHein__TheBHYTChuaDenHanSuDung; break;
                    case Enum.His_UCHein__TheBHYTDaHetHanSuDung: message = MessageViResource.His_UCHein__TheBHYTDaHetHanSuDung; break;
                    case Enum.His_UCHein__SoTheBHYTNamTrongDanhSachDenVuiLongKiemTraLai: message = MessageViResource.His_UCHein__SoTheBHYTNamTrongDanhSachDenVuiLongKiemTraLai; break;

                    default: message = defaultViMessage; break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    default: message = defaultEnMessage; break;
                }
            }
            return message;
        }
    }
}
