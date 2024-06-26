using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrSignTemplate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("EMR.Desktop.Plugins.EmrSignTemplate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BenhNhanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("EmrSignTemplate__BenhNhanKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaTonTaiNguoiKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("EmrSignTemplate__DaTonTaiNguoiKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDulieuThutuKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("EmrSignTemplate__KhongCoDulieuThutuKy", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
