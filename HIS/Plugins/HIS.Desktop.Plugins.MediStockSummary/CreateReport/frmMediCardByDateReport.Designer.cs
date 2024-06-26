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
namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport
{
    partial class frmMediCardByDateReport
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
            this.lblExpiredDate = new DevExpress.XtraEditors.LabelControl();
            this.lblPackageNumber = new DevExpress.XtraEditors.LabelControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.dtTimeTo = new DevExpress.XtraEditors.DateEdit();
            this.dtTimeFrom = new DevExpress.XtraEditors.DateEdit();
            this.lblReportTypeName = new DevExpress.XtraEditors.LabelControl();
            this.lblReportTypeCode = new DevExpress.XtraEditors.LabelControl();
            this.lblMediStockName = new DevExpress.XtraEditors.LabelControl();
            this.lblMediStockCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediMateCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMediMateName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciPackageNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExpiredDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem__Save = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMateCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMateName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPackageNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblExpiredDate);
            this.layoutControl1.Controls.Add(this.lblPackageNumber);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.dtTimeTo);
            this.layoutControl1.Controls.Add(this.dtTimeFrom);
            this.layoutControl1.Controls.Add(this.lblReportTypeName);
            this.layoutControl1.Controls.Add(this.lblReportTypeCode);
            this.layoutControl1.Controls.Add(this.lblMediStockName);
            this.layoutControl1.Controls.Add(this.lblMediStockCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 38);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(191, 159, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(756, 115);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblExpiredDate
            // 
            this.lblExpiredDate.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblExpiredDate.Location = new System.Drawing.Point(392, 55);
            this.lblExpiredDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblExpiredDate.Name = "lblExpiredDate";
            this.lblExpiredDate.Size = new System.Drawing.Size(340, 20);
            this.lblExpiredDate.StyleController = this.layoutControl1;
            this.lblExpiredDate.TabIndex = 17;
            // 
            // lblPackageNumber
            // 
            this.lblPackageNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPackageNumber.Location = new System.Drawing.Point(98, 55);
            this.lblPackageNumber.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblPackageNumber.Name = "lblPackageNumber";
            this.lblPackageNumber.Size = new System.Drawing.Size(193, 20);
            this.lblPackageNumber.StyleController = this.layoutControl1;
            this.lblPackageNumber.TabIndex = 16;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(615, 105);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(117, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtTimeTo
            // 
            this.dtTimeTo.EditValue = null;
            this.dtTimeTo.Location = new System.Drawing.Point(392, 81);
            this.dtTimeTo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtTimeTo.Name = "dtTimeTo";
            this.dtTimeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeTo.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeTo.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeTo.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeTo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeTo.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTimeTo.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTimeTo.Size = new System.Drawing.Size(193, 22);
            this.dtTimeTo.StyleController = this.layoutControl1;
            this.dtTimeTo.TabIndex = 13;
            this.dtTimeTo.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeTo_Closed);
            // 
            // dtTimeFrom
            // 
            this.dtTimeFrom.EditValue = null;
            this.dtTimeFrom.Location = new System.Drawing.Point(98, 81);
            this.dtTimeFrom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtTimeFrom.Name = "dtTimeFrom";
            this.dtTimeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtTimeFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtTimeFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtTimeFrom.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtTimeFrom.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtTimeFrom.Size = new System.Drawing.Size(193, 22);
            this.dtTimeFrom.StyleController = this.layoutControl1;
            this.dtTimeFrom.TabIndex = 12;
            this.dtTimeFrom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtTimeFrom_Closed);
            // 
            // lblReportTypeName
            // 
            this.lblReportTypeName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblReportTypeName.Location = new System.Drawing.Point(392, 29);
            this.lblReportTypeName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblReportTypeName.Name = "lblReportTypeName";
            this.lblReportTypeName.Size = new System.Drawing.Size(340, 20);
            this.lblReportTypeName.StyleController = this.layoutControl1;
            this.lblReportTypeName.TabIndex = 8;
            // 
            // lblReportTypeCode
            // 
            this.lblReportTypeCode.Location = new System.Drawing.Point(98, 29);
            this.lblReportTypeCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblReportTypeCode.Name = "lblReportTypeCode";
            this.lblReportTypeCode.Size = new System.Drawing.Size(193, 18);
            this.lblReportTypeCode.StyleController = this.layoutControl1;
            this.lblReportTypeCode.TabIndex = 7;
            // 
            // lblMediStockName
            // 
            this.lblMediStockName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediStockName.Location = new System.Drawing.Point(392, 3);
            this.lblMediStockName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblMediStockName.Name = "lblMediStockName";
            this.lblMediStockName.Size = new System.Drawing.Size(340, 20);
            this.lblMediStockName.StyleController = this.layoutControl1;
            this.lblMediStockName.TabIndex = 5;
            // 
            // lblMediStockCode
            // 
            this.lblMediStockCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMediStockCode.Location = new System.Drawing.Point(98, 3);
            this.lblMediStockCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblMediStockCode.Name = "lblMediStockCode";
            this.lblMediStockCode.Size = new System.Drawing.Size(193, 18);
            this.lblMediStockCode.StyleController = this.layoutControl1;
            this.lblMediStockCode.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.lciMediMateCode,
            this.lciMediMateName,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.lciPackageNumber,
            this.lciExpiredDate});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(735, 135);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.lblMediStockCode;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(220, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(294, 26);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.Text = "Mã kho:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.lblMediStockName;
            this.layoutControlItem2.Location = new System.Drawing.Point(294, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(441, 26);
            this.layoutControlItem2.Text = "Tên kho:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // lciMediMateCode
            // 
            this.lciMediMateCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediMateCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediMateCode.Control = this.lblReportTypeCode;
            this.lciMediMateCode.Location = new System.Drawing.Point(0, 26);
            this.lciMediMateCode.MaxSize = new System.Drawing.Size(0, 24);
            this.lciMediMateCode.MinSize = new System.Drawing.Size(220, 24);
            this.lciMediMateCode.Name = "lciMediMateCode";
            this.lciMediMateCode.Size = new System.Drawing.Size(294, 26);
            this.lciMediMateCode.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciMediMateCode.Text = "Mã loại thuốc:";
            this.lciMediMateCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediMateCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediMateCode.TextToControlDistance = 5;
            // 
            // lciMediMateName
            // 
            this.lciMediMateName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMediMateName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMediMateName.Control = this.lblReportTypeName;
            this.lciMediMateName.Location = new System.Drawing.Point(294, 26);
            this.lciMediMateName.Name = "lciMediMateName";
            this.lciMediMateName.Size = new System.Drawing.Size(441, 26);
            this.lciMediMateName.Text = "Tên loại thuốc:";
            this.lciMediMateName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMediMateName.TextSize = new System.Drawing.Size(90, 20);
            this.lciMediMateName.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem9.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem9.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem9.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem9.Control = this.dtTimeFrom;
            this.layoutControlItem9.Location = new System.Drawing.Point(0, 78);
            this.layoutControlItem9.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem9.MinSize = new System.Drawing.Size(220, 24);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem9.Text = "Từ:";
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem9.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutControlItem10.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.dtTimeTo;
            this.layoutControlItem10.Location = new System.Drawing.Point(294, 78);
            this.layoutControlItem10.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem10.MinSize = new System.Drawing.Size(220, 24);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem10.Text = "Đến:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnSave;
            this.layoutControlItem3.Location = new System.Drawing.Point(612, 102);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(123, 33);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 102);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(612, 33);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(588, 78);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(147, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciPackageNumber
            // 
            this.lciPackageNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPackageNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPackageNumber.Control = this.lblPackageNumber;
            this.lciPackageNumber.Location = new System.Drawing.Point(0, 52);
            this.lciPackageNumber.Name = "lciPackageNumber";
            this.lciPackageNumber.Size = new System.Drawing.Size(294, 26);
            this.lciPackageNumber.Text = "Số lô:";
            this.lciPackageNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPackageNumber.TextSize = new System.Drawing.Size(90, 20);
            this.lciPackageNumber.TextToControlDistance = 5;
            this.lciPackageNumber.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // lciExpiredDate
            // 
            this.lciExpiredDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExpiredDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExpiredDate.Control = this.lblExpiredDate;
            this.lciExpiredDate.Location = new System.Drawing.Point(294, 52);
            this.lciExpiredDate.Name = "lciExpiredDate";
            this.lciExpiredDate.Size = new System.Drawing.Size(441, 26);
            this.lciExpiredDate.Text = "Hạn dùng:";
            this.lciExpiredDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExpiredDate.TextSize = new System.Drawing.Size(90, 20);
            this.lciExpiredDate.TextToControlDistance = 5;
            this.lciExpiredDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem__Save});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem__Save)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem__Save
            // 
            this.barButtonItem__Save.Caption = "Lưu (Ctrl S)";
            this.barButtonItem__Save.Id = 0;
            this.barButtonItem__Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem__Save.Name = "barButtonItem__Save";
            this.barButtonItem__Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem__Save_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 38);
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(756, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 153);
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(756, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 115);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(756, 38);
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 115);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmMediCardByDateReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 153);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmMediCardByDateReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMediCardByDateReport";
            this.Load += new System.EventHandler(this.frmMediCardByDateReport_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeTo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtTimeFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMateCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMediMateName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPackageNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExpiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DateEdit dtTimeTo;
        private DevExpress.XtraEditors.DateEdit dtTimeFrom;
        private DevExpress.XtraEditors.LabelControl lblReportTypeName;
        private DevExpress.XtraEditors.LabelControl lblReportTypeCode;
        private DevExpress.XtraEditors.LabelControl lblMediStockName;
        private DevExpress.XtraEditors.LabelControl lblMediStockCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem lciMediMateCode;
        private DevExpress.XtraLayout.LayoutControlItem lciMediMateName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem__Save;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.LabelControl lblPackageNumber;
        private DevExpress.XtraLayout.LayoutControlItem lciPackageNumber;
        private DevExpress.XtraEditors.LabelControl lblExpiredDate;
        private DevExpress.XtraLayout.LayoutControlItem lciExpiredDate;
    }
}
