using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineUpdate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormMedicineUpdate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormMedicineUpdate = new ResourceManager("HIS.Desktop.Plugins.MedicineUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineUpdate.FormMedicineUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
