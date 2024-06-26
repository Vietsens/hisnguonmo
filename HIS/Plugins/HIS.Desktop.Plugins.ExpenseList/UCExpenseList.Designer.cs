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
namespace HIS.Desktop.Plugins.ExpenseList
{
    partial class UCExpenseList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCExpenseList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCreateList = new DevExpress.XtraEditors.SimpleButton();
            this.btnCreateType = new DevExpress.XtraEditors.SimpleButton();
            this.dtExpenseTime = new DevExpress.XtraEditors.DateEdit();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.txtPrice = new DevExpress.XtraEditors.SpinEdit();
            this.cboDepartment = new DevExpress.XtraEditors.LookUpEdit();
            this.txtDepartmentCode = new DevExpress.XtraEditors.TextEdit();
            this.cboExpenseType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtExpenseTypeCode = new DevExpress.XtraEditors.TextEdit();
            this.cboPeriod = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPeriodCode = new DevExpress.XtraEditors.TextEdit();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroupPeriod = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.cboFilterPeriod = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.cboFilterExpenseType = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer3 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.dtExpenseTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtExpenseTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutExpenseTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpenseTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer4 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.dtCreateTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtCreateTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutCreateTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutCreateTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupCreateTime = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupExpenseType = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupExpenseTime = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarSeparatorItem1 = new DevExpress.XtraNavBar.NavBarSeparatorItem();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.gridControlExpenseList = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpenseList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_ExpenseDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnDeleteExpense = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn_Expense_PeriodCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_PeriodName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_DepartmentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ExpenseTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_ModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Expense_Modifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnDeleteDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPeriod = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpenseType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnAdd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnEdit = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnRefresh = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpenseTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCreateType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnCreateList = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExpenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpenseTypeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.navBarGroupControlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterExpenseType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.navBarGroupControlContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTimeTo)).BeginInit();
            this.navBarGroupControlContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCreateTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCreateTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpenseList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpenseList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteExpense)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCreateType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCreateList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCreateList);
            this.layoutControl1.Controls.Add(this.btnCreateType);
            this.layoutControl1.Controls.Add(this.dtExpenseTime);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnEdit);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Controls.Add(this.txtPrice);
            this.layoutControl1.Controls.Add(this.cboDepartment);
            this.layoutControl1.Controls.Add(this.txtDepartmentCode);
            this.layoutControl1.Controls.Add(this.cboExpenseType);
            this.layoutControl1.Controls.Add(this.txtExpenseTypeCode);
            this.layoutControl1.Controls.Add(this.cboPeriod);
            this.layoutControl1.Controls.Add(this.txtPeriodCode);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Controls.Add(this.navBarControl1);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.gridControlExpenseList);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCreateList
            // 
            this.btnCreateList.Location = new System.Drawing.Point(1212, 26);
            this.btnCreateList.Name = "btnCreateList";
            this.btnCreateList.Size = new System.Drawing.Size(106, 22);
            this.btnCreateList.StyleController = this.layoutControl1;
            this.btnCreateList.TabIndex = 22;
            this.btnCreateList.Text = "Tạo nhanh";
            this.btnCreateList.Click += new System.EventHandler(this.btnCreateList_Click);
            // 
            // btnCreateType
            // 
            this.btnCreateType.Location = new System.Drawing.Point(1212, 576);
            this.btnCreateType.Name = "btnCreateType";
            this.btnCreateType.Size = new System.Drawing.Size(106, 22);
            this.btnCreateType.StyleController = this.layoutControl1;
            this.btnCreateType.TabIndex = 21;
            this.btnCreateType.Text = "Tạo khoản";
            this.btnCreateType.Click += new System.EventHandler(this.btnCreateType_Click);
            // 
            // dtExpenseTime
            // 
            this.dtExpenseTime.EditValue = null;
            this.dtExpenseTime.Location = new System.Drawing.Point(1197, 2);
            this.dtExpenseTime.Name = "dtExpenseTime";
            this.dtExpenseTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtExpenseTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtExpenseTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtExpenseTime.Size = new System.Drawing.Size(121, 20);
            this.dtExpenseTime.StyleController = this.layoutControl1;
            this.dtExpenseTime.TabIndex = 14;
            this.dtExpenseTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExpenseTime_Closed);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1102, 26);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(106, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 20;
            this.btnNew.Text = "Làm mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(992, 26);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(106, 22);
            this.btnEdit.StyleController = this.layoutControl1;
            this.btnEdit.TabIndex = 19;
            this.btnEdit.Text = "Sửa (Ctrl E)";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(882, 26);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 18;
            this.btnAdd.Text = "Thêm (Ctrl A)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtPrice
            // 
            this.txtPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtPrice.Location = new System.Drawing.Point(757, 26);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtPrice.Properties.DisplayFormat.FormatString = "#,##";
            this.txtPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtPrice.Properties.EditFormat.FormatString = "#,##";
            this.txtPrice.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtPrice.Size = new System.Drawing.Size(121, 20);
            this.txtPrice.StyleController = this.layoutControl1;
            this.txtPrice.TabIndex = 17;
            this.txtPrice.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPrice_PreviewKeyDown);
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(440, 26);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Size = new System.Drawing.Size(218, 20);
            this.cboDepartment.StyleController = this.layoutControl1;
            this.cboDepartment.TabIndex = 16;
            this.cboDepartment.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
            // 
            // txtDepartmentCode
            // 
            this.txtDepartmentCode.Location = new System.Drawing.Point(317, 26);
            this.txtDepartmentCode.Name = "txtDepartmentCode";
            this.txtDepartmentCode.Size = new System.Drawing.Size(123, 20);
            this.txtDepartmentCode.StyleController = this.layoutControl1;
            this.txtDepartmentCode.TabIndex = 15;
            this.txtDepartmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDepartmentCode_PreviewKeyDown);
            // 
            // cboExpenseType
            // 
            this.cboExpenseType.Location = new System.Drawing.Point(880, 2);
            this.cboExpenseType.Name = "cboExpenseType";
            this.cboExpenseType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboExpenseType.Properties.NullText = "";
            this.cboExpenseType.Size = new System.Drawing.Size(218, 20);
            this.cboExpenseType.StyleController = this.layoutControl1;
            this.cboExpenseType.TabIndex = 13;
            this.cboExpenseType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExpenseType_Closed);
            // 
            // txtExpenseTypeCode
            // 
            this.txtExpenseTypeCode.Location = new System.Drawing.Point(757, 2);
            this.txtExpenseTypeCode.Name = "txtExpenseTypeCode";
            this.txtExpenseTypeCode.Size = new System.Drawing.Size(123, 20);
            this.txtExpenseTypeCode.StyleController = this.layoutControl1;
            this.txtExpenseTypeCode.TabIndex = 12;
            this.txtExpenseTypeCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExpenseTypeCode_PreviewKeyDown);
            // 
            // cboPeriod
            // 
            this.cboPeriod.Location = new System.Drawing.Point(440, 2);
            this.cboPeriod.Name = "cboPeriod";
            this.cboPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPeriod.Properties.NullText = "";
            this.cboPeriod.Size = new System.Drawing.Size(218, 20);
            this.cboPeriod.StyleController = this.layoutControl1;
            this.cboPeriod.TabIndex = 11;
            this.cboPeriod.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPeriod_Closed);
            // 
            // txtPeriodCode
            // 
            this.txtPeriodCode.Location = new System.Drawing.Point(317, 2);
            this.txtPeriodCode.Name = "txtPeriodCode";
            this.txtPeriodCode.Size = new System.Drawing.Size(123, 20);
            this.txtPeriodCode.StyleController = this.layoutControl1;
            this.txtPeriodCode.TabIndex = 10;
            this.txtPeriodCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPeriodCode_PreviewKeyDown);
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(222, 551);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(1096, 21);
            this.ucPaging1.TabIndex = 9;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(112, 576);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(106, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(2, 576);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(106, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroupPeriod;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer2);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer3);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer4);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroupCreateTime,
            this.navBarGroupPeriod,
            this.navBarGroupExpenseType,
            this.navBarGroupExpenseTime});
            this.navBarControl1.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[] {
            this.navBarSeparatorItem1});
            this.navBarControl1.Location = new System.Drawing.Point(2, 26);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 216;
            this.navBarControl1.Size = new System.Drawing.Size(216, 546);
            this.navBarControl1.TabIndex = 6;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // navBarGroupPeriod
            // 
            this.navBarGroupPeriod.Caption = "Kỳ dữ liệu";
            this.navBarGroupPeriod.ControlContainer = this.navBarGroupControlContainer1;
            this.navBarGroupPeriod.Expanded = true;
            this.navBarGroupPeriod.GroupClientHeight = 60;
            this.navBarGroupPeriod.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupPeriod.Name = "navBarGroupPeriod";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.layoutControl2);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(208, 56);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.cboFilterPeriod);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(208, 56);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // cboFilterPeriod
            // 
            this.cboFilterPeriod.Location = new System.Drawing.Point(0, 12);
            this.cboFilterPeriod.Name = "cboFilterPeriod";
            this.cboFilterPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboFilterPeriod.Properties.NullText = "";
            this.cboFilterPeriod.Size = new System.Drawing.Size(208, 20);
            this.cboFilterPeriod.StyleController = this.layoutControl2;
            this.cboFilterPeriod.TabIndex = 4;
            this.cboFilterPeriod.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboFilterPeriod_Closed);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
            this.layoutControlGroup2.Size = new System.Drawing.Size(208, 56);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cboFilterPeriod;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(208, 36);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // navBarGroupControlContainer2
            // 
            this.navBarGroupControlContainer2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer2.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer2.Controls.Add(this.layoutControl3);
            this.navBarGroupControlContainer2.Name = "navBarGroupControlContainer2";
            this.navBarGroupControlContainer2.Size = new System.Drawing.Size(208, 56);
            this.navBarGroupControlContainer2.TabIndex = 1;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.cboFilterExpenseType);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(208, 56);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // cboFilterExpenseType
            // 
            this.cboFilterExpenseType.Location = new System.Drawing.Point(0, 12);
            this.cboFilterExpenseType.Name = "cboFilterExpenseType";
            this.cboFilterExpenseType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboFilterExpenseType.Properties.NullText = "";
            this.cboFilterExpenseType.Size = new System.Drawing.Size(208, 20);
            this.cboFilterExpenseType.StyleController = this.layoutControl3;
            this.cboFilterExpenseType.TabIndex = 4;
            this.cboFilterExpenseType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboFilterExpenseType_Closed);
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
            this.layoutControlGroup3.Size = new System.Drawing.Size(208, 56);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.cboFilterExpenseType;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(208, 36);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // navBarGroupControlContainer3
            // 
            this.navBarGroupControlContainer3.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer3.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer3.Controls.Add(this.layoutControl4);
            this.navBarGroupControlContainer3.Name = "navBarGroupControlContainer3";
            this.navBarGroupControlContainer3.Size = new System.Drawing.Size(208, 76);
            this.navBarGroupControlContainer3.TabIndex = 2;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.dtExpenseTimeTo);
            this.layoutControl4.Controls.Add(this.dtExpenseTimeFrom);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(0, 0);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup4;
            this.layoutControl4.Size = new System.Drawing.Size(208, 76);
            this.layoutControl4.TabIndex = 0;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // dtExpenseTimeTo
            // 
            this.dtExpenseTimeTo.EditValue = null;
            this.dtExpenseTimeTo.Location = new System.Drawing.Point(97, 36);
            this.dtExpenseTimeTo.Name = "dtExpenseTimeTo";
            this.dtExpenseTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtExpenseTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtExpenseTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtExpenseTimeTo.Size = new System.Drawing.Size(109, 20);
            this.dtExpenseTimeTo.StyleController = this.layoutControl4;
            this.dtExpenseTimeTo.TabIndex = 5;
            this.dtExpenseTimeTo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExpenseTimeTo_Closed);
            // 
            // dtExpenseTimeFrom
            // 
            this.dtExpenseTimeFrom.EditValue = null;
            this.dtExpenseTimeFrom.Location = new System.Drawing.Point(97, 12);
            this.dtExpenseTimeFrom.Name = "dtExpenseTimeFrom";
            this.dtExpenseTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpenseTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtExpenseTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtExpenseTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpenseTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtExpenseTimeFrom.Size = new System.Drawing.Size(109, 20);
            this.dtExpenseTimeFrom.StyleController = this.layoutControl4;
            this.dtExpenseTimeFrom.TabIndex = 4;
            this.dtExpenseTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExpenseTimeFrom_Closed);
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutExpenseTimeFrom,
            this.layoutExpenseTimeTo});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
            this.layoutControlGroup4.Size = new System.Drawing.Size(208, 76);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // layoutExpenseTimeFrom
            // 
            this.layoutExpenseTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpenseTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpenseTimeFrom.Control = this.dtExpenseTimeFrom;
            this.layoutExpenseTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.layoutExpenseTimeFrom.Name = "layoutExpenseTimeFrom";
            this.layoutExpenseTimeFrom.Size = new System.Drawing.Size(208, 24);
            this.layoutExpenseTimeFrom.Text = "Từ:";
            this.layoutExpenseTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpenseTimeFrom.TextSize = new System.Drawing.Size(90, 20);
            this.layoutExpenseTimeFrom.TextToControlDistance = 5;
            // 
            // layoutExpenseTimeTo
            // 
            this.layoutExpenseTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpenseTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpenseTimeTo.Control = this.dtExpenseTimeTo;
            this.layoutExpenseTimeTo.Location = new System.Drawing.Point(0, 24);
            this.layoutExpenseTimeTo.Name = "layoutExpenseTimeTo";
            this.layoutExpenseTimeTo.Size = new System.Drawing.Size(208, 32);
            this.layoutExpenseTimeTo.Text = "Đến:";
            this.layoutExpenseTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpenseTimeTo.TextSize = new System.Drawing.Size(90, 20);
            this.layoutExpenseTimeTo.TextToControlDistance = 5;
            // 
            // navBarGroupControlContainer4
            // 
            this.navBarGroupControlContainer4.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer4.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer4.Controls.Add(this.layoutControl5);
            this.navBarGroupControlContainer4.Name = "navBarGroupControlContainer4";
            this.navBarGroupControlContainer4.Size = new System.Drawing.Size(208, 71);
            this.navBarGroupControlContainer4.TabIndex = 3;
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.dtCreateTimeTo);
            this.layoutControl5.Controls.Add(this.dtCreateTimeFrom);
            this.layoutControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl5.Location = new System.Drawing.Point(0, 0);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroup5;
            this.layoutControl5.Size = new System.Drawing.Size(208, 71);
            this.layoutControl5.TabIndex = 0;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // dtCreateTimeTo
            // 
            this.dtCreateTimeTo.EditValue = null;
            this.dtCreateTimeTo.Location = new System.Drawing.Point(97, 36);
            this.dtCreateTimeTo.Name = "dtCreateTimeTo";
            this.dtCreateTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtCreateTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtCreateTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtCreateTimeTo.Size = new System.Drawing.Size(109, 20);
            this.dtCreateTimeTo.StyleController = this.layoutControl5;
            this.dtCreateTimeTo.TabIndex = 5;
            this.dtCreateTimeTo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtCreateTimeTo_Closed);
            // 
            // dtCreateTimeFrom
            // 
            this.dtCreateTimeFrom.EditValue = null;
            this.dtCreateTimeFrom.Location = new System.Drawing.Point(97, 12);
            this.dtCreateTimeFrom.Name = "dtCreateTimeFrom";
            this.dtCreateTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtCreateTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtCreateTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtCreateTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtCreateTimeFrom.Size = new System.Drawing.Size(109, 20);
            this.dtCreateTimeFrom.StyleController = this.layoutControl5;
            this.dtCreateTimeFrom.TabIndex = 4;
            this.dtCreateTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtCreateTimeFrom_Closed);
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutCreateTimeFrom,
            this.layoutCreateTimeTo});
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
            this.layoutControlGroup5.Size = new System.Drawing.Size(208, 71);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutCreateTimeFrom
            // 
            this.layoutCreateTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutCreateTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutCreateTimeFrom.Control = this.dtCreateTimeFrom;
            this.layoutCreateTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.layoutCreateTimeFrom.Name = "layoutCreateTimeFrom";
            this.layoutCreateTimeFrom.Size = new System.Drawing.Size(208, 24);
            this.layoutCreateTimeFrom.Text = "Từ:";
            this.layoutCreateTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutCreateTimeFrom.TextSize = new System.Drawing.Size(90, 20);
            this.layoutCreateTimeFrom.TextToControlDistance = 5;
            // 
            // layoutCreateTimeTo
            // 
            this.layoutCreateTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutCreateTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutCreateTimeTo.Control = this.dtCreateTimeTo;
            this.layoutCreateTimeTo.Location = new System.Drawing.Point(0, 24);
            this.layoutCreateTimeTo.Name = "layoutCreateTimeTo";
            this.layoutCreateTimeTo.Size = new System.Drawing.Size(208, 27);
            this.layoutCreateTimeTo.Text = "Đến:";
            this.layoutCreateTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutCreateTimeTo.TextSize = new System.Drawing.Size(90, 20);
            this.layoutCreateTimeTo.TextToControlDistance = 5;
            // 
            // navBarGroupCreateTime
            // 
            this.navBarGroupCreateTime.Caption = "Thời gian tạo";
            this.navBarGroupCreateTime.ControlContainer = this.navBarGroupControlContainer4;
            this.navBarGroupCreateTime.Expanded = true;
            this.navBarGroupCreateTime.GroupClientHeight = 75;
            this.navBarGroupCreateTime.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupCreateTime.Name = "navBarGroupCreateTime";
            // 
            // navBarGroupExpenseType
            // 
            this.navBarGroupExpenseType.Caption = "Loại chi";
            this.navBarGroupExpenseType.ControlContainer = this.navBarGroupControlContainer2;
            this.navBarGroupExpenseType.Expanded = true;
            this.navBarGroupExpenseType.GroupClientHeight = 60;
            this.navBarGroupExpenseType.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupExpenseType.Name = "navBarGroupExpenseType";
            // 
            // navBarGroupExpenseTime
            // 
            this.navBarGroupExpenseTime.Caption = "Thời gian chi";
            this.navBarGroupExpenseTime.ControlContainer = this.navBarGroupControlContainer3;
            this.navBarGroupExpenseTime.Expanded = true;
            this.navBarGroupExpenseTime.GroupClientHeight = 80;
            this.navBarGroupExpenseTime.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupExpenseTime.Name = "navBarGroupExpenseTime";
            // 
            // navBarSeparatorItem1
            // 
            this.navBarSeparatorItem1.CanDrag = false;
            this.navBarSeparatorItem1.Enabled = false;
            this.navBarSeparatorItem1.Hint = null;
            this.navBarSeparatorItem1.LargeImageIndex = 0;
            this.navBarSeparatorItem1.LargeImageSize = new System.Drawing.Size(0, 0);
            this.navBarSeparatorItem1.Name = "navBarSeparatorItem1";
            this.navBarSeparatorItem1.SmallImageIndex = 0;
            this.navBarSeparatorItem1.SmallImageSize = new System.Drawing.Size(0, 0);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(216, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 5;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // gridControlExpenseList
            // 
            this.gridControlExpenseList.Location = new System.Drawing.Point(222, 52);
            this.gridControlExpenseList.MainView = this.gridViewExpenseList;
            this.gridControlExpenseList.Name = "gridControlExpenseList";
            this.gridControlExpenseList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemBtnDeleteExpense,
            this.repositoryItemBtnDeleteDisable});
            this.gridControlExpenseList.Size = new System.Drawing.Size(1096, 495);
            this.gridControlExpenseList.TabIndex = 4;
            this.gridControlExpenseList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpenseList});
            // 
            // gridViewExpenseList
            // 
            this.gridViewExpenseList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridCol_ExpenseDelete,
            this.gridColumn_Expense_PeriodCode,
            this.gridColumn_Expense_PeriodName,
            this.gridColumn_Expense_ExpenseCode,
            this.gridColumn_Expense_ExpenseTypeCode,
            this.gridColumn_Expense_ExpenseTypeName,
            this.gridColumn_Expense_DepartmentName,
            this.gridColumn_Expense_ExpenseTime,
            this.gridColumn_Expense_Price,
            this.gridColumn_Expense_CreateTime,
            this.gridColumn_Expense_Creator,
            this.gridColumn_Expense_ModifyTime,
            this.gridColumn_Expense_Modifier});
            this.gridViewExpenseList.GridControl = this.gridControlExpenseList;
            this.gridViewExpenseList.Name = "gridViewExpenseList";
            this.gridViewExpenseList.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpenseList.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpenseList.OptionsView.ShowGroupPanel = false;
            this.gridViewExpenseList.OptionsView.ShowIndicator = false;
            this.gridViewExpenseList.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewExpenseList_RowClick);
            this.gridViewExpenseList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpenseList_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 35;
            // 
            // gridCol_ExpenseDelete
            // 
            this.gridCol_ExpenseDelete.Caption = "Xóa";
            this.gridCol_ExpenseDelete.ColumnEdit = this.repositoryItemBtnDeleteExpense;
            this.gridCol_ExpenseDelete.FieldName = "EDIT_EXPENSE";
            this.gridCol_ExpenseDelete.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridCol_ExpenseDelete.Name = "gridCol_ExpenseDelete";
            this.gridCol_ExpenseDelete.OptionsColumn.ShowCaption = false;
            this.gridCol_ExpenseDelete.Visible = true;
            this.gridCol_ExpenseDelete.VisibleIndex = 1;
            this.gridCol_ExpenseDelete.Width = 25;
            // 
            // repositoryItemBtnDeleteExpense
            // 
            this.repositoryItemBtnDeleteExpense.AutoHeight = false;
            this.repositoryItemBtnDeleteExpense.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnDeleteExpense.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.repositoryItemBtnDeleteExpense.Name = "repositoryItemBtnDeleteExpense";
            this.repositoryItemBtnDeleteExpense.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnDeleteExpense.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnDeleteExpense_ButtonClick);
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
            this.gridColumn_Expense_PeriodCode.VisibleIndex = 2;
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
            this.gridColumn_Expense_PeriodName.VisibleIndex = 3;
            this.gridColumn_Expense_PeriodName.Width = 120;
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
            this.gridColumn_Expense_ExpenseCode.VisibleIndex = 4;
            this.gridColumn_Expense_ExpenseCode.Width = 90;
            // 
            // gridColumn_Expense_ExpenseTypeCode
            // 
            this.gridColumn_Expense_ExpenseTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseTypeCode.Caption = "Mã loại";
            this.gridColumn_Expense_ExpenseTypeCode.FieldName = "EXPENSE_TYPE_CODE";
            this.gridColumn_Expense_ExpenseTypeCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_ExpenseTypeCode.Name = "gridColumn_Expense_ExpenseTypeCode";
            this.gridColumn_Expense_ExpenseTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ExpenseTypeCode.Visible = true;
            this.gridColumn_Expense_ExpenseTypeCode.VisibleIndex = 5;
            // 
            // gridColumn_Expense_ExpenseTypeName
            // 
            this.gridColumn_Expense_ExpenseTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseTypeName.Caption = "Tên loại";
            this.gridColumn_Expense_ExpenseTypeName.FieldName = "EXPENSE_TYPE_NAME";
            this.gridColumn_Expense_ExpenseTypeName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Expense_ExpenseTypeName.Name = "gridColumn_Expense_ExpenseTypeName";
            this.gridColumn_Expense_ExpenseTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ExpenseTypeName.Visible = true;
            this.gridColumn_Expense_ExpenseTypeName.VisibleIndex = 6;
            this.gridColumn_Expense_ExpenseTypeName.Width = 250;
            // 
            // gridColumn_Expense_DepartmentName
            // 
            this.gridColumn_Expense_DepartmentName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_DepartmentName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_DepartmentName.Caption = "Khoa";
            this.gridColumn_Expense_DepartmentName.FieldName = "DEPARTMENT_NAME";
            this.gridColumn_Expense_DepartmentName.Name = "gridColumn_Expense_DepartmentName";
            this.gridColumn_Expense_DepartmentName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_DepartmentName.Visible = true;
            this.gridColumn_Expense_DepartmentName.VisibleIndex = 7;
            this.gridColumn_Expense_DepartmentName.Width = 200;
            // 
            // gridColumn_Expense_ExpenseTime
            // 
            this.gridColumn_Expense_ExpenseTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_ExpenseTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_ExpenseTime.Caption = "Thời gian chi";
            this.gridColumn_Expense_ExpenseTime.FieldName = "EXPENSE_TIME_STR";
            this.gridColumn_Expense_ExpenseTime.Name = "gridColumn_Expense_ExpenseTime";
            this.gridColumn_Expense_ExpenseTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_ExpenseTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Expense_ExpenseTime.Visible = true;
            this.gridColumn_Expense_ExpenseTime.VisibleIndex = 9;
            this.gridColumn_Expense_ExpenseTime.Width = 120;
            // 
            // gridColumn_Expense_Price
            // 
            this.gridColumn_Expense_Price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Expense_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Expense_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Expense_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Expense_Price.Caption = "Tổng tiền";
            this.gridColumn_Expense_Price.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Expense_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Expense_Price.FieldName = "PRICE";
            this.gridColumn_Expense_Price.Name = "gridColumn_Expense_Price";
            this.gridColumn_Expense_Price.OptionsColumn.AllowEdit = false;
            this.gridColumn_Expense_Price.Visible = true;
            this.gridColumn_Expense_Price.VisibleIndex = 8;
            this.gridColumn_Expense_Price.Width = 90;
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
            this.gridColumn_Expense_CreateTime.VisibleIndex = 10;
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
            this.gridColumn_Expense_Creator.VisibleIndex = 11;
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
            this.gridColumn_Expense_ModifyTime.VisibleIndex = 12;
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
            this.gridColumn_Expense_Modifier.VisibleIndex = 13;
            // 
            // repositoryItemBtnDeleteDisable
            // 
            this.repositoryItemBtnDeleteDisable.AutoHeight = false;
            this.repositoryItemBtnDeleteDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnDeleteDisable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Xóa", null, null, true)});
            this.repositoryItemBtnDeleteDisable.Name = "repositoryItemBtnDeleteDisable";
            this.repositoryItemBtnDeleteDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutPeriod,
            this.layoutControlItem10,
            this.layoutExpenseType,
            this.layoutControlItem12,
            this.layoutDepartment,
            this.layoutControlItem11,
            this.layoutPrice,
            this.lciBtnAdd,
            this.lciBtnEdit,
            this.lciBtnRefresh,
            this.layoutExpenseTime,
            this.lciBtnCreateType,
            this.layoutControlItem8,
            this.emptySpaceItem2,
            this.lciBtnCreateList});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlExpenseList;
            this.layoutControlItem1.Location = new System.Drawing.Point(220, 50);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1100, 499);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtKeyword;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(240, 24);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(200, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.navBarControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(220, 550);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnFind;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 574);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnRefresh;
            this.layoutControlItem7.Location = new System.Drawing.Point(110, 574);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutPeriod
            // 
            this.layoutPeriod.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPeriod.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPeriod.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPeriod.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPeriod.Control = this.txtPeriodCode;
            this.layoutPeriod.Location = new System.Drawing.Point(220, 0);
            this.layoutPeriod.Name = "layoutPeriod";
            this.layoutPeriod.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutPeriod.Size = new System.Drawing.Size(220, 24);
            this.layoutPeriod.Text = "Kỳ dữ liệu:";
            this.layoutPeriod.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPeriod.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPeriod.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.cboPeriod;
            this.layoutControlItem10.Location = new System.Drawing.Point(440, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem10.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutExpenseType
            // 
            this.layoutExpenseType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutExpenseType.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutExpenseType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpenseType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpenseType.Control = this.txtExpenseTypeCode;
            this.layoutExpenseType.Location = new System.Drawing.Point(660, 0);
            this.layoutExpenseType.Name = "layoutExpenseType";
            this.layoutExpenseType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutExpenseType.Size = new System.Drawing.Size(220, 24);
            this.layoutExpenseType.Text = "Loại chi:";
            this.layoutExpenseType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpenseType.TextSize = new System.Drawing.Size(90, 20);
            this.layoutExpenseType.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.cboExpenseType;
            this.layoutControlItem12.Location = new System.Drawing.Point(880, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem12.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutDepartment
            // 
            this.layoutDepartment.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutDepartment.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDepartment.Control = this.txtDepartmentCode;
            this.layoutDepartment.Location = new System.Drawing.Point(220, 24);
            this.layoutDepartment.MaxSize = new System.Drawing.Size(230, 24);
            this.layoutDepartment.MinSize = new System.Drawing.Size(200, 24);
            this.layoutDepartment.Name = "layoutDepartment";
            this.layoutDepartment.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutDepartment.Size = new System.Drawing.Size(220, 26);
            this.layoutDepartment.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutDepartment.Text = "Khoa:";
            this.layoutDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDepartment.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDepartment.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.cboDepartment;
            this.layoutControlItem11.Location = new System.Drawing.Point(440, 24);
            this.layoutControlItem11.MaxSize = new System.Drawing.Size(230, 24);
            this.layoutControlItem11.MinSize = new System.Drawing.Size(200, 24);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem11.Size = new System.Drawing.Size(220, 26);
            this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutPrice
            // 
            this.layoutPrice.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPrice.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPrice.Control = this.txtPrice;
            this.layoutPrice.Location = new System.Drawing.Point(660, 24);
            this.layoutPrice.MaxSize = new System.Drawing.Size(240, 24);
            this.layoutPrice.MinSize = new System.Drawing.Size(220, 24);
            this.layoutPrice.Name = "layoutPrice";
            this.layoutPrice.Size = new System.Drawing.Size(220, 26);
            this.layoutPrice.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutPrice.Text = "Số tiền:";
            this.layoutPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPrice.TextToControlDistance = 5;
            // 
            // lciBtnAdd
            // 
            this.lciBtnAdd.Control = this.btnAdd;
            this.lciBtnAdd.Location = new System.Drawing.Point(880, 24);
            this.lciBtnAdd.Name = "lciBtnAdd";
            this.lciBtnAdd.Size = new System.Drawing.Size(110, 26);
            this.lciBtnAdd.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnAdd.TextVisible = false;
            // 
            // lciBtnEdit
            // 
            this.lciBtnEdit.Control = this.btnEdit;
            this.lciBtnEdit.Location = new System.Drawing.Point(990, 24);
            this.lciBtnEdit.Name = "lciBtnEdit";
            this.lciBtnEdit.Size = new System.Drawing.Size(110, 26);
            this.lciBtnEdit.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnEdit.TextVisible = false;
            // 
            // lciBtnRefresh
            // 
            this.lciBtnRefresh.Control = this.btnNew;
            this.lciBtnRefresh.Location = new System.Drawing.Point(1100, 24);
            this.lciBtnRefresh.Name = "lciBtnRefresh";
            this.lciBtnRefresh.Size = new System.Drawing.Size(110, 26);
            this.lciBtnRefresh.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnRefresh.TextVisible = false;
            // 
            // layoutExpenseTime
            // 
            this.layoutExpenseTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpenseTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpenseTime.Control = this.dtExpenseTime;
            this.layoutExpenseTime.Location = new System.Drawing.Point(1100, 0);
            this.layoutExpenseTime.Name = "layoutExpenseTime";
            this.layoutExpenseTime.Size = new System.Drawing.Size(220, 24);
            this.layoutExpenseTime.Text = "Thời gian chi:";
            this.layoutExpenseTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpenseTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutExpenseTime.TextToControlDistance = 5;
            // 
            // lciBtnCreateType
            // 
            this.lciBtnCreateType.Control = this.btnCreateType;
            this.lciBtnCreateType.Location = new System.Drawing.Point(1210, 574);
            this.lciBtnCreateType.Name = "lciBtnCreateType";
            this.lciBtnCreateType.Size = new System.Drawing.Size(110, 26);
            this.lciBtnCreateType.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCreateType.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.ucPaging1;
            this.layoutControlItem8.Location = new System.Drawing.Point(220, 549);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(1100, 25);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(220, 574);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(990, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnCreateList
            // 
            this.lciBtnCreateList.Control = this.btnCreateList;
            this.lciBtnCreateList.Location = new System.Drawing.Point(1210, 24);
            this.lciBtnCreateList.Name = "lciBtnCreateList";
            this.lciBtnCreateList.Size = new System.Drawing.Size(110, 26);
            this.lciBtnCreateList.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCreateList.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCExpenseList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCExpenseList";
            this.Size = new System.Drawing.Size(1320, 600);
            this.Load += new System.EventHandler(this.UCExpenseList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepartmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExpenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpenseTypeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.navBarGroupControlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboFilterExpenseType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.navBarGroupControlContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpenseTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTimeTo)).EndInit();
            this.navBarGroupControlContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCreateTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCreateTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpenseList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpenseList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteExpense)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnRefresh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpenseTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCreateType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCreateList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupPeriod;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraGrid.GridControl gridControlExpenseList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpenseList;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.LookUpEdit cboFilterPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.LookUpEdit cboFilterExpenseType;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupExpenseType;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer3;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraEditors.DateEdit dtExpenseTimeTo;
        private DevExpress.XtraEditors.DateEdit dtExpenseTimeFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpenseTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpenseTimeTo;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupExpenseTime;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_DepartmentName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ExpenseTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Price;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Creator;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_ModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_Modifier;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_ExpenseDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnDeleteExpense;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.LookUpEdit cboExpenseType;
        private DevExpress.XtraEditors.TextEdit txtExpenseTypeCode;
        private DevExpress.XtraEditors.LookUpEdit cboPeriod;
        private DevExpress.XtraEditors.TextEdit txtPeriodCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutPeriod;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpenseType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnEdit;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraEditors.SpinEdit txtPrice;
        private DevExpress.XtraEditors.LookUpEdit cboDepartment;
        private DevExpress.XtraEditors.TextEdit txtDepartmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutDepartment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutPrice;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnAdd;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnEdit;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnRefresh;
        private DevExpress.XtraEditors.DateEdit dtExpenseTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpenseTime;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupCreateTime;
        private DevExpress.XtraNavBar.NavBarSeparatorItem navBarSeparatorItem1;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer4;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeTo;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutCreateTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutCreateTimeTo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_PeriodCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Expense_PeriodName;
        private DevExpress.XtraEditors.SimpleButton btnCreateType;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCreateType;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnCreateList;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCreateList;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnDeleteDisable;
    }
}
