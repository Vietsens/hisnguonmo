using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExportPrescription.Resources
{
    public class ResourceLanguageManager
    {
        public static ResourceManager LanguageFormApprovalExportPrescription { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                ResourceLanguageManager.LanguageFormApprovalExportPrescription = new ResourceManager("HIS.Desktop.Plugins.ApprovalExportPrescription.Resources.Lang", typeof(HIS.Desktop.Plugins.ApprovalExportPrescription.FormApprovalExportPrescription).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
