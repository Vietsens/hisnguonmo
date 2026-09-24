namespace HIS.Desktop.Plugins.KskServiceEditList
{
    partial class frmKskServiceEditResult
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
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlResult = new DevExpress.XtraGrid.GridControl();
            this.gridViewResult = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcResStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResAction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResResult = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcResDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblSummary = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciSummary = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGridResult = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnExport = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnClose = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnClose);
            this.layoutControl1.Controls.Add(this.btnExport);
            this.layoutControl1.Controls.Add(this.gridControlResult);
            this.layoutControl1.Controls.Add(this.lblSummary);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1000, 560);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(882, 532);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(116, 22);
            this.btnClose.StyleController = this.layoutControl1;
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // btnExport
            //
            this.btnExport.Location = new System.Drawing.Point(762, 532);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(116, 22);
            this.btnExport.StyleController = this.layoutControl1;
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Xuất Excel";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            //
            // gridControlResult
            //
            this.gridControlResult.Location = new System.Drawing.Point(2, 62);
            this.gridControlResult.MainView = this.gridViewResult;
            this.gridControlResult.Name = "gridControlResult";
            this.gridControlResult.Size = new System.Drawing.Size(996, 466);
            this.gridControlResult.TabIndex = 0;
            this.gridControlResult.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewResult});
            //
            // gridViewResult
            //
            this.gridViewResult.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcResStt,
            this.gcResTreatmentCode,
            this.gcResPatientName,
            this.gcResAction,
            this.gcResServiceCode,
            this.gcResServiceName,
            this.gcResResult,
            this.gcResDescription});
            this.gridViewResult.GridControl = this.gridControlResult;
            this.gridViewResult.Name = "gridViewResult";
            this.gridViewResult.OptionsBehavior.Editable = false;
            this.gridViewResult.OptionsView.ShowAutoFilterRow = true;
            this.gridViewResult.OptionsView.ShowGroupPanel = false;
            this.gridViewResult.OptionsView.ShowIndicator = false;
            this.gridViewResult.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewResult_RowCellStyle);
            this.gridViewResult.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewResult_CustomUnboundColumnData);
            //
            // gcResStt
            //
            this.gcResStt.Caption = "STT";
            this.gcResStt.FieldName = "STT";
            this.gcResStt.Name = "gcResStt";
            this.gcResStt.OptionsFilter.AllowFilter = false;
            this.gcResStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcResStt.Visible = true;
            this.gcResStt.VisibleIndex = 0;
            this.gcResStt.Width = 45;
            //
            // gcResTreatmentCode
            //
            this.gcResTreatmentCode.Caption = "Mã điều trị";
            this.gcResTreatmentCode.FieldName = "TreatmentCode";
            this.gcResTreatmentCode.Name = "gcResTreatmentCode";
            this.gcResTreatmentCode.Visible = true;
            this.gcResTreatmentCode.VisibleIndex = 1;
            this.gcResTreatmentCode.Width = 100;
            //
            // gcResPatientName
            //
            this.gcResPatientName.Caption = "Họ tên";
            this.gcResPatientName.FieldName = "PatientName";
            this.gcResPatientName.Name = "gcResPatientName";
            this.gcResPatientName.Visible = true;
            this.gcResPatientName.VisibleIndex = 2;
            this.gcResPatientName.Width = 160;
            //
            // gcResAction
            //
            this.gcResAction.Caption = "Thao tác";
            this.gcResAction.FieldName = "ActionDisplay";
            this.gcResAction.Name = "gcResAction";
            this.gcResAction.Visible = true;
            this.gcResAction.VisibleIndex = 3;
            this.gcResAction.Width = 110;
            //
            // gcResServiceCode
            //
            this.gcResServiceCode.Caption = "Mã dịch vụ";
            this.gcResServiceCode.FieldName = "ServiceCode";
            this.gcResServiceCode.Name = "gcResServiceCode";
            this.gcResServiceCode.Visible = true;
            this.gcResServiceCode.VisibleIndex = 4;
            this.gcResServiceCode.Width = 90;
            //
            // gcResServiceName
            //
            this.gcResServiceName.Caption = "Tên dịch vụ";
            this.gcResServiceName.FieldName = "ServiceName";
            this.gcResServiceName.Name = "gcResServiceName";
            this.gcResServiceName.Visible = true;
            this.gcResServiceName.VisibleIndex = 5;
            this.gcResServiceName.Width = 180;
            //
            // gcResResult
            //
            this.gcResResult.Caption = "Kết quả";
            this.gcResResult.FieldName = "ResultDisplay";
            this.gcResResult.Name = "gcResResult";
            this.gcResResult.Visible = true;
            this.gcResResult.VisibleIndex = 6;
            this.gcResResult.Width = 80;
            //
            // gcResDescription
            //
            this.gcResDescription.Caption = "Lý do";
            this.gcResDescription.FieldName = "Description";
            this.gcResDescription.Name = "gcResDescription";
            this.gcResDescription.Visible = true;
            this.gcResDescription.VisibleIndex = 7;
            this.gcResDescription.Width = 280;
            //
            // lblSummary
            //
            this.lblSummary.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSummary.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblSummary.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblSummary.Location = new System.Drawing.Point(104, 2);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(894, 56);
            this.lblSummary.StyleController = this.layoutControl1;
            this.lblSummary.TabIndex = 3;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciSummary,
            this.lciGridResult,
            this.emptySpaceItem1,
            this.lciBtnExport,
            this.lciBtnClose});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1000, 560);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciSummary
            //
            this.lciSummary.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSummary.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSummary.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lciSummary.Control = this.lblSummary;
            this.lciSummary.Location = new System.Drawing.Point(0, 0);
            this.lciSummary.Name = "lciSummary";
            this.lciSummary.Size = new System.Drawing.Size(1000, 60);
            this.lciSummary.Text = "Tổng hợp:";
            this.lciSummary.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSummary.TextSize = new System.Drawing.Size(97, 20);
            this.lciSummary.TextToControlDistance = 5;
            //
            // lciGridResult
            //
            this.lciGridResult.Control = this.gridControlResult;
            this.lciGridResult.Location = new System.Drawing.Point(0, 60);
            this.lciGridResult.Name = "lciGridResult";
            this.lciGridResult.Size = new System.Drawing.Size(1000, 470);
            this.lciGridResult.TextSize = new System.Drawing.Size(0, 0);
            this.lciGridResult.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 530);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(760, 30);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciBtnExport
            //
            this.lciBtnExport.Control = this.btnExport;
            this.lciBtnExport.Location = new System.Drawing.Point(760, 530);
            this.lciBtnExport.Name = "lciBtnExport";
            this.lciBtnExport.Size = new System.Drawing.Size(120, 30);
            this.lciBtnExport.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnExport.TextVisible = false;
            //
            // lciBtnClose
            //
            this.lciBtnClose.Control = this.btnClose;
            this.lciBtnClose.Location = new System.Drawing.Point(880, 530);
            this.lciBtnClose.Name = "lciBtnClose";
            this.lciBtnClose.Size = new System.Drawing.Size(120, 30);
            this.lciBtnClose.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnClose.TextVisible = false;
            //
            // frmKskServiceEditResult
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 560);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmKskServiceEditResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kết quả sửa dịch vụ";
            this.Load += new System.EventHandler(this.frmKskServiceEditResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGridResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnExport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraGrid.GridControl gridControlResult;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewResult;
        private DevExpress.XtraGrid.Columns.GridColumn gcResStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcResTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcResPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gcResAction;
        private DevExpress.XtraGrid.Columns.GridColumn gcResServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcResServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gcResResult;
        private DevExpress.XtraGrid.Columns.GridColumn gcResDescription;
        private DevExpress.XtraEditors.SimpleButton btnExport;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControlItem lciSummary;
        private DevExpress.XtraLayout.LayoutControlItem lciGridResult;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnExport;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnClose;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
