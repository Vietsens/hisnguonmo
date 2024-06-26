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
namespace HIS.Desktop.Plugins.AllocationIndrectFee
{
    partial class frmAllocationIndrectFee
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
            this.gridControlExpenseType = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpenseType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Expense_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_PeriodCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_PeriodName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Department = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Modifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.spinTotalAmount = new DevExpress.XtraEditors.SpinEdit();
            this.lblPeriod = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpenseType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpenseType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTotalAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlExpenseType);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.spinTotalAmount);
            this.layoutControl1.Controls.Add(this.lblPeriod);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 433);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlExpenseType
            // 
            this.gridControlExpenseType.Location = new System.Drawing.Point(0, 26);
            this.gridControlExpenseType.MainView = this.gridViewExpenseType;
            this.gridControlExpenseType.Name = "gridControlExpenseType";
            this.gridControlExpenseType.Size = new System.Drawing.Size(880, 407);
            this.gridControlExpenseType.TabIndex = 7;
            this.gridControlExpenseType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpenseType});
            // 
            // gridViewExpenseType
            // 
            this.gridViewExpenseType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Expense_Stt,
            this.gridColumn_Expense_PeriodCode,
            this.gridColumn_Expense_PeriodName,
            this.gridColumn_Expense_ExpenseCode,
            this.gridColumn_Expense_ExpenseType,
            this.gridColumn_Expense_Department,
            this.gridColumn_Expense_price,
            this.gridColumn_Expense_CreateTime,
            this.gridColumn_Expense_Creator,
            this.gridColumn_Expense_ModifyTime,
            this.gridColumn_Expense_Modifier});
            this.gridViewExpenseType.GridControl = this.gridControlExpenseType;
            this.gridViewExpenseType.Name = "gridViewExpenseType";
            this.gridViewExpenseType.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpenseType.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpenseType.OptionsView.ShowGroupPanel = false;
            this.gridViewExpenseType.OptionsView.ShowIndicator = false;
            this.gridViewExpenseType.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpenseType_CustomUnboundColumnData);
            // 
            // gridColumn_Expense_Stt
            // 
            this.gridColumn_Expense_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Expense_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_Stt.Caption = "STT";
            this.gridColumn_Expense_Stt.FieldName = "STT";
            this.gridColumn_Expense_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_Stt.Name = "gridColumn_Expense_Stt";
            this.gridColumn_Expense_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Expense_Stt.Visible = true;
            this.gridColumn_Expense_Stt.VisibleIndex = 0;
            this.gridColumn_Expense_Stt.Width = 30;
            // 
            // gridColumn_Expense_PeriodCode
            // 
            this.gridColumn_Expense_PeriodCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_PeriodCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_PeriodCode.Caption = "Mã kỳ";
            this.gridColumn_Expense_PeriodCode.FieldName = "PERIOD_CODE";
            this.gridColumn_Expense_PeriodCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_PeriodCode.Name = "gridColumn_Expense_PeriodCode";
            this.gridColumn_Expense_PeriodCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_PeriodCode.Visible = true;
            this.gridColumn_Expense_PeriodCode.VisibleIndex = 1;
            this.gridColumn_Expense_PeriodCode.Width = 60;
            // 
            // gridColumn_Expense_PeriodName
            // 
            this.gridColumn_Expense_PeriodName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_PeriodName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_PeriodName.Caption = "Tên kỳ";
            this.gridColumn_Expense_PeriodName.FieldName = "PERIOD_NAME";
            this.gridColumn_Expense_PeriodName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_PeriodName.Name = "gridColumn_Expense_PeriodName";
            this.gridColumn_Expense_PeriodName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_PeriodName.Visible = true;
            this.gridColumn_Expense_PeriodName.VisibleIndex = 2;
            this.gridColumn_Expense_PeriodName.Width = 150;
            // 
            // gridColumn_Expense_ExpenseCode
            // 
            this.gridColumn_Expense_ExpenseCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseCode.Caption = "Mã phiếu";
            this.gridColumn_Expense_ExpenseCode.FieldName = "EXPENSE_CODE";
            this.gridColumn_Expense_ExpenseCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_ExpenseCode.Name = "gridColumn_Expense_ExpenseCode";
            this.gridColumn_Expense_ExpenseCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ExpenseCode.Visible = true;
            this.gridColumn_Expense_ExpenseCode.VisibleIndex = 3;
            this.gridColumn_Expense_ExpenseCode.Width = 80;
            // 
            // gridColumn_Expense_ExpenseType
            // 
            this.gridColumn_Expense_ExpenseType.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseType.Caption = "Loại";
            this.gridColumn_Expense_ExpenseType.FieldName = "EXPENSE_TYPE_NAME";
            this.gridColumn_Expense_ExpenseType.Name = "gridColumn_Expense_ExpenseType";
            this.gridColumn_Expense_ExpenseType.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ExpenseType.Visible = true;
            this.gridColumn_Expense_ExpenseType.VisibleIndex = 4;
            this.gridColumn_Expense_ExpenseType.Width = 250;
            // 
            // gridColumn_Expense_Department
            // 
            this.gridColumn_Expense_Department.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_Department.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_Department.Caption = "Khoa";
            this.gridColumn_Expense_Department.FieldName = "DEPARTMENT_NAME";
            this.gridColumn_Expense_Department.Name = "gridColumn_Expense_Department";
            this.gridColumn_Expense_Department.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_Department.Visible = true;
            this.gridColumn_Expense_Department.VisibleIndex = 5;
            this.gridColumn_Expense_Department.Width = 150;
            // 
            // gridColumn_Expense_price
            // 
            this.gridColumn_Expense_price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Expense_price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_price.Caption = "Số tiền";
            this.gridColumn_Expense_price.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Expense_price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Expense_price.FieldName = "PRICE";
            this.gridColumn_Expense_price.Name = "gridColumn_Expense_price";
            this.gridColumn_Expense_price.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_price.Visible = true;
            this.gridColumn_Expense_price.VisibleIndex = 6;
            this.gridColumn_Expense_price.Width = 100;
            // 
            // gridColumn_Expense_CreateTime
            // 
            this.gridColumn_Expense_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_CreateTime.Caption = "Thời gian tạo";
            this.gridColumn_Expense_CreateTime.FieldName = "CREATE_TIME_STR";
            this.gridColumn_Expense_CreateTime.Name = "gridColumn_Expense_CreateTime";
            this.gridColumn_Expense_CreateTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Expense_CreateTime.Visible = true;
            this.gridColumn_Expense_CreateTime.VisibleIndex = 7;
            this.gridColumn_Expense_CreateTime.Width = 120;
            // 
            // gridColumn_Expense_Creator
            // 
            this.gridColumn_Expense_Creator.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_Creator.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_Creator.Caption = "Người tạo";
            this.gridColumn_Expense_Creator.FieldName = "CREATOR";
            this.gridColumn_Expense_Creator.Name = "gridColumn_Expense_Creator";
            this.gridColumn_Expense_Creator.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_Creator.Visible = true;
            this.gridColumn_Expense_Creator.VisibleIndex = 8;
            // 
            // gridColumn_Expense_ModifyTime
            // 
            this.gridColumn_Expense_ModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_ModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ModifyTime.Caption = "Thời gian sửa";
            this.gridColumn_Expense_ModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gridColumn_Expense_ModifyTime.Name = "gridColumn_Expense_ModifyTime";
            this.gridColumn_Expense_ModifyTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Expense_ModifyTime.Visible = true;
            this.gridColumn_Expense_ModifyTime.VisibleIndex = 9;
            this.gridColumn_Expense_ModifyTime.Width = 120;
            // 
            // gridColumn_Expense_Modifier
            // 
            this.gridColumn_Expense_Modifier.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_Modifier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_Modifier.Caption = "Người sửa";
            this.gridColumn_Expense_Modifier.FieldName = "MODIFIER";
            this.gridColumn_Expense_Modifier.Name = "gridColumn_Expense_Modifier";
            this.gridColumn_Expense_Modifier.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_Modifier.Visible = true;
            this.gridColumn_Expense_Modifier.VisibleIndex = 10;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(772, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // spinTotalAmount
            // 
            this.spinTotalAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTotalAmount.Location = new System.Drawing.Point(537, 2);
            this.spinTotalAmount.Name = "spinTotalAmount";
            this.spinTotalAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTotalAmount.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinTotalAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinTotalAmount.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinTotalAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinTotalAmount.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spinTotalAmount.Size = new System.Drawing.Size(121, 20);
            this.spinTotalAmount.StyleController = this.layoutControl1;
            this.spinTotalAmount.TabIndex = 5;
            this.spinTotalAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTotalAmount_PreviewKeyDown);
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPeriod.Location = new System.Drawing.Point(97, 2);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(341, 20);
            this.lblPeriod.StyleController = this.layoutControl1;
            this.lblPeriod.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutPeriod,
            this.layoutTotalAmount,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 433);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutPeriod
            // 
            this.layoutPeriod.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPeriod.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPeriod.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPeriod.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPeriod.Control = this.lblPeriod;
            this.layoutPeriod.Location = new System.Drawing.Point(0, 0);
            this.layoutPeriod.Name = "layoutPeriod";
            this.layoutPeriod.Size = new System.Drawing.Size(440, 26);
            this.layoutPeriod.Text = "Kỳ:";
            this.layoutPeriod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPeriod.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPeriod.TextToControlDistance = 5;
            // 
            // layoutTotalAmount
            // 
            this.layoutTotalAmount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutTotalAmount.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutTotalAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalAmount.Control = this.spinTotalAmount;
            this.layoutTotalAmount.Location = new System.Drawing.Point(440, 0);
            this.layoutTotalAmount.Name = "layoutTotalAmount";
            this.layoutTotalAmount.Size = new System.Drawing.Size(220, 26);
            this.layoutTotalAmount.Text = "Tổng tiền:";
            this.layoutTotalAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalAmount.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTotalAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSave;
            this.layoutControlItem3.Location = new System.Drawing.Point(770, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlExpenseType;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(880, 407);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(660, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(110, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnRCSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCSave
            // 
            this.bbtnRCSave.Caption = "Lưu (Ctrl S)";
            this.bbtnRCSave.Id = 0;
            this.bbtnRCSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnRCSave.Name = "bbtnRCSave";
            this.bbtnRCSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(880, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 462);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 433);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 433);
            // 
            // frmAllocationIndrectFee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 462);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmAllocationIndrectFee";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phân bổ chi phí gián tiếp";
            this.Load += new System.EventHandler(this.frmAllocationIndrectFee_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpenseType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpenseType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTotalAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlExpenseType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpenseType;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SpinEdit spinTotalAmount;
        private DevExpress.XtraEditors.LabelControl lblPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_PeriodCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_PeriodName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseType;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Department;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_price;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Creator;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Modifier;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
