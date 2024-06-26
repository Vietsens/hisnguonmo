using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormInImpMestEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormInImpMestEdit = new ResourceManager("HIS.Desktop.Plugins.ManuImpMestUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.ManuImpMestUpdate.frmManuImpMestUpdate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static ResourceManager LanguageResource { get; set; }
        public static ResourceManager LanguageResource__frmDetail { get; set; }
    }
}
