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

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    partial class frmSettingSavePath
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettingSavePath));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnPathCollinearXml = new DevExpress.XtraEditors.SimpleButton();
            this.txtPathCollinearXml = new DevExpress.XtraEditors.TextEdit();
            this.btnPathXmlGDYK = new DevExpress.XtraEditors.SimpleButton();
            this.btnPathXml = new DevExpress.XtraEditors.SimpleButton();
            this.txtPathXml = new DevExpress.XtraEditors.TextEdit();
            this.txtPathXmlGDYK = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPathSaveXml = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPathSaveXml12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPathCollinearXml = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPathCollinearXml.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPathXml.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPathXmlGDYK.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathSaveXml)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathSaveXml12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathCollinearXml)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(604, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 84);
            this.barDockControlBottom.Size = new System.Drawing.Size(604, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 84);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(604, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 84);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPathCollinearXml);
            this.layoutControl1.Controls.Add(this.txtPathCollinearXml);
            this.layoutControl1.Controls.Add(this.btnPathXmlGDYK);
            this.layoutControl1.Controls.Add(this.btnPathXml);
            this.layoutControl1.Controls.Add(this.txtPathXml);
            this.layoutControl1.Controls.Add(this.txtPathXmlGDYK);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(604, 84);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPathCollinearXml
            // 
            this.btnPathCollinearXml.Image = global::HIS.Desktop.Plugins.ExportXmlQD130.Properties.Resources.Folder;
            this.btnPathCollinearXml.Location = new System.Drawing.Point(577, 30);
            this.btnPathCollinearXml.Name = "btnPathCollinearXml";
            this.btnPathCollinearXml.Size = new System.Drawing.Size(25, 24);
            this.btnPathCollinearXml.StyleController = this.layoutControl1;
            this.btnPathCollinearXml.TabIndex = 16;
            this.btnPathCollinearXml.Click += new System.EventHandler(this.btnPathCollinearXml_Click);
            // 
            // txtPathCollinearXml
            // 
            this.txtPathCollinearXml.Location = new System.Drawing.Point(377, 30);
            this.txtPathCollinearXml.MenuManager = this.barManager1;
            this.txtPathCollinearXml.Name = "txtPathCollinearXml";
            this.txtPathCollinearXml.Size = new System.Drawing.Size(196, 20);
            this.txtPathCollinearXml.StyleController = this.layoutControl1;
            this.txtPathCollinearXml.TabIndex = 15;
            // 
            // btnPathXmlGDYK
            // 
            this.btnPathXmlGDYK.Image = ((System.Drawing.Image)(resources.GetObject("btnPathXmlGDYK.Image")));
            this.btnPathXmlGDYK.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPathXmlGDYK.Location = new System.Drawing.Point(577, 58);
            this.btnPathXmlGDYK.Name = "btnPathXmlGDYK";
            this.btnPathXmlGDYK.Size = new System.Drawing.Size(25, 24);
            this.btnPathXmlGDYK.StyleController = this.layoutControl1;
            this.btnPathXmlGDYK.TabIndex = 14;
            this.btnPathXmlGDYK.Text = "simpleButton2";
            this.btnPathXmlGDYK.Click += new System.EventHandler(this.btnPathXmlGDYK_Click);
            // 
            // btnPathXml
            // 
            this.btnPathXml.Image = ((System.Drawing.Image)(resources.GetObject("btnPathXml.Image")));
            this.btnPathXml.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPathXml.Location = new System.Drawing.Point(577, 2);
            this.btnPathXml.Name = "btnPathXml";
            this.btnPathXml.Size = new System.Drawing.Size(25, 24);
            this.btnPathXml.StyleController = this.layoutControl1;
            this.btnPathXml.TabIndex = 13;
            this.btnPathXml.Text = "simpleButton1";
            this.btnPathXml.Click += new System.EventHandler(this.btnPathXml_Click);
            // 
            // txtPathXml
            // 
            this.txtPathXml.Location = new System.Drawing.Point(377, 2);
            this.txtPathXml.MenuManager = this.barManager1;
            this.txtPathXml.Name = "txtPathXml";
            this.txtPathXml.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtPathXml.Size = new System.Drawing.Size(196, 20);
            this.txtPathXml.StyleController = this.layoutControl1;
            this.txtPathXml.TabIndex = 11;
            // 
            // txtPathXmlGDYK
            // 
            this.txtPathXmlGDYK.Location = new System.Drawing.Point(377, 58);
            this.txtPathXmlGDYK.MenuManager = this.barManager1;
            this.txtPathXmlGDYK.Name = "txtPathXmlGDYK";
            this.txtPathXmlGDYK.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtPathXmlGDYK.Size = new System.Drawing.Size(196, 20);
            this.txtPathXmlGDYK.StyleController = this.layoutControl1;
            this.txtPathXmlGDYK.TabIndex = 12;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPathSaveXml,
            this.lciPathSaveXml12,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.lciPathCollinearXml,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Size = new System.Drawing.Size(604, 84);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPathSaveXml
            // 
            this.lciPathSaveXml.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPathSaveXml.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPathSaveXml.Control = this.txtPathXml;
            this.lciPathSaveXml.Location = new System.Drawing.Point(0, 0);
            this.lciPathSaveXml.Name = "lciPathSaveXml";
            this.lciPathSaveXml.OptionsToolTip.ToolTip = "Thư mục lưu file từ XML 1 đến XML 11";
            this.lciPathSaveXml.Size = new System.Drawing.Size(575, 28);
            this.lciPathSaveXml.Text = "Thư mục lưu XML (không bao gồm giám định y khoa):";
            this.lciPathSaveXml.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPathSaveXml.TextSize = new System.Drawing.Size(370, 20);
            this.lciPathSaveXml.TextToControlDistance = 5;
            // 
            // lciPathSaveXml12
            // 
            this.lciPathSaveXml12.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPathSaveXml12.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPathSaveXml12.Control = this.txtPathXmlGDYK;
            this.lciPathSaveXml12.Location = new System.Drawing.Point(0, 56);
            this.lciPathSaveXml12.Name = "lciPathSaveXml12";
            this.lciPathSaveXml12.OptionsToolTip.ToolTip = "Thư mục lưu giám định y khoa (XML 12)";
            this.lciPathSaveXml12.Size = new System.Drawing.Size(575, 28);
            this.lciPathSaveXml12.Text = "Thư mục lưu gdyk:";
            this.lciPathSaveXml12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPathSaveXml12.TextSize = new System.Drawing.Size(370, 20);
            this.lciPathSaveXml12.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnPathXml;
            this.layoutControlItem1.Location = new System.Drawing.Point(575, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(29, 28);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnPathXmlGDYK;
            this.layoutControlItem2.Location = new System.Drawing.Point(575, 56);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(29, 28);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciPathCollinearXml
            // 
            this.lciPathCollinearXml.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPathCollinearXml.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPathCollinearXml.Control = this.txtPathCollinearXml;
            this.lciPathCollinearXml.Location = new System.Drawing.Point(0, 28);
            this.lciPathCollinearXml.Name = "lciPathCollinearXml";
            this.lciPathCollinearXml.OptionsToolTip.ToolTip = "Thư mục lưu file từ XML 1 đến XML 11";
            this.lciPathCollinearXml.Size = new System.Drawing.Size(575, 28);
            this.lciPathCollinearXml.Text = "Thư mục lưu XML thông tuyến (không bao gồm giám định y khoa):";
            this.lciPathCollinearXml.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPathCollinearXml.TextSize = new System.Drawing.Size(370, 20);
            this.lciPathCollinearXml.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnPathCollinearXml;
            this.layoutControlItem4.Location = new System.Drawing.Point(575, 28);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(29, 28);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // frmSettingSavePath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 84);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmSettingSavePath";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập thư mục lưu";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSettingSavePath_FormClosed);
            this.Load += new System.EventHandler(this.frmSettingSavePath_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPathCollinearXml.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPathXml.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPathXmlGDYK.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathSaveXml)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathSaveXml12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPathCollinearXml)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciPathSaveXml;
        private DevExpress.XtraLayout.LayoutControlItem lciPathSaveXml12;
        private DevExpress.XtraEditors.TextEdit txtPathXml;
        private DevExpress.XtraEditors.TextEdit txtPathXmlGDYK;
        private DevExpress.XtraEditors.SimpleButton btnPathXmlGDYK;
        private DevExpress.XtraEditors.SimpleButton btnPathXml;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnPathCollinearXml;
        private DevExpress.XtraEditors.TextEdit txtPathCollinearXml;
        private DevExpress.XtraLayout.LayoutControlItem lciPathCollinearXml;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
