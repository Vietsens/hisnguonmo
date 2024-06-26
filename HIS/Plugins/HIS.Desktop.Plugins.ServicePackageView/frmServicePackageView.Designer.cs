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
namespace HIS.Desktop.Plugins.ServicePackageView
{
    partial class frmServicePackageView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmServicePackageView));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeListSereServDetail = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumnServiceName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnAmount = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnPrice = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnPriceTotal = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnPatientType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnCPNgoaiGoi = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnExecuteRoom = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnRequestRoom = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumnRequestDepartment = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.chkCPNgoaiGoi = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.btnCPNgoaiGoi = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.chkCPNgoaiGoi_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListSereServDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCPNgoaiGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCPNgoaiGoi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCPNgoaiGoi_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeListSereServDetail);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1114, 463);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeListSereServDetail
            // 
            this.treeListSereServDetail.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumnServiceName,
            this.treeListColumnAmount,
            this.treeListColumnPrice,
            this.treeListColumnPriceTotal,
            this.treeListColumnPatientType,
            this.treeListColumnCPNgoaiGoi,
            this.treeListColumnExecuteRoom,
            this.treeListColumnRequestRoom,
            this.treeListColumnRequestDepartment,
            this.treeListColumn1});
            this.treeListSereServDetail.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListSereServDetail.Location = new System.Drawing.Point(2, 2);
            this.treeListSereServDetail.Name = "treeListSereServDetail";
            this.treeListSereServDetail.OptionsBehavior.PopulateServiceColumns = true;
            this.treeListSereServDetail.OptionsView.AutoWidth = false;
            this.treeListSereServDetail.OptionsView.ShowIndicator = false;
            this.treeListSereServDetail.OptionsView.ShowVertLines = false;
            this.treeListSereServDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.chkCPNgoaiGoi,
            this.btnCPNgoaiGoi,
            this.chkCPNgoaiGoi_Disable});
            this.treeListSereServDetail.Size = new System.Drawing.Size(1110, 459);
            this.treeListSereServDetail.TabIndex = 5;
            this.treeListSereServDetail.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListSereServDetail_CustomNodeCellEdit);
            this.treeListSereServDetail.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeListSereServDetail_NodeCellStyle);
            this.treeListSereServDetail.GetNodeDisplayValue += new DevExpress.XtraTreeList.GetNodeDisplayValueEventHandler(this.treeListSereServDetail_GetNodeDisplayValue);
            this.treeListSereServDetail.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListSereServDetail_CustomUnboundColumnData);
            this.treeListSereServDetail.BeforeCheckNode += new DevExpress.XtraTreeList.CheckNodeEventHandler(this.treeListSereServDetail_BeforeCheckNode);
            // 
            // treeListColumnServiceName
            // 
            this.treeListColumnServiceName.Caption = "Tên dịch vụ";
            this.treeListColumnServiceName.FieldName = "Tên dịch vụ";
            this.treeListColumnServiceName.Name = "treeListColumnServiceName";
            this.treeListColumnServiceName.OptionsColumn.AllowEdit = false;
            this.treeListColumnServiceName.OptionsColumn.FixedWidth = true;
            this.treeListColumnServiceName.OptionsColumn.ReadOnly = true;
            this.treeListColumnServiceName.Visible = true;
            this.treeListColumnServiceName.VisibleIndex = 0;
            this.treeListColumnServiceName.Width = 288;
            // 
            // treeListColumnAmount
            // 
            this.treeListColumnAmount.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumnAmount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumnAmount.Caption = "Số lượng";
            this.treeListColumnAmount.FieldName = "Số lượng";
            this.treeListColumnAmount.Name = "treeListColumnAmount";
            this.treeListColumnAmount.OptionsColumn.AllowEdit = false;
            this.treeListColumnAmount.OptionsColumn.FixedWidth = true;
            this.treeListColumnAmount.OptionsColumn.ReadOnly = true;
            this.treeListColumnAmount.Visible = true;
            this.treeListColumnAmount.VisibleIndex = 1;
            this.treeListColumnAmount.Width = 100;
            // 
            // treeListColumnPrice
            // 
            this.treeListColumnPrice.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumnPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumnPrice.Caption = "Đơn giá";
            this.treeListColumnPrice.FieldName = "Đơn giá";
            this.treeListColumnPrice.Name = "treeListColumnPrice";
            this.treeListColumnPrice.OptionsColumn.AllowEdit = false;
            this.treeListColumnPrice.OptionsColumn.FixedWidth = true;
            this.treeListColumnPrice.OptionsColumn.ReadOnly = true;
            this.treeListColumnPrice.Visible = true;
            this.treeListColumnPrice.VisibleIndex = 2;
            this.treeListColumnPrice.Width = 150;
            // 
            // treeListColumnPriceTotal
            // 
            this.treeListColumnPriceTotal.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumnPriceTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumnPriceTotal.Caption = "Thành tiền";
            this.treeListColumnPriceTotal.FieldName = "Thành tiền";
            this.treeListColumnPriceTotal.Name = "treeListColumnPriceTotal";
            this.treeListColumnPriceTotal.OptionsColumn.FixedWidth = true;
            this.treeListColumnPriceTotal.Visible = true;
            this.treeListColumnPriceTotal.VisibleIndex = 3;
            this.treeListColumnPriceTotal.Width = 150;
            // 
            // treeListColumnPatientType
            // 
            this.treeListColumnPatientType.Caption = "ĐT thanh toán";
            this.treeListColumnPatientType.FieldName = "ĐT thanh toán";
            this.treeListColumnPatientType.Name = "treeListColumnPatientType";
            this.treeListColumnPatientType.OptionsColumn.AllowEdit = false;
            this.treeListColumnPatientType.OptionsColumn.FixedWidth = true;
            this.treeListColumnPatientType.OptionsColumn.ReadOnly = true;
            this.treeListColumnPatientType.Visible = true;
            this.treeListColumnPatientType.VisibleIndex = 4;
            this.treeListColumnPatientType.Width = 100;
            // 
            // treeListColumnCPNgoaiGoi
            // 
            this.treeListColumnCPNgoaiGoi.Caption = "CP ngoài gói";
            this.treeListColumnCPNgoaiGoi.FieldName = "CPNgoaiGoi";
            this.treeListColumnCPNgoaiGoi.Name = "treeListColumnCPNgoaiGoi";
            this.treeListColumnCPNgoaiGoi.Visible = true;
            this.treeListColumnCPNgoaiGoi.VisibleIndex = 5;
            this.treeListColumnCPNgoaiGoi.Width = 86;
            // 
            // treeListColumnExecuteRoom
            // 
            this.treeListColumnExecuteRoom.Caption = "Phòng xử lý";
            this.treeListColumnExecuteRoom.FieldName = "Phòng xử lý";
            this.treeListColumnExecuteRoom.Name = "treeListColumnExecuteRoom";
            this.treeListColumnExecuteRoom.OptionsColumn.AllowEdit = false;
            this.treeListColumnExecuteRoom.OptionsColumn.FixedWidth = true;
            this.treeListColumnExecuteRoom.OptionsColumn.ReadOnly = true;
            this.treeListColumnExecuteRoom.Visible = true;
            this.treeListColumnExecuteRoom.VisibleIndex = 6;
            this.treeListColumnExecuteRoom.Width = 150;
            // 
            // treeListColumnRequestRoom
            // 
            this.treeListColumnRequestRoom.Caption = "Phòng yêu cầu";
            this.treeListColumnRequestRoom.FieldName = "Phòng yêu cầu";
            this.treeListColumnRequestRoom.Name = "treeListColumnRequestRoom";
            this.treeListColumnRequestRoom.OptionsColumn.AllowEdit = false;
            this.treeListColumnRequestRoom.OptionsColumn.FixedWidth = true;
            this.treeListColumnRequestRoom.OptionsColumn.ReadOnly = true;
            this.treeListColumnRequestRoom.Visible = true;
            this.treeListColumnRequestRoom.VisibleIndex = 7;
            this.treeListColumnRequestRoom.Width = 150;
            // 
            // treeListColumnRequestDepartment
            // 
            this.treeListColumnRequestDepartment.Caption = "Khoa chỉ định";
            this.treeListColumnRequestDepartment.FieldName = "Khoa chỉ định";
            this.treeListColumnRequestDepartment.Name = "treeListColumnRequestDepartment";
            this.treeListColumnRequestDepartment.OptionsColumn.AllowEdit = false;
            this.treeListColumnRequestDepartment.OptionsColumn.FixedWidth = true;
            this.treeListColumnRequestDepartment.OptionsColumn.ReadOnly = true;
            this.treeListColumnRequestDepartment.Visible = true;
            this.treeListColumnRequestDepartment.VisibleIndex = 8;
            this.treeListColumnRequestDepartment.Width = 200;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "treeListColumnSereServId";
            this.treeListColumn1.FieldName = "treeListColumnSereServId";
            this.treeListColumn1.Name = "treeListColumn1";
            // 
            // chkCPNgoaiGoi
            // 
            this.chkCPNgoaiGoi.AutoHeight = false;
            this.chkCPNgoaiGoi.Name = "chkCPNgoaiGoi";
            this.chkCPNgoaiGoi.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkCPNgoaiGoi.CheckStateChanged += new System.EventHandler(this.chkCPNgoaiGoi_CheckStateChanged);
            this.chkCPNgoaiGoi.EditValueChanged += new System.EventHandler(this.chkCPNgoaiGoi_EditValueChanged);
            // 
            // btnCPNgoaiGoi
            // 
            this.btnCPNgoaiGoi.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("btnCPNgoaiGoi.Appearance.Image")));
            this.btnCPNgoaiGoi.Appearance.Options.UseImage = true;
            this.btnCPNgoaiGoi.AutoHeight = false;
            this.btnCPNgoaiGoi.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.btnCPNgoaiGoi.Name = "btnCPNgoaiGoi";
            // 
            // chkCPNgoaiGoi_Disable
            // 
            this.chkCPNgoaiGoi_Disable.AutoHeight = false;
            this.chkCPNgoaiGoi_Disable.Name = "chkCPNgoaiGoi_Disable";
            this.chkCPNgoaiGoi_Disable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkCPNgoaiGoi_Disable.ReadOnly = true;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(1114, 463);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeListSereServDetail;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1114, 463);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmServicePackageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1114, 463);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmServicePackageView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Xem gói dịch vụ";
            this.Load += new System.EventHandler(this.frmServicePackageView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListSereServDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCPNgoaiGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCPNgoaiGoi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCPNgoaiGoi_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraTreeList.TreeList treeListSereServDetail;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnServiceName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnAmount;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnPrice;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnPatientType;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnExecuteRoom;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnRequestRoom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnRequestDepartment;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnPriceTotal;
        internal DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkCPNgoaiGoi;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit btnCPNgoaiGoi;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumnCPNgoaiGoi;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkCPNgoaiGoi_Disable;
    }
}
