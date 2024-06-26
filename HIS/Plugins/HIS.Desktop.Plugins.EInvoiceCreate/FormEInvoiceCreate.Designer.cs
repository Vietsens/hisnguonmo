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
namespace HIS.Desktop.Plugins.EInvoiceCreate
{
    partial class FormEInvoiceCreate
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
            this.chkIsSplitByCashierDeposit = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnExport = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.lblError = new DevExpress.XtraEditors.LabelControl();
            this.lblSuccess = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalSelect = new DevExpress.XtraEditors.LabelControl();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.btnCreateEBill = new DevExpress.XtraEditors.SimpleButton();
            this.cboPayForm = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit4View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.spNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.gridControlTreatment = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gc_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_PatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_GenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_PatientDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_TotalPatientPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_TotalDepositAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_TotalBillAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_TotalRepayAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_InTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gc_OutTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.cboEndType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit3View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtOutDate = new DevExpress.XtraEditors.DateEdit();
            this.dtInDate = new DevExpress.XtraEditors.DateEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciInDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOutDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciEndType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboAccountBook = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPayForm = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalSelect = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSuccess = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciError = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dtLastDepositTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lciLastDepositTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.dtLastDepositTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.lciLastDepositTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsSplitByCashierDeposit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit4View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOutDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEndType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSuccess)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLastDepositTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLastDepositTimeTo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkIsSplitByCashierDeposit);
            this.layoutControl1.Controls.Add(this.lblError);
            this.layoutControl1.Controls.Add(this.lblSuccess);
            this.layoutControl1.Controls.Add(this.lblTotalSelect);
            this.layoutControl1.Controls.Add(this.separatorControl1);
            this.layoutControl1.Controls.Add(this.btnCreateEBill);
            this.layoutControl1.Controls.Add(this.cboPayForm);
            this.layoutControl1.Controls.Add(this.spNumOrder);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.cboAccountBook);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkIsSplitByCashierDeposit
            // 
            this.chkIsSplitByCashierDeposit.Location = new System.Drawing.Point(977, 74);
            this.chkIsSplitByCashierDeposit.MenuManager = this.barManager1;
            this.chkIsSplitByCashierDeposit.Name = "chkIsSplitByCashierDeposit";
            this.chkIsSplitByCashierDeposit.Properties.Caption = "Ghi nhận theo TN";
            this.chkIsSplitByCashierDeposit.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkIsSplitByCashierDeposit.Size = new System.Drawing.Size(121, 19);
            this.chkIsSplitByCashierDeposit.StyleController = this.layoutControl1;
            this.chkIsSplitByCashierDeposit.TabIndex = 13;
            this.chkIsSplitByCashierDeposit.ToolTip = "Ghi nhận theo thu ngân tạm thu dịch vụ";
            this.chkIsSplitByCashierDeposit.CheckedChanged += new System.EventHandler(this.chkIsSplitByCashierDeposit_CheckedChanged);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControl1);
            this.barManager1.DockControls.Add(this.barDockControl2);
            this.barManager1.DockControls.Add(this.barDockControl3);
            this.barManager1.DockControls.Add(this.barDockControl4);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnSearch,
            this.barBtnExport});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnExport)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSearch
            // 
            this.barBtnSearch.Caption = "Ctrl F";
            this.barBtnSearch.Id = 0;
            this.barBtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnSearch.Name = "barBtnSearch";
            this.barBtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSearch_ItemClick);
            // 
            // barBtnExport
            // 
            this.barBtnExport.Caption = "Ctrl E";
            this.barBtnExport.Id = 1;
            this.barBtnExport.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
            this.barBtnExport.Name = "barBtnExport";
            this.barBtnExport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnExport_ItemClick);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(1100, 29);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 561);
            this.barDockControl2.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(1100, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 532);
            // 
            // lblError
            // 
            this.lblError.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblError.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblError.Location = new System.Drawing.Point(977, 196);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(121, 20);
            this.lblError.StyleController = this.layoutControl1;
            this.lblError.TabIndex = 12;
            // 
            // lblSuccess
            // 
            this.lblSuccess.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblSuccess.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSuccess.Location = new System.Drawing.Point(977, 172);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(121, 20);
            this.lblSuccess.StyleController = this.layoutControl1;
            this.lblSuccess.TabIndex = 11;
            // 
            // lblTotalSelect
            // 
            this.lblTotalSelect.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalSelect.Location = new System.Drawing.Point(977, 148);
            this.lblTotalSelect.Name = "lblTotalSelect";
            this.lblTotalSelect.Size = new System.Drawing.Size(121, 20);
            this.lblTotalSelect.StyleController = this.layoutControl1;
            this.lblTotalSelect.TabIndex = 10;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(882, 124);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(216, 20);
            this.separatorControl1.TabIndex = 9;
            // 
            // btnCreateEBill
            // 
            this.btnCreateEBill.Location = new System.Drawing.Point(882, 98);
            this.btnCreateEBill.Name = "btnCreateEBill";
            this.btnCreateEBill.Size = new System.Drawing.Size(216, 22);
            this.btnCreateEBill.StyleController = this.layoutControl1;
            this.btnCreateEBill.TabIndex = 8;
            this.btnCreateEBill.Text = "Xuất hóa đơn (Ctrl E)";
            this.btnCreateEBill.Click += new System.EventHandler(this.btnCreateEBill_Click);
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(977, 50);
            this.cboPayForm.MenuManager = this.barManager1;
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Properties.View = this.gridLookUpEdit4View;
            this.cboPayForm.Size = new System.Drawing.Size(121, 20);
            this.cboPayForm.StyleController = this.layoutControl1;
            this.cboPayForm.TabIndex = 7;
            // 
            // gridLookUpEdit4View
            // 
            this.gridLookUpEdit4View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit4View.Name = "gridLookUpEdit4View";
            this.gridLookUpEdit4View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit4View.OptionsView.ShowGroupPanel = false;
            // 
            // spNumOrder
            // 
            this.spNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spNumOrder.Enabled = false;
            this.spNumOrder.Location = new System.Drawing.Point(977, 26);
            this.spNumOrder.Name = "spNumOrder";
            this.spNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spNumOrder.Size = new System.Drawing.Size(121, 20);
            this.spNumOrder.StyleController = this.layoutControl1;
            this.spNumOrder.TabIndex = 6;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.dtLastDepositTimeTo);
            this.layoutControl2.Controls.Add(this.dtLastDepositTimeFrom);
            this.layoutControl2.Controls.Add(this.ucPaging1);
            this.layoutControl2.Controls.Add(this.gridControlTreatment);
            this.layoutControl2.Controls.Add(this.btnSearch);
            this.layoutControl2.Controls.Add(this.txtPatientCode);
            this.layoutControl2.Controls.Add(this.txtTreatmentCode);
            this.layoutControl2.Controls.Add(this.txtKeyWord);
            this.layoutControl2.Controls.Add(this.cboEndType);
            this.layoutControl2.Controls.Add(this.cboDepartment);
            this.layoutControl2.Controls.Add(this.dtOutDate);
            this.layoutControl2.Controls.Add(this.dtInDate);
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(880, 532);
            this.layoutControl2.TabIndex = 5;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 510);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(876, 20);
            this.ucPaging1.TabIndex = 13;
            // 
            // gridControlTreatment
            // 
            this.gridControlTreatment.Location = new System.Drawing.Point(2, 52);
            this.gridControlTreatment.MainView = this.gridViewTreatment;
            this.gridControlTreatment.Name = "gridControlTreatment";
            this.gridControlTreatment.Size = new System.Drawing.Size(876, 454);
            this.gridControlTreatment.TabIndex = 12;
            this.gridControlTreatment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatment});
            // 
            // gridViewTreatment
            // 
            this.gridViewTreatment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gc_Stt,
            this.gc_TreatmentCode,
            this.gc_PatientCode,
            this.gc_PatientName,
            this.gc_GenderName,
            this.gc_PatientDob,
            this.gc_TotalPatientPrice,
            this.gc_TotalDepositAmount,
            this.gc_TotalBillAmount,
            this.gc_TotalRepayAmount,
            this.gc_InTime,
            this.gc_OutTime});
            this.gridViewTreatment.GridControl = this.gridControlTreatment;
            this.gridViewTreatment.Name = "gridViewTreatment";
            this.gridViewTreatment.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewTreatment.OptionsSelection.MultiSelect = true;
            this.gridViewTreatment.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewTreatment.OptionsView.ColumnAutoWidth = false;
            this.gridViewTreatment.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatment.OptionsView.ShowIndicator = false;
            this.gridViewTreatment.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewTreatment_SelectionChanged);
            this.gridViewTreatment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatment_CustomUnboundColumnData);
            // 
            // gc_Stt
            // 
            this.gc_Stt.Caption = "STT";
            this.gc_Stt.FieldName = "STT";
            this.gc_Stt.Name = "gc_Stt";
            this.gc_Stt.OptionsColumn.AllowEdit = false;
            this.gc_Stt.OptionsFilter.AllowFilter = false;
            this.gc_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_Stt.Visible = true;
            this.gc_Stt.VisibleIndex = 1;
            this.gc_Stt.Width = 30;
            // 
            // gc_TreatmentCode
            // 
            this.gc_TreatmentCode.Caption = "Mã điều trị";
            this.gc_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.gc_TreatmentCode.Name = "gc_TreatmentCode";
            this.gc_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.gc_TreatmentCode.Visible = true;
            this.gc_TreatmentCode.VisibleIndex = 2;
            this.gc_TreatmentCode.Width = 90;
            // 
            // gc_PatientCode
            // 
            this.gc_PatientCode.Caption = "Mã bệnh nhân";
            this.gc_PatientCode.FieldName = "TDL_PATIENT_CODE";
            this.gc_PatientCode.Name = "gc_PatientCode";
            this.gc_PatientCode.OptionsColumn.AllowEdit = false;
            this.gc_PatientCode.Visible = true;
            this.gc_PatientCode.VisibleIndex = 3;
            this.gc_PatientCode.Width = 80;
            // 
            // gc_PatientName
            // 
            this.gc_PatientName.Caption = "Tên bệnh nhân";
            this.gc_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.gc_PatientName.Name = "gc_PatientName";
            this.gc_PatientName.OptionsColumn.AllowEdit = false;
            this.gc_PatientName.Visible = true;
            this.gc_PatientName.VisibleIndex = 4;
            this.gc_PatientName.Width = 150;
            // 
            // gc_GenderName
            // 
            this.gc_GenderName.Caption = "Giới tính";
            this.gc_GenderName.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gc_GenderName.Name = "gc_GenderName";
            this.gc_GenderName.OptionsColumn.AllowEdit = false;
            this.gc_GenderName.Visible = true;
            this.gc_GenderName.VisibleIndex = 5;
            this.gc_GenderName.Width = 50;
            // 
            // gc_PatientDob
            // 
            this.gc_PatientDob.Caption = "Ngày sinh";
            this.gc_PatientDob.FieldName = "DOB_STR";
            this.gc_PatientDob.Name = "gc_PatientDob";
            this.gc_PatientDob.OptionsColumn.AllowEdit = false;
            this.gc_PatientDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_PatientDob.Visible = true;
            this.gc_PatientDob.VisibleIndex = 6;
            this.gc_PatientDob.Width = 100;
            // 
            // gc_TotalPatientPrice
            // 
            this.gc_TotalPatientPrice.Caption = "Thu bệnh nhân";
            this.gc_TotalPatientPrice.FieldName = "TOTAL_PATIENT_PRICE";
            this.gc_TotalPatientPrice.Name = "gc_TotalPatientPrice";
            this.gc_TotalPatientPrice.OptionsColumn.AllowEdit = false;
            this.gc_TotalPatientPrice.Visible = true;
            this.gc_TotalPatientPrice.VisibleIndex = 7;
            this.gc_TotalPatientPrice.Width = 120;
            // 
            // gc_TotalDepositAmount
            // 
            this.gc_TotalDepositAmount.Caption = "Tạm thu";
            this.gc_TotalDepositAmount.FieldName = "TOTAL_DEPOSIT_AMOUNT";
            this.gc_TotalDepositAmount.Name = "gc_TotalDepositAmount";
            this.gc_TotalDepositAmount.OptionsColumn.AllowEdit = false;
            this.gc_TotalDepositAmount.Visible = true;
            this.gc_TotalDepositAmount.VisibleIndex = 8;
            this.gc_TotalDepositAmount.Width = 120;
            // 
            // gc_TotalBillAmount
            // 
            this.gc_TotalBillAmount.Caption = "Thanh toán";
            this.gc_TotalBillAmount.FieldName = "TOTAL_BILL_AMOUNT";
            this.gc_TotalBillAmount.Name = "gc_TotalBillAmount";
            this.gc_TotalBillAmount.OptionsColumn.AllowEdit = false;
            this.gc_TotalBillAmount.Visible = true;
            this.gc_TotalBillAmount.VisibleIndex = 9;
            this.gc_TotalBillAmount.Width = 120;
            // 
            // gc_TotalRepayAmount
            // 
            this.gc_TotalRepayAmount.Caption = "Hoàn ứng";
            this.gc_TotalRepayAmount.FieldName = "TOTAL_REPAY_AMOUNT";
            this.gc_TotalRepayAmount.Name = "gc_TotalRepayAmount";
            this.gc_TotalRepayAmount.OptionsColumn.AllowEdit = false;
            this.gc_TotalRepayAmount.Visible = true;
            this.gc_TotalRepayAmount.VisibleIndex = 10;
            this.gc_TotalRepayAmount.Width = 120;
            // 
            // gc_InTime
            // 
            this.gc_InTime.Caption = "Thời gian vào";
            this.gc_InTime.FieldName = "IN_TIME_STR";
            this.gc_InTime.Name = "gc_InTime";
            this.gc_InTime.OptionsColumn.AllowEdit = false;
            this.gc_InTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_InTime.Visible = true;
            this.gc_InTime.VisibleIndex = 11;
            this.gc_InTime.Width = 120;
            // 
            // gc_OutTime
            // 
            this.gc_OutTime.Caption = "Thời gian ra";
            this.gc_OutTime.FieldName = "OUT_TIME_STR";
            this.gc_OutTime.Name = "gc_OutTime";
            this.gc_OutTime.OptionsColumn.AllowEdit = false;
            this.gc_OutTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gc_OutTime.Visible = true;
            this.gc_OutTime.VisibleIndex = 12;
            this.gc_OutTime.Width = 120;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(768, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(110, 22);
            this.btnSearch.StyleController = this.layoutControl2;
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(2, 26);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPatientCode.Size = new System.Drawing.Size(87, 20);
            this.txtPatientCode.StyleController = this.layoutControl2;
            this.txtPatientCode.TabIndex = 10;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(93, 26);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(93, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl2;
            this.txtTreatmentCode.TabIndex = 9;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(190, 26);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyWord.Size = new System.Drawing.Size(160, 20);
            this.txtKeyWord.StyleController = this.layoutControl2;
            this.txtKeyWord.TabIndex = 8;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // cboEndType
            // 
            this.cboEndType.Location = new System.Drawing.Point(658, 26);
            this.cboEndType.Name = "cboEndType";
            this.cboEndType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboEndType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboEndType.Properties.NullText = "";
            this.cboEndType.Properties.View = this.gridLookUpEdit3View;
            this.cboEndType.Size = new System.Drawing.Size(106, 20);
            this.cboEndType.StyleController = this.layoutControl2;
            this.cboEndType.TabIndex = 7;
            this.cboEndType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboEndType_PreviewKeyDown);
            // 
            // gridLookUpEdit3View
            // 
            this.gridLookUpEdit3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit3View.Name = "gridLookUpEdit3View";
            this.gridLookUpEdit3View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit3View.OptionsView.ShowGroupPanel = false;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(449, 26);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridLookUpEdit2View;
            this.cboDepartment.Size = new System.Drawing.Size(110, 20);
            this.cboDepartment.StyleController = this.layoutControl2;
            this.cboDepartment.TabIndex = 6;
            this.cboDepartment.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
            this.cboDepartment.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboDepartment_PreviewKeyDown);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // dtOutDate
            // 
            this.dtOutDate.EditValue = null;
            this.dtOutDate.Location = new System.Drawing.Point(760, 2);
            this.dtOutDate.Name = "dtOutDate";
            this.dtOutDate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtOutDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtOutDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtOutDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtOutDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtOutDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtOutDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtOutDate.Size = new System.Drawing.Size(118, 20);
            this.dtOutDate.StyleController = this.layoutControl2;
            this.dtOutDate.TabIndex = 5;
            this.dtOutDate.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtOutDate_PreviewKeyDown);
            // 
            // dtInDate
            // 
            this.dtInDate.EditValue = null;
            this.dtInDate.Location = new System.Drawing.Point(548, 2);
            this.dtInDate.Name = "dtInDate";
            this.dtInDate.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtInDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInDate.Size = new System.Drawing.Size(113, 20);
            this.dtInDate.StyleController = this.layoutControl2;
            this.dtInDate.TabIndex = 4;
            this.dtInDate.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtInDate_PreviewKeyDown);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7,
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutControlItem19,
            this.emptySpaceItem1,
            this.lciDepartment,
            this.lciEndType,
            this.lciOutDate,
            this.lciInDate,
            this.layoutControlItem9,
            this.layoutControlItem8,
            this.lciLastDepositTimeFrom,
            this.lciLastDepositTimeTo});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(880, 532);
            this.Root.TextVisible = false;
            // 
            // lciInDate
            // 
            this.lciInDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciInDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciInDate.Control = this.dtInDate;
            this.lciInDate.Location = new System.Drawing.Point(451, 0);
            this.lciInDate.Name = "lciInDate";
            this.lciInDate.Size = new System.Drawing.Size(212, 24);
            this.lciInDate.Text = "Ngày vào:";
            this.lciInDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciInDate.TextSize = new System.Drawing.Size(90, 13);
            this.lciInDate.TextToControlDistance = 5;
            // 
            // lciOutDate
            // 
            this.lciOutDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciOutDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciOutDate.Control = this.dtOutDate;
            this.lciOutDate.Location = new System.Drawing.Point(663, 0);
            this.lciOutDate.Name = "lciOutDate";
            this.lciOutDate.Size = new System.Drawing.Size(217, 24);
            this.lciOutDate.Text = "Ngày ra:";
            this.lciOutDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciOutDate.TextSize = new System.Drawing.Size(90, 13);
            this.lciOutDate.TextToControlDistance = 5;
            // 
            // lciDepartment
            // 
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.cboDepartment;
            this.lciDepartment.Location = new System.Drawing.Point(352, 24);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Size = new System.Drawing.Size(209, 26);
            this.lciDepartment.Text = "Khoa khám:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(90, 13);
            this.lciDepartment.TextToControlDistance = 5;
            // 
            // lciEndType
            // 
            this.lciEndType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciEndType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciEndType.Control = this.cboEndType;
            this.lciEndType.Location = new System.Drawing.Point(561, 24);
            this.lciEndType.Name = "lciEndType";
            this.lciEndType.Size = new System.Drawing.Size(205, 26);
            this.lciEndType.Text = "Trạng thái:";
            this.lciEndType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciEndType.TextSize = new System.Drawing.Size(90, 13);
            this.lciEndType.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtKeyWord;
            this.layoutControlItem7.Location = new System.Drawing.Point(188, 24);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(164, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.txtTreatmentCode;
            this.layoutControlItem8.Location = new System.Drawing.Point(91, 24);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(97, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.txtPatientCode;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(91, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(441, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSearch;
            this.layoutControlItem10.Location = new System.Drawing.Point(766, 24);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(114, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.gridControlTreatment;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(880, 458);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.ucPaging1;
            this.layoutControlItem19.Location = new System.Drawing.Point(0, 508);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(880, 24);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(977, 2);
            this.cboAccountBook.MenuManager = this.barManager1;
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Properties.View = this.gridLookUpEdit1View;
            this.cboAccountBook.Size = new System.Drawing.Size(121, 20);
            this.cboAccountBook.StyleController = this.layoutControl1;
            this.cboAccountBook.TabIndex = 4;
            this.cboAccountBook.EditValueChanged += new System.EventHandler(this.cboAccountBook_EditValueChanged);
            this.cboAccountBook.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboAccountBook_PreviewKeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciAccountBook,
            this.layoutControlItem2,
            this.lciNumOrder,
            this.lciPayForm,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.lciTotalSelect,
            this.lciSuccess,
            this.lciError,
            this.emptySpaceItem2,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciAccountBook
            // 
            this.lciAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAccountBook.Control = this.cboAccountBook;
            this.lciAccountBook.Location = new System.Drawing.Point(880, 0);
            this.lciAccountBook.Name = "lciAccountBook";
            this.lciAccountBook.Size = new System.Drawing.Size(220, 24);
            this.lciAccountBook.Text = "Sổ thu chi:";
            this.lciAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAccountBook.TextSize = new System.Drawing.Size(90, 13);
            this.lciAccountBook.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(880, 532);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumOrder.Control = this.spNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(880, 24);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.Size = new System.Drawing.Size(220, 24);
            this.lciNumOrder.Text = "Số chứng từ:";
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(90, 13);
            this.lciNumOrder.TextToControlDistance = 5;
            // 
            // lciPayForm
            // 
            this.lciPayForm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPayForm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPayForm.Control = this.cboPayForm;
            this.lciPayForm.Location = new System.Drawing.Point(880, 48);
            this.lciPayForm.Name = "lciPayForm";
            this.lciPayForm.Size = new System.Drawing.Size(220, 24);
            this.lciPayForm.Text = "Hình thức:";
            this.lciPayForm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPayForm.TextSize = new System.Drawing.Size(90, 13);
            this.lciPayForm.TextToControlDistance = 5;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.btnCreateEBill;
            this.layoutControlItem14.Location = new System.Drawing.Point(880, 96);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(220, 26);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.separatorControl1;
            this.layoutControlItem15.Location = new System.Drawing.Point(880, 122);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // lciTotalSelect
            // 
            this.lciTotalSelect.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotalSelect.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotalSelect.Control = this.lblTotalSelect;
            this.lciTotalSelect.Location = new System.Drawing.Point(880, 146);
            this.lciTotalSelect.Name = "lciTotalSelect";
            this.lciTotalSelect.Size = new System.Drawing.Size(220, 24);
            this.lciTotalSelect.Text = "Tổng số hồ sơ:";
            this.lciTotalSelect.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTotalSelect.TextSize = new System.Drawing.Size(90, 20);
            this.lciTotalSelect.TextToControlDistance = 5;
            // 
            // lciSuccess
            // 
            this.lciSuccess.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSuccess.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSuccess.Control = this.lblSuccess;
            this.lciSuccess.Location = new System.Drawing.Point(880, 170);
            this.lciSuccess.Name = "lciSuccess";
            this.lciSuccess.Size = new System.Drawing.Size(220, 24);
            this.lciSuccess.Text = "Thành công:";
            this.lciSuccess.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSuccess.TextSize = new System.Drawing.Size(90, 20);
            this.lciSuccess.TextToControlDistance = 5;
            // 
            // lciError
            // 
            this.lciError.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciError.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciError.Control = this.lblError;
            this.lciError.Location = new System.Drawing.Point(880, 194);
            this.lciError.Name = "lciError";
            this.lciError.Size = new System.Drawing.Size(220, 24);
            this.lciError.Text = "Thất bại:";
            this.lciError.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciError.TextSize = new System.Drawing.Size(90, 20);
            this.lciError.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(880, 218);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(220, 314);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.chkIsSplitByCashierDeposit;
            this.layoutControlItem1.Location = new System.Drawing.Point(880, 72);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem1.Text = " ";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // dtLastDepositTimeFrom
            // 
            this.dtLastDepositTimeFrom.EditValue = null;
            this.dtLastDepositTimeFrom.Location = new System.Drawing.Point(97, 2);
            this.dtLastDepositTimeFrom.MenuManager = this.barManager1;
            this.dtLastDepositTimeFrom.Name = "dtLastDepositTimeFrom";
            this.dtLastDepositTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastDepositTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastDepositTimeFrom.Size = new System.Drawing.Size(123, 20);
            this.dtLastDepositTimeFrom.StyleController = this.layoutControl2;
            this.dtLastDepositTimeFrom.TabIndex = 14;
            // 
            // lciLastDepositTimeFrom
            // 
            this.lciLastDepositTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciLastDepositTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciLastDepositTimeFrom.Control = this.dtLastDepositTimeFrom;
            this.lciLastDepositTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.lciLastDepositTimeFrom.Name = "lciLastDepositTimeFrom";
            this.lciLastDepositTimeFrom.OptionsToolTip.ToolTip = "Ngày giao dịch từ";
            this.lciLastDepositTimeFrom.Size = new System.Drawing.Size(222, 24);
            this.lciLastDepositTimeFrom.Text = "Ngày GD từ:";
            this.lciLastDepositTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciLastDepositTimeFrom.TextSize = new System.Drawing.Size(90, 20);
            this.lciLastDepositTimeFrom.TextToControlDistance = 5;
            // 
            // dtLastDepositTimeTo
            // 
            this.dtLastDepositTimeTo.EditValue = null;
            this.dtLastDepositTimeTo.Location = new System.Drawing.Point(319, 2);
            this.dtLastDepositTimeTo.MenuManager = this.barManager1;
            this.dtLastDepositTimeTo.Name = "dtLastDepositTimeTo";
            this.dtLastDepositTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastDepositTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLastDepositTimeTo.Size = new System.Drawing.Size(120, 20);
            this.dtLastDepositTimeTo.StyleController = this.layoutControl2;
            this.dtLastDepositTimeTo.TabIndex = 15;
            // 
            // lciLastDepositTimeTo
            // 
            this.lciLastDepositTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciLastDepositTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciLastDepositTimeTo.Control = this.dtLastDepositTimeTo;
            this.lciLastDepositTimeTo.Location = new System.Drawing.Point(222, 0);
            this.lciLastDepositTimeTo.Name = "lciLastDepositTimeTo";
            this.lciLastDepositTimeTo.OptionsToolTip.ToolTip = "Ngày giao dịch đến";
            this.lciLastDepositTimeTo.Size = new System.Drawing.Size(219, 24);
            this.lciLastDepositTimeTo.Text = "Ngày GD đến:";
            this.lciLastDepositTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciLastDepositTimeTo.TextSize = new System.Drawing.Size(90, 20);
            this.lciLastDepositTimeTo.TextToControlDistance = 5;
            // 
            // FormEInvoiceCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "FormEInvoiceCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xuất hóa đơn điện tử";
            this.Load += new System.EventHandler(this.FormEInvoiceCreate_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIsSplitByCashierDeposit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit4View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtOutDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOutDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEndType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSuccess)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLastDepositTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLastDepositTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLastDepositTimeTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.GridLookUpEdit cboAccountBook;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraEditors.GridLookUpEdit cboEndType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit3View;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.DateEdit dtOutDate;
        private DevExpress.XtraEditors.DateEdit dtInDate;
        private DevExpress.XtraLayout.LayoutControlItem lciInDate;
        private DevExpress.XtraLayout.LayoutControlItem lciOutDate;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciEndType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.LabelControl lblSuccess;
        private DevExpress.XtraEditors.LabelControl lblTotalSelect;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.SimpleButton btnCreateEBill;
        private DevExpress.XtraEditors.GridLookUpEdit cboPayForm;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit4View;
        private DevExpress.XtraEditors.SpinEdit spNumOrder;
        private DevExpress.XtraGrid.GridControl gridControlTreatment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem lciPayForm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalSelect;
        private DevExpress.XtraLayout.LayoutControlItem lciSuccess;
        private DevExpress.XtraEditors.LabelControl lblError;
        private DevExpress.XtraLayout.LayoutControlItem lciError;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraGrid.Columns.GridColumn gc_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gc_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gc_PatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gc_GenderName;
        private DevExpress.XtraGrid.Columns.GridColumn gc_PatientDob;
        private DevExpress.XtraGrid.Columns.GridColumn gc_TotalPatientPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gc_TotalDepositAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gc_TotalBillAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gc_TotalRepayAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gc_InTime;
        private DevExpress.XtraGrid.Columns.GridColumn gc_OutTime;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarButtonItem barBtnSearch;
        private DevExpress.XtraBars.BarButtonItem barBtnExport;
        private DevExpress.XtraEditors.CheckEdit chkIsSplitByCashierDeposit;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.DateEdit dtLastDepositTimeTo;
        private DevExpress.XtraEditors.DateEdit dtLastDepositTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciLastDepositTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciLastDepositTimeTo;

    }
}
