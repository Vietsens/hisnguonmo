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
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Worker đồng bộ dữ liệu Khám chữa bệnh (Kết thúc khám/Xuất viện) lên CSDL dùng chung
    /// ngành Y tế theo Quyết định 4750/QĐ-BYT.
    /// - Mục 3: đăng nhập lấy phiên làm việc (/get-token).
    /// - Mục 6: đồng bộ file XML (mã hoá base64) (/csdl-4750/import-csdl-4750-by-xml-file).
    /// Worker tự quản lý token, chủ động lấy lại token khi hết hạn hoặc bị trả về 401.
    /// Thông tin kết nối lấy từ config HIS.CSDL_4750.CONNECTION_INFO:
    ///   BaseURL | username | password | loginApi | checkinApi [| examApi | importXmlApi]
    /// Feature này (Kết thúc khám/Xuất viện = mục 6) dùng loginApi (mục 3) và importXmlApi (mục 6).
    /// </summary>
    public class Csdl4750Worker
    {
        //Đường dẫn mặc định theo tài liệu QĐ 4750 (dùng khi config không khai báo)
        private const string DEFAULT_LOGIN_PATH = "get-token";
        private const string DEFAULT_IMPORT_XML_PATH = "csdl-4750/import-csdl-4750-by-xml-file";
        //Trừ hao vài giây để chủ động lấy lại token trước khi thực sự hết hạn
        private const int TOKEN_SAFETY_MARGIN_SECOND = 60;

        private readonly string baseUrl;
        private readonly string username;
        private readonly string password;
        private readonly string loginApi;
        private readonly string importXmlApi;

        private string token;
        private DateTime tokenExpireTime = DateTime.MinValue;

        /// <summary>
        /// True khi config HIS.CSDL_4750.CONNECTION_INFO có đủ Base URL | username | password.
        /// </summary>
        public bool IsValidConfig { get; private set; }

        public Csdl4750Worker(string connectionInfo)
        {
            try
            {
                //Mặc định endpoint theo tài liệu; sẽ ghi đè nếu config khai báo
                this.loginApi = DEFAULT_LOGIN_PATH;
                this.importXmlApi = DEFAULT_IMPORT_XML_PATH;

                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    //BaseURL | username | password | loginApi | checkinApi [| examApi | importXmlApi]
                    string[] parts = connectionInfo.Split('|');
                    if (parts.Length >= 3)
                    {
                        this.baseUrl = (parts[0] ?? "").Trim();
                        this.username = (parts[1] ?? "").Trim();
                        this.password = (parts[2] ?? "").Trim();
                    }
                    //loginApi (mục 3)
                    if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                    {
                        this.loginApi = parts[3].Trim();
                    }
                    //importXmlApi (mục 6) - trường cuối, tuỳ chọn
                    if (parts.Length > 6 && !string.IsNullOrWhiteSpace(parts[6]))
                    {
                        this.importXmlApi = parts[6].Trim();
                    }
                    //parts[4]=checkinApi (mục 4), parts[5]=examApi (mục 5) không dùng cho feature này
                }

                this.IsValidConfig = !string.IsNullOrEmpty(this.baseUrl)
                    && !string.IsNullOrEmpty(this.username)
                    && !string.IsNullOrEmpty(this.password);

                if (!this.IsValidConfig)
                {
                    LogSystem.Warn("Csdl4750Worker - Cau hinh HIS.CSDL_4750.CONNECTION_INFO khong hop le. Can dinh dang: BaseURL | username | password | loginApi | checkinApi [| examApi | importXmlApi]");
                }

                //Đảm bảo bắt tay được TLS 1.2 với cổng HTTPS trên .NET Framework 4.5
                try
                {
                    ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
                }
                catch (Exception exTls)
                {
                    LogSystem.Warn(exTls);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                this.IsValidConfig = false;
            }
        }

        private string BuildUrl(string path)
        {
            return this.baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        /// <summary>
        /// Đảm bảo token còn hiệu lực; nếu chưa có hoặc đã hết hạn thì đăng nhập lấy token mới.
        /// </summary>
        private async Task<bool> EnsureTokenAsync()
        {
            if (!string.IsNullOrEmpty(this.token) && DateTime.Now < this.tokenExpireTime)
            {
                return true;
            }
            return await LoginAsync();
        }

        /// <summary>
        /// Mục 3 - API đăng nhập lấy phiên làm việc (/get-token).
        /// </summary>
        private async Task<bool> LoginAsync()
        {
            try
            {
                this.token = null;
                this.tokenExpireTime = DateTime.MinValue;

                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(this.username), "username");
                    content.Add(new StringContent(this.password), "password");

                    string loginUrl = BuildUrl(this.loginApi);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
                    request.Content = content;

                    //Log toàn bộ dữ liệu input (mục 3 - đăng nhập)
                    await LogRequestAsync("Login", request);

                    HttpResponseMessage response = await client.SendAsync(request);

                    //Log toàn bộ dữ liệu output
                    string body = await LogResponseAsync("Login", response);

                    if (!response.IsSuccessStatusCode)
                    {
                        LogSystem.Warn("Csdl4750Worker - Dang nhap that bai. HttpStatus: " + (int)response.StatusCode + ". Body: " + body);
                        return false;
                    }

                    JObject json = JObject.Parse(body);
                    bool success = json.Value<bool?>("success") ?? false;
                    this.token = (json.Value<string>("token") ?? "").Trim();
                    //time: thời gian hiệu lực của token (phút)
                    int minutes = json.Value<int?>("time") ?? 0;

                    if (!success || string.IsNullOrEmpty(this.token))
                    {
                        LogSystem.Warn("Csdl4750Worker - Dang nhap khong tra ve token hop le. Message: " + json.Value<string>("message"));
                        this.token = null;
                        return false;
                    }

                    this.tokenExpireTime = minutes > 0
                        ? DateTime.Now.AddMinutes(minutes).AddSeconds(-TOKEN_SAFETY_MARGIN_SECOND)
                        : DateTime.Now.AddMinutes(5);

                    LogSystem.Info("Csdl4750Worker - Dang nhap thanh cong. Token het han luc: " + this.tokenExpireTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// Mục 6 - Đồng bộ dữ liệu KCB (Kết thúc khám/Xuất viện) bằng file XML mã hoá base64.
        /// Tự lấy lại token khi hết hạn hoặc bị trả về 401 rồi gửi lại 1 lần.
        /// </summary>
        /// <param name="xmlBytes">Nội dung file XML theo QĐ 4750 (chưa mã hoá base64).</param>
        /// <param name="maLk">Mã liên kết hồ sơ (dùng để ghi log).</param>
        /// <returns>Kết quả liên thông (Success + Message lấy từ API) để caller lưu trạng thái.</returns>
        public async Task<Csdl4750ImportResult> ImportXmlAsync(byte[] xmlBytes, string maLk)
        {
            Csdl4750ImportResult ret = new Csdl4750ImportResult();
            try
            {
                if (!this.IsValidConfig)
                {
                    ret.Message = "Cấu hình kết nối CSDL 4750 không hợp lệ";
                    return ret;
                }
                if (xmlBytes == null || xmlBytes.Length == 0)
                {
                    ret.Message = "Không có dữ liệu XML để đồng bộ";
                    LogSystem.Warn("Csdl4750Worker - Khong co du lieu XML de dong bo. MA_LK: " + maLk);
                    return ret;
                }
                if (!await EnsureTokenAsync())
                {
                    ret.Message = "Đăng nhập lấy token thất bại";
                    return ret;
                }

                ImportResult result = await PostImportAsync(xmlBytes, maLk);
                //Nếu token hết hạn/không hợp lệ (401) -> lấy lại token và gửi lại 1 lần
                if (result.Unauthorized)
                {
                    LogSystem.Info("Csdl4750Worker - Token bi tu choi (401), lay lai token va gui lai. MA_LK: " + maLk);
                    if (!await LoginAsync())
                    {
                        ret.Message = "Đăng nhập lại lấy token thất bại (401)";
                        return ret;
                    }
                    result = await PostImportAsync(xmlBytes, maLk);
                }

                ret.Success = result.Ok;
                ret.Message = result.Message;
                LogSystem.Info("Csdl4750Worker - Dong bo KCB 4750 MA_LK: " + maLk + " thanh cong: " + result.Ok + ". Message: " + result.Message);
                return ret;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ret.Success = false;
                ret.Message = "Lỗi ngoại lệ khi gửi: " + ex.Message;
                return ret;
            }
        }

        /// <summary>
        /// Gọi API import (mục 6). Trả về Unauthorized=true khi 401 để caller lấy lại token.
        /// </summary>
        private async Task<ImportResult> PostImportAsync(byte[] xmlBytes, string maLk)
        {
            ImportResult result = new ImportResult();
            try
            {
                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    //Server đọc trực tiếp file như XML thô (XmlSerializer.Deserialize -> GIAMDINHHS),
                    //nên gửi FILE upload (có filename) chứa NỘI DUNG XML NGUYÊN BẢN (không base64).
                    ByteArrayContent fileContent = new ByteArrayContent(xmlBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                    string fileName = (string.IsNullOrEmpty(maLk) ? "data" : maLk) + ".xml";
                    content.Add(fileContent, "file", fileName);

                    string importUrl = BuildUrl(this.importXmlApi);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, importUrl);
                    request.Content = content;
                    //Gửi scheme chuẩn "Bearer" (server bóc token theo "Bearer " phân biệt hoa/thường)
                    string bearerToken = (this.token ?? "").Trim();
                    if (bearerToken.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        bearerToken = bearerToken.Substring(7).Trim();
                    }
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + bearerToken);

                    //Log toàn bộ dữ liệu input (mục 6 - import XML). Kèm tóm tắt.
                    LogSystem.Info(string.Format(
                        "Csdl4750Worker - [REQUEST] Import XML tom tat. MA_LK: {0}, fileName: {1}, xmlBytes: {2}",
                        maLk, fileName, (xmlBytes != null ? xmlBytes.Length : 0)));
                    await LogRequestAsync("Import XML (MA_LK: " + maLk + ")", request);

                    HttpResponseMessage response = await client.SendAsync(request);

                    //Log toàn bộ dữ liệu output
                    result.Body = await LogResponseAsync("Import XML (MA_LK: " + maLk + ")", response);

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result.Unauthorized = true;
                        result.Message = "401 - Không có quyền truy cập: " + ExtractApiMessage(result.Body);
                        LogSystem.Warn("Csdl4750Worker - Import bi tu choi (401). Body: " + result.Body);
                        return result;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        //400/500: dữ liệu sai định dạng/cấu trúc/thiếu trường bắt buộc (mục 6.2)
                        result.Ok = false;
                        result.Message = string.Format("HTTP {0} - {1}", (int)response.StatusCode, ExtractApiMessage(result.Body));
                        LogSystem.Warn("Csdl4750Worker - Import that bai. HttpStatus: " + (int)response.StatusCode + ". Body: " + result.Body);
                        return result;
                    }

                    //HTTP 200: đánh giá kết quả theo JSON trả về (mục 6.2): TotalSucceed/Inserted/Updated/TotalFailed/ListFailed/Total
                    result.Ok = true;
                    result.Message = ExtractApiMessage(result.Body);
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(result.Body))
                        {
                            JObject j = JObject.Parse(result.Body);
                            if (j["TotalSucceed"] != null || j["TotalFailed"] != null)
                            {
                                int totalSucceed = j.Value<int?>("TotalSucceed") ?? 0;
                                int inserted = j.Value<int?>("Inserted") ?? 0;
                                int updated = j.Value<int?>("Updated") ?? 0;
                                int totalFailed = j.Value<int?>("TotalFailed") ?? 0;
                                int total = j.Value<int?>("Total") ?? 0;
                                string listFailed = j["ListFailed"] != null
                                    ? j["ListFailed"].ToString(Newtonsoft.Json.Formatting.None)
                                    : "";

                                //Hồ sơ được coi là thành công khi không có hồ sơ nào thất bại và có ít nhất 1 hồ sơ import được
                                result.Ok = totalFailed == 0 && totalSucceed > 0;
                                result.Message = string.Format(
                                    "TotalSucceed={0}, Inserted={1}, Updated={2}, TotalFailed={3}, Total={4}{5}",
                                    totalSucceed, inserted, updated, totalFailed, total,
                                    (totalFailed > 0 ? ", ListFailed=" + listFailed : ""));

                                LogSystem.Info("Csdl4750Worker - [KET QUA] MA_LK: " + maLk + ". " + result.Message);

                                if (!result.Ok)
                                {
                                    LogSystem.Warn("Csdl4750Worker - Import KCB 4750 co ho so that bai. MA_LK: " + maLk + ", ListFailed: " + listFailed);
                                }
                            }
                        }
                    }
                    catch (Exception exParse)
                    {
                        //Không parse được JSON kết quả -> giữ Ok theo HTTP 200 nhưng cảnh báo
                        LogSystem.Warn("Csdl4750Worker - Khong parse duoc ket qua import theo muc 6.2: " + exParse.Message + ". Body: " + result.Body);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result.Ok = false;
                result.Message = "Lỗi ngoại lệ khi gửi: " + ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Rút gọn thông điệp trả về từ API (ưu tiên field "message"/"title", nếu không có thì lấy body) và cắt độ dài.
        /// </summary>
        private static string ExtractApiMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return "";
            }
            string msg = body;
            try
            {
                JObject j = JObject.Parse(body);
                string m = j.Value<string>("message");
                if (string.IsNullOrEmpty(m))
                {
                    m = j.Value<string>("title");
                }
                if (!string.IsNullOrEmpty(m))
                {
                    msg = m;
                }
            }
            catch
            {
                //body không phải JSON -> giữ nguyên
            }
            //Cắt bớt để vừa cột DESC (VARCHAR2 4000 BYTE); tiếng Việt UTF-8 nhiều byte nên giới hạn an toàn
            if (msg != null && msg.Length > 1000)
            {
                msg = msg.Substring(0, 1000);
            }
            return msg;
        }

        /// <summary>
        /// Ghi log đầy đủ HTTP request: method, URL, toàn bộ header (kể cả Authorization, Content-Type/boundary)
        /// và body thật sự (multipart) gửi lên server.
        /// </summary>
        private static async Task LogRequestAsync(string tag, HttpRequestMessage request)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Csdl4750Worker - [REQUEST] " + tag);
                sb.AppendLine(request.Method + " " + request.RequestUri);
                sb.AppendLine("Headers:");
                sb.Append(DumpHeaders(request.Headers));
                if (request.Content != null)
                {
                    sb.Append(DumpHeaders(request.Content.Headers));
                    string body = await request.Content.ReadAsStringAsync();
                    sb.AppendLine("Body:");
                    sb.Append(body);
                }
                LogSystem.Info(sb.ToString());
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Csdl4750Worker - Loi ghi log request: " + ex.Message);
            }
        }

        /// <summary>
        /// Ghi log đầy đủ HTTP response: status code + reason, toàn bộ header và body. Trả về body để tái sử dụng.
        /// </summary>
        private static async Task<string> LogResponseAsync(string tag, HttpResponseMessage response)
        {
            string body = "";
            try
            {
                body = await response.Content.ReadAsStringAsync();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Csdl4750Worker - [RESPONSE] " + tag);
                sb.AppendLine("HttpStatus: " + (int)response.StatusCode + " " + response.ReasonPhrase);
                sb.AppendLine("Headers:");
                sb.Append(DumpHeaders(response.Headers));
                if (response.Content != null)
                {
                    sb.Append(DumpHeaders(response.Content.Headers));
                }
                sb.AppendLine("Body:");
                sb.Append(body);
                LogSystem.Info(sb.ToString());
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Csdl4750Worker - Loi ghi log response: " + ex.Message);
            }
            return body;
        }

        private static string DumpHeaders(System.Net.Http.Headers.HttpHeaders headers)
        {
            StringBuilder sb = new StringBuilder();
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    sb.Append("  ").Append(h.Key).Append(": ").AppendLine(string.Join(", ", h.Value));
                }
            }
            return sb.ToString();
        }

        private class ImportResult
        {
            public bool Ok { get; set; }
            public bool Unauthorized { get; set; }
            public string Body { get; set; }
            public string Message { get; set; }
        }
    }

    /// <summary>
    /// Kết quả liên thông CSDL 4750 trả cho caller (lấy Success + Message từ API để lưu trạng thái).
    /// </summary>
    public class Csdl4750ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
