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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ReportAll
{
    public static class AcsUserSaleGet
    {
        const int MAX_REQUEST_LENGTH_PARAM = 20;
        public static void Get()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisMediStockFilter HisMediStockfilter = new HisMediStockFilter();
                HisMediStockfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                HisMediStockfilter.IS_BUSINESS = true;
                var cashierRoom = new BackendAdapter(paramCommon).Get<List<HIS_MEDI_STOCK>>(
                    HisRequestUriStore.HIS_MEDI_STOCK_GET, ApiConsumer.ApiConsumers.MosConsumer, HisMediStockfilter, paramCommon);

                HisUserRoomViewFilter UserRoomfilter = new HisUserRoomViewFilter();
                UserRoomfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                UserRoomfilter.ROOM_IDs = cashierRoom.Select(o => o.ROOM_ID).ToList();
                var userRoom = new BackendAdapter(paramCommon).Get<List<V_HIS_USER_ROOM>>(
                    HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, UserRoomfilter, paramCommon);

                paramCommon = new CommonParam();

                var skip = 0;
                var loginnames = userRoom.Select(o => o.LOGINNAME).Distinct().ToList();
                //Inventec.Common.Logging.LogSystem.Info("userRoom:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => userRoom), userRoom));
                List<ACS_USER> acsUserSale = new List<ACS_USER>();
                while (loginnames.Count - skip > 0)
                {
                    var lists = loginnames.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + MAX_REQUEST_LENGTH_PARAM;
                    AcsUserFilter AcsUserfilter = new AcsUserFilter();
                    AcsUserfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    AcsUserfilter.LOGINNAMEs = lists;
                    acsUserSale.AddRange(new BackendAdapter(paramCommon).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>(AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, AcsUserfilter, paramCommon));
                }
                HIS.UC.FormType.Config.AcsFormTypeConfig.HisAcsUserSale = acsUserSale;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
