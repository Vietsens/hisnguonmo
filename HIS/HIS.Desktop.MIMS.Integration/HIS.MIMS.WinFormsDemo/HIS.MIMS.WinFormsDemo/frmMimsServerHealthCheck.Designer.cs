namespace HIS.MIMS.WinFormsDemo
{
    partial class frmMimsServerHealthCheck
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblCdsUrl = new System.Windows.Forms.Label();
            this.txtCdsUrl = new System.Windows.Forms.TextBox();
            this.lblVnUrl = new System.Windows.Forms.Label();
            this.txtVnUrl = new System.Windows.Forms.TextBox();
            this.btnTestVnContra = new System.Windows.Forms.Button();
            this.btnTestCdsDrugDrug = new System.Windows.Forms.Button();
            this.btnTestCdsDrugHealth = new System.Windows.Forms.Button();
            this.btnTestDrugInfo = new System.Windows.Forms.Button();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.lblRequest = new System.Windows.Forms.Label();
            this.txtRequest = new System.Windows.Forms.TextBox();
            this.lblResponse = new System.Windows.Forms.Label();
            this.txtResponse = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblCdsUrl
            //
            this.lblCdsUrl.AutoSize = false;
            this.lblCdsUrl.Location = new System.Drawing.Point(12, 14);
            this.lblCdsUrl.Name = "lblCdsUrl";
            this.lblCdsUrl.Size = new System.Drawing.Size(130, 20);
            this.lblCdsUrl.TabIndex = 0;
            this.lblCdsUrl.Text = "Endpoint CDS:";
            //
            // txtCdsUrl
            //
            this.txtCdsUrl.Location = new System.Drawing.Point(148, 11);
            this.txtCdsUrl.Name = "txtCdsUrl";
            this.txtCdsUrl.ReadOnly = true;
            this.txtCdsUrl.Size = new System.Drawing.Size(710, 22);
            this.txtCdsUrl.TabIndex = 1;
            //
            // lblVnUrl
            //
            this.lblVnUrl.AutoSize = false;
            this.lblVnUrl.Location = new System.Drawing.Point(12, 42);
            this.lblVnUrl.Name = "lblVnUrl";
            this.lblVnUrl.Size = new System.Drawing.Size(130, 20);
            this.lblVnUrl.TabIndex = 2;
            this.lblVnUrl.Text = "Endpoint VN Contra:";
            //
            // txtVnUrl
            //
            this.txtVnUrl.Location = new System.Drawing.Point(148, 39);
            this.txtVnUrl.Name = "txtVnUrl";
            this.txtVnUrl.ReadOnly = true;
            this.txtVnUrl.Size = new System.Drawing.Size(710, 22);
            this.txtVnUrl.TabIndex = 3;
            //
            // btnTestVnContra
            //
            this.btnTestVnContra.Location = new System.Drawing.Point(12, 75);
            this.btnTestVnContra.Name = "btnTestVnContra";
            this.btnTestVnContra.Size = new System.Drawing.Size(200, 32);
            this.btnTestVnContra.TabIndex = 4;
            this.btnTestVnContra.Text = "Test VN Contraindication";
            this.btnTestVnContra.UseVisualStyleBackColor = true;
            this.btnTestVnContra.Click += new System.EventHandler(this.btnTestVnContra_Click);
            //
            // btnTestCdsDrugDrug
            //
            this.btnTestCdsDrugDrug.Location = new System.Drawing.Point(220, 75);
            this.btnTestCdsDrugDrug.Name = "btnTestCdsDrugDrug";
            this.btnTestCdsDrugDrug.Size = new System.Drawing.Size(180, 32);
            this.btnTestCdsDrugDrug.TabIndex = 5;
            this.btnTestCdsDrugDrug.Text = "Test CDS Drug-Drug";
            this.btnTestCdsDrugDrug.UseVisualStyleBackColor = true;
            this.btnTestCdsDrugDrug.Click += new System.EventHandler(this.btnTestCdsDrugDrug_Click);
            //
            // btnTestCdsDrugHealth
            //
            this.btnTestCdsDrugHealth.Location = new System.Drawing.Point(408, 75);
            this.btnTestCdsDrugHealth.Name = "btnTestCdsDrugHealth";
            this.btnTestCdsDrugHealth.Size = new System.Drawing.Size(200, 32);
            this.btnTestCdsDrugHealth.TabIndex = 6;
            this.btnTestCdsDrugHealth.Text = "Test CDS Drug-Health";
            this.btnTestCdsDrugHealth.UseVisualStyleBackColor = true;
            this.btnTestCdsDrugHealth.Click += new System.EventHandler(this.btnTestCdsDrugHealth_Click);
            //
            // btnTestDrugInfo
            //
            this.btnTestDrugInfo.Location = new System.Drawing.Point(616, 75);
            this.btnTestDrugInfo.Name = "btnTestDrugInfo";
            this.btnTestDrugInfo.Size = new System.Drawing.Size(140, 32);
            this.btnTestDrugInfo.TabIndex = 7;
            this.btnTestDrugInfo.Text = "Test Drug Info";
            this.btnTestDrugInfo.UseVisualStyleBackColor = true;
            this.btnTestDrugInfo.Click += new System.EventHandler(this.btnTestDrugInfo_Click);
            //
            // btnRunAll
            //
            this.btnRunAll.Location = new System.Drawing.Point(764, 75);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(94, 32);
            this.btnRunAll.TabIndex = 8;
            this.btnRunAll.Text = "Run All";
            this.btnRunAll.UseVisualStyleBackColor = true;
            this.btnRunAll.Click += new System.EventHandler(this.btnRunAll_Click);
            //
            // lblRequest
            //
            this.lblRequest.AutoSize = false;
            this.lblRequest.Location = new System.Drawing.Point(12, 118);
            this.lblRequest.Name = "lblRequest";
            this.lblRequest.Size = new System.Drawing.Size(150, 20);
            this.lblRequest.TabIndex = 9;
            this.lblRequest.Text = "Request XML:";
            //
            // txtRequest
            //
            this.txtRequest.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtRequest.Location = new System.Drawing.Point(12, 138);
            this.txtRequest.Multiline = true;
            this.txtRequest.Name = "txtRequest";
            this.txtRequest.ReadOnly = true;
            this.txtRequest.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRequest.Size = new System.Drawing.Size(846, 140);
            this.txtRequest.TabIndex = 10;
            this.txtRequest.WordWrap = false;
            //
            // lblResponse
            //
            this.lblResponse.AutoSize = false;
            this.lblResponse.Location = new System.Drawing.Point(12, 290);
            this.lblResponse.Name = "lblResponse";
            this.lblResponse.Size = new System.Drawing.Size(150, 20);
            this.lblResponse.TabIndex = 11;
            this.lblResponse.Text = "Response XML:";
            //
            // txtResponse
            //
            this.txtResponse.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtResponse.Location = new System.Drawing.Point(12, 310);
            this.txtResponse.Multiline = true;
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ReadOnly = true;
            this.txtResponse.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResponse.Size = new System.Drawing.Size(846, 240);
            this.txtResponse.TabIndex = 12;
            this.txtResponse.WordWrap = false;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(12, 560);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(5);
            this.lblStatus.Size = new System.Drawing.Size(846, 34);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Sẵn sàng test MIMS server.";
            //
            // frmMimsServerHealthCheck
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 608);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtResponse);
            this.Controls.Add(this.lblResponse);
            this.Controls.Add(this.txtRequest);
            this.Controls.Add(this.lblRequest);
            this.Controls.Add(this.btnRunAll);
            this.Controls.Add(this.btnTestDrugInfo);
            this.Controls.Add(this.btnTestCdsDrugHealth);
            this.Controls.Add(this.btnTestCdsDrugDrug);
            this.Controls.Add(this.btnTestVnContra);
            this.Controls.Add(this.txtVnUrl);
            this.Controls.Add(this.lblVnUrl);
            this.Controls.Add(this.txtCdsUrl);
            this.Controls.Add(this.lblCdsUrl);
            this.Name = "frmMimsServerHealthCheck";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MIMS Server Health Check";
            this.Load += new System.EventHandler(this.frmMimsServerHealthCheck_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblCdsUrl;
        private System.Windows.Forms.TextBox txtCdsUrl;
        private System.Windows.Forms.Label lblVnUrl;
        private System.Windows.Forms.TextBox txtVnUrl;
        private System.Windows.Forms.Button btnTestVnContra;
        private System.Windows.Forms.Button btnTestCdsDrugDrug;
        private System.Windows.Forms.Button btnTestCdsDrugHealth;
        private System.Windows.Forms.Button btnTestDrugInfo;
        private System.Windows.Forms.Button btnRunAll;
        private System.Windows.Forms.Label lblRequest;
        private System.Windows.Forms.TextBox txtRequest;
        private System.Windows.Forms.Label lblResponse;
        private System.Windows.Forms.TextBox txtResponse;
        private System.Windows.Forms.Label lblStatus;
    }
}
