using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.ImportEmrSigner.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("EMR.Desktop.Plugins.ImportEmrSigner.Resources.Lang", typeof(EMR.Desktop.Plugins.ImportEmrSigner.FormImportEmrSigner).Assembly);

        internal static string HeThongTBKQXLYCCuaFrontendThatBai
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBKQXLYCCuaFrontendThatBai", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string Maxlength
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Maxlength", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongHopLe
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongHopLe", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GioiHanAnh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GioiHanAnh", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
