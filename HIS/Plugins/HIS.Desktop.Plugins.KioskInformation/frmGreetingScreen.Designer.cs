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
namespace HIS.Desktop.Plugins.KioskInformation
{
    partial class frmGreetingScreen
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
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.labelError = new System.Windows.Forms.Label();
            this.btnAgree = new System.Windows.Forms.Button();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciGreetingLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciError = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.lblBranchName = new System.Windows.Forms.Label();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBranch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGreeting = new DevExpress.XtraLayout.LayoutControlItem();
            this.timer = new System.Windows.Forms.Timer();
            this.timerWallPaper = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGreetingLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGreeting)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl3);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1360, 729);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl3
            // 
            this.layoutControl3.BackColor = System.Drawing.Color.Transparent;
            this.layoutControl3.Controls.Add(this.labelError);
            this.layoutControl3.Controls.Add(this.btnAgree);
            this.layoutControl3.Controls.Add(this.txtTreatmentCode);
            this.layoutControl3.Controls.Add(this.label2);
            this.layoutControl3.Location = new System.Drawing.Point(0, 106);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(1360, 623);
            this.layoutControl3.TabIndex = 5;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // labelError
            // 
            this.labelError.BackColor = System.Drawing.Color.White;
            this.labelError.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelError.ForeColor = System.Drawing.Color.Crimson;
            this.labelError.Location = new System.Drawing.Point(2, 253);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(1356, 146);
            this.labelError.TabIndex = 9;
            this.labelError.Text = "Không tồn tại dữ liệu trên hệ thống. Vui lòng kiểm tra lại thông tin đã nhập.";
            this.labelError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAgree
            // 
            this.btnAgree.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAgree.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnAgree.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAgree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgree.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgree.ForeColor = System.Drawing.Color.White;
            this.btnAgree.Location = new System.Drawing.Point(785, 165);
            this.btnAgree.Margin = new System.Windows.Forms.Padding(0);
            this.btnAgree.MaximumSize = new System.Drawing.Size(0, 53);
            this.btnAgree.MinimumSize = new System.Drawing.Size(0, 53);
            this.btnAgree.Name = "btnAgree";
            this.btnAgree.Size = new System.Drawing.Size(166, 53);
            this.btnAgree.TabIndex = 8;
            this.btnAgree.Text = "ĐỒNG Ý";
            this.btnAgree.UseVisualStyleBackColor = false;
            this.btnAgree.Click += new System.EventHandler(this.btnAgree_Click);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.EditValue = "";
            this.txtTreatmentCode.EnterMoveNextControl = true;
            this.txtTreatmentCode.Location = new System.Drawing.Point(409, 165);
            this.txtTreatmentCode.Margin = new System.Windows.Forms.Padding(0);
            this.txtTreatmentCode.MaximumSize = new System.Drawing.Size(0, 53);
            this.txtTreatmentCode.MinimumSize = new System.Drawing.Size(0, 53);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 24F);
            this.txtTreatmentCode.Properties.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.txtTreatmentCode.Properties.Appearance.Options.UseFont = true;
            this.txtTreatmentCode.Properties.Appearance.Options.UseForeColor = true;
            this.txtTreatmentCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.txtTreatmentCode.Properties.ValidateOnEnterKey = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(376, 48);
            this.txtTreatmentCode.StyleController = this.layoutControl3;
            this.txtTreatmentCode.TabIndex = 7;
            this.txtTreatmentCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTreatmentCode_KeyDown);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Calibri", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label2.Location = new System.Drawing.Point(0, 20);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(130, 0, 130, 0);
            this.label2.Size = new System.Drawing.Size(1360, 125);
            this.label2.TabIndex = 4;
            this.label2.Text = "VUI LÒNG NHẬP SỐ ĐIỆN THOẠI HOẶC MÃ ĐIỀU TRỊ ĐỂ TRA CỨU THÔNG TIN";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.BackgroundImageVisible = true;
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGreetingLabel,
            this.emptySpaceItem4,
            this.emptySpaceItem6,
            this.emptySpaceItem8,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.emptySpaceItem9,
            this.lciError});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1360, 623);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // lciGreetingLabel
            // 
            this.lciGreetingLabel.Control = this.label2;
            this.lciGreetingLabel.Location = new System.Drawing.Point(0, 0);
            this.lciGreetingLabel.MinSize = new System.Drawing.Size(20, 60);
            this.lciGreetingLabel.Name = "lciGreetingLabel";
            this.lciGreetingLabel.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 20, 20);
            this.lciGreetingLabel.Size = new System.Drawing.Size(1360, 165);
            this.lciGreetingLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGreetingLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciGreetingLabel.TextVisible = false;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(0, 401);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(1360, 222);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem6
            // 
            this.emptySpaceItem6.AllowHotTrack = false;
            this.emptySpaceItem6.Location = new System.Drawing.Point(0, 165);
            this.emptySpaceItem6.Name = "emptySpaceItem6";
            this.emptySpaceItem6.Size = new System.Drawing.Size(409, 53);
            this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem8
            // 
            this.emptySpaceItem8.AllowHotTrack = false;
            this.emptySpaceItem8.Location = new System.Drawing.Point(951, 165);
            this.emptySpaceItem8.Name = "emptySpaceItem8";
            this.emptySpaceItem8.Size = new System.Drawing.Size(409, 53);
            this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.BackColor = System.Drawing.Color.Transparent;
            this.layoutControlItem5.AppearanceItemCaption.BackColor2 = System.Drawing.Color.Transparent;
            this.layoutControlItem5.AppearanceItemCaption.BorderColor = System.Drawing.Color.Blue;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseBackColor = true;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseBorderColor = true;
            this.layoutControlItem5.Control = this.txtTreatmentCode;
            this.layoutControlItem5.Location = new System.Drawing.Point(409, 165);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 48);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(1, 48);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem5.Size = new System.Drawing.Size(376, 53);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnAgree;
            this.layoutControlItem6.Location = new System.Drawing.Point(785, 165);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Size = new System.Drawing.Size(166, 53);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem9
            // 
            this.emptySpaceItem9.AllowHotTrack = false;
            this.emptySpaceItem9.Location = new System.Drawing.Point(0, 218);
            this.emptySpaceItem9.Name = "emptySpaceItem9";
            this.emptySpaceItem9.Size = new System.Drawing.Size(1360, 33);
            this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciError
            // 
            this.lciError.Control = this.labelError;
            this.lciError.Location = new System.Drawing.Point(0, 251);
            this.lciError.MinSize = new System.Drawing.Size(24, 24);
            this.lciError.Name = "lciError";
            this.lciError.Size = new System.Drawing.Size(1360, 150);
            this.lciError.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciError.TextSize = new System.Drawing.Size(0, 0);
            this.lciError.TextVisible = false;
            this.lciError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControl2
            // 
            this.layoutControl2.BackColor = System.Drawing.Color.DodgerBlue;
            this.layoutControl2.Controls.Add(this.lblBranchName);
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1360, 106);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // lblBranchName
            // 
            this.lblBranchName.BackColor = System.Drawing.Color.DodgerBlue;
            this.lblBranchName.Font = new System.Drawing.Font("Calibri", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBranchName.ForeColor = System.Drawing.Color.White;
            this.lblBranchName.Location = new System.Drawing.Point(12, 12);
            this.lblBranchName.Name = "lblBranchName";
            this.lblBranchName.Size = new System.Drawing.Size(1336, 82);
            this.lblBranchName.TabIndex = 4;
            this.lblBranchName.Text = "CHÀO MỪNG BẠN ĐẾN VỚI ";
            this.lblBranchName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1360, 106);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblBranchName;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1340, 86);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.BackgroundImage = global::HIS.Desktop.Plugins.KioskInformation.Properties.Resources.kiosk_background_1366;
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBranch,
            this.lciGreeting});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1360, 729);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciBranch
            // 
            this.lciBranch.Control = this.layoutControl2;
            this.lciBranch.Location = new System.Drawing.Point(0, 0);
            this.lciBranch.MinSize = new System.Drawing.Size(44, 44);
            this.lciBranch.Name = "lciBranch";
            this.lciBranch.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciBranch.Size = new System.Drawing.Size(1360, 106);
            this.lciBranch.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBranch.TextSize = new System.Drawing.Size(0, 0);
            this.lciBranch.TextVisible = false;
            // 
            // lciGreeting
            // 
            this.lciGreeting.Control = this.layoutControl3;
            this.lciGreeting.Location = new System.Drawing.Point(0, 106);
            this.lciGreeting.MinSize = new System.Drawing.Size(124, 177);
            this.lciGreeting.Name = "lciGreeting";
            this.lciGreeting.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciGreeting.Size = new System.Drawing.Size(1360, 623);
            this.lciGreeting.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGreeting.TextSize = new System.Drawing.Size(0, 0);
            this.lciGreeting.TextVisible = false;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // timerWallPaper
            // 
            this.timerWallPaper.Interval = 1000;
            //this.timerWallPaper.Tick += new System.EventHandler(this.timerWallPaper_Tick);
            // 
            // frmGreetingScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 729);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGreetingScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmGreetingScreen_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGreetingLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGreeting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciBranch;
        private DevExpress.XtraLayout.LayoutControlItem lciGreeting;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraLayout.LayoutControlItem lciGreetingLabel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem8;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private System.Windows.Forms.Label lblBranchName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private System.Windows.Forms.Button btnAgree;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer timerWallPaper;
        private System.Windows.Forms.Label labelError;
        private DevExpress.XtraLayout.LayoutControlItem lciError;
    }
}
