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
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.Print
{
    public partial class frmPrintPreview : Form
    {
        public delegate void DelegateEventLog();

        Workbook workbook = new Workbook();
        Worksheet worksheet;
        DelegateEventLog eventLog;
        string defaultPrintName;
        string pathFileTemplate = "";
        int numCopy;

        PrintableComponentLink link;
        PrintingSystem printingSystem;

        public frmPrintPreview(MemoryStream data, DelegateEventLog eventLog, string defaultPrintName, string pathFileTemplate, int numCopy)
        {
            try
            {
                InitializeComponent();
                this.eventLog = eventLog;
                this.numCopy = numCopy;
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                InitMemo(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmPrintPreview(string path, DelegateEventLog eventLog, string pathFileTemplate, string defaultPrintName, int numCopy)
        {
            try
            {
                InitializeComponent();
                this.eventLog = eventLog;
                this.numCopy = numCopy;
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                InitPath(path);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMemo(MemoryStream data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("t1 - Start init print preview");
                data.Position = 0;
                workbook.LoadDocument(data, DocumentFormat.Xlsx);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitPath(string path)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("t1 - Start init print preview");
                workbook.LoadDocument(path);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmPrintPreview_Load(object sender, EventArgs e)
        {
            try
            {
                if (documentViewer1.PrintingSystem == null)
                {
                    printingSystem = new PrintingSystem();
                    link = new PrintableComponentLink(printingSystem);
                    printingSystem.Links.Add(link);
                    documentViewer1.DocumentSource = printingSystem;
                }
                else
                {
                    link = (documentViewer1.DocumentSource as PrintingSystem).Links[0] as PrintableComponentLink;
                }
                if (link != null)
                {
                    link.Component = workbook;
                    link.CreateDocument();
                }

                //ExcelFile Xls = flexCelPrintDocument1.Workbook;
                Inventec.Common.Logging.LogSystem.Debug("t2 - LoadSheetConfig");
                LoadSheetConfig();
                Inventec.Common.Logging.LogSystem.Debug("t3 - InitPrintPreviewControl");
                //InitPrintPreviewControl();
                Inventec.Common.Logging.LogSystem.Debug("t4 - End init print preview");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSheetConfig()
        {
            try
            {
                worksheet = workbook.Worksheets.ActiveWorksheet;
                WorksheetPrintOptions printOptions = worksheet.PrintOptions;
                printOptions.PrintGridlines = false;
                printOptions.PrintHeadings = false;
                printOptions.BlackAndWhite = false;

                ExportOptions options = printingSystem.ExportOptions;
                // Set Print Preview options.
                options.PrintPreview.ActionAfterExport = ActionAfterExport.AskUser;
                options.PrintPreview.DefaultDirectory = "C:\\Temp";
                options.PrintPreview.DefaultFileName = "Report";
                options.PrintPreview.SaveMode = SaveMode.UsingDefaultPath;
                options.PrintPreview.ShowOptionsBeforeExport = true;

                // Set E-mail options.
                options.Email.RecipientAddress = "someone@somewhere.com";
                options.Email.RecipientName = "Someone";
                options.Email.Subject = "Test";
                options.Email.Body = "Test";

                // Set CSV-specific export options.
                options.Csv.Encoding = Encoding.Unicode;
                options.Csv.Separator =
                    System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToString();

                // Set HTML-specific export options.
                options.Html.CharacterSet = "UTF-8";
                options.Html.RemoveSecondarySymbols = false;
                options.Html.Title = "Test Title";

                // Set Image-specific export options.
                options.Image.Format = System.Drawing.Imaging.ImageFormat.Jpeg;

                // Set MHT-specific export options.
                options.Mht.CharacterSet = "UTF-8";
                options.Mht.RemoveSecondarySymbols = false;
                options.Mht.Title = "Test Title";

                // Set PDF-specific export options.
                options.Pdf.Compressed = true;
                options.Pdf.ImageQuality = PdfJpegImageQuality.Low;
                options.Pdf.NeverEmbeddedFonts = "Tahoma;Courier New";
                options.Pdf.DocumentOptions.Application = "Test Application";
                options.Pdf.DocumentOptions.Author = "Test Team";
                options.Pdf.DocumentOptions.Keywords = "Test1, Test2";
                options.Pdf.DocumentOptions.Subject = "Test Subject";
                options.Pdf.DocumentOptions.Title = "Test Title";

                // Set Text-specific export options.
                options.Text.Encoding = Encoding.Unicode;
                options.Text.Separator =
                    System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator.ToString();

                // Set XLS-specific export options.
                options.Xls.ShowGridLines = false;
                options.Xls.SheetName = "Page 1";
                options.Xls.TextExportMode = TextExportMode.Value;

                // Set XLSX-specific export options.
                options.Xlsx.ShowGridLines = false;
                options.Xlsx.SheetName = "Page 1";
                options.Xlsx.TextExportMode = TextExportMode.Value;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(defaultPrintName))
                    link.Print(defaultPrintName);
                else
                    link.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(defaultPrintName))
                    link.Print(defaultPrintName);
                else
                {
                    link.PaperKind = link.PrintingSystemBase.PageSettings.PaperKind;
                    link.PaperName = link.PrintingSystemBase.PageSettings.PaperName;
                    link.Margins = link.PrintingSystemBase.PageMargins;
                    link.Landscape = link.PrintingSystemBase.PageSettings.Landscape;
                    link.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

    }
}
