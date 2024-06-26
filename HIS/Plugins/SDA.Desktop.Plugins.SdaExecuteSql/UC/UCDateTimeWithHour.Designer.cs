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
namespace SDA.Desktop.Plugins.SdaExecuteSql.UC
{
    partial class UCDateTimeWithHour
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
            this.lbNote = new System.Windows.Forms.Label();
            this.dateDateTimeWithHour = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciDateTimeWithHour = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dateDateTimeWithHour.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateDateTimeWithHour.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateTimeWithHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lbNote);
            this.layoutControl1.Controls.Add(this.dateDateTimeWithHour);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(500, 25);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lbNote
            // 
            this.lbNote.Location = new System.Drawing.Point(252, 2);
            this.lbNote.Name = "lbNote";
            this.lbNote.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lbNote.Size = new System.Drawing.Size(246, 21);
            this.lbNote.TabIndex = 6;
            // 
            // dateDateTimeWithHour
            // 
            this.dateDateTimeWithHour.EditValue = null;
            this.dateDateTimeWithHour.Location = new System.Drawing.Point(125, 0);
            this.dateDateTimeWithHour.Name = "dateDateTimeWithHour";
            this.dateDateTimeWithHour.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateDateTimeWithHour.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dateDateTimeWithHour.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dateDateTimeWithHour.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateDateTimeWithHour.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
            this.dateDateTimeWithHour.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dateDateTimeWithHour.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
            this.dateDateTimeWithHour.Size = new System.Drawing.Size(125, 20);
            this.dateDateTimeWithHour.StyleController = this.layoutControl1;
            this.dateDateTimeWithHour.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDateTimeWithHour,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(500, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciDateTimeWithHour
            // 
            this.lciDateTimeWithHour.Control = this.dateDateTimeWithHour;
            this.lciDateTimeWithHour.Location = new System.Drawing.Point(0, 0);
            this.lciDateTimeWithHour.Name = "lciDateTimeWithHour";
            this.lciDateTimeWithHour.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciDateTimeWithHour.Size = new System.Drawing.Size(250, 25);
            this.lciDateTimeWithHour.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDateTimeWithHour.TextSize = new System.Drawing.Size(120, 20);
            this.lciDateTimeWithHour.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lbNote;
            this.layoutControlItem1.Location = new System.Drawing.Point(250, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(250, 25);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // UCDateTimeWithHour
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCDateTimeWithHour";
            this.Size = new System.Drawing.Size(500, 25);
            this.Load += new System.EventHandler(this.UCDateTimeWithHour_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dateDateTimeWithHour.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateDateTimeWithHour.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateTimeWithHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DateEdit dateDateTimeWithHour;
        private DevExpress.XtraLayout.LayoutControlItem lciDateTimeWithHour;
        private System.Windows.Forms.Label lbNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
