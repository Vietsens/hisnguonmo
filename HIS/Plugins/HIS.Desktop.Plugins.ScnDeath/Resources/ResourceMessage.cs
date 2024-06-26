using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ScnDeath
{
    public class ResourceMessage
    {
        internal static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ScnDeath.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_DuLieuPhaiLonHayKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TaiCoSoYTe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_TaiCoSoYTe", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TaiNha
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_TaiNha", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string Khac
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_Khac", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TrangThai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_TrangThai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                return "";
            }
        }

        internal static string TruongDuLieuVuotQuaKyTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ScnDeath_TruongDuLieuVuotQuaKyTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
