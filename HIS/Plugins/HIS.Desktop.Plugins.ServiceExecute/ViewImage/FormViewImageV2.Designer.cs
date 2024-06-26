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
namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    partial class FormViewImageV2
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.layoutView1 = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.gc_Image = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.layoutViewField_gridColumnImage = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gc_Check = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.layoutViewField_gridColumnCheck = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gc_Caption = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutViewField_gridColumnName = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.gc_Stt = new DevExpress.XtraGrid.Columns.LayoutViewColumn();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.layoutViewField_layoutViewColumn1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewField();
            this.layoutViewCard1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewCard();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(992, 539);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(2, 2);
            this.gridControl1.MainView = this.layoutView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemPictureEdit1,
            this.repositoryItemCheckEdit1,
            this.repositoryItemSpinEdit1,
            this.repositoryItemButtonEdit1});
            this.gridControl1.Size = new System.Drawing.Size(988, 535);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.layoutView1});
            // 
            // layoutView1
            // 
            this.layoutView1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.layoutView1.CardHorzInterval = 0;
            this.layoutView1.CardMinSize = new System.Drawing.Size(313, 249);
            this.layoutView1.CardVertInterval = 0;
            this.layoutView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.LayoutViewColumn[] {
            this.gc_Image,
            this.gc_Check,
            this.gc_Caption,
            this.gc_Stt});
            this.layoutView1.GridControl = this.gridControl1;
            this.layoutView1.Name = "layoutView1";
            this.layoutView1.OptionsHeaderPanel.EnableCustomizeButton = false;
            this.layoutView1.OptionsHeaderPanel.ShowCustomizeButton = false;
            this.layoutView1.OptionsView.CardArrangeRule = DevExpress.XtraGrid.Views.Layout.LayoutCardArrangeRule.AllowPartialCards;
            this.layoutView1.OptionsView.ShowCardCaption = false;
            this.layoutView1.OptionsView.ShowCardExpandButton = false;
            this.layoutView1.OptionsView.ViewMode = DevExpress.XtraGrid.Views.Layout.LayoutViewMode.MultiRow;
            this.layoutView1.TemplateCard = this.layoutViewCard1;
            this.layoutView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.layoutView1_CellValueChanged);
            // 
            // gc_Image
            // 
            this.gc_Image.Caption = "Ảnh";
            this.gc_Image.ColumnEdit = this.repositoryItemPictureEdit1;
            this.gc_Image.FieldName = "IMAGE_DISPLAY";
            this.gc_Image.LayoutViewField = this.layoutViewField_gridColumnImage;
            this.gc_Image.Name = "gc_Image";
            this.gc_Image.OptionsColumn.AllowEdit = false;
            this.gc_Image.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gc_Image.OptionsFilter.AllowFilter = false;
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
            // 
            // layoutViewField_gridColumnImage
            // 
            this.layoutViewField_gridColumnImage.EditorPreferredWidth = 313;
            this.layoutViewField_gridColumnImage.Location = new System.Drawing.Point(0, 0);
            this.layoutViewField_gridColumnImage.Name = "layoutViewField_gridColumnImage";
            this.layoutViewField_gridColumnImage.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutViewField_gridColumnImage.Size = new System.Drawing.Size(313, 225);
            this.layoutViewField_gridColumnImage.StartNewLine = true;
            this.layoutViewField_gridColumnImage.TextSize = new System.Drawing.Size(0, 0);
            this.layoutViewField_gridColumnImage.TextVisible = false;
            // 
            // gc_Check
            // 
            this.gc_Check.Caption = "Chọn";
            this.gc_Check.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gc_Check.FieldName = "IsChecked";
            this.gc_Check.LayoutViewField = this.layoutViewField_gridColumnCheck;
            this.gc_Check.Name = "gc_Check";
            this.gc_Check.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gc_Check.OptionsFilter.AllowFilter = false;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.CheckedChanged += new System.EventHandler(this.repositoryItemCheckEdit1_CheckedChanged);
            // 
            // layoutViewField_gridColumnCheck
            // 
            this.layoutViewField_gridColumnCheck.EditorPreferredWidth = 22;
            this.layoutViewField_gridColumnCheck.Location = new System.Drawing.Point(287, 225);
            this.layoutViewField_gridColumnCheck.Name = "layoutViewField_gridColumnCheck";
            this.layoutViewField_gridColumnCheck.Size = new System.Drawing.Size(26, 24);
            this.layoutViewField_gridColumnCheck.TextSize = new System.Drawing.Size(0, 0);
            this.layoutViewField_gridColumnCheck.TextVisible = false;
            // 
            // gc_Caption
            // 
            this.gc_Caption.Caption = "Ghi chú";
            this.gc_Caption.ColumnEdit = this.repositoryItemButtonEdit1;
            this.gc_Caption.CustomizationCaption = "Ghi chú";
            this.gc_Caption.FieldName = "CAPTION";
            this.gc_Caption.LayoutViewField = this.layoutViewField_gridColumnName;
            this.gc_Caption.Name = "gc_Caption";
            this.gc_Caption.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gc_Caption.OptionsFilter.AllowFilter = false;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonClick);
            // 
            // layoutViewField_gridColumnName
            // 
            this.layoutViewField_gridColumnName.EditorPreferredWidth = 140;
            this.layoutViewField_gridColumnName.Location = new System.Drawing.Point(0, 225);
            this.layoutViewField_gridColumnName.Name = "layoutViewField_gridColumnName";
            this.layoutViewField_gridColumnName.Size = new System.Drawing.Size(188, 24);
            this.layoutViewField_gridColumnName.TextSize = new System.Drawing.Size(39, 13);
            // 
            // gc_Stt
            // 
            this.gc_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gc_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gc_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gc_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gc_Stt.Caption = "Stt";
            this.gc_Stt.ColumnEdit = this.repositoryItemSpinEdit1;
            this.gc_Stt.FieldName = "STTImage";
            this.gc_Stt.LayoutViewField = this.layoutViewField_layoutViewColumn1;
            this.gc_Stt.Name = "gc_Stt";
            this.gc_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gc_Stt.OptionsFilter.AllowFilter = false;
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[] {
            1410065407,
            2,
            0,
            0});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            this.repositoryItemSpinEdit1.EditValueChanged += new System.EventHandler(this.repositoryItemSpinEdit1_EditValueChanged);
            // 
            // layoutViewField_layoutViewColumn1
            // 
            this.layoutViewField_layoutViewColumn1.EditorPreferredWidth = 51;
            this.layoutViewField_layoutViewColumn1.Location = new System.Drawing.Point(188, 225);
            this.layoutViewField_layoutViewColumn1.Name = "layoutViewField_layoutViewColumn1";
            this.layoutViewField_layoutViewColumn1.Size = new System.Drawing.Size(99, 24);
            this.layoutViewField_layoutViewColumn1.TextSize = new System.Drawing.Size(39, 13);
            // 
            // layoutViewCard1
            // 
            this.layoutViewCard1.CustomizationFormText = "TemplateCard";
            this.layoutViewCard1.GroupBordersVisible = false;
            this.layoutViewCard1.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.layoutViewCard1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutViewField_gridColumnImage,
            this.layoutViewField_gridColumnName,
            this.layoutViewField_gridColumnCheck,
            this.layoutViewField_layoutViewColumn1});
            this.layoutViewCard1.Name = "layoutViewCard1";
            this.layoutViewCard1.OptionsItemText.TextToControlDistance = 5;
            this.layoutViewCard1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutViewCard1.Text = "TemplateCard";
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(992, 539);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(992, 539);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // FormViewImageV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 539);
            this.Controls.Add(this.layoutControl1);
            this.Name = "FormViewImageV2";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormViewImageV2_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormViewImageV2_FormClosed);
            this.Load += new System.EventHandler(this.FormViewImageV2_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_gridColumnName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewField_layoutViewColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Layout.LayoutView layoutView1;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gc_Image;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gc_Check;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gc_Caption;
        private DevExpress.XtraGrid.Columns.LayoutViewColumn gc_Stt;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumnImage;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumnCheck;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_gridColumnName;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewField layoutViewField_layoutViewColumn1;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewCard layoutViewCard1;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
    }
}
