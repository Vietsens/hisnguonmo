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
using Inventec.Core;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.HisRationSumList.HisRationSumList
{
    class HisRationSumListBehavior : BusinessBase, IHisRationSumList
    {
        object[] entity;
        internal HisRationSumListBehavior(CommonParam param, object[] filter)
            : base()
        {
            try
            {
                this.entity = filter;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        object IHisRationSumList.Run()
        {
            Inventec.Desktop.Common.Modules.Module module = null;
            try
            {
                if (this.entity != null && this.entity.Count() > 0 && this.entity.GetType() == typeof(object[]))
                {
                    foreach (var item in this.entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            module = (Inventec.Desktop.Common.Modules.Module)item;
                    }
                    
                }
                if (module != null)
                    return new HIS.Desktop.Plugins.HisRationSumList.HisRationSumList.UcHisRationSumList(module);
                else
                    return new HIS.Desktop.Plugins.HisRationSumList.HisRationSumList.UcHisRationSumList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
           
        }
    }
}
