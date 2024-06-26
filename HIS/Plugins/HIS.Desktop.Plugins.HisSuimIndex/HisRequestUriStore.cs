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

namespace HIS.Desktop.Plugins.HisSuimIndex
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_SUIM_INDEX_CREATE = "api/HisSuimIndex/Create";
        internal const string MOSHIS_SUIM_INDEX_DELETE = "api/HisSuimIndex/Delete";
        internal const string MOSHIS_SUIM_INDEX_UPDATE = "api/HisSuimIndex/Update";
        internal const string MOSHIS_SUIM_INDEX_GET = "api/HisSuimIndex/Get";
        internal const string MOSHIS_SUIM_INDEX_GETVIEW = "api/HisSuimIndex/GetView";
        internal const string MOSHIS_SUIM_INDEX_CHANGELOCK = "api/HisSuimIndex/ChangeLock";
    }
}
