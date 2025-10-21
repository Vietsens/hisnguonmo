using EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScaned;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EMR.Desktop.Plugins.ListEmrDocumentScaned.ListEmrDocumentScanedProcessor;

namespace EMR.Desktop.Plugins.ListEmrDocumentScaned
{
    class ListEmrDocumentScanedProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "EMR.Desktop.Plugins.ListEmrDocumentScaned",
            "Danh sách văn bản chưa nghi nhận",
            "Common",
            68,
            "",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class EmrDocumentScanned : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public EmrDocumentScanned()
            {
                param = new CommonParam();
            }
            public EmrDocumentScanned(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    IListEmrDocumentScaned behavior = ListEmrDocumentScanedFactory.MakeIControl(param, arge);
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
