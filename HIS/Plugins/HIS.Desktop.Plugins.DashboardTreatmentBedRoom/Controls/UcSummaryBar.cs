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
using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// Dai thong ke dau man hinh: Nguoi benh / Giuong benh / Che do cham soc.
    /// Ba khoi nen bo goc ve trong OnPaint, con cac con so va nhan la LabelControl that.
    /// </summary>
    public partial class UcSummaryBar : XtraUserControl
    {
        public const int DEFAULT_HEIGHT = 122;

        private const int RADIUS = 10;
        private const int GAP = 12;
        private const int PAD = 16;
        private const int NUMBER_TOP = 42;
        private const int CAPTION_TOP = 72;

        private Rectangle rectPatients, rectBeds, rectCare;

        /// <summary>
        /// Kho nhan cho cum che do cham soc. Bon o dau lay tu Designer, thieu thi sinh them.
        /// Giu lai de dung lai giua cac lan lam moi, khong dung roi huy lien tuc.
        /// </summary>
        private readonly List<LabelControl> careNumbers = new List<LabelControl>();
        private readonly List<LabelControl> careCaptions = new List<LabelControl>();

        /// <summary>So o dang thuc su hien, phan con lai trong kho bi an di.</summary>
        private int careVisibleCount;

        public UcSummaryBar()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            BackColor = BoardTheme.PageBack;
            Height = DEFAULT_HEIGHT;

            careNumbers.AddRange(new LabelControl[] { lblCare1, lblCare2, lblCare3, lblSpecial });
            careCaptions.AddRange(new LabelControl[] { lblCare1Cap, lblCare2Cap, lblCare3Cap, lblSpecialCap });
            careVisibleCount = careNumbers.Count;

            // InitializeComponent gan Size -> da ban OnSizeChanged mot lan luc kho nhan con rong,
            // luot do khong dat duoc gi. Phai bo tri lai o day, neu khong cac nhan se nam nguyen
            // toa do Designer va de len nhom ben canh.
            RelayoutGroups();
        }

        /// <param name="sdo">Cac con so tong, lay thang tu API.</param>
        /// <param name="careLevels">Danh sach o cham soc, do UcInpatientBoard dung san tu danh muc.</param>
        public void SetData(HisTreatmentBedRoomDashboardSDO sdo, List<CareLevelSummaryADO> careLevels)
        {
            if (sdo != null)
            {
                lblInPatientCurrent.Text = sdo.CurrentInpatientTotal.ToString();
                lblWaitIn.Text = sdo.WaitingHospitalizeTotal.ToString();
                lblInToday.Text = sdo.HospitalizeTotal.ToString();
                lblOutToday.Text = sdo.DischargeTotal.ToString();

                lblBedTotal.Text = sdo.BedTotal.ToString();
                lblBedUsed.Text = sdo.UsedBedTotal.ToString();
                lblBedEmpty.Text = sdo.EmptyBedTotal.ToString();
            }

            BindCareLevels(careLevels);
            RelayoutGroups();
        }

        private void BindCareLevels(List<CareLevelSummaryADO> levels)
        {
            if (levels == null) levels = new List<CareLevelSummaryADO>();

            EnsureCareSlots(levels.Count);

            SuspendLayout();
            try
            {
                for (int i = 0; i < careNumbers.Count; i++)
                {
                    bool used = i < levels.Count;
                    careNumbers[i].Visible = used;
                    careCaptions[i].Visible = used;
                    if (!used) continue;

                    CareLevelSummaryADO level = levels[i];
                    careNumbers[i].Text = level.TOTAL.ToString();
                    careNumbers[i].Appearance.ForeColor = level.DISPLAY_COLOR.HasValue
                        ? level.DISPLAY_COLOR.Value
                        : BoardTheme.TextBlue;
                    careCaptions[i].Text = level.CARE_LEVEL_NAME;
                    careCaptions[i].ToolTip = level.CARE_LEVEL_NAME;
                }
            }
            finally
            {
                ResumeLayout(false);
            }

            careVisibleCount = levels.Count;
        }

        /// <summary>
        /// Sinh them nhan khi danh muc co nhieu cap hon so o dung san tren Designer.
        /// Nhan moi sao lai kieu cua o dau tien de khong lech phong.
        /// </summary>
        private void EnsureCareSlots(int needed)
        {
            while (careNumbers.Count < needed)
            {
                LabelControl number = new LabelControl();
                number.Appearance.Font = lblCare1.Appearance.Font;
                number.Appearance.ForeColor = BoardTheme.TextBlue;
                number.Appearance.Options.UseFont = true;
                number.Appearance.Options.UseForeColor = true;
                number.AutoSizeMode = LabelAutoSizeMode.None;

                LabelControl caption = new LabelControl();
                caption.Appearance.Font = lblCare1Cap.Appearance.Font;
                caption.Appearance.ForeColor = lblCare1Cap.Appearance.ForeColor;
                caption.Appearance.Options.UseFont = true;
                caption.Appearance.Options.UseForeColor = true;
                caption.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
                caption.AutoSizeMode = LabelAutoSizeMode.None;

                careNumbers.Add(number);
                careCaptions.Add(caption);
                Controls.Add(number);
                Controls.Add(caption);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RelayoutGroups();
        }

        /// <summary>
        /// Chia be ngang theo so o thuc te cua tung nhom: 4 nguoi benh / 3 giuong / N cham soc.
        /// Danh muc co bao nhieu cap thi nhom cham soc rong bay nhieu phan.
        /// </summary>
        private void RelayoutGroups()
        {
            int usable = Width - GAP * 2;
            if (usable <= 0) return;

            // Kho nhan chua nap xong (dang trong InitializeComponent) thi chua bo tri duoc gi
            if (careNumbers.Count == 0) return;

            int careCount = careVisibleCount > 0 ? careVisibleCount : 1;
            int weightTotal = 4 + 3 + careCount;

            int w1 = usable * 4 / weightTotal;
            int w2 = usable * 3 / weightTotal;
            int w3 = usable - w1 - w2;

            rectPatients = new Rectangle(0, 0, w1, Height);
            rectBeds = new Rectangle(w1 + GAP, 0, w2, Height);
            rectCare = new Rectangle(w1 + GAP + w2 + GAP, 0, w3, Height);

            PlaceHeader(icoPatients, lblPatientsTitle, rectPatients);
            PlaceHeader(icoBeds, lblBedsTitle, rectBeds);
            PlaceHeader(icoCare, lblCareTitle, rectCare);

            PlaceColumn(rectPatients, 0, 4, lblInPatientCurrent, lblInPatientCurrentCap);
            PlaceColumn(rectPatients, 1, 4, lblWaitIn, lblWaitInCap);
            PlaceColumn(rectPatients, 2, 4, lblInToday, lblInTodayCap);
            PlaceColumn(rectPatients, 3, 4, lblOutToday, lblOutTodayCap);

            lblNote.SetBounds(rectPatients.X + PAD, rectPatients.Bottom - 22,
                Math.Max(20, rectPatients.Width - PAD * 2), 16);

            PlaceColumn(rectBeds, 0, 3, lblBedTotal, lblBedTotalCap);
            PlaceColumn(rectBeds, 1, 3, lblBedUsed, lblBedUsedCap);
            PlaceColumn(rectBeds, 2, 3, lblBedEmpty, lblBedEmptyCap);

            for (int i = 0; i < careVisibleCount && i < careNumbers.Count; i++)
            {
                PlaceColumn(rectCare, i, careVisibleCount, careNumbers[i], careCaptions[i]);
            }

            Invalidate();
        }

        private void PlaceHeader(IconBox icon, LabelControl title, Rectangle group)
        {
            icon.Location = new Point(group.X + PAD, group.Y + 13);
            title.Location = new Point(group.X + PAD + 22, group.Y + 12);
        }

        private void PlaceColumn(Rectangle group, int index, int count, LabelControl number, LabelControl caption)
        {
            if (count <= 0) return;

            int colW = (group.Width - PAD * 2) / count;
            if (colW < 1) colW = 1;
            int x = group.X + PAD + colW * index;

            number.SetBounds(x, group.Y + NUMBER_TOP, colW, 30);
            caption.SetBounds(x, group.Y + CAPTION_TOP, colW, 16);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // base truoc, xem chu thich trong UcBedCard.OnPaint
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            BoardTheme.DrawCard(e.Graphics, rectPatients, RADIUS, BoardTheme.CardBack, BoardTheme.CardBorder);
            BoardTheme.DrawCard(e.Graphics, rectBeds, RADIUS, BoardTheme.CardBack, BoardTheme.CardBorder);
            BoardTheme.DrawCard(e.Graphics, rectCare, RADIUS, BoardTheme.CardBack, BoardTheme.CardBorder);
        }
    }
}
