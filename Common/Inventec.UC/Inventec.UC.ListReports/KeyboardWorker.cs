using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InvoiceBook
{
    [KeyboardAction("BtnSearch", "HIS.Desktop.Plugins.ExpenseList.UCExpenseList", "BtnFind", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("ThisClose", "HIS.Desktop.Plugins.ExpenseList.UCExpenseList", "BtnFind", KeyStroke = XKeys.Escape)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<IDesktopToolContext>
    {
        public KeyboardWorker() : base() { }

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
