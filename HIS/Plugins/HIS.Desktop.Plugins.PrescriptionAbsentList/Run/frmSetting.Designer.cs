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
namespace HIS.Desktop.Plugins.PrescriptionAbsentList.Run
{
    partial class frmSetting
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
            this.layoutControlRoot = new DevExpress.XtraLayout.LayoutControl();
            this.spnCoChuTenQuay = new DevExpress.XtraEditors.SpinEdit();
            this.spnCoChuTenBN = new DevExpress.XtraEditors.SpinEdit();
            this.spnCoChuSTT = new DevExpress.XtraEditors.SpinEdit();
            this.btnOpenScreen = new DevExpress.XtraEditors.SimpleButton();
            this.spRowNumber = new DevExpress.XtraEditors.SpinEdit();
            this.lblRoomName = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.chkAutoOpenWaitingScreen = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuTenQuay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuTenBN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuSTT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spRowNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoOpenWaitingScreen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.Controls.Add(this.chkAutoOpenWaitingScreen);
            this.layoutControlRoot.Controls.Add(this.spnCoChuTenQuay);
            this.layoutControlRoot.Controls.Add(this.spnCoChuTenBN);
            this.layoutControlRoot.Controls.Add(this.spnCoChuSTT);
            this.layoutControlRoot.Controls.Add(this.btnOpenScreen);
            this.layoutControlRoot.Controls.Add(this.spRowNumber);
            this.layoutControlRoot.Controls.Add(this.lblRoomName);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(340, 170, 250, 350);
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(384, 171);
            this.layoutControlRoot.TabIndex = 0;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // spnCoChuTenQuay
            // 
            this.spnCoChuTenQuay.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuTenQuay.Location = new System.Drawing.Point(157, 107);
            this.spnCoChuTenQuay.Name = "spnCoChuTenQuay";
            this.spnCoChuTenQuay.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnCoChuTenQuay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnCoChuTenQuay.Properties.IsFloatValue = false;
            this.spnCoChuTenQuay.Properties.Mask.EditMask = "N00";
            this.spnCoChuTenQuay.Properties.MaxValue = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.spnCoChuTenQuay.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuTenQuay.Size = new System.Drawing.Size(225, 20);
            this.spnCoChuTenQuay.StyleController = this.layoutControlRoot;
            this.spnCoChuTenQuay.TabIndex = 9;
            // 
            // spnCoChuTenBN
            // 
            this.spnCoChuTenBN.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuTenBN.Location = new System.Drawing.Point(157, 83);
            this.spnCoChuTenBN.Name = "spnCoChuTenBN";
            this.spnCoChuTenBN.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnCoChuTenBN.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnCoChuTenBN.Properties.IsFloatValue = false;
            this.spnCoChuTenBN.Properties.Mask.EditMask = "N00";
            this.spnCoChuTenBN.Properties.MaxValue = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.spnCoChuTenBN.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuTenBN.Size = new System.Drawing.Size(225, 20);
            this.spnCoChuTenBN.StyleController = this.layoutControlRoot;
            this.spnCoChuTenBN.TabIndex = 8;
            // 
            // spnCoChuSTT
            // 
            this.spnCoChuSTT.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuSTT.Location = new System.Drawing.Point(157, 59);
            this.spnCoChuSTT.Name = "spnCoChuSTT";
            this.spnCoChuSTT.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spnCoChuSTT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spnCoChuSTT.Properties.IsFloatValue = false;
            this.spnCoChuSTT.Properties.Mask.EditMask = "N00";
            this.spnCoChuSTT.Properties.MaxValue = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.spnCoChuSTT.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spnCoChuSTT.Size = new System.Drawing.Size(225, 20);
            this.spnCoChuSTT.StyleController = this.layoutControlRoot;
            this.spnCoChuSTT.TabIndex = 7;
            // 
            // btnOpenScreen
            // 
            this.btnOpenScreen.Location = new System.Drawing.Point(266, 131);
            this.btnOpenScreen.Name = "btnOpenScreen";
            this.btnOpenScreen.Size = new System.Drawing.Size(116, 22);
            this.btnOpenScreen.StyleController = this.layoutControlRoot;
            this.btnOpenScreen.TabIndex = 6;
            this.btnOpenScreen.Text = "Bật màn hình";
            this.btnOpenScreen.Click += new System.EventHandler(this.btnOpenScreen_Click);
            // 
            // spRowNumber
            // 
            this.spRowNumber.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spRowNumber.Location = new System.Drawing.Point(157, 35);
            this.spRowNumber.Name = "spRowNumber";
            this.spRowNumber.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spRowNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spRowNumber.Properties.IsFloatValue = false;
            this.spRowNumber.Properties.Mask.EditMask = "N00";
            this.spRowNumber.Properties.MaxValue = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.spRowNumber.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spRowNumber.Size = new System.Drawing.Size(225, 20);
            this.spRowNumber.StyleController = this.layoutControlRoot;
            this.spRowNumber.TabIndex = 5;
            this.spRowNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spRowNumber_PreviewKeyDown);
            // 
            // lblRoomName
            // 
            this.lblRoomName.Appearance.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoomName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRoomName.Location = new System.Drawing.Point(2, 12);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(380, 19);
            this.lblRoomName.StyleController = this.layoutControlRoot;
            this.lblRoomName.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 0);
            this.Root.Size = new System.Drawing.Size(384, 171);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblRoomName;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(384, 23);
            this.layoutControlItem1.Text = " ";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.spRowNumber;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(384, 24);
            this.layoutControlItem2.Text = "Số dòng trên 1 trang:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnOpenScreen;
            this.layoutControlItem3.Location = new System.Drawing.Point(264, 119);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(120, 42);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 143);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(264, 18);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.spnCoChuSTT;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 47);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(384, 24);
            this.layoutControlItem4.Text = "Cỡ chữ STT:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.spnCoChuTenBN;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 71);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(384, 24);
            this.layoutControlItem5.Text = "Cỡ chữ tên bệnh nhân:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.spnCoChuTenQuay;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 95);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(384, 24);
            this.layoutControlItem6.Text = "Cỡ chữ tên quầy:";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // chkAutoOpenWaitingScreen
            // 
            this.chkAutoOpenWaitingScreen.Location = new System.Drawing.Point(157, 131);
            this.chkAutoOpenWaitingScreen.Name = "chkAutoOpenWaitingScreen";
            this.chkAutoOpenWaitingScreen.Properties.Caption = "";
            this.chkAutoOpenWaitingScreen.Size = new System.Drawing.Size(105, 19);
            this.chkAutoOpenWaitingScreen.StyleController = this.layoutControlRoot;
            this.chkAutoOpenWaitingScreen.TabIndex = 10;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.chkAutoOpenWaitingScreen;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 119);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(264, 24);
            this.layoutControlItem7.Text = "Tự động mở màn hình chờ";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(150, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 171);
            this.Controls.Add(this.layoutControlRoot);
            this.MaximumSize = new System.Drawing.Size(400, 210);
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "frmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách vắng mặt phát thuốc";
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.Controls.SetChildIndex(this.layoutControlRoot, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuTenQuay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuTenBN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnCoChuSTT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spRowNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoOpenWaitingScreen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton btnOpenScreen;
        private DevExpress.XtraEditors.SpinEdit spRowNumber;
        private DevExpress.XtraEditors.LabelControl lblRoomName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SpinEdit spnCoChuSTT;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SpinEdit spnCoChuTenQuay;
        private DevExpress.XtraEditors.SpinEdit spnCoChuTenBN;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.CheckEdit chkAutoOpenWaitingScreen;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
    }
}
