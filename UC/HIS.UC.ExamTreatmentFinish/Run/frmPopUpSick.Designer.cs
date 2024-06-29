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
namespace HIS.UC.ExamTreatmentFinish.Run
{
	partial class frmPopUpSick
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraScrollableControl3 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.xtraScrollableControl2 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.btnOke = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciOther = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciDeath = new DevExpress.XtraLayout.LayoutControlItem();
            this.timer1 = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtraTabPage1.SuspendLayout();
            this.xtraTabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDeath)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.xtraTabControl1);
            this.layoutControl1.Controls.Add(this.xtraScrollableControl1);
            this.layoutControl1.Controls.Add(this.btnOke);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(534, 357);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
            this.xtraTabControl1.Size = new System.Drawing.Size(534, 133);
            this.xtraTabControl1.TabIndex = 6;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtraTabPage1,
            this.xtraTabPage2});
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.xtraScrollableControl3);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(528, 105);
            this.xtraTabPage1.Text = "Thông tin chung";
            // 
            // xtraScrollableControl3
            // 
            this.xtraScrollableControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl3.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl3.Name = "xtraScrollableControl3";
            this.xtraScrollableControl3.Size = new System.Drawing.Size(528, 105);
            this.xtraScrollableControl3.TabIndex = 0;
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Controls.Add(this.xtraScrollableControl2);
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.Size = new System.Drawing.Size(528, 110);
            this.xtraTabPage2.Text = "Thông tin chi tiết";
            // 
            // xtraScrollableControl2
            // 
            this.xtraScrollableControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl2.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl2.Name = "xtraScrollableControl2";
            this.xtraScrollableControl2.Size = new System.Drawing.Size(528, 110);
            this.xtraScrollableControl2.TabIndex = 0;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 135);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(530, 194);
            this.xtraScrollableControl1.TabIndex = 5;
            // 
            // btnOke
            // 
            this.btnOke.Location = new System.Drawing.Point(462, 333);
            this.btnOke.Name = "btnOke";
            this.btnOke.Size = new System.Drawing.Size(70, 22);
            this.btnOke.StyleController = this.layoutControl1;
            this.btnOke.TabIndex = 4;
            this.btnOke.Text = "Đồng ý";
            this.btnOke.Click += new System.EventHandler(this.btnOke_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciOther,
            this.emptySpaceItem1,
            this.lciDeath});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(534, 357);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnOke;
            this.layoutControlItem1.Location = new System.Drawing.Point(460, 331);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(74, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciOther
            // 
            this.lciOther.Control = this.xtraScrollableControl1;
            this.lciOther.Location = new System.Drawing.Point(0, 133);
            this.lciOther.Name = "lciOther";
            this.lciOther.Size = new System.Drawing.Size(534, 198);
            this.lciOther.TextSize = new System.Drawing.Size(0, 0);
            this.lciOther.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 331);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(460, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciDeath
            // 
            this.lciDeath.Control = this.xtraTabControl1;
            this.lciDeath.Location = new System.Drawing.Point(0, 0);
            this.lciDeath.Name = "lciDeath";
            this.lciDeath.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciDeath.Size = new System.Drawing.Size(534, 133);
            this.lciDeath.TextSize = new System.Drawing.Size(0, 0);
            this.lciDeath.TextVisible = false;
            // 
            // frmPopUpSick
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 357);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmPopUpSick";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông tin";
            this.Load += new System.EventHandler(this.frmPopUpSick_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtraTabPage1.ResumeLayout(false);
            this.xtraTabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciOther)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDeath)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
		private DevExpress.XtraEditors.SimpleButton btnOke;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem lciOther;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl3;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl2;
        private DevExpress.XtraLayout.LayoutControlItem lciDeath;
    }
}
