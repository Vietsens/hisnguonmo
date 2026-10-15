namespace HIS.Desktop.Plugins.HisTrackingList.TrackingView
{
    partial class UcTrackingListView
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
            if (disposing)
            {
                this.CancelLoading();
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlStatus = new DevExpress.XtraEditors.PanelControl();
            this.btnRetry = new DevExpress.XtraEditors.SimpleButton();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.pnlViewer = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pnlStatus)).BeginInit();
            this.pnlStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlViewer)).BeginInit();
            this.SuspendLayout();
            //
            // pnlStatus
            //
            this.pnlStatus.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlStatus.Controls.Add(this.btnRetry);
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlStatus.Location = new System.Drawing.Point(0, 0);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(20, 60, 20, 20);
            this.pnlStatus.Size = new System.Drawing.Size(615, 481);
            this.pnlStatus.TabIndex = 0;
            this.pnlStatus.Resize += new System.EventHandler(this.pnlStatus_Resize);
            //
            // btnRetry
            //
            this.btnRetry.Location = new System.Drawing.Point(252, 96);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(110, 24);
            this.btnRetry.TabIndex = 1;
            this.btnRetry.Text = "Thử lại";
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            //
            // lblStatus
            //
            this.lblStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.lblStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblStatus.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Location = new System.Drawing.Point(20, 60);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(575, 16);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = " ";
            //
            // pnlViewer
            //
            this.pnlViewer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewer.Location = new System.Drawing.Point(0, 0);
            this.pnlViewer.Name = "pnlViewer";
            this.pnlViewer.Size = new System.Drawing.Size(615, 481);
            this.pnlViewer.TabIndex = 1;
            this.pnlViewer.Visible = false;
            //
            // UcTrackingListView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlViewer);
            this.Controls.Add(this.pnlStatus);
            this.Name = "UcTrackingListView";
            this.Size = new System.Drawing.Size(615, 481);
            this.Load += new System.EventHandler(this.UcTrackingListView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pnlStatus)).EndInit();
            this.pnlStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlStatus;
        private DevExpress.XtraEditors.SimpleButton btnRetry;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.PanelControl pnlViewer;
    }
}
