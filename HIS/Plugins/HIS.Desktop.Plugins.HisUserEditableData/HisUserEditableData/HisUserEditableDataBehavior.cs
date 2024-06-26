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
using MOS.SDO;


namespace HIS.Desktop.Plugins.HisUserEditableData.HisUserEditableData
{
    class HisUserEditableDataBehavior : IHisUserEditableData
    {
        object[] entity;
        internal HisUserEditableDataBehavior(CommonParam param, object[] entity)
            : base()
        {
            this.entity = entity;
        }
        //override
        object IHisUserEditableData.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                //            
                long longData = 0;
                Grantable grant = Grantable.HIS_BID;
                string allowLogginames = "";
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is long)
                        {
                            longData = (long)entity[i];
                        }
                        if (entity[i] is Grantable)
                        {
                            grant = (Grantable)entity[i];
                        }
                        if (entity[i] is string)
                        {
                            allowLogginames = (string)entity[i];
                        }

                    }
                }
                if (longData > 0)
                {
                    result = new frmHisUserEditableData(moduleData, longData, grant, allowLogginames);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("HisUserEditableDataBehavior longData == 0");
                }
            }
            catch (Exception e)
            {
                Inventec.Common.Logging.LogSystem.Warn(e);
            }
            return result;
        }
    }
}
