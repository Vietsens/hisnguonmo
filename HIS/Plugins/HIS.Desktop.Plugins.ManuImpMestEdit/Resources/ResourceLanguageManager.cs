using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ManuImpMestEdit.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormManuImpMestEdit { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormManuImpMestEdit = new ResourceManager("HIS.Desktop.Plugins.ManuImpMestEdit.Resources.Lang", typeof(HIS.Desktop.Plugins.ManuImpMestEdit.FormManuImpMestEdit).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
