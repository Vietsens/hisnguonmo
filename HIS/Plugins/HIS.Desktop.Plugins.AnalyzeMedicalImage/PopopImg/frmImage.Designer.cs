namespace HIS.Desktop.Plugins.AnalyzeMedicalImage.PopopImg
{
    partial class frmImage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImage));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.richEditControl = new DevExpress.XtraRichEdit.RichEditControl();
            this.pdfViewer1 = new DevExpress.XtraPdfViewer.PdfViewer();
            this.pteAnhChupFileDinhKem = new DevExpress.XtraEditors.PictureEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.wordview = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pteAnhChupFileDinhKem.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordview)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.richEditControl);
            this.layoutControl1.Controls.Add(this.pdfViewer1);
            this.layoutControl1.Controls.Add(this.pteAnhChupFileDinhKem);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(804, 711);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // richEditControl
            // 
            this.richEditControl.Location = new System.Drawing.Point(12, 12);
            this.richEditControl.Name = "richEditControl";
            this.richEditControl.ReadOnly = true;
            this.richEditControl.Size = new System.Drawing.Size(780, 193);
            this.richEditControl.TabIndex = 6;
            this.richEditControl.Text = "richEditControl1";
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.Location = new System.Drawing.Point(12, 444);
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.NavigationPaneInitialVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Hidden;
            this.pdfViewer1.ReadOnly = true;
            this.pdfViewer1.ShowPrintStatusDialog = false;
            this.pdfViewer1.Size = new System.Drawing.Size(780, 255);
            this.pdfViewer1.TabIndex = 5;
            this.pdfViewer1.TabStop = false;
            this.pdfViewer1.Visible = false;
            this.pdfViewer1.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToVisible;
            // 
            // pteAnhChupFileDinhKem
            // 
            this.pteAnhChupFileDinhKem.Location = new System.Drawing.Point(12, 209);
            this.pteAnhChupFileDinhKem.Name = "pteAnhChupFileDinhKem";
            this.pteAnhChupFileDinhKem.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pteAnhChupFileDinhKem.Size = new System.Drawing.Size(780, 231);
            this.pteAnhChupFileDinhKem.StyleController = this.layoutControl1;
            this.pteAnhChupFileDinhKem.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.wordview});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(804, 711);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.pteAnhChupFileDinhKem;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 197);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(784, 235);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.pdfViewer1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 432);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(784, 259);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // wordview
            // 
            this.wordview.Control = this.richEditControl;
            this.wordview.Location = new System.Drawing.Point(0, 0);
            this.wordview.Name = "wordview";
            this.wordview.Size = new System.Drawing.Size(784, 197);
            this.wordview.TextSize = new System.Drawing.Size(0, 0);
            this.wordview.TextVisible = false;
            // 
            // frmImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 711);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmImage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi tiết";
            this.Load += new System.EventHandler(this.frmImage_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pteAnhChupFileDinhKem.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wordview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.PictureEdit pteAnhChupFileDinhKem;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraPdfViewer.PdfViewer pdfViewer1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraRichEdit.RichEditControl richEditControl;
        private DevExpress.XtraLayout.LayoutControlItem wordview;
    }
}