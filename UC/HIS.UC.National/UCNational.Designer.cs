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
namespace HIS.UC.National
{
    partial class UCNational
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cboNationals = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtNationalMainText = new DevExpress.XtraEditors.TextEdit();
            this.chkEditNational = new DevExpress.XtraEditors.CheckEdit();
            this.txtNationalCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciNationalText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboNationals.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditNational.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNationalText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.chkEditNational);
            this.layoutControl1.Controls.Add(this.txtNationalCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(348, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.cboNationals);
            this.panelControl1.Controls.Add(this.txtNationalMainText);
            this.panelControl1.Location = new System.Drawing.Point(150, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(151, 20);
            this.panelControl1.TabIndex = 6;
            // 
            // cboNationals
            // 
            this.cboNationals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboNationals.Location = new System.Drawing.Point(0, 0);
            this.cboNationals.Name = "cboNationals";
            this.cboNationals.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboNationals.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboNationals.Properties.ImmediatePopup = true;
            this.cboNationals.Properties.NullText = "";
            this.cboNationals.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboNationals.Properties.View = this.gridLookUpEdit1View;
            this.cboNationals.Size = new System.Drawing.Size(151, 20);
            this.cboNationals.TabIndex = 0;
            this.cboNationals.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboNationals_Closed);
            this.cboNationals.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboNationals_Properties_ButtonClick);
            this.cboNationals.EditValueChanged += new System.EventHandler(this.cboNationals_EditValueChanged);
            this.cboNationals.TextChanged += new System.EventHandler(this.cboNationals_TextChanged);
            this.cboNationals.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboNationals_KeyUp);
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
            // txtNationalMainText
            // 
            this.txtNationalMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNationalMainText.Location = new System.Drawing.Point(0, 0);
            this.txtNationalMainText.Name = "txtNationalMainText";
            this.txtNationalMainText.Size = new System.Drawing.Size(151, 20);
            this.txtNationalMainText.TabIndex = 0;
            this.txtNationalMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNationalMainText_PreviewKeyDown);
            // 
            // chkEditNational
            // 
            this.chkEditNational.Location = new System.Drawing.Point(305, 2);
            this.chkEditNational.Name = "chkEditNational";
            this.chkEditNational.Properties.Caption = "Sửa";
            this.chkEditNational.Size = new System.Drawing.Size(41, 19);
            this.chkEditNational.StyleController = this.layoutControl1;
            this.chkEditNational.TabIndex = 1;
            this.chkEditNational.CheckedChanged += new System.EventHandler(this.chkEditNational_CheckedChanged);
            this.chkEditNational.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditNational_PreviewKeyDown);
            // 
            // txtNationalCode
            // 
            this.txtNationalCode.Location = new System.Drawing.Point(107, 2);
            this.txtNationalCode.Name = "txtNationalCode";
            this.txtNationalCode.Size = new System.Drawing.Size(43, 20);
            this.txtNationalCode.StyleController = this.layoutControl1;
            this.txtNationalCode.TabIndex = 0;
            this.txtNationalCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtNationalCode_InvalidValue);
            this.txtNationalCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtNationalCode_PreviewKeyDown);
            this.txtNationalCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtNationalCode_Validating);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciNationalText,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(348, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciNationalText
            // 
            this.lciNationalText.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciNationalText.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciNationalText.Control = this.txtNationalCode;
            this.lciNationalText.Location = new System.Drawing.Point(0, 0);
            this.lciNationalText.MaxSize = new System.Drawing.Size(0, 24);
            this.lciNationalText.MinSize = new System.Drawing.Size(150, 24);
            this.lciNationalText.Name = "lciNationalText";
            this.lciNationalText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciNationalText.Size = new System.Drawing.Size(150, 24);
            this.lciNationalText.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciNationalText.Text = "Quốc gia:";
            this.lciNationalText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciNationalText.TextSize = new System.Drawing.Size(100, 20);
            this.lciNationalText.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.chkEditNational;
            this.layoutControlItem2.Location = new System.Drawing.Point(303, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(45, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.panelControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(150, 0);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(150, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(153, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCNational
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCNational";
            this.Size = new System.Drawing.Size(348, 24);
            this.Load += new System.EventHandler(this.UCNational_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboNationals.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditNational.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNationalCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciNationalText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtNationalMainText;
        private DevExpress.XtraEditors.CheckEdit chkEditNational;
        private DevExpress.XtraEditors.TextEdit txtNationalCode;
        private DevExpress.XtraLayout.LayoutControlItem lciNationalText;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private CustomGridLookUpEditWithFilterMultiColumn cboNationals;
        private CustomGridViewWithFilterMultiColumn gridLookUpEdit1View;
    }
}
