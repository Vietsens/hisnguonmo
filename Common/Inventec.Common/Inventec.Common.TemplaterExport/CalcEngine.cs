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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Inventec.Common.TemplaterExport
{
    /// <summary>
    /// Bộ tính biểu thức cho template Word (thay cho công thức Excel / FlexCel Aggregate, Evaluate).
    /// Dựa trên DataColumn.Expression và DataTable.Compute của .NET nên hỗ trợ: + - * / %, so sánh, AND OR NOT,
    /// IIF(dk, a, b), ISNULL(a, b), LEN, TRIM, SUBSTRING, CONVERT và các hàm gộp sum/avg/min/max/count.
    /// Quy ước: null trong biểu thức đơn = 0; null trong hàm gộp bị bỏ qua (giống Excel bỏ ô trống).
    /// </summary>
    internal static class CalcEngine
    {
        const string RESULT_COLUMN = "__result";
        const string CALC_COLUMN = "__calc";

        static readonly HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AND", "OR", "NOT", "IIF", "ISNULL", "LEN", "TRIM", "SUBSTRING", "CONVERT", "TRUE", "FALSE", "NULL",
            "LIKE", "IN", "IS", "CHILD", "PARENT", "SUM", "AVG", "MIN", "MAX", "COUNT", "STDEV", "VAR"
        };

        static readonly Regex aggregateCallRegex = new Regex(@"(?<![\w.])(sum|avg|min|max|count)\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //Cho phep ten co dau cham (Pat.Items) de di vao object con
        static readonly Regex identifierRegex = new Regex(@"(?<![\w.'])([A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*)(?![\w.]|\s*\()", RegexOptions.Compiled);

        #region Parse

        /// <summary>
        /// Tách chuỗi theo dấu phân cách ở mức ngoài cùng (bỏ qua dấu nằm trong ngoặc hoặc trong chuỗi '...')
        /// </summary>
        public static List<string> SplitTopLevel(string input, char separator)
        {
            List<string> result = new List<string>();
            if (input == null) return result;
            int depth = 0;
            bool inQuote = false;
            StringBuilder current = new StringBuilder();
            foreach (char c in input)
            {
                if (c == '\'') inQuote = !inQuote;
                if (!inQuote)
                {
                    if (c == '(') depth++;
                    else if (c == ')') depth--;
                }
                if (c == separator && depth == 0 && !inQuote)
                {
                    result.Add(current.ToString().Trim());
                    current.Length = 0;
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(current.ToString().Trim());
            return result;
        }

        /// <summary>
        /// Lấy tên các định danh (field/key) xuất hiện trong biểu thức, bỏ từ khóa và tên hàm
        /// </summary>
        public static List<string> GetIdentifiers(string expression)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(expression)) return result;
            string stripped = Regex.Replace(expression, @"'[^']*'", "''");
            foreach (Match m in identifierRegex.Matches(stripped))
            {
                string name = m.Groups[1].Value;
                if (keywords.Contains(name)) continue;
                if (!result.Contains(name, StringComparer.OrdinalIgnoreCase)) result.Add(name);
            }
            return result;
        }

        /// <summary>
        /// Lấy cả tên nằm trong lời gọi hàm gộp: sum(List, expr, filter) -> List + các field trong expr/filter
        /// </summary>
        public static List<string> GetAllIdentifiers(string expression)
        {
            List<string> result = GetIdentifiers(expression);
            if (string.IsNullOrEmpty(expression)) return result;
            foreach (Match m in aggregateCallRegex.Matches(expression))
            {
                int open = m.Index + m.Length - 1;
                int close = FindMatchingParen(expression, open);
                if (close < 0) continue;
                foreach (var name in GetIdentifiers(expression.Substring(open + 1, close - open - 1)))
                {
                    if (!result.Contains(name, StringComparer.OrdinalIgnoreCase)) result.Add(name);
                }
                var args = SplitTopLevel(expression.Substring(open + 1, close - open - 1), ',');
                if (args.Count > 0 && !string.IsNullOrEmpty(args[0]) && !result.Contains(args[0], StringComparer.OrdinalIgnoreCase)) result.Add(args[0]);
            }
            return result;
        }

        /// <summary>
        /// Bỏ các lời gọi hàm gộp khỏi biểu thức (thay bằng 0), trả thêm tên danh sách chúng dùng.
        /// Dùng để biết biểu thức còn tham chiếu tên nào ở mức ngoài.
        /// </summary>
        public static string StripAggregateCalls(string expression, List<string> listNames)
        {
            if (string.IsNullOrEmpty(expression)) return expression;
            var matches = aggregateCallRegex.Matches(expression).Cast<Match>().OrderByDescending(m => m.Index).ToList();
            string result = expression;
            foreach (var m in matches)
            {
                int open = m.Index + m.Length - 1;
                int close = FindMatchingParen(result, open);
                if (close < 0) continue;
                var args = SplitTopLevel(result.Substring(open + 1, close - open - 1), ',');
                if (listNames != null && args.Count > 0 && !string.IsNullOrEmpty(args[0]) && !listNames.Contains(args[0], StringComparer.OrdinalIgnoreCase)) listNames.Add(args[0]);
                result = result.Substring(0, m.Index) + "(0)" + result.Substring(close + 1);
            }
            return result;
        }

        static int FindMatchingParen(string s, int openIndex)
        {
            int depth = 0;
            bool inQuote = false;
            for (int i = openIndex; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\'') inQuote = !inQuote;
                if (inQuote) continue;
                if (c == '(') depth++;
                else if (c == ')')
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Thay các lời gọi sum(List, expr, filter) / count(List, , filter) trong biểu thức bằng giá trị số đã tính
        /// </summary>
        public static string ReplaceAggregateCalls(string expression, Func<string, IEnumerable> listLookup)
        {
            if (string.IsNullOrEmpty(expression) || listLookup == null) return expression;
            var matches = aggregateCallRegex.Matches(expression).Cast<Match>().OrderByDescending(m => m.Index).ToList();
            string result = expression;
            foreach (var m in matches)
            {
                int open = m.Index + m.Length - 1;
                int close = FindMatchingParen(result, open);
                if (close < 0) continue;
                var args = SplitTopLevel(result.Substring(open + 1, close - open - 1), ',');
                if (args.Count == 0 || string.IsNullOrEmpty(args[0])) continue;
                IEnumerable list = listLookup(args[0]);
                if (list == null) continue;
                decimal? value = Aggregate(m.Groups[1].Value, list, args.Count > 1 ? args[1] : null, args.Count > 2 ? args[2] : null);
                string literal = "(" + (value ?? 0m).ToString(CultureInfo.InvariantCulture) + ")";
                result = result.Substring(0, m.Index) + literal + result.Substring(close + 1);
            }
            return result;
        }

        #endregion

        #region Evaluate

        /// <summary>
        /// Tính biểu thức đơn. fieldLookup trả giá trị theo tên định danh, listLookup trả danh sách cho hàm gộp.
        /// Trả về decimal / string / bool, hoặc null nếu không tính được.
        /// </summary>
        public static object EvaluateScalar(string expression, Func<string, object> fieldLookup, Func<string, IEnumerable> listLookup)
        {
            if (string.IsNullOrWhiteSpace(expression)) return null;
            string prepared = ReplaceAggregateCalls(expression, listLookup);
            var identifiers = GetIdentifiers(prepared);
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in identifiers)
            {
                values[name] = fieldLookup != null ? fieldLookup(name) : null;
            }
            try
            {
                return EvaluateWithTable(prepared, values, false);
            }
            catch (Exception ex1)
            {
                Logging.LogSystem.Debug(ex1);
                try
                {
                    //Thu lai: ep chuoi so ("0.08") sang so
                    return EvaluateWithTable(prepared, values, true);
                }
                catch (Exception ex2)
                {
                    Logging.LogSystem.Warn("CalcEngine: khong tinh duoc bieu thuc [" + expression + "]: " + ex2.Message);
                    return null;
                }
            }
        }

        static object EvaluateWithTable(string expression, Dictionary<string, object> values, bool parseNumericStrings)
        {
            Dictionary<string, object> normalized = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in values)
            {
                object v = Normalize(pair.Value, parseNumericStrings);
                if (v == null) v = 0m;
                //Ten co dau cham khong dung duoc lam ten cot -> doi thanh A__B trong ca bieu thuc va cot
                string columnName = pair.Key;
                if (columnName.Contains("."))
                {
                    columnName = columnName.Replace(".", "__");
                    expression = Regex.Replace(expression, @"(?<![\w.'])" + Regex.Escape(pair.Key) + @"(?![\w.])", columnName);
                }
                normalized[columnName] = v;
            }
            //Kieu ket qua: thu so truoc, roi chuoi (IIF tra chu), cuoi cung bool (bieu thuc so sanh)
            Exception last = null;
            foreach (Type resultType in new[] { typeof(decimal), typeof(string), typeof(bool) })
            {
                try
                {
                    using (DataTable table = new DataTable())
                    {
                        table.Locale = CultureInfo.InvariantCulture;
                        foreach (var pair in normalized)
                        {
                            table.Columns.Add(pair.Key, pair.Value.GetType());
                        }
                        table.Columns.Add(RESULT_COLUMN, resultType, expression);
                        DataRow row = table.NewRow();
                        foreach (var pair in normalized)
                        {
                            row[pair.Key] = pair.Value;
                        }
                        table.Rows.Add(row);
                        object result = row[RESULT_COLUMN];
                        return result == DBNull.Value ? null : result;
                    }
                }
                catch (Exception ex)
                {
                    last = ex;
                }
            }
            throw last ?? new InvalidOperationException("Khong tinh duoc bieu thuc");
        }

        /// <summary>
        /// Hàm gộp trên danh sách: func = sum | avg | min | max | count.
        /// expression: tên field hoặc biểu thức theo field của dòng (vd PRICE*AMOUNT); count không cần expression.
        /// filter: điều kiện lọc dòng theo cú pháp DataTable (vd AMOUNT>0 AND IS_HOME_PRES=1).
        /// </summary>
        public static decimal? Aggregate(string func, IEnumerable list, string expression, string filter)
        {
            if (list == null) return null;
            func = (func ?? "sum").Trim().ToLowerInvariant();
            string functionName;
            switch (func)
            {
                case "sum": functionName = "Sum"; break;
                case "avg": functionName = "Avg"; break;
                case "min": functionName = "Min"; break;
                case "max": functionName = "Max"; break;
                case "count": functionName = "Count"; break;
                default: throw new ArgumentException("Ham gop khong ho tro: " + func);
            }
            if (!string.IsNullOrWhiteSpace(filter)) filter = filter.Trim(); else filter = null;
            using (DataTable table = BuildTable(list))
            {
                string target = null;
                if (!string.IsNullOrWhiteSpace(expression))
                {
                    expression = expression.Trim();
                    if (table.Columns.Contains(expression))
                    {
                        target = expression;
                    }
                    else
                    {
                        table.Columns.Add(CALC_COLUMN, typeof(decimal), expression);
                        target = CALC_COLUMN;
                    }
                }
                if (target == null)
                {
                    if (func == "count") return table.Select(filter ?? "").Length;
                    throw new ArgumentException("Ham " + func + " can bieu thuc hoac ten field");
                }
                object result = table.Compute(string.Format("{0}([{1}])", functionName, target), filter);
                if (result == null || result == DBNull.Value) return 0m;
                return Convert.ToDecimal(result, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Dựng DataTable từ danh sách object (POCO) hoặc danh sách IDictionary. Bỏ qua field là danh sách con.
        /// </summary>
        static DataTable BuildTable(IEnumerable list)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.InvariantCulture;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, Type> columnTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            List<string> columnOrder = new List<string>();
            foreach (object item in list)
            {
                if (item == null) continue;
                Dictionary<string, object> row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in GetFields(item))
                {
                    object v = Normalize(pair.Value, false);
                    if (pair.Value != null && v == null) continue; //danh sach con, object con
                    row[pair.Key] = v;
                    if (!columnTypes.ContainsKey(pair.Key))
                    {
                        columnTypes[pair.Key] = null;
                        columnOrder.Add(pair.Key);
                    }
                    if (v != null && columnTypes[pair.Key] == null) columnTypes[pair.Key] = v.GetType();
                }
                rows.Add(row);
            }
            foreach (string column in columnOrder)
            {
                table.Columns.Add(column, columnTypes[column] ?? typeof(decimal));
            }
            foreach (var row in rows)
            {
                DataRow dataRow = table.NewRow();
                foreach (var pair in row)
                {
                    if (pair.Value == null) continue;
                    try
                    {
                        dataRow[pair.Key] = Convert.ChangeType(pair.Value, table.Columns[pair.Key].DataType, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        dataRow[pair.Key] = DBNull.Value;
                    }
                }
                table.Rows.Add(dataRow);
            }
            return table;
        }

        /// <summary>
        /// Liệt kê field của một dòng: IDictionary hoặc public property của object
        /// </summary>
        public static IEnumerable<KeyValuePair<string, object>> GetFields(object item)
        {
            if (item == null) yield break;
            var genericDic = item as IDictionary<string, object>;
            if (genericDic != null)
            {
                foreach (var pair in genericDic) yield return pair;
                yield break;
            }
            var dic = item as IDictionary;
            if (dic != null)
            {
                foreach (DictionaryEntry entry in dic) yield return new KeyValuePair<string, object>(Convert.ToString(entry.Key), entry.Value);
                yield break;
            }
            foreach (var pi in item.GetType().GetProperties())
            {
                if (!pi.CanRead || pi.GetIndexParameters().Length > 0) continue;
                object value;
                try { value = pi.GetValue(item, null); }
                catch { continue; }
                yield return new KeyValuePair<string, object>(pi.Name, value);
            }
        }

        /// <summary>
        /// Chuẩn hóa giá trị về decimal / string / bool / DateTime. Trả null nếu là null hoặc kiểu không dùng được (danh sách, object).
        /// </summary>
        public static object Normalize(object value, bool parseNumericStrings)
        {
            if (value == null || value == DBNull.Value) return null;
            if (value is string)
            {
                string s = (string)value;
                decimal d;
                if (parseNumericStrings && decimal.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;
                return s;
            }
            if (value is bool) return value;
            if (value is System.DateTime) return value;
            if (value is decimal) return value;
            if (value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint
                || value is long || value is ulong || value is float || value is double)
            {
                return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
            if (value is IEnumerable) return null;
            if (value.GetType().IsPrimitive || value is Guid || value is Enum) return value.ToString();
            return null;
        }

        #endregion
    }
}
