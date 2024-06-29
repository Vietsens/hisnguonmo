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
    partial class UCEthnic
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
            this.txtEthnicCode = new DevExpress.XtraEditors.TextEdit();
            this.cboEthnic = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciEthnic = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEthnicCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEthnic.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEthnic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtEthnicCode);
            this.layoutControl1.Controls.Add(this.cboEthnic);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtEthnicCode
            // 
            this.txtEthnicCode.Location = new System.Drawing.Point(75, 0);
            this.txtEthnicCode.Name = "txtEthnicCode";
            this.txtEthnicCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtEthnicCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtEthnicCode.Size = new System.Drawing.Size(52, 20);
            this.txtEthnicCode.StyleController = this.layoutControl1;
            this.txtEthnicCode.TabIndex = 31;
            this.txtEthnicCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtEthnicCode_PreviewKeyDown);
            // 
            // cboEthnic
            // 
            this.cboEthnic.Location = new System.Drawing.Point(127, 0);
            this.cboEthnic.Name = "cboEthnic";
            this.cboEthnic.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboEthnic.Properties.NullText = "";
            this.cboEthnic.Size = new System.Drawing.Size(92, 20);
            this.cboEthnic.StyleController = this.layoutControl1;
            this.cboEthnic.TabIndex = 32;
            this.cboEthnic.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboEthnic_Closed);
            this.cboEthnic.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboEthnic_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciEthnic});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboEthnic;
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
            // lciEthnic
            // 
            this.lciEthnic.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciEthnic.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciEthnic.Control = this.txtEthnicCode;
            this.lciEthnic.Location = new System.Drawing.Point(0, 0);
            this.lciEthnic.MaxSize = new System.Drawing.Size(0, 20);
            this.lciEthnic.MinSize = new System.Drawing.Size(127, 20);
            this.lciEthnic.Name = "lciEthnic";
            this.lciEthnic.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciEthnic.Size = new System.Drawing.Size(127, 24);
            this.lciEthnic.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciEthnic.Text = "Dân tộc:";
            this.lciEthnic.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciEthnic.TextSize = new System.Drawing.Size(70, 20);
            this.lciEthnic.TextToControlDistance = 5;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // UCEthnic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCEthnic";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCEthnic_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtEthnicCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEthnic.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEthnic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.LookUpEdit cboEthnic;
        internal DevExpress.XtraEditors.TextEdit txtEthnicCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciEthnic;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
