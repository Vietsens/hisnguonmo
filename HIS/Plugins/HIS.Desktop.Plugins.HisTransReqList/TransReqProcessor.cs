using HIS.Desktop.Plugins.HisTransReqList.HisTransReqList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTransReqList
{
    class TransReqProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.HisTransReqList",
            "Danh sách QR",
            "Common",
            68,
            "tai-chinh.png",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class TransReq : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public TransReq()
            {
                param = new CommonParam();
            }
            public TransReq(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    ITransReq behavior = TransReqFactory.MakeIControl(param, arge);
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
