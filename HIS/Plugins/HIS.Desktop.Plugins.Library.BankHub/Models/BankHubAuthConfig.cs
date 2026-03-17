using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Models
{
    /// <summary>
    /// Cấu hình cho luồng OAuth2 PKCE của BankHub Keycloak.
    /// </summary>
    public class BankHubAuthConfig
    {
        //// Sandbox Keycloak endpoints
        //private const string SANDBOX_AUTH_URL =
        //    "https://api-sandbox.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/auth";
        //private const string SANDBOX_TOKEN_URL =
        //    "https://api-sandbox.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/token";
        //private const string SANDBOX_LOGOUT_URL =
        //    "https://api-sandbox.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/logout";

        //// Production Keycloak endpoints (cập nhật khi go-live)
        //private const string PROD_AUTH_URL =
        //    "https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/auth";
        //private const string PROD_TOKEN_URL =
        //    "https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/token";
        //private const string PROD_LOGOUT_URL =
        //    "https://api.mbbank.com.vn/biz-kc/realms/bank-hub/protocol/openid-connect/logout";

        /// <summary>URL Keycloak Authorization (hiển thị trang đăng nhập)</summary>
        public string AuthUrl { get; set; }

        /// <summary>URL lấy Access Token</summary>
        public string TokenUrl { get; set; }

        /// <summary>URL Logout</summary>
        public string LogoutUrl { get; set; }

        /// <summary>Client ID do MB cấp cho đối tác (VD: "fast", "sse")</summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Redirect URI của đối tác (phải được cấu hình trong Keycloak).
        /// Sau khi đăng nhập thành công, Keycloak redirect về URL này kèm ?code=...
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>Scope, mặc định "openid"</summary>
        public string Scope { get; set; }

        /// <summary>Timeout HTTP (giây), mặc định 30</summary>
        public int TimeoutSeconds { get; set; }

        ///// <summary>Tạo cấu hình Sandbox</summary>
        //public static BankHubAuthConfig Sandbox(string clientId, string redirectUri)
        //{
        //    return new BankHubAuthConfig
        //    {
        //        AuthUrl = SANDBOX_AUTH_URL,
        //        TokenUrl = SANDBOX_TOKEN_URL,
        //        LogoutUrl = SANDBOX_LOGOUT_URL,
        //        ClientId = clientId,
        //        RedirectUri = redirectUri,
        //        Scope = "openid",
        //        TimeoutSeconds = 30
        //    };
        //}

        /// <summary>Tạo cấu hình Production</summary>
        public static BankHubAuthConfig Production(string authUrl, string tokenUrl, string logoutUrl, string clientId, string redirectUri)
        {
            return new BankHubAuthConfig
            {
                AuthUrl = authUrl,
                TokenUrl = tokenUrl,
                LogoutUrl = logoutUrl,
                ClientId = clientId,
                RedirectUri = redirectUri,
                Scope = "openid",
                TimeoutSeconds = 30
            };
        }
    }

}
