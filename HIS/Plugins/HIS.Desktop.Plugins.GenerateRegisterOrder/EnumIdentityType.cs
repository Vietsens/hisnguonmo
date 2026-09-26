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
namespace HIS.Desktop.Plugins.GenerateRegisterOrder
{
    /// <summary>
    /// Hinh thuc xac dinh nguoi benh tai man lay so ki-ot.
    /// Tuong ung cot HIS_REGISTER_REQ.IDENTITY_TYPE do PTTK_54254 bo sung.
    /// </summary>
    public enum EnumIdentityType
    {
        /// <summary>Khong xac dinh danh - giu duong lay so hien tai</summary>
        None = 0,

        /// <summary>Quet ma QR tren the can cuoc cong dan</summary>
        Cccd = 1,

        /// <summary>Quet ma QR dinh danh hien tren ung dung VNeID</summary>
        VneId = 2,

        /// <summary>Nguoi benh go tay so CCCD hoac CMND</summary>
        CccdManual = 3,

        /// <summary>Quet ma QR tren the bao hiem y te hoac go tay ma the</summary>
        Bhyt = 4
    }

    /// <summary>
    /// Ma loai dinh danh gui len may chu. Khong dung so de tranh lech nghia khi backend doi enum.
    /// </summary>
    public class IdentityTypeCode
    {
        /// <summary>Can cuoc cong dan - dung cho ca quet QR the lan go tay</summary>
        public const string CCCD = "CCCD";

        /// <summary>Ma QR dinh danh tren ung dung VNeID</summary>
        public const string VNEID = "VNEID";

        /// <summary>The bao hiem y te</summary>
        public const string BHYT = "BHYT";
    }

    /// <summary>
    /// Cach nguoi benh dua thong tin vao: quet ma hay go tay.
    /// </summary>
    public class IdentityInputMode
    {
        /// <summary>Quet bang dau doc ma QR</summary>
        public const string QR = "QR";

        /// <summary>Go tay tren ban phim man hinh</summary>
        public const string MANUAL = "MANUAL";
    }
}
