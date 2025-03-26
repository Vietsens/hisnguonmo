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
namespace HIS.UC.FormType.TimeFromTo
{
    partial class UCTimeFromTo
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
            this.dtTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.dtTimeTo);
            this.layoutControl1.Controls.Add(this.dtTimeFrom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 25);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // dtTimeTo
            // 
            this.dtTimeTo.EditValue = null;
            this.dtTimeTo.Location = new System.Drawing.Point(424, 2);
            this.dtTimeTo.Name = "dtTimeTo";
            this.dtTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeTo.Size = new System.Drawing.Size(234, 20);
            this.dtTimeTo.StyleController = this.layoutControl1;
            this.dtTimeTo.TabIndex = 2;
            // 
            // dtTimeFrom
            // 
            this.dtTimeFrom.EditValue = null;
            this.dtTimeFrom.Location = new System.Drawing.Point(97, 2);
            this.dtTimeFrom.Name = "dtTimeFrom";
            this.dtTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dtTimeFrom.Size = new System.Drawing.Size(232, 20);
            this.dtTimeFrom.StyleController = this.layoutControl1;
            this.dtTimeFrom.TabIndex = 1;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutTimeFrom,
            this.layoutTimeTo});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutTimeFrom
            // 
            this.layoutTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTimeFrom.Control = this.dtTimeFrom;
            this.layoutTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.layoutTimeFrom.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutTimeFrom.MinSize = new System.Drawing.Size(147, 24);
            this.layoutTimeFrom.Name = "layoutTimeFrom";
            this.layoutTimeFrom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutTimeFrom.Size = new System.Drawing.Size(329, 25);
            this.layoutTimeFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutTimeFrom.Text = "Từ:";
            this.layoutTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTimeFrom.TextSize = new System.Drawing.Size(90, 13);
            this.layoutTimeFrom.TextToControlDistance = 5;
            // 
            // layoutTimeTo
            // 
            this.layoutTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTimeTo.Control = this.dtTimeTo;
            this.layoutTimeTo.Location = new System.Drawing.Point(329, 0);
            this.layoutTimeTo.Name = "layoutTimeTo";
            this.layoutTimeTo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutTimeTo.Size = new System.Drawing.Size(331, 25);
            this.layoutTimeTo.Text = "Đến:";
            this.layoutTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTimeTo.TextSize = new System.Drawing.Size(90, 13);
            this.layoutTimeTo.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCTimeFromTo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCTimeFromTo";
            this.Size = new System.Drawing.Size(660, 25);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.DateEdit dtTimeTo;
        private DevExpress.XtraEditors.DateEdit dtTimeFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutTimeTo;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
