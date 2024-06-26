using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentInspection.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.TreatmentInspection.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string Thongbao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__Thongbao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__TranhCaoTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChuaLuutuBenhAn
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__ChuaLuutuBenhAn", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaDuyetGiamDinh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__DaDuyetGiamDinh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauTrang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauTrang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Mauvang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauVang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauXanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauXanh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string MauDo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_TreatmentInspection__MauDo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
