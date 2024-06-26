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
namespace LIS.Desktop.Plugins.SampleInfo
{
    partial class frmSampleInfo
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
            this.txtTube = new DevExpress.XtraEditors.TextEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSavePrint = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.chkCloseAfterPrint = new DevExpress.XtraEditors.CheckEdit();
            this.txtAppointmentPlace = new DevExpress.XtraEditors.ButtonEdit();
            this.dtAppointmentTime = new DevExpress.XtraEditors.DateEdit();
            this.btnPrintAssign = new DevExpress.XtraEditors.SimpleButton();
            this.txtSampleSender = new DevExpress.XtraEditors.TextEdit();
            this.cboMediOrgCode = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtMediOrgCode = new DevExpress.XtraEditors.TextEdit();
            this.chkPreviousStatus = new DevExpress.XtraEditors.CheckEdit();
            this.cboPatientStatus = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveAndPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtBarcode = new DevExpress.XtraEditors.TextEdit();
            this.dtSampleTime = new DevExpress.XtraEditors.DateEdit();
            this.cboSampleUser = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSampleLoginname = new DevExpress.XtraEditors.TextEdit();
            this.cboSampleType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSampleTypeCode = new DevExpress.XtraEditors.TextEdit();
            this.lblPatientDob = new DevExpress.XtraEditors.LabelControl();
            this.lblGenderName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.lblServiceReqCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciServiceReqCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGenderName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleUser = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleSender = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.timer1 = new System.Windows.Forms.Timer();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTube.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseAfterPrint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAppointmentPlace.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleSender.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediOrgCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediOrgCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPreviousStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSampleTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSampleTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleLoginname.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleTypeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGenderName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleSender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtTube);
            this.layoutControl1.Controls.Add(this.chkCloseAfterPrint);
            this.layoutControl1.Controls.Add(this.txtAppointmentPlace);
            this.layoutControl1.Controls.Add(this.dtAppointmentTime);
            this.layoutControl1.Controls.Add(this.btnPrintAssign);
            this.layoutControl1.Controls.Add(this.txtSampleSender);
            this.layoutControl1.Controls.Add(this.cboMediOrgCode);
            this.layoutControl1.Controls.Add(this.txtMediOrgCode);
            this.layoutControl1.Controls.Add(this.chkPreviousStatus);
            this.layoutControl1.Controls.Add(this.cboPatientStatus);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSaveAndPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtBarcode);
            this.layoutControl1.Controls.Add(this.dtSampleTime);
            this.layoutControl1.Controls.Add(this.cboSampleUser);
            this.layoutControl1.Controls.Add(this.txtSampleLoginname);
            this.layoutControl1.Controls.Add(this.cboSampleType);
            this.layoutControl1.Controls.Add(this.txtSampleTypeCode);
            this.layoutControl1.Controls.Add(this.lblPatientDob);
            this.layoutControl1.Controls.Add(this.lblGenderName);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Controls.Add(this.lblTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblServiceReqCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(476, 330);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtTube
            // 
            this.txtTube.Location = new System.Drawing.Point(102, 122);
            this.txtTube.MenuManager = this.barManager1;
            this.txtTube.Name = "txtTube";
            this.txtTube.Properties.ReadOnly = true;
            this.txtTube.Size = new System.Drawing.Size(355, 20);
            this.txtTube.StyleController = this.layoutControl1;
            this.txtTube.TabIndex = 26;
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
            this.barBtnSavePrint,
            this.barBtnPrint});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSavePrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnPrint)});
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
            // barBtnSavePrint
            // 
            this.barBtnSavePrint.Caption = "Lưu in (Ctrl I)";
            this.barBtnSavePrint.Id = 1;
            this.barBtnSavePrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.barBtnSavePrint.Name = "barBtnSavePrint";
            this.barBtnSavePrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSavePrint_ItemClick);
            // 
            // barBtnPrint
            // 
            this.barBtnPrint.Caption = "In barcode (Ctrl P)";
            this.barBtnPrint.Id = 2;
            this.barBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barBtnPrint.Name = "barBtnPrint";
            this.barBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(476, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 359);
            this.barDockControlBottom.Size = new System.Drawing.Size(476, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 330);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(476, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 330);
            // 
            // chkCloseAfterPrint
            // 
            this.chkCloseAfterPrint.Location = new System.Drawing.Point(359, 292);
            this.chkCloseAfterPrint.MenuManager = this.barManager1;
            this.chkCloseAfterPrint.Name = "chkCloseAfterPrint";
            this.chkCloseAfterPrint.Properties.Caption = "Đóng sau khi in";
            this.chkCloseAfterPrint.Size = new System.Drawing.Size(98, 19);
            this.chkCloseAfterPrint.StyleController = this.layoutControl1;
            this.chkCloseAfterPrint.TabIndex = 25;
            this.chkCloseAfterPrint.ToolTip = "Đóng chức năng sau khi thực hiện in hoặc đóng cửa sổ xem in";
            // 
            // txtAppointmentPlace
            // 
            this.txtAppointmentPlace.Location = new System.Drawing.Point(102, 268);
            this.txtAppointmentPlace.Margin = new System.Windows.Forms.Padding(2);
            this.txtAppointmentPlace.MenuManager = this.barManager1;
            this.txtAppointmentPlace.Name = "txtAppointmentPlace";
            this.txtAppointmentPlace.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.txtAppointmentPlace.Size = new System.Drawing.Size(355, 20);
            this.txtAppointmentPlace.StyleController = this.layoutControl1;
            this.txtAppointmentPlace.TabIndex = 24;
            this.txtAppointmentPlace.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtAppointmentPlace_ButtonClick);
            // 
            // dtAppointmentTime
            // 
            this.dtAppointmentTime.EditValue = null;
            this.dtAppointmentTime.Location = new System.Drawing.Point(102, 244);
            this.dtAppointmentTime.Margin = new System.Windows.Forms.Padding(2);
            this.dtAppointmentTime.MenuManager = this.barManager1;
            this.dtAppointmentTime.Name = "dtAppointmentTime";
            this.dtAppointmentTime.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtAppointmentTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Size = new System.Drawing.Size(355, 20);
            this.dtAppointmentTime.StyleController = this.layoutControl1;
            this.dtAppointmentTime.TabIndex = 22;
            this.dtAppointmentTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtAppointmentTime_Closed);
            // 
            // btnPrintAssign
            // 
            this.btnPrintAssign.Location = new System.Drawing.Point(359, 315);
            this.btnPrintAssign.Name = "btnPrintAssign";
            this.btnPrintAssign.Size = new System.Drawing.Size(98, 22);
            this.btnPrintAssign.StyleController = this.layoutControl1;
            this.btnPrintAssign.TabIndex = 19;
            this.btnPrintAssign.Text = "In phiếu CĐ";
            this.btnPrintAssign.ToolTip = "In phiếu chỉ định xét nghiệm";
            this.btnPrintAssign.Click += new System.EventHandler(this.btnPrintAssign_Click);
            // 
            // txtSampleSender
            // 
            this.txtSampleSender.Location = new System.Drawing.Point(325, 195);
            this.txtSampleSender.Margin = new System.Windows.Forms.Padding(2);
            this.txtSampleSender.MenuManager = this.barManager1;
            this.txtSampleSender.Name = "txtSampleSender";
            this.txtSampleSender.Size = new System.Drawing.Size(131, 20);
            this.txtSampleSender.StyleController = this.layoutControl1;
            this.txtSampleSender.TabIndex = 14;
            this.txtSampleSender.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSampleSender_PreviewKeyDown);
            // 
            // cboMediOrgCode
            // 
            this.cboMediOrgCode.Location = new System.Drawing.Point(164, 195);
            this.cboMediOrgCode.Margin = new System.Windows.Forms.Padding(2);
            this.cboMediOrgCode.MenuManager = this.barManager1;
            this.cboMediOrgCode.Name = "cboMediOrgCode";
            this.cboMediOrgCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboMediOrgCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cboMediOrgCode.Properties.NullText = "";
            this.cboMediOrgCode.Properties.View = this.gridView2;
            this.cboMediOrgCode.Size = new System.Drawing.Size(154, 20);
            this.cboMediOrgCode.StyleController = this.layoutControl1;
            this.cboMediOrgCode.TabIndex = 13;
            this.cboMediOrgCode.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMediOrgCode_Closed);
            this.cboMediOrgCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboMediOrgCode_ButtonClick);
            this.cboMediOrgCode.EditValueChanged += new System.EventHandler(this.cboMediOrgCode_EditValueChanged);
            this.cboMediOrgCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMediOrgCode_KeyUp);
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // txtMediOrgCode
            // 
            this.txtMediOrgCode.Location = new System.Drawing.Point(103, 195);
            this.txtMediOrgCode.Margin = new System.Windows.Forms.Padding(2);
            this.txtMediOrgCode.MenuManager = this.barManager1;
            this.txtMediOrgCode.Name = "txtMediOrgCode";
            this.txtMediOrgCode.Size = new System.Drawing.Size(61, 20);
            this.txtMediOrgCode.StyleController = this.layoutControl1;
            this.txtMediOrgCode.TabIndex = 12;
            this.txtMediOrgCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMediOrgCode_KeyDown);
            // 
            // chkPreviousStatus
            // 
            this.chkPreviousStatus.EditValue = true;
            this.chkPreviousStatus.Location = new System.Drawing.Point(436, 220);
            this.chkPreviousStatus.MenuManager = this.barManager1;
            this.chkPreviousStatus.Name = "chkPreviousStatus";
            this.chkPreviousStatus.Properties.Caption = "";
            this.chkPreviousStatus.Properties.FullFocusRect = true;
            this.chkPreviousStatus.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPreviousStatus.Size = new System.Drawing.Size(21, 19);
            this.chkPreviousStatus.StyleController = this.layoutControl1;
            this.chkPreviousStatus.TabIndex = 16;
            this.chkPreviousStatus.ToolTip = "Mặc định chọn dữ liệu của lần lưu trước";
            // 
            // cboPatientStatus
            // 
            this.cboPatientStatus.Location = new System.Drawing.Point(102, 220);
            this.cboPatientStatus.MenuManager = this.barManager1;
            this.cboPatientStatus.Name = "cboPatientStatus";
            this.cboPatientStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPatientStatus.Properties.NullText = "";
            this.cboPatientStatus.Properties.View = this.gridView1;
            this.cboPatientStatus.Size = new System.Drawing.Size(330, 20);
            this.cboPatientStatus.StyleController = this.layoutControl1;
            this.cboPatientStatus.TabIndex = 15;
            this.cboPatientStatus.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboPatientStatus_PreviewKeyDown);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(235, 315);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(120, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "In barcode (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSaveAndPrint
            // 
            this.btnSaveAndPrint.Location = new System.Drawing.Point(129, 315);
            this.btnSaveAndPrint.Name = "btnSaveAndPrint";
            this.btnSaveAndPrint.Size = new System.Drawing.Size(102, 22);
            this.btnSaveAndPrint.StyleController = this.layoutControl1;
            this.btnSaveAndPrint.TabIndex = 18;
            this.btnSaveAndPrint.Text = "Lưu in (Ctrl I)";
            this.btnSaveAndPrint.Click += new System.EventHandler(this.btnSaveAndPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(19, 315);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtBarcode
            // 
            this.txtBarcode.Location = new System.Drawing.Point(102, 74);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Properties.ReadOnly = true;
            this.txtBarcode.Size = new System.Drawing.Size(355, 20);
            this.txtBarcode.StyleController = this.layoutControl1;
            this.txtBarcode.TabIndex = 6;
            // 
            // dtSampleTime
            // 
            this.dtSampleTime.EditValue = null;
            this.dtSampleTime.Location = new System.Drawing.Point(102, 170);
            this.dtSampleTime.Name = "dtSampleTime";
            this.dtSampleTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSampleTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSampleTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtSampleTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtSampleTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtSampleTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtSampleTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtSampleTime.Size = new System.Drawing.Size(355, 20);
            this.dtSampleTime.StyleController = this.layoutControl1;
            this.dtSampleTime.TabIndex = 11;
            this.dtSampleTime.EditValueChanged += new System.EventHandler(this.dtSampleTime_EditValueChanged);
            this.dtSampleTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtSampleTime_PreviewKeyDown);
            // 
            // cboSampleUser
            // 
            this.cboSampleUser.Location = new System.Drawing.Point(221, 146);
            this.cboSampleUser.Name = "cboSampleUser";
            this.cboSampleUser.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboSampleUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSampleUser.Properties.NullText = "";
            this.cboSampleUser.Properties.View = this.gridLookUpEdit2View;
            this.cboSampleUser.Size = new System.Drawing.Size(236, 20);
            this.cboSampleUser.StyleController = this.layoutControl1;
            this.cboSampleUser.TabIndex = 10;
            this.cboSampleUser.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSampleUser_Closed);
            this.cboSampleUser.EditValueChanged += new System.EventHandler(this.cboSampleUser_EditValueChanged);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // txtSampleLoginname
            // 
            this.txtSampleLoginname.Location = new System.Drawing.Point(102, 146);
            this.txtSampleLoginname.Name = "txtSampleLoginname";
            this.txtSampleLoginname.Size = new System.Drawing.Size(119, 20);
            this.txtSampleLoginname.StyleController = this.layoutControl1;
            this.txtSampleLoginname.TabIndex = 9;
            this.txtSampleLoginname.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSampleLoginname_PreviewKeyDown);
            // 
            // cboSampleType
            // 
            this.cboSampleType.Location = new System.Drawing.Point(221, 98);
            this.cboSampleType.Name = "cboSampleType";
            this.cboSampleType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboSampleType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSampleType.Properties.NullText = "";
            this.cboSampleType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            this.cboSampleType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboSampleType.Properties.View = this.gridLookUpEdit1View;
            this.cboSampleType.Size = new System.Drawing.Size(236, 20);
            this.cboSampleType.StyleController = this.layoutControl1;
            this.cboSampleType.TabIndex = 8;
            this.cboSampleType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSampleType_Closed);
            this.cboSampleType.EditValueChanged += new System.EventHandler(this.cboSampleType_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtSampleTypeCode
            // 
            this.txtSampleTypeCode.Location = new System.Drawing.Point(102, 98);
            this.txtSampleTypeCode.Name = "txtSampleTypeCode";
            this.txtSampleTypeCode.Size = new System.Drawing.Size(119, 20);
            this.txtSampleTypeCode.StyleController = this.layoutControl1;
            this.txtSampleTypeCode.TabIndex = 7;
            this.txtSampleTypeCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSampleTypeCode_PreviewKeyDown);
            // 
            // lblPatientDob
            // 
            this.lblPatientDob.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientDob.Location = new System.Drawing.Point(306, 50);
            this.lblPatientDob.Name = "lblPatientDob";
            this.lblPatientDob.Size = new System.Drawing.Size(151, 20);
            this.lblPatientDob.StyleController = this.layoutControl1;
            this.lblPatientDob.TabIndex = 5;
            // 
            // lblGenderName
            // 
            this.lblGenderName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblGenderName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGenderName.Location = new System.Drawing.Point(102, 50);
            this.lblGenderName.Name = "lblGenderName";
            this.lblGenderName.Size = new System.Drawing.Size(105, 20);
            this.lblGenderName.StyleController = this.layoutControl1;
            this.lblGenderName.TabIndex = 4;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(306, 26);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(151, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 3;
            // 
            // lblTreatmentCode
            // 
            this.lblTreatmentCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTreatmentCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTreatmentCode.Location = new System.Drawing.Point(102, 26);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Size = new System.Drawing.Size(105, 20);
            this.lblTreatmentCode.StyleController = this.layoutControl1;
            this.lblTreatmentCode.TabIndex = 2;
            // 
            // lblServiceReqCode
            // 
            this.lblServiceReqCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblServiceReqCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblServiceReqCode.Location = new System.Drawing.Point(102, 2);
            this.lblServiceReqCode.Name = "lblServiceReqCode";
            this.lblServiceReqCode.Size = new System.Drawing.Size(355, 20);
            this.lblServiceReqCode.StyleController = this.layoutControl1;
            this.lblServiceReqCode.TabIndex = 1;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciServiceReqCode,
            this.lciPatientName,
            this.lciGenderName,
            this.lciSampleType,
            this.layoutControlItem8,
            this.lciSampleUser,
            this.layoutControlItem10,
            this.lciSampleTime,
            this.lciBarcode,
            this.layoutControlItem13,
            this.lciPatientDob,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.lciPatientStatus,
            this.layoutControlItem4,
            this.lciSampleSender,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem3,
            this.layoutControlItem7,
            this.layoutControlItem11,
            this.lciTreatmentCode,
            this.emptySpaceItem1,
            this.layoutControlItem9,
            this.emptySpaceItem2,
            this.layoutControlItem12});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(459, 339);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciServiceReqCode
            // 
            this.lciServiceReqCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceReqCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceReqCode.Control = this.lblServiceReqCode;
            this.lciServiceReqCode.Location = new System.Drawing.Point(0, 0);
            this.lciServiceReqCode.Name = "lciServiceReqCode";
            this.lciServiceReqCode.Size = new System.Drawing.Size(459, 24);
            this.lciServiceReqCode.Text = "Mã yêu cầu:";
            this.lciServiceReqCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciServiceReqCode.TextSize = new System.Drawing.Size(95, 20);
            this.lciServiceReqCode.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(209, 24);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(250, 24);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciGenderName
            // 
            this.lciGenderName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGenderName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGenderName.Control = this.lblGenderName;
            this.lciGenderName.Location = new System.Drawing.Point(0, 48);
            this.lciGenderName.Name = "lciGenderName";
            this.lciGenderName.Size = new System.Drawing.Size(209, 24);
            this.lciGenderName.Text = "Giới tính:";
            this.lciGenderName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGenderName.TextSize = new System.Drawing.Size(95, 20);
            this.lciGenderName.TextToControlDistance = 5;
            // 
            // lciSampleType
            // 
            this.lciSampleType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciSampleType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciSampleType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleType.Control = this.txtSampleTypeCode;
            this.lciSampleType.Location = new System.Drawing.Point(0, 96);
            this.lciSampleType.Name = "lciSampleType";
            this.lciSampleType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciSampleType.Size = new System.Drawing.Size(221, 24);
            this.lciSampleType.Text = "Loại mẫu:";
            this.lciSampleType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleType.TextSize = new System.Drawing.Size(95, 20);
            this.lciSampleType.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.cboSampleType;
            this.layoutControlItem8.Location = new System.Drawing.Point(221, 96);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem8.Size = new System.Drawing.Size(238, 24);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // lciSampleUser
            // 
            this.lciSampleUser.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciSampleUser.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciSampleUser.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleUser.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleUser.Control = this.txtSampleLoginname;
            this.lciSampleUser.Location = new System.Drawing.Point(0, 144);
            this.lciSampleUser.Name = "lciSampleUser";
            this.lciSampleUser.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciSampleUser.Size = new System.Drawing.Size(221, 24);
            this.lciSampleUser.Text = "Người lấy mẫu:";
            this.lciSampleUser.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleUser.TextSize = new System.Drawing.Size(95, 20);
            this.lciSampleUser.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.cboSampleUser;
            this.layoutControlItem10.Location = new System.Drawing.Point(221, 144);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem10.Size = new System.Drawing.Size(238, 24);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // lciSampleTime
            // 
            this.lciSampleTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciSampleTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciSampleTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleTime.Control = this.dtSampleTime;
            this.lciSampleTime.Location = new System.Drawing.Point(0, 168);
            this.lciSampleTime.Name = "lciSampleTime";
            this.lciSampleTime.OptionsToolTip.ToolTip = "Thời gian lấy mẫu";
            this.lciSampleTime.Size = new System.Drawing.Size(459, 24);
            this.lciSampleTime.Text = "Tg lấy mẫu:";
            this.lciSampleTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleTime.TextSize = new System.Drawing.Size(95, 20);
            this.lciSampleTime.TextToControlDistance = 5;
            // 
            // lciBarcode
            // 
            this.lciBarcode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBarcode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBarcode.Control = this.txtBarcode;
            this.lciBarcode.Location = new System.Drawing.Point(0, 72);
            this.lciBarcode.Name = "lciBarcode";
            this.lciBarcode.Size = new System.Drawing.Size(459, 24);
            this.lciBarcode.Text = "Barcode:";
            this.lciBarcode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBarcode.TextSize = new System.Drawing.Size(95, 20);
            this.lciBarcode.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.btnSave;
            this.layoutControlItem13.Location = new System.Drawing.Point(17, 313);
            this.layoutControlItem13.MaxSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem13.MinSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // lciPatientDob
            // 
            this.lciPatientDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientDob.Control = this.lblPatientDob;
            this.lciPatientDob.Location = new System.Drawing.Point(209, 48);
            this.lciPatientDob.Name = "lciPatientDob";
            this.lciPatientDob.Size = new System.Drawing.Size(250, 24);
            this.lciPatientDob.Text = "Ngày sinh:";
            this.lciPatientDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientDob.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSaveAndPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(127, 313);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 26);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(77, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(106, 26);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPrint;
            this.layoutControlItem2.Location = new System.Drawing.Point(233, 313);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(124, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciPatientStatus
            // 
            this.lciPatientStatus.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientStatus.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientStatus.Control = this.cboPatientStatus;
            this.lciPatientStatus.Location = new System.Drawing.Point(0, 218);
            this.lciPatientStatus.Name = "lciPatientStatus";
            this.lciPatientStatus.OptionsToolTip.ToolTip = "Tình trạng bệnh nhân";
            this.lciPatientStatus.Size = new System.Drawing.Size(434, 24);
            this.lciPatientStatus.Text = "Tình trạng BN:";
            this.lciPatientStatus.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientStatus.TextSize = new System.Drawing.Size(95, 20);
            this.lciPatientStatus.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.chkPreviousStatus;
            this.layoutControlItem4.Location = new System.Drawing.Point(434, 218);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(25, 24);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // lciSampleSender
            // 
            this.lciSampleSender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleSender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleSender.Control = this.txtMediOrgCode;
            this.lciSampleSender.Location = new System.Drawing.Point(0, 192);
            this.lciSampleSender.Name = "lciSampleSender";
            this.lciSampleSender.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 3, 3);
            this.lciSampleSender.Size = new System.Drawing.Size(164, 26);
            this.lciSampleSender.Text = "Đơn vị lấy mẫu:";
            this.lciSampleSender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleSender.TextSize = new System.Drawing.Size(95, 20);
            this.lciSampleSender.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.cboMediOrgCode;
            this.layoutControlItem5.Location = new System.Drawing.Point(164, 192);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 3, 3, 3);
            this.layoutControlItem5.Size = new System.Drawing.Size(157, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtSampleSender;
            this.layoutControlItem6.Location = new System.Drawing.Point(321, 192);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 3, 3, 3);
            this.layoutControlItem6.Size = new System.Drawing.Size(138, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnPrintAssign;
            this.layoutControlItem3.Location = new System.Drawing.Point(357, 313);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(102, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.dtAppointmentTime;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 242);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.OptionsToolTip.ToolTip = "Thời gian hẹn trả kết quả";
            this.layoutControlItem7.Size = new System.Drawing.Size(459, 24);
            this.layoutControlItem7.Text = "TG hẹn trả:";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(95, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem11.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem11.Control = this.txtAppointmentPlace;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 266);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(459, 24);
            this.layoutControlItem11.Text = "Nơi nhận KQ:";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(95, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTreatmentCode.Control = this.lblTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(0, 24);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(209, 24);
            this.lciTreatmentCode.Text = "Mã điều trị:";
            this.lciTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(95, 20);
            this.lciTreatmentCode.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 313);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(17, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            this.emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.chkCloseAfterPrint;
            this.layoutControlItem9.Location = new System.Drawing.Point(357, 290);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(102, 23);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 290);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(357, 23);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem12.Control = this.txtTube;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(459, 24);
            this.layoutControlItem12.Text = "Ống bệnh phẩm:";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(95, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // frmSampleInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 359);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmSampleInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thông tin lấy mẫu";
            this.Load += new System.EventHandler(this.frmSampleInfo_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTube.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCloseAfterPrint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAppointmentPlace.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleSender.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMediOrgCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMediOrgCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPreviousStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSampleTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSampleTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleLoginname.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleTypeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGenderName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleSender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.TextEdit txtBarcode;
        private DevExpress.XtraEditors.DateEdit dtSampleTime;
        private DevExpress.XtraEditors.GridLookUpEdit cboSampleUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.TextEdit txtSampleLoginname;
        private DevExpress.XtraEditors.GridLookUpEdit cboSampleType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtSampleTypeCode;
        private DevExpress.XtraEditors.LabelControl lblPatientDob;
        private DevExpress.XtraEditors.LabelControl lblGenderName;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.LabelControl lblServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciGenderName;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientDob;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleUser;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleTime;
        private DevExpress.XtraLayout.LayoutControlItem lciBarcode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSaveAndPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraBars.BarButtonItem barBtnSavePrint;
        private DevExpress.XtraBars.BarButtonItem barBtnPrint;
        private DevExpress.XtraEditors.GridLookUpEdit cboPatientStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientStatus;
        private DevExpress.XtraEditors.CheckEdit chkPreviousStatus;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.TextEdit txtSampleSender;
        private DevExpress.XtraEditors.GridLookUpEdit cboMediOrgCode;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.TextEdit txtMediOrgCode;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleSender;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.SimpleButton btnPrintAssign;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraEditors.DateEdit dtAppointmentTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private DevExpress.XtraEditors.ButtonEdit txtAppointmentPlace;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.CheckEdit chkCloseAfterPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.TextEdit txtTube;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
    }
}
