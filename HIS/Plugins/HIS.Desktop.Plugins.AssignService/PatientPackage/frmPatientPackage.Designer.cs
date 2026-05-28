namespace HIS.Desktop.Plugins.AssignService.PatientPackage
{
    partial class frmPatientPackage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridControlPackage = new DevExpress.XtraGrid.GridControl();
            this.gridViewPackage = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColPkgStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgRegisterDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColPkgModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtSearchPackage = new DevExpress.XtraEditors.TextEdit();
            this.lblPackageList = new DevExpress.XtraEditors.LabelControl();
            this.gridControlDt = new DevExpress.XtraGrid.GridControl();
            this.gridViewDt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gColDtCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEditDt = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gColDtServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDtServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDtServiceTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDtAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDtAmountUsed = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gColDtAmountThisTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinEditAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.txtSearchDt = new DevExpress.XtraEditors.TextEdit();
            this.lblDtList = new DevExpress.XtraEditors.LabelControl();
            this.panelControlBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnSelect = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchPackage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditDt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchDt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlBottom)).BeginInit();
            this.panelControlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainerControl1
            //
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.gridControlPackage);
            this.splitContainerControl1.Panel1.Controls.Add(this.txtSearchPackage);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblPackageList);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.gridControlDt);
            this.splitContainerControl1.Panel2.Controls.Add(this.txtSearchDt);
            this.splitContainerControl1.Panel2.Controls.Add(this.lblDtList);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(984, 521);
            this.splitContainerControl1.SplitterPosition = 480;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            //
            // gridControlPackage
            //
            this.gridControlPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPackage.Location = new System.Drawing.Point(0, 44);
            this.gridControlPackage.MainView = this.gridViewPackage;
            this.gridControlPackage.Name = "gridControlPackage";
            this.gridControlPackage.Size = new System.Drawing.Size(480, 477);
            this.gridControlPackage.TabIndex = 1;
            this.gridControlPackage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPackage});
            //
            // gridViewPackage
            //
            this.gridViewPackage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColPkgStt,
            this.gColPkgName,
            this.gColPkgRegisterDate,
            this.gColPkgNote,
            this.gColPkgCreateTime,
            this.gColPkgCreator,
            this.gColPkgModifyTime,
            this.gColPkgModifier});
            this.gridViewPackage.GridControl = this.gridControlPackage;
            this.gridViewPackage.Name = "gridViewPackage";
            this.gridViewPackage.OptionsBehavior.Editable = false;
            this.gridViewPackage.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPackage.OptionsView.ShowGroupPanel = false;
            this.gridViewPackage.OptionsView.ShowIndicator = false;
            //
            // gColPkgStt
            //
            this.gColPkgStt.Caption = "#";
            this.gColPkgStt.FieldName = "STT";
            this.gColPkgStt.Name = "gColPkgStt";
            this.gColPkgStt.OptionsColumn.AllowEdit = false;
            this.gColPkgStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gColPkgStt.Visible = true;
            this.gColPkgStt.VisibleIndex = 0;
            this.gColPkgStt.Width = 40;
            //
            // gColPkgName
            //
            this.gColPkgName.Caption = "Tên gói";
            this.gColPkgName.FieldName = "PACKAGE_NAME";
            this.gColPkgName.Name = "gColPkgName";
            this.gColPkgName.OptionsColumn.AllowEdit = false;
            this.gColPkgName.Visible = true;
            this.gColPkgName.VisibleIndex = 1;
            this.gColPkgName.Width = 160;
            //
            // gColPkgRegisterDate
            //
            this.gColPkgRegisterDate.Caption = "Ngày ĐK";
            this.gColPkgRegisterDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gColPkgRegisterDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gColPkgRegisterDate.FieldName = "REGISTER_DATE";
            this.gColPkgRegisterDate.Name = "gColPkgRegisterDate";
            this.gColPkgRegisterDate.OptionsColumn.AllowEdit = false;
            this.gColPkgRegisterDate.Visible = true;
            this.gColPkgRegisterDate.VisibleIndex = 2;
            this.gColPkgRegisterDate.Width = 80;
            //
            // gColPkgNote
            //
            this.gColPkgNote.Caption = "Ghi chú";
            this.gColPkgNote.FieldName = "NOTE";
            this.gColPkgNote.Name = "gColPkgNote";
            this.gColPkgNote.OptionsColumn.AllowEdit = false;
            this.gColPkgNote.Visible = true;
            this.gColPkgNote.VisibleIndex = 3;
            this.gColPkgNote.Width = 140;
            //
            // gColPkgCreateTime
            //
            this.gColPkgCreateTime.Caption = "Thời gian tạo";
            this.gColPkgCreateTime.FieldName = "CREATE_TIME_STR";
            this.gColPkgCreateTime.Name = "gColPkgCreateTime";
            this.gColPkgCreateTime.OptionsColumn.AllowEdit = false;
            this.gColPkgCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gColPkgCreateTime.Visible = true;
            this.gColPkgCreateTime.VisibleIndex = 4;
            this.gColPkgCreateTime.Width = 110;
            //
            // gColPkgCreator
            //
            this.gColPkgCreator.Caption = "Người tạo";
            this.gColPkgCreator.FieldName = "CREATOR";
            this.gColPkgCreator.Name = "gColPkgCreator";
            this.gColPkgCreator.OptionsColumn.AllowEdit = false;
            this.gColPkgCreator.Visible = true;
            this.gColPkgCreator.VisibleIndex = 5;
            this.gColPkgCreator.Width = 80;
            //
            // gColPkgModifyTime
            //
            this.gColPkgModifyTime.Caption = "Thời gian sửa";
            this.gColPkgModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gColPkgModifyTime.Name = "gColPkgModifyTime";
            this.gColPkgModifyTime.OptionsColumn.AllowEdit = false;
            this.gColPkgModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gColPkgModifyTime.Visible = true;
            this.gColPkgModifyTime.VisibleIndex = 6;
            this.gColPkgModifyTime.Width = 110;
            //
            // gColPkgModifier
            //
            this.gColPkgModifier.Caption = "Người sửa";
            this.gColPkgModifier.FieldName = "MODIFIER";
            this.gColPkgModifier.Name = "gColPkgModifier";
            this.gColPkgModifier.OptionsColumn.AllowEdit = false;
            this.gColPkgModifier.Visible = true;
            this.gColPkgModifier.VisibleIndex = 7;
            this.gColPkgModifier.Width = 80;
            //
            // txtSearchPackage
            //
            this.txtSearchPackage.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearchPackage.Location = new System.Drawing.Point(0, 24);
            this.txtSearchPackage.Name = "txtSearchPackage";
            this.txtSearchPackage.Properties.NullValuePrompt = "Tìm kiếm: Tên gói...";
            this.txtSearchPackage.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchPackage.Size = new System.Drawing.Size(480, 20);
            this.txtSearchPackage.TabIndex = 0;
            //
            // lblPackageList
            //
            this.lblPackageList.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblPackageList.Location = new System.Drawing.Point(0, 0);
            this.lblPackageList.Name = "lblPackageList";
            this.lblPackageList.Padding = new System.Windows.Forms.Padding(4);
            this.lblPackageList.Size = new System.Drawing.Size(480, 24);
            this.lblPackageList.TabIndex = 2;
            this.lblPackageList.Text = "Danh sách gói dịch vụ";
            //
            // gridControlDt
            //
            this.gridControlDt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlDt.Location = new System.Drawing.Point(0, 44);
            this.gridControlDt.MainView = this.gridViewDt;
            this.gridControlDt.Name = "gridControlDt";
            this.gridControlDt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEditDt,
            this.repositoryItemSpinEditAmount});
            this.gridControlDt.Size = new System.Drawing.Size(498, 477);
            this.gridControlDt.TabIndex = 1;
            this.gridControlDt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDt});
            //
            // gridViewDt
            //
            this.gridViewDt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gColDtCheck,
            this.gColDtServiceCode,
            this.gColDtServiceName,
            this.gColDtServiceTypeName,
            this.gColDtAmount,
            this.gColDtAmountUsed,
            this.gColDtAmountThisTime});
            this.gridViewDt.GridControl = this.gridControlDt;
            this.gridViewDt.Name = "gridViewDt";
            this.gridViewDt.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDt.OptionsView.ShowGroupPanel = false;
            this.gridViewDt.OptionsView.ShowIndicator = false;
            //
            // gColDtCheck
            //
            this.gColDtCheck.Caption = "";
            this.gColDtCheck.ColumnEdit = this.repositoryItemCheckEditDt;
            this.gColDtCheck.FieldName = "IsChecked";
            this.gColDtCheck.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gColDtCheck.Name = "gColDtCheck";
            this.gColDtCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gColDtCheck.Visible = true;
            this.gColDtCheck.VisibleIndex = 0;
            this.gColDtCheck.Width = 30;
            //
            // repositoryItemCheckEditDt
            //
            this.repositoryItemCheckEditDt.AutoHeight = false;
            this.repositoryItemCheckEditDt.Name = "repositoryItemCheckEditDt";
            //
            // gColDtServiceCode
            //
            this.gColDtServiceCode.Caption = "Mã DV";
            this.gColDtServiceCode.FieldName = "SV_SERVICE_CODE";
            this.gColDtServiceCode.Name = "gColDtServiceCode";
            this.gColDtServiceCode.OptionsColumn.AllowEdit = false;
            this.gColDtServiceCode.Visible = true;
            this.gColDtServiceCode.VisibleIndex = 1;
            this.gColDtServiceCode.Width = 80;
            //
            // gColDtServiceName
            //
            this.gColDtServiceName.Caption = "Tên dịch vụ";
            this.gColDtServiceName.FieldName = "SERVICE_NAME";
            this.gColDtServiceName.Name = "gColDtServiceName";
            this.gColDtServiceName.OptionsColumn.AllowEdit = false;
            this.gColDtServiceName.Visible = true;
            this.gColDtServiceName.VisibleIndex = 2;
            this.gColDtServiceName.Width = 170;
            //
            // gColDtServiceTypeName
            //
            this.gColDtServiceTypeName.Caption = "Loại DV";
            this.gColDtServiceTypeName.FieldName = "SERVICE_TYPE_NAME";
            this.gColDtServiceTypeName.Name = "gColDtServiceTypeName";
            this.gColDtServiceTypeName.OptionsColumn.AllowEdit = false;
            this.gColDtServiceTypeName.Visible = true;
            this.gColDtServiceTypeName.VisibleIndex = 3;
            this.gColDtServiceTypeName.Width = 80;
            //
            // gColDtAmount
            //
            this.gColDtAmount.Caption = "Trong gói";
            this.gColDtAmount.FieldName = "AMOUNT";
            this.gColDtAmount.Name = "gColDtAmount";
            this.gColDtAmount.OptionsColumn.AllowEdit = false;
            this.gColDtAmount.Visible = true;
            this.gColDtAmount.VisibleIndex = 4;
            this.gColDtAmount.Width = 60;
            //
            // gColDtAmountUsed
            //
            this.gColDtAmountUsed.Caption = "Đã dùng";
            this.gColDtAmountUsed.FieldName = "AMOUNT_USED";
            this.gColDtAmountUsed.Name = "gColDtAmountUsed";
            this.gColDtAmountUsed.OptionsColumn.AllowEdit = false;
            this.gColDtAmountUsed.Visible = true;
            this.gColDtAmountUsed.VisibleIndex = 5;
            this.gColDtAmountUsed.Width = 60;
            //
            // gColDtAmountThisTime
            //
            this.gColDtAmountThisTime.Caption = "Lần này";
            this.gColDtAmountThisTime.ColumnEdit = this.repositoryItemSpinEditAmount;
            this.gColDtAmountThisTime.FieldName = "AmountThisTime";
            this.gColDtAmountThisTime.Name = "gColDtAmountThisTime";
            this.gColDtAmountThisTime.Visible = true;
            this.gColDtAmountThisTime.VisibleIndex = 6;
            this.gColDtAmountThisTime.Width = 60;
            //
            // repositoryItemSpinEditAmount
            //
            this.repositoryItemSpinEditAmount.AutoHeight = false;
            this.repositoryItemSpinEditAmount.IsFloatValue = false;
            this.repositoryItemSpinEditAmount.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.repositoryItemSpinEditAmount.Mask.EditMask = "N0";
            this.repositoryItemSpinEditAmount.Name = "repositoryItemSpinEditAmount";
            //
            // txtSearchDt
            //
            this.txtSearchDt.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSearchDt.Location = new System.Drawing.Point(0, 24);
            this.txtSearchDt.Name = "txtSearchDt";
            this.txtSearchDt.Properties.NullValuePrompt = "Tìm kiếm: Mã / tên dịch vụ...";
            this.txtSearchDt.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearchDt.Size = new System.Drawing.Size(498, 20);
            this.txtSearchDt.TabIndex = 0;
            //
            // lblDtList
            //
            this.lblDtList.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDtList.Location = new System.Drawing.Point(0, 0);
            this.lblDtList.Name = "lblDtList";
            this.lblDtList.Padding = new System.Windows.Forms.Padding(4);
            this.lblDtList.Size = new System.Drawing.Size(498, 24);
            this.lblDtList.TabIndex = 2;
            this.lblDtList.Text = "Dịch vụ trong gói";
            //
            // panelControlBottom
            //
            this.panelControlBottom.Controls.Add(this.btnSelect);
            this.panelControlBottom.Controls.Add(this.btnCancel);
            this.panelControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControlBottom.Location = new System.Drawing.Point(0, 521);
            this.panelControlBottom.Name = "panelControlBottom";
            this.panelControlBottom.Size = new System.Drawing.Size(984, 40);
            this.panelControlBottom.TabIndex = 1;
            //
            // btnSelect
            //
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(889, 7);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(85, 26);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Chọn";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(798, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 26);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmPatientPackage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.panelControlBottom);
            this.Name = "frmPatientPackage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn dịch vụ trong gói";
            this.Load += new System.EventHandler(this.frmPatientPackage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchPackage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditDt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEditAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchDt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlBottom)).EndInit();
            this.panelControlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl gridControlPackage;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPackage;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgStt;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgName;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgRegisterDate;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgNote;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gColPkgModifier;
        private DevExpress.XtraEditors.TextEdit txtSearchPackage;
        private DevExpress.XtraEditors.LabelControl lblPackageList;
        private DevExpress.XtraGrid.GridControl gridControlDt;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDt;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditDt;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtServiceTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtAmountUsed;
        private DevExpress.XtraGrid.Columns.GridColumn gColDtAmountThisTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEditAmount;
        private DevExpress.XtraEditors.TextEdit txtSearchDt;
        private DevExpress.XtraEditors.LabelControl lblDtList;
        private DevExpress.XtraEditors.PanelControl panelControlBottom;
        private DevExpress.XtraEditors.SimpleButton btnSelect;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
