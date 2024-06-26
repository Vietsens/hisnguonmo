using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentList.Resources
{
    class ResourceMessage
    {
        //static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentList.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
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

        internal static string TranhCaoTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__TranhCaoTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__ThongTinHenKham", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__KhongCoDuongDan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__KeyWork_InDate
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__KeyWork_InDate", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind_InDate
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind_InDate", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind__InMonth
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__InMonth", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind__InYear
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__InYear", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind__InTime
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__InTime", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }


         internal static string typeCodeFind__KeyWork_OutDate
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__KeyWork_OutDate", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind_OutDate
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind_OutDate", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string typeCodeFind__OutMonth
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__OutMonth", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind__OutYear
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__OutYear", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

         internal static string typeCodeFind__OutTime
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentList__typeCodeFind__OutTime", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
