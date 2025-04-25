using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LibraryMessage;

namespace HIS.UC.UCPatientRaw
{
    public class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCPatientRaw.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string SoTuoiKhongDuocNhoHon7
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_SoTuoiKhongDuocNhoHon7", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TieuDeCuaSoThongBaoLaThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_TieuDeCuaSoThongBaoLaCanhBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_ThieuTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_XuLyThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
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

        public static string typeCodeFind__MaBA
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaBA", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaBA_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaBA_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaBN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaBN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaBN_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaBN_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__SoDT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__SoDT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__SoDT_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__SoDT_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }


        public static string typeCodeFind__MaHK
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaHK", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaHK_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaHK_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaCT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaCT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaCT_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaCT_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__SoThe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__SoThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        public static string typeCodeFind__MaTV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaTuVan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__SoThe_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__SoThe_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaNV
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaNV", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaNV_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaNV_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaDT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaDT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaDT_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaDT_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaCMCC
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaCMCC", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string typeCodeFind__MaCMCC_ToolTip
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_typeCodeFind__MaCMCC_ToolTip", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

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

        internal static string DotKhamTruocCuaBenhNhanConNoTienVienPhi3
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_DotKhamTruocCuaBenhNhanConNoTienVienPhi3", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string MaCmndCccdKhongTontai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_MaCmndCccdKhongTontai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BenhNhanDaTuVong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HIS_UC_UCPatientRaw_BenhNhanDaTuVong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
