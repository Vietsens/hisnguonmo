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
    class MosGetListAsyncBehavior : BusinessBase, IGetDataTAsync
    {
        object entity;
        string uri;
        internal MosGetListAsyncBehavior(CommonParam param, object filter, string uriParam)
            : base(param)
        {
            this.entity = filter;
            this.uri = uriParam;
        }

        async Task<object> IGetDataTAsync.ExecuteAsync<T>()
        {
            try
            {
                return await new BackendAdapter(param).GetAsync<List<T>>(uri, ApiConsumers.MosConsumer, entity, param).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
