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

        public static T CreateMultipartRequest<T>(
    string baseUri,
    string requestUri,
    string citizenNumber,
    object fingerprint,
    object faceId,
    object handSignature,
    string apiKey,
    string transactionId,
    string hash)
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

                    client.Timeout = new TimeSpan(0, 0, 90);

                    var form = new MultipartFormDataContent();

                    // ===== text params =====
                    form.Add(new StringContent(citizenNumber), "citizenNumber");
                    form.Add(new StringContent(apiKey), "apiKey");
                    form.Add(new StringContent(transactionId), "transactionId");
                    form.Add(new StringContent(hash), "hash");

                    // ===== file params =====
                    AddFiles(form, "fingerprint", fingerprint);
                    AddFiles(form, "faceId", faceId);
                    AddFiles(form, "handSignature", handSignature);

                    Inventec.Common.Logging.LogSystem.Info("Call multipart API: " + fullUrl);

                    HttpResponseMessage resp = client.PostAsync(fullUrl, form).Result;

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp == null ? 0 : (int)resp.StatusCode;
                        throw new Exception(
                            string.Format("UploadFiles error. StatusCode: {0}", statusCode)
                        );
                    }

                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    Inventec.Common.Logging.LogSystem.Info("Upload response: " + responseData);

                    return JsonConvert.DeserializeObject<T>(responseData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                throw;
            }
        }

        private static void AddFiles(
    MultipartFormDataContent form,
    string paramName,
    object files)
        {
            if (files == null) return;

            if (files is IEnumerable<byte[]> byteFiles)
            {
                int index = 0;
                foreach (var file in byteFiles)
                {
                    var content = new ByteArrayContent(file);
                    content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    string ext = paramName == "handSignature" ? ".png" : ".jpg"; ;
                    string fileName = paramName + "_" + index + ext;
                    form.Add(content, paramName, fileName);
                    index++;
                }
            }
        }

    }
}
