using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyOrderList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.SupplyOrderList",
           "Danh sach don dat hang",
           "Common",
           16,
           "",
           "",
           Module.MODULE_TYPE_ID__UC,
           true,
           true)
       ]
    public class SupplyOrderListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public SupplyOrderListProcessor()
        {
            param = new CommonParam();
        }

        public SupplyOrderListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                SupplyOrderList.ISupplyOrderList behavior = SupplyOrderList.SupplyOrderListFactory.MakeISupplyOrderList(param, args);
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
