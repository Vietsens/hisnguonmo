using Inventec.Core;
using Inventec.Fss.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Fss.Client
{
    public class FileDelete
    {
        public static bool DeleteFile(string clientCode, string fileUrl)
        {
            try
            {
                return DeleteFile(clientCode, fileUrl, FssConstant.BASE_URI);
            }
            catch (FileDeleteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FileDeleteException("Exception when uploading file", ex);
            }
        }

        public static bool DeleteFile(string clientCode, string fileUrl, string baseUri)
        {
            bool result = false;
            FssErrorHelper.Reset();
            string deleteUri = FssErrorHelper.Combine(baseUri, FssConstant.DELETE_URI);
            const string ACTION = "xóa file trên máy chủ lưu trữ";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = FssErrorHelper.ParseBaseUri(baseUri, ACTION);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_CLIENT_CODE, clientCode);

                    string requestedUrl = FssConstant.DELETE_URI;

                    ApiParam apiParam = new ApiParam();
                    CommonParam commonParam = new CommonParam();

                    apiParam.CommonParam = commonParam;
                    apiParam.ApiData = fileUrl;

                    Inventec.Core.ApiResultObject<bool> rs = null;

                    HttpResponseMessage resp = null;
                    try
                    {
                        resp = client.PostAsJsonAsync(requestedUrl, apiParam).Result;
                    }
                    catch (Exception exCall)
                    {
                        // vCong 62522: timeout/khong ket noi toi FSS
                        string msg;
                        Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.Wrap(exCall, deleteUri, ACTION, out msg);
                        throw new FileDeleteException(msg, ie);
                    }
                    string responseData = resp.Content.ReadAsStringAsync().Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        // giu hanh vi cu (tra false), chi ghi nhan ly do de noi goi hien thi qua FssErrorHelper.LastErrorMessage
                        string msg;
                        FssErrorHelper.FromStatus(resp, deleteUri, ACTION, out msg);
                    }
                    try
                    {
                        rs = Newtonsoft.Json.JsonConvert.DeserializeObject<Inventec.Core.ApiResultObject<bool>>(responseData);
                        if (rs != null && rs.Success)
                        {
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (FileDeleteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                string msg;
                Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.Wrap(ex, deleteUri, ACTION, out msg);
                throw new FileDeleteException(msg, ie);
            }
            return result;
        }
    }
}
