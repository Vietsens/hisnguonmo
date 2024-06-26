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
using TYT.Desktop.Plugins.TYTKhh.TYTKhh;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYT.Desktop.Plugins.TYTKhh
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "TYT.Desktop.Plugins.TYTKhh",
        "Kế hoạch hóa gia đình",
        "Common",
        62,
        "bidList.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class TYTKhhProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public TYTKhhProcessor()
        {
            param = new CommonParam();
        }
        public TYTKhhProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                ITYTKhh behavior = TYTKhhFactory.MakeITYTKhh(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            return false;
        }
    }
}
