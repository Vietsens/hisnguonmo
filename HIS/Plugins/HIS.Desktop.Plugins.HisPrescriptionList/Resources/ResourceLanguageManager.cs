using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPrescriptionList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisPrescriptionList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisPrescriptionList = new ResourceManager("HIS.Desktop.Plugins.HisPrescriptionList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisPrescriptionList.UCHisPrescriptionList).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
