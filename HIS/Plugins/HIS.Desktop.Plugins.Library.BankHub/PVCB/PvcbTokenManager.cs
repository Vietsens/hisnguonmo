using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.PVCB
{/// <summary>
 /// Quản lý access_token cho dịch vụ Chi hộ trong nước (PVcomBank).
 /// API: POST {base_url_get_token} với grant_type=client_credentials (OAuth2).
 ///
 /// Đặc điểm:
 ///  - Tự động đăng nhập, cache token và chủ động refresh TRƯỚC khi hết hạn.
 ///  - Thread-safe: dùng SemaphoreSlim + double-checked để nhiều luồng gọi
 ///    đồng thời chỉ phát sinh đúng 1 lần lấy token.
 ///  - Dùng chung 1 HttpClient static (tránh socket exhaustion / latency do
 ///    new HttpClient() mỗi lần gọi).
 ///
 /// Khuyến nghị: khởi tạo 1 instance duy nhất (singleton) dùng chung toàn ứng dụng.
 /// </summary>
    public sealed class PvcbTokenManager
    {
        // ----- HttpClient dùng chung -----
        private static readonly HttpClient _http;

        static PvcbTokenManager()
        {
            // .NET Framework 4.5 mặc định dùng TLS 1.0 -> bắt buộc bật TLS 1.2,
            // nếu không sẽ lỗi handshake khi gọi https://apis-uat.pvcombank.io
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // Tắt proxy auto-detect để kết nối thẳng đến PVCB.
            // Không có cấu hình này, HttpClient sẽ đọc IE proxy settings → có thể hang/timeout
            // nếu hệ thống có proxy auto-config sai (PAC script không tới được, ...).
            var handler = new HttpClientHandler
            {
                UseProxy = false,
                Proxy = null
            };
            _http = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // ----- Cấu hình -----
        private readonly string _tokenUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;

        // Refresh sớm hơn thời điểm hết hạn 1 khoảng đệm để tránh dùng token sắp die.
        private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(60);

        // ----- Trạng thái cache -----
        private string _accessToken;
        private string _tokenType;
        private DateTime _expiresAtUtc = DateTime.MinValue;
        private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

        /// <param name="tokenUrl">VD UAT: https://apis-uat.pvcombank.io/idp/oauth2/token</param>
        /// <param name="clientId">client_id do PVCB cung cấp</param>
        /// <param name="clientSecret">client_secret do PVCB cung cấp</param>
        public PvcbTokenManager(string tokenUrl, string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(tokenUrl)) throw new ArgumentNullException("tokenUrl");
            if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentNullException("clientId");
            if (string.IsNullOrWhiteSpace(clientSecret)) throw new ArgumentNullException("clientSecret");

            _tokenUrl = tokenUrl;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        /// <summary>
        /// Lấy access_token hợp lệ. Tự động đăng nhập nếu chưa có hoặc đã/sắp hết hạn.
        /// Đây là entry point chính nên gọi trước mỗi lần call API dịch vụ.
        /// </summary>
        public async Task<string> GetAccessTokenAsync(CancellationToken ct = default(CancellationToken))
        {
            if (IsValid()) return _accessToken;

            await _refreshLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // Double-check: luồng khác có thể đã refresh trong lúc mình chờ lock
                if (IsValid()) return _accessToken;

                await RefreshAsync(ct).ConfigureAwait(false);
                return _accessToken;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        /// <summary>
        /// Trả về header "Bearer {access_token}" để gán thẳng vào Authorization.
        /// </summary>
        public async Task<string> GetAuthorizationHeaderAsync(CancellationToken ct = default(CancellationToken))
        {
            var token = await GetAccessTokenAsync(ct).ConfigureAwait(false);
            var type = string.IsNullOrEmpty(_tokenType) ? "Bearer" : ToTitleCase(_tokenType);
            return type + " " + token;
        }

        /// <summary>
        /// Bản đồng bộ (sync) cho code chưa dùng async/await.
        /// Dùng Task.Run để tránh deadlock sync-over-async trên IIS/ASP.NET (SynchronizationContext).
        /// Nếu có thể, ưu tiên dùng GetAccessTokenAsync().
        /// </summary>
        public string GetAccessToken()
        {
            return Task.Run(() => GetAccessTokenAsync()).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Xóa token đang cache. Gọi khi API dịch vụ trả về 401 (token invalid/hết hạn)
        /// để lần gọi kế tiếp tự đăng nhập lại.
        /// </summary>
        public void Invalidate()
        {
            _accessToken = null;
            _expiresAtUtc = DateTime.MinValue;
        }

        public DateTime GetExpires()
        {
            return _expiresAtUtc;
        }

        // ===================== Internal =====================

        private bool IsValid()
        {
            return !string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAtUtc;
        }

        private async Task RefreshAsync(CancellationToken ct)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "openid"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
            });

            using (var req = new HttpRequestMessage(HttpMethod.Post, _tokenUrl) { Content = form })
            using (var resp = await _http.SendAsync(req, ct).ConfigureAwait(false))
            {
                var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                    throw new PvcbAuthException((int)resp.StatusCode,
                        "Lấy access_token thất bại. Body: " + body);

                TokenResponse token;
                try
                {
                    token = JsonConvert.DeserializeObject<TokenResponse>(body);
                }
                catch (Exception ex)
                {
                    throw new PvcbAuthException((int)resp.StatusCode,
                        "Không parse được response token. Body: " + body, ex);
                }

                if (token == null || string.IsNullOrEmpty(token.AccessToken))
                    throw new PvcbAuthException((int)resp.StatusCode,
                        "Response không chứa access_token. Body: " + body);

                _accessToken = token.AccessToken;
                _tokenType = token.TokenType;

                // expires_in tính bằng giây (tài liệu = 1800). Trừ đi RefreshBuffer
                // để chủ động refresh sớm. Đảm bảo không âm.
                var lifetime = TimeSpan.FromSeconds(Math.Max(token.ExpiresIn, 0));
                var effective = lifetime > RefreshBuffer ? lifetime - RefreshBuffer : lifetime;
                _expiresAtUtc = DateTime.UtcNow + effective;
            }
        }

        private static string ToTitleCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }

        // ===================== Model & Exception =====================

        private sealed class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }

    /// <summary>Lỗi khi lấy/làm mới access_token.</summary>
    public sealed class PvcbAuthException : Exception
    {
        public int HttpStatus { get; private set; }

        public PvcbAuthException(int httpStatus, string message)
            : base(message)
        {
            HttpStatus = httpStatus;
        }

        public PvcbAuthException(int httpStatus, string message, Exception inner)
            : base(message, inner)
        {
            HttpStatus = httpStatus;
        }
    }
}
