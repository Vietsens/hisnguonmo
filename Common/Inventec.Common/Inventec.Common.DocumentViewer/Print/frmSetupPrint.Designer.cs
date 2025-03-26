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
namespace Inventec.Common.DocumentViewer.Print
{
    partial class frmSetupPrint
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
            this.layoutControlRoot = new DevExpress.XtraLayout.LayoutControl();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlPageRange = new DevExpress.XtraLayout.LayoutControl();
            this.chkPages = new DevExpress.XtraEditors.CheckEdit();
            this.numericUpDownFromPage = new DevExpress.XtraEditors.SpinEdit();
            this.numericUpDownToPage = new DevExpress.XtraEditors.SpinEdit();
            this.chkCurrentPage = new DevExpress.XtraEditors.CheckEdit();
            this.chkAllPages = new DevExpress.XtraEditors.CheckEdit();
            this.lcgPageRange = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboPrinters = new DevExpress.XtraEditors.ComboBoxEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPrinters = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlPageRange)).BeginInit();
            this.layoutControlPageRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPages.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFromPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCurrentPage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllPages.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPageRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinters.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.Controls.Add(this.btnPrint);
            this.layoutControlRoot.Controls.Add(this.layoutControlPageRange);
            this.layoutControlRoot.Controls.Add(this.layoutControl1);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(338, 63, 250, 350);
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(214, 161);
            this.layoutControlRoot.TabIndex = 0;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(109, 137);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(103, 22);
            this.btnPrint.StyleController = this.layoutControlRoot;
            this.btnPrint.TabIndex = 47;
            this.btnPrint.Text = "In";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // layoutControlPageRange
            // 
            this.layoutControlPageRange.Controls.Add(this.chkPages);
            this.layoutControlPageRange.Controls.Add(this.numericUpDownFromPage);
            this.layoutControlPageRange.Controls.Add(this.numericUpDownToPage);
            this.layoutControlPageRange.Controls.Add(this.chkCurrentPage);
            this.layoutControlPageRange.Controls.Add(this.chkAllPages);
            this.layoutControlPageRange.Location = new System.Drawing.Point(2, 48);
            this.layoutControlPageRange.Name = "layoutControlPageRange";
            this.layoutControlPageRange.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControlPageRange.Root = this.lcgPageRange;
            this.layoutControlPageRange.Size = new System.Drawing.Size(210, 85);
            this.layoutControlPageRange.TabIndex = 46;
            this.layoutControlPageRange.Text = "layoutControl5";
            // 
            // chkPages
            // 
            this.chkPages.Location = new System.Drawing.Point(12, 53);
            this.chkPages.Name = "chkPages";
            this.chkPages.Properties.Caption = "Từ:";
            this.chkPages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkPages.Properties.RadioGroupIndex = 1;
            this.chkPages.Size = new System.Drawing.Size(47, 19);
            this.chkPages.StyleController = this.layoutControlPageRange;
            this.chkPages.TabIndex = 13;
            this.chkPages.TabStop = false;
            this.chkPages.CheckedChanged += new System.EventHandler(this.chkPages_CheckedChanged);
            // 
            // numericUpDownFromPage
            // 
            this.numericUpDownFromPage.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownFromPage.Location = new System.Drawing.Point(63, 53);
            this.numericUpDownFromPage.Name = "numericUpDownFromPage";
            this.numericUpDownFromPage.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.numericUpDownFromPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numericUpDownFromPage.Properties.IsFloatValue = false;
            this.numericUpDownFromPage.Properties.Mask.EditMask = "N00";
            this.numericUpDownFromPage.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownFromPage.StyleController = this.layoutControlPageRange;
            this.numericUpDownFromPage.TabIndex = 14;
            // 
            // numericUpDownToPage
            // 
            this.numericUpDownToPage.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericUpDownToPage.Location = new System.Drawing.Point(132, 53);
            this.numericUpDownToPage.Name = "numericUpDownToPage";
            this.numericUpDownToPage.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.numericUpDownToPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.numericUpDownToPage.Properties.IsFloatValue = false;
            this.numericUpDownToPage.Properties.Mask.EditMask = "N00";
            this.numericUpDownToPage.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownToPage.StyleController = this.layoutControlPageRange;
            this.numericUpDownToPage.TabIndex = 15;
            // 
            // chkCurrentPage
            // 
            this.chkCurrentPage.Location = new System.Drawing.Point(107, 30);
            this.chkCurrentPage.Name = "chkCurrentPage";
            this.chkCurrentPage.Properties.Caption = "Trang hiện tại";
            this.chkCurrentPage.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkCurrentPage.Properties.RadioGroupIndex = 1;
            this.chkCurrentPage.Size = new System.Drawing.Size(91, 19);
            this.chkCurrentPage.StyleController = this.layoutControlPageRange;
            this.chkCurrentPage.TabIndex = 12;
            this.chkCurrentPage.TabStop = false;
            // 
            // chkAllPages
            // 
            this.chkAllPages.Location = new System.Drawing.Point(12, 30);
            this.chkAllPages.Name = "chkAllPages";
            this.chkAllPages.Properties.Caption = "Tất cả";
            this.chkAllPages.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkAllPages.Properties.RadioGroupIndex = 1;
            this.chkAllPages.Size = new System.Drawing.Size(91, 19);
            this.chkAllPages.StyleController = this.layoutControlPageRange;
            this.chkAllPages.TabIndex = 10;
            this.chkAllPages.TabStop = false;
            // 
            // lcgPageRange
            // 
            this.lcgPageRange.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgPageRange.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem16,
            this.layoutControlItem28,
            this.layoutControlItem29,
            this.layoutControlItem30,
            this.layoutControlItem18});
            this.lcgPageRange.Location = new System.Drawing.Point(0, 0);
            this.lcgPageRange.Name = "lcgPageRange";
            this.lcgPageRange.Size = new System.Drawing.Size(210, 85);
            this.lcgPageRange.Text = "Khoảng trang in";
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.Control = this.chkAllPages;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(95, 23);
            this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem16.TextVisible = false;
            // 
            // layoutControlItem28
            // 
            this.layoutControlItem28.Control = this.numericUpDownFromPage;
            this.layoutControlItem28.Location = new System.Drawing.Point(51, 23);
            this.layoutControlItem28.Name = "layoutControlItem28";
            this.layoutControlItem28.Size = new System.Drawing.Size(69, 24);
            this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem28.TextVisible = false;
            // 
            // layoutControlItem29
            // 
            this.layoutControlItem29.Control = this.numericUpDownToPage;
            this.layoutControlItem29.Location = new System.Drawing.Point(120, 23);
            this.layoutControlItem29.Name = "layoutControlItem29";
            this.layoutControlItem29.Size = new System.Drawing.Size(70, 24);
            this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem29.TextVisible = false;
            // 
            // layoutControlItem30
            // 
            this.layoutControlItem30.Control = this.chkPages;
            this.layoutControlItem30.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem30.Name = "layoutControlItem30";
            this.layoutControlItem30.Size = new System.Drawing.Size(51, 24);
            this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem30.TextVisible = false;
            // 
            // layoutControlItem18
            // 
            this.layoutControlItem18.Control = this.chkCurrentPage;
            this.layoutControlItem18.Location = new System.Drawing.Point(95, 0);
            this.layoutControlItem18.Name = "layoutControlItem18";
            this.layoutControlItem18.Size = new System.Drawing.Size(95, 23);
            this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem18.TextVisible = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboPrinters);
            this.layoutControl1.Location = new System.Drawing.Point(2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(210, 42);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboPrinters
            // 
            this.cboPrinters.Location = new System.Drawing.Point(61, 16);
            this.cboPrinters.Name = "cboPrinters";
            this.cboPrinters.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPrinters.Properties.PopupSizeable = true;
            this.cboPrinters.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboPrinters.Size = new System.Drawing.Size(143, 20);
            this.cboPrinters.StyleController = this.layoutControl1;
            this.cboPrinters.TabIndex = 7;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPrinters,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
            this.layoutControlGroup1.Size = new System.Drawing.Size(210, 42);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPrinters
            // 
            this.lciPrinters.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrinters.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrinters.Control = this.cboPrinters;
            this.lciPrinters.Location = new System.Drawing.Point(0, 10);
            this.lciPrinters.Name = "lciPrinters";
            this.lciPrinters.Size = new System.Drawing.Size(202, 24);
            this.lciPrinters.Text = "Máy in:";
            this.lciPrinters.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPrinters.TextSize = new System.Drawing.Size(50, 20);
            this.lciPrinters.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(202, 10);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(214, 161);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(214, 46);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControlPageRange;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 46);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(214, 89);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 152);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(107, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnPrint;
            this.layoutControlItem3.Location = new System.Drawing.Point(107, 135);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(107, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmSetupPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 161);
            this.Controls.Add(this.layoutControlRoot);
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(230, 200);
            this.Name = "frmSetupPrint";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thiết lập in";
            this.Load += new System.EventHandler(this.frmSetupPrint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlPageRange)).EndInit();
            this.layoutControlPageRange.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPages.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFromPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownToPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCurrentPage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllPages.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgPageRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboPrinters.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrinters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControlPageRange;
        private DevExpress.XtraEditors.CheckEdit chkPages;
        private DevExpress.XtraEditors.SpinEdit numericUpDownFromPage;
        private DevExpress.XtraEditors.SpinEdit numericUpDownToPage;
        private DevExpress.XtraEditors.CheckEdit chkCurrentPage;
        private DevExpress.XtraEditors.CheckEdit chkAllPages;
        private DevExpress.XtraLayout.LayoutControlGroup lcgPageRange;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem28;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem29;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem30;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem18;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.ComboBoxEdit cboPrinters;
        private DevExpress.XtraLayout.LayoutControlItem lciPrinters;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
