using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalContractList.Resources
{
    class ResourceMessage
    {
        //static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentList.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisMedicalContractList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string InDay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__InDay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string InMonth
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__InMonth", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__Thongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
