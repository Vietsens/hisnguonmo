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
namespace HIS.UC.Icd
{
    partial class UCIcd
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
            this.cboIcds = new Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn();
            this.gridLookUpEdit1View = new Inventec.Desktop.CustomControl.CustomGridViewWithFilterMultiColumn();
            this.txtIcdMainText = new DevExpress.XtraEditors.TextEdit();
            this.chkEditIcd = new DevExpress.XtraEditors.CheckEdit();
            this.txtIcdCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciIcdText = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.chkEditIcd);
            this.layoutControl1.Controls.Add(this.txtIcdCode);
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
            this.panelControl1.Controls.Add(this.cboIcds);
            this.panelControl1.Controls.Add(this.txtIcdMainText);
            this.panelControl1.Location = new System.Drawing.Point(260, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(349, 20);
            this.panelControl1.TabIndex = 6;
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
            this.cboIcds.Properties.ImmediatePopup = true;
            this.cboIcds.Properties.NullText = "";
            this.cboIcds.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboIcds.Properties.View = this.gridLookUpEdit1View;
            this.cboIcds.Size = new System.Drawing.Size(349, 20);
            this.cboIcds.TabIndex = 1;
            this.cboIcds.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboIcds_Closed);
            this.cboIcds.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboIcds_Properties_ButtonClick);
            this.cboIcds.EditValueChanged += new System.EventHandler(this.cboIcds_EditValueChanged);
            this.cboIcds.TextChanged += new System.EventHandler(this.cboIcds_TextChanged);
            this.cboIcds.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboIcds_KeyUp);
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
            // txtIcdMainText
            // 
            this.txtIcdMainText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIcdMainText.Location = new System.Drawing.Point(0, 0);
            this.txtIcdMainText.Name = "txtIcdMainText";
            this.txtIcdMainText.Size = new System.Drawing.Size(349, 20);
            this.txtIcdMainText.TabIndex = 0;
            this.txtIcdMainText.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdMainText_PreviewKeyDown);
            // 
            // chkEditIcd
            // 
            this.chkEditIcd.Location = new System.Drawing.Point(613, 2);
            this.chkEditIcd.Name = "chkEditIcd";
            this.chkEditIcd.Properties.Caption = "Sửa";
            this.chkEditIcd.Size = new System.Drawing.Size(45, 19);
            this.chkEditIcd.StyleController = this.layoutControl1;
            this.chkEditIcd.TabIndex = 2;
            this.chkEditIcd.CheckedChanged += new System.EventHandler(this.chkEditIcd_CheckedChanged);
            this.chkEditIcd.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkEditIcd_PreviewKeyDown);
            // 
            // txtIcdCode
            // 
            this.txtIcdCode.Location = new System.Drawing.Point(97, 2);
            this.txtIcdCode.Name = "txtIcdCode";
            this.txtIcdCode.Size = new System.Drawing.Size(163, 20);
            this.txtIcdCode.StyleController = this.layoutControl1;
            this.txtIcdCode.TabIndex = 0;
            this.txtIcdCode.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtIcdCode_InvalidValue);
            this.txtIcdCode.TextChanged += new System.EventHandler(this.txtIcdCode_TextChanged);
            this.txtIcdCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtIcdCode_PreviewKeyDown);
            this.txtIcdCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtIcdCode_Validating);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciIcdText,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciIcdText
            // 
            this.lciIcdText.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIcdText.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIcdText.Control = this.txtIcdCode;
            this.lciIcdText.Location = new System.Drawing.Point(0, 0);
            this.lciIcdText.MaxSize = new System.Drawing.Size(0, 24);
            this.lciIcdText.MinSize = new System.Drawing.Size(150, 24);
            this.lciIcdText.Name = "lciIcdText";
            this.lciIcdText.OptionsToolTip.ToolTip = "Chẩn đoán chính";
            this.lciIcdText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciIcdText.Size = new System.Drawing.Size(260, 24);
            this.lciIcdText.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciIcdText.Text = "CĐ chính:";
            this.lciIcdText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIcdText.TextSize = new System.Drawing.Size(90, 20);
            this.lciIcdText.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.chkEditIcd;
            this.layoutControlItem2.Location = new System.Drawing.Point(611, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(49, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.panelControl1;
            this.layoutControlItem3.Location = new System.Drawing.Point(260, 0);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(150, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(351, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCIcd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UCIcd";
            this.Size = new System.Drawing.Size(660, 24);
            this.Load += new System.EventHandler(this.UCIcd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboIcds.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdMainText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkEditIcd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIcdCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIcdText)).EndInit();
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
        private DevExpress.XtraEditors.TextEdit txtIcdMainText;
        private DevExpress.XtraEditors.CheckEdit chkEditIcd;
        private DevExpress.XtraEditors.TextEdit txtIcdCode;
        private DevExpress.XtraLayout.LayoutControlItem lciIcdText;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private CustomGridLookUpEditWithFilterMultiColumn cboIcds;
        private CustomGridViewWithFilterMultiColumn gridLookUpEdit1View;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
    }
}
