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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier.Base
{
    /// <summary>
    /// Drawing helpers of the waiting screen: rounded cards, round queue number, status badge, calendar/eye/speaker icons
    /// </summary>
    internal static class WaitingScreenDrawHelper
    {
        internal static void PrepareGraphics(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        internal static GraphicsPath CreateRoundedPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        internal static void FillRounded(Graphics g, RectangleF rect, float radius, Color backColor, Color borderColor)
        {
            using (GraphicsPath path = CreateRoundedPath(rect, radius))
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    g.FillPath(brush, path);
                }
                if (borderColor != Color.Empty)
                {
                    using (Pen pen = new Pen(borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Queue number inside a circle
        /// </summary>
        internal static void DrawCircleNumber(Graphics g, Rectangle cell, string text, Font font, Color backColor, Color foreColor)
        {
            float d = Math.Min(cell.Height - 10, cell.Width - 8);
            if (d <= 0) return;
            RectangleF circle = new RectangleF(cell.X + (cell.Width - d) / 2f, cell.Y + (cell.Height - d) / 2f, d, d);
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillEllipse(brush, circle);
            }
            using (Pen pen = new Pen(Color.FromArgb(70, Color.White), 2f))
            {
                g.DrawEllipse(pen, circle.X + 2, circle.Y + 2, circle.Width - 4, circle.Height - 4);
            }
            DrawText(g, text, font, foreColor, circle, StringAlignment.Center);
        }

        /// <summary>
        /// Rounded status badge with centered text
        /// </summary>
        internal static void DrawBadge(Graphics g, Rectangle cell, string text, Font font, Color backColor, Color foreColor)
        {
            if (String.IsNullOrEmpty(text)) return;
            SizeF textSize = g.MeasureString(text, font);
            float width = Math.Min(cell.Width - 12, Math.Max(textSize.Width + 24, cell.Width * 0.62f));
            float height = Math.Min(cell.Height - 12, textSize.Height + 8);
            RectangleF rect = new RectangleF(cell.X + (cell.Width - width) / 2f, cell.Y + (cell.Height - height) / 2f, width, height);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, ControlPaint.Light(backColor, 0.25f), backColor, LinearGradientMode.Vertical))
            using (GraphicsPath path = CreateRoundedPath(rect, height * 0.22f))
            {
                g.FillPath(brush, path);
            }
            DrawText(g, text, font, foreColor, rect, StringAlignment.Center);
        }

        internal static void DrawText(Graphics g, string text, Font font, Color color, RectangleF rect, StringAlignment alignment)
        {
            using (StringFormat format = new StringFormat())
            using (SolidBrush brush = new SolidBrush(color))
            {
                format.Alignment = alignment;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                g.DrawString(text ?? "", font, brush, rect, format);
            }
        }

        /// <summary>
        /// Calendar icon shown before the birth year
        /// </summary>
        internal static void DrawCalendarIcon(Graphics g, RectangleF r, Color color)
        {
            float w = r.Width;
            using (Pen pen = new Pen(color, Math.Max(1.5f, w / 12f)))
            using (SolidBrush brush = new SolidBrush(color))
            {
                RectangleF body = new RectangleF(r.X, r.Y + w * 0.12f, w, r.Height - w * 0.12f);
                g.DrawRectangle(pen, body.X, body.Y, body.Width, body.Height);
                g.FillRectangle(brush, body.X, body.Y, body.Width, body.Height * 0.25f);
                g.DrawLine(pen, r.X + w * 0.28f, r.Y, r.X + w * 0.28f, r.Y + w * 0.26f);
                g.DrawLine(pen, r.X + w * 0.72f, r.Y, r.X + w * 0.72f, r.Y + w * 0.26f);
                float gap = body.Width / 4f;
                float dot = body.Width * 0.14f;
                for (int row = 0; row < 2; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        g.FillRectangle(brush, body.X + gap * (col + 1) - dot / 2f, body.Y + body.Height * (0.5f + row * 0.25f) - dot / 2f, dot, dot);
                    }
                }
            }
        }

        /// <summary>
        /// Eye-slash icon (hidden amount)
        /// </summary>
        internal static void DrawEyeSlashIcon(Graphics g, RectangleF r, Color color)
        {
            using (Pen pen = new Pen(color, Math.Max(1.5f, r.Width / 10f)))
            {
                RectangleF eye = new RectangleF(r.X, r.Y + r.Height * 0.2f, r.Width, r.Height * 0.6f);
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddArc(eye.X, eye.Y, eye.Width, eye.Height * 2, 200, 140);
                    path.AddArc(eye.X, eye.Y - eye.Height, eye.Width, eye.Height * 2, 20, 140);
                    path.CloseFigure();
                    g.DrawPath(pen, path);
                }
                float pupil = r.Width * 0.3f;
                g.DrawEllipse(pen, r.X + (r.Width - pupil) / 2f, r.Y + (r.Height - pupil) / 2f, pupil, pupil);
                g.DrawLine(pen, r.X + r.Width * 0.1f, r.Y + r.Height * 0.1f, r.Right - r.Width * 0.1f, r.Bottom - r.Height * 0.1f);
            }
        }

        /// <summary>
        /// Speaker icon
        /// </summary>
        internal static void DrawSpeakerIcon(Graphics g, RectangleF r, Color color)
        {
            float w = r.Width;
            float h = r.Height;
            using (SolidBrush brush = new SolidBrush(color))
            using (Pen pen = new Pen(color, Math.Max(2f, w / 14f)))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                PointF[] body = new PointF[]
                {
                    new PointF(r.X, r.Y + h * 0.36f),
                    new PointF(r.X + w * 0.2f, r.Y + h * 0.36f),
                    new PointF(r.X + w * 0.5f, r.Y + h * 0.1f),
                    new PointF(r.X + w * 0.5f, r.Y + h * 0.9f),
                    new PointF(r.X + w * 0.2f, r.Y + h * 0.64f),
                    new PointF(r.X, r.Y + h * 0.64f)
                };
                g.FillPolygon(brush, body);
                g.DrawArc(pen, r.X + w * 0.38f, r.Y + h * 0.3f, w * 0.3f, h * 0.4f, -50, 100);
                g.DrawArc(pen, r.X + w * 0.36f, r.Y + h * 0.15f, w * 0.48f, h * 0.7f, -50, 100);
                g.DrawArc(pen, r.X + w * 0.34f, r.Y, w * 0.66f, h, -50, 100);
            }
        }

        /// <summary>
        /// Enable double buffering on owner-drawn panels to avoid flicker while blinking
        /// </summary>
        internal static void EnableDoubleBuffer(Control control)
        {
            try
            {
                typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(control, true, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static Color ParseColor(string html, Color defaultColor)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(html))
                {
                    return ColorTranslator.FromHtml(html);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return defaultColor;
        }

        internal static string ToHtml(Color color)
        {
            return String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// Show the form full screen on the extended monitor if any, otherwise maximized on the primary monitor
        /// </summary>
        internal static void ShowInExtendMonitor(Form form)
        {
            Screen secondScreen = null;
            foreach (Screen sc in Screen.AllScreens)
            {
                if (!sc.Primary)
                {
                    secondScreen = sc;
                    break;
                }
            }
            if (secondScreen != null)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition = FormStartPosition.Manual;
                form.Location = secondScreen.Bounds.Location;
            }
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }
    }
}
