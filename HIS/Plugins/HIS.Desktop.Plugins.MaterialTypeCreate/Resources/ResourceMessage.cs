using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager language = new System.Resources.ResourceManager("HIS.Desktop.Plugins.MaterialTypeCreate.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string Luuchinhsachgiathatbai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_MaterialType_Create_Luuchinhsachgiathatbai", language, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Thongbao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_MaterialType_Create_Thongbao", language, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
