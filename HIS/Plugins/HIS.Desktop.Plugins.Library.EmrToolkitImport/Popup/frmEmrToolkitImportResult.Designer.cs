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
namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup
{
    partial class frmEmrToolkitImportResult
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelTop = new DevExpress.XtraEditors.PanelControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.picStatus = new DevExpress.XtraEditors.PictureEdit();
            this.panelInfo = new DevExpress.XtraEditors.PanelControl();
            this.memInfo = new DevExpress.XtraEditors.MemoEdit();
            this.tabResult = new DevExpress.XtraTab.XtraTabControl();
            this.tabReceived = new DevExpress.XtraTab.XtraTabPage();
            this.memReceived = new DevExpress.XtraEditors.MemoEdit();
            this.tabSent = new DevExpress.XtraTab.XtraTabPage();
            this.memSent = new DevExpress.XtraEditors.MemoEdit();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnCopy = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelInfo)).BeginInit();
            this.panelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memInfo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabResult)).BeginInit();
            this.tabResult.SuspendLayout();
            this.tabReceived.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memReceived.Properties)).BeginInit();
            this.tabSent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memSent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // tabResult
            //
            this.tabResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabResult.Location = new System.Drawing.Point(0, 122);
            this.tabResult.Name = "tabResult";
            this.tabResult.SelectedTabPage = this.tabReceived;
            this.tabResult.Size = new System.Drawing.Size(744, 396);
            this.tabResult.TabIndex = 2;
            this.tabResult.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabReceived,
            this.tabSent});
            //
            // tabReceived
            //
            this.tabReceived.Controls.Add(this.memReceived);
            this.tabReceived.Name = "tabReceived";
            this.tabReceived.Size = new System.Drawing.Size(738, 368);
            this.tabReceived.Text = "JSON nhận về";
            //
            // memReceived
            //
            this.memReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memReceived.Location = new System.Drawing.Point(0, 0);
            this.memReceived.Name = "memReceived";
            this.memReceived.Properties.Appearance.Font = new System.Drawing.Font("Consolas", 9F);
            this.memReceived.Properties.Appearance.Options.UseFont = true;
            this.memReceived.Properties.ReadOnly = true;
            this.memReceived.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.memReceived.Size = new System.Drawing.Size(738, 368);
            this.memReceived.TabIndex = 0;
            //
            // tabSent
            //
            this.tabSent.Controls.Add(this.memSent);
            this.tabSent.Name = "tabSent";
            this.tabSent.Size = new System.Drawing.Size(738, 368);
            this.tabSent.Text = "JSON đã gửi";
            //
            // memSent
            //
            this.memSent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memSent.Location = new System.Drawing.Point(0, 0);
            this.memSent.Name = "memSent";
            this.memSent.Properties.Appearance.Font = new System.Drawing.Font("Consolas", 9F);
            this.memSent.Properties.Appearance.Options.UseFont = true;
            this.memSent.Properties.ReadOnly = true;
            this.memSent.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.memSent.Size = new System.Drawing.Size(738, 368);
            this.memSent.TabIndex = 0;
            //
            // panelInfo
            //
            this.panelInfo.Controls.Add(this.memInfo);
            this.panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelInfo.Location = new System.Drawing.Point(0, 56);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Padding = new System.Windows.Forms.Padding(8, 2, 8, 4);
            this.panelInfo.Size = new System.Drawing.Size(744, 66);
            this.panelInfo.TabIndex = 1;
            //
            // memInfo
            //
            this.memInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memInfo.Location = new System.Drawing.Point(10, 4);
            this.memInfo.Name = "memInfo";
            this.memInfo.Properties.ReadOnly = true;
            this.memInfo.Size = new System.Drawing.Size(724, 56);
            this.memInfo.TabIndex = 0;
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblStatus);
            this.panelTop.Controls.Add(this.picStatus);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(744, 56);
            this.panelTop.TabIndex = 0;
            //
            // picStatus
            //
            this.picStatus.Location = new System.Drawing.Point(12, 11);
            this.picStatus.Name = "picStatus";
            this.picStatus.Properties.AllowFocused = false;
            this.picStatus.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.picStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picStatus.Properties.ShowMenu = false;
            this.picStatus.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            this.picStatus.Size = new System.Drawing.Size(32, 32);
            this.picStatus.TabIndex = 0;
            //
            // lblStatus
            //
            this.lblStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(54, 18);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(53, 19);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "...";
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnCopy);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 518);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(744, 44);
            this.panelBottom.TabIndex = 3;
            //
            // btnCopy
            //
            this.btnCopy.Location = new System.Drawing.Point(515, 10);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(120, 24);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Sao chép JSON";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(641, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmEmrToolkitImportResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 562);
            this.Controls.Add(this.tabResult);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.MinimumSize = new System.Drawing.Size(560, 360);
            this.Name = "frmEmrToolkitImportResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết quả gửi dữ liệu qua EMRTOOLKIT";
            this.Load += new System.EventHandler(this.frmEmrToolkitImportResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelInfo)).EndInit();
            this.panelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memInfo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabResult)).EndInit();
            this.tabResult.ResumeLayout(false);
            this.tabReceived.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memReceived.Properties)).EndInit();
            this.tabSent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memSent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelTop;
        private DevExpress.XtraEditors.PictureEdit picStatus;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.PanelControl panelInfo;
        private DevExpress.XtraEditors.MemoEdit memInfo;
        private DevExpress.XtraTab.XtraTabControl tabResult;
        private DevExpress.XtraTab.XtraTabPage tabReceived;
        private DevExpress.XtraEditors.MemoEdit memReceived;
        private DevExpress.XtraTab.XtraTabPage tabSent;
        private DevExpress.XtraEditors.MemoEdit memSent;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.SimpleButton btnCopy;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
