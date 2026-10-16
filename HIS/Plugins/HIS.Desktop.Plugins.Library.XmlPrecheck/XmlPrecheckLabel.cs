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
using HIS.Desktop.Plugins.Library.XmlPrecheck.ADO;
using System;

namespace HIS.Desktop.Plugins.Library.XmlPrecheck
{
    /// <summary>
    /// vCong MDInsight ban 3.0 - Suy nhan "so bo" va "du lieu da doi" tu du lieu DA CO, khong luu them cot nao.
    ///
    /// Mot ket qua la SO BO khi ho so chua co thoi gian khoa vien phi, hoac moc kiem (uu tien thoi diem gui,
    /// khong co thi lay thoi diem ket luan) SOM HON thoi gian khoa vien phi - quy tac QT-32.
    /// Ket qua la DA CU khi ho so duoc sua sau moc kiem - quy tac QT-33.
    ///
    /// Dung chung cho man Ket thuc dieu tri va man Xuat XML 130 de hai noi hien cung mot cach.
    /// </summary>
    public static class XmlPrecheckLabel
    {
        /// <summary>Moc kiem de so sanh: thoi diem gui neu co, khong thi thoi diem ket luan</summary>
        public static long? GetCheckMark(long? sendTime, long? checkTime)
        {
            if (sendTime.HasValue && sendTime.Value > 0)
            {
                return sendTime;
            }
            return checkTime.HasValue && checkTime.Value > 0 ? checkTime : null;
        }

        /// <summary>Ket qua co phai la so bo (kiem truoc khi khoa vien phi) hay khong</summary>
        public static bool IsPreliminary(long? sendTime, long? checkTime, long? feeLockTime)
        {
            long? mark = GetCheckMark(sendTime, checkTime);
            if (!mark.HasValue)
            {
                return false;
            }
            if (!feeLockTime.HasValue || feeLockTime.Value <= 0)
            {
                return true;
            }
            return mark.Value < feeLockTime.Value;
        }

        /// <summary>Ho so da khoa vien phi SAU khi kiem so bo -> ket qua so bo phai kiem lai</summary>
        public static bool IsRecheckNeeded(long? sendTime, long? checkTime, long? feeLockTime)
        {
            return IsPreliminary(sendTime, checkTime, feeLockTime)
                && feeLockTime.HasValue && feeLockTime.Value > 0;
        }

        /// <summary>Ho so duoc sua sau moc kiem - ket qua la anh chup da cu</summary>
        public static bool IsStale(long? modifyTime, long? sendTime, long? checkTime)
        {
            long? mark = GetCheckMark(sendTime, checkTime);
            if (!mark.HasValue || !modifyTime.HasValue || modifyTime.Value <= 0)
            {
                return false;
            }
            return modifyTime.Value > mark.Value;
        }

        /// <summary>
        /// Ghep nhan vao chu trang thai: "{trang thai} (so bo) - can kiem lai · du lieu da doi".
        /// Khong rơi vao truong hop nao thi tra ve nguyen chu trang thai.
        /// </summary>
        public static string Decorate(string statusText, bool preliminary, bool recheckNeeded, bool stale)
        {
            string result = statusText ?? "";
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            if (preliminary)
            {
                result += " " + Resources.ResourceMessageLang.XmlPrecheckNhanSoBo;
                if (recheckNeeded)
                {
                    result += " - " + Resources.ResourceMessageLang.XmlPrecheckNhanCanKiemLai;
                }
            }

            if (stale)
            {
                result += " · " + Resources.ResourceMessageLang.XmlPrecheckNhanDuLieuDaDoi;
            }

            return result;
        }

        /// <summary>Chu hien thi cua tung ma trang thai - dung chung hai man</summary>
        public static string StatusName(short? status)
        {
            if (!status.HasValue)
            {
                return "";
            }

            switch ((EnumXmlPrecheckStatus)status.Value)
            {
                case EnumXmlPrecheckStatus.NoError:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiKhongLoi;
                case EnumXmlPrecheckStatus.Warning:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiCanhBao;
                case EnumXmlPrecheckStatus.Critical:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiLoiNghiemTrong;
                case EnumXmlPrecheckStatus.Pending:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiChuaCoKetQua;
                case EnumXmlPrecheckStatus.CheckFailed:
                    return Resources.ResourceMessageLang.MdInsightTrangThaiKhongKiemTraDuoc;
                default:
                    return "";
            }
        }

        /// <summary>Chu trang thai cua mot ket qua, da ghep nhan so bo / da cu theo ho so</summary>
        public static string Describe(MdInsightResultADO result, long? feeLockTime, long? modifyTime)
        {
            if (result == null)
            {
                return "";
            }

            bool preliminary = IsPreliminary(result.SendTime, result.CheckTime, feeLockTime);
            bool recheck = IsRecheckNeeded(result.SendTime, result.CheckTime, feeLockTime);
            bool stale = IsStale(modifyTime, result.SendTime, result.CheckTime);

            return Decorate(StatusName((short)result.Status), preliminary, recheck, stale);
        }
    }
}
