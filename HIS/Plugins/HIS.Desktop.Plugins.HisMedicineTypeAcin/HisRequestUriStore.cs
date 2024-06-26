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

namespace HIS.Desktop.Plugins.HisMedicineTypeAcin
{
    public class HisRequestUriStore
    {
        internal const string HIS_SERV_SEGR_GET = "/api/HisServSegr/Get";
        internal const string HIS_SERV_SEGR_DELETE_LIST = "/api/HisServSegr/DeleteList";
        internal const string HIS_SERV_SEGR_CREATE_LIST = "/api/HisServSegr/CreateList";
        internal const string HIS_SERVICE_GROUP_CREATE = "/api/HisServiceGroup/Create";
        internal const string HIS_SERVICE_GROUP_UPDATE = "/api/HisServiceGroup/Update";
        internal const string HIS_SERVICE_GROUP_DELETE = "/api/HisServiceGroup/Delete";
        internal const string HIS_SERVICE_GROUP_CHANGE_LOCK = "/api/HisServiceGroup/ChangeLock";
        internal const string HIS_ACTIVE_INGREDIENT_DELETE = "/api/HisActiveIngredient/Delete";
        internal const string MOSHIS_ACTIVE_INGREDIENT_CHANGE_LOCK = "/api/HisActiveIngredient/ChangeLock";

    }
}
