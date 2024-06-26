using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAR.Desktop.Plugins.SarPrintList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("SAR.Desktop.Plugins.SarPrintList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanCoMuonXoaKhongDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SarPrintList__BanCoMuonXoaKhongDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
