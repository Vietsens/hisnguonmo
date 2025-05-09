﻿using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using FlexCel.XlsAdapter;

namespace Inventec.Common.FlexCellExport
{
    public class ProcessSingleTag
    {
        public bool AddSingleKey(Store store, string keyName, object value)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (System.String.IsNullOrEmpty(keyName)) throw new ArgumentNullException("key");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();
                
                store.flexCel.SetValue(keyName, value);
                //ghi de du lieu trung
                store.DictionaryTemplateKey[keyName] = value;
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

        public bool AddSingleKeyFromObject(Store store, object data)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (data == null) throw new ArgumentNullException("data");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                foreach (var prop in data.GetType().GetProperties())
                {
                    AddSingleKey(store, prop.Name, prop.GetValue(data, null));

                    LogSystem.Debug(System.String.Format("{0}={1}", prop.Name, prop.GetValue(data, null)));
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

        public bool ProcessData(Store store, Dictionary<string, object> dicData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.flexCel == null) throw new ArgumentNullException("store.flexCel");
                if (dicData == null) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                if (dicData.Count > 0)
                {
                    foreach (KeyValuePair<string, object> pair in dicData)
                    {
                        store.flexCel.SetValue(pair.Key, pair.Value);

                        //ghi de du lieu trung
                        store.DictionaryTemplateKey[pair.Key] = pair.Value;
                    }
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
    }
}
