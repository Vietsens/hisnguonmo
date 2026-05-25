using HIS.Desktop.Plugins.ApprovalExamAnesthesia.ApprovalExamAnesthesia;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamAnesthesia
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.ApprovalExamAnesthesia",
       "Duyệt phiếu khám tiền gây mê",
       "Common",
       14,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true
       )
    ]
    public class ApprovalExamAnesthesiaProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ApprovalExamAnesthesiaProcessor()
        {
            param = new CommonParam();
        }

        public ApprovalExamAnesthesiaProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IApprovalExamAnesthesia behavior = ApprovalExamAnesthesiaFactory.MakeIApprovalExamAnesthesia(param, args);
                result = behavior != null ? behavior.Run() : null;
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
