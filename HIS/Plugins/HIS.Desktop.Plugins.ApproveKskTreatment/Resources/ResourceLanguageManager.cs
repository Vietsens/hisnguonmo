using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApproveKskTreatment.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageUCHisAggrExpMestList { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageUCHisAggrExpMestList = new ResourceManager("HIS.Desktop.Plugins.ApproveKskTreatment.Resources.Lang", typeof(HIS.Desktop.Plugins.ApproveKskTreatment.UCApproveKskTreatment).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
