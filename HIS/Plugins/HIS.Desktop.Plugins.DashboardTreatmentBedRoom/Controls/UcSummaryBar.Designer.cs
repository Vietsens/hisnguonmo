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
namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    partial class UcSummaryBar
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.icoPatients = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblPatientsTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblInPatientCurrent = new DevExpress.XtraEditors.LabelControl();
            this.lblInPatientCurrentCap = new DevExpress.XtraEditors.LabelControl();
            this.lblWaitIn = new DevExpress.XtraEditors.LabelControl();
            this.lblWaitInCap = new DevExpress.XtraEditors.LabelControl();
            this.lblInToday = new DevExpress.XtraEditors.LabelControl();
            this.lblInTodayCap = new DevExpress.XtraEditors.LabelControl();
            this.lblOutToday = new DevExpress.XtraEditors.LabelControl();
            this.lblOutTodayCap = new DevExpress.XtraEditors.LabelControl();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.icoBeds = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblBedsTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblBedTotal = new DevExpress.XtraEditors.LabelControl();
            this.lblBedTotalCap = new DevExpress.XtraEditors.LabelControl();
            this.lblBedUsed = new DevExpress.XtraEditors.LabelControl();
            this.lblBedUsedCap = new DevExpress.XtraEditors.LabelControl();
            this.lblBedEmpty = new DevExpress.XtraEditors.LabelControl();
            this.lblBedEmptyCap = new DevExpress.XtraEditors.LabelControl();
            this.icoCare = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblCareTitle = new DevExpress.XtraEditors.LabelControl();
            this.lblCare1 = new DevExpress.XtraEditors.LabelControl();
            this.lblCare1Cap = new DevExpress.XtraEditors.LabelControl();
            this.lblCare2 = new DevExpress.XtraEditors.LabelControl();
            this.lblCare2Cap = new DevExpress.XtraEditors.LabelControl();
            this.lblCare3 = new DevExpress.XtraEditors.LabelControl();
            this.lblCare3Cap = new DevExpress.XtraEditors.LabelControl();
            this.lblSpecial = new DevExpress.XtraEditors.LabelControl();
            this.lblSpecialCap = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            //
            // icoPatients
            //
            this.icoPatients.BackColor = System.Drawing.Color.Transparent;
            this.icoPatients.IconColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.icoPatients.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Users;
            this.icoPatients.Location = new System.Drawing.Point(16, 13);
            this.icoPatients.Name = "icoPatients";
            this.icoPatients.Size = new System.Drawing.Size(15, 15);
            this.icoPatients.TabIndex = 0;
            //
            // lblPatientsTitle
            //
            this.lblPatientsTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPatientsTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblPatientsTitle.Appearance.Options.UseFont = true;
            this.lblPatientsTitle.Appearance.Options.UseForeColor = true;
            this.lblPatientsTitle.Location = new System.Drawing.Point(38, 12);
            this.lblPatientsTitle.Name = "lblPatientsTitle";
            this.lblPatientsTitle.Size = new System.Drawing.Size(69, 16);
            this.lblPatientsTitle.TabIndex = 1;
            this.lblPatientsTitle.Text = "Người bệnh";
            //
            // lblInPatientCurrent
            //
            this.lblInPatientCurrent.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblInPatientCurrent.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblInPatientCurrent.Appearance.Options.UseFont = true;
            this.lblInPatientCurrent.Appearance.Options.UseForeColor = true;
            this.lblInPatientCurrent.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInPatientCurrent.Location = new System.Drawing.Point(16, 42);
            this.lblInPatientCurrent.Name = "lblInPatientCurrent";
            this.lblInPatientCurrent.Size = new System.Drawing.Size(124, 30);
            this.lblInPatientCurrent.TabIndex = 2;
            this.lblInPatientCurrent.Text = "0";
            //
            // lblInPatientCurrentCap
            //
            this.lblInPatientCurrentCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblInPatientCurrentCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblInPatientCurrentCap.Appearance.Options.UseFont = true;
            this.lblInPatientCurrentCap.Appearance.Options.UseForeColor = true;
            this.lblInPatientCurrentCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblInPatientCurrentCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInPatientCurrentCap.Location = new System.Drawing.Point(16, 72);
            this.lblInPatientCurrentCap.Name = "lblInPatientCurrentCap";
            this.lblInPatientCurrentCap.Size = new System.Drawing.Size(124, 16);
            this.lblInPatientCurrentCap.TabIndex = 3;
            this.lblInPatientCurrentCap.Text = "Nội trú hiện tại";
            //
            // lblWaitIn
            //
            this.lblWaitIn.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblWaitIn.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblWaitIn.Appearance.Options.UseFont = true;
            this.lblWaitIn.Appearance.Options.UseForeColor = true;
            this.lblWaitIn.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWaitIn.Location = new System.Drawing.Point(140, 42);
            this.lblWaitIn.Name = "lblWaitIn";
            this.lblWaitIn.Size = new System.Drawing.Size(124, 30);
            this.lblWaitIn.TabIndex = 4;
            this.lblWaitIn.Text = "0";
            //
            // lblWaitInCap
            //
            this.lblWaitInCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblWaitInCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblWaitInCap.Appearance.Options.UseFont = true;
            this.lblWaitInCap.Appearance.Options.UseForeColor = true;
            this.lblWaitInCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblWaitInCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWaitInCap.Location = new System.Drawing.Point(140, 72);
            this.lblWaitInCap.Name = "lblWaitInCap";
            this.lblWaitInCap.Size = new System.Drawing.Size(124, 16);
            this.lblWaitInCap.TabIndex = 5;
            this.lblWaitInCap.Text = "Chờ nhập viện";
            //
            // lblInToday
            //
            this.lblInToday.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblInToday.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblInToday.Appearance.Options.UseFont = true;
            this.lblInToday.Appearance.Options.UseForeColor = true;
            this.lblInToday.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInToday.Location = new System.Drawing.Point(264, 42);
            this.lblInToday.Name = "lblInToday";
            this.lblInToday.Size = new System.Drawing.Size(124, 30);
            this.lblInToday.TabIndex = 6;
            this.lblInToday.Text = "0";
            //
            // lblInTodayCap
            //
            this.lblInTodayCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblInTodayCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblInTodayCap.Appearance.Options.UseFont = true;
            this.lblInTodayCap.Appearance.Options.UseForeColor = true;
            this.lblInTodayCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblInTodayCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblInTodayCap.Location = new System.Drawing.Point(264, 72);
            this.lblInTodayCap.Name = "lblInTodayCap";
            this.lblInTodayCap.Size = new System.Drawing.Size(124, 16);
            this.lblInTodayCap.TabIndex = 7;
            this.lblInTodayCap.Text = "Nhập viện";
            //
            // lblOutToday
            //
            this.lblOutToday.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblOutToday.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblOutToday.Appearance.Options.UseFont = true;
            this.lblOutToday.Appearance.Options.UseForeColor = true;
            this.lblOutToday.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOutToday.Location = new System.Drawing.Point(388, 42);
            this.lblOutToday.Name = "lblOutToday";
            this.lblOutToday.Size = new System.Drawing.Size(124, 30);
            this.lblOutToday.TabIndex = 8;
            this.lblOutToday.Text = "0";
            //
            // lblOutTodayCap
            //
            this.lblOutTodayCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblOutTodayCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblOutTodayCap.Appearance.Options.UseFont = true;
            this.lblOutTodayCap.Appearance.Options.UseForeColor = true;
            this.lblOutTodayCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblOutTodayCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblOutTodayCap.Location = new System.Drawing.Point(388, 72);
            this.lblOutTodayCap.Name = "lblOutTodayCap";
            this.lblOutTodayCap.Size = new System.Drawing.Size(124, 16);
            this.lblOutTodayCap.TabIndex = 9;
            this.lblOutTodayCap.Text = "Xuất viện";
            //
            // lblNote
            //
            this.lblNote.Appearance.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Italic);
            this.lblNote.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblNote.Appearance.Options.UseFont = true;
            this.lblNote.Appearance.Options.UseForeColor = true;
            this.lblNote.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNote.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNote.Location = new System.Drawing.Point(16, 100);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(497, 16);
            this.lblNote.TabIndex = 10;
            this.lblNote.Text = "* số nhập/xuất được cộng dồn từ đầu ngày, các chỉ số còn lại hiển thị realtime";
            //
            // icoBeds
            //
            this.icoBeds.BackColor = System.Drawing.Color.Transparent;
            this.icoBeds.IconColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.icoBeds.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Bed;
            this.icoBeds.Location = new System.Drawing.Point(557, 13);
            this.icoBeds.Name = "icoBeds";
            this.icoBeds.Size = new System.Drawing.Size(15, 15);
            this.icoBeds.TabIndex = 11;
            //
            // lblBedsTitle
            //
            this.lblBedsTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblBedsTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblBedsTitle.Appearance.Options.UseFont = true;
            this.lblBedsTitle.Appearance.Options.UseForeColor = true;
            this.lblBedsTitle.Location = new System.Drawing.Point(579, 12);
            this.lblBedsTitle.Name = "lblBedsTitle";
            this.lblBedsTitle.Size = new System.Drawing.Size(76, 16);
            this.lblBedsTitle.TabIndex = 12;
            this.lblBedsTitle.Text = "Giường bệnh";
            //
            // lblBedTotal
            //
            this.lblBedTotal.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBedTotal.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblBedTotal.Appearance.Options.UseFont = true;
            this.lblBedTotal.Appearance.Options.UseForeColor = true;
            this.lblBedTotal.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedTotal.Location = new System.Drawing.Point(557, 42);
            this.lblBedTotal.Name = "lblBedTotal";
            this.lblBedTotal.Size = new System.Drawing.Size(121, 30);
            this.lblBedTotal.TabIndex = 13;
            this.lblBedTotal.Text = "0";
            //
            // lblBedTotalCap
            //
            this.lblBedTotalCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblBedTotalCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblBedTotalCap.Appearance.Options.UseFont = true;
            this.lblBedTotalCap.Appearance.Options.UseForeColor = true;
            this.lblBedTotalCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblBedTotalCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedTotalCap.Location = new System.Drawing.Point(557, 72);
            this.lblBedTotalCap.Name = "lblBedTotalCap";
            this.lblBedTotalCap.Size = new System.Drawing.Size(121, 16);
            this.lblBedTotalCap.TabIndex = 14;
            this.lblBedTotalCap.Text = "Thực kê";
            //
            // lblBedUsed
            //
            this.lblBedUsed.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBedUsed.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblBedUsed.Appearance.Options.UseFont = true;
            this.lblBedUsed.Appearance.Options.UseForeColor = true;
            this.lblBedUsed.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedUsed.Location = new System.Drawing.Point(678, 42);
            this.lblBedUsed.Name = "lblBedUsed";
            this.lblBedUsed.Size = new System.Drawing.Size(121, 30);
            this.lblBedUsed.TabIndex = 15;
            this.lblBedUsed.Text = "0";
            //
            // lblBedUsedCap
            //
            this.lblBedUsedCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblBedUsedCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblBedUsedCap.Appearance.Options.UseFont = true;
            this.lblBedUsedCap.Appearance.Options.UseForeColor = true;
            this.lblBedUsedCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblBedUsedCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedUsedCap.Location = new System.Drawing.Point(678, 72);
            this.lblBedUsedCap.Name = "lblBedUsedCap";
            this.lblBedUsedCap.Size = new System.Drawing.Size(121, 16);
            this.lblBedUsedCap.TabIndex = 16;
            this.lblBedUsedCap.Text = "Sử dụng";
            //
            // lblBedEmpty
            //
            this.lblBedEmpty.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblBedEmpty.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblBedEmpty.Appearance.Options.UseFont = true;
            this.lblBedEmpty.Appearance.Options.UseForeColor = true;
            this.lblBedEmpty.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedEmpty.Location = new System.Drawing.Point(799, 42);
            this.lblBedEmpty.Name = "lblBedEmpty";
            this.lblBedEmpty.Size = new System.Drawing.Size(121, 30);
            this.lblBedEmpty.TabIndex = 17;
            this.lblBedEmpty.Text = "0";
            //
            // lblBedEmptyCap
            //
            this.lblBedEmptyCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblBedEmptyCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblBedEmptyCap.Appearance.Options.UseFont = true;
            this.lblBedEmptyCap.Appearance.Options.UseForeColor = true;
            this.lblBedEmptyCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblBedEmptyCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBedEmptyCap.Location = new System.Drawing.Point(799, 72);
            this.lblBedEmptyCap.Name = "lblBedEmptyCap";
            this.lblBedEmptyCap.Size = new System.Drawing.Size(121, 16);
            this.lblBedEmptyCap.TabIndex = 18;
            this.lblBedEmptyCap.Text = "Trống";
            //
            // icoCare
            //
            this.icoCare.BackColor = System.Drawing.Color.Transparent;
            this.icoCare.IconColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.icoCare.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.ShieldHeart;
            this.icoCare.Location = new System.Drawing.Point(966, 13);
            this.icoCare.Name = "icoCare";
            this.icoCare.Size = new System.Drawing.Size(15, 15);
            this.icoCare.TabIndex = 19;
            //
            // lblCareTitle
            //
            this.lblCareTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCareTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblCareTitle.Appearance.Options.UseFont = true;
            this.lblCareTitle.Appearance.Options.UseForeColor = true;
            this.lblCareTitle.Location = new System.Drawing.Point(988, 12);
            this.lblCareTitle.Name = "lblCareTitle";
            this.lblCareTitle.Size = new System.Drawing.Size(101, 16);
            this.lblCareTitle.TabIndex = 20;
            this.lblCareTitle.Text = "Chế độ chăm sóc";
            //
            // lblCare1
            //
            this.lblCare1.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblCare1.Appearance.ForeColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.lblCare1.Appearance.Options.UseFont = true;
            this.lblCare1.Appearance.Options.UseForeColor = true;
            this.lblCare1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare1.Location = new System.Drawing.Point(966, 42);
            this.lblCare1.Name = "lblCare1";
            this.lblCare1.Size = new System.Drawing.Size(124, 30);
            this.lblCare1.TabIndex = 21;
            this.lblCare1.Text = "0";
            //
            // lblCare1Cap
            //
            this.lblCare1Cap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCare1Cap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblCare1Cap.Appearance.Options.UseFont = true;
            this.lblCare1Cap.Appearance.Options.UseForeColor = true;
            this.lblCare1Cap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblCare1Cap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare1Cap.Location = new System.Drawing.Point(966, 72);
            this.lblCare1Cap.Name = "lblCare1Cap";
            this.lblCare1Cap.Size = new System.Drawing.Size(124, 16);
            this.lblCare1Cap.TabIndex = 22;
            this.lblCare1Cap.Text = "Cấp I";
            //
            // lblCare2
            //
            this.lblCare2.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblCare2.Appearance.ForeColor = System.Drawing.Color.FromArgb(245, 158, 11);
            this.lblCare2.Appearance.Options.UseFont = true;
            this.lblCare2.Appearance.Options.UseForeColor = true;
            this.lblCare2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare2.Location = new System.Drawing.Point(1090, 42);
            this.lblCare2.Name = "lblCare2";
            this.lblCare2.Size = new System.Drawing.Size(124, 30);
            this.lblCare2.TabIndex = 23;
            this.lblCare2.Text = "0";
            //
            // lblCare2Cap
            //
            this.lblCare2Cap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCare2Cap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblCare2Cap.Appearance.Options.UseFont = true;
            this.lblCare2Cap.Appearance.Options.UseForeColor = true;
            this.lblCare2Cap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblCare2Cap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare2Cap.Location = new System.Drawing.Point(1090, 72);
            this.lblCare2Cap.Name = "lblCare2Cap";
            this.lblCare2Cap.Size = new System.Drawing.Size(124, 16);
            this.lblCare2Cap.TabIndex = 24;
            this.lblCare2Cap.Text = "Cấp II";
            //
            // lblCare3
            //
            this.lblCare3.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblCare3.Appearance.ForeColor = System.Drawing.Color.FromArgb(34, 197, 94);
            this.lblCare3.Appearance.Options.UseFont = true;
            this.lblCare3.Appearance.Options.UseForeColor = true;
            this.lblCare3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare3.Location = new System.Drawing.Point(1214, 42);
            this.lblCare3.Name = "lblCare3";
            this.lblCare3.Size = new System.Drawing.Size(124, 30);
            this.lblCare3.TabIndex = 25;
            this.lblCare3.Text = "0";
            //
            // lblCare3Cap
            //
            this.lblCare3Cap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCare3Cap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblCare3Cap.Appearance.Options.UseFont = true;
            this.lblCare3Cap.Appearance.Options.UseForeColor = true;
            this.lblCare3Cap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblCare3Cap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCare3Cap.Location = new System.Drawing.Point(1214, 72);
            this.lblCare3Cap.Name = "lblCare3Cap";
            this.lblCare3Cap.Size = new System.Drawing.Size(124, 16);
            this.lblCare3Cap.TabIndex = 26;
            this.lblCare3Cap.Text = "Cấp III";
            //
            // lblSpecial
            //
            this.lblSpecial.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblSpecial.Appearance.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblSpecial.Appearance.Options.UseFont = true;
            this.lblSpecial.Appearance.Options.UseForeColor = true;
            this.lblSpecial.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSpecial.Location = new System.Drawing.Point(1338, 42);
            this.lblSpecial.Name = "lblSpecial";
            this.lblSpecial.Size = new System.Drawing.Size(124, 30);
            this.lblSpecial.TabIndex = 27;
            this.lblSpecial.Text = "0";
            //
            // lblSpecialCap
            //
            this.lblSpecialCap.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblSpecialCap.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblSpecialCap.Appearance.Options.UseFont = true;
            this.lblSpecialCap.Appearance.Options.UseForeColor = true;
            this.lblSpecialCap.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblSpecialCap.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSpecialCap.Location = new System.Drawing.Point(1338, 72);
            this.lblSpecialCap.Name = "lblSpecialCap";
            this.lblSpecialCap.Size = new System.Drawing.Size(124, 16);
            this.lblSpecialCap.TabIndex = 28;
            this.lblSpecialCap.Text = "Y lệnh đặc thù";
            //
            // UcSummaryBar
            //
            this.Controls.Add(this.lblSpecialCap);
            this.Controls.Add(this.lblSpecial);
            this.Controls.Add(this.lblCare3Cap);
            this.Controls.Add(this.lblCare3);
            this.Controls.Add(this.lblCare2Cap);
            this.Controls.Add(this.lblCare2);
            this.Controls.Add(this.lblCare1Cap);
            this.Controls.Add(this.lblCare1);
            this.Controls.Add(this.lblCareTitle);
            this.Controls.Add(this.icoCare);
            this.Controls.Add(this.lblBedEmptyCap);
            this.Controls.Add(this.lblBedEmpty);
            this.Controls.Add(this.lblBedUsedCap);
            this.Controls.Add(this.lblBedUsed);
            this.Controls.Add(this.lblBedTotalCap);
            this.Controls.Add(this.lblBedTotal);
            this.Controls.Add(this.lblBedsTitle);
            this.Controls.Add(this.icoBeds);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.lblOutTodayCap);
            this.Controls.Add(this.lblOutToday);
            this.Controls.Add(this.lblInTodayCap);
            this.Controls.Add(this.lblInToday);
            this.Controls.Add(this.lblWaitInCap);
            this.Controls.Add(this.lblWaitIn);
            this.Controls.Add(this.lblInPatientCurrentCap);
            this.Controls.Add(this.lblInPatientCurrent);
            this.Controls.Add(this.lblPatientsTitle);
            this.Controls.Add(this.icoPatients);
            this.Name = "UcSummaryBar";
            this.Size = new System.Drawing.Size(1480, 122);
            this.ResumeLayout(false);
        }

        #endregion

        private IconBox icoPatients;
        private DevExpress.XtraEditors.LabelControl lblPatientsTitle;
        private DevExpress.XtraEditors.LabelControl lblInPatientCurrent;
        private DevExpress.XtraEditors.LabelControl lblInPatientCurrentCap;
        private DevExpress.XtraEditors.LabelControl lblWaitIn;
        private DevExpress.XtraEditors.LabelControl lblWaitInCap;
        private DevExpress.XtraEditors.LabelControl lblInToday;
        private DevExpress.XtraEditors.LabelControl lblInTodayCap;
        private DevExpress.XtraEditors.LabelControl lblOutToday;
        private DevExpress.XtraEditors.LabelControl lblOutTodayCap;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private IconBox icoBeds;
        private DevExpress.XtraEditors.LabelControl lblBedsTitle;
        private DevExpress.XtraEditors.LabelControl lblBedTotal;
        private DevExpress.XtraEditors.LabelControl lblBedTotalCap;
        private DevExpress.XtraEditors.LabelControl lblBedUsed;
        private DevExpress.XtraEditors.LabelControl lblBedUsedCap;
        private DevExpress.XtraEditors.LabelControl lblBedEmpty;
        private DevExpress.XtraEditors.LabelControl lblBedEmptyCap;
        private IconBox icoCare;
        private DevExpress.XtraEditors.LabelControl lblCareTitle;
        private DevExpress.XtraEditors.LabelControl lblCare1;
        private DevExpress.XtraEditors.LabelControl lblCare1Cap;
        private DevExpress.XtraEditors.LabelControl lblCare2;
        private DevExpress.XtraEditors.LabelControl lblCare2Cap;
        private DevExpress.XtraEditors.LabelControl lblCare3;
        private DevExpress.XtraEditors.LabelControl lblCare3Cap;
        private DevExpress.XtraEditors.LabelControl lblSpecial;
        private DevExpress.XtraEditors.LabelControl lblSpecialCap;
    }
}
