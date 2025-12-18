using HIS.Desktop.Plugins.MchExamServiceList.MchExamServiceList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchExamServiceList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.MchExamServiceList",
           "",
           "",
           0,
           "",
           "",
           Module.MODULE_TYPE_ID__UC,
           true,
           true)]

    class UCMchExamServiceListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public UCMchExamServiceListProcessor()
        {
                
        }
        public UCMchExamServiceListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IMchExamServiceList behavior = MchExamServiceListFactory.MakeIMchExamServiceList(param, args);
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
