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
namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    partial class FormHisTreatmentRecordChecking
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHisTreatmentRecordChecking));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject13 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject14 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject15 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject16 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.chkIncludeCancelDoc = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnKhongDat = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnDat = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnHuyDuyet = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.chkToiTao = new DevExpress.XtraEditors.CheckEdit();
            this.Gc_Treatment = new DevExpress.XtraGrid.GridControl();
            this.Gv_Treatment = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gv_Treatment_Gc_STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_Treatment_Gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_Treatment_Gc_PatientDOB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_Treatment_Gc_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.chkUuTien = new DevExpress.XtraEditors.CheckEdit();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnHuyDuyet = new DevExpress.XtraEditors.SimpleButton();
            this.btnDat = new DevExpress.XtraEditors.SimpleButton();
            this.btnKhongDat = new DevExpress.XtraEditors.SimpleButton();
            this.LblSubIcdYhct = new DevExpress.XtraEditors.LabelControl();
            this.LblIcdYhct = new DevExpress.XtraEditors.LabelControl();
            this.Gc_EmrDocument = new DevExpress.XtraGrid.GridControl();
            this.Gv_EmrDocument = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gv_ED_Gc_STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.Gv_ED_Gc_View = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Gv_ED_Gc_Name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_ED_Gc_HisCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_ED_Gc_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_ED_Gc_Signers = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_ED_Gc_UnSigners = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_ED_Gc_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.Gc_InfoRecord = new DevExpress.XtraGrid.GridControl();
            this.Gv_InfoRecord = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gv_IR_STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.Gv_IR_Code = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_IR_Type = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_IR_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_IR_DepartmentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gv_IR_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.GcEmrDocumentType = new DevExpress.XtraGrid.GridControl();
            this.GvEmrDocumentType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gr_Gc_Name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.LblNote = new DevExpress.XtraEditors.LabelControl();
            this.LblSubIcd = new DevExpress.XtraEditors.LabelControl();
            this.LblMainIcd = new DevExpress.XtraEditors.LabelControl();
            this.LblAddress = new DevExpress.XtraEditors.LabelControl();
            this.LblMediOrg = new DevExpress.XtraEditors.LabelControl();
            this.LblHeinTime = new DevExpress.XtraEditors.LabelControl();
            this.LblHeinNumber = new DevExpress.XtraEditors.LabelControl();
            this.LblPatientType = new DevExpress.XtraEditors.LabelControl();
            this.LblGender = new DevExpress.XtraEditors.LabelControl();
            this.LblDob = new DevExpress.XtraEditors.LabelControl();
            this.LblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.LblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.BtnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.TxtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.LcgPatientInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciDob = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciGender = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciHeinNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciMainIcd = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciSubIcd = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciNote = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciHeinTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciMediOrg = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciIcdYhct = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciSubIcdYhct = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciTotalInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciGC_Treatment = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeCancelDoc.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkToiTao.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_Treatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_Treatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUuTien.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_EmrDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_EmrDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_InfoRecord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_InfoRecord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GcEmrDocumentType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GvEmrDocumentType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcgPatientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDob)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciGender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciHeinNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciMainIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSubIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciHeinTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciMediOrg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciIcdYhct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSubIcdYhct)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTotalInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGC_Treatment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkIncludeCancelDoc);
            this.layoutControl1.Controls.Add(this.chkToiTao);
            this.layoutControl1.Controls.Add(this.Gc_Treatment);
            this.layoutControl1.Controls.Add(this.chkUuTien);
            this.layoutControl1.Controls.Add(this.lblStatus);
            this.layoutControl1.Controls.Add(this.btnHuyDuyet);
            this.layoutControl1.Controls.Add(this.btnDat);
            this.layoutControl1.Controls.Add(this.btnKhongDat);
            this.layoutControl1.Controls.Add(this.LblSubIcdYhct);
            this.layoutControl1.Controls.Add(this.LblIcdYhct);
            this.layoutControl1.Controls.Add(this.Gc_EmrDocument);
            this.layoutControl1.Controls.Add(this.Gc_InfoRecord);
            this.layoutControl1.Controls.Add(this.GcEmrDocumentType);
            this.layoutControl1.Controls.Add(this.LblNote);
            this.layoutControl1.Controls.Add(this.LblSubIcd);
            this.layoutControl1.Controls.Add(this.LblMainIcd);
            this.layoutControl1.Controls.Add(this.LblAddress);
            this.layoutControl1.Controls.Add(this.LblMediOrg);
            this.layoutControl1.Controls.Add(this.LblHeinTime);
            this.layoutControl1.Controls.Add(this.LblHeinNumber);
            this.layoutControl1.Controls.Add(this.LblPatientType);
            this.layoutControl1.Controls.Add(this.LblGender);
            this.layoutControl1.Controls.Add(this.LblDob);
            this.layoutControl1.Controls.Add(this.LblPatientName);
            this.layoutControl1.Controls.Add(this.LblPatientCode);
            this.layoutControl1.Controls.Add(this.BtnSearch);
            this.layoutControl1.Controls.Add(this.TxtTreatmentCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(54, 210, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1264, 632);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkIncludeCancelDoc
            // 
            this.chkIncludeCancelDoc.Location = new System.Drawing.Point(243, 268);
            this.chkIncludeCancelDoc.MenuManager = this.barManager1;
            this.chkIncludeCancelDoc.Name = "chkIncludeCancelDoc";
            this.chkIncludeCancelDoc.Properties.Caption = "Bao gồm văn bản hủy";
            this.chkIncludeCancelDoc.Size = new System.Drawing.Size(126, 19);
            this.chkIncludeCancelDoc.StyleController = this.layoutControl1;
            this.chkIncludeCancelDoc.TabIndex = 31;
            this.chkIncludeCancelDoc.CheckedChanged += new System.EventHandler(this.chkIncludeCancelDoc_CheckedChanged);
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
            this.barButtonItem1,
            this.bbtnKhongDat,
            this.bbtnDat,
            this.bbtnHuyDuyet});
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnKhongDat),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnDat),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnHuyDuyet)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Tim";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // bbtnKhongDat
            // 
            this.bbtnKhongDat.Caption = "Không đạt (Ctrl D)";
            this.bbtnKhongDat.Id = 1;
            this.bbtnKhongDat.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.bbtnKhongDat.Name = "bbtnKhongDat";
            this.bbtnKhongDat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnKhongDat_ItemClick);
            // 
            // bbtnDat
            // 
            this.bbtnDat.Caption = "Đạt (Ctrl S)";
            this.bbtnDat.Id = 2;
            this.bbtnDat.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnDat.Name = "bbtnDat";
            this.bbtnDat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnDat_ItemClick);
            // 
            // bbtnHuyDuyet
            // 
            this.bbtnHuyDuyet.Caption = "Hủy duyệt (Ctrl H)";
            this.bbtnHuyDuyet.Id = 3;
            this.bbtnHuyDuyet.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H));
            this.bbtnHuyDuyet.Name = "bbtnHuyDuyet";
            this.bbtnHuyDuyet.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnHuyDuyet_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1264, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 661);
            this.barDockControlBottom.Size = new System.Drawing.Size(1264, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 632);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1264, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 632);
            // 
            // chkToiTao
            // 
            this.chkToiTao.Location = new System.Drawing.Point(130, 268);
            this.chkToiTao.MenuManager = this.barManager1;
            this.chkToiTao.Name = "chkToiTao";
            this.chkToiTao.Properties.Caption = "Tôi tạo";
            this.chkToiTao.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkToiTao.Size = new System.Drawing.Size(109, 19);
            this.chkToiTao.StyleController = this.layoutControl1;
            this.chkToiTao.TabIndex = 30;
            this.chkToiTao.ToolTip = "Chỉ hiển thị các dữ liệu mà tôi là người tạo";
            this.chkToiTao.CheckedChanged += new System.EventHandler(this.chkToiTao_CheckedChanged);
            // 
            // Gc_Treatment
            // 
            this.Gc_Treatment.Location = new System.Drawing.Point(997, 28);
            this.Gc_Treatment.MainView = this.Gv_Treatment;
            this.Gc_Treatment.MenuManager = this.barManager1;
            this.Gc_Treatment.Name = "Gc_Treatment";
            this.Gc_Treatment.Size = new System.Drawing.Size(265, 236);
            this.Gc_Treatment.TabIndex = 29;
            this.Gc_Treatment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.Gv_Treatment});
            // 
            // Gv_Treatment
            // 
            this.Gv_Treatment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gv_Treatment_Gc_STT,
            this.Gv_Treatment_Gc_PatientName,
            this.Gv_Treatment_Gc_PatientDOB,
            this.Gv_Treatment_Gc_TreatmentCode});
            this.Gv_Treatment.GridControl = this.Gc_Treatment;
            this.Gv_Treatment.Name = "Gv_Treatment";
            this.Gv_Treatment.OptionsView.ColumnAutoWidth = false;
            this.Gv_Treatment.OptionsView.ShowGroupPanel = false;
            this.Gv_Treatment.OptionsView.ShowIndicator = false;
            this.Gv_Treatment.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.Gv_Treatment_RowCellClick);
            this.Gv_Treatment.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_Treatment_RowCellStyle);
            this.Gv_Treatment.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.Gv_Treatment_CustomUnboundColumnData);
            // 
            // Gv_Treatment_Gc_STT
            // 
            this.Gv_Treatment_Gc_STT.AppearanceCell.Options.UseTextOptions = true;
            this.Gv_Treatment_Gc_STT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gv_Treatment_Gc_STT.Caption = "STT";
            this.Gv_Treatment_Gc_STT.FieldName = "STT";
            this.Gv_Treatment_Gc_STT.Name = "Gv_Treatment_Gc_STT";
            this.Gv_Treatment_Gc_STT.OptionsColumn.AllowEdit = false;
            this.Gv_Treatment_Gc_STT.OptionsFilter.AllowFilter = false;
            this.Gv_Treatment_Gc_STT.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Gv_Treatment_Gc_STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gv_Treatment_Gc_STT.Visible = true;
            this.Gv_Treatment_Gc_STT.VisibleIndex = 0;
            this.Gv_Treatment_Gc_STT.Width = 35;
            // 
            // Gv_Treatment_Gc_PatientName
            // 
            this.Gv_Treatment_Gc_PatientName.Caption = "Tên bệnh nhân";
            this.Gv_Treatment_Gc_PatientName.FieldName = "TDL_PATIENT_NAME";
            this.Gv_Treatment_Gc_PatientName.Name = "Gv_Treatment_Gc_PatientName";
            this.Gv_Treatment_Gc_PatientName.OptionsColumn.AllowEdit = false;
            this.Gv_Treatment_Gc_PatientName.Visible = true;
            this.Gv_Treatment_Gc_PatientName.VisibleIndex = 1;
            this.Gv_Treatment_Gc_PatientName.Width = 120;
            // 
            // Gv_Treatment_Gc_PatientDOB
            // 
            this.Gv_Treatment_Gc_PatientDOB.Caption = "Năm sinh";
            this.Gv_Treatment_Gc_PatientDOB.FieldName = "PATIENT_DOB_ForDisplay";
            this.Gv_Treatment_Gc_PatientDOB.Name = "Gv_Treatment_Gc_PatientDOB";
            this.Gv_Treatment_Gc_PatientDOB.OptionsColumn.AllowEdit = false;
            this.Gv_Treatment_Gc_PatientDOB.OptionsFilter.AllowFilter = false;
            this.Gv_Treatment_Gc_PatientDOB.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
            this.Gv_Treatment_Gc_PatientDOB.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gv_Treatment_Gc_PatientDOB.Visible = true;
            this.Gv_Treatment_Gc_PatientDOB.VisibleIndex = 2;
            this.Gv_Treatment_Gc_PatientDOB.Width = 60;
            // 
            // Gv_Treatment_Gc_TreatmentCode
            // 
            this.Gv_Treatment_Gc_TreatmentCode.Caption = "Mã điều trị";
            this.Gv_Treatment_Gc_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.Gv_Treatment_Gc_TreatmentCode.Name = "Gv_Treatment_Gc_TreatmentCode";
            this.Gv_Treatment_Gc_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.Gv_Treatment_Gc_TreatmentCode.Visible = true;
            this.Gv_Treatment_Gc_TreatmentCode.VisibleIndex = 3;
            this.Gv_Treatment_Gc_TreatmentCode.Width = 120;
            // 
            // chkUuTien
            // 
            this.chkUuTien.Location = new System.Drawing.Point(2, 268);
            this.chkUuTien.MenuManager = this.barManager1;
            this.chkUuTien.Name = "chkUuTien";
            this.chkUuTien.Properties.Caption = "Ưu tiên có dữ liệu";
            this.chkUuTien.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkUuTien.Size = new System.Drawing.Size(124, 19);
            this.chkUuTien.StyleController = this.layoutControl1;
            this.chkUuTien.TabIndex = 28;
            this.chkUuTien.ToolTip = "Ưu tiên hiển thị các loại văn bản có dữ liệu lên đầu";
            this.chkUuTien.CheckedChanged += new System.EventHandler(this.chkUuTien_CheckedChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStatus.Location = new System.Drawing.Point(100, 241);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(890, 20);
            this.lblStatus.StyleController = this.layoutControl1;
            this.lblStatus.TabIndex = 27;
            // 
            // btnHuyDuyet
            // 
            this.btnHuyDuyet.Location = new System.Drawing.Point(1100, 608);
            this.btnHuyDuyet.Name = "btnHuyDuyet";
            this.btnHuyDuyet.Size = new System.Drawing.Size(162, 22);
            this.btnHuyDuyet.StyleController = this.layoutControl1;
            this.btnHuyDuyet.TabIndex = 26;
            this.btnHuyDuyet.Text = "Hủy duyệt (Ctrl H)";
            this.btnHuyDuyet.ToolTip = "Hủy thông tin duyệt hồ sơ bệnh án (thông tin xác nhận đạt/không đạt điều kiện lưu" +
    " tủ bệnh án)";
            this.btnHuyDuyet.Click += new System.EventHandler(this.btnHuyDuyet_Click);
            // 
            // btnDat
            // 
            this.btnDat.Location = new System.Drawing.Point(972, 608);
            this.btnDat.Name = "btnDat";
            this.btnDat.Size = new System.Drawing.Size(124, 22);
            this.btnDat.StyleController = this.layoutControl1;
            this.btnDat.TabIndex = 25;
            this.btnDat.Text = "Đạt (Ctrl S)";
            this.btnDat.ToolTip = "Xác nhận hồ sơ đạt điều kiện lưu tủ bệnh án";
            this.btnDat.Click += new System.EventHandler(this.btnDat_Click);
            // 
            // btnKhongDat
            // 
            this.btnKhongDat.Location = new System.Drawing.Point(824, 608);
            this.btnKhongDat.Name = "btnKhongDat";
            this.btnKhongDat.Size = new System.Drawing.Size(144, 22);
            this.btnKhongDat.StyleController = this.layoutControl1;
            this.btnKhongDat.TabIndex = 24;
            this.btnKhongDat.Text = "Không đạt (Ctrl D)";
            this.btnKhongDat.ToolTip = "Xác nhận hồ sơ chưa đạt điều kiện lưu tủ bệnh án";
            this.btnKhongDat.Click += new System.EventHandler(this.btnKhongDat_Click);
            // 
            // LblSubIcdYhct
            // 
            this.LblSubIcdYhct.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblSubIcdYhct.Location = new System.Drawing.Point(100, 193);
            this.LblSubIcdYhct.Name = "LblSubIcdYhct";
            this.LblSubIcdYhct.Size = new System.Drawing.Size(890, 20);
            this.LblSubIcdYhct.StyleController = this.layoutControl1;
            this.LblSubIcdYhct.TabIndex = 23;
            // 
            // LblIcdYhct
            // 
            this.LblIcdYhct.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblIcdYhct.Location = new System.Drawing.Point(100, 169);
            this.LblIcdYhct.Name = "LblIcdYhct";
            this.LblIcdYhct.Size = new System.Drawing.Size(890, 20);
            this.LblIcdYhct.StyleController = this.layoutControl1;
            this.LblIcdYhct.TabIndex = 22;
            // 
            // Gc_EmrDocument
            // 
            this.Gc_EmrDocument.Location = new System.Drawing.Point(678, 291);
            this.Gc_EmrDocument.MainView = this.Gv_EmrDocument;
            this.Gc_EmrDocument.Name = "Gc_EmrDocument";
            this.Gc_EmrDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonView,
            this.repositoryItemPictureEdit1});
            this.Gc_EmrDocument.Size = new System.Drawing.Size(584, 313);
            this.Gc_EmrDocument.TabIndex = 21;
            this.Gc_EmrDocument.ToolTipController = this.toolTipController1;
            this.Gc_EmrDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.Gv_EmrDocument});
            // 
            // Gv_EmrDocument
            // 
            this.Gv_EmrDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gv_ED_Gc_STT,
            this.Gv_ED_Gc_View,
            this.Gv_ED_Gc_Name,
            this.Gv_ED_Gc_HisCode,
            this.Gv_ED_Gc_Creator,
            this.Gv_ED_Gc_Signers,
            this.Gv_ED_Gc_UnSigners,
            this.Gv_ED_Gc_CreateTime});
            this.Gv_EmrDocument.GridControl = this.Gc_EmrDocument;
            this.Gv_EmrDocument.Name = "Gv_EmrDocument";
            this.Gv_EmrDocument.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.Gv_EmrDocument.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.Gv_EmrDocument.OptionsView.ColumnAutoWidth = false;
            this.Gv_EmrDocument.OptionsView.ShowGroupPanel = false;
            this.Gv_EmrDocument.OptionsView.ShowIndicator = false;
            this.Gv_EmrDocument.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.Gv_EmrDocument_RowCellClick);
            this.Gv_EmrDocument.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_EmrDocument_RowCellStyle);
            this.Gv_EmrDocument.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.Gv_EmrDocument_CustomUnboundColumnData);
            // 
            // Gv_ED_Gc_STT
            // 
            this.Gv_ED_Gc_STT.Caption = "Trạng thái";
            this.Gv_ED_Gc_STT.ColumnEdit = this.repositoryItemPictureEdit1;
            this.Gv_ED_Gc_STT.FieldName = "STT";
            this.Gv_ED_Gc_STT.Name = "Gv_ED_Gc_STT";
            this.Gv_ED_Gc_STT.OptionsColumn.ShowCaption = false;
            this.Gv_ED_Gc_STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gv_ED_Gc_STT.Visible = true;
            this.Gv_ED_Gc_STT.VisibleIndex = 0;
            this.Gv_ED_Gc_STT.Width = 25;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.NullText = " ";
            // 
            // Gv_ED_Gc_View
            // 
            this.Gv_ED_Gc_View.Caption = "Xem";
            this.Gv_ED_Gc_View.ColumnEdit = this.repositoryItemButtonView;
            this.Gv_ED_Gc_View.Name = "Gv_ED_Gc_View";
            this.Gv_ED_Gc_View.OptionsColumn.ShowCaption = false;
            this.Gv_ED_Gc_View.Visible = true;
            this.Gv_ED_Gc_View.VisibleIndex = 1;
            this.Gv_ED_Gc_View.Width = 25;
            // 
            // repositoryItemButtonView
            // 
            this.repositoryItemButtonView.AutoHeight = false;
            this.repositoryItemButtonView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonView.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject13, serializableAppearanceObject14, serializableAppearanceObject15, serializableAppearanceObject16, "", null, null, true)});
            this.repositoryItemButtonView.Name = "repositoryItemButtonView";
            this.repositoryItemButtonView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonView.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonView_ButtonClick);
            // 
            // Gv_ED_Gc_Name
            // 
            this.Gv_ED_Gc_Name.Caption = "Tên văn bản";
            this.Gv_ED_Gc_Name.FieldName = "DOCUMENT_NAME";
            this.Gv_ED_Gc_Name.Name = "Gv_ED_Gc_Name";
            this.Gv_ED_Gc_Name.OptionsColumn.AllowEdit = false;
            this.Gv_ED_Gc_Name.Visible = true;
            this.Gv_ED_Gc_Name.VisibleIndex = 2;
            this.Gv_ED_Gc_Name.Width = 300;
            // 
            // Gv_ED_Gc_HisCode
            // 
            this.Gv_ED_Gc_HisCode.Caption = "Mã HIS";
            this.Gv_ED_Gc_HisCode.FieldName = "HIS_CODE";
            this.Gv_ED_Gc_HisCode.Name = "Gv_ED_Gc_HisCode";
            this.Gv_ED_Gc_HisCode.OptionsColumn.AllowEdit = false;
            this.Gv_ED_Gc_HisCode.Visible = true;
            this.Gv_ED_Gc_HisCode.VisibleIndex = 3;
            this.Gv_ED_Gc_HisCode.Width = 80;
            // 
            // Gv_ED_Gc_Creator
            // 
            this.Gv_ED_Gc_Creator.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.Gv_ED_Gc_Creator.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
            this.Gv_ED_Gc_Creator.AppearanceCell.Options.UseFont = true;
            this.Gv_ED_Gc_Creator.AppearanceCell.Options.UseForeColor = true;
            this.Gv_ED_Gc_Creator.Caption = "Người tạo";
            this.Gv_ED_Gc_Creator.FieldName = "CREATOR";
            this.Gv_ED_Gc_Creator.Name = "Gv_ED_Gc_Creator";
            this.Gv_ED_Gc_Creator.OptionsColumn.AllowEdit = false;
            this.Gv_ED_Gc_Creator.Visible = true;
            this.Gv_ED_Gc_Creator.VisibleIndex = 4;
            this.Gv_ED_Gc_Creator.Width = 150;
            // 
            // Gv_ED_Gc_Signers
            // 
            this.Gv_ED_Gc_Signers.Caption = "Người ký";
            this.Gv_ED_Gc_Signers.FieldName = "SIGNERS_Str";
            this.Gv_ED_Gc_Signers.Name = "Gv_ED_Gc_Signers";
            this.Gv_ED_Gc_Signers.OptionsColumn.ReadOnly = true;
            this.Gv_ED_Gc_Signers.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.Gv_ED_Gc_Signers.Visible = true;
            this.Gv_ED_Gc_Signers.VisibleIndex = 5;
            this.Gv_ED_Gc_Signers.Width = 120;
            // 
            // Gv_ED_Gc_UnSigners
            // 
            this.Gv_ED_Gc_UnSigners.Caption = "Người chưa ký";
            this.Gv_ED_Gc_UnSigners.FieldName = "UN_SIGNERS_Str";
            this.Gv_ED_Gc_UnSigners.Name = "Gv_ED_Gc_UnSigners";
            this.Gv_ED_Gc_UnSigners.OptionsColumn.ReadOnly = true;
            this.Gv_ED_Gc_UnSigners.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.Gv_ED_Gc_UnSigners.Visible = true;
            this.Gv_ED_Gc_UnSigners.VisibleIndex = 6;
            this.Gv_ED_Gc_UnSigners.Width = 120;
            // 
            // Gv_ED_Gc_CreateTime
            // 
            this.Gv_ED_Gc_CreateTime.Caption = "Thời gian tạo";
            this.Gv_ED_Gc_CreateTime.FieldName = "CREATE_TIME_Str";
            this.Gv_ED_Gc_CreateTime.FieldNameSortGroup = "CREATE_TIME";
            this.Gv_ED_Gc_CreateTime.Name = "Gv_ED_Gc_CreateTime";
            this.Gv_ED_Gc_CreateTime.OptionsColumn.AllowEdit = false;
            this.Gv_ED_Gc_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.Gv_ED_Gc_CreateTime.Visible = true;
            this.Gv_ED_Gc_CreateTime.VisibleIndex = 7;
            this.Gv_ED_Gc_CreateTime.Width = 125;
            // 
            // toolTipController1
            // 
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            // 
            // Gc_InfoRecord
            // 
            this.Gc_InfoRecord.Location = new System.Drawing.Point(222, 291);
            this.Gc_InfoRecord.MainView = this.Gv_InfoRecord;
            this.Gc_InfoRecord.Name = "Gc_InfoRecord";
            this.Gc_InfoRecord.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit2,
            this.repositoryItemTextEdit1});
            this.Gc_InfoRecord.Size = new System.Drawing.Size(452, 313);
            this.Gc_InfoRecord.TabIndex = 20;
            this.Gc_InfoRecord.ToolTipController = this.toolTipController1;
            this.Gc_InfoRecord.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.Gv_InfoRecord});
            // 
            // Gv_InfoRecord
            // 
            this.Gv_InfoRecord.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gv_IR_STT,
            this.Gv_IR_Code,
            this.Gv_IR_Type,
            this.Gv_IR_CreateTime,
            this.Gv_IR_DepartmentName,
            this.Gv_IR_Creator});
            this.Gv_InfoRecord.GridControl = this.Gc_InfoRecord;
            this.Gv_InfoRecord.Name = "Gv_InfoRecord";
            this.Gv_InfoRecord.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
            this.Gv_InfoRecord.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.Gv_InfoRecord.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.Gv_InfoRecord.OptionsView.ColumnAutoWidth = false;
            this.Gv_InfoRecord.OptionsView.ShowGroupPanel = false;
            this.Gv_InfoRecord.OptionsView.ShowIndicator = false;
            this.Gv_InfoRecord.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.Gv_InfoRecord_RowCellClick);
            this.Gv_InfoRecord.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.Gv_InfoRecord_CustomUnboundColumnData);
            this.Gv_InfoRecord.Click += new System.EventHandler(this.Gv_InfoRecord_Click);
            this.Gv_InfoRecord.DoubleClick += new System.EventHandler(this.Gv_InfoRecord_DoubleClick);
            // 
            // Gv_IR_STT
            // 
            this.Gv_IR_STT.Caption = "Trạng thái";
            this.Gv_IR_STT.ColumnEdit = this.repositoryItemPictureEdit2;
            this.Gv_IR_STT.FieldName = "STT";
            this.Gv_IR_STT.Name = "Gv_IR_STT";
            this.Gv_IR_STT.OptionsColumn.AllowEdit = false;
            this.Gv_IR_STT.OptionsColumn.ShowCaption = false;
            this.Gv_IR_STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gv_IR_STT.Visible = true;
            this.Gv_IR_STT.VisibleIndex = 0;
            this.Gv_IR_STT.Width = 25;
            // 
            // repositoryItemPictureEdit2
            // 
            this.repositoryItemPictureEdit2.Name = "repositoryItemPictureEdit2";
            this.repositoryItemPictureEdit2.NullText = " ";
            // 
            // Gv_IR_Code
            // 
            this.Gv_IR_Code.Caption = "Mã";
            this.Gv_IR_Code.FieldName = "CODE";
            this.Gv_IR_Code.Name = "Gv_IR_Code";
            this.Gv_IR_Code.OptionsColumn.AllowEdit = false;
            this.Gv_IR_Code.Visible = true;
            this.Gv_IR_Code.VisibleIndex = 1;
            this.Gv_IR_Code.Width = 110;
            // 
            // Gv_IR_Type
            // 
            this.Gv_IR_Type.Caption = "Loại";
            this.Gv_IR_Type.FieldName = "TYPE";
            this.Gv_IR_Type.Name = "Gv_IR_Type";
            this.Gv_IR_Type.OptionsColumn.AllowEdit = false;
            this.Gv_IR_Type.Visible = true;
            this.Gv_IR_Type.VisibleIndex = 2;
            this.Gv_IR_Type.Width = 120;
            // 
            // Gv_IR_CreateTime
            // 
            this.Gv_IR_CreateTime.Caption = "Thời gian";
            this.Gv_IR_CreateTime.FieldName = "CREATE_TIME_STR";
            this.Gv_IR_CreateTime.Name = "Gv_IR_CreateTime";
            this.Gv_IR_CreateTime.OptionsColumn.AllowEdit = false;
            this.Gv_IR_CreateTime.Visible = true;
            this.Gv_IR_CreateTime.VisibleIndex = 3;
            this.Gv_IR_CreateTime.Width = 150;
            // 
            // Gv_IR_DepartmentName
            // 
            this.Gv_IR_DepartmentName.Caption = "Khoa";
            this.Gv_IR_DepartmentName.FieldName = "DEPARTMENT_NAME";
            this.Gv_IR_DepartmentName.Name = "Gv_IR_DepartmentName";
            this.Gv_IR_DepartmentName.OptionsColumn.AllowEdit = false;
            this.Gv_IR_DepartmentName.Visible = true;
            this.Gv_IR_DepartmentName.VisibleIndex = 4;
            this.Gv_IR_DepartmentName.Width = 200;
            // 
            // Gv_IR_Creator
            // 
            this.Gv_IR_Creator.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Underline);
            this.Gv_IR_Creator.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
            this.Gv_IR_Creator.AppearanceCell.Options.UseFont = true;
            this.Gv_IR_Creator.AppearanceCell.Options.UseForeColor = true;
            this.Gv_IR_Creator.Caption = "Người tạo";
            this.Gv_IR_Creator.FieldName = "CREATOR";
            this.Gv_IR_Creator.Name = "Gv_IR_Creator";
            this.Gv_IR_Creator.OptionsColumn.AllowEdit = false;
            this.Gv_IR_Creator.ToolTip = "Click vào để xem thông tin chi tiết";
            this.Gv_IR_Creator.Visible = true;
            this.Gv_IR_Creator.VisibleIndex = 5;
            this.Gv_IR_Creator.Width = 150;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            this.repositoryItemTextEdit1.ReadOnly = true;
            // 
            // GcEmrDocumentType
            // 
            this.GcEmrDocumentType.Location = new System.Drawing.Point(2, 291);
            this.GcEmrDocumentType.MainView = this.GvEmrDocumentType;
            this.GcEmrDocumentType.Name = "GcEmrDocumentType";
            this.GcEmrDocumentType.Size = new System.Drawing.Size(216, 313);
            this.GcEmrDocumentType.TabIndex = 19;
            this.GcEmrDocumentType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.GvEmrDocumentType});
            // 
            // GvEmrDocumentType
            // 
            this.GvEmrDocumentType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gr_Gc_Name});
            this.GvEmrDocumentType.GridControl = this.GcEmrDocumentType;
            this.GvEmrDocumentType.Name = "GvEmrDocumentType";
            this.GvEmrDocumentType.OptionsView.ShowColumnHeaders = false;
            this.GvEmrDocumentType.OptionsView.ShowGroupPanel = false;
            this.GvEmrDocumentType.OptionsView.ShowIndicator = false;
            this.GvEmrDocumentType.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.GvEmrDocumentType_RowCellClick);
            this.GvEmrDocumentType.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.GvEmrDocumentType_RowCellStyle);
            // 
            // Gr_Gc_Name
            // 
            this.Gr_Gc_Name.Caption = "Loại văn bản";
            this.Gr_Gc_Name.FieldName = "DOCUMENT_TYPE_NAME";
            this.Gr_Gc_Name.Name = "Gr_Gc_Name";
            this.Gr_Gc_Name.OptionsColumn.AllowEdit = false;
            this.Gr_Gc_Name.Visible = true;
            this.Gr_Gc_Name.VisibleIndex = 0;
            // 
            // LblNote
            // 
            this.LblNote.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblNote.Location = new System.Drawing.Point(100, 217);
            this.LblNote.Name = "LblNote";
            this.LblNote.Size = new System.Drawing.Size(890, 20);
            this.LblNote.StyleController = this.layoutControl1;
            this.LblNote.TabIndex = 18;
            // 
            // LblSubIcd
            // 
            this.LblSubIcd.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblSubIcd.Location = new System.Drawing.Point(100, 145);
            this.LblSubIcd.Name = "LblSubIcd";
            this.LblSubIcd.Size = new System.Drawing.Size(890, 20);
            this.LblSubIcd.StyleController = this.layoutControl1;
            this.LblSubIcd.TabIndex = 17;
            // 
            // LblMainIcd
            // 
            this.LblMainIcd.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblMainIcd.Location = new System.Drawing.Point(100, 121);
            this.LblMainIcd.Name = "LblMainIcd";
            this.LblMainIcd.Size = new System.Drawing.Size(890, 20);
            this.LblMainIcd.StyleController = this.layoutControl1;
            this.LblMainIcd.TabIndex = 16;
            // 
            // LblAddress
            // 
            this.LblAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblAddress.Location = new System.Drawing.Point(100, 97);
            this.LblAddress.Name = "LblAddress";
            this.LblAddress.Size = new System.Drawing.Size(890, 20);
            this.LblAddress.StyleController = this.layoutControl1;
            this.LblAddress.TabIndex = 15;
            // 
            // LblMediOrg
            // 
            this.LblMediOrg.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblMediOrg.Location = new System.Drawing.Point(846, 73);
            this.LblMediOrg.Name = "LblMediOrg";
            this.LblMediOrg.Size = new System.Drawing.Size(144, 20);
            this.LblMediOrg.StyleController = this.layoutControl1;
            this.LblMediOrg.TabIndex = 13;
            // 
            // LblHeinTime
            // 
            this.LblHeinTime.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblHeinTime.Location = new System.Drawing.Point(624, 73);
            this.LblHeinTime.Name = "LblHeinTime";
            this.LblHeinTime.Size = new System.Drawing.Size(123, 20);
            this.LblHeinTime.StyleController = this.layoutControl1;
            this.LblHeinTime.TabIndex = 12;
            // 
            // LblHeinNumber
            // 
            this.LblHeinNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblHeinNumber.Location = new System.Drawing.Point(349, 73);
            this.LblHeinNumber.Name = "LblHeinNumber";
            this.LblHeinNumber.Size = new System.Drawing.Size(176, 20);
            this.LblHeinNumber.StyleController = this.layoutControl1;
            this.LblHeinNumber.TabIndex = 11;
            // 
            // LblPatientType
            // 
            this.LblPatientType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblPatientType.Location = new System.Drawing.Point(100, 73);
            this.LblPatientType.Name = "LblPatientType";
            this.LblPatientType.Size = new System.Drawing.Size(150, 20);
            this.LblPatientType.StyleController = this.layoutControl1;
            this.LblPatientType.TabIndex = 10;
            // 
            // LblGender
            // 
            this.LblGender.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblGender.Location = new System.Drawing.Point(846, 49);
            this.LblGender.Name = "LblGender";
            this.LblGender.Size = new System.Drawing.Size(144, 20);
            this.LblGender.StyleController = this.layoutControl1;
            this.LblGender.TabIndex = 9;
            // 
            // LblDob
            // 
            this.LblDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblDob.Location = new System.Drawing.Point(624, 49);
            this.LblDob.Name = "LblDob";
            this.LblDob.Size = new System.Drawing.Size(123, 20);
            this.LblDob.StyleController = this.layoutControl1;
            this.LblDob.TabIndex = 8;
            // 
            // LblPatientName
            // 
            this.LblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblPatientName.Location = new System.Drawing.Point(349, 49);
            this.LblPatientName.Name = "LblPatientName";
            this.LblPatientName.Size = new System.Drawing.Size(176, 20);
            this.LblPatientName.StyleController = this.layoutControl1;
            this.LblPatientName.TabIndex = 7;
            // 
            // LblPatientCode
            // 
            this.LblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblPatientCode.Location = new System.Drawing.Point(100, 49);
            this.LblPatientCode.Name = "LblPatientCode";
            this.LblPatientCode.Size = new System.Drawing.Size(150, 20);
            this.LblPatientCode.StyleController = this.layoutControl1;
            this.LblPatientCode.TabIndex = 6;
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(287, 2);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(139, 22);
            this.BtnSearch.StyleController = this.layoutControl1;
            this.BtnSearch.TabIndex = 5;
            this.BtnSearch.Text = "Tìm (Ctrl F)";
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // TxtTreatmentCode
            // 
            this.TxtTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.TxtTreatmentCode.Name = "TxtTreatmentCode";
            this.TxtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.TxtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.TxtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.TxtTreatmentCode.Size = new System.Drawing.Size(281, 20);
            this.TxtTreatmentCode.StyleController = this.layoutControl1;
            this.TxtTreatmentCode.TabIndex = 4;
            this.TxtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtTreatmentCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.LcgPatientInfo,
            this.layoutControlItem3,
            this.LciTotalInfo,
            this.layoutControlItem5,
            this.layoutControlItem4,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem2,
            this.layoutControlItem9,
            this.emptySpaceItem3,
            this.lciGC_Treatment,
            this.layoutControlItem11,
            this.layoutControlItem10});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1264, 632);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.TxtTreatmentCode;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(285, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.BtnSearch;
            this.layoutControlItem2.Location = new System.Drawing.Point(285, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(143, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(428, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(836, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // LcgPatientInfo
            // 
            this.LcgPatientInfo.ExpandButtonVisible = true;
            this.LcgPatientInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LciPatientCode,
            this.LciPatientName,
            this.LciDob,
            this.LciGender,
            this.LciPatientType,
            this.LciHeinNumber,
            this.LciAddress,
            this.LciMainIcd,
            this.LciSubIcd,
            this.LciNote,
            this.LciHeinTime,
            this.LciMediOrg,
            this.LciIcdYhct,
            this.LciSubIcdYhct,
            this.layoutControlItem8});
            this.LcgPatientInfo.Location = new System.Drawing.Point(0, 26);
            this.LcgPatientInfo.Name = "LcgPatientInfo";
            this.LcgPatientInfo.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.LcgPatientInfo.Size = new System.Drawing.Size(995, 240);
            this.LcgPatientInfo.Text = "Thông tin bệnh nhân";
            // 
            // LciPatientCode
            // 
            this.LciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciPatientCode.Control = this.LblPatientCode;
            this.LciPatientCode.Location = new System.Drawing.Point(0, 0);
            this.LciPatientCode.Name = "LciPatientCode";
            this.LciPatientCode.OptionsToolTip.ToolTip = "Mã bệnh nhân";
            this.LciPatientCode.Size = new System.Drawing.Size(249, 24);
            this.LciPatientCode.Text = "Mã BN:";
            this.LciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciPatientCode.TextSize = new System.Drawing.Size(90, 20);
            this.LciPatientCode.TextToControlDistance = 5;
            // 
            // LciPatientName
            // 
            this.LciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciPatientName.Control = this.LblPatientName;
            this.LciPatientName.Location = new System.Drawing.Point(249, 0);
            this.LciPatientName.Name = "LciPatientName";
            this.LciPatientName.OptionsToolTip.ToolTip = "Tên bệnh nhân";
            this.LciPatientName.Size = new System.Drawing.Size(275, 24);
            this.LciPatientName.Text = "Tên BN:";
            this.LciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.LciPatientName.TextToControlDistance = 5;
            // 
            // LciDob
            // 
            this.LciDob.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciDob.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciDob.Control = this.LblDob;
            this.LciDob.Location = new System.Drawing.Point(524, 0);
            this.LciDob.Name = "LciDob";
            this.LciDob.Size = new System.Drawing.Size(222, 24);
            this.LciDob.Text = "Ngày sinh:";
            this.LciDob.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciDob.TextSize = new System.Drawing.Size(90, 20);
            this.LciDob.TextToControlDistance = 5;
            // 
            // LciGender
            // 
            this.LciGender.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciGender.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciGender.Control = this.LblGender;
            this.LciGender.Location = new System.Drawing.Point(746, 0);
            this.LciGender.Name = "LciGender";
            this.LciGender.Size = new System.Drawing.Size(243, 24);
            this.LciGender.Text = "Giới tính:";
            this.LciGender.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciGender.TextSize = new System.Drawing.Size(90, 20);
            this.LciGender.TextToControlDistance = 5;
            // 
            // LciPatientType
            // 
            this.LciPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciPatientType.Control = this.LblPatientType;
            this.LciPatientType.Location = new System.Drawing.Point(0, 24);
            this.LciPatientType.Name = "LciPatientType";
            this.LciPatientType.Size = new System.Drawing.Size(249, 24);
            this.LciPatientType.Text = "Đối tượng:";
            this.LciPatientType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciPatientType.TextSize = new System.Drawing.Size(90, 20);
            this.LciPatientType.TextToControlDistance = 5;
            // 
            // LciHeinNumber
            // 
            this.LciHeinNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciHeinNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciHeinNumber.Control = this.LblHeinNumber;
            this.LciHeinNumber.Location = new System.Drawing.Point(249, 24);
            this.LciHeinNumber.Name = "LciHeinNumber";
            this.LciHeinNumber.Size = new System.Drawing.Size(275, 24);
            this.LciHeinNumber.Text = "Số thẻ BHYT:";
            this.LciHeinNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciHeinNumber.TextSize = new System.Drawing.Size(90, 20);
            this.LciHeinNumber.TextToControlDistance = 5;
            // 
            // LciAddress
            // 
            this.LciAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciAddress.Control = this.LblAddress;
            this.LciAddress.Location = new System.Drawing.Point(0, 48);
            this.LciAddress.Name = "LciAddress";
            this.LciAddress.Size = new System.Drawing.Size(989, 24);
            this.LciAddress.Text = "Địa chỉ:";
            this.LciAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciAddress.TextSize = new System.Drawing.Size(90, 20);
            this.LciAddress.TextToControlDistance = 5;
            // 
            // LciMainIcd
            // 
            this.LciMainIcd.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciMainIcd.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciMainIcd.Control = this.LblMainIcd;
            this.LciMainIcd.Location = new System.Drawing.Point(0, 72);
            this.LciMainIcd.Name = "LciMainIcd";
            this.LciMainIcd.Size = new System.Drawing.Size(989, 24);
            this.LciMainIcd.Text = "Chẩn đoán chính:";
            this.LciMainIcd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciMainIcd.TextSize = new System.Drawing.Size(90, 20);
            this.LciMainIcd.TextToControlDistance = 5;
            // 
            // LciSubIcd
            // 
            this.LciSubIcd.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciSubIcd.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciSubIcd.Control = this.LblSubIcd;
            this.LciSubIcd.Location = new System.Drawing.Point(0, 96);
            this.LciSubIcd.Name = "LciSubIcd";
            this.LciSubIcd.Size = new System.Drawing.Size(989, 24);
            this.LciSubIcd.Text = "Chẩn đoán phụ:";
            this.LciSubIcd.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciSubIcd.TextSize = new System.Drawing.Size(90, 20);
            this.LciSubIcd.TextToControlDistance = 5;
            // 
            // LciNote
            // 
            this.LciNote.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciNote.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciNote.Control = this.LblNote;
            this.LciNote.Location = new System.Drawing.Point(0, 168);
            this.LciNote.Name = "LciNote";
            this.LciNote.Size = new System.Drawing.Size(989, 24);
            this.LciNote.Text = "Ghi chú ra viện:";
            this.LciNote.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciNote.TextSize = new System.Drawing.Size(90, 20);
            this.LciNote.TextToControlDistance = 5;
            // 
            // LciHeinTime
            // 
            this.LciHeinTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciHeinTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciHeinTime.Control = this.LblHeinTime;
            this.LciHeinTime.Location = new System.Drawing.Point(524, 24);
            this.LciHeinTime.Name = "LciHeinTime";
            this.LciHeinTime.Size = new System.Drawing.Size(222, 24);
            this.LciHeinTime.Text = "Hạn thẻ:";
            this.LciHeinTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciHeinTime.TextSize = new System.Drawing.Size(90, 20);
            this.LciHeinTime.TextToControlDistance = 5;
            // 
            // LciMediOrg
            // 
            this.LciMediOrg.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciMediOrg.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciMediOrg.Control = this.LblMediOrg;
            this.LciMediOrg.Location = new System.Drawing.Point(746, 24);
            this.LciMediOrg.Name = "LciMediOrg";
            this.LciMediOrg.OptionsToolTip.ToolTip = "Nơi đăng ký khám chữa bệnh ban đầu";
            this.LciMediOrg.Size = new System.Drawing.Size(243, 24);
            this.LciMediOrg.Text = "KCBBĐ:";
            this.LciMediOrg.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciMediOrg.TextSize = new System.Drawing.Size(90, 20);
            this.LciMediOrg.TextToControlDistance = 5;
            // 
            // LciIcdYhct
            // 
            this.LciIcdYhct.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciIcdYhct.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciIcdYhct.Control = this.LblIcdYhct;
            this.LciIcdYhct.Location = new System.Drawing.Point(0, 120);
            this.LciIcdYhct.Name = "LciIcdYhct";
            this.LciIcdYhct.OptionsToolTip.ToolTip = "Chẩn đoán y học cổ truyền";
            this.LciIcdYhct.Size = new System.Drawing.Size(989, 24);
            this.LciIcdYhct.Text = "CĐ YHCT:";
            this.LciIcdYhct.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciIcdYhct.TextSize = new System.Drawing.Size(90, 20);
            this.LciIcdYhct.TextToControlDistance = 5;
            // 
            // LciSubIcdYhct
            // 
            this.LciSubIcdYhct.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciSubIcdYhct.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciSubIcdYhct.Control = this.LblSubIcdYhct;
            this.LciSubIcdYhct.Location = new System.Drawing.Point(0, 144);
            this.LciSubIcdYhct.Name = "LciSubIcdYhct";
            this.LciSubIcdYhct.OptionsToolTip.ToolTip = "Chẩn đoán y học cổ truyền kèm theo";
            this.LciSubIcdYhct.Size = new System.Drawing.Size(989, 24);
            this.LciSubIcdYhct.Text = "CĐ YHCT phụ:";
            this.LciSubIcdYhct.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciSubIcdYhct.TextSize = new System.Drawing.Size(90, 20);
            this.LciSubIcdYhct.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem8.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem8.Control = this.lblStatus;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 192);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(989, 24);
            this.layoutControlItem8.Text = "Trạng thái:";
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem8.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.GcEmrDocumentType;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 289);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(220, 0);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(220, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(220, 317);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // LciTotalInfo
            // 
            this.LciTotalInfo.Control = this.Gc_InfoRecord;
            this.LciTotalInfo.Location = new System.Drawing.Point(220, 289);
            this.LciTotalInfo.Name = "LciTotalInfo";
            this.LciTotalInfo.Size = new System.Drawing.Size(456, 317);
            this.LciTotalInfo.TextSize = new System.Drawing.Size(0, 0);
            this.LciTotalInfo.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.Gc_EmrDocument;
            this.layoutControlItem5.Location = new System.Drawing.Point(676, 289);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(588, 317);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnKhongDat;
            this.layoutControlItem4.Location = new System.Drawing.Point(822, 606);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(148, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnDat;
            this.layoutControlItem6.Location = new System.Drawing.Point(970, 606);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(128, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnHuyDuyet;
            this.layoutControlItem7.Location = new System.Drawing.Point(1098, 606);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(166, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 606);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(822, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.chkUuTien;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 266);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(128, 23);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(371, 266);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(893, 23);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciGC_Treatment
            // 
            this.lciGC_Treatment.Control = this.Gc_Treatment;
            this.lciGC_Treatment.Location = new System.Drawing.Point(995, 26);
            this.lciGC_Treatment.Name = "lciGC_Treatment";
            this.lciGC_Treatment.Size = new System.Drawing.Size(269, 240);
            this.lciGC_Treatment.TextSize = new System.Drawing.Size(0, 0);
            this.lciGC_Treatment.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.chkToiTao;
            this.layoutControlItem11.Location = new System.Drawing.Point(128, 266);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(113, 23);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.chkIncludeCancelDoc;
            this.layoutControlItem10.Location = new System.Drawing.Point(241, 266);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(130, 23);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "NotApprova.png");
            this.imageList1.Images.SetKeyName(1, "ky.png");
            this.imageList1.Images.SetKeyName(2, "02.png");
            this.imageList1.Images.SetKeyName(3, "03.png");
            this.imageList1.Images.SetKeyName(4, "04.png");
            this.imageList1.Images.SetKeyName(5, "05.png");
            // 
            // FormHisTreatmentRecordChecking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 661);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormHisTreatmentRecordChecking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tra soát hồ sơ bệnh án";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormHisTreatmentRecordChecking_Load);
            this.Resize += new System.EventHandler(this.FormHisTreatmentRecordChecking_Resize);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkIncludeCancelDoc.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkToiTao.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_Treatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_Treatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUuTien.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_EmrDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_EmrDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gc_InfoRecord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Gv_InfoRecord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GcEmrDocumentType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GvEmrDocumentType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LcgPatientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciDob)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciGender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciHeinNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciMainIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSubIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciHeinTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciMediOrg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciIcdYhct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciSubIcdYhct)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTotalInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGC_Treatment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton BtnSearch;
        private DevExpress.XtraEditors.TextEdit TxtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.LabelControl LblGender;
        private DevExpress.XtraEditors.LabelControl LblDob;
        private DevExpress.XtraEditors.LabelControl LblPatientName;
        private DevExpress.XtraEditors.LabelControl LblPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem LciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem LciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem LciDob;
        private DevExpress.XtraLayout.LayoutControlItem LciGender;
        private DevExpress.XtraLayout.LayoutControlGroup LcgPatientInfo;
        private DevExpress.XtraEditors.LabelControl LblPatientType;
        private DevExpress.XtraLayout.LayoutControlItem LciPatientType;
        private DevExpress.XtraEditors.LabelControl LblHeinNumber;
        private DevExpress.XtraLayout.LayoutControlItem LciHeinNumber;
        private DevExpress.XtraEditors.LabelControl LblMediOrg;
        private DevExpress.XtraEditors.LabelControl LblHeinTime;
        private DevExpress.XtraLayout.LayoutControlItem LciHeinTime;
        private DevExpress.XtraLayout.LayoutControlItem LciMediOrg;
        private DevExpress.XtraEditors.LabelControl LblAddress;
        private DevExpress.XtraLayout.LayoutControlItem LciAddress;
        private DevExpress.XtraEditors.LabelControl LblNote;
        private DevExpress.XtraEditors.LabelControl LblSubIcd;
        private DevExpress.XtraEditors.LabelControl LblMainIcd;
        private DevExpress.XtraLayout.LayoutControlItem LciMainIcd;
        private DevExpress.XtraLayout.LayoutControlItem LciSubIcd;
        private DevExpress.XtraLayout.LayoutControlItem LciNote;
        private DevExpress.XtraGrid.GridControl Gc_EmrDocument;
        private DevExpress.XtraGrid.Views.Grid.GridView Gv_EmrDocument;
        private DevExpress.XtraGrid.GridControl Gc_InfoRecord;
        private DevExpress.XtraGrid.Views.Grid.GridView Gv_InfoRecord;
        private DevExpress.XtraGrid.GridControl GcEmrDocumentType;
        private DevExpress.XtraGrid.Views.Grid.GridView GvEmrDocumentType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem LciTotalInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Columns.GridColumn Gr_Gc_Name;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_STT;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_Name;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_HisCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_STT;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_Code;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_Type;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_DepartmentName;
        private DevExpress.XtraEditors.LabelControl LblSubIcdYhct;
        private DevExpress.XtraEditors.LabelControl LblIcdYhct;
        private DevExpress.XtraLayout.LayoutControlItem LciIcdYhct;
        private DevExpress.XtraLayout.LayoutControlItem LciSubIcdYhct;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_View;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonView;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit2;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.SimpleButton btnHuyDuyet;
        private DevExpress.XtraEditors.SimpleButton btnDat;
        private DevExpress.XtraEditors.SimpleButton btnKhongDat;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarButtonItem bbtnKhongDat;
        private DevExpress.XtraBars.BarButtonItem bbtnDat;
        private DevExpress.XtraBars.BarButtonItem bbtnHuyDuyet;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_IR_Creator;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_Creator;
		private DevExpress.XtraEditors.CheckEdit chkUuTien;
		private DevExpress.XtraEditors.LabelControl lblStatus;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraGrid.GridControl Gc_Treatment;
        private DevExpress.XtraGrid.Views.Grid.GridView Gv_Treatment;
        private DevExpress.XtraLayout.LayoutControlItem lciGC_Treatment;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_Treatment_Gc_STT;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_Treatment_Gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_Treatment_Gc_PatientDOB;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_Treatment_Gc_TreatmentCode;
        private DevExpress.XtraEditors.CheckEdit chkToiTao;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_Signers;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_UnSigners;
        private DevExpress.XtraGrid.Columns.GridColumn Gv_ED_Gc_CreateTime;
        private DevExpress.XtraEditors.CheckEdit chkIncludeCancelDoc;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
    }
}
