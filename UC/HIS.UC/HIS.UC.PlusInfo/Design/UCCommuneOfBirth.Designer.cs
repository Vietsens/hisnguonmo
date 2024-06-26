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
    partial class UCCommuneOfBirth
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
            this.cboCommuneNowName = new DevExpress.XtraEditors.LookUpEdit();
            this.txtCommuneNowCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCommuneNow = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommuneNowName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommuneNowCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCommuneNow)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboCommuneNowName);
            this.layoutControl1.Controls.Add(this.txtCommuneNowCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboCommuneNowName
            // 
            this.cboCommuneNowName.Location = new System.Drawing.Point(127, 0);
            this.cboCommuneNowName.Name = "cboCommuneNowName";
            this.cboCommuneNowName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboCommuneNowName.Properties.NullText = "";
            this.cboCommuneNowName.Size = new System.Drawing.Size(92, 20);
            this.cboCommuneNowName.StyleController = this.layoutControl1;
            this.cboCommuneNowName.TabIndex = 1;
            this.cboCommuneNowName.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCommuneNowName_Closed);
            this.cboCommuneNowName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboCommuneNowName_KeyDown);
            this.cboCommuneNowName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboCommuneNowName_KeyUp);
            // 
            // txtCommuneNowCode
            // 
            this.txtCommuneNowCode.Location = new System.Drawing.Point(75, 0);
            this.txtCommuneNowCode.Name = "txtCommuneNowCode";
            this.txtCommuneNowCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtCommuneNowCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtCommuneNowCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCommuneNowCode.Size = new System.Drawing.Size(52, 20);
            this.txtCommuneNowCode.StyleController = this.layoutControl1;
            this.txtCommuneNowCode.TabIndex = 0;
            this.txtCommuneNowCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCommuneNowCode_KeyDown);
            this.txtCommuneNowCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCommuneNowCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciCommuneNow});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboCommuneNowName;
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
            // lciCommuneNow
            // 
            this.lciCommuneNow.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCommuneNow.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCommuneNow.Control = this.txtCommuneNowCode;
            this.lciCommuneNow.Location = new System.Drawing.Point(0, 0);
            this.lciCommuneNow.MaxSize = new System.Drawing.Size(0, 20);
            this.lciCommuneNow.MinSize = new System.Drawing.Size(125, 20);
            this.lciCommuneNow.Name = "lciCommuneNow";
            this.lciCommuneNow.OptionsToolTip.ToolTip = "Xã khai sinh";
            this.lciCommuneNow.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciCommuneNow.Size = new System.Drawing.Size(127, 24);
            this.lciCommuneNow.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCommuneNow.Text = "Xã KS:";
            this.lciCommuneNow.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCommuneNow.TextSize = new System.Drawing.Size(70, 20);
            this.lciCommuneNow.TextToControlDistance = 5;
            // 
            // UCCommuneOfBirth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCCommuneOfBirth";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCCommuneNow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboCommuneNowName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommuneNowCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCommuneNow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.LookUpEdit cboCommuneNowName;
        internal DevExpress.XtraEditors.TextEdit txtCommuneNowCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciCommuneNow;
    }
}
