using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AdjustmentTransaction
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.AdjustmentTransaction",
        "Điều chỉnh hóa đơn",
        "Common",
        59,
        "thanh-toan.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]

    public class AdjhstmentTransactionProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public AdjhstmentTransactionProcessor()
        {
            param = new CommonParam();
        }
        public AdjhstmentTransactionProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                IAdjustmentTransaction behavior = AdjustmentTransactionFactory.MakeIIAdjustmentTransaction(param, args);
                result = (behavior != null) ? behavior.Run() : null;
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
            return false;
        }
    }
}
