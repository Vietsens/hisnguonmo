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
namespace HIS.UC.UCCheckTT
{
    partial class UCCheckTT
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCheckTT));
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnPrintf = new DevExpress.XtraEditors.SimpleButton();
			this.gridControlHistory = new DevExpress.XtraGrid.GridControl();
			this.gridViewHistory = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemButton__View = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.lblNgayDu5Nam = new DevExpress.XtraEditors.LabelControl();
			this.lblThoiHanThe = new DevExpress.XtraEditors.LabelControl();
			this.lblKetQua = new DevExpress.XtraEditors.LabelControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDiaChi = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControlHistory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewHistory)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__View)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDiaChi)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			this.SuspendLayout();
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.btnPrintf);
			this.layoutControl1.Controls.Add(this.gridControlHistory);
			this.layoutControl1.Controls.Add(this.lblNgayDu5Nam);
			this.layoutControl1.Controls.Add(this.lblThoiHanThe);
			this.layoutControl1.Controls.Add(this.lblKetQua);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(421, 88, 250, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(625, 361);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			// 
			// btnPrintf
			// 
			this.btnPrintf.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintf.Image")));
			this.btnPrintf.Location = new System.Drawing.Point(594, 337);
			this.btnPrintf.Name = "btnPrintf";
			this.btnPrintf.Size = new System.Drawing.Size(29, 22);
			this.btnPrintf.StyleController = this.layoutControl1;
			this.btnPrintf.TabIndex = 18;
			this.btnPrintf.Text = "\r\n";
			this.btnPrintf.ToolTip = "In danh sách khám chữa bệnh";
			this.btnPrintf.Click += new System.EventHandler(this.btnPrintf_Click);
			// 
			// gridControlHistory
			// 
			this.gridControlHistory.Location = new System.Drawing.Point(2, 53);
			this.gridControlHistory.MainView = this.gridViewHistory;
			this.gridControlHistory.Name = "gridControlHistory";
			this.gridControlHistory.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButton__View});
			this.gridControlHistory.Size = new System.Drawing.Size(588, 306);
			this.gridControlHistory.TabIndex = 17;
			this.gridControlHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewHistory});
			// 
			// gridViewHistory
			// 
			this.gridViewHistory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn7,
            this.gridColumn6,
            this.gridColumn8});
			this.gridViewHistory.GridControl = this.gridControlHistory;
			this.gridViewHistory.Name = "gridViewHistory";
			this.gridViewHistory.OptionsView.ColumnAutoWidth = false;
			this.gridViewHistory.OptionsView.ShowGroupPanel = false;
			this.gridViewHistory.OptionsView.ShowIndicator = false;
			this.gridViewHistory.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHistory_CustomUnboundColumnData);
			// 
			// gridColumn1
			// 
			this.gridColumn1.Caption = "gridColumn1";
			this.gridColumn1.ColumnEdit = this.repositoryItemButton__View;
			this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.OptionsColumn.ShowCaption = false;
			this.gridColumn1.Visible = true;
			this.gridColumn1.VisibleIndex = 0;
			this.gridColumn1.Width = 20;
			// 
			// repositoryItemButton__View
			// 
			this.repositoryItemButton__View.AutoHeight = false;
			this.repositoryItemButton__View.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButton__View.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xem chi tiết", null, null, true)});
			this.repositoryItemButton__View.Name = "repositoryItemButton__View";
			this.repositoryItemButton__View.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
			this.repositoryItemButton__View.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButton__View_ButtonClick);
			// 
			// gridColumn3
			// 
			this.gridColumn3.Caption = "Tên cơ sở KCB";
			this.gridColumn3.FieldName = "cskcbbd_name";
			this.gridColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
			this.gridColumn3.Name = "gridColumn3";
			this.gridColumn3.OptionsColumn.AllowEdit = false;
			this.gridColumn3.OptionsColumn.AllowSize = false;
			this.gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn3.Visible = true;
			this.gridColumn3.VisibleIndex = 1;
			this.gridColumn3.Width = 228;
			// 
			// gridColumn4
			// 
			this.gridColumn4.Caption = "Từ ngày";
			this.gridColumn4.FieldName = "ngayVao_str";
			this.gridColumn4.Name = "gridColumn4";
			this.gridColumn4.OptionsColumn.AllowEdit = false;
			this.gridColumn4.OptionsColumn.AllowSize = false;
			this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn4.Visible = true;
			this.gridColumn4.VisibleIndex = 2;
			this.gridColumn4.Width = 134;
			// 
			// gridColumn5
			// 
			this.gridColumn5.Caption = "Đến ngày";
			this.gridColumn5.FieldName = "ngayRa_str";
			this.gridColumn5.Name = "gridColumn5";
			this.gridColumn5.OptionsColumn.AllowEdit = false;
			this.gridColumn5.OptionsColumn.AllowSize = false;
			this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn5.Visible = true;
			this.gridColumn5.VisibleIndex = 3;
			this.gridColumn5.Width = 120;
			// 
			// gridColumn7
			// 
			this.gridColumn7.Caption = "Tình trạng";
			this.gridColumn7.FieldName = "tinhTrang_str";
			this.gridColumn7.Name = "gridColumn7";
			this.gridColumn7.OptionsColumn.AllowEdit = false;
			this.gridColumn7.OptionsColumn.AllowSize = false;
			this.gridColumn7.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn7.Visible = true;
			this.gridColumn7.VisibleIndex = 4;
			this.gridColumn7.Width = 118;
			// 
			// gridColumn6
			// 
			this.gridColumn6.Caption = "Tên bệnh";
			this.gridColumn6.FieldName = "tenBenh";
			this.gridColumn6.Name = "gridColumn6";
			this.gridColumn6.OptionsColumn.AllowEdit = false;
			this.gridColumn6.Width = 164;
			// 
			// gridColumn8
			// 
			this.gridColumn8.Caption = "Kết quả";
			this.gridColumn8.FieldName = "kqDieuTri";
			this.gridColumn8.Name = "gridColumn8";
			this.gridColumn8.OptionsColumn.AllowEdit = false;
			this.gridColumn8.UnboundType = DevExpress.Data.UnboundColumnType.Object;
			this.gridColumn8.Width = 148;
			// 
			// lblNgayDu5Nam
			// 
			this.lblNgayDu5Nam.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblNgayDu5Nam.Location = new System.Drawing.Point(497, 29);
			this.lblNgayDu5Nam.Name = "lblNgayDu5Nam";
			this.lblNgayDu5Nam.Size = new System.Drawing.Size(126, 20);
			this.lblNgayDu5Nam.StyleController = this.layoutControl1;
			this.lblNgayDu5Nam.TabIndex = 16;
			// 
			// lblThoiHanThe
			// 
			this.lblThoiHanThe.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblThoiHanThe.Location = new System.Drawing.Point(87, 29);
			this.lblThoiHanThe.Name = "lblThoiHanThe";
			this.lblThoiHanThe.Size = new System.Drawing.Size(301, 20);
			this.lblThoiHanThe.StyleController = this.layoutControl1;
			this.lblThoiHanThe.TabIndex = 9;
			// 
			// lblKetQua
			// 
			this.lblKetQua.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblKetQua.Appearance.ForeColor = System.Drawing.Color.Blue;
			this.lblKetQua.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblKetQua.Location = new System.Drawing.Point(87, 2);
			this.lblKetQua.Name = "lblKetQua";
			this.lblKetQua.Size = new System.Drawing.Size(536, 23);
			this.lblKetQua.StyleController = this.layoutControl1;
			this.lblKetQua.TabIndex = 7;
			// 
			// layoutControlGroup1
			// 
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.lciDiaChi,
            this.layoutControlItem4,
            this.emptySpaceItem2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(625, 361);
			this.layoutControlGroup1.Text = "Lịch sử khám chữa bệnh";
			// 
			// layoutControlItem1
			// 
			this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
			this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
			this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.layoutControlItem1.Control = this.lblKetQua;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(625, 27);
			this.layoutControlItem1.Text = "Kết quả:";
			this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(80, 20);
			this.layoutControlItem1.TextToControlDistance = 5;
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
			this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
			this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.layoutControlItem2.Control = this.lblThoiHanThe;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 27);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(390, 24);
			this.layoutControlItem2.Text = "Giá trị thẻ từ:";
			this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(80, 20);
			this.layoutControlItem2.TextToControlDistance = 5;
			// 
			// layoutControlItem3
			// 
			this.layoutControlItem3.Control = this.gridControlHistory;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 51);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(592, 310);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			// 
			// lciDiaChi
			// 
			this.lciDiaChi.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
			this.lciDiaChi.AppearanceItemCaption.Options.UseForeColor = true;
			this.lciDiaChi.AppearanceItemCaption.Options.UseTextOptions = true;
			this.lciDiaChi.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.lciDiaChi.Control = this.lblNgayDu5Nam;
			this.lciDiaChi.Location = new System.Drawing.Point(390, 27);
			this.lciDiaChi.Name = "lciDiaChi";
			this.lciDiaChi.Size = new System.Drawing.Size(235, 24);
			this.lciDiaChi.Text = "Ngày đủ 5 năm:";
			this.lciDiaChi.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.lciDiaChi.TextSize = new System.Drawing.Size(100, 20);
			this.lciDiaChi.TextToControlDistance = 5;
			// 
			// layoutControlItem4
			// 
			this.layoutControlItem4.Control = this.btnPrintf;
			this.layoutControlItem4.Location = new System.Drawing.Point(592, 335);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(33, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			// 
			// emptySpaceItem2
			// 
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(592, 51);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(33, 284);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			// 
			// UCCheckTT
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "UCCheckTT";
			this.Size = new System.Drawing.Size(625, 361);
			this.Load += new System.EventHandler(this.UCCheckTT_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControlHistory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewHistory)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton__View)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDiaChi)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblKetQua;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.LabelControl lblThoiHanThe;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.LabelControl lblNgayDu5Nam;
        private DevExpress.XtraLayout.LayoutControlItem lciDiaChi;
        private DevExpress.XtraGrid.GridControl gridControlHistory;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewHistory;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton__View;
        private DevExpress.XtraEditors.SimpleButton btnPrintf;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
