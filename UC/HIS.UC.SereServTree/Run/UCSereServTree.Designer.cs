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
namespace HIS.UC.SereServTree.Run
{
    partial class UCSereServTree
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
            this.trvService = new DevExpress.XtraTreeList.TreeList();
            this.treeColumnServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnAmount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnPriceNoVAT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnVATPercent = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalHeinPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPatientPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnDiscount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceReqCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTransactionCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnIsExpend = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnDVKTC = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemchkIsExpend__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemchkIsExpend__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.toolTipController = new DevExpress.Utils.ToolTipController();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // trvService
            // 
            this.trvService.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedCell.Options.UseBackColor = true;
            this.trvService.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedRow.Options.UseBackColor = true;
            this.trvService.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.trvService.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeColumnServiceName,
            this.treeColumnAmount,
            this.treeColumnPriceNoVAT,
            this.treeColumnVATPercent,
            this.treeColumnTotalPrice,
            this.treeColumnTotalHeinPrice,
            this.treeColumnTotalPatientPrice,
            this.treeColumnDiscount,
            this.treeColumnServiceCode,
            this.treeColumnServiceReqCode,
            this.treeColumnTransactionCode,
            this.treeColumnIsExpend,
            this.treeColumnDVKTC,
            this.treeListColumnPrice});
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.KeyFieldName = "CONCRETE_ID__IN_SETY";
            this.trvService.Location = new System.Drawing.Point(2, 26);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvService.OptionsBehavior.AutoPopulateColumns = false;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.FindDelay = 100;
            this.trvService.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.trvService.OptionsFind.ShowClearButton = false;
            this.trvService.OptionsFind.ShowFindButton = false;
            this.trvService.OptionsSelection.InvertSelection = true;
            this.trvService.OptionsView.AutoWidth = false;
            this.trvService.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvService.OptionsView.ShowCheckBoxes = true;
            this.trvService.OptionsView.ShowHorzLines = false;
            this.trvService.OptionsView.ShowIndicator = false;
            this.trvService.OptionsView.ShowVertLines = false;
            this.trvService.ParentFieldName = "PARENT_ID__IN_SETY";
            this.trvService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemchkIsExpend__Enable,
            this.repositoryItemchkIsExpend__Disable});
            this.trvService.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.trvService.Size = new System.Drawing.Size(872, 522);
            this.trvService.TabIndex = 3;
            this.trvService.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.trvService_GetStateImage);
            this.trvService.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.trvService_GetSelectImage);
            this.trvService.StateImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_StateImageClick);
            this.trvService.SelectImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_SelectImageClick);
            this.trvService.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trvService_CustomNodeCellEdit);
            this.trvService.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.trvService_NodeCellStyle);
            this.trvService.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
            this.trvService.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
            this.trvService.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
            this.trvService.ShownEditor += new System.EventHandler(this.trvService_ShownEditor);
            this.trvService.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
            this.trvService.CustomDrawNodeCheckBox += new DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventHandler(this.trvService_CustomDrawNodeCheckBox);
            this.trvService.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
            this.trvService.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.trvService_ShowingEditor);
            this.trvService.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
            // 
            // treeColumnServiceName
            // 
            this.treeColumnServiceName.Caption = "Tên dịch vụ";
            this.treeColumnServiceName.FieldName = "TDL_SERVICE_NAME";
            this.treeColumnServiceName.MinWidth = 34;
            this.treeColumnServiceName.Name = "treeColumnServiceName";
            this.treeColumnServiceName.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceName.Width = 202;
            // 
            // treeColumnAmount
            // 
            this.treeColumnAmount.Caption = "Số lượng";
            this.treeColumnAmount.FieldName = "AMOUNT_PLUS";
            this.treeColumnAmount.Format.FormatString = "#,##0.00";
            this.treeColumnAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnAmount.Name = "treeColumnAmount";
            this.treeColumnAmount.OptionsColumn.AllowEdit = false;
            this.treeColumnAmount.Width = 100;
            // 
            // treeColumnPriceNoVAT
            // 
            this.treeColumnPriceNoVAT.Caption = "Đơn giá trước VAT";
            this.treeColumnPriceNoVAT.FieldName = "PRICE";
            this.treeColumnPriceNoVAT.Format.FormatString = "#,##0.0000";
            this.treeColumnPriceNoVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnPriceNoVAT.Name = "treeColumnPriceNoVAT";
            this.treeColumnPriceNoVAT.OptionsColumn.AllowEdit = false;
            this.treeColumnPriceNoVAT.Width = 138;
            // 
            // treeColumnVATPercent
            // 
            this.treeColumnVATPercent.Caption = "VAT (%)";
            this.treeColumnVATPercent.FieldName = "VAT";
            this.treeColumnVATPercent.Format.FormatString = "#,##0.00";
            this.treeColumnVATPercent.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnVATPercent.Name = "treeColumnVATPercent";
            this.treeColumnVATPercent.OptionsColumn.AllowEdit = false;
            this.treeColumnVATPercent.Width = 78;
            // 
            // treeColumnTotalPrice
            // 
            this.treeColumnTotalPrice.Caption = "Thành tiền";
            this.treeColumnTotalPrice.FieldName = "VIR_TOTAL_PRICE";
            this.treeColumnTotalPrice.Format.FormatString = "#,##0.0000";
            this.treeColumnTotalPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnTotalPrice.Name = "treeColumnTotalPrice";
            this.treeColumnTotalPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPrice.Width = 138;
            // 
            // treeColumnTotalHeinPrice
            // 
            this.treeColumnTotalHeinPrice.Caption = "Đồng chi chả";
            this.treeColumnTotalHeinPrice.FieldName = "VIR_TOTAL_HEIN_PRICE";
            this.treeColumnTotalHeinPrice.Format.FormatString = "#,##0.0000";
            this.treeColumnTotalHeinPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnTotalHeinPrice.Name = "treeColumnTotalHeinPrice";
            this.treeColumnTotalHeinPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalHeinPrice.Width = 138;
            // 
            // treeColumnTotalPatientPrice
            // 
            this.treeColumnTotalPatientPrice.Caption = "Bệnh nhân chi chả";
            this.treeColumnTotalPatientPrice.FieldName = "VIR_TOTAL_PATIENT_PRICE";
            this.treeColumnTotalPatientPrice.Format.FormatString = "#,##0.0000";
            this.treeColumnTotalPatientPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnTotalPatientPrice.Name = "treeColumnTotalPatientPrice";
            this.treeColumnTotalPatientPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPatientPrice.Width = 138;
            // 
            // treeColumnDiscount
            // 
            this.treeColumnDiscount.Caption = "Chiết khấu";
            this.treeColumnDiscount.FieldName = "DISCOUNT";
            this.treeColumnDiscount.Format.FormatString = "#,##0.0000";
            this.treeColumnDiscount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnDiscount.Name = "treeColumnDiscount";
            this.treeColumnDiscount.OptionsColumn.AllowEdit = false;
            this.treeColumnDiscount.Width = 138;
            // 
            // treeColumnServiceCode
            // 
            this.treeColumnServiceCode.Caption = "Mã dịch vụ";
            this.treeColumnServiceCode.FieldName = "TDL_SERVICE_CODE";
            this.treeColumnServiceCode.MinWidth = 34;
            this.treeColumnServiceCode.Name = "treeColumnServiceCode";
            this.treeColumnServiceCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceCode.Width = 118;
            // 
            // treeColumnServiceReqCode
            // 
            this.treeColumnServiceReqCode.Caption = "Mã yêu cầu";
            this.treeColumnServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.treeColumnServiceReqCode.Name = "treeColumnServiceReqCode";
            this.treeColumnServiceReqCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceReqCode.Width = 118;
            // 
            // treeColumnTransactionCode
            // 
            this.treeColumnTransactionCode.Caption = "Mã giao dịch";
            this.treeColumnTransactionCode.FieldName = "TRANSACTION_CODE";
            this.treeColumnTransactionCode.Name = "treeColumnTransactionCode";
            this.treeColumnTransactionCode.OptionsColumn.AllowEdit = false;
            this.treeColumnTransactionCode.Width = 118;
            // 
            // treeColumnIsExpend
            // 
            this.treeColumnIsExpend.Caption = "Hao phí";
            this.treeColumnIsExpend.FieldName = "IsExpend";
            this.treeColumnIsExpend.Name = "treeColumnIsExpend";
            this.treeColumnIsExpend.Width = 78;
            // 
            // treeColumnDVKTC
            // 
            this.treeColumnDVKTC.Caption = "DVKTC";
            this.treeColumnDVKTC.FieldName = "DVKTC";
            this.treeColumnDVKTC.Format.FormatString = "#,##0.0000";
            this.treeColumnDVKTC.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnDVKTC.Name = "treeColumnDVKTC";
            this.treeColumnDVKTC.OptionsColumn.AllowEdit = false;
            this.treeColumnDVKTC.ToolTip = "Thuộc gói dịch vụ kỹ thuật cao";
            this.treeColumnDVKTC.Width = 138;
            // 
            // treeListColumnPrice
            // 
            this.treeListColumnPrice.Caption = "Đơn giá";
            this.treeListColumnPrice.FieldName = "VIR_PRICE";
            this.treeListColumnPrice.Name = "treeListColumnPrice";
            this.treeListColumnPrice.Width = 93;
            // 
            // repositoryItemchkIsExpend__Enable
            // 
            this.repositoryItemchkIsExpend__Enable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Enable.Name = "repositoryItemchkIsExpend__Enable";
            this.repositoryItemchkIsExpend__Enable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemchkIsExpend__Enable.CheckedChanged += new System.EventHandler(this.repositoryItemchkIsExpend__Enable_CheckedChanged);
            // 
            // repositoryItemchkIsExpend__Disable
            // 
            this.repositoryItemchkIsExpend__Disable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Disable.Name = "repositoryItemchkIsExpend__Disable";
            this.repositoryItemchkIsExpend__Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemchkIsExpend__Disable.ReadOnly = true;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.trvService);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(876, 550);
            this.layoutControl1.TabIndex = 31;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(872, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(876, 550);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciKeyword
            // 
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(876, 24);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.trvService;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(876, 526);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // toolTipController
            // 
            this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController_GetActiveObjectInfo);
            // 
            // UCSereServTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCSereServTree";
            this.Size = new System.Drawing.Size(876, 550);
            this.Load += new System.EventHandler(this.UCServiceTree_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTransactionCode;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnIsExpend;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceReqCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnAmount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnPriceNoVAT;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalHeinPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPatientPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnVATPercent;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnDiscount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnDVKTC;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Disable;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnPrice;
        private DevExpress.Utils.ToolTipController toolTipController;
    }
}
