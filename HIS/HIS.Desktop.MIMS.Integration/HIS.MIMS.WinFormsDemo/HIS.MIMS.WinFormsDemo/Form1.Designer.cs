namespace HIS.MIMS.WinFormsDemo
{
    partial class Form1
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.btnTestDrugInfoProduct = new System.Windows.Forms.Button();
            this.btnTestDrugInfoGGPI = new System.Windows.Forms.Button();
            this.btnTestCdsInteraction = new System.Windows.Forms.Button();
            this.btnTestVnContra = new System.Windows.Forms.Button();
            this.btnTestDrugDrugAlert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(12, 51);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(960, 598);
            this.webBrowser1.TabIndex = 0;
            // 
            // btnTestDrugInfoProduct
            // 
            this.btnTestDrugInfoProduct.Location = new System.Drawing.Point(12, 12);
            this.btnTestDrugInfoProduct.Name = "btnTestDrugInfoProduct";
            this.btnTestDrugInfoProduct.Size = new System.Drawing.Size(160, 30);
            this.btnTestDrugInfoProduct.TabIndex = 1;
            this.btnTestDrugInfoProduct.Text = "Drug Info (Product)";
            this.btnTestDrugInfoProduct.UseVisualStyleBackColor = true;
            this.btnTestDrugInfoProduct.Click += new System.EventHandler(this.btnTestDrugInfoProduct_Click);
            // 
            // btnTestDrugInfoGGPI
            // 
            this.btnTestDrugInfoGGPI.Location = new System.Drawing.Point(178, 12);
            this.btnTestDrugInfoGGPI.Name = "btnTestDrugInfoGGPI";
            this.btnTestDrugInfoGGPI.Size = new System.Drawing.Size(160, 30);
            this.btnTestDrugInfoGGPI.TabIndex = 2;
            this.btnTestDrugInfoGGPI.Text = "Drug Info (GGPI)";
            this.btnTestDrugInfoGGPI.UseVisualStyleBackColor = true;
            this.btnTestDrugInfoGGPI.Click += new System.EventHandler(this.btnTestDrugInfoGGPI_Click);
            // 
            // btnTestCdsInteraction
            // 
            this.btnTestCdsInteraction.Location = new System.Drawing.Point(344, 12);
            this.btnTestCdsInteraction.Name = "btnTestCdsInteraction";
            this.btnTestCdsInteraction.Size = new System.Drawing.Size(160, 30);
            this.btnTestCdsInteraction.TabIndex = 3;
            this.btnTestCdsInteraction.Text = "CDS Interaction";
            this.btnTestCdsInteraction.UseVisualStyleBackColor = true;
            this.btnTestCdsInteraction.Click += new System.EventHandler(this.btnTestCdsInteraction_Click);
            // 
            // btnTestVnContra
            // 
            this.btnTestVnContra.Location = new System.Drawing.Point(510, 12);
            this.btnTestVnContra.Name = "btnTestVnContra";
            this.btnTestVnContra.Size = new System.Drawing.Size(160, 30);
            this.btnTestVnContra.TabIndex = 4;
            this.btnTestVnContra.Text = "VN Contra";
            this.btnTestVnContra.UseVisualStyleBackColor = true;
            this.btnTestVnContra.Click += new System.EventHandler(this.btnTestVnContra_Click);
            // 
            // btnTestDrugDrugAlert
            // 
            this.btnTestDrugDrugAlert.Location = new System.Drawing.Point(676, 12);
            this.btnTestDrugDrugAlert.Name = "btnTestDrugDrugAlert";
            this.btnTestDrugDrugAlert.Size = new System.Drawing.Size(160, 30);
            this.btnTestDrugDrugAlert.TabIndex = 5;
            this.btnTestDrugDrugAlert.Text = "Drug-ICD Alert";
            this.btnTestDrugDrugAlert.UseVisualStyleBackColor = true;
            this.btnTestDrugDrugAlert.Click += new System.EventHandler(this.btnTestDrugDrugAlert_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.btnTestDrugDrugAlert);
            this.Controls.Add(this.btnTestVnContra);
            this.Controls.Add(this.btnTestCdsInteraction);
            this.Controls.Add(this.btnTestDrugInfoGGPI);
            this.Controls.Add(this.btnTestDrugInfoProduct);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Form1";
            this.Text = "HIS.MIMS.WinFormsDemo";
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btnTestDrugInfoProduct;
        private System.Windows.Forms.Button btnTestDrugInfoGGPI;
        private System.Windows.Forms.Button btnTestCdsInteraction;
        private System.Windows.Forms.Button btnTestVnContra;
        private System.Windows.Forms.Button btnTestDrugDrugAlert;
    }
}
