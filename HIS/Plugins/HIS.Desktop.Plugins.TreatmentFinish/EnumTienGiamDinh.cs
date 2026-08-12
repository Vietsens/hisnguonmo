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

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// Ket qua kiem tra ho so tren he thong tien giam dinh.
    /// Khong co trong IMSys.DbConfig vi day la trang thai noi bo cua plugin, khong luu xuong CSDL.
    /// </summary>
    public enum EnumTienGiamDinhStatus
    {
        /// <summary>Chua tra cuu trong phien lam viec nay</summary>
        NotChecked = 0,

        /// <summary>He ngoai khong tra ve loi nao</summary>
        NoError = 1,

        /// <summary>Co loi nhung khong thuoc muc chan</summary>
        Warning = 2,

        /// <summary>Co loi nghiem trong - thuoc muc chan</summary>
        Critical = 3,

        /// <summary>Khong tra cuu duoc: he ngoai loi, qua thoi gian cho, hoac sai thong tin xac thuc</summary>
        CheckFailed = 4
    }

    /// <summary>
    /// Ba nhom loi ma he thong tien giam dinh tra ve.
    /// Tham chieu dac ta API muc 4 - GET /api/order-check/violations.
    /// </summary>
    public enum EnumTienGiamDinhErrorGroup
    {
        /// <summary>Sai sot y lenh - bo quet order-check phat hien tren HIS</summary>
        OrderCheck = 1,

        /// <summary>Loi tra cuu the BHYT tren cong BHXH</summary>
        HeinCard = 2,

        /// <summary>Loi ho so XML theo Quyet dinh 3176</summary>
        Xml3176 = 3
    }

    /// <summary>
    /// Ly do khong tra cuu duoc - dung de hien thong bao dung ngu canh cho nguoi dung.
    /// </summary>
    public enum EnumTienGiamDinhFailReason
    {
        /// <summary>Khong that bai</summary>
        None = 0,

        /// <summary>Chua khai bao thong tin ket noi hoac khai thieu thanh phan bat buoc</summary>
        NotConfigured = 1,

        /// <summary>Sai chuoi xac thuc - loi cau hinh, khong phai loi nghiep vu</summary>
        Unauthorized = 2,

        /// <summary>Qua thoi gian cho</summary>
        Timeout = 3,

        /// <summary>He ngoai qua tai tam thoi, da thu lai nhung van hong</summary>
        RateLimited = 4,

        /// <summary>Loi he thong phia he ngoai hoac loi ket noi mang</summary>
        SystemError = 5
    }
}
