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
namespace HIS.Desktop.Plugins.UpdateVaccinationExam.Run
{
    partial class frmUpdateVaccinationExam
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
            this.layoutControlRoot = new DevExpress.XtraLayout.LayoutControl();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.dtRequestTime = new DevExpress.XtraEditors.DateEdit();
            this.cboRequestRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtRequestRoom = new DevExpress.XtraEditors.TextEdit();
            this.lblTDLPatientDOB = new DevExpress.XtraEditors.LabelControl();
            this.lblTDLPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblConclude = new DevExpress.XtraEditors.LabelControl();
            this.txtVaccinationExamCode = new DevExpress.XtraEditors.TextEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciVaccinationExamCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciConclude = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTDLPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTDLPatientDOB = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lciRequestRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRequestTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderEditorInfo = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).BeginInit();
            this.layoutControlRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtRequestTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRequestTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRequestRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRequestRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVaccinationExamCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVaccinationExamCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclude)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTDLPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTDLPatientDOB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRequestRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRequestTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlRoot
            // 
            this.layoutControlRoot.Controls.Add(this.btnSave);
            this.layoutControlRoot.Controls.Add(this.dtRequestTime);
            this.layoutControlRoot.Controls.Add(this.cboRequestRoom);
            this.layoutControlRoot.Controls.Add(this.txtRequestRoom);
            this.layoutControlRoot.Controls.Add(this.lblTDLPatientDOB);
            this.layoutControlRoot.Controls.Add(this.lblTDLPatientName);
            this.layoutControlRoot.Controls.Add(this.lblConclude);
            this.layoutControlRoot.Controls.Add(this.txtVaccinationExamCode);
            this.layoutControlRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControlRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutControlRoot.Name = "layoutControlRoot";
            this.layoutControlRoot.Root = this.Root;
            this.layoutControlRoot.Size = new System.Drawing.Size(507, 152);
            this.layoutControlRoot.TabIndex = 0;
            this.layoutControlRoot.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(363, 122);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(142, 22);
            this.btnSave.StyleController = this.layoutControlRoot;
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dtRequestTime
            // 
            this.dtRequestTime.EditValue = null;
            this.dtRequestTime.Location = new System.Drawing.Point(127, 98);
            this.dtRequestTime.Name = "dtRequestTime";
            this.dtRequestTime.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dtRequestTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtRequestTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtRequestTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtRequestTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtRequestTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
            this.dtRequestTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtRequestTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm";
            this.dtRequestTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.dtRequestTime.Size = new System.Drawing.Size(378, 20);
            this.dtRequestTime.StyleController = this.layoutControlRoot;
            this.dtRequestTime.TabIndex = 10;
            // 
            // cboRequestRoom
            // 
            this.cboRequestRoom.Location = new System.Drawing.Point(216, 74);
            this.cboRequestRoom.Name = "cboRequestRoom";
            this.cboRequestRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.cboRequestRoom.Properties.AutoComplete = false;
            this.cboRequestRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboRequestRoom.Properties.NullText = "";
            this.cboRequestRoom.Properties.View = this.gridLookUpEdit1View;
            this.cboRequestRoom.Size = new System.Drawing.Size(289, 20);
            this.cboRequestRoom.StyleController = this.layoutControlRoot;
            this.cboRequestRoom.TabIndex = 9;
            this.cboRequestRoom.EditValueChanged += new System.EventHandler(this.cboRequestRoom_EditValueChanged);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtRequestRoom
            // 
            this.txtRequestRoom.Location = new System.Drawing.Point(127, 74);
            this.txtRequestRoom.Name = "txtRequestRoom";
            this.txtRequestRoom.Size = new System.Drawing.Size(89, 20);
            this.txtRequestRoom.StyleController = this.layoutControlRoot;
            this.txtRequestRoom.TabIndex = 8;
            this.txtRequestRoom.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRequestRoom_PreviewKeyDown);
            // 
            // lblTDLPatientDOB
            // 
            this.lblTDLPatientDOB.Location = new System.Drawing.Point(376, 50);
            this.lblTDLPatientDOB.Name = "lblTDLPatientDOB";
            this.lblTDLPatientDOB.Size = new System.Drawing.Size(129, 20);
            this.lblTDLPatientDOB.StyleController = this.layoutControlRoot;
            this.lblTDLPatientDOB.TabIndex = 7;
            // 
            // lblTDLPatientName
            // 
            this.lblTDLPatientName.Location = new System.Drawing.Point(127, 50);
            this.lblTDLPatientName.MaximumSize = new System.Drawing.Size(170, 0);
            this.lblTDLPatientName.MinimumSize = new System.Drawing.Size(170, 0);
            this.lblTDLPatientName.Name = "lblTDLPatientName";
            this.lblTDLPatientName.Size = new System.Drawing.Size(170, 20);
            this.lblTDLPatientName.StyleController = this.layoutControlRoot;
            this.lblTDLPatientName.TabIndex = 6;
            // 
            // lblConclude
            // 
            this.lblConclude.Location = new System.Drawing.Point(127, 26);
            this.lblConclude.MinimumSize = new System.Drawing.Size(50, 0);
            this.lblConclude.Name = "lblConclude";
            this.lblConclude.Size = new System.Drawing.Size(378, 20);
            this.lblConclude.StyleController = this.layoutControlRoot;
            this.lblConclude.TabIndex = 5;
            // 
            // txtVaccinationExamCode
            // 
            this.txtVaccinationExamCode.Location = new System.Drawing.Point(127, 2);
            this.txtVaccinationExamCode.Name = "txtVaccinationExamCode";
            this.txtVaccinationExamCode.Size = new System.Drawing.Size(378, 20);
            this.txtVaccinationExamCode.StyleController = this.layoutControlRoot;
            this.txtVaccinationExamCode.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciVaccinationExamCode,
            this.lciConclude,
            this.lciTDLPatientName,
            this.lciTDLPatientDOB,
            this.emptySpaceItem3,
            this.lciRequestRoom,
            this.layoutControlItem6,
            this.lciRequestTime,
            this.layoutControlItem8});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(507, 159);
            this.Root.TextVisible = false;
            // 
            // lciVaccinationExamCode
            // 
            this.lciVaccinationExamCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciVaccinationExamCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciVaccinationExamCode.Control = this.txtVaccinationExamCode;
            this.lciVaccinationExamCode.Location = new System.Drawing.Point(0, 0);
            this.lciVaccinationExamCode.Name = "lciVaccinationExamCode";
            this.lciVaccinationExamCode.Size = new System.Drawing.Size(507, 24);
            this.lciVaccinationExamCode.Text = "Mã khám tiêm chủng:";
            this.lciVaccinationExamCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciVaccinationExamCode.TextSize = new System.Drawing.Size(120, 20);
            this.lciVaccinationExamCode.TextToControlDistance = 5;
            // 
            // lciConclude
            // 
            this.lciConclude.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciConclude.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciConclude.Control = this.lblConclude;
            this.lciConclude.Location = new System.Drawing.Point(0, 24);
            this.lciConclude.Name = "lciConclude";
            this.lciConclude.Size = new System.Drawing.Size(507, 24);
            this.lciConclude.Text = "Trạng thái:";
            this.lciConclude.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciConclude.TextSize = new System.Drawing.Size(120, 20);
            this.lciConclude.TextToControlDistance = 5;
            // 
            // lciTDLPatientName
            // 
            this.lciTDLPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTDLPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTDLPatientName.Control = this.lblTDLPatientName;
            this.lciTDLPatientName.Location = new System.Drawing.Point(0, 48);
            this.lciTDLPatientName.Name = "lciTDLPatientName";
            this.lciTDLPatientName.Size = new System.Drawing.Size(299, 24);
            this.lciTDLPatientName.Text = "Tên bệnh nhân:";
            this.lciTDLPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTDLPatientName.TextSize = new System.Drawing.Size(120, 20);
            this.lciTDLPatientName.TextToControlDistance = 5;
            // 
            // lciTDLPatientDOB
            // 
            this.lciTDLPatientDOB.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTDLPatientDOB.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTDLPatientDOB.Control = this.lblTDLPatientDOB;
            this.lciTDLPatientDOB.Location = new System.Drawing.Point(299, 48);
            this.lciTDLPatientDOB.Name = "lciTDLPatientDOB";
            this.lciTDLPatientDOB.Size = new System.Drawing.Size(208, 24);
            this.lciTDLPatientDOB.Text = "Ngày sinh:";
            this.lciTDLPatientDOB.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTDLPatientDOB.TextSize = new System.Drawing.Size(70, 20);
            this.lciTDLPatientDOB.TextToControlDistance = 5;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(0, 120);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(361, 39);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lciRequestRoom
            // 
            this.lciRequestRoom.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciRequestRoom.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciRequestRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRequestRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRequestRoom.Control = this.txtRequestRoom;
            this.lciRequestRoom.Location = new System.Drawing.Point(0, 72);
            this.lciRequestRoom.Name = "lciRequestRoom";
            this.lciRequestRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciRequestRoom.Size = new System.Drawing.Size(216, 24);
            this.lciRequestRoom.Text = "Phòng khám:";
            this.lciRequestRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRequestRoom.TextSize = new System.Drawing.Size(120, 20);
            this.lciRequestRoom.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.cboRequestRoom;
            this.layoutControlItem6.Location = new System.Drawing.Point(216, 72);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem6.Size = new System.Drawing.Size(291, 24);
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextToControlDistance = 0;
            this.layoutControlItem6.TextVisible = false;
            // 
            // lciRequestTime
            // 
            this.lciRequestTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciRequestTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciRequestTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRequestTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRequestTime.Control = this.dtRequestTime;
            this.lciRequestTime.Location = new System.Drawing.Point(0, 96);
            this.lciRequestTime.Name = "lciRequestTime";
            this.lciRequestTime.OptionsToolTip.ToolTip = "Thời gian yêu cầu";
            this.lciRequestTime.Size = new System.Drawing.Size(507, 24);
            this.lciRequestTime.Text = "Thời gian:";
            this.lciRequestTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRequestTime.TextSize = new System.Drawing.Size(120, 20);
            this.lciRequestTime.TextToControlDistance = 5;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnSave;
            this.layoutControlItem8.Location = new System.Drawing.Point(361, 120);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(146, 32);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControl1);
            this.barManager1.DockControls.Add(this.barDockControl2);
            this.barManager1.DockControls.Add(this.barDockControl3);
            this.barManager1.DockControls.Add(this.barDockControl4);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.FloatLocation = new System.Drawing.Point(680, 129);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Offset = 558;
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnSave
            // 
            this.bbtnSave.Caption = "Lưu (Ctrl S)";
            this.bbtnSave.Id = 0;
            this.bbtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnSave.Name = "bbtnSave";
            this.bbtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl1.Location = new System.Drawing.Point(0, 0);
            this.barDockControl1.Size = new System.Drawing.Size(507, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 152);
            this.barDockControl2.Size = new System.Drawing.Size(507, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 0);
            this.barDockControl3.Size = new System.Drawing.Size(0, 152);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(507, 0);
            this.barDockControl4.Size = new System.Drawing.Size(0, 152);
            // 
            // frmUpdateVaccinationExam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 152);
            this.Controls.Add(this.layoutControlRoot);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.MaximumSize = new System.Drawing.Size(525, 220);
            this.Name = "frmUpdateVaccinationExam";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sửa khám tiêm chủng";
            this.Load += new System.EventHandler(this.frmUpdateVaccinationExam_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControlRoot, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlRoot)).EndInit();
            this.layoutControlRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtRequestTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtRequestTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRequestRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRequestRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVaccinationExamCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciVaccinationExamCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciConclude)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTDLPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTDLPatientDOB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRequestRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRequestTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderEditorInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControlRoot;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.DateEdit dtRequestTime;
        private DevExpress.XtraEditors.GridLookUpEdit cboRequestRoom;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraEditors.TextEdit txtRequestRoom;
        private DevExpress.XtraEditors.LabelControl lblTDLPatientDOB;
        private DevExpress.XtraEditors.LabelControl lblTDLPatientName;
        private DevExpress.XtraEditors.LabelControl lblConclude;
        private DevExpress.XtraEditors.TextEdit txtVaccinationExamCode;
        private DevExpress.XtraLayout.LayoutControlItem lciVaccinationExamCode;
        private DevExpress.XtraLayout.LayoutControlItem lciConclude;
        private DevExpress.XtraLayout.LayoutControlItem lciTDLPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciTDLPatientDOB;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciRequestRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem lciRequestTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditorInfo;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
    }
}
