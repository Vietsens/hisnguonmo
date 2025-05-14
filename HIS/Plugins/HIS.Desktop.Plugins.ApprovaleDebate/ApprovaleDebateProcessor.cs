using HIS.Desktop.Plugins.a2ApprovaleDebate.ApprovaleDebate;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.a2ApprovaleDebate
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
   "HIS.Desktop.Plugins.a2ApprovaleDebate",
   "Duyệt hội chẩn",
   "Common",
   14,
   "pivot_32x32.png",
   "A",
   Module.MODULE_TYPE_ID__UC,
   true,
   true
   )
]
    public class ApprovaleDebateProcessor : ModuleBase, IDesktopRoot
    {

        CommonParam param;
        public ApprovaleDebateProcessor()
        {
            param = new CommonParam();
        }
        public ApprovaleDebateProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IApprovaleDebate behavior = ApprovaleDebateFactory.MakeIApprovalSurgery(param, args);
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
