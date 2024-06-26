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

namespace HIS.Desktop.Plugins.HisMedicineUseForm
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_MEDICINE_USE_FORM_CREATE = "api/HisMedicineUseForm/Create";
        internal const string MOSHIS_MEDICINE_USE_FORM_DELETE = "api/HisMedicineUseForm/Delete";
        internal const string MOSHIS_MEDICINE_USE_FORM_UPDATE = "api/HisMedicineUseForm/Update";
        internal const string MOSHIS_MEDICINE_USE_FORM_GET = "api/HisMedicineUseForm/Get";
        internal const string MOSHIS_MEDICINE_USE_FORM_GROUP_CHANGE_LOCK = "api/HisMedicineUseForm/ChangeLock";
    }
}
