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
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    /// <summary>
    /// Cảnh báo lô sắp hết hạn sử dụng trên cây tồn kho thuốc + vật tư (không áp dụng nhánh Máu).
    /// Tô màu NỀN dòng lô theo hạn còn lại tính từ ngày hiện tại (tháng lịch, so sánh theo ngày):
    ///   - Nền đỏ        : lô đã quá hạn sử dụng
    ///   - Nền hồng nhạt : lô còn hạn dưới 3 tháng — sắp hết hạn, ưu tiên xử lý/xuất trước
    ///   - Nền vàng      : lô còn hạn từ 3 đến dưới 6 tháng — cần lưu ý theo dõi hạn dùng
    ///   - Không tô      : lô còn hạn từ 6 tháng trở lên hoặc không có thông tin hạn sử dụng
    /// Chỉ dùng màu nền — giữ nguyên toàn bộ logic màu CHỮ hiện có (đỏ = dưới tồn tối thiểu,
    /// xanh dương = lô ưu tiên xuất trước). Node loại (node cha) không tô.
    /// </summary>
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        #region Màu cảnh báo HSD (tông nhạt — đảm bảo chữ đen/xanh dương vẫn đọc rõ)
        /// <summary>Nền đỏ — lô đã quá hạn sử dụng</summary>
        private static readonly Color ExpiredDateExpiredBackColor = Color.FromArgb(255, 122, 122);
        /// <summary>Nền hồng nhạt — lô còn hạn dưới 3 tháng</summary>
        private static readonly Color ExpiredDateUnder3MonthsBackColor = Color.FromArgb(255, 205, 210);
        /// <summary>Nền vàng — lô còn hạn từ 3 đến dưới 6 tháng</summary>
        private static readonly Color ExpiredDateUnder6MonthsBackColor = Color.FromArgb(255, 255, 153);
        #endregion

        #region Mốc cảnh báo (cache theo ngày — NodeCellStyle gọi mỗi cell khi repaint nên phải nhẹ)
        /// <summary>Ngày hiện tại dạng yyyyMMdd</summary>
        private long expiredDateWarningToday;
        /// <summary>Mốc [ngày hiện tại + 3 tháng lịch] dạng yyyyMMdd</summary>
        private long expiredDateWarning3Months;
        /// <summary>Mốc [ngày hiện tại + 6 tháng lịch] dạng yyyyMMdd</summary>
        private long expiredDateWarning6Months;
        /// <summary>Ngày đã tính mốc — qua ngày mới thì tính lại</summary>
        private DateTime expiredDateWarningComputedDate = DateTime.MinValue;
        #endregion

        /// <summary>Chú giải màu cảnh báo HSD — hiển thị dưới cây tồn kho thuốc/vật tư</summary>
        private DevExpress.XtraEditors.PanelControl pnlExpiredDateLegend;

        /// <summary>
        /// Tính lại 3 mốc cảnh báo khi sang ngày mới (tháng lịch: DateTime.AddMonths).
        /// </summary>
        private void EnsureExpiredDateWarningThresholds()
        {
            DateTime today = DateTime.Now.Date;
            if (this.expiredDateWarningComputedDate == today)
                return;
            this.expiredDateWarningToday = Int64.Parse(today.ToString("yyyyMMdd"));
            this.expiredDateWarning3Months = Int64.Parse(today.AddMonths(3).ToString("yyyyMMdd"));
            this.expiredDateWarning6Months = Int64.Parse(today.AddMonths(6).ToString("yyyyMMdd"));
            this.expiredDateWarningComputedDate = today;
        }

        /// <summary>
        /// Xác định màu nền cảnh báo HSD cho dòng lô. Mỗi dòng chỉ một màu — mức nặng ưu tiên:
        /// Đỏ (quá hạn) → Hồng nhạt (còn &lt; 3 tháng; HSD đúng hôm nay = hồng nhạt) → Vàng (3–&lt; 6 tháng).
        /// </summary>
        /// <param name="expiredDate">Hạn sử dụng của lô dạng yyyyMMddHHmmss; &lt;= 0 = không có HSD</param>
        /// <param name="backColor">Màu nền cảnh báo nếu có</param>
        /// <returns>true nếu dòng lô cần tô màu cảnh báo</returns>
        private bool TryGetExpiredDateWarningBackColor(long expiredDate, out Color backColor)
        {
            backColor = Color.Empty;
            try
            {
                // Lô không có thông tin hạn sử dụng → không tô, không báo lỗi
                if (expiredDate <= 0)
                    return false;
                EnsureExpiredDateWarningThresholds();
                long expiredDay = expiredDate / 1000000; // yyyyMMddHHmmss → yyyyMMdd (so sánh theo ngày, bỏ giờ phút)
                if (expiredDay < this.expiredDateWarningToday)
                {
                    backColor = ExpiredDateExpiredBackColor;
                    return true;
                }
                if (expiredDay < this.expiredDateWarning3Months)
                {
                    backColor = ExpiredDateUnder3MonthsBackColor;
                    return true;
                }
                if (expiredDay < this.expiredDateWarning6Months)
                {
                    backColor = ExpiredDateUnder6MonthsBackColor;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Thêm chú giải màu vào panelControlMediMate. Gọi SAU khi add cây thuốc/vật tư (Dock = Fill)
        /// để chú giải dock Bottom chiếm dải dưới, cây chiếm phần còn lại. Không gọi cho nhánh Máu.
        /// panelControlMediMate.Controls.Clear() mỗi lần đổi nhánh nên phải add lại mỗi lần.
        /// </summary>
        private void AddExpiredDateLegendToPanel()
        {
            try
            {
                if (this.pnlExpiredDateLegend == null)
                    InitExpiredDateLegend();
                if (this.pnlExpiredDateLegend != null && !this.panelControlMediMate.Controls.Contains(this.pnlExpiredDateLegend))
                    this.panelControlMediMate.Controls.Add(this.pnlExpiredDateLegend);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khởi tạo panel chú giải: [Cảnh báo HSD:] [Đã quá hạn] [Còn dưới 3 tháng] [Còn 3 đến dưới 6 tháng].
        /// </summary>
        private void InitExpiredDateLegend()
        {
            try
            {
                this.pnlExpiredDateLegend = new DevExpress.XtraEditors.PanelControl();
                this.pnlExpiredDateLegend.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                this.pnlExpiredDateLegend.Height = 26;
                this.pnlExpiredDateLegend.Dock = DockStyle.Bottom;

                // Dock = Left: control add SAU được dock TRƯỚC (nằm trái nhất) → add ngược thứ tự hiển thị
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendLabel(
                    GetExpiredDateLegendText("UCMediStockSummary.lblExpiredDateLegend3To6Months.Text"),
                    ExpiredDateUnder6MonthsBackColor));
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendSpacer());
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendLabel(
                    GetExpiredDateLegendText("UCMediStockSummary.lblExpiredDateLegendUnder3Months.Text"),
                    ExpiredDateUnder3MonthsBackColor));
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendSpacer());
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendLabel(
                    GetExpiredDateLegendText("UCMediStockSummary.lblExpiredDateLegendExpired.Text"),
                    ExpiredDateExpiredBackColor));
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendSpacer());
                this.pnlExpiredDateLegend.Controls.Add(CreateExpiredDateLegendLabel(
                    GetExpiredDateLegendText("UCMediStockSummary.lblExpiredDateLegendTitle.Text"),
                    null));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetExpiredDateLegendText(string languageKey)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    languageKey,
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        private DevExpress.XtraEditors.LabelControl CreateExpiredDateLegendLabel(string text, Color? backColor)
        {
            DevExpress.XtraEditors.LabelControl lbl = new DevExpress.XtraEditors.LabelControl();
            lbl.Text = text;
            lbl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lbl.Dock = DockStyle.Left;
            lbl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lbl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            lbl.Appearance.Options.UseTextOptions = true;
            if (backColor.HasValue)
            {
                lbl.Appearance.BackColor = backColor.Value;
                lbl.Appearance.Options.UseBackColor = true;
            }
            lbl.Width = lbl.CalcBestSize().Width + 16;
            return lbl;
        }

        private Control CreateExpiredDateLegendSpacer()
        {
            DevExpress.XtraEditors.PanelControl spacer = new DevExpress.XtraEditors.PanelControl();
            spacer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            spacer.Width = 8;
            spacer.Dock = DockStyle.Left;
            return spacer;
        }
    }
}
