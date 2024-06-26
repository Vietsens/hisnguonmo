using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisExpMestLaboratory.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExpMestLaboratory.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExpMestLaboratory.HisExpMestLaboratory.FormLaboratory).Assembly);

        internal static string DuLieuBatBuoc
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("DuLieuBatBuoc", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoDuLieuHoaChat
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoDuLieuHoaChat", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string SoLuongLonHonKhaDung
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("SoLuongLonHonKhaDung", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string KhongCoSoLuong
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoSoLuong", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
