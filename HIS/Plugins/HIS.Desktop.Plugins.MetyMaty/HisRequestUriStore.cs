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

namespace HIS.Desktop.Plugins.MetyMaty
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_METY_PRODUCT_DELETE = "api/HisMetyProduct/Delete";
        internal const string MOSHIS_METY_PRODUCT_UPDATE= "api/HisMetyProduct/Update";
        internal const string MOSHIS_METY_PRODUCT_CREATE = "api/HisMetyProduct/Create";
        internal const string MOSHIS_METY_PRODUCT_GET_VIEW = "api/HisMetyProduct/GetView";
        internal const string MOSHIS_MATERIAL_TYPE_GET = "api/HisMaterialType/Get";
        internal const string MOSHIS_MEDICINE_TYPE_GET = "api/HisMedicineType/Get";

        internal const string MOSHIS_METY_MATY_GET = "api/HisMetyMaty/Get";
        internal const string MOSHIS_METY_MATY_UPDATELIST = "api/HisMetyMaty/UpdateList";
        internal const string MOSHIS_METY_MATY_DELETELIST = "api/HisMetyMaty/DeleteList";
        internal const string MOSHIS_METY_MATY_CREATELIST = "api/HisMetyMaty/CreateList";

        internal const string MOSHIS_METY_METY_GET = "api/HisMetyMety/Get";
        internal const string MOSHIS_METY_METY_DELETELIST = "api/HisMetyMety/DeleteList";
        internal const string MOSHIS_METY_METY_UPDATELIST = "api/HisMetyMety/UpdateList";
        internal const string MOSHIS_METY_METY_CREATELIST = "api/HisMetyMety/CreateList";
    }
}
