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
    partial class UcRoomCard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAutoScroll();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblRoomName = new DevExpress.XtraEditors.LabelControl();
            this.flpBeds = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            //
            // lblRoomName
            //
            this.lblRoomName.Anchor = System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.lblRoomName.Appearance.Font = new System.Drawing.Font("Segoe UI", 12.5F, System.Drawing.FontStyle.Bold);
            this.lblRoomName.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblRoomName.Appearance.Options.UseFont = true;
            this.lblRoomName.Appearance.Options.UseForeColor = true;
            this.lblRoomName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblRoomName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblRoomName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRoomName.Location = new System.Drawing.Point(10, 1);
            this.lblRoomName.Name = "lblRoomName";
            this.lblRoomName.Size = new System.Drawing.Size(400, 40);
            this.lblRoomName.TabIndex = 0;
            this.lblRoomName.Text = "201";
            //
            // flpBeds
            //
            this.flpBeds.AutoScroll = true;
            this.flpBeds.BackColor = System.Drawing.Color.White;
            this.flpBeds.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpBeds.Location = new System.Drawing.Point(10, 42);
            this.flpBeds.Margin = new System.Windows.Forms.Padding(0);
            this.flpBeds.Name = "flpBeds";
            this.flpBeds.Padding = new System.Windows.Forms.Padding(0);
            this.flpBeds.Size = new System.Drawing.Size(400, 278);
            this.flpBeds.TabIndex = 1;
            this.flpBeds.WrapContents = false;
            //
            // UcRoomCard
            //
            this.Controls.Add(this.flpBeds);
            this.Controls.Add(this.lblRoomName);
            this.Name = "UcRoomCard";
            this.Size = new System.Drawing.Size(420, 330);
            this.ResumeLayout(false);
        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblRoomName;
        private System.Windows.Forms.FlowLayoutPanel flpBeds;
    }
}
