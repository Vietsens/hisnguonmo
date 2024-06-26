using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BedAssign.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormBedAssign { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormBedAssign = new ResourceManager("HIS.Desktop.Plugins.BedAssign.Resources.Lang", typeof(HIS.Desktop.Plugins.BedAssign.FormBedAssign).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
