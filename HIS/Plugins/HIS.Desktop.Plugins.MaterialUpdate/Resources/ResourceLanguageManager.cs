using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialUpdate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormMaterialUpdate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormMaterialUpdate = new ResourceManager("HIS.Desktop.Plugins.MaterialUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.MaterialUpdate.FormMaterialUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
