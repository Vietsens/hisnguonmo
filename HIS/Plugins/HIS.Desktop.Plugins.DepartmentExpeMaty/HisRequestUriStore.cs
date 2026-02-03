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

namespace HIS.Desktop.Plugins.DepartmentExpeMaty
{
    class HisRequestUriStore
    {
        internal const string CustomerSource_GET = "api/HisDepartmentExpeMaty/Get";
        internal const string CustomerSource_GETVIEW = "api/HisDepartmentExpeMaty/GetView";
        internal const string CustomerSource_DELETE = "api/HisDepartmentExpeMaty/Delete";
        internal const string CustomerSource_CHANGELOCK = "api/HisDepartmentExpeMaty/ChangeLock";
        internal const string CustomerSource_Create = "api/HisDepartmentExpeMaty/Create";
        internal const string CustomerSource_UPDATE = "api/HisDepartmentExpeMaty/Update";
    }
}
