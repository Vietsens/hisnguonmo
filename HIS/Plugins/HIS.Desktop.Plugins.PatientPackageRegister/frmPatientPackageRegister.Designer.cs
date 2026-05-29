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
namespace HIS.Desktop.Plugins.PatientPackageRegister
{
    partial class frmPatientPackageRegister
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
            DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItemLuu = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemInPhieu = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.grpTimKiemBN = new System.Windows.Forms.GroupBox();
            this.layoutControlSearch = new DevExpress.XtraLayout.LayoutControl();
            this.txtMaBenhNhan = new DevExpress.XtraEditors.TextEdit();
            this.btnTimKiem = new DevExpress.XtraEditors.SimpleButton();
            this.btnDanhSachGoi = new DevExpress.XtraEditors.DropDownButton();
            this.rootSearch = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMaBenhNhan = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnTimKiem = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnDanhSachGoi = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItemTimKiem = new DevExpress.XtraLayout.EmptySpaceItem();
            this.grpHanhChinh = new System.Windows.Forms.GroupBox();
            this.layoutControlHC = new DevExpress.XtraLayout.LayoutControl();
            this.lblHoTen = new DevExpress.XtraEditors.LabelControl();
            this.lblNgaySinh = new DevExpress.XtraEditors.LabelControl();
            this.lblGioiTinh = new DevExpress.XtraEditors.LabelControl();
            this.lblCCCD = new DevExpress.XtraEditors.LabelControl();
            this.lblDienThoai = new DevExpress.XtraEditors.LabelControl();
            this.lblDiaChi = new DevExpress.XtraEditors.LabelControl();
            this.rootHC = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciHoTen = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNgaySinh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGioiTinh = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCCCD = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDienThoai = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDiaChi = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpThongTinGoi = new System.Windows.Forms.GroupBox();
            this.layoutControlInfo = new DevExpress.XtraLayout.LayoutControl();
            this.cboMauGoi = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewMauGoi = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtTenGoi = new DevExpress.XtraEditors.TextEdit();
            this.dteNgayDangKy = new DevExpress.XtraEditors.DateEdit();
            this.cboDoiTuongTT = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewDoiTuongTT = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboTrangThai = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewTrangThai = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.memGhiChu = new DevExpress.XtraEditors.MemoEdit();
            this.rootInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMauGoi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTenGoi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNgayDangKy = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDoiTuongTT = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTrangThai = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGhiChu = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.splitContainerDV = new System.Windows.Forms.SplitContainer();
            this.grpDanhMucDV = new System.Windows.Forms.GroupBox();
            this.layoutControlDanhMuc = new DevExpress.XtraLayout.LayoutControl();
            this.txtTimKiemDV = new DevExpress.XtraEditors.TextEdit();
            this.cboLoaiDV = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewLoaiDV = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdDanhMucDV = new DevExpress.XtraGrid.GridControl();
            this.gvDanhMucDV = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.rootDanhMuc = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTimKiemDV = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciLoaiDV = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrdDanhMucDV = new DevExpress.XtraLayout.LayoutControlItem();
            this.grpDichVuTrongGoi = new System.Windows.Forms.GroupBox();
            this.layoutControlDVTrongGoi = new DevExpress.XtraLayout.LayoutControl();
            this.btnPhiGoi = new DevExpress.XtraEditors.SimpleButton();
            this.grdDichVuTrongGoi = new DevExpress.XtraGrid.GridControl();
            this.gvDichVuTrongGoi = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblTongCong = new DevExpress.XtraEditors.LabelControl();
            this.rootDVTrongGoi = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnPhiGoi = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrdDichVuTrongGoi = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.layoutControlButtons = new DevExpress.XtraLayout.LayoutControl();
            this.btnInPhieu = new DevExpress.XtraEditors.SimpleButton();
            this.btnHuyBo = new DevExpress.XtraEditors.SimpleButton();
            this.btnLuu = new DevExpress.XtraEditors.SimpleButton();
            this.rootButtons = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnInPhieu = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnHuyBo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnLuu = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.grpTimKiemBN.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlSearch)).BeginInit();
            this.layoutControlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaBenhNhan.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaBenhNhan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnTimKiem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDanhSachGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTimKiem)).BeginInit();
            this.grpHanhChinh.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlHC)).BeginInit();
            this.layoutControlHC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rootHC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHoTen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgaySinh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGioiTinh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCCCD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDienThoai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDiaChi)).BeginInit();
            this.grpThongTinGoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlInfo)).BeginInit();
            this.layoutControlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMauGoi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMauGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenGoi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteNgayDangKy.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteNgayDangKy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDoiTuongTT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDoiTuongTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTrangThai.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTrangThai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memGhiChu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMauGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTenGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgayDangKy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDoiTuongTT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTrangThai)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGhiChu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDV)).BeginInit();
            this.splitContainerDV.Panel1.SuspendLayout();
            this.splitContainerDV.Panel2.SuspendLayout();
            this.splitContainerDV.SuspendLayout();
            this.grpDanhMucDV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDanhMuc)).BeginInit();
            this.layoutControlDanhMuc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimKiemDV.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLoaiDV.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLoaiDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDanhMucDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDanhMucDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootDanhMuc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimKiemDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLoaiDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrdDanhMucDV)).BeginInit();
            this.grpDichVuTrongGoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDVTrongGoi)).BeginInit();
            this.layoutControlDVTrongGoi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDichVuTrongGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDichVuTrongGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootDVTrongGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPhiGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrdDichVuTrongGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).BeginInit();
            this.layoutControlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rootButtons)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnInPhieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnHuyBo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnLuu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            this.SuspendLayout();
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
            this.barButtonItemLuu,
            this.barButtonItemInPhieu});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItemLuu
            // 
            this.barButtonItemLuu.Caption = "Lưu";
            this.barButtonItemLuu.Id = 0;
            this.barButtonItemLuu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItemLuu.Name = "barButtonItemLuu";
            this.barButtonItemLuu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemLuu_ItemClick);
            // 
            // barButtonItemInPhieu
            // 
            this.barButtonItemInPhieu.Caption = "In phiếu";
            this.barButtonItemInPhieu.Id = 1;
            this.barButtonItemInPhieu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barButtonItemInPhieu.Name = "barButtonItemInPhieu";
            this.barButtonItemInPhieu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemInPhieu_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(2, 2);
            this.barDockControlTop.Size = new System.Drawing.Size(1336, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(2, 595);
            this.barDockControlBottom.Size = new System.Drawing.Size(1336, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(2, 31);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 564);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1338, 31);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 564);
            // 
            // grpTimKiemBN
            // 
            this.grpTimKiemBN.Controls.Add(this.layoutControlSearch);
            this.grpTimKiemBN.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpTimKiemBN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTimKiemBN.Location = new System.Drawing.Point(2, 31);
            this.grpTimKiemBN.Margin = new System.Windows.Forms.Padding(0);
            this.grpTimKiemBN.Name = "grpTimKiemBN";
            this.grpTimKiemBN.Padding = new System.Windows.Forms.Padding(5);
            this.grpTimKiemBN.Size = new System.Drawing.Size(1336, 49);
            this.grpTimKiemBN.TabIndex = 0;
            this.grpTimKiemBN.TabStop = false;
            this.grpTimKiemBN.Text = "Tìm kiếm bệnh nhân";
            // 
            // layoutControlSearch
            // 
            this.layoutControlSearch.Controls.Add(this.txtMaBenhNhan);
            this.layoutControlSearch.Controls.Add(this.btnTimKiem);
            this.layoutControlSearch.Controls.Add(this.btnDanhSachGoi);
            this.layoutControlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlSearch.Location = new System.Drawing.Point(5, 18);
            this.layoutControlSearch.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlSearch.Name = "layoutControlSearch";
            this.layoutControlSearch.Root = this.rootSearch;
            this.layoutControlSearch.Size = new System.Drawing.Size(1326, 26);
            this.layoutControlSearch.TabIndex = 0;
            // 
            // txtMaBenhNhan
            // 
            this.txtMaBenhNhan.Location = new System.Drawing.Point(97, 2);
            this.txtMaBenhNhan.MenuManager = this.barManager1;
            this.txtMaBenhNhan.Name = "txtMaBenhNhan";
            this.txtMaBenhNhan.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtMaBenhNhan.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtMaBenhNhan.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtMaBenhNhan.Size = new System.Drawing.Size(199, 20);
            this.txtMaBenhNhan.StyleController = this.layoutControlSearch;
            this.txtMaBenhNhan.TabIndex = 0;
            this.txtMaBenhNhan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMaBenhNhan_KeyDown);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Location = new System.Drawing.Point(300, 2);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(93, 22);
            this.btnTimKiem.StyleController = this.layoutControlSearch;
            this.btnTimKiem.TabIndex = 1;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // btnDanhSachGoi
            // 
            this.btnDanhSachGoi.Location = new System.Drawing.Point(397, 2);
            this.btnDanhSachGoi.Name = "btnDanhSachGoi";
            this.btnDanhSachGoi.Size = new System.Drawing.Size(130, 22);
            this.btnDanhSachGoi.StyleController = this.layoutControlSearch;
            this.btnDanhSachGoi.TabIndex = 2;
            this.btnDanhSachGoi.Text = "Danh sách gói";
            this.btnDanhSachGoi.Click += new System.EventHandler(this.btnDanhSachGoi_Click);
            // 
            // rootSearch
            // 
            this.rootSearch.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootSearch.GroupBordersVisible = false;
            this.rootSearch.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMaBenhNhan,
            this.lciBtnTimKiem,
            this.lciBtnDanhSachGoi,
            this.emptySpaceItemTimKiem});
            this.rootSearch.Location = new System.Drawing.Point(0, 0);
            this.rootSearch.Name = "rootSearch";
            this.rootSearch.Size = new System.Drawing.Size(1326, 26);
            this.rootSearch.TextVisible = false;
            // 
            // lciMaBenhNhan
            // 
            this.lciMaBenhNhan.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMaBenhNhan.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMaBenhNhan.Control = this.txtMaBenhNhan;
            this.lciMaBenhNhan.Location = new System.Drawing.Point(0, 0);
            this.lciMaBenhNhan.Name = "lciMaBenhNhan";
            this.lciMaBenhNhan.Size = new System.Drawing.Size(298, 26);
            this.lciMaBenhNhan.Text = "Mã bệnh nhân:";
            this.lciMaBenhNhan.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMaBenhNhan.TextSize = new System.Drawing.Size(90, 20);
            this.lciMaBenhNhan.TextToControlDistance = 5;
            // 
            // lciBtnTimKiem
            // 
            this.lciBtnTimKiem.Control = this.btnTimKiem;
            this.lciBtnTimKiem.Location = new System.Drawing.Point(298, 0);
            this.lciBtnTimKiem.Name = "lciBtnTimKiem";
            this.lciBtnTimKiem.Size = new System.Drawing.Size(97, 26);
            this.lciBtnTimKiem.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnTimKiem.TextVisible = false;
            // 
            // lciBtnDanhSachGoi
            // 
            this.lciBtnDanhSachGoi.Control = this.btnDanhSachGoi;
            this.lciBtnDanhSachGoi.Location = new System.Drawing.Point(395, 0);
            this.lciBtnDanhSachGoi.Name = "lciBtnDanhSachGoi";
            this.lciBtnDanhSachGoi.Size = new System.Drawing.Size(134, 26);
            this.lciBtnDanhSachGoi.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDanhSachGoi.TextVisible = false;
            // 
            // emptySpaceItemTimKiem
            // 
            this.emptySpaceItemTimKiem.AllowHotTrack = false;
            this.emptySpaceItemTimKiem.Location = new System.Drawing.Point(529, 0);
            this.emptySpaceItemTimKiem.Name = "emptySpaceItemTimKiem";
            this.emptySpaceItemTimKiem.Size = new System.Drawing.Size(797, 26);
            this.emptySpaceItemTimKiem.TextSize = new System.Drawing.Size(0, 0);
            // 
            // grpHanhChinh
            // 
            this.grpHanhChinh.Controls.Add(this.layoutControlHC);
            this.grpHanhChinh.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHanhChinh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpHanhChinh.Location = new System.Drawing.Point(2, 80);
            this.grpHanhChinh.Margin = new System.Windows.Forms.Padding(0);
            this.grpHanhChinh.Name = "grpHanhChinh";
            this.grpHanhChinh.Padding = new System.Windows.Forms.Padding(5);
            this.grpHanhChinh.Size = new System.Drawing.Size(1336, 74);
            this.grpHanhChinh.TabIndex = 1;
            this.grpHanhChinh.TabStop = false;
            this.grpHanhChinh.Text = "Thông tin hành chính";
            // 
            // layoutControlHC
            // 
            this.layoutControlHC.Controls.Add(this.lblHoTen);
            this.layoutControlHC.Controls.Add(this.lblNgaySinh);
            this.layoutControlHC.Controls.Add(this.lblGioiTinh);
            this.layoutControlHC.Controls.Add(this.lblCCCD);
            this.layoutControlHC.Controls.Add(this.lblDienThoai);
            this.layoutControlHC.Controls.Add(this.lblDiaChi);
            this.layoutControlHC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlHC.Location = new System.Drawing.Point(5, 18);
            this.layoutControlHC.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlHC.Name = "layoutControlHC";
            this.layoutControlHC.Root = this.rootHC;
            this.layoutControlHC.Size = new System.Drawing.Size(1326, 51);
            this.layoutControlHC.TabIndex = 0;
            // 
            // lblHoTen
            // 
            this.lblHoTen.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblHoTen.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHoTen.Location = new System.Drawing.Point(97, 2);
            this.lblHoTen.Name = "lblHoTen";
            this.lblHoTen.Size = new System.Drawing.Size(222, 20);
            this.lblHoTen.StyleController = this.layoutControlHC;
            this.lblHoTen.TabIndex = 0;
            this.lblHoTen.Text = " ";
            // 
            // lblNgaySinh
            // 
            this.lblNgaySinh.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNgaySinh.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNgaySinh.Location = new System.Drawing.Point(388, 2);
            this.lblNgaySinh.Name = "lblNgaySinh";
            this.lblNgaySinh.Size = new System.Drawing.Size(140, 20);
            this.lblNgaySinh.StyleController = this.layoutControlHC;
            this.lblNgaySinh.TabIndex = 1;
            this.lblNgaySinh.Text = " ";
            // 
            // lblGioiTinh
            // 
            this.lblGioiTinh.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGioiTinh.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGioiTinh.Location = new System.Drawing.Point(597, 2);
            this.lblGioiTinh.Name = "lblGioiTinh";
            this.lblGioiTinh.Size = new System.Drawing.Size(127, 20);
            this.lblGioiTinh.StyleController = this.layoutControlHC;
            this.lblGioiTinh.TabIndex = 2;
            this.lblGioiTinh.Text = " ";
            // 
            // lblCCCD
            // 
            this.lblCCCD.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCCCD.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCCCD.Location = new System.Drawing.Point(793, 2);
            this.lblCCCD.Name = "lblCCCD";
            this.lblCCCD.Size = new System.Drawing.Size(161, 20);
            this.lblCCCD.StyleController = this.layoutControlHC;
            this.lblCCCD.TabIndex = 3;
            this.lblCCCD.Text = " ";
            // 
            // lblDienThoai
            // 
            this.lblDienThoai.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDienThoai.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDienThoai.Location = new System.Drawing.Point(1023, 2);
            this.lblDienThoai.Name = "lblDienThoai";
            this.lblDienThoai.Size = new System.Drawing.Size(301, 20);
            this.lblDienThoai.StyleController = this.layoutControlHC;
            toolTipItem3.Text = "Điện thoại";
            superToolTip3.Items.Add(toolTipItem3);
            this.lblDienThoai.SuperTip = superToolTip3;
            this.lblDienThoai.TabIndex = 4;
            this.lblDienThoai.Text = " ";
            // 
            // lblDiaChi
            // 
            this.lblDiaChi.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDiaChi.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiaChi.Location = new System.Drawing.Point(97, 26);
            this.lblDiaChi.Name = "lblDiaChi";
            this.lblDiaChi.Size = new System.Drawing.Size(1227, 20);
            this.lblDiaChi.StyleController = this.layoutControlHC;
            this.lblDiaChi.TabIndex = 5;
            this.lblDiaChi.Text = " ";
            // 
            // rootHC
            // 
            this.rootHC.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootHC.GroupBordersVisible = false;
            this.rootHC.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciHoTen,
            this.lciNgaySinh,
            this.lciGioiTinh,
            this.lciCCCD,
            this.lciDienThoai,
            this.lciDiaChi});
            this.rootHC.Location = new System.Drawing.Point(0, 0);
            this.rootHC.Name = "rootHC";
            this.rootHC.Size = new System.Drawing.Size(1326, 51);
            this.rootHC.TextVisible = false;
            // 
            // lciHoTen
            // 
            this.lciHoTen.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHoTen.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHoTen.Control = this.lblHoTen;
            this.lciHoTen.Location = new System.Drawing.Point(0, 0);
            this.lciHoTen.Name = "lciHoTen";
            this.lciHoTen.Size = new System.Drawing.Size(321, 24);
            this.lciHoTen.Text = "Họ và tên:";
            this.lciHoTen.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHoTen.TextSize = new System.Drawing.Size(90, 20);
            this.lciHoTen.TextToControlDistance = 5;
            // 
            // lciNgaySinh
            // 
            this.lciNgaySinh.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNgaySinh.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNgaySinh.Control = this.lblNgaySinh;
            this.lciNgaySinh.Location = new System.Drawing.Point(321, 0);
            this.lciNgaySinh.Name = "lciNgaySinh";
            this.lciNgaySinh.Size = new System.Drawing.Size(209, 24);
            this.lciNgaySinh.Text = "Ngày sinh:";
            this.lciNgaySinh.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNgaySinh.TextSize = new System.Drawing.Size(60, 20);
            this.lciNgaySinh.TextToControlDistance = 5;
            // 
            // lciGioiTinh
            // 
            this.lciGioiTinh.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGioiTinh.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGioiTinh.Control = this.lblGioiTinh;
            this.lciGioiTinh.Location = new System.Drawing.Point(530, 0);
            this.lciGioiTinh.Name = "lciGioiTinh";
            this.lciGioiTinh.Size = new System.Drawing.Size(196, 24);
            this.lciGioiTinh.Text = "Giới tính:";
            this.lciGioiTinh.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGioiTinh.TextSize = new System.Drawing.Size(60, 20);
            this.lciGioiTinh.TextToControlDistance = 5;
            // 
            // lciCCCD
            // 
            this.lciCCCD.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCCCD.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCCCD.Control = this.lblCCCD;
            this.lciCCCD.Location = new System.Drawing.Point(726, 0);
            this.lciCCCD.Name = "lciCCCD";
            this.lciCCCD.Size = new System.Drawing.Size(230, 24);
            this.lciCCCD.Text = "CCCD:";
            this.lciCCCD.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCCCD.TextSize = new System.Drawing.Size(60, 20);
            this.lciCCCD.TextToControlDistance = 5;
            // 
            // lciDienThoai
            // 
            this.lciDienThoai.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDienThoai.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDienThoai.Control = this.lblDienThoai;
            this.lciDienThoai.Location = new System.Drawing.Point(956, 0);
            this.lciDienThoai.Name = "lciDienThoai";
            this.lciDienThoai.OptionsToolTip.ToolTip = "Điện thoại";
            this.lciDienThoai.Size = new System.Drawing.Size(370, 24);
            this.lciDienThoai.Text = "ĐT:";
            this.lciDienThoai.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDienThoai.TextSize = new System.Drawing.Size(60, 20);
            this.lciDienThoai.TextToControlDistance = 5;
            // 
            // lciDiaChi
            // 
            this.lciDiaChi.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDiaChi.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDiaChi.Control = this.lblDiaChi;
            this.lciDiaChi.Location = new System.Drawing.Point(0, 24);
            this.lciDiaChi.Name = "lciDiaChi";
            this.lciDiaChi.Size = new System.Drawing.Size(1326, 27);
            this.lciDiaChi.Text = "Địa chỉ:";
            this.lciDiaChi.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDiaChi.TextSize = new System.Drawing.Size(90, 20);
            this.lciDiaChi.TextToControlDistance = 5;
            // 
            // grpThongTinGoi
            // 
            this.grpThongTinGoi.Controls.Add(this.layoutControlInfo);
            this.grpThongTinGoi.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpThongTinGoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpThongTinGoi.Location = new System.Drawing.Point(2, 154);
            this.grpThongTinGoi.Margin = new System.Windows.Forms.Padding(0);
            this.grpThongTinGoi.Name = "grpThongTinGoi";
            this.grpThongTinGoi.Padding = new System.Windows.Forms.Padding(5);
            this.grpThongTinGoi.Size = new System.Drawing.Size(1336, 105);
            this.grpThongTinGoi.TabIndex = 2;
            this.grpThongTinGoi.TabStop = false;
            this.grpThongTinGoi.Text = "Thông tin gói";
            // 
            // layoutControlInfo
            // 
            this.layoutControlInfo.Controls.Add(this.cboMauGoi);
            this.layoutControlInfo.Controls.Add(this.txtTenGoi);
            this.layoutControlInfo.Controls.Add(this.dteNgayDangKy);
            this.layoutControlInfo.Controls.Add(this.cboDoiTuongTT);
            this.layoutControlInfo.Controls.Add(this.cboTrangThai);
            this.layoutControlInfo.Controls.Add(this.memGhiChu);
            this.layoutControlInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlInfo.Location = new System.Drawing.Point(5, 18);
            this.layoutControlInfo.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlInfo.Name = "layoutControlInfo";
            this.layoutControlInfo.Root = this.rootInfo;
            this.layoutControlInfo.Size = new System.Drawing.Size(1326, 82);
            this.layoutControlInfo.TabIndex = 0;
            // 
            // cboMauGoi
            // 
            this.cboMauGoi.Location = new System.Drawing.Point(97, 2);
            this.cboMauGoi.MenuManager = this.barManager1;
            this.cboMauGoi.Name = "cboMauGoi";
            this.cboMauGoi.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboMauGoi.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboMauGoi.Properties.NullText = "";
            this.cboMauGoi.Properties.View = this.gridViewMauGoi;
            this.cboMauGoi.Size = new System.Drawing.Size(329, 20);
            this.cboMauGoi.StyleController = this.layoutControlInfo;
            this.cboMauGoi.TabIndex = 0;
            this.cboMauGoi.EditValueChanged += new System.EventHandler(this.cboMauGoi_EditValueChanged);
            // 
            // gridViewMauGoi
            // 
            this.gridViewMauGoi.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewMauGoi.Name = "gridViewMauGoi";
            this.gridViewMauGoi.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewMauGoi.OptionsView.ShowGroupPanel = false;
            // 
            // txtTenGoi
            // 
            this.txtTenGoi.Location = new System.Drawing.Point(97, 26);
            this.txtTenGoi.MenuManager = this.barManager1;
            this.txtTenGoi.Name = "txtTenGoi";
            this.txtTenGoi.Size = new System.Drawing.Size(329, 20);
            this.txtTenGoi.StyleController = this.layoutControlInfo;
            this.txtTenGoi.TabIndex = 1;
            // 
            // dteNgayDangKy
            // 
            this.dteNgayDangKy.EditValue = null;
            this.dteNgayDangKy.Location = new System.Drawing.Point(515, 26);
            this.dteNgayDangKy.MenuManager = this.barManager1;
            this.dteNgayDangKy.Name = "dteNgayDangKy";
            this.dteNgayDangKy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteNgayDangKy.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dteNgayDangKy.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteNgayDangKy.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dteNgayDangKy.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteNgayDangKy.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteNgayDangKy.Size = new System.Drawing.Size(187, 20);
            this.dteNgayDangKy.StyleController = this.layoutControlInfo;
            this.dteNgayDangKy.TabIndex = 2;
            // 
            // cboDoiTuongTT
            // 
            this.cboDoiTuongTT.Location = new System.Drawing.Point(791, 26);
            this.cboDoiTuongTT.MenuManager = this.barManager1;
            this.cboDoiTuongTT.Name = "cboDoiTuongTT";
            this.cboDoiTuongTT.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDoiTuongTT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDoiTuongTT.Properties.NullText = "";
            this.cboDoiTuongTT.Properties.View = this.gridViewDoiTuongTT;
            this.cboDoiTuongTT.Size = new System.Drawing.Size(178, 20);
            this.cboDoiTuongTT.StyleController = this.layoutControlInfo;
            toolTipItem4.Text = "Đối tượng thanh toán";
            superToolTip4.Items.Add(toolTipItem4);
            this.cboDoiTuongTT.SuperTip = superToolTip4;
            this.cboDoiTuongTT.TabIndex = 3;
            this.cboDoiTuongTT.EditValueChanged += new System.EventHandler(this.cboDoiTuongTT_EditValueChanged);
            // 
            // gridViewDoiTuongTT
            // 
            this.gridViewDoiTuongTT.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDoiTuongTT.Name = "gridViewDoiTuongTT";
            this.gridViewDoiTuongTT.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDoiTuongTT.OptionsView.ShowGroupPanel = false;
            // 
            // cboTrangThai
            // 
            this.cboTrangThai.Location = new System.Drawing.Point(97, 50);
            this.cboTrangThai.MenuManager = this.barManager1;
            this.cboTrangThai.Name = "cboTrangThai";
            this.cboTrangThai.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTrangThai.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTrangThai.Properties.NullText = "";
            this.cboTrangThai.Properties.View = this.gridViewTrangThai;
            this.cboTrangThai.Size = new System.Drawing.Size(329, 20);
            this.cboTrangThai.StyleController = this.layoutControlInfo;
            this.cboTrangThai.TabIndex = 4;
            this.cboTrangThai.EditValueChanged += new System.EventHandler(this.cboTrangThai_EditValueChanged);
            // 
            // gridViewTrangThai
            // 
            this.gridViewTrangThai.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewTrangThai.Name = "gridViewTrangThai";
            this.gridViewTrangThai.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewTrangThai.OptionsView.ShowGroupPanel = false;
            // 
            // memGhiChu
            // 
            this.memGhiChu.Location = new System.Drawing.Point(515, 50);
            this.memGhiChu.MenuManager = this.barManager1;
            this.memGhiChu.Name = "memGhiChu";
            this.memGhiChu.Size = new System.Drawing.Size(809, 30);
            this.memGhiChu.StyleController = this.layoutControlInfo;
            this.memGhiChu.TabIndex = 5;
            // 
            // rootInfo
            // 
            this.rootInfo.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootInfo.GroupBordersVisible = false;
            this.rootInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMauGoi,
            this.lciTenGoi,
            this.lciNgayDangKy,
            this.lciDoiTuongTT,
            this.lciTrangThai,
            this.lciGhiChu,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.rootInfo.Location = new System.Drawing.Point(0, 0);
            this.rootInfo.Name = "rootInfo";
            this.rootInfo.Size = new System.Drawing.Size(1326, 82);
            this.rootInfo.TextVisible = false;
            // 
            // lciMauGoi
            // 
            this.lciMauGoi.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMauGoi.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMauGoi.Control = this.cboMauGoi;
            this.lciMauGoi.Location = new System.Drawing.Point(0, 0);
            this.lciMauGoi.Name = "lciMauGoi";
            this.lciMauGoi.Size = new System.Drawing.Size(428, 24);
            this.lciMauGoi.Text = "Mẫu gói:";
            this.lciMauGoi.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMauGoi.TextSize = new System.Drawing.Size(90, 20);
            this.lciMauGoi.TextToControlDistance = 5;
            // 
            // lciTenGoi
            // 
            this.lciTenGoi.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTenGoi.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTenGoi.Control = this.txtTenGoi;
            this.lciTenGoi.Location = new System.Drawing.Point(0, 24);
            this.lciTenGoi.Name = "lciTenGoi";
            this.lciTenGoi.Size = new System.Drawing.Size(428, 24);
            this.lciTenGoi.Text = "Tên gói:";
            this.lciTenGoi.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTenGoi.TextSize = new System.Drawing.Size(90, 20);
            this.lciTenGoi.TextToControlDistance = 5;
            // 
            // lciNgayDangKy
            // 
            this.lciNgayDangKy.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNgayDangKy.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNgayDangKy.Control = this.dteNgayDangKy;
            this.lciNgayDangKy.Location = new System.Drawing.Point(428, 24);
            this.lciNgayDangKy.Name = "lciNgayDangKy";
            this.lciNgayDangKy.Size = new System.Drawing.Size(276, 24);
            this.lciNgayDangKy.Text = "Ngày đăng ký:";
            this.lciNgayDangKy.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNgayDangKy.TextSize = new System.Drawing.Size(80, 20);
            this.lciNgayDangKy.TextToControlDistance = 5;
            // 
            // lciDoiTuongTT
            // 
            this.lciDoiTuongTT.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDoiTuongTT.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDoiTuongTT.Control = this.cboDoiTuongTT;
            this.lciDoiTuongTT.Location = new System.Drawing.Point(704, 24);
            this.lciDoiTuongTT.Name = "lciDoiTuongTT";
            this.lciDoiTuongTT.OptionsToolTip.ToolTip = "Đối tượng thanh toán";
            this.lciDoiTuongTT.Size = new System.Drawing.Size(267, 24);
            this.lciDoiTuongTT.Text = "Đối tượng TT:";
            this.lciDoiTuongTT.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDoiTuongTT.TextSize = new System.Drawing.Size(80, 20);
            this.lciDoiTuongTT.TextToControlDistance = 5;
            // 
            // lciTrangThai
            // 
            this.lciTrangThai.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTrangThai.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTrangThai.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lciTrangThai.Control = this.cboTrangThai;
            this.lciTrangThai.Location = new System.Drawing.Point(0, 48);
            this.lciTrangThai.Name = "lciTrangThai";
            this.lciTrangThai.Size = new System.Drawing.Size(428, 34);
            this.lciTrangThai.Text = "Trạng thái:";
            this.lciTrangThai.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTrangThai.TextSize = new System.Drawing.Size(90, 20);
            this.lciTrangThai.TextToControlDistance = 5;
            // 
            // lciGhiChu
            // 
            this.lciGhiChu.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGhiChu.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGhiChu.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lciGhiChu.Control = this.memGhiChu;
            this.lciGhiChu.Location = new System.Drawing.Point(428, 48);
            this.lciGhiChu.Name = "lciGhiChu";
            this.lciGhiChu.Size = new System.Drawing.Size(898, 34);
            this.lciGhiChu.Text = "Ghi chú:";
            this.lciGhiChu.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGhiChu.TextSize = new System.Drawing.Size(80, 20);
            this.lciGhiChu.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(428, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(898, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(971, 24);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(355, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // splitContainerDV
            // 
            this.splitContainerDV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDV.Location = new System.Drawing.Point(2, 259);
            this.splitContainerDV.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerDV.Name = "splitContainerDV";
            // 
            // splitContainerDV.Panel1
            // 
            this.splitContainerDV.Panel1.Controls.Add(this.grpDanhMucDV);
            // 
            // splitContainerDV.Panel2
            // 
            this.splitContainerDV.Panel2.Controls.Add(this.grpDichVuTrongGoi);
            this.splitContainerDV.Size = new System.Drawing.Size(1336, 291);
            this.splitContainerDV.SplitterDistance = 675;
            this.splitContainerDV.TabIndex = 3;
            // 
            // grpDanhMucDV
            // 
            this.grpDanhMucDV.Controls.Add(this.layoutControlDanhMuc);
            this.grpDanhMucDV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDanhMucDV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDanhMucDV.Location = new System.Drawing.Point(0, 0);
            this.grpDanhMucDV.Margin = new System.Windows.Forms.Padding(0);
            this.grpDanhMucDV.Name = "grpDanhMucDV";
            this.grpDanhMucDV.Padding = new System.Windows.Forms.Padding(5);
            this.grpDanhMucDV.Size = new System.Drawing.Size(675, 291);
            this.grpDanhMucDV.TabIndex = 0;
            this.grpDanhMucDV.TabStop = false;
            this.grpDanhMucDV.Text = "Danh mục dịch vụ";
            // 
            // layoutControlDanhMuc
            // 
            this.layoutControlDanhMuc.Controls.Add(this.txtTimKiemDV);
            this.layoutControlDanhMuc.Controls.Add(this.cboLoaiDV);
            this.layoutControlDanhMuc.Controls.Add(this.grdDanhMucDV);
            this.layoutControlDanhMuc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlDanhMuc.Location = new System.Drawing.Point(5, 18);
            this.layoutControlDanhMuc.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlDanhMuc.Name = "layoutControlDanhMuc";
            this.layoutControlDanhMuc.Root = this.rootDanhMuc;
            this.layoutControlDanhMuc.Size = new System.Drawing.Size(665, 268);
            this.layoutControlDanhMuc.TabIndex = 0;
            // 
            // txtTimKiemDV
            // 
            this.txtTimKiemDV.Location = new System.Drawing.Point(2, 2);
            this.txtTimKiemDV.MenuManager = this.barManager1;
            this.txtTimKiemDV.Name = "txtTimKiemDV";
            this.txtTimKiemDV.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtTimKiemDV.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTimKiemDV.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTimKiemDV.Size = new System.Drawing.Size(401, 20);
            this.txtTimKiemDV.StyleController = this.layoutControlDanhMuc;
            this.txtTimKiemDV.TabIndex = 0;
            this.txtTimKiemDV.EditValueChanged += new System.EventHandler(this.txtTimKiemDV_EditValueChanged);
            // 
            // cboLoaiDV
            // 
            this.cboLoaiDV.Location = new System.Drawing.Point(407, 2);
            this.cboLoaiDV.MenuManager = this.barManager1;
            this.cboLoaiDV.Name = "cboLoaiDV";
            this.cboLoaiDV.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboLoaiDV.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboLoaiDV.Properties.NullText = "";
            this.cboLoaiDV.Properties.View = this.gridViewLoaiDV;
            this.cboLoaiDV.Size = new System.Drawing.Size(256, 20);
            this.cboLoaiDV.StyleController = this.layoutControlDanhMuc;
            this.cboLoaiDV.TabIndex = 1;
            this.cboLoaiDV.EditValueChanged += new System.EventHandler(this.cboLoaiDV_EditValueChanged);
            // 
            // gridViewLoaiDV
            // 
            this.gridViewLoaiDV.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewLoaiDV.Name = "gridViewLoaiDV";
            this.gridViewLoaiDV.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewLoaiDV.OptionsView.ShowGroupPanel = false;
            // 
            // grdDanhMucDV
            // 
            this.grdDanhMucDV.Location = new System.Drawing.Point(2, 26);
            this.grdDanhMucDV.MainView = this.gvDanhMucDV;
            this.grdDanhMucDV.MenuManager = this.barManager1;
            this.grdDanhMucDV.Name = "grdDanhMucDV";
            this.grdDanhMucDV.Size = new System.Drawing.Size(661, 240);
            this.grdDanhMucDV.TabIndex = 2;
            this.grdDanhMucDV.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDanhMucDV});
            // 
            // gvDanhMucDV
            // 
            this.gvDanhMucDV.GridControl = this.grdDanhMucDV;
            this.gvDanhMucDV.Name = "gvDanhMucDV";
            this.gvDanhMucDV.OptionsView.ShowGroupPanel = false;
            this.gvDanhMucDV.OptionsView.ShowIndicator = false;
            // 
            // rootDanhMuc
            // 
            this.rootDanhMuc.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootDanhMuc.GroupBordersVisible = false;
            this.rootDanhMuc.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTimKiemDV,
            this.lciLoaiDV,
            this.lciGrdDanhMucDV});
            this.rootDanhMuc.Location = new System.Drawing.Point(0, 0);
            this.rootDanhMuc.Name = "rootDanhMuc";
            this.rootDanhMuc.Size = new System.Drawing.Size(665, 268);
            this.rootDanhMuc.TextVisible = false;
            // 
            // lciTimKiemDV
            // 
            this.lciTimKiemDV.Control = this.txtTimKiemDV;
            this.lciTimKiemDV.Location = new System.Drawing.Point(0, 0);
            this.lciTimKiemDV.Name = "lciTimKiemDV";
            this.lciTimKiemDV.Size = new System.Drawing.Size(405, 24);
            this.lciTimKiemDV.TextSize = new System.Drawing.Size(0, 0);
            this.lciTimKiemDV.TextVisible = false;
            // 
            // lciLoaiDV
            // 
            this.lciLoaiDV.Control = this.cboLoaiDV;
            this.lciLoaiDV.Location = new System.Drawing.Point(405, 0);
            this.lciLoaiDV.Name = "lciLoaiDV";
            this.lciLoaiDV.Size = new System.Drawing.Size(260, 24);
            this.lciLoaiDV.TextSize = new System.Drawing.Size(0, 0);
            this.lciLoaiDV.TextVisible = false;
            // 
            // lciGrdDanhMucDV
            // 
            this.lciGrdDanhMucDV.Control = this.grdDanhMucDV;
            this.lciGrdDanhMucDV.Location = new System.Drawing.Point(0, 24);
            this.lciGrdDanhMucDV.Name = "lciGrdDanhMucDV";
            this.lciGrdDanhMucDV.Size = new System.Drawing.Size(665, 244);
            this.lciGrdDanhMucDV.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrdDanhMucDV.TextVisible = false;
            // 
            // grpDichVuTrongGoi
            // 
            this.grpDichVuTrongGoi.Controls.Add(this.layoutControlDVTrongGoi);
            this.grpDichVuTrongGoi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDichVuTrongGoi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDichVuTrongGoi.Location = new System.Drawing.Point(0, 0);
            this.grpDichVuTrongGoi.Margin = new System.Windows.Forms.Padding(0);
            this.grpDichVuTrongGoi.Name = "grpDichVuTrongGoi";
            this.grpDichVuTrongGoi.Padding = new System.Windows.Forms.Padding(5);
            this.grpDichVuTrongGoi.Size = new System.Drawing.Size(657, 291);
            this.grpDichVuTrongGoi.TabIndex = 0;
            this.grpDichVuTrongGoi.TabStop = false;
            this.grpDichVuTrongGoi.Text = "Dịch vụ trong gói";
            // 
            // layoutControlDVTrongGoi
            // 
            this.layoutControlDVTrongGoi.Controls.Add(this.btnPhiGoi);
            this.layoutControlDVTrongGoi.Controls.Add(this.grdDichVuTrongGoi);
            this.layoutControlDVTrongGoi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlDVTrongGoi.Location = new System.Drawing.Point(5, 18);
            this.layoutControlDVTrongGoi.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlDVTrongGoi.Name = "layoutControlDVTrongGoi";
            this.layoutControlDVTrongGoi.Root = this.rootDVTrongGoi;
            this.layoutControlDVTrongGoi.Size = new System.Drawing.Size(647, 268);
            this.layoutControlDVTrongGoi.TabIndex = 0;
            // 
            // btnPhiGoi
            // 
            this.btnPhiGoi.Location = new System.Drawing.Point(2, 2);
            this.btnPhiGoi.Name = "btnPhiGoi";
            this.btnPhiGoi.Size = new System.Drawing.Size(110, 22);
            this.btnPhiGoi.StyleController = this.layoutControlDVTrongGoi;
            this.btnPhiGoi.TabIndex = 0;
            this.btnPhiGoi.Text = "Phí gói";
            this.btnPhiGoi.Click += new System.EventHandler(this.btnPhiGoi_Click);
            // 
            // grdDichVuTrongGoi
            // 
            this.grdDichVuTrongGoi.Location = new System.Drawing.Point(2, 28);
            this.grdDichVuTrongGoi.MainView = this.gvDichVuTrongGoi;
            this.grdDichVuTrongGoi.MenuManager = this.barManager1;
            this.grdDichVuTrongGoi.Name = "grdDichVuTrongGoi";
            this.grdDichVuTrongGoi.Size = new System.Drawing.Size(643, 238);
            this.grdDichVuTrongGoi.TabIndex = 1;
            this.grdDichVuTrongGoi.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDichVuTrongGoi});
            // 
            // gvDichVuTrongGoi
            // 
            this.gvDichVuTrongGoi.GridControl = this.grdDichVuTrongGoi;
            this.gvDichVuTrongGoi.Name = "gvDichVuTrongGoi";
            this.gvDichVuTrongGoi.OptionsView.ShowGroupPanel = false;
            this.gvDichVuTrongGoi.OptionsView.ShowIndicator = false;
            // 
            // lblTongCong
            // 
            this.lblTongCong.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblTongCong.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTongCong.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTongCong.Location = new System.Drawing.Point(2, 2);
            this.lblTongCong.Name = "lblTongCong";
            this.lblTongCong.Size = new System.Drawing.Size(1332, 14);
            this.lblTongCong.StyleController = this.layoutControlButtons;
            this.lblTongCong.TabIndex = 2;
            this.lblTongCong.Text = "Tổng cộng: 0";
            // 
            // rootDVTrongGoi
            // 
            this.rootDVTrongGoi.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootDVTrongGoi.GroupBordersVisible = false;
            this.rootDVTrongGoi.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnPhiGoi,
            this.lciGrdDichVuTrongGoi,
            this.emptySpaceItem3});
            this.rootDVTrongGoi.Location = new System.Drawing.Point(0, 0);
            this.rootDVTrongGoi.Name = "rootDVTrongGoi";
            this.rootDVTrongGoi.Size = new System.Drawing.Size(647, 268);
            this.rootDVTrongGoi.TextVisible = false;
            // 
            // lciBtnPhiGoi
            // 
            this.lciBtnPhiGoi.Control = this.btnPhiGoi;
            this.lciBtnPhiGoi.Location = new System.Drawing.Point(0, 0);
            this.lciBtnPhiGoi.Name = "lciBtnPhiGoi";
            this.lciBtnPhiGoi.Size = new System.Drawing.Size(114, 26);
            this.lciBtnPhiGoi.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnPhiGoi.TextVisible = false;
            // 
            // lciGrdDichVuTrongGoi
            // 
            this.lciGrdDichVuTrongGoi.Control = this.grdDichVuTrongGoi;
            this.lciGrdDichVuTrongGoi.Location = new System.Drawing.Point(0, 26);
            this.lciGrdDichVuTrongGoi.Name = "lciGrdDichVuTrongGoi";
            this.lciGrdDichVuTrongGoi.Size = new System.Drawing.Size(647, 242);
            this.lciGrdDichVuTrongGoi.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrdDichVuTrongGoi.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(114, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(533, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.layoutControlButtons);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(2, 550);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(1336, 45);
            this.pnlButtons.TabIndex = 4;
            // 
            // layoutControlButtons
            // 
            this.layoutControlButtons.Controls.Add(this.btnInPhieu);
            this.layoutControlButtons.Controls.Add(this.btnHuyBo);
            this.layoutControlButtons.Controls.Add(this.lblTongCong);
            this.layoutControlButtons.Controls.Add(this.btnLuu);
            this.layoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlButtons.Location = new System.Drawing.Point(0, 0);
            this.layoutControlButtons.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControlButtons.Name = "layoutControlButtons";
            this.layoutControlButtons.Root = this.rootButtons;
            this.layoutControlButtons.Size = new System.Drawing.Size(1336, 45);
            this.layoutControlButtons.TabIndex = 0;
            // 
            // btnInPhieu
            // 
            this.btnInPhieu.Location = new System.Drawing.Point(922, 20);
            this.btnInPhieu.Name = "btnInPhieu";
            this.btnInPhieu.Size = new System.Drawing.Size(136, 22);
            this.btnInPhieu.StyleController = this.layoutControlButtons;
            this.btnInPhieu.TabIndex = 0;
            this.btnInPhieu.Text = "In phiếu (Ctrl P)";
            this.btnInPhieu.Click += new System.EventHandler(this.btnInPhieu_Click);
            // 
            // btnHuyBo
            // 
            this.btnHuyBo.Location = new System.Drawing.Point(1062, 20);
            this.btnHuyBo.Name = "btnHuyBo";
            this.btnHuyBo.Size = new System.Drawing.Size(127, 22);
            this.btnHuyBo.StyleController = this.layoutControlButtons;
            this.btnHuyBo.TabIndex = 1;
            this.btnHuyBo.Text = "Làm mới";
            this.btnHuyBo.Click += new System.EventHandler(this.btnHuyBo_Click);
            // 
            // btnLuu
            // 
            this.btnLuu.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnLuu.Appearance.Options.UseFont = true;
            this.btnLuu.Location = new System.Drawing.Point(1193, 20);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(141, 22);
            this.btnLuu.StyleController = this.layoutControlButtons;
            this.btnLuu.TabIndex = 2;
            this.btnLuu.Text = "Lưu (Ctrl S)";
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // rootButtons
            // 
            this.rootButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.rootButtons.GroupBordersVisible = false;
            this.rootButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnInPhieu,
            this.lciBtnHuyBo,
            this.lciBtnLuu,
            this.emptySpaceItem4,
            this.layoutControlItem1});
            this.rootButtons.Location = new System.Drawing.Point(0, 0);
            this.rootButtons.Name = "rootButtons";
            this.rootButtons.Size = new System.Drawing.Size(1336, 45);
            this.rootButtons.TextVisible = false;
            // 
            // lciBtnInPhieu
            // 
            this.lciBtnInPhieu.Control = this.btnInPhieu;
            this.lciBtnInPhieu.Location = new System.Drawing.Point(920, 18);
            this.lciBtnInPhieu.MaxSize = new System.Drawing.Size(0, 26);
            this.lciBtnInPhieu.MinSize = new System.Drawing.Size(89, 1);
            this.lciBtnInPhieu.Name = "lciBtnInPhieu";
            this.lciBtnInPhieu.Size = new System.Drawing.Size(140, 27);
            this.lciBtnInPhieu.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnInPhieu.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnInPhieu.TextVisible = false;
            // 
            // lciBtnHuyBo
            // 
            this.lciBtnHuyBo.Control = this.btnHuyBo;
            this.lciBtnHuyBo.Location = new System.Drawing.Point(1060, 18);
            this.lciBtnHuyBo.Name = "lciBtnHuyBo";
            this.lciBtnHuyBo.Size = new System.Drawing.Size(131, 27);
            this.lciBtnHuyBo.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnHuyBo.TextVisible = false;
            // 
            // lciBtnLuu
            // 
            this.lciBtnLuu.Control = this.btnLuu;
            this.lciBtnLuu.Location = new System.Drawing.Point(1191, 18);
            this.lciBtnLuu.Name = "lciBtnLuu";
            this.lciBtnLuu.Size = new System.Drawing.Size(145, 27);
            this.lciBtnLuu.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnLuu.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblTongCong;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1336, 18);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(0, 18);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(920, 27);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmPatientPackageRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1340, 597);
            this.Controls.Add(this.splitContainerDV);
            this.Controls.Add(this.grpThongTinGoi);
            this.Controls.Add(this.grpHanhChinh);
            this.Controls.Add(this.grpTimKiemBN);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "frmPatientPackageRegister";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng ký gói dịch vụ";
            this.Load += new System.EventHandler(this.frmPatientPackageRegister_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.pnlButtons, 0);
            this.Controls.SetChildIndex(this.grpTimKiemBN, 0);
            this.Controls.SetChildIndex(this.grpHanhChinh, 0);
            this.Controls.SetChildIndex(this.grpThongTinGoi, 0);
            this.Controls.SetChildIndex(this.splitContainerDV, 0);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.grpTimKiemBN.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlSearch)).EndInit();
            this.layoutControlSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaBenhNhan.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaBenhNhan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnTimKiem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDanhSachGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTimKiem)).EndInit();
            this.grpHanhChinh.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlHC)).EndInit();
            this.layoutControlHC.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rootHC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHoTen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgaySinh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGioiTinh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCCCD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDienThoai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDiaChi)).EndInit();
            this.grpThongTinGoi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlInfo)).EndInit();
            this.layoutControlInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboMauGoi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMauGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenGoi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteNgayDangKy.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteNgayDangKy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDoiTuongTT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDoiTuongTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTrangThai.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTrangThai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memGhiChu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMauGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTenGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgayDangKy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDoiTuongTT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTrangThai)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGhiChu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.splitContainerDV.Panel1.ResumeLayout(false);
            this.splitContainerDV.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerDV)).EndInit();
            this.splitContainerDV.ResumeLayout(false);
            this.grpDanhMucDV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDanhMuc)).EndInit();
            this.layoutControlDanhMuc.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTimKiemDV.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLoaiDV.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLoaiDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDanhMucDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDanhMucDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootDanhMuc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimKiemDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLoaiDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrdDanhMucDV)).EndInit();
            this.grpDichVuTrongGoi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlDVTrongGoi)).EndInit();
            this.layoutControlDVTrongGoi.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDichVuTrongGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDichVuTrongGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rootDVTrongGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnPhiGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrdDichVuTrongGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).EndInit();
            this.layoutControlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rootButtons)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnInPhieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnHuyBo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnLuu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemLuu;
        private DevExpress.XtraBars.BarButtonItem barButtonItemInPhieu;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;

        private System.Windows.Forms.GroupBox grpTimKiemBN;
        private System.Windows.Forms.GroupBox grpHanhChinh;
        private System.Windows.Forms.GroupBox grpThongTinGoi;
        private System.Windows.Forms.SplitContainer splitContainerDV;
        private System.Windows.Forms.GroupBox grpDanhMucDV;
        private System.Windows.Forms.GroupBox grpDichVuTrongGoi;
        private System.Windows.Forms.Panel pnlButtons;

        private DevExpress.XtraLayout.LayoutControl layoutControlSearch;
        private DevExpress.XtraLayout.LayoutControl layoutControlHC;
        private DevExpress.XtraLayout.LayoutControl layoutControlInfo;
        private DevExpress.XtraLayout.LayoutControl layoutControlDanhMuc;
        private DevExpress.XtraLayout.LayoutControl layoutControlDVTrongGoi;
        private DevExpress.XtraLayout.LayoutControl layoutControlButtons;

        private DevExpress.XtraEditors.TextEdit txtMaBenhNhan;
        private DevExpress.XtraEditors.SimpleButton btnTimKiem;
        private DevExpress.XtraEditors.DropDownButton btnDanhSachGoi;
        private DevExpress.XtraLayout.LayoutControlGroup rootSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciMaBenhNhan;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnTimKiem;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDanhSachGoi;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemTimKiem;

        private DevExpress.XtraEditors.LabelControl lblHoTen;
        private DevExpress.XtraEditors.LabelControl lblNgaySinh;
        private DevExpress.XtraEditors.LabelControl lblGioiTinh;
        private DevExpress.XtraEditors.LabelControl lblCCCD;
        private DevExpress.XtraEditors.LabelControl lblDienThoai;
        private DevExpress.XtraEditors.LabelControl lblDiaChi;
        private DevExpress.XtraLayout.LayoutControlGroup rootHC;
        private DevExpress.XtraLayout.LayoutControlItem lciHoTen;
        private DevExpress.XtraLayout.LayoutControlItem lciNgaySinh;
        private DevExpress.XtraLayout.LayoutControlItem lciGioiTinh;
        private DevExpress.XtraLayout.LayoutControlItem lciCCCD;
        private DevExpress.XtraLayout.LayoutControlItem lciDienThoai;
        private DevExpress.XtraLayout.LayoutControlItem lciDiaChi;

        private DevExpress.XtraEditors.GridLookUpEdit cboMauGoi;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMauGoi;
        private DevExpress.XtraEditors.TextEdit txtTenGoi;
        private DevExpress.XtraEditors.DateEdit dteNgayDangKy;
        private DevExpress.XtraEditors.GridLookUpEdit cboDoiTuongTT;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDoiTuongTT;
        private DevExpress.XtraEditors.GridLookUpEdit cboTrangThai;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTrangThai;
        private DevExpress.XtraEditors.MemoEdit memGhiChu;
        private DevExpress.XtraLayout.LayoutControlGroup rootInfo;
        private DevExpress.XtraLayout.LayoutControlItem lciMauGoi;
        private DevExpress.XtraLayout.LayoutControlItem lciTenGoi;
        private DevExpress.XtraLayout.LayoutControlItem lciNgayDangKy;
        private DevExpress.XtraLayout.LayoutControlItem lciDoiTuongTT;
        private DevExpress.XtraLayout.LayoutControlItem lciTrangThai;
        private DevExpress.XtraLayout.LayoutControlItem lciGhiChu;

        private DevExpress.XtraEditors.TextEdit txtTimKiemDV;
        private DevExpress.XtraEditors.GridLookUpEdit cboLoaiDV;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLoaiDV;
        private DevExpress.XtraGrid.GridControl grdDanhMucDV;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDanhMucDV;
        private DevExpress.XtraLayout.LayoutControlGroup rootDanhMuc;
        private DevExpress.XtraLayout.LayoutControlItem lciTimKiemDV;
        private DevExpress.XtraLayout.LayoutControlItem lciLoaiDV;
        private DevExpress.XtraLayout.LayoutControlItem lciGrdDanhMucDV;

        private DevExpress.XtraEditors.SimpleButton btnPhiGoi;
        private DevExpress.XtraGrid.GridControl grdDichVuTrongGoi;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDichVuTrongGoi;
        private DevExpress.XtraEditors.LabelControl lblTongCong;
        private DevExpress.XtraLayout.LayoutControlGroup rootDVTrongGoi;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnPhiGoi;
        private DevExpress.XtraLayout.LayoutControlItem lciGrdDichVuTrongGoi;

        private DevExpress.XtraEditors.SimpleButton btnInPhieu;
        private DevExpress.XtraEditors.SimpleButton btnHuyBo;
        private DevExpress.XtraEditors.SimpleButton btnLuu;
        private DevExpress.XtraLayout.LayoutControlGroup rootButtons;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnInPhieu;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnHuyBo;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnLuu;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
    }
}
