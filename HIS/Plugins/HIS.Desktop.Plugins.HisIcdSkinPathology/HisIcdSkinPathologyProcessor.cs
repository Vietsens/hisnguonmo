using HIS.Desktop.Plugins.HisIcdSkinPathology.HisIcdSkinPathology;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisIcdSkinPathology
{
    class HisIcdSkinPathologyProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.HisIcdSkinPathology",
            "Danh mục",
            "Bussiness",
            13,
            "icd.png",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class IcdSkinPathologyProcessor: ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public IcdSkinPathologyProcessor()
            {
                param = new CommonParam();
            }
            public IcdSkinPathologyProcessor(CommonParam paramBussiness)
            {
                param = (paramBussiness == null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    IHisIcdSkinPathology behavior = HisIcdSkinPathologyFactory.MakeIControl(param, args);
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
