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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Fss.Client
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
        public static List<FileUploadInfo> UploadFile(string clientCode, string fileStoreLocation, List<FileHolder> files, bool keepOriginalFile)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(FssConstant.BASE_URI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_CLIENT_CODE, clientCode);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_FILE_STORE_LOCATION, fileStoreLocation);

                    ProcessHttpsCertificate("");

                    using (var content = new MultipartFormDataContent())
                    {
                        foreach (FileHolder file in files)
                        {
                            content.Add(new StreamContent(file.Content), "file", file.FileName);
                        }

                        using (HttpResponseMessage message = client.PostAsync(string.Format("{0}?keepOriginalFileName={1}", FssConstant.UPLOAD_URI, keepOriginalFile), content).Result)
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

        /// <summary>
        /// Hàm fix lỗi khi backend có ssl "System.Net.WebException: The underlying connection was closed: An unexpected error occurred on a receive. ---> System.ComponentModel.Win32Exception: The client and server cannot communicate, because they do not possess a common algorithm"
        /// </summary>
        static void ProcessHttpsCertificate(string baseUri)
        {
            try
            {
                if (!String.IsNullOrEmpty(baseUri) && baseUri.StartsWith("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                     (sender, cert, chain, sslPolicyErrors) => true;

                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }
                else if (!String.IsNullOrEmpty(FssConstant.BASE_URI) && FssConstant.BASE_URI.StartsWith("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                     (sender, cert, chain, sslPolicyErrors) => true;

                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Tai 1 file len he thong FSS
        /// </summary>
        /// <param name="clientCode">Ma client (chi chua ky tu alpabet va ky tu gach chan "_")</param>
        /// <param name="fileStoreLocation">Thu muc ma client muon luu tru</param>
        /// <param name="stream">Du lieu file can upload</param>
        /// <param name="fileName">Ten file</param>
        /// <returns>
        /// - Neu thanh cong, tra ve duong dan luu tru cua file tren he thong FSS (khong bao gom base-address)
        /// - Neu that bai, tra ve FileUploadException
        /// </returns>
        public static FileUploadInfo UploadFile(string clientCode, string fileStoreLocation, MemoryStream stream, string fileName, bool keepOriginalFile)
        {
            try
            {
                stream.Position = 0;
                List<FileHolder> files = new List<FileHolder>() { new FileHolder(stream, fileName) };
                List<FileUploadInfo> result = UploadFile(clientCode, fileStoreLocation, files, keepOriginalFile);
                return result[0];
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

        public static List<FileUploadInfo> UploadFile(string clientCode, string fileStoreLocation, List<FileHolder> files)
        {
            return UploadFile(clientCode, fileStoreLocation, files, false);
        }

        public static FileUploadInfo UploadFile(string clientCode, string fileStoreLocation, MemoryStream stream, string fileName)
        {
            return UploadFile(clientCode, fileStoreLocation, stream, fileName, false);
        }

        /// <summary>
        /// Tai 1 file len he thong FSS
        /// </summary>
        /// <param name="clientCode">Ma client (chi chua ky tu alpabet va ky tu gach chan "_")</param>
        /// <param name="fileStoreLocation">Thu muc ma client muon luu tru</param>
        /// <param name="stream">Du lieu file can upload</param>
        /// <param name="fileName">Ten file</param>
        /// <returns>
        /// - Neu thanh cong, tra ve duong dan luu tru cua file tren he thong FSS (khong bao gom base-address)
        /// - Neu that bai, tra ve FileUploadException
        /// </returns>
        public static FileUploadInfo UploadFile(string clientCode, string fileStoreLocation, MemoryStream stream, string fileName, bool keepOriginalFile, string baseUri)
        {
            try
            {
                stream.Position = 0;
                List<FileHolder> files = new List<FileHolder>() { new FileHolder(stream, fileName) };
                List<FileUploadInfo> result = UploadFile(clientCode, fileStoreLocation, files, keepOriginalFile, baseUri);
                return result[0];
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
        public static List<FileUploadInfo> UploadFile(string clientCode, string fileStoreLocation, List<FileHolder> files, bool keepOriginalFile, string baseUri)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.Timeout = new TimeSpan(0, 0, FssConstant.TIME_OUT);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_CLIENT_CODE, clientCode);
                    client.DefaultRequestHeaders.Add(FssConstant.HEADER_FILE_STORE_LOCATION, fileStoreLocation);

                    ProcessHttpsCertificate(baseUri);

                    using (var content = new MultipartFormDataContent())
                    {
                        foreach (FileHolder file in files)
                        {
                            content.Add(new StreamContent(file.Content), "file", file.FileName);
                        }

                        using (HttpResponseMessage message = client.PostAsync(string.Format("{0}?keepOriginalFileName={1}", FssConstant.UPLOAD_URI, keepOriginalFile), content).Result)
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
