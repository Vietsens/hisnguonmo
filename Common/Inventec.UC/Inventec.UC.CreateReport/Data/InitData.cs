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
using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Data
{
    public class InitData
    {
        internal Inventec.Token.ClientSystem.ClientTokenManager clientToken;
        internal Inventec.Common.WebApiClient.ApiConsumer sarconsumer;
        internal Inventec.Common.WebApiClient.ApiConsumer mrsconsumer;

        public InitData(Inventec.Common.WebApiClient.ApiConsumer sarConsumer,SAR.EFMODEL.DataModels.SAR_REPORT_TYPE sarreporttype,Inventec.Common.WebApiClient.ApiConsumer mrsConsumer, Inventec.Token.ClientSystem.ClientTokenManager clientTokenManager)
        {
            try
            {
                this.sarconsumer = sarConsumer;
                this.mrsconsumer = mrsConsumer;
                this.clientToken = clientTokenManager;
                GlobalStore.reportType = sarreporttype;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
