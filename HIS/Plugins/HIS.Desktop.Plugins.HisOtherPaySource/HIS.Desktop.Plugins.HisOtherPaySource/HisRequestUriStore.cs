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

namespace HIS.Desktop.Plugins.HisOtherPaySource
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_OTHER_PAY_SOURCE_CREATE = "api/HisOtherPaySource/Create";
        internal const string MOSHIS_OTHER_PAY_SOURCE_DELETE = "api/HisOtherPaySource/Delete";
        internal const string MOSHIS_OTHER_PAY_SOURCE_UPDATE = "api/HisOtherPaySource/Update";
        internal const string MOSHIS_CASHIER_ROOM_GET = "api/HisOtherPaySource/Get";
        internal const string MOSHIS_CASHIER_ROOM_GETVIEW = "api/HisOtherPaySource/GetView";
        internal const string MOSHIS_CASHIER_ROOM_CHANGELOCK = "api/HisOtherPaySource/ChangeLock";
    }
}
