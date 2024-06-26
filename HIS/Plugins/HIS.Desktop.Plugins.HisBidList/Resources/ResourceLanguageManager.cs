using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisBidList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisBidList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisBidList = new ResourceManager("HIS.Desktop.Plugins.HisBidList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBidList.UCHisBidList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
