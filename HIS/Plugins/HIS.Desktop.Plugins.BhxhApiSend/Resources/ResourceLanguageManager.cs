using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BhxhApiSend.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormBhxhApiSend { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormBhxhApiSend = new ResourceManager("HIS.Desktop.Plugins.BhxhApiSend.Resources.Lang", typeof(HIS.Desktop.Plugins.BhxhApiSend.FormBhxhApiSend).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static ResourceManager LanguageResource { get; set; }
    }
}
