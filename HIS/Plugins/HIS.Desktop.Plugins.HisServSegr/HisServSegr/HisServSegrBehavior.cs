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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServSegr.HisServSegr
{
    class HisServSegrBehavior : Tool<IDesktopToolContext>, IHisServSegr
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal HisServSegrBehavior()
            : base()
        {

        }

        internal HisServSegrBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IHisServSegr.Run()
        {
            object result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            long moduleType = 0;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            moduleType = (long)item;
                        }
                    }
                    if (moduleType > 0)
                    {
                        if (moduleType == Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC)
                        {
                            result = new ucServSegr(moduleData);
                        }
                        else //if (moduleType == Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM)
                            result = new frmServSegr(moduleData);
                    }
                    else
                        result = new ucServSegr(moduleData);
                }
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
