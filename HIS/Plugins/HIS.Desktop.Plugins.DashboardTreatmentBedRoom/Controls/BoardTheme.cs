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
using System.Windows.Forms;

using HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// Bang mau + font + tien ich ve dung chung cho toan bo man hinh.
    /// Doi giao dien thi sua o day, khong sua rai rac trong tung control.
    /// </summary>
    internal static class BoardTheme
    {
        // --- Nen / vien ---
        public static readonly Color PageBack = Color.FromArgb(243, 244, 246);
        public static readonly Color CardBack = Color.White;
        public static readonly Color CardBorder = Color.FromArgb(232, 234, 238);
        public static readonly Color Separator = Color.FromArgb(238, 240, 243);

        // --- Chu ---
        public static readonly Color TextDark = Color.FromArgb(31, 41, 55);
        public static readonly Color TextBody = Color.FromArgb(75, 85, 99);
        public static readonly Color TextMuted = Color.FromArgb(148, 156, 168);
        public static readonly Color TextBlue = Color.FromArgb(29, 78, 216);

        // --- Cap cham soc ---
        public static readonly Color Level1 = Color.FromArgb(239, 68, 68);
        public static readonly Color Level2 = Color.FromArgb(245, 158, 11);
        public static readonly Color Level3 = Color.FromArgb(34, 197, 94);
        public static readonly Color LevelNone = Color.FromArgb(203, 209, 217);

        public static readonly Color Level1Back = Color.FromArgb(254, 242, 242);
        public static readonly Color Level2Back = Color.FromArgb(255, 251, 235);
        public static readonly Color Level3Back = Color.White;

        // --- O sinh hieu ---
        public static readonly Color PulseBack = Color.FromArgb(254, 226, 226);
        public static readonly Color PulseFore = Color.FromArgb(239, 68, 68);
        public static readonly Color TempBack = Color.FromArgb(254, 243, 199);
        public static readonly Color TempFore = Color.FromArgb(217, 119, 6);
        public static readonly Color BpBack = Color.FromArgb(219, 234, 254);
        public static readonly Color BpFore = Color.FromArgb(37, 99, 235);

        // --- Giuong trong ---
        public static readonly Color EmptyBack = Color.FromArgb(247, 248, 250);
        public static readonly Color EmptyHatch = Color.FromArgb(235, 237, 241);
        public static readonly Color EmptyBorder = Color.FromArgb(223, 226, 232);

        // --- Font ---
        public static readonly Font FontRoomTitle = new Font("Segoe UI", 12.5f, FontStyle.Bold);
        public static readonly Font FontPatient = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontBedCode = new Font("Segoe UI", 8.25f, FontStyle.Bold);
        public static readonly Font FontMeta = new Font("Segoe UI", 8.25f, FontStyle.Regular);
        public static readonly Font FontDiagnosis = new Font("Segoe UI", 8.75f, FontStyle.Regular);
        public static readonly Font FontVitalValue = new Font("Segoe UI", 8.25f, FontStyle.Bold);
        public static readonly Font FontVitalCaption = new Font("Segoe UI", 6.75f, FontStyle.Regular);
        public static readonly Font FontStatNumber = new Font("Segoe UI", 18f, FontStyle.Bold);
        public static readonly Font FontStatCaption = new Font("Segoe UI", 8.25f, FontStyle.Regular);
        public static readonly Font FontGroupTitle = new Font("Segoe UI", 9.75f, FontStyle.Bold);
        public static readonly Font FontNote = new Font("Segoe UI", 7.5f, FontStyle.Italic);
        public static readonly Font FontEmptyBed = new Font("Segoe UI", 8.75f, FontStyle.Regular);

        public const TextFormatFlags FlagsLeft =
            TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine |
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
            TextFormatFlags.NoPadding;

        public const TextFormatFlags FlagsCenter =
            TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine |
            TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter |
            TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

        public static Color GetLevelColor(CareLevel level)
        {
            switch (level)
            {
                case CareLevel.Level1: return Level1;
                case CareLevel.Level2: return Level2;
                case CareLevel.Level3: return Level3;
                default: return LevelNone;
            }
        }

        public static Color GetLevelBackColor(CareLevel level)
        {
            switch (level)
            {
                case CareLevel.Level1: return Level1Back;
                case CareLevel.Level2: return Level2Back;
                default: return Level3Back;
            }
        }

        /// <summary>
        /// Doc DISPLAY_COLOR cua danh muc HIS. Truong nay khong thong nhat mot dinh dang nen nhan
        /// het cac kieu hay gap: "#RRGGBB", "#AARRGGBB", "RRGGBB", "r,g,b", so nguyen ARGB, ten mau.
        /// Doc khong duoc thi tra null de roi ve mau mac dinh - mot o mau sai cau hinh khong duoc
        /// phep lam hong ca bang.
        ///
        /// Cho nhap nhang: chuoi toan chu so nhu "16711680" vua doc duoc thanh hex vua doc duoc
        /// thanh thap phan, hai cach ra hai mau khac han. Thu tu quyet dinh:
        ///   1. co dau '#'                        -> hex
        ///   2. toan chu so va dai tu 7 ky tu     -> thap phan (mau hex khong bao gio dai 7 ky tu)
        ///   3. dai 6 hoac 8 va doc duoc hex      -> hex
        ///   4. so nguyen (ke ca so am kieu .NET) -> thap phan
        ///   5. con lai                           -> ten mau
        /// </summary>
        public static Color? ParseDisplayColor(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return null;

            string s = raw.Trim();
            if (s.Length == 0) return null;

            try
            {
                if (s.IndexOf(',') >= 0) return ParseRgbTriple(s);

                bool hasHash = s.StartsWith("#");
                string body = hasHash ? s.Substring(1) : s;
                if (body.Length == 0) return null;

                if (hasHash) return ParseHex(body);

                if (!(IsAllDigits(body) && body.Length >= 7))
                {
                    Color? hex = ParseHex(body);
                    if (hex.HasValue) return hex;
                }

                int argb;
                if (int.TryParse(s, System.Globalization.NumberStyles.AllowLeadingSign,
                    System.Globalization.CultureInfo.InvariantCulture, out argb))
                {
                    Color c = Color.FromArgb(argb);
                    // So nguyen khong kem alpha (vd 16711680 = do) se ra alpha 0 -> vo hinh
                    return c.A == 0 ? Color.FromArgb(255, c.R, c.G, c.B) : c;
                }

                Color named = Color.FromName(s);
                return named.IsKnownColor ? (Color?)named : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Khong doc duoc DISPLAY_COLOR: " + raw + ". " + ex.Message);
                return null;
            }
        }

        private static Color? ParseHex(string body)
        {
            if (body.Length != 6 && body.Length != 8) return null;

            uint value;
            if (!uint.TryParse(body, System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture, out value)) return null;

            if (body.Length == 6)
            {
                return Color.FromArgb(255,
                    (int)((value >> 16) & 0xFF), (int)((value >> 8) & 0xFF), (int)(value & 0xFF));
            }
            return Color.FromArgb((int)((value >> 24) & 0xFF),
                (int)((value >> 16) & 0xFF), (int)((value >> 8) & 0xFF), (int)(value & 0xFF));
        }

        private static Color? ParseRgbTriple(string s)
        {
            string[] parts = s.Split(',');
            if (parts.Length < 3) return null;

            int r, g, b;
            if (int.TryParse(parts[0].Trim(), out r) &&
                int.TryParse(parts[1].Trim(), out g) &&
                int.TryParse(parts[2].Trim(), out b))
            {
                return Color.FromArgb(Clamp(r), Clamp(g), Clamp(b));
            }
            return null;
        }

        private static bool IsAllDigits(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < '0' || s[i] > '9') return false;
            }
            return true;
        }

        private static int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return v;
        }

        /// <summary>
        /// Pha mau ve phia trang de lam nen the. Dung khi mau den tu DISPLAY_COLOR cua HIS -
        /// mau danh muc thuong rat dam, to nguyen ban lam nen thi khong doc noi chu.
        /// </summary>
        public static Color Lighten(Color color, double amount)
        {
            if (amount < 0) amount = 0;
            if (amount > 1) amount = 1;

            int r = (int)(color.R + (255 - color.R) * amount);
            int g = (int)(color.G + (255 - color.G) * amount);
            int b = (int)(color.B + (255 - color.B) * amount);
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Duong bien bo goc. Dung chung cho the phong / the giuong / o sinh hieu.
        /// </summary>
        public static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(r);
                return path;
            }

            int d = radius * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void FillRounded(Graphics g, Rectangle r, int radius, Color back)
        {
            using (GraphicsPath path = RoundedRect(r, radius))
            using (SolidBrush brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Ve nen + vien bo goc. Tru 1px o phai/duoi de net vien khong bi cat.
        /// </summary>
        public static void DrawCard(Graphics g, Rectangle r, int radius, Color back, Color border)
        {
            Rectangle rr = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
            using (GraphicsPath path = RoundedRect(rr, radius))
            {
                using (SolidBrush brush = new SolidBrush(back))
                {
                    g.FillPath(brush, path);
                }
                using (Pen pen = new Pen(border))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Bat double buffer cho Panel/TableLayoutPanel (thuoc tinh protected nen phai reflection).
        /// Khong bat thi cuon danh sach phong se nhay rat ro.
        /// </summary>
        public static void EnableDoubleBuffer(Control control)
        {
            if (control == null || SystemInformation.TerminalServerSession) return;

            System.Reflection.PropertyInfo prop = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (prop != null)
            {
                prop.SetValue(control, true, null);
            }
        }

        public static int MeasureWidth(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding).Width;
        }
    }
}
