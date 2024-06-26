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

namespace HIS.Desktop.Plugins.HisWorkingShift
{
    class HisRequestUriStore
    {
        internal const string WorkingShift_GET = "api/HisWorkingShift/Get";
        internal const string WorkingShift_GETVIEW = "api/HisWorkingShift/GetView";
        internal const string WorkingShift_DELETE = "api/HisWorkingShift/Delete";
        internal const string WorkingShift_CHANGELOCK = "api/HisWorkingShift/ChangeLock";
        internal const string WorkingShift_Create = "api/HisWorkingShift/Create";
        internal const string WorkingShift_UPDATE = "api/HisWorkingShift/Update";
    }
}
