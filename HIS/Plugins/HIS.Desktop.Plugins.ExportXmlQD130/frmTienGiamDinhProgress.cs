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
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    /// <summary>
    /// Hop thoai tien trinh khi tra cuu lo ho so tren he thong tien giam dinh.
    ///
    /// He ngoai gioi han 60 luot goi moi phut va chi tra mot ho so moi luot,
    /// nen lo lon co the mat vai phut. Phai cho nguoi dung thay tien do va huy duoc,
    /// tranh tuong phan mem treo roi tat ngang (PTTK_53286 muc B.4.2).
    ///
    /// Giao dien dung bang code, khong dung file Designer vi day la hop thoai tien ich
    /// rat nho, chi co mot thanh tien trinh va mot nut huy.
    /// </summary>
    public class frmTienGiamDinhProgress : XtraForm
    {
        private readonly ProgressBarControl progressBar;
        private readonly LabelControl lblStatus;
        private readonly SimpleButton btnCancel;
        private readonly CancellationTokenSource cancelSource;

        /// <summary>Token de truyen vao vong lap tra cuu</summary>
        public CancellationToken CancelToken
        {
            get { return this.cancelSource.Token; }
        }

        /// <summary>Nguoi dung da bam huy hay chua</summary>
        public bool IsCancelled
        {
            get { return this.cancelSource.IsCancellationRequested; }
        }

        public frmTienGiamDinhProgress(int total, string caption, string cancelText)
        {
            this.cancelSource = new CancellationTokenSource();

            this.SuspendLayout();

            this.Text = caption;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.ClientSize = new Size(420, 110);
            this.ControlBox = false;

            this.lblStatus = new LabelControl();
            this.lblStatus.Location = new Point(14, 14);
            this.lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
            this.lblStatus.Size = new Size(392, 18);

            this.progressBar = new ProgressBarControl();
            this.progressBar.Location = new Point(14, 38);
            this.progressBar.Size = new Size(392, 20);
            this.progressBar.Properties.Minimum = 0;
            this.progressBar.Properties.Maximum = total <= 0 ? 1 : total;
            this.progressBar.Properties.ShowTitle = true;
            this.progressBar.EditValue = 0;

            this.btnCancel = new SimpleButton();
            this.btnCancel.Text = cancelText;
            this.btnCancel.Location = new Point(312, 70);
            this.btnCancel.Size = new Size(94, 26);
            this.btnCancel.Click += btnCancel_Click;

            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnCancel);

            this.ResumeLayout(false);
        }

        /// <summary>Cap nhat tien do. Goi tu luong giao dien sau moi ho so da tra xong.</summary>
        public void SetProgress(int done, int total, string statusText)
        {
            try
            {
                this.progressBar.EditValue = done;
                this.lblStatus.Text = statusText;
                //Cho giao dien ve lai ngay - vong lap tra cuu dang chay tren cung luong UI
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnCancel.Enabled = false;
                this.cancelSource.Cancel();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.cancelSource != null)
                {
                    this.cancelSource.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            base.Dispose(disposing);
        }
    }
}
