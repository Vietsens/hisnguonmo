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
namespace HIS.UC.ExamTreatmentFinish
{
    partial class frmICDInformation
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
            this.panelControlIcds = new DevExpress.XtraEditors.PanelControl();
            this.cboIcds = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.customGridViewWithFilterMultiColumn1 = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtIcdMainText = new DevExpress.XtraEditors.TextEdit();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.txtIcdText = new DevExpress.XtraEditors.TextEdit();
            this.txtIcdSubCode = new DevExpress.XtraEditors.TextEdit();
            this.chkEditIcd = new DevExpress.XtraEditors.CheckEdit();
            this.txtIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciICDCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciICDSubCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciICDText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPanelControlIcds = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlIcds)).BeginInit();
            this.panelControlIcds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDSubCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPanelControlIcds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControlIcds);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.txtIcdText);
            this.layoutControl1.Controls.Add(this.txtIcdSubCode);
            this.layoutControl1.Controls.Add(this.chkEditIcd);
            this.layoutControl1.Controls.Add(this.txtIcdCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(552, 171, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(448, 78);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControlIcds
            // 
            this.panelControlIcds.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlIcds.Controls.Add(this.cboIcds);
            this.panelControlIcds.Controls.Add(this.txtIcdMainText);
            this.panelControlIcds.Location = new System.Drawing.Point(172, 2);
            this.panelControlIcds.Name = "panelControlIcds";
            this.panelControlIcds.Size = new System.Drawing.Size(222, 20);
            this.panelControlIcds.TabIndex = 22;
            // 
            // cboIcds
            // 
            this.cboIcds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboIcds.Location = new System.Drawing.Point(0, 0);
            this.cboIcds.Name = "cboIcds";
            this.cboIcds.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboIcds.Properties.AutoComplete = false;
            this.cboIcds.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboIcds.Properties.NullText = "";
            this.cboIcds.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboIcds.Properties.View = this.customGridViewWithFilterMultiColumn1;
            this.cboIcds.Size = new System.Drawing.Size(222, 20);
            this.cboIcds.TabIndex = 1;
            this.cboIcds.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcds_Closed);
            this.cboIcds.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcds_ButtonClick);
            this.cboIcds.EditValueChanged += new System.EventHandler(this.cboIcds_EditValueChanged);
            this.cboIcds.TextChanged += new System.EventHandler(this.cboIcds_TextChanged);
            this.cboIcds.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboIcds_KeyUp);
            // 
            // customGridViewWithFilterMultiColumn1
            // 
            this.customGridViewWithFilterMultiColumn1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.customGridViewWithFilterMultiColumn1.Name = "customGridViewWithFilterMultiColumn1";
            this.customGridViewWithFilterMultiColumn1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.customGridViewWithFilterMultiColumn1.OptionsView.ShowGroupPanel = false;
            // 
            // txtIcdMainText
            // 
            this.txtIcdMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIcdMainText.Location = new System.Drawing.Point(0, 0);
            this.txtIcdMainText.Name = "txtIcdMainText";
            this.txtIcdMainText.Size = new System.Drawing.Size(222, 20);
            this.txtIcdMainText.TabIndex = 0;
            this.txtIcdMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainText_PreviewKeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(361, 54);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtIcdText
            // 
            this.txtIcdText.Location = new System.Drawing.Point(172, 26);
            this.txtIcdText.Name = "txtIcdText";
            this.txtIcdText.Properties.NullValuePrompt = "Nhấn F1 để chọn bệnh";
            this.txtIcdText.Size = new System.Drawing.Size(274, 20);
            this.txtIcdText.StyleController = this.layoutControl1;
            this.txtIcdText.TabIndex = 8;
            this.txtIcdText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyDown);
            this.txtIcdText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIcdText_KeyUp);
            // 
            // txtIcdSubCode
            // 
            this.txtIcdSubCode.Location = new System.Drawing.Point(97, 26);
            this.txtIcdSubCode.Name = "txtIcdSubCode";
            this.txtIcdSubCode.Size = new System.Drawing.Size(75, 20);
            this.txtIcdSubCode.StyleController = this.layoutControl1;
            this.txtIcdSubCode.TabIndex = 7;
            this.txtIcdSubCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyDown);
            this.txtIcdSubCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtIcdSubCode_KeyUp);
            // 
            // chkEditIcd
            // 
            this.chkEditIcd.Location = new System.Drawing.Point(398, 2);
            this.chkEditIcd.Name = "chkEditIcd";
            this.chkEditIcd.Properties.Caption = "Sửa";
            this.chkEditIcd.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.chkEditIcd.Size = new System.Drawing.Size(48, 19);
            this.chkEditIcd.StyleController = this.layoutControl1;
            this.chkEditIcd.TabIndex = 6;
            this.chkEditIcd.CheckedChanged += new System.EventHandler(this.chkEditIcd_CheckedChanged);
            this.chkEditIcd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcd_PreviewKeyDown);
            // 
            // txtIcdCode
            // 
            this.txtIcdCode.Location = new System.Drawing.Point(97, 2);
            this.txtIcdCode.Name = "txtIcdCode";
            this.txtIcdCode.Size = new System.Drawing.Size(75, 20);
            this.txtIcdCode.StyleController = this.layoutControl1;
            this.txtIcdCode.TabIndex = 4;
            this.txtIcdCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCode_InvalidValue);
            this.txtIcdCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciICDCode,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.lciICDSubCode,
            this.lciICDText,
            this.layoutControlItem1,
            this.lciPanelControlIcds});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(448, 78);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciICDCode
            // 
            this.lciICDCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciICDCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciICDCode.Control = this.txtIcdCode;
            this.lciICDCode.Location = new System.Drawing.Point(0, 0);
            this.lciICDCode.Name = "lciICDCode";
            this.lciICDCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciICDCode.Size = new System.Drawing.Size(172, 24);
            this.lciICDCode.Text = "CĐ chính:";
            this.lciICDCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciICDCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciICDCode.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(359, 30);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.chkEditIcd;
            this.layoutControlItem3.Location = new System.Drawing.Point(396, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(52, 24);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // lciICDSubCode
            // 
            this.lciICDSubCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciICDSubCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciICDSubCode.Control = this.txtIcdSubCode;
            this.lciICDSubCode.Location = new System.Drawing.Point(0, 24);
            this.lciICDSubCode.Name = "lciICDSubCode";
            this.lciICDSubCode.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciICDSubCode.Size = new System.Drawing.Size(172, 24);
            this.lciICDSubCode.Text = "CĐ phụ:";
            this.lciICDSubCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciICDSubCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciICDSubCode.TextToControlDistance = 5;
            // 
            // lciICDText
            // 
            this.lciICDText.Control = this.txtIcdText;
            this.lciICDText.Location = new System.Drawing.Point(172, 24);
            this.lciICDText.Name = "lciICDText";
            this.lciICDText.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciICDText.Size = new System.Drawing.Size(276, 24);
            this.lciICDText.TextSize = new System.Drawing.Size(0, 0);
            this.lciICDText.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnSave;
            this.layoutControlItem1.Location = new System.Drawing.Point(359, 48);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(89, 30);
            this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 4, 0);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciPanelControlIcds
            // 
            this.lciPanelControlIcds.Control = this.panelControlIcds;
            this.lciPanelControlIcds.Location = new System.Drawing.Point(172, 0);
            this.lciPanelControlIcds.Name = "lciPanelControlIcds";
            this.lciPanelControlIcds.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciPanelControlIcds.Size = new System.Drawing.Size(224, 24);
            this.lciPanelControlIcds.TextSize = new System.Drawing.Size(0, 0);
            this.lciPanelControlIcds.TextVisible = false;
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
            this.bar1.FloatLocation = new System.Drawing.Point(540, 131);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnSave)});
            this.bar1.Offset = 453;
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
            this.barDockControl1.Size = new System.Drawing.Size(448, 0);
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl2.Location = new System.Drawing.Point(0, 78);
            this.barDockControl2.Size = new System.Drawing.Size(448, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl3.Location = new System.Drawing.Point(0, 0);
            this.barDockControl3.Size = new System.Drawing.Size(0, 78);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(448, 0);
            this.barDockControl4.Size = new System.Drawing.Size(0, 78);
            // 
            // frmICDInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(448, 78);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControl2);
            this.Controls.Add(this.barDockControl1);
            this.Name = "frmICDInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thông tin chuẩn đoán hiển thị trên giấy ra viện";
            this.Load += new System.EventHandler(this.frmICDInformation_Load);
            this.Controls.SetChildIndex(this.barDockControl1, 0);
            this.Controls.SetChildIndex(this.barDockControl2, 0);
            this.Controls.SetChildIndex(this.barDockControl4, 0);
            this.Controls.SetChildIndex(this.barDockControl3, 0);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlIcds)).EndInit();
            this.panelControlIcds.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customGridViewWithFilterMultiColumn1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdSubCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDSubCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciICDText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPanelControlIcds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.CheckEdit chkEditIcd;
        private DevExpress.XtraEditors.TextEdit txtIcdCode;
        private DevExpress.XtraLayout.LayoutControlItem lciICDCode;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit txtIcdSubCode;
        private DevExpress.XtraLayout.LayoutControlItem lciICDSubCode;
        private DevExpress.XtraEditors.TextEdit txtIcdText;
        private DevExpress.XtraLayout.LayoutControlItem lciICDText;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarButtonItem bbtnSave;
        private DevExpress.XtraEditors.PanelControl panelControlIcds;
        private Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboIcds;
        private Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn customGridViewWithFilterMultiColumn1;
        private DevExpress.XtraEditors.TextEdit txtIcdMainText;
        private DevExpress.XtraLayout.LayoutControlItem lciPanelControlIcds;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
