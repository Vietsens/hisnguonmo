/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
namespace HIS.Desktop.Plugins.BidCreate.Forms
{
    partial class frmTransferMediOrgSelect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtTransferCodePreview = new DevExpress.XtraEditors.TextEdit();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlMediOrg = new DevExpress.XtraGrid.GridControl();
            this.gridViewMediOrg = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcMediOrgCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcMediOrgName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPreview = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSelect = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransferCodePreview.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSelect)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.txtTransferCodePreview);
            this.layoutControl1.Controls.Add(this.btnSelect);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.gridControlMediOrg);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(600, 500);
            this.layoutControl1.TabIndex = 0;
            //
            // txtTransferCodePreview
            //
            this.txtTransferCodePreview.Location = new System.Drawing.Point(12, 466);
            this.txtTransferCodePreview.Name = "txtTransferCodePreview";
            this.txtTransferCodePreview.Properties.MaxLength = 10;
            this.txtTransferCodePreview.Properties.NullValuePrompt = "Giá trị ghép (Default \"C.\", có thể chỉnh sửa)";
            this.txtTransferCodePreview.Size = new System.Drawing.Size(434, 22);
            this.txtTransferCodePreview.StyleController = this.layoutControl1;
            this.txtTransferCodePreview.TabIndex = 2;
            //
            // btnSelect
            //
            this.btnSelect.Location = new System.Drawing.Point(450, 466);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(138, 22);
            this.btnSelect.StyleController = this.layoutControl1;
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "Chọn (Ctrl S)";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            //
            // txtKeyword
            //
            this.txtKeyword.Location = new System.Drawing.Point(12, 12);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Size = new System.Drawing.Size(576, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.EditValueChanged += new System.EventHandler(this.txtKeyword_EditValueChanged);
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
            //
            // gridControlMediOrg
            //
            this.gridControlMediOrg.Location = new System.Drawing.Point(12, 36);
            this.gridControlMediOrg.MainView = this.gridViewMediOrg;
            this.gridControlMediOrg.Name = "gridControlMediOrg";
            this.gridControlMediOrg.Size = new System.Drawing.Size(576, 426);
            this.gridControlMediOrg.TabIndex = 1;
            this.gridControlMediOrg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMediOrg});
            //
            // gridViewMediOrg
            //
            this.gridViewMediOrg.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcMediOrgCode,
            this.gcMediOrgName});
            this.gridViewMediOrg.GridControl = this.gridControlMediOrg;
            this.gridViewMediOrg.Name = "gridViewMediOrg";
            this.gridViewMediOrg.OptionsBehavior.Editable = false;
            this.gridViewMediOrg.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewMediOrg.OptionsView.ShowGroupPanel = false;
            this.gridViewMediOrg.OptionsView.ShowIndicator = false;
            this.gridViewMediOrg.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewMediOrg_FocusedRowChanged);
            this.gridViewMediOrg.DoubleClick += new System.EventHandler(this.gridViewMediOrg_DoubleClick);
            this.gridViewMediOrg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewMediOrg_KeyDown);
            //
            // gcMediOrgCode
            //
            this.gcMediOrgCode.Caption = "Mã CSKCB";
            this.gcMediOrgCode.FieldName = "MEDI_ORG_CODE";
            this.gcMediOrgCode.Name = "gcMediOrgCode";
            this.gcMediOrgCode.OptionsColumn.AllowEdit = false;
            this.gcMediOrgCode.Visible = true;
            this.gcMediOrgCode.VisibleIndex = 0;
            this.gcMediOrgCode.Width = 120;
            //
            // gcMediOrgName
            //
            this.gcMediOrgName.Caption = "Tên CSKCB";
            this.gcMediOrgName.FieldName = "MEDI_ORG_NAME";
            this.gcMediOrgName.Name = "gcMediOrgName";
            this.gcMediOrgName.OptionsColumn.AllowEdit = false;
            this.gcMediOrgName.Visible = true;
            this.gcMediOrgName.VisibleIndex = 1;
            this.gcMediOrgName.Width = 440;
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.lciGrid,
            this.lciPreview,
            this.lciBtnSelect});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(600, 500);
            this.Root.TextVisible = false;
            //
            // lciKeyword
            //
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(580, 24);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlMediOrg;
            this.lciGrid.Location = new System.Drawing.Point(0, 24);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(580, 430);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciPreview
            //
            this.lciPreview.Control = this.txtTransferCodePreview;
            this.lciPreview.Location = new System.Drawing.Point(0, 454);
            this.lciPreview.Name = "lciPreview";
            this.lciPreview.Size = new System.Drawing.Size(438, 26);
            this.lciPreview.TextSize = new System.Drawing.Size(0, 0);
            this.lciPreview.TextVisible = false;
            //
            // lciBtnSelect
            //
            this.lciBtnSelect.Control = this.btnSelect;
            this.lciBtnSelect.Location = new System.Drawing.Point(438, 454);
            this.lciBtnSelect.Name = "lciBtnSelect";
            this.lciBtnSelect.Size = new System.Drawing.Size(142, 26);
            this.lciBtnSelect.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSelect.TextVisible = false;
            //
            // frmTransferMediOrgSelect
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTransferMediOrgSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm chọn CSKCB";
            this.Load += new System.EventHandler(this.frmTransferMediOrgSelect_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTransferMediOrgSelect_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTransferCodePreview.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMediOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMediOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSelect)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraGrid.GridControl gridControlMediOrg;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMediOrg;
        private DevExpress.XtraGrid.Columns.GridColumn gcMediOrgCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcMediOrgName;
        private DevExpress.XtraEditors.TextEdit txtTransferCodePreview;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciPreview;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSelect;
    }
}
