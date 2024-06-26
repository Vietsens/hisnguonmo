using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrDocumentList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisBidList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisBidList = new ResourceManager("EMR.Desktop.Plugins.EmrDocumentList.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrDocumentList.UCEmrDocumentList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
