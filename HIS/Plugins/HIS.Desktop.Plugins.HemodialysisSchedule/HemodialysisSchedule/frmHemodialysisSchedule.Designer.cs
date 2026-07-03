/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule
{
    partial class frmHemodialysisSchedule
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
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridControlSchedule = new DevExpress.XtraGrid.GridControl();
            this.gridViewSchedule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colScheduleDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colShift = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colInTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTemplate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoTemplate = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.repoTemplateView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colNote = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelTop = new DevExpress.XtraEditors.PanelControl();
            this.lblRoom = new DevExpress.XtraEditors.LabelControl();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.dtDate = new DevExpress.XtraEditors.DateEdit();
            this.lblShift = new DevExpress.XtraEditors.LabelControl();
            this.cboShift = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewShift = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtSearchTop = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchTop = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnCopy = new DevExpress.XtraEditors.SimpleButton();
            this.lblCopyFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtCopyFromDate = new DevExpress.XtraEditors.DateEdit();
            this.gridControlTreatment = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSelect = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colSTTb = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientNameB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCodeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentCodeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDobB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGenderB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colInTimeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTreatmentTypeB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHeinCardB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIcdNameB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.lblDept = new DevExpress.XtraEditors.LabelControl();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridViewDept = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkAllDepartment = new DevExpress.XtraEditors.CheckEdit();
            this.lblFrom = new DevExpress.XtraEditors.LabelControl();
            this.dtInTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblTo = new DevExpress.XtraEditors.LabelControl();
            this.dtInTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.txtSearchBottom = new DevExpress.XtraEditors.TextEdit();
            this.btnSearchBottom = new DevExpress.XtraEditors.SimpleButton();
            this.chkSelectAll = new DevExpress.XtraEditors.CheckEdit();
            this.btnAddToSchedule = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplateView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShift.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchTop.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDept)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchBottom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelectAll.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.gridControlSchedule);
            this.splitContainerControl1.Panel1.Controls.Add(this.panelTop);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.gridControlTreatment);
            this.splitContainerControl1.Panel2.Controls.Add(this.panelBottom);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1250, 700);
            this.splitContainerControl1.SplitterPosition = 380;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // gridControlSchedule
            // 
            this.gridControlSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlSchedule.Location = new System.Drawing.Point(0, 36);
            this.gridControlSchedule.MainView = this.gridViewSchedule;
            this.gridControlSchedule.Name = "gridControlSchedule";
            this.gridControlSchedule.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoDelete,
            this.repoTemplate});
            this.gridControlSchedule.Size = new System.Drawing.Size(1250, 344);
            this.gridControlSchedule.TabIndex = 1;
            this.gridControlSchedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSchedule,
            this.repoTemplateView});
            // 
            // gridViewSchedule
            // 
            this.gridViewSchedule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDelete,
            this.colSTT,
            this.colScheduleDate,
            this.colShift,
            this.colPatientName,
            this.colPatientCode,
            this.colTreatmentCode,
            this.colDob,
            this.colGender,
            this.colInTime,
            this.colPatientType,
            this.colTemplate,
            this.colNote,
            this.colCreateTime,
            this.colCreator,
            this.colModifyTime,
            this.colModifier});
            this.gridViewSchedule.GridControl = this.gridControlSchedule;
            this.gridViewSchedule.Name = "gridViewSchedule";
            this.gridViewSchedule.OptionsView.ColumnAutoWidth = false;
            this.gridViewSchedule.OptionsView.ShowGroupPanel = false;
            this.gridViewSchedule.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewSchedule_CellValueChanged);
            this.gridViewSchedule.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSchedule_CustomUnboundColumnData);
            this.gridViewSchedule.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewSchedule_CustomColumnDisplayText);
            // 
            // colDelete
            // 
            this.colDelete.Caption = " ";
            this.colDelete.ColumnEdit = this.repoDelete;
            this.colDelete.FieldName = "DELETE";
            this.colDelete.Name = "colDelete";
            this.colDelete.OptionsColumn.ShowCaption = false;
            this.colDelete.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 0;
            this.colDelete.Width = 30;
            // 
            // repoDelete
            // 
            this.repoDelete.AutoHeight = false;
            this.repoDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repoDelete.Name = "repoDelete";
            this.repoDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoDelete_ButtonClick);
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.Name = "colSTT";
            this.colSTT.OptionsColumn.AllowEdit = false;
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 1;
            this.colSTT.Width = 40;
            // 
            // colScheduleDate
            // 
            this.colScheduleDate.Caption = "Ngày";
            this.colScheduleDate.FieldName = "SCHEDULE_DATE";
            this.colScheduleDate.Name = "colScheduleDate";
            this.colScheduleDate.OptionsColumn.AllowEdit = false;
            this.colScheduleDate.Visible = true;
            this.colScheduleDate.VisibleIndex = 2;
            this.colScheduleDate.Width = 80;
            // 
            // colShift
            // 
            this.colShift.Caption = "Ca";
            this.colShift.FieldName = "KIDNEY_SHIFT";
            this.colShift.Name = "colShift";
            this.colShift.OptionsColumn.AllowEdit = false;
            this.colShift.Visible = true;
            this.colShift.VisibleIndex = 3;
            this.colShift.Width = 50;
            // 
            // colPatientName
            // 
            this.colPatientName.Caption = "Tên bệnh nhân";
            this.colPatientName.FieldName = "TDL_PATIENT_NAME";
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.OptionsColumn.AllowEdit = false;
            this.colPatientName.Visible = true;
            this.colPatientName.VisibleIndex = 4;
            this.colPatientName.Width = 160;
            // 
            // colPatientCode
            // 
            this.colPatientCode.Caption = "Mã bệnh nhân";
            this.colPatientCode.FieldName = "TDL_PATIENT_CODE";
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 5;
            this.colPatientCode.Width = 90;
            // 
            // colTreatmentCode
            // 
            this.colTreatmentCode.Caption = "Mã điều trị";
            this.colTreatmentCode.FieldName = "TREATMENT_CODE";
            this.colTreatmentCode.Name = "colTreatmentCode";
            this.colTreatmentCode.OptionsColumn.AllowEdit = false;
            this.colTreatmentCode.Visible = true;
            this.colTreatmentCode.VisibleIndex = 6;
            this.colTreatmentCode.Width = 100;
            // 
            // colDob
            // 
            this.colDob.Caption = "Ngày sinh";
            this.colDob.FieldName = "TDL_PATIENT_DOB";
            this.colDob.Name = "colDob";
            this.colDob.OptionsColumn.AllowEdit = false;
            this.colDob.Visible = true;
            this.colDob.VisibleIndex = 7;
            this.colDob.Width = 70;
            // 
            // colGender
            // 
            this.colGender.Caption = "Giới tính";
            this.colGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.colGender.Name = "colGender";
            this.colGender.OptionsColumn.AllowEdit = false;
            this.colGender.Visible = true;
            this.colGender.VisibleIndex = 8;
            this.colGender.Width = 60;
            // 
            // colInTime
            // 
            this.colInTime.Caption = "Ngày vào";
            this.colInTime.FieldName = "IN_TIME";
            this.colInTime.Name = "colInTime";
            this.colInTime.OptionsColumn.AllowEdit = false;
            this.colInTime.Visible = true;
            this.colInTime.VisibleIndex = 9;
            this.colInTime.Width = 80;
            // 
            // colPatientType
            // 
            this.colPatientType.Caption = "Đối tượng";
            this.colPatientType.FieldName = "TDL_PATIENT_TYPE_NAME";
            this.colPatientType.Name = "colPatientType";
            this.colPatientType.OptionsColumn.AllowEdit = false;
            this.colPatientType.Visible = true;
            this.colPatientType.VisibleIndex = 10;
            this.colPatientType.Width = 90;
            // 
            // colTemplate
            // 
            this.colTemplate.Caption = "Gói vật tư";
            this.colTemplate.ColumnEdit = this.repoTemplate;
            this.colTemplate.FieldName = "EXP_MEST_TEMPLATE_ID";
            this.colTemplate.Name = "colTemplate";
            this.colTemplate.Visible = true;
            this.colTemplate.VisibleIndex = 11;
            this.colTemplate.Width = 150;
            // 
            // repoTemplate
            // 
            this.repoTemplate.AutoHeight = false;
            this.repoTemplate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoTemplate.Name = "repoTemplate";
            this.repoTemplate.NullText = "";
            this.repoTemplate.View = this.repoTemplateView;
            // 
            // repoTemplateView
            // 
            this.repoTemplateView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repoTemplateView.GridControl = this.gridControlSchedule;
            this.repoTemplateView.Name = "repoTemplateView";
            this.repoTemplateView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repoTemplateView.OptionsView.ShowGroupPanel = false;
            // 
            // colNote
            // 
            this.colNote.Caption = "Ghi chú";
            this.colNote.FieldName = "NOTE";
            this.colNote.Name = "colNote";
            this.colNote.Visible = true;
            this.colNote.VisibleIndex = 12;
            this.colNote.Width = 160;
            // 
            // colCreateTime
            // 
            this.colCreateTime.Caption = "Ngày tạo";
            this.colCreateTime.FieldName = "CREATE_TIME";
            this.colCreateTime.Name = "colCreateTime";
            this.colCreateTime.OptionsColumn.AllowEdit = false;
            this.colCreateTime.Visible = true;
            this.colCreateTime.VisibleIndex = 13;
            this.colCreateTime.Width = 110;
            // 
            // colCreator
            // 
            this.colCreator.Caption = "Người tạo";
            this.colCreator.FieldName = "CREATOR";
            this.colCreator.Name = "colCreator";
            this.colCreator.OptionsColumn.AllowEdit = false;
            this.colCreator.Visible = true;
            this.colCreator.VisibleIndex = 14;
            this.colCreator.Width = 90;
            // 
            // colModifyTime
            // 
            this.colModifyTime.Caption = "Ngày sửa";
            this.colModifyTime.FieldName = "MODIFY_TIME";
            this.colModifyTime.Name = "colModifyTime";
            this.colModifyTime.OptionsColumn.AllowEdit = false;
            this.colModifyTime.Visible = true;
            this.colModifyTime.VisibleIndex = 15;
            this.colModifyTime.Width = 110;
            // 
            // colModifier
            // 
            this.colModifier.Caption = "Người sửa";
            this.colModifier.FieldName = "MODIFIER";
            this.colModifier.Name = "colModifier";
            this.colModifier.OptionsColumn.AllowEdit = false;
            this.colModifier.Visible = true;
            this.colModifier.VisibleIndex = 16;
            this.colModifier.Width = 90;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.lblRoom);
            this.panelTop.Controls.Add(this.cboRoom);
            this.panelTop.Controls.Add(this.lblDate);
            this.panelTop.Controls.Add(this.dtDate);
            this.panelTop.Controls.Add(this.lblShift);
            this.panelTop.Controls.Add(this.cboShift);
            this.panelTop.Controls.Add(this.txtSearchTop);
            this.panelTop.Controls.Add(this.btnSearchTop);
            this.panelTop.Controls.Add(this.btnPrint);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.btnCopy);
            this.panelTop.Controls.Add(this.lblCopyFrom);
            this.panelTop.Controls.Add(this.dtCopyFromDate);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1250, 36);
            this.panelTop.TabIndex = 0;
            // 
            // lblRoom
            // 
            this.lblRoom.Location = new System.Drawing.Point(6, 11);
            this.lblRoom.Name = "lblRoom";
            this.lblRoom.Size = new System.Drawing.Size(60, 13);
            this.lblRoom.TabIndex = 0;
            this.lblRoom.Text = "Phòng chạy:";
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(70, 8);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridViewRoom;
            this.cboRoom.Size = new System.Drawing.Size(160, 20);
            this.cboRoom.TabIndex = 1;
            // 
            // gridViewRoom
            // 
            this.gridViewRoom.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewRoom.Name = "gridViewRoom";
            this.gridViewRoom.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewRoom.OptionsView.ShowGroupPanel = false;
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(236, 11);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(29, 13);
            this.lblDate.TabIndex = 2;
            this.lblDate.Text = "Ngày:";
            // 
            // dtDate
            // 
            this.dtDate.EditValue = null;
            this.dtDate.Location = new System.Drawing.Point(275, 8);
            this.dtDate.Name = "dtDate";
            this.dtDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtDate.Size = new System.Drawing.Size(95, 20);
            this.dtDate.TabIndex = 3;
            // 
            // lblShift
            // 
            this.lblShift.Location = new System.Drawing.Point(378, 11);
            this.lblShift.Name = "lblShift";
            this.lblShift.Size = new System.Drawing.Size(17, 13);
            this.lblShift.TabIndex = 4;
            this.lblShift.Text = "Ca:";
            // 
            // cboShift
            // 
            this.cboShift.Location = new System.Drawing.Point(400, 8);
            this.cboShift.Name = "cboShift";
            this.cboShift.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboShift.Properties.View = this.gridViewShift;
            this.cboShift.Size = new System.Drawing.Size(70, 20);
            this.cboShift.TabIndex = 5;
            // 
            // gridViewShift
            // 
            this.gridViewShift.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewShift.Name = "gridViewShift";
            this.gridViewShift.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewShift.OptionsView.ShowGroupPanel = false;
            // 
            // txtSearchTop
            // 
            this.txtSearchTop.Location = new System.Drawing.Point(478, 8);
            this.txtSearchTop.Name = "txtSearchTop";
            this.txtSearchTop.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchTop.Size = new System.Drawing.Size(160, 20);
            this.txtSearchTop.TabIndex = 6;
            this.txtSearchTop.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearchTop_KeyUp);
            // 
            // btnSearchTop
            // 
            this.btnSearchTop.Location = new System.Drawing.Point(644, 7);
            this.btnSearchTop.Name = "btnSearchTop";
            this.btnSearchTop.Size = new System.Drawing.Size(85, 22);
            this.btnSearchTop.TabIndex = 7;
            this.btnSearchTop.Text = "Tìm (Ctrl F)";
            this.btnSearchTop.Click += new System.EventHandler(this.btnSearchTop_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(733, 7);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(80, 22);
            this.btnPrint.TabIndex = 8;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(817, 7);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 22);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(906, 7);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(90, 22);
            this.btnCopy.TabIndex = 10;
            this.btnCopy.Text = "Sao chép";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblCopyFrom
            // 
            this.lblCopyFrom.Location = new System.Drawing.Point(1004, 11);
            this.lblCopyFrom.Name = "lblCopyFrom";
            this.lblCopyFrom.Size = new System.Drawing.Size(89, 13);
            this.lblCopyFrom.TabIndex = 11;
            this.lblCopyFrom.Text = "Sao chép từ ngày:";
            // 
            // dtCopyFromDate
            // 
            this.dtCopyFromDate.EditValue = null;
            this.dtCopyFromDate.Location = new System.Drawing.Point(1107, 8);
            this.dtCopyFromDate.Name = "dtCopyFromDate";
            this.dtCopyFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCopyFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCopyFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtCopyFromDate.Size = new System.Drawing.Size(95, 20);
            this.dtCopyFromDate.TabIndex = 12;
            // 
            // gridControlTreatment
            // 
            this.gridControlTreatment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlTreatment.Location = new System.Drawing.Point(0, 36);
            this.gridControlTreatment.MainView = this.gridViewTreatment;
            this.gridControlTreatment.Name = "gridControlTreatment";
            this.gridControlTreatment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCheck});
            this.gridControlTreatment.Size = new System.Drawing.Size(1250, 279);
            this.gridControlTreatment.TabIndex = 1;
            this.gridControlTreatment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatment});
            // 
            // gridViewTreatment
            // 
            this.gridViewTreatment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSelect,
            this.colSTTb,
            this.colPatientNameB,
            this.colPatientCodeB,
            this.colTreatmentCodeB,
            this.colDobB,
            this.colGenderB,
            this.colInTimeB,
            this.colTreatmentTypeB,
            this.colHeinCardB,
            this.colIcdNameB});
            this.gridViewTreatment.GridControl = this.gridControlTreatment;
            this.gridViewTreatment.Name = "gridViewTreatment";
            this.gridViewTreatment.OptionsView.ColumnAutoWidth = false;
            this.gridViewTreatment.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatment_CustomUnboundColumnData);
            this.gridViewTreatment.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewTreatment_CustomColumnDisplayText);
            // 
            // colSelect
            // 
            this.colSelect.Caption = " ";
            this.colSelect.ColumnEdit = this.repoCheck;
            this.colSelect.FieldName = "IsSelected";
            this.colSelect.Name = "colSelect";
            this.colSelect.OptionsColumn.ShowCaption = false;
            this.colSelect.Visible = true;
            this.colSelect.VisibleIndex = 0;
            this.colSelect.Width = 30;
            // 
            // repoCheck
            // 
            this.repoCheck.AutoHeight = false;
            this.repoCheck.Name = "repoCheck";
            // 
            // colSTTb
            // 
            this.colSTTb.Caption = "STT";
            this.colSTTb.FieldName = "STT";
            this.colSTTb.Name = "colSTTb";
            this.colSTTb.OptionsColumn.AllowEdit = false;
            this.colSTTb.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTTb.Visible = true;
            this.colSTTb.VisibleIndex = 1;
            this.colSTTb.Width = 40;
            // 
            // colPatientNameB
            // 
            this.colPatientNameB.Caption = "Tên bệnh nhân";
            this.colPatientNameB.FieldName = "TDL_PATIENT_NAME";
            this.colPatientNameB.Name = "colPatientNameB";
            this.colPatientNameB.OptionsColumn.AllowEdit = false;
            this.colPatientNameB.Visible = true;
            this.colPatientNameB.VisibleIndex = 2;
            this.colPatientNameB.Width = 170;
            // 
            // colPatientCodeB
            // 
            this.colPatientCodeB.Caption = "Mã bệnh nhân";
            this.colPatientCodeB.FieldName = "TDL_PATIENT_CODE";
            this.colPatientCodeB.Name = "colPatientCodeB";
            this.colPatientCodeB.OptionsColumn.AllowEdit = false;
            this.colPatientCodeB.Visible = true;
            this.colPatientCodeB.VisibleIndex = 3;
            this.colPatientCodeB.Width = 90;
            // 
            // colTreatmentCodeB
            // 
            this.colTreatmentCodeB.Caption = "Mã điều trị";
            this.colTreatmentCodeB.FieldName = "TREATMENT_CODE";
            this.colTreatmentCodeB.Name = "colTreatmentCodeB";
            this.colTreatmentCodeB.OptionsColumn.AllowEdit = false;
            this.colTreatmentCodeB.Visible = true;
            this.colTreatmentCodeB.VisibleIndex = 4;
            this.colTreatmentCodeB.Width = 100;
            // 
            // colDobB
            // 
            this.colDobB.Caption = "Ngày sinh";
            this.colDobB.FieldName = "TDL_PATIENT_DOB";
            this.colDobB.Name = "colDobB";
            this.colDobB.OptionsColumn.AllowEdit = false;
            this.colDobB.Visible = true;
            this.colDobB.VisibleIndex = 5;
            this.colDobB.Width = 70;
            // 
            // colGenderB
            // 
            this.colGenderB.Caption = "Giới tính";
            this.colGenderB.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.colGenderB.Name = "colGenderB";
            this.colGenderB.OptionsColumn.AllowEdit = false;
            this.colGenderB.Visible = true;
            this.colGenderB.VisibleIndex = 6;
            this.colGenderB.Width = 60;
            // 
            // colInTimeB
            // 
            this.colInTimeB.Caption = "Ngày vào";
            this.colInTimeB.FieldName = "IN_TIME";
            this.colInTimeB.Name = "colInTimeB";
            this.colInTimeB.OptionsColumn.AllowEdit = false;
            this.colInTimeB.Visible = true;
            this.colInTimeB.VisibleIndex = 7;
            this.colInTimeB.Width = 80;
            // 
            // colTreatmentTypeB
            // 
            this.colTreatmentTypeB.Caption = "Diện điều trị";
            this.colTreatmentTypeB.FieldName = "TDL_TREATMENT_TYPE_NAME";
            this.colTreatmentTypeB.Name = "colTreatmentTypeB";
            this.colTreatmentTypeB.OptionsColumn.AllowEdit = false;
            this.colTreatmentTypeB.Visible = true;
            this.colTreatmentTypeB.VisibleIndex = 8;
            this.colTreatmentTypeB.Width = 110;
            // 
            // colHeinCardB
            // 
            this.colHeinCardB.Caption = "Số thẻ BHYT";
            this.colHeinCardB.FieldName = "TDL_PATIENT_HEIN_CARD_NUMBER";
            this.colHeinCardB.Name = "colHeinCardB";
            this.colHeinCardB.OptionsColumn.AllowEdit = false;
            this.colHeinCardB.Visible = true;
            this.colHeinCardB.VisibleIndex = 9;
            this.colHeinCardB.Width = 130;
            // 
            // colIcdNameB
            // 
            this.colIcdNameB.Caption = "Chẩn đoán chính";
            this.colIcdNameB.FieldName = "ICD_NAME";
            this.colIcdNameB.Name = "colIcdNameB";
            this.colIcdNameB.OptionsColumn.AllowEdit = false;
            this.colIcdNameB.Visible = true;
            this.colIcdNameB.VisibleIndex = 10;
            this.colIcdNameB.Width = 220;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.lblDept);
            this.panelBottom.Controls.Add(this.cboDepartment);
            this.panelBottom.Controls.Add(this.chkAllDepartment);
            this.panelBottom.Controls.Add(this.lblFrom);
            this.panelBottom.Controls.Add(this.dtInTimeFrom);
            this.panelBottom.Controls.Add(this.lblTo);
            this.panelBottom.Controls.Add(this.dtInTimeTo);
            this.panelBottom.Controls.Add(this.txtSearchBottom);
            this.panelBottom.Controls.Add(this.btnSearchBottom);
            this.panelBottom.Controls.Add(this.chkSelectAll);
            this.panelBottom.Controls.Add(this.btnAddToSchedule);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBottom.Location = new System.Drawing.Point(0, 0);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1250, 36);
            this.panelBottom.TabIndex = 0;
            // 
            // lblDept
            // 
            this.lblDept.Location = new System.Drawing.Point(6, 11);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(28, 13);
            this.lblDept.TabIndex = 0;
            this.lblDept.Text = "Khoa:";
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(48, 8);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridViewDept;
            this.cboDepartment.Size = new System.Drawing.Size(160, 20);
            this.cboDepartment.TabIndex = 1;
            // 
            // gridViewDept
            // 
            this.gridViewDept.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDept.Name = "gridViewDept";
            this.gridViewDept.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDept.OptionsView.ShowGroupPanel = false;
            // 
            // chkAllDepartment
            // 
            this.chkAllDepartment.Location = new System.Drawing.Point(214, 9);
            this.chkAllDepartment.Name = "chkAllDepartment";
            this.chkAllDepartment.Properties.Caption = "Toàn khoa";
            this.chkAllDepartment.Size = new System.Drawing.Size(90, 19);
            this.chkAllDepartment.TabIndex = 2;
            this.chkAllDepartment.CheckedChanged += new System.EventHandler(this.chkAllDepartment_CheckedChanged);
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(310, 11);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(64, 13);
            this.lblFrom.TabIndex = 3;
            this.lblFrom.Text = "Ngày vào từ:";
            // 
            // dtInTimeFrom
            // 
            this.dtInTimeFrom.EditValue = null;
            this.dtInTimeFrom.Location = new System.Drawing.Point(382, 8);
            this.dtInTimeFrom.Name = "dtInTimeFrom";
            this.dtInTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInTimeFrom.Size = new System.Drawing.Size(95, 20);
            this.dtInTimeFrom.TabIndex = 4;
            // 
            // lblTo
            // 
            this.lblTo.Location = new System.Drawing.Point(485, 11);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(24, 13);
            this.lblTo.TabIndex = 5;
            this.lblTo.Text = "Đến:";
            // 
            // dtInTimeTo
            // 
            this.dtInTimeTo.EditValue = null;
            this.dtInTimeTo.Location = new System.Drawing.Point(514, 8);
            this.dtInTimeTo.Name = "dtInTimeTo";
            this.dtInTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtInTimeTo.Size = new System.Drawing.Size(95, 20);
            this.dtInTimeTo.TabIndex = 6;
            // 
            // txtSearchBottom
            // 
            this.txtSearchBottom.Location = new System.Drawing.Point(616, 8);
            this.txtSearchBottom.Name = "txtSearchBottom";
            this.txtSearchBottom.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearchBottom.Size = new System.Drawing.Size(160, 20);
            this.txtSearchBottom.TabIndex = 7;
            this.txtSearchBottom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearchBottom_KeyUp);
            // 
            // btnSearchBottom
            // 
            this.btnSearchBottom.Location = new System.Drawing.Point(782, 7);
            this.btnSearchBottom.Name = "btnSearchBottom";
            this.btnSearchBottom.Size = new System.Drawing.Size(115, 22);
            this.btnSearchBottom.TabIndex = 8;
            this.btnSearchBottom.Text = "Tìm (Ctrl Shift F)";
            this.btnSearchBottom.Click += new System.EventHandler(this.btnSearchBottom_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Location = new System.Drawing.Point(905, 9);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Properties.Caption = "Chọn tất cả";
            this.chkSelectAll.Size = new System.Drawing.Size(95, 19);
            this.chkSelectAll.TabIndex = 9;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // btnAddToSchedule
            // 
            this.btnAddToSchedule.Location = new System.Drawing.Point(1004, 7);
            this.btnAddToSchedule.Name = "btnAddToSchedule";
            this.btnAddToSchedule.Size = new System.Drawing.Size(140, 22);
            this.btnAddToSchedule.TabIndex = 10;
            this.btnAddToSchedule.Text = "Đưa vào lịch (Ctrl A)";
            this.btnAddToSchedule.Click += new System.EventHandler(this.btnAddToSchedule_Click);
            // 
            // frmHemodialysisSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1250, 700);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "frmHemodialysisSchedule";
            this.Text = "Xếp lịch chạy thận";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmHemodialysisSchedule_Load);
            this.Controls.SetChildIndex(this.splitContainerControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTemplateView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboShift.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchTop.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCopyFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDept)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAllDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchBottom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSelectAll.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;

        private DevExpress.XtraEditors.PanelControl panelTop;
        private DevExpress.XtraEditors.LabelControl lblRoom;
        private DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRoom;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private DevExpress.XtraEditors.DateEdit dtDate;
        private DevExpress.XtraEditors.LabelControl lblShift;
        private DevExpress.XtraEditors.GridLookUpEdit cboShift;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewShift;
        private DevExpress.XtraEditors.TextEdit txtSearchTop;
        private DevExpress.XtraEditors.SimpleButton btnSearchTop;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnCopy;
        private DevExpress.XtraEditors.LabelControl lblCopyFrom;
        private DevExpress.XtraEditors.DateEdit dtCopyFromDate;

        private DevExpress.XtraGrid.GridControl gridControlSchedule;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSchedule;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoDelete;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colScheduleDate;
        private DevExpress.XtraGrid.Columns.GridColumn colShift;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn colDob;
        private DevExpress.XtraGrid.Columns.GridColumn colGender;
        private DevExpress.XtraGrid.Columns.GridColumn colInTime;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientType;
        private DevExpress.XtraGrid.Columns.GridColumn colTemplate;
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repoTemplate;
        private DevExpress.XtraGrid.Views.Grid.GridView repoTemplateView;
        private DevExpress.XtraGrid.Columns.GridColumn colNote;
        private DevExpress.XtraGrid.Columns.GridColumn colCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colCreator;
        private DevExpress.XtraGrid.Columns.GridColumn colModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn colModifier;

        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.LabelControl lblDept;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDept;
        private DevExpress.XtraEditors.CheckEdit chkAllDepartment;
        private DevExpress.XtraEditors.LabelControl lblFrom;
        private DevExpress.XtraEditors.DateEdit dtInTimeFrom;
        private DevExpress.XtraEditors.LabelControl lblTo;
        private DevExpress.XtraEditors.DateEdit dtInTimeTo;
        private DevExpress.XtraEditors.TextEdit txtSearchBottom;
        private DevExpress.XtraEditors.SimpleButton btnSearchBottom;
        private DevExpress.XtraEditors.CheckEdit chkSelectAll;
        private DevExpress.XtraEditors.SimpleButton btnAddToSchedule;

        private DevExpress.XtraGrid.GridControl gridControlTreatment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatment;
        private DevExpress.XtraGrid.Columns.GridColumn colSelect;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheck;
        private DevExpress.XtraGrid.Columns.GridColumn colSTTb;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientNameB;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCodeB;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCodeB;
        private DevExpress.XtraGrid.Columns.GridColumn colDobB;
        private DevExpress.XtraGrid.Columns.GridColumn colGenderB;
        private DevExpress.XtraGrid.Columns.GridColumn colInTimeB;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentTypeB;
        private DevExpress.XtraGrid.Columns.GridColumn colHeinCardB;
        private DevExpress.XtraGrid.Columns.GridColumn colIcdNameB;
    }
}
