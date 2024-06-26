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
namespace SAR.Desktop.Plugins.SarImportReportTemplate.SarImportReportTemplate
{
    partial class frmSarImportReportTemplate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSarImportReportTemplate));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlImport = new DevExpress.XtraGrid.GridControl();
            this.gridViewImport = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColErr = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColDel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.grdColReportCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColReportName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColReportTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColExtension = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColTutorial = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnError = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnShowLineError = new DevExpress.XtraEditors.SimpleButton();
            this.btnChooseFile = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownLoadFile = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.a = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
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
            this.bbtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(884, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 511);
            this.barDockControlBottom.Size = new System.Drawing.Size(884, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 482);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(884, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 482);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlImport);
            this.layoutControl1.Controls.Add(this.btnImport);
            this.layoutControl1.Controls.Add(this.btnShowLineError);
            this.layoutControl1.Controls.Add(this.btnChooseFile);
            this.layoutControl1.Controls.Add(this.btnDownLoadFile);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(884, 482);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlImport
            // 
            this.gridControlImport.Location = new System.Drawing.Point(2, 28);
            this.gridControlImport.MainView = this.gridViewImport;
            this.gridControlImport.MenuManager = this.barManager1;
            this.gridControlImport.Name = "gridControlImport";
            this.gridControlImport.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnDelete,
            this.btnError});
            this.gridControlImport.Size = new System.Drawing.Size(880, 452);
            this.gridControlImport.TabIndex = 8;
            this.gridControlImport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewImport});
            // 
            // gridViewImport
            // 
            this.gridViewImport.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSTT,
            this.grdColErr,
            this.grdColDel,
            this.grdColReportCode,
            this.grdColReportName,
            this.grdColReportTypeCode,
            this.grdColExtension,
            this.grdColTutorial});
            this.gridViewImport.GridControl = this.gridControlImport;
            this.gridViewImport.Name = "gridViewImport";
            this.gridViewImport.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewImport.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewImport.OptionsFind.AllowFindPanel = false;
            this.gridViewImport.OptionsView.ShowGroupPanel = false;
            this.gridViewImport.OptionsView.ShowIndicator = false;
            this.gridViewImport.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewImport_CustomRowCellEdit);
            this.gridViewImport.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewImport_CustomUnboundColumnData);
            // 
            // grdColSTT
            // 
            this.grdColSTT.Caption = "STT";
            this.grdColSTT.FieldName = "STT";
            this.grdColSTT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColSTT.Name = "grdColSTT";
            this.grdColSTT.OptionsColumn.AllowEdit = false;
            this.grdColSTT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColSTT.Visible = true;
            this.grdColSTT.VisibleIndex = 0;
            this.grdColSTT.Width = 40;
            // 
            // grdColErr
            // 
            this.grdColErr.Caption = "Lỗi";
            this.grdColErr.FieldName = "ERROR";
            this.grdColErr.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColErr.Name = "grdColErr";
            this.grdColErr.OptionsColumn.ShowCaption = false;
            this.grdColErr.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColErr.Visible = true;
            this.grdColErr.VisibleIndex = 1;
            this.grdColErr.Width = 30;
            // 
            // grdColDel
            // 
            this.grdColDel.Caption = "Xóa";
            this.grdColDel.ColumnEdit = this.btnDelete;
            this.grdColDel.FieldName = "DELETE";
            this.grdColDel.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColDel.Name = "grdColDel";
            this.grdColDel.OptionsColumn.ShowCaption = false;
            this.grdColDel.Visible = true;
            this.grdColDel.VisibleIndex = 2;
            this.grdColDel.Width = 30;
            // 
            // btnDelete
            // 
            this.btnDelete.AutoHeight = false;
            this.btnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnDelete_ButtonClick);
            // 
            // grdColReportCode
            // 
            this.grdColReportCode.Caption = "Mã";
            this.grdColReportCode.FieldName = "REPORT_TEMPLATE_CODE";
            this.grdColReportCode.Name = "grdColReportCode";
            this.grdColReportCode.OptionsColumn.AllowEdit = false;
            this.grdColReportCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColReportCode.Visible = true;
            this.grdColReportCode.VisibleIndex = 3;
            this.grdColReportCode.Width = 100;
            // 
            // grdColReportName
            // 
            this.grdColReportName.Caption = "Tên";
            this.grdColReportName.FieldName = "REPORT_TEMPLATE_NAME";
            this.grdColReportName.Name = "grdColReportName";
            this.grdColReportName.OptionsColumn.AllowEdit = false;
            this.grdColReportName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColReportName.Visible = true;
            this.grdColReportName.VisibleIndex = 4;
            this.grdColReportName.Width = 140;
            // 
            // grdColReportTypeCode
            // 
            this.grdColReportTypeCode.Caption = "Mã loại báo cáo";
            this.grdColReportTypeCode.FieldName = "REPORT_TYPE_CODE";
            this.grdColReportTypeCode.Name = "grdColReportTypeCode";
            this.grdColReportTypeCode.OptionsColumn.AllowEdit = false;
            this.grdColReportTypeCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColReportTypeCode.Visible = true;
            this.grdColReportTypeCode.VisibleIndex = 5;
            this.grdColReportTypeCode.Width = 96;
            // 
            // grdColExtension
            // 
            this.grdColExtension.Caption = "Định dạng nhận";
            this.grdColExtension.FieldName = "EXTENSION_RECEIVE";
            this.grdColExtension.Name = "grdColExtension";
            this.grdColExtension.OptionsColumn.AllowEdit = false;
            this.grdColExtension.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColExtension.Visible = true;
            this.grdColExtension.VisibleIndex = 6;
            this.grdColExtension.Width = 150;
            // 
            // grdColTutorial
            // 
            this.grdColTutorial.Caption = "Hướng dẫn";
            this.grdColTutorial.FieldName = "TUTORIAL";
            this.grdColTutorial.Name = "grdColTutorial";
            this.grdColTutorial.OptionsColumn.AllowEdit = false;
            this.grdColTutorial.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColTutorial.Visible = true;
            this.grdColTutorial.VisibleIndex = 7;
            this.grdColTutorial.Width = 290;
            // 
            // btnError
            // 
            this.btnError.AutoHeight = false;
            this.btnError.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnError.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Lỗi", null, null, true)});
            this.btnError.Name = "btnError";
            this.btnError.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnError.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnError_ButtonClick);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(354, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(105, 22);
            this.btnImport.StyleController = this.layoutControl1;
            this.btnImport.TabIndex = 7;
            this.btnImport.Text = "Lưu (Ctrl S)";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnShowLineError
            // 
            this.btnShowLineError.Location = new System.Drawing.Point(242, 2);
            this.btnShowLineError.Name = "btnShowLineError";
            this.btnShowLineError.Size = new System.Drawing.Size(108, 22);
            this.btnShowLineError.StyleController = this.layoutControl1;
            this.btnShowLineError.TabIndex = 6;
            this.btnShowLineError.Text = "Dòng lỗi";
            this.btnShowLineError.Click += new System.EventHandler(this.btnShowLineError_Click);
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Location = new System.Drawing.Point(121, 2);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(117, 22);
            this.btnChooseFile.StyleController = this.layoutControl1;
            this.btnChooseFile.TabIndex = 5;
            this.btnChooseFile.Text = "Import";
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // btnDownLoadFile
            // 
            this.btnDownLoadFile.Location = new System.Drawing.Point(2, 2);
            this.btnDownLoadFile.Name = "btnDownLoadFile";
            this.btnDownLoadFile.Size = new System.Drawing.Size(115, 22);
            this.btnDownLoadFile.StyleController = this.layoutControl1;
            this.btnDownLoadFile.TabIndex = 4;
            this.btnDownLoadFile.Text = "Tải file mẫu";
            this.btnDownLoadFile.Click += new System.EventHandler(this.btnDownLoadFile_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.a,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(884, 482);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnDownLoadFile;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(119, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // a
            // 
            this.a.Control = this.btnChooseFile;
            this.a.Location = new System.Drawing.Point(119, 0);
            this.a.Name = "a";
            this.a.Size = new System.Drawing.Size(121, 26);
            this.a.Text = "Import";
            this.a.TextSize = new System.Drawing.Size(0, 0);
            this.a.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnShowLineError;
            this.layoutControlItem3.Location = new System.Drawing.Point(240, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(112, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnImport;
            this.layoutControlItem4.Location = new System.Drawing.Point(352, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(109, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(461, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(423, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gridControlImport;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(884, 456);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // frmSarImportReportTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 511);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmSarImportReportTemplate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSarImportReportTemplate";
            this.Load += new System.EventHandler(this.frmSarImportReportTemplate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnDownLoadFile;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gridControlImport;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewImport;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraEditors.SimpleButton btnShowLineError;
        private DevExpress.XtraEditors.SimpleButton btnChooseFile;
        private DevExpress.XtraLayout.LayoutControlItem a;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSTT;
        private DevExpress.XtraGrid.Columns.GridColumn grdColErr;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDel;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReportCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReportName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColReportTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColExtension;
        private DevExpress.XtraGrid.Columns.GridColumn grdColTutorial;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnError;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
