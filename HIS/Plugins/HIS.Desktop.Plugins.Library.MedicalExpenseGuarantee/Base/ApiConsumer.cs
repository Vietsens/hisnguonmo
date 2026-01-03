using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
using Inventec.Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.Base
{
    public class ApiConsumer
    {
        private string baseUri;
        private string applicationCode;
        private string limet;
        private string cskcbbd;
        private HttpClient _httpClient;

        public ApiConsumer(string baseUri, string applicationCode, string limet, string cskcbbd)
        {
            this.baseUri = baseUri;
            this.applicationCode = applicationCode;
            this.limet = limet;
            this.cskcbbd = cskcbbd;
            _httpClient = new HttpClient();
        }


        public T CreateRequest<T>(string requestUri, object sendData)
        {
            _httpClient.BaseAddress = new Uri(baseUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.Timeout = TimeSpan.FromSeconds(180);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            //_httpClient.DefaultRequestHeaders.Add("HospitalCode", this.cskcbbd);  

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string requestJson = JsonConvert.SerializeObject(sendData);
            LogSystem.Info("requestJson " + requestJson);
            LogSystem.Info("baseUri " + baseUri);
            LogSystem.Info("requestUri " + requestUri);
            HttpResponseMessage resp = null;

            try
            {
                resp = _httpClient.PostAsync(requestUri, new StringContent(requestJson, Encoding.UTF8, "application/json")).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            string responseData = resp.Content.ReadAsStringAsync().Result;
            LogSystem.Info("responseData " + responseData);

            if (resp == null || !resp.IsSuccessStatusCode)
            {
                int statusCode = resp.StatusCode.GetHashCode();
                Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestUri, statusCode));
                Inventec.Common.Logging.LogSystem.Info("________________________sendJsonData : " + requestJson);
                Inventec.Common.Logging.LogSystem.Error("_______________________responseData: " + responseData);
            }

            T data = JsonConvert.DeserializeObject<T>(responseData);
            if (data == null)
            {
                throw new Exception(string.Format("Loi khi goi API. Response :{0}", responseData ?? "null"));
            }
            return data;
        }

        public string ConvertSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2")); // x2 = lowercase hex
                }
                return sb.ToString();
            }
        }

        public static string RemoveVietnameseAccent(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string noAccent = RemoveVietnameseAccent(input);
            return noAccent.ToLowerInvariant().Trim();
        }
    }
}
