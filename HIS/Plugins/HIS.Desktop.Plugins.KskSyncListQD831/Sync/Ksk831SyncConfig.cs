/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync
{
    /// <summary>
    /// Cấu hình liên thông HSSK QĐ831 (HIS_CONFIG key MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO).
    /// Định dạng: &lt;tài khoản&gt;|&lt;mật khẩu&gt;|&lt;địa chỉ gốc&gt;|&lt;api-login&gt;|&lt;api-push&gt;.
    /// </summary>
    internal class Ksk831SyncConfig
    {
        internal string Username { get; private set; }
        internal string Password { get; private set; }
        internal string BaseUrl { get; private set; }
        internal string LoginApi { get; private set; }
        internal string PushApi { get; private set; }

        internal string LoginUrl { get { return Combine(BaseUrl, LoginApi); } }
        internal string PushUrl { get { return Combine(BaseUrl, PushApi); } }

        /// <summary>
        /// TEMP (fake để test): cấu hình mặc định theo tài liệu HSSK 831 (base Development) + tài khoản test.
        /// TODO: XÓA khi đã khai báo HIS_CONFIG MOS.HIS_KSK_SYNC.HSSK_AREA_831_CONNECTION_INFO thật.
        /// </summary>
        internal static Ksk831SyncConfig TempDefault()
        {
            return new Ksk831SyncConfig
            {
                Username = "86137_his",
                Password = "Hoc@2026",
                BaseUrl = "https://dev-api-csdl.kdlyt.vinhlong.vn/api/",
                LoginApi = "get-token",
                PushApi = "CSDLYTE831/import-csdlyte831-mobile"
            };
        }

        /// <summary>Parse chuỗi cấu hình. Thiếu trường / rỗng -&gt; null.</summary>
        internal static Ksk831SyncConfig Parse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            string[] p = raw.Split('|');
            if (p.Length < 5) return null;
            var cfg = new Ksk831SyncConfig
            {
                Username = p[0].Trim(),
                Password = p[1].Trim(),
                BaseUrl = p[2].Trim(),
                LoginApi = p[3].Trim(),
                PushApi = p[4].Trim()
            };
            if (string.IsNullOrEmpty(cfg.BaseUrl) || string.IsNullOrEmpty(cfg.LoginApi) || string.IsNullOrEmpty(cfg.PushApi))
                return null;
            return cfg;
        }

        private static string Combine(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl)) return path;
            if (string.IsNullOrEmpty(path)) return baseUrl;
            return baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }
    }
}
