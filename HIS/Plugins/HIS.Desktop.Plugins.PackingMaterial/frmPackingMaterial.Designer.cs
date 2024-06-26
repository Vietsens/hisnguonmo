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
namespace HIS.Desktop.Plugins.PackingMaterial
{
    partial class frmPackingMaterial
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject17 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject18 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject19 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject20 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblCpServiceUnitName = new DevExpress.XtraEditors.LabelControl();
            this.lblTpServiceUnitName = new DevExpress.XtraEditors.LabelControl();
            this.popupControlContainerChePham = new DevExpress.XtraBars.PopupControlContainer();
            this.gridControlPopupChePham = new DevExpress.XtraGrid.GridControl();
            this.gridViewPopupChePham = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDinhMuc = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.popupControlContainerThanhPham = new DevExpress.XtraBars.PopupControlContainer();
            this.gridControlPopupThanhPham = new DevExpress.XtraGrid.GridControl();
            this.gridViewPopupThanhPham = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlExpImpMest = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpImpMest = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_ExpImpMest_Type = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_ExpImpMest_Code = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControlMaterialPaty = new DevExpress.XtraGrid.GridControl();
            this.gridViewMaterialPaty = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_MaterialPaty_PatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_MaterialPaty_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpin_Paty__Price = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.Gc_MaterialPaty_VatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpin_Paty__Vat = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridControlPrepa = new Inventec.Desktop.CustomControl.MyGridControl();
            this.gridViewPrepa = new Inventec.Desktop.CustomControl.MyGridView();
            this.Gc_ChePham_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_ChePham_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton__Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Gc_ChePham_MaterialTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_ChePham_MaterialTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_ChePham_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpin__Amount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.Gc_ChePham_ServiceUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeleteUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMATERIAL_TYPE_CODEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMATERIAL_TYPE_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAMOUNTUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSERVICE_UNIT_NAMEUnb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnDinhMuc = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.spinCPAmount = new DevExpress.XtraEditors.SpinEdit();
            this.txtChePham = new DevExpress.XtraEditors.ButtonEdit();
            this.txtDocumentNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtPackageNumber = new DevExpress.XtraEditors.TextEdit();
            this.dtExpTime = new DevExpress.XtraEditors.DateEdit();
            this.spinTPAmount = new DevExpress.XtraEditors.SpinEdit();
            this.txtThanhPham = new DevExpress.XtraEditors.ButtonEdit();
            this.txtMediStockName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMediStock = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciThanhPham = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTPAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDocumentNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPackageNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChePham = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCPAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerChePham)).BeginInit();
            this.popupControlContainerChePham.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupChePham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupChePham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerThanhPham)).BeginInit();
            this.popupControlContainerThanhPham.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupThanhPham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupThanhPham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpImpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpImpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaterialPaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaterialPaty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin_Paty__Price)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin_Paty__Vat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrepa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrepa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin__Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinCPAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChePham.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocumentNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackageNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTPAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThanhPham.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediStockName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThanhPham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTPAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDocumentNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPackageNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChePham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCPAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblCpServiceUnitName);
            this.layoutControl1.Controls.Add(this.lblTpServiceUnitName);
            this.layoutControl1.Controls.Add(this.popupControlContainerChePham);
            this.layoutControl1.Controls.Add(this.popupControlContainerThanhPham);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.gridControlExpImpMest);
            this.layoutControl1.Controls.Add(this.gridControlMaterialPaty);
            this.layoutControl1.Controls.Add(this.gridControlPrepa);
            this.layoutControl1.Controls.Add(this.btnDinhMuc);
            this.layoutControl1.Controls.Add(this.btnAdd);
            this.layoutControl1.Controls.Add(this.spinCPAmount);
            this.layoutControl1.Controls.Add(this.txtChePham);
            this.layoutControl1.Controls.Add(this.txtDocumentNumber);
            this.layoutControl1.Controls.Add(this.txtPackageNumber);
            this.layoutControl1.Controls.Add(this.dtExpTime);
            this.layoutControl1.Controls.Add(this.spinTPAmount);
            this.layoutControl1.Controls.Add(this.txtThanhPham);
            this.layoutControl1.Controls.Add(this.txtMediStockName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(990, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblCpServiceUnitName
            // 
            this.lblCpServiceUnitName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCpServiceUnitName.Location = new System.Drawing.Point(652, 50);
            this.lblCpServiceUnitName.Name = "lblCpServiceUnitName";
            this.lblCpServiceUnitName.Size = new System.Drawing.Size(116, 13);
            this.lblCpServiceUnitName.StyleController = this.layoutControl1;
            this.lblCpServiceUnitName.TabIndex = 23;
            // 
            // lblTpServiceUnitName
            // 
            this.lblTpServiceUnitName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTpServiceUnitName.Location = new System.Drawing.Point(892, 2);
            this.lblTpServiceUnitName.Name = "lblTpServiceUnitName";
            this.lblTpServiceUnitName.Size = new System.Drawing.Size(96, 13);
            this.lblTpServiceUnitName.StyleController = this.layoutControl1;
            this.lblTpServiceUnitName.TabIndex = 22;
            // 
            // popupControlContainerChePham
            // 
            this.popupControlContainerChePham.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.popupControlContainerChePham.Controls.Add(this.gridControlPopupChePham);
            this.popupControlContainerChePham.Location = new System.Drawing.Point(230, 141);
            this.popupControlContainerChePham.Manager = this.barManager1;
            this.popupControlContainerChePham.Name = "popupControlContainerChePham";
            this.popupControlContainerChePham.Size = new System.Drawing.Size(409, 36);
            this.popupControlContainerChePham.TabIndex = 21;
            this.popupControlContainerChePham.Visible = false;
            this.popupControlContainerChePham.CloseUp += new System.EventHandler(this.popupControlContainerChePham_CloseUp);
            // 
            // gridControlPopupChePham
            // 
            this.gridControlPopupChePham.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPopupChePham.Location = new System.Drawing.Point(0, 0);
            this.gridControlPopupChePham.MainView = this.gridViewPopupChePham;
            this.gridControlPopupChePham.MenuManager = this.barManager1;
            this.gridControlPopupChePham.Name = "gridControlPopupChePham";
            this.gridControlPopupChePham.Size = new System.Drawing.Size(409, 36);
            this.gridControlPopupChePham.TabIndex = 0;
            this.gridControlPopupChePham.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPopupChePham});
            // 
            // gridViewPopupChePham
            // 
            this.gridViewPopupChePham.GridControl = this.gridControlPopupChePham;
            this.gridViewPopupChePham.Name = "gridViewPopupChePham";
            this.gridViewPopupChePham.OptionsBehavior.Editable = false;
            this.gridViewPopupChePham.OptionsView.ShowGroupPanel = false;
            this.gridViewPopupChePham.OptionsView.ShowIndicator = false;
            this.gridViewPopupChePham.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewPopupChePham_RowClick);
            this.gridViewPopupChePham.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewPopupChePham_KeyDown);
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
            this.barBtnSave,
            this.barBtnPrint,
            this.barBtnNew,
            this.barBtnAdd,
            this.barBtnDinhMuc});
            this.barManager1.MaxItemId = 5;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnDinhMuc)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSave
            // 
            this.barBtnSave.Caption = "Lưu (Ctrl S)";
            this.barBtnSave.Id = 0;
            this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnSave.Name = "barBtnSave";
            this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
            // 
            // barBtnPrint
            // 
            this.barBtnPrint.Caption = "In (Ctrl P)";
            this.barBtnPrint.Id = 1;
            this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnPrint.Name = "barBtnPrint";
            this.barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            // 
            // barBtnNew
            // 
            this.barBtnNew.Caption = "Mới (Ctrl N)";
            this.barBtnNew.Id = 2;
            this.barBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barBtnNew.Name = "barBtnNew";
            this.barBtnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnNew_ItemClick);
            // 
            // barBtnAdd
            // 
            this.barBtnAdd.Caption = "Bổ sung (Ctrl A)";
            this.barBtnAdd.Id = 3;
            this.barBtnAdd.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barBtnAdd.Name = "barBtnAdd";
            this.barBtnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnAdd_ItemClick);
            // 
            // barBtnDinhMuc
            // 
            this.barBtnDinhMuc.Caption = "Định mức (Ctrl D)";
            this.barBtnDinhMuc.Id = 4;
            this.barBtnDinhMuc.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.barBtnDinhMuc.Name = "barBtnDinhMuc";
            this.barBtnDinhMuc.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnDinhMuc_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(990, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(990, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // popupControlContainerThanhPham
            // 
            this.popupControlContainerThanhPham.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.popupControlContainerThanhPham.Controls.Add(this.gridControlPopupThanhPham);
            this.popupControlContainerThanhPham.Location = new System.Drawing.Point(230, 183);
            this.popupControlContainerThanhPham.Manager = this.barManager1;
            this.popupControlContainerThanhPham.Name = "popupControlContainerThanhPham";
            this.popupControlContainerThanhPham.Size = new System.Drawing.Size(409, 37);
            this.popupControlContainerThanhPham.TabIndex = 20;
            this.popupControlContainerThanhPham.Visible = false;
            this.popupControlContainerThanhPham.CloseUp += new System.EventHandler(this.popupControlContainerThanhPham_CloseUp);
            // 
            // gridControlPopupThanhPham
            // 
            this.gridControlPopupThanhPham.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPopupThanhPham.Location = new System.Drawing.Point(0, 0);
            this.gridControlPopupThanhPham.MainView = this.gridViewPopupThanhPham;
            this.gridControlPopupThanhPham.MenuManager = this.barManager1;
            this.gridControlPopupThanhPham.Name = "gridControlPopupThanhPham";
            this.gridControlPopupThanhPham.Size = new System.Drawing.Size(409, 37);
            this.gridControlPopupThanhPham.TabIndex = 0;
            this.gridControlPopupThanhPham.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPopupThanhPham});
            // 
            // gridViewPopupThanhPham
            // 
            this.gridViewPopupThanhPham.GridControl = this.gridControlPopupThanhPham;
            this.gridViewPopupThanhPham.Name = "gridViewPopupThanhPham";
            this.gridViewPopupThanhPham.OptionsBehavior.Editable = false;
            this.gridViewPopupThanhPham.OptionsView.ShowGroupPanel = false;
            this.gridViewPopupThanhPham.OptionsView.ShowIndicator = false;
            this.gridViewPopupThanhPham.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewPopupThanhPham_RowClick);
            this.gridViewPopupThanhPham.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewPopupThanhPham_KeyDown);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(869, 508);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(119, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 19;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(745, 508);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(120, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 18;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(621, 508);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridControlExpImpMest
            // 
            this.gridControlExpImpMest.Location = new System.Drawing.Point(495, 332);
            this.gridControlExpImpMest.MainView = this.gridViewExpImpMest;
            this.gridControlExpImpMest.Name = "gridControlExpImpMest";
            this.gridControlExpImpMest.Size = new System.Drawing.Size(495, 174);
            this.gridControlExpImpMest.TabIndex = 16;
            this.gridControlExpImpMest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpImpMest});
            // 
            // gridViewExpImpMest
            // 
            this.gridViewExpImpMest.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_ExpImpMest_Type,
            this.Gc_ExpImpMest_Code});
            this.gridViewExpImpMest.GridControl = this.gridControlExpImpMest;
            this.gridViewExpImpMest.GroupPanelText = "Danh sách phiếu nhập/xuất đóng gói";
            this.gridViewExpImpMest.Name = "gridViewExpImpMest";
            this.gridViewExpImpMest.OptionsView.ShowIndicator = false;
            // 
            // Gc_ExpImpMest_Type
            // 
            this.Gc_ExpImpMest_Type.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ExpImpMest_Type.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ExpImpMest_Type.Caption = "Loại phiếu";
            this.Gc_ExpImpMest_Type.FieldName = "TYPE_NAME";
            this.Gc_ExpImpMest_Type.Name = "Gc_ExpImpMest_Type";
            this.Gc_ExpImpMest_Type.Visible = true;
            this.Gc_ExpImpMest_Type.VisibleIndex = 0;
            this.Gc_ExpImpMest_Type.Width = 235;
            // 
            // Gc_ExpImpMest_Code
            // 
            this.Gc_ExpImpMest_Code.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ExpImpMest_Code.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ExpImpMest_Code.Caption = "Mã phiếu";
            this.Gc_ExpImpMest_Code.FieldName = "ExpImpCode";
            this.Gc_ExpImpMest_Code.Name = "Gc_ExpImpMest_Code";
            this.Gc_ExpImpMest_Code.Visible = true;
            this.Gc_ExpImpMest_Code.VisibleIndex = 1;
            this.Gc_ExpImpMest_Code.Width = 200;
            // 
            // gridControlMaterialPaty
            // 
            this.gridControlMaterialPaty.Location = new System.Drawing.Point(0, 332);
            this.gridControlMaterialPaty.MainView = this.gridViewMaterialPaty;
            this.gridControlMaterialPaty.Name = "gridControlMaterialPaty";
            this.gridControlMaterialPaty.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpin_Paty__Price,
            this.repositoryItemSpin_Paty__Vat});
            this.gridControlMaterialPaty.Size = new System.Drawing.Size(495, 174);
            this.gridControlMaterialPaty.TabIndex = 15;
            this.gridControlMaterialPaty.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMaterialPaty});
            // 
            // gridViewMaterialPaty
            // 
            this.gridViewMaterialPaty.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_MaterialPaty_PatientType,
            this.Gc_MaterialPaty_Price,
            this.Gc_MaterialPaty_VatRatio});
            this.gridViewMaterialPaty.GridControl = this.gridControlMaterialPaty;
            this.gridViewMaterialPaty.GroupPanelText = "Danh sách giá theo đối tượng";
            this.gridViewMaterialPaty.Name = "gridViewMaterialPaty";
            this.gridViewMaterialPaty.OptionsView.ShowIndicator = false;
            this.gridViewMaterialPaty.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewMaterialPaty_CellValueChanged);
            // 
            // Gc_MaterialPaty_PatientType
            // 
            this.Gc_MaterialPaty_PatientType.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_MaterialPaty_PatientType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_MaterialPaty_PatientType.Caption = "Đối tượng";
            this.Gc_MaterialPaty_PatientType.FieldName = "PATIENT_TYPE_NAME";
            this.Gc_MaterialPaty_PatientType.Name = "Gc_MaterialPaty_PatientType";
            this.Gc_MaterialPaty_PatientType.OptionsColumn.AllowEdit = false;
            this.Gc_MaterialPaty_PatientType.OptionsColumn.ReadOnly = true;
            this.Gc_MaterialPaty_PatientType.Visible = true;
            this.Gc_MaterialPaty_PatientType.VisibleIndex = 0;
            this.Gc_MaterialPaty_PatientType.Width = 195;
            // 
            // Gc_MaterialPaty_Price
            // 
            this.Gc_MaterialPaty_Price.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_MaterialPaty_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_MaterialPaty_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_MaterialPaty_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_MaterialPaty_Price.Caption = "Giá";
            this.Gc_MaterialPaty_Price.ColumnEdit = this.repositoryItemSpin_Paty__Price;
            this.Gc_MaterialPaty_Price.FieldName = "PRICE";
            this.Gc_MaterialPaty_Price.Name = "Gc_MaterialPaty_Price";
            this.Gc_MaterialPaty_Price.Visible = true;
            this.Gc_MaterialPaty_Price.VisibleIndex = 1;
            this.Gc_MaterialPaty_Price.Width = 120;
            // 
            // repositoryItemSpin_Paty__Price
            // 
            this.repositoryItemSpin_Paty__Price.AutoHeight = false;
            this.repositoryItemSpin_Paty__Price.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpin_Paty__Price.Name = "repositoryItemSpin_Paty__Price";
            // 
            // Gc_MaterialPaty_VatRatio
            // 
            this.Gc_MaterialPaty_VatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_MaterialPaty_VatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_MaterialPaty_VatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_MaterialPaty_VatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_MaterialPaty_VatRatio.Caption = "Vat (%)";
            this.Gc_MaterialPaty_VatRatio.ColumnEdit = this.repositoryItemSpin_Paty__Vat;
            this.Gc_MaterialPaty_VatRatio.FieldName = "VAT";
            this.Gc_MaterialPaty_VatRatio.Name = "Gc_MaterialPaty_VatRatio";
            this.Gc_MaterialPaty_VatRatio.Visible = true;
            this.Gc_MaterialPaty_VatRatio.VisibleIndex = 2;
            this.Gc_MaterialPaty_VatRatio.Width = 120;
            // 
            // repositoryItemSpin_Paty__Vat
            // 
            this.repositoryItemSpin_Paty__Vat.AutoHeight = false;
            this.repositoryItemSpin_Paty__Vat.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpin_Paty__Vat.Name = "repositoryItemSpin_Paty__Vat";
            // 
            // gridControlPrepa
            // 
            this.gridControlPrepa.Location = new System.Drawing.Point(0, 74);
            this.gridControlPrepa.MainView = this.gridViewPrepa;
            this.gridControlPrepa.Name = "gridControlPrepa";
            this.gridControlPrepa.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSpin__Amount,
            this.repositoryItemButton__Delete});
            this.gridControlPrepa.Size = new System.Drawing.Size(990, 258);
            this.gridControlPrepa.TabIndex = 14;
            this.gridControlPrepa.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPrepa});
            // 
            // gridViewPrepa
            // 
            this.gridViewPrepa.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_ChePham_Stt,
            this.Gc_ChePham_Delete,
            this.Gc_ChePham_MaterialTypeCode,
            this.Gc_ChePham_MaterialTypeName,
            this.Gc_ChePham_Amount,
            this.Gc_ChePham_ServiceUnit,
            this.colSTTUnb,
            this.colDeleteUnb,
            this.colMATERIAL_TYPE_CODEUnb,
            this.colMATERIAL_TYPE_NAMEUnb,
            this.colAMOUNTUnb,
            this.colSERVICE_UNIT_NAMEUnb});
            this.gridViewPrepa.GridControl = this.gridControlPrepa;
            this.gridViewPrepa.Name = "gridViewPrepa";
            this.gridViewPrepa.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
            this.gridViewPrepa.OptionsView.ShowGroupPanel = false;
            this.gridViewPrepa.OptionsView.ShowIndicator = false;
            this.gridViewPrepa.CustomRowColumnError += new System.EventHandler<Inventec.Desktop.CustomControl.RowColumnErrorEventArgs>(this.gridViewPrepa_CustomRowColumnError);
            this.gridViewPrepa.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewPrepa_RowCellStyle);
            this.gridViewPrepa.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewPrepa_CellValueChanged);
            this.gridViewPrepa.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPrepa_CustomUnboundColumnData);
            // 
            // Gc_ChePham_Stt
            // 
            this.Gc_ChePham_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_ChePham_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_ChePham_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ChePham_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ChePham_Stt.Caption = "STT";
            this.Gc_ChePham_Stt.FieldName = "STT";
            this.Gc_ChePham_Stt.FieldNameSortGroup = "STTUnb";
            this.Gc_ChePham_Stt.Name = "Gc_ChePham_Stt";
            this.Gc_ChePham_Stt.OptionsColumn.AllowEdit = false;
            this.Gc_ChePham_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_ChePham_Stt.OptionsColumn.ReadOnly = true;
            this.Gc_ChePham_Stt.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_ChePham_Stt.Visible = true;
            this.Gc_ChePham_Stt.VisibleIndex = 0;
            this.Gc_ChePham_Stt.Width = 40;
            // 
            // Gc_ChePham_Delete
            // 
            this.Gc_ChePham_Delete.Caption = "Xóa";
            this.Gc_ChePham_Delete.ColumnEdit = this.repositoryItemButton__Delete;
            this.Gc_ChePham_Delete.FieldName = "Delete";
            this.Gc_ChePham_Delete.FieldNameSortGroup = "DeleteUnb";
            this.Gc_ChePham_Delete.Name = "Gc_ChePham_Delete";
            this.Gc_ChePham_Delete.OptionsColumn.ShowCaption = false;
            this.Gc_ChePham_Delete.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_Delete.Visible = true;
            this.Gc_ChePham_Delete.VisibleIndex = 1;
            this.Gc_ChePham_Delete.Width = 20;
            // 
            // repositoryItemButton__Delete
            // 
            this.repositoryItemButton__Delete.AutoHeight = false;
            this.repositoryItemButton__Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject17, serializableAppearanceObject18, serializableAppearanceObject19, serializableAppearanceObject20, "Xóa", null, null, true)});
            this.repositoryItemButton__Delete.Name = "repositoryItemButton__Delete";
            this.repositoryItemButton__Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton__Delete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__Delete_ButtonClick);
            // 
            // Gc_ChePham_MaterialTypeCode
            // 
            this.Gc_ChePham_MaterialTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ChePham_MaterialTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ChePham_MaterialTypeCode.Caption = "Mã chế phẩm";
            this.Gc_ChePham_MaterialTypeCode.FieldName = "MATERIAL_TYPE_CODE";
            this.Gc_ChePham_MaterialTypeCode.FieldNameSortGroup = "MATERIAL_TYPE_CODEUnb";
            this.Gc_ChePham_MaterialTypeCode.Name = "Gc_ChePham_MaterialTypeCode";
            this.Gc_ChePham_MaterialTypeCode.OptionsColumn.AllowEdit = false;
            this.Gc_ChePham_MaterialTypeCode.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_MaterialTypeCode.Visible = true;
            this.Gc_ChePham_MaterialTypeCode.VisibleIndex = 2;
            this.Gc_ChePham_MaterialTypeCode.Width = 120;
            // 
            // Gc_ChePham_MaterialTypeName
            // 
            this.Gc_ChePham_MaterialTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ChePham_MaterialTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ChePham_MaterialTypeName.Caption = "Tên chế phẩm";
            this.Gc_ChePham_MaterialTypeName.FieldName = "MATERIAL_TYPE_NAME";
            this.Gc_ChePham_MaterialTypeName.FieldNameSortGroup = "MATERIAL_TYPE_NAMEUnb";
            this.Gc_ChePham_MaterialTypeName.Name = "Gc_ChePham_MaterialTypeName";
            this.Gc_ChePham_MaterialTypeName.OptionsColumn.AllowEdit = false;
            this.Gc_ChePham_MaterialTypeName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_MaterialTypeName.Visible = true;
            this.Gc_ChePham_MaterialTypeName.VisibleIndex = 3;
            this.Gc_ChePham_MaterialTypeName.Width = 330;
            // 
            // Gc_ChePham_Amount
            // 
            this.Gc_ChePham_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_ChePham_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_ChePham_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ChePham_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ChePham_Amount.Caption = "Số lượng";
            this.Gc_ChePham_Amount.ColumnEdit = this.repositoryItemSpin__Amount;
            this.Gc_ChePham_Amount.FieldName = "AMOUNT";
            this.Gc_ChePham_Amount.FieldNameSortGroup = "AMOUNTUnb";
            this.Gc_ChePham_Amount.Name = "Gc_ChePham_Amount";
            this.Gc_ChePham_Amount.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_Amount.Visible = true;
            this.Gc_ChePham_Amount.VisibleIndex = 4;
            this.Gc_ChePham_Amount.Width = 200;
            // 
            // repositoryItemSpin__Amount
            // 
            this.repositoryItemSpin__Amount.AutoHeight = false;
            this.repositoryItemSpin__Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpin__Amount.Mask.EditMask = "f2";
            this.repositoryItemSpin__Amount.MaxValue = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
            this.repositoryItemSpin__Amount.Name = "repositoryItemSpin__Amount";
            // 
            // Gc_ChePham_ServiceUnit
            // 
            this.Gc_ChePham_ServiceUnit.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ChePham_ServiceUnit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ChePham_ServiceUnit.Caption = "Đơn vị tính";
            this.Gc_ChePham_ServiceUnit.FieldName = "SERVICE_UNIT_NAME";
            this.Gc_ChePham_ServiceUnit.FieldNameSortGroup = "SERVICE_UNIT_NAMEUnb";
            this.Gc_ChePham_ServiceUnit.Name = "Gc_ChePham_ServiceUnit";
            this.Gc_ChePham_ServiceUnit.OptionsColumn.AllowEdit = false;
            this.Gc_ChePham_ServiceUnit.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
            this.Gc_ChePham_ServiceUnit.Visible = true;
            this.Gc_ChePham_ServiceUnit.VisibleIndex = 5;
            this.Gc_ChePham_ServiceUnit.Width = 165;
            // 
            // colSTTUnb
            // 
            this.colSTTUnb.FieldName = "STTUnb";
            this.colSTTUnb.Name = "colSTTUnb";
            this.colSTTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colDeleteUnb
            // 
            this.colDeleteUnb.FieldName = "DeleteUnb";
            this.colDeleteUnb.Name = "colDeleteUnb";
            this.colDeleteUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMATERIAL_TYPE_CODEUnb
            // 
            this.colMATERIAL_TYPE_CODEUnb.FieldName = "MATERIAL_TYPE_CODEUnb";
            this.colMATERIAL_TYPE_CODEUnb.Name = "colMATERIAL_TYPE_CODEUnb";
            this.colMATERIAL_TYPE_CODEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colMATERIAL_TYPE_NAMEUnb
            // 
            this.colMATERIAL_TYPE_NAMEUnb.FieldName = "MATERIAL_TYPE_NAMEUnb";
            this.colMATERIAL_TYPE_NAMEUnb.Name = "colMATERIAL_TYPE_NAMEUnb";
            this.colMATERIAL_TYPE_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colAMOUNTUnb
            // 
            this.colAMOUNTUnb.FieldName = "AMOUNTUnb";
            this.colAMOUNTUnb.Name = "colAMOUNTUnb";
            this.colAMOUNTUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // colSERVICE_UNIT_NAMEUnb
            // 
            this.colSERVICE_UNIT_NAMEUnb.FieldName = "SERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.Name = "colSERVICE_UNIT_NAMEUnb";
            this.colSERVICE_UNIT_NAMEUnb.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // 
            // btnDinhMuc
            // 
            this.btnDinhMuc.Location = new System.Drawing.Point(882, 50);
            this.btnDinhMuc.Name = "btnDinhMuc";
            this.btnDinhMuc.Size = new System.Drawing.Size(106, 22);
            this.btnDinhMuc.StyleController = this.layoutControl1;
            this.btnDinhMuc.TabIndex = 13;
            this.btnDinhMuc.Text = "Định mức (Ctrl D)";
            this.btnDinhMuc.Click += new System.EventHandler(this.btnDinhMuc_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(772, 50);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(106, 22);
            this.btnAdd.StyleController = this.layoutControl1;
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "Bổ sung (Ctrl A)";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // spinCPAmount
            // 
            this.spinCPAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinCPAmount.Location = new System.Drawing.Point(497, 50);
            this.spinCPAmount.Name = "spinCPAmount";
            this.spinCPAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinCPAmount.Properties.Mask.EditMask = "f2";
            this.spinCPAmount.Properties.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.spinCPAmount.Size = new System.Drawing.Size(151, 20);
            this.spinCPAmount.StyleController = this.layoutControl1;
            this.spinCPAmount.TabIndex = 11;
            this.spinCPAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinCPAmount_PreviewKeyDown);
            // 
            // txtChePham
            // 
            this.txtChePham.Location = new System.Drawing.Point(97, 50);
            this.txtChePham.Name = "txtChePham";
            this.txtChePham.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
            this.txtChePham.Size = new System.Drawing.Size(301, 20);
            this.txtChePham.StyleController = this.layoutControl1;
            this.txtChePham.TabIndex = 10;
            this.txtChePham.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtChePham_ButtonClick);
            this.txtChePham.TextChanged += new System.EventHandler(this.txtChePham_TextChanged);
            this.txtChePham.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtChePham_KeyDown);
            this.txtChePham.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtChePham_PreviewKeyDown);
            // 
            // txtDocumentNumber
            // 
            this.txtDocumentNumber.Location = new System.Drawing.Point(747, 26);
            this.txtDocumentNumber.Name = "txtDocumentNumber";
            this.txtDocumentNumber.Size = new System.Drawing.Size(241, 20);
            this.txtDocumentNumber.StyleController = this.layoutControl1;
            this.txtDocumentNumber.TabIndex = 9;
            this.txtDocumentNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDocumentNumber_PreviewKeyDown);
            // 
            // txtPackageNumber
            // 
            this.txtPackageNumber.Location = new System.Drawing.Point(347, 26);
            this.txtPackageNumber.Name = "txtPackageNumber";
            this.txtPackageNumber.Size = new System.Drawing.Size(301, 20);
            this.txtPackageNumber.StyleController = this.layoutControl1;
            this.txtPackageNumber.TabIndex = 8;
            this.txtPackageNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPackageNumber_PreviewKeyDown);
            // 
            // dtExpTime
            // 
            this.dtExpTime.EditValue = null;
            this.dtExpTime.Location = new System.Drawing.Point(97, 26);
            this.dtExpTime.Name = "dtExpTime";
            this.dtExpTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpTime.Size = new System.Drawing.Size(151, 20);
            this.dtExpTime.StyleController = this.layoutControl1;
            this.dtExpTime.TabIndex = 7;
            this.dtExpTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtExpTime_PreviewKeyDown);
            // 
            // spinTPAmount
            // 
            this.spinTPAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinTPAmount.Location = new System.Drawing.Point(747, 2);
            this.spinTPAmount.Name = "spinTPAmount";
            this.spinTPAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinTPAmount.Properties.MaxValue = new decimal(new int[] {
            -727379969,
            232,
            0,
            0});
            this.spinTPAmount.Size = new System.Drawing.Size(141, 20);
            this.spinTPAmount.StyleController = this.layoutControl1;
            this.spinTPAmount.TabIndex = 6;
            this.spinTPAmount.EditValueChanged += new System.EventHandler(this.spinTPAmount_EditValueChanged);
            this.spinTPAmount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinTPAmount_PreviewKeyDown);
            // 
            // txtThanhPham
            // 
            this.txtThanhPham.Location = new System.Drawing.Point(347, 2);
            this.txtThanhPham.Name = "txtThanhPham";
            this.txtThanhPham.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
            this.txtThanhPham.Size = new System.Drawing.Size(301, 20);
            this.txtThanhPham.StyleController = this.layoutControl1;
            this.txtThanhPham.TabIndex = 5;
            this.txtThanhPham.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtThanhPham_ButtonClick);
            this.txtThanhPham.TextChanged += new System.EventHandler(this.txtThanhPham_TextChanged);
            this.txtThanhPham.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtThanhPham_KeyDown);
            this.txtThanhPham.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtThanhPham_PreviewKeyDown);
            // 
            // txtMediStockName
            // 
            this.txtMediStockName.Location = new System.Drawing.Point(97, 2);
            this.txtMediStockName.Name = "txtMediStockName";
            this.txtMediStockName.Properties.ReadOnly = true;
            this.txtMediStockName.Size = new System.Drawing.Size(151, 20);
            this.txtMediStockName.StyleController = this.layoutControl1;
            this.txtMediStockName.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMediStock,
            this.lciThanhPham,
            this.lciTPAmount,
            this.lciExpTime,
            this.lciDocumentNumber,
            this.lciPackageNumber,
            this.lciChePham,
            this.lciCPAmount,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.emptySpaceItem1,
            this.layoutControlItem16,
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(990, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciMediStock
            // 
            this.lciMediStock.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciMediStock.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciMediStock.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediStock.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediStock.Control = this.txtMediStockName;
            this.lciMediStock.Location = new System.Drawing.Point(0, 0);
            this.lciMediStock.Name = "lciMediStock";
            this.lciMediStock.Size = new System.Drawing.Size(250, 24);
            this.lciMediStock.Text = "Kho đóng gói:";
            this.lciMediStock.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediStock.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediStock.TextToControlDistance = 5;
            // 
            // lciThanhPham
            // 
            this.lciThanhPham.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciThanhPham.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciThanhPham.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciThanhPham.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciThanhPham.Control = this.txtThanhPham;
            this.lciThanhPham.Location = new System.Drawing.Point(250, 0);
            this.lciThanhPham.Name = "lciThanhPham";
            this.lciThanhPham.OptionsToolTip.ToolTip = "Vật tư thành phẩm";
            this.lciThanhPham.Size = new System.Drawing.Size(400, 24);
            this.lciThanhPham.Text = "Vật tư TP:";
            this.lciThanhPham.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciThanhPham.TextSize = new System.Drawing.Size(90, 20);
            this.lciThanhPham.TextToControlDistance = 5;
            // 
            // lciTPAmount
            // 
            this.lciTPAmount.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciTPAmount.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTPAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTPAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTPAmount.Control = this.spinTPAmount;
            this.lciTPAmount.Location = new System.Drawing.Point(650, 0);
            this.lciTPAmount.Name = "lciTPAmount";
            this.lciTPAmount.Size = new System.Drawing.Size(240, 24);
            this.lciTPAmount.Text = "Số lượng:";
            this.lciTPAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTPAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lciTPAmount.TextToControlDistance = 5;
            // 
            // lciExpTime
            // 
            this.lciExpTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpTime.Control = this.dtExpTime;
            this.lciExpTime.Location = new System.Drawing.Point(0, 24);
            this.lciExpTime.Name = "lciExpTime";
            this.lciExpTime.Size = new System.Drawing.Size(250, 24);
            this.lciExpTime.Text = "Hạn sử dụng:";
            this.lciExpTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpTime.TextToControlDistance = 5;
            // 
            // lciDocumentNumber
            // 
            this.lciDocumentNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDocumentNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDocumentNumber.Control = this.txtDocumentNumber;
            this.lciDocumentNumber.Location = new System.Drawing.Point(650, 24);
            this.lciDocumentNumber.Name = "lciDocumentNumber";
            this.lciDocumentNumber.Size = new System.Drawing.Size(340, 24);
            this.lciDocumentNumber.Text = "Số công văn:";
            this.lciDocumentNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDocumentNumber.TextSize = new System.Drawing.Size(90, 20);
            this.lciDocumentNumber.TextToControlDistance = 5;
            // 
            // lciPackageNumber
            // 
            this.lciPackageNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPackageNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPackageNumber.Control = this.txtPackageNumber;
            this.lciPackageNumber.Location = new System.Drawing.Point(250, 24);
            this.lciPackageNumber.Name = "lciPackageNumber";
            this.lciPackageNumber.Size = new System.Drawing.Size(400, 24);
            this.lciPackageNumber.Text = "Số lô:";
            this.lciPackageNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPackageNumber.TextSize = new System.Drawing.Size(90, 20);
            this.lciPackageNumber.TextToControlDistance = 5;
            // 
            // lciChePham
            // 
            this.lciChePham.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChePham.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChePham.Control = this.txtChePham;
            this.lciChePham.Location = new System.Drawing.Point(0, 48);
            this.lciChePham.Name = "lciChePham";
            this.lciChePham.Size = new System.Drawing.Size(400, 26);
            this.lciChePham.Text = "Chế phẩm:";
            this.lciChePham.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChePham.TextSize = new System.Drawing.Size(90, 20);
            this.lciChePham.TextToControlDistance = 5;
            // 
            // lciCPAmount
            // 
            this.lciCPAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCPAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCPAmount.Control = this.spinCPAmount;
            this.lciCPAmount.Location = new System.Drawing.Point(400, 48);
            this.lciCPAmount.Name = "lciCPAmount";
            this.lciCPAmount.Size = new System.Drawing.Size(250, 26);
            this.lciCPAmount.Text = "Số lượng:";
            this.lciCPAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCPAmount.TextSize = new System.Drawing.Size(90, 20);
            this.lciCPAmount.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnAdd;
            this.layoutControlItem9.Location = new System.Drawing.Point(770, 48);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnDinhMuc;
            this.layoutControlItem10.Location = new System.Drawing.Point(880, 48);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.gridControlPrepa;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 74);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem11.Size = new System.Drawing.Size(990, 258);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.gridControlMaterialPaty;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 332);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem12.Size = new System.Drawing.Size(495, 174);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.gridControlExpImpMest;
            this.layoutControlItem13.Location = new System.Drawing.Point(495, 332);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem13.Size = new System.Drawing.Size(495, 174);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.btnSave;
            this.layoutControlItem14.Location = new System.Drawing.Point(619, 506);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(124, 26);
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.btnPrint;
            this.layoutControlItem15.Location = new System.Drawing.Point(743, 506);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(124, 26);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 506);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(619, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.btnNew;
            this.layoutControlItem16.Location = new System.Drawing.Point(867, 506);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(123, 26);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblTpServiceUnitName;
            this.layoutControlItem1.Location = new System.Drawing.Point(890, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(100, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.lblCpServiceUnitName;
            this.layoutControlItem2.Location = new System.Drawing.Point(650, 48);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(120, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxValidationProvider2
            // 
            this.dxValidationProvider2.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider2_ValidationFailed);
            // 
            // frmPackingMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmPackingMaterial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đóng gói vật tư";
            this.Load += new System.EventHandler(this.frmPackingMaterial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerChePham)).EndInit();
            this.popupControlContainerChePham.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupChePham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupChePham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupControlContainerThanhPham)).EndInit();
            this.popupControlContainerThanhPham.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPopupThanhPham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPopupThanhPham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpImpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpImpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlMaterialPaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaterialPaty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin_Paty__Price)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin_Paty__Vat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPrepa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPrepa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__Delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpin__Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinCPAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChePham.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDocumentNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPackageNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinTPAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThanhPham.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediStockName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThanhPham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTPAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDocumentNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPackageNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChePham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCPAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtMediStockName;
        private DevExpress.XtraLayout.LayoutControlItem lciMediStock;
        private DevExpress.XtraEditors.SpinEdit spinTPAmount;
        private DevExpress.XtraEditors.ButtonEdit txtThanhPham;
        private DevExpress.XtraLayout.LayoutControlItem lciThanhPham;
        private DevExpress.XtraLayout.LayoutControlItem lciTPAmount;
        private DevExpress.XtraEditors.DateEdit dtExpTime;
        private DevExpress.XtraLayout.LayoutControlItem lciExpTime;
        private DevExpress.XtraEditors.TextEdit txtPackageNumber;
        private DevExpress.XtraLayout.LayoutControlItem lciPackageNumber;
        private DevExpress.XtraEditors.TextEdit txtDocumentNumber;
        private DevExpress.XtraLayout.LayoutControlItem lciDocumentNumber;
        private DevExpress.XtraEditors.ButtonEdit txtChePham;
        private DevExpress.XtraLayout.LayoutControlItem lciChePham;
        private DevExpress.XtraEditors.SpinEdit spinCPAmount;
        private DevExpress.XtraLayout.LayoutControlItem lciCPAmount;
        private DevExpress.XtraEditors.SimpleButton btnDinhMuc;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridControlExpImpMest;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpImpMest;
        private DevExpress.XtraGrid.GridControl gridControlMaterialPaty;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaterialPaty;
        private Inventec.Desktop.CustomControl.MyGridControl gridControlPrepa;
        private Inventec.Desktop.CustomControl.MyGridView gridViewPrepa;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.PopupControlContainer popupControlContainerChePham;
        private DevExpress.XtraGrid.GridControl gridControlPopupChePham;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPopupChePham;
        private DevExpress.XtraBars.PopupControlContainer popupControlContainerThanhPham;
        private DevExpress.XtraGrid.GridControl gridControlPopupThanhPham;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPopupThanhPham;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_Delete;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_MaterialTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_MaterialTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_Amount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpin__Amount;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ChePham_ServiceUnit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton__Delete;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_MaterialPaty_PatientType;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_MaterialPaty_Price;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpin_Paty__Price;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_MaterialPaty_VatRatio;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpin_Paty__Vat;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ExpImpMest_Type;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ExpImpMest_Code;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraBars.BarButtonItem barBtnNew;
        private DevExpress.XtraBars.BarButtonItem barBtnAdd;
        private DevExpress.XtraBars.BarButtonItem barBtnDinhMuc;
        private DevExpress.XtraGrid.Columns.GridColumn colSTTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colDeleteUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colMATERIAL_TYPE_CODEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colMATERIAL_TYPE_NAMEUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colAMOUNTUnb;
        private DevExpress.XtraGrid.Columns.GridColumn colSERVICE_UNIT_NAMEUnb;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
        private DevExpress.XtraEditors.LabelControl lblTpServiceUnitName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.LabelControl lblCpServiceUnitName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
