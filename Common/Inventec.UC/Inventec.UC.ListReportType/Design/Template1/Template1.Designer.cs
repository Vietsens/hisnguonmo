﻿namespace Inventec.UC.ListReportType.Design.Template1
{
    partial class Template1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Template1));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.gridControlReportType = new DevExpress.XtraGrid.GridControl();
            this.gridViewReportType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnCreateReport = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlReportType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewReportType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCreateReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1366, 550);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.txtKeyWord);
            this.layoutControl3.Controls.Add(this.ucPaging1);
            this.layoutControl3.Controls.Add(this.gridControlReportType);
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(1366, 550);
            this.layoutControl3.TabIndex = 5;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(2, 2);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyWord.Size = new System.Drawing.Size(1362, 20);
            this.txtKeyWord.StyleController = this.layoutControl3;
            this.txtKeyWord.TabIndex = 15;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 524);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(1362, 24);
            this.ucPaging1.TabIndex = 14;
            // 
            // gridControlReportType
            // 
            this.gridControlReportType.Location = new System.Drawing.Point(2, 26);
            this.gridControlReportType.MainView = this.gridViewReportType;
            this.gridControlReportType.Name = "gridControlReportType";
            this.gridControlReportType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnCreateReport});
            this.gridControlReportType.Size = new System.Drawing.Size(1362, 494);
            this.gridControlReportType.TabIndex = 4;
            this.gridControlReportType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewReportType});
            // 
            // gridViewReportType
            // 
            this.gridViewReportType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.gridViewReportType.GridControl = this.gridControlReportType;
            this.gridViewReportType.Name = "gridViewReportType";
            this.gridViewReportType.OptionsFind.AllowFindPanel = false;
            this.gridViewReportType.OptionsFind.FindNullPrompt = "Từ khóa tìm kiếm";
            this.gridViewReportType.OptionsFind.ShowClearButton = false;
            this.gridViewReportType.OptionsFind.ShowCloseButton = false;
            this.gridViewReportType.OptionsFind.ShowFindButton = false;
            this.gridViewReportType.OptionsView.ColumnAutoWidth = false;
            this.gridViewReportType.OptionsView.ShowGroupPanel = false;
            this.gridViewReportType.OptionsView.ShowIndicator = false;
            this.gridViewReportType.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewReportType_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 36;
            // 
            // gridColumn2
            // 
            this.gridColumn2.ColumnEdit = this.btnCreateReport;
            this.gridColumn2.FieldName = "BtnCreateReport";
            this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ShowCaption = false;
            this.gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 25;
            // 
            // btnCreateReport
            // 
            this.btnCreateReport.AutoHeight = false;
            this.btnCreateReport.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnCreateReport.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Tạo báo cáo", null, null, true)});
            this.btnCreateReport.Name = "btnCreateReport";
            this.btnCreateReport.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnCreateReport.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnCreateReport_ButtonClick);
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "Mã loại báo cáo";
            this.gridColumn3.FieldName = "REPORT_TYPE_CODE";
            this.gridColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 100;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Tên loại báo cáo";
            this.gridColumn4.FieldName = "REPORT_TYPE_NAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsColumn.FixedWidth = true;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 500;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Người tạo";
            this.gridColumn5.FieldName = "CREATOR";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Width = 100;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "Thời gian tạo";
            this.gridColumn6.FieldName = "CREATE_TIME_STR";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn6.Width = 120;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Người sửa";
            this.gridColumn7.FieldName = "MODIFIER";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.Width = 100;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "Thời gian sửa";
            this.gridColumn8.FieldName = "MODIFY_TIME_STR";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn8.Width = 120;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7,
            this.layoutControlItem17,
            this.layoutControlItem1});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1366, 550);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.gridControlReportType;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(1366, 498);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.ucPaging1;
            this.layoutControlItem17.Location = new System.Drawing.Point(0, 522);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(1366, 28);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtKeyWord;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1366, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1366, 550);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl3;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(1366, 550);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(1366, 550);
            this.Load += new System.EventHandler(this.Template1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlReportType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewReportType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCreateReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.GridControl gridControlReportType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewReportType;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnCreateReport;
        private Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
