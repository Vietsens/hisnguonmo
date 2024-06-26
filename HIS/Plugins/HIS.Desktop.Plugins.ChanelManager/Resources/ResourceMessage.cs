using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ChanelManager.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ChanelManager.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongChonControlCanAnHien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongChonControlCanAnHien", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
