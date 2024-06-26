using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InImpMestEdit.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormInImpMestEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormInImpMestEdit = new ResourceManager("HIS.Desktop.Plugins.InitImpMestEdit.Resources.Lang", typeof(HIS.Desktop.Plugins.InImpMestEdit.FormInImpMestEdit).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
