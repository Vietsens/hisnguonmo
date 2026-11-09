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
                HttpResponseMessage result = null;
                try
                {
                    result = httpClient.PostAsync(fullapi, new StringContent(text, Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    throw Base.ElectronicBillErrorUtil.WrapHttpCall(ex, Base.ProviderType.SoftDream, fullapi);
                }
                if (result == null)
                {
                    throw Base.ElectronicBillErrorUtil.NoResponse(Base.ProviderType.SoftDream, "gọi " + fullapi);
                }
                if (!result.IsSuccessStatusCode)
                {
                    int hashCode = result.StatusCode.GetHashCode();
                    string errorData = "";
                    try { errorData = result.Content != null ? result.Content.ReadAsStringAsync().Result : ""; }
                    catch (Exception exRead) { LogSystem.Warn(exRead); }
                    LogSystem.Error(string.Format("Loi khi goi API: {0}. StatusCode: {1}. Response: {2}", fullapi, hashCode, errorData));
                    throw Base.ElectronicBillErrorUtil.HttpStatusError(Base.ProviderType.SoftDream, fullapi, hashCode, result.ReasonPhrase, errorData);
                }
                string result2 = result.Content.ReadAsStringAsync().Result;
                LogSystem.Info("__________________api responseData: " + result2);
                T t = default(T);
                try
                {
                    t = JsonConvert.DeserializeObject<T>(result2);
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    throw Base.ElectronicBillErrorUtil.ParseError(Base.ProviderType.SoftDream, fullapi, result2, ex);
                }
                if (t == null)
                {
                    throw Base.ElectronicBillErrorUtil.ParseError(Base.ProviderType.SoftDream, fullapi, result2, null);
                }
                result3 = t;
            }
            return result3;
        }
    }
}
