using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrImpMestList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCAggrImpMestList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCAggrImpMestList = new ResourceManager("HIS.Desktop.Plugins.AggrImpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.AggrImpMestList.UCAggrImpMestList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
