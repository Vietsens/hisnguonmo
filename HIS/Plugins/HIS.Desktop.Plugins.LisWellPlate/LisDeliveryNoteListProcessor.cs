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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;

namespace HIS.Desktop.Plugins.LisWellPlate
{
    class LisWellPlateProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.LisWellPlate",
           "Giếng bệnh phẩm",
           "",
           0,
           "",
           "",
           Module.MODULE_TYPE_ID__UC,
           true,
           true)
        ]
        public class LisWellPlateQProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public LisWellPlateQProcessor()
            {
                param = new CommonParam();
            }
            public LisWellPlateQProcessor(CommonParam paramBusiness)
            {
                param = (paramBusiness != null ? paramBusiness : new CommonParam());
            }

            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    LisWellPlate.ILisWellPlate behavior = LisWellPlate.LisWellPlateFactory.MakeILisWellPlate(param, args);
                    result = behavior != null ? (object)(behavior.Run()) : null;
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
                bool result = false;
                try
                {
                    result = true;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    result = false;
                }
                return result;
            }
        }
    }
}
