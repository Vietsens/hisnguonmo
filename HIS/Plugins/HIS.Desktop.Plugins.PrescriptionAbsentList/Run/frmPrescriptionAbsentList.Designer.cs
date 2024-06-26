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
namespace HIS.Desktop.Plugins.PrescriptionAbsentList.Run
{
    partial class frmPrescriptionAbsentList
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
            this.layoutControlRoot = new DevExpress.XtraLayout.LayoutControl();
            this.lblDefault01 = new DevExpress.XtraEditors.LabelControl();
            this.pictureEditBranchImage = new DevExpress.XtraEditors.PictureEdit();
            this.gridControlExpMest = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMest = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEditName = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gridColumnGateCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBranchImage = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditBranchImage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEditName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranchImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.layoutControlRoot.Controls.Add(this.lblDefault01);
            this.layoutControlRoot.Controls.Add(this.pictureEditBranchImage);
            this.layoutControlRoot.Controls.Add(this.gridControlExpMest);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(410, 256, 250, 350);
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(884, 881);
            this.layoutControlRoot.TabIndex = 4;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // lblDefault01
            // 
            this.lblDefault01.Appearance.Font = new System.Drawing.Font("Arial", 50.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDefault01.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblDefault01.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblDefault01.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDefault01.Location = new System.Drawing.Point(183, 2);
            this.lblDefault01.Name = "lblDefault01";
            this.lblDefault01.Padding = new System.Windows.Forms.Padding(0, 50, 0, 10);
            this.lblDefault01.Size = new System.Drawing.Size(699, 138);
            this.lblDefault01.StyleController = this.layoutControlRoot;
            this.lblDefault01.TabIndex = 7;
            this.lblDefault01.Text = "ĐÃ GỌI";
            // 
            // pictureEditBranchImage
            // 
            this.pictureEditBranchImage.Location = new System.Drawing.Point(2, 2);
            this.pictureEditBranchImage.MaximumSize = new System.Drawing.Size(500, 0);
            this.pictureEditBranchImage.Name = "pictureEditBranchImage";
            this.pictureEditBranchImage.Properties.AllowFocused = false;
            this.pictureEditBranchImage.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEditBranchImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.pictureEditBranchImage.Size = new System.Drawing.Size(177, 138);
            this.pictureEditBranchImage.StyleController = this.layoutControlRoot;
            this.pictureEditBranchImage.TabIndex = 6;
            // 
            // gridControlExpMest
            // 
            this.gridControlExpMest.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gridControlExpMest.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControlExpMest.Location = new System.Drawing.Point(2, 144);
            this.gridControlExpMest.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.gridControlExpMest.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlExpMest.MainView = this.gridViewExpMest;
            this.gridControlExpMest.Name = "gridControlExpMest";
            this.gridControlExpMest.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEditName});
            this.gridControlExpMest.Size = new System.Drawing.Size(880, 735);
            this.gridControlExpMest.TabIndex = 5;
            this.gridControlExpMest.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMest});
            // 
            // gridViewExpMest
            // 
            this.gridViewExpMest.Appearance.Empty.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridViewExpMest.Appearance.Empty.Options.UseBackColor = true;
            this.gridViewExpMest.Appearance.HorzLine.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridViewExpMest.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridViewExpMest.Appearance.Row.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridViewExpMest.Appearance.Row.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridViewExpMest.Appearance.Row.Options.UseBackColor = true;
            this.gridViewExpMest.Appearance.Row.Options.UseBorderColor = true;
            this.gridViewExpMest.Appearance.VertLine.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.gridViewExpMest.Appearance.VertLine.Options.UseBackColor = true;
            this.gridViewExpMest.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewExpMest.ColumnPanelRowHeight = 100;
            this.gridViewExpMest.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTT,
            this.gridColumnPatientName,
            this.gridColumnGateCode});
            this.gridViewExpMest.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridViewExpMest.GridControl = this.gridControlExpMest;
            this.gridViewExpMest.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewExpMest.Name = "gridViewExpMest";
            this.gridViewExpMest.OptionsFind.AllowFindPanel = false;
            this.gridViewExpMest.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewExpMest.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewExpMest.OptionsView.RowAutoHeight = true;
            this.gridViewExpMest.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMest.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewExpMest.OptionsView.ShowIndicator = false;
            this.gridViewExpMest.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewExpMest.RowHeight = 65;
            this.gridViewExpMest.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // gridColumnSTT
            // 
            this.gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
            this.gridColumnSTT.AppearanceCell.Options.UseFont = true;
            this.gridColumnSTT.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnSTT.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnSTT.AppearanceHeader.ForeColor = System.Drawing.Color.Blue;
            this.gridColumnSTT.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseFont = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.Caption = "STT";
            this.gridColumnSTT.FieldName = "NUM_ORDER";
            this.gridColumnSTT.Name = "gridColumnSTT";
            this.gridColumnSTT.OptionsColumn.AllowEdit = false;
            this.gridColumnSTT.OptionsColumn.AllowFocus = false;
            this.gridColumnSTT.OptionsColumn.AllowShowHide = false;
            this.gridColumnSTT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnSTT.OptionsColumn.ReadOnly = true;
            this.gridColumnSTT.Visible = true;
            this.gridColumnSTT.VisibleIndex = 0;
            this.gridColumnSTT.Width = 176;
            // 
            // gridColumnPatientName
            // 
            this.gridColumnPatientName.AppearanceCell.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnPatientName.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
            this.gridColumnPatientName.AppearanceCell.Options.UseFont = true;
            this.gridColumnPatientName.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnPatientName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnPatientName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnPatientName.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumnPatientName.AppearanceHeader.BackColor = System.Drawing.Color.Blue;
            this.gridColumnPatientName.AppearanceHeader.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnPatientName.AppearanceHeader.ForeColor = System.Drawing.Color.White;
            this.gridColumnPatientName.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnPatientName.AppearanceHeader.Options.UseFont = true;
            this.gridColumnPatientName.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnPatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnPatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnPatientName.Caption = "HỌ VÀ TÊN";
            this.gridColumnPatientName.ColumnEdit = this.repositoryItemMemoEditName;
            this.gridColumnPatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumnPatientName.Name = "gridColumnPatientName";
            this.gridColumnPatientName.OptionsColumn.AllowEdit = false;
            this.gridColumnPatientName.OptionsColumn.AllowFocus = false;
            this.gridColumnPatientName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnPatientName.OptionsColumn.ReadOnly = true;
            this.gridColumnPatientName.Visible = true;
            this.gridColumnPatientName.VisibleIndex = 1;
            this.gridColumnPatientName.Width = 523;
            // 
            // repositoryItemMemoEditName
            // 
            this.repositoryItemMemoEditName.Name = "repositoryItemMemoEditName";
            // 
            // gridColumnGateCode
            // 
            this.gridColumnGateCode.AppearanceCell.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnGateCode.AppearanceCell.ForeColor = System.Drawing.Color.Blue;
            this.gridColumnGateCode.AppearanceCell.Options.UseFont = true;
            this.gridColumnGateCode.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnGateCode.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnGateCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnGateCode.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumnGateCode.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.gridColumnGateCode.AppearanceHeader.Font = new System.Drawing.Font("Arial", 45F, System.Drawing.FontStyle.Bold);
            this.gridColumnGateCode.AppearanceHeader.ForeColor = System.Drawing.Color.Blue;
            this.gridColumnGateCode.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnGateCode.AppearanceHeader.Options.UseFont = true;
            this.gridColumnGateCode.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnGateCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnGateCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnGateCode.Caption = "QUẦY";
            this.gridColumnGateCode.FieldName = "GATE_CODE";
            this.gridColumnGateCode.Name = "gridColumnGateCode";
            this.gridColumnGateCode.OptionsColumn.AllowEdit = false;
            this.gridColumnGateCode.OptionsColumn.AllowFocus = false;
            this.gridColumnGateCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnGateCode.OptionsColumn.ReadOnly = true;
            this.gridColumnGateCode.Visible = true;
            this.gridColumnGateCode.VisibleIndex = 2;
            this.gridColumnGateCode.Width = 181;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciBranchImage,
            this.layoutControlItem3});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(884, 881);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlExpMest;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 142);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(884, 739);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciBranchImage
            // 
            this.lciBranchImage.Control = this.pictureEditBranchImage;
            this.lciBranchImage.Location = new System.Drawing.Point(0, 0);
            this.lciBranchImage.Name = "lciBranchImage";
            this.lciBranchImage.Size = new System.Drawing.Size(181, 142);
            this.lciBranchImage.TextSize = new System.Drawing.Size(0, 0);
            this.lciBranchImage.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblDefault01;
            this.layoutControlItem3.Location = new System.Drawing.Point(181, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(703, 142);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmPrescriptionAbsentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 881);
            this.Controls.Add(this.layoutControlRoot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmPrescriptionAbsentList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách vắng mặt phát thuốc";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmPrescriptionAbsentList_Load);
            this.Controls.SetChildIndex(this.layoutControlRoot, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditBranchImage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEditName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranchImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.PictureEdit pictureEditBranchImage;
        internal DevExpress.XtraGrid.GridControl gridControlExpMest;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMest;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnGateCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciBranchImage;
        private DevExpress.XtraEditors.LabelControl lblDefault01;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEditName;
    }
}
