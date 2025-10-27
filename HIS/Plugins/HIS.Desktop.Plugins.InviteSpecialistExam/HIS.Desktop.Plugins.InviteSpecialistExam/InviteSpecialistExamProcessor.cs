using HIS.Desktop.Plugins.InviteSpecialistExam.InviteSpecialistExam;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HIS.Desktop.Plugins.InviteSpecialistExam.InviteSpecialistExamProcessor;

namespace HIS.Desktop.Plugins.InviteSpecialistExam
{
    class InviteSpecialistExamProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
            "HIS.Desktop.Plugins.InviteSpecialistExam",
            "Mời khám chuyên khoa",
            "Common",
            68,
            "",
            "A",
            Module.MODULE_TYPE_ID__FORM,
            true,
            true)]
        public class InviteSpecial : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public InviteSpecial()
            {
                param = new CommonParam();
            }
            public InviteSpecial(CommonParam paramBussiness)
            {
                param = (paramBussiness != null ? paramBussiness : new CommonParam());
            }
            public object Run(object[] arge)
            {
                object result = null;
                try
                {
                    IinviteSpecialistExam behavior = InviteSpecialistExamFactory.MakeIControl(param, arge);
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
