using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PrepareList.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PrepareList.Resources.Lang", typeof(HIS.Desktop.Plugins.PrepareList.UCPrepareList).Assembly);

        internal static string HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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

        internal static string CacPhieuDaDuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("CacPhieuDaDuyet", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string BanChuaChonDuLieuDuyet
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonDuLieuDuyet", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
