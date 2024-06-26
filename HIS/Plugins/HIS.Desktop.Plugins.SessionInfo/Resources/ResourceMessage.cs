using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SessionInfo.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.SessionInfo.Resources.Message", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanChuaChonSoBienLai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_SessionInfo__BanChuaChonSoBienLai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_SessionInfo__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuLieuUyQuyen
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_SessionInfo__KhongCoDuLieuUyQuyen", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonSoTamThu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_SessionInfo__BanChuaChonSoTamThu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
