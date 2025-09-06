using HIS.Desktop.Plugins.HisRegimenTemp.HisReigimenTemp;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisRegimenTemp
{
    class RegimenTempProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.HisRegimenTemp",
            "Nội dung mẫu",
            "Business",
            25,
            "ngon-ngu.png",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class RegiTempProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public RegiTempProcessor()
            {
                param = new CommonParam();
            }
            public RegiTempProcessor(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    IRegimenTemp behavior = RegimenTempFactory.MakeIControl(param, arge);
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
