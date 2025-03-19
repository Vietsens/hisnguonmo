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
using DevExpress.XtraEditors;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.FlexCelPrint.Ado;
using DevExpress.Pdf;

namespace Inventec.Common.FlexCelPrint
{

    /// <summary>
    /// Printing / Previewing and Exporting files to excel|pdf|image.
    /// </summary>
    public partial class frmSetupPrintPreviewMerge : DevExpress.XtraEditors.XtraForm
    {
        #region Variable
        DelegateEventLog eventLog;
        DelegateReturnEventPrint eventPrint;
        DelegatePrintLog PrintLog;
        DelegateShowPrintLog ShowPrintLog;
        Action ShowTutorialModule;
        private FlexCel.Render.FlexCelPrintDocument flexCelPrintDocument1;
        private FlexCel.Render.FlexCelPdfExport flexCelPdfExport1;
        bool chGridLines = false;
        bool chHeadings = false;
        bool chFormulaText = false;
        bool chPrintLeft = true;
        string edHeader = "";
        string edFooter = "";
        bool cbResetPageNumber = false;
        bool chAntiAlias = false;
        string edLeft = "";
        string edTop = "";
        string edRight = "";
        string edBottom = "";
        bool chFitIn = false;
        string edHPages = "";
        string edVPages = "";
        int edZoom = 100;
        double edl = 0;
        double edt = 0;
        double edr = 0;
        double edb = 0;
        double edf = 0;
        double edh = 0;

        bool printHCentered;
        bool printVCentered;

        bool landscape;
        bool blackAndWhite;

        int pageCount = 0;
        bool isFirstLoadPaged = false;

        int page;
        int maxPage = -1;
        bool isFirstLoad = true;

        private System.Drawing.Drawing2D.GraphicsPath mousePath;

        private string defaultPrintName;
        string pathFileTemplate = "";
        string pathFileResult = "";

        bool isPreview = false;
        bool isAllowExport = false;
        bool isAllowEditTemplateFile = true;
        bool isPdfOnly = false;
        bool isHasPdfFiles = false;
        public enum PrintSystem
        {
            Flexcel,
            Excel
        }

        Dictionary<string, object> TemplateKey;

        Inventec.Common.SignLibrary.ADO.InputADO emrInputADO;
        byte[] emrByteInputFile;
        const string SignTagFormat = "$$SIGN$$.{0}";
        const string SignTagSigned = "SIGNED";

        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles_Excel;
        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles_Pdf;
        PaperSize currentPaperSize;

        List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> selectedPartialFiles_Excel;
        PrintMergeAdo firstFile;
        EnumButton currentOnclickButton;

        enum EnumButton
        {
            Null,
            btnExportAsExcel,
            btnExportAsPdf,
            btnExportAsImage,
            btnTemplateKey,
            btnOpenFileTemplate,
            BtnPrintLog,
        }
        #endregion

        #region Construction
        public frmSetupPrintPreviewMerge()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("1. InitializeComponent");
                InitializeComponent();
                CheckSizePreview();
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Debug("2. print temp");
                Inventec.Common.Logging.LogSystem.Debug("3. Initialize memo");
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("No printers are installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("No printers are installed");
                }

