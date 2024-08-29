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
namespace HIS.Desktop.Plugins.AssignPaan
{
    partial class frmAssignPaan
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
            this.cboTracking = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSavePrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.panelControlSubIcd = new DevExpress.XtraEditors.PanelControl();
            this.cboUsername = new DevExpress.XtraEditors.LookUpEdit();
            this.txtLoginname = new DevExpress.XtraEditors.TextEdit();
            this.btnRefersh = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnSavePrint = new DevExpress.XtraEditors.SimpleButton();
            this.txtDescription = new DevExpress.XtraEditors.MemoEdit();
            this.dtLiquidTime = new DevExpress.XtraEditors.DateEdit();
            this.cboPaanLiquid = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPaanLiquidCode = new DevExpress.XtraEditors.TextEdit();
            this.cboPaanPosition = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPaanPositionCode = new DevExpress.XtraEditors.TextEdit();
            this.spinPrice = new DevExpress.XtraEditors.SpinEdit();
            this.cboExecuteRoom = new DevExpress.XtraEditors.LookUpEdit();
            this.txtPaanServiceTypeCode = new DevExpress.XtraEditors.TextEdit();
            this.checkIsSurgery = new DevExpress.XtraEditors.CheckEdit();
            this.cboPatientType = new DevExpress.XtraEditors.LookUpEdit();
            this.dtInstructionTime = new DevExpress.XtraEditors.DateEdit();
            this.panelControlUcIcd = new DevExpress.XtraEditors.PanelControl();
            this.cboPaanServiceType = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridView1 = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.cboTestSampleType = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.licIcd = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciInstructionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIsSurgery = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaanServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboPaanServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaanPosition = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboPaanPosition = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPaanLiquid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboPaanLiquid = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciLoginname = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboExecuteRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTracking = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciLiquidDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSubIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginname.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLiquidTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLiquidTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanLiquid.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanLiquidCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanPosition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanPositionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinPrice.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExecuteRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanServiceTypeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkIsSurgery.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlUcIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTestSampleType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.licIcd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsSurgery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanLiquid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanLiquid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLoginname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboExecuteRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLiquidDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboTracking);
            this.layoutControl1.Controls.Add(this.panelControlSubIcd);
            this.layoutControl1.Controls.Add(this.cboUsername);
            this.layoutControl1.Controls.Add(this.txtLoginname);
            this.layoutControl1.Controls.Add(this.btnRefersh);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.btnSavePrint);
            this.layoutControl1.Controls.Add(this.txtDescription);
            this.layoutControl1.Controls.Add(this.dtLiquidTime);
            this.layoutControl1.Controls.Add(this.cboPaanLiquid);
            this.layoutControl1.Controls.Add(this.txtPaanLiquidCode);
            this.layoutControl1.Controls.Add(this.cboPaanPosition);
            this.layoutControl1.Controls.Add(this.txtPaanPositionCode);
            this.layoutControl1.Controls.Add(this.spinPrice);
            this.layoutControl1.Controls.Add(this.cboExecuteRoom);
            this.layoutControl1.Controls.Add(this.txtPaanServiceTypeCode);
            this.layoutControl1.Controls.Add(this.checkIsSurgery);
            this.layoutControl1.Controls.Add(this.cboPatientType);
            this.layoutControl1.Controls.Add(this.dtInstructionTime);
            this.layoutControl1.Controls.Add(this.panelControlUcIcd);
            this.layoutControl1.Controls.Add(this.cboPaanServiceType);
            this.layoutControl1.Controls.Add(this.cboTestSampleType);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1022, 203);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboTracking
            // 
            this.cboTracking.Location = new System.Drawing.Point(846, 76);
            this.cboTracking.MenuManager = this.barManager1;
            this.cboTracking.Name = "cboTracking";
            this.cboTracking.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTracking.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.cboTracking.Properties.NullText = "";
            this.cboTracking.Properties.View = this.gridLookUpEdit1View;
            this.cboTracking.Size = new System.Drawing.Size(116, 20);
            this.cboTracking.StyleController = this.layoutControl1;
            this.cboTracking.TabIndex = 27;
            this.cboTracking.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboTracking_Closed);
            this.cboTracking.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTracking_ButtonClick);
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
            this.bbtnRCSavePrint,
            this.bbtnRCSave,
            this.bbtnPrint,
            this.bbtnRCRefresh});
            this.barManager1.MaxItemId = 4;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSavePrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCRefresh)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCSavePrint
            // 
            this.bbtnRCSavePrint.Caption = "Lưu in (Ctrl I)";
            this.bbtnRCSavePrint.Id = 0;
            this.bbtnRCSavePrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
            this.bbtnRCSavePrint.Name = "bbtnRCSavePrint";
            this.bbtnRCSavePrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCSavePrint_ItemClick);
            // 
            // bbtnRCSave
            // 
            this.bbtnRCSave.Caption = "Lưu (Ctrl S)";
            this.bbtnRCSave.Id = 1;
            this.bbtnRCSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnRCSave.Name = "bbtnRCSave";
            this.bbtnRCSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCSave_ItemClick);
            // 
            // bbtnPrint
            // 
            this.bbtnPrint.Caption = "In (Ctrl P)";
            this.bbtnPrint.Id = 2;
            this.bbtnPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnPrint.Name = "bbtnPrint";
            this.bbtnPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnPrint_ItemClick);
            // 
            // bbtnRCRefresh
            // 
            this.bbtnRCRefresh.Caption = "Làm lại (Ctrl R)";
            this.bbtnRCRefresh.Id = 3;
            this.bbtnRCRefresh.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R));
            this.bbtnRCRefresh.Name = "bbtnRCRefresh";
            this.bbtnRCRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCRefresh_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1022, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 232);
            this.barDockControlBottom.Size = new System.Drawing.Size(1022, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 203);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1022, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 203);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // panelControlSubIcd
            // 
            this.panelControlSubIcd.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlSubIcd.Location = new System.Drawing.Point(409, 0);
            this.panelControlSubIcd.Margin = new System.Windows.Forms.Padding(2);
            this.panelControlSubIcd.Name = "panelControlSubIcd";
            this.panelControlSubIcd.Size = new System.Drawing.Size(613, 26);
            this.panelControlSubIcd.TabIndex = 26;
            // 
            // cboUsername
            // 
            this.cboUsername.Location = new System.Drawing.Point(650, 28);
            this.cboUsername.MenuManager = this.barManager1;
            this.cboUsername.Name = "cboUsername";
            this.cboUsername.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboUsername.Properties.NullText = "";
            this.cboUsername.Size = new System.Drawing.Size(370, 20);
            this.cboUsername.StyleController = this.layoutControl1;
            this.cboUsername.TabIndex = 11;
            this.cboUsername.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboUsername_Closed);
            this.cboUsername.EditValueChanged += new System.EventHandler(this.cboUsername_EditValueChanged);
            this.cboUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboUsername_KeyUp);
            // 
            // txtLoginname
            // 
            this.txtLoginname.Location = new System.Drawing.Point(506, 28);
            this.txtLoginname.MenuManager = this.barManager1;
            this.txtLoginname.Name = "txtLoginname";
            this.txtLoginname.Size = new System.Drawing.Size(144, 20);
            this.txtLoginname.StyleController = this.layoutControl1;
            this.txtLoginname.TabIndex = 10;
            this.txtLoginname.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLoginname_PreviewKeyDown);
            // 
            // btnRefersh
            // 
            this.btnRefersh.Location = new System.Drawing.Point(913, 175);
            this.btnRefersh.Name = "btnRefersh";
            this.btnRefersh.Size = new System.Drawing.Size(107, 26);
            this.btnRefersh.StyleController = this.layoutControl1;
            this.btnRefersh.TabIndex = 25;
            this.btnRefersh.Text = "Làm lại (Ctrl R)";
            this.btnRefersh.Click += new System.EventHandler(this.btnRefersh_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(801, 175);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(108, 26);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 24;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(690, 175);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(107, 26);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 23;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSavePrint
            // 
            this.btnSavePrint.Location = new System.Drawing.Point(578, 175);
            this.btnSavePrint.Name = "btnSavePrint";
            this.btnSavePrint.Size = new System.Drawing.Size(108, 26);
            this.btnSavePrint.StyleController = this.layoutControl1;
            this.btnSavePrint.TabIndex = 22;
            this.btnSavePrint.Text = "Lưu in (Ctrl I)";
            this.btnSavePrint.Click += new System.EventHandler(this.btnSavePrint_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(97, 124);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Properties.MaxLength = 1000;
            this.txtDescription.Size = new System.Drawing.Size(923, 47);
            this.txtDescription.StyleController = this.layoutControl1;
            this.txtDescription.TabIndex = 21;
            this.txtDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDescription_PreviewKeyDown);
            // 
            // dtLiquidTime
            // 
            this.dtLiquidTime.EditValue = null;
            this.dtLiquidTime.Location = new System.Drawing.Point(846, 100);
            this.dtLiquidTime.Name = "dtLiquidTime";
            this.dtLiquidTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLiquidTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtLiquidTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtLiquidTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtLiquidTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtLiquidTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtLiquidTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtLiquidTime.Size = new System.Drawing.Size(174, 20);
            this.dtLiquidTime.StyleController = this.layoutControl1;
            this.dtLiquidTime.TabIndex = 20;
            this.dtLiquidTime.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtLiquidTime_Closed);
            this.dtLiquidTime.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dtLiquidTime_PreviewKeyDown);
            // 
            // cboPaanLiquid
            // 
            this.cboPaanLiquid.Location = new System.Drawing.Point(588, 100);
            this.cboPaanLiquid.Name = "cboPaanLiquid";
            this.cboPaanLiquid.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPaanLiquid.Properties.NullText = "";
            this.cboPaanLiquid.Size = new System.Drawing.Size(159, 20);
            this.cboPaanLiquid.StyleController = this.layoutControl1;
            this.cboPaanLiquid.TabIndex = 19;
            this.cboPaanLiquid.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPaanLiquid_Closed);
            this.cboPaanLiquid.EditValueChanged += new System.EventHandler(this.cboPaanLiquid_EditValueChanged);
            // 
            // txtPaanLiquidCode
            // 
            this.txtPaanLiquidCode.Location = new System.Drawing.Point(506, 100);
            this.txtPaanLiquidCode.Name = "txtPaanLiquidCode";
            this.txtPaanLiquidCode.Size = new System.Drawing.Size(82, 20);
            this.txtPaanLiquidCode.StyleController = this.layoutControl1;
            this.txtPaanLiquidCode.TabIndex = 18;
            this.txtPaanLiquidCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPaanLiquidCode_PreviewKeyDown);
            // 
            // cboPaanPosition
            // 
            this.cboPaanPosition.Location = new System.Drawing.Point(169, 100);
            this.cboPaanPosition.Name = "cboPaanPosition";
            this.cboPaanPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPaanPosition.Properties.NullText = "";
            this.cboPaanPosition.Size = new System.Drawing.Size(238, 20);
            this.cboPaanPosition.StyleController = this.layoutControl1;
            this.cboPaanPosition.TabIndex = 17;
            this.cboPaanPosition.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPaanPosition_Closed);
            this.cboPaanPosition.EditValueChanged += new System.EventHandler(this.cboPaanPosition_EditValueChanged);
            // 
            // txtPaanPositionCode
            // 
            this.txtPaanPositionCode.Location = new System.Drawing.Point(97, 100);
            this.txtPaanPositionCode.Name = "txtPaanPositionCode";
            this.txtPaanPositionCode.Size = new System.Drawing.Size(72, 20);
            this.txtPaanPositionCode.StyleController = this.layoutControl1;
            this.txtPaanPositionCode.TabIndex = 16;
            this.txtPaanPositionCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPaanPositionCode_PreviewKeyDown);
            // 
            // spinPrice
            // 
            this.spinPrice.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinPrice.Enabled = false;
            this.spinPrice.Location = new System.Drawing.Point(846, 52);
            this.spinPrice.Name = "spinPrice";
            this.spinPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinPrice.Properties.DisplayFormat.FormatString = "#,##0.0000";
            this.spinPrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinPrice.Properties.EditFormat.FormatString = "#,##0.0000";
            this.spinPrice.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.spinPrice.Properties.Mask.EditMask = "#,##0.0000";
            this.spinPrice.Size = new System.Drawing.Size(174, 20);
            this.spinPrice.StyleController = this.layoutControl1;
            this.spinPrice.TabIndex = 15;
            // 
            // cboExecuteRoom
            // 
            this.cboExecuteRoom.Location = new System.Drawing.Point(97, 76);
            this.cboExecuteRoom.Name = "cboExecuteRoom";
            this.cboExecuteRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboExecuteRoom.Properties.NullText = "";
            this.cboExecuteRoom.Size = new System.Drawing.Size(310, 20);
            this.cboExecuteRoom.StyleController = this.layoutControl1;
            this.cboExecuteRoom.TabIndex = 14;
            this.cboExecuteRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExecuteRoom_Closed);
            this.cboExecuteRoom.EditValueChanged += new System.EventHandler(this.cboExecuteRoom_EditValueChanged);
            // 
            // txtPaanServiceTypeCode
            // 
            this.txtPaanServiceTypeCode.Location = new System.Drawing.Point(97, 52);
            this.txtPaanServiceTypeCode.Name = "txtPaanServiceTypeCode";
            this.txtPaanServiceTypeCode.Size = new System.Drawing.Size(72, 20);
            this.txtPaanServiceTypeCode.StyleController = this.layoutControl1;
            this.txtPaanServiceTypeCode.TabIndex = 12;
            this.txtPaanServiceTypeCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPaanServiceTypeCode_PreviewKeyDown);
            // 
            // checkIsSurgery
            // 
            this.checkIsSurgery.Location = new System.Drawing.Point(381, 28);
            this.checkIsSurgery.Name = "checkIsSurgery";
            this.checkIsSurgery.Properties.Caption = "";
            this.checkIsSurgery.Size = new System.Drawing.Size(26, 19);
            this.checkIsSurgery.StyleController = this.layoutControl1;
            this.checkIsSurgery.TabIndex = 9;
            this.checkIsSurgery.CheckedChanged += new System.EventHandler(this.checkIsSurgery_CheckedChanged);
            this.checkIsSurgery.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.checkIsSurgery_PreviewKeyDown);
            // 
            // cboPatientType
            // 
            this.cboPatientType.Location = new System.Drawing.Point(506, 52);
            this.cboPatientType.Name = "cboPatientType";
            this.cboPatientType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPatientType.Properties.NullText = "";
            this.cboPatientType.Size = new System.Drawing.Size(241, 20);
            this.cboPatientType.StyleController = this.layoutControl1;
            this.cboPatientType.TabIndex = 8;
            this.cboPatientType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
            this.cboPatientType.EditValueChanged += new System.EventHandler(this.cboPatientType_EditValueChanged);
            // 
            // dtInstructionTime
            // 
            this.dtInstructionTime.EditValue = null;
            this.dtInstructionTime.Location = new System.Drawing.Point(97, 28);
            this.dtInstructionTime.Name = "dtInstructionTime";
            this.dtInstructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtInstructionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtInstructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtInstructionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtInstructionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtInstructionTime.Size = new System.Drawing.Size(185, 20);
            this.dtInstructionTime.StyleController = this.layoutControl1;
            this.dtInstructionTime.TabIndex = 7;
            this.dtInstructionTime.EditValueChanged += new System.EventHandler(this.dtInstructionTime_EditValueChanged);
            // 
            // panelControlUcIcd
            // 
            this.panelControlUcIcd.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlUcIcd.Location = new System.Drawing.Point(0, 0);
            this.panelControlUcIcd.Name = "panelControlUcIcd";
            this.panelControlUcIcd.Size = new System.Drawing.Size(409, 26);
            this.panelControlUcIcd.TabIndex = 4;
            // 
            // cboPaanServiceType
            // 
            this.cboPaanServiceType.Location = new System.Drawing.Point(169, 52);
            this.cboPaanServiceType.Name = "cboPaanServiceType";
            this.cboPaanServiceType.Properties.AutoComplete = false;
            this.cboPaanServiceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboPaanServiceType.Properties.NullText = "";
            this.cboPaanServiceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboPaanServiceType.Properties.View = this.gridView1;
            this.cboPaanServiceType.Size = new System.Drawing.Size(238, 20);
            this.cboPaanServiceType.StyleController = this.layoutControl1;
            this.cboPaanServiceType.TabIndex = 13;
            this.cboPaanServiceType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPanServiceType_Closed);
            this.cboPaanServiceType.EditValueChanged += new System.EventHandler(this.cboPanServiceType_EditValueChanged);
            this.cboPaanServiceType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboPaanServiceType_KeyUp);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // cboTestSampleType
            // 
            this.cboTestSampleType.Location = new System.Drawing.Point(506, 76);
            this.cboTestSampleType.MenuManager = this.barManager1;
            this.cboTestSampleType.Name = "cboTestSampleType";
            this.cboTestSampleType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboTestSampleType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboTestSampleType.Properties.NullText = "";
            this.cboTestSampleType.Size = new System.Drawing.Size(241, 20);
            this.cboTestSampleType.StyleController = this.layoutControl1;
            this.cboTestSampleType.TabIndex = 28;
            this.cboTestSampleType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboTestSampleType_ButtonClick);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.licIcd,
            this.lciInstructionTime,
            this.lciIsSurgery,
            this.lciPaanServiceType,
            this.lciCboPaanServiceType,
            this.lciPrice,
            this.lciPaanPosition,
            this.lciCboPaanPosition,
            this.lciPaanLiquid,
            this.lciCboPaanLiquid,
            this.lciDescription,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem2,
            this.lciLoginname,
            this.layoutControlItem7,
            this.layoutControlItem6,
            this.lciCboExecuteRoom,
            this.lciTracking,
            this.lciPatientType,
            this.lciSampleType,
            this.lciLiquidDate,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1022, 203);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // licIcd
            // 
            this.licIcd.Control = this.panelControlUcIcd;
            this.licIcd.Location = new System.Drawing.Point(0, 0);
            this.licIcd.Name = "licIcd";
            this.licIcd.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.licIcd.Size = new System.Drawing.Size(409, 26);
            this.licIcd.TextSize = new System.Drawing.Size(0, 0);
            this.licIcd.TextVisible = false;
            // 
            // lciInstructionTime
            // 
            this.lciInstructionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciInstructionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciInstructionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciInstructionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciInstructionTime.Control = this.dtInstructionTime;
            this.lciInstructionTime.Location = new System.Drawing.Point(0, 26);
            this.lciInstructionTime.Name = "lciInstructionTime";
            this.lciInstructionTime.Size = new System.Drawing.Size(284, 24);
            this.lciInstructionTime.Text = "Thời gian chỉ định:";
            this.lciInstructionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciInstructionTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciInstructionTime.TextToControlDistance = 5;
            // 
            // lciIsSurgery
            // 
            this.lciIsSurgery.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIsSurgery.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIsSurgery.Control = this.checkIsSurgery;
            this.lciIsSurgery.Location = new System.Drawing.Point(284, 26);
            this.lciIsSurgery.Name = "lciIsSurgery";
            this.lciIsSurgery.Size = new System.Drawing.Size(125, 24);
            this.lciIsSurgery.Text = "Cấp cứu:";
            this.lciIsSurgery.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIsSurgery.TextSize = new System.Drawing.Size(90, 20);
            this.lciIsSurgery.TextToControlDistance = 5;
            // 
            // lciPaanServiceType
            // 
            this.lciPaanServiceType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPaanServiceType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPaanServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPaanServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPaanServiceType.Control = this.txtPaanServiceTypeCode;
            this.lciPaanServiceType.Location = new System.Drawing.Point(0, 50);
            this.lciPaanServiceType.Name = "lciPaanServiceType";
            this.lciPaanServiceType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciPaanServiceType.Size = new System.Drawing.Size(169, 24);
            this.lciPaanServiceType.Text = "Dịch vụ:";
            this.lciPaanServiceType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPaanServiceType.TextSize = new System.Drawing.Size(90, 20);
            this.lciPaanServiceType.TextToControlDistance = 5;
            // 
            // lciCboPaanServiceType
            // 
            this.lciCboPaanServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboPaanServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboPaanServiceType.Control = this.cboPaanServiceType;
            this.lciCboPaanServiceType.Location = new System.Drawing.Point(169, 50);
            this.lciCboPaanServiceType.Name = "lciCboPaanServiceType";
            this.lciCboPaanServiceType.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboPaanServiceType.Size = new System.Drawing.Size(240, 24);
            this.lciCboPaanServiceType.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboPaanServiceType.TextVisible = false;
            // 
            // lciPrice
            // 
            this.lciPrice.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPrice.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPrice.Control = this.spinPrice;
            this.lciPrice.Location = new System.Drawing.Point(749, 50);
            this.lciPrice.Name = "lciPrice";
            this.lciPrice.Size = new System.Drawing.Size(273, 24);
            this.lciPrice.Text = "Đơn giá:";
            this.lciPrice.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPrice.TextSize = new System.Drawing.Size(90, 20);
            this.lciPrice.TextToControlDistance = 5;
            // 
            // lciPaanPosition
            // 
            this.lciPaanPosition.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPaanPosition.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPaanPosition.Control = this.txtPaanPositionCode;
            this.lciPaanPosition.Location = new System.Drawing.Point(0, 98);
            this.lciPaanPosition.Name = "lciPaanPosition";
            this.lciPaanPosition.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciPaanPosition.Size = new System.Drawing.Size(169, 24);
            this.lciPaanPosition.Text = "Vị trí sinh thiết:";
            this.lciPaanPosition.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPaanPosition.TextSize = new System.Drawing.Size(90, 20);
            this.lciPaanPosition.TextToControlDistance = 5;
            // 
            // lciCboPaanPosition
            // 
            this.lciCboPaanPosition.Control = this.cboPaanPosition;
            this.lciCboPaanPosition.Location = new System.Drawing.Point(169, 98);
            this.lciCboPaanPosition.Name = "lciCboPaanPosition";
            this.lciCboPaanPosition.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboPaanPosition.Size = new System.Drawing.Size(240, 24);
            this.lciCboPaanPosition.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboPaanPosition.TextVisible = false;
            // 
            // lciPaanLiquid
            // 
            this.lciPaanLiquid.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPaanLiquid.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPaanLiquid.Control = this.txtPaanLiquidCode;
            this.lciPaanLiquid.Location = new System.Drawing.Point(409, 98);
            this.lciPaanLiquid.Name = "lciPaanLiquid";
            this.lciPaanLiquid.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciPaanLiquid.Size = new System.Drawing.Size(179, 24);
            this.lciPaanLiquid.Text = "Cố định dung dịch:";
            this.lciPaanLiquid.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPaanLiquid.TextSize = new System.Drawing.Size(90, 20);
            this.lciPaanLiquid.TextToControlDistance = 5;
            // 
            // lciCboPaanLiquid
            // 
            this.lciCboPaanLiquid.Control = this.cboPaanLiquid;
            this.lciCboPaanLiquid.Location = new System.Drawing.Point(588, 98);
            this.lciCboPaanLiquid.Name = "lciCboPaanLiquid";
            this.lciCboPaanLiquid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboPaanLiquid.Size = new System.Drawing.Size(161, 24);
            this.lciCboPaanLiquid.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboPaanLiquid.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboPaanLiquid.TextToControlDistance = 0;
            this.lciCboPaanLiquid.TextVisible = false;
            // 
            // lciDescription
            // 
            this.lciDescription.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDescription.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDescription.Control = this.txtDescription;
            this.lciDescription.Location = new System.Drawing.Point(0, 122);
            this.lciDescription.Name = "lciDescription";
            this.lciDescription.Size = new System.Drawing.Size(1022, 51);
            this.lciDescription.Text = "Mô tả:";
            this.lciDescription.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDescription.TextSize = new System.Drawing.Size(90, 20);
            this.lciDescription.TextToControlDistance = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSavePrint;
            this.layoutControlItem1.Location = new System.Drawing.Point(576, 173);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(77, 30);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(112, 30);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnSave;
            this.layoutControlItem2.Location = new System.Drawing.Point(688, 173);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(68, 30);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(111, 30);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnPrint;
            this.layoutControlItem4.Location = new System.Drawing.Point(799, 173);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(60, 30);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(112, 30);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnRefersh;
            this.layoutControlItem5.Location = new System.Drawing.Point(911, 173);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 30);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(83, 30);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(111, 30);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 173);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(576, 30);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciLoginname
            // 
            this.lciLoginname.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciLoginname.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciLoginname.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciLoginname.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciLoginname.Control = this.txtLoginname;
            this.lciLoginname.Location = new System.Drawing.Point(409, 26);
            this.lciLoginname.Name = "lciLoginname";
            this.lciLoginname.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciLoginname.Size = new System.Drawing.Size(241, 24);
            this.lciLoginname.Text = "Người chỉ định:";
            this.lciLoginname.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciLoginname.TextSize = new System.Drawing.Size(90, 20);
            this.lciLoginname.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.cboUsername;
            this.layoutControlItem7.Location = new System.Drawing.Point(650, 26);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem7.Size = new System.Drawing.Size(372, 24);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.panelControlSubIcd;
            this.layoutControlItem6.Location = new System.Drawing.Point(409, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Size = new System.Drawing.Size(613, 26);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciCboExecuteRoom
            // 
            this.lciCboExecuteRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciCboExecuteRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciCboExecuteRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboExecuteRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboExecuteRoom.Control = this.cboExecuteRoom;
            this.lciCboExecuteRoom.Location = new System.Drawing.Point(0, 74);
            this.lciCboExecuteRoom.MaxSize = new System.Drawing.Size(0, 24);
            this.lciCboExecuteRoom.MinSize = new System.Drawing.Size(409, 24);
            this.lciCboExecuteRoom.Name = "lciCboExecuteRoom";
            this.lciCboExecuteRoom.Size = new System.Drawing.Size(409, 24);
            this.lciCboExecuteRoom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCboExecuteRoom.Text = "Phòng xử lý:";
            this.lciCboExecuteRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboExecuteRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciCboExecuteRoom.TextToControlDistance = 5;
            // 
            // lciTracking
            // 
            this.lciTracking.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTracking.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTracking.Control = this.cboTracking;
            this.lciTracking.Location = new System.Drawing.Point(749, 74);
            this.lciTracking.Name = "lciTracking";
            this.lciTracking.Size = new System.Drawing.Size(215, 24);
            this.lciTracking.Text = "Tờ điều trị:";
            this.lciTracking.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTracking.TextSize = new System.Drawing.Size(90, 20);
            this.lciTracking.TextToControlDistance = 5;
            // 
            // lciPatientType
            // 
            this.lciPatientType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciPatientType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciPatientType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientType.Control = this.cboPatientType;
            this.lciPatientType.Location = new System.Drawing.Point(409, 50);
            this.lciPatientType.Name = "lciPatientType";
            this.lciPatientType.Size = new System.Drawing.Size(340, 24);
            this.lciPatientType.Text = "Đối tượng:";
            this.lciPatientType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientType.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientType.TextToControlDistance = 5;
            // 
            // lciSampleType
            // 
            this.lciSampleType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleType.Control = this.cboTestSampleType;
            this.lciSampleType.Location = new System.Drawing.Point(409, 74);
            this.lciSampleType.Name = "lciSampleType";
            this.lciSampleType.OptionsToolTip.ToolTip = "Loại mẫu bệnh phẩm xét nghiệm";
            this.lciSampleType.Size = new System.Drawing.Size(340, 24);
            this.lciSampleType.Text = "Loại mẫu XN:";
            this.lciSampleType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleType.TextSize = new System.Drawing.Size(90, 20);
            this.lciSampleType.TextToControlDistance = 5;
            // 
            // lciLiquidDate
            // 
            this.lciLiquidDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciLiquidDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciLiquidDate.Control = this.dtLiquidTime;
            this.lciLiquidDate.Location = new System.Drawing.Point(749, 98);
            this.lciLiquidDate.Name = "lciLiquidDate";
            this.lciLiquidDate.Size = new System.Drawing.Size(273, 24);
            this.lciLiquidDate.Text = "Ngày cố định:";
            this.lciLiquidDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciLiquidDate.TextSize = new System.Drawing.Size(90, 20);
            this.lciLiquidDate.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(964, 74);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(58, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmAssignPaan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 232);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmAssignPaan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chỉ định giải phẩu bệnh lý";
            this.Load += new System.EventHandler(this.frmAssignPaan_Load);
            this.Controls.SetChildIndex(this.barDockControlTop, 0);
            this.Controls.SetChildIndex(this.barDockControlBottom, 0);
            this.Controls.SetChildIndex(this.barDockControlRight, 0);
            this.Controls.SetChildIndex(this.barDockControlLeft, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboTracking.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlSubIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoginname.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLiquidTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtLiquidTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanLiquid.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanLiquidCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanPosition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanPositionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinPrice.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExecuteRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPaanServiceTypeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkIsSurgery.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPatientType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtInstructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlUcIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboPaanServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTestSampleType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.licIcd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciInstructionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIsSurgery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPaanLiquid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboPaanLiquid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLoginname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboExecuteRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTracking)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciLiquidDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControlUcIcd;
        private DevExpress.XtraLayout.LayoutControlItem licIcd;
        private DevExpress.XtraEditors.DateEdit dtInstructionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciInstructionTime;
        private DevExpress.XtraEditors.CheckEdit checkIsSurgery;
        private DevExpress.XtraEditors.LookUpEdit cboPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientType;
        private DevExpress.XtraLayout.LayoutControlItem lciIsSurgery;
        private DevExpress.XtraEditors.SpinEdit spinPrice;
        private DevExpress.XtraEditors.LookUpEdit cboExecuteRoom;
        private DevExpress.XtraEditors.TextEdit txtPaanServiceTypeCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPaanServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciCboPaanServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciCboExecuteRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciPrice;
        private DevExpress.XtraEditors.DateEdit dtLiquidTime;
        private DevExpress.XtraEditors.LookUpEdit cboPaanLiquid;
        private DevExpress.XtraEditors.TextEdit txtPaanLiquidCode;
        private DevExpress.XtraEditors.LookUpEdit cboPaanPosition;
        private DevExpress.XtraEditors.TextEdit txtPaanPositionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPaanPosition;
        private DevExpress.XtraLayout.LayoutControlItem lciCboPaanPosition;
        private DevExpress.XtraLayout.LayoutControlItem lciPaanLiquid;
        private DevExpress.XtraLayout.LayoutControlItem lciCboPaanLiquid;
        private DevExpress.XtraLayout.LayoutControlItem lciLiquidDate;
        private DevExpress.XtraEditors.MemoEdit txtDescription;
        private DevExpress.XtraLayout.LayoutControlItem lciDescription;
        private DevExpress.XtraEditors.SimpleButton btnRefersh;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnSavePrint;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSavePrint;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarButtonItem bbtnPrint;
        private DevExpress.XtraBars.BarButtonItem bbtnRCRefresh;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.LookUpEdit cboUsername;
        private DevExpress.XtraEditors.TextEdit txtLoginname;
        private DevExpress.XtraLayout.LayoutControlItem lciLoginname;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraEditors.PanelControl panelControlSubIcd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.GridLookUpEdit cboTracking;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciTracking;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboPaanServiceType;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn gridView1;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleType;
        private DevExpress.XtraEditors.LookUpEdit cboTestSampleType;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
