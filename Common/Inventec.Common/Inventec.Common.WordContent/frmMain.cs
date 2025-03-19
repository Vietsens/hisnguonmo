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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using Inventec.Common.SignLibrary;
using System.IO;
using SAR.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.WordContent.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.WordContent.SarPrint;
using Inventec.Core;
using AutoMapper;
using Inventec.Common.WordContent.Base;
using System.Text.RegularExpressions;

namespace Inventec.Common.WordContent
{
    /// <summary>
    /// just testing
    /// </summary>
    public class frmMain : System.Windows.Forms.Form
    {
        private WinWordControl.WinWordControl winWordControl1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnOpenTemplate;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnOpenEMR;
        private DevExpress.XtraBars.BarButtonItem bbtnTemplateKey;
        private IContainer components;

        bool isViewOnly;
        string fileName;
        string templateFileName;
        Inventec.Common.SignLibrary.ADO.InputADO emrInputADO;
        private Timer timer1;
        Dictionary<string, object> templateKey;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        Action<SAR.EFMODEL.DataModels.SAR_PRINT> actUpdateReference;
        int positionHandleControl = -1;
        SAR_PRINT oldSarPrint;
        SAR_PRINT saveResultPrint = null;
        SAR.EFMODEL.DataModels.SAR_PRINT_TYPE sarPrintType;
        private DevExpress.XtraBars.BarEditItem txtTitle;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraBars.BarEditItem txtDescription;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private TextEdit txtTitle1;
        private TextEdit txtDescription1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;

