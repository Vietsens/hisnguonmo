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
namespace HIS.Desktop.Plugins.TransactionDepositCancel
{
    partial class frmTransactionDepositCancel
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
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblTransactionCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutTransactionCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblAmount = new DevExpress.XtraEditors.LabelControl();
            this.layoutAmount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblVirPatientName = new DevExpress.XtraEditors.LabelControl();
            this.layoutVirPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtCancelReason = new DevExpress.XtraEditors.MemoEdit();
            this.layoutCancelReason = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCancelReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCancelReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtCancelReason);
            this.layoutControl1.Controls.Add(this.lblVirPatientName);
            this.layoutControl1.Controls.Add(this.lblTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblAmount);
            this.layoutControl1.Controls.Add(this.lblTransactionCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 133);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutTransactionCode,
            this.layoutAmount,
            this.layoutTreatmentCode,
            this.layoutVirPatientName,
            this.layoutCancelReason,
            this.layoutControlItem6,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(440, 133);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblTransactionCode
            // 
            this.lblTransactionCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTransactionCode.Location = new System.Drawing.Point(97, 2);
            this.lblTransactionCode.Name = "lblTransactionCode";
            this.lblTransactionCode.Size = new System.Drawing.Size(121, 20);
            this.lblTransactionCode.StyleController = this.layoutControl1;
            this.lblTransactionCode.TabIndex = 4;
            // 
            // layoutTransactionCode
            // 
            this.layoutTransactionCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTransactionCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTransactionCode.Control = this.lblTransactionCode;
            this.layoutTransactionCode.Location = new System.Drawing.Point(0, 0);
            this.layoutTransactionCode.Name = "layoutTransactionCode";
            this.layoutTransactionCode.Size = new System.Drawing.Size(220, 24);
            this.layoutTransactionCode.Text = "Mã giao dịch:";
            this.layoutTransactionCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTransactionCode.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTransactionCode.TextToControlDistance = 5;
            // 
            // lblAmount
            // 
            this.lblAmount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblAmount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAmount.Location = new System.Drawing.Point(317, 2);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(121, 20);
            this.lblAmount.StyleController = this.layoutControl1;
            this.lblAmount.TabIndex = 5;
            // 
            // layoutAmount
            // 
            this.layoutAmount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAmount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAmount.Control = this.lblAmount;
            this.layoutAmount.Location = new System.Drawing.Point(220, 0);
            this.layoutAmount.Name = "layoutAmount";
            this.layoutAmount.Size = new System.Drawing.Size(220, 24);
            this.layoutAmount.Text = "Số tiền:";
            this.layoutAmount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAmount.TextSize = new System.Drawing.Size(90, 20);
            this.layoutAmount.TextToControlDistance = 5;
            // 
            // lblTreatmentCode
            // 
            this.lblTreatmentCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTreatmentCode.Location = new System.Drawing.Point(97, 26);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Size = new System.Drawing.Size(121, 20);
            this.lblTreatmentCode.StyleController = this.layoutControl1;
            this.lblTreatmentCode.TabIndex = 6;
            // 
            // layoutTreatmentCode
            // 
            this.layoutTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTreatmentCode.Control = this.lblTreatmentCode;
            this.layoutTreatmentCode.Location = new System.Drawing.Point(0, 24);
            this.layoutTreatmentCode.Name = "layoutTreatmentCode";
            this.layoutTreatmentCode.Size = new System.Drawing.Size(220, 24);
            this.layoutTreatmentCode.Text = "Mã điều trị:";
            this.layoutTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTreatmentCode.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTreatmentCode.TextToControlDistance = 5;
            // 
            // lblVirPatientName
            // 
            this.lblVirPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVirPatientName.Location = new System.Drawing.Point(317, 26);
            this.lblVirPatientName.Name = "lblVirPatientName";
            this.lblVirPatientName.Size = new System.Drawing.Size(121, 20);
            this.lblVirPatientName.StyleController = this.layoutControl1;
            this.lblVirPatientName.TabIndex = 7;
            // 
            // layoutVirPatientName
            // 
            this.layoutVirPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutVirPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutVirPatientName.Control = this.lblVirPatientName;
            this.layoutVirPatientName.Location = new System.Drawing.Point(220, 24);
            this.layoutVirPatientName.Name = "layoutVirPatientName";
            this.layoutVirPatientName.Size = new System.Drawing.Size(220, 24);
            this.layoutVirPatientName.Text = "Bệnh nhân:";
            this.layoutVirPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutVirPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.layoutVirPatientName.TextToControlDistance = 5;
            // 
            // txtCancelReason
            // 
            this.txtCancelReason.Location = new System.Drawing.Point(97, 50);
            this.txtCancelReason.Name = "txtCancelReason";
            this.txtCancelReason.Size = new System.Drawing.Size(341, 55);
            this.txtCancelReason.StyleController = this.layoutControl1;
            this.txtCancelReason.TabIndex = 8;
            this.txtCancelReason.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCancelReason_PreviewKeyDown);
            // 
            // layoutCancelReason
            // 
            this.layoutCancelReason.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutCancelReason.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutCancelReason.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutCancelReason.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutCancelReason.Control = this.txtCancelReason;
            this.layoutCancelReason.Location = new System.Drawing.Point(0, 48);
            this.layoutCancelReason.Name = "layoutCancelReason";
            this.layoutCancelReason.Size = new System.Drawing.Size(440, 59);
            this.layoutCancelReason.Text = "Lý do:";
            this.layoutCancelReason.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutCancelReason.TextSize = new System.Drawing.Size(90, 20);
            this.layoutCancelReason.TextToControlDistance = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 109);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(330, 107);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 107);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 26);
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
            this.bbtnRCSave});
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 162);
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 133);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 133);
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave)});
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
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmTransactionDepositCancel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 162);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTransactionDepositCancel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hủy tạm ứng";
            this.Load += new System.EventHandler(this.frmTransactionDepositCancel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTransactionCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCancelReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCancelReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.MemoEdit txtCancelReason;
        private DevExpress.XtraEditors.LabelControl lblVirPatientName;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.LabelControl lblAmount;
        private DevExpress.XtraEditors.LabelControl lblTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutTransactionCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutVirPatientName;
        private DevExpress.XtraLayout.LayoutControlItem layoutCancelReason;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
