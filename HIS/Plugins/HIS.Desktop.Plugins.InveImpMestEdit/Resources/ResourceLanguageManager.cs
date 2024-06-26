using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InveImpMestEdit.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormInveImpMestEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormInveImpMestEdit = new ResourceManager("HIS.Desktop.Plugins.InveImpMestEdit.Resources.Lang", typeof(HIS.Desktop.Plugins.InveImpMestEdit.FormInveImpMestEdit).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
