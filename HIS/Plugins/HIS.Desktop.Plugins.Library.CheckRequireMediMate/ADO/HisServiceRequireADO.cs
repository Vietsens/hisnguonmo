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

namespace HIS.Desktop.Plugins.Library.CheckRequireMediMate.ADO
{
    /// <summary>
    /// Ban rut gon cua HIS_SERVICE, chi giu cac cot can cho viec kiem tra.
    /// Dung lam kieu nhan JSON tu api/HisService/Get de doc duoc cot moi IS_REQUIRE_MEDI_MATE
    /// ma KHONG phu thuoc DLL MOS.EFMODEL moi (cac cot khac trong JSON bi bo qua khi deserialize).
    /// Ten property phai TRUNG TUYET DOI ten cot backend.
    /// </summary>
    public class HisServiceRequireADO
    {
        public long ID { get; set; }

        public string SERVICE_CODE { get; set; }

        public string SERVICE_NAME { get; set; }

        /// <summary>Co "Co thuoc, vat tu di kem": 1 = co; NULL/0 = khong kiem tra (viec 3353)</summary>
        public short? IS_REQUIRE_MEDI_MATE { get; set; }
    }
}
