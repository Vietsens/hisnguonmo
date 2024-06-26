using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCMaterialList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCMaterialList = new ResourceManager("HIS.Desktop.Plugins.MaterialList.Resources.Lang", typeof(HIS.Desktop.Plugins.MaterialList.UCMaterialList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
