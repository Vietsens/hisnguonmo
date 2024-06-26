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
using Inventec.Desktop.CustomControl;
namespace HIS.UC.NextTreatmentInstruction
{
    partial class UCNextTreatmentInstruction
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
            this.components = new System.ComponentModel.Container();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cboNextTreatmentInstructions = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtNextTreatmentInstructionMainText = new DevExpress.XtraEditors.TextEdit();
            this.chkEditNextTreatmentInstruction = new DevExpress.XtraEditors.CheckEdit();
            this.txtNextTreatmentInstructionCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciNextTreatmentInstructionText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboNextTreatmentInstructions.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextTreatmentInstructionMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditNextTreatmentInstruction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextTreatmentInstructionCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNextTreatmentInstructionText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.chkEditNextTreatmentInstruction);
            this.layoutControl1.Controls.Add(this.txtNextTreatmentInstructionCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.cboNextTreatmentInstructions);
            this.panelControl1.Controls.Add(this.txtNextTreatmentInstructionMainText);
            this.panelControl1.Location = new System.Drawing.Point(156, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(431, 20);
            this.panelControl1.TabIndex = 6;
            // 
            // cboNextTreatmentInstructions
            // 
            this.cboNextTreatmentInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboNextTreatmentInstructions.Location = new System.Drawing.Point(0, 0);
            this.cboNextTreatmentInstructions.Name = "cboNextTreatmentInstructions";
            this.cboNextTreatmentInstructions.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboNextTreatmentInstructions.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboNextTreatmentInstructions.Properties.ImmediatePopup = true;
            this.cboNextTreatmentInstructions.Properties.NullText = "";
            this.cboNextTreatmentInstructions.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboNextTreatmentInstructions.Properties.View = this.gridLookUpEdit1View;
            this.cboNextTreatmentInstructions.Size = new System.Drawing.Size(431, 20);
            this.cboNextTreatmentInstructions.TabIndex = 0;
            this.cboNextTreatmentInstructions.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNextTreatmentInstructions_Closed);
            this.cboNextTreatmentInstructions.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNextTreatmentInstructions_Properties_ButtonClick);
            this.cboNextTreatmentInstructions.EditValueChanged += new System.EventHandler(this.cboNextTreatmentInstructions_EditValueChanged);
            this.cboNextTreatmentInstructions.TextChanged += new System.EventHandler(this.cboNextTreatmentInstructions_TextChanged);
            this.cboNextTreatmentInstructions.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboNextTreatmentInstructions_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsBehavior.AllowIncrementalSearch = true;
            this.gridLookUpEdit1View.OptionsFind.FindNullPrompt = "";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // txtNextTreatmentInstructionMainText
            // 
            this.txtNextTreatmentInstructionMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNextTreatmentInstructionMainText.Location = new System.Drawing.Point(0, 0);
            this.txtNextTreatmentInstructionMainText.Name = "txtNextTreatmentInstructionMainText";
            this.txtNextTreatmentInstructionMainText.Size = new System.Drawing.Size(431, 20);
            this.txtNextTreatmentInstructionMainText.TabIndex = 0;
            this.txtNextTreatmentInstructionMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNextTreatmentInstructionMainText_PreviewKeyDown);
            // 
            // chkEditNextTreatmentInstruction
            // 
            this.chkEditNextTreatmentInstruction.Location = new System.Drawing.Point(591, 2);
            this.chkEditNextTreatmentInstruction.Name = "chkEditNextTreatmentInstruction";
            this.chkEditNextTreatmentInstruction.Properties.Caption = "Sửa";
            this.chkEditNextTreatmentInstruction.Size = new System.Drawing.Size(67, 19);
            this.chkEditNextTreatmentInstruction.StyleController = this.layoutControl1;
            this.chkEditNextTreatmentInstruction.TabIndex = 1;
            this.chkEditNextTreatmentInstruction.CheckedChanged += new System.EventHandler(this.chkEditNextTreatmentInstruction_CheckedChanged);
            this.chkEditNextTreatmentInstruction.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditNextTreatmentInstruction_PreviewKeyDown);
            // 
            // txtNextTreatmentInstructionCode
            // 
            this.txtNextTreatmentInstructionCode.Location = new System.Drawing.Point(97, 2);
            this.txtNextTreatmentInstructionCode.Name = "txtNextTreatmentInstructionCode";
            this.txtNextTreatmentInstructionCode.Size = new System.Drawing.Size(59, 20);
            this.txtNextTreatmentInstructionCode.StyleController = this.layoutControl1;
            this.txtNextTreatmentInstructionCode.TabIndex = 0;
            this.txtNextTreatmentInstructionCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtNextTreatmentInstructionCode_InvalidValue);
            this.txtNextTreatmentInstructionCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNextTreatmentInstructionCode_PreviewKeyDown);
            this.txtNextTreatmentInstructionCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtNextTreatmentInstructionCode_Validating);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciNextTreatmentInstructionText,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciNextTreatmentInstructionText
            // 
            this.lciNextTreatmentInstructionText.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNextTreatmentInstructionText.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNextTreatmentInstructionText.Control = this.txtNextTreatmentInstructionCode;
            this.lciNextTreatmentInstructionText.Location = new System.Drawing.Point(0, 0);
            this.lciNextTreatmentInstructionText.MaxSize = new System.Drawing.Size(0, 24);
            this.lciNextTreatmentInstructionText.MinSize = new System.Drawing.Size(150, 24);
            this.lciNextTreatmentInstructionText.Name = "lciNextTreatmentInstructionText";
            this.lciNextTreatmentInstructionText.OptionsToolTip.ToolTip = "Chẩn đoán chính";
            this.lciNextTreatmentInstructionText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciNextTreatmentInstructionText.Size = new System.Drawing.Size(156, 24);
            this.lciNextTreatmentInstructionText.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciNextTreatmentInstructionText.Text = "Hướng điều trị:";
            this.lciNextTreatmentInstructionText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNextTreatmentInstructionText.TextSize = new System.Drawing.Size(90, 20);
            this.lciNextTreatmentInstructionText.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.chkEditNextTreatmentInstruction;
            this.layoutControlItem2.Location = new System.Drawing.Point(589, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(71, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.panelControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(156, 0);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(150, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(433, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCNextTreatmentInstruction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCNextTreatmentInstruction";
            this.Size = new System.Drawing.Size(660, 24);
            this.Load += new System.EventHandler(this.UCNextTreatmentInstruction_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboNextTreatmentInstructions.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextTreatmentInstructionMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditNextTreatmentInstruction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextTreatmentInstructionCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNextTreatmentInstructionText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtNextTreatmentInstructionMainText;
        private DevExpress.XtraEditors.CheckEdit chkEditNextTreatmentInstruction;
        private DevExpress.XtraEditors.TextEdit txtNextTreatmentInstructionCode;
        private DevExpress.XtraLayout.LayoutControlItem lciNextTreatmentInstructionText;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private CustomGridLookUpEditWithFilterMultiColumn cboNextTreatmentInstructions;
        private CustomGridViewWithFilterMultiColumn gridLookUpEdit1View;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
    }
}
