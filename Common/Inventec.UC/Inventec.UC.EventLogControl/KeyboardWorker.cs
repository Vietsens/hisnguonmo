using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl
{
    [KeyboardAction("BtnSearch", "HIS.Desktop.Plugins.TreatmentList.EventLogControl", "BtnSearch", KeyStroke = (XKeys.B | XKeys.Control | XKeys.MiddleMouseButton)), KeyboardAction("BtnRefreshs", "HIS.Desktop.Plugins.TreatmentList.EventLogControl", "BtnRefreshs", KeyStroke = (XKeys.B | XKeys.Control | XKeys.ShiftKey)), ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<DesktopToolContext>
    {
        public override IActionSet Actions
        {
            get
            {
                return base.Actions;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
