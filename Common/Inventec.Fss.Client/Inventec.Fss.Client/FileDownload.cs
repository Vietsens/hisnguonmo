using Inventec.Core;
using Inventec.Fss.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Inventec.Fss.Client
{
    public class FileDownload
    {
        public static MemoryStream GetFile(string fileUrl)
        {
            try
            {
                return GetFile(fileUrl, null);
            }
            catch (FileUploadException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FileUploadException("Exception when uploading file", ex);
            }
        }

        public static MemoryStream GetFile(string fileUrl, string baseUri)
        {
            MemoryStream stream = new MemoryStream();
            FssErrorHelper.Reset();
            string effectiveBase = !string.IsNullOrEmpty(baseUri) ? baseUri : FssConstant.BASE_URI;
            string downloadUri = FssErrorHelper.Combine(effectiveBase, FssConstant.DOWNLOAD_URI);
            const string ACTION = "tải file từ máy chủ lưu trữ";
            try
            {
                using (var client = new HttpClient())
                {
                    if (!String.IsNullOrEmpty(fileUrl) && fileUrl.Replace("/", "\\").StartsWith("\\"))
                    {
                        string directUri = effectiveBase + fileUrl;
                        HttpResponseMessage result = null;
                        try
                        {
                            result = client.GetAsync(directUri).Result;
                        }
                        catch (Exception exCall)
                        {
                            string msg;
                            Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.Wrap(exCall, directUri, ACTION, out msg);
                            throw new FileDownloadException(msg, ie);
                        }
                        if (!result.IsSuccessStatusCode)
                        {
                            string msg;
                            Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.FromStatus(result, directUri, ACTION, out msg);
                            throw new FileDownloadException(result.StatusCode, msg, ie);
                        }
                        stream = ReferenceEquals(result.Content, null) ? null : new MemoryStream(result.Content.ReadAsByteArrayAsync().Result);
                    }
                    else if (!String.IsNullOrEmpty(fileUrl))
                    {
                        client.BaseAddress = FssErrorHelper.ParseBaseUri(effectiveBase, ACTION);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);

                        string requestedUrl = FssConstant.DOWNLOAD_URI;

                        ApiParam apiParam = new ApiParam();
                        CommonParam commonParam = new CommonParam();

                        apiParam.CommonParam = commonParam;
                        apiParam.ApiData = fileUrl;

                        HttpResponseMessage resp = null;
                        try
                        {
                            resp = client.PostAsJsonAsync(requestedUrl, apiParam).Result;
                        }
                        catch (Exception exCall)
                        {
                            // vCong 62522: timeout/khong ket noi toi FSS
                            string msg;
                            Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.Wrap(exCall, downloadUri, ACTION, out msg);
                            throw new FileUploadException(msg, ie);
                        }
                        if (!resp.IsSuccessStatusCode)
                        {
                            //LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(data, false)));
                            string msg;
                            Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.FromStatus(resp, downloadUri, ACTION, out msg);
                            throw new FileUploadException(resp.StatusCode, msg, ie);
                        }
                        string responseData = resp.Content.ReadAsStringAsync().Result;
                        byte[] content = null;
                        try
                        {
                            content = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(responseData);
                        }
                        catch (Exception exParse)
                        {
                            // FSS tra 200 nhung noi dung khong phai file (vd HTML loi) -> phia FSS
                            Inventec.Common.IntegrationError.IntegrationException ie = Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(FssErrorHelper.Partner, downloadUri, responseData, exParse);
                            throw new FileUploadException(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ie, FssErrorHelper.Partner, ACTION), ie);
                        }
                        try
                        {
                            stream = new MemoryStream(content);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
            catch (FileUploadException ex)
            {
                throw ex;
            }
            catch (FileDownloadException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                string msg;
                Inventec.Common.IntegrationError.IntegrationException ie = FssErrorHelper.Wrap(ex, downloadUri, ACTION, out msg);
                throw new FileUploadException(msg, ie);
            }
            return stream;
        }
    }
}
