using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServicePatyList.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisServicePatyList.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string ChuaNhapTruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisServicePatyList__ChuaNhapTruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_HisServicePatyList__BanCoMuonXoaDuLieuKhong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HeThongTBKQXLYCCuaFrontendThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBKQXLYCCuaFrontendThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("ThongBao", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string HisServicePaty__ExportExcel__ThanhCong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisServicePaty__ExportExcel__ThanhCong", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        public static string HisServicePaty__ExportExcel__ThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HisServicePaty__ExportExcel__ThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
