using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InfomationExecute.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormTextLibrary { get; set; }
		public static ResourceManager LanguageResource { get; internal set; }

		internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormTextLibrary = new ResourceManager("HIS.Desktop.Plugins.InfomationExecute.Resources.Lang", typeof(HIS.Desktop.Plugins.InfomationExecute.frmInfomationExecute).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
