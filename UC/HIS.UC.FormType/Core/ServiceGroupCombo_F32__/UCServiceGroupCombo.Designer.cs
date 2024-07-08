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
namespace HIS.UC.FormType.Core.ServiceGroupCombo_F32__
{
    partial class UCServiceGroupCombo
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboServiceGroup = new DevExpress.XtraEditors.LookUpEdit();
            this.cboServiceType = new DevExpress.XtraEditors.LookUpEdit();
            this.txtServiceTypeCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCboServiceGroup = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceTypeCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboServiceGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboServiceGroup);
            this.layoutControl1.Controls.Add(this.cboServiceType);
            this.layoutControl1.Controls.Add(this.txtServiceTypeCode);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboServiceGroup
            // 
            this.cboServiceGroup.Location = new System.Drawing.Point(427, 2);
            this.cboServiceGroup.Name = "cboServiceGroup";
            this.cboServiceGroup.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboServiceGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboServiceGroup.Properties.NullText = "";
            this.cboServiceGroup.Size = new System.Drawing.Size(231, 20);
            this.cboServiceGroup.StyleController = this.layoutControl1;
            this.cboServiceGroup.TabIndex = 6;
            this.cboServiceGroup.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboServiceGroup_ButtonClick);
            this.cboServiceGroup.EditValueChanged += new System.EventHandler(this.cboServiceGroup_EditValueChanged);
            // 
            // cboServiceType
            // 
            this.cboServiceType.Location = new System.Drawing.Point(140, 2);
            this.cboServiceType.Name = "cboServiceType";
            this.cboServiceType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboServiceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.cboServiceType.Properties.NullText = "";
            this.cboServiceType.Size = new System.Drawing.Size(188, 20);
            this.cboServiceType.StyleController = this.layoutControl1;
            this.cboServiceType.TabIndex = 5;
            this.cboServiceType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboServiceType_Closed);
            this.cboServiceType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboServiceType_ButtonClick);
            this.cboServiceType.EditValueChanged += new System.EventHandler(this.cboServiceType_EditValueChanged);
            // 
            // txtServiceTypeCode
            // 
            this.txtServiceTypeCode.Location = new System.Drawing.Point(97, 2);
            this.txtServiceTypeCode.Name = "txtServiceTypeCode";
            this.txtServiceTypeCode.Size = new System.Drawing.Size(43, 20);
            this.txtServiceTypeCode.StyleController = this.layoutControl1;
            this.txtServiceTypeCode.TabIndex = 4;
            this.txtServiceTypeCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtServiceTypeCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciServiceType,
            this.lciCboServiceType,
            this.lciCboServiceGroup});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciServiceType
            // 
            this.lciServiceType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lciServiceType.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciServiceType.Control = this.txtServiceTypeCode;
            this.lciServiceType.Location = new System.Drawing.Point(0, 0);
            this.lciServiceType.MaxSize = new System.Drawing.Size(0, 150);
            this.lciServiceType.MinSize = new System.Drawing.Size(140, 24);
            this.lciServiceType.Name = "lciServiceType";
            this.lciServiceType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciServiceType.Size = new System.Drawing.Size(140, 24);
            this.lciServiceType.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciServiceType.Text = "Loại dịch vụ:";
            this.lciServiceType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciServiceType.TextSize = new System.Drawing.Size(90, 20);
            this.lciServiceType.TextToControlDistance = 5;
            // 
            // lciCboServiceType
            // 
            this.lciCboServiceType.Control = this.cboServiceType;
            this.lciCboServiceType.Location = new System.Drawing.Point(140, 0);
            this.lciCboServiceType.Name = "lciCboServiceType";
            this.lciCboServiceType.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboServiceType.Size = new System.Drawing.Size(190, 24);
            this.lciCboServiceType.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboServiceType.TextVisible = false;
            // 
            // lciCboServiceGroup
            // 
            this.lciCboServiceGroup.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboServiceGroup.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboServiceGroup.Control = this.cboServiceGroup;
            this.lciCboServiceGroup.Location = new System.Drawing.Point(330, 0);
            this.lciCboServiceGroup.Name = "lciCboServiceGroup";
            this.lciCboServiceGroup.Size = new System.Drawing.Size(330, 24);
            this.lciCboServiceGroup.Text = "Nhóm dịch vụ:";
            this.lciCboServiceGroup.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboServiceGroup.TextSize = new System.Drawing.Size(90, 20);
            this.lciCboServiceGroup.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCServiceGroupCombo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCServiceGroupCombo";
            this.Size = new System.Drawing.Size(660, 24);
            this.Load += new System.EventHandler(this.UCServiceGroupCombo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtServiceTypeCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboServiceGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LookUpEdit cboServiceGroup;
        private DevExpress.XtraEditors.LookUpEdit cboServiceType;
        private DevExpress.XtraEditors.TextEdit txtServiceTypeCode;
        private DevExpress.XtraLayout.LayoutControlItem lciServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciCboServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciCboServiceGroup;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
