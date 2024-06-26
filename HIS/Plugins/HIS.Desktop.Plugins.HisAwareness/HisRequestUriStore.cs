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

namespace HIS.Desktop.Plugins.HisAwareness
{
    class HisRequestUriStore
    {
        internal const string HIS_AWARENESS_CREATE = "api/HisAwareness/Create";
        internal const string HIS_AWARENESS_DELETE = "api/HisAwareness/Delete";
        internal const string HIS_AWARENESS_UPDATE = "api/HisAwareness/Update";
        internal const string HIS_AWARENESS_GET = "api/HisAwareness/Get";
        internal const string HIS_AWARENESS_CHANGE_LOCK = "api/HisAwareness/ChangeLock";
    }
}
