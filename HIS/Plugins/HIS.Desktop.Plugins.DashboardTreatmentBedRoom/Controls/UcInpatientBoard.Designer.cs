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
    partial class UcInpatientBoard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (tmrRefresh != null)
                {
                    tmrRefresh.Stop();
                    tmrRefresh.Tick -= TmrRefresh_Tick;
                }
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ucSummary = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.UcSummaryBar();
            this.pnlScroll = new HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls.NoAutoScrollPanel();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.tmrRefresh = new System.Windows.Forms.Timer(this.components);
            this.pnlScroll.SuspendLayout();
            this.SuspendLayout();
            //
            // ucSummary
            //
            this.ucSummary.Anchor = System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.ucSummary.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.ucSummary.Location = new System.Drawing.Point(12, 12);
            this.ucSummary.Name = "ucSummary";
            this.ucSummary.Size = new System.Drawing.Size(1480, 122);
            this.ucSummary.TabIndex = 0;
            //
            // pnlScroll
            //
            this.pnlScroll.Anchor = System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlScroll.Controls.Add(this.pnlCanvas);
            this.pnlScroll.Location = new System.Drawing.Point(0, 146);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Size = new System.Drawing.Size(1504, 714);
            this.pnlScroll.TabIndex = 1;
            //
            // pnlCanvas
            //
            this.pnlCanvas.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.pnlCanvas.Location = new System.Drawing.Point(0, 0);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(1504, 0);
            this.pnlCanvas.TabIndex = 0;
            //
            // tmrRefresh
            //
            this.tmrRefresh.Interval = 30000;
            this.tmrRefresh.Tick += new System.EventHandler(this.TmrRefresh_Tick);
            //
            // UcInpatientBoard
            //
            this.BackColor = System.Drawing.Color.FromArgb(243, 244, 246);
            this.Controls.Add(this.pnlScroll);
            this.Controls.Add(this.ucSummary);
            this.Name = "UcInpatientBoard";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.Size = new System.Drawing.Size(1504, 860);
            this.pnlScroll.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private UcSummaryBar ucSummary;
        private NoAutoScrollPanel pnlScroll;
        private System.Windows.Forms.Panel pnlCanvas;
        private System.Windows.Forms.Timer tmrRefresh;
    }
}
