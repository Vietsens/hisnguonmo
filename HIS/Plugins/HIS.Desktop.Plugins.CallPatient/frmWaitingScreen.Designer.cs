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
namespace HIS.Desktop.Plugins.CallPatient
{
    partial class frmWaitingScreen1
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
            this.layoutControlGroupColor = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutPatientCallNow = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.lblUserName = new DevExpress.XtraEditors.LabelControl();
            this.lblRoomName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControlWaitingPatient = new DevExpress.XtraGrid.GridControl();
            this.gridViewWaitingPatient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnLastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnFirstName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnAge = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceReqStt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnInstructionTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnServiceReqType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.timerForHightLightCallPatientLayout = new System.Windows.Forms.Timer();
            this.timerForScrollTextBottom = new System.Windows.Forms.Timer();
            this.timerForScrollListPatient = new System.Windows.Forms.Timer();
            this.timerAutoLoadDataPatient = new System.Windows.Forms.Timer();
            this.imageList1 = new System.Windows.Forms.ImageList();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).BeginInit();
            this.layoutControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientCallNow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingPatient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl5);
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl5
            // 
            this.layoutControl5.Controls.Add(this.lblSrollText);
            this.layoutControl5.Location = new System.Drawing.Point(12, 630);
            this.layoutControl5.Name = "layoutControl5";
            this.layoutControl5.Root = this.layoutControlGroupColor;
            this.layoutControl5.Size = new System.Drawing.Size(1310, 50);
            this.layoutControl5.TabIndex = 7;
            this.layoutControl5.Text = "layoutControl5";
            // 
            // lblSrollText
            // 
            this.lblSrollText.Appearance.Font = new System.Drawing.Font("Tahoma", 22F);
            this.lblSrollText.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblSrollText.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSrollText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSrollText.Location = new System.Drawing.Point(4, 4);
            this.lblSrollText.Name = "lblSrollText";
            this.lblSrollText.Size = new System.Drawing.Size(1302, 35);
            this.lblSrollText.StyleController = this.layoutControl5;
            this.lblSrollText.TabIndex = 4;
            // 
            // layoutControlGroupColor
            // 
            this.layoutControlGroupColor.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 22F);
            this.layoutControlGroupColor.AppearanceGroup.ForeColor = System.Drawing.Color.Red;
            this.layoutControlGroupColor.AppearanceGroup.Options.UseFont = true;
            this.layoutControlGroupColor.AppearanceGroup.Options.UseForeColor = true;
            this.layoutControlGroupColor.AppearanceGroup.Options.UseTextOptions = true;
            this.layoutControlGroupColor.AppearanceGroup.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlGroupColor.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroupColor.GroupBordersVisible = false;
            this.layoutControlGroupColor.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutPatientCallNow});
            this.layoutControlGroupColor.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupColor.Name = "layoutControlGroupColor";
            this.layoutControlGroupColor.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroupColor.Size = new System.Drawing.Size(1310, 50);
            this.layoutControlGroupColor.TextVisible = false;
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
            this.layoutPatientCallNow.Size = new System.Drawing.Size(1306, 46);
            this.layoutPatientCallNow.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPatientCallNow.TextSize = new System.Drawing.Size(0, 0);
            this.layoutPatientCallNow.TextToControlDistance = 0;
            this.layoutPatientCallNow.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.layoutControl3);
            this.layoutControl2.Controls.Add(this.gridControlWaitingPatient);
            this.layoutControl2.Location = new System.Drawing.Point(12, 12);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1310, 614);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.lblUserName);
            this.layoutControl3.Controls.Add(this.lblRoomName);
            this.layoutControl3.Location = new System.Drawing.Point(2, 2);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(1306, 41);
            this.layoutControl3.TabIndex = 5;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // lblUserName
            // 
            this.lblUserName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Appearance.Font = new System.Drawing.Font("Arial", 22F);
            this.lblUserName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblUserName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblUserName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblUserName.Location = new System.Drawing.Point(730, 2);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(574, 35);
            this.lblUserName.StyleController = this.layoutControl3;
            this.lblUserName.TabIndex = 5;
            this.lblUserName.Text = "Nguyễn Xuân Cường";
            // 
            // lblRoomName
            // 
            this.lblRoomName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblRoomName.Appearance.Font = new System.Drawing.Font("Arial", 22F);
            this.lblRoomName.Appearance.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblRoomName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRoomName.Location = new System.Drawing.Point(2, 2);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Padding = new System.Windows.Forms.Padding(2);
            this.lblRoomName.Size = new System.Drawing.Size(724, 37);
            this.lblRoomName.StyleController = this.layoutControl3;
            this.lblRoomName.TabIndex = 4;
            this.lblRoomName.Text = "PHÒNG KHÁM NHI KHOA - P206";
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem10,
            this.layoutControlItem6});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(1306, 41);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.BackColor = System.Drawing.Color.Black;
            this.layoutControlItem10.AppearanceItemCaption.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.layoutControlItem10.AppearanceItemCaption.Options.UseBackColor = true;
            this.layoutControlItem10.AppearanceItemCaption.Options.UseBorderColor = true;
            this.layoutControlItem10.Control = this.lblRoomName;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(728, 41);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.lblUserName;
            this.layoutControlItem6.Location = new System.Drawing.Point(728, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(578, 41);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // gridControlWaitingPatient
            // 
            this.gridControlWaitingPatient.EmbeddedNavigator.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gridControlWaitingPatient.EmbeddedNavigator.Appearance.Options.UseForeColor = true;
            this.gridControlWaitingPatient.Location = new System.Drawing.Point(2, 47);
            this.gridControlWaitingPatient.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
            this.gridControlWaitingPatient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlWaitingPatient.MainView = this.gridViewWaitingPatient;
            this.gridControlWaitingPatient.Name = "gridControlWaitingPatient";
            this.gridControlWaitingPatient.Size = new System.Drawing.Size(1306, 565);
            this.gridControlWaitingPatient.TabIndex = 4;
            this.gridControlWaitingPatient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewWaitingPatient});
            // 
            // gridViewWaitingPatient
            // 
            this.gridViewWaitingPatient.Appearance.Empty.BackColor = System.Drawing.Color.Azure;
            this.gridViewWaitingPatient.Appearance.Empty.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.Empty.BorderColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.Empty.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.Empty.Options.UseBorderColor = true;
            this.gridViewWaitingPatient.Appearance.FocusedCell.BackColor = System.Drawing.Color.Azure;
            this.gridViewWaitingPatient.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.HorzLine.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.HorzLine.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.OddRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.OddRow.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.Preview.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.Preview.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridViewWaitingPatient.Appearance.Row.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.Row.Options.UseForeColor = true;
            this.gridViewWaitingPatient.Appearance.RowSeparator.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingPatient.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.SelectedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Transparent;
            this.gridViewWaitingPatient.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.TopNewRow.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingPatient.Appearance.TopNewRow.Options.UseBackColor = true;
            this.gridViewWaitingPatient.Appearance.VertLine.BackColor = System.Drawing.Color.Black;
            this.gridViewWaitingPatient.Appearance.VertLine.Options.UseBackColor = true;
            this.gridViewWaitingPatient.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.gridViewWaitingPatient.ColumnPanelRowHeight = 50;
            this.gridViewWaitingPatient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTT,
            this.gridColumnLastName,
            this.gridColumnFirstName,
            this.gridColumnAge,
            this.gridColumnServiceReqStt,
            this.gridColumnInstructionTime,
            this.gridColumnServiceReqType});
            this.gridViewWaitingPatient.GridControl = this.gridControlWaitingPatient;
            this.gridViewWaitingPatient.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingPatient.Name = "gridViewWaitingPatient";
            this.gridViewWaitingPatient.OptionsFind.AllowFindPanel = false;
            this.gridViewWaitingPatient.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewWaitingPatient.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.gridViewWaitingPatient.OptionsView.ColumnAutoWidth = false;
            this.gridViewWaitingPatient.OptionsView.ShowGroupPanel = false;
            this.gridViewWaitingPatient.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingPatient.OptionsView.ShowIndicator = false;
            this.gridViewWaitingPatient.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
            this.gridViewWaitingPatient.RowHeight = 50;
            this.gridViewWaitingPatient.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridViewWaitingPatient.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewWaitingPatient_CustomUnboundColumnData);
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
            this.gridColumnSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnSTT.Visible = true;
            this.gridColumnSTT.VisibleIndex = 0;
            this.gridColumnSTT.Width = 80;
            // 
            // gridColumnLastName
            // 
            this.gridColumnLastName.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnLastName.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.gridColumnLastName.AppearanceCell.Options.UseFont = true;
            this.gridColumnLastName.AppearanceCell.Options.UseForeColor = true;
            this.gridColumnLastName.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnLastName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumnLastName.AppearanceHeader.Font = new System.Drawing.Font("Arial", 22F);
            this.gridColumnLastName.AppearanceHeader.Options.UseFont = true;
            this.gridColumnLastName.Caption = "Họ";
            this.gridColumnLastName.FieldName = "TDL_PATIENT_LAST_NAME";
            this.gridColumnLastName.Name = "gridColumnLastName";
            this.gridColumnLastName.OptionsColumn.AllowEdit = false;
            this.gridColumnLastName.OptionsColumn.AllowFocus = false;
            this.gridColumnLastName.Visible = true;
            this.gridColumnLastName.VisibleIndex = 1;
            this.gridColumnLastName.Width = 320;
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
            this.gridColumnFirstName.Visible = true;
            this.gridColumnFirstName.VisibleIndex = 2;
            this.gridColumnFirstName.Width = 150;
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
            this.gridColumnAge.Width = 80;
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
            this.gridColumnServiceReqStt.Visible = true;
            this.gridColumnServiceReqStt.VisibleIndex = 4;
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
            this.gridColumnInstructionTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnInstructionTime.Visible = true;
            this.gridColumnInstructionTime.VisibleIndex = 5;
            this.gridColumnInstructionTime.Width = 270;
            // 
            // gridColumnServiceReqType
            // 
            this.gridColumnServiceReqType.AppearanceCell.Font = new System.Drawing.Font("Arial", 22F);
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
            this.gridColumnServiceReqType.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumnServiceReqType.Visible = true;
            this.gridColumnServiceReqType.VisibleIndex = 6;
            this.gridColumnServiceReqType.Width = 260;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.layoutControlItem2});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1310, 614);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.gridControlWaitingPatient;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 45);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(1310, 569);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.layoutControl3;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1310, 45);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1334, 692);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1314, 618);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.layoutControl5;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 618);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1314, 54);
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
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // frmWaitingScreen1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1334, 692);
            this.Controls.Add(this.layoutControl1);
            this.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.Name = "frmWaitingScreen1";
            this.Text = "Màn hình gọi bệnh nhân";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmWaitingScreen_Load);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl5)).EndInit();
            this.layoutControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientCallNow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlWaitingPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewWaitingPatient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl5;
        internal DevExpress.XtraEditors.LabelControl lblSrollText;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupColor;
        private DevExpress.XtraLayout.LayoutControlItem layoutPatientCallNow;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.LabelControl lblUserName;
        private DevExpress.XtraEditors.LabelControl lblRoomName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        internal DevExpress.XtraGrid.GridControl gridControlWaitingPatient;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewWaitingPatient;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTT;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnLastName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnFirstName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnAge;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceReqStt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnInstructionTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnServiceReqType;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        internal System.Windows.Forms.Timer timerForHightLightCallPatientLayout;
        internal System.Windows.Forms.Timer timerForScrollTextBottom;
        private System.Windows.Forms.Timer timerForScrollListPatient;
        private System.Windows.Forms.Timer timerAutoLoadDataPatient;
        private System.Windows.Forms.ImageList imageList1;
    }
}
