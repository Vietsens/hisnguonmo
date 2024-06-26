using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApproveServiceReqCLS.Resources
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguagefrmApproveServiceReqCLS { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguagefrmApproveServiceReqCLS = new ResourceManager("HIS.Desktop.Plugins.ApproveServiceReqCLS.Resources.Lang", typeof(HIS.Desktop.Plugins.ApproveServiceReqCLS.ApproveServiceReqCLS.frmApproveServiceReqCLS).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