                this.isPreview = false;
                this.isAllowExport = false;
                if (this.isPreview) UpdateItemsDisable();
                Inventec.Common.Logging.LogSystem.Debug("end InitializeComponent");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckSizePreview()
        {
            try
            {
                //neu cao hon man hinh se thu nho lai vua nhin
                if (this.Height >= SystemInformation.WorkingArea.Size.Height)
                {
                    this.Height = SystemInformation.WorkingArea.Size.Height - 30;
                }

                if (this.Width >= SystemInformation.WorkingArea.Size.Width)
                {
                    this.Width = SystemInformation.WorkingArea.Size.Width - 10;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmSetupPrintPreviewMerge(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isSingleCopy)
        {
            try
            {
                InitializeComponent();
                SetCaptionByLanguageKey();
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("No printers are installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("No printers are installed");
                }
                if (path != null && path.Length > 0)
                    InitPath(path);

                numericUpDownCopies.Value = numCopy;
                this.isPreview = isPreview;
                this.isAllowExport = isallowExport;
                if (this.isPreview) UpdateItemsDisable();
                this.TemplateKey = templateKey;
                this.eventLog = eventLog;
                this.eventPrint = eventPrint;
                this.emrInputADO = emrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                this.PrintLog = printLog;
                this.ShowPrintLog = showLog;

                if (isSingleCopy)
                {
                    numericUpDownCopies.Value = 1;
                    numericUpDownCopies.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetPartialFileList(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> partialFiles)
        {
            try
            {
                this.partialFiles_Excel = new List<PrintMergeAdo>();
                this.partialFiles_Pdf = new List<PrintMergeAdo>();

                foreach (var item in partialFiles)
                {
                    if (item.IsPdfFile)
                    {
                        this.partialFiles_Pdf.Add(item);
                    }
                    else
                    {
                        this.partialFiles_Excel.Add(item);
                    }
                }

                if (this.partialFiles_Pdf != null && this.partialFiles_Pdf.Count() > 0)
                {
                    this.isHasPdfFiles = true;
                }
                if (this.isHasPdfFiles && (this.partialFiles_Excel == null || this.partialFiles_Excel.Count() == 0))
                {
                    this.isPdfOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetTutorialModule(Action _showTutorialModule)
        {
            this.ShowTutorialModule = _showTutorialModule;
        }

        void SaveMemoryStream(MemoryStream ms, string FileName)
        {
            FileStream outStream = File.OpenWrite(FileName);
            ms.WriteTo(outStream);
            outStream.Flush();
            outStream.Close();
        }

        private void InitMemo(MemoryStream data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("t1 - Start init print preview");

                data.Position = 0;
                flexCelPrintDocument1.Workbook = new XlsFile();
                flexCelPrintDocument1.Workbook.Open(data);

                Inventec.Common.Logging.LogSystem.Debug("t2 - LoadSheetConfig");
                LoadSheetConfig();
                Inventec.Common.Logging.LogSystem.Debug("t3 - InitPrintPreviewControl");
                InitPrintPreviewControl();
                Inventec.Common.Logging.LogSystem.Debug("t4 - End init print preview");
                ChkHidePrinting.Checked = FlexCelPrintUtil.GetConfigValue();
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
                this.pathFileResult = path;
                if (this.partialFiles_Excel == null || this.partialFiles_Excel.Count == 0)
                {
                    flexCelPrintDocument1.Workbook = new XlsFile();
                    flexCelPrintDocument1.Workbook.Open(this.pathFileResult);
                    Inventec.Common.Logging.LogSystem.Debug("t2 - LoadSheetConfig");
                    LoadSheetConfig();
                    Inventec.Common.Logging.LogSystem.Debug("t3 - InitPrintPreviewControl");
                    InitPrintPreviewControl();
                    ChkHidePrinting.Checked = FlexCelPrintUtil.GetConfigValue();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateItemsDisable()
        {
            //bool IsEdit = !(ActionType == GlobalVariables.ActionView);
            if (!layoutControl3.IsInitialized) return;
            layoutControl3.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl3.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null)
                    {
                        lci.Enabled = false;
                    }
                }
            }
            finally
            {
                layoutControl3.EndUpdate();
            }
        }
        #endregion

        #region Onload
        private void frmSetupPrintPreviewMerge_Load(object sender, EventArgs e)
        {
            try
            {
                AlignFontSizeForControls();
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreviewMerge_Load.1");
                Inventec.Common.Logging.LogSystem.Debug("partialFiles_Excel.Count=" + (partialFiles_Excel != null ? partialFiles_Excel.Count : 0));
                Inventec.Common.Logging.LogSystem.Debug("partialFiles_Pdf.Count=" + (partialFiles_Pdf != null ? partialFiles_Pdf.Count : 0));
                if ((partialFiles_Excel != null && partialFiles_Excel.Count > 0) || (partialFiles_Pdf != null && partialFiles_Pdf.Count > 0))
                {
                    List<PrintMergeAdo> partialFiles = new List<PrintMergeAdo>();
                    if (partialFiles_Excel != null)
                        partialFiles.AddRange(partialFiles_Excel);
                    if (partialFiles_Pdf != null)
                        partialFiles.AddRange(partialFiles_Pdf);

                    string mergePdf = Utils.GenerateTempFileWithin();
                    InsertPage(partialFiles, mergePdf);
                    if (File.Exists(mergePdf))
                    {
                        this.pdfViewer1.DetachStreamAfterLoadComplete = true;
                        this.pdfViewer1.LoadDocument(mergePdf);
                    }
                }

                numericUpDownCopies.Focus();
                numericUpDownCopies.SelectAll();

                btnEmr.Enabled = true;
                //btnOpenFileTemplate.Enabled = false;
                //btnOpenFileTemplate.ToolTip = pathFileTemplate;
                //lciForbtnOpenFileTemplate.OptionsToolTip.ToolTip = pathFileTemplate;

                this.pdfViewer1.NavigationPaneInitialVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Hidden;
                this.pdfViewer1.NavigationPaneVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Hidden;
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreviewMerge_Load.2");
                if (this.partialFiles_Excel != null && this.partialFiles_Excel.Count > 0)
                {
                    this.firstFile = this.partialFiles_Excel.FirstOrDefault();
                    if (this.firstFile != null)
                    {
                        this.isAllowExport = firstFile.isAllowExport;
                        this.isAllowEditTemplateFile = firstFile.IsAllowEditTemplateFile;
                        if (firstFile.isAllowExport)
                        {
                            btnExportAsExcel.Enabled = true;
                            btnExportAsImage.Enabled = true;
                            btnExportAsPdf.Enabled = true;
                        }
                        else
                        {
                            btnExportAsExcel.Enabled = false;
                            btnExportAsImage.Enabled = false;
                            btnExportAsPdf.Enabled = false;
                        }
                        //if (firstFile.ShowPrintLog !=null)
                        //{
                        //    this.ShowPrintLog = firstFile.ShowPrintLog;
                        //    BtnPrintLog.Enabled = true;
                        //}
                        //else
                        //{
                        //    BtnPrintLog.Enabled = false;
                        //}
                        if (firstFile.PrintLog != null)
                        {
                            this.PrintLog = firstFile.PrintLog;
                        }
                        if (firstFile.eventLog != null)
                        {
                            this.eventLog = firstFile.eventLog;
                        }
                        if (firstFile.eventPrint != null)
                        {
                            this.eventPrint = firstFile.eventPrint;
                        }

                        if (firstFile.saveMemoryStream != null && firstFile.saveMemoryStream.Length > 0)
                        {
                            InitMemo(firstFile.saveMemoryStream);
                        }
                        else if (!String.IsNullOrEmpty(firstFile.saveFilePath))
                        {
                            InitPath(firstFile.saveFilePath);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreviewMerge_Load.End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AlignFontSizeForControls()
        {
            try
            {
                var formFontSize = this.Font.Size;
                if (formFontSize == 8.75F)
                {
                    this.btnTemplateKey.Font = new System.Drawing.Font("Tahoma", 8.75F);
                }
                else if (formFontSize == 9.25F)
                {
                    this.btnTemplateKey.Font = new System.Drawing.Font("Tahoma", 8.75F);
                }
                else if (formFontSize == 9.75F)
                {
                    this.btnTemplateKey.Font = new System.Drawing.Font("Tahoma", 9.00F);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void InsertPage(List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> fileListJoin, string desFileJoined)
        {
            try
            {
                List<MemoryStream> joinStreams = new List<MemoryStream>();
                if (fileListJoin != null && fileListJoin.Count > 0)
                {
                    foreach (var item in fileListJoin)
                    {
                        MemoryStream fileStream = new MemoryStream();
                        if (item.saveMemoryStream != null && item.saveMemoryStream.Length > 0)
                        {
                            item.saveMemoryStream.Position = 0;
                            if (item.IsPdfFile)
                            {
                                string fileTemp1 = Utils.GenerateTempFileWithin(".pdf");
                                if (!String.IsNullOrEmpty(item.saveFilePath))
                                {
                                    fileTemp1 = item.saveFilePath;
                                }
                                else
                                {
                                    File.WriteAllBytes(fileTemp1, Inventec.Common.SignLibrary.Utils.StreamToByte(item.saveMemoryStream));
                                    item.saveFilePath = fileTemp1;
                                }
                                fileStream = new MemoryStream(Inventec.Common.SignLibrary.Utils.FileToByte(fileTemp1));
                                fileStream.Position = 0;
                                item.saveMemoryStream.Position = 0;
                                joinStreams.Add(fileStream);
                            }
                            else
                            {
                                string fileTemp = Utils.GenerateTempFileWithin(".xlsx");
                                File.WriteAllBytes(fileTemp, Inventec.Common.SignLibrary.Utils.StreamToByte(item.saveMemoryStream));
                                FlexCelPrintUtil.ConvertExcelToPdfByFlexCel(fileTemp, fileStream);
                                if (fileStream != null && fileStream.Length > 0)
                                {
                                    fileStream.Position = 0;
                                    joinStreams.Add(fileStream);
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(item.saveFilePath) && File.Exists(item.saveFilePath))
                        {
                            FlexCelPrintUtil.ConvertExcelToPdfByFlexCel(item.saveFilePath, fileStream);
                            if (fileStream != null && fileStream.Length > 0)
                            {
                                fileStream.Position = 0;
                                joinStreams.Add(fileStream);
                            }
                        }
                    }

                    Inventec.Common.SignLibrary.PdfDocumentProcess.InsertPageExt(joinStreams, desFileJoined);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSetupPrintPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if ((e.X >= panel2.Location.X && e.X <= (panel2.Location.X + panel2.Width)) &&
                    (e.Y >= panel2.Location.Y && e.Y <= (panel2.Location.Y + panel2.Height)))
                {
                    if (e.Delta < 0)
                    {
                        FlexCelPrintUtil.SendMessage(printPreviewControl1.Handle, FlexCelPrintUtil.WM_VSCROLL, (IntPtr)FlexCelPrintUtil.SB_PAGEDOWN, IntPtr.Zero);
                    }
                    else
                    {
                        FlexCelPrintUtil.SendMessage(printPreviewControl1.Handle, FlexCelPrintUtil.WM_VSCROLL, (IntPtr)FlexCelPrintUtil.SB_PAGEUP, IntPtr.Zero);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSetupPrintPreview_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Update the drawing based upon the mouse wheel scrolling.

                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_MouseMove begin");

                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
                int numberOfPixelsToMove = numberOfTextLinesToMove;

                if (numberOfPixelsToMove != 0)
                {
                    System.Drawing.Drawing2D.Matrix translateMatrix = new System.Drawing.Drawing2D.Matrix();
                    translateMatrix.Translate(0, numberOfPixelsToMove);
                    mousePath.Transform(translateMatrix);
                }
                printPreviewControl1.Invalidate();


                //if (e.Delta > 0)
                //{
                //    trackBarZoomPage.Value += 10;
                //}
                //else
                //{
                //    trackBarZoomPage.Value -= 10;
                //}
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_MouseMove end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message message, System.Windows.Forms.Keys keys)
        {
            try
            {
                switch (keys)
                {
                    //case Keys.Down:
                    //    spinChangePage.Focus();
                    //    break;
                    //case Keys.Up:
                    //    spinChangePage.Focus();
                    //    break;
                    //case Keys.VolumeUp:
                    //    spinChangePage.Focus();
                    //    break;
                    //case Keys.VolumeDown:
                    //    spinChangePage.Focus();
                    //    break;
                    //case Keys.NumPad8:
                    //    spinChangePage.Focus();
                    //    break;
                    //case Keys.NumPad2:
                    //    spinChangePage.Focus();
                    //    break;
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }

            return base.ProcessCmdKey(ref message, keys);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            try
            {
                base.OnLostFocus(e);
                this.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ActiveTopForm()
        {
            try
            {
                Form active = null;
                var a = Application.OpenForms.Cast<Form>().ToList();//.First(x => x.Focused);
                if (a != null && a.Count > 0)
                {
                    for (int i = (Application.OpenForms.Count - 1); i >= 0; i--)
                    {
                        if (Application.OpenForms[i].Name == "frmWaitForm" || String.IsNullOrEmpty(Application.OpenForms[i].Name)) continue;
                        //if (Application.OpenForms[i] == this) continue;

                        active = Application.OpenForms[i];
                        break;
                    }
                }


                if (active != null)
                {
                    active.Activate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__FORM");
                this.btnExportAsExcel.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_EXPORT_AS_EXCEL");
                this.btnExportAsImage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_EXPORT_AS_IMAGE");
                this.btnExportAsPdf.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_EXPORT_AS_PDF");
                this.btnOpenFileTemplate.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_OPEN_FILE_TEMPLATE");
                this.btnPageSetup.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_PAGE_SETUP");
                this.btnPrint.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_PRINT");
                this.btnPrintSetup.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_PRINT_SETUP");
                this.btnTemplateKey.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__BTN_TEMPLATE_KEY");
                this.chkHorizontally.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__CHK_HORIZONTALLY");
                this.chkAllPages.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__CHK_ALL_PAGES");
                this.chkCurrentPage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__CHK_CURRENT_PAGE");
                this.chkPages.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__CHK_PAGES");
                this.chkVertically.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__CHK_VERTICALLY");
                //this.lblOf.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LBL_OF");
                this.lcgCenterOnPage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_CENTER_ON_PAGE");
                this.lcgPageRange.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAGE_RANGE");
                this.lcgPaperOrientation.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAPER_ORIENTATION");
                this.lcgPaperProperties.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAPER_PROPERTIES");
                this.lciAvailablePrinters.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_AVAILABLE_PRINTERS");
                this.lciBlackAndWhite.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_BLACK_AND_WHITE");
                this.lciNumericUpDownCopies.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_NUMERIC_UP_DOWN_COPIES");
                //this.lciPage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_PAGE");
                this.lciSize.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_SIZE");
                this.lciSource.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_SOURCE");
                this.lciZoom.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_ZOOM");
                this.rdLandscape.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__RD_LANDSCAPE");
                this.rdPartrain.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__RD_PARTRAIN");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetResourceMessage(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceMessage.languageMessage, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        #endregion

        #region Button click
        private void btnOpenTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ShowTutorialModule != null)
                {
                    ShowTutorialModule();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("ShowTutorialModule null");
                    MessageBox.Show("Không tìm thấy file hướng dẫn sử dụng và chỉnh sửa biểu mẫu");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnPrintSetup_Click(object sender, EventArgs e)
        {
            try
            {
                DoSetup();
                //LoadSheetConfig();
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnExportAsImage_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.btnExportAsImage;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnExportAsExcel_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.btnExportAsExcel;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnExportAsPdf_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.btnExportAsPdf;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ExportToPdf(FlexCel.Render.FlexCelPrintDocument flexCelPrintDocument1, string outFile)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                flexCelPrintDocument1.Workbook.Save(stream);
                stream.Position = 0;

                this.flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();
                this.flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                this.flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                this.flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                this.flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new XlsFile();
                flexCelPdfExport1.Workbook.Open(stream);

                if (flexCelPdfExport1.Workbook == null)
                {
                    MessageBox.Show("You need to open a file first.");
                    return;
                }

                //if (!LoadPreferencesPdf()) return;

                using (FileStream Pdf = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                    try
                    {
                        flexCelPdfExport1.BeginExport(Pdf);

                        flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                        flexCelPdfExport1.ExportSheet();

                        flexCelPdfExport1.EndExport();
                    }
                    finally
                    {
                        flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled) return;
                Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void chkPages_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                numericUpDownFromPage.Enabled = numericUpDownToPage.Enabled = chkPages.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnPageSetup_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                pd.AllowSomePages = true;
                pd.AllowCurrentPage = true;
                pd.AllowSelection = true;
                pd.PrinterSettings = this.flexCelPrintDocument1.PrinterSettings;
                if (chkPages.Checked)
                {
                    pd.PrinterSettings.PrintRange = PrintRange.SomePages;
                    pd.PrinterSettings.FromPage = 1;
                    if (numericUpDownFromPage.EditValue != null)
                        pd.PrinterSettings.ToPage = (int)numericUpDownFromPage.Value;
                    else
                        pd.PrinterSettings.ToPage = 1;
                    pd.PrinterSettings.MinimumPage = 1;
                    if (numericUpDownToPage.EditValue != null)
                        pd.PrinterSettings.MaximumPage = (int)numericUpDownToPage.Value;
                }
                else if (chkAllPages.Checked)
                {
                    pd.PrinterSettings.PrintRange = PrintRange.AllPages;
                }
                else if (chkCurrentPage.Checked)
                {
                    pd.PrinterSettings.PrintRange = PrintRange.CurrentPage;
                }

                if (pd.ShowDialog() == DialogResult.OK)
                {
                    flexCelPrintDocument1.PrinterSettings = pd.PrinterSettings;

                    if (pd.PrinterSettings.PrintRange == PrintRange.SomePages)
                    {
                        chkPages.Checked = true;
                        numericUpDownFromPage.EditValue = pd.PrinterSettings.FromPage;
                        numericUpDownToPage.EditValue = pd.PrinterSettings.ToPage;

                        page = this.flexCelPrintDocument1.PrinterSettings.FromPage;
                        maxPage = this.flexCelPrintDocument1.PrinterSettings.ToPage;
                    }
                    else if (pd.PrinterSettings.PrintRange == PrintRange.CurrentPage)
                    {
                        chkCurrentPage.Checked = true;
                        numericUpDownFromPage.Enabled = numericUpDownToPage.Enabled = false;
                        //maxPage = (int)spinChangePage.Value;
                        //page = (int)spinChangePage.Value;
                    }
                    else// if (pd.PrinterSettings.PrintRange == PrintRange.AllPages)
                    {
                        chkAllPages.Checked = true;
                        numericUpDownFromPage.Enabled = numericUpDownToPage.Enabled = false;
                        maxPage = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void bbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnOpenFileTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.btnOpenFileTemplate;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.btnTemplateKey;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEmr_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.partialFiles_Excel != null && this.partialFiles_Excel.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("btnEmr_Click.1");

                    foreach (var data in partialFiles_Excel)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("btnEmr_Click.1.1");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data.printTypeCode), data.printTypeCode));
                        FlexCel.Render.FlexCelPrintDocument flexCelPrintDocumentPartial = new FlexCel.Render.FlexCelPrintDocument();
                        flexCelPrintDocumentPartial.AllVisibleSheets = false;
                        flexCelPrintDocumentPartial.ResetPageNumberOnEachSheet = true;
                        flexCelPrintDocumentPartial.Workbook = null;
                        flexCelPrintDocumentPartial.GetPrinterHardMargins += new FlexCel.Render.PrintHardMarginsEventHandler(this.flexCelPrintDocument1_GetPrinterHardMargins);
                        flexCelPrintDocumentPartial.BeforePrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_BeforePrintPage);
                        flexCelPrintDocumentPartial.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.flexCelPrintDocument1_BeginPrint);
                        flexCelPrintDocumentPartial.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_PrintPage);
                        Inventec.Common.Logging.LogSystem.Debug("btnEmr_Click.1.2");
                        flexCelPrintDocumentPartial.Workbook = new XlsFile();
                        if (data.saveMemoryStream != null && data.saveMemoryStream.Length > 0)
                        {
                            data.saveMemoryStream.Position = 0;
                            flexCelPrintDocumentPartial.Workbook.Open(data.saveMemoryStream);
                        }
                        else if (!String.IsNullOrEmpty(data.saveFilePath) && File.Exists(data.saveFilePath))
                        {
                            flexCelPrintDocumentPartial.Workbook.Open(data.saveFilePath);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("btnEmr_Click.1.3");
                        SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                        string fileTemp = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
                        ExportToPdf(flexCelPrintDocumentPartial, fileTemp);
                        if (File.Exists(fileTemp))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileTemp), fileTemp));
                            Inventec.Common.SignLibrary.ADO.InputADO emrInputADO = data.EmrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                            if (emrInputADO != null)
                            {
                                emrInputADO.PrintNumberCopies = (short)(data.numCopy);
                                if (emrInputADO.PaperSizeDefault == null)
                                {
                                    TPaperDimensions t1 = flexCelPrintDocumentPartial.Workbook.PrintPaperDimensions;
                                    currentPaperSize = new PaperSize(t1.PaperName, System.Convert.ToInt32(t1.Width), System.Convert.ToInt32(t1.Height));
                                    emrInputADO.PaperSizeDefault = currentPaperSize;
                                }
                            }
                            else
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("emrInputADO.HisCode", emrInputADO != null ? emrInputADO.HisCode : ""));
                            libraryProcessor.ShowPopup(fileTemp, emrInputADO);
                            try
                            {
                                File.Delete(fileTemp);
                            }
                            catch { }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug("btnEmr_Click.2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrintLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnPrintLog.Enabled) return;

                this.currentOnclickButton = EnumButton.BtnPrintLog;
                popupControlContainer1.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void btnPrePage_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (spinChangePage.EditValue == null)
        //        {
        //            spinChangePage.Value = 1;
        //            btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
        //            btnPrePage.Enabled = false;
        //        }
        //        else
        //        {
        //            if (spinChangePage.Value == 1)
        //            {
        //                return;
        //            }
        //            spinChangePage.Value = spinChangePage.Value - 1;
        //            btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
        //        }
        //        printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}

        //private void btnNextPage_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (spinChangePage.EditValue == null)
        //        {
        //            spinChangePage.Value = 1;
        //            btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
        //            btnPrePage.Enabled = false;
        //        }
        //        else
        //        {
        //            if (spinChangePage.Value == spinChangePage.Properties.MaxValue)
        //            {
        //                return;
        //            }
        //            spinChangePage.Value = spinChangePage.Value + 1;
        //            btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
        //            btnNextPage.Enabled = (spinChangePage.Properties.MaxValue > 1 && spinChangePage.Value < spinChangePage.Properties.MaxValue);
        //        }
        //        printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}
        #endregion

        #region Event
        private void rdPartrain_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (landscape)
                {
                    spinZoom.Value = (int)(edZoom * 0.7);
                }
                else
                {
                    spinZoom.Value = edZoom;
                }
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void rdLandscape_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!landscape)
                {
                    spinZoom.Value = (int)(edZoom * 1.4);
                }
                else
                {
                    spinZoom.Value = edZoom;
                }
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void chkHorizontally_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkVertically_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void flexCelPrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_PrintPage begin");
                //if (!e.HasMorePages)
                //{
                //    //spinChangePage.Properties.MaxValue = page;
                //    //btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
                //    numericUpDownFromPage.Properties.MaxValue = page;
                //    numericUpDownToPage.Properties.MaxValue = page;
                //    //lblTotalPage.Text = page + "";
                //    pageCount = page;

                //    isFirstLoad = false;
                //    ActiveTopForm();
                //}
                //else
                //{
                //    if (maxPage > -1)
                //    {
                //        e.HasMorePages = (page <= pageCount && page < maxPage);
                //    }
                //    page++;
                //}
                Inventec.Common.Logging.LogSystem.Debug("flexCelPrintDocument1_PrintPage end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
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

        private void spinChangePage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (spinChangePage.EditValue != null)
                //{
                //    printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;

                //    btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
                //    btnNextPage.Enabled = (spinChangePage.Properties.MaxValue > 1 && spinChangePage.Value < spinChangePage.Properties.MaxValue);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        //private void trackBarZoomPage_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //ExcelFile Xls = flexCelPrintDocument1.Workbook;
        //        //Xls.PrintScale = trackBarZoomPage.Value;

        //        printPreviewControl1.Zoom = (double)((double)(trackBarZoomPage.Value) / (double)100);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}

        private void cboPaperSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridLookUpEdit gridCheckMark = sender as GridLookUpEdit;
                if (gridCheckMark == null) return;

                var item = ((List<PaperSize>)cboPaperSize.Properties.DataSource).FirstOrDefault(o => o.Kind.ToString() == (e.Value ?? "").ToString());
                if (item != null)
                    sb.Append(string.Format("{0}", item.PaperName));
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinZoom_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitPrintPreviewControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPaperSize_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPaperSize.EditValue != null)
                {
                    TPaperDimensions t = flexCelPrintDocument1.Workbook.PrintPaperDimensions;
                    PaperSize oldPsize = new PaperSize(t.PaperName, System.Convert.ToInt32(t.Width), System.Convert.ToInt32(t.Height));

                    InitPrintPreviewControl();

                    TPaperDimensions t1 = flexCelPrintDocument1.Workbook.PrintPaperDimensions;
                    currentPaperSize = new PaperSize(t1.PaperName, System.Convert.ToInt32(t1.Width), System.Convert.ToInt32(t1.Height));
                    decimal zo = (decimal)((decimal)((decimal)currentPaperSize.Width / (decimal)oldPsize.Width) * spinZoom.Value);
                    //MessageBox.Show("oldPsize.Width=" + oldPsize.Width + ", newPsize1.Width = " + newPsize1.Width + ", OldZoom = " + spinZoom.Value + ", NewZoom = " + zo);
                    spinZoom.EditValue = zo;
                    spinZoom.Update();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void ChkHidePrinting_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FlexCelPrintUtil.SetConfigValue(ChkHidePrinting.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void GetSelectedPartialFiles()
        {
            try
            {
                this.selectedPartialFiles_Excel = new List<Ado.PrintMergeAdo>();
                int[] selectRows = gridViewPopupMenu.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        this.selectedPartialFiles_Excel.Add((PrintMergeAdo)gridViewPopupMenu.GetRow(selectRows[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnOKPopupMenu_Click(object sender, EventArgs e)
        {
            try
            {
                switch (this.currentOnclickButton)
                {
                    case EnumButton.btnExportAsExcel:
                        ExportAsExcelProcess();
                        break;
                    case EnumButton.btnExportAsPdf:
                        ExportAsPdfProcess();
                        break;
                    case EnumButton.btnExportAsImage:
                        ExportAsImageProcess();
                        break;
                    case EnumButton.btnTemplateKey:
                        TemplateKeyProcess();
                        break;
                    case EnumButton.btnOpenFileTemplate:
                        OpenFileTemplateProcess();
                        break;
                    case EnumButton.BtnPrintLog:
                        PrintLogProcess();
                        break;
                    case EnumButton.Null:
                        break;
                    default:
                        break;
                }
                if (this.firstFile != null)
                {
                    flexCelPrintDocument1.Workbook = new XlsFile();
                    if (this.firstFile.saveMemoryStream != null && this.firstFile.saveMemoryStream.Length > 0)
                    {
                        this.firstFile.saveMemoryStream.Position = 0;
                        flexCelPrintDocument1.Workbook.Open(this.firstFile.saveMemoryStream);
                    }
                    else if (!String.IsNullOrEmpty(this.firstFile.saveFilePath))
                    {
                        flexCelPrintDocument1.Workbook.Open(this.firstFile.saveFilePath);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExportAsExcelProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    foreach (var item in selectedPartialFiles_Excel)
                    {
                        flexCelPrintDocument1.Workbook = new XlsFile();
                        if (item.saveMemoryStream != null && item.saveMemoryStream.Length > 0)
                        {
                            item.saveMemoryStream.Position = 0;
                            flexCelPrintDocument1.Workbook.Open(item.saveMemoryStream);
                        }
                        else if (!String.IsNullOrEmpty(item.saveFilePath))
                        {
                            flexCelPrintDocument1.Workbook.Open(item.saveFilePath);
                        }

                        if (flexCelPrintDocument1.Workbook == null)
                        {
                            MessageBox.Show("You need to open a file first.");
                            return;
                        }

                        if (!LoadPreferences()) return;

                        if (exportExcelDialog.ShowDialog() == DialogResult.OK)
                        {
                            //using (FileStream excel = new FileStream(exportExcelDialog.FileName, FileMode.Create))
                            //{
                            int SaveSheet = flexCelPrintDocument1.Workbook.ActiveSheet;
                            flexCelPrintDocument1.Workbook.Save(exportExcelDialog.FileName);
                            flexCelPrintDocument1.Workbook.ActiveSheet = SaveSheet;
                            if (MessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Process.Start(exportExcelDialog.FileName);
                            }
                            //}
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExportAsPdfProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    string mergePdf = Utils.GenerateTempFileWithin();
                    InsertPage(this.selectedPartialFiles_Excel, mergePdf);


                    if (exportPdfDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.Copy(mergePdf, exportPdfDialog.FileName, true);

                        if (File.Exists(exportPdfDialog.FileName) && MessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Process.Start(exportPdfDialog.FileName);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExportAsImageProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    string mergePdf = Utils.GenerateTempFileWithin();
                    InsertPage(this.selectedPartialFiles_Excel, mergePdf);

                    if (exportImageDialog.ShowDialog() == DialogResult.OK)
                    {
                        int largestEdgeLength = 1000;
                        using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
                        {
                            processor.LoadDocument(mergePdf);
                            if (processor.Document.Pages.Count == 1)
                            {
                                Bitmap image = processor.CreateBitmap(1, largestEdgeLength);

                                image.Save(exportImageDialog.FileName);
                            }
                            else if (processor.Document.Pages.Count > 1)
                            {
                                for (int i = 1; i <= processor.Document.Pages.Count; i++)
                                {

                                    Bitmap image = processor.CreateBitmap(i, largestEdgeLength);

                                    string partialImg = exportImageDialog.FileName.Replace(".", "(" + i + ")" + ".");
                                    image.Save(partialImg);
                                }
                            }
                        }

                        if (File.Exists(exportImageDialog.FileName) && MessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Process.Start(exportImageDialog.FileName);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TemplateKeyProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    foreach (var item in selectedPartialFiles_Excel)
                    {
                        Dictionary<string, object> TemplateKeyOne = item.TemplateKey;
                        if (TemplateKeyOne == null)
                        {
                            TemplateKeyOne = new Dictionary<string, object>();

                            flexCelPrintDocument1.Workbook = new XlsFile();
                            if (item.saveMemoryStream != null && item.saveMemoryStream.Length > 0)
                            {
                                item.saveMemoryStream.Position = 0;
                                flexCelPrintDocument1.Workbook.Open(item.saveMemoryStream);
                            }
                            else if (!String.IsNullOrEmpty(item.saveFilePath))
                            {
                                flexCelPrintDocument1.Workbook.Open(item.saveFilePath);
                            }
                            item.saveMemoryStream.Position = 0;

                            ExcelFile Xls = flexCelPrintDocument1.Workbook;

                            var sheetCount = Xls.SheetCount;
                            int xf = -1;
                            long count = 0;
                            try
                            {
                                count = long.Parse(Xls.GetCellValue(sheetCount, 1, 2, ref xf).ToString());
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                            for (int i = 1; i < count + 1; i++)
                            {
                                var key = Xls.GetCellValue(sheetCount, i + 1, 1, ref xf).ToString();
                                var value = Xls.GetCellValue(sheetCount, i + 1, 2, ref xf);
                                TemplateKeyOne.Add(key, value);
                            }
                        }
                        var tempKey = new TemplateKey.PreviewTemplateKey(TemplateKeyOne);
                        tempKey.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_TEMPLATE_KEY__FORM") + " ( " + item.printTypeCode + " ) ";
                        if (tempKey != null)
                        {
                            tempKey.ShowDialog();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OpenFileTemplateProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    foreach (var item in this.selectedPartialFiles_Excel)
                    {
                        string saveFilePath = item.saveFilePath;
                        if (!String.IsNullOrEmpty(saveFilePath))
                        {
                            System.Diagnostics.Process.Start(saveFilePath);
                        }
                    }
                    //string mergePdf = Utils.GenerateTempFileWithin();
                    //InsertPage(selectedPartialFiles, mergePdf);
                    //if (File.Exists(mergePdf))
                    //{
                    //    System.Diagnostics.Process.Start(mergePdf);
                    //}
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintLogProcess()
        {
            try
            {
                GetSelectedPartialFiles();
                popupControlContainer1.HidePopup();

                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    bool isHasPrintlog = false;
                    foreach (var item in selectedPartialFiles_Excel)
                    {
                        if (item.ShowPrintLog != null)
                        {
                            item.ShowPrintLog();
                            isHasPrintlog = true;
                        }
                    }
                    if (!isHasPrintlog)
                    {
                        MessageBox.Show("Không có lịch sử in!");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn biểu in nào!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancelPopupMenu_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentOnclickButton = EnumButton.Null;
                this.selectedPartialFiles_Excel = new List<PrintMergeAdo>();
                popupControlContainer1.HidePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSetupPrintPreviewMerge_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreviewMerge_FormClosing.1");
                this.eventLog = null;
                this.eventPrint = null;
                this.PrintLog = null;
                this.ShowPrintLog = null;
                this.ShowTutorialModule = null;
                this.flexCelPrintDocument1 = null;
                this.flexCelPdfExport1 = null;
                this.TemplateKey = null;
                this.emrInputADO = null;
                if (this.partialFiles_Excel != null && this.partialFiles_Excel.Count > 0)
                {
                    foreach (var item in this.partialFiles_Excel)
                    {
                        if (item.saveMemoryStream != null)
                        {
                            item.saveMemoryStream.Dispose();
                        }
                    }
                }
                this.partialFiles_Excel = null;
                if (this.partialFiles_Pdf != null && this.partialFiles_Pdf.Count > 0)
                {
                    foreach (var item in this.partialFiles_Pdf)
                    {
                        if (item.saveMemoryStream != null)
                        {
                            item.saveMemoryStream.Dispose();
                        }
                    }
                }
                this.partialFiles_Pdf = null;
                this.currentPaperSize = null;
                if (this.selectedPartialFiles_Excel != null && this.selectedPartialFiles_Excel.Count > 0)
                {
                    foreach (var item in this.selectedPartialFiles_Excel)
                    {
                        if (item.saveMemoryStream != null)
                        {
                            item.saveMemoryStream.Dispose();
                        }
                    }
                }
                this.selectedPartialFiles_Excel = null;
                this.firstFile = null;
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreviewMerge_FormClosing.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
