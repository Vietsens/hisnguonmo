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
namespace HIS.Desktop.Plugins.RadixChangeCabinet
{
    partial class frmFormUC
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
            this.barButtonItem__F2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem_Save = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__Them = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__Moi = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__In = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bbtnEdit = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem__F2,
            this.barButtonItem_Save,
            this.barButtonItem__Them,
            this.barButtonItem__Moi,
            this.barButtonItem__In,
            this.bbtnEdit});
            this.barManager1.MaxItemId = 6;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__F2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_Save),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__Them),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__Moi),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__In),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnEdit)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem__F2
            // 
            this.barButtonItem__F2.Caption = "F2";
            this.barButtonItem__F2.Id = 0;
            this.barButtonItem__F2.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.barButtonItem__F2.Name = "barButtonItem__F2";
            this.barButtonItem__F2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__F2_ItemClick);
            // 
            // barButtonItem_Save
            // 
            this.barButtonItem_Save.Caption = "Luu (Ctrl S)";
            this.barButtonItem_Save.Id = 1;
            this.barButtonItem_Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem_Save.Name = "barButtonItem_Save";
            this.barButtonItem_Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Save_ItemClick);
            // 
            // barButtonItem__Them
            // 
            this.barButtonItem__Them.Caption = "Them (Ctrl A)";
            this.barButtonItem__Them.Id = 2;
            this.barButtonItem__Them.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barButtonItem__Them.Name = "barButtonItem__Them";
            this.barButtonItem__Them.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Them_ItemClick);
            // 
            // barButtonItem__Moi
            // 
            this.barButtonItem__Moi.Caption = "Moi (Ctrl N)";
            this.barButtonItem__Moi.Id = 3;
            this.barButtonItem__Moi.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N));
            this.barButtonItem__Moi.Name = "barButtonItem__Moi";
            this.barButtonItem__Moi.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Moi_ItemClick);
            // 
            // barButtonItem__In
            // 
            this.barButtonItem__In.Caption = "In (Ctrl P)";
            this.barButtonItem__In.Id = 4;
            this.barButtonItem__In.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barButtonItem__In.Name = "barButtonItem__In";
            this.barButtonItem__In.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__In_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(1368, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 678);
            this.barDockControlBottom.Size = new System.Drawing.Size(1368, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 649);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1368, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 649);
            // 
            // bbtnEdit
            // 
            this.bbtnEdit.Caption = "Sửa (Ctrl E)";
            this.bbtnEdit.Id = 5;
            this.bbtnEdit.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
            this.bbtnEdit.Name = "bbtnEdit";
            this.bbtnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnEdit_ItemClick);
            // 
            // frmFormUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 678);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmFormUC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmFormUC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFormUC_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__F2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Save;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__Them;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__Moi;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__In;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnEdit;

    }
}
