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
        /// <summary>vCong 62522: kho lưu trữ dữ liệu định danh 2ID.</summary>
        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Other("kho lưu trữ định danh 2ID"); }
        }

        /// <summary>Bỏ query string (chứa apiKey/hash) khỏi địa chỉ trước khi đưa vào thông báo/log.</summary>
        private static string StripQuery(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int i = url.IndexOf('?');
            return i >= 0 ? url.Substring(0, i) : url;
        }

        /// <summary>Che giá trị apiKey/hash trong chuỗi form/json trước khi ghi log.</summary>
        private static string MaskSecret(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            text = System.Text.RegularExpressions.Regex.Replace(text, "(apiKey|hash)=[^&]*", "$1=***", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return System.Text.RegularExpressions.Regex.Replace(text, "\"(apiKey|hash)\"\\s*:\\s*\"[^\"]*\"", "\"$1\":\"***\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static T CreateRequest<T>(string baseUri, string requestUri, object sendData, string contentType)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (string.IsNullOrWhiteSpace(baseUri))
                        throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Chưa thiết lập địa chỉ kho lưu trữ định danh 2ID");

                    if (!baseUri.EndsWith("/"))
                        baseUri += "/";


                    if (requestUri.StartsWith("/"))
                        requestUri = requestUri.Substring(1);

                    string fullUrl = baseUri + requestUri;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.Timeout = new TimeSpan(0, 0, 90);

                    HttpContent content = null;
                    
                    // Kiểm tra contentType để quyết định cách gửi dữ liệu
                    if (contentType == "application/x-www-form-urlencoded")
                    {
                        // Gửi dữ liệu dạng form-urlencoded
                        var properties = sendData.GetType().GetProperties();
                        var formData = new List<KeyValuePair<string, string>>();
                        
                        foreach (var prop in properties)
                        {
                            var value = prop.GetValue(sendData);
                            if (value != null)
                            {
                                formData.Add(new KeyValuePair<string, string>(prop.Name, value.ToString()));
                            }
                        }
                        
                        content = new FormUrlEncodedContent(formData);
                        Inventec.Common.Logging.LogSystem.Info("_____sendFormData : " + MaskSecret(string.Join("&", formData.Select(x => $"{x.Key}={x.Value}"))));
                    }
                    else
                    {
                        // Gửi dữ liệu dạng JSON
                        string sendJsonData = JsonConvert.SerializeObject(sendData);
                        Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                        content = new StringContent(sendJsonData, Encoding.UTF8, contentType);
                    }

                    string safeUrl = StripQuery(fullUrl); // vCong 62522: khong dua apiKey/hash vao thong bao
                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.PostAsync(fullUrl, content).Result;
                    }
                    catch (Exception exCall)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(exCall, Partner, safeUrl);
                    }

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp == null ? 0 : (int)resp.StatusCode;
                        string errorContent = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                        Inventec.Common.Logging.LogSystem.Error($"API Error: {safeUrl}, StatusCode: {statusCode}, ErrorContent: {errorContent}");
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, safeUrl, statusCode, resp != null ? resp.ReasonPhrase : "", errorContent);
                    }

                    string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                    Inventec.Common.Logging.LogSystem.Info("api responseData: " + responseData);

                    T data;
                    try
                    {
                        data = JsonConvert.DeserializeObject<T>(responseData);
                    }
                    catch (Exception exParse)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, safeUrl, responseData, exParse);
                    }
                    if (data == null)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, safeUrl, responseData, null);
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
                    if (string.IsNullOrWhiteSpace(baseUri))
                        throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner, "Chưa thiết lập địa chỉ kho lưu trữ định danh 2ID");
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

                    string safeUrl = StripQuery(fullUrl);
                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.PostAsync(fullUrl, form).Result;
                    }
                    catch (Exception exCall)
                    {
                        // vCong 62522: timeout/DNS/SSL/khong ket noi -> exception co nguon
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(exCall, Partner, safeUrl);
                    }

                    if (resp == null || !resp.IsSuccessStatusCode)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, safeUrl, resp);
                    }

                    string responseData = Inventec.Common.IntegrationError.IntegrationHttpError.ReadBody(resp);
                    Inventec.Common.Logging.LogSystem.Info("Upload response: " + responseData);

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseData);
                    }
                    catch (Exception exParse)
                    {
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, safeUrl, responseData, exParse);
                    }
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
                    
                    // Set ContentType theo loại file
                    if (paramName == "handSignature" || paramName == "fingerprint")
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    else
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    string ext = (paramName == "handSignature" || paramName == "fingerprint") ? ".png" : ".jpg";
                    string fileName = paramName + "_" + index + ext;
                    form.Add(content, paramName, fileName);
                    index++;
                }
            }
            else if (files is IEnumerable<string> base64Files)
            {
                int index = 0;
                foreach (var base64 in base64Files)
                {
                    if (string.IsNullOrWhiteSpace(base64)) continue;
                    
                    byte[] file;
                    try
                    {
                        // Loại bỏ prefix base64 nếu có
                        string cleanBase64 = base64
                            .Replace("data:image/png;base64,", "")
                            .Replace("data:image/jpeg;base64,", "")
                            .Replace("data:image/jpg;base64,", "")
                            .Replace("\r", "").Replace("\n", "").Replace(" ", "");
                        
                        file = Convert.FromBase64String(cleanBase64);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Invalid base64 for " + paramName + ": " + ex.Message);
                        continue; // skip invalid base64
                    }
                    
                    var content = new ByteArrayContent(file);
                    
                    // Set ContentType theo loại file
                    if (paramName == "handSignature" || paramName == "fingerprint")
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                    else
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    
                    string ext = (paramName == "handSignature" || paramName == "fingerprint") ? ".png" : ".jpg";
                    string fileName = paramName + "_" + index + ext;
                    form.Add(content, paramName, fileName);
                    index++;
                }
            }
        }

    }
}
