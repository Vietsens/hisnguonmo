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
namespace HIS.Desktop.Plugins.HisImpMestMediMate.Base
{
    class MediMateRequestUriStore
    {
        /// <summary>Chi tiet nhap thuoc</summary>
        internal const string HIS_IMP_MEST_MEDICINE_GETVIEW = "api/HisImpMestMedicine/GetView";

        /// <summary>Chi tiet nhap vat tu</summary>
        internal const string HIS_IMP_MEST_MATERIAL_GETVIEW = "api/HisImpMestMaterial/GetView";

        /// <summary>Thong tin phieu nhap - lay Ngay hoa don</summary>
        internal const string HIS_IMP_MEST_GETVIEW = "api/HisImpMest/GetView";

        /// <summary>Lo thuoc - lay Nguon nhap</summary>
        internal const string HIS_MEDICINE_GETVIEW = "api/HisMedicine/GetView";

        /// <summary>Lo vat tu - lay Nguon nhap</summary>
        internal const string HIS_MATERIAL_GETVIEW = "api/HisMaterial/GetView";

        /// <summary>Danh muc nguon nhap</summary>
        internal const string HIS_IMP_SOURCE_GET = "api/HisImpSource/Get";
    }
}
