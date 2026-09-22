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
namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    partial class frmChooseServiceReqEmrToolkit
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
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
            this.grdServiceReq = new DevExpress.XtraGrid.GridControl();
            this.gridViewServiceReq = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcServiceReqCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcIntructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelBottom = new DevExpress.XtraEditors.PanelControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdServiceReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            //
            // grdServiceReq
            //
            this.grdServiceReq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdServiceReq.Location = new System.Drawing.Point(0, 0);
            this.grdServiceReq.MainView = this.gridViewServiceReq;
            this.grdServiceReq.Name = "grdServiceReq";
            this.grdServiceReq.Size = new System.Drawing.Size(614, 276);
            this.grdServiceReq.TabIndex = 0;
            this.grdServiceReq.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewServiceReq});
            //
            // gridViewServiceReq
            //
            this.gridViewServiceReq.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gcServiceReqCode,
            this.gcIntructionTime,
            this.gcRoomName});
            this.gridViewServiceReq.GridControl = this.grdServiceReq;
            this.gridViewServiceReq.Name = "gridViewServiceReq";
            this.gridViewServiceReq.OptionsBehavior.Editable = false;
            this.gridViewServiceReq.OptionsFind.AllowFindPanel = false;
            this.gridViewServiceReq.OptionsView.ShowGroupPanel = false;
            this.gridViewServiceReq.OptionsView.ShowIndicator = false;
            this.gridViewServiceReq.DoubleClick += new System.EventHandler(this.gridViewServiceReq_DoubleClick);
            //
            // gcServiceReqCode
            //
            this.gcServiceReqCode.Caption = "Mã phiếu";
            this.gcServiceReqCode.FieldName = "SERVICE_REQ_CODE";
            this.gcServiceReqCode.Name = "gcServiceReqCode";
            this.gcServiceReqCode.OptionsColumn.AllowEdit = false;
            this.gcServiceReqCode.Visible = true;
            this.gcServiceReqCode.VisibleIndex = 0;
            this.gcServiceReqCode.Width = 160;
            //
            // gcIntructionTime
            //
            this.gcIntructionTime.Caption = "Thời gian chỉ định";
            this.gcIntructionTime.FieldName = "INTRUCTION_TIME_STR";
            this.gcIntructionTime.Name = "gcIntructionTime";
            this.gcIntructionTime.OptionsColumn.AllowEdit = false;
            this.gcIntructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gcIntructionTime.Visible = true;
            this.gcIntructionTime.VisibleIndex = 1;
            this.gcIntructionTime.Width = 170;
            //
            // gcRoomName
            //
            this.gcRoomName.Caption = "Phòng chỉ định";
            this.gcRoomName.FieldName = "REQUEST_ROOM_NAME";
            this.gcRoomName.Name = "gcRoomName";
            this.gcRoomName.OptionsColumn.AllowEdit = false;
            this.gcRoomName.Visible = true;
            this.gcRoomName.VisibleIndex = 2;
            this.gcRoomName.Width = 250;
            //
            // panelBottom
            //
            this.panelBottom.Controls.Add(this.btnChoose);
            this.panelBottom.Controls.Add(this.btnCancel);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 276);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(614, 44);
            this.panelBottom.TabIndex = 1;
            //
            // btnChoose
            //
            this.btnChoose.Location = new System.Drawing.Point(400, 7);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(98, 30);
            this.btnChoose.TabIndex = 0;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(504, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Đóng";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmChooseServiceReqEmrToolkit
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 320);
            this.Controls.Add(this.grdServiceReq);
            this.Controls.Add(this.panelBottom);
            this.MinimumSize = new System.Drawing.Size(480, 280);
            this.Name = "frmChooseServiceReqEmrToolkit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn phiếu xét nghiệm";
            this.Load += new System.EventHandler(this.frmChooseServiceReqEmrToolkit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridViewServiceReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdServiceReq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBottom)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraGrid.GridControl grdServiceReq;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewServiceReq;
        private DevExpress.XtraGrid.Columns.GridColumn gcServiceReqCode;
        private DevExpress.XtraGrid.Columns.GridColumn gcIntructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gcRoomName;
        private DevExpress.XtraEditors.PanelControl panelBottom;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}
