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

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    [KeyboardAction("BtnAddShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnAddShortcut", KeyStroke = XKeys.Control | XKeys.A)]
    [KeyboardAction("BtnAddShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "FocusMediMateSearch", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("BtnSaveShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaveShortcut", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("BtnCancelShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnCancelShortcut", KeyStroke = XKeys.Control | XKeys.H)]
    [KeyboardAction("BtnSaveShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaveShortcut", KeyStroke = XKeys.F5)]
    [KeyboardAction("BtnSaveWithPrintShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaveWithPrintShortcut", KeyStroke = XKeys.Control | XKeys.I)]
    [KeyboardAction("BtnSaveWithPrintShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaveWithPrintShortcut", KeyStroke = XKeys.F9)]
    [KeyboardAction("BtnNewShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnNewShortcut", KeyStroke = XKeys.Control | XKeys.N)]
    [KeyboardAction("BtnNewExpMestShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnNewExpMestShortcut", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("BtnNewExpMestShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnNewExpMestShortcut", KeyStroke = XKeys.F7)]
    [KeyboardAction("BtnNewShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnNewShortcut", KeyStroke = XKeys.F8)]
    [KeyboardAction("BtnSaleBillShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaleBillShortcut", KeyStroke = XKeys.Control | XKeys.T)]
    [KeyboardAction("BtnSaleBillShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnSaleBillShortcut", KeyStroke = XKeys.F10)]
    [KeyboardAction("FocusPresCodeShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "FocusPresCodeShortcut", KeyStroke = XKeys.F2)]
    [KeyboardAction("BtnPresCodeShortcut", "HIS.Desktop.Plugins.ExpMestSaleCreate.UCExpMestSaleCreate", "BtnPresCodeShortcut", KeyStroke = XKeys.F3)]
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
