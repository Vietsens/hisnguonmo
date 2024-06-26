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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.ConfigButton
{
    class SdaConfigButtonUserGet
    {
        internal static List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON_USER> Get(List<long> moduleButtonIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaConfigAppUserFilter filter = new SdaConfigAppUserFilter();
                filter.IS_ACTIVE = 1;
                filter.LOGINNAME = loginName;
                //if (moduleButtonIds != null && moduleButtonIds.Count > 0)
                //    filter.MODULE_BUTTON_IDs = moduleButtonIds;
                return new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON_USER>>("api/SdaModuleButtonUser/Get", ApiConsumers.SdaConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
