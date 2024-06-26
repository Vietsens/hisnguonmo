using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish.Resources
{
    class ResourceLanguageManager
    {
        internal static System.Resources.ResourceManager LanguageFormTreatmentFinish = new System.Resources.ResourceManager(
            "HIS.Desktop.Plugins.Library.PrintTreatmentFinish.Resources.Lang", System.Reflection.Assembly.GetExecutingAssembly());
        
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

        internal static string GiayRaVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiayRaVien", LanguageFormTreatmentFinish, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return "";
            }
        }

        internal static string GiayChuyenVien
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("GiayChuyenVien", LanguageFormTreatmentFinish, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
