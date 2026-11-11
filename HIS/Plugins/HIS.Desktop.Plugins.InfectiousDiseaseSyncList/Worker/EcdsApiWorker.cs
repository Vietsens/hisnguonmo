/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * HTTP client tới cổng ECDS (kết nối TRỰC TIẾP từ client).
 * 4 bước: [1] Login -> [2] Danh mục -> [3] Đẩy ca bệnh -> [4] Đối soát.
 * KHÔNG dùng BackendAdapter (đó là backend nội bộ MOS).
 */
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker
{
    internal class EcdsApiWorker
    {
        private const string PATH_LOGIN = "/api/fast/v1/auth/login";
        private const string PATH_CASE_UPSERT = "/api/fast/v1/ca-benh/cap-nhat";
        private const string PATH_CASE_UPSERT_MANY = "/api/fast/v1/ca-benh/cap-nhat-nhieu";
        private const string PATH_DANHMUC = "/api/fast/v1/danh-muc/";

        /// <summary>Đối tác dùng dựng thông báo lỗi (vCong 62522)</summary>
        internal static Inventec.Common.IntegrationError.IntegrationPartner Partner
        {
            get { return Inventec.Common.IntegrationError.IntegrationPartner.HealthPortal("eCDS"); }
        }

        /// <summary>
        /// (vCong 62522) Lỗi đã phân loại nguồn (kết nối / cổng eCDS trả lỗi / thiết lập tại viện / lỗi code HIS) của thao tác gần nhất.
        /// Các hàm EnsureLogin/LayDanhMuc/DayCaBenh trả false/rỗng/null khi lỗi nên màn hình đọc thuộc tính này để báo lý do.
        /// </summary>
        internal string LastErrorMessage { get; private set; }

        private void SetError(string message)
        {
            if (string.IsNullOrWhiteSpace(LastErrorMessage)) LastErrorMessage = message;
        }

        /// <summary>Đường dẫn login lấy theo cấu hình (LoginPath), fallback mặc định.</summary>
        private static string LoginPath()
        {
            return !string.IsNullOrWhiteSpace(EcdsConfigCFG.LoginPath) ? EcdsConfigCFG.LoginPath : PATH_LOGIN;
        }

        /// <summary>Đường dẫn đẩy ca bệnh lấy theo cấu hình (PushPath), fallback mặc định.</summary>
        private static string PushPath()
        {
            return !string.IsNullOrWhiteSpace(EcdsConfigCFG.PushPath) ? EcdsConfigCFG.PushPath : PATH_CASE_UPSERT;
        }

        private HttpClient CreateClient()
        {
            // .NET 4.5 mặc định chưa bật TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // Kiểm tra BaseUrl là URL http/https hợp lệ TRƯỚC khi tạo client -> tránh UriFormatException
            // khó hiểu ("hostname could not be parsed") khi cấu hình sai; báo lỗi rõ, chỉ đúng key config.
            Uri baseUri;
            if (!Uri.TryCreate(EcdsConfigCFG.BaseUrl, UriKind.Absolute, out baseUri)
                || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            {
                // (vCong 62522) gắn nguồn lỗi THIẾT LẬP TẠI VIỆN
                throw new Inventec.Common.IntegrationError.IntegrationException(Inventec.Common.IntegrationError.ErrorSource.Config, Partner,
                    "ECDS BaseUrl không hợp lệ: '" + (EcdsConfigCFG.BaseUrl ?? "") + "'. Kiểm tra cấu hình "
                    + EcdsConfigCFG.CONFIG_KEY + " — phần BaseUrl phải là URL đầy đủ (VD https://daotao-gs.vadp.gov.vn).");
            }

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(EcdsConfigCFG.TimeoutSecond);
            client.BaseAddress = baseUri;
            return client;
        }

        /// <summary>[1] Đăng nhập lấy token; chỉ gọi khi token hết hạn.</summary>
        internal bool EnsureLogin()
        {
            LastErrorMessage = null;
            try
            {
                if (EcdsTokenStore.IsValid()) return true;
                if (!EcdsConfigCFG.IsValid())
                {
                    Inventec.Common.Logging.LogSystem.Warn("ECDS: thiếu cấu hình kết nối (BASE_URL/USERNAME/PASSWORD).");
                    SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.ConfigError(Partner, "Thiếu cấu hình kết nối cổng eCDS (BASE_URL/USERNAME/PASSWORD) trong cấu hình " + EcdsConfigCFG.CONFIG_KEY)); // vCong 62522
                    return false;
                }

                var body = new { username = EcdsConfigCFG.Username, password = EcdsConfigCFG.Password };
                var result = PostRaw<DangNhapResultDto>(LoginPath(), body, needAuth: false);
                if (result != null && result.thanhCong && result.duLieu != null
                    && !string.IsNullOrEmpty(result.duLieu.accessToken))
                {
                    EcdsTokenStore.Set(result.duLieu.accessToken, result.duLieu.expiresIn);
                    return true;
                }
                Inventec.Common.Logging.LogSystem.Error("ECDS login thất bại: "
                    + (result != null ? result.thongDiep : "response null"));
                // (vCong 62522) sai tài khoản → THIẾT LẬP TẠI VIỆN; không phản hồi → KẾT NỐI
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.LoginError(Partner, result != null ? JoinError(result.maLoi, result.thongDiep) : null));
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đăng nhập cổng eCDS")); // vCong 62522
                return false;
            }
        }

        /// <summary>[2] Lấy 1 danh mục ECDS.</summary>
        internal List<DanhMucItemDto> LayDanhMuc(string tenDanhMuc, SearchDanhMucFastDto filter)
        {
            try
            {
                if (!EnsureLogin()) return new List<DanhMucItemDto>();
                // duLieu là object phân trang { danhSach:[...], tongSo, trangSo... } -> lấy danhSach.
                var result = PostRaw<DanhMucPageDto>(PATH_DANHMUC + tenDanhMuc, filter ?? new SearchDanhMucFastDto());
                return (result != null && result.duLieu != null && result.duLieu.danhSach != null)
                    ? result.duLieu.danhSach : new List<DanhMucItemDto>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "lấy danh mục " + tenDanhMuc)); // vCong 62522
                return new List<DanhMucItemDto>();
            }
        }

        /// <summary>[3] Đẩy 1 ca bệnh (POST /ca-benh/cap-nhat). Có log request + response.</summary>
        internal KetQuaEcdsDto<CaBenhResultDto> DayCaBenh(EcdsDiseaseCaseDto dto)
        {
            try
            {
                if (!EnsureLogin()) return null;
                string url = PushPath();
                Inventec.Common.Logging.LogSystem.Info(
                    "ECDS DayCaBenh -> POST " + url + " ___req:" + JsonConvert.SerializeObject(dto));
                var result = PostRaw<CaBenhResultDto>(url, dto, needAuth: true, logRaw: true);
                Inventec.Common.Logging.LogSystem.Info("ECDS DayCaBenh <- " + DescribeResult(result));
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("ECDS DayCaBenh lỗi.", ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đẩy ca bệnh")); // vCong 62522
                return null;
            }
        }

        /// <summary>[3'] Đẩy nhiều ca bệnh 1 lần (POST /ca-benh/cap-nhat-nhieu — body là MẢNG THÔ DTO). Có log.</summary>
        internal KetQuaEcdsDto<DayNhieuKetQuaDto> DayNhieuCaBenh(List<EcdsDiseaseCaseDto> list)
        {
            try
            {
                if (!EnsureLogin()) return null;
                Inventec.Common.Logging.LogSystem.Info(
                    "ECDS DayNhieuCaBenh -> POST " + PATH_CASE_UPSERT_MANY
                    + " (" + (list != null ? list.Count : 0) + " ca) ___req:" + JsonConvert.SerializeObject(list));
                var result = PostRaw<DayNhieuKetQuaDto>(PATH_CASE_UPSERT_MANY, list, needAuth: true, logRaw: true);
                Inventec.Common.Logging.LogSystem.Info("ECDS DayNhieuCaBenh <- " + DescribeResult(result));
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("ECDS DayNhieuCaBenh lỗi.", ex);
                SetError(Inventec.Common.IntegrationError.IntegrationErrorMessage.FromException(ex, Partner, "đẩy danh sách ca bệnh")); // vCong 62522
                return null;
            }
        }

        /// <summary>Ghép mã lỗi + thông điệp cổng trả về (vCong 62522)</summary>
        internal static string JoinError(string maLoi, string thongDiep)
        {
            if (string.IsNullOrWhiteSpace(maLoi)) return thongDiep;
            return "Mã lỗi " + maLoi + (string.IsNullOrWhiteSpace(thongDiep) ? "" : ". " + thongDiep);
        }

        /// <summary>Thông báo khi cổng trả kết quả không thành công / không có kết quả (vCong 62522)</summary>
        internal string BuildFailMessage<T>(KetQuaEcdsDto<T> result, string fallbackSummary)
        {
            if (result == null)
                return !string.IsNullOrWhiteSpace(LastErrorMessage) ? LastErrorMessage : Inventec.Common.IntegrationError.IntegrationErrorMessage.NoResponse(Partner, fallbackSummary);
            return Inventec.Common.IntegrationError.IntegrationErrorMessage.PartnerError(Partner, JoinError(result.maLoi, result.thongDiep), "Cổng eCDS từ chối " + fallbackSummary);
        }

        /// <summary>Tóm tắt kết quả trả về cổng để ghi log.</summary>
        private static string DescribeResult<T>(KetQuaEcdsDto<T> r)
        {
            if (r == null) return "null response (không có phản hồi từ cổng)";
            return "thanhCong=" + r.thanhCong + " maLoi=" + r.maLoi + " thongDiep=" + r.thongDiep;
        }

        // ---- Core: POST JSON + Bearer, deserialize KetQuaEcdsDto<T> ----
        private KetQuaEcdsDto<T> PostRaw<T>(string path, object body, bool needAuth = true, bool logRaw = false)
        {
            using (var client = CreateClient())
            {
                if (needAuth && EcdsTokenStore.IsValid())
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", EcdsTokenStore.AccessToken);

                string json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Chạy trên threadpool (Task.Run) để tránh deadlock khi gọi từ UI thread.
                string uri = (client.BaseAddress != null ? client.BaseAddress.ToString().TrimEnd('/') : "") + path;
                HttpResponseMessage resp;
                try
                {
                    resp = System.Threading.Tasks.Task.Run(() => client.PostAsync(path, content))
                        .GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // (vCong 62522) timeout / DNS / từ chối kết nối / SSL → gắn nguồn lỗi KẾT NỐI
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.WrapCall(ex, Partner, uri);
                }
                if (resp == null) throw Inventec.Common.IntegrationError.IntegrationHttpError.NoResponse(Partner, "gọi " + uri);
                string respStr = System.Threading.Tasks.Task.Run(() => resp.Content.ReadAsStringAsync())
                    .GetAwaiter().GetResult();

                if (logRaw)
                    Inventec.Common.Logging.LogSystem.Info(
                        "ECDS RESP " + path + " (HTTP " + (int)resp.StatusCode + ") ___resp:" + respStr);

                // (vCong 62522) cổng có thể trả mã HTTP lỗi kèm JSON {thanhCong,maLoi,thongDiep}: vẫn đọc như cũ;
                // không đọc được thì báo theo mã HTTP (401/403 = tài khoản, 404 = địa chỉ, 5xx = cổng/kết nối)
                if (string.IsNullOrEmpty(respStr))
                {
                    if (!resp.IsSuccessStatusCode)
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, uri, (int)resp.StatusCode, resp.ReasonPhrase, respStr);
                    return null;
                }
                KetQuaEcdsDto<T> parsed;
                try
                {
                    parsed = JsonConvert.DeserializeObject<KetQuaEcdsDto<T>>(respStr);
                }
                catch (Exception ex)
                {
                    if (!resp.IsSuccessStatusCode)
                        throw Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, uri, (int)resp.StatusCode, resp.ReasonPhrase, respStr);
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.ParseError(Partner, uri, respStr, ex);
                }
                if (parsed == null && !resp.IsSuccessStatusCode)
                    throw Inventec.Common.IntegrationError.IntegrationHttpError.FromStatus(Partner, uri, (int)resp.StatusCode, resp.ReasonPhrase, respStr);
                return parsed;
            }
        }
    }

    /// <summary>Dữ liệu trả về khi đẩy 1 ca bệnh (duLieu của /ca-benh/cap-nhat).</summary>
    public class CaBenhResultDto
    {
        public string maCaBenh { get; set; }
        public string id { get; set; }
    }

    /// <summary>duLieu của /ca-benh/cap-nhat-nhieu — tổng hợp + chi tiết từng ca theo chỉ số.</summary>
    public class DayNhieuKetQuaDto
    {
        public int? tongSo { get; set; }
        public int? soThanhCong { get; set; }
        public int? soLoi { get; set; }
        public List<ChiTietCaBenhDto> chiTiet { get; set; }
    }

    /// <summary>1 phần tử chiTiet — kết quả đẩy của ca ở vị trí chiSo trong mảng gửi lên.</summary>
    public class ChiTietCaBenhDto
    {
        public int chiSo { get; set; }
        public bool thanhCong { get; set; }
        public string idCaBenh { get; set; }
    }

    /// <summary>duLieu của API danh mục — object phân trang, danh sách nằm trong danhSach.</summary>
    public class DanhMucPageDto
    {
        public List<DanhMucItemDto> danhSach { get; set; }
        public int? tongSo { get; set; }
        public int? trangSo { get; set; }
        public int? kichThuocTrang { get; set; }
        public int? tongSoTrang { get; set; }
    }
}
