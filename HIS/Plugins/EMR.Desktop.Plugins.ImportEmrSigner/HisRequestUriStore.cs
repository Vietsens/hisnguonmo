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

namespace EMR.Desktop.Plugins.ImportEmrSigner
{
    class HisRequestUriStore
    {
        internal const string EMR_SIGNER_CREATE_LIST = "api/EmrSigner/CreateList";
        internal const string EMR_SIGNER_DELETE = "api/EmrSigner/Delete";
        internal const string EMR_SIGNER_UPDATE = "api/EmrSigner/Update";
        internal const string EMR_SIGNER_GET = "api/EmrSigner/Get";
        internal const string EMR_SIGNER_LOCK = "api/EmrSigner/Lock";
        internal const string EMR_SIGNER_UNLOCK = "api/EmrSigner/UnLock";
    }
}
