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
using HIS.Desktop.LocalStorage.BackendData.Core;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Base
{
    class DATA
    {
        static List<HIS_ROOM_TYPE_MODULE> roomTypeModules;
        public static List<HIS_ROOM_TYPE_MODULE> RoomTypeModules
        {
            get
            {
                if (roomTypeModules == null)
                {
                    CommonParam param = new CommonParam();
                    HisRoomTypeModuleFilter roomTypeModuleFilter = new HisRoomTypeModuleFilter();
                    roomTypeModuleFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    roomTypeModules = new BackendAdapter(param).Get<List<HIS_ROOM_TYPE_MODULE>>(HisRequestUriStore.HIS_ROOM_TYPE_MODULE__GET, ApiConsumers.MosConsumer, roomTypeModuleFilter, param) ?? new List<HIS_ROOM_TYPE_MODULE>();

                    TranslateWorker.TranslateData<HIS_ROOM_TYPE_MODULE>(roomTypeModules);
                }
                return roomTypeModules;
            }
            set
            {
                roomTypeModules = value;
            }
        }

        public static List<ACS.EFMODEL.DataModels.ACS_MODULE_GROUP> ModuleGroups { get; set; }

        public static string OsInfo { get; set; }
    }
}
