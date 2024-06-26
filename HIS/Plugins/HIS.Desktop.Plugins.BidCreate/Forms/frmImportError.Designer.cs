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
namespace HIS.Desktop.Plugins.BidCreate.Forms
{
    partial class frmImportError
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
            this.btnExportExcel = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlErrors = new DevExpress.XtraGrid.GridControl();
            this.gridViewErrors = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ErrorImport_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ErrorDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_Type = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_TypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_MapTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidNumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_Supplier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ImpPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ImpVatRatio = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidPackageCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidGroupCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_BidYear = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_TypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_NationalName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ManufacturerName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ActiveIngrBhytCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_ActiveIngrBhytName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_Concentra = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_REGISTER_NUMBER = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_MONTH_LIFESPAN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_DAY_LIFESPAN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_HOUR_LIFESPAN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_MaTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_TenTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_MaDT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_TenBHYT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ErrorImport_QCDG = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnExportExcel);
            this.layoutControl1.Controls.Add(this.gridControlErrors);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(992, 508);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(106, 22);
            this.btnExportExcel.StyleController = this.layoutControl1;
            this.btnExportExcel.TabIndex = 5;
            this.btnExportExcel.Text = "Xuất excel (Ctrl E)";
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // gridControlErrors
            // 
            this.gridControlErrors.Location = new System.Drawing.Point(0, 0);
            this.gridControlErrors.MainView = this.gridViewErrors;
            this.gridControlErrors.Name = "gridControlErrors";
            this.gridControlErrors.Size = new System.Drawing.Size(1100, 506);
            this.gridControlErrors.TabIndex = 4;
            this.gridControlErrors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewErrors});
            // 
            // gridViewErrors
            // 
            this.gridViewErrors.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ErrorImport_Stt,
            this.gridColumn_ErrorImport_ErrorDescription,
            this.gridColumn_ErrorImport_Type,
            this.gridColumn_ErrorImport_TypeCode,
            this.gridColumn_ErrorImport_MapTypeCode,
            this.gridColumn_ErrorImport_BidNumOrder,
            this.gridColumn_ErrorImport_Supplier,
            this.gridColumn_ErrorImport_Amount,
            this.gridColumn_ErrorImport_ImpPrice,
            this.gridColumn_ErrorImport_ImpVatRatio,
            this.gridColumn_ErrorImport_BidTypeCode,
            this.gridColumn_ErrorImport_BidPackageCode,
            this.gridColumn_ErrorImport_BidGroupCode,
            this.gridColumn_ErrorImport_BidNumber,
            this.gridColumn_ErrorImport_BidName,
            this.gridColumn_ErrorImport_BidYear,
            this.gridColumn_ErrorImport_TypeName,
            this.gridColumn_ErrorImport_NationalName,
            this.gridColumn_ErrorImport_ManufacturerName,
            this.gridColumn_ErrorImport_ActiveIngrBhytCode,
            this.gridColumn_ErrorImport_ActiveIngrBhytName,
            this.gridColumn_ErrorImport_Concentra,
            this.gridColumn_ErrorImport_REGISTER_NUMBER,
            this.gridColumn_ErrorImport_MONTH_LIFESPAN,
            this.gridColumn_ErrorImport_DAY_LIFESPAN,
            this.gridColumn_ErrorImport_HOUR_LIFESPAN,
            this.gridColumn_ErrorImport_MaTT,
            this.gridColumn_ErrorImport_TenTT,
            this.gridColumn_ErrorImport_MaDT,
            this.gridColumn_ErrorImport_TenBHYT,
            this.gridColumn_ErrorImport_QCDG});
            this.gridViewErrors.GridControl = this.gridControlErrors;
            this.gridViewErrors.Name = "gridViewErrors";
            this.gridViewErrors.OptionsView.ColumnAutoWidth = false;
            this.gridViewErrors.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewErrors.OptionsView.ShowGroupPanel = false;
            this.gridViewErrors.OptionsView.ShowIndicator = false;
            this.gridViewErrors.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewErrors_CustomUnboundColumnData);
            // 
            // gridColumn_ErrorImport_Stt
            // 
            this.gridColumn_ErrorImport_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ErrorImport_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_Stt.Caption = "STT";
            this.gridColumn_ErrorImport_Stt.FieldName = "STT";
            this.gridColumn_ErrorImport_Stt.Name = "gridColumn_ErrorImport_Stt";
            this.gridColumn_ErrorImport_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_ErrorImport_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_ErrorImport_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_Stt.Visible = true;
            this.gridColumn_ErrorImport_Stt.VisibleIndex = 0;
            this.gridColumn_ErrorImport_Stt.Width = 40;
            // 
            // gridColumn_ErrorImport_ErrorDescription
            // 
            this.gridColumn_ErrorImport_ErrorDescription.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ErrorDescription.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn_ErrorImport_ErrorDescription.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ErrorDescription.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ErrorDescription.Caption = "Mô tả lỗi";
            this.gridColumn_ErrorImport_ErrorDescription.FieldName = "ErrorDescription";
            this.gridColumn_ErrorImport_ErrorDescription.Name = "gridColumn_ErrorImport_ErrorDescription";
            this.gridColumn_ErrorImport_ErrorDescription.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ErrorDescription.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_ErrorDescription.Visible = true;
            this.gridColumn_ErrorImport_ErrorDescription.VisibleIndex = 1;
            this.gridColumn_ErrorImport_ErrorDescription.Width = 200;
            // 
            // gridColumn_ErrorImport_Type
            // 
            this.gridColumn_ErrorImport_Type.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Type.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_Type.Caption = "Loại";
            this.gridColumn_ErrorImport_Type.FieldName = "TYPE_DISPLAY";
            this.gridColumn_ErrorImport_Type.Name = "gridColumn_ErrorImport_Type";
            this.gridColumn_ErrorImport_Type.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_Type.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_Type.Visible = true;
            this.gridColumn_ErrorImport_Type.VisibleIndex = 2;
            this.gridColumn_ErrorImport_Type.Width = 80;
            // 
            // gridColumn_ErrorImport_TypeCode
            // 
            this.gridColumn_ErrorImport_TypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_TypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_TypeCode.Caption = "Mã thuốc/vật tư/máu";
            this.gridColumn_ErrorImport_TypeCode.FieldName = "MEDICINE_TYPE_CODE";
            this.gridColumn_ErrorImport_TypeCode.Name = "gridColumn_ErrorImport_TypeCode";
            this.gridColumn_ErrorImport_TypeCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_TypeCode.Visible = true;
            this.gridColumn_ErrorImport_TypeCode.VisibleIndex = 3;
            // 
            // gridColumn_ErrorImport_MapTypeCode
            // 
            this.gridColumn_ErrorImport_MapTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_MapTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_MapTypeCode.Caption = "Mã vt tương đương";
            this.gridColumn_ErrorImport_MapTypeCode.FieldName = "MATERIAL_TYPE_MAP_CODE";
            this.gridColumn_ErrorImport_MapTypeCode.Name = "gridColumn_ErrorImport_MapTypeCode";
            this.gridColumn_ErrorImport_MapTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ErrorImport_MapTypeCode.Visible = true;
            this.gridColumn_ErrorImport_MapTypeCode.VisibleIndex = 4;
            this.gridColumn_ErrorImport_MapTypeCode.Width = 60;
            // 
            // gridColumn_ErrorImport_BidNumOrder
            // 
            this.gridColumn_ErrorImport_BidNumOrder.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidNumOrder.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ErrorImport_BidNumOrder.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidNumOrder.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidNumOrder.Caption = "Stt thầu";
            this.gridColumn_ErrorImport_BidNumOrder.FieldName = "BID_NUM_ORDER";
            this.gridColumn_ErrorImport_BidNumOrder.Name = "gridColumn_ErrorImport_BidNumOrder";
            this.gridColumn_ErrorImport_BidNumOrder.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidNumOrder.Visible = true;
            this.gridColumn_ErrorImport_BidNumOrder.VisibleIndex = 5;
            this.gridColumn_ErrorImport_BidNumOrder.Width = 50;
            // 
            // gridColumn_ErrorImport_Supplier
            // 
            this.gridColumn_ErrorImport_Supplier.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Supplier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_Supplier.Caption = "Mã nhà thầu";
            this.gridColumn_ErrorImport_Supplier.FieldName = "SUPPLIER_CODE";
            this.gridColumn_ErrorImport_Supplier.Name = "gridColumn_ErrorImport_Supplier";
            this.gridColumn_ErrorImport_Supplier.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_Supplier.Visible = true;
            this.gridColumn_ErrorImport_Supplier.VisibleIndex = 6;
            this.gridColumn_ErrorImport_Supplier.Width = 60;
            // 
            // gridColumn_ErrorImport_Amount
            // 
            this.gridColumn_ErrorImport_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ErrorImport_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_Amount.Caption = "Số lượng";
            this.gridColumn_ErrorImport_Amount.FieldName = "AMOUNT";
            this.gridColumn_ErrorImport_Amount.Name = "gridColumn_ErrorImport_Amount";
            this.gridColumn_ErrorImport_Amount.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_Amount.Visible = true;
            this.gridColumn_ErrorImport_Amount.VisibleIndex = 7;
            this.gridColumn_ErrorImport_Amount.Width = 100;
            // 
            // gridColumn_ErrorImport_ImpPrice
            // 
            this.gridColumn_ErrorImport_ImpPrice.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ImpPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ErrorImport_ImpPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ImpPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ImpPrice.Caption = "Đơn giá";
            this.gridColumn_ErrorImport_ImpPrice.FieldName = "IMP_PRICE";
            this.gridColumn_ErrorImport_ImpPrice.Name = "gridColumn_ErrorImport_ImpPrice";
            this.gridColumn_ErrorImport_ImpPrice.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ImpPrice.Visible = true;
            this.gridColumn_ErrorImport_ImpPrice.VisibleIndex = 8;
            this.gridColumn_ErrorImport_ImpPrice.Width = 100;
            // 
            // gridColumn_ErrorImport_ImpVatRatio
            // 
            this.gridColumn_ErrorImport_ImpVatRatio.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ImpVatRatio.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ImpVatRatio.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ImpVatRatio.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ImpVatRatio.Caption = "VAT (%)";
            this.gridColumn_ErrorImport_ImpVatRatio.FieldName = "ImpVatRatio";
            this.gridColumn_ErrorImport_ImpVatRatio.Name = "gridColumn_ErrorImport_ImpVatRatio";
            this.gridColumn_ErrorImport_ImpVatRatio.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ImpVatRatio.Visible = true;
            this.gridColumn_ErrorImport_ImpVatRatio.VisibleIndex = 9;
            // 
            // gridColumn_ErrorImport_BidTypeCode
            // 
            this.gridColumn_ErrorImport_BidTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidTypeCode.Caption = "Đơn vị";
            this.gridColumn_ErrorImport_BidTypeCode.FieldName = "BID_TYPE_CODE";
            this.gridColumn_ErrorImport_BidTypeCode.Name = "gridColumn_ErrorImport_BidTypeCode";
            this.gridColumn_ErrorImport_BidTypeCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidTypeCode.Visible = true;
            this.gridColumn_ErrorImport_BidTypeCode.VisibleIndex = 10;
            // 
            // gridColumn_ErrorImport_BidPackageCode
            // 
            this.gridColumn_ErrorImport_BidPackageCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidPackageCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidPackageCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidPackageCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidPackageCode.Caption = "Gói thầu";
            this.gridColumn_ErrorImport_BidPackageCode.FieldName = "BID_PACKAGE_CODE";
            this.gridColumn_ErrorImport_BidPackageCode.Name = "gridColumn_ErrorImport_BidPackageCode";
            this.gridColumn_ErrorImport_BidPackageCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidPackageCode.Visible = true;
            this.gridColumn_ErrorImport_BidPackageCode.VisibleIndex = 11;
            this.gridColumn_ErrorImport_BidPackageCode.Width = 60;
            // 
            // gridColumn_ErrorImport_BidGroupCode
            // 
            this.gridColumn_ErrorImport_BidGroupCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidGroupCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidGroupCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidGroupCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidGroupCode.Caption = "Nhóm thầu";
            this.gridColumn_ErrorImport_BidGroupCode.FieldName = "BID_GROUP_CODE";
            this.gridColumn_ErrorImport_BidGroupCode.Name = "gridColumn_ErrorImport_BidGroupCode";
            this.gridColumn_ErrorImport_BidGroupCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidGroupCode.Visible = true;
            this.gridColumn_ErrorImport_BidGroupCode.VisibleIndex = 12;
            this.gridColumn_ErrorImport_BidGroupCode.Width = 60;
            // 
            // gridColumn_ErrorImport_BidNumber
            // 
            this.gridColumn_ErrorImport_BidNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidNumber.Caption = "Quyết định thầu";
            this.gridColumn_ErrorImport_BidNumber.FieldName = "BID_NUMBER";
            this.gridColumn_ErrorImport_BidNumber.Name = "gridColumn_ErrorImport_BidNumber";
            this.gridColumn_ErrorImport_BidNumber.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidNumber.Visible = true;
            this.gridColumn_ErrorImport_BidNumber.VisibleIndex = 13;
            this.gridColumn_ErrorImport_BidNumber.Width = 90;
            // 
            // gridColumn_ErrorImport_BidName
            // 
            this.gridColumn_ErrorImport_BidName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidName.Caption = "Tên thầu";
            this.gridColumn_ErrorImport_BidName.FieldName = "BID_NAME";
            this.gridColumn_ErrorImport_BidName.Name = "gridColumn_ErrorImport_BidName";
            this.gridColumn_ErrorImport_BidName.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidName.Visible = true;
            this.gridColumn_ErrorImport_BidName.VisibleIndex = 14;
            this.gridColumn_ErrorImport_BidName.Width = 100;
            // 
            // gridColumn_ErrorImport_BidYear
            // 
            this.gridColumn_ErrorImport_BidYear.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidYear.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidYear.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_BidYear.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_BidYear.Caption = "Năm QĐ";
            this.gridColumn_ErrorImport_BidYear.FieldName = "BID_YEAR";
            this.gridColumn_ErrorImport_BidYear.Name = "gridColumn_ErrorImport_BidYear";
            this.gridColumn_ErrorImport_BidYear.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_BidYear.Visible = true;
            this.gridColumn_ErrorImport_BidYear.VisibleIndex = 15;
            this.gridColumn_ErrorImport_BidYear.Width = 60;
            // 
            // gridColumn_ErrorImport_TypeName
            // 
            this.gridColumn_ErrorImport_TypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_TypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_TypeName.Caption = "Tên thuốc/vật tư";
            this.gridColumn_ErrorImport_TypeName.FieldName = "MEDICINE_TYPE_NAME";
            this.gridColumn_ErrorImport_TypeName.Name = "gridColumn_ErrorImport_TypeName";
            this.gridColumn_ErrorImport_TypeName.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_TypeName.Visible = true;
            this.gridColumn_ErrorImport_TypeName.VisibleIndex = 16;
            this.gridColumn_ErrorImport_TypeName.Width = 150;
            // 
            // gridColumn_ErrorImport_NationalName
            // 
            this.gridColumn_ErrorImport_NationalName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_NationalName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_NationalName.Caption = "Quốc gia";
            this.gridColumn_ErrorImport_NationalName.FieldName = "NATIONAL_NAME";
            this.gridColumn_ErrorImport_NationalName.Name = "gridColumn_ErrorImport_NationalName";
            this.gridColumn_ErrorImport_NationalName.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_NationalName.Visible = true;
            this.gridColumn_ErrorImport_NationalName.VisibleIndex = 17;
            this.gridColumn_ErrorImport_NationalName.Width = 120;
            // 
            // gridColumn_ErrorImport_ManufacturerName
            // 
            this.gridColumn_ErrorImport_ManufacturerName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ManufacturerName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ManufacturerName.Caption = "Hãng sản xuất";
            this.gridColumn_ErrorImport_ManufacturerName.FieldName = "MANUFACTURER_NAME";
            this.gridColumn_ErrorImport_ManufacturerName.Name = "gridColumn_ErrorImport_ManufacturerName";
            this.gridColumn_ErrorImport_ManufacturerName.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ManufacturerName.Visible = true;
            this.gridColumn_ErrorImport_ManufacturerName.VisibleIndex = 18;
            this.gridColumn_ErrorImport_ManufacturerName.Width = 150;
            // 
            // gridColumn_ErrorImport_ActiveIngrBhytCode
            // 
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.Caption = "Mã hoạt chất";
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.FieldName = "ACTIVE_INGR_BHYT_CODE";
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.Name = "gridColumn_ErrorImport_ActiveIngrBhytCode";
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.Visible = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytCode.VisibleIndex = 19;
            // 
            // gridColumn_ErrorImport_ActiveIngrBhytName
            // 
            this.gridColumn_ErrorImport_ActiveIngrBhytName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_ActiveIngrBhytName.Caption = "Tên hoạt chất";
            this.gridColumn_ErrorImport_ActiveIngrBhytName.FieldName = "ACTIVE_INGR_BHYT_NAME";
            this.gridColumn_ErrorImport_ActiveIngrBhytName.Name = "gridColumn_ErrorImport_ActiveIngrBhytName";
            this.gridColumn_ErrorImport_ActiveIngrBhytName.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytName.Visible = true;
            this.gridColumn_ErrorImport_ActiveIngrBhytName.VisibleIndex = 20;
            this.gridColumn_ErrorImport_ActiveIngrBhytName.Width = 150;
            // 
            // gridColumn_ErrorImport_Concentra
            // 
            this.gridColumn_ErrorImport_Concentra.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ErrorImport_Concentra.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ErrorImport_Concentra.Caption = "Hàm lượng";
            this.gridColumn_ErrorImport_Concentra.FieldName = "CONCENTRA";
            this.gridColumn_ErrorImport_Concentra.Name = "gridColumn_ErrorImport_Concentra";
            this.gridColumn_ErrorImport_Concentra.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_Concentra.Visible = true;
            this.gridColumn_ErrorImport_Concentra.VisibleIndex = 21;
            // 
            // gridColumn_ErrorImport_REGISTER_NUMBER
            // 
            this.gridColumn_ErrorImport_REGISTER_NUMBER.Caption = "Số đăng kí";
            this.gridColumn_ErrorImport_REGISTER_NUMBER.FieldName = "GV_REGISTER_NUMBER";
            this.gridColumn_ErrorImport_REGISTER_NUMBER.Name = "gridColumn_ErrorImport_REGISTER_NUMBER";
            this.gridColumn_ErrorImport_REGISTER_NUMBER.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_REGISTER_NUMBER.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_REGISTER_NUMBER.Visible = true;
            this.gridColumn_ErrorImport_REGISTER_NUMBER.VisibleIndex = 22;
            // 
            // gridColumn_ErrorImport_MONTH_LIFESPAN
            // 
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.Caption = "Tuổi thọ (tháng)";
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.FieldName = "MONTH_LIFESPAN_STR";
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.Name = "gridColumn_ErrorImport_MONTH_LIFESPAN";
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.Visible = true;
            this.gridColumn_ErrorImport_MONTH_LIFESPAN.VisibleIndex = 23;
            // 
            // gridColumn_ErrorImport_DAY_LIFESPAN
            // 
            this.gridColumn_ErrorImport_DAY_LIFESPAN.Caption = "Tuổi thọ (ngày)";
            this.gridColumn_ErrorImport_DAY_LIFESPAN.FieldName = "DAY_LIFESPAN_STR";
            this.gridColumn_ErrorImport_DAY_LIFESPAN.Name = "gridColumn_ErrorImport_DAY_LIFESPAN";
            this.gridColumn_ErrorImport_DAY_LIFESPAN.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_DAY_LIFESPAN.Visible = true;
            this.gridColumn_ErrorImport_DAY_LIFESPAN.VisibleIndex = 24;
            // 
            // gridColumn_ErrorImport_HOUR_LIFESPAN
            // 
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.Caption = "Tuổi thọ (giờ)";
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.FieldName = "HOUR_LIFESPAN_STR";
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.Name = "gridColumn_ErrorImport_HOUR_LIFESPAN";
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.Visible = true;
            this.gridColumn_ErrorImport_HOUR_LIFESPAN.VisibleIndex = 25;
            // 
            // gridColumn_ErrorImport_MaTT
            // 
            this.gridColumn_ErrorImport_MaTT.Caption = "Mã trúng thầu";
            this.gridColumn_ErrorImport_MaTT.FieldName = "GV_BID_MATERIAL_TYPE_CODE";
            this.gridColumn_ErrorImport_MaTT.Name = "gridColumn_ErrorImport_MaTT";
            this.gridColumn_ErrorImport_MaTT.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_MaTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_MaTT.Visible = true;
            this.gridColumn_ErrorImport_MaTT.VisibleIndex = 28;
            // 
            // gridColumn_ErrorImport_TenTT
            // 
            this.gridColumn_ErrorImport_TenTT.Caption = "Tên trúng thầu";
            this.gridColumn_ErrorImport_TenTT.FieldName = "GV_BID_MATERIAL_TYPE_NAME";
            this.gridColumn_ErrorImport_TenTT.Name = "gridColumn_ErrorImport_TenTT";
            this.gridColumn_ErrorImport_TenTT.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_TenTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_TenTT.Visible = true;
            this.gridColumn_ErrorImport_TenTT.VisibleIndex = 29;
            // 
            // gridColumn_ErrorImport_MaDT
            // 
            this.gridColumn_ErrorImport_MaDT.Caption = "Mã dự thầu";
            this.gridColumn_ErrorImport_MaDT.FieldName = "GV_JOIN_BID_MATERIAL_TYPE_CODE";
            this.gridColumn_ErrorImport_MaDT.Name = "gridColumn_ErrorImport_MaDT";
            this.gridColumn_ErrorImport_MaDT.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_MaDT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_MaDT.Visible = true;
            this.gridColumn_ErrorImport_MaDT.VisibleIndex = 30;
            // 
            // gridColumn_ErrorImport_TenBHYT
            // 
            this.gridColumn_ErrorImport_TenBHYT.Caption = "Tên bảo hiểm y tế";
            this.gridColumn_ErrorImport_TenBHYT.FieldName = "GV_HEIN_SERVICE_BHYT_NAME";
            this.gridColumn_ErrorImport_TenBHYT.Name = "gridColumn_ErrorImport_TenBHYT";
            this.gridColumn_ErrorImport_TenBHYT.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_TenBHYT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_TenBHYT.Visible = true;
            this.gridColumn_ErrorImport_TenBHYT.VisibleIndex = 26;
            // 
            // gridColumn_ErrorImport_QCDG
            // 
            this.gridColumn_ErrorImport_QCDG.Caption = "Quy cách đóng gói";
            this.gridColumn_ErrorImport_QCDG.FieldName = "GV_PACKING_TYPE_NAME";
            this.gridColumn_ErrorImport_QCDG.Name = "gridColumn_ErrorImport_QCDG";
            this.gridColumn_ErrorImport_QCDG.OptionsColumn.ReadOnly = true;
            this.gridColumn_ErrorImport_QCDG.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ErrorImport_QCDG.Visible = true;
            this.gridColumn_ErrorImport_QCDG.VisibleIndex = 27;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlErrors;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(1100, 506);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnExportExcel;
            this.layoutControlItem2.Location = new System.Drawing.Point(990, 506);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 506);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(990, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
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
            this.barBtnExportExcel});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnExportExcel)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnExportExcel
            // 
            this.barBtnExportExcel.Caption = "Xuất excel (Ctrl E)";
            this.barBtnExportExcel.Id = 0;
            this.barBtnExportExcel.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
            this.barBtnExportExcel.Name = "barBtnExportExcel";
            this.barBtnExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnExportExcel_ItemClick);
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
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
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // frmImportError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmImportError";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Danh sách import lỗi";
            this.Load += new System.EventHandler(this.frmImportError_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnExportExcel;
        private DevExpress.XtraGrid.GridControl gridControlErrors;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewErrors;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ErrorDescription;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_TypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_TypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_Type;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ImpPrice;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ImpVatRatio;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_Supplier;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidNumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_NationalName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ManufacturerName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ActiveIngrBhytCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_ActiveIngrBhytName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_Concentra;
        private DevExpress.XtraBars.BarButtonItem barBtnExportExcel;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidPackageCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidGroupCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_BidYear;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_MaTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_TenTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_MaDT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_TenBHYT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_QCDG;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_MapTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_REGISTER_NUMBER;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_MONTH_LIFESPAN;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_DAY_LIFESPAN;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ErrorImport_HOUR_LIFESPAN;
    }
}
