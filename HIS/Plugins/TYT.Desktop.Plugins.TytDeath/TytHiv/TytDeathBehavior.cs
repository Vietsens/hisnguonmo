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

namespace TYT.Desktop.Plugins.TytDeath.TytDeath
{
    class TytDeathBehavior : Tool<IDesktopToolContext>, ITytDeath
    {
        object[] entity;
        internal TytDeathBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITytDeath.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                V_HIS_PATIENT patient = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is V_HIS_PATIENT)
                        {
                            patient = (V_HIS_PATIENT)entity[i];
                        }
                    }
                }
                if (moduleData != null && patient == null)
                {
                    return new UC_TytDeath(moduleData);
                }
                else if (moduleData != null && patient != null)
                {
                    return new UC_TytDeath(moduleData, patient);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}