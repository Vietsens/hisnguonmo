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
namespace HIS.UC.FormType.HourFromTo
{
    partial class UCHourFromTo
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutHourFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dtHourTo = new DevExpress.XtraEditors.TimeEdit();
            this.dtHourFrom = new DevExpress.XtraEditors.TimeEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHourFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtHourTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtHourFrom.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.dtHourTo);
            this.layoutControl1.Controls.Add(this.dtHourFrom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 25);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutHourFrom,
            this.layoutTimeTo});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutHourFrom
            // 
            this.layoutHourFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutHourFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutHourFrom.Control = this.dtHourFrom;
            this.layoutHourFrom.Location = new System.Drawing.Point(0, 0);
            this.layoutHourFrom.MinSize = new System.Drawing.Size(50, 25);
            this.layoutHourFrom.Name = "layoutHourFrom";
            this.layoutHourFrom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.layoutHourFrom.Size = new System.Drawing.Size(314, 25);
            this.layoutHourFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutHourFrom.Text = "Từ giờ:";
            this.layoutHourFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutHourFrom.TextSize = new System.Drawing.Size(90, 13);
            this.layoutHourFrom.TextToControlDistance = 5;
            // 
            // layoutTimeTo
            // 
            this.layoutTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutTimeTo.Control = this.dtHourTo;
            this.layoutTimeTo.Location = new System.Drawing.Point(314, 0);
            this.layoutTimeTo.Name = "layoutTimeTo";
            this.layoutTimeTo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutTimeTo.Size = new System.Drawing.Size(346, 25);
            this.layoutTimeTo.Text = "Đến giờ:";
            this.layoutTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutTimeTo.TextSize = new System.Drawing.Size(90, 13);
            this.layoutTimeTo.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dtHourTo
            // 
            this.dtHourTo.EditValue = null;
            this.dtHourTo.Location = new System.Drawing.Point(409, 2);
            this.dtHourTo.Name = "dtHourTo";
            this.dtHourTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtHourTo.Properties.DisplayFormat.FormatString = "HH:mm:ss";
            this.dtHourTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtHourTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dtHourTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtHourTo.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.dtHourTo.Properties.Mask.EditMask = "HH:mm:ss";
            this.dtHourTo.Properties.TimeEditStyle = DevExpress.XtraEditors.Repository.TimeEditStyle.TouchUI;
            this.dtHourTo.Size = new System.Drawing.Size(249, 20);
            this.dtHourTo.StyleController = this.layoutControl1;
            this.dtHourTo.TabIndex = 2;
            // 
            // dtHourFrom
            // 
            this.dtHourFrom.EditValue = null;
            this.dtHourFrom.Location = new System.Drawing.Point(97, 2);
            this.dtHourFrom.Name = "dtHourFrom";
            this.dtHourFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtHourFrom.Properties.DisplayFormat.FormatString = "HH:mm:ss";
            this.dtHourFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtHourFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtHourFrom.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.dtHourFrom.Properties.Mask.EditMask = "HH:mm:ss";
            this.dtHourFrom.Properties.TimeEditStyle = DevExpress.XtraEditors.Repository.TimeEditStyle.TouchUI;
            this.dtHourFrom.Size = new System.Drawing.Size(217, 20);
            this.dtHourFrom.StyleController = this.layoutControl1;
            this.dtHourFrom.TabIndex = 1;
            // 
            // UCHourFromTo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCHourFromTo";
            this.Size = new System.Drawing.Size(660, 25);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHourFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtHourTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtHourFrom.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutHourFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutTimeTo;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.TimeEdit dtHourTo;
        private DevExpress.XtraEditors.TimeEdit dtHourFrom;
    }
}
