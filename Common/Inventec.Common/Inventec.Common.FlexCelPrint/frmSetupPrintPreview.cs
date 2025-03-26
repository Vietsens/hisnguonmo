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

namespace Inventec.Common.FlexCelPrint
{
    public delegate void DelegateEventLog();
    public delegate void DelegateReturnEventPrint();
    public delegate bool DelegatePrintLog(ref string message, string printReason);
    public delegate void DelegateShowPrintLog();
    public delegate long DelegateGetNumOrderPrint();
    public delegate void DelegateRemoteSupport();

    /// <summary>
    /// Printing / Previewing and Exporting files to excel|pdf|image.
    /// </summary>
    public partial class frmSetupPrintPreview : DevExpress.XtraEditors.XtraForm
    {
        #region Variable
        DelegateEventLog eventLog;
        DelegateReturnEventPrint eventPrint;
        DelegatePrintLog PrintLog;
        DelegateShowPrintLog ShowPrintLog;
        DelegateGetNumOrderPrint getNumOrderPrint;
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
        bool isPrintExceptionReason = false;
        bool isShowPrintExceptionReason = false;
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

        List<string> partialFiles;
        List<MemoryStream> partialStreams;
        PaperSize currentPaperSize;
        #endregion

        #region Construction
        #region MemoryStream
        public frmSetupPrintPreview(MemoryStream data, DelegateEventLog eventLog, string pathFileTemplate)
            : this(data, eventLog, pathFileTemplate, "")
        {
        }

