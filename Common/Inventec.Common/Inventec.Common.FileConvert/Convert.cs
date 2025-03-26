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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;

namespace Inventec.Common.FileConvert
{
    public class Convert
    {
        public static bool ExcelToPdfUsingFlex(MemoryStream inputStream, string inputFile, MemoryStream outputStream, string outputFile)
        {
            bool success = false;

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
            if (inputStream != null && inputStream.Length > 0)
            {
                inputStream.Position = 0;
                flexCelPdfExport1.Workbook.Open(inputStream);
            }
            else
            {
                flexCelPdfExport1.Workbook.Open(inputFile);
            }

            if (flexCelPdfExport1.Workbook == null)
            {
                System.Windows.Forms.MessageBox.Show("You need to open a file first.");
                return success;
            }

            //if (!LoadPreferencesPdf()) return success;
            if (outputStream != null)
            {
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(outputStream);

                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();

                    flexCelPdfExport1.EndExport();

                    success = true;
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
            }
            else if (!String.IsNullOrEmpty(outputFile))
            {
                using (FileStream Pdf = new FileStream(outputFile, FileMode.OpenOrCreate))
                {
                    int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                    try
                    {
                        flexCelPdfExport1.BeginExport(Pdf);

                        flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                        flexCelPdfExport1.ExportSheet();

                        flexCelPdfExport1.EndExport();

                        success = true;
                    }
                    finally
                    {
                        flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                    }
                }
            }

            return success;
        }

        public static bool ExcelToPdfUsingOffice(string inputFile, string outputFile)
        {
            bool success = false;

            if (String.IsNullOrEmpty(inputFile))
                throw new ArgumentNullException("inFile is null");

            if (String.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException("outFile is null");
            }

            if (!String.IsNullOrEmpty(outputFile))
            {
                using (FileStream Pdf = new FileStream(outputFile, FileMode.OpenOrCreate))
                {
                    success = ExportWorkbookToPdf(inputFile, outputFile);
                }
            }

            return success;
        }

