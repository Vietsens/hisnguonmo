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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti
{
    class SereServGet
    {
        static List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> sereservs;
        internal static List<MOS.EFMODEL.DataModels.HIS_SERE_SERV> GetByServiceReqId(long serviceReqId)
        {
            try
            {
                if (sereservs == null || sereservs.Count == 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                    sereServFilter.SERVICE_REQ_ID = serviceReqId;
                    sereservs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereservs;
        }
    }
}
