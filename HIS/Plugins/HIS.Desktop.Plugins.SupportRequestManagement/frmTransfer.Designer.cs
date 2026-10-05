namespace HIS.Desktop.Plugins.SupportRequestManagement
{
    partial class frmTransfer
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
            this.lblHolderCap = new DevExpress.XtraEditors.LabelControl();
            this.lblHolder = new DevExpress.XtraEditors.LabelControl();
            this.lblTargetCap = new DevExpress.XtraEditors.LabelControl();
            this.cboTarget = new DevExpress.XtraEditors.GridLookUpEdit();
            this.lblReasonCap = new DevExpress.XtraEditors.LabelControl();
            this.memReason = new DevExpress.XtraEditors.MemoEdit();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.cboTarget.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReason.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblCodeCap
            //
            this.lblCodeCap.Location = new System.Drawing.Point(14, 16);
            this.lblCodeCap.Name = "lblCodeCap";
            this.lblCodeCap.Size = new System.Drawing.Size(58, 13);
            this.lblCodeCap.TabIndex = 0;
            this.lblCodeCap.Text = "Mã yêu cầu:";
            //
            // lblCode
            //
            this.lblCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.Appearance.Options.UseFont = true;
            this.lblCode.Location = new System.Drawing.Point(132, 16);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(0, 13);
            this.lblCode.TabIndex = 1;
            //
            // lblHolderCap
            //
            this.lblHolderCap.Location = new System.Drawing.Point(14, 40);
            this.lblHolderCap.Name = "lblHolderCap";
            this.lblHolderCap.Size = new System.Drawing.Size(94, 13);
            this.lblHolderCap.TabIndex = 2;
            this.lblHolderCap.Text = "Người đang giữ việc:";
            //
            // lblHolder
            //
            this.lblHolder.Location = new System.Drawing.Point(132, 40);
            this.lblHolder.Name = "lblHolder";
            this.lblHolder.Size = new System.Drawing.Size(0, 13);
            this.lblHolder.TabIndex = 3;
            //
            // lblTargetCap
            //
            this.lblTargetCap.Location = new System.Drawing.Point(14, 68);
            this.lblTargetCap.Name = "lblTargetCap";
            this.lblTargetCap.Size = new System.Drawing.Size(81, 13);
            this.lblTargetCap.TabIndex = 4;
            this.lblTargetCap.Text = "Chuyển sang:";
            //
            // cboTarget
            //
            this.cboTarget.Location = new System.Drawing.Point(132, 65);
            this.cboTarget.Name = "cboTarget";
            this.cboTarget.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTarget.Properties.NullText = "";
            this.cboTarget.Size = new System.Drawing.Size(330, 20);
            this.cboTarget.TabIndex = 5;
            this.cboTarget.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTarget_ButtonClick);
            //
            // lblReasonCap
            //
            this.lblReasonCap.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblReasonCap.Appearance.Options.UseForeColor = true;
            this.lblReasonCap.Location = new System.Drawing.Point(14, 96);
            this.lblReasonCap.Name = "lblReasonCap";
            this.lblReasonCap.Size = new System.Drawing.Size(100, 13);
            this.lblReasonCap.TabIndex = 6;
            this.lblReasonCap.Text = "Lý do chuyển việc:";
            //
            // memReason
            //
            this.memReason.Location = new System.Drawing.Point(132, 93);
            this.memReason.Name = "memReason";
            this.memReason.Size = new System.Drawing.Size(330, 90);
            this.memReason.TabIndex = 7;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(282, 196);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 24);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Chuyển việc";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(388, 196);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Huỷ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmTransfer
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 233);
            this.Controls.Add(this.lblCodeCap);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.lblHolderCap);
            this.Controls.Add(this.lblHolder);
            this.Controls.Add(this.lblTargetCap);
            this.Controls.Add(this.cboTarget);
            this.Controls.Add(this.lblReasonCap);
            this.Controls.Add(this.memReason);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chuyển việc";
            this.Load += new System.EventHandler(this.frmTransfer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cboTarget.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memReason.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblCodeCap;
        private DevExpress.XtraEditors.LabelControl lblCode;
        private DevExpress.XtraEditors.LabelControl lblHolderCap;
        private DevExpress.XtraEditors.LabelControl lblHolder;
        private DevExpress.XtraEditors.LabelControl lblTargetCap;
        private DevExpress.XtraEditors.GridLookUpEdit cboTarget;
        private DevExpress.XtraEditors.LabelControl lblReasonCap;
        private DevExpress.XtraEditors.MemoEdit memReason;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
