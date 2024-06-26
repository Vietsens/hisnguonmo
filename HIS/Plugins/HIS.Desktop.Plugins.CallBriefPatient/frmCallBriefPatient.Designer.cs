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
namespace HIS.Desktop.Plugins.CallBriefPatient
{
    partial class frmCallBriefPatient
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
   System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCallBriefPatient));
   this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
   this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
   this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
   this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
   this.btnSave = new DevExpress.XtraEditors.SimpleButton();
   this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
   this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
   this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
   this.dtLogTime = new DevExpress.XtraEditors.DateEdit();
   this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
   this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
   this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
   this.barManager1 = new DevExpress.XtraBars.BarManager();
   this.bar2 = new DevExpress.XtraBars.Bar();
   this.barSave = new DevExpress.XtraBars.BarButtonItem();
   this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
   this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
   this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
   this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
   this.layoutControl1.SuspendLayout();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
   this.layoutControl2.SuspendLayout();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.dtLogTime.Properties.CalendarTimeProperties)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.dtLogTime.Properties)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
   ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
   this.SuspendLayout();
   // 
   // layoutControl1
   // 
   this.layoutControl1.Controls.Add(this.layoutControl2);
   this.layoutControl1.Controls.Add(this.dtLogTime);
   this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
   this.layoutControl1.Location = new System.Drawing.Point(0, 22);
   this.layoutControl1.Name = "layoutControl1";
   this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(477, 168, 250, 350);
   this.layoutControl1.Root = this.layoutControlGroup1;
   this.layoutControl1.Size = new System.Drawing.Size(259, 49);
   this.layoutControl1.TabIndex = 0;
   this.layoutControl1.Text = "layoutControl1";
   // 
   // layoutControl2
   // 
   this.layoutControl2.Controls.Add(this.layoutControl3);
   this.layoutControl2.Controls.Add(this.btnSave);
   this.layoutControl2.Location = new System.Drawing.Point(2, 26);
   this.layoutControl2.Name = "layoutControl2";
   this.layoutControl2.Root = this.Root;
   this.layoutControl2.Size = new System.Drawing.Size(238, 26);
   this.layoutControl2.TabIndex = 6;
   this.layoutControl2.Text = "layoutControl2";
   // 
   // layoutControl3
   // 
   this.layoutControl3.Location = new System.Drawing.Point(2, 2);
   this.layoutControl3.Name = "layoutControl3";
   this.layoutControl3.Root = this.layoutControlGroup2;
   this.layoutControl3.Size = new System.Drawing.Size(163, 22);
   this.layoutControl3.TabIndex = 6;
   this.layoutControl3.Text = "layoutControl3";
   // 
   // layoutControlGroup2
   // 
   this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
   this.layoutControlGroup2.GroupBordersVisible = false;
   this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
   this.layoutControlGroup2.Name = "layoutControlGroup2";
   this.layoutControlGroup2.Size = new System.Drawing.Size(163, 22);
   this.layoutControlGroup2.TextVisible = false;
   // 
   // btnSave
   // 
   this.btnSave.Location = new System.Drawing.Point(169, 2);
   this.btnSave.Name = "btnSave";
   this.btnSave.Size = new System.Drawing.Size(67, 22);
   this.btnSave.StyleController = this.layoutControl2;
   this.btnSave.TabIndex = 5;
   this.btnSave.Text = "Lưu(Ctrl S)";
   this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
   // 
   // Root
   // 
   this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
   this.Root.GroupBordersVisible = false;
   this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem4});
   this.Root.Location = new System.Drawing.Point(0, 0);
   this.Root.Name = "Root";
   this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
   this.Root.Size = new System.Drawing.Size(238, 26);
   this.Root.TextVisible = false;
   // 
   // layoutControlItem2
   // 
   this.layoutControlItem2.Control = this.btnSave;
   this.layoutControlItem2.Location = new System.Drawing.Point(167, 0);
   this.layoutControlItem2.Name = "layoutControlItem2";
   this.layoutControlItem2.Size = new System.Drawing.Size(71, 26);
   this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
   this.layoutControlItem2.TextVisible = false;
   // 
   // layoutControlItem4
   // 
   this.layoutControlItem4.Control = this.layoutControl3;
   this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
   this.layoutControlItem4.Name = "layoutControlItem4";
   this.layoutControlItem4.Size = new System.Drawing.Size(167, 26);
   this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
   this.layoutControlItem4.TextVisible = false;
   // 
   // dtLogTime
   // 
   this.dtLogTime.EditValue = null;
   this.dtLogTime.Location = new System.Drawing.Point(97, 2);
   this.dtLogTime.Name = "dtLogTime";
   this.dtLogTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
   this.dtLogTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
   this.dtLogTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
   this.dtLogTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
   this.dtLogTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
   this.dtLogTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
   this.dtLogTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
   this.dtLogTime.Size = new System.Drawing.Size(143, 20);
   this.dtLogTime.StyleController = this.layoutControl1;
   this.dtLogTime.TabIndex = 4;
   this.dtLogTime.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtLogTime_KeyDown_1);
   // 
   // layoutControlGroup1
   // 
   this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
   this.layoutControlGroup1.GroupBordersVisible = false;
   this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3});
   this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
   this.layoutControlGroup1.Name = "Root";
   this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
   this.layoutControlGroup1.Size = new System.Drawing.Size(242, 54);
   this.layoutControlGroup1.TextVisible = false;
   // 
   // layoutControlItem1
   // 
   this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
   this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
   this.layoutControlItem1.Control = this.dtLogTime;
   this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
   this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 24);
   this.layoutControlItem1.MinSize = new System.Drawing.Size(109, 24);
   this.layoutControlItem1.Name = "layoutControlItem1";
   this.layoutControlItem1.Size = new System.Drawing.Size(242, 24);
   this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
   this.layoutControlItem1.Text = "Thời gian:";
   this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
   this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 20);
   this.layoutControlItem1.TextToControlDistance = 5;
   // 
   // layoutControlItem3
   // 
   this.layoutControlItem3.Control = this.layoutControl2;
   this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
   this.layoutControlItem3.Name = "layoutControlItem3";
   this.layoutControlItem3.Size = new System.Drawing.Size(242, 30);
   this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
   this.layoutControlItem3.TextVisible = false;
   // 
   // barManager1
   // 
   this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
   this.barManager1.DockControls.Add(this.barDockControlTop);
   this.barManager1.DockControls.Add(this.barDockControlBottom);
   this.barManager1.DockControls.Add(this.barDockControlLeft);
   this.barManager1.DockControls.Add(this.barDockControlRight);
   this.barManager1.Form = this;
   this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barSave});
   this.barManager1.MainMenu = this.bar2;
   this.barManager1.MaxItemId = 1;
   // 
   // bar2
   // 
   this.bar2.BarName = "Main menu";
   this.bar2.DockCol = 0;
   this.bar2.DockRow = 0;
   this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
   this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSave)});
   this.bar2.OptionsBar.MultiLine = true;
   this.bar2.OptionsBar.UseWholeRow = true;
   this.bar2.Text = "Main menu";
   this.bar2.Visible = false;
   // 
   // barSave
   // 
   this.barSave.Caption = "CtrlS";
   this.barSave.Id = 0;
   this.barSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
   this.barSave.Name = "barSave";
   this.barSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSave_ItemClick);
   // 
   // barDockControlTop
   // 
   this.barDockControlTop.CausesValidation = false;
   this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
   this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
   this.barDockControlTop.Size = new System.Drawing.Size(259, 22);
   // 
   // barDockControlBottom
   // 
   this.barDockControlBottom.CausesValidation = false;
   this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
   this.barDockControlBottom.Location = new System.Drawing.Point(0, 71);
   this.barDockControlBottom.Size = new System.Drawing.Size(259, 0);
   // 
   // barDockControlLeft
   // 
   this.barDockControlLeft.CausesValidation = false;
   this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
   this.barDockControlLeft.Location = new System.Drawing.Point(0, 22);
   this.barDockControlLeft.Size = new System.Drawing.Size(0, 49);
   // 
   // barDockControlRight
   // 
   this.barDockControlRight.CausesValidation = false;
   this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
   this.barDockControlRight.Location = new System.Drawing.Point(259, 22);
   this.barDockControlRight.Size = new System.Drawing.Size(0, 49);
   // 
   // frmCallBriefPatient
   // 
   this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
   this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
   this.ClientSize = new System.Drawing.Size(259, 71);
   this.Controls.Add(this.layoutControl1);
   this.Controls.Add(this.barDockControlLeft);
   this.Controls.Add(this.barDockControlRight);
   this.Controls.Add(this.barDockControlBottom);
   this.Controls.Add(this.barDockControlTop);
   this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
   this.Name = "frmCallBriefPatient";
   this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
   this.Text = "Tách bệnh án";
   this.Load += new System.EventHandler(this.frmCallBriefPatient_Load);
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
   this.layoutControl1.ResumeLayout(false);
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
   this.layoutControl2.ResumeLayout(false);
   ((System.ComponentModel.ISupportInitialize)(this.layoutControl3)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.dtLogTime.Properties.CalendarTimeProperties)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.dtLogTime.Properties)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
   ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
   this.ResumeLayout(false);
   this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.DateEdit dtLogTime;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem barSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControl layoutControl3;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;

    }
}
