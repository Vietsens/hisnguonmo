namespace HIS.Desktop.Plugins.FundAllocation
{
    partial class frmCancelReason
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
            this.lblReason = new DevExpress.XtraEditors.LabelControl();
            this.memoReason = new DevExpress.XtraEditors.MemoEdit();
            this.panelButton = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.memoReason.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).BeginInit();
            this.panelButton.SuspendLayout();
            this.SuspendLayout();
            //
            // lblReason
            //
            this.lblReason.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblReason.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblReason.Location = new System.Drawing.Point(0, 0);
            this.lblReason.Name = "lblReason";
            this.lblReason.Padding = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.lblReason.Size = new System.Drawing.Size(440, 24);
            this.lblReason.TabIndex = 0;
            this.lblReason.Text = "Lý do hủy phân bổ:";
            //
            // memoReason
            //
            this.memoReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoReason.Location = new System.Drawing.Point(0, 24);
            this.memoReason.Name = "memoReason";
            this.memoReason.Properties.MaxLength = 2000;
            this.memoReason.Size = new System.Drawing.Size(440, 100);
            this.memoReason.TabIndex = 1;
            //
            // panelButton
            //
            this.panelButton.Controls.Add(this.btnCancel);
            this.panelButton.Controls.Add(this.btnOk);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButton.Location = new System.Drawing.Point(0, 124);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(440, 36);
            this.panelButton.TabIndex = 2;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(334, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 22);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Đóng";
            //
            // btnOk
            //
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(228, 7);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 22);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Đồng ý";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // frmCancelReason
            //
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(440, 160);
            this.Controls.Add(this.memoReason);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.panelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCancelReason";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hủy phân bổ";
            this.Load += new System.EventHandler(this.frmCancelReason_Load);
            ((System.ComponentModel.ISupportInitialize)(this.memoReason.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).EndInit();
            this.panelButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblReason;
        private DevExpress.XtraEditors.MemoEdit memoReason;
        private DevExpress.XtraEditors.PanelControl panelButton;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
    }
}
