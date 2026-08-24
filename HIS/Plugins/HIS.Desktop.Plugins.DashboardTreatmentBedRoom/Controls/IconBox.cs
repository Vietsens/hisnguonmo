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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    public enum IconKind
    {
        None = 0,
        Heart = 1,
        Thermometer = 2,
        BloodPressure = 3,
        Person = 4,
        Bed = 5,
        Users = 6,
        ShieldHeart = 7
    }

    /// <summary>
    /// O ve icon vector. Lam thanh control rieng de keo tha duoc tren Designer va nhin thay ngay
    /// luc thiet ke - dat anh PNG vao resource thi Designer chi hien mot o xam.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Kind")]
    public class IconBox : Control
    {
        private IconKind kind = IconKind.Heart;
        private Color iconColor = Color.FromArgb(107, 114, 128);

        public IconBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(14, 14);
            TabStop = false;
        }

        [Category("Giao diện"), DefaultValue(IconKind.Heart)]
        [Description("Loại icon được vẽ")]
        public IconKind Kind
        {
            get { return kind; }
            set { kind = value; Invalidate(); }
        }

        [Category("Giao diện")]
        [Description("Màu nét vẽ của icon")]
        public Color IconColor
        {
            get { return iconColor; }
            set { iconColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF r = new RectangleF(0, 0, Width, Height);

            switch (kind)
            {
                case IconKind.Heart: IconPainter.DrawHeart(e.Graphics, r, iconColor); break;
                case IconKind.Thermometer: IconPainter.DrawThermometer(e.Graphics, r, iconColor); break;
                case IconKind.BloodPressure: IconPainter.DrawBloodPressure(e.Graphics, r, iconColor); break;
                case IconKind.Person: IconPainter.DrawPerson(e.Graphics, r, iconColor); break;
                case IconKind.Bed: IconPainter.DrawBed(e.Graphics, r, iconColor); break;
                case IconKind.Users: IconPainter.DrawUsers(e.Graphics, r, iconColor); break;
                case IconKind.ShieldHeart: IconPainter.DrawShieldHeart(e.Graphics, r, iconColor); break;
            }
        }
    }
}
