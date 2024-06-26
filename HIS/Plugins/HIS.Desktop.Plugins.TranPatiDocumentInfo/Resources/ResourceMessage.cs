using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TranPatiDocumentInfo.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TranPatiDocumentInfo.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string Thongbao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TranPatiDocumentInfo__Thongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
