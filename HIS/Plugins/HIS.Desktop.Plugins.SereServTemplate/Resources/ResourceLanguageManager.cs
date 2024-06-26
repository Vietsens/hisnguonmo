using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SereServTemplate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormSereServTemplate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormSereServTemplate = new ResourceManager("HIS.Desktop.Plugins.SereServTemplate.Resources.Lang", typeof(HIS.Desktop.Plugins.SereServTemplate.FormSereServTemplate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
