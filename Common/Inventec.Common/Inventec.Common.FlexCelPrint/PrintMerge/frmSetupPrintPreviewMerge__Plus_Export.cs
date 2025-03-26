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
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Inventec.Common.FlexCelPrint
{
    /// <summary>
    /// Printing / Previewing and Exporting xls files.
    /// </summary>
    public partial class frmSetupPrintPreviewMerge : DevExpress.XtraEditors.XtraForm
    {
        #region Common methods to Export with FlexCelImgExport
        private Bitmap CreateBitmap(double Resolution, TPaperDimensions pd, PixelFormat PxFormat)
        {
            Bitmap Result =
                new Bitmap((int)Math.Ceiling(pd.Width / 96F * Resolution),
                (int)Math.Ceiling(pd.Height / 96F * Resolution), PxFormat);
            Result.SetResolution((float)Resolution, (float)Resolution);
            return Result;

        }
        #endregion

        #region Export using FlexCelImgExport - simple images the hard way. DO NOT USE IF NOT DESPERATE!
        //The methods shows how to use FlexCelImgExport the "hard way", without using SaveAsImage.
        //For normal operation you should only need to call SaveAsImage, but you could use the code here
        //if you need to customize the ImgExport output, or if you need to get all the images as different files.
        private void CreateImg(Stream OutStream, FlexCelImgExport ImgExport, ImageFormat ImgFormat, ImageColorDepth Colors, ref TImgExportInfo ExportInfo)
        {
            TPaperDimensions pd = ImgExport.GetRealPageSize();

            PixelFormat RgbPixFormat;
            if (Colors != ImageColorDepth.TrueColor) RgbPixFormat = PixelFormat.Format32bppPArgb; else RgbPixFormat = PixelFormat.Format24bppRgb;
            PixelFormat PixFormat = PixelFormat.Format1bppIndexed;
            switch (Colors)
            {
                case ImageColorDepth.TrueColor: PixFormat = RgbPixFormat; break;
                case ImageColorDepth.Color256: PixFormat = PixelFormat.Format8bppIndexed; break;
            }

            using (Bitmap OutImg = CreateBitmap(ImgExport.Resolution, pd, PixFormat))
            {
                Bitmap ActualOutImg;
                if (Colors != ImageColorDepth.TrueColor) ActualOutImg = CreateBitmap(ImgExport.Resolution, pd, RgbPixFormat); else ActualOutImg = OutImg;
                try
                {
                    using (Graphics Gr = Graphics.FromImage(ActualOutImg))
                    {
                        Gr.FillRectangle(Brushes.White, 0, 0, ActualOutImg.Width, ActualOutImg.Height); //Clear the background
                        ImgExport.ExportNext(Gr, ref ExportInfo);
                    }

                    if (Colors == ImageColorDepth.BlackAndWhite) FloydSteinbergDither.ConvertToBlackAndWhite(ActualOutImg, OutImg);
                    else
                        if (Colors == ImageColorDepth.Color256)
                        {
                            OctreeQuantizer.ConvertTo256Colors(ActualOutImg, OutImg);
                        }
                }
                finally
                {
                    if (ActualOutImg != OutImg) ActualOutImg.Dispose();
                }

                OutImg.Save(OutStream, ImgFormat);
            }
        }

        private void ExportAllImages(FlexCelImgExport ImgExport, ImageFormat ImgFormat, ImageColorDepth ColorDepth)
        {
            TImgExportInfo ExportInfo = null; //For first page.
            int i = 0;
            do
            {
                string FileName = Path.GetDirectoryName(exportImageDialog.FileName)
                    + Path.DirectorySeparatorChar
                    + Path.GetFileNameWithoutExtension(exportImageDialog.FileName)
                    + "_" + ImgExport.Workbook.SheetName
                    + String.Format("_{0:0000}", i) +
                    Path.GetExtension(exportImageDialog.FileName);
                using (FileStream ImageStream = new FileStream(FileName, FileMode.Create))
                {
                    CreateImg(ImageStream, ImgExport, ImgFormat, ColorDepth, ref ExportInfo);
                }
                i++;
            } while (ExportInfo.CurrentPage < ExportInfo.TotalPages);
        }

        private void DoExportUsingFlexCelImgExportComplex(ImageColorDepth ColorDepth)
        {
            if (!HasFileOpen()) return;
            if (!LoadPreferences()) return;

            if (exportImageDialog.ShowDialog() != DialogResult.OK) return;

            System.Drawing.Imaging.ImageFormat ImgFormat = System.Drawing.Imaging.ImageFormat.Png;
            if (String.Compare(Path.GetExtension(exportImageDialog.FileName), ".jpg", true) == 0)
                ImgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

            using (FlexCelImgExport ImgExport = new FlexCelImgExport(flexCelPrintDocument1.Workbook))
            {
                ImgExport.Resolution = 96; //To get a better quality image but with larger file size too, increate this value. (for example to 300 or 600 dpi)

                ExportAllImages(ImgExport, ImgFormat, ColorDepth);

            }

        }
        #endregion

        #region Export using FlexCelImgExport - simple images the simple way.

        private void DoExportUsingFlexCelImgExportSimple(ImageColorDepth ColorDepth)
        {
            if (!HasFileOpen()) return;
            if (!LoadPreferences()) return;

            if (exportImageDialog.ShowDialog() != DialogResult.OK) return;

            ImageExportType ImgFormat = ImageExportType.Png;
            if (String.Compare(Path.GetExtension(exportImageDialog.FileName), ".jpg", true) == 0)
                ImgFormat = ImageExportType.Jpeg;

            using (FlexCelImgExport ImgExport = new FlexCelImgExport(flexCelPrintDocument1.Workbook))
            {
                ImgExport.AllVisibleSheets = false;
                ImgExport.ResetPageNumberOnEachSheet = cbResetPageNumber;
                ImgExport.Resolution = 96; //To get a better quality image but with larger file size too, increate this value. (for example to 300 or 600 dpi)
                ImgExport.SaveAsImage(exportImageDialog.FileName, ImgFormat, ColorDepth);
            }
        }

        #endregion

        #region Export using FlexCelImageExport - MultiPageTiff
        //How to create a multipage tiff using FlexCelImgExport.        
        //This will create a multipage tiff with the data.
        private void DoExportMultiPageTiff(ImageColorDepth ColorDepth, bool IsFax)
        {
            if (!HasFileOpen()) return;
            if (!LoadPreferences()) return;

            if (exportTiffDialog.ShowDialog() != DialogResult.OK) return;

            ImageExportType ExportType = ImageExportType.Tiff;
            if (IsFax) ExportType = ImageExportType.Fax;

            using (FlexCelImgExport ImgExport = new FlexCelImgExport(flexCelPrintDocument1.Workbook))
            {
                ImgExport.AllVisibleSheets = false;
                ImgExport.ResetPageNumberOnEachSheet = cbResetPageNumber;

                ImgExport.Resolution = 96; //To get a better quality image but with larger file size too, increate this value. (for example to 300 or 600 dpi)
                using (FileStream TiffStream = new FileStream(exportTiffDialog.FileName, FileMode.Create))
                {
                    ImgExport.SaveAsImage(TiffStream, ExportType, ColorDepth);
                }
            }
            if (MessageBox.Show("Do you want to open the generated file?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(exportTiffDialog.FileName);
            }

        }

        private void ImgBlackAndWhite_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportComplex(ImageColorDepth.BlackAndWhite);
        }

        private void Img256Colors_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportComplex(ImageColorDepth.Color256);
        }

        private void ImgTrueColor_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportComplex(ImageColorDepth.TrueColor);
        }

        private void ImgBlackAndWhite2_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportSimple(ImageColorDepth.BlackAndWhite);
        }

        private void Img256Colors2_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportSimple(ImageColorDepth.Color256);
        }

        private void ImgTrueColor2_Click(object sender, System.EventArgs e)
        {
            DoExportUsingFlexCelImgExportSimple(ImageColorDepth.TrueColor);
        }

        private void TiffFax_Click(object sender, System.EventArgs e)
        {
            DoExportMultiPageTiff(ImageColorDepth.BlackAndWhite, true);
        }

        private void TiffBlackAndWhite_Click(object sender, System.EventArgs e)
        {
            DoExportMultiPageTiff(ImageColorDepth.BlackAndWhite, false);
        }

        private void Tiff256Colors_Click(object sender, System.EventArgs e)
        {
            DoExportMultiPageTiff(ImageColorDepth.Color256, false);
        }

        private void TiffTrueColor_Click(object sender, System.EventArgs e)
        {
            DoExportMultiPageTiff(ImageColorDepth.TrueColor, false);
        }

        #endregion


    }
}
