using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DebateDiagnostic.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormDebateDiagnostic { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormDebateDiagnostic = new ResourceManager("HIS.Desktop.Plugins.DebateDiagnostic.Resources.Lang", typeof(HIS.Desktop.Plugins.DebateDiagnostic.FormDebateDiagnostic).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
