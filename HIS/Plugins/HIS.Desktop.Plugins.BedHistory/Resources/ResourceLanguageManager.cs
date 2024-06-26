using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedHistory.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormBedHistory { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormBedHistory = new ResourceManager("HIS.Desktop.Plugins.BedHistory.Resources.Lang", typeof(HIS.Desktop.Plugins.BedHistory.FormBedHistory).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
