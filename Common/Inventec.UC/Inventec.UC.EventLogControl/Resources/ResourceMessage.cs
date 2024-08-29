using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("Inventec.UC.EventLogControl.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string typeCodeFind__KeyWork
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__KeyWork", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__MaBN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__MaBN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__MaDT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__MaDT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__MaPX
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__MaPX", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__MaPN
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__MaPN", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__MaYL
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("typeCodeFind__MaYL", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GetResourceMessage(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string Ngaythangkhongduocbotrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Ngaythangkhongduocbotrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string InDay
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("INVENTEC_UC_EVENT_LOG_CONTROL__IN_DAY", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("INVENTEC_UC_EVENT_LOG_CONTROL__IN_MONTH", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