        string fileSavePath = "";
        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(WordContentADO wordContentADO)
        {
            InitializeComponent();
            try
            {
                if (wordContentADO != null)
                {
                    this.fileName = wordContentADO.FileName;
                    this.templateFileName = wordContentADO.TemplateFileName;
                    this.emrInputADO = wordContentADO.EmrInputADO;
                    this.templateKey = wordContentADO.TemplateKey;
                    this.actUpdateReference = wordContentADO.ActUpdateReference;
                    this.oldSarPrint = wordContentADO.OldSarPrint;
                    this.sarPrintType = wordContentADO.SarPrintType;

                    if (wordContentADO.IsViewOnly.HasValue)
                    {
                        isViewOnly = wordContentADO.IsViewOnly.Value;
                        bbtnSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }
                }
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch { }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// cleanuup ressources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // just to be shure!
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Dispose>>winWordControl1.CloseControl.1");
                winWordControl1.CloseControl();
                Inventec.Common.Logging.LogSystem.Info("Dispose>>winWordControl1.CloseControl.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        ///
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.winWordControl1 = new WinWordControl.WinWordControl();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnOpenEMR = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnTemplateKey = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnOpenTemplate = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.txtTitle = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.txtDescription = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.timer1 = new System.Windows.Forms.Timer();
            this.dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtTitle1 = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription1 = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // winWordControl1
            // 
            this.winWordControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winWordControl1.Location = new System.Drawing.Point(0, 78);
            this.winWordControl1.Name = "winWordControl1";
            this.winWordControl1.Size = new System.Drawing.Size(1262, 596);
            this.winWordControl1.TabIndex = 2;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "WordDateien (*.doc)|*.doc";
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnOpenTemplate,
            this.bbtnOpenEMR,
            this.bbtnTemplateKey,
            this.bbtnSave,
            this.txtTitle,
            this.txtDescription});
            this.barManager1.MaxItemId = 6;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.repositoryItemTextEdit2});
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnOpenEMR),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnTemplateKey),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnOpenTemplate),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Text = "Tools";
            // 
            // bbtnOpenEMR
            // 
            this.bbtnOpenEMR.Caption = "EMR";
            this.bbtnOpenEMR.Glyph = ((System.Drawing.Image)(resources.GetObject("bbtnOpenEMR.Glyph")));
            this.bbtnOpenEMR.Id = 1;
            this.bbtnOpenEMR.Name = "bbtnOpenEMR";
            this.bbtnOpenEMR.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bbtnOpenEMR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnOpenEMR_ItemClick);
            // 
            // bbtnTemplateKey
            // 
            this.bbtnTemplateKey.Caption = "Template key";
            this.bbtnTemplateKey.Glyph = ((System.Drawing.Image)(resources.GetObject("bbtnTemplateKey.Glyph")));
            this.bbtnTemplateKey.Id = 2;
            this.bbtnTemplateKey.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbtnTemplateKey.LargeGlyph")));
            this.bbtnTemplateKey.Name = "bbtnTemplateKey";
            this.bbtnTemplateKey.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bbtnTemplateKey.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnTemplateKey_ItemClick);
            // 
            // bbtnOpenTemplate
            // 
            this.bbtnOpenTemplate.Caption = "Mở file mẫu";
            this.bbtnOpenTemplate.Glyph = ((System.Drawing.Image)(resources.GetObject("bbtnOpenTemplate.Glyph")));
            this.bbtnOpenTemplate.Id = 0;
            this.bbtnOpenTemplate.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbtnOpenTemplate.LargeGlyph")));
            this.bbtnOpenTemplate.Name = "bbtnOpenTemplate";
            this.bbtnOpenTemplate.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bbtnOpenTemplate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnOpenTemplate_ItemClick);
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu";
            this.bbtnSave.Glyph = ((System.Drawing.Image)(resources.GetObject("bbtnSave.Glyph")));
            this.bbtnSave.Id = 3;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbtnSave.LargeGlyph")));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1262, 31);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 674);
            this.barDockControlBottom.Size = new System.Drawing.Size(1262, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 643);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1262, 31);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 643);
            // 
            // txtTitle
            // 
            this.txtTitle.Caption = "Tiêu đề:";
            this.txtTitle.Edit = this.repositoryItemTextEdit1;
            this.txtTitle.Id = 4;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // txtDescription
            // 
            this.txtDescription.Caption = "Mô tả:";
            this.txtDescription.Edit = this.repositoryItemTextEdit2;
            this.txtDescription.Id = 5;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // repositoryItemTextEdit2
            // 
            this.repositoryItemTextEdit2.AutoHeight = false;
            this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // dxValidationProvider
            // 
            this.dxValidationProvider.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider_ValidationFailed);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtTitle1);
            this.layoutControl1.Controls.Add(this.txtDescription1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.Location = new System.Drawing.Point(0, 31);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1262, 47);
            this.layoutControl1.TabIndex = 7;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtTitle1
            // 
            this.txtTitle1.Location = new System.Drawing.Point(87, 12);
            this.txtTitle1.Name = "txtTitle1";
            this.txtTitle1.Size = new System.Drawing.Size(541, 20);
            this.txtTitle1.StyleController = this.layoutControl1;
            this.txtTitle1.TabIndex = 5;
            // 
            // txtDescription1
            // 
            this.txtDescription1.Location = new System.Drawing.Point(707, 12);
            this.txtDescription1.Name = "txtDescription1";
            this.txtDescription1.Size = new System.Drawing.Size(543, 20);
            this.txtDescription1.StyleController = this.layoutControl1;
            this.txtDescription1.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1262, 47);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.txtTitle1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(620, 27);
            this.layoutControlItem1.Text = "Tiêu đề:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.txtDescription1;
            this.layoutControlItem2.Location = new System.Drawing.Point(620, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(622, 27);
            this.layoutControlItem2.Text = "Mô tả:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1262, 674);
            this.Controls.Add(this.winWordControl1);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.OnActivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void OnActivate(object sender, System.EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string strTitle = "", strDescription = "";

            if (oldSarPrint != null && oldSarPrint.CONTENT != null)
            {
                strDescription = this.oldSarPrint.DESCRIPTION;
                strTitle = this.oldSarPrint.TITLE;


                fileName = Utils.GetFullPathFile(Guid.NewGuid().ToString() + ".docx");
                Utils.ByteToFile(oldSarPrint.CONTENT, fileName);
            }
            else
            {
                if (this.sarPrintType != null)
                {
                    strDescription = this.sarPrintType.FILE_PATTERN;
                    strTitle = this.sarPrintType.PRINT_TYPE_NAME;
                }
                else
                {
                    string fileNameSub = "", valueStr = "";
                    if (!String.IsNullOrWhiteSpace(this.fileName) && this.fileName.Length > 5)
                    {
                        fileNameSub = this.fileName.Substring(0, this.fileName.Length - 5);
                        if (!String.IsNullOrWhiteSpace(fileNameSub) && fileNameSub.Length > 0)
                        {
                            int index = fileNameSub.LastIndexOf("\\");
                            if (index != -1)
                            {
                                valueStr = fileNameSub.Substring(index + 1);
                            }
                        }
                    }
                    strDescription = !String.IsNullOrWhiteSpace(valueStr) && valueStr.Length > 0 ? valueStr : this.fileName;
                    strTitle = !String.IsNullOrWhiteSpace(valueStr) && valueStr.Length > 0 ? valueStr : this.fileName;
                }
            }
            txtDescription1.Text = strDescription;
            txtTitle1.Text = strTitle;

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sarPrintType), sarPrintType)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strDescription), strDescription)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strTitle), strTitle)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName)
                );
            ValidControl();

            winWordControl1.SetDelegate(SaveBySaveAsDoc);

            timer1.Interval = 200;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                if (!String.IsNullOrEmpty(fileName))
                {
                    //ReplaceText(fileName);

                    winWordControl1.LoadDocument(fileName);
                    winWordControl1.MoveToTop();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveBySaveAsDoc(string saveFile)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SaveBySaveAsDoc.1");
                if (!isViewOnly)
                {
                    SaveItemClick(false);
                    winWordControl1.CloseControl();
                    if (File.Exists(fileSavePath))
                    {
                        winWordControl1.LoadDocument(fileSavePath);
                        winWordControl1.PrintPreview();
                    }
                }
                else
                {
                    MessageBox.Show("Bạn không có quyền sửa");
                    winWordControl1.CloseControl();
                    winWordControl1.LoadDocument(fileName);
                }
                Inventec.Common.Logging.LogSystem.Debug("SaveBySaveAsDoc.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static bool ReplaceText(string sourceFile)
        {
            bool success = false;
            try
            {
                // For complete examples and data files, please go to https://github.com/aspose-pdf/Aspose.PDF-for-.NET
                // The path to the documents directory.
                // Open document
                if (System.IO.File.Exists(sourceFile))
                {
                    License.LicenceProcess.SetLicenseForAspose();
                    Aspose.Words.Document pdfDocument = new Aspose.Words.Document(sourceFile);
                    string docText = pdfDocument.Range.Text;
                    var arrDoc = docText.Split(new string[] { "[[" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrDoc != null && arrDoc.Length > 0)
                    {
                        foreach (var item in arrDoc)
                        {
                            if (item.Contains("]]"))
                            {
                                try
                                {
                                    string strReplace = "[[" + item.Substring(0, item.IndexOf("]]") + 2);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strReplace), strReplace)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                    pdfDocument.Range.Replace(strReplace, "", false, false);
                                }
                                catch (Exception exx)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Replace key in docx file error____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item), exx);
                                }

                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("pdfDocument.Range.Text", docText));
                    // Save resulting PDF document.
                    pdfDocument.Save(sourceFile);//outFile
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sourceFile), sourceFile));
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnOpenTemplate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.templateFileName))
                {
                    System.Diagnostics.Process.Start(this.templateFileName);
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => templateFileName), templateFileName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnOpenEMR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                string fileNameCopy = Utils.GetFullPathFile(Guid.NewGuid().ToString() + ".docx");
                File.Copy(this.fileName, fileNameCopy, true);

                string fileNamePdf = Utils.GetFullPathFile(Guid.NewGuid().ToString() + ".pdf");

                Inventec.Common.Logging.LogSystem.Debug("WordContent.OpenEMR.1____"
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.fileName), this.fileName)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileNameCopy), fileNameCopy)
                   + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileNamePdf), fileNamePdf)
                   );

                if (Inventec.Common.Integrate.FileConvert.DocToPdf(null, fileNameCopy, null, fileNamePdf))
                {
                    Inventec.Common.Logging.LogSystem.Debug("WordContent.OpenEMR.2");
                    string base64FileContent = Convert.ToBase64String(File.ReadAllBytes(fileNamePdf));

                    libraryProcessor.ShowPopup(base64FileContent, FileType.Pdf, this.emrInputADO);
                    Inventec.Common.Logging.LogSystem.Debug("WordContent.OpenEMR.3");
                }
                Inventec.Common.Logging.LogSystem.Debug("WordContent.OpenEMR.4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnTemplateKey_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.templateKey != null && templateKey.Count > 0)
                {
                    PreviewTemplateKey previewTemplateKey = new PreviewTemplateKey(this.templateKey);
                    previewTemplateKey.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy danh sách key trong biểu mẫu");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveItemClick(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void SaveItemClick(bool showPreview)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("bbtnSave_ItemClick.1");
                if (this.isViewOnly)
                {
                    Inventec.Common.Logging.LogSystem.Info("Che do chi xem khong cho phep luu____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isViewOnly), isViewOnly));
                    return;
                }

                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;

                CommonParam param = new CommonParam();
                object rs = null;

                if (this.oldSarPrint != null && (this.oldSarPrint.ID <= 0 || this.oldSarPrint.CONTENT == null))
                {
                    param.Messages.Add(WordContentStorage.NguoiDungNhapDuLieuKhongHopLe);
                    Inventec.Desktop.Common.Message.MessageManager.Show(param, null);
                    return;
                }
                fileSavePath = Utils.GetFullPathFile(Guid.NewGuid().ToString() + ".docx");

                SAR_PRINT objPrint = new SAR_PRINT();
                if (this.oldSarPrint != null)
                {
                    Mapper.CreateMap<SAR_PRINT, SAR_PRINT>();
                    objPrint = Mapper.Map<SAR_PRINT, SAR_PRINT>(this.oldSarPrint);
                }
                Inventec.Common.Logging.LogSystem.Debug("bbtnSave_ItemClick.2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileSavePath), fileSavePath));
                objPrint.CONTENT = this.winWordControl1.GetWordStream(fileSavePath);
                Inventec.Common.Logging.LogSystem.Debug("bbtnSave_ItemClick.3");
                objPrint.TITLE = txtTitle1.Text.Trim();
                objPrint.DESCRIPTION = txtDescription1.Text.Trim();
                if (String.IsNullOrEmpty(objPrint.TITLE))
                {
                    objPrint.TITLE = Inventec.Common.DateTime.Get.NowAsDateSeparateString();
                }
                if (this.sarPrintType != null)
                {
                    objPrint.PRINT_TYPE_ID = this.sarPrintType.ID;
                }
                if (this.oldSarPrint != null)
                {
                    rs = new SarPrintLogic(param).Update(objPrint);
                }
                else
                {
                    rs = new SarPrintLogic(param).Create(objPrint);
                }
                Inventec.Common.Logging.LogSystem.Debug("bbtnSave_ItemClick.4");
                //quickPrintItem1.Enabled = printItem1.Enabled = printPreviewItem1.Enabled = true;//TODO

                if (rs != null)
                {
                    saveResultPrint = (SAR_PRINT)rs;
                    this.oldSarPrint = (SAR_PRINT)rs;
                    if (this.actUpdateReference != null)
                    {
                        try
                        {
                            this.actUpdateReference((SAR_PRINT)rs);
                        }
                        catch { }
                    }

                    if (showPreview)
                        ShowPrintPreview();

                    //this.Dispose();
                    //this.Close();
                }
                else
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show(param, null);
                }
                Inventec.Common.Logging.LogSystem.Debug("bbtnSave_ItemClick.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ShowPrintPreview()
        {
            try
            {
                if (this.saveResultPrint != null)
                {
                    this.bbtnSave.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    this.winWordControl1.PrintPreview();
                    //    frmOtherShow frm = new frmOtherShow(this.saveResultPrint.CONTENT, this.emrInputADO, this.dicParam);
                    //    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validate
        private void ValidateTitle()
        {
            PrintEditor__TitleValidationRule oDobDateRule = new PrintEditor__TitleValidationRule();
            oDobDateRule.txtTitle = txtTitle1;
            oDobDateRule.ErrorText = "Trường dữ liệu bắt buộc";
            oDobDateRule.ErrorType = ErrorType.Warning;
            //this.dxValidationProvider.SetValidationRule(txtTitle.Edit, oDobDateRule);
        }

        private void ValidControl()
        {
            try
            {
                ValidateTitle();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
