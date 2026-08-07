using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Pharmacology
{
    class PharmacologyProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.Pharmacology",
            "Dược lý",
            "Bussiness",
            8,
            "thuoc.png",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class PharmacologyModuleProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;

            public PharmacologyModuleProcessor()
            {
                param = new CommonParam();
            }

            public PharmacologyModuleProcessor(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }

            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    IPharmacology behavior = PharmacologyFactory.MakeIControl(param, args);
                    result = behavior != null ? (object)(behavior.Run()) : null;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
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
                    LogSystem.Error(ex);
                    return result;
                }
                return result;
            }
        }
    }
}
