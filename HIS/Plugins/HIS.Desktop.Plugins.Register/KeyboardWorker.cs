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

namespace HIS.Desktop.Plugins.Register
{
    [KeyboardAction("PatientNew", "HIS.Desktop.Plugins.Register.Run.UCRegister", "PatientNew", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("Save", "HIS.Desktop.Plugins.Register.Run.UCRegister", "Save", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("SaveAndPrint", "HIS.Desktop.Plugins.Register.Run.UCRegister", "SaveAndPrint", KeyStroke = XKeys.Control | XKeys.I)]
    [KeyboardAction("Print", "HIS.Desktop.Plugins.Register.Run.UCRegister", "Print", KeyStroke = XKeys.Control | XKeys.P)]
    [KeyboardAction("AssignService", "HIS.Desktop.Plugins.Register.Run.UCRegister", "AssignService", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("Bill", "HIS.Desktop.Plugins.Register.Run.UCRegister", "Bill", KeyStroke = XKeys.Control | XKeys.B)]
    [KeyboardAction("Deposit", "HIS.Desktop.Plugins.Register.Run.UCRegister", "Deposit", KeyStroke = XKeys.Control | XKeys.T)]
    [KeyboardAction("InBed", "HIS.Desktop.Plugins.Register.Run.UCRegister", "InBed", KeyStroke = XKeys.Control | XKeys.G)]
    [KeyboardAction("New", "HIS.Desktop.Plugins.Register.Run.UCRegister", "New", KeyStroke = XKeys.Control | XKeys.N)]
    [KeyboardAction("ClickF2", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF2", KeyStroke = XKeys.F2)]
    [KeyboardAction("ClickF3", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF3", KeyStroke = XKeys.F4)]
    [KeyboardAction("ClickF4", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF4", KeyStroke = XKeys.F5)]
    [KeyboardAction("ClickF5", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF5", KeyStroke = XKeys.F6)]
    [KeyboardAction("ClickF6", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF6", KeyStroke = XKeys.F7)]
    [KeyboardAction("ClickF7", "HIS.Desktop.Plugins.Register.Run.UCRegister", "ClickF7", KeyStroke = XKeys.F3)]
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
