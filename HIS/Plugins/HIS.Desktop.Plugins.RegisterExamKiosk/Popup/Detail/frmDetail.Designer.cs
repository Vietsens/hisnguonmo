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
namespace HIS.Desktop.Plugins.RegisterExamKiosk.Popup.Detail
{
    partial class frmDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetail));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnThoat = new System.Windows.Forms.Button();
            this.gridControlDetail = new DevExpress.XtraGrid.GridControl();
            this.gridViewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MemoEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MemoEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnThoat);
            this.layoutControl1.Controls.Add(this.gridControlDetail);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1362, 746);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnThoat
            // 
            this.btnThoat.BackColor = System.Drawing.Color.Teal;
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Location = new System.Drawing.Point(586, 663);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(219, 83);
            this.btnThoat.TabIndex = 5;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // gridControlDetail
            // 
            this.gridControlDetail.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.Color.White;
            this.gridControlDetail.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControlDetail.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.gridControlDetail.Location = new System.Drawing.Point(2, 2);
            this.gridControlDetail.MainView = this.gridViewDetail;
            this.gridControlDetail.Name = "gridControlDetail";
            this.gridControlDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.MemoEdit});
            this.gridControlDetail.Size = new System.Drawing.Size(1358, 659);
            this.gridControlDetail.TabIndex = 4;
            this.gridControlDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDetail});
            // 
            // gridViewDetail
            // 
            this.gridViewDetail.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.ColumnFilterButtonActive.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.ColumnFilterButtonActive.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.CustomizationFormHint.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.CustomizationFormHint.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.DetailTip.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.DetailTip.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.Empty.ForeColor = System.Drawing.Color.White;
            this.gridViewDetail.Appearance.Empty.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.EvenRow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.EvenRow.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FilterCloseButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FilterCloseButton.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FilterPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FilterPanel.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FixedLine.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FixedLine.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FocusedCell.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.gridViewDetail.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.FocusedRow.Options.UseFont = true;
            this.gridViewDetail.Appearance.FooterPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.FooterPanel.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.GroupButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.GroupButton.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.GroupFooter.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.GroupFooter.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.GroupPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.GroupPanel.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.GroupRow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.GroupRow.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.HeaderPanel.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.gridViewDetail.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.White;
            this.gridViewDetail.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDetail.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.HorzLine.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.HorzLine.ForeColor = System.Drawing.Color.White;
            this.gridViewDetail.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.HorzLine.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.OddRow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.OddRow.ForeColor = System.Drawing.Color.White;
            this.gridViewDetail.Appearance.OddRow.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.OddRow.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.Preview.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.Preview.ForeColor = System.Drawing.Color.White;
            this.gridViewDetail.Appearance.Preview.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.Preview.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 16.25F);
            this.gridViewDetail.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridViewDetail.Appearance.Row.Options.UseFont = true;
            this.gridViewDetail.Appearance.Row.Options.UseForeColor = true;
            this.gridViewDetail.Appearance.Row.Options.UseTextOptions = true;
            this.gridViewDetail.Appearance.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewDetail.Appearance.RowSeparator.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.SelectedRow.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.VertLine.BackColor = System.Drawing.Color.LightSteelBlue;
            this.gridViewDetail.Appearance.VertLine.Options.UseBackColor = true;
            this.gridViewDetail.Appearance.ViewCaption.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.gridViewDetail.Appearance.ViewCaption.Options.UseFont = true;
            this.gridViewDetail.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gridViewDetail.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5});
            this.gridViewDetail.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewDetail.GridControl = this.gridControlDetail;
            this.gridViewDetail.Name = "gridViewDetail";
            this.gridViewDetail.OptionsCustomization.AllowFilter = false;
            this.gridViewDetail.OptionsCustomization.AllowSort = false;
            this.gridViewDetail.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewDetail.OptionsView.RowAutoHeight = true;
            this.gridViewDetail.OptionsView.ShowGroupPanel = false;
            this.gridViewDetail.OptionsView.ShowIndicator = false;
            this.gridViewDetail.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDetail_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn1.Caption = "Mã điều trị";
            this.gridColumn1.ColumnEdit = this.MemoEdit;
            this.gridColumn1.FieldName = "TREATMENT_CODE";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 154;
            // 
            // MemoEdit
            // 
            this.MemoEdit.Name = "MemoEdit";
            this.MemoEdit.ReadOnly = true;
            this.MemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceCell.ForeColor = System.Drawing.Color.Black;
            this.gridColumn2.AppearanceCell.Options.UseForeColor = true;
            this.gridColumn2.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn2.Caption = "Ngày khám";
            this.gridColumn2.ColumnEdit = this.MemoEdit;
            this.gridColumn2.FieldName = "IN_TIME_STR";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 168;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.ForeColor = System.Drawing.Color.Black;
            this.gridColumn3.AppearanceCell.Options.UseForeColor = true;
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn3.Caption = "Ngày vào viện";
            this.gridColumn3.ColumnEdit = this.MemoEdit;
            this.gridColumn3.FieldName = "CLINICAL_IN_TIME_STR";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 192;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.ForeColor = System.Drawing.Color.Black;
            this.gridColumn4.AppearanceCell.Options.UseForeColor = true;
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn4.Caption = "Ngày ra viện";
            this.gridColumn4.ColumnEdit = this.MemoEdit;
            this.gridColumn4.FieldName = "OUT_TIME_STR";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 190;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceCell.ForeColor = System.Drawing.Color.Black;
            this.gridColumn5.AppearanceCell.Options.UseForeColor = true;
            this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn5.Caption = "Thông tin bệnh";
            this.gridColumn5.ColumnEdit = this.MemoEdit;
            this.gridColumn5.FieldName = "ICD_DETAIL";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 652;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1362, 746);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlDetail;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1362, 663);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnThoat;
            this.layoutControlItem2.Location = new System.Drawing.Point(586, 663);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(219, 83);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.AppearanceItemCaption.BackColor = System.Drawing.Color.LightSteelBlue;
            this.emptySpaceItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.emptySpaceItem1.AppearanceItemCaption.Options.UseBackColor = true;
            this.emptySpaceItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 663);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(586, 83);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.AppearanceItemCaption.BackColor = System.Drawing.Color.LightSteelBlue;
            this.emptySpaceItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.emptySpaceItem2.AppearanceItemCaption.Options.UseBackColor = true;
            this.emptySpaceItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.emptySpaceItem2.Location = new System.Drawing.Point(805, 663);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(557, 83);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 746);
            this.Controls.Add(this.layoutControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lịch sử khám bệnh";
            this.Load += new System.EventHandler(this.frmDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MemoEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDetail;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private System.Windows.Forms.Button btnThoat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit MemoEdit;
    }
}
