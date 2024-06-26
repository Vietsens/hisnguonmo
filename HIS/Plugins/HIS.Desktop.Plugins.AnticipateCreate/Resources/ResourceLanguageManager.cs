using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnticipateCreate.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCAnticipateCreate { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCAnticipateCreate = new ResourceManager("HIS.Desktop.Plugins.AnticipateCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.AnticipateCreate.UCAnticipateCreate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
