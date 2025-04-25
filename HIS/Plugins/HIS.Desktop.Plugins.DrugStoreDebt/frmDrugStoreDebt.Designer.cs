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
namespace HIS.Desktop.Plugins.DrugStoreDebt
{
    partial class frmDrugStoreDebt
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.cboCashierRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtTransactionCode = new DevExpress.XtraEditors.TextEdit();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveAndPrint = new DevExpress.XtraEditors.SimpleButton();
            this.spinNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.cboAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtTransactionTime = new DevExpress.XtraEditors.DateEdit();
            this.gridControlExpMestDetail = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridViewExpMestDetail = new Inventec.Desktop.CustomControl.MyGridView();
            this.gridCol_ExpMestCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_MediMateName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_PriceVat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_DiscountAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_TotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMEDI_MATE_CODEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMEDI_MATE_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSERVICE_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAMOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPRICE_VATUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDISCOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTOTAL_PRICEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPRICE_VAT_STRUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDISCOUNT_STRUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTOTAL_PRICE_STRUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colITEM_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEXP_MEST_CODEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.lblDob = new DevExpress.XtraEditors.LabelControl();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtExpMestCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.spinDebtAmount = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpMestCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTransactionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCashierRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDebtAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnItemSaveAndPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnItemPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnItemNew = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMestCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDebtAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCashierRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDebtAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.cboCashierRoom);
            this.layoutControl1.Controls.Add(this.txtTransactionCode);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnSaveAndPrint);
            this.layoutControl1.Controls.Add(this.spinNumOrder);
            this.layoutControl1.Controls.Add(this.cboAccountBook);
            this.layoutControl1.Controls.Add(this.dtTransactionTime);
            this.layoutControl1.Controls.Add(this.gridControlExpMestDetail);
            this.layoutControl1.Controls.Add(this.lblAddress);
            this.layoutControl1.Controls.Add(this.lblDob);
            this.layoutControl1.Controls.Add(this.lblGender);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Controls.Add(this.lblPatientCode);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Controls.Add(this.txtExpMestCode);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.spinDebtAmount);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(990, 582);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(772, 558);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(106, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 19;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // cboCashierRoom
            // 
            this.cboCashierRoom.Location = new System.Drawing.Point(867, 486);
            this.cboCashierRoom.Name = "cboCashierRoom";
            this.cboCashierRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCashierRoom.Properties.NullText = "";
            this.cboCashierRoom.Properties.View = this.gridView1;
            this.cboCashierRoom.Size = new System.Drawing.Size(121, 20);
            this.cboCashierRoom.StyleController = this.layoutControl1;
            this.cboCashierRoom.TabIndex = 23;
            this.cboCashierRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCashierRoom_Closed);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // txtTransactionCode
            // 
            this.txtTransactionCode.Location = new System.Drawing.Point(867, 510);
            this.txtTransactionCode.Name = "txtTransactionCode";
            this.txtTransactionCode.Properties.ReadOnly = true;
            this.txtTransactionCode.Size = new System.Drawing.Size(121, 20);
            this.txtTransactionCode.StyleController = this.layoutControl1;
            this.txtTransactionCode.TabIndex = 21;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(882, 558);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(106, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 20;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(662, 558);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAndPrint
            // 
            this.btnSaveAndPrint.Location = new System.Drawing.Point(552, 558);
            this.btnSaveAndPrint.Name = "btnSaveAndPrint";
            this.btnSaveAndPrint.Size = new System.Drawing.Size(106, 22);
            this.btnSaveAndPrint.StyleController = this.layoutControl1;
            this.btnSaveAndPrint.TabIndex = 17;
            this.btnSaveAndPrint.Text = "Lưu in (Ctrl I)";
            this.btnSaveAndPrint.Click += new System.EventHandler(this.btnSaveAndPrint_Click);
            // 
            // spinNumOrder
            // 
            this.spinNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumOrder.Location = new System.Drawing.Point(657, 486);
            this.spinNumOrder.Name = "spinNumOrder";
            this.spinNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumOrder.Size = new System.Drawing.Size(111, 20);
            this.spinNumOrder.StyleController = this.layoutControl1;
            this.spinNumOrder.TabIndex = 15;
            this.spinNumOrder.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(this.spinNumOrder_Spin);
            this.spinNumOrder.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinNumOrder_PreviewKeyDown);
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(382, 486);
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Properties.View = this.gridLookUpEdit1View;
            this.cboAccountBook.Size = new System.Drawing.Size(166, 20);
            this.cboAccountBook.StyleController = this.layoutControl1;
            this.cboAccountBook.TabIndex = 14;
            this.cboAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAccountBook_Closed);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // dtTransactionTime
            // 
            this.dtTransactionTime.EditValue = null;
            this.dtTransactionTime.Location = new System.Drawing.Point(107, 486);
            this.dtTransactionTime.Name = "dtTransactionTime";
            this.dtTransactionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTransactionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTransactionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTransactionTime.Size = new System.Drawing.Size(166, 20);
            this.dtTransactionTime.StyleController = this.layoutControl1;
            this.dtTransactionTime.TabIndex = 13;
            this.dtTransactionTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTransactionTime_Closed);
            this.dtTransactionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTransactionTime_PreviewKeyDown);
            // 
            // gridControlExpMestDetail
            // 
            this.gridControlExpMestDetail.Location = new System.Drawing.Point(0, 74);
            this.gridControlExpMestDetail.MainView = this.gridViewExpMestDetail;
            this.gridControlExpMestDetail.Name = "gridControlExpMestDetail";
            this.gridControlExpMestDetail.Size = new System.Drawing.Size(990, 410);
            this.gridControlExpMestDetail.TabIndex = 12;
            this.gridControlExpMestDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestDetail});
            // 
            // gridViewExpMestDetail
            // 
            this.gridViewExpMestDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridCol_ExpMestCode,
            this.gridCol_MediMateName,
            this.gridCol_Amount,
            this.gridCol_PriceVat,
            this.gridCol_DiscountAmount,
            this.gridCol_TotalPrice,
            this.gridColumn1,
            this.colMEDI_MATE_CODEUnb,
            this.colMEDI_MATE_NAMEUnb,
            this.colSERVICE_UNIT_NAMEUnb,
            this.colAMOUNTUnb,
            this.colPRICE_VATUnb,
            this.colDISCOUNTUnb,
            this.colTOTAL_PRICEUnb,
            this.colUnb,
            this.colPRICE_VAT_STRUnb,
            this.colDISCOUNT_STRUnb,
            this.colTOTAL_PRICE_STRUnb,
            this.colITEM_NAMEUnb,
            this.colEXP_MEST_CODEUnb});
            this.gridViewExpMestDetail.GridControl = this.gridControlExpMestDetail;
            this.gridViewExpMestDetail.GroupCount = 1;
            this.gridViewExpMestDetail.Name = "gridViewExpMestDetail";
            this.gridViewExpMestDetail.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridViewExpMestDetail.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewExpMestDetail.OptionsSelection.MultiSelect = true;
            this.gridViewExpMestDetail.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewExpMestDetail.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewExpMestDetail.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestDetail.OptionsView.ShowIndicator = false;
            this.gridViewExpMestDetail.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridCol_ExpMestCode, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridViewExpMestDetail.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewExpMestDetail_RowCellStyle);
            this.gridViewExpMestDetail.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewExpMestDetail_SelectionChanged);
            this.gridViewExpMestDetail.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestDetail_CustomUnboundColumnData);
            this.gridViewExpMestDetail.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewExpMestDetail_MouseDown);
            // 
            // gridCol_ExpMestCode
            // 
            this.gridCol_ExpMestCode.Caption = "Mã phiếu xuất";
            this.gridCol_ExpMestCode.FieldName = "EXP_MEST_CODE_PLUS";
            this.gridCol_ExpMestCode.Name = "gridCol_ExpMestCode";
            this.gridCol_ExpMestCode.OptionsColumn.AllowEdit = false;
            this.gridCol_ExpMestCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_ExpMestCode.Visible = true;
            this.gridCol_ExpMestCode.VisibleIndex = 2;
            // 
            // gridCol_MediMateName
            // 
            this.gridCol_MediMateName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_MediMateName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_MediMateName.Caption = "Tên";
            this.gridCol_MediMateName.FieldName = "ITEM_NAME";
            this.gridCol_MediMateName.FieldNameSortGroup = "ITEM_NAMEUnb";
            this.gridCol_MediMateName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridCol_MediMateName.Name = "gridCol_MediMateName";
            this.gridCol_MediMateName.OptionsColumn.AllowEdit = false;
            this.gridCol_MediMateName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_MediMateName.Visible = true;
            this.gridCol_MediMateName.VisibleIndex = 1;
            this.gridCol_MediMateName.Width = 463;
            // 
            // gridCol_Amount
            // 
            this.gridCol_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridCol_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridCol_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_Amount.Caption = "Số lượng";
            this.gridCol_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridCol_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridCol_Amount.FieldName = "AMOUNT";
            this.gridCol_Amount.FieldNameSortGroup = "AMOUNTUnb";
            this.gridCol_Amount.Name = "gridCol_Amount";
            this.gridCol_Amount.OptionsColumn.AllowEdit = false;
            this.gridCol_Amount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_Amount.Visible = true;
            this.gridCol_Amount.VisibleIndex = 2;
            this.gridCol_Amount.Width = 110;
            // 
            // gridCol_PriceVat
            // 
            this.gridCol_PriceVat.AppearanceCell.Options.UseTextOptions = true;
            this.gridCol_PriceVat.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridCol_PriceVat.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_PriceVat.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_PriceVat.Caption = "Đơn giá";
            this.gridCol_PriceVat.DisplayFormat.FormatString = "#,##0.000";
            this.gridCol_PriceVat.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridCol_PriceVat.FieldName = "PRICE_VAT_STR";
            this.gridCol_PriceVat.FieldNameSortGroup = "PRICE_VAT_STRUnb";
            this.gridCol_PriceVat.Name = "gridCol_PriceVat";
            this.gridCol_PriceVat.OptionsColumn.AllowEdit = false;
            this.gridCol_PriceVat.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_PriceVat.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridCol_PriceVat.Visible = true;
            this.gridCol_PriceVat.VisibleIndex = 3;
            this.gridCol_PriceVat.Width = 120;
            // 
            // gridCol_DiscountAmount
            // 
            this.gridCol_DiscountAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridCol_DiscountAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridCol_DiscountAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_DiscountAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_DiscountAmount.Caption = "Chiết khấu";
            this.gridCol_DiscountAmount.DisplayFormat.FormatString = "#,##0.000";
            this.gridCol_DiscountAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridCol_DiscountAmount.FieldName = "DISCOUNT_STR";
            this.gridCol_DiscountAmount.FieldNameSortGroup = "DISCOUNT_STRUnb";
            this.gridCol_DiscountAmount.Name = "gridCol_DiscountAmount";
            this.gridCol_DiscountAmount.OptionsColumn.AllowEdit = false;
            this.gridCol_DiscountAmount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_DiscountAmount.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridCol_DiscountAmount.Visible = true;
            this.gridCol_DiscountAmount.VisibleIndex = 4;
            this.gridCol_DiscountAmount.Width = 120;
            // 
            // gridCol_TotalPrice
            // 
            this.gridCol_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridCol_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridCol_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_TotalPrice.Caption = "Thành tiền";
            this.gridCol_TotalPrice.DisplayFormat.FormatString = "#,##0.000";
            this.gridCol_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridCol_TotalPrice.FieldName = "TOTAL_PRICE_STR";
            this.gridCol_TotalPrice.FieldNameSortGroup = "TOTAL_PRICE_STRUnb";
            this.gridCol_TotalPrice.Name = "gridCol_TotalPrice";
            this.gridCol_TotalPrice.OptionsColumn.AllowEdit = false;
            this.gridCol_TotalPrice.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.gridCol_TotalPrice.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridCol_TotalPrice.Visible = true;
            this.gridCol_TotalPrice.VisibleIndex = 5;
            this.gridCol_TotalPrice.Width = 160;
            // 
            // gridColumn1
            // 
            this.gridColumn1.FieldName = "DX$CheckboxSelectorColumnUnb";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMEDI_MATE_CODEUnb
            // 
            this.colMEDI_MATE_CODEUnb.FieldName = "MEDI_MATE_CODEUnb";
            this.colMEDI_MATE_CODEUnb.Name = "colMEDI_MATE_CODEUnb";
            this.colMEDI_MATE_CODEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMEDI_MATE_NAMEUnb
            // 
            this.colMEDI_MATE_NAMEUnb.FieldName = "MEDI_MATE_NAMEUnb";
            this.colMEDI_MATE_NAMEUnb.Name = "colMEDI_MATE_NAMEUnb";
            this.colMEDI_MATE_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colSERVICE_UNIT_NAMEUnb
            // 
            this.colSERVICE_UNIT_NAMEUnb.FieldName = "SERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.Name = "colSERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colAMOUNTUnb
            // 
            this.colAMOUNTUnb.FieldName = "AMOUNTUnb";
            this.colAMOUNTUnb.Name = "colAMOUNTUnb";
            this.colAMOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colPRICE_VATUnb
            // 
            this.colPRICE_VATUnb.FieldName = "PRICE_VATUnb";
            this.colPRICE_VATUnb.Name = "colPRICE_VATUnb";
            this.colPRICE_VATUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDISCOUNTUnb
            // 
            this.colDISCOUNTUnb.FieldName = "DISCOUNTUnb";
            this.colDISCOUNTUnb.Name = "colDISCOUNTUnb";
            this.colDISCOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colTOTAL_PRICEUnb
            // 
            this.colTOTAL_PRICEUnb.FieldName = "TOTAL_PRICEUnb";
            this.colTOTAL_PRICEUnb.Name = "colTOTAL_PRICEUnb";
            this.colTOTAL_PRICEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colUnb
            // 
            this.colUnb.FieldName = "Unb";
            this.colUnb.Name = "colUnb";
            this.colUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colPRICE_VAT_STRUnb
            // 
            this.colPRICE_VAT_STRUnb.FieldName = "PRICE_VAT_STRUnb";
            this.colPRICE_VAT_STRUnb.Name = "colPRICE_VAT_STRUnb";
            this.colPRICE_VAT_STRUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDISCOUNT_STRUnb
            // 
            this.colDISCOUNT_STRUnb.FieldName = "DISCOUNT_STRUnb";
            this.colDISCOUNT_STRUnb.Name = "colDISCOUNT_STRUnb";
            this.colDISCOUNT_STRUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colTOTAL_PRICE_STRUnb
            // 
            this.colTOTAL_PRICE_STRUnb.FieldName = "TOTAL_PRICE_STRUnb";
            this.colTOTAL_PRICE_STRUnb.Name = "colTOTAL_PRICE_STRUnb";
            this.colTOTAL_PRICE_STRUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colITEM_NAMEUnb
            // 
            this.colITEM_NAMEUnb.FieldName = "ITEM_NAMEUnb";
            this.colITEM_NAMEUnb.Name = "colITEM_NAMEUnb";
            this.colITEM_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colEXP_MEST_CODEUnb
            // 
            this.colEXP_MEST_CODEUnb.FieldName = "EXP_MEST_CODEUnb";
            this.colEXP_MEST_CODEUnb.Name = "colEXP_MEST_CODEUnb";
            this.colEXP_MEST_CODEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // lblAddress
            // 
            this.lblAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAddress.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAddress.Location = new System.Drawing.Point(97, 52);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(891, 20);
            this.lblAddress.StyleController = this.layoutControl1;
            this.lblAddress.TabIndex = 11;
            // 
            // lblDob
            // 
            this.lblDob.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDob.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDob.Location = new System.Drawing.Point(903, 28);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(85, 20);
            this.lblDob.StyleController = this.layoutControl1;
            this.lblDob.TabIndex = 10;
            // 
            // lblGender
            // 
            this.lblGender.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGender.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGender.Location = new System.Drawing.Point(673, 28);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(131, 20);
            this.lblGender.StyleController = this.layoutControl1;
            this.lblGender.TabIndex = 9;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(327, 28);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(247, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 8;
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientCode.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(97, 28);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(131, 20);
            this.lblPatientCode.StyleController = this.layoutControl1;
            this.lblPatientCode.TabIndex = 7;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(232, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(111, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 6;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtExpMestCode
            // 
            this.txtExpMestCode.Location = new System.Drawing.Point(117, 2);
            this.txtExpMestCode.Name = "txtExpMestCode";
            this.txtExpMestCode.Properties.NullValuePrompt = "Mã phiếu xuất";
            this.txtExpMestCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtExpMestCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtExpMestCode.Size = new System.Drawing.Size(111, 20);
            this.txtExpMestCode.StyleController = this.layoutControl1;
            this.txtExpMestCode.TabIndex = 5;
            this.txtExpMestCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExpMestCode_KeyPress);
            this.txtExpMestCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExpMestCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(111, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 4;
            this.txtTreatmentCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTreatmentCode_KeyPress);
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(107, 510);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.MaxLength = 2000;
            this.txtDescription.Size = new System.Drawing.Size(661, 44);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 16;
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            // 
            // spinDebtAmount
            // 
            this.spinDebtAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDebtAmount.Location = new System.Drawing.Point(867, 534);
            this.spinDebtAmount.Name = "spinDebtAmount";
            this.spinDebtAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Bold);
            this.spinDebtAmount.Properties.Appearance.ForeColor = System.Drawing.Color.Red;
            this.spinDebtAmount.Properties.Appearance.Options.UseFont = true;
            this.spinDebtAmount.Properties.Appearance.Options.UseForeColor = true;
            this.spinDebtAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDebtAmount.Properties.DisplayFormat.FormatString = "#,##0.00";
            this.spinDebtAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinDebtAmount.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.spinDebtAmount.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.spinDebtAmount.Size = new System.Drawing.Size(121, 20);
            this.spinDebtAmount.StyleController = this.layoutControl1;
            this.spinDebtAmount.TabIndex = 22;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTreatmentCode,
            this.lciExpMestCode,
            this.layoutControlItem3,
            this.lciPatientCode,
            this.lciPatientName,
            this.lciGender,
            this.lciDob,
            this.lciAddress,
            this.layoutControlItem9,
            this.lciTransactionTime,
            this.lciAccountBook,
            this.lciNumOrder,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem17,
            this.lciDescription,
            this.emptySpaceItem2,
            this.emptySpaceItem3,
            this.lciTransactionCode,
            this.lciCashierRoom,
            this.lciDebtAmount,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(990, 582);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.Control = this.txtTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(0, 0);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(115, 26);
            this.lciTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciTreatmentCode.TextToControlDistance = 0;
            this.lciTreatmentCode.TextVisible = false;
            // 
            // lciExpMestCode
            // 
            this.lciExpMestCode.Control = this.txtExpMestCode;
            this.lciExpMestCode.Location = new System.Drawing.Point(115, 0);
            this.lciExpMestCode.Name = "lciExpMestCode";
            this.lciExpMestCode.Size = new System.Drawing.Size(115, 26);
            this.lciExpMestCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpMestCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciExpMestCode.TextToControlDistance = 0;
            this.lciExpMestCode.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnFind;
            this.layoutControlItem3.Location = new System.Drawing.Point(230, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(115, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.lblPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(0, 26);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(230, 24);
            this.lciPatientCode.Text = "Mã bệnh nhân:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(230, 26);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(346, 24);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciGender
            // 
            this.lciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGender.Control = this.lblGender;
            this.lciGender.Location = new System.Drawing.Point(576, 26);
            this.lciGender.Name = "lciGender";
            this.lciGender.Size = new System.Drawing.Size(230, 24);
            this.lciGender.Text = "Giới tính:";
            this.lciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGender.TextSize = new System.Drawing.Size(90, 20);
            this.lciGender.TextToControlDistance = 5;
            // 
            // lciDob
            // 
            this.lciDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDob.Control = this.lblDob;
            this.lciDob.Location = new System.Drawing.Point(806, 26);
            this.lciDob.Name = "lciDob";
            this.lciDob.Size = new System.Drawing.Size(184, 24);
            this.lciDob.Text = "Ngày sinh:";
            this.lciDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciDob.TextToControlDistance = 5;
            // 
            // lciAddress
            // 
            this.lciAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAddress.Control = this.lblAddress;
            this.lciAddress.Location = new System.Drawing.Point(0, 50);
            this.lciAddress.Name = "lciAddress";
            this.lciAddress.Size = new System.Drawing.Size(990, 24);
            this.lciAddress.Text = "Địa chỉ:";
            this.lciAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAddress.TextSize = new System.Drawing.Size(90, 20);
            this.lciAddress.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.gridControlExpMestDetail;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 74);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem9.Size = new System.Drawing.Size(990, 410);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // lciTransactionTime
            // 
            this.lciTransactionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciTransactionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTransactionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionTime.Control = this.dtTransactionTime;
            this.lciTransactionTime.Location = new System.Drawing.Point(0, 484);
            this.lciTransactionTime.Name = "lciTransactionTime";
            this.lciTransactionTime.Size = new System.Drawing.Size(275, 24);
            this.lciTransactionTime.Text = "Thời gian chốt:";
            this.lciTransactionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionTime.TextSize = new System.Drawing.Size(100, 20);
            this.lciTransactionTime.TextToControlDistance = 5;
            // 
            // lciAccountBook
            // 
            this.lciAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciAccountBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAccountBook.Control = this.cboAccountBook;
            this.lciAccountBook.Location = new System.Drawing.Point(275, 484);
            this.lciAccountBook.Name = "lciAccountBook";
            this.lciAccountBook.Size = new System.Drawing.Size(275, 24);
            this.lciAccountBook.Text = "Sổ thu chi:";
            this.lciAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAccountBook.TextSize = new System.Drawing.Size(100, 20);
            this.lciAccountBook.TextToControlDistance = 5;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumOrder.Control = this.spinNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(550, 484);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.Size = new System.Drawing.Size(220, 24);
            this.lciNumOrder.Text = "Số chứng từ:";
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(100, 20);
            this.lciNumOrder.TextToControlDistance = 5;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.btnSaveAndPrint;
            this.layoutControlItem14.Location = new System.Drawing.Point(550, 556);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.btnSave;
            this.layoutControlItem15.Location = new System.Drawing.Point(660, 556);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.btnNew;
            this.layoutControlItem17.Location = new System.Drawing.Point(880, 556);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // lciDescription
            // 
            this.lciDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDescription.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lciDescription.Control = this.txtDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 508);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(770, 48);
            this.lciDescription.Text = "Ghi chú:";
            this.lciDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDescription.TextSize = new System.Drawing.Size(100, 20);
            this.lciDescription.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(345, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(645, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 556);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(550, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciTransactionCode
            // 
            this.lciTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionCode.Control = this.txtTransactionCode;
            this.lciTransactionCode.Location = new System.Drawing.Point(770, 508);
            this.lciTransactionCode.Name = "lciTransactionCode";
            this.lciTransactionCode.Size = new System.Drawing.Size(220, 24);
            this.lciTransactionCode.Text = "Mã giao dịch:";
            this.lciTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciTransactionCode.TextToControlDistance = 5;
            // 
            // lciCashierRoom
            // 
            this.lciCashierRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCashierRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCashierRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCashierRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCashierRoom.Control = this.cboCashierRoom;
            this.lciCashierRoom.Location = new System.Drawing.Point(770, 484);
            this.lciCashierRoom.Name = "lciCashierRoom";
            this.lciCashierRoom.OptionsToolTip.ToolTip = "Phòng thu ngân";
            this.lciCashierRoom.Size = new System.Drawing.Size(220, 24);
            this.lciCashierRoom.Text = "Phòng TN:";
            this.lciCashierRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCashierRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciCashierRoom.TextToControlDistance = 5;
            // 
            // lciDebtAmount
            // 
            this.lciDebtAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDebtAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDebtAmount.Control = this.spinDebtAmount;
            this.lciDebtAmount.Location = new System.Drawing.Point(770, 532);
            this.lciDebtAmount.Name = "lciDebtAmount";
            this.lciDebtAmount.Size = new System.Drawing.Size(220, 24);
            this.lciDebtAmount.Text = "Số nợ cần chốt:";
            this.lciDebtAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDebtAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lciDebtAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(770, 556);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnItemSaveAndPrint,
            this.barBtnItemSave,
            this.barBtnItemPrint,
            this.barBtnItemNew});
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemSaveAndPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemNew)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnItemSaveAndPrint
            // 
            this.barBtnItemSaveAndPrint.Caption = "Lưu in (Ctrl I)";
            this.barBtnItemSaveAndPrint.Id = 0;
            this.barBtnItemSaveAndPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.barBtnItemSaveAndPrint.Name = "barBtnItemSaveAndPrint";
            this.barBtnItemSaveAndPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemSaveAndPrint_ItemClick);
            // 
            // barBtnItemSave
            // 
            this.barBtnItemSave.Caption = "Lưu (Ctrl S)";
            this.barBtnItemSave.Id = 1;
            this.barBtnItemSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnItemSave.Name = "barBtnItemSave";
            this.barBtnItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemSave_ItemClick);
            // 
            // barBtnItemPrint
            // 
            this.barBtnItemPrint.Caption = "In (Ctrl P)";
            this.barBtnItemPrint.Id = 2;
            this.barBtnItemPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnItemPrint.Name = "barBtnItemPrint";
            this.barBtnItemPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemPrint_ItemClick);
            // 
            // barBtnItemNew
            // 
            this.barBtnItemNew.Caption = "Mới (Ctrl N)";
            this.barBtnItemNew.Id = 3;
            this.barBtnItemNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barBtnItemNew.Name = "barBtnItemNew";
            this.barBtnItemNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemNew_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 611);
            this.barDockControlBottom.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 582);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(990, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 582);
            // 
            // frmDrugStoreDebt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 611);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmDrugStoreDebt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chốt nợ nhà thuốc";
            this.Load += new System.EventHandler(this.frmDrugStoreDebt_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpMestCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDebtAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpMestCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCashierRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDebtAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtTransactionCode;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndPrint;
        private DevExpress.XtraEditors.SpinEdit spinNumOrder;
        private DevExpress.XtraEditors.GridLookUpEdit cboAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.DateEdit dtTransactionTime;
        private Inventec.Desktop.CustomControl.MyGridControl gridControlExpMestDetail;
        private Inventec.Desktop.CustomControl.MyGridView gridViewExpMestDetail;
        private DevExpress.XtraEditors.LabelControl lblAddress;
        private DevExpress.XtraEditors.LabelControl lblDob;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.TextEdit txtExpMestCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciExpMestCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciGender;
        private DevExpress.XtraLayout.LayoutControlItem lciDob;
        private DevExpress.XtraLayout.LayoutControlItem lciAddress;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciDebtAmount;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_MediMateName;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_PriceVat;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_TotalPrice;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.SpinEdit spinDebtAmount;
        private DevExpress.XtraEditors.GridLookUpEdit cboCashierRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciCashierRoom;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_DiscountAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn colMEDI_MATE_CODEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colMEDI_MATE_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colSERVICE_UNIT_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colAMOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colPRICE_VATUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDISCOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colTOTAL_PRICEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colUnb;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_ExpMestCode;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraGrid.Columns.GridColumn colPRICE_VAT_STRUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDISCOUNT_STRUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colTOTAL_PRICE_STRUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colITEM_NAMEUnb;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem barBtnItemSaveAndPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnItemSave;
        private DevExpress.XtraBars.BarButtonItem barBtnItemPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnItemNew;
        private DevExpress.XtraGrid.Columns.GridColumn colEXP_MEST_CODEUnb;
    }
}
