using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.OtherForms.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.OtherForms.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string Tuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Caption_Tuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThangTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Caption_ThangTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NgayTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Caption_NgayTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GioTuoi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Caption_GioTuoi", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
