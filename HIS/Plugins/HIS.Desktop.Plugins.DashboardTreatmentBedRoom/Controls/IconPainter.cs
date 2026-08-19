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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// Ve icon bang GDI+ thay vi nhung file anh.
    /// Loi: khong phu thuoc resource / font icon, doi mau va scale tuy y, khong vo net khi doi DPI.
    /// </summary>
    internal static class IconPainter
    {
        public static void DrawHeart(Graphics g, RectangleF r, Color color)
        {
            float w = r.Width, h = r.Height;
            PointF bottom = new PointF(r.X + w / 2f, r.Y + h * 0.97f);
            PointF top = new PointF(r.X + w / 2f, r.Y + h * 0.30f);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(bottom,
                    new PointF(r.X - w * 0.08f, r.Y + h * 0.50f),
                    new PointF(r.X + w * 0.06f, r.Y - h * 0.06f),
                    top);
                path.AddBezier(top,
                    new PointF(r.X + w * 0.94f, r.Y - h * 0.06f),
                    new PointF(r.X + w * 1.08f, r.Y + h * 0.50f),
                    bottom);
                path.CloseFigure();

                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public static void DrawThermometer(Graphics g, RectangleF r, Color color)
        {
            float stemW = r.Width * 0.30f;
            float bulbD = r.Width * 0.52f;
            float cx = r.X + r.Width / 2f;

            using (SolidBrush brush = new SolidBrush(color))
            {
                // Than nhiet ke
                RectangleF stem = new RectangleF(cx - stemW / 2f, r.Y, stemW, r.Height * 0.72f);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(stem.X, stem.Y, stemW, stemW, 180, 180);
                    path.AddLine(stem.Right, stem.Y + stemW / 2f, stem.Right, stem.Bottom);
                    path.AddLine(stem.Right, stem.Bottom, stem.X, stem.Bottom);
                    path.CloseFigure();
                    g.FillPath(brush, path);
                }

                // Bau thuy ngan
                g.FillEllipse(brush, cx - bulbD / 2f, r.Bottom - bulbD, bulbD, bulbD);
            }

            // Vach chia
            using (Pen pen = new Pen(Color.FromArgb(120, Color.White), 1f))
            {
                for (int i = 1; i <= 3; i++)
                {
                    float y = r.Y + r.Height * 0.16f * i;
                    g.DrawLine(pen, cx - stemW / 2f + 1f, y, cx + stemW * 0.10f, y);
                }
            }
        }

        public static void DrawBloodPressure(Graphics g, RectangleF r, Color color)
        {
            using (Pen pen = new Pen(color, 1.6f))
            using (SolidBrush brush = new SolidBrush(color))
            {
                // Man hinh may do
                RectangleF box = new RectangleF(r.X, r.Y + r.Height * 0.06f, r.Width, r.Height * 0.66f);
                using (GraphicsPath path = BoardTheme.RoundedRect(
                    Rectangle.Round(box), (int)(r.Width * 0.18f)))
                {
                    g.DrawPath(pen, path);
                }

                // Kim chi + vach
                float cx = box.X + box.Width / 2f;
                float cy = box.Y + box.Height * 0.55f;
                g.FillEllipse(brush, cx - 1.4f, cy - 1.4f, 2.8f, 2.8f);
                g.DrawLine(pen, cx, cy, cx + box.Width * 0.22f, cy - box.Height * 0.28f);

                // Ong dan
                g.DrawLine(pen, cx, box.Bottom, cx, r.Bottom);
                g.DrawLine(pen, cx - r.Width * 0.22f, r.Bottom, cx + r.Width * 0.22f, r.Bottom);
            }
        }

        public static void DrawPerson(Graphics g, RectangleF r, Color color)
        {
            using (Pen pen = new Pen(color, 1.3f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float headD = r.Width * 0.46f;
                g.DrawEllipse(pen, r.X + (r.Width - headD) / 2f, r.Y + r.Height * 0.04f, headD, headD);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(r.X + r.Width * 0.08f, r.Y + r.Height * 0.55f,
                        r.Width * 0.84f, r.Height * 0.80f, 190, 160);
                    g.DrawPath(pen, path);
                }
            }
        }

        public static void DrawUsers(Graphics g, RectangleF r, Color color)
        {
            DrawPerson(g, new RectangleF(r.X, r.Y, r.Width * 0.72f, r.Height), color);
            using (Pen pen = new Pen(color, 1.3f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float headD = r.Width * 0.30f;
                g.DrawArc(pen, r.Right - headD - 1f, r.Y + r.Height * 0.06f, headD, headD, 280, 200);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(r.X + r.Width * 0.44f, r.Y + r.Height * 0.55f,
                        r.Width * 0.72f, r.Height * 0.78f, 250, 100);
                    g.DrawPath(pen, path);
                }
            }
        }

        public static void DrawBed(Graphics g, RectangleF r, Color color)
        {
            using (Pen pen = new Pen(color, 1.4f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float top = r.Y + r.Height * 0.30f;
                float bottom = r.Y + r.Height * 0.80f;

                // Dau giuong
                g.DrawLine(pen, r.X, r.Y + r.Height * 0.16f, r.X, bottom);
                // Mat giuong
                g.DrawLine(pen, r.X, top + r.Height * 0.18f, r.Right, top + r.Height * 0.18f);
                g.DrawLine(pen, r.Right, top + r.Height * 0.18f, r.Right, bottom);
                // Goi
                g.DrawArc(pen, r.X + r.Width * 0.10f, top - r.Height * 0.04f,
                    r.Width * 0.30f, r.Height * 0.30f, 180, 180);
                // Chan giuong
                g.DrawLine(pen, r.X, bottom, r.Right, bottom);
            }
        }

        public static void DrawShieldHeart(Graphics g, RectangleF r, Color color)
        {
            using (Pen pen = new Pen(color, 1.4f))
            using (GraphicsPath path = new GraphicsPath())
            {
                float w = r.Width, h = r.Height;
                PointF bottom = new PointF(r.X + w / 2f, r.Y + h * 0.94f);
                PointF top = new PointF(r.X + w / 2f, r.Y + h * 0.32f);

                path.AddBezier(bottom,
                    new PointF(r.X - w * 0.06f, r.Y + h * 0.50f),
                    new PointF(r.X + w * 0.08f, r.Y + h * 0.00f),
                    top);
                path.AddBezier(top,
                    new PointF(r.X + w * 0.92f, r.Y + h * 0.00f),
                    new PointF(r.X + w * 1.06f, r.Y + h * 0.50f),
                    bottom);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }
    }
}
