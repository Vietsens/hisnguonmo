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

namespace HIS.Desktop.LocalStorage.ConfigApplication
{
    class SdaConfigAppGet
    {
        internal static List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP> Get()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaConfigAppFilter configAppFilter = new SdaConfigAppFilter();
                configAppFilter.IS_ACTIVE = 1;
                configAppFilter.APP_CODE_ACCEPT = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];
                return new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP>>("/api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
