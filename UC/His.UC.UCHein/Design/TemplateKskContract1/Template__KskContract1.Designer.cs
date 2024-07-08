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
namespace His.UC.UCHein.Design.TemplateKskContract1
{
    partial class Template__KskContract1
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
            this.cboKskContract = new DevExpress.XtraEditors.LookUpEdit();
            this.txtKskContractCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.lblCaptionKskContract = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboKskContract.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKskContractCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCaptionKskContract)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboKskContract);
            this.layoutControl1.Controls.Add(this.txtKskContractCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1320, 25);
            this.layoutControl1.TabIndex = 146;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboKskContract
            // 
            this.cboKskContract.Location = new System.Drawing.Point(164, 2);
            this.cboKskContract.Name = "cboKskContract";
            this.cboKskContract.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboKskContract.Properties.NullText = "";
            this.cboKskContract.Properties.PopupSizeable = false;
            this.cboKskContract.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboKskContract.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboKskContract_Properties_GetNotInListValue);
            this.cboKskContract.Size = new System.Drawing.Size(274, 20);
            this.cboKskContract.StyleController = this.layoutControl1;
            this.cboKskContract.TabIndex = 2;
            this.cboKskContract.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboKskContract_Closed);
            this.cboKskContract.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboKskContract_KeyUp);
            // 
            // txtKskContractCode
            // 
            this.txtKskContractCode.Location = new System.Drawing.Point(97, 2);
            this.txtKskContractCode.Name = "txtKskContractCode";
            this.txtKskContractCode.Properties.Appearance.Options.UseTextOptions = true;
            this.txtKskContractCode.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.txtKskContractCode.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtKskContractCode.Size = new System.Drawing.Size(67, 20);
            this.txtKskContractCode.StyleController = this.layoutControl1;
            this.txtKskContractCode.TabIndex = 1;
            this.txtKskContractCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMaKskContract_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem2,
            this.lblCaptionKskContract,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1320, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(440, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(880, 25);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // lblCaptionKskContract
            // 
            this.lblCaptionKskContract.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lblCaptionKskContract.AppearanceItemCaption.Options.UseForeColor = true;
            this.lblCaptionKskContract.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblCaptionKskContract.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCaptionKskContract.Control = this.txtKskContractCode;
            this.lblCaptionKskContract.Location = new System.Drawing.Point(0, 0);
            this.lblCaptionKskContract.Name = "lblCaptionKskContract";
            this.lblCaptionKskContract.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lblCaptionKskContract.Size = new System.Drawing.Size(164, 25);
            this.lblCaptionKskContract.Text = "Hợp đồng KSK:";
            this.lblCaptionKskContract.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblCaptionKskContract.TextSize = new System.Drawing.Size(90, 20);
            this.lblCaptionKskContract.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboKskContract;
            this.layoutControlItem2.Location = new System.Drawing.Point(164, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem2.Size = new System.Drawing.Size(276, 25);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidateHiddenControls = false;
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // Template__KskContract1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "Template__KskContract1";
            this.Size = new System.Drawing.Size(1320, 25);
            this.Load += new System.EventHandler(this.Template__KskContract1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboKskContract.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKskContractCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblCaptionKskContract)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal DevExpress.XtraEditors.TextEdit txtKskContractCode;
        internal DevExpress.XtraEditors.LookUpEdit cboKskContract;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControlItem lblCaptionKskContract;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;


    }
}