        public frmSetupPrintPreview(MemoryStream data, DelegateEventLog eventLog, string pathFileTemplate, string defaultPrintName)
            : this(data, defaultPrintName, pathFileTemplate)
        {
            try
            {
                this.eventLog = eventLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmSetupPrintPreview(MemoryStream data, string pathFileTemplate)
            : this(data, "", pathFileTemplate)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate)
            : this(data, defaultPrintName, pathFileTemplate, 1)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, false, true)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, true)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("1. InitializeComponent");
                InitializeComponent();
                CheckSizePreview();
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Debug("2. print temp");
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                Inventec.Common.Logging.LogSystem.Debug("3. Initialize memo");
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("No printers are installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("No printers are installed");
                }
                this.emrByteInputFile = FlexCelPrintUtil.StreamToByte(data);
                InitMemo(data);
                numericUpDownCopies.Value = numCopy;
                this.isPreview = isPreview;
                this.isAllowExport = isallowExport;
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

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("1. InitializeComponent");
                InitializeComponent();
                CheckSizePreview();
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Debug("2. print temp");
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                Inventec.Common.Logging.LogSystem.Debug("3. Initialize memo");
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("No printers are installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("No printers are installed");
                }
                this.emrByteInputFile = FlexCelPrintUtil.StreamToByte(data);
                InitMemo(data);
                numericUpDownCopies.Value = numCopy;
                this.isPreview = isPreview;
                this.isAllowExport = isallowExport;
                if (this.isPreview) UpdateItemsDisable();
                this.TemplateKey = templateKey;
                Inventec.Common.Logging.LogSystem.Debug("end InitializeComponent");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, null)
        {

        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, object emrInputADO)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, null, emrInputADO)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, eventPrint, emrInputADO, null, null)
        {
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isAllowEditTemplateFile, bool isSingleCopy)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("1. InitializeComponent");
                InitializeComponent();
                CheckSizePreview();
                SetCaptionByLanguageKey();
                Inventec.Common.Logging.LogSystem.Debug("2. print temp");
                this.defaultPrintName = defaultPrintName;
                this.pathFileTemplate = pathFileTemplate;
                Inventec.Common.Logging.LogSystem.Debug("3. Initialize memo");
                if (PrinterSettings.InstalledPrinters == null || PrinterSettings.InstalledPrinters.Count <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("No printers are installed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception("No printers are installed");
                }
                this.emrByteInputFile = FlexCelPrintUtil.StreamToByte(data);
                InitMemo(data);
                numericUpDownCopies.Value = numCopy;
                if (isSingleCopy)
                {
                    numericUpDownCopies.Value = 1;
                    numericUpDownCopies.Enabled = false;
                }
                this.isPreview = isPreview;
                this.isAllowExport = isallowExport;
                this.isAllowEditTemplateFile = isAllowEditTemplateFile;
                this.eventLog = eventLog;
                this.eventPrint = eventPrint;
                if (this.isPreview) UpdateItemsDisable();
                this.TemplateKey = templateKey;
                this.emrInputADO = emrInputADO as Inventec.Common.SignLibrary.ADO.InputADO;
                this.PrintLog = printLog;
                this.ShowPrintLog = showLog;
                Inventec.Common.Logging.LogSystem.Debug("end InitializeComponent");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isAllowEditTemplateFile, bool isSingleCopy, DelegateGetNumOrderPrint getNumOrderPrint, bool isPrintExceptionReason)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, isAllowEditTemplateFile, isSingleCopy)
        {
            try
            {
                this.getNumOrderPrint = getNumOrderPrint;
                this.isPrintExceptionReason = isPrintExceptionReason;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, true)
        {

        }

        public frmSetupPrintPreview(MemoryStream data, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isAllowEditTemplateFile)
            : this(data, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, eventLog, eventPrint, emrInputADO, printLog, showLog, true, false)
        {
        }
        #endregion

        #region path
        public frmSetupPrintPreview(string path, DelegateEventLog eventLog, string pathFileTemplate)
            : this(path, eventLog, pathFileTemplate, "")
        {
        }

        public frmSetupPrintPreview(string path, DelegateEventLog eventLog, string pathFileTemplate, string defaultPrintName)
            : this(path, defaultPrintName, pathFileTemplate)
        {
            try
            {
                this.eventLog = eventLog;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmSetupPrintPreview(string path, string pathFileTemplate)
            : this(path, "", pathFileTemplate)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate)
            : this(path, defaultPrintName, pathFileTemplate, 1)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy)
            : this(path, defaultPrintName, pathFileTemplate, numCopy, false, true)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview)
            : this(path, defaultPrintName, pathFileTemplate, numCopy, isPreview, true)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport)
            : this(path, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, null)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey)
            : this(path, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, null, null, null, null, null)
        {
        }

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog, bool isSingleCopy)
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
                this.emrByteInputFile = FlexCelPrintUtil.FileToByte(path);
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

        public frmSetupPrintPreview(string path, string defaultPrintName, string pathFileTemplate, int numCopy, bool isPreview, bool isallowExport, Dictionary<string, object> templateKey, DelegateEventLog eventLog, DelegateReturnEventPrint eventPrint, object emrInputADO, DelegatePrintLog printLog, DelegateShowPrintLog showLog)
            : this(path, defaultPrintName, pathFileTemplate, numCopy, isPreview, isallowExport, templateKey, null, null, null, null, null, false)
        {
        }
        #endregion

        public void SetPartialFileList(List<string> partialFiles)
        {
            this.partialFiles = partialFiles;
        }

        public void SetPartialFileList(List<MemoryStream> partialStreams)
        {
            this.partialStreams = partialStreams;
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
                flexCelPrintDocument1.Workbook = new XlsFile();
                flexCelPrintDocument1.Workbook.Open(this.pathFileResult);
                ExcelFile Xls = flexCelPrintDocument1.Workbook;
                Inventec.Common.Logging.LogSystem.Debug("t2 - LoadSheetConfig");
                LoadSheetConfig();
                Inventec.Common.Logging.LogSystem.Debug("t3 - InitPrintPreviewControl");
                InitPrintPreviewControl();
                ChkHidePrinting.Checked = FlexCelPrintUtil.GetConfigValue();
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_FormClosing.1");
                base.OnFormClosing(e);

                this.flexCelPrintDocument1.GetPrinterHardMargins -= new FlexCel.Render.PrintHardMarginsEventHandler(this.flexCelPrintDocument1_GetPrinterHardMargins);
                this.flexCelPrintDocument1.BeforePrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_BeforePrintPage);
                this.flexCelPrintDocument1.BeginPrint -= new System.Drawing.Printing.PrintEventHandler(this.flexCelPrintDocument1_BeginPrint);
                this.flexCelPrintDocument1.PrintPage -= new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_PrintPage);
                this.btnExportAsExcel.Click -= new System.EventHandler(this.btnExportAsExcel_Click);
                this.btnExportAsPdf.Click -= new System.EventHandler(this.btnExportAsPdf_Click);
                this.btnExportAsImage.Click -= new System.EventHandler(this.btnExportAsImage_Click);
                this.bbtnPrintShortcut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnPrintShortcut_ItemClick);
                this.rdPartrain.CheckedChanged -= new System.EventHandler(this.rdPartrain_CheckedChanged);
                this.rdLandscape.CheckedChanged -= new System.EventHandler(this.rdLandscape_CheckedChanged);
                this.chkPages.CheckedChanged -= new System.EventHandler(this.chkPages_CheckedChanged);
                this.btnPrintSetup.Click -= new System.EventHandler(this.btnPageSetup_Click);
                this.btnPageSetup.Click -= new System.EventHandler(this.btnPrintSetup_Click);
                this.btnPrint.Click -= new System.EventHandler(this.btnPrint_Click);
                this.btnNextPage.Click -= new System.EventHandler(this.btnNextPage_Click);
                this.btnPrePage.Click -= new System.EventHandler(this.btnPrePage_Click);
                this.trackBarZoomPage.EditValueChanged -= new System.EventHandler(this.trackBarZoomPage_EditValueChanged);
                this.chkVertically.CheckedChanged -= new System.EventHandler(this.chkVertically_CheckedChanged);
                this.chkHorizontally.CheckedChanged -= new System.EventHandler(this.chkHorizontally_CheckedChanged);
                this.spinZoom.ValueChanged -= new System.EventHandler(this.spinZoom_ValueChanged);
                this.cboPaperSize.EditValueChanged -= new System.EventHandler(this.cboPaperSize_EditValueChanged);
                this.ChkHidePrinting.CheckedChanged -= new System.EventHandler(this.ChkHidePrinting_CheckedChanged);
                this.btnOpenFileTemplate.Click -= new System.EventHandler(this.btnOpenFileTemplate_Click);
                this.btnOpenTutorial.Click -= new System.EventHandler(this.btnOpenTutorial_Click);
                this.BtnPrintLog.Click -= new System.EventHandler(this.BtnPrintLog_Click);
                this.btnEmr.Click -= new System.EventHandler(this.btnEmr_Click);
                this.btnTemplateKey.Click -= new System.EventHandler(this.simpleButton1_Click);
                this.spinChangePage.EditValueChanged -= new System.EventHandler(this.spinChangePage_EditValueChanged);
                this.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.frmSetupPrintPreview_MouseWheel);

                this.Dispose(true);

                this.eventLog = null;
                this.eventPrint = null;
                this.PrintLog = null;
                this.ShowPrintLog = null;
                this.getNumOrderPrint = null;
                this.ShowTutorialModule = null;
                this.flexCelPrintDocument1 = null;
                this.flexCelPdfExport1 = null;
                this.TemplateKey = null;
                this.emrInputADO = null;
                this.partialFiles = null;
                this.partialStreams = null;
                this.currentPaperSize = null;

                GC.SuppressFinalize(this);

                // Force garbage collection.
                GC.Collect();
                // Wait for all finalizers to complete
                GC.WaitForPendingFinalizers();
                // Force garbage collection again.
                GC.Collect();
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_FormClosing.2");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }
        
        private void frmSetupPrintPreview_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_Load.1");
                numericUpDownCopies.Focus();
                numericUpDownCopies.SelectAll();
                btnExportAsExcel.Enabled = isAllowExport;
                btnExportAsImage.Enabled = isAllowExport;
                btnExportAsPdf.Enabled = isAllowExport;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isAllowExport), isAllowExport));
                btnEmr.Enabled = (this.emrByteInputFile != null);
                BtnPrintLog.Enabled = this.ShowPrintLog != null;
                btnOpenFileTemplate.Enabled = this.isAllowEditTemplateFile;
                btnOpenFileTemplate.ToolTip = pathFileTemplate;
                lciForbtnOpenFileTemplate.OptionsToolTip.ToolTip = pathFileTemplate;
                if (this.isPrintExceptionReason)
                {
                    this.isShowPrintExceptionReason = true;
                    btnExportAsExcel.Enabled = false;
                    btnExportAsPdf.Enabled = false;
                    btnExportAsImage.Enabled = false;
                    btnEmr.Enabled = false;
                }
                Inventec.Common.Logging.LogSystem.Debug("frmSetupPrintPreview_Load.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
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


                if (e.Delta > 0)
                {
                    trackBarZoomPage.Value += 10;
                }
                else
                {
                    trackBarZoomPage.Value -= 10;
                }
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
                    case Keys.Down:
                        spinChangePage.Focus();
                        break;
                    case Keys.Up:
                        spinChangePage.Focus();
                        break;
                    case Keys.VolumeUp:
                        spinChangePage.Focus();
                        break;
                    case Keys.VolumeDown:
                        spinChangePage.Focus();
                        break;
                    case Keys.NumPad8:
                        spinChangePage.Focus();
                        break;
                    case Keys.NumPad2:
                        spinChangePage.Focus();
                        break;
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
                this.lblOf.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LBL_OF");
                this.lcgCenterOnPage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_CENTER_ON_PAGE");
                this.lcgPageRange.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAGE_RANGE");
                this.lcgPaperOrientation.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAPER_ORIENTATION");
                this.lcgPaperProperties.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCG_PAPER_PROPERTIES");
                this.lciAvailablePrinters.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_AVAILABLE_PRINTERS");
                this.lciBlackAndWhite.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_BLACK_AND_WHITE");
                this.lciNumericUpDownCopies.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_NUMERIC_UP_DOWN_COPIES");
                this.lciPage.Text = GetResourceMessage("IVT_LANGUAGE_KEY__FORM_PRINT_PREVIEW__LCI_PAGE");
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
                DoExportUsingFlexCelImgExportComplex(ImageColorDepth.BlackAndWhite);
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnExportAsPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (exportPdfDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToPdf(exportPdfDialog.FileName);

                    if (File.Exists(exportPdfDialog.FileName) && MessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start(exportPdfDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ExportToPdf(string outFile)
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

                if (!LoadPreferencesPdf()) return;

                using (FileStream Pdf = new FileStream(outFile, FileMode.OpenOrCreate))
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
                        maxPage = (int)spinChangePage.Value;
                        page = (int)spinChangePage.Value;
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
                if (!String.IsNullOrEmpty(this.pathFileTemplate))
                {
                    System.Diagnostics.Process.Start(this.pathFileTemplate);
                }
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
                if (TemplateKey == null)
                {
                    TemplateKey = new Dictionary<string, object>();
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
                        TemplateKey.Add(key, value);
                    }
                }
                var tempKey = new TemplateKey.PreviewTemplateKey(TemplateKey);
                if (tempKey != null)
                {
                    tempKey.ShowDialog();
                }
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
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();
                //Convert excel file to pdf before call SignLibrary
                //byte[] emrByteInputFilePdf = null;
                //string outFile = "";
                //Inventec.Common.FileConvert.Convert.ExcelToPdf(this.emrByteInputFile, "", ref emrByteInputFilePdf, ref outFile);

                string fileTemp = Inventec.Common.SignLibrary.Utils.GenerateTempFileWithin();
                ExportToPdf(fileTemp);
                if (File.Exists(fileTemp))
                {
                    this.emrInputADO.PrintNumberCopies = (short)(numericUpDownCopies.Value);
                    this.emrInputADO.PaperSizeDefault = currentPaperSize;
                    libraryProcessor.ShowPopup(fileTemp, this.emrInputADO);
                    try
                    {
                        File.Delete(fileTemp);
                    }
                    catch { }
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

                if (ShowPrintLog != null)
                {
                    ShowPrintLog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            try
            {
                if (spinChangePage.EditValue == null)
                {
                    spinChangePage.Value = 1;
                    btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
                    btnPrePage.Enabled = false;
                }
                else
                {
                    if (spinChangePage.Value == 1)
                    {
                        return;
                    }
                    spinChangePage.Value = spinChangePage.Value - 1;
                    btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
                }
                printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                if (spinChangePage.EditValue == null)
                {
                    spinChangePage.Value = 1;
                    btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
                    btnPrePage.Enabled = false;
                }
                else
                {
                    if (spinChangePage.Value == spinChangePage.Properties.MaxValue)
                    {
                        return;
                    }
                    spinChangePage.Value = spinChangePage.Value + 1;
                    btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
                    btnNextPage.Enabled = (spinChangePage.Properties.MaxValue > 1 && spinChangePage.Value < spinChangePage.Properties.MaxValue);
                }
                printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
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
                if (!e.HasMorePages)
                {
                    spinChangePage.Properties.MaxValue = page;
                    btnNextPage.Enabled = spinChangePage.Properties.MaxValue > 1;
                    numericUpDownFromPage.Properties.MaxValue = page;
                    numericUpDownToPage.Properties.MaxValue = page;
                    lblTotalPage.Text = page + "";
                    pageCount = page;

                    isFirstLoad = false;
                    ActiveTopForm();
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
                if (spinChangePage.EditValue != null)
                {
                    printPreviewControl1.StartPage = (int)spinChangePage.Value - 1;

                    btnPrePage.Enabled = (spinChangePage.Value > 1 && spinChangePage.Properties.MaxValue > 1);
                    btnNextPage.Enabled = (spinChangePage.Properties.MaxValue > 1 && spinChangePage.Value < spinChangePage.Properties.MaxValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void trackBarZoomPage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //ExcelFile Xls = flexCelPrintDocument1.Workbook;
                //Xls.PrintScale = trackBarZoomPage.Value;

                printPreviewControl1.Zoom = (double)((double)(trackBarZoomPage.Value) / (double)100);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

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
            InitPrintPreviewControl();
        }

        private void cboPaperSize_EditValueChanged(object sender, EventArgs e)
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

        private void bbtnRemoteSupport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (Ado.SupportAdo.RemoteSupport != null)
                {
                    Ado.SupportAdo.RemoteSupport();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
