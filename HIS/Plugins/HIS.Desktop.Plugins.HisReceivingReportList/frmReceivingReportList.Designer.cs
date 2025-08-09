
namespace HIS.Desktop.Plugins.HisReceivingReportList
{
    partial class frmReceivingReportList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReceivingReportList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject17 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject18 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject19 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject20 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroup1 = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.dtCreateTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtCreateTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.grvReportList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnViewDetail = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colReportID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBeginDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEndDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReceivingReport = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreationTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPageNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotalPage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colActive = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReportList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.navBarControl1);
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Location = new System.Drawing.Point(1, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1132, 598);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroup1;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroup1});
            this.navBarControl1.Location = new System.Drawing.Point(2, 26);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 284;
            this.navBarControl1.Size = new System.Drawing.Size(284, 570);
            this.navBarControl1.TabIndex = 8;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // navBarGroup1
            // 
            this.navBarGroup1.Caption = "Thời gian tạo";
            this.navBarGroup1.ControlContainer = this.navBarGroupControlContainer1;
            this.navBarGroup1.Expanded = true;
            this.navBarGroup1.GroupClientHeight = 135;
            this.navBarGroup1.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroup1.Name = "navBarGroup1";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.dtCreateTimeTo);
            this.navBarGroupControlContainer1.Controls.Add(this.dtCreateTimeFrom);
            this.navBarGroupControlContainer1.Controls.Add(this.labelControl2);
            this.navBarGroupControlContainer1.Controls.Add(this.labelControl1);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(276, 131);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // dtCreateTimeTo
            // 
            this.dtCreateTimeTo.EditValue = null;
            this.dtCreateTimeTo.Location = new System.Drawing.Point(127, 46);
            this.dtCreateTimeTo.Name = "dtCreateTimeTo";
            this.dtCreateTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Size = new System.Drawing.Size(145, 20);
            this.dtCreateTimeTo.TabIndex = 5;
            // 
            // dtCreateTimeFrom
            // 
            this.dtCreateTimeFrom.EditValue = null;
            this.dtCreateTimeFrom.Location = new System.Drawing.Point(127, 13);
            this.dtCreateTimeFrom.Name = "dtCreateTimeFrom";
            this.dtCreateTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Size = new System.Drawing.Size(145, 20);
            this.dtCreateTimeFrom.TabIndex = 4;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(97, 49);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(24, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Đến:";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(104, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(17, 13);
            this.labelControl1.TabIndex = 2;
            this.labelControl1.Text = "Từ:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullText = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(284, 20);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 5;
            this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(290, 2);
            this.gridControl1.MainView = this.grvReportList;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(0);
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnViewDetail});
            this.gridControl1.Size = new System.Drawing.Size(840, 594);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvReportList});
            // 
            // grvReportList
            // 
            this.grvReportList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSTT,
            this.gridColumn2,
            this.colReportID,
            this.colBeginDate,
            this.colEndDate,
            this.colReceivingReport,
            this.colCreationTime,
            this.colPageNumber,
            this.colTotalPage,
            this.colActive,
            this.colCreateTime,
            this.colModifyTime,
            this.colCreator,
            this.colModifier});
            this.grvReportList.GridControl = this.gridControl1;
            this.grvReportList.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grvReportList.Name = "grvReportList";
            this.grvReportList.OptionsPrint.AutoWidth = false;
            this.grvReportList.OptionsView.ColumnAutoWidth = false;
            this.grvReportList.OptionsView.ShowGroupPanel = false;
            this.grvReportList.OptionsView.ShowIndicator = false;
            this.grvReportList.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            this.grvReportList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.Name = "colSTT";
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 35;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.btnViewDetail;
            this.gridColumn2.FieldName = "btnViewDetail";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ShowCaption = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 30;
            // 
            // btnViewDetail
            // 
            this.btnViewDetail.AutoHeight = false;
            this.btnViewDetail.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnViewDetail.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject17, serializableAppearanceObject18, serializableAppearanceObject19, serializableAppearanceObject20, "", null, null, true)});
            this.btnViewDetail.Name = "btnViewDetail";
            this.btnViewDetail.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnViewDetail.Click += new System.EventHandler(this.btnViewDetail_Click);
            // 
            // colReportID
            // 
            this.colReportID.Caption = "Mã giao dịch";
            this.colReportID.FieldName = "REPORT_ID";
            this.colReportID.Name = "colReportID";
            this.colReportID.Visible = true;
            this.colReportID.VisibleIndex = 2;
            this.colReportID.Width = 70;
            // 
            // colBeginDate
            // 
            this.colBeginDate.Caption = "Thời gian bắt đầu đối soát";
            this.colBeginDate.FieldName = "BEGIN_DATE_STR";
            this.colBeginDate.Name = "colBeginDate";
            this.colBeginDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colBeginDate.Visible = true;
            this.colBeginDate.VisibleIndex = 3;
            this.colBeginDate.Width = 150;
            // 
            // colEndDate
            // 
            this.colEndDate.Caption = "Thời gian kết thúc đối soát";
            this.colEndDate.FieldName = "END_DATE_STR";
            this.colEndDate.Name = "colEndDate";
            this.colEndDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colEndDate.Visible = true;
            this.colEndDate.VisibleIndex = 4;
            this.colEndDate.Width = 150;
            // 
            // colReceivingReport
            // 
            this.colReceivingReport.Caption = "Mã định danh phần mềm";
            this.colReceivingReport.FieldName = "RECEIVING_REPORT";
            this.colReceivingReport.Name = "colReceivingReport";
            this.colReceivingReport.Visible = true;
            this.colReceivingReport.VisibleIndex = 5;
            this.colReceivingReport.Width = 130;
            // 
            // colCreationTime
            // 
            this.colCreationTime.Caption = "Thời gian tạo báo cáo";
            this.colCreationTime.FieldName = "CREATION_DATETIME_STR";
            this.colCreationTime.Name = "colCreationTime";
            this.colCreationTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colCreationTime.Visible = true;
            this.colCreationTime.VisibleIndex = 6;
            this.colCreationTime.Width = 120;
            // 
            // colPageNumber
            // 
            this.colPageNumber.Caption = "Số thứ tự trang báo cáo";
            this.colPageNumber.FieldName = "PAGE_NUMBER";
            this.colPageNumber.Name = "colPageNumber";
            this.colPageNumber.Visible = true;
            this.colPageNumber.VisibleIndex = 7;
            this.colPageNumber.Width = 130;
            // 
            // colTotalPage
            // 
            this.colTotalPage.Caption = "Tổng số trang";
            this.colTotalPage.FieldName = "TOTAL_PAGE";
            this.colTotalPage.Name = "colTotalPage";
            this.colTotalPage.Visible = true;
            this.colTotalPage.VisibleIndex = 8;
            // 
            // colActive
            // 
            this.colActive.Caption = "Trạng thái";
            this.colActive.FieldName = "IS_ACTIVE_STR";
            this.colActive.Name = "colActive";
            this.colActive.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colActive.Visible = true;
            this.colActive.VisibleIndex = 9;
            this.colActive.Width = 80;
            // 
            // colCreateTime
            // 
            this.colCreateTime.Caption = "Thời gian tạo";
            this.colCreateTime.FieldName = "CREATE_TIME_STR";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 10;
            this.colCreateTime.Width = 110;
            // 
            // colModifyTime
            // 
            this.colModifyTime.Caption = "Thời gian sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME_STR";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 11;
            this.colModifyTime.Width = 110;
            // 
            // colCreator
            // 
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 12;
            this.colCreator.Width = 70;
            // 
            // colModifier
            // 
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 13;
            this.colModifier.Width = 70;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1132, 598);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(288, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(844, 598);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtSearch;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(288, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.navBarControl1;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(288, 574);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(147, 604);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(143, 22);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(1, 604);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(140, 22);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(292, 604);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(839, 21);
            this.ucPaging1.TabIndex = 9;
            // 
            // frmReceivingReportList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 630);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.ucPaging1);
            this.Name = "frmReceivingReportList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách giao dịch";
            this.Load += new System.EventHandler(this.frmReceivingReportList_Load);
            this.Controls.SetChildIndex(this.ucPaging1, 0);
            this.Controls.SetChildIndex(this.btnRefresh, 0);
            this.Controls.SetChildIndex(this.btnSearch, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            this.navBarGroupControlContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReportList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnViewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView grvReportList;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeTo;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeFrom;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn colReportID;
        private DevExpress.XtraGrid.Columns.GridColumn colBeginDate;
        private DevExpress.XtraGrid.Columns.GridColumn colEndDate;
        private DevExpress.XtraGrid.Columns.GridColumn colReceivingReport;
        private DevExpress.XtraGrid.Columns.GridColumn colCreationTime;
        private DevExpress.XtraGrid.Columns.GridColumn colPageNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colTotalPage;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnViewDetail;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;
        private DevExpress.XtraGrid.Columns.GridColumn colActive;
    }
}