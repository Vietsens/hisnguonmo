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
namespace HIS.UC.TreatmentInfo
{
    partial class UCTreatmentInfo
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
            this.lblHeinCardToTime = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinCardFromTime = new DevExpress.XtraEditors.LabelControl();
            this.lblHeinCardNumber = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientTypeName = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutPatientTypeName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutHeinCardNumber = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutHeinCardFromTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutHeinCardToTime = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientTypeName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardFromTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardToTime)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblHeinCardToTime);
            this.layoutControl1.Controls.Add(this.lblHeinCardFromTime);
            this.layoutControl1.Controls.Add(this.lblHeinCardNumber);
            this.layoutControl1.Controls.Add(this.lblPatientTypeName);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(220, 100);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblHeinCardToTime
            // 
            this.lblHeinCardToTime.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCardToTime.Location = new System.Drawing.Point(97, 74);
            this.lblHeinCardToTime.Name = "lblHeinCardToTime";
            this.lblHeinCardToTime.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeinCardToTime.Size = new System.Drawing.Size(121, 20);
            this.lblHeinCardToTime.StyleController = this.layoutControl1;
            this.lblHeinCardToTime.TabIndex = 10;
            // 
            // lblHeinCardFromTime
            // 
            this.lblHeinCardFromTime.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCardFromTime.Location = new System.Drawing.Point(97, 50);
            this.lblHeinCardFromTime.Name = "lblHeinCardFromTime";
            this.lblHeinCardFromTime.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeinCardFromTime.Size = new System.Drawing.Size(121, 20);
            this.lblHeinCardFromTime.StyleController = this.layoutControl1;
            this.lblHeinCardFromTime.TabIndex = 9;
            // 
            // lblHeinCardNumber
            // 
            this.lblHeinCardNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHeinCardNumber.Location = new System.Drawing.Point(97, 26);
            this.lblHeinCardNumber.Name = "lblHeinCardNumber";
            this.lblHeinCardNumber.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblHeinCardNumber.Size = new System.Drawing.Size(121, 20);
            this.lblHeinCardNumber.StyleController = this.layoutControl1;
            this.lblHeinCardNumber.TabIndex = 8;
            // 
            // lblPatientTypeName
            // 
            this.lblPatientTypeName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientTypeName.Location = new System.Drawing.Point(97, 2);
            this.lblPatientTypeName.Name = "lblPatientTypeName";
            this.lblPatientTypeName.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.lblPatientTypeName.Size = new System.Drawing.Size(121, 20);
            this.lblPatientTypeName.StyleController = this.layoutControl1;
            this.lblPatientTypeName.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutPatientTypeName,
            this.layoutHeinCardNumber,
            this.layoutHeinCardFromTime,
            this.layoutHeinCardToTime});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(220, 100);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutPatientTypeName
            // 
            this.layoutPatientTypeName.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutPatientTypeName.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutPatientTypeName.Control = this.lblPatientTypeName;
            this.layoutPatientTypeName.Location = new System.Drawing.Point(0, 0);
            this.layoutPatientTypeName.Name = "layoutPatientTypeName";
            this.layoutPatientTypeName.Size = new System.Drawing.Size(220, 24);
            this.layoutPatientTypeName.Text = "Đối tượng:";
            this.layoutPatientTypeName.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutPatientTypeName.TextSize = new System.Drawing.Size(90, 20);
            this.layoutPatientTypeName.TextToControlDistance = 5;
            // 
            // layoutHeinCardNumber
            // 
            this.layoutHeinCardNumber.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutHeinCardNumber.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutHeinCardNumber.Control = this.lblHeinCardNumber;
            this.layoutHeinCardNumber.Location = new System.Drawing.Point(0, 24);
            this.layoutHeinCardNumber.Name = "layoutHeinCardNumber";
            this.layoutHeinCardNumber.Size = new System.Drawing.Size(220, 24);
            this.layoutHeinCardNumber.Text = "Số thẻ Bhyt:";
            this.layoutHeinCardNumber.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutHeinCardNumber.TextSize = new System.Drawing.Size(90, 20);
            this.layoutHeinCardNumber.TextToControlDistance = 5;
            // 
            // layoutHeinCardFromTime
            // 
            this.layoutHeinCardFromTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutHeinCardFromTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutHeinCardFromTime.Control = this.lblHeinCardFromTime;
            this.layoutHeinCardFromTime.Location = new System.Drawing.Point(0, 48);
            this.layoutHeinCardFromTime.Name = "layoutHeinCardFromTime";
            this.layoutHeinCardFromTime.Size = new System.Drawing.Size(220, 24);
            this.layoutHeinCardFromTime.Text = "Hạn thẻ từ:";
            this.layoutHeinCardFromTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutHeinCardFromTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutHeinCardFromTime.TextToControlDistance = 5;
            // 
            // layoutHeinCardToTime
            // 
            this.layoutHeinCardToTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutHeinCardToTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutHeinCardToTime.Control = this.lblHeinCardToTime;
            this.layoutHeinCardToTime.Location = new System.Drawing.Point(0, 72);
            this.layoutHeinCardToTime.Name = "layoutHeinCardToTime";
            this.layoutHeinCardToTime.Size = new System.Drawing.Size(220, 28);
            this.layoutHeinCardToTime.Text = "Hạn thẻ đến:";
            this.layoutHeinCardToTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutHeinCardToTime.TextSize = new System.Drawing.Size(90, 20);
            this.layoutHeinCardToTime.TextToControlDistance = 5;
            // 
            // UCTreatmentInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCTreatmentInfo";
            this.Size = new System.Drawing.Size(220, 100);
            this.Load += new System.EventHandler(this.UCTreatmentInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutPatientTypeName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardFromTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinCardToTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblPatientTypeName;
        private DevExpress.XtraLayout.LayoutControlItem layoutPatientTypeName;
        private DevExpress.XtraEditors.LabelControl lblHeinCardToTime;
        private DevExpress.XtraEditors.LabelControl lblHeinCardFromTime;
        private DevExpress.XtraEditors.LabelControl lblHeinCardNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutHeinCardNumber;
        private DevExpress.XtraLayout.LayoutControlItem layoutHeinCardFromTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutHeinCardToTime;
    }
}
