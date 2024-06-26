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
namespace HIS.UC.PlusInfo.Design
{
    partial class UCCMNDDate
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtCMNDDate = new DevExpress.XtraEditors.ButtonEdit();
            this.dtCMNDDate = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciCMNDDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCMNDDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCMNDDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCMNDDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMNDDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panel1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtCMNDDate);
            this.panel1.Controls.Add(this.dtCMNDDate);
            this.panel1.Location = new System.Drawing.Point(75, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 20);
            this.panel1.TabIndex = 4;
            // 
            // txtCMNDDate
            // 
            this.txtCMNDDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCMNDDate.Location = new System.Drawing.Point(0, 0);
            this.txtCMNDDate.Name = "txtCMNDDate";
            this.txtCMNDDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.txtCMNDDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtCMNDDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtCMNDDate.Properties.Mask.EditMask = "\\d{2}/\\d{2}/\\d{4}";
            this.txtCMNDDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.txtCMNDDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtCMNDDate.Size = new System.Drawing.Size(144, 20);
            this.txtCMNDDate.TabIndex = 0;
            this.txtCMNDDate.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCMNDDate_ButtonClick);
            this.txtCMNDDate.EditValueChanged += new System.EventHandler(this.txtCMNDDate_EditValueChanged);
            this.txtCMNDDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCMNDDate_KeyDown);
            this.txtCMNDDate.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCMNDDate_PreviewKeyDown);
            this.txtCMNDDate.Validated += new System.EventHandler(this.txtCMNDDate_Validated);
            // 
            // dtCMNDDate
            // 
            this.dtCMNDDate.EditValue = null;
            this.dtCMNDDate.Location = new System.Drawing.Point(0, 1);
            this.dtCMNDDate.Name = "dtCMNDDate";
            this.dtCMNDDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCMNDDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCMNDDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtCMNDDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCMNDDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtCMNDDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtCMNDDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtCMNDDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtCMNDDate.Properties.NullValuePrompt = "dd/MM/yyyy";
            this.dtCMNDDate.Properties.NullValuePromptShowForEmptyValue = true;
            this.dtCMNDDate.Properties.ShowNullValuePromptWhenFocused = true;
            this.dtCMNDDate.Size = new System.Drawing.Size(137, 20);
            this.dtCMNDDate.TabIndex = 1;
            this.dtCMNDDate.Visible = false;
            this.dtCMNDDate.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtCMNDDate_Closed);
            this.dtCMNDDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtCMNDDate_KeyDown);
            this.dtCMNDDate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dtCMNDDate_KeyPress);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCMNDDate});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciCMNDDate
            // 
            this.lciCMNDDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCMNDDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCMNDDate.Control = this.panel1;
            this.lciCMNDDate.Location = new System.Drawing.Point(0, 0);
            this.lciCMNDDate.MaxSize = new System.Drawing.Size(0, 20);
            this.lciCMNDDate.MinSize = new System.Drawing.Size(110, 20);
            this.lciCMNDDate.Name = "lciCMNDDate";
            this.lciCMNDDate.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciCMNDDate.Size = new System.Drawing.Size(219, 24);
            this.lciCMNDDate.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCMNDDate.Text = "Ngày cấp:";
            this.lciCMNDDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCMNDDate.TextSize = new System.Drawing.Size(70, 20);
            this.lciCMNDDate.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // UCCMNDDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCCMNDDate";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCCMNDDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCMNDDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCMNDDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCMNDDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMNDDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.DateEdit dtCMNDDate;
        private DevExpress.XtraLayout.LayoutControlItem lciCMNDDate;
        private DevExpress.XtraEditors.ButtonEdit txtCMNDDate;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
