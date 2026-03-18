using HIS.Desktop.Plugins.RefundByTransfer.RefundByTransfer;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RefundByTransfer
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.RefundByTransfer",
    "",
    "Common",
    14,
    "newcontact_32x32.png",
    "A",
    Module.MODULE_TYPE_ID__FORM,
    true,
    true
    )
    ]
    public class RefundByTransferProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public RefundByTransferProcessor()
        {
            param = new CommonParam();
        }
        public RefundByTransferProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IRefundByTransfer behavior = RefundByTransferFactory.MakeIControl(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
