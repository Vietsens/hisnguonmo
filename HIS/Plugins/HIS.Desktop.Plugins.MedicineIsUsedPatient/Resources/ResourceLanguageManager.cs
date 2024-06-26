using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.Resources
{
    class ResourceLanguageManager
    {
        internal static ResourceManager LanguageResource { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MedicineIsUsedPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineIsUsedPatient.MedicineIsUsedPatient.frmMedicineIsUsedPatient).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string KhongTimThayMaDieuTri
        {
            get
            {
                try
                {
                    return Inventec.Common.Resource.Get.Value("Plugins_ElectronicBillTotal__KhongTimThayMaDieuTri", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                    return Inventec.Common.Resource.Get.Value("Plugins_ElectronicBillTotal__ThongBao", LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
