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
        // Buoc da la 1px - nho nhat co the - nen muon cham hon chi con cach gian nhip.
        // 1px moi 80ms ~ 12px/giay.
        private const int SCROLL_INTERVAL_MS = 80;
        private const int SCROLL_STEP = 1;

        /// <summary>
        /// Thoi gian dung o dau va cuoi, tinh bang mili giay.
        /// Ghi theo mili giay roi chia ra so nhip, khong ghi thang so nhip: doi SCROLL_INTERVAL_MS
        /// ma so nhip de nguyen thi thoi gian dung am tham doi theo, rat de nham.
        /// </summary>
        private const int DWELL_MS = 2500;
        private const int DWELL_TICK = DWELL_MS / SCROLL_INTERVAL_MS;

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

                // Treo layout quanh ca vong lap: khong treo thi FlowLayoutPanel sap xep lai
                // sau MOI the giuong doi be rong, voi mot buong 6 giuong la 6 luot thua
                flpBeds.SuspendLayout();
                try
                {
                    foreach (Control c in flpBeds.Controls)
                    {
                        int wanted = w - c.Margin.Horizontal;
                        if (wanted > 0 && c.Width != wanted) c.Width = wanted;
                    }
                }
                finally
                {
                    flpBeds.ResumeLayout(true);
                }
            }
            finally
            {
                resizingChildren = false;
            }
        }

        /// <summary>
        /// Sinh khoa doi chieu cho tung giuong, dam bao KHONG TRUNG NHAU.
        ///
        /// Vi sao phai lam: khoa uu tien BedId, thieu thi roi ve BedCode. API tra ve BedId rong
        /// va BedCode trung nhau (hoac cung rong) thi nhieu giuong ra cung mot khoa. Luc do
        /// bedCards.Add nem ArgumentException ngay giua vong lap, cac giuong con lai khong duoc
        /// dung the nao ca - API bao 9 giuong ma tren man chi hien 2.
        ///
        /// Trung thi noi them so thu tu vao sau, va ghi log de con biet duong sua du lieu goc.
        /// </summary>
        private List<string> BuildUniqueBedKeys(
            TreatmentBedRoomDashboardRoomSDO room, List<TreatmentBedRoomDashboardBedSDO> beds)
        {
            List<string> keys = new List<string>(beds.Count);
            HashSet<string> used = new HashSet<string>();
            int duplicated = 0;
            int missing = 0;

            for (int i = 0; i < beds.Count; i++)
            {
                string key = UcBedCard.GetBedKey(beds[i]);

                if (string.IsNullOrEmpty(key))
                {
                    key = "idx" + i;
                    missing++;
                }
                else if (used.Contains(key))
                {
                    key = key + "#" + i;
                    duplicated++;
                }

                used.Add(key);
                keys.Add(key);
            }

            if (missing > 0 || duplicated > 0)
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "Buong {0}: {1} giuong khong co ca BedId lan BedCode, {2} giuong trung khoa. "
                    + "Da tu sinh khoa de khong mat the, nhung nen sua du lieu goc.",
                    room == null ? "?" : room.BedRoomCode, missing, duplicated));
            }

            return keys;
        }

        /// <summary>
        /// Sap giuong theo thu tu hien thi: cap cham soc truoc, khong co cap thi xuong sau,
        /// giuong trong xuong cuoi cung.
        ///
        /// Sap theo VI TRI cua cap trong danh muc chu khong theo CARE_LEVEL_ID: ID chi la so
        /// dinh danh, sap theo no thi Cap I co the nam duoi Cap III. Vi tri do UcInpatientBoard
        /// truyen xuong, dung chung nguon voi cum thong ke.
        ///
        /// Dung OrderBy chu khong List.Sort: OrderBy on dinh, cac giuong cung nhom giu nguyen
        /// thu tu API tra ve nen khong bi xao tron vo co giua cac lan lam moi.
        /// </summary>
        private List<TreatmentBedRoomDashboardBedSDO> SortBeds(
            List<TreatmentBedRoomDashboardBedSDO> beds, Dictionary<long, int> careLevelOrder)
        {
            if (beds == null) return new List<TreatmentBedRoomDashboardBedSDO>();

            return beds.OrderBy(o => GetSortRank(o, careLevelOrder)).ToList();
        }

        private const int RANK_UNKNOWN_LEVEL = 1000000;
        private const int RANK_NO_LEVEL = 2000000;
        private const int RANK_EMPTY_BED = 3000000;

        private int GetSortRank(TreatmentBedRoomDashboardBedSDO bed, Dictionary<long, int> careLevelOrder)
        {
            if (bed == null || bed.Treatment == null) return RANK_EMPTY_BED;
            if (!bed.Treatment.CareLevel.HasValue) return RANK_NO_LEVEL;

            int order;
            if (careLevelOrder != null && careLevelOrder.TryGetValue(bed.Treatment.CareLevel.Value, out order))
            {
                return order;
            }

            // Co CARE_LEVEL_ID nhung khong tim thay trong danh muc (cap da ngung hoat dong chang han):
            // van xep tren giuong khong co cap, duoi cac cap con hieu luc
            return RANK_UNKNOWN_LEVEL;
        }

        /// <summary>
        /// Gan / cap nhat du lieu phong. Chi them - bo - sua tai cho, khong dung lai toan bo the giuong.
        /// </summary>
        public void SetData(TreatmentBedRoomDashboardRoomSDO ado, Dictionary<long, int> careLevelOrder)
        {
            data = ado;
            if (ado == null) return;

            lblRoomName.Text = ado.BedRoomCode;
            List<TreatmentBedRoomDashboardBedSDO> beds = SortBeds(ado.Beds, careLevelOrder);

            List<string> bedKeys = BuildUniqueBedKeys(ado, beds);

            flpBeds.SuspendLayout();
            try
            {
                HashSet<string> keep = new HashSet<string>();
                for (int i = 0; i < beds.Count; i++) keep.Add(bedKeys[i]);

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
                    string key = bedKeys[i];
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

                if (bedCards.Count != beds.Count)
                {
                    Inventec.Common.Logging.LogSystem.Warn(string.Format(
                        "Buong {0}: API tra {1} giuong nhung chi dung duoc {2} the",
                        ado.BedRoomCode, beds.Count, bedCards.Count));
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
