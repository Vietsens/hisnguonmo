using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrTreatmentList.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrTreatmentList.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrTreatmentList.UcEmrTreatmentList).Assembly);

        internal static string KhongCoQuyenXem
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("KhongCoQuyenXem", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
