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
namespace HIS.Desktop.Plugins.ExpenseByPeriod
{
    partial class UCExpenseByPeriod
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeListExpenseSdo = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_ExpenseTypeName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_ExpenseCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_Department = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_Price = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_ExpenseTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_CreateTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_Creator = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_ModifyTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn_ExpenseSdo_Modifier = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnShow = new DevExpress.XtraEditors.SimpleButton();
            this.cboPeriod = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPeriodCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.radioClose = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.radioOpen = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListExpenseSdo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioClose.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioOpen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.radioOpen);
            this.layoutControl1.Controls.Add(this.radioClose);
            this.layoutControl1.Controls.Add(this.treeListExpenseSdo);
            this.layoutControl1.Controls.Add(this.btnShow);
            this.layoutControl1.Controls.Add(this.cboPeriod);
            this.layoutControl1.Controls.Add(this.txtPeriodCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 670);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeListExpenseSdo
            // 
            this.treeListExpenseSdo.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.treeListExpenseSdo.Appearance.FocusedCell.Options.UseBackColor = true;
            this.treeListExpenseSdo.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.treeListExpenseSdo.Appearance.FocusedRow.Options.UseBackColor = true;
            this.treeListExpenseSdo.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.treeListExpenseSdo.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.treeListExpenseSdo.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode,
            this.treeListColumn_ExpenseSdo_ExpenseTypeName,
            this.treeListColumn_ExpenseSdo_ExpenseCode,
            this.treeListColumn_ExpenseSdo_Department,
            this.treeListColumn_ExpenseSdo_Price,
            this.treeListColumn_ExpenseSdo_ExpenseTime,
            this.treeListColumn_ExpenseSdo_CreateTime,
            this.treeListColumn_ExpenseSdo_Creator,
            this.treeListColumn_ExpenseSdo_ModifyTime,
            this.treeListColumn_ExpenseSdo_Modifier});
            this.treeListExpenseSdo.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListExpenseSdo.KeyFieldName = "ExpenseTypeId";
            this.treeListExpenseSdo.Location = new System.Drawing.Point(0, 26);
            this.treeListExpenseSdo.Name = "treeListExpenseSdo";
            this.treeListExpenseSdo.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.treeListExpenseSdo.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.treeListExpenseSdo.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeListExpenseSdo.OptionsBehavior.AutoPopulateColumns = false;
            this.treeListExpenseSdo.OptionsBehavior.EnableFiltering = true;
            this.treeListExpenseSdo.OptionsBehavior.PopulateServiceColumns = true;
            this.treeListExpenseSdo.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.treeListExpenseSdo.OptionsView.AutoWidth = false;
            this.treeListExpenseSdo.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.treeListExpenseSdo.OptionsView.ShowHorzLines = false;
            this.treeListExpenseSdo.OptionsView.ShowIndicator = false;
            this.treeListExpenseSdo.OptionsView.ShowVertLines = false;
            this.treeListExpenseSdo.ParentFieldName = "ParentId";
            this.treeListExpenseSdo.Size = new System.Drawing.Size(1320, 644);
            this.treeListExpenseSdo.TabIndex = 7;
            this.treeListExpenseSdo.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListExpenseSdo_NodeCellStyle);
            this.treeListExpenseSdo.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListExpenseSdo_CustomUnboundColumnData);
            // 
            // treeListColumn_ExpenseSdo_ExpenseTypeCode
            // 
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Caption = "Mã loại";
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.FieldName = "EXPENSE_TYPE_CODE";
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Name = "treeListColumn_ExpenseSdo_ExpenseTypeCode";
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Visible = true;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.VisibleIndex = 0;
            this.treeListColumn_ExpenseSdo_ExpenseTypeCode.Width = 100;
            // 
            // treeListColumn_ExpenseSdo_ExpenseTypeName
            // 
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.Caption = "Tên loại";
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.FieldName = "EXPENSE_TYPE_NAME";
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.Name = "treeListColumn_ExpenseSdo_ExpenseTypeName";
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.Visible = true;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.VisibleIndex = 1;
            this.treeListColumn_ExpenseSdo_ExpenseTypeName.Width = 350;
            // 
            // treeListColumn_ExpenseSdo_ExpenseCode
            // 
            this.treeListColumn_ExpenseSdo_ExpenseCode.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ExpenseCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ExpenseCode.Caption = "Mã phiếu";
            this.treeListColumn_ExpenseSdo_ExpenseCode.FieldName = "EXPENSE_CODE";
            this.treeListColumn_ExpenseSdo_ExpenseCode.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListColumn_ExpenseSdo_ExpenseCode.Name = "treeListColumn_ExpenseSdo_ExpenseCode";
            this.treeListColumn_ExpenseSdo_ExpenseCode.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_ExpenseCode.Visible = true;
            this.treeListColumn_ExpenseSdo_ExpenseCode.VisibleIndex = 2;
            this.treeListColumn_ExpenseSdo_ExpenseCode.Width = 100;
            // 
            // treeListColumn_ExpenseSdo_Department
            // 
            this.treeListColumn_ExpenseSdo_Department.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_Department.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_Department.Caption = "Khoa";
            this.treeListColumn_ExpenseSdo_Department.FieldName = "DEPARTMENT_NAME";
            this.treeListColumn_ExpenseSdo_Department.Name = "treeListColumn_ExpenseSdo_Department";
            this.treeListColumn_ExpenseSdo_Department.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_Department.Visible = true;
            this.treeListColumn_ExpenseSdo_Department.VisibleIndex = 3;
            this.treeListColumn_ExpenseSdo_Department.Width = 300;
            // 
            // treeListColumn_ExpenseSdo_Price
            // 
            this.treeListColumn_ExpenseSdo_Price.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn_ExpenseSdo_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_Price.Caption = "Số tiền";
            this.treeListColumn_ExpenseSdo_Price.FieldName = "PRICE";
            this.treeListColumn_ExpenseSdo_Price.Format.FormatString = "#,##0.0000";
            this.treeListColumn_ExpenseSdo_Price.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn_ExpenseSdo_Price.Name = "treeListColumn_ExpenseSdo_Price";
            this.treeListColumn_ExpenseSdo_Price.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_Price.Visible = true;
            this.treeListColumn_ExpenseSdo_Price.VisibleIndex = 4;
            this.treeListColumn_ExpenseSdo_Price.Width = 120;
            // 
            // treeListColumn_ExpenseSdo_ExpenseTime
            // 
            this.treeListColumn_ExpenseSdo_ExpenseTime.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ExpenseTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ExpenseTime.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ExpenseTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ExpenseTime.Caption = "Thời gian chi";
            this.treeListColumn_ExpenseSdo_ExpenseTime.FieldName = "EXPENSE_TIME_STR";
            this.treeListColumn_ExpenseSdo_ExpenseTime.Name = "treeListColumn_ExpenseSdo_ExpenseTime";
            this.treeListColumn_ExpenseSdo_ExpenseTime.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_ExpenseTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn_ExpenseSdo_ExpenseTime.Visible = true;
            this.treeListColumn_ExpenseSdo_ExpenseTime.VisibleIndex = 5;
            this.treeListColumn_ExpenseSdo_ExpenseTime.Width = 120;
            // 
            // treeListColumn_ExpenseSdo_CreateTime
            // 
            this.treeListColumn_ExpenseSdo_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_CreateTime.Caption = "Thời gian tạo";
            this.treeListColumn_ExpenseSdo_CreateTime.FieldName = "CREATE_TIME_STR";
            this.treeListColumn_ExpenseSdo_CreateTime.Name = "treeListColumn_ExpenseSdo_CreateTime";
            this.treeListColumn_ExpenseSdo_CreateTime.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_CreateTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn_ExpenseSdo_CreateTime.Visible = true;
            this.treeListColumn_ExpenseSdo_CreateTime.VisibleIndex = 6;
            this.treeListColumn_ExpenseSdo_CreateTime.Width = 120;
            // 
            // treeListColumn_ExpenseSdo_Creator
            // 
            this.treeListColumn_ExpenseSdo_Creator.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_Creator.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_Creator.Caption = "Người tạo";
            this.treeListColumn_ExpenseSdo_Creator.FieldName = "CREATOR";
            this.treeListColumn_ExpenseSdo_Creator.Name = "treeListColumn_ExpenseSdo_Creator";
            this.treeListColumn_ExpenseSdo_Creator.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_Creator.Visible = true;
            this.treeListColumn_ExpenseSdo_Creator.VisibleIndex = 7;
            // 
            // treeListColumn_ExpenseSdo_ModifyTime
            // 
            this.treeListColumn_ExpenseSdo_ModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_ModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_ModifyTime.Caption = "Thời gián sửa";
            this.treeListColumn_ExpenseSdo_ModifyTime.FieldName = "MODIFY_TIME_STR";
            this.treeListColumn_ExpenseSdo_ModifyTime.Name = "treeListColumn_ExpenseSdo_ModifyTime";
            this.treeListColumn_ExpenseSdo_ModifyTime.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_ModifyTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn_ExpenseSdo_ModifyTime.Visible = true;
            this.treeListColumn_ExpenseSdo_ModifyTime.VisibleIndex = 8;
            this.treeListColumn_ExpenseSdo_ModifyTime.Width = 120;
            // 
            // treeListColumn_ExpenseSdo_Modifier
            // 
            this.treeListColumn_ExpenseSdo_Modifier.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn_ExpenseSdo_Modifier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn_ExpenseSdo_Modifier.Caption = "Người sửa";
            this.treeListColumn_ExpenseSdo_Modifier.FieldName = "MODIFIER";
            this.treeListColumn_ExpenseSdo_Modifier.Name = "treeListColumn_ExpenseSdo_Modifier";
            this.treeListColumn_ExpenseSdo_Modifier.OptionsColumn.AllowEdit = false;
            this.treeListColumn_ExpenseSdo_Modifier.Visible = true;
            this.treeListColumn_ExpenseSdo_Modifier.VisibleIndex = 9;
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point(1212, 2);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(106, 22);
            this.btnShow.StyleController = this.layoutControl1;
            this.btnShow.TabIndex = 6;
            this.btnShow.Text = "Xem (Ctrl S)";
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // cboPeriod
            // 
            this.cboPeriod.Location = new System.Drawing.Point(160, 2);
            this.cboPeriod.Name = "cboPeriod";
            this.cboPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPeriod.Properties.NullText = "";
            this.cboPeriod.Size = new System.Drawing.Size(278, 20);
            this.cboPeriod.StyleController = this.layoutControl1;
            this.cboPeriod.TabIndex = 5;
            this.cboPeriod.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPeriod_Closed);
            this.cboPeriod.EditValueChanged += new System.EventHandler(this.cboPeriod_EditValueChanged);
            // 
            // txtPeriodCode
            // 
            this.txtPeriodCode.Location = new System.Drawing.Point(97, 2);
            this.txtPeriodCode.Name = "txtPeriodCode";
            this.txtPeriodCode.Size = new System.Drawing.Size(63, 20);
            this.txtPeriodCode.StyleController = this.layoutControl1;
            this.txtPeriodCode.TabIndex = 4;
            this.txtPeriodCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPeriodCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutPeriod,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 670);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutPeriod
            // 
            this.layoutPeriod.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPeriod.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPeriod.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPeriod.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPeriod.Control = this.txtPeriodCode;
            this.layoutPeriod.Location = new System.Drawing.Point(0, 0);
            this.layoutPeriod.Name = "layoutPeriod";
            this.layoutPeriod.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutPeriod.Size = new System.Drawing.Size(160, 26);
            this.layoutPeriod.Text = "Kỳ tài chính:";
            this.layoutPeriod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPeriod.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPeriod.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboPeriod;
            this.layoutControlItem2.Location = new System.Drawing.Point(160, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(280, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnShow;
            this.layoutControlItem3.Location = new System.Drawing.Point(1210, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.treeListExpenseSdo;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(1320, 644);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(440, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(385, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // radioClose
            // 
            this.radioClose.EditValue = true;
            this.radioClose.Location = new System.Drawing.Point(827, 2);
            this.radioClose.Name = "radioClose";
            this.radioClose.Properties.Caption = "Đóng";
            this.radioClose.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioClose.Properties.RadioGroupIndex = 1;
            this.radioClose.Size = new System.Drawing.Size(188, 19);
            this.radioClose.StyleController = this.layoutControl1;
            this.radioClose.TabIndex = 8;
            this.radioClose.CheckedChanged += new System.EventHandler(this.radioClose_CheckedChanged);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.radioClose;
            this.layoutControlItem1.Location = new System.Drawing.Point(825, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(192, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // radioOpen
            // 
            this.radioOpen.Location = new System.Drawing.Point(1019, 2);
            this.radioOpen.Name = "radioOpen";
            this.radioOpen.Properties.Caption = "Mở";
            this.radioOpen.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioOpen.Properties.RadioGroupIndex = 1;
            this.radioOpen.Size = new System.Drawing.Size(189, 19);
            this.radioOpen.StyleController = this.layoutControl1;
            this.radioOpen.TabIndex = 9;
            this.radioOpen.TabStop = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.radioOpen;
            this.layoutControlItem5.Location = new System.Drawing.Point(1017, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(193, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // UCExpenseByPeriod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCExpenseByPeriod";
            this.Size = new System.Drawing.Size(1320, 670);
            this.Load += new System.EventHandler(this.UCExpenseByPeriod_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListExpenseSdo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioClose.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioOpen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraTreeList.TreeList treeListExpenseSdo;
        private DevExpress.XtraEditors.SimpleButton btnShow;
        private DevExpress.XtraEditors.LookUpEdit cboPeriod;
        private DevExpress.XtraEditors.TextEdit txtPeriodCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_ExpenseTypeCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_ExpenseTypeName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_ExpenseCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_Department;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_Price;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_ExpenseTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_CreateTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_Creator;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_ModifyTime;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn_ExpenseSdo_Modifier;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.CheckEdit radioOpen;
        private DevExpress.XtraEditors.CheckEdit radioClose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}
