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
    /// Mot dong chi tiet thuoc/vat tu/dich vu di kem trong phieu thanh toan PT-TT.
    /// Toan bo gia tri hien thi da duoc processor tinh san, template khong can cong thuc.
    /// Doi tuong nay chi bo sung them, KHONG thay the "SereServFollow" dang dung o mau cu.
    /// </summary>
    public class Mps000324ItemADO
    {
        /// <summary>Id loai dich vu cua nhom chua dong nay. Noi voi Mps000324GroupADO.ID</summary>
        public long GROUP_ID { get; set; }

        /// <summary>So thu tu chay lien tuc tren toan phieu (1..n)</summary>
        public int NUM_ORDER { get; set; }

        /// <summary>So thu tu chay lai tu 1 trong tung nhom (1..n)</summary>
        public int NUM_ORDER_IN_GROUP { get; set; }

        /// <summary>Id ban ghi HIS_SERE_SERV goc</summary>
        public long SERE_SERV_ID { get; set; }

        /// <summary>Ma dich vu</summary>
        public string SERVICE_CODE { get; set; }

        /// <summary>Ten thuoc va dung cu</summary>
        public string SERVICE_NAME { get; set; }

        /// <summary>Don vi tinh</summary>
        public string SERVICE_UNIT_NAME { get; set; }

        /// <summary>So luong</summary>
        public decimal AMOUNT { get; set; }

        /// <summary>Don gia. Null khi dong khong co gia de o tren mau de trong</summary>
        public decimal? PRICE { get; set; }

        /// <summary>Thanh tien = AMOUNT * PRICE. Null khi dong khong co gia</summary>
        public decimal? INTO_MONEY { get; set; }

        /// <summary>
        /// Cot Ghi chu tren phieu = TEN DOI TUONG THANH TOAN cua dong dich vu
        /// (HIS_PATIENT_TYPE.PATIENT_TYPE_NAME: Thu phi / Dich vu / BHYT / Hao phi...).
        /// KHONG phai co IS_EXPEND — doi doi tuong thanh toan la cot nay doi theo.
        /// </summary>
        public string NOTE { get; set; }

        /// <summary>Id doi tuong thanh toan cua dong dich vu</summary>
        public long PATIENT_TYPE_ID { get; set; }

        /// <summary>Ma doi tuong thanh toan</summary>
        public string PATIENT_TYPE_CODE { get; set; }

        /// <summary>Ten doi tuong thanh toan — cung gia tri voi NOTE, dat ten ro nghia</summary>
        public string PATIENT_TYPE_NAME { get; set; }

        /// <summary>1: hao phi, 0: khong hao phi. De mau in loc/to mau neu can</summary>
        public short IS_EXPEND { get; set; }

        /// <summary>Dien giai co hao phi: "Hao Phi" / "Thu Phi". Tach rieng khoi NOTE
        /// de mau in nao can cach hien thi cu van dung duoc</summary>
        public string EXPEND_NOTE { get; set; }
    }
}
