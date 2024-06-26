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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using LIS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.LisBacAntibiotic.ExroRoom
{
    class LisBacAntibioticBehavior : Tool<IDesktopToolContext>, ILisBacAntibiotic
    {
        object[] entity;
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module currentModule;
        LIS_ANTIBIOTIC executeRoom;
        LIS.EFMODEL.DataModels.LIS_BACTERIUM executeRoom1;


      

        internal LisBacAntibioticBehavior()
            : base()
        {

        }

        internal LisBacAntibioticBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }
   

        object ILisBacAntibiotic.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is LIS_ANTIBIOTIC)
                        {
                            executeRoom = (LIS_ANTIBIOTIC)item;
                        }
                        if (item is LIS_BACTERIUM)
                        {
                            executeRoom1 = (LIS_BACTERIUM)item;
                        }
                      
                    }
                   
                    if (executeRoom != null)
                    {                   
                        result = new UCLisBacAntibiotic(executeRoom);
                    }
                    else if (executeRoom1 != null)
                    {
                        result = new UCLisBacAntibiotic(executeRoom1);
                    }
                    else
                    {
                        result = new UCLisBacAntibiotic();
                    }
                    
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
