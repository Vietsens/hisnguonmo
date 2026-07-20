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
namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    partial class frmImpMestAttachList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.components = new System.ComponentModel.Container();
            this.panelTop = new DevExpress.XtraEditors.PanelControl();
            this.lblCount = new DevExpress.XtraEditors.LabelControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnAttachNew = new DevExpress.XtraEditors.SimpleButton();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlDocList = new DevExpress.XtraGrid.GridControl();
            this.gridViewDocList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcView = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcEdit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcDelete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoBtnDelete = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gcDocName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).BeginInit();
            this.SuspendLayout();
            //
            // gridControlDocList
            //
            this.gridControlDocList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlDocList.Location = new System.Drawing.Point(0, 40);
            this.gridControlDocList.MainView = this.gridViewDocList;
            this.gridControlDocList.Name = "gridControlDocList";
            this.gridControlDocList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoBtnView,
            this.repoBtnEdit,
            this.repoBtnDelete});
            this.gridControlDocList.Size = new System.Drawing.Size(944, 424);
            this.gridControlDocList.TabIndex = 0;
            this.gridControlDocList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDocList});
            //
            // gridViewDocList
            //
            this.gridViewDocList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcStt,
            this.gcView,
            this.gcEdit,
            this.gcDelete,
            this.gcDocName,
            this.gcType,
            this.gcCreator,
            this.gcCreateTime,
            this.gcModifyTime,
            this.gcModifier});
            this.gridViewDocList.GridControl = this.gridControlDocList;
            this.gridViewDocList.Name = "gridViewDocList";
            this.gridViewDocList.OptionsBehavior.Editable = true;
            this.gridViewDocList.OptionsCustomization.AllowGroup = false;
            this.gridViewDocList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewDocList.OptionsView.ColumnAutoWidth = false;
            this.gridViewDocList.OptionsView.ShowGroupPanel = false;
            this.gridViewDocList.OptionsView.ShowIndicator = false;
            this.gridViewDocList.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDocList_CustomUnboundColumnData);
            //
            // gcStt
            //
            this.gcStt.Caption = "STT";
            this.gcStt.FieldName = "STT";
            this.gcStt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcStt.Name = "gcStt";
            this.gcStt.OptionsColumn.AllowEdit = false;
            this.gcStt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcStt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcStt.Visible = true;
            this.gcStt.VisibleIndex = 0;
            this.gcStt.Width = 40;
            //
            // gcView
            //
            this.gcView.Caption = "Xem";
            this.gcView.ColumnEdit = this.repoBtnView;
            this.gcView.FieldName = "VIEW";
            this.gcView.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcView.Name = "gcView";
            this.gcView.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcView.OptionsColumn.ShowCaption = false;
            this.gcView.Visible = true;
            this.gcView.VisibleIndex = 1;
            this.gcView.Width = 52;
            //
            // repoBtnView
            //
            this.repoBtnView.AutoHeight = false;
            this.repoBtnView.Name = "repoBtnView";
            this.repoBtnView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnView.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnView_ButtonClick);
            //
            // gcEdit
            //
            this.gcEdit.Caption = "Sửa";
            this.gcEdit.ColumnEdit = this.repoBtnEdit;
            this.gcEdit.FieldName = "EDIT";
            this.gcEdit.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcEdit.Name = "gcEdit";
            this.gcEdit.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcEdit.OptionsColumn.ShowCaption = false;
            this.gcEdit.Visible = true;
            this.gcEdit.VisibleIndex = 2;
            this.gcEdit.Width = 52;
            //
            // repoBtnEdit
            //
            this.repoBtnEdit.AutoHeight = false;
            this.repoBtnEdit.Name = "repoBtnEdit";
            this.repoBtnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnEdit_ButtonClick);
            //
            // gcDelete
            //
            this.gcDelete.Caption = "Xóa";
            this.gcDelete.ColumnEdit = this.repoBtnDelete;
            this.gcDelete.FieldName = "DELETE";
            this.gcDelete.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gcDelete.Name = "gcDelete";
            this.gcDelete.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gcDelete.OptionsColumn.ShowCaption = false;
            this.gcDelete.Visible = true;
            this.gcDelete.VisibleIndex = 3;
            this.gcDelete.Width = 52;
            //
            // repoBtnDelete
            //
            this.repoBtnDelete.AutoHeight = false;
            this.repoBtnDelete.Name = "repoBtnDelete";
            this.repoBtnDelete.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoBtnDelete.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repoBtnDelete_ButtonClick);
            //
            // gcDocName
            //
            this.gcDocName.Caption = "Tên văn bản";
            this.gcDocName.FieldName = "DOCUMENT_NAME";
            this.gcDocName.Name = "gcDocName";
            this.gcDocName.OptionsColumn.AllowEdit = false;
            this.gcDocName.Visible = true;
            this.gcDocName.VisibleIndex = 4;
            this.gcDocName.Width = 260;
            //
            // gcType
            //
            this.gcType.Caption = "Loại";
            this.gcType.FieldName = "LOAI";
            this.gcType.Name = "gcType";
            this.gcType.OptionsColumn.AllowEdit = false;
            this.gcType.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.gcType.Visible = true;
            this.gcType.VisibleIndex = 5;
            this.gcType.Width = 70;
            //
            // gcCreator
            //
            this.gcCreator.Caption = "Người đính kèm";
            this.gcCreator.FieldName = "CREATOR";
            this.gcCreator.Name = "gcCreator";
            this.gcCreator.OptionsColumn.AllowEdit = false;
            this.gcCreator.Visible = true;
            this.gcCreator.VisibleIndex = 6;
            this.gcCreator.Width = 120;
            //
            // gcCreateTime
            //
            this.gcCreateTime.Caption = "Thời gian đính kèm";
            this.gcCreateTime.FieldName = "CREATE_TIME_STR";
            this.gcCreateTime.Name = "gcCreateTime";
            this.gcCreateTime.OptionsColumn.AllowEdit = false;
            this.gcCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcCreateTime.Visible = true;
            this.gcCreateTime.VisibleIndex = 7;
            this.gcCreateTime.Width = 130;
            //
            // gcModifyTime
            //
            this.gcModifyTime.Caption = "Thời gian sửa";
            this.gcModifyTime.FieldName = "MODIFY_TIME_STR";
            this.gcModifyTime.Name = "gcModifyTime";
            this.gcModifyTime.OptionsColumn.AllowEdit = false;
            this.gcModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcModifyTime.Visible = true;
            this.gcModifyTime.VisibleIndex = 8;
            this.gcModifyTime.Width = 130;
            //
            // gcModifier
            //
            this.gcModifier.Caption = "Người sửa";
            this.gcModifier.FieldName = "MODIFIER";
            this.gcModifier.Name = "gcModifier";
            this.gcModifier.OptionsColumn.AllowEdit = false;
            this.gcModifier.Visible = true;
            this.gcModifier.VisibleIndex = 9;
            this.gcModifier.Width = 110;
            //
            // panelTop
            //
            this.panelTop.Controls.Add(this.lblCount);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.btnAttachNew);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(944, 40);
            this.panelTop.TabIndex = 1;
            //
            // btnAttachNew
            //
            this.btnAttachNew.Location = new System.Drawing.Point(8, 8);
            this.btnAttachNew.Name = "btnAttachNew";
            this.btnAttachNew.Size = new System.Drawing.Size(120, 26);
            this.btnAttachNew.TabIndex = 0;
            this.btnAttachNew.Text = "Đính kèm mới";
            this.btnAttachNew.Click += new System.EventHandler(this.btnAttachNew_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(134, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 26);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblCount
            //
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.Appearance.Options.UseTextOptions = true;
            this.lblCount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCount.Location = new System.Drawing.Point(744, 14);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(192, 13);
            this.lblCount.TabIndex = 2;
            this.lblCount.Text = "0 tài liệu";
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 464);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(944, 44);
            this.panelBottom.TabIndex = 2;
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(852, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 26);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // frmImpMestAttachList
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 508);
            this.Controls.Add(this.gridControlDocList);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelBottom);
            this.MinimumSize = new System.Drawing.Size(720, 360);
            this.Name = "frmImpMestAttachList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách tài liệu đính kèm";
            this.Load += new System.EventHandler(this.frmImpMestAttachList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelTop)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoBtnDelete)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelTop;
        private DevExpress.XtraEditors.SimpleButton btnAttachNew;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.LabelControl lblCount;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraGrid.GridControl gridControlDocList;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDocList;
        private DevExpress.XtraGrid.Columns.GridColumn gcStt;
        private DevExpress.XtraGrid.Columns.GridColumn gcView;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnView;
        private DevExpress.XtraGrid.Columns.GridColumn gcEdit;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gcDelete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoBtnDelete;
        private DevExpress.XtraGrid.Columns.GridColumn gcDocName;
        private DevExpress.XtraGrid.Columns.GridColumn gcType;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gcCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcModifier;
    }
}
