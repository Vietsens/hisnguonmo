using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCHeniInfo
{
    class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.UC.UCHeniInfo.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string PhaiCoGiayChungNhanKhongCungChiTra
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhaiCoGiayChungNhanKhongCungChiTra", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhaiDatDu5Nam6ThangMoiCoTheChonDTMCCT", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string MaBenhKhongKhopVoiTenBenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhKhongKhopVoiTenBenh", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaBenhChinhKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaBenhChinhKhongHopLe", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocNhapTenBenhVoiTruongHopBenhNhanLaDungTuyenGioiThieu", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiDiemMienCungChiTraPhaiCungNamVoiNamHienTai", languageMessage, LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("SoTheBHYTKhongHopLe", languageMessage, LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("SoTheDaDuocSuDung", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BatBuocPhaiChonTruongHop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BatBuocPhaiChonTruongHop", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungNhapNgayKhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungNhapNgayKhongHopLe", languageMessage, LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("TruongDuLieuBatBuoc", languageMessage, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MaDangKyKCBBDKhacVoiCuaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MaDangKyKCBBDKhacVoiCuaVien", languageMessage, LanguageManager.GetCulture());
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
