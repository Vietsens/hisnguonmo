using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineMediStockSummary.Resources
{
    public class ResourceLanguageManager
    {
        internal static ResourceManager LanguageUCMedicineMediStockSummary { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCMedicineMediStockSummary = new ResourceManager("HIS.Desktop.Plugins.MedicineMediStockSummary.Resources.Lang", typeof(HIS.Desktop.Plugins.MedicineMediStockSummary.ucMediaStockSummaryByMedicine).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
