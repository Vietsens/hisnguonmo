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

namespace HIS.Desktop.Plugins.HisMachineInspection
{
    class HisRequestUriStore
    {
        internal const string URI_CREATE = "api/HisMachineInspection/Create";
        internal const string URI_DELETE = "api/HisMachineInspection/Delete";
        internal const string URI_UPDATE = "api/HisMachineInspection/Update";
        internal const string URI_GET = "api/HisMachineInspection/GetView";
        internal const string URI_CHANGE_LOCK = "api/HisMachineInspection/ChangeLock";
        internal const string URI_GET_MACHINE = "api/HisMachine/Get";
    }
}
