using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vva.Desktop.Plugins.VvaVoiceCommand.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("VVA.Desktop.Plugins.VvaVoiceCommand.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ThongBaoDuLieuDaCo
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_VvaVoiceCommand__ThongBaoDuLieuDaCo", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonXoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_VvaVoiceCommand__BanCoMuonXoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_VvaVoiceCommand__BanCoMuonKhoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonBoKhoaDuLieu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_VvaVoiceCommand__BanCoMuonBoKhoaDuLieu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_VvaVoiceCommand__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
