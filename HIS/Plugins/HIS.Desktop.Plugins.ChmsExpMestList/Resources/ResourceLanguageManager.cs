using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ChmsExpMestList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormChmsExpMestList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormChmsExpMestList = new ResourceManager("HIS.Desktop.Plugins.ChmsExpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.ChmsExpMestList.UCChmsExpMestList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
