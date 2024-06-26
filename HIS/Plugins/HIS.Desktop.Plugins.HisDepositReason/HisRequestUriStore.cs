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

namespace HIS.Desktop.Plugins.HisDepositReason
{
    class HisRequestUriStore
    {
        internal const string HisDepositReason_CREATE = "api/HisDepositReason/Create";
        internal const string HisDepositReason_DELETE = "api/HisDepositReason/Delete";
        internal const string HisDepositReason_UPDATE = "api/HisDepositReason/Update";
        internal const string HisDepositReason_GET = "api/HisDepositReason/Get";
        internal const string HisDepositReason_CHANGE_LOCK = "api/HisDepositReason/ChangeLock";
    }
}
