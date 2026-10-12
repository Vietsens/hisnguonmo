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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Màn tivi theo mockup của viện: dải trên trắng (tên viện, khoa, ngày giờ), thẻ trắng chứa thanh "DANH SÁCH CHỜ" navy
    /// và bảng 3 cột có icon tiêu đề, thanh "ĐANG GỌI" nền teal với tên bệnh nhân màu vàng.
    /// Nhận lượt gọi từ màn Viện phí qua SetPatient (cùng kênh với Màn hình chờ thu ngân 2).
    /// </summary>
    public partial class frmWaitingScreenCashierV3 : HIS.Desktop.Utility.FormBase
    {
        private WaitingScreenConfigADO cfg;
        private V_HIS_ROOM room;
        private string cashierRoomName;
        private readonly WaitingScreenDataWorker dataWorker = new WaitingScreenDataWorker();
        private List<WaitingPatientADO> allRows = new List<WaitingPatientADO>();
        private int pageIndex = 0;
        private volatile bool isLoading = false;
        private string currentCallTreatmentCode;
        private string currentCallName;
        private string currentCallYear;
        private int blinkCount = 0;
        private Color callingNameColor = ColorTranslator.FromHtml("#FFD400");
        private ImageList headerImages;

        private static readonly string[] DAY_NAMES = { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
        private static readonly Color HEADER_STRIP_BACK = Color.White;
        private static readonly Color BLUE = ColorTranslator.FromHtml("#1F5FA0");
        private static readonly Color NAVY = ColorTranslator.FromHtml("#0E3A5B");
        private static readonly Color LOGO_RED = ColorTranslator.FromHtml("#D9322B");
        private const string LOGO_FILE_NAME = "CallPatientCashierV3.logo.png";
        // Màu thẻ danh sách và thanh đang gọi: đọc từ cấu hình ở ApplyStyle
        private Color columnHeaderBack = ColorTranslator.FromHtml(WaitingScreenConfigADO.DEFAULT_COLUMN_HEADER_BACK_COLOR);
        private Color callRoomColor = Color.White;
        private static readonly Color ROW_ODD_BACK = ColorTranslator.FromHtml("#FFFFFF");
        private static readonly Color HIGHLIGHT_BACK = ColorTranslator.FromHtml("#FFF3C4");
        private static readonly Color HIGHLIGHT_FORE = ColorTranslator.FromHtml("#9A4B00");

        public frmWaitingScreenCashierV3(Inventec.Desktop.Common.Modules.Module module, V_HIS_ROOM room, string cashierRoomName, WaitingScreenConfigADO cfg)
            : base(module)
        {
            InitializeComponent();
            this.room = room;
            this.cashierRoomName = cashierRoomName ?? "";
            this.cfg = cfg ?? WaitingScreenConfigADO.CreateDefault("", null);
            this.cfg.Normalize();
            // Kênh nhận lượt gọi: màn Viện phí tìm form theo tên này và gọi SetPatient (như Màn hình chờ thu ngân 2)
            this.Name = "CALL_PATIENT_CASHIER_TWO_" + (room != null ? room.ROOM_CODE : "");
            SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmWaitingScreenCashierV3_Load(object sender, EventArgs e)
        {
            try
            {
                lblBranch.Text = (BranchDataWorker.Branch != null ? (BranchDataWorker.Branch.BRANCH_NAME ?? "") : "").ToUpper();
                lblCalling.Text = "";
                lblPage.Text = "";
                ApplyStyle();
                UpdateClock();
                timerClock.Interval = 1000;
                timerClock.Start();
                RestartTimers();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Nhận cấu hình mới từ form cấu hình (nút Lưu): vẽ lại và làm mới ngay (R14).</summary>
        public void ApplyConfig(WaitingScreenConfigADO newConfig)
        {
            try
            {
                if (newConfig == null) return;
                newConfig.Normalize();
                this.cfg = newConfig;
                this.pageIndex = 0;
                ApplyStyle();
                RenderCalling(false);
                RestartTimers();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Được màn Viện phí gọi qua reflection khi thu ngân bấm biểu tượng loa trên dòng hồ sơ
        /// (cùng chữ ký với Màn hình chờ thu ngân 2). Hiện "HỌ TÊN (năm sinh) - VÀO <tên phòng thu ngân>" (R10, R11).
        /// </summary>
        public void SetPatient(string nameclient, long dateborn, string treatmentcode, int? counters)
        {
            try
            {
                if (this.IsDisposed) return;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(delegate { SetPatient(nameclient, dateborn, treatmentcode, counters); }));
                    return;
                }
                this.currentCallName = (nameclient ?? "").Trim().ToUpper();
                this.currentCallYear = WaitingScreenDataWorker.GetYear(dateborn);
                this.currentCallTreatmentCode = treatmentcode;
                RenderCalling(false);
                StartBlink();
                gridControl.Invalidate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Hiển thị

        private void ApplyStyle()
        {
            try
            {
                Color back = frmConfigCashierV3.ParseColor(cfg.BackColor, "#0E3A5B");
                Color titleColor = frmConfigCashierV3.ParseColor(cfg.TitleColor, "#FFFFFF");
                Color listColor = frmConfigCashierV3.ParseColor(cfg.ListColor, "#0B2545");
                this.callingNameColor = frmConfigCashierV3.ParseColor(cfg.CallingColor, "#FFD400");

                // Dải trên: nền trắng, chữ xanh (tên viện nhỏ, khoa đậm lớn), logo trái, ngày / giờ + icon đồng hồ bên phải
                this.BackColor = back;
                panelHeader.Appearance.BackColor = HEADER_STRIP_BACK;
                float branchSize = Math.Max(12f, cfg.TitleFontSize * 0.5f);
                float departmentSize = Math.Max(14f, cfg.TitleFontSize * 0.8f);
                lblBranch.Appearance.Font = new Font("Tahoma", branchSize, FontStyle.Bold);
                lblBranch.Appearance.ForeColor = BLUE;
                lblDepartment.Appearance.Font = new Font("Tahoma", departmentSize, FontStyle.Bold);
                lblDepartment.Appearance.ForeColor = NAVY;
                string departmentTitle = !string.IsNullOrWhiteSpace(cfg.DepartmentTitle)
                    ? cfg.DepartmentTitle
                    : (this.room != null ? (this.room.DEPARTMENT_NAME ?? "") : "");
                lblDepartment.Text = departmentTitle.Trim().ToUpper();
                lblClock.Appearance.Font = new Font("Tahoma", Math.Max(12f, cfg.TitleFontSize * 0.45f), FontStyle.Bold);
                lblClock.Appearance.ForeColor = BLUE;
                lblTime.Appearance.Font = new Font("Tahoma", Math.Max(14f, cfg.TitleFontSize * 0.7f), FontStyle.Bold);
                lblTime.Appearance.ForeColor = NAVY;
                lblTime.Appearance.Image = DrawHeaderIcon(4, (int)(Math.Max(14f, cfg.TitleFontSize * 0.7f) * 1.3f), NAVY);
                panelHeader.Height = (int)(branchSize * 1.6f + departmentSize * 1.6f + 24);
                int logoSize = panelHeader.Height - 20;
                picLogo.Size = new Size(logoSize, logoSize);
                picLogo.Image = LoadLogo(logoSize);
                picLogo.Visible = picLogo.Image != null;

                // Thẻ trắng + thanh tiêu đề navy
                panelCard.Appearance.BackColor = back;
                panelTable.Appearance.BackColor = Color.White;
                lblTitle.Appearance.Font = new Font("Tahoma", cfg.TitleFontSize, FontStyle.Bold);
                lblTitle.Appearance.ForeColor = titleColor;
                lblTitle.Appearance.BackColor = frmConfigCashierV3.ParseColor(cfg.ListTitleBackColor, WaitingScreenConfigADO.DEFAULT_LIST_TITLE_BACK_COLOR);
                this.columnHeaderBack = frmConfigCashierV3.ParseColor(cfg.ColumnHeaderBackColor, WaitingScreenConfigADO.DEFAULT_COLUMN_HEADER_BACK_COLOR);
                Color rowAltBack = frmConfigCashierV3.ParseColor(cfg.RowAltBackColor, WaitingScreenConfigADO.DEFAULT_ROW_ALT_BACK_COLOR);
                Color callTitleColor = frmConfigCashierV3.ParseColor(cfg.CallTitleColor, WaitingScreenConfigADO.DEFAULT_CALL_TITLE_COLOR);
                this.callRoomColor = frmConfigCashierV3.ParseColor(cfg.CallRoomColor, WaitingScreenConfigADO.DEFAULT_CALL_ROOM_COLOR);
                lblTitle.Height = (int)(cfg.TitleFontSize * 1.8f);

                // Bảng: tiêu đề cột nền xanh thép chữ trắng có icon, dòng xen kẽ, chữ navy đậm
                float headerFontSize = Math.Max(12f, cfg.ListFontSize * 0.7f);
                gridView.Appearance.Row.Font = new Font("Tahoma", cfg.ListFontSize, FontStyle.Bold);
                gridView.Appearance.Row.ForeColor = listColor;
                gridView.Appearance.Row.BackColor = Color.White;
                gridView.Appearance.EvenRow.BackColor = rowAltBack;
                gridView.Appearance.EvenRow.ForeColor = listColor;
                gridView.Appearance.OddRow.BackColor = ROW_ODD_BACK;
                gridView.Appearance.OddRow.ForeColor = listColor;
                gridView.Appearance.HeaderPanel.Font = new Font("Tahoma", headerFontSize, FontStyle.Bold);
                gridView.Appearance.HeaderPanel.BackColor = this.columnHeaderBack;
                gridView.Appearance.HeaderPanel.BorderColor = this.columnHeaderBack;
                gridView.Appearance.HeaderPanel.ForeColor = Color.White;
                gridView.Appearance.Empty.BackColor = Color.White;
                gridView.Appearance.VertLine.BackColor = Color.White;
                gridView.Appearance.HorzLine.BackColor = Color.White;
                gridView.RowHeight = (int)(cfg.ListFontSize * 2.4f);
                gridView.ColumnPanelRowHeight = (int)(headerFontSize * 2.6f);
                ApplyHeaderIcons((int)(headerFontSize * 1.6f));

                // Thanh đang gọi: gradient teal bằng Appearance của PanelControl
                panelBottom.Appearance.BackColor = frmConfigCashierV3.ParseColor(cfg.CallBarBackColor, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR);
                panelBottom.Appearance.BackColor2 = frmConfigCashierV3.ParseColor(cfg.CallBarBackColor2, WaitingScreenConfigADO.DEFAULT_CALL_BAR_BACK_COLOR2);
                panelBottom.Appearance.GradientMode = LinearGradientMode.Horizontal;
                float callTitleSize = Math.Max(12f, cfg.CallingFontSize * 0.7f);
                lblCallingTitle.Appearance.Font = new Font("Tahoma", callTitleSize, FontStyle.Bold);
                lblCallingTitle.Appearance.ForeColor = callTitleColor;
                lblCallingTitle.Appearance.Image = DrawHeaderIcon(3, (int)(callTitleSize * 1.4f), callTitleColor);
                lblCalling.Appearance.Font = new Font("Tahoma", cfg.CallingFontSize, FontStyle.Bold);
                lblCalling.Appearance.ForeColor = Color.White;
                panelBottom.Height = (int)(cfg.CallingFontSize * 1.6f + Math.Max(12f, cfg.CallingFontSize * 0.7f) * 1.6f + 30);
                lblPage.Appearance.Font = new Font("Tahoma", 14f, FontStyle.Bold);
                lblPage.Appearance.ForeColor = Color.White;

                LayoutHeader();
                LayoutBottom();
                panelBottom.Invalidate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Icon trắng vẽ bằng GDI+ cho tiêu đề cột (người, lịch, lớp) như mockup; vẽ ra ở CustomDrawColumnHeader.</summary>
        private void ApplyHeaderIcons(int size)
        {
            try
            {
                if (size < 12) size = 12;
                ImageList old = this.headerImages;
                ImageList images = new ImageList();
                images.ColorDepth = ColorDepth.Depth32Bit;
                images.ImageSize = new Size(size, size);
                images.Images.Add(DrawHeaderIcon(0, size, Color.White));
                images.Images.Add(DrawHeaderIcon(1, size, Color.White));
                images.Images.Add(DrawHeaderIcon(2, size, Color.White));
                this.headerImages = images;
                gridView.Invalidate();
                if (old != null) old.Dispose();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private int GetHeaderIconIndex(DevExpress.XtraGrid.Columns.GridColumn column)
        {
            if (column == colPatientName) return 0;
            if (column == colYearOfBirth) return 1;
            if (column == colServiceGroups) return 2;
            return -1;
        }

        /// <summary>
        /// Tiêu đề cột theo mockup: nền xanh thép, icon + chữ trắng căn giữa, vạch ngăn trắng mờ giữa các cột.
        /// Vẽ qua sự kiện CustomDrawColumnHeader của GridView (không phụ thuộc skin).
        /// </summary>
        private void gridView_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                Rectangle r = e.Bounds;
                using (SolidBrush back = new SolidBrush(this.columnHeaderBack))
                {
                    e.Graphics.FillRectangle(back, r);
                }
                if (e.Column == null)
                {
                    e.Handled = true;
                    return;
                }
                if (e.Column.VisibleIndex < gridView.VisibleColumns.Count - 1)
                {
                    using (Pen sep = new Pen(Color.FromArgb(140, 255, 255, 255)))
                    {
                        e.Graphics.DrawLine(sep, r.Right - 1, r.Top + 6, r.Right - 1, r.Bottom - 6);
                    }
                }

                Font font = gridView.Appearance.HeaderPanel.Font;
                string text = e.Column.GetCaption();
                SizeF textSize = e.Graphics.MeasureString(text, font);
                Image icon = null;
                int iconIndex = GetHeaderIconIndex(e.Column);
                if (this.headerImages != null && iconIndex >= 0 && iconIndex < this.headerImages.Images.Count)
                {
                    icon = this.headerImages.Images[iconIndex];
                }
                float gap = icon != null ? 12f : 0f;
                float iconWidth = icon != null ? icon.Width : 0f;
                float total = iconWidth + gap + textSize.Width;
                float x = r.Left + Math.Max(4f, (r.Width - total) / 2f);
                if (icon != null)
                {
                    e.Graphics.DrawImage(icon, x, r.Top + (r.Height - icon.Height) / 2f, icon.Width, icon.Height);
                }
                using (SolidBrush fore = new SolidBrush(Color.White))
                {
                    e.Graphics.DrawString(text, font, fore, x + iconWidth + gap, r.Top + (r.Height - textSize.Height) / 2f);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Logo góc trái: ưu tiên file ảnh của viện đặt cạnh plugin (Plugins\Module\CallPatientCashierV3.logo.png), không có thì vẽ logo mặc định.</summary>
        private static Image LoadLogo(int size)
        {
            try
            {
                string path = System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "Plugins", "Module", LOGO_FILE_NAME);
                if (System.IO.File.Exists(path))
                {
                    using (Image file = Image.FromFile(path))
                    {
                        return new Bitmap(file);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            // Không có logo của viện thì không hiện logo (người phụ trách yêu cầu bỏ logo vẽ sẵn 08/10/2026)
            return null;
        }

        /// <summary>
        /// Icon vẽ bằng GDI+: 0 người, 1 lịch, 2 lớp (tiêu đề cột); 3 loa (ĐANG GỌI); 4 đồng hồ (giờ); 5 logo mặc định (vòng tròn xanh, chữ thập trắng, tim đỏ).
        /// </summary>
        private static Bitmap DrawHeaderIcon(int kind, int size, Color color)
        {
            Bitmap bmp = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (Pen pen = new Pen(color, Math.Max(2f, size / 10f)))
                using (SolidBrush brush = new SolidBrush(color))
                {
                    float s = size;
                    switch (kind)
                    {
                        case 3: // loa: thân chữ nhật + loa hình thang + 2 cung sóng
                            g.FillRectangle(brush, s * 0.1f, s * 0.36f, s * 0.2f, s * 0.28f);
                            g.FillPolygon(brush, new PointF[]
                            {
                                new PointF(s * 0.3f, s * 0.36f), new PointF(s * 0.55f, s * 0.14f),
                                new PointF(s * 0.55f, s * 0.86f), new PointF(s * 0.3f, s * 0.64f)
                            });
                            g.DrawArc(pen, s * 0.42f, s * 0.26f, s * 0.36f, s * 0.48f, -55, 110);
                            g.DrawArc(pen, s * 0.46f, s * 0.1f, s * 0.5f, s * 0.8f, -55, 110);
                            break;
                        case 4: // đồng hồ: vòng tròn + kim giờ, kim phút
                            float d = s * 0.78f;
                            g.DrawEllipse(pen, (s - d) / 2f, (s - d) / 2f, d, d);
                            g.DrawLine(pen, s * 0.5f, s * 0.5f, s * 0.5f, s * 0.24f);
                            g.DrawLine(pen, s * 0.5f, s * 0.5f, s * 0.7f, s * 0.6f);
                            break;
                        case 5: // logo mặc định: vòng tròn xanh, chữ thập trắng, tim đỏ nhỏ góc dưới phải
                            g.FillEllipse(brush, s * 0.04f, s * 0.04f, s * 0.92f, s * 0.92f);
                            using (SolidBrush white = new SolidBrush(Color.White))
                            {
                                g.FillRectangle(white, s * 0.42f, s * 0.2f, s * 0.16f, s * 0.6f);
                                g.FillRectangle(white, s * 0.2f, s * 0.42f, s * 0.6f, s * 0.16f);
                            }
                            using (SolidBrush red = new SolidBrush(LOGO_RED))
                            {
                                float hs = s * 0.26f;
                                float hx = s * 0.6f;
                                float hy = s * 0.58f;
                                g.FillEllipse(red, hx, hy, hs * 0.55f, hs * 0.55f);
                                g.FillEllipse(red, hx + hs * 0.42f, hy, hs * 0.55f, hs * 0.55f);
                                g.FillPolygon(red, new PointF[]
                                {
                                    new PointF(hx + hs * 0.02f, hy + hs * 0.38f), new PointF(hx + hs * 0.95f, hy + hs * 0.38f),
                                    new PointF(hx + hs * 0.485f, hy + hs * 0.95f)
                                });
                            }
                            break;
                        case 0: // người: đầu tròn + vai bán nguyệt
                            float r = s * 0.17f;
                            g.FillEllipse(brush, s * 0.5f - r, s * 0.12f, 2 * r, 2 * r);
                            g.FillPie(brush, s * 0.15f, s * 0.52f, s * 0.7f, s * 0.7f, 180, 180);
                            break;
                        case 1: // lịch: khung + thanh đầu + 2 vòng treo + 3 ô ngày
                            RectangleF rc = new RectangleF(s * 0.12f, s * 0.22f, s * 0.76f, s * 0.66f);
                            g.DrawRectangle(pen, rc.X, rc.Y, rc.Width, rc.Height);
                            g.FillRectangle(brush, rc.X, rc.Y, rc.Width, s * 0.16f);
                            g.FillRectangle(brush, s * 0.28f, s * 0.08f, s * 0.08f, s * 0.22f);
                            g.FillRectangle(brush, s * 0.64f, s * 0.08f, s * 0.08f, s * 0.22f);
                            g.FillRectangle(brush, s * 0.24f, s * 0.5f, s * 0.12f, s * 0.11f);
                            g.FillRectangle(brush, s * 0.44f, s * 0.5f, s * 0.12f, s * 0.11f);
                            g.FillRectangle(brush, s * 0.64f, s * 0.5f, s * 0.12f, s * 0.11f);
                            g.FillRectangle(brush, s * 0.24f, s * 0.68f, s * 0.12f, s * 0.11f);
                            g.FillRectangle(brush, s * 0.44f, s * 0.68f, s * 0.12f, s * 0.11f);
                            break;
                        default: // lớp: 3 hình thoi xếp chồng
                            for (int i = 2; i >= 0; i--)
                            {
                                float dy = s * 0.17f * i;
                                PointF[] pts = new PointF[]
                                {
                                    new PointF(s * 0.5f, s * 0.1f + dy),
                                    new PointF(s * 0.9f, s * 0.3f + dy),
                                    new PointF(s * 0.5f, s * 0.5f + dy),
                                    new PointF(s * 0.1f, s * 0.3f + dy)
                                };
                                if (i == 0) g.FillPolygon(brush, pts);
                                else g.DrawPolygon(pen, pts);
                            }
                            break;
                    }
                }
            }
            return bmp;
        }

        private void LayoutHeader()
        {
            try
            {
                picLogo.Location = new Point(20, 10);
                int textLeft = picLogo.Visible ? 20 + picLogo.Width + 14 : 24;
                lblBranch.Location = new Point(textLeft, 10);
                lblDepartment.Location = new Point(textLeft, 10 + lblBranch.Height + 2);
                lblClock.Location = new Point(Math.Max(20, panelHeader.Width - lblClock.Width - 24), 10);
                lblTime.Location = new Point(Math.Max(20, panelHeader.Width - lblTime.Width - 24), 10 + lblClock.Height + 2);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LayoutBottom()
        {
            try
            {
                lblCallingTitle.Location = new Point(24, 10);
                lblCalling.Location = new Point(70, 10 + lblCallingTitle.Height + 4);
                lblPage.Location = new Point(Math.Max(20, panelBottom.Width - lblPage.Width - 24), 12);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void panelHeader_Resize(object sender, EventArgs e)
        {
            LayoutHeader();
        }

        private void panelBottom_Resize(object sender, EventArgs e)
        {
            LayoutBottom();
        }

        private void UpdateClock()
        {
            try
            {
                DateTime now = DateTime.Now;
                lblClock.Text = string.Format("{0}, {1:dd/MM/yyyy}", DAY_NAMES[(int)now.DayOfWeek], now).ToUpper();
                lblTime.Text = now.ToString("HH:mm") + " ";
                LayoutHeader();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        /// <summary>Dòng đang gọi: tên (năm sinh) màu vàng cấu hình, phần "VÀO <phòng>" màu trắng; blinkOn = đổi tên sang trắng khi nháy.</summary>
        private void RenderCalling(bool blinkOn)
        {
            try
            {
                if (string.IsNullOrEmpty(this.currentCallName))
                {
                    lblCalling.Text = "";
                    LayoutBottom();
                    return;
                }
                // Định dạng HTML của LabelControl DevExpress: <color=R,G,B>
                Color c = blinkOn ? Color.White : this.callingNameColor;
                string nameColor = string.Format("{0},{1},{2}", c.R, c.G, c.B);
                string roomName = (this.cashierRoomName ?? "").Trim().ToUpper();
                string namePart = string.IsNullOrEmpty(this.currentCallYear)
                    ? this.currentCallName
                    : string.Format("{0} ({1})", this.currentCallName, this.currentCallYear);
                string roomColor = string.Format("{0},{1},{2}", this.callRoomColor.R, this.callRoomColor.G, this.callRoomColor.B);
                lblCalling.Text = string.Format("<color={0}>{1}</color><color={3}> - VÀO {2}</color>", nameColor, namePart, roomName, roomColor);
                LayoutBottom();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Dữ liệu

        private void RestartTimers()
        {
            try
            {
                timerReload.Stop();
                timerReload.Interval = Math.Max(5, cfg.RefreshSeconds) * 1000;
                timerReload.Start();
                timerPage.Stop();
                timerPage.Interval = Math.Max(5, cfg.PageSeconds) * 1000;
                timerPage.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerReload_Tick(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

        private void LoadDataAsync()
        {
            try
            {
                if (this.isLoading) return;
                this.isLoading = true;
                WaitingScreenConfigADO snapshot = this.cfg;
                Task.Factory.StartNew(delegate
                {
                    List<WaitingPatientADO> data = null;
                    try
                    {
                        data = this.dataWorker.Load(snapshot);
                    }
                    catch (Exception ex)
                    {
                        LogSystem.Warn(ex);
                    }
                    try
                    {
                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(delegate
                            {
                                try { ApplyData(data); }
                                finally { this.isLoading = false; }
                            }));
                        }
                        else
                        {
                            this.isLoading = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.isLoading = false;
                        LogSystem.Warn(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                this.isLoading = false;
                LogSystem.Error(ex);
            }
        }

        /// <summary>null = lần làm mới lỗi → giữ nguyên dữ liệu cũ, không báo trên tivi.</summary>
        private void ApplyData(List<WaitingPatientADO> data)
        {
            try
            {
                if (data == null) return;
                if (this.allRows == null || this.allRows.Count != data.Count)
                {
                    LogSystem.Info("CallPatientCashierV3 | Tivi cap nhat: " + data.Count + " dong (truoc do " + (this.allRows != null ? this.allRows.Count : 0) + ")");
                }
                this.allRows = data;
                if (this.pageIndex >= PageCount) this.pageIndex = 0;
                BindPage();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private int PageCount
        {
            get
            {
                int maxRows = Math.Max(1, cfg.MaxRows);
                int count = this.allRows != null ? this.allRows.Count : 0;
                return Math.Max(1, (count + maxRows - 1) / maxRows);
            }
        }

        private void BindPage()
        {
            try
            {
                int maxRows = Math.Max(1, cfg.MaxRows);
                List<WaitingPatientADO> rows = (this.allRows ?? new List<WaitingPatientADO>()).Skip(this.pageIndex * maxRows).Take(maxRows).ToList();
                gridControl.BeginUpdate();
                gridControl.DataSource = rows;
                gridControl.EndUpdate();
                lblPage.Text = PageCount > 1 ? string.Format("Trang {0}/{1}", this.pageIndex + 1, PageCount) : "";
                LayoutBottom();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timerPage_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PageCount <= 1)
                {
                    if (this.pageIndex != 0) { this.pageIndex = 0; BindPage(); }
                    return;
                }
                this.pageIndex = (this.pageIndex + 1) % PageCount;
                BindPage();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                WaitingPatientADO row = gridView.GetRow(e.RowHandle) as WaitingPatientADO;
                if (row == null) return;
                if (!string.IsNullOrEmpty(this.currentCallTreatmentCode) && row.TREATMENT_CODE == this.currentCallTreatmentCode)
                {
                    e.Appearance.BackColor = HIGHLIGHT_BACK;
                    e.Appearance.ForeColor = HIGHLIGHT_FORE;
                    e.HighPriority = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Nháy dòng đang gọi

        private void StartBlink()
        {
            try
            {
                this.blinkCount = 0;
                timerBlink.Stop();
                timerBlink.Interval = 400;
                timerBlink.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerBlink_Tick(object sender, EventArgs e)
        {
            try
            {
                this.blinkCount++;
                RenderCalling(this.blinkCount % 2 == 1);
                if (this.blinkCount >= 8)
                {
                    timerBlink.Stop();
                    RenderCalling(false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        private void frmWaitingScreenCashierV3_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerReload.Stop();
                timerPage.Stop();
                timerClock.Stop();
                timerBlink.Stop();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
