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
namespace HIS.UC.MaterialType.Run
{
    partial class UCMaterialType
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
            this.treeColumnServiceCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnAmount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnPriceNoVAT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnVATPercent = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalHeinPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPatientPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnDiscount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnIsExpend = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn6 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn7 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemchkIsExpend__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemchkIsExpend__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheckEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportExcel = new DevExpress.XtraEditors.SimpleButton();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMedicineTypeAdd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExportExcel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciImport = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicineTypeAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExportExcel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciImport)).BeginInit();
            this.SuspendLayout();
            // 
            // trvService
            // 
            this.trvService.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeColumnServiceName,
            this.treeColumnServiceCode,
            this.treeColumnAmount,
            this.treeColumnPriceNoVAT,
            this.treeColumnVATPercent,
            this.treeColumnTotalPrice,
            this.treeColumnTotalHeinPrice,
            this.treeColumnTotalPatientPrice,
            this.treeColumnDiscount,
            this.treeColumnIsExpend,
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3,
            this.treeListColumn4,
            this.treeListColumn5,
            this.treeListColumn6,
            this.treeListColumn7});
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.Location = new System.Drawing.Point(2, 28);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvService.OptionsBehavior.AutoPopulateColumns = false;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.FindDelay = 100;
            this.trvService.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.trvService.OptionsFind.ShowClearButton = false;
            this.trvService.OptionsFind.ShowFindButton = false;
            this.trvService.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.trvService.OptionsView.AutoWidth = false;
            this.trvService.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvService.OptionsView.ShowCheckBoxes = true;
            this.trvService.OptionsView.ShowHorzLines = false;
            this.trvService.OptionsView.ShowIndicator = false;
            this.trvService.OptionsView.ShowVertLines = false;
            this.trvService.ParentFieldName = "PARENT_ID";
            this.trvService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemchkIsExpend__Enable,
            this.repositoryItemchkIsExpend__Disable,
            this.repositoryItemCheckEdit1,
            this.repositoryItemCheckEdit2,
            this.repositoryItemCheckEdit3,
            this.repositoryItemCheckEdit4});
            this.trvService.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.trvService.Size = new System.Drawing.Size(1312, 486);
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
            this.trvService.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
            this.trvService.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
            this.trvService.Click += new System.EventHandler(this.trvService_Click);
            this.trvService.DoubleClick += new System.EventHandler(this.trvService_DoubleClick);
            this.trvService.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
            // 
            // treeColumnServiceName
            // 
            this.treeColumnServiceName.Caption = "Tên vật tư";
            this.treeColumnServiceName.FieldName = "MATERIAL_TYPE_NAME";
            this.treeColumnServiceName.MinWidth = 34;
            this.treeColumnServiceName.Name = "treeColumnServiceName";
            this.treeColumnServiceName.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceName.Width = 202;
            // 
            // treeColumnServiceCode
            // 
            this.treeColumnServiceCode.Caption = "Mã vật tư";
            this.treeColumnServiceCode.FieldName = "MATERIAL_TYPE_CODE";
            this.treeColumnServiceCode.MinWidth = 34;
            this.treeColumnServiceCode.Name = "treeColumnServiceCode";
            this.treeColumnServiceCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceCode.Width = 118;
            // 
            // treeColumnAmount
            // 
            this.treeColumnAmount.Caption = "Hàm lượng nồng độ";
            this.treeColumnAmount.FieldName = "CONCENTRA";
            this.treeColumnAmount.Name = "treeColumnAmount";
            this.treeColumnAmount.OptionsColumn.AllowEdit = false;
            this.treeColumnAmount.Width = 120;
            // 
            // treeColumnPriceNoVAT
            // 
            this.treeColumnPriceNoVAT.Caption = "Giá nhập";
            this.treeColumnPriceNoVAT.FieldName = "IMP_PRICE_DISPLAY";
            this.treeColumnPriceNoVAT.Format.FormatString = "#,##0.0000";
            this.treeColumnPriceNoVAT.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnPriceNoVAT.Name = "treeColumnPriceNoVAT";
            this.treeColumnPriceNoVAT.OptionsColumn.AllowEdit = false;
            this.treeColumnPriceNoVAT.Width = 138;
            // 
            // treeColumnVATPercent
            // 
            this.treeColumnVATPercent.Caption = "VAT (%)";
            this.treeColumnVATPercent.FieldName = "IMP_VAT_RATIO_DISPLAY";
            this.treeColumnVATPercent.Format.FormatString = "#,##0.00";
            this.treeColumnVATPercent.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnVATPercent.Name = "treeColumnVATPercent";
            this.treeColumnVATPercent.OptionsColumn.AllowEdit = false;
            this.treeColumnVATPercent.Width = 78;
            // 
            // treeColumnTotalPrice
            // 
            this.treeColumnTotalPrice.Caption = "Quy cách đóng gói";
            this.treeColumnTotalPrice.FieldName = "PACKING_TYPE_NAME";
            this.treeColumnTotalPrice.Name = "treeColumnTotalPrice";
            this.treeColumnTotalPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPrice.Width = 150;
            // 
            // treeColumnTotalHeinPrice
            // 
            this.treeColumnTotalHeinPrice.Caption = "Cảnh báo HSD";
            this.treeColumnTotalHeinPrice.FieldName = "ALERT_EXPIRED_DATE";
            this.treeColumnTotalHeinPrice.Name = "treeColumnTotalHeinPrice";
            this.treeColumnTotalHeinPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalHeinPrice.Width = 138;
            // 
            // treeColumnTotalPatientPrice
            // 
            this.treeColumnTotalPatientPrice.Caption = "Cảnh báo tồn kho";
            this.treeColumnTotalPatientPrice.FieldName = "ALERT_MIN_IN_STOCK";
            this.treeColumnTotalPatientPrice.Name = "treeColumnTotalPatientPrice";
            this.treeColumnTotalPatientPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPatientPrice.Width = 150;
            // 
            // treeColumnDiscount
            // 
            this.treeColumnDiscount.Caption = "STT hiện";
            this.treeColumnDiscount.FieldName = "NUM_ORDER";
            this.treeColumnDiscount.Name = "treeColumnDiscount";
            this.treeColumnDiscount.OptionsColumn.AllowEdit = false;
            this.treeColumnDiscount.Width = 80;
            // 
            // treeColumnIsExpend
            // 
            this.treeColumnIsExpend.Caption = "Dừng nhập";
            this.treeColumnIsExpend.ColumnEdit = this.repositoryItemCheckEdit1;
            this.treeColumnIsExpend.FieldName = "IsStopImp";
            this.treeColumnIsExpend.Name = "treeColumnIsExpend";
            this.treeColumnIsExpend.Width = 80;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEdit1.ValueGrayed = false;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Hãng sản xuất";
            this.treeListColumn1.FieldName = "MANUFACTURER_NAME";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Width = 150;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Đơn vị tính";
            this.treeListColumn2.FieldName = "SERVICE_UNIT_NAME";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.Width = 120;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "Mã dịch vụ BHYT";
            this.treeListColumn3.FieldName = "HEIN_SERVICE_BHYT_CODE";
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.Width = 90;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.Caption = "Tên dịch vụ BHYT";
            this.treeListColumn4.FieldName = "HEIN_SERVICE_BHYT_NAME";
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.Width = 168;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.Caption = "Mã loại BH";
            this.treeListColumn5.FieldName = "HEIN_SERVICE_BHYT_TYPE_CODE";
            this.treeListColumn5.Name = "treeListColumn5";
            this.treeListColumn5.Width = 118;
            // 
            // treeListColumn6
            // 
            this.treeListColumn6.Caption = "Mã loại dịch vụ BH";
            this.treeListColumn6.FieldName = "HEIN_SERVICE_TYPE_CODE";
            this.treeListColumn6.Name = "treeListColumn6";
            this.treeListColumn6.Width = 90;
            // 
            // treeListColumn7
            // 
            this.treeListColumn7.Caption = "Tên loại dịch vụ BH";
            this.treeListColumn7.FieldName = "HEIN_SERVICE_TYPE_NAME";
            this.treeListColumn7.Name = "treeListColumn7";
            this.treeListColumn7.Width = 168;
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
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            this.repositoryItemCheckEdit2.ReadOnly = true;
            // 
            // repositoryItemCheckEdit3
            // 
            this.repositoryItemCheckEdit3.AutoHeight = false;
            this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
            this.repositoryItemCheckEdit3.ReadOnly = true;
            // 
            // repositoryItemCheckEdit4
            // 
            this.repositoryItemCheckEdit4.AutoHeight = false;
            this.repositoryItemCheckEdit4.Name = "repositoryItemCheckEdit4";
            this.repositoryItemCheckEdit4.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEdit4.ReadOnly = true;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnImport);
            this.layoutControl1.Controls.Add(this.btnExportExcel);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.trvService);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1316, 516);
            this.layoutControl1.TabIndex = 31;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(1042, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(85, 22);
            this.btnImport.StyleController = this.layoutControl1;
            this.btnImport.TabIndex = 9;
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(1221, 2);
            this.btnExportExcel.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(93, 22);
            this.btnExportExcel.StyleController = this.layoutControl1;
            this.btnExportExcel.TabIndex = 8;
            this.btnExportExcel.Text = "Xuất excel";
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1131, 2);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(86, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 7;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(1036, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyword,
            this.layoutControlItem4,
            this.lciMedicineTypeAdd,
            this.lciExportExcel,
            this.lciImport});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1316, 516);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciKeyword
            // 
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 0);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(1040, 26);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            this.lciKeyword.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.trvService;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1316, 490);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // lciMedicineTypeAdd
            // 
            this.lciMedicineTypeAdd.Control = this.btnNew;
            this.lciMedicineTypeAdd.Location = new System.Drawing.Point(1129, 0);
            this.lciMedicineTypeAdd.Name = "lciMedicineTypeAdd";
            this.lciMedicineTypeAdd.Size = new System.Drawing.Size(90, 26);
            this.lciMedicineTypeAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciMedicineTypeAdd.TextVisible = false;
            this.lciMedicineTypeAdd.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciExportExcel
            // 
            this.lciExportExcel.Control = this.btnExportExcel;
            this.lciExportExcel.Location = new System.Drawing.Point(1219, 0);
            this.lciExportExcel.Name = "lciExportExcel";
            this.lciExportExcel.Size = new System.Drawing.Size(97, 26);
            this.lciExportExcel.TextSize = new System.Drawing.Size(0, 0);
            this.lciExportExcel.TextVisible = false;
            this.lciExportExcel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciImport
            // 
            this.lciImport.Control = this.btnImport;
            this.lciImport.Location = new System.Drawing.Point(1040, 0);
            this.lciImport.Name = "lciImport";
            this.lciImport.Size = new System.Drawing.Size(89, 26);
            this.lciImport.TextSize = new System.Drawing.Size(0, 0);
            this.lciImport.TextVisible = false;
            // 
            // UCMaterialType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCMaterialType";
            this.Size = new System.Drawing.Size(1316, 516);
            this.Load += new System.EventHandler(this.UCServiceTree_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicineTypeAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExportExcel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciImport)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceName;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnIsExpend;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnAmount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnPriceNoVAT;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalHeinPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPatientPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnVATPercent;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnDiscount;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraLayout.LayoutControlItem lciMedicineTypeAdd;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn6;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn7;
        private DevExpress.XtraEditors.SimpleButton btnExportExcel;
        private DevExpress.XtraLayout.LayoutControlItem lciExportExcel;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit4;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraLayout.LayoutControlItem lciImport;
    }
}
