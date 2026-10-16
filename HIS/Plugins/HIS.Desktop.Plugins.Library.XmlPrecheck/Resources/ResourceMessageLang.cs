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

namespace HIS.Desktop.Plugins.Library.XmlPrecheck.Resources
{
    /// <summary>
    /// Chuoi hien thi cua thu vien kiem tra loi XML - dung chung cho man Xuat XML 130 va man Ket thuc dieu tri.
    /// Danh sach thuoc tinh sinh tu cac khoa trong Message.Lang.vi.resx; them khoa moi thi them vao ca ba resx roi them thuoc tinh o day.
    /// </summary>
    public class ResourceMessageLang
    {
        public static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.Library.XmlPrecheck.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        /// <summary>Doc chuoi theo ngon ngu dang dung. Loi thi tra rong, khong lam vo luong nghiep vu.</summary>
        public static string Get(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, languageMessage,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        public static string ThongBao { get { return Get("ThongBao"); } }
        public static string MdInsightNutKiemTraLoi { get { return Get("MdInsightNutKiemTraLoi"); } }
        public static string MdInsightNutLayKetQua { get { return Get("MdInsightNutLayKetQua"); } }
        public static string MdInsightNutHuy { get { return Get("MdInsightNutHuy"); } }
        public static string MdInsightTieuDeTienTrinh { get { return Get("MdInsightTieuDeTienTrinh"); } }
        public static string MdInsightDangGui { get { return Get("MdInsightDangGui"); } }
        public static string MdInsightDangLayKetQua { get { return Get("MdInsightDangLayKetQua"); } }
        public static string MdInsightChuaChonHoSo { get { return Get("MdInsightChuaChonHoSo"); } }
        public static string MdInsightXacNhanLoLon { get { return Get("MdInsightXacNhanLoLon"); } }
        public static string MdInsightXacNhanLoLonVuotHan { get { return Get("MdInsightXacNhanLoLonVuotHan"); } }
        public static string MdInsightHeNgoaiKhongPhanHoi { get { return Get("MdInsightHeNgoaiKhongPhanHoi"); } }
        public static string MdInsightKhongCoCoSoHopLe { get { return Get("MdInsightKhongCoCoSoHopLe"); } }
        public static string MdInsightTrangThaiKhongLoi { get { return Get("MdInsightTrangThaiKhongLoi"); } }
        public static string MdInsightTrangThaiCanhBao { get { return Get("MdInsightTrangThaiCanhBao"); } }
        public static string MdInsightTrangThaiLoiNghiemTrong { get { return Get("MdInsightTrangThaiLoiNghiemTrong"); } }
        public static string MdInsightTrangThaiChuaCoKetQua { get { return Get("MdInsightTrangThaiChuaCoKetQua"); } }
        public static string MdInsightTrangThaiKhongKiemTraDuoc { get { return Get("MdInsightTrangThaiKhongKiemTraDuoc"); } }
        public static string MdInsightLyDoChuaKhaiTaiKhoan { get { return Get("MdInsightLyDoChuaKhaiTaiKhoan"); } }
        public static string MdInsightLyDoKhongKetXuatDuoc { get { return Get("MdInsightLyDoKhongKetXuatDuoc"); } }
        public static string MdInsightLyDoGuiThatBai { get { return Get("MdInsightLyDoGuiThatBai"); } }
        public static string MdInsightLyDoSaiXacThuc { get { return Get("MdInsightLyDoSaiXacThuc"); } }
        public static string MdInsightLyDoQuaHanGoi { get { return Get("MdInsightLyDoQuaHanGoi"); } }
        public static string MdInsightLyDoHeNgoaiLoi { get { return Get("MdInsightLyDoHeNgoaiLoi"); } }
        public static string MdInsightLyDoChoKetQua { get { return Get("MdInsightLyDoChoKetQua"); } }
        public static string MdInsightLyDoChuaDuThoiGian { get { return Get("MdInsightLyDoChuaDuThoiGian"); } }
        public static string MdInsightLyDoQuaHanChoKetQua { get { return Get("MdInsightLyDoQuaHanChoKetQua"); } }
        public static string MdInsightConLoiChuaLuu { get { return Get("MdInsightConLoiChuaLuu"); } }
        public static string MdInsightTongKetKetQua { get { return Get("MdInsightTongKetKetQua"); } }
        public static string MdInsightCanhBaoDuLieuBenhNhan { get { return Get("MdInsightCanhBaoDuLieuBenhNhan"); } }
        public static string MdInsightCotKetQua { get { return Get("MdInsightCotKetQua"); } }
        public static string MdInsightCotSoLoi { get { return Get("MdInsightCotSoLoi"); } }
        public static string MdInsightCotThoiDiem { get { return Get("MdInsightCotThoiDiem"); } }
        public static string MdInsightDangCho { get { return Get("MdInsightDangCho"); } }
        public static string MdInsightDangKetXuat { get { return Get("MdInsightDangKetXuat"); } }
        public static string MdInsightKhongCoDienGiai { get { return Get("MdInsightKhongCoDienGiai"); } }
        public static string MdInsightTieuDeKetQua { get { return Get("MdInsightTieuDeKetQua"); } }
        public static string MdInsightNutXuatDanhSach { get { return Get("MdInsightNutXuatDanhSach"); } }
        public static string MdInsightNutDong { get { return Get("MdInsightNutDong"); } }
        public static string MdInsightNhomDanhSachHoSo { get { return Get("MdInsightNhomDanhSachHoSo"); } }
        public static string MdInsightNhomChiTietLoi { get { return Get("MdInsightNhomChiTietLoi"); } }
        public static string MdInsightNhomDienGiai { get { return Get("MdInsightNhomDienGiai"); } }
        public static string MdInsightCotMaDieuTri { get { return Get("MdInsightCotMaDieuTri"); } }
        public static string MdInsightCotTenBenhNhan { get { return Get("MdInsightCotTenBenhNhan"); } }
        public static string MdInsightCotSoLoiNghiemTrong { get { return Get("MdInsightCotSoLoiNghiemTrong"); } }
        public static string MdInsightCotLyDo { get { return Get("MdInsightCotLyDo"); } }
        public static string MdInsightCotMucDo { get { return Get("MdInsightCotMucDo"); } }
        public static string MdInsightCotTepThanhPhan { get { return Get("MdInsightCotTepThanhPhan"); } }
        public static string MdInsightCotTheDuLieu { get { return Get("MdInsightCotTheDuLieu"); } }
        public static string MdInsightCotDongThuMay { get { return Get("MdInsightCotDongThuMay"); } }
        public static string MdInsightCotGiaTriHienTai { get { return Get("MdInsightCotGiaTriHienTai"); } }
        public static string MdInsightCotNoiDungLoi { get { return Get("MdInsightCotNoiDungLoi"); } }
        public static string MdInsightMucDoNghiemTrong { get { return Get("MdInsightMucDoNghiemTrong"); } }
        public static string MdInsightMucDoCanhBao { get { return Get("MdInsightMucDoCanhBao"); } }
        public static string MdInsightXuatKhongCoDuLieu { get { return Get("MdInsightXuatKhongCoDuLieu"); } }
        public static string MdInsightXuatThanhCong { get { return Get("MdInsightXuatThanhCong"); } }
        public static string MdInsightXuatThatBai { get { return Get("MdInsightXuatThatBai"); } }
        public static string MdInsightChuaTungGui { get { return Get("MdInsightChuaTungGui"); } }
        public static string MdInsightLyDoKhaiNhamCoSo { get { return Get("MdInsightLyDoKhaiNhamCoSo"); } }
        public static string MdInsightXacNhanGuiLai { get { return Get("MdInsightXacNhanGuiLai"); } }
        public static string MdInsightLyDoLechCoSo { get { return Get("MdInsightLyDoLechCoSo"); } }
        public static string XmlPrecheckMenuKiemTraSoBo { get { return Get("XmlPrecheckMenuKiemTraSoBo"); } }
        public static string XmlPrecheckMenuLayKetQua { get { return Get("XmlPrecheckMenuLayKetQua"); } }
        public static string XmlPrecheckMenuXemKetQua { get { return Get("XmlPrecheckMenuXemKetQua"); } }
        public static string XmlPrecheckNutMoRongGoiY { get { return Get("XmlPrecheckNutMoRongGoiY"); } }
        public static string XmlPrecheckGoiYLuuTamTruoc { get { return Get("XmlPrecheckGoiYLuuTamTruoc"); } }
        public static string XmlPrecheckDaKhoaVienPhi { get { return Get("XmlPrecheckDaKhoaVienPhi"); } }
        public static string XmlPrecheckTieuDeSoBo { get { return Get("XmlPrecheckTieuDeSoBo"); } }
        public static string XmlPrecheckKhongLoiSoBo { get { return Get("XmlPrecheckKhongLoiSoBo"); } }
        public static string XmlPrecheckDaGuiChuaCoKetQua { get { return Get("XmlPrecheckDaGuiChuaCoKetQua"); } }
        public static string XmlPrecheckChuaGui { get { return Get("XmlPrecheckChuaGui"); } }
        public static string XmlPrecheckLoiCauHinhTaiKhoan { get { return Get("XmlPrecheckLoiCauHinhTaiKhoan"); } }
        public static string XmlPrecheckKetQuaGanNhat { get { return Get("XmlPrecheckKetQuaGanNhat"); } }
        public static string XmlPrecheckKetQuaGanNhatSoLoi { get { return Get("XmlPrecheckKetQuaGanNhatSoLoi"); } }
        public static string XmlPrecheckNhanSoBo { get { return Get("XmlPrecheckNhanSoBo"); } }
        public static string XmlPrecheckNhanCanKiemLai { get { return Get("XmlPrecheckNhanCanKiemLai"); } }
        public static string XmlPrecheckNhanDuLieuDaDoi { get { return Get("XmlPrecheckNhanDuLieuDaDoi"); } }
        public static string XmlPrecheckBoKetXuatChuaCapNhat { get { return Get("XmlPrecheckBoKetXuatChuaCapNhat"); } }
        public static string XmlPrecheckKhongKetXuatDuocThieu { get { return Get("XmlPrecheckKhongKetXuatDuocThieu"); } }
        public static string XmlPrecheckDangChuanBi { get { return Get("XmlPrecheckDangChuanBi"); } }
        public static string XmlPrecheckDangGuiMot { get { return Get("XmlPrecheckDangGuiMot"); } }
        public static string XmlPrecheckXacNhanGuiLai { get { return Get("XmlPrecheckXacNhanGuiLai"); } }
        public static string XmlPrecheckNhacLuuTam { get { return Get("XmlPrecheckNhacLuuTam"); } }
        public static string XmlPrecheckTomTatDong { get { return Get("XmlPrecheckTomTatDong"); } }
        public static string XmlPrecheckKhongCoDichVu { get { return Get("XmlPrecheckKhongCoDichVu"); } }
    }
}
