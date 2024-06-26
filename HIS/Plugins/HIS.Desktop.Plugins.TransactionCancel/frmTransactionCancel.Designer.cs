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
namespace HIS.Desktop.Plugins.TransactionCancel
{
    partial class frmTransactionCancel
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtRejectCancel = new DevExpress.XtraEditors.MemoEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.BarBtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnReject = new DevExpress.XtraEditors.SimpleButton();
            this.lblCancelReqReason = new DevExpress.XtraEditors.LabelControl();
            this.lblCancelReqUser = new DevExpress.XtraEditors.LabelControl();
            this.lblCancelReqRoom = new DevExpress.XtraEditors.LabelControl();
            this.lblCancelReqDepartment = new DevExpress.XtraEditors.LabelControl();
            this.lblBillNumberPos = new DevExpress.XtraEditors.LabelControl();
            this.chkConnectionPOS = new DevExpress.XtraEditors.CheckEdit();
            this.cboCancelReason = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkNotCancelBill = new DevExpress.XtraEditors.CheckEdit();
            this.btnCancelHIS = new DevExpress.XtraEditors.SimpleButton();
            this.BtnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.dtCancelTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtOtherCancelReason = new DevExpress.XtraEditors.MemoEdit();
            this.lblVirPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.lblAmount = new DevExpress.XtraEditors.LabelControl();
            this.lblTransactionCode = new DevExpress.XtraEditors.LabelControl();
            this.lblQT = new DevExpress.XtraEditors.LabelControl();
            this.lblCK = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutOtherCancelReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.LblBtnPrint = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancelHIS = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNotCancelBill = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcicheckPOS = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutVirPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancelReqDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancelReqRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancelReqUser = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCheckCK = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcicheckQT = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancelReqReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnReject = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciReasonRejectCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRejectCancel.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkConnectionPOS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCancelReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotCancelBill.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCancelTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCancelTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOtherCancelReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutOtherCancelReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LblBtnPrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelHIS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNotCancelBill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcicheckPOS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCheckCK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcicheckQT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnReject)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReasonRejectCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtRejectCancel);
            this.layoutControl1.Controls.Add(this.btnReject);
            this.layoutControl1.Controls.Add(this.lblCancelReqReason);
            this.layoutControl1.Controls.Add(this.lblCancelReqUser);
            this.layoutControl1.Controls.Add(this.lblCancelReqRoom);
            this.layoutControl1.Controls.Add(this.lblCancelReqDepartment);
            this.layoutControl1.Controls.Add(this.lblBillNumberPos);
            this.layoutControl1.Controls.Add(this.chkConnectionPOS);
            this.layoutControl1.Controls.Add(this.cboCancelReason);
            this.layoutControl1.Controls.Add(this.chkNotCancelBill);
            this.layoutControl1.Controls.Add(this.btnCancelHIS);
            this.layoutControl1.Controls.Add(this.BtnPrint);
            this.layoutControl1.Controls.Add(this.dtCancelTime);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtOtherCancelReason);
            this.layoutControl1.Controls.Add(this.lblVirPatientName);
            this.layoutControl1.Controls.Add(this.lblTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblAmount);
            this.layoutControl1.Controls.Add(this.lblTransactionCode);
            this.layoutControl1.Controls.Add(this.lblQT);
            this.layoutControl1.Controls.Add(this.lblCK);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 38);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(855, 366);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtRejectCancel
            // 
            this.txtRejectCancel.Location = new System.Drawing.Point(118, 293);
            this.txtRejectCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRejectCancel.MenuManager = this.barManager1;
            this.txtRejectCancel.Name = "txtRejectCancel";
            this.txtRejectCancel.Size = new System.Drawing.Size(734, 43);
            this.txtRejectCancel.StyleController = this.layoutControl1;
            this.txtRejectCancel.TabIndex = 19;
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
            this.BarBtnPrint});
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
            new DevExpress.XtraBars.LinkPersistInfo(this.BarBtnPrint)});
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
            // BarBtnPrint
            // 
            this.BarBtnPrint.Caption = "In (Ctrl P)";
            this.BarBtnPrint.Id = 1;
            this.BarBtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.BarBtnPrint.Name = "BarBtnPrint";
            this.BarBtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarBtnPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(855, 38);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 404);
            this.barDockControlBottom.Size = new System.Drawing.Size(855, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 366);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(855, 38);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 366);
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(654, 342);
            this.btnReject.Margin = new System.Windows.Forms.Padding(4);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(104, 20);
            this.btnReject.StyleController = this.layoutControl1;
            this.btnReject.TabIndex = 14;
            this.btnReject.Text = "Từ chối";
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // lblCancelReqReason
            // 
            this.lblCancelReqReason.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCancelReqReason.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCancelReqReason.Location = new System.Drawing.Point(555, 107);
            this.lblCancelReqReason.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblCancelReqReason.Name = "lblCancelReqReason";
            this.lblCancelReqReason.Size = new System.Drawing.Size(297, 20);
            this.lblCancelReqReason.StyleController = this.layoutControl1;
            this.lblCancelReqReason.TabIndex = 18;
            // 
            // lblCancelReqUser
            // 
            this.lblCancelReqUser.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCancelReqUser.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCancelReqUser.Location = new System.Drawing.Point(555, 81);
            this.lblCancelReqUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblCancelReqUser.Name = "lblCancelReqUser";
            this.lblCancelReqUser.Size = new System.Drawing.Size(297, 20);
            this.lblCancelReqUser.StyleController = this.layoutControl1;
            this.lblCancelReqUser.TabIndex = 17;
            // 
            // lblCancelReqRoom
            // 
            this.lblCancelReqRoom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCancelReqRoom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCancelReqRoom.Location = new System.Drawing.Point(555, 55);
            this.lblCancelReqRoom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblCancelReqRoom.Name = "lblCancelReqRoom";
            this.lblCancelReqRoom.Size = new System.Drawing.Size(297, 20);
            this.lblCancelReqRoom.StyleController = this.layoutControl1;
            this.lblCancelReqRoom.TabIndex = 16;
            // 
            // lblCancelReqDepartment
            // 
            this.lblCancelReqDepartment.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCancelReqDepartment.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCancelReqDepartment.Location = new System.Drawing.Point(555, 3);
            this.lblCancelReqDepartment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lblCancelReqDepartment.Name = "lblCancelReqDepartment";
            this.lblCancelReqDepartment.Size = new System.Drawing.Size(297, 20);
            this.lblCancelReqDepartment.StyleController = this.layoutControl1;
            this.lblCancelReqDepartment.TabIndex = 15;
            // 
            // lblBillNumberPos
            // 
            this.lblBillNumberPos.Location = new System.Drawing.Point(118, 29);
            this.lblBillNumberPos.Margin = new System.Windows.Forms.Padding(4);
            this.lblBillNumberPos.Name = "lblBillNumberPos";
            this.lblBillNumberPos.Size = new System.Drawing.Size(734, 20);
            this.lblBillNumberPos.StyleController = this.layoutControl1;
            this.lblBillNumberPos.TabIndex = 14;
            // 
            // chkConnectionPOS
            // 
            this.chkConnectionPOS.Location = new System.Drawing.Point(283, 342);
            this.chkConnectionPOS.Margin = new System.Windows.Forms.Padding(4);
            this.chkConnectionPOS.MenuManager = this.barManager1;
            this.chkConnectionPOS.Name = "chkConnectionPOS";
            this.chkConnectionPOS.Properties.Caption = "";
            this.chkConnectionPOS.Properties.FullFocusRect = true;
            this.chkConnectionPOS.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkConnectionPOS.Size = new System.Drawing.Size(174, 19);
            this.chkConnectionPOS.StyleController = this.layoutControl1;
            this.chkConnectionPOS.TabIndex = 11;
            this.chkConnectionPOS.CheckedChanged += new System.EventHandler(this.chkConnectionPOS_CheckedChanged);
            // 
            // cboCancelReason
            // 
            this.cboCancelReason.EditValue = "";
            this.cboCancelReason.Location = new System.Drawing.Point(118, 187);
            this.cboCancelReason.Margin = new System.Windows.Forms.Padding(4);
            this.cboCancelReason.MenuManager = this.barManager1;
            this.cboCancelReason.Name = "cboCancelReason";
            this.cboCancelReason.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboCancelReason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboCancelReason.Properties.NullText = "";
            this.cboCancelReason.Properties.View = this.gridLookUpEdit1View;
            this.cboCancelReason.Size = new System.Drawing.Size(734, 22);
            this.cboCancelReason.StyleController = this.layoutControl1;
            this.cboCancelReason.TabIndex = 11;
            this.cboCancelReason.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCancelReason_ButtonClick);
            this.cboCancelReason.EditValueChanged += new System.EventHandler(this.cboCancelReason_EditValueChanged);
            this.cboCancelReason.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCancelReason_KeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // chkNotCancelBill
            // 
            this.chkNotCancelBill.Location = new System.Drawing.Point(19, 342);
            this.chkNotCancelBill.Margin = new System.Windows.Forms.Padding(4);
            this.chkNotCancelBill.MenuManager = this.barManager1;
            this.chkNotCancelBill.Name = "chkNotCancelBill";
            this.chkNotCancelBill.Properties.Caption = "Không hủy HĐĐT";
            this.chkNotCancelBill.Size = new System.Drawing.Size(183, 21);
            this.chkNotCancelBill.StyleController = this.layoutControl1;
            this.chkNotCancelBill.TabIndex = 10;
            this.chkNotCancelBill.ToolTip = "Không tự động hủy hóa đơn điện tử";
            this.chkNotCancelBill.CheckedChanged += new System.EventHandler(this.chkNotCancelBill_CheckedChanged);
            // 
            // btnCancelHIS
            // 
            this.btnCancelHIS.Location = new System.Drawing.Point(463, 342);
            this.btnCancelHIS.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelHIS.Name = "btnCancelHIS";
            this.btnCancelHIS.Size = new System.Drawing.Size(104, 20);
            this.btnCancelHIS.StyleController = this.layoutControl1;
            this.btnCancelHIS.TabIndex = 12;
            this.btnCancelHIS.Text = "Hủy nội bộ HIS";
            this.btnCancelHIS.Click += new System.EventHandler(this.btnCancelHIS_Click);
            // 
            // BtnPrint
            // 
            this.BtnPrint.Location = new System.Drawing.Point(573, 342);
            this.BtnPrint.Margin = new System.Windows.Forms.Padding(4);
            this.BtnPrint.Name = "BtnPrint";
            this.BtnPrint.Size = new System.Drawing.Size(75, 20);
            this.BtnPrint.StyleController = this.layoutControl1;
            this.BtnPrint.TabIndex = 13;
            this.BtnPrint.Text = "In (Ctrl P)";
            this.BtnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // dtCancelTime
            // 
            this.dtCancelTime.EditValue = null;
            this.dtCancelTime.Location = new System.Drawing.Point(118, 159);
            this.dtCancelTime.Margin = new System.Windows.Forms.Padding(4);
            this.dtCancelTime.MenuManager = this.barManager1;
            this.dtCancelTime.Name = "dtCancelTime";
            this.dtCancelTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCancelTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCancelTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtCancelTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCancelTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtCancelTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCancelTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtCancelTime.Size = new System.Drawing.Size(734, 22);
            this.dtCancelTime.StyleController = this.layoutControl1;
            this.dtCancelTime.TabIndex = 1;
            this.dtCancelTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtCancelTime_KeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(764, 342);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 20);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtOtherCancelReason
            // 
            this.txtOtherCancelReason.Location = new System.Drawing.Point(118, 215);
            this.txtOtherCancelReason.Margin = new System.Windows.Forms.Padding(4);
            this.txtOtherCancelReason.Name = "txtOtherCancelReason";
            this.txtOtherCancelReason.Size = new System.Drawing.Size(734, 72);
            this.txtOtherCancelReason.StyleController = this.layoutControl1;
            this.txtOtherCancelReason.TabIndex = 2;
            this.txtOtherCancelReason.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCancelReason_PreviewKeyDown);
            // 
            // lblVirPatientName
            // 
            this.lblVirPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVirPatientName.Location = new System.Drawing.Point(118, 55);
            this.lblVirPatientName.Margin = new System.Windows.Forms.Padding(4);
            this.lblVirPatientName.Name = "lblVirPatientName";
            this.lblVirPatientName.Size = new System.Drawing.Size(306, 20);
            this.lblVirPatientName.StyleController = this.layoutControl1;
            this.lblVirPatientName.TabIndex = 7;
            // 
            // lblTreatmentCode
            // 
            this.lblTreatmentCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTreatmentCode.Location = new System.Drawing.Point(118, 81);
            this.lblTreatmentCode.Margin = new System.Windows.Forms.Padding(4);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Size = new System.Drawing.Size(306, 20);
            this.lblTreatmentCode.StyleController = this.layoutControl1;
            this.lblTreatmentCode.TabIndex = 6;
            // 
            // lblAmount
            // 
            this.lblAmount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblAmount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAmount.Location = new System.Drawing.Point(118, 107);
            this.lblAmount.Margin = new System.Windows.Forms.Padding(4);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(306, 20);
            this.lblAmount.StyleController = this.layoutControl1;
            this.lblAmount.TabIndex = 5;
            // 
            // lblTransactionCode
            // 
            this.lblTransactionCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTransactionCode.Location = new System.Drawing.Point(118, 3);
            this.lblTransactionCode.Margin = new System.Windows.Forms.Padding(4);
            this.lblTransactionCode.Name = "lblTransactionCode";
            this.lblTransactionCode.Size = new System.Drawing.Size(306, 20);
            this.lblTransactionCode.StyleController = this.layoutControl1;
            this.lblTransactionCode.TabIndex = 4;
            // 
            // lblQT
            // 
            this.lblQT.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblQT.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblQT.Location = new System.Drawing.Point(555, 133);
            this.lblQT.Margin = new System.Windows.Forms.Padding(4);
            this.lblQT.Name = "lblQT";
            this.lblQT.Size = new System.Drawing.Size(297, 20);
            this.lblQT.StyleController = this.layoutControl1;
            this.lblQT.TabIndex = 5;
            // 
            // lblCK
            // 
            this.lblCK.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCK.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCK.Location = new System.Drawing.Point(118, 133);
            this.lblCK.Margin = new System.Windows.Forms.Padding(4);
            this.lblCK.Name = "lblCK";
            this.lblCK.Size = new System.Drawing.Size(306, 20);
            this.lblCK.StyleController = this.layoutControl1;
            this.lblCK.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutTransactionCode,
            this.layoutTreatmentCode,
            this.layoutOtherCancelReason,
            this.layoutControlItem6,
            this.LblBtnPrint,
            this.lciCancelHIS,
            this.lciNotCancelBill,
            this.layoutControlItem3,
            this.lcicheckPOS,
            this.layoutVirPatientName,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutAmount,
            this.lciCancelReqDepartment,
            this.lciCancelReqRoom,
            this.lciCancelReqUser,
            this.lciCheckCK,
            this.lcicheckQT,
            this.lciCancelReqReason,
            this.lciBtnReject,
            this.lciReasonRejectCancel});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(855, 366);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutTransactionCode
            // 
            this.layoutTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTransactionCode.Control = this.lblTransactionCode;
            this.layoutTransactionCode.Location = new System.Drawing.Point(0, 0);
            this.layoutTransactionCode.Name = "layoutTransactionCode";
            this.layoutTransactionCode.Size = new System.Drawing.Size(427, 26);
            this.layoutTransactionCode.Text = "Mã giao dịch:";
            this.layoutTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTransactionCode.TextSize = new System.Drawing.Size(110, 20);
            this.layoutTransactionCode.TextToControlDistance = 5;
            // 
            // layoutTreatmentCode
            // 
            this.layoutTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTreatmentCode.Control = this.lblTreatmentCode;
            this.layoutTreatmentCode.Location = new System.Drawing.Point(0, 78);
            this.layoutTreatmentCode.Name = "layoutTreatmentCode";
            this.layoutTreatmentCode.Size = new System.Drawing.Size(427, 26);
            this.layoutTreatmentCode.Text = "Mã điều trị:";
            this.layoutTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTreatmentCode.TextSize = new System.Drawing.Size(110, 20);
            this.layoutTreatmentCode.TextToControlDistance = 5;
            // 
            // layoutOtherCancelReason
            // 
            this.layoutOtherCancelReason.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutOtherCancelReason.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutOtherCancelReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutOtherCancelReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutOtherCancelReason.Control = this.txtOtherCancelReason;
            this.layoutOtherCancelReason.Location = new System.Drawing.Point(0, 212);
            this.layoutOtherCancelReason.Name = "layoutOtherCancelReason";
            this.layoutOtherCancelReason.Size = new System.Drawing.Size(855, 78);
            this.layoutOtherCancelReason.Text = "Lý do khác:";
            this.layoutOtherCancelReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutOtherCancelReason.TextSize = new System.Drawing.Size(110, 20);
            this.layoutOtherCancelReason.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(761, 339);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(68, 26);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(94, 27);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // LblBtnPrint
            // 
            this.LblBtnPrint.Control = this.BtnPrint;
            this.LblBtnPrint.Location = new System.Drawing.Point(570, 339);
            this.LblBtnPrint.MaxSize = new System.Drawing.Size(110, 26);
            this.LblBtnPrint.MinSize = new System.Drawing.Size(60, 26);
            this.LblBtnPrint.Name = "LblBtnPrint";
            this.LblBtnPrint.Size = new System.Drawing.Size(81, 27);
            this.LblBtnPrint.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.LblBtnPrint.TextSize = new System.Drawing.Size(0, 0);
            this.LblBtnPrint.TextVisible = false;
            // 
            // lciCancelHIS
            // 
            this.lciCancelHIS.Control = this.btnCancelHIS;
            this.lciCancelHIS.Location = new System.Drawing.Point(460, 339);
            this.lciCancelHIS.MaxSize = new System.Drawing.Size(110, 26);
            this.lciCancelHIS.MinSize = new System.Drawing.Size(84, 26);
            this.lciCancelHIS.Name = "lciCancelHIS";
            this.lciCancelHIS.Size = new System.Drawing.Size(110, 27);
            this.lciCancelHIS.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCancelHIS.TextSize = new System.Drawing.Size(0, 0);
            this.lciCancelHIS.TextVisible = false;
            // 
            // lciNotCancelBill
            // 
            this.lciNotCancelBill.Control = this.chkNotCancelBill;
            this.lciNotCancelBill.Location = new System.Drawing.Point(16, 339);
            this.lciNotCancelBill.Name = "lciNotCancelBill";
            this.lciNotCancelBill.Size = new System.Drawing.Size(189, 27);
            this.lciNotCancelBill.TextSize = new System.Drawing.Size(0, 0);
            this.lciNotCancelBill.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.chkConnectionPOS;
            this.layoutControlItem3.Location = new System.Drawing.Point(205, 339);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(255, 27);
            this.layoutControlItem3.Text = "Kết nối POS:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // lcicheckPOS
            // 
            this.lcicheckPOS.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcicheckPOS.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcicheckPOS.Control = this.lblBillNumberPos;
            this.lcicheckPOS.Location = new System.Drawing.Point(0, 26);
            this.lcicheckPOS.Name = "lcicheckPOS";
            this.lcicheckPOS.Size = new System.Drawing.Size(855, 26);
            this.lcicheckPOS.Text = "Số hóa đơn (POS):";
            this.lcicheckPOS.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcicheckPOS.TextSize = new System.Drawing.Size(110, 20);
            this.lcicheckPOS.TextToControlDistance = 5;
            // 
            // layoutVirPatientName
            // 
            this.layoutVirPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutVirPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutVirPatientName.Control = this.lblVirPatientName;
            this.layoutVirPatientName.Location = new System.Drawing.Point(0, 52);
            this.layoutVirPatientName.Name = "layoutVirPatientName";
            this.layoutVirPatientName.Size = new System.Drawing.Size(427, 26);
            this.layoutVirPatientName.Text = "Bệnh nhân:";
            this.layoutVirPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutVirPatientName.TextSize = new System.Drawing.Size(110, 20);
            this.layoutVirPatientName.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.dtCancelTime;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 156);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(855, 28);
            this.layoutControlItem1.Text = "Thời gian hủy:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.cboCancelReason;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 184);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(855, 28);
            this.layoutControlItem2.Text = "Lý do hủy:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 339);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(16, 27);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutAmount
            // 
            this.layoutAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAmount.Control = this.lblAmount;
            this.layoutAmount.Location = new System.Drawing.Point(0, 104);
            this.layoutAmount.Name = "layoutAmount";
            this.layoutAmount.Size = new System.Drawing.Size(427, 26);
            this.layoutAmount.Text = "Số tiền:";
            this.layoutAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAmount.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutAmount.TextSize = new System.Drawing.Size(110, 20);
            this.layoutAmount.TextToControlDistance = 5;
            // 
            // lciCancelReqDepartment
            // 
            this.lciCancelReqDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCancelReqDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCancelReqDepartment.Control = this.lblCancelReqDepartment;
            this.lciCancelReqDepartment.Location = new System.Drawing.Point(427, 0);
            this.lciCancelReqDepartment.Name = "lciCancelReqDepartment";
            this.lciCancelReqDepartment.Size = new System.Drawing.Size(428, 26);
            this.lciCancelReqDepartment.Text = "Khoa yêu cầu hủy: ";
            this.lciCancelReqDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCancelReqDepartment.TextSize = new System.Drawing.Size(120, 20);
            this.lciCancelReqDepartment.TextToControlDistance = 5;
            this.lciCancelReqDepartment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciCancelReqRoom
            // 
            this.lciCancelReqRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCancelReqRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCancelReqRoom.Control = this.lblCancelReqRoom;
            this.lciCancelReqRoom.Location = new System.Drawing.Point(427, 52);
            this.lciCancelReqRoom.Name = "lciCancelReqRoom";
            this.lciCancelReqRoom.Size = new System.Drawing.Size(428, 26);
            this.lciCancelReqRoom.Text = "Phòng yêu cầu hủy: ";
            this.lciCancelReqRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCancelReqRoom.TextSize = new System.Drawing.Size(120, 20);
            this.lciCancelReqRoom.TextToControlDistance = 5;
            this.lciCancelReqRoom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciCancelReqUser
            // 
            this.lciCancelReqUser.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCancelReqUser.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCancelReqUser.Control = this.lblCancelReqUser;
            this.lciCancelReqUser.Location = new System.Drawing.Point(427, 78);
            this.lciCancelReqUser.Name = "lciCancelReqUser";
            this.lciCancelReqUser.Size = new System.Drawing.Size(428, 26);
            this.lciCancelReqUser.Text = "Người yêu cầu hủy: ";
            this.lciCancelReqUser.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCancelReqUser.TextSize = new System.Drawing.Size(120, 20);
            this.lciCancelReqUser.TextToControlDistance = 5;
            this.lciCancelReqUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciCheckCK
            // 
            this.lciCheckCK.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCheckCK.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCheckCK.Control = this.lblCK;
            this.lciCheckCK.CustomizationFormText = "Số tiền CK:";
            this.lciCheckCK.Location = new System.Drawing.Point(0, 130);
            this.lciCheckCK.Name = "lciCheckCK";
            this.lciCheckCK.Size = new System.Drawing.Size(427, 26);
            this.lciCheckCK.Text = "Số tiền CK:";
            this.lciCheckCK.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCheckCK.TextSize = new System.Drawing.Size(110, 20);
            this.lciCheckCK.TextToControlDistance = 5;
            // 
            // lcicheckQT
            // 
            this.lcicheckQT.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lcicheckQT.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lcicheckQT.Control = this.lblQT;
            this.lcicheckQT.CustomizationFormText = "Số tiền QT:";
            this.lcicheckQT.Location = new System.Drawing.Point(427, 130);
            this.lcicheckQT.Name = "lcicheckQT";
            this.lcicheckQT.Size = new System.Drawing.Size(428, 26);
            this.lcicheckQT.Text = "Số tiền QT:";
            this.lcicheckQT.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lcicheckQT.TextSize = new System.Drawing.Size(120, 20);
            this.lcicheckQT.TextToControlDistance = 5;
            // 
            // lciCancelReqReason
            // 
            this.lciCancelReqReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCancelReqReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCancelReqReason.Control = this.lblCancelReqReason;
            this.lciCancelReqReason.Location = new System.Drawing.Point(427, 104);
            this.lciCancelReqReason.Name = "lciCancelReqReason";
            this.lciCancelReqReason.Size = new System.Drawing.Size(428, 26);
            this.lciCancelReqReason.Text = "Lý do: ";
            this.lciCancelReqReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCancelReqReason.TextSize = new System.Drawing.Size(120, 20);
            this.lciCancelReqReason.TextToControlDistance = 5;
            this.lciCancelReqReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciBtnReject
            // 
            this.lciBtnReject.Control = this.btnReject;
            this.lciBtnReject.Location = new System.Drawing.Point(651, 339);
            this.lciBtnReject.MaxSize = new System.Drawing.Size(110, 26);
            this.lciBtnReject.MinSize = new System.Drawing.Size(84, 26);
            this.lciBtnReject.Name = "lciBtnReject";
            this.lciBtnReject.Size = new System.Drawing.Size(110, 27);
            this.lciBtnReject.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnReject.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnReject.TextVisible = false;
            this.lciBtnReject.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciReasonRejectCancel
            // 
            this.lciReasonRejectCancel.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciReasonRejectCancel.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciReasonRejectCancel.Control = this.txtRejectCancel;
            this.lciReasonRejectCancel.Location = new System.Drawing.Point(0, 290);
            this.lciReasonRejectCancel.Name = "lciReasonRejectCancel";
            this.lciReasonRejectCancel.Size = new System.Drawing.Size(855, 49);
            this.lciReasonRejectCancel.Text = "Lý do từ chối hủy:";
            this.lciReasonRejectCancel.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReasonRejectCancel.TextSize = new System.Drawing.Size(110, 20);
            this.lciReasonRejectCancel.TextToControlDistance = 5;
            this.lciReasonRejectCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmTransactionCancel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 404);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmTransactionCancel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hủy giao dịch";
            this.Load += new System.EventHandler(this.frmTransactionDepositCancel_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtRejectCancel.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkConnectionPOS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCancelReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotCancelBill.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCancelTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCancelTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOtherCancelReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutOtherCancelReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LblBtnPrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelHIS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNotCancelBill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcicheckPOS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCheckCK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcicheckQT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancelReqReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnReject)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReasonRejectCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.MemoEdit txtOtherCancelReason;
        private DevExpress.XtraEditors.LabelControl lblVirPatientName;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.LabelControl lblAmount;
        private DevExpress.XtraEditors.LabelControl lblTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutVirPatientName;
        private DevExpress.XtraLayout.LayoutControlItem layoutOtherCancelReason;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DateEdit dtCancelTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.SimpleButton BtnPrint;
        private DevExpress.XtraLayout.LayoutControlItem LblBtnPrint;
        private DevExpress.XtraBars.BarButtonItem BarBtnPrint;
        private DevExpress.XtraEditors.SimpleButton btnCancelHIS;
        private DevExpress.XtraLayout.LayoutControlItem lciCancelHIS;
        private DevExpress.XtraEditors.CheckEdit chkNotCancelBill;
        private DevExpress.XtraLayout.LayoutControlItem lciNotCancelBill;
        private DevExpress.XtraEditors.GridLookUpEdit cboCancelReason;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.CheckEdit chkConnectionPOS;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.LabelControl lblBillNumberPos;
        private DevExpress.XtraLayout.LayoutControlItem lcicheckPOS;
        private DevExpress.XtraEditors.LabelControl lblQT;
        private DevExpress.XtraEditors.LabelControl lblCK;
        private DevExpress.XtraLayout.LayoutControlItem lcicheckQT;
        private DevExpress.XtraLayout.LayoutControlItem lciCheckCK;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl lblCancelReqDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciCancelReqDepartment;
        private DevExpress.XtraEditors.LabelControl lblCancelReqRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciCancelReqRoom;
        private DevExpress.XtraEditors.LabelControl lblCancelReqReason;
        private DevExpress.XtraEditors.LabelControl lblCancelReqUser;
        private DevExpress.XtraLayout.LayoutControlItem lciCancelReqUser;
        private DevExpress.XtraLayout.LayoutControlItem lciCancelReqReason;
        private DevExpress.XtraEditors.SimpleButton btnReject;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnReject;
        private DevExpress.XtraEditors.MemoEdit txtRejectCancel;
        private DevExpress.XtraLayout.LayoutControlItem lciReasonRejectCancel;
    }
}
