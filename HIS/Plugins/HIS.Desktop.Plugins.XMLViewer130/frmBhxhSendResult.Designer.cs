namespace HIS.Desktop.Plugins.XMLViewer130
{
    partial class frmBhxhSendResult
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
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.txtMaKetQua = new DevExpress.XtraEditors.TextEdit();
            this.txtMaGiaoDich = new DevExpress.XtraEditors.TextEdit();
            this.txtThongDiep = new DevExpress.XtraEditors.TextEdit();
            this.txtThoiGianTiepNhan = new DevExpress.XtraEditors.TextEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMaKetQua = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMaGiaoDich = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciThongDiep = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciThoiGianTiepNhan = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaKetQua.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaGiaoDich.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThongDiep.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThoiGianTiepNhan.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaKetQua)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaGiaoDich)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThongDiep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThoiGianTiepNhan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblStatus);
            this.layoutControl1.Controls.Add(this.txtMaKetQua);
            this.layoutControl1.Controls.Add(this.txtMaGiaoDich);
            this.layoutControl1.Controls.Add(this.txtThongDiep);
            this.layoutControl1.Controls.Add(this.txtThoiGianTiepNhan);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(420, 260);
            this.layoutControl1.TabIndex = 0;
            //
            // lblStatus
            //
            this.lblStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Appearance.Options.UseFont = true;
            this.lblStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStatus.Location = new System.Drawing.Point(12, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(396, 24);
            this.lblStatus.StyleController = this.layoutControl1;
            this.lblStatus.TabIndex = 4;
            //
            // txtMaKetQua
            //
            this.txtMaKetQua.Location = new System.Drawing.Point(137, 48);
            this.txtMaKetQua.Name = "txtMaKetQua";
            this.txtMaKetQua.Properties.ReadOnly = true;
            this.txtMaKetQua.Size = new System.Drawing.Size(271, 20);
            this.txtMaKetQua.StyleController = this.layoutControl1;
            this.txtMaKetQua.TabIndex = 5;
            //
            // txtMaGiaoDich
            //
            this.txtMaGiaoDich.Location = new System.Drawing.Point(137, 80);
            this.txtMaGiaoDich.Name = "txtMaGiaoDich";
            this.txtMaGiaoDich.Properties.ReadOnly = true;
            this.txtMaGiaoDich.Size = new System.Drawing.Size(271, 20);
            this.txtMaGiaoDich.StyleController = this.layoutControl1;
            this.txtMaGiaoDich.TabIndex = 6;
            //
            // txtThongDiep
            //
            this.txtThongDiep.Location = new System.Drawing.Point(137, 112);
            this.txtThongDiep.Name = "txtThongDiep";
            this.txtThongDiep.Properties.ReadOnly = true;
            this.txtThongDiep.Size = new System.Drawing.Size(271, 20);
            this.txtThongDiep.StyleController = this.layoutControl1;
            this.txtThongDiep.TabIndex = 7;
            //
            // txtThoiGianTiepNhan
            //
            this.txtThoiGianTiepNhan.Location = new System.Drawing.Point(137, 144);
            this.txtThoiGianTiepNhan.Name = "txtThoiGianTiepNhan";
            this.txtThoiGianTiepNhan.Properties.ReadOnly = true;
            this.txtThoiGianTiepNhan.Size = new System.Drawing.Size(271, 20);
            this.txtThoiGianTiepNhan.StyleController = this.layoutControl1;
            this.txtThoiGianTiepNhan.TabIndex = 8;
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(318, 226);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciStatus,
            this.lciMaKetQua,
            this.lciMaGiaoDich,
            this.lciThongDiep,
            this.lciThoiGianTiepNhan,
            this.lciBtnClose,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(420, 260);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciStatus
            //
            this.lciStatus.Control = this.lblStatus;
            this.lciStatus.Location = new System.Drawing.Point(0, 0);
            this.lciStatus.Name = "lciStatus";
            this.lciStatus.Size = new System.Drawing.Size(400, 28);
            this.lciStatus.TextSize = new System.Drawing.Size(0, 0);
            this.lciStatus.TextVisible = false;
            //
            // lciMaKetQua
            //
            this.lciMaKetQua.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMaKetQua.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMaKetQua.Control = this.txtMaKetQua;
            this.lciMaKetQua.Location = new System.Drawing.Point(0, 28);
            this.lciMaKetQua.Name = "lciMaKetQua";
            this.lciMaKetQua.Size = new System.Drawing.Size(400, 32);
            this.lciMaKetQua.Text = "Mã kết quả:";
            this.lciMaKetQua.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMaKetQua.TextSize = new System.Drawing.Size(120, 20);
            this.lciMaKetQua.TextToControlDistance = 5;
            //
            // lciMaGiaoDich
            //
            this.lciMaGiaoDich.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMaGiaoDich.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMaGiaoDich.Control = this.txtMaGiaoDich;
            this.lciMaGiaoDich.Location = new System.Drawing.Point(0, 60);
            this.lciMaGiaoDich.Name = "lciMaGiaoDich";
            this.lciMaGiaoDich.Size = new System.Drawing.Size(400, 32);
            this.lciMaGiaoDich.Text = "Mã giao dịch:";
            this.lciMaGiaoDich.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMaGiaoDich.TextSize = new System.Drawing.Size(120, 20);
            this.lciMaGiaoDich.TextToControlDistance = 5;
            //
            // lciThongDiep
            //
            this.lciThongDiep.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciThongDiep.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciThongDiep.Control = this.txtThongDiep;
            this.lciThongDiep.Location = new System.Drawing.Point(0, 92);
            this.lciThongDiep.Name = "lciThongDiep";
            this.lciThongDiep.Size = new System.Drawing.Size(400, 32);
            this.lciThongDiep.Text = "Thông điệp:";
            this.lciThongDiep.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciThongDiep.TextSize = new System.Drawing.Size(120, 20);
            this.lciThongDiep.TextToControlDistance = 5;
            //
            // lciThoiGianTiepNhan
            //
            this.lciThoiGianTiepNhan.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciThoiGianTiepNhan.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciThoiGianTiepNhan.Control = this.txtThoiGianTiepNhan;
            this.lciThoiGianTiepNhan.Location = new System.Drawing.Point(0, 124);
            this.lciThoiGianTiepNhan.Name = "lciThoiGianTiepNhan";
            this.lciThoiGianTiepNhan.Size = new System.Drawing.Size(400, 32);
            this.lciThoiGianTiepNhan.Text = "Thời gian tiếp nhận:";
            this.lciThoiGianTiepNhan.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciThoiGianTiepNhan.TextSize = new System.Drawing.Size(120, 20);
            this.lciThoiGianTiepNhan.TextToControlDistance = 5;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 156);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(400, 62);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(300, 218);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(100, 26);
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // frmBhxhSendResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 260);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBhxhSendResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kết quả gửi Cổng BHXH";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaKetQua.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaGiaoDich.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThongDiep.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtThoiGianTiepNhan.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaKetQua)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaGiaoDich)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThongDiep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciThoiGianTiepNhan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.TextEdit txtMaKetQua;
        private DevExpress.XtraEditors.TextEdit txtMaGiaoDich;
        private DevExpress.XtraEditors.TextEdit txtThongDiep;
        private DevExpress.XtraEditors.TextEdit txtThoiGianTiepNhan;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciStatus;
        private DevExpress.XtraLayout.LayoutControlItem lciMaKetQua;
        private DevExpress.XtraLayout.LayoutControlItem lciMaGiaoDich;
        private DevExpress.XtraLayout.LayoutControlItem lciThongDiep;
        private DevExpress.XtraLayout.LayoutControlItem lciThoiGianTiepNhan;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
