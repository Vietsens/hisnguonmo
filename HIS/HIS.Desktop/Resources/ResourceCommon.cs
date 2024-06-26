using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Resources
{
    internal class ResourceCommon
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanVuiLongThoatPhanMemRaVaoLaiDeCapNhat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanVuiLongThoatPhanMemRaVaoLaiDeCapNhat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BanCoChacChanMuonThoatCuaSoDangLamViec
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanCoChacChanMuonThoatCuaSoDangLamViec", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TruyCapVaoPhanMemThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Message.TruyCapVaoPhanMemThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiKhoanChuaDuocCauHinhTaiKhoanPhongCuaChiNhanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TaiKhoanChuaDuocCauHinhTaiKhoanPhongCuaChiNhanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string VuiLongDoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Message.VuiLongDoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhienBanHienTaiDangChay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Message.PhienBanHienTaiDangChay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DangKiemTraPhienBanCapNhat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Message.DangKiemTraPhienBanCapNhat", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThongBaoBanCoMuonDongBoGioMayChu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.ThongBaoBanCoMuonDongBoGioMayChu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Lang__Caption__Vi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Caption.Vi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Lang__Caption__En
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Caption.En", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string Lang__Caption__My
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Caption.My", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string CaptionOtherButton
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.Caption_Other_Button", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBaoBanCoMuonThoatPhanMem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Desktop.Resources.Common.Lang.ThongBaoBanCoMuonThoatPhanMem", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DongTabBenPhai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DongTabBenPhai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DongTabKhac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DongTabKhac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string PhienLamViecConHieuLucDen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("PhienLamViecConHieuLucDen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
