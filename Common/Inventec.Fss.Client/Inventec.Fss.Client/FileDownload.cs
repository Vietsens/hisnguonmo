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
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage resp = client.GetAsync((!String.IsNullOrEmpty(baseUri) ? baseUri : FssConstant.BASE_URI) + fileUrl).Result;
                    if (!resp.IsSuccessStatusCode)
                    {
                        throw new FileDownloadException(resp.StatusCode, resp.ReasonPhrase);
                    }
                    if (resp.Content != null)
                    {
                        return new MemoryStream(resp.Content.ReadAsByteArrayAsync().Result);
                    }
                    return null;
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
        }
    }
}
