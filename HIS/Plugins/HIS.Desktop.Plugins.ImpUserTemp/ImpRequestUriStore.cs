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

namespace HIS.Desktop.Plugins.ImpUserTemp
{
    class ImpRequestUriStore
    {
        internal const string IMP_USER_TEMP_CREATE = "/api/HisImpUserTemp/Create";
        internal const string IMP_USER_TEMP_DELETE = "/api/HisImpUserTemp/Delete";
        internal const string IMP_USER_TEMP_UPDATE = "/api/HisImpUserTemp/Update";
        internal const string IMP_USER_TEMP_GET = "/api/HisImpUserTemp/Get";

        internal const string IMP_USER_TEMP_DT_DELETE = "/api/HisImpUserTempDt/Delete";
        internal const string IMP_USER_TEMP_DT_UPDATE = "/api/HisImpUserTempDt/Update";
        internal const string IMP_USER_TEMP_DT_GET = "/api/HisImpUserTempDt/Get";
        internal const string IMP_USER_TEMP_DT_CREATE = "/api/HisImpUserTempDt/Create";
        internal const string IMP_USER_TEMP_DT_GETVIEW = "/api/HisImpUserTempDt/GetView";
    }
}
