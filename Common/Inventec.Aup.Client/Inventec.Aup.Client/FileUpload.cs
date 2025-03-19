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
using Inventec.Aup.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Inventec.Aup.Client
{
    public class FileUpload
    {
        /// <summary>
        /// Tai nhieu file len he thong FSS
        /// </summary>
        /// <param name="clientCode">Ma client (chi chua ky tu alpabet va ky tu gach chan "_")</param>
        /// <param name="fileStoreLocation">Thu muc ma client muon luu tru</param>
        /// <param name="files">Du lieu cac file can upload</param>
        /// <param name="keepOriginalFile">Co giu ten ban dau cua file hay khong</param>
        /// <returns>
        /// - Neu thanh cong, tra ve duong dan luu tru cua file tren he thong FSS (khong bao gom base-address)
        /// - Neu that bai, tra ve FileUploadException
        /// </returns>
        public static List<FileUploadInfo> UploadFile(string clientCode, List<FileUploadInfo> files)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(AupConstant.BASE_URI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, AupConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(AupConstant.HEADER_CLIENT_CODE, clientCode);

                    Inventec.Common.WebApiClient.ApiParam apiParam = new Inventec.Common.WebApiClient.ApiParam();
                    if (files != null)
                    {
                        apiParam.CommonParam = new CommonParam();
                        apiParam.ApiData = files;

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => files), files.Select(o => o.Url)));

                    }

                    Inventec.Common.Logging.LogSystem.Debug("clientCode=" + clientCode + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AupConstant.BASE_URI), AupConstant.BASE_URI));

                    using (HttpResponseMessage message = client.PostAsJsonAsync(string.Format("{0}", AupConstant.UPLOAD_URI), apiParam).Result)
                    {
                        if (message.IsSuccessStatusCode)
                        {
                            string jsonString = message.Content.ReadAsStringAsync().Result;                            
                            if (!String.IsNullOrWhiteSpace(jsonString))
                            {
                                var rsData = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResultObject<List<FileUploadInfo>>>(jsonString);
                                return rsData != null ? rsData.Data : null;
                            }
                        }
                        else
                        {
                            throw new FileUploadException(message.StatusCode, message.ReasonPhrase);
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
            return null;
        }

        /// <summary>
        /// Tai nhieu file len he thong FSS
        /// </summary>
        /// <param name="clientCode">Ma client (chi chua ky tu alpabet va ky tu gach chan "_")</param>
        /// <param name="fileStoreLocation">Thu muc ma client muon luu tru</param>
        /// <param name="files">Du lieu cac file can upload</param>
        /// <param name="keepOriginalFile">Co giu ten ban dau cua file hay khong</param>
        /// <returns>
        /// - Neu thanh cong, tra ve duong dan luu tru cua file tren he thong FSS (khong bao gom base-address)
        /// - Neu that bai, tra ve FileUploadException
        /// </returns>
        public static List<FileUploadInfo> UploadFile(string clientCode, List<FileUploadInfo> files, string baseUri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, AupConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(AupConstant.HEADER_CLIENT_CODE, clientCode);

                    Inventec.Common.WebApiClient.ApiParam apiParam = new Inventec.Common.WebApiClient.ApiParam();
                    if (files != null)
                    {
                        apiParam.CommonParam = new CommonParam();
                        apiParam.ApiData = files;
                    }

                    using (HttpResponseMessage message = client.PostAsJsonAsync(string.Format("{0}", AupConstant.UPLOAD_URI), apiParam).Result)
                    {
                        if (message.IsSuccessStatusCode)
                        {
                            string jsonString = message.Content.ReadAsStringAsync().Result;
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileUploadInfo>>(jsonString);
                        }
                        else
                        {
                            throw new FileUploadException(message.StatusCode, message.ReasonPhrase);
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
        }

    }
}
