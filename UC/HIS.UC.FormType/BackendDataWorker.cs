using ACS.Filter;
using HIS.UC.FormType.Base;
using Inventec.Common.Adapter;
using Inventec.Core;
using SDA.Filter;
using System;
using System.Collections.Generic;

namespace HIS.UC.FormType
{
    public class BackendDataWorker
    {
        private static Dictionary<Type, object> dic = new Dictionary<Type, object>();
        private static Object thisLock = new Object();

        public static List<T> Get<T>(string uri,Inventec.Common.WebApiClient.ApiConsumer Consummer,object filter) where T : class
        {
            object result = null;
            try
            {
						CommonParam param = new CommonParam();
                Type type = typeof(T);
                lock (thisLock)
                {
                    if (filter != null)
                    {
										result = new BackendAdapter(param).Get<List<T>>(uri,Consummer,filter,param);
                    }
                    else
                    {
                        if (!dic.TryGetValue(type, out result))
                        {
												result = new BackendAdapter(param).Get<List<T>>(uri,Consummer,filter,param);
                            dic.Add(type, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return (List<T>)result;
        }

        public static List<T> Get<T>() where T : class
        {
            object result = null;
            try
            {
                Type type = typeof(T);
                lock (thisLock)
                {
                    if (!dic.TryGetValue(type, out result))
                    {
										result = null;
                        dic.Add(type, result);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return (List<T>)result;
        }

      

        public static bool Reset<T>() where T : class
        {
            bool result = false;
            try
            {
                Type type = typeof(T);
                lock (thisLock)
                {
                    result = dic.Remove(type);
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static bool ResetAll()
        {
            bool result = false;
            try
            {
                foreach (var item in dic)
                {
                    dic.Remove(item.Key);
                }
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

    }
}
