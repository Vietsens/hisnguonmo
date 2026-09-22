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
namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup
{
    partial class frmEmrToolkitOtp
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.txtOtp = new DevExpress.XtraEditors.TextEdit();
            this.lblChannel = new DevExpress.XtraEditors.LabelControl();
            this.lblExpire = new DevExpress.XtraEditors.LabelControl();
            this.btnAccept = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciOtp = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChannel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpire = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAccept = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.tmrCountdown = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOtp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOtp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChannel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl
            //
            this.layoutControl.Controls.Add(this.txtOtp);
            this.layoutControl.Controls.Add(this.lblChannel);
            this.layoutControl.Controls.Add(this.lblExpire);
            this.layoutControl.Controls.Add(this.btnAccept);
            this.layoutControl.Controls.Add(this.btnCancel);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(434, 168);
            this.layoutControl.TabIndex = 0;
            //
            // txtOtp
            //
            this.txtOtp.Location = new System.Drawing.Point(114, 62);
            this.txtOtp.Name = "txtOtp";
            this.txtOtp.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtOtp.Properties.Appearance.Options.UseFont = true;
            this.txtOtp.Properties.MaxLength = 10;
            this.txtOtp.Properties.NullValuePrompt = "Nhập mã OTP";
            this.txtOtp.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtOtp.Size = new System.Drawing.Size(308, 24);
            this.txtOtp.StyleController = this.layoutControl;
            this.txtOtp.TabIndex = 0;
            this.txtOtp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOtp_KeyDown);
            //
            // lblChannel
            //
            this.lblChannel.Location = new System.Drawing.Point(114, 12);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(308, 13);
            this.lblChannel.StyleController = this.layoutControl;
            this.lblChannel.TabIndex = 1;
            //
            // lblExpire
            //
            this.lblExpire.Location = new System.Drawing.Point(114, 37);
            this.lblExpire.Name = "lblExpire";
            this.lblExpire.Size = new System.Drawing.Size(308, 13);
            this.lblExpire.StyleController = this.layoutControl;
            this.lblExpire.TabIndex = 2;
            //
            // btnAccept
            //
            this.btnAccept.Location = new System.Drawing.Point(222, 98);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(98, 30);
            this.btnAccept.StyleController = this.layoutControl;
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "Xem kết quả";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(324, 98);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 30);
            this.btnCancel.StyleController = this.layoutControl;
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Đóng";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciChannel,
            this.lciExpire,
            this.lciOtp,
            this.lciAccept,
            this.lciCancel,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(434, 168);
            this.Root.TextVisible = false;
            //
            // lciOtp
            //
            this.lciOtp.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciOtp.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciOtp.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciOtp.Control = this.txtOtp;
            this.lciOtp.Location = new System.Drawing.Point(0, 50);
            this.lciOtp.Name = "lciOtp";
            this.lciOtp.Size = new System.Drawing.Size(414, 28);
            this.lciOtp.Text = "Mã OTP";
            this.lciOtp.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciOtp.TextSize = new System.Drawing.Size(98, 20);
            //
            // lciChannel
            //
            this.lciChannel.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChannel.Control = this.lblChannel;
            this.lciChannel.Location = new System.Drawing.Point(0, 0);
            this.lciChannel.Name = "lciChannel";
            this.lciChannel.Size = new System.Drawing.Size(414, 25);
            this.lciChannel.Text = "Kênh gửi OTP";
            this.lciChannel.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChannel.TextSize = new System.Drawing.Size(98, 20);
            //
            // lciExpire
            //
            this.lciExpire.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpire.Control = this.lblExpire;
            this.lciExpire.Location = new System.Drawing.Point(0, 25);
            this.lciExpire.Name = "lciExpire";
            this.lciExpire.Size = new System.Drawing.Size(414, 25);
            this.lciExpire.Text = "Hiệu lực còn";
            this.lciExpire.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpire.TextSize = new System.Drawing.Size(98, 20);
            //
            // lciAccept
            //
            this.lciAccept.Control = this.btnAccept;
            this.lciAccept.Location = new System.Drawing.Point(210, 78);
            this.lciAccept.MaxSize = new System.Drawing.Size(102, 34);
            this.lciAccept.MinSize = new System.Drawing.Size(102, 34);
            this.lciAccept.Name = "lciAccept";
            this.lciAccept.Size = new System.Drawing.Size(102, 70);
            this.lciAccept.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciAccept.TextSize = new System.Drawing.Size(0, 0);
            this.lciAccept.TextVisible = false;
            //
            // lciCancel
            //
            this.lciCancel.Control = this.btnCancel;
            this.lciCancel.Location = new System.Drawing.Point(312, 78);
            this.lciCancel.MaxSize = new System.Drawing.Size(102, 34);
            this.lciCancel.MinSize = new System.Drawing.Size(102, 34);
            this.lciCancel.Name = "lciCancel";
            this.lciCancel.Size = new System.Drawing.Size(102, 70);
            this.lciCancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lciCancel.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 78);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(210, 70);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // tmrCountdown
            //
            this.tmrCountdown.Interval = 1000;
            this.tmrCountdown.Tick += new System.EventHandler(this.tmrCountdown_Tick);
            //
            // frmEmrToolkitOtp
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 168);
            this.Controls.Add(this.layoutControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 200);
            this.Name = "frmEmrToolkitOtp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xác thực OTP xem kết quả liên thông";
            this.Load += new System.EventHandler(this.frmEmrToolkitOtp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChannel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOtp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOtp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtOtp;
        private DevExpress.XtraEditors.LabelControl lblChannel;
        private DevExpress.XtraEditors.LabelControl lblExpire;
        private DevExpress.XtraEditors.SimpleButton btnAccept;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlItem lciOtp;
        private DevExpress.XtraLayout.LayoutControlItem lciChannel;
        private DevExpress.XtraLayout.LayoutControlItem lciExpire;
        private DevExpress.XtraLayout.LayoutControlItem lciAccept;
        private DevExpress.XtraLayout.LayoutControlItem lciCancel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private System.Windows.Forms.Timer tmrCountdown;
    }
}
