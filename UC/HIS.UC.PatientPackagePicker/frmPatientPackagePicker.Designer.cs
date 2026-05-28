/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
namespace HIS.UC.PatientPackagePicker
{
    partial class frmPatientPackagePicker
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
            this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
            this.grpPackage = new DevExpress.XtraEditors.GroupControl();
            this.gridControlPackage = new DevExpress.XtraGrid.GridControl();
            this.gridViewPackage = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colPackageName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRegisterDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtKeywordPackage = new DevExpress.XtraEditors.TextEdit();
            this.lblSearchPackage = new DevExpress.XtraEditors.LabelControl();
            this.grpDetail = new DevExpress.XtraEditors.GroupControl();
            this.gridControlDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colServiceCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colServiceTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmountUsed = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmountThisTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoAmountThisTime = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.txtKeywordDetail = new DevExpress.XtraEditors.TextEdit();
            this.lblSearchDetail = new DevExpress.XtraEditors.LabelControl();
            this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpPackage)).BeginInit();
            this.grpPackage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordPackage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpDetail)).BeginInit();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAmountThisTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordDetail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer
            //
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.Controls.Add(this.grpPackage);
            this.splitContainer.Panel1.Text = "Panel1";
            this.splitContainer.Panel2.Controls.Add(this.grpDetail);
            this.splitContainer.Panel2.Text = "Panel2";
            this.splitContainer.Size = new System.Drawing.Size(1370, 540);
            this.splitContainer.SplitterPosition = 670;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.Text = "splitContainer";
            //
            // grpPackage
            //
            this.grpPackage.Controls.Add(this.gridControlPackage);
            this.grpPackage.Controls.Add(this.txtKeywordPackage);
            this.grpPackage.Controls.Add(this.lblSearchPackage);
            this.grpPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPackage.Location = new System.Drawing.Point(0, 0);
            this.grpPackage.Name = "grpPackage";
            this.grpPackage.Size = new System.Drawing.Size(670, 540);
            this.grpPackage.TabIndex = 0;
            this.grpPackage.Text = "Danh sách gói dịch vụ";
            //
            // gridControlPackage
            //
            this.gridControlPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControlPackage.Location = new System.Drawing.Point(7, 58);
            this.gridControlPackage.MainView = this.gridViewPackage;
            this.gridControlPackage.Name = "gridControlPackage";
            this.gridControlPackage.Size = new System.Drawing.Size(656, 475);
            this.gridControlPackage.TabIndex = 2;
            this.gridControlPackage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPackage});
            //
            // gridViewPackage
            //
            this.gridViewPackage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colPackageName,
            this.colRegisterDate,
            this.colNote,
            this.colCreateTime,
            this.colCreator,
            this.colModifyTime,
            this.colModifier});
            this.gridViewPackage.GridControl = this.gridControlPackage;
            this.gridViewPackage.Name = "gridViewPackage";
            this.gridViewPackage.OptionsBehavior.Editable = false;
            this.gridViewPackage.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPackage.OptionsView.ShowGroupPanel = false;
            this.gridViewPackage.OptionsView.ShowIndicator = false;
            this.gridViewPackage.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewPackage_FocusedRowChanged);
            this.gridViewPackage.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewPackage_CustomColumnDisplayText);
            //
            // colPackageName
            //
            this.colPackageName.Caption = "Tên gói";
            this.colPackageName.FieldName = "PACKAGE_NAME";
            this.colPackageName.Name = "colPackageName";
            this.colPackageName.OptionsColumn.AllowEdit = false;
            this.colPackageName.OptionsColumn.ReadOnly = true;
            this.colPackageName.Visible = true;
            this.colPackageName.VisibleIndex = 0;
            this.colPackageName.Width = 180;
            //
            // colRegisterDate
            //
            this.colRegisterDate.Caption = "Ngày ĐK";
            this.colRegisterDate.FieldName = "REGISTER_DATE";
            this.colRegisterDate.Name = "colRegisterDate";
            this.colRegisterDate.OptionsColumn.AllowEdit = false;
            this.colRegisterDate.OptionsColumn.ReadOnly = true;
            this.colRegisterDate.Visible = true;
            this.colRegisterDate.VisibleIndex = 1;
            this.colRegisterDate.Width = 85;
            //
            // colNote
            //
            this.colNote.Caption = "Ghi chú";
            this.colNote.FieldName = "NOTE";
            this.colNote.Name = "colNote";
            this.colNote.OptionsColumn.AllowEdit = false;
            this.colNote.OptionsColumn.ReadOnly = true;
            this.colNote.Visible = true;
            this.colNote.VisibleIndex = 2;
            this.colNote.Width = 180;
            //
            // colCreateTime
            //
            this.colCreateTime.Caption = "Ngày tạo";
            this.colCreateTime.FieldName = "CREATE_TIME";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.OptionsColumn.AllowEdit = false;
            this.colCreateTime.OptionsColumn.ReadOnly = true;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 3;
            this.colCreateTime.Width = 85;
            //
            // colCreator
            //
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.OptionsColumn.AllowEdit = false;
            this.colCreator.OptionsColumn.ReadOnly = true;
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 4;
            this.colCreator.Width = 80;
            //
            // colModifyTime
            //
            this.colModifyTime.Caption = "Ngày sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.OptionsColumn.AllowEdit = false;
            this.colModifyTime.OptionsColumn.ReadOnly = true;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 5;
            this.colModifyTime.Width = 85;
            //
            // colModifier
            //
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.OptionsColumn.AllowEdit = false;
            this.colModifier.OptionsColumn.ReadOnly = true;
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 6;
            this.colModifier.Width = 80;
            //
            // txtKeywordPackage
            //
            this.txtKeywordPackage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeywordPackage.Location = new System.Drawing.Point(67, 30);
            this.txtKeywordPackage.Name = "txtKeywordPackage";
            this.txtKeywordPackage.Properties.EditValueChangedDelay = 400;
            this.txtKeywordPackage.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtKeywordPackage.Properties.NullValuePrompt = "Tên gói...";
            this.txtKeywordPackage.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeywordPackage.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeywordPackage.Size = new System.Drawing.Size(596, 22);
            this.txtKeywordPackage.TabIndex = 1;
            this.txtKeywordPackage.EditValueChanged += new System.EventHandler(this.txtKeywordPackage_EditValueChanged);
            //
            // lblSearchPackage
            //
            this.lblSearchPackage.Location = new System.Drawing.Point(7, 33);
            this.lblSearchPackage.Name = "lblSearchPackage";
            this.lblSearchPackage.Size = new System.Drawing.Size(54, 13);
            this.lblSearchPackage.TabIndex = 0;
            this.lblSearchPackage.Text = "Tìm kiếm:";
            //
            // grpDetail
            //
            this.grpDetail.Controls.Add(this.gridControlDetail);
            this.grpDetail.Controls.Add(this.txtKeywordDetail);
            this.grpDetail.Controls.Add(this.lblSearchDetail);
            this.grpDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDetail.Location = new System.Drawing.Point(0, 0);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(695, 540);
            this.grpDetail.TabIndex = 0;
            this.grpDetail.Text = "Dịch vụ trong gói";
            //
            // gridControlDetail
            //
            this.gridControlDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridControlDetail.Location = new System.Drawing.Point(7, 58);
            this.gridControlDetail.MainView = this.gridViewDetail;
            this.gridControlDetail.Name = "gridControlDetail";
            this.gridControlDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCheck,
            this.repoAmountThisTime});
            this.gridControlDetail.Size = new System.Drawing.Size(681, 475);
            this.gridControlDetail.TabIndex = 2;
            this.gridControlDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDetail});
            //
            // gridViewDetail
            //
            this.gridViewDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCheck,
            this.colServiceCode,
            this.colServiceName,
            this.colServiceTypeName,
            this.colAmount,
            this.colAmountUsed,
            this.colAmountThisTime});
            this.gridViewDetail.GridControl = this.gridControlDetail;
            this.gridViewDetail.Name = "gridViewDetail";
            this.gridViewDetail.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewDetail.OptionsView.ShowIndicator = false;
            this.gridViewDetail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDetail_CellValueChanged);
            //
            // colCheck
            //
            this.colCheck.Caption = " ";
            this.colCheck.ColumnEdit = this.repoCheck;
            this.colCheck.FieldName = "IS_CHECKED";
            this.colCheck.Name = "colCheck";
            this.colCheck.Visible = true;
            this.colCheck.VisibleIndex = 0;
            this.colCheck.Width = 30;
            //
            // repoCheck
            //
            this.repoCheck.AutoHeight = false;
            this.repoCheck.Name = "repoCheck";
            //
            // colServiceCode
            //
            this.colServiceCode.Caption = "Mã DV";
            this.colServiceCode.FieldName = "SERVICE_CODE";
            this.colServiceCode.Name = "colServiceCode";
            this.colServiceCode.OptionsColumn.AllowEdit = false;
            this.colServiceCode.OptionsColumn.ReadOnly = true;
            this.colServiceCode.Visible = true;
            this.colServiceCode.VisibleIndex = 1;
            this.colServiceCode.Width = 80;
            //
            // colServiceName
            //
            this.colServiceName.Caption = "Tên dịch vụ";
            this.colServiceName.FieldName = "SERVICE_NAME";
            this.colServiceName.Name = "colServiceName";
            this.colServiceName.OptionsColumn.AllowEdit = false;
            this.colServiceName.OptionsColumn.ReadOnly = true;
            this.colServiceName.Visible = true;
            this.colServiceName.VisibleIndex = 2;
            this.colServiceName.Width = 280;
            //
            // colServiceTypeName
            //
            this.colServiceTypeName.Caption = "Loại DV";
            this.colServiceTypeName.FieldName = "SERVICE_TYPE_NAME";
            this.colServiceTypeName.Name = "colServiceTypeName";
            this.colServiceTypeName.OptionsColumn.AllowEdit = false;
            this.colServiceTypeName.OptionsColumn.ReadOnly = true;
            this.colServiceTypeName.Visible = true;
            this.colServiceTypeName.VisibleIndex = 3;
            this.colServiceTypeName.Width = 90;
            //
            // colAmount
            //
            this.colAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmount.AppearanceCell.Options.UseTextOptions = true;
            this.colAmount.Caption = "Trong gói";
            this.colAmount.DisplayFormat.FormatString = "{0:n0}";
            this.colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colAmount.FieldName = "AMOUNT";
            this.colAmount.Name = "colAmount";
            this.colAmount.OptionsColumn.AllowEdit = false;
            this.colAmount.OptionsColumn.ReadOnly = true;
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 4;
            this.colAmount.Width = 70;
            //
            // colAmountUsed
            //
            this.colAmountUsed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmountUsed.AppearanceCell.Options.UseTextOptions = true;
            this.colAmountUsed.Caption = "Đã dùng";
            this.colAmountUsed.DisplayFormat.FormatString = "{0:n0}";
            this.colAmountUsed.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colAmountUsed.FieldName = "AMOUNT_USED";
            this.colAmountUsed.Name = "colAmountUsed";
            this.colAmountUsed.OptionsColumn.AllowEdit = false;
            this.colAmountUsed.OptionsColumn.ReadOnly = true;
            this.colAmountUsed.Visible = true;
            this.colAmountUsed.VisibleIndex = 5;
            this.colAmountUsed.Width = 70;
            //
            // colAmountThisTime
            //
            this.colAmountThisTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colAmountThisTime.AppearanceCell.Options.UseTextOptions = true;
            this.colAmountThisTime.Caption = "Lần này";
            this.colAmountThisTime.ColumnEdit = this.repoAmountThisTime;
            this.colAmountThisTime.FieldName = "AMOUNT_THIS_TIME";
            this.colAmountThisTime.Name = "colAmountThisTime";
            this.colAmountThisTime.Visible = true;
            this.colAmountThisTime.VisibleIndex = 6;
            this.colAmountThisTime.Width = 70;
            //
            // repoAmountThisTime
            //
            this.repoAmountThisTime.AutoHeight = false;
            this.repoAmountThisTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Up),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.repoAmountThisTime.DisplayFormat.FormatString = "{0:n0}";
            this.repoAmountThisTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoAmountThisTime.EditFormat.FormatString = "{0:n0}";
            this.repoAmountThisTime.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.repoAmountThisTime.MaxValue = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.repoAmountThisTime.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.repoAmountThisTime.Name = "repoAmountThisTime";
            //
            // txtKeywordDetail
            //
            this.txtKeywordDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeywordDetail.Location = new System.Drawing.Point(67, 30);
            this.txtKeywordDetail.Name = "txtKeywordDetail";
            this.txtKeywordDetail.Properties.EditValueChangedDelay = 400;
            this.txtKeywordDetail.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
            this.txtKeywordDetail.Properties.NullValuePrompt = "Mã / tên dịch vụ...";
            this.txtKeywordDetail.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeywordDetail.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeywordDetail.Size = new System.Drawing.Size(621, 22);
            this.txtKeywordDetail.TabIndex = 1;
            this.txtKeywordDetail.EditValueChanged += new System.EventHandler(this.txtKeywordDetail_EditValueChanged);
            //
            // lblSearchDetail
            //
            this.lblSearchDetail.Location = new System.Drawing.Point(7, 33);
            this.lblSearchDetail.Name = "lblSearchDetail";
            this.lblSearchDetail.Size = new System.Drawing.Size(54, 13);
            this.lblSearchDetail.TabIndex = 0;
            this.lblSearchDetail.Text = "Tìm kiếm:";
            //
            // pnlBottom
            //
            this.pnlBottom.Controls.Add(this.btnChoose);
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 540);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1370, 40);
            this.pnlBottom.TabIndex = 1;
            //
            // btnChoose
            //
            this.btnChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChoose.Location = new System.Drawing.Point(1278, 8);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(80, 24);
            this.btnChoose.TabIndex = 2;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(1192, 8);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmPatientPackagePicker
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 580);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.pnlBottom);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(900, 450);
            this.Name = "frmPatientPackagePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn dịch vụ trong gói";
            this.Load += new System.EventHandler(this.frmPatientPackagePicker_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPatientPackagePicker_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordPackage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPackage)).EndInit();
            this.grpPackage.ResumeLayout(false);
            this.grpPackage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeywordDetail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoAmountThisTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).EndInit();
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpDetail)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainer;
        private DevExpress.XtraEditors.GroupControl grpPackage;
        private DevExpress.XtraEditors.GroupControl grpDetail;
        private DevExpress.XtraGrid.GridControl gridControlPackage;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPackage;
        private DevExpress.XtraGrid.Columns.GridColumn colPackageName;
        private DevExpress.XtraGrid.Columns.GridColumn colRegisterDate;
        private DevExpress.XtraGrid.Columns.GridColumn colNote;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;
        private DevExpress.XtraEditors.TextEdit txtKeywordPackage;
        private DevExpress.XtraEditors.LabelControl lblSearchPackage;
        private DevExpress.XtraGrid.GridControl gridControlDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDetail;
        private DevExpress.XtraGrid.Columns.GridColumn colCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheck;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceCode;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn colServiceTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colAmountUsed;
        private DevExpress.XtraGrid.Columns.GridColumn colAmountThisTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repoAmountThisTime;
        private DevExpress.XtraEditors.TextEdit txtKeywordDetail;
        private DevExpress.XtraEditors.LabelControl lblSearchDetail;
        private DevExpress.XtraEditors.PanelControl pnlBottom;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
