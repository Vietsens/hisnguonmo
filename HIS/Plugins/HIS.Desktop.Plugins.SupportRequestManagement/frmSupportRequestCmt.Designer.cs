namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    partial class frmSupportRequestCmt
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
            this.lblContentCap = new DevExpress.XtraEditors.LabelControl();
            this.memContent = new DevExpress.XtraEditors.MemoEdit();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblContentCap
            //
            this.lblContentCap.Location = new System.Drawing.Point(14, 14);
            this.lblContentCap.Name = "lblContentCap";
            this.lblContentCap.Size = new System.Drawing.Size(48, 13);
            this.lblContentCap.TabIndex = 0;
            this.lblContentCap.Text = "Nội dung:";
            //
            // memContent
            //
            this.memContent.Location = new System.Drawing.Point(14, 33);
            this.memContent.Name = "memContent";
            this.memContent.Size = new System.Drawing.Size(448, 130);
            this.memContent.TabIndex = 1;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(288, 174);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(94, 24);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Lưu";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(388, 174);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Huỷ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmSupportRequestCmt
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 211);
            this.Controls.Add(this.lblContentCap);
            this.Controls.Add(this.memContent);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSupportRequestCmt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trao đổi";
            this.Load += new System.EventHandler(this.frmSupportRequestCmt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.memContent.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblContentCap;
        private DevExpress.XtraEditors.MemoEdit memContent;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
