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
namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    partial class frmAssignServiceHistory
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
            this.gridControlHistory = new DevExpress.XtraGrid.GridControl();
            this.gridViewHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grc_History_STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grc_History_ServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grc_History_IntructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grc_History_RequestRoom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grc_History_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlHistory);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(585, 122);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlHistory
            // 
            this.gridControlHistory.Location = new System.Drawing.Point(2, 2);
            this.gridControlHistory.MainView = this.gridViewHistory;
            this.gridControlHistory.Name = "gridControlHistory";
            this.gridControlHistory.Size = new System.Drawing.Size(581, 118);
            this.gridControlHistory.TabIndex = 4;
            this.gridControlHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewHistory});
            // 
            // gridViewHistory
            // 
            this.gridViewHistory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grc_History_STT,
            this.grc_History_ServiceReqCode,
            this.grc_History_IntructionTime,
            this.grc_History_RequestRoom,
            this.grc_History_Amount});
            this.gridViewHistory.GridControl = this.gridControlHistory;
            this.gridViewHistory.Name = "gridViewHistory";
            this.gridViewHistory.OptionsView.ShowGroupPanel = false;
            this.gridViewHistory.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHistory_CustomUnboundColumnData);
            // 
            // grc_History_STT
            // 
            this.grc_History_STT.Caption = "STT";
            this.grc_History_STT.FieldName = "STT";
            this.grc_History_STT.Name = "grc_History_STT";
            this.grc_History_STT.OptionsColumn.AllowEdit = false;
            this.grc_History_STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grc_History_STT.Visible = true;
            this.grc_History_STT.VisibleIndex = 0;
            this.grc_History_STT.Width = 33;
            // 
            // grc_History_ServiceReqCode
            // 
            this.grc_History_ServiceReqCode.Caption = "Mã y lệnh";
            this.grc_History_ServiceReqCode.FieldName = "TDL_SERVICE_REQ_CODE";
            this.grc_History_ServiceReqCode.Name = "grc_History_ServiceReqCode";
            this.grc_History_ServiceReqCode.OptionsColumn.AllowEdit = false;
            this.grc_History_ServiceReqCode.Visible = true;
            this.grc_History_ServiceReqCode.VisibleIndex = 1;
            this.grc_History_ServiceReqCode.Width = 126;
            // 
            // grc_History_IntructionTime
            // 
            this.grc_History_IntructionTime.Caption = "Thời gian y lệnh";
            this.grc_History_IntructionTime.FieldName = "TDL_INTRUCTION_TIME_STR";
            this.grc_History_IntructionTime.Name = "grc_History_IntructionTime";
            this.grc_History_IntructionTime.OptionsColumn.AllowEdit = false;
            this.grc_History_IntructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.grc_History_IntructionTime.Visible = true;
            this.grc_History_IntructionTime.VisibleIndex = 2;
            this.grc_History_IntructionTime.Width = 150;
            // 
            // grc_History_RequestRoom
            // 
            this.grc_History_RequestRoom.Caption = "Phòng chỉ định";
            this.grc_History_RequestRoom.FieldName = "REQUEST_ROOM_NAME";
            this.grc_History_RequestRoom.Name = "grc_History_RequestRoom";
            this.grc_History_RequestRoom.OptionsColumn.AllowEdit = false;
            this.grc_History_RequestRoom.Visible = true;
            this.grc_History_RequestRoom.VisibleIndex = 3;
            this.grc_History_RequestRoom.Width = 183;
            // 
            // grc_History_Amount
            // 
            this.grc_History_Amount.Caption = "Số lượng";
            this.grc_History_Amount.FieldName = "AMOUNT";
            this.grc_History_Amount.Name = "grc_History_Amount";
            this.grc_History_Amount.OptionsColumn.AllowEdit = false;
            this.grc_History_Amount.Visible = true;
            this.grc_History_Amount.VisibleIndex = 4;
            this.grc_History_Amount.Width = 71;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(585, 122);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlHistory;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(585, 122);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmAssignServiceHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 122);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmAssignServiceHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lịch sử chỉ định";
            this.Load += new System.EventHandler(this.frmAssignServiceHistory_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlHistory;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewHistory;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn grc_History_STT;
        private DevExpress.XtraGrid.Columns.GridColumn grc_History_ServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn grc_History_IntructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn grc_History_RequestRoom;
        private DevExpress.XtraGrid.Columns.GridColumn grc_History_Amount;
    }
}
