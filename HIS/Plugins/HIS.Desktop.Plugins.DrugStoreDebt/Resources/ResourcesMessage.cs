using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DrugStoreDebt.Resources
{
    class ResourcesMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.DrugStoreDebt.Resources.Message", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string VuotQuaDoDaiChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("VuotQuaDoDaiChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string NguoiDungChuaChonPhieuXuatDeChotNo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("NguoiDungChuaChonPhieuXuatDeChotNo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
