using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DeathInformationList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.DeathInformationList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongLayDuocChungThu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongLayDuocChungThu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string KhongLayDuocChungThu2
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongLayDuocChungThu2", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SaiDiaChi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SaiDiaChi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
