namespace HIS.Desktop.Plugins.KskSyncList.Preview
{
    partial class frmKskSyncPreview
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.rtxtContent = new System.Windows.Forms.RichTextBox();
            this.lblStatusValue = new DevExpress.XtraEditors.LabelControl();
            this.lblConclusionTimeValue = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientValue = new DevExpress.XtraEditors.LabelControl();
            this.lblKskTypeValue = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKskType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatient = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclusionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciContent = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNote = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciContent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.lblNote);
            this.layoutControl1.Controls.Add(this.rtxtContent);
            this.layoutControl1.Controls.Add(this.lblStatusValue);
            this.layoutControl1.Controls.Add(this.lblConclusionTimeValue);
            this.layoutControl1.Controls.Add(this.lblPatientValue);
            this.layoutControl1.Controls.Add(this.lblKskTypeValue);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(604, 436);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(506, 402);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(86, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblNote
            // 
            this.lblNote.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(110)))), ((int)(((byte)(20)))));
            this.lblNote.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNote.Location = new System.Drawing.Point(12, 385);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(580, 13);
            this.lblNote.StyleController = this.layoutControl1;
            this.lblNote.TabIndex = 5;
            this.lblNote.Text = "⚠ Bản xem trước minh hoạ dữ liệu sẽ đẩy lên Cổng dữ liệu BYT (QĐ 1551). Danh sách" +
    " trường theo từng loại KSK.";
            // 
            // memoContent
            // 
            this.rtxtContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtContent.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtxtContent.Location = new System.Drawing.Point(12, 62);
            this.rtxtContent.Name = "rtxtContent";
            this.rtxtContent.ReadOnly = true;
            this.rtxtContent.Size = new System.Drawing.Size(580, 319);
            this.rtxtContent.TabIndex = 4;
            this.rtxtContent.WordWrap = false;
            // 
            // lblStatusValue
            // 
            this.lblStatusValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStatusValue.Location = new System.Drawing.Point(357, 29);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Size = new System.Drawing.Size(235, 13);
            this.lblStatusValue.StyleController = this.layoutControl1;
            this.lblStatusValue.TabIndex = 3;
            this.lblStatusValue.Text = "...";
            // 
            // lblConclusionTimeValue
            // 
            this.lblConclusionTimeValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblConclusionTimeValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblConclusionTimeValue.Location = new System.Drawing.Point(107, 29);
            this.lblConclusionTimeValue.Name = "lblConclusionTimeValue";
            this.lblConclusionTimeValue.Size = new System.Drawing.Size(161, 13);
            this.lblConclusionTimeValue.StyleController = this.layoutControl1;
            this.lblConclusionTimeValue.TabIndex = 2;
            this.lblConclusionTimeValue.Text = "...";
            // 
            // lblPatientValue
            // 
            this.lblPatientValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientValue.Location = new System.Drawing.Point(357, 12);
            this.lblPatientValue.Name = "lblPatientValue";
            this.lblPatientValue.Size = new System.Drawing.Size(235, 13);
            this.lblPatientValue.StyleController = this.layoutControl1;
            this.lblPatientValue.TabIndex = 1;
            this.lblPatientValue.Text = "...";
            // 
            // lblKskTypeValue
            // 
            this.lblKskTypeValue.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblKskTypeValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblKskTypeValue.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblKskTypeValue.Location = new System.Drawing.Point(107, 12);
            this.lblKskTypeValue.Name = "lblKskTypeValue";
            this.lblKskTypeValue.Size = new System.Drawing.Size(161, 13);
            this.lblKskTypeValue.StyleController = this.layoutControl1;
            this.lblKskTypeValue.TabIndex = 0;
            this.lblKskTypeValue.Text = "...";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKskType,
            this.lciPatient,
            this.lciConclusionTime,
            this.lciStatus,
            this.lciContent,
            this.lciNote,
            this.emptySpaceItem1,
            this.lciBtnClose});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(604, 436);
            this.Root.TextVisible = false;
            // 
            // lciKskType
            // 
            this.lciKskType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciKskType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciKskType.Control = this.lblKskTypeValue;
            this.lciKskType.Location = new System.Drawing.Point(0, 0);
            this.lciKskType.Name = "lciKskType";
            this.lciKskType.Size = new System.Drawing.Size(260, 17);
            this.lciKskType.Text = "Loại KSK:";
            this.lciKskType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKskType.TextSize = new System.Drawing.Size(90, 13);
            this.lciKskType.TextToControlDistance = 5;
            // 
            // lciPatient
            // 
            this.lciPatient.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatient.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatient.Control = this.lblPatientValue;
            this.lciPatient.Location = new System.Drawing.Point(260, 0);
            this.lciPatient.Name = "lciPatient";
            this.lciPatient.Size = new System.Drawing.Size(324, 17);
            this.lciPatient.Text = "Bệnh nhân:";
            this.lciPatient.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatient.TextSize = new System.Drawing.Size(80, 13);
            this.lciPatient.TextToControlDistance = 5;
            // 
            // lciConclusionTime
            // 
            this.lciConclusionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusionTime.Control = this.lblConclusionTimeValue;
            this.lciConclusionTime.Location = new System.Drawing.Point(0, 17);
            this.lciConclusionTime.Name = "lciConclusionTime";
            this.lciConclusionTime.Size = new System.Drawing.Size(260, 17);
            this.lciConclusionTime.Text = "Ngày kết luận:";
            this.lciConclusionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusionTime.TextSize = new System.Drawing.Size(90, 13);
            this.lciConclusionTime.TextToControlDistance = 5;
            // 
            // lciStatus
            // 
            this.lciStatus.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciStatus.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciStatus.Control = this.lblStatusValue;
            this.lciStatus.Location = new System.Drawing.Point(260, 17);
            this.lciStatus.Name = "lciStatus";
            this.lciStatus.Size = new System.Drawing.Size(324, 17);
            this.lciStatus.Text = "Trạng thái:";
            this.lciStatus.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciStatus.TextSize = new System.Drawing.Size(80, 13);
            this.lciStatus.TextToControlDistance = 5;
            // 
            // lciContent
            // 
            this.lciContent.Control = this.rtxtContent;
            this.lciContent.Location = new System.Drawing.Point(0, 34);
            this.lciContent.Name = "lciContent";
            this.lciContent.Size = new System.Drawing.Size(584, 339);
            this.lciContent.Text = "Nội dung dữ liệu sẽ gửi (xem trước):";
            this.lciContent.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciContent.TextSize = new System.Drawing.Size(173, 13);
            // 
            // lciNote
            // 
            this.lciNote.Control = this.lblNote;
            this.lciNote.Location = new System.Drawing.Point(0, 373);
            this.lciNote.Name = "lciNote";
            this.lciNote.Size = new System.Drawing.Size(584, 17);
            this.lciNote.TextSize = new System.Drawing.Size(0, 0);
            this.lciNote.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 390);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(494, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnClose
            // 
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(494, 390);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(90, 26);
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            // 
            // frmKskSyncPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 436);
            this.Controls.Add(this.layoutControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmKskSyncPreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xem dữ liệu sẽ đẩy lên Cổng dữ liệu BYT";
            this.Load += new System.EventHandler(this.frmKskSyncPreview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciContent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.LabelControl lblKskTypeValue;
        private DevExpress.XtraEditors.LabelControl lblPatientValue;
        private DevExpress.XtraEditors.LabelControl lblConclusionTimeValue;
        private DevExpress.XtraEditors.LabelControl lblStatusValue;
        private System.Windows.Forms.RichTextBox rtxtContent;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciKskType;
        private DevExpress.XtraLayout.LayoutControlItem lciPatient;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciStatus;
        private DevExpress.XtraLayout.LayoutControlItem lciContent;
        private DevExpress.XtraLayout.LayoutControlItem lciNote;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
    }
}
