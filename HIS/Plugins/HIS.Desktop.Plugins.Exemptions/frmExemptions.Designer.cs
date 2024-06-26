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
namespace HIS.Desktop.Plugins.Exemptions
{
    partial class frmExemptions
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.spinEditMienGiamTreatmnet = new DevExpress.XtraEditors.SpinEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem__Save = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__New = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.chkMienGiamTreatment = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.trvService = new DevExpress.XtraTreeList.TreeList();
            this.treeColumnServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnAmount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnPriceNoVAT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnIsExpend = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnVATPercent = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalHeinPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnTotalPatientPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnDiscount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeColumnServiceReqCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemchkIsExpend__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemchkIsExpend__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemSpinEdit__Discount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemTextEdit__D = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.spinEditTyLe = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.txtHeinTo = new DevExpress.XtraEditors.LabelControl();
            this.txtHeinFrom = new DevExpress.XtraEditors.LabelControl();
            this.txtHeinCard = new DevExpress.XtraEditors.LabelControl();
            this.txtMediOrg = new DevExpress.XtraEditors.LabelControl();
            this.txtGender = new DevExpress.XtraEditors.LabelControl();
            this.txtDOB = new DevExpress.XtraEditors.LabelControl();
            this.txtAddress = new DevExpress.XtraEditors.LabelControl();
            this.txtPatientName = new DevExpress.XtraEditors.LabelControl();
            this.txtPatienType = new DevExpress.XtraEditors.LabelControl();
            this.txtPatient = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem21 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtFindTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditMienGiamTreatmnet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMienGiamTreatment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit__Discount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit__D)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditTyLe.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.spinEditMienGiamTreatmnet);
            this.layoutControl1.Controls.Add(this.chkMienGiamTreatment);
            this.layoutControl1.Controls.Add(this.layoutControl4);
            this.layoutControl1.Controls.Add(this.simpleButton1);
            this.layoutControl1.Controls.Add(this.spinEditTyLe);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtFindTreatmentCode);
            this.layoutControl1.Controls.Add(this.btnNew);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(266, 284, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 633);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // spinEditMienGiamTreatmnet
            // 
            this.spinEditMienGiamTreatmnet.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEditMienGiamTreatmnet.Location = new System.Drawing.Point(375, 609);
            this.spinEditMienGiamTreatmnet.MenuManager = this.barManager1;
            this.spinEditMienGiamTreatmnet.Name = "spinEditMienGiamTreatmnet";
            this.spinEditMienGiamTreatmnet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinEditMienGiamTreatmnet.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinEditMienGiamTreatmnet.Properties.EditFormat.FormatString = "#######0;";
            this.spinEditMienGiamTreatmnet.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEditMienGiamTreatmnet.Size = new System.Drawing.Size(50, 20);
            this.spinEditMienGiamTreatmnet.StyleController = this.layoutControl1;
            this.spinEditMienGiamTreatmnet.TabIndex = 40;
            this.spinEditMienGiamTreatmnet.ToolTip = "Các chỉ định tiếp theo của bệnh nhân sẽ được tự động tính số tiền miễn giảm theo " +
    "tỷ lệ đã nhập";
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
            this.barButtonItem__Save,
            this.barButtonItem__New});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__Save),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__New)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem__Save
            // 
            this.barButtonItem__Save.Caption = "barButtonItem1";
            this.barButtonItem__Save.Id = 0;
            this.barButtonItem__Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem__Save.Name = "barButtonItem__Save";
            this.barButtonItem__Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Save_ItemClick);
            // 
            // barButtonItem__New
            // 
            this.barButtonItem__New.Caption = "barButtonItem1";
            this.barButtonItem__New.Id = 1;
            this.barButtonItem__New.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barButtonItem__New.Name = "barButtonItem__New";
            this.barButtonItem__New.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__New_ItemClick);
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 662);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 633);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 633);
            // 
            // chkMienGiamTreatment
            // 
            this.chkMienGiamTreatment.Location = new System.Drawing.Point(197, 609);
            this.chkMienGiamTreatment.MenuManager = this.barManager1;
            this.chkMienGiamTreatment.Name = "chkMienGiamTreatment";
            this.chkMienGiamTreatment.Properties.Caption = "";
            this.chkMienGiamTreatment.Size = new System.Drawing.Size(19, 19);
            this.chkMienGiamTreatment.StyleController = this.layoutControl1;
            this.chkMienGiamTreatment.TabIndex = 39;
            this.chkMienGiamTreatment.ToolTip = "Nếu tích chọn vào checkbox này, các chỉ định tiếp theo của bệnh nhân sẽ được tự đ" +
    "ộng tính số tiền miễn giảm theo tỷ lệ đã nhập";
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.trvService);
            this.layoutControl4.Location = new System.Drawing.Point(2, 80);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(1096, 525);
            this.layoutControl4.TabIndex = 38;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // trvService
            // 
            this.trvService.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedCell.Options.UseBackColor = true;
            this.trvService.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.FocusedRow.Options.UseBackColor = true;
            this.trvService.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.trvService.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.trvService.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeColumnServiceName,
            this.treeColumnAmount,
            this.treeColumnPriceNoVAT,
            this.treeColumnIsExpend,
            this.treeColumnVATPercent,
            this.treeColumnTotalPrice,
            this.treeColumnTotalHeinPrice,
            this.treeColumnTotalPatientPrice,
            this.treeColumnDiscount,
            this.treeColumnServiceCode,
            this.treeColumnServiceReqCode,
            this.treeListColumn1});
            this.trvService.Cursor = System.Windows.Forms.Cursors.Default;
            this.trvService.KeyFieldName = "CONCRETE_ID__IN_SETY";
            this.trvService.Location = new System.Drawing.Point(2, 2);
            this.trvService.Name = "trvService";
            this.trvService.OptionsBehavior.AllowIndeterminateCheckState = true;
            this.trvService.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.trvService.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.trvService.OptionsBehavior.AutoPopulateColumns = false;
            this.trvService.OptionsBehavior.EnableFiltering = true;
            this.trvService.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Smart;
            this.trvService.OptionsFind.FindDelay = 100;
            this.trvService.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
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
            this.trvService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemchkIsExpend__Enable,
            this.repositoryItemchkIsExpend__Disable,
            this.repositoryItemSpinEdit__Discount,
            this.repositoryItemTextEdit__D});
            this.trvService.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
            this.trvService.Size = new System.Drawing.Size(1092, 521);
            this.trvService.TabIndex = 4;
            this.trvService.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trvService_CustomNodeCellEdit);
            this.trvService.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
            this.trvService.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
            this.trvService.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
            this.trvService.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
            this.trvService.CustomDrawNodeCheckBox += new DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventHandler(this.trvService_CustomDrawNodeCheckBox);
            this.trvService.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.trvService_CellValueChanged);
            // 
            // treeColumnServiceName
            // 
            this.treeColumnServiceName.Caption = "Tên dịch vụ";
            this.treeColumnServiceName.FieldName = "TDL_SERVICE_NAME";
            this.treeColumnServiceName.MinWidth = 34;
            this.treeColumnServiceName.Name = "treeColumnServiceName";
            this.treeColumnServiceName.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceName.Visible = true;
            this.treeColumnServiceName.VisibleIndex = 0;
            this.treeColumnServiceName.Width = 203;
            // 
            // treeColumnAmount
            // 
            this.treeColumnAmount.Caption = "Số lượng";
            this.treeColumnAmount.FieldName = "AMOUNT_PLUS";
            this.treeColumnAmount.Format.FormatString = "#,##0.00";
            this.treeColumnAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnAmount.Name = "treeColumnAmount";
            this.treeColumnAmount.OptionsColumn.AllowEdit = false;
            this.treeColumnAmount.Visible = true;
            this.treeColumnAmount.VisibleIndex = 1;
            this.treeColumnAmount.Width = 55;
            // 
            // treeColumnPriceNoVAT
            // 
            this.treeColumnPriceNoVAT.Caption = "Đơn giá";
            this.treeColumnPriceNoVAT.FieldName = "VIR_PRICE_DISPLAY";
            this.treeColumnPriceNoVAT.Name = "treeColumnPriceNoVAT";
            this.treeColumnPriceNoVAT.OptionsColumn.AllowEdit = false;
            this.treeColumnPriceNoVAT.Visible = true;
            this.treeColumnPriceNoVAT.VisibleIndex = 2;
            this.treeColumnPriceNoVAT.Width = 100;
            // 
            // treeColumnIsExpend
            // 
            this.treeColumnIsExpend.Caption = "Hao phí";
            this.treeColumnIsExpend.FieldName = "IsExpend";
            this.treeColumnIsExpend.Name = "treeColumnIsExpend";
            this.treeColumnIsExpend.OptionsColumn.AllowEdit = false;
            this.treeColumnIsExpend.Visible = true;
            this.treeColumnIsExpend.VisibleIndex = 7;
            this.treeColumnIsExpend.Width = 60;
            // 
            // treeColumnVATPercent
            // 
            this.treeColumnVATPercent.Caption = "VAT (%)";
            this.treeColumnVATPercent.FieldName = "VAT_DISPLAY";
            this.treeColumnVATPercent.Format.FormatString = "#,##0.00";
            this.treeColumnVATPercent.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeColumnVATPercent.Name = "treeColumnVATPercent";
            this.treeColumnVATPercent.OptionsColumn.AllowEdit = false;
            this.treeColumnVATPercent.Visible = true;
            this.treeColumnVATPercent.VisibleIndex = 8;
            this.treeColumnVATPercent.Width = 78;
            // 
            // treeColumnTotalPrice
            // 
            this.treeColumnTotalPrice.Caption = "Thành tiền";
            this.treeColumnTotalPrice.FieldName = "VIR_TOTAL_PRICE_DISPLAY";
            this.treeColumnTotalPrice.Name = "treeColumnTotalPrice";
            this.treeColumnTotalPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPrice.Visible = true;
            this.treeColumnTotalPrice.VisibleIndex = 3;
            this.treeColumnTotalPrice.Width = 126;
            // 
            // treeColumnTotalHeinPrice
            // 
            this.treeColumnTotalHeinPrice.Caption = "Đồng chi chả";
            this.treeColumnTotalHeinPrice.FieldName = "VIR_TOTAL_HEIN_PRICE_DISPLAY";
            this.treeColumnTotalHeinPrice.Name = "treeColumnTotalHeinPrice";
            this.treeColumnTotalHeinPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalHeinPrice.Visible = true;
            this.treeColumnTotalHeinPrice.VisibleIndex = 4;
            this.treeColumnTotalHeinPrice.Width = 138;
            // 
            // treeColumnTotalPatientPrice
            // 
            this.treeColumnTotalPatientPrice.Caption = "Bệnh nhân chi chả";
            this.treeColumnTotalPatientPrice.FieldName = "VIR_TOTAL_PATIENT_PRICE_DISPLAY";
            this.treeColumnTotalPatientPrice.Name = "treeColumnTotalPatientPrice";
            this.treeColumnTotalPatientPrice.OptionsColumn.AllowEdit = false;
            this.treeColumnTotalPatientPrice.Visible = true;
            this.treeColumnTotalPatientPrice.VisibleIndex = 5;
            this.treeColumnTotalPatientPrice.Width = 112;
            // 
            // treeColumnDiscount
            // 
            this.treeColumnDiscount.Caption = "Chiết khấu";
            this.treeColumnDiscount.FieldName = "VIR_TOTAL_DISCOUNT";
            this.treeColumnDiscount.Name = "treeColumnDiscount";
            this.treeColumnDiscount.Visible = true;
            this.treeColumnDiscount.VisibleIndex = 6;
            this.treeColumnDiscount.Width = 138;
            // 
            // treeColumnServiceCode
            // 
            this.treeColumnServiceCode.Caption = "Mã dịch vụ";
            this.treeColumnServiceCode.FieldName = "TDL_SERVICE_CODE";
            this.treeColumnServiceCode.MinWidth = 34;
            this.treeColumnServiceCode.Name = "treeColumnServiceCode";
            this.treeColumnServiceCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceCode.Visible = true;
            this.treeColumnServiceCode.VisibleIndex = 9;
            this.treeColumnServiceCode.Width = 118;
            // 
            // treeColumnServiceReqCode
            // 
            this.treeColumnServiceReqCode.Caption = "Mã yêu cầu";
            this.treeColumnServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.treeColumnServiceReqCode.Name = "treeColumnServiceReqCode";
            this.treeColumnServiceReqCode.OptionsColumn.AllowEdit = false;
            this.treeColumnServiceReqCode.Visible = true;
            this.treeColumnServiceReqCode.VisibleIndex = 10;
            this.treeColumnServiceReqCode.Width = 118;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Người thực hiện";
            this.treeListColumn1.FieldName = "LOGIN_USERNAME";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 11;
            this.treeListColumn1.Width = 200;
            // 
            // repositoryItemchkIsExpend__Enable
            // 
            this.repositoryItemchkIsExpend__Enable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Enable.Name = "repositoryItemchkIsExpend__Enable";
            this.repositoryItemchkIsExpend__Enable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // repositoryItemchkIsExpend__Disable
            // 
            this.repositoryItemchkIsExpend__Disable.AutoHeight = false;
            this.repositoryItemchkIsExpend__Disable.Name = "repositoryItemchkIsExpend__Disable";
            this.repositoryItemchkIsExpend__Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemchkIsExpend__Disable.ReadOnly = true;
            // 
            // repositoryItemSpinEdit__Discount
            // 
            this.repositoryItemSpinEdit__Discount.AutoHeight = false;
            this.repositoryItemSpinEdit__Discount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit__Discount.DisplayFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinEdit__Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit__Discount.EditFormat.FormatString = "#,##0.0000";
            this.repositoryItemSpinEdit__Discount.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemSpinEdit__Discount.MaxValue = new decimal(new int[] {
            1316134911,
            2328,
            0,
            0});
            this.repositoryItemSpinEdit__Discount.Name = "repositoryItemSpinEdit__Discount";
            // 
            // repositoryItemTextEdit__D
            // 
            this.repositoryItemTextEdit__D.AutoHeight = false;
            this.repositoryItemTextEdit__D.Name = "repositoryItemTextEdit__D";
            this.repositoryItemTextEdit__D.ReadOnly = true;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1096, 525);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.trvService;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1096, 525);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(464, 2);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(115, 22);
            this.simpleButton1.StyleController = this.layoutControl1;
            this.simpleButton1.TabIndex = 37;
            this.simpleButton1.Text = "Đồng ý";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // spinEditTyLe
            // 
            this.spinEditTyLe.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinEditTyLe.Location = new System.Drawing.Point(410, 2);
            this.spinEditTyLe.Name = "spinEditTyLe";
            this.spinEditTyLe.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinEditTyLe.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinEditTyLe.Properties.Mask.EditMask = "#######0;";
            this.spinEditTyLe.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spinEditTyLe.Size = new System.Drawing.Size(50, 20);
            this.spinEditTyLe.StyleController = this.layoutControl1;
            this.spinEditTyLe.TabIndex = 36;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.txtHeinTo);
            this.layoutControl2.Controls.Add(this.txtHeinFrom);
            this.layoutControl2.Controls.Add(this.txtHeinCard);
            this.layoutControl2.Controls.Add(this.txtMediOrg);
            this.layoutControl2.Controls.Add(this.txtGender);
            this.layoutControl2.Controls.Add(this.txtDOB);
            this.layoutControl2.Controls.Add(this.txtAddress);
            this.layoutControl2.Controls.Add(this.txtPatientName);
            this.layoutControl2.Controls.Add(this.txtPatienType);
            this.layoutControl2.Controls.Add(this.txtPatient);
            this.layoutControl2.Controls.Add(this.labelControl2);
            this.layoutControl2.Controls.Add(this.txtPatientCode);
            this.layoutControl2.Controls.Add(this.labelControl1);
            this.layoutControl2.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem24,
            this.layoutControlItem12,
            this.layoutControlItem10});
            this.layoutControl2.Location = new System.Drawing.Point(2, 28);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1096, 48);
            this.layoutControl2.TabIndex = 35;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // txtHeinTo
            // 
            this.txtHeinTo.Location = new System.Drawing.Point(727, 26);
            this.txtHeinTo.Name = "txtHeinTo";
            this.txtHeinTo.Size = new System.Drawing.Size(81, 20);
            this.txtHeinTo.StyleController = this.layoutControl2;
            this.txtHeinTo.TabIndex = 26;
            // 
            // txtHeinFrom
            // 
            this.txtHeinFrom.Location = new System.Drawing.Point(547, 26);
            this.txtHeinFrom.Name = "txtHeinFrom";
            this.txtHeinFrom.Size = new System.Drawing.Size(81, 20);
            this.txtHeinFrom.StyleController = this.layoutControl2;
            this.txtHeinFrom.TabIndex = 25;
            // 
            // txtHeinCard
            // 
            this.txtHeinCard.Location = new System.Drawing.Point(297, 26);
            this.txtHeinCard.Name = "txtHeinCard";
            this.txtHeinCard.Size = new System.Drawing.Size(151, 20);
            this.txtHeinCard.StyleController = this.layoutControl2;
            this.txtHeinCard.TabIndex = 24;
            // 
            // txtMediOrg
            // 
            this.txtMediOrg.Location = new System.Drawing.Point(907, 26);
            this.txtMediOrg.Name = "txtMediOrg";
            this.txtMediOrg.Size = new System.Drawing.Size(187, 20);
            this.txtMediOrg.StyleController = this.layoutControl2;
            this.txtMediOrg.TabIndex = 23;
            // 
            // txtGender
            // 
            this.txtGender.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGender.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtGender.Location = new System.Drawing.Point(547, 2);
            this.txtGender.Name = "txtGender";
            this.txtGender.Size = new System.Drawing.Size(81, 20);
            this.txtGender.StyleController = this.layoutControl2;
            this.txtGender.TabIndex = 22;
            // 
            // txtDOB
            // 
            this.txtDOB.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDOB.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtDOB.Location = new System.Drawing.Point(727, 2);
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.Size = new System.Drawing.Size(81, 20);
            this.txtDOB.StyleController = this.layoutControl2;
            this.txtDOB.TabIndex = 21;
            // 
            // txtAddress
            // 
            this.txtAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAddress.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtAddress.Location = new System.Drawing.Point(907, 2);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(187, 20);
            this.txtAddress.StyleController = this.layoutControl2;
            this.txtAddress.TabIndex = 20;
            // 
            // txtPatientName
            // 
            this.txtPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatientName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtPatientName.Location = new System.Drawing.Point(297, 2);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(151, 20);
            this.txtPatientName.StyleController = this.layoutControl2;
            this.txtPatientName.TabIndex = 19;
            // 
            // txtPatienType
            // 
            this.txtPatienType.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatienType.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtPatienType.Location = new System.Drawing.Point(97, 26);
            this.txtPatienType.Name = "txtPatienType";
            this.txtPatienType.Size = new System.Drawing.Size(101, 20);
            this.txtPatienType.StyleController = this.layoutControl2;
            this.txtPatienType.TabIndex = 18;
            // 
            // txtPatient
            // 
            this.txtPatient.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPatient.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtPatient.Location = new System.Drawing.Point(97, 2);
            this.txtPatient.Name = "txtPatient";
            this.txtPatient.Size = new System.Drawing.Size(101, 20);
            this.txtPatient.StyleController = this.layoutControl2;
            this.txtPatient.TabIndex = 17;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(2, 26);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(63, 13);
            this.labelControl2.StyleController = this.layoutControl2;
            this.labelControl2.TabIndex = 16;
            this.labelControl2.Text = "labelControl2";
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(2, 2);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Size = new System.Drawing.Size(63, 13);
            this.txtPatientCode.StyleController = this.layoutControl2;
            this.txtPatientCode.TabIndex = 15;
            this.txtPatientCode.Text = "labelControl2";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(2, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(1092, 13);
            this.labelControl1.StyleController = this.layoutControl2;
            this.labelControl1.TabIndex = 14;
            this.labelControl1.Text = "labelControl1";
            // 
            // layoutControlItem24
            // 
            this.layoutControlItem24.Control = this.labelControl1;
            this.layoutControlItem24.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem24.Name = "layoutControlItem24";
            this.layoutControlItem24.Size = new System.Drawing.Size(1096, 48);
            this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem24.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.labelControl2;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(1096, 24);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.txtPatientCode;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(1096, 48);
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextToControlDistance = 0;
            this.layoutControlItem10.TextVisible = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem13,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.layoutControlItem16,
            this.layoutControlItem20,
            this.layoutControlItem21,
            this.layoutControlItem22,
            this.layoutControlItem23,
            this.layoutControlItem25,
            this.layoutControlItem26});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1096, 48);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.txtPatient;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(200, 24);
            this.layoutControlItem13.Text = "Mã bệnh nhân:";
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem13.TextToControlDistance = 5;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem14.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem14.Control = this.txtPatienType;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(200, 24);
            this.layoutControlItem14.Text = "Đối tượng:";
            this.layoutControlItem14.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem14.TextToControlDistance = 5;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem15.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem15.Control = this.txtPatientName;
            this.layoutControlItem15.Location = new System.Drawing.Point(200, 0);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(250, 24);
            this.layoutControlItem15.Text = "Tên bệnh nhân:";
            this.layoutControlItem15.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem15.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem16.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem16.Control = this.txtAddress;
            this.layoutControlItem16.Location = new System.Drawing.Point(810, 0);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(286, 24);
            this.layoutControlItem16.Text = "Địa chỉ:";
            this.layoutControlItem16.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem16.TextToControlDistance = 5;
            // 
            // layoutControlItem20
            // 
            this.layoutControlItem20.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem20.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem20.Control = this.txtDOB;
            this.layoutControlItem20.Location = new System.Drawing.Point(630, 0);
            this.layoutControlItem20.Name = "layoutControlItem20";
            this.layoutControlItem20.Size = new System.Drawing.Size(180, 24);
            this.layoutControlItem20.Text = "Ngày sinh:";
            this.layoutControlItem20.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem20.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem20.TextToControlDistance = 5;
            // 
            // layoutControlItem21
            // 
            this.layoutControlItem21.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem21.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem21.Control = this.txtGender;
            this.layoutControlItem21.Location = new System.Drawing.Point(450, 0);
            this.layoutControlItem21.Name = "layoutControlItem21";
            this.layoutControlItem21.Size = new System.Drawing.Size(180, 24);
            this.layoutControlItem21.Text = "Giới tính";
            this.layoutControlItem21.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem21.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem21.TextToControlDistance = 5;
            // 
            // layoutControlItem22
            // 
            this.layoutControlItem22.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem22.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem22.Control = this.txtMediOrg;
            this.layoutControlItem22.Location = new System.Drawing.Point(810, 24);
            this.layoutControlItem22.Name = "layoutControlItem22";
            this.layoutControlItem22.Size = new System.Drawing.Size(286, 24);
            this.layoutControlItem22.Text = "Nơi ĐKKCBBĐ:";
            this.layoutControlItem22.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem22.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem22.TextToControlDistance = 5;
            // 
            // layoutControlItem23
            // 
            this.layoutControlItem23.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem23.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem23.Control = this.txtHeinCard;
            this.layoutControlItem23.Location = new System.Drawing.Point(200, 24);
            this.layoutControlItem23.Name = "layoutControlItem23";
            this.layoutControlItem23.Size = new System.Drawing.Size(250, 24);
            this.layoutControlItem23.Text = "Số thẻ BH:";
            this.layoutControlItem23.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem23.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem23.TextToControlDistance = 5;
            // 
            // layoutControlItem25
            // 
            this.layoutControlItem25.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem25.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem25.Control = this.txtHeinFrom;
            this.layoutControlItem25.Location = new System.Drawing.Point(450, 24);
            this.layoutControlItem25.Name = "layoutControlItem25";
            this.layoutControlItem25.Size = new System.Drawing.Size(180, 24);
            this.layoutControlItem25.Text = "Hạn từ:";
            this.layoutControlItem25.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem25.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem25.TextToControlDistance = 5;
            // 
            // layoutControlItem26
            // 
            this.layoutControlItem26.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem26.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem26.Control = this.txtHeinTo;
            this.layoutControlItem26.Location = new System.Drawing.Point(630, 24);
            this.layoutControlItem26.Name = "layoutControlItem26";
            this.layoutControlItem26.Size = new System.Drawing.Size(180, 24);
            this.layoutControlItem26.Text = "Hạn đến:";
            this.layoutControlItem26.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem26.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem26.TextToControlDistance = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(145, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(106, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 33;
            this.btnSearch.Text = "Tìm(Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtFindTreatmentCode
            // 
            this.txtFindTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.txtFindTreatmentCode.Name = "txtFindTreatmentCode";
            this.txtFindTreatmentCode.Properties.NullValuePrompt = "Tìm kiếm theo mã điều trị";
            this.txtFindTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtFindTreatmentCode.Size = new System.Drawing.Size(139, 20);
            this.txtFindTreatmentCode.StyleController = this.layoutControl1;
            this.txtFindTreatmentCode.TabIndex = 32;
            this.txtFindTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtFindTreatmentCode_PreviewKeyDown);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(1007, 609);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(91, 22);
            this.btnNew.StyleController = this.layoutControl1;
            this.btnNew.TabIndex = 24;
            this.btnNew.Text = "Mới (Ctrl N)";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(918, 609);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 21;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnSave,
            this.layoutControlItem19,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.emptySpaceItem1,
            this.layoutControlItem9,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.emptySpaceItem2,
            this.layoutControlItem4,
            this.layoutControlItem2,
            this.layoutControlItem6});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 633);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciBtnSave
            // 
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Location = new System.Drawing.Point(916, 607);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new System.Drawing.Size(89, 26);
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            // 
            // layoutControlItem19
            // 
            this.layoutControlItem19.Control = this.btnNew;
            this.layoutControlItem19.Location = new System.Drawing.Point(1005, 607);
            this.layoutControlItem19.Name = "layoutControlItem19";
            this.layoutControlItem19.Size = new System.Drawing.Size(95, 26);
            this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem19.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtFindTreatmentCode;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(143, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnSearch;
            this.layoutControlItem8.Location = new System.Drawing.Point(143, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(581, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(519, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.layoutControl2;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(1100, 52);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.spinEditTyLe;
            this.layoutControlItem1.Location = new System.Drawing.Point(253, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(209, 26);
            this.layoutControlItem1.Text = "Tỷ lệ miễn giảm (%) ";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButton1;
            this.layoutControlItem3.Location = new System.Drawing.Point(462, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(119, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(427, 607);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(489, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.layoutControl4;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 78);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1100, 529);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.chkMienGiamTreatment;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 607);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(218, 26);
            this.layoutControlItem2.Text = "Tự động miễn giảm các chỉ định";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(190, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.spinEditMienGiamTreatmnet;
            this.layoutControlItem6.Location = new System.Drawing.Point(218, 607);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(209, 26);
            this.layoutControlItem6.Text = "Tỷ lệ miễn giảm (%): ";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // frmExemptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 662);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmExemptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Miễn giảm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmExemptions_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinEditMienGiamTreatmnet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMienGiamTreatment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemchkIsExpend__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit__Discount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit__D)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinEditTyLe.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem19;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtFindTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.LabelControl txtHeinTo;
        private DevExpress.XtraEditors.LabelControl txtHeinFrom;
        private DevExpress.XtraEditors.LabelControl txtHeinCard;
        private DevExpress.XtraEditors.LabelControl txtMediOrg;
        private DevExpress.XtraEditors.LabelControl txtGender;
        private DevExpress.XtraEditors.LabelControl txtDOB;
        private DevExpress.XtraEditors.LabelControl txtAddress;
        private DevExpress.XtraEditors.LabelControl txtPatientName;
        private DevExpress.XtraEditors.LabelControl txtPatienType;
        private DevExpress.XtraEditors.LabelControl txtPatient;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl txtPatientCode;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem24;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem20;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem21;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem22;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem23;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem25;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem26;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SpinEdit spinEditTyLe;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        internal DevExpress.XtraTreeList.TreeList trvService;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnAmount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnPriceNoVAT;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnVATPercent;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalHeinPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnTotalPatientPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnDiscount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnServiceReqCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeColumnIsExpend;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemchkIsExpend__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit__Discount;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit__D;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__Save;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__New;
        private DevExpress.XtraEditors.SpinEdit spinEditMienGiamTreatmnet;
        private DevExpress.XtraEditors.CheckEdit chkMienGiamTreatment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
    }
}
