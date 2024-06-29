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
namespace HIS.UC.UCRelativeInfo
{
    partial class UCRelativeInfo
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
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtMother = new DevExpress.XtraEditors.TextEdit();
            this.txtFather = new DevExpress.XtraEditors.TextEdit();
            this.chkCapGiayNghiOm = new DevExpress.XtraEditors.CheckEdit();
            this.txtRelativePhone = new DevExpress.XtraEditors.TextEdit();
            this.txtRelativeCMNDNumber = new DevExpress.XtraEditors.TextEdit();
            this.txtRelativeAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtCorrelated = new DevExpress.XtraEditors.TextEdit();
            this.txtHomePerson = new DevExpress.XtraEditors.TextEdit();
            this.lcgHomePerson = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciHomPerson = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRelative = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAddress = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCMND = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFortxtRelativePhone = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciForchkCapGiayNghiOm = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFather = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciMother = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProviderControl = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProviderControl = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMother.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFather.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCapGiayNghiOm.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativePhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativeCMNDNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativeAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorrelated.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHomePerson.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgHomePerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHomPerson)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRelative)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMND)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFortxtRelativePhone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForchkCapGiayNghiOm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFather)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMother)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtMother);
            this.layoutControl1.Controls.Add(this.txtFather);
            this.layoutControl1.Controls.Add(this.chkCapGiayNghiOm);
            this.layoutControl1.Controls.Add(this.txtRelativePhone);
            this.layoutControl1.Controls.Add(this.txtRelativeCMNDNumber);
            this.layoutControl1.Controls.Add(this.txtRelativeAddress);
            this.layoutControl1.Controls.Add(this.txtCorrelated);
            this.layoutControl1.Controls.Add(this.txtHomePerson);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.lcgHomePerson;
            this.layoutControl1.Size = new System.Drawing.Size(438, 117);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtMother
            // 
            this.txtMother.Location = new System.Drawing.Point(296, 21);
            this.txtMother.Name = "txtMother";
            this.txtMother.Size = new System.Drawing.Size(139, 20);
            this.txtMother.StyleController = this.layoutControl1;
            this.txtMother.TabIndex = 15;
            this.txtMother.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtMother_PreviewKeyDown);
            // 
            // txtFather
            // 
            this.txtFather.Location = new System.Drawing.Point(78, 21);
            this.txtFather.Name = "txtFather";
            this.txtFather.Size = new System.Drawing.Size(139, 20);
            this.txtFather.StyleController = this.layoutControl1;
            this.txtFather.TabIndex = 14;
            this.txtFather.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtFather_PreviewKeyDown);
            // 
            // chkCapGiayNghiOm
            // 
            this.chkCapGiayNghiOm.Location = new System.Drawing.Point(297, 93);
            this.chkCapGiayNghiOm.Name = "chkCapGiayNghiOm";
            this.chkCapGiayNghiOm.Properties.Caption = "";
            this.chkCapGiayNghiOm.Size = new System.Drawing.Size(138, 19);
            this.chkCapGiayNghiOm.StyleController = this.layoutControl1;
            toolTipItem1.Text = "Nếu có cấp giấy nghỉ ốm thì tích vào ô này";
            superToolTip1.Items.Add(toolTipItem1);
            this.chkCapGiayNghiOm.SuperTip = superToolTip1;
            this.chkCapGiayNghiOm.TabIndex = 13;
            this.chkCapGiayNghiOm.CheckedChanged += new System.EventHandler(this.chkCapGiayNghiOm_CheckedChanged);
            this.chkCapGiayNghiOm.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkCapGiayNghiOm_PreviewKeyDown);
            // 
            // txtRelativePhone
            // 
            this.txtRelativePhone.Location = new System.Drawing.Point(297, 69);
            this.txtRelativePhone.Name = "txtRelativePhone";
            this.txtRelativePhone.Properties.MaxLength = 12;
            this.txtRelativePhone.Size = new System.Drawing.Size(138, 20);
            this.txtRelativePhone.StyleController = this.layoutControl1;
            this.txtRelativePhone.TabIndex = 11;
            this.txtRelativePhone.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativePhone_PreviewKeyDown);
            // 
            // txtRelativeCMNDNumber
            // 
            this.txtRelativeCMNDNumber.Location = new System.Drawing.Point(78, 69);
            this.txtRelativeCMNDNumber.Name = "txtRelativeCMNDNumber";
            this.txtRelativeCMNDNumber.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRelativeCMNDNumber.Properties.MaxLength = 12;
            this.txtRelativeCMNDNumber.Size = new System.Drawing.Size(140, 20);
            this.txtRelativeCMNDNumber.StyleController = this.layoutControl1;
            this.txtRelativeCMNDNumber.TabIndex = 10;
            this.txtRelativeCMNDNumber.EditValueChanged += new System.EventHandler(this.txtRelativeCMNDNumber_EditValueChanged);
            this.txtRelativeCMNDNumber.TextChanged += new System.EventHandler(this.txtRelativeCMNDNumber_TextChanged);
            this.txtRelativeCMNDNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRelativeCMNDNumber_KeyPress);
            this.txtRelativeCMNDNumber.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativeCMNDNumber_PreviewKeyDown);
            this.txtRelativeCMNDNumber.Validated += new System.EventHandler(this.txtRelativeCMNDNumber_Validated);
            // 
            // txtRelativeAddress
            // 
            this.txtRelativeAddress.Location = new System.Drawing.Point(78, 93);
            this.txtRelativeAddress.Name = "txtRelativeAddress";
            this.txtRelativeAddress.Size = new System.Drawing.Size(140, 20);
            this.txtRelativeAddress.StyleController = this.layoutControl1;
            this.txtRelativeAddress.TabIndex = 12;
            this.txtRelativeAddress.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRelativeAddress_PreviewKeyDown);
            // 
            // txtCorrelated
            // 
            this.txtCorrelated.Location = new System.Drawing.Point(297, 45);
            this.txtCorrelated.Name = "txtCorrelated";
            this.txtCorrelated.Size = new System.Drawing.Size(138, 20);
            this.txtCorrelated.StyleController = this.layoutControl1;
            this.txtCorrelated.TabIndex = 9;
            this.txtCorrelated.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCorrelated_PreviewKeyDown);
            // 
            // txtHomePerson
            // 
            this.txtHomePerson.Location = new System.Drawing.Point(78, 45);
            this.txtHomePerson.Name = "txtHomePerson";
            this.txtHomePerson.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtHomePerson.Size = new System.Drawing.Size(140, 20);
            this.txtHomePerson.StyleController = this.layoutControl1;
            this.txtHomePerson.TabIndex = 8;
            this.txtHomePerson.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtHomePerson_PreviewKeyDown);
            // 
            // lcgHomePerson
            // 
            this.lcgHomePerson.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.lcgHomePerson.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciHomPerson,
            this.lciRelative,
            this.lciAddress,
            this.lciCMND,
            this.lciFortxtRelativePhone,
            this.lciForchkCapGiayNghiOm,
            this.lciFather,
            this.lciMother});
            this.lcgHomePerson.Location = new System.Drawing.Point(0, 0);
            this.lcgHomePerson.Name = "lcgHomePerson";
            this.lcgHomePerson.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lcgHomePerson.Size = new System.Drawing.Size(438, 117);
            this.lcgHomePerson.Text = "Người nhà (F9)";
            // 
            // lciHomPerson
            // 
            this.lciHomPerson.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHomPerson.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHomPerson.Control = this.txtHomePerson;
            this.lciHomPerson.Location = new System.Drawing.Point(0, 24);
            this.lciHomPerson.MaxSize = new System.Drawing.Size(0, 24);
            this.lciHomPerson.MinSize = new System.Drawing.Size(110, 24);
            this.lciHomPerson.Name = "lciHomPerson";
            this.lciHomPerson.Size = new System.Drawing.Size(219, 24);
            this.lciHomPerson.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHomPerson.Text = "Họ tên:";
            this.lciHomPerson.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHomPerson.TextSize = new System.Drawing.Size(70, 20);
            this.lciHomPerson.TextToControlDistance = 5;
            // 
            // lciRelative
            // 
            this.lciRelative.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRelative.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRelative.Control = this.txtCorrelated;
            this.lciRelative.Location = new System.Drawing.Point(219, 24);
            this.lciRelative.Name = "lciRelative";
            this.lciRelative.Size = new System.Drawing.Size(217, 24);
            this.lciRelative.Text = "Quan hệ:";
            this.lciRelative.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRelative.TextSize = new System.Drawing.Size(70, 20);
            this.lciRelative.TextToControlDistance = 5;
            // 
            // lciAddress
            // 
            this.lciAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAddress.Control = this.txtRelativeAddress;
            this.lciAddress.Location = new System.Drawing.Point(0, 72);
            this.lciAddress.Name = "lciAddress";
            this.lciAddress.Size = new System.Drawing.Size(219, 25);
            this.lciAddress.Text = "Địa chỉ:";
            this.lciAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAddress.TextSize = new System.Drawing.Size(70, 20);
            this.lciAddress.TextToControlDistance = 5;
            // 
            // lciCMND
            // 
            this.lciCMND.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCMND.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCMND.Control = this.txtRelativeCMNDNumber;
            this.lciCMND.Location = new System.Drawing.Point(0, 48);
            this.lciCMND.Name = "lciCMND";
            this.lciCMND.Size = new System.Drawing.Size(219, 24);
            this.lciCMND.Text = "CMND/CCCD:";
            this.lciCMND.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCMND.TextSize = new System.Drawing.Size(70, 20);
            this.lciCMND.TextToControlDistance = 5;
            // 
            // lciFortxtRelativePhone
            // 
            this.lciFortxtRelativePhone.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFortxtRelativePhone.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFortxtRelativePhone.Control = this.txtRelativePhone;
            this.lciFortxtRelativePhone.Location = new System.Drawing.Point(219, 48);
            this.lciFortxtRelativePhone.Name = "lciFortxtRelativePhone";
            this.lciFortxtRelativePhone.Size = new System.Drawing.Size(217, 24);
            this.lciFortxtRelativePhone.Text = "SĐT:";
            this.lciFortxtRelativePhone.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFortxtRelativePhone.TextSize = new System.Drawing.Size(70, 20);
            this.lciFortxtRelativePhone.TextToControlDistance = 5;
            // 
            // lciForchkCapGiayNghiOm
            // 
            this.lciForchkCapGiayNghiOm.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciForchkCapGiayNghiOm.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciForchkCapGiayNghiOm.Control = this.chkCapGiayNghiOm;
            this.lciForchkCapGiayNghiOm.Location = new System.Drawing.Point(219, 72);
            this.lciForchkCapGiayNghiOm.Name = "lciForchkCapGiayNghiOm";
            this.lciForchkCapGiayNghiOm.OptionsToolTip.ToolTip = "Nếu có cấp giấy nghỉ ốm thì tích vào ô này";
            this.lciForchkCapGiayNghiOm.Size = new System.Drawing.Size(217, 25);
            this.lciForchkCapGiayNghiOm.Text = "CG nghỉ ốm:";
            this.lciForchkCapGiayNghiOm.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciForchkCapGiayNghiOm.TextSize = new System.Drawing.Size(70, 20);
            this.lciForchkCapGiayNghiOm.TextToControlDistance = 5;
            // 
            // lciFather
            // 
            this.lciFather.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFather.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFather.Control = this.txtFather;
            this.lciFather.Location = new System.Drawing.Point(0, 0);
            this.lciFather.Name = "lciFather";
            this.lciFather.Size = new System.Drawing.Size(218, 24);
            this.lciFather.Text = "Bố:";
            this.lciFather.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFather.TextSize = new System.Drawing.Size(70, 20);
            this.lciFather.TextToControlDistance = 5;
            // 
            // lciMother
            // 
            this.lciMother.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciMother.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciMother.Control = this.txtMother;
            this.lciMother.Location = new System.Drawing.Point(218, 0);
            this.lciMother.Name = "lciMother";
            this.lciMother.Size = new System.Drawing.Size(218, 24);
            this.lciMother.Text = "Mẹ:";
            this.lciMother.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciMother.TextSize = new System.Drawing.Size(70, 20);
            this.lciMother.TextToControlDistance = 5;
            // 
            // dxValidationProviderControl
            // 
            this.dxValidationProviderControl.ValidateHiddenControls = false;
            // 
            // dxErrorProviderControl
            // 
            this.dxErrorProviderControl.ContainerControl = this;
            // 
            // UCRelativeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCRelativeInfo";
            this.Size = new System.Drawing.Size(438, 117);
            this.Load += new System.EventHandler(this.UCRelativeInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMother.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFather.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkCapGiayNghiOm.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativePhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativeCMNDNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelativeAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorrelated.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHomePerson.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcgHomePerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHomPerson)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRelative)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCMND)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFortxtRelativePhone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciForchkCapGiayNghiOm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFather)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciMother)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProviderControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProviderControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup lcgHomePerson;
        internal DevExpress.XtraEditors.TextEdit txtHomePerson;
        private DevExpress.XtraLayout.LayoutControlItem lciHomPerson;
        internal DevExpress.XtraEditors.TextEdit txtCorrelated;
        private DevExpress.XtraLayout.LayoutControlItem lciRelative;
        internal DevExpress.XtraEditors.TextEdit txtRelativeAddress;
        private DevExpress.XtraLayout.LayoutControlItem lciAddress;
        private DevExpress.XtraLayout.LayoutControlItem lciCMND;
        internal DevExpress.XtraEditors.TextEdit txtRelativeCMNDNumber;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderControl;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderControl;
        private DevExpress.XtraEditors.TextEdit txtRelativePhone;
        private DevExpress.XtraLayout.LayoutControlItem lciFortxtRelativePhone;
        private DevExpress.XtraEditors.CheckEdit chkCapGiayNghiOm;
        private DevExpress.XtraLayout.LayoutControlItem lciForchkCapGiayNghiOm;
        private DevExpress.XtraEditors.TextEdit txtMother;
        private DevExpress.XtraEditors.TextEdit txtFather;
        private DevExpress.XtraLayout.LayoutControlItem lciFather;
        private DevExpress.XtraLayout.LayoutControlItem lciMother;
        DevExpress.Utils.ToolTipItem toolTipItem2;
    }
}
