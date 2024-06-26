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
namespace HIS.Desktop.Plugins.TransactionNumOrderUpdate
{
    partial class frmUpdateNumOrder
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
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblTransactionCode = new DevExpress.XtraEditors.LabelControl();
            this.lciTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblTransactionType = new DevExpress.XtraEditors.LabelControl();
            this.lciTransactionType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblGenderName = new DevExpress.XtraEditors.LabelControl();
            this.lciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblPatientDob = new DevExpress.XtraEditors.LabelControl();
            this.lciDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.spinNumOrder = new DevExpress.XtraEditors.SpinEdit();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.spinNumOrder);
            this.layoutControl1.Controls.Add(this.lblPatientDob);
            this.layoutControl1.Controls.Add(this.lblGenderName);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Controls.Add(this.lblTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblTransactionType);
            this.layoutControl1.Controls.Add(this.lblTransactionCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 132);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTransactionCode,
            this.lciTransactionType,
            this.lciTreatmentCode,
            this.lciPatientName,
            this.lciGender,
            this.lciDob,
            this.lciNumOrder,
            this.layoutControlItem1,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(440, 132);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblTransactionCode
            // 
            this.lblTransactionCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTransactionCode.Location = new System.Drawing.Point(97, 2);
            this.lblTransactionCode.Name = "lblTransactionCode";
            this.lblTransactionCode.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblTransactionCode.Size = new System.Drawing.Size(121, 20);
            this.lblTransactionCode.StyleController = this.layoutControl1;
            this.lblTransactionCode.TabIndex = 4;
            // 
            // lciTransactionCode
            // 
            this.lciTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionCode.Control = this.lblTransactionCode;
            this.lciTransactionCode.Location = new System.Drawing.Point(0, 0);
            this.lciTransactionCode.Name = "lciTransactionCode";
            this.lciTransactionCode.Size = new System.Drawing.Size(220, 24);
            this.lciTransactionCode.Text = "Mã giao dịch:";
            this.lciTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciTransactionCode.TextToControlDistance = 5;
            // 
            // lblTransactionType
            // 
            this.lblTransactionType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTransactionType.Location = new System.Drawing.Point(317, 2);
            this.lblTransactionType.Name = "lblTransactionType";
            this.lblTransactionType.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblTransactionType.Size = new System.Drawing.Size(121, 20);
            this.lblTransactionType.StyleController = this.layoutControl1;
            this.lblTransactionType.TabIndex = 5;
            // 
            // lciTransactionType
            // 
            this.lciTransactionType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTransactionType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTransactionType.Control = this.lblTransactionType;
            this.lciTransactionType.Location = new System.Drawing.Point(220, 0);
            this.lciTransactionType.Name = "lciTransactionType";
            this.lciTransactionType.Size = new System.Drawing.Size(220, 24);
            this.lciTransactionType.Text = "Loại giao dịch:";
            this.lciTransactionType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTransactionType.TextSize = new System.Drawing.Size(90, 20);
            this.lciTransactionType.TextToControlDistance = 5;
            // 
            // lblTreatmentCode
            // 
            this.lblTreatmentCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTreatmentCode.Location = new System.Drawing.Point(97, 26);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblTreatmentCode.Size = new System.Drawing.Size(121, 20);
            this.lblTreatmentCode.StyleController = this.layoutControl1;
            this.lblTreatmentCode.TabIndex = 6;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTreatmentCode.Control = this.lblTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(0, 24);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(220, 24);
            this.lciTreatmentCode.Text = "Mã điều trị:";
            this.lciTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciTreatmentCode.TextToControlDistance = 5;
            // 
            // lblPatientName
            // 
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(317, 26);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblPatientName.Size = new System.Drawing.Size(121, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 7;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(220, 24);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(220, 24);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lblGenderName
            // 
            this.lblGenderName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGenderName.Location = new System.Drawing.Point(97, 50);
            this.lblGenderName.Name = "lblGenderName";
            this.lblGenderName.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.lblGenderName.Size = new System.Drawing.Size(121, 20);
            this.lblGenderName.StyleController = this.layoutControl1;
            this.lblGenderName.TabIndex = 8;
            // 
            // lciGender
            // 
            this.lciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGender.Control = this.lblGenderName;
            this.lciGender.Location = new System.Drawing.Point(0, 48);
            this.lciGender.Name = "lciGender";
            this.lciGender.Size = new System.Drawing.Size(220, 24);
            this.lciGender.Text = "Giới tính:";
            this.lciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGender.TextSize = new System.Drawing.Size(90, 20);
            this.lciGender.TextToControlDistance = 5;
            // 
            // lblPatientDob
            // 
            this.lblPatientDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientDob.Location = new System.Drawing.Point(317, 50);
            this.lblPatientDob.Name = "lblPatientDob";
            this.lblPatientDob.Size = new System.Drawing.Size(121, 20);
            this.lblPatientDob.StyleController = this.layoutControl1;
            this.lblPatientDob.TabIndex = 9;
            // 
            // lciDob
            // 
            this.lciDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDob.Control = this.lblPatientDob;
            this.lciDob.Location = new System.Drawing.Point(220, 48);
            this.lciDob.Name = "lciDob";
            this.lciDob.Size = new System.Drawing.Size(220, 24);
            this.lciDob.Text = "Ngày sinh:";
            this.lciDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciDob.TextToControlDistance = 5;
            // 
            // spinNumOrder
            // 
            this.spinNumOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumOrder.Location = new System.Drawing.Point(97, 74);
            this.spinNumOrder.Name = "spinNumOrder";
            this.spinNumOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumOrder.Properties.MaxValue = new decimal(new int[] {
            276447231,
            23283,
            0,
            0});
            this.spinNumOrder.Size = new System.Drawing.Size(341, 20);
            this.spinNumOrder.StyleController = this.layoutControl1;
            this.spinNumOrder.TabIndex = 10;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciNumOrder.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumOrder.Control = this.spinNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(0, 72);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.OptionsToolTip.ToolTip = "Số biên lai (hóa đơn)";
            this.lciNumOrder.Size = new System.Drawing.Size(440, 24);
            this.lciNumOrder.Text = "Số BL/HĐ:";
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(90, 20);
            this.lciNumOrder.TextToControlDistance = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSave;
            this.layoutControlItem1.Location = new System.Drawing.Point(330, 96);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(110, 36);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 36);
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
            this.barBtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(440, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 161);
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 132);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 132);
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave)});
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
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmUpdateNumOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 161);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmUpdateNumOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sửa số biên lại/hóa đơn";
            this.Load += new System.EventHandler(this.frmUpdateNumOrder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTransactionType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblPatientDob;
        private DevExpress.XtraEditors.LabelControl lblGenderName;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.LabelControl lblTransactionType;
        private DevExpress.XtraEditors.LabelControl lblTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTransactionType;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciGender;
        private DevExpress.XtraLayout.LayoutControlItem lciDob;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SpinEdit spinNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
