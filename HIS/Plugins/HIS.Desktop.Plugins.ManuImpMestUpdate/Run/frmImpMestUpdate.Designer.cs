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
namespace HIS.Desktop.Plugins.ManuImpMestUpdate.Run
{
    partial class frmImpMestUpdate
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
            this.barButtonItem__Save = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__CtrlU = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem_CtrlR = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__CtrlA = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem__CtrlP = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bbtnFocusSearch = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
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
            this.barButtonItem__Save,
            this.barButtonItem__CtrlU,
            this.barButtonItem_CtrlR,
            this.barButtonItem__CtrlA,
            this.barButtonItem__CtrlP,
            this.bbtnFocusSearch});
            this.barManager1.MaxItemId = 6;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__Save),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__CtrlU),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_CtrlR),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__CtrlA),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__CtrlP),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnFocusSearch)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem__Save
            // 
            this.barButtonItem__Save.Caption = "Luu (Ctrl S)";
            this.barButtonItem__Save.Id = 0;
            this.barButtonItem__Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem__Save.Name = "barButtonItem__Save";
            this.barButtonItem__Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Save_ItemClick);
            // 
            // barButtonItem__CtrlU
            // 
            this.barButtonItem__CtrlU.Caption = "Cap Nhat(Ctrl U)";
            this.barButtonItem__CtrlU.Id = 1;
            this.barButtonItem__CtrlU.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U));
            this.barButtonItem__CtrlU.Name = "barButtonItem__CtrlU";
            this.barButtonItem__CtrlU.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__CtrlU_ItemClick);
            // 
            // barButtonItem_CtrlR
            // 
            this.barButtonItem_CtrlR.Caption = "Huy (Ctrl R)";
            this.barButtonItem_CtrlR.Id = 2;
            this.barButtonItem_CtrlR.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barButtonItem_CtrlR.Name = "barButtonItem_CtrlR";
            this.barButtonItem_CtrlR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_CtrlR_ItemClick);
            // 
            // barButtonItem__CtrlA
            // 
            this.barButtonItem__CtrlA.Caption = "Them (Ctrl A)";
            this.barButtonItem__CtrlA.Id = 3;
            this.barButtonItem__CtrlA.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barButtonItem__CtrlA.Name = "barButtonItem__CtrlA";
            this.barButtonItem__CtrlA.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__CtrlA_ItemClick);
            // 
            // barButtonItem__CtrlP
            // 
            this.barButtonItem__CtrlP.Caption = "In (Ctrl P)";
            this.barButtonItem__CtrlP.Id = 4;
            this.barButtonItem__CtrlP.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barButtonItem__CtrlP.Name = "barButtonItem__CtrlP";
            this.barButtonItem__CtrlP.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__CtrlP_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1229, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 454);
            this.barDockControlBottom.Size = new System.Drawing.Size(1229, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 425);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1229, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 425);
            // 
            // bbtnFocusSearch
            // 
            this.bbtnFocusSearch.Caption = "F2";
            this.bbtnFocusSearch.Id = 5;
            this.bbtnFocusSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.bbtnFocusSearch.Name = "bbtnFocusSearch";
            this.bbtnFocusSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnFocusSearch_ItemClick);
            // 
            // frmImpMestUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 454);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmImpMestUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmImpMestUpdate";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmImpMestUpdate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__Save;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__CtrlU;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_CtrlR;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__CtrlA;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__CtrlP;
        private DevExpress.XtraBars.BarButtonItem bbtnFocusSearch;

    }
}
