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

namespace MPS.Processor.Mps000324.PDO
{
    /// <summary>
    /// Nhom dich vu trong phieu thanh toan PT-TT (Khac / Thuoc, dich truyen / Vat tu y te...).
    /// Gom theo HIS_SERVICE_TYPE, kem so La Ma va tong tien cua nhom.
    /// Doi tuong nay chi bo sung them, KHONG thay the "ServiceTypes" dang dung o mau cu.
    /// </summary>
    public class Mps000324GroupADO
    {
        /// <summary>= HIS_SERVICE_TYPE.ID. Noi voi Mps000324ItemADO.GROUP_ID</summary>
        public long ID { get; set; }

        /// <summary>So thu tu nhom, dem tu 1 theo thu tu xuat hien tren phieu</summary>
        public int NUM_ORDER { get; set; }

        /// <summary>So La Ma sinh tu NUM_ORDER: I, II, III...</summary>
        public string NUM_ORDER_ROMAN { get; set; }

        /// <summary>So La Ma sinh tu HIS_SERVICE_TYPE.ID (Khac=12 -> XII, Thuoc=6 -> VI)</summary>
        public string SERVICE_TYPE_ROMAN { get; set; }

        /// <summary>Ma loai dich vu</summary>
        public string SERVICE_TYPE_CODE { get; set; }

        /// <summary>Ten nhom hien thi tren phieu</summary>
        public string SERVICE_TYPE_NAME { get; set; }

        /// <summary>So dong chi tiet thuoc nhom</summary>
        public int ITEM_COUNT { get; set; }

        /// <summary>Tong tien cua nhom. Null khi ca nhom khong co dong nao co gia</summary>
        public decimal? TOTAL_AMOUNT { get; set; }
    }
}
