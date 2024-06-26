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
namespace HIS.Desktop.Plugins.CallPatientNumOrder
{
    partial class frmWaitingScreenNumOrder
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
            this.lblNumOrder = new DevExpress.XtraEditors.LabelControl();
            this.lblProcessExam = new DevExpress.XtraEditors.LabelControl();
            this.lblRegisterRoom = new DevExpress.XtraEditors.LabelControl();
            this.lblInvitingTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblOrganizationName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciOrganization = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInvitingTitle = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRegisterRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProcessExam = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.timer = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrganization)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInvitingTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProcessExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblNumOrder);
            this.layoutControl1.Controls.Add(this.lblProcessExam);
            this.layoutControl1.Controls.Add(this.lblRegisterRoom);
            this.layoutControl1.Controls.Add(this.lblInvitingTitle);
            this.layoutControl1.Controls.Add(this.lblOrganizationName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblNumOrder
            // 
            this.lblNumOrder.Appearance.Font = new System.Drawing.Font("Tahoma", 250F, System.Drawing.FontStyle.Bold);
            this.lblNumOrder.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblNumOrder.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblNumOrder.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNumOrder.Location = new System.Drawing.Point(2, 156);
            this.lblNumOrder.Name = "lblNumOrder";
            this.lblNumOrder.Size = new System.Drawing.Size(1330, 402);
            this.lblNumOrder.StyleController = this.layoutControl1;
            this.lblNumOrder.TabIndex = 9;
            this.lblNumOrder.Text = "0";
            // 
            // lblProcessExam
            // 
            this.lblProcessExam.Appearance.Font = new System.Drawing.Font("Tahoma", 25F, System.Drawing.FontStyle.Bold);
            this.lblProcessExam.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lblProcessExam.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblProcessExam.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblProcessExam.Location = new System.Drawing.Point(2, 649);
            this.lblProcessExam.Name = "lblProcessExam";
            this.lblProcessExam.Size = new System.Drawing.Size(1330, 41);
            this.lblProcessExam.StyleController = this.layoutControl1;
            this.lblProcessExam.TabIndex = 8;
            this.lblProcessExam.Text = "ĐỂ LÀM THỦ TỤC KHÁM BỆNH";
            // 
            // lblRegisterRoom
            // 
            this.lblRegisterRoom.Appearance.Font = new System.Drawing.Font("Tahoma", 35F, System.Drawing.FontStyle.Bold);
            this.lblRegisterRoom.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblRegisterRoom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRegisterRoom.Location = new System.Drawing.Point(10, 580);
            this.lblRegisterRoom.Name = "lblRegisterRoom";
            this.lblRegisterRoom.Size = new System.Drawing.Size(1314, 57);
            this.lblRegisterRoom.StyleController = this.layoutControl1;
            this.lblRegisterRoom.TabIndex = 7;
            this.lblRegisterRoom.Text = "VÀO QUẦY TIẾP ĐÓN SỐ 1";
            // 
            // lblInvitingTitle
            // 
            this.lblInvitingTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 35F, System.Drawing.FontStyle.Bold);
            this.lblInvitingTitle.Appearance.ForeColor = System.Drawing.Color.Purple;
            this.lblInvitingTitle.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblInvitingTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInvitingTitle.Location = new System.Drawing.Point(10, 87);
            this.lblInvitingTitle.Name = "lblInvitingTitle";
            this.lblInvitingTitle.Size = new System.Drawing.Size(1314, 57);
            this.lblInvitingTitle.StyleController = this.layoutControl1;
            this.lblInvitingTitle.TabIndex = 5;
            this.lblInvitingTitle.Text = "KÍNH MỜI KHÁCH HÀNG CÓ SỐ THỨ TỰ";
            // 
            // lblOrganizationName
            // 
            this.lblOrganizationName.Appearance.Font = new System.Drawing.Font("Tahoma", 35F, System.Drawing.FontStyle.Bold);
            this.lblOrganizationName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblOrganizationName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOrganizationName.Location = new System.Drawing.Point(10, 10);
            this.lblOrganizationName.Name = "lblOrganizationName";
            this.lblOrganizationName.Size = new System.Drawing.Size(1314, 57);
            this.lblOrganizationName.StyleController = this.layoutControl1;
            this.lblOrganizationName.TabIndex = 4;
            this.lblOrganizationName.Text = "BỆNH VIỆN ĐA KHOA TW CẦN THƠ";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.BackgroundImage = global::HIS.Desktop.Plugins.CallPatientNumOrder.Properties.Resources._19383_en_1;
            this.layoutControlGroup1.BackgroundImageVisible = true;
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciOrganization,
            this.lciInvitingTitle,
            this.lciRegisterRoom,
            this.lciProcessExam,
            this.lciNumOrder,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControlGroup1.TextVisible = false;
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
            // lciRegisterRoom
            // 
            this.lciRegisterRoom.Control = this.lblRegisterRoom;
            this.lciRegisterRoom.Location = new System.Drawing.Point(0, 570);
            this.lciRegisterRoom.Name = "lciRegisterRoom";
            this.lciRegisterRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
            this.lciRegisterRoom.Size = new System.Drawing.Size(1334, 77);
            this.lciRegisterRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lciRegisterRoom.TextSize = new System.Drawing.Size(0, 0);
            this.lciRegisterRoom.TextToControlDistance = 0;
            this.lciRegisterRoom.TextVisible = false;
            // 
            // lciProcessExam
            // 
            this.lciProcessExam.Control = this.lblProcessExam;
            this.lciProcessExam.Location = new System.Drawing.Point(0, 647);
            this.lciProcessExam.Name = "lciProcessExam";
            this.lciProcessExam.Size = new System.Drawing.Size(1334, 45);
            this.lciProcessExam.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lciProcessExam.TextSize = new System.Drawing.Size(0, 0);
            this.lciProcessExam.TextToControlDistance = 0;
            this.lciProcessExam.TextVisible = false;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.Control = this.lblNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(0, 154);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.Size = new System.Drawing.Size(1334, 406);
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(0, 0);
            this.lciNumOrder.TextToControlDistance = 0;
            this.lciNumOrder.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 560);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(1334, 10);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmWaitingScreenNumOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 692);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmWaitingScreenNumOrder";
            this.Text = "Màn hình chờ tiếp đón";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmWaitingScreenNumOrder_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOrganization)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInvitingTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRegisterRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProcessExam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.LabelControl lblNumOrder;
        private DevExpress.XtraEditors.LabelControl lblProcessExam;
        private DevExpress.XtraEditors.LabelControl lblRegisterRoom;
        private DevExpress.XtraEditors.LabelControl lblInvitingTitle;
        private DevExpress.XtraEditors.LabelControl lblOrganizationName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciOrganization;
        private DevExpress.XtraLayout.LayoutControlItem lciInvitingTitle;
        private DevExpress.XtraLayout.LayoutControlItem lciRegisterRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciProcessExam;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private System.Windows.Forms.Timer timer;

    }
}
