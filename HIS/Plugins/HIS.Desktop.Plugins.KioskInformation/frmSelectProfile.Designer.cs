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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace HIS.Desktop.Plugins.KioskInformation
{
    partial class frmSelectProfile
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
            this.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.label1 = new System.Windows.Forms.Label();
            this.gridControlSelectProfile = new DevExpress.XtraGrid.GridControl();
            this.gridViewSelectProfile = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEditAddress = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerWallpaperSelectForm = new System.Windows.Forms.Timer(this.components);
            this.timerOffGrid = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelectProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEditAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.label1);
            this.layoutControl1.Controls.Add(this.gridControlSelectProfile);
            this.layoutControl1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1366, 600);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Crimson;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1342, 56);
            this.label1.TabIndex = 6;
            this.label1.Text = "MỜI CHỌN HỒ SƠ PHÙ HỢP";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gridControlSelectProfile
            // 
            this.gridControlSelectProfile.Location = new System.Drawing.Point(12, 72);
            this.gridControlSelectProfile.MainView = this.gridViewSelectProfile;
            this.gridControlSelectProfile.Name = "gridControlSelectProfile";
            this.gridControlSelectProfile.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEditAddress});
            this.gridControlSelectProfile.Size = new System.Drawing.Size(1342, 516);
            this.gridControlSelectProfile.TabIndex = 5;
            this.gridControlSelectProfile.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSelectProfile});
            // 
            // gridViewSelectProfile
            // 
            this.gridViewSelectProfile.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 18F);
            this.gridViewSelectProfile.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.DodgerBlue;
            this.gridViewSelectProfile.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewSelectProfile.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridViewSelectProfile.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 16F);
            this.gridViewSelectProfile.Appearance.Row.ForeColor = System.Drawing.Color.DodgerBlue;
            this.gridViewSelectProfile.Appearance.Row.Options.UseFont = true;
            this.gridViewSelectProfile.Appearance.Row.Options.UseForeColor = true;
            this.gridViewSelectProfile.Appearance.Row.Options.UseTextOptions = true;
            this.gridViewSelectProfile.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.gridViewSelectProfile.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn6,
            this.gridColumn4,
            this.gridColumn5});
            this.gridViewSelectProfile.GridControl = this.gridControlSelectProfile;
            this.gridViewSelectProfile.Name = "gridViewSelectProfile";
            this.gridViewSelectProfile.OptionsView.ColumnAutoWidth = false;
            this.gridViewSelectProfile.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewSelectProfile.OptionsView.RowAutoHeight = true;
            this.gridViewSelectProfile.OptionsView.ShowDetailButtons = false;
            this.gridViewSelectProfile.OptionsView.ShowGroupPanel = false;
            this.gridViewSelectProfile.OptionsView.ShowIndicator = false;
            this.gridViewSelectProfile.RowHeight = 55;
            this.gridViewSelectProfile.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewSelectProfile_CustomUnboundColumnData);
            this.gridViewSelectProfile.Click += new System.EventHandler(this.gridViewSelectProfile_Click);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 70;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "HỌ TÊN";
            this.gridColumn2.FieldName = "PatientName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 300;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "NGÀY SINH";
            this.gridColumn3.FieldName = "DobStr";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 180;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "MÃ ĐIỀU TRỊ";
            this.gridColumn6.FieldName = "TreatmentCode";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            this.gridColumn6.Width = 180;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "NGÀY VÀO VIỆN";
            this.gridColumn4.FieldName = "IntimeStr";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 180;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "ĐỊA CHỈ";
            this.gridColumn5.ColumnEdit = this.repositoryItemMemoEditAddress;
            this.gridColumn5.FieldName = "PatientAddress";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            this.gridColumn5.Width = 500;
            // 
            // repositoryItemMemoEditAddress
            // 
            this.repositoryItemMemoEditAddress.Name = "repositoryItemMemoEditAddress";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1366, 600);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControlSelectProfile;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 60);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1346, 520);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.label1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 60);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(24, 60);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1346, 60);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // timerWallpaperSelectForm
            // 
            this.timerWallpaperSelectForm.Interval = 10000;
            this.timerWallpaperSelectForm.Tick += new System.EventHandler(this.timerWallpaperSelectForm_Tick);
            // 
            // timerOffGrid
            // 
            this.timerOffGrid.Interval = 1000;
            this.timerOffGrid.Tick += new System.EventHandler(this.timerOffGrid_Tick);
            // 
            // frmSelectProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1366, 600);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSelectProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSelectProfile";
            this.Load += new System.EventHandler(this.frmSelectProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSelectProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSelectProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEditAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private LayoutControl layoutControl1;

        private LayoutControlGroup layoutControlGroup1;

        private Label label1;

        private GridControl gridControlSelectProfile;

        private GridView gridViewSelectProfile;

        private GridColumn gridColumn1;

        private GridColumn gridColumn2;

        private GridColumn gridColumn3;

        private GridColumn gridColumn5;

        private LayoutControlItem layoutControlItem2;

        private LayoutControlItem layoutControlItem1;

        private Timer timerWallpaperSelectForm;

        private Timer timerOffGrid;

        private GridColumn gridColumn6;

        private RepositoryItemMemoEdit repositoryItemMemoEditAddress;
        private GridColumn gridColumn4;
    }
}
