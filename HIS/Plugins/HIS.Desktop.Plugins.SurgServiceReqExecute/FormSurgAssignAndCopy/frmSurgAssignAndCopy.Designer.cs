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
namespace HIS.Desktop.Plugins.SurgServiceReqExecute.FormSurgAssignAndCopy
{
    partial class frmSurgAssignAndCopy
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
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.calendarInstructionDate = new DevExpress.XtraEditors.Controls.CalendarControl();
            this.dtInstructionDateTo = new DevExpress.XtraEditors.DateEdit();
            this.dtInstructionDateFrom = new DevExpress.XtraEditors.DateEdit();
            this.chkNgayLienTiep = new DevExpress.XtraEditors.CheckEdit();
            this.timeInstructionTime = new DevExpress.XtraEditors.TimeSpanEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciNgayLienTiep = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInstructionDateFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInstructionDateTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciCalendarInstructionDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calendarInstructionDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNgayLienTiep.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeInstructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgayLienTiep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionDateFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionDateTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCalendarInstructionDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.Controls.Add(this.btnSelect);
            this.layoutControlRoot.Controls.Add(this.calendarInstructionDate);
            this.layoutControlRoot.Controls.Add(this.dtInstructionDateTo);
            this.layoutControlRoot.Controls.Add(this.dtInstructionDateFrom);
            this.layoutControlRoot.Controls.Add(this.chkNgayLienTiep);
            this.layoutControlRoot.Controls.Add(this.timeInstructionTime);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(466, 153, 250, 350);
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(383, 350);
            this.layoutControlRoot.TabIndex = 4;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(264, 315);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(107, 22);
            this.btnSelect.StyleController = this.layoutControlRoot;
            this.btnSelect.TabIndex = 9;
            this.btnSelect.Text = "Chọn";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // calendarInstructionDate
            // 
            this.calendarInstructionDate.AutoSize = false;
            this.calendarInstructionDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.calendarInstructionDate.Location = new System.Drawing.Point(127, 84);
            this.calendarInstructionDate.Name = "calendarInstructionDate";
            this.calendarInstructionDate.Size = new System.Drawing.Size(244, 227);
            this.calendarInstructionDate.StyleController = this.layoutControlRoot;
            this.calendarInstructionDate.TabIndex = 8;
            // 
            // dtInstructionDateTo
            // 
            this.dtInstructionDateTo.EditValue = null;
            this.dtInstructionDateTo.Location = new System.Drawing.Point(245, 60);
            this.dtInstructionDateTo.Name = "dtInstructionDateTo";
            this.dtInstructionDateTo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtInstructionDateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionDateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionDateTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionDateTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionDateTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionDateTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionDateTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInstructionDateTo.Size = new System.Drawing.Size(126, 20);
            this.dtInstructionDateTo.StyleController = this.layoutControlRoot;
            this.dtInstructionDateTo.TabIndex = 7;
            // 
            // dtInstructionDateFrom
            // 
            this.dtInstructionDateFrom.EditValue = null;
            this.dtInstructionDateFrom.Location = new System.Drawing.Point(127, 60);
            this.dtInstructionDateFrom.Name = "dtInstructionDateFrom";
            this.dtInstructionDateFrom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtInstructionDateFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionDateFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionDateFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionDateFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtInstructionDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionDateFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInstructionDateFrom.Size = new System.Drawing.Size(114, 20);
            this.dtInstructionDateFrom.StyleController = this.layoutControlRoot;
            this.dtInstructionDateFrom.TabIndex = 6;
            // 
            // chkNgayLienTiep
            // 
            this.chkNgayLienTiep.Location = new System.Drawing.Point(127, 12);
            this.chkNgayLienTiep.Name = "chkNgayLienTiep";
            this.chkNgayLienTiep.Properties.Caption = "";
            this.chkNgayLienTiep.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkNgayLienTiep.Size = new System.Drawing.Size(26, 19);
            this.chkNgayLienTiep.StyleController = this.layoutControlRoot;
            this.chkNgayLienTiep.TabIndex = 4;
            this.chkNgayLienTiep.CheckedChanged += new System.EventHandler(this.chkNgayLienTiep_CheckedChanged);
            // 
            // timeInstructionTime
            // 
            this.timeInstructionTime.EditValue = System.TimeSpan.Parse("738363.00:00:00");
            this.timeInstructionTime.Location = new System.Drawing.Point(127, 36);
            this.timeInstructionTime.Name = "timeInstructionTime";
            this.timeInstructionTime.Properties.AllowEditDays = false;
            this.timeInstructionTime.Properties.AllowEditSeconds = false;
            this.timeInstructionTime.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.timeInstructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeInstructionTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeInstructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeInstructionTime.Properties.Mask.EditMask = "HH:mm";
            this.timeInstructionTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.timeInstructionTime.Size = new System.Drawing.Size(244, 20);
            this.timeInstructionTime.StyleController = this.layoutControlRoot;
            this.timeInstructionTime.TabIndex = 5;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciNgayLienTiep,
            this.layoutControlItem2,
            this.lciInstructionDateFrom,
            this.lciInstructionDateTo,
            this.emptySpaceItem1,
            this.lciCalendarInstructionDate,
            this.layoutControlItem6,
            this.emptySpaceItem2});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(383, 350);
            this.Root.TextVisible = false;
            // 
            // lciNgayLienTiep
            // 
            this.lciNgayLienTiep.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNgayLienTiep.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNgayLienTiep.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lciNgayLienTiep.Control = this.chkNgayLienTiep;
            this.lciNgayLienTiep.Location = new System.Drawing.Point(0, 0);
            this.lciNgayLienTiep.Name = "lciNgayLienTiep";
            this.lciNgayLienTiep.Size = new System.Drawing.Size(145, 24);
            this.lciNgayLienTiep.Text = "Ngày liên tiếp:";
            this.lciNgayLienTiep.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNgayLienTiep.TextSize = new System.Drawing.Size(110, 20);
            this.lciNgayLienTiep.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.layoutControlItem2.Control = this.timeInstructionTime;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(363, 24);
            this.layoutControlItem2.Text = "Giờ y lệnh:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(110, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // lciInstructionDateFrom
            // 
            this.lciInstructionDateFrom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciInstructionDateFrom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciInstructionDateFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciInstructionDateFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciInstructionDateFrom.Control = this.dtInstructionDateFrom;
            this.lciInstructionDateFrom.Location = new System.Drawing.Point(0, 48);
            this.lciInstructionDateFrom.Name = "lciInstructionDateFrom";
            this.lciInstructionDateFrom.Size = new System.Drawing.Size(233, 24);
            this.lciInstructionDateFrom.Text = "Ngày y lệnh:";
            this.lciInstructionDateFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciInstructionDateFrom.TextSize = new System.Drawing.Size(110, 20);
            this.lciInstructionDateFrom.TextToControlDistance = 5;
            // 
            // lciInstructionDateTo
            // 
            this.lciInstructionDateTo.Control = this.dtInstructionDateTo;
            this.lciInstructionDateTo.Location = new System.Drawing.Point(233, 48);
            this.lciInstructionDateTo.Name = "lciInstructionDateTo";
            this.lciInstructionDateTo.Size = new System.Drawing.Size(130, 24);
            this.lciInstructionDateTo.TextSize = new System.Drawing.Size(0, 0);
            this.lciInstructionDateTo.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 303);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(252, 27);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciCalendarInstructionDate
            // 
            this.lciCalendarInstructionDate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCalendarInstructionDate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCalendarInstructionDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCalendarInstructionDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCalendarInstructionDate.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lciCalendarInstructionDate.Control = this.calendarInstructionDate;
            this.lciCalendarInstructionDate.Location = new System.Drawing.Point(0, 72);
            this.lciCalendarInstructionDate.Name = "lciCalendarInstructionDate";
            this.lciCalendarInstructionDate.Size = new System.Drawing.Size(363, 231);
            this.lciCalendarInstructionDate.Text = "Ngày y lệnh:";
            this.lciCalendarInstructionDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCalendarInstructionDate.TextSize = new System.Drawing.Size(110, 20);
            this.lciCalendarInstructionDate.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnSelect;
            this.layoutControlItem6.Location = new System.Drawing.Point(252, 303);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(111, 27);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(145, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(218, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // frmSurgAssignAndCopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 350);
            this.Controls.Add(this.layoutControlRoot);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSurgAssignAndCopy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thời gian chỉ định";
            this.Load += new System.EventHandler(this.frmSurgAssignAndCopy_Load);
            this.Controls.SetChildIndex(this.layoutControlRoot, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendarInstructionDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionDateFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNgayLienTiep.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeInstructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNgayLienTiep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionDateFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionDateTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCalendarInstructionDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.Controls.CalendarControl calendarInstructionDate;
        private DevExpress.XtraEditors.DateEdit dtInstructionDateTo;
        private DevExpress.XtraEditors.DateEdit dtInstructionDateFrom;
        private DevExpress.XtraEditors.CheckEdit chkNgayLienTiep;
        private DevExpress.XtraLayout.LayoutControlItem lciNgayLienTiep;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lciInstructionDateFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciInstructionDateTo;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciCalendarInstructionDate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.TimeSpanEdit timeInstructionTime;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
    }
}
