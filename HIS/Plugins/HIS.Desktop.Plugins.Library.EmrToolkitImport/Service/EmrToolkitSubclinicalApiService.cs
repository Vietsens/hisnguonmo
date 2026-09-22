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
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Config;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult;
using Inventec.Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Service
{
    /// <summary>
    /// Reads subclinical results another facility shared on the EMRToolkit gateway:
    /// api/LabResult/CheckValidity, api/LabResult/RequestView and api/LabResult/DownloadPdf,
    /// plus the login call they depend on.
    ///
    /// Pushing results of this hospital is NOT done here — a background job in MOS owns it.
    ///
    /// Sensitive values (password, token, OTP, patient identifier) are never logged.
    /// </summary>
    internal class EmrToolkitSubclinicalApiService
    {
        private const string HEADER_TOKEN_CODE = "tokencode";
        private const string URI_CREATE_TOKEN = "/api/Token/CreateToken";
        private const string URI_CHECK_VALIDITY = "/api/LabResult/CheckValidity";
        private const string URI_REQUEST_VIEW = "/api/LabResult/RequestView";
        private const string URI_DOWNLOAD_PDF = "/api/LabResult/DownloadPdf";

        private const string TEMP_FOLDER_NAME = "EmrToolkitLabResult";

        /// <summary>Configuration of the current flow, loaded once per service instance.</summary>
        private EmrToolkitSubclinicalConfigCFG config;

        /// <summary>Token of the current flow — one login per flow, not per call.</summary>
        private string token;

        #region Read flow

        /// <summary>
        /// GET api/LabResult/CheckValidity — records still valid for a patient.
        /// An empty list with an empty message means the patient simply has nothing shared.
        /// </summary>
        /// <param name="soDinhDanhBenhNhan">Patient identification number</param>
        /// <param name="message">Failure reason, empty when the call succeeded</param>
        /// <returns>Records on the gateway, never null</returns>
        internal List<LabResultValidityADO> CheckValidity(string soDinhDanhBenhNhan, out string message)
        {
            message = "";
            List<LabResultValidityADO> data = new List<LabResultValidityADO>();
            try
            {
                if (!PrepareCall(out message)) return data;

                if (string.IsNullOrWhiteSpace(soDinhDanhBenhNhan))
                {
                    message = Resources.ResourceMessage.ThieuSoDinhDanhBenhNhan;
                    return data;
                }

                string url = this.config.BaseUrl + URI_CHECK_VALIDITY
                    + "?soDinhDanhBenhNhan=" + Uri.EscapeDataString(soDinhDanhBenhNhan.Trim());

                string raw;
                bool httpOk = GetJson(url, out raw);
                EmrOutput<List<LabResultValidityADO>> output = DeserializeOutput<List<LabResultValidityADO>>(raw);

                if (!httpOk || output == null || !output.Success)
                {
                    message = output != null && !string.IsNullOrWhiteSpace(output.Message)
                        ? output.Message
                        : Resources.ResourceMessage.KiemTraHieuLucThatBai;
                    return data;
                }

                if (output.Data != null) data = output.Data;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogSystem.Error("EmrToolkitSubclinicalApiService.CheckValidity thất bại.", ex);
            }
            return data;
        }

        /// <summary>
        /// POST api/LabResult/RequestView — asks the gateway to send the patient an OTP.
        /// The gateway rate limits this to one call every five seconds per patient.
        /// </summary>
        /// <param name="request">Lookup keys; SoDinhDanhBenhNhan is required</param>
        /// <param name="message">Failure reason, empty when the call succeeded</param>
        /// <returns>Transaction data, null on failure</returns>
        internal RequestViewResultADO RequestView(RequestViewRequestADO request, out string message)
        {
            message = "";
            try
            {
                if (!PrepareCall(out message)) return null;

                if (request == null || string.IsNullOrWhiteSpace(request.SoDinhDanhBenhNhan))
                {
                    message = Resources.ResourceMessage.ThieuSoDinhDanhBenhNhan;
                    return null;
                }

                if (string.IsNullOrWhiteSpace(request.MaCSKCB))
                {
                    request.MaCSKCB = this.config.MaCskcb;
                }

                string raw;
                string url = this.config.BaseUrl + URI_REQUEST_VIEW;
                bool httpOk = PostJson(url, request, out raw);
                EmrOutput<RequestViewResultADO> output = DeserializeOutput<RequestViewResultADO>(raw);

                if (!httpOk || output == null || !output.Success
                    || output.Data == null || string.IsNullOrWhiteSpace(output.Data.TransactionId))
                {
                    message = output != null && !string.IsNullOrWhiteSpace(output.Message)
                        ? output.Message
                        : Resources.ResourceMessage.XinMaOtpThatBai;
                    return null;
                }

                return output.Data;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogSystem.Error("EmrToolkitSubclinicalApiService.RequestView thất bại.", ex);
            }
            return null;
        }

        /// <summary>
        /// POST api/LabResult/DownloadPdf — downloads the result form(s).
        /// The gateway returns a single pdf, or a zip when the batch spans several forms;
        /// a zip is extracted so the caller always gets plain pdf paths.
        ///
        /// The OTP session is consumed by this call: viewing again needs a new OTP.
        /// </summary>
        /// <param name="request">Transaction id plus the OTP typed in by the user</param>
        /// <returns>Local pdf paths, never null</returns>
        internal LabResultPdfResultADO DownloadPdf(DownloadPdfRequestADO request)
        {
            LabResultPdfResultADO result = new LabResultPdfResultADO();
            try
            {
                string message;
                if (!PrepareCall(out message))
                {
                    result.Success = false;
                    result.Message = message;
                    return result;
                }

                if (request == null
                    || string.IsNullOrWhiteSpace(request.TransactionId)
                    || string.IsNullOrWhiteSpace(request.OTP))
                {
                    result.Success = false;
                    result.Message = Resources.ResourceMessage.ThieuTransactionIdHoacOtp;
                    return result;
                }

                byte[] content;
                string contentType;
                string errorBody;
                string url = this.config.BaseUrl + URI_DOWNLOAD_PDF;

                if (!PostForFile(url, request, out content, out contentType, out errorBody))
                {
                    result.Success = false;
                    result.Message = ExtractMessage(errorBody, Resources.ResourceMessage.TaiPhieuKetQuaThatBai);
                    return result;
                }

                if (content == null || content.Length == 0)
                {
                    result.Success = false;
                    result.Message = Resources.ResourceMessage.TaiPhieuKetQuaThatBai;
                    return result;
                }

                result.OutputFolder = CreateOutputFolder();
                if (IsZipContent(content, contentType))
                    ExtractZipToPdfFiles(content, result);
                else
                    SaveSinglePdf(content, result);

                result.Success = result.PdfFilePaths.Count > 0;
                if (!result.Success && string.IsNullOrWhiteSpace(result.Message))
                {
                    result.Message = Resources.ResourceMessage.TaiPhieuKetQuaThatBai;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                LogSystem.Error("EmrToolkitSubclinicalApiService.DownloadPdf thất bại.", ex);
            }
            return result;
        }

        #endregion

        #region Shared steps

        /// <summary>Loads the configuration, enables TLS 1.2 and makes sure a token is held.</summary>
        private bool PrepareCall(out string message)
        {
            message = "";

            this.config = EmrToolkitSubclinicalConfigCFG.Load();
            if (this.config == null)
            {
                message = Resources.ResourceMessage.ChuaCauHinhKetNoiEmrToolkit;
                return false;
            }

            EnableTls12();

            if (string.IsNullOrEmpty(this.token) && !CreateToken())
            {
                message = Resources.ResourceMessage.KhongLayDuocTokenEmrToolkit;
                return false;
            }
            return true;
        }

        /// <summary>The gateway may run on https with a modern TLS version only.</summary>
        private void EnableTls12()
        {
            try
            {
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>POST api/Token/CreateToken. Stores the token for this service instance.</summary>
        private bool CreateToken()
        {
            try
            {
                CreateTokenRequestADO request = new CreateTokenRequestADO();
                request.TenDangNhap = this.config.Username;
                request.MatKhau = this.config.Password;

                string raw;
                string url = this.config.BaseUrl + URI_CREATE_TOKEN;
                if (!PostJson(url, request, out raw))
                {
                    LogSystem.Error("EMRTOOLKIT CreateToken lỗi HTTP.");
                    return false;
                }

                EmrOutput<TokenResultADO> output = DeserializeOutput<TokenResultADO>(raw);
                if (output == null || !output.Success || output.Data == null
                    || string.IsNullOrEmpty(output.Data.Token))
                {
                    LogSystem.Error("EMRTOOLKIT CreateToken thất bại. Message="
                        + (output != null ? output.Message : ""));
                    return false;
                }

                this.token = output.Data.Token;

                // Facility code: configuration first, token owner as fallback
                if (string.IsNullOrWhiteSpace(this.config.MaCskcb))
                {
                    LogSystem.Debug("EMRTOOLKIT: dùng mã CSKCB theo token của cổng");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error("EmrToolkitSubclinicalApiService.CreateToken thất bại.", ex);
                return false;
            }
        }

        #endregion

        #region HTTP helpers

        /// <summary>
        /// Sends a POST with a json body and the tokencode header.
        /// rawResponse always holds the body, including on failure, for logging.
        /// </summary>
        private bool PostJson(string url, object body, out string rawResponse)
        {
            rawResponse = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(this.config.TimeoutSecond);

                    string json = JsonConvert.SerializeObject(body);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        message.Content = content;
                        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(this.token))
                        {
                            message.Headers.TryAddWithoutValidation(HEADER_TOKEN_CODE, this.token);
                        }

                        HttpResponseMessage response = client.SendAsync(message).Result;
                        rawResponse = response.Content != null
                            ? response.Content.ReadAsStringAsync().Result : null;

                        if (!response.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("POST {0} lỗi HTTP {1}. Message={2}",
                                url, (int)response.StatusCode, ExtractMessage(rawResponse, "")));
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("PostJson lỗi. Url=" + url, ex);
                return false;
            }
        }

        /// <summary>Sends a GET with the tokencode header.</summary>
        private bool GetJson(string url, out string rawResponse)
        {
            rawResponse = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(this.config.TimeoutSecond);

                    using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(this.token))
                        {
                            message.Headers.TryAddWithoutValidation(HEADER_TOKEN_CODE, this.token);
                        }

                        HttpResponseMessage response = client.SendAsync(message).Result;
                        rawResponse = response.Content != null
                            ? response.Content.ReadAsStringAsync().Result : null;

                        if (!response.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format("GET {0} lỗi HTTP {1}. Message={2}",
                                url, (int)response.StatusCode, ExtractMessage(rawResponse, "")));
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("GetJson lỗi. Url=" + url, ex);
                return false;
            }
        }

        /// <summary>
        /// Sends a POST whose successful response is a binary file (pdf or zip).
        /// On failure the body is json and is returned through errorBody.
        /// </summary>
        private bool PostForFile(string url, object body,
            out byte[] content, out string contentType, out string errorBody)
        {
            content = null;
            contentType = null;
            errorBody = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(this.config.TimeoutSecond);

                    string json = JsonConvert.SerializeObject(body);
                    StringContent requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                    using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        message.Content = requestContent;
                        if (!string.IsNullOrEmpty(this.token))
                        {
                            message.Headers.TryAddWithoutValidation(HEADER_TOKEN_CODE, this.token);
                        }

                        HttpResponseMessage response = client.SendAsync(message).Result;
                        if (response.Content != null && response.Content.Headers != null
                            && response.Content.Headers.ContentType != null)
                        {
                            contentType = response.Content.Headers.ContentType.MediaType;
                        }

                        if (!response.IsSuccessStatusCode)
                        {
                            errorBody = response.Content != null
                                ? response.Content.ReadAsStringAsync().Result : null;
                            LogSystem.Error(string.Format("POST {0} lỗi HTTP {1}. Message={2}",
                                url, (int)response.StatusCode, ExtractMessage(errorBody, "")));
                            return false;
                        }

                        // A json body on a 200 means the gateway reported a logical failure
                        if (!string.IsNullOrEmpty(contentType)
                            && contentType.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            errorBody = response.Content.ReadAsStringAsync().Result;
                            return false;
                        }

                        content = response.Content.ReadAsByteArrayAsync().Result;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("PostForFile lỗi. Url=" + url, ex);
                return false;
            }
        }

        /// <summary>
        /// Parses a gateway response into EmrOutput of T.
        /// The gateway may answer with a single object or an array holding one.
        /// </summary>
        private EmrOutput<T> DeserializeOutput<T>(string raw)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return null;

                JToken parsed = JToken.Parse(raw);
                if (parsed.Type == JTokenType.Array)
                {
                    JArray array = (JArray)parsed;
                    if (array.Count == 0) return null;
                    parsed = array[0];
                }
                return parsed.ToObject<EmrOutput<T>>();
            }
            catch (Exception ex)
            {
                LogSystem.Error("DeserializeOutput lỗi.", ex);
                return null;
            }
        }

        /// <summary>Pulls the Message field out of an error body, falling back to a default.</summary>
        private string ExtractMessage(string raw, string defaultMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return defaultMessage;

                JToken parsed = JToken.Parse(raw);
                if (parsed.Type == JTokenType.Array && ((JArray)parsed).Count > 0)
                {
                    parsed = ((JArray)parsed)[0];
                }

                JToken messageToken = parsed["Message"];
                string message = messageToken != null ? messageToken.ToString() : null;
                return !string.IsNullOrWhiteSpace(message) ? message : defaultMessage;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return defaultMessage;
            }
        }

        #endregion

        #region File helpers

        /// <summary>Creates a run specific folder under the user temp directory.</summary>
        private string CreateOutputFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), TEMP_FOLDER_NAME,
                DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return folder;
        }

        /// <summary>A zip always starts with PK; the content type alone is not reliable.</summary>
        private bool IsZipContent(byte[] content, string contentType)
        {
            if (!string.IsNullOrEmpty(contentType)
                && contentType.IndexOf("zip", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
            return content != null && content.Length > 1 && content[0] == 0x50 && content[1] == 0x4B;
        }

        /// <summary>Writes a single pdf response to disk.</summary>
        private void SaveSinglePdf(byte[] content, LabResultPdfResultADO result)
        {
            try
            {
                string filePath = Path.Combine(result.OutputFolder, "PhieuXetNghiem.pdf");
                File.WriteAllBytes(filePath, content);
                result.PdfFilePaths.Add(filePath);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                LogSystem.Error("SaveSinglePdf lỗi.", ex);
            }
        }

        /// <summary>Extracts every pdf entry of a zip response to disk.</summary>
        private void ExtractZipToPdfFiles(byte[] content, LabResultPdfResultADO result)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(content))
                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;
                        if (!entry.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) continue;

                        string filePath = Path.Combine(result.OutputFolder, Path.GetFileName(entry.Name));
                        entry.ExtractToFile(filePath, true);
                        result.PdfFilePaths.Add(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                LogSystem.Error("ExtractZipToPdfFiles lỗi.", ex);
            }
        }

        #endregion
    }
}
