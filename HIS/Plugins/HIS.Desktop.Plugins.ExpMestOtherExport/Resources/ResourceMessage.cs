using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestOtherExport.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ExpMestOtherExport.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TonTaiThuocVatTuKhongCoTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__TonTaiThuocVatTuKhongCoTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongPhaiLonHonKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__SoLuongPhaiLonHonKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongXuatLonHonSoLuongKhadungTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__SoLuongXuatLonHonSoLuongKhadungTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayThe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayThe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DuLieuDocTuExcelRong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__DuLieuDocTuExcelRong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__KhongCoMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongCoLoai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__KhongCoLoai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoLuongKhongChinhXac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__SoLuongKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string MaKhongChinhXac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__MaKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string LoaiKhongChinhXac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__LoaiKhongChinhXac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThuocVatTuMauKhongCoTrongKho
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__ThuocVatTuMauKhongCoTrongKho", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongTimThayMauIn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__KhongTimThayMauIn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string XuLyThanhCongBanMuonMoFileKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__XuLyThanhCongBanMuonMoFileKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExpMestOtherExport__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
