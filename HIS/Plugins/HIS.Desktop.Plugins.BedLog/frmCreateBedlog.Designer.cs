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
namespace HIS.Desktop.Plugins.BedLog
{
    partial class frmCreateBedlog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCreateBedlog));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboBed = new DevExpress.XtraEditors.LookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItem_Save = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonI_Refesh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btnRefesh = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave_Create = new DevExpress.XtraEditors.SimpleButton();
            this.dtToTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciToTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboBed = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.cboBedServiceType = new DevExpress.XtraEditors.LookUpEdit();
            this.lciBedServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.dtFromTime = new DevExpress.XtraEditors.DateEdit();
            this.lciFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboBed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.dtFromTime);
            this.layoutControl1.Controls.Add(this.cboBedServiceType);
            this.layoutControl1.Controls.Add(this.cboBed);
            this.layoutControl1.Controls.Add(this.btnRefesh);
            this.layoutControl1.Controls.Add(this.btnSave_Create);
            this.layoutControl1.Controls.Add(this.dtToTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(750, 133, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 109);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboBed
            // 
            this.cboBed.Location = new System.Drawing.Point(97, 2);
            this.cboBed.MenuManager = this.barManager1;
            this.cboBed.Name = "cboBed";
            this.cboBed.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboBed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBed.Properties.NullText = "";
            this.cboBed.Size = new System.Drawing.Size(324, 20);
            this.cboBed.StyleController = this.layoutControl1;
            this.cboBed.TabIndex = 13;
            this.cboBed.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBed_Closed);
            this.cboBed.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboBed_PreviewKeyDown);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barButtonItem_Save,
            this.barButtonI_Refesh});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem_Save),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonI_Refesh)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItem_Save
            // 
            this.barButtonItem_Save.Caption = "Lưu (Ctrl S)";
            this.barButtonItem_Save.Id = 0;
            this.barButtonItem_Save.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItem_Save.Name = "barButtonItem_Save";
            this.barButtonItem_Save.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem_Save_ItemClick);
            // 
            // barButtonI_Refesh
            // 
            this.barButtonI_Refesh.Caption = "Làm lại (Ctrl R)";
            this.barButtonI_Refesh.Id = 1;
            this.barButtonI_Refesh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.barButtonI_Refesh.Name = "barButtonI_Refesh";
            this.barButtonI_Refesh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonI_Refesh_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(440, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 138);
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 109);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 109);
            // 
            // btnRefesh
            // 
            this.btnRefesh.Location = new System.Drawing.Point(320, 108);
            this.btnRefesh.Name = "btnRefesh";
            this.btnRefesh.Size = new System.Drawing.Size(101, 22);
            this.btnRefesh.StyleController = this.layoutControl1;
            this.btnRefesh.TabIndex = 11;
            this.btnRefesh.Text = "Làm lại (Ctrl R)";
            this.btnRefesh.Click += new System.EventHandler(this.btnRefesh_Click);
            // 
            // btnSave_Create
            // 
            this.btnSave_Create.Location = new System.Drawing.Point(214, 108);
            this.btnSave_Create.Name = "btnSave_Create";
            this.btnSave_Create.Size = new System.Drawing.Size(102, 22);
            this.btnSave_Create.StyleController = this.layoutControl1;
            this.btnSave_Create.TabIndex = 8;
            this.btnSave_Create.Text = "Lưu (Ctrl S)";
            this.btnSave_Create.Click += new System.EventHandler(this.btnSave_Create_Click);
            // 
            // dtToTime
            // 
            this.dtToTime.EditValue = null;
            this.dtToTime.Location = new System.Drawing.Point(97, 74);
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:ss";
            this.dtToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:ss";
            this.dtToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:ss";
            this.dtToTime.Size = new System.Drawing.Size(324, 20);
            this.dtToTime.StyleController = this.layoutControl1;
            this.dtToTime.TabIndex = 7;
            this.dtToTime.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(this.dtToTime_CloseUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciToTime,
            this.layoutControlItem5,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutControlItem1,
            this.lciCboBed,
            this.lciBedServiceType,
            this.lciFromTime});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(423, 132);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciToTime
            // 
            this.lciToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciToTime.Control = this.dtToTime;
            this.lciToTime.Location = new System.Drawing.Point(0, 72);
            this.lciToTime.Name = "lciToTime";
            this.lciToTime.Size = new System.Drawing.Size(423, 24);
            this.lciToTime.Text = "Đến:";
            this.lciToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciToTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciToTime.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnSave_Create;
            this.layoutControlItem5.Location = new System.Drawing.Point(212, 106);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(106, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 96);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(423, 10);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 106);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(212, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnRefesh;
            this.layoutControlItem1.Location = new System.Drawing.Point(318, 106);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(105, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciCboBed
            // 
            this.lciCboBed.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCboBed.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCboBed.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboBed.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboBed.Control = this.cboBed;
            this.lciCboBed.Location = new System.Drawing.Point(0, 0);
            this.lciCboBed.Name = "lciCboBed";
            this.lciCboBed.Size = new System.Drawing.Size(423, 24);
            this.lciCboBed.Text = "Giường:";
            this.lciCboBed.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboBed.TextSize = new System.Drawing.Size(90, 20);
            this.lciCboBed.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // cboBedServiceType
            // 
            this.cboBedServiceType.Location = new System.Drawing.Point(97, 26);
            this.cboBedServiceType.MenuManager = this.barManager1;
            this.cboBedServiceType.Name = "cboBedServiceType";
            this.cboBedServiceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBedServiceType.Properties.NullText = "";
            this.cboBedServiceType.Size = new System.Drawing.Size(324, 20);
            this.cboBedServiceType.StyleController = this.layoutControl1;
            this.cboBedServiceType.TabIndex = 14;
            // 
            // lciBedServiceType
            // 
            this.lciBedServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedServiceType.Control = this.cboBedServiceType;
            this.lciBedServiceType.Location = new System.Drawing.Point(0, 24);
            this.lciBedServiceType.Name = "lciBedServiceType";
            this.lciBedServiceType.Size = new System.Drawing.Size(423, 24);
            this.lciBedServiceType.Text = "Dịch vụ giường:";
            this.lciBedServiceType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedServiceType.TextSize = new System.Drawing.Size(90, 20);
            this.lciBedServiceType.TextToControlDistance = 5;
            // 
            // dtFromTime
            // 
            this.dtFromTime.EditValue = null;
            this.dtFromTime.Location = new System.Drawing.Point(97, 50);
            this.dtFromTime.MenuManager = this.barManager1;
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Size = new System.Drawing.Size(324, 20);
            this.dtFromTime.StyleController = this.layoutControl1;
            this.dtFromTime.TabIndex = 15;
            // 
            // lciFromTime
            // 
            this.lciFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFromTime.Control = this.dtFromTime;
            this.lciFromTime.Location = new System.Drawing.Point(0, 48);
            this.lciFromTime.Name = "lciFromTime";
            this.lciFromTime.Size = new System.Drawing.Size(423, 24);
            this.lciFromTime.Text = "Từ:";
            this.lciFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFromTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciFromTime.TextToControlDistance = 5;
            // 
            // frmCreateBedlog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 138);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCreateBedlog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gán giường";
            this.Load += new System.EventHandler(this.frmBedLog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboBed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave_Create;
        private DevExpress.XtraEditors.DateEdit dtToTime;
        private DevExpress.XtraLayout.LayoutControlItem lciToTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem_Save;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SimpleButton btnRefesh;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonI_Refesh;
        private DevExpress.XtraEditors.LookUpEdit cboBed;
        private DevExpress.XtraLayout.LayoutControlItem lciCboBed;
        private DevExpress.XtraEditors.DateEdit dtFromTime;
        private DevExpress.XtraEditors.LookUpEdit cboBedServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciBedServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciFromTime;
    }
}
