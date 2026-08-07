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
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(EcdsConfigCFG.TimeoutSecond);
            if (!string.IsNullOrEmpty(EcdsConfigCFG.BaseUrl))
                client.BaseAddress = new Uri(EcdsConfigCFG.BaseUrl);
            return client;
        }

        /// <summary>[1] Đăng nhập lấy token; chỉ gọi khi token hết hạn.</summary>
        internal bool EnsureLogin()
        {
            try
            {
                if (EcdsTokenStore.IsValid()) return true;
                if (!EcdsConfigCFG.IsValid())
                {
                    Inventec.Common.Logging.LogSystem.Warn("ECDS: thiếu cấu hình kết nối (BASE_URL/USERNAME/PASSWORD).");
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
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                return new List<DanhMucItemDto>();
            }
        }

        /// <summary>[3] Đẩy 1 ca bệnh.</summary>
        internal KetQuaEcdsDto<CaBenhResultDto> DayCaBenh(EcdsDiseaseCaseDto dto)
        {
            try
            {
                if (!EnsureLogin()) return null;
                return PostRaw<CaBenhResultDto>(PushPath(), dto);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>[3'] Đẩy nhiều ca bệnh (batch).</summary>
        internal KetQuaEcdsDto<object> DayNhieuCaBenh(List<EcdsDiseaseCaseDto> list)
        {
            try
            {
                if (!EnsureLogin()) return null;
                return PostRaw<object>(PATH_CASE_UPSERT_MANY, list);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        // ---- Core: POST JSON + Bearer, deserialize KetQuaEcdsDto<T> ----
        private KetQuaEcdsDto<T> PostRaw<T>(string path, object body, bool needAuth = true)
        {
            using (var client = CreateClient())
            {
                if (needAuth && EcdsTokenStore.IsValid())
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", EcdsTokenStore.AccessToken);

                string json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Chạy trên threadpool (Task.Run) để tránh deadlock khi gọi từ UI thread.
                var resp = System.Threading.Tasks.Task.Run(() => client.PostAsync(path, content))
                    .GetAwaiter().GetResult();
                string respStr = System.Threading.Tasks.Task.Run(() => resp.Content.ReadAsStringAsync())
                    .GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(respStr)) return null;
                return JsonConvert.DeserializeObject<KetQuaEcdsDto<T>>(respStr);
            }
        }
    }

    /// <summary>Dữ liệu trả về khi đẩy ca bệnh (mã ca bệnh cổng).</summary>
    public class CaBenhResultDto
    {
        public string maCaBenh { get; set; }
        public string id { get; set; }
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
