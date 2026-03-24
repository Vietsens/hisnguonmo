using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Models
{
    /// <summary>
    /// Token response từ Keycloak (lấy token và refresh token).
    /// </summary>
    public class TokenResponse
    {
        /// <summary>Access token dùng để gọi API</summary>
        public string access_token { get; set; }

        /// <summary>Thời hạn access token (giây). Mặc định 900 = 15 phút</summary>
        public int expires_in { get; set; }

        /// <summary>Thời hạn refresh token (giây). Mặc định 1800 = 30 phút</summary>
        public int refresh_expires_in { get; set; }

        /// <summary>Refresh token dùng để lấy access token mới khi hết hạn</summary>
        public string refresh_token { get; set; }

        /// <summary>Loại token (Bearer)</summary>
        public string token_type { get; set; }

        /// <summary>ID token (OpenID Connect)</summary>
        public string id_token { get; set; }

        /// <summary>Session state</summary>
        public string session_state { get; set; }

        /// <summary>Scope được cấp</summary>
        public string scope { get; set; }

        // ---- Computed helpers ----

        /// <summary>Thời điểm token hết hạn (tính từ lúc nhận được)</summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>Thời điểm refresh token hết hạn</summary>
        public DateTime RefreshExpiresAt { get; set; }

        /// <summary>Kiểm tra access token còn hiệu lực không (có buffer 30 giây)</summary>
        public bool IsAccessTokenValid()
        {
            return !string.IsNullOrEmpty(access_token)
                && DateTime.UtcNow < ExpiresAt.AddSeconds(-30);
        }

        /// <summary>Kiểm tra refresh token còn hiệu lực không</summary>
        public bool IsRefreshTokenValid()
        {
            return !string.IsNullOrEmpty(refresh_token)
                && DateTime.UtcNow < RefreshExpiresAt.AddSeconds(-30);
        }
    }

}
