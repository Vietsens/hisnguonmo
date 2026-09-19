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

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO
{
    /// <summary>Nguon cua dich vu doi ung trong cap vi pham</summary>
    public enum ViolationSource
    {
        /// <summary>Dich vu da duoc chi dinh truoc do trong lan dieu tri</summary>
        Assigned = 1,

        /// <summary>Dich vu dang duoc tich chon cung luc tren luoi</summary>
        Assigning = 2
    }

    /// <summary>
    /// 1 dong hien thi tren form canh bao/chan: cap dich vu vi pham quy tac loai tru.
    /// </summary>
    public class ServiceExclusiveViolationADO
    {
        /// <summary>Dich vu da co (da chi dinh truoc, hoac dang tich chon cung phieu)</summary>
        public long ASSIGNED_SERVICE_ID { get; set; }
        public string ASSIGNED_SERVICE_NAME { get; set; }

        /// <summary>Dich vu nguoi dung dang chi dinh, bi loai tru boi dich vu tren</summary>
        public long ASSIGNING_SERVICE_ID { get; set; }
        public string ASSIGNING_SERVICE_NAME { get; set; }

        /// <summary>1 = Canh bao, 2 = Chan</summary>
        public short HANDLE_TYPE_ID { get; set; }

        public string HANDLE_TYPE_NAME
        {
            get
            {
                return this.HANDLE_TYPE_ID == (short)HandleType.Block ? "Chặn" : "Cảnh báo";
            }
        }

        public ViolationSource SOURCE { get; set; }

        public string SOURCE_NAME
        {
            get
            {
                return this.SOURCE == ViolationSource.Assigning
                    ? "Đang chỉ định cùng lúc"
                    : "Đã chỉ định trước đó";
            }
        }
    }
}
