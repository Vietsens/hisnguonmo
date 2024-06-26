using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ScnNutrition
{
    public class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ScnNutrition.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnNutrition_TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string DuLieuPhaiLonHayKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnNutrition_DuLieuPhaiLonHayKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }
    }
}
