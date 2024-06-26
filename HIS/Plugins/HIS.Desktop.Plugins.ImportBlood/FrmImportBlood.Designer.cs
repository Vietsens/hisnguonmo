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
namespace HIS.Desktop.Plugins.ImportBlood
{
    partial class FrmImportBlood
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
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnThem = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnCapNhat = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnHuy = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnLuu = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnLuuNhap = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnMoi = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnIn = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
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
            this.bbtnThem,
            this.bbtnCapNhat,
            this.bbtnHuy,
            this.bbtnLuu,
            this.bbtnLuuNhap,
            this.bbtnMoi,
            this.bbtnIn});
            this.barManager1.MaxItemId = 7;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnThem),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnCapNhat),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnHuy),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnLuu),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnLuuNhap),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnMoi),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnIn)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnThem
            // 
            this.bbtnThem.Caption = "Thêm (Ctrl A)";
            this.bbtnThem.Id = 0;
            this.bbtnThem.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.bbtnThem.Name = "bbtnThem";
            this.bbtnThem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnThem_ItemClick);
            // 
            // bbtnCapNhat
            // 
            this.bbtnCapNhat.Caption = "Cập nhật (Ctrl U)";
            this.bbtnCapNhat.Id = 1;
            this.bbtnCapNhat.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U));
            this.bbtnCapNhat.Name = "bbtnCapNhat";
            this.bbtnCapNhat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnCapNhat_ItemClick);
            // 
            // bbtnHuy
            // 
            this.bbtnHuy.Caption = "Hủy (Ctrl Q)";
            this.bbtnHuy.Id = 2;
            this.bbtnHuy.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q));
            this.bbtnHuy.Name = "bbtnHuy";
            this.bbtnHuy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnHuy_ItemClick);
            // 
            // bbtnLuu
            // 
            this.bbtnLuu.Caption = "Lưu (Ctrl S)";
            this.bbtnLuu.Id = 3;
            this.bbtnLuu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnLuu.Name = "bbtnLuu";
            this.bbtnLuu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnLuu_ItemClick);
            // 
            // bbtnLuuNhap
            // 
            this.bbtnLuuNhap.Caption = "Lưu nháp (Ctrl D)";
            this.bbtnLuuNhap.Id = 4;
            this.bbtnLuuNhap.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.bbtnLuuNhap.Name = "bbtnLuuNhap";
            this.bbtnLuuNhap.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnLuuNhap_ItemClick);
            // 
            // bbtnMoi
            // 
            this.bbtnMoi.Caption = "Mới (Ctrl N)";
            this.bbtnMoi.Id = 5;
            this.bbtnMoi.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.bbtnMoi.Name = "bbtnMoi";
            this.bbtnMoi.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnMoi_ItemClick);
            // 
            // bbtnIn
            // 
            this.bbtnIn.Caption = "In (Ctrl P)";
            this.bbtnIn.Id = 6;
            this.bbtnIn.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnIn.Name = "bbtnIn";
            this.bbtnIn.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnIn_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(788, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 470);
            this.barDockControlBottom.Size = new System.Drawing.Size(788, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 441);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(788, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 441);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(788, 441);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Location = new System.Drawing.Point(2, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(784, 437);
            this.panelControl1.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(788, 441);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.panelControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(788, 441);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // FrmImportBlood
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 470);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FrmImportBlood";
            this.Text = "Sửa phiếu nhập máu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmImportBlood_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnThem;
        private DevExpress.XtraBars.BarButtonItem bbtnCapNhat;
        private DevExpress.XtraBars.BarButtonItem bbtnHuy;
        private DevExpress.XtraBars.BarButtonItem bbtnLuu;
        private DevExpress.XtraBars.BarButtonItem bbtnLuuNhap;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnMoi;
        private DevExpress.XtraBars.BarButtonItem bbtnIn;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;

    }
}
