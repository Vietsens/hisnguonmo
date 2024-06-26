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
namespace EMR.Desktop.Plugins.EmrDocumentListAll
{
    partial class FrmEmrDocumentListAll
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
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.navBarGroupStatus = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
            this.cboStatus = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtKeyWord = new DevExpress.XtraEditors.TextEdit();
            this.chkNotAutoCompleteZero = new DevExpress.XtraEditors.CheckEdit();
            this.txtPatientCode = new DevExpress.XtraEditors.TextEdit();
            this.txtTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciKeyWord = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.GcSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcDispenseCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcDocumentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcTreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcPatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcDob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcGenderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcRejecter = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcRejectReason = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcNextSigner = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcDocumentTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcCreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcCreator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcModifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcURL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GcDispenseTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupCreateDate = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupDocumentType = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer4 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.dtCreateTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lciDateFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.dtCreateTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.lciDateTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer5 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl8 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.cboDocumentType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.gridView4 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).BeginInit();
            this.layoutControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAutoCompleteZero.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.navBarGroupControlContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).BeginInit();
            this.layoutControl7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateTo)).BeginInit();
            this.navBarGroupControlContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).BeginInit();
            this.layoutControl8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1147, 556);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.ucPaging1);
            this.layoutControl2.Controls.Add(this.layoutControl3);
            this.layoutControl2.Controls.Add(this.gridControl1);
            this.layoutControl2.Location = new System.Drawing.Point(2, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1143, 552);
            this.layoutControl2.TabIndex = 4;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(201, 529);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(940, 21);
            this.ucPaging1.TabIndex = 6;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.btnRefresh);
            this.layoutControl3.Controls.Add(this.btnSearch);
            this.layoutControl3.Controls.Add(this.navBarControl1);
            this.layoutControl3.Controls.Add(this.cboDepartment);
            this.layoutControl3.Controls.Add(this.txtKeyWord);
            this.layoutControl3.Controls.Add(this.chkNotAutoCompleteZero);
            this.layoutControl3.Controls.Add(this.txtPatientCode);
            this.layoutControl3.Controls.Add(this.txtTreatmentCode);
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup2;
            this.layoutControl3.Size = new System.Drawing.Size(199, 552);
            this.layoutControl3.TabIndex = 5;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(102, 528);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(95, 22);
            this.btnRefresh.StyleController = this.layoutControl3;
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Text = "Làm lại (Ctrl R)";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(2, 528);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(96, 22);
            this.btnSearch.StyleController = this.layoutControl3;
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Tìm kiếm (Ctrl F)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.navBarGroupStatus;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer4);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer5);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.navBarGroupStatus,
            this.navBarGroupCreateDate,
            this.navBarGroupDocumentType});
            this.navBarControl1.Location = new System.Drawing.Point(2, 74);
            this.navBarControl1.Margin = new System.Windows.Forms.Padding(0);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 195;
            this.navBarControl1.Size = new System.Drawing.Size(195, 450);
            this.navBarControl1.TabIndex = 9;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // navBarGroupStatus
            // 
            this.navBarGroupStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navBarGroupStatus.Appearance.Options.UseFont = true;
            this.navBarGroupStatus.AppearanceBackground.BackColor = System.Drawing.Color.Silver;
            this.navBarGroupStatus.AppearanceBackground.Options.UseBackColor = true;
            this.navBarGroupStatus.Caption = "Trạng thái";
            this.navBarGroupStatus.ControlContainer = this.navBarGroupControlContainer1;
            this.navBarGroupStatus.Expanded = true;
            this.navBarGroupStatus.GroupClientHeight = 40;
            this.navBarGroupStatus.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupStatus.Name = "navBarGroupStatus";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.layoutControl4);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(187, 36);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // layoutControl4
            // 
            this.layoutControl4.Controls.Add(this.cboStatus);
            this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl4.Location = new System.Drawing.Point(0, 0);
            this.layoutControl4.Name = "layoutControl4";
            this.layoutControl4.Root = this.layoutControlGroup3;
            this.layoutControl4.Size = new System.Drawing.Size(187, 36);
            this.layoutControl4.TabIndex = 0;
            this.layoutControl4.Text = "layoutControl4";
            // 
            // cboStatus
            // 
            this.cboStatus.Location = new System.Drawing.Point(2, 2);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboStatus.Properties.AutoComplete = false;
            this.cboStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboStatus.Properties.NullText = "";
            this.cboStatus.Properties.PopupSizeable = false;
            this.cboStatus.Properties.View = this.gridView2;
            this.cboStatus.Size = new System.Drawing.Size(183, 20);
            this.cboStatus.StyleController = this.layoutControl4;
            this.cboStatus.TabIndex = 4;
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciStatus});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup3.Size = new System.Drawing.Size(187, 36);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // lciStatus
            // 
            this.lciStatus.Control = this.cboStatus;
            this.lciStatus.Location = new System.Drawing.Point(0, 0);
            this.lciStatus.Name = "lciStatus";
            this.lciStatus.Size = new System.Drawing.Size(187, 36);
            this.lciStatus.TextSize = new System.Drawing.Size(0, 0);
            this.lciStatus.TextVisible = false;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(33, 50);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridLookUpEdit1View;
            this.cboDepartment.Size = new System.Drawing.Size(164, 20);
            this.cboDepartment.StyleController = this.layoutControl3;
            this.cboDepartment.TabIndex = 8;
            this.cboDepartment.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboDepartment_CustomDisplayText);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtKeyWord
            // 
            this.txtKeyWord.Location = new System.Drawing.Point(2, 26);
            this.txtKeyWord.Name = "txtKeyWord";
            this.txtKeyWord.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyWord.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyWord.Size = new System.Drawing.Size(195, 20);
            this.txtKeyWord.StyleController = this.layoutControl3;
            this.txtKeyWord.TabIndex = 7;
            this.txtKeyWord.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyWord_PreviewKeyDown);
            // 
            // chkNotAutoCompleteZero
            // 
            this.chkNotAutoCompleteZero.Location = new System.Drawing.Point(178, 2);
            this.chkNotAutoCompleteZero.Name = "chkNotAutoCompleteZero";
            this.chkNotAutoCompleteZero.Properties.Caption = "";
            this.chkNotAutoCompleteZero.Size = new System.Drawing.Size(19, 19);
            this.chkNotAutoCompleteZero.StyleController = this.layoutControl3;
            this.chkNotAutoCompleteZero.TabIndex = 6;
            this.chkNotAutoCompleteZero.ToolTip = "Không tự bổ sung ký tự 0";
            this.chkNotAutoCompleteZero.CheckedChanged += new System.EventHandler(this.chkNotAutoCompleteZero_CheckedChanged);
            // 
            // txtPatientCode
            // 
            this.txtPatientCode.Location = new System.Drawing.Point(91, 2);
            this.txtPatientCode.Name = "txtPatientCode";
            this.txtPatientCode.Properties.NullValuePrompt = "Mã bệnh nhân";
            this.txtPatientCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtPatientCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtPatientCode.Size = new System.Drawing.Size(83, 20);
            this.txtPatientCode.StyleController = this.layoutControl3;
            this.txtPatientCode.TabIndex = 5;
            this.txtPatientCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPatientCode_KeyPress);
            this.txtPatientCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPatientCode_PreviewKeyDown);
            // 
            // txtTreatmentCode
            // 
            this.txtTreatmentCode.Location = new System.Drawing.Point(2, 2);
            this.txtTreatmentCode.Name = "txtTreatmentCode";
            this.txtTreatmentCode.Properties.NullValuePrompt = "Mã điều trị";
            this.txtTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTreatmentCode.Size = new System.Drawing.Size(85, 20);
            this.txtTreatmentCode.StyleController = this.layoutControl3;
            this.txtTreatmentCode.TabIndex = 4;
            this.txtTreatmentCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTreatmentCode_KeyPress);
            this.txtTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtTreatmentCode_PreviewKeyDown);
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciTreatmentCode,
            this.lciPatientCode,
            this.layoutControlItem6,
            this.lciKeyWord,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem10,
            this.layoutControlItem11});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Size = new System.Drawing.Size(199, 552);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.Control = this.txtTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(0, 0);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(89, 24);
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciTreatmentCode.TextVisible = false;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.Control = this.txtPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(89, 0);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(87, 24);
            this.lciPatientCode.TextSize = new System.Drawing.Size(0, 0);
            this.lciPatientCode.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.chkNotAutoCompleteZero;
            this.layoutControlItem6.Location = new System.Drawing.Point(176, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(23, 24);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciKeyWord
            // 
            this.lciKeyWord.Control = this.txtKeyWord;
            this.lciKeyWord.Location = new System.Drawing.Point(0, 24);
            this.lciKeyWord.Name = "lciKeyWord";
            this.lciKeyWord.Size = new System.Drawing.Size(199, 24);
            this.lciKeyWord.TextSize = new System.Drawing.Size(0, 0);
            this.lciKeyWord.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.cboDepartment;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(199, 24);
            this.layoutControlItem4.Text = "Khoa:";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(28, 13);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.navBarControl1;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(199, 454);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.btnSearch;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 526);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(100, 26);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.btnRefresh;
            this.layoutControlItem11.Location = new System.Drawing.Point(100, 526);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(99, 26);
            this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem11.TextVisible = false;
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(199, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(944, 527);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.GcSTT,
            this.GcDispenseCode,
            this.GcDocumentName,
            this.GcTreatmentCode,
            this.GcPatientCode,
            this.GcPatientName,
            this.GcDob,
            this.GcGenderName,
            this.GcRejecter,
            this.GcRejectReason,
            this.GcNextSigner,
            this.GcDocumentTypeName,
            this.GcCreateTime,
            this.GcCreator,
            this.GcModifyTime,
            this.GcModifier,
            this.GcURL,
            this.GcDispenseTime});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
            this.gridView1.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridView1_CustomRowCellEdit);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView_CustomUnboundColumnData);
            // 
            // GcSTT
            // 
            this.GcSTT.AppearanceCell.Options.UseTextOptions = true;
            this.GcSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.GcSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.GcSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.GcSTT.Caption = "STT";
            this.GcSTT.FieldName = "STT";
            this.GcSTT.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.GcSTT.Name = "GcSTT";
            this.GcSTT.OptionsColumn.AllowEdit = false;
            this.GcSTT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcSTT.Visible = true;
            this.GcSTT.VisibleIndex = 0;
            this.GcSTT.Width = 55;
            // 
            // GcDispenseCode
            // 
            this.GcDispenseCode.Caption = "Mã văn bản";
            this.GcDispenseCode.FieldName = "DOCUMENT_CODE";
            this.GcDispenseCode.Name = "GcDispenseCode";
            this.GcDispenseCode.OptionsColumn.ReadOnly = true;
            this.GcDispenseCode.Visible = true;
            this.GcDispenseCode.VisibleIndex = 1;
            this.GcDispenseCode.Width = 100;
            // 
            // GcDocumentName
            // 
            this.GcDocumentName.Caption = "Tên văn bản";
            this.GcDocumentName.FieldName = "DOCUMENT_NAME";
            this.GcDocumentName.Name = "GcDocumentName";
            this.GcDocumentName.OptionsColumn.AllowEdit = false;
            this.GcDocumentName.Visible = true;
            this.GcDocumentName.VisibleIndex = 2;
            this.GcDocumentName.Width = 100;
            // 
            // GcTreatmentCode
            // 
            this.GcTreatmentCode.Caption = "Mã điều trị";
            this.GcTreatmentCode.FieldName = "TREATMENT_CODE";
            this.GcTreatmentCode.Name = "GcTreatmentCode";
            this.GcTreatmentCode.OptionsColumn.ReadOnly = true;
            this.GcTreatmentCode.Visible = true;
            this.GcTreatmentCode.VisibleIndex = 3;
            this.GcTreatmentCode.Width = 90;
            // 
            // GcPatientCode
            // 
            this.GcPatientCode.Caption = "Mã bệnh nhân";
            this.GcPatientCode.FieldName = "PATIENT_CODE";
            this.GcPatientCode.Name = "GcPatientCode";
            this.GcPatientCode.OptionsColumn.ReadOnly = true;
            this.GcPatientCode.Visible = true;
            this.GcPatientCode.VisibleIndex = 4;
            this.GcPatientCode.Width = 90;
            // 
            // GcPatientName
            // 
            this.GcPatientName.Caption = "Họ tên";
            this.GcPatientName.FieldName = "VIR_PATIENT_NAME";
            this.GcPatientName.Name = "GcPatientName";
            this.GcPatientName.OptionsColumn.AllowEdit = false;
            this.GcPatientName.Visible = true;
            this.GcPatientName.VisibleIndex = 5;
            this.GcPatientName.Width = 120;
            // 
            // GcDob
            // 
            this.GcDob.Caption = "Ngày sinh";
            this.GcDob.FieldName = "DOB_STR";
            this.GcDob.Name = "GcDob";
            this.GcDob.OptionsColumn.AllowEdit = false;
            this.GcDob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcDob.Visible = true;
            this.GcDob.VisibleIndex = 6;
            this.GcDob.Width = 120;
            // 
            // GcGenderName
            // 
            this.GcGenderName.Caption = "Giới tính";
            this.GcGenderName.FieldName = "GENDER_NAME";
            this.GcGenderName.Name = "GcGenderName";
            this.GcGenderName.OptionsColumn.AllowEdit = false;
            this.GcGenderName.Visible = true;
            this.GcGenderName.VisibleIndex = 7;
            this.GcGenderName.Width = 80;
            // 
            // GcRejecter
            // 
            this.GcRejecter.Caption = "Người từ chối";
            this.GcRejecter.FieldName = "REJECTER";
            this.GcRejecter.Name = "GcRejecter";
            this.GcRejecter.OptionsColumn.AllowEdit = false;
            this.GcRejecter.Visible = true;
            this.GcRejecter.VisibleIndex = 8;
            this.GcRejecter.Width = 92;
            // 
            // GcRejectReason
            // 
            this.GcRejectReason.Caption = "Lý do từ chối";
            this.GcRejectReason.FieldName = "REJECT_REASON";
            this.GcRejectReason.Name = "GcRejectReason";
            this.GcRejectReason.OptionsColumn.AllowEdit = false;
            this.GcRejectReason.Visible = true;
            this.GcRejectReason.VisibleIndex = 9;
            this.GcRejectReason.Width = 93;
            // 
            // GcNextSigner
            // 
            this.GcNextSigner.Caption = "Người ký tiếp theo";
            this.GcNextSigner.FieldName = "NEXT_SIGNER_STR";
            this.GcNextSigner.Name = "GcNextSigner";
            this.GcNextSigner.OptionsColumn.AllowEdit = false;
            this.GcNextSigner.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcNextSigner.Visible = true;
            this.GcNextSigner.VisibleIndex = 10;
            this.GcNextSigner.Width = 120;
            // 
            // GcDocumentTypeName
            // 
            this.GcDocumentTypeName.Caption = "Loại văn bản ";
            this.GcDocumentTypeName.FieldName = "DOCUMENT_TYPE_NAME";
            this.GcDocumentTypeName.Name = "GcDocumentTypeName";
            this.GcDocumentTypeName.OptionsColumn.AllowEdit = false;
            this.GcDocumentTypeName.Visible = true;
            this.GcDocumentTypeName.VisibleIndex = 11;
            this.GcDocumentTypeName.Width = 120;
            // 
            // GcCreateTime
            // 
            this.GcCreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.GcCreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.GcCreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.GcCreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.GcCreateTime.Caption = "Ngày tạo";
            this.GcCreateTime.FieldName = "CREATE_TIME_DISPLAY";
            this.GcCreateTime.Name = "GcCreateTime";
            this.GcCreateTime.OptionsColumn.AllowEdit = false;
            this.GcCreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcCreateTime.Visible = true;
            this.GcCreateTime.VisibleIndex = 12;
            this.GcCreateTime.Width = 120;
            // 
            // GcCreator
            // 
            this.GcCreator.Caption = "Người tạo";
            this.GcCreator.FieldName = "CREATOR";
            this.GcCreator.Name = "GcCreator";
            this.GcCreator.OptionsColumn.AllowEdit = false;
            this.GcCreator.Visible = true;
            this.GcCreator.VisibleIndex = 13;
            this.GcCreator.Width = 110;
            // 
            // GcModifyTime
            // 
            this.GcModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.GcModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.GcModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.GcModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.GcModifyTime.Caption = "Ngày sửa";
            this.GcModifyTime.FieldName = "MODIFY_TIME_DISPLAY";
            this.GcModifyTime.Name = "GcModifyTime";
            this.GcModifyTime.OptionsColumn.AllowEdit = false;
            this.GcModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcModifyTime.Visible = true;
            this.GcModifyTime.VisibleIndex = 14;
            this.GcModifyTime.Width = 120;
            // 
            // GcModifier
            // 
            this.GcModifier.Caption = "Người sửa";
            this.GcModifier.FieldName = "MODIFIER";
            this.GcModifier.Name = "GcModifier";
            this.GcModifier.OptionsColumn.AllowEdit = false;
            this.GcModifier.Visible = true;
            this.GcModifier.VisibleIndex = 15;
            this.GcModifier.Width = 110;
            // 
            // GcURL
            // 
            this.GcURL.Caption = "URL";
            this.GcURL.FieldName = "URL";
            this.GcURL.Name = "GcURL";
            this.GcURL.OptionsColumn.ReadOnly = true;
            this.GcURL.Width = 150;
            // 
            // GcDispenseTime
            // 
            this.GcDispenseTime.Caption = "Thời gian lưu";
            this.GcDispenseTime.FieldName = "STORE_TIME_STR";
            this.GcDispenseTime.Name = "GcDispenseTime";
            this.GcDispenseTime.OptionsColumn.AllowEdit = false;
            this.GcDispenseTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GcDispenseTime.Width = 120;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem12});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1143, 552);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(199, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(944, 527);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.layoutControl3;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(199, 552);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.ucPaging1;
            this.layoutControlItem12.Location = new System.Drawing.Point(199, 527);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(944, 25);
            this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem12.TextVisible = false;
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
            this.layoutControlGroup1.Size = new System.Drawing.Size(1147, 556);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.layoutControl2;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1147, 556);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // navBarGroupCreateDate
            // 
            this.navBarGroupCreateDate.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navBarGroupCreateDate.Appearance.Options.UseFont = true;
            this.navBarGroupCreateDate.Caption = "Ngày tạo";
            this.navBarGroupCreateDate.ControlContainer = this.navBarGroupControlContainer4;
            this.navBarGroupCreateDate.Expanded = true;
            this.navBarGroupCreateDate.GroupClientHeight = 57;
            this.navBarGroupCreateDate.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupCreateDate.Name = "navBarGroupCreateDate";
            // 
            // navBarGroupDocumentType
            // 
            this.navBarGroupDocumentType.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.navBarGroupDocumentType.Appearance.Options.UseFont = true;
            this.navBarGroupDocumentType.Caption = "Loại văn bản";
            this.navBarGroupDocumentType.ControlContainer = this.navBarGroupControlContainer5;
            this.navBarGroupDocumentType.Expanded = true;
            this.navBarGroupDocumentType.GroupClientHeight = 40;
            this.navBarGroupDocumentType.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.navBarGroupDocumentType.Name = "navBarGroupDocumentType";
            // 
            // navBarGroupControlContainer4
            // 
            this.navBarGroupControlContainer4.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer4.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer4.Controls.Add(this.layoutControl7);
            this.navBarGroupControlContainer4.Name = "navBarGroupControlContainer4";
            this.navBarGroupControlContainer4.Size = new System.Drawing.Size(187, 53);
            this.navBarGroupControlContainer4.TabIndex = 1;
            // 
            // layoutControl7
            // 
            this.layoutControl7.Controls.Add(this.dtCreateTimeTo);
            this.layoutControl7.Controls.Add(this.dtCreateTimeFrom);
            this.layoutControl7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl7.Location = new System.Drawing.Point(0, 0);
            this.layoutControl7.Name = "layoutControl7";
            this.layoutControl7.Root = this.layoutControlGroup6;
            this.layoutControl7.Size = new System.Drawing.Size(187, 53);
            this.layoutControl7.TabIndex = 0;
            this.layoutControl7.Text = "layoutControl7";
            // 
            // layoutControlGroup6
            // 
            this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup6.GroupBordersVisible = false;
            this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciDateFrom,
            this.lciDateTo});
            this.layoutControlGroup6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup6.Name = "layoutControlGroup6";
            this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup6.Size = new System.Drawing.Size(187, 53);
            this.layoutControlGroup6.TextVisible = false;
            // 
            // dtCreateTimeFrom
            // 
            this.dtCreateTimeFrom.EditValue = null;
            this.dtCreateTimeFrom.Location = new System.Drawing.Point(30, 2);
            this.dtCreateTimeFrom.Name = "dtCreateTimeFrom";
            this.dtCreateTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeFrom.Size = new System.Drawing.Size(155, 20);
            this.dtCreateTimeFrom.StyleController = this.layoutControl7;
            this.dtCreateTimeFrom.TabIndex = 4;
            // 
            // lciDateFrom
            // 
            this.lciDateFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDateFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDateFrom.Control = this.dtCreateTimeFrom;
            this.lciDateFrom.Location = new System.Drawing.Point(0, 0);
            this.lciDateFrom.Name = "lciDateFrom";
            this.lciDateFrom.Size = new System.Drawing.Size(187, 24);
            this.lciDateFrom.Text = "Từ:";
            this.lciDateFrom.TextSize = new System.Drawing.Size(24, 13);
            // 
            // dtCreateTimeTo
            // 
            this.dtCreateTimeTo.EditValue = null;
            this.dtCreateTimeTo.Location = new System.Drawing.Point(30, 26);
            this.dtCreateTimeTo.Name = "dtCreateTimeTo";
            this.dtCreateTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtCreateTimeTo.Size = new System.Drawing.Size(155, 20);
            this.dtCreateTimeTo.StyleController = this.layoutControl7;
            this.dtCreateTimeTo.TabIndex = 5;
            // 
            // lciDateTo
            // 
            this.lciDateTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDateTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDateTo.Control = this.dtCreateTimeTo;
            this.lciDateTo.Location = new System.Drawing.Point(0, 24);
            this.lciDateTo.Name = "lciDateTo";
            this.lciDateTo.Size = new System.Drawing.Size(187, 29);
            this.lciDateTo.Text = "Đến:";
            this.lciDateTo.TextSize = new System.Drawing.Size(24, 13);
            // 
            // navBarGroupControlContainer5
            // 
            this.navBarGroupControlContainer5.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer5.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer5.Controls.Add(this.layoutControl8);
            this.navBarGroupControlContainer5.Name = "navBarGroupControlContainer5";
            this.navBarGroupControlContainer5.Size = new System.Drawing.Size(187, 36);
            this.navBarGroupControlContainer5.TabIndex = 2;
            // 
            // layoutControl8
            // 
            this.layoutControl8.Controls.Add(this.cboDocumentType);
            this.layoutControl8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl8.Location = new System.Drawing.Point(0, 0);
            this.layoutControl8.Name = "layoutControl8";
            this.layoutControl8.Root = this.layoutControlGroup7;
            this.layoutControl8.Size = new System.Drawing.Size(187, 36);
            this.layoutControl8.TabIndex = 0;
            this.layoutControl8.Text = "layoutControl8";
            // 
            // layoutControlGroup7
            // 
            this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup7.GroupBordersVisible = false;
            this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem13});
            this.layoutControlGroup7.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup7.Name = "layoutControlGroup7";
            this.layoutControlGroup7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup7.Size = new System.Drawing.Size(187, 36);
            this.layoutControlGroup7.TextVisible = false;
            // 
            // cboDocumentType
            // 
            this.cboDocumentType.Location = new System.Drawing.Point(2, 2);
            this.cboDocumentType.Name = "cboDocumentType";
            this.cboDocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDocumentType.Properties.NullText = "";
            this.cboDocumentType.Properties.View = this.gridView4;
            this.cboDocumentType.Size = new System.Drawing.Size(183, 20);
            this.cboDocumentType.StyleController = this.layoutControl8;
            this.cboDocumentType.TabIndex = 4;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.Control = this.cboDocumentType;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(187, 36);
            this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem13.TextVisible = false;
            // 
            // gridView4
            // 
            this.gridView4.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView4.Name = "gridView4";
            this.gridView4.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // FrmEmrDocumentListAll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "FrmEmrDocumentListAll";
            this.Size = new System.Drawing.Size(1147, 556);
            this.Load += new System.EventHandler(this.FrmEmrDocumentListAll_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl4)).EndInit();
            this.layoutControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyWord.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNotAutoCompleteZero.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPatientCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciKeyWord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.navBarGroupControlContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl7)).EndInit();
            this.layoutControl7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCreateTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDateTo)).EndInit();
            this.navBarGroupControlContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl8)).EndInit();
            this.layoutControl8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraEditors.CheckEdit chkNotAutoCompleteZero;
        private DevExpress.XtraEditors.TextEdit txtPatientCode;
        private DevExpress.XtraEditors.TextEdit txtTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem lciKeyWord;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupStatus;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl4;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraEditors.GridLookUpEdit cboStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraLayout.LayoutControlItem lciStatus;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraGrid.Columns.GridColumn GcSTT;
        private DevExpress.XtraGrid.Columns.GridColumn GcDispenseCode;
        private DevExpress.XtraGrid.Columns.GridColumn GcDocumentName;
        private DevExpress.XtraGrid.Columns.GridColumn GcTreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn GcPatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn GcPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn GcDob;
        private DevExpress.XtraGrid.Columns.GridColumn GcGenderName;
        private DevExpress.XtraGrid.Columns.GridColumn GcRejecter;
        private DevExpress.XtraGrid.Columns.GridColumn GcRejectReason;
        private DevExpress.XtraGrid.Columns.GridColumn GcNextSigner;
        private DevExpress.XtraGrid.Columns.GridColumn GcDocumentTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn GcCreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn GcCreator;
        private DevExpress.XtraGrid.Columns.GridColumn GcModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn GcModifier;
        private DevExpress.XtraGrid.Columns.GridColumn GcURL;
        private DevExpress.XtraGrid.Columns.GridColumn GcDispenseTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer4;
        private DevExpress.XtraLayout.LayoutControl layoutControl7;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup6;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupCreateDate;
        private DevExpress.XtraNavBar.NavBarGroup navBarGroupDocumentType;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem lciDateFrom;
        private DevExpress.XtraEditors.DateEdit dtCreateTimeTo;
        private DevExpress.XtraLayout.LayoutControlItem lciDateTo;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer5;
        private DevExpress.XtraLayout.LayoutControl layoutControl8;
        private DevExpress.XtraEditors.GridLookUpEdit cboDocumentType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView4;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
    }
}
