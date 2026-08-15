/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
namespace HIS.Desktop.Plugins.TreatmentInspectionV2
{
    partial class frmImportRecordInspection
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
            this.btnDownloadTemplate = new DevExpress.XtraEditors.SimpleButton();
            this.btnChooseFile = new DevExpress.XtraEditors.SimpleButton();
            this.btnShowLineError = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.lblSummary = new DevExpress.XtraEditors.LabelControl();
            this.gridControlData = new DevExpress.XtraGrid.GridControl();
            this.gridViewData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcError = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcInTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcOutTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcEndDepartment = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIcd = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnGDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnGError = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnGWarning = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnDownloadTemplate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnChooseFile = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnShowLineError = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnSave = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItemTop = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSummary = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDownloadTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnChooseFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnShowLineError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnDownloadTemplate);
            this.layoutControl1.Controls.Add(this.btnChooseFile);
            this.layoutControl1.Controls.Add(this.btnShowLineError);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.lblSummary);
            this.layoutControl1.Controls.Add(this.gridControlData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1004, 561);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnDownloadTemplate
            //
            this.btnDownloadTemplate.Location = new System.Drawing.Point(2, 2);
            this.btnDownloadTemplate.Name = "btnDownloadTemplate";
            this.btnDownloadTemplate.Size = new System.Drawing.Size(126, 22);
            this.btnDownloadTemplate.StyleController = this.layoutControl1;
            this.btnDownloadTemplate.TabIndex = 0;
            this.btnDownloadTemplate.Text = "Tải file mẫu";
            this.btnDownloadTemplate.Click += new System.EventHandler(this.btnDownloadTemplate_Click);
            //
            // btnChooseFile
            //
            this.btnChooseFile.Location = new System.Drawing.Point(132, 2);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(146, 22);
            this.btnChooseFile.StyleController = this.layoutControl1;
            this.btnChooseFile.TabIndex = 1;
            this.btnChooseFile.Text = "Chọn file Excel";
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            //
            // btnShowLineError
            //
            this.btnShowLineError.Enabled = false;
            this.btnShowLineError.Location = new System.Drawing.Point(282, 2);
            this.btnShowLineError.Name = "btnShowLineError";
            this.btnShowLineError.Size = new System.Drawing.Size(146, 22);
            this.btnShowLineError.StyleController = this.layoutControl1;
            this.btnShowLineError.TabIndex = 2;
            this.btnShowLineError.Text = "Chỉ dòng lỗi";
            this.btnShowLineError.Click += new System.EventHandler(this.btnShowLineError_Click);
            //
            // btnSave
            //
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(432, 2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(126, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // lblSummary
            //
            this.lblSummary.Location = new System.Drawing.Point(2, 539);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(1000, 20);
            this.lblSummary.StyleController = this.layoutControl1;
            this.lblSummary.TabIndex = 5;
            this.lblSummary.Text = "";
            //
            // gridControlData
            //
            this.gridControlData.Location = new System.Drawing.Point(2, 28);
            this.gridControlData.MainView = this.gridViewData;
            this.gridControlData.Name = "gridControlData";
            this.gridControlData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnGDelete,
            this.btnGError,
            this.btnGWarning});
            this.gridControlData.Size = new System.Drawing.Size(1000, 507);
            this.gridControlData.TabIndex = 4;
            this.gridControlData.ToolTipController = this.toolTipController1;
            this.gridControlData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewData});
            //
            // gridViewData
            //
            this.gridViewData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcError,
            this.gcDelete,
            this.gcTreatmentCode,
            this.gcPatientCode,
            this.gcPatientName,
            this.gcInTime,
            this.gcOutTime,
            this.gcEndDepartment,
            this.gcIcd,
            this.gcNote,
            this.gcStatus});
            this.gridViewData.GridControl = this.gridControlData;
            this.gridViewData.Name = "gridViewData";
            this.gridViewData.OptionsFind.AllowFindPanel = false;
            this.gridViewData.OptionsView.ColumnAutoWidth = false;
            this.gridViewData.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewData.OptionsView.ShowDetailButtons = false;
            this.gridViewData.OptionsView.ShowGroupPanel = false;
            this.gridViewData.OptionsView.ShowIndicator = false;
            this.gridViewData.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewData_CustomUnboundColumnData);
            this.gridViewData.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewData_CustomRowCellEdit);
            this.gridViewData.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewData_RowStyle);
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 45;
            //
            // gcError
            //
            this.gcError.Caption = " ";
            this.gcError.FieldName = "ERROR_VIEW";
            this.gcError.Name = "gcError";
            this.gcError.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcError.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcError.Visible = true;
            this.gcError.VisibleIndex = 1;
            this.gcError.Width = 26;
            //
            // gcDelete
            //
            this.gcDelete.Caption = " ";
            this.gcDelete.FieldName = "DELETE_VIEW";
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcDelete.ToolTip = "Bỏ dòng này khỏi danh sách";
            this.gcDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 2;
            this.gcDelete.Width = 30;
            //
            // gcTreatmentCode
            //
            this.gcTreatmentCode.Caption = "Mã điều trị";
            this.gcTreatmentCode.FieldName = "TREATMENT_CODE";
            this.gcTreatmentCode.Name = "gcTreatmentCode";
            this.gcTreatmentCode.OptionsColumn.AllowEdit = false;
            this.gcTreatmentCode.Visible = true;
            this.gcTreatmentCode.VisibleIndex = 3;
            this.gcTreatmentCode.Width = 120;
            //
            // gcPatientCode
            //
            this.gcPatientCode.Caption = "Mã bệnh nhân";
            this.gcPatientCode.FieldName = "PATIENT_CODE";
            this.gcPatientCode.Name = "gcPatientCode";
            this.gcPatientCode.OptionsColumn.AllowEdit = false;
            this.gcPatientCode.Visible = true;
            this.gcPatientCode.VisibleIndex = 4;
            this.gcPatientCode.Width = 110;
            //
            // gcPatientName
            //
            this.gcPatientName.Caption = "Tên bệnh nhân";
            this.gcPatientName.FieldName = "PATIENT_NAME";
            this.gcPatientName.Name = "gcPatientName";
            this.gcPatientName.OptionsColumn.AllowEdit = false;
            this.gcPatientName.Visible = true;
            this.gcPatientName.VisibleIndex = 5;
            this.gcPatientName.Width = 180;
            //
            // gcInTime
            //
            this.gcInTime.Caption = "Thời gian vào viện";
            this.gcInTime.FieldName = "IN_TIME_STR";
            this.gcInTime.Name = "gcInTime";
            this.gcInTime.OptionsColumn.AllowEdit = false;
            this.gcInTime.Visible = true;
            this.gcInTime.VisibleIndex = 6;
            this.gcInTime.Width = 120;
            //
            // gcOutTime
            //
            this.gcOutTime.Caption = "Thời gian ra viện";
            this.gcOutTime.FieldName = "OUT_TIME_STR";
            this.gcOutTime.Name = "gcOutTime";
            this.gcOutTime.OptionsColumn.AllowEdit = false;
            this.gcOutTime.Visible = true;
            this.gcOutTime.VisibleIndex = 7;
            this.gcOutTime.Width = 120;
            //
            // gcEndDepartment
            //
            this.gcEndDepartment.Caption = "Khoa kết thúc";
            this.gcEndDepartment.FieldName = "END_DEPARTMENT_NAME";
            this.gcEndDepartment.Name = "gcEndDepartment";
            this.gcEndDepartment.OptionsColumn.AllowEdit = false;
            this.gcEndDepartment.Visible = true;
            this.gcEndDepartment.VisibleIndex = 8;
            this.gcEndDepartment.Width = 160;
            //
            // gcIcd
            //
            this.gcIcd.Caption = "Chẩn đoán";
            this.gcIcd.FieldName = "ICD";
            this.gcIcd.Name = "gcIcd";
            this.gcIcd.OptionsColumn.AllowEdit = false;
            this.gcIcd.Visible = true;
            this.gcIcd.VisibleIndex = 9;
            this.gcIcd.Width = 200;
            //
            // gcNote
            //
            this.gcNote.Caption = "Ghi chú";
            this.gcNote.FieldName = "NOTE";
            this.gcNote.Name = "gcNote";
            this.gcNote.OptionsColumn.AllowEdit = false;
            this.gcNote.ToolTip = "Ghi chú trong file nhập khẩu — chỉ hiển thị, không lưu";
            this.gcNote.Visible = true;
            this.gcNote.VisibleIndex = 10;
            this.gcNote.Width = 200;
            //
            // gcStatus
            //
            this.gcStatus.Caption = "Trạng thái";
            this.gcStatus.FieldName = "STATUS";
            this.gcStatus.Name = "gcStatus";
            this.gcStatus.OptionsColumn.AllowEdit = false;
            this.gcStatus.Visible = true;
            this.gcStatus.VisibleIndex = 11;
            this.gcStatus.Width = 100;
            //
            // btnGError
            //
            // Same shape as the error button of the other import screens: a glyph-only button whose
            // image is assigned at run time from the embedded icon.
            this.btnGError.AutoHeight = false;
            this.btnGError.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.btnGError.Name = "btnGError";
            this.btnGError.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGError.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGError_ButtonClick);
            //
            // btnGWarning
            //
            // Same shape as the error button; it marks rows that stay saveable.
            this.btnGWarning.AutoHeight = false;
            this.btnGWarning.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)});
            this.btnGWarning.Name = "btnGWarning";
            this.btnGWarning.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGWarning.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGWarning_ButtonClick);
            //
            // toolTipController1
            //
            this.toolTipController1.AllowHtmlText = true;
            this.toolTipController1.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            //
            // btnGDelete
            //
            this.btnGDelete.AutoHeight = false;
            this.btnGDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.btnGDelete.Name = "btnGDelete";
            this.btnGDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnGDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGDelete_ButtonClick);
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBtnDownloadTemplate,
            this.lciBtnChooseFile,
            this.lciBtnShowLineError,
            this.lciBtnSave,
            this.emptySpaceItemTop,
            this.lciGrid,
            this.lciSummary});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1004, 561);
            this.Root.TextVisible = false;
            //
            // lciBtnDownloadTemplate
            //
            this.lciBtnDownloadTemplate.Control = this.btnDownloadTemplate;
            this.lciBtnDownloadTemplate.Location = new System.Drawing.Point(0, 0);
            this.lciBtnDownloadTemplate.Name = "lciBtnDownloadTemplate";
            this.lciBtnDownloadTemplate.Size = new System.Drawing.Size(130, 26);
            this.lciBtnDownloadTemplate.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDownloadTemplate.TextVisible = false;
            //
            // lciBtnChooseFile
            //
            this.lciBtnChooseFile.Control = this.btnChooseFile;
            this.lciBtnChooseFile.Location = new System.Drawing.Point(130, 0);
            this.lciBtnChooseFile.Name = "lciBtnChooseFile";
            this.lciBtnChooseFile.Size = new System.Drawing.Size(150, 26);
            this.lciBtnChooseFile.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnChooseFile.TextVisible = false;
            //
            // lciBtnShowLineError
            //
            this.lciBtnShowLineError.Control = this.btnShowLineError;
            this.lciBtnShowLineError.Location = new System.Drawing.Point(280, 0);
            this.lciBtnShowLineError.Name = "lciBtnShowLineError";
            this.lciBtnShowLineError.Size = new System.Drawing.Size(150, 26);
            this.lciBtnShowLineError.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnShowLineError.TextVisible = false;
            //
            // lciBtnSave
            //
            this.lciBtnSave.Control = this.btnSave;
            this.lciBtnSave.Location = new System.Drawing.Point(430, 0);
            this.lciBtnSave.Name = "lciBtnSave";
            this.lciBtnSave.Size = new System.Drawing.Size(130, 26);
            this.lciBtnSave.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnSave.TextVisible = false;
            //
            // emptySpaceItemTop
            //
            this.emptySpaceItemTop.AllowHotTrack = false;
            this.emptySpaceItemTop.Location = new System.Drawing.Point(560, 0);
            this.emptySpaceItemTop.Name = "emptySpaceItemTop";
            this.emptySpaceItemTop.Size = new System.Drawing.Size(444, 26);
            this.emptySpaceItemTop.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciGrid
            //
            this.lciGrid.Control = this.gridControlData;
            this.lciGrid.Location = new System.Drawing.Point(0, 26);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(1004, 511);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciSummary
            //
            this.lciSummary.Control = this.lblSummary;
            this.lciSummary.Location = new System.Drawing.Point(0, 537);
            this.lciSummary.Name = "lciSummary";
            this.lciSummary.Size = new System.Drawing.Size(1004, 24);
            this.lciSummary.TextSize = new System.Drawing.Size(0, 0);
            this.lciSummary.TextVisible = false;
            //
            // frmImportRecordInspection
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 561);
            this.Controls.Add(this.layoutControl1);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "frmImportRecordInspection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhập khẩu danh sách hồ sơ giám định";
            this.Load += new System.EventHandler(this.frmImportRecordInspection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDownloadTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnChooseFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnShowLineError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSummary)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SimpleButton btnDownloadTemplate;
        private DevExpress.XtraEditors.SimpleButton btnChooseFile;
        private DevExpress.XtraEditors.SimpleButton btnShowLineError;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.LabelControl lblSummary;
        private DevExpress.XtraGrid.GridControl gridControlData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewData;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcError;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gcInTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcOutTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcEndDepartment;
        private DevExpress.XtraGrid.Columns.GridColumn gcIcd;
        private DevExpress.XtraGrid.Columns.GridColumn gcNote;
        private DevExpress.XtraGrid.Columns.GridColumn gcStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGError;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnGWarning;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDownloadTemplate;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnChooseFile;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnShowLineError;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnSave;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemTop;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciSummary;
    }
}
