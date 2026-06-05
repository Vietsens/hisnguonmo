/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.TreatmentAppointment.SendZaloResult
{
    partial class frmSendZaloResult
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.pnlHeader = new DevExpress.XtraEditors.PanelControl();
            this.lblHeading = new DevExpress.XtraEditors.LabelControl();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.lblFailureHeader = new DevExpress.XtraEditors.LabelControl();
            this.memoFailures = new DevExpress.XtraEditors.MemoEdit();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciHeader = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFailureHeader = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMemoFailures = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceBottom = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlHeader)).BeginInit();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoFailures.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFailureHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMemoFailures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.pnlHeader);
            this.layoutControl1.Controls.Add(this.lblDescription);
            this.layoutControl1.Controls.Add(this.lblFailureHeader);
            this.layoutControl1.Controls.Add(this.memoFailures);
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(560, 380);
            this.layoutControl1.TabIndex = 0;
            //
            // pnlHeader
            //
            this.pnlHeader.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(139)))), ((int)(((byte)(34)))));
            this.pnlHeader.Appearance.Options.UseBackColor = true;
            this.pnlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlHeader.Controls.Add(this.lblHeading);
            this.pnlHeader.Location = new System.Drawing.Point(12, 12);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(536, 50);
            this.pnlHeader.TabIndex = 0;
            //
            // lblHeading
            //
            this.lblHeading.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeading.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblHeading.Appearance.Options.UseFont = true;
            this.lblHeading.Appearance.Options.UseForeColor = true;
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeading.Location = new System.Drawing.Point(0, 0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.lblHeading.Size = new System.Drawing.Size(536, 50);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Đã gửi thành công 0/0 bệnh nhân";
            this.lblHeading.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            //
            // lblDescription
            //
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 76);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(536, 30);
            this.lblDescription.StyleController = this.layoutControl1;
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "—";
            this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            //
            // lblFailureHeader
            //
            this.lblFailureHeader.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblFailureHeader.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lblFailureHeader.Appearance.Options.UseFont = true;
            this.lblFailureHeader.Appearance.Options.UseForeColor = true;
            this.lblFailureHeader.Location = new System.Drawing.Point(12, 120);
            this.lblFailureHeader.Name = "lblFailureHeader";
            this.lblFailureHeader.Size = new System.Drawing.Size(536, 16);
            this.lblFailureHeader.StyleController = this.layoutControl1;
            this.lblFailureHeader.TabIndex = 2;
            this.lblFailureHeader.Text = "Chi tiết các trường hợp gửi thất bại:";
            //
            // memoFailures
            //
            this.memoFailures.Location = new System.Drawing.Point(12, 140);
            this.memoFailures.Name = "memoFailures";
            this.memoFailures.Properties.ReadOnly = true;
            this.memoFailures.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.memoFailures.Size = new System.Drawing.Size(536, 174);
            this.memoFailures.StyleController = this.layoutControl1;
            this.memoFailures.TabIndex = 3;
            //
            // btnClose
            //
            this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnClose.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnClose.Appearance.Options.UseBackColor = true;
            this.btnClose.Appearance.Options.UseForeColor = true;
            this.btnClose.Location = new System.Drawing.Point(466, 346);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciHeader,
            this.lciDescription,
            this.lciFailureHeader,
            this.lciMemoFailures,
            this.emptySpaceBottom,
            this.lciBtnClose});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(560, 380);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciHeader
            //
            this.lciHeader.Control = this.pnlHeader;
            this.lciHeader.Location = new System.Drawing.Point(0, 0);
            this.lciHeader.MaxSize = new System.Drawing.Size(0, 54);
            this.lciHeader.MinSize = new System.Drawing.Size(54, 54);
            this.lciHeader.Name = "lciHeader";
            this.lciHeader.Size = new System.Drawing.Size(540, 54);
            this.lciHeader.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHeader.TextSize = new System.Drawing.Size(0, 0);
            this.lciHeader.TextVisible = false;
            //
            // lciDescription
            //
            this.lciDescription.Control = this.lblDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 54);
            this.lciDescription.MaxSize = new System.Drawing.Size(0, 44);
            this.lciDescription.MinSize = new System.Drawing.Size(54, 44);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(540, 44);
            this.lciDescription.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDescription.TextSize = new System.Drawing.Size(0, 0);
            this.lciDescription.TextVisible = false;
            //
            // lciFailureHeader
            //
            this.lciFailureHeader.Control = this.lblFailureHeader;
            this.lciFailureHeader.Location = new System.Drawing.Point(0, 98);
            this.lciFailureHeader.MaxSize = new System.Drawing.Size(0, 20);
            this.lciFailureHeader.MinSize = new System.Drawing.Size(54, 20);
            this.lciFailureHeader.Name = "lciFailureHeader";
            this.lciFailureHeader.Size = new System.Drawing.Size(540, 20);
            this.lciFailureHeader.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciFailureHeader.TextSize = new System.Drawing.Size(0, 0);
            this.lciFailureHeader.TextVisible = false;
            //
            // lciMemoFailures
            //
            this.lciMemoFailures.Control = this.memoFailures;
            this.lciMemoFailures.Location = new System.Drawing.Point(0, 118);
            this.lciMemoFailures.Name = "lciMemoFailures";
            this.lciMemoFailures.Size = new System.Drawing.Size(540, 178);
            this.lciMemoFailures.TextSize = new System.Drawing.Size(0, 0);
            this.lciMemoFailures.TextVisible = false;
            //
            // emptySpaceBottom
            //
            this.emptySpaceBottom.AllowHotTrack = false;
            this.emptySpaceBottom.Location = new System.Drawing.Point(0, 296);
            this.emptySpaceBottom.Name = "emptySpaceBottom";
            this.emptySpaceBottom.Size = new System.Drawing.Size(454, 26);
            this.emptySpaceBottom.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(454, 296);
            this.lciBtnClose.MaxSize = new System.Drawing.Size(86, 26);
            this.lciBtnClose.MinSize = new System.Drawing.Size(86, 26);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(86, 26);
            this.lciBtnClose.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // frmSendZaloResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 380);
            this.Controls.Add(this.layoutControl1);
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Name = "frmSendZaloResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kết quả gửi tin Zalo";
            this.Load += new System.EventHandler(this.frmSendZaloResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlHeader)).EndInit();
            this.pnlHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoFailures.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFailureHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMemoFailures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl pnlHeader;
        private DevExpress.XtraEditors.LabelControl lblHeading;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.LabelControl lblFailureHeader;
        private DevExpress.XtraEditors.MemoEdit memoFailures;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciHeader;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraLayout.LayoutControlItem lciFailureHeader;
        private DevExpress.XtraLayout.LayoutControlItem lciMemoFailures;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceBottom;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
    }
}
