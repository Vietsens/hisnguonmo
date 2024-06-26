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

namespace HIS.Desktop.Plugins.HisProcessing
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_PROCESSING_METHOD_CREATE = "api/HisProcessingMethod/Create";
        internal const string MOSHIS_PROCESSING_METHOD_DELETE = "api/HisProcessingMethod/Delete";
        internal const string MOSHIS_PROCESSING_METHOD_UPDATE = "api/HisProcessingMethod/Update";
        internal const string MOSHIS_PROCESSING_METHOD_GET = "api/HisProcessingMethod/Get";
        internal const string MOSHIS_PROCESSING_METHOD_CHANGE_LOCK = "api/HisProcessingMethod/ChangeLock";
    }
}
