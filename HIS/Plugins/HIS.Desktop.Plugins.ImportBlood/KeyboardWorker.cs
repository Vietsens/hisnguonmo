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

namespace HIS.Desktop.Plugins.ImportBlood
{
    [KeyboardAction("BtnAdd", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnAdd", KeyStroke = XKeys.Control | XKeys.A)]
    [KeyboardAction("BtnSave", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnSave", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("BtnSaveDraft", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnSaveDraft", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("BtnNew", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnNew", KeyStroke = XKeys.Control | XKeys.N)]
    [KeyboardAction("BtnPrint", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnPrint", KeyStroke = XKeys.Control | XKeys.P)]
    [KeyboardAction("BtnUpdate", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnUpdate", KeyStroke = XKeys.Control | XKeys.U)]
    [KeyboardAction("BtnCancel", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnCancel", KeyStroke = XKeys.Control | XKeys.Q)]
    [KeyboardAction("BtnBlood", "HIS.Desktop.Plugins.ImportBlood.UCImportBloodPlus", "BtnBlood", KeyStroke = XKeys.Control | XKeys.I)]
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
