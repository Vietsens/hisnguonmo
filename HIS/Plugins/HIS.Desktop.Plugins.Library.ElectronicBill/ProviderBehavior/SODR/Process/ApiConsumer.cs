using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process
{
    internal class ApiConsumer
    {
        public static T CreateRequest<T>(string fullapi, string token, object objData)
        {
            T result3;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                bool flag = !string.IsNullOrWhiteSpace(token);
                if (flag)
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);
                }
                httpClient.Timeout = new TimeSpan(0, 0, 90);
                string text = JsonConvert.SerializeObject(objData);
                LogSystem.Info("_____sendJsonData : " + text);
                HttpResponseMessage result = httpClient.PostAsync(fullapi, new StringContent(text, Encoding.UTF8, "application/json")).Result;
                bool flag2 = result == null || !result.IsSuccessStatusCode;
                if (flag2)
                {
                    int hashCode = result.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}. StatusCode: {1}", fullapi, hashCode));
                }
                string result2 = result.Content.ReadAsStringAsync().Result;
                LogSystem.Info("__________________api responseData: " + result2);
                T t = default(T);
                try
                {
                    t = JsonConvert.DeserializeObject<T>(result2);
                    bool flag3 = t == null;
                    if (flag3)
                    {
                        throw new Exception(string.Format("Loi khi goi API. Response {0}:", result2));
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    throw new Exception(result2);
                }
                result3 = t;
            }
            return result3;
        }
    }
}
