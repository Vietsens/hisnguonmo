using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisSaleExpMestList.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisPrescriptionList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisPrescriptionList = new ResourceManager("HIS.Desktop.Plugins.HisSaleExpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSaleExpMestList.UCHisSaleExpMestList).Assembly);
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
