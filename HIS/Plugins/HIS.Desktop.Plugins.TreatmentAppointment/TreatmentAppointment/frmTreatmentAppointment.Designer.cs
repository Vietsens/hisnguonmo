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
namespace HIS.Desktop.Plugins.TreatmentAppointment
{
    partial class frmTreatmentAppointment
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTreatmentAppointment));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.dtAppointmentTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dtAppointmentTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.spnAppointmentDay = new DevExpress.XtraEditors.SpinEdit();
            this.cboEndDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboAppointmentTimeOption = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkIsAppointmentReminded = new DevExpress.XtraEditors.CheckEdit();
            this.chkNotAppointmentAttended = new DevExpress.XtraEditors.CheckEdit();
            this.chkNotAppointmentReminded = new DevExpress.XtraEditors.CheckEdit();
            this.chkIsAppointmentAttended = new DevExpress.XtraEditors.CheckEdit();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.gridControlTreatmentAppointment = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatmentAppointment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnRemind = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEditStatus = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.gridColumnStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPhone = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAppointDay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnInTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnIcdName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnAppointmentRemind = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.btnCancelAppointmentRemind = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDay = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciEmptySpace = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAppointmentTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAppointmentTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAppointmentDay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAppointmentTimeOption.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsAppointmentReminded.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAppointmentAttended.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAppointmentReminded.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsAppointmentAttended.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentAppointment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentAppointment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEditStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAppointmentRemind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelAppointmentRemind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEmptySpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTimeTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.dtAppointmentTimeTo);
            this.layoutControl2.Controls.Add(this.dtAppointmentTimeFrom);
            this.layoutControl2.Controls.Add(this.txtPatientCode);
            this.layoutControl2.Controls.Add(this.txtTreatmentCode);
            this.layoutControl2.Controls.Add(this.spnAppointmentDay);
            this.layoutControl2.Controls.Add(this.cboEndDepartment);
            this.layoutControl2.Controls.Add(this.cboAppointmentTimeOption);
            this.layoutControl2.Controls.Add(this.chkIsAppointmentReminded);
            this.layoutControl2.Controls.Add(this.chkNotAppointmentAttended);
            this.layoutControl2.Controls.Add(this.chkNotAppointmentReminded);
            this.layoutControl2.Controls.Add(this.chkIsAppointmentAttended);
            this.layoutControl2.Controls.Add(this.ucPaging);
            this.layoutControl2.Controls.Add(this.btnSearch);
            this.layoutControl2.Controls.Add(this.txtSearch);
            this.layoutControl2.Controls.Add(this.gridControlTreatmentAppointment);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 29);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(1281, 580);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl1";
            // 
            // dtAppointmentTimeTo
            // 
            this.dtAppointmentTimeTo.EditValue = null;
            this.dtAppointmentTimeTo.Location = new System.Drawing.Point(1169, 7);
            this.dtAppointmentTimeTo.MenuManager = this.barManager1;
            this.dtAppointmentTimeTo.Name = "dtAppointmentTimeTo";
            this.dtAppointmentTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtAppointmentTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtAppointmentTimeTo.Size = new System.Drawing.Size(93, 20);
            this.dtAppointmentTimeTo.StyleController = this.layoutControl2;
            this.dtAppointmentTimeTo.TabIndex = 14;
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
            this.bbtnSearch});
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSearch)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSearch
            // 
            this.bbtnSearch.Caption = "Tìm kiếm (Ctrl F)";
            this.bbtnSearch.Id = 3;
            this.bbtnSearch.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.bbtnSearch.Name = "bbtnSearch";
            this.bbtnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSearch_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1281, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 609);
            this.barDockControlBottom.Size = new System.Drawing.Size(1281, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 580);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1281, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 580);
            // 
            // dtAppointmentTimeFrom
            // 
            this.dtAppointmentTimeFrom.EditValue = null;
            this.dtAppointmentTimeFrom.Location = new System.Drawing.Point(1022, 7);
            this.dtAppointmentTimeFrom.MenuManager = this.barManager1;
            this.dtAppointmentTimeFrom.Name = "dtAppointmentTimeFrom";
            this.dtAppointmentTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtAppointmentTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtAppointmentTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtAppointmentTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtAppointmentTimeFrom.Size = new System.Drawing.Size(116, 20);
            this.dtAppointmentTimeFrom.StyleController = this.layoutControl2;
            this.dtAppointmentTimeFrom.TabIndex = 13;
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(120, 31);
            this.txtPatientCode.MenuManager = this.barManager1;
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Size = new System.Drawing.Size(109, 20);
            this.txtPatientCode.StyleController = this.layoutControl2;
            this.txtPatientCode.TabIndex = 12;
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(7, 31);
            this.txtTreatmentCode.MenuManager = this.barManager1;
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã hẹn khám";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(109, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl2;
            this.txtTreatmentCode.TabIndex = 11;
            // 
            // spnAppointmentDay
            // 
            this.spnAppointmentDay.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spnAppointmentDay.Location = new System.Drawing.Point(929, 7);
            this.spnAppointmentDay.MenuManager = this.barManager1;
            this.spnAppointmentDay.Name = "spnAppointmentDay";
            this.spnAppointmentDay.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.spnAppointmentDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.spnAppointmentDay.Size = new System.Drawing.Size(36, 20);
            this.spnAppointmentDay.StyleController = this.layoutControl2;
            this.spnAppointmentDay.TabIndex = 7;
            this.spnAppointmentDay.EditValueChanged += new System.EventHandler(this.spnAppointmentDay_EditValueChanged);
            this.spnAppointmentDay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.spnAppointmentDay_KeyPress);
            // 
            // cboEndDepartment
            // 
            this.cboEndDepartment.Location = new System.Drawing.Point(575, 7);
            this.cboEndDepartment.MenuManager = this.barManager1;
            this.cboEndDepartment.Name = "cboEndDepartment";
            this.cboEndDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboEndDepartment.Properties.NullText = "";
            this.cboEndDepartment.Properties.View = this.gridLookUpEdit2View;
            this.cboEndDepartment.Size = new System.Drawing.Size(121, 20);
            this.cboEndDepartment.StyleController = this.layoutControl2;
            this.cboEndDepartment.TabIndex = 5;
            this.cboEndDepartment.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboEndDepartment_CustomDisplayText);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboAppointmentTimeOption
            // 
            this.cboAppointmentTimeOption.Location = new System.Drawing.Point(714, 7);
            this.cboAppointmentTimeOption.MenuManager = this.barManager1;
            this.cboAppointmentTimeOption.Name = "cboAppointmentTimeOption";
            this.cboAppointmentTimeOption.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboAppointmentTimeOption.Properties.NullText = "";
            this.cboAppointmentTimeOption.Properties.View = this.gridLookUpEdit1View;
            this.cboAppointmentTimeOption.Size = new System.Drawing.Size(211, 20);
            this.cboAppointmentTimeOption.StyleController = this.layoutControl2;
            this.cboAppointmentTimeOption.TabIndex = 6;
            this.cboAppointmentTimeOption.EditValueChanged += new System.EventHandler(this.cboAppointmentTimeOption_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // chkIsAppointmentReminded
            // 
            this.chkIsAppointmentReminded.Location = new System.Drawing.Point(457, 7);
            this.chkIsAppointmentReminded.MenuManager = this.barManager1;
            this.chkIsAppointmentReminded.Name = "chkIsAppointmentReminded";
            this.chkIsAppointmentReminded.Properties.Caption = "";
            this.chkIsAppointmentReminded.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkIsAppointmentReminded.Properties.RadioGroupIndex = 2;
            this.chkIsAppointmentReminded.Size = new System.Drawing.Size(24, 19);
            this.chkIsAppointmentReminded.StyleController = this.layoutControl2;
            this.chkIsAppointmentReminded.TabIndex = 4;
            this.chkIsAppointmentReminded.TabStop = false;
            // 
            // chkNotAppointmentAttended
            // 
            this.chkNotAppointmentAttended.Location = new System.Drawing.Point(210, 7);
            this.chkNotAppointmentAttended.MenuManager = this.barManager1;
            this.chkNotAppointmentAttended.Name = "chkNotAppointmentAttended";
            this.chkNotAppointmentAttended.Properties.Caption = "";
            this.chkNotAppointmentAttended.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkNotAppointmentAttended.Properties.RadioGroupIndex = 1;
            this.chkNotAppointmentAttended.Size = new System.Drawing.Size(19, 19);
            this.chkNotAppointmentAttended.StyleController = this.layoutControl2;
            this.chkNotAppointmentAttended.TabIndex = 2;
            this.chkNotAppointmentAttended.TabStop = false;
            // 
            // chkNotAppointmentReminded
            // 
            this.chkNotAppointmentReminded.Location = new System.Drawing.Point(349, 7);
            this.chkNotAppointmentReminded.MenuManager = this.barManager1;
            this.chkNotAppointmentReminded.Name = "chkNotAppointmentReminded";
            this.chkNotAppointmentReminded.Properties.Caption = "";
            this.chkNotAppointmentReminded.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkNotAppointmentReminded.Properties.RadioGroupIndex = 2;
            this.chkNotAppointmentReminded.Size = new System.Drawing.Size(19, 19);
            this.chkNotAppointmentReminded.StyleController = this.layoutControl2;
            this.chkNotAppointmentReminded.TabIndex = 3;
            this.chkNotAppointmentReminded.TabStop = false;
            // 
            // chkIsAppointmentAttended
            // 
            this.chkIsAppointmentAttended.Location = new System.Drawing.Point(97, 7);
            this.chkIsAppointmentAttended.MenuManager = this.barManager1;
            this.chkIsAppointmentAttended.Name = "chkIsAppointmentAttended";
            this.chkIsAppointmentAttended.Properties.Caption = "";
            this.chkIsAppointmentAttended.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkIsAppointmentAttended.Properties.RadioGroupIndex = 1;
            this.chkIsAppointmentAttended.Size = new System.Drawing.Size(19, 19);
            this.chkIsAppointmentAttended.StyleController = this.layoutControl2;
            this.chkIsAppointmentAttended.TabIndex = 1;
            this.chkIsAppointmentAttended.TabStop = false;
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(7, 553);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1267, 20);
            this.ucPaging.TabIndex = 7;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(580, 31);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(116, 22);
            this.btnSearch.StyleController = this.layoutControl2;
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(233, 31);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Size = new System.Drawing.Size(343, 20);
            this.txtSearch.StyleController = this.layoutControl2;
            this.txtSearch.TabIndex = 8;
            this.txtSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyUp);
            // 
            // gridControlTreatmentAppointment
            // 
            this.gridControlTreatmentAppointment.Location = new System.Drawing.Point(7, 57);
            this.gridControlTreatmentAppointment.MainView = this.gridViewTreatmentAppointment;
            this.gridControlTreatmentAppointment.Name = "gridControlTreatmentAppointment";
            this.gridControlTreatmentAppointment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.btnAppointmentRemind,
            this.btnCancelAppointmentRemind,
            this.repositoryItemPictureEditStatus});
            this.gridControlTreatmentAppointment.Size = new System.Drawing.Size(1267, 492);
            this.gridControlTreatmentAppointment.TabIndex = 10;
            this.gridControlTreatmentAppointment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatmentAppointment});
            // 
            // gridViewTreatmentAppointment
            // 
            this.gridViewTreatmentAppointment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTT,
            this.gridColumnRemind,
            this.gridColumn3,
            this.gridColumnStatus,
            this.gridColumnPatientCode,
            this.gridColumnTreatmentCode,
            this.gridColumnPatientName,
            this.gridColumnGender,
            this.gridColumnDob,
            this.gridColumnAddress,
            this.gridColumnPhone,
            this.gridColumnAppointDay,
            this.gridColumnInTime,
            this.gridColumnIcdName});
            this.gridViewTreatmentAppointment.GridControl = this.gridControlTreatmentAppointment;
            this.gridViewTreatmentAppointment.Name = "gridViewTreatmentAppointment";
            this.gridViewTreatmentAppointment.OptionsView.ColumnAutoWidth = false;
            this.gridViewTreatmentAppointment.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatmentAppointment.OptionsView.ShowIndicator = false;
            this.gridViewTreatmentAppointment.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewTreatmentAppointment_CustomRowCellEdit);
            this.gridViewTreatmentAppointment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatmentAppointment_CustomUnboundColumnData);
            // 
            // gridColumnSTT
            // 
            this.gridColumnSTT.Caption = "STT";
            this.gridColumnSTT.FieldName = "STT";
            this.gridColumnSTT.Name = "gridColumnSTT";
            this.gridColumnSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnSTT.Visible = true;
            this.gridColumnSTT.VisibleIndex = 0;
            this.gridColumnSTT.Width = 40;
            // 
            // gridColumnRemind
            // 
            this.gridColumnRemind.Caption = "Nhắc hẹn";
            this.gridColumnRemind.FieldName = "REMIND";
            this.gridColumnRemind.Name = "gridColumnRemind";
            this.gridColumnRemind.OptionsColumn.ShowCaption = false;
            this.gridColumnRemind.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnRemind.Visible = true;
            this.gridColumnRemind.VisibleIndex = 1;
            this.gridColumnRemind.Width = 30;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Trạng thái";
            this.gridColumn3.ColumnEdit = this.repositoryItemPictureEditStatus;
            this.gridColumn3.FieldName = "STATUS";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.ShowCaption = false;
            this.gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 30;
            // 
            // repositoryItemPictureEditStatus
            // 
            this.repositoryItemPictureEditStatus.Name = "repositoryItemPictureEditStatus";
            // 
            // gridColumnStatus
            // 
            this.gridColumnStatus.Caption = "Trạng thái";
            this.gridColumnStatus.FieldName = "STATUS_STR";
            this.gridColumnStatus.Name = "gridColumnStatus";
            this.gridColumnStatus.OptionsColumn.AllowEdit = false;
            this.gridColumnStatus.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnStatus.Visible = true;
            this.gridColumnStatus.VisibleIndex = 3;
            this.gridColumnStatus.Width = 120;
            // 
            // gridColumnPatientCode
            // 
            this.gridColumnPatientCode.Caption = "Mã bệnh nhân";
            this.gridColumnPatientCode.FieldName = "TDL_PATIENT_CODE";
            this.gridColumnPatientCode.Name = "gridColumnPatientCode";
            this.gridColumnPatientCode.OptionsColumn.AllowEdit = false;
            this.gridColumnPatientCode.Visible = true;
            this.gridColumnPatientCode.VisibleIndex = 4;
            this.gridColumnPatientCode.Width = 120;
            // 
            // gridColumnTreatmentCode
            // 
            this.gridColumnTreatmentCode.Caption = "Mã điều trị";
            this.gridColumnTreatmentCode.FieldName = "TREATMENT_CODE";
            this.gridColumnTreatmentCode.Name = "gridColumnTreatmentCode";
            this.gridColumnTreatmentCode.OptionsColumn.AllowEdit = false;
            this.gridColumnTreatmentCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnTreatmentCode.OptionsFilter.AllowFilter = false;
            this.gridColumnTreatmentCode.Visible = true;
            this.gridColumnTreatmentCode.VisibleIndex = 5;
            this.gridColumnTreatmentCode.Width = 120;
            // 
            // gridColumnPatientName
            // 
            this.gridColumnPatientName.Caption = "Tên bệnh nhân";
            this.gridColumnPatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumnPatientName.Name = "gridColumnPatientName";
            this.gridColumnPatientName.OptionsColumn.AllowEdit = false;
            this.gridColumnPatientName.Visible = true;
            this.gridColumnPatientName.VisibleIndex = 6;
            this.gridColumnPatientName.Width = 150;
            // 
            // gridColumnGender
            // 
            this.gridColumnGender.Caption = "Giới tính";
            this.gridColumnGender.FieldName = "TDL_PATIENT_GENDER_NAME";
            this.gridColumnGender.Name = "gridColumnGender";
            this.gridColumnGender.OptionsColumn.AllowEdit = false;
            this.gridColumnGender.Visible = true;
            this.gridColumnGender.VisibleIndex = 7;
            this.gridColumnGender.Width = 70;
            // 
            // gridColumnDob
            // 
            this.gridColumnDob.Caption = "Ngày sinh";
            this.gridColumnDob.FieldName = "TDL_PATIENT_DOB_STR";
            this.gridColumnDob.Name = "gridColumnDob";
            this.gridColumnDob.OptionsColumn.AllowEdit = false;
            this.gridColumnDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnDob.Visible = true;
            this.gridColumnDob.VisibleIndex = 8;
            this.gridColumnDob.Width = 100;
            // 
            // gridColumnAddress
            // 
            this.gridColumnAddress.Caption = "Địa chỉ";
            this.gridColumnAddress.FieldName = "TDL_PATIENT_ADDRESS";
            this.gridColumnAddress.Name = "gridColumnAddress";
            this.gridColumnAddress.OptionsColumn.AllowEdit = false;
            this.gridColumnAddress.Visible = true;
            this.gridColumnAddress.VisibleIndex = 9;
            this.gridColumnAddress.Width = 200;
            // 
            // gridColumnPhone
            // 
            this.gridColumnPhone.Caption = "Số điện thoại";
            this.gridColumnPhone.FieldName = "PATIENT_PHONE_STR";
            this.gridColumnPhone.Name = "gridColumnPhone";
            this.gridColumnPhone.OptionsColumn.AllowEdit = false;
            this.gridColumnPhone.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnPhone.Visible = true;
            this.gridColumnPhone.VisibleIndex = 10;
            this.gridColumnPhone.Width = 100;
            // 
            // gridColumnAppointDay
            // 
            this.gridColumnAppointDay.Caption = "Ngày hẹn khám";
            this.gridColumnAppointDay.FieldName = "APPOINTMENT_TIME_STR";
            this.gridColumnAppointDay.Name = "gridColumnAppointDay";
            this.gridColumnAppointDay.OptionsColumn.AllowEdit = false;
            this.gridColumnAppointDay.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnAppointDay.Visible = true;
            this.gridColumnAppointDay.VisibleIndex = 11;
            this.gridColumnAppointDay.Width = 120;
            // 
            // gridColumnInTime
            // 
            this.gridColumnInTime.Caption = "Thời gian vào";
            this.gridColumnInTime.FieldName = "IN_TIME_STR";
            this.gridColumnInTime.Name = "gridColumnInTime";
            this.gridColumnInTime.OptionsColumn.AllowEdit = false;
            this.gridColumnInTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnInTime.Visible = true;
            this.gridColumnInTime.VisibleIndex = 12;
            this.gridColumnInTime.Width = 120;
            // 
            // gridColumnIcdName
            // 
            this.gridColumnIcdName.Caption = "Chấn đoán chính";
            this.gridColumnIcdName.FieldName = "ICD_NAME";
            this.gridColumnIcdName.Name = "gridColumnIcdName";
            this.gridColumnIcdName.OptionsColumn.AllowEdit = false;
            this.gridColumnIcdName.Visible = true;
            this.gridColumnIcdName.VisibleIndex = 13;
            this.gridColumnIcdName.Width = 200;
            // 
            // btnAppointmentRemind
            // 
            this.btnAppointmentRemind.AutoHeight = false;
            this.btnAppointmentRemind.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnAppointmentRemind.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Xác nhận gọi nhắc hẹn khám", null, null, true)});
            this.btnAppointmentRemind.Name = "btnAppointmentRemind";
            this.btnAppointmentRemind.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnAppointmentRemind.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnAppointmentRemind_ButtonClick);
            // 
            // btnCancelAppointmentRemind
            // 
            this.btnCancelAppointmentRemind.AutoHeight = false;
            this.btnCancelAppointmentRemind.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnCancelAppointmentRemind.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Hủy xác nhận gọi nhắc hẹn khám", null, null, true)});
            this.btnCancelAppointmentRemind.Name = "btnCancelAppointmentRemind";
            this.btnCancelAppointmentRemind.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.btnCancelAppointmentRemind.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnCancelAppointmentRemind_ButtonClick);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.lciDay,
            this.emptySpaceItem3,
            this.lciEmptySpace,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.lciAppointmentTimeFrom,
            this.lciAppointmentTimeTo,
            this.emptySpaceItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1281, 580);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlTreatmentAppointment;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 50);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1271, 496);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.txtSearch;
            this.layoutControlItem2.Location = new System.Drawing.Point(226, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(347, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(573, 24);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(120, 26);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(120, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(120, 26);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(693, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(578, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.ucPaging;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 546);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1271, 24);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.chkIsAppointmentAttended;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(113, 24);
            this.layoutControlItem5.Text = "Đã tái khám";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem6.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem6.Control = this.chkNotAppointmentReminded;
            this.layoutControlItem6.Location = new System.Drawing.Point(252, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(113, 24);
            this.layoutControlItem6.Text = "Chưa gọi nhắc";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem7.Control = this.chkNotAppointmentAttended;
            this.layoutControlItem7.Location = new System.Drawing.Point(113, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(113, 24);
            this.layoutControlItem7.Text = "Chưa tái khám";
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem8.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem8.Control = this.chkIsAppointmentReminded;
            this.layoutControlItem8.Location = new System.Drawing.Point(365, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(113, 24);
            this.layoutControlItem8.Text = "Đã gọi nhắc";
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(80, 20);
            this.layoutControlItem8.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.cboAppointmentTimeOption;
            this.layoutControlItem9.Location = new System.Drawing.Point(693, 0);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(250, 24);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(68, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(229, 24);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.Text = "      ";
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(9, 13);
            this.layoutControlItem9.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.cboEndDepartment;
            this.layoutControlItem10.Location = new System.Drawing.Point(478, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(215, 24);
            this.layoutControlItem10.Text = "Khoa kết thúc:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // lciDay
            // 
            this.lciDay.Control = this.spnAppointmentDay;
            this.lciDay.Location = new System.Drawing.Point(922, 0);
            this.lciDay.MaxSize = new System.Drawing.Size(40, 24);
            this.lciDay.MinSize = new System.Drawing.Size(40, 24);
            this.lciDay.Name = "lciDay";
            this.lciDay.Size = new System.Drawing.Size(40, 24);
            this.lciDay.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDay.Text = " ";
            this.lciDay.TextSize = new System.Drawing.Size(0, 0);
            this.lciDay.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(226, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(26, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciEmptySpace
            // 
            this.lciEmptySpace.AllowHotTrack = false;
            this.lciEmptySpace.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciEmptySpace.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lciEmptySpace.Location = new System.Drawing.Point(962, 0);
            this.lciEmptySpace.MaxSize = new System.Drawing.Size(80, 0);
            this.lciEmptySpace.MinSize = new System.Drawing.Size(40, 10);
            this.lciEmptySpace.Name = "lciEmptySpace";
            this.lciEmptySpace.Size = new System.Drawing.Size(53, 24);
            this.lciEmptySpace.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciEmptySpace.Text = "ngày tới";
            this.lciEmptySpace.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciEmptySpace.TextSize = new System.Drawing.Size(0, 20);
            this.lciEmptySpace.TextVisible = true;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.txtTreatmentCode;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(113, 26);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.txtPatientCode;
            this.layoutControlItem13.Location = new System.Drawing.Point(113, 24);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(113, 26);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // lciAppointmentTimeFrom
            // 
            this.lciAppointmentTimeFrom.Control = this.dtAppointmentTimeFrom;
            this.lciAppointmentTimeFrom.Location = new System.Drawing.Point(1015, 0);
            this.lciAppointmentTimeFrom.MaxSize = new System.Drawing.Size(120, 24);
            this.lciAppointmentTimeFrom.MinSize = new System.Drawing.Size(120, 24);
            this.lciAppointmentTimeFrom.Name = "lciAppointmentTimeFrom";
            this.lciAppointmentTimeFrom.Size = new System.Drawing.Size(120, 24);
            this.lciAppointmentTimeFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciAppointmentTimeFrom.TextSize = new System.Drawing.Size(0, 0);
            this.lciAppointmentTimeFrom.TextVisible = false;
            this.lciAppointmentTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciAppointmentTimeTo
            // 
            this.lciAppointmentTimeTo.Control = this.dtAppointmentTimeTo;
            this.lciAppointmentTimeTo.Location = new System.Drawing.Point(1135, 0);
            this.lciAppointmentTimeTo.MaxSize = new System.Drawing.Size(150, 24);
            this.lciAppointmentTimeTo.MinSize = new System.Drawing.Size(100, 24);
            this.lciAppointmentTimeTo.Name = "lciAppointmentTimeTo";
            this.lciAppointmentTimeTo.Size = new System.Drawing.Size(124, 24);
            this.lciAppointmentTimeTo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciAppointmentTimeTo.Text = "Đến:";
            this.lciAppointmentTimeTo.TextSize = new System.Drawing.Size(24, 13);
            this.lciAppointmentTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // emptySpaceItem4
            // 
            this.emptySpaceItem4.AllowHotTrack = false;
            this.emptySpaceItem4.Location = new System.Drawing.Point(1259, 0);
            this.emptySpaceItem4.Name = "emptySpaceItem4";
            this.emptySpaceItem4.Size = new System.Drawing.Size(12, 24);
            this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "04.png");
            this.imageListStatus.Images.SetKeyName(1, "05.png");
            // 
            // frmTreatmentAppointment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 609);
            this.Controls.Add(this.layoutControl2);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTreatmentAppointment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách bệnh nhân hẹn khám";
            this.Load += new System.EventHandler(this.frmTreatmentAppointment_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spnAppointmentDay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEndDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAppointmentTimeOption.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsAppointmentReminded.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAppointmentAttended.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAppointmentReminded.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsAppointmentAttended.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentAppointment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentAppointment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEditStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnAppointmentRemind)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancelAppointmentRemind)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciEmptySpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAppointmentTimeTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraGrid.GridControl gridControlTreatmentAppointment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatmentAppointment;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSearch;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnRemind;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnStatus;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAppointDay;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnInTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnIcdName;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnAppointmentRemind;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnCancelAppointmentRemind;
        private DevExpress.XtraEditors.SpinEdit spnAppointmentDay;
        private DevExpress.XtraEditors.GridLookUpEdit cboEndDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.GridLookUpEdit cboAppointmentTimeOption;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.CheckEdit chkIsAppointmentReminded;
        private DevExpress.XtraEditors.CheckEdit chkNotAppointmentAttended;
        private DevExpress.XtraEditors.CheckEdit chkNotAppointmentReminded;
        private DevExpress.XtraEditors.CheckEdit chkIsAppointmentAttended;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem lciDay;
        private DevExpress.XtraLayout.EmptySpaceItem lciEmptySpace;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnGender;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDob;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAddress;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPhone;
        private System.Windows.Forms.ImageList imageListStatus;
        private DevExpress.XtraEditors.DateEdit dtAppointmentTimeTo;
        private DevExpress.XtraEditors.DateEdit dtAppointmentTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciAppointmentTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciAppointmentTimeTo;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEditStatus;
    }
}
