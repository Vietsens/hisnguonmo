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
using System;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup
{
    /// <summary>
    /// Collects the one time password the gateway sent to the patient.
    /// Pure UI — the OTP is handed back through <see cref="OtpValue"/> and never logged.
    /// </summary>
    public partial class frmEmrToolkitOtp : HIS.Desktop.Utility.FormBase
    {
        private readonly RequestViewResultADO transaction;

        /// <summary>Expiry moment of the OTP session, null when the gateway did not send one.</summary>
        private DateTime? expireTime;

        /// <summary>What the user typed. Only meaningful when DialogResult is OK.</summary>
        public string OtpValue { get; private set; }

        public frmEmrToolkitOtp(RequestViewResultADO transaction)
        {
            InitializeComponent();
            try
            {
                this.transaction = transaction ?? new RequestViewResultADO();
                this.OtpValue = "";
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmEmrToolkitOtp_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillTransactionInfo();
                this.txtOtp.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.Library.EmrToolkitImport.Resources.Lang",
                    typeof(frmEmrToolkitOtp).Assembly);

                this.Text = GetLang("frmEmrToolkitOtp.Text");
                this.lciOtp.Text = GetLang("frmEmrToolkitOtp.lciOtp.Text");
                this.lciChannel.Text = GetLang("frmEmrToolkitOtp.lciChannel.Text");
                this.lciExpire.Text = GetLang("frmEmrToolkitOtp.lciExpire.Text");
                this.btnAccept.Text = GetLang("frmEmrToolkitOtp.btnAccept.Text");
                this.btnCancel.Text = GetLang("frmEmrToolkitOtp.btnCancel.Text");
                this.txtOtp.Properties.NullValuePrompt = GetLang("frmEmrToolkitOtp.txtOtp.NullValuePrompt");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? key : value;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return key;
        }

        /// <summary>Shows the channels the OTP was sent through and starts the countdown.</summary>
        private void FillTransactionInfo()
        {
            try
            {
                this.lblChannel.Text = this.transaction.KenhGuiOTP != null
                    ? string.Join(", ", this.transaction.KenhGuiOTP.ToArray())
                    : "";

                DateTime parsed;
                if (!string.IsNullOrWhiteSpace(this.transaction.ThoiHanOTP)
                    && DateTime.TryParse(this.transaction.ThoiHanOTP, CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out parsed))
                {
                    this.expireTime = parsed.ToLocalTime();
                    this.tmrCountdown.Start();
                    ShowRemainingTime();
                }
                else
                {
                    this.lblExpire.Text = "";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void tmrCountdown_Tick(object sender, EventArgs e)
        {
            try
            {
                ShowRemainingTime();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Refreshes the remaining time label; disables accepting once the OTP has expired,
        /// because the gateway would answer 410 anyway.
        /// </summary>
        private void ShowRemainingTime()
        {
            try
            {
                if (!this.expireTime.HasValue)
                    return;

                TimeSpan remaining = this.expireTime.Value - DateTime.Now;
                if (remaining.TotalSeconds <= 0)
                {
                    this.tmrCountdown.Stop();
                    this.lblExpire.Text = Resources.ResourceMessage.MaOtpDaHetHan;
                    this.lblExpire.Appearance.ForeColor = System.Drawing.Color.Red;
                    this.btnAccept.Enabled = false;
                    return;
                }

                this.lblExpire.Text = string.Format("{0:00}:{1:00}",
                    (int)remaining.TotalMinutes, remaining.Seconds);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtOtp_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    AcceptOtp();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                AcceptOtp();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Validates the field then closes with OK so the caller can download.</summary>
        private void AcceptOtp()
        {
            try
            {
                string otp = (this.txtOtp.Text ?? "").Trim();
                if (string.IsNullOrEmpty(otp))
                {
                    this.txtOtp.ErrorText = Resources.ResourceMessage.ThieuMaOtp;
                    this.txtOtp.Focus();
                    return;
                }

                this.txtOtp.ErrorText = "";
                this.OtpValue = otp;
                this.tmrCountdown.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
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
                this.OtpValue = "";
                this.tmrCountdown.Stop();
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
