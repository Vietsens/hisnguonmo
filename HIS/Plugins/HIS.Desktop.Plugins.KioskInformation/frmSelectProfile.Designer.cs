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
            this.layoutControl1 = new LayoutControl();
            this.label1 = new Label();
            this.gridControlSelectProfile = new GridControl();
            this.gridViewSelectProfile = new GridView();
            this.gridColumn1 = new GridColumn();
            this.gridColumn2 = new GridColumn();
            this.gridColumn3 = new GridColumn();
            this.gridColumn6 = new GridColumn();
            this.gridColumn5 = new GridColumn();
            this.repositoryItemMemoEditAddress = new RepositoryItemMemoEdit();
            this.layoutControlGroup1 = new LayoutControlGroup();
            this.layoutControlItem2 = new LayoutControlItem();
            this.layoutControlItem1 = new LayoutControlItem();
            this.timerWallpaperSelectForm = new Timer();
            this.timerOffGrid = new Timer();
            ((ISupportInitialize)this.layoutControl1).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((ISupportInitialize)this.gridControlSelectProfile).BeginInit();
            ((ISupportInitialize)this.gridViewSelectProfile).BeginInit();
            ((ISupportInitialize)this.repositoryItemMemoEditAddress).BeginInit();
            ((ISupportInitialize)this.layoutControlGroup1).BeginInit();
            ((ISupportInitialize)this.layoutControlItem2).BeginInit();
            ((ISupportInitialize)this.layoutControlItem1).BeginInit();
            this.SuspendLayout();
            this.layoutControl1.Controls.Add(this.label1);
            this.layoutControl1.Controls.Add(this.gridControlSelectProfile);
            this.layoutControl1.Cursor = Cursors.Arrow;
            this.layoutControl1.Dock = DockStyle.Fill;
            this.layoutControl1.Location = new Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new Size(1150, 450);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            this.label1.Font = new Font("Microsoft Sans Serif", 20f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.ForeColor = Color.Crimson;
            this.label1.Location = new Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new Size(1126, 56);
            this.label1.TabIndex = 6;
            this.label1.Text = "MỜI CHỌN HỒ SƠ PHÙ HỢP";
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.gridControlSelectProfile.Location = new Point(12, 72);
            this.gridControlSelectProfile.MainView = this.gridViewSelectProfile;
            this.gridControlSelectProfile.Name = "gridControlSelectProfile";
            this.gridControlSelectProfile.RepositoryItems.AddRange(new RepositoryItem[]
			{
				this.repositoryItemMemoEditAddress
			});
            this.gridControlSelectProfile.Size = new Size(1126, 366);
            this.gridControlSelectProfile.TabIndex = 5;
            this.gridControlSelectProfile.ViewCollection.AddRange(new BaseView[]
			{
				this.gridViewSelectProfile
			});
            this.gridViewSelectProfile.Appearance.HeaderPanel.Font = new Font("Tahoma", 18f);
            this.gridViewSelectProfile.Appearance.HeaderPanel.ForeColor = Color.DodgerBlue;
            this.gridViewSelectProfile.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewSelectProfile.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridViewSelectProfile.Appearance.Row.Font = new Font("Tahoma", 16f);
            this.gridViewSelectProfile.Appearance.Row.ForeColor = Color.DodgerBlue;
            this.gridViewSelectProfile.Appearance.Row.Options.UseFont = true;
            this.gridViewSelectProfile.Appearance.Row.Options.UseForeColor = true;
            this.gridViewSelectProfile.Appearance.Row.Options.UseTextOptions = true;
            this.gridViewSelectProfile.Appearance.Row.TextOptions.VAlignment = VertAlignment.Top;
            this.gridViewSelectProfile.Columns.AddRange(new GridColumn[]
			{
				this.gridColumn1,
				this.gridColumn2,
				this.gridColumn3,
				this.gridColumn6,
				this.gridColumn5
			});
            this.gridViewSelectProfile.GridControl = this.gridControlSelectProfile;
            this.gridViewSelectProfile.Name = "gridViewSelectProfile";
            this.gridViewSelectProfile.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
            this.gridViewSelectProfile.OptionsView.RowAutoHeight = true;
            this.gridViewSelectProfile.OptionsView.ShowDetailButtons = false;
            this.gridViewSelectProfile.OptionsView.ShowGroupPanel = false;
            this.gridViewSelectProfile.OptionsView.ShowIndicator = false;
            this.gridViewSelectProfile.RowHeight = 55;
            this.gridViewSelectProfile.CustomUnboundColumnData += new CustomColumnDataEventHandler(this.gridViewSelectProfile_CustomUnboundColumnData);
            this.gridViewSelectProfile.Click += new EventHandler(this.gridViewSelectProfile_Click);
            this.gridColumn1.Caption = "STT";
            this.gridColumn1.FieldName = "STT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.UnboundType = UnboundColumnType.Object;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 58;
            this.gridColumn2.Caption = "HỌ TÊN";
            this.gridColumn2.FieldName = "PatientName";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 250;
            this.gridColumn3.Caption = "NGÀY SINH";
            this.gridColumn3.FieldName = "DobStr";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.UnboundType = UnboundColumnType.Object;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 180;
            this.gridColumn6.Caption = "MÃ ĐIỀU TRỊ";
            this.gridColumn6.FieldName = "TreatmentCode";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            this.gridColumn6.Width = 200;
            this.gridColumn5.Caption = "ĐỊA CHỈ";
            this.gridColumn5.ColumnEdit = this.repositoryItemMemoEditAddress;
            this.gridColumn5.FieldName = "PatientAddress";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 360;
            this.repositoryItemMemoEditAddress.Name = "repositoryItemMemoEditAddress";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new BaseLayoutItem[]
			{
				this.layoutControlItem2,
				this.layoutControlItem1
			});
            this.layoutControlGroup1.Location = new Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new Size(1150, 450);
            this.layoutControlGroup1.TextVisible = false;
            this.layoutControlItem2.Control = this.gridControlSelectProfile;
            this.layoutControlItem2.Location = new Point(0, 60);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new Size(1130, 370);
            this.layoutControlItem2.TextSize = new Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            this.layoutControlItem1.Control = this.label1;
            this.layoutControlItem1.Location = new Point(0, 0);
            this.layoutControlItem1.MaxSize = new Size(0, 60);
            this.layoutControlItem1.MinSize = new Size(24, 60);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new Size(1130, 60);
            this.layoutControlItem1.SizeConstraintsType = SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            this.timerWallpaperSelectForm.Interval = 10000;
            this.timerWallpaperSelectForm.Tick += new EventHandler(this.timerWallpaperSelectForm_Tick);
            this.timerOffGrid.Interval = 1000;
            this.timerOffGrid.Tick += new EventHandler(this.timerOffGrid_Tick);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new Size(1150, 450);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmSelectProfile";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "frmSelectProfile";
            this.Load += new EventHandler(this.frmSelectProfile_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((ISupportInitialize)this.layoutControl1).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((ISupportInitialize)this.gridControlSelectProfile).EndInit();
            ((ISupportInitialize)this.gridViewSelectProfile).EndInit();
            ((ISupportInitialize)this.repositoryItemMemoEditAddress).EndInit();
            ((ISupportInitialize)this.layoutControlGroup1).EndInit();
            ((ISupportInitialize)this.layoutControlItem2).EndInit();
            ((ISupportInitialize)this.layoutControlItem1).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
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
    }
}
