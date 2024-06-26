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

namespace HIS.Desktop.Plugins.HisStation
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_STATION_CREATE = "api/HisStation/Create";
        internal const string MOSHIS_STATION_DELETE = "api/HisStation/Delete";
        internal const string MOSHIS_STATION_UPDATE = "api/HisStation/Update";
        internal const string MOSHIS_STATION_GET = "api/HisStation/Get";
        internal const string MOSHIS_STATION_GETVIEW = "api/HisStation/GetView";
        internal const string MOSHIS_STATION_CHANGELOCK = "api/HisStation/ChangeLock";
    }
}
