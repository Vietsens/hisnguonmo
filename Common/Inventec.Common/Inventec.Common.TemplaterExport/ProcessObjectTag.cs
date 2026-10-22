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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.TemplaterExport
{
    public class ProcessObjectTag
    {
        //private Dictionary<string, object> DicData = new Dictionary<string, object>();
        public bool AddObjectData<T>(Store store, string key, List<T> data)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (data == null) throw new ArgumentNullException("data");
                store.RegisterList(key, data);
                if (!ProcessWithCalc(store, data))
                {
                    store.templateDoc.Process(data);
                }

                AddObjectKeyIntoListkey1<T>(store, key, data);

                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool AddObjectData<T>(Store store, T data)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (data == null) throw new ArgumentNullException("data");

                IList dataList = data as IList;
                if (dataList != null)
                {
                    store.RegisterList(GetElementTypeName(dataList), dataList);
                    if (!ProcessWithCalc(store, dataList))
                    {
                        store.templateDoc.Process(data);
                    }
                }
                else
                {
                    store.templateDoc.Process(data);
                }

                if (data is IList)
                {
                    AddObjectKeyIntoListkey<T>(store, "", data);
                }

                //DicData[key] = data;

                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public bool AddDynamicObjectData<T>(Store store, string key, T data)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (data == null) throw new ArgumentNullException("data");

                if (data is System.Data.DataTable || data is System.Data.DataSet || data is IDictionary<string, object> || data is IDictionary)
                {
                    store.templateDoc.Process(data);
                }
                else
                {
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    string dataPath = Utils.GenerateTempFileWithin("", ".docx");
                    File.WriteAllText(dataPath, jsonString);

                    var json = new StreamReader(Utils.Open(dataPath));

                    var newtonsoft = new Newtonsoft.Json.JsonSerializer();
                    newtonsoft.Converters.Add(new DictionaryConverter());

                    while (char.IsWhiteSpace((char)json.Peek()))
                    {
                        json.Read();
                    }

                    var calcTags = store.GetCalcTags();
                    if (json.Peek() == '[')
                    {
                        var deser = newtonsoft.Deserialize<IDictionary<string, object>[]>(new JsonTextReader(json));
                        store.RegisterList(key, deser);
                        foreach (var row in deser)
                        {
                            store.InjectRowContexts(row, calcTags, true);
                        }
                        store.templateDoc.Process(deser);
                    }
                    else
                    {
                        var deser = newtonsoft.Deserialize<IDictionary<string, object>>(new JsonTextReader(json));
                        foreach (var pair in deser.ToList())
                        {
                            if (pair.Value is IEnumerable && !(pair.Value is string) && !(pair.Value is IDictionary<string, object>))
                            {
                                store.RegisterList(pair.Key, (IEnumerable)pair.Value);
                            }
                        }
                        store.InjectRowContexts(deser, calcTags, true);
                        store.templateDoc.Process(deser);
                    }


                    AddObjectKeyIntoListkey<T>(store, key, data);

                    //DicData[key] = data;
                    try
                    {
                        if (File.Exists(dataPath))
                            File.Delete(dataPath);
                    }
                    catch { }
                }
                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Nếu template có tag FuncCalc/FuncAggregate tham chiếu field của dòng thì đổi dòng sang IDictionary,
        /// gắn RowContext rồi Process. Trả false để Process như cũ khi không cần.
        /// </summary>
        private bool ProcessWithCalc(Store store, IEnumerable data)
        {
            try
            {
                var calcTags = store.GetCalcTags();
                if (calcTags.Count == 0) return false;
                object first = data.Cast<object>().FirstOrDefault(o => o != null);
                if (first == null) return false;
                var fieldNames = CollectFieldNames(first, 0);
                if (!calcTags.Any(t => t.Identifiers.Any(id => fieldNames.Contains(id)))) return false;
                var rows = Store.ToDictionaryRows(data);
                if (rows == null) return false;
                foreach (var row in rows)
                {
                    store.InjectRowContexts(row, calcTags, true);
                }
                store.templateDoc.Process(rows);
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Tên field của object và của phần tử danh sách con (tối đa 2 cấp)
        /// </summary>
        private static HashSet<string> CollectFieldNames(object item, int depth)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (item == null || depth > 2) return names;
            foreach (var pair in CalcEngine.GetFields(item))
            {
                names.Add(pair.Key);
                object child = pair.Value;
                if (child == null || child is string || child is IDictionary) continue;
                var list = child as IEnumerable;
                if (list == null) continue;
                object firstChild = list.Cast<object>().FirstOrDefault(o => o != null);
                if (firstChild != null) names.UnionWith(CollectFieldNames(firstChild, depth + 1));
            }
            return names;
        }

        private static string GetElementTypeName(IList list)
        {
            Type type = list.GetType();
            if (type.IsGenericType && type.GetGenericArguments().Length == 1) return type.GetGenericArguments()[0].Name;
            object first = list.Cast<object>().FirstOrDefault(o => o != null);
            return first != null ? first.GetType().Name : type.Name;
        }

        protected void AddObjectKeyIntoListkey<T>(Store store, string key, T data)
        {
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                //if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (data == null) return;
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                System.Reflection.PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (pi.GetGetMethod().IsVirtual) continue;

                        string keyName = string.Format("{0}{1}", (System.String.IsNullOrEmpty(key) ? "" : key + "."), pi.Name);

                        if (!store.DictionaryTemplateKey.ContainsKey(keyName))
                        {
                            store.DictionaryTemplateKey.Add(keyName, (data != null ? pi.GetValue(data) : null));
                        }
                        else
                        {
                            store.DictionaryTemplateKey[keyName] = (data != null ? pi.GetValue(data) : null);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void AddObjectKeyIntoListkey1<T>(Store store, string key, List<T> listData)
        {
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (listData == null || listData.Count == 0) return;
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();
                Dictionary<string, List<object>> DicValue = new Dictionary<string, List<object>>();
                GetFieldValues<T>(listData, key, ref DicValue);

                foreach (var item in DicValue)
                {
                    string value = string.Join("\n", item.Value);
                    if (!store.DictionaryTemplateKey.ContainsKey(item.Key))
                    {
                        store.DictionaryTemplateKey.Add(item.Key, value);
                    }
                    else
                    {
                        store.DictionaryTemplateKey[item.Key] = value;
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void GetFieldValues<T>(List<T> listData, string key, ref Dictionary<string, List<object>> DicValue)
        {
            try
            {
                foreach (T item in listData)
                {
                    System.Reflection.PropertyInfo[] pis = typeof(T).GetProperties();
                    if (pis != null && pis.Length > 0)
                    {
                        foreach (var pi in pis)
                        {
                            if (pi.GetGetMethod().IsVirtual) continue;

                            string keyName = string.Format("{0}{1}", (System.String.IsNullOrEmpty(key) ? "" : key + "."), pi.Name);

                            if (!DicValue.ContainsKey(keyName))
                            {
                                DicValue[keyName] = new List<object>();
                            }
                            DicValue[keyName].Add(pi.GetValue(item));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
