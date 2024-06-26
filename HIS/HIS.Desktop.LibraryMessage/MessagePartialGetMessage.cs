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
using System;
namespace HIS.Desktop.LibraryMessage
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
                    case Enum.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ: message = MessageViResource.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ; break;
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
                    case Enum.NguoiDungNhapThangSinhLonHonHienTai: message = MessageViResource.NguoiDungNhapThangSinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNgaySinhLonHonHienTai: message = MessageViResource.NguoiDungNhapNgaySinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNamSinhKhongHopLe: message = MessageViResource.NguoiDungNhapNamSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapThangSinhKhongHopLe: message = MessageViResource.NguoiDungNhapThangSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapNgaySinhKhongHopLe: message = MessageViResource.NguoiDungNhapNgaySinhKhongHopLe; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNamKhongHopLe: message = MessageViResource.NguoiDungNhapNamKhongHopLe; break;
                    case Enum.NguoiDungNhapThangKhongHopLe: message = MessageViResource.NguoiDungNhapThangKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayKhongHopLe: message = MessageViResource.NguoiDungNhapNgayKhongHopLe; break;
                    case Enum.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong: message = MessageViResource.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong; break;
                    case Enum.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac: message = MessageViResource.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac; break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageViResource.TieuDeThongTinHienThiPhanTrang; break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageViResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai; break;
                    case Enum.HeThongTBKetNoiDenMayChuTot: message = MessageViResource.HeThongTBKetNoiDenMayChuTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongTot: message = MessageViResource.HeThongTBKetNoiDenMayChuKhongTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongOnDinh: message = MessageViResource.HeThongTBKetNoiDenMayChuKhongOnDinh; break;
                    case Enum.HeThongTBKetNoiDenMayChuThatBai: message = MessageViResource.HeThongTBKetNoiDenMayChuThatBai; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong: message = MessageViResource.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh: message = MessageViResource.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh; break;
                    case Enum.ChucNangDangPhatTrienVuiLongThuLaiSau: message = MessageViResource.ChucNangDangPhatTrienVuiLongThuLaiSau; break;
                    case Enum.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec: message = MessageViResource.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec; break;
                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageViResource.HeThongTBBanQuyenKhongHopLe; break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageViResource.ThieuTruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapDuLieuKhongHopLe: message = MessageViResource.NguoiDungNhapDuLieuKhongHopLe; break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageViResource.TruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai: message = MessageViResource.NguoiDungNhapNgayPhaiNhoHonNgayHienTai; break;
                    case Enum.DungLuongFileDinhKemQuaLon: message = MessageViResource.DungLuongFileDinhKemQuaLon; break;
                    case Enum.ThongBaoDuLieuTrong: message = MessageViResource.ThongBaoDuLieuTrong; break;
                    case Enum.HeThongThongBaoMoTaChoUpdatingDialogForm: message = MessageViResource.HeThongThongBaoMoTaChoUpdatingDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting: message = MessageViResource.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting; break;
                    case Enum.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram: message = MessageViResource.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay: message = MessageViResource.HeThongTBKhongTimThayPluginsCuaChucNangNay; break;
                    case Enum.DuLieuDangKhoa: message = MessageViResource.DuLieuDangKhoa; break;
                    case Enum.DuLieuDangMo: message = MessageViResource.DuLieuDangMo; break;
                    case Enum.SoLuongKhongDuocBeHonKhong: message = MessageViResource.SoLuongKhongDuocBeHonKhong; break;
                    case Enum.TruongDuLieuKhongNhanGiaTriAm: message = MessageViResource.TruongDuLieuKhongNhanGiaTriAm; break;
                    case Enum.TruongDuLieuLaVAT: message = MessageViResource.TruongDuLieuLaVAT; break;
                    case Enum.Plugins_HisDesktop__KhongKhoiTaoDuocModule: message = MessageViResource.Plugins_HisDesktop__KhongKhoiTaoDuocModule; break;
                    case Enum.ImportExcel__DuLieuDocTuFileExcelRong: message = MessageViResource.DuLieuDocTuFileExcelRong; break;
                    case Enum.Common__SoLuongPhaiLonHonKhong: message = MessageViResource.Common__SoLuongPhaiLonHonKhong; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa: message = MessageViResource.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa: message = MessageViResource.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBTruyCapVaoPhanMemThanhCong: message = MessageViResource.HeThongTBTruyCapVaoPhanMemThanhCong; break;
                    case Enum.AlertHospitalFeeNotBHYT: message = MessageViResource.AlertHospitalFeeNotBHYT; break;
                    case Enum.AlertWarningHeinFee: message = MessageViResource.AlertWarningHeinFee; break;

                    default: message = defaultViMessage; break;
                }
            }
            else if (Language == LanguageEnum.English)
            {
                switch (enumBC)
                {
                    case Enum.HeThongTBKQXLYCCuaFrontendThanhCong: message = MessageEnResource.HeThongTBKQXLYCCuaFrontendThanhCong; break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThatBai: message = MessageEnResource.HeThongTBKQXLYCCuaFrontendThatBai; break;
                    case Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat: message = MessageEnResource.HeThongTBXuatHienExceptionChuaKiemDuocSoat; break;
                    case Enum.TaiKhoanKhongCoQuyenThucHienChucNang: message = MessageEnResource.TaiKhoanKhongCoQuyenThucHienChucNang; break;
                    case Enum.NguoiDungChuaNhapTaiKhoanDeDangNhap: message = MessageEnResource.NguoiDungChuaNhapTaiKhoanDeDangNhap; break;
                    case Enum.NguoiDungChuaNhapMatKhauDeDangNhap: message = MessageEnResource.NguoiDungChuaNhapMatKhauDeDangNhap; break;
                    case Enum.TieuDeCuaSoThongBaoLaThongBao: message = MessageEnResource.TieuDeCuaSoThongBaoLaThongBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaCanhBao: message = MessageEnResource.TieuDeCuaSoThongBaoLaCanhBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaLoi: message = MessageEnResource.TieuDeCuaSoThongBaoLaLoi; break;
                    case Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong: message = MessageEnResource.PhanMemKhongKetNoiDuocToiMayChuHeThong; break;
                    case Enum.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong: message = MessageEnResource.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong; break;
                    case Enum.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap: message = MessageEnResource.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap; break;
                    case Enum.HeThongThongBaoMoTaChoWaitDialogForm: message = MessageEnResource.HeThongThongBaoMoTaChoWaitDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogForm: message = MessageEnResource.HeThongThongBaoTieuDeChoWaitDialogForm; break;
                    case Enum.NguoiDungNhapThangSinhLonHonHienTai: message = MessageEnResource.NguoiDungNhapThangSinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNgaySinhLonHonHienTai: message = MessageEnResource.NguoiDungNhapNgaySinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNamSinhKhongHopLe: message = MessageEnResource.NguoiDungNhapNamSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapThangSinhKhongHopLe: message = MessageEnResource.NguoiDungNhapThangSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapNgaySinhKhongHopLe: message = MessageEnResource.NguoiDungNhapNgaySinhKhongHopLe; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong: message = MessageEnResource.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong: message = MessageEnResource.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong: message = MessageEnResource.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNamKhongHopLe: message = MessageEnResource.NguoiDungNhapNamKhongHopLe; break;
                    case Enum.NguoiDungNhapThangKhongHopLe: message = MessageEnResource.NguoiDungNhapThangKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayKhongHopLe: message = MessageEnResource.NguoiDungNhapNgayKhongHopLe; break;
                    case Enum.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong: message = MessageEnResource.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong; break;
                    case Enum.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac: message = MessageEnResource.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac; break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageEnResource.TieuDeThongTinHienThiPhanTrang; break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageEnResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai; break;
                    case Enum.HeThongTBKetNoiDenMayChuTot: message = MessageEnResource.HeThongTBKetNoiDenMayChuTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongTot: message = MessageEnResource.HeThongTBKetNoiDenMayChuKhongTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongOnDinh: message = MessageEnResource.HeThongTBKetNoiDenMayChuKhongOnDinh; break;
                    case Enum.HeThongTBKetNoiDenMayChuThatBai: message = MessageEnResource.HeThongTBKetNoiDenMayChuThatBai; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong: message = MessageEnResource.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh: message = MessageEnResource.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh; break;
                    case Enum.ChucNangDangPhatTrienVuiLongThuLaiSau: message = MessageEnResource.ChucNangDangPhatTrienVuiLongThuLaiSau; break;
                    case Enum.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec: message = MessageEnResource.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec; break;
                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageEnResource.HeThongTBBanQuyenKhongHopLe; break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageEnResource.ThieuTruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapDuLieuKhongHopLe: message = MessageEnResource.NguoiDungNhapDuLieuKhongHopLe; break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageEnResource.TruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai: message = MessageEnResource.NguoiDungNhapNgayPhaiNhoHonNgayHienTai; break;
                    case Enum.DungLuongFileDinhKemQuaLon: message = MessageEnResource.DungLuongFileDinhKemQuaLon; break;
                    case Enum.ThongBaoDuLieuTrong: message = MessageEnResource.ThongBaoDuLieuTrong; break;
                    case Enum.HeThongThongBaoMoTaChoUpdatingDialogForm: message = MessageEnResource.HeThongThongBaoMoTaChoUpdatingDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting: message = MessageEnResource.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting; break;
                    case Enum.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram: message = MessageEnResource.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay: message = MessageEnResource.HeThongTBKhongTimThayPluginsCuaChucNangNay; break;
                    case Enum.DuLieuDangKhoa: message = MessageEnResource.DuLieuDangKhoa; break;
                    case Enum.DuLieuDangMo: message = MessageEnResource.DuLieuDangMo; break;
                    case Enum.SoLuongKhongDuocBeHonKhong: message = MessageEnResource.SoLuongKhongDuocBeHonKhong; break;
                    case Enum.TruongDuLieuKhongNhanGiaTriAm: message = MessageEnResource.TruongDuLieuKhongNhanGiaTriAm; break;
                    case Enum.TruongDuLieuLaVAT: message = MessageEnResource.TruongDuLieuLaVAT; break;
                    case Enum.Plugins_HisDesktop__KhongKhoiTaoDuocModule: message = MessageEnResource.Plugins_HisDesktop__KhongKhoiTaoDuocModule; break;
                    case Enum.Common__SoLuongPhaiLonHonKhong: message = MessageEnResource.Common__SoLuongPhaiLonHonKhong; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa: message = MessageEnResource.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa: message = MessageEnResource.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBTruyCapVaoPhanMemThanhCong: message = MessageEnResource.HeThongTBTruyCapVaoPhanMemThanhCong; break;
                    case Enum.AlertWarningHeinFee: message = MessageEnResource.AlertWarningHeinFee; break;

                    default: message = defaultEnMessage; break;
                }
            }
            else if (Language == LanguageEnum.Mianmar)
            {
                switch (enumBC)
                {
                    case Enum.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ: message = MessageMyResource.SoTienDaKeXChoBHYTDaVuotMucGioiHanYLaZ; break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThanhCong: message = MessageMyResource.HeThongTBKQXLYCCuaFrontendThanhCong; break;
                    case Enum.HeThongTBKQXLYCCuaFrontendThatBai: message = MessageMyResource.HeThongTBKQXLYCCuaFrontendThatBai; break;
                    case Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat: message = MessageMyResource.HeThongTBXuatHienExceptionChuaKiemDuocSoat; break;
                    case Enum.TaiKhoanKhongCoQuyenThucHienChucNang: message = MessageMyResource.TaiKhoanKhongCoQuyenThucHienChucNang; break;
                    case Enum.NguoiDungChuaNhapTaiKhoanDeDangNhap: message = MessageMyResource.NguoiDungChuaNhapTaiKhoanDeDangNhap; break;
                    case Enum.NguoiDungChuaNhapMatKhauDeDangNhap: message = MessageMyResource.NguoiDungChuaNhapMatKhauDeDangNhap; break;
                    case Enum.TieuDeCuaSoThongBaoLaThongBao: message = MessageMyResource.TieuDeCuaSoThongBaoLaThongBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaCanhBao: message = MessageMyResource.TieuDeCuaSoThongBaoLaCanhBao; break;
                    case Enum.TieuDeCuaSoThongBaoLaLoi: message = MessageMyResource.TieuDeCuaSoThongBaoLaLoi; break;
                    case Enum.PhanMemKhongKetNoiDuocToiMayChuHeThong: message = MessageMyResource.PhanMemKhongKetNoiDuocToiMayChuHeThong; break;
                    case Enum.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong: message = MessageMyResource.HeThongThongBaoTienDoHoanThanhTaiCauHinhHeThong; break;
                    case Enum.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap: message = MessageMyResource.NguoiDungNhapTaiKhoanHoacMatKhauKhongChinhXacDeDangNhap; break;
                    case Enum.HeThongThongBaoMoTaChoWaitDialogForm: message = MessageMyResource.HeThongThongBaoMoTaChoWaitDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogForm: message = MessageMyResource.HeThongThongBaoTieuDeChoWaitDialogForm; break;
                    case Enum.NguoiDungNhapThangSinhLonHonHienTai: message = MessageMyResource.NguoiDungNhapThangSinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNgaySinhLonHonHienTai: message = MessageMyResource.NguoiDungNhapNgaySinhLonHonHienTai; break;
                    case Enum.NguoiDungNhapNamSinhKhongHopLe: message = MessageMyResource.NguoiDungNhapNamSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapThangSinhKhongHopLe: message = MessageMyResource.NguoiDungNhapThangSinhKhongHopLe; break;
                    case Enum.NguoiDungNhapNgaySinhKhongHopLe: message = MessageMyResource.NguoiDungNhapNgaySinhKhongHopLe; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong: message = MessageMyResource.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong: message = MessageMyResource.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong: message = MessageMyResource.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNamKhongHopLe: message = MessageMyResource.NguoiDungNhapNamKhongHopLe; break;
                    case Enum.NguoiDungNhapThangKhongHopLe: message = MessageMyResource.NguoiDungNhapThangKhongHopLe; break;
                    case Enum.NguoiDungNhapNgayKhongHopLe: message = MessageMyResource.NguoiDungNhapNgayKhongHopLe; break;
                    case Enum.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong: message = MessageMyResource.NguoiDungNhapNguoiDungKhongDuocGanQuyenVaoPhong; break;
                    case Enum.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac: message = MessageMyResource.NguoiDungDoiMatKhauMatKhauXacNhanKhongChinhXac; break;
                    case Enum.TieuDeThongTinHienThiPhanTrang: message = MessageMyResource.TieuDeThongTinHienThiPhanTrang; break;
                    case Enum.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai: message = MessageMyResource.HeThongTBNguoiDungDaHetPhienLamViecVuiLongDangNhapLai; break;
                    case Enum.HeThongTBKetNoiDenMayChuTot: message = MessageMyResource.HeThongTBKetNoiDenMayChuTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongTot: message = MessageMyResource.HeThongTBKetNoiDenMayChuKhongTot; break;
                    case Enum.HeThongTBKetNoiDenMayChuKhongOnDinh: message = MessageMyResource.HeThongTBKetNoiDenMayChuKhongOnDinh; break;
                    case Enum.HeThongTBKetNoiDenMayChuThatBai: message = MessageMyResource.HeThongTBKetNoiDenMayChuThatBai; break;
                    case Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong: message = MessageMyResource.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong; break;
                    case Enum.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh: message = MessageMyResource.NguoiDungNhapNguoiDungDuocCauHinhVaoPhongKhongDuocGanQuyenVuiLongKiemTraLaiCauHinh; break;
                    case Enum.ChucNangDangPhatTrienVuiLongThuLaiSau: message = MessageMyResource.ChucNangDangPhatTrienVuiLongThuLaiSau; break;
                    case Enum.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec: message = MessageMyResource.NguoiDungNhapNguoiDungChuaDuocCauHinhVaoPhongLamViec; break;
                    case Enum.HeThongTBBanQuyenKhongHopLe: message = MessageMyResource.HeThongTBBanQuyenKhongHopLe; break;
                    case Enum.ThieuTruongDuLieuBatBuoc: message = MessageMyResource.ThieuTruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapDuLieuKhongHopLe: message = MessageMyResource.NguoiDungNhapDuLieuKhongHopLe; break;
                    case Enum.TruongDuLieuBatBuoc: message = MessageMyResource.TruongDuLieuBatBuoc; break;
                    case Enum.NguoiDungNhapNgayPhaiNhoHonNgayHienTai: message = MessageMyResource.NguoiDungNhapNgayPhaiNhoHonNgayHienTai; break;
                    case Enum.DungLuongFileDinhKemQuaLon: message = MessageMyResource.DungLuongFileDinhKemQuaLon; break;
                    case Enum.ThongBaoDuLieuTrong: message = MessageMyResource.ThongBaoDuLieuTrong; break;
                    case Enum.HeThongThongBaoMoTaChoUpdatingDialogForm: message = MessageMyResource.HeThongThongBaoMoTaChoUpdatingDialogForm; break;
                    case Enum.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting: message = MessageMyResource.HeThongThongBaoTieuDeChoWaitDialogFormIsPleaseWaiting; break;
                    case Enum.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram: message = MessageMyResource.ThongBaoTaiTruocCacDuLieuCauHinhVeMayTram; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay: message = MessageMyResource.HeThongTBKhongTimThayPluginsCuaChucNangNay; break;
                    case Enum.DuLieuDangKhoa: message = MessageMyResource.DuLieuDangKhoa; break;
                    case Enum.DuLieuDangMo: message = MessageMyResource.DuLieuDangMo; break;
                    case Enum.SoLuongKhongDuocBeHonKhong: message = MessageMyResource.SoLuongKhongDuocBeHonKhong; break;
                    case Enum.TruongDuLieuKhongNhanGiaTriAm: message = MessageMyResource.TruongDuLieuKhongNhanGiaTriAm; break;
                    case Enum.TruongDuLieuLaVAT: message = MessageMyResource.TruongDuLieuLaVAT; break;
                    case Enum.Plugins_HisDesktop__KhongKhoiTaoDuocModule: message = MessageMyResource.Plugins_HisDesktop__KhongKhoiTaoDuocModule; break;
                    case Enum.ImportExcel__DuLieuDocTuFileExcelRong: message = MessageMyResource.DuLieuDocTuFileExcelRong; break;
                    case Enum.Common__SoLuongPhaiLonHonKhong: message = MessageMyResource.Common__SoLuongPhaiLonHonKhong; break;
                    case Enum.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa: message = MessageMyResource.HeThongTBKhongTimThayPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa: message = MessageMyResource.HeThongTBKhongKhoiTaoDuocPluginsCuaChucNangNayVoiMa; break;
                    case Enum.HeThongTBTruyCapVaoPhanMemThanhCong: message = MessageMyResource.HeThongTBTruyCapVaoPhanMemThanhCong; break;
                    case Enum.AlertHospitalFeeNotBHYT: message = MessageMyResource.AlertHospitalFeeNotBHYT; break;
                    case Enum.AlertWarningHeinFee: message = MessageMyResource.AlertWarningHeinFee; break;

                    default: message = defaultViMessage; break;
                }
            }
            return message;
        }
    }
}
