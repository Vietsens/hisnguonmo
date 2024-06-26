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

namespace HIS.Desktop.Plugins.SamplePathologyReq
{
    [KeyboardAction("bbtnSearch_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "bbtnSearch_ItemClick", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("bbtnF2_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "bbtnF2_ItemClick", KeyStroke =  XKeys.F2)]
    [KeyboardAction("bbtnF3_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "bbtnF3_ItemClick", KeyStroke = XKeys.F3)]
    [KeyboardAction("bbtnCall_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "bbtnCall_ItemClick", KeyStroke = XKeys.F6)]
    [KeyboardAction("bbtnCallBack_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "bbtnCallBack_ItemClick", KeyStroke =  XKeys.F7)]
    [KeyboardAction("barSave_ItemClick", "HIS.Desktop.Plugins.BaseCompensationCreate.UCBaseCompensationCreate", "barSave_ItemClick", KeyStroke = XKeys.Control | XKeys.S)]
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
