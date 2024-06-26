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
namespace HIS.Desktop.Plugins.SwapService
{
    partial class frmSwapService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSwapService));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlSwapService = new DevExpress.XtraGrid.GridControl();
            this.gridViewSwapService = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinEditAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemcboPatientType = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColExpend = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColIsOutParentFee = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEditService = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemChkOutKtcFee = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemChkIsExpend = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.btnSwapService = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSwapService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSwapService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemcboPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkOutKtcFee)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkIsExpend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.gridControlSwapService);
            this.layoutControl1.Controls.Add(this.btnSwapService);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(236, 178, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1130, 352);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Nhập từ khóa tìm kiếm...";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Size = new System.Drawing.Size(471, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 7;
            this.txtKeyword.EditValueChanged += new System.EventHandler(this.txtKeyword_EditValueChanged);
            // 
            // gridControlSwapService
            // 
            this.gridControlSwapService.Location = new System.Drawing.Point(2, 26);
            this.gridControlSwapService.MainView = this.gridViewSwapService;
            this.gridControlSwapService.Name = "gridControlSwapService";
            this.gridControlSwapService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEditService,
            this.repositoryItemChkOutKtcFee,
            this.repositoryItemChkIsExpend,
            this.repositoryItemcboPatientType,
            this.repositoryItemSpinEditAmount});
            this.gridControlSwapService.Size = new System.Drawing.Size(1126, 298);
            this.gridControlSwapService.TabIndex = 6;
            this.gridControlSwapService.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSwapService});
            // 
            // gridViewSwapService
            // 
            this.gridViewSwapService.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColServiceCode,
            this.gridColServiceName,
            this.gridColAmount,
            this.gridColPrice,
            this.gridCol,
            this.gridColExpend,
            this.gridColIsOutParentFee,
            this.gridColumn1});
            this.gridViewSwapService.GridControl = this.gridControlSwapService;
            this.gridViewSwapService.Name = "gridViewSwapService";
            this.gridViewSwapService.OptionsDetail.EnableMasterViewMode = false;
            this.gridViewSwapService.OptionsFind.FindDelay = 100;
            this.gridViewSwapService.OptionsFind.FindNullPrompt = "Nhập từ khóa tìm kiếm ...";
            this.gridViewSwapService.OptionsFind.ShowClearButton = false;
            this.gridViewSwapService.OptionsFind.ShowCloseButton = false;
            this.gridViewSwapService.OptionsFind.ShowFindButton = false;
            this.gridViewSwapService.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewSwapService.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewSwapService.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewSwapService.OptionsView.ShowGroupPanel = false;
            this.gridViewSwapService.OptionsView.ShowIndicator = false;
            this.gridViewSwapService.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSwapService_CustomUnboundColumnData);
            this.gridViewSwapService.Click += new System.EventHandler(this.gridViewSwapService_Click);
            // 
            // gridColServiceCode
            // 
            this.gridColServiceCode.Caption = "Mã dịch vụ";
            this.gridColServiceCode.FieldName = "TDL_SERVICE_CODE";
            this.gridColServiceCode.Name = "gridColServiceCode";
            this.gridColServiceCode.OptionsColumn.AllowEdit = false;
            this.gridColServiceCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.gridColServiceCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridColServiceCode.Visible = true;
            this.gridColServiceCode.VisibleIndex = 1;
            this.gridColServiceCode.Width = 96;
            // 
            // gridColServiceName
            // 
            this.gridColServiceName.Caption = "Tên dịch vụ";
            this.gridColServiceName.FieldName = "TDL_SERVICE_NAME";
            this.gridColServiceName.Name = "gridColServiceName";
            this.gridColServiceName.Visible = true;
            this.gridColServiceName.VisibleIndex = 2;
            this.gridColServiceName.Width = 444;
            // 
            // gridColAmount
            // 
            this.gridColAmount.Caption = "Số lượng";
            this.gridColAmount.ColumnEdit = this.repositoryItemSpinEditAmount;
            this.gridColAmount.FieldName = "AMOUNT";
            this.gridColAmount.Name = "gridColAmount";
            this.gridColAmount.Visible = true;
            this.gridColAmount.VisibleIndex = 3;
            this.gridColAmount.Width = 104;
            // 
            // repositoryItemSpinEditAmount
            // 
            this.repositoryItemSpinEditAmount.AutoHeight = false;
            this.repositoryItemSpinEditAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEditAmount.MaxLength = 9999999;
            this.repositoryItemSpinEditAmount.Name = "repositoryItemSpinEditAmount";
            // 
            // gridColPrice
            // 
            this.gridColPrice.Caption = "Đơn giá";
            this.gridColPrice.FieldName = "PRICE_DISPLAY";
            this.gridColPrice.Name = "gridColPrice";
            this.gridColPrice.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColPrice.Visible = true;
            this.gridColPrice.VisibleIndex = 4;
            this.gridColPrice.Width = 128;
            // 
            // gridCol
            // 
            this.gridCol.Caption = "ĐT thanh toán";
            this.gridCol.ColumnEdit = this.repositoryItemcboPatientType;
            this.gridCol.FieldName = "PATIENT_TYPE_ID";
            this.gridCol.Name = "gridCol";
            this.gridCol.Visible = true;
            this.gridCol.VisibleIndex = 5;
            this.gridCol.Width = 115;
            // 
            // repositoryItemcboPatientType
            // 
            this.repositoryItemcboPatientType.AutoHeight = false;
            this.repositoryItemcboPatientType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemcboPatientType.Name = "repositoryItemcboPatientType";
            this.repositoryItemcboPatientType.NullText = "";
            this.repositoryItemcboPatientType.View = this.repositoryItemGridLookUpEdit1View;
            // 
            // repositoryItemGridLookUpEdit1View
            // 
            this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
            this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // gridColExpend
            // 
            this.gridColExpend.Caption = "Hao phí";
            this.gridColExpend.FieldName = "IsExpend";
            this.gridColExpend.Name = "gridColExpend";
            this.gridColExpend.Visible = true;
            this.gridColExpend.VisibleIndex = 6;
            this.gridColExpend.Width = 82;
            // 
            // gridColIsOutParentFee
            // 
            this.gridColIsOutParentFee.Caption = "CP ngoài gói";
            this.gridColIsOutParentFee.FieldName = "IsOutKtcFee";
            this.gridColIsOutParentFee.Name = "gridColIsOutParentFee";
            this.gridColIsOutParentFee.Visible = true;
            this.gridColIsOutParentFee.VisibleIndex = 7;
            this.gridColIsOutParentFee.Width = 105;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.ColumnEdit = this.repositoryItemCheckEditService;
            this.gridColumn1.FieldName = "checkService";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ShowCaption = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 20;
            // 
            // repositoryItemCheckEditService
            // 
            this.repositoryItemCheckEditService.AutoHeight = false;
            this.repositoryItemCheckEditService.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.repositoryItemCheckEditService.Name = "repositoryItemCheckEditService";
            this.repositoryItemCheckEditService.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEditService.Click += new System.EventHandler(this.repositoryItemCheckEditService_Click);
            // 
            // repositoryItemChkOutKtcFee
            // 
            this.repositoryItemChkOutKtcFee.AutoHeight = false;
            this.repositoryItemChkOutKtcFee.Name = "repositoryItemChkOutKtcFee";
            this.repositoryItemChkOutKtcFee.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // repositoryItemChkIsExpend
            // 
            this.repositoryItemChkIsExpend.AutoHeight = false;
            this.repositoryItemChkIsExpend.Name = "repositoryItemChkIsExpend";
            this.repositoryItemChkIsExpend.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // btnSwapService
            // 
            this.btnSwapService.Location = new System.Drawing.Point(943, 328);
            this.btnSwapService.Name = "btnSwapService";
            this.btnSwapService.Size = new System.Drawing.Size(185, 22);
            this.btnSwapService.StyleController = this.layoutControl1;
            this.btnSwapService.TabIndex = 5;
            this.btnSwapService.Text = "Đổi dịch vụ";
            this.btnSwapService.Click += new System.EventHandler(this.btnSwapService_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1130, 352);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSwapService;
            this.layoutControlItem2.Location = new System.Drawing.Point(941, 326);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(189, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 326);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(941, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlSwapService;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1130, 302);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtKeyword;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(475, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(475, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(655, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmSwapService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 352);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSwapService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đổi dịch vụ phẫu thuật thủ thuật";
            this.Load += new System.EventHandler(this.frmSwapService_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSwapService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSwapService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemcboPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemGridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkOutKtcFee)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemChkIsExpend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnSwapService;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.GridControl gridControlSwapService;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSwapService;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol;
        private DevExpress.XtraGrid.Columns.GridColumn gridColExpend;
        private DevExpress.XtraGrid.Columns.GridColumn gridColIsOutParentFee;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditService;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemChkOutKtcFee;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemChkIsExpend;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemGridLookUpEdit1View;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEditAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;

    }
}
