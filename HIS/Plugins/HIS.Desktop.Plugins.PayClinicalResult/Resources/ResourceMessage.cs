using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PayClinicalResult.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.PayClinicalResult.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonHuyBatDauKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__BanCoMuonHuyBatDauKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string XacNhanHuyBatDau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ExecuteRoom__XacNhanHuyBatDau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
