/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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
namespace HIS.Desktop.Plugins.MedicineMediStockSummaryVertical
{
    partial class ucMedicineMediStockSummaryVertical
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
            this.grdData = new DevExpress.XtraGrid.GridControl();
            this.gridViewData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboType = new Inventec.Desktop.CustomControl.NoFocus.CustomGridLookUpEditWithFilterMultiColumnNoFocus();
            this.cboTypeView = new Inventec.Desktop.CustomControl.NoFocus.CustomGridViewWithFilterMultiColumnNoFocus();
            this.btnExportExcel = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.dteToDate = new DevExpress.XtraEditors.DateEdit();
            this.dteFromDate = new DevExpress.XtraEditors.DateEdit();
            this.cboBranch = new DevExpress.XtraEditors.GridLookUpEdit();
            this.cboBranchView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.chkMaterial = new DevExpress.XtraEditors.CheckEdit();
            this.chkMedicine = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciMedicine = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMaterial = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBranch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFromDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciToDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSearch = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExportExcel = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciGrid = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTypeView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranchView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMaterial.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedicine.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExportExcel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grdData);
            this.layoutControl1.Controls.Add(this.cboType);
            this.layoutControl1.Controls.Add(this.btnExportExcel);
            this.layoutControl1.Controls.Add(this.btnSearch);
            this.layoutControl1.Controls.Add(this.dteToDate);
            this.layoutControl1.Controls.Add(this.dteFromDate);
            this.layoutControl1.Controls.Add(this.cboBranch);
            this.layoutControl1.Controls.Add(this.chkMaterial);
            this.layoutControl1.Controls.Add(this.chkMedicine);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1366, 513);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grdData
            // 
            this.grdData.Location = new System.Drawing.Point(2, 28);
            this.grdData.MainView = this.gridViewData;
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(1362, 483);
            this.grdData.TabIndex = 14;
            this.grdData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewData});
            // 
            // gridViewData
            // 
            this.gridViewData.GridControl = this.grdData;
            this.gridViewData.Name = "gridViewData";
            this.gridViewData.OptionsView.ShowFooter = true;
            this.gridViewData.OptionsView.ShowGroupPanel = false;
            // 
            // cboType
            // 
            this.cboType.Location = new System.Drawing.Point(990, 2);
            this.cboType.Name = "cboType";
            this.cboType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboType.Properties.AutoComplete = false;
            this.cboType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboType.Properties.NullText = "";
            this.cboType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboType.Properties.View = this.cboTypeView;
            this.cboType.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboType_ButtonClick);
            this.cboType.Size = new System.Drawing.Size(212, 20);
            this.cboType.StyleController = this.layoutControl1;
            this.cboType.TabIndex = 12;
            this.cboType.EditValueChanged += new System.EventHandler(this.cboType_EditValueChanged);
            // 
            // cboTypeView
            // 
            this.cboTypeView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cboTypeView.Name = "cboTypeView";
            this.cboTypeView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cboTypeView.OptionsView.ShowGroupPanel = false;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(1295, 2);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(69, 22);
            this.btnExportExcel.StyleController = this.layoutControl1;
            this.btnExportExcel.TabIndex = 11;
            this.btnExportExcel.Text = "Xuất Excel";
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(1206, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(85, 22);
            this.btnSearch.StyleController = this.layoutControl1;
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dteToDate
            // 
            this.dteToDate.EditValue = null;
            this.dteToDate.Location = new System.Drawing.Point(772, 2);
            this.dteToDate.Name = "dteToDate";
            this.dteToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteToDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
            this.dteToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dteToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dteToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteToDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dteToDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.dteToDate.Size = new System.Drawing.Size(109, 20);
            this.dteToDate.StyleController = this.layoutControl1;
            this.dteToDate.TabIndex = 8;
            // 
            // dteFromDate
            // 
            this.dteFromDate.EditValue = null;
            this.dteFromDate.Location = new System.Drawing.Point(593, 2);
            this.dteFromDate.Name = "dteFromDate";
            this.dteFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dteFromDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.Vista;
            this.dteFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dteFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dteFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.dteFromDate.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dteFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dteFromDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
            this.dteFromDate.Size = new System.Drawing.Size(110, 20);
            this.dteFromDate.StyleController = this.layoutControl1;
            this.dteFromDate.TabIndex = 7;
            // 
            // cboBranch
            // 
            this.cboBranch.Location = new System.Drawing.Point(318, 2);
            this.cboBranch.Name = "cboBranch";
            this.cboBranch.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboBranch.Properties.NullText = "";
            this.cboBranch.Properties.View = this.cboBranchView;
            this.cboBranch.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboBranch_ButtonClick);
            this.cboBranch.Size = new System.Drawing.Size(206, 20);
            this.cboBranch.StyleController = this.layoutControl1;
            this.cboBranch.TabIndex = 6;
            // 
            // cboBranchView
            // 
            this.cboBranchView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.cboBranchView.Name = "cboBranchView";
            this.cboBranchView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.cboBranchView.OptionsView.ShowGroupPanel = false;
            // 
            // chkMaterial
            // 
            this.chkMaterial.Location = new System.Drawing.Point(162, 2);
            this.chkMaterial.Name = "chkMaterial";
            this.chkMaterial.Properties.Caption = "Vật tư";
            this.chkMaterial.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkMaterial.Size = new System.Drawing.Size(87, 19);
            this.chkMaterial.StyleController = this.layoutControl1;
            this.chkMaterial.TabIndex = 5;
            this.chkMaterial.CheckedChanged += new System.EventHandler(this.chkMaterial_CheckedChanged);
            // 
            // chkMedicine
            // 
            this.chkMedicine.Location = new System.Drawing.Point(67, 2);
            this.chkMedicine.Name = "chkMedicine";
            this.chkMedicine.Properties.Caption = "Thuốc";
            this.chkMedicine.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.chkMedicine.Size = new System.Drawing.Size(91, 19);
            this.chkMedicine.StyleController = this.layoutControl1;
            this.chkMedicine.TabIndex = 4;
            this.chkMedicine.CheckedChanged += new System.EventHandler(this.chkMedicine_CheckedChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciMedicine,
            this.lciMaterial,
            this.lciBranch,
            this.lciFromDate,
            this.lciToDate,
            this.lciGrid,
            this.lciType,
            this.lciSearch,
            this.lciExportExcel});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1366, 513);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciMedicine
            // 
            this.lciMedicine.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMedicine.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMedicine.Control = this.chkMedicine;
            this.lciMedicine.Location = new System.Drawing.Point(0, 0);
            this.lciMedicine.Name = "lciMedicine";
            this.lciMedicine.Size = new System.Drawing.Size(160, 26);
            this.lciMedicine.Text = "Loại thuốc:";
            this.lciMedicine.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMedicine.TextSize = new System.Drawing.Size(60, 20);
            this.lciMedicine.TextToControlDistance = 5;
            // 
            // lciMaterial
            // 
            this.lciMaterial.Control = this.chkMaterial;
            this.lciMaterial.Location = new System.Drawing.Point(160, 0);
            this.lciMaterial.Name = "lciMaterial";
            this.lciMaterial.Size = new System.Drawing.Size(91, 26);
            this.lciMaterial.TextSize = new System.Drawing.Size(0, 0);
            this.lciMaterial.TextVisible = false;
            // 
            // lciBranch
            // 
            this.lciBranch.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBranch.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBranch.Control = this.cboBranch;
            this.lciBranch.Location = new System.Drawing.Point(251, 0);
            this.lciBranch.Name = "lciBranch";
            this.lciBranch.Size = new System.Drawing.Size(275, 26);
            this.lciBranch.Text = "Chi nhánh:";
            this.lciBranch.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBranch.TextSize = new System.Drawing.Size(60, 20);
            this.lciBranch.TextToControlDistance = 5;
            // 
            // lciFromDate
            // 
            this.lciFromDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFromDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFromDate.Control = this.dteFromDate;
            this.lciFromDate.Location = new System.Drawing.Point(526, 0);
            this.lciFromDate.Name = "lciFromDate";
            this.lciFromDate.Size = new System.Drawing.Size(179, 26);
            this.lciFromDate.Text = "Từ ngày:";
            this.lciFromDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFromDate.TextSize = new System.Drawing.Size(60, 20);
            this.lciFromDate.TextToControlDistance = 5;
            // 
            // lciToDate
            // 
            this.lciToDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciToDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciToDate.Control = this.dteToDate;
            this.lciToDate.Location = new System.Drawing.Point(705, 0);
            this.lciToDate.Name = "lciToDate";
            this.lciToDate.Size = new System.Drawing.Size(178, 26);
            this.lciToDate.Text = "Đến ngày:";
            this.lciToDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciToDate.TextSize = new System.Drawing.Size(60, 20);
            this.lciToDate.TextToControlDistance = 5;
            // 
            // lciSearch
            // 
            this.lciSearch.Control = this.btnSearch;
            this.lciSearch.Location = new System.Drawing.Point(1204, 0);
            this.lciSearch.Name = "lciSearch";
            this.lciSearch.Size = new System.Drawing.Size(89, 26);
            this.lciSearch.TextSize = new System.Drawing.Size(0, 0);
            this.lciSearch.TextVisible = false;
            // 
            // lciExportExcel
            // 
            this.lciExportExcel.Control = this.btnExportExcel;
            this.lciExportExcel.Location = new System.Drawing.Point(1293, 0);
            this.lciExportExcel.Name = "lciExportExcel";
            this.lciExportExcel.Size = new System.Drawing.Size(73, 26);
            this.lciExportExcel.TextSize = new System.Drawing.Size(0, 0);
            this.lciExportExcel.TextVisible = false;
            // 
            // lciType
            // 
            this.lciType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciType.Control = this.cboType;
            this.lciType.Location = new System.Drawing.Point(883, 0);
            this.lciType.Name = "lciType";
            this.lciType.Size = new System.Drawing.Size(321, 26);
            this.lciType.Text = "Loại thuốc/vật tư:";
            this.lciType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciType.TextSize = new System.Drawing.Size(100, 20);
            this.lciType.TextToControlDistance = 5;
            // 
            // lciGrid
            // 
            this.lciGrid.Control = this.grdData;
            this.lciGrid.Location = new System.Drawing.Point(0, 26);
            this.lciGrid.Name = "lciGrid";
            this.lciGrid.Size = new System.Drawing.Size(1366, 487);
            this.lciGrid.TextSize = new System.Drawing.Size(0, 0);
            this.lciGrid.TextVisible = false;
            // 
            // ucMedicineMediStockSummaryVertical
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "ucMedicineMediStockSummaryVertical";
            this.Size = new System.Drawing.Size(1366, 513);
            this.Load += new System.EventHandler(this.ucMedicineMediStockSummaryVertical_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTypeView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteToDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dteFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBranchView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMaterial.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkMedicine.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMedicine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBranch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSearch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExportExcel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DateEdit dteToDate;
        private DevExpress.XtraEditors.DateEdit dteFromDate;
        private DevExpress.XtraEditors.GridLookUpEdit cboBranch;
        private DevExpress.XtraGrid.Views.Grid.GridView cboBranchView;
        private DevExpress.XtraEditors.CheckEdit chkMaterial;
        private DevExpress.XtraEditors.CheckEdit chkMedicine;
        private DevExpress.XtraLayout.LayoutControlItem lciMedicine;
        private DevExpress.XtraLayout.LayoutControlItem lciMaterial;
        private DevExpress.XtraLayout.LayoutControlItem lciBranch;
        private DevExpress.XtraLayout.LayoutControlItem lciFromDate;
        private DevExpress.XtraLayout.LayoutControlItem lciToDate;
        private DevExpress.XtraGrid.GridControl grdData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewData;
        private Inventec.Desktop.CustomControl.NoFocus.CustomGridLookUpEditWithFilterMultiColumnNoFocus cboType;
        private Inventec.Desktop.CustomControl.NoFocus.CustomGridViewWithFilterMultiColumnNoFocus cboTypeView;
        private DevExpress.XtraEditors.SimpleButton btnExportExcel;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciSearch;
        private DevExpress.XtraLayout.LayoutControlItem lciExportExcel;
        private DevExpress.XtraLayout.LayoutControlItem lciType;
        private DevExpress.XtraLayout.LayoutControlItem lciGrid;
    }
}
