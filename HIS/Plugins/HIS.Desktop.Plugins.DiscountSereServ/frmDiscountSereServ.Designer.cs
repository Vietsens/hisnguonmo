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
namespace HIS.Desktop.Plugins.DiscountSereServ
{
    partial class frmDiscountSereServ
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
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.lblTotalSalePrice = new DevExpress.XtraEditors.LabelControl();
            this.lblDiscount = new DevExpress.XtraEditors.LabelControl();
            this.txtDiscountRatio = new DevExpress.XtraEditors.SpinEdit();
            this.txtDiscountPrice = new DevExpress.XtraEditors.SpinEdit();
            this.lblVirTotalPatientPrice = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutVirTotalPatientPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDiscountPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDiscountRatio = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDiscount = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalSalePrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscountRatio.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscountPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirTotalPatientPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscountPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscountRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalSalePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.lblTotalSalePrice);
            this.layoutControl1.Controls.Add(this.lblDiscount);
            this.layoutControl1.Controls.Add(this.txtDiscountRatio);
            this.layoutControl1.Controls.Add(this.txtDiscountPrice);
            this.layoutControl1.Controls.Add(this.lblVirTotalPatientPrice);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(220, 146);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(112, 122);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTotalSalePrice
            // 
            this.lblTotalSalePrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTotalSalePrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTotalSalePrice.Location = new System.Drawing.Point(97, 98);
            this.lblTotalSalePrice.Name = "lblTotalSalePrice";
            this.lblTotalSalePrice.Size = new System.Drawing.Size(121, 20);
            this.lblTotalSalePrice.StyleController = this.layoutControl1;
            this.lblTotalSalePrice.TabIndex = 8;
            // 
            // lblDiscount
            // 
            this.lblDiscount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDiscount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiscount.Location = new System.Drawing.Point(97, 26);
            this.lblDiscount.Name = "lblDiscount";
            this.lblDiscount.Size = new System.Drawing.Size(121, 20);
            this.lblDiscount.StyleController = this.layoutControl1;
            this.lblDiscount.TabIndex = 7;
            // 
            // txtDiscountRatio
            // 
            this.txtDiscountRatio.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtDiscountRatio.Location = new System.Drawing.Point(97, 74);
            this.txtDiscountRatio.Name = "txtDiscountRatio";
            this.txtDiscountRatio.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDiscountRatio.Properties.DisplayFormat.FormatString = "#,##0.00";
            this.txtDiscountRatio.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscountRatio.Properties.EditFormat.FormatString = "#,##0.00";
            this.txtDiscountRatio.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscountRatio.Properties.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtDiscountRatio.Size = new System.Drawing.Size(121, 20);
            this.txtDiscountRatio.StyleController = this.layoutControl1;
            this.txtDiscountRatio.TabIndex = 6;
            this.txtDiscountRatio.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDiscountRatio_PreviewKeyDown);
            // 
            // txtDiscountPrice
            // 
            this.txtDiscountPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtDiscountPrice.Location = new System.Drawing.Point(97, 50);
            this.txtDiscountPrice.Name = "txtDiscountPrice";
            this.txtDiscountPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.txtDiscountPrice.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.txtDiscountPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscountPrice.Properties.EditFormat.FormatString = "#,##0.0000";
            this.txtDiscountPrice.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtDiscountPrice.Size = new System.Drawing.Size(121, 20);
            this.txtDiscountPrice.StyleController = this.layoutControl1;
            this.txtDiscountPrice.TabIndex = 5;
            this.txtDiscountPrice.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDiscountPrice_PreviewKeyDown);
            // 
            // lblVirTotalPatientPrice
            // 
            this.lblVirTotalPatientPrice.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblVirTotalPatientPrice.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVirTotalPatientPrice.Location = new System.Drawing.Point(97, 2);
            this.lblVirTotalPatientPrice.Name = "lblVirTotalPatientPrice";
            this.lblVirTotalPatientPrice.Size = new System.Drawing.Size(121, 20);
            this.lblVirTotalPatientPrice.StyleController = this.layoutControl1;
            this.lblVirTotalPatientPrice.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutVirTotalPatientPrice,
            this.layoutDiscountPrice,
            this.layoutDiscountRatio,
            this.layoutDiscount,
            this.layoutTotalSalePrice,
            this.layoutControlItem6,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(220, 146);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutVirTotalPatientPrice
            // 
            this.layoutVirTotalPatientPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutVirTotalPatientPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutVirTotalPatientPrice.Control = this.lblVirTotalPatientPrice;
            this.layoutVirTotalPatientPrice.Location = new System.Drawing.Point(0, 0);
            this.layoutVirTotalPatientPrice.Name = "layoutVirTotalPatientPrice";
            this.layoutVirTotalPatientPrice.Size = new System.Drawing.Size(220, 24);
            this.layoutVirTotalPatientPrice.Text = "Bệnh nhân trả:";
            this.layoutVirTotalPatientPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutVirTotalPatientPrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutVirTotalPatientPrice.TextToControlDistance = 5;
            // 
            // layoutDiscountPrice
            // 
            this.layoutDiscountPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDiscountPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDiscountPrice.Control = this.txtDiscountPrice;
            this.layoutDiscountPrice.Location = new System.Drawing.Point(0, 48);
            this.layoutDiscountPrice.Name = "layoutDiscountPrice";
            this.layoutDiscountPrice.Size = new System.Drawing.Size(220, 24);
            this.layoutDiscountPrice.Text = "Theo tiền:";
            this.layoutDiscountPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDiscountPrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDiscountPrice.TextToControlDistance = 5;
            // 
            // layoutDiscountRatio
            // 
            this.layoutDiscountRatio.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDiscountRatio.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDiscountRatio.Control = this.txtDiscountRatio;
            this.layoutDiscountRatio.Location = new System.Drawing.Point(0, 72);
            this.layoutDiscountRatio.Name = "layoutDiscountRatio";
            this.layoutDiscountRatio.Size = new System.Drawing.Size(220, 24);
            this.layoutDiscountRatio.Text = "Theo %:";
            this.layoutDiscountRatio.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDiscountRatio.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDiscountRatio.TextToControlDistance = 5;
            // 
            // layoutDiscount
            // 
            this.layoutDiscount.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutDiscount.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutDiscount.Control = this.lblDiscount;
            this.layoutDiscount.Location = new System.Drawing.Point(0, 24);
            this.layoutDiscount.Name = "layoutDiscount";
            this.layoutDiscount.Size = new System.Drawing.Size(220, 24);
            this.layoutDiscount.Text = "Chiết khấu:";
            this.layoutDiscount.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutDiscount.TextSize = new System.Drawing.Size(90, 20);
            this.layoutDiscount.TextToControlDistance = 5;
            // 
            // layoutTotalSalePrice
            // 
            this.layoutTotalSalePrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalSalePrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalSalePrice.Control = this.lblTotalSalePrice;
            this.layoutTotalSalePrice.Location = new System.Drawing.Point(0, 96);
            this.layoutTotalSalePrice.Name = "layoutTotalSalePrice";
            this.layoutTotalSalePrice.Size = new System.Drawing.Size(220, 24);
            this.layoutTotalSalePrice.Text = "Cần thực thu:";
            this.layoutTotalSalePrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalSalePrice.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTotalSalePrice.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSave;
            this.layoutControlItem6.Location = new System.Drawing.Point(110, 120);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 120);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(110, 26);
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
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave, true)});
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
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(220, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 175);
            this.barDockControlBottom.Size = new System.Drawing.Size(220, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 146);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(220, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 146);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmDiscountSereServ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 175);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmDiscountSereServ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chiết khấu";
            this.Load += new System.EventHandler(this.frmDiscountSereServ_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscountRatio.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiscountPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVirTotalPatientPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscountPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscountRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDiscount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalSalePrice)).EndInit();
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
        private DevExpress.XtraEditors.LabelControl lblTotalSalePrice;
        private DevExpress.XtraEditors.LabelControl lblDiscount;
        private DevExpress.XtraEditors.SpinEdit txtDiscountRatio;
        private DevExpress.XtraEditors.SpinEdit txtDiscountPrice;
        private DevExpress.XtraEditors.LabelControl lblVirTotalPatientPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutVirTotalPatientPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutDiscountPrice;
        private DevExpress.XtraLayout.LayoutControlItem layoutDiscountRatio;
        private DevExpress.XtraLayout.LayoutControlItem layoutDiscount;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalSalePrice;
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
