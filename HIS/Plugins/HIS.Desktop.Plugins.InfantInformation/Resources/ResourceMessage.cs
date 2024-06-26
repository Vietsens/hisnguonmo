using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InfantInformation
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.InfantInformation.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string BanNhapSaiDinhDangGio
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__SaiDinhDangGio", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoThangPhaiNhoHon9
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__SoThangPhaiNhoHon9", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string SoTuanPhaiNhoHon40
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__SoTuanPhaiNhoHon40", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string PhaiNhapDungChuanCMT
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__PhaiNhapDungChuanCMT", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TruongDuLieuVuotQuaKyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_InfantInformation__TruongDuLieuVuotQuaKyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
