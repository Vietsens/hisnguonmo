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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint
{
    class FlexCelPrintUtil
    {
        ////Method to calculate PaperSize from Centimeter to 1/100 of an inch
        ///// Caclulates the paper size
        ///// </summary>
        ///// <param name="WidthInCentimeters"></param>
        ///// <param name="HeightInCentimetres"></param>
        ///// <returns></returns>
        //public static System.Drawing.Printing.PaperSize CalculatePaperSize(double WidthInCentimeters,
        //    double HeightInCentimetres)
        //{
        //    int Width = int.Parse((Math.Round((WidthInCentimeters), 0, MidpointRounding.AwayFromZero)).ToString());
        //    int Height = int.Parse((Math.Round((HeightInCentimetres), 0, MidpointRounding.AwayFromZero)).ToString());

        //    PaperSize NewSize = new PaperSize();
        //    NewSize.RawKind = (int)PaperKind.Custom;
        //    NewSize.Width = Width + 20;
        //    NewSize.Height = Height;
        //    NewSize.PaperName = "Letter (" + Width + "," + Height + ")";

        //    return NewSize;

        //}
        internal const int WM_VSCROLLCLIPBOARD = 0x030A;
        internal const int WM_MOUSEWHEEL = 0x020A;

        internal const int WM_VSCROLL = 277;
        internal const int SB_LINEUP = 0;
        internal const int SB_LINEDOWN = 1;
        internal const int SB_PAGEUP = 2;
        internal const int SB_PAGEDOWN = 3;
        internal const int SB_ENDSCROLL = 4;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        internal static byte[] StreamToByte(Stream input)
        {
            input.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        internal static byte[] FileToByte(string input)
        {
            return File.ReadAllBytes(input);
        }

        private const string KeyHidePrinting = "Inventec.Common.FlexCelPrint.HidePrinting";

        internal static void SetConfigValue(bool check)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[KeyHidePrinting];
            if (entry == null)
                config.AppSettings.Settings.Add(KeyHidePrinting, check ? "1" : "0");
            else
                config.AppSettings.Settings[KeyHidePrinting].Value = check ? "1" : "0";

            config.Save(ConfigurationSaveMode.Modified);
        }

        internal static bool GetConfigValue()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var entry = config.AppSettings.Settings[KeyHidePrinting];
            if (entry == null)
            {
                return false;
            }
            else
            {
                return entry.Value == "1";
            }
        }

        public static void ConvertExcelToPdfByFlexCel(MemoryStream excelStream, MemoryStream pdfStream)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.1");
                FlexCel.Render.FlexCelPdfExport flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();
                flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new FlexCel.XlsAdapter.XlsFile();
                flexCelPdfExport1.Workbook.Open(excelStream);

                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2");
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(pdfStream);

                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();

                    flexCelPdfExport1.EndExport();
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
                pdfStream.Position = 0;
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.3");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public static void ConvertExcelToPdfByFlexCel(string excelPath, MemoryStream pdfStream)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.1");
                FlexCel.Render.FlexCelPdfExport flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();
                flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new FlexCel.XlsAdapter.XlsFile();
                flexCelPdfExport1.Workbook.Open(excelPath);

                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2");
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(pdfStream);

                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();

                    flexCelPdfExport1.EndExport();
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
                pdfStream.Position = 0;
                Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.3");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

    }
}
