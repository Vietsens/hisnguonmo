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
namespace HIS.Desktop.Plugins.RequestForUsingAccountBook
{
    partial class frmRequestForUsingAccountBook
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboPayform = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblNumOrder = new DevExpress.XtraEditors.LabelControl();
            this.lblAccountBook = new DevExpress.XtraEditors.LabelControl();
            this.btnCancelUsing = new DevExpress.XtraEditors.SimpleButton();
            this.btnRequest = new DevExpress.XtraEditors.SimpleButton();
            this.cboCashierUser = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciCashierUser = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAccountBook = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNumOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPayform = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.timerCheckAuthority = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboPayform.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierUser.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCashierUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPayform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboPayform);
            this.layoutControl1.Controls.Add(this.lblNumOrder);
            this.layoutControl1.Controls.Add(this.lblAccountBook);
            this.layoutControl1.Controls.Add(this.btnCancelUsing);
            this.layoutControl1.Controls.Add(this.btnRequest);
            this.layoutControl1.Controls.Add(this.cboCashierUser);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(168, 320, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(550, 87);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboPayform
            // 
            this.cboPayform.Location = new System.Drawing.Point(127, 28);
            this.cboPayform.Name = "cboPayform";
            this.cboPayform.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboPayform.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPayform.Properties.NullText = "";
            this.cboPayform.Properties.View = this.gridView1;
            this.cboPayform.Size = new System.Drawing.Size(146, 20);
            this.cboPayform.StyleController = this.layoutControl1;
            this.cboPayform.TabIndex = 9;
            this.cboPayform.EditValueChanged += new System.EventHandler(this.cboPayform_EditValueChanged);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // lblNumOrder
            // 
            this.lblNumOrder.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblNumOrder.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNumOrder.Location = new System.Drawing.Point(402, 52);
            this.lblNumOrder.Name = "lblNumOrder";
            this.lblNumOrder.Size = new System.Drawing.Size(146, 20);
            this.lblNumOrder.StyleController = this.layoutControl1;
            this.lblNumOrder.TabIndex = 8;
            // 
            // lblAccountBook
            // 
            this.lblAccountBook.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAccountBook.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAccountBook.Location = new System.Drawing.Point(127, 52);
            this.lblAccountBook.Name = "lblAccountBook";
            this.lblAccountBook.Size = new System.Drawing.Size(146, 20);
            this.lblAccountBook.StyleController = this.layoutControl1;
            this.lblAccountBook.TabIndex = 7;
            // 
            // btnCancelUsing
            // 
            this.btnCancelUsing.Location = new System.Drawing.Point(414, 2);
            this.btnCancelUsing.Name = "btnCancelUsing";
            this.btnCancelUsing.Size = new System.Drawing.Size(134, 22);
            this.btnCancelUsing.StyleController = this.layoutControl1;
            this.btnCancelUsing.TabIndex = 6;
            this.btnCancelUsing.Text = "Hủy yêu cầu";
            this.btnCancelUsing.Click += new System.EventHandler(this.btnCancelUsing_Click);
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(277, 2);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(133, 22);
            this.btnRequest.StyleController = this.layoutControl1;
            this.btnRequest.TabIndex = 5;
            this.btnRequest.Text = "Yêu cầu";
            this.btnRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // cboCashierUser
            // 
            this.cboCashierUser.Location = new System.Drawing.Point(127, 2);
            this.cboCashierUser.Name = "cboCashierUser";
            this.cboCashierUser.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboCashierUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboCashierUser.Properties.NullText = "";
            this.cboCashierUser.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
            this.cboCashierUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboCashierUser.Properties.View = this.gridLookUpEdit1View;
            this.cboCashierUser.Size = new System.Drawing.Size(146, 20);
            this.cboCashierUser.StyleController = this.layoutControl1;
            this.cboCashierUser.TabIndex = 4;
            this.cboCashierUser.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCashierUser_Closed);
            this.cboCashierUser.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCashierUser_ButtonClick);
            this.cboCashierUser.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboCashierUser_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCashierUser,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.lciAccountBook,
            this.lciNumOrder,
            this.emptySpaceItem1,
            this.lciPayform,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(550, 87);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciCashierUser
            // 
            this.lciCashierUser.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCashierUser.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCashierUser.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCashierUser.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCashierUser.Control = this.cboCashierUser;
            this.lciCashierUser.Location = new System.Drawing.Point(0, 0);
            this.lciCashierUser.Name = "lciCashierUser";
            this.lciCashierUser.Size = new System.Drawing.Size(275, 26);
            this.lciCashierUser.Text = "Thu ngân:";
            this.lciCashierUser.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCashierUser.TextSize = new System.Drawing.Size(120, 20);
            this.lciCashierUser.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnRequest;
            this.layoutControlItem2.Location = new System.Drawing.Point(275, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(137, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnCancelUsing;
            this.layoutControlItem3.Location = new System.Drawing.Point(412, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(138, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciAccountBook
            // 
            this.lciAccountBook.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAccountBook.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAccountBook.Control = this.lblAccountBook;
            this.lciAccountBook.Location = new System.Drawing.Point(0, 50);
            this.lciAccountBook.Name = "lciAccountBook";
            this.lciAccountBook.Size = new System.Drawing.Size(275, 24);
            this.lciAccountBook.Text = "Sổ biên lai/Hóa đơn:";
            this.lciAccountBook.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAccountBook.TextSize = new System.Drawing.Size(120, 20);
            this.lciAccountBook.TextToControlDistance = 5;
            // 
            // lciNumOrder
            // 
            this.lciNumOrder.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNumOrder.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNumOrder.Control = this.lblNumOrder;
            this.lciNumOrder.Location = new System.Drawing.Point(275, 50);
            this.lciNumOrder.Name = "lciNumOrder";
            this.lciNumOrder.Size = new System.Drawing.Size(275, 24);
            this.lciNumOrder.Text = "Số:";
            this.lciNumOrder.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNumOrder.TextSize = new System.Drawing.Size(120, 20);
            this.lciNumOrder.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 74);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(550, 13);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPayform
            // 
            this.lciPayform.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPayform.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPayform.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPayform.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPayform.Control = this.cboPayform;
            this.lciPayform.Location = new System.Drawing.Point(0, 26);
            this.lciPayform.Name = "lciPayform";
            this.lciPayform.Size = new System.Drawing.Size(275, 24);
            this.lciPayform.Text = "Hình thức:";
            this.lciPayform.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPayform.TextSize = new System.Drawing.Size(120, 20);
            this.lciPayform.TextToControlDistance = 5;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(275, 26);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(275, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // timerCheckAuthority
            // 
            //this.timerCheckAuthority.Tick += new System.EventHandler(this.timerCheckAuthority_Tick);
            // 
            // frmRequestForUsingAccountBook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 87);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmRequestForUsingAccountBook";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Yêu cầu sử dụng sổ biên lai/hóa đơn";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRequestForUsingAccountBook_FormClosing);
            this.Load += new System.EventHandler(this.frmRequestForUsingAccountBook_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboPayform.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCashierUser.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCashierUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAccountBook)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNumOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPayform)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblNumOrder;
        private DevExpress.XtraEditors.LabelControl lblAccountBook;
        private DevExpress.XtraEditors.SimpleButton btnCancelUsing;
        private DevExpress.XtraEditors.SimpleButton btnRequest;
        private DevExpress.XtraEditors.GridLookUpEdit cboCashierUser;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciCashierUser;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciAccountBook;
        private DevExpress.XtraLayout.LayoutControlItem lciNumOrder;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private System.Windows.Forms.Timer timerCheckAuthority;
        private DevExpress.XtraEditors.GridLookUpEdit cboPayform;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciPayform;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
