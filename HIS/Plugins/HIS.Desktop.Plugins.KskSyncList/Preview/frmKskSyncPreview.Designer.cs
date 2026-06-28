namespace HIS.Desktop.Plugins.KskSyncList.Preview
{
    partial class frmKskSyncPreview
    {
        private System.ComponentModel.IContainer components = null;

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
            this.panelHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblStatusValue = new DevExpress.XtraEditors.LabelControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.lblConclusionTimeValue = new DevExpress.XtraEditors.LabelControl();
            this.lblConclusionTime = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientValue = new DevExpress.XtraEditors.LabelControl();
            this.lblPatient = new DevExpress.XtraEditors.LabelControl();
            this.lblKskTypeValue = new DevExpress.XtraEditors.LabelControl();
            this.lblKskType = new DevExpress.XtraEditors.LabelControl();
            this.lblContentTitle = new DevExpress.XtraEditors.LabelControl();
            this.memoContent = new DevExpress.XtraEditors.MemoEdit();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoContent.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // panelHeader
            //
            this.panelHeader.Controls.Add(this.lblStatusValue);
            this.panelHeader.Controls.Add(this.lblStatus);
            this.panelHeader.Controls.Add(this.lblConclusionTimeValue);
            this.panelHeader.Controls.Add(this.lblConclusionTime);
            this.panelHeader.Controls.Add(this.lblPatientValue);
            this.panelHeader.Controls.Add(this.lblPatient);
            this.panelHeader.Controls.Add(this.lblKskTypeValue);
            this.panelHeader.Controls.Add(this.lblKskType);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(584, 70);
            this.panelHeader.TabIndex = 0;
            //
            // lblKskType
            //
            this.lblKskType.Location = new System.Drawing.Point(40, 16);
            this.lblKskType.Name = "lblKskType";
            this.lblKskType.Size = new System.Drawing.Size(44, 13);
            this.lblKskType.TabIndex = 0;
            this.lblKskType.Text = "Loại KSK:";
            //
            // lblKskTypeValue
            //
            this.lblKskTypeValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblKskTypeValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblKskTypeValue.Location = new System.Drawing.Point(120, 16);
            this.lblKskTypeValue.Name = "lblKskTypeValue";
            this.lblKskTypeValue.Size = new System.Drawing.Size(20, 13);
            this.lblKskTypeValue.TabIndex = 1;
            this.lblKskTypeValue.Text = "...";
            //
            // lblPatient
            //
            this.lblPatient.Location = new System.Drawing.Point(360, 16);
            this.lblPatient.Name = "lblPatient";
            this.lblPatient.Size = new System.Drawing.Size(50, 13);
            this.lblPatient.TabIndex = 2;
            this.lblPatient.Text = "Bệnh nhân:";
            //
            // lblPatientValue
            //
            this.lblPatientValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientValue.Location = new System.Drawing.Point(420, 16);
            this.lblPatientValue.Name = "lblPatientValue";
            this.lblPatientValue.Size = new System.Drawing.Size(20, 13);
            this.lblPatientValue.TabIndex = 3;
            this.lblPatientValue.Text = "...";
            //
            // lblConclusionTime
            //
            this.lblConclusionTime.Location = new System.Drawing.Point(40, 42);
            this.lblConclusionTime.Name = "lblConclusionTime";
            this.lblConclusionTime.Size = new System.Drawing.Size(67, 13);
            this.lblConclusionTime.TabIndex = 4;
            this.lblConclusionTime.Text = "Ngày kết luận:";
            //
            // lblConclusionTimeValue
            //
            this.lblConclusionTimeValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblConclusionTimeValue.Location = new System.Drawing.Point(120, 42);
            this.lblConclusionTimeValue.Name = "lblConclusionTimeValue";
            this.lblConclusionTimeValue.Size = new System.Drawing.Size(20, 13);
            this.lblConclusionTimeValue.TabIndex = 5;
            this.lblConclusionTimeValue.Text = "...";
            //
            // lblStatus
            //
            this.lblStatus.Location = new System.Drawing.Point(360, 42);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(41, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Trạng thái:";
            //
            // lblStatusValue
            //
            this.lblStatusValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.Location = new System.Drawing.Point(420, 42);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(20, 13);
            this.lblStatusValue.TabIndex = 7;
            this.lblStatusValue.Text = "...";
            //
            // lblContentTitle
            //
            this.lblContentTitle.Location = new System.Drawing.Point(14, 80);
            this.lblContentTitle.Name = "lblContentTitle";
            this.lblContentTitle.Size = new System.Drawing.Size(199, 13);
            this.lblContentTitle.TabIndex = 1;
            this.lblContentTitle.Text = "Nội dung dữ liệu sẽ gửi (xem trước — minh hoạ):";
            //
            // memoContent
            //
            this.memoContent.Location = new System.Drawing.Point(14, 99);
            this.memoContent.Name = "memoContent";
            this.memoContent.Properties.Appearance.Font = new System.Drawing.Font("Consolas", 9F);
            this.memoContent.Properties.Appearance.Options.UseFont = true;
            this.memoContent.Properties.ReadOnly = true;
            this.memoContent.Size = new System.Drawing.Size(556, 230);
            this.memoContent.TabIndex = 2;
            //
            // lblNote
            //
            this.lblNote.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(110)))), ((int)(((byte)(20)))));
            this.lblNote.Location = new System.Drawing.Point(14, 337);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(556, 26);
            this.lblNote.TabIndex = 3;
            this.lblNote.Text = "⚠ Danh sách trường & định dạng theo từng loại KSK sẽ được chuẩn hoá theo phụ lục hướng dẫn QĐ 1551 (chưa cung cấp).\r\nBản xem trước này chỉ minh hoạ bố cục.";
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(495, 372);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmKskSyncPreview
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 408);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.memoContent);
            this.Controls.Add(this.lblContentTitle);
            this.Controls.Add(this.panelHeader);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "frmKskSyncPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xem dữ liệu sẽ đẩy lên Cổng dữ liệu BYT";
            this.Load += new System.EventHandler(this.frmKskSyncPreview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelHeader)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoContent.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelHeader;
        private DevExpress.XtraEditors.LabelControl lblKskType;
        private DevExpress.XtraEditors.LabelControl lblKskTypeValue;
        private DevExpress.XtraEditors.LabelControl lblPatient;
        private DevExpress.XtraEditors.LabelControl lblPatientValue;
        private DevExpress.XtraEditors.LabelControl lblConclusionTime;
        private DevExpress.XtraEditors.LabelControl lblConclusionTimeValue;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.LabelControl lblStatusValue;
        private DevExpress.XtraEditors.LabelControl lblContentTitle;
        private DevExpress.XtraEditors.MemoEdit memoContent;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
