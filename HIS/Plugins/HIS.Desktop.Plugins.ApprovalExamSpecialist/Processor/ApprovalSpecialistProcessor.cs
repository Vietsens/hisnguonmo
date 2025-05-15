using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Plugins.ApprovalExamSpecialist.ApprovalExamSpecialist;
namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Processor
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.ApprovalExamSpecialist",
       "Duyệt khám chuyên khoa",
       "Common",
       14,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true
       )
    ]
    public class ApprovalExamSpecialist : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ApprovalExamSpecialist()
        {
            param = new CommonParam();
        }
        public ApprovalExamSpecialist(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IApprovalExamSpecialist behavior = ApprovalExamSpecialistFactory.MakeIApprovalExamSpecialist(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            bool result = false;
            try
            {
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }
    }
}
