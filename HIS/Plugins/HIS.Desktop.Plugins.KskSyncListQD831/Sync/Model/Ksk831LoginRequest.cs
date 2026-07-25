/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync.Model
{
    /// <summary>
    /// Đầu vào API đăng nhập /get-token (multipart/form-data) theo tài liệu HSSK 831:
    /// { "username": "Tên đăng nhập", "password": "Mật khẩu" }.
    /// </summary>
    internal class Ksk831LoginRequest
    {
        /// <summary>username — Chuỗi, tối đa 50, bắt buộc.</summary>
        internal string Username { get; set; }

        /// <summary>password — Chuỗi, tối đa 30, bắt buộc.</summary>
        internal string Password { get; set; }
    }
}
