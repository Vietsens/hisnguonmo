using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.BankHub.Helper
{
    /// <summary>
    /// Helper tạo PKCE (Proof Key for Code Exchange) cho OAuth2 Authorization Code Flow.
    /// code_challenge = BASE64URL(SHA256(ASCII(code_verifier)))
    /// </summary>
    public static class PkceHelper
    {
        /// <summary>
        /// Tạo code_verifier ngẫu nhiên (32 bytes, Base64Url encoded, không padding).
        /// Lưu giá trị này lại để dùng ở Bước 3 (exchange token).
        /// </summary>
        public static string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes);
        }

        /// <summary>
        /// Tạo code_challenge từ code_verifier.
        /// code_challenge = BASE64URL(SHA256(ASCII(code_verifier)))
        /// </summary>
        public static string GenerateCodeChallenge(string codeVerifier)
        {
            if (string.IsNullOrEmpty(codeVerifier))
                throw new ArgumentNullException("codeVerifier");

            byte[] bytes = Encoding.ASCII.GetBytes(codeVerifier);
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                return Base64UrlEncode(hash);
            }
        }

        /// <summary>Tạo state ngẫu nhiên để chống CSRF</summary>
        public static string GenerateState()
        {
            var bytes = new byte[24];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Parse query string từ URL, trả về dictionary key=value.
        /// VD: https://redirect.uri?code=abc&state=xyz
        /// </summary>
        public static System.Collections.Generic.Dictionary<string, string> ParseQueryString(string url)
        {
            var result = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(url)) return result;

            int qIndex = url.IndexOf('?');
            if (qIndex < 0) return result;

            string query = url.Substring(qIndex + 1);
            // Xử lý fragment (#) nếu có
            int hashIndex = query.IndexOf('#');
            if (hashIndex >= 0) query = query.Substring(0, hashIndex);

            foreach (string part in query.Split('&'))
            {
                int eqIndex = part.IndexOf('=');
                if (eqIndex > 0)
                {
                    string key = Uri.UnescapeDataString(part.Substring(0, eqIndex));
                    string val = Uri.UnescapeDataString(part.Substring(eqIndex + 1));
                    result[key] = val;
                }
            }
            return result;
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
