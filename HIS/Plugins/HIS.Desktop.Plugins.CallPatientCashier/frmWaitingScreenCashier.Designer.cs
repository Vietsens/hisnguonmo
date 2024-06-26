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
namespace HIS.Desktop.Plugins.CallPatientCashier
{
    partial class frmWaitingScreenCashier
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
            this.lblProcessExam = new DevExpress.XtraEditors.LabelControl();
            this.lblRegisterRoom = new DevExpress.XtraEditors.LabelControl();
            this.lblNumOrder = new DevExpress.XtraEditors.LabelControl();
            this.lblOrganizationName = new DevExpress.XtraEditors.LabelControl();
            this.lblInvitingTitle = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciInvitingTitle = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOrganization = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciRegisterRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timer = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInvitingTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrganization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblProcessExam);
            this.layoutControl1.Controls.Add(this.lblRegisterRoom);
            this.layoutControl1.Controls.Add(this.lblNumOrder);
            this.layoutControl1.Controls.Add(this.lblOrganizationName);
            this.layoutControl1.Controls.Add(this.lblInvitingTitle);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblProcessExam
            // 
            this.lblProcessExam.Appearance.Font = new System.Drawing.Font("Tahoma", 24.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessExam.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblProcessExam.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProcessExam.Location = new System.Drawing.Point(2, 650);
            this.lblProcessExam.Name = "lblProcessExam";
            this.lblProcessExam.Size = new System.Drawing.Size(1330, 40);
            this.lblProcessExam.StyleController = this.layoutControl1;
            this.lblProcessExam.TabIndex = 8;
            this.lblProcessExam.Text = "ĐỂ LÀM THỦ TỤC THANH TOÁN";
            // 
            // lblRegisterRoom
            // 
            this.lblRegisterRoom.Appearance.Font = new System.Drawing.Font("Tahoma", 35.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRegisterRoom.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblRegisterRoom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblRegisterRoom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRegisterRoom.Location = new System.Drawing.Point(10, 581);
            this.lblRegisterRoom.Name = "lblRegisterRoom";
            this.lblRegisterRoom.Size = new System.Drawing.Size(1314, 57);
            this.lblRegisterRoom.StyleController = this.layoutControl1;
            this.lblRegisterRoom.TabIndex = 7;
            this.lblRegisterRoom.Text = "VÀO QUẦY THU NGÂN SỐ";
            // 
            // lblNumOrder
            // 
            this.lblNumOrder.Appearance.Font = new System.Drawing.Font("Tahoma", 249.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumOrder.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblNumOrder.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNumOrder.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNumOrder.Location = new System.Drawing.Point(2, 156);
            this.lblNumOrder.Name = "lblNumOrder";
            this.lblNumOrder.Size = new System.Drawing.Size(1330, 402);
            this.lblNumOrder.StyleController = this.layoutControl1;
            this.lblNumOrder.TabIndex = 6;
            this.lblNumOrder.Text = "0";
            // 
            // lblOrganizationName
            // 
            this.lblOrganizationName.Appearance.Font = new System.Drawing.Font("Tahoma", 35.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrganizationName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblOrganizationName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOrganizationName.Location = new System.Drawing.Point(10, 10);
            this.lblOrganizationName.Name = "lblOrganizationName";
            this.lblOrganizationName.Size = new System.Drawing.Size(1314, 57);
            this.lblOrganizationName.StyleController = this.layoutControl1;
            this.lblOrganizationName.TabIndex = 5;
            this.lblOrganizationName.Text = "BỆNH VIỆN BẠCH MAI";
            // 
            // lblInvitingTitle
            // 
            this.lblInvitingTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 35.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvitingTitle.Appearance.ForeColor = System.Drawing.Color.Purple;
            this.lblInvitingTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblInvitingTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInvitingTitle.Location = new System.Drawing.Point(10, 87);
            this.lblInvitingTitle.Name = "lblInvitingTitle";
            this.lblInvitingTitle.Size = new System.Drawing.Size(1314, 57);
            this.lblInvitingTitle.StyleController = this.layoutControl1;
            this.lblInvitingTitle.TabIndex = 4;
            this.lblInvitingTitle.Text = "KÍNH MỜI KHÁCH HÀNG CÓ SỐ THỨ TỰ";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.BackgroundImage = global::HIS.Desktop.Plugins.CallPatientCashier.Properties.Resources._19207_en_1;
            this.layoutControlGroup1.BackgroundImageVisible = true;
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciInvitingTitle,
            this.lciOrganization,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.lciRegisterRoom,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciInvitingTitle
            // 
            this.lciInvitingTitle.Control = this.lblInvitingTitle;
            this.lciInvitingTitle.Location = new System.Drawing.Point(0, 77);
            this.lciInvitingTitle.Name = "lciInvitingTitle";
            this.lciInvitingTitle.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lciInvitingTitle.Size = new System.Drawing.Size(1334, 77);
            this.lciInvitingTitle.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lciInvitingTitle.TextSize = new System.Drawing.Size(0, 0);
            this.lciInvitingTitle.TextToControlDistance = 0;
            this.lciInvitingTitle.TextVisible = false;
            // 
            // lciOrganization
            // 
            this.lciOrganization.Control = this.lblOrganizationName;
            this.lciOrganization.Location = new System.Drawing.Point(0, 0);
            this.lciOrganization.Name = "lciOrganization";
            this.lciOrganization.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lciOrganization.Size = new System.Drawing.Size(1334, 77);
            this.lciOrganization.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lciOrganization.TextSize = new System.Drawing.Size(0, 0);
            this.lciOrganization.TextToControlDistance = 0;
            this.lciOrganization.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.lblNumOrder;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 154);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1334, 406);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 560);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(1334, 11);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciRegisterRoom
            // 
            this.lciRegisterRoom.Control = this.lblRegisterRoom;
            this.lciRegisterRoom.Location = new System.Drawing.Point(0, 571);
            this.lciRegisterRoom.Name = "lciRegisterRoom";
            this.lciRegisterRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lciRegisterRoom.Size = new System.Drawing.Size(1334, 77);
            this.lciRegisterRoom.TextSize = new System.Drawing.Size(0, 0);
            this.lciRegisterRoom.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblProcessExam;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 648);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1334, 44);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmWaitingScreenCashier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 692);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmWaitingScreenCashier";
            this.Text = "Màn hình chờ thu ngân";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmWaitingScreenCashier_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInvitingTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrganization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblOrganizationName;
        private DevExpress.XtraEditors.LabelControl lblInvitingTitle;
        private DevExpress.XtraLayout.LayoutControlItem lciInvitingTitle;
        private DevExpress.XtraLayout.LayoutControlItem lciOrganization;
        private DevExpress.XtraEditors.LabelControl lblNumOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl lblProcessExam;
        private DevExpress.XtraEditors.LabelControl lblRegisterRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciRegisterRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.Timer timer;

    }
}
