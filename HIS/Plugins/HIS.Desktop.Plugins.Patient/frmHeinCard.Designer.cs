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
namespace HIS.Desktop.Plugins.Patient
{
    partial class frmHeinCard
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
            this.gridControlHeinCard = new DevExpress.XtraGrid.GridControl();
            this.gridViewHeinCard = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColHeinCardNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColFromDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColToDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColHeinMediOrg = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHeinCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHeinCard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlHeinCard);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1028, 485);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlHeinCard
            // 
            this.gridControlHeinCard.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlHeinCard.Location = new System.Drawing.Point(12, 12);
            this.gridControlHeinCard.MainView = this.gridViewHeinCard;
            this.gridControlHeinCard.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gridControlHeinCard.Name = "gridControlHeinCard";
            this.gridControlHeinCard.Size = new System.Drawing.Size(1004, 461);
            this.gridControlHeinCard.TabIndex = 4;
            this.gridControlHeinCard.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewHeinCard});
            // 
            // gridViewHeinCard
            // 
            this.gridViewHeinCard.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColHeinCardNumber,
            this.gridColFromDate,
            this.gridColToDate,
            this.gridColPatientName,
            this.gridColHeinMediOrg,
            this.gridColCreateTime,
            this.gridColCreator,
            this.gridColModifyTime,
            this.gridColModifier});
            this.gridViewHeinCard.GridControl = this.gridControlHeinCard;
            this.gridViewHeinCard.Name = "gridViewHeinCard";
            this.gridViewHeinCard.OptionsView.ShowGroupPanel = false;
            this.gridViewHeinCard.OptionsView.ShowIndicator = false;
            this.gridViewHeinCard.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHeinCard_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 54;
            // 
            // gridColHeinCardNumber
            // 
            this.gridColHeinCardNumber.Caption = "Số thẻ";
            this.gridColHeinCardNumber.FieldName = "HEIN_CARD_NUMBER";
            this.gridColHeinCardNumber.Name = "gridColHeinCardNumber";
            this.gridColHeinCardNumber.OptionsColumn.AllowEdit = false;
            this.gridColHeinCardNumber.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColHeinCardNumber.Visible = true;
            this.gridColHeinCardNumber.VisibleIndex = 1;
            this.gridColHeinCardNumber.Width = 150;
            // 
            // gridColFromDate
            // 
            this.gridColFromDate.Caption = "Từ ngày";
            this.gridColFromDate.FieldName = "FROM_DATE_DISPLAY";
            this.gridColFromDate.Name = "gridColFromDate";
            this.gridColFromDate.OptionsColumn.AllowEdit = false;
            this.gridColFromDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColFromDate.Visible = true;
            this.gridColFromDate.VisibleIndex = 2;
            this.gridColFromDate.Width = 128;
            // 
            // gridColToDate
            // 
            this.gridColToDate.Caption = "Đến ngày";
            this.gridColToDate.FieldName = "TO_DATE_DISPLAY";
            this.gridColToDate.Name = "gridColToDate";
            this.gridColToDate.OptionsColumn.AllowEdit = false;
            this.gridColToDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColToDate.Visible = true;
            this.gridColToDate.VisibleIndex = 3;
            this.gridColToDate.Width = 128;
            // 
            // gridColPatientName
            // 
            this.gridColPatientName.Caption = "Mã ĐKKCB";
            this.gridColPatientName.FieldName = "HEIN_MEDI_ORG_CODE";
            this.gridColPatientName.Name = "gridColPatientName";
            this.gridColPatientName.OptionsColumn.AllowEdit = false;
            this.gridColPatientName.Visible = true;
            this.gridColPatientName.VisibleIndex = 4;
            this.gridColPatientName.Width = 85;
            // 
            // gridColHeinMediOrg
            // 
            this.gridColHeinMediOrg.Caption = "Nơi ĐKKCB";
            this.gridColHeinMediOrg.FieldName = "HEIN_MEDI_ORG_NAME";
            this.gridColHeinMediOrg.Name = "gridColHeinMediOrg";
            this.gridColHeinMediOrg.OptionsColumn.AllowEdit = false;
            this.gridColHeinMediOrg.Visible = true;
            this.gridColHeinMediOrg.VisibleIndex = 5;
            this.gridColHeinMediOrg.Width = 215;
            // 
            // gridColCreateTime
            // 
            this.gridColCreateTime.Caption = "Thời gian tạo";
            this.gridColCreateTime.FieldName = "CREATE_TIME_DISPLAY";
            this.gridColCreateTime.Name = "gridColCreateTime";
            this.gridColCreateTime.OptionsColumn.AllowEdit = false;
            this.gridColCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColCreateTime.Visible = true;
            this.gridColCreateTime.VisibleIndex = 6;
            this.gridColCreateTime.Width = 128;
            // 
            // gridColCreator
            // 
            this.gridColCreator.Caption = "Người tạo";
            this.gridColCreator.FieldName = "CREATOR";
            this.gridColCreator.Name = "gridColCreator";
            this.gridColCreator.OptionsColumn.AllowEdit = false;
            this.gridColCreator.Visible = true;
            this.gridColCreator.VisibleIndex = 7;
            this.gridColCreator.Width = 161;
            // 
            // gridColModifyTime
            // 
            this.gridColModifyTime.Caption = "Thời gian sửa";
            this.gridColModifyTime.FieldName = "MODIFY_TIME_DISPLAY";
            this.gridColModifyTime.Name = "gridColModifyTime";
            this.gridColModifyTime.OptionsColumn.AllowEdit = false;
            this.gridColModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColModifyTime.Visible = true;
            this.gridColModifyTime.VisibleIndex = 8;
            this.gridColModifyTime.Width = 128;
            // 
            // gridColModifier
            // 
            this.gridColModifier.Caption = "Người sửa";
            this.gridColModifier.FieldName = "MODIFIER";
            this.gridColModifier.Name = "gridColModifier";
            this.gridColModifier.OptionsColumn.AllowEdit = false;
            this.gridColModifier.Visible = true;
            this.gridColModifier.VisibleIndex = 9;
            this.gridColModifier.Width = 171;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 4;
            this.layoutControlGroup1.Size = new System.Drawing.Size(1028, 485);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlHeinCard;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1008, 465);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmHeinCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 485);
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmHeinCard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông tin thẻ BHYT";
            this.Load += new System.EventHandler(this.frmHeinCard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHeinCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHeinCard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControlHeinCard;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewHeinCard;
        private DevExpress.XtraGrid.Columns.GridColumn gridColHeinCardNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColFromDate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColToDate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColHeinCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColHeinMediOrg;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColCreator;
        private DevExpress.XtraGrid.Columns.GridColumn gridColModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColModifier;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColStt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;

    }
}
