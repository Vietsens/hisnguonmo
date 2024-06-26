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
namespace HIS.Desktop.Plugins.BedAssign
{
    partial class FormBedAssign
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboBed = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridLookUpEditView_Bed = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboBedServiceType = new DevExpress.XtraEditors.LookUpEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.dtToTime = new DevExpress.XtraEditors.DateEdit();
            this.dtFromTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciBedServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciToTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBed = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEditView_Bed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboBed);
            this.layoutControl1.Controls.Add(this.cboBedServiceType);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.dtToTime);
            this.layoutControl1.Controls.Add(this.dtFromTime);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(440, 94);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboBed
            // 
            this.cboBed.Location = new System.Drawing.Point(117, 50);
            this.cboBed.MenuManager = this.barManager1;
            this.cboBed.Name = "cboBed";
            this.cboBed.Properties.AutoComplete = false;
            this.cboBed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboBed.Properties.NullText = "";
            this.cboBed.Properties.View = this.gridLookUpEditView_Bed;
            this.cboBed.Size = new System.Drawing.Size(304, 20);
            this.cboBed.StyleController = this.layoutControl1;
            this.cboBed.TabIndex = 11;
            this.cboBed.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBed_Closed);
            this.cboBed.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboBed_ButtonClick);
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
            this.barButtonItemSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barButtonItemSave
            // 
            this.barButtonItemSave.Caption = "Ctrl S";
            this.barButtonItemSave.Id = 0;
            this.barButtonItemSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barButtonItemSave.Name = "barButtonItemSave";
            this.barButtonItemSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemSave_ItemClick);
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
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 123);
            this.barDockControlBottom.Size = new System.Drawing.Size(440, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 94);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(440, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 94);
            // 
            // gridLookUpEditView_Bed
            // 
            this.gridLookUpEditView_Bed.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEditView_Bed.Name = "gridLookUpEditView_Bed";
            this.gridLookUpEditView_Bed.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEditView_Bed.OptionsView.ShowGroupPanel = false;
            this.gridLookUpEditView_Bed.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridLookUpEditView_Bed_RowStyle);
            // 
            // cboBedServiceType
            // 
            this.cboBedServiceType.EnterMoveNextControl = true;
            this.cboBedServiceType.Location = new System.Drawing.Point(117, 74);
            this.cboBedServiceType.Name = "cboBedServiceType";
            this.cboBedServiceType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboBedServiceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.cboBedServiceType.Properties.NullText = "";
            this.cboBedServiceType.Size = new System.Drawing.Size(304, 20);
            this.cboBedServiceType.StyleController = this.layoutControl1;
            this.cboBedServiceType.TabIndex = 10;
            this.cboBedServiceType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBedServiceType_Closed);
            this.cboBedServiceType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboBedServiceType_ButtonClick);
            this.cboBedServiceType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboBedServiceType_PreviewKeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(319, 98);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtToTime
            // 
            this.dtToTime.EditValue = null;
            this.dtToTime.EnterMoveNextControl = true;
            this.dtToTime.Location = new System.Drawing.Point(117, 26);
            this.dtToTime.Name = "dtToTime";
            this.dtToTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtToTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtToTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtToTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtToTime.Size = new System.Drawing.Size(304, 20);
            this.dtToTime.StyleController = this.layoutControl1;
            this.dtToTime.TabIndex = 7;
            this.dtToTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtToTime_Closed);
            // 
            // dtFromTime
            // 
            this.dtFromTime.EditValue = null;
            this.dtFromTime.EnterMoveNextControl = true;
            this.dtFromTime.Location = new System.Drawing.Point(117, 2);
            this.dtFromTime.Name = "dtFromTime";
            this.dtFromTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtFromTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtFromTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtFromTime.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.dtFromTime.Size = new System.Drawing.Size(304, 20);
            this.dtFromTime.StyleController = this.layoutControl1;
            this.dtFromTime.TabIndex = 6;
            this.dtFromTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtFromTime_Closed);
            this.dtFromTime.EditValueChanged += new System.EventHandler(this.dtFromTime_EditValueChanged);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem5,
            this.emptySpaceItem1,
            this.lciBedServiceType,
            this.lciFromTime,
            this.lciToTime,
            this.lciBed});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(423, 122);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnSave;
            this.layoutControlItem5.Location = new System.Drawing.Point(317, 96);
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
            this.emptySpaceItem1.Size = new System.Drawing.Size(317, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciBedServiceType
            // 
            this.lciBedServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBedServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBedServiceType.Control = this.cboBedServiceType;
            this.lciBedServiceType.Location = new System.Drawing.Point(0, 72);
            this.lciBedServiceType.Name = "lciBedServiceType";
            this.lciBedServiceType.Size = new System.Drawing.Size(423, 24);
            this.lciBedServiceType.Text = "Dịch vụ giường:";
            this.lciBedServiceType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBedServiceType.TextSize = new System.Drawing.Size(110, 20);
            this.lciBedServiceType.TextToControlDistance = 5;
            // 
            // lciFromTime
            // 
            this.lciFromTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciFromTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFromTime.Control = this.dtFromTime;
            this.lciFromTime.Location = new System.Drawing.Point(0, 0);
            this.lciFromTime.Name = "lciFromTime";
            this.lciFromTime.Size = new System.Drawing.Size(423, 24);
            this.lciFromTime.Text = "Từ:";
            this.lciFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFromTime.TextSize = new System.Drawing.Size(110, 20);
            this.lciFromTime.TextToControlDistance = 5;
            // 
            // lciToTime
            // 
            this.lciToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciToTime.Control = this.dtToTime;
            this.lciToTime.Location = new System.Drawing.Point(0, 24);
            this.lciToTime.Name = "lciToTime";
            this.lciToTime.Size = new System.Drawing.Size(423, 24);
            this.lciToTime.Text = "Đến:";
            this.lciToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciToTime.TextSize = new System.Drawing.Size(110, 20);
            this.lciToTime.TextToControlDistance = 5;
            // 
            // lciBed
            // 
            this.lciBed.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciBed.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciBed.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBed.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBed.Control = this.cboBed;
            this.lciBed.Location = new System.Drawing.Point(0, 48);
            this.lciBed.Name = "lciBed";
            this.lciBed.Size = new System.Drawing.Size(423, 24);
            this.lciBed.Text = "Giường: ";
            this.lciBed.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBed.TextSize = new System.Drawing.Size(110, 20);
            this.lciBed.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // FormBedAssign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 123);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormBedAssign";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gán giường";
            this.Load += new System.EventHandler(this.FormBedAssign_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboBed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEditView_Bed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboBedServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtToTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtFromTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBedServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciToTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.DateEdit dtToTime;
        private DevExpress.XtraEditors.DateEdit dtFromTime;
        private DevExpress.XtraLayout.LayoutControlItem lciFromTime;
        private DevExpress.XtraLayout.LayoutControlItem lciToTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.LookUpEdit cboBedServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciBedServiceType;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.GridLookUpEdit cboBed;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEditView_Bed;
        private DevExpress.XtraLayout.LayoutControlItem lciBed;
    }
}
