using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.CallPatientDepartmentV1",
        "Màn hình chờ xử lý theo khoa 1",
        "Common",
        63,
        "man-hinh.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class CallPatientDepartmentV1Processor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public CallPatientDepartmentV1Processor()
        {
            param = new CommonParam();
        }
        public CallPatientDepartmentV1Processor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                CallPatientDepartmentV1.ICallPatientDepartmentV1 behavior = CallPatientDepartmentV1.CallPatientDepartmentV1Factory.MakeIControl(param, args);
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
