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
namespace HIS.Desktop.Plugins.AssignService.AssignService
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnChoose = new DevExpress.XtraEditors.SimpleButton();
            this.calendarIntructionTime = new DevExpress.XtraEditors.Controls.CalendarControl();
            this.lblCalendaInput = new DevExpress.XtraEditors.LabelControl();
            this.lblTimeInput = new DevExpress.XtraEditors.LabelControl();
            this.timeIntruction = new DevExpress.XtraEditors.TimeSpanEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.calendarIntructionTime.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnChoose);
            this.layoutControl1.Controls.Add(this.calendarIntructionTime);
            this.layoutControl1.Controls.Add(this.lblCalendaInput);
            this.layoutControl1.Controls.Add(this.lblTimeInput);
            this.layoutControl1.Controls.Add(this.timeIntruction);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(402, 125, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(348, 326);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(251, 267);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(85, 22);
            this.btnChoose.StyleController = this.layoutControl1;
            this.btnChoose.TabIndex = 10;
            this.btnChoose.Text = "Chọn";
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
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
            this.calendarIntructionTime.Location = new System.Drawing.Point(103, 36);
            this.calendarIntructionTime.Name = "calendarIntructionTime";
            this.calendarIntructionTime.SelectionBehavior = DevExpress.XtraEditors.Controls.CalendarSelectionBehavior.OutlookStyle;
            this.calendarIntructionTime.SelectionMode = DevExpress.XtraEditors.Repository.CalendarSelectionMode.Multiple;
            this.calendarIntructionTime.Size = new System.Drawing.Size(233, 227);
            this.calendarIntructionTime.StyleController = this.layoutControl1;
            this.calendarIntructionTime.TabIndex = 9;
            // 
            // lblCalendaInput
            // 
            this.lblCalendaInput.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblCalendaInput.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCalendaInput.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCalendaInput.Location = new System.Drawing.Point(12, 36);
            this.lblCalendaInput.Name = "lblCalendaInput";
            this.lblCalendaInput.Size = new System.Drawing.Size(87, 13);
            this.lblCalendaInput.StyleController = this.layoutControl1;
            this.lblCalendaInput.TabIndex = 8;
            this.lblCalendaInput.Text = "Ngày y lệnh:";
            // 
            // lblTimeInput
            // 
            this.lblTimeInput.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblTimeInput.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblTimeInput.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTimeInput.Location = new System.Drawing.Point(12, 12);
            this.lblTimeInput.Name = "lblTimeInput";
            this.lblTimeInput.Size = new System.Drawing.Size(87, 13);
            this.lblTimeInput.StyleController = this.layoutControl1;
            this.lblTimeInput.TabIndex = 7;
            this.lblTimeInput.Text = "Giờ y lệnh:";
            // 
            // timeIntruction
            // 
            this.timeIntruction.EditValue = System.TimeSpan.Parse("00:00:00");
            this.timeIntruction.Location = new System.Drawing.Point(103, 12);
            this.timeIntruction.Name = "timeIntruction";
            this.timeIntruction.Properties.AllowEditDays = false;
            this.timeIntruction.Properties.AllowEditSeconds = false;
            this.timeIntruction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.timeIntruction.Properties.DisplayFormat.FormatString = "HH:mm";
            this.timeIntruction.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.timeIntruction.Properties.Mask.EditMask = "HH:mm";
            this.timeIntruction.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.timeIntruction.Size = new System.Drawing.Size(233, 20);
            this.timeIntruction.StyleController = this.layoutControl1;
            this.timeIntruction.TabIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(348, 326);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.lblTimeInput;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(91, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.timeIntruction;
            this.layoutControlItem2.Location = new System.Drawing.Point(91, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(237, 24);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.calendarIntructionTime;
            this.layoutControlItem3.Location = new System.Drawing.Point(91, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(237, 231);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.lblCalendaInput;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(91, 231);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnChoose;
            this.layoutControlItem5.Location = new System.Drawing.Point(239, 255);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(89, 51);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 255);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(239, 51);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmMultiIntructonTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 326);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMultiIntructonTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chọn nhiều ngày y lệnh";
            this.Load += new System.EventHandler(this.frmMultiIntructonTime1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.calendarIntructionTime.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeIntruction.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.LabelControl lblTimeInput;
        private DevExpress.XtraEditors.TimeSpanEdit timeIntruction;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.Controls.CalendarControl calendarIntructionTime;
        private DevExpress.XtraEditors.LabelControl lblCalendaInput;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SimpleButton btnChoose;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;

    }
}
