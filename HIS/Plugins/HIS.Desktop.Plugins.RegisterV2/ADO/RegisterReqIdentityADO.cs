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
namespace HIS.Desktop.Plugins.RegisterV2.ADO
{
    /// <summary>
    /// Thong tin nguoi benh doc tu chuoi JSON cua ban ghi cap so thu tu.
    /// Ten truong phai trung voi ben man ki-ot ghi ra - PTTK_54254 muc B.2.5.
    ///
    /// Ba cot IDENTITY_TYPE / IDENTITY_NUMBER / IDENTITY_JSON doc thang tu
    /// HIS_REGISTER_REQ vi thu vien mo hinh du lieu cua backend da co san.
    /// </summary>
    public class RegisterReqIdentityInfoADO
    {
        /// <summary>Loai dinh danh: CCCD / VNEID / BHYT</summary>
        public string IdentityType { get; set; }

        /// <summary>Cach nhap: QR (quet) hoac MANUAL (go tay)</summary>
        public string InputMode { get; set; }

        /// <summary>So CCCD, so CMND hoac ma the BHYT</summary>
        public string IdentityNumber { get; set; }

        /// <summary>So CMND cu</summary>
        public string OldIdNumber { get; set; }

        /// <summary>Ho ten ghi tren giay to</summary>
        public string PatientName { get; set; }

        /// <summary>Ngay sinh dang dd/MM/yyyy</summary>
        public string Dob { get; set; }

        /// <summary>Gioi tinh theo chuoi ghi tren giay to</summary>
        public string Gender { get; set; }

        /// <summary>Dia chi day du ghi tren giay to</summary>
        public string Address { get; set; }

        /// <summary>Ngay cap the can cuoc</summary>
        public string IssueDate { get; set; }

        /// <summary>Ma noi dang ky kham chua benh ban dau</summary>
        public string HeinMediOrgCode { get; set; }

        /// <summary>Han the BHYT tu ngay</summary>
        public string HeinFromDate { get; set; }

        /// <summary>Han the BHYT den ngay</summary>
        public string HeinToDate { get; set; }

        /// <summary>
        /// Chuoi quet goc. Dua lai chuoi nay vao dung luong quet ma dang co
        /// o khung Benh nhan de giu nguyen moi hanh vi san co.
        /// </summary>
        public string RawData { get; set; }

        /// <summary>Thoi diem dinh danh, dang yyyyMMddHHmmss</summary>
        public string IssueTime { get; set; }
    }
}
