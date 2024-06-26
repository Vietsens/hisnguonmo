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
namespace HIS.Desktop.Plugins.TreatmentLockList
{
    partial class frmTreatmentLockList
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
            this.gridControlTreatmentLock = new DevExpress.XtraGrid.GridControl();
            this.gridViewTreatmentLock = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_TreatmentLock_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TreatmentLock_LockerLoginname = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TreatmentLock_IsLock = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TreatmentLock_FeeLockTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_TreatmentLock_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentLock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlTreatmentLock);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(554, 562);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlTreatmentLock
            // 
            this.gridControlTreatmentLock.Location = new System.Drawing.Point(2, 2);
            this.gridControlTreatmentLock.MainView = this.gridViewTreatmentLock;
            this.gridControlTreatmentLock.Name = "gridControlTreatmentLock";
            this.gridControlTreatmentLock.Size = new System.Drawing.Size(550, 558);
            this.gridControlTreatmentLock.TabIndex = 4;
            this.gridControlTreatmentLock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTreatmentLock});
            // 
            // gridViewTreatmentLock
            // 
            this.gridViewTreatmentLock.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_TreatmentLock_Stt,
            this.gridColumn_TreatmentLock_LockerLoginname,
            this.gridColumn_TreatmentLock_IsLock,
            this.gridColumn_TreatmentLock_FeeLockTime,
            this.gridColumn_TreatmentLock_CreateTime});
            this.gridViewTreatmentLock.GridControl = this.gridControlTreatmentLock;
            this.gridViewTreatmentLock.Name = "gridViewTreatmentLock";
            this.gridViewTreatmentLock.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewTreatmentLock.OptionsView.ShowGroupPanel = false;
            this.gridViewTreatmentLock.OptionsView.ShowIndicator = false;
            this.gridViewTreatmentLock.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewTreatmentLock_CustomUnboundColumnData);
            // 
            // gridColumn_TreatmentLock_Stt
            // 
            this.gridColumn_TreatmentLock_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_TreatmentLock_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_Stt.Caption = "STT";
            this.gridColumn_TreatmentLock_Stt.FieldName = "STT";
            this.gridColumn_TreatmentLock_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_TreatmentLock_Stt.Name = "gridColumn_TreatmentLock_Stt";
            this.gridColumn_TreatmentLock_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_TreatmentLock_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_TreatmentLock_Stt.Visible = true;
            this.gridColumn_TreatmentLock_Stt.VisibleIndex = 0;
            this.gridColumn_TreatmentLock_Stt.Width = 35;
            // 
            // gridColumn_TreatmentLock_LockerLoginname
            // 
            this.gridColumn_TreatmentLock_LockerLoginname.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_LockerLoginname.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_LockerLoginname.Caption = "Người thực hiện";
            this.gridColumn_TreatmentLock_LockerLoginname.FieldName = "LOGINNAME";
            this.gridColumn_TreatmentLock_LockerLoginname.Name = "gridColumn_TreatmentLock_LockerLoginname";
            this.gridColumn_TreatmentLock_LockerLoginname.OptionsColumn.AllowEdit = false;
            this.gridColumn_TreatmentLock_LockerLoginname.Visible = true;
            this.gridColumn_TreatmentLock_LockerLoginname.VisibleIndex = 1;
            this.gridColumn_TreatmentLock_LockerLoginname.Width = 130;
            // 
            // gridColumn_TreatmentLock_IsLock
            // 
            this.gridColumn_TreatmentLock_IsLock.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_IsLock.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_IsLock.Caption = "Thao tác";
            this.gridColumn_TreatmentLock_IsLock.FieldName = "TREATMENT_LOG_TYPE_NAME";
            this.gridColumn_TreatmentLock_IsLock.Name = "gridColumn_TreatmentLock_IsLock";
            this.gridColumn_TreatmentLock_IsLock.OptionsColumn.AllowEdit = false;
            this.gridColumn_TreatmentLock_IsLock.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_TreatmentLock_IsLock.Visible = true;
            this.gridColumn_TreatmentLock_IsLock.VisibleIndex = 2;
            this.gridColumn_TreatmentLock_IsLock.Width = 130;
            // 
            // gridColumn_TreatmentLock_FeeLockTime
            // 
            this.gridColumn_TreatmentLock_FeeLockTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_FeeLockTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_FeeLockTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_FeeLockTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_FeeLockTime.Caption = "Thời gian";
            this.gridColumn_TreatmentLock_FeeLockTime.FieldName = "USER_TIME_STR";
            this.gridColumn_TreatmentLock_FeeLockTime.Name = "gridColumn_TreatmentLock_FeeLockTime";
            this.gridColumn_TreatmentLock_FeeLockTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_TreatmentLock_FeeLockTime.ToolTip = "Thời gian khóa viện phí";
            this.gridColumn_TreatmentLock_FeeLockTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_TreatmentLock_FeeLockTime.Visible = true;
            this.gridColumn_TreatmentLock_FeeLockTime.VisibleIndex = 3;
            this.gridColumn_TreatmentLock_FeeLockTime.Width = 120;
            // 
            // gridColumn_TreatmentLock_CreateTime
            // 
            this.gridColumn_TreatmentLock_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_TreatmentLock_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_TreatmentLock_CreateTime.Caption = "Thời gian tạo";
            this.gridColumn_TreatmentLock_CreateTime.FieldName = "CREATE_TIME_STR";
            this.gridColumn_TreatmentLock_CreateTime.Name = "gridColumn_TreatmentLock_CreateTime";
            this.gridColumn_TreatmentLock_CreateTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_TreatmentLock_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_TreatmentLock_CreateTime.Visible = true;
            this.gridColumn_TreatmentLock_CreateTime.VisibleIndex = 4;
            this.gridColumn_TreatmentLock_CreateTime.Width = 131;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(554, 562);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlTreatmentLock;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(554, 562);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmTreatmentLockList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 562);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmTreatmentLockList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lịch sử duyệt khóa";
            this.Load += new System.EventHandler(this.frmTreatmentLockList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTreatmentLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTreatmentLock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlTreatmentLock;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTreatmentLock;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TreatmentLock_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TreatmentLock_LockerLoginname;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TreatmentLock_IsLock;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TreatmentLock_CreateTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_TreatmentLock_FeeLockTime;
    }
}
