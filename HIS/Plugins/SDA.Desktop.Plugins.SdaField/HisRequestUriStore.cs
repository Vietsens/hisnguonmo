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

namespace SDA.Desktop.Plugins.SdaField
{
    class HisRequestUriStore
    {
        internal const string SDAHIS_FIELD_CREATE = "api/sdaModuleField/Create";
        internal const string SDAHIS_FIELD_DELETE = "api/sdaModuleField/Delete";
        internal const string SDAHIS_FIELD_UPDATE = "api/sdaModuleField/Update";
        internal const string SDAHIS_FIELD_GET = "api/sdaModuleField/Get";
        internal const string SDAHIS_FIELD_CHANGE_LOCK = "api/sdaModuleField/ChangeLock";
    }
}
