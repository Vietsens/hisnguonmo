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

namespace HIS.Desktop.Plugins.HisSuimIndexUnit
{
    class HisRequestUriStore
    {
        internal const string HIS_SUIM_INDEX_UNIT_CREATE = "api/HisSuimIndexUnit/Create";
        internal const string HIS_SUIM_INDEX_UNIT_DELETE = "api/HisSuimIndexUnit/Delete";
        internal const string HIS_SUIM_INDEX_UNIT_UPDATE = "api/HisSuimIndexUnit/Update";
        internal const string HIS_SUIM_INDEX_UNIT_GET = "api/HisSuimIndexUnit/Get";
        internal const string HIS_SUIM_INDEX_UNIT_CHANGE_LOCK = "api/HisSuimIndexUnit/ChangeLock";
    }
}
