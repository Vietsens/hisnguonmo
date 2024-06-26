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
namespace Inventec.UC.TreatmentRecord
{
    partial class UCTreatmentList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCTreatmentList));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.gridControlTreatmentList = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatmentList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.grdColTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColGenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColDOB = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColInComingTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColOutComingTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.imageListMIE = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.gridControlTreatmentList);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 550);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 512);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(436, 36);
            this.ucPaging1.TabIndex = 5;
            // 
            // gridControlTreatmentList
            // 
            this.gridControlTreatmentList.Location = new System.Drawing.Point(2, 2);
            this.gridControlTreatmentList.MainView = this.gridViewTreatmentList;
            this.gridControlTreatmentList.Name = "gridControlTreatmentList";
            this.gridControlTreatmentList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit1});
            this.gridControlTreatmentList.Size = new System.Drawing.Size(436, 506);
            this.gridControlTreatmentList.TabIndex = 4;
            this.gridControlTreatmentList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatmentList});
            // 
            // gridViewTreatmentList
            // 
            this.gridViewTreatmentList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColSTT,
            this.grdColStatus,
            this.grdColTreatmentCode,
            this.grdColPatientCode,
            this.grdColPatientName,
            this.grdColGenderName,
            this.grdColDOB,
            this.grdColInComingTime,
            this.grdColOutComingTime});
            this.gridViewTreatmentList.GridControl = this.gridControlTreatmentList;
            this.gridViewTreatmentList.Name = "gridViewTreatmentList";
            this.gridViewTreatmentList.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewTreatmentList.OptionsSelection.MultiSelect = true;
            this.gridViewTreatmentList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewTreatmentList.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewTreatmentList.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewTreatmentList.OptionsView.ColumnAutoWidth = false;
            this.gridViewTreatmentList.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatmentList.OptionsView.ShowIndicator = false;
            this.gridViewTreatmentList.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewTreatmentList_RowCellClick);
            this.gridViewTreatmentList.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewTreatmentList_PopupMenuShowing);
            this.gridViewTreatmentList.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewTreatmentList_SelectionChanged);
            this.gridViewTreatmentList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatmentList_CustomUnboundColumnData);
            this.gridViewTreatmentList.Click += new System.EventHandler(this.gridViewTreatmentList_Click);
            // 
            // grdColSTT
            // 
            this.grdColSTT.Caption = "STT";
            this.grdColSTT.FieldName = "STT";
            this.grdColSTT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColSTT.Name = "grdColSTT";
            this.grdColSTT.OptionsColumn.AllowEdit = false;
            this.grdColSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColSTT.Visible = true;
            this.grdColSTT.VisibleIndex = 1;
            this.grdColSTT.Width = 27;
            // 
            // grdColStatus
            // 
            this.grdColStatus.Caption = "Trạng thái";
            this.grdColStatus.ColumnEdit = this.repositoryItemPictureEdit1;
            this.grdColStatus.FieldName = "IMG_STATUS";
            this.grdColStatus.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColStatus.Name = "grdColStatus";
            this.grdColStatus.OptionsColumn.AllowEdit = false;
            this.grdColStatus.OptionsColumn.ShowCaption = false;
            this.grdColStatus.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColStatus.Visible = true;
            this.grdColStatus.VisibleIndex = 2;
            this.grdColStatus.Width = 20;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            // 
            // grdColTreatmentCode
            // 
            this.grdColTreatmentCode.Caption = "Mã điều trị";
            this.grdColTreatmentCode.FieldName = "TREATMENT_CODE";
            this.grdColTreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.grdColTreatmentCode.Name = "grdColTreatmentCode";
            this.grdColTreatmentCode.OptionsColumn.AllowEdit = false;
            this.grdColTreatmentCode.Visible = true;
            this.grdColTreatmentCode.VisibleIndex = 3;
            this.grdColTreatmentCode.Width = 93;
            // 
            // grdColPatientCode
            // 
            this.grdColPatientCode.Caption = "Mã bệnh nhân";
            this.grdColPatientCode.FieldName = "PATIENT_CODE";
            this.grdColPatientCode.Name = "grdColPatientCode";
            this.grdColPatientCode.OptionsColumn.AllowEdit = false;
            this.grdColPatientCode.Visible = true;
            this.grdColPatientCode.VisibleIndex = 4;
            this.grdColPatientCode.Width = 101;
            // 
            // grdColPatientName
            // 
            this.grdColPatientName.Caption = "Tên bệnh nhân";
            this.grdColPatientName.FieldName = "VIR_PATIENT_NAME";
            this.grdColPatientName.Name = "grdColPatientName";
            this.grdColPatientName.OptionsColumn.AllowEdit = false;
            this.grdColPatientName.Visible = true;
            this.grdColPatientName.VisibleIndex = 5;
            this.grdColPatientName.Width = 216;
            // 
            // grdColGenderName
            // 
            this.grdColGenderName.Caption = "Giới tính";
            this.grdColGenderName.FieldName = "GENDER_NAME";
            this.grdColGenderName.Name = "grdColGenderName";
            this.grdColGenderName.OptionsColumn.AllowEdit = false;
            this.grdColGenderName.Visible = true;
            this.grdColGenderName.VisibleIndex = 6;
            // 
            // grdColDOB
            // 
            this.grdColDOB.Caption = "Ngày sinh";
            this.grdColDOB.FieldName = "DOB_DISPLAY";
            this.grdColDOB.Name = "grdColDOB";
            this.grdColDOB.OptionsColumn.AllowEdit = false;
            this.grdColDOB.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColDOB.Visible = true;
            this.grdColDOB.VisibleIndex = 7;
            this.grdColDOB.Width = 76;
            // 
            // grdColInComingTime
            // 
            this.grdColInComingTime.Caption = "Ngày vào viện";
            this.grdColInComingTime.FieldName = "IN_COMING_TIME";
            this.grdColInComingTime.Name = "grdColInComingTime";
            this.grdColInComingTime.OptionsColumn.AllowEdit = false;
            this.grdColInComingTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColInComingTime.Visible = true;
            this.grdColInComingTime.VisibleIndex = 8;
            this.grdColInComingTime.Width = 90;
            // 
            // grdColOutComingTime
            // 
            this.grdColOutComingTime.Caption = "Ngày ra viện";
            this.grdColOutComingTime.FieldName = "OUT_COMING_TIME";
            this.grdColOutComingTime.Name = "grdColOutComingTime";
            this.grdColOutComingTime.OptionsColumn.AllowEdit = false;
            this.grdColOutComingTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grdColOutComingTime.Visible = true;
            this.grdColOutComingTime.VisibleIndex = 9;
            this.grdColOutComingTime.Width = 90;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(440, 550);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlTreatmentList;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(440, 510);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.ucPaging1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 510);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(440, 40);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // toolTipController1
            // 
            this.toolTipController1.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.MaxItemId = 0;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 550);
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 550);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 550);
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "arrow-34-16 (1).ico");
            this.imageListStatus.Images.SetKeyName(1, "arrow-34-16.ico");
            this.imageListStatus.Images.SetKeyName(2, "arrow-34-16 (3).ico");
            this.imageListStatus.Images.SetKeyName(3, "arrow-34-16 (2).ico");
            this.imageListStatus.Images.SetKeyName(4, "plus.png");
            // 
            // imageListMIE
            // 
            this.imageListMIE.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMIE.ImageStream")));
            this.imageListMIE.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMIE.Images.SetKeyName(0, "status_0.png");
            this.imageListMIE.Images.SetKeyName(1, "status_1a.png");
            this.imageListMIE.Images.SetKeyName(2, "status_3a.png");
            this.imageListMIE.Images.SetKeyName(3, "status_2a.png");
            // 
            // UCTreatmentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCTreatmentList";
            this.Size = new System.Drawing.Size(440, 550);
            this.Load += new System.EventHandler(this.UCTreatmentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlTreatmentList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatmentList;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn grdColSTT;
        private DevExpress.XtraGrid.Columns.GridColumn grdColStatus;
        private DevExpress.XtraGrid.Columns.GridColumn grdColTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColGenderName;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDOB;
        private DevExpress.XtraGrid.Columns.GridColumn grdColInComingTime;
        private DevExpress.XtraGrid.Columns.GridColumn grdColOutComingTime;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private System.Windows.Forms.ImageList imageListStatus;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        private System.Windows.Forms.ImageList imageListMIE;
    }
}
