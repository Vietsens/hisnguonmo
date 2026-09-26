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
using Newtonsoft.Json;
using System;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder.ADO
{
    /// <summary>
    /// Thong tin nguoi benh doc duoc tai buoc dinh danh tren man lay so ki-ot.
    /// Doi tuong nay duoc chuoi hoa thanh JSON va luu vao HIS_REGISTER_REQ.IDENTITY_JSON.
    /// Cau truc truong theo PTTK_54254 muc B.2.5 - KHONG doi ten truong khi chua sua tai lieu.
    /// </summary>
    public class IdentityInfoADO
    {
        /// <summary>Loai dinh danh: CCCD / VNEID / BHYT</summary>
        public string IdentityType { get; set; }

        /// <summary>Cach nhap: QR (quet) hoac MANUAL (go tay)</summary>
        public string InputMode { get; set; }

        /// <summary>So CCCD, so CMND hoac ma the BHYT</summary>
        public string IdentityNumber { get; set; }

        /// <summary>So CMND cu doc duoc tu ma QR the can cuoc</summary>
        public string OldIdNumber { get; set; }

        /// <summary>Ho ten ghi tren giay to</summary>
        public string PatientName { get; set; }

        /// <summary>Ngay sinh dang dd/MM/yyyy, co the chi co MM/yyyy hoac yyyy</summary>
        public string Dob { get; set; }

        /// <summary>Gioi tinh theo chuoi ghi tren giay to</summary>
        public string Gender { get; set; }

        /// <summary>Dia chi day du ghi tren giay to</summary>
        public string Address { get; set; }

        /// <summary>Ngay cap the can cuoc dang dd/MM/yyyy</summary>
        public string IssueDate { get; set; }

        /// <summary>Ma noi dang ky kham chua benh ban dau tren the BHYT</summary>
        public string HeinMediOrgCode { get; set; }

        /// <summary>Han the BHYT tu ngay</summary>
        public string HeinFromDate { get; set; }

        /// <summary>Han the BHYT den ngay</summary>
        public string HeinToDate { get; set; }

        /// <summary>
        /// Chuoi quet goc. Man Tiep don 2 dua lai chuoi nay vao dung luong quet ma
        /// dang co o khung Benh nhan (F2), nho vay khong phai viet lai bo doc.
        /// </summary>
        public string RawData { get; set; }

        /// <summary>Thoi diem dinh danh, dang yyyyMMddHHmmss</summary>
        public string IssueTime { get; set; }

        /// <summary>
        /// Hinh thuc nguoi benh da chon tren popup. Chi dung trong bo nho cua man ki-ot,
        /// khong gui len may chu nen khong dua vao JSON.
        /// </summary>
        [JsonIgnore]
        public EnumIdentityType SelectedType { get; set; }

        public IdentityInfoADO()
        {
            this.SelectedType = EnumIdentityType.None;
        }

        /// <summary>
        /// Chuoi hoa thanh JSON de gui kem luot cap so.
        /// Bo qua truong rong cho gon, vi cot luu gioi han 4000 byte.
        /// </summary>
        public string ToJson()
        {
            try
            {
                return JsonConvert.SerializeObject(this, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// So dinh danh da che bot de hien thi tren man ki-ot.
        /// Chi giu 4 ky tu cuoi, phan con lai thay bang dau sao.
        /// Theo quy tac bao ve du lieu nguoi benh - PTTK_54254 R19.
        /// </summary>
        public string GetMaskedNumber()
        {
            try
            {
                string number = this.IdentityNumber ?? "";
                number = number.Trim();
                if (number.Length <= 4)
                {
                    return number;
                }
                return new String('*', number.Length - 4) + number.Substring(number.Length - 4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Da xac dinh duoc nguoi benh hay chua</summary>
        public bool HasIdentity()
        {
            return !String.IsNullOrWhiteSpace(this.IdentityType)
                && !String.IsNullOrWhiteSpace(this.IdentityNumber);
        }
    }
}
