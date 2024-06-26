using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrDocumentListAll.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisBidList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisBidList = new ResourceManager("EMR.Desktop.Plugins.EmrDocumentListAll.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrDocumentListAll.FrmEmrDocumentListAll).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
