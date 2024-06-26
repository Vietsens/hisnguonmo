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
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MatyMaty
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
     "HIS.Desktop.Plugins.MatyMaty",
     "Danh mục",
     "Bussiness",
     4,
     "showproduct_32x32.png",
     "A",
     Module.MODULE_TYPE_ID__FORM,
     true,
     true)
  ]
    public class MatyMatyProcessor : ModuleBase, IDesktopRoot
    {

        CommonParam param;

        public MatyMatyProcessor()
        {
            param = new CommonParam();
        }
        public MatyMatyProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                List<HIS_MATY_MATY> LisMatyMaty = null;
                long materialTypeId = 0;
                if (args.GetType() == typeof(object[]))
                {
                    if (args != null && args.Count() > 0)
                    {
                        for (int i = 0; i < args.Count(); i++)
                        {
                            if (args[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)args[i];
                            }
                            else if (args[i] is List<HIS_MATY_MATY>)
                            {
                                LisMatyMaty = (List<HIS_MATY_MATY>)args[i];
                            }
                            else if (args[i] is long)
                            {
                                materialTypeId = (long)args[i];
                            }
                        }
                    }
                }

                if (materialTypeId > 0)
                    result = new frmMatyMaty(moduleData, materialTypeId, LisMatyMaty);
                else
                    result = new frmMatyMaty(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
