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

namespace HIS.Desktop.Plugins.AllocateLostExpMestCreate
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_MEDICINE_GET_IN_STOCK_MEDICINE = "/api/HisMedicine/GetInStockMedicine";
        internal const string MOSHIS_MATERIAL_GET_IN_STOCK_MATERIAL = "/api/HisMaterial/GetInStockMaterial";
        internal const string MOSHIS_LOST_EXP_MEST_CREATE = "/api/HisLostExpMest/Create";
        internal const string MOSHIS_LOST_EXP_MEST_GETVIEW = "/api/HisLostExpMest/GetView";

        public const string PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_MAT_MAT__MPS000168 = "Mps000168";
        public const string PRINT_TYPE_CODE__BIEUMAU__HOI_DONG_PHIEU_XUAT_MAT_MAT__MPS000205 = "Mps000205";
    }
}
