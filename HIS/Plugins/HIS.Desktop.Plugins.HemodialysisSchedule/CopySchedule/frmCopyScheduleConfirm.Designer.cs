/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    partial class frmCopyScheduleConfirm
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
            this.lblRoom = new DevExpress.XtraEditors.LabelControl();
            this.lblSourceDate = new DevExpress.XtraEditors.LabelControl();
            this.lblTargetDate = new DevExpress.XtraEditors.LabelControl();
            this.lblAdd = new DevExpress.XtraEditors.LabelControl();
            this.lblSkip = new DevExpress.XtraEditors.LabelControl();
            this.memoSkip = new DevExpress.XtraEditors.MemoEdit();
            this.lblFootnote = new DevExpress.XtraEditors.LabelControl();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.memoSkip.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRoom
            // 
            this.lblRoom.Location = new System.Drawing.Point(16, 16);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(60, 13);
            this.lblRoom.TabIndex = 0;
            this.lblRoom.Text = "Phòng chạy:";
            // 
            // lblSourceDate
            // 
            this.lblSourceDate.Location = new System.Drawing.Point(16, 38);
            this.lblSourceDate.Name = "lblSourceDate";
            this.lblSourceDate.Size = new System.Drawing.Size(62, 13);
            this.lblSourceDate.TabIndex = 1;
            this.lblSourceDate.Text = "Ngày nguồn:";
            // 
            // lblTargetDate
            // 
            this.lblTargetDate.Location = new System.Drawing.Point(16, 60);
            this.lblTargetDate.Name = "lblTargetDate";
            this.lblTargetDate.Size = new System.Drawing.Size(51, 13);
            this.lblTargetDate.TabIndex = 2;
            this.lblTargetDate.Text = "Ngày đích:";
            // 
            // lblAdd
            // 
            this.lblAdd.Appearance.ForeColor = System.Drawing.Color.Green;
            this.lblAdd.Location = new System.Drawing.Point(16, 90);
            this.lblAdd.Name = "lblAdd";
            this.lblAdd.Size = new System.Drawing.Size(39, 13);
            this.lblAdd.TabIndex = 3;
            this.lblAdd.Text = "Sẽ thêm";
            // 
            // lblSkip
            // 
            this.lblSkip.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblSkip.Location = new System.Drawing.Point(16, 112);
            this.lblSkip.Name = "lblSkip";
            this.lblSkip.Size = new System.Drawing.Size(33, 13);
            this.lblSkip.TabIndex = 4;
            this.lblSkip.Text = "Sẽ skip";
            // 
            // memoSkip
            // 
            this.memoSkip.Location = new System.Drawing.Point(16, 132);
            this.memoSkip.Name = "memoSkip";
            this.memoSkip.Properties.ReadOnly = true;
            this.memoSkip.Size = new System.Drawing.Size(450, 90);
            this.memoSkip.TabIndex = 5;
            // 
            // lblFootnote
            // 
            this.lblFootnote.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblFootnote.Location = new System.Drawing.Point(16, 230);
            this.lblFootnote.Name = "lblFootnote";
            this.lblFootnote.Size = new System.Drawing.Size(35, 13);
            this.lblFootnote.TabIndex = 6;
            this.lblFootnote.Text = "Ghi chú";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(276, 256);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(140, 26);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "Sao chép";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(422, 256);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 26);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmCopyScheduleConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 296);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblFootnote);
            this.Controls.Add(this.memoSkip);
            this.Controls.Add(this.lblSkip);
            this.Controls.Add(this.lblAdd);
            this.Controls.Add(this.lblTargetDate);
            this.Controls.Add(this.lblSourceDate);
            this.Controls.Add(this.lblRoom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCopyScheduleConfirm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xác nhận sao chép lịch";
            ((System.ComponentModel.ISupportInitialize)(this.memoSkip.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblRoom;
        private DevExpress.XtraEditors.LabelControl lblSourceDate;
        private DevExpress.XtraEditors.LabelControl lblTargetDate;
        private DevExpress.XtraEditors.LabelControl lblAdd;
        private DevExpress.XtraEditors.LabelControl lblSkip;
        private DevExpress.XtraEditors.MemoEdit memoSkip;
        private DevExpress.XtraEditors.LabelControl lblFootnote;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
