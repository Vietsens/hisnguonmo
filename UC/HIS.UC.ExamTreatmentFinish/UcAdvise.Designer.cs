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
namespace HIS.UC.ExamTreatmentFinish
{
    partial class UcAdvise
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcAdvise));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.txtSearch = new DevExpress.XtraEditors.TextEdit();
            this.txtAdvise = new DevExpress.XtraEditors.MemoEdit();
            this.gridControlRoomExam = new DevExpress.XtraGrid.GridControl();
            this.gridViewRoomExam = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnCheck = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.imageListIcon = new System.Windows.Forms.ImageList();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdvise.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRoomExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoomExam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.xtraScrollableControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(450, 250);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.layoutControl2);
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(450, 250);
            this.xtraScrollableControl1.TabIndex = 7;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.txtSearch);
            this.layoutControl2.Controls.Add(this.txtAdvise);
            this.layoutControl2.Controls.Add(this.gridControlRoomExam);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(450, 250);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(109, 4);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.txtSearch.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtSearch.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtSearch.Size = new System.Drawing.Size(337, 20);
            this.txtSearch.StyleController = this.layoutControl2;
            this.txtSearch.TabIndex = 6;
            this.txtSearch.EditValueChanged += new System.EventHandler(this.txtSearch_EditValueChanged);
            // 
            // txtAdvise
            // 
            this.txtAdvise.Location = new System.Drawing.Point(109, 161);
            this.txtAdvise.Name = "txtAdvise";
            this.txtAdvise.Properties.MaxLength = 500;
            this.txtAdvise.Properties.NullValuePrompt = "Lời dặn bác sĩ";
            this.txtAdvise.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtAdvise.Size = new System.Drawing.Size(337, 85);
            this.txtAdvise.StyleController = this.layoutControl2;
            this.txtAdvise.TabIndex = 4;
            // 
            // gridControlRoomExam
            // 
            this.gridControlRoomExam.Location = new System.Drawing.Point(109, 28);
            this.gridControlRoomExam.MainView = this.gridViewRoomExam;
            this.gridControlRoomExam.Name = "gridControlRoomExam";
            this.gridControlRoomExam.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlRoomExam.Size = new System.Drawing.Size(337, 129);
            this.gridControlRoomExam.TabIndex = 5;
            this.gridControlRoomExam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewRoomExam});
            // 
            // gridViewRoomExam
            // 
            this.gridViewRoomExam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnCheck,
            this.gridColumn2,
            this.gridColumn3});
            this.gridViewRoomExam.GridControl = this.gridControlRoomExam;
            this.gridViewRoomExam.Images = this.imageListIcon;
            this.gridViewRoomExam.Name = "gridViewRoomExam";
            this.gridViewRoomExam.OptionsView.ShowGroupPanel = false;
            this.gridViewRoomExam.OptionsView.ShowIndicator = false;
            this.gridViewRoomExam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewRoomExam_MouseDown);
            // 
            // gridColumnCheck
            // 
            this.gridColumnCheck.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumnCheck.FieldName = "IsCheck";
            this.gridColumnCheck.Image = global::HIS.UC.ExamTreatmentFinish.Properties.Resources.dau_tích_02;
            this.gridColumnCheck.ImageAlignment = System.Drawing.StringAlignment.Center;
            this.gridColumnCheck.Name = "gridColumnCheck";
            this.gridColumnCheck.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.OptionsColumn.ShowCaption = false;
            this.gridColumnCheck.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnCheck.OptionsFilter.AllowFilter = false;
            this.gridColumnCheck.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.OptionsFilter.ImmediateUpdateAutoFilter = false;
            this.gridColumnCheck.OptionsFilter.ImmediateUpdatePopupDateFilterOnCheck = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.OptionsFilter.ImmediateUpdatePopupDateFilterOnDateChange = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnCheck.Visible = true;
            this.gridColumnCheck.VisibleIndex = 0;
            this.gridColumnCheck.Width = 26;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Mã";
            this.gridColumn2.FieldName = "ROOM_CODE";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 54;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Tên";
            this.gridColumn3.FieldName = "ROOM_NAME";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 280;
            // 
            // imageListIcon
            // 
            this.imageListIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListIcon.ImageStream")));
            this.imageListIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListIcon.Images.SetKeyName(0, "01.png");
            this.imageListIcon.Images.SetKeyName(1, "02.png");
            this.imageListIcon.Images.SetKeyName(2, "04.png");
            this.imageListIcon.Images.SetKeyName(3, "03.png");
            this.imageListIcon.Images.SetKeyName(4, "05.png");
            this.imageListIcon.Images.SetKeyName(5, "dau tích-01.jpg");
            this.imageListIcon.Images.SetKeyName(6, "dau tích-02.jpg");
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup2.Size = new System.Drawing.Size(450, 250);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(446, 24);
            this.layoutControlItem3.Text = " ";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.gridControlRoomExam;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(446, 133);
            this.layoutControlItem2.Text = "PK lần sau:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.txtAdvise;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 157);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(446, 89);
            this.layoutControlItem1.Text = "Lời dặn bác sĩ:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(100, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(450, 250);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.xtraScrollableControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem4.Size = new System.Drawing.Size(450, 250);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // UcAdvise
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UcAdvise";
            this.Size = new System.Drawing.Size(450, 250);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.xtraScrollableControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtSearch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAdvise.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlRoomExam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewRoomExam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.MemoEdit txtAdvise;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraGrid.GridControl gridControlRoomExam;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewRoomExam;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.ImageList imageListIcon;
        private DevExpress.XtraEditors.TextEdit txtSearch;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;

    }
}
