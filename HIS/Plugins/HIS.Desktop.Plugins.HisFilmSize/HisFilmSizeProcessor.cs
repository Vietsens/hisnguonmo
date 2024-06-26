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
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Core;
using HIS.Desktop.Plugins.HisFilmSize.HisFilmSize;

namespace HIS.Desktop.Plugins.HisFilmSize
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.HisFilmSize",
        "Danh sách gói thầu",
        "Bussiness",
        4,
        "dich-vu.png",
        "D",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)
]
    public class HisFilmSizeProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisFilmSizeProcessor()
        {
            param = new CommonParam();
        }
        public HisFilmSizeProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());

        }
        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                Module moduleData = null;
                if (args != null && args.Count() > 0)
                {
                    for (int i = 0; i < args.Count(); i++)
                    {
                        if (args[i] is Module)
                        {
                            moduleData = (Module)args[i];
                        }
                    }
                    result = new frmHisFilmSize(moduleData);
                }
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
