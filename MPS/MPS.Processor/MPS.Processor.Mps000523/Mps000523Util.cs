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
using System.Text;
using MOS.EFMODEL.DataModels;

namespace MPS.Processor.Mps000523
{
    /// <summary>Pure helpers of Mps000523 (no AbstractProcessor here: the factory takes the first AbstractProcessor type).</summary>
    internal static class Mps000523Util
    {
        /// <summary>FDI numbering, upper row then lower row as printed on the form.</summary>
        internal static readonly int[] TEETH = { 18, 17, 16, 15, 14, 13, 12, 11, 21, 22, 23, 24, 25, 26, 27, 28,
                                                 48, 47, 46, 45, 44, 43, 42, 41, 31, 32, 33, 34, 35, 36, 37, 38 };

        // ---------------------------------------------------------------- column access by name

        /// <summary>Column value by name; null when the object or the column (older EFMODEL) is missing.</summary>
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
            string s = v != null ? v.ToString().Trim() : null;
            return string.IsNullOrEmpty(s) ? null : s;
        }

        internal static long? Long(object data, string column)
        {
            object v = Col(data, column);
            if (v == null) return null;
            long r;
            return long.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out r) ? (long?)r : null;
        }

        /// <summary>Measure without trailing zeros: 170.00 -> "170", 65.50 -> "65.5".</summary>
        internal static string Num(decimal? v)
        {
            return v.HasValue ? v.Value.ToString("0.##", CultureInfo.InvariantCulture) : "";
        }

        // ---------------------------------------------------------------- strings

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

        /// <summary>Lower case, NFC, single spaces — for matching catalog names typed by the portal or the hospital.</summary>
        internal static string Norm(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            string n = s.Normalize(NormalizationForm.FormC).ToLower(new CultureInfo("vi-VN"));
            return string.Join(" ", n.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>Age in full years at the exam time (yyyyMMddHHmmss).</summary>
        internal static string Age(long dob, long atTime)
        {
            try
            {
                if (dob <= 0 || atTime <= 0) return "";
                string d = dob.ToString(), a = atTime.ToString();
                int age = int.Parse(a.Substring(0, 4)) - int.Parse(d.Substring(0, 4));
                if (string.CompareOrdinal(a.Substring(4, 4), d.Substring(4, 4)) < 0) age--;
                return age >= 0 ? age.ToString() : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        // ---------------------------------------------------------------- ICD

        /// <summary>"cầu – trụ – trục" with fixed positions under the template label; a missing part prints "…", none at all prints "".</summary>
        internal static string Refraction(string sphere, string cylinder, string axis)
        {
            string[] parts = { (sphere ?? "").Trim(), (cylinder ?? "").Trim(), (axis ?? "").Trim() };
            if (parts.All(o => o.Length == 0)) return "";
            return string.Join(" – ", parts.Select(o => o.Length > 0 ? o : "…"));
        }

        /// <summary>"Mã -- Tên;Mã -- Tên" (ICD text of HIS_KSK_GENERAL) -> "Mã - Tên; Mã - Tên", like the examination parts.</summary>
        internal static string IcdText(string names)
        {
            var list = new List<string>();
            AddIcdNames(list, names);
            return string.Join("; ", list);
        }

        /// <summary>Adds the names of a "Mã -- Tên; Mã -- Tên" portal list once each, printed as "Mã - Tên".</summary>
        internal static void AddIcdNames(List<string> target, string names)
        {
            if (string.IsNullOrWhiteSpace(names)) return;
            foreach (string part in names.Split(';'))
            {
                string item = part.Replace(" -- ", " - ").Trim();
                if (item.Length > 0 && !target.Contains(item)) target.Add(item);
            }
        }

        /// <summary>HIS ICD codes + names (';' separated, same order) -> "I10 - Tăng huyết áp; E11 - ...".</summary>
        internal static string JoinIcd(string codes, string names)
        {
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

        // ---------------------------------------------------------------- catalog names -> boxes of the form

        /// <summary>Level 1..5 of a HIS_HEALTH_EXAM_RANK (by code, then name: "L3", "Loại III"); 0 = unknown.</summary>
        internal static int RankLevel(long? rankId, List<HIS_HEALTH_EXAM_RANK> ranks)
        {
            if (!rankId.HasValue || rankId.Value <= 0 || ranks == null) return 0;
            HIS_HEALTH_EXAM_RANK r = ranks.FirstOrDefault(o => o != null && o.ID == rankId.Value);
            if (r == null) return 0;
            int level = ParseLevel(r.HEALTH_EXAM_RANK_CODE);
            return level > 0 ? level : ParseLevel(r.HEALTH_EXAM_RANK_NAME);
        }

        /// <summary>Reads the LAST token ("Loại III" -> 3, "L2" -> 2, "4" -> 4) so the "I" of "LOẠI" is not taken.</summary>
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

        /// <summary>D6/D7 frequency name -> 1 Hầu như không, 2 Một vài ngày, 3 Hơn một nửa số ngày, 4 Gần như mọi ngày.</summary>
        internal static int FrequencyLevel(string name)
        {
            string n = Norm(name);
            if (n.Length == 0) return 0;
            if (n.Contains("hầu như không")) return 1;
            if (n.Contains("vài ngày")) return 2;
            if (n.Contains("hơn một") || n.Contains("nửa") || n.Contains("nữa")) return 3;
            if (n.Contains("mọi ngày")) return 4;
            return 0;
        }

        /// <summary>Tooth status name -> code 0..9 printed on the form; "Không ghi nhận"/unknown -> "".</summary>
        internal static string ToothCode(string name)
        {
            string n = Norm(name);
            if (n.Length == 0 || n.Contains("không ghi nhận") || n.Contains("chưa ghi")) return "";
            if (n.Contains("trám sâu lại")) return "2";
            if (n.Contains("trám tốt")) return "3";
            if (n.Contains("mất do sâu")) return "4";
            if (n.Contains("mất")) return "5";
            if (n.Contains("bít hố rãnh")) return "6";
            if (n.Contains("trụ") || n.Contains("cầu") || n.Contains("implant")) return "7";
            if (n.Contains("chưa mọc")) return "8";
            if (n.Contains("loại trừ")) return "9";
            if (n.Contains("sâu")) return "1";
            if (n.Contains("bình thường")) return "0";
            return "";
        }

        /// <summary>"Đề nghị" name -> 1..5 of the form.</summary>
        internal static int SuggestOption(string name)
        {
            string n = Norm(name);
            if (n.Length == 0) return 0;
            if (n.Contains("bình thường")) return 1;
            if (n.Contains("nguy cơ")) return 2;
            if (n.Contains("mạn tính")) return 3;
            if (n.Contains("chuyển")) return 4;
            return 5;
        }
    }
}
