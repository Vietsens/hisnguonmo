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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000524
{
    /// <summary>Hàm thuần của Mps000524 (không kế thừa AbstractProcessor: factory lấy kiểu AbstractProcessor đầu tiên).</summary>
    internal static class Mps000524Util
    {
        // ---------------------------------------------------------------- đọc cột theo tên

        /// <summary>Giá trị cột theo tên; null khi đối tượng null hoặc không có cột (EFMODEL cũ).</summary>
        internal static object Col(object data, string column)
        {
            if (data == null || string.IsNullOrEmpty(column)) return null;
            try
            {
                var pi = data.GetType().GetProperty(column);
                return pi != null ? pi.GetValue(data, null) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        internal static string Str(object data, string column)
        {
            object v = Col(data, column);
            string s = v != null ? Convert.ToString(v, CultureInfo.InvariantCulture).Trim() : null;
            return string.IsNullOrEmpty(s) ? null : s;
        }

        internal static long? Long(object data, string column)
        {
            object v = Col(data, column);
            if (v == null) return null;
            long r;
            return long.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? (long?)r : null;
        }

        /// <summary>Số đo bỏ số 0 thừa: 170.00 -> "170", 65.50 -> "65.5".</summary>
        internal static string Num(decimal? v)
        {
            return v.HasValue ? v.Value.ToString("0.##", CultureInfo.InvariantCulture) : "";
        }

        // ---------------------------------------------------------------- chuỗi

        internal static string First(params string[] values)
        {
            foreach (string v in values)
                if (!string.IsNullOrWhiteSpace(v)) return v.Trim();
            return null;
        }

        internal static string Join(string separator, params string[] values)
        {
            return string.Join(separator, values.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray());
        }

        /// <summary>yyyyMMddHHmmss -> dd/MM/yyyy; rỗng khi không có.</summary>
        internal static string Date(long? time)
        {
            if (!time.HasValue || time.Value <= 0) return "";
            try
            {
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(time.Value);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        // ---------------------------------------------------------------- ICD

        /// <summary>
        /// Mã + tên ICD (phân tách ';', cùng thứ tự) -> "I10 - Tăng huyết áp; E11 - ...".
        /// Tên đã ở dạng "Mã -- Tên" (danh mục cổng) thì in theo tên.
        /// </summary>
        internal static string Icd(string codes, string names)
        {
            if (!string.IsNullOrWhiteSpace(names) && names.Contains(" -- "))
            {
                var list = new List<string>();
                foreach (string part in names.Split(';'))
                {
                    string item = part.Replace(" -- ", " - ").Trim();
                    if (item.Length > 0 && !list.Contains(item)) list.Add(item);
                }
                return string.Join("; ", list);
            }
            var c = (codes ?? "").Split(';').Select(o => o.Trim()).ToList();
            var n = (names ?? "").Split(';').Select(o => o.Trim()).ToList();
            var parts = new List<string>();
            for (int i = 0; i < Math.Max(c.Count, n.Count); i++)
            {
                string ci = i < c.Count ? c[i] : "", ni = i < n.Count ? n[i] : "";
                if (ci.Length == 0 && ni.Length == 0) continue;
                parts.Add(ci.Length > 0 && ni.Length > 0 ? ci + " - " + ni : ci + ni);
            }
            return string.Join("; ", parts);
        }

        // ---------------------------------------------------------------- phân loại sức khỏe

        /// <summary>Mức 1..5 của HIS_HEALTH_EXAM_RANK (theo mã rồi tên: "L3", "Loại III"); 0 = không xác định.</summary>
        internal static int RankLevel(long? rankId, List<HIS_HEALTH_EXAM_RANK> ranks)
        {
            if (!rankId.HasValue || rankId.Value <= 0 || ranks == null) return 0;
            HIS_HEALTH_EXAM_RANK r = ranks.FirstOrDefault(o => o != null && o.ID == rankId.Value);
            if (r == null) return 0;
            int level = ParseLevel(r.HEALTH_EXAM_RANK_CODE);
            return level > 0 ? level : ParseLevel(r.HEALTH_EXAM_RANK_NAME);
        }

        /// <summary>Đọc token CUỐI ("Loại III" -> 3, "L2" -> 2, "4" -> 4) để không bắt nhầm chữ "I" của "LOẠI".</summary>
        internal static int ParseLevel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            string[] parts = value.Trim().Split(new[] { ' ', '\t', ':', '-', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return 0;
            string last = parts[parts.Length - 1].ToUpperInvariant();
            if (last.Length > 1 && last[0] == 'L' && char.IsDigit(last[1])) last = last.Substring(1);
            int n;
            if (int.TryParse(last, out n)) return n >= 1 && n <= 5 ? n : 0;
            switch (last)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
                default: return 0;
            }
        }
    }
}
