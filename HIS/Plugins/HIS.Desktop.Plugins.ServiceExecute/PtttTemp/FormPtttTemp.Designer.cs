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
namespace HIS.Desktop.Plugins.ServiceExecute.PtttTemp
{
    partial class FormPtttTemp
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
			this.chkPublicDepartment = new DevExpress.XtraEditors.CheckEdit();
			this.chkPublic = new DevExpress.XtraEditors.CheckEdit();
			this.txtPtttTempName = new DevExpress.XtraEditors.TextEdit();
			this.txtPtttTempCode = new DevExpress.XtraEditors.TextEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciPtttTempCode = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPtttTempName = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
			this.barManager1 = new DevExpress.XtraBars.BarManager();
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chkPublicDepartment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPublic.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPtttTempName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPtttTempCode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPtttTempCode)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPtttTempName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.btnSave);
			this.layoutControl1.Controls.Add(this.chkPublicDepartment);
			this.layoutControl1.Controls.Add(this.chkPublic);
			this.layoutControl1.Controls.Add(this.txtPtttTempName);
			this.layoutControl1.Controls.Add(this.txtPtttTempCode);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 29);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(447, 86);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(324, 74);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(104, 22);
			this.btnSave.StyleController = this.layoutControl1;
			this.btnSave.TabIndex = 8;
			this.btnSave.Text = "Lưu (Ctrl S)";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// chkPublicDepartment
			// 
			this.chkPublicDepartment.Location = new System.Drawing.Point(218, 50);
			this.chkPublicDepartment.Name = "chkPublicDepartment";
			this.chkPublicDepartment.Properties.Caption = "Công khai trong khoa";
			this.chkPublicDepartment.Size = new System.Drawing.Size(210, 19);
			this.chkPublicDepartment.StyleController = this.layoutControl1;
			this.chkPublicDepartment.TabIndex = 7;
			this.chkPublicDepartment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chkPublicDepartment_KeyDown);
			// 
			// chkPublic
			// 
			this.chkPublic.Location = new System.Drawing.Point(67, 50);
			this.chkPublic.Name = "chkPublic";
			this.chkPublic.Properties.Caption = "Công khai toàn viện";
			this.chkPublic.Size = new System.Drawing.Size(147, 19);
			this.chkPublic.StyleController = this.layoutControl1;
			this.chkPublic.TabIndex = 6;
			this.chkPublic.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPublic_PreviewKeyDown);
			// 
			// txtPtttTempName
			// 
			this.txtPtttTempName.Location = new System.Drawing.Point(67, 26);
			this.txtPtttTempName.Name = "txtPtttTempName";
			this.txtPtttTempName.Size = new System.Drawing.Size(361, 20);
			this.txtPtttTempName.StyleController = this.layoutControl1;
			this.txtPtttTempName.TabIndex = 5;
			this.txtPtttTempName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttTempName_PreviewKeyDown);
			// 
			// txtPtttTempCode
			// 
			this.txtPtttTempCode.Location = new System.Drawing.Point(67, 2);
			this.txtPtttTempCode.Name = "txtPtttTempCode";
			this.txtPtttTempCode.Size = new System.Drawing.Size(361, 20);
			this.txtPtttTempCode.StyleController = this.layoutControl1;
			this.txtPtttTempCode.TabIndex = 4;
			this.txtPtttTempCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttTempCode_PreviewKeyDown);
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPtttTempCode,
            this.lciPtttTempName,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem5});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(430, 98);
			this.layoutControlGroup1.TextVisible = false;
			// 
			// lciPtttTempCode
			// 
			this.lciPtttTempCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
			this.lciPtttTempCode.AppearanceItemCaption.Options.UseForeColor = true;
			this.lciPtttTempCode.AppearanceItemCaption.Options.UseTextOptions = true;
			this.lciPtttTempCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.lciPtttTempCode.Control = this.txtPtttTempCode;
			this.lciPtttTempCode.Location = new System.Drawing.Point(0, 0);
			this.lciPtttTempCode.Name = "lciPtttTempCode";
			this.lciPtttTempCode.Size = new System.Drawing.Size(430, 24);
			this.lciPtttTempCode.Text = "Mã:";
			this.lciPtttTempCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPtttTempCode.TextSize = new System.Drawing.Size(60, 20);
			this.lciPtttTempCode.TextToControlDistance = 5;
			// 
			// lciPtttTempName
			// 
			this.lciPtttTempName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
			this.lciPtttTempName.AppearanceItemCaption.Options.UseForeColor = true;
			this.lciPtttTempName.AppearanceItemCaption.Options.UseTextOptions = true;
			this.lciPtttTempName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.lciPtttTempName.Control = this.txtPtttTempName;
			this.lciPtttTempName.Location = new System.Drawing.Point(0, 24);
			this.lciPtttTempName.Name = "lciPtttTempName";
			this.lciPtttTempName.Size = new System.Drawing.Size(430, 24);
			this.lciPtttTempName.Text = "Tên:";
			this.lciPtttTempName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciPtttTempName.TextSize = new System.Drawing.Size(60, 20);
			this.lciPtttTempName.TextToControlDistance = 5;
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.chkPublic;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(216, 24);
			this.layoutControlItem3.Text = " ";
			this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(60, 20);
			this.layoutControlItem3.TextToControlDistance = 5;
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.chkPublicDepartment;
			this.layoutControlItem4.Location = new System.Drawing.Point(216, 48);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(214, 24);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 72);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(322, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// layoutControlItem5
			// 
			this.layoutControlItem5.Control = this.btnSave;
			this.layoutControlItem5.Location = new System.Drawing.Point(322, 72);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(108, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			// 
			// dxValidationProvider1
			// 
			this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
			// 
			// barManager1
			// 
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnSave});
			this.barManager1.MaxItemId = 1;
			// 
			// bar1
			// 
			this.bar1.BarName = "Tools";
			this.bar1.DockCol = 0;
			this.bar1.DockRow = 0;
			this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave)});
			this.bar1.Text = "Tools";
			this.bar1.Visible = false;
			// 
			// barBtnSave
			// 
			this.barBtnSave.Caption = "Ctrl S";
			this.barBtnSave.Id = 0;
			this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
			this.barBtnSave.Name = "barBtnSave";
			this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
			// 
			// barDockControlTop
			// 
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
			this.barDockControlTop.Size = new System.Drawing.Size(447, 0);
			// 
			// barDockControlBottom
			// 
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 115);
			this.barDockControlBottom.Size = new System.Drawing.Size(447, 0);
			// 
			// barDockControlLeft
			// 
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 86);
			// 
			// barDockControlRight
			// 
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(447, 29);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 86);
			// 
			// FormPtttTemp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(447, 115);
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "FormPtttTemp";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mẫu phẫu thuật thủ thuật";
			this.Load += new System.EventHandler(this.FormPtttTemp_Load);
			this.Controls.SetChildIndex(this.barDockControlTop, 0);
			this.Controls.SetChildIndex(this.barDockControlBottom, 0);
			this.Controls.SetChildIndex(this.barDockControlRight, 0);
			this.Controls.SetChildIndex(this.barDockControlLeft, 0);
			this.Controls.SetChildIndex(this.layoutControl1, 0);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chkPublicDepartment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPublic.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPtttTempName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPtttTempCode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPtttTempCode)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPtttTempName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtPtttTempCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPtttTempCode;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.CheckEdit chkPublicDepartment;
        private DevExpress.XtraEditors.CheckEdit chkPublic;
        private DevExpress.XtraEditors.TextEdit txtPtttTempName;
        private DevExpress.XtraLayout.LayoutControlItem lciPtttTempName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
