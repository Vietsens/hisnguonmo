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
namespace HIS.Desktop.Plugins.ExpMestSaleCreate.FormChoosePrescription
{
    partial class FormPrescription
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrescription));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlServiceReqSearch = new DevExpress.XtraGrid.GridControl();
            this.gridViewServiceReqSearch = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gvS_Gc_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_PatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_PatientDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_PatientGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_IntructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_RequestRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvS_Gc_RequestName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.cboStatus = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.txtServiceReqCode = new DevExpress.XtraEditors.TextEdit();
            this.gridControlSelecteServiceReq = new DevExpress.XtraGrid.GridControl();
            this.gridViewSelecteServiceReq = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gvSvs_gc_STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvSvs_gc_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gvSvs_gc_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvSvs_gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvSvs_gc_PatientDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvSvs_gc_PatientGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gvSvs_gc_IntructinonTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dtIntructionDateTo = new DevExpress.XtraEditors.DateEdit();
            this.dtIntructionDatefrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciIntructionDatefrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIntructionDateTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciServiceReqCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciKeyword = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManagerShorcut = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnChoose = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceReqSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReqSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceReqCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelecteServiceReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelecteServiceReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDateTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDateTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDatefrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDatefrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDatefrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDateTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerShorcut)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Controls.Add(this.gridControlServiceReqSearch);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.cboStatus);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.txtPatientCode);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Controls.Add(this.txtServiceReqCode);
            this.layoutControl1.Controls.Add(this.gridControlSelecteServiceReq);
            this.layoutControl1.Controls.Add(this.dtIntructionDateTo);
            this.layoutControl1.Controls.Add(this.dtIntructionDatefrom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(992, 508);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(106, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 15;
            this.btnChoose.Text = "Đồng ý (Ctrl A)";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // gridControlServiceReqSearch
            // 
            this.gridControlServiceReqSearch.Location = new System.Drawing.Point(2, 52);
            this.gridControlServiceReqSearch.MainView = this.gridViewServiceReqSearch;
            this.gridControlServiceReqSearch.Name = "gridControlServiceReqSearch";
            this.gridControlServiceReqSearch.Size = new System.Drawing.Size(766, 452);
            this.gridControlServiceReqSearch.TabIndex = 14;
            this.gridControlServiceReqSearch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewServiceReqSearch});
            // 
            // gridViewServiceReqSearch
            // 
            this.gridViewServiceReqSearch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gvS_Gc_Stt,
            this.gvS_Gc_ServiceReqCode,
            this.gvS_Gc_TreatmentCode,
            this.gvS_Gc_PatientCode,
            this.gvS_Gc_PatientName,
            this.gvS_Gc_PatientDob,
            this.gvS_Gc_PatientGender,
            this.gvS_Gc_IntructionTime,
            this.gvS_Gc_RequestRoomName,
            this.gvS_Gc_RequestName});
            this.gridViewServiceReqSearch.GridControl = this.gridControlServiceReqSearch;
            this.gridViewServiceReqSearch.Name = "gridViewServiceReqSearch";
            this.gridViewServiceReqSearch.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewServiceReqSearch.OptionsSelection.MultiSelect = true;
            this.gridViewServiceReqSearch.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewServiceReqSearch.OptionsView.ColumnAutoWidth = false;
            this.gridViewServiceReqSearch.OptionsView.ShowGroupPanel = false;
            this.gridViewServiceReqSearch.OptionsView.ShowIndicator = false;
            this.gridViewServiceReqSearch.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewServiceReqSearch_SelectionChanged);
            this.gridViewServiceReqSearch.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewServiceReqSearch_CustomUnboundColumnData);
            // 
            // gvS_Gc_Stt
            // 
            this.gvS_Gc_Stt.Caption = "STT";
            this.gvS_Gc_Stt.FieldName = "STT";
            this.gvS_Gc_Stt.Name = "gvS_Gc_Stt";
            this.gvS_Gc_Stt.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_Stt.OptionsFilter.AllowFilter = false;
            this.gvS_Gc_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvS_Gc_Stt.Visible = true;
            this.gvS_Gc_Stt.VisibleIndex = 1;
            this.gvS_Gc_Stt.Width = 30;
            // 
            // gvS_Gc_ServiceReqCode
            // 
            this.gvS_Gc_ServiceReqCode.Caption = "Mã y lệnh";
            this.gvS_Gc_ServiceReqCode.FieldName = "SERVICE_REQ_CODE";
            this.gvS_Gc_ServiceReqCode.Name = "gvS_Gc_ServiceReqCode";
            this.gvS_Gc_ServiceReqCode.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_ServiceReqCode.Visible = true;
            this.gvS_Gc_ServiceReqCode.VisibleIndex = 2;
            this.gvS_Gc_ServiceReqCode.Width = 100;
            // 
            // gvS_Gc_TreatmentCode
            // 
            this.gvS_Gc_TreatmentCode.Caption = "Mã điều trị";
            this.gvS_Gc_TreatmentCode.FieldName = "TDL_TREATMENT_CODE";
            this.gvS_Gc_TreatmentCode.Name = "gvS_Gc_TreatmentCode";
            this.gvS_Gc_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_TreatmentCode.Visible = true;
            this.gvS_Gc_TreatmentCode.VisibleIndex = 3;
            this.gvS_Gc_TreatmentCode.Width = 100;
            // 
            // gvS_Gc_PatientCode
            // 
            this.gvS_Gc_PatientCode.Caption = "Mã bệnh nhân";
            this.gvS_Gc_PatientCode.FieldName = "TDL_PATIENT_CODE";
            this.gvS_Gc_PatientCode.Name = "gvS_Gc_PatientCode";
            this.gvS_Gc_PatientCode.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_PatientCode.Visible = true;
            this.gvS_Gc_PatientCode.VisibleIndex = 4;
            this.gvS_Gc_PatientCode.Width = 100;
            // 
            // gvS_Gc_PatientName
            // 
            this.gvS_Gc_PatientName.Caption = "Tên bệnh nhân";
            this.gvS_Gc_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.gvS_Gc_PatientName.Name = "gvS_Gc_PatientName";
            this.gvS_Gc_PatientName.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_PatientName.Visible = true;
            this.gvS_Gc_PatientName.VisibleIndex = 5;
            this.gvS_Gc_PatientName.Width = 150;
            // 
            // gvS_Gc_PatientDob
            // 
            this.gvS_Gc_PatientDob.Caption = "Ngày sinh";
            this.gvS_Gc_PatientDob.FieldName = "DOB_STR";
            this.gvS_Gc_PatientDob.Name = "gvS_Gc_PatientDob";
            this.gvS_Gc_PatientDob.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_PatientDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvS_Gc_PatientDob.Visible = true;
            this.gvS_Gc_PatientDob.VisibleIndex = 6;
            this.gvS_Gc_PatientDob.Width = 100;
            // 
            // gvS_Gc_PatientGender
            // 
            this.gvS_Gc_PatientGender.Caption = "Giới tính";
            this.gvS_Gc_PatientGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gvS_Gc_PatientGender.Name = "gvS_Gc_PatientGender";
            this.gvS_Gc_PatientGender.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_PatientGender.Visible = true;
            this.gvS_Gc_PatientGender.VisibleIndex = 7;
            this.gvS_Gc_PatientGender.Width = 70;
            // 
            // gvS_Gc_IntructionTime
            // 
            this.gvS_Gc_IntructionTime.Caption = "Thời gian y lệnh";
            this.gvS_Gc_IntructionTime.FieldName = "INTRUCTION_TIME_STR";
            this.gvS_Gc_IntructionTime.Name = "gvS_Gc_IntructionTime";
            this.gvS_Gc_IntructionTime.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_IntructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvS_Gc_IntructionTime.Visible = true;
            this.gvS_Gc_IntructionTime.VisibleIndex = 8;
            this.gvS_Gc_IntructionTime.Width = 120;
            // 
            // gvS_Gc_RequestRoomName
            // 
            this.gvS_Gc_RequestRoomName.Caption = "Phòng yêu cầu";
            this.gvS_Gc_RequestRoomName.FieldName = "REQUEST_ROOM_NAME";
            this.gvS_Gc_RequestRoomName.Name = "gvS_Gc_RequestRoomName";
            this.gvS_Gc_RequestRoomName.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_RequestRoomName.Visible = true;
            this.gvS_Gc_RequestRoomName.VisibleIndex = 9;
            this.gvS_Gc_RequestRoomName.Width = 120;
            // 
            // gvS_Gc_RequestName
            // 
            this.gvS_Gc_RequestName.Caption = "Người chỉ định";
            this.gvS_Gc_RequestName.FieldName = "REQUEST_NAME_STR";
            this.gvS_Gc_RequestName.Name = "gvS_Gc_RequestName";
            this.gvS_Gc_RequestName.OptionsColumn.AllowEdit = false;
            this.gvS_Gc_RequestName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvS_Gc_RequestName.Visible = true;
            this.gvS_Gc_RequestName.VisibleIndex = 10;
            this.gvS_Gc_RequestName.Width = 150;
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 508);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(766, 22);
            this.ucPaging1.TabIndex = 13;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(633, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(135, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Tìm(Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cboStatus
            // 
            this.cboStatus.Location = new System.Drawing.Point(355, 26);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.cboStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboStatus.Properties.NullText = "";
            this.cboStatus.Properties.View = this.gridLookUpEdit1View;
            this.cboStatus.Size = new System.Drawing.Size(128, 20);
            this.cboStatus.StyleController = this.layoutControl1;
            this.cboStatus.TabIndex = 11;
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 26);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(349, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 10;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(633, 2);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPatientCode.Size = new System.Drawing.Size(135, 20);
            this.txtPatientCode.StyleController = this.layoutControl1;
            this.txtPatientCode.TabIndex = 9;
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(487, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(142, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 8;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // txtServiceReqCode
            // 
            this.txtServiceReqCode.Location = new System.Drawing.Point(355, 2);
            this.txtServiceReqCode.Name = "txtServiceReqCode";
            this.txtServiceReqCode.Properties.NullValuePrompt = "Mã y lệnh";
            this.txtServiceReqCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtServiceReqCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtServiceReqCode.Size = new System.Drawing.Size(128, 20);
            this.txtServiceReqCode.StyleController = this.layoutControl1;
            this.txtServiceReqCode.TabIndex = 7;
            this.txtServiceReqCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceReqCode_PreviewKeyDown);
            // 
            // gridControlSelecteServiceReq
            // 
            this.gridControlSelecteServiceReq.Location = new System.Drawing.Point(772, 2);
            this.gridControlSelecteServiceReq.MainView = this.gridViewSelecteServiceReq;
            this.gridControlSelecteServiceReq.Name = "gridControlSelecteServiceReq";
            this.gridControlSelecteServiceReq.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemBtnDelete});
            this.gridControlSelecteServiceReq.Size = new System.Drawing.Size(326, 502);
            this.gridControlSelecteServiceReq.TabIndex = 6;
            this.gridControlSelecteServiceReq.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSelecteServiceReq});
            // 
            // gridViewSelecteServiceReq
            // 
            this.gridViewSelecteServiceReq.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gvSvs_gc_STT,
            this.gvSvs_gc_Delete,
            this.gvSvs_gc_ServiceReqCode,
            this.gvSvs_gc_PatientName,
            this.gvSvs_gc_PatientDob,
            this.gvSvs_gc_PatientGender,
            this.gvSvs_gc_IntructinonTime});
            this.gridViewSelecteServiceReq.GridControl = this.gridControlSelecteServiceReq;
            this.gridViewSelecteServiceReq.Name = "gridViewSelecteServiceReq";
            this.gridViewSelecteServiceReq.OptionsView.ColumnAutoWidth = false;
            this.gridViewSelecteServiceReq.OptionsView.ShowGroupPanel = false;
            this.gridViewSelecteServiceReq.OptionsView.ShowIndicator = false;
            this.gridViewSelecteServiceReq.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSelecteServiceReq_CustomUnboundColumnData);
            // 
            // gvSvs_gc_STT
            // 
            this.gvSvs_gc_STT.Caption = "STT";
            this.gvSvs_gc_STT.FieldName = "STT";
            this.gvSvs_gc_STT.Name = "gvSvs_gc_STT";
            this.gvSvs_gc_STT.OptionsColumn.AllowEdit = false;
            this.gvSvs_gc_STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvSvs_gc_STT.Visible = true;
            this.gvSvs_gc_STT.VisibleIndex = 0;
            this.gvSvs_gc_STT.Width = 30;
            // 
            // gvSvs_gc_Delete
            // 
            this.gvSvs_gc_Delete.Caption = "xóa";
            this.gvSvs_gc_Delete.ColumnEdit = this.repositoryItemBtnDelete;
            this.gvSvs_gc_Delete.FieldName = "DELETE";
            this.gvSvs_gc_Delete.Name = "gvSvs_gc_Delete";
            this.gvSvs_gc_Delete.OptionsColumn.ShowCaption = false;
            this.gvSvs_gc_Delete.Visible = true;
            this.gvSvs_gc_Delete.VisibleIndex = 1;
            this.gvSvs_gc_Delete.Width = 25;
            // 
            // repositoryItemBtnDelete
            // 
            this.repositoryItemBtnDelete.AutoHeight = false;
            this.repositoryItemBtnDelete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnDelete.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.repositoryItemBtnDelete.Name = "repositoryItemBtnDelete";
            this.repositoryItemBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnDelete_ButtonClick);
            // 
            // gvSvs_gc_ServiceReqCode
            // 
            this.gvSvs_gc_ServiceReqCode.Caption = "Mã y lệnh";
            this.gvSvs_gc_ServiceReqCode.FieldName = "SERVICE_REQ_CODE";
            this.gvSvs_gc_ServiceReqCode.Name = "gvSvs_gc_ServiceReqCode";
            this.gvSvs_gc_ServiceReqCode.OptionsColumn.AllowEdit = false;
            this.gvSvs_gc_ServiceReqCode.Visible = true;
            this.gvSvs_gc_ServiceReqCode.VisibleIndex = 2;
            this.gvSvs_gc_ServiceReqCode.Width = 100;
            // 
            // gvSvs_gc_PatientName
            // 
            this.gvSvs_gc_PatientName.Caption = "Tên bệnh nhân";
            this.gvSvs_gc_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.gvSvs_gc_PatientName.Name = "gvSvs_gc_PatientName";
            this.gvSvs_gc_PatientName.OptionsColumn.AllowEdit = false;
            this.gvSvs_gc_PatientName.Visible = true;
            this.gvSvs_gc_PatientName.VisibleIndex = 3;
            this.gvSvs_gc_PatientName.Width = 120;
            // 
            // gvSvs_gc_PatientDob
            // 
            this.gvSvs_gc_PatientDob.Caption = "Ngày sinh";
            this.gvSvs_gc_PatientDob.FieldName = "DOB_STR";
            this.gvSvs_gc_PatientDob.Name = "gvSvs_gc_PatientDob";
            this.gvSvs_gc_PatientDob.OptionsColumn.AllowEdit = false;
            this.gvSvs_gc_PatientDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvSvs_gc_PatientDob.Visible = true;
            this.gvSvs_gc_PatientDob.VisibleIndex = 4;
            this.gvSvs_gc_PatientDob.Width = 100;
            // 
            // gvSvs_gc_PatientGender
            // 
            this.gvSvs_gc_PatientGender.Caption = "Giới tính";
            this.gvSvs_gc_PatientGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gvSvs_gc_PatientGender.Name = "gvSvs_gc_PatientGender";
            this.gvSvs_gc_PatientGender.OptionsColumn.AllowEdit = false;
            this.gvSvs_gc_PatientGender.Visible = true;
            this.gvSvs_gc_PatientGender.VisibleIndex = 5;
            this.gvSvs_gc_PatientGender.Width = 70;
            // 
            // gvSvs_gc_IntructinonTime
            // 
            this.gvSvs_gc_IntructinonTime.Caption = "Thời gian y lệnh";
            this.gvSvs_gc_IntructinonTime.FieldName = "INTRUCTION_TIME_STR";
            this.gvSvs_gc_IntructinonTime.Name = "gvSvs_gc_IntructinonTime";
            this.gvSvs_gc_IntructinonTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gvSvs_gc_IntructinonTime.Visible = true;
            this.gvSvs_gc_IntructinonTime.VisibleIndex = 6;
            this.gvSvs_gc_IntructinonTime.Width = 120;
            // 
            // dtIntructionDateTo
            // 
            this.dtIntructionDateTo.EditValue = null;
            this.dtIntructionDateTo.Location = new System.Drawing.Point(221, 2);
            this.dtIntructionDateTo.Name = "dtIntructionDateTo";
            this.dtIntructionDateTo.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtIntructionDateTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionDateTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionDateTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionDateTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionDateTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionDateTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionDateTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtIntructionDateTo.Size = new System.Drawing.Size(130, 20);
            this.dtIntructionDateTo.StyleController = this.layoutControl1;
            this.dtIntructionDateTo.TabIndex = 5;
            // 
            // dtIntructionDatefrom
            // 
            this.dtIntructionDatefrom.EditValue = null;
            this.dtIntructionDatefrom.Location = new System.Drawing.Point(97, 2);
            this.dtIntructionDatefrom.Name = "dtIntructionDatefrom";
            this.dtIntructionDatefrom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.dtIntructionDatefrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionDatefrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionDatefrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionDatefrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionDatefrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionDatefrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionDatefrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtIntructionDatefrom.Size = new System.Drawing.Size(120, 20);
            this.dtIntructionDatefrom.StyleController = this.layoutControl1;
            this.dtIntructionDatefrom.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciIntructionDatefrom,
            this.lciIntructionDateTo,
            this.lciStatus,
            this.layoutControlItem9,
            this.layoutControlItem11,
            this.layoutControlItem3,
            this.layoutControlItem10,
            this.layoutControlItem12,
            this.emptySpaceItem2,
            this.lciServiceReqCode,
            this.lciTreatmentCode,
            this.lciPatientCode,
            this.emptySpaceItem3,
            this.lciKeyword});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciIntructionDatefrom
            // 
            this.lciIntructionDatefrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionDatefrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionDatefrom.Control = this.dtIntructionDatefrom;
            this.lciIntructionDatefrom.Location = new System.Drawing.Point(0, 0);
            this.lciIntructionDatefrom.Name = "lciIntructionDatefrom";
            this.lciIntructionDatefrom.Size = new System.Drawing.Size(219, 24);
            this.lciIntructionDatefrom.Text = "Ngày y lệnh:";
            this.lciIntructionDatefrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionDatefrom.TextSize = new System.Drawing.Size(90, 20);
            this.lciIntructionDatefrom.TextToControlDistance = 5;
            // 
            // lciIntructionDateTo
            // 
            this.lciIntructionDateTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionDateTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionDateTo.Control = this.dtIntructionDateTo;
            this.lciIntructionDateTo.Location = new System.Drawing.Point(219, 0);
            this.lciIntructionDateTo.Name = "lciIntructionDateTo";
            this.lciIntructionDateTo.Size = new System.Drawing.Size(134, 24);
            this.lciIntructionDateTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionDateTo.TextSize = new System.Drawing.Size(0, 0);
            this.lciIntructionDateTo.TextToControlDistance = 0;
            this.lciIntructionDateTo.TextVisible = false;
            // 
            // lciStatus
            // 
            this.lciStatus.Control = this.cboStatus;
            this.lciStatus.Location = new System.Drawing.Point(353, 24);
            this.lciStatus.Name = "lciStatus";
            this.lciStatus.Size = new System.Drawing.Size(132, 26);
            this.lciStatus.TextSize = new System.Drawing.Size(0, 0);
            this.lciStatus.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnSearch;
            this.layoutControlItem9.Location = new System.Drawing.Point(631, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(139, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.gridControlServiceReqSearch;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(770, 456);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlSelecteServiceReq;
            this.layoutControlItem3.Location = new System.Drawing.Point(770, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(330, 506);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.ucPaging1;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 506);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(770, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.btnChoose;
            this.layoutControlItem12.Location = new System.Drawing.Point(990, 506);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(770, 506);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(220, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciServiceReqCode
            // 
            this.lciServiceReqCode.Control = this.txtServiceReqCode;
            this.lciServiceReqCode.Location = new System.Drawing.Point(353, 0);
            this.lciServiceReqCode.Name = "lciServiceReqCode";
            this.lciServiceReqCode.Size = new System.Drawing.Size(132, 24);
            this.lciServiceReqCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciServiceReqCode.TextVisible = false;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.Control = this.txtTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(485, 0);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(146, 24);
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciTreatmentCode.TextVisible = false;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.Control = this.txtPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(631, 0);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(139, 24);
            this.lciPatientCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientCode.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(485, 24);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(146, 26);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciKeyword
            // 
            this.lciKeyword.Control = this.txtKeyword;
            this.lciKeyword.Location = new System.Drawing.Point(0, 24);
            this.lciKeyword.Name = "lciKeyword";
            this.lciKeyword.Size = new System.Drawing.Size(353, 26);
            this.lciKeyword.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyword.TextVisible = false;
            // 
            // barManagerShorcut
            // 
            this.barManagerShorcut.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManagerShorcut.DockControls.Add(this.barDockControl1);
            this.barManagerShorcut.DockControls.Add(this.barDockControl2);
            this.barManagerShorcut.DockControls.Add(this.barDockControl3);
            this.barManagerShorcut.DockControls.Add(this.barDockControl4);
            this.barManagerShorcut.Form = this;
            this.barManagerShorcut.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnFind,
            this.barBtnChoose});
            this.barManagerShorcut.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnFind),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnChoose)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnFind
            // 
            this.barBtnFind.Caption = "Ctrl F";
            this.barBtnFind.Id = 0;
            this.barBtnFind.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnFind.Name = "barBtnFind";
            this.barBtnFind.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFind_ItemClick);
            // 
            // barBtnChoose
            // 
            this.barBtnChoose.Caption = "Ctrl A";
            this.barBtnChoose.Id = 1;
            this.barBtnChoose.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A));
            this.barBtnChoose.Name = "barBtnChoose";
            this.barBtnChoose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnChoose_ItemClick);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(1100, 29);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 561);
            this.barDockControl2.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 29);
            this.barDockControl3.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(1100, 29);
            this.barDockControl4.Size = new System.Drawing.Size(0, 532);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormPrescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "FormPrescription";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách đơn mua ngoài";
            this.Load += new System.EventHandler(this.FormPrescription_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlServiceReqSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReqSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceReqCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelecteServiceReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelecteServiceReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDateTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDateTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDatefrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionDatefrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDatefrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDateTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerShorcut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraGrid.GridControl gridControlServiceReqSearch;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewServiceReqSearch;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.GridLookUpEdit cboStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraEditors.TextEdit txtServiceReqCode;
        private DevExpress.XtraGrid.GridControl gridControlSelecteServiceReq;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSelecteServiceReq;
        private DevExpress.XtraEditors.DateEdit dtIntructionDateTo;
        private DevExpress.XtraEditors.DateEdit dtIntructionDatefrom;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionDatefrom;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionDateTo;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyword;
        private DevExpress.XtraLayout.LayoutControlItem lciStatus;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_PatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_PatientDob;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_PatientGender;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_IntructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_RequestRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gvS_Gc_RequestName;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_STT;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_Delete;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_PatientDob;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_PatientGender;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gvSvs_gc_IntructinonTime;
        private DevExpress.XtraBars.BarManager barManagerShorcut;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraBars.BarButtonItem barBtnFind;
        private DevExpress.XtraBars.BarButtonItem barBtnChoose;
        private System.Windows.Forms.Timer timer1;
    }
}
