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
namespace HIS.Desktop.Plugins.TransactionBillListPrint
{
    partial class FormTransactionBillListPrint
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
            this.lblTongTienThucNop = new DevExpress.XtraEditors.LabelControl();
            this.lblTongTienHuy = new DevExpress.XtraEditors.LabelControl();
            this.lblTongTien = new DevExpress.XtraEditors.LabelControl();
            this.BtnPrintNowForXuatBan = new DevExpress.XtraEditors.SimpleButton();
            this.BtnPrintNow = new DevExpress.XtraEditors.SimpleButton();
            this.TxtTemplateCode = new DevExpress.XtraEditors.TextEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrintNow = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrintNowXuatBan = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.TxtSymbolCode = new DevExpress.XtraEditors.TextEdit();
            this.TxtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.BtnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.BtnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.BtnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.BarNumOrder = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.ChkBL_HD_H = new DevExpress.XtraEditors.CheckEdit();
            this.SpNumOrderTo = new DevExpress.XtraEditors.SpinEdit();
            this.SpNumOrderFrom = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciNumOrderFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciNumOrderTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.LcBL_HD_H = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Transaction_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_TransactionCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_KcAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_BillFunAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Exemption = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_ExemptionReason = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_ThucThu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_PayFormName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Tig_TransactionCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_TransactionTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Cashier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_TransactionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_CashierRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_NumOrder = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_AccountBookCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_AccountBookName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_VirPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_GenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_PatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_NationalTransactionCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_ModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Transaction_Modifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CboBillType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciBillType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciSymbolCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTemplateCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTongTien = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTongTienHuy = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTongTienThucNop = new DevExpress.XtraLayout.LayoutControlItem();
            this.LcDateFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcDateTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboDateFrom = new DevExpress.XtraEditors.DateEdit();
            this.cboDateTo = new DevExpress.XtraEditors.DateEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TxtTemplateCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtSymbolCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChkBL_HD_H.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpNumOrderTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpNumOrderFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNumOrderFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNumOrderTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcBL_HD_H)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CboBillType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciBillType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSymbolCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplateCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTienHuy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTienThucNop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcDateFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateTo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblTongTienThucNop);
            this.layoutControl1.Controls.Add(this.lblTongTienHuy);
            this.layoutControl1.Controls.Add(this.lblTongTien);
            this.layoutControl1.Controls.Add(this.BtnPrintNowForXuatBan);
            this.layoutControl1.Controls.Add(this.BtnPrintNow);
            this.layoutControl1.Controls.Add(this.TxtTemplateCode);
            this.layoutControl1.Controls.Add(this.TxtSymbolCode);
            this.layoutControl1.Controls.Add(this.TxtKeyWord);
            this.layoutControl1.Controls.Add(this.BtnPrint);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.BtnRefresh);
            this.layoutControl1.Controls.Add(this.BtnSearch);
            this.layoutControl1.Controls.Add(this.navBarControl1);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.CboBillType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(486, 323, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 393);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblTongTienThucNop
            // 
            this.lblTongTienThucNop.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblTongTienThucNop.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblTongTienThucNop.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTongTienThucNop.Location = new System.Drawing.Point(767, 345);
            this.lblTongTienThucNop.Name = "lblTongTienThucNop";
            this.lblTongTienThucNop.Size = new System.Drawing.Size(111, 20);
            this.lblTongTienThucNop.StyleController = this.layoutControl1;
            this.lblTongTienThucNop.TabIndex = 19;
            // 
            // lblTongTienHuy
            // 
            this.lblTongTienHuy.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblTongTienHuy.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblTongTienHuy.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTongTienHuy.Location = new System.Drawing.Point(511, 345);
            this.lblTongTienHuy.Name = "lblTongTienHuy";
            this.lblTongTienHuy.Size = new System.Drawing.Size(122, 20);
            this.lblTongTienHuy.StyleController = this.layoutControl1;
            this.lblTongTienHuy.TabIndex = 18;
            // 
            // lblTongTien
            // 
            this.lblTongTien.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lblTongTien.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblTongTien.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTongTien.Location = new System.Drawing.Point(297, 345);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(115, 20);
            this.lblTongTien.StyleController = this.layoutControl1;
            this.lblTongTien.TabIndex = 17;
            // 
            // BtnPrintNowForXuatBan
            // 
            this.BtnPrintNowForXuatBan.Location = new System.Drawing.Point(568, 369);
            this.BtnPrintNowForXuatBan.Name = "BtnPrintNowForXuatBan";
            this.BtnPrintNowForXuatBan.Size = new System.Drawing.Size(126, 22);
            this.BtnPrintNowForXuatBan.StyleController = this.layoutControl1;
            this.BtnPrintNowForXuatBan.TabIndex = 16;
            this.BtnPrintNowForXuatBan.Text = "In xuất bán ngay(Ctrl K)";
            this.BtnPrintNowForXuatBan.Click += new System.EventHandler(this.BtnPrintNowForXuatBan_Click);
            // 
            // BtnPrintNow
            // 
            this.BtnPrintNow.Location = new System.Drawing.Point(698, 369);
            this.BtnPrintNow.Name = "BtnPrintNow";
            this.BtnPrintNow.Size = new System.Drawing.Size(104, 22);
            this.BtnPrintNow.StyleController = this.layoutControl1;
            this.BtnPrintNow.TabIndex = 15;
            this.BtnPrintNow.Text = "In ngay(Ctrl I)";
            this.BtnPrintNow.Click += new System.EventHandler(this.BtnPrintNow_Click);
            // 
            // TxtTemplateCode
            // 
            this.TxtTemplateCode.Location = new System.Drawing.Point(97, 50);
            this.TxtTemplateCode.MenuManager = this.barManager1;
            this.TxtTemplateCode.Name = "TxtTemplateCode";
            this.TxtTemplateCode.Size = new System.Drawing.Size(121, 20);
            this.TxtTemplateCode.StyleController = this.layoutControl1;
            this.TxtTemplateCode.TabIndex = 14;
            this.TxtTemplateCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtTemplateCode_PreviewKeyDown);
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
            this.barBtnSearch,
            this.barBtnRefresh,
            this.barBtnPrint,
            this.barBtnPrintNow,
            this.barBtnPrintNowXuatBan});
            this.barManager1.MaxItemId = 5;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.FloatLocation = new System.Drawing.Point(69, 157);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSearch),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnRefresh),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrintNow),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrintNowXuatBan)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSearch
            // 
            this.barBtnSearch.Caption = "Ctrl F";
            this.barBtnSearch.Id = 0;
            this.barBtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnSearch.Name = "barBtnSearch";
            this.barBtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSearch_ItemClick);
            // 
            // barBtnRefresh
            // 
            this.barBtnRefresh.Caption = "Ctrl R";
            this.barBtnRefresh.Id = 1;
            this.barBtnRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barBtnRefresh.Name = "barBtnRefresh";
            this.barBtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnRefresh_ItemClick);
            // 
            // barBtnPrint
            // 
            this.barBtnPrint.Caption = "Ctrl P";
            this.barBtnPrint.Id = 2;
            this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnPrint.Name = "barBtnPrint";
            this.barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            // 
            // barBtnPrintNow
            // 
            this.barBtnPrintNow.Caption = "Ctrl I";
            this.barBtnPrintNow.Id = 3;
            this.barBtnPrintNow.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.barBtnPrintNow.Name = "barBtnPrintNow";
            this.barBtnPrintNow.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrintNow_ItemClick);
            // 
            // barBtnPrintNowXuatBan
            // 
            this.barBtnPrintNowXuatBan.Caption = "Ctrl K";
            this.barBtnPrintNowXuatBan.Id = 4;
            this.barBtnPrintNowXuatBan.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K));
            this.barBtnPrintNowXuatBan.Name = "barBtnPrintNowXuatBan";
            this.barBtnPrintNowXuatBan.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrintNowXuatBan_ItemClick);
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 422);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 393);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 393);
            // 
            // TxtSymbolCode
            // 
            this.TxtSymbolCode.Location = new System.Drawing.Point(97, 74);
            this.TxtSymbolCode.MenuManager = this.barManager1;
            this.TxtSymbolCode.Name = "TxtSymbolCode";
            this.TxtSymbolCode.Size = new System.Drawing.Size(121, 20);
            this.TxtSymbolCode.StyleController = this.layoutControl1;
            this.TxtSymbolCode.TabIndex = 13;
            this.TxtSymbolCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtSymbolCode_PreviewKeyDown);
            // 
            // TxtKeyWord
            // 
            this.TxtKeyWord.Location = new System.Drawing.Point(2, 2);
            this.TxtKeyWord.Name = "TxtKeyWord";
            this.TxtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.TxtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.TxtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.TxtKeyWord.Size = new System.Drawing.Size(216, 20);
            this.TxtKeyWord.StyleController = this.layoutControl1;
            this.TxtKeyWord.TabIndex = 12;
            this.TxtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtKeyWord_PreviewKeyDown);
            // 
            // BtnPrint
            // 
            this.BtnPrint.Location = new System.Drawing.Point(806, 369);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.Size = new System.Drawing.Size(72, 22);
            this.BtnPrint.StyleController = this.layoutControl1;
            this.BtnPrint.TabIndex = 11;
            this.BtnPrint.Text = "In (Ctrl P)";
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(222, 313);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(656, 28);
            this.ucPaging1.TabIndex = 10;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Location = new System.Drawing.Point(112, 369);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(106, 22);
            this.BtnRefresh.StyleController = this.layoutControl1;
            this.BtnRefresh.TabIndex = 9;
            this.BtnRefresh.Text = "Làm lại (Ctrl R)";
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(2, 369);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(106, 22);
            this.BtnSearch.StyleController = this.layoutControl1;
            this.BtnSearch.TabIndex = 8;
            this.BtnSearch.Text = "Tìm (Ctrl F)";
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.BarNumOrder;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.BarNumOrder});
            this.navBarControl1.Location = new System.Drawing.Point(2, 98);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 216;
            this.navBarControl1.Size = new System.Drawing.Size(216, 267);
            this.navBarControl1.TabIndex = 7;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // BarNumOrder
            // 
            this.BarNumOrder.Caption = "Số hóa đơn";
            this.BarNumOrder.ControlContainer = this.navBarGroupControlContainer1;
            this.BarNumOrder.Expanded = true;
            this.BarNumOrder.GroupClientHeight = 135;
            this.BarNumOrder.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.BarNumOrder.Name = "BarNumOrder";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.layoutControl2);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(208, 131);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.ChkBL_HD_H);
            this.layoutControl2.Controls.Add(this.SpNumOrderTo);
            this.layoutControl2.Controls.Add(this.SpNumOrderFrom);
            this.layoutControl2.Controls.Add(this.cboDateFrom);
            this.layoutControl2.Controls.Add(this.cboDateTo);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(208, 131);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // ChkBL_HD_H
            // 
            this.ChkBL_HD_H.Location = new System.Drawing.Point(88, 50);
            this.ChkBL_HD_H.MenuManager = this.barManager1;
            this.ChkBL_HD_H.Name = "ChkBL_HD_H";
            this.ChkBL_HD_H.Properties.Caption = "";
            this.ChkBL_HD_H.Properties.FullFocusRect = true;
            this.ChkBL_HD_H.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.ChkBL_HD_H.Size = new System.Drawing.Size(118, 19);
            this.ChkBL_HD_H.StyleController = this.layoutControl2;
            this.ChkBL_HD_H.TabIndex = 6;
            // 
            // SpNumOrderTo
            // 
            this.SpNumOrderTo.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SpNumOrderTo.Location = new System.Drawing.Point(90, 26);
            this.SpNumOrderTo.Name = "SpNumOrderTo";
            this.SpNumOrderTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.SpNumOrderTo.Properties.IsFloatValue = false;
            this.SpNumOrderTo.Properties.Mask.EditMask = "N00";
            this.SpNumOrderTo.Properties.MaxValue = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.SpNumOrderTo.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SpNumOrderTo.Size = new System.Drawing.Size(116, 20);
            this.SpNumOrderTo.StyleController = this.layoutControl2;
            this.SpNumOrderTo.TabIndex = 5;
            // 
            // SpNumOrderFrom
            // 
            this.SpNumOrderFrom.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SpNumOrderFrom.Location = new System.Drawing.Point(90, 2);
            this.SpNumOrderFrom.Name = "SpNumOrderFrom";
            this.SpNumOrderFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.SpNumOrderFrom.Properties.IsFloatValue = false;
            this.SpNumOrderFrom.Properties.Mask.EditMask = "N00";
            this.SpNumOrderFrom.Properties.MaxValue = new decimal(new int[] {
            -1981284353,
            -1966660860,
            0,
            0});
            this.SpNumOrderFrom.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SpNumOrderFrom.Size = new System.Drawing.Size(116, 20);
            this.SpNumOrderFrom.StyleController = this.layoutControl2;
            this.SpNumOrderFrom.TabIndex = 4;
            this.SpNumOrderFrom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.SpNumOrderFrom_PreviewKeyDown);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LciNumOrderFrom,
            this.LciNumOrderTo,
            this.LcBL_HD_H,
            this.LcDateFrom,
            this.lcDateTo});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(208, 131);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // LciNumOrderFrom
            // 
            this.LciNumOrderFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciNumOrderFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciNumOrderFrom.Control = this.SpNumOrderFrom;
            this.LciNumOrderFrom.Location = new System.Drawing.Point(0, 0);
            this.LciNumOrderFrom.Name = "LciNumOrderFrom";
            this.LciNumOrderFrom.Size = new System.Drawing.Size(208, 24);
            this.LciNumOrderFrom.Text = "Từ:";
            this.LciNumOrderFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciNumOrderFrom.TextSize = new System.Drawing.Size(83, 20);
            this.LciNumOrderFrom.TextToControlDistance = 5;
            // 
            // LciNumOrderTo
            // 
            this.LciNumOrderTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciNumOrderTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciNumOrderTo.Control = this.SpNumOrderTo;
            this.LciNumOrderTo.Location = new System.Drawing.Point(0, 24);
            this.LciNumOrderTo.Name = "LciNumOrderTo";
            this.LciNumOrderTo.Size = new System.Drawing.Size(208, 24);
            this.LciNumOrderTo.Text = "Đến:";
            this.LciNumOrderTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciNumOrderTo.TextSize = new System.Drawing.Size(83, 20);
            this.LciNumOrderTo.TextToControlDistance = 5;
            // 
            // LcBL_HD_H
            // 
            this.LcBL_HD_H.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LcBL_HD_H.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LcBL_HD_H.Control = this.ChkBL_HD_H;
            this.LcBL_HD_H.Location = new System.Drawing.Point(0, 48);
            this.LcBL_HD_H.Name = "LcBL_HD_H";
            this.LcBL_HD_H.OptionsToolTip.ToolTip = "Biên lai/hóa đơn hủy";
            this.LcBL_HD_H.Size = new System.Drawing.Size(208, 24);
            this.LcBL_HD_H.Text = "BL/HĐ hủy:";
            this.LcBL_HD_H.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LcBL_HD_H.TextSize = new System.Drawing.Size(83, 20);
            this.LcBL_HD_H.TextToControlDistance = 3;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(222, 2);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(656, 307);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Transaction_Stt,
            this.gridColumn_Transaction_TransactionCode,
            this.gridColumn_Transaction_Amount,
            this.gridColumn_Transaction_KcAmount,
            this.gridColumn_Transaction_BillFunAmount,
            this.gridColumn_Transaction_Exemption,
            this.gridColumn_Transaction_ExemptionReason,
            this.gridColumn_Transaction_ThucThu,
            this.gridColumn_Transaction_PayFormName,
            this.gridColumn_Transaction_Tig_TransactionCode,
            this.gridColumn_Transaction_TransactionTypeName,
            this.gridColumn_Transaction_Cashier,
            this.gridColumn_Transaction_TransactionTime,
            this.gridColumn_Transaction_CashierRoomName,
            this.gridColumn_Transaction_NumOrder,
            this.gridColumn_Transaction_AccountBookCode,
            this.gridColumn_Transaction_AccountBookName,
            this.gridColumn_Transaction_TreatmentCode,
            this.gridColumn_Transaction_VirPatientName,
            this.gridColumn_Transaction_Dob,
            this.gridColumn_Transaction_GenderName,
            this.gridColumn_Transaction_PatientCode,
            this.gridColumn_Transaction_NationalTransactionCode,
            this.gridColumn_Transaction_CreateTime,
            this.gridColumn_Transaction_Creator,
            this.gridColumn_Transaction_ModifyTime,
            this.gridColumn_Transaction_Modifier});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            // 
            // gridColumn_Transaction_Stt
            // 
            this.gridColumn_Transaction_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Transaction_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Stt.Caption = "STT";
            this.gridColumn_Transaction_Stt.FieldName = "STT";
            this.gridColumn_Transaction_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Transaction_Stt.Name = "gridColumn_Transaction_Stt";
            this.gridColumn_Transaction_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_Stt.Visible = true;
            this.gridColumn_Transaction_Stt.VisibleIndex = 1;
            this.gridColumn_Transaction_Stt.Width = 35;
            // 
            // gridColumn_Transaction_TransactionCode
            // 
            this.gridColumn_Transaction_TransactionCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_TransactionCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_TransactionCode.Caption = "Mã giao dịch";
            this.gridColumn_Transaction_TransactionCode.FieldName = "TRANSACTION_CODE";
            this.gridColumn_Transaction_TransactionCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Transaction_TransactionCode.Name = "gridColumn_Transaction_TransactionCode";
            this.gridColumn_Transaction_TransactionCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_TransactionCode.Visible = true;
            this.gridColumn_Transaction_TransactionCode.VisibleIndex = 2;
            this.gridColumn_Transaction_TransactionCode.Width = 90;
            // 
            // gridColumn_Transaction_Amount
            // 
            this.gridColumn_Transaction_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Transaction_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Amount.Caption = "Số tiền";
            this.gridColumn_Transaction_Amount.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Transaction_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Transaction_Amount.FieldName = "AMOUNT";
            this.gridColumn_Transaction_Amount.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_Transaction_Amount.Name = "gridColumn_Transaction_Amount";
            this.gridColumn_Transaction_Amount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Amount.Visible = true;
            this.gridColumn_Transaction_Amount.VisibleIndex = 3;
            this.gridColumn_Transaction_Amount.Width = 100;
            // 
            // gridColumn_Transaction_KcAmount
            // 
            this.gridColumn_Transaction_KcAmount.Caption = "Kết chuyển";
            this.gridColumn_Transaction_KcAmount.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Transaction_KcAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Transaction_KcAmount.FieldName = "KC_AMOUNT";
            this.gridColumn_Transaction_KcAmount.Name = "gridColumn_Transaction_KcAmount";
            this.gridColumn_Transaction_KcAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_KcAmount.Visible = true;
            this.gridColumn_Transaction_KcAmount.VisibleIndex = 4;
            this.gridColumn_Transaction_KcAmount.Width = 91;
            // 
            // gridColumn_Transaction_BillFunAmount
            // 
            this.gridColumn_Transaction_BillFunAmount.Caption = "Quỹ chi trả";
            this.gridColumn_Transaction_BillFunAmount.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Transaction_BillFunAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Transaction_BillFunAmount.FieldName = "TDL_BILL_FUND_AMOUNT";
            this.gridColumn_Transaction_BillFunAmount.Name = "gridColumn_Transaction_BillFunAmount";
            this.gridColumn_Transaction_BillFunAmount.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_BillFunAmount.Visible = true;
            this.gridColumn_Transaction_BillFunAmount.VisibleIndex = 5;
            this.gridColumn_Transaction_BillFunAmount.Width = 89;
            // 
            // gridColumn_Transaction_Exemption
            // 
            this.gridColumn_Transaction_Exemption.Caption = "Miễn giảm";
            this.gridColumn_Transaction_Exemption.FieldName = "EXEMPTION";
            this.gridColumn_Transaction_Exemption.Name = "gridColumn_Transaction_Exemption";
            this.gridColumn_Transaction_Exemption.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Exemption.Visible = true;
            this.gridColumn_Transaction_Exemption.VisibleIndex = 6;
            this.gridColumn_Transaction_Exemption.Width = 87;
            // 
            // gridColumn_Transaction_ExemptionReason
            // 
            this.gridColumn_Transaction_ExemptionReason.Caption = "Lý do miễn giảm";
            this.gridColumn_Transaction_ExemptionReason.FieldName = "EXEMPTION_REASON";
            this.gridColumn_Transaction_ExemptionReason.Name = "gridColumn_Transaction_ExemptionReason";
            this.gridColumn_Transaction_ExemptionReason.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_ExemptionReason.Visible = true;
            this.gridColumn_Transaction_ExemptionReason.VisibleIndex = 7;
            this.gridColumn_Transaction_ExemptionReason.Width = 108;
            // 
            // gridColumn_Transaction_ThucThu
            // 
            this.gridColumn_Transaction_ThucThu.Caption = "Thực thu";
            this.gridColumn_Transaction_ThucThu.DisplayFormat.FormatString = "#,##0.0000";
            this.gridColumn_Transaction_ThucThu.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Transaction_ThucThu.FieldName = "THUC_THU_STR";
            this.gridColumn_Transaction_ThucThu.Name = "gridColumn_Transaction_ThucThu";
            this.gridColumn_Transaction_ThucThu.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_ThucThu.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_ThucThu.Visible = true;
            this.gridColumn_Transaction_ThucThu.VisibleIndex = 8;
            this.gridColumn_Transaction_ThucThu.Width = 109;
            // 
            // gridColumn_Transaction_PayFormName
            // 
            this.gridColumn_Transaction_PayFormName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_PayFormName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_PayFormName.Caption = "Hình thức";
            this.gridColumn_Transaction_PayFormName.FieldName = "PAY_FORM_NAME";
            this.gridColumn_Transaction_PayFormName.Name = "gridColumn_Transaction_PayFormName";
            this.gridColumn_Transaction_PayFormName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_PayFormName.Visible = true;
            this.gridColumn_Transaction_PayFormName.VisibleIndex = 9;
            this.gridColumn_Transaction_PayFormName.Width = 110;
            // 
            // gridColumn_Transaction_Tig_TransactionCode
            // 
            this.gridColumn_Transaction_Tig_TransactionCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Tig_TransactionCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Tig_TransactionCode.Caption = "Mã giao dịch thẻ";
            this.gridColumn_Transaction_Tig_TransactionCode.FieldName = "TIG_TRANSACTION_CODE";
            this.gridColumn_Transaction_Tig_TransactionCode.Name = "gridColumn_Transaction_Tig_TransactionCode";
            this.gridColumn_Transaction_Tig_TransactionCode.Visible = true;
            this.gridColumn_Transaction_Tig_TransactionCode.VisibleIndex = 10;
            this.gridColumn_Transaction_Tig_TransactionCode.Width = 110;
            // 
            // gridColumn_Transaction_TransactionTypeName
            // 
            this.gridColumn_Transaction_TransactionTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_TransactionTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_TransactionTypeName.Caption = "Loại giao dịch";
            this.gridColumn_Transaction_TransactionTypeName.FieldName = "TRANSACTION_TYPE_NAME";
            this.gridColumn_Transaction_TransactionTypeName.Name = "gridColumn_Transaction_TransactionTypeName";
            this.gridColumn_Transaction_TransactionTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_TransactionTypeName.Visible = true;
            this.gridColumn_Transaction_TransactionTypeName.VisibleIndex = 11;
            // 
            // gridColumn_Transaction_Cashier
            // 
            this.gridColumn_Transaction_Cashier.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Cashier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Cashier.Caption = "Giao dịch viên";
            this.gridColumn_Transaction_Cashier.FieldName = "CASHIER";
            this.gridColumn_Transaction_Cashier.Name = "gridColumn_Transaction_Cashier";
            this.gridColumn_Transaction_Cashier.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Cashier.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_Cashier.Visible = true;
            this.gridColumn_Transaction_Cashier.VisibleIndex = 12;
            this.gridColumn_Transaction_Cashier.Width = 120;
            // 
            // gridColumn_Transaction_TransactionTime
            // 
            this.gridColumn_Transaction_TransactionTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_TransactionTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_TransactionTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_TransactionTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_TransactionTime.Caption = "Thời gian giao dịch";
            this.gridColumn_Transaction_TransactionTime.FieldName = "TRANSACTION_TIME_STR";
            this.gridColumn_Transaction_TransactionTime.Name = "gridColumn_Transaction_TransactionTime";
            this.gridColumn_Transaction_TransactionTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_TransactionTime.ToolTip = "Thời gian giao dịch";
            this.gridColumn_Transaction_TransactionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_TransactionTime.Visible = true;
            this.gridColumn_Transaction_TransactionTime.VisibleIndex = 13;
            this.gridColumn_Transaction_TransactionTime.Width = 120;
            // 
            // gridColumn_Transaction_CashierRoomName
            // 
            this.gridColumn_Transaction_CashierRoomName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_CashierRoomName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_CashierRoomName.Caption = "Phòng thu ngân";
            this.gridColumn_Transaction_CashierRoomName.FieldName = "CASHIER_ROOM_NAME";
            this.gridColumn_Transaction_CashierRoomName.Name = "gridColumn_Transaction_CashierRoomName";
            this.gridColumn_Transaction_CashierRoomName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_CashierRoomName.Visible = true;
            this.gridColumn_Transaction_CashierRoomName.VisibleIndex = 14;
            this.gridColumn_Transaction_CashierRoomName.Width = 120;
            // 
            // gridColumn_Transaction_NumOrder
            // 
            this.gridColumn_Transaction_NumOrder.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_NumOrder.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_Transaction_NumOrder.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_NumOrder.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_NumOrder.Caption = "Số";
            this.gridColumn_Transaction_NumOrder.DisplayFormat.FormatString = "#,##";
            this.gridColumn_Transaction_NumOrder.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_Transaction_NumOrder.FieldName = "NUM_ORDER";
            this.gridColumn_Transaction_NumOrder.Name = "gridColumn_Transaction_NumOrder";
            this.gridColumn_Transaction_NumOrder.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_NumOrder.Visible = true;
            this.gridColumn_Transaction_NumOrder.VisibleIndex = 15;
            this.gridColumn_Transaction_NumOrder.Width = 60;
            // 
            // gridColumn_Transaction_AccountBookCode
            // 
            this.gridColumn_Transaction_AccountBookCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_AccountBookCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_AccountBookCode.Caption = "Mã sổ";
            this.gridColumn_Transaction_AccountBookCode.FieldName = "ACCOUNT_BOOK_CODE";
            this.gridColumn_Transaction_AccountBookCode.Name = "gridColumn_Transaction_AccountBookCode";
            this.gridColumn_Transaction_AccountBookCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_AccountBookCode.Visible = true;
            this.gridColumn_Transaction_AccountBookCode.VisibleIndex = 16;
            this.gridColumn_Transaction_AccountBookCode.Width = 65;
            // 
            // gridColumn_Transaction_AccountBookName
            // 
            this.gridColumn_Transaction_AccountBookName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_AccountBookName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_AccountBookName.Caption = "Tên sổ";
            this.gridColumn_Transaction_AccountBookName.FieldName = "ACCOUNT_BOOK_NAME";
            this.gridColumn_Transaction_AccountBookName.Name = "gridColumn_Transaction_AccountBookName";
            this.gridColumn_Transaction_AccountBookName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_AccountBookName.Visible = true;
            this.gridColumn_Transaction_AccountBookName.VisibleIndex = 17;
            this.gridColumn_Transaction_AccountBookName.Width = 120;
            // 
            // gridColumn_Transaction_TreatmentCode
            // 
            this.gridColumn_Transaction_TreatmentCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_TreatmentCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_TreatmentCode.Caption = "Mã điều trị";
            this.gridColumn_Transaction_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.gridColumn_Transaction_TreatmentCode.Name = "gridColumn_Transaction_TreatmentCode";
            this.gridColumn_Transaction_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_TreatmentCode.Visible = true;
            this.gridColumn_Transaction_TreatmentCode.VisibleIndex = 18;
            this.gridColumn_Transaction_TreatmentCode.Width = 90;
            // 
            // gridColumn_Transaction_VirPatientName
            // 
            this.gridColumn_Transaction_VirPatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_VirPatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_VirPatientName.Caption = "Bệnh nhân";
            this.gridColumn_Transaction_VirPatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumn_Transaction_VirPatientName.Name = "gridColumn_Transaction_VirPatientName";
            this.gridColumn_Transaction_VirPatientName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_VirPatientName.Visible = true;
            this.gridColumn_Transaction_VirPatientName.VisibleIndex = 19;
            this.gridColumn_Transaction_VirPatientName.Width = 90;
            // 
            // gridColumn_Transaction_Dob
            // 
            this.gridColumn_Transaction_Dob.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Dob.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Dob.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Dob.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Dob.Caption = "Ngày sinh";
            this.gridColumn_Transaction_Dob.FieldName = "DOB_STR";
            this.gridColumn_Transaction_Dob.Name = "gridColumn_Transaction_Dob";
            this.gridColumn_Transaction_Dob.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_Dob.Visible = true;
            this.gridColumn_Transaction_Dob.VisibleIndex = 20;
            this.gridColumn_Transaction_Dob.Width = 80;
            // 
            // gridColumn_Transaction_GenderName
            // 
            this.gridColumn_Transaction_GenderName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_GenderName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_GenderName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_GenderName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_GenderName.Caption = "Giới tính";
            this.gridColumn_Transaction_GenderName.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gridColumn_Transaction_GenderName.Name = "gridColumn_Transaction_GenderName";
            this.gridColumn_Transaction_GenderName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_GenderName.Visible = true;
            this.gridColumn_Transaction_GenderName.VisibleIndex = 21;
            this.gridColumn_Transaction_GenderName.Width = 55;
            // 
            // gridColumn_Transaction_PatientCode
            // 
            this.gridColumn_Transaction_PatientCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_PatientCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_PatientCode.Caption = "Mã bệnh nhân";
            this.gridColumn_Transaction_PatientCode.FieldName = "TDL_PATIENT_CODE";
            this.gridColumn_Transaction_PatientCode.Name = "gridColumn_Transaction_PatientCode";
            this.gridColumn_Transaction_PatientCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_PatientCode.Visible = true;
            this.gridColumn_Transaction_PatientCode.VisibleIndex = 22;
            this.gridColumn_Transaction_PatientCode.Width = 90;
            // 
            // gridColumn_Transaction_NationalTransactionCode
            // 
            this.gridColumn_Transaction_NationalTransactionCode.Caption = "Mã quốc gia";
            this.gridColumn_Transaction_NationalTransactionCode.FieldName = "NATIONAL_TRANSACTION_CODE";
            this.gridColumn_Transaction_NationalTransactionCode.Name = "gridColumn_Transaction_NationalTransactionCode";
            this.gridColumn_Transaction_NationalTransactionCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_NationalTransactionCode.Visible = true;
            this.gridColumn_Transaction_NationalTransactionCode.VisibleIndex = 23;
            this.gridColumn_Transaction_NationalTransactionCode.Width = 90;
            // 
            // gridColumn_Transaction_CreateTime
            // 
            this.gridColumn_Transaction_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_CreateTime.Caption = "Thời gian tạo";
            this.gridColumn_Transaction_CreateTime.FieldName = "CREATE_TIME_STR";
            this.gridColumn_Transaction_CreateTime.Name = "gridColumn_Transaction_CreateTime";
            this.gridColumn_Transaction_CreateTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_CreateTime.Visible = true;
            this.gridColumn_Transaction_CreateTime.VisibleIndex = 24;
            this.gridColumn_Transaction_CreateTime.Width = 120;
            // 
            // gridColumn_Transaction_Creator
            // 
            this.gridColumn_Transaction_Creator.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Creator.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Creator.Caption = "Người tạo";
            this.gridColumn_Transaction_Creator.FieldName = "CREATOR";
            this.gridColumn_Transaction_Creator.Name = "gridColumn_Transaction_Creator";
            this.gridColumn_Transaction_Creator.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Creator.Visible = true;
            this.gridColumn_Transaction_Creator.VisibleIndex = 25;
            // 
            // gridColumn_Transaction_ModifyTime
            // 
            this.gridColumn_Transaction_ModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Transaction_ModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_ModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_ModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_ModifyTime.Caption = "Thời gian sửa";
            this.gridColumn_Transaction_ModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gridColumn_Transaction_ModifyTime.Name = "gridColumn_Transaction_ModifyTime";
            this.gridColumn_Transaction_ModifyTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_ModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Transaction_ModifyTime.Visible = true;
            this.gridColumn_Transaction_ModifyTime.VisibleIndex = 26;
            this.gridColumn_Transaction_ModifyTime.Width = 120;
            // 
            // gridColumn_Transaction_Modifier
            // 
            this.gridColumn_Transaction_Modifier.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Transaction_Modifier.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Transaction_Modifier.Caption = "Người sửa";
            this.gridColumn_Transaction_Modifier.FieldName = "MODIFIER";
            this.gridColumn_Transaction_Modifier.Name = "gridColumn_Transaction_Modifier";
            this.gridColumn_Transaction_Modifier.OptionsColumn.AllowEdit = false;
            this.gridColumn_Transaction_Modifier.Visible = true;
            this.gridColumn_Transaction_Modifier.VisibleIndex = 27;
            // 
            // CboBillType
            // 
            this.CboBillType.Location = new System.Drawing.Point(97, 26);
            this.CboBillType.Name = "CboBillType";
            this.CboBillType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.CboBillType.Properties.NullText = "";
            this.CboBillType.Properties.View = this.gridLookUpEdit1View;
            this.CboBillType.Size = new System.Drawing.Size(121, 20);
            this.CboBillType.StyleController = this.layoutControl1;
            this.CboBillType.TabIndex = 4;
            this.CboBillType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.CboBillType_Closed);
            this.CboBillType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.CboBillType_PreviewKeyDown);
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
            this.LciBillType,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem1,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.LciSymbolCode,
            this.lciTemplateCode,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.lciTongTien,
            this.lciTongTienHuy,
            this.lciTongTienThucNop});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 393);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // LciBillType
            // 
            this.LciBillType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciBillType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciBillType.Control = this.CboBillType;
            this.LciBillType.Location = new System.Drawing.Point(0, 24);
            this.LciBillType.Name = "LciBillType";
            this.LciBillType.Size = new System.Drawing.Size(220, 24);
            this.LciBillType.Text = "Loại hóa đơn:";
            this.LciBillType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciBillType.TextSize = new System.Drawing.Size(90, 20);
            this.LciBillType.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(220, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(660, 311);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.navBarControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(220, 271);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.BtnSearch;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 367);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.BtnRefresh;
            this.layoutControlItem6.Location = new System.Drawing.Point(110, 367);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.ucPaging1;
            this.layoutControlItem7.Location = new System.Drawing.Point(220, 311);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(660, 32);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(220, 367);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(346, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.BtnPrint;
            this.layoutControlItem8.Location = new System.Drawing.Point(804, 367);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(76, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.TxtKeyWord;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // LciSymbolCode
            // 
            this.LciSymbolCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciSymbolCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciSymbolCode.Control = this.TxtSymbolCode;
            this.LciSymbolCode.Location = new System.Drawing.Point(0, 72);
            this.LciSymbolCode.Name = "LciSymbolCode";
            this.LciSymbolCode.Size = new System.Drawing.Size(220, 24);
            this.LciSymbolCode.Text = "Ký hiệu:";
            this.LciSymbolCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciSymbolCode.TextSize = new System.Drawing.Size(90, 20);
            this.LciSymbolCode.TextToControlDistance = 5;
            // 
            // lciTemplateCode
            // 
            this.lciTemplateCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTemplateCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTemplateCode.Control = this.TxtTemplateCode;
            this.lciTemplateCode.Location = new System.Drawing.Point(0, 48);
            this.lciTemplateCode.Name = "lciTemplateCode";
            this.lciTemplateCode.Size = new System.Drawing.Size(220, 24);
            this.lciTemplateCode.Text = "Mẫu số:";
            this.lciTemplateCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTemplateCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciTemplateCode.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.BtnPrintNow;
            this.layoutControlItem1.Location = new System.Drawing.Point(696, 367);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(108, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.BtnPrintNowForXuatBan;
            this.layoutControlItem3.Location = new System.Drawing.Point(566, 367);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(130, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciTongTien
            // 
            this.lciTongTien.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lciTongTien.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lciTongTien.AppearanceItemCaption.Options.UseFont = true;
            this.lciTongTien.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTongTien.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTongTien.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTongTien.Control = this.lblTongTien;
            this.lciTongTien.Location = new System.Drawing.Point(220, 343);
            this.lciTongTien.Name = "lciTongTien";
            this.lciTongTien.Size = new System.Drawing.Size(194, 24);
            this.lciTongTien.Text = "Tổng tiền:";
            this.lciTongTien.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTongTien.TextSize = new System.Drawing.Size(70, 20);
            this.lciTongTien.TextToControlDistance = 5;
            // 
            // lciTongTienHuy
            // 
            this.lciTongTienHuy.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lciTongTienHuy.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lciTongTienHuy.AppearanceItemCaption.Options.UseFont = true;
            this.lciTongTienHuy.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTongTienHuy.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTongTienHuy.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTongTienHuy.Control = this.lblTongTienHuy;
            this.lciTongTienHuy.Location = new System.Drawing.Point(414, 343);
            this.lciTongTienHuy.Name = "lciTongTienHuy";
            this.lciTongTienHuy.Size = new System.Drawing.Size(221, 24);
            this.lciTongTienHuy.Text = "Tổng tiền hủy:";
            this.lciTongTienHuy.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTongTienHuy.TextSize = new System.Drawing.Size(90, 20);
            this.lciTongTienHuy.TextToControlDistance = 5;
            // 
            // lciTongTienThucNop
            // 
            this.lciTongTienThucNop.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lciTongTienThucNop.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lciTongTienThucNop.AppearanceItemCaption.Options.UseFont = true;
            this.lciTongTienThucNop.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTongTienThucNop.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTongTienThucNop.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTongTienThucNop.Control = this.lblTongTienThucNop;
            this.lciTongTienThucNop.Location = new System.Drawing.Point(635, 343);
            this.lciTongTienThucNop.Name = "lciTongTienThucNop";
            this.lciTongTienThucNop.Size = new System.Drawing.Size(245, 24);
            this.lciTongTienThucNop.Text = "Tổng tiền thực nộp:";
            this.lciTongTienThucNop.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTongTienThucNop.TextSize = new System.Drawing.Size(125, 20);
            this.lciTongTienThucNop.TextToControlDistance = 5;
            // 
            // LcDateFrom
            // 
            this.LcDateFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LcDateFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LcDateFrom.Control = this.cboDateFrom;
            this.LcDateFrom.Location = new System.Drawing.Point(0, 72);
            this.LcDateFrom.Name = "LcDateFrom";
            this.LcDateFrom.Size = new System.Drawing.Size(208, 24);
            this.LcDateFrom.Text = "Từ ngày:";
            this.LcDateFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LcDateFrom.TextSize = new System.Drawing.Size(83, 20);
            this.LcDateFrom.TextToControlDistance = 5;
            // 
            // lcDateTo
            // 
            this.lcDateTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcDateTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcDateTo.Control = this.cboDateTo;
            this.lcDateTo.Location = new System.Drawing.Point(0, 96);
            this.lcDateTo.Name = "lcDateTo";
            this.lcDateTo.Size = new System.Drawing.Size(208, 35);
            this.lcDateTo.Text = "Đến ngày:";
            this.lcDateTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcDateTo.TextSize = new System.Drawing.Size(83, 20);
            this.lcDateTo.TextToControlDistance = 5;
            // 
            // cboDateFrom
            // 
            this.cboDateFrom.EditValue = null;
            this.cboDateFrom.Location = new System.Drawing.Point(90, 74);
            this.cboDateFrom.MenuManager = this.barManager1;
            this.cboDateFrom.Name = "cboDateFrom";
            this.cboDateFrom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDateFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.cboDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.cboDateFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.cboDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.cboDateFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.cboDateFrom.Size = new System.Drawing.Size(116, 20);
            this.cboDateFrom.StyleController = this.layoutControl2;
            this.cboDateFrom.TabIndex = 7;
            // 
            // cboDateTo
            // 
            this.cboDateTo.EditValue = null;
            this.cboDateTo.Location = new System.Drawing.Point(90, 98);
            this.cboDateTo.MenuManager = this.barManager1;
            this.cboDateTo.Name = "cboDateTo";
            this.cboDateTo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDateTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.cboDateTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.cboDateTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.cboDateTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.cboDateTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.cboDateTo.Size = new System.Drawing.Size(116, 20);
            this.cboDateTo.StyleController = this.layoutControl2;
            this.cboDateTo.TabIndex = 8;
            // 
            // FormTransactionBillListPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 422);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormTransactionBillListPrint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTransactionBillListPrint";
            this.Load += new System.EventHandler(this.FormTransactionBillListPrint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TxtTemplateCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtSymbolCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ChkBL_HD_H.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpNumOrderTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpNumOrderFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNumOrderFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNumOrderTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcBL_HD_H)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CboBillType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciBillType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSymbolCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplateCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTienHuy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTongTienThucNop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcDateFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcDateTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDateTo.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.GridLookUpEdit CboBillType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem LciBillType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.TextEdit TxtKeyWord;
        private DevExpress.XtraEditors.SimpleButton BtnPrint;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraEditors.SimpleButton BtnRefresh;
        private DevExpress.XtraEditors.SimpleButton BtnSearch;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup BarNumOrder;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.SpinEdit SpNumOrderTo;
        private DevExpress.XtraEditors.SpinEdit SpNumOrderFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem LciNumOrderFrom;
        private DevExpress.XtraLayout.LayoutControlItem LciNumOrderTo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_TransactionCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Tig_TransactionCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_KcAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_BillFunAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Exemption;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_ExemptionReason;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_ThucThu;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_PayFormName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Cashier;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_CashierRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_NumOrder;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_AccountBookCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_AccountBookName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_TransactionTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_TransactionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_VirPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Dob;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_GenderName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_PatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_NationalTransactionCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Creator;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_ModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Transaction_Modifier;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSearch;
        private DevExpress.XtraBars.BarButtonItem barBtnRefresh;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.TextEdit TxtTemplateCode;
        private DevExpress.XtraEditors.TextEdit TxtSymbolCode;
        private DevExpress.XtraLayout.LayoutControlItem LciSymbolCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTemplateCode;
        private DevExpress.XtraEditors.SimpleButton BtnPrintNow;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem barBtnPrintNow;
        private DevExpress.XtraEditors.SimpleButton BtnPrintNowForXuatBan;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraBars.BarButtonItem barBtnPrintNowXuatBan;
        private DevExpress.XtraEditors.LabelControl lblTongTienThucNop;
        private DevExpress.XtraEditors.LabelControl lblTongTienHuy;
        private DevExpress.XtraEditors.LabelControl lblTongTien;
        private DevExpress.XtraLayout.LayoutControlItem lciTongTien;
        private DevExpress.XtraLayout.LayoutControlItem lciTongTienHuy;
        private DevExpress.XtraLayout.LayoutControlItem lciTongTienThucNop;
        private DevExpress.XtraEditors.CheckEdit ChkBL_HD_H;
        private DevExpress.XtraLayout.LayoutControlItem LcBL_HD_H;
        private DevExpress.XtraEditors.DateEdit cboDateFrom;
        private DevExpress.XtraEditors.DateEdit cboDateTo;
        private DevExpress.XtraLayout.LayoutControlItem LcDateFrom;
        private DevExpress.XtraLayout.LayoutControlItem lcDateTo;
    }
}
