using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.SupplyAnticipateCreate",
           "Lap du tru cung ung",
           "Common",
           16,
           "",
           "",
           Module.MODULE_TYPE_ID__FORM,
           true,
           true)
       ]
    public class SupplyAnticipateCreateProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public SupplyAnticipateCreateProcessor()
        {
            param = new CommonParam();
        }

        public SupplyAnticipateCreateProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                SupplyAnticipateCreate.ISupplyAnticipateCreate behavior = SupplyAnticipateCreate.SupplyAnticipateCreateFactory.MakeISupplyAnticipateCreate(param, args);
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
