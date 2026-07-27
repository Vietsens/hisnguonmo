/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Cache accessToken theo phiên máy (RAM). Login lại chỉ khi hết hạn.
 * ⚠ KHÔNG lưu token xuống đĩa, KHÔNG log token.
 */
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker
{
    internal static class EcdsTokenStore
    {
        private static string _accessToken;
        private static DateTime _expireAt = DateTime.MinValue;

        /// <summary>Token còn hiệu lực (trừ hao 60s).</summary>
        internal static bool IsValid()
        {
            return !string.IsNullOrEmpty(_accessToken) && DateTime.Now < _expireAt.AddSeconds(-60);
        }

        internal static string AccessToken
        {
            get { return _accessToken; }
        }

        internal static void Set(string token, long expiresInSecond)
        {
            _accessToken = token;
            _expireAt = DateTime.Now.AddSeconds(expiresInSecond > 0 ? expiresInSecond : 3600);
        }

        internal static void Clear()
        {
            _accessToken = null;
            _expireAt = DateTime.MinValue;
        }
    }
}
