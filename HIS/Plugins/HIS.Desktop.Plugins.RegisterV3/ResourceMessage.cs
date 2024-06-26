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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterV3
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.RegisterV3.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string GoiSangCongBHXHTraVeMaLoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GoiSangCongBHXHTraVeMaLoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string DichVuDinhKemDichVuChuaCoChinhSachGia
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DichVuDinhKemDichVuChuaCoChinhSachGia", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Title_InDichVuKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_Title_InDichVuKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Title_InTheBenhNhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_Title_InTheBenhNhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TenBNVuotQuaMaxLength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TenBNVuotQuaMaxLength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HoDemBNVuotQuaMaxLength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_HoDemBNVuotQuaMaxLength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheBHYTKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_SoTheBHYTKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheDaDuocSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_SoTheDaDuocSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DotKhamTruocCuaBenhNhanConNoTienVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DotKhamTruocCuaBenhNhanConNoTienVienPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocCoThoiSuDungDen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ThuocCoThoiSuDungDen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DotKhamTruocCuaBenhNhanCoThuocChuaUongHet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DotKhamTruocCuaBenhNhanCoThuocChuaUongHet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBaoKetQuaTimKiemBenhNhanKhiQuetTheDuLieuTraVeNull
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBaoKetQuaTimKiemBenhNhanKhiQuetTheDuLieuTraVeNull", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoTheBhytSapHatHan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_CanhBaoTheBhytSapHatHan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DoiTuongBenhNhanLaBHYTBatBuocPhaiChonLaDungTuyen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DoiTuongBenhNhanLaBHYTBatBuocPhaiChonLaDungTuyen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MaHoNgheoKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_MaHoNgheoKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NgaySinhKhongDuocNhoHon7
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_NgaySinhKhongDuocNhoHon7", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhapGioSinhKhongDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_NhapGioSinhKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhapNgaySinhKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_NhapNgaySinhKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThongTinNgaySinhPhaiNhoHonNgayHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ThongTinNgaySinhPhaiNhoHonNgayHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChonPhongThuNganTruocKhiMoTinhNangNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ChonPhongThuNganTruocKhiMoTinhNangNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheBhytChuaDenHanSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TheBhytChuaDenHanSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheBhytDaHetHanSuDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TheBhytDaHetHanSuDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string QuaGioiHanKiemTraTheQuaCongBHXH
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_QuaGioiHanKiemTraTheQuaCongBHXH", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TheDaDuocSuDungTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TheDaDuocSuDungTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TimDuocMotBNTheoThongTinNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TimDuocMotBNTheoThongTinNhap", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeCuaSoThongBaoLaThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TieuDeCuaSoThongBaoLaCanhBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XuLyThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChucNangNayChuaDuocHoTroTrongPhienBanNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_ChucNangNayChuaDuocHoTroTrongPhienBanNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBKetQuaTraVeCuaServerKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_HeThongTBKetQuaTraVeCuaServerKhongHopLe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonDichVuKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_BanChuaChonDichVuKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MotSoThongTinVungThongTinKhacKhongDungDinhDang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_MotSoThongTinVungThongTinKhacNhapKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DuLieuRong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_DuLieuRong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhNhanKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_MaBenhNhanKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaHenKhamKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_MaHenKhamKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaChuongTrinhKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_MaChuongTrinhKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoTheKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_SoTheKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CanhBaoDichVuDaDuocChiDinhTrongKhoangThoiGianCauHinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_CanhBaoDichVuDaDuocChiDinhTrongKhoangThoiGianCauHinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonNhapThongTinKhuVuc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_BanCoMuonNhapThongTinKhuVuc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VuotQuaLuotKhamBHYTTrongNgay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_Register_VuotQuaLuotKhamBHYTTrongNgay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
    }
}
