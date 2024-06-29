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

namespace HIS.UC.UCPatientRaw.Base
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCPatientRaw.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
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

        internal static string DotKhamTruocCuaBenhNhanConNoTienVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_DotKhamTruocCuaBenhNhanConNoTienVienPhi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_ThuocCoThoiSuDungDen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_DotKhamTruocCuaBenhNhanCoThuocChuaUongHet", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string NgaySinhKhongDuocNhoHon7
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_SoTuoiKhongDuocNhoHon7", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_NhapGioSinhKhongDung", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_NhapNgaySinhKhongDungDinhDang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_ThongTinNgaySinhPhaiNhoHonNgayHienTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_ChonPhongThuNganTruocKhiMoTinhNangNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TruongDuLieuBatBuoc_ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TruongDuLieuBatBuoc_XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TruongDuLieuBatBuoc_ChucNangNayChuaDuocHoTroTrongPhienBanNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_MaBenhNhanKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_MaHenKhamKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_MaChuongTrinhKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_SoTheKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaDieuTriKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_MaDieuTriKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_CanhBaoDichVuDaDuocChiDinhTrongKhoangThoiGianCauHinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TenBNVuotQuaMaxLength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_HoDemBNVuotQuaMaxLength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SaiDinhDangGioAge
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_SaiDinhDangGioAge", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SaiDinhDangGioAgeVuotGiaTri23
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_SaiDinhDangGioAgeVuotGiaTri23", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
