using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentAppointment.Resources
{
    public class ResourceMessageLang
    {
        public static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentAppointment.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string DaTaiKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaTaiKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaTaiKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaTaiKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TatCa
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TatCa", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DenNgayHenKhamTrong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DenNgayHenKhamTrong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string DaQuaThoiGianHenKham
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaQuaThoiGianHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThoiGianHenKhamTrongKhoang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThoiGianHenKhamTrongKhoang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
