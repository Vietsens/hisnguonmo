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
using FlexCel.Report;
using Inventec.Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Inventec.Common.FlexCellExport
{
    public class ProcessObjectTag
    {
        private Dictionary<string, object> DicData = new Dictionary<string, object>();

        public bool AddObjectData<T>(Store store, string key, List<T> listData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (listData == null) throw new ArgumentNullException("listData");

                store.flexCel.AddTable(key, listData);

                AddObjectKeyIntoListkey<T>(store, key, listData.FirstOrDefault());

                if (listData.Count > 0)
                {
                    DicData[key] = listData;
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

        public bool AddObjectData(Store store, string key, DataTable listData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (listData == null) throw new ArgumentNullException("listData");

                store.flexCel.AddTable(key, listData);

                if (listData.Rows.Count > 0)
                {
                    DicData[key] = listData;
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

        public bool AddRelationship(Store store, string masterTable, string detailTable, string masterKeyField, string detailKeyField)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(masterTable)) throw new ArgumentNullException("masterTable");
                if (System.String.IsNullOrEmpty(detailTable)) throw new ArgumentNullException("detailTable");
                if (System.String.IsNullOrEmpty(masterKeyField)) throw new ArgumentNullException("masterKeyField");
                if (System.String.IsNullOrEmpty(detailKeyField)) throw new ArgumentNullException("detailKeyField");

                store.flexCel.AddRelationship(masterTable, detailTable, masterKeyField, detailKeyField);

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

        public bool AddRelationship(Store store, string masterTable, string detailTable, string[] masterKeyFields, string[] detailKeyFields)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(masterTable)) throw new ArgumentNullException("masterTable");
                if (System.String.IsNullOrEmpty(detailTable)) throw new ArgumentNullException("detailTable");
                if (masterKeyFields == null) throw new ArgumentNullException("masterKeyFields");
                if (detailKeyFields == null) throw new ArgumentNullException("detailKeyFields");

                store.flexCel.AddRelationship(masterTable, detailTable, masterKeyFields, detailKeyFields);

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

        public bool SetUserFunction(Store store, string functionName, TFlexCelUserFunction tUserFunction)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(functionName)) throw new ArgumentNullException("functionName");
                if (tUserFunction == null) throw new ArgumentNullException("TFlexCelUserFunction");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                store.flexCel.SetUserFunction(functionName, tUserFunction);

                store.DictionaryTemplateKey[functionName] = functionName;

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

        protected void AddObjectKeyIntoListkey<T>(Store store, string key, T data)
        {
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (data == null) return;
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                System.Reflection.PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (pi.GetGetMethod().IsVirtual) continue;

                        string keyName = string.Format("{0}.{1}", key, pi.Name);

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

        public Dictionary<string, object> GetTotalData { get { return DicData; } }

        /// <summary>
        /// Thêm vào danh sách dữ liệu 1 đối tượng không phải dạng danh sách.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="key"></param>
        /// <param name="listData"></param>
        /// <returns></returns>
        public bool OverWriteObjectData(string key, object listData)
        {
            bool result = false;
            try
            {
                if (System.String.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
                if (listData == null) throw new ArgumentNullException("listData");

                DicData[key] = listData;

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
    }
}
