/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Config
{
    /// <summary>
    /// Đọc cấu hình kết nối EMRTOOLKIT từ HisConfig toàn viện.
    ///
    /// Key: <c>HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo</c>
    /// Giá trị có dạng: <c>&lt;địa chỉ&gt;|&lt;tài khoản&gt;|&lt;mật khẩu&gt;</c>
    /// (tùy chọn thêm phần thứ 4 là IDMauPhieu Giấy Chuyển Viện).
    /// </summary>
    internal class EmrToolkitConfigCFG
    {
        /// <summary>Key cấu hình hệ thống khai báo thông tin cổng EmrToolkit.</summary>
        internal const string CONFIG_KEY__CONNECTION_INFO = "HIS.Desktop.Plugins.EmrToolKit.ConnectionInfo";

        private const int DEFAULT_ID_MAU_PHIEU_GCV = 524;
        private const int DEFAULT_TIMEOUT_SECONDS = 60;

        /// <summary>Địa chỉ gốc EMRTOOLKIT (không có dấu '/' cuối)</summary>
        internal static string BaseUrl { get; private set; }

        /// <summary>Tài khoản đăng nhập</summary>
        internal static string Username { get; private set; }

        /// <summary>Mật khẩu đăng nhập</summary>
        internal static string Password { get; private set; }

        /// <summary>ID mẫu phiếu Giấy Chuyển Viện</summary>
        internal static int IdMauPhieuGiayChuyenVien { get; private set; }

        /// <summary>Timeout HTTP (giây)</summary>
        internal static int TimeoutSeconds { get; private set; }

        /// <summary>Cấu hình ConnectionInfo có giá trị hợp lệ (đủ để gọi API) hay không.</summary>
        internal static bool HasConnectionInfo { get; private set; }

        /// <summary>
        /// Nạp cấu hình từ HisConfig. Gọi mỗi lần trước khi thực hiện luồng import.
        /// </summary>
        internal static void LoadConfig()
        {
            BaseUrl = "";
            Username = "";
            Password = "";
            IdMauPhieuGiayChuyenVien = DEFAULT_ID_MAU_PHIEU_GCV;
            TimeoutSeconds = DEFAULT_TIMEOUT_SECONDS;
            HasConnectionInfo = false;

            try
            {
                string raw = HisConfigs.Get<string>(CONFIG_KEY__CONNECTION_INFO);
                ParseConnectionInfo(raw);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra nhanh key ConnectionInfo có giá trị hay không (cho điều kiện hiển thị menu).
        /// </summary>
        internal static bool CheckHasConnectionInfo()
        {
            try
            {
                return !string.IsNullOrWhiteSpace(HisConfigs.Get<string>(CONFIG_KEY__CONNECTION_INFO));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>Tách chuỗi cấu hình dạng địa_chỉ|tài_khoản|mật_khẩu[|IDMauPhieu].</summary>
        private static void ParseConnectionInfo(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return;

            string[] parts = raw.Split('|');
            if (parts.Length >= 1)
                BaseUrl = (parts[0] ?? "").Trim().TrimEnd('/');
            if (parts.Length >= 2)
                Username = (parts[1] ?? "").Trim();
            if (parts.Length >= 3)
                Password = (parts[2] ?? "").Trim();

            // Tùy chọn: phần thứ 4 là IDMauPhieu
            if (parts.Length >= 4)
            {
                int value;
                if (int.TryParse((parts[3] ?? "").Trim(), out value) && value > 0)
                    IdMauPhieuGiayChuyenVien = value;
            }

            HasConnectionInfo = !string.IsNullOrWhiteSpace(BaseUrl)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
