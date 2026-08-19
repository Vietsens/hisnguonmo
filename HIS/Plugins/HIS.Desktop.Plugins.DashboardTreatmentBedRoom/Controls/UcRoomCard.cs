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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using MOS.SDO;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// The 1 phong dieu tri: ten phong + danh sach giuong.
    /// Kich thuoc do UcInpatientBoard quyet dinh (luoi 4 cot); phong nao nhieu giuong
    /// thi cuon trong the, khong lam vo bo cuc ben ngoai.
    /// </summary>
    public partial class UcRoomCard : XtraUserControl
    {
        private const int RADIUS = 10;
        private const int HEADER_HEIGHT = 42;
        private const int BODY_PAD = 10;

        // Buong nhieu giuong hon cho nhin thay thi ban to tu troi de lo dan phan con lai.
        // 1px moi 40ms ~ 25px/giay: du cham de doc, du nhanh de khong phai cho lau.
        // Toi dau va cuoi thi dung DWELL_TICK nhip cho nguoi xem kip doc truoc khi doi chieu.
        private const int SCROLL_INTERVAL_MS = 40;
        private const int SCROLL_STEP = 1;
        private const int DWELL_TICK = 45;

        private readonly Timer tmrAutoScroll = new Timer();
        private bool scrollingDown = true;
        private int dwellLeft;

        // Khoa la chuoi vi API co the khong tra ID giuong (xem UcBedCard.GetBedKey)
        private readonly Dictionary<string, UcBedCard> bedCards = new Dictionary<string, UcBedCard>();

        private TreatmentBedRoomDashboardRoomSDO data;
        private bool resizingChildren;

        public event EventHandler<TreatmentBedRoomDashboardBedSDO> BedClicked;

        public UcRoomCard()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            BackColor = BoardTheme.PageBack;

            BoardTheme.EnableDoubleBuffer(flpBeds);
            flpBeds.ClientSizeChanged += FlpBeds_ClientSizeChanged;

            tmrAutoScroll.Interval = SCROLL_INTERVAL_MS;
            tmrAutoScroll.Tick += TmrAutoScroll_Tick;
        }

        public string RoomKey
        {
            get { return data == null ? string.Empty : data.BedRoomId.ToString(); }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            lblRoomName.SetBounds(BODY_PAD, 1, Math.Max(10, Width - BODY_PAD * 2), HEADER_HEIGHT - 2);
            flpBeds.SetBounds(
                BODY_PAD,
                HEADER_HEIGHT,
                Math.Max(10, Width - BODY_PAD * 2),
                Math.Max(10, Height - HEADER_HEIGHT - BODY_PAD));
        }

        private void FlpBeds_ClientSizeChanged(object sender, EventArgs e)
        {
            ResizeBedCards();
            UpdateAutoScroll();
        }

        #region Tu cuon khi qua nhieu giuong
        /// <summary>
        /// Phan noi dung bi tran ra ngoai vung nhin, tinh bang pixel. 0 = vua du cho, khong can cuon.
        /// </summary>
        private int GetScrollRange()
        {
            return Math.Max(0, flpBeds.DisplayRectangle.Height - flpBeds.ClientSize.Height);
        }

        /// <summary>
        /// Bat ban to khi buong co nhieu giuong hon cho nhin thay, tat khi vua du.
        /// Goi lai sau moi lan nap du lieu va moi lan doi kich thuoc.
        /// </summary>
        private void UpdateAutoScroll()
        {
            bool needScroll = GetScrollRange() > 0;

            if (!needScroll)
            {
                tmrAutoScroll.Stop();
                // Ve dau de lan sau con it giuong thi khong bi ket o giua chung
                if (flpBeds.AutoScrollPosition.Y != 0) flpBeds.AutoScrollPosition = Point.Empty;
                return;
            }

            if (!tmrAutoScroll.Enabled)
            {
                scrollingDown = true;
                dwellLeft = DWELL_TICK;
                tmrAutoScroll.Start();
            }
        }

        private void TmrAutoScroll_Tick(object sender, EventArgs e)
        {
            try
            {
                int range = GetScrollRange();
                if (range <= 0)
                {
                    tmrAutoScroll.Stop();
                    return;
                }

                // Dang re chuot vao the thi dung lai, khong ai doc duoc chu dang chay
                if (ClientRectangle.Contains(PointToClient(MousePosition))) return;

                if (dwellLeft > 0)
                {
                    dwellLeft--;
                    return;
                }

                int current = -flpBeds.AutoScrollPosition.Y;
                int next = current + (scrollingDown ? SCROLL_STEP : -SCROLL_STEP);

                // Cham day thi doi chieu chu khong nhay ve dau: nhay lam nguoi xem mat dau
                if (next >= range)
                {
                    next = range;
                    scrollingDown = false;
                    dwellLeft = DWELL_TICK;
                }
                else if (next <= 0)
                {
                    next = 0;
                    scrollingDown = true;
                    dwellLeft = DWELL_TICK;
                }

                flpBeds.AutoScrollPosition = new Point(0, next);
            }
            catch (Exception ex)
            {
                tmrAutoScroll.Stop();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Goi tu Dispose trong file Designer.</summary>
        private void StopAutoScroll()
        {
            if (tmrAutoScroll == null) return;
            tmrAutoScroll.Stop();
            tmrAutoScroll.Tick -= TmrAutoScroll_Tick;
            tmrAutoScroll.Dispose();
        }
        #endregion

        /// <summary>
        /// Giu be rong the giuong bam sat vung nhin cua flpBeds.
        /// Khi thanh cuon doc xuat hien, ClientSize hep lai -> phai co lai, neu khong se sinh cuon ngang.
        /// </summary>
        private void ResizeBedCards()
        {
            if (resizingChildren) return;
            resizingChildren = true;
            try
            {
                int w = flpBeds.ClientSize.Width;
                if (w <= 0) return;

                foreach (Control c in flpBeds.Controls)
                {
                    int wanted = w - c.Margin.Horizontal;
                    if (wanted > 0 && c.Width != wanted) c.Width = wanted;
                }
            }
            finally
            {
                resizingChildren = false;
            }
        }

        /// <summary>
        /// Gan / cap nhat du lieu phong. Chi them - bo - sua tai cho, khong dung lai toan bo the giuong.
        /// </summary>
        public void SetData(TreatmentBedRoomDashboardRoomSDO ado)
        {
            data = ado;
            if (ado == null) return;

            lblRoomName.Text = ado.BedRoomCode;
            List<TreatmentBedRoomDashboardBedSDO> beds = ado.Beds ?? new List<TreatmentBedRoomDashboardBedSDO>();

            flpBeds.SuspendLayout();
            try
            {
                HashSet<string> keep = new HashSet<string>();
                for (int i = 0; i < beds.Count; i++) keep.Add(UcBedCard.GetBedKey(beds[i]));

                List<string> removing = new List<string>();
                foreach (KeyValuePair<string, UcBedCard> pair in bedCards)
                {
                    if (!keep.Contains(pair.Key)) removing.Add(pair.Key);
                }
                for (int i = 0; i < removing.Count; i++)
                {
                    UcBedCard card = bedCards[removing[i]];
                    flpBeds.Controls.Remove(card);
                    card.BedClicked -= Card_BedClicked;
                    card.Dispose();
                    bedCards.Remove(removing[i]);
                }

                for (int i = 0; i < beds.Count; i++)
                {
                    TreatmentBedRoomDashboardBedSDO bed = beds[i];
                    string key = UcBedCard.GetBedKey(bed);
                    UcBedCard card;
                    if (!bedCards.TryGetValue(key, out card))
                    {
                        card = new UcBedCard();
                        card.Margin = new Padding(0, 0, 0, 8);
                        card.BedClicked += Card_BedClicked;
                        bedCards.Add(key, card);
                        flpBeds.Controls.Add(card);
                    }
                    card.SetData(bed);
                    flpBeds.Controls.SetChildIndex(card, i);
                }
            }
            finally
            {
                flpBeds.ResumeLayout(true);
            }

            ResizeBedCards();
            UpdateAutoScroll();
            Invalidate();
        }

        private void Card_BedClicked(object sender, TreatmentBedRoomDashboardBedSDO e)
        {
            if (BedClicked != null) BedClicked(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // base truoc, xem chu thich trong UcBedCard.OnPaint
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            BoardTheme.DrawCard(g, new Rectangle(0, 0, Width, Height), RADIUS,
                BoardTheme.CardBack, BoardTheme.CardBorder);

            using (Pen pen = new Pen(BoardTheme.Separator))
            {
                g.DrawLine(pen, 1, HEADER_HEIGHT - 1, Width - 2, HEADER_HEIGHT - 1);
            }
        }
    }
}
