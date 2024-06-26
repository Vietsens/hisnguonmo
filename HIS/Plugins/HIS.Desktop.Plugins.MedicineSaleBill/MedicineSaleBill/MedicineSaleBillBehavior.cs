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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineSaleBill.MedicineSaleBill
{
    class MedicineSaleBillBehavior : Tool<IDesktopToolContext>, IMedicineSaleBill
    {
        object[] entity;

        internal MedicineSaleBillBehavior()
            : base()
        {

        }

        internal MedicineSaleBillBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IMedicineSaleBill.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long? expMestId = null;
                List<long> expMestIds = null;
                DelegateSelectData delegateSelectData = null;
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
                            expMestId = (long)entity[i];
                        }
                        else if (entity[i] is List<long>)
                        {
                            expMestIds = (List<long>)entity[i];
                        }
                        else if (entity[i] is DelegateSelectData)
                        {
                            delegateSelectData = (DelegateSelectData)entity[i];
                        }
                    }
                }
                if (moduleData != null && expMestId.HasValue)
                {
                    return new frmMedicineSaleBill(moduleData, expMestId.Value, delegateSelectData);
                }
                else if (moduleData != null && expMestIds != null && expMestIds.Count > 0)
                {
                    return new frmMedicineSaleBill(moduleData, expMestIds, delegateSelectData);
                }
                else if (moduleData != null)
                {
                    return new frmMedicineSaleBill(moduleData);
                }
                else
                {
                    return null;
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
