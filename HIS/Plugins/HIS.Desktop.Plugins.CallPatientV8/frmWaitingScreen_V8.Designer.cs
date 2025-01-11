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
namespace HIS.Desktop.Plugins.CallPatientV8
{
    partial class frmWaitingScreen_V48
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
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.lblTitUp = new System.Windows.Forms.Label();
            this.lblTitDown = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.gridControlWaitingCls = new DevExpress.XtraGrid.GridControl();
            this.gridViewWaitingCls = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gridColumnLastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAge = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnPhuongPhapPhauThuat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.gridColumnGioMoXong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerAutoLoadDataPatient = new System.Windows.Forms.Timer();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.timerSetDataToGridControl = new System.Windows.Forms.Timer();
            this.timer1 = new System.Windows.Forms.Timer();
            this.timerTimeZone = new System.Windows.Forms.Timer();
            this.timerForScrollListPatient = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            this.layoutControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingCls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingCls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1350, 730);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.layoutControl6);
            this.layoutControl2.Location = new System.Drawing.Point(2, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1346, 726);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControl6
            // 
            this.layoutControl6.BackColor = System.Drawing.Color.Lavender;
            this.layoutControl6.Controls.Add(this.lblTitUp);
            this.layoutControl6.Controls.Add(this.lblTitDown);
            this.layoutControl6.Controls.Add(this.lblTime);
            this.layoutControl6.Controls.Add(this.gridControlWaitingCls);
            this.layoutControl6.Location = new System.Drawing.Point(2, 2);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.Root = this.layoutControlGroup5;
            this.layoutControl6.Size = new System.Drawing.Size(1342, 722);
            this.layoutControl6.TabIndex = 4;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // lblTitUp
            // 
            this.lblTitUp.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitUp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.lblTitUp.Location = new System.Drawing.Point(2, 630);
            this.lblTitUp.Name = "lblTitUp";
            this.lblTitUp.Size = new System.Drawing.Size(1338, 40);
            this.lblTitUp.TabIndex = 8;
            this.lblTitUp.Text = "NGƯỜI NHÀ BỆNH NHÂN XEM TRẠNG THÁI TRÊN MÀN HÌNH LCD";
            this.lblTitUp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitDown
            // 
            this.lblTitDown.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.lblTitDown.Location = new System.Drawing.Point(2, 674);
            this.lblTitDown.Name = "lblTitDown";
            this.lblTitDown.Size = new System.Drawing.Size(1338, 46);
            this.lblTitDown.TabIndex = 7;
            this.lblTitDown.Text = "VUI LÒNG KHÔNG ĐI VÀO KHU CHĂM SÓC ĐẶC BIỆT";
            this.lblTitDown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTime
            // 
            this.lblTime.BackColor = System.Drawing.Color.Lavender;
            this.lblTime.Font = new System.Drawing.Font("Arial", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(17)))), ((int)(((byte)(19)))));
            this.lblTime.Location = new System.Drawing.Point(2, 2);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(1320, 69);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "08/01/2025 16:30:01";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gridControlWaitingCls
            // 
            this.gridControlWaitingCls.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gridControlWaitingCls.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControlWaitingCls.Location = new System.Drawing.Point(0, 73);
            this.gridControlWaitingCls.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.gridControlWaitingCls.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlWaitingCls.MainView = this.gridViewWaitingCls;
            this.gridControlWaitingCls.Name = "gridControlWaitingCls";
            this.gridControlWaitingCls.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1,
            this.repositoryItemMemoEdit2,
            this.repositoryItemMemoEdit3});
            this.gridControlWaitingCls.Size = new System.Drawing.Size(1342, 555);
            this.gridControlWaitingCls.TabIndex = 4;
            this.gridControlWaitingCls.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWaitingCls});
            // 
            // gridViewWaitingCls
            // 
            this.gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Empty.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Empty.BorderColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Empty.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Empty.Options.UseBorderColor = true;
            this.gridViewWaitingCls.Appearance.FocusedCell.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.HeaderPanel.BackColor = System.Drawing.Color.White;
            this.gridViewWaitingCls.Appearance.HeaderPanel.BorderColor = System.Drawing.Color.White;
            this.gridViewWaitingCls.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.White;
            this.gridViewWaitingCls.Appearance.HeaderPanel.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.HeaderPanel.Options.UseBorderColor = true;
            this.gridViewWaitingCls.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.gridViewWaitingCls.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewWaitingCls.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewWaitingCls.Appearance.HorzLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.gridViewWaitingCls.Appearance.HorzLine.BorderColor = System.Drawing.Color.White;
            this.gridViewWaitingCls.Appearance.HorzLine.ForeColor = System.Drawing.Color.Red;
            this.gridViewWaitingCls.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.HorzLine.Options.UseBorderColor = true;
            this.gridViewWaitingCls.Appearance.HorzLine.Options.UseForeColor = true;
            this.gridViewWaitingCls.Appearance.OddRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.OddRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Preview.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Preview.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Row.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Row.Options.UseForeColor = true;
            this.gridViewWaitingCls.Appearance.RowSeparator.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.TopNewRow.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.TopNewRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.VertLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.gridViewWaitingCls.Appearance.VertLine.BorderColor = System.Drawing.Color.White;
            this.gridViewWaitingCls.Appearance.VertLine.ForeColor = System.Drawing.Color.Red;
            this.gridViewWaitingCls.Appearance.VertLine.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.VertLine.Options.UseBorderColor = true;
            this.gridViewWaitingCls.Appearance.VertLine.Options.UseForeColor = true;
            this.gridViewWaitingCls.ColumnPanelRowHeight = 70;
            this.gridViewWaitingCls.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTT,
            this.gridColumnLastName,
            this.gridColumnAge,
            this.gridColumnPhuongPhapPhauThuat,
            this.gridColumnGioMoXong,
            this.gridColumn1});
            this.gridViewWaitingCls.GridControl = this.gridControlWaitingCls;
            this.gridViewWaitingCls.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingCls.Name = "gridViewWaitingCls";
            this.gridViewWaitingCls.OptionsFind.AllowFindPanel = false;
            this.gridViewWaitingCls.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewWaitingCls.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewWaitingCls.OptionsView.AllowHtmlDrawHeaders = true;
            this.gridViewWaitingCls.OptionsView.BestFitUseErrorInfo = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingCls.OptionsView.RowAutoHeight = true;
            this.gridViewWaitingCls.OptionsView.ShowGroupPanel = false;
            this.gridViewWaitingCls.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewWaitingCls.OptionsView.ShowIndicator = false;
            this.gridViewWaitingCls.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingCls.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewWaitingCls.RowHeight = 65;
            this.gridViewWaitingCls.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingCls.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewWaitingCls_CustomDrawColumnHeader);
            this.gridViewWaitingCls.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewWaitingCls_RowStyle);
            this.gridViewWaitingCls.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewWaitingCls_CustomUnboundColumnData);
            // 
            // gridColumnSTT
            // 
            this.gridColumnSTT.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.gridColumnSTT.AppearanceCell.Options.UseBackColor = true;
            this.gridColumnSTT.AppearanceCell.Options.UseBorderColor = true;
            this.gridColumnSTT.AppearanceCell.Options.UseFont = true;
            this.gridColumnSTT.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnSTT.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.AppearanceHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnSTT.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(253)))));
            this.gridColumnSTT.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseBorderColor = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseFont = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.Caption = "STT";
            this.gridColumnSTT.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumnSTT.FieldName = "STT";
            this.gridColumnSTT.Name = "gridColumnSTT";
            this.gridColumnSTT.OptionsColumn.AllowEdit = false;
            this.gridColumnSTT.OptionsColumn.AllowFocus = false;
            this.gridColumnSTT.OptionsColumn.AllowMove = false;
            this.gridColumnSTT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnSTT.OptionsColumn.ReadOnly = true;
            this.gridColumnSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnSTT.Visible = true;
            this.gridColumnSTT.VisibleIndex = 0;
            this.gridColumnSTT.Width = 193;
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // gridColumnLastName
            // 
            this.gridColumnLastName.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnLastName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.gridColumnLastName.AppearanceCell.Options.UseBackColor = true;
            this.gridColumnLastName.AppearanceCell.Options.UseBorderColor = true;
            this.gridColumnLastName.AppearanceCell.Options.UseFont = true;
            this.gridColumnLastName.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnLastName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLastName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnLastName.AppearanceHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnLastName.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(253)))));
            this.gridColumnLastName.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnLastName.AppearanceHeader.Options.UseBorderColor = true;
            this.gridColumnLastName.AppearanceHeader.Options.UseFont = true;
            this.gridColumnLastName.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnLastName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnLastName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLastName.Caption = "HỌ TÊN";
            this.gridColumnLastName.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumnLastName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumnLastName.Name = "gridColumnLastName";
            this.gridColumnLastName.OptionsColumn.AllowEdit = false;
            this.gridColumnLastName.OptionsColumn.AllowFocus = false;
            this.gridColumnLastName.OptionsColumn.AllowMove = false;
            this.gridColumnLastName.OptionsColumn.AllowShowHide = false;
            this.gridColumnLastName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnLastName.OptionsColumn.ReadOnly = true;
            this.gridColumnLastName.Visible = true;
            this.gridColumnLastName.VisibleIndex = 1;
            this.gridColumnLastName.Width = 334;
            // 
            // gridColumnAge
            // 
            this.gridColumnAge.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.gridColumnAge.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnAge.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.gridColumnAge.AppearanceCell.Options.UseBackColor = true;
            this.gridColumnAge.AppearanceCell.Options.UseBorderColor = true;
            this.gridColumnAge.AppearanceCell.Options.UseFont = true;
            this.gridColumnAge.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnAge.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnAge.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAge.AppearanceHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnAge.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(253)))));
            this.gridColumnAge.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnAge.AppearanceHeader.Options.UseBorderColor = true;
            this.gridColumnAge.AppearanceHeader.Options.UseFont = true;
            this.gridColumnAge.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnAge.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnAge.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAge.Caption = "TUỔI";
            this.gridColumnAge.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumnAge.FieldName = "AGE_DISPLAY";
            this.gridColumnAge.Name = "gridColumnAge";
            this.gridColumnAge.OptionsColumn.AllowEdit = false;
            this.gridColumnAge.OptionsColumn.AllowFocus = false;
            this.gridColumnAge.OptionsColumn.AllowMove = false;
            this.gridColumnAge.OptionsColumn.AllowShowHide = false;
            this.gridColumnAge.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumnAge.OptionsColumn.ReadOnly = true;
            this.gridColumnAge.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnAge.Visible = true;
            this.gridColumnAge.VisibleIndex = 2;
            this.gridColumnAge.Width = 106;
            // 
            // gridColumnPhuongPhapPhauThuat
            // 
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.Options.UseBackColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.Options.UseBorderColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.Options.UseFont = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.BorderColor = System.Drawing.Color.White;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(253)))));
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Options.UseBorderColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Options.UseFont = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnPhuongPhapPhauThuat.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnPhuongPhapPhauThuat.Caption = "KHOA";
            this.gridColumnPhuongPhapPhauThuat.ColumnEdit = this.repositoryItemMemoEdit2;
            this.gridColumnPhuongPhapPhauThuat.FieldName = "REQUEST_DEPARTMENT_NAME";
            this.gridColumnPhuongPhapPhauThuat.Name = "gridColumnPhuongPhapPhauThuat";
            this.gridColumnPhuongPhapPhauThuat.OptionsColumn.AllowEdit = false;
            this.gridColumnPhuongPhapPhauThuat.OptionsColumn.AllowFocus = false;
            this.gridColumnPhuongPhapPhauThuat.OptionsColumn.AllowMove = false;
            this.gridColumnPhuongPhapPhauThuat.OptionsColumn.AllowShowHide = false;
            this.gridColumnPhuongPhapPhauThuat.OptionsColumn.ReadOnly = true;
            this.gridColumnPhuongPhapPhauThuat.Visible = true;
            this.gridColumnPhuongPhapPhauThuat.VisibleIndex = 3;
            this.gridColumnPhuongPhapPhauThuat.Width = 451;
            // 
            // repositoryItemMemoEdit2
            // 
            this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
            // 
            // gridColumnGioMoXong
            // 
            this.gridColumnGioMoXong.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.gridColumnGioMoXong.AppearanceCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnGioMoXong.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(107)))), ((int)(((byte)(225)))));
            this.gridColumnGioMoXong.AppearanceCell.Options.UseBackColor = true;
            this.gridColumnGioMoXong.AppearanceCell.Options.UseBorderColor = true;
            this.gridColumnGioMoXong.AppearanceCell.Options.UseFont = true;
            this.gridColumnGioMoXong.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnGioMoXong.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnGioMoXong.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnGioMoXong.AppearanceHeader.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridColumnGioMoXong.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnGioMoXong.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(244)))), ((int)(((byte)(253)))));
            this.gridColumnGioMoXong.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnGioMoXong.AppearanceHeader.Options.UseBorderColor = true;
            this.gridColumnGioMoXong.AppearanceHeader.Options.UseFont = true;
            this.gridColumnGioMoXong.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnGioMoXong.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnGioMoXong.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnGioMoXong.Caption = "TRẠNG THÁI";
            this.gridColumnGioMoXong.ColumnEdit = this.repositoryItemMemoEdit1;
            this.gridColumnGioMoXong.FieldName = "DISPLAY_NAME_str";
            this.gridColumnGioMoXong.Name = "gridColumnGioMoXong";
            this.gridColumnGioMoXong.OptionsColumn.AllowEdit = false;
            this.gridColumnGioMoXong.OptionsColumn.AllowFocus = false;
            this.gridColumnGioMoXong.OptionsColumn.AllowMove = false;
            this.gridColumnGioMoXong.OptionsColumn.AllowShowHide = false;
            this.gridColumnGioMoXong.OptionsColumn.ReadOnly = true;
            this.gridColumnGioMoXong.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnGioMoXong.Visible = true;
            this.gridColumnGioMoXong.VisibleIndex = 4;
            this.gridColumnGioMoXong.Width = 256;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "gridColumn1";
            this.gridColumn1.FieldName = "SERVICE_REQ_STT_ID";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // repositoryItemMemoEdit3
            // 
            this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.AppearanceItemCaption.BackColor = System.Drawing.Color.Black;
            this.layoutControlGroup5.AppearanceItemCaption.BorderColor = System.Drawing.Color.Black;
            this.layoutControlGroup5.AppearanceItemCaption.Options.UseBackColor = true;
            this.layoutControlGroup5.AppearanceItemCaption.Options.UseBorderColor = true;
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem12,
            this.layoutControlItem9,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup5.Size = new System.Drawing.Size(1342, 722);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.gridControlWaitingCls;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 73);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem12.Size = new System.Drawing.Size(1342, 555);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.lblTime;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(24, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 20, 2, 2);
            this.layoutControlItem9.Size = new System.Drawing.Size(1342, 73);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.lblTitDown;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 672);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(24, 50);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1342, 50);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblTitUp;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 628);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(24, 30);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1342, 44);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1346, 726);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.layoutControl6;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1346, 726);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.AppearanceGroup.Options.UseBackColor = true;
            this.layoutControlGroup1.AppearanceGroup.Options.UseBorderColor = true;
            this.layoutControlGroup1.AppearanceItemCaption.Options.UseBackColor = true;
            this.layoutControlGroup1.AppearanceItemCaption.Options.UseBorderColor = true;
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1350, 730);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1350, 730);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // timerAutoLoadDataPatient
            // 
            this.timerAutoLoadDataPatient.Tick += new System.EventHandler(this.timerAutoLoadDataPatient_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // timerSetDataToGridControl
            // 
            this.timerSetDataToGridControl.Interval = 2000;
            this.timerSetDataToGridControl.Tick += new System.EventHandler(this.timerSetDataToGridControl_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerTimeZone
            // 
            this.timerTimeZone.Interval = 1000;
            this.timerTimeZone.Tick += new System.EventHandler(this.timerTimeZone_Tick);
            // 
            // timerForScrollListPatient
            // 
            this.timerForScrollListPatient.Interval = 2000;
            this.timerForScrollListPatient.Tick += new System.EventHandler(this.timerForScrollListPatient_Tick);
            // 
            // frmWaitingScreen_V48
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(1350, 730);
            this.Controls.Add(this.layoutControl1);
            this.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.Name = "frmWaitingScreen_V48";
            this.Text = "Màn hình gọi bệnh nhân";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWaitingScreen_V4_FormClosing);
            this.Load += new System.EventHandler(this.frmWaitingScreen_QY_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            this.layoutControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingCls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingCls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl6;
        internal DevExpress.XtraGrid.GridControl gridControlWaitingCls;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWaitingCls;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLastName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAge;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private System.Windows.Forms.Timer timerAutoLoadDataPatient;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer timerSetDataToGridControl;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnPhuongPhapPhauThuat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnGioMoXong;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit3;
        private System.Windows.Forms.Timer timerTimeZone;
        private System.Windows.Forms.Timer timerForScrollListPatient;
        private System.Windows.Forms.Label lblTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private System.Windows.Forms.Label lblTitUp;
        private System.Windows.Forms.Label lblTitDown;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
    }
}
