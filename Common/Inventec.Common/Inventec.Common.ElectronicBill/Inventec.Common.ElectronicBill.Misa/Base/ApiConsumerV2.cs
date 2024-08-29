using Inventec.Common.ElectronicBill.Misa.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Inventec.Common.ElectronicBill.Misa.Base
{
    class ApiConsumerV2
    {
        /// <summary>
        /// Key header chứa chuỗi Token bảo mật
        /// </summary>
        public const string Authorization = "Authorization";

        /// <summary>
        /// Key header chứa taxcode
        /// </summary>
        public const string CompanyTaxCode = "CompanyTaxCode";

        private string baseUri;
        private string UserName;
        private string Password;
        private string AppID;
        private string TaxCode;
        private static Dictionary<string, string> DicAuthorization = new Dictionary<string, string>();

        public ApiConsumerV2(string uri, string appID, string taxCode, string userName, string password)
        {
            this.baseUri = uri;
            this.UserName = userName;
            this.Password = password;
            this.AppID = appID;
            this.TaxCode = taxCode;
        }

        public T CreateRequest<T>(string requestUri, object sendData)
        {
            string authInfo = GetAuthInfo();
            using (var client = new HttpClient())
            {
                string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                string uri = this.baseUri.Replace(extension, "");
                string url = extension + requestUri;

                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 90);
                if (!string.IsNullOrWhiteSpace(authInfo))
                {
                    client.DefaultRequestHeaders.Add(Authorization, string.Format("{0} {1}", "Bearer", authInfo));
                }

                if (!string.IsNullOrWhiteSpace(this.TaxCode))
                {
                    client.DefaultRequestHeaders.Add(CompanyTaxCode, this.TaxCode);
                }

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(url, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (HttpException ex)
                {
                    if (ex.ErrorCode == 401)
                    {
                        throw new Exception("Thông tin đăng nhập không đúng");
                    }
                    else if (ex.ErrorCode == 404)
                    {
                        throw new Exception("Không tìm thấy tài nguyên");
                    }
                    else if (ex.ErrorCode == 400)
                    {
                        throw new Exception("Thông tin tham số không đúng");
                    }
                    else if (ex.ErrorCode == 500)
                    {
                        throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    Inventec.Common.Logging.LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", uri, url, statusCode));
                }

                string responseData = resp.Content.ReadAsStringAsync().Result;
                //Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        public T Sign<T>(string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.Timeout = new TimeSpan(0, 0, 90);
                client.DefaultRequestHeaders.Add("MisaTokenKey", "491CB943-E466-4D25-B0A9-7042594F59F2");

                HttpResponseMessage resp = null;
                try
                {
                    string sendJsonData = JsonConvert.SerializeObject(sendData);
                    Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);
                    resp = client.PostAsync(requestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                }
                catch (HttpException ex)
                {
                    if (ex.ErrorCode == 401)
                    {
                        throw new Exception("Thông tin đăng nhập không đúng");
                    }
                    else if (ex.ErrorCode == 404)
                    {
                        throw new Exception("Không tìm thấy tài nguyên");
                    }
                    else if (ex.ErrorCode == 400)
                    {
                        throw new Exception("Thông tin tham số không đúng");
                    }
                    else if (ex.ErrorCode == 500)
                    {
                        throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", this.baseUri, requestUri, statusCode));
                }
                string responseData = resp.Content.ReadAsStringAsync().Result;
                Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        private string GetAuthInfo()
        {
            string result = "";
            if (DicAuthorization.ContainsKey(UserName) && !String.IsNullOrWhiteSpace(DicAuthorization[UserName]))
            {
                result = DicAuthorization[UserName];
            }
            else
            {
                using (var client = new HttpClient())
                {
                    string extension = this.baseUri.Substring(this.baseUri.IndexOf('/', this.baseUri.IndexOf("//") + 2));
                    string uri = this.baseUri.Replace(extension, "");

                    if (extension.ToLower().Contains("/code"))
                    {
                        extension = extension.ToLower().Replace("/code", "");
                    }

                    string url = extension + RequestUriStore.ApiLoginV2;

                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, 90);
                    if (!string.IsNullOrWhiteSpace(this.TaxCode))
                    {
                        client.DefaultRequestHeaders.Add(CompanyTaxCode, this.TaxCode);
                    }

                    HttpResponseMessage resp = null;
                    //string uri = extension + string.Format(RequestUriStore.ApiLogin, HttpUtility.UrlEncode(AppID), HttpUtility.UrlEncode(TaxCode), HttpUtility.UrlEncode(UserName), HttpUtility.UrlEncode(Password));
                    try
                    {
                        GetTokenParameter data = new GetTokenParameter();
                        data.AppID = this.AppID;
                        data.TaxCode = this.TaxCode;
                        data.Password = this.Password;
                        data.UserName = this.UserName;

                        string sendJsonData = JsonConvert.SerializeObject(data);
                        resp = client.PostAsync(url, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;
                    }
                    catch (HttpException ex)
                    {
                        if (ex.ErrorCode == 401)
                        {
                            throw new Exception("Thông tin đăng nhập không đúng");
                        }
                        else if (ex.ErrorCode == 404)
                        {
                            throw new Exception("Không tìm thấy tài nguyên");
                        }
                        else if (ex.ErrorCode == 400)
                        {
                            throw new Exception("Thông tin tham số không đúng");
                        }
                        else if (ex.ErrorCode == 500)
                        {
                            throw new Exception("Lỗi trên server. Gặp lỗi này vui lòng liên hệ với MISA để được hỗ trợ");
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (resp == null)
                    {
                        throw new Exception(string.Format("Loi khi goi API: {0}{1}", uri, url));
                    }

                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        int statusCode = resp.StatusCode.GetHashCode();
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseData), responseData));
                        throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", uri, url, statusCode));
                    }

                    if (!String.IsNullOrWhiteSpace(responseData))
                    {
                        ApiResult data = JsonConvert.DeserializeObject<ApiResult>(responseData);
                        if (data.Success && !string.IsNullOrWhiteSpace(data.Data))
                        {
                            result = data.Data;
                            DicAuthorization[UserName] = data.Data;
                        }
                    }
                }
            }
            return result;
        }
    }
}
