namespace HIS.Desktop.Plugins.MchTreatmentExamService.UCAdress
{
    partial class UCAddress
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
            this.txtAddress = new DevExpress.XtraEditors.TextEdit();
            this.txtCommuneCode = new DevExpress.XtraEditors.TextEdit();
            this.txtDistrictCode = new DevExpress.XtraEditors.TextEdit();
            this.txtProvinceCode = new DevExpress.XtraEditors.TextEdit();
            this.togChangeStructAdress = new DevExpress.XtraEditors.ToggleSwitch();
            this.cboProvince = new DevExpress.XtraEditors.LookUpEdit();
            this.cboDistrict = new DevExpress.XtraEditors.LookUpEdit();
            this.cboCommune = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProvince = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciDistrict = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciCommune = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciAddress = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommuneCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistrictCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.togChangeStructAdress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProvince.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDistrict.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommune.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvince)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDistrict)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCommune)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtAddress);
            this.layoutControl1.Controls.Add(this.txtCommuneCode);
            this.layoutControl1.Controls.Add(this.txtDistrictCode);
            this.layoutControl1.Controls.Add(this.txtProvinceCode);
            this.layoutControl1.Controls.Add(this.togChangeStructAdress);
            this.layoutControl1.Controls.Add(this.cboProvince);
            this.layoutControl1.Controls.Add(this.cboDistrict);
            this.layoutControl1.Controls.Add(this.cboCommune);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1109, 28);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(827, 2);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(280, 20);
            this.txtAddress.StyleController = this.layoutControl1;
            this.txtAddress.TabIndex = 11;
            this.txtAddress.EditValueChanged += new System.EventHandler(this.txtAddress_EditValueChanged);
            // 
            // txtCommuneCode
            // 
            this.txtCommuneCode.Location = new System.Drawing.Point(575, 2);
            this.txtCommuneCode.Name = "txtCommuneCode";
            this.txtCommuneCode.Size = new System.Drawing.Size(53, 20);
            this.txtCommuneCode.StyleController = this.layoutControl1;
            this.txtCommuneCode.TabIndex = 10;
            this.txtCommuneCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtCommuneCode_PreviewKeyDown);
            // 
            // txtDistrictCode
            // 
            this.txtDistrictCode.Location = new System.Drawing.Point(365, 2);
            this.txtDistrictCode.Name = "txtDistrictCode";
            this.txtDistrictCode.Size = new System.Drawing.Size(43, 20);
            this.txtDistrictCode.StyleController = this.layoutControl1;
            this.txtDistrictCode.TabIndex = 9;
            this.txtDistrictCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtDistrictCode_PreviewKeyDown);
            // 
            // txtProvinceCode
            // 
            this.txtProvinceCode.Location = new System.Drawing.Point(121, 2);
            this.txtProvinceCode.Name = "txtProvinceCode";
            this.txtProvinceCode.Size = new System.Drawing.Size(43, 20);
            this.txtProvinceCode.StyleController = this.layoutControl1;
            this.txtProvinceCode.TabIndex = 5;
            this.txtProvinceCode.EditValueChanged += new System.EventHandler(this.txtProvinceCode_EditValueChanged);
            this.txtProvinceCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtProvinceCode_PreviewKeyDown);
            // 
            // togChangeStructAdress
            // 
            this.togChangeStructAdress.Location = new System.Drawing.Point(2, 2);
            this.togChangeStructAdress.Name = "togChangeStructAdress";
            this.togChangeStructAdress.Properties.OffText = "";
            this.togChangeStructAdress.Properties.OnText = "";
            this.togChangeStructAdress.Size = new System.Drawing.Size(70, 24);
            this.togChangeStructAdress.StyleController = this.layoutControl1;
            this.togChangeStructAdress.TabIndex = 4;
            this.togChangeStructAdress.Toggled += new System.EventHandler(this.togChangeStructAdress_Toggled);
            // 
            // cboProvince
            // 
            this.cboProvince.Location = new System.Drawing.Point(164, 2);
            this.cboProvince.Name = "cboProvince";
            this.cboProvince.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboProvince.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboProvince.Properties.NullText = "";
            this.cboProvince.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboProvince_Properties_GetNotInListValue);
            this.cboProvince.Size = new System.Drawing.Size(142, 20);
            this.cboProvince.StyleController = this.layoutControl1;
            this.cboProvince.TabIndex = 6;
            this.cboProvince.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProvince_Closed);
            this.cboProvince.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProvince_ButtonClick);
            this.cboProvince.EditValueChanged += new System.EventHandler(this.cboProvince_EditValueChanged);
            this.cboProvince.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboProvince_KeyUp);
            // 
            // cboDistrict
            // 
            this.cboDistrict.Location = new System.Drawing.Point(408, 2);
            this.cboDistrict.Name = "cboDistrict";
            this.cboDistrict.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboDistrict.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboDistrict.Properties.NullText = "";
            this.cboDistrict.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboDistrict_Properties_GetNotInListValue);
            this.cboDistrict.Size = new System.Drawing.Size(128, 20);
            this.cboDistrict.StyleController = this.layoutControl1;
            this.cboDistrict.TabIndex = 7;
            this.cboDistrict.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboDistrict_Closed);
            this.cboDistrict.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboDistrict_ButtonClick);
            this.cboDistrict.EditValueChanged += new System.EventHandler(this.cboDistrict_EditValueChanged);
            this.cboDistrict.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboDistrict_KeyUp);
            // 
            // cboCommune
            // 
            this.cboCommune.Location = new System.Drawing.Point(628, 2);
            this.cboCommune.Name = "cboCommune";
            this.cboCommune.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboCommune.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboCommune.Properties.NullText = "";
            this.cboCommune.Properties.GetNotInListValue += new DevExpress.XtraEditors.Controls.GetNotInListValueEventHandler(this.cboCommune_Properties_GetNotInListValue);
            this.cboCommune.Size = new System.Drawing.Size(140, 20);
            this.cboCommune.StyleController = this.layoutControl1;
            this.cboCommune.TabIndex = 8;
            this.cboCommune.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboCommune_Closed);
            this.cboCommune.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboCommune_ButtonClick);
            this.cboCommune.EditValueChanged += new System.EventHandler(this.cboCommune_EditValueChanged);
            this.cboCommune.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboCommune_KeyUp);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lciProvince,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.lciDistrict,
            this.lciCommune,
            this.lciAddress});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1109, 28);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.togChangeStructAdress;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(74, 28);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(74, 28);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(74, 28);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciProvince
            // 
            this.lciProvince.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciProvince.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciProvince.Control = this.txtProvinceCode;
            this.lciProvince.Location = new System.Drawing.Point(74, 0);
            this.lciProvince.MaxSize = new System.Drawing.Size(90, 24);
            this.lciProvince.MinSize = new System.Drawing.Size(90, 24);
            this.lciProvince.Name = "lciProvince";
            this.lciProvince.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciProvince.Size = new System.Drawing.Size(90, 28);
            this.lciProvince.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciProvince.Text = "Tỉnh:";
            this.lciProvince.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciProvince.TextSize = new System.Drawing.Size(40, 20);
            this.lciProvince.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboProvince;
            this.layoutControlItem3.Location = new System.Drawing.Point(164, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(120, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(144, 28);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.cboDistrict;
            this.layoutControlItem4.Location = new System.Drawing.Point(408, 0);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(130, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem4.Size = new System.Drawing.Size(130, 28);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.cboCommune;
            this.layoutControlItem5.Location = new System.Drawing.Point(628, 0);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 24);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(130, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(142, 28);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // lciDistrict
            // 
            this.lciDistrict.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciDistrict.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciDistrict.Control = this.txtDistrictCode;
            this.lciDistrict.Location = new System.Drawing.Point(308, 0);
            this.lciDistrict.MaxSize = new System.Drawing.Size(100, 24);
            this.lciDistrict.MinSize = new System.Drawing.Size(100, 24);
            this.lciDistrict.Name = "lciDistrict";
            this.lciDistrict.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciDistrict.Size = new System.Drawing.Size(100, 28);
            this.lciDistrict.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciDistrict.Text = "Huyện:";
            this.lciDistrict.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciDistrict.TextSize = new System.Drawing.Size(50, 20);
            this.lciDistrict.TextToControlDistance = 5;
            // 
            // lciCommune
            // 
            this.lciCommune.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCommune.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCommune.Control = this.txtCommuneCode;
            this.lciCommune.Location = new System.Drawing.Point(538, 0);
            this.lciCommune.MaxSize = new System.Drawing.Size(90, 24);
            this.lciCommune.MinSize = new System.Drawing.Size(90, 24);
            this.lciCommune.Name = "lciCommune";
            this.lciCommune.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciCommune.Size = new System.Drawing.Size(90, 28);
            this.lciCommune.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciCommune.Text = "Xã:";
            this.lciCommune.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCommune.TextSize = new System.Drawing.Size(30, 20);
            this.lciCommune.TextToControlDistance = 5;
            // 
            // lciAddress
            // 
            this.lciAddress.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciAddress.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciAddress.Control = this.txtAddress;
            this.lciAddress.Location = new System.Drawing.Point(770, 0);
            this.lciAddress.Name = "lciAddress";
            this.lciAddress.Size = new System.Drawing.Size(339, 28);
            this.lciAddress.Text = "Địa chỉ:";
            this.lciAddress.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciAddress.TextSize = new System.Drawing.Size(50, 20);
            this.lciAddress.TextToControlDistance = 5;
            // 
            // UCAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCAddress";
            this.Size = new System.Drawing.Size(1109, 28);
            this.Load += new System.EventHandler(this.UCAdress_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCommuneCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDistrictCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProvinceCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.togChangeStructAdress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProvince.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDistrict.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCommune.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProvince)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciDistrict)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCommune)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciAddress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.TextEdit txtCommuneCode;
        private DevExpress.XtraEditors.TextEdit txtDistrictCode;
        private DevExpress.XtraEditors.TextEdit txtProvinceCode;
        private DevExpress.XtraEditors.ToggleSwitch togChangeStructAdress;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciProvince;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem lciDistrict;
        private DevExpress.XtraLayout.LayoutControlItem lciCommune;
        private DevExpress.XtraEditors.TextEdit txtAddress;
        private DevExpress.XtraLayout.LayoutControlItem lciAddress;
        private DevExpress.XtraEditors.LookUpEdit cboProvince;
        private DevExpress.XtraEditors.LookUpEdit cboDistrict;
        private DevExpress.XtraEditors.LookUpEdit cboCommune;
    }
}