        public static bool ExcelToPdf(MemoryStream inputStream, string inputFile, MemoryStream outputStream, string outputFile)
        {
            bool success = false;
            try
            {
                if ((inputStream == null || inputStream.Length == 0) && String.IsNullOrEmpty(inputFile))
                    throw new ArgumentNullException("inStream & inFile is null");

                if (outputStream == null && String.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException("outStream & outFile is null");
                }

                Workbook workbook = new Workbook();
                bool valid = false;
                if (inputStream != null && inputStream.Length > 0)
                {
                    string ext = Get.GetFileType(inputStream);
                    if (ext == ".xls")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xls);
                    }
                    else if (ext == ".xlsm")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xlsm);
                    }
                    else if (ext == ".xltx")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xltx);
                    }
                    else if (ext == ".xlt")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xlt);
                    }
                    else if (ext == ".xltm")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xltm);
                    }
                    else
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    }
                }
                else if (!String.IsNullOrEmpty(inputFile))
                {
                    string ext = Path.GetExtension(inputFile);
                    if (ext == ".xls")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xls);
                    }
                    else if (ext == ".xlsm")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xlsm);
                    }
                    else if (ext == ".xltx")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xltx);
                    }
                    else if (ext == ".xlt")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xlt);
                    }
                    else if (ext == ".xltm")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xltm);
                    }
                    else
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    }
                }
                if (valid)
                {
                    if (outputStream != null)
                    {
                        workbook.ExportToPdf(outputStream);
                        outputStream.Position = 0;
                    }
                    if (!String.IsNullOrEmpty(outputFile))
                    {
                        workbook.ExportToPdf(outputFile);
                    }
                    workbook.Dispose();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        public static bool ExcelToPdf(byte[] inputStream, string inputFile, ref byte[] outputStream, ref string outputFile)
        {
            bool success = false;
            try
            {
                if ((inputStream == null || inputStream.Length == 0) && String.IsNullOrEmpty(inputFile))
                    throw new ArgumentNullException("inStream & inFile is null");

                if (!String.IsNullOrEmpty(inputFile) && String.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException("outFile is null");
                }

                Workbook workbook = new Workbook();
                bool valid = false;
                if (inputStream != null && inputStream.Length > 0)
                {
                    string ext = Get.GetFileType(inputStream);
                    if (ext == ".xls")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xls);
                    }
                    else if (ext == ".xlsm")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xlsm);
                    }
                    else if (ext == ".xltx")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xltx);
                    }
                    else if (ext == ".xlt")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xlt);
                    }
                    else if (ext == ".xltm")
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.Xltm);
                    }
                    else
                    {
                        valid = workbook.LoadDocument(inputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    }
                }
                else if (!String.IsNullOrEmpty(inputFile))
                {
                    string ext = Path.GetExtension(inputFile);
                    if (ext == ".xls")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xls);
                    }
                    else if (ext == ".xlsm")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xlsm);
                    }
                    else if (ext == ".xltx")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xltx);
                    }
                    else if (ext == ".xlt")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xlt);
                    }
                    else if (ext == ".xltm")
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.Xltm);
                    }
                    else
                    {
                        valid = workbook.LoadDocument(inputFile, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    }
                }
                if (valid)
                {

                    if (!String.IsNullOrEmpty(outputFile))
                    {
                        workbook.ExportToPdf(outputFile);
                    }
                    else
                    {
                        MemoryStream stream = new MemoryStream();
                        workbook.ExportToPdf(stream);
                        stream.Position = 0;
                        outputStream = StreamToByte(stream);
                    }
                    workbook.Dispose();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        public static bool DocToPdf(MemoryStream inputStream, string inputFile, MemoryStream outputStream, string outputFile)
        {
            bool success = false;
            try
            {
                if ((inputStream == null || inputStream.Length == 0) && String.IsNullOrEmpty(inputFile))
                    throw new ArgumentNullException("inStream & inFile is null");

                if (outputStream == null && String.IsNullOrEmpty(outputFile))
                {
                    throw new ArgumentNullException("outStream & outFile is null");
                }

                RichEditDocumentServer server = new RichEditDocumentServer();
                if (!String.IsNullOrEmpty(inputFile))
                {
                    string ext = Path.GetExtension(inputFile);
                    if (ext == ".doc")
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                    }
                    else if (ext == ".html")
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.Html);
                    }
                    else if (ext == ".mht")
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.Mht);
                    }
                    else if (ext == ".txt")
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                    }
                    else if (ext == ".rtf")
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
                    }
                    else
                    {
                        server.LoadDocument(inputFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    }
                }
                else if (inputStream != null && inputStream.Length > 0)
                {
                    string ext = Get.GetFileType(inputStream);
                    if (ext == ".doc")
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                    }
                    else if (ext == ".html")
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Html);
                    }
                    else if (ext == ".mht")
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Mht);
                    }
                    else if (ext == ".txt")
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                    }
                    else if (ext == ".rtf")
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
                    }
                    else
                    {
                        server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    }
                }

                if (outputStream != null)
                {
                    server.ExportToPdf(outputStream);
                    outputStream.Position = 0;
                }
                if (!String.IsNullOrEmpty(outputFile))
                {
                    DevExpress.XtraPrinting.PdfExportOptions options = new DevExpress.XtraPrinting.PdfExportOptions();
                    options.Compressed = false;
                    options.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    using (FileStream pdfFileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        server.ExportToPdf(pdfFileStream, options);
                    }
                }

                server.Dispose();
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return success;
        }

        static bool ExportWorkbookToPdf(string workbookPath, string outputPath)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
                // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;

                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close();
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

            // You can use the following method to automatically open the PDF after export if you wish
            // Make sure that the file actually exists first...
            //if (System.IO.File.Exists(outputPath))
            //{
            //    System.Diagnostics.Process.Start(outputPath);
            //}

            return exportSuccessful;
        }

        internal static byte[] StreamToByte(Stream input)
        {
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

        public static byte[] FileToByte(string input)
        {
            return File.ReadAllBytes(input);
        }

        public static void ByteToFile(byte[] arrInFile, string saveFile)
        {
            File.WriteAllBytes(saveFile, arrInFile);
        }

        public static string ConvertRdlcToPdf(string rdlcFile)
        {
            string outPdfFile = Path.GetTempFileName();
            Microsoft.Reporting.WinForms.Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            Microsoft.Reporting.WinForms.ReportViewer reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();

            string outFP = "";
            reportViewer.LocalReport.ReportPath = rdlcFile;
            byte[] bytes = reportViewer.LocalReport.Render(
               "PDF", null, out mimeType, out encoding, out outFP,
               out streamIds, out warnings);

            using (FileStream fs = new FileStream(outPdfFile, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
            return outPdfFile;
        }
    }
}
