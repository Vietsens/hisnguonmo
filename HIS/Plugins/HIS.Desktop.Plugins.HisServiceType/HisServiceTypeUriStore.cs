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

namespace HIS.Desktop.Plugins.HisServiceType
{
    public class HISRequestUriStore
    {
        internal const string MOSHIS_SERVICE_TYPE_CREATE = "api/HisServiceType/Create";
        internal const string MOSHIS_SERVICE_TYPE_TYPE_DELETE = "api/HisServiceType/Delete";
        internal const string MOSHIS_SERVICE_TYPE_TYPE_UPDATE = "api/HisServiceType/Update";
        internal const string MOSHIS_SERVICE_TYPE_TYPE_GET = "api/HisServiceType/Get";
        internal const string MOSHIS_SERVICE_TYPE_TYPE_CHANGE_LOCK = "api/HisServiceType/ChangeLock";
       
    }
}
