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
namespace LIS.Desktop.Plugins.SampleWarning
{
    partial class frmSampleWarning
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
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.gridControlSample = new DevExpress.XtraGrid.GridControl();
            this.gridViewSample = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Sample_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_Barcode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_SampleStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_IntructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_SampleTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_ApproveTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_SampleType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_ConditionName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_RequestUser = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_SampleUser = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_ApproveUser = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_RequestRoom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_ExecuteRoom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_Gender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Sample_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.spinHourNumber = new DevExpress.XtraEditors.SpinEdit();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.cboTypeFilter = new DevExpress.XtraEditors.ComboBoxEdit();
            this.dtIntructionTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtIntructionTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIntructionTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIntructionTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciHourNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnFind = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSample)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHourNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTypeFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHourNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.gridControlSample);
            this.layoutControl1.Controls.Add(this.spinHourNumber);
            this.layoutControl1.Controls.Add(this.btnRefresh);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Controls.Add(this.cboTypeFilter);
            this.layoutControl1.Controls.Add(this.dtIntructionTimeTo);
            this.layoutControl1.Controls.Add(this.dtIntructionTimeFrom);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 510);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(1096, 20);
            this.ucPaging1.TabIndex = 12;
            // 
            // gridControlSample
            // 
            this.gridControlSample.Location = new System.Drawing.Point(0, 26);
            this.gridControlSample.MainView = this.gridViewSample;
            this.gridControlSample.Name = "gridControlSample";
            this.gridControlSample.Size = new System.Drawing.Size(1100, 482);
            this.gridControlSample.TabIndex = 11;
            this.gridControlSample.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSample});
            // 
            // gridViewSample
            // 
            this.gridViewSample.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Sample_Stt,
            this.Gc_Sample_ServiceReqCode,
            this.Gc_Sample_TreatmentCode,
            this.Gc_Sample_PatientName,
            this.Gc_Sample_Barcode,
            this.Gc_Sample_SampleStt,
            this.Gc_Sample_IntructionTime,
            this.Gc_Sample_SampleTime,
            this.Gc_Sample_ApproveTime,
            this.Gc_Sample_SampleType,
            this.Gc_Sample_ConditionName,
            this.Gc_Sample_RequestUser,
            this.Gc_Sample_SampleUser,
            this.Gc_Sample_ApproveUser,
            this.Gc_Sample_RequestRoom,
            this.Gc_Sample_ExecuteRoom,
            this.Gc_Sample_Gender,
            this.Gc_Sample_Dob});
            this.gridViewSample.GridControl = this.gridControlSample;
            this.gridViewSample.Name = "gridViewSample";
            this.gridViewSample.OptionsView.ColumnAutoWidth = false;
            this.gridViewSample.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewSample.OptionsView.ShowGroupPanel = false;
            this.gridViewSample.OptionsView.ShowIndicator = false;
            this.gridViewSample.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSample_CustomUnboundColumnData);
            // 
            // Gc_Sample_Stt
            // 
            this.Gc_Sample_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Sample_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.Gc_Sample_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_Stt.Caption = "STT";
            this.Gc_Sample_Stt.FieldName = "STT";
            this.Gc_Sample_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Sample_Stt.Name = "Gc_Sample_Stt";
            this.Gc_Sample_Stt.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_Sample_Stt.OptionsColumn.ReadOnly = true;
            this.Gc_Sample_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_Stt.Visible = true;
            this.Gc_Sample_Stt.VisibleIndex = 0;
            this.Gc_Sample_Stt.Width = 45;
            // 
            // Gc_Sample_ServiceReqCode
            // 
            this.Gc_Sample_ServiceReqCode.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_ServiceReqCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ServiceReqCode.Caption = "Mã y lệnh";
            this.Gc_Sample_ServiceReqCode.FieldName = "SERVICE_REQ_CODE";
            this.Gc_Sample_ServiceReqCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Sample_ServiceReqCode.Name = "Gc_Sample_ServiceReqCode";
            this.Gc_Sample_ServiceReqCode.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_ServiceReqCode.Visible = true;
            this.Gc_Sample_ServiceReqCode.VisibleIndex = 1;
            this.Gc_Sample_ServiceReqCode.Width = 95;
            // 
            // Gc_Sample_TreatmentCode
            // 
            this.Gc_Sample_TreatmentCode.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_TreatmentCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_TreatmentCode.Caption = "Mã điều trị";
            this.Gc_Sample_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.Gc_Sample_TreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Sample_TreatmentCode.Name = "Gc_Sample_TreatmentCode";
            this.Gc_Sample_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_TreatmentCode.Visible = true;
            this.Gc_Sample_TreatmentCode.VisibleIndex = 2;
            this.Gc_Sample_TreatmentCode.Width = 95;
            // 
            // Gc_Sample_PatientName
            // 
            this.Gc_Sample_PatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_PatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_PatientName.Caption = "Tên bệnh nhân";
            this.Gc_Sample_PatientName.FieldName = "PATIENT_NAME";
            this.Gc_Sample_PatientName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Sample_PatientName.Name = "Gc_Sample_PatientName";
            this.Gc_Sample_PatientName.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_PatientName.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_PatientName.Visible = true;
            this.Gc_Sample_PatientName.VisibleIndex = 3;
            this.Gc_Sample_PatientName.Width = 120;
            // 
            // Gc_Sample_Barcode
            // 
            this.Gc_Sample_Barcode.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_Barcode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_Barcode.Caption = "Barcode";
            this.Gc_Sample_Barcode.FieldName = "BARCODE";
            this.Gc_Sample_Barcode.Name = "Gc_Sample_Barcode";
            this.Gc_Sample_Barcode.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_Barcode.Visible = true;
            this.Gc_Sample_Barcode.VisibleIndex = 4;
            this.Gc_Sample_Barcode.Width = 55;
            // 
            // Gc_Sample_SampleStt
            // 
            this.Gc_Sample_SampleStt.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_SampleStt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_SampleStt.Caption = "Trạng thái";
            this.Gc_Sample_SampleStt.FieldName = "SAMPLE_STT_NAME";
            this.Gc_Sample_SampleStt.Name = "Gc_Sample_SampleStt";
            this.Gc_Sample_SampleStt.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_SampleStt.Visible = true;
            this.Gc_Sample_SampleStt.VisibleIndex = 5;
            this.Gc_Sample_SampleStt.Width = 85;
            // 
            // Gc_Sample_IntructionTime
            // 
            this.Gc_Sample_IntructionTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Sample_IntructionTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_IntructionTime.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_IntructionTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_IntructionTime.Caption = "Thời gian y lệnh";
            this.Gc_Sample_IntructionTime.FieldName = "INTRUCTION_TIME_STR";
            this.Gc_Sample_IntructionTime.Name = "Gc_Sample_IntructionTime";
            this.Gc_Sample_IntructionTime.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_IntructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_IntructionTime.Visible = true;
            this.Gc_Sample_IntructionTime.VisibleIndex = 6;
            this.Gc_Sample_IntructionTime.Width = 125;
            // 
            // Gc_Sample_SampleTime
            // 
            this.Gc_Sample_SampleTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Sample_SampleTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_SampleTime.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_SampleTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_SampleTime.Caption = "Thời gian lấy mẫu";
            this.Gc_Sample_SampleTime.FieldName = "SAMPLE_TIME_STR";
            this.Gc_Sample_SampleTime.Name = "Gc_Sample_SampleTime";
            this.Gc_Sample_SampleTime.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_SampleTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_SampleTime.Visible = true;
            this.Gc_Sample_SampleTime.VisibleIndex = 7;
            this.Gc_Sample_SampleTime.Width = 125;
            // 
            // Gc_Sample_ApproveTime
            // 
            this.Gc_Sample_ApproveTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_Sample_ApproveTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ApproveTime.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_ApproveTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ApproveTime.Caption = "Thời gian duyệt mẫu";
            this.Gc_Sample_ApproveTime.FieldName = "APPROVE_TIME_STR";
            this.Gc_Sample_ApproveTime.Name = "Gc_Sample_ApproveTime";
            this.Gc_Sample_ApproveTime.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_ApproveTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_ApproveTime.Visible = true;
            this.Gc_Sample_ApproveTime.VisibleIndex = 8;
            this.Gc_Sample_ApproveTime.Width = 125;
            // 
            // Gc_Sample_SampleType
            // 
            this.Gc_Sample_SampleType.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_SampleType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_SampleType.Caption = "Loại mẫu";
            this.Gc_Sample_SampleType.FieldName = "SAMPLE_TYPE_NAME";
            this.Gc_Sample_SampleType.Name = "Gc_Sample_SampleType";
            this.Gc_Sample_SampleType.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_SampleType.Visible = true;
            this.Gc_Sample_SampleType.VisibleIndex = 9;
            // 
            // Gc_Sample_ConditionName
            // 
            this.Gc_Sample_ConditionName.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_ConditionName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ConditionName.Caption = "Tình trạng mẫu";
            this.Gc_Sample_ConditionName.FieldName = "SAMPLE_CONDITION_NAME";
            this.Gc_Sample_ConditionName.Name = "Gc_Sample_ConditionName";
            this.Gc_Sample_ConditionName.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_ConditionName.Visible = true;
            this.Gc_Sample_ConditionName.VisibleIndex = 10;
            this.Gc_Sample_ConditionName.Width = 85;
            // 
            // Gc_Sample_RequestUser
            // 
            this.Gc_Sample_RequestUser.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_RequestUser.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_RequestUser.Caption = "Người chỉ định";
            this.Gc_Sample_RequestUser.FieldName = "REQUEST_USER";
            this.Gc_Sample_RequestUser.Name = "Gc_Sample_RequestUser";
            this.Gc_Sample_RequestUser.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_RequestUser.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_RequestUser.Visible = true;
            this.Gc_Sample_RequestUser.VisibleIndex = 11;
            this.Gc_Sample_RequestUser.Width = 120;
            // 
            // Gc_Sample_SampleUser
            // 
            this.Gc_Sample_SampleUser.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_SampleUser.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_SampleUser.Caption = "Người lấy mẫu";
            this.Gc_Sample_SampleUser.FieldName = "SAMPLE_USER";
            this.Gc_Sample_SampleUser.Name = "Gc_Sample_SampleUser";
            this.Gc_Sample_SampleUser.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_SampleUser.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_SampleUser.Visible = true;
            this.Gc_Sample_SampleUser.VisibleIndex = 12;
            this.Gc_Sample_SampleUser.Width = 120;
            // 
            // Gc_Sample_ApproveUser
            // 
            this.Gc_Sample_ApproveUser.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_ApproveUser.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ApproveUser.Caption = "Người duyệt mẫu";
            this.Gc_Sample_ApproveUser.FieldName = "APPROVE_USER";
            this.Gc_Sample_ApproveUser.Name = "Gc_Sample_ApproveUser";
            this.Gc_Sample_ApproveUser.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_ApproveUser.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_ApproveUser.Visible = true;
            this.Gc_Sample_ApproveUser.VisibleIndex = 13;
            this.Gc_Sample_ApproveUser.Width = 120;
            // 
            // Gc_Sample_RequestRoom
            // 
            this.Gc_Sample_RequestRoom.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_RequestRoom.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_RequestRoom.Caption = "Phòng yêu cầu";
            this.Gc_Sample_RequestRoom.FieldName = "REQUEST_ROOM_NAME";
            this.Gc_Sample_RequestRoom.Name = "Gc_Sample_RequestRoom";
            this.Gc_Sample_RequestRoom.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_RequestRoom.Visible = true;
            this.Gc_Sample_RequestRoom.VisibleIndex = 14;
            this.Gc_Sample_RequestRoom.Width = 100;
            // 
            // Gc_Sample_ExecuteRoom
            // 
            this.Gc_Sample_ExecuteRoom.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_ExecuteRoom.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_ExecuteRoom.Caption = "Phòng xử lý";
            this.Gc_Sample_ExecuteRoom.FieldName = "EXECUTE_ROOM_NAME";
            this.Gc_Sample_ExecuteRoom.Name = "Gc_Sample_ExecuteRoom";
            this.Gc_Sample_ExecuteRoom.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_ExecuteRoom.Visible = true;
            this.Gc_Sample_ExecuteRoom.VisibleIndex = 15;
            this.Gc_Sample_ExecuteRoom.Width = 100;
            // 
            // Gc_Sample_Gender
            // 
            this.Gc_Sample_Gender.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_Gender.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_Gender.Caption = "Giới tính";
            this.Gc_Sample_Gender.FieldName = "GENDER_NAME";
            this.Gc_Sample_Gender.Name = "Gc_Sample_Gender";
            this.Gc_Sample_Gender.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_Gender.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_Gender.Visible = true;
            this.Gc_Sample_Gender.VisibleIndex = 16;
            this.Gc_Sample_Gender.Width = 60;
            // 
            // Gc_Sample_Dob
            // 
            this.Gc_Sample_Dob.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_Sample_Dob.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_Sample_Dob.Caption = "Ngày sinh";
            this.Gc_Sample_Dob.FieldName = "PATIENT_DOB_STR";
            this.Gc_Sample_Dob.Name = "Gc_Sample_Dob";
            this.Gc_Sample_Dob.OptionsColumn.AllowEdit = false;
            this.Gc_Sample_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Sample_Dob.Visible = true;
            this.Gc_Sample_Dob.VisibleIndex = 17;
            this.Gc_Sample_Dob.Width = 90;
            // 
            // spinHourNumber
            // 
            this.spinHourNumber.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinHourNumber.Location = new System.Drawing.Point(677, 2);
            this.spinHourNumber.Name = "spinHourNumber";
            this.spinHourNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinHourNumber.Properties.MaxValue = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.spinHourNumber.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spinHourNumber.Size = new System.Drawing.Size(71, 20);
            this.spinHourNumber.StyleController = this.layoutControl1;
            this.spinHourNumber.TabIndex = 10;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(992, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(106, 22);
            this.btnRefresh.StyleController = this.layoutControl1;
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(882, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(106, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 8;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // cboTypeFilter
            // 
            this.cboTypeFilter.Location = new System.Drawing.Point(752, 2);
            this.cboTypeFilter.Name = "cboTypeFilter";
            this.cboTypeFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboTypeFilter.Properties.Items.AddRange(new object[] {
            "Chưa lấy mẫu",
            "Chưa thực hiện",
            "Chưa trả kết quả"});
            this.cboTypeFilter.Size = new System.Drawing.Size(126, 20);
            this.cboTypeFilter.StyleController = this.layoutControl1;
            this.cboTypeFilter.TabIndex = 7;
            // 
            // dtIntructionTimeTo
            // 
            this.dtIntructionTimeTo.EditValue = null;
            this.dtIntructionTimeTo.Location = new System.Drawing.Point(477, 2);
            this.dtIntructionTimeTo.Name = "dtIntructionTimeTo";
            this.dtIntructionTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTimeTo.Size = new System.Drawing.Size(121, 20);
            this.dtIntructionTimeTo.StyleController = this.layoutControl1;
            this.dtIntructionTimeTo.TabIndex = 6;
            // 
            // dtIntructionTimeFrom
            // 
            this.dtIntructionTimeFrom.EditValue = null;
            this.dtIntructionTimeFrom.Location = new System.Drawing.Point(277, 2);
            this.dtIntructionTimeFrom.Name = "dtIntructionTimeFrom";
            this.dtIntructionTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTimeFrom.Size = new System.Drawing.Size(121, 20);
            this.dtIntructionTimeFrom.StyleController = this.layoutControl1;
            this.dtIntructionTimeFrom.TabIndex = 5;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 2);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Size = new System.Drawing.Size(196, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 4;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciIntructionTimeFrom,
            this.lciIntructionTimeTo,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.lciHourNumber,
            this.layoutControlItem8,
            this.layoutControlItem9});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1100, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.txtKeyword;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(200, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciIntructionTimeFrom
            // 
            this.lciIntructionTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionTimeFrom.Control = this.dtIntructionTimeFrom;
            this.lciIntructionTimeFrom.Location = new System.Drawing.Point(200, 0);
            this.lciIntructionTimeFrom.Name = "lciIntructionTimeFrom";
            this.lciIntructionTimeFrom.OptionsToolTip.ToolTip = "Thời gian y lệnh từ - đến";
            this.lciIntructionTimeFrom.Size = new System.Drawing.Size(200, 26);
            this.lciIntructionTimeFrom.Text = "Tg y lệnh từ:";
            this.lciIntructionTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionTimeFrom.TextSize = new System.Drawing.Size(70, 20);
            this.lciIntructionTimeFrom.TextToControlDistance = 5;
            // 
            // lciIntructionTimeTo
            // 
            this.lciIntructionTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionTimeTo.Control = this.dtIntructionTimeTo;
            this.lciIntructionTimeTo.Location = new System.Drawing.Point(400, 0);
            this.lciIntructionTimeTo.Name = "lciIntructionTimeTo";
            this.lciIntructionTimeTo.Size = new System.Drawing.Size(200, 26);
            this.lciIntructionTimeTo.Text = "Đến:";
            this.lciIntructionTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionTimeTo.TextSize = new System.Drawing.Size(70, 20);
            this.lciIntructionTimeTo.TextToControlDistance = 5;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cboTypeFilter;
            this.layoutControlItem4.Location = new System.Drawing.Point(750, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(130, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnFind;
            this.layoutControlItem5.Location = new System.Drawing.Point(880, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnRefresh;
            this.layoutControlItem6.Location = new System.Drawing.Point(990, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciHourNumber
            // 
            this.lciHourNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHourNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHourNumber.Control = this.spinHourNumber;
            this.lciHourNumber.Location = new System.Drawing.Point(600, 0);
            this.lciHourNumber.Name = "lciHourNumber";
            this.lciHourNumber.Size = new System.Drawing.Size(150, 26);
            this.lciHourNumber.Text = "Số giờ:";
            this.lciHourNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHourNumber.TextSize = new System.Drawing.Size(70, 20);
            this.lciHourNumber.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.gridControlSample;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem8.Size = new System.Drawing.Size(1100, 482);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.ucPaging1;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 508);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(1100, 24);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnFind,
            this.barBtnRefresh});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnFind),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnRefresh)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnFind
            // 
            this.barBtnFind.Caption = "Tìm (Ctrl F)";
            this.barBtnFind.Id = 0;
            this.barBtnFind.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnFind.Name = "barBtnFind";
            this.barBtnFind.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnFind_ItemClick);
            // 
            // barBtnRefresh
            // 
            this.barBtnRefresh.Caption = "Làm lại (Ctrl R)";
            this.barBtnRefresh.Id = 1;
            this.barBtnRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barBtnRefresh.Name = "barBtnRefresh";
            this.barBtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnRefresh_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1100, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Size = new System.Drawing.Size(1100, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 532);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1100, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // frmSampleWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmSampleWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cảnh báo xét nghiệm";
            this.Load += new System.EventHandler(this.frmSampleWarning_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSample)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHourNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTypeFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHourNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraGrid.GridControl gridControlSample;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSample;
        private DevExpress.XtraEditors.SpinEdit spinHourNumber;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraEditors.ComboBoxEdit cboTypeFilter;
        private DevExpress.XtraEditors.DateEdit dtIntructionTimeTo;
        private DevExpress.XtraEditors.DateEdit dtIntructionTimeFrom;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionTimeTo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem lciHourNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_Barcode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_SampleStt;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_IntructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_SampleTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_ApproveTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_SampleType;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_ConditionName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_RequestUser;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_SampleUser;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_ApproveUser;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_RequestRoom;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_ExecuteRoom;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_Gender;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sample_Dob;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnFind;
        private DevExpress.XtraBars.BarButtonItem barBtnRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
