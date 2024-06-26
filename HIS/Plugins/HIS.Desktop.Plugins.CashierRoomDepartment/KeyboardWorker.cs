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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CashierRoomDepartment
{
    [Inventec.Desktop.Core.Actions.KeyboardAction("FindCashier", "HIS.Desktop.Plugins.CashierRoomDepartment.UCCashierRoomDepartment", "FindCashier", KeyStroke = Inventec.Desktop.Core.XKeys.Control | Inventec.Desktop.Core.XKeys.D)]
    [Inventec.Desktop.Core.Actions.KeyboardAction("FindDepartment", "HIS.Desktop.Plugins.CashierRoomDepartment.UCCashierRoomDepartment", "FindDepartment", KeyStroke = Inventec.Desktop.Core.XKeys.Control | Inventec.Desktop.Core.XKeys.F)]
    [Inventec.Desktop.Core.Actions.KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.CashierRoomDepartment.UCCashierRoomDepartment", "SaveShortcut", KeyStroke = Inventec.Desktop.Core.XKeys.Control | Inventec.Desktop.Core.XKeys.S)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Inventec.Desktop.Core.Tools.Tool<Inventec.Desktop.Core.IDesktopToolContext>
    {
        public KeyboardWorker() : base() { }

        public override Inventec.Desktop.Core.Actions.IActionSet Actions
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
