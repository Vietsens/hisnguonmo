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
namespace HIS.Desktop.Plugins.OpensourceHisStoreReportDetail
{
    partial class UCItemComment
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
            this.picShowRepply = new DevExpress.XtraEditors.PictureEdit();
            this.lblNumRepply = new DevExpress.XtraEditors.LabelControl();
            this.lblNumLike = new DevExpress.XtraEditors.LabelControl();
            this.lblContent = new DevExpress.XtraEditors.LabelControl();
            this.lblTimePost = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblUserName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblStatusLike = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picShowRepply.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblStatusLike)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.panelControl1);
            this.layoutControl1.Controls.Add(this.lblNumRepply);
            this.layoutControl1.Controls.Add(this.lblNumLike);
            this.layoutControl1.Controls.Add(this.lblContent);
            this.layoutControl1.Controls.Add(this.lblTimePost);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(507, 66);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.picShowRepply);
            this.panelControl1.Location = new System.Drawing.Point(178, 51);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(20, 13);
            this.panelControl1.TabIndex = 44;
            // 
            // picShowRepply
            // 
            this.picShowRepply.Dock = System.Windows.Forms.DockStyle.Left;
            this.picShowRepply.EditValue = global::HIS.Desktop.Plugins.OpensourceHisStoreReportDetail.Properties.Resources.arrowDown;
            this.picShowRepply.Location = new System.Drawing.Point(0, 0);
            this.picShowRepply.Name = "picShowRepply";
            this.picShowRepply.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.picShowRepply.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.picShowRepply.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.picShowRepply.Size = new System.Drawing.Size(24, 13);
            this.picShowRepply.TabIndex = 43;
            this.picShowRepply.Click += new System.EventHandler(this.picRepply_Click);
            // 
            // lblNumRepply
            // 
            this.lblNumRepply.Location = new System.Drawing.Point(154, 51);
            this.lblNumRepply.Name = "lblNumRepply";
            this.lblNumRepply.Size = new System.Drawing.Size(20, 13);
            this.lblNumRepply.StyleController = this.layoutControl1;
            this.lblNumRepply.TabIndex = 41;
            this.lblNumRepply.Text = "(10)";
            // 
            // lblNumLike
            // 
            this.lblNumLike.Location = new System.Drawing.Point(57, 51);
            this.lblNumLike.Name = "lblNumLike";
            this.lblNumLike.Size = new System.Drawing.Size(20, 13);
            this.lblNumLike.StyleController = this.layoutControl1;
            this.lblNumLike.TabIndex = 40;
            this.lblNumLike.Text = "(29)";
            // 
            // lblContent
            // 
            this.lblContent.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblContent.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblContent.Location = new System.Drawing.Point(2, 19);
            this.lblContent.MaximumSize = new System.Drawing.Size(600, 300);
            this.lblContent.MinimumSize = new System.Drawing.Size(300, 20);
            this.lblContent.Name = "lblContent";
            this.lblContent.Padding = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.lblContent.Size = new System.Drawing.Size(503, 28);
            this.lblContent.StyleController = this.layoutControl1;
            this.lblContent.TabIndex = 39;
            this.lblContent.Text = "Thông tin tóm tắt:";
            // 
            // lblTimePost
            // 
            this.lblTimePost.Location = new System.Drawing.Point(163, 2);
            this.lblTimePost.Name = "lblTimePost";
            this.lblTimePost.Size = new System.Drawing.Size(116, 13);
            this.lblTimePost.StyleController = this.layoutControl1;
            this.lblTimePost.TabIndex = 4;
            this.lblTimePost.Text = "|  26/06/2024 09:31:32 ";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lblUserName,
            this.layoutControlItem1,
            this.lblStatusLike,
            this.emptySpaceItem1,
            this.layoutControlItem3,
            this.emptySpaceItem3,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(507, 66);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblUserName
            // 
            this.lblUserName.Control = this.lblTimePost;
            this.lblUserName.Location = new System.Drawing.Point(0, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(507, 17);
            this.lblUserName.Text = "0001_tungdt - Đặng Thanh Tùng";
            this.lblUserName.TextSize = new System.Drawing.Size(158, 13);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblContent;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(507, 32);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lblStatusLike
            // 
            this.lblStatusLike.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblStatusLike.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblStatusLike.Control = this.lblNumLike;
            this.lblStatusLike.Location = new System.Drawing.Point(0, 49);
            this.lblStatusLike.Name = "lblStatusLike";
            this.lblStatusLike.Size = new System.Drawing.Size(79, 17);
            this.lblStatusLike.Text = "Thích";
            this.lblStatusLike.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblStatusLike.TextSize = new System.Drawing.Size(50, 10);
            this.lblStatusLike.TextToControlDistance = 5;
            this.lblStatusLike.Click += new System.EventHandler(this.picStatusLike_Click);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(79, 49);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(18, 17);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem3.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem3.Control = this.lblNumRepply;
            this.layoutControlItem3.Location = new System.Drawing.Point(97, 49);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(79, 17);
            this.layoutControlItem3.Text = "Phản hồi";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(50, 10);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(200, 49);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(307, 17);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.panelControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(176, 49);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(24, 17);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // UCItemComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCItemComment";
            this.Size = new System.Drawing.Size(507, 66);
            this.Load += new System.EventHandler(this.UCItemComment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picShowRepply.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblStatusLike)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.LabelControl lblTimePost;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lblUserName;
        private DevExpress.XtraEditors.LabelControl lblContent;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.LabelControl lblNumRepply;
        private DevExpress.XtraEditors.LabelControl lblNumLike;
        private DevExpress.XtraLayout.LayoutControlItem lblStatusLike;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraEditors.PictureEdit picShowRepply;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;

    }
}
