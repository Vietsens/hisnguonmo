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

namespace Inventec.Common.FlexCelPrint.DocumentPrint
{
    class ThreadPrintPartialForPDF
    {
        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles;

        public ThreadPrintPartialForPDF(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles)
        {
            this.partialFiles = partialFiles;
        }

        public bool Print()
        {
            bool success = false;
            try
            {
                if (this.partialFiles != null && this.partialFiles.Count > 0)
                {
                    foreach (var data in this.partialFiles)
                    {
                        success = PrintOneFileForPdf(data);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
            return success;
        }

        private bool PrintOneFileForPdf(Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            bool success = false;
            try
            {
                if (data!=null && !String.IsNullOrEmpty(data.saveFilePath))
                {
                    Inventec.Common.Logging.LogSystem.Info("__data.saveFilePath: " + data.saveFilePath);
                    SimplePrint(data.saveFilePath);
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        internal static bool SimplePrint(string inputFile, int copyCount = 1, string printerName = "", System.Drawing.Printing.PaperSize paperSize = null)
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SimplePrint.1");
                Inventec.Common.FlexCelPrint.DocumentPrint.License.LicenceProcess.SetLicenseForAspose();

                // Create PdfViewer object
                Aspose.Pdf.Facades.PdfViewer viewer = new Aspose.Pdf.Facades.PdfViewer();

                // Open input PDF file
                viewer.BindPdf(inputFile);

                // Set attributes for printing
                viewer.AutoResize = true;         // Print the file with adjusted size
                viewer.AutoRotate = true;         // Print the file with adjusted rotation
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
                if (!String.IsNullOrEmpty(printerName))
                    printDialog.PrinterSettings.PrinterName = printerName;


                Aspose.Pdf.PageCollection pageCollection = pdfDocument.Pages;
                // Get particular page
                Aspose.Pdf.Page pdfPage = pageCollection[1];


                Aspose.Pdf.Facades.PdfPageEditor pageEditor = new Aspose.Pdf.Facades.PdfPageEditor();
                pageEditor.BindPdf(inputFile);

                ////// Set PageSize (if required)
                int iWidth = (int)(Math.Round(((pdfPage.Rect.Width * 100) / 72), 0, MidpointRounding.AwayFromZero));
                int iHeight = (int)(Math.Round(((pdfPage.Rect.Height * 100) / 72), 0, MidpointRounding.AwayFromZero));

                if (paperSize != null)
                {
                    if (String.IsNullOrEmpty(paperSize.PaperName))
                    {
                        paperSize.PaperName = paperSize.Kind.ToString();
                    }
                    printDialog.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                }
                else
                {
                    //A4	9	827	1169
                    //DEBUG 2021-07-26 20:39:03,775 [1] - ___iWidth:826______iHeight:583______paperSize:null___
                    //___iWidth:583______iHeight:826______paperSize:null___
                    //A5	11	583	827
                    if ((iWidth < iHeight && iWidth <= 585 && iHeight <= 827) || (iWidth > iHeight && iWidth <= 827 && iHeight <= 585))
                    {
                        System.Drawing.Printing.PaperSize PaperSizeA5 = new System.Drawing.Printing.PaperSize();
                        PaperSizeA5.RawKind = 11;

                        printDialog.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize(PaperSizeA5.Kind.ToString(), iWidth, iHeight);
                        printDialog.PrinterSettings.DefaultPageSettings.PaperSize.RawKind = PaperSizeA5.RawKind;

                        if (pageEditor.GetPageSize(1).IsLandscape)
                        {
                            pageEditor.Alignment = Aspose.Pdf.Facades.AlignmentType.Center;
                            viewer.AutoRotate = false;
                            viewer.AutoResize = false;
                        }
                    }
                    //else
                    //    pgs.PaperSize = new System.Drawing.Printing.PaperSize("Custom", iWidth, iHeight);
                }

                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ps = printDialog.PrinterSettings;


                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => iWidth), iWidth)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => iHeight), iHeight)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paperSize), paperSize));

                    if (pageEditor.GetPageSize(1).IsLandscape)
                    {
                        pgs.Landscape = true;
                    }

                    if (ps.DefaultPageSettings.PaperSize != null && ps.DefaultPageSettings.PaperSize.RawKind > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("paperSizeInput", ps.DefaultPageSettings.PaperSize));
                        if (String.IsNullOrEmpty(ps.DefaultPageSettings.PaperSize.PaperName))
                        {
                            ps.DefaultPageSettings.PaperSize.PaperName = ps.DefaultPageSettings.PaperSize.Kind.ToString();
                        }
                        pgs.PaperSize = ps.DefaultPageSettings.PaperSize;
                    }
                    else
                    {
                        //A4	9	827	1169
                        //DEBUG 2021-07-26 20:39:03,775 [1] - ___iWidth:826______iHeight:583______paperSize:null___
                        //___iWidth:583______iHeight:826______paperSize:null___
                        //A5	11	583	827
                        if ((iWidth < iHeight && iWidth <= 585 && iHeight <= 827) || (iWidth > iHeight && iWidth <= 827 && iHeight <= 585))
                        {
                            System.Drawing.Printing.PaperSize PaperSizeA5 = new System.Drawing.Printing.PaperSize();
                            PaperSizeA5.RawKind = 11;

                            pgs.PaperSize = new System.Drawing.Printing.PaperSize(PaperSizeA5.Kind.ToString(), iWidth, iHeight);
                            pgs.PaperSize.RawKind = PaperSizeA5.RawKind;

                            if (pageEditor.GetPageSize(1).IsLandscape)
                            {
                                pageEditor.Alignment = Aspose.Pdf.Facades.AlignmentType.Center;
                                viewer.AutoRotate = false;
                                viewer.AutoResize = false;
                            }
                        }
                        else
                            pgs.PaperSize = new System.Drawing.Printing.PaperSize("Custom", iWidth, iHeight);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("pageEditor.GetPageSize(1)", pageEditor.GetPageSize(1)));

                    pgs.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
                    // Specify the page size of printout
                    ps.DefaultPageSettings.PaperSize = pgs.PaperSize;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ps.DefaultPageSettings.PaperSize), ps.DefaultPageSettings.PaperSize)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pgs.Landscape), pgs.Landscape));

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
