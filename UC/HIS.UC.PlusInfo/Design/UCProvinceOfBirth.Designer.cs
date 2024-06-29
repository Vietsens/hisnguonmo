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
    partial class UCProvinceOfBirth
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
            this.txtProvinceOfBirthCode = new DevExpress.XtraEditors.TextEdit();
            this.cboProvinceOfBirth = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProvinceOfBirth = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderControl = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProviderControl = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceOfBirthCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProvinceOfBirth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvinceOfBirth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtProvinceOfBirthCode);
            this.layoutControl1.Controls.Add(this.cboProvinceOfBirth);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtProvinceOfBirthCode
            // 
            this.txtProvinceOfBirthCode.Location = new System.Drawing.Point(75, 0);
            this.txtProvinceOfBirthCode.Name = "txtProvinceOfBirthCode";
            this.txtProvinceOfBirthCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtProvinceOfBirthCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtProvinceOfBirthCode.Size = new System.Drawing.Size(52, 20);
            this.txtProvinceOfBirthCode.StyleController = this.layoutControl1;
            this.txtProvinceOfBirthCode.TabIndex = 0;
            this.txtProvinceOfBirthCode.Tag = "1";
            this.txtProvinceOfBirthCode.EditValueChanged += new System.EventHandler(this.txtProvinceOfBirthCode_EditValueChanged);
            this.txtProvinceOfBirthCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProvinceOfBirthCode_KeyDown);
            // 
            // cboProvinceOfBirth
            // 
            this.cboProvinceOfBirth.Location = new System.Drawing.Point(127, 0);
            this.cboProvinceOfBirth.Name = "cboProvinceOfBirth";
            this.cboProvinceOfBirth.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboProvinceOfBirth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboProvinceOfBirth.Properties.NullText = "";
            this.cboProvinceOfBirth.Size = new System.Drawing.Size(92, 20);
            this.cboProvinceOfBirth.StyleController = this.layoutControl1;
            this.cboProvinceOfBirth.TabIndex = 1;
            this.cboProvinceOfBirth.Tag = "2";
            this.cboProvinceOfBirth.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvinceOfBirth_Closed);
            this.cboProvinceOfBirth.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboProvinceOfBirth_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciProvinceOfBirth});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboProvinceOfBirth;
            this.layoutControlItem1.Location = new System.Drawing.Point(127, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(92, 24);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciProvinceOfBirth
            // 
            this.lciProvinceOfBirth.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciProvinceOfBirth.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciProvinceOfBirth.Control = this.txtProvinceOfBirthCode;
            this.lciProvinceOfBirth.Location = new System.Drawing.Point(0, 0);
            this.lciProvinceOfBirth.MaxSize = new System.Drawing.Size(0, 20);
            this.lciProvinceOfBirth.MinSize = new System.Drawing.Size(127, 20);
            this.lciProvinceOfBirth.Name = "lciProvinceOfBirth";
            this.lciProvinceOfBirth.OptionsToolTip.ToolTip = "Tỉnh khai sinh";
            this.lciProvinceOfBirth.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciProvinceOfBirth.Size = new System.Drawing.Size(127, 24);
            this.lciProvinceOfBirth.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciProvinceOfBirth.Text = "Tỉnh KS:";
            this.lciProvinceOfBirth.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciProvinceOfBirth.TextSize = new System.Drawing.Size(70, 20);
            this.lciProvinceOfBirth.TextToControlDistance = 5;
            // 
            // dxErrorProviderControl
            // 
            this.dxErrorProviderControl.ContainerControl = this;
            // 
            // UCProvinceOfBirth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCProvinceOfBirth";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCProvinceOfBirth_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceOfBirthCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProvinceOfBirth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvinceOfBirth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.LookUpEdit cboProvinceOfBirth;
        internal DevExpress.XtraEditors.TextEdit txtProvinceOfBirthCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciProvinceOfBirth;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderControl;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderControl;
    }
}
