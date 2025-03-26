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
