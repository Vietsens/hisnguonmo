using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    internal class StorageApiClient
    {
        public static T CreateRequest<T>(string baseUri, string requestUri, object sendData, string contentType)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    if (!baseUri.EndsWith("/"))
                        baseUri += "/";


                    if (requestUri.StartsWith("/"))
                        requestUri = requestUri.Substring(1);

                    string fullUrl = baseUri + requestUri;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", contentType);
                    client.Timeout = new TimeSpan(0, 0, 90);

                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);

                    HttpResponseMessage resp = client.PostAsync(fullUrl, new StringContent(sendJsonData, Encoding.UTF8, contentType)).Result;

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp == null ? 0 : (int)resp.StatusCode;
                        throw new Exception(string.Format("Lỗi khi gọi API: {0}. StatusCode: {1}", fullUrl, statusCode));
                    }

                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    Inventec.Common.Logging.LogSystem.Info("api responseData: " + responseData);

                    T data = JsonConvert.DeserializeObject<T>(responseData);
                    if (data == null)
                    {
                        throw new Exception(string.Format("Lỗi khi gọi API. Response {0}:", responseData));
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                throw;

            }

        }
    }
}
