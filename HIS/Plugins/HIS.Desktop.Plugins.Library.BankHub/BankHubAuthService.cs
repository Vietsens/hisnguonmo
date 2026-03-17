using HIS.Desktop.Plugins.Library.BankHub.Helper;
using HIS.Desktop.Plugins.Library.BankHub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub
{
    /// <summary>
    /// Service xử lý toàn bộ luồng xác thực OAuth2 PKCE với BankHub Keycloak.
    ///
    /// Luồng hoạt động:
    ///   Bước 1: Gọi <see cref="CreatePkceSession"/> → nhận LoginUrl
    ///   Bước 2: Load LoginUrl vào WebView, bắt sự kiện BeforeNavigate
    ///           Khi URL chứa redirect_uri + ?code= → gọi <see cref="ExchangeCodeForToken"/>
    ///   Bước 3: Lưu <see cref="TokenResponse"/>, dùng access_token cho BankHubApiClient
    ///   Bước 4: Khi token sắp hết hạn → gọi <see cref="RefreshAccessToken"/>
    ///   Bước 5: Khi kết thúc phiên → gọi <see cref="Logout"/>
    /// </summary>
    public class BankHubAuthService
    {
        private readonly BankHubAuthConfig _config;

        /// <summary>Token hiện tại (null nếu chưa đăng nhập)</summary>
        public TokenResponse CurrentToken { get; private set; }

        public BankHubAuthService(BankHubAuthConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (string.IsNullOrEmpty(config.ClientId))
                throw new ArgumentException("ClientId không được rỗng");
            if (string.IsNullOrEmpty(config.RedirectUri))
                throw new ArgumentException("RedirectUri không được rỗng");

            _config = config;
        }

        // =============================================
        // BƯỚC 1: TẠO PKCE SESSION + LOGIN URL
        // =============================================

        /// <summary>
        /// Tạo phiên PKCE mới: sinh code_verifier, code_challenge, state.
        /// Trả về <see cref="PkceSession"/> chứa <see cref="PkceSession.LoginUrl"/>
        /// để load vào WebView.
        ///
        /// <para><b>QUAN TRỌNG:</b> Lưu PkceSession lại (dùng ở Bước 3).</para>
        /// </summary>
        public PkceSession CreatePkceSession()
        {
            string codeVerifier = PkceHelper.GenerateCodeVerifier();
            string codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
            string state = PkceHelper.GenerateState();

            string loginUrl = BuildLoginUrl(codeChallenge, state);

            return new PkceSession
            {
                CodeVerifier = codeVerifier,
                CodeChallenge = codeChallenge,
                State = state,
                LoginUrl = loginUrl
            };
        }

        private string BuildLoginUrl(string codeChallenge, string state)
        {
            var sb = new StringBuilder();
            sb.Append(_config.AuthUrl);
            sb.Append("?client_id=").Append(Uri.EscapeDataString(_config.ClientId));
            sb.Append("&response_type=code");
            sb.Append("&scope=").Append(Uri.EscapeDataString(_config.Scope ?? "openid"));
            sb.Append("&redirect_uri=").Append(_config.RedirectUri);
            sb.Append("&state=").Append(Uri.EscapeDataString(state));
            sb.Append("&code_challenge=").Append(Uri.EscapeDataString(codeChallenge));
            sb.Append("&code_challenge_method=S256");
            return sb.ToString();
        }

        // =============================================
        // BƯỚC 2: XỬ LÝ REDIRECT TỪ WEBVIEW
        // =============================================

        /// <summary>
        /// Kiểm tra URL từ sự kiện BeforeNavigate của WebView có phải là redirect callback không.
        /// Nếu đúng, trả về authorization code. Nếu không, trả về null.
        ///
        /// <para>Dùng trong sự kiện <b>EO.WebBrowser.WebView.BeforeNavigate</b>:</para>
        /// <code>
        /// void webView_BeforeNavigate(object sender, BeforeNavigateEventArgs e) {
        ///     string code = authService.TryGetAuthorizationCode(e.Url, pkceSession, out string error);
        ///     if (code != null) {
        ///         e.Handled = true;
        ///         var token = authService.ExchangeCodeForToken(code, pkceSession);
        ///     }
        /// }
        /// </code>
        /// </summary>
        /// <param name="navigatedUrl">URL từ sự kiện BeforeNavigate (e.Url)</param>
        /// <param name="session">PkceSession tạo ở Bước 1</param>
        /// <param name="error">Nội dung lỗi nếu có (VD: access_denied)</param>
        /// <returns>Authorization code nếu thành công, null nếu chưa phải redirect hoặc có lỗi</returns>
        public string TryGetAuthorizationCode(string navigatedUrl, PkceSession session, out string error)
        {
            error = null;

            if (string.IsNullOrEmpty(navigatedUrl)) return null;

            // Chỉ xử lý khi URL bắt đầu bằng redirect_uri
            if (!navigatedUrl.StartsWith(_config.RedirectUri, StringComparison.OrdinalIgnoreCase))
                return null;

            var queryParams = PkceHelper.ParseQueryString(navigatedUrl);

            // Kiểm tra lỗi từ Keycloak (VD: access_denied, login_required)
            string errorCode;
            if (queryParams.TryGetValue("error", out errorCode))
            {
                string errorDesc;
                queryParams.TryGetValue("error_description", out errorDesc);
                error = string.Format("{0}: {1}", errorCode, errorDesc ?? "(no description)");
                return null;
            }

            // Kiểm tra code có tồn tại không
            string code;
            if (!queryParams.TryGetValue("code", out code) || string.IsNullOrEmpty(code))
                return null;

            // Kiểm tra state khớp (chống CSRF)
            if (session != null && !string.IsNullOrEmpty(session.State))
            {
                string returnedState;
                queryParams.TryGetValue("state", out returnedState);
                if (returnedState != session.State)
                {
                    error = "State không khớp - có thể bị tấn công CSRF";
                    return null;
                }
            }

            return code;
        }

        // =============================================
        // BƯỚC 3: ĐỔI CODE → ACCESS TOKEN
        // =============================================

        /// <summary>
        /// Đổi Authorization Code lấy Access Token và Refresh Token.
        /// Gọi sau khi bắt được code từ sự kiện BeforeNavigate.
        /// </summary>
        /// <param name="authorizationCode">Code lấy từ <see cref="TryGetAuthorizationCode"/></param>
        /// <param name="session">PkceSession tạo ở Bước 1 (cần CodeVerifier)</param>
        /// <returns>TokenResponse chứa access_token, refresh_token, expires_in...</returns>
        /// <exception cref="BankHubException">Khi lỗi HTTP hoặc Keycloak trả lỗi</exception>
        public TokenResponse ExchangeCodeForToken(string authorizationCode, PkceSession session)
        {
            if (string.IsNullOrEmpty(authorizationCode))
                throw new ArgumentNullException("authorizationCode");
            if (session == null)
                throw new ArgumentNullException("session");
            if (string.IsNullOrEmpty(session.CodeVerifier))
                throw new ArgumentException("CodeVerifier trong PkceSession không được rỗng");

            var formData = new Dictionary<string, string>
            {
                { "grant_type",    "authorization_code" },
                { "client_id",     _config.ClientId },
                { "code",          authorizationCode },
                { "redirect_uri",  _config.RedirectUri },
                { "code_verifier", session.CodeVerifier }
            };

            string responseJson = PostFormUrlEncoded(_config.TokenUrl, formData, accessToken: null);
            var token = ParseTokenResponse(responseJson);
            CurrentToken = token;
            return token;
        }

        // =============================================
        // BƯỚC 4: LÀM MỚI TOKEN BẰNG REFRESH TOKEN
        // =============================================

        /// <summary>
        /// Lấy access token mới bằng refresh token (không cần đăng nhập lại).
        /// Nên gọi khi <see cref="TokenResponse.IsAccessTokenValid"/> trả về false
        /// nhưng <see cref="TokenResponse.IsRefreshTokenValid"/> vẫn còn true.
        /// </summary>
        /// <param name="refreshToken">
        /// Refresh token từ TokenResponse trước. Nếu null thì dùng CurrentToken.
        /// </param>
        public TokenResponse RefreshAccessToken(string refreshToken = null)
        {
            string token = refreshToken ?? (CurrentToken != null ? CurrentToken.refresh_token : null);
            if (string.IsNullOrEmpty(token))
                throw new BankHubException("Không có refresh token. Cần đăng nhập lại.");

            var formData = new Dictionary<string, string>
            {
                { "grant_type",    "refresh_token" },
                { "client_id",     _config.ClientId },
                { "refresh_token", token }
            };

            string responseJson = PostFormUrlEncoded(_config.TokenUrl, formData, accessToken: null);
            var newToken = ParseTokenResponse(responseJson);
            CurrentToken = newToken;
            return newToken;
        }

        /// <summary>
        /// Tự động làm mới token nếu cần. Trả về access_token hiện tại (hợp lệ).
        /// Ném exception nếu cả access token và refresh token đều hết hạn.
        /// </summary>
        public string GetValidAccessToken()
        {
            if (CurrentToken == null)
                throw new BankHubException("Chưa đăng nhập. Gọi ExchangeCodeForToken trước.");

            if (CurrentToken.IsAccessTokenValid())
                return CurrentToken.access_token;

            if (!CurrentToken.IsRefreshTokenValid())
                throw new BankHubException("Cả access token và refresh token đều hết hạn. Cần đăng nhập lại.");

            var refreshed = RefreshAccessToken();
            return refreshed.access_token;
        }

        // =============================================
        // BƯỚC 5: LOGOUT
        // =============================================

        /// <summary>
        /// Đăng xuất khỏi BankHub Keycloak.
        /// Keycloak sẽ trả về HTTP 204 No Content nếu thành công.
        /// </summary>
        /// <param name="accessToken">Access token (null → dùng CurrentToken)</param>
        /// <param name="refreshToken">Refresh token (null → dùng CurrentToken)</param>
        /// <returns>true nếu logout thành công (HTTP 204), false nếu thất bại</returns>
        public bool Logout(string accessToken = null, string refreshToken = null)
        {
            string at = accessToken ?? (CurrentToken != null ? CurrentToken.access_token : null);
            string rt = refreshToken ?? (CurrentToken != null ? CurrentToken.refresh_token : null);

            if (string.IsNullOrEmpty(at) || string.IsNullOrEmpty(rt))
                throw new BankHubException("Cần access_token và refresh_token để logout.");

            var formData = new Dictionary<string, string>
            {
                { "client_id",     _config.ClientId },
                { "refresh_token", rt }
            };

            try
            {
                PostFormUrlEncoded(_config.LogoutUrl, formData, accessToken: at, expectNoContent: true);
                CurrentToken = null;
                return true;
            }
            catch (BankHubException ex)
            {
                // HTTP 204 = No Content = thành công (không có body)
                if (ex.Message.Contains("204"))
                {
                    CurrentToken = null;
                    return true;
                }
                return false;
            }
        }

        // =============================================
        // PRIVATE HELPERS
        // =============================================

        private string PostFormUrlEncoded(
            string url,
            Dictionary<string, string> formData,
            string accessToken,
            bool expectNoContent = false)
        {
            var sb = new StringBuilder();
            foreach (var kv in formData)
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value));
            }

            byte[] bodyBytes = Encoding.UTF8.GetBytes(sb.ToString());

            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bodyBytes.Length;
            request.Timeout = _config.TimeoutSeconds * 1000;

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("Authorization", "Bearer " + accessToken);
            }

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bodyBytes, 0, bodyBytes.Length);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (expectNoContent || response.StatusCode == HttpStatusCode.NoContent)
                        return string.Empty;

                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var httpResp = (HttpWebResponse)ex.Response;
                    if (httpResp.StatusCode == HttpStatusCode.NoContent) return string.Empty;

                    string body;
                    using (var reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8))
                    {
                        body = reader.ReadToEnd();
                    }
                    throw new BankHubException(
                        string.Format("Keycloak lỗi HTTP {0}: {1}", (int)httpResp.StatusCode, body), ex);
                }
                throw new BankHubException("Lỗi kết nối Keycloak: " + ex.Message, ex);
            }
        }

        private TokenResponse ParseTokenResponse(string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new BankHubException("Keycloak trả về response rỗng");

            // Kiểm tra lỗi từ Keycloak ({"error":"...", "error_description":"..."})
            if (json.Contains("\"error\""))
            {
                var errDict = JsonHelper.DeserializeToDictionary(json);
                object errVal, errDesc;
                errDict.TryGetValue("error", out errVal);
                errDict.TryGetValue("error_description", out errDesc);
                throw new BankHubException(
                    string.Format("Keycloak lỗi: {0} - {1}", errVal, errDesc));
            }

            var token = JsonHelper.Deserialize<TokenResponse>(json);
            if (token == null)
                throw new BankHubException("Không parse được TokenResponse từ Keycloak");

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => token), token));
            // Tính thời điểm hết hạn
            var now = DateTime.Now;
            token.ExpiresAt = now.AddSeconds(token.expires_in);
            token.RefreshExpiresAt = now.AddSeconds(token.refresh_expires_in);

            return token;
        }
    }
}
