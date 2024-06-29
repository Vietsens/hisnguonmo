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
namespace HIS.Desktop.Plugins.HisOpenStore
{
    partial class UCItemService
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
            this.tileControlMain = new DevExpress.XtraEditors.TileControl();
            this.picService = new DevExpress.XtraEditors.PictureEdit();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picService.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tileControlMain
            // 
            this.tileControlMain.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlMain.AppearanceItem.Normal.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlMain.AppearanceItem.Normal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.tileControlMain.AppearanceItem.Normal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.tileControlMain.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileControlMain.AppearanceItem.Normal.Options.UseBorderColor = true;
            this.tileControlMain.AppearanceItem.Normal.Options.UseFont = true;
            this.tileControlMain.AppearanceItem.Normal.Options.UseTextOptions = true;
            this.tileControlMain.AppearanceItem.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tileControlMain.BackColor = System.Drawing.Color.Transparent;
            this.tileControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileControlMain.DragSize = new System.Drawing.Size(0, 0);
            this.tileControlMain.ItemSize = 1200;
            this.tileControlMain.Location = new System.Drawing.Point(0, 0);
            this.tileControlMain.MaxId = 2;
            this.tileControlMain.Name = "tileControlMain";
            this.tileControlMain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileControlMain.Size = new System.Drawing.Size(600, 368);
            this.tileControlMain.TabIndex = 13;
            this.tileControlMain.Text = "tileControl1";
            // 
            // picService
            // 
            this.picService.Dock = System.Windows.Forms.DockStyle.Top;
            this.picService.EditValue = global::HIS.Desktop.Plugins.HisOpenStore.Properties.Resources.Service;
            this.picService.Location = new System.Drawing.Point(0, 0);
            this.picService.Name = "picService";
            this.picService.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picService.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.picService.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picService.Size = new System.Drawing.Size(600, 244);
            this.picService.TabIndex = 16;
            this.picService.Click += new System.EventHandler(this.picService_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(162)))), ((int)(((byte)(232)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(0, 247);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(600, 121);
            this.label1.TabIndex = 20;
            this.label1.Text = "Dịch vụ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // UCItemService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picService);
            this.Controls.Add(this.tileControlMain);
            this.Name = "UCItemService";
            this.Size = new System.Drawing.Size(600, 368);
            this.Load += new System.EventHandler(this.UCItemService_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picService.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TileControl tileControlMain;
        private DevExpress.XtraEditors.PictureEdit picService;
        private System.Windows.Forms.Label label1;

    }
}
