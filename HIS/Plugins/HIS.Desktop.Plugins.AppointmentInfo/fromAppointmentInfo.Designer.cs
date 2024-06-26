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
namespace HIS.Desktop.Plugins.AppointmentInfo
{
    partial class fromAppointmentInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fromAppointmentInfo));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnAppointmentService = new DevExpress.XtraEditors.SimpleButton();
            this.txtAdvise = new DevExpress.XtraEditors.MemoEdit();
            this.gridControlExamRoom = new DevExpress.XtraGrid.GridControl();
            this.gridViewExamRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridCol_Select = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheck_Select = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridCol_RoomCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridCol_ExamRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.cboTimeFrame = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.spinDayNumber = new DevExpress.XtraEditors.SpinEdit();
            this.dtAppointmentTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciAppointmentTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDayNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTimeFrame = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAppointmentExamRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAdvise = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.imageListIcon = new System.Windows.Forms.ImageList(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdvise.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExamRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExamRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck_Select)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTimeFrame.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDayNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentExamRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAdvise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnAppointmentService);
            this.layoutControl1.Controls.Add(this.txtAdvise);
            this.layoutControl1.Controls.Add(this.gridControlExamRoom);
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Controls.Add(this.cboTimeFrame);
            this.layoutControl1.Controls.Add(this.spinDayNumber);
            this.layoutControl1.Controls.Add(this.dtAppointmentTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 532);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(742, 508);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(136, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAppointmentService
            // 
            this.btnAppointmentService.Location = new System.Drawing.Point(602, 508);
            this.btnAppointmentService.Name = "btnAppointmentService";
            this.btnAppointmentService.Size = new System.Drawing.Size(136, 22);
            this.btnAppointmentService.StyleController = this.layoutControl1;
            this.btnAppointmentService.TabIndex = 10;
            this.btnAppointmentService.Text = "Dịch vụ hẹn khám";
            this.btnAppointmentService.Click += new System.EventHandler(this.btnAppointmentService_Click);
            // 
            // txtAdvise
            // 
            this.txtAdvise.Location = new System.Drawing.Point(97, 369);
            this.txtAdvise.Name = "txtAdvise";
            this.txtAdvise.Size = new System.Drawing.Size(781, 135);
            this.txtAdvise.StyleController = this.layoutControl1;
            this.txtAdvise.TabIndex = 9;
            this.txtAdvise.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdvise_KeyDown);
            // 
            // gridControlExamRoom
            // 
            this.gridControlExamRoom.Location = new System.Drawing.Point(97, 50);
            this.gridControlExamRoom.MainView = this.gridViewExamRoom;
            this.gridControlExamRoom.Name = "gridControlExamRoom";
            this.gridControlExamRoom.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck_Select});
            this.gridControlExamRoom.Size = new System.Drawing.Size(781, 315);
            this.gridControlExamRoom.TabIndex = 8;
            this.gridControlExamRoom.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExamRoom});
            // 
            // gridViewExamRoom
            // 
            this.gridViewExamRoom.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridCol_Select,
            this.gridCol_RoomCode,
            this.gridCol_ExamRoomName});
            this.gridViewExamRoom.GridControl = this.gridControlExamRoom;
            this.gridViewExamRoom.Name = "gridViewExamRoom";
            this.gridViewExamRoom.OptionsView.ColumnAutoWidth = false;
            this.gridViewExamRoom.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExamRoom.OptionsView.ShowGroupPanel = false;
            this.gridViewExamRoom.OptionsView.ShowIndicator = false;
            this.gridViewExamRoom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewExamRoom_MouseDown);
            // 
            // gridCol_Select
            // 
            this.gridCol_Select.AppearanceCell.Options.UseTextOptions = true;
            this.gridCol_Select.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_Select.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_Select.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_Select.Caption = "Chọn";
            this.gridCol_Select.ColumnEdit = this.repositoryItemCheck_Select;
            this.gridCol_Select.FieldName = "IsCheck";
            this.gridCol_Select.Name = "gridCol_Select";
            this.gridCol_Select.OptionsColumn.ShowCaption = false;
            this.gridCol_Select.Visible = true;
            this.gridCol_Select.VisibleIndex = 0;
            this.gridCol_Select.Width = 30;
            // 
            // repositoryItemCheck_Select
            // 
            this.repositoryItemCheck_Select.AutoHeight = false;
            this.repositoryItemCheck_Select.Name = "repositoryItemCheck_Select";
            this.repositoryItemCheck_Select.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridCol_RoomCode
            // 
            this.gridCol_RoomCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_RoomCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_RoomCode.Caption = "Mã phòng khám";
            this.gridCol_RoomCode.FieldName = "EXECUTE_ROOM_CODE";
            this.gridCol_RoomCode.Name = "gridCol_RoomCode";
            this.gridCol_RoomCode.OptionsColumn.AllowEdit = false;
            this.gridCol_RoomCode.Visible = true;
            this.gridCol_RoomCode.VisibleIndex = 1;
            this.gridCol_RoomCode.Width = 150;
            // 
            // gridCol_ExamRoomName
            // 
            this.gridCol_ExamRoomName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridCol_ExamRoomName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridCol_ExamRoomName.Caption = "Tên phòng khám";
            this.gridCol_ExamRoomName.FieldName = "EXECUTE_ROOM_NAME";
            this.gridCol_ExamRoomName.Name = "gridCol_ExamRoomName";
            this.gridCol_ExamRoomName.OptionsColumn.AllowEdit = false;
            this.gridCol_ExamRoomName.Visible = true;
            this.gridCol_ExamRoomName.VisibleIndex = 2;
            this.gridCol_ExamRoomName.Width = 650;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(97, 26);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtSearch.Size = new System.Drawing.Size(781, 20);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 7;
            this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
            // 
            // cboTimeFrame
            // 
            this.cboTimeFrame.Location = new System.Drawing.Point(537, 2);
            this.cboTimeFrame.Name = "cboTimeFrame";
            this.cboTimeFrame.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTimeFrame.Properties.NullText = "";
            this.cboTimeFrame.Properties.View = this.gridLookUpEdit1View;
            this.cboTimeFrame.Size = new System.Drawing.Size(231, 20);
            this.cboTimeFrame.StyleController = this.layoutControl1;
            this.cboTimeFrame.TabIndex = 6;
            this.cboTimeFrame.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTimeFrame_Closed);
            this.cboTimeFrame.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTimeFrame_ButtonClick);
            this.cboTimeFrame.EditValueChanged += new System.EventHandler(this.cboTimeFrame_EditValueChanged);
            this.cboTimeFrame.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboTimeFrame_CustomDisplayText);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            this.gridLookUpEdit1View.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridLookUpEdit1View_RowCellStyle);
            this.gridLookUpEdit1View.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridLookUpEdit1View_CustomUnboundColumnData);
            // 
            // spinDayNumber
            // 
            this.spinDayNumber.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinDayNumber.Location = new System.Drawing.Point(317, 2);
            this.spinDayNumber.Name = "spinDayNumber";
            this.spinDayNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinDayNumber.Size = new System.Drawing.Size(121, 20);
            this.spinDayNumber.StyleController = this.layoutControl1;
            this.spinDayNumber.TabIndex = 5;
            this.spinDayNumber.EditValueChanged += new System.EventHandler(this.spinDayNumber_EditValueChanged);
            this.spinDayNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinDayNumber_PreviewKeyDown);
            // 
            // dtAppointmentTime
            // 
            this.dtAppointmentTime.EditValue = null;
            this.dtAppointmentTime.Location = new System.Drawing.Point(97, 2);
            this.dtAppointmentTime.Name = "dtAppointmentTime";
            this.dtAppointmentTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtAppointmentTime.Size = new System.Drawing.Size(121, 20);
            this.dtAppointmentTime.StyleController = this.layoutControl1;
            this.dtAppointmentTime.TabIndex = 4;
            this.dtAppointmentTime.EditValueChanged += new System.EventHandler(this.dtAppointmentTime_EditValueChanged);
            this.dtAppointmentTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtAppointmentTime_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciAppointmentTime,
            this.lciDayNumber,
            this.lciTimeFrame,
            this.lciSearch,
            this.lciAppointmentExamRoom,
            this.lciAdvise,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 532);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciAppointmentTime
            // 
            this.lciAppointmentTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciAppointmentTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciAppointmentTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAppointmentTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAppointmentTime.Control = this.dtAppointmentTime;
            this.lciAppointmentTime.Location = new System.Drawing.Point(0, 0);
            this.lciAppointmentTime.Name = "lciAppointmentTime";
            this.lciAppointmentTime.OptionsToolTip.ToolTip = "Thời gian hẹn khám";
            this.lciAppointmentTime.Size = new System.Drawing.Size(220, 24);
            this.lciAppointmentTime.Text = "Tg hẹn khám:";
            this.lciAppointmentTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAppointmentTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciAppointmentTime.TextToControlDistance = 5;
            // 
            // lciDayNumber
            // 
            this.lciDayNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDayNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDayNumber.Control = this.spinDayNumber;
            this.lciDayNumber.Location = new System.Drawing.Point(220, 0);
            this.lciDayNumber.Name = "lciDayNumber";
            this.lciDayNumber.Size = new System.Drawing.Size(220, 24);
            this.lciDayNumber.Text = "Số ngày:";
            this.lciDayNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDayNumber.TextSize = new System.Drawing.Size(90, 20);
            this.lciDayNumber.TextToControlDistance = 5;
            // 
            // lciTimeFrame
            // 
            this.lciTimeFrame.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTimeFrame.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTimeFrame.Control = this.cboTimeFrame;
            this.lciTimeFrame.Location = new System.Drawing.Point(440, 0);
            this.lciTimeFrame.Name = "lciTimeFrame";
            this.lciTimeFrame.Size = new System.Drawing.Size(330, 24);
            this.lciTimeFrame.Text = "Khung giờ:";
            this.lciTimeFrame.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTimeFrame.TextSize = new System.Drawing.Size(90, 20);
            this.lciTimeFrame.TextToControlDistance = 5;
            // 
            // lciSearch
            // 
            this.lciSearch.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSearch.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSearch.Control = this.txtSearch;
            this.lciSearch.Location = new System.Drawing.Point(0, 24);
            this.lciSearch.Name = "lciSearch";
            this.lciSearch.Size = new System.Drawing.Size(880, 24);
            this.lciSearch.Text = " ";
            this.lciSearch.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSearch.TextSize = new System.Drawing.Size(90, 20);
            this.lciSearch.TextToControlDistance = 5;
            // 
            // lciAppointmentExamRoom
            // 
            this.lciAppointmentExamRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAppointmentExamRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAppointmentExamRoom.Control = this.gridControlExamRoom;
            this.lciAppointmentExamRoom.Location = new System.Drawing.Point(0, 48);
            this.lciAppointmentExamRoom.Name = "lciAppointmentExamRoom";
            this.lciAppointmentExamRoom.OptionsToolTip.ToolTip = "Phòng khám lần sau";
            this.lciAppointmentExamRoom.Size = new System.Drawing.Size(880, 319);
            this.lciAppointmentExamRoom.Text = "Pk lần sau:";
            this.lciAppointmentExamRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAppointmentExamRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciAppointmentExamRoom.TextToControlDistance = 5;
            // 
            // lciAdvise
            // 
            this.lciAdvise.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAdvise.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAdvise.Control = this.txtAdvise;
            this.lciAdvise.Location = new System.Drawing.Point(0, 367);
            this.lciAdvise.Name = "lciAdvise";
            this.lciAdvise.Size = new System.Drawing.Size(880, 139);
            this.lciAdvise.Text = "Lời dặn bác sỹ:";
            this.lciAdvise.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAdvise.TextSize = new System.Drawing.Size(90, 20);
            this.lciAdvise.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnAppointmentService;
            this.layoutControlItem7.Location = new System.Drawing.Point(600, 506);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(140, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnSave;
            this.layoutControlItem8.Location = new System.Drawing.Point(740, 506);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(140, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 506);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(600, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(770, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(110, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // imageListIcon
            // 
            this.imageListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcon.ImageStream")));
            this.imageListIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcon.Images.SetKeyName(0, "01.png");
            this.imageListIcon.Images.SetKeyName(1, "02.png");
            this.imageListIcon.Images.SetKeyName(2, "04.png");
            this.imageListIcon.Images.SetKeyName(3, "03.png");
            this.imageListIcon.Images.SetKeyName(4, "05.png");
            this.imageListIcon.Images.SetKeyName(5, "dau tích-01.jpg");
            this.imageListIcon.Images.SetKeyName(6, "dau tích-02.jpg");
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSave
            // 
            this.barBtnSave.Caption = "Lưu (Ctrl S)";
            this.barBtnSave.Id = 0;
            this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnSave.Name = "barBtnSave";
            this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(880, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 561);
            this.barDockControlBottom.Size = new System.Drawing.Size(880, 0);
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
            this.barDockControlRight.Location = new System.Drawing.Point(880, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 532);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // fromAppointmentInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 561);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "fromAppointmentInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.fromAppointmentInfo_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAdvise.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExamRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExamRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck_Select)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTimeFrame.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinDayNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDayNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentExamRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAdvise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit cboTimeFrame;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.SpinEdit spinDayNumber;
        private DevExpress.XtraEditors.DateEdit dtAppointmentTime;
        private DevExpress.XtraLayout.LayoutControlItem lciAppointmentTime;
        private DevExpress.XtraLayout.LayoutControlItem lciDayNumber;
        private DevExpress.XtraLayout.LayoutControlItem lciTimeFrame;
        private DevExpress.XtraGrid.GridControl gridControlExamRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExamRoom;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciAppointmentExamRoom;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnAppointmentService;
        private DevExpress.XtraEditors.MemoEdit txtAdvise;
        private DevExpress.XtraLayout.LayoutControlItem lciAdvise;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private System.Windows.Forms.ImageList imageListIcon;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_Select;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck_Select;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_RoomCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridCol_ExamRoomName;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
