namespace HIS.Desktop.Plugins.CreateTransReqQR.CreateTransReqQR
{
    partial class frmSubScreen
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblGenderName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblDob = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblAddress = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblAmount = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblStt = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblStt);
            this.layoutControl1.Controls.Add(this.lblAmount);
            this.layoutControl1.Controls.Add(this.pictureEdit1);
            this.layoutControl1.Controls.Add(this.lblAddress);
            this.layoutControl1.Controls.Add(this.lblDob);
            this.layoutControl1.Controls.Add(this.lblGenderName);
            this.layoutControl1.Controls.Add(this.lblPatientName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1115, 706);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1115, 706);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblPatientName
            // 
            this.lblPatientName.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblPatientName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(97, 2);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(224, 20);
            this.lblPatientName.StyleController = this.layoutControl1;
            this.lblPatientName.TabIndex = 4;
            this.lblPatientName.Text = " ";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem1.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem1.Control = this.lblPatientName;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(323, 24);
            this.layoutControlItem1.Text = "Tên bệnh nhân:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // lblGenderName
            // 
            this.lblGenderName.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblGenderName.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblGenderName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblGenderName.Location = new System.Drawing.Point(390, 2);
            this.lblGenderName.Name = "lblGenderName";
            this.lblGenderName.Size = new System.Drawing.Size(91, 20);
            this.lblGenderName.StyleController = this.layoutControl1;
            this.lblGenderName.TabIndex = 5;
            this.lblGenderName.Text = " ";
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem2.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem2.Control = this.lblGenderName;
            this.layoutControlItem2.Location = new System.Drawing.Point(323, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(160, 24);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(80, 24);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(160, 24);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.Text = "Giới tính:";
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(60, 20);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // lblDob
            // 
            this.lblDob.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblDob.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblDob.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDob.Location = new System.Drawing.Point(550, 2);
            this.lblDob.Name = "lblDob";
            this.lblDob.Size = new System.Drawing.Size(91, 20);
            this.lblDob.StyleController = this.layoutControl1;
            this.lblDob.TabIndex = 6;
            this.lblDob.Text = " ";
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.lblDob;
            this.layoutControlItem3.Location = new System.Drawing.Point(483, 0);
            this.layoutControlItem3.MaxSize = new System.Drawing.Size(160, 24);
            this.layoutControlItem3.MinSize = new System.Drawing.Size(80, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(160, 24);
            this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem3.Text = "Ngày sinh:";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(60, 20);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // lblAddress
            // 
            this.lblAddress.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.lblAddress.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.lblAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAddress.Location = new System.Drawing.Point(710, 2);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(403, 20);
            this.lblAddress.StyleController = this.layoutControl1;
            this.lblAddress.TabIndex = 7;
            this.lblAddress.Text = " ";
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.lblAddress;
            this.layoutControlItem4.Location = new System.Drawing.Point(643, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(472, 24);
            this.layoutControlItem4.Text = "Địa chỉ:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(60, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // pictureEdit1
            // 
            this.pictureEdit1.Location = new System.Drawing.Point(220, 44);
            this.pictureEdit1.Name = "pictureEdit1";
            this.pictureEdit1.Properties.Padding = new System.Windows.Forms.Padding(15);
            this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
            this.pictureEdit1.Size = new System.Drawing.Size(675, 558);
            this.pictureEdit1.StyleController = this.layoutControl1;
            this.pictureEdit1.TabIndex = 8;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.pictureEdit1;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(220, 220, 20, 10);
            this.layoutControlItem5.Size = new System.Drawing.Size(1115, 588);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // lblAmount
            // 
            this.lblAmount.AllowHtmlString = true;
            this.lblAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.lblAmount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblAmount.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAmount.Location = new System.Drawing.Point(7, 614);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(1106, 31);
            this.lblAmount.StyleController = this.layoutControl1;
            this.lblAmount.TabIndex = 9;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.lblAmount;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 612);
            this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem6.MinSize = new System.Drawing.Size(27, 35);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(1115, 35);
            this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem6.Text = " ";
            this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem6.TextToControlDistance = 5;
            // 
            // lblStt
            // 
            this.lblStt.AllowHtmlString = true;
            this.lblStt.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.lblStt.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblStt.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStt.Location = new System.Drawing.Point(7, 649);
            this.lblStt.Name = "lblStt";
            this.lblStt.Size = new System.Drawing.Size(1098, 31);
            this.lblStt.StyleController = this.layoutControl1;
            this.lblStt.TabIndex = 10;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem7.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlItem7.Control = this.lblStt;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 647);
            this.layoutControlItem7.MaxSize = new System.Drawing.Size(0, 35);
            this.layoutControlItem7.MinSize = new System.Drawing.Size(27, 35);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 10, 2, 2);
            this.layoutControlItem7.Size = new System.Drawing.Size(1115, 35);
            this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem7.TextToControlDistance = 5;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 682);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(1115, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmSubScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1115, 706);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmSubScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmSubScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblAddress;
        private DevExpress.XtraEditors.LabelControl lblDob;
        private DevExpress.XtraEditors.LabelControl lblGenderName;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.LabelControl lblStt;
        private DevExpress.XtraEditors.LabelControl lblAmount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}