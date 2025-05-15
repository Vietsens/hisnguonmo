using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovaleDebate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string NhapQuaMaxlength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NhapQuaMaxlength", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
