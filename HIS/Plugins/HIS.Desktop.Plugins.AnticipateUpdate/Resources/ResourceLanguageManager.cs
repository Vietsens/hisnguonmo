using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnticipateUpdate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCAnticipateUpdate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCAnticipateUpdate = new ResourceManager("HIS.Desktop.Plugins.AnticipateUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.AnticipateUpdate.UCAnticipateUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
