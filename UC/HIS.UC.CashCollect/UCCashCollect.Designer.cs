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
namespace HIS.UC.CashCollect
{
    partial class UCCashCollect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCashCollect));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlCashCollect = new DevExpress.XtraGrid.GridControl();
            this.gridViewCashCollect = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemCheck__Enable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemCheck__Disable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemButton_Disable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemButton_Enable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCashCollect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCashCollect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Disable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Enable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlCashCollect);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(923, 578);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlCashCollect
            // 
            this.gridControlCashCollect.Location = new System.Drawing.Point(3, 3);
            this.gridControlCashCollect.MainView = this.gridViewCashCollect;
            this.gridControlCashCollect.Name = "gridControlCashCollect";
            this.gridControlCashCollect.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheck__Enable,
            this.repositoryItemCheck__Disable,
            this.repositoryItemButton_Disable,
            this.repositoryItemButton_Enable});
            this.gridControlCashCollect.Size = new System.Drawing.Size(917, 572);
            this.gridControlCashCollect.TabIndex = 4;
            this.gridControlCashCollect.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCashCollect});
            // 
            // gridViewCashCollect
            // 
            this.gridViewCashCollect.GridControl = this.gridControlCashCollect;
            this.gridViewCashCollect.Name = "gridViewCashCollect";
            this.gridViewCashCollect.OptionsView.ColumnAutoWidth = false;
            this.gridViewCashCollect.OptionsView.ShowGroupPanel = false;
            this.gridViewCashCollect.OptionsView.ShowIndicator = false;
            this.gridViewCashCollect.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewCashCollect_CustomRowCellEdit);
            this.gridViewCashCollect.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewCashCollect_CellValueChanged);
            this.gridViewCashCollect.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewCashCollect_CustomUnboundColumnData);
            this.gridViewCashCollect.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewCashCollect_MouseDown);
            // 
            // repositoryItemCheck__Enable
            // 
            this.repositoryItemCheck__Enable.AutoHeight = false;
            this.repositoryItemCheck__Enable.Name = "repositoryItemCheck__Enable";
            // 
            // repositoryItemCheck__Disable
            // 
            this.repositoryItemCheck__Disable.AutoHeight = false;
            this.repositoryItemCheck__Disable.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Style2;
            this.repositoryItemCheck__Disable.Name = "repositoryItemCheck__Disable";
            this.repositoryItemCheck__Disable.ReadOnly = true;
            // 
            // repositoryItemButton_Disable
            // 
            this.repositoryItemButton_Disable.AutoHeight = false;
            this.repositoryItemButton_Disable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButton_Disable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Hủy", null, null, true)});
            this.repositoryItemButton_Disable.Name = "repositoryItemButton_Disable";
            this.repositoryItemButton_Disable.ReadOnly = true;
            this.repositoryItemButton_Disable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // repositoryItemButton_Enable
            // 
            this.repositoryItemButton_Enable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButton_Enable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Hủy ", null, null, true)});
            this.repositoryItemButton_Enable.Name = "repositoryItemButton_Enable";
            this.repositoryItemButton_Enable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButton_Enable.Click += new System.EventHandler(this.repositoryItemButton_Enable_Click);
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
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(923, 578);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlCashCollect;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(923, 578);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // UCCashCollect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCCashCollect";
            this.Size = new System.Drawing.Size(923, 578);
            this.Load += new System.EventHandler(this.UCCashCollect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlCashCollect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCashCollect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheck__Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Disable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButton_Enable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlCashCollect;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCashCollect;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Enable;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheck__Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton_Disable;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButton_Enable;
    }
}
