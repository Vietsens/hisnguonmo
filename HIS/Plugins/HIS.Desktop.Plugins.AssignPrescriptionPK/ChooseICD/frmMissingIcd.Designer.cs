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
namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD
{
    partial class frmMissingIcd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMissingIcd));
            this.gridControlMissingIcd = new DevExpress.XtraGrid.GridControl();
            this.gridViewMissingIcd = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIcdName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIsMainIcd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditMainICD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumnIsCauseIcd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditICDCause = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.barManager2 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.gridControlRemoveIcd = new DevExpress.XtraGrid.GridControl();
            this.gridViewRemoveIcd = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditRemoveMainICD = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckEditRemoveICDCause = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnAddIcd = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnSkip = new DevExpress.XtraEditors.SimpleButton();
            this.lciSkip = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMissingIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMissingIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditMainICD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditICDCause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRemoveIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRemoveIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditRemoveMainICD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditRemoveICDCause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControlMissingIcd
            // 
            this.gridControlMissingIcd.Location = new System.Drawing.Point(2, 52);
            this.gridControlMissingIcd.MainView = this.gridViewMissingIcd;
            this.gridControlMissingIcd.Name = "gridControlMissingIcd";
            this.gridControlMissingIcd.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.CheckEditMainICD,
            this.CheckEditICDCause});
            this.gridControlMissingIcd.Size = new System.Drawing.Size(636, 218);
            this.gridControlMissingIcd.TabIndex = 29;
            this.gridControlMissingIcd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMissingIcd});
            // 
            // gridViewMissingIcd
            // 
            this.gridViewMissingIcd.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnServiceName,
            this.gridColumnIcdName,
            this.gridColumnIsMainIcd,
            this.gridColumnIsCauseIcd});
            this.gridViewMissingIcd.GridControl = this.gridControlMissingIcd;
            this.gridViewMissingIcd.Name = "gridViewMissingIcd";
            this.gridViewMissingIcd.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewMissingIcd.OptionsView.ShowGroupPanel = false;
            this.gridViewMissingIcd.OptionsView.ShowIndicator = false;
            this.gridViewMissingIcd.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMissingIcd_CustomRowCellEdit);
            this.gridViewMissingIcd.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMissingIcd_CellValueChanged);
            this.gridViewMissingIcd.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMissingIcd_CustomUnboundColumnData);
            this.gridViewMissingIcd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewMissingIcd_MouseDown);
            // 
            // gridColumnServiceName
            // 
            this.gridColumnServiceName.Caption = "Dịch vụ";
            this.gridColumnServiceName.FieldName = "SERVICE_NAME";
            this.gridColumnServiceName.Name = "gridColumnServiceName";
            this.gridColumnServiceName.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceName.Visible = true;
            this.gridColumnServiceName.VisibleIndex = 0;
            this.gridColumnServiceName.Width = 200;
            // 
            // gridColumnIcdName
            // 
            this.gridColumnIcdName.Caption = "Chẩn đoán (ICD)";
            this.gridColumnIcdName.FieldName = "ICD_NAME";
            this.gridColumnIcdName.Name = "gridColumnIcdName";
            this.gridColumnIcdName.OptionsColumn.AllowEdit = false;
            this.gridColumnIcdName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnIcdName.Visible = true;
            this.gridColumnIcdName.VisibleIndex = 1;
            this.gridColumnIcdName.Width = 302;
            // 
            // gridColumnIsMainIcd
            // 
            this.gridColumnIsMainIcd.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsMainIcd.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsMainIcd.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsMainIcd.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsMainIcd.Caption = "CĐ chính";
            this.gridColumnIsMainIcd.ColumnEdit = this.CheckEditMainICD;
            this.gridColumnIsMainIcd.FieldName = "ICD_MAIN_CHECK";
            this.gridColumnIsMainIcd.Name = "gridColumnIsMainIcd";
            this.gridColumnIsMainIcd.ToolTip = "Chẩn đoán chính";
            this.gridColumnIsMainIcd.Visible = true;
            this.gridColumnIsMainIcd.VisibleIndex = 2;
            this.gridColumnIsMainIcd.Width = 74;
            // 
            // CheckEditMainICD
            // 
            this.CheckEditMainICD.AutoHeight = false;
            this.CheckEditMainICD.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.CheckEditMainICD.Name = "CheckEditMainICD";
            // 
            // gridColumnIsCauseIcd
            // 
            this.gridColumnIsCauseIcd.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnIsCauseIcd.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsCauseIcd.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnIsCauseIcd.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnIsCauseIcd.Caption = "CĐ phụ";
            this.gridColumnIsCauseIcd.ColumnEdit = this.CheckEditICDCause;
            this.gridColumnIsCauseIcd.FieldName = "ICD_CAUSE_CHECK";
            this.gridColumnIsCauseIcd.Name = "gridColumnIsCauseIcd";
            this.gridColumnIsCauseIcd.ToolTip = "Chẩn đoán phụ";
            this.gridColumnIsCauseIcd.Visible = true;
            this.gridColumnIsCauseIcd.VisibleIndex = 3;
            this.gridColumnIsCauseIcd.Width = 62;
            // 
            // CheckEditICDCause
            // 
            this.CheckEditICDCause.AutoHeight = false;
            this.CheckEditICDCause.Name = "CheckEditICDCause";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSkip);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.btnAddIcd);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(271, 144, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(644, 587);
            this.layoutControl1.TabIndex = 30;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.btnFind);
            this.layoutControl3.Controls.Add(this.txtSearch);
            this.layoutControl3.Controls.Add(this.labelControl2);
            this.layoutControl3.Controls.Add(this.gridControlRemoveIcd);
            this.layoutControl3.Location = new System.Drawing.Point(2, 278);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(640, 281);
            this.layoutControl3.TabIndex = 36;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(293, 26);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(121, 22);
            this.btnFind.StyleController = this.layoutControl3;
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "Tìm (Ctrl D)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(2, 26);
            this.txtSearch.MenuManager = this.barManager2;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(287, 20);
            this.txtSearch.StyleController = this.layoutControl3;
            this.txtSearch.TabIndex = 6;
            // 
            // barManager2
            // 
            this.barManager2.DockControls.Add(this.barDockControl1);
            this.barManager2.DockControls.Add(this.barDockControl2);
            this.barManager2.DockControls.Add(this.barDockControl3);
            this.barManager2.DockControls.Add(this.barDockControl4);
            this.barManager2.Form = this;
            this.barManager2.MaxItemId = 0;
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 29);
            this.barDockControl1.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 616);
            this.barDockControl2.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 587);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(644, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 587);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(2, 2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(636, 20);
            this.labelControl2.StyleController = this.layoutControl3;
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Các dịch vụ chống chỉ định trong các trường hợp chẩn đoán sau:";
            // 
            // gridControlRemoveIcd
            // 
            this.gridControlRemoveIcd.Location = new System.Drawing.Point(2, 52);
            this.gridControlRemoveIcd.MainView = this.gridViewRemoveIcd;
            this.gridControlRemoveIcd.MenuManager = this.barManager2;
            this.gridControlRemoveIcd.Name = "gridControlRemoveIcd";
            this.gridControlRemoveIcd.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.CheckEditRemoveMainICD,
            this.CheckEditRemoveICDCause});
            this.gridControlRemoveIcd.Size = new System.Drawing.Size(636, 227);
            this.gridControlRemoveIcd.TabIndex = 4;
            this.gridControlRemoveIcd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRemoveIcd});
            // 
            // gridViewRemoveIcd
            // 
            this.gridViewRemoveIcd.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gridViewRemoveIcd.GridControl = this.gridControlRemoveIcd;
            this.gridViewRemoveIcd.Name = "gridViewRemoveIcd";
            this.gridViewRemoveIcd.OptionsView.ShowGroupPanel = false;
            this.gridViewRemoveIcd.OptionsView.ShowIndicator = false;
            this.gridViewRemoveIcd.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewRemoveIcd_CellValueChanged);
            this.gridViewRemoveIcd.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewRemoveIcd_CustomUnboundColumnData);
            this.gridViewRemoveIcd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewRemoveIcd_MouseDown);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Dịch vụ";
            this.gridColumn1.FieldName = "SERVICE_NAME";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 200;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Chẩn đoán (ICD)";
            this.gridColumn2.FieldName = "ICD_NAME";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 302;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "CĐ chính";
            this.gridColumn3.ColumnEdit = this.CheckEditRemoveMainICD;
            this.gridColumn3.FieldName = "ICD_MAIN_CHECK";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ToolTip = "Chẩn đoán chính";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 74;
            // 
            // CheckEditRemoveMainICD
            // 
            this.CheckEditRemoveMainICD.AutoHeight = false;
            this.CheckEditRemoveMainICD.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.CheckEditRemoveMainICD.Name = "CheckEditRemoveMainICD";
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "CĐ phụ";
            this.gridColumn4.ColumnEdit = this.CheckEditRemoveICDCause;
            this.gridColumn4.FieldName = "ICD_CAUSE_CHECK";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.ToolTip = "Chẩn đoán phụ";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 62;
            // 
            // CheckEditRemoveICDCause
            // 
            this.CheckEditRemoveICDCause.AutoHeight = false;
            this.CheckEditRemoveICDCause.Name = "CheckEditRemoveICDCause";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutControlItem12,
            this.emptySpaceItem3});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(640, 281);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.gridControlRemoveIcd;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(640, 231);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.labelControl2;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem10.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem10.MinSize = new System.Drawing.Size(67, 24);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(640, 24);
            this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.txtSearch;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(291, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.btnFind;
            this.layoutControlItem12.Location = new System.Drawing.Point(291, 24);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(125, 26);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(416, 24);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(224, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.gridControlMissingIcd);
            this.layoutControl2.Controls.Add(this.labelControl1);
            this.layoutControl2.Controls.Add(this.btnSearch);
            this.layoutControl2.Controls.Add(this.txtKeyword);
            this.layoutControl2.Location = new System.Drawing.Point(2, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(640, 272);
            this.layoutControl2.TabIndex = 35;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // labelControl1
            // 
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(2, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(636, 20);
            this.labelControl1.StyleController = this.layoutControl2;
            this.labelControl1.TabIndex = 32;
            this.labelControl1.Text = "Các dịch vụ chỉ cho phép chỉ định trong các trường hợp chẩn đoán sau:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(296, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(113, 22);
            this.btnSearch.StyleController = this.layoutControl2;
            this.btnSearch.TabIndex = 34;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 26);
            this.txtKeyword.MenuManager = this.barManager2;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(290, 20);
            this.txtKeyword.StyleController = this.layoutControl2;
            this.txtKeyword.TabIndex = 33;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem2,
            this.layoutControlItem4});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(640, 272);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlMissingIcd;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(640, 222);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtKeyword;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(294, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSearch;
            this.layoutControlItem6.Location = new System.Drawing.Point(294, 24);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(117, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(411, 24);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(229, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.labelControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(14, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(640, 24);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // btnAddIcd
            // 
            this.btnAddIcd.Location = new System.Drawing.Point(476, 563);
            this.btnAddIcd.Name = "btnAddIcd";
            this.btnAddIcd.Size = new System.Drawing.Size(106, 22);
            this.btnAddIcd.StyleController = this.layoutControl1;
            this.btnAddIcd.TabIndex = 31;
            this.btnAddIcd.Text = "Cập nhật chẩn đoán";
            this.btnAddIcd.Click += new System.EventHandler(this.btnAddIcd_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(586, 563);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 30;
            this.btnPrint.Text = "Đóng";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.lciSkip});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(644, 587);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPrint;
            this.layoutControlItem2.Location = new System.Drawing.Point(584, 561);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(60, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 561);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(409, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnAddIcd;
            this.layoutControlItem3.Location = new System.Drawing.Point(474, 561);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.layoutControl2;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(644, 276);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.layoutControl3;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 276);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(644, 285);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
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
            this.bbtnPrint,
            this.bbtnSearch,
            this.bbtnFind});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnFind)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnPrint
            // 
            this.bbtnPrint.Caption = "In (Ctrl P)";
            this.bbtnPrint.Id = 0;
            this.bbtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnPrint.Name = "bbtnPrint";
            this.bbtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnPrint_ItemClick);
            // 
            // bbtnSearch
            // 
            this.bbtnSearch.Caption = "Tìm (Ctrl F)";
            this.bbtnSearch.Id = 1;
            this.bbtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnSearch.Name = "bbtnSearch";
            this.bbtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
            // 
            // bbtnFind
            // 
            this.bbtnFind.Caption = "Tìm (Ctrl D)";
            this.bbtnFind.Id = 2;
            this.bbtnFind.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.bbtnFind.Name = "bbtnFind";
            this.bbtnFind.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnFind_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(644, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 616);
            this.barDockControlBottom.Size = new System.Drawing.Size(644, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 587);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(644, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 587);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(411, 563);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(61, 22);
            this.btnSkip.StyleController = this.layoutControl1;
            this.btnSkip.TabIndex = 37;
            this.btnSkip.Text = "Bỏ qua";
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // lciSkip
            // 
            this.lciSkip.Control = this.btnSkip;
            this.lciSkip.Location = new System.Drawing.Point(409, 561);
            this.lciSkip.Name = "lciSkip";
            this.lciSkip.Size = new System.Drawing.Size(65, 26);
            this.lciSkip.TextSize = new System.Drawing.Size(0, 0);
            this.lciSkip.TextVisible = false;
            this.lciSkip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // frmMissingIcd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 616);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMissingIcd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiếu thông tin chẩn đoán";
            this.Load += new System.EventHandler(this.frmDetail_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMissingIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMissingIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditMainICD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditICDCause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRemoveIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRemoveIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditRemoveMainICD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckEditRemoveICDCause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControlMissingIcd;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMissingIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIcdName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsMainIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIsCauseIcd;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.BarManager barManager2;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnPrint;
        private DevExpress.XtraEditors.SimpleButton btnAddIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditMainICD;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditICDCause;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraGrid.GridControl gridControlRemoveIcd;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRemoveIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditRemoveMainICD;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckEditRemoveICDCause;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraBars.BarButtonItem bbtnFind;
        private DevExpress.XtraEditors.SimpleButton btnSkip;
        private DevExpress.XtraLayout.LayoutControlItem lciSkip;
    }
}
