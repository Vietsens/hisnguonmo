using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrApprove.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageResource { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AggrApprove.Resources.Lang", typeof(HIS.Desktop.Plugins.AggrApprove.frmAggrApprove).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
