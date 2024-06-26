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

namespace HIS.Desktop.Plugins.LisSampleAggregation
{
    [KeyboardAction("CreateAggrSample", "HIS.Desktop.Plugins.LisSampleAggregation.Run.UCLisSampleAggregation", "CreateAggrSample", KeyStroke = XKeys.F2)]
    [KeyboardAction("SearchData", "HIS.Desktop.Plugins.LisSampleAggregation.Run.UCLisSampleAggregation", "SearchData", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("Print", "HIS.Desktop.Plugins.LisSampleAggregation.Run.UCLisSampleAggregation", "Print", KeyStroke = XKeys.Control | XKeys.P)]
    [KeyboardAction("SetTrayShortcut", "HIS.Desktop.Plugins.LisSampleAggregation.Run.UCLisSampleAggregation", "SetTrayShortcut", KeyStroke = XKeys.F5)]
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
