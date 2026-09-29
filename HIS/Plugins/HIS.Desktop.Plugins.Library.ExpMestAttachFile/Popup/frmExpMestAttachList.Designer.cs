namespace HIS.Desktop.Plugins.Library.ExpMestAttachFile.Popup
{
    partial class frmExpMestAttachList
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
            this.lblReadOnly = new DevExpress.XtraEditors.LabelControl();
            this.lblCount = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnAttachNew = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlDocument = new DevExpress.XtraGrid.GridControl();
            this.gridViewDocument = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcView = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoBtnDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcDocumentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciReadOnly = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCount = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciAttachNew = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRefresh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReadOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAttachNew)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciClose)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblReadOnly);
            this.layoutControl1.Controls.Add(this.lblCount);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnAttachNew);
            this.layoutControl1.Controls.Add(this.gridControlDocument);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(800, 450);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // lblReadOnly
            //
            this.lblReadOnly.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblReadOnly.Location = new System.Drawing.Point(2, 2);
            this.lblReadOnly.Name = "lblReadOnly";
            this.lblReadOnly.Size = new System.Drawing.Size(796, 13);
            this.lblReadOnly.StyleController = this.layoutControl1;
            this.lblReadOnly.TabIndex = 5;
            this.lblReadOnly.Text = "Phiếu đã hoàn thành/đã thanh toán: chỉ được xem và bổ sung đơn, không được xóa.";
            //
            // lblCount
            //
            this.lblCount.Location = new System.Drawing.Point(2, 430);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(296, 13);
            this.lblCount.StyleController = this.layoutControl1;
            this.lblCount.TabIndex = 4;
            this.lblCount.Text = "0 tài liệu";
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(712, 426);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(602, 426);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(106, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Làm mới (F5)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnAttachNew
            //
            this.btnAttachNew.Location = new System.Drawing.Point(472, 426);
            this.btnAttachNew.Name = "btnAttachNew";
            this.btnAttachNew.Size = new System.Drawing.Size(126, 22);
            this.btnAttachNew.StyleController = this.layoutControl1;
            this.btnAttachNew.TabIndex = 1;
            this.btnAttachNew.Text = "Đính kèm mới (Ctrl N)";
            this.btnAttachNew.Click += new System.EventHandler(this.btnAttachNew_Click);
            //
            // gridControlDocument
            //
            this.gridControlDocument.Location = new System.Drawing.Point(2, 22);
            this.gridControlDocument.MainView = this.gridViewDocument;
            this.gridControlDocument.Name = "gridControlDocument";
            this.gridControlDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnView,
            this.repoBtnDelete,
            this.repoBtnDeleteDisable});
            this.gridControlDocument.Size = new System.Drawing.Size(796, 400);
            this.gridControlDocument.TabIndex = 0;
            this.gridControlDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDocument});
            //
            // gridViewDocument
            //
            this.gridViewDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcView,
            this.gcDelete,
            this.gcDocumentName,
            this.gcCreateTime,
            this.gcCreator,
            this.gcModifyTime,
            this.gcModifier});
            this.gridViewDocument.GridControl = this.gridControlDocument;
            this.gridViewDocument.Name = "gridViewDocument";
            this.gridViewDocument.OptionsFind.AllowFindPanel = false;
            this.gridViewDocument.OptionsView.ColumnAutoWidth = true;
            this.gridViewDocument.OptionsView.ShowGroupPanel = false;
            this.gridViewDocument.OptionsView.ShowIndicator = false;
            this.gridViewDocument.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewDocument_CustomRowCellEdit);
            this.gridViewDocument.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDocument_CustomUnboundColumnData);
            this.gridViewDocument.DoubleClick += new System.EventHandler(this.gridViewDocument_DoubleClick);
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
            // gcView
            //
            this.gcView.ColumnEdit = this.repoBtnView;
            this.gcView.FieldName = "VIEW";
            this.gcView.Name = "gcView";
            this.gcView.OptionsColumn.ShowCaption = false;
            this.gcView.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcView.Visible = true;
            this.gcView.VisibleIndex = 1;
            this.gcView.Width = 30;
            //
            // repoBtnView
            //
            this.repoBtnView.AutoHeight = false;
            this.repoBtnView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoBtnView.Name = "repoBtnView";
            this.repoBtnView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnView.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnView_ButtonClick);
            //
            // gcDelete
            //
            this.gcDelete.ColumnEdit = this.repoBtnDelete;
            this.gcDelete.FieldName = "DELETE";
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.OptionsColumn.ShowCaption = false;
            this.gcDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 2;
            this.gcDelete.Width = 30;
            //
            // repoBtnDelete
            //
            this.repoBtnDelete.AutoHeight = false;
            this.repoBtnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoBtnDelete.Name = "repoBtnDelete";
            this.repoBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDelete_ButtonClick);
            //
            // repoBtnDeleteDisable
            //
            this.repoBtnDeleteDisable.AutoHeight = false;
            this.repoBtnDeleteDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repoBtnDeleteDisable.Name = "repoBtnDeleteDisable";
            this.repoBtnDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnDeleteDisable.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDeleteDisable_ButtonClick);
            //
            // gcDocumentName
            //
            this.gcDocumentName.Caption = "Tên văn bản";
            this.gcDocumentName.FieldName = "DOCUMENT_NAME";
            this.gcDocumentName.Name = "gcDocumentName";
            this.gcDocumentName.OptionsColumn.AllowEdit = false;
            this.gcDocumentName.Visible = true;
            this.gcDocumentName.VisibleIndex = 3;
            this.gcDocumentName.Width = 220;
            //
            // gcCreateTime
            //
            this.gcCreateTime.Caption = "Thời gian đính kèm";
            this.gcCreateTime.FieldName = "CREATE_TIME_STR";
            this.gcCreateTime.Name = "gcCreateTime";
            this.gcCreateTime.OptionsColumn.AllowEdit = false;
            this.gcCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcCreateTime.Visible = true;
            this.gcCreateTime.VisibleIndex = 4;
            this.gcCreateTime.Width = 120;
            //
            // gcCreator
            //
            this.gcCreator.Caption = "Người đính kèm";
            this.gcCreator.FieldName = "CREATOR";
            this.gcCreator.Name = "gcCreator";
            this.gcCreator.OptionsColumn.AllowEdit = false;
            this.gcCreator.Visible = true;
            this.gcCreator.VisibleIndex = 5;
            this.gcCreator.Width = 100;
            //
            // gcModifyTime
            //
            this.gcModifyTime.Caption = "Thời gian sửa";
            this.gcModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gcModifyTime.Name = "gcModifyTime";
            this.gcModifyTime.OptionsColumn.AllowEdit = false;
            this.gcModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcModifyTime.Visible = true;
            this.gcModifyTime.VisibleIndex = 6;
            this.gcModifyTime.Width = 120;
            //
            // gcModifier
            //
            this.gcModifier.Caption = "Người sửa";
            this.gcModifier.FieldName = "MODIFIER";
            this.gcModifier.Name = "gcModifier";
            this.gcModifier.OptionsColumn.AllowEdit = false;
            this.gcModifier.Visible = true;
            this.gcModifier.VisibleIndex = 7;
            this.gcModifier.Width = 100;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciReadOnly,
            this.lciGrid,
            this.lciCount,
            this.emptySpaceItem1,
            this.lciAttachNew,
            this.lciRefresh,
            this.lciClose});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(800, 450);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciReadOnly
            //
            this.lciReadOnly.Control = this.lblReadOnly;
            this.lciReadOnly.Location = new System.Drawing.Point(0, 0);
            this.lciReadOnly.Name = "lciReadOnly";
            this.lciReadOnly.Size = new System.Drawing.Size(800, 20);
            this.lciReadOnly.TextSize = new System.Drawing.Size(0, 0);
            this.lciReadOnly.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlDocument;
            this.lciGrid.Location = new System.Drawing.Point(0, 20);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(800, 404);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciCount
            //
            this.lciCount.Control = this.lblCount;
            this.lciCount.Location = new System.Drawing.Point(0, 424);
            this.lciCount.Name = "lciCount";
            this.lciCount.Size = new System.Drawing.Size(300, 26);
            this.lciCount.TextSize = new System.Drawing.Size(0, 0);
            this.lciCount.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(300, 424);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(170, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciAttachNew
            //
            this.lciAttachNew.Control = this.btnAttachNew;
            this.lciAttachNew.Location = new System.Drawing.Point(470, 424);
            this.lciAttachNew.Name = "lciAttachNew";
            this.lciAttachNew.Size = new System.Drawing.Size(130, 26);
            this.lciAttachNew.TextSize = new System.Drawing.Size(0, 0);
            this.lciAttachNew.TextVisible = false;
            //
            // lciRefresh
            //
            this.lciRefresh.Control = this.btnRefresh;
            this.lciRefresh.Location = new System.Drawing.Point(600, 424);
            this.lciRefresh.Name = "lciRefresh";
            this.lciRefresh.Size = new System.Drawing.Size(110, 26);
            this.lciRefresh.TextSize = new System.Drawing.Size(0, 0);
            this.lciRefresh.TextVisible = false;
            //
            // lciClose
            //
            this.lciClose.Control = this.btnClose;
            this.lciClose.Location = new System.Drawing.Point(710, 424);
            this.lciClose.Name = "lciClose";
            this.lciClose.Size = new System.Drawing.Size(90, 26);
            this.lciClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciClose.TextVisible = false;
            //
            // frmExpMestAttachList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "frmExpMestAttachList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đơn thuốc đính kèm";
            this.Load += new System.EventHandler(this.frmExpMestAttachList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDeleteDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReadOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAttachNew)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlDocument;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDocument;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcView;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcDocumentName;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnView;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDeleteDisable;
        private DevExpress.XtraEditors.SimpleButton btnAttachNew;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.LabelControl lblCount;
        private DevExpress.XtraEditors.LabelControl lblReadOnly;
        private DevExpress.XtraLayout.LayoutControlItem lciReadOnly;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciCount;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciAttachNew;
        private DevExpress.XtraLayout.LayoutControlItem lciRefresh;
        private DevExpress.XtraLayout.LayoutControlItem lciClose;
    }
}
