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
using HIS.UC.Service;

namespace HIS.Desktop.Plugins.ServiceIcd.ServiceIcd
{
    class ServiceIcdBehavior : Tool<IDesktopToolContext>, IServiceIcd
    {
        object[] entity;
        List<long> serviceIds;
        List<long> activeIngrIds;
        Inventec.Desktop.Common.Modules.Module currentModule;
        ServiceADO service;
        HIS_ICD icd;
        List<HIS_ICD> listIcd;
        List<HIS_ACTIVE_INGREDIENT> listActiveIngr;

        internal ServiceIcdBehavior()
            : base()
        {

        }

        internal ServiceIcdBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IServiceIcd.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        if (item is ServiceADO)
                        {
                            service = (ServiceADO)item;
                        }
                        if (item is List<long>)
                        {
                            serviceIds = (List<long>)item;
                        }
                        if (item is HIS_ICD)
                        {
                            icd = (HIS_ICD)item;
                        }
                        if (item is List<HIS_ICD>)
                        {
                            listIcd = (List<HIS_ICD>)item;
                        }
                        if (item is List<HIS_ACTIVE_INGREDIENT>)
                        {
                            listActiveIngr = (List<HIS_ACTIVE_INGREDIENT>)item;
                            activeIngrIds = listActiveIngr != null ? listActiveIngr.Select(o => o.ID).ToList() : null;
                        }
                    }
                    if (service != null)
                    {
                        result = new UCServiceIcd(service,currentModule);
                    }
                    else if (serviceIds != null && icd != null && currentModule != null)
                    {
                        result = new frmServiceIcd(currentModule, icd, serviceIds, activeIngrIds);
                    }
                    else if (serviceIds != null && listIcd != null && currentModule != null)
                    {
                        result = new frmServiceIcd(currentModule, listIcd, serviceIds, activeIngrIds);
                    }
                    else
                    {
                        result = new UCServiceIcd(currentModule);
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
