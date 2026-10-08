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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientDrugStoreCashier.ADO;
using HIS.Desktop.Plugins.CallPatientDrugStoreCashier.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier
{
    public partial class frmCashierWaitingScreen : FormBase
    {
        static readonly Color BACKGROUND_COLOR = Color.FromArgb(230, 243, 236);
        static readonly Color HEADER_LINE_COLOR = Color.FromArgb(178, 216, 194);
        static readonly Color SUB_TITLE_COLOR = Color.FromArgb(31, 45, 61);
        static readonly Color STT_COLOR = Color.FromArgb(46, 125, 90);
        static readonly Color ROW_COLOR = Color.White;
        static readonly Color ROW_ALT_COLOR = Color.FromArgb(234, 237, 240);
        static readonly Color BILLED_COLOR = Color.FromArgb(67, 160, 71);
        static readonly Color NOT_BILLED_COLOR = Color.FromArgb(240, 108, 62);
        static readonly Color CALLING_LABEL_COLOR = Color.FromArgb(255, 228, 92);
        static readonly CultureInfo VI_CULTURE = new CultureInfo("vi-VN");

        const string FONT_FAMILY = "Arial";
        const string HIDDEN_AMOUNT = "***** đ";
        const string CONFIG_KEY__TIMER_RELOAD = "EXE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS";
        const int DEFAULT_RELOAD_SECONDS = 10;
        const int BLINK_COUNT = 8;

        CashierScreenConfigADO config;
        V_HIS_MEDI_STOCK mediStock;
        DelegateSelectData callingPatientHandler;
        bool isLoadingData;

        string organizationName;
        Image organizationLogo;

        string callingName;
        string callingYear;
        decimal? callingAmount;
        int blinkCount;
        bool isBlinkOn;

        Color titleColor;
        Color listColor;
        Color callingColor;
        Color callingBackColor;
        int headerTopHeight;
        Font fontTitle;
        Font fontBigTitle;
        Font fontSubTitle;
        Font fontSubTitleSmall;
        Font fontHeader;
        Font fontList;
        Font fontListBold;
        Font fontBadge;
        Font fontCalling;
        Font fontCallingLabel;
        Font fontInvite;

        public frmCashierWaitingScreen(CashierScreenConfigADO config, V_HIS_MEDI_STOCK mediStock)
            : base()
        {
            InitializeComponent();
            this.config = config ?? CashierScreenConfigADO.CreateDefault();
            this.config.FillDefault();
            this.mediStock = mediStock;
            WaitingScreenDrawHelper.EnableDoubleBuffer(this.panelHeader);
            WaitingScreenDrawHelper.EnableDoubleBuffer(this.panelCalling);
        }

        private void frmCashierWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                ApplyConfig();
                LoadOrganizationInfo();
                RegisterCallingPatient();
                LoadDataAsync();

                timerReload.Interval = GetReloadInterval();
                timerReload.Start();
                timerScroll.Start();
                timerBlink.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private int GetReloadInterval()
        {
            int seconds = 0;
            try
            {
                seconds = HisConfigs.Get<int>(CONFIG_KEY__TIMER_RELOAD);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            if (seconds <= 0) seconds = DEFAULT_RELOAD_SECONDS;
            return Math.Max(3, seconds) * 1000;
        }

        #region Config & header
        private void ApplyConfig()
        {
            this.titleColor = WaitingScreenDrawHelper.ParseColor(this.config.TitleColor, Color.FromArgb(19, 128, 74));
            this.listColor = WaitingScreenDrawHelper.ParseColor(this.config.ListColor, SUB_TITLE_COLOR);
            this.callingColor = WaitingScreenDrawHelper.ParseColor(this.config.CallingColor, Color.White);
            this.callingBackColor = WaitingScreenDrawHelper.ParseColor(this.config.CallingBackColor, Color.FromArgb(242, 107, 33));

            this.fontTitle = new Font(FONT_FAMILY, this.config.TitleFontSize, FontStyle.Bold);
            this.fontBigTitle = new Font(FONT_FAMILY, Math.Max(8f, this.config.TitleFontSize * 0.95f), FontStyle.Bold);
            this.fontSubTitle = new Font(FONT_FAMILY, Math.Max(8f, this.config.TitleFontSize * 0.72f), FontStyle.Bold);
            this.fontSubTitleSmall = new Font(FONT_FAMILY, Math.Max(8f, this.config.TitleFontSize * 0.5f), FontStyle.Regular);
            this.fontHeader = new Font(FONT_FAMILY, this.config.ListFontSize, FontStyle.Bold);
            this.fontList = new Font(FONT_FAMILY, this.config.ListFontSize, FontStyle.Regular);
            this.fontListBold = new Font(FONT_FAMILY, this.config.ListFontSize, FontStyle.Bold);
            this.fontBadge = new Font(FONT_FAMILY, Math.Max(8f, this.config.ListFontSize * 0.8f), FontStyle.Bold);
            this.fontCalling = new Font(FONT_FAMILY, this.config.CallingFontSize, FontStyle.Bold);
            this.fontCallingLabel = new Font(FONT_FAMILY, this.config.CallingFontSize, FontStyle.Bold);
            this.fontInvite = new Font(FONT_FAMILY, Math.Max(8f, this.config.CallingFontSize * 0.75f), FontStyle.Bold);

            this.headerTopHeight = (int)Math.Max(80, this.fontTitle.GetHeight() * 2.6f);
            this.panelHeader.Height = this.headerTopHeight + (int)(this.fontBigTitle.GetHeight() * 2f);
            this.panelCalling.Height = (int)Math.Max(56, this.fontCalling.GetHeight() * 2.3f);
            this.gridViewPatient.RowHeight = (int)Math.Max(36, this.fontList.GetHeight() * 2.1f);
            this.gridViewPatient.ColumnPanelRowHeight = (int)Math.Max(28, this.fontHeader.GetHeight() * 1.6f);
        }

        private void LoadOrganizationInfo()
        {
            try
            {
                this.organizationName = HisMediOrgCFG.ORGANIZATION_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            // Logo is downloaded from FSS, load it in background so the screen does not freeze
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Image logo = HisMediOrgCFG.ORGANIZATION_LOGO;
                    if (logo != null && !this.IsDisposed && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.organizationLogo = logo;
                            this.panelHeader.Invalidate();
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            });
        }

        private void panelHeader_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                WaitingScreenDrawHelper.PrepareGraphics(g);
                Rectangle full = this.panelHeader.ClientRectangle;
                if (full.Width <= 0 || full.Height <= 0) return;
                using (SolidBrush bandBrush = new SolidBrush(BACKGROUND_COLOR))
                {
                    g.FillRectangle(bandBrush, full);
                }
                Rectangle r = new Rectangle(full.X, full.Y, full.Width, Math.Min(full.Height, this.headerTopHeight));

                using (LinearGradientBrush brush = new LinearGradientBrush(r, Color.White, BACKGROUND_COLOR, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, r);
                }
                using (Pen pen = new Pen(HEADER_LINE_COLOR, 2f))
                {
                    g.DrawLine(pen, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
                }

                float pad = 24f;
                float x = pad;
                if (this.organizationLogo != null)
                {
                    float logoSize = r.Height - 16f;
                    float ratio = Math.Min(logoSize / this.organizationLogo.Width, logoSize / this.organizationLogo.Height);
                    float w = this.organizationLogo.Width * ratio;
                    float h = this.organizationLogo.Height * ratio;
                    g.DrawImage(this.organizationLogo, x, (r.Height - h) / 2f, w, h);
                    x += w + 14f;
                }

                SizeF titleSize = g.MeasureString(this.textScreenTitle, this.fontSubTitle);
                SizeF nameSize = g.MeasureString(this.textScreenName, this.fontSubTitleSmall);
                float rightWidth = Math.Max(titleSize.Width, nameSize.Width);
                float rightX = r.Right - pad - rightWidth;
                float rightY = (r.Height - titleSize.Height - nameSize.Height) / 2f;
                WaitingScreenDrawHelper.DrawText(g, this.textScreenTitle, this.fontSubTitle, SUB_TITLE_COLOR, new RectangleF(rightX, rightY, rightWidth, titleSize.Height), StringAlignment.Far);
                WaitingScreenDrawHelper.DrawText(g, this.textScreenName, this.fontSubTitleSmall, SUB_TITLE_COLOR, new RectangleF(rightX, rightY + titleSize.Height, rightWidth, nameSize.Height), StringAlignment.Far);

                if (!String.IsNullOrEmpty(this.organizationName))
                {
                    WaitingScreenDrawHelper.DrawText(g, this.organizationName.ToUpper(), this.fontTitle, this.titleColor, new RectangleF(x, 0, Math.Max(0, rightX - 16f - x), r.Height), StringAlignment.Near);
                }

                // Title band above the list: "<title> - <room name>"
                string bigTitle = this.textBigTitle;
                if (this.mediStock != null && !String.IsNullOrEmpty(this.mediStock.MEDI_STOCK_NAME))
                {
                    bigTitle += " - " + this.mediStock.MEDI_STOCK_NAME.ToUpper();
                }
                WaitingScreenDrawHelper.DrawText(g, bigTitle, this.fontBigTitle, SUB_TITLE_COLOR, new RectangleF(full.X + pad, r.Bottom, full.Width - pad * 2, full.Height - r.Height), StringAlignment.Center);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void panel_Resize(object sender, EventArgs e)
        {
            try
            {
                Control control = sender as Control;
                if (control != null) control.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Data
        private void timerReload_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataAsync()
        {
            if (this.isLoadingData || this.mediStock == null) return;
            this.isLoadingData = true;
            Task.Factory.StartNew(() =>
            {
                CommonParam param = new CommonParam();
                List<CashierRowADO> rows = null;
                try
                {
                    rows = GetData(param);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                try
                {
                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new MethodInvoker(() => BindData(rows, param)));
                    }
                    else
                    {
                        this.isLoadingData = false;
                    }
                }
                catch (Exception ex)
                {
                    this.isLoadingData = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            });
        }

        private List<CashierRowADO> GetData(CommonParam param)
        {
            // Data source: sale exports of the current pharmacy created today
            HisExpMestFilter filter = new HisExpMestFilter();
            filter.EXP_MEST_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BAN;
            filter.MEDI_STOCK_ID = this.mediStock.ID;
            filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.StartDay() ?? 0).ToString());
            filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64((Inventec.Common.DateTime.Get.EndDay() ?? 0).ToString());
            filter.IS_NOT_TAKEN = false;
            // Payment status follows the bill (receipt) state of the export
            if (this.config.BillFilter == CashierScreenConfigADO.BILL_FILTER__NOT_BILLED)
            {
                filter.HAS_BILL_ID = false;
            }
            else if (this.config.BillFilter == CashierScreenConfigADO.BILL_FILTER__BILLED)
            {
                filter.HAS_BILL_ID = true;
            }
            filter.ORDER_FIELD = "CREATE_TIME";
            filter.ORDER_DIRECTION = "ASC";

            var result = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, filter, param);
            if (result == null) return null;
            return result.Select(o => new CashierRowADO(o)).OrderBy(o => o.SORT_NUMBER).ThenBy(o => o.ID).ToList();
        }

        private void BindData(List<CashierRowADO> rows, CommonParam param)
        {
            try
            {
                if (rows != null)
                {
                    int topRowIndex = this.gridViewPatient.TopRowIndex;
                    this.gridControlPatient.BeginUpdate();
                    this.gridControlPatient.DataSource = rows;
                    this.gridControlPatient.EndUpdate();
                    this.gridViewPatient.TopRowIndex = topRowIndex;
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isLoadingData = false;
            }
        }

        private void timerScroll_Tick(object sender, EventArgs e)
        {
            try
            {
                // Page down when the list is longer than the screen, back to top at the end
                int rowCount = this.gridViewPatient.RowCount;
                if (rowCount == 0) return;
                int lastRowHandle = this.gridViewPatient.GetVisibleRowHandle(rowCount - 1);
                if (this.gridViewPatient.IsRowVisible(lastRowHandle) == RowVisibleState.Visible)
                {
                    if (this.gridViewPatient.TopRowIndex != 0) this.gridViewPatient.TopRowIndex = 0;
                }
                else
                {
                    int rowsPerPage = Math.Max(1, (this.gridControlPatient.Height - this.gridViewPatient.ColumnPanelRowHeight) / Math.Max(1, this.gridViewPatient.RowHeight));
                    this.gridViewPatient.TopRowIndex += rowsPerPage;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string FormatAmount(decimal? amount)
        {
            return (amount ?? 0).ToString("#,##0", VI_CULTURE) + " đ";
        }
        #endregion

        #region Grid draw
        private void gridViewPatient_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                e.Handled = true;
                Graphics g = e.Graphics;
                WaitingScreenDrawHelper.PrepareGraphics(g);
                using (SolidBrush brush = new SolidBrush(BACKGROUND_COLOR))
                {
                    g.FillRectangle(brush, e.Bounds);
                }
                if (e.Column == null) return;
                RectangleF r = e.Bounds;
                StringAlignment alignment = StringAlignment.Center;
                if (e.Column == this.gcPatientName)
                {
                    alignment = StringAlignment.Near;
                    r.X += 22;
                    r.Width -= 22;
                }
                WaitingScreenDrawHelper.DrawText(g, e.Column.GetCaption(), this.fontHeader, this.listColor, r, alignment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPatient_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                e.Handled = true;
                Graphics g = e.Graphics;
                WaitingScreenDrawHelper.PrepareGraphics(g);
                Rectangle cell = e.Bounds;
                using (SolidBrush brush = new SolidBrush(BACKGROUND_COLOR))
                {
                    g.FillRectangle(brush, cell);
                }
                CashierRowADO row = this.gridViewPatient.GetRow(e.RowHandle) as CashierRowADO;
                if (row == null) return;

                if (e.Column == this.gcStt)
                {
                    WaitingScreenDrawHelper.DrawCircleNumber(g, cell, row.STT_DISPLAY, this.fontListBold, STT_COLOR, Color.White);
                    return;
                }

                int visibleIndex = this.gridViewPatient.GetVisibleIndex(e.RowHandle);
                RectangleF card = DrawRowCardSegment(g, cell, e.Column, visibleIndex % 2 == 0 ? ROW_COLOR : ROW_ALT_COLOR);

                if (e.Column == this.gcPatientName)
                {
                    WaitingScreenDrawHelper.DrawText(g, row.PATIENT_NAME, this.fontListBold, this.listColor, new RectangleF(card.X + 16, card.Y, card.Width - 24, card.Height), StringAlignment.Near);
                }
                else if (e.Column == this.gcDobYear)
                {
                    DrawYearCell(g, card, row.DOB_YEAR);
                }
                else if (e.Column == this.gcAmount)
                {
                    DrawAmountCell(g, card, row.AMOUNT);
                }
                else if (e.Column == this.gcStatus)
                {
                    WaitingScreenDrawHelper.DrawBadge(g, Rectangle.Round(card), row.IS_BILLED ? this.textStatusBilled : this.textStatusNotBilled, this.fontBadge, row.IS_BILLED ? BILLED_COLOR : NOT_BILLED_COLOR, Color.White);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Each row is one rounded card from the name column to the status column: first cell rounds the left side, last cell the right side
        /// </summary>
        private RectangleF DrawRowCardSegment(Graphics g, Rectangle cell, DevExpress.XtraGrid.Columns.GridColumn column, Color color)
        {
            bool isFirst = column == this.gcPatientName;
            bool isLast = column == this.gcStatus;
            float vMargin = Math.Max(3f, cell.Height * 0.08f);
            float radius = cell.Height * 0.2f;
            RectangleF card = new RectangleF(cell.X, cell.Y + vMargin, cell.Width, cell.Height - vMargin * 2);
            if (isFirst)
            {
                card.X += 6;
                card.Width -= 6;
            }
            if (isLast)
            {
                card.Width -= 6;
            }
            RectangleF shape = card;
            if (!isFirst)
            {
                shape.X -= radius * 2;
                shape.Width += radius * 2;
            }
            if (!isLast)
            {
                shape.Width += radius * 2;
            }
            // Cells have a ~2px gap between them; overlap neighbours so the row has no seams
            RectangleF clip = card;
            if (!isFirst)
            {
                clip.X -= 2;
                clip.Width += 2;
            }
            if (!isLast)
            {
                clip.Width += 2;
            }
            GraphicsState state = g.Save();
            g.SetClip(clip);
            WaitingScreenDrawHelper.FillRounded(g, shape, radius, color, Color.Empty);
            g.Restore(state);
            return card;
        }

        private void DrawYearCell(Graphics g, RectangleF card, string year)
        {
            if (String.IsNullOrEmpty(year)) return;
            float iconSize = this.fontList.GetHeight() * 0.85f;
            SizeF textSize = g.MeasureString(year, this.fontList);
            float total = iconSize + 8 + textSize.Width;
            float x = card.X + (card.Width - total) / 2f;
            WaitingScreenDrawHelper.DrawCalendarIcon(g, new RectangleF(x, card.Y + (card.Height - iconSize) / 2f, iconSize, iconSize), this.listColor);
            WaitingScreenDrawHelper.DrawText(g, year, this.fontList, this.listColor, new RectangleF(x + iconSize + 8, card.Y, textSize.Width + 4, card.Height), StringAlignment.Near);
        }

        private void DrawAmountCell(Graphics g, RectangleF card, decimal? amount)
        {
            if (this.config.IsHideAmount)
            {
                float iconSize = this.fontList.GetHeight() * 0.85f;
                SizeF textSize = g.MeasureString(HIDDEN_AMOUNT, this.fontListBold);
                float total = textSize.Width + 8 + iconSize;
                float x = card.X + (card.Width - total) / 2f;
                WaitingScreenDrawHelper.DrawText(g, HIDDEN_AMOUNT, this.fontListBold, this.listColor, new RectangleF(x, card.Y, textSize.Width + 4, card.Height), StringAlignment.Near);
                WaitingScreenDrawHelper.DrawEyeSlashIcon(g, new RectangleF(x + textSize.Width + 8, card.Y + (card.Height - iconSize) / 2f, iconSize, iconSize), this.listColor);
            }
            else
            {
                WaitingScreenDrawHelper.DrawText(g, FormatAmount(amount), this.fontListBold, this.listColor, card, StringAlignment.Center);
            }
        }
        #endregion

        #region Calling patient
        /// <summary>
        /// Combine into (not overwrite) the room calling delegate so every open waiting screen receives the speaker button call
        /// </summary>
        private void RegisterCallingPatient()
        {
            if (this.mediStock == null) return;
            this.callingPatientHandler = new DelegateSelectData(this.OnCallingPatient);
            DelegateSelectData existing = null;
            CallPatientDataWorker.DicDelegateCallingPatient.TryGetValue(this.mediStock.ROOM_ID, out existing);
            CallPatientDataWorker.DicDelegateCallingPatient[this.mediStock.ROOM_ID] = (DelegateSelectData)Delegate.Combine(existing, this.callingPatientHandler);
        }

        private void UnregisterCallingPatient()
        {
            try
            {
                if (this.mediStock == null || this.callingPatientHandler == null) return;
                DelegateSelectData existing = null;
                if (CallPatientDataWorker.DicDelegateCallingPatient.TryGetValue(this.mediStock.ROOM_ID, out existing))
                {
                    DelegateSelectData remain = (DelegateSelectData)Delegate.Remove(existing, this.callingPatientHandler);
                    if (remain == null)
                    {
                        CallPatientDataWorker.DicDelegateCallingPatient.Remove(this.mediStock.ROOM_ID);
                    }
                    else
                    {
                        CallPatientDataWorker.DicDelegateCallingPatient[this.mediStock.ROOM_ID] = remain;
                    }
                }
                this.callingPatientHandler = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Raised from the call-patient thread (not the UI thread) when the speaker button is clicked in the export list
        /// </summary>
        private void OnCallingPatient(object data)
        {
            try
            {
                string name = null;
                long? dob = null;
                decimal? amount = null;
                if (data is V_HIS_EXP_MEST_2)
                {
                    V_HIS_EXP_MEST_2 expMest = (V_HIS_EXP_MEST_2)data;
                    name = expMest.TDL_PATIENT_NAME;
                    dob = expMest.TDL_PATIENT_DOB;
                    amount = expMest.TDL_TOTAL_PRICE;
                }
                else if (data is HIS_EXP_MEST)
                {
                    HIS_EXP_MEST expMest = (HIS_EXP_MEST)data;
                    name = expMest.TDL_PATIENT_NAME;
                    dob = expMest.TDL_PATIENT_DOB;
                    amount = expMest.TDL_TOTAL_PRICE;
                }
                else
                {
                    return;
                }
                if (this.IsDisposed || !this.IsHandleCreated) return;
                this.BeginInvoke(new MethodInvoker(() => ShowCallingPatient(name, dob, amount)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowCallingPatient(string name, long? dob, decimal? amount)
        {
            try
            {
                this.callingName = (name ?? "").ToUpper();
                this.callingYear = CashierRowADO.GetYear(dob);
                this.callingAmount = amount;
                this.blinkCount = BLINK_COUNT;
                this.isBlinkOn = true;
                this.panelCalling.Visible = true;
                this.panelCalling.Invalidate();
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerBlink_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.blinkCount <= 0) return;
                this.blinkCount--;
                this.isBlinkOn = this.blinkCount % 2 == 1;
                this.panelCalling.Invalidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string BuildCallingInfo()
        {
            string info = this.callingName;
            if (!String.IsNullOrEmpty(this.callingYear)) info += " - " + this.callingYear;
            if (!this.config.IsHideAmount && this.callingAmount.HasValue) info += " - " + this.textAmountLabel + " " + FormatAmount(this.callingAmount);
            return info;
        }

        private void panelCalling_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                WaitingScreenDrawHelper.PrepareGraphics(g);
                Rectangle r = this.panelCalling.ClientRectangle;
                if (r.Width <= 0 || r.Height <= 0) return;

                using (LinearGradientBrush brush = new LinearGradientBrush(r, this.callingBackColor, ControlPaint.Light(this.callingBackColor, 0.4f), LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(brush, r);
                }
                using (Pen pen = new Pen(Color.FromArgb(90, Color.White), 2f))
                {
                    g.DrawLine(pen, r.Left, r.Top + 1, r.Right, r.Top + 1);
                }

                Color labelColor = this.isBlinkOn ? this.callingColor : CALLING_LABEL_COLOR;
                Color infoColor = this.isBlinkOn ? CALLING_LABEL_COLOR : this.callingColor;
                float pad = 24f;
                float x = pad;

                SizeF labelSize = g.MeasureString(this.textCallingLabel, this.fontCallingLabel);
                WaitingScreenDrawHelper.DrawText(g, this.textCallingLabel, this.fontCallingLabel, labelColor, new RectangleF(x, 0, labelSize.Width, r.Height), StringAlignment.Near);
                x += labelSize.Width + this.fontCallingLabel.Size * 0.3f;

                string invite = (String.IsNullOrWhiteSpace(this.config.InviteText) ? this.textDefaultInvite : this.config.InviteText).ToUpper();
                SizeF inviteSize = g.MeasureString(invite, this.fontInvite);
                float iconSize = this.fontCalling.GetHeight() * 1.1f;
                float inviteX = r.Right - pad - inviteSize.Width;
                float iconX = inviteX - 10f - iconSize;
                WaitingScreenDrawHelper.DrawSpeakerIcon(g, new RectangleF(iconX, (r.Height - iconSize) / 2f, iconSize, iconSize), infoColor);
                WaitingScreenDrawHelper.DrawText(g, invite, this.fontInvite, this.callingColor, new RectangleF(inviteX, 0, inviteSize.Width + 2, r.Height), StringAlignment.Near);

                WaitingScreenDrawHelper.DrawText(g, BuildCallingInfo(), this.fontCalling, infoColor, new RectangleF(x, 0, Math.Max(0, iconX - 16f - x), r.Height), StringAlignment.Near);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void frmCashierWaitingScreen_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCashierWaitingScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                timerReload.Stop();
                timerScroll.Stop();
                timerBlink.Stop();
                UnregisterCallingPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
