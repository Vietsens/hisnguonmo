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
using MOS.EFMODEL.DataModels;
using System.Text;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO
{
    /// <summary>
    /// Ban tam "Noi dung khong dat" cua mot ho so dieu tri (viec 57750).
    /// Du lieu nam tren 4 cot cua HIS_TREATMENT: REJECT_STORE_DRAFT / _TIME / _LOGINNAME / _USERNAME.
    /// </summary>
    class RecordDraftADO
    {
        /// <summary>Gioi han cua cot ly do tu choi va cot ban tam: VARCHAR2(4000 BYTE), tinh theo UTF-8.</summary>
        internal const int MAX_BYTES = 4000;

        /// <summary>Noi dung ban tam. Null/rong = khong co ban tam.</summary>
        public string Content { get; set; }

        /// <summary>Thoi diem luu tam gan nhat, yyyyMMddHHmmss.</summary>
        public long? Time { get; set; }

        /// <summary>Tai khoan luu tam gan nhat.</summary>
        public string Loginname { get; set; }

        /// <summary>Ho ten nguoi luu tam gan nhat.</summary>
        public string Username { get; set; }

        public bool HasDraft
        {
            get { return !string.IsNullOrWhiteSpace(this.Content); }
        }

        /// <summary>Doc ban tam tu ban ghi ho so dieu tri. Null = khong co ban tam.</summary>
        public static RecordDraftADO FromTreatment(HIS_TREATMENT treatment)
        {
            RecordDraftADO result = new RecordDraftADO();
            if (treatment == null) return result;

            result.Content = treatment.REJECT_STORE_DRAFT;
            result.Time = treatment.REJECT_STORE_DRAFT_TIME;
            result.Loginname = treatment.REJECT_STORE_DRAFT_LOGINNAME;
            result.Username = treatment.REJECT_STORE_DRAFT_USERNAME;
            return result;
        }

        public void Clear()
        {
            this.Content = null;
            this.Time = null;
            this.Loginname = null;
            this.Username = null;
        }

        /// <summary>So byte UTF-8 cua noi dung - don vi cua gioi han 4000 BYTE tren Oracle.</summary>
        public static int CountBytes(string text)
        {
            return string.IsNullOrEmpty(text) ? 0 : Encoding.UTF8.GetByteCount(text);
        }
    }
}
