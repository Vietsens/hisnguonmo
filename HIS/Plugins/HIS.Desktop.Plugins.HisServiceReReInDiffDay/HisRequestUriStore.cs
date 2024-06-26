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

namespace HIS.Desktop.Plugins.HisServiceReReInDiffDay
{
    class HisRequestUriStore
    {
        internal const string HIS_SERVICE_RERE_TIME_CREATE = "/api/HisServiceRereTime/Create";
        internal const string HIS_SERVICE_RERE_TIME_DELETE = "/api/HisServiceRereTime/Delete";
        internal const string HIS_SERVICE_RERE_TIME_UPDATE = "/api/HisServiceRereTime/Update";
        internal const string HIS_SERVICE_RERE_TIME_GET = "/api/HisServiceRereTime/GetView";
        internal const string HIS_SERVICE_RERE_TIME_CHANGE_LOCK = "/api/HisServiceRereTime/ChangeLock";
    }
}
