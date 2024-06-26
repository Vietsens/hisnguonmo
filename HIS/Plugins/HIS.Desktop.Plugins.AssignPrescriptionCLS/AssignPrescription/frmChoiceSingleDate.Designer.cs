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
namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    partial class frmChoiceSingleDate
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
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.timeIntruction = new DevExpress.XtraEditors.TimeSpanEdit();
            this.dtIntructionTime = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciFordtIntructionTime = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordtIntructionTime)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.timeIntruction);
            this.layoutControl1.Controls.Add(this.dtIntructionTime);
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(326, 71);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(230, 36);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(84, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 11;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.lciFordtIntructionTime,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(326, 71);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.btnChoose;
            this.layoutControlItem1.Location = new System.Drawing.Point(218, 24);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(88, 27);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(218, 27);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // timeIntruction
            // 
            this.timeIntruction.EditValue = System.TimeSpan.Parse("00:00:00");
            this.timeIntruction.EnterMoveNextControl = true;
            this.timeIntruction.Location = new System.Drawing.Point(230, 12);
            this.timeIntruction.Name = "timeIntruction";
            this.timeIntruction.Properties.AllowEditDays = false;
            this.timeIntruction.Properties.AllowEditSeconds = false;
            this.timeIntruction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeIntruction.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeIntruction.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeIntruction.Properties.Mask.EditMask = "HH:mm";
            this.timeIntruction.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.timeIntruction.Size = new System.Drawing.Size(84, 20);
            this.timeIntruction.StyleController = this.layoutControl1;
            this.timeIntruction.TabIndex = 13;
            // 
            // dtIntructionTime
            // 
            this.dtIntructionTime.EditValue = null;
            this.dtIntructionTime.Location = new System.Drawing.Point(107, 12);
            this.dtIntructionTime.Name = "dtIntructionTime";
            this.dtIntructionTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtIntructionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtIntructionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtIntructionTime.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.dtIntructionTime.Size = new System.Drawing.Size(119, 20);
            this.dtIntructionTime.StyleController = this.layoutControl1;
            this.dtIntructionTime.TabIndex = 14;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.timeIntruction;
            this.layoutControlItem2.Location = new System.Drawing.Point(218, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(88, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // lciFordtIntructionTime
            // 
            this.lciFordtIntructionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciFordtIntructionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciFordtIntructionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciFordtIntructionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciFordtIntructionTime.Control = this.dtIntructionTime;
            this.lciFordtIntructionTime.Location = new System.Drawing.Point(0, 0);
            this.lciFordtIntructionTime.Name = "lciFordtIntructionTime";
            this.lciFordtIntructionTime.Size = new System.Drawing.Size(218, 24);
            this.lciFordtIntructionTime.Text = "Ngày y lệnh:";
            this.lciFordtIntructionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciFordtIntructionTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciFordtIntructionTime.TextToControlDistance = 5;
            // 
            // frmChoiceSingleDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 71);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmChoiceSingleDate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn ngày";
            this.Load += new System.EventHandler(this.frmChoiceSingleDate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtIntructionTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciFordtIntructionTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        internal DevExpress.XtraEditors.TimeSpanEdit timeIntruction;
        internal DevExpress.XtraEditors.DateEdit dtIntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciFordtIntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
