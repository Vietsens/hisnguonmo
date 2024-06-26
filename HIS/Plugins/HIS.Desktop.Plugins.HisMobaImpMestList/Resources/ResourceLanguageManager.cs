using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMobaImpMestList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisMobaImpMestList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisMobaImpMestList = new ResourceManager("HIS.Desktop.Plugins.HisMobaImpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMobaImpMestList.UCHisMobaImpMestList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
