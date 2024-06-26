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
namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    partial class frmMultiIntructonTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiIntructonTime));
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.calendarIntructionTime = new DevExpress.XtraEditors.Controls.CalendarControl();
            this.timeIntruction = new DevExpress.XtraEditors.TimeSpanEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciIntructionTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciIntructionDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calendarIntructionTime.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(291, 267);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(69, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 3;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Controls.Add(this.calendarIntructionTime);
            this.layoutControl1.Controls.Add(this.timeIntruction);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(372, 335);
            this.layoutControl1.TabIndex = 7;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // calendarIntructionTime
            // 
            this.calendarIntructionTime.AutoSize = false;
            this.calendarIntructionTime.CalendarAppearance.DayCellHighlighted.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.calendarIntructionTime.CalendarAppearance.DayCellHighlighted.Options.UseFont = true;
            this.calendarIntructionTime.CalendarAppearance.DayCellSelected.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.calendarIntructionTime.CalendarAppearance.DayCellSelected.Options.UseFont = true;
            this.calendarIntructionTime.CalendarAppearance.DayCellSpecialSelected.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.calendarIntructionTime.CalendarAppearance.DayCellSpecialSelected.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.calendarIntructionTime.CalendarAppearance.DayCellSpecialSelected.Options.UseFont = true;
            this.calendarIntructionTime.CalendarAppearance.DayCellSpecialSelected.Options.UseForeColor = true;
            this.calendarIntructionTime.CalendarTimeProperties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.calendarIntructionTime.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.calendarIntructionTime.DateTime = new System.DateTime(((long)(0)));
            this.calendarIntructionTime.EditValue = null;
            this.calendarIntructionTime.Location = new System.Drawing.Point(107, 36);
            this.calendarIntructionTime.Name = "calendarIntructionTime";
            this.calendarIntructionTime.SelectionBehavior = DevExpress.XtraEditors.Controls.CalendarSelectionBehavior.OutlookStyle;
            this.calendarIntructionTime.SelectionMode = DevExpress.XtraEditors.Repository.CalendarSelectionMode.Multiple;
            this.calendarIntructionTime.Size = new System.Drawing.Size(253, 227);
            this.calendarIntructionTime.StyleController = this.layoutControl1;
            this.calendarIntructionTime.TabIndex = 6;
            // 
            // timeIntruction
            // 
            this.timeIntruction.EditValue = System.TimeSpan.Parse("00:00:00");
            this.timeIntruction.Location = new System.Drawing.Point(107, 12);
            this.timeIntruction.Name = "timeIntruction";
            this.timeIntruction.Properties.AllowEditDays = false;
            this.timeIntruction.Properties.AllowEditSeconds = false;
            this.timeIntruction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeIntruction.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeIntruction.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeIntruction.Properties.Mask.EditMask = "HH:mm";
            this.timeIntruction.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.timeIntruction.Size = new System.Drawing.Size(68, 20);
            this.timeIntruction.StyleController = this.layoutControl1;
            this.timeIntruction.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciIntructionTime,
            this.lciIntructionDate,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(372, 335);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciIntructionTime
            // 
            this.lciIntructionTime.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciIntructionTime.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciIntructionTime.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionTime.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionTime.Control = this.timeIntruction;
            this.lciIntructionTime.Location = new System.Drawing.Point(0, 0);
            this.lciIntructionTime.Name = "lciIntructionTime";
            this.lciIntructionTime.Size = new System.Drawing.Size(167, 24);
            this.lciIntructionTime.Text = "Giờ y lệnh:";
            this.lciIntructionTime.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionTime.TextSize = new System.Drawing.Size(90, 20);
            this.lciIntructionTime.TextToControlDistance = 5;
            // 
            // lciIntructionDate
            // 
            this.lciIntructionDate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.lciIntructionDate.AppearanceItemCaption.Options.UseForeColor = true;
            this.lciIntructionDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciIntructionDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciIntructionDate.Control = this.calendarIntructionTime;
            this.lciIntructionDate.Location = new System.Drawing.Point(0, 24);
            this.lciIntructionDate.Name = "lciIntructionDate";
            this.lciIntructionDate.Size = new System.Drawing.Size(352, 231);
            this.lciIntructionDate.Text = "Ngày y lệnh:";
            this.lciIntructionDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciIntructionDate.TextSize = new System.Drawing.Size(90, 20);
            this.lciIntructionDate.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnChoose;
            this.layoutControlItem3.Location = new System.Drawing.Point(279, 255);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(73, 60);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 255);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(279, 60);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(167, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(185, 24);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmMultiIntructonTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 335);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMultiIntructonTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn nhiều ngày y lệnh";
            this.Load += new System.EventHandler(this.frmMultiIntructonTime_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendarIntructionTime.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciIntructionDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraEditors.TimeSpanEdit timeIntruction;
        private DevExpress.XtraEditors.Controls.CalendarControl calendarIntructionTime;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionTime;
        private DevExpress.XtraLayout.LayoutControlItem lciIntructionDate;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
