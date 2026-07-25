namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    partial class UcKskConclusionIcd
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
            this.grpIcd = new DevExpress.XtraEditors.GroupControl();
            this.pnlSecondaryIcd = new DevExpress.XtraEditors.PanelControl();
            this.btnChooseIcd = new DevExpress.XtraEditors.SimpleButton();
            this.rdoIcdConclusion = new DevExpress.XtraEditors.RadioGroup();
            ((System.ComponentModel.ISupportInitialize)(this.grpIcd)).BeginInit();
            this.grpIcd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSecondaryIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoIcdConclusion.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grpIcd
            // 
            this.grpIcd.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.grpIcd.AppearanceCaption.Options.UseFont = true;
            this.grpIcd.Controls.Add(this.pnlSecondaryIcd);
            this.grpIcd.Controls.Add(this.btnChooseIcd);
            this.grpIcd.Controls.Add(this.rdoIcdConclusion);
            this.grpIcd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpIcd.Location = new System.Drawing.Point(0, 0);
            this.grpIcd.Name = "grpIcd";
            this.grpIcd.Size = new System.Drawing.Size(391, 92);
            this.grpIcd.TabIndex = 0;
            this.grpIcd.Text = "Kết luận theo bệnh (ICD - 10)";
            // 
            // pnlSecondaryIcd
            // 
            this.pnlSecondaryIcd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSecondaryIcd.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlSecondaryIcd.Location = new System.Drawing.Point(6, 62);
            this.pnlSecondaryIcd.Name = "pnlSecondaryIcd";
            this.pnlSecondaryIcd.Size = new System.Drawing.Size(345, 24);
            this.pnlSecondaryIcd.TabIndex = 1;
            // 
            // btnChooseIcd
            // 
            this.btnChooseIcd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseIcd.Location = new System.Drawing.Point(355, 62);
            this.btnChooseIcd.Name = "btnChooseIcd";
            this.btnChooseIcd.Size = new System.Drawing.Size(30, 24);
            this.btnChooseIcd.TabIndex = 2;
            this.btnChooseIcd.Text = "...";
            this.btnChooseIcd.Click += new System.EventHandler(this.btnChooseIcd_Click);
            // 
            // rdoIcdConclusion
            // 
            this.rdoIcdConclusion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rdoIcdConclusion.Location = new System.Drawing.Point(6, 26);
            this.rdoIcdConclusion.Name = "rdoIcdConclusion";
            this.rdoIcdConclusion.Properties.Columns = 3;
            this.rdoIcdConclusion.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((long)(1)), "Chưa phát hiện bất thường"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((long)(2)), "Chẩn đoán sơ bộ, ghi rõ theo mã ICD"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(((long)(3)), "Chẩn đoán xác định, ghi rõ theo mã ICD")});
            this.rdoIcdConclusion.Size = new System.Drawing.Size(379, 30);
            this.rdoIcdConclusion.TabIndex = 0;
            // 
            // UcKskConclusionIcd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpIcd);
            this.Name = "UcKskConclusionIcd";
            this.Size = new System.Drawing.Size(391, 92);
            ((System.ComponentModel.ISupportInitialize)(this.grpIcd)).EndInit();
            this.grpIcd.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlSecondaryIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoIcdConclusion.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpIcd;
        private DevExpress.XtraEditors.RadioGroup rdoIcdConclusion;
        private DevExpress.XtraEditors.PanelControl pnlSecondaryIcd;
        private DevExpress.XtraEditors.SimpleButton btnChooseIcd;
    }
}
