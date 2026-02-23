using HIS.Desktop.Plugins.TreatmentGuaranteeList.TreatmentGuaranteeList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentGuaranteeList
{
    class TreatmentGuaranteeListProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.TreatmentGuaranteeList",
            "Danh sách hồ sơ bảo lãnh",
            "Common",
            68,
            "tai-chinh.png",
            "A",
            Module.MODULE_TYPE_ID__UC,
            true,
            true)]
        public class TreatmentGuaranteeList : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public TreatmentGuaranteeList()
            {
                param = new CommonParam();
            }
            public TreatmentGuaranteeList(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    ITreatmentGuaranteeList behavior = TreatmentGuaranteeListFactory.MakeIControl(param, arge);
                    result = behavior != null ? (object)(behavior.Run()) : null;
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
                    return result;
                }
                return result;
            }
        }
    }
}
