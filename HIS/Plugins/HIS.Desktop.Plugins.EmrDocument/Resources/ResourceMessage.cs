using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmrDocument.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.EmrDocument.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string KhongChonVanBan
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ChooseRoom__KhongChonVanBan", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_EmrDocument__ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TaiVeThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_EmrDocument__TaiVeThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuMucDaTonTai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_EmrDocument__ThuMucDaTonTai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongLayDuocFile
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_EmrDocument__KhongLayDuocFile", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThongBaoCoMuonXoaCacDuLieuDaChon
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_EmrDocument__ThongBaoCoMuonXoaCacDuLieuDaChon", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
