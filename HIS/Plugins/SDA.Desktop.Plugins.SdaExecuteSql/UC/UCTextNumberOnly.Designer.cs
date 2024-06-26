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
    partial class UCTextNumberOnly
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
            this.lbNote = new System.Windows.Forms.Label();
            this.txtTextNumberOnly = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTextNumberOnly = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTextNumberOnly.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTextNumberOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lbNote);
            this.layoutControl1.Controls.Add(this.txtTextNumberOnly);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(471, 25);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lbNote
            // 
            this.lbNote.Location = new System.Drawing.Point(237, 2);
            this.lbNote.Name = "lbNote";
            this.lbNote.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lbNote.Size = new System.Drawing.Size(232, 21);
            this.lbNote.TabIndex = 5;
            // 
            // txtTextNumberOnly
            // 
            this.txtTextNumberOnly.Location = new System.Drawing.Point(125, 0);
            this.txtTextNumberOnly.Name = "txtTextNumberOnly";
            this.txtTextNumberOnly.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtTextNumberOnly.Properties.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtTextNumberOnly.Properties.Appearance.Options.UseBackColor = true;
            this.txtTextNumberOnly.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.txtTextNumberOnly.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtTextNumberOnly.Properties.Mask.ShowPlaceHolders = false;
            this.txtTextNumberOnly.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtTextNumberOnly.Properties.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTextNumberOnly_Properties_KeyPress);
            this.txtTextNumberOnly.Size = new System.Drawing.Size(110, 20);
            this.txtTextNumberOnly.StyleController = this.layoutControl1;
            this.txtTextNumberOnly.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTextNumberOnly,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(471, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciTextNumberOnly
            // 
            this.lciTextNumberOnly.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTextNumberOnly.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTextNumberOnly.Control = this.txtTextNumberOnly;
            this.lciTextNumberOnly.Location = new System.Drawing.Point(0, 0);
            this.lciTextNumberOnly.Name = "lciTextNumberOnly";
            this.lciTextNumberOnly.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciTextNumberOnly.Size = new System.Drawing.Size(235, 25);
            this.lciTextNumberOnly.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTextNumberOnly.TextSize = new System.Drawing.Size(120, 20);
            this.lciTextNumberOnly.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lbNote;
            this.layoutControlItem1.Location = new System.Drawing.Point(235, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(236, 25);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // dxErrorProvider
            // 
            this.dxErrorProvider.ContainerControl = this;
            // 
            // UCTextNumberOnly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCTextNumberOnly";
            this.Size = new System.Drawing.Size(471, 25);
            this.Load += new System.EventHandler(this.UCTextNumberOnly_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTextNumberOnly.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTextNumberOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtTextNumberOnly;
        private DevExpress.XtraLayout.LayoutControlItem lciTextNumberOnly;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private System.Windows.Forms.Label lbNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
    }
}
