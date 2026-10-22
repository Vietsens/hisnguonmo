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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Inventec.Common.TemplaterExport
{
    /// <summary>
    /// Bọc một dòng dữ liệu để Templater đưa nguyên dòng vào FuncCalc/FuncAggregate
    /// (Templater không đưa IDictionary vào hàm metadata, chỉ đưa object thường).
    /// </summary>
    public class RowContext
    {
        [JsonIgnore]
        public IDictionary<string, object> Fields { get; private set; }

        public RowContext(IDictionary<string, object> fields)
        {
            this.Fields = fields;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Hàm tính toán cho template Word, thay cho công thức Excel / FlexCel Aggregate, Evaluate:
    ///   [[Key]:FuncCalc(bieu_thuc, so_le, convert)]                      — bieu_thuc dùng VALUE (giá trị tag), field của dòng, key đơn, sum(List,expr,filter)
    ///   [[Key]:FuncAggregate(sum|avg|min|max|count, List, expr, filter, so_le, convert)]
    /// so_le: số chữ số thập phân, hoặc "speech" để đọc thành chữ. convert = 1: đổi dấu phẩy/chấm kiểu VN.
    /// Chỉ dùng dạng ngoặc vì biểu thức có dấu trừ. Không được chứa dấu ':' và ']'.
    /// Tag có thể không tồn tại trong dữ liệu: khi đó được tính ở bước cuối trước khi xuất file (ProcessCalcTags).
    /// </summary>
    public partial class Store
    {
        const string FUNC_CALC = "FuncCalc";
        const string FUNC_AGGREGATE = "FuncAggregate";
        const string CALC_VALUE_KEYWORD = "VALUE";
        const string CALC_SPEECH = "speech";

        /// <summary>
        /// Danh sách đã bind theo tên (key truyền vào AddObjectData) để FuncAggregate / sum(List,...) tra được
        /// </summary>
        private Dictionary<string, IEnumerable> listRegistry = new Dictionary<string, IEnumerable>(StringComparer.OrdinalIgnoreCase);

        internal class CalcTagInfo
        {
            public string Tag { get; set; }
            public string Metadata { get; set; }
            /// <summary>Mọi tên biểu thức dùng, kể cả field bên trong sum(List, expr) — để so khớp với dòng danh sách</summary>
            public List<string> Identifiers { get; set; }
            /// <summary>Tên ở mức ngoài + tên danh sách — phải tra được hết thì mới tính ngay ở bước key đơn</summary>
            public List<string> OuterIdentifiers { get; set; }
            public bool IsAggregate { get; set; }
        }

        #region Registry

        public void RegisterList(string key, IEnumerable data)
        {
            if (string.IsNullOrEmpty(key) || data == null) return;
            this.listRegistry[key] = data;
        }

        private void ResetCalcState()
        {
            this.listRegistry = new Dictionary<string, IEnumerable>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Router

        private bool TryCalcFunction(object arg1, string arg2, out object result)
        {
            result = arg1;
            if (!IsCalcMetadata(arg2)) return false;
            result = arg2.StartsWith(FUNC_AGGREGATE) ? Aggregate(arg1, arg2) : Calc(arg1, arg2);
            return true;
        }

        static bool IsCalcMetadata(string metadata)
        {
            //So sanh kem dau "(" de khong nham voi FuncCalculateAge
            return metadata != null && (metadata.StartsWith(FUNC_CALC + "(") || metadata.StartsWith(FUNC_AGGREGATE + "("));
        }

        /// <summary>
        /// Tách tham số dạng FuncX(a,b,c) ở mức ngoài cùng (giữ dấu phẩy nằm trong ngoặc con như IIF(a,b,c))
        /// </summary>
        static List<string> GetCalcParameters(string metadata, string funcName)
        {
            string rest = metadata.Substring(funcName.Length).Trim();
            if (rest.StartsWith("("))
            {
                int end = rest.LastIndexOf(')');
                rest = end > 0 ? rest.Substring(1, end - 1) : rest.Substring(1);
            }
            return CalcEngine.SplitTopLevel(rest, ',');
        }

        #endregion

        #region Functions

        /// <summary>
        /// [[RATIO_VAT]:FuncCalc(VALUE*100,0)]  [[VAT_PERCENT]:FuncCalc(RATIO_VAT*100)]  [[ROW_TOTAL]:FuncCalc(PRICE*AMOUNT-DISCOUNT,2,1)]
        /// </summary>
        object Calc(object argument, string metadata)
        {
            try
            {
                var parameters = GetCalcParameters(metadata, FUNC_CALC);
                string expression = parameters.Count > 0 ? parameters[0] : null;
                string digits = parameters.Count > 1 ? parameters[1] : null;
                string convert = parameters.Count > 2 ? parameters[2] : null;
                IDictionary<string, object> fields;
                object value = ResolveCalcContext(argument, out fields);
                object result = CalcEngine.EvaluateScalar(expression,
                    name => LookupField(name, value, fields),
                    name => LookupList(name, fields));
                return FormatCalcResult(result, digits, convert);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// [[TOTAL]:FuncAggregate(sum,Medicines,PRICE*AMOUNT)]  [[CNT]:FuncAggregate(count,Medicines,,IS_HOME_PRES=1)]
        /// </summary>
        object Aggregate(object argument, string metadata)
        {
            try
            {
                var parameters = GetCalcParameters(metadata, FUNC_AGGREGATE);
                string func = parameters.Count > 0 ? parameters[0] : "sum";
                string listName = parameters.Count > 1 ? parameters[1] : null;
                string expression = parameters.Count > 2 ? parameters[2] : null;
                string filter = parameters.Count > 3 ? parameters[3] : null;
                string digits = parameters.Count > 4 ? parameters[4] : null;
                string convert = parameters.Count > 5 ? parameters[5] : null;
                IDictionary<string, object> fields;
                ResolveCalcContext(argument, out fields);
                IEnumerable list = LookupList(listName, fields);
                if (list == null)
                {
                    Logging.LogSystem.Warn("FuncAggregate: khong tim thay danh sach [" + listName + "] (danh sach phai bind qua AddObjectData truoc khi xuat file)");
                    return null;
                }
                decimal? result = CalcEngine.Aggregate(func, list, expression, filter);
                return FormatCalcResult(result, digits, convert);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Xác định ngữ cảnh của tag: RowContext → field của dòng; object thường → public property; giá trị đơn → VALUE
        /// </summary>
        static object ResolveCalcContext(object argument, out IDictionary<string, object> fields)
        {
            fields = null;
            if (argument == null) return null;
            var context = argument as RowContext;
            if (context != null)
            {
                fields = context.Fields;
                return null;
            }
            var dictionary = argument as IDictionary<string, object>;
            if (dictionary != null)
            {
                fields = dictionary;
                return null;
            }
            if (argument is string || argument.GetType().IsPrimitive || argument is decimal || argument is System.DateTime || argument is Enum)
            {
                return argument;
            }
            if (argument is IEnumerable)
            {
                return null;
            }
            fields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in CalcEngine.GetFields(argument))
            {
                fields[pair.Key] = pair.Value;
            }
            return null;
        }

        /// <summary>
        /// Tra giá trị theo tên: VALUE → giá trị tag; field của dòng; key đơn. Tên có dấu chấm (Pat.AGE) đi vào object con.
        /// </summary>
        object LookupField(string name, object value, IDictionary<string, object> fields)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (string.Equals(name, CALC_VALUE_KEYWORD, StringComparison.OrdinalIgnoreCase)) return value;
            object found;
            if (TryGetIgnoreCase(fields, name, out found)) return found;
            if (TryGetIgnoreCase(this.DictionaryTemplateKey, name, out found)) return found;
            if (name.Contains("."))
            {
                string[] parts = name.Split('.');
                object current = LookupField(parts[0], value, fields);
                for (int i = 1; i < parts.Length && current != null; i++)
                {
                    current = GetMember(current, parts[i]);
                }
                return current;
            }
            return null;
        }

        /// <summary>
        /// Tra danh sách theo tên: field của dòng; registry (key AddObjectData); key đơn; tên có dấu chấm đi vào object con.
        /// </summary>
        IEnumerable LookupList(string name, IDictionary<string, object> fields)
        {
            if (string.IsNullOrEmpty(name)) return null;
            object found;
            if (TryGetIgnoreCase(fields, name, out found) && found is IEnumerable && !(found is string)) return (IEnumerable)found;
            IEnumerable list;
            if (this.listRegistry.TryGetValue(name, out list)) return list;
            if (TryGetIgnoreCase(this.DictionaryTemplateKey, name, out found) && found is IEnumerable && !(found is string)) return (IEnumerable)found;
            if (name.Contains("."))
            {
                found = LookupField(name, null, fields);
                if (found is IEnumerable && !(found is string)) return (IEnumerable)found;
            }
            return null;
        }

        static object GetMember(object owner, string memberName)
        {
            if (owner == null || string.IsNullOrEmpty(memberName)) return null;
            var dictionary = owner as IDictionary<string, object>;
            object found;
            if (dictionary != null) return TryGetIgnoreCase(dictionary, memberName, out found) ? found : null;
            foreach (var pair in CalcEngine.GetFields(owner))
            {
                if (string.Equals(pair.Key, memberName, StringComparison.OrdinalIgnoreCase)) return pair.Value;
            }
            return null;
        }

        static bool TryGetIgnoreCase(IDictionary<string, object> dictionary, string name, out object value)
        {
            value = null;
            if (dictionary == null || name == null) return false;
            if (dictionary.TryGetValue(name, out value)) return true;
            string key = dictionary.Keys.FirstOrDefault(k => string.Equals(k, name, StringComparison.OrdinalIgnoreCase));
            if (key == null) return false;
            value = dictionary[key];
            return true;
        }

        /// <summary>
        /// Định dạng kết quả: không tham số → số gọn (8, 12.5); so_le → NumberToString(d, n) (+ convert=1 đổi dấu); "speech" → đọc thành chữ
        /// </summary>
        static string FormatCalcResult(object result, string digits, string convert)
        {
            if (result == null) return string.Empty;
            if (result is decimal)
            {
                decimal d = (decimal)result;
                if (!string.IsNullOrEmpty(digits))
                {
                    if (digits.Equals(CALC_SPEECH, StringComparison.OrdinalIgnoreCase))
                    {
                        decimal rounded = Math.Round(d, 0, MidpointRounding.AwayFromZero);
                        return Inventec.Common.String.Convert.CurrencyToVneseString(rounded.ToString("0", CultureInfo.InvariantCulture));
                    }
                    int numberDigit;
                    if (int.TryParse(digits, out numberDigit))
                    {
                        string text = Inventec.Common.Number.Convert.NumberToString(d, numberDigit);
                        if (convert == "1")
                        {
                            text = text.Replace(",", "_");
                            text = text.Replace(".", ",");
                            text = text.Replace("_", ".");
                        }
                        return text;
                    }
                }
                return d.ToString("0.############", CultureInfo.InvariantCulture);
            }
            return result.ToString();
        }

        #endregion

        #region Tag scan / inject / final pass

        /// <summary>
        /// Các tag còn lại trong template có metadata FuncCalc/FuncAggregate, kèm tên field mà biểu thức tham chiếu
        /// </summary>
        internal List<CalcTagInfo> GetCalcTags()
        {
            List<CalcTagInfo> result = new List<CalcTagInfo>();
            try
            {
                if (this.templateDoc == null) return result;
                var templater = this.templateDoc.Templater;
                foreach (var tag in templater.Tags.ToList())
                {
                    int i = 0;
                    string[] metadata;
                    while ((metadata = templater.GetMetadata(tag, i)) != null)
                    {
                        foreach (var m in metadata)
                        {
                            if (!IsCalcMetadata(m)) continue;
                            if (result.Any(r => r.Tag == tag && r.Metadata == m)) continue;
                            bool isAggregate = m.StartsWith(FUNC_AGGREGATE);
                            var parameters = GetCalcParameters(m, isAggregate ? FUNC_AGGREGATE : FUNC_CALC);
                            List<string> identifiers = new List<string>();
                            List<string> outerIdentifiers = new List<string>();
                            if (isAggregate)
                            {
                                if (parameters.Count > 1 && !string.IsNullOrEmpty(parameters[1]))
                                {
                                    identifiers.Add(parameters[1]);
                                    outerIdentifiers.Add(parameters[1]);
                                }
                                if (parameters.Count > 2) identifiers.AddRange(CalcEngine.GetIdentifiers(parameters[2]));
                                if (parameters.Count > 3) identifiers.AddRange(CalcEngine.GetIdentifiers(parameters[3]));
                            }
                            else
                            {
                                string expression = parameters.Count > 0 ? parameters[0] : string.Empty;
                                identifiers = CalcEngine.GetAllIdentifiers(expression);
                                List<string> listNames = new List<string>();
                                outerIdentifiers = CalcEngine.GetIdentifiers(CalcEngine.StripAggregateCalls(expression, listNames));
                                outerIdentifiers.AddRange(listNames);
                            }
                            result.Add(new CalcTagInfo { Tag = tag, Metadata = m, Identifiers = identifiers, OuterIdentifiers = outerIdentifiers, IsAggregate = isAggregate });
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Dữ liệu key đơn đưa vào Templater: bản sao có thêm RowContext cho tag FuncCalc tham chiếu key đơn.
        /// Không sửa dictionary gốc của processor (tránh RowContext lọt vào DictionaryTemplateKey / export JSON).
        /// </summary>
        internal IDictionary<string, object> PrepareCalcData(IDictionary<string, object> data)
        {
            try
            {
                var calcTags = GetCalcTags();
                if (calcTags.Count == 0) return data;
                var copy = new Dictionary<string, object>(data);
                //Chi tinh ngay khi moi ten deu tra duoc; tag con tham chieu field cua danh sach bind sau thi de lai cho buoc danh sach / buoc cuoi
                InjectRowContexts(copy, calcTags, false, true);
                return copy;
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return data;
        }

        /// <summary>
        /// Gắn RowContext (tên = tag) vào dòng IDictionary mà tag FuncCalc/FuncAggregate tham chiếu field của dòng đó.
        /// recursive = true: đi xuống cả object con và danh sách con.
        /// </summary>
        internal void InjectRowContexts(IDictionary<string, object> row, List<CalcTagInfo> calcTags, bool recursive)
        {
            InjectRowContexts(row, calcTags, recursive, false);
        }

        /// <summary>
        /// requireAllResolvable = false (dòng danh sách): gắn khi biểu thức có ít nhất 1 field của dòng.
        /// requireAllResolvable = true (key đơn): gắn khi mọi tên ở mức ngoài đều tra được (dòng, key đơn, danh sách đã bind).
        /// </summary>
        internal void InjectRowContexts(IDictionary<string, object> row, List<CalcTagInfo> calcTags, bool recursive, bool requireAllResolvable)
        {
            if (row == null || calcTags == null || calcTags.Count == 0) return;
            if (recursive)
            {
                foreach (var key in row.Keys.ToList())
                {
                    object child = row[key];
                    if (child == null || child is string || child is RowContext) continue;
                    var childDictionary = child as IDictionary<string, object>;
                    if (childDictionary != null)
                    {
                        InjectRowContexts(childDictionary, calcTags, true, requireAllResolvable);
                        continue;
                    }
                    var childList = child as IEnumerable;
                    if (childList == null) continue;
                    foreach (var item in childList)
                    {
                        InjectRowContexts(item as IDictionary<string, object>, calcTags, true, requireAllResolvable);
                    }
                }
            }
            foreach (var info in calcTags)
            {
                if (row.Keys.Any(k => string.Equals(k, info.Tag, StringComparison.OrdinalIgnoreCase))) continue;
                bool inject;
                if (requireAllResolvable)
                {
                    inject = info.OuterIdentifiers.Count > 0 && info.OuterIdentifiers.All(id => IsResolvable(id, row));
                }
                else
                {
                    //So khop theo phan dau cua ten (Pat.Items -> Pat)
                    inject = info.Identifiers.Any(id => ContainsKeyIgnoreCase(row, id.Split('.')[0]));
                }
                if (inject)
                {
                    row[info.Tag] = new RowContext(row);
                }
            }
        }

        bool IsResolvable(string identifier, IDictionary<string, object> row)
        {
            if (string.IsNullOrEmpty(identifier)) return false;
            if (string.Equals(identifier, CALC_VALUE_KEYWORD, StringComparison.OrdinalIgnoreCase)) return true;
            string head = identifier.Split('.')[0];
            return ContainsKeyIgnoreCase(row, head) || ContainsKeyIgnoreCase(this.DictionaryTemplateKey, head) || this.listRegistry.ContainsKey(head);
        }

        static bool ContainsKeyIgnoreCase(IDictionary<string, object> dictionary, string key)
        {
            if (dictionary == null || key == null) return false;
            return dictionary.Keys.Any(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Đổi danh sách object sang danh sách IDictionary (qua JSON, giống AddDynamicObjectData) để gắn được RowContext
        /// </summary>
        internal static IDictionary<string, object>[] ToDictionaryRows(IEnumerable data)
        {
            string json = JsonConvert.SerializeObject(data);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new DictionaryConverter());
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                return serializer.Deserialize<IDictionary<string, object>[]>(reader);
            }
        }

        /// <summary>
        /// Bước cuối trước khi xuất file: tính các tag FuncCalc/FuncAggregate chưa được xử lý (key không có trong dữ liệu)
        /// </summary>
        internal void ProcessCalcTags()
        {
            try
            {
                if (this.templateDoc == null) return;
                var templater = this.templateDoc.Templater;
                foreach (var tag in templater.Tags.ToList())
                {
                    int i = 0;
                    int guard = 0;
                    string[] metadata;
                    while ((metadata = templater.GetMetadata(tag, i)) != null && guard++ < 100000)
                    {
                        string calc = metadata.FirstOrDefault(IsCalcMetadata);
                        if (calc == null)
                        {
                            i++;
                            continue;
                        }
                        object value = null;
                        TryGetIgnoreCase(this.DictionaryTemplateKey, tag, out value);
                        object result = calc.StartsWith(FUNC_AGGREGATE) ? Aggregate(value, calc) : Calc(value, calc);
                        templater.Replace(tag, i, result == null ? string.Empty : result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
