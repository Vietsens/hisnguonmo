namespace HIS.Desktop.Plugins.HisPatientPackage
{
    partial class frmHisPatientPackage
    {
        /// <summary> Required designer variable. </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> Clean up any resources being used. </summary>
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
            this.SuspendLayout();
            //
            // frmHisPatientPackage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 650);
            this.MinimumSize = new System.Drawing.Size(820, 480);
            this.Name = "frmHisPatientPackage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách gói dịch vụ bệnh nhân";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmHisPatientPackage_Load);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
