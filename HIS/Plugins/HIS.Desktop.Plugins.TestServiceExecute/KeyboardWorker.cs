/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestServiceExecute
{
    [KeyboardAction("Save", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "Save", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("End", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "End", KeyStroke = XKeys.Control | XKeys.E)]
    [KeyboardAction("Print", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "Print", KeyStroke = XKeys.Control | XKeys.P)]
    [KeyboardAction("AssignService", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "AssignService", KeyStroke = XKeys.F9)]
    [KeyboardAction("AssignPre", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "AssignPre", KeyStroke = XKeys.F8)]
    //[KeyboardAction("UCServiceExecuteBtnService", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "UCServiceExecuteBtnService", KeyStroke = XKeys.F9)]
    //[KeyboardAction("UCServiceExecuteBtnPrescription", "HIS.Desktop.Plugins.TestServiceExecute.UCServiceExecute", "UCServiceExecuteBtnPrescription", KeyStroke = XKeys.F8)]
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
