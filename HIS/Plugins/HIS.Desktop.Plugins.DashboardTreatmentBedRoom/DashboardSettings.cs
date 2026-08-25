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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    /// <summary>
    /// Nhớ số cột và thời gian tải lại người dùng đã chọn, để lần sau mở form khỏi nhập lại.
    ///
    /// Ghi vào thư mục dữ liệu riêng của tài khoản Windows chứ không ghi cạnh file chạy:
    /// HIS thường nằm trong Program Files, ghi vào đó cần quyền quản trị và sẽ hỏng âm thầm
    /// trên máy người dùng thường. Mỗi tài khoản Windows có cấu hình riêng, đúng ý nghĩa
    /// "cái tôi vừa chọn" hơn là một giá trị dùng chung cho cả máy.
    ///
    /// Mọi lỗi đọc/ghi đều nuốt và trả về mặc định: không nhớ được thiết lập là chuyện nhỏ,
    /// không được phép làm hỏng việc mở màn hình.
    /// </summary>
    internal static class DashboardSettings
    {
        private const string FOLDER_NAME = "HIS.DashboardTreatmentBedRoom";
        private const string FILE_NAME = "settings.cfg";
        private const string KEY_COLUMN = "COLUMN_COUNT";
        private const string KEY_RELOAD = "RELOAD_SECOND";

        private static string GetFilePath()
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                FOLDER_NAME);
            return Path.Combine(dir, FILE_NAME);
        }

        /// <summary>
        /// Đọc thiết lập đã lưu. Trả 0 cho giá trị không có hoặc không đọc được,
        /// nơi gọi cứ giữ nguyên giá trị mặc định trên form.
        /// </summary>
        public static void Load(out int columnCount, out int reloadSecond)
        {
            columnCount = 0;
            reloadSecond = 0;

            try
            {
                string path = GetFilePath();
                if (!File.Exists(path)) return;

                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    int pos = line.IndexOf('=');
                    if (pos <= 0) continue;

                    string key = line.Substring(0, pos).Trim();
                    string value = line.Substring(pos + 1).Trim();

                    int parsed;
                    if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)) continue;

                    if (key == KEY_COLUMN) columnCount = parsed;
                    else if (key == KEY_RELOAD) reloadSecond = parsed;
                }

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "Doc thiet lap da luu: so cot={0}, thoi gian tai lai={1}s", columnCount, reloadSecond));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                columnCount = 0;
                reloadSecond = 0;
            }
        }

        /// <summary>Ghi lại thiết lập vừa dùng. Chỉ gọi sau khi giá trị đã qua kiểm tra hợp lệ.</summary>
        public static void Save(int columnCount, int reloadSecond)
        {
            try
            {
                string path = GetFilePath();
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                List<string> lines = new List<string>();
                lines.Add(KEY_COLUMN + "=" + columnCount.ToString(CultureInfo.InvariantCulture));
                lines.Add(KEY_RELOAD + "=" + reloadSecond.ToString(CultureInfo.InvariantCulture));

                File.WriteAllLines(path, lines.ToArray(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
