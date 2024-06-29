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
namespace HIS.Desktop.Plugins.OpensourceHisStore.CardItem
{
    partial class CardItem
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
            this.txtServiceName = new DevExpress.XtraEditors.LabelControl();
            this.txtServiceCode = new DevExpress.XtraEditors.LabelControl();
            this.txtSumary = new DevExpress.XtraEditors.LabelControl();
            this.txtDepartment = new DevExpress.XtraEditors.LabelControl();
            this.txtRate = new DevExpress.XtraEditors.LabelControl();
            this.txtPrice = new DevExpress.XtraEditors.LabelControl();
            this.ImageUrl = new DevExpress.XtraEditors.PictureEdit();
            this.ImageAuthor = new DevExpress.XtraEditors.PictureEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageUrl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageAuthor.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Appearance.Control.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.layoutControl1.Appearance.Control.Options.UseBorderColor = true;
            this.layoutControl1.Controls.Add(this.txtServiceName);
            this.layoutControl1.Controls.Add(this.txtServiceCode);
            this.layoutControl1.Controls.Add(this.txtSumary);
            this.layoutControl1.Controls.Add(this.txtDepartment);
            this.layoutControl1.Controls.Add(this.txtRate);
            this.layoutControl1.Controls.Add(this.txtPrice);
            this.layoutControl1.Controls.Add(this.ImageUrl);
            this.layoutControl1.Controls.Add(this.ImageAuthor);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(348, 298);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtServiceName
            // 
            this.txtServiceName.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtServiceName.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.txtServiceName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.txtServiceName.Location = new System.Drawing.Point(126, 37);
            this.txtServiceName.MaximumSize = new System.Drawing.Size(160, 0);
            this.txtServiceName.MinimumSize = new System.Drawing.Size(0, 30);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Padding = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.txtServiceName.Size = new System.Drawing.Size(160, 34);
            this.txtServiceName.StyleController = this.layoutControl1;
            this.txtServiceName.TabIndex = 13;
            this.txtServiceName.Text = "Tên báo cáo nhập thuốc vật tư từ nhà cung cấp";
            this.txtServiceName.Click += new System.EventHandler(this.Control_Click);
            // 
            // txtServiceCode
            // 
            this.txtServiceCode.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtServiceCode.Location = new System.Drawing.Point(126, 12);
            this.txtServiceCode.Name = "txtServiceCode";
            this.txtServiceCode.Padding = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.txtServiceCode.Size = new System.Drawing.Size(61, 21);
            this.txtServiceCode.StyleController = this.layoutControl1;
            this.txtServiceCode.TabIndex = 12;
            this.txtServiceCode.Text = "Mã báo cáo";
            this.txtServiceCode.Click += new System.EventHandler(this.Control_Click);
            // 
            // txtSumary
            // 
            this.txtSumary.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtSumary.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.txtSumary.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.txtSumary.Location = new System.Drawing.Point(12, 100);
            this.txtSumary.MaximumSize = new System.Drawing.Size(275, 0);
            this.txtSumary.MinimumSize = new System.Drawing.Size(0, 40);
            this.txtSumary.Name = "txtSumary";
            this.txtSumary.Padding = new System.Windows.Forms.Padding(3, 5, 1, 3);
            this.txtSumary.Size = new System.Drawing.Size(275, 40);
            this.txtSumary.StyleController = this.layoutControl1;
            this.txtSumary.TabIndex = 11;
            this.txtSumary.Text = "Báo cáo chi tiết số lượng thuốc,vật tư nhập từ Nhà Cung Cấp trong và ngoài thầu";
            this.txtSumary.Click += new System.EventHandler(this.Control_Click);
            // 
            // txtDepartment
            // 
            this.txtDepartment.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtDepartment.Location = new System.Drawing.Point(159, 75);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Padding = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.txtDepartment.Size = new System.Drawing.Size(30, 21);
            this.txtDepartment.StyleController = this.layoutControl1;
            this.txtDepartment.TabIndex = 10;
            this.txtDepartment.Text = "Khoa";
            this.txtDepartment.Click += new System.EventHandler(this.Control_Click);
            // 
            // txtRate
            // 
            this.txtRate.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtRate.Location = new System.Drawing.Point(126, 75);
            this.txtRate.Name = "txtRate";
            this.txtRate.Padding = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.txtRate.Size = new System.Drawing.Size(29, 21);
            this.txtRate.StyleController = this.layoutControl1;
            this.txtRate.TabIndex = 9;
            this.txtRate.Text = "Rate";
            // 
            // txtPrice
            // 
            this.txtPrice.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.txtPrice.Location = new System.Drawing.Point(291, 12);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Padding = new System.Windows.Forms.Padding(3);
            this.txtPrice.Size = new System.Drawing.Size(45, 19);
            this.txtPrice.StyleController = this.layoutControl1;
            this.txtPrice.TabIndex = 7;
            this.txtPrice.Text = "Miễn phí";
            this.txtPrice.Click += new System.EventHandler(this.Control_Click);
            // 
            // ImageUrl
            // 
            this.ImageUrl.Location = new System.Drawing.Point(12, 144);
            this.ImageUrl.MaximumSize = new System.Drawing.Size(0, 150);
            this.ImageUrl.MinimumSize = new System.Drawing.Size(0, 100);
            this.ImageUrl.Name = "ImageUrl";
            this.ImageUrl.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.ImageUrl.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.ImageUrl.Size = new System.Drawing.Size(324, 142);
            this.ImageUrl.StyleController = this.layoutControl1;
            this.ImageUrl.TabIndex = 5;
            this.ImageUrl.Click += new System.EventHandler(this.Control_Click);
            // 
            // ImageAuthor
            // 
            this.ImageAuthor.Location = new System.Drawing.Point(12, 12);
            this.ImageAuthor.MaximumSize = new System.Drawing.Size(110, 0);
            this.ImageAuthor.MinimumSize = new System.Drawing.Size(110, 0);
            this.ImageAuthor.Name = "ImageAuthor";
            this.ImageAuthor.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.ImageAuthor.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.ImageAuthor.Size = new System.Drawing.Size(110, 84);
            this.ImageAuthor.StyleController = this.layoutControl1;
            this.ImageAuthor.TabIndex = 4;
            this.ImageAuthor.Click += new System.EventHandler(this.Control_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.layoutControlItem9,
            this.layoutControlItem10,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(348, 298);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ImageAuthor;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(114, 88);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.ImageUrl;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 132);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(328, 146);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.txtPrice;
            this.layoutControlItem4.Location = new System.Drawing.Point(279, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(49, 25);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.txtRate;
            this.layoutControlItem6.Location = new System.Drawing.Point(114, 63);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(33, 25);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.txtDepartment;
            this.layoutControlItem7.Location = new System.Drawing.Point(147, 63);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(181, 25);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.txtSumary;
            this.layoutControlItem8.Location = new System.Drawing.Point(0, 88);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(328, 44);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.Control = this.txtServiceCode;
            this.layoutControlItem9.Location = new System.Drawing.Point(114, 0);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(65, 25);
            this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem9.TextVisible = false;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.Control = this.txtServiceName;
            this.layoutControlItem10.Location = new System.Drawing.Point(114, 25);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(214, 38);
            this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem10.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(179, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(100, 25);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            this.emptySpaceItem1.Click += new System.EventHandler(this.Control_Click);
            // 
            // CardItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.layoutControl1);
            this.MaximumSize = new System.Drawing.Size(350, 300);
            this.MinimumSize = new System.Drawing.Size(2, 300);
            this.Name = "CardItem";
            this.Size = new System.Drawing.Size(348, 298);
            this.MouseEnter += new System.EventHandler(this.CardItem_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.CardItem_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImageUrl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImageAuthor.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl txtServiceName;
        private DevExpress.XtraEditors.LabelControl txtServiceCode;
        private DevExpress.XtraEditors.LabelControl txtSumary;
        private DevExpress.XtraEditors.LabelControl txtDepartment;
        private DevExpress.XtraEditors.LabelControl txtRate;
        private DevExpress.XtraEditors.LabelControl txtPrice;
        private DevExpress.XtraEditors.PictureEdit ImageUrl;
        private DevExpress.XtraEditors.PictureEdit ImageAuthor;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
