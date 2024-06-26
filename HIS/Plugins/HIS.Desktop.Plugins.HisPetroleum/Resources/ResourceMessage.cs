using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPetroleum.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisPetroleum.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string GiaTriNamNgoaiKhoangChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiaTriNamNgoaiKhoangChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChiChoPhep2ChuSoSauDauThapPhan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiChoPhep2ChuSoSauDauThapPhan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
