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
namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient
{
    partial class frmPublicMedicineMultiPatient
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblSelectedCount = new DevExpress.XtraEditors.LabelControl();
            this.chkTachHDSD = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.chkSign = new DevExpress.XtraEditors.CheckEdit();
            this.chkPrintDocumentSigned = new DevExpress.XtraEditors.CheckEdit();
            this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
            this.groupControlPatient = new DevExpress.XtraEditors.GroupControl();
            this.gridControlPatient = new DevExpress.XtraGrid.GridControl();
            this.gridViewPatient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEditPatient = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcBedRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcBedName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.groupControlFilterCondition = new DevExpress.XtraEditors.GroupControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkSortEstablish = new DevExpress.XtraEditors.CheckEdit();
            this.chkSortName = new DevExpress.XtraEditors.CheckEdit();
            this.checkedCboHisPatientType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkIsExpend = new DevExpress.XtraEditors.CheckEdit();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkChemical = new DevExpress.XtraEditors.CheckEdit();
            this.chkBlood = new DevExpress.XtraEditors.CheckEdit();
            this.dtToTime = new DevExpress.XtraEditors.DateEdit();
            this.dtFromTime = new DevExpress.XtraEditors.DateEdit();
            this.chkMaterial = new DevExpress.XtraEditors.CheckEdit();
            this.chkMedicine = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMedicine = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMaterial = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBlood = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChemical = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsExpend = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciToTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciSortName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSortEstablish = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciGroupPatient = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGroupFilter = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSelectedCount = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTachHDSD = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSign = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrintDocumentSigned = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCreate = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkTachHDSD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSign.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintDocumentSigned.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlPatient)).BeginInit();
            this.groupControlPatient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlFilterCondition)).BeginInit();
            this.groupControlFilterCondition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSortEstablish.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSortName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedCboHisPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsExpend.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkChemical.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBlood.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMaterial.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedicine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChemical)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsExpend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSortName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSortEstablish)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGroupPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGroupFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSelectedCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTachHDSD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintDocumentSigned)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCreate)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.lblSelectedCount);
            this.layoutControl1.Controls.Add(this.chkTachHDSD);
            this.layoutControl1.Controls.Add(this.chkSign);
            this.layoutControl1.Controls.Add(this.chkPrintDocumentSigned);
            this.layoutControl1.Controls.Add(this.btnCreate);
            this.layoutControl1.Controls.Add(this.groupControlPatient);
            this.layoutControl1.Controls.Add(this.groupControlFilterCondition);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(966, 560);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // lblSelectedCount
            //
            this.lblSelectedCount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSelectedCount.Location = new System.Drawing.Point(2, 536);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(498, 22);
            this.lblSelectedCount.StyleController = this.layoutControl1;
            this.lblSelectedCount.TabIndex = 7;
            //
            // chkTachHDSD
            //
            this.chkTachHDSD.Location = new System.Drawing.Point(599, 536);
            this.chkTachHDSD.MenuManager = this.barManager1;
            this.chkTachHDSD.Name = "chkTachHDSD";
            this.chkTachHDSD.Properties.Caption = "";
            this.chkTachHDSD.Size = new System.Drawing.Size(27, 19);
            this.chkTachHDSD.StyleController = this.layoutControl1;
            this.chkTachHDSD.TabIndex = 2;
            this.chkTachHDSD.CheckedChanged += new System.EventHandler(this.chkTachHDSD_CheckedChanged);
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
            this.barButtonItem1});
            this.barManager1.MaxItemId = 1;
            //
            // bar1
            //
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            //
            // barButtonItem1
            //
            this.barButtonItem1.Caption = "Tạo (Ctrl T)";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(966, 0);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 560);
            this.barDockControlBottom.Size = new System.Drawing.Size(966, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 560);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(966, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 560);
            //
            // chkSign
            //
            this.chkSign.Location = new System.Drawing.Point(705, 536);
            this.chkSign.MenuManager = this.barManager1;
            this.chkSign.Name = "chkSign";
            this.chkSign.Properties.Caption = "";
            this.chkSign.Properties.FullFocusRect = true;
            this.chkSign.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkSign.Size = new System.Drawing.Size(26, 19);
            this.chkSign.StyleController = this.layoutControl1;
            this.chkSign.TabIndex = 3;
            this.chkSign.CheckedChanged += new System.EventHandler(this.chkSign_CheckedChanged);
            //
            // chkPrintDocumentSigned
            //
            this.chkPrintDocumentSigned.Location = new System.Drawing.Point(830, 536);
            this.chkPrintDocumentSigned.MenuManager = this.barManager1;
            this.chkPrintDocumentSigned.Name = "chkPrintDocumentSigned";
            this.chkPrintDocumentSigned.Properties.Caption = "";
            this.chkPrintDocumentSigned.Properties.FullFocusRect = true;
            this.chkPrintDocumentSigned.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkPrintDocumentSigned.Size = new System.Drawing.Size(27, 19);
            this.chkPrintDocumentSigned.StyleController = this.layoutControl1;
            this.chkPrintDocumentSigned.TabIndex = 4;
            this.chkPrintDocumentSigned.CheckedChanged += new System.EventHandler(this.chkPrintDocumentSigned_CheckedChanged);
            //
            // btnCreate
            //
            this.btnCreate.Location = new System.Drawing.Point(861, 536);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(103, 22);
            this.btnCreate.StyleController = this.layoutControl1;
            this.btnCreate.TabIndex = 5;
            this.btnCreate.Text = "Tạo (Ctrl T)";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // groupControlPatient
            //
            this.groupControlPatient.Controls.Add(this.gridControlPatient);
            this.groupControlPatient.Location = new System.Drawing.Point(2, 2);
            this.groupControlPatient.Name = "groupControlPatient";
            this.groupControlPatient.Size = new System.Drawing.Size(962, 381);
            this.groupControlPatient.TabIndex = 0;
            this.groupControlPatient.Text = "Danh sách bệnh nhân";
            //
            // gridControlPatient
            //
            this.gridControlPatient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlPatient.Location = new System.Drawing.Point(2, 20);
            this.gridControlPatient.MainView = this.gridViewPatient;
            this.gridControlPatient.MenuManager = this.barManager1;
            this.gridControlPatient.Name = "gridControlPatient";
            this.gridControlPatient.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEditPatient});
            this.gridControlPatient.Size = new System.Drawing.Size(958, 359);
            this.gridControlPatient.TabIndex = 0;
            this.gridControlPatient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPatient});
            //
            // gridViewPatient
            //
            this.gridViewPatient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcCheck,
            this.gcStt,
            this.gcBedRoomName,
            this.gcBedName,
            this.gcTreatmentCode,
            this.gcPatientCode,
            this.gcPatientName,
            this.gcDob,
            this.gcGender});
            this.gridViewPatient.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewPatient.GridControl = this.gridControlPatient;
            this.gridViewPatient.Name = "gridViewPatient";
            this.gridViewPatient.OptionsBehavior.Editable = false;
            this.gridViewPatient.OptionsFind.AlwaysVisible = true;
            this.gridViewPatient.OptionsFind.FindNullPrompt = "Từ khóa tìm kiếm (mã điều trị, mã BN, tên bệnh nhân, giường)";
            this.gridViewPatient.OptionsFind.ShowClearButton = false;
            this.gridViewPatient.OptionsFind.ShowFindButton = false;
            this.gridViewPatient.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewPatient.OptionsView.ShowGroupPanel = false;
            this.gridViewPatient.OptionsView.ShowIndicator = false;
            this.gridViewPatient.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewPatient_CustomDrawColumnHeader);
            this.gridViewPatient.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPatient_CustomUnboundColumnData);
            this.gridViewPatient.ColumnFilterChanged += new System.EventHandler(this.gridViewPatient_ColumnFilterChanged);
            this.gridViewPatient.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridViewPatient_KeyDown);
            this.gridViewPatient.Click += new System.EventHandler(this.gridViewPatient_Click);
            //
            // gcCheck
            //
            this.gcCheck.ColumnEdit = this.repositoryItemCheckEditPatient;
            this.gcCheck.FieldName = "IsChecked";
            this.gcCheck.Name = "gcCheck";
            this.gcCheck.OptionsColumn.AllowEdit = false;
            this.gcCheck.OptionsColumn.AllowSize = false;
            this.gcCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcCheck.OptionsColumn.ShowCaption = false;
            this.gcCheck.OptionsFilter.AllowFilter = false;
            this.gcCheck.Visible = true;
            this.gcCheck.VisibleIndex = 0;
            this.gcCheck.Width = 30;
            //
            // repositoryItemCheckEditPatient
            //
            this.repositoryItemCheckEditPatient.AutoHeight = false;
            this.repositoryItemCheckEditPatient.Name = "repositoryItemCheckEditPatient";
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 1;
            this.gcStt.Width = 40;
            //
            // gcBedRoomName
            //
            this.gcBedRoomName.Caption = "Buồng";
            this.gcBedRoomName.FieldName = "BED_ROOM_NAME";
            this.gcBedRoomName.Name = "gcBedRoomName";
            this.gcBedRoomName.OptionsColumn.AllowEdit = false;
            this.gcBedRoomName.Visible = true;
            this.gcBedRoomName.VisibleIndex = 2;
            this.gcBedRoomName.Width = 130;
            //
            // gcBedName
            //
            this.gcBedName.Caption = "Giường";
            this.gcBedName.FieldName = "BED_NAME";
            this.gcBedName.Name = "gcBedName";
            this.gcBedName.OptionsColumn.AllowEdit = false;
            this.gcBedName.Visible = true;
            this.gcBedName.VisibleIndex = 3;
            this.gcBedName.Width = 80;
            //
            // gcTreatmentCode
            //
            this.gcTreatmentCode.Caption = "Mã điều trị";
            this.gcTreatmentCode.FieldName = "TREATMENT_CODE";
            this.gcTreatmentCode.Name = "gcTreatmentCode";
            this.gcTreatmentCode.OptionsColumn.AllowEdit = false;
            this.gcTreatmentCode.Visible = true;
            this.gcTreatmentCode.VisibleIndex = 4;
            this.gcTreatmentCode.Width = 110;
            //
            // gcPatientCode
            //
            this.gcPatientCode.Caption = "Mã BN";
            this.gcPatientCode.FieldName = "TDL_PATIENT_CODE";
            this.gcPatientCode.Name = "gcPatientCode";
            this.gcPatientCode.OptionsColumn.AllowEdit = false;
            this.gcPatientCode.Visible = true;
            this.gcPatientCode.VisibleIndex = 5;
            this.gcPatientCode.Width = 100;
            //
            // gcPatientName
            //
            this.gcPatientName.Caption = "Tên bệnh nhân";
            this.gcPatientName.FieldName = "TDL_PATIENT_NAME";
            this.gcPatientName.Name = "gcPatientName";
            this.gcPatientName.OptionsColumn.AllowEdit = false;
            this.gcPatientName.Visible = true;
            this.gcPatientName.VisibleIndex = 6;
            this.gcPatientName.Width = 220;
            //
            // gcDob
            //
            this.gcDob.Caption = "Ngày sinh";
            this.gcDob.FieldName = "DOB_STR";
            this.gcDob.Name = "gcDob";
            this.gcDob.OptionsColumn.AllowEdit = false;
            this.gcDob.Visible = true;
            this.gcDob.VisibleIndex = 7;
            this.gcDob.Width = 90;
            //
            // gcGender
            //
            this.gcGender.Caption = "Giới tính";
            this.gcGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gcGender.Name = "gcGender";
            this.gcGender.OptionsColumn.AllowEdit = false;
            this.gcGender.Visible = true;
            this.gcGender.VisibleIndex = 8;
            this.gcGender.Width = 70;
            //
            // groupControlFilterCondition
            //
            this.groupControlFilterCondition.Controls.Add(this.layoutControl2);
            this.groupControlFilterCondition.Location = new System.Drawing.Point(2, 387);
            this.groupControlFilterCondition.Name = "groupControlFilterCondition";
            this.groupControlFilterCondition.Size = new System.Drawing.Size(962, 145);
            this.groupControlFilterCondition.TabIndex = 1;
            this.groupControlFilterCondition.Text = "Điều kiện lọc";
            //
            // layoutControl2
            //
            this.layoutControl2.Controls.Add(this.cboRoom);
            this.layoutControl2.Controls.Add(this.chkSortEstablish);
            this.layoutControl2.Controls.Add(this.chkSortName);
            this.layoutControl2.Controls.Add(this.checkedCboHisPatientType);
            this.layoutControl2.Controls.Add(this.chkIsExpend);
            this.layoutControl2.Controls.Add(this.cboDepartment);
            this.layoutControl2.Controls.Add(this.chkChemical);
            this.layoutControl2.Controls.Add(this.chkBlood);
            this.layoutControl2.Controls.Add(this.dtToTime);
            this.layoutControl2.Controls.Add(this.dtFromTime);
            this.layoutControl2.Controls.Add(this.chkMaterial);
            this.layoutControl2.Controls.Add(this.chkMedicine);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(2, 20);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(958, 123);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            //
            // cboRoom
            //
            this.cboRoom.EditValue = "";
            this.cboRoom.Location = new System.Drawing.Point(424, 26);
            this.cboRoom.MenuManager = this.barManager1;
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridView2;
            this.cboRoom.Size = new System.Drawing.Size(234, 20);
            this.cboRoom.StyleController = this.layoutControl2;
            this.cboRoom.TabIndex = 7;
            this.cboRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
            this.cboRoom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboRoom_ButtonClick);
            //
            // gridView2
            //
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            //
            // chkSortEstablish
            //
            this.chkSortEstablish.Location = new System.Drawing.Point(97, 100);
            this.chkSortEstablish.MenuManager = this.barManager1;
            this.chkSortEstablish.Name = "chkSortEstablish";
            this.chkSortEstablish.Properties.Caption = "Theo thiết lập";
            this.chkSortEstablish.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkSortEstablish.Properties.RadioGroupIndex = 1;
            this.chkSortEstablish.Size = new System.Drawing.Size(859, 19);
            this.chkSortEstablish.StyleController = this.layoutControl2;
            this.chkSortEstablish.TabIndex = 13;
            this.chkSortEstablish.TabStop = false;
            this.chkSortEstablish.CheckedChanged += new System.EventHandler(this.chkSortEstablish_CheckedChanged);
            //
            // chkSortName
            //
            this.chkSortName.EditValue = true;
            this.chkSortName.Location = new System.Drawing.Point(97, 76);
            this.chkSortName.MenuManager = this.barManager1;
            this.chkSortName.Name = "chkSortName";
            this.chkSortName.Properties.Caption = "Sắp xếp theo tên";
            this.chkSortName.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkSortName.Properties.RadioGroupIndex = 1;
            this.chkSortName.Size = new System.Drawing.Size(859, 19);
            this.chkSortName.StyleController = this.layoutControl2;
            this.chkSortName.TabIndex = 12;
            this.chkSortName.TabStop = false;
            //
            // checkedCboHisPatientType
            //
            this.checkedCboHisPatientType.EditValue = "";
            this.checkedCboHisPatientType.Location = new System.Drawing.Point(705, 26);
            this.checkedCboHisPatientType.MenuManager = this.barManager1;
            this.checkedCboHisPatientType.Name = "checkedCboHisPatientType";
            this.checkedCboHisPatientType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.checkedCboHisPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.checkedCboHisPatientType.Properties.NullText = "";
            this.checkedCboHisPatientType.Properties.View = this.gridView1;
            this.checkedCboHisPatientType.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.checkedCboHisPatientType_Properties_ButtonClick);
            this.checkedCboHisPatientType.Size = new System.Drawing.Size(155, 20);
            this.checkedCboHisPatientType.StyleController = this.layoutControl2;
            this.checkedCboHisPatientType.TabIndex = 8;
            this.checkedCboHisPatientType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.checkedCboHisPatientType_CustomDisplayText);
            //
            // gridView1
            //
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            //
            // chkIsExpend
            //
            this.chkIsExpend.Location = new System.Drawing.Point(919, 26);
            this.chkIsExpend.MenuManager = this.barManager1;
            this.chkIsExpend.Name = "chkIsExpend";
            this.chkIsExpend.Properties.Caption = "";
            this.chkIsExpend.Size = new System.Drawing.Size(37, 19);
            this.chkIsExpend.StyleController = this.layoutControl2;
            this.chkIsExpend.TabIndex = 9;
            //
            // cboDepartment
            //
            this.cboDepartment.Location = new System.Drawing.Point(97, 26);
            this.cboDepartment.MenuManager = this.barManager1;
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridLookUpEdit1View;
            this.cboDepartment.Size = new System.Drawing.Size(228, 20);
            this.cboDepartment.StyleController = this.layoutControl2;
            this.cboDepartment.TabIndex = 6;
            this.cboDepartment.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDepartment_Closed);
            this.cboDepartment.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDepartment_ButtonClick);
            //
            // gridLookUpEdit1View
            //
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            //
            // chkChemical
            //
            this.chkChemical.EditValue = true;
            this.chkChemical.Location = new System.Drawing.Point(600, 2);
            this.chkChemical.MenuManager = this.barManager1;
            this.chkChemical.Name = "chkChemical";
            this.chkChemical.Properties.Caption = "";
            this.chkChemical.Size = new System.Drawing.Size(100, 19);
            this.chkChemical.StyleController = this.layoutControl2;
            this.chkChemical.TabIndex = 5;
            //
            // chkBlood
            //
            this.chkBlood.EditValue = true;
            this.chkBlood.Location = new System.Drawing.Point(424, 2);
            this.chkBlood.MenuManager = this.barManager1;
            this.chkBlood.Name = "chkBlood";
            this.chkBlood.Properties.Caption = "";
            this.chkBlood.Size = new System.Drawing.Size(77, 19);
            this.chkBlood.StyleController = this.layoutControl2;
            this.chkBlood.TabIndex = 4;
            //
            // dtToTime
            //
            this.dtToTime.EditValue = null;
            this.dtToTime.Location = new System.Drawing.Point(424, 50);
            this.dtToTime.MenuManager = this.barManager1;
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtToTime.Size = new System.Drawing.Size(234, 20);
            this.dtToTime.StyleController = this.layoutControl2;
            this.dtToTime.TabIndex = 11;
            //
            // dtFromTime
            //
            this.dtFromTime.EditValue = null;
            this.dtFromTime.Location = new System.Drawing.Point(97, 50);
            this.dtFromTime.MenuManager = this.barManager1;
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtFromTime.Size = new System.Drawing.Size(228, 20);
            this.dtFromTime.StyleController = this.layoutControl2;
            this.dtFromTime.TabIndex = 10;
            //
            // chkMaterial
            //
            this.chkMaterial.EditValue = true;
            this.chkMaterial.Location = new System.Drawing.Point(259, 2);
            this.chkMaterial.MenuManager = this.barManager1;
            this.chkMaterial.Name = "chkMaterial";
            this.chkMaterial.Properties.Caption = "";
            this.chkMaterial.Size = new System.Drawing.Size(66, 19);
            this.chkMaterial.StyleController = this.layoutControl2;
            this.chkMaterial.TabIndex = 3;
            //
            // chkMedicine
            //
            this.chkMedicine.EditValue = true;
            this.chkMedicine.Location = new System.Drawing.Point(97, 2);
            this.chkMedicine.MenuManager = this.barManager1;
            this.chkMedicine.Name = "chkMedicine";
            this.chkMedicine.Properties.Caption = "";
            this.chkMedicine.Size = new System.Drawing.Size(63, 19);
            this.chkMedicine.StyleController = this.layoutControl2;
            this.chkMedicine.TabIndex = 2;
            //
            // layoutControlGroup2
            //
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMedicine,
            this.lciMaterial,
            this.lciBlood,
            this.lciChemical,
            this.emptySpaceItem3,
            this.lciDepartment,
            this.lciRoom,
            this.lciPatientType,
            this.lciIsExpend,
            this.lciFromTime,
            this.lciToTime,
            this.emptySpaceItem2,
            this.lciSortName,
            this.lciSortEstablish});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "Root";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(958, 123);
            this.layoutControlGroup2.TextVisible = false;
            //
            // lciMedicine
            //
            this.lciMedicine.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMedicine.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMedicine.Control = this.chkMedicine;
            this.lciMedicine.Location = new System.Drawing.Point(0, 0);
            this.lciMedicine.Name = "lciMedicine";
            this.lciMedicine.Size = new System.Drawing.Size(162, 24);
            this.lciMedicine.Text = "Thuốc";
            this.lciMedicine.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMedicine.TextSize = new System.Drawing.Size(90, 20);
            this.lciMedicine.TextToControlDistance = 5;
            //
            // lciMaterial
            //
            this.lciMaterial.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMaterial.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMaterial.Control = this.chkMaterial;
            this.lciMaterial.Location = new System.Drawing.Point(162, 0);
            this.lciMaterial.Name = "lciMaterial";
            this.lciMaterial.Size = new System.Drawing.Size(165, 24);
            this.lciMaterial.Text = "Vật tư";
            this.lciMaterial.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMaterial.TextSize = new System.Drawing.Size(90, 20);
            this.lciMaterial.TextToControlDistance = 5;
            //
            // lciBlood
            //
            this.lciBlood.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBlood.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBlood.Control = this.chkBlood;
            this.lciBlood.Location = new System.Drawing.Point(327, 0);
            this.lciBlood.Name = "lciBlood";
            this.lciBlood.Size = new System.Drawing.Size(176, 24);
            this.lciBlood.Text = "Máu";
            this.lciBlood.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBlood.TextSize = new System.Drawing.Size(90, 20);
            this.lciBlood.TextToControlDistance = 5;
            //
            // lciChemical
            //
            this.lciChemical.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChemical.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChemical.Control = this.chkChemical;
            this.lciChemical.Location = new System.Drawing.Point(503, 0);
            this.lciChemical.Name = "lciChemical";
            this.lciChemical.Size = new System.Drawing.Size(199, 24);
            this.lciChemical.Text = "Hóa chất";
            this.lciChemical.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChemical.TextSize = new System.Drawing.Size(90, 20);
            this.lciChemical.TextToControlDistance = 5;
            //
            // emptySpaceItem3
            //
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(702, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(256, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciDepartment
            //
            this.lciDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDepartment.Control = this.cboDepartment;
            this.lciDepartment.Location = new System.Drawing.Point(0, 24);
            this.lciDepartment.Name = "lciDepartment";
            this.lciDepartment.Size = new System.Drawing.Size(327, 24);
            this.lciDepartment.Text = "Khoa yêu cầu:";
            this.lciDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDepartment.TextSize = new System.Drawing.Size(90, 20);
            this.lciDepartment.TextToControlDistance = 5;
            //
            // lciRoom
            //
            this.lciRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoom.Control = this.cboRoom;
            this.lciRoom.Location = new System.Drawing.Point(327, 24);
            this.lciRoom.Name = "lciRoom";
            this.lciRoom.Size = new System.Drawing.Size(333, 24);
            this.lciRoom.Text = "Phòng yêu cầu:";
            this.lciRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciRoom.TextToControlDistance = 5;
            //
            // lciPatientType
            //
            this.lciPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientType.Control = this.checkedCboHisPatientType;
            this.lciPatientType.Location = new System.Drawing.Point(660, 24);
            this.lciPatientType.Name = "lciPatientType";
            this.lciPatientType.OptionsToolTip.ToolTip = "Đối tượng thanh toán";
            this.lciPatientType.Size = new System.Drawing.Size(202, 24);
            this.lciPatientType.Text = "ĐTTT:";
            this.lciPatientType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientType.TextSize = new System.Drawing.Size(38, 20);
            this.lciPatientType.TextToControlDistance = 5;
            //
            // lciIsExpend
            //
            this.lciIsExpend.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsExpend.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsExpend.Control = this.chkIsExpend;
            this.lciIsExpend.Location = new System.Drawing.Point(862, 24);
            this.lciIsExpend.Name = "lciIsExpend";
            this.lciIsExpend.Size = new System.Drawing.Size(96, 24);
            this.lciIsExpend.Text = "Hao phí";
            this.lciIsExpend.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsExpend.TextSize = new System.Drawing.Size(50, 20);
            this.lciIsExpend.TextToControlDistance = 5;
            //
            // lciFromTime
            //
            this.lciFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFromTime.Control = this.dtFromTime;
            this.lciFromTime.Location = new System.Drawing.Point(0, 48);
            this.lciFromTime.Name = "lciFromTime";
            this.lciFromTime.Size = new System.Drawing.Size(327, 26);
            this.lciFromTime.Text = "Tạo từ:";
            this.lciFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFromTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciFromTime.TextToControlDistance = 5;
            //
            // lciToTime
            //
            this.lciToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciToTime.Control = this.dtToTime;
            this.lciToTime.Location = new System.Drawing.Point(327, 48);
            this.lciToTime.Name = "lciToTime";
            this.lciToTime.Size = new System.Drawing.Size(333, 26);
            this.lciToTime.Text = "Tạo đến:";
            this.lciToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciToTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciToTime.TextToControlDistance = 5;
            //
            // emptySpaceItem2
            //
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(660, 48);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(298, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            //
            // lciSortName
            //
            this.lciSortName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSortName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSortName.Control = this.chkSortName;
            this.lciSortName.Location = new System.Drawing.Point(0, 74);
            this.lciSortName.Name = "lciSortName";
            this.lciSortName.Size = new System.Drawing.Size(958, 24);
            this.lciSortName.Text = "Sắp xếp:";
            this.lciSortName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSortName.TextSize = new System.Drawing.Size(90, 20);
            this.lciSortName.TextToControlDistance = 5;
            //
            // lciSortEstablish
            //
            this.lciSortEstablish.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSortEstablish.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSortEstablish.Control = this.chkSortEstablish;
            this.lciSortEstablish.Location = new System.Drawing.Point(0, 98);
            this.lciSortEstablish.Name = "lciSortEstablish";
            this.lciSortEstablish.Size = new System.Drawing.Size(958, 25);
            this.lciSortEstablish.Text = " ";
            this.lciSortEstablish.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSortEstablish.TextSize = new System.Drawing.Size(90, 20);
            this.lciSortEstablish.TextToControlDistance = 5;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciGroupPatient,
            this.lciGroupFilter,
            this.lciSelectedCount,
            this.lciTachHDSD,
            this.lciSign,
            this.lciPrintDocumentSigned,
            this.lciCreate});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(966, 560);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciGroupPatient
            //
            this.lciGroupPatient.Control = this.groupControlPatient;
            this.lciGroupPatient.Location = new System.Drawing.Point(0, 0);
            this.lciGroupPatient.Name = "lciGroupPatient";
            this.lciGroupPatient.Size = new System.Drawing.Size(966, 385);
            this.lciGroupPatient.TextSize = new System.Drawing.Size(0, 0);
            this.lciGroupPatient.TextVisible = false;
            //
            // lciGroupFilter
            //
            this.lciGroupFilter.Control = this.groupControlFilterCondition;
            this.lciGroupFilter.Location = new System.Drawing.Point(0, 385);
            this.lciGroupFilter.MaxSize = new System.Drawing.Size(0, 149);
            this.lciGroupFilter.MinSize = new System.Drawing.Size(104, 149);
            this.lciGroupFilter.Name = "lciGroupFilter";
            this.lciGroupFilter.Size = new System.Drawing.Size(966, 149);
            this.lciGroupFilter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciGroupFilter.TextSize = new System.Drawing.Size(0, 0);
            this.lciGroupFilter.TextVisible = false;
            //
            // lciSelectedCount
            //
            this.lciSelectedCount.Control = this.lblSelectedCount;
            this.lciSelectedCount.Location = new System.Drawing.Point(0, 534);
            this.lciSelectedCount.Name = "lciSelectedCount";
            this.lciSelectedCount.Size = new System.Drawing.Size(502, 26);
            this.lciSelectedCount.TextSize = new System.Drawing.Size(0, 0);
            this.lciSelectedCount.TextVisible = false;
            //
            // lciTachHDSD
            //
            this.lciTachHDSD.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTachHDSD.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTachHDSD.Control = this.chkTachHDSD;
            this.lciTachHDSD.Location = new System.Drawing.Point(502, 534);
            this.lciTachHDSD.Name = "lciTachHDSD";
            this.lciTachHDSD.Size = new System.Drawing.Size(126, 26);
            this.lciTachHDSD.Text = "Tách HDSD:";
            this.lciTachHDSD.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTachHDSD.TextSize = new System.Drawing.Size(90, 20);
            this.lciTachHDSD.TextToControlDistance = 5;
            //
            // lciSign
            //
            this.lciSign.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSign.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSign.Control = this.chkSign;
            this.lciSign.Location = new System.Drawing.Point(628, 534);
            this.lciSign.Name = "lciSign";
            this.lciSign.Size = new System.Drawing.Size(105, 26);
            this.lciSign.Text = "Ký:";
            this.lciSign.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSign.TextSize = new System.Drawing.Size(70, 20);
            this.lciSign.TextToControlDistance = 5;
            //
            // lciPrintDocumentSigned
            //
            this.lciPrintDocumentSigned.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrintDocumentSigned.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrintDocumentSigned.Control = this.chkPrintDocumentSigned;
            this.lciPrintDocumentSigned.Location = new System.Drawing.Point(733, 534);
            this.lciPrintDocumentSigned.Name = "lciPrintDocumentSigned";
            this.lciPrintDocumentSigned.Size = new System.Drawing.Size(126, 26);
            this.lciPrintDocumentSigned.Text = "In văn bản ký:";
            this.lciPrintDocumentSigned.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPrintDocumentSigned.TextSize = new System.Drawing.Size(90, 20);
            this.lciPrintDocumentSigned.TextToControlDistance = 5;
            //
            // lciCreate
            //
            this.lciCreate.Control = this.btnCreate;
            this.lciCreate.Location = new System.Drawing.Point(859, 534);
            this.lciCreate.Name = "lciCreate";
            this.lciCreate.Size = new System.Drawing.Size(107, 26);
            this.lciCreate.TextSize = new System.Drawing.Size(0, 0);
            this.lciCreate.TextVisible = false;
            //
            // frmPublicMedicineMultiPatient
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 560);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmPublicMedicineMultiPatient";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Công khai thuốc nhiều BN";
            this.Load += new System.EventHandler(this.frmPublicMedicineMultiPatient_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkTachHDSD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSign.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrintDocumentSigned.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlPatient)).EndInit();
            this.groupControlPatient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEditPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlFilterCondition)).EndInit();
            this.groupControlFilterCondition.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSortEstablish.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSortName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkedCboHisPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsExpend.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkChemical.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBlood.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMaterial.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedicine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChemical)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsExpend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSortName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSortEstablish)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGroupPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGroupFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSelectedCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTachHDSD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrintDocumentSigned)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCreate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblSelectedCount;
        private DevExpress.XtraEditors.CheckEdit chkTachHDSD;
        private DevExpress.XtraEditors.CheckEdit chkSign;
        private DevExpress.XtraEditors.CheckEdit chkPrintDocumentSigned;
        private DevExpress.XtraEditors.SimpleButton btnCreate;
        private DevExpress.XtraEditors.GroupControl groupControlPatient;
        private DevExpress.XtraGrid.GridControl gridControlPatient;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPatient;
        private DevExpress.XtraGrid.Columns.GridColumn gcCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEditPatient;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcBedRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gcBedName;
        private DevExpress.XtraGrid.Columns.GridColumn gcTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gcDob;
        private DevExpress.XtraGrid.Columns.GridColumn gcGender;
        private DevExpress.XtraEditors.GroupControl groupControlFilterCondition;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.CheckEdit chkMedicine;
        private DevExpress.XtraEditors.CheckEdit chkMaterial;
        private DevExpress.XtraEditors.CheckEdit chkBlood;
        private DevExpress.XtraEditors.CheckEdit chkChemical;
        private DevExpress.XtraEditors.CheckEdit chkIsExpend;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.GridLookUpEdit checkedCboHisPatientType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.DateEdit dtFromTime;
        private DevExpress.XtraEditors.DateEdit dtToTime;
        private DevExpress.XtraEditors.CheckEdit chkSortName;
        private DevExpress.XtraEditors.CheckEdit chkSortEstablish;
        private DevExpress.XtraLayout.LayoutControlItem lciMedicine;
        private DevExpress.XtraLayout.LayoutControlItem lciMaterial;
        private DevExpress.XtraLayout.LayoutControlItem lciBlood;
        private DevExpress.XtraLayout.LayoutControlItem lciChemical;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciIsExpend;
        private DevExpress.XtraLayout.LayoutControlItem lciFromTime;
        private DevExpress.XtraLayout.LayoutControlItem lciToTime;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem lciSortName;
        private DevExpress.XtraLayout.LayoutControlItem lciSortEstablish;
        private DevExpress.XtraLayout.LayoutControlItem lciGroupPatient;
        private DevExpress.XtraLayout.LayoutControlItem lciGroupFilter;
        private DevExpress.XtraLayout.LayoutControlItem lciSelectedCount;
        private DevExpress.XtraLayout.LayoutControlItem lciTachHDSD;
        private DevExpress.XtraLayout.LayoutControlItem lciSign;
        private DevExpress.XtraLayout.LayoutControlItem lciPrintDocumentSigned;
        private DevExpress.XtraLayout.LayoutControlItem lciCreate;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
