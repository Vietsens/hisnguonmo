namespace HIS.Desktop.Plugins.CallPatientDepartmentV1
{
    partial class FormConfigWaitingScreen
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tgExtendMonitor = new DevExpress.XtraEditors.ToggleSwitch();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Check = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckRoom = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.Gc_RoomCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_RoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_OrderIndex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSpinOrder = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.LblRoom = new DevExpress.XtraEditors.LabelControl();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.spinReloadTime = new DevExpress.XtraEditors.SpinEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tgExtendMonitor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tgExtendMonitor);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Controls.Add(this.LblRoom);
            this.layoutControl1.Controls.Add(this.txtSearch);
            this.layoutControl1.Controls.Add(this.spinReloadTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(550, 400);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tgExtendMonitor
            // 
            this.tgExtendMonitor.Location = new System.Drawing.Point(303, 374);
            this.tgExtendMonitor.Name = "tgExtendMonitor";
            this.tgExtendMonitor.Properties.OffText = "Bật màn hình mở rộng";
            this.tgExtendMonitor.Properties.OnText = "Bật màn hình mở rộng";
            this.tgExtendMonitor.Size = new System.Drawing.Size(245, 24);
            this.tgExtendMonitor.StyleController = this.layoutControl1;
            this.tgExtendMonitor.TabIndex = 6;
            this.tgExtendMonitor.Toggled += new System.EventHandler(this.tgExtendMonitor_Toggled);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(2, 49);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckRoom,
            this.repositoryItemSpinOrder});
            this.gridControl1.Size = new System.Drawing.Size(546, 321);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Check,
            this.Gc_RoomCode,
            this.Gc_RoomName,
            this.Gc_OrderIndex});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // Gc_Check
            // 
            this.Gc_Check.Caption = " ";
            this.Gc_Check.ColumnEdit = this.repositoryItemCheckRoom;
            this.Gc_Check.FieldName = "IsCheck";
            this.Gc_Check.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Check.Name = "Gc_Check";
            this.Gc_Check.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_Check.OptionsColumn.ShowCaption = false;
            this.Gc_Check.OptionsFilter.AllowFilter = false;
            this.Gc_Check.Visible = true;
            this.Gc_Check.VisibleIndex = 0;
            this.Gc_Check.Width = 30;
            // 
            // repositoryItemCheckRoom
            // 
            this.repositoryItemCheckRoom.AutoHeight = false;
            this.repositoryItemCheckRoom.Name = "repositoryItemCheckRoom";
            this.repositoryItemCheckRoom.CheckedChanged += new System.EventHandler(this.repositoryItemCheckRoom_CheckedChanged);
            // 
            // Gc_RoomCode
            // 
            this.Gc_RoomCode.Caption = "Mã";
            this.Gc_RoomCode.FieldName = "EXECUTE_ROOM_CODE";
            this.Gc_RoomCode.Name = "Gc_RoomCode";
            this.Gc_RoomCode.OptionsColumn.AllowEdit = false;
            this.Gc_RoomCode.Visible = true;
            this.Gc_RoomCode.VisibleIndex = 1;
            this.Gc_RoomCode.Width = 80;
            // 
            // Gc_RoomName
            // 
            this.Gc_RoomName.Caption = "Tên";
            this.Gc_RoomName.FieldName = "EXECUTE_ROOM_NAME";
            this.Gc_RoomName.Name = "Gc_RoomName";
            this.Gc_RoomName.OptionsColumn.AllowEdit = false;
            this.Gc_RoomName.Visible = true;
            this.Gc_RoomName.VisibleIndex = 2;
            this.Gc_RoomName.Width = 300;
            // 
            // Gc_OrderIndex
            // 
            this.Gc_OrderIndex.Caption = "Vị trí";
            this.Gc_OrderIndex.ColumnEdit = this.repositoryItemSpinOrder;
            this.Gc_OrderIndex.FieldName = "OrderIndex";
            this.Gc_OrderIndex.Name = "Gc_OrderIndex";
            this.Gc_OrderIndex.Visible = true;
            this.Gc_OrderIndex.VisibleIndex = 3;
            this.Gc_OrderIndex.Width = 60;
            // 
            // repositoryItemSpinOrder
            // 
            this.repositoryItemSpinOrder.AutoHeight = false;
            this.repositoryItemSpinOrder.IsFloatValue = false;
            this.repositoryItemSpinOrder.MaxValue = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.repositoryItemSpinOrder.Name = "repositoryItemSpinOrder";
            // 
            // LblRoom
            // 
            this.LblRoom.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.LblRoom.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.LblRoom.Location = new System.Drawing.Point(2, 2);
            this.LblRoom.Name = "LblRoom";
            this.LblRoom.Size = new System.Drawing.Size(546, 19);
            this.LblRoom.StyleController = this.layoutControl1;
            this.LblRoom.TabIndex = 4;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(2, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.NullValuePrompt = "Nhập từ khóa để tìm kiếm phòng thực hiện";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Size = new System.Drawing.Size(546, 20);
            this.txtSearch.StyleController = this.layoutControl1;
            this.txtSearch.TabIndex = 7;
            this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
            // 
            // spinReloadTime
            // 
            this.spinReloadTime.EditValue = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.spinReloadTime.Location = new System.Drawing.Point(111, 374);
            this.spinReloadTime.Name = "spinReloadTime";
            this.spinReloadTime.Properties.IsFloatValue = false;
            this.spinReloadTime.Properties.MaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.spinReloadTime.Properties.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinReloadTime.Size = new System.Drawing.Size(95, 20);
            this.spinReloadTime.StyleController = this.layoutControl1;
            this.spinReloadTime.TabIndex = 8;
            this.spinReloadTime.EditValueChanged += new System.EventHandler(this.spinReloadTime_EditValueChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(550, 400);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.LblRoom;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(550, 23);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 23);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(550, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 47);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(550, 325);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.spinReloadTime;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 372);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(208, 28);
            this.layoutControlItem4.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem4.Text = "Thời gian tải lại (giây):";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(106, 13);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.tgExtendMonitor;
            this.layoutControlItem5.Location = new System.Drawing.Point(301, 372);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(249, 28);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(208, 372);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(93, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // FormConfigWaitingScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 400);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormConfigWaitingScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thiết lập màn hình chờ";
            this.Load += new System.EventHandler(this.FormConfigWaitingScreen_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tgExtendMonitor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinReloadTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl LblRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.ToggleSwitch tgExtendMonitor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Check;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckRoom;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_RoomCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_RoomName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_OrderIndex;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinOrder;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.SpinEdit spinReloadTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
