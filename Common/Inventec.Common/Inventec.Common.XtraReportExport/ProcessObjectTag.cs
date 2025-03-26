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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.XtraReportExport
{
    public class ProcessObjectTag
    {
        public bool AddObjectData<T>(Store store, List<T> data)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.xtraReport == null) throw new ArgumentNullException("store.xtraReport");
                if (data == null) throw new ArgumentNullException("data");

                store.xtraReport.DataSource = data;
                AddObjectKeyIntoListkey<T>(store, "", data.FirstOrDefault());
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
                if (store.xtraReport == null) throw new ArgumentNullException("store.xtraReport");
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
