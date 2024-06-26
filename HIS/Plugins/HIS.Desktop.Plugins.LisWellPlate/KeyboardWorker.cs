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

namespace HIS.Desktop.Plugins.LisWellPlate
{
    [KeyboardAction("bbtnFindx", "HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate", "bbtnFindx", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("bbtnSavex", "HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate", "bbtnSavex", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("bbtnNewx", "HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate", "bbtnNewx", KeyStroke = XKeys.Control | XKeys.N)]
    [KeyboardAction("bbtnKTDMx", "HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate", "bbtnKTDMx", KeyStroke = XKeys.Control | XKeys.A)]
    [KeyboardAction("bbtnKTGMx", "HIS.Desktop.Plugins.LisWellPlate.Run.UCLisWellPlate", "bbtnKTGMx", KeyStroke = XKeys.Control | XKeys.D)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<DesktopToolContext>
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
