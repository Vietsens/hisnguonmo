namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    partial class frmTienGiamDinhResult
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            this.splitContainerMain = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridControlSummary = new DevExpress.XtraGrid.GridControl();
            this.gridViewSummary = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColErrorCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColStatusName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControlDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColGroupName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColSeverityName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColErrorCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.lblSummary = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainerMain
            //
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Horizontal = false;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Panel1.Controls.Add(this.gridControlSummary);
            this.splitContainerMain.Panel1.Text = "Panel1";
            this.splitContainerMain.Panel2.Controls.Add(this.gridControlDetail);
            this.splitContainerMain.Panel2.Text = "Panel2";
            this.splitContainerMain.Size = new System.Drawing.Size(984, 486);
            this.splitContainerMain.SplitterPosition = 220;
            this.splitContainerMain.TabIndex = 0;
            this.splitContainerMain.Text = "splitContainerMain";
            //
            // gridControlSummary
            //
            this.gridControlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlSummary.Location = new System.Drawing.Point(0, 0);
            this.gridControlSummary.MainView = this.gridViewSummary;
            this.gridControlSummary.Name = "gridControlSummary";
            this.gridControlSummary.Size = new System.Drawing.Size(984, 220);
            this.gridControlSummary.TabIndex = 0;
            this.gridControlSummary.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSummary});
            //
            // gridViewSummary
            //
            this.gridViewSummary.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColTreatmentCode,
            this.gridColPatientName,
            this.gridColErrorCount,
            this.gridColStatusName});
            this.gridViewSummary.GridControl = this.gridControlSummary;
            this.gridViewSummary.Name = "gridViewSummary";
            this.gridViewSummary.OptionsBehavior.Editable = false;
            this.gridViewSummary.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewSummary.OptionsView.ShowGroupPanel = false;
            this.gridViewSummary.OptionsView.ShowIndicator = false;
            //
            // gridColTreatmentCode
            //
            this.gridColTreatmentCode.Caption = "Mã điều trị";
            this.gridColTreatmentCode.FieldName = "TreatmentCode";
            this.gridColTreatmentCode.Name = "gridColTreatmentCode";
            this.gridColTreatmentCode.OptionsColumn.AllowEdit = false;
            this.gridColTreatmentCode.Visible = true;
            this.gridColTreatmentCode.VisibleIndex = 0;
            this.gridColTreatmentCode.Width = 150;
            //
            // gridColPatientName
            //
            this.gridColPatientName.Caption = "Tên bệnh nhân";
            this.gridColPatientName.FieldName = "PatientName";
            this.gridColPatientName.Name = "gridColPatientName";
            this.gridColPatientName.OptionsColumn.AllowEdit = false;
            this.gridColPatientName.Visible = true;
            this.gridColPatientName.VisibleIndex = 1;
            this.gridColPatientName.Width = 240;
            //
            // gridColErrorCount
            //
            this.gridColErrorCount.Caption = "Số lỗi";
            this.gridColErrorCount.FieldName = "TotalErrorCount";
            this.gridColErrorCount.Name = "gridColErrorCount";
            this.gridColErrorCount.OptionsColumn.AllowEdit = false;
            this.gridColErrorCount.Visible = true;
            this.gridColErrorCount.VisibleIndex = 2;
            this.gridColErrorCount.Width = 80;
            //
            // gridColStatusName
            //
            this.gridColStatusName.Caption = "Mức độ";
            this.gridColStatusName.FieldName = "StatusName";
            this.gridColStatusName.Name = "gridColStatusName";
            this.gridColStatusName.OptionsColumn.AllowEdit = false;
            this.gridColStatusName.Visible = true;
            this.gridColStatusName.VisibleIndex = 3;
            this.gridColStatusName.Width = 200;
            //
            // gridControlDetail
            //
            this.gridControlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlDetail.Location = new System.Drawing.Point(0, 0);
            this.gridControlDetail.MainView = this.gridViewDetail;
            this.gridControlDetail.Name = "gridControlDetail";
            this.gridControlDetail.Size = new System.Drawing.Size(984, 256);
            this.gridControlDetail.TabIndex = 1;
            this.gridControlDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDetail});
            //
            // gridViewDetail
            //
            this.gridViewDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColGroupName,
            this.gridColSeverityName,
            this.gridColErrorCode,
            this.gridColDescription});
            this.gridViewDetail.GridControl = this.gridControlDetail;
            this.gridViewDetail.Name = "gridViewDetail";
            this.gridViewDetail.OptionsBehavior.Editable = false;
            this.gridViewDetail.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewDetail.OptionsView.ShowIndicator = false;
            //
            // gridColGroupName
            //
            this.gridColGroupName.Caption = "Nhóm lỗi";
            this.gridColGroupName.FieldName = "GroupName";
            this.gridColGroupName.Name = "gridColGroupName";
            this.gridColGroupName.OptionsColumn.AllowEdit = false;
            this.gridColGroupName.Visible = true;
            this.gridColGroupName.VisibleIndex = 0;
            this.gridColGroupName.Width = 220;
            //
            // gridColSeverityName
            //
            this.gridColSeverityName.Caption = "Mức độ";
            this.gridColSeverityName.FieldName = "SeverityName";
            this.gridColSeverityName.Name = "gridColSeverityName";
            this.gridColSeverityName.OptionsColumn.AllowEdit = false;
            this.gridColSeverityName.Visible = true;
            this.gridColSeverityName.VisibleIndex = 1;
            this.gridColSeverityName.Width = 110;
            //
            // gridColErrorCode
            //
            this.gridColErrorCode.Caption = "Mã lỗi";
            this.gridColErrorCode.FieldName = "Code";
            this.gridColErrorCode.Name = "gridColErrorCode";
            this.gridColErrorCode.OptionsColumn.AllowEdit = false;
            this.gridColErrorCode.Visible = true;
            this.gridColErrorCode.VisibleIndex = 2;
            this.gridColErrorCode.Width = 140;
            //
            // gridColDescription
            //
            this.gridColDescription.Caption = "Mô tả lỗi";
            this.gridColDescription.FieldName = "Description";
            this.gridColDescription.Name = "gridColDescription";
            this.gridColDescription.OptionsColumn.AllowEdit = false;
            this.gridColDescription.Visible = true;
            this.gridColDescription.VisibleIndex = 3;
            this.gridColDescription.Width = 500;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.lblSummary);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 486);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(984, 36);
            this.panelBottom.TabIndex = 1;
            //
            // lblSummary
            //
            this.lblSummary.Location = new System.Drawing.Point(10, 11);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(0, 13);
            this.lblSummary.TabIndex = 0;
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(882, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmTienGiamDinhResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 522);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.panelBottom);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "frmTienGiamDinhResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kết quả kiểm tra tiền giám định";
            this.Load += new System.EventHandler(this.frmTienGiamDinhResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerMain;
        private DevExpress.XtraGrid.GridControl gridControlSummary;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSummary;
        private DevExpress.XtraGrid.Columns.GridColumn gridColTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColErrorCount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColStatusName;
        private DevExpress.XtraGrid.GridControl gridControlDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gridColGroupName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSeverityName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColErrorCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColDescription;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
