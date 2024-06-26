using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqSessionDetail.Resources
{
    class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormBidDetail { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormBidDetail = new ResourceManager("HIS.Desktop.Plugins.ServiceReqSessionDetail.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqSessionDetail.frmServiceReqSessionDetail).Assembly);
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
