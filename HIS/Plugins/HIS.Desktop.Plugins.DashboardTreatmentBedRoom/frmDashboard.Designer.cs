namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom
{
    partial class frmDashboard
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
            this.components = new System.ComponentModel.Container();
            this.ucBoard = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.UcInpatientBoard();
            this.lblEscHint = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            //
            // ucBoard
            //
            this.ucBoard.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.ucBoard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBoard.Location = new System.Drawing.Point(0, 0);
            this.ucBoard.Name = "ucBoard";
            this.ucBoard.Padding = new System.Windows.Forms.Padding(12);
            this.ucBoard.RefreshIntervalSecond = 0;
            this.ucBoard.Size = new System.Drawing.Size(1504, 838);
            this.ucBoard.TabIndex = 0;
            //
            // lblEscHint
            //
            this.lblEscHint.Appearance.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.lblEscHint.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblEscHint.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblEscHint.Appearance.Options.UseBackColor = true;
            this.lblEscHint.Appearance.Options.UseFont = true;
            this.lblEscHint.Appearance.Options.UseForeColor = true;
            this.lblEscHint.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblEscHint.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblEscHint.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEscHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblEscHint.Location = new System.Drawing.Point(0, 838);
            this.lblEscHint.Name = "lblEscHint";
            this.lblEscHint.Padding = new System.Windows.Forms.Padding(14, 0, 14, 0);
            this.lblEscHint.Size = new System.Drawing.Size(1504, 22);
            this.lblEscHint.TabIndex = 1;
            this.lblEscHint.Text = "Nhấn ESC để đóng";
            //
            // frmDashboard
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.ClientSize = new System.Drawing.Size(1504, 860);
            this.Controls.Add(this.ucBoard);
            this.Controls.Add(this.lblEscHint);
            // Khong cho dong bang chuot: bo ca vien cua so lan nut dieu khien, chi thoat bang ESC
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDashboard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmDashboard_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.UcInpatientBoard ucBoard;
        private DevExpress.XtraEditors.LabelControl lblEscHint;
    }
}
