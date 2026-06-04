/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.TreatmentAppointment.SelectZaloTemplate
{
    partial class frmSelectZaloTemplate
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
            this.lblPatientCountCaption = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCountValue = new DevExpress.XtraEditors.LabelControl();
            this.lblGatewayCaption = new DevExpress.XtraEditors.LabelControl();
            this.lblGatewayValue = new DevExpress.XtraEditors.LabelControl();
            this.cboTemplate = new DevExpress.XtraEditors.LookUpEdit();
            this.lblQualityBadge = new DevExpress.XtraEditors.LabelControl();
            this.lblPreviewHeader = new DevExpress.XtraEditors.LabelControl();
            this.rtxtPreview = new System.Windows.Forms.RichTextBox();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.btnConfirm = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPatientCountLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCountValue = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGatewayLabel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGatewayValue = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTemplate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciQualityBadge = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPreviewHeader = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRtxtPreview = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciNote = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceBottom = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBtnConfirm = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnCancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCountLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCountValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGatewayLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGatewayValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciQualityBadge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreviewHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRtxtPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnConfirm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblPatientCountCaption);
            this.layoutControl1.Controls.Add(this.lblPatientCountValue);
            this.layoutControl1.Controls.Add(this.lblGatewayCaption);
            this.layoutControl1.Controls.Add(this.lblGatewayValue);
            this.layoutControl1.Controls.Add(this.cboTemplate);
            this.layoutControl1.Controls.Add(this.lblQualityBadge);
            this.layoutControl1.Controls.Add(this.lblPreviewHeader);
            this.layoutControl1.Controls.Add(this.rtxtPreview);
            this.layoutControl1.Controls.Add(this.lblNote);
            this.layoutControl1.Controls.Add(this.btnConfirm);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(720, 480);
            this.layoutControl1.TabIndex = 0;
            // 
            // lblPatientCountCaption
            // 
            this.lblPatientCountCaption.Location = new System.Drawing.Point(12, 12);
            this.lblPatientCountCaption.Name = "lblPatientCountCaption";
            this.lblPatientCountCaption.Size = new System.Drawing.Size(80, 13);
            this.lblPatientCountCaption.StyleController = this.layoutControl1;
            this.lblPatientCountCaption.TabIndex = 0;
            this.lblPatientCountCaption.Text = "Số bệnh nhân:";
            // 
            // lblPatientCountValue
            // 
            this.lblPatientCountValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblPatientCountValue.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.lblPatientCountValue.Location = new System.Drawing.Point(96, 12);
            this.lblPatientCountValue.Name = "lblPatientCountValue";
            this.lblPatientCountValue.Size = new System.Drawing.Size(78, 14);
            this.lblPatientCountValue.StyleController = this.layoutControl1;
            this.lblPatientCountValue.TabIndex = 1;
            this.lblPatientCountValue.Text = "0 bệnh nhân";
            // 
            // lblGatewayCaption
            // 
            this.lblGatewayCaption.Location = new System.Drawing.Point(642, 12);
            this.lblGatewayCaption.Name = "lblGatewayCaption";
            this.lblGatewayCaption.Size = new System.Drawing.Size(50, 13);
            this.lblGatewayCaption.StyleController = this.layoutControl1;
            this.lblGatewayCaption.TabIndex = 2;
            this.lblGatewayCaption.Text = "Gateway:";
            // 
            // lblGatewayValue
            // 
            this.lblGatewayValue.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblGatewayValue.Location = new System.Drawing.Point(696, 12);
            this.lblGatewayValue.Name = "lblGatewayValue";
            this.lblGatewayValue.Size = new System.Drawing.Size(12, 14);
            this.lblGatewayValue.StyleController = this.layoutControl1;
            this.lblGatewayValue.TabIndex = 3;
            this.lblGatewayValue.Text = "—";
            // 
            // cboTemplate
            // 
            this.cboTemplate.Location = new System.Drawing.Point(96, 30);
            this.cboTemplate.Name = "cboTemplate";
            this.cboTemplate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTemplate.Properties.NullText = "";
            this.cboTemplate.Size = new System.Drawing.Size(530, 20);
            this.cboTemplate.StyleController = this.layoutControl1;
            this.cboTemplate.TabIndex = 4;
            this.cboTemplate.EditValueChanged += new System.EventHandler(this.cboTemplate_EditValueChanged);
            // 
            // lblQualityBadge
            // 
            this.lblQualityBadge.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.lblQualityBadge.Location = new System.Drawing.Point(630, 30);
            this.lblQualityBadge.Name = "lblQualityBadge";
            this.lblQualityBadge.Size = new System.Drawing.Size(78, 20);
            this.lblQualityBadge.StyleController = this.layoutControl1;
            this.lblQualityBadge.TabIndex = 5;
            this.lblQualityBadge.Text = "—";
            // 
            // lblPreviewHeader
            // 
            this.lblPreviewHeader.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblPreviewHeader.Location = new System.Drawing.Point(12, 54);
            this.lblPreviewHeader.Name = "lblPreviewHeader";
            this.lblPreviewHeader.Size = new System.Drawing.Size(99, 13);
            this.lblPreviewHeader.StyleController = this.layoutControl1;
            this.lblPreviewHeader.TabIndex = 6;
            this.lblPreviewHeader.Text = "Nội dung xem trước:";
            // 
            // rtxtPreview
            // 
            this.rtxtPreview.BackColor = System.Drawing.Color.White;
            this.rtxtPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtPreview.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.rtxtPreview.Location = new System.Drawing.Point(12, 71);
            this.rtxtPreview.Name = "rtxtPreview";
            this.rtxtPreview.ReadOnly = true;
            this.rtxtPreview.Size = new System.Drawing.Size(696, 354);
            this.rtxtPreview.TabIndex = 7;
            this.rtxtPreview.Text = "";
            // 
            // lblNote
            // 
            this.lblNote.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Italic);
            this.lblNote.Appearance.ForeColor = System.Drawing.Color.Gray;
            this.lblNote.Location = new System.Drawing.Point(12, 429);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(341, 13);
            this.lblNote.StyleController = this.layoutControl1;
            this.lblNote.TabIndex = 8;
            this.lblNote.Text = "Các giá trị tô vàng được hệ thống tự điền theo từng bệnh nhân khi gửi.";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnConfirm.Appearance.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Appearance.Options.UseBackColor = true;
            this.btnConfirm.Appearance.Options.UseForeColor = true;
            this.btnConfirm.Location = new System.Drawing.Point(496, 446);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(126, 22);
            this.btnConfirm.StyleController = this.layoutControl1;
            this.btnConfirm.TabIndex = 9;
            this.btnConfirm.Text = "Xác nhận gửi";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(626, 446);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPatientCountLabel,
            this.lciPatientCountValue,
            this.lciGatewayLabel,
            this.lciGatewayValue,
            this.lciTemplate,
            this.lciQualityBadge,
            this.lciPreviewHeader,
            this.lciRtxtPreview,
            this.lciNote,
            this.emptySpaceBottom,
            this.lciBtnConfirm,
            this.lciBtnCancel,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(720, 480);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciPatientCountLabel
            // 
            this.lciPatientCountLabel.Control = this.lblPatientCountCaption;
            this.lciPatientCountLabel.Location = new System.Drawing.Point(0, 0);
            this.lciPatientCountLabel.MaxSize = new System.Drawing.Size(84, 17);
            this.lciPatientCountLabel.MinSize = new System.Drawing.Size(84, 17);
            this.lciPatientCountLabel.Name = "lciPatientCountLabel";
            this.lciPatientCountLabel.Size = new System.Drawing.Size(84, 18);
            this.lciPatientCountLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciPatientCountLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientCountLabel.TextVisible = false;
            // 
            // lciPatientCountValue
            // 
            this.lciPatientCountValue.Control = this.lblPatientCountValue;
            this.lciPatientCountValue.Location = new System.Drawing.Point(84, 0);
            this.lciPatientCountValue.Name = "lciPatientCountValue";
            this.lciPatientCountValue.Size = new System.Drawing.Size(82, 18);
            this.lciPatientCountValue.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientCountValue.TextVisible = false;
            // 
            // lciGatewayLabel
            // 
            this.lciGatewayLabel.Control = this.lblGatewayCaption;
            this.lciGatewayLabel.Location = new System.Drawing.Point(630, 0);
            this.lciGatewayLabel.MaxSize = new System.Drawing.Size(54, 17);
            this.lciGatewayLabel.MinSize = new System.Drawing.Size(54, 17);
            this.lciGatewayLabel.Name = "lciGatewayLabel";
            this.lciGatewayLabel.Size = new System.Drawing.Size(54, 18);
            this.lciGatewayLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGatewayLabel.TextSize = new System.Drawing.Size(0, 0);
            this.lciGatewayLabel.TextVisible = false;
            // 
            // lciGatewayValue
            // 
            this.lciGatewayValue.Control = this.lblGatewayValue;
            this.lciGatewayValue.Location = new System.Drawing.Point(684, 0);
            this.lciGatewayValue.Name = "lciGatewayValue";
            this.lciGatewayValue.Size = new System.Drawing.Size(16, 18);
            this.lciGatewayValue.TextSize = new System.Drawing.Size(0, 0);
            this.lciGatewayValue.TextVisible = false;
            // 
            // lciTemplate
            // 
            this.lciTemplate.Control = this.cboTemplate;
            this.lciTemplate.Location = new System.Drawing.Point(0, 18);
            this.lciTemplate.Name = "lciTemplate";
            this.lciTemplate.Size = new System.Drawing.Size(618, 24);
            this.lciTemplate.Text = "Mẫu tin nhắn:";
            this.lciTemplate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTemplate.TextSize = new System.Drawing.Size(80, 13);
            this.lciTemplate.TextToControlDistance = 4;
            // 
            // lciQualityBadge
            // 
            this.lciQualityBadge.Control = this.lblQualityBadge;
            this.lciQualityBadge.Location = new System.Drawing.Point(618, 18);
            this.lciQualityBadge.MaxSize = new System.Drawing.Size(82, 24);
            this.lciQualityBadge.MinSize = new System.Drawing.Size(82, 24);
            this.lciQualityBadge.Name = "lciQualityBadge";
            this.lciQualityBadge.Size = new System.Drawing.Size(82, 24);
            this.lciQualityBadge.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciQualityBadge.TextSize = new System.Drawing.Size(0, 0);
            this.lciQualityBadge.TextVisible = false;
            // 
            // lciPreviewHeader
            // 
            this.lciPreviewHeader.Control = this.lblPreviewHeader;
            this.lciPreviewHeader.Location = new System.Drawing.Point(0, 42);
            this.lciPreviewHeader.Name = "lciPreviewHeader";
            this.lciPreviewHeader.Size = new System.Drawing.Size(700, 17);
            this.lciPreviewHeader.TextSize = new System.Drawing.Size(0, 0);
            this.lciPreviewHeader.TextVisible = false;
            // 
            // lciRtxtPreview
            // 
            this.lciRtxtPreview.Control = this.rtxtPreview;
            this.lciRtxtPreview.Location = new System.Drawing.Point(0, 59);
            this.lciRtxtPreview.Name = "lciRtxtPreview";
            this.lciRtxtPreview.Size = new System.Drawing.Size(700, 358);
            this.lciRtxtPreview.TextSize = new System.Drawing.Size(0, 0);
            this.lciRtxtPreview.TextVisible = false;
            // 
            // lciNote
            // 
            this.lciNote.Control = this.lblNote;
            this.lciNote.Location = new System.Drawing.Point(0, 417);
            this.lciNote.Name = "lciNote";
            this.lciNote.Size = new System.Drawing.Size(700, 17);
            this.lciNote.TextSize = new System.Drawing.Size(0, 0);
            this.lciNote.TextVisible = false;
            // 
            // emptySpaceBottom
            // 
            this.emptySpaceBottom.AllowHotTrack = false;
            this.emptySpaceBottom.Location = new System.Drawing.Point(0, 434);
            this.emptySpaceBottom.Name = "emptySpaceBottom";
            this.emptySpaceBottom.Size = new System.Drawing.Size(484, 26);
            this.emptySpaceBottom.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBtnConfirm
            // 
            this.lciBtnConfirm.Control = this.btnConfirm;
            this.lciBtnConfirm.Location = new System.Drawing.Point(484, 434);
            this.lciBtnConfirm.MaxSize = new System.Drawing.Size(130, 26);
            this.lciBtnConfirm.MinSize = new System.Drawing.Size(130, 26);
            this.lciBtnConfirm.Name = "lciBtnConfirm";
            this.lciBtnConfirm.Size = new System.Drawing.Size(130, 26);
            this.lciBtnConfirm.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnConfirm.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnConfirm.TextVisible = false;
            // 
            // lciBtnCancel
            // 
            this.lciBtnCancel.Control = this.btnCancel;
            this.lciBtnCancel.Location = new System.Drawing.Point(614, 434);
            this.lciBtnCancel.MaxSize = new System.Drawing.Size(86, 26);
            this.lciBtnCancel.MinSize = new System.Drawing.Size(86, 26);
            this.lciBtnCancel.Name = "lciBtnCancel";
            this.lciBtnCancel.Size = new System.Drawing.Size(86, 26);
            this.lciBtnCancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnCancel.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnCancel.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(166, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(464, 18);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmSelectZaloTemplate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 480);
            this.Controls.Add(this.layoutControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectZaloTemplate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gửi tin Zalo nhắc tái khám";
            this.Load += new System.EventHandler(this.frmSelectZaloTemplate_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboTemplate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCountLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCountValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGatewayLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGatewayValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciQualityBadge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPreviewHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRtxtPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnConfirm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblPatientCountCaption;
        private DevExpress.XtraEditors.LabelControl lblPatientCountValue;
        private DevExpress.XtraEditors.LabelControl lblGatewayCaption;
        private DevExpress.XtraEditors.LabelControl lblGatewayValue;
        private DevExpress.XtraEditors.LookUpEdit cboTemplate;
        private DevExpress.XtraEditors.LabelControl lblQualityBadge;
        private DevExpress.XtraEditors.LabelControl lblPreviewHeader;
        private System.Windows.Forms.RichTextBox rtxtPreview;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraEditors.SimpleButton btnConfirm;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCountLabel;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCountValue;
        private DevExpress.XtraLayout.LayoutControlItem lciGatewayLabel;
        private DevExpress.XtraLayout.LayoutControlItem lciGatewayValue;
        private DevExpress.XtraLayout.LayoutControlItem lciTemplate;
        private DevExpress.XtraLayout.LayoutControlItem lciQualityBadge;
        private DevExpress.XtraLayout.LayoutControlItem lciPreviewHeader;
        private DevExpress.XtraLayout.LayoutControlItem lciRtxtPreview;
        private DevExpress.XtraLayout.LayoutControlItem lciNote;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceBottom;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnConfirm;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnCancel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
