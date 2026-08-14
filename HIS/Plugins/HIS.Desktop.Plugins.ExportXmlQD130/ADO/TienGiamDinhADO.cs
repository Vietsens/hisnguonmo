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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.ExportXmlQD130.ADO
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

        /// <summary>Co loi nghiem trong - thuoc muc chan xuat</summary>
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

    /// <summary>Ly do khong tra cuu duoc - dung de hien thong bao dung ngu canh</summary>
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

    /// <summary>
    /// Mot dong loi do he thong tien giam dinh tra ve, da chuan hoa ve chung mot dang
    /// de hien thi bat ke thuoc nhom nao.
    /// </summary>
    public class TienGiamDinhErrorADO
    {
        /// <summary>Nhom loi</summary>
        public EnumTienGiamDinhErrorGroup Group { get; set; }

        /// <summary>Ten nhom loi hien thi tren luoi - do man hinh gan theo ngon ngu dang dung</summary>
        public string GroupName { get; set; }

        /// <summary>Loi nghiem trong hay khong - quyet dinh co chan xuat hay khong</summary>
        public bool IsCritical { get; set; }

        /// <summary>Muc do hien thi tren luoi</summary>
        public string SeverityName { get; set; }

        /// <summary>Ma quy tac / ma loi do he ngoai tra ve</summary>
        public string Code { get; set; }

        /// <summary>Mo ta loi doc duoc cho nguoi dung</summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Ket qua tra cuu loi cua mot dot dieu tri.
    /// Chi ton tai trong phien lam viec, khong luu xuong CSDL.
    /// </summary>
    public class TienGiamDinhResultADO
    {
        public TienGiamDinhResultADO()
        {
            this.Errors = new List<TienGiamDinhErrorADO>();
            this.Status = EnumTienGiamDinhStatus.NotChecked;
            this.FailReason = EnumTienGiamDinhFailReason.None;
        }

        /// <summary>Ma dot dieu tri da tra cuu</summary>
        public string TreatmentCode { get; set; }

        /// <summary>Ten benh nhan - lay tu dong tuong ung tren luoi chinh, chi de hien thi</summary>
        public string PatientName { get; set; }

        /// <summary>Ket qua tong the cua ho so</summary>
        public EnumTienGiamDinhStatus Status { get; set; }

        /// <summary>Ten trang thai hien thi tren luoi</summary>
        public string StatusName { get; set; }

        /// <summary>Ly do khong tra cuu duoc. Chi co nghia khi Status = CheckFailed</summary>
        public EnumTienGiamDinhFailReason FailReason { get; set; }

        /// <summary>Danh sach loi da chuan hoa cua ca ba nhom</summary>
        public List<TienGiamDinhErrorADO> Errors { get; set; }

        /// <summary>He ngoai bao danh sach loi bi cat bot do vuot nguong</summary>
        public bool IsTruncated { get; set; }

        /// <summary>Ma dinh danh luot goi - ghi nhat ky de doi chieu khi co tranh chap</summary>
        public string RequestId { get; set; }

        /// <summary>Co loi nghiem trong hay khong - dieu kien chan xuat</summary>
        public bool HasCriticalError
        {
            get { return this.Status == EnumTienGiamDinhStatus.Critical; }
        }

        /// <summary>Tong so dong loi cua ca ba nhom</summary>
        public int TotalErrorCount
        {
            get { return this.Errors == null ? 0 : this.Errors.Count; }
        }

        /// <summary>So dong loi nghiem trong</summary>
        public int CriticalErrorCount
        {
            get
            {
                if (this.Errors == null)
                {
                    return 0;
                }
                int count = 0;
                foreach (TienGiamDinhErrorADO error in this.Errors)
                {
                    if (error.IsCritical)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
