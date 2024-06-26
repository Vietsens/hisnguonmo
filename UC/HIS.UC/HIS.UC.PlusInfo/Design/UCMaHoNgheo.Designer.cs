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
    partial class UCMaHoNgheo
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
            this.txtHoNgheoCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMaHoNgheo = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderMaHN = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoNgheoCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaHoNgheo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderMaHN)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtHoNgheoCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtHoNgheoCode
            // 
            this.txtHoNgheoCode.Location = new System.Drawing.Point(75, 0);
            this.txtHoNgheoCode.Name = "txtHoNgheoCode";
            this.txtHoNgheoCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtHoNgheoCode.Properties.MaxLength = 20;
            this.txtHoNgheoCode.Size = new System.Drawing.Size(144, 20);
            this.txtHoNgheoCode.StyleController = this.layoutControl1;
            this.txtHoNgheoCode.TabIndex = 31;
            this.txtHoNgheoCode.EditValueChanged += new System.EventHandler(this.txtHoNgheoCode_EditValueChanged);
            this.txtHoNgheoCode.TextChanged += new System.EventHandler(this.txtHoNgheoCode_TextChanged);
            this.txtHoNgheoCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHoNgheoCode_KeyDown);
            this.txtHoNgheoCode.Validated += new System.EventHandler(this.txtHoNgheoCode_Validated);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMaHoNgheo});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciMaHoNgheo
            // 
            this.lciMaHoNgheo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMaHoNgheo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMaHoNgheo.Control = this.txtHoNgheoCode;
            this.lciMaHoNgheo.Location = new System.Drawing.Point(0, 0);
            this.lciMaHoNgheo.MaxSize = new System.Drawing.Size(0, 20);
            this.lciMaHoNgheo.MinSize = new System.Drawing.Size(110, 20);
            this.lciMaHoNgheo.Name = "lciMaHoNgheo";
            this.lciMaHoNgheo.OptionsToolTip.ToolTip = "Mã hộ nghèo";
            this.lciMaHoNgheo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciMaHoNgheo.Size = new System.Drawing.Size(219, 24);
            this.lciMaHoNgheo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciMaHoNgheo.Text = "Mã HN:";
            this.lciMaHoNgheo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMaHoNgheo.TextSize = new System.Drawing.Size(70, 20);
            this.lciMaHoNgheo.TextToControlDistance = 5;
            // 
            // UCMaHoNgheo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCMaHoNgheo";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCMaHoNgheo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtHoNgheoCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaHoNgheo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderMaHN)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.TextEdit txtHoNgheoCode;
        private DevExpress.XtraLayout.LayoutControlItem lciMaHoNgheo;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderMaHN;
    }
}
