using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.LocalStorage.SdaConfig
{
    public class SdaConfigs
    {
        private static Dictionary<string, object> dic = new Dictionary<string, object>();
        private static Object thisLock = new Object();

        /// <summary>
        /// Lay du lieu cua cau hinh tren sdaconfig theo chuoi key
        /// </summary>
        /// <typeparam name="T">(Gia tri cua mot trong các kieu du lieu: string, int, long, decimal, List of string)</typeparam>
        /// <param name="key">Mot chuoi key trong ConfigKeys tuong ung voi key cau hinh tren sdaconfig</param>
        /// <returns>value</returns>
        public static T Get<T>(string key)
        {
            T result = default(T);
            Type type = typeof(T);
            object data = null;
            lock (thisLock)
            {
                if (!dic.TryGetValue(key, out data))
                {
                    data = ConfigUtil.GetStrConfig(key);
                    dic.Add(key, data);
                }
                if (type == typeof(List<string>))
                {
                    data = ConfigUtil.GetStrConfigs(key);
                }
                else
                {
                    data = ConfigUtil.GetStrConfig(key);
                }
                result = (T)System.Convert.ChangeType(data, typeof(T));
            }
            return result;
        }
    }
}
