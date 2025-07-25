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
            try
            {
                using (var client = new HttpClient())
                {
                    if (!String.IsNullOrEmpty(fileUrl) && fileUrl.Replace("/", "\\").StartsWith("\\"))
                    {
                        HttpResponseMessage result = client.GetAsync((!string.IsNullOrEmpty(baseUri) ? baseUri : FssConstant.BASE_URI) + fileUrl).Result;
                        if (!result.IsSuccessStatusCode)
                        {
                            throw new FileDownloadException(result.StatusCode, result.ReasonPhrase);
                        }
                        stream = ReferenceEquals(result.Content, null) ? null : new MemoryStream(result.Content.ReadAsByteArrayAsync().Result);
                    }
                    else if (!String.IsNullOrEmpty(fileUrl))
                    {
                        client.BaseAddress = new Uri(!String.IsNullOrEmpty(baseUri) ? baseUri : FssConstant.BASE_URI);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);

                        string requestedUrl = FssConstant.DOWNLOAD_URI;

                        ApiParam apiParam = new ApiParam();
                        CommonParam commonParam = new CommonParam();

                        apiParam.CommonParam = commonParam;
                        apiParam.ApiData = fileUrl;

                        HttpResponseMessage resp = client.PostAsJsonAsync(requestedUrl, apiParam).Result;
                        if (!resp.IsSuccessStatusCode)
                        {
                            //LogSystem.Error(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}. Input: {3}.", client.BaseAddress.AbsoluteUri, requestedUrl, resp.StatusCode.GetHashCode(), Utils.SerializeObject(data, false)));
                            throw new Exception("StatusCode:" + resp.StatusCode);
                        }
                        string responseData = resp.Content.ReadAsStringAsync().Result;                        
                        byte[] content = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(responseData);
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
            catch (Exception ex)
            {
                throw new FileUploadException("Exception when uploading file", ex);
            }
            return stream;
        }
    }
}
