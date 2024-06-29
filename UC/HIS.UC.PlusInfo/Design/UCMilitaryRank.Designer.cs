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
    partial class UCMilitaryRank
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
            this.txtMilitaryRankCode = new DevExpress.XtraEditors.TextEdit();
            this.cboMilitaryRank = new DevExpress.XtraEditors.LookUpEdit();
            this.lcgMilitaryRank = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMilitaryRankCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMilitaryRank.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMilitaryRank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtMilitaryRankCode);
            this.layoutControl1.Controls.Add(this.cboMilitaryRank);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.lcgMilitaryRank;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtMilitaryRankCode
            // 
            this.txtMilitaryRankCode.Location = new System.Drawing.Point(75, 0);
            this.txtMilitaryRankCode.Name = "txtMilitaryRankCode";
            this.txtMilitaryRankCode.Size = new System.Drawing.Size(52, 20);
            this.txtMilitaryRankCode.StyleController = this.layoutControl1;
            this.txtMilitaryRankCode.TabIndex = 0;
            this.txtMilitaryRankCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMilitaryRankCode_PreviewKeyDown);
            // 
            // cboMilitaryRank
            // 
            this.cboMilitaryRank.Location = new System.Drawing.Point(127, 0);
            this.cboMilitaryRank.Name = "cboMilitaryRank";
            this.cboMilitaryRank.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboMilitaryRank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMilitaryRank.Properties.NullText = "";
            this.cboMilitaryRank.Size = new System.Drawing.Size(92, 20);
            this.cboMilitaryRank.StyleController = this.layoutControl1;
            this.cboMilitaryRank.TabIndex = 1;
            this.cboMilitaryRank.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboMilitaryRank_Closed);
            this.cboMilitaryRank.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboMilitaryRank_KeyDown);
            this.cboMilitaryRank.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboMilitaryRank_KeyUp);
            // 
            // lcgMilitaryRank
            // 
            this.lcgMilitaryRank.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgMilitaryRank.GroupBordersVisible = false;
            this.lcgMilitaryRank.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.lcgMilitaryRank.Location = new System.Drawing.Point(0, 0);
            this.lcgMilitaryRank.Name = "lcgMilitaryRank";
            this.lcgMilitaryRank.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgMilitaryRank.Size = new System.Drawing.Size(219, 24);
            this.lcgMilitaryRank.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboMilitaryRank;
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
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.txtMilitaryRankCode;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(127, 20);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(127, 24);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "Quân hàm:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(70, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // UCMilitaryRank
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCMilitaryRank";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCMilitaryRank_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMilitaryRankCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMilitaryRank.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgMilitaryRank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgMilitaryRank;
        internal DevExpress.XtraEditors.TextEdit txtMilitaryRankCode;
        internal DevExpress.XtraEditors.LookUpEdit cboMilitaryRank;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
