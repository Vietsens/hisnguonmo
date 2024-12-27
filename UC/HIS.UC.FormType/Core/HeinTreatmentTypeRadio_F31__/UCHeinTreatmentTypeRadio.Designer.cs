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
namespace HIS.UC.FormType.Core.HeinTreatmentTypeRadio
{
    partial class UCHeinTreatmentTypeRadio
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.radioAll = new DevExpress.XtraEditors.CheckEdit();
            this.layoutHeinTreatmentType = new DevExpress.XtraLayout.LayoutControlItem();
            this.radioExam = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.radioTreat = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinTreatmentType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioExam.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioTreat.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.radioTreat);
            this.layoutControl1.Controls.Add(this.radioExam);
            this.layoutControl1.Controls.Add(this.radioAll);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(660, 25);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutHeinTreatmentType,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(660, 25);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // radioAll
            // 
            this.radioAll.EditValue = true;
            this.radioAll.Location = new System.Drawing.Point(97, 2);
            this.radioAll.Name = "radioAll";
            this.radioAll.Properties.Caption = "Tất cả";
            this.radioAll.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioAll.Properties.RadioGroupIndex = 1;
            this.radioAll.Size = new System.Drawing.Size(211, 19);
            this.radioAll.StyleController = this.layoutControl1;
            this.radioAll.TabIndex = 4;
            // 
            // layoutHeinTreatmentType
            // 
            this.layoutHeinTreatmentType.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutHeinTreatmentType.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutHeinTreatmentType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutHeinTreatmentType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutHeinTreatmentType.Control = this.radioAll;
            this.layoutHeinTreatmentType.Location = new System.Drawing.Point(0, 0);
            this.layoutHeinTreatmentType.Name = "layoutHeinTreatmentType";
            this.layoutHeinTreatmentType.Size = new System.Drawing.Size(310, 25);
            this.layoutHeinTreatmentType.Text = "Đối tượng:";
            this.layoutHeinTreatmentType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutHeinTreatmentType.TextSize = new System.Drawing.Size(90, 20);
            this.layoutHeinTreatmentType.TextToControlDistance = 5;
            // 
            // radioExam
            // 
            this.radioExam.Location = new System.Drawing.Point(312, 2);
            this.radioExam.Name = "radioExam";
            this.radioExam.Properties.Caption = "Ngoại trú";
            this.radioExam.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioExam.Properties.RadioGroupIndex = 1;
            this.radioExam.Size = new System.Drawing.Size(216, 19);
            this.radioExam.StyleController = this.layoutControl1;
            this.radioExam.TabIndex = 5;
            this.radioExam.TabStop = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.radioExam;
            this.layoutControlItem2.Location = new System.Drawing.Point(310, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(220, 25);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // radioTreat
            // 
            this.radioTreat.Location = new System.Drawing.Point(532, 2);
            this.radioTreat.Name = "radioTreat";
            this.radioTreat.Properties.Caption = "Nội trú";
            this.radioTreat.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.radioTreat.Properties.RadioGroupIndex = 1;
            this.radioTreat.Size = new System.Drawing.Size(126, 19);
            this.radioTreat.StyleController = this.layoutControl1;
            this.radioTreat.TabIndex = 6;
            this.radioTreat.TabStop = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.radioTreat;
            this.layoutControlItem3.Location = new System.Drawing.Point(530, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(130, 25);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCHeinTreatmentTypeRadio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCHeinTreatmentTypeRadio";
            this.Size = new System.Drawing.Size(660, 25);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutHeinTreatmentType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioExam.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioTreat.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.CheckEdit radioTreat;
        private DevExpress.XtraEditors.CheckEdit radioExam;
        private DevExpress.XtraEditors.CheckEdit radioAll;
        private DevExpress.XtraLayout.LayoutControlItem layoutHeinTreatmentType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
