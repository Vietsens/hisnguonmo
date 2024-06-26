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
namespace HIS.Desktop.Plugins.BillTransferAccounting
{
    partial class frmBillTransferAccounting
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
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cboPayForm = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPayFormCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTotalFromNumberOder = new DevExpress.XtraEditors.TextEdit();
            this.spinNumberOrder = new DevExpress.XtraEditors.SpinEdit();
            this.cboAccountBook = new DevExpress.XtraEditors.LookUpEdit();
            this.txtAccountBookCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutNumberOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTotalFromNumberOder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutPayForm = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalFromNumberOder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumberOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNumberOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalFromNumberOder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.cboPayForm);
            this.layoutControl1.Controls.Add(this.txtPayFormCode);
            this.layoutControl1.Controls.Add(this.txtTotalFromNumberOder);
            this.layoutControl1.Controls.Add(this.spinNumberOrder);
            this.layoutControl1.Controls.Add(this.cboAccountBook);
            this.layoutControl1.Controls.Add(this.txtAccountBookCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(63, 188, 312, 437);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(362, 112);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(212, 98);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(131, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSave.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnSave_PreviewKeyDown);
            // 
            // cboPayForm
            // 
            this.cboPayForm.Location = new System.Drawing.Point(158, 74);
            this.cboPayForm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboPayForm.Name = "cboPayForm";
            this.cboPayForm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayForm.Properties.DropDownRows = 3;
            this.cboPayForm.Properties.NullText = "";
            this.cboPayForm.Size = new System.Drawing.Size(185, 20);
            this.cboPayForm.StyleController = this.layoutControl1;
            this.cboPayForm.TabIndex = 100;
            this.cboPayForm.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPayForm_Closed);
            this.cboPayForm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboPayForm_KeyUp);
            // 
            // txtPayFormCode
            // 
            this.txtPayFormCode.Location = new System.Drawing.Point(97, 74);
            this.txtPayFormCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPayFormCode.Name = "txtPayFormCode";
            this.txtPayFormCode.Size = new System.Drawing.Size(61, 20);
            this.txtPayFormCode.StyleController = this.layoutControl1;
            this.txtPayFormCode.TabIndex = 8;
            this.txtPayFormCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayFormCode_KeyDown);
            this.txtPayFormCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnSave_PreviewKeyDown);
            // 
            // txtTotalFromNumberOder
            // 
            this.txtTotalFromNumberOder.Enabled = false;
            this.txtTotalFromNumberOder.Location = new System.Drawing.Point(97, 50);
            this.txtTotalFromNumberOder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTotalFromNumberOder.Name = "txtTotalFromNumberOder";
            this.txtTotalFromNumberOder.Size = new System.Drawing.Size(246, 20);
            this.txtTotalFromNumberOder.StyleController = this.layoutControl1;
            this.txtTotalFromNumberOder.TabIndex = 7;
            // 
            // spinNumberOrder
            // 
            this.spinNumberOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinNumberOrder.Enabled = false;
            this.spinNumberOrder.Location = new System.Drawing.Point(97, 26);
            this.spinNumberOrder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spinNumberOrder.Name = "spinNumberOrder";
            this.spinNumberOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinNumberOrder.Size = new System.Drawing.Size(246, 20);
            this.spinNumberOrder.StyleController = this.layoutControl1;
            this.spinNumberOrder.TabIndex = 6;
            // 
            // cboAccountBook
            // 
            this.cboAccountBook.Location = new System.Drawing.Point(158, 2);
            this.cboAccountBook.Name = "cboAccountBook";
            this.cboAccountBook.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAccountBook.Properties.NullText = "";
            this.cboAccountBook.Size = new System.Drawing.Size(185, 20);
            this.cboAccountBook.StyleController = this.layoutControl1;
            this.cboAccountBook.TabIndex = 5;
            this.cboAccountBook.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboAccountBook_Closed);
            // 
            // txtAccountBookCode
            // 
            this.txtAccountBookCode.Location = new System.Drawing.Point(97, 2);
            this.txtAccountBookCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAccountBookCode.Name = "txtAccountBookCode";
            this.txtAccountBookCode.Size = new System.Drawing.Size(61, 20);
            this.txtAccountBookCode.StyleController = this.layoutControl1;
            this.txtAccountBookCode.TabIndex = 4;
            this.txtAccountBookCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtAccountBookCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutAccountBook,
            this.layoutNumberOrder,
            this.layoutTotalFromNumberOder,
            this.layoutPayForm,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem2,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(345, 122);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutAccountBook
            // 
            this.layoutAccountBook.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutAccountBook.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutAccountBook.Control = this.txtAccountBookCode;
            this.layoutAccountBook.Location = new System.Drawing.Point(0, 0);
            this.layoutAccountBook.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutAccountBook.MinSize = new System.Drawing.Size(135, 24);
            this.layoutAccountBook.Name = "layoutAccountBook";
            this.layoutAccountBook.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutAccountBook.Size = new System.Drawing.Size(158, 24);
            this.layoutAccountBook.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutAccountBook.Text = "Sổ thu chi:";
            this.layoutAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutAccountBook.TextSize = new System.Drawing.Size(90, 13);
            this.layoutAccountBook.TextToControlDistance = 5;
            // 
            // layoutNumberOrder
            // 
            this.layoutNumberOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutNumberOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutNumberOrder.Control = this.spinNumberOrder;
            this.layoutNumberOrder.Location = new System.Drawing.Point(0, 24);
            this.layoutNumberOrder.Name = "layoutNumberOrder";
            this.layoutNumberOrder.Size = new System.Drawing.Size(345, 24);
            this.layoutNumberOrder.Text = "Số chứng từ:";
            this.layoutNumberOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutNumberOrder.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutNumberOrder.TextSize = new System.Drawing.Size(90, 13);
            this.layoutNumberOrder.TextToControlDistance = 5;
            // 
            // layoutTotalFromNumberOder
            // 
            this.layoutTotalFromNumberOder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTotalFromNumberOder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTotalFromNumberOder.Control = this.txtTotalFromNumberOder;
            this.layoutTotalFromNumberOder.Location = new System.Drawing.Point(0, 48);
            this.layoutTotalFromNumberOder.Name = "layoutTotalFromNumberOder";
            this.layoutTotalFromNumberOder.Size = new System.Drawing.Size(345, 24);
            this.layoutTotalFromNumberOder.Text = "Tổng/Từ/HT:";
            this.layoutTotalFromNumberOder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTotalFromNumberOder.TextSize = new System.Drawing.Size(90, 13);
            this.layoutTotalFromNumberOder.TextToControlDistance = 5;
            // 
            // layoutPayForm
            // 
            this.layoutPayForm.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutPayForm.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPayForm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPayForm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPayForm.Control = this.txtPayFormCode;
            this.layoutPayForm.Location = new System.Drawing.Point(0, 72);
            this.layoutPayForm.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutPayForm.MinSize = new System.Drawing.Size(135, 24);
            this.layoutPayForm.Name = "layoutPayForm";
            this.layoutPayForm.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutPayForm.Size = new System.Drawing.Size(158, 24);
            this.layoutPayForm.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutPayForm.Text = "Hình thức:";
            this.layoutPayForm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPayForm.TextSize = new System.Drawing.Size(90, 13);
            this.layoutPayForm.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.cboPayForm;
            this.layoutControlItem6.Location = new System.Drawing.Point(158, 72);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(187, 24);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnSave;
            this.layoutControlItem7.Location = new System.Drawing.Point(210, 96);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(135, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboAccountBook;
            this.layoutControlItem2.Location = new System.Drawing.Point(158, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(187, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(210, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider2
            // 
            this.dxValidationProvider2.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider2_ValidationFailed);
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
            this.bbtnRCSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCSave
            // 
            this.bbtnRCSave.Caption = "Lưu (Ctrl S)";
            this.bbtnRCSave.Id = 0;
            this.bbtnRCSave.Name = "bbtnRCSave";
            this.bbtnRCSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(362, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 141);
            this.barDockControlBottom.Size = new System.Drawing.Size(362, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 112);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(362, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 112);
            // 
            // frmBillTransferAccounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 141);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBillTransferAccounting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết chuyển";
            this.Load += new System.EventHandler(this.frmBillTransferAccounting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboPayForm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPayFormCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalFromNumberOder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinNumberOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAccountBook.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAccountBookCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNumberOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTotalFromNumberOder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPayForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal DevExpress.XtraEditors.LookUpEdit cboPayForm;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.SimpleButton btnSave;
        //private DevExpress.XtraEditors.LookUpEdit cboPayForm;
        internal DevExpress.XtraEditors.TextEdit txtPayFormCode;
        private DevExpress.XtraEditors.TextEdit txtTotalFromNumberOder;
        internal DevExpress.XtraEditors.SpinEdit spinNumberOrder;
        internal DevExpress.XtraEditors.LookUpEdit cboAccountBook;
        internal DevExpress.XtraEditors.TextEdit txtAccountBookCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutAccountBook;
        internal DevExpress.XtraLayout.LayoutControlItem layoutNumberOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutTotalFromNumberOder;
        private DevExpress.XtraLayout.LayoutControlItem layoutPayForm;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
