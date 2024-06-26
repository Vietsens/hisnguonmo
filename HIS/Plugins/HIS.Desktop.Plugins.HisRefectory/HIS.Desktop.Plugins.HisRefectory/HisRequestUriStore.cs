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

namespace HIS.Desktop.Plugins.HisRefectory
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_REFECTORY_CREATE = "api/HisRefectory/Create";
        internal const string MOSHIS_REFECTORY_DELETE = "api/HisRefectory/Delete";
        internal const string MOSHIS_REFECTORY_UPDATE = "api/HisRefectory/Update";
        internal const string MOSHIS_REFECTORY_GET = "api/HisRefectory/Get";
        internal const string MOSHIS_REFECTORY_GETVIEW = "api/HisRefectory/GetView";
        internal const string MOSHIS_REFECTORY_CHANGELOCK = "api/HisRefectory/ChangeLock";
    }
}
