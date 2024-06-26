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
namespace HIS.Desktop.Plugins.ExpMestDepaCreate.Print
{
    partial class frmPrintByCondition
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
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.checkChemistry = new DevExpress.XtraEditors.CheckEdit();
            this.checkMaterial = new DevExpress.XtraEditors.CheckEdit();
            this.checkMedicine = new DevExpress.XtraEditors.CheckEdit();
            this.txtTitlePrint = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutTitlePrint = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutCondition = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkChemistry.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMaterial.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMedicine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitlePrint.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTitlePrint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.checkChemistry);
            this.layoutControl1.Controls.Add(this.checkMaterial);
            this.layoutControl1.Controls.Add(this.checkMedicine);
            this.layoutControl1.Controls.Add(this.txtTitlePrint);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 67);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(538, 50);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // checkChemistry
            // 
            this.checkChemistry.Location = new System.Drawing.Point(518, 26);
            this.checkChemistry.Name = "checkChemistry";
            this.checkChemistry.Properties.Caption = "Hóa chất";
            this.checkChemistry.Size = new System.Drawing.Size(123, 19);
            this.checkChemistry.StyleController = this.layoutControl1;
            this.checkChemistry.TabIndex = 7;
            this.checkChemistry.CheckedChanged += new System.EventHandler(this.checkChemistry_CheckedChanged);
            // 
            // checkMaterial
            // 
            this.checkMaterial.Location = new System.Drawing.Point(304, 26);
            this.checkMaterial.Name = "checkMaterial";
            this.checkMaterial.Properties.Caption = "Vật tư";
            this.checkMaterial.Size = new System.Drawing.Size(210, 19);
            this.checkMaterial.StyleController = this.layoutControl1;
            this.checkMaterial.TabIndex = 6;
            this.checkMaterial.CheckedChanged += new System.EventHandler(this.checkMaterial_CheckedChanged);
            // 
            // checkMedicine
            // 
            this.checkMedicine.Location = new System.Drawing.Point(97, 26);
            this.checkMedicine.Name = "checkMedicine";
            this.checkMedicine.Properties.Caption = "Thuốc";
            this.checkMedicine.Size = new System.Drawing.Size(203, 19);
            this.checkMedicine.StyleController = this.layoutControl1;
            this.checkMedicine.TabIndex = 5;
            this.checkMedicine.CheckedChanged += new System.EventHandler(this.checkMedicine_CheckedChanged);
            // 
            // txtTitlePrint
            // 
            this.txtTitlePrint.Location = new System.Drawing.Point(97, 2);
            this.txtTitlePrint.Name = "txtTitlePrint";
            this.txtTitlePrint.Size = new System.Drawing.Size(544, 20);
            this.txtTitlePrint.StyleController = this.layoutControl1;
            this.txtTitlePrint.TabIndex = 4;
            this.txtTitlePrint.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTitlePrint_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutTitlePrint,
            this.layoutCondition,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(643, 74);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutTitlePrint
            // 
            this.layoutTitlePrint.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutTitlePrint.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutTitlePrint.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTitlePrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTitlePrint.Control = this.txtTitlePrint;
            this.layoutTitlePrint.Location = new System.Drawing.Point(0, 0);
            this.layoutTitlePrint.Name = "layoutTitlePrint";
            this.layoutTitlePrint.Size = new System.Drawing.Size(643, 24);
            this.layoutTitlePrint.Text = "Tiêu đề in:";
            this.layoutTitlePrint.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTitlePrint.TextSize = new System.Drawing.Size(90, 20);
            this.layoutTitlePrint.TextToControlDistance = 5;
            // 
            // layoutCondition
            // 
            this.layoutCondition.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutCondition.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutCondition.Control = this.checkMedicine;
            this.layoutCondition.Location = new System.Drawing.Point(0, 24);
            this.layoutCondition.Name = "layoutCondition";
            this.layoutCondition.Size = new System.Drawing.Size(302, 24);
            this.layoutCondition.Text = "Điều kiện:";
            this.layoutCondition.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutCondition.TextSize = new System.Drawing.Size(90, 20);
            this.layoutCondition.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.checkMaterial;
            this.layoutControlItem3.Location = new System.Drawing.Point(302, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(214, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.checkChemistry;
            this.layoutControlItem4.Location = new System.Drawing.Point(516, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(127, 24);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnPrint;
            this.layoutControlItem5.Location = new System.Drawing.Point(536, 48);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(107, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(536, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
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
            this.bbtnRCPrint});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCPrint)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCPrint
            // 
            this.bbtnRCPrint.Caption = "In (Ctrl P)";
            this.bbtnRCPrint.Id = 0;
            this.bbtnRCPrint.Name = "bbtnRCPrint";
            this.bbtnRCPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(660, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 96);
            this.barDockControlBottom.Size = new System.Drawing.Size(660, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 67);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(660, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 67);
            // 
            // frmPrintByCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 96);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmPrintByCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "In phiếu xuất hoa phí khoa phòng theo điều kiện";
            this.Load += new System.EventHandler(this.frmPrintByCondition_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkChemistry.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMaterial.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMedicine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitlePrint.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTitlePrint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.CheckEdit checkChemistry;
        private DevExpress.XtraEditors.CheckEdit checkMaterial;
        private DevExpress.XtraEditors.CheckEdit checkMedicine;
        private DevExpress.XtraEditors.TextEdit txtTitlePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutTitlePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutCondition;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnRCPrint;
    }
}
