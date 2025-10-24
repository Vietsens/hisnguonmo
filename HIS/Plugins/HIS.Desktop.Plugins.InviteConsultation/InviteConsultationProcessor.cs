using HIS.Desktop.Plugins.InviteConsultation.InviteConsultation;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InviteConsultation
{
    class InviteConsultationProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.InviteConsultation",
            "Mời hội chẩn",
            "Common",
            68,
            "",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class InviteConsultation : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public InviteConsultation()
            {
                param = new CommonParam();
            }
            public InviteConsultation(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    IinviteConsultation behavior = InviteConsultationFactory.MakeIControl(param, arge);
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
