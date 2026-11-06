using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.SupplyOrderCreate",
           "Lap don dat hang",
           "Common",
           16,
           "",
           "",
           Module.MODULE_TYPE_ID__FORM,
           true,
           true)
       ]
    public class SupplyOrderCreateProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public SupplyOrderCreateProcessor()
        {
            param = new CommonParam();
        }

        public SupplyOrderCreateProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                SupplyOrderCreate.ISupplyOrderCreate behavior = SupplyOrderCreate.SupplyOrderCreateFactory.MakeISupplyOrderCreate(param, args);
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
