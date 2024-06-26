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

namespace HIS.Desktop.Plugins.BedAssign.Base
{
    class GlobalStore
    {
        public const string HIS_BED_SERVICE_TYPE_GETVIEW = "api/HisBedServiceType/GetView";
        public const string HIS_BED_BSTY_GETVIEW = "/api/HisBedBsty/GetView";
        public const string HIS_SERVICE_REQ_GETVIEW = "api/HisServiceReq/GetView";
        public const string HIS_SERE_SERV_GETVIEW = "api/HisSereServ/GetView";
        public const string HIS_SERE_SERV_PTTT_GETVIEW = "api/HisSereServPttt/GetView";
        public const string HIS_PTTT_GROUP_GET = "/api/HisPtttGroup/Get";
        public const string HIS_BED_LOG_CREATE = "/api/HisBedLog/Create";
        public const string HIS_BED_LOG_UPDATE = "api/HisBedLog/Update";
        public const string HIS_SERVICE_GETVIEW = "api/HisService/GetView";
        public const string HIS_SERE_SERV_GET = "api/HisSereServ/Get";

        private static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> serviceRoom;
        public static List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM> listServiceRoom
        {
            get
            {
                if (serviceRoom == null || serviceRoom.Count == 0)
                {
                    serviceRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                        <MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(p => p.IS_ACTIVE == 1).ToList();
                }
                return serviceRoom;
            }
            set
            {
                serviceRoom = value;
            }
        }
    }
}
