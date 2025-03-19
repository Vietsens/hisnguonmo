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
namespace HIS.UC.FormType.MediStockSttFilterCheckBoxGroup
{
    public partial class UCMediStockSttFilterCheckBoxGroup
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
            this.lblTitleName = new DevExpress.XtraEditors.LabelControl();
            this.radioInventory = new DevExpress.XtraEditors.CheckEdit();
            this.radioExp = new DevExpress.XtraEditors.CheckEdit();
            this.radioImp = new DevExpress.XtraEditors.CheckEdit();
            this.radioAll = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciTitleName = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioInventory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioExp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioImp.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitleName)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblTitleName);
            this.layoutControl1.Controls.Add(this.radioInventory);
            this.layoutControl1.Controls.Add(this.radioExp);
            this.layoutControl1.Controls.Add(this.radioImp);
            this.layoutControl1.Controls.Add(this.radioAll);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 25);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblTitleName
            // 
            this.lblTitleName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTitleName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTitleName.Location = new System.Drawing.Point(2, 2);
            this.lblTitleName.Name = "lblTitleName";
            this.lblTitleName.Size = new System.Drawing.Size(76, 21);
            this.lblTitleName.StyleController = this.layoutControl1;
            this.lblTitleName.TabIndex = 24;
            this.lblTitleName.Text = " ";
            // 
            // radioInventory
            // 
            this.radioInventory.Location = new System.Drawing.Point(504, 2);
            this.radioInventory.Name = "radioInventory";
            this.radioInventory.Properties.AutoWidth = true;
            this.radioInventory.Properties.Caption = "Tồn";
            this.radioInventory.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioInventory.Properties.RadioGroupIndex = 1;
            this.radioInventory.Size = new System.Drawing.Size(40, 19);
            this.radioInventory.StyleController = this.layoutControl1;
            this.radioInventory.TabIndex = 4;
            this.radioInventory.TabStop = false;
            // 
            // radioExp
            // 
            this.radioExp.Location = new System.Drawing.Point(361, 2);
            this.radioExp.Name = "radioExp";
            this.radioExp.Properties.AutoWidth = true;
            this.radioExp.Properties.Caption = "Xuất";
            this.radioExp.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioExp.Properties.RadioGroupIndex = 1;
            this.radioExp.Size = new System.Drawing.Size(44, 19);
            this.radioExp.StyleController = this.layoutControl1;
            this.radioExp.TabIndex = 3;
            this.radioExp.TabStop = false;
            // 
            // radioImp
            // 
            this.radioImp.Location = new System.Drawing.Point(222, 2);
            this.radioImp.Name = "radioImp";
            this.radioImp.Properties.AutoWidth = true;
            this.radioImp.Properties.Caption = "Nhập";
            this.radioImp.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioImp.Properties.RadioGroupIndex = 1;
            this.radioImp.Size = new System.Drawing.Size(47, 19);
            this.radioImp.StyleController = this.layoutControl1;
            this.radioImp.TabIndex = 2;
            this.radioImp.TabStop = false;
            // 
            // radioAll
            // 
            this.radioAll.EditValue = true;
            this.radioAll.Location = new System.Drawing.Point(92, 2);
            this.radioAll.Name = "radioAll";
            this.radioAll.Properties.AutoWidth = true;
            this.radioAll.Properties.Caption = "Tất cả";
            this.radioAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioAll.Properties.RadioGroupIndex = 1;
            this.radioAll.Size = new System.Drawing.Size(52, 19);
            this.radioAll.StyleController = this.layoutControl1;
            this.radioAll.TabIndex = 1;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4,
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.layoutControlItem1,
            this.lciTitleName});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.radioAll;
            this.layoutControlItem4.Location = new System.Drawing.Point(90, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(130, 25);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.radioImp;
            this.layoutControlItem3.Location = new System.Drawing.Point(220, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(139, 25);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.radioExp;
            this.layoutControlItem2.Location = new System.Drawing.Point(359, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(143, 25);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.radioInventory;
            this.layoutControlItem1.Location = new System.Drawing.Point(502, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(158, 25);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciTitleName
            // 
            this.lciTitleName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciTitleName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciTitleName.Control = this.lblTitleName;
            this.lciTitleName.Location = new System.Drawing.Point(0, 0);
            this.lciTitleName.MaxSize = new System.Drawing.Size(500, 100);
            this.lciTitleName.MinSize = new System.Drawing.Size(10, 10);
            this.lciTitleName.Name = "lciTitleName";
            this.lciTitleName.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 12, 2, 2);
            this.lciTitleName.Size = new System.Drawing.Size(90, 25);
            this.lciTitleName.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciTitleName.Text = " ";
            this.lciTitleName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciTitleName.TextSize = new System.Drawing.Size(0, 0);
            this.lciTitleName.TextToControlDistance = 0;
            this.lciTitleName.TextVisible = false;
            // 
            // UCMediStockSttFilterCheckBoxGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCMediStockSttFilterCheckBoxGroup";
            this.Size = new System.Drawing.Size(660, 25);
            this.Load += new System.EventHandler(this.UCMediStockSttFilterCheckBoxGroup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radioInventory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioExp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioImp.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciTitleName)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.CheckEdit radioInventory;
        private DevExpress.XtraEditors.CheckEdit radioExp;
        private DevExpress.XtraEditors.CheckEdit radioImp;
        private DevExpress.XtraEditors.CheckEdit radioAll;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.LabelControl lblTitleName;
        private DevExpress.XtraLayout.LayoutControlItem lciTitleName;
    }
}
