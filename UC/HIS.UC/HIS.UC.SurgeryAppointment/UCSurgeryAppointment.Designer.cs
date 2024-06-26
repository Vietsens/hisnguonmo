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
namespace HIS.UC.SurgeryAppointment
{
    partial class UCSurgeryAppointment
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtGhiChu = new DevExpress.XtraEditors.MemoEdit();
            this.txtDichVuMo = new DevExpress.XtraEditors.MemoEdit();
            this.dtSurgeryTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.chkPrintAppointSurg = new DevExpress.XtraEditors.CheckEdit();
            this.lciPrintAppointSurg = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGhiChu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDichVuMo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSurgeryTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSurgeryTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintAppointSurg.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintAppointSurg)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkPrintAppointSurg);
            this.layoutControl1.Controls.Add(this.txtGhiChu);
            this.layoutControl1.Controls.Add(this.txtDichVuMo);
            this.layoutControl1.Controls.Add(this.dtSurgeryTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(925, 111, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(500, 149);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtGhiChu
            // 
            this.txtGhiChu.Location = new System.Drawing.Point(107, 75);
            this.txtGhiChu.Name = "txtGhiChu";
            this.txtGhiChu.Properties.MaxLength = 500;
            this.txtGhiChu.Size = new System.Drawing.Size(391, 48);
            this.txtGhiChu.StyleController = this.layoutControl1;
            this.txtGhiChu.TabIndex = 6;
            // 
            // txtDichVuMo
            // 
            this.txtDichVuMo.Location = new System.Drawing.Point(107, 26);
            this.txtDichVuMo.Name = "txtDichVuMo";
            this.txtDichVuMo.Properties.MaxLength = 500;
            this.txtDichVuMo.Size = new System.Drawing.Size(391, 45);
            this.txtDichVuMo.StyleController = this.layoutControl1;
            this.txtDichVuMo.TabIndex = 5;
            // 
            // dtSurgeryTime
            // 
            this.dtSurgeryTime.EditValue = null;
            this.dtSurgeryTime.Location = new System.Drawing.Point(107, 2);
            this.dtSurgeryTime.Name = "dtSurgeryTime";
            this.dtSurgeryTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSurgeryTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtSurgeryTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtSurgeryTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtSurgeryTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtSurgeryTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtSurgeryTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtSurgeryTime.Size = new System.Drawing.Size(141, 20);
            this.dtSurgeryTime.StyleController = this.layoutControl1;
            this.dtSurgeryTime.TabIndex = 4;
            this.dtSurgeryTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtSurgeryTime_Closed);
            this.dtSurgeryTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtSurgeryTime_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.lciPrintAppointSurg});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(500, 149);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.dtSurgeryTime;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(250, 24);
            this.layoutControlItem1.Text = "Ngày hẹn mổ:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.txtDichVuMo;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(500, 49);
            this.layoutControlItem2.Text = "Dịch vụ mổ:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.txtGhiChu;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 73);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(500, 52);
            this.layoutControlItem3.Text = "Ghi chú:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(250, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(250, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // chkPrintAppointSurg
            // 
            this.chkPrintAppointSurg.Location = new System.Drawing.Point(107, 127);
            this.chkPrintAppointSurg.Name = "chkPrintAppointSurg";
            this.chkPrintAppointSurg.Properties.Caption = "";
            this.chkPrintAppointSurg.Size = new System.Drawing.Size(391, 19);
            this.chkPrintAppointSurg.StyleController = this.layoutControl1;
            this.chkPrintAppointSurg.TabIndex = 7;
            this.chkPrintAppointSurg.CheckedChanged += new System.EventHandler(this.chkPrintAppointSurg_CheckedChanged);
            // 
            // lciPrintAppointSurg
            // 
            this.lciPrintAppointSurg.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrintAppointSurg.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrintAppointSurg.Control = this.chkPrintAppointSurg;
            this.lciPrintAppointSurg.Location = new System.Drawing.Point(0, 125);
            this.lciPrintAppointSurg.Name = "lciPrintAppointSurg";
            this.lciPrintAppointSurg.Size = new System.Drawing.Size(500, 24);
            this.lciPrintAppointSurg.Text = "In phiếu hẹn mổ:";
            this.lciPrintAppointSurg.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPrintAppointSurg.TextSize = new System.Drawing.Size(100, 20);
            this.lciPrintAppointSurg.TextToControlDistance = 5;
            // 
            // UCSurgeryAppointment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCSurgeryAppointment";
            this.Size = new System.Drawing.Size(500, 149);
            this.Load += new System.EventHandler(this.UCSurgeryAppointment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtGhiChu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDichVuMo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSurgeryTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSurgeryTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintAppointSurg.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintAppointSurg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.MemoEdit txtGhiChu;
        private DevExpress.XtraEditors.MemoEdit txtDichVuMo;
        private DevExpress.XtraEditors.DateEdit dtSurgeryTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.CheckEdit chkPrintAppointSurg;
        private DevExpress.XtraLayout.LayoutControlItem lciPrintAppointSurg;
    }
}
