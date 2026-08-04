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
using System.IO;
using System.Runtime.InteropServices;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Nen anh chu ky dien tu (EMR_SIGNER.SIGN_IMAGE) truoc khi encode base64 dien vao the CKDT_
    /// cua XML KHAMSUCKHOE (QD 1551) — cong tiep nhan gioi han moi the CKDT_ toi da 65000 ky tu.
    ///
    /// Anh chu ky thuong la anh CHUP/SCAN: nen giay trang nga bi nhieu (hang nghin mau xap xi nhau)
    /// nen PNG khong nen duoc, 1 anh 600x433 co the ra hon 334000 ky tu base64. Phan ton dung luong
    /// la NHIEU NEN, khong phai kich thuoc anh. Vi vay xu ly theo thu tu:
    ///   (1) ep nen gan trang (min(R,G,B) >= BACKGROUND_THRESHOLD) ve trang tinh  -> bo nhieu nen;
    ///   (2) posterize (lam tron tung kenh mau theo buoc) -> giam manh so mau;
    ///   (3) CHI khi (1)+(2) van vuot nguong moi thu nho kich thuoc anh.
    /// Nho vay giu nguyen do phan giai anh trong hau het truong hop (net chu ky khong bi mo).
    ///
    /// Khong dung PNG 8bpp-indexed vi GDI+ (System.Drawing) khong xuat duoc dinh dang nay;
    /// ep nen + posterize tren 24bpp da du nho ma khong can tu viet quantizer.
    /// </summary>
    internal static class KskSignImageCompressor
    {
        /// <summary>Gioi han do dai base64 cua 1 the CKDT_ theo cong tiep nhan QD 1551.</summary>
        public const int MAX_CKDT_BASE64_LENGTH = 65000;

        /// <summary>Nguong coi la nen giay: pixel co min(R,G,B) >= nguong nay -> ep ve trang tinh.</summary>
        private const int BACKGROUND_THRESHOLD = 215;

        /// <summary>Cac buoc posterize thu lan luot (buoc cang lon -> cang it mau -> file cang nho).</summary>
        private static readonly int[] POSTERIZE_STEPS = new int[] { 32, 64 };

        /// <summary>Cac ti le thu nho thu lan luot — 1.0 (giu nguyen kich thuoc) duoc uu tien truoc.</summary>
        private static readonly double[] SCALES = new double[] { 1.0, 0.75, 0.5, 0.35 };

        /// <summary>
        /// Tra base64 cua anh chu ky, da nen neu vuot MAX_CKDT_BASE64_LENGTH.
        /// Anh da du nho -> giu NGUYEN anh goc (khong xu ly gi). Loi/khong decode duoc anh -> tra
        /// base64 goc de khong chan viec dong bo.
        /// </summary>
        public static string ToBase64(byte[] signImage, string loginName)
        {
            if (signImage == null || signImage.Length == 0) return string.Empty;

            string raw = Convert.ToBase64String(signImage);
            if (raw.Length <= MAX_CKDT_BASE64_LENGTH) return raw;

            try
            {
                using (MemoryStream ms = new MemoryStream(signImage))
                using (Image src = Image.FromStream(ms))
                {
                    // Uu tien giu do phan giai: thu het cac buoc posterize o kich thuoc hien tai
                    // truoc khi ha kich thuoc xuong muc nho hon.
                    foreach (double scale in SCALES)
                    {
                        foreach (int step in POSTERIZE_STEPS)
                        {
                            string b64 = Encode(src, step, scale);
                            if (b64 == null || b64.Length > MAX_CKDT_BASE64_LENGTH) continue;

                            Inventec.Common.Logging.LogSystem.Info(string.Format(
                                "KskSignImage: nen chu ky '{0}' {1}x{2} tu {3} -> {4} ky tu base64"
                                + " (nen trang >= {5}, posterize {6}, ti le {7:0.##}).",
                                loginName, src.Width, src.Height, raw.Length, b64.Length,
                                BACKGROUND_THRESHOLD, step, scale));
                            return b64;
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Warn(string.Format(
                    "KskSignImage: chu ky '{0}' van vuot {1} ky tu sau khi nen het muc -> giu anh goc"
                    + " ({2} ky tu). Can thay anh chu ky goc nho hon.",
                    loginName, MAX_CKDT_BASE64_LENGTH, raw.Length));
            }
            catch (Exception ex)
            {
                // Khong decode duoc (khong phai anh / dinh dang la) -> giu anh goc, chi canh bao.
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return raw;
        }

        /// <summary>Ve lai anh o ti le chi dinh, ep nen + posterize, roi encode PNG -> base64.</summary>
        private static string Encode(Image src, int posterizeStep, double scale)
        {
            int width = Math.Max(1, (int)Math.Round(src.Width * scale));
            int height = Math.Max(1, (int)Math.Round(src.Height * scale));

            using (Bitmap bmp = Redraw(src, width, height))
            {
                FlattenAndPosterize(bmp, posterizeStep);
                using (MemoryStream outMs = new MemoryStream())
                {
                    bmp.Save(outMs, ImageFormat.Png);
                    return Convert.ToBase64String(outMs.ToArray());
                }
            }
        }

        /// <summary>
        /// Ve anh nguon len bitmap 24bpp nen trang — dong thoi bo kenh alpha (anh chu ky hau het
        /// opaque, vung trong suot phai thanh trang chu khong phai den).
        /// </summary>
        private static Bitmap Redraw(Image src, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(src, new Rectangle(0, 0, width, height));
                }
                return bmp;
            }
            catch
            {
                bmp.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Ep nen gan trang ve trang tinh + posterize cac pixel con lai (net muc chu ky).
        /// Duyet bang LockBits de khong goi GetPixel/SetPixel tren tung pixel (rat cham).
        /// </summary>
        private static void FlattenAndPosterize(Bitmap bmp, int posterizeStep)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                int stride = data.Stride;
                int total = stride * bmp.Height;
                byte[] buffer = new byte[total];
                Marshal.Copy(data.Scan0, buffer, 0, total);

                for (int y = 0; y < bmp.Height; y++)
                {
                    int row = y * stride;
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int i = row + x * 3;                    // 24bpp: thu tu byte la B, G, R
                        byte b = buffer[i], g = buffer[i + 1], r = buffer[i + 2];

                        int min = b < g ? (b < r ? b : r) : (g < r ? g : r);
                        if (min >= BACKGROUND_THRESHOLD)
                        {
                            buffer[i] = 255; buffer[i + 1] = 255; buffer[i + 2] = 255;
                            continue;
                        }
                        buffer[i] = Posterize(b, posterizeStep);
                        buffer[i + 1] = Posterize(g, posterizeStep);
                        buffer[i + 2] = Posterize(r, posterizeStep);
                    }
                }
                Marshal.Copy(buffer, 0, data.Scan0, total);
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        /// <summary>Lam tron gia tri kenh mau ve boi cua step (muc gan trang nhat keo len 255).</summary>
        private static byte Posterize(byte value, int step)
        {
            int quantized = (value / step) * step;
            return (byte)(quantized > 255 - step ? 255 : quantized);
        }
    }
}
