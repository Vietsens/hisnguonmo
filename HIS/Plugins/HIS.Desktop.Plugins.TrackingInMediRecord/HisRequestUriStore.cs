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

namespace HIS.Desktop.Plugins.TrackingInMediRecord
{
    public class HisRequestUriStore
    {
        internal const string HIS_TREATMENT_GET = "api/HisTreatment/Get";
        internal const string HIS_TRACKING_GET = "api/HisTracking/Get";
        internal const string HIS_DATA_STORE_GET = "api/HisDataStore/Get";
        public const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        public const string HIS_EXP_MEST_GET = "api/HisExpMest/Get";
        public const string HIS_SERE_SERV_GET = "/api/HisSereServ/Get";
        public const string HIS_CARE_GET = "api/HisCare/Get";
        public const string HIS_DHST_GET = "api/HisDhst/Get";
        public const string HIS_CARE_DETAIL_GETVIEW = "api/HisCareDetail/GetView";
    }
}
