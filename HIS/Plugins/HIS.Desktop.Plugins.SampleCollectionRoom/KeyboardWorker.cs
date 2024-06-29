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

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    [KeyboardAction("SEARCH", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "SEARCH", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("FocusF1", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "FocusF1", KeyStroke = XKeys.F1)]
    [KeyboardAction("FocusF2", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "FocusF2", KeyStroke = XKeys.F2)]
    [KeyboardAction("FocusF3", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "FocusF3", KeyStroke = XKeys.F3)]
    [KeyboardAction("FocusF4", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "FocusF3", KeyStroke = XKeys.F4)]
    [KeyboardAction("PrintBarcode", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "PrintBarcode", KeyStroke = XKeys.Control | XKeys.P)]
    [KeyboardAction("ShotcurtCall", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "ShotcurtCall", KeyStroke = XKeys.F6)]
    [KeyboardAction("ShotcurtReCall", "HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC", "ShotcurtReCall", KeyStroke = XKeys.F7)]

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
