/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Token.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Management;
using System.Net;
using System.Web.UI.WebControls;


namespace Inventec.Common.WebApiClient
{
    /// <summary>
    /// Class de goi cac API do server back-end cung cap
    /// </summary>
    public class ApiConsumer
    {
        private const string API_PARAM = "param";
        private const string CLIENT_IP_ADDRESS_PARAM = "ClientIpAddress";
        private int TIME_OUT = int.Parse(ConfigurationManager.AppSettings["Inventec.Common.WebApiClient.Timeout"] ?? "60");//thoi gian timeout tinh theo seconds (60s)
        private string baseUri = null;
        private string token = null;
        private string applicationCode = null;
        private Dictionary<string, string> dicHeaderRequestParam = null;

        /// <summary>
        /// Khoi tao voi tham so truyen vao la base uri cua api do server back-end cung cap
        /// </summary>
        /// <param name="baseUri">BaseUri cua server back-end</param>
        public ApiConsumer(string baseUri, string applicationCode)
        {
            this.Init(baseUri, null, applicationCode);
        }

        /// <summary>
        /// Khoi tao voi tham so truyen vao la base uri cua api do server back-end cung cap va tokenCode cua front-end
        /// </summary>
        /// <param name="baseUri">BaseUri cua server back-end</param>
        public ApiConsumer(string baseUri, string tokenCode, string applicationCode)
        {
            this.Init(baseUri, tokenCode, applicationCode);
        }

        public string GetBaseUri()
        {
            return this.baseUri;
        }

        public string GetTokenCode()
        {
            return this.token;
        }

        private void Init(string baseUri, string tokenCode, string applicationCode)
        {
            //Cau hinh JsonConvert
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.None,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            //ServiceStack.Text.JsConfig.IncludeNullValues = false;
            //ServiceStack.Text.JsConfig.ThrowOnDeserializationError = true;
            //ServiceStack.Text.JsConfig.PreferInterfaces = false;
            //ServiceStack.Text.JsConfig.ExcludeTypeInfo = true;
            //ServiceStack.Text.JsConfig.IncludePublicFields = false;

            this.baseUri = baseUri;
            this.token = tokenCode;
            this.applicationCode = applicationCode;
        }

        /// <summary>
        /// Cap nhat tokenCode
        /// </summary>
        /// <param name="tokenCode"></param>
        public void SetTokenCode(string tokenCode)
        {
            this.token = tokenCode;
        }

        /// <summary>
        /// Cap nhat tokenCode
        /// </summary>
        /// <param name="tokenCode"></param>
        public void SetBaseUri(string baseUri)
        {
            this.baseUri = baseUri;
        }

