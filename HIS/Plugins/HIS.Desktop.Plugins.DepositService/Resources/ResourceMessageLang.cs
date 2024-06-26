using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DepositService.Base
{
    class ResourceMessageLang
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.DepositService.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        internal static string BnCoSuDungTheKhamChuaBenh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BnCoSuDungTheKhamChuaBenh", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ThieuThongTinTheNganHang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThieuThongTinTheNganHang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string ChuaNhapDuTheNganHang
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChuaNhapDuTheNganHang", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TruongDuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DepositService__TruongDuLieuBatBuoc", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        internal static string TamUngQuaTheThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_DepositService__TamUngQuaTheThatBai", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
