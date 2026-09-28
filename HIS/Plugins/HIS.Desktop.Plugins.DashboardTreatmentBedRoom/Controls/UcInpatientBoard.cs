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
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// Man hinh bang dien tu noi tru.
    /// Luoi phong N cot, vung nhin cao dung 2 hang. Nhieu hon 2 hang thi chia trang, moi trang
    /// 2 hang, tu lat sang trang sau theo PageIntervalSecond roi quay vong ve trang dau -
    /// bang treo tuong khong ai cam chuot de cuon.
    /// </summary>
    public partial class UcInpatientBoard : XtraUserControl
    {
        /// <summary>So cot mac dinh khi nguoi dung khong chon.</summary>
        public const int DEFAULT_COLUMN_COUNT = 4;

        private int columnCount = DEFAULT_COLUMN_COUNT;
        private const int VISIBLE_ROW_COUNT = 2;
        private const int GAP = 12;
        private const int MIN_CARD_WIDTH = 260;
        private const int MIN_CARD_HEIGHT = 200;

        // Khoa doi chieu la BedRoomId cua buong
        private readonly Dictionary<string, UcRoomCard> roomCards = new Dictionary<string, UcRoomCard>();
        private readonly List<UcRoomCard> orderedCards = new List<UcRoomCard>();

        /// <summary>Trang dang hien, dem tu 0.</summary>
        private int pageIndex;

        /// <summary>Ban to yeu cau nap lai du lieu. Nguoi dung control bat su kien nay de goi API.</summary>
        public event EventHandler DataRefreshRequested;

        public event EventHandler<TreatmentBedRoomDashboardBedSDO> BedClicked;

        public UcInpatientBoard()
        {
            InitializeComponent();

            BoardTheme.EnableDoubleBuffer(pnlScroll);
            BoardTheme.EnableDoubleBuffer(pnlCanvas);
        }

        /// <summary>
        /// So cot cua luoi buong. Gan gia tri khong hop le se bi bo qua - noi chot gia tri la
        /// frmDashboard, o day chi tu bao ve de mot con so la khong lam vo bo cuc.
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
            set
            {
                if (value < 1) return;
                if (columnCount == value) return;

                columnCount = value;
                pageIndex = 0;
                RelayoutRooms();
            }
        }

        /// <summary>Chu ky lat trang, tinh bang giay. 0 = khong lat, chi hien trang dau.</summary>
        public int PageIntervalSecond
        {
            get { return tmrPage.Enabled ? tmrPage.Interval / 1000 : 0; }
            set
            {
                if (value <= 0)
                {
                    tmrPage.Stop();
                    return;
                }
                tmrPage.Interval = value * 1000;
                tmrPage.Start();
            }
        }

        private int PageSize
        {
            get { return columnCount * VISIBLE_ROW_COUNT; }
        }

        private int PageCount
        {
            get { return Math.Max(1, (orderedCards.Count + PageSize - 1) / PageSize); }
        }

        /// <summary>
        /// Het trang thi quay ve trang dau. Chi co 1 trang thi khong lam gi, de khoi
        /// sap xep lai the phong vo ich moi nhip.
        /// </summary>
        private void TmrPage_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PageCount <= 1)
                {
                    pageIndex = 0;
                    return;
                }

                GoToPage(pageIndex + 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            TurnPageByHand(-1);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            TurnPageByHand(1);
        }

        /// <summary>
        /// Nguoi dung bam lat tay: sang trang roi dem lai tu dau chu ky. Khong dem lai thi
        /// vua bam xong co the bi tu lat tiep ngay giua chung, chua kip doc trang vua chon.
        /// </summary>
        private void TurnPageByHand(int step)
        {
            try
            {
                if (PageCount <= 1) return;

                GoToPage(pageIndex + step);

                if (tmrPage.Enabled)
                {
                    tmrPage.Stop();
                    tmrPage.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Sang trang bat ky, vuot dau/cuoi thi quay vong.</summary>
        private void GoToPage(int index)
        {
            int count = PageCount;
            pageIndex = ((index % count) + count) % count;
            RelayoutRooms();

            // Sang trang moi thi ve dau, dung giu vi tri cuon cua trang truoc
            if (pnlScroll.AutoScrollPosition != Point.Empty) pnlScroll.AutoScrollPosition = Point.Empty;
        }

        private void UpdatePageInfo()
        {
            int count = PageCount;
            lblPageInfo.Text = string.Format("Trang {0}/{1}", pageIndex + 1, count);
            btnPrevPage.Enabled = count > 1;
            btnNextPage.Enabled = count > 1;
        }

        /// <summary>Chu ky tu nap lai, tinh bang giay. 0 = tat.</summary>
        public int RefreshIntervalSecond
        {
            get { return tmrRefresh.Interval / 1000; }
            set
            {
                if (value <= 0)
                {
                    tmrRefresh.Stop();
                    return;
                }
                tmrRefresh.Interval = value * 1000;
                tmrRefresh.Start();
            }
        }

        private void TmrRefresh_Tick(object sender, EventArgs e)
        {
            if (DataRefreshRequested != null) DataRefreshRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Nap / cap nhat du lieu. Chi them - bo - sua the tai cho nen goi lai lien tuc van khong nhay
        /// va khong mat vi tri dang cuon.
        /// </summary>
        /// <summary>
        /// Sinh khoa doi chieu cho tung buong, dam bao KHONG TRUNG NHAU.
        /// Cung ly do nhu ben UcRoomCard: BedRoomId bang 0 hoac trung nhau thi Dictionary.Add
        /// nem loi giua vong lap, cac buong sau khong duoc dung the nao ca.
        /// </summary>
        private List<string> BuildUniqueRoomKeys(List<TreatmentBedRoomDashboardRoomSDO> rooms)
        {
            List<string> keys = new List<string>(rooms.Count);
            HashSet<string> used = new HashSet<string>();
            int fixedUp = 0;

            for (int i = 0; i < rooms.Count; i++)
            {
                TreatmentBedRoomDashboardRoomSDO room = rooms[i];
                string key = (room != null && room.BedRoomId != 0)
                    ? room.BedRoomId.ToString()
                    : ((room == null ? null : room.BedRoomCode) ?? string.Empty);

                if (string.IsNullOrEmpty(key) || used.Contains(key))
                {
                    key = key + "#" + i;
                    fixedUp++;
                }

                used.Add(key);
                keys.Add(key);
            }

            if (fixedUp > 0)
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "{0} buong khong co BedRoomId hoac bi trung khoa. Da tu sinh khoa de khong mat the.",
                    fixedUp));
            }

            return keys;
        }

        public void SetData(HisTreatmentBedRoomDashboardSDO sdo)
        {
            if (sdo == null) return;

            List<CareLevelSummaryADO> careLevels = BuildCareLevels(sdo);
            ucSummary.SetData(sdo, careLevels);

            // Thu tu sap giuong lay tu chinh danh sach vua dung cho cum thong ke.
            // Dung chung mot nguon nen thu tu o cum "Che do cham soc" va thu tu giuong trong
            // buong luon khop nhau; tach ra hai cho tinh rieng la som muon cung lech.
            Dictionary<long, int> careLevelOrder = new Dictionary<long, int>();
            for (int i = 0; i < careLevels.Count; i++)
            {
                if (!careLevelOrder.ContainsKey(careLevels[i].CARE_LEVEL_ID))
                {
                    careLevelOrder.Add(careLevels[i].CARE_LEVEL_ID, i);
                }
            }

            List<TreatmentBedRoomDashboardRoomSDO> rooms = sdo.Rooms ?? new List<TreatmentBedRoomDashboardRoomSDO>();
            List<string> roomKeys = BuildUniqueRoomKeys(rooms);

            pnlCanvas.SuspendLayout();
            try
            {
                HashSet<string> keep = new HashSet<string>();
                for (int i = 0; i < rooms.Count; i++) keep.Add(roomKeys[i]);

                List<string> removing = new List<string>();
                foreach (KeyValuePair<string, UcRoomCard> pair in roomCards)
                {
                    if (!keep.Contains(pair.Key)) removing.Add(pair.Key);
                }
                for (int i = 0; i < removing.Count; i++)
                {
                    UcRoomCard card = roomCards[removing[i]];
                    pnlCanvas.Controls.Remove(card);
                    card.BedClicked -= Card_BedClicked;
                    card.Dispose();
                    roomCards.Remove(removing[i]);
                }

                orderedCards.Clear();
                for (int i = 0; i < rooms.Count; i++)
                {
                    TreatmentBedRoomDashboardRoomSDO room = rooms[i];
                    string key = roomKeys[i];
                    UcRoomCard card;
                    if (!roomCards.TryGetValue(key, out card))
                    {
                        card = new UcRoomCard();
                        card.BedClicked += Card_BedClicked;

                        // Dung the giuong TRUOC khi gan the buong vao pnlCanvas.
                        // Control chua co cha thi chua tao handle cua so, moi thao tac gan nhu
                        // mien phi; gan vao cha roi moi dung thi tung the giuong tao handle rieng
                        // va bat cha sap xep lai - do la phan ton nhat cua ca qua trinh.
                        card.SetData(room, careLevelOrder);

                        roomCards.Add(key, card);
                        pnlCanvas.Controls.Add(card);
                    }
                    else
                    {
                        card.SetData(room, careLevelOrder);
                    }
                    orderedCards.Add(card);
                }
            }
            finally
            {
                pnlCanvas.ResumeLayout(false);
            }

            // Lam moi giu nguyen trang dang xem; bot buong lam mat trang do thi ve trang dau
            if (pageIndex >= PageCount) pageIndex = 0;

            RelayoutRooms();
        }

        #region Che do cham soc
        /// <summary>
        /// Dung danh sach o cho cum "Che do cham soc".
        ///
        /// So o bang dung so ban ghi HIS_CARE_LEVEL dang hoat dong, KHONG lay theo danh sach
        /// CareLevels cua API: cap nao khong co benh nhan thi API khong tra ve, nhung o van phai
        /// hien voi so 0 chu khong duoc bien mat.
        /// Ten va mau lay tu danh muc, so luong lay tu CareLevels.
        /// </summary>
        private List<CareLevelSummaryADO> BuildCareLevels(HisTreatmentBedRoomDashboardSDO sdo)
        {
            List<CareLevelSummaryADO> result = new List<CareLevelSummaryADO>();
            try
            {
                List<HIS_CARE_LEVEL> catalog = BackendDataWorker.Get<HIS_CARE_LEVEL>();
                if (catalog == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc danh muc HIS_CARE_LEVEL tu cache");
                    return result;
                }

                Dictionary<long, long> totalById = new Dictionary<long, long>();
                if (sdo != null && sdo.CareLevels != null)
                {
                    foreach (TreatmentBedRoomCareLevelSDO item in sdo.CareLevels)
                    {
                        if (item == null || !item.CareLevelId.HasValue) continue;
                        totalById[item.CareLevelId.Value] = item.Total;
                    }
                }

                // HIS_CARE_LEVEL khong co truong thu tu nen sap theo ma cho on dinh giua cac lan lam moi
                List<HIS_CARE_LEVEL> actives = catalog
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.CARE_LEVEL_CODE)
                    .ToList();

                foreach (HIS_CARE_LEVEL level in actives)
                {
                    long total;
                    result.Add(new CareLevelSummaryADO()
                    {
                        CARE_LEVEL_ID = level.ID,
                        CARE_LEVEL_CODE = level.CARE_LEVEL_CODE,
                        CARE_LEVEL_NAME = level.CARE_LEVEL_NAME,
                        DISPLAY_COLOR = BoardTheme.ParseDisplayColor(level.DISPLAY_COLOR),
                        TOTAL = totalById.TryGetValue(level.ID, out total) ? total : 0
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        private void Card_BedClicked(object sender, TreatmentBedRoomDashboardBedSDO e)
        {
            if (BedClicked != null) BedClicked(this, e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            LayoutSections();
        }

        private void LayoutSections()
        {
            int left = Padding.Left;
            int width = Math.Max(10, Width - Padding.Horizontal);

            ucSummary.SetBounds(left, Padding.Top, width, UcSummaryBar.DEFAULT_HEIGHT);

            int top = Padding.Top + UcSummaryBar.DEFAULT_HEIGHT + GAP;

            // Hang nut lat trang nam sat day, can phai
            int footerTop = Height - Padding.Bottom - btnNextPage.Height;
            int right = Width - Padding.Right;
            btnNextPage.Location = new Point(right - btnNextPage.Width, footerTop);
            btnPrevPage.Location = new Point(btnNextPage.Left - GAP - btnPrevPage.Width, footerTop);
            lblPageInfo.SetBounds(btnPrevPage.Left - GAP - lblPageInfo.Width, footerTop, lblPageInfo.Width, btnNextPage.Height);

            // Vung cuon trai ra sat hai mep: le 12px quanh the phong do chinh pnlCanvas lo ra,
            // neu de pnlScroll thut vao nua thi le bi cong don hai lan.
            pnlScroll.SetBounds(0, top, Width, Math.Max(10, footerTop - top));

            RelayoutRooms();
        }

        /// <summary>
        /// Tinh lai kich thuoc va vi tri the phong.
        /// Moc do la kich thuoc NGOAI cua pnlScroll, khong phai ClientSize: ClientSize co lai ngay
        /// khi thanh cuon hien ra, lay no lam moc thi chieu cao the va thanh cuon tinh vong lan nhau
        /// -> lo mot phan hang thu ba o day man hinh.
        ///
        /// Chi xep the cua trang dang hien, the cac trang khac an di. Trang cuoi it hang hon
        /// thi cac hang nam tu tren xuong, chieu cao the van nhu trang day du.
        /// </summary>
        private void RelayoutRooms()
        {
            int viewW = pnlScroll.Width;
            int viewH = pnlScroll.Height;

            UpdatePageInfo();

            if (orderedCards.Count == 0)
            {
                pnlCanvas.Size = new Size(Math.Max(10, viewW), 0);
                return;
            }

            int pageStart = pageIndex * PageSize;
            int pageEnd = Math.Min(orderedCards.Count, pageStart + PageSize);
            int rows = (pageEnd - pageStart + columnCount - 1) / columnCount;

            int cardH = (viewH - GAP * (VISIBLE_ROW_COUNT + 1)) / VISIBLE_ROW_COUNT;
            if (cardH < MIN_CARD_HEIGHT) cardH = MIN_CARD_HEIGHT;

            int totalH = GAP + rows * (cardH + GAP);
            if (totalH > viewH) viewW -= SystemInformation.VerticalScrollBarWidth;
            if (viewW < 10) viewW = 10;

            int available = viewW - GAP * (columnCount + 1);
            int cardW = available / columnCount;
            int extra = available - cardW * columnCount;

            int canvasW = viewW;
            if (cardW < MIN_CARD_WIDTH)
            {
                // Cua so qua hep: giu nguyen 4 cot va be rong toi thieu, chap nhan cuon ngang
                // con hon bop the den muc khong doc duoc.
                cardW = MIN_CARD_WIDTH;
                extra = 0;
                canvasW = GAP * (columnCount + 1) + cardW * columnCount;
            }

            pnlCanvas.Size = new Size(canvasW, totalH);

            // Chia phan du cho cac cot dau -> khong bi thua mot dai trang ben phai
            int[] colX = new int[columnCount];
            int[] colW = new int[columnCount];
            int x = GAP;
            for (int c = 0; c < columnCount; c++)
            {
                colW[c] = cardW + (c < extra ? 1 : 0);
                colX[c] = x;
                x += colW[c] + GAP;
            }

            pnlCanvas.SuspendLayout();
            try
            {
                for (int i = 0; i < orderedCards.Count; i++)
                {
                    if (i < pageStart || i >= pageEnd)
                    {
                        orderedCards[i].Visible = false;
                        continue;
                    }

                    int r = (i - pageStart) / columnCount;
                    int c = (i - pageStart) % columnCount;
                    orderedCards[i].SetBounds(colX[c], GAP + r * (cardH + GAP), colW[c], cardH);
                    orderedCards[i].Visible = true;
                }
            }
            finally
            {
                pnlCanvas.ResumeLayout(true);
            }
        }
    }
}
