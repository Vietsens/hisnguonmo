﻿namespace Inventec.Common.XtraReportExport.TemplateKey
{
    partial class PreviewTemplateKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreviewTemplateKey));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.gridControlKey = new DevExpress.XtraGrid.GridControl();
            this.gridViewKey = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.GcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcKey = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcValue = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Controls.Add(this.gridControlKey);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(864, 462);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(2, 2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullText = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(860, 20);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // gridControlKey
            // 
            this.gridControlKey.Location = new System.Drawing.Point(2, 26);
            this.gridControlKey.MainView = this.gridViewKey;
            this.gridControlKey.Name = "gridControlKey";
            this.gridControlKey.Size = new System.Drawing.Size(860, 434);
            this.gridControlKey.TabIndex = 0;
            this.gridControlKey.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewKey});
            // 
            // gridViewKey
            // 
            this.gridViewKey.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.GcStt,
            this.GcKey,
            this.GcValue});
            this.gridViewKey.GridControl = this.gridControlKey;
            this.gridViewKey.Name = "gridViewKey";
            this.gridViewKey.OptionsView.ShowGroupPanel = false;
            this.gridViewKey.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewKey_CustomUnboundColumnData);
            // 
            // GcStt
            // 
            this.GcStt.Caption = "STT";
            this.GcStt.FieldName = "STT";
            this.GcStt.Name = "GcStt";
            this.GcStt.OptionsColumn.AllowEdit = false;
            this.GcStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.GcStt.OptionsFilter.AllowFilter = false;
            this.GcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcStt.Visible = true;
            this.GcStt.VisibleIndex = 0;
            this.GcStt.Width = 54;
            // 
            // GcKey
            // 
            this.GcKey.Caption = "Key";
            this.GcKey.FieldName = "Name";
            this.GcKey.Name = "GcKey";
            this.GcKey.Visible = true;
            this.GcKey.VisibleIndex = 1;
            this.GcKey.Width = 381;
            // 
            // GcValue
            // 
            this.GcValue.Caption = "Giá trị";
            this.GcValue.FieldName = "Value";
            this.GcValue.Name = "GcValue";
            this.GcValue.OptionsColumn.AllowEdit = false;
            this.GcValue.Visible = true;
            this.GcValue.VisibleIndex = 2;
            this.GcValue.Width = 407;
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
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(864, 462);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlKey;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(864, 438);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtSearch;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(864, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // PreviewTemplateKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 462);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PreviewTemplateKey";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preview Template Key";
            this.Load += new System.EventHandler(this.PreviewTemplateKey_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlKey;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewKey;
        private DevExpress.XtraGrid.Columns.GridColumn GcStt;
        private DevExpress.XtraGrid.Columns.GridColumn GcKey;
        private DevExpress.XtraGrid.Columns.GridColumn GcValue;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}