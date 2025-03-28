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
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using MOS.EFMODEL.DataModels;
namespace HIS.Desktop.Plugins.DrugInterventionInfo
{
    class DrugInterventionInfoBehavior : BusinessBase, IDrugInterventionInfo
    {
         object[] entity;
         internal DrugInterventionInfoBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

         object IDrugInterventionInfo.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long  id = 0;
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is long)
                            {
                                id = (long)entity[i];
                            }
                        }
                    }
                }

                return new frmDrugInterventionInfo(moduleData, id);
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
