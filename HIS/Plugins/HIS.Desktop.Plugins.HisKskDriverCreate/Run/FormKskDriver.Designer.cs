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
using DevExpress.XtraBars;
using System;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.Run
{
    partial class FormKskDriver
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormKskDriver));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtPatietnCode = new DevExpress.XtraEditors.TextEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnF3 = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnF4 = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnCtrlF = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnCtrlS = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnCtrlR = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnF2 = new DevExpress.XtraBars.BarButtonItem();
            this.btnReset = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlKskDriver = new DevExpress.XtraGrid.GridControl();
            this.gridViewKskDriver = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEditCopy = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkSignFileCertUtil = new DevExpress.XtraEditors.CheckEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.chkAutoPush = new DevExpress.XtraEditors.CheckEdit();
            this.groupKsk = new System.Windows.Forms.GroupBox();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.dtAppointmentTime = new DevExpress.XtraEditors.DateEdit();
            this.chkPositive = new DevExpress.XtraEditors.CheckEdit();
            this.chkNegative = new DevExpress.XtraEditors.CheckEdit();
            this.chkMgKhi = new DevExpress.XtraEditors.CheckEdit();
            this.chkMgMau = new DevExpress.XtraEditors.CheckEdit();
            this.spConcentration = new DevExpress.XtraEditors.SpinEdit();
            this.txtSickCondition = new DevExpress.XtraEditors.TextEdit();
            this.txtReasonBadHeathly = new DevExpress.XtraEditors.TextEdit();
            this.cboConclusionName = new HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit();
            this.gridLookUpEdit3View = new HIS.Desktop.Utilities.Extensions.CustomGridView();
            this.cboLicenesClass = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboConclusion = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKskDriverCode = new DevExpress.XtraEditors.TextEdit();
            this.dtConclusionTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciConclusionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciKskDriverCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclusion = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciLicenesClass = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclusionName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciReasonBadHeathly = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSickCondition = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConcentration = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDrug = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.groupPatient = new System.Windows.Forms.GroupBox();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.dtTimeCccd = new DevExpress.XtraEditors.DateEdit();
            this.txtPlaceCccd = new DevExpress.XtraEditors.TextEdit();
            this.txtCccdCmnd = new DevExpress.XtraEditors.TextEdit();
            this.lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblDob = new DevExpress.XtraEditors.LabelControl();
            this.lblGender = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCccdNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPlaceCccd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTimeCccd = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtServiceReqCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciSearchTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearchServiceReqCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciAutoPush = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatient = new DevExpress.XtraLayout.LayoutControlItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatietnCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKskDriver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKskDriver)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditCopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignFileCertUtil.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoPush.Properties)).BeginInit();
            this.groupKsk.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPositive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNegative.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMgKhi.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMgMau.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spConcentration.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSickCondition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonBadHeathly.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConclusionName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLicenesClass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConclusion.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKskDriverCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskDriverCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLicenesClass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReasonBadHeathly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSickCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConcentration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDrug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.groupPatient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeCccd.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeCccd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlaceCccd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCccdCmnd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCccdNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlaceCccd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeCccd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceReqCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchServiceReqCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAutoPush)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtPatietnCode);
            this.layoutControl1.Controls.Add(this.btnReset);
            this.layoutControl1.Controls.Add(this.gridControlKskDriver);
            this.layoutControl1.Controls.Add(this.chkSignFileCertUtil);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.chkAutoPush);
            this.layoutControl1.Controls.Add(this.groupKsk);
            this.layoutControl1.Controls.Add(this.groupPatient);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.txtServiceReqCode);
            this.layoutControl1.Controls.Add(this.txtTreatmentCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(990, 481);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtPatietnCode
            // 
            this.txtPatietnCode.Location = new System.Drawing.Point(2, 2);
            this.txtPatietnCode.MenuManager = this.barManager1;
            this.txtPatietnCode.Name = "txtPatietnCode";
            this.txtPatietnCode.Properties.NullValuePrompt = "Mã bệnh nhân (F2)";
            this.txtPatietnCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatietnCode.Size = new System.Drawing.Size(133, 20);
            this.txtPatietnCode.StyleController = this.layoutControl1;
            this.txtPatietnCode.TabIndex = 19;
            this.txtPatietnCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatietnCode_PreviewKeyDown);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barBtnF3,
            this.barBtnF4,
            this.barBtnCtrlF,
            this.barBtnCtrlS,
            this.bbtnCtrlR,
            this.barBtnF2});
            this.barManager1.MaxItemId = 7;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnF3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnF4),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnCtrlF),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnCtrlS),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnCtrlR),
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnF2)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnF3
            // 
            this.barBtnF3.Caption = "F3";
            this.barBtnF3.Id = 1;
            this.barBtnF3.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F3);
            this.barBtnF3.Name = "barBtnF3";
            this.barBtnF3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnF3_ItemClick);
            // 
            // barBtnF4
            // 
            this.barBtnF4.Caption = "F4";
            this.barBtnF4.Id = 2;
            this.barBtnF4.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F4);
            this.barBtnF4.Name = "barBtnF4";
            this.barBtnF4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnF4_ItemClick);
            // 
            // barBtnCtrlF
            // 
            this.barBtnCtrlF.Caption = "Ctrl F";
            this.barBtnCtrlF.Id = 3;
            this.barBtnCtrlF.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barBtnCtrlF.Name = "barBtnCtrlF";
            this.barBtnCtrlF.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnCtrlF_ItemClick);
            // 
            // barBtnCtrlS
            // 
            this.barBtnCtrlS.Caption = "Ctrl S";
            this.barBtnCtrlS.Id = 4;
            this.barBtnCtrlS.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnCtrlS.Name = "barBtnCtrlS";
            this.barBtnCtrlS.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnCtrlS_ItemClick);
            // 
            // bbtnCtrlR
            // 
            this.bbtnCtrlR.Caption = "Ctrl R";
            this.bbtnCtrlR.Id = 5;
            this.bbtnCtrlR.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.bbtnCtrlR.Name = "bbtnCtrlR";
            this.bbtnCtrlR.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnCtrlR_ItemClick);
            // 
            // barBtnF2
            // 
            this.barBtnF2.Caption = "F2";
            this.barBtnF2.Id = 6;
            this.barBtnF2.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F2);
            this.barBtnF2.Name = "barBtnF2";
            this.barBtnF2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnF2_ItemClick);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(909, 457);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(79, 22);
            this.btnReset.StyleController = this.layoutControl1;
            this.btnReset.TabIndex = 18;
            this.btnReset.Text = "Làm lại (Ctrl R)";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // gridControlKskDriver
            // 
            this.gridControlKskDriver.Location = new System.Drawing.Point(2, 28);
            this.gridControlKskDriver.MainView = this.gridViewKskDriver;
            this.gridControlKskDriver.MenuManager = this.barManager1;
            this.gridControlKskDriver.Name = "gridControlKskDriver";
            this.gridControlKskDriver.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEditCopy});
            this.gridControlKskDriver.Size = new System.Drawing.Size(986, 130);
            this.gridControlKskDriver.TabIndex = 17;
            this.gridControlKskDriver.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewKskDriver});
            // 
            // gridViewKskDriver
            // 
            this.gridViewKskDriver.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9});
            this.gridViewKskDriver.GridControl = this.gridControlKskDriver;
            this.gridViewKskDriver.Name = "gridViewKskDriver";
            this.gridViewKskDriver.OptionsView.ColumnAutoWidth = false;
            this.gridViewKskDriver.OptionsView.ShowGroupPanel = false;
            this.gridViewKskDriver.OptionsView.ShowIndicator = false;
            this.gridViewKskDriver.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewKskDriver_CustomUnboundColumnData);
            this.gridViewKskDriver.Click += new System.EventHandler(this.gridViewKskDriver_Click);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 40;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Copy";
            this.gridColumn2.ColumnEdit = this.repositoryItemButtonEditCopy;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 40;
            // 
            // repositoryItemButtonEditCopy
            // 
            this.repositoryItemButtonEditCopy.AutoHeight = false;
            this.repositoryItemButtonEditCopy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEditCopy.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.repositoryItemButtonEditCopy.Name = "repositoryItemButtonEditCopy";
            this.repositoryItemButtonEditCopy.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEditCopy.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditCopy_ButtonClick);
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Số hồ sơ";
            this.gridColumn3.FieldName = "KSK_DRIVER_CODE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 200;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Ngày kết luận";
            this.gridColumn4.FieldName = "CONCLUSION_TIME_STR";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 100;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Kết luận";
            this.gridColumn5.FieldName = "CONCLUSION_STR";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 200;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Hạng bằng lái";
            this.gridColumn6.FieldName = "LICENSE_CLASS";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            this.gridColumn6.Width = 100;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Bác sĩ kết luận";
            this.gridColumn7.FieldName = "CONCLUDER_NAME";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            this.gridColumn7.Width = 150;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Nồng độ cồn";
            this.gridColumn8.FieldName = "CONCENTRATION_STR";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            this.gridColumn8.Width = 150;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Ma túy";
            this.gridColumn9.FieldName = "DRUG_TYPE_STR";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            this.gridColumn9.Width = 100;
            // 
            // chkSignFileCertUtil
            // 
            this.chkSignFileCertUtil.Location = new System.Drawing.Point(670, 457);
            this.chkSignFileCertUtil.MenuManager = this.barManager1;
            this.chkSignFileCertUtil.Name = "chkSignFileCertUtil";
            this.chkSignFileCertUtil.Properties.Caption = "";
            this.chkSignFileCertUtil.Size = new System.Drawing.Size(19, 19);
            this.chkSignFileCertUtil.StyleController = this.layoutControl1;
            this.chkSignFileCertUtil.TabIndex = 16;
            this.chkSignFileCertUtil.ToolTip = "Thực hiện ký số sử dụng USB token khi đồng bộ dữ liệu";
            this.chkSignFileCertUtil.CheckedChanged += new System.EventHandler(this.chkSignFileCertUtil_CheckedChanged);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(841, 457);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkAutoPush
            // 
            this.chkAutoPush.Location = new System.Drawing.Point(818, 457);
            this.chkAutoPush.Name = "chkAutoPush";
            this.chkAutoPush.Properties.Caption = "";
            this.chkAutoPush.Size = new System.Drawing.Size(19, 19);
            this.chkAutoPush.StyleController = this.layoutControl1;
            this.chkAutoPush.TabIndex = 14;
            this.chkAutoPush.ToolTip = "Tự động đẩy cổng sau khi lưu thành công";
            this.chkAutoPush.CheckedChanged += new System.EventHandler(this.chkAutoPush_CheckedChanged);
            // 
            // groupKsk
            // 
            this.groupKsk.Controls.Add(this.layoutControl3);
            this.groupKsk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupKsk.Location = new System.Drawing.Point(2, 289);
            this.groupKsk.Name = "groupKsk";
            this.groupKsk.Size = new System.Drawing.Size(986, 164);
            this.groupKsk.TabIndex = 13;
            this.groupKsk.TabStop = false;
            this.groupKsk.Text = "Thông tin khám sức khỏe";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.dtAppointmentTime);
            this.layoutControl3.Controls.Add(this.chkPositive);
            this.layoutControl3.Controls.Add(this.chkNegative);
            this.layoutControl3.Controls.Add(this.chkMgKhi);
            this.layoutControl3.Controls.Add(this.chkMgMau);
            this.layoutControl3.Controls.Add(this.spConcentration);
            this.layoutControl3.Controls.Add(this.txtSickCondition);
            this.layoutControl3.Controls.Add(this.txtReasonBadHeathly);
            this.layoutControl3.Controls.Add(this.cboConclusionName);
            this.layoutControl3.Controls.Add(this.cboLicenesClass);
            this.layoutControl3.Controls.Add(this.cboConclusion);
            this.layoutControl3.Controls.Add(this.txtKskDriverCode);
            this.layoutControl3.Controls.Add(this.dtConclusionTime);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(3, 16);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(980, 145);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // dtAppointmentTime
            // 
            this.dtAppointmentTime.EditValue = null;
            this.dtAppointmentTime.Location = new System.Drawing.Point(137, 50);
            this.dtAppointmentTime.Margin = new System.Windows.Forms.Padding(2);
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
            this.dtAppointmentTime.Size = new System.Drawing.Size(272, 20);
            this.dtAppointmentTime.StyleController = this.layoutControl3;
            this.dtAppointmentTime.TabIndex = 16;
            this.dtAppointmentTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtAppointmentTime_Closed);
            this.dtAppointmentTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtAppointmentTime_PreviewKeyDown);
            // 
            // chkPositive
            // 
            this.chkPositive.EditValue = true;
            this.chkPositive.Location = new System.Drawing.Point(320, 122);
            this.chkPositive.Name = "chkPositive";
            this.chkPositive.Properties.Caption = "Dương tính";
            this.chkPositive.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkPositive.Size = new System.Drawing.Size(658, 19);
            this.chkPositive.StyleController = this.layoutControl3;
            this.chkPositive.TabIndex = 15;
            this.chkPositive.CheckedChanged += new System.EventHandler(this.chkPositive_CheckedChanged);
            this.chkPositive.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chkPositive_KeyDown);
            // 
            // chkNegative
            // 
            this.chkNegative.EnterMoveNextControl = true;
            this.chkNegative.Location = new System.Drawing.Point(107, 122);
            this.chkNegative.Name = "chkNegative";
            this.chkNegative.Properties.Caption = "Âm tính";
            this.chkNegative.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkNegative.Size = new System.Drawing.Size(209, 19);
            this.chkNegative.StyleController = this.layoutControl3;
            this.chkNegative.TabIndex = 14;
            this.chkNegative.TabStop = false;
            this.chkNegative.CheckedChanged += new System.EventHandler(this.chkNegative_CheckedChanged);
            this.chkNegative.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkNegative_PreviewKeyDown);
            // 
            // chkMgKhi
            // 
            this.chkMgKhi.EditValue = true;
            this.chkMgKhi.Location = new System.Drawing.Point(413, 98);
            this.chkMgKhi.Name = "chkMgKhi";
            this.chkMgKhi.Properties.Caption = "mg/1 lít khí thở";
            this.chkMgKhi.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkMgKhi.Size = new System.Drawing.Size(167, 19);
            this.chkMgKhi.StyleController = this.layoutControl3;
            this.chkMgKhi.TabIndex = 13;
            this.chkMgKhi.TabStop = false;
            this.chkMgKhi.CheckedChanged += new System.EventHandler(this.chkMgKhi_CheckedChanged);
            this.chkMgKhi.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMgKhi_PreviewKeyDown);
            // 
            // chkMgMau
            // 
            this.chkMgMau.EnterMoveNextControl = true;
            this.chkMgMau.Location = new System.Drawing.Point(584, 98);
            this.chkMgMau.Name = "chkMgMau";
            this.chkMgMau.Properties.Caption = "mg/100ml máu";
            this.chkMgMau.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkMgMau.Size = new System.Drawing.Size(394, 19);
            this.chkMgMau.StyleController = this.layoutControl3;
            this.chkMgMau.TabIndex = 12;
            this.chkMgMau.TabStop = false;
            this.chkMgMau.CheckedChanged += new System.EventHandler(this.chkMgMau_CheckedChanged);
            this.chkMgMau.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkMgMau_PreviewKeyDown);
            // 
            // spConcentration
            // 
            this.spConcentration.EditValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spConcentration.Location = new System.Drawing.Point(137, 98);
            this.spConcentration.Name = "spConcentration";
            this.spConcentration.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.spConcentration.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.spConcentration.Properties.MaxValue = new decimal(new int[] {
            -1530494977,
            232830,
            0,
            0});
            this.spConcentration.Size = new System.Drawing.Size(272, 20);
            this.spConcentration.StyleController = this.layoutControl3;
            this.spConcentration.TabIndex = 11;
            this.spConcentration.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.spConcentration_ButtonClick);
            this.spConcentration.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spConcentration_PreviewKeyDown);
            // 
            // txtSickCondition
            // 
            this.txtSickCondition.Location = new System.Drawing.Point(137, 74);
            this.txtSickCondition.Name = "txtSickCondition";
            this.txtSickCondition.Size = new System.Drawing.Size(841, 20);
            this.txtSickCondition.StyleController = this.layoutControl3;
            this.txtSickCondition.TabIndex = 10;
            this.txtSickCondition.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSickCondition_PreviewKeyDown);
            // 
            // txtReasonBadHeathly
            // 
            this.txtReasonBadHeathly.Location = new System.Drawing.Point(508, 50);
            this.txtReasonBadHeathly.Name = "txtReasonBadHeathly";
            this.txtReasonBadHeathly.Size = new System.Drawing.Size(470, 20);
            this.txtReasonBadHeathly.StyleController = this.layoutControl3;
            this.txtReasonBadHeathly.TabIndex = 9;
            this.txtReasonBadHeathly.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtReasonBadHeathly_PreviewKeyDown);
            // 
            // cboConclusionName
            // 
            this.cboConclusionName.Location = new System.Drawing.Point(792, 26);
            this.cboConclusionName.Name = "cboConclusionName";
            this.cboConclusionName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboConclusionName.Properties.NullText = "";
            this.cboConclusionName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboConclusionName.Properties.View = this.gridLookUpEdit3View;
            this.cboConclusionName.Size = new System.Drawing.Size(186, 20);
            this.cboConclusionName.StyleController = this.layoutControl3;
            this.cboConclusionName.TabIndex = 8;
            this.cboConclusionName.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboConclusionName_Closed);
            this.cboConclusionName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboConclusionName_PreviewKeyDown);
            // 
            // gridLookUpEdit3View
            // 
            this.gridLookUpEdit3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit3View.Name = "gridLookUpEdit3View";
            this.gridLookUpEdit3View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit3View.OptionsView.ShowGroupPanel = false;
            // 
            // cboLicenesClass
            // 
            this.cboLicenesClass.Location = new System.Drawing.Point(508, 26);
            this.cboLicenesClass.Name = "cboLicenesClass";
            this.cboLicenesClass.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboLicenesClass.Properties.NullText = "";
            this.cboLicenesClass.Properties.View = this.gridLookUpEdit2View;
            this.cboLicenesClass.Size = new System.Drawing.Size(185, 20);
            this.cboLicenesClass.StyleController = this.layoutControl3;
            this.cboLicenesClass.TabIndex = 7;
            this.cboLicenesClass.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboLicenesClass_Closed);
            this.cboLicenesClass.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboLicenesClass_PreviewKeyDown);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboConclusion
            // 
            this.cboConclusion.Location = new System.Drawing.Point(137, 26);
            this.cboConclusion.Name = "cboConclusion";
            this.cboConclusion.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboConclusion.Properties.NullText = "";
            this.cboConclusion.Properties.PopupFormSize = new System.Drawing.Size(500, 0);
            this.cboConclusion.Properties.View = this.gridLookUpEdit1View;
            this.cboConclusion.Size = new System.Drawing.Size(272, 20);
            this.cboConclusion.StyleController = this.layoutControl3;
            this.cboConclusion.TabIndex = 6;
            this.cboConclusion.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboConclusion_Closed);
            this.cboConclusion.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboConclusion_PreviewKeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtKskDriverCode
            // 
            this.txtKskDriverCode.Enabled = false;
            this.txtKskDriverCode.Location = new System.Drawing.Point(508, 2);
            this.txtKskDriverCode.Name = "txtKskDriverCode";
            this.txtKskDriverCode.Size = new System.Drawing.Size(185, 20);
            this.txtKskDriverCode.StyleController = this.layoutControl3;
            this.txtKskDriverCode.TabIndex = 5;
            // 
            // dtConclusionTime
            // 
            this.dtConclusionTime.EditValue = null;
            this.dtConclusionTime.Location = new System.Drawing.Point(137, 2);
            this.dtConclusionTime.Name = "dtConclusionTime";
            this.dtConclusionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtConclusionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtConclusionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtConclusionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtConclusionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtConclusionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtConclusionTime.Size = new System.Drawing.Size(272, 20);
            this.dtConclusionTime.StyleController = this.layoutControl3;
            this.dtConclusionTime.TabIndex = 4;
            this.dtConclusionTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtConclusionTime_Closed);
            this.dtConclusionTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtConclusionTime_PreviewKeyDown);
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciConclusionTime,
            this.emptySpaceItem3,
            this.lciKskDriverCode,
            this.lciConclusion,
            this.lciLicenesClass,
            this.lciConclusionName,
            this.lciReasonBadHeathly,
            this.lciSickCondition,
            this.lciConcentration,
            this.layoutControlItem12,
            this.layoutControlItem13,
            this.lciDrug,
            this.layoutControlItem15,
            this.layoutControlItem3});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(980, 145);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // lciConclusionTime
            // 
            this.lciConclusionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciConclusionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciConclusionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusionTime.Control = this.dtConclusionTime;
            this.lciConclusionTime.Location = new System.Drawing.Point(0, 0);
            this.lciConclusionTime.Name = "lciConclusionTime";
            this.lciConclusionTime.Size = new System.Drawing.Size(411, 24);
            this.lciConclusionTime.Text = "Ngày kết luận:";
            this.lciConclusionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusionTime.TextSize = new System.Drawing.Size(130, 20);
            this.lciConclusionTime.TextToControlDistance = 5;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(695, 0);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(285, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciKskDriverCode
            // 
            this.lciKskDriverCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciKskDriverCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciKskDriverCode.Control = this.txtKskDriverCode;
            this.lciKskDriverCode.Location = new System.Drawing.Point(411, 0);
            this.lciKskDriverCode.Name = "lciKskDriverCode";
            this.lciKskDriverCode.Size = new System.Drawing.Size(284, 24);
            this.lciKskDriverCode.Text = "Số hồ sơ:";
            this.lciKskDriverCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciKskDriverCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciKskDriverCode.TextToControlDistance = 5;
            // 
            // lciConclusion
            // 
            this.lciConclusion.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciConclusion.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciConclusion.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusion.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusion.Control = this.cboConclusion;
            this.lciConclusion.Location = new System.Drawing.Point(0, 24);
            this.lciConclusion.Name = "lciConclusion";
            this.lciConclusion.Size = new System.Drawing.Size(411, 24);
            this.lciConclusion.Text = "Kết luận:";
            this.lciConclusion.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusion.TextSize = new System.Drawing.Size(130, 20);
            this.lciConclusion.TextToControlDistance = 5;
            // 
            // lciLicenesClass
            // 
            this.lciLicenesClass.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciLicenesClass.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciLicenesClass.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciLicenesClass.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciLicenesClass.Control = this.cboLicenesClass;
            this.lciLicenesClass.Location = new System.Drawing.Point(411, 24);
            this.lciLicenesClass.Name = "lciLicenesClass";
            this.lciLicenesClass.Size = new System.Drawing.Size(284, 24);
            this.lciLicenesClass.Text = "Hạng bằng lái:";
            this.lciLicenesClass.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciLicenesClass.TextSize = new System.Drawing.Size(90, 20);
            this.lciLicenesClass.TextToControlDistance = 5;
            // 
            // lciConclusionName
            // 
            this.lciConclusionName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciConclusionName.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciConclusionName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclusionName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclusionName.Control = this.cboConclusionName;
            this.lciConclusionName.Location = new System.Drawing.Point(695, 24);
            this.lciConclusionName.Name = "lciConclusionName";
            this.lciConclusionName.OptionsToolTip.ToolTip = "Bác sĩ kết luận";
            this.lciConclusionName.Size = new System.Drawing.Size(285, 24);
            this.lciConclusionName.Text = "BS kết luận:";
            this.lciConclusionName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclusionName.TextSize = new System.Drawing.Size(90, 20);
            this.lciConclusionName.TextToControlDistance = 5;
            // 
            // lciReasonBadHeathly
            // 
            this.lciReasonBadHeathly.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciReasonBadHeathly.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciReasonBadHeathly.Control = this.txtReasonBadHeathly;
            this.lciReasonBadHeathly.Location = new System.Drawing.Point(411, 48);
            this.lciReasonBadHeathly.Name = "lciReasonBadHeathly";
            this.lciReasonBadHeathly.OptionsToolTip.ToolTip = "Lý do sức khỏe kém";
            this.lciReasonBadHeathly.Size = new System.Drawing.Size(569, 24);
            this.lciReasonBadHeathly.Text = "Lý do SK kém:";
            this.lciReasonBadHeathly.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciReasonBadHeathly.TextSize = new System.Drawing.Size(90, 13);
            this.lciReasonBadHeathly.TextToControlDistance = 5;
            // 
            // lciSickCondition
            // 
            this.lciSickCondition.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSickCondition.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSickCondition.Control = this.txtSickCondition;
            this.lciSickCondition.Location = new System.Drawing.Point(0, 72);
            this.lciSickCondition.Name = "lciSickCondition";
            this.lciSickCondition.Size = new System.Drawing.Size(980, 24);
            this.lciSickCondition.Text = "Tình trạng bệnh:";
            this.lciSickCondition.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSickCondition.TextSize = new System.Drawing.Size(130, 20);
            this.lciSickCondition.TextToControlDistance = 5;
            // 
            // lciConcentration
            // 
            this.lciConcentration.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lciConcentration.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciConcentration.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConcentration.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConcentration.Control = this.spConcentration;
            this.lciConcentration.Location = new System.Drawing.Point(0, 96);
            this.lciConcentration.Name = "lciConcentration";
            this.lciConcentration.Size = new System.Drawing.Size(411, 24);
            this.lciConcentration.Text = "Nồng độ cồn:";
            this.lciConcentration.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConcentration.TextSize = new System.Drawing.Size(130, 20);
            this.lciConcentration.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.chkMgMau;
            this.layoutControlItem12.Location = new System.Drawing.Point(582, 96);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(398, 24);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.chkMgKhi;
            this.layoutControlItem13.Location = new System.Drawing.Point(411, 96);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(171, 24);
            this.layoutControlItem13.Text = " ";
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // lciDrug
            // 
            this.lciDrug.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDrug.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDrug.Control = this.chkNegative;
            this.lciDrug.Location = new System.Drawing.Point(0, 120);
            this.lciDrug.Name = "lciDrug";
            this.lciDrug.Size = new System.Drawing.Size(318, 25);
            this.lciDrug.Text = "Ma túy:";
            this.lciDrug.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDrug.TextSize = new System.Drawing.Size(100, 20);
            this.lciDrug.TextToControlDistance = 5;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.chkPositive;
            this.layoutControlItem15.Location = new System.Drawing.Point(318, 120);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(662, 25);
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.dtAppointmentTime;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(411, 24);
            this.layoutControlItem3.Text = "Ngày khám lại:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(130, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // groupPatient
            // 
            this.groupPatient.Controls.Add(this.layoutControl2);
            this.groupPatient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupPatient.Location = new System.Drawing.Point(2, 162);
            this.groupPatient.Name = "groupPatient";
            this.groupPatient.Size = new System.Drawing.Size(986, 123);
            this.groupPatient.TabIndex = 12;
            this.groupPatient.TabStop = false;
            this.groupPatient.Text = "Thông tin người khám";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.dtTimeCccd);
            this.layoutControl2.Controls.Add(this.txtPlaceCccd);
            this.layoutControl2.Controls.Add(this.txtCccdCmnd);
            this.layoutControl2.Controls.Add(this.lblAddress);
            this.layoutControl2.Controls.Add(this.lblPatientName);
            this.layoutControl2.Controls.Add(this.lblDob);
            this.layoutControl2.Controls.Add(this.lblGender);
            this.layoutControl2.Controls.Add(this.lblPatientCode);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(3, 16);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(980, 104);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // dtTimeCccd
            // 
            this.dtTimeCccd.EditValue = null;
            this.dtTimeCccd.Location = new System.Drawing.Point(508, 74);
            this.dtTimeCccd.Name = "dtTimeCccd";
            this.dtTimeCccd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeCccd.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeCccd.Size = new System.Drawing.Size(185, 20);
            this.dtTimeCccd.StyleController = this.layoutControl2;
            this.dtTimeCccd.TabIndex = 16;
            this.dtTimeCccd.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeCccd_Closed);
            this.dtTimeCccd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtTimeCccd_PreviewKeyDown);
            // 
            // txtPlaceCccd
            // 
            this.txtPlaceCccd.Location = new System.Drawing.Point(792, 74);
            this.txtPlaceCccd.Name = "txtPlaceCccd";
            this.txtPlaceCccd.Size = new System.Drawing.Size(186, 20);
            this.txtPlaceCccd.StyleController = this.layoutControl2;
            this.txtPlaceCccd.TabIndex = 15;
            this.txtPlaceCccd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPlaceCccd_PreviewKeyDown);
            // 
            // txtCccdCmnd
            // 
            this.txtCccdCmnd.Location = new System.Drawing.Point(137, 74);
            this.txtCccdCmnd.Name = "txtCccdCmnd";
            this.txtCccdCmnd.Size = new System.Drawing.Size(272, 20);
            this.txtCccdCmnd.StyleController = this.layoutControl2;
            this.txtCccdCmnd.TabIndex = 13;
            this.txtCccdCmnd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCccdCmnd_PreviewKeyDown);
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAddress.Location = new System.Drawing.Point(137, 50);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(841, 20);
            this.lblAddress.StyleController = this.layoutControl2;
            this.lblAddress.TabIndex = 12;
            // 
            // lblPatientName
            // 
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(137, 26);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Padding = new System.Windows.Forms.Padding(2);
            this.lblPatientName.Size = new System.Drawing.Size(272, 20);
            this.lblPatientName.StyleController = this.layoutControl2;
            this.lblPatientName.TabIndex = 9;
            // 
            // lblDob
            // 
            this.lblDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDob.Location = new System.Drawing.Point(792, 26);
            this.lblDob.Name = "lblDob";
            this.lblDob.Padding = new System.Windows.Forms.Padding(2);
            this.lblDob.Size = new System.Drawing.Size(186, 20);
            this.lblDob.StyleController = this.layoutControl2;
            this.lblDob.TabIndex = 11;
            // 
            // lblGender
            // 
            this.lblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGender.Location = new System.Drawing.Point(508, 26);
            this.lblGender.Name = "lblGender";
            this.lblGender.Padding = new System.Windows.Forms.Padding(2);
            this.lblGender.Size = new System.Drawing.Size(185, 20);
            this.lblGender.StyleController = this.layoutControl2;
            this.lblGender.TabIndex = 10;
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(137, 2);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Padding = new System.Windows.Forms.Padding(2);
            this.lblPatientCode.Size = new System.Drawing.Size(841, 20);
            this.lblPatientCode.StyleController = this.layoutControl2;
            this.lblPatientCode.TabIndex = 8;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciPatientCode,
            this.lciPatientName,
            this.lciGender,
            this.lciDob,
            this.lciAddress,
            this.lciCccdNumber,
            this.lciPlaceCccd,
            this.lciTimeCccd});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(980, 104);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.lblPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(0, 0);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.OptionsToolTip.ToolTip = "Mã bệnh nhân";
            this.lciPatientCode.Size = new System.Drawing.Size(980, 24);
            this.lciPatientCode.Text = "Mã BN:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(130, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(0, 24);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.OptionsToolTip.ToolTip = "Tên bệnh nhân";
            this.lciPatientName.Size = new System.Drawing.Size(411, 24);
            this.lciPatientName.Text = "Tên BN:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(130, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciGender
            // 
            this.lciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciGender.Control = this.lblGender;
            this.lciGender.Location = new System.Drawing.Point(411, 24);
            this.lciGender.Name = "lciGender";
            this.lciGender.Size = new System.Drawing.Size(284, 24);
            this.lciGender.Text = "Giới tính:";
            this.lciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciGender.TextSize = new System.Drawing.Size(90, 20);
            this.lciGender.TextToControlDistance = 5;
            // 
            // lciDob
            // 
            this.lciDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDob.Control = this.lblDob;
            this.lciDob.Location = new System.Drawing.Point(695, 24);
            this.lciDob.Name = "lciDob";
            this.lciDob.Size = new System.Drawing.Size(285, 24);
            this.lciDob.Text = "Ngày sinh:";
            this.lciDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDob.TextSize = new System.Drawing.Size(90, 20);
            this.lciDob.TextToControlDistance = 5;
            // 
            // lciAddress
            // 
            this.lciAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAddress.Control = this.lblAddress;
            this.lciAddress.Location = new System.Drawing.Point(0, 48);
            this.lciAddress.Name = "lciAddress";
            this.lciAddress.Size = new System.Drawing.Size(980, 24);
            this.lciAddress.Text = "Địa chỉ:";
            this.lciAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAddress.TextSize = new System.Drawing.Size(130, 20);
            this.lciAddress.TextToControlDistance = 5;
            // 
            // lciCccdNumber
            // 
            this.lciCccdNumber.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCccdNumber.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCccdNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCccdNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCccdNumber.Control = this.txtCccdCmnd;
            this.lciCccdNumber.Location = new System.Drawing.Point(0, 72);
            this.lciCccdNumber.Name = "lciCccdNumber";
            this.lciCccdNumber.OptionsToolTip.ToolTip = "Số Chứng minh nhân dân/ Căn cước công dân/ Hộ chiếu";
            this.lciCccdNumber.Size = new System.Drawing.Size(411, 32);
            this.lciCccdNumber.Text = "Số CMND/CCCD/HC:";
            this.lciCccdNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCccdNumber.TextSize = new System.Drawing.Size(130, 20);
            this.lciCccdNumber.TextToControlDistance = 5;
            // 
            // lciPlaceCccd
            // 
            this.lciPlaceCccd.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPlaceCccd.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPlaceCccd.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPlaceCccd.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPlaceCccd.Control = this.txtPlaceCccd;
            this.lciPlaceCccd.Location = new System.Drawing.Point(695, 72);
            this.lciPlaceCccd.Name = "lciPlaceCccd";
            this.lciPlaceCccd.Size = new System.Drawing.Size(285, 32);
            this.lciPlaceCccd.Text = "Nơi cấp:";
            this.lciPlaceCccd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPlaceCccd.TextSize = new System.Drawing.Size(90, 20);
            this.lciPlaceCccd.TextToControlDistance = 5;
            // 
            // lciTimeCccd
            // 
            this.lciTimeCccd.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciTimeCccd.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciTimeCccd.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTimeCccd.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTimeCccd.Control = this.dtTimeCccd;
            this.lciTimeCccd.Location = new System.Drawing.Point(411, 72);
            this.lciTimeCccd.Name = "lciTimeCccd";
            this.lciTimeCccd.Size = new System.Drawing.Size(284, 32);
            this.lciTimeCccd.Text = "Ngày cấp:";
            this.lciTimeCccd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTimeCccd.TextSize = new System.Drawing.Size(90, 20);
            this.lciTimeCccd.TextToControlDistance = 5;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(429, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(123, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Tìm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtServiceReqCode
            // 
            this.txtServiceReqCode.Location = new System.Drawing.Point(283, 2);
            this.txtServiceReqCode.Name = "txtServiceReqCode";
            this.txtServiceReqCode.Properties.NullValuePrompt = "Mã y lệnh khám (F4)";
            this.txtServiceReqCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtServiceReqCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtServiceReqCode.Size = new System.Drawing.Size(142, 20);
            this.txtServiceReqCode.StyleController = this.layoutControl1;
            this.txtServiceReqCode.TabIndex = 6;
            this.txtServiceReqCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceReqCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(139, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị (F3)";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(140, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl1;
            this.txtTreatmentCode.TabIndex = 5;
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciSearchTreatmentCode,
            this.lciSearchServiceReqCode,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem2,
            this.lciAutoPush,
            this.layoutControlItem17,
            this.layoutControlItem5,
            this.layoutControlItem7,
            this.layoutControlItem6,
            this.lciPatient});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(990, 481);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciSearchTreatmentCode
            // 
            this.lciSearchTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSearchTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSearchTreatmentCode.Control = this.txtTreatmentCode;
            this.lciSearchTreatmentCode.Location = new System.Drawing.Point(137, 0);
            this.lciSearchTreatmentCode.Name = "lciSearchTreatmentCode";
            this.lciSearchTreatmentCode.Size = new System.Drawing.Size(144, 26);
            this.lciSearchTreatmentCode.Text = "Mã điều trị:";
            this.lciSearchTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSearchTreatmentCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearchTreatmentCode.TextToControlDistance = 0;
            this.lciSearchTreatmentCode.TextVisible = false;
            // 
            // lciSearchServiceReqCode
            // 
            this.lciSearchServiceReqCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSearchServiceReqCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSearchServiceReqCode.Control = this.txtServiceReqCode;
            this.lciSearchServiceReqCode.Location = new System.Drawing.Point(281, 0);
            this.lciSearchServiceReqCode.Name = "lciSearchServiceReqCode";
            this.lciSearchServiceReqCode.Size = new System.Drawing.Size(146, 26);
            this.lciSearchServiceReqCode.Text = "Mã y lệnh:";
            this.lciSearchServiceReqCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSearchServiceReqCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearchServiceReqCode.TextToControlDistance = 0;
            this.lciSearchServiceReqCode.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnSearch;
            this.layoutControlItem4.Location = new System.Drawing.Point(427, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(127, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(554, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(436, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.groupPatient;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 160);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(104, 119);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(990, 127);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.groupKsk;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 287);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 167);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(990, 168);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 455);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(560, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciAutoPush
            // 
            this.lciAutoPush.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAutoPush.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAutoPush.Control = this.chkAutoPush;
            this.lciAutoPush.Location = new System.Drawing.Point(691, 455);
            this.lciAutoPush.Name = "lciAutoPush";
            this.lciAutoPush.OptionsToolTip.ToolTip = "Tự động đẩy cổng sau khi lưu thành công";
            this.lciAutoPush.Size = new System.Drawing.Size(148, 26);
            this.lciAutoPush.Text = "Tự động đẩy cổng";
            this.lciAutoPush.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAutoPush.TextSize = new System.Drawing.Size(120, 20);
            this.lciAutoPush.TextToControlDistance = 5;
            // 
            // layoutControlItem17
            // 
            this.layoutControlItem17.Control = this.btnSave;
            this.layoutControlItem17.Location = new System.Drawing.Point(839, 455);
            this.layoutControlItem17.Name = "layoutControlItem17";
            this.layoutControlItem17.Size = new System.Drawing.Size(68, 26);
            this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem17.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.chkSignFileCertUtil;
            this.layoutControlItem5.Location = new System.Drawing.Point(560, 455);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.OptionsToolTip.ToolTip = "Thực hiện ký số sử dụng USB token khi đồng bộ dữ liệu";
            this.layoutControlItem5.Size = new System.Drawing.Size(131, 26);
            this.layoutControlItem5.Text = "Ký số dùng USB token";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(105, 13);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnReset;
            this.layoutControlItem7.Location = new System.Drawing.Point(907, 455);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(83, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.gridControlKskDriver;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(990, 134);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciPatient
            // 
            this.lciPatient.Control = this.txtPatietnCode;
            this.lciPatient.Location = new System.Drawing.Point(0, 0);
            this.lciPatient.Name = "lciPatient";
            this.lciPatient.Size = new System.Drawing.Size(137, 26);
            this.lciPatient.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatient.TextVisible = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 510);
            this.barDockControlBottom.Size = new System.Drawing.Size(990, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 481);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(990, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 481);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // FormKskDriver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 510);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormKskDriver";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hồ sơ khám sức khỏe lái xe";
            this.Load += new System.EventHandler(this.FormKskDriver_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPatietnCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlKskDriver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewKskDriver)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEditCopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSignFileCertUtil.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAutoPush.Properties)).EndInit();
            this.groupKsk.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtAppointmentTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPositive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNegative.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMgKhi.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMgMau.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spConcentration.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSickCondition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReasonBadHeathly.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConclusionName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLicenesClass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConclusion.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKskDriverCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtConclusionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKskDriverCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLicenesClass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclusionName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciReasonBadHeathly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSickCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConcentration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDrug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.groupPatient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeCccd.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeCccd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlaceCccd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCccdCmnd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCccdNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPlaceCccd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTimeCccd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceReqCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearchServiceReqCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAutoPush)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtServiceReqCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciSearchTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciSearchServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblGender;
        private DevExpress.XtraEditors.LabelControl lblDob;
        private System.Windows.Forms.GroupBox groupPatient;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciGender;
        private DevExpress.XtraLayout.LayoutControlItem lciDob;
        private DevExpress.XtraEditors.LabelControl lblAddress;
        private DevExpress.XtraLayout.LayoutControlItem lciAddress;
        private DevExpress.XtraEditors.TextEdit txtPlaceCccd;
        private DevExpress.XtraEditors.TextEdit txtCccdCmnd;
        private DevExpress.XtraLayout.LayoutControlItem lciCccdNumber;
        private DevExpress.XtraLayout.LayoutControlItem lciPlaceCccd;
        private DevExpress.XtraEditors.DateEdit dtTimeCccd;
        private DevExpress.XtraLayout.LayoutControlItem lciTimeCccd;
        private System.Windows.Forms.GroupBox groupKsk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraEditors.CheckEdit chkPositive;
        private DevExpress.XtraEditors.CheckEdit chkNegative;
        private DevExpress.XtraEditors.CheckEdit chkMgKhi;
        private DevExpress.XtraEditors.CheckEdit chkMgMau;
        private DevExpress.XtraEditors.SpinEdit spConcentration;
        private DevExpress.XtraEditors.TextEdit txtSickCondition;
        private DevExpress.XtraEditors.TextEdit txtReasonBadHeathly;
        private HIS.Desktop.Utilities.Extensions.CustomGridLookUpEdit cboConclusionName;
        private HIS.Desktop.Utilities.Extensions.CustomGridView gridLookUpEdit3View;
        private DevExpress.XtraEditors.GridLookUpEdit cboLicenesClass;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.GridLookUpEdit cboConclusion;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtKskDriverCode;
        private DevExpress.XtraEditors.DateEdit dtConclusionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusionTime;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciKskDriverCode;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusion;
        private DevExpress.XtraLayout.LayoutControlItem lciLicenesClass;
        private DevExpress.XtraLayout.LayoutControlItem lciConclusionName;
        private DevExpress.XtraLayout.LayoutControlItem lciReasonBadHeathly;
        private DevExpress.XtraLayout.LayoutControlItem lciSickCondition;
        private DevExpress.XtraLayout.LayoutControlItem lciConcentration;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem lciDrug;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.CheckEdit chkAutoPush;
        private DevExpress.XtraLayout.LayoutControlItem lciAutoPush;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem17;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnF3;
        private DevExpress.XtraBars.BarButtonItem barBtnF4;
        private DevExpress.XtraBars.BarButtonItem barBtnCtrlF;
        private DevExpress.XtraBars.BarButtonItem barBtnCtrlS;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DateEdit dtAppointmentTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.CheckEdit chkSignFileCertUtil;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.GridControl gridControlKskDriver;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewKskDriver;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEditCopy;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraEditors.SimpleButton btnReset;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraBars.BarButtonItem bbtnCtrlR;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.TextEdit txtPatietnCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatient;
        private DevExpress.XtraBars.BarButtonItem barBtnF2;
    }
}
