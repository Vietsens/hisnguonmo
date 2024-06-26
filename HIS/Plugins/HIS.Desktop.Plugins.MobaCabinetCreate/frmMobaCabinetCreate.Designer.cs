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
namespace HIS.Desktop.Plugins.MobaCabinetCreate
{
    partial class frmMobaCabinetCreate
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
            this.cboTracking = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.cboImpMediStock = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblTotalVatPrice = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalFeePrice = new DevExpress.XtraEditors.LabelControl();
            this.lblTotalPrice = new DevExpress.XtraEditors.LabelControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.xtraTabControlMain = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPageMedicine = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlExpMestMedicine = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestMedicine = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ExpMestMedicine_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_MedicineTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_MedicineTypName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_ServiceUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_ImpMediStockName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_MobaAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_CanMobaAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_ImpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_BidNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_PackageNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_RegisterNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMedicine_ExpiredDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMediSpinMobaAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinMediMobaAmountDisable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.xtraTabPageMaterial = new DevExpress.XtraTab.XtraTabPage();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlExpMestMaterial = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestMaterial = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ExpMestMaterial_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_MaterialTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_MaterialTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_ServiceUnitName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_ImpMediStockName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_MobaAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_CanMobaAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_ImpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_BidNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_PackageNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_RegisterNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestMaterial_ExpiredDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinMateMobaAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemSpinMateMobaAmountDisable = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutTotalPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalFeePrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalVatPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTracking = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboImpMediStock.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControlMain)).BeginInit();
            this.xtraTabControlMain.SuspendLayout();
            this.xtraTabPageMedicine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMediSpinMobaAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMediMobaAmountDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.xtraTabPageMaterial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMateMobaAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMateMobaAmountDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalFeePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalVatPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboTracking);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.cboImpMediStock);
            this.layoutControl1.Controls.Add(this.lblTotalVatPrice);
            this.layoutControl1.Controls.Add(this.lblTotalFeePrice);
            this.layoutControl1.Controls.Add(this.lblTotalPrice);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.xtraTabControlMain);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 583);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboTracking
            // 
            this.cboTracking.Location = new System.Drawing.Point(388, 2);
            this.cboTracking.MenuManager = this.barManager1;
            this.cboTracking.Name = "cboTracking";
            this.cboTracking.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTracking.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cboTracking.Properties.NullText = "";
            this.cboTracking.Properties.View = this.gridView1;
            this.cboTracking.Size = new System.Drawing.Size(196, 20);
            this.cboTracking.StyleController = this.layoutControl1;
            this.cboTracking.TabIndex = 12;
            this.cboTracking.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTracking_ButtonClick);
            this.cboTracking.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboTracking_KeyUp);
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
            this.bbtnRCSave,
            this.bbtnRCPrint});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCPrint)});
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
            // bbtnRCPrint
            // 
            this.bbtnRCPrint.Caption = "In (Ctrl P)";
            this.bbtnRCPrint.Id = 1;
            this.bbtnRCPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnRCPrint.Name = "bbtnRCPrint";
            this.bbtnRCPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1100, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 612);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 583);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 583);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(683, 2);
            this.txtDescription.MenuManager = this.barManager1;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(344, 20);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 11;
            // 
            // cboImpMediStock
            // 
            this.cboImpMediStock.Location = new System.Drawing.Point(97, 2);
            this.cboImpMediStock.MenuManager = this.barManager1;
            this.cboImpMediStock.Name = "cboImpMediStock";
            this.cboImpMediStock.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboImpMediStock.Properties.NullText = "";
            this.cboImpMediStock.Properties.View = this.gridLookUpEdit1View;
            this.cboImpMediStock.Size = new System.Drawing.Size(192, 20);
            this.cboImpMediStock.StyleController = this.layoutControl1;
            this.cboImpMediStock.TabIndex = 10;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // lblTotalVatPrice
            // 
            this.lblTotalVatPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalVatPrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTotalVatPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalVatPrice.Location = new System.Drawing.Point(757, 559);
            this.lblTotalVatPrice.Name = "lblTotalVatPrice";
            this.lblTotalVatPrice.Size = new System.Drawing.Size(121, 20);
            this.lblTotalVatPrice.StyleController = this.layoutControl1;
            this.lblTotalVatPrice.TabIndex = 9;
            // 
            // lblTotalFeePrice
            // 
            this.lblTotalFeePrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalFeePrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTotalFeePrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalFeePrice.Location = new System.Drawing.Point(537, 559);
            this.lblTotalFeePrice.Name = "lblTotalFeePrice";
            this.lblTotalFeePrice.Size = new System.Drawing.Size(121, 20);
            this.lblTotalFeePrice.StyleController = this.layoutControl1;
            this.lblTotalFeePrice.TabIndex = 8;
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalPrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTotalPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalPrice.Location = new System.Drawing.Point(317, 559);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(121, 20);
            this.lblTotalPrice.StyleController = this.layoutControl1;
            this.lblTotalPrice.TabIndex = 7;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(992, 559);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(106, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(882, 559);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // xtraTabControlMain
            // 
            this.xtraTabControlMain.Location = new System.Drawing.Point(2, 26);
            this.xtraTabControlMain.Name = "xtraTabControlMain";
            this.xtraTabControlMain.SelectedTabPage = this.xtraTabPageMedicine;
            this.xtraTabControlMain.Size = new System.Drawing.Size(1096, 529);
            this.xtraTabControlMain.TabIndex = 4;
            this.xtraTabControlMain.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPageMedicine,
            this.xtraTabPageMaterial});
            // 
            // xtraTabPageMedicine
            // 
            this.xtraTabPageMedicine.Controls.Add(this.layoutControl2);
            this.xtraTabPageMedicine.Name = "xtraTabPageMedicine";
            this.xtraTabPageMedicine.Size = new System.Drawing.Size(1090, 501);
            this.xtraTabPageMedicine.Text = "Thuốc";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.gridControlExpMestMedicine);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(1090, 501);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // gridControlExpMestMedicine
            // 
            this.gridControlExpMestMedicine.Location = new System.Drawing.Point(0, 0);
            this.gridControlExpMestMedicine.MainView = this.gridViewExpMestMedicine;
            this.gridControlExpMestMedicine.Name = "gridControlExpMestMedicine";
            this.gridControlExpMestMedicine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMediSpinMobaAmount,
            this.repositoryItemSpinMediMobaAmountDisable});
            this.gridControlExpMestMedicine.Size = new System.Drawing.Size(1090, 501);
            this.gridControlExpMestMedicine.TabIndex = 4;
            this.gridControlExpMestMedicine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestMedicine});
            // 
            // gridViewExpMestMedicine
            // 
            this.gridViewExpMestMedicine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ExpMestMedicine_Stt,
            this.gridColumn_ExpMestMedicine_MedicineTypeCode,
            this.gridColumn_ExpMestMedicine_MedicineTypName,
            this.gridColumn_ExpMestMedicine_ServiceUnitName,
            this.gridColumn_ExpMestMedicine_ImpMediStockName,
            this.gridColumn_ExpMestMedicine_MobaAmount,
            this.gridColumn_ExpMestMedicine_Amount,
            this.gridColumn_ExpMestMedicine_CanMobaAmount,
            this.gridColumn_ExpMestMedicine_ImpPrice,
            this.gridColumn_ExpMestMedicine_VatRatio,
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice,
            this.gridColumn_ExpMestMedicine_BidNumber,
            this.gridColumn_ExpMestMedicine_PackageNumber,
            this.gridColumn_ExpMestMedicine_RegisterNumber,
            this.gridColumn_ExpMestMedicine_ExpiredDate});
            this.gridViewExpMestMedicine.GridControl = this.gridControlExpMestMedicine;
            this.gridViewExpMestMedicine.Name = "gridViewExpMestMedicine";
            this.gridViewExpMestMedicine.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewExpMestMedicine.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewExpMestMedicine.OptionsSelection.MultiSelect = true;
            this.gridViewExpMestMedicine.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewExpMestMedicine.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewExpMestMedicine.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewExpMestMedicine.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestMedicine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestMedicine.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestMedicine.OptionsView.ShowIndicator = false;
            this.gridViewExpMestMedicine.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewExpMestMedicine_CustomRowCellEdit);
            this.gridViewExpMestMedicine.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewExpMestMedicine_SelectionChanged);
            this.gridViewExpMestMedicine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewExpMestMedicine_CellValueChanged);
            this.gridViewExpMestMedicine.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestMedicine_CustomUnboundColumnData);
            // 
            // gridColumn_ExpMestMedicine_Stt
            // 
            this.gridColumn_ExpMestMedicine_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_Stt.Caption = "STT";
            this.gridColumn_ExpMestMedicine_Stt.FieldName = "STT";
            this.gridColumn_ExpMestMedicine_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMedicine_Stt.Name = "gridColumn_ExpMestMedicine_Stt";
            this.gridColumn_ExpMestMedicine_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_Stt.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_ExpMestMedicine_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMedicine_Stt.Visible = true;
            this.gridColumn_ExpMestMedicine_Stt.VisibleIndex = 1;
            this.gridColumn_ExpMestMedicine_Stt.Width = 30;
            // 
            // gridColumn_ExpMestMedicine_MedicineTypeCode
            // 
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.Caption = "Mã thuốc";
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.FieldName = "MEDICINE_TYPE_CODE";
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.Name = "gridColumn_ExpMestMedicine_MedicineTypeCode";
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.Visible = true;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.VisibleIndex = 2;
            this.gridColumn_ExpMestMedicine_MedicineTypeCode.Width = 60;
            // 
            // gridColumn_ExpMestMedicine_MedicineTypName
            // 
            this.gridColumn_ExpMestMedicine_MedicineTypName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_MedicineTypName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_MedicineTypName.Caption = "Tên thuốc";
            this.gridColumn_ExpMestMedicine_MedicineTypName.FieldName = "MEDICINE_TYPE_NAME";
            this.gridColumn_ExpMestMedicine_MedicineTypName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMedicine_MedicineTypName.Name = "gridColumn_ExpMestMedicine_MedicineTypName";
            this.gridColumn_ExpMestMedicine_MedicineTypName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_MedicineTypName.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_MedicineTypName.Visible = true;
            this.gridColumn_ExpMestMedicine_MedicineTypName.VisibleIndex = 3;
            this.gridColumn_ExpMestMedicine_MedicineTypName.Width = 250;
            // 
            // gridColumn_ExpMestMedicine_ServiceUnitName
            // 
            this.gridColumn_ExpMestMedicine_ServiceUnitName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.Caption = "Đơn vị tính";
            this.gridColumn_ExpMestMedicine_ServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn_ExpMestMedicine_ServiceUnitName.Name = "gridColumn_ExpMestMedicine_ServiceUnitName";
            this.gridColumn_ExpMestMedicine_ServiceUnitName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.Visible = true;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.VisibleIndex = 4;
            this.gridColumn_ExpMestMedicine_ServiceUnitName.Width = 80;
            // 
            // gridColumn_ExpMestMedicine_ImpMediStockName
            // 
            this.gridColumn_ExpMestMedicine_ImpMediStockName.Caption = "Kho nhập";
            this.gridColumn_ExpMestMedicine_ImpMediStockName.FieldName = "IMP_MEDI_STOCK_NAME";
            this.gridColumn_ExpMestMedicine_ImpMediStockName.Name = "gridColumn_ExpMestMedicine_ImpMediStockName";
            this.gridColumn_ExpMestMedicine_ImpMediStockName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_ImpMediStockName.Visible = true;
            this.gridColumn_ExpMestMedicine_ImpMediStockName.VisibleIndex = 5;
            this.gridColumn_ExpMestMedicine_ImpMediStockName.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_MobaAmount
            // 
            this.gridColumn_ExpMestMedicine_MobaAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_MobaAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_MobaAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_MobaAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_MobaAmount.Caption = "Số lượng thu hồi";
            this.gridColumn_ExpMestMedicine_MobaAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMedicine_MobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_MobaAmount.FieldName = "MOBA_AMOUNT";
            this.gridColumn_ExpMestMedicine_MobaAmount.Name = "gridColumn_ExpMestMedicine_MobaAmount";
            this.gridColumn_ExpMestMedicine_MobaAmount.Visible = true;
            this.gridColumn_ExpMestMedicine_MobaAmount.VisibleIndex = 6;
            this.gridColumn_ExpMestMedicine_MobaAmount.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_Amount
            // 
            this.gridColumn_ExpMestMedicine_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_Amount.Caption = "Số lượng xuất";
            this.gridColumn_ExpMestMedicine_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMedicine_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_Amount.FieldName = "AMOUNT";
            this.gridColumn_ExpMestMedicine_Amount.Name = "gridColumn_ExpMestMedicine_Amount";
            this.gridColumn_ExpMestMedicine_Amount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_Amount.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_Amount.Visible = true;
            this.gridColumn_ExpMestMedicine_Amount.VisibleIndex = 7;
            this.gridColumn_ExpMestMedicine_Amount.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_CanMobaAmount
            // 
            this.gridColumn_ExpMestMedicine_CanMobaAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.Caption = "Số lượng khả dụng";
            this.gridColumn_ExpMestMedicine_CanMobaAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMedicine_CanMobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.FieldName = "CAN_MOBA_AMOUNT";
            this.gridColumn_ExpMestMedicine_CanMobaAmount.Name = "gridColumn_ExpMestMedicine_CanMobaAmount";
            this.gridColumn_ExpMestMedicine_CanMobaAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.Visible = true;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.VisibleIndex = 8;
            this.gridColumn_ExpMestMedicine_CanMobaAmount.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_ImpPrice
            // 
            this.gridColumn_ExpMestMedicine_ImpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_ImpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_ImpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_ImpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_ImpPrice.Caption = "Giá xuất";
            this.gridColumn_ExpMestMedicine_ImpPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_ExpMestMedicine_ImpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_ImpPrice.FieldName = "PRICE";
            this.gridColumn_ExpMestMedicine_ImpPrice.Name = "gridColumn_ExpMestMedicine_ImpPrice";
            this.gridColumn_ExpMestMedicine_ImpPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_ImpPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_ImpPrice.Visible = true;
            this.gridColumn_ExpMestMedicine_ImpPrice.VisibleIndex = 9;
            this.gridColumn_ExpMestMedicine_ImpPrice.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_VatRatio
            // 
            this.gridColumn_ExpMestMedicine_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_VatRatio.Caption = "VAT (%)";
            this.gridColumn_ExpMestMedicine_VatRatio.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMedicine_VatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_VatRatio.FieldName = "VAT_RATIO_PLUS";
            this.gridColumn_ExpMestMedicine_VatRatio.Name = "gridColumn_ExpMestMedicine_VatRatio";
            this.gridColumn_ExpMestMedicine_VatRatio.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_VatRatio.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_VatRatio.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMedicine_VatRatio.Visible = true;
            this.gridColumn_ExpMestMedicine_VatRatio.VisibleIndex = 10;
            // 
            // gridColumn_ExpMestMedicine_VirTotalImpPrice
            // 
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Caption = "Thành tiền";
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.FieldName = "VIR_TOTAL_IMP_PRICE";
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Name = "gridColumn_ExpMestMedicine_VirTotalImpPrice";
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Visible = true;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.VisibleIndex = 11;
            this.gridColumn_ExpMestMedicine_VirTotalImpPrice.Width = 100;
            // 
            // gridColumn_ExpMestMedicine_BidNumber
            // 
            this.gridColumn_ExpMestMedicine_BidNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_BidNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_BidNumber.Caption = "Gói thầu";
            this.gridColumn_ExpMestMedicine_BidNumber.FieldName = "BID_NUMBER";
            this.gridColumn_ExpMestMedicine_BidNumber.Name = "gridColumn_ExpMestMedicine_BidNumber";
            this.gridColumn_ExpMestMedicine_BidNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_BidNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_BidNumber.Visible = true;
            this.gridColumn_ExpMestMedicine_BidNumber.VisibleIndex = 12;
            // 
            // gridColumn_ExpMestMedicine_PackageNumber
            // 
            this.gridColumn_ExpMestMedicine_PackageNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_PackageNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_PackageNumber.Caption = "Số lô";
            this.gridColumn_ExpMestMedicine_PackageNumber.FieldName = "PACKAGE_NUMBER";
            this.gridColumn_ExpMestMedicine_PackageNumber.Name = "gridColumn_ExpMestMedicine_PackageNumber";
            this.gridColumn_ExpMestMedicine_PackageNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_PackageNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_PackageNumber.Visible = true;
            this.gridColumn_ExpMestMedicine_PackageNumber.VisibleIndex = 13;
            // 
            // gridColumn_ExpMestMedicine_RegisterNumber
            // 
            this.gridColumn_ExpMestMedicine_RegisterNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_RegisterNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_RegisterNumber.Caption = "Số đăng ký";
            this.gridColumn_ExpMestMedicine_RegisterNumber.FieldName = "REGISTER_NUMBER";
            this.gridColumn_ExpMestMedicine_RegisterNumber.Name = "gridColumn_ExpMestMedicine_RegisterNumber";
            this.gridColumn_ExpMestMedicine_RegisterNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_RegisterNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_RegisterNumber.Visible = true;
            this.gridColumn_ExpMestMedicine_RegisterNumber.VisibleIndex = 14;
            // 
            // gridColumn_ExpMestMedicine_ExpiredDate
            // 
            this.gridColumn_ExpMestMedicine_ExpiredDate.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_ExpiredDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_ExpiredDate.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMedicine_ExpiredDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMedicine_ExpiredDate.Caption = "Hạn sửa dụng";
            this.gridColumn_ExpMestMedicine_ExpiredDate.FieldName = "EXPIRED_DATE_STR";
            this.gridColumn_ExpMestMedicine_ExpiredDate.Name = "gridColumn_ExpMestMedicine_ExpiredDate";
            this.gridColumn_ExpMestMedicine_ExpiredDate.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMedicine_ExpiredDate.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMedicine_ExpiredDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMedicine_ExpiredDate.Visible = true;
            this.gridColumn_ExpMestMedicine_ExpiredDate.VisibleIndex = 15;
            this.gridColumn_ExpMestMedicine_ExpiredDate.Width = 90;
            // 
            // repositoryItemMediSpinMobaAmount
            // 
            this.repositoryItemMediSpinMobaAmount.AutoHeight = false;
            this.repositoryItemMediSpinMobaAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemMediSpinMobaAmount.DisplayFormat.FormatString = "#,##0.00";
            this.repositoryItemMediSpinMobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemMediSpinMobaAmount.EditFormat.FormatString = "#,##0.00";
            this.repositoryItemMediSpinMobaAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemMediSpinMobaAmount.MaxValue = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.repositoryItemMediSpinMobaAmount.Name = "repositoryItemMediSpinMobaAmount";
            // 
            // repositoryItemSpinMediMobaAmountDisable
            // 
            this.repositoryItemSpinMediMobaAmountDisable.AutoHeight = false;
            this.repositoryItemSpinMediMobaAmountDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemSpinMediMobaAmountDisable.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMediMobaAmountDisable.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMediMobaAmountDisable.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMediMobaAmountDisable.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMediMobaAmountDisable.Name = "repositoryItemSpinMediMobaAmountDisable";
            this.repositoryItemSpinMediMobaAmountDisable.ReadOnly = true;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1090, 501);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlExpMestMedicine;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(1090, 501);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // xtraTabPageMaterial
            // 
            this.xtraTabPageMaterial.Controls.Add(this.layoutControl3);
            this.xtraTabPageMaterial.Name = "xtraTabPageMaterial";
            this.xtraTabPageMaterial.Size = new System.Drawing.Size(1090, 501);
            this.xtraTabPageMaterial.Text = "Vật tư";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.gridControlExpMestMaterial);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(1090, 501);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // gridControlExpMestMaterial
            // 
            this.gridControlExpMestMaterial.Location = new System.Drawing.Point(0, 0);
            this.gridControlExpMestMaterial.MainView = this.gridViewExpMestMaterial;
            this.gridControlExpMestMaterial.Name = "gridControlExpMestMaterial";
            this.gridControlExpMestMaterial.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpinMateMobaAmount,
            this.repositoryItemSpinMateMobaAmountDisable});
            this.gridControlExpMestMaterial.Size = new System.Drawing.Size(1090, 501);
            this.gridControlExpMestMaterial.TabIndex = 4;
            this.gridControlExpMestMaterial.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestMaterial});
            // 
            // gridViewExpMestMaterial
            // 
            this.gridViewExpMestMaterial.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ExpMestMaterial_Stt,
            this.gridColumn_ExpMestMaterial_MaterialTypeCode,
            this.gridColumn_ExpMestMaterial_MaterialTypeName,
            this.gridColumn_ExpMestMaterial_ServiceUnitName,
            this.gridColumn_ExpMestMaterial_ImpMediStockName,
            this.gridColumn_ExpMestMaterial_MobaAmount,
            this.gridColumn_ExpMestMaterial_Amount,
            this.gridColumn_ExpMestMaterial_CanMobaAmount,
            this.gridColumn_ExpMestMaterial_ImpPrice,
            this.gridColumn_ExpMestMaterial_VatRatio,
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice,
            this.gridColumn_ExpMestMaterial_BidNumber,
            this.gridColumn_ExpMestMaterial_PackageNumber,
            this.gridColumn_ExpMestMaterial_RegisterNumber,
            this.gridColumn_ExpMestMaterial_ExpiredDate});
            this.gridViewExpMestMaterial.GridControl = this.gridControlExpMestMaterial;
            this.gridViewExpMestMaterial.Name = "gridViewExpMestMaterial";
            this.gridViewExpMestMaterial.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewExpMestMaterial.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewExpMestMaterial.OptionsSelection.MultiSelect = true;
            this.gridViewExpMestMaterial.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewExpMestMaterial.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewExpMestMaterial.OptionsView.ColumnAutoWidth = false;
            this.gridViewExpMestMaterial.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExpMestMaterial.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestMaterial.OptionsView.ShowIndicator = false;
            this.gridViewExpMestMaterial.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewExpMestMaterial_CustomRowCellEdit);
            this.gridViewExpMestMaterial.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewExpMestMaterial_SelectionChanged);
            this.gridViewExpMestMaterial.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewExpMestMaterial_CellValueChanged);
            this.gridViewExpMestMaterial.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestMaterial_CustomUnboundColumnData);
            // 
            // gridColumn_ExpMestMaterial_Stt
            // 
            this.gridColumn_ExpMestMaterial_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_Stt.Caption = "STT";
            this.gridColumn_ExpMestMaterial_Stt.FieldName = "STT";
            this.gridColumn_ExpMestMaterial_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMaterial_Stt.Name = "gridColumn_ExpMestMaterial_Stt";
            this.gridColumn_ExpMestMaterial_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_Stt.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_ExpMestMaterial_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMaterial_Stt.Visible = true;
            this.gridColumn_ExpMestMaterial_Stt.VisibleIndex = 1;
            this.gridColumn_ExpMestMaterial_Stt.Width = 30;
            // 
            // gridColumn_ExpMestMaterial_MaterialTypeCode
            // 
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.Caption = "Mã vật tư";
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.FieldName = "MATERIAL_TYPE_CODE";
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.Name = "gridColumn_ExpMestMaterial_MaterialTypeCode";
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.Visible = true;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.VisibleIndex = 2;
            this.gridColumn_ExpMestMaterial_MaterialTypeCode.Width = 60;
            // 
            // gridColumn_ExpMestMaterial_MaterialTypeName
            // 
            this.gridColumn_ExpMestMaterial_MaterialTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.Caption = "Tên vật tư";
            this.gridColumn_ExpMestMaterial_MaterialTypeName.FieldName = "MATERIAL_TYPE_NAME";
            this.gridColumn_ExpMestMaterial_MaterialTypeName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.Name = "gridColumn_ExpMestMaterial_MaterialTypeName";
            this.gridColumn_ExpMestMaterial_MaterialTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.Visible = true;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.VisibleIndex = 3;
            this.gridColumn_ExpMestMaterial_MaterialTypeName.Width = 250;
            // 
            // gridColumn_ExpMestMaterial_ServiceUnitName
            // 
            this.gridColumn_ExpMestMaterial_ServiceUnitName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.Caption = "Đơn vị tính";
            this.gridColumn_ExpMestMaterial_ServiceUnitName.FieldName = "SERVICE_UNIT_NAME";
            this.gridColumn_ExpMestMaterial_ServiceUnitName.Name = "gridColumn_ExpMestMaterial_ServiceUnitName";
            this.gridColumn_ExpMestMaterial_ServiceUnitName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.Visible = true;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.VisibleIndex = 4;
            this.gridColumn_ExpMestMaterial_ServiceUnitName.Width = 80;
            // 
            // gridColumn_ExpMestMaterial_ImpMediStockName
            // 
            this.gridColumn_ExpMestMaterial_ImpMediStockName.Caption = "Kho nhập";
            this.gridColumn_ExpMestMaterial_ImpMediStockName.FieldName = "IMP_MEDI_STOCK_NAME";
            this.gridColumn_ExpMestMaterial_ImpMediStockName.Name = "gridColumn_ExpMestMaterial_ImpMediStockName";
            this.gridColumn_ExpMestMaterial_ImpMediStockName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_ImpMediStockName.Visible = true;
            this.gridColumn_ExpMestMaterial_ImpMediStockName.VisibleIndex = 5;
            this.gridColumn_ExpMestMaterial_ImpMediStockName.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_MobaAmount
            // 
            this.gridColumn_ExpMestMaterial_MobaAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_MobaAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_MobaAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_MobaAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_MobaAmount.Caption = "Số lượng thu hồi";
            this.gridColumn_ExpMestMaterial_MobaAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMaterial_MobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_MobaAmount.FieldName = "MOBA_AMOUNT";
            this.gridColumn_ExpMestMaterial_MobaAmount.Name = "gridColumn_ExpMestMaterial_MobaAmount";
            this.gridColumn_ExpMestMaterial_MobaAmount.Visible = true;
            this.gridColumn_ExpMestMaterial_MobaAmount.VisibleIndex = 6;
            this.gridColumn_ExpMestMaterial_MobaAmount.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_Amount
            // 
            this.gridColumn_ExpMestMaterial_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_Amount.Caption = "Số lượng xuất";
            this.gridColumn_ExpMestMaterial_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMaterial_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_Amount.FieldName = "AMOUNT";
            this.gridColumn_ExpMestMaterial_Amount.Name = "gridColumn_ExpMestMaterial_Amount";
            this.gridColumn_ExpMestMaterial_Amount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_Amount.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_Amount.Visible = true;
            this.gridColumn_ExpMestMaterial_Amount.VisibleIndex = 7;
            this.gridColumn_ExpMestMaterial_Amount.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_CanMobaAmount
            // 
            this.gridColumn_ExpMestMaterial_CanMobaAmount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.Caption = "Số lượng khả dụng";
            this.gridColumn_ExpMestMaterial_CanMobaAmount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMaterial_CanMobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.FieldName = "CAN_MOBA_AMOUNT";
            this.gridColumn_ExpMestMaterial_CanMobaAmount.Name = "gridColumn_ExpMestMaterial_CanMobaAmount";
            this.gridColumn_ExpMestMaterial_CanMobaAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.Visible = true;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.VisibleIndex = 8;
            this.gridColumn_ExpMestMaterial_CanMobaAmount.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_ImpPrice
            // 
            this.gridColumn_ExpMestMaterial_ImpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_ImpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_ImpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_ImpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_ImpPrice.Caption = "Giá xuất";
            this.gridColumn_ExpMestMaterial_ImpPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_ExpMestMaterial_ImpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_ImpPrice.FieldName = "PRICE";
            this.gridColumn_ExpMestMaterial_ImpPrice.Name = "gridColumn_ExpMestMaterial_ImpPrice";
            this.gridColumn_ExpMestMaterial_ImpPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_ImpPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_ImpPrice.Visible = true;
            this.gridColumn_ExpMestMaterial_ImpPrice.VisibleIndex = 9;
            this.gridColumn_ExpMestMaterial_ImpPrice.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_VatRatio
            // 
            this.gridColumn_ExpMestMaterial_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_VatRatio.Caption = "VAT (%)";
            this.gridColumn_ExpMestMaterial_VatRatio.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestMaterial_VatRatio.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_VatRatio.FieldName = "VAT_RATIO_PLUS";
            this.gridColumn_ExpMestMaterial_VatRatio.Name = "gridColumn_ExpMestMaterial_VatRatio";
            this.gridColumn_ExpMestMaterial_VatRatio.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_VatRatio.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_VatRatio.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMaterial_VatRatio.Visible = true;
            this.gridColumn_ExpMestMaterial_VatRatio.VisibleIndex = 10;
            // 
            // gridColumn_ExpMestMaterial_VirTotalImpPrice
            // 
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Caption = "Thành tiền";
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.FieldName = "VIR_TOTAL_IMP_PRICE";
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Name = "gridColumn_ExpMestMaterial_VirTotalImpPrice";
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Visible = true;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.VisibleIndex = 11;
            this.gridColumn_ExpMestMaterial_VirTotalImpPrice.Width = 100;
            // 
            // gridColumn_ExpMestMaterial_BidNumber
            // 
            this.gridColumn_ExpMestMaterial_BidNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_BidNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_BidNumber.Caption = "Gói thầu";
            this.gridColumn_ExpMestMaterial_BidNumber.FieldName = "BID_NUMBER";
            this.gridColumn_ExpMestMaterial_BidNumber.Name = "gridColumn_ExpMestMaterial_BidNumber";
            this.gridColumn_ExpMestMaterial_BidNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_BidNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_BidNumber.Visible = true;
            this.gridColumn_ExpMestMaterial_BidNumber.VisibleIndex = 12;
            // 
            // gridColumn_ExpMestMaterial_PackageNumber
            // 
            this.gridColumn_ExpMestMaterial_PackageNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_PackageNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_PackageNumber.Caption = "Số lô";
            this.gridColumn_ExpMestMaterial_PackageNumber.FieldName = "PACKAGE_NUMBER";
            this.gridColumn_ExpMestMaterial_PackageNumber.Name = "gridColumn_ExpMestMaterial_PackageNumber";
            this.gridColumn_ExpMestMaterial_PackageNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_PackageNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_PackageNumber.Visible = true;
            this.gridColumn_ExpMestMaterial_PackageNumber.VisibleIndex = 13;
            // 
            // gridColumn_ExpMestMaterial_RegisterNumber
            // 
            this.gridColumn_ExpMestMaterial_RegisterNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_RegisterNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_RegisterNumber.Caption = "Số đăng ký";
            this.gridColumn_ExpMestMaterial_RegisterNumber.FieldName = "REGISTER_NUMBER";
            this.gridColumn_ExpMestMaterial_RegisterNumber.Name = "gridColumn_ExpMestMaterial_RegisterNumber";
            this.gridColumn_ExpMestMaterial_RegisterNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_RegisterNumber.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_RegisterNumber.Visible = true;
            this.gridColumn_ExpMestMaterial_RegisterNumber.VisibleIndex = 14;
            // 
            // gridColumn_ExpMestMaterial_ExpiredDate
            // 
            this.gridColumn_ExpMestMaterial_ExpiredDate.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_ExpiredDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_ExpiredDate.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestMaterial_ExpiredDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestMaterial_ExpiredDate.Caption = "Hạn sử dụng";
            this.gridColumn_ExpMestMaterial_ExpiredDate.FieldName = "EXPIRED_DATE_STR";
            this.gridColumn_ExpMestMaterial_ExpiredDate.Name = "gridColumn_ExpMestMaterial_ExpiredDate";
            this.gridColumn_ExpMestMaterial_ExpiredDate.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestMaterial_ExpiredDate.OptionsColumn.AllowFocus = false;
            this.gridColumn_ExpMestMaterial_ExpiredDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestMaterial_ExpiredDate.Visible = true;
            this.gridColumn_ExpMestMaterial_ExpiredDate.VisibleIndex = 15;
            this.gridColumn_ExpMestMaterial_ExpiredDate.Width = 90;
            // 
            // repositoryItemSpinMateMobaAmount
            // 
            this.repositoryItemSpinMateMobaAmount.AutoHeight = false;
            this.repositoryItemSpinMateMobaAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemSpinMateMobaAmount.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMateMobaAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMateMobaAmount.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMateMobaAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMateMobaAmount.Name = "repositoryItemSpinMateMobaAmount";
            // 
            // repositoryItemSpinMateMobaAmountDisable
            // 
            this.repositoryItemSpinMateMobaAmountDisable.AutoHeight = false;
            this.repositoryItemSpinMateMobaAmountDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.repositoryItemSpinMateMobaAmountDisable.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMateMobaAmountDisable.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMateMobaAmountDisable.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinMateMobaAmountDisable.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinMateMobaAmountDisable.Name = "repositoryItemSpinMateMobaAmountDisable";
            this.repositoryItemSpinMateMobaAmountDisable.ReadOnly = true;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1090, 501);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.gridControlExpMestMaterial;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Size = new System.Drawing.Size(1090, 501);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutTotalPrice,
            this.layoutTotalFeePrice,
            this.layoutTotalVatPrice,
            this.layoutControlItem5,
            this.emptySpaceItem2,
            this.layoutControlItem7,
            this.lciTracking});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 583);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.xtraTabControlMain;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1100, 533);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(880, 557);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnPrint;
            this.layoutControlItem3.Location = new System.Drawing.Point(990, 557);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 557);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(220, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutTotalPrice
            // 
            this.layoutTotalPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutTotalPrice.AppearanceItemCaption.Options.UseFont = true;
            this.layoutTotalPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalPrice.Control = this.lblTotalPrice;
            this.layoutTotalPrice.Location = new System.Drawing.Point(220, 557);
            this.layoutTotalPrice.Name = "layoutTotalPrice";
            this.layoutTotalPrice.Size = new System.Drawing.Size(220, 26);
            this.layoutTotalPrice.Text = "Tổng tiền:";
            this.layoutTotalPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalPrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTotalPrice.TextToControlDistance = 5;
            // 
            // layoutTotalFeePrice
            // 
            this.layoutTotalFeePrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutTotalFeePrice.AppearanceItemCaption.Options.UseFont = true;
            this.layoutTotalFeePrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalFeePrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalFeePrice.Control = this.lblTotalFeePrice;
            this.layoutTotalFeePrice.Location = new System.Drawing.Point(440, 557);
            this.layoutTotalFeePrice.Name = "layoutTotalFeePrice";
            this.layoutTotalFeePrice.Size = new System.Drawing.Size(220, 26);
            this.layoutTotalFeePrice.Text = "Tiền vốn:";
            this.layoutTotalFeePrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalFeePrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTotalFeePrice.TextToControlDistance = 5;
            // 
            // layoutTotalVatPrice
            // 
            this.layoutTotalVatPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutTotalVatPrice.AppearanceItemCaption.Options.UseFont = true;
            this.layoutTotalVatPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalVatPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalVatPrice.Control = this.lblTotalVatPrice;
            this.layoutTotalVatPrice.Location = new System.Drawing.Point(660, 557);
            this.layoutTotalVatPrice.Name = "layoutTotalVatPrice";
            this.layoutTotalVatPrice.Size = new System.Drawing.Size(220, 26);
            this.layoutTotalVatPrice.Text = "Tiền thuế:";
            this.layoutTotalVatPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalVatPrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTotalVatPrice.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.cboImpMediStock;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(291, 24);
            this.layoutControlItem5.Text = "Kho nhập:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(1029, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(71, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.txtDescription;
            this.layoutControlItem7.Location = new System.Drawing.Point(586, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(443, 24);
            this.layoutControlItem7.Text = "Lý do:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // lciTracking
            // 
            this.lciTracking.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTracking.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTracking.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTracking.Control = this.cboTracking;
            this.lciTracking.Location = new System.Drawing.Point(291, 0);
            this.lciTracking.Name = "lciTracking";
            this.lciTracking.Size = new System.Drawing.Size(295, 24);
            this.lciTracking.Text = "Tờ điều trị:";
            this.lciTracking.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTracking.TextSize = new System.Drawing.Size(90, 20);
            this.lciTracking.TextToControlDistance = 5;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // dxValidationProviderEditorInfo
            // 
            this.dxValidationProviderEditorInfo.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProviderEditorInfo_ValidationFailed);
            // 
            // frmMobaCabinetCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 612);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmMobaCabinetCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nhập đơn tủ trực trả lại";
            this.Load += new System.EventHandler(this.frmMobaCabinetCreate_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboImpMediStock.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControlMain)).EndInit();
            this.xtraTabControlMain.ResumeLayout(false);
            this.xtraTabPageMedicine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMediSpinMobaAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMediMobaAmountDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.xtraTabPageMaterial.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMateMobaAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinMateMobaAmountDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalFeePrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalVatPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraTab.XtraTabControl xtraTabControlMain;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageMedicine;
        private DevExpress.XtraTab.XtraTabPage xtraTabPageMaterial;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraGrid.GridControl gridControlExpMestMedicine;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestMedicine;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_MedicineTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_MedicineTypName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_ServiceUnitName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_MobaAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_CanMobaAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_ImpPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_VatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_VirTotalImpPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_PackageNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_BidNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_RegisterNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_ExpiredDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemMediSpinMobaAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMediMobaAmountDisable;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraGrid.GridControl gridControlExpMestMaterial;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestMaterial;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_MaterialTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_MaterialTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_ServiceUnitName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_MobaAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_CanMobaAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_ImpPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_VatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_VirTotalImpPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_PackageNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_BidNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_RegisterNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_ExpiredDate;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMateMobaAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinMateMobaAmountDisable;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarButtonItem bbtnRCPrint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.LabelControl lblTotalVatPrice;
        private DevExpress.XtraEditors.LabelControl lblTotalFeePrice;
        private DevExpress.XtraEditors.LabelControl lblTotalPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalFeePrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalVatPrice;
        private DevExpress.XtraEditors.GridLookUpEdit cboImpMediStock;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMedicine_ImpMediStockName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestMaterial_ImpMediStockName;
        private DevExpress.XtraEditors.GridLookUpEdit cboTracking;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciTracking;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
    }
}
