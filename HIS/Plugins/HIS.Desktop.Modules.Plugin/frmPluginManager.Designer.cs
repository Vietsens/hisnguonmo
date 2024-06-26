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
namespace Inventec.Desktop.Modules.Plugin
{
    partial class frmPluginManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPluginManager));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlLibOfPlugin = new DevExpress.XtraGrid.GridControl();
            this.gridViewLibOfPlugin = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControlPlugins = new DevExpress.XtraGrid.GridControl();
            this.gridViewPlugins = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLibOfPlugin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLibOfPlugin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPlugins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPlugins)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControlLibOfPlugin);
            this.layoutControl1.Controls.Add(this.gridControlPlugins);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(864, 433);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControlLibOfPlugin
            // 
            this.gridControlLibOfPlugin.Location = new System.Drawing.Point(2, 218);
            this.gridControlLibOfPlugin.MainView = this.gridViewLibOfPlugin;
            this.gridControlLibOfPlugin.Name = "gridControlLibOfPlugin";
            this.gridControlLibOfPlugin.Size = new System.Drawing.Size(860, 213);
            this.gridControlLibOfPlugin.TabIndex = 5;
            this.gridControlLibOfPlugin.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLibOfPlugin});
            // 
            // gridViewLibOfPlugin
            // 
            this.gridViewLibOfPlugin.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn4,
            this.gridColumn5});
            this.gridViewLibOfPlugin.GridControl = this.gridControlLibOfPlugin;
            this.gridViewLibOfPlugin.Name = "gridViewLibOfPlugin";
            this.gridViewLibOfPlugin.OptionsView.ColumnAutoWidth = false;
            this.gridViewLibOfPlugin.OptionsView.ShowGroupPanel = false;
            this.gridViewLibOfPlugin.OptionsView.ShowIndicator = false;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Mã chức năng";
            this.gridColumn6.FieldName = "Code";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 0;
            this.gridColumn6.Width = 90;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Tên chức năng";
            this.gridColumn7.FieldName = "Name";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 1;
            this.gridColumn7.Width = 160;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Key ngôn ngữ";
            this.gridColumn8.FieldName = "KeyLanguage";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 2;
            this.gridColumn8.Width = 250;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Mô tả";
            this.gridColumn9.FieldName = "Description";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 3;
            this.gridColumn9.Width = 109;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Mã chức năng cha";
            this.gridColumn10.FieldName = "ParentCode";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 4;
            this.gridColumn10.Width = 100;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Nhóm";
            this.gridColumn11.FieldName = "GroupName";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 5;
            this.gridColumn11.Width = 70;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Key ngôn ngữ chức năng cha";
            this.gridColumn4.FieldName = "GroupCaptionLanguageKey";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 6;
            this.gridColumn4.Width = 250;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Lớp sử dụng";
            this.gridColumn5.FieldName = "FormalName";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 7;
            this.gridColumn5.Width = 150;
            // 
            // gridControlPlugins
            // 
            this.gridControlPlugins.Location = new System.Drawing.Point(2, 2);
            this.gridControlPlugins.MainView = this.gridViewPlugins;
            this.gridControlPlugins.Name = "gridControlPlugins";
            this.gridControlPlugins.Size = new System.Drawing.Size(860, 212);
            this.gridControlPlugins.TabIndex = 4;
            this.gridControlPlugins.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPlugins});
            this.gridControlPlugins.Click += new System.EventHandler(this.gridControlPlugins_Click);
            // 
            // gridViewPlugins
            // 
            this.gridViewPlugins.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gridViewPlugins.GridControl = this.gridControlPlugins;
            this.gridViewPlugins.Name = "gridViewPlugins";
            this.gridViewPlugins.OptionsView.ShowGroupPanel = false;
            this.gridViewPlugins.OptionsView.ShowIndicator = false;
            this.gridViewPlugins.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewPlugins_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Tên plugin";
            this.gridColumn1.FieldName = "NameDisplay";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 176;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Mô tả";
            this.gridColumn2.FieldName = "FormalName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 267;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Đường dẫn";
            this.gridColumn3.FieldName = "Location";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 300;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(864, 433);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlPlugins;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(864, 216);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControlLibOfPlugin;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 216);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(864, 217);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // frmPluginManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 433);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPluginManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý plugins";
            this.Load += new System.EventHandler(this.frmPluginManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlLibOfPlugin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLibOfPlugin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlPlugins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewPlugins)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlLibOfPlugin;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLibOfPlugin;
        private DevExpress.XtraGrid.GridControl gridControlPlugins;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPlugins;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
    }
}
