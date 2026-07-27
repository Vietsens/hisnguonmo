namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    partial class frmInfectiousDiseaseSyncList
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "frmInfectiousDiseaseSyncList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đồng bộ danh sách ca bệnh truyền nhiễm — Cổng ECDS";
            this.Load += new System.EventHandler(this.frmInfectiousDiseaseSyncList_Load);
            this.ResumeLayout(false);
        }
    }
}
