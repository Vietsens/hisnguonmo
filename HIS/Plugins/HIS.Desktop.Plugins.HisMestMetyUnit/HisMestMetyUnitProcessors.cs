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
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.HisMestMetyUnit.HisMestMetyUnit;

namespace HIS.Desktop.Plugins.HisMestMetyUnit
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.HisMestMetyUnit",
        "Thiết lập",
        "Bussiness",
        14,
        "thiet-lap.png",
        "D",
         Module.MODULE_TYPE_ID__FORM,
        true,
        true
        )
    ]
    class HisMestMetyUnitProcessors:ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisMestMetyUnitProcessors()
        {
            param = new CommonParam();
        }
        public HisMestMetyUnitProcessors(CommonParam commonparam)
        {
            param = (commonparam != null ? commonparam : new CommonParam());
        }
        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object Result = null;
            try
            {
                Module Module = null;
                if (args != null && args.Count() > 0)
                {
                    for (int i = 0; i < args.Count(); i++)
                    {
                        if (args[i] is Module)
                        {
                            Module = (Module)args[i];
                        }
                    }
                    Result = new frmHisMestMetyUnit(Module);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                Result = null;
            }
            return Result;
        }
        public override bool IsEnable()
        {
            bool success = false;
            try
            {
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                LogSystem.Error(ex);
            }
            return success;
        }
    }
}
