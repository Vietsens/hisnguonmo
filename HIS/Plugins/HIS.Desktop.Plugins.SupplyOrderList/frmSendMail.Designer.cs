namespace HIS.Desktop.Plugins.SupplyOrderList
{
    partial class frmSendMail
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
            this.panelTop = new DevExpress.XtraEditors.PanelControl();
            this.lblBody = new DevExpress.XtraEditors.LabelControl();
            this.txtSubject = new DevExpress.XtraEditors.TextEdit();
            this.lblSubject = new DevExpress.XtraEditors.LabelControl();
            this.txtCc = new DevExpress.XtraEditors.TextEdit();
            this.lblCc = new DevExpress.XtraEditors.LabelControl();
            this.txtTo = new DevExpress.XtraEditors.TextEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.memoBody = new DevExpress.XtraEditors.MemoEdit();
            this.panelButton = new DevExpress.XtraEditors.PanelControl();
            this.lblHint = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSend = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubject.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoBody.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).BeginInit();
            this.panelButton.SuspendLayout();
            this.SuspendLayout();
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblBody);
            this.panelTop.Controls.Add(this.txtSubject);
            this.panelTop.Controls.Add(this.lblSubject);
            this.panelTop.Controls.Add(this.txtCc);
            this.panelTop.Controls.Add(this.lblCc);
            this.panelTop.Controls.Add(this.txtTo);
            this.panelTop.Controls.Add(this.lblTo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(640, 104);
            this.panelTop.TabIndex = 0;
            //
            // lblTo
            //
            this.lblTo.Location = new System.Drawing.Point(8, 11);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(60, 13);
            this.lblTo.TabIndex = 0;
            this.lblTo.Text = "Gui den:";
            //
            // txtTo
            //
            this.txtTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTo.Location = new System.Drawing.Point(84, 8);
            this.txtTo.Name = "txtTo";
            this.txtTo.Properties.MaxLength = 1000;
            this.txtTo.Size = new System.Drawing.Size(548, 20);
            this.txtTo.TabIndex = 1;
            //
            // lblCc
            //
            this.lblCc.Location = new System.Drawing.Point(8, 37);
            this.lblCc.Name = "lblCc";
            this.lblCc.Size = new System.Drawing.Size(60, 13);
            this.lblCc.TabIndex = 2;
            this.lblCc.Text = "CC:";
            //
            // txtCc
            //
            this.txtCc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCc.Location = new System.Drawing.Point(84, 34);
            this.txtCc.Name = "txtCc";
            this.txtCc.Properties.MaxLength = 1000;
            this.txtCc.Size = new System.Drawing.Size(548, 20);
            this.txtCc.TabIndex = 3;
            //
            // lblSubject
            //
            this.lblSubject.Location = new System.Drawing.Point(8, 63);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(60, 13);
            this.lblSubject.TabIndex = 4;
            this.lblSubject.Text = "Tieu de:";
            //
            // txtSubject
            //
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(84, 60);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Properties.MaxLength = 500;
            this.txtSubject.Size = new System.Drawing.Size(548, 20);
            this.txtSubject.TabIndex = 5;
            //
            // lblBody
            //
            this.lblBody.Location = new System.Drawing.Point(8, 86);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(60, 13);
            this.lblBody.TabIndex = 6;
            this.lblBody.Text = "Noi dung:";
            //
            // memoBody
            //
            this.memoBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoBody.Location = new System.Drawing.Point(0, 104);
            this.memoBody.Name = "memoBody";
            this.memoBody.Size = new System.Drawing.Size(640, 296);
            this.memoBody.TabIndex = 1;
            //
            // panelButton
            //
            this.panelButton.Controls.Add(this.lblHint);
            this.panelButton.Controls.Add(this.btnCancel);
            this.panelButton.Controls.Add(this.btnSend);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButton.Location = new System.Drawing.Point(0, 400);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(640, 40);
            this.panelButton.TabIndex = 2;
            //
            // lblHint
            //
            this.lblHint.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblHint.Location = new System.Drawing.Point(8, 13);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(300, 13);
            this.lblHint.TabIndex = 0;
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(534, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 22);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Dong";
            //
            // btnSend
            //
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(428, 9);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 22);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Gui";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            //
            // frmSendMail
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(640, 440);
            this.Controls.Add(this.memoBody);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSendMail";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gui email don dat hang";
            this.Load += new System.EventHandler(this.frmSendMail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSubject.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoBody.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).EndInit();
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelTop;
        private DevExpress.XtraEditors.LabelControl lblBody;
        private DevExpress.XtraEditors.TextEdit txtSubject;
        private DevExpress.XtraEditors.LabelControl lblSubject;
        private DevExpress.XtraEditors.TextEdit txtCc;
        private DevExpress.XtraEditors.LabelControl lblCc;
        private DevExpress.XtraEditors.TextEdit txtTo;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.MemoEdit memoBody;
        private DevExpress.XtraEditors.PanelControl panelButton;
        private DevExpress.XtraEditors.LabelControl lblHint;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnSend;
    }
}
