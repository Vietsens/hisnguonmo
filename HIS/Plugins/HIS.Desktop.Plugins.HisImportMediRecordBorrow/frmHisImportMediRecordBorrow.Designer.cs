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
namespace HIS.Desktop.Plugins.HisImportMediRecordBorrow
{
    partial class frmHisImportMediRecordBorrow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHisImportMediRecordBorrow));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnShowLineError = new DevExpress.XtraEditors.SimpleButton();
            this.btnChooseFile = new DevExpress.XtraEditors.SimpleButton();
            this.btnImport = new DevExpress.XtraEditors.SimpleButton();
            this.btnDownLoadFile = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlData = new DevExpress.XtraGrid.GridControl();
            this.gridViewData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colError = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton_Delete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.colTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStoreCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDataStoreName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBorrowUsername = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBorrowPhone = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDepartmentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAppointmentTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButton_ER = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Delete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_ER)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnShowLineError);
            this.layoutControl1.Controls.Add(this.btnChooseFile);
            this.layoutControl1.Controls.Add(this.btnImport);
            this.layoutControl1.Controls.Add(this.btnDownLoadFile);
            this.layoutControl1.Controls.Add(this.gridControlData);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1040, 330);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnShowLineError
            // 
            this.btnShowLineError.Location = new System.Drawing.Point(180, 2);
            this.btnShowLineError.Name = "btnShowLineError";
            this.btnShowLineError.Size = new System.Drawing.Size(85, 22);
            this.btnShowLineError.StyleController = this.layoutControl1;
            this.btnShowLineError.TabIndex = 2;
            this.btnShowLineError.Text = "Dòng lỗi";
            this.btnShowLineError.Click += new System.EventHandler(this.btnShowLineError_Click);
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Location = new System.Drawing.Point(91, 2);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(85, 22);
            this.btnChooseFile.StyleController = this.layoutControl1;
            this.btnChooseFile.TabIndex = 1;
            this.btnChooseFile.Text = "Import";
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(269, 2);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(85, 22);
            this.btnImport.StyleController = this.layoutControl1;
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Lưu (Ctrl S)";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnDownLoadFile
            // 
            this.btnDownLoadFile.Location = new System.Drawing.Point(2, 2);
            this.btnDownLoadFile.Name = "btnDownLoadFile";
            this.btnDownLoadFile.Size = new System.Drawing.Size(85, 22);
            this.btnDownLoadFile.StyleController = this.layoutControl1;
            this.btnDownLoadFile.TabIndex = 0;
            this.btnDownLoadFile.Text = "Tải file mẫu";
            this.btnDownLoadFile.Click += new System.EventHandler(this.btnDownLoadFile_Click);
            // 
            // gridControlData
            // 
            this.gridControlData.Location = new System.Drawing.Point(2, 28);
            this.gridControlData.MainView = this.gridViewData;
            this.gridControlData.Name = "gridControlData";
            this.gridControlData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButton_Delete,
            this.repositoryItemButton_ER});
            this.gridControlData.Size = new System.Drawing.Size(1036, 300);
            this.gridControlData.TabIndex = 4;
            this.gridControlData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewData});
            // 
            // gridViewData
            // 
            this.gridViewData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSTT,
            this.colError,
            this.colDelete,
            this.colTreatmentCode,
            this.colPatientCode,
            this.colStoreCode,
            this.colDataStoreName,
            this.colPatientName,
            this.colBorrowUsername,
            this.colBorrowPhone,
            this.colDepartmentName,
            this.colAppointmentTime});
            this.gridViewData.GridControl = this.gridControlData;
            this.gridViewData.Name = "gridViewData";
            this.gridViewData.OptionsView.ShowGroupPanel = false;
            this.gridViewData.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewData_CustomRowCellEdit);
            this.gridViewData.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewData_CustomUnboundColumnData);
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT_";
            this.colSTT.Name = "colSTT";
            this.colSTT.OptionsColumn.AllowEdit = false;
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 40;
            // 
            // colError
            // 
            this.colError.Caption = "Lỗi";
            this.colError.FieldName = "ERROR_";
            this.colError.Name = "colError";
            this.colError.OptionsColumn.ShowCaption = false;
            this.colError.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.colError.Visible = true;
            this.colError.VisibleIndex = 1;
            this.colError.Width = 30;
            // 
            // colDelete
            // 
            this.colDelete.Caption = "Xóa";
            this.colDelete.ColumnEdit = this.repositoryItemButton_Delete;
            this.colDelete.FieldName = "DELETE_";
            this.colDelete.Name = "colDelete";
            this.colDelete.OptionsColumn.ShowCaption = false;
            this.colDelete.Visible = true;
            this.colDelete.VisibleIndex = 2;
            this.colDelete.Width = 30;
            // 
            // repositoryItemButton_Delete
            // 
            this.repositoryItemButton_Delete.AutoHeight = false;
            this.repositoryItemButton_Delete.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.repositoryItemButton_Delete.Name = "repositoryItemButton_Delete";
            this.repositoryItemButton_Delete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton_Delete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton_Delete_ButtonClick);
            // 
            // colTreatmentCode
            // 
            this.colTreatmentCode.Caption = "Mã điều trị";
            this.colTreatmentCode.FieldName = "TREATMENT_CODE";
            this.colTreatmentCode.Name = "colTreatmentCode";
            this.colTreatmentCode.OptionsColumn.AllowEdit = false;
            this.colTreatmentCode.Visible = true;
            this.colTreatmentCode.VisibleIndex = 3;
            this.colTreatmentCode.Width = 90;
            // 
            // colPatientCode
            // 
            this.colPatientCode.Caption = "Mã bệnh nhân";
            this.colPatientCode.FieldName = "PATIENT_CODE";
            this.colPatientCode.Name = "colPatientCode";
            this.colPatientCode.OptionsColumn.AllowEdit = false;
            this.colPatientCode.Visible = true;
            this.colPatientCode.VisibleIndex = 4;
            this.colPatientCode.Width = 90;
            // 
            // colStoreCode
            // 
            this.colStoreCode.Caption = "Mã lưu trữ";
            this.colStoreCode.FieldName = "MEDI_RECORD_STORE_CODE";
            this.colStoreCode.Name = "colStoreCode";
            this.colStoreCode.OptionsColumn.AllowEdit = false;
            this.colStoreCode.Visible = true;
            this.colStoreCode.VisibleIndex = 5;
            this.colStoreCode.Width = 110;
            // 
            // colDataStoreName
            // 
            this.colDataStoreName.Caption = "Kho lưu trữ";
            this.colDataStoreName.FieldName = "DATA_STORE_NAME";
            this.colDataStoreName.Name = "colDataStoreName";
            this.colDataStoreName.OptionsColumn.AllowEdit = false;
            this.colDataStoreName.Visible = true;
            this.colDataStoreName.VisibleIndex = 6;
            this.colDataStoreName.Width = 100;
            // 
            // colPatientName
            // 
            this.colPatientName.Caption = "Tên bệnh nhân";
            this.colPatientName.FieldName = "PATIENT_NAME";
            this.colPatientName.Name = "colPatientName";
            this.colPatientName.OptionsColumn.AllowEdit = false;
            this.colPatientName.Visible = true;
            this.colPatientName.VisibleIndex = 7;
            this.colPatientName.Width = 140;
            // 
            // colBorrowUsername
            // 
            this.colBorrowUsername.Caption = "Người mượn";
            this.colBorrowUsername.FieldName = "BORROW_LOGINNAME";
            this.colBorrowUsername.Name = "colBorrowUsername";
            this.colBorrowUsername.OptionsColumn.AllowEdit = false;
            this.colBorrowUsername.Visible = true;
            this.colBorrowUsername.VisibleIndex = 8;
            this.colBorrowUsername.Width = 140;
            // 
            // colBorrowPhone
            // 
            this.colBorrowPhone.Caption = "Sđt người mượn";
            this.colBorrowPhone.FieldName = "BORROW_PHONE";
            this.colBorrowPhone.Name = "colBorrowPhone";
            this.colBorrowPhone.OptionsColumn.AllowEdit = false;
            this.colBorrowPhone.Visible = true;
            this.colBorrowPhone.VisibleIndex = 9;
            this.colBorrowPhone.Width = 90;
            // 
            // colDepartmentName
            // 
            this.colDepartmentName.Caption = "Khoa mượn";
            this.colDepartmentName.FieldName = "DEPARTMENT_NAME";
            this.colDepartmentName.Name = "colDepartmentName";
            this.colDepartmentName.OptionsColumn.AllowEdit = false;
            this.colDepartmentName.Visible = true;
            this.colDepartmentName.VisibleIndex = 10;
            this.colDepartmentName.Width = 120;
            // 
            // colAppointmentTime
            // 
            this.colAppointmentTime.Caption = "Thời gian hẹn trả";
            this.colAppointmentTime.FieldName = "APPOINTMENT_TIME_STR";
            this.colAppointmentTime.Name = "colAppointmentTime";
            this.colAppointmentTime.OptionsColumn.AllowEdit = false;
            this.colAppointmentTime.Visible = true;
            this.colAppointmentTime.VisibleIndex = 11;
            this.colAppointmentTime.Width = 120;
            // 
            // repositoryItemButton_ER
            // 
            this.repositoryItemButton_ER.AutoHeight = false;
            this.repositoryItemButton_ER.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButton_ER.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Hiển thị lỗi", null, null, true)});
            this.repositoryItemButton_ER.Name = "repositoryItemButton_ER";
            this.repositoryItemButton_ER.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton_ER.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton_ER_ButtonClick);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem2,
            this.layoutControlItem5,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1040, 330);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlData;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1040, 304);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(356, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(684, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnShowLineError;
            this.layoutControlItem5.Location = new System.Drawing.Point(178, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnDownLoadFile;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnChooseFile;
            this.layoutControlItem4.Location = new System.Drawing.Point(89, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnImport;
            this.layoutControlItem3.Location = new System.Drawing.Point(267, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
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
            this.barButtonItem1.Caption = "Lưu";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1040, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 359);
            this.barDockControlBottom.Size = new System.Drawing.Size(1040, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 330);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1040, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 330);
            // 
            // frmHisImportMediRecordBorrow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 359);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmHisImportMediRecordBorrow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nhập khẩu phiếu mượn bệnh án";
            this.Load += new System.EventHandler(this.frmHisImportMediRecordBorrow_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Delete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_ER)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnChooseFile;
        private DevExpress.XtraEditors.SimpleButton btnImport;
        private DevExpress.XtraEditors.SimpleButton btnDownLoadFile;
        private DevExpress.XtraEditors.SimpleButton btnShowLineError;
        private DevExpress.XtraGrid.GridControl gridControlData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewData;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colError;
        private DevExpress.XtraGrid.Columns.GridColumn colDelete;
        private DevExpress.XtraGrid.Columns.GridColumn colTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn colStoreCode;
        private DevExpress.XtraGrid.Columns.GridColumn colDataStoreName;
        private DevExpress.XtraGrid.Columns.GridColumn colPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn colBorrowUsername;
        private DevExpress.XtraGrid.Columns.GridColumn colBorrowPhone;
        private DevExpress.XtraGrid.Columns.GridColumn colDepartmentName;
        private DevExpress.XtraGrid.Columns.GridColumn colAppointmentTime;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton_Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton_ER;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
