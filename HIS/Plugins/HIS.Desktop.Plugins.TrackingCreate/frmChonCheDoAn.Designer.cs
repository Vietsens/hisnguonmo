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
namespace HIS.Desktop.Plugins.TrackingCreate
{
    partial class frmChonCheDoAn
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
            this.btnDongY = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.grcCheDoAn = new DevExpress.XtraGrid.GridControl();
            this.grdCheDoAn = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdColCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdColDietCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColNutritionInfo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemo = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.grdColProcessingForm = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdColTreatmentApply = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciKeyWord = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnDongY = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcCheDoAn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdCheDoAn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDongY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.btnDongY);
            this.layoutControl1.Controls.Add(this.txtKeyWord);
            this.layoutControl1.Controls.Add(this.grcCheDoAn);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(884, 461);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            //
            // btnDongY
            //
            this.btnDongY.Location = new System.Drawing.Point(762, 437);
            this.btnDongY.Name = "btnDongY";
            this.btnDongY.Size = new System.Drawing.Size(120, 22);
            this.btnDongY.StyleController = this.layoutControl1;
            this.btnDongY.TabIndex = 2;
            this.btnDongY.Text = "Đồng ý (Ctrl S)";
            this.btnDongY.Click += new System.EventHandler(this.btnDongY_Click);
            //
            // txtKeyWord
            //
            this.txtKeyWord.Location = new System.Drawing.Point(2, 2);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Nhập mã chế độ ăn hoặc áp dụng điều trị để tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyWord.Size = new System.Drawing.Size(880, 20);
            this.txtKeyWord.StyleController = this.layoutControl1;
            this.txtKeyWord.TabIndex = 0;
            this.txtKeyWord.EditValueChanged += new System.EventHandler(this.txtKeyWord_EditValueChanged);
            this.txtKeyWord.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyWord_KeyDown);
            //
            // grcCheDoAn
            //
            this.grcCheDoAn.Location = new System.Drawing.Point(2, 26);
            this.grcCheDoAn.MainView = this.grdCheDoAn;
            this.grcCheDoAn.Name = "grcCheDoAn";
            this.grcCheDoAn.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck,
            this.repositoryItemMemo});
            this.grcCheDoAn.Size = new System.Drawing.Size(880, 407);
            this.grcCheDoAn.TabIndex = 1;
            this.grcCheDoAn.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdCheDoAn});
            //
            // grdCheDoAn
            //
            this.grdCheDoAn.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdColCheck,
            this.grdColDietCode,
            this.grdColNutritionInfo,
            this.grdColProcessingForm,
            this.grdColTreatmentApply});
            this.grdCheDoAn.GridControl = this.grcCheDoAn;
            this.grdCheDoAn.Name = "grdCheDoAn";
            this.grdCheDoAn.OptionsBehavior.Editable = false;
            this.grdCheDoAn.OptionsCustomization.AllowFilter = false;
            this.grdCheDoAn.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdCheDoAn.OptionsView.RowAutoHeight = true;
            this.grdCheDoAn.OptionsView.ShowGroupPanel = false;
            this.grdCheDoAn.OptionsView.ShowIndicator = false;
            this.grdCheDoAn.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.grdCheDoAn_RowCellClick);
            this.grdCheDoAn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdCheDoAn_KeyDown);
            this.grdCheDoAn.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.grdCheDoAn_CustomDrawColumnHeader);
            this.grdCheDoAn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdCheDoAn_MouseDown);
            //
            // grdColCheck
            //
            this.grdColCheck.ColumnEdit = this.repositoryItemCheck;
            this.grdColCheck.FieldName = "IsChecked";
            this.grdColCheck.Name = "grdColCheck";
            this.grdColCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.grdColCheck.OptionsColumn.FixedWidth = true;
            this.grdColCheck.OptionsColumn.ShowCaption = false;
            this.grdColCheck.ToolTip = "Chọn tất cả";
            this.grdColCheck.Visible = true;
            this.grdColCheck.VisibleIndex = 0;
            this.grdColCheck.Width = 30;
            //
            // repositoryItemCheck
            //
            this.repositoryItemCheck.AutoHeight = false;
            this.repositoryItemCheck.Name = "repositoryItemCheck";
            //
            // grdColDietCode
            //
            this.grdColDietCode.Caption = "Mã chế độ ăn";
            this.grdColDietCode.FieldName = "DIET_CODE";
            this.grdColDietCode.Name = "grdColDietCode";
            this.grdColDietCode.Visible = true;
            this.grdColDietCode.VisibleIndex = 1;
            this.grdColDietCode.Width = 90;
            //
            // grdColNutritionInfo
            //
            this.grdColNutritionInfo.Caption = "Thành phần dinh dưỡng/24h";
            this.grdColNutritionInfo.ColumnEdit = this.repositoryItemMemo;
            this.grdColNutritionInfo.FieldName = "NUTRITION_INFO";
            this.grdColNutritionInfo.Name = "grdColNutritionInfo";
            this.grdColNutritionInfo.Visible = true;
            this.grdColNutritionInfo.VisibleIndex = 2;
            this.grdColNutritionInfo.Width = 250;
            //
            // repositoryItemMemo
            //
            this.repositoryItemMemo.Name = "repositoryItemMemo";
            //
            // grdColProcessingForm
            //
            this.grdColProcessingForm.Caption = "Dạng chế biến";
            this.grdColProcessingForm.ColumnEdit = this.repositoryItemMemo;
            this.grdColProcessingForm.FieldName = "PROCESSING_FORM";
            this.grdColProcessingForm.Name = "grdColProcessingForm";
            this.grdColProcessingForm.Visible = true;
            this.grdColProcessingForm.VisibleIndex = 3;
            this.grdColProcessingForm.Width = 250;
            //
            // grdColTreatmentApply
            //
            this.grdColTreatmentApply.Caption = "Áp dụng điều trị";
            this.grdColTreatmentApply.ColumnEdit = this.repositoryItemMemo;
            this.grdColTreatmentApply.FieldName = "TREATMENT_APPLY";
            this.grdColTreatmentApply.Name = "grdColTreatmentApply";
            this.grdColTreatmentApply.Visible = true;
            this.grdColTreatmentApply.VisibleIndex = 4;
            this.grdColTreatmentApply.Width = 240;
            //
            // layoutControlGroup1
            //
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciKeyWord,
            this.lciGrid,
            this.lciBtnDongY,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(884, 461);
            this.layoutControlGroup1.TextVisible = false;
            //
            // lciKeyWord
            //
            this.lciKeyWord.Control = this.txtKeyWord;
            this.lciKeyWord.Location = new System.Drawing.Point(0, 0);
            this.lciKeyWord.Name = "lciKeyWord";
            this.lciKeyWord.Size = new System.Drawing.Size(884, 24);
            this.lciKeyWord.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyWord.TextVisible = false;
            //
            // lciGrid
            //
            this.lciGrid.Control = this.grcCheDoAn;
            this.lciGrid.Location = new System.Drawing.Point(0, 24);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(884, 411);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            //
            // lciBtnDongY
            //
            this.lciBtnDongY.Control = this.btnDongY;
            this.lciBtnDongY.Location = new System.Drawing.Point(760, 435);
            this.lciBtnDongY.Name = "lciBtnDongY";
            this.lciBtnDongY.Size = new System.Drawing.Size(124, 26);
            this.lciBtnDongY.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDongY.TextVisible = false;
            //
            // emptySpaceItem1
            //
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 435);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(760, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            //
            // frmChonCheDoAn
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.layoutControl1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChonCheDoAn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn chế độ ăn";
            this.Load += new System.EventHandler(this.frmChonCheDoAn_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmChonCheDoAn_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcCheDoAn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdCheDoAn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDongY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyWord;
        private DevExpress.XtraGrid.GridControl grcCheDoAn;
        private DevExpress.XtraGrid.Views.Grid.GridView grdCheDoAn;
        private DevExpress.XtraGrid.Columns.GridColumn grdColCheck;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck;
        private DevExpress.XtraGrid.Columns.GridColumn grdColDietCode;
        private DevExpress.XtraGrid.Columns.GridColumn grdColNutritionInfo;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemo;
        private DevExpress.XtraGrid.Columns.GridColumn grdColProcessingForm;
        private DevExpress.XtraGrid.Columns.GridColumn grdColTreatmentApply;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
        private DevExpress.XtraEditors.SimpleButton btnDongY;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDongY;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
