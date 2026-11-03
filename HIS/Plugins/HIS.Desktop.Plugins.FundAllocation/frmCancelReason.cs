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
using HIS.Desktop.Plugins.FundAllocation.Resources;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.FundAllocation
{
    /// <summary>
    /// Input the mandatory reason before cancelling the allocation
    /// </summary>
    public partial class frmCancelReason : DevExpress.XtraEditors.XtraForm
    {
        public string Reason { get; private set; }

        public frmCancelReason()
        {
            InitializeComponent();
        }

        private void frmCancelReason_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = ResourceLanguageManager.Get("frmCancelReason.Text");
                this.lblReason.Text = ResourceLanguageManager.Get("frmCancelReason.lblReason.Text");
                this.btnOk.Text = ResourceLanguageManager.Get("frmCancelReason.btnOk.Text");
                this.btnCancel.Text = ResourceLanguageManager.Get("frmCancelReason.btnCancel.Text");
                this.memoReason.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                string reason = this.memoReason.Text != null ? this.memoReason.Text.Trim() : "";
                if (String.IsNullOrEmpty(reason))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("frmFundAllocation.Message.NhapLyDoHuy"), ResourceLanguageManager.Get("frmFundAllocation.Message.ThongBao"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.memoReason.Focus();
                    return;
                }
                this.Reason = reason;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
