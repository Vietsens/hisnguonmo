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
    partial class UcBedCard
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
            this.lblBedCode = new DevExpress.XtraEditors.LabelControl();
            this.lblPatientName = new DevExpress.XtraEditors.LabelControl();
            this.lblMeta = new DevExpress.XtraEditors.LabelControl();
            this.lblDiagnosis = new DevExpress.XtraEditors.LabelControl();
            this.lblDoctor = new DevExpress.XtraEditors.LabelControl();
            this.icoDoctor = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.icoPulse = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblPulse = new DevExpress.XtraEditors.LabelControl();
            this.lblPulseCaption = new DevExpress.XtraEditors.LabelControl();
            this.icoTemp = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblTemp = new DevExpress.XtraEditors.LabelControl();
            this.lblTempCaption = new DevExpress.XtraEditors.LabelControl();
            this.icoBp = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblBp = new DevExpress.XtraEditors.LabelControl();
            this.lblBpCaption = new DevExpress.XtraEditors.LabelControl();
            this.icoEmptyBed = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconBox();
            this.lblEmptyCode = new DevExpress.XtraEditors.LabelControl();
            this.lblEmptyText = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            //
            // lblBedCode
            //
            this.lblBedCode.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBedCode.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblBedCode.Appearance.Options.UseFont = true;
            this.lblBedCode.Appearance.Options.UseForeColor = true;
            this.lblBedCode.Location = new System.Drawing.Point(16, 13);
            this.lblBedCode.Name = "lblBedCode";
            this.lblBedCode.Size = new System.Drawing.Size(37, 14);
            this.lblBedCode.TabIndex = 0;
            this.lblBedCode.Text = "201-A";
            //
            // lblPatientName
            //
            this.lblPatientName.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPatientName.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblPatientName.Appearance.Options.UseFont = true;
            this.lblPatientName.Appearance.Options.UseForeColor = true;
            this.lblPatientName.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblPatientName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPatientName.Location = new System.Drawing.Point(59, 11);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(197, 18);
            this.lblPatientName.TabIndex = 1;
            this.lblPatientName.Text = "Nguyễn Thị Y";
            //
            // lblMeta
            //
            this.lblMeta.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblMeta.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblMeta.Appearance.Options.UseFont = true;
            this.lblMeta.Appearance.Options.UseForeColor = true;
            this.lblMeta.Location = new System.Drawing.Point(16, 35);
            this.lblMeta.Name = "lblMeta";
            this.lblMeta.Size = new System.Drawing.Size(65, 14);
            this.lblMeta.TabIndex = 2;
            this.lblMeta.Text = "78 tuổi · Nữ";
            //
            // lblDiagnosis
            //
            this.lblDiagnosis.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.lblDiagnosis.Appearance.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblDiagnosis.Appearance.Options.UseFont = true;
            this.lblDiagnosis.Appearance.Options.UseForeColor = true;
            this.lblDiagnosis.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblDiagnosis.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiagnosis.Location = new System.Drawing.Point(91, 35);
            this.lblDiagnosis.Name = "lblDiagnosis";
            this.lblDiagnosis.Size = new System.Drawing.Size(165, 15);
            this.lblDiagnosis.TabIndex = 3;
            this.lblDiagnosis.Text = "C24.0 Ung thư đường mật rốn gan";
            //
            // icoDoctor
            //
            this.icoDoctor.BackColor = System.Drawing.Color.Transparent;
            this.icoDoctor.IconColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.icoDoctor.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Person;
            this.icoDoctor.Location = new System.Drawing.Point(16, 57);
            this.icoDoctor.Name = "icoDoctor";
            this.icoDoctor.Size = new System.Drawing.Size(11, 13);
            this.icoDoctor.TabIndex = 4;
            //
            // lblDoctor
            //
            this.lblDoctor.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDoctor.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblDoctor.Appearance.Options.UseFont = true;
            this.lblDoctor.Appearance.Options.UseForeColor = true;
            this.lblDoctor.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.lblDoctor.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDoctor.Location = new System.Drawing.Point(32, 56);
            this.lblDoctor.Name = "lblDoctor";
            this.lblDoctor.Size = new System.Drawing.Size(224, 15);
            this.lblDoctor.TabIndex = 5;
            this.lblDoctor.Text = "BS. Trần Văn B";
            //
            // icoPulse
            //
            this.icoPulse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.icoPulse.BackColor = System.Drawing.Color.Transparent;
            this.icoPulse.IconColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.icoPulse.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Heart;
            this.icoPulse.Location = new System.Drawing.Point(287, 17);
            this.icoPulse.Name = "icoPulse";
            this.icoPulse.Size = new System.Drawing.Size(12, 12);
            this.icoPulse.TabIndex = 6;
            //
            // lblPulse
            //
            this.lblPulse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblPulse.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPulse.Appearance.ForeColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.lblPulse.Appearance.Options.UseFont = true;
            this.lblPulse.Appearance.Options.UseForeColor = true;
            this.lblPulse.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblPulse.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPulse.Location = new System.Drawing.Point(272, 31);
            this.lblPulse.Name = "lblPulse";
            this.lblPulse.Size = new System.Drawing.Size(42, 14);
            this.lblPulse.TabIndex = 7;
            this.lblPulse.Text = "86";
            //
            // lblPulseCaption
            //
            this.lblPulseCaption.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblPulseCaption.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75F);
            this.lblPulseCaption.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblPulseCaption.Appearance.Options.UseFont = true;
            this.lblPulseCaption.Appearance.Options.UseForeColor = true;
            this.lblPulseCaption.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblPulseCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPulseCaption.Location = new System.Drawing.Point(268, 52);
            this.lblPulseCaption.Name = "lblPulseCaption";
            this.lblPulseCaption.Size = new System.Drawing.Size(50, 22);
            this.lblPulseCaption.TabIndex = 8;
            this.lblPulseCaption.Text = "Mạch\r\n(lần/phút)";
            //
            // icoTemp
            //
            this.icoTemp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.icoTemp.BackColor = System.Drawing.Color.Transparent;
            this.icoTemp.IconColor = System.Drawing.Color.FromArgb(217, 119, 6);
            this.icoTemp.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Thermometer;
            this.icoTemp.Location = new System.Drawing.Point(334, 17);
            this.icoTemp.Name = "icoTemp";
            this.icoTemp.Size = new System.Drawing.Size(12, 12);
            this.icoTemp.TabIndex = 9;
            //
            // lblTemp
            //
            this.lblTemp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblTemp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTemp.Appearance.ForeColor = System.Drawing.Color.FromArgb(217, 119, 6);
            this.lblTemp.Appearance.Options.UseFont = true;
            this.lblTemp.Appearance.Options.UseForeColor = true;
            this.lblTemp.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTemp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTemp.Location = new System.Drawing.Point(319, 31);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(42, 14);
            this.lblTemp.TabIndex = 10;
            this.lblTemp.Text = "36.8";
            //
            // lblTempCaption
            //
            this.lblTempCaption.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblTempCaption.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75F);
            this.lblTempCaption.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblTempCaption.Appearance.Options.UseFont = true;
            this.lblTempCaption.Appearance.Options.UseForeColor = true;
            this.lblTempCaption.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblTempCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblTempCaption.Location = new System.Drawing.Point(315, 52);
            this.lblTempCaption.Name = "lblTempCaption";
            this.lblTempCaption.Size = new System.Drawing.Size(50, 22);
            this.lblTempCaption.TabIndex = 11;
            this.lblTempCaption.Text = "Nhiệt độ\r\n(°C)";
            //
            // icoBp
            //
            this.icoBp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.icoBp.BackColor = System.Drawing.Color.Transparent;
            this.icoBp.IconColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.icoBp.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.BloodPressure;
            this.icoBp.Location = new System.Drawing.Point(381, 17);
            this.icoBp.Name = "icoBp";
            this.icoBp.Size = new System.Drawing.Size(12, 12);
            this.icoBp.TabIndex = 12;
            //
            // lblBp
            //
            this.lblBp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblBp.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBp.Appearance.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lblBp.Appearance.Options.UseFont = true;
            this.lblBp.Appearance.Options.UseForeColor = true;
            this.lblBp.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblBp.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBp.Location = new System.Drawing.Point(366, 31);
            this.lblBp.Name = "lblBp";
            this.lblBp.Size = new System.Drawing.Size(42, 14);
            this.lblBp.TabIndex = 13;
            this.lblBp.Text = "120/80";
            //
            // lblBpCaption
            //
            this.lblBpCaption.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblBpCaption.Appearance.Font = new System.Drawing.Font("Segoe UI", 6.75F);
            this.lblBpCaption.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblBpCaption.Appearance.Options.UseFont = true;
            this.lblBpCaption.Appearance.Options.UseForeColor = true;
            this.lblBpCaption.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblBpCaption.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBpCaption.Location = new System.Drawing.Point(362, 52);
            this.lblBpCaption.Name = "lblBpCaption";
            this.lblBpCaption.Size = new System.Drawing.Size(50, 22);
            this.lblBpCaption.TabIndex = 14;
            this.lblBpCaption.Text = "Huyết áp\r\n(mmHg)";
            //
            // icoEmptyBed
            //
            this.icoEmptyBed.BackColor = System.Drawing.Color.Transparent;
            this.icoEmptyBed.IconColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.icoEmptyBed.Kind = HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.IconKind.Bed;
            this.icoEmptyBed.Location = new System.Drawing.Point(190, 37);
            this.icoEmptyBed.Name = "icoEmptyBed";
            this.icoEmptyBed.Size = new System.Drawing.Size(14, 14);
            this.icoEmptyBed.TabIndex = 15;
            this.icoEmptyBed.Visible = false;
            //
            // lblEmptyCode
            //
            this.lblEmptyCode.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblEmptyCode.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblEmptyCode.Appearance.Options.UseFont = true;
            this.lblEmptyCode.Appearance.Options.UseForeColor = true;
            this.lblEmptyCode.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblEmptyCode.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEmptyCode.Location = new System.Drawing.Point(130, 38);
            this.lblEmptyCode.Name = "lblEmptyCode";
            this.lblEmptyCode.Size = new System.Drawing.Size(50, 14);
            this.lblEmptyCode.TabIndex = 16;
            this.lblEmptyCode.Text = "201-B";
            this.lblEmptyCode.Visible = false;
            //
            // lblEmptyText
            //
            this.lblEmptyText.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.75F);
            this.lblEmptyText.Appearance.ForeColor = System.Drawing.Color.FromArgb(148, 156, 168);
            this.lblEmptyText.Appearance.Options.UseFont = true;
            this.lblEmptyText.Appearance.Options.UseForeColor = true;
            this.lblEmptyText.Location = new System.Drawing.Point(210, 38);
            this.lblEmptyText.Name = "lblEmptyText";
            this.lblEmptyText.Size = new System.Drawing.Size(69, 15);
            this.lblEmptyText.TabIndex = 17;
            this.lblEmptyText.Text = "Giường trống";
            this.lblEmptyText.Visible = false;
            //
            // UcBedCard
            //
            this.Controls.Add(this.lblEmptyText);
            this.Controls.Add(this.lblEmptyCode);
            this.Controls.Add(this.icoEmptyBed);
            this.Controls.Add(this.lblBpCaption);
            this.Controls.Add(this.lblBp);
            this.Controls.Add(this.icoBp);
            this.Controls.Add(this.lblTempCaption);
            this.Controls.Add(this.lblTemp);
            this.Controls.Add(this.icoTemp);
            this.Controls.Add(this.lblPulseCaption);
            this.Controls.Add(this.lblPulse);
            this.Controls.Add(this.icoPulse);
            this.Controls.Add(this.lblDoctor);
            this.Controls.Add(this.icoDoctor);
            this.Controls.Add(this.lblDiagnosis);
            this.Controls.Add(this.lblMeta);
            this.Controls.Add(this.lblPatientName);
            this.Controls.Add(this.lblBedCode);
            this.Name = "UcBedCard";
            this.Size = new System.Drawing.Size(420, 88);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblBedCode;
        private DevExpress.XtraEditors.LabelControl lblPatientName;
        private DevExpress.XtraEditors.LabelControl lblMeta;
        private DevExpress.XtraEditors.LabelControl lblDiagnosis;
        private DevExpress.XtraEditors.LabelControl lblDoctor;
        private IconBox icoDoctor;
        private IconBox icoPulse;
        private DevExpress.XtraEditors.LabelControl lblPulse;
        private DevExpress.XtraEditors.LabelControl lblPulseCaption;
        private IconBox icoTemp;
        private DevExpress.XtraEditors.LabelControl lblTemp;
        private DevExpress.XtraEditors.LabelControl lblTempCaption;
        private IconBox icoBp;
        private DevExpress.XtraEditors.LabelControl lblBp;
        private DevExpress.XtraEditors.LabelControl lblBpCaption;
        private IconBox icoEmptyBed;
        private DevExpress.XtraEditors.LabelControl lblEmptyCode;
        private DevExpress.XtraEditors.LabelControl lblEmptyText;
    }
}
