namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    partial class frmOverWarning
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
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.gridControlWarning = new DevExpress.XtraGrid.GridControl();
            this.gridViewWarning = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelButton = new DevExpress.XtraEditors.PanelControl();
            this.chkRead = new DevExpress.XtraEditors.CheckEdit();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).BeginInit();
            this.panelButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkRead.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMessage.Location = new System.Drawing.Point(0, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Padding = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.lblMessage.Size = new System.Drawing.Size(720, 30);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Dat vuot so luong con lai";
            //
            // gridControlWarning
            //
            this.gridControlWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlWarning.Location = new System.Drawing.Point(0, 30);
            this.gridControlWarning.MainView = this.gridViewWarning;
            this.gridControlWarning.Name = "gridControlWarning";
            this.gridControlWarning.Size = new System.Drawing.Size(720, 260);
            this.gridControlWarning.TabIndex = 1;
            this.gridControlWarning.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWarning});
            //
            // gridViewWarning
            //
            this.gridViewWarning.GridControl = this.gridControlWarning;
            this.gridViewWarning.Name = "gridViewWarning";
            //
            // panelButton
            //
            this.panelButton.Controls.Add(this.chkRead);
            this.panelButton.Controls.Add(this.btnCancel);
            this.panelButton.Controls.Add(this.btnOk);
            this.panelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButton.Location = new System.Drawing.Point(0, 290);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(720, 40);
            this.panelButton.TabIndex = 2;
            //
            // chkRead
            //
            this.chkRead.Location = new System.Drawing.Point(8, 10);
            this.chkRead.Name = "chkRead";
            this.chkRead.Properties.Caption = "Toi da doc canh bao";
            this.chkRead.Size = new System.Drawing.Size(380, 19);
            this.chkRead.TabIndex = 0;
            this.chkRead.CheckedChanged += new System.EventHandler(this.chkRead_CheckedChanged);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(614, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 22);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Khong dat";
            //
            // btnOk
            //
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(488, 9);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(120, 22);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Dong y dat";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // frmOverWarning
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(720, 330);
            this.Controls.Add(this.gridControlWarning);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.panelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOverWarning";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Canh bao dat vuot";
            this.Load += new System.EventHandler(this.frmOverWarning_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelButton)).EndInit();
            this.panelButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkRead.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraGrid.GridControl gridControlWarning;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWarning;
        private DevExpress.XtraEditors.PanelControl panelButton;
        private DevExpress.XtraEditors.CheckEdit chkRead;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
    }
}
