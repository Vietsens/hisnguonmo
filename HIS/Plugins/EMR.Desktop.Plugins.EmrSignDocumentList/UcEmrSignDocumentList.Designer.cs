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
namespace EMR.Desktop.Plugins.EmrSignDocumentList
{
    partial class UcEmrSignDocumentList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UcEmrSignDocumentList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject9 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject10 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject11 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject12 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnViewAndSign = new DevExpress.XtraEditors.SimpleButton();
            this.btnErrorSignDetail = new DevExpress.XtraEditors.SimpleButton();
            this.lblDocProcessErrorCount = new DevExpress.XtraEditors.LabelControl();
            this.lblDocProcessedCount = new DevExpress.XtraEditors.LabelControl();
            this.btnSigns = new DevExpress.XtraEditors.SimpleButton();
            this.ucPaging = new Inventec.UC.Paging.UcPaging();
            this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
            this.NavBarCreateTime = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.DtTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.DtTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.LciTimeFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.LciTimeTo = new DevExpress.XtraLayout.LayoutControlItem();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
            this.cboEmrDocumentType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboDepartment = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.rdoStatus__TuChoiKy = new DevExpress.XtraEditors.CheckEdit();
            this.rdoStatus__DaKy = new DevExpress.XtraEditors.CheckEdit();
            this.rdoStatus__ChoKy = new DevExpress.XtraEditors.CheckEdit();
            this.chkDelete = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
            this.NavBarStatus = new DevExpress.XtraNavBar.NavBarGroup();
            this.BtnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.BtnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlDocument = new DevExpress.XtraGrid.GridControl();
            this.gridViewDocument = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.STT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_View = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnView = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.Gc_Sign = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_DocumentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_DocumentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_PatientCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_PatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Dob = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Gender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Department = new DevExpress.XtraGrid.Columns.GridColumn();
            this.GC_StoreTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Url = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Creator = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_ModifyTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Gc_Modifier = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnSign = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repositoryItemBtnSignDisable = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.TxtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciForbtnErrorSignDetail = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Gc_RejectReason = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
            this.navBarControl1.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTimeFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTimeTo)).BeginInit();
            this.navBarGroupControlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
            this.layoutControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmrDocumentType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__TuChoiKy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__DaKy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__ChoKy.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDelete.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocument)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnSign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnSignDisable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForbtnErrorSignDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnViewAndSign);
            this.layoutControl1.Controls.Add(this.btnErrorSignDetail);
            this.layoutControl1.Controls.Add(this.lblDocProcessErrorCount);
            this.layoutControl1.Controls.Add(this.lblDocProcessedCount);
            this.layoutControl1.Controls.Add(this.btnSigns);
            this.layoutControl1.Controls.Add(this.ucPaging);
            this.layoutControl1.Controls.Add(this.navBarControl1);
            this.layoutControl1.Controls.Add(this.BtnRefresh);
            this.layoutControl1.Controls.Add(this.BtnSearch);
            this.layoutControl1.Controls.Add(this.gridControlDocument);
            this.layoutControl1.Controls.Add(this.TxtKeyword);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnViewAndSign
            // 
            this.btnViewAndSign.Enabled = false;
            this.btnViewAndSign.Location = new System.Drawing.Point(107, 550);
            this.btnViewAndSign.Name = "btnViewAndSign";
            this.btnViewAndSign.Size = new System.Drawing.Size(117, 22);
            this.btnViewAndSign.StyleController = this.layoutControl1;
            this.btnViewAndSign.TabIndex = 16;
            this.btnViewAndSign.Text = "Xem và Ký";
            this.btnViewAndSign.ToolTip = "Xem và Ký lần lượt các văn bản được chọn";
            this.btnViewAndSign.Click += new System.EventHandler(this.btnViewAndSign_Click);
            // 
            // btnErrorSignDetail
            // 
            this.btnErrorSignDetail.Image = ((System.Drawing.Image)(resources.GetObject("btnErrorSignDetail.Image")));
            this.btnErrorSignDetail.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnErrorSignDetail.Location = new System.Drawing.Point(199, 520);
            this.btnErrorSignDetail.Margin = new System.Windows.Forms.Padding(2);
            this.btnErrorSignDetail.Name = "btnErrorSignDetail";
            this.btnErrorSignDetail.Size = new System.Drawing.Size(25, 22);
            this.btnErrorSignDetail.StyleController = this.layoutControl1;
            this.btnErrorSignDetail.TabIndex = 15;
            this.btnErrorSignDetail.Click += new System.EventHandler(this.btnErrorSignDetail_Click);
            // 
            // lblDocProcessErrorCount
            // 
            this.lblDocProcessErrorCount.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lblDocProcessErrorCount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDocProcessErrorCount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDocProcessErrorCount.Location = new System.Drawing.Point(126, 520);
            this.lblDocProcessErrorCount.Margin = new System.Windows.Forms.Padding(2);
            this.lblDocProcessErrorCount.Name = "lblDocProcessErrorCount";
            this.lblDocProcessErrorCount.Size = new System.Drawing.Size(69, 26);
            this.lblDocProcessErrorCount.StyleController = this.layoutControl1;
            this.lblDocProcessErrorCount.TabIndex = 14;
            // 
            // lblDocProcessedCount
            // 
            this.lblDocProcessedCount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDocProcessedCount.Location = new System.Drawing.Point(2, 520);
            this.lblDocProcessedCount.Margin = new System.Windows.Forms.Padding(2);
            this.lblDocProcessedCount.Name = "lblDocProcessedCount";
            this.lblDocProcessedCount.Size = new System.Drawing.Size(120, 26);
            this.lblDocProcessedCount.StyleController = this.layoutControl1;
            this.lblDocProcessedCount.TabIndex = 13;
            // 
            // btnSigns
            // 
            this.btnSigns.Location = new System.Drawing.Point(2, 550);
            this.btnSigns.Name = "btnSigns";
            this.btnSigns.Size = new System.Drawing.Size(101, 22);
            this.btnSigns.StyleController = this.layoutControl1;
            this.btnSigns.TabIndex = 10;
            this.btnSigns.Text = "Ký duyệt(Ctrl S)";
            this.btnSigns.Click += new System.EventHandler(this.btnSigns_Click);
            // 
            // ucPaging
            // 
            this.ucPaging.Location = new System.Drawing.Point(228, 578);
            this.ucPaging.Name = "ucPaging";
            this.ucPaging.Size = new System.Drawing.Size(1090, 20);
            this.ucPaging.TabIndex = 9;
            // 
            // navBarControl1
            // 
            this.navBarControl1.ActiveGroup = this.NavBarCreateTime;
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer1);
            this.navBarControl1.Controls.Add(this.navBarGroupControlContainer2);
            this.navBarControl1.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.NavBarCreateTime,
            this.NavBarStatus});
            this.navBarControl1.Location = new System.Drawing.Point(2, 26);
            this.navBarControl1.Name = "navBarControl1";
            this.navBarControl1.OptionsNavPane.ExpandedWidth = 222;
            this.navBarControl1.Size = new System.Drawing.Size(222, 490);
            this.navBarControl1.TabIndex = 8;
            this.navBarControl1.Text = "navBarControl1";
            // 
            // NavBarCreateTime
            // 
            this.NavBarCreateTime.Caption = "Ngày tạo";
            this.NavBarCreateTime.ControlContainer = this.navBarGroupControlContainer1;
            this.NavBarCreateTime.Expanded = true;
            this.NavBarCreateTime.GroupClientHeight = 56;
            this.NavBarCreateTime.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.NavBarCreateTime.Name = "NavBarCreateTime";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer1.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer1.Controls.Add(this.layoutControl2);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(214, 52);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.DtTimeTo);
            this.layoutControl2.Controls.Add(this.DtTimeFrom);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup2;
            this.layoutControl2.Size = new System.Drawing.Size(214, 52);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // DtTimeTo
            // 
            this.DtTimeTo.EditValue = null;
            this.DtTimeTo.Location = new System.Drawing.Point(97, 26);
            this.DtTimeTo.Name = "DtTimeTo";
            this.DtTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeTo.Size = new System.Drawing.Size(115, 20);
            this.DtTimeTo.StyleController = this.layoutControl2;
            this.DtTimeTo.TabIndex = 5;
            // 
            // DtTimeFrom
            // 
            this.DtTimeFrom.EditValue = null;
            this.DtTimeFrom.Location = new System.Drawing.Point(97, 2);
            this.DtTimeFrom.Name = "DtTimeFrom";
            this.DtTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.DtTimeFrom.Size = new System.Drawing.Size(115, 20);
            this.DtTimeFrom.StyleController = this.layoutControl2;
            this.DtTimeFrom.TabIndex = 4;
            // 
            // layoutControlGroup2
            // 
            this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup2.GroupBordersVisible = false;
            this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.LciTimeFrom,
            this.LciTimeTo});
            this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup2.Name = "layoutControlGroup2";
            this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup2.Size = new System.Drawing.Size(214, 52);
            this.layoutControlGroup2.TextVisible = false;
            // 
            // LciTimeFrom
            // 
            this.LciTimeFrom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciTimeFrom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciTimeFrom.Control = this.DtTimeFrom;
            this.LciTimeFrom.Location = new System.Drawing.Point(0, 0);
            this.LciTimeFrom.Name = "LciTimeFrom";
            this.LciTimeFrom.Size = new System.Drawing.Size(214, 24);
            this.LciTimeFrom.Text = "Từ:";
            this.LciTimeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciTimeFrom.TextSize = new System.Drawing.Size(90, 20);
            this.LciTimeFrom.TextToControlDistance = 5;
            // 
            // LciTimeTo
            // 
            this.LciTimeTo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.LciTimeTo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.LciTimeTo.Control = this.DtTimeTo;
            this.LciTimeTo.Location = new System.Drawing.Point(0, 24);
            this.LciTimeTo.Name = "LciTimeTo";
            this.LciTimeTo.Size = new System.Drawing.Size(214, 28);
            this.LciTimeTo.Text = "Đến:";
            this.LciTimeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.LciTimeTo.TextSize = new System.Drawing.Size(90, 20);
            this.LciTimeTo.TextToControlDistance = 5;
            // 
            // navBarGroupControlContainer2
            // 
            this.navBarGroupControlContainer2.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.navBarGroupControlContainer2.Appearance.Options.UseBackColor = true;
            this.navBarGroupControlContainer2.Controls.Add(this.layoutControl3);
            this.navBarGroupControlContainer2.Name = "navBarGroupControlContainer2";
            this.navBarGroupControlContainer2.Size = new System.Drawing.Size(214, 196);
            this.navBarGroupControlContainer2.TabIndex = 1;
            // 
            // layoutControl3
            // 
            this.layoutControl3.Controls.Add(this.cboEmrDocumentType);
            this.layoutControl3.Controls.Add(this.cboDepartment);
            this.layoutControl3.Controls.Add(this.rdoStatus__TuChoiKy);
            this.layoutControl3.Controls.Add(this.rdoStatus__DaKy);
            this.layoutControl3.Controls.Add(this.rdoStatus__ChoKy);
            this.layoutControl3.Controls.Add(this.chkDelete);
            this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl3.Location = new System.Drawing.Point(0, 0);
            this.layoutControl3.Name = "layoutControl3";
            this.layoutControl3.Root = this.layoutControlGroup3;
            this.layoutControl3.Size = new System.Drawing.Size(214, 196);
            this.layoutControl3.TabIndex = 0;
            this.layoutControl3.Text = "layoutControl3";
            // 
            // cboEmrDocumentType
            // 
            this.cboEmrDocumentType.Location = new System.Drawing.Point(94, 124);
            this.cboEmrDocumentType.Name = "cboEmrDocumentType";
            this.cboEmrDocumentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboEmrDocumentType.Properties.NullText = "";
            this.cboEmrDocumentType.Properties.View = this.gridLookUpEdit2View;
            this.cboEmrDocumentType.Size = new System.Drawing.Size(116, 20);
            this.cboEmrDocumentType.StyleController = this.layoutControl3;
            this.cboEmrDocumentType.TabIndex = 8;
            this.cboEmrDocumentType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboEmrDocumentType_CustomDisplayText);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboDepartment
            // 
            this.cboDepartment.Location = new System.Drawing.Point(94, 100);
            this.cboDepartment.Name = "cboDepartment";
            this.cboDepartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDepartment.Properties.NullText = "";
            this.cboDepartment.Properties.View = this.gridLookUpEdit1View;
            this.cboDepartment.Size = new System.Drawing.Size(116, 20);
            this.cboDepartment.StyleController = this.layoutControl3;
            this.cboDepartment.TabIndex = 7;
            this.cboDepartment.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cboDepartment_CustomDisplayText);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // rdoStatus__TuChoiKy
            // 
            this.rdoStatus__TuChoiKy.Location = new System.Drawing.Point(94, 52);
            this.rdoStatus__TuChoiKy.Name = "rdoStatus__TuChoiKy";
            this.rdoStatus__TuChoiKy.Properties.Caption = "Từ chối ký";
            this.rdoStatus__TuChoiKy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdoStatus__TuChoiKy.Properties.FullFocusRect = true;
            this.rdoStatus__TuChoiKy.Properties.RadioGroupIndex = 1;
            this.rdoStatus__TuChoiKy.Size = new System.Drawing.Size(116, 19);
            this.rdoStatus__TuChoiKy.StyleController = this.layoutControl3;
            this.rdoStatus__TuChoiKy.TabIndex = 6;
            this.rdoStatus__TuChoiKy.TabStop = false;
            this.rdoStatus__TuChoiKy.CheckedChanged += new System.EventHandler(this.rdoStatus__TuChoiKy_CheckedChanged);
            // 
            // rdoStatus__DaKy
            // 
            this.rdoStatus__DaKy.Location = new System.Drawing.Point(94, 28);
            this.rdoStatus__DaKy.Name = "rdoStatus__DaKy";
            this.rdoStatus__DaKy.Properties.Caption = "Đã ký";
            this.rdoStatus__DaKy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdoStatus__DaKy.Properties.FullFocusRect = true;
            this.rdoStatus__DaKy.Properties.RadioGroupIndex = 1;
            this.rdoStatus__DaKy.Size = new System.Drawing.Size(116, 19);
            this.rdoStatus__DaKy.StyleController = this.layoutControl3;
            this.rdoStatus__DaKy.TabIndex = 5;
            this.rdoStatus__DaKy.TabStop = false;
            this.rdoStatus__DaKy.CheckedChanged += new System.EventHandler(this.rdoStatus__DaKy_CheckedChanged);
            // 
            // rdoStatus__ChoKy
            // 
            this.rdoStatus__ChoKy.Location = new System.Drawing.Point(94, 4);
            this.rdoStatus__ChoKy.Name = "rdoStatus__ChoKy";
            this.rdoStatus__ChoKy.Properties.Caption = "Chờ ký";
            this.rdoStatus__ChoKy.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.rdoStatus__ChoKy.Properties.FullFocusRect = true;
            this.rdoStatus__ChoKy.Properties.RadioGroupIndex = 1;
            this.rdoStatus__ChoKy.Size = new System.Drawing.Size(116, 19);
            this.rdoStatus__ChoKy.StyleController = this.layoutControl3;
            this.rdoStatus__ChoKy.TabIndex = 4;
            this.rdoStatus__ChoKy.TabStop = false;
            this.rdoStatus__ChoKy.CheckedChanged += new System.EventHandler(this.rdoStatus__ChoKy_CheckedChanged);
            // 
            // chkDelete
            // 
            this.chkDelete.Location = new System.Drawing.Point(94, 76);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Properties.Caption = "";
            this.chkDelete.Size = new System.Drawing.Size(116, 19);
            this.chkDelete.StyleController = this.layoutControl3;
            this.chkDelete.TabIndex = 5;
            // 
            // layoutControlGroup3
            // 
            this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup3.GroupBordersVisible = false;
            this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem10,
            this.layoutControlItem11,
            this.layoutControlItem12,
            this.layoutControlItem9,
            this.layoutControlItem13,
            this.layoutControlItem16});
            this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup3.Name = "layoutControlGroup3";
            this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
            this.layoutControlGroup3.Size = new System.Drawing.Size(214, 196);
            this.layoutControlGroup3.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.rdoStatus__ChoKy;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(210, 24);
            this.layoutControlItem10.Text = " ";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // layoutControlItem11
            // 
            this.layoutControlItem11.Control = this.rdoStatus__DaKy;
            this.layoutControlItem11.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem11.Name = "layoutControlItem11";
            this.layoutControlItem11.Size = new System.Drawing.Size(210, 24);
            this.layoutControlItem11.Text = " ";
            this.layoutControlItem11.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem11.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem11.TextToControlDistance = 5;
            // 
            // layoutControlItem12
            // 
            this.layoutControlItem12.Control = this.rdoStatus__TuChoiKy;
            this.layoutControlItem12.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem12.Name = "layoutControlItem12";
            this.layoutControlItem12.Size = new System.Drawing.Size(210, 24);
            this.layoutControlItem12.Text = " ";
            this.layoutControlItem12.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem12.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem12.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem9.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem9.Control = this.chkDelete;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 72);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(210, 24);
            this.layoutControlItem9.Text = "Bao gồm VBH:";
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem9.TextToControlDistance = 5;
            // 
            // layoutControlItem13
            // 
            this.layoutControlItem13.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem13.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem13.Control = this.cboDepartment;
            this.layoutControlItem13.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem13.Name = "layoutControlItem13";
            this.layoutControlItem13.Size = new System.Drawing.Size(210, 24);
            this.layoutControlItem13.Text = "Khoa ra viện:";
            this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem13.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem13.TextToControlDistance = 5;
            // 
            // layoutControlItem16
            // 
            this.layoutControlItem16.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem16.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem16.Control = this.cboEmrDocumentType;
            this.layoutControlItem16.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem16.Name = "layoutControlItem16";
            this.layoutControlItem16.Size = new System.Drawing.Size(210, 72);
            this.layoutControlItem16.Text = "Loại văn bản:";
            this.layoutControlItem16.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem16.TextSize = new System.Drawing.Size(85, 20);
            this.layoutControlItem16.TextToControlDistance = 5;
            // 
            // NavBarStatus
            // 
            this.NavBarStatus.Caption = "Trạng thái";
            this.NavBarStatus.ControlContainer = this.navBarGroupControlContainer2;
            this.NavBarStatus.Expanded = true;
            this.NavBarStatus.GroupClientHeight = 200;
            this.NavBarStatus.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.NavBarStatus.Name = "NavBarStatus";
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Location = new System.Drawing.Point(107, 576);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(117, 22);
            this.BtnRefresh.StyleController = this.layoutControl1;
            this.BtnRefresh.TabIndex = 7;
            this.BtnRefresh.Text = "Làm lại(Ctrl R)";
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // BtnSearch
            // 
            this.BtnSearch.Location = new System.Drawing.Point(2, 576);
            this.BtnSearch.Name = "BtnSearch";
            this.BtnSearch.Size = new System.Drawing.Size(101, 22);
            this.BtnSearch.StyleController = this.layoutControl1;
            this.BtnSearch.TabIndex = 6;
            this.BtnSearch.Text = "Tìm(Ctrl F)";
            this.BtnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // gridControlDocument
            // 
            this.gridControlDocument.Location = new System.Drawing.Point(228, 2);
            this.gridControlDocument.MainView = this.gridViewDocument;
            this.gridControlDocument.Name = "gridControlDocument";
            this.gridControlDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemBtnSign,
            this.repositoryItemBtnSignDisable,
            this.repositoryItemBtnView});
            this.gridControlDocument.Size = new System.Drawing.Size(1090, 572);
            this.gridControlDocument.TabIndex = 5;
            this.gridControlDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDocument});
            // 
            // gridViewDocument
            // 
            this.gridViewDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.STT,
            this.Gc_View,
            this.Gc_Sign,
            this.Gc_DocumentCode,
            this.Gc_DocumentName,
            this.Gc_TreatmentCode,
            this.Gc_PatientCode,
            this.Gc_PatientName,
            this.Gc_Dob,
            this.Gc_Gender,
            this.Gc_Department,
            this.Gc_RejectReason,
            this.GC_StoreTime,
            this.Gc_Url,
            this.Gc_CreateTime,
            this.Gc_Creator,
            this.Gc_ModifyTime,
            this.Gc_Modifier});
            this.gridViewDocument.GridControl = this.gridControlDocument;
            this.gridViewDocument.Name = "gridViewDocument";
            this.gridViewDocument.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gridViewDocument.OptionsSelection.MultiSelect = true;
            this.gridViewDocument.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewDocument.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewDocument.OptionsView.ColumnAutoWidth = false;
            this.gridViewDocument.OptionsView.ShowGroupPanel = false;
            this.gridViewDocument.OptionsView.ShowIndicator = false;
            this.gridViewDocument.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridViewDocument_RowCellStyle);
            this.gridViewDocument.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewDocument_CustomRowCellEdit);
            this.gridViewDocument.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewDocument_SelectionChanged);
            this.gridViewDocument.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewDocument_CustomUnboundColumnData);
            this.gridViewDocument.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridViewDocument_MouseDown);
            // 
            // STT
            // 
            this.STT.AppearanceCell.Options.UseTextOptions = true;
            this.STT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.STT.AppearanceHeader.Options.UseTextOptions = true;
            this.STT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.STT.Caption = "STT";
            this.STT.FieldName = "STT";
            this.STT.Name = "STT";
            this.STT.OptionsColumn.AllowEdit = false;
            this.STT.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.STT.Visible = true;
            this.STT.VisibleIndex = 1;
            this.STT.Width = 40;
            // 
            // Gc_View
            // 
            this.Gc_View.Caption = "Xem";
            this.Gc_View.ColumnEdit = this.repositoryItemBtnView;
            this.Gc_View.FieldName = "DETAIL";
            this.Gc_View.Name = "Gc_View";
            this.Gc_View.OptionsColumn.ShowCaption = false;
            this.Gc_View.OptionsFilter.AllowAutoFilter = false;
            this.Gc_View.OptionsFilter.AllowFilter = false;
            this.Gc_View.Visible = true;
            this.Gc_View.VisibleIndex = 2;
            this.Gc_View.Width = 25;
            // 
            // repositoryItemBtnView
            // 
            this.repositoryItemBtnView.AutoHeight = false;
            this.repositoryItemBtnView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnView.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xem", null, null, true)});
            this.repositoryItemBtnView.Name = "repositoryItemBtnView";
            this.repositoryItemBtnView.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // Gc_Sign
            // 
            this.Gc_Sign.Caption = "Ký";
            this.Gc_Sign.FieldName = "SIGN";
            this.Gc_Sign.Name = "Gc_Sign";
            this.Gc_Sign.OptionsColumn.ShowCaption = false;
            this.Gc_Sign.Visible = true;
            this.Gc_Sign.VisibleIndex = 3;
            this.Gc_Sign.Width = 25;
            // 
            // Gc_DocumentCode
            // 
            this.Gc_DocumentCode.Caption = "Mã văn bản";
            this.Gc_DocumentCode.FieldName = "DOCUMENT_CODE";
            this.Gc_DocumentCode.Name = "Gc_DocumentCode";
            this.Gc_DocumentCode.OptionsColumn.ReadOnly = true;
            this.Gc_DocumentCode.Visible = true;
            this.Gc_DocumentCode.VisibleIndex = 4;
            this.Gc_DocumentCode.Width = 100;
            // 
            // Gc_DocumentName
            // 
            this.Gc_DocumentName.Caption = "Tên văn bản";
            this.Gc_DocumentName.FieldName = "DOCUMENT_NAME";
            this.Gc_DocumentName.Name = "Gc_DocumentName";
            this.Gc_DocumentName.OptionsColumn.AllowEdit = false;
            this.Gc_DocumentName.Visible = true;
            this.Gc_DocumentName.VisibleIndex = 5;
            this.Gc_DocumentName.Width = 200;
            // 
            // Gc_TreatmentCode
            // 
            this.Gc_TreatmentCode.Caption = "Mã điều trị";
            this.Gc_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.Gc_TreatmentCode.Name = "Gc_TreatmentCode";
            this.Gc_TreatmentCode.OptionsColumn.ReadOnly = true;
            this.Gc_TreatmentCode.Visible = true;
            this.Gc_TreatmentCode.VisibleIndex = 6;
            this.Gc_TreatmentCode.Width = 90;
            // 
            // Gc_PatientCode
            // 
            this.Gc_PatientCode.Caption = "Mã bệnh nhân";
            this.Gc_PatientCode.FieldName = "PATIENT_CODE";
            this.Gc_PatientCode.Name = "Gc_PatientCode";
            this.Gc_PatientCode.OptionsColumn.ReadOnly = true;
            this.Gc_PatientCode.Visible = true;
            this.Gc_PatientCode.VisibleIndex = 7;
            this.Gc_PatientCode.Width = 90;
            // 
            // Gc_PatientName
            // 
            this.Gc_PatientName.Caption = "Họ tên";
            this.Gc_PatientName.FieldName = "VIR_PATIENT_NAME";
            this.Gc_PatientName.Name = "Gc_PatientName";
            this.Gc_PatientName.OptionsColumn.AllowEdit = false;
            this.Gc_PatientName.Visible = true;
            this.Gc_PatientName.VisibleIndex = 8;
            this.Gc_PatientName.Width = 120;
            // 
            // Gc_Dob
            // 
            this.Gc_Dob.Caption = "Ngày sinh";
            this.Gc_Dob.FieldName = "DOB_STR";
            this.Gc_Dob.Name = "Gc_Dob";
            this.Gc_Dob.OptionsColumn.AllowEdit = false;
            this.Gc_Dob.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_Dob.Visible = true;
            this.Gc_Dob.VisibleIndex = 9;
            this.Gc_Dob.Width = 120;
            // 
            // Gc_Gender
            // 
            this.Gc_Gender.Caption = "Giới tính";
            this.Gc_Gender.FieldName = "GENDER_NAME";
            this.Gc_Gender.Name = "Gc_Gender";
            this.Gc_Gender.OptionsColumn.AllowEdit = false;
            this.Gc_Gender.Visible = true;
            this.Gc_Gender.VisibleIndex = 10;
            this.Gc_Gender.Width = 80;
            // 
            // Gc_Department
            // 
            this.Gc_Department.Caption = "Khoa";
            this.Gc_Department.FieldName = "CURRENT_DEPARTMENT_NAME";
            this.Gc_Department.Name = "Gc_Department";
            this.Gc_Department.OptionsColumn.AllowEdit = false;
            this.Gc_Department.Visible = true;
            this.Gc_Department.VisibleIndex = 11;
            this.Gc_Department.Width = 120;
            // 
            // GC_StoreTime
            // 
            this.GC_StoreTime.Caption = "Thời gian Lưu";
            this.GC_StoreTime.FieldName = "STORE_TIME_STR";
            this.GC_StoreTime.Name = "GC_StoreTime";
            this.GC_StoreTime.OptionsColumn.AllowEdit = false;
            this.GC_StoreTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.GC_StoreTime.Width = 120;
            // 
            // Gc_Url
            // 
            this.Gc_Url.Caption = "URL";
            this.Gc_Url.FieldName = "URL";
            this.Gc_Url.Name = "Gc_Url";
            this.Gc_Url.OptionsColumn.ReadOnly = true;
            this.Gc_Url.Width = 150;
            // 
            // Gc_CreateTime
            // 
            this.Gc_CreateTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_CreateTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_CreateTime.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_CreateTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_CreateTime.Caption = "Ngày tạo";
            this.Gc_CreateTime.FieldName = "CREATE_TIME_DISPLAY";
            this.Gc_CreateTime.Name = "Gc_CreateTime";
            this.Gc_CreateTime.OptionsColumn.AllowEdit = false;
            this.Gc_CreateTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_CreateTime.Visible = true;
            this.Gc_CreateTime.VisibleIndex = 13;
            this.Gc_CreateTime.Width = 120;
            // 
            // Gc_Creator
            // 
            this.Gc_Creator.Caption = "Người tạo";
            this.Gc_Creator.FieldName = "CREATOR";
            this.Gc_Creator.Name = "Gc_Creator";
            this.Gc_Creator.OptionsColumn.AllowEdit = false;
            this.Gc_Creator.Visible = true;
            this.Gc_Creator.VisibleIndex = 14;
            this.Gc_Creator.Width = 110;
            // 
            // Gc_ModifyTime
            // 
            this.Gc_ModifyTime.AppearanceCell.Options.UseTextOptions = true;
            this.Gc_ModifyTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ModifyTime.AppearanceHeader.Options.UseTextOptions = true;
            this.Gc_ModifyTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.Gc_ModifyTime.Caption = "Ngày sửa";
            this.Gc_ModifyTime.FieldName = "MODIFY_TIME_DISPLAY";
            this.Gc_ModifyTime.Name = "Gc_ModifyTime";
            this.Gc_ModifyTime.OptionsColumn.AllowEdit = false;
            this.Gc_ModifyTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.Gc_ModifyTime.Visible = true;
            this.Gc_ModifyTime.VisibleIndex = 15;
            this.Gc_ModifyTime.Width = 120;
            // 
            // Gc_Modifier
            // 
            this.Gc_Modifier.Caption = "Người sửa";
            this.Gc_Modifier.FieldName = "MODIFIER";
            this.Gc_Modifier.Name = "Gc_Modifier";
            this.Gc_Modifier.OptionsColumn.AllowEdit = false;
            this.Gc_Modifier.Visible = true;
            this.Gc_Modifier.VisibleIndex = 16;
            this.Gc_Modifier.Width = 110;
            // 
            // repositoryItemBtnSign
            // 
            this.repositoryItemBtnSign.AutoHeight = false;
            this.repositoryItemBtnSign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnSign.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Ký", null, null, true)});
            this.repositoryItemBtnSign.Name = "repositoryItemBtnSign";
            this.repositoryItemBtnSign.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // repositoryItemBtnSignDisable
            // 
            this.repositoryItemBtnSignDisable.AutoHeight = false;
            this.repositoryItemBtnSignDisable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, false, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnSignDisable.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject9, serializableAppearanceObject10, serializableAppearanceObject11, serializableAppearanceObject12, "Ký", null, null, true)});
            this.repositoryItemBtnSignDisable.Name = "repositoryItemBtnSignDisable";
            this.repositoryItemBtnSignDisable.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            // 
            // TxtKeyword
            // 
            this.TxtKeyword.Location = new System.Drawing.Point(2, 2);
            this.TxtKeyword.Name = "TxtKeyword";
            this.TxtKeyword.Properties.NullText = "Từ khóa tìm kiếm";
            this.TxtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.TxtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.TxtKeyword.Size = new System.Drawing.Size(222, 20);
            this.TxtKeyword.StyleController = this.layoutControl1;
            this.TxtKeyword.TabIndex = 4;
            this.TxtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TxtKeyword_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem14,
            this.layoutControlItem15,
            this.lciForbtnErrorSignDetail,
            this.layoutControlItem8});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 600);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.TxtKeyword;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(226, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControlDocument;
            this.layoutControlItem2.Location = new System.Drawing.Point(226, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1094, 576);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.BtnSearch;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 574);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.BtnRefresh;
            this.layoutControlItem4.Location = new System.Drawing.Point(105, 574);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(121, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.navBarControl1;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(226, 494);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.ucPaging;
            this.layoutControlItem6.Location = new System.Drawing.Point(226, 576);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(1094, 24);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnSigns;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 548);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem14
            // 
            this.layoutControlItem14.Control = this.lblDocProcessedCount;
            this.layoutControlItem14.Location = new System.Drawing.Point(0, 518);
            this.layoutControlItem14.MaxSize = new System.Drawing.Size(0, 40);
            this.layoutControlItem14.MinSize = new System.Drawing.Size(18, 30);
            this.layoutControlItem14.Name = "layoutControlItem14";
            this.layoutControlItem14.Size = new System.Drawing.Size(124, 30);
            this.layoutControlItem14.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem14.TextVisible = false;
            // 
            // layoutControlItem15
            // 
            this.layoutControlItem15.Control = this.lblDocProcessErrorCount;
            this.layoutControlItem15.Location = new System.Drawing.Point(124, 518);
            this.layoutControlItem15.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem15.MinSize = new System.Drawing.Size(18, 30);
            this.layoutControlItem15.Name = "layoutControlItem15";
            this.layoutControlItem15.Size = new System.Drawing.Size(73, 30);
            this.layoutControlItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem15.TextVisible = false;
            // 
            // lciForbtnErrorSignDetail
            // 
            this.lciForbtnErrorSignDetail.Control = this.btnErrorSignDetail;
            this.lciForbtnErrorSignDetail.Location = new System.Drawing.Point(197, 518);
            this.lciForbtnErrorSignDetail.Name = "lciForbtnErrorSignDetail";
            this.lciForbtnErrorSignDetail.Size = new System.Drawing.Size(29, 30);
            this.lciForbtnErrorSignDetail.TextSize = new System.Drawing.Size(0, 0);
            this.lciForbtnErrorSignDetail.TextVisible = false;
            this.lciForbtnErrorSignDetail.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnViewAndSign;
            this.layoutControlItem8.Location = new System.Drawing.Point(105, 548);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.OptionsToolTip.ToolTip = "Xem và Ký lần lượt các văn bản được chọn";
            this.layoutControlItem8.Size = new System.Drawing.Size(121, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // Gc_RejectReason
            // 
            this.Gc_RejectReason.Caption = "Lý do từ chối";
            this.Gc_RejectReason.FieldName = "REJECT_REASON";
            this.Gc_RejectReason.Name = "Gc_RejectReason";
            this.Gc_RejectReason.OptionsColumn.AllowEdit = false;
            this.Gc_RejectReason.Visible = true;
            this.Gc_RejectReason.VisibleIndex = 12;
            this.Gc_RejectReason.Width = 120;
            // 
            // UcEmrSignDocumentList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UcEmrSignDocumentList";
            this.Size = new System.Drawing.Size(1320, 600);
            this.Load += new System.EventHandler(this.UcEmrSignDocumentList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
            this.navBarControl1.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTimeFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LciTimeTo)).EndInit();
            this.navBarGroupControlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
            this.layoutControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboEmrDocumentType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__TuChoiKy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__DaKy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdoStatus__ChoKy.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDelete.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDocument)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnSign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnSignDisable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForbtnErrorSignDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl gridControlDocument;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDocument;
        private DevExpress.XtraEditors.TextEdit TxtKeyword;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraNavBar.NavBarControl navBarControl1;
        private DevExpress.XtraNavBar.NavBarGroup NavBarCreateTime;
        private DevExpress.XtraEditors.SimpleButton BtnRefresh;
        private DevExpress.XtraEditors.SimpleButton BtnSearch;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.DateEdit DtTimeTo;
        private DevExpress.XtraEditors.DateEdit DtTimeFrom;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraLayout.LayoutControlItem LciTimeFrom;
        private DevExpress.XtraLayout.LayoutControlItem LciTimeTo;
        private DevExpress.XtraNavBar.NavBarGroup NavBarStatus;
        private DevExpress.XtraGrid.Columns.GridColumn STT;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Sign;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_DocumentCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_DocumentName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_PatientCode;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_PatientName;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Dob;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Gender;
        private DevExpress.XtraGrid.Columns.GridColumn GC_StoreTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Url;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_CreateTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Creator;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_ModifyTime;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Modifier;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnSign;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnSignDisable;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_View;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnView;
        private DevExpress.XtraEditors.CheckEdit chkDelete;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup3;
        private DevExpress.XtraEditors.CheckEdit rdoStatus__TuChoiKy;
        private DevExpress.XtraEditors.CheckEdit rdoStatus__DaKy;
        private DevExpress.XtraEditors.CheckEdit rdoStatus__ChoKy;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem11;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem12;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.SimpleButton btnSigns;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.LabelControl lblDocProcessErrorCount;
        private DevExpress.XtraEditors.LabelControl lblDocProcessedCount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem14;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem15;
        private DevExpress.XtraEditors.SimpleButton btnErrorSignDetail;
        private DevExpress.XtraLayout.LayoutControlItem lciForbtnErrorSignDetail;
        private DevExpress.XtraEditors.SimpleButton btnViewAndSign;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_Department;
        private DevExpress.XtraEditors.GridLookUpEdit cboEmrDocumentType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.GridLookUpEdit cboDepartment;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem13;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem16;
        private DevExpress.XtraGrid.Columns.GridColumn Gc_RejectReason;
    }
}
