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
using MOS.SDO;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// The thong tin 1 giuong benh.
    ///
    /// Chia viec ro rang de vua nhin duoc tren Designer vua dep luc chay:
    ///  - Chu nghia la LabelControl that cua DevExpress -> Designer hien ra, keo tha duoc.
    ///  - Nen bo goc, thanh mau cap cham soc, o mau sau sinh hieu thi ve trong OnPaint,
    ///    vi PanelControl cua DevExpress khong bo goc duoc. OnPaint cung chay luc thiet ke
    ///    nen tren Designer van thay dung hinh hai.
    /// </summary>
    public partial class UcBedCard : XtraUserControl
    {
        public const int HEIGHT_WITH_VITAL = 88;
        public const int HEIGHT_NO_VITAL = 72;
        public const int HEIGHT_VITAL_STACKED = 106;

        /// <summary>Duoi be rong nay thi cum sinh hieu chuyen xuong duoi de khong an cho ho ten.</summary>
        private const int COMPACT_WIDTH = 340;

        private const int RADIUS = 8;
        private const int BAR_WIDTH = 4;
        private const int PAD_LEFT = BAR_WIDTH + 12;
        private const int PAD_RIGHT = 12;

        private TreatmentBedRoomDashboardBedSDO data;
        private string signature;
        private bool hovered;

        // Gia tri suy ra, tinh mot lan luc bind. Khong tinh trong OnPaint: parse mau va format so
        // se chay lai moi lan cuon, moi lan re chuot, nhan voi so the dang hien.
        private bool isEmpty = true;
        private bool hasVitalSign;
        private Color? displayColor;

        public event EventHandler<TreatmentBedRoomDashboardBedSDO> BedClicked;

        public UcBedCard()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            // The chi de xem, khong nhan focus -> tranh keo ban to nhay lung tung khi nap lai du lieu
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            BackColor = BoardTheme.CardBack;

            HookChildClicks(this);
        }

        public TreatmentBedRoomDashboardBedSDO Data
        {
            get { return data; }
        }

        /// <summary>
        /// Khoa doi chieu khi lam moi. Uu tien BedId that; API khong tra ID thi roi ve BedCode.
        /// </summary>
        public static string GetBedKey(TreatmentBedRoomDashboardBedSDO sdo)
        {
            if (sdo == null) return string.Empty;
            if (sdo.BedId.HasValue && sdo.BedId.Value != 0) return "#" + sdo.BedId.Value;
            return sdo.BedCode ?? sdo.BedName ?? string.Empty;
        }

        /// <summary>
        /// Bam vao bat cu dau tren the deu tinh la bam vao the.
        /// Khong noi lai thi bam trung chu se khong ra su kien nao.
        /// </summary>
        private void HookChildClicks(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                c.Click += Child_Click;
                c.MouseEnter += Child_MouseEnter;
                c.MouseLeave += Child_MouseLeave;
                if (c.Controls.Count > 0) HookChildClicks(c);
            }
        }

        private void Child_Click(object sender, EventArgs e) { OnClick(EventArgs.Empty); }
        private void Child_MouseEnter(object sender, EventArgs e) { SetHovered(true); }
        private void Child_MouseLeave(object sender, EventArgs e) { SetHovered(ClientRectangle.Contains(PointToClient(MousePosition))); }

        /// <summary>
        /// Gan du lieu. Chi dung vao control khi noi dung thuc su doi, nho vay nap lai theo chu ky
        /// khong sinh handle moi va khong nhay man hinh. 
        /// </summary> 
        public bool SetData(TreatmentBedRoomDashboardBedSDO sdo)
        {
            if (sdo == null) return false;

            string newSignature = BuildSignature(sdo);
            if (string.Equals(signature, newSignature, StringComparison.Ordinal))
            {
                data = sdo;
                return false;
            }

            data = sdo;
            signature = newSignature;

            // Chi coi la co nguoi khi vua dung trang thai vua co khoi treatment.
            // Tin mot trong hai la du de mot ban ghi loi lam vo the (NullReference) 
            // hoac to mau mot giuong dang trong. 
            isEmpty = !(sdo.Treatment != null
                && string.Equals((sdo.Status ?? string.Empty).Trim(), STATUS_OCCUPIED, StringComparison.OrdinalIgnoreCase));

            SuspendLayout();
            try
            {
                if (isEmpty) BindEmpty(sdo);
                else BindOccupied(sdo);
                ApplyDesiredHeight();
            }
            finally
            {
                ResumeLayout(false);
            }

            Invalidate();
            return true;
        }

        private const string STATUS_OCCUPIED = "OCCUPIED";

        /// <summary>
        /// Dau van tay de biet du lieu co thuc su doi hay khong.
        /// THEM TRUONG MOI HIEN THI TREN THE THI PHAI THEM VAO DAY,
        /// khong thi truong do se khong bao gio tu cap nhat.
        /// </summary>
        private static string BuildSignature(TreatmentBedRoomDashboardBedSDO sdo)
        {
            TreatmentBedRoomDashboardTreatmentSDO t = sdo.Treatment;
            if (t == null) return string.Concat(sdo.BedCode, "|", sdo.Status, "|-");

            return string.Join("|", new string[]
            {
                sdo.BedCode, sdo.Status,
                t.Name, t.Age.ToString(), t.Gender, t.Diagnosis, t.DoctorUsername,
                t.CareLevel.ToString(), t.DisplayColor,
                t.Pulse.ToString(), t.Temperature.ToString(), t.BloodPressure
            });
        }

        private void BindEmpty(TreatmentBedRoomDashboardBedSDO sdo)
        {
            hasVitalSign = false;
            displayColor = null;

            SetOccupiedVisible(false);
            SetVitalVisible(false);

            lblEmptyCode.Visible = true;
            lblEmptyText.Visible = true;
            icoEmptyBed.Visible = true;
            lblEmptyCode.Text = string.IsNullOrEmpty(sdo.BedCode) ? sdo.BedName : sdo.BedCode;

            LayoutEmptyRow();
        }

        private void BindOccupied(TreatmentBedRoomDashboardBedSDO sdo)
        {
            TreatmentBedRoomDashboardTreatmentSDO t = sdo.Treatment;

            // InvariantCulture cho nhiet do: may cai vi-VN se ra "36,8" thay vi "36.8"
            string pulse = t.Pulse.HasValue ? t.Pulse.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            string temp = t.Temperature.HasValue
                ? t.Temperature.Value.ToString("0.#", CultureInfo.InvariantCulture)
                : string.Empty;
            string bloodPressure = (t.BloodPressure ?? string.Empty).Trim();

            hasVitalSign = pulse.Length > 0 || temp.Length > 0 || bloodPressure.Length > 0;
            displayColor = BoardTheme.ParseDisplayColor(t.DisplayColor);

            lblEmptyCode.Visible = false;
            lblEmptyText.Visible = false;
            icoEmptyBed.Visible = false;

            SetOccupiedVisible(true);
            SetVitalVisible(hasVitalSign);

            lblBedCode.Text = string.IsNullOrEmpty(sdo.BedCode) ? sdo.BedName : sdo.BedCode;
            lblPatientName.Text = t.Name;
            lblMeta.Text = string.Format("{0} tuổi · {1}", t.Age.HasValue ? t.Age.Value.ToString() : "", t.Gender);
            lblDiagnosis.Text = t.Diagnosis;
            lblDoctor.Text = t.DoctorUsername;

            if (hasVitalSign)
            {
                lblPulse.Text = pulse;
                lblTemp.Text = temp;
                lblBp.Text = bloodPressure;
            }

            RelayoutOccupied();
        }

        private void SetOccupiedVisible(bool visible)
        {
            lblBedCode.Visible = visible;
            lblPatientName.Visible = visible;
            lblMeta.Visible = visible;
            lblDiagnosis.Visible = visible;
            lblDoctor.Visible = visible;
            icoDoctor.Visible = visible;
        }

        private void SetVitalVisible(bool visible)
        {
            icoPulse.Visible = visible;
            lblPulse.Visible = visible;
            icoTemp.Visible = visible;
            lblTemp.Visible = visible;
            icoBp.Visible = visible;
            lblBp.Visible = visible;

            bool caption = visible && !IsCompact;
            lblPulseCaption.Visible = caption;
            lblTempCaption.Visible = caption;
            lblBpCaption.Visible = caption;
        }

        private bool IsCompact
        {
            get { return Width > 0 && Width < COMPACT_WIDTH; }
        }

        private bool HasSideVital
        {
            get { return data != null && !isEmpty && hasVitalSign && !IsCompact; }
        }

        private bool HasStackedVital
        {
            get { return data != null && !isEmpty && hasVitalSign && IsCompact; }
        }

        private void ApplyDesiredHeight()
        {
            int wanted;
            if (data == null || isEmpty || !hasVitalSign) wanted = HEIGHT_NO_VITAL;
            else wanted = IsCompact ? HEIGHT_VITAL_STACKED : HEIGHT_WITH_VITAL;

            if (Height != wanted) Height = wanted;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (data == null) return;

            SetVitalVisible(!isEmpty && hasVitalSign);
            if (isEmpty) LayoutEmptyRow();
            else RelayoutOccupied();

            ApplyDesiredHeight();
        }

        /// <summary>
        /// Dat lai vi tri cac nhan theo be rong hien tai.
        /// Neo Anchor khong du: khi hep, cum sinh hieu phai nhay hang xuong duoi thanh chip,
        /// va chan doan phai an theo cho con lai cua ho ten.
        /// </summary>
        private void RelayoutOccupied()
        {
            bool side = HasSideVital;
            bool stacked = HasStackedVital;

            int textRight = Width - PAD_RIGHT - (side ? VitalBlockWidth + 10 : 0);
            if (textRight < PAD_LEFT + 40) textRight = PAD_LEFT + 40;

            int y1 = side ? 11 : 9;
            int y2 = y1 + 22;
            int y3 = y2 + 21;

            lblBedCode.Location = new Point(PAD_LEFT, y1 + 2);
            int nameX = PAD_LEFT + lblBedCode.Width + 8;
            lblPatientName.SetBounds(nameX, y1, Math.Max(20, textRight - nameX), 18);

            lblMeta.Location = new Point(PAD_LEFT, y2);
            int icdX = PAD_LEFT + lblMeta.Width + 10;
            lblDiagnosis.SetBounds(icdX, y2, Math.Max(20, textRight - icdX), 15);

            icoDoctor.Location = new Point(PAD_LEFT, y3 + 1);
            lblDoctor.SetBounds(PAD_LEFT + 16, y3, Math.Max(20, textRight - PAD_LEFT - 16), 15);

            if (side) LayoutVitalTiles(Width - PAD_RIGHT - VitalBlockWidth, 12, true);
            else if (stacked) LayoutVitalChips(PAD_LEFT, y3 + 24);
        }

        private const int TILE_W = 42;
        private const int TILE_GAP = 5;
        private static int VitalBlockWidth { get { return TILE_W * 3 + TILE_GAP * 2; } }

        private void LayoutVitalTiles(int x, int y, bool withCaption)
        {
            LayoutOneTile(icoPulse, lblPulse, lblPulseCaption, x, y, withCaption);
            LayoutOneTile(icoTemp, lblTemp, lblTempCaption, x + TILE_W + TILE_GAP, y, withCaption);
            LayoutOneTile(icoBp, lblBp, lblBpCaption, x + (TILE_W + TILE_GAP) * 2, y, withCaption);
        }

        private void LayoutOneTile(IconBox icon, LabelControl value, LabelControl caption,
            int x, int y, bool withCaption)
        {
            icon.SetBounds(x + (TILE_W - 12) / 2, y + 5, 12, 12);
            value.SetBounds(x, y + 19, TILE_W, 14);
            if (withCaption) caption.SetBounds(x - 4, y + 42, TILE_W + 8, 22);
        }

        /// <summary>Kieu thu gon: 3 chip nam mot hang duoi cung, bo phan chu thich.</summary>
        private void LayoutVitalChips(int x, int y)
        {
            x = LayoutOneChip(icoPulse, lblPulse, x, y);
            x = LayoutOneChip(icoTemp, lblTemp, x, y);
            LayoutOneChip(icoBp, lblBp, x, y);
        }

        private int LayoutOneChip(IconBox icon, LabelControl value, int x, int y)
        {
            int textW = TextRenderer.MeasureText(value.Text, value.Appearance.Font).Width;
            if (textW < 18) textW = 18;

            icon.SetBounds(x + 6, y + 5, 11, 11);
            value.SetBounds(x + 21, y + 4, textW + 2, 14);

            return x + 6 + 11 + 4 + textW + 7 + 5;
        }

        private void LayoutEmptyRow()
        {
            int codeW = lblEmptyCode.Width;
            int textW = TextRenderer.MeasureText(lblEmptyText.Text, lblEmptyText.Appearance.Font).Width;
            int total = codeW + 10 + 14 + 6 + textW;

            int x = (Width - total) / 2;
            int midY = Height / 2;

            lblEmptyCode.SetBounds(x, midY - 7, codeW, 14);
            icoEmptyBed.SetBounds(x + codeW + 10, midY - 7, 14, 14);
            lblEmptyText.SetBounds(x + codeW + 10 + 14 + 6, midY - 8, textW + 4, 16);
        }

        private void SetHovered(bool value)
        {
            if (hovered == value) return;
            hovered = value;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); SetHovered(true); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); SetHovered(ClientRectangle.Contains(PointToClient(MousePosition))); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (BedClicked != null && data != null) BedClicked(this, data);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // base PHAI goi truoc: XtraUserControl to nen theo skin trong OnPaint,
            // goi sau thi no xoa sach thanh mau va o sinh hieu vua ve, the tro lai trang tron.
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(0, 0, Width, Height);

            if (data == null || isEmpty) PaintEmptyBackground(g, r);
            else PaintOccupiedBackground(g, r);
        }

        private void PaintEmptyBackground(Graphics g, Rectangle r)
        {
            Rectangle rr = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
            using (GraphicsPath path = BoardTheme.RoundedRect(rr, RADIUS))
            {
                using (SolidBrush back = new SolidBrush(BoardTheme.EmptyBack)) g.FillPath(back, path);

                // Nen soc cheo dac trung cua giuong trong
                using (HatchBrush hatch = new HatchBrush(HatchStyle.LightUpwardDiagonal,
                    BoardTheme.EmptyHatch, Color.Transparent))
                {
                    Region old = g.Clip;
                    g.SetClip(path);
                    g.FillRectangle(hatch, rr);
                    g.Clip = old;
                }

                using (Pen pen = new Pen(BoardTheme.EmptyBorder))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawPath(pen, path);
                }
            }
        }

        private void PaintOccupiedBackground(Graphics g, Rectangle r)
        {
            // Mau da parse san luc bind, o day chi lay ra dung
            Color levelColor = displayColor.HasValue ? displayColor.Value : BoardTheme.LevelNone;
            Color backColor = displayColor.HasValue
                ? BoardTheme.Lighten(levelColor, 0.93)
                : BoardTheme.CardBack;

            Color borderColor = hovered ? Color.FromArgb(120, levelColor) : BoardTheme.CardBorder;
            BoardTheme.DrawCard(g, r, RADIUS, backColor, borderColor);

            Rectangle rr = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
            using (GraphicsPath path = BoardTheme.RoundedRect(rr, RADIUS))
            {
                Region old = g.Clip;
                g.SetClip(path);
                using (SolidBrush brush = new SolidBrush(levelColor))
                {
                    g.FillRectangle(brush, r.X, r.Y, BAR_WIDTH, r.Height);
                }
                g.Clip = old;
            }

            if (icoPulse.Visible) PaintVitalBackgrounds(g);
        }

        /// <summary>
        /// O mau sau moi cum sinh hieu. Kich thuoc suy tu vi tri icon va nhan gia tri
        /// nen keo hai control do tren Designer thi o mau tu chay theo.
        /// </summary>
        private void PaintVitalBackgrounds(Graphics g)
        {
            PaintOneVitalBackground(g, icoPulse, lblPulse, BoardTheme.PulseBack);
            PaintOneVitalBackground(g, icoTemp, lblTemp, BoardTheme.TempBack);
            PaintOneVitalBackground(g, icoBp, lblBp, BoardTheme.BpBack);
        }

        private void PaintOneVitalBackground(Graphics g, IconBox icon, LabelControl value, Color back)
        {
            Rectangle box = Rectangle.Union(icon.Bounds, value.Bounds);
            box.Inflate(IsCompact ? 6 : 4, IsCompact ? 4 : 5);
            BoardTheme.FillRounded(g, box, IsCompact ? 6 : 7, back);
        }
    }
}
