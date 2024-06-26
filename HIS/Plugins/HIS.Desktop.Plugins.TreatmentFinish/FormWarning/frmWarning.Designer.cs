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
namespace HIS.Desktop.Plugins.TreatmentFinish.FormWarning
{
    partial class frmWarning
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
            this.chkSkip = new DevExpress.XtraEditors.CheckEdit();
            this.gridControlWarning = new DevExpress.XtraGrid.GridControl();
            this.gridViewWarning = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciSkip = new DevExpress.XtraLayout.LayoutControlItem();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSkip.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWarning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.Controls.Add(this.chkSkip);
            this.layoutControlRoot.Controls.Add(this.gridControlWarning);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(646, 348);
            this.layoutControlRoot.TabIndex = 0;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // chkSkip
            // 
            this.chkSkip.Location = new System.Drawing.Point(562, 327);
            this.chkSkip.Name = "chkSkip";
            this.chkSkip.Properties.Caption = "Bỏ qua";
            this.chkSkip.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkSkip.Size = new System.Drawing.Size(82, 19);
            this.chkSkip.StyleController = this.layoutControlRoot;
            this.chkSkip.TabIndex = 5;
            this.chkSkip.CheckedChanged += new System.EventHandler(this.chkSkip_CheckedChanged);
            // 
            // gridControlWarning
            // 
            this.gridControlWarning.Location = new System.Drawing.Point(2, 2);
            this.gridControlWarning.MainView = this.gridViewWarning;
            this.gridControlWarning.Name = "gridControlWarning";
            this.gridControlWarning.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.gridControlWarning.Size = new System.Drawing.Size(642, 321);
            this.gridControlWarning.TabIndex = 4;
            this.gridControlWarning.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWarning});
            // 
            // gridViewWarning
            // 
            this.gridViewWarning.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnDescription});
            this.gridViewWarning.GridControl = this.gridControlWarning;
            this.gridViewWarning.Name = "gridViewWarning";
            this.gridViewWarning.OptionsView.RowAutoHeight = true;
            this.gridViewWarning.OptionsView.ShowGroupPanel = false;
            this.gridViewWarning.OptionsView.ShowIndicator = false;
            // 
            // gridColumnDescription
            // 
            this.gridColumnDescription.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnDescription.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnDescription.Caption = "Mô tả";
            this.gridColumnDescription.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumnDescription.FieldName = "Description";
            this.gridColumnDescription.Name = "gridColumnDescription";
            this.gridColumnDescription.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnDescription.OptionsEditForm.StartNewRow = true;
            this.gridColumnDescription.Visible = true;
            this.gridColumnDescription.VisibleIndex = 0;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.lciSkip});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(646, 348);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlWarning;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(646, 325);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 325);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(560, 23);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciSkip
            // 
            this.lciSkip.Control = this.chkSkip;
            this.lciSkip.Location = new System.Drawing.Point(560, 325);
            this.lciSkip.Name = "lciSkip";
            this.lciSkip.Size = new System.Drawing.Size(86, 23);
            this.lciSkip.TextSize = new System.Drawing.Size(0, 0);
            this.lciSkip.TextVisible = false;
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            this.repositoryItemMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            // 
            // frmWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(646, 348);
            this.Controls.Add(this.layoutControlRoot);
            this.Name = "frmWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cảnh báo";
            this.Load += new System.EventHandler(this.frmWarning_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSkip.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWarning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSkip)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl gridControlWarning;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWarning;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDescription;
        private DevExpress.XtraEditors.CheckEdit chkSkip;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciSkip;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
    }
}
