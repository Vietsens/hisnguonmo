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
namespace HIS.Desktop.Plugins.MobaExamPresCreate
{
    partial class frmImpMobaCreate
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.lblTotalVatPrice = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalImpPrice = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalPrice = new DevExpress.XtraEditors.LabelControl();
            this.xtraTabControlMain = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPageMedicine = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlMedicine = new DevExpress.XtraGrid.GridControl();
            this.gridViewMedicine = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Medicine_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_Code = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_Name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_ServiceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_ThAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_AvaiAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_ExpAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Medicine_TotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinMedicineThAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinMedicineThAmountDisable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.xtraTabPageMaterial = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlMaterial = new DevExpress.XtraGrid.GridControl();
            this.gridViewMaterial = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Material_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_Code = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_Name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_ServiceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_ThAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_AvaiAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_ExpAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Material_TotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinMaterialThAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinMaterialThAmountDisable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtReason = new DevExpress.XtraEditors.TextEdit();
            this.cboTracking = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnItemPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalImpPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTotalVatPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciTracking = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControlMain)).BeginInit();
            this.xtraTabControlMain.SuspendLayout();
            this.xtraTabPageMedicine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMedicineThAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMedicineThAmountDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.xtraTabPageMaterial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMaterialThAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMaterialThAmountDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalImpPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalVatPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.lblTotalVatPrice);
            this.layoutControl1.Controls.Add(this.lblTotalImpPrice);
            this.layoutControl1.Controls.Add(this.lblTotalPrice);
            this.layoutControl1.Controls.Add(this.xtraTabControlMain);
            this.layoutControl1.Controls.Add(this.txtReason);
            this.layoutControl1.Controls.Add(this.cboTracking);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 38);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1467, 652);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1287, 622);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(177, 27);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 10;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1103, 622);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(178, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTotalVatPrice
            // 
            this.lblTotalVatPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalVatPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalVatPrice.Location = new System.Drawing.Point(832, 622);
            this.lblTotalVatPrice.Margin = new System.Windows.Forms.Padding(4);
            this.lblTotalVatPrice.Name = "lblTotalVatPrice";
            this.lblTotalVatPrice.Size = new System.Drawing.Size(265, 20);
            this.lblTotalVatPrice.StyleController = this.layoutControl1;
            this.lblTotalVatPrice.TabIndex = 8;
            // 
            // lblTotalImpPrice
            // 
            this.lblTotalImpPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalImpPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalImpPrice.Location = new System.Drawing.Point(465, 622);
            this.lblTotalImpPrice.Margin = new System.Windows.Forms.Padding(4);
            this.lblTotalImpPrice.Name = "lblTotalImpPrice";
            this.lblTotalImpPrice.Size = new System.Drawing.Size(266, 20);
            this.lblTotalImpPrice.StyleController = this.layoutControl1;
            this.lblTotalImpPrice.TabIndex = 7;
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPrice.Location = new System.Drawing.Point(98, 622);
            this.lblTotalPrice.Margin = new System.Windows.Forms.Padding(4);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(266, 20);
            this.lblTotalPrice.StyleController = this.layoutControl1;
            this.lblTotalPrice.TabIndex = 6;
            // 
            // xtraTabControlMain
            // 
            this.xtraTabControlMain.Location = new System.Drawing.Point(0, 28);
            this.xtraTabControlMain.Margin = new System.Windows.Forms.Padding(4);
            this.xtraTabControlMain.Name = "xtraTabControlMain";
            this.xtraTabControlMain.SelectedTabPage = this.xtraTabPageMedicine;
            this.xtraTabControlMain.Size = new System.Drawing.Size(1467, 591);
            this.xtraTabControlMain.TabIndex = 5;
            this.xtraTabControlMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPageMedicine,
            this.xtraTabPageMaterial});
            // 
            // xtraTabPageMedicine
            // 
            this.xtraTabPageMedicine.Controls.Add(this.layoutControl2);
            this.xtraTabPageMedicine.Margin = new System.Windows.Forms.Padding(4);
            this.xtraTabPageMedicine.Name = "xtraTabPageMedicine";
            this.xtraTabPageMedicine.Size = new System.Drawing.Size(1460, 556);
            this.xtraTabPageMedicine.Text = "Thuốc";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.gridControlMedicine);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(1460, 556);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // gridControlMedicine
            // 
            this.gridControlMedicine.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlMedicine.Location = new System.Drawing.Point(0, 0);
            this.gridControlMedicine.MainView = this.gridViewMedicine;
            this.gridControlMedicine.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlMedicine.Name = "gridControlMedicine";
            this.gridControlMedicine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinMedicineThAmount,
            this.repositoryItemSpinMedicineThAmountDisable});
            this.gridControlMedicine.Size = new System.Drawing.Size(1460, 556);
            this.gridControlMedicine.TabIndex = 4;
            this.gridControlMedicine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMedicine});
            // 
            // gridViewMedicine
            // 
            this.gridViewMedicine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Medicine_Stt,
            this.gridColumn_Medicine_Code,
            this.gridColumn_Medicine_Name,
            this.gridColumn_Medicine_ServiceUnit,
            this.gridColumn_Medicine_ThAmount,
            this.gridColumn_Medicine_AvaiAmount,
            this.gridColumn_Medicine_ExpAmount,
            this.gridColumn_Medicine_Price,
            this.gridColumn_Medicine_VatRatio,
            this.gridColumn_Medicine_TotalPrice,
            this.gridColumn1});
            this.gridViewMedicine.GridControl = this.gridControlMedicine;
            this.gridViewMedicine.Name = "gridViewMedicine";
            this.gridViewMedicine.OptionsBehavior.AutoPopulateColumns = false;
            this.gridViewMedicine.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewMedicine.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewMedicine.OptionsSelection.MultiSelect = true;
            this.gridViewMedicine.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewMedicine.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewMedicine.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewMedicine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewMedicine.OptionsView.ShowGroupPanel = false;
            this.gridViewMedicine.OptionsView.ShowIndicator = false;
            this.gridViewMedicine.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMedicine_CustomRowCellEdit);
            this.gridViewMedicine.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewMedicine_SelectionChanged);
            this.gridViewMedicine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMedicine_CellValueChanged);
            this.gridViewMedicine.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMedicine_CustomUnboundColumnData);
            // 
            // gridColumn_Medicine_Stt
            // 
            this.gridColumn_Medicine_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_Stt.Caption = "STT";
            this.gridColumn_Medicine_Stt.FieldName = "STT";
            this.gridColumn_Medicine_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Medicine_Stt.Name = "gridColumn_Medicine_Stt";
            this.gridColumn_Medicine_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_Stt.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_Medicine_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Medicine_Stt.Visible = true;
            this.gridColumn_Medicine_Stt.VisibleIndex = 1;
            this.gridColumn_Medicine_Stt.Width = 30;
            // 
            // gridColumn_Medicine_Code
            // 
            this.gridColumn_Medicine_Code.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Code.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_Code.Caption = "Mã thuốc";
            this.gridColumn_Medicine_Code.FieldName = "SERVICE_CODE";
            this.gridColumn_Medicine_Code.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Medicine_Code.Name = "gridColumn_Medicine_Code";
            this.gridColumn_Medicine_Code.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_Code.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_Code.Visible = true;
            this.gridColumn_Medicine_Code.VisibleIndex = 2;
            this.gridColumn_Medicine_Code.Width = 60;
            // 
            // gridColumn_Medicine_Name
            // 
            this.gridColumn_Medicine_Name.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Name.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_Name.Caption = "Tên thuốc";
            this.gridColumn_Medicine_Name.FieldName = "SERVICE_NAME";
            this.gridColumn_Medicine_Name.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Medicine_Name.Name = "gridColumn_Medicine_Name";
            this.gridColumn_Medicine_Name.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_Name.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_Name.Visible = true;
            this.gridColumn_Medicine_Name.VisibleIndex = 3;
            this.gridColumn_Medicine_Name.Width = 250;
            // 
            // gridColumn_Medicine_ServiceUnit
            // 
            this.gridColumn_Medicine_ServiceUnit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_ServiceUnit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_ServiceUnit.Caption = "Đơn vị tính";
            this.gridColumn_Medicine_ServiceUnit.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn_Medicine_ServiceUnit.Name = "gridColumn_Medicine_ServiceUnit";
            this.gridColumn_Medicine_ServiceUnit.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_ServiceUnit.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_ServiceUnit.Visible = true;
            this.gridColumn_Medicine_ServiceUnit.VisibleIndex = 4;
            this.gridColumn_Medicine_ServiceUnit.Width = 81;
            // 
            // gridColumn_Medicine_ThAmount
            // 
            this.gridColumn_Medicine_ThAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_ThAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_ThAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_ThAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_ThAmount.Caption = "Số lượng thu hồi";
            this.gridColumn_Medicine_ThAmount.DisplayFormat.FormatString = "#,##";
            this.gridColumn_Medicine_ThAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_ThAmount.FieldName = "TH_AMOUNT";
            this.gridColumn_Medicine_ThAmount.Name = "gridColumn_Medicine_ThAmount";
            this.gridColumn_Medicine_ThAmount.Visible = true;
            this.gridColumn_Medicine_ThAmount.VisibleIndex = 5;
            this.gridColumn_Medicine_ThAmount.Width = 107;
            // 
            // gridColumn_Medicine_AvaiAmount
            // 
            this.gridColumn_Medicine_AvaiAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_AvaiAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_AvaiAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_AvaiAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_AvaiAmount.Caption = "Khả dụng";
            this.gridColumn_Medicine_AvaiAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Medicine_AvaiAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_AvaiAmount.FieldName = "AVAI_AMOUNT";
            this.gridColumn_Medicine_AvaiAmount.Name = "gridColumn_Medicine_AvaiAmount";
            this.gridColumn_Medicine_AvaiAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_AvaiAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_AvaiAmount.Visible = true;
            this.gridColumn_Medicine_AvaiAmount.VisibleIndex = 6;
            this.gridColumn_Medicine_AvaiAmount.Width = 107;
            // 
            // gridColumn_Medicine_ExpAmount
            // 
            this.gridColumn_Medicine_ExpAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_ExpAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_ExpAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_ExpAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_ExpAmount.Caption = "Số lượng xuất";
            this.gridColumn_Medicine_ExpAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Medicine_ExpAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_ExpAmount.FieldName = "EXP_AMOUNT";
            this.gridColumn_Medicine_ExpAmount.Name = "gridColumn_Medicine_ExpAmount";
            this.gridColumn_Medicine_ExpAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_ExpAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_ExpAmount.Visible = true;
            this.gridColumn_Medicine_ExpAmount.VisibleIndex = 7;
            this.gridColumn_Medicine_ExpAmount.Width = 100;
            // 
            // gridColumn_Medicine_Price
            // 
            this.gridColumn_Medicine_Price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_Price.Caption = "Giá xuất";
            this.gridColumn_Medicine_Price.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Medicine_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_Price.FieldName = "PRICE";
            this.gridColumn_Medicine_Price.Name = "gridColumn_Medicine_Price";
            this.gridColumn_Medicine_Price.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_Price.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_Price.Visible = true;
            this.gridColumn_Medicine_Price.VisibleIndex = 9;
            this.gridColumn_Medicine_Price.Width = 123;
            // 
            // gridColumn_Medicine_VatRatio
            // 
            this.gridColumn_Medicine_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_VatRatio.Caption = "VAT (%)";
            this.gridColumn_Medicine_VatRatio.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Medicine_VatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_VatRatio.FieldName = "VAT_RATIO";
            this.gridColumn_Medicine_VatRatio.Name = "gridColumn_Medicine_VatRatio";
            this.gridColumn_Medicine_VatRatio.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_VatRatio.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_VatRatio.Visible = true;
            this.gridColumn_Medicine_VatRatio.VisibleIndex = 10;
            this.gridColumn_Medicine_VatRatio.Width = 79;
            // 
            // gridColumn_Medicine_TotalPrice
            // 
            this.gridColumn_Medicine_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Medicine_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Medicine_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Medicine_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Medicine_TotalPrice.Caption = "Thành tiền";
            this.gridColumn_Medicine_TotalPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Medicine_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Medicine_TotalPrice.FieldName = "TOTAL_PRICE";
            this.gridColumn_Medicine_TotalPrice.Name = "gridColumn_Medicine_TotalPrice";
            this.gridColumn_Medicine_TotalPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_Medicine_TotalPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_Medicine_TotalPrice.Visible = true;
            this.gridColumn_Medicine_TotalPrice.VisibleIndex = 11;
            this.gridColumn_Medicine_TotalPrice.Width = 128;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Số lô";
            this.gridColumn1.FieldName = "PACKAGE_NUMBER";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 8;
            // 
            // repositoryItemSpinMedicineThAmount
            // 
            this.repositoryItemSpinMedicineThAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemSpinMedicineThAmount.IsFloatValue = false;
            this.repositoryItemSpinMedicineThAmount.Mask.EditMask = "N00";
            this.repositoryItemSpinMedicineThAmount.MaxLength = 14;
            this.repositoryItemSpinMedicineThAmount.Name = "repositoryItemSpinMedicineThAmount";
            // 
            // repositoryItemSpinMedicineThAmountDisable
            // 
            this.repositoryItemSpinMedicineThAmountDisable.AutoHeight = false;
            this.repositoryItemSpinMedicineThAmountDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.repositoryItemSpinMedicineThAmountDisable.MaxLength = 14;
            this.repositoryItemSpinMedicineThAmountDisable.Name = "repositoryItemSpinMedicineThAmountDisable";
            this.repositoryItemSpinMedicineThAmountDisable.ReadOnly = true;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1460, 556);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlMedicine;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(1460, 556);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // xtraTabPageMaterial
            // 
            this.xtraTabPageMaterial.Controls.Add(this.layoutControl3);
            this.xtraTabPageMaterial.Margin = new System.Windows.Forms.Padding(4);
            this.xtraTabPageMaterial.Name = "xtraTabPageMaterial";
            this.xtraTabPageMaterial.Size = new System.Drawing.Size(1460, 556);
            this.xtraTabPageMaterial.Text = "Vật tư";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.gridControlMaterial);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(1460, 556);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // gridControlMaterial
            // 
            this.gridControlMaterial.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlMaterial.Location = new System.Drawing.Point(0, 0);
            this.gridControlMaterial.MainView = this.gridViewMaterial;
            this.gridControlMaterial.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlMaterial.Name = "gridControlMaterial";
            this.gridControlMaterial.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinMaterialThAmount,
            this.repositoryItemSpinMaterialThAmountDisable});
            this.gridControlMaterial.Size = new System.Drawing.Size(1460, 556);
            this.gridControlMaterial.TabIndex = 4;
            this.gridControlMaterial.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMaterial});
            // 
            // gridViewMaterial
            // 
            this.gridViewMaterial.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Material_VatRatio,
            this.gridColumn_Material_Stt,
            this.gridColumn_Material_Code,
            this.gridColumn_Material_Name,
            this.gridColumn_Material_ServiceUnit,
            this.gridColumn_Material_ThAmount,
            this.gridColumn_Material_AvaiAmount,
            this.gridColumn_Material_ExpAmount,
            this.gridColumn_Material_Price,
            this.gridColumn_Material_TotalPrice});
            this.gridViewMaterial.GridControl = this.gridControlMaterial;
            this.gridViewMaterial.Name = "gridViewMaterial";
            this.gridViewMaterial.OptionsBehavior.AutoPopulateColumns = false;
            this.gridViewMaterial.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewMaterial.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewMaterial.OptionsSelection.MultiSelect = true;
            this.gridViewMaterial.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewMaterial.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewMaterial.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewMaterial.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewMaterial.OptionsView.ShowGroupPanel = false;
            this.gridViewMaterial.OptionsView.ShowIndicator = false;
            this.gridViewMaterial.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewMaterial_CustomRowCellEdit);
            this.gridViewMaterial.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewMaterial_SelectionChanged);
            this.gridViewMaterial.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMaterial_CellValueChanged);
            this.gridViewMaterial.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewMaterial_CustomUnboundColumnData);
            // 
            // gridColumn_Material_VatRatio
            // 
            this.gridColumn_Material_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_VatRatio.Caption = "VAT (%)";
            this.gridColumn_Material_VatRatio.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Material_VatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_VatRatio.FieldName = "VAT_RATIO";
            this.gridColumn_Material_VatRatio.Name = "gridColumn_Material_VatRatio";
            this.gridColumn_Material_VatRatio.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_VatRatio.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_VatRatio.Visible = true;
            this.gridColumn_Material_VatRatio.VisibleIndex = 9;
            this.gridColumn_Material_VatRatio.Width = 65;
            // 
            // gridColumn_Material_Stt
            // 
            this.gridColumn_Material_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_Stt.Caption = "STT";
            this.gridColumn_Material_Stt.FieldName = "STT";
            this.gridColumn_Material_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Material_Stt.Name = "gridColumn_Material_Stt";
            this.gridColumn_Material_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_Stt.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_Material_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Material_Stt.Visible = true;
            this.gridColumn_Material_Stt.VisibleIndex = 1;
            this.gridColumn_Material_Stt.Width = 30;
            // 
            // gridColumn_Material_Code
            // 
            this.gridColumn_Material_Code.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_Code.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_Code.Caption = "Mã vật tư";
            this.gridColumn_Material_Code.FieldName = "SERVICE_CODE";
            this.gridColumn_Material_Code.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Material_Code.Name = "gridColumn_Material_Code";
            this.gridColumn_Material_Code.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_Code.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_Code.Visible = true;
            this.gridColumn_Material_Code.VisibleIndex = 2;
            this.gridColumn_Material_Code.Width = 65;
            // 
            // gridColumn_Material_Name
            // 
            this.gridColumn_Material_Name.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_Name.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_Name.Caption = "Tên vật tư";
            this.gridColumn_Material_Name.FieldName = "SERVICE_NAME";
            this.gridColumn_Material_Name.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Material_Name.Name = "gridColumn_Material_Name";
            this.gridColumn_Material_Name.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_Name.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_Name.Visible = true;
            this.gridColumn_Material_Name.VisibleIndex = 3;
            this.gridColumn_Material_Name.Width = 250;
            // 
            // gridColumn_Material_ServiceUnit
            // 
            this.gridColumn_Material_ServiceUnit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_ServiceUnit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_ServiceUnit.Caption = "Đơn vị tính";
            this.gridColumn_Material_ServiceUnit.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn_Material_ServiceUnit.Name = "gridColumn_Material_ServiceUnit";
            this.gridColumn_Material_ServiceUnit.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_ServiceUnit.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_ServiceUnit.Visible = true;
            this.gridColumn_Material_ServiceUnit.VisibleIndex = 4;
            this.gridColumn_Material_ServiceUnit.Width = 65;
            // 
            // gridColumn_Material_ThAmount
            // 
            this.gridColumn_Material_ThAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_ThAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_ThAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_ThAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_ThAmount.Caption = "Số lượng thu hồi";
            this.gridColumn_Material_ThAmount.DisplayFormat.FormatString = "#,##";
            this.gridColumn_Material_ThAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_ThAmount.FieldName = "TH_AMOUNT";
            this.gridColumn_Material_ThAmount.Name = "gridColumn_Material_ThAmount";
            this.gridColumn_Material_ThAmount.Visible = true;
            this.gridColumn_Material_ThAmount.VisibleIndex = 5;
            this.gridColumn_Material_ThAmount.Width = 85;
            // 
            // gridColumn_Material_AvaiAmount
            // 
            this.gridColumn_Material_AvaiAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_AvaiAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_AvaiAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_AvaiAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_AvaiAmount.Caption = "Khả dụng";
            this.gridColumn_Material_AvaiAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Material_AvaiAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_AvaiAmount.FieldName = "AVAI_AMOUNT";
            this.gridColumn_Material_AvaiAmount.Name = "gridColumn_Material_AvaiAmount";
            this.gridColumn_Material_AvaiAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_AvaiAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_AvaiAmount.Visible = true;
            this.gridColumn_Material_AvaiAmount.VisibleIndex = 6;
            this.gridColumn_Material_AvaiAmount.Width = 85;
            // 
            // gridColumn_Material_ExpAmount
            // 
            this.gridColumn_Material_ExpAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_ExpAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_ExpAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_ExpAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_ExpAmount.Caption = "Số lượng xuất";
            this.gridColumn_Material_ExpAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_Material_ExpAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_ExpAmount.FieldName = "EXP_AMOUNT";
            this.gridColumn_Material_ExpAmount.Name = "gridColumn_Material_ExpAmount";
            this.gridColumn_Material_ExpAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_ExpAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_ExpAmount.Visible = true;
            this.gridColumn_Material_ExpAmount.VisibleIndex = 7;
            // 
            // gridColumn_Material_Price
            // 
            this.gridColumn_Material_Price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_Price.Caption = "Giá xuất";
            this.gridColumn_Material_Price.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Material_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_Price.FieldName = "PRICE";
            this.gridColumn_Material_Price.Name = "gridColumn_Material_Price";
            this.gridColumn_Material_Price.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_Price.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_Price.Visible = true;
            this.gridColumn_Material_Price.VisibleIndex = 8;
            this.gridColumn_Material_Price.Width = 100;
            // 
            // gridColumn_Material_TotalPrice
            // 
            this.gridColumn_Material_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Material_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Material_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Material_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Material_TotalPrice.Caption = "Thành tiền";
            this.gridColumn_Material_TotalPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Material_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Material_TotalPrice.FieldName = "TOTAL_PRICE";
            this.gridColumn_Material_TotalPrice.Name = "gridColumn_Material_TotalPrice";
            this.gridColumn_Material_TotalPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_Material_TotalPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_Material_TotalPrice.Visible = true;
            this.gridColumn_Material_TotalPrice.VisibleIndex = 10;
            this.gridColumn_Material_TotalPrice.Width = 100;
            // 
            // repositoryItemSpinMaterialThAmount
            // 
            this.repositoryItemSpinMaterialThAmount.AutoHeight = false;
            this.repositoryItemSpinMaterialThAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemSpinMaterialThAmount.IsFloatValue = false;
            this.repositoryItemSpinMaterialThAmount.Mask.EditMask = "N00";
            this.repositoryItemSpinMaterialThAmount.MaxLength = 14;
            this.repositoryItemSpinMaterialThAmount.Name = "repositoryItemSpinMaterialThAmount";
            // 
            // repositoryItemSpinMaterialThAmountDisable
            // 
            this.repositoryItemSpinMaterialThAmountDisable.AutoHeight = false;
            this.repositoryItemSpinMaterialThAmountDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.repositoryItemSpinMaterialThAmountDisable.MaxLength = 14;
            this.repositoryItemSpinMaterialThAmountDisable.Name = "repositoryItemSpinMaterialThAmountDisable";
            this.repositoryItemSpinMaterialThAmountDisable.ReadOnly = true;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1460, 556);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlMaterial;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(1460, 556);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // txtReason
            // 
            this.txtReason.Location = new System.Drawing.Point(98, 3);
            this.txtReason.Margin = new System.Windows.Forms.Padding(4);
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(633, 22);
            this.txtReason.StyleController = this.layoutControl1;
            this.txtReason.TabIndex = 4;
            // 
            // cboTracking
            // 
            this.cboTracking.Location = new System.Drawing.Point(806, 3);
            this.cboTracking.MenuManager = this.barManager1;
            this.cboTracking.Name = "cboTracking";
            this.cboTracking.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTracking.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cboTracking.Properties.NullText = "";
            this.cboTracking.Properties.PopupSizeable = false;
            this.cboTracking.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboTracking.Properties.View = this.gridLookUpEdit1View;
            this.cboTracking.Size = new System.Drawing.Size(291, 22);
            this.cboTracking.StyleController = this.layoutControl1;
            this.cboTracking.TabIndex = 11;
            this.cboTracking.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTracking_ButtonClick);
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
            this.barBtnItemSave,
            this.barBtnItemPrint});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnItemPrint)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnItemSave
            // 
            this.barBtnItemSave.Caption = "Lưu (Ctrl S)";
            this.barBtnItemSave.Id = 0;
            this.barBtnItemSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnItemSave.Name = "barBtnItemSave";
            this.barBtnItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemSave_ItemClick);
            // 
            // barBtnItemPrint
            // 
            this.barBtnItemPrint.Caption = "In (Ctrl P)";
            this.barBtnItemPrint.Id = 1;
            this.barBtnItemPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnItemPrint.Name = "barBtnItemPrint";
            this.barBtnItemPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnItemPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1467, 38);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 690);
            this.barDockControlBottom.Size = new System.Drawing.Size(1467, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 652);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1467, 38);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 652);
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
            this.lciReason,
            this.layoutControlItem2,
            this.lciTotalPrice,
            this.lciTotalImpPrice,
            this.lciTotalVatPrice,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.emptySpaceItem1,
            this.lciTracking});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1467, 652);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciReason
            // 
            this.lciReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciReason.Control = this.txtReason;
            this.lciReason.Location = new System.Drawing.Point(0, 0);
            this.lciReason.Name = "lciReason";
            this.lciReason.Size = new System.Drawing.Size(734, 28);
            this.lciReason.Text = "Lý do:";
            this.lciReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReason.TextSize = new System.Drawing.Size(90, 20);
            this.lciReason.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.xtraTabControlMain;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 28);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(1467, 591);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciTotalPrice
            // 
            this.lciTotalPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lciTotalPrice.AppearanceItemCaption.Options.UseFont = true;
            this.lciTotalPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotalPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotalPrice.Control = this.lblTotalPrice;
            this.lciTotalPrice.Location = new System.Drawing.Point(0, 619);
            this.lciTotalPrice.Name = "lciTotalPrice";
            this.lciTotalPrice.Size = new System.Drawing.Size(367, 33);
            this.lciTotalPrice.Text = "Tổng tiền:";
            this.lciTotalPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTotalPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lciTotalPrice.TextToControlDistance = 5;
            // 
            // lciTotalImpPrice
            // 
            this.lciTotalImpPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lciTotalImpPrice.AppearanceItemCaption.Options.UseFont = true;
            this.lciTotalImpPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotalImpPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotalImpPrice.Control = this.lblTotalImpPrice;
            this.lciTotalImpPrice.Location = new System.Drawing.Point(367, 619);
            this.lciTotalImpPrice.Name = "lciTotalImpPrice";
            this.lciTotalImpPrice.Size = new System.Drawing.Size(367, 33);
            this.lciTotalImpPrice.Text = "Tiền vốn:";
            this.lciTotalImpPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTotalImpPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lciTotalImpPrice.TextToControlDistance = 5;
            // 
            // lciTotalVatPrice
            // 
            this.lciTotalVatPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lciTotalVatPrice.AppearanceItemCaption.Options.UseFont = true;
            this.lciTotalVatPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTotalVatPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTotalVatPrice.Control = this.lblTotalVatPrice;
            this.lciTotalVatPrice.Location = new System.Drawing.Point(734, 619);
            this.lciTotalVatPrice.Name = "lciTotalVatPrice";
            this.lciTotalVatPrice.Size = new System.Drawing.Size(366, 33);
            this.lciTotalVatPrice.Text = "Tiền thuế:";
            this.lciTotalVatPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTotalVatPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lciTotalVatPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnSave;
            this.layoutControlItem8.Location = new System.Drawing.Point(1100, 619);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(184, 33);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnPrint;
            this.layoutControlItem9.Location = new System.Drawing.Point(1284, 619);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(183, 33);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(1100, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(367, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciTracking
            // 
            this.lciTracking.Control = this.cboTracking;
            this.lciTracking.Location = new System.Drawing.Point(734, 0);
            this.lciTracking.Name = "lciTracking";
            this.lciTracking.Size = new System.Drawing.Size(366, 28);
            this.lciTracking.Text = "Tờ điều trị:";
            this.lciTracking.TextSize = new System.Drawing.Size(66, 17);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmImpMobaCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1467, 690);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmImpMobaCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tạo đơn phòng khám trả lại";
            this.Load += new System.EventHandler(this.frmImpMobaCreate_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControlMain)).EndInit();
            this.xtraTabControlMain.ResumeLayout(false);
            this.xtraTabPageMedicine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMedicineThAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMedicineThAmountDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.xtraTabPageMaterial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMaterialThAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMaterialThAmountDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalImpPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTotalVatPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtReason;
        private DevExpress.XtraLayout.LayoutControlItem lciReason;
        private DevExpress.XtraTab.XtraTabControl xtraTabControlMain;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageMedicine;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageMaterial;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraGrid.GridControl gridControlMedicine;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMedicine;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraGrid.GridControl gridControlMaterial;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaterial;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.LabelControl lblTotalPrice;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalPrice;
        private DevExpress.XtraEditors.LabelControl lblTotalVatPrice;
        private DevExpress.XtraEditors.LabelControl lblTotalImpPrice;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalImpPrice;
        private DevExpress.XtraLayout.LayoutControlItem lciTotalVatPrice;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_Code;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_Name;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_ServiceUnit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_ThAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_AvaiAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_Price;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_VatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_TotalPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_VatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_Code;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_Name;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_ServiceUnit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_ThAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_AvaiAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_Price;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_TotalPrice;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnItemSave;
        private DevExpress.XtraBars.BarButtonItem barBtnItemPrint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMedicineThAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMaterialThAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMedicineThAmountDisable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMaterialThAmountDisable;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Medicine_ExpAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Material_ExpAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.GridLookUpEdit cboTracking;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciTracking;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
