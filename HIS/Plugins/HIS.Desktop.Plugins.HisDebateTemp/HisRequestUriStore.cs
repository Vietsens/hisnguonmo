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

namespace HIS.Desktop.Plugins.HisDebateTemp
{
    class HisRequestUriStore
    {
        internal const string HisDebateTemp_Create = "api/HisDebateTemp/Create";
        internal const string HisDebateTemp_Update = "api/HisDebateTemp/Update";
        internal const string HisDebateTemp_Delete = "api/HisDebateTemp/Delete";
        internal const string HisDebateTemp_Get = "api/HisDebateTemp/Get";
        internal const string HisDebateTemp_GetView = "api/HisDebateTemp/GetView";
        internal const string HisDebateTemp_ChangeLock = "api/HisDebateTemp/ChangeLock";
    }
}
