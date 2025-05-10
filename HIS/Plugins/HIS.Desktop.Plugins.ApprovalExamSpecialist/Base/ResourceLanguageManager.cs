using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageApprovalSpecialist { get; set; }
        internal static void InitResourceLanguageManager()
        {
            try
            {
                //LanguageApprovalSpecialist = new ResourceManager("HIS.Desktop.Plugins.BedRoomPartial.Resources.Lang", typeof(HIS.Desktop.Plugins.ApprovalExamSpecialist.frmApprovalExamSpecialist).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
