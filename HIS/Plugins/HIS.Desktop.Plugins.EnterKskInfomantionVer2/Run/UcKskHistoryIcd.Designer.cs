namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    partial class UcKskHistoryIcd
    {
        /// <summary> Required designer variable. </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.txtIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.txtIcdName = new DevExpress.XtraEditors.TextEdit();
            this.btnChooseIcd = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdName.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // txtIcdCode — ô mã ICD (chỉ đọc, ghép dấu ;)
            //
            this.txtIcdCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.txtIcdCode.Location = new System.Drawing.Point(0, 0);
            this.txtIcdCode.Name = "txtIcdCode";
            this.txtIcdCode.Properties.NullValuePrompt = "Mã ICD";
            this.txtIcdCode.Properties.ReadOnly = true;
            this.txtIcdCode.Size = new System.Drawing.Size(90, 20);
            this.txtIcdCode.TabIndex = 0;
            this.txtIcdCode.TabStop = false;
            //
            // txtIcdName — ô tên ICD (chỉ đọc, ghép dấu ;)
            //
            this.txtIcdName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIcdName.Location = new System.Drawing.Point(92, 0);
            this.txtIcdName.Name = "txtIcdName";
            this.txtIcdName.Properties.NullValuePrompt = "Nhấn ... để chọn mã ICD tiền sử";
            this.txtIcdName.Properties.ReadOnly = true;
            this.txtIcdName.Size = new System.Drawing.Size(205, 20);
            this.txtIcdName.TabIndex = 1;
            this.txtIcdName.TabStop = false;
            //
            // btnChooseIcd — nút "..." mở popup chọn nhiều ICD
            //
            this.btnChooseIcd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseIcd.Location = new System.Drawing.Point(299, 0);
            this.btnChooseIcd.Name = "btnChooseIcd";
            this.btnChooseIcd.Size = new System.Drawing.Size(26, 20);
            this.btnChooseIcd.TabIndex = 2;
            this.btnChooseIcd.Text = "...";
            this.btnChooseIcd.ToolTip = "Chọn mã bệnh ICD";
            this.btnChooseIcd.Click += new System.EventHandler(this.btnChooseIcd_Click);
            //
            // UcKskHistoryIcd
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtIcdCode);
            this.Controls.Add(this.txtIcdName);
            this.Controls.Add(this.btnChooseIcd);
            this.Name = "UcKskHistoryIcd";
            this.Size = new System.Drawing.Size(325, 20);
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdName.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtIcdCode;
        private DevExpress.XtraEditors.TextEdit txtIcdName;
        private DevExpress.XtraEditors.SimpleButton btnChooseIcd;
    }
}
