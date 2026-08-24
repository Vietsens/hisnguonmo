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
    /// Mot vai tro kip mo, lay tu danh muc HIS_EXECUTE_ROLE (KHONG hardcode ma vai tro tren mau in).
    /// Moi vai tro gom san ten tat ca thanh vien dam nhan vai tro do trong kip.
    /// </summary>
    public class Mps000324EkipRoleADO
    {
        /// <summary>= HIS_EXECUTE_ROLE.ID</summary>
        public long EXECUTE_ROLE_ID { get; set; }

        /// <summary>Ma vai tro trong danh muc (01, 02, 03...)</summary>
        public string EXECUTE_ROLE_CODE { get; set; }

        /// <summary>Ten vai tro hien thi — lay tu danh muc, khong viet cung tren mau</summary>
        public string EXECUTE_ROLE_NAME { get; set; }

        /// <summary>So thu tu hien thi, dem tu 1 theo thu tu ma vai tro</summary>
        public int NUM_ORDER { get; set; }

        /// <summary>So thanh vien dam nhan vai tro nay. 0 = vai tro khong co nguoi</summary>
        public int USER_COUNT { get; set; }

        /// <summary>Ten tat ca thanh vien, ngan cach bang ", ". Rong khi khong co nguoi</summary>
        public string USERNAMES { get; set; }

        /// <summary>Tai khoan tat ca thanh vien, ngan cach bang ", "</summary>
        public string LOGINNAMES { get; set; }

        /// <summary>1: vai tro phau thuat vien chinh</summary>
        public short IS_SURG_MAIN { get; set; }
    }
}
