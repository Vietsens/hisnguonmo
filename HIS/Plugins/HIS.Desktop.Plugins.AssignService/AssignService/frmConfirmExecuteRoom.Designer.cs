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
    partial class frmConfirmExecuteRoom
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
            this.btnDisagree = new DevExpress.XtraEditors.SimpleButton();
            this.btnAgree = new DevExpress.XtraEditors.SimpleButton();
            this.txtServiceNoRoom = new DevExpress.XtraEditors.MemoEdit();
            this.grdExecuteRoom = new DevExpress.XtraGrid.GridControl();
            this.gridViewExecuteRoom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnExecuteRoomName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceCodes = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblQuestion = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciQuestion = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciServiceNoRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAgree = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDisagree = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceNoRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdExecuteRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExecuteRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciQuestion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceNoRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAgree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDisagree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnDisagree);
            this.layoutControl1.Controls.Add(this.btnAgree);
            this.layoutControl1.Controls.Add(this.txtServiceNoRoom);
            this.layoutControl1.Controls.Add(this.grdExecuteRoom);
            this.layoutControl1.Controls.Add(this.lblQuestion);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(534, 441);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnDisagree
            //
            this.btnDisagree.Location = new System.Drawing.Point(426, 417);
            this.btnDisagree.Name = "btnDisagree";
            this.btnDisagree.Size = new System.Drawing.Size(106, 22);
            this.btnDisagree.StyleController = this.layoutControl1;
            this.btnDisagree.TabIndex = 1;
            this.btnDisagree.Text = "Không đồng ý";
            this.btnDisagree.Click += new System.EventHandler(this.btnDisagree_Click);
            //
            // btnAgree
            //
            this.btnAgree.Location = new System.Drawing.Point(316, 417);
            this.btnAgree.Name = "btnAgree";
            this.btnAgree.Size = new System.Drawing.Size(106, 22);
            this.btnAgree.StyleController = this.layoutControl1;
            this.btnAgree.TabIndex = 2;
            this.btnAgree.Text = "Đồng ý";
            this.btnAgree.Click += new System.EventHandler(this.btnAgree_Click);
            //
            // txtServiceNoRoom
            //
            this.txtServiceNoRoom.Location = new System.Drawing.Point(2, 261);
            this.txtServiceNoRoom.Name = "txtServiceNoRoom";
            this.txtServiceNoRoom.Properties.ReadOnly = true;
            this.txtServiceNoRoom.Size = new System.Drawing.Size(530, 152);
            this.txtServiceNoRoom.StyleController = this.layoutControl1;
            this.txtServiceNoRoom.TabIndex = 3;
            //
            // grdExecuteRoom
            //
            this.grdExecuteRoom.Location = new System.Drawing.Point(2, 28);
            this.grdExecuteRoom.MainView = this.gridViewExecuteRoom;
            this.grdExecuteRoom.Name = "grdExecuteRoom";
            this.grdExecuteRoom.Size = new System.Drawing.Size(530, 211);
            this.grdExecuteRoom.TabIndex = 4;
            this.grdExecuteRoom.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExecuteRoom});
            //
            // gridViewExecuteRoom
            //
            this.gridViewExecuteRoom.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnExecuteRoomName,
            this.gridColumnServiceCount,
            this.gridColumnServiceCodes});
            this.gridViewExecuteRoom.GridControl = this.grdExecuteRoom;
            this.gridViewExecuteRoom.Name = "gridViewExecuteRoom";
            this.gridViewExecuteRoom.OptionsBehavior.Editable = false;
            this.gridViewExecuteRoom.OptionsCustomization.AllowFilter = false;
            this.gridViewExecuteRoom.OptionsCustomization.AllowGroup = false;
            this.gridViewExecuteRoom.OptionsCustomization.AllowSort = false;
            this.gridViewExecuteRoom.OptionsFind.AllowFindPanel = false;
            this.gridViewExecuteRoom.OptionsMenu.EnableColumnMenu = false;
            this.gridViewExecuteRoom.OptionsView.ShowGroupPanel = false;
            this.gridViewExecuteRoom.OptionsView.ShowIndicator = false;
            //
            // gridColumnExecuteRoomName
            //
            this.gridColumnExecuteRoomName.Caption = "Phòng xử lý";
            this.gridColumnExecuteRoomName.FieldName = "EXECUTE_ROOM_DISPLAY";
            this.gridColumnExecuteRoomName.Name = "gridColumnExecuteRoomName";
            this.gridColumnExecuteRoomName.OptionsColumn.AllowEdit = false;
            this.gridColumnExecuteRoomName.Visible = true;
            this.gridColumnExecuteRoomName.VisibleIndex = 0;
            this.gridColumnExecuteRoomName.Width = 260;
            //
            // gridColumnServiceCount
            //
            this.gridColumnServiceCount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnServiceCount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumnServiceCount.Caption = "Số dịch vụ";
            this.gridColumnServiceCount.FieldName = "SERVICE_COUNT";
            this.gridColumnServiceCount.Name = "gridColumnServiceCount";
            this.gridColumnServiceCount.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceCount.Visible = true;
            this.gridColumnServiceCount.VisibleIndex = 1;
            this.gridColumnServiceCount.Width = 70;
            //
            // gridColumnServiceCodes
            //
            this.gridColumnServiceCodes.Caption = "Mã dịch vụ";
            this.gridColumnServiceCodes.FieldName = "SERVICE_CODES";
            this.gridColumnServiceCodes.Name = "gridColumnServiceCodes";
            this.gridColumnServiceCodes.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceCodes.Visible = true;
            this.gridColumnServiceCodes.VisibleIndex = 2;
            this.gridColumnServiceCodes.Width = 200;
            //
            // lblQuestion
            //
            this.lblQuestion.Location = new System.Drawing.Point(2, 2);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(530, 22);
            this.lblQuestion.StyleController = this.layoutControl1;
            this.lblQuestion.TabIndex = 5;
            this.lblQuestion.Text = "Có đồng ý chọn những phòng xử lý này không?";
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciQuestion,
            this.lciGrid,
            this.lciServiceNoRoom,
            this.lciAgree,
            this.lciDisagree,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(534, 441);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciQuestion
            //
            this.lciQuestion.Control = this.lblQuestion;
            this.lciQuestion.Location = new System.Drawing.Point(0, 0);
            this.lciQuestion.Name = "lciQuestion";
            this.lciQuestion.Size = new System.Drawing.Size(534, 26);
            this.lciQuestion.TextSize = new System.Drawing.Size(0, 0);
            this.lciQuestion.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.grdExecuteRoom;
            this.lciGrid.Location = new System.Drawing.Point(0, 26);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(534, 215);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciServiceNoRoom
            //
            this.lciServiceNoRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceNoRoom.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lciServiceNoRoom.Control = this.txtServiceNoRoom;
            this.lciServiceNoRoom.Location = new System.Drawing.Point(0, 241);
            this.lciServiceNoRoom.Name = "lciServiceNoRoom";
            this.lciServiceNoRoom.Size = new System.Drawing.Size(534, 174);
            this.lciServiceNoRoom.Text = "Dịch vụ chưa chọn phòng xử lý";
            this.lciServiceNoRoom.TextLocation = DevExpress.Utils.Locations.Top;
            this.lciServiceNoRoom.TextSize = new System.Drawing.Size(200, 13);
            //
            // lciAgree
            //
            this.lciAgree.Control = this.btnAgree;
            this.lciAgree.Location = new System.Drawing.Point(314, 415);
            this.lciAgree.Name = "lciAgree";
            this.lciAgree.Size = new System.Drawing.Size(110, 26);
            this.lciAgree.TextSize = new System.Drawing.Size(0, 0);
            this.lciAgree.TextVisible = false;
            //
            // lciDisagree
            //
            this.lciDisagree.Control = this.btnDisagree;
            this.lciDisagree.Location = new System.Drawing.Point(424, 415);
            this.lciDisagree.Name = "lciDisagree";
            this.lciDisagree.Size = new System.Drawing.Size(110, 26);
            this.lciDisagree.TextSize = new System.Drawing.Size(0, 0);
            this.lciDisagree.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 415);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(314, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmConfirmExecuteRoom
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 441);
            this.Controls.Add(this.layoutControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfirmExecuteRoom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Xác nhận phòng xử lý";
            this.Load += new System.EventHandler(this.frmConfirmExecuteRoom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceNoRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdExecuteRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExecuteRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciQuestion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceNoRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAgree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDisagree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblQuestion;
        private DevExpress.XtraGrid.GridControl grdExecuteRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExecuteRoom;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnExecuteRoomName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceCount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceCodes;
        private DevExpress.XtraEditors.MemoEdit txtServiceNoRoom;
        private DevExpress.XtraEditors.SimpleButton btnAgree;
        private DevExpress.XtraEditors.SimpleButton btnDisagree;
        private DevExpress.XtraLayout.LayoutControlItem lciQuestion;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceNoRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciAgree;
        private DevExpress.XtraLayout.LayoutControlItem lciDisagree;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
