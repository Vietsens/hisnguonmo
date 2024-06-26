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
namespace HIS.Desktop.Plugins.ElectronicBillTotal
{
    partial class FormElectronicBillTotal
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
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.trvService = new DevExpress.XtraTreeList.TreeList();
            this.tc_ServiceCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_ServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_Amount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_Price = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_VatRatio = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_TotalPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_TotalHeinPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tc_TotalPatientPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportBill = new DevExpress.XtraEditors.SimpleButton();
            this.chkHideHddt = new DevExpress.XtraEditors.CheckEdit();
            this.chkVienPhi = new DevExpress.XtraEditors.CheckEdit();
            this.chkNhaThuoc = new DevExpress.XtraEditors.CheckEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.lblPatienType = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientAddress = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientDob = new DevExpress.XtraEditors.LabelControl();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblMediOrg = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinCard = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinFrom = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinTo = new DevExpress.XtraEditors.LabelControl();
            this.lblRightRoute = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinRatio = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatienType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediOrg = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHeinCard = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHeinFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHeinTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRightRoute = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHeinRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHideHddt = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bBtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.bBtnExport = new DevExpress.XtraBars.BarButtonItem();
            this.bBtnNew = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.timerLoadData = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVienPhi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNhaThuoc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatienType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRightRoute)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.trvService);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnExportBill);
            this.layoutControl1.Controls.Add(this.chkHideHddt);
            this.layoutControl1.Controls.Add(this.chkVienPhi);
            this.layoutControl1.Controls.Add(this.chkNhaThuoc);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblPatientCode);
            this.layoutControl1.Controls.Add(this.lblPatienType);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Controls.Add(this.lblPatientAddress);
            this.layoutControl1.Controls.Add(this.lblPatientDob);
            this.layoutControl1.Controls.Add(this.lblGender);
            this.layoutControl1.Controls.Add(this.lblMediOrg);
            this.layoutControl1.Controls.Add(this.lblHeinCard);
            this.layoutControl1.Controls.Add(this.lblHeinFrom);
            this.layoutControl1.Controls.Add(this.lblHeinTo);
            this.layoutControl1.Controls.Add(this.lblRightRoute);
            this.layoutControl1.Controls.Add(this.lblHeinRatio);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 632);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1102, 608);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(106, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 36;
            this.btnPrint.Text = "In";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // trvService
            // 
            this.trvService.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tc_ServiceCode,
            this.tc_ServiceName,
            this.tc_Amount,
            this.tc_Price,
            this.tc_VatRatio,
            this.tc_TotalPrice,
            this.tc_TotalHeinPrice,
            this.tc_TotalPatientPrice});
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.KeyFieldName = "CONCRETE_ID__IN_SETY";
            this.trvService.Location = new System.Drawing.Point(2, 100);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvService.OptionsBehavior.AutoPopulateColumns = false;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.ShowClearButton = false;
            this.trvService.OptionsFind.ShowFindButton = false;
            this.trvService.OptionsSelection.InvertSelection = true;
            this.trvService.OptionsView.AutoWidth = false;
            this.trvService.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.trvService.OptionsView.ShowCheckBoxes = true;
            this.trvService.OptionsView.ShowHorzLines = false;
            this.trvService.OptionsView.ShowIndicator = false;
            this.trvService.OptionsView.ShowVertLines = false;
            this.trvService.ParentFieldName = "PARENT_ID__IN_SETY";
            this.trvService.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.trvService.Size = new System.Drawing.Size(1316, 504);
            this.trvService.TabIndex = 35;
            this.trvService.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
            this.trvService.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
            this.trvService.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
            this.trvService.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
            this.trvService.CustomDrawNodeCheckBox += new DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventHandler(this.trvService_CustomDrawNodeCheckBox);
            // 
            // tc_ServiceCode
            // 
            this.tc_ServiceCode.Caption = "Mã dịch vụ";
            this.tc_ServiceCode.FieldName = "TDL_SERVICE_CODE";
            this.tc_ServiceCode.MinWidth = 34;
            this.tc_ServiceCode.Name = "tc_ServiceCode";
            this.tc_ServiceCode.OptionsColumn.AllowEdit = false;
            this.tc_ServiceCode.Visible = true;
            this.tc_ServiceCode.VisibleIndex = 0;
            this.tc_ServiceCode.Width = 180;
            // 
            // tc_ServiceName
            // 
            this.tc_ServiceName.Caption = "Tên dịch vụ";
            this.tc_ServiceName.FieldName = "TDL_SERVICE_NAME";
            this.tc_ServiceName.Name = "tc_ServiceName";
            this.tc_ServiceName.OptionsColumn.AllowEdit = false;
            this.tc_ServiceName.Visible = true;
            this.tc_ServiceName.VisibleIndex = 1;
            this.tc_ServiceName.Width = 500;
            // 
            // tc_Amount
            // 
            this.tc_Amount.Caption = "Số lượng";
            this.tc_Amount.FieldName = "AMOUNT_DISPLAY";
            this.tc_Amount.Name = "tc_Amount";
            this.tc_Amount.OptionsColumn.AllowEdit = false;
            this.tc_Amount.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_Amount.Visible = true;
            this.tc_Amount.VisibleIndex = 2;
            this.tc_Amount.Width = 80;
            // 
            // tc_Price
            // 
            this.tc_Price.Caption = "Đơn giá";
            this.tc_Price.FieldName = "VIR_PRICE_DISPLAY";
            this.tc_Price.Name = "tc_Price";
            this.tc_Price.OptionsColumn.AllowEdit = false;
            this.tc_Price.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_Price.Visible = true;
            this.tc_Price.VisibleIndex = 3;
            this.tc_Price.Width = 120;
            // 
            // tc_VatRatio
            // 
            this.tc_VatRatio.Caption = "VAT (%)";
            this.tc_VatRatio.FieldName = "VAT_DISPLAY";
            this.tc_VatRatio.Name = "tc_VatRatio";
            this.tc_VatRatio.OptionsColumn.AllowEdit = false;
            this.tc_VatRatio.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_VatRatio.Visible = true;
            this.tc_VatRatio.VisibleIndex = 4;
            this.tc_VatRatio.Width = 60;
            // 
            // tc_TotalPrice
            // 
            this.tc_TotalPrice.Caption = "Thành tiền";
            this.tc_TotalPrice.FieldName = "VIR_TOTAL_PRICE_DISPLAY";
            this.tc_TotalPrice.Name = "tc_TotalPrice";
            this.tc_TotalPrice.OptionsColumn.AllowEdit = false;
            this.tc_TotalPrice.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_TotalPrice.Visible = true;
            this.tc_TotalPrice.VisibleIndex = 5;
            this.tc_TotalPrice.Width = 120;
            // 
            // tc_TotalHeinPrice
            // 
            this.tc_TotalHeinPrice.Caption = "Đồng chi trả";
            this.tc_TotalHeinPrice.FieldName = "VIR_TOTAL_HEIN_PRICE_DISPLAY";
            this.tc_TotalHeinPrice.Name = "tc_TotalHeinPrice";
            this.tc_TotalHeinPrice.OptionsColumn.AllowEdit = false;
            this.tc_TotalHeinPrice.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_TotalHeinPrice.Visible = true;
            this.tc_TotalHeinPrice.VisibleIndex = 6;
            this.tc_TotalHeinPrice.Width = 110;
            // 
            // tc_TotalPatientPrice
            // 
            this.tc_TotalPatientPrice.Caption = "Bệnh nhân trả";
            this.tc_TotalPatientPrice.FieldName = "VIR_TOTAL_PATIENT_PRICE_DISPLAY";
            this.tc_TotalPatientPrice.Name = "tc_TotalPatientPrice";
            this.tc_TotalPatientPrice.OptionsColumn.AllowEdit = false;
            this.tc_TotalPatientPrice.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.tc_TotalPatientPrice.Visible = true;
            this.tc_TotalPatientPrice.VisibleIndex = 7;
            this.tc_TotalPatientPrice.Width = 110;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1212, 608);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(106, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 33;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnExportBill
            // 
            this.btnExportBill.Location = new System.Drawing.Point(992, 608);
            this.btnExportBill.Name = "btnExportBill";
            this.btnExportBill.Size = new System.Drawing.Size(106, 22);
            this.btnExportBill.StyleController = this.layoutControl1;
            this.btnExportBill.TabIndex = 31;
            this.btnExportBill.Text = "Xuất (Ctrl S)";
            this.btnExportBill.Click += new System.EventHandler(this.btnExportBill_Click);
            // 
            // chkHideHddt
            // 
            this.chkHideHddt.Location = new System.Drawing.Point(969, 608);
            this.chkHideHddt.Name = "chkHideHddt";
            this.chkHideHddt.Properties.Caption = "";
            this.chkHideHddt.Size = new System.Drawing.Size(19, 19);
            this.chkHideHddt.StyleController = this.layoutControl1;
            this.chkHideHddt.TabIndex = 30;
            this.chkHideHddt.CheckedChanged += new System.EventHandler(this.chkHideHddt_CheckedChanged);
            // 
            // chkVienPhi
            // 
            this.chkVienPhi.EditValue = true;
            this.chkVienPhi.Location = new System.Drawing.Point(332, 2);
            this.chkVienPhi.Name = "chkVienPhi";
            this.chkVienPhi.Properties.Caption = "Viện phí";
            this.chkVienPhi.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkVienPhi.Size = new System.Drawing.Size(106, 19);
            this.chkVienPhi.StyleController = this.layoutControl1;
            this.chkVienPhi.TabIndex = 8;
            this.chkVienPhi.CheckedChanged += new System.EventHandler(this.chkVienPhi_CheckedChanged);
            // 
            // chkNhaThuoc
            // 
            this.chkNhaThuoc.Location = new System.Drawing.Point(442, 2);
            this.chkNhaThuoc.Name = "chkNhaThuoc";
            this.chkNhaThuoc.Properties.Caption = "Xuất bán";
            this.chkNhaThuoc.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkNhaThuoc.Size = new System.Drawing.Size(106, 19);
            this.chkNhaThuoc.StyleController = this.layoutControl1;
            this.chkNhaThuoc.TabIndex = 7;
            this.chkNhaThuoc.CheckedChanged += new System.EventHandler(this.chkNhaThuoc_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(222, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(106, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Tìm kiếm theo mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(216, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 4;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientCode.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(97, 28);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(121, 20);
            this.lblPatientCode.StyleController = this.layoutControl1;
            this.lblPatientCode.TabIndex = 17;
            // 
            // lblPatienType
            // 
            this.lblPatienType.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatienType.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatienType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatienType.Location = new System.Drawing.Point(97, 52);
            this.lblPatienType.Name = "lblPatienType";
            this.lblPatienType.Size = new System.Drawing.Size(121, 20);
            this.lblPatienType.StyleController = this.layoutControl1;
            this.lblPatienType.TabIndex = 18;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(317, 28);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(231, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 19;
            // 
            // lblPatientAddress
            // 
            this.lblPatientAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientAddress.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientAddress.Location = new System.Drawing.Point(1087, 28);
            this.lblPatientAddress.Name = "lblPatientAddress";
            this.lblPatientAddress.Size = new System.Drawing.Size(231, 20);
            this.lblPatientAddress.StyleController = this.layoutControl1;
            this.lblPatientAddress.TabIndex = 20;
            // 
            // lblPatientDob
            // 
            this.lblPatientDob.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientDob.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientDob.Location = new System.Drawing.Point(867, 28);
            this.lblPatientDob.Name = "lblPatientDob";
            this.lblPatientDob.Size = new System.Drawing.Size(121, 20);
            this.lblPatientDob.StyleController = this.layoutControl1;
            this.lblPatientDob.TabIndex = 21;
            // 
            // lblGender
            // 
            this.lblGender.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGender.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGender.Location = new System.Drawing.Point(647, 28);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(121, 20);
            this.lblGender.StyleController = this.layoutControl1;
            this.lblGender.TabIndex = 22;
            // 
            // lblMediOrg
            // 
            this.lblMediOrg.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediOrg.Location = new System.Drawing.Point(1087, 52);
            this.lblMediOrg.Name = "lblMediOrg";
            this.lblMediOrg.Size = new System.Drawing.Size(231, 20);
            this.lblMediOrg.StyleController = this.layoutControl1;
            this.lblMediOrg.TabIndex = 23;
            // 
            // lblHeinCard
            // 
            this.lblHeinCard.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCard.Location = new System.Drawing.Point(317, 52);
            this.lblHeinCard.Name = "lblHeinCard";
            this.lblHeinCard.Size = new System.Drawing.Size(231, 20);
            this.lblHeinCard.StyleController = this.layoutControl1;
            this.lblHeinCard.TabIndex = 24;
            // 
            // lblHeinFrom
            // 
            this.lblHeinFrom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinFrom.Location = new System.Drawing.Point(647, 52);
            this.lblHeinFrom.Name = "lblHeinFrom";
            this.lblHeinFrom.Size = new System.Drawing.Size(121, 20);
            this.lblHeinFrom.StyleController = this.layoutControl1;
            this.lblHeinFrom.TabIndex = 25;
            // 
            // lblHeinTo
            // 
            this.lblHeinTo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinTo.Location = new System.Drawing.Point(867, 52);
            this.lblHeinTo.Name = "lblHeinTo";
            this.lblHeinTo.Size = new System.Drawing.Size(121, 20);
            this.lblHeinTo.StyleController = this.layoutControl1;
            this.lblHeinTo.TabIndex = 26;
            // 
            // lblRightRoute
            // 
            this.lblRightRoute.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRightRoute.Location = new System.Drawing.Point(97, 76);
            this.lblRightRoute.Name = "lblRightRoute";
            this.lblRightRoute.Size = new System.Drawing.Size(121, 20);
            this.lblRightRoute.StyleController = this.layoutControl1;
            this.lblRightRoute.TabIndex = 27;
            // 
            // lblHeinRatio
            // 
            this.lblHeinRatio.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinRatio.Location = new System.Drawing.Point(317, 76);
            this.lblHeinRatio.Name = "lblHeinRatio";
            this.lblHeinRatio.Size = new System.Drawing.Size(1001, 20);
            this.lblHeinRatio.StyleController = this.layoutControl1;
            this.lblHeinRatio.TabIndex = 28;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.lciPatientCode,
            this.lciPatienType,
            this.lciPatientName,
            this.lciPatientAddress,
            this.lciPatientDob,
            this.lciGender,
            this.lciMediOrg,
            this.lciHeinCard,
            this.lciHeinFrom,
            this.lciHeinTo,
            this.lciRightRoute,
            this.lciHeinRatio,
            this.lciHideHddt,
            this.layoutControlItem7,
            this.layoutControlItem9,
            this.emptySpaceItem2,
            this.layoutControlItem6,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 632);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtTreatmentCode;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(220, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSearch;
            this.layoutControlItem2.Location = new System.Drawing.Point(220, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.chkVienPhi;
            this.layoutControlItem3.Location = new System.Drawing.Point(330, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.chkNhaThuoc;
            this.layoutControlItem4.Location = new System.Drawing.Point(440, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(550, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(770, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.lblPatientCode;
            this.lciPatientCode.CustomizationFormText = "Mã bệnh nhân:";
            this.lciPatientCode.Location = new System.Drawing.Point(0, 26);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(220, 24);
            this.lciPatientCode.Text = "Mã bệnh nhân:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPatienType
            // 
            this.lciPatienType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatienType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatienType.Control = this.lblPatienType;
            this.lciPatienType.CustomizationFormText = "Đối tượng:";
            this.lciPatienType.Location = new System.Drawing.Point(0, 50);
            this.lciPatienType.Name = "lciPatienType";
            this.lciPatienType.Size = new System.Drawing.Size(220, 24);
            this.lciPatienType.Text = "Đối tượng:";
            this.lciPatienType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatienType.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatienType.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.CustomizationFormText = "Tên bệnh nhân:";
            this.lciPatientName.Location = new System.Drawing.Point(220, 26);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(330, 24);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciPatientAddress
            // 
            this.lciPatientAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientAddress.Control = this.lblPatientAddress;
            this.lciPatientAddress.CustomizationFormText = "Địa chỉ:";
            this.lciPatientAddress.Location = new System.Drawing.Point(990, 26);
            this.lciPatientAddress.Name = "lciPatientAddress";
            this.lciPatientAddress.Size = new System.Drawing.Size(330, 24);
            this.lciPatientAddress.Text = "Địa chỉ:";
            this.lciPatientAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientAddress.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientAddress.TextToControlDistance = 5;
            // 
            // lciPatientDob
            // 
            this.lciPatientDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientDob.Control = this.lblPatientDob;
            this.lciPatientDob.CustomizationFormText = "Ngày sinh:";
            this.lciPatientDob.Location = new System.Drawing.Point(770, 26);
            this.lciPatientDob.Name = "lciPatientDob";
            this.lciPatientDob.Size = new System.Drawing.Size(220, 24);
            this.lciPatientDob.Text = "Ngày sinh:";
            this.lciPatientDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientDob.TextToControlDistance = 5;
            // 
            // lciGender
            // 
            this.lciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGender.Control = this.lblGender;
            this.lciGender.CustomizationFormText = "Giới tính";
            this.lciGender.Location = new System.Drawing.Point(550, 26);
            this.lciGender.Name = "lciGender";
            this.lciGender.Size = new System.Drawing.Size(220, 24);
            this.lciGender.Text = "Giới tính:";
            this.lciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGender.TextSize = new System.Drawing.Size(90, 20);
            this.lciGender.TextToControlDistance = 5;
            // 
            // lciMediOrg
            // 
            this.lciMediOrg.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediOrg.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediOrg.Control = this.lblMediOrg;
            this.lciMediOrg.CustomizationFormText = "Nơi ĐKKCBBĐ:";
            this.lciMediOrg.Location = new System.Drawing.Point(990, 50);
            this.lciMediOrg.Name = "lciMediOrg";
            this.lciMediOrg.Size = new System.Drawing.Size(330, 24);
            this.lciMediOrg.Text = "Nơi ĐKKCBBĐ:";
            this.lciMediOrg.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediOrg.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediOrg.TextToControlDistance = 5;
            // 
            // lciHeinCard
            // 
            this.lciHeinCard.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHeinCard.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHeinCard.Control = this.lblHeinCard;
            this.lciHeinCard.CustomizationFormText = "Số thẻ BH:";
            this.lciHeinCard.Location = new System.Drawing.Point(220, 50);
            this.lciHeinCard.Name = "lciHeinCard";
            this.lciHeinCard.Size = new System.Drawing.Size(330, 24);
            this.lciHeinCard.Text = "Số thẻ BH:";
            this.lciHeinCard.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHeinCard.TextSize = new System.Drawing.Size(90, 20);
            this.lciHeinCard.TextToControlDistance = 5;
            // 
            // lciHeinFrom
            // 
            this.lciHeinFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHeinFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHeinFrom.Control = this.lblHeinFrom;
            this.lciHeinFrom.CustomizationFormText = "Hạn từ:";
            this.lciHeinFrom.Location = new System.Drawing.Point(550, 50);
            this.lciHeinFrom.Name = "lciHeinFrom";
            this.lciHeinFrom.Size = new System.Drawing.Size(220, 24);
            this.lciHeinFrom.Text = "Hạn từ:";
            this.lciHeinFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHeinFrom.TextSize = new System.Drawing.Size(90, 20);
            this.lciHeinFrom.TextToControlDistance = 5;
            // 
            // lciHeinTo
            // 
            this.lciHeinTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHeinTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHeinTo.Control = this.lblHeinTo;
            this.lciHeinTo.CustomizationFormText = "Hạn đến:";
            this.lciHeinTo.Location = new System.Drawing.Point(770, 50);
            this.lciHeinTo.Name = "lciHeinTo";
            this.lciHeinTo.Size = new System.Drawing.Size(220, 24);
            this.lciHeinTo.Text = "Hạn đến:";
            this.lciHeinTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHeinTo.TextSize = new System.Drawing.Size(90, 20);
            this.lciHeinTo.TextToControlDistance = 5;
            // 
            // lciRightRoute
            // 
            this.lciRightRoute.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRightRoute.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRightRoute.Control = this.lblRightRoute;
            this.lciRightRoute.CustomizationFormText = "Tuyến:";
            this.lciRightRoute.Location = new System.Drawing.Point(0, 74);
            this.lciRightRoute.Name = "lciRightRoute";
            this.lciRightRoute.Size = new System.Drawing.Size(220, 24);
            this.lciRightRoute.Text = "Tuyến:";
            this.lciRightRoute.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRightRoute.TextSize = new System.Drawing.Size(90, 20);
            this.lciRightRoute.TextToControlDistance = 5;
            // 
            // lciHeinRatio
            // 
            this.lciHeinRatio.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHeinRatio.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHeinRatio.Control = this.lblHeinRatio;
            this.lciHeinRatio.CustomizationFormText = "Mức hưởng:";
            this.lciHeinRatio.Location = new System.Drawing.Point(220, 74);
            this.lciHeinRatio.Name = "lciHeinRatio";
            this.lciHeinRatio.Size = new System.Drawing.Size(1100, 24);
            this.lciHeinRatio.Text = "Mức hưởng:";
            this.lciHeinRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHeinRatio.TextSize = new System.Drawing.Size(90, 20);
            this.lciHeinRatio.TextToControlDistance = 5;
            // 
            // lciHideHddt
            // 
            this.lciHideHddt.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHideHddt.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHideHddt.Control = this.chkHideHddt;
            this.lciHideHddt.Location = new System.Drawing.Point(762, 606);
            this.lciHideHddt.Name = "lciHideHddt";
            this.lciHideHddt.Size = new System.Drawing.Size(228, 26);
            this.lciHideHddt.Text = "Không hiển thị HĐ ĐT:";
            this.lciHideHddt.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHideHddt.TextSize = new System.Drawing.Size(200, 20);
            this.lciHideHddt.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnExportBill;
            this.layoutControlItem7.Location = new System.Drawing.Point(990, 606);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnNew;
            this.layoutControlItem9.Location = new System.Drawing.Point(1210, 606);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 606);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(762, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.trvService;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 98);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(1320, 508);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnPrint;
            this.layoutControlItem5.Location = new System.Drawing.Point(1100, 606);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
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
            this.bBtnSearch,
            this.bBtnExport,
            this.bBtnNew});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnExport),
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnNew)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bBtnSearch
            // 
            this.bBtnSearch.Caption = "Ctrl F";
            this.bBtnSearch.Id = 0;
            this.bBtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bBtnSearch.Name = "bBtnSearch";
            this.bBtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnSearch_ItemClick);
            // 
            // bBtnExport
            // 
            this.bBtnExport.Caption = "Ctrl S";
            this.bBtnExport.Id = 1;
            this.bBtnExport.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bBtnExport.Name = "bBtnExport";
            this.bBtnExport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnExport_ItemClick);
            // 
            // bBtnNew
            // 
            this.bBtnNew.Caption = "Ctrl N";
            this.bBtnNew.Id = 2;
            this.bBtnNew.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.bBtnNew.Name = "bBtnNew";
            this.bBtnNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnNew_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1320, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 661);
            this.barDockControlBottom.Size = new System.Drawing.Size(1320, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 632);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1320, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 632);
            // 
            // timerLoadData
            // 
            this.timerLoadData.Enabled = true;
            // 
            // FormElectronicBillTotal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1320, 661);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormElectronicBillTotal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xuất hóa đơn điện tử";
            this.Load += new System.EventHandler(this.FormElectronicBillTotal_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideHddt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVienPhi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNhaThuoc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatienType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRightRoute)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeinRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHideHddt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.CheckEdit chkVienPhi;
        private DevExpress.XtraEditors.CheckEdit chkNhaThuoc;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraEditors.LabelControl lblPatienType;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblPatientAddress;
        private DevExpress.XtraEditors.LabelControl lblPatientDob;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblMediOrg;
        private DevExpress.XtraEditors.LabelControl lblHeinCard;
        private DevExpress.XtraEditors.LabelControl lblHeinFrom;
        private DevExpress.XtraEditors.LabelControl lblHeinTo;
        private DevExpress.XtraEditors.LabelControl lblRightRoute;
        private DevExpress.XtraEditors.LabelControl lblHeinRatio;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatienType;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientAddress;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientDob;
        private DevExpress.XtraLayout.LayoutControlItem lciGender;
        private DevExpress.XtraLayout.LayoutControlItem lciMediOrg;
        private DevExpress.XtraLayout.LayoutControlItem lciHeinCard;
        private DevExpress.XtraLayout.LayoutControlItem lciHeinFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciHeinTo;
        private DevExpress.XtraLayout.LayoutControlItem lciRightRoute;
        private DevExpress.XtraLayout.LayoutControlItem lciHeinRatio;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnExportBill;
        private DevExpress.XtraEditors.CheckEdit chkHideHddt;
        private DevExpress.XtraLayout.LayoutControlItem lciHideHddt;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_ServiceCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_ServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_Amount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_Price;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_VatRatio;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_TotalPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_TotalHeinPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tc_TotalPatientPrice;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bBtnSearch;
        private DevExpress.XtraBars.BarButtonItem bBtnExport;
        private DevExpress.XtraBars.BarButtonItem bBtnNew;
        private System.Windows.Forms.Timer timerLoadData;

    }
}
