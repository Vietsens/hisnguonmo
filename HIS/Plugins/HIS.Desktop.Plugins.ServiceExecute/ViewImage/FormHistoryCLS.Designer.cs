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
namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    partial class FormHistoryCLS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHistoryCLS));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlSereServHistory = new DevExpress.XtraGrid.GridControl();
            this.gridViewSereServHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Vỉew = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Gc_ServiceName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_IntructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_EndTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSereServHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSereServHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlSereServHistory);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(498, 369);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlSereServHistory
            // 
            this.gridControlSereServHistory.Location = new System.Drawing.Point(2, 2);
            this.gridControlSereServHistory.MainView = this.gridViewSereServHistory;
            this.gridControlSereServHistory.Name = "gridControlSereServHistory";
            this.gridControlSereServHistory.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonView});
            this.gridControlSereServHistory.Size = new System.Drawing.Size(494, 365);
            this.gridControlSereServHistory.TabIndex = 4;
            this.gridControlSereServHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSereServHistory});
            // 
            // gridViewSereServHistory
            // 
            this.gridViewSereServHistory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Vỉew,
            this.Gc_ServiceName,
            this.Gc_IntructionTime,
            this.Gc_EndTime});
            this.gridViewSereServHistory.GridControl = this.gridControlSereServHistory;
            this.gridViewSereServHistory.GroupCount = 1;
            this.gridViewSereServHistory.GroupFormat = "{0} [#image]{1} {2}";
            this.gridViewSereServHistory.Name = "gridViewSereServHistory";
            this.gridViewSereServHistory.OptionsView.ColumnAutoWidth = false;
            this.gridViewSereServHistory.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewSereServHistory.OptionsView.ShowGroupPanel = false;
            this.gridViewSereServHistory.OptionsView.ShowIndicator = false;
            this.gridViewSereServHistory.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.Gc_IntructionTime, DevExpress.Data.ColumnSortOrder.Descending)});
            this.gridViewSereServHistory.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gridViewSereServHistory_CustomDrawGroupRow);
            this.gridViewSereServHistory.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSereServHistory_CustomUnboundColumnData);
            // 
            // Gc_Vỉew
            // 
            this.Gc_Vỉew.Caption = "Xem";
            this.Gc_Vỉew.ColumnEdit = this.repositoryItemButtonView;
            this.Gc_Vỉew.FieldName = "View";
            this.Gc_Vỉew.Name = "Gc_Vỉew";
            this.Gc_Vỉew.OptionsColumn.ShowCaption = false;
            this.Gc_Vỉew.Visible = true;
            this.Gc_Vỉew.VisibleIndex = 0;
            this.Gc_Vỉew.Width = 41;
            // 
            // repositoryItemButtonView
            // 
            this.repositoryItemButtonView.AutoHeight = false;
            this.repositoryItemButtonView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonView.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Xem", null, null, true)});
            this.repositoryItemButtonView.Name = "repositoryItemButtonView";
            this.repositoryItemButtonView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonView.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonView_ButtonClick);
            // 
            // Gc_ServiceName
            // 
            this.Gc_ServiceName.Caption = "Dịch vụ";
            this.Gc_ServiceName.FieldName = "TDL_SERVICE_NAME";
            this.Gc_ServiceName.Name = "Gc_ServiceName";
            this.Gc_ServiceName.OptionsColumn.AllowEdit = false;
            this.Gc_ServiceName.Visible = true;
            this.Gc_ServiceName.VisibleIndex = 1;
            this.Gc_ServiceName.Width = 350;
            // 
            // Gc_IntructionTime
            // 
            this.Gc_IntructionTime.Caption = " ";
            this.Gc_IntructionTime.FieldName = "TIME_END";
            this.Gc_IntructionTime.Name = "Gc_IntructionTime";
            this.Gc_IntructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_IntructionTime.Visible = true;
            this.Gc_IntructionTime.VisibleIndex = 1;
            // 
            // Gc_EndTime
            // 
            this.Gc_EndTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_EndTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_EndTime.Caption = "Thời gian kết thúc";
            this.Gc_EndTime.FieldName = "END_TIME_STR";
            this.Gc_EndTime.Name = "Gc_EndTime";
            this.Gc_EndTime.OptionsColumn.AllowEdit = false;
            this.Gc_EndTime.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_EndTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_EndTime.Width = 120;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(498, 369);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlSereServHistory;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(498, 369);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // FormHistoryCLS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 369);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormHistoryCLS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lịch sử kết quả";
            this.Load += new System.EventHandler(this.FormHistoryCLS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSereServHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSereServHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlSereServHistory;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSereServHistory;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ServiceName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_IntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Vỉew;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonView;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_EndTime;
    }
}
