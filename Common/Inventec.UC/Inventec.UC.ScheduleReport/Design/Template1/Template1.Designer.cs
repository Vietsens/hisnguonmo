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
namespace Inventec.UC.ScheduleReport.Design.Template1
{
    partial class Template1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Template1));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.checkPeriod = new DevExpress.XtraEditors.CheckEdit();
            this.checkOnce = new DevExpress.XtraEditors.CheckEdit();
            this.checkDaybyDay = new DevExpress.XtraEditors.CheckEdit();
            this.checkWeekByWeek = new DevExpress.XtraEditors.CheckEdit();
            this.checkMonthByMonth = new DevExpress.XtraEditors.CheckEdit();
            this.checkLastDayOfMonth = new DevExpress.XtraEditors.CheckEdit();
            this.lblHour = new DevExpress.XtraEditors.LabelControl();
            this.spinHour = new DevExpress.XtraEditors.SpinEdit();
            this.lblMinutes = new DevExpress.XtraEditors.LabelControl();
            this.spinMinutes = new DevExpress.XtraEditors.SpinEdit();
            this.lblExecuteDate = new DevExpress.XtraEditors.LabelControl();
            this.dtExecuteDate = new DevExpress.XtraEditors.DateEdit();
            this.lblEnd = new DevExpress.XtraEditors.LabelControl();
            this.dtEndDate = new DevExpress.XtraEditors.DateEdit();
            this.btnSchedule = new DevExpress.XtraEditors.SimpleButton();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkPeriod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkOnce.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkDaybyDay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkWeekByWeek.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMonthByMonth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkLastDayOfMonth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHour.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMinutes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExecuteDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExecuteDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.MaxItemId = 0;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1160, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 40);
            this.barDockControlBottom.Size = new System.Drawing.Size(1160, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 40);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1160, 0);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 40);
            // 
            // checkPeriod
            // 
            this.checkPeriod.AutoSizeInLayoutControl = true;
            this.checkPeriod.Location = new System.Drawing.Point(5, 10);
            this.checkPeriod.MenuManager = this.barManager1;
            this.checkPeriod.Name = "checkPeriod";
            this.checkPeriod.Properties.Caption = "Lịch chạy:";
            this.checkPeriod.Size = new System.Drawing.Size(75, 19);
            this.checkPeriod.TabIndex = 4;
            this.checkPeriod.CheckedChanged += new System.EventHandler(this.checkPeriod_CheckedChanged);
            this.checkPeriod.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.checkPeriod_PreviewKeyDown);
            // 
            // checkOnce
            // 
            this.checkOnce.AutoSizeInLayoutControl = true;
            this.checkOnce.Location = new System.Drawing.Point(85, 10);
            this.checkOnce.MenuManager = this.barManager1;
            this.checkOnce.Name = "checkOnce";
            this.checkOnce.Properties.Caption = "Một lần";
            this.checkOnce.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkOnce.Properties.RadioGroupIndex = 1;
            this.checkOnce.Size = new System.Drawing.Size(75, 19);
            this.checkOnce.TabIndex = 5;
            this.checkOnce.TabStop = false;
            this.checkOnce.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkOnce_KeyDown);
            // 
            // checkDaybyDay
            // 
            this.checkDaybyDay.AutoSizeInLayoutControl = true;
            this.checkDaybyDay.Location = new System.Drawing.Point(165, 10);
            this.checkDaybyDay.MenuManager = this.barManager1;
            this.checkDaybyDay.Name = "checkDaybyDay";
            this.checkDaybyDay.Properties.Caption = "Hằng ngày";
            this.checkDaybyDay.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkDaybyDay.Properties.RadioGroupIndex = 1;
            this.checkDaybyDay.Size = new System.Drawing.Size(75, 19);
            this.checkDaybyDay.TabIndex = 6;
            this.checkDaybyDay.TabStop = false;
            this.checkDaybyDay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkDaybyDay_KeyDown);
            // 
            // checkWeekByWeek
            // 
            this.checkWeekByWeek.Location = new System.Drawing.Point(250, 10);
            this.checkWeekByWeek.MenuManager = this.barManager1;
            this.checkWeekByWeek.Name = "checkWeekByWeek";
            this.checkWeekByWeek.Properties.Caption = "Hằng tuần";
            this.checkWeekByWeek.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkWeekByWeek.Properties.RadioGroupIndex = 1;
            this.checkWeekByWeek.Size = new System.Drawing.Size(75, 19);
            this.checkWeekByWeek.TabIndex = 7;
            this.checkWeekByWeek.TabStop = false;
            this.checkWeekByWeek.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkWeekByWeek_KeyDown);
            // 
            // checkMonthByMonth
            // 
            this.checkMonthByMonth.Location = new System.Drawing.Point(335, 10);
            this.checkMonthByMonth.MenuManager = this.barManager1;
            this.checkMonthByMonth.Name = "checkMonthByMonth";
            this.checkMonthByMonth.Properties.Caption = "Hằng tháng";
            this.checkMonthByMonth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkMonthByMonth.Properties.RadioGroupIndex = 1;
            this.checkMonthByMonth.Size = new System.Drawing.Size(75, 19);
            this.checkMonthByMonth.TabIndex = 8;
            this.checkMonthByMonth.TabStop = false;
            this.checkMonthByMonth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkMonthByMonth_KeyDown);
            // 
            // checkLastDayOfMonth
            // 
            this.checkLastDayOfMonth.Location = new System.Drawing.Point(420, 10);
            this.checkLastDayOfMonth.MenuManager = this.barManager1;
            this.checkLastDayOfMonth.Name = "checkLastDayOfMonth";
            this.checkLastDayOfMonth.Properties.Caption = "Cuối tháng";
            this.checkLastDayOfMonth.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
            this.checkLastDayOfMonth.Properties.RadioGroupIndex = 1;
            this.checkLastDayOfMonth.Size = new System.Drawing.Size(75, 19);
            this.checkLastDayOfMonth.TabIndex = 9;
            this.checkLastDayOfMonth.TabStop = false;
            this.checkLastDayOfMonth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkLastDayOfMonth_KeyDown);
            // 
            // lblHour
            // 
            this.lblHour.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblHour.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblHour.Location = new System.Drawing.Point(515, 13);
            this.lblHour.Name = "lblHour";
            this.lblHour.Size = new System.Drawing.Size(30, 13);
            this.lblHour.TabIndex = 10;
            this.lblHour.Text = "Giờ:";
            // 
            // spinHour
            // 
            this.spinHour.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinHour.Location = new System.Drawing.Point(550, 10);
            this.spinHour.MenuManager = this.barManager1;
            this.spinHour.Name = "spinHour";
            this.spinHour.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinHour.Properties.MaxLength = 2;
            this.spinHour.Properties.MaxValue = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.spinHour.Size = new System.Drawing.Size(45, 20);
            this.spinHour.TabIndex = 11;
            this.spinHour.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinHour_PreviewKeyDown);
            // 
            // lblMinutes
            // 
            this.lblMinutes.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblMinutes.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblMinutes.Location = new System.Drawing.Point(600, 13);
            this.lblMinutes.Name = "lblMinutes";
            this.lblMinutes.Size = new System.Drawing.Size(40, 13);
            this.lblMinutes.TabIndex = 12;
            this.lblMinutes.Text = "Phút:";
            // 
            // spinMinutes
            // 
            this.spinMinutes.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.spinMinutes.Location = new System.Drawing.Point(645, 10);
            this.spinMinutes.MenuManager = this.barManager1;
            this.spinMinutes.Name = "spinMinutes";
            this.spinMinutes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.spinMinutes.Properties.MaxLength = 2;
            this.spinMinutes.Properties.MaxValue = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.spinMinutes.Size = new System.Drawing.Size(45, 20);
            this.spinMinutes.TabIndex = 13;
            this.spinMinutes.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.spinMinutes_PreviewKeyDown);
            // 
            // lblExecuteDate
            // 
            this.lblExecuteDate.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblExecuteDate.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblExecuteDate.Location = new System.Drawing.Point(690, 13);
            this.lblExecuteDate.Name = "lblExecuteDate";
            this.lblExecuteDate.Size = new System.Drawing.Size(40, 13);
            this.lblExecuteDate.TabIndex = 14;
            this.lblExecuteDate.Text = "Ngày:";
            // 
            // dtExecuteDate
            // 
            this.dtExecuteDate.EditValue = null;
            this.dtExecuteDate.Location = new System.Drawing.Point(735, 10);
            this.dtExecuteDate.MenuManager = this.barManager1;
            this.dtExecuteDate.Name = "dtExecuteDate";
            this.dtExecuteDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExecuteDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExecuteDate.Size = new System.Drawing.Size(120, 20);
            this.dtExecuteDate.TabIndex = 15;
            this.dtExecuteDate.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExecuteDate_Closed);
            // 
            // lblEnd
            // 
            this.lblEnd.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblEnd.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEnd.Location = new System.Drawing.Point(860, 13);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(50, 13);
            this.lblEnd.TabIndex = 16;
            this.lblEnd.Text = "Kết thúc:";
            // 
            // dtEndDate
            // 
            this.dtEndDate.EditValue = null;
            this.dtEndDate.Location = new System.Drawing.Point(915, 10);
            this.dtEndDate.MenuManager = this.barManager1;
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndDate.Size = new System.Drawing.Size(120, 20);
            this.dtEndDate.TabIndex = 17;
            this.dtEndDate.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtEndDate_Closed);
            // 
            // btnSchedule
            // 
            this.btnSchedule.Image = ((System.Drawing.Image)(resources.GetObject("btnSchedule.Image")));
            this.btnSchedule.Location = new System.Drawing.Point(1040, 4);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(120, 35);
            this.btnSchedule.TabIndex = 18;
            this.btnSchedule.Text = "Đặt lịch (Ctrl C)";
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // Template1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSchedule);
            this.Controls.Add(this.dtEndDate);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.dtExecuteDate);
            this.Controls.Add(this.lblExecuteDate);
            this.Controls.Add(this.spinMinutes);
            this.Controls.Add(this.lblMinutes);
            this.Controls.Add(this.spinHour);
            this.Controls.Add(this.lblHour);
            this.Controls.Add(this.checkLastDayOfMonth);
            this.Controls.Add(this.checkMonthByMonth);
            this.Controls.Add(this.checkWeekByWeek);
            this.Controls.Add(this.checkDaybyDay);
            this.Controls.Add(this.checkOnce);
            this.Controls.Add(this.checkPeriod);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "Template1";
            this.Size = new System.Drawing.Size(1160, 40);
            this.Load += new System.EventHandler(this.Template1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkPeriod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkOnce.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkDaybyDay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkWeekByWeek.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMonthByMonth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkLastDayOfMonth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinHour.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMinutes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExecuteDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExecuteDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.CheckEdit checkPeriod;
        private DevExpress.XtraEditors.CheckEdit checkLastDayOfMonth;
        private DevExpress.XtraEditors.CheckEdit checkMonthByMonth;
        private DevExpress.XtraEditors.CheckEdit checkWeekByWeek;
        private DevExpress.XtraEditors.CheckEdit checkDaybyDay;
        private DevExpress.XtraEditors.CheckEdit checkOnce;
        private DevExpress.XtraEditors.SimpleButton btnSchedule;
        private DevExpress.XtraEditors.DateEdit dtEndDate;
        private DevExpress.XtraEditors.LabelControl lblEnd;
        private DevExpress.XtraEditors.DateEdit dtExecuteDate;
        private DevExpress.XtraEditors.LabelControl lblExecuteDate;
        private DevExpress.XtraEditors.SpinEdit spinMinutes;
        private DevExpress.XtraEditors.LabelControl lblMinutes;
        private DevExpress.XtraEditors.SpinEdit spinHour;
        private DevExpress.XtraEditors.LabelControl lblHour;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;

    }
}
