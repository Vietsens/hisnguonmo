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
    partial class UCNational
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
            this.cboNational = new DevExpress.XtraEditors.LookUpEdit();
            this.txtNationalCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNational = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboNational.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNational)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboNational);
            this.layoutControl1.Controls.Add(this.txtNationalCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboNational
            // 
            this.cboNational.Location = new System.Drawing.Point(127, 0);
            this.cboNational.Name = "cboNational";
            this.cboNational.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.cboNational.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboNational.Properties.NullText = "";
            this.cboNational.Size = new System.Drawing.Size(92, 20);
            this.cboNational.StyleController = this.layoutControl1;
            this.cboNational.TabIndex = 1;
            this.cboNational.Tag = "NATIONAL_NAME";
            this.cboNational.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNational_Closed);
            this.cboNational.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboNational_KeyUp);
            // 
            // txtNationalCode
            // 
            this.txtNationalCode.Location = new System.Drawing.Point(75, 0);
            this.txtNationalCode.Name = "txtNationalCode";
            this.txtNationalCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtNationalCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtNationalCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNationalCode.Size = new System.Drawing.Size(52, 20);
            this.txtNationalCode.StyleController = this.layoutControl1;
            this.txtNationalCode.TabIndex = 0;
            this.txtNationalCode.Tag = "NATIONAL_CODE";
            this.txtNationalCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNationalCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciNational});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboNational;
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
            // lciNational
            // 
            this.lciNational.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciNational.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciNational.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNational.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNational.Control = this.txtNationalCode;
            this.lciNational.Location = new System.Drawing.Point(0, 0);
            this.lciNational.MaxSize = new System.Drawing.Size(0, 20);
            this.lciNational.MinSize = new System.Drawing.Size(127, 20);
            this.lciNational.Name = "lciNational";
            this.lciNational.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciNational.Size = new System.Drawing.Size(127, 24);
            this.lciNational.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciNational.Text = "Quốc tịch:";
            this.lciNational.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNational.TextSize = new System.Drawing.Size(70, 20);
            this.lciNational.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // UCNational
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCNational";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCNational_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboNational.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNational)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.LookUpEdit cboNational;
        internal DevExpress.XtraEditors.TextEdit txtNationalCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciNational;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
