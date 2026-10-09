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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Worker đồng bộ hồ sơ KCB (Kết thúc khám/Xuất viện) lên Cổng CSDL Y tế Cần Thơ theo QĐ 3176
    /// (tài liệu "Tích hợp API CSDL Y tế Cần Thơ — 3176" + Phụ lục V QĐ 1977/QĐ-SYT):
    /// - POST {BaseURL}/api/get-token (multipart username/password) -> {success, token, time(phút)}.
    /// - POST {BaseURL}/api/csdl-3176/import-csdl-3176-by-xml-file-khong-dong-bo (multipart: file = XML tổng
    ///   GIAMDINHHS, từng XML con trong NOIDUNGFILE đã Base64; type = 3176) -> response3176.
    /// Cổng riêng, tách khỏi Csdl4750Worker để viện đang chạy 4750 không bị ảnh hưởng.
    /// Khóa HIS.CSDL_CANTHO_3176.CONNECTION_INFO: BaseURL | username | password [| loginApi | importApi].
    /// </summary>
    public class CsdlCt3176Worker
    {
        //Đường dẫn mặc định theo tài liệu Cần Thơ (dùng khi khóa không khai báo)
        private const string DEFAULT_LOGIN_PATH = "api/get-token";
        private const string DEFAULT_IMPORT_PATH = "api/csdl-3176/import-csdl-3176-by-xml-file-khong-dong-bo";
        private const int TOKEN_SAFETY_MARGIN_SECOND = 60;
        private const int HTTP_TIMEOUT_SECOND = 60;
        //Cắt bớt để vừa cột CSDL4750_FINISH_DESC (VARCHAR2 4000 BYTE, tiếng Việt UTF-8 nhiều byte)
        private const int MAX_MESSAGE_LENGTH = 1000;
        private const string TAG = "CsdlCt3176Worker";

        private readonly string baseUrl;
        private readonly string username;
        private readonly string password;
        private readonly string loginApi;
        private readonly string importApi;

        private string token;
        private DateTime tokenExpireTime = DateTime.MinValue;

        //Nối tuần tự token + import trên cùng worker (nhiều hồ sơ đẩy song song)
        private readonly System.Threading.SemaphoreSlim importGate = new System.Threading.SemaphoreSlim(1, 1);

        /// <summary>True khi khóa có đủ BaseURL | username | password.</summary>
        public bool IsValidConfig { get; private set; }

        public CsdlCt3176Worker(string connectionInfo)
        {
            try
            {
                this.loginApi = DEFAULT_LOGIN_PATH;
                this.importApi = DEFAULT_IMPORT_PATH;

                if (!string.IsNullOrWhiteSpace(connectionInfo))
                {
                    //BaseURL | username | password [| loginApi | importApi]
                    string[] parts = connectionInfo.Split('|');
                    if (parts.Length >= 3)
                    {
                        this.baseUrl = (parts[0] ?? "").Trim();
                        this.username = (parts[1] ?? "").Trim();
                        this.password = (parts[2] ?? "").Trim();
                    }
                    if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                    {
                        this.loginApi = parts[3].Trim();
                    }
                    if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                    {
                        this.importApi = parts[4].Trim();
                    }
                }

                this.IsValidConfig = !string.IsNullOrEmpty(this.baseUrl)
                    && !string.IsNullOrEmpty(this.username)
                    && !string.IsNullOrEmpty(this.password);

                if (!this.IsValidConfig)
                {
                    LogSystem.Warn(TAG + " - Cau hinh HIS.CSDL_CANTHO_3176.CONNECTION_INFO khong hop le. Can dinh dang: BaseURL | username | password [| loginApi | importApi]");
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

        /// <summary>
        /// Ghép BaseURL + path. BaseURL khai sẵn đuôi "/api" mà path cũng bắt đầu bằng "api/"
        /// thì bỏ bớt một lần để không thành ".../api/api/...".
        /// </summary>
        private string BuildUrl(string path)
        {
            string b = this.baseUrl.TrimEnd('/');
            string p = (path ?? "").TrimStart('/');
            if (b.EndsWith("/api", StringComparison.OrdinalIgnoreCase)
                && p.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
            {
                p = p.Substring(4);
            }
            return b + "/" + p;
        }

        private async Task<bool> EnsureTokenAsync()
        {
            if (!string.IsNullOrEmpty(this.token) && DateTime.Now < this.tokenExpireTime)
            {
                return true;
            }
            return await LoginAsync();
        }

        /// <summary>POST /api/get-token (multipart username/password).</summary>
        private async Task<bool> LoginAsync()
        {
            try
            {
                this.token = null;
                this.tokenExpireTime = DateTime.MinValue;

                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    content.Add(new StringContent(this.username), "username");
                    content.Add(new StringContent(this.password), "password");

                    string loginUrl = BuildUrl(this.loginApi);
                    HttpResponseMessage response = await client.PostAsync(loginUrl, content);
                    string body = await response.Content.ReadAsStringAsync();

                    //Không log body đăng nhập (chứa token còn hiệu lực) — chỉ log trạng thái + message.
                    JObject json = TryParse(body);
                    string message = (json != null) ? (json.Value<string>("message") ?? "") : Truncate(body);
                    if (!response.IsSuccessStatusCode)
                    {
                        LogSystem.Warn(TAG + " - Dang nhap that bai. url=" + loginUrl + "; HttpStatus=" + (int)response.StatusCode + "; message=" + message);
                        return false;
                    }

                    bool success = json != null && (json.Value<bool?>("success") ?? false);
                    this.token = (json != null) ? (json.Value<string>("token") ?? "").Trim() : "";
                    //time: thời gian hiệu lực token (phút) — cổng trả dạng chuỗi, vd "180"
                    int minutes = 0;
                    if (json != null && json["time"] != null)
                    {
                        int.TryParse(json["time"].ToString(), out minutes);
                    }

                    if (!success || string.IsNullOrEmpty(this.token))
                    {
                        LogSystem.Warn(TAG + " - Dang nhap khong tra ve token hop le. message=" + message);
                        this.token = null;
                        return false;
                    }

                    this.tokenExpireTime = minutes > 0
                        ? DateTime.Now.AddMinutes(minutes).AddSeconds(-TOKEN_SAFETY_MARGIN_SECOND)
                        : DateTime.Now.AddMinutes(5);

                    LogSystem.Info(TAG + " - Dang nhap thanh cong. Token het han luc: " + this.tokenExpireTime.ToString("yyyy-MM-dd HH:mm:ss"));
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
        /// Đẩy 1 file XML tổng GIAMDINHHS (QĐ 3176). Tự lấy lại token khi hết hạn hoặc bị 401 rồi gửi lại 1 lần.
        /// Message trả về có IdLanGui để đối soát trên cổng.
        /// </summary>
        public async Task<Csdl4750ImportResult> ImportXmlAsync(byte[] xmlBytes, string maLk)
        {
            Csdl4750ImportResult ret = new Csdl4750ImportResult();
            try
            {
                if (!this.IsValidConfig)
                {
                    ret.Message = "CT: cấu hình kết nối cổng CSDL Y tế Cần Thơ không hợp lệ";
                    return ret;
                }
                if (xmlBytes == null || xmlBytes.Length == 0)
                {
                    ret.Message = "CT: không có dữ liệu XML để đồng bộ";
                    return ret;
                }
                await this.importGate.WaitAsync();
                try
                {
                    if (!await EnsureTokenAsync())
                    {
                        ret.Message = "CT: đăng nhập lấy token thất bại";
                        return ret;
                    }

                    ImportResult result = await PostImportAsync(xmlBytes, maLk);
                    if (result.Unauthorized)
                    {
                        LogSystem.Info(TAG + " - Token bi tu choi (401), lay lai token va gui lai. MA_LK: " + maLk);
                        if (!await LoginAsync())
                        {
                            ret.Message = "CT: đăng nhập lại lấy token thất bại (401)";
                            return ret;
                        }
                        result = await PostImportAsync(xmlBytes, maLk);
                    }

                    ret.Success = result.Ok;
                    ret.Message = "CT: " + result.Message;
                    LogSystem.Info(TAG + " - Dong bo KCB 3176 MA_LK: " + maLk + " thanh cong: " + result.Ok + ". " + result.Message);
                    return ret;
                }
                finally
                {
                    this.importGate.Release();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                ret.Success = false;
                ret.Message = "CT: lỗi ngoại lệ khi gửi: " + ex.Message;
                return ret;
            }
        }

        private async Task<ImportResult> PostImportAsync(byte[] xmlBytes, string maLk)
        {
            ImportResult result = new ImportResult();
            try
            {
                using (HttpClient client = new HttpClient())
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    client.Timeout = TimeSpan.FromSeconds(HTTP_TIMEOUT_SECOND);
                    //Gửi nguyên file XML tổng GIAMDINHHS (các XML con đã Base64 trong NOIDUNGFILE).
                    ByteArrayContent fileContent = new ByteArrayContent(xmlBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                    string fileName = (string.IsNullOrEmpty(maLk) ? "data" : maLk) + ".xml";
                    content.Add(fileContent, "file", fileName);
                    content.Add(new StringContent("3176"), "type");

                    string importUrl = BuildUrl(this.importApi);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, importUrl);
                    request.Content = content;
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + (this.token ?? "").Trim());

                    HttpResponseMessage response = await client.SendAsync(request);
                    string body = await response.Content.ReadAsStringAsync();
                    //Chỉ log trạng thái + nội dung đã cắt ngắn (không log XML gửi đi — có dữ liệu bệnh nhân).
                    LogSystem.Info(TAG + " - [RESPONSE] MA_LK: " + maLk + "; HttpStatus=" + (int)response.StatusCode + "; body=" + Truncate(body));

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result.Unauthorized = true;
                        result.Message = "401 - Không có quyền truy cập";
                        return result;
                    }

                    JObject json = TryParse(body);
                    if (!response.IsSuccessStatusCode)
                    {
                        result.Ok = false;
                        result.Message = string.Format("HTTP {0} - {1}", (int)response.StatusCode, ExtractApiMessage(json, body));
                        return result;
                    }

                    //HTTP 200 chưa chắc thành công: đọc response3176 (hoặc các trường ở gốc JSON).
                    JObject r = (json != null && json["response3176"] is JObject) ? (JObject)json["response3176"] : json;
                    if (r == null || (r["TotalSucceed"] == null && r["TotalFailed"] == null && r["IdLanGui"] == null))
                    {
                        bool? success = (json != null) ? json.Value<bool?>("success") : null;
                        result.Ok = success != false;
                        result.Message = "Không đọc được kết quả response3176 - " + ExtractApiMessage(json, body);
                        return result;
                    }

                    int totalSucceed = ToInt(r["TotalSucceed"]);
                    int inserted = ToInt(r["Inserted"]);
                    int updated = ToInt(r["Updated"]);
                    int totalFailed = ToInt(r["TotalFailed"]);
                    int total = ToInt(r["Total"]);
                    string idLanGui = (r["IdLanGui"] != null) ? r["IdLanGui"].ToString() : "";
                    List<string> errors = ReadListLoi(r["ListLoi"]);

                    //Thành công hoàn toàn: TotalFailed = 0 và ListLoi rỗng (tài liệu mục 11)
                    result.Ok = totalFailed == 0 && errors.Count == 0;
                    result.Message = string.Format(
                        "IdLanGui={0}, TotalSucceed={1}, Inserted={2}, Updated={3}, TotalFailed={4}, Total={5}{6}",
                        idLanGui, totalSucceed, inserted, updated, totalFailed, total,
                        (errors.Count > 0) ? (", ListLoi: " + string.Join("; ", errors.ToArray())) : "");
                    result.Message = Truncate(result.Message);
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

        /// <summary>ListLoi: [{MA_LK, CHI_TIET_LOI}] -> "MA_LK: CHI_TIET_LOI".</summary>
        private static List<string> ReadListLoi(JToken token)
        {
            List<string> errors = new List<string>();
            try
            {
                JArray arr = token as JArray;
                if (arr == null) return errors;
                foreach (JToken item in arr)
                {
                    JObject o = item as JObject;
                    if (o == null)
                    {
                        errors.Add(item.ToString());
                        continue;
                    }
                    string maLk = o.Value<string>("MA_LK") ?? "";
                    string detail = o.Value<string>("CHI_TIET_LOI") ?? o.ToString(Newtonsoft.Json.Formatting.None);
                    errors.Add(string.IsNullOrEmpty(maLk) ? detail : (maLk + ": " + detail));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return errors;
        }

        private static int ToInt(JToken t)
        {
            int v = 0;
            if (t != null) int.TryParse(t.ToString(), out v);
            return v;
        }

        private static JObject TryParse(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body)) return null;
                return JObject.Parse(body);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Ưu tiên field "message"/"title" của JSON lỗi, nếu không có thì lấy body (đã cắt).</summary>
        private static string ExtractApiMessage(JObject json, string body)
        {
            string msg = null;
            if (json != null)
            {
                msg = json.Value<string>("message");
                if (string.IsNullOrEmpty(msg)) msg = json.Value<string>("title");
                if (!string.IsNullOrEmpty(msg) && json["errors"] != null)
                {
                    msg += " " + json["errors"].ToString(Newtonsoft.Json.Formatting.None);
                }
            }
            return Truncate(string.IsNullOrEmpty(msg) ? body : msg);
        }

        private static string Truncate(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return (s.Length > MAX_MESSAGE_LENGTH) ? s.Substring(0, MAX_MESSAGE_LENGTH) : s;
        }

        private class ImportResult
        {
            public bool Ok { get; set; }
            public bool Unauthorized { get; set; }
            public string Message { get; set; }
        }
    }
}
