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
namespace HIS.Desktop.Plugins.ConnectionTest
{
    partial class frmUpdateCondition
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
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.cboSampleCondition = new DevExpress.XtraEditors.LookUpEdit();
            this.txtSampleConditionCode = new DevExpress.XtraEditors.TextEdit();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientCode = new DevExpress.XtraEditors.LabelControl();
            this.lblTreatmentCode = new DevExpress.XtraEditors.LabelControl();
            this.lblBarcode = new DevExpress.XtraEditors.LabelControl();
            this.lblServiceReqCode = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciServiceReqCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTreatmentCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciPatientName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciSampleCondition = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barBtnSave = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleCondition.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleConditionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.cboSampleCondition);
            this.layoutControl1.Controls.Add(this.txtSampleConditionCode);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Controls.Add(this.lblPatientCode);
            this.layoutControl1.Controls.Add(this.lblTreatmentCode);
            this.layoutControl1.Controls.Add(this.lblBarcode);
            this.layoutControl1.Controls.Add(this.lblServiceReqCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 38);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(609, 149);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(469, 107);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(137, 27);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cboSampleCondition
            // 
            this.cboSampleCondition.Location = new System.Drawing.Point(276, 80);
            this.cboSampleCondition.Margin = new System.Windows.Forms.Padding(4);
            this.cboSampleCondition.Name = "cboSampleCondition";
            this.cboSampleCondition.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboSampleCondition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboSampleCondition.Properties.NullText = "";
            this.cboSampleCondition.Size = new System.Drawing.Size(331, 22);
            this.cboSampleCondition.StyleController = this.layoutControl1;
            this.cboSampleCondition.TabIndex = 10;
            this.cboSampleCondition.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboSampleCondition_Closed);
            this.cboSampleCondition.EditValueChanged += new System.EventHandler(this.cboSampleCondition_EditValueChanged);
            // 
            // txtSampleConditionCode
            // 
            this.txtSampleConditionCode.Location = new System.Drawing.Point(97, 80);
            this.txtSampleConditionCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtSampleConditionCode.Name = "txtSampleConditionCode";
            this.txtSampleConditionCode.Size = new System.Drawing.Size(179, 22);
            this.txtSampleConditionCode.StyleController = this.layoutControl1;
            this.txtSampleConditionCode.TabIndex = 9;
            this.txtSampleConditionCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtSampleConditionCode_PreviewKeyDown);
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(98, 55);
            this.lblPatientName.Margin = new System.Windows.Forms.Padding(4);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(508, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 8;
            // 
            // lblPatientCode
            // 
            this.lblPatientCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPatientCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientCode.Location = new System.Drawing.Point(403, 29);
            this.lblPatientCode.Margin = new System.Windows.Forms.Padding(4);
            this.lblPatientCode.Name = "lblPatientCode";
            this.lblPatientCode.Size = new System.Drawing.Size(203, 20);
            this.lblPatientCode.StyleController = this.layoutControl1;
            this.lblPatientCode.TabIndex = 7;
            // 
            // lblTreatmentCode
            // 
            this.lblTreatmentCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTreatmentCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTreatmentCode.Location = new System.Drawing.Point(98, 29);
            this.lblTreatmentCode.Margin = new System.Windows.Forms.Padding(4);
            this.lblTreatmentCode.Name = "lblTreatmentCode";
            this.lblTreatmentCode.Size = new System.Drawing.Size(204, 20);
            this.lblTreatmentCode.StyleController = this.layoutControl1;
            this.lblTreatmentCode.TabIndex = 6;
            // 
            // lblBarcode
            // 
            this.lblBarcode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBarcode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBarcode.Location = new System.Drawing.Point(402, 3);
            this.lblBarcode.Margin = new System.Windows.Forms.Padding(4);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(204, 20);
            this.lblBarcode.StyleController = this.layoutControl1;
            this.lblBarcode.TabIndex = 5;
            // 
            // lblServiceReqCode
            // 
            this.lblServiceReqCode.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblServiceReqCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblServiceReqCode.Location = new System.Drawing.Point(98, 3);
            this.lblServiceReqCode.Margin = new System.Windows.Forms.Padding(5);
            this.lblServiceReqCode.Name = "lblServiceReqCode";
            this.lblServiceReqCode.Size = new System.Drawing.Size(203, 20);
            this.lblServiceReqCode.StyleController = this.layoutControl1;
            this.lblServiceReqCode.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciBarcode,
            this.lciTreatmentCode,
            this.lciPatientCode,
            this.lciPatientName,
            this.lciSampleCondition,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.emptySpaceItem1,
            this.lciServiceReqCode});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(609, 149);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciServiceReqCode
            // 
            this.lciServiceReqCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceReqCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceReqCode.Control = this.lblServiceReqCode;
            this.lciServiceReqCode.Location = new System.Drawing.Point(0, 0);
            this.lciServiceReqCode.Name = "lciServiceReqCode";
            this.lciServiceReqCode.Size = new System.Drawing.Size(304, 26);
            this.lciServiceReqCode.Text = "Mã yêu cầu:";
            this.lciServiceReqCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciServiceReqCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciServiceReqCode.TextToControlDistance = 5;
            // 
            // lciBarcode
            // 
            this.lciBarcode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciBarcode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciBarcode.Control = this.lblBarcode;
            this.lciBarcode.Location = new System.Drawing.Point(304, 0);
            this.lciBarcode.Name = "lciBarcode";
            this.lciBarcode.Size = new System.Drawing.Size(305, 26);
            this.lciBarcode.Text = "Barcode:";
            this.lciBarcode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciBarcode.TextSize = new System.Drawing.Size(90, 20);
            this.lciBarcode.TextToControlDistance = 5;
            // 
            // lciTreatmentCode
            // 
            this.lciTreatmentCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTreatmentCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTreatmentCode.Control = this.lblTreatmentCode;
            this.lciTreatmentCode.Location = new System.Drawing.Point(0, 26);
            this.lciTreatmentCode.Name = "lciTreatmentCode";
            this.lciTreatmentCode.Size = new System.Drawing.Size(305, 26);
            this.lciTreatmentCode.Text = "Mã điều trị:";
            this.lciTreatmentCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTreatmentCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciTreatmentCode.TextToControlDistance = 5;
            // 
            // lciPatientCode
            // 
            this.lciPatientCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientCode.Control = this.lblPatientCode;
            this.lciPatientCode.Location = new System.Drawing.Point(305, 26);
            this.lciPatientCode.Name = "lciPatientCode";
            this.lciPatientCode.Size = new System.Drawing.Size(304, 26);
            this.lciPatientCode.Text = "Mã bệnh nhân:";
            this.lciPatientCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientCode.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientCode.TextToControlDistance = 5;
            // 
            // lciPatientName
            // 
            this.lciPatientName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciPatientName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciPatientName.Control = this.lblPatientName;
            this.lciPatientName.Location = new System.Drawing.Point(0, 52);
            this.lciPatientName.Name = "lciPatientName";
            this.lciPatientName.Size = new System.Drawing.Size(609, 26);
            this.lciPatientName.Text = "Tên bệnh nhân:";
            this.lciPatientName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciPatientName.TextSize = new System.Drawing.Size(90, 20);
            this.lciPatientName.TextToControlDistance = 5;
            // 
            // lciSampleCondition
            // 
            this.lciSampleCondition.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciSampleCondition.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciSampleCondition.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciSampleCondition.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciSampleCondition.Control = this.txtSampleConditionCode;
            this.lciSampleCondition.Location = new System.Drawing.Point(0, 78);
            this.lciSampleCondition.Name = "lciSampleCondition";
            this.lciSampleCondition.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciSampleCondition.Size = new System.Drawing.Size(276, 26);
            this.lciSampleCondition.Text = "Tình trạng:";
            this.lciSampleCondition.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciSampleCondition.TextSize = new System.Drawing.Size(90, 20);
            this.lciSampleCondition.TextToControlDistance = 5;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.cboSampleCondition;
            this.layoutControlItem7.Location = new System.Drawing.Point(276, 78);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem7.Size = new System.Drawing.Size(333, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnSave;
            this.layoutControlItem8.Location = new System.Drawing.Point(466, 104);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(143, 45);
            this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextToControlDistance = 0;
            this.layoutControlItem8.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 104);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(466, 45);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
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
            this.barBtnSave});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barBtnSave)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // barBtnSave
            // 
            this.barBtnSave.Caption = "Lưu (Ctrl S)";
            this.barBtnSave.Id = 0;
            this.barBtnSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.barBtnSave.Name = "barBtnSave";
            this.barBtnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlTop.Size = new System.Drawing.Size(609, 38);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 187);
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlBottom.Size = new System.Drawing.Size(609, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 38);
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 149);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(609, 38);
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 149);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // frmUpdateCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 187);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmUpdateCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cập nhật tình trạng mẫu";
            this.Load += new System.EventHandler(this.frmUpdateCondition_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboSampleCondition.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSampleConditionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceReqCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTreatmentCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciPatientName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciSampleCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.LookUpEdit cboSampleCondition;
        private DevExpress.XtraEditors.TextEdit txtSampleConditionCode;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblPatientCode;
        private DevExpress.XtraEditors.LabelControl lblTreatmentCode;
        private DevExpress.XtraEditors.LabelControl lblBarcode;
        private DevExpress.XtraEditors.LabelControl lblServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceReqCode;
        private DevExpress.XtraLayout.LayoutControlItem lciBarcode;
        private DevExpress.XtraLayout.LayoutControlItem lciTreatmentCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientCode;
        private DevExpress.XtraLayout.LayoutControlItem lciPatientName;
        private DevExpress.XtraLayout.LayoutControlItem lciSampleCondition;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem barBtnSave;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
