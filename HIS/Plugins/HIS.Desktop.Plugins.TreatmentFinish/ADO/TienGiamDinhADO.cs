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

namespace HIS.Desktop.Plugins.TreatmentFinish.ADO
{
    /// <summary>
    /// Mot dong loi do he thong tien giam dinh tra ve, da chuan hoa ve chung 1 dang
    /// de hien thi bat ke thuoc nhom nao.
    /// </summary>
    public class TienGiamDinhErrorADO
    {
        /// <summary>Nhom loi: sai sot y lenh / loi tra the BHYT / loi ho so XML</summary>
        public EnumTienGiamDinhErrorGroup Group { get; set; }

        /// <summary>Ten nhom loi hien thi cho nguoi dung</summary>
        public string GroupName { get; set; }

        /// <summary>Loi nghiem trong hay khong - quyet dinh co chan hay khong</summary>
        public bool IsCritical { get; set; }

        /// <summary>Ma quy tac / ma loi do he ngoai tra ve. Co the rong</summary>
        public string Code { get; set; }

        /// <summary>Mo ta loi doc duoc cho nguoi dung</summary>
        public string Description { get; set; }

        /// <summary>
        /// Chuoi hien thi day du gom tien to nhom loi.
        /// Dung khi can nhoi vao danh sach canh bao chi chua 1 chuoi van ban.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (string.IsNullOrEmpty(this.GroupName))
                {
                    return this.Description;
                }
                return this.GroupName + ": " + this.Description;
            }
        }
    }

    /// <summary>
    /// Ket qua tra cuu loi cua mot dot dieu tri tren he thong tien giam dinh.
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

        /// <summary>Ket qua tong the cua ho so</summary>
        public EnumTienGiamDinhStatus Status { get; set; }

        /// <summary>Ly do khong tra cuu duoc. Chi co nghia khi Status = CheckFailed</summary>
        public EnumTienGiamDinhFailReason FailReason { get; set; }

        /// <summary>Danh sach loi da chuan hoa cua ca ba nhom</summary>
        public List<TienGiamDinhErrorADO> Errors { get; set; }

        /// <summary>
        /// He ngoai bao danh sach loi bi cat bot do vuot nguong.
        /// Danh sach tra ve khong day du nen phai coi la co loi nghiem trong.
        /// </summary>
        public bool IsTruncated { get; set; }

        /// <summary>
        /// Ma dinh danh luot goi do he ngoai tra ve.
        /// Ghi vao nhat ky de doi chieu khi co tranh chap.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>Co loi nghiem trong hay khong - dieu kien chan</summary>
        public bool HasCriticalError
        {
            get { return this.Status == EnumTienGiamDinhStatus.Critical; }
        }

        /// <summary>Co bat ky loi nao hay khong, khong phan biet muc do</summary>
        public bool HasAnyError
        {
            get
            {
                return this.Status == EnumTienGiamDinhStatus.Critical
                    || this.Status == EnumTienGiamDinhStatus.Warning;
            }
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
