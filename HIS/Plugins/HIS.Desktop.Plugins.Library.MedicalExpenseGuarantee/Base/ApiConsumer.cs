using HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee.ADO;
using Inventec.Common.Logging;
using Inventec.Common.WebApiClient;
using Inventec.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private string hasUri;
        private string acsUri;
        private string applicationCode;
        private string limet;
        private string cskcbbd;
        private string Username;
        private string Password;
        private HttpClient _httpClient;
        private CommonParam common;

        public ApiConsumer(string hasUri, string acsUri, string applicationCode, string limet, string cskcbbd, string user, string pass)
        {
            this.hasUri = hasUri;
            this.acsUri = acsUri;
            this.applicationCode = applicationCode;
            this.limet = limet;
            this.cskcbbd = cskcbbd;
            this.Username = user;
            this.Password = pass;
            _httpClient = new HttpClient();
        }

        private ApiConsumerWrapper emrConsumerWrapper;
        public ApiConsumerWrapper EmrConsumerWrapper
        {
            get
            {
                if (emrConsumerWrapper == null)
                {
                    emrConsumerWrapper = new ApiConsumerWrapper(true, applicationCode, hasUri, acsUri, Username, Password);
                    emrConsumerWrapper.UseRegistry(false);
                }
                return emrConsumerWrapper;
            }
        }


        public T CreateRequest<T>(string requestUri, object sendData)
        {
            _httpClient.BaseAddress = new Uri(hasUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.Timeout = TimeSpan.FromSeconds(180);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                string token = this.EmrConsumerWrapper.GetTokenCode();
                LogSystem.Info("TokenCode" + token);
                if (!string.IsNullOrEmpty(token))
                {
                    if (_httpClient.DefaultRequestHeaders.Contains("TokenCode"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("TokenCode");
                    }
                    _httpClient.DefaultRequestHeaders.Add("TokenCode", token);
                }
                else
                {
                    LogSystem.Warn("EmrConsumerWrapper token is null or empty."); 
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            //_httpClient.DefaultRequestHeaders.Add("HospitalCode", this.cskcbbd);  

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string requestJson = JsonConvert.SerializeObject(sendData);
            LogSystem.Info("requestJson " + requestJson);
            LogSystem.Info("hasUri " + hasUri);
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
                Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.hasUri, requestUri, statusCode));
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

            return Inventec.Common.String.Convert.UnSignVNese2(input).ToLower().Trim();

            //string noAccent = RemoveVietnameseAccent(input);
            //return noAccent.ToLowerInvariant().Trim();
        }

        
    }
}
