namespace HIS.Desktop.Plugins.HisReceivingReportList
{
    partial class frmReceivingDetail
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.grvReceivingDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReportID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDetailCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTransTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIssueBank = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBeneficiaryBank = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRealAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMCC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRefID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreditTrace = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreditNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReserveInfo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReceivingDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Location = new System.Drawing.Point(3, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(793, 443);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.grvReceivingDetail;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(793, 443);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvReceivingDetail});
            // 
            // grvReceivingDetail
            // 
            this.grvReceivingDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSTT,
            this.colReportID,
            this.colDetailCode,
            this.colTransTime,
            this.colAmount,
            this.colIssueBank,
            this.colStatus,
            this.colBeneficiaryBank,
            this.colRealAccount,
            this.colMCC,
            this.colRefID,
            this.colCreditTrace,
            this.colCreditNumber,
            this.colReserveInfo,
            this.colCreateTime,
            this.colModifyTime,
            this.colCreator,
            this.colModifier});
            this.grvReceivingDetail.GridControl = this.gridControl1;
            this.grvReceivingDetail.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grvReceivingDetail.Name = "grvReceivingDetail";
            this.grvReceivingDetail.OptionsMenu.ShowAutoFilterRowItem = false;
            this.grvReceivingDetail.OptionsView.ColumnAutoWidth = false;
            this.grvReceivingDetail.OptionsView.ShowGroupPanel = false;
            this.grvReceivingDetail.OptionsView.ShowIndicator = false;
            this.grvReceivingDetail.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grvReceivingDetail_CustomUnboundColumnData);
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.Name = "colSTT";
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 35;
            // 
            // colReportID
            // 
            this.colReportID.Caption = "Mã";
            this.colReportID.FieldName = "RECEIVING_REPORT_ID";
            this.colReportID.Name = "colReportID";
            this.colReportID.Visible = true;
            this.colReportID.VisibleIndex = 1;
            this.colReportID.Width = 35;
            // 
            // colDetailCode
            // 
            this.colDetailCode.Caption = "Mã tài khoản ảo";
            this.colDetailCode.FieldName = "RECEIVING_DETAIL_CODE ";
            this.colDetailCode.Name = "colDetailCode";
            this.colDetailCode.Visible = true;
            this.colDetailCode.VisibleIndex = 2;
            this.colDetailCode.Width = 90;
            // 
            // colTransTime
            // 
            this.colTransTime.Caption = "Thời gian giao dịch";
            this.colTransTime.FieldName = "TRANS_DATETIME_STR";
            this.colTransTime.Name = "colTransTime";
            this.colTransTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colTransTime.Visible = true;
            this.colTransTime.VisibleIndex = 3;
            this.colTransTime.Width = 100;
            // 
            // colAmount
            // 
            this.colAmount.Caption = "Tiền";
            this.colAmount.FieldName = "AMOUNT ";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 4;
            this.colAmount.Width = 50;
            // 
            // colIssueBank
            // 
            this.colIssueBank.Caption = "Mã định danh người chuyển";
            this.colIssueBank.FieldName = "ISSUE_BANK ";
            this.colIssueBank.Name = "colIssueBank";
            this.colIssueBank.Visible = true;
            this.colIssueBank.VisibleIndex = 5;
            this.colIssueBank.Width = 140;
            // 
            // colStatus
            // 
            this.colStatus.Caption = "Trạng thái";
            this.colStatus.FieldName = "STATUS ";
            this.colStatus.Name = "colStatus";
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 6;
            this.colStatus.Width = 60;
            // 
            // colBeneficiaryBank
            // 
            this.colBeneficiaryBank.Caption = "Tài khoản định danh người nhận";
            this.colBeneficiaryBank.FieldName = "BENEFICIARY_BANK ";
            this.colBeneficiaryBank.Name = "colBeneficiaryBank";
            this.colBeneficiaryBank.Visible = true;
            this.colBeneficiaryBank.VisibleIndex = 7;
            this.colBeneficiaryBank.Width = 165;
            // 
            // colRealAccount
            // 
            this.colRealAccount.Caption = "Số tài khoản người nhận";
            this.colRealAccount.FieldName = "REAL_MERCHANT_ACCOUNT";
            this.colRealAccount.Name = "colRealAccount";
            this.colRealAccount.Visible = true;
            this.colRealAccount.VisibleIndex = 8;
            this.colRealAccount.Width = 150;
            // 
            // colMCC
            // 
            this.colMCC.Caption = "Mã đại lý";
            this.colMCC.FieldName = "MCC ";
            this.colMCC.Name = "colMCC";
            this.colMCC.Visible = true;
            this.colMCC.VisibleIndex = 9;
            this.colMCC.Width = 90;
            // 
            // colRefID
            // 
            this.colRefID.Caption = "Số tham chiếu giao dịch";
            this.colRefID.FieldName = "REF_ID ";
            this.colRefID.Name = "colRefID";
            this.colRefID.Visible = true;
            this.colRefID.VisibleIndex = 10;
            this.colRefID.Width = 110;
            // 
            // colCreditTrace
            // 
            this.colCreditTrace.Caption = "Số lưu vết giao dịch ";
            this.colCreditTrace.FieldName = "CREDIT_TRACE ";
            this.colCreditTrace.Name = "colCreditTrace";
            this.colCreditTrace.Visible = true;
            this.colCreditTrace.VisibleIndex = 11;
            this.colCreditTrace.Width = 110;
            // 
            // colCreditNumber
            // 
            this.colCreditNumber.Caption = "Số tham chiếu của thông điệp";
            this.colCreditNumber.FieldName = "CREDIT_REFERENCE_NUMBER";
            this.colCreditNumber.Name = "colCreditNumber";
            this.colCreditNumber.Visible = true;
            this.colCreditNumber.VisibleIndex = 12;
            this.colCreditNumber.Width = 110;
            // 
            // colReserveInfo
            // 
            this.colReserveInfo.Caption = "Thông tin dự phòng";
            this.colReserveInfo.FieldName = "RESERVE_INFO";
            this.colReserveInfo.Name = "colReserveInfo";
            this.colReserveInfo.Visible = true;
            this.colReserveInfo.VisibleIndex = 13;
            this.colReserveInfo.Width = 100;
            // 
            // colCreateTime
            // 
            this.colCreateTime.Caption = "Ngày tạo";
            this.colCreateTime.FieldName = "CREATE_TIME_STR";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 14;
            this.colCreateTime.Width = 70;
            // 
            // colModifyTime
            // 
            this.colModifyTime.Caption = "Ngày sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME_STR";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 15;
            this.colModifyTime.Width = 70;
            // 
            // colCreator
            // 
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 16;
            this.colCreator.Width = 70;
            // 
            // colModifier
            // 
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 17;
            this.colModifier.Width = 70;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(793, 443);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(793, 443);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmReceivingDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmReceivingDetail";
            this.Text = "Báo cáo chi tiết";
            this.Load += new System.EventHandler(this.frmReceivingDetail_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvReceivingDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView grvReceivingDetail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colReportID;
        private DevExpress.XtraGrid.Columns.GridColumn colDetailCode;
        private DevExpress.XtraGrid.Columns.GridColumn colTransTime;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colIssueBank;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.XtraGrid.Columns.GridColumn colBeneficiaryBank;
        private DevExpress.XtraGrid.Columns.GridColumn colRealAccount;
        private DevExpress.XtraGrid.Columns.GridColumn colMCC;
        private DevExpress.XtraGrid.Columns.GridColumn colRefID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreditTrace;
        private DevExpress.XtraGrid.Columns.GridColumn colCreditNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colReserveInfo;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;
    }
}