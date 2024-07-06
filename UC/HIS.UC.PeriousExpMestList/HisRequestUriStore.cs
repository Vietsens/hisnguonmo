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

namespace HIS.UC.PeriousExpMestList
{
    class HisRequestUriStore
    {
        public const string HIS_EXP_MEST__GET = "api/HisExpMest/Get";
        public const string HIS_SERVICE_REQ_GET = "api/HisServiceReq/Get";
        public const string HIS_SERVICE_REQ_GETVIEW7 = "api/HisServiceReq/GetView7";
        public const string HIS_SERVICE_REQ_METY__GET = "api/HisServiceReqMety/Get";
        public const string HIS_SERVICE_REQ_MATY__GET = "api/HisServiceReqMaty/Get";
        public const string HIS_SERVICE_REQ_METY__GETVIEW = "api/HisServiceReqMety/GetView";
        public const string HIS_SERVICE_REQ_MATY__GETVIEW = "api/HisServiceReqMaty/GetView";
    }
}