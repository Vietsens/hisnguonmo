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
namespace HIS.Desktop.Modules.WaitingForm
{
    partial class frmUpgradeOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpgradeOption));
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.btnUpgradeHand = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblStatus.Location = new System.Drawing.Point(12, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(516, 74);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = resources.GetString("lblStatus.Text");
            // 
            // btnUpgradeHand
            // 
            this.btnUpgradeHand.Location = new System.Drawing.Point(412, 109);
            this.btnUpgradeHand.Name = "btnUpgradeHand";
            this.btnUpgradeHand.Size = new System.Drawing.Size(116, 23);
            this.btnUpgradeHand.TabIndex = 1;
            this.btnUpgradeHand.Text = "Nâng cấp thủ công";
            this.btnUpgradeHand.Click += new System.EventHandler(this.btnUpgradeHand_Click);
            // 
            // frmUpgradeOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 147);
            this.Controls.Add(this.btnUpgradeHand);
            this.Controls.Add(this.lblStatus);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpgradeOption";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nâng cấp - Upgrade";
            this.Load += new System.EventHandler(this.frmUpgradeOption_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.SimpleButton btnUpgradeHand;
    }
}
