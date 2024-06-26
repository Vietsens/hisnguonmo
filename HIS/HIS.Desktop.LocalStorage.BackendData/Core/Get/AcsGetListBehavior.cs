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
//using Sdc.Client;

namespace HIS.Desktop.LocalStorage.BackendData.Get
{
    class AcsGetListBehavior : BusinessBase, IGetDataT
    {
        object entity;
        string uri;
        internal AcsGetListBehavior(CommonParam param, object filter, string uriParam)
            : base(param)
        {
            this.entity = filter;
            this.uri = uriParam;
        }

        object IGetDataT.Execute<T>()
        {
            try
            {
                //object t = Activator.CreateInstance(typeof(T));
                //if (t is ACS.EFMODEL.DataModels.ACS_USER)
                //{
                //    List<ACS.EFMODEL.DataModels.ACS_USER> data = null;

                //    var server = new Sdc.Client.GetSync(ProcessGet);
                //    if (server.Connect())
                //    {
                //        server.Send(HIS.Service.LocalStorage.AdapterLib.DataKeyConstan.DataConfigKey__ACS_USER);
                //    }

                //    data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ACS.EFMODEL.DataModels.ACS_USER>>(new SDC.WCFClient.CacheLocalClient.CacheLocalClientManager().Get(HIS.Service.LocalStorage.AdapterLib.DataKeyConstan.DataConfigKey__ACS_USER));
                //    if (data == null || data.Count == 0)
                //    {
                //        data = new BackendAdapter(param).Get<List<T>>(uri, ApiConsumers.AcsConsumer, entity, param) as List<ACS.EFMODEL.DataModels.ACS_USER>;
                //        new SDC.WCFClient.CacheLocalClient.CacheLocalClientManager().Set(HIS.Service.LocalStorage.AdapterLib.DataKeyConstan.DataConfigKey__ACS_USER, data);
                //    }
                //    return data;
                //}

                return new BackendAdapter(param).GetStrong<List<T>>(uri, ApiConsumers.AcsConsumer, param, entity, 0, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        //void ProcessGet(WSResponse data)
        //{

        //}
    }
}
