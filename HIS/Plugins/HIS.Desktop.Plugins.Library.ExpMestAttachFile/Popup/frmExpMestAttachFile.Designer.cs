namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Popup
{
    partial class frmExpMestAttachFile
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblHint = new DevExpress.XtraEditors.LabelControl();
            this.panelPreview = new DevExpress.XtraEditors.PanelControl();
            this.pdfViewerPreview = new DevExpress.XtraPdfViewer.PdfViewer();
            this.picPreview = new DevExpress.XtraEditors.PictureEdit();
            this.gridControlFiles = new DevExpress.XtraGrid.GridControl();
            this.gridViewFiles = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcFileName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnRotateRight = new DevExpress.XtraEditors.SimpleButton();
            this.btnRotateLeft = new DevExpress.XtraEditors.SimpleButton();
            this.btnCapture = new DevExpress.XtraEditors.SimpleButton();
            this.btnChooseFile = new DevExpress.XtraEditors.SimpleButton();
            this.txtDocumentName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDocumentName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChooseFile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCapture = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRotateLeft = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRotateRight = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciGridFiles = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPreview = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHint = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciOk = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelPreview)).BeginInit();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocumentName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDocumentName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChooseFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCapture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRotateLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRotateRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciClose)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblHint);
            this.layoutControl1.Controls.Add(this.panelPreview);
            this.layoutControl1.Controls.Add(this.gridControlFiles);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.btnOk);
            this.layoutControl1.Controls.Add(this.btnRotateRight);
            this.layoutControl1.Controls.Add(this.btnRotateLeft);
            this.layoutControl1.Controls.Add(this.btnCapture);
            this.layoutControl1.Controls.Add(this.btnChooseFile);
            this.layoutControl1.Controls.Add(this.txtDocumentName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(900, 580);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // lblHint
            //
            this.lblHint.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblHint.Location = new System.Drawing.Point(2, 556);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(676, 13);
            this.lblHint.StyleController = this.layoutControl1;
            this.lblHint.TabIndex = 10;
            this.lblHint.Text = "Định dạng cho phép: jpg, jpeg, png, bmp, gif, pdf.";
            //
            // panelPreview
            //
            this.panelPreview.Controls.Add(this.pdfViewerPreview);
            this.panelPreview.Controls.Add(this.picPreview);
            this.panelPreview.Location = new System.Drawing.Point(312, 52);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(586, 498);
            this.panelPreview.TabIndex = 9;
            //
            // pdfViewerPreview
            //
            this.pdfViewerPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewerPreview.Location = new System.Drawing.Point(2, 2);
            this.pdfViewerPreview.Name = "pdfViewerPreview";
            this.pdfViewerPreview.Size = new System.Drawing.Size(582, 494);
            this.pdfViewerPreview.TabIndex = 1;
            this.pdfViewerPreview.Visible = false;
            //
            // picPreview
            //
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(2, 2);
            this.picPreview.Name = "picPreview";
            this.picPreview.Properties.ReadOnly = true;
            this.picPreview.Properties.ShowMenu = false;
            this.picPreview.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.picPreview.Size = new System.Drawing.Size(582, 494);
            this.picPreview.TabIndex = 0;
            //
            // gridControlFiles
            //
            this.gridControlFiles.Location = new System.Drawing.Point(2, 52);
            this.gridControlFiles.MainView = this.gridViewFiles;
            this.gridControlFiles.Name = "gridControlFiles";
            this.gridControlFiles.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnDelete});
            this.gridControlFiles.Size = new System.Drawing.Size(306, 498);
            this.gridControlFiles.TabIndex = 8;
            this.gridControlFiles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewFiles});
            //
            // gridViewFiles
            //
            this.gridViewFiles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcDelete,
            this.gcFileName});
            this.gridViewFiles.GridControl = this.gridControlFiles;
            this.gridViewFiles.Name = "gridViewFiles";
            this.gridViewFiles.OptionsCustomization.AllowSort = false;
            this.gridViewFiles.OptionsFind.AllowFindPanel = false;
            this.gridViewFiles.OptionsView.ColumnAutoWidth = true;
            this.gridViewFiles.OptionsView.ShowGroupPanel = false;
            this.gridViewFiles.OptionsView.ShowIndicator = false;
            this.gridViewFiles.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewFiles_FocusedRowChanged);
            this.gridViewFiles.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewFiles_CustomUnboundColumnData);
            //
            // gcStt
            //
            this.gcStt.AppearanceCell.Options.UseTextOptions = true;
            this.gcStt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 40;
            //
            // gcDelete
            //
            this.gcDelete.ColumnEdit = this.repoBtnDelete;
            this.gcDelete.FieldName = "DELETE";
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.OptionsColumn.ShowCaption = false;
            this.gcDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 1;
            this.gcDelete.Width = 30;
            //
            // gcFileName
            //
            this.gcFileName.Caption = "Tên tệp";
            this.gcFileName.FieldName = "FileName";
            this.gcFileName.Name = "gcFileName";
            this.gcFileName.OptionsColumn.AllowEdit = false;
            this.gcFileName.Visible = true;
            this.gcFileName.VisibleIndex = 2;
            this.gcFileName.Width = 230;
            //
            // repoBtnDelete
            //
            this.repoBtnDelete.AutoHeight = false;
            this.repoBtnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repoBtnDelete.Name = "repoBtnDelete";
            this.repoBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDelete_ButtonClick);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(802, 554);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(96, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(682, 554);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(116, 22);
            this.btnOk.StyleController = this.layoutControl1;
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Đồng ý (Ctrl S)";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnRotateRight
            //
            this.btnRotateRight.Location = new System.Drawing.Point(282, 26);
            this.btnRotateRight.Name = "btnRotateRight";
            this.btnRotateRight.Size = new System.Drawing.Size(76, 22);
            this.btnRotateRight.StyleController = this.layoutControl1;
            this.btnRotateRight.TabIndex = 5;
            this.btnRotateRight.Text = "Xoay phải ↷";
            this.btnRotateRight.Click += new System.EventHandler(this.btnRotateRight_Click);
            //
            // btnRotateLeft
            //
            this.btnRotateLeft.Location = new System.Drawing.Point(202, 26);
            this.btnRotateLeft.Name = "btnRotateLeft";
            this.btnRotateLeft.Size = new System.Drawing.Size(76, 22);
            this.btnRotateLeft.StyleController = this.layoutControl1;
            this.btnRotateLeft.TabIndex = 4;
            this.btnRotateLeft.Text = "↶ Xoay trái";
            this.btnRotateLeft.Click += new System.EventHandler(this.btnRotateLeft_Click);
            //
            // btnCapture
            //
            this.btnCapture.Location = new System.Drawing.Point(102, 26);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(96, 22);
            this.btnCapture.StyleController = this.layoutControl1;
            this.btnCapture.TabIndex = 3;
            this.btnCapture.Text = "Chụp ảnh";
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            //
            // btnChooseFile
            //
            this.btnChooseFile.Location = new System.Drawing.Point(2, 26);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(96, 22);
            this.btnChooseFile.StyleController = this.layoutControl1;
            this.btnChooseFile.TabIndex = 2;
            this.btnChooseFile.Text = "Chọn tệp";
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            //
            // txtDocumentName
            //
            this.txtDocumentName.Location = new System.Drawing.Point(97, 2);
            this.txtDocumentName.Name = "txtDocumentName";
            this.txtDocumentName.Properties.MaxLength = 500;
            this.txtDocumentName.Size = new System.Drawing.Size(801, 20);
            this.txtDocumentName.StyleController = this.layoutControl1;
            this.txtDocumentName.TabIndex = 1;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDocumentName,
            this.lciChooseFile,
            this.lciCapture,
            this.lciRotateLeft,
            this.lciRotateRight,
            this.emptySpaceItem1,
            this.lciGridFiles,
            this.lciPreview,
            this.lciHint,
            this.emptySpaceItem2,
            this.lciOk,
            this.lciClose});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(900, 580);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciDocumentName
            //
            this.lciDocumentName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDocumentName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDocumentName.Control = this.txtDocumentName;
            this.lciDocumentName.Location = new System.Drawing.Point(0, 0);
            this.lciDocumentName.Name = "lciDocumentName";
            this.lciDocumentName.Size = new System.Drawing.Size(900, 24);
            this.lciDocumentName.Text = "Tên văn bản:";
            this.lciDocumentName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDocumentName.TextSize = new System.Drawing.Size(90, 20);
            this.lciDocumentName.TextToControlDistance = 5;
            //
            // lciChooseFile
            //
            this.lciChooseFile.Control = this.btnChooseFile;
            this.lciChooseFile.Location = new System.Drawing.Point(0, 24);
            this.lciChooseFile.Name = "lciChooseFile";
            this.lciChooseFile.Size = new System.Drawing.Size(100, 26);
            this.lciChooseFile.TextSize = new System.Drawing.Size(0, 0);
            this.lciChooseFile.TextVisible = false;
            //
            // lciCapture
            //
            this.lciCapture.Control = this.btnCapture;
            this.lciCapture.Location = new System.Drawing.Point(100, 24);
            this.lciCapture.Name = "lciCapture";
            this.lciCapture.Size = new System.Drawing.Size(100, 26);
            this.lciCapture.TextSize = new System.Drawing.Size(0, 0);
            this.lciCapture.TextVisible = false;
            //
            // lciRotateLeft
            //
            this.lciRotateLeft.Control = this.btnRotateLeft;
            this.lciRotateLeft.Location = new System.Drawing.Point(200, 24);
            this.lciRotateLeft.Name = "lciRotateLeft";
            this.lciRotateLeft.Size = new System.Drawing.Size(80, 26);
            this.lciRotateLeft.TextSize = new System.Drawing.Size(0, 0);
            this.lciRotateLeft.TextVisible = false;
            //
            // lciRotateRight
            //
            this.lciRotateRight.Control = this.btnRotateRight;
            this.lciRotateRight.Location = new System.Drawing.Point(280, 24);
            this.lciRotateRight.Name = "lciRotateRight";
            this.lciRotateRight.Size = new System.Drawing.Size(80, 26);
            this.lciRotateRight.TextSize = new System.Drawing.Size(0, 0);
            this.lciRotateRight.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(360, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(540, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciGridFiles
            //
            this.lciGridFiles.Control = this.gridControlFiles;
            this.lciGridFiles.Location = new System.Drawing.Point(0, 50);
            this.lciGridFiles.Name = "lciGridFiles";
            this.lciGridFiles.Size = new System.Drawing.Size(310, 502);
            this.lciGridFiles.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridFiles.TextVisible = false;
            //
            // lciPreview
            //
            this.lciPreview.Control = this.panelPreview;
            this.lciPreview.Location = new System.Drawing.Point(310, 50);
            this.lciPreview.Name = "lciPreview";
            this.lciPreview.Size = new System.Drawing.Size(590, 502);
            this.lciPreview.TextSize = new System.Drawing.Size(0, 0);
            this.lciPreview.TextVisible = false;
            //
            // lciHint
            //
            this.lciHint.Control = this.lblHint;
            this.lciHint.Location = new System.Drawing.Point(0, 552);
            this.lciHint.Name = "lciHint";
            this.lciHint.Size = new System.Drawing.Size(560, 28);
            this.lciHint.TextSize = new System.Drawing.Size(0, 0);
            this.lciHint.TextVisible = false;
            //
            // emptySpaceItem2
            //
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(560, 552);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(120, 28);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciOk
            //
            this.lciOk.Control = this.btnOk;
            this.lciOk.Location = new System.Drawing.Point(680, 552);
            this.lciOk.Name = "lciOk";
            this.lciOk.Size = new System.Drawing.Size(120, 28);
            this.lciOk.TextSize = new System.Drawing.Size(0, 0);
            this.lciOk.TextVisible = false;
            //
            // lciClose
            //
            this.lciClose.Control = this.btnClose;
            this.lciClose.Location = new System.Drawing.Point(800, 552);
            this.lciClose.Name = "lciClose";
            this.lciClose.Size = new System.Drawing.Size(100, 28);
            this.lciClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciClose.TextVisible = false;
            //
            // frmExpMestAttachFile
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 580);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmExpMestAttachFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đính kèm đơn thuốc";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmExpMestAttachFile_FormClosed);
            this.Load += new System.EventHandler(this.frmExpMestAttachFile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelPreview)).EndInit();
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPreview.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocumentName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDocumentName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChooseFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCapture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRotateLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRotateRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtDocumentName;
        private DevExpress.XtraEditors.SimpleButton btnChooseFile;
        private DevExpress.XtraEditors.SimpleButton btnCapture;
        private DevExpress.XtraEditors.SimpleButton btnRotateLeft;
        private DevExpress.XtraEditors.SimpleButton btnRotateRight;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraGrid.GridControl gridControlFiles;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewFiles;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcFileName;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDelete;
        private DevExpress.XtraEditors.PanelControl panelPreview;
        private DevExpress.XtraEditors.PictureEdit picPreview;
        private DevExpress.XtraPdfViewer.PdfViewer pdfViewerPreview;
        private DevExpress.XtraEditors.LabelControl lblHint;
        private DevExpress.XtraLayout.LayoutControlItem lciDocumentName;
        private DevExpress.XtraLayout.LayoutControlItem lciChooseFile;
        private DevExpress.XtraLayout.LayoutControlItem lciCapture;
        private DevExpress.XtraLayout.LayoutControlItem lciRotateLeft;
        private DevExpress.XtraLayout.LayoutControlItem lciRotateRight;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciGridFiles;
        private DevExpress.XtraLayout.LayoutControlItem lciPreview;
        private DevExpress.XtraLayout.LayoutControlItem lciHint;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem lciOk;
        private DevExpress.XtraLayout.LayoutControlItem lciClose;
    }
}
