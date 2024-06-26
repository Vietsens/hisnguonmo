using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDosageForm.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisDosageForm.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string DaTonTaiTenDaDuocGanVoiMa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin__DaTonTaiTenDaDuocGanVoiMa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
