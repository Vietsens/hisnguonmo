using HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.OpensourceHisStoreReportDetail;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.OpensourceHisStoreReportDetail",
       "Khác",
       "Business",
       8,
       "",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]
    public class OpensourceHisStoreReportDetailProcessor : ModuleBase, IDesktopRoot
    {
         CommonParam param;
        public OpensourceHisStoreReportDetailProcessor()
        {
            param = new CommonParam();
        }
        public OpensourceHisStoreReportDetailProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                IOpensourceHisStoreReportDetail behavior = OpensourceHisStoreReportDetailFactory.MakeIHisTreatmentRecordChecking(param, args);
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
            return true;
        }
    }
}
