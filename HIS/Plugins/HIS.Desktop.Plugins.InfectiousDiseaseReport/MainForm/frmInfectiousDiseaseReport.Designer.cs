namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    partial class frmInfectiousDiseaseReport
    {
        private System.ComponentModel.IContainer components = null;

        // Toàn bộ control được dựng trong __BuildUi.cs (BuildUi) và khai báo ở frmInfectiousDiseaseReport.cs.
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // frmInfectiousDiseaseReport
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 640);
            this.MinimumSize = new System.Drawing.Size(760, 520);
            this.Name = "frmInfectiousDiseaseReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Báo cáo ca bệnh truyền nhiễm — Cổng giám sát quốc gia (ECDS)";
            this.Load += new System.EventHandler(this.frmInfectiousDiseaseReport_Load);
            this.ResumeLayout(false);
        }
    }
}
