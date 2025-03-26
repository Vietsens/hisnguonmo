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
using Telerik.WinControls.UI;

namespace Inventec.Common.DocumentViewer.TelerikPdf
{
    public partial class FormTelerikPdf : Form
    {
        string url;
        int NumberOfCopy = 1;
        bool DeleteFile;
        bool IsPrintNow;

        string PrintPageSize { get; set; }
        public FormTelerikPdf(string url)
        {
            InitializeComponent();
            //this.url = url;
            //radPdfViewer1.LoadDocument(this.url);

            if (File.Exists(this.url))
            {
                Inventec.Common.Logging.LogSystem.Debug("FormTelerikPdf Load URL");
                radPdfViewer1.LoadDocument(this.url);
            }
            else
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("FormTelerikPdf Load Stream");
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                WebClient client = new WebClient();
                byte[] byteData = client.DownloadData(this.url);
                Stream ms = new MemoryStream(byteData);
                WaitingManager.Hide();
                radPdfViewer1.LoadDocument(ms);
            }
        }

        public FormTelerikPdf(InputADO data)
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex) { }

            if (data != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                this.url = data.URL;
                this.DeleteFile = data.DeleteWhenClose;
                if (data.NumberOfCopy.HasValue && data.NumberOfCopy.Value > 1)
                {
                    this.NumberOfCopy = data.NumberOfCopy.Value;
                }
                if (!string.IsNullOrEmpty(data.PrintPageSize))
                {
                    PrintPageSize = data.PrintPageSize;
                }
                if (File.Exists(this.url))
                {
                    Inventec.Common.Logging.LogSystem.Debug("FormTelerikPdf Load URL");
                    radPdfViewer1.LoadDocument(this.url);
                }
                else
                {
                    WaitingManager.Show();
                    Inventec.Common.Logging.LogSystem.Debug("FormTelerikPdf Load Stream");
                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    WebClient client = new WebClient();
                    byte[] byteData = client.DownloadData(this.url);
                    Stream ms = new MemoryStream(byteData);
                    WaitingManager.Hide();
                    radPdfViewer1.LoadDocument(ms);
                }
            }
        }

        public void PrintFile()
        {
            try
            {
                IsPrintNow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTelerikPdf_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.Copies = (short)NumberOfCopy;
                var kindEdnum = printerSettings.DefaultPageSettings.PaperSize.Kind;
                if (!string.IsNullOrEmpty(PrintPageSize) && System.Enum.IsDefined(kindEdnum.GetType(), PrintPageSize))
                {
                    var paperSize = printerSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault(o => o.PaperName == PrintPageSize);
                    printerSettings.DefaultPageSettings.PaperSize = paperSize;
                }
                printerSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                radPdfViewerNavigator1.PrintButton.Click += new EventHandler(PrintButton_Click);
                radPdfViewerNavigator1.PrintDocument.PrinterSettings = printerSettings;

                radPrintDocument1.PrinterSettings = printerSettings;
                Inventec.Common.Logging.LogSystem.Info("FormTelerikPdf_Load end");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintButton_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("PrintButton_Click");
                System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();
                //printDialog.AllowPrintToFile = true;
                printDialog.AllowSomePages = true;
                printDialog.PrinterSettings.MinimumPage = 1;
                printDialog.PrinterSettings.MaximumPage = radPdfViewer1.Document.Pages.Count;
                printDialog.PrinterSettings.FromPage = 1;
                printDialog.PrinterSettings.ToPage = radPdfViewer1.Document.Pages.Count;
                printDialog.PrinterSettings.Copies = (short)NumberOfCopy;
                var kindEdnum = printDialog.PrinterSettings.DefaultPageSettings.PaperSize.Kind;
                if (!string.IsNullOrEmpty(PrintPageSize) && System.Enum.IsDefined(kindEdnum.GetType(), PrintPageSize))
                {
                    var paperSize = printDialog.PrinterSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault(o => o.PaperName == PrintPageSize);
                    printDialog.PrinterSettings.DefaultPageSettings.PaperSize = paperSize;
                }
                printDialog.PrinterSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                Inventec.Common.Logging.LogSystem.Debug("PrintPageSize: "+ PrintPageSize + "___RawKind: " + printDialog.PrinterSettings.DefaultPageSettings.PaperSize.RawKind + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => printDialog.PrinterSettings), printDialog.PrinterSettings));
                if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RadPrintDocument radPrintDocument = new RadPrintDocument();
                    radPrintDocument.PrinterSettings = printDialog.PrinterSettings;
                    radPdfViewer1.PrintPreview(radPrintDocument);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTelerikPdf_FormClosed(object sender, FormClosedEventArgs e)
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

        private void radPdfViewer1_DocumentLoaded(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("radPdfViewer1_DocumentLoaded");
                if (IsPrintNow)
                {
                    PrinterSettings printerSettings = new PrinterSettings();
                    printerSettings.Copies = (short)NumberOfCopy;
                    var kindEdnum = printerSettings.DefaultPageSettings.PaperSize.Kind;
                    if (!string.IsNullOrEmpty(PrintPageSize) && System.Enum.IsDefined(kindEdnum.GetType(), PrintPageSize))
                    {
                        var paperSize = printerSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault(o => o.PaperName == PrintPageSize);
                        printerSettings.DefaultPageSettings.PaperSize = paperSize;
                    }
                    printerSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    radPrintDocument1.PrinterSettings = printerSettings;
                    radPdfViewer1.Print(false, radPrintDocument1);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
