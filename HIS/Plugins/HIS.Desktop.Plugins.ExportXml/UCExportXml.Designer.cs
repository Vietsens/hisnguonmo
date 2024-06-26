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
namespace HIS.Desktop.Plugins.ExportXml
{
    partial class UCExportXml
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.CboBranch = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboFilterType = new System.Windows.Forms.ComboBox();
            this.ucPaging1 = new Inventec.UC.Paging.UcPaging();
            this.btnExportXml = new DevExpress.XtraEditors.SimpleButton();
            this.panelControlSereServTree = new DevExpress.XtraEditors.PanelControl();
            this.gridControlHeinApprovalBhyt = new DevExpress.XtraGrid.GridControl();
            this.gridViewHeinApprovalBhyt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_HeinApprovalBhyt_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_TreatmentCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_VirPatientName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_RightRouteCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_Address = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_HeinApprovalBhyt_Execute_Time = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnFind = new DevExpress.XtraEditors.SimpleButton();
            this.txtKeyword = new DevExpress.XtraEditors.TextEdit();
            this.txtFindTreatmentCode = new DevExpress.XtraEditors.TextEdit();
            this.dtToExecuteTime = new DevExpress.XtraEditors.DateEdit();
            this.dtFromExecuteTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutFromExecuteTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutToExecuteTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboBranch = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CboBranch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSereServTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHeinApprovalBhyt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHeinApprovalBhyt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindTreatmentCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToExecuteTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToExecuteTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromExecuteTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromExecuteTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutFromExecuteTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutToExecuteTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboBranch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.CboBranch);
            this.layoutControl1.Controls.Add(this.cboFilterType);
            this.layoutControl1.Controls.Add(this.ucPaging1);
            this.layoutControl1.Controls.Add(this.btnExportXml);
            this.layoutControl1.Controls.Add(this.panelControlSereServTree);
            this.layoutControl1.Controls.Add(this.gridControlHeinApprovalBhyt);
            this.layoutControl1.Controls.Add(this.btnFind);
            this.layoutControl1.Controls.Add(this.txtKeyword);
            this.layoutControl1.Controls.Add(this.txtFindTreatmentCode);
            this.layoutControl1.Controls.Add(this.dtToExecuteTime);
            this.layoutControl1.Controls.Add(this.dtFromExecuteTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 670);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // CboBranch
            // 
            this.CboBranch.Location = new System.Drawing.Point(97, 26);
            this.CboBranch.Name = "CboBranch";
            this.CboBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.CboBranch.Properties.NullText = "";
            this.CboBranch.Properties.View = this.gridLookUpEdit1View;
            this.CboBranch.Size = new System.Drawing.Size(561, 20);
            this.CboBranch.StyleController = this.layoutControl1;
            this.CboBranch.TabIndex = 14;
            this.CboBranch.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.CboBranch_CustomDisplayText);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // cboFilterType
            // 
            this.cboFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterType.FormattingEnabled = true;
            this.cboFilterType.Location = new System.Drawing.Point(442, 50);
            this.cboFilterType.Name = "cboFilterType";
            this.cboFilterType.Size = new System.Drawing.Size(106, 21);
            this.cboFilterType.TabIndex = 8;
            // 
            // ucPaging1
            // 
            this.ucPaging1.Location = new System.Drawing.Point(2, 616);
            this.ucPaging1.Name = "ucPaging1";
            this.ucPaging1.Size = new System.Drawing.Size(656, 26);
            this.ucPaging1.TabIndex = 13;
            // 
            // btnExportXml
            // 
            this.btnExportXml.Location = new System.Drawing.Point(552, 646);
            this.btnExportXml.Name = "btnExportXml";
            this.btnExportXml.Size = new System.Drawing.Size(106, 22);
            this.btnExportXml.StyleController = this.layoutControl1;
            this.btnExportXml.TabIndex = 12;
            this.btnExportXml.Text = "Xuất (Ctrl E)";
            this.btnExportXml.Click += new System.EventHandler(this.btnExportXml_Click);
            // 
            // panelControlSereServTree
            // 
            this.panelControlSereServTree.Location = new System.Drawing.Point(660, 0);
            this.panelControlSereServTree.Name = "panelControlSereServTree";
            this.panelControlSereServTree.Size = new System.Drawing.Size(660, 670);
            this.panelControlSereServTree.TabIndex = 11;
            // 
            // gridControlHeinApprovalBhyt
            // 
            this.gridControlHeinApprovalBhyt.Location = new System.Drawing.Point(0, 74);
            this.gridControlHeinApprovalBhyt.MainView = this.gridViewHeinApprovalBhyt;
            this.gridControlHeinApprovalBhyt.Name = "gridControlHeinApprovalBhyt";
            this.gridControlHeinApprovalBhyt.Size = new System.Drawing.Size(660, 540);
            this.gridControlHeinApprovalBhyt.TabIndex = 10;
            this.gridControlHeinApprovalBhyt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewHeinApprovalBhyt});
            // 
            // gridViewHeinApprovalBhyt
            // 
            this.gridViewHeinApprovalBhyt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_HeinApprovalBhyt_Stt,
            this.gridColumn_HeinApprovalBhyt_TreatmentCode,
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode,
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber,
            this.gridColumn_HeinApprovalBhyt_VirPatientName,
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime,
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime,
            this.gridColumn_HeinApprovalBhyt_RightRouteCode,
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode,
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode,
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode,
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode,
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName,
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate,
            this.gridColumn_HeinApprovalBhyt_Address,
            this.gridColumn_HeinApprovalBhyt_Execute_Time});
            this.gridViewHeinApprovalBhyt.GridControl = this.gridControlHeinApprovalBhyt;
            this.gridViewHeinApprovalBhyt.Name = "gridViewHeinApprovalBhyt";
            this.gridViewHeinApprovalBhyt.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            this.gridViewHeinApprovalBhyt.OptionsSelection.MultiSelect = true;
            this.gridViewHeinApprovalBhyt.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewHeinApprovalBhyt.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewHeinApprovalBhyt.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DevExpress.Utils.DefaultBoolean.True;
            this.gridViewHeinApprovalBhyt.OptionsView.ColumnAutoWidth = false;
            this.gridViewHeinApprovalBhyt.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewHeinApprovalBhyt.OptionsView.ShowGroupPanel = false;
            this.gridViewHeinApprovalBhyt.OptionsView.ShowIndicator = false;
            this.gridViewHeinApprovalBhyt.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewHeinApprovalBhyt_RowClick);
            this.gridViewHeinApprovalBhyt.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridViewHeinApprovalBhyt_SelectionChanged);
            this.gridViewHeinApprovalBhyt.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewHeinApprovalBhyt_CustomUnboundColumnData);
            // 
            // gridColumn_HeinApprovalBhyt_Stt
            // 
            this.gridColumn_HeinApprovalBhyt_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_HeinApprovalBhyt_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_Stt.Caption = "STT";
            this.gridColumn_HeinApprovalBhyt_Stt.FieldName = "STT";
            this.gridColumn_HeinApprovalBhyt_Stt.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_HeinApprovalBhyt_Stt.Name = "gridColumn_HeinApprovalBhyt_Stt";
            this.gridColumn_HeinApprovalBhyt_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_Stt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn_HeinApprovalBhyt_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_Stt.Visible = true;
            this.gridColumn_HeinApprovalBhyt_Stt.VisibleIndex = 1;
            this.gridColumn_HeinApprovalBhyt_Stt.Width = 35;
            // 
            // gridColumn_HeinApprovalBhyt_TreatmentCode
            // 
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.Caption = "Mã điều trị";
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.FieldName = "TREATMENT_CODE";
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.Name = "gridColumn_HeinApprovalBhyt_TreatmentCode";
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.VisibleIndex = 2;
            this.gridColumn_HeinApprovalBhyt_TreatmentCode.Width = 85;
            // 
            // gridColumn_HeinApprovalBhyt_HeinApprovalCode
            // 
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Caption = "Mã hs duyệt";
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.FieldName = "HEIN_APPROVAL_CODE";
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Name = "gridColumn_HeinApprovalBhyt_HeinApprovalCode";
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.VisibleIndex = 3;
            this.gridColumn_HeinApprovalBhyt_HeinApprovalCode.Width = 84;
            // 
            // gridColumn_HeinApprovalBhyt_HeinCardNumber
            // 
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Caption = "Số thẻ";
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.FieldName = "HEIN_CARD_NUMBER";
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Name = "gridColumn_HeinApprovalBhyt_HeinCardNumber";
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.VisibleIndex = 4;
            this.gridColumn_HeinApprovalBhyt_HeinCardNumber.Width = 110;
            // 
            // gridColumn_HeinApprovalBhyt_VirPatientName
            // 
            this.gridColumn_HeinApprovalBhyt_VirPatientName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_VirPatientName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_VirPatientName.Caption = "Bệnh nhân";
            this.gridColumn_HeinApprovalBhyt_VirPatientName.FieldName = "TDL_PATIENT_NAME";
            this.gridColumn_HeinApprovalBhyt_VirPatientName.Name = "gridColumn_HeinApprovalBhyt_VirPatientName";
            this.gridColumn_HeinApprovalBhyt_VirPatientName.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_VirPatientName.Visible = true;
            this.gridColumn_HeinApprovalBhyt_VirPatientName.VisibleIndex = 5;
            this.gridColumn_HeinApprovalBhyt_VirPatientName.Width = 120;
            // 
            // gridColumn_HeinApprovalBhyt_HeinCardFromTime
            // 
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.Caption = "Hạn từ";
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.FieldName = "HEIN_CARD_FROM_TIME_STR";
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.Name = "gridColumn_HeinApprovalBhyt_HeinCardFromTime";
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.VisibleIndex = 6;
            this.gridColumn_HeinApprovalBhyt_HeinCardFromTime.Width = 90;
            // 
            // gridColumn_HeinApprovalBhyt_HeinCardToTime
            // 
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.Caption = "Hạn đến";
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.FieldName = "HEIN_CARD_TO_TIME_STR";
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.Name = "gridColumn_HeinApprovalBhyt_HeinCardToTime";
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.VisibleIndex = 7;
            this.gridColumn_HeinApprovalBhyt_HeinCardToTime.Width = 90;
            // 
            // gridColumn_HeinApprovalBhyt_RightRouteCode
            // 
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.Caption = "Tuyến";
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.FieldName = "RIGHT_ROUTE_CODE_STR";
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.Name = "gridColumn_HeinApprovalBhyt_RightRouteCode";
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_RightRouteCode.VisibleIndex = 8;
            // 
            // gridColumn_HeinApprovalBhyt_RightRouteTypeCode
            // 
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.Caption = "Loại tuyến";
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.FieldName = "RIGHT_ROUTE_TYPE_CODE_STR";
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.Name = "gridColumn_HeinApprovalBhyt_RightRouteTypeCode";
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_RightRouteTypeCode.VisibleIndex = 9;
            // 
            // gridColumn_HeinApprovalBhyt_UpToStandardCode
            // 
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.Caption = "5 năm 6 tháng";
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.FieldName = "UP_TO_STRANDARD_CODE_STR";
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.Name = "gridColumn_HeinApprovalBhyt_UpToStandardCode";
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_UpToStandardCode.VisibleIndex = 10;
            // 
            // gridColumn_HeinApprovalBhyt_LiveAreaCode
            // 
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.Caption = "Mã KV sống";
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.FieldName = "LIVE_AREA_CODE";
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.Name = "gridColumn_HeinApprovalBhyt_LiveAreaCode";
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.VisibleIndex = 11;
            this.gridColumn_HeinApprovalBhyt_LiveAreaCode.Width = 60;
            // 
            // gridColumn_HeinApprovalBhyt_HeinMediOrgCode
            // 
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.Caption = "Mã ĐK KCB BĐ";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.FieldName = "HEIN_MEDI_ORG_CODE";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.Name = "gridColumn_HeinApprovalBhyt_HeinMediOrgCode";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.VisibleIndex = 12;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgCode.Width = 60;
            // 
            // gridColumn_HeinApprovalBhyt_HeinMediOrgName
            // 
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.Caption = "Tên ĐK KCB BĐ";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.FieldName = "HEIN_MEDI_ORG_NAME";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.Name = "gridColumn_HeinApprovalBhyt_HeinMediOrgName";
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.VisibleIndex = 13;
            this.gridColumn_HeinApprovalBhyt_HeinMediOrgName.Width = 200;
            // 
            // gridColumn_HeinApprovalBhyt_HasBirthCertificate
            // 
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.Caption = "Có giấy KS";
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.FieldName = "HAS_BIRTH_CERTIFICATE_STR";
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.Name = "gridColumn_HeinApprovalBhyt_HasBirthCertificate";
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.Visible = true;
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.VisibleIndex = 14;
            this.gridColumn_HeinApprovalBhyt_HasBirthCertificate.Width = 60;
            // 
            // gridColumn_HeinApprovalBhyt_Address
            // 
            this.gridColumn_HeinApprovalBhyt_Address.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_Address.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_Address.Caption = "Địa chỉ thẻ bhyt";
            this.gridColumn_HeinApprovalBhyt_Address.FieldName = "ADDRESS";
            this.gridColumn_HeinApprovalBhyt_Address.Name = "gridColumn_HeinApprovalBhyt_Address";
            this.gridColumn_HeinApprovalBhyt_Address.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_Address.Visible = true;
            this.gridColumn_HeinApprovalBhyt_Address.VisibleIndex = 16;
            this.gridColumn_HeinApprovalBhyt_Address.Width = 200;
            // 
            // gridColumn_HeinApprovalBhyt_Execute_Time
            // 
            this.gridColumn_HeinApprovalBhyt_Execute_Time.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.Caption = "Thời gian giám định";
            this.gridColumn_HeinApprovalBhyt_Execute_Time.FieldName = "EXECUTE_TIME_STR";
            this.gridColumn_HeinApprovalBhyt_Execute_Time.Name = "gridColumn_HeinApprovalBhyt_Execute_Time";
            this.gridColumn_HeinApprovalBhyt_Execute_Time.OptionsColumn.AllowEdit = false;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.Visible = true;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.VisibleIndex = 15;
            this.gridColumn_HeinApprovalBhyt_Execute_Time.Width = 120;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(552, 50);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(106, 22);
            this.btnFind.StyleController = this.layoutControl1;
            this.btnFind.TabIndex = 9;
            this.btnFind.Text = "Tìm (Ctrl F)";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(2, 50);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Properties.NullValuePrompt = "Từ khóa tìm kiếm";
            this.txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtKeyword.Size = new System.Drawing.Size(436, 20);
            this.txtKeyword.StyleController = this.layoutControl1;
            this.txtKeyword.TabIndex = 7;
            this.txtKeyword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
            // 
            // txtFindTreatmentCode
            // 
            this.txtFindTreatmentCode.Location = new System.Drawing.Point(442, 2);
            this.txtFindTreatmentCode.Name = "txtFindTreatmentCode";
            this.txtFindTreatmentCode.Properties.NullValuePrompt = "Mã điều trị (F2)";
            this.txtFindTreatmentCode.Properties.NullValuePromptShowForEmptyValue = true;
            this.txtFindTreatmentCode.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtFindTreatmentCode.Size = new System.Drawing.Size(216, 20);
            this.txtFindTreatmentCode.StyleController = this.layoutControl1;
            this.txtFindTreatmentCode.TabIndex = 6;
            this.txtFindTreatmentCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtFindTreatmentCode_PreviewKeyDown);
            // 
            // dtToExecuteTime
            // 
            this.dtToExecuteTime.EditValue = null;
            this.dtToExecuteTime.Location = new System.Drawing.Point(317, 2);
            this.dtToExecuteTime.Name = "dtToExecuteTime";
            this.dtToExecuteTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToExecuteTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToExecuteTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtToExecuteTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToExecuteTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtToExecuteTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToExecuteTime.Size = new System.Drawing.Size(121, 20);
            this.dtToExecuteTime.StyleController = this.layoutControl1;
            this.dtToExecuteTime.TabIndex = 5;
            this.dtToExecuteTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtToExecuteTime_Closed);
            // 
            // dtFromExecuteTime
            // 
            this.dtFromExecuteTime.EditValue = null;
            this.dtFromExecuteTime.Location = new System.Drawing.Point(97, 2);
            this.dtFromExecuteTime.Name = "dtFromExecuteTime";
            this.dtFromExecuteTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromExecuteTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromExecuteTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtFromExecuteTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromExecuteTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtFromExecuteTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromExecuteTime.Size = new System.Drawing.Size(121, 20);
            this.dtFromExecuteTime.StyleController = this.layoutControl1;
            this.dtFromExecuteTime.TabIndex = 4;
            this.dtFromExecuteTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFromExecuteTime_Closed);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutFromExecuteTime,
            this.layoutToExecuteTime,
            this.layoutControlItem5,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem10,
            this.emptySpaceItem2,
            this.layoutControlItem9,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItem6,
            this.lciCboBranch});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 670);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutFromExecuteTime
            // 
            this.layoutFromExecuteTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutFromExecuteTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutFromExecuteTime.Control = this.dtFromExecuteTime;
            this.layoutFromExecuteTime.Location = new System.Drawing.Point(0, 0);
            this.layoutFromExecuteTime.Name = "layoutFromExecuteTime";
            this.layoutFromExecuteTime.Size = new System.Drawing.Size(220, 24);
            this.layoutFromExecuteTime.Text = "Duyệt bhyt từ:";
            this.layoutFromExecuteTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutFromExecuteTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutFromExecuteTime.TextToControlDistance = 5;
            // 
            // layoutToExecuteTime
            // 
            this.layoutToExecuteTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutToExecuteTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutToExecuteTime.Control = this.dtToExecuteTime;
            this.layoutToExecuteTime.Location = new System.Drawing.Point(220, 0);
            this.layoutToExecuteTime.Name = "layoutToExecuteTime";
            this.layoutToExecuteTime.Size = new System.Drawing.Size(220, 24);
            this.layoutToExecuteTime.Text = "Đến:";
            this.layoutToExecuteTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutToExecuteTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutToExecuteTime.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.txtKeyword;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 48);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(440, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.gridControlHeinApprovalBhyt;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 74);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem7.Size = new System.Drawing.Size(660, 540);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.panelControlSereServTree;
            this.layoutControlItem8.Location = new System.Drawing.Point(660, 0);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem8.Size = new System.Drawing.Size(660, 670);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.ucPaging1;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 614);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(660, 30);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 644);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(550, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.btnExportXml;
            this.layoutControlItem9.Location = new System.Drawing.Point(550, 644);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboFilterType;
            this.layoutControlItem1.Location = new System.Drawing.Point(440, 48);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.txtFindTreatmentCode;
            this.layoutControlItem3.Location = new System.Drawing.Point(440, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(220, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnFind;
            this.layoutControlItem6.Location = new System.Drawing.Point(550, 48);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(110, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciCboBranch
            // 
            this.lciCboBranch.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboBranch.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboBranch.Control = this.CboBranch;
            this.lciCboBranch.Location = new System.Drawing.Point(0, 24);
            this.lciCboBranch.Name = "lciCboBranch";
            this.lciCboBranch.Size = new System.Drawing.Size(660, 24);
            this.lciCboBranch.Text = "Chi nhánh:";
            this.lciCboBranch.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboBranch.TextSize = new System.Drawing.Size(90, 13);
            this.lciCboBranch.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCExportXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCExportXml";
            this.Size = new System.Drawing.Size(1320, 670);
            this.Load += new System.EventHandler(this.UCExportXml_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CboBranch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSereServTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlHeinApprovalBhyt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewHeinApprovalBhyt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFindTreatmentCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToExecuteTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToExecuteTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromExecuteTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromExecuteTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutFromExecuteTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutToExecuteTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboBranch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DateEdit dtToExecuteTime;
        private DevExpress.XtraEditors.DateEdit dtFromExecuteTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutFromExecuteTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutToExecuteTime;
        private DevExpress.XtraEditors.TextEdit txtKeyword;
        private DevExpress.XtraEditors.TextEdit txtFindTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private Inventec.UC.Paging.UcPaging ucPaging1;
        private DevExpress.XtraEditors.SimpleButton btnExportXml;
        private DevExpress.XtraEditors.PanelControl panelControlSereServTree;
        private DevExpress.XtraGrid.GridControl gridControlHeinApprovalBhyt;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewHeinApprovalBhyt;
        private DevExpress.XtraEditors.SimpleButton btnFind;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_TreatmentCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinApprovalCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinCardNumber;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_VirPatientName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinCardFromTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinCardToTime;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_RightRouteCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_RightRouteTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_UpToStandardCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinMediOrgCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HeinMediOrgName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_Address;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_LiveAreaCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_HasBirthCertificate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_HeinApprovalBhyt_Execute_Time;
        private System.Windows.Forms.ComboBox cboFilterType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.GridLookUpEdit CboBranch;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciCboBranch;
    }
}
