using Inventec.Desktop.Core;
using System;
using Inventec.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionRefectory
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.AssignNutritionRefectory",
       "Chỉ định suất ăn",
       "Common",
       14,
       "newitem_32x32.png",
       "A",
       Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )
    ]
    class AssignNutritionRefectoryProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public AssignNutritionRefectoryProcessor()
        {
            param = new CommonParam();
        }
        public AssignNutritionRefectoryProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IAssignNutritionRefectory behavior = AssignNutritionRefectoryFactory.MakeIAssignNutritionRefectory(param, args);
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
