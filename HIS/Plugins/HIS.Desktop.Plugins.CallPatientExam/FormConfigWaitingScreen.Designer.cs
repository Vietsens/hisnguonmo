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
namespace HIS.Desktop.Plugins.CallPatientExam
{
    partial class FormConfigWaitingScreen
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.chkRoom = new DevExpress.XtraEditors.CheckEdit();
            this.tgExtendMonitor = new DevExpress.XtraEditors.ToggleSwitch();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.Gc_Check = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckRoom = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.Gc_RoomCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_RoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tgExtendMonitor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkRoom);
            this.layoutControl1.Controls.Add(this.tgExtendMonitor);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(534, 317);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkRoom
            // 
            this.chkRoom.Location = new System.Drawing.Point(12, 258);
            this.chkRoom.Name = "chkRoom";
            this.chkRoom.Properties.Caption = "Chỉ hiển thị phòng khám thuộc khoa người dùng đang làm việc";
            this.chkRoom.Size = new System.Drawing.Size(510, 19);
            this.chkRoom.StyleController = this.layoutControl1;
            this.chkRoom.TabIndex = 7;
            this.chkRoom.CheckedChanged += new System.EventHandler(this.chkRoom_CheckedChanged);
            // 
            // tgExtendMonitor
            // 
            this.tgExtendMonitor.Location = new System.Drawing.Point(218, 281);
            this.tgExtendMonitor.Name = "tgExtendMonitor";
            this.tgExtendMonitor.Properties.OffText = "Bật màn hình mở rộng";
            this.tgExtendMonitor.Properties.OnText = "Bật màn hình mở rộng";
            this.tgExtendMonitor.Size = new System.Drawing.Size(304, 24);
            this.tgExtendMonitor.StyleController = this.layoutControl1;
            this.tgExtendMonitor.TabIndex = 6;
            this.tgExtendMonitor.Toggled += new System.EventHandler(this.tgExtendMonitor_Toggled);
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 12);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckRoom});
            this.gridControl1.Size = new System.Drawing.Size(510, 242);
            this.gridControl1.TabIndex = 5;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.Gc_Check,
            this.Gc_RoomCode,
            this.Gc_RoomName});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            // 
            // Gc_Check
            // 
            this.Gc_Check.Caption = "Check";
            this.Gc_Check.ColumnEdit = this.repositoryItemCheckRoom;
            this.Gc_Check.FieldName = "IsCheck";
            this.Gc_Check.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.Gc_Check.Name = "Gc_Check";
            this.Gc_Check.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.Gc_Check.OptionsColumn.ShowCaption = false;
            this.Gc_Check.OptionsFilter.AllowFilter = false;
            this.Gc_Check.Visible = true;
            this.Gc_Check.VisibleIndex = 0;
            this.Gc_Check.Width = 50;
            // 
            // repositoryItemCheckRoom
            // 
            this.repositoryItemCheckRoom.AutoHeight = false;
            this.repositoryItemCheckRoom.Name = "repositoryItemCheckRoom";
            this.repositoryItemCheckRoom.CheckedChanged += new System.EventHandler(this.repositoryItemCheckRoom_CheckedChanged);
            // 
            // Gc_RoomCode
            // 
            this.Gc_RoomCode.Caption = "Mã phòng/buồng";
            this.Gc_RoomCode.FieldName = "EXECUTE_ROOM_CODE";
            this.Gc_RoomCode.Name = "Gc_RoomCode";
            this.Gc_RoomCode.Visible = true;
            this.Gc_RoomCode.VisibleIndex = 1;
            this.Gc_RoomCode.Width = 150;
            // 
            // Gc_RoomName
            // 
            this.Gc_RoomName.Caption = "Tên phòng/buồng";
            this.Gc_RoomName.FieldName = "EXECUTE_ROOM_NAME";
            this.Gc_RoomName.Name = "Gc_RoomName";
            this.Gc_RoomName.Visible = true;
            this.Gc_RoomName.VisibleIndex = 2;
            this.Gc_RoomName.Width = 300;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(534, 317);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(514, 246);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 269);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(206, 28);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.tgExtendMonitor;
            this.layoutControlItem3.Location = new System.Drawing.Point(206, 269);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(308, 28);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.chkRoom;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 246);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(514, 23);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // FormConfigWaitingScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 317);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormConfigWaitingScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormConfigWaitingScreen";
            this.Load += new System.EventHandler(this.FormConfigWaitingScreen_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tgExtendMonitor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.ToggleSwitch tgExtendMonitor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Check;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckRoom;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_RoomCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_RoomName;
        private DevExpress.XtraEditors.CheckEdit chkRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
