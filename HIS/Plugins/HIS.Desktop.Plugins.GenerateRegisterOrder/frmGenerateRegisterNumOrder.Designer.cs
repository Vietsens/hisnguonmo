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
namespace HIS.Desktop.Plugins.GenerateRegisterOrder
{
    partial class frmGenerateRegisterNumOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerateRegisterNumOrder));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tileControlRegisterGate = new DevExpress.XtraEditors.TileControl();
            this.lblTitlePage = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTitlePage = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.pnlIdentityBar = new DevExpress.XtraEditors.PanelControl();
            this.lblIdentityInfo = new DevExpress.XtraEditors.LabelControl();
            this.btnCancelIdentity = new DevExpress.XtraEditors.SimpleButton();
            this.btnReprint = new DevExpress.XtraEditors.SimpleButton();
            this.lciIdentityBar = new DevExpress.XtraLayout.LayoutControlItem();
            this.tmrAutoReset = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitlePage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlIdentityBar)).BeginInit();
            this.pnlIdentityBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lciIdentityBar)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tileControlRegisterGate);
            this.layoutControl1.Controls.Add(this.lblTitlePage);
            this.layoutControl1.Controls.Add(this.pnlIdentityBar);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 660);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tileControlRegisterGate
            // 
            this.tileControlRegisterGate.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlRegisterGate.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlRegisterGate.AppearanceItem.Normal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlRegisterGate.AppearanceItem.Normal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.tileControlRegisterGate.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileControlRegisterGate.AppearanceItem.Normal.Options.UseBorderColor = true;
            this.tileControlRegisterGate.AppearanceItem.Normal.Options.UseFont = true;
            this.tileControlRegisterGate.AppearanceItem.Normal.Options.UseTextOptions = true;
            this.tileControlRegisterGate.AppearanceItem.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tileControlRegisterGate.BackColor = System.Drawing.Color.Transparent;
            this.tileControlRegisterGate.DragSize = new System.Drawing.Size(0, 0);
            this.tileControlRegisterGate.Location = new System.Drawing.Point(2, 132);
            this.tileControlRegisterGate.MaxId = 2;
            this.tileControlRegisterGate.Name = "tileControlRegisterGate";
            this.tileControlRegisterGate.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileControlRegisterGate.Size = new System.Drawing.Size(1316, 526);
            this.tileControlRegisterGate.TabIndex = 11;
            this.tileControlRegisterGate.Text = "tileControl1";
            // 
            // lblTitlePage
            // 
            this.lblTitlePage.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.lblTitlePage.Appearance.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold);
            this.lblTitlePage.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblTitlePage.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTitlePage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitlePage.Location = new System.Drawing.Point(2, 2);
            this.lblTitlePage.Name = "lblTitlePage";
            this.lblTitlePage.Size = new System.Drawing.Size(1316, 86);
            this.lblTitlePage.StyleController = this.layoutControl1;
            this.lblTitlePage.TabIndex = 4;
            this.lblTitlePage.Text = "BỆNH VIỆN HỮU NGHỊ ĐA KHOA NGHỆ AN";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("layoutControlGroup1.BackgroundImage")));
            this.layoutControlGroup1.BackgroundImageVisible = true;
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTitlePage,
            this.lciIdentityBar,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 660);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciTitlePage
            // 
            this.lciTitlePage.Control = this.lblTitlePage;
            this.lciTitlePage.Location = new System.Drawing.Point(0, 0);
            this.lciTitlePage.MaxSize = new System.Drawing.Size(0, 120);
            this.lciTitlePage.MinSize = new System.Drawing.Size(43, 80);
            this.lciTitlePage.Name = "lciTitlePage";
            this.lciTitlePage.Size = new System.Drawing.Size(1320, 90);
            this.lciTitlePage.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTitlePage.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTitlePage.TextSize = new System.Drawing.Size(0, 0);
            this.lciTitlePage.TextToControlDistance = 0;
            this.lciTitlePage.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.layoutControlItem1.AppearanceItemCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.tileControlRegisterGate;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 160);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1320, 500);
            this.layoutControlItem1.Text = "Vui lòng chọn để lấy số thứ tự";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(453, 37);
            //
            // pnlIdentityBar
            //
            this.pnlIdentityBar.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(245)))), ((int)(((byte)(252)))));
            this.pnlIdentityBar.Appearance.Options.UseBackColor = true;
            this.pnlIdentityBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlIdentityBar.Controls.Add(this.lblIdentityInfo);
            this.pnlIdentityBar.Controls.Add(this.btnReprint);
            this.pnlIdentityBar.Controls.Add(this.btnCancelIdentity);
            this.pnlIdentityBar.Location = new System.Drawing.Point(2, 92);
            this.pnlIdentityBar.Name = "pnlIdentityBar";
            this.pnlIdentityBar.Size = new System.Drawing.Size(1316, 66);
            this.pnlIdentityBar.TabIndex = 12;
            //
            // lblIdentityInfo
            //
            this.lblIdentityInfo.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold);
            this.lblIdentityInfo.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(150)))));
            this.lblIdentityInfo.Appearance.Options.UseFont = true;
            this.lblIdentityInfo.Appearance.Options.UseForeColor = true;
            this.lblIdentityInfo.Appearance.Options.UseTextOptions = true;
            this.lblIdentityInfo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblIdentityInfo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblIdentityInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblIdentityInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIdentityInfo.Location = new System.Drawing.Point(0, 0);
            this.lblIdentityInfo.Name = "lblIdentityInfo";
            this.lblIdentityInfo.Size = new System.Drawing.Size(1086, 66);
            this.lblIdentityInfo.TabIndex = 0;
            this.lblIdentityInfo.Text = "";
            //
            // btnCancelIdentity
            //
            this.btnCancelIdentity.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnCancelIdentity.Appearance.Options.UseFont = true;
            this.btnCancelIdentity.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancelIdentity.Location = new System.Drawing.Point(1086, 0);
            this.btnCancelIdentity.Name = "btnCancelIdentity";
            this.btnCancelIdentity.Size = new System.Drawing.Size(230, 66);
            this.btnCancelIdentity.TabIndex = 1;
            this.btnCancelIdentity.Text = "Hủy / chọn lại";
            this.btnCancelIdentity.Click += new System.EventHandler(this.btnCancelIdentity_Click);
            //
            // btnReprint
            //
            this.btnReprint.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.btnReprint.Appearance.Options.UseFont = true;
            this.btnReprint.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnReprint.Location = new System.Drawing.Point(886, 0);
            this.btnReprint.Name = "btnReprint";
            this.btnReprint.Size = new System.Drawing.Size(200, 66);
            this.btnReprint.TabIndex = 2;
            this.btnReprint.Text = "In lại phiếu";
            this.btnReprint.Visible = false;
            this.btnReprint.Click += new System.EventHandler(this.btnReprint_Click);
            //
            // tmrAutoReset
            //
            this.tmrAutoReset.Interval = 15000;
            this.tmrAutoReset.Tick += new System.EventHandler(this.tmrAutoReset_Tick);
            //
            // lciIdentityBar
            //
            this.lciIdentityBar.Control = this.pnlIdentityBar;
            this.lciIdentityBar.Location = new System.Drawing.Point(0, 90);
            this.lciIdentityBar.MaxSize = new System.Drawing.Size(0, 70);
            this.lciIdentityBar.MinSize = new System.Drawing.Size(43, 70);
            this.lciIdentityBar.Name = "lciIdentityBar";
            this.lciIdentityBar.Size = new System.Drawing.Size(1320, 70);
            this.lciIdentityBar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIdentityBar.TextSize = new System.Drawing.Size(0, 0);
            this.lciIdentityBar.TextToControlDistance = 0;
            this.lciIdentityBar.TextVisible = false;
            //
            // frmGenerateRegisterNumOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1320, 660);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmGenerateRegisterNumOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmGenerateRegisterNumOrder";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmGenerateRegisterNumOrder_FormClosed);
            this.Load += new System.EventHandler(this.frmGenerateRegisterNumOrder_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitlePage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlIdentityBar)).EndInit();
            this.pnlIdentityBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lciIdentityBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblTitlePage;
        private DevExpress.XtraLayout.LayoutControlItem lciTitlePage;
        private DevExpress.XtraEditors.TileControl tileControlRegisterGate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.PanelControl pnlIdentityBar;
        private DevExpress.XtraEditors.LabelControl lblIdentityInfo;
        private DevExpress.XtraEditors.SimpleButton btnCancelIdentity;
        private DevExpress.XtraEditors.SimpleButton btnReprint;
        private DevExpress.XtraLayout.LayoutControlItem lciIdentityBar;
        private System.Windows.Forms.Timer tmrAutoReset;
    }
}
