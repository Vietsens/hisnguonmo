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
namespace Inventec.UC.TreeSereServHein
{
    partial class UCTreeSereServHein
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCTreeSereServHein));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeSereServ = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn5 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn13 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn6 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn7 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn4 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn9 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn10 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn11 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn8 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn12 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.spinSoLuong = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.chkCheckedColumn = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.txtReadOnly = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.imageCollectionStatus = new DevExpress.Utils.ImageCollection(this.components);
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeSereServ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSoLuong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckedColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReadOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollectionStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeSereServ);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(880, 768);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // treeSereServ
            // 
            this.treeSereServ.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeSereServ.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn5,
            this.treeListColumn3,
            this.treeListColumn13,
            this.treeListColumn6,
            this.treeListColumn2,
            this.treeListColumn7,
            this.treeListColumn4,
            this.treeListColumn9,
            this.treeListColumn10,
            this.treeListColumn11,
            this.treeListColumn8,
            this.treeListColumn12});
            this.treeSereServ.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeSereServ.Location = new System.Drawing.Point(2, 2);
            this.treeSereServ.LookAndFeel.SkinName = "Office 2013";
            this.treeSereServ.Name = "treeSereServ";
            this.treeSereServ.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            this.treeSereServ.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeSereServ.OptionsBehavior.Editable = false;
            this.treeSereServ.OptionsBehavior.EnableFiltering = true;
            this.treeSereServ.OptionsFilter.FilterMode = DevExpress.XtraTreeList.FilterMode.Extended;
            this.treeSereServ.OptionsFind.AlwaysVisible = true;
            this.treeSereServ.OptionsFind.FindDelay = 100;
            this.treeSereServ.OptionsFind.FindNullPrompt = "Nhập chuỗi tìm kiếm ...";
            this.treeSereServ.OptionsFind.ShowClearButton = false;
            this.treeSereServ.OptionsFind.ShowCloseButton = false;
            this.treeSereServ.OptionsFind.ShowFindButton = false;
            this.treeSereServ.OptionsView.AutoWidth = false;
            this.treeSereServ.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
            this.treeSereServ.OptionsView.ShowHorzLines = false;
            this.treeSereServ.OptionsView.ShowIndicator = false;
            this.treeSereServ.OptionsView.ShowVertLines = false;
            this.treeSereServ.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.spinSoLuong,
            this.chkCheckedColumn,
            this.txtReadOnly});
            this.treeSereServ.SelectImageList = this.imageCollectionStatus;
            this.treeSereServ.Size = new System.Drawing.Size(876, 764);
            this.treeSereServ.StateImageList = this.imageCollectionStatus;
            this.treeSereServ.TabIndex = 17;
            this.treeSereServ.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeSereServHein_GetStateImage);
            this.treeSereServ.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.treeSereServ_GetSelectImage);
            this.treeSereServ.StateImageClick += new DevExpress.XtraTreeList.NodeClickEventHandler(this.treeSereServHein_StateImageClick);
            this.treeSereServ.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeSereServ_CustomDrawNodeCell);
            this.treeSereServ.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeSereServHein_PopupMenuShowing);
            this.treeSereServ.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeSereServ_CellValueChanging);
            this.treeSereServ.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeSereServ_CellValueChanged);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Tên dịch vụ";
            this.treeListColumn1.FieldName = "ServiceName";
            this.treeListColumn1.Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
            this.treeListColumn1.MinWidth = 52;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 221;
            // 
            // treeListColumn5
            // 
            this.treeListColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn5.Caption = "Số lượng";
            this.treeListColumn5.FieldName = "AMOUNT";
            this.treeListColumn5.Format.FormatString = "#,##0.0000";
            this.treeListColumn5.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn5.Name = "treeListColumn5";
            this.treeListColumn5.Visible = true;
            this.treeListColumn5.VisibleIndex = 1;
            this.treeListColumn5.Width = 60;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn3.Caption = "Đơn giá trước VAT";
            this.treeListColumn3.FieldName = "PRICE";
            this.treeListColumn3.Format.FormatString = "#,##0.0000";
            this.treeListColumn3.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn3.Name = "treeListColumn3";
            this.treeListColumn3.OptionsColumn.AllowEdit = false;
            this.treeListColumn3.Visible = true;
            this.treeListColumn3.VisibleIndex = 2;
            this.treeListColumn3.Width = 120;
            // 
            // treeListColumn13
            // 
            this.treeListColumn13.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn13.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn13.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn13.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn13.Caption = "VAT (%)";
            this.treeListColumn13.FieldName = "VAT";
            this.treeListColumn13.Format.FormatString = "#,##0.00";
            this.treeListColumn13.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn13.Name = "treeListColumn13";
            this.treeListColumn13.OptionsColumn.AllowEdit = false;
            this.treeListColumn13.Width = 60;
            // 
            // treeListColumn6
            // 
            this.treeListColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn6.Caption = "Thành tiền";
            this.treeListColumn6.FieldName = "VIR_TOTAL_PRICE";
            this.treeListColumn6.Name = "treeListColumn6";
            this.treeListColumn6.OptionsColumn.AllowEdit = false;
            this.treeListColumn6.Visible = true;
            this.treeListColumn6.VisibleIndex = 3;
            this.treeListColumn6.Width = 120;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn2.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn2.Caption = "Đồng chi trả";
            this.treeListColumn2.FieldName = "VIR_TOTAL_HEIN_PRICE";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowEdit = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 4;
            this.treeListColumn2.Width = 120;
            // 
            // treeListColumn7
            // 
            this.treeListColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn7.Caption = "Bệnh nhân chi trả";
            this.treeListColumn7.FieldName = "VIR_TOTAL_PATIENT_PRICE";
            this.treeListColumn7.Name = "treeListColumn7";
            this.treeListColumn7.OptionsColumn.AllowEdit = false;
            this.treeListColumn7.Visible = true;
            this.treeListColumn7.VisibleIndex = 5;
            this.treeListColumn7.Width = 120;
            // 
            // treeListColumn4
            // 
            this.treeListColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn4.Caption = "Chiết khấu";
            this.treeListColumn4.FieldName = "DISCOUNT";
            this.treeListColumn4.Format.FormatString = "#,##0.0000";
            this.treeListColumn4.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn4.Name = "treeListColumn4";
            this.treeListColumn4.OptionsColumn.AllowEdit = false;
            this.treeListColumn4.Width = 132;
            // 
            // treeListColumn9
            // 
            this.treeListColumn9.Caption = "Mã dịch vụ";
            this.treeListColumn9.FieldName = "SERVICE_CODE";
            this.treeListColumn9.Name = "treeListColumn9";
            this.treeListColumn9.OptionsColumn.AllowEdit = false;
            this.treeListColumn9.Width = 100;
            // 
            // treeListColumn10
            // 
            this.treeListColumn10.Caption = "Mã yêu cầu";
            this.treeListColumn10.FieldName = "SERVICE_REQ_CODE";
            this.treeListColumn10.Name = "treeListColumn10";
            this.treeListColumn10.OptionsColumn.AllowEdit = false;
            this.treeListColumn10.Width = 100;
            // 
            // treeListColumn11
            // 
            this.treeListColumn11.Caption = "Mã giao dịch";
            this.treeListColumn11.FieldName = "Mã giao dịch";
            this.treeListColumn11.Name = "treeListColumn11";
            this.treeListColumn11.OptionsColumn.AllowEdit = false;
            this.treeListColumn11.Width = 100;
            // 
            // treeListColumn8
            // 
            this.treeListColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn8.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn8.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeListColumn8.Caption = "Hao phí";
            this.treeListColumn8.FieldName = "IS_EXPEND_DISPLAY";
            this.treeListColumn8.Name = "treeListColumn8";
            this.treeListColumn8.OptionsColumn.AllowEdit = false;
            this.treeListColumn8.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
            this.treeListColumn8.Width = 72;
            // 
            // treeListColumn12
            // 
            this.treeListColumn12.AppearanceCell.Options.UseTextOptions = true;
            this.treeListColumn12.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn12.AppearanceHeader.Options.UseTextOptions = true;
            this.treeListColumn12.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.treeListColumn12.Caption = "DVKTC";
            this.treeListColumn12.FieldName = "DVKTC";
            this.treeListColumn12.Format.FormatString = "#,##0.0000";
            this.treeListColumn12.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            this.treeListColumn12.Name = "treeListColumn12";
            this.treeListColumn12.ToolTip = "Thuộc gói dịch vụ kỹ thuật cao";
            this.treeListColumn12.Width = 120;
            // 
            // spinSoLuong
            // 
            this.spinSoLuong.AutoHeight = false;
            this.spinSoLuong.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinSoLuong.MaxValue = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.spinSoLuong.Name = "spinSoLuong";
            // 
            // chkCheckedColumn
            // 
            this.chkCheckedColumn.AutoHeight = false;
            this.chkCheckedColumn.Name = "chkCheckedColumn";
            // 
            // txtReadOnly
            // 
            this.txtReadOnly.AutoHeight = false;
            this.txtReadOnly.Name = "txtReadOnly";
            this.txtReadOnly.ReadOnly = true;
            // 
            // imageCollectionStatus
            // 
            this.imageCollectionStatus.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollectionStatus.ImageStream")));
            this.imageCollectionStatus.Images.SetKeyName(0, "Currency_16x16.png");
            this.imageCollectionStatus.Images.SetKeyName(1, "CommaStyle_32x32.png");
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(880, 768);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.treeSereServ;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(880, 768);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCTreeSereServHein
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCTreeSereServHein";
            this.Size = new System.Drawing.Size(880, 768);
            this.Load += new System.EventHandler(this.UCTreeSereServHein_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeSereServ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSoLuong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCheckedColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReadOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollectionStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraTreeList.TreeList treeSereServ;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn5;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn13;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn6;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn7;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn4;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn9;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn10;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn11;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn8;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn12;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit spinSoLuong;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit chkCheckedColumn;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtReadOnly;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.Utils.ImageCollection imageCollectionStatus;
    }
}
