using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggregateAndIssuePrescriptionOrderNumber.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.AggregateAndIssuePrescriptionOrderNumber.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string WellcomeText
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("WellcomeText", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string LoadingData
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("LoadingData", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Printing
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Printing", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
