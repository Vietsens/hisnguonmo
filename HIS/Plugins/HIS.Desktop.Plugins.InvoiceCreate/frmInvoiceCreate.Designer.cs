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
namespace HIS.Desktop.Plugins.InvoiceCreate
{
    partial class frmInvoiceCreate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInvoiceCreate));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnCreateElectricInvoice = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlInvoiceDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewInvoiceDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_InvoiceDetail_GoodsName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTxtGoodsName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn_InvoiceDetail_GoodsUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTxtGoodsUnit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.gridColumn_InvoiceDetail_Price = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinPrice = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_InvoiceDetail_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_InvoiceDetail_Discount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinDiscount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn_InvoiceDetail_TotalPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_InvoiceDetail_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.txtVatRatio = new DevExpress.XtraEditors.SpinEdit();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtPayFormCode = new DevExpress.XtraEditors.TextEdit();
            this.txtNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.txtSellerAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtSellerAccountNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtSellerPhone = new DevExpress.XtraEditors.TextEdit();
            this.txtSellerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.txtSellerName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlSellerInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutSellerName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutSellerTaxCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutSellerPhone = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutSellerAccountNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutSellerAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.txtBuyerDescription = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerOrganization = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerAccountNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.txtBuyerName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlBuyerInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutBuyerName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBuyerTaxCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBuyerAccountNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBuyerOrganization = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBuyerAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBuyerDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtDiscount = new DevExpress.XtraEditors.SpinEdit();
            this.txtAmount = new DevExpress.XtraEditors.SpinEdit();
            this.dtInvoiceTime = new DevExpress.XtraEditors.DateEdit();
            this.cboPayForm = new DevExpress.XtraEditors.LookUpEdit();
            this.txtTongTuDen = new DevExpress.XtraEditors.TextEdit();
            this.cboInvoiceBook = new DevExpress.XtraEditors.LookUpEdit();
            this.txtTemplateCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutInvoiceBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTongTuDen = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutInvoiceTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDiscount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPayForm = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutVatRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCNew = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlInvoiceDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewInvoiceDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTxtGoodsName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTxtGoodsUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVatRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerAccountNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlSellerInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerTaxCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerPhone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerAccountNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOrganization.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlBuyerInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerTaxCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerAccountNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerOrganization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTongTuDen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplateCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInvoiceBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTongTuDen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInvoiceTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVatRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnCreateElectricInvoice);
            this.layoutControl1.Controls.Add(this.gridControlInvoiceDetail);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.txtVatRatio);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtPayFormCode);
            this.layoutControl1.Controls.Add(this.txtNumOrder);
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.txtDiscount);
            this.layoutControl1.Controls.Add(this.txtAmount);
            this.layoutControl1.Controls.Add(this.dtInvoiceTime);
            this.layoutControl1.Controls.Add(this.cboPayForm);
            this.layoutControl1.Controls.Add(this.txtTongTuDen);
            this.layoutControl1.Controls.Add(this.cboInvoiceBook);
            this.layoutControl1.Controls.Add(this.txtTemplateCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 583);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnCreateElectricInvoice
            // 
            this.btnCreateElectricInvoice.Location = new System.Drawing.Point(433, 559);
            this.btnCreateElectricInvoice.Name = "btnCreateElectricInvoice";
            this.btnCreateElectricInvoice.Size = new System.Drawing.Size(143, 22);
            this.btnCreateElectricInvoice.StyleController = this.layoutControl1;
            this.btnCreateElectricInvoice.TabIndex = 30;
            this.btnCreateElectricInvoice.Text = "Tạo hóa đơn điện tử (Ctrl B)";
            this.btnCreateElectricInvoice.Click += new System.EventHandler(this.btnCreateElectricInvoice_Click);
            // 
            // gridControlInvoiceDetail
            // 
            this.gridControlInvoiceDetail.Location = new System.Drawing.Point(0, 0);
            this.gridControlInvoiceDetail.MainView = this.gridViewInvoiceDetail;
            this.gridControlInvoiceDetail.Name = "gridControlInvoiceDetail";
            this.gridControlInvoiceDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTxtGoodsName,
            this.repositoryItemTxtGoodsUnit,
            this.repositoryItemSpinPrice,
            this.repositoryItemSpinAmount,
            this.repositoryItemSpinDiscount,
            this.repositoryItemBtnDelete});
            this.gridControlInvoiceDetail.Size = new System.Drawing.Size(880, 338);
            this.gridControlInvoiceDetail.TabIndex = 29;
            this.gridControlInvoiceDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewInvoiceDetail});
            // 
            // gridViewInvoiceDetail
            // 
            this.gridViewInvoiceDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_InvoiceDetail_GoodsName,
            this.gridColumn_InvoiceDetail_GoodsUnit,
            this.gridColumn_InvoiceDetail_Price,
            this.gridColumn_InvoiceDetail_Amount,
            this.gridColumn_InvoiceDetail_Discount,
            this.gridColumn_InvoiceDetail_TotalPrice,
            this.gridColumn_InvoiceDetail_Delete});
            this.gridViewInvoiceDetail.GridControl = this.gridControlInvoiceDetail;
            this.gridViewInvoiceDetail.Name = "gridViewInvoiceDetail";
            this.gridViewInvoiceDetail.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewInvoiceDetail.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewInvoiceDetail.OptionsCustomization.AllowColumnResizing = false;
            this.gridViewInvoiceDetail.OptionsMenu.EnableColumnMenu = false;
            this.gridViewInvoiceDetail.OptionsNavigation.AutoFocusNewRow = true;
            this.gridViewInvoiceDetail.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridViewInvoiceDetail.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.gridViewInvoiceDetail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewInvoiceDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewInvoiceDetail.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gridViewInvoiceDetail_InitNewRow);
            this.gridViewInvoiceDetail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewInvoiceDetail_CellValueChanged);
            this.gridViewInvoiceDetail.InvalidRowException += new DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventHandler(this.gridViewInvoiceDetail_InvalidRowException);
            this.gridViewInvoiceDetail.ValidateRow += new DevExpress.XtraGrid.Views.Base.ValidateRowEventHandler(this.gridViewInvoiceDetail_ValidateRow);
            // 
            // gridColumn_InvoiceDetail_GoodsName
            // 
            this.gridColumn_InvoiceDetail_GoodsName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_GoodsName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_GoodsName.Caption = "Tên hàng hóa";
            this.gridColumn_InvoiceDetail_GoodsName.ColumnEdit = this.repositoryItemTxtGoodsName;
            this.gridColumn_InvoiceDetail_GoodsName.FieldName = "GOODS_NAME";
            this.gridColumn_InvoiceDetail_GoodsName.Name = "gridColumn_InvoiceDetail_GoodsName";
            this.gridColumn_InvoiceDetail_GoodsName.Visible = true;
            this.gridColumn_InvoiceDetail_GoodsName.VisibleIndex = 0;
            this.gridColumn_InvoiceDetail_GoodsName.Width = 265;
            // 
            // repositoryItemTxtGoodsName
            // 
            this.repositoryItemTxtGoodsName.AutoHeight = false;
            this.repositoryItemTxtGoodsName.MaxLength = 500;
            this.repositoryItemTxtGoodsName.Name = "repositoryItemTxtGoodsName";
            // 
            // gridColumn_InvoiceDetail_GoodsUnit
            // 
            this.gridColumn_InvoiceDetail_GoodsUnit.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_GoodsUnit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_GoodsUnit.Caption = "Đơn vị tính";
            this.gridColumn_InvoiceDetail_GoodsUnit.ColumnEdit = this.repositoryItemTxtGoodsUnit;
            this.gridColumn_InvoiceDetail_GoodsUnit.FieldName = "GOODS_UNIT";
            this.gridColumn_InvoiceDetail_GoodsUnit.Name = "gridColumn_InvoiceDetail_GoodsUnit";
            this.gridColumn_InvoiceDetail_GoodsUnit.Visible = true;
            this.gridColumn_InvoiceDetail_GoodsUnit.VisibleIndex = 1;
            this.gridColumn_InvoiceDetail_GoodsUnit.Width = 100;
            // 
            // repositoryItemTxtGoodsUnit
            // 
            this.repositoryItemTxtGoodsUnit.AutoHeight = false;
            this.repositoryItemTxtGoodsUnit.MaxLength = 100;
            this.repositoryItemTxtGoodsUnit.Name = "repositoryItemTxtGoodsUnit";
            // 
            // gridColumn_InvoiceDetail_Price
            // 
            this.gridColumn_InvoiceDetail_Price.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Price.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_InvoiceDetail_Price.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Price.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_Price.Caption = "Đơn giá";
            this.gridColumn_InvoiceDetail_Price.ColumnEdit = this.repositoryItemSpinPrice;
            this.gridColumn_InvoiceDetail_Price.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_InvoiceDetail_Price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_InvoiceDetail_Price.FieldName = "PRICE";
            this.gridColumn_InvoiceDetail_Price.Name = "gridColumn_InvoiceDetail_Price";
            this.gridColumn_InvoiceDetail_Price.Visible = true;
            this.gridColumn_InvoiceDetail_Price.VisibleIndex = 2;
            this.gridColumn_InvoiceDetail_Price.Width = 120;
            // 
            // repositoryItemSpinPrice
            // 
            this.repositoryItemSpinPrice.AutoHeight = false;
            this.repositoryItemSpinPrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinPrice.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinPrice.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinPrice.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.repositoryItemSpinPrice.Name = "repositoryItemSpinPrice";
            // 
            // gridColumn_InvoiceDetail_Amount
            // 
            this.gridColumn_InvoiceDetail_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_InvoiceDetail_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_Amount.Caption = "Số lượng";
            this.gridColumn_InvoiceDetail_Amount.ColumnEdit = this.repositoryItemSpinAmount;
            this.gridColumn_InvoiceDetail_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_InvoiceDetail_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_InvoiceDetail_Amount.FieldName = "AMOUNT";
            this.gridColumn_InvoiceDetail_Amount.Name = "gridColumn_InvoiceDetail_Amount";
            this.gridColumn_InvoiceDetail_Amount.Visible = true;
            this.gridColumn_InvoiceDetail_Amount.VisibleIndex = 3;
            this.gridColumn_InvoiceDetail_Amount.Width = 110;
            // 
            // repositoryItemSpinAmount
            // 
            this.repositoryItemSpinAmount.AutoHeight = false;
            this.repositoryItemSpinAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinAmount.DisplayFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinAmount.EditFormat.FormatString = "#,##0.00";
            this.repositoryItemSpinAmount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinAmount.MaxValue = new decimal(new int[] {
            1569325055,
            23283064,
            0,
            0});
            this.repositoryItemSpinAmount.Name = "repositoryItemSpinAmount";
            // 
            // gridColumn_InvoiceDetail_Discount
            // 
            this.gridColumn_InvoiceDetail_Discount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Discount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_InvoiceDetail_Discount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_Discount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_Discount.Caption = "Chiết khấu";
            this.gridColumn_InvoiceDetail_Discount.ColumnEdit = this.repositoryItemSpinDiscount;
            this.gridColumn_InvoiceDetail_Discount.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_InvoiceDetail_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_InvoiceDetail_Discount.FieldName = "DISCOUNT";
            this.gridColumn_InvoiceDetail_Discount.Name = "gridColumn_InvoiceDetail_Discount";
            this.gridColumn_InvoiceDetail_Discount.Visible = true;
            this.gridColumn_InvoiceDetail_Discount.VisibleIndex = 4;
            this.gridColumn_InvoiceDetail_Discount.Width = 120;
            // 
            // repositoryItemSpinDiscount
            // 
            this.repositoryItemSpinDiscount.AutoHeight = false;
            this.repositoryItemSpinDiscount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinDiscount.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinDiscount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinDiscount.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinDiscount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinDiscount.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.repositoryItemSpinDiscount.Name = "repositoryItemSpinDiscount";
            // 
            // gridColumn_InvoiceDetail_TotalPrice
            // 
            this.gridColumn_InvoiceDetail_TotalPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_TotalPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_InvoiceDetail_TotalPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_InvoiceDetail_TotalPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_InvoiceDetail_TotalPrice.Caption = "Thành tiền";
            this.gridColumn_InvoiceDetail_TotalPrice.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_InvoiceDetail_TotalPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_InvoiceDetail_TotalPrice.FieldName = "TOTAL_PRICE";
            this.gridColumn_InvoiceDetail_TotalPrice.Name = "gridColumn_InvoiceDetail_TotalPrice";
            this.gridColumn_InvoiceDetail_TotalPrice.OptionsColumn.AllowEdit = false;
            this.gridColumn_InvoiceDetail_TotalPrice.OptionsColumn.AllowFocus = false;
            this.gridColumn_InvoiceDetail_TotalPrice.Visible = true;
            this.gridColumn_InvoiceDetail_TotalPrice.VisibleIndex = 5;
            this.gridColumn_InvoiceDetail_TotalPrice.Width = 120;
            // 
            // gridColumn_InvoiceDetail_Delete
            // 
            this.gridColumn_InvoiceDetail_Delete.Caption = "Xóa";
            this.gridColumn_InvoiceDetail_Delete.ColumnEdit = this.repositoryItemBtnDelete;
            this.gridColumn_InvoiceDetail_Delete.FieldName = "DELETE";
            this.gridColumn_InvoiceDetail_Delete.Name = "gridColumn_InvoiceDetail_Delete";
            this.gridColumn_InvoiceDetail_Delete.OptionsColumn.ShowCaption = false;
            this.gridColumn_InvoiceDetail_Delete.Visible = true;
            this.gridColumn_InvoiceDetail_Delete.VisibleIndex = 6;
            this.gridColumn_InvoiceDetail_Delete.Width = 25;
            // 
            // repositoryItemBtnDelete
            // 
            this.repositoryItemBtnDelete.AutoHeight = false;
            this.repositoryItemBtnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.repositoryItemBtnDelete.Name = "repositoryItemBtnDelete";
            this.repositoryItemBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnDelete_ButtonClick);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(681, 559);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(96, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 27;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // txtVatRatio
            // 
            this.txtVatRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtVatRatio.Location = new System.Drawing.Point(757, 364);
            this.txtVatRatio.Name = "txtVatRatio";
            this.txtVatRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtVatRatio.Properties.DisplayFormat.FormatString = "#,##0.00";
            this.txtVatRatio.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtVatRatio.Properties.EditFormat.FormatString = "#,##0.00";
            this.txtVatRatio.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtVatRatio.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtVatRatio.Size = new System.Drawing.Size(121, 20);
            this.txtVatRatio.StyleController = this.layoutControl1;
            this.txtVatRatio.TabIndex = 14;
            this.txtVatRatio.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtVatRatio_PreviewKeyDown);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(781, 559);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(97, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 28;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(580, 559);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(97, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 26;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtPayFormCode
            // 
            this.txtPayFormCode.Location = new System.Drawing.Point(757, 340);
            this.txtPayFormCode.Name = "txtPayFormCode";
            this.txtPayFormCode.Size = new System.Drawing.Size(33, 20);
            this.txtPayFormCode.StyleController = this.layoutControl1;
            this.txtPayFormCode.TabIndex = 9;
            this.txtPayFormCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPayFormCode_PreviewKeyDown);
            // 
            // txtNumOrder
            // 
            this.txtNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtNumOrder.Location = new System.Drawing.Point(317, 340);
            this.txtNumOrder.Name = "txtNumOrder";
            this.txtNumOrder.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtNumOrder.Properties.MaxValue = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.txtNumOrder.Size = new System.Drawing.Size(121, 20);
            this.txtNumOrder.StyleController = this.layoutControl1;
            this.txtNumOrder.TabIndex = 7;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.txtSellerAddress);
            this.layoutControl3.Controls.Add(this.txtSellerAccountNumber);
            this.layoutControl3.Controls.Add(this.txtSellerPhone);
            this.layoutControl3.Controls.Add(this.txtSellerTaxCode);
            this.layoutControl3.Controls.Add(this.txtSellerName);
            this.layoutControl3.Location = new System.Drawing.Point(440, 386);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlSellerInfo;
            this.layoutControl3.Size = new System.Drawing.Size(440, 171);
            this.layoutControl3.TabIndex = 16;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // txtSellerAddress
            // 
            this.txtSellerAddress.Location = new System.Drawing.Point(98, 98);
            this.txtSellerAddress.Name = "txtSellerAddress";
            this.txtSellerAddress.Size = new System.Drawing.Size(339, 20);
            this.txtSellerAddress.StyleController = this.layoutControl3;
            this.txtSellerAddress.TabIndex = 25;
            this.txtSellerAddress.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSellerAddress_PreviewKeyDown);
            // 
            // txtSellerAccountNumber
            // 
            this.txtSellerAccountNumber.Location = new System.Drawing.Point(317, 74);
            this.txtSellerAccountNumber.Name = "txtSellerAccountNumber";
            this.txtSellerAccountNumber.Properties.MaxLength = 50;
            this.txtSellerAccountNumber.Size = new System.Drawing.Size(120, 20);
            this.txtSellerAccountNumber.StyleController = this.layoutControl3;
            this.txtSellerAccountNumber.TabIndex = 24;
            this.txtSellerAccountNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSellerAccountNumber_PreviewKeyDown);
            // 
            // txtSellerPhone
            // 
            this.txtSellerPhone.Location = new System.Drawing.Point(98, 74);
            this.txtSellerPhone.Name = "txtSellerPhone";
            this.txtSellerPhone.Properties.MaxLength = 20;
            this.txtSellerPhone.Size = new System.Drawing.Size(120, 20);
            this.txtSellerPhone.StyleController = this.layoutControl3;
            this.txtSellerPhone.TabIndex = 23;
            this.txtSellerPhone.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSellerPhone_PreviewKeyDown);
            // 
            // txtSellerTaxCode
            // 
            this.txtSellerTaxCode.Location = new System.Drawing.Point(98, 50);
            this.txtSellerTaxCode.Name = "txtSellerTaxCode";
            this.txtSellerTaxCode.Properties.MaxLength = 14;
            this.txtSellerTaxCode.Size = new System.Drawing.Size(339, 20);
            this.txtSellerTaxCode.StyleController = this.layoutControl3;
            this.txtSellerTaxCode.TabIndex = 22;
            this.txtSellerTaxCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSellerTaxCode_PreviewKeyDown);
            // 
            // txtSellerName
            // 
            this.txtSellerName.Location = new System.Drawing.Point(98, 26);
            this.txtSellerName.Name = "txtSellerName";
            this.txtSellerName.Size = new System.Drawing.Size(339, 20);
            this.txtSellerName.StyleController = this.layoutControl3;
            this.txtSellerName.TabIndex = 21;
            this.txtSellerName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSellerName_PreviewKeyDown);
            // 
            // layoutControlSellerInfo
            // 
            this.layoutControlSellerInfo.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.layoutControlSellerInfo.AppearanceGroup.Options.UseFont = true;
            this.layoutControlSellerInfo.AppearanceGroup.Options.UseTextOptions = true;
            this.layoutControlSellerInfo.AppearanceGroup.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlSellerInfo.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlSellerInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutSellerName,
            this.layoutSellerTaxCode,
            this.layoutSellerPhone,
            this.layoutSellerAccountNumber,
            this.layoutSellerAddress});
            this.layoutControlSellerInfo.Location = new System.Drawing.Point(0, 0);
            this.layoutControlSellerInfo.Name = "layoutControlSellerInfo";
            this.layoutControlSellerInfo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlSellerInfo.Size = new System.Drawing.Size(440, 171);
            this.layoutControlSellerInfo.Text = "THÔNG TIN NGƯỜI BÁN";
            // 
            // layoutSellerName
            // 
            this.layoutSellerName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutSellerName.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutSellerName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutSellerName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutSellerName.Control = this.txtSellerName;
            this.layoutSellerName.Location = new System.Drawing.Point(0, 0);
            this.layoutSellerName.Name = "layoutSellerName";
            this.layoutSellerName.Size = new System.Drawing.Size(438, 24);
            this.layoutSellerName.Text = "Tên:";
            this.layoutSellerName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutSellerName.TextSize = new System.Drawing.Size(90, 20);
            this.layoutSellerName.TextToControlDistance = 5;
            // 
            // layoutSellerTaxCode
            // 
            this.layoutSellerTaxCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutSellerTaxCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutSellerTaxCode.Control = this.txtSellerTaxCode;
            this.layoutSellerTaxCode.Location = new System.Drawing.Point(0, 24);
            this.layoutSellerTaxCode.Name = "layoutSellerTaxCode";
            this.layoutSellerTaxCode.Size = new System.Drawing.Size(438, 24);
            this.layoutSellerTaxCode.Text = "Mã số thuế:";
            this.layoutSellerTaxCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutSellerTaxCode.TextSize = new System.Drawing.Size(90, 20);
            this.layoutSellerTaxCode.TextToControlDistance = 5;
            // 
            // layoutSellerPhone
            // 
            this.layoutSellerPhone.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutSellerPhone.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutSellerPhone.Control = this.txtSellerPhone;
            this.layoutSellerPhone.Location = new System.Drawing.Point(0, 48);
            this.layoutSellerPhone.Name = "layoutSellerPhone";
            this.layoutSellerPhone.Size = new System.Drawing.Size(219, 24);
            this.layoutSellerPhone.Text = "Số điện thoai:";
            this.layoutSellerPhone.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutSellerPhone.TextSize = new System.Drawing.Size(90, 20);
            this.layoutSellerPhone.TextToControlDistance = 5;
            // 
            // layoutSellerAccountNumber
            // 
            this.layoutSellerAccountNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutSellerAccountNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutSellerAccountNumber.Control = this.txtSellerAccountNumber;
            this.layoutSellerAccountNumber.Location = new System.Drawing.Point(219, 48);
            this.layoutSellerAccountNumber.Name = "layoutSellerAccountNumber";
            this.layoutSellerAccountNumber.Size = new System.Drawing.Size(219, 24);
            this.layoutSellerAccountNumber.Text = "Số tài khoản:";
            this.layoutSellerAccountNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutSellerAccountNumber.TextSize = new System.Drawing.Size(90, 20);
            this.layoutSellerAccountNumber.TextToControlDistance = 5;
            // 
            // layoutSellerAddress
            // 
            this.layoutSellerAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutSellerAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutSellerAddress.Control = this.txtSellerAddress;
            this.layoutSellerAddress.Location = new System.Drawing.Point(0, 72);
            this.layoutSellerAddress.Name = "layoutSellerAddress";
            this.layoutSellerAddress.Size = new System.Drawing.Size(438, 74);
            this.layoutSellerAddress.Text = "Địa chỉ:";
            this.layoutSellerAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutSellerAddress.TextSize = new System.Drawing.Size(90, 20);
            this.layoutSellerAddress.TextToControlDistance = 5;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.txtBuyerDescription);
            this.layoutControl2.Controls.Add(this.txtBuyerAddress);
            this.layoutControl2.Controls.Add(this.txtBuyerOrganization);
            this.layoutControl2.Controls.Add(this.txtBuyerAccountNumber);
            this.layoutControl2.Controls.Add(this.txtBuyerTaxCode);
            this.layoutControl2.Controls.Add(this.txtBuyerName);
            this.layoutControl2.Location = new System.Drawing.Point(0, 386);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlBuyerInfo;
            this.layoutControl2.Size = new System.Drawing.Size(440, 171);
            this.layoutControl2.TabIndex = 15;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // txtBuyerDescription
            // 
            this.txtBuyerDescription.Location = new System.Drawing.Point(98, 122);
            this.txtBuyerDescription.Name = "txtBuyerDescription";
            this.txtBuyerDescription.Size = new System.Drawing.Size(339, 20);
            this.txtBuyerDescription.StyleController = this.layoutControl2;
            this.txtBuyerDescription.TabIndex = 20;
            this.txtBuyerDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerDescription_PreviewKeyDown);
            // 
            // txtBuyerAddress
            // 
            this.txtBuyerAddress.Location = new System.Drawing.Point(98, 98);
            this.txtBuyerAddress.Name = "txtBuyerAddress";
            this.txtBuyerAddress.Size = new System.Drawing.Size(339, 20);
            this.txtBuyerAddress.StyleController = this.layoutControl2;
            this.txtBuyerAddress.TabIndex = 19;
            this.txtBuyerAddress.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerAddress_PreviewKeyDown);
            // 
            // txtBuyerOrganization
            // 
            this.txtBuyerOrganization.Location = new System.Drawing.Point(98, 74);
            this.txtBuyerOrganization.Name = "txtBuyerOrganization";
            this.txtBuyerOrganization.Size = new System.Drawing.Size(339, 20);
            this.txtBuyerOrganization.StyleController = this.layoutControl2;
            this.txtBuyerOrganization.TabIndex = 18;
            this.txtBuyerOrganization.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerOrganization_PreviewKeyDown);
            // 
            // txtBuyerAccountNumber
            // 
            this.txtBuyerAccountNumber.Location = new System.Drawing.Point(317, 50);
            this.txtBuyerAccountNumber.Name = "txtBuyerAccountNumber";
            this.txtBuyerAccountNumber.Properties.MaxLength = 50;
            this.txtBuyerAccountNumber.Size = new System.Drawing.Size(120, 20);
            this.txtBuyerAccountNumber.StyleController = this.layoutControl2;
            this.txtBuyerAccountNumber.TabIndex = 17;
            this.txtBuyerAccountNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerAccountNumber_PreviewKeyDown);
            // 
            // txtBuyerTaxCode
            // 
            this.txtBuyerTaxCode.Location = new System.Drawing.Point(98, 50);
            this.txtBuyerTaxCode.Name = "txtBuyerTaxCode";
            this.txtBuyerTaxCode.Properties.MaxLength = 14;
            this.txtBuyerTaxCode.Size = new System.Drawing.Size(120, 20);
            this.txtBuyerTaxCode.StyleController = this.layoutControl2;
            this.txtBuyerTaxCode.TabIndex = 16;
            this.txtBuyerTaxCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerTaxCode_PreviewKeyDown);
            // 
            // txtBuyerName
            // 
            this.txtBuyerName.Location = new System.Drawing.Point(98, 26);
            this.txtBuyerName.Name = "txtBuyerName";
            this.txtBuyerName.Size = new System.Drawing.Size(339, 20);
            this.txtBuyerName.StyleController = this.layoutControl2;
            this.txtBuyerName.TabIndex = 15;
            this.txtBuyerName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBuyerName_PreviewKeyDown);
            // 
            // layoutControlBuyerInfo
            // 
            this.layoutControlBuyerInfo.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.layoutControlBuyerInfo.AppearanceGroup.Options.UseFont = true;
            this.layoutControlBuyerInfo.AppearanceGroup.Options.UseTextOptions = true;
            this.layoutControlBuyerInfo.AppearanceGroup.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlBuyerInfo.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlBuyerInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutBuyerName,
            this.layoutBuyerTaxCode,
            this.layoutBuyerAccountNumber,
            this.layoutBuyerOrganization,
            this.layoutBuyerAddress,
            this.layoutBuyerDescription});
            this.layoutControlBuyerInfo.Location = new System.Drawing.Point(0, 0);
            this.layoutControlBuyerInfo.Name = "layoutControlBuyerInfo";
            this.layoutControlBuyerInfo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlBuyerInfo.Size = new System.Drawing.Size(440, 171);
            this.layoutControlBuyerInfo.Text = "THÔNG TIN NGƯỜI MUA";
            // 
            // layoutBuyerName
            // 
            this.layoutBuyerName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutBuyerName.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutBuyerName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerName.Control = this.txtBuyerName;
            this.layoutBuyerName.Location = new System.Drawing.Point(0, 0);
            this.layoutBuyerName.Name = "layoutBuyerName";
            this.layoutBuyerName.Size = new System.Drawing.Size(438, 24);
            this.layoutBuyerName.Text = "Tên:";
            this.layoutBuyerName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerName.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerName.TextToControlDistance = 5;
            // 
            // layoutBuyerTaxCode
            // 
            this.layoutBuyerTaxCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerTaxCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerTaxCode.Control = this.txtBuyerTaxCode;
            this.layoutBuyerTaxCode.Location = new System.Drawing.Point(0, 24);
            this.layoutBuyerTaxCode.Name = "layoutBuyerTaxCode";
            this.layoutBuyerTaxCode.Size = new System.Drawing.Size(219, 24);
            this.layoutBuyerTaxCode.Text = "Mã số thuế:";
            this.layoutBuyerTaxCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerTaxCode.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerTaxCode.TextToControlDistance = 5;
            // 
            // layoutBuyerAccountNumber
            // 
            this.layoutBuyerAccountNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerAccountNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerAccountNumber.Control = this.txtBuyerAccountNumber;
            this.layoutBuyerAccountNumber.Location = new System.Drawing.Point(219, 24);
            this.layoutBuyerAccountNumber.Name = "layoutBuyerAccountNumber";
            this.layoutBuyerAccountNumber.Size = new System.Drawing.Size(219, 24);
            this.layoutBuyerAccountNumber.Text = "Số tài khoản:";
            this.layoutBuyerAccountNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerAccountNumber.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerAccountNumber.TextToControlDistance = 5;
            // 
            // layoutBuyerOrganization
            // 
            this.layoutBuyerOrganization.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerOrganization.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerOrganization.Control = this.txtBuyerOrganization;
            this.layoutBuyerOrganization.Location = new System.Drawing.Point(0, 48);
            this.layoutBuyerOrganization.Name = "layoutBuyerOrganization";
            this.layoutBuyerOrganization.Size = new System.Drawing.Size(438, 24);
            this.layoutBuyerOrganization.Text = "Tổ chức:";
            this.layoutBuyerOrganization.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerOrganization.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerOrganization.TextToControlDistance = 5;
            // 
            // layoutBuyerAddress
            // 
            this.layoutBuyerAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerAddress.Control = this.txtBuyerAddress;
            this.layoutBuyerAddress.Location = new System.Drawing.Point(0, 72);
            this.layoutBuyerAddress.Name = "layoutBuyerAddress";
            this.layoutBuyerAddress.Size = new System.Drawing.Size(438, 24);
            this.layoutBuyerAddress.Text = "Địa chỉ:";
            this.layoutBuyerAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerAddress.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerAddress.TextToControlDistance = 5;
            // 
            // layoutBuyerDescription
            // 
            this.layoutBuyerDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBuyerDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBuyerDescription.Control = this.txtBuyerDescription;
            this.layoutBuyerDescription.Location = new System.Drawing.Point(0, 96);
            this.layoutBuyerDescription.Name = "layoutBuyerDescription";
            this.layoutBuyerDescription.Size = new System.Drawing.Size(438, 50);
            this.layoutBuyerDescription.Text = "Mô tả:";
            this.layoutBuyerDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBuyerDescription.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBuyerDescription.TextToControlDistance = 5;
            // 
            // txtDiscount
            // 
            this.txtDiscount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtDiscount.Location = new System.Drawing.Point(537, 364);
            this.txtDiscount.Name = "txtDiscount";
            this.txtDiscount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDiscount.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.txtDiscount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscount.Properties.EditFormat.FormatString = "#,##0.0000";
            this.txtDiscount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscount.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.txtDiscount.Size = new System.Drawing.Size(121, 20);
            this.txtDiscount.StyleController = this.layoutControl1;
            this.txtDiscount.TabIndex = 13;
            this.txtDiscount.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDiscount_PreviewKeyDown);
            // 
            // txtAmount
            // 
            this.txtAmount.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtAmount.Enabled = false;
            this.txtAmount.Location = new System.Drawing.Point(317, 364);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtAmount.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.txtAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtAmount.Properties.EditFormat.FormatString = "#,##0.0000";
            this.txtAmount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtAmount.Size = new System.Drawing.Size(121, 20);
            this.txtAmount.StyleController = this.layoutControl1;
            this.txtAmount.TabIndex = 12;
            // 
            // dtInvoiceTime
            // 
            this.dtInvoiceTime.EditValue = null;
            this.dtInvoiceTime.Location = new System.Drawing.Point(97, 364);
            this.dtInvoiceTime.Name = "dtInvoiceTime";
            this.dtInvoiceTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInvoiceTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInvoiceTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInvoiceTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInvoiceTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInvoiceTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInvoiceTime.Size = new System.Drawing.Size(121, 20);
            this.dtInvoiceTime.StyleController = this.layoutControl1;
            this.dtInvoiceTime.TabIndex = 11;
            this.dtInvoiceTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtInvoiceTime_Closed);
            this.dtInvoiceTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtInvoiceTime_PreviewKeyDown);
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(790, 340);
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Size = new System.Drawing.Size(88, 20);
            this.cboPayForm.StyleController = this.layoutControl1;
            this.cboPayForm.TabIndex = 10;
            this.cboPayForm.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayForm_Closed);
            this.cboPayForm.EditValueChanged += new System.EventHandler(this.cboPayForm_EditValueChanged);
            // 
            // txtTongTuDen
            // 
            this.txtTongTuDen.Enabled = false;
            this.txtTongTuDen.Location = new System.Drawing.Point(537, 340);
            this.txtTongTuDen.Name = "txtTongTuDen";
            this.txtTongTuDen.Size = new System.Drawing.Size(121, 20);
            this.txtTongTuDen.StyleController = this.layoutControl1;
            this.txtTongTuDen.TabIndex = 8;
            // 
            // cboInvoiceBook
            // 
            this.cboInvoiceBook.Location = new System.Drawing.Point(135, 340);
            this.cboInvoiceBook.Name = "cboInvoiceBook";
            this.cboInvoiceBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboInvoiceBook.Properties.NullText = "";
            this.cboInvoiceBook.Size = new System.Drawing.Size(83, 20);
            this.cboInvoiceBook.StyleController = this.layoutControl1;
            this.cboInvoiceBook.TabIndex = 6;
            this.cboInvoiceBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboInvoiceBook_Closed);
            this.cboInvoiceBook.EditValueChanged += new System.EventHandler(this.cboInvoiceBook_EditValueChanged);
            // 
            // txtTemplateCode
            // 
            this.txtTemplateCode.Location = new System.Drawing.Point(97, 340);
            this.txtTemplateCode.Name = "txtTemplateCode";
            this.txtTemplateCode.Size = new System.Drawing.Size(38, 20);
            this.txtTemplateCode.StyleController = this.layoutControl1;
            this.txtTemplateCode.TabIndex = 5;
            this.txtTemplateCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTemplateCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutInvoiceBook,
            this.layoutControlItem3,
            this.layoutTongTuDen,
            this.layoutControlItem6,
            this.layoutInvoiceTime,
            this.layoutAmount,
            this.layoutDiscount,
            this.layoutControlItem10,
            this.layoutControlItem12,
            this.layoutNumOrder,
            this.layoutPayForm,
            this.emptySpaceItem2,
            this.layoutControlItem2,
            this.layoutControlItem5,
            this.layoutVatRatio,
            this.layoutControlItem1,
            this.layoutControlItem4,
            this.layoutControlItem7});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 583);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutInvoiceBook
            // 
            this.layoutInvoiceBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutInvoiceBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutInvoiceBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutInvoiceBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutInvoiceBook.Control = this.txtTemplateCode;
            this.layoutInvoiceBook.Location = new System.Drawing.Point(0, 338);
            this.layoutInvoiceBook.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutInvoiceBook.MinSize = new System.Drawing.Size(135, 24);
            this.layoutInvoiceBook.Name = "layoutInvoiceBook";
            this.layoutInvoiceBook.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutInvoiceBook.Size = new System.Drawing.Size(135, 24);
            this.layoutInvoiceBook.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutInvoiceBook.Text = "Sổ hóa đơn:";
            this.layoutInvoiceBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutInvoiceBook.TextSize = new System.Drawing.Size(90, 20);
            this.layoutInvoiceBook.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboInvoiceBook;
            this.layoutControlItem3.Location = new System.Drawing.Point(135, 338);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(85, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutTongTuDen
            // 
            this.layoutTongTuDen.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTongTuDen.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTongTuDen.Control = this.txtTongTuDen;
            this.layoutTongTuDen.Location = new System.Drawing.Point(440, 338);
            this.layoutTongTuDen.Name = "layoutTongTuDen";
            this.layoutTongTuDen.Size = new System.Drawing.Size(220, 24);
            this.layoutTongTuDen.Text = "Tổng/Từ/Đến:";
            this.layoutTongTuDen.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTongTuDen.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTongTuDen.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.cboPayForm;
            this.layoutControlItem6.Location = new System.Drawing.Point(790, 338);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(90, 24);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutInvoiceTime
            // 
            this.layoutInvoiceTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutInvoiceTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutInvoiceTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutInvoiceTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutInvoiceTime.Control = this.dtInvoiceTime;
            this.layoutInvoiceTime.Location = new System.Drawing.Point(0, 362);
            this.layoutInvoiceTime.Name = "layoutInvoiceTime";
            this.layoutInvoiceTime.Size = new System.Drawing.Size(220, 24);
            this.layoutInvoiceTime.Text = "Thời gian tạo:";
            this.layoutInvoiceTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutInvoiceTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutInvoiceTime.TextToControlDistance = 5;
            // 
            // layoutAmount
            // 
            this.layoutAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAmount.Control = this.txtAmount;
            this.layoutAmount.Location = new System.Drawing.Point(220, 362);
            this.layoutAmount.Name = "layoutAmount";
            this.layoutAmount.Size = new System.Drawing.Size(220, 24);
            this.layoutAmount.Text = "Số tiền:";
            this.layoutAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAmount.TextSize = new System.Drawing.Size(90, 20);
            this.layoutAmount.TextToControlDistance = 5;
            // 
            // layoutDiscount
            // 
            this.layoutDiscount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDiscount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDiscount.Control = this.txtDiscount;
            this.layoutDiscount.Location = new System.Drawing.Point(440, 362);
            this.layoutDiscount.Name = "layoutDiscount";
            this.layoutDiscount.Size = new System.Drawing.Size(220, 24);
            this.layoutDiscount.Text = "Chiết khấu:";
            this.layoutDiscount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDiscount.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDiscount.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.layoutControl2;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 386);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem10.Size = new System.Drawing.Size(440, 171);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.layoutControl3;
            this.layoutControlItem12.Location = new System.Drawing.Point(440, 386);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem12.Size = new System.Drawing.Size(440, 171);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutNumOrder
            // 
            this.layoutNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutNumOrder.Control = this.txtNumOrder;
            this.layoutNumOrder.Location = new System.Drawing.Point(220, 338);
            this.layoutNumOrder.Name = "layoutNumOrder";
            this.layoutNumOrder.Size = new System.Drawing.Size(220, 24);
            this.layoutNumOrder.Text = "Số hóa đơn:";
            this.layoutNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutNumOrder.TextSize = new System.Drawing.Size(90, 20);
            this.layoutNumOrder.TextToControlDistance = 5;
            // 
            // layoutPayForm
            // 
            this.layoutPayForm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPayForm.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPayForm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPayForm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPayForm.Control = this.txtPayFormCode;
            this.layoutPayForm.Location = new System.Drawing.Point(660, 338);
            this.layoutPayForm.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutPayForm.MinSize = new System.Drawing.Size(130, 24);
            this.layoutPayForm.Name = "layoutPayForm";
            this.layoutPayForm.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutPayForm.Size = new System.Drawing.Size(130, 24);
            this.layoutPayForm.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutPayForm.Text = "Hình thức:";
            this.layoutPayForm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPayForm.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPayForm.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 557);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(431, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(578, 557);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(101, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnNew;
            this.layoutControlItem5.Location = new System.Drawing.Point(779, 557);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(101, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutVatRatio
            // 
            this.layoutVatRatio.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutVatRatio.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutVatRatio.Control = this.txtVatRatio;
            this.layoutVatRatio.Location = new System.Drawing.Point(660, 362);
            this.layoutVatRatio.Name = "layoutVatRatio";
            this.layoutVatRatio.Size = new System.Drawing.Size(220, 24);
            this.layoutVatRatio.Text = "Thuế GTGT (%):";
            this.layoutVatRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutVatRatio.TextSize = new System.Drawing.Size(90, 20);
            this.layoutVatRatio.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(679, 557);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(100, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControlInvoiceDetail;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(880, 338);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnCreateElectricInvoice;
            this.layoutControlItem7.Location = new System.Drawing.Point(431, 557);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(147, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            this.layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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
            this.bbtnRCSave,
            this.bbtnRCPrint,
            this.bbtnRCNew});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCNew)});
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
            // bbtnRCNew
            // 
            this.bbtnRCNew.Caption = "Mới (Ctrl N)";
            this.bbtnRCNew.Id = 2;
            this.bbtnRCNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.bbtnRCNew.Name = "bbtnRCNew";
            this.bbtnRCNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCNew_ItemClick);
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 612);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
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
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 583);
            // 
            // frmInvoiceCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 612);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmInvoiceCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tạo hóa đơn";
            this.Load += new System.EventHandler(this.frmInvoiceCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlInvoiceDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewInvoiceDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTxtGoodsName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTxtGoodsUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVatRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerAccountNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSellerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlSellerInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerTaxCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerPhone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerAccountNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSellerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerOrganization.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerAccountNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuyerName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlBuyerInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerTaxCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerAccountNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerOrganization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBuyerDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInvoiceTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTongTuDen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboInvoiceBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTemplateCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInvoiceBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTongTuDen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutInvoiceTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVatRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SpinEdit txtVatRatio;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit txtPayFormCode;
        private DevExpress.XtraEditors.SpinEdit txtNumOrder;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.TextEdit txtSellerAddress;
        private DevExpress.XtraEditors.TextEdit txtSellerAccountNumber;
        private DevExpress.XtraEditors.TextEdit txtSellerPhone;
        private DevExpress.XtraEditors.TextEdit txtSellerTaxCode;
        private DevExpress.XtraEditors.TextEdit txtSellerName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlSellerInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutSellerName;
        private DevExpress.XtraLayout.LayoutControlItem layoutSellerTaxCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutSellerPhone;
        private DevExpress.XtraLayout.LayoutControlItem layoutSellerAccountNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutSellerAddress;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.TextEdit txtBuyerDescription;
        private DevExpress.XtraEditors.TextEdit txtBuyerAddress;
        private DevExpress.XtraEditors.TextEdit txtBuyerOrganization;
        private DevExpress.XtraEditors.TextEdit txtBuyerAccountNumber;
        private DevExpress.XtraEditors.TextEdit txtBuyerTaxCode;
        private DevExpress.XtraEditors.TextEdit txtBuyerName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlBuyerInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerName;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerTaxCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerAccountNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerOrganization;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerAddress;
        private DevExpress.XtraLayout.LayoutControlItem layoutBuyerDescription;
        private DevExpress.XtraEditors.SpinEdit txtDiscount;
        private DevExpress.XtraEditors.SpinEdit txtAmount;
        private DevExpress.XtraEditors.DateEdit dtInvoiceTime;
        private DevExpress.XtraEditors.LookUpEdit cboPayForm;
        private DevExpress.XtraEditors.TextEdit txtTongTuDen;
        private DevExpress.XtraEditors.LookUpEdit cboInvoiceBook;
        private DevExpress.XtraEditors.TextEdit txtTemplateCode;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutInvoiceBook;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutTongTuDen;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutInvoiceTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutDiscount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutPayForm;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutVatRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gridControlInvoiceDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewInvoiceDetail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_GoodsName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_GoodsUnit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_Price;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_Discount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_TotalPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_InvoiceDetail_Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTxtGoodsName;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTxtGoodsUnit;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinPrice;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinAmount;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinDiscount;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnDelete;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarButtonItem bbtnRCPrint;
        private DevExpress.XtraBars.BarButtonItem bbtnRCNew;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private System.Windows.Forms.BindingSource bindingSource1;
        private DevExpress.XtraEditors.SimpleButton btnCreateElectricInvoice;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;

    }
}
