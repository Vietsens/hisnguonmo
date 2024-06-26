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
namespace EMR.Desktop.Plugins.EmrTreatmentList
{
	partial class frmStatusApproval
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
			this.btnEnd = new DevExpress.XtraEditors.SimpleButton();
			this.memResult = new DevExpress.XtraEditors.MemoEdit();
			this.lblProcessName = new DevExpress.XtraEditors.LabelControl();
			this.lblProcessNumber = new DevExpress.XtraEditors.LabelControl();
			this.pbProcess = new System.Windows.Forms.ProgressBar();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.memResult.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.btnEnd);
			this.layoutControl1.Controls.Add(this.memResult);
			this.layoutControl1.Controls.Add(this.lblProcessName);
			this.layoutControl1.Controls.Add(this.lblProcessNumber);
			this.layoutControl1.Controls.Add(this.pbProcess);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(470, 218);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			// 
			// btnEnd
			// 
			this.btnEnd.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnEnd.Location = new System.Drawing.Point(347, 194);
			this.btnEnd.Name = "btnEnd";
			this.btnEnd.Size = new System.Drawing.Size(121, 22);
			this.btnEnd.StyleController = this.layoutControl1;
			this.btnEnd.TabIndex = 8;
			this.btnEnd.Text = "Kết thúc";
			this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
			// 
			// memResult
			// 
			this.memResult.Location = new System.Drawing.Point(2, 74);
			this.memResult.Name = "memResult";
			this.memResult.Properties.ReadOnly = true;
			this.memResult.Size = new System.Drawing.Size(466, 116);
			this.memResult.StyleController = this.layoutControl1;
			this.memResult.TabIndex = 7;
			// 
			// lblProcessName
			// 
			this.lblProcessName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblProcessName.Location = new System.Drawing.Point(57, 50);
			this.lblProcessName.Name = "lblProcessName";
			this.lblProcessName.Size = new System.Drawing.Size(411, 20);
			this.lblProcessName.StyleController = this.layoutControl1;
			this.lblProcessName.TabIndex = 6;
			// 
			// lblProcessNumber
			// 
			this.lblProcessNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblProcessNumber.Location = new System.Drawing.Point(57, 26);
			this.lblProcessNumber.Name = "lblProcessNumber";
			this.lblProcessNumber.Size = new System.Drawing.Size(411, 20);
			this.lblProcessNumber.StyleController = this.layoutControl1;
			this.lblProcessNumber.TabIndex = 5;
			// 
			// pbProcess
			// 
			this.pbProcess.Location = new System.Drawing.Point(2, 2);
			this.pbProcess.Name = "pbProcess";
			this.pbProcess.Size = new System.Drawing.Size(466, 20);
			this.pbProcess.TabIndex = 4;
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(470, 218);
			this.layoutControlGroup1.TextVisible = false;
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.Control = this.pbProcess;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(470, 24);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.lblProcessNumber;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(470, 24);
			this.layoutControlItem2.Text = "Đã xử lý:";
			this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(50, 20);
			this.layoutControlItem2.TextToControlDistance = 5;
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.lblProcessName;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(470, 24);
			this.layoutControlItem3.Text = "Đang ký:";
			this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(50, 20);
			this.layoutControlItem3.TextToControlDistance = 5;
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.memResult;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 72);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(470, 120);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			// 
			// layoutControlItem5
			// 
			this.layoutControlItem5.Control = this.btnEnd;
			this.layoutControlItem5.Location = new System.Drawing.Point(345, 192);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(125, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			// 
			// emptySpaceItem1
			// 
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 192);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(345, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			// 
			// frmStatusApproval
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(470, 218);
			this.Controls.Add(this.layoutControl1);
			this.Name = "frmStatusApproval";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Trạng thái ký duyệt hồ sơ";
			this.Load += new System.EventHandler(this.frmStatusApproval_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.memResult.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraLayout.LayoutControl layoutControl1;
		private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraEditors.SimpleButton btnEnd;
		private DevExpress.XtraEditors.MemoEdit memResult;
		private DevExpress.XtraEditors.LabelControl lblProcessName;
		private DevExpress.XtraEditors.LabelControl lblProcessNumber;
		private System.Windows.Forms.ProgressBar pbProcess;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
	}
}
