using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApproveMobaImpMest.Resources
{
    class ResourceMessage
    {
        static System.Resources.ResourceManager languageMessage = new System.Resources.ResourceManager("HIS.Desktop.Plugins.ApproveMobaImpMest.Resources.Message.Lang", System.Reflection.Assembly.GetExecutingAssembly());

        internal static string SpinEdit__SoLuongDuyetPhaiLonHonKhongVaNhoHonGiaTriYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ApproveMobaImpMest__SpinEdit__SoLuongDuyetPhaiLonHonKhongVaNhoHonGiaTriYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TextEdit__GhiChuVuotQuakyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugin_ApproveMobaImpMest__TextEdit__GhiChuVuotQuakyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string ThuocVatTuCoSoLuongDuyetNhoHonKhongHoacLonGiaTriYeuCau
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuCoSoLuongDuyetNhoHonKhongHoacLonGiaTriYeuCau", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ThuocVatTuCoGhiChuVuotQuakyTuChoPhep
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ThuocVatTuCoGhiChuVuotQuakyTuChoPhep", languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
