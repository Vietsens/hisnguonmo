using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCMedicineList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCMedicineList = new ResourceManager("HIS.Desktop.Plugins.MedicineList.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineList.UCMedicineList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
