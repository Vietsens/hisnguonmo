using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.TemplaterExport
{
    public class ProcessSingleTag
    {
        public bool ProcessData(Store store, Dictionary<string, object> dicData)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (dicData == null) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                if (dicData.Count > 0)
                {
                    store.templateDoc.Process(dicData);
                    foreach (KeyValuePair<string, object> pair in dicData)
                    {
                        if (store.DictionaryTemplateKey.ContainsKey(pair.Key))
                        {
                            //ghi de du lieu trung
                            store.DictionaryTemplateKey[pair.Key] = pair.Value;
                        }
                        else
                        {
                            store.DictionaryTemplateKey.Add(pair.Key, pair.Value);
                        }
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

        public bool ProcessData(Store store, Dictionary<string, object> dicData, bool bCreateObjectContainer)
        {
            bool result = false;
            try
            {
                if (store == null) throw new ArgumentNullException("store");
                if (store.templateDoc == null) throw new ArgumentNullException("store.templateDoc");
                if (dicData == null) throw new ArgumentNullException("dicData");
                if (store.DictionaryTemplateKey == null)
                    store.DictionaryTemplateKey = new Dictionary<string, object>();

                if (dicData.Count > 0)
                {
                    if (bCreateObjectContainer)
                    {
                        store.templateDoc.Process(
                        new
                        {
                            SingleKey = dicData
                        });
                    }
                    else
                    {
                        store.templateDoc.Process(dicData);
                    }

                    foreach (KeyValuePair<string, object> pair in dicData)
                    {
                        if (store.DictionaryTemplateKey.ContainsKey(System.String.Format("{0}{1}", bCreateObjectContainer ? "SingleKey." : "", pair.Key)))
                        {
                            //ghi de du lieu trung
                            store.DictionaryTemplateKey[System.String.Format("{0}{1}", bCreateObjectContainer ? "SingleKey." : "", pair.Key)] = pair.Value;
                        }
                        else
                        {
                            store.DictionaryTemplateKey.Add(System.String.Format("{0}{1}", bCreateObjectContainer ? "SingleKey." : "", pair.Key), pair.Value);
                        }
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
