/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Dung anh dien vao the CKDT_ cua XML KHAMSUCKHOE (QD 1551): anh chu ky (EMR_SIGNER.SIGN_IMAGE) o tren,
    /// ho ten nguoi kham o duoi. Chuan QD 1551 khong co the ho ten bac si, cong HOC chi hien thi anh CKDT_
    /// o cot "Ho ten va chu ky cua Bac sy" nen ho ten phai nam trong anh. Nguoi kham chua co anh chu ky
    /// -> anh chi co dong ho ten.
    ///
    /// Tra PNG; ma hoa base64 (va nen neu vuot 65000 ky tu) van di qua KskSignImageCompressor.ToBase64.
    /// </summary>
    internal static class KskSignImageComposer
    {
        /// <summary>Chieu cao anh chu ky sau khi thu phong (giu ti le).</summary>
        private const int SIGN_HEIGHT = 100;

        /// <summary>Chieu rong toi da anh chu ky; vuot thi thu theo chieu rong.</summary>
        private const int SIGN_MAX_WIDTH = 300;

        /// <summary>Khoang cach giua anh chu ky va dong ho ten.</summary>
        private const int NAME_GAP = 4;

        /// <summary>Le trang 4 phia.</summary>
        private const int PADDING = 8;

        private const string NAME_FONT_FAMILY = "Times New Roman";

        /// <summary>Co chu dong ho ten (pixel).</summary>
        private const float NAME_FONT_SIZE = 26f;

        /// <summary>
        /// Tra PNG anh chu ky kem ho ten:
        ///   - co anh chu ky + co ho ten -> anh chu ky o tren, ho ten o duoi;
        ///   - khong co anh chu ky       -> anh chi co ho ten;
        ///   - khong co ho ten           -> giu nguyen anh chu ky goc (khong co gi de ghep);
        ///   - ca hai deu khong co       -> null (the CKDT_ de trong nhu cu).
        /// Anh chu ky khong doc duoc (khong phai anh/dinh dang la) -> giu nguyen anh goc, chi canh bao,
        /// de khong lam mat chu ky da co.
        /// </summary>
        public static byte[] Compose(byte[] signImage, string userName, string loginName)
        {
            bool hasSign = signImage != null && signImage.Length > 0;
            string name = (userName ?? "").Trim().Normalize(NormalizationForm.FormC);
            if (name.Length == 0) return hasSign ? signImage : null;

            Image sign = null;
            try
            {
                if (hasSign)
                {
                    try
                    {
                        sign = Image.FromStream(new MemoryStream(signImage));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("KskSignImage: khong doc duoc anh chu ky '" + loginName
                            + "' -> giu anh goc, khong ghep ho ten.");
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        return signImage;
                    }
                }
                return Draw(sign, name);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("KskSignImage: loi dung anh chu ky kem ho ten '" + loginName + "'.");
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return hasSign ? signImage : null;
            }
            finally
            {
                if (sign != null) sign.Dispose();
            }
        }

        /// <summary>Ve anh nen trang: anh chu ky (neu co) can giua o tren, dong ho ten can giua o duoi.</summary>
        private static byte[] Draw(Image sign, string name)
        {
            using (Font font = new Font(NAME_FONT_FAMILY, NAME_FONT_SIZE, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Size text = MeasureText(name, font);

                int signWidth = 0, signHeight = 0;
                if (sign != null)
                {
                    double scale = (double)SIGN_HEIGHT / sign.Height;
                    if (sign.Width * scale > SIGN_MAX_WIDTH) scale = (double)SIGN_MAX_WIDTH / sign.Width;
                    signWidth = Math.Max(1, (int)Math.Round(sign.Width * scale));
                    signHeight = Math.Max(1, (int)Math.Round(sign.Height * scale));
                }

                int width = Math.Max(signWidth, text.Width) + 2 * PADDING;
                int height = PADDING + (sign != null ? signHeight + NAME_GAP : 0) + text.Height + PADDING;

                using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        // Nen trang tinh: anh chu ky co kenh alpha thi vung trong suot thanh trang, khong thanh den.
                        g.Clear(Color.White);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        int y = PADDING;
                        if (sign != null)
                        {
                            g.DrawImage(sign, new Rectangle((width - signWidth) / 2, y, signWidth, signHeight));
                            y += signHeight + NAME_GAP;
                        }
                        g.DrawString(name, font, Brushes.Black, (width - text.Width) / 2f, y);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>Kich thuoc (lam tron len) cua dong chu khi ve bang font chi dinh.</summary>
        private static Size MeasureText(string text, Font font)
        {
            using (Bitmap probe = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(probe))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                SizeF size = g.MeasureString(text, font);
                return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
            }
        }
    }
}
