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
namespace Inventec.UC.ListReports.Form
{
    partial class frmUpdate
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
            this.components = new System.ComponentModel.Container();
            this.LayoutReportCode = new DevExpress.XtraEditors.LabelControl();
            this.lblReportCode = new DevExpress.XtraEditors.LabelControl();
            this.LayoutReportStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblReportStatus = new DevExpress.XtraEditors.LabelControl();
            this.LayoutReportName = new DevExpress.XtraEditors.LabelControl();
            this.txtReportName = new DevExpress.XtraEditors.TextEdit();
            this.LayoutReportDescription = new DevExpress.XtraEditors.LabelControl();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtReportName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // LayoutReportCode
            // 
            this.LayoutReportCode.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportCode.Location = new System.Drawing.Point(19, 10);
            this.LayoutReportCode.Name = "LayoutReportCode";
            this.LayoutReportCode.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportCode.TabIndex = 0;
            this.LayoutReportCode.Text = "Mã báo cáo:";
            // 
            // lblReportCode
            // 
            this.lblReportCode.Location = new System.Drawing.Point(95, 10);
            this.lblReportCode.Name = "lblReportCode";
            this.lblReportCode.Size = new System.Drawing.Size(0, 13);
            this.lblReportCode.TabIndex = 1;
            // 
            // LayoutReportStatus
            // 
            this.LayoutReportStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportStatus.Location = new System.Drawing.Point(219, 10);
            this.LayoutReportStatus.Name = "LayoutReportStatus";
            this.LayoutReportStatus.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportStatus.TabIndex = 2;
            this.LayoutReportStatus.Text = "Trạng thái:";
            // 
            // lblReportStatus
            // 
            this.lblReportStatus.Location = new System.Drawing.Point(315, 10);
            this.lblReportStatus.Name = "lblReportStatus";
            this.lblReportStatus.Size = new System.Drawing.Size(0, 13);
            this.lblReportStatus.TabIndex = 3;
            // 
            // LayoutReportName
            // 
            this.LayoutReportName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportName.Location = new System.Drawing.Point(19, 40);
            this.LayoutReportName.Name = "LayoutReportName";
            this.LayoutReportName.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportName.TabIndex = 4;
            this.LayoutReportName.Text = "Tên báo cáo:";
            // 
            // txtReportName
            // 
            this.txtReportName.Location = new System.Drawing.Point(95, 37);
            this.txtReportName.Name = "txtReportName";
            this.txtReportName.Size = new System.Drawing.Size(350, 20);
            this.txtReportName.TabIndex = 5;
            this.txtReportName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtReportName_PreviewKeyDown);
            // 
            // LayoutReportDescription
            // 
            this.LayoutReportDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LayoutReportDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LayoutReportDescription.Location = new System.Drawing.Point(19, 70);
            this.LayoutReportDescription.Name = "LayoutReportDescription";
            this.LayoutReportDescription.Size = new System.Drawing.Size(70, 13);
            this.LayoutReportDescription.TabIndex = 6;
            this.LayoutReportDescription.Text = "Mô tả:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(95, 67);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(350, 120);
            this.txtDescription.TabIndex = 7;
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(335, 193);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 26);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(449, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 227);
            this.barDockControlBottom.Size = new System.Drawing.Size(449, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 198);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(449, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 198);
            // 
            // frmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 227);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.LayoutReportDescription);
            this.Controls.Add(this.txtReportName);
            this.Controls.Add(this.LayoutReportName);
            this.Controls.Add(this.lblReportStatus);
            this.Controls.Add(this.LayoutReportStatus);
            this.Controls.Add(this.lblReportCode);
            this.Controls.Add(this.LayoutReportCode);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa";
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtReportName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl LayoutReportCode;
        private DevExpress.XtraEditors.LabelControl lblReportCode;
        private DevExpress.XtraEditors.LabelControl LayoutReportStatus;
        private DevExpress.XtraEditors.LabelControl lblReportStatus;
        private DevExpress.XtraEditors.LabelControl LayoutReportName;
        private DevExpress.XtraEditors.TextEdit txtReportName;
        private DevExpress.XtraEditors.LabelControl LayoutReportDescription;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
