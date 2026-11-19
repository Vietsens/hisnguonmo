namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    partial class frmXml130SendLog
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
            this.panelFilter = new DevExpress.XtraEditors.PanelControl();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.dtTo = new DevExpress.XtraEditors.DateEdit();
            this.lblResult = new DevExpress.XtraEditors.LabelControl();
            this.cboResult = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblMode = new DevExpress.XtraEditors.LabelControl();
            this.cboMode = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.lblKeepDays = new DevExpress.XtraEditors.LabelControl();
            this.gridControlLog = new DevExpress.XtraGrid.GridControl();
            this.gridViewLog = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColSendTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColSendMode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColSendResult = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColCheckCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColSendDesc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMachineName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColRoundCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.lblSummary = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).BeginInit();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboResult.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // panelFilter
            //
            this.panelFilter.Controls.Add(this.lblFrom);
            this.panelFilter.Controls.Add(this.dtFrom);
            this.panelFilter.Controls.Add(this.lblTo);
            this.panelFilter.Controls.Add(this.dtTo);
            this.panelFilter.Controls.Add(this.lblResult);
            this.panelFilter.Controls.Add(this.cboResult);
            this.panelFilter.Controls.Add(this.lblMode);
            this.panelFilter.Controls.Add(this.cboMode);
            this.panelFilter.Controls.Add(this.lblTreatmentCode);
            this.panelFilter.Controls.Add(this.txtTreatmentCode);
            this.panelFilter.Controls.Add(this.btnSearch);
            this.panelFilter.Controls.Add(this.btnExport);
            this.panelFilter.Controls.Add(this.lblKeepDays);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(1084, 70);
            this.panelFilter.TabIndex = 0;
            //
            // lblFrom
            //
            this.lblFrom.Location = new System.Drawing.Point(12, 14);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(42, 13);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Từ ngày:";
            //
            // dtFrom
            //
            this.dtFrom.EditValue = null;
            this.dtFrom.Location = new System.Drawing.Point(78, 11);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtFrom.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFrom.Size = new System.Drawing.Size(140, 20);
            this.dtFrom.TabIndex = 1;
            //
            // lblTo
            //
            this.lblTo.Location = new System.Drawing.Point(230, 14);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(48, 13);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến ngày:";
            //
            // dtTo
            //
            this.dtTo.EditValue = null;
            this.dtTo.Location = new System.Drawing.Point(296, 11);
            this.dtTo.Name = "dtTo";
            this.dtTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dtTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTo.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTo.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTo.Size = new System.Drawing.Size(140, 20);
            this.dtTo.TabIndex = 3;
            //
            // lblResult
            //
            this.lblResult.Location = new System.Drawing.Point(448, 14);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(55, 13);
            this.lblResult.TabIndex = 4;
            this.lblResult.Text = "Kết quả gửi:";
            //
            // cboResult
            //
            this.cboResult.Location = new System.Drawing.Point(530, 11);
            this.cboResult.Name = "cboResult";
            this.cboResult.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboResult.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboResult.Size = new System.Drawing.Size(130, 20);
            this.cboResult.TabIndex = 5;
            //
            // lblMode
            //
            this.lblMode.Location = new System.Drawing.Point(672, 14);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(63, 13);
            this.lblMode.TabIndex = 6;
            this.lblMode.Text = "Hình thức gửi:";
            //
            // cboMode
            //
            this.cboMode.Location = new System.Drawing.Point(756, 11);
            this.cboMode.Name = "cboMode";
            this.cboMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboMode.Size = new System.Drawing.Size(150, 20);
            this.cboMode.TabIndex = 7;
            //
            // lblTreatmentCode
            //
            this.lblTreatmentCode.Location = new System.Drawing.Point(12, 43);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Size = new System.Drawing.Size(55, 13);
            this.lblTreatmentCode.TabIndex = 8;
            this.lblTreatmentCode.Text = "Mã điều trị:";
            //
            // txtTreatmentCode
            //
            this.txtTreatmentCode.Location = new System.Drawing.Point(78, 40);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Size = new System.Drawing.Size(140, 20);
            this.txtTreatmentCode.TabIndex = 9;
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(230, 39);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 22);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tra cứu";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnExport
            //
            this.btnExport.Location = new System.Drawing.Point(336, 39);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 22);
            this.btnExport.TabIndex = 11;
            this.btnExport.Text = "Kết xuất Excel";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // lblKeepDays
            //
            this.lblKeepDays.Location = new System.Drawing.Point(448, 43);
            this.lblKeepDays.Name = "lblKeepDays";
            this.lblKeepDays.Size = new System.Drawing.Size(0, 13);
            this.lblKeepDays.TabIndex = 12;
            //
            // gridControlLog
            //
            this.gridControlLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlLog.Location = new System.Drawing.Point(0, 70);
            this.gridControlLog.MainView = this.gridViewLog;
            this.gridControlLog.Name = "gridControlLog";
            this.gridControlLog.Size = new System.Drawing.Size(1084, 482);
            this.gridControlLog.TabIndex = 1;
            this.gridControlLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLog});
            //
            // gridViewLog
            //
            this.gridViewLog.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColStt,
            this.gridColSendTime,
            this.gridColTreatmentCode,
            this.gridColPatientCode,
            this.gridColSendMode,
            this.gridColSendResult,
            this.gridColCheckCode,
            this.gridColSendDesc,
            this.gridColCreator,
            this.gridColMachineName,
            this.gridColRoundCode});
            this.gridViewLog.GridControl = this.gridControlLog;
            this.gridViewLog.Name = "gridViewLog";
            this.gridViewLog.OptionsBehavior.Editable = false;
            this.gridViewLog.OptionsView.ColumnAutoWidth = false;
            this.gridViewLog.OptionsView.ShowGroupPanel = false;
            this.gridViewLog.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewLog_RowCellStyle);
            this.gridViewLog.DoubleClick += new System.EventHandler(this.gridViewLog_DoubleClick);
            //
            // gridColStt
            //
            this.gridColStt.Caption = "STT";
            this.gridColStt.FieldName = "STT";
            this.gridColStt.Name = "gridColStt";
            this.gridColStt.OptionsColumn.FixedWidth = true;
            this.gridColStt.Visible = true;
            this.gridColStt.VisibleIndex = 0;
            this.gridColStt.Width = 50;
            //
            // gridColSendTime
            //
            this.gridColSendTime.Caption = "Thời gian gửi";
            this.gridColSendTime.FieldName = "SEND_TIME_TEXT";
            this.gridColSendTime.Name = "gridColSendTime";
            this.gridColSendTime.OptionsColumn.FixedWidth = true;
            this.gridColSendTime.Visible = true;
            this.gridColSendTime.VisibleIndex = 1;
            this.gridColSendTime.Width = 130;
            //
            // gridColTreatmentCode
            //
            this.gridColTreatmentCode.Caption = "Mã điều trị";
            this.gridColTreatmentCode.FieldName = "TREATMENT_CODE";
            this.gridColTreatmentCode.Name = "gridColTreatmentCode";
            this.gridColTreatmentCode.OptionsColumn.FixedWidth = true;
            this.gridColTreatmentCode.Visible = true;
            this.gridColTreatmentCode.VisibleIndex = 2;
            this.gridColTreatmentCode.Width = 110;
            //
            // gridColPatientCode
            //
            this.gridColPatientCode.Caption = "Mã bệnh nhân";
            this.gridColPatientCode.FieldName = "PATIENT_CODE";
            this.gridColPatientCode.Name = "gridColPatientCode";
            this.gridColPatientCode.OptionsColumn.FixedWidth = true;
            this.gridColPatientCode.Visible = true;
            this.gridColPatientCode.VisibleIndex = 3;
            this.gridColPatientCode.Width = 110;
            //
            // gridColSendMode
            //
            this.gridColSendMode.Caption = "Hình thức gửi";
            this.gridColSendMode.FieldName = "SEND_MODE_TEXT";
            this.gridColSendMode.Name = "gridColSendMode";
            this.gridColSendMode.OptionsColumn.FixedWidth = true;
            this.gridColSendMode.Visible = true;
            this.gridColSendMode.VisibleIndex = 4;
            this.gridColSendMode.Width = 120;
            //
            // gridColSendResult
            //
            this.gridColSendResult.Caption = "Kết quả";
            this.gridColSendResult.FieldName = "SEND_RESULT_TEXT";
            this.gridColSendResult.Name = "gridColSendResult";
            this.gridColSendResult.OptionsColumn.FixedWidth = true;
            this.gridColSendResult.Visible = true;
            this.gridColSendResult.VisibleIndex = 5;
            this.gridColSendResult.Width = 90;
            //
            // gridColCheckCode
            //
            this.gridColCheckCode.Caption = "Mã kiểm tra";
            this.gridColCheckCode.FieldName = "CHECK_CODE";
            this.gridColCheckCode.Name = "gridColCheckCode";
            this.gridColCheckCode.OptionsColumn.FixedWidth = true;
            this.gridColCheckCode.Visible = true;
            this.gridColCheckCode.VisibleIndex = 6;
            this.gridColCheckCode.Width = 110;
            //
            // gridColSendDesc
            //
            this.gridColSendDesc.Caption = "Nội dung Cổng trả về";
            this.gridColSendDesc.FieldName = "SEND_DESC";
            this.gridColSendDesc.Name = "gridColSendDesc";
            this.gridColSendDesc.Visible = true;
            this.gridColSendDesc.VisibleIndex = 7;
            this.gridColSendDesc.Width = 280;
            //
            // gridColCreator
            //
            this.gridColCreator.Caption = "Tài khoản";
            this.gridColCreator.FieldName = "CREATOR";
            this.gridColCreator.Name = "gridColCreator";
            this.gridColCreator.OptionsColumn.FixedWidth = true;
            this.gridColCreator.Visible = true;
            this.gridColCreator.VisibleIndex = 8;
            this.gridColCreator.Width = 100;
            //
            // gridColMachineName
            //
            this.gridColMachineName.Caption = "Máy thực hiện";
            this.gridColMachineName.FieldName = "MACHINE_NAME";
            this.gridColMachineName.Name = "gridColMachineName";
            this.gridColMachineName.OptionsColumn.FixedWidth = true;
            this.gridColMachineName.Visible = true;
            this.gridColMachineName.VisibleIndex = 9;
            this.gridColMachineName.Width = 120;
            //
            // gridColRoundCode
            //
            this.gridColRoundCode.Caption = "Mã lượt gửi";
            this.gridColRoundCode.FieldName = "SEND_ROUND_CODE";
            this.gridColRoundCode.Name = "gridColRoundCode";
            this.gridColRoundCode.OptionsColumn.FixedWidth = true;
            this.gridColRoundCode.Visible = true;
            this.gridColRoundCode.VisibleIndex = 10;
            this.gridColRoundCode.Width = 170;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.lblSummary);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 552);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1084, 36);
            this.panelBottom.TabIndex = 2;
            //
            // lblSummary
            //
            this.lblSummary.Location = new System.Drawing.Point(12, 12);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(0, 13);
            this.lblSummary.TabIndex = 0;
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(972, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 22);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmXml130SendLog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 588);
            this.Controls.Add(this.gridControlLog);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelFilter);
            this.MinimumSize = new System.Drawing.Size(900, 420);
            this.Name = "frmXml130SendLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhật ký gửi XML 130 lên Cổng giám định BHYT";
            this.Load += new System.EventHandler(this.frmXml130SendLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelFilter)).EndInit();
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboResult.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelFilter;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.DateEdit dtFrom;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.DateEdit dtTo;
        private DevExpress.XtraEditors.LabelControl lblResult;
        private DevExpress.XtraEditors.ComboBoxEdit cboResult;
        private DevExpress.XtraEditors.LabelControl lblMode;
        private DevExpress.XtraEditors.ComboBoxEdit cboMode;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnExport;
        private DevExpress.XtraEditors.LabelControl lblKeepDays;
        private DevExpress.XtraGrid.GridControl gridControlLog;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLog;
        private DevExpress.XtraGrid.Columns.GridColumn gridColStt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSendTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSendMode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSendResult;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCheckCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColSendDesc;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMachineName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColRoundCode;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraEditors.SimpleButton btnClose;
    }
}
