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

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.Base
{
    class RequestUriStore
    {
        public const string HIS_MEDICINE_BEAN__RELEASEBEANALL = "api/HisMedicineBean/ReleaseAll";
        public const string HIS_MEDICINE_BEAN__TAKEBEANLIST = "api/HisMedicineBean/TakeList";
        public const string HIS_MEDICINE_BEAN__RELEASE = "api/HisMedicineBean/Release";
        
        public const string HIS_MATERIAL_BEAN__RELEASE = "api/HisMaterialBean/Release";
        public const string HIS_MATERIAL_BEAN__TAKEBEANLIST = "api/HisMaterialBean/TakeList";
        public const string HIS_MATERIAL_BEAN__RELEASEBEANALL = "api/HisMaterialBean/ReleaseAll";

        public const string HIS_EXP_MEST__SALE_CREATE = "api/HisExpMest/SaleCreate";
        public const string HIS_EXP_MEST__SALE_UPDATE = "api/HisExpMest/SaleUpdate";

        public const string HIS_MEDICINE_BEAN__GET = "api/HisMedicineBean/Get";
        public const string HIS_MATERIAL_BEAN__GET = "api/HisMaterialBean/Get";

        public const string HIS_EXP_MEST__SALE_CREATE_LIST = "api/HisExpMest/SaleCreateList";
        public const string HIS_EXP_MEST__SALE_UPDATE_LIST = "api/HisExpMest/SaleUpdateList";
    }
}
