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
namespace HIS.Desktop.Plugins.ExamServiceAdd
{
    partial class FormExamServiceAdd
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.chkChangeDepartment = new DevExpress.XtraEditors.CheckEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barButtonItemSave = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemPrint = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.chkIsPrimary = new DevExpress.XtraEditors.CheckEdit();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtService = new DevExpress.XtraEditors.TextEdit();
            this.cboExamService = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cboExecuteRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtExecuteRoom = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExecuteRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciService = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChkIsPrimary = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciChangeDepartment = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkChangeDepartment.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsPrimary.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExamService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExecuteRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExecuteRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExecuteRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkIsPrimary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChangeDepartment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.chkChangeDepartment);
            this.layoutControl1.Controls.Add(this.chkIsPrimary);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtService);
            this.layoutControl1.Controls.Add(this.cboExamService);
            this.layoutControl1.Controls.Add(this.cboExecuteRoom);
            this.layoutControl1.Controls.Add(this.txtExecuteRoom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(550, 80);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // chkChangeDepartment
            // 
            this.chkChangeDepartment.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkChangeDepartment.EnterMoveNextControl = true;
            this.chkChangeDepartment.Location = new System.Drawing.Point(363, 50);
            this.chkChangeDepartment.MenuManager = this.barManager1;
            this.chkChangeDepartment.Name = "chkChangeDepartment";
            this.chkChangeDepartment.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Silver;
            this.chkChangeDepartment.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.chkChangeDepartment.Properties.Caption = "";
            this.chkChangeDepartment.Size = new System.Drawing.Size(34, 19);
            this.chkChangeDepartment.StyleController = this.layoutControl1;
            this.chkChangeDepartment.TabIndex = 11;
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
            this.barButtonItemSave,
            this.barButtonItemPrint});
            this.barManager1.MaxItemId = 2;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemPrint)});
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
            // barButtonItemPrint
            // 
            this.barButtonItemPrint.Caption = "Ctrl P";
            this.barButtonItemPrint.Id = 1;
            this.barButtonItemPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.barButtonItemPrint.Name = "barButtonItemPrint";
            this.barButtonItemPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemPrint_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(550, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 109);
            this.barDockControlBottom.Size = new System.Drawing.Size(550, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 80);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(550, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 80);
            // 
            // chkIsPrimary
            // 
            this.chkIsPrimary.EnterMoveNextControl = true;
            this.chkIsPrimary.Location = new System.Drawing.Point(97, 50);
            this.chkIsPrimary.MenuManager = this.barManager1;
            this.chkIsPrimary.Name = "chkIsPrimary";
            this.chkIsPrimary.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Silver;
            this.chkIsPrimary.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.chkIsPrimary.Properties.Caption = "";
            this.chkIsPrimary.Size = new System.Drawing.Size(34, 19);
            this.chkIsPrimary.StyleController = this.layoutControl1;
            this.chkIsPrimary.TabIndex = 10;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(434, 74);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(97, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 9;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(332, 74);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(98, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtService
            // 
            this.txtService.Location = new System.Drawing.Point(97, 26);
            this.txtService.Name = "txtService";
            this.txtService.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtService.Size = new System.Drawing.Size(68, 20);
            this.txtService.StyleController = this.layoutControl1;
            this.txtService.TabIndex = 7;
            this.txtService.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtService_PreviewKeyDown);
            // 
            // cboExamService
            // 
            this.cboExamService.Location = new System.Drawing.Point(165, 26);
            this.cboExamService.Name = "cboExamService";
            this.cboExamService.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboExamService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboExamService.Properties.NullText = "";
            this.cboExamService.Properties.View = this.gridLookUpEdit2View;
            this.cboExamService.Size = new System.Drawing.Size(366, 20);
            this.cboExamService.StyleController = this.layoutControl1;
            this.cboExamService.TabIndex = 6;
            this.cboExamService.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExamService_Closed);
            this.cboExamService.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.cboExamService_PreviewKeyDown);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // cboExecuteRoom
            // 
            this.cboExecuteRoom.Location = new System.Drawing.Point(165, 2);
            this.cboExecuteRoom.Name = "cboExecuteRoom";
            this.cboExecuteRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboExecuteRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboExecuteRoom.Properties.NullText = "";
            this.cboExecuteRoom.Properties.View = this.gridLookUpEdit1View;
            this.cboExecuteRoom.Size = new System.Drawing.Size(366, 20);
            this.cboExecuteRoom.StyleController = this.layoutControl1;
            this.cboExecuteRoom.TabIndex = 5;
            this.cboExecuteRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExecuteRoom_Closed);
            this.cboExecuteRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboExecuteRoom_KeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtExecuteRoom
            // 
            this.txtExecuteRoom.Location = new System.Drawing.Point(97, 2);
            this.txtExecuteRoom.Name = "txtExecuteRoom";
            this.txtExecuteRoom.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtExecuteRoom.Size = new System.Drawing.Size(68, 20);
            this.txtExecuteRoom.StyleController = this.layoutControl1;
            this.txtExecuteRoom.TabIndex = 4;
            this.txtExecuteRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExecuteRoom_KeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciExecuteRoom,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.lciService,
            this.emptySpaceItem1,
            this.layoutControlItem5,
            this.layoutControlItem1,
            this.lciChkIsPrimary,
            this.lciChangeDepartment,
            this.emptySpaceItem2,
            this.emptySpaceItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(533, 98);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciExecuteRoom
            // 
            this.lciExecuteRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Brown;
            this.lciExecuteRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciExecuteRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExecuteRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExecuteRoom.Control = this.txtExecuteRoom;
            this.lciExecuteRoom.Location = new System.Drawing.Point(0, 0);
            this.lciExecuteRoom.Name = "lciExecuteRoom";
            this.lciExecuteRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciExecuteRoom.Size = new System.Drawing.Size(165, 24);
            this.lciExecuteRoom.Text = "Phòng khám:";
            this.lciExecuteRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExecuteRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciExecuteRoom.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboExecuteRoom;
            this.layoutControlItem2.Location = new System.Drawing.Point(165, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(368, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboExamService;
            this.layoutControlItem3.Location = new System.Drawing.Point(165, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(368, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciService
            // 
            this.lciService.AppearanceItemCaption.ForeColor = System.Drawing.Color.Brown;
            this.lciService.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciService.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciService.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciService.Control = this.txtService;
            this.lciService.Location = new System.Drawing.Point(0, 24);
            this.lciService.Name = "lciService";
            this.lciService.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciService.Size = new System.Drawing.Size(165, 24);
            this.lciService.Text = "Dịch vụ khám:";
            this.lciService.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciService.TextSize = new System.Drawing.Size(90, 20);
            this.lciService.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 72);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(330, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnSave;
            this.layoutControlItem5.Location = new System.Drawing.Point(330, 72);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(102, 26);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnPrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(432, 72);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(101, 26);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciChkIsPrimary
            // 
            this.lciChkIsPrimary.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChkIsPrimary.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChkIsPrimary.Control = this.chkIsPrimary;
            this.lciChkIsPrimary.Location = new System.Drawing.Point(0, 48);
            this.lciChkIsPrimary.Name = "lciChkIsPrimary";
            this.lciChkIsPrimary.Size = new System.Drawing.Size(133, 24);
            this.lciChkIsPrimary.Text = "Khám chính:";
            this.lciChkIsPrimary.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChkIsPrimary.TextSize = new System.Drawing.Size(90, 20);
            this.lciChkIsPrimary.TextToControlDistance = 5;
            // 
            // lciChangeDepartment
            // 
            this.lciChangeDepartment.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciChangeDepartment.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciChangeDepartment.Control = this.chkChangeDepartment;
            this.lciChangeDepartment.Location = new System.Drawing.Point(266, 48);
            this.lciChangeDepartment.Name = "lciChangeDepartment";
            this.lciChangeDepartment.Size = new System.Drawing.Size(133, 24);
            this.lciChangeDepartment.Text = "Chuyển khoa:";
            this.lciChangeDepartment.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciChangeDepartment.TextSize = new System.Drawing.Size(90, 20);
            this.lciChangeDepartment.TextToControlDistance = 5;
            // 
            // dxValidationProvider
            // 
            this.dxValidationProvider.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider_ValidationFailed);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(133, 48);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(133, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(399, 48);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(134, 24);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // FormExamServiceAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 109);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormExamServiceAdd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Khám thêm";
            this.Load += new System.EventHandler(this.FormExamServiceAdd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkChangeDepartment.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIsPrimary.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExamService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExecuteRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExecuteRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExecuteRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChkIsPrimary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciChangeDepartment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit cboExecuteRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtExecuteRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciExecuteRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.TextEdit txtService;
        private DevExpress.XtraEditors.GridLookUpEdit cboExamService;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciService;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItemPrint;
        private DevExpress.XtraEditors.CheckEdit chkIsPrimary;
        private DevExpress.XtraLayout.LayoutControlItem lciChkIsPrimary;
        private DevExpress.XtraEditors.CheckEdit chkChangeDepartment;
        private DevExpress.XtraLayout.LayoutControlItem lciChangeDepartment;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
    }
}
