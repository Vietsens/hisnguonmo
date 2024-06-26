using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CompareBhytInfo.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }

        internal static string KhongTimThayFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimThayFile", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuLieuLoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoDuLieuLoi", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuLieuXml
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoDuLieuXml", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoThongTinDanhMucBhyt
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoThongTinDanhMucBhyt", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
