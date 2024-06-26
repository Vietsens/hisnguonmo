using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrSignDocumentList.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrSignDocumentList.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrSignDocumentList.UcEmrSignDocumentList).Assembly);

        internal static string KetThucXuLy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KetThucXuLy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string ChiChoPhepKyVoiCacVanBanChoKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChiChoPhepKyVoiCacVanBanChoKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }
        
        internal static string BanChuaCoVanBanChoKyNao
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaCoVanBanChoKyNao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongTimDuocVanBanKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongTimDuocVanBanKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string ChoKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("ChoKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string TuChoiKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("TuChoiKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string DaKy
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DaKy", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
