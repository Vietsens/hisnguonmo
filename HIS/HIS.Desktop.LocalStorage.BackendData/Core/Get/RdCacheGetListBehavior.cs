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
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Get
{
    class RdCacheGetListBehavior : BusinessBase, IGetDataT
    {
        object entity;
        string uriRDService;
        string uriService;
        Inventec.Common.WebApiClient.ApiConsumer apiConsumer;

        internal RdCacheGetListBehavior(CommonParam param, object filter, string uriRDService, string uriService, Inventec.Common.WebApiClient.ApiConsumer apiConsumer)
            : base(param)
        {
            this.entity = filter;
            this.uriRDService = uriRDService;
            this.uriService = uriService;
            this.apiConsumer = apiConsumer;
        }

        internal RdCacheGetListBehavior(CommonParam param, object filter, string uriRDService)
            : base(param)
        {
            this.entity = filter;
            this.uriRDService = uriRDService;
        }

        object IGetDataT.Execute<T>()
        {
            try
            {
                List<T> rs = new BackendAdapter(param).GetStrong<List<T>>(uriRDService, ApiConsumers.RdCacheConsumer, param, entity, 0, null);
                if (rs == null || rs.Count == 0)
                {
                    rs = new BackendAdapter(param).GetStrong<List<T>>(this.uriService, apiConsumer, param, entity, 0, null);
                }
                return rs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
