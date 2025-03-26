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
using DevExpress.Pdf;
using Inventec.Common.DocumentViewer.Config;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.DocumentViewer.Template
{
    public partial class frmPdfViewer : Form
    {
        string url;
        int NumberOfCopy = 1;
        string PrintPageSize { get; set; }
        bool DeleteFile;
        Action actPrintSuccess;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "Inventec.Common.DocumentViewer";
        bool isLoadDefaultPrintSetup = false;
        string defaultPrinterName = null;
        PageRange defaultPageRange = PageRange.AllPages;
        enum PageRange
        {
            AllPages = 1,
            CurrentPage = 2,
            SomePages = 3,
        }

        public frmPdfViewer(string url)
        {
            try
            {
                InitializeComponent();
                this.url = url;
                if (File.Exists(this.url))
                {
                    pdfViewer1.DetachStreamAfterLoadComplete = true;
                    pdfViewer1.LoadDocument(this.url);
                }
                else
                {
                    WaitingManager.Show();
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    WebClient client = new WebClient();
                    byte[] byteData = client.DownloadData(this.url);
                    Stream ms = new MemoryStream(byteData);
                    WaitingManager.Hide();

                    pdfViewer1.LoadDocument(ms);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }           
        }

        public frmPdfViewer(string url, Action actPrintSuccess)
        {
            try
            {
                InitializeComponent();
                this.url = url;
                this.actPrintSuccess = actPrintSuccess;
                if (File.Exists(this.url))
                {
                    pdfViewer1.DetachStreamAfterLoadComplete = true;
                    pdfViewer1.LoadDocument(this.url);
                }
                else
                {
                    WebClient client = new WebClient();
                    byte[] byteData = client.DownloadData(this.url);
                    Stream ms = new MemoryStream(byteData);
                    pdfViewer1.LoadDocument(ms);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmPdfViewer(InputADO data)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

                if (data != null)
                {
                    this.url = data.URL;
                    this.DeleteFile = data.DeleteWhenClose;
                    this.actPrintSuccess = data.ActPrintSuccess;
                    if (data.NumberOfCopy.HasValue && data.NumberOfCopy.Value > 1)
                    {
                        this.NumberOfCopy = data.NumberOfCopy.Value;
                    }

                    if (!string.IsNullOrEmpty(data.PrintPageSize))
                    {
                        PrintPageSize = data.PrintPageSize;
                    }

                    WebClient client = new WebClient();
                    byte[] byteData = client.DownloadData(this.url);
                    MemoryStream ms = new MemoryStream(byteData);
                    pdfViewer1.LoadDocument(ms);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool isPrintingCanceled = true;
        public void PrintFile()
        {
            try
            {
                //Chèn đoạn code tạo hàm PrintPage cho pdfViewer1
                pdfViewer1.PrintPage += viewer_PrintPage;
                PdfPrinterSettings pdfPrinterSettings = new DevExpress.Pdf.PdfPrinterSettings();
                pdfPrinterSettings.Settings.Copies = (short)NumberOfCopy;
                var kindEdnum = pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize.Kind;
                if (!string.IsNullOrEmpty(PrintPageSize) && System.Enum.IsDefined(kindEdnum.GetType(), PrintPageSize))
                {
                    var paperSize = pdfPrinterSettings.Settings.PaperSizes.Cast<PaperSize>().FirstOrDefault(o => o.PaperName == PrintPageSize);
                    pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize = paperSize;
                }
                pdfPrinterSettings.Settings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                if (this.isLoadDefaultPrintSetup)
                {
                    if (PrinterSettings.InstalledPrinters != null)
                    {
                        foreach (String printer in PrinterSettings.InstalledPrinters)
                        {
                            if (this.defaultPrinterName == printer)
                            {
                                pdfPrinterSettings.Settings.PrinterName = this.defaultPrinterName;
                            }
                        }
                    }

                    if (this.defaultPageRange == PageRange.CurrentPage)
                    {
                        pdfPrinterSettings.Settings.PrintRange = PrintRange.CurrentPage;
                        pdfPrinterSettings.Settings.FromPage = pdfViewer1.CurrentPageNumber;
                        pdfPrinterSettings.Settings.ToPage = pdfViewer1.CurrentPageNumber;
                    }
                    else
                    {
                        pdfPrinterSettings.Settings.PrintRange = PrintRange.AllPages;
                        pdfPrinterSettings.Settings.FromPage = 0;
                        pdfPrinterSettings.Settings.ToPage = 0;
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("PrintPageSize: " + PrintPageSize + "___RawKind: " + pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize.RawKind + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pdfPrinterSettings.Settings), pdfPrinterSettings.Settings));
                pdfViewer1.Print(pdfPrinterSettings);


                if (!isPrintingCanceled)
                {
                    if (this.actPrintSuccess != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("this.actPrintSuccess != null: " + (this.actPrintSuccess != null));
                        try
                        {
                            this.actPrintSuccess();
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                    //call api update
                    pdfViewer1.PrintPage -= viewer_PrintPage;
                }
                isPrintingCanceled = true;

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PrintFile(string PrinterName, PrintRange PrintRange = PrintRange.AllPages, int FromPage = 0, int ToPage = 0)
        {
            try
            {
                //Chèn đoạn code tạo hàm PrintPage cho pdfViewer1
                pdfViewer1.PrintPage += viewer_PrintPage;

                PdfPrinterSettings pdfPrinterSettings = new DevExpress.Pdf.PdfPrinterSettings();
                if (PrinterSettings.InstalledPrinters != null)
                {
                    foreach (String printer in PrinterSettings.InstalledPrinters)
                    {
                        if (PrinterName == printer)
                        {
                            pdfPrinterSettings.Settings.PrinterName = PrinterName;
                        }
                    }
                }
                var kindEdnum = pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize.Kind;
                if (!string.IsNullOrEmpty(PrintPageSize) && System.Enum.IsDefined(kindEdnum.GetType(), PrintPageSize))
                {
                    var paperSize = pdfPrinterSettings.Settings.PaperSizes.Cast<PaperSize>().FirstOrDefault(o => o.PaperName == PrintPageSize);
                    pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize = paperSize;
                }
                pdfPrinterSettings.Settings.Copies = (short)NumberOfCopy;
                pdfPrinterSettings.Settings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                if (PrintRange == PrintRange.SomePages)
                {
                    pdfPrinterSettings.Settings.PrintRange = PrintRange.SomePages;
                    pdfPrinterSettings.Settings.FromPage = FromPage;
                    pdfPrinterSettings.Settings.ToPage = ToPage;
                }
                else if (PrintRange == PrintRange.CurrentPage)
                {
                    pdfPrinterSettings.Settings.PrintRange = PrintRange.CurrentPage;
                    pdfPrinterSettings.Settings.FromPage = pdfViewer1.CurrentPageNumber;
                    pdfPrinterSettings.Settings.ToPage = pdfViewer1.CurrentPageNumber;
                }
                else
                {
                    pdfPrinterSettings.Settings.PrintRange = PrintRange.AllPages;
                    pdfPrinterSettings.Settings.FromPage = 0;
                    pdfPrinterSettings.Settings.ToPage = 0;
                }

                Inventec.Common.Logging.LogSystem.Debug("PrintPageSize: " + PrintPageSize + "___RawKind: " + pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize.RawKind + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pdfPrinterSettings.Settings), pdfPrinterSettings.Settings));
                pdfViewer1.Print(pdfPrinterSettings);


                if (!isPrintingCanceled)
                {
                    if (this.actPrintSuccess != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("this.actPrintSuccess != null: " + (this.actPrintSuccess != null));
                        try
                        {
                            this.actPrintSuccess();
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                    //call api update
                    pdfViewer1.PrintPage -= viewer_PrintPage;
                }
                isPrintingCanceled = true;

                //this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void viewer_PrintPage(object sender, DevExpress.Pdf.PdfPrintPageEventArgs e)
        {
            isPrintingCanceled = false;
        }

        private void frmPdfViewer_Load(object sender, EventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //pdfViewer1.LoadDocument(this.url);
                //WaitingManager.Hide();
                LoadDefaultPrintSetup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDefaultPrintSetup()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    string pageRangeDefault = "";
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.PrinterName)
                        {
                            this.defaultPrinterName = item.VALUE;
                        }
                        else if (item.KEY == ControlStateConstant.PageRange)
                        {
                            pageRangeDefault = item.VALUE;
                        }
                    }
                    if (pageRangeDefault == ((int)PageRange.AllPages).ToString())
                        this.defaultPageRange = PageRange.AllPages;
                    else if (pageRangeDefault == ((int)PageRange.CurrentPage).ToString())
                        this.defaultPageRange = PageRange.CurrentPage;
                    else if (pageRangeDefault == ((int)PageRange.SomePages).ToString())
                        this.defaultPageRange = PageRange.AllPages;
                    else
                        this.defaultPageRange = PageRange.AllPages;
                }

                this.isLoadDefaultPrintSetup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "Pdf Files|*.pdf";

                var result = dialog.ShowDialog(); //shows save file dialog
                if (result == DialogResult.OK)
                {
                    var wClient = new WebClient();
                    wClient.DownloadFile(this.url, dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintNow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Info("btnPrintNow_ItemClick");
            PrintFile();
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Info("btnPrint_ItemClick");
            try
            {
                Print.frmSetupPrint frmSetupPrint = new Print.frmSetupPrint(PrintFile, this.pdfViewer1.PageCount);
                frmSetupPrint.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmPdfViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (DeleteFile && !String.IsNullOrWhiteSpace(this.url) && File.Exists(this.url))
                {
                    File.Delete(this.url);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
