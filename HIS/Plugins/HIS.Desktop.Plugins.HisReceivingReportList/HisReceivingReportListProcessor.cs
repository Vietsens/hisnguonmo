using HIS.Desktop.Plugins.HisReceivingReportList.Run;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisReceivingReportList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
      "HIS.Desktop.Plugins.HisReceivingReportList",
      "Báo cáo đối soát ngân hàng",
      "Bussiness",
      4,
      "tru-so-hcm.png",
      "A",
      Module.MODULE_TYPE_ID__FORM,
      true,
      true)
   ]
    public class HisReceivingReportListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisReceivingReportListProcessor()
        {
            param = new CommonParam();
        }
        public HisReceivingReportListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IHisReceivingReportList behavior = HisReceivingReportListFactory.MakeReceivingControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
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
