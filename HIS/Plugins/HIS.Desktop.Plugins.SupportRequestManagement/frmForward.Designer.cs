namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    partial class frmForward
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
            this.lblCodeCap = new DevExpress.XtraEditors.LabelControl();
            this.lblCode = new DevExpress.XtraEditors.LabelControl();
            this.lblFileCap = new DevExpress.XtraEditors.LabelControl();
            this.lblFile = new DevExpress.XtraEditors.LabelControl();
            this.lblTitleCap = new DevExpress.XtraEditors.LabelControl();
            this.txtTitle = new DevExpress.XtraEditors.TextEdit();
            this.lblContentCap = new DevExpress.XtraEditors.LabelControl();
            this.memContent = new DevExpress.XtraEditors.MemoEdit();
            this.lblReasonCap = new DevExpress.XtraEditors.LabelControl();
            this.memReason = new DevExpress.XtraEditors.MemoEdit();
            this.lblWarn = new DevExpress.XtraEditors.LabelControl();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReason.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblCodeCap
            //
            this.lblCodeCap.Location = new System.Drawing.Point(14, 14);
            this.lblCodeCap.Name = "lblCodeCap";
            this.lblCodeCap.Size = new System.Drawing.Size(58, 13);
            this.lblCodeCap.TabIndex = 0;
            this.lblCodeCap.Text = "Mã yêu cầu:";
            //
            // lblCode
            //
            this.lblCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.Appearance.Options.UseFont = true;
            this.lblCode.Location = new System.Drawing.Point(124, 14);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(0, 13);
            this.lblCode.TabIndex = 1;
            //
            // lblFileCap
            //
            this.lblFileCap.Location = new System.Drawing.Point(320, 14);
            this.lblFileCap.Name = "lblFileCap";
            this.lblFileCap.Size = new System.Drawing.Size(68, 13);
            this.lblFileCap.TabIndex = 2;
            this.lblFileCap.Text = "Tệp đính kèm:";
            //
            // lblFile
            //
            this.lblFile.Location = new System.Drawing.Point(398, 14);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(0, 13);
            this.lblFile.TabIndex = 3;
            //
            // lblTitleCap
            //
            this.lblTitleCap.Location = new System.Drawing.Point(14, 42);
            this.lblTitleCap.Name = "lblTitleCap";
            this.lblTitleCap.Size = new System.Drawing.Size(38, 13);
            this.lblTitleCap.TabIndex = 4;
            this.lblTitleCap.Text = "Tiêu đề:";
            //
            // txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(124, 39);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(420, 20);
            this.txtTitle.TabIndex = 5;
            //
            // lblContentCap
            //
            this.lblContentCap.Location = new System.Drawing.Point(14, 70);
            this.lblContentCap.Name = "lblContentCap";
            this.lblContentCap.Size = new System.Drawing.Size(48, 13);
            this.lblContentCap.TabIndex = 6;
            this.lblContentCap.Text = "Nội dung:";
            //
            // memContent
            //
            this.memContent.Location = new System.Drawing.Point(124, 67);
            this.memContent.Name = "memContent";
            this.memContent.Size = new System.Drawing.Size(420, 120);
            this.memContent.TabIndex = 7;
            //
            // lblReasonCap
            //
            this.lblReasonCap.Location = new System.Drawing.Point(14, 198);
            this.lblReasonCap.Name = "lblReasonCap";
            this.lblReasonCap.Size = new System.Drawing.Size(103, 13);
            this.lblReasonCap.TabIndex = 8;
            this.lblReasonCap.Text = "Lý do chuyển tiếp:";
            //
            // memReason
            //
            this.memReason.Location = new System.Drawing.Point(124, 195);
            this.memReason.Name = "memReason";
            this.memReason.Size = new System.Drawing.Size(420, 58);
            this.memReason.TabIndex = 9;
            //
            // lblWarn
            //
            this.lblWarn.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(75)))), ((int)(((byte)(0)))));
            this.lblWarn.Appearance.Options.UseForeColor = true;
            this.lblWarn.Location = new System.Drawing.Point(14, 263);
            this.lblWarn.Name = "lblWarn";
            this.lblWarn.Size = new System.Drawing.Size(530, 26);
            this.lblWarn.TabIndex = 10;
            this.lblWarn.Text = "Tiêu đề và nội dung sẽ được tạo thành một yêu cầu khách hàng trên hệ thống của côn" +
    "g ty.\r\nSau khi chuyển, yêu cầu chuyển sang trạng thái Đã chuyển công ty và không " +
    "chuyển tiếp được lần nữa.";
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(330, 300);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(120, 24);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "Chuyển công ty";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(456, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 24);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Huỷ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmForward
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 337);
            this.Controls.Add(this.lblCodeCap);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.lblFileCap);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblTitleCap);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblContentCap);
            this.Controls.Add(this.memContent);
            this.Controls.Add(this.lblReasonCap);
            this.Controls.Add(this.memReason);
            this.Controls.Add(this.lblWarn);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmForward";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chuyển yêu cầu lên công ty";
            this.Load += new System.EventHandler(this.frmForward_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReason.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblCodeCap;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraEditors.LabelControl lblFileCap;
        private DevExpress.XtraEditors.LabelControl lblFile;
        private DevExpress.XtraEditors.LabelControl lblTitleCap;
        private DevExpress.XtraEditors.TextEdit txtTitle;
        private DevExpress.XtraEditors.LabelControl lblContentCap;
        private DevExpress.XtraEditors.MemoEdit memContent;
        private DevExpress.XtraEditors.LabelControl lblReasonCap;
        private DevExpress.XtraEditors.MemoEdit memReason;
        private DevExpress.XtraEditors.LabelControl lblWarn;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
