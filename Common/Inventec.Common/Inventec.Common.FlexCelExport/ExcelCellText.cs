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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;

namespace Inventec.Common.FlexCellExport
{
    /// <summary>
    /// Xu ly chuoi dai vuot gioi han 1 o Excel (32767 ky tu).
    /// FlexCel nem FlexCelXlsAdapterException khi Save neu co o vuot gioi han -> ca phieu in khong ra.
    /// </summary>
    public static class ExcelCellText
    {
        public const int MAX_LENGTH = 32767;
        public const int LINE_LENGTH = 500;
        const string TRUNCATED_MARK = "...";

        /// <summary>
        /// Gia tri dua vao 1 o: chuoi vuot gioi han thi cat bot va them "..." de nguoi doc biet da bi cat.
        /// Gia tri khong phai chuoi giu nguyen.
        /// </summary>
        public static object LimitCellValue(string keyName, object value)
        {
            string str = value as string;
            if (str == null || str.Length <= MAX_LENGTH)
                return value;

            LogSystem.Warn(System.String.Format("Key {0} dai {1} ky tu, vuot gioi han o Excel ({2}) -> cat bot. Muon in du dung bang {0}_LINES (vung ten __{0}_LINES__, truong LINE).", keyName, str.Length, MAX_LENGTH));
            int length = MAX_LENGTH - TRUNCATED_MARK.Length;
            if (Char.IsHighSurrogate(str[length - 1]))
                length--;
            return str.Substring(0, length) + TRUNCATED_MARK;
        }

        /// <summary>
        /// Chia chuoi thanh cac doan dai toi da maxLength, khong cat doi cap ky tu surrogate.
        /// </summary>
        public static List<string> SplitByLength(string value, int maxLength)
        {
            List<string> result = new List<string>();
            if (System.String.IsNullOrEmpty(value))
                return result;

            int start = 0;
            while (start < value.Length)
            {
                int length = Math.Min(maxLength, value.Length - start);
                if (start + length < value.Length && Char.IsHighSurrogate(value[start + length - 1]))
                    length--;
                result.Add(value.Substring(start, length));
                start += length;
            }
            return result;
        }

        /// <summary>
        /// Tach van ban thanh cac dong de in tren nhieu dong Excel:
        /// tach theo xuong dong, dong nao dai qua LINE_LENGTH thi ngat tai khoang trang gan nhat.
        /// </summary>
        public static List<TemplateKeyLine> SplitToLines(string value)
        {
            List<TemplateKeyLine> result = new List<TemplateKeyLine>();
            if (System.String.IsNullOrEmpty(value))
                return result;

            string[] paragraphs = value.TrimEnd('\r', '\n').Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            foreach (string paragraph in paragraphs)
            {
                string remain = paragraph;
                do
                {
                    string line = remain;
                    if (remain.Length > LINE_LENGTH)
                    {
                        int breakAt = remain.LastIndexOf(' ', LINE_LENGTH - 1, LINE_LENGTH / 2);
                        int length = breakAt > 0 ? breakAt + 1 : LINE_LENGTH;
                        if (Char.IsHighSurrogate(remain[length - 1]))
                            length--;
                        line = remain.Substring(0, length);
                    }
                    remain = remain.Substring(line.Length);
                    result.Add(new TemplateKeyLine(result.Count + 1, line));
                }
                while (remain.Length > 0);
            }
            return result;
        }
    }
}
