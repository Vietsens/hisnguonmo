using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisHivGroupPatient.HisHivGroupPatient;

namespace HIS.Desktop.Plugins.HisHivGroupPatient
{
    class HisHivGroupPatientProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.HisHivGroupPatient",
            "Danh mục chung",
            "Bussiness",
            8,
            "module.png",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]

        public class GroupPatientProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public GroupPatientProcessor()
            {
                param = new CommonParam();
            }

            public GroupPatientProcessor(CommonParam paramBuss)
            {
                param = paramBuss != null ? paramBuss : new CommonParam();
            }

            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    IGroupPatient Igp = GroupPatientFactory.MakeIControl(param, arge);
                    result = Igp != null ? (object)Igp.Run() : null;
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
