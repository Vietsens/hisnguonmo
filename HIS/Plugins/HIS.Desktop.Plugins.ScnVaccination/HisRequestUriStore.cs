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

namespace HIS.Desktop.Plugins.ScnVaccination
{
    class HisRequestUriStore
    {
        internal const string SCN_VACCINATION_CREATE = "api/ScnVaccination/Create";
        internal const string SCN_VACCINATION_DELETE = "api/ScnVaccination/Delete";
        internal const string SCN_VACCINATION_UPDATE = "api/ScnVaccination/Update";
        internal const string SCN_VACCINATION_GET = "api/ScnVaccination/Get";
        internal const string SCN_VACCINATION_GET_VIEW = "api/ScnVaccination/GetView";
    }
}
