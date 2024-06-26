using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisManuImportMestMedicine.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisImportMestMedicine { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisImportMestMedicine = new ResourceManager("HIS.Desktop.Plugins.HisManuImportMestMedicine.Resources.Lang", typeof(HIS.Desktop.Plugins.HisManuImportMestMedicine.UCHisManuImportMestMedicine).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
