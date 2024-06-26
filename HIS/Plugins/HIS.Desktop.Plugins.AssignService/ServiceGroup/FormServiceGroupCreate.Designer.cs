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
namespace HIS.Desktop.Plugins.AssignService.ServiceGroup
{
    partial class FormServiceGroupCreate
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
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.txtServiceGroupCode = new DevExpress.XtraEditors.TextEdit();
            this.lciServiceGroupCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.txtServiceGroupName = new DevExpress.XtraEditors.TextEdit();
            this.lciServiceGroupName = new DevExpress.XtraLayout.LayoutControlItem();
            this.mmDescription = new DevExpress.XtraEditors.MemoEdit();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.chkPublic = new DevExpress.XtraEditors.CheckEdit();
            this.lciPublic = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceGroupCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceGroupCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceGroupName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceGroupName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPublic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPublic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.chkPublic);
            this.layoutControl1.Controls.Add(this.mmDescription);
            this.layoutControl1.Controls.Add(this.txtServiceGroupName);
            this.layoutControl1.Controls.Add(this.txtServiceGroupCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 232);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciServiceGroupCode,
            this.lciServiceGroupName,
            this.lciDescription,
            this.lciPublic,
            this.emptySpaceItem1,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(440, 232);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // txtServiceGroupCode
            // 
            this.txtServiceGroupCode.Location = new System.Drawing.Point(97, 2);
            this.txtServiceGroupCode.Name = "txtServiceGroupCode";
            this.txtServiceGroupCode.Size = new System.Drawing.Size(341, 20);
            this.txtServiceGroupCode.StyleController = this.layoutControl1;
            this.txtServiceGroupCode.TabIndex = 4;
            this.txtServiceGroupCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceGroupCode_PreviewKeyDown);
            // 
            // lciServiceGroupCode
            // 
            this.lciServiceGroupCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciServiceGroupCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciServiceGroupCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceGroupCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceGroupCode.Control = this.txtServiceGroupCode;
            this.lciServiceGroupCode.Location = new System.Drawing.Point(0, 0);
            this.lciServiceGroupCode.Name = "lciServiceGroupCode";
            this.lciServiceGroupCode.Size = new System.Drawing.Size(440, 24);
            this.lciServiceGroupCode.Text = "Mã:";
            this.lciServiceGroupCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciServiceGroupCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciServiceGroupCode.TextToControlDistance = 5;
            // 
            // txtServiceGroupName
            // 
            this.txtServiceGroupName.Location = new System.Drawing.Point(97, 26);
            this.txtServiceGroupName.Name = "txtServiceGroupName";
            this.txtServiceGroupName.Size = new System.Drawing.Size(341, 20);
            this.txtServiceGroupName.StyleController = this.layoutControl1;
            this.txtServiceGroupName.TabIndex = 5;
            this.txtServiceGroupName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceGroupName_PreviewKeyDown);
            // 
            // lciServiceGroupName
            // 
            this.lciServiceGroupName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciServiceGroupName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciServiceGroupName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceGroupName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceGroupName.Control = this.txtServiceGroupName;
            this.lciServiceGroupName.Location = new System.Drawing.Point(0, 24);
            this.lciServiceGroupName.Name = "lciServiceGroupName";
            this.lciServiceGroupName.Size = new System.Drawing.Size(440, 24);
            this.lciServiceGroupName.Text = "Tên:";
            this.lciServiceGroupName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciServiceGroupName.TextSize = new System.Drawing.Size(90, 20);
            this.lciServiceGroupName.TextToControlDistance = 5;
            // 
            // mmDescription
            // 
            this.mmDescription.Location = new System.Drawing.Point(97, 50);
            this.mmDescription.Name = "mmDescription";
            this.mmDescription.Size = new System.Drawing.Size(341, 130);
            this.mmDescription.StyleController = this.layoutControl1;
            this.mmDescription.TabIndex = 6;
            // 
            // lciDescription
            // 
            this.lciDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDescription.Control = this.mmDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 48);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(440, 134);
            this.lciDescription.Text = "Mô tả:";
            this.lciDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDescription.TextSize = new System.Drawing.Size(90, 20);
            this.lciDescription.TextToControlDistance = 5;
            // 
            // chkPublic
            // 
            this.chkPublic.Location = new System.Drawing.Point(97, 184);
            this.chkPublic.Name = "chkPublic";
            this.chkPublic.Properties.Caption = "";
            this.chkPublic.Size = new System.Drawing.Size(341, 19);
            this.chkPublic.StyleController = this.layoutControl1;
            this.chkPublic.TabIndex = 7;
            // 
            // lciPublic
            // 
            this.lciPublic.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPublic.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPublic.Control = this.chkPublic;
            this.lciPublic.Location = new System.Drawing.Point(0, 182);
            this.lciPublic.Name = "lciPublic";
            this.lciPublic.Size = new System.Drawing.Size(440, 24);
            this.lciPublic.Text = "Chia sẻ:";
            this.lciPublic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPublic.TextSize = new System.Drawing.Size(90, 20);
            this.lciPublic.TextToControlDistance = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 208);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu(Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnSave;
            this.layoutControlItem5.Location = new System.Drawing.Point(330, 206);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 206);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControl1);
            this.barManager1.DockControls.Add(this.barDockControl2);
            this.barManager1.DockControls.Add(this.barDockControl3);
            this.barManager1.DockControls.Add(this.barDockControl4);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(440, 29);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 261);
            this.barDockControl2.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 232);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(440, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 232);
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
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // FormServiceGroupCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 261);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "FormServiceGroupCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tạo nhóm dịch vụ";
            this.Load += new System.EventHandler(this.FormServiceGroupCreate_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceGroupCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceGroupCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceGroupName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceGroupName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mmDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPublic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPublic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.CheckEdit chkPublic;
        private DevExpress.XtraEditors.MemoEdit mmDescription;
        private DevExpress.XtraEditors.TextEdit txtServiceGroupName;
        private DevExpress.XtraEditors.TextEdit txtServiceGroupCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceGroupCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceGroupName;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraLayout.LayoutControlItem lciPublic;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
