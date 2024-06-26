using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisSampleAggregation.Resources
{
    class ResourceLanguageManager
    {
        public static System.Resources.ResourceManager LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.LisSampleAggregation.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

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

        internal static string KhongCoMauBenhPhamUngVoiBarcode
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoMauBenhPhamUngVoiBarcode", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauDaGop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("MauDaGop", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Mau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Mau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDanhSachMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoDanhSachMau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonMau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonMau", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Gop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Gop", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaGop
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaGop", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
