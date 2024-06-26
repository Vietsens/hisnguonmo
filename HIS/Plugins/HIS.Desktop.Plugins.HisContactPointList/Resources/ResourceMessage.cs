using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisContactPointList.Resources
{
    class ResourceMessage
    {
        //static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisContactPointList.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisContactPointList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string InDay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__InDay", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__InMonth", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__Thongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TranhCaoTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__TranhCaoTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongTinHenKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__ThongTinHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuongDan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_HisContactPointList__KhongCoDuongDan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
