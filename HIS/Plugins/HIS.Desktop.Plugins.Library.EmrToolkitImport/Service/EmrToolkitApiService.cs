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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Config;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using Inventec.Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Service
{
    /// <summary>
    /// Lớp gọi REST API EMRTOOLKIT (token + mã hóa + import).
    /// Dùng System.Net.Http.HttpClient + Newtonsoft.Json (đồng bộ qua .Result).
    /// KHÔNG chứa UI — chỉ thuần gọi HTTP và trả kết quả.
    /// </summary>
    internal class EmrToolkitApiService
    {
        private const string HEADER_TOKEN_CODE = "tokencode";
        private const string URI_CREATE_TOKEN = "/api/Token/CreateToken";
        private const string URI_MA_HOA_JSON = "/api/EMR/MaHoaJson";
        private const string URI_IMPORT = "/api/EMR/v2/Import";

        /// <summary>(vCong 62522) Lỗi đã phân loại nguồn của bước HTTP gần nhất (các bước trả null khi lỗi)</summary>
        private string lastError;

        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.Emr("EMR Toolkit"); }
        }

        private void SetError(string message)
        {
            if (string.IsNullOrWhiteSpace(this.lastError)) this.lastError = message;
        }

        /// <summary>
        /// Thực hiện đầy đủ 3 bước: CreateToken -> MaHoaJson -> Import.
        /// </summary>
        /// <param name="model">Dữ liệu cần import (đã build sẵn)</param>
        /// <returns>Kết quả tổng hợp (không bao giờ null)</returns>
        internal EmrToolkitImportResult ImportEmr(EmrImportModel model)
        {
            EmrToolkitImportResult result = new EmrToolkitImportResult();
            result.Step = EmrToolkitImportStep.None;
            try
            {
                EmrToolkitConfigCFG.LoadConfig();

                if (model == null)
                {
                    result.Success = false;
                    result.Message = Inventec.Common.IntegrationError.IntegrationErrorMessage.SoftwareError(Partner, "Dữ liệu gửi đi (model) đang null", null); // vCong 62522
                    return result;
                }

                if (!EmrToolkitConfigCFG.HasConnectionInfo)
                {
                    result.Success = false;
                    result.Message = Inventec.Common.IntegrationError.IntegrationErrorMessage.ConfigError(Partner, "Chưa cấu hình kết nối EMRTOOLKIT (key " + EmrToolkitConfigCFG.CONFIG_KEY__CONNECTION_INFO + ")"); // vCong 62522
                    return result;
                }

                // Gắn IDMauPhieu từ cấu hình nếu chưa có
                if (model.IDMauPhieu <= 0)
                    model.IDMauPhieu = EmrToolkitConfigCFG.IdMauPhieuGiayChuyenVien;

                // Bật TLS 1.2 cho HTTPS endpoint ngoài
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                // ----- Bước 1: Lấy token (đăng nhập bằng ConnectionInfo) -----
                result.Step = EmrToolkitImportStep.CreateToken;
                this.lastError = null;
                TokenResultADO tokenData = CreateToken();
                if (tokenData == null || string.IsNullOrEmpty(tokenData.Token))
                {
                    result.Success = false;
                    // (vCong 62522) lý do thật: kết nối / sai tài khoản / EMR Toolkit trả lỗi
                    result.Message = this.lastError ?? Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(Partner, "Không lấy được token từ EMRTOOLKIT (CreateToken)");
                    return result;
                }
                string token = tokenData.Token;

                // Mã CSKCB: ưu tiên giá trị có sẵn trong model, nếu trống lấy theo token
                if (string.IsNullOrWhiteSpace(model.MaCoSoKhamChuaBenh))
                    model.MaCoSoKhamChuaBenh = tokenData.MaCSKCB;

                result.RawRequestJson = JsonConvert.SerializeObject(model, Formatting.Indented);

                // Log toàn bộ model gửi đi để xác nhận đã truyền đủ thông tin.
                // CẢNH BÁO: JSON chứa dữ liệu nhạy cảm bệnh nhân (tên, CCCD, địa chỉ, SĐT,
                // chẩn đoán). Chỉ dùng để debug — cân nhắc tắt khi lên production.
                LogSystem.Info("EMRTOOLKIT ImportEmr - Model gửi sang (MaHoaJson):" + Environment.NewLine + result.RawRequestJson);

                // ----- Bước 2: Mã hóa JSON -----
                result.Step = EmrToolkitImportStep.MaHoaJson;
                this.lastError = null;
                MaHoaJsonResultADO encrypted = MaHoaJson(model, token);
                if (encrypted == null
                    || string.IsNullOrEmpty(encrypted.DuLieu)
                    || string.IsNullOrEmpty(encrypted.KeyGiaiMa))
                {
                    result.Success = false;
                    result.Message = this.lastError ?? Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, "Kết quả mã hóa thiếu DuLieu/KeyGiaiMa", "Mã hóa JSON thất bại (MaHoaJson)"); // vCong 62522
                    return result;
                }

                // ----- Bước 3: Import -----
                result.Step = EmrToolkitImportStep.Import;
                ImportRequestADO importRequest = new ImportRequestADO
                {
                    IDMauPhieu = model.IDMauPhieu,
                    MaCSKCB = tokenData.MaCSKCB,
                    DuLieu = encrypted.DuLieu,
                    KeyGiaiMa = encrypted.KeyGiaiMa
                };

                string importRawResponse;
                this.lastError = null;
                EmrOutput<object> importOutput = Import(importRequest, token, out importRawResponse);
                result.RawResponseJson = importRawResponse;

                if (importOutput == null)
                {
                    result.Success = false;
                    result.Message = this.lastError ?? Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, importRawResponse, "API Import không trả về dữ liệu hợp lệ"); // vCong 62522
                    return result;
                }

                result.Success = importOutput.Success;
                result.ImportData = importOutput.Data;
                result.Message = importOutput.Message;
                if (!importOutput.Success)
                    result.Message = Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, importOutput.Message, "EMR Toolkit từ chối import"); // vCong 62522
                if (importOutput.Success)
                    result.Step = EmrToolkitImportStep.Completed;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "gửi dữ liệu sang EMR Toolkit"); // vCong 62522
                LogSystem.Error(
                    "EmrToolkitApiService.ImportEmr thất bại."
                    + LogUtil.TraceData(LogUtil.GetMemberName(() => model), model),
                    ex);
                return result;
            }
        }

        #region ----- Các bước HTTP -----

        /// <summary>Bước 1 — POST /api/Token/CreateToken, trả về thông tin token (Token + MaCSKCB).</summary>
        private TokenResultADO CreateToken()
        {
            try
            {
                CreateTokenRequestADO request = new CreateTokenRequestADO
                {
                    TenDangNhap = EmrToolkitConfigCFG.Username,
                    MatKhau = EmrToolkitConfigCFG.Password
                };

                string raw;
                string url = EmrToolkitConfigCFG.BaseUrl + URI_CREATE_TOKEN;
                if (!PostJson(url, request, null, out raw))
                    return null;

                EmrOutput<TokenResultADO> output = DeserializeOutput<TokenResultADO>(raw);
                if (output != null && output.Success && output.Data != null
                    && !string.IsNullOrEmpty(output.Data.Token))
                    return output.Data;

                LogSystem.Error("CreateToken EMRTOOLKIT thất bại. Response: " + raw);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(Partner, output != null && !string.IsNullOrWhiteSpace(output.Message) ? output.Message : raw)); // vCong 62522
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "lấy token")); // vCong 62522
                return null;
            }
        }

        /// <summary>Bước 2 — POST /api/EMR/MaHoaJson, trả về {DuLieu, KeyGiaiMa}.</summary>
        private MaHoaJsonResultADO MaHoaJson(EmrImportModel model, string token)
        {
            try
            {
                string raw;
                string url = EmrToolkitConfigCFG.BaseUrl + URI_MA_HOA_JSON;
                if (!PostJson(url, model, token, out raw))
                    return null;

                EmrOutput<MaHoaJsonResultADO> output = DeserializeOutput<MaHoaJsonResultADO>(raw);
                if (output != null && output.Success && output.Data != null)
                    return output.Data;

                LogSystem.Error("MaHoaJson EMRTOOLKIT thất bại. Response: " + raw);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, output != null && !string.IsNullOrWhiteSpace(output.Message) ? output.Message : raw, "Mã hóa JSON thất bại (MaHoaJson)")); // vCong 62522
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "mã hóa dữ liệu")); // vCong 62522
                return null;
            }
        }

        /// <summary>Bước 3 — POST /api/EMR/v2/Import.</summary>
        private EmrOutput<object> Import(ImportRequestADO request, string token, out string rawResponse)
        {
            rawResponse = null;
            try
            {
                string url = EmrToolkitConfigCFG.BaseUrl + URI_IMPORT;
                if (!PostJson(url, request, token, out rawResponse))
                    return null;

                return DeserializeOutput<object>(rawResponse);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "import dữ liệu")); // vCong 62522
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Parse phản hồi EMRTOOLKIT thành EmrOutput&lt;T&gt;.
        /// API có thể trả về 1 object đơn ({...}) hoặc 1 mảng ([{...}]);
        /// nếu là mảng thì lấy phần tử đầu tiên.
        /// </summary>
        private EmrOutput<T> DeserializeOutput<T>(string raw)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw))
                    return null;

                JToken token = JToken.Parse(raw);
                if (token.Type == JTokenType.Array)
                {
                    JArray array = (JArray)token;
                    if (array.Count == 0)
                        return null;
                    token = array[0];
                }
                return token.ToObject<EmrOutput<T>>();
            }
            catch (Exception ex)
            {
                LogSystem.Error("DeserializeOutput lỗi. Raw=" + raw, ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, "EMR Toolkit", raw, ex), Partner, null)); // vCong 62522
                return null;
            }
        }

        /// <summary>
        /// Gửi 1 POST JSON. Trả về true nếu HTTP 2xx; rawResponse luôn được gán
        /// nội dung body trả về (kể cả khi lỗi) để phục vụ log/hiển thị.
        /// </summary>
        private bool PostJson(string url, object body, string token, out string rawResponse)
        {
            rawResponse = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(EmrToolkitConfigCFG.TimeoutSeconds);

                    string json = JsonConvert.SerializeObject(body);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                    {
                        requestMessage.Content = content;
                        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (!string.IsNullOrEmpty(token))
                            requestMessage.Headers.TryAddWithoutValidation(HEADER_TOKEN_CODE, token);

                        HttpResponseMessage response;
                        try
                        {
                            response = client.SendAsync(requestMessage).Result;
                        }
                        catch (Exception ex)
                        {
                            // (vCong 62522) timeout / DNS / từ chối kết nối / SSL
                            throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, url);
                        }
                        rawResponse = response.Content != null ? response.Content.ReadAsStringAsync().Result : null;

                        if (!response.IsSuccessStatusCode)
                        {
                            LogSystem.Error(string.Format(
                                "POST {0} lỗi HTTP {1}. Response: {2}",
                                url, (int)response.StatusCode, rawResponse));
                            // (vCong 62522) 401/403 = tài khoản, 404 = địa chỉ, 5xx = phía EMR Toolkit/kết nối
                            SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, url, (int)response.StatusCode, response.ReasonPhrase, rawResponse), Partner, null));
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("PostJson lỗi. Url=" + url, ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, null)); // vCong 62522
                return false;
            }
        }
    }
}
