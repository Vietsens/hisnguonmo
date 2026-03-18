namespace HIS.Desktop.Plugins.Library.BankHub.Popup
{
    partial class BankLogin
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
            this.webControl1 = new EO.WinForm.WebControl();
            this.webView1 = new EO.WebBrowser.WebView();
            this.SuspendLayout();
            // 
            // webControl1
            // 
            this.webControl1.BackColor = System.Drawing.Color.White;
            this.webControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl1.Location = new System.Drawing.Point(0, 0);
            this.webControl1.Name = "webControl1";
            this.webControl1.Size = new System.Drawing.Size(800, 450);
            this.webControl1.TabIndex = 0;
            this.webControl1.Text = "webControl1";
            this.webControl1.WebView = this.webView1;
            // 
            // webView1
            // 
            this.webView1.InputMsgFilter = null;
            this.webView1.ObjectForScripting = null;
            this.webView1.Title = null;
            // 
            // BankLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webControl1);
            this.Name = "BankLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BankLogin";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.BankLogin_Load);
            this.Controls.SetChildIndex(this.webControl1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EO.WinForm.WebControl webControl1;
        private EO.WebBrowser.WebView webView1;
    }
}