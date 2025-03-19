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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.DocumentViewer.Print
{
    class PrintProcess
    {
        internal static bool SimplePrint(string inputFile, int copyCount = 1)
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SimplePrint.1");
                Inventec.Common.SignLibrary.License.LicenceProcess.SetLicenseForAspose();

                // Create PdfViewer object
                Aspose.Pdf.Facades.PdfViewer viewer = new Aspose.Pdf.Facades.PdfViewer();

                // Open input PDF file
                viewer.BindPdf(inputFile);

                // Set attributes for printing
                viewer.AutoResize = false;         // Print the file with adjusted size
                viewer.AutoRotate = false;         // Print the file with adjusted rotation
                viewer.PrintPageDialog = true;   // Do not produce the page number dialog when printing

                // Create objects for printer and page settings and PrintDocument
                System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
                System.Drawing.Printing.PageSettings pgs = new System.Drawing.Printing.PageSettings();
                System.Drawing.Printing.PrintDocument prtdoc = new System.Drawing.Printing.PrintDocument();

                // Set printer name
                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(inputFile);

                System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();
                //printDialog.AllowPrintToFile = true;
                printDialog.AllowSomePages = true;
                printDialog.PrinterSettings.MinimumPage = 1;
                printDialog.PrinterSettings.MaximumPage = viewer.PageCount;
                printDialog.PrinterSettings.FromPage = 1;
                printDialog.PrinterSettings.ToPage = viewer.PageCount;
                printDialog.PrinterSettings.Copies = copyCount > 0 ? (short)(copyCount) : (short)1;

                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ps = printDialog.PrinterSettings;
                    //ps.PrinterName = prtdoc.PrinterSettings.PrinterName;
                    //ps.PrinterName = "HP LaserJet 2300 Series PS";
                    // Set output file name and PrintToFile attribute
                    //ps.PrintFileName = "PdfToPostScript_out.pdf";
                    //ps.PrintToFile = true;

                    Aspose.Pdf.PageCollection pageCollection = pdfDocument.Pages;
                    // Get particular page
                    Aspose.Pdf.Page pdfPage = pageCollection[1];


                    ////// Set PageSize (if required)
                    int iWidth = (int)(Math.Round((pdfPage.Rect.Width * 100) / 72, 0, MidpointRounding.AwayFromZero));
                    int iHeight = (int)(Math.Round((pdfPage.Rect.Height * 100) / 72, 0, MidpointRounding.AwayFromZero));
                    ////pgs.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.Custom;                

                    pgs.PaperSize = new System.Drawing.Printing.PaperSize("Custom", iWidth, iHeight);
                    pgs.Landscape = iWidth > iHeight;//TODO

                    //// Set PageMargins (if required)

                    //if (margins != null)
                    //    pgs.Margins = margins;
                    //else
                    //pgs.Margins = new System.Drawing.Printing.Margins((int)(Math.Round((pdfDocument.PageInfo.Margin.Left) / 72, 0, MidpointRounding.AwayFromZero)), (int)(Math.Round((pdfDocument.PageInfo.Margin.Right) / 72, 0, MidpointRounding.AwayFromZero)), (int)(Math.Round((pdfDocument.PageInfo.Margin.Top) / 72, 0, MidpointRounding.AwayFromZero)), (int)(Math.Round((pdfDocument.PageInfo.Margin.Bottom) / 72, 0, MidpointRounding.AwayFromZero)));
                    pgs.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                    // Specify the page size of printout
                    ps.DefaultPageSettings.PaperSize = pgs.PaperSize;

                    // Document printing code goes here
                    // Print document using printer and page settings
                    viewer.PrintDocumentWithSettings(pgs, ps);
                    // Check the print status
                    if (viewer.PrintStatus != null)
                    {
                        // An exception was thrown
                        if (viewer.PrintStatus is Exception)
                        {
                            Exception ex = viewer.PrintStatus as Exception;
                            // Get exception message
                            Inventec.Common.Logging.LogSystem.Warn("In văn bản lỗi.", ex);
                            //System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        // No errors were found. Printing job has completed successfully
                        Console.WriteLine("printing completed without any issue..");
                        Inventec.Common.Logging.LogSystem.Debug("printing completed without any issue..");
                        success = true;
                    }
                }

                // Close the PDF file after priting
                viewer.Close();
                Inventec.Common.Logging.LogSystem.Debug("SimplePrint.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }
    }
}
