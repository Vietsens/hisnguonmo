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
    partial class UCCMND
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
            this.txtCMND = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciCMND = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderCMND = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxErrorProviderControl = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCMND.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMND)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderCMND)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtCMND);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtCMND
            // 
            this.txtCMND.Location = new System.Drawing.Point(75, 0);
            this.txtCMND.Name = "txtCMND";
            this.txtCMND.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.txtCMND.Properties.MaxLength = 12;
            this.txtCMND.Size = new System.Drawing.Size(144, 20);
            this.txtCMND.StyleController = this.layoutControl1;
            this.txtCMND.TabIndex = 31;
            this.txtCMND.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCMND_KeyDown);
            this.txtCMND.Validated += new System.EventHandler(this.txtCMND_Validated);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCMND});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciCMND
            // 
            this.lciCMND.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCMND.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCMND.Control = this.txtCMND;
            this.lciCMND.Location = new System.Drawing.Point(0, 0);
            this.lciCMND.MaxSize = new System.Drawing.Size(0, 20);
            this.lciCMND.MinSize = new System.Drawing.Size(110, 20);
            this.lciCMND.Name = "layoutControlItem2";
            this.lciCMND.OptionsToolTip.ToolTip = "Chứng minh nhân dân/ Căn cước công dân/ Hộ chiếu";
            this.lciCMND.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciCMND.Size = new System.Drawing.Size(219, 24);
            this.lciCMND.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCMND.Text = "CMT/CC/HC:";
            this.lciCMND.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCMND.TextSize = new System.Drawing.Size(70, 20);
            this.lciCMND.TextToControlDistance = 5;
            // 
            // dxErrorProviderControl
            // 
            this.dxErrorProviderControl.ContainerControl = this;
            // 
            // UCCMND
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCCMND";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCCMND_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCMND.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMND)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderCMND)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.TextEdit txtCMND;
        private DevExpress.XtraLayout.LayoutControlItem lciCMND;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderCMND;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderControl;
    }
}
