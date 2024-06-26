using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServiceReqMaty.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisServiceReqMaty.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChuaNhapDuThongTin
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisServiceReqMaty__ChuaNhapDuThongTin", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaChonLoaiVatTu
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisServiceReqMaty__ChuaChonLoaiVatTu", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string NhapLonHon0
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisServiceReqMaty__NhapLonHon0", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
