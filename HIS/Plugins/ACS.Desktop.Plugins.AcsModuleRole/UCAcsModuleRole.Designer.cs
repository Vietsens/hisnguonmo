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
using System.Windows.Forms;
namespace ACS.Desktop.Plugins.AcsModuleRole
{
    partial class UCAcsModuleRole : HIS.Desktop.Utility.UserControlBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCAcsModuleRole));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboApplication = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKeyword2 = new DevExpress.XtraEditors.TextEdit();
            this.txtKeyword1 = new DevExpress.XtraEditors.TextEdit();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl2 = new DevExpress.XtraGrid.GridControl();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grclcheck21 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grclcheck22 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.check22 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grclPatientTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grclNamePatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.check21 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.grViewModule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grclCheckForModule = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkForRole = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grclCheckForRole = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkForModule = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grclCodeStock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grclNameStock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.cboChoose = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnFind2 = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.ucPaging2 = new Inventec.UC.Paging.UcPaging();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.btnFind1 = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layout = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.imgStock = new DevExpress.Utils.ImageCollection();
            this.imgPatient = new DevExpress.Utils.ImageCollection();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboApplication.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.check22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.check21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grViewModule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkForRole)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkForModule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboChoose.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboApplication);
            this.layoutControl1.Controls.Add(this.txtKeyword2);
            this.layoutControl1.Controls.Add(this.txtKeyword1);
            this.layoutControl1.Controls.Add(this.layoutControl6);
            this.layoutControl1.Controls.Add(this.layoutControl4);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.btnFind2);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.ucPaging2);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.btnFind1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 22);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(990, 509);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboApplication
            // 
            this.cboApplication.Location = new System.Drawing.Point(257, 2);
            this.cboApplication.Name = "cboApplication";
            this.cboApplication.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboApplication.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboApplication.Properties.NullText = "";
            this.cboApplication.Properties.View = this.gridLookUpEdit1View;
            this.cboApplication.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboApplication_Properties_ButtonClick);
            this.cboApplication.Size = new System.Drawing.Size(144, 20);
            this.cboApplication.StyleController = this.layoutControl1;
            this.cboApplication.TabIndex = 23;
            this.cboApplication.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboApplication_Closed);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtKeyword2
            // 
            this.txtKeyword2.Location = new System.Drawing.Point(569, 2);
            this.txtKeyword2.Name = "txtKeyword2";
            this.txtKeyword2.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword2.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword2.Size = new System.Drawing.Size(187, 20);
            this.txtKeyword2.StyleController = this.layoutControl1;
            this.txtKeyword2.TabIndex = 22;
            this.txtKeyword2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword2_KeyDown);
            // 
            // txtKeyword1
            // 
            this.txtKeyword1.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword1.Name = "txtKeyword1";
            this.txtKeyword1.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
            this.txtKeyword1.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword1.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword1.Size = new System.Drawing.Size(196, 20);
            this.txtKeyword1.StyleController = this.layoutControl1;
            this.txtKeyword1.TabIndex = 21;
            this.txtKeyword1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword1_KeyDown);
            // 
            // layoutControl6
            // 
            this.layoutControl6.Location = new System.Drawing.Point(856, 2);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.Root = this.layoutControlGroup5;
            this.layoutControl6.Size = new System.Drawing.Size(132, 22);
            this.layoutControl6.TabIndex = 20;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Size = new System.Drawing.Size(132, 22);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.gridControl2);
            this.layoutControl4.Controls.Add(this.gridControl1);
            this.layoutControl4.Location = new System.Drawing.Point(2, 28);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(986, 429);
            this.layoutControl4.TabIndex = 19;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // gridControl2
            // 
            this.gridControl2.Location = new System.Drawing.Point(495, 2);
            this.gridControl2.MainView = this.gridView2;
            this.gridControl2.Name = "gridControl2";
            this.gridControl2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.check21,
            this.check22,
            this.repositoryItemCheckEdit2});
            this.gridControl2.Size = new System.Drawing.Size(489, 425);
            this.gridControl2.TabIndex = 5;
            this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            this.gridControl2.Click += new System.EventHandler(this.gridControl2_Click);
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grclcheck21,
            this.grclcheck22,
            this.grclPatientTypeCode,
            this.grclNamePatientType});
            this.gridView2.GridControl = this.gridControl2;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsView.ShowGroupPanel = false;
            this.gridView2.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView2_CustomRowCellEdit);
            this.gridView2.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView2_CustomUnboundColumnData);
            this.gridView2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView2_MouseDown);
            // 
            // grclcheck21
            // 
            this.grclcheck21.Caption = "gridColumn1";
            this.grclcheck21.ColumnEdit = this.repositoryItemCheckEdit2;
            this.grclcheck21.FieldName = "check21";
            this.grclcheck21.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.grclcheck21.Name = "grclcheck21";
            this.grclcheck21.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grclcheck21.OptionsColumn.ShowCaption = false;
            this.grclcheck21.OptionsFilter.AllowAutoFilter = false;
            this.grclcheck21.OptionsFilter.AllowFilter = false;
            this.grclcheck21.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclcheck21.Visible = true;
            this.grclcheck21.VisibleIndex = 1;
            this.grclcheck21.Width = 27;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            this.repositoryItemCheckEdit2.ReadOnly = true;
            // 
            // grclcheck22
            // 
            this.grclcheck22.Caption = "gridColumn2";
            this.grclcheck22.ColumnEdit = this.check22;
            this.grclcheck22.FieldName = "check22";
            this.grclcheck22.Name = "grclcheck22";
            this.grclcheck22.OptionsColumn.ShowCaption = false;
            this.grclcheck22.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclcheck22.Visible = true;
            this.grclcheck22.VisibleIndex = 0;
            this.grclcheck22.Width = 32;
            // 
            // check22
            // 
            this.check22.AutoHeight = false;
            this.check22.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.check22.Name = "check22";
            this.check22.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.check22.Click += new System.EventHandler(this.check22_Click);
            // 
            // grclPatientTypeCode
            // 
            this.grclPatientTypeCode.Caption = "Mã đối tượng";
            this.grclPatientTypeCode.FieldName = "ROLE_CODE";
            this.grclPatientTypeCode.Name = "grclPatientTypeCode";
            this.grclPatientTypeCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclPatientTypeCode.Visible = true;
            this.grclPatientTypeCode.VisibleIndex = 2;
            this.grclPatientTypeCode.Width = 134;
            // 
            // grclNamePatientType
            // 
            this.grclNamePatientType.Caption = "Đối tượng";
            this.grclNamePatientType.FieldName = "ROLE_NAME";
            this.grclNamePatientType.Name = "grclNamePatientType";
            this.grclNamePatientType.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclNamePatientType.Visible = true;
            this.grclNamePatientType.VisibleIndex = 3;
            this.grclNamePatientType.Width = 144;
            // 
            // check21
            // 
            this.check21.AutoHeight = false;
            this.check21.Name = "check21";
            this.check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
            this.check21.Click += new System.EventHandler(this.check21_Click);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.grViewModule;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.chkForRole,
            this.chkForModule,
            this.repositoryItemCheckEdit1});
            this.gridControl1.Size = new System.Drawing.Size(489, 425);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grViewModule});
            // 
            // grViewModule
            // 
            this.grViewModule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grclCheckForModule,
            this.grclCheckForRole,
            this.grclCodeStock,
            this.grclNameStock,
            this.gridColumn1});
            this.grViewModule.GridControl = this.gridControl1;
            this.grViewModule.Name = "grViewModule";
            this.grViewModule.OptionsView.ShowGroupPanel = false;
            this.grViewModule.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView1_CustomRowCellEdit);
            this.grViewModule.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            this.grViewModule.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridView1_MouseDown);
            // 
            // grclCheckForModule
            // 
            this.grclCheckForModule.ColumnEdit = this.chkForRole;
            this.grclCheckForModule.FieldName = "grclCheckForModule";
            this.grclCheckForModule.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.grclCheckForModule.Name = "grclCheckForModule";
            this.grclCheckForModule.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grclCheckForModule.OptionsColumn.ShowCaption = false;
            this.grclCheckForModule.OptionsFilter.AllowAutoFilter = false;
            this.grclCheckForModule.OptionsFilter.AllowFilter = false;
            this.grclCheckForModule.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclCheckForModule.Visible = true;
            this.grclCheckForModule.VisibleIndex = 1;
            this.grclCheckForModule.Width = 30;
            // 
            // chkForRole
            // 
            this.chkForRole.AutoHeight = false;
            this.chkForRole.Name = "chkForRole";
            this.chkForRole.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
            this.chkForRole.Click += new System.EventHandler(this.check11_Click);
            // 
            // grclCheckForRole
            // 
            this.grclCheckForRole.ColumnEdit = this.chkForModule;
            this.grclCheckForRole.FieldName = "grclCheckForRole";
            this.grclCheckForRole.Name = "grclCheckForRole";
            this.grclCheckForRole.OptionsColumn.ShowCaption = false;
            this.grclCheckForRole.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grclCheckForRole.Visible = true;
            this.grclCheckForRole.VisibleIndex = 0;
            this.grclCheckForRole.Width = 22;
            // 
            // chkForModule
            // 
            this.chkForModule.AutoHeight = false;
            this.chkForModule.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkForModule.Name = "chkForModule";
            this.chkForModule.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkForModule.Click += new System.EventHandler(this.check12_Click);
            // 
            // grclCodeStock
            // 
            this.grclCodeStock.Caption = "Tên chức năng";
            this.grclCodeStock.FieldName = "MODULE_NAME";
            this.grclCodeStock.Name = "grclCodeStock";
            this.grclCodeStock.OptionsColumn.ReadOnly = true;
            this.grclCodeStock.Visible = true;
            this.grclCodeStock.VisibleIndex = 2;
            this.grclCodeStock.Width = 144;
            // 
            // grclNameStock
            // 
            this.grclNameStock.Caption = "Link";
            this.grclNameStock.FieldName = "MODULE_LINK";
            this.grclNameStock.Name = "grclNameStock";
            this.grclNameStock.OptionsColumn.ReadOnly = true;
            this.grclNameStock.Visible = true;
            this.grclNameStock.VisibleIndex = 3;
            this.grclNameStock.Width = 152;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Ứng dụng";
            this.gridColumn1.FieldName = "APPLICATION_NAME";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 4;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem7});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(986, 429);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(493, 429);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.gridControl2;
            this.layoutControlItem7.Location = new System.Drawing.Point(493, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(493, 429);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.layoutControl5);
            this.layoutControl2.Controls.Add(this.cboChoose);
            this.layoutControl2.Location = new System.Drawing.Point(0, 483);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(883, 26);
            this.layoutControl2.TabIndex = 18;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControl5
            // 
            this.layoutControl5.Location = new System.Drawing.Point(2, 2);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroup4;
            this.layoutControl5.Size = new System.Drawing.Size(685, 22);
            this.layoutControl5.TabIndex = 5;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Size = new System.Drawing.Size(685, 22);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // cboChoose
            // 
            this.cboChoose.Location = new System.Drawing.Point(749, 2);
            this.cboChoose.Name = "cboChoose";
            this.cboChoose.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboChoose.Properties.Items.AddRange(new object[] {
            "Chức năng",
            "Vai trò"});
            this.cboChoose.Size = new System.Drawing.Size(132, 20);
            this.cboChoose.StyleController = this.layoutControl2;
            this.cboChoose.TabIndex = 4;
            this.cboChoose.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboChoose_Closed);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6,
            this.layoutControlItem11});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.OptionsItemText.TextToControlDistance = 4;
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(883, 26);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.cboChoose;
            this.layoutControlItem6.Location = new System.Drawing.Point(689, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(194, 26);
            this.layoutControlItem6.Text = "Chọn theo:";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(54, 13);
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.layoutControl5;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(689, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // btnFind2
            // 
            this.btnFind2.Location = new System.Drawing.Point(760, 2);
            this.btnFind2.Margin = new System.Windows.Forms.Padding(2);
            this.btnFind2.Name = "btnFind2";
            this.btnFind2.Size = new System.Drawing.Size(92, 22);
            this.btnFind2.StyleController = this.layoutControl1;
            this.btnFind2.TabIndex = 17;
            this.btnFind2.Text = "Tìm kiếm (Ctrl D)";
            this.btnFind2.Click += new System.EventHandler(this.btnFind2_Click);
            // 
            // layoutControl3
            // 
            this.layoutControl3.Location = new System.Drawing.Point(495, 2);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(70, 22);
            this.layoutControl3.TabIndex = 16;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup2.Size = new System.Drawing.Size(70, 22);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(885, 485);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(103, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ucPaging2
            // 
            this.ucPaging2.Location = new System.Drawing.Point(497, 461);
            this.ucPaging2.Margin = new System.Windows.Forms.Padding(2);
            this.ucPaging2.Name = "ucPaging2";
            this.ucPaging2.Size = new System.Drawing.Size(491, 20);
            this.ucPaging2.TabIndex = 12;
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 461);
            this.ucPaging1.Margin = new System.Windows.Forms.Padding(2);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(491, 20);
            this.ucPaging1.TabIndex = 11;
            // 
            // btnFind1
            // 
            this.btnFind1.Location = new System.Drawing.Point(405, 2);
            this.btnFind1.Margin = new System.Windows.Forms.Padding(2);
            this.btnFind1.Name = "btnFind1";
            this.btnFind1.Size = new System.Drawing.Size(86, 22);
            this.btnFind1.StyleController = this.layoutControl1;
            this.btnFind1.TabIndex = 8;
            this.btnFind1.Text = "Tìm kiếm (Ctrl F)";
            this.btnFind1.Click += new System.EventHandler(this.btnFind1_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem13,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem12,
            this.layoutControlItem14,
            this.layout,
            this.layoutControlItem15});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(990, 509);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnFind1;
            this.layoutControlItem5.Location = new System.Drawing.Point(403, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(90, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.ucPaging1;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 459);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(495, 24);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.ucPaging2;
            this.layoutControlItem9.Location = new System.Drawing.Point(495, 459);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(495, 24);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSave;
            this.layoutControlItem10.Location = new System.Drawing.Point(883, 483);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(107, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.layoutControl3;
            this.layoutControlItem13.Location = new System.Drawing.Point(493, 0);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(74, 26);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnFind2;
            this.layoutControlItem1.Location = new System.Drawing.Point(758, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(96, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl2;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 483);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(883, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.layoutControl4;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(990, 433);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.layoutControl6;
            this.layoutControlItem12.Location = new System.Drawing.Point(854, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(136, 26);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.txtKeyword1;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(200, 26);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layout
            // 
            this.layout.Control = this.txtKeyword2;
            this.layout.Location = new System.Drawing.Point(567, 0);
            this.layout.Name = "layout";
            this.layout.Size = new System.Drawing.Size(191, 26);
            this.layout.TextSize = new System.Drawing.Size(0, 0);
            this.layout.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.cboApplication;
            this.layoutControlItem15.Location = new System.Drawing.Point(200, 0);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(203, 26);
            this.layoutControlItem15.Text = "Ứng dụng:";
            this.layoutControlItem15.TextSize = new System.Drawing.Size(51, 13);
            // 
            // imgStock
            // 
            this.imgStock.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgStock.ImageStream")));
            this.imgStock.Images.SetKeyName(0, "checkbox.png");
            this.imgStock.Images.SetKeyName(1, "uncheckbox.png");
            // 
            // imgPatient
            // 
            this.imgPatient.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imgPatient.ImageStream")));
            this.imgPatient.Images.SetKeyName(0, "checkbox.png");
            this.imgPatient.Images.SetKeyName(1, "uncheckbox.png");
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 3;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            this.bar2.Visible = false;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "ctrl F";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "ctrl D";
            this.barButtonItem2.Id = 1;
            this.barButtonItem2.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "ctrl S";
            this.barButtonItem3.Id = 2;
            this.barButtonItem3.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem3.Name = "barButtonItem3";
            this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(990, 22);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 531);
            this.barDockControlBottom.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 509);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(990, 22);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 509);
            // 
            // UCAcsModuleRole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCAcsModuleRole";
            this.Size = new System.Drawing.Size(990, 531);
            this.Load += new System.EventHandler(this.UCAcsModuelRole_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboApplication.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.check22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.check21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grViewModule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkForRole)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkForModule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboChoose.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnFind1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private Inventec.UC.Paging.UcPaging ucPaging2;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton btnFind2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraGrid.GridControl gridControl2;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView grViewModule;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkForRole;
        private DevExpress.XtraGrid.Columns.GridColumn grclCheckForRole;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkForModule;
        private DevExpress.XtraGrid.Columns.GridColumn grclCodeStock;
        private DevExpress.XtraGrid.Columns.GridColumn grclNameStock;
        private DevExpress.XtraGrid.Columns.GridColumn grclcheck21;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit check21;
        private DevExpress.XtraGrid.Columns.GridColumn grclcheck22;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit check22;
        private DevExpress.XtraGrid.Columns.GridColumn grclPatientTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn grclNamePatientType;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraEditors.ComboBoxEdit cboChoose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControl layoutControl6;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn grclCheckForModule;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.Utils.ImageCollection imgStock;
        private DevExpress.Utils.ImageCollection imgPatient;
        private DevExpress.XtraEditors.TextEdit txtKeyword2;
        private DevExpress.XtraEditors.TextEdit txtKeyword1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layout;
        private DevExpress.XtraEditors.GridLookUpEdit cboApplication;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
    }
}
