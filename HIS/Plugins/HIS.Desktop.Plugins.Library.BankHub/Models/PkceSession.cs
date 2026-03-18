using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Models
{
    /// <summary>
    /// Thông tin phiên PKCE đang chờ người dùng đăng nhập.
    /// Lưu trữ tạm thời để dùng ở Bước 3 (exchange code → token).
    /// </summary>
    public class PkceSession
    {
        /// <summary>code_verifier - BẮT BUỘC lưu lại để exchange token</summary>
        public string CodeVerifier { get; set; }

        /// <summary>code_challenge - gửi lên URL auth</summary>
        public string CodeChallenge { get; set; }

        /// <summary>state - chống CSRF, phải khớp khi nhận redirect</summary>
        public string State { get; set; }

        /// <summary>URL đăng nhập đầy đủ để load vào WebView</summary>
        public string LoginUrl { get; set; }
    }
}