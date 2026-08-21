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
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    /// <summary>
    /// Mở màn hình mở rộng ra màn hình phụ.
    /// </summary>
    internal static class ShowFormProcessor
    {
        /// <summary>
        /// Mở form kín màn hình phụ. Máy chỉ có một màn thì mở ở màn chính.
        ///
        /// Ba điểm bắt buộc, thiếu cái nào là HIS bị chặn:
        ///  - Show() KHÔNG truyền owner: form có chủ luôn nằm đè lên chủ, người dùng không quay
        ///    lại HIS được.
        ///  - ShowInTaskbar = true: còn lối Alt+Tab / bấm taskbar để về HIS khi chỉ có một màn hình.
        ///  - Không đặt TopMost: đặt là nó đè mọi cửa sổ khác của HIS.
        ///
        /// Thứ tự bắt buộc: Normal → đặt Bounds → Show() → Maximized.
        /// Maximized trước khi Show là hỏng: lúc đó cửa sổ chưa có handle, Windows phóng nó theo
        /// màn hình đang chứa toạ độ mặc định (màn chính), và bản thân việc gán Maximized ghi đè
        /// luôn Bounds vừa đặt — nên form vẫn nằm ở màn chính. Phải phóng SAU khi đã hiện đúng màn.
        /// </summary>
        /// <summary>
        /// Đóng các bảng điện tử đang mở trước khi mở bảng mới. Chỉ cho phép một bảng tại một thời điểm.
        ///
        /// PHẢI gom danh sách trước rồi mới đóng. Application.OpenForms là tập hợp sống — đóng form
        /// ngay trong lúc duyệt nó sẽ ném InvalidOperationException, mà lỗi này bắn ra giữa vòng lặp
        /// thông báo của WinForms nên không ai bắt được, hệ quả là văng cả HIS.
        /// </summary>
        private static void CloseOtherInstances(Form keep)
        {
            try
            {
                List<Form> opened = new List<Form>();
                foreach (Form f in Application.OpenForms)
                {
                    if (f == null || ReferenceEquals(f, keep)) continue;
                    if (f.GetType() == keep.GetType()) opened.Add(f);
                }

                for (int i = 0; i < opened.Count; i++)
                {
                    try
                    {
                        Inventec.Common.Logging.LogSystem.Info(
                            "Dong bang dien tu dang mo truoc khi mo bang moi: " + opened[i].Name);

                        // Close() se keo theo Dispose, nho vay ban to lam moi va ban to tu cuon
                        // cua bang cu deu dung han. Bo qua buoc nay thi chung van chay ngam,
                        // van goi API va van cham vao control da huy.
                        opened[i].Close();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void ShowFullScreenOnSecondMonitor(Form form)
        {
            if (form == null) return;

            try
            {
                CloseOtherInstances(form);

                Screen[] screens = Screen.AllScreens.OrderBy(o => !o.Primary).ToArray();
                Screen target = screens.Length > 1 ? screens[1] : Screen.PrimaryScreen;

                if (screens.Length <= 1)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Chi co mot man hinh, mo bang dien tu ngay tren man chinh");
                }

                form.StartPosition = FormStartPosition.Manual;
                form.WindowState = FormWindowState.Normal;
                form.Bounds = target.Bounds;

                form.Show();

                // Phong to sau khi da hien: luc nay handle da co, Windows phong dung man dang chua form
                form.WindowState = FormWindowState.Maximized;
                form.BringToFront();
                form.Activate();

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "Mo bang dien tu tren man hinh {0} ({1}), tong so man hinh: {2}",
                    target.DeviceName, target.Bounds, screens.Length));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                try
                {
                    form.Show();
                }
                catch (Exception ex2)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex2);
                }
            }
        }
    }
}
