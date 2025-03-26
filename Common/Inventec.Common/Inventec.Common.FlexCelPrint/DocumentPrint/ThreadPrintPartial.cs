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
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCelPrint.DocumentPrint
{
    class ThreadPrintPartial
    {
        int page;
        int maxPage = -1;
        int pageCount = 0;
        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles;
        FlexCelPrintDocument flexCelPrintDocument1;

        public ThreadPrintPartial(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles, FlexCelPrintDocument flexCelPrintDocument1)
        {
            this.partialFiles = partialFiles;
            this.flexCelPrintDocument1 = flexCelPrintDocument1;
        }

        public bool Print()
        {
            bool success = false;
            if (this.partialFiles != null && this.partialFiles.Count > 0)
            {
                foreach (var data in this.partialFiles)
                {
                    if (data.ShowPrintLog != null)
                    {
                        string messagerError = "";
                        if (data.PrintLog(ref messagerError, null))
                        {
                            PrintOneFile(data);
                        }
                        else
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(messagerError);
                        }
                    }
                    else
                    {
                        PrintOneFile(data);
                    }
                                    
                    if (data.eventLog != null)
                    {
                        data.eventLog();
                    }

                    if (data.eventPrint != null)
                    {
                        data.eventPrint();
                    }


                    success = true;
                }
            }
            return success;
        }

        private void PrintOneFile(Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("PrintOneFile.1");
                pageCount = 0;
                page = 0;
                maxPage = -1;

                FlexCel.Render.FlexCelPrintDocument flexCelPrintDocumentPartial = new FlexCel.Render.FlexCelPrintDocument();
                flexCelPrintDocumentPartial.AllVisibleSheets = false;
                flexCelPrintDocumentPartial.ResetPageNumberOnEachSheet = true;
                flexCelPrintDocumentPartial.Workbook = null;
                flexCelPrintDocumentPartial.GetPrinterHardMargins += new FlexCel.Render.PrintHardMarginsEventHandler(this.flexCelPrintDocument1_GetPrinterHardMargins);
                flexCelPrintDocumentPartial.BeforePrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_BeforePrintPage);
                flexCelPrintDocumentPartial.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.flexCelPrintDocument1_BeginPrint);
                flexCelPrintDocumentPartial.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_PrintPage);

                flexCelPrintDocumentPartial.PrinterSettings.PrinterName = flexCelPrintDocument1.PrinterSettings.PrinterName;
                flexCelPrintDocumentPartial.DefaultPageSettings.PaperSize = flexCelPrintDocument1.DefaultPageSettings.PaperSize;
                flexCelPrintDocumentPartial.PrinterSettings.DefaultPageSettings.PaperSize = flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSize;
                flexCelPrintDocumentPartial.DefaultPageSettings.PaperSource = flexCelPrintDocument1.DefaultPageSettings.PaperSource;
                flexCelPrintDocumentPartial.PrinterSettings.DefaultPageSettings.PaperSource = flexCelPrintDocument1.PrinterSettings.DefaultPageSettings.PaperSource;
                flexCelPrintDocumentPartial.PrinterSettings.Copies = flexCelPrintDocument1.PrinterSettings.Copies;

                flexCelPrintDocumentPartial.Workbook = new XlsFile();
                if (data.saveMemoryStream != null && data.saveMemoryStream.Length > 0)
                {
                    data.saveMemoryStream.Position = 0;
                    flexCelPrintDocumentPartial.Workbook.Open(data.saveMemoryStream);
                }
                else if (!String.IsNullOrEmpty(data.saveFilePath))
                {
                    flexCelPrintDocumentPartial.Workbook.Open(data.saveFilePath);
                }

                //if (data.printerName != null)
                //{
                //    flexCelPrintDocumentPartial.PrinterSettings.PrinterName = data.printerName;
                //}

                DocumentPrint.ThreadPrint thred = new DocumentPrint.ThreadPrint(flexCelPrintDocumentPartial);
                thred.Print();
                Inventec.Common.Logging.LogSystem.Debug("PrintOneFile.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }         
        }

        [DllImport("gdi32.dll")]
        private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 capindex);

        /// <summary>
        /// This event will adjust for a better position on the page for some printers. 
        /// It is not normally necessary, and it has to make an unmanaged call to GetDeviceCaps,
        /// but it is given here as an example of how it could be done.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flexCelPrintDocument1_GetPrinterHardMargins(object sender, FlexCel.Render.PrintHardMarginsEventArgs e)
        {
            const int PHYSICALOFFSETX = 112;
            const int PHYSICALOFFSETY = 113;

            double DpiX = e.Graphics.DpiX;
            double DpiY = e.Graphics.DpiY;

            IntPtr Hdc = e.Graphics.GetHdc();
            try
            {
                e.XMargin = (float)(GetDeviceCaps(Hdc, PHYSICALOFFSETX) * 100.0 / DpiX);
                e.YMargin = (float)(GetDeviceCaps(Hdc, PHYSICALOFFSETY) * 100.0 / DpiY);
            }

            finally
            {
                e.Graphics.ReleaseHdc(Hdc);
            }
        }

        private void flexCelPrintDocument1_BeforePrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.Bicubic;
        }

        private void flexCelPrintDocument1_BeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_BeginPrint begin");
                if (maxPage == -1)//Print all page
                {
                    page = 1;
                }
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_BeginPrint end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void flexCelPrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_PrintPage begin");
                if (!e.HasMorePages)
                {
                    //spinChangePage.Properties.MaxValue = page;
                    //btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
                    //numericUpDownFromPage.Properties.MaxValue = page;
                    //numericUpDownToPage.Properties.MaxValue = page;
                    //lblTotalPage.Text = page + "";
                    pageCount = page;

                    //isFirstLoad = false;
                    //ActiveTopForm();
                }
                else
                {
                    if (maxPage > -1)
                    {
                        e.HasMorePages = (page <= pageCount && page < maxPage);
                    }
                    page++;
                }
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_PrintPage end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
    }
}
