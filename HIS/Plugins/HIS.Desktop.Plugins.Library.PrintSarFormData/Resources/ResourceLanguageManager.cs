using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintSarFormData.Resources
{
    class ResourceLanguageManager
    {
        internal static System.Resources.ResourceManager LanguageFormTreatmentFinish = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.Library.PrintSarFormData.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
        internal static string BenhNhanChuaKhoaVienPhi
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("BenhNhanChuaKhoaVienPhi", LanguageFormTreatmentFinish, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
