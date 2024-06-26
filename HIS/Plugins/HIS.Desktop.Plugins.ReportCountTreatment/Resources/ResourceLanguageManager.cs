using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReportCountTreatment.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ReportCountTreatment.Resources.Lang", typeof(HIS.Desktop.Plugins.ReportCountTreatment.FormReportCountTreatment).Assembly);

        internal static string BanChuaChonChiNhanh
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BanChuaChonChiNhanh", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
