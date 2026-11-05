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
using DevExpress.XtraLayout;
using System;
using System.IO;
using System.Reflection;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    /// <summary>
    /// Bố cục phiên bản 2 của màn trả kết quả (CĐHA/TDCN/Nội soi/Siêu âm/GPBL):
    ///   - Camera chiếm trọn bên trái, các nút thao tác chuyển lên phía trên cột ảnh.
    ///   - Ba ô thời gian (TG bắt đầu / TG kết thúc / TG kết quả) gom chung một dòng, hiện đủ ngày giờ.
    ///   - Số phim + Cỡ phim lên cùng dòng TG y lệnh; ô Kết luận rộng toàn tab và cao hơn.
    ///   - Ba nút Chỉ định (F9) / Kê đơn (F8) / Kê đơn YHCT nới rộng để hiện đủ chữ.
    ///
    /// Bố cục nằm trong hai tệp XML nhúng (sinh bằng LayoutControl.SaveLayoutToXml), nạp lại
    /// bằng RestoreLayoutFromStream — cùng cơ chế với <see cref="InitRestoreLayout"/>.
    ///
    /// Bật/tắt qua HIS_CONFIG "HIS.Desktop.Plugins.ServiceExecute.IsResultLayoutV2".
    /// Mặc định TẮT (khác "1"): KHÔNG nạp gì, giao diện giữ nguyên như cũ.
    /// </summary>
    public partial class UCServiceExecute
    {
        #region Declare ResultLayoutV2
        private const string RESULT_LAYOUT_V2__RESOURCE_PREFIX =
            "HIS.Desktop.Plugins.ServiceExecute.Resources.ResultLayoutV2.";

        private bool isResultLayoutV2;
        #endregion

        /// <summary>
        /// Gọi ngay trong constructor, sau InitializeComponent và trước mọi tùy biến nghiệp vụ
        /// (Expanded của nhóm, ProcessCustomizeUI) để không ghi đè các tùy biến đó.
        /// </summary>
        private void InitResultLayoutV2()
        {
            try
            {
                isResultLayoutV2 = Config.AppConfigKeys.IsResultLayoutV2;
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => isResultLayoutV2), isResultLayoutV2));
                if (!isResultLayoutV2) return;

                RestoreResultLayoutV2(this.layoutControl1);
                RestoreResultLayoutV2(this.layoutControl2);
                ApplyResultLayoutV2Editors();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RestoreResultLayoutV2(LayoutControl layoutControl)
        {
            try
            {
                if (layoutControl == null) return;

                string resourceName = RESULT_LAYOUT_V2__RESOURCE_PREFIX + layoutControl.Name + ".xml";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(
                            "ResultLayoutV2: khong tim thay resource " + resourceName);
                        return;
                    }
                    layoutControl.RestoreLayoutFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Các thuộc tính của ô nhập không nằm trong tệp bố cục nên phải đặt riêng:
        /// bỏ phần giây để ngày giờ hiện đủ trong ô hẹp, thêm chữ mờ và chú thích khi trỏ chuột.
        /// </summary>
        private void ApplyResultLayoutV2Editors()
        {
            try
            {
                this.dtBeginTime.Properties.NullValuePrompt = "Bắt đầu";
                this.dtBeginTime.Properties.NullValuePromptShowForEmptyValue = true;
                this.dtBeginTime.ToolTip = "Thời gian bắt đầu thực hiện dịch vụ";

                this.dtEndTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                this.dtEndTime.Properties.NullValuePrompt = "Kết thúc";
                this.dtEndTime.Properties.NullValuePromptShowForEmptyValue = true;
                this.dtEndTime.ToolTip = "Thời gian kết thúc thực hiện dịch vụ";

                this.dtResult.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
                this.dtResult.Properties.NullValuePrompt = "Ngày KQ";
                this.dtResult.Properties.NullValuePromptShowForEmptyValue = true;
                this.dtResult.ToolTip = "Thời gian đọc, trả kết quả";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gọi cuối UCServiceExecute_Load: đa ngôn ngữ chạy sau khi dựng control nên ghi đè nhãn
        /// lấy từ tệp bố cục — đặt lại ba nhãn thời gian cho khớp bề ngang mới.
        /// </summary>
        private void ApplyResultLayoutV2Captions()
        {
            try
            {
                if (!isResultLayoutV2) return;

                this.LciBeginTime.Text = "TG bắt đầu:";
                this.LciBeginTime.OptionsToolTip.ToolTip = "Thời gian bắt đầu thực hiện";
                this.LciEndTime.Text = "TG kết thúc:";
                this.LciEndTime.OptionsToolTip.ToolTip = "Thời gian kết thúc thực hiện";
                this.lciDateResult.Text = "TG kết quả:";
                this.lciDateResult.OptionsToolTip.ToolTip = "Thời gian đọc, trả kết quả";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