        public void AddDicHeaderRequest(string headerKey, string headerValue)
        {
            if (dicHeaderRequestParam == null)
            {
                dicHeaderRequestParam = new Dictionary<string, string>();
            }
            if (dicHeaderRequestParam.ContainsKey(headerKey))
            {
                dicHeaderRequestParam[headerKey] = headerValue;
            }
            else
            {
                dicHeaderRequestParam.Add(headerKey, headerValue);
            }
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T Get<T>(string uri, object commonParam, object filter, params object[] listParam)
        {
            return Get<T>(uri, commonParam, filter, 0, listParam);
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T Get<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            T result = default(T);
            string requestedUrl = "";
            try
            {
                Inventec.Common.Logging.LogSystem.Info("WebApiClient.Get. api:" + uri + "__Begin");
                using (var client = new HttpClient())
                {
                    this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                    if (filter != null || commonParam != null)
                    {
                        ApiParam apiParam = new ApiParam();
                        apiParam.CommonParam = commonParam;
                        apiParam.ApiData = filter;
                        string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, true)));
                        requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                    }
                    Inventec.Common.Logging.LogSystem.Debug("begin call api: " + requestedUrl);
                    HttpResponseMessage resp = client.GetAsync(requestedUrl).Result;
                    Inventec.Common.Logging.LogSystem.Debug("Received response api: " + uri);
                    if (!resp.IsSuccessStatusCode)
                    {
                        LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}{4}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(filter, false), Utils.SerializeObject(commonParam, false)));
                        throw new ApiException(resp.StatusCode);
                    }

                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));

                    result = Utils.DeserializeObject<T>(responseData);
                    Inventec.Common.Logging.LogSystem.Debug("DeserializeObject response api finish: " + uri);
                }
                Inventec.Common.Logging.LogSystem.Info("WebApiClient.Get. api:" + uri + "__End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => requestedUrl), requestedUrl)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => baseUri), baseUri));
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T GetStrong<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetStrong. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, false)));
                    requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                }
                Inventec.Common.Logging.LogSystem.Debug("begin call api: " + requestedUrl);
                using (Stream s = client.GetStreamAsync(requestedUrl).Result)
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(reader);
                }
                Inventec.Common.Logging.LogSystem.Debug("Received response api: " + uri);

                Inventec.Common.Logging.LogSystem.Debug("DeserializeObject response api finish: " + uri);
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetStrong. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> GetStrongAsync<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetStrongAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, false)));
                    requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                }
                Inventec.Common.Logging.LogSystem.Debug("begin call api: " + requestedUrl);
                using (Stream s = await client.GetStreamAsync(requestedUrl).ConfigureAwait(false))
                using (StreamReader sr = new StreamReader(s))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(reader);
                }

                Inventec.Common.Logging.LogSystem.Debug("Received response api: " + uri);
                Inventec.Common.Logging.LogSystem.Debug("DeserializeObject response api finish: " + uri);
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetStrongAsync. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> GetAsync<T>(string uri, object commonParam, object filter, params object[] listParam)
        {
            return await GetAsync<T>(uri, commonParam, filter, 0);
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> GetAsync<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, true)));
                    requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                }
                Inventec.Common.Logging.LogSystem.Debug("begin call api: " + requestedUrl);
                HttpResponseMessage resp = await client.GetAsync(requestedUrl).ConfigureAwait(false);
                Inventec.Common.Logging.LogSystem.Debug("Received response api: " + uri);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}{4}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(filter, false), Utils.SerializeObject(commonParam, false)));
                    throw new ApiException(resp.StatusCode);
                }

                string responseData = await resp.Content.ReadAsStringAsync();
                result = Utils.DeserializeObject<T>(responseData);
                Inventec.Common.Logging.LogSystem.Debug("DeserializeObject response api finish: " + uri);
                Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetAsync. api:" + uri + "__End");
                return result;
            }
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public FileHolder GetFile(string uri, object commonParam, object filter, params object[] listParam)
        {
            return GetFile(uri, commonParam, filter, 0, listParam);
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public FileHolder GetFile(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetFile. api:" + uri + "__Begin");
            FileHolder result = null;
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    requestedUrl += string.Format("{0}={1}&", API_PARAM, Uri.EscapeDataString(Utils.SerializeObject(apiParam, false)));
                }
                HttpResponseMessage resp = client.GetAsync(requestedUrl).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                if (resp.Content != null && resp.Content.Headers != null && resp.Content.Headers.ContentDisposition != null)
                {
                    result = new FileHolder();
                    result.FileName = resp.Content.Headers.ContentDisposition.FileName;
                    result.Content = new MemoryStream(resp.Content.ReadAsByteArrayAsync().Result);
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetFile. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T Post<T>(string uri, object commonParam, object data, params object[] listParam)
        {
            return Post<T>(uri, commonParam, data, 0, listParam);
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T Post<T>(string uri, object commonParam, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.Post. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }
                HttpResponseMessage resp = client.PostAsJsonAsync(requestedUrl, apiParam).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(data, false)));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                try
                {
                    result = Utils.DeserializeObject<T>(responseData);
                }
                catch (Exception ex)
                {
                    LogSystem.Error("responseData: " + responseData);
                    LogSystem.Error("requestedUrl: " + requestedUrl);
                    LogSystem.Error("apiParam: " + LogUtil.TraceData("apiParam", apiParam));
                    throw ex;
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.Post. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T PostWithouApiParam<T>(string uri, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParam. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                HttpResponseMessage resp = client.PostAsJsonAsync(requestedUrl, data).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                try
                {
                    if (typeof(T).ToString() == "System.String")
                    {
                        Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParam. 1");
                        result = (T)System.Convert.ChangeType(responseData, typeof(T));
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParam.2");
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
                catch (Exception exx)
                {
                    result = (T)System.Convert.ChangeType(responseData, typeof(T));
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParam. api:" + uri + "__End");
            return result;
        }

        public async Task<T> PostWithouApiParamAsync<T>(string uri, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParamAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                HttpResponseMessage resp = await client.PostAsJsonAsync(requestedUrl, data);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = await resp.Content.ReadAsStringAsync();

                try
                {
                    if (typeof(T).ToString() == "System.String")
                    {
                        result = (T)System.Convert.ChangeType(responseData, typeof(T));
                    }
                    else
                    {
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
                catch (Exception exx)
                {
                    result = (T)System.Convert.ChangeType(responseData, typeof(T));
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithouApiParamAsync. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> PostAsync<T>(string uri, object commonParam, object data, params object[] listParam)
        {
            return await PostAsync<T>(uri, commonParam, data, 0);
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> PostAsync<T>(string uri, object commonParam, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }

                HttpResponseMessage resp = await client.PostAsJsonAsync(requestedUrl, apiParam).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(data, false)));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = await resp.Content.ReadAsStringAsync();
                result = Utils.DeserializeObject<T>(responseData);

            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostAsync. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Post du lieu co kem file
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="files">Danh sach file can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T PostWithFile<T>(string uri, object commonParam, object data, List<FileHolder> files, params object[] listParam)
        {
            return PostWithFile<T>(uri, commonParam, data, files, 0, listParam);
        }

        /// <summary>
        /// Post du lieu co kem file
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="files">Danh sach file can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T PostWithFile<T>(string uri, object commonParam, object data, List<FileHolder> files, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFile. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }
                using (var content = new MultipartFormDataContent())
                {
                    foreach (FileHolder file in files)
                    {
                        content.Add(new StreamContent(file.Content), "File", file.FileName);
                    }
                    HttpContent stringContent = new StringContent(Utils.SerializeObject(apiParam, false));
                    content.Add(stringContent, "Data", "Data");

                    using (HttpResponseMessage resp = client.PostAsync(requestedUrl, content).Result)
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                            throw new ApiException(resp.StatusCode);
                        }
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFile. api:" + uri + "__End");
            return result;
        }

        public async Task<T> PostWithFileAsync<T>(string uri, object commonParam, object data, List<FileHolder> files, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }
                using (var content = new MultipartFormDataContent())
                {
                    foreach (FileHolder file in files)
                    {
                        content.Add(new StreamContent(file.Content), "File", file.FileName);
                    }
                    HttpContent stringContent = new StringContent(Utils.SerializeObject(apiParam, false));
                    content.Add(stringContent, "Data", "Data");

                    using (HttpResponseMessage resp = await client.PostAsync(requestedUrl, content))
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                            throw new ApiException(resp.StatusCode);
                        }
                        string responseData = await resp.Content.ReadAsStringAsync();
                        result = Utils.DeserializeObject<T>(responseData);

                    }
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileAsync. api:" + uri + "__End");
            return result;
        }

        public T PostWithFileWithouApiParam<T>(string uri, Dictionary<string, object> dicContent, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileWithouApiParam. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                using (var content = new MultipartFormDataContent())
                {
                    if (dicContent != null && dicContent.Count > 0)
                    {
                        foreach (var item in dicContent)
                        {
                            if (item.Value is byte[])
                            {
                                var imageContent = new ByteArrayContent((byte[])item.Value);
                                imageContent.Headers.ContentType =
                                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");
                                content.Add(imageContent, item.Key, "audio.wav");
                            }
                            else if (item.Value is MemoryStream)
                            {
                                var imageContent = new ByteArrayContent(Utils.StreamToByte((MemoryStream)item.Value));
                                imageContent.Headers.ContentType =
                                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");
                                content.Add(imageContent, item.Key, "audio.wav");
                            }
                            else if (item.Value is string)
                            {
                                HttpContent stringContent = new StringContent((string)item.Value);
                                content.Add(stringContent, item.Key, item.Key);
                            }
                            else if (item.Value is long || item.Value is int || item.Value is short || item.Value is decimal || item.Value is double)
                            {
                                HttpContent stringContent = new StringContent(item.Value != null ? item.Value + "" : "");
                                content.Add(stringContent, item.Key, item.Key);
                            }
                            else
                            {
                                HttpContent stringContent = new StringContent(item.Value != null ? Utils.SerializeObject(item.Value, false) : "");
                                content.Add(stringContent, item.Key, item.Key);
                            }
                        }
                    }

                    using (HttpResponseMessage resp = client.PostAsync(requestedUrl, content).Result)
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                            throw new ApiException(resp.StatusCode);
                        }
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileWithouApiParam. api:" + uri + "__End");
            return result;
        }

        public string PostFormUrlEncoded(string uri, Dictionary<string, object> dicContent, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostFormUrlEncoded. api:" + uri + "__Begin");
            string result = String.Empty;
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                var dict = new Dictionary<string, string>();
                foreach (var item in dicContent)
                {
                    dict.Add(item.Key, (string)item.Value);
                }

                var req = new HttpRequestMessage(HttpMethod.Post, requestedUrl) { Content = new FormUrlEncodedContent(dict) };
                using (HttpResponseMessage resp = client.SendAsync(req).Result)
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                        throw new ApiException(resp.StatusCode);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostFormUrlEncoded. api:" + uri + "__End");
            return result;
        }

        public async Task<string> PostFormUrlEncodedAsync<T>(string uri, Dictionary<string, object> dicContent, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostFormUrlEncodedAsync. api:" + uri + "__Begin");
            string result = String.Empty;
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                var dict = new Dictionary<string, string>();
                foreach (var item in dicContent)
                {
                    dict.Add(item.Key, (string)item.Value);
                }

                var req = new HttpRequestMessage(HttpMethod.Post, requestedUrl) { Content = new FormUrlEncodedContent(dict) };
                using (HttpResponseMessage resp = await client.SendAsync(req))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                        throw new ApiException(resp.StatusCode);
                    }
                    result = resp.Content.ReadAsStringAsync().Result;
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostFormUrlEncodedAsync. api:" + uri + "__End");
            return result;
        }

        public async Task<T> PostWithFileWithouApiParamAsync<T>(string uri, Dictionary<string, object> dicContent, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileWithouApiParamAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                using (var content = new MultipartFormDataContent())
                {
                    if (dicContent != null && dicContent.Count > 0)
                    {
                        foreach (var item in dicContent)
                        {
                            if (item.Value is byte[])
                            {
                                var imageContent = new ByteArrayContent((byte[])item.Value);
                                imageContent.Headers.ContentType =
                                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");
                                content.Add(imageContent, item.Key, "audio.wav");
                            }
                            else if (item.Value is MemoryStream)
                            {
                                var imageContent = new ByteArrayContent(Utils.StreamToByte((MemoryStream)item.Value));
                                imageContent.Headers.ContentType =
                                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");
                                content.Add(imageContent, item.Key, "audio.wav");
                            }
                            else
                            {
                                HttpContent stringContent = new StringContent(item.Value != null ? Utils.SerializeObject(item.Value, false) : "");
                                content.Add(stringContent, item.Key, item.Key);
                            }
                        }
                    }

                    using (HttpResponseMessage resp = await client.PostAsync(requestedUrl, content))
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                            throw new ApiException(resp.StatusCode);
                        }
                        string responseData = await resp.Content.ReadAsStringAsync();
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithFileWithouApiParamAsync. api:" + uri + "__End");
            return result;
        }

        public T PostWithBinaryWithouApiParam<T>(string uri, byte[] bData, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithBinaryWithouApiParam. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("audio/wav"));

                var byteArrayContent = new ByteArrayContent(bData);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
                using (HttpResponseMessage resp = client.PostAsync(requestedUrl, byteArrayContent).Result)
                {
                    //resp.EnsureSuccessStatusCode();
                    if (!resp.IsSuccessStatusCode)
                    {
                        LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                        throw new ApiException(resp.StatusCode);
                    }
                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    result = Utils.DeserializeObject<T>(responseData);
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithBinaryWithouApiParam. api:" + uri + "__End");
            return result;
        }

        public async Task<T> PostWithBinaryWithouApiParamAsync<T>(string uri, byte[] bData, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithBinaryWithouApiParamAsync. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("audio/wav"));

                var byteArrayContent = new ByteArrayContent(bData);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");

                using (HttpResponseMessage resp = await client.PostAsync(requestedUrl, byteArrayContent))
                {
                    //resp.EnsureSuccessStatusCode();
                    if (!resp.IsSuccessStatusCode)
                    {
                        LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                        throw new ApiException(resp.StatusCode);
                    }
                    string responseData = await resp.Content.ReadAsStringAsync();
                    result = Utils.DeserializeObject<T>(responseData);
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostWithBinaryWithouApiParamAsync. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Put du lieu
        /// Exception: Exception, ArgumentException, ApiException
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can put</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        public T Put<T>(string uri, object commonParam, object data, params object[] listParam)
        {
            return Put<T>(uri, commonParam, data, 0, listParam);
        }

        public T Put<T>(string uri, object commonParam, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.Put. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }
                HttpResponseMessage resp = client.PutAsJsonAsync(requestedUrl, apiParam).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                result = Utils.DeserializeObject<T>(responseData);
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.Put. api:" + uri + "__End");
            return result;
        }

        public T PutWithouApiParam<T>(string uri, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PutWithouApiParam. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                HttpResponseMessage resp = client.PutAsJsonAsync(requestedUrl, data).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestedUrl, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;

                try
                {
                    if (typeof(T).ToString() == "System.String")
                    {
                        result = (T)System.Convert.ChangeType(responseData, typeof(T));
                    }
                    else
                    {
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
                catch (Exception exx)
                {
                    result = (T)System.Convert.ChangeType(responseData, typeof(T));
                }
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PutWithouApiParam. api:" + uri + "__End");
            return result;
        }

        public T DeleteWithouApiParam<T>(string uri, int userTimeout)
        {
            Inventec.Common.Logging.LogSystem.Debug("WebApiClient.DeleteWithouApiParam. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, (userTimeout > 0 ? userTimeout : TIME_OUT));

                HttpResponseMessage resp = client.DeleteAsync(uri).Result;
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, uri, resp.StatusCode.GetHashCode()));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                try
                {
                    if (typeof(T).ToString() == "System.String")
                    {
                        result = (T)System.Convert.ChangeType(responseData, typeof(T));
                    }
                    else
                    {
                        result = Utils.DeserializeObject<T>(responseData);
                    }
                }
                catch (Exception exx)
                {
                    result = (T)System.Convert.ChangeType(responseData, typeof(T));
                }
            }
            Inventec.Common.Logging.LogSystem.Debug("WebApiClient.DeleteWithouApiParam. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Tao HttpRequest voi cac tham so cho truoc
        /// </summary>
        /// <param name="client"></param>
        /// <param name="uri"></param>
        /// <param name="requestedUrl"></param>
        /// <param name="listParam"></param>
        private void HttpRequestBuilder(HttpClient client, string uri, ref string requestedUrl, int userTimeout, params object[] listParam)
        {
            if (!string.IsNullOrWhiteSpace(this.token))
            {
                client.DefaultRequestHeaders.Add(HeaderConstants.TOKEN_PARAM, this.token);
            }
            if (!string.IsNullOrWhiteSpace(this.applicationCode))
            {
                client.DefaultRequestHeaders.Add(HeaderConstants.APPLICATION_CODE_PARAM, this.applicationCode);
            }

            if (dicHeaderRequestParam != null && dicHeaderRequestParam.Count > 0)
            {
                foreach (var hder in dicHeaderRequestParam)
                {
                    //client.DefaultRequestHeaders.Add(hder.Key, hder.Value);

                    client.DefaultRequestHeaders.TryAddWithoutValidation(hder.Key, hder.Value);
                }
            }

            client.BaseAddress = new Uri(this.baseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.Timeout = new TimeSpan(0, 0, (userTimeout > 0 ? userTimeout : TIME_OUT));

            requestedUrl = string.Format("{0}?", uri);
            if (listParam != null && listParam.Length > 0)
            {
                if (listParam.Length % 2 != 0)
                {
                    throw new ArgumentException("Danh sach param khong hop le. So luong param phai la so chan");
                }
                for (int i = 0; i < listParam.Length; )
                {
                    requestedUrl += string.Format("{0}={1}&", HttpUtility.UrlEncode(listParam[i] + ""), HttpUtility.UrlEncode(listParam[i + 1] + ""));
                    i = i + 2;
                }
            }

            this.SetIpAddressToHeader(client);
            this.ProcessHttpsCertificate();
        }

        private void ProcessHttpsCertificate()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.baseUri) && this.baseUri.StartsWith("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                     (sender, cert, chain, sslPolicyErrors) => true;
                }

                string isFixtWebException = (ConfigurationManager.AppSettings["Inventec.Common.WebApiClient.IsFixtWebException"] ?? "0");
                if (isFixtWebException == "1")
                {
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetIpAddressToHeader(HttpClient client)
        {
            try
            {
                string ipAddress = GetIpLocal();
                if (!String.IsNullOrEmpty(ipAddress))
                {
                    client.DefaultRequestHeaders.Add(CLIENT_IP_ADDRESS_PARAM, ipAddress);
                    Inventec.Common.Logging.LogSystem.Info("SetIpAddressToHeader:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ipAddress), ipAddress));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string GetIpLocal()
        {
            string ipAddress = "";
            try
            {
                var headers = (HttpContext.Current != null && HttpContext.Current.Request != null) ? HttpContext.Current.Request.Headers : null;
                if (headers != null && headers.AllKeys.Contains(CLIENT_IP_ADDRESS_PARAM))
                {
                    ipAddress = headers[CLIENT_IP_ADDRESS_PARAM];
                }
                else if (headers != null && headers.AllKeys.Contains("HTTP_X_FORWARDED_FOR"))
                {
                    ipAddress = headers["HTTP_X_FORWARDED_FOR"];
                    string[] ipRange = ipAddress.Split(',');
                    int le = ipRange.Length - 1;
                    ipAddress = ipRange[le];
                }
                else if (headers != null && headers.AllKeys.Contains("REMOTE_ADDR"))
                {
                    ipAddress = headers["REMOTE_ADDR"];
                }
                else
                {
                    ipAddress = IpAddressUtils.GetIpAddressLocal();
                }
                if (!String.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = ipAddress.Replace("(Preferred)", "").Replace("(Duplicate)", "");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            return ipAddress;
        }

        /// <summary>
        /// Get du lieu nen
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T GetZip<T>(string uri, object commonParam, object filter, params object[] listParam)
        {
            return GetZip<T>(uri, commonParam, filter, 0, listParam);
        }

        /// <summary>
        /// Get du lieu nen
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public T GetZip<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetZip. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, true)));
                    requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                }
                //Inventec.Common.Logging.LogSystem.Debug("GetZip begin send: " + uri);
                HttpResponseMessage resp = client.GetAsync(requestedUrl).Result;
                //Inventec.Common.Logging.LogSystem.Debug("GetZip end send: " + uri);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}{4}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(filter, false), Utils.SerializeObject(commonParam, false)));
                    throw new ApiException(resp.StatusCode);
                }

                byte[] responseReult = resp.Content.ReadAsByteArrayAsync().Result;
                result = MessagePack.MessagePackSerializer.Deserialize<T>(responseReult, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
                //Inventec.Common.Logging.LogSystem.Debug("GetZip end result: " + uri);
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetZip. api:" + uri + "__End");
            return result;
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> GetAsyncAwait<T>(string uri, object commonParam, object filter, params object[] listParam)
        {
            return await GetAsyncAwait<T>(uri, commonParam, filter, 0);
        }

        /// <summary>
        /// Get du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="filter">Du lieu Filter</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> GetAsyncAwait<T>(string uri, object commonParam, object filter, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetAsyncAwait. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);
                if (filter != null || commonParam != null)
                {
                    ApiParam apiParam = new ApiParam();
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = filter;
                    string param = Convert.ToBase64String(Encoding.UTF8.GetBytes(Utils.SerializeObject(apiParam, true)));
                    requestedUrl += string.Format("{0}={1}", API_PARAM, param);
                }
                Inventec.Common.Logging.LogSystem.Debug("begin call api: " + requestedUrl);
                HttpResponseMessage resp = await client.GetAsync(requestedUrl);
                Inventec.Common.Logging.LogSystem.Debug("Received response api: " + uri);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}{4}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(filter, false), Utils.SerializeObject(commonParam, false)));
                    throw new ApiException(resp.StatusCode);
                }

                string responseData = await resp.Content.ReadAsStringAsync();
                result = Utils.DeserializeObject<T>(responseData);
                Inventec.Common.Logging.LogSystem.Debug("DeserializeObject response api finish: " + uri);
                Inventec.Common.Logging.LogSystem.Info("WebApiClient.GetAsyncAwait. api:" + uri + "__End");
                return result;
            }
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> PostAsyncAwait<T>(string uri, object commonParam, object data, params object[] listParam)
        {
            return await PostAsyncAwait<T>(uri, commonParam, data, 0);
        }

        /// <summary>
        /// Post du lieu
        /// </summary>
        /// <typeparam name="T">Kieu du lieu mong muon tra ve</typeparam>
        /// <param name="uri">Uri cua API can goi</param>
        /// <param name="commonParam">Doi tuong commonParam</param>
        /// <param name="data">Du lieu can post</param>
        /// <param name="listParam">Danh sach cac tham so bo sung. Kieu du lieu phai la primitive</param>
        /// <returns></returns>
        /// <exceptions>Exception, ArgumentException, ApiException</exceptions>
        public async Task<T> PostAsyncAwait<T>(string uri, object commonParam, object data, int userTimeout, params object[] listParam)
        {
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostAsyncAwait. api:" + uri + "__Begin");
            T result = default(T);
            using (var client = new HttpClient())
            {
                string requestedUrl = "";
                this.HttpRequestBuilder(client, uri, ref requestedUrl, userTimeout, listParam);

                ApiParam apiParam = new ApiParam();
                if (data != null || commonParam != null)
                {
                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = data;
                }

                HttpResponseMessage resp = await client.PostAsJsonAsync(requestedUrl, apiParam);
                if (!resp.IsSuccessStatusCode)
                {
                    LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(data, false)));
                    throw new ApiException(resp.StatusCode);
                }
                string responseData = await resp.Content.ReadAsStringAsync();
                result = Utils.DeserializeObject<T>(responseData);
            }
            Inventec.Common.Logging.LogSystem.Info("WebApiClient.PostAsyncAwait. api:" + uri + "__End");
            return result;
        }
    }
}
