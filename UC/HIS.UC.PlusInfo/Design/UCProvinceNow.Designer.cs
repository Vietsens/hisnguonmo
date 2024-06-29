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
    partial class UCProvinceNow
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
            this.cboProvinceNowName = new DevExpress.XtraEditors.LookUpEdit();
            this.txtProvinceNowCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProvinceNow = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboProvinceNowName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceNowCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvinceNow)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboProvinceNowName);
            this.layoutControl1.Controls.Add(this.txtProvinceNowCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboProvinceNowName
            // 
            this.cboProvinceNowName.Location = new System.Drawing.Point(127, 0);
            this.cboProvinceNowName.Name = "cboProvinceNowName";
            this.cboProvinceNowName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboProvinceNowName.Properties.NullText = "";
            this.cboProvinceNowName.Size = new System.Drawing.Size(92, 20);
            this.cboProvinceNowName.StyleController = this.layoutControl1;
            this.cboProvinceNowName.TabIndex = 1;
            this.cboProvinceNowName.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvinceNowName_Closed);
            this.cboProvinceNowName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboProvinceNowName_KeyUp);
            // 
            // txtProvinceNowCode
            // 
            this.txtProvinceNowCode.Location = new System.Drawing.Point(75, 0);
            this.txtProvinceNowCode.Name = "txtProvinceNowCode";
            this.txtProvinceNowCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtProvinceNowCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtProvinceNowCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProvinceNowCode.Size = new System.Drawing.Size(52, 20);
            this.txtProvinceNowCode.StyleController = this.layoutControl1;
            this.txtProvinceNowCode.TabIndex = 0;
            this.txtProvinceNowCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProvinceNowCode_KeyDown);
            this.txtProvinceNowCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtProvinceNowCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciProvinceNow});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboProvinceNowName;
            this.layoutControlItem1.Location = new System.Drawing.Point(127, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(52, 20);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(92, 24);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciProvinceNow
            // 
            this.lciProvinceNow.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciProvinceNow.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciProvinceNow.Control = this.txtProvinceNowCode;
            this.lciProvinceNow.Location = new System.Drawing.Point(0, 0);
            this.lciProvinceNow.MaxSize = new System.Drawing.Size(0, 20);
            this.lciProvinceNow.MinSize = new System.Drawing.Size(127, 20);
            this.lciProvinceNow.Name = "lciProvinceNow";
            this.lciProvinceNow.OptionsToolTip.ToolTip = "Tỉnh hiện tại";
            this.lciProvinceNow.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciProvinceNow.Size = new System.Drawing.Size(127, 24);
            this.lciProvinceNow.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciProvinceNow.Text = "Tỉnh HT:";
            this.lciProvinceNow.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciProvinceNow.TextSize = new System.Drawing.Size(70, 20);
            this.lciProvinceNow.TextToControlDistance = 5;
            // 
            // UCProvinceNow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCProvinceNow";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCProvince_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboProvinceNowName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceNowCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvinceNow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.LookUpEdit cboProvinceNowName;
        internal DevExpress.XtraEditors.TextEdit txtProvinceNowCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciProvinceNow;
    }
}
