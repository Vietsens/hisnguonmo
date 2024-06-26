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
namespace HIS.Desktop.Plugins.CallPatientCLS
{
    partial class frmWaitingScreen_QY2
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
            this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
            this.lblSrollText = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutPatientCallNow = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.lblRoomName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciUsername = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlWaitingCls = new DevExpress.XtraGrid.GridControl();
            this.gridViewWaitingCls = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFirstName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAge = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceReqStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnInstructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceReqType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerForHightLightCallPatientLayout = new System.Windows.Forms.Timer();
            this.timerForScrollTextBottom = new System.Windows.Forms.Timer();
            this.timerForScrollListPatient = new System.Windows.Forms.Timer();
            this.timerAutoLoadDataPatient = new System.Windows.Forms.Timer();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.timerSetDataToGridControl = new System.Windows.Forms.Timer();
            this.timerAutoRefactorName = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientCallNow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUsername)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).BeginInit();
            this.layoutControl6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingCls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingCls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl5);
            this.layoutControl1.Controls.Add(this.layoutControl4);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1800, 898);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.lblSrollText);
            this.layoutControl5.Location = new System.Drawing.Point(3, 841);
            this.layoutControl5.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroup4;
            this.layoutControl5.Size = new System.Drawing.Size(1794, 54);
            this.layoutControl5.TabIndex = 7;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // lblSrollText
            // 
            this.lblSrollText.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblSrollText.Appearance.Font = new System.Drawing.Font("Tahoma", 22F);
            this.lblSrollText.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblSrollText.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSrollText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSrollText.Location = new System.Drawing.Point(3, 3);
            this.lblSrollText.Margin = new System.Windows.Forms.Padding(4);
            this.lblSrollText.Name = "lblSrollText";
            this.lblSrollText.Size = new System.Drawing.Size(1788, 45);
            this.lblSrollText.StyleController = this.layoutControl5;
            this.lblSrollText.TabIndex = 4;
            // 
            // layoutControlGroup4
            // 
            this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup4.GroupBordersVisible = false;
            this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutPatientCallNow});
            this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup4.Name = "layoutControlGroup4";
            this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup4.Size = new System.Drawing.Size(1794, 54);
            this.layoutControlGroup4.TextVisible = false;
            // 
            // layoutPatientCallNow
            // 
            this.layoutPatientCallNow.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 20F);
            this.layoutPatientCallNow.AppearanceItemCaption.ForeColor = System.Drawing.Color.White;
            this.layoutPatientCallNow.AppearanceItemCaption.Options.UseFont = true;
            this.layoutPatientCallNow.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutPatientCallNow.Control = this.lblSrollText;
            this.layoutPatientCallNow.Location = new System.Drawing.Point(0, 0);
            this.layoutPatientCallNow.Name = "layoutPatientCallNow";
            this.layoutPatientCallNow.Size = new System.Drawing.Size(1794, 54);
            this.layoutPatientCallNow.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPatientCallNow.TextSize = new System.Drawing.Size(0, 0);
            this.layoutPatientCallNow.TextToControlDistance = 0;
            this.layoutPatientCallNow.TextVisible = false;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.lblUserName);
            this.layoutControl4.Controls.Add(this.lblRoomName);
            this.layoutControl4.Location = new System.Drawing.Point(3, 3);
            this.layoutControl4.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(1794, 48);
            this.layoutControl4.TabIndex = 6;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // lblUserName
            // 
            this.lblUserName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Appearance.Font = new System.Drawing.Font("Arial", 22F);
            this.lblUserName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblUserName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblUserName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblUserName.Location = new System.Drawing.Point(1143, 3);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(648, 42);
            this.lblUserName.StyleController = this.layoutControl4;
            this.lblUserName.TabIndex = 5;
            this.lblUserName.Text = "Nguyễn Xuân Cường";
            // 
            // lblRoomName
            // 
            this.lblRoomName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblRoomName.Appearance.Font = new System.Drawing.Font("Arial", 22F);
            this.lblRoomName.Appearance.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblRoomName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRoomName.Location = new System.Drawing.Point(3, 3);
            this.lblRoomName.Margin = new System.Windows.Forms.Padding(4);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(1134, 42);
            this.lblRoomName.StyleController = this.layoutControl4;
            this.lblRoomName.TabIndex = 4;
            this.lblRoomName.Text = "PHÒNG KHÁM NHI KHOA - P206";
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7,
            this.lciUsername});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(1794, 48);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.BackColor = System.Drawing.Color.Black;
            this.layoutControlItem7.AppearanceItemCaption.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.layoutControlItem7.AppearanceItemCaption.Options.UseBackColor = true;
            this.layoutControlItem7.AppearanceItemCaption.Options.UseBorderColor = true;
            this.layoutControlItem7.Control = this.lblRoomName;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(1140, 48);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // lciUsername
            // 
            this.lciUsername.AppearanceItemCaption.Font = new System.Drawing.Font("Arial", 22F);
            this.lciUsername.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.lciUsername.AppearanceItemCaption.Options.UseFont = true;
            this.lciUsername.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciUsername.Control = this.lblUserName;
            this.lciUsername.Location = new System.Drawing.Point(1140, 0);
            this.lciUsername.Name = "lciUsername";
            this.lciUsername.Size = new System.Drawing.Size(654, 48);
            this.lciUsername.Text = "Bác sỹ: ";
            this.lciUsername.TextSize = new System.Drawing.Size(0, 0);
            this.lciUsername.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.layoutControl6);
            this.layoutControl2.Location = new System.Drawing.Point(3, 57);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1794, 778);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControl6
            // 
            this.layoutControl6.Controls.Add(this.gridControlWaitingCls);
            this.layoutControl6.Location = new System.Drawing.Point(3, 3);
            this.layoutControl6.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl6.Name = "layoutControl6";
            this.layoutControl6.Root = this.layoutControlGroup5;
            this.layoutControl6.Size = new System.Drawing.Size(1788, 772);
            this.layoutControl6.TabIndex = 4;
            this.layoutControl6.Text = "layoutControl6";
            // 
            // gridControlWaitingCls
            // 
            this.gridControlWaitingCls.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gridControlWaitingCls.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControlWaitingCls.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlWaitingCls.Location = new System.Drawing.Point(0, 0);
            this.gridControlWaitingCls.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.gridControlWaitingCls.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlWaitingCls.MainView = this.gridViewWaitingCls;
            this.gridControlWaitingCls.Margin = new System.Windows.Forms.Padding(4);
            this.gridControlWaitingCls.Name = "gridControlWaitingCls";
            this.gridControlWaitingCls.Size = new System.Drawing.Size(1788, 772);
            this.gridControlWaitingCls.TabIndex = 4;
            this.gridControlWaitingCls.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWaitingCls});
            // 
            // gridViewWaitingCls
            // 
            this.gridViewWaitingCls.Appearance.Empty.BackColor = System.Drawing.Color.Azure;
            this.gridViewWaitingCls.Appearance.Empty.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Empty.BorderColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Empty.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Empty.Options.UseBorderColor = true;
            this.gridViewWaitingCls.Appearance.FocusedCell.BackColor = System.Drawing.Color.Azure;
            this.gridViewWaitingCls.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.HorzLine.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.OddRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.OddRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Preview.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Preview.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.Row.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.Row.Options.UseForeColor = true;
            this.gridViewWaitingCls.Appearance.RowSeparator.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingCls.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.TopNewRow.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.TopNewRow.Options.UseBackColor = true;
            this.gridViewWaitingCls.Appearance.VertLine.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingCls.Appearance.VertLine.Options.UseBackColor = true;
            this.gridViewWaitingCls.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewWaitingCls.ColumnPanelRowHeight = 50;
            this.gridViewWaitingCls.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTT,
            this.gridColumnLastName,
            this.gridColumnFirstName,
            this.gridColumnAge,
            this.gridColumnServiceReqStt,
            this.gridColumnInstructionTime,
            this.gridColumnServiceReqType});
            this.gridViewWaitingCls.GridControl = this.gridControlWaitingCls;
            this.gridViewWaitingCls.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingCls.Name = "gridViewWaitingCls";
            this.gridViewWaitingCls.OptionsFind.AllowFindPanel = false;
            this.gridViewWaitingCls.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewWaitingCls.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewWaitingCls.OptionsView.ShowGroupPanel = false;
            this.gridViewWaitingCls.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingCls.OptionsView.ShowIndicator = false;
            this.gridViewWaitingCls.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingCls.RowHeight = 50;
            this.gridViewWaitingCls.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingCls.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridViewWaitingCls_RowStyle);
            this.gridViewWaitingCls.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewWaitingCls_CustomUnboundColumnData);
            // 
            // gridColumnSTT
            // 
            this.gridColumnSTT.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnSTT.AppearanceCell.ForeColor = System.Drawing.Color.Red;
            this.gridColumnSTT.AppearanceCell.Options.UseFont = true;
            this.gridColumnSTT.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnSTT.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnSTT.AppearanceHeader.Options.UseFont = true;
            this.gridColumnSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnSTT.Caption = "#";
            this.gridColumnSTT.FieldName = "NUM_ORDER";
            this.gridColumnSTT.Name = "gridColumnSTT";
            this.gridColumnSTT.OptionsColumn.AllowEdit = false;
            this.gridColumnSTT.OptionsColumn.AllowFocus = false;
            this.gridColumnSTT.OptionsColumn.AllowMove = false;
            this.gridColumnSTT.OptionsColumn.AllowShowHide = false;
            this.gridColumnSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnSTT.Visible = true;
            this.gridColumnSTT.VisibleIndex = 0;
            this.gridColumnSTT.Width = 71;
            // 
            // gridColumnLastName
            // 
            this.gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnLastName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnLastName.AppearanceCell.Options.UseFont = true;
            this.gridColumnLastName.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnLastName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLastName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnLastName.AppearanceHeader.Options.UseFont = true;
            this.gridColumnLastName.Caption = "Họ";
            this.gridColumnLastName.FieldName = "TDL_PATIENT_LAST_NAME";
            this.gridColumnLastName.Name = "gridColumnLastName";
            this.gridColumnLastName.OptionsColumn.AllowEdit = false;
            this.gridColumnLastName.OptionsColumn.AllowFocus = false;
            this.gridColumnLastName.OptionsColumn.AllowMove = false;
            this.gridColumnLastName.OptionsColumn.AllowShowHide = false;
            this.gridColumnLastName.Visible = true;
            this.gridColumnLastName.VisibleIndex = 1;
            this.gridColumnLastName.Width = 289;
            // 
            // gridColumnFirstName
            // 
            this.gridColumnFirstName.AppearanceCell.Font = new System.Drawing.Font("Arial", 21.75F);
            this.gridColumnFirstName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnFirstName.AppearanceCell.Options.UseFont = true;
            this.gridColumnFirstName.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnFirstName.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnFirstName.AppearanceHeader.Options.UseFont = true;
            this.gridColumnFirstName.Caption = "Tên";
            this.gridColumnFirstName.FieldName = "TDL_PATIENT_FIRST_NAME";
            this.gridColumnFirstName.Name = "gridColumnFirstName";
            this.gridColumnFirstName.OptionsColumn.AllowEdit = false;
            this.gridColumnFirstName.OptionsColumn.AllowFocus = false;
            this.gridColumnFirstName.OptionsColumn.AllowMove = false;
            this.gridColumnFirstName.OptionsColumn.AllowShowHide = false;
            this.gridColumnFirstName.Visible = true;
            this.gridColumnFirstName.VisibleIndex = 2;
            this.gridColumnFirstName.Width = 162;
            // 
            // gridColumnAge
            // 
            this.gridColumnAge.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnAge.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnAge.AppearanceCell.Options.UseFont = true;
            this.gridColumnAge.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnAge.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnAge.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAge.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnAge.AppearanceHeader.Options.UseFont = true;
            this.gridColumnAge.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnAge.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnAge.Caption = "Tuổi";
            this.gridColumnAge.FieldName = "AGE_DISPLAY";
            this.gridColumnAge.Name = "gridColumnAge";
            this.gridColumnAge.OptionsColumn.AllowEdit = false;
            this.gridColumnAge.OptionsColumn.AllowFocus = false;
            this.gridColumnAge.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnAge.Visible = true;
            this.gridColumnAge.VisibleIndex = 3;
            this.gridColumnAge.Width = 86;
            // 
            // gridColumnServiceReqStt
            // 
            this.gridColumnServiceReqStt.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnServiceReqStt.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnServiceReqStt.AppearanceCell.Options.UseFont = true;
            this.gridColumnServiceReqStt.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnServiceReqStt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnServiceReqStt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnServiceReqStt.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnServiceReqStt.AppearanceHeader.Options.UseFont = true;
            this.gridColumnServiceReqStt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnServiceReqStt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnServiceReqStt.Caption = "Trạng thái";
            this.gridColumnServiceReqStt.FieldName = "SERVICE_REQ_STT_NAME";
            this.gridColumnServiceReqStt.Name = "gridColumnServiceReqStt";
            this.gridColumnServiceReqStt.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceReqStt.OptionsColumn.AllowFocus = false;
            this.gridColumnServiceReqStt.Width = 180;
            // 
            // gridColumnInstructionTime
            // 
            this.gridColumnInstructionTime.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnInstructionTime.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnInstructionTime.AppearanceCell.Options.UseFont = true;
            this.gridColumnInstructionTime.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnInstructionTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnInstructionTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnInstructionTime.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnInstructionTime.AppearanceHeader.Options.UseFont = true;
            this.gridColumnInstructionTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnInstructionTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnInstructionTime.Caption = "Thời gian chỉ định";
            this.gridColumnInstructionTime.FieldName = "INSTRUCTION_TIME_STR";
            this.gridColumnInstructionTime.Name = "gridColumnInstructionTime";
            this.gridColumnInstructionTime.OptionsColumn.AllowEdit = false;
            this.gridColumnInstructionTime.OptionsColumn.AllowFocus = false;
            this.gridColumnInstructionTime.Width = 270;
            // 
            // gridColumnServiceReqType
            // 
            this.gridColumnServiceReqType.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 22F);
            this.gridColumnServiceReqType.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnServiceReqType.AppearanceCell.Options.UseFont = true;
            this.gridColumnServiceReqType.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnServiceReqType.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnServiceReqType.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnServiceReqType.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.gridColumnServiceReqType.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnServiceReqType.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
            this.gridColumnServiceReqType.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumnServiceReqType.AppearanceHeader.Options.UseFont = true;
            this.gridColumnServiceReqType.AppearanceHeader.Options.UseForeColor = true;
            this.gridColumnServiceReqType.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumnServiceReqType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnServiceReqType.Caption = "Loại";
            this.gridColumnServiceReqType.FieldName = "SERVICE_REQ_TYPE_NAME";
            this.gridColumnServiceReqType.Name = "gridColumnServiceReqType";
            this.gridColumnServiceReqType.OptionsColumn.AllowEdit = false;
            this.gridColumnServiceReqType.OptionsColumn.AllowFocus = false;
            this.gridColumnServiceReqType.OptionsColumn.AllowShowHide = false;
            this.gridColumnServiceReqType.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnServiceReqType.Width = 270;
            // 
            // layoutControlGroup5
            // 
            this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup5.GroupBordersVisible = false;
            this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem12});
            this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup5.Name = "layoutControlGroup5";
            this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup5.Size = new System.Drawing.Size(1788, 772);
            this.layoutControlGroup5.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.gridControlWaitingCls;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem12.Size = new System.Drawing.Size(1788, 772);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
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
            this.Root.Size = new System.Drawing.Size(1794, 778);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.layoutControl6;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1794, 778);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1800, 898);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 54);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1800, 784);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.layoutControl4;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1800, 54);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.layoutControl5;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 838);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1800, 60);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // timerForHightLightCallPatientLayout
            // 
            this.timerForHightLightCallPatientLayout.Interval = 1000;
            this.timerForHightLightCallPatientLayout.Tick += new System.EventHandler(this.timerForHightLightCallPatientLayout_Tick);
            // 
            // timerForScrollTextBottom
            // 
            this.timerForScrollTextBottom.Interval = 500;
            this.timerForScrollTextBottom.Tick += new System.EventHandler(this.timerForScrollTextBottom_Tick);
            // 
            // timerForScrollListPatient
            // 
            this.timerForScrollListPatient.Interval = 5000;
            this.timerForScrollListPatient.Tick += new System.EventHandler(this.timerForScrollListPatient_Tick);
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
            this.timerSetDataToGridControl.Interval = 10000;
            this.timerSetDataToGridControl.Tick += new System.EventHandler(this.timerSetDataToGridControl_Tick);
            // 
            // timerAutoRefactorName
            // 
            this.timerAutoRefactorName.Interval = 500;
            this.timerAutoRefactorName.Tick += new System.EventHandler(this.timerAutoRefactorName_Tick);
            // 
            // frmWaitingScreen_QY2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1800, 898);
            this.Controls.Add(this.layoutControl1);
            this.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmWaitingScreen_QY2";
            this.Text = "Màn hình gọi bệnh nhân";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmWaitingScreen_QY2_FormClosing);
            this.Load += new System.EventHandler(this.frmWaitingScreen_QY_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientCallNow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciUsername)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl6)).EndInit();
            this.layoutControl6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingCls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingCls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        internal DevExpress.XtraEditors.LabelControl lblSrollText;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup4;
        private DevExpress.XtraLayout.LayoutControlItem layoutPatientCallNow;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraEditors.LabelControl lblUserName;
        private DevExpress.XtraEditors.LabelControl lblRoomName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem lciUsername;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl6;
        internal DevExpress.XtraGrid.GridControl gridControlWaitingCls;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWaitingCls;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLastName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFirstName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAge;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceReqStt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnInstructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceReqType;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        internal System.Windows.Forms.Timer timerForHightLightCallPatientLayout;
        internal System.Windows.Forms.Timer timerForScrollTextBottom;
        private System.Windows.Forms.Timer timerForScrollListPatient;
        private System.Windows.Forms.Timer timerAutoLoadDataPatient;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Timer timerSetDataToGridControl;
        private System.Windows.Forms.Timer timerAutoRefactorName;
    }
}
