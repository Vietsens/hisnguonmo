using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyAnticipateList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.SupplyAnticipateList",
           "Danh sach du tru cung ung",
           "Common",
           16,
           "",
           "",
           Module.MODULE_TYPE_ID__UC,
           true,
           true)
       ]
    public class SupplyAnticipateListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public SupplyAnticipateListProcessor()
        {
            param = new CommonParam();
        }

        public SupplyAnticipateListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                SupplyAnticipateList.ISupplyAnticipateList behavior = SupplyAnticipateList.SupplyAnticipateListFactory.MakeISupplyAnticipateList(param, args);
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
