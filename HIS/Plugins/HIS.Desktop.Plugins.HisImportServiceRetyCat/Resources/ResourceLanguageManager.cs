using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportServiceRetyCat.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisImportServiceRetyCat.Resources.Lang", typeof(HIS.Desktop.Plugins.HisImportServiceRetyCat.FormImportServiceRetyCat).Assembly);

        internal static string HeThongTBKQXLYCCuaFrontendThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBKQXLYCCuaFrontendThatBai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongHopLe", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NhomMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhomMau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Maxlength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Maxlength", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaTonTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaTonTai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaTonTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaTonTai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BiTrung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BiTrung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string CoThiPhaiNhap
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CoThiPhaiNhap", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThieuTruongDL
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuTruongDuLieuBatBuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongDongThoiCoCacThongTin
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongDongThoiCoCacThongTin", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
