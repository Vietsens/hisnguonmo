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
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using FlexCel.Core;
using FlexCel.XlsAdapter;
using FlexCel.Render;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace Inventec.Common.FlexCelPrint
{
    public partial class frmSetupPrintPreview : DevExpress.XtraEditors.XtraForm
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.SaveFileDialog exportImageDialog;
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetupPrintPreview));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.flexCelPrintDocument1 = new FlexCel.Render.FlexCelPrintDocument();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.btnExportAsExcel = new System.Windows.Forms.Button();
            this.btnExportAsPdf = new System.Windows.Forms.Button();
            this.btnExportAsImage = new System.Windows.Forms.Button();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.bbtnPrintShortcut = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemClose = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.rdPartrain = new System.Windows.Forms.RadioButton();
            this.rdLandscape = new System.Windows.Forms.RadioButton();
            this.chkCurrentPage = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.chkPages = new DevExpress.XtraEditors.CheckEdit();
            this.numericUpDownFromPage = new DevExpress.XtraEditors.SpinEdit();
            this.numericUpDownToPage = new DevExpress.XtraEditors.SpinEdit();
            this.chkAllPages = new DevExpress.XtraEditors.CheckEdit();
            this.lcgPageRange = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnPrintSetup = new System.Windows.Forms.Button();
            this.btnPageSetup = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.lblTotalPage = new DevExpress.XtraEditors.LabelControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnNextPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrePage = new DevExpress.XtraEditors.SimpleButton();
            this.trackBarZoomPage = new DevExpress.XtraEditors.TrackBarControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.chkVertically = new DevExpress.XtraEditors.CheckEdit();
            this.chkHorizontally = new DevExpress.XtraEditors.CheckEdit();
            this.numericUpDownCopies = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.spinZoom = new DevExpress.XtraEditors.SpinEdit();
            this.cboSourcePage = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cboPaperSize = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lcgPaperProperties = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciSource = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSize = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciZoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.ChkHidePrinting = new DevExpress.XtraEditors.CheckEdit();
            this.chkBlackAndWhite = new DevExpress.XtraEditors.CheckEdit();
            this.cboPrinters = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAvailablePrinters = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBlackAndWhite = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.btnOpenTutorial = new System.Windows.Forms.Button();
            this.btnOpenFileTemplate = new DevExpress.XtraEditors.SimpleButton();
            this.BtnPrintLog = new System.Windows.Forms.Button();
            this.btnEmr = new System.Windows.Forms.Button();
            this.btnTemplateKey = new System.Windows.Forms.Button();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem35 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciForbtnOpenFileTemplate = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.lcgPaperOrientation = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumericUpDownCopies = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcgCenterOnPage = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem33 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblOf = new DevExpress.XtraEditors.LabelControl();
            this.spinChangePage = new DevExpress.XtraEditors.SpinEdit();
            this.panel2 = new System.Windows.Forms.Panel();
            this.printPreviewControl1 = new System.Windows.Forms.PrintPreviewControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPage = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.exportImageDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportPdfDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportExcelDialog = new System.Windows.Forms.SaveFileDialog();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.exportTiffDialog = new System.Windows.Forms.SaveFileDialog();
            this.bbtnRemoteSupport = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCurrentPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPages.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFromPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllPages.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPageRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkVertically.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHorizontally.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinZoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSourcePage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaperSize.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPaperProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChkHidePrinting.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBlackAndWhite.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinters.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAvailablePrinters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBlackAndWhite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            this.layoutControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForbtnOpenFileTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPaperOrientation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumericUpDownCopies)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgCenterOnPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinChangePage.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "xls";
            this.openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|Excel 97/2003|*.xls|Excel 2007|*.xlsx;*.xlsm|All " +
    "files|*.*";
            this.openFileDialog1.Title = "Open an Excel File";
            // 
            // printDialog1
            // 
            this.printDialog1.AllowSomePages = true;
            // 
            // flexCelPrintDocument1
            // 
            this.flexCelPrintDocument1.AllVisibleSheets = false;
            this.flexCelPrintDocument1.ResetPageNumberOnEachSheet = true;
            this.flexCelPrintDocument1.Workbook = null;
            this.flexCelPrintDocument1.GetPrinterHardMargins += new FlexCel.Render.PrintHardMarginsEventHandler(this.flexCelPrintDocument1_GetPrinterHardMargins);
            this.flexCelPrintDocument1.BeforePrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_BeforePrintPage);
            this.flexCelPrintDocument1.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.flexCelPrintDocument1_BeginPrint);
            this.flexCelPrintDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.flexCelPrintDocument1_PrintPage);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Document = this.flexCelPrintDocument1;
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // btnExportAsExcel
            // 
            this.btnExportAsExcel.Location = new System.Drawing.Point(12, 12);
            this.btnExportAsExcel.Name = "btnExportAsExcel";
            this.btnExportAsExcel.Size = new System.Drawing.Size(78, 20);
            this.btnExportAsExcel.TabIndex = 16;
            this.btnExportAsExcel.Text = "Export as Excel";
            this.btnExportAsExcel.UseVisualStyleBackColor = true;
            this.btnExportAsExcel.Click += new System.EventHandler(this.btnExportAsExcel_Click);
            // 
            // btnExportAsPdf
            // 
            this.btnExportAsPdf.Location = new System.Drawing.Point(94, 12);
            this.btnExportAsPdf.Name = "btnExportAsPdf";
            this.btnExportAsPdf.Size = new System.Drawing.Size(73, 20);
            this.btnExportAsPdf.TabIndex = 17;
            this.btnExportAsPdf.Text = "Export as Pdf";
            this.btnExportAsPdf.UseVisualStyleBackColor = true;
            this.btnExportAsPdf.Click += new System.EventHandler(this.btnExportAsPdf_Click);
            // 
            // btnExportAsImage
            // 
            this.btnExportAsImage.Location = new System.Drawing.Point(171, 12);
            this.btnExportAsImage.Name = "btnExportAsImage";
            this.btnExportAsImage.Size = new System.Drawing.Size(75, 20);
            this.btnExportAsImage.TabIndex = 18;
            this.btnExportAsImage.Text = "Export as Image";
            this.btnExportAsImage.UseVisualStyleBackColor = true;
            this.btnExportAsImage.Click += new System.EventHandler(this.btnExportAsImage_Click);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnPrintShortcut,
            this.barButtonItemClose,
            this.bbtnRemoteSupport});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 3;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnPrintShortcut),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemClose),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRemoteSupport)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            this.bar2.Visible = false;
            // 
            // bbtnPrintShortcut
            // 
            this.bbtnPrintShortcut.Caption = "Print (Ctrl P)";
            this.bbtnPrintShortcut.Id = 0;
            this.bbtnPrintShortcut.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnPrintShortcut.Name = "bbtnPrintShortcut";
            this.bbtnPrintShortcut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnPrintShortcut_ItemClick);
            // 
            // barButtonItemClose
            // 
            this.barButtonItemClose.Caption = "esc";
            this.barButtonItemClose.Id = 1;
            this.barButtonItemClose.Name = "barButtonItemClose";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1154, 22);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 661);
            this.barDockControlBottom.Size = new System.Drawing.Size(1154, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 639);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1154, 22);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 639);
            // 
            // rdPartrain
            // 
            this.rdPartrain.Location = new System.Drawing.Point(13, 31);
            this.rdPartrain.Name = "rdPartrain";
            this.rdPartrain.Size = new System.Drawing.Size(113, 24);
            this.rdPartrain.TabIndex = 4;
            this.rdPartrain.TabStop = true;
            this.rdPartrain.Text = "Portrain";
            this.rdPartrain.UseVisualStyleBackColor = true;
            this.rdPartrain.CheckedChanged += new System.EventHandler(this.rdPartrain_CheckedChanged);
            // 
            // rdLandscape
            // 
            this.rdLandscape.Location = new System.Drawing.Point(130, 31);
            this.rdLandscape.Name = "rdLandscape";
            this.rdLandscape.Size = new System.Drawing.Size(115, 24);
            this.rdLandscape.TabIndex = 5;
            this.rdLandscape.TabStop = true;
            this.rdLandscape.Text = "Landscape";
            this.rdLandscape.UseVisualStyleBackColor = true;
            this.rdLandscape.CheckedChanged += new System.EventHandler(this.rdLandscape_CheckedChanged);
            // 
            // chkCurrentPage
            // 
            this.chkCurrentPage.Location = new System.Drawing.Point(131, 30);
            this.chkCurrentPage.Name = "chkCurrentPage";
            this.chkCurrentPage.Properties.Caption = "Current Page";
            this.chkCurrentPage.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkCurrentPage.Properties.RadioGroupIndex = 1;
            this.chkCurrentPage.Size = new System.Drawing.Size(115, 19);
            this.chkCurrentPage.StyleController = this.layoutControl5;
            this.chkCurrentPage.TabIndex = 12;
            this.chkCurrentPage.TabStop = false;
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.chkPages);
            this.layoutControl5.Controls.Add(this.numericUpDownFromPage);
            this.layoutControl5.Controls.Add(this.numericUpDownToPage);
            this.layoutControl5.Controls.Add(this.chkCurrentPage);
            this.layoutControl5.Controls.Add(this.chkAllPages);
            this.layoutControl5.Location = new System.Drawing.Point(2, 426);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl5.Root = this.lcgPageRange;
            this.layoutControl5.Size = new System.Drawing.Size(258, 85);
            this.layoutControl5.TabIndex = 45;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // chkPages
            // 
            this.chkPages.Location = new System.Drawing.Point(12, 53);
            this.chkPages.Name = "chkPages";
            this.chkPages.Properties.Caption = "Pages:";
            this.chkPages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkPages.Properties.RadioGroupIndex = 1;
            this.chkPages.Size = new System.Drawing.Size(60, 19);
            this.chkPages.StyleController = this.layoutControl5;
            this.chkPages.TabIndex = 13;
            this.chkPages.TabStop = false;
            this.chkPages.CheckedChanged += new System.EventHandler(this.chkPages_CheckedChanged);
            // 
            // numericUpDownFromPage
            // 
            this.numericUpDownFromPage.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownFromPage.Location = new System.Drawing.Point(76, 53);
            this.numericUpDownFromPage.Name = "numericUpDownFromPage";
            this.numericUpDownFromPage.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.numericUpDownFromPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numericUpDownFromPage.Size = new System.Drawing.Size(82, 20);
            this.numericUpDownFromPage.StyleController = this.layoutControl5;
            this.numericUpDownFromPage.TabIndex = 14;
            // 
            // numericUpDownToPage
            // 
            this.numericUpDownToPage.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownToPage.Location = new System.Drawing.Point(162, 53);
            this.numericUpDownToPage.Name = "numericUpDownToPage";
            this.numericUpDownToPage.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.numericUpDownToPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numericUpDownToPage.Size = new System.Drawing.Size(84, 20);
            this.numericUpDownToPage.StyleController = this.layoutControl5;
            this.numericUpDownToPage.TabIndex = 15;
            // 
            // chkAllPages
            // 
            this.chkAllPages.Location = new System.Drawing.Point(12, 30);
            this.chkAllPages.Name = "chkAllPages";
            this.chkAllPages.Properties.Caption = "All";
            this.chkAllPages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkAllPages.Properties.RadioGroupIndex = 1;
            this.chkAllPages.Size = new System.Drawing.Size(115, 19);
            this.chkAllPages.StyleController = this.layoutControl5;
            this.chkAllPages.TabIndex = 10;
            this.chkAllPages.TabStop = false;
            // 
            // lcgPageRange
            // 
            this.lcgPageRange.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgPageRange.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem16,
            this.layoutControlItem28,
            this.layoutControlItem29,
            this.layoutControlItem30,
            this.layoutControlItem18});
            this.lcgPageRange.Location = new System.Drawing.Point(0, 0);
            this.lcgPageRange.Name = "lcgPageRange";
            this.lcgPageRange.Size = new System.Drawing.Size(258, 85);
            this.lcgPageRange.Text = "Page Range";
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.chkAllPages;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(119, 23);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.Control = this.numericUpDownFromPage;
            this.layoutControlItem28.Location = new System.Drawing.Point(64, 23);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(86, 24);
            this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem28.TextVisible = false;
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.Control = this.numericUpDownToPage;
            this.layoutControlItem29.Location = new System.Drawing.Point(150, 23);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Size = new System.Drawing.Size(88, 24);
            this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem29.TextVisible = false;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.Control = this.chkPages;
            this.layoutControlItem30.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.Size = new System.Drawing.Size(64, 24);
            this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem30.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.chkCurrentPage;
            this.layoutControlItem18.Location = new System.Drawing.Point(119, 0);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(119, 23);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // btnPrintSetup
            // 
            this.btnPrintSetup.Location = new System.Drawing.Point(134, 51);
            this.btnPrintSetup.Name = "btnPrintSetup";
            this.btnPrintSetup.Size = new System.Drawing.Size(126, 22);
            this.btnPrintSetup.TabIndex = 3;
            this.btnPrintSetup.Text = "Print setup";
            this.btnPrintSetup.UseVisualStyleBackColor = true;
            this.btnPrintSetup.Click += new System.EventHandler(this.btnPageSetup_Click);
            // 
            // btnPageSetup
            // 
            this.btnPageSetup.Location = new System.Drawing.Point(134, 26);
            this.btnPageSetup.Name = "btnPageSetup";
            this.btnPageSetup.Size = new System.Drawing.Size(126, 21);
            this.btnPageSetup.TabIndex = 2;
            this.btnPageSetup.Text = "Page setup";
            this.btnPageSetup.UseVisualStyleBackColor = true;
            this.btnPageSetup.Click += new System.EventHandler(this.btnPrintSetup_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(2, 2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(128, 71);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print\r\n (Ctrl P)";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lblTotalPage
            // 
            this.lblTotalPage.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalPage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPage.Location = new System.Drawing.Point(457, 592);
            this.lblTotalPage.Name = "lblTotalPage";
            this.lblTotalPage.Size = new System.Drawing.Size(49, 31);
            this.lblTotalPage.StyleController = this.layoutControl1;
            this.lblTotalPage.TabIndex = 46;
            this.lblTotalPage.Text = "1";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnNextPage);
            this.layoutControl1.Controls.Add(this.btnPrePage);
            this.layoutControl1.Controls.Add(this.trackBarZoomPage);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.lblTotalPage);
            this.layoutControl1.Controls.Add(this.lblOf);
            this.layoutControl1.Controls.Add(this.spinChangePage);
            this.layoutControl1.Controls.Add(this.panel2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 22);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1154, 639);
            this.layoutControl1.TabIndex = 46;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnNextPage
            // 
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.Location = new System.Drawing.Point(520, 592);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(40, 31);
            this.btnNextPage.StyleController = this.layoutControl1;
            this.btnNextPage.TabIndex = 48;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrePage
            // 
            this.btnPrePage.Enabled = false;
            this.btnPrePage.Image = ((System.Drawing.Image)(resources.GetObject("btnPrePage.Image")));
            this.btnPrePage.Location = new System.Drawing.Point(317, 592);
            this.btnPrePage.Name = "btnPrePage";
            this.btnPrePage.Size = new System.Drawing.Size(36, 31);
            this.btnPrePage.StyleController = this.layoutControl1;
            this.btnPrePage.TabIndex = 47;
            this.btnPrePage.Click += new System.EventHandler(this.btnPrePage_Click);
            // 
            // trackBarZoomPage
            // 
            this.trackBarZoomPage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.trackBarZoomPage.EditValue = 90;
            this.trackBarZoomPage.Location = new System.Drawing.Point(647, 592);
            this.trackBarZoomPage.MenuManager = this.barManager1;
            this.trackBarZoomPage.Name = "trackBarZoomPage";
            this.trackBarZoomPage.Properties.AllowFocused = false;
            this.trackBarZoomPage.Properties.LabelAppearance.Options.UseTextOptions = true;
            this.trackBarZoomPage.Properties.LabelAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.trackBarZoomPage.Properties.Maximum = 400;
            this.trackBarZoomPage.Properties.Minimum = 1;
            this.trackBarZoomPage.Properties.ShowLabels = true;
            this.trackBarZoomPage.Properties.ShowValueToolTip = true;
            this.trackBarZoomPage.Size = new System.Drawing.Size(505, 45);
            this.trackBarZoomPage.StyleController = this.layoutControl1;
            this.trackBarZoomPage.TabIndex = 20;
            this.trackBarZoomPage.Value = 90;
            this.trackBarZoomPage.EditValueChanged += new System.EventHandler(this.trackBarZoomPage_EditValueChanged);
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.chkVertically);
            this.layoutControl3.Controls.Add(this.chkHorizontally);
            this.layoutControl3.Controls.Add(this.numericUpDownCopies);
            this.layoutControl3.Controls.Add(this.layoutControl7);
            this.layoutControl3.Controls.Add(this.layoutControl2);
            this.layoutControl3.Controls.Add(this.layoutControl6);
            this.layoutControl3.Controls.Add(this.layoutControl5);
            this.layoutControl3.Controls.Add(this.layoutControl4);
            this.layoutControl3.Controls.Add(this.btnPrint);
            this.layoutControl3.Controls.Add(this.btnPageSetup);
            this.layoutControl3.Controls.Add(this.btnPrintSetup);
            this.layoutControl3.Location = new System.Drawing.Point(2, 2);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl3.Root = this.Root;
            this.layoutControl3.Size = new System.Drawing.Size(262, 635);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // chkVertically
            // 
            this.chkVertically.Location = new System.Drawing.Point(15, 180);
            this.chkVertically.MenuManager = this.barManager1;
            this.chkVertically.Name = "chkVertically";
            this.chkVertically.Properties.Caption = "Vertically";
            this.chkVertically.Size = new System.Drawing.Size(114, 19);
            this.chkVertically.StyleController = this.layoutControl3;
            this.chkVertically.TabIndex = 50;
            this.chkVertically.CheckedChanged += new System.EventHandler(this.chkVertically_CheckedChanged);
            // 
            // chkHorizontally
            // 
            this.chkHorizontally.Location = new System.Drawing.Point(133, 180);
            this.chkHorizontally.MenuManager = this.barManager1;
            this.chkHorizontally.Name = "chkHorizontally";
            this.chkHorizontally.Properties.Caption = "Horizontally";
            this.chkHorizontally.Size = new System.Drawing.Size(114, 19);
            this.chkHorizontally.StyleController = this.layoutControl3;
            this.chkHorizontally.TabIndex = 49;
            this.chkHorizontally.CheckedChanged += new System.EventHandler(this.chkHorizontally_CheckedChanged);
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownCopies.Location = new System.Drawing.Point(189, 2);
            this.numericUpDownCopies.Name = "numericUpDownCopies";
            this.numericUpDownCopies.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.numericUpDownCopies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numericUpDownCopies.Size = new System.Drawing.Size(71, 20);
            this.numericUpDownCopies.StyleController = this.layoutControl3;
            this.numericUpDownCopies.TabIndex = 0;
            // 
            // layoutControl7
            // 
            this.layoutControl7.Controls.Add(this.spinZoom);
            this.layoutControl7.Controls.Add(this.cboSourcePage);
            this.layoutControl7.Controls.Add(this.cboPaperSize);
            this.layoutControl7.Location = new System.Drawing.Point(2, 312);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(548, 155, 250, 350);
            this.layoutControl7.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl7.Root = this.lcgPaperProperties;
            this.layoutControl7.Size = new System.Drawing.Size(258, 110);
            this.layoutControl7.TabIndex = 48;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // spinZoom
            // 
            this.spinZoom.EditValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinZoom.Location = new System.Drawing.Point(107, 78);
            this.spinZoom.MenuManager = this.barManager1;
            this.spinZoom.Name = "spinZoom";
            this.spinZoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinZoom.Properties.DisplayFormat.FormatString = "N00";
            this.spinZoom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinZoom.Properties.MaxValue = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.spinZoom.Properties.MinValue = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.spinZoom.Size = new System.Drawing.Size(139, 20);
            this.spinZoom.StyleController = this.layoutControl7;
            this.spinZoom.TabIndex = 10;
            this.spinZoom.ValueChanged += new System.EventHandler(this.spinZoom_ValueChanged);
            // 
            // cboSourcePage
            // 
            this.cboSourcePage.Location = new System.Drawing.Point(107, 30);
            this.cboSourcePage.Name = "cboSourcePage";
            this.cboSourcePage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSourcePage.Properties.PopupSizeable = true;
            this.cboSourcePage.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboSourcePage.Size = new System.Drawing.Size(139, 20);
            this.cboSourcePage.StyleController = this.layoutControl7;
            this.cboSourcePage.TabIndex = 8;
            // 
            // cboPaperSize
            // 
            this.cboPaperSize.Location = new System.Drawing.Point(107, 54);
            this.cboPaperSize.MenuManager = this.barManager1;
            this.cboPaperSize.Name = "cboPaperSize";
            this.cboPaperSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPaperSize.Properties.NullText = "";
            this.cboPaperSize.Properties.View = this.gridLookUpEdit1View;
            this.cboPaperSize.Size = new System.Drawing.Size(139, 20);
            this.cboPaperSize.StyleController = this.layoutControl7;
            this.cboPaperSize.TabIndex = 9;
            this.cboPaperSize.EditValueChanged += new System.EventHandler(this.cboPaperSize_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // lcgPaperProperties
            // 
            this.lcgPaperProperties.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgPaperProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciSource,
            this.lciSize,
            this.lciZoom});
            this.lcgPaperProperties.Location = new System.Drawing.Point(0, 0);
            this.lcgPaperProperties.Name = "lcgPaperProperties";
            this.lcgPaperProperties.Size = new System.Drawing.Size(258, 110);
            this.lcgPaperProperties.Text = "Paper Properties";
            // 
            // lciSource
            // 
            this.lciSource.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSource.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSource.Control = this.cboSourcePage;
            this.lciSource.Location = new System.Drawing.Point(0, 0);
            this.lciSource.Name = "lciSource";
            this.lciSource.Size = new System.Drawing.Size(238, 24);
            this.lciSource.Text = "Source:";
            this.lciSource.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSource.TextSize = new System.Drawing.Size(90, 13);
            this.lciSource.TextToControlDistance = 5;
            this.lciSource.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciSize
            // 
            this.lciSize.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSize.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSize.Control = this.cboPaperSize;
            this.lciSize.Location = new System.Drawing.Point(0, 24);
            this.lciSize.Name = "lciSize";
            this.lciSize.Size = new System.Drawing.Size(238, 24);
            this.lciSize.Text = "Size:";
            this.lciSize.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSize.TextSize = new System.Drawing.Size(90, 13);
            this.lciSize.TextToControlDistance = 5;
            // 
            // lciZoom
            // 
            this.lciZoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciZoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciZoom.Control = this.spinZoom;
            this.lciZoom.Location = new System.Drawing.Point(0, 48);
            this.lciZoom.Name = "lciZoom";
            this.lciZoom.Size = new System.Drawing.Size(238, 24);
            this.lciZoom.Text = "Zoom:";
            this.lciZoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciZoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciZoom.TextToControlDistance = 5;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.ChkHidePrinting);
            this.layoutControl2.Controls.Add(this.chkBlackAndWhite);
            this.layoutControl2.Controls.Add(this.cboPrinters);
            this.layoutControl2.Location = new System.Drawing.Point(2, 216);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(258, 92);
            this.layoutControl2.TabIndex = 47;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // ChkHidePrinting
            // 
            this.ChkHidePrinting.Location = new System.Drawing.Point(107, 59);
            this.ChkHidePrinting.MenuManager = this.barManager1;
            this.ChkHidePrinting.Name = "ChkHidePrinting";
            this.ChkHidePrinting.Properties.Caption = "";
            this.ChkHidePrinting.Size = new System.Drawing.Size(139, 19);
            this.ChkHidePrinting.StyleController = this.layoutControl2;
            this.ChkHidePrinting.TabIndex = 8;
            this.ChkHidePrinting.CheckedChanged += new System.EventHandler(this.ChkHidePrinting_CheckedChanged);
            // 
            // chkBlackAndWhite
            // 
            this.chkBlackAndWhite.Location = new System.Drawing.Point(107, 36);
            this.chkBlackAndWhite.MenuManager = this.barManager1;
            this.chkBlackAndWhite.Name = "chkBlackAndWhite";
            this.chkBlackAndWhite.Properties.Caption = "";
            this.chkBlackAndWhite.Size = new System.Drawing.Size(139, 19);
            this.chkBlackAndWhite.StyleController = this.layoutControl2;
            this.chkBlackAndWhite.TabIndex = 7;
            // 
            // cboPrinters
            // 
            this.cboPrinters.Location = new System.Drawing.Point(107, 12);
            this.cboPrinters.Name = "cboPrinters";
            this.cboPrinters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPrinters.Properties.PopupSizeable = true;
            this.cboPrinters.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboPrinters.Size = new System.Drawing.Size(139, 20);
            this.cboPrinters.StyleController = this.layoutControl2;
            this.cboPrinters.TabIndex = 6;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciAvailablePrinters,
            this.lciBlackAndWhite,
            this.layoutControlItem26});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(258, 92);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // lciAvailablePrinters
            // 
            this.lciAvailablePrinters.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAvailablePrinters.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAvailablePrinters.Control = this.cboPrinters;
            this.lciAvailablePrinters.Location = new System.Drawing.Point(0, 0);
            this.lciAvailablePrinters.Name = "lciAvailablePrinters";
            this.lciAvailablePrinters.Size = new System.Drawing.Size(238, 24);
            this.lciAvailablePrinters.Text = "Available Printers:";
            this.lciAvailablePrinters.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAvailablePrinters.TextSize = new System.Drawing.Size(90, 13);
            this.lciAvailablePrinters.TextToControlDistance = 5;
            // 
            // lciBlackAndWhite
            // 
            this.lciBlackAndWhite.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBlackAndWhite.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBlackAndWhite.Control = this.chkBlackAndWhite;
            this.lciBlackAndWhite.Location = new System.Drawing.Point(0, 24);
            this.lciBlackAndWhite.Name = "lciBlackAndWhite";
            this.lciBlackAndWhite.Size = new System.Drawing.Size(238, 23);
            this.lciBlackAndWhite.Text = "Black And White:";
            this.lciBlackAndWhite.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBlackAndWhite.TextSize = new System.Drawing.Size(90, 13);
            this.lciBlackAndWhite.TextToControlDistance = 5;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem26.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem26.Control = this.ChkHidePrinting;
            this.layoutControlItem26.Location = new System.Drawing.Point(0, 47);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.OptionsToolTip.ToolTip = "Ẩn cửa sổ thông báo trạng thái khi đang in";
            this.layoutControlItem26.Size = new System.Drawing.Size(238, 25);
            this.layoutControlItem26.Text = "Ẩn TB đang in:";
            this.layoutControlItem26.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem26.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem26.TextToControlDistance = 5;
            // 
            // layoutControl6
            // 
            this.layoutControl6.Controls.Add(this.btnOpenTutorial);
            this.layoutControl6.Controls.Add(this.btnOpenFileTemplate);
            this.layoutControl6.Controls.Add(this.BtnPrintLog);
            this.layoutControl6.Controls.Add(this.btnEmr);
            this.layoutControl6.Controls.Add(this.btnTemplateKey);
            this.layoutControl6.Controls.Add(this.btnExportAsImage);
            this.layoutControl6.Controls.Add(this.btnExportAsExcel);
            this.layoutControl6.Controls.Add(this.btnExportAsPdf);
            this.layoutControl6.Location = new System.Drawing.Point(2, 515);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl6.Root = this.layoutControlGroup5;
            this.layoutControl6.Size = new System.Drawing.Size(258, 118);
            this.layoutControl6.TabIndex = 46;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // btnOpenTutorial
            // 
            this.btnOpenTutorial.Location = new System.Drawing.Point(12, 78);
            this.btnOpenTutorial.Name = "btnOpenTutorial";
            this.btnOpenTutorial.Size = new System.Drawing.Size(155, 28);
            this.btnOpenTutorial.TabIndex = 23;
            this.btnOpenTutorial.Text = "Hướng dẫn sử dụng";
            this.btnOpenTutorial.UseVisualStyleBackColor = true;
            this.btnOpenTutorial.Click += new System.EventHandler(this.btnOpenTutorial_Click);
            // 
            // btnOpenFileTemplate
            // 
            this.btnOpenFileTemplate.Location = new System.Drawing.Point(12, 36);
            this.btnOpenFileTemplate.Name = "btnOpenFileTemplate";
            this.btnOpenFileTemplate.Size = new System.Drawing.Size(78, 38);
            this.btnOpenFileTemplate.StyleController = this.layoutControl6;
            this.btnOpenFileTemplate.TabIndex = 22;
            this.btnOpenFileTemplate.Text = "Mở file mẫu";
            this.btnOpenFileTemplate.Click += new System.EventHandler(this.btnOpenFileTemplate_Click);
            // 
            // BtnPrintLog
            // 
            this.BtnPrintLog.Location = new System.Drawing.Point(171, 78);
            this.BtnPrintLog.Name = "BtnPrintLog";
            this.BtnPrintLog.Size = new System.Drawing.Size(75, 28);
            this.BtnPrintLog.TabIndex = 21;
            this.BtnPrintLog.Text = "Lịch sử in";
            this.BtnPrintLog.UseVisualStyleBackColor = true;
            this.BtnPrintLog.Click += new System.EventHandler(this.BtnPrintLog_Click);
            // 
            // btnEmr
            // 
            this.btnEmr.Location = new System.Drawing.Point(171, 36);
            this.btnEmr.Name = "btnEmr";
            this.btnEmr.Size = new System.Drawing.Size(75, 38);
            this.btnEmr.TabIndex = 21;
            this.btnEmr.Text = "EMR";
            this.btnEmr.UseVisualStyleBackColor = true;
            this.btnEmr.Click += new System.EventHandler(this.btnEmr_Click);
            // 
            // btnTemplateKey
            // 
            this.btnTemplateKey.Location = new System.Drawing.Point(94, 36);
            this.btnTemplateKey.Name = "btnTemplateKey";
            this.btnTemplateKey.Size = new System.Drawing.Size(73, 38);
            this.btnTemplateKey.TabIndex = 20;
            this.btnTemplateKey.Text = "Template Key";
            this.btnTemplateKey.UseVisualStyleBackColor = true;
            this.btnTemplateKey.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem20,
            this.layoutControlItem21,
            this.layoutControlItem22,
            this.layoutControlItem35,
            this.layoutControlItem5,
            this.layoutControlItem17,
            this.lciForbtnOpenFileTemplate,
            this.layoutControlItem27});
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Size = new System.Drawing.Size(258, 118);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.Control = this.btnExportAsExcel;
            this.layoutControlItem20.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(82, 24);
            this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem20.TextVisible = false;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.Control = this.btnExportAsPdf;
            this.layoutControlItem21.Location = new System.Drawing.Point(82, 0);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(77, 24);
            this.layoutControlItem21.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem21.TextVisible = false;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.Control = this.btnExportAsImage;
            this.layoutControlItem22.Location = new System.Drawing.Point(159, 0);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(79, 24);
            this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem22.TextVisible = false;
            // 
            // layoutControlItem35
            // 
            this.layoutControlItem35.Control = this.btnTemplateKey;
            this.layoutControlItem35.Location = new System.Drawing.Point(82, 24);
            this.layoutControlItem35.Name = "layoutControlItem35";
            this.layoutControlItem35.Size = new System.Drawing.Size(77, 42);
            this.layoutControlItem35.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem35.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnEmr;
            this.layoutControlItem5.Location = new System.Drawing.Point(159, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(79, 42);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.BtnPrintLog;
            this.layoutControlItem17.Location = new System.Drawing.Point(159, 66);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(79, 32);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // lciForbtnOpenFileTemplate
            // 
            this.lciForbtnOpenFileTemplate.Control = this.btnOpenFileTemplate;
            this.lciForbtnOpenFileTemplate.Location = new System.Drawing.Point(0, 24);
            this.lciForbtnOpenFileTemplate.MaxSize = new System.Drawing.Size(0, 42);
            this.lciForbtnOpenFileTemplate.MinSize = new System.Drawing.Size(67, 42);
            this.lciForbtnOpenFileTemplate.Name = "lciForbtnOpenFileTemplate";
            this.lciForbtnOpenFileTemplate.Size = new System.Drawing.Size(82, 42);
            this.lciForbtnOpenFileTemplate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciForbtnOpenFileTemplate.TextSize = new System.Drawing.Size(0, 0);
            this.lciForbtnOpenFileTemplate.TextVisible = false;
            // 
            // layoutControlItem27
            // 
            this.layoutControlItem27.Control = this.btnOpenTutorial;
            this.layoutControlItem27.Location = new System.Drawing.Point(0, 66);
            this.layoutControlItem27.Name = "layoutControlItem27";
            this.layoutControlItem27.Size = new System.Drawing.Size(159, 32);
            this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem27.TextVisible = false;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.rdLandscape);
            this.layoutControl4.Controls.Add(this.rdPartrain);
            this.layoutControl4.Location = new System.Drawing.Point(2, 77);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl4.Root = this.lcgPaperOrientation;
            this.layoutControl4.Size = new System.Drawing.Size(258, 68);
            this.layoutControl4.TabIndex = 44;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // lcgPaperOrientation
            // 
            this.lcgPaperOrientation.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgPaperOrientation.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem12,
            this.layoutControlItem13});
            this.lcgPaperOrientation.Location = new System.Drawing.Point(0, 0);
            this.lcgPaperOrientation.Name = "lcgPaperOrientation";
            this.lcgPaperOrientation.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lcgPaperOrientation.Size = new System.Drawing.Size(258, 68);
            this.lcgPaperOrientation.Text = "Paper Orientation";
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.rdPartrain;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.MaxSize = new System.Drawing.Size(0, 29);
            this.layoutControlItem12.MinSize = new System.Drawing.Size(10, 10);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(117, 28);
            this.layoutControlItem12.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextToControlDistance = 0;
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.rdLandscape;
            this.layoutControlItem13.Location = new System.Drawing.Point(117, 0);
            this.layoutControlItem13.MaxSize = new System.Drawing.Size(0, 29);
            this.layoutControlItem13.MinSize = new System.Drawing.Size(10, 10);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(119, 28);
            this.layoutControlItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextToControlDistance = 0;
            this.layoutControlItem13.TextVisible = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem19,
            this.layoutControlItem1,
            this.layoutControlItem25,
            this.lciNumericUpDownCopies,
            this.lcgCenterOnPage});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(262, 635);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnPrint;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(132, 75);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnPageSetup;
            this.layoutControlItem10.Location = new System.Drawing.Point(132, 24);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(130, 25);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.btnPrintSetup;
            this.layoutControlItem11.Location = new System.Drawing.Point(132, 49);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(130, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.layoutControl4;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 75);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(262, 72);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.layoutControl5;
            this.layoutControlItem15.Location = new System.Drawing.Point(0, 424);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(262, 89);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.layoutControl6;
            this.layoutControlItem19.Location = new System.Drawing.Point(0, 513);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(262, 122);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 214);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(262, 96);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.Control = this.layoutControl7;
            this.layoutControlItem25.Location = new System.Drawing.Point(0, 310);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Size = new System.Drawing.Size(262, 114);
            this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem25.TextVisible = false;
            // 
            // lciNumericUpDownCopies
            // 
            this.lciNumericUpDownCopies.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumericUpDownCopies.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumericUpDownCopies.Control = this.numericUpDownCopies;
            this.lciNumericUpDownCopies.Location = new System.Drawing.Point(132, 0);
            this.lciNumericUpDownCopies.Name = "lciNumericUpDownCopies";
            this.lciNumericUpDownCopies.Size = new System.Drawing.Size(130, 24);
            this.lciNumericUpDownCopies.Text = "Copies:";
            this.lciNumericUpDownCopies.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumericUpDownCopies.TextSize = new System.Drawing.Size(50, 20);
            this.lciNumericUpDownCopies.TextToControlDistance = 5;
            // 
            // lcgCenterOnPage
            // 
            this.lcgCenterOnPage.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgCenterOnPage.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem9,
            this.layoutControlItem33});
            this.lcgCenterOnPage.Location = new System.Drawing.Point(0, 147);
            this.lcgCenterOnPage.Name = "lcgCenterOnPage";
            this.lcgCenterOnPage.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lcgCenterOnPage.Size = new System.Drawing.Size(262, 67);
            this.lcgCenterOnPage.Text = "Center on page";
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.chkHorizontally;
            this.layoutControlItem9.Location = new System.Drawing.Point(118, 0);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(0, 23);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(82, 23);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(118, 23);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.Text = "layout";
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem33
            // 
            this.layoutControlItem33.Control = this.chkVertically;
            this.layoutControlItem33.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem33.MaxSize = new System.Drawing.Size(0, 23);
            this.layoutControlItem33.MinSize = new System.Drawing.Size(69, 23);
            this.layoutControlItem33.Name = "layoutControlItem33";
            this.layoutControlItem33.Size = new System.Drawing.Size(118, 23);
            this.layoutControlItem33.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem33.Text = "lay";
            this.layoutControlItem33.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem33.TextVisible = false;
            // 
            // lblOf
            // 
            this.lblOf.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblOf.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblOf.Location = new System.Drawing.Point(442, 592);
            this.lblOf.Name = "lblOf";
            this.lblOf.Size = new System.Drawing.Size(11, 31);
            this.lblOf.StyleController = this.layoutControl1;
            this.lblOf.TabIndex = 46;
            this.lblOf.Text = "/";
            // 
            // spinChangePage
            // 
            this.spinChangePage.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinChangePage.Location = new System.Drawing.Point(367, 592);
            this.spinChangePage.Name = "spinChangePage";
            this.spinChangePage.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.spinChangePage.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.spinChangePage.Properties.Appearance.Options.UseFont = true;
            this.spinChangePage.Properties.AutoHeight = false;
            this.spinChangePage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinChangePage.Size = new System.Drawing.Size(61, 31);
            this.spinChangePage.StyleController = this.layoutControl1;
            this.spinChangePage.TabIndex = 19;
            this.spinChangePage.EditValueChanged += new System.EventHandler(this.spinChangePage_EditValueChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.printPreviewControl1);
            this.panel2.Location = new System.Drawing.Point(268, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(884, 586);
            this.panel2.TabIndex = 41;
            // 
            // printPreviewControl1
            // 
            this.printPreviewControl1.AutoZoom = false;
            this.printPreviewControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.printPreviewControl1.Location = new System.Drawing.Point(0, 0);
            this.printPreviewControl1.Name = "printPreviewControl1";
            this.printPreviewControl1.Size = new System.Drawing.Size(884, 586);
            this.printPreviewControl1.TabIndex = 2;
            this.printPreviewControl1.Zoom = 0.85D;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.lciPage,
            this.layoutControlItem6,
            this.layoutControlItem8,
            this.layoutControlItem7,
            this.layoutControlItem23,
            this.layoutControlItem24,
            this.emptySpaceItem3,
            this.emptySpaceItem4,
            this.emptySpaceItem5,
            this.emptySpaceItem6,
            this.emptySpaceItem2,
            this.emptySpaceItem8});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1154, 639);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panel2;
            this.layoutControlItem2.Location = new System.Drawing.Point(266, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(888, 590);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.layoutControl3;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(266, 639);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // lciPage
            // 
            this.lciPage.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPage.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPage.Control = this.spinChangePage;
            this.lciPage.Location = new System.Drawing.Point(365, 590);
            this.lciPage.MaxSize = new System.Drawing.Size(0, 35);
            this.lciPage.MinSize = new System.Drawing.Size(40, 30);
            this.lciPage.Name = "lciPage";
            this.lciPage.Size = new System.Drawing.Size(65, 35);
            this.lciPage.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPage.Text = "Page:";
            this.lciPage.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPage.TextSize = new System.Drawing.Size(0, 0);
            this.lciPage.TextToControlDistance = 0;
            this.lciPage.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.lblOf;
            this.layoutControlItem6.Location = new System.Drawing.Point(440, 590);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(15, 35);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(15, 33);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(15, 35);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.trackBarZoomPage;
            this.layoutControlItem8.Location = new System.Drawing.Point(645, 590);
            this.layoutControlItem8.MaxSize = new System.Drawing.Size(0, 49);
            this.layoutControlItem8.MinSize = new System.Drawing.Size(49, 49);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(509, 49);
            this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.lblTotalPage;
            this.layoutControlItem7.Location = new System.Drawing.Point(455, 590);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(14, 33);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(53, 35);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.Control = this.btnPrePage;
            this.layoutControlItem23.Location = new System.Drawing.Point(315, 590);
            this.layoutControlItem23.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem23.MinSize = new System.Drawing.Size(40, 35);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Size = new System.Drawing.Size(40, 35);
            this.layoutControlItem23.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem23.TextVisible = false;
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.btnNextPage;
            this.layoutControlItem24.Location = new System.Drawing.Point(518, 590);
            this.layoutControlItem24.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem24.MinSize = new System.Drawing.Size(40, 35);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Size = new System.Drawing.Size(44, 35);
            this.layoutControlItem24.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(266, 590);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(49, 35);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(562, 590);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(83, 35);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(430, 590);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(10, 35);
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem6
            // 
            this.emptySpaceItem6.AllowHotTrack = false;
            this.emptySpaceItem6.Location = new System.Drawing.Point(508, 590);
            this.emptySpaceItem6.Name = "emptySpaceItem6";
            this.emptySpaceItem6.Size = new System.Drawing.Size(10, 35);
            this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(355, 590);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(10, 35);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem8
            // 
            this.emptySpaceItem8.AllowHotTrack = false;
            this.emptySpaceItem8.Location = new System.Drawing.Point(266, 625);
            this.emptySpaceItem8.Name = "emptySpaceItem8";
            this.emptySpaceItem8.Size = new System.Drawing.Size(379, 14);
            this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
            // 
            // exportImageDialog
            // 
            this.exportImageDialog.DefaultExt = "png";
            this.exportImageDialog.Filter = "Png files|*.png|Jpg files|*.jpg";
            this.exportImageDialog.Title = "Save image as...";
            // 
            // exportPdfDialog
            // 
            this.exportPdfDialog.DefaultExt = "pdf";
            this.exportPdfDialog.Filter = "Pdf files|*.pdf";
            // 
            // exportExcelDialog
            // 
            this.exportExcelDialog.DefaultExt = "xlsx";
            this.exportExcelDialog.Filter = "Excel files|*.xlsx";
            // 
            // pageSetupDialog1
            // 
            this.pageSetupDialog1.Document = this.flexCelPrintDocument1;
            // 
            // exportTiffDialog
            // 
            this.exportTiffDialog.DefaultExt = "tif";
            this.exportTiffDialog.Filter = "TIFF Files|*.tif";
            this.exportTiffDialog.Title = "Save image as multi page tiff...";
            // 
            // bbtnRemoteSupport
            // 
            this.bbtnRemoteSupport.Caption = "RemoteSupport";
            this.bbtnRemoteSupport.Id = 2;
            this.bbtnRemoteSupport.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2));
            this.bbtnRemoteSupport.Name = "bbtnRemoteSupport";
            this.bbtnRemoteSupport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRemoteSupport_ItemClick);
            // 
            // frmSetupPrintPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 661);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "frmSetupPrintPreview";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print preview";
            this.Load += new System.EventHandler(this.frmSetupPrintPreview_Load);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.frmSetupPrintPreview_MouseWheel);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCurrentPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPages.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFromPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllPages.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPageRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkVertically.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHorizontally.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinZoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSourcePage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaperSize.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPaperProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChkHidePrinting.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBlackAndWhite.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinters.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAvailablePrinters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBlackAndWhite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            this.layoutControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem35)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForbtnOpenFileTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lcgPaperOrientation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumericUpDownCopies)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgCenterOnPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinChangePage.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panel2;
        private PrintPreviewControl printPreviewControl1;
        private SaveFileDialog exportPdfDialog;
        private SaveFileDialog exportExcelDialog;
        private Button btnPageSetup;
        private Button btnPrint;
        private RadioButton rdLandscape;
        private RadioButton rdPartrain;
        private Button btnExportAsImage;
        private Button btnExportAsPdf;
        private Button btnExportAsExcel;
        private PageSetupDialog pageSetupDialog1;
        private SaveFileDialog exportTiffDialog;
        private DevExpress.XtraEditors.CheckEdit chkAllPages;
        private DevExpress.XtraEditors.CheckEdit chkCurrentPage;
        private Button btnPrintSetup;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnPrintShortcut;
        private DevExpress.XtraEditors.SpinEdit spinChangePage;
        private DevExpress.XtraEditors.LabelControl lblTotalPage;
        private DevExpress.XtraEditors.LabelControl lblOf;
        private DevExpress.XtraEditors.TrackBarControl trackBarZoomPage;
        private DevExpress.XtraBars.BarButtonItem barButtonItemClose;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem lciPage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControlGroup lcgPaperOrientation;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        private DevExpress.XtraLayout.LayoutControlGroup lcgPageRange;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControl layoutControl6;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.ComboBoxEdit cboPrinters;
        private DevExpress.XtraLayout.LayoutControlItem lciAvailablePrinters;
        private DevExpress.XtraEditors.CheckEdit chkBlackAndWhite;
        private DevExpress.XtraLayout.LayoutControlItem lciBlackAndWhite;
        private DevExpress.XtraLayout.LayoutControl layoutControl7;
        private DevExpress.XtraLayout.LayoutControlGroup lcgPaperProperties;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraEditors.ComboBoxEdit cboSourcePage;
        private DevExpress.XtraEditors.GridLookUpEdit cboPaperSize;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciSource;
        private DevExpress.XtraLayout.LayoutControlItem lciSize;
        private DevExpress.XtraEditors.SpinEdit numericUpDownFromPage;
        private DevExpress.XtraEditors.SpinEdit numericUpDownToPage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
        private DevExpress.XtraEditors.CheckEdit chkPages;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraEditors.SpinEdit numericUpDownCopies;
        private DevExpress.XtraLayout.LayoutControlItem lciNumericUpDownCopies;
        private DevExpress.XtraEditors.CheckEdit chkVertically;
        private DevExpress.XtraEditors.CheckEdit chkHorizontally;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem33;
        private DevExpress.XtraLayout.LayoutControlGroup lcgCenterOnPage;
        private Button btnTemplateKey;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem35;
        private DevExpress.XtraEditors.SpinEdit spinZoom;
        private DevExpress.XtraLayout.LayoutControlItem lciZoom;
        private Button btnEmr;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private Button BtnPrintLog;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.SimpleButton btnNextPage;
        private DevExpress.XtraEditors.SimpleButton btnPrePage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem8;
        private DevExpress.XtraEditors.SimpleButton btnOpenFileTemplate;
        private DevExpress.XtraLayout.LayoutControlItem lciForbtnOpenFileTemplate;
        private DevExpress.XtraEditors.CheckEdit ChkHidePrinting;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private Button btnOpenTutorial;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem27;
        private DevExpress.XtraBars.BarButtonItem bbtnRemoteSupport;
    }
}

