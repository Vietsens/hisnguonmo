/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using HIS.Desktop.Plugins.TreatmentAppointment.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentAppointment.SendZaloResult
{
    public partial class frmSendZaloResult : FormBase
    {
        #region Declare
        private readonly SendAppointmentZaloResultADO result;
        #endregion

        #region Construct
        public frmSendZaloResult(SendAppointmentZaloResultADO result)
        {
            try
            {
                InitializeComponent();
                this.result = result ?? new SendAppointmentZaloResultADO();
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Methods
        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmSendZaloResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                RenderResult();
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
                    "HIS.Desktop.Plugins.TreatmentAppointment.Resources.Lang",
                    typeof(frmSendZaloResult).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmSendZaloResult.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnClose.Text = Inventec.Common.Resource.Get.Value(
                    "frmSendZaloResult.btnClose.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RenderResult()
        {
            try
            {
                int requested = this.result.TotalRequested;
                int success = this.result.TotalSuccess;
                int failed = this.result.TotalFailed;

                if (failed == 0 && success > 0)
                {
                    // All success — green
                    this.pnlHeader.Appearance.BackColor = Color.FromArgb(34, 139, 34);
                    this.lblHeading.Text = string.Format(
                        Resources.ResourceMessageLang.DaGuiThanhCongFormat, success, requested);
                    this.lblDescription.Text = Resources.ResourceMessageLang.DescriptionGuiThanhCong;
                    this.lblFailureHeader.Visible = false;
                    this.memoFailures.Visible = false;
                    this.lciFailureHeader.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciMemoFailures.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else if (success == 0)
                {
                    // All failed — red
                    this.pnlHeader.Appearance.BackColor = Color.FromArgb(180, 20, 20);
                    this.lblHeading.Text = string.Format(
                        Resources.ResourceMessageLang.GuiThatBaiFormat, failed, requested);
                    this.lblDescription.Text = Resources.ResourceMessageLang.DescriptionGuiThatBai;
                    RenderFailureDetails();
                }
                else
                {
                    // Mixed — orange
                    this.pnlHeader.Appearance.BackColor = Color.FromArgb(255, 140, 0);
                    this.lblHeading.Text = string.Format(
                        Resources.ResourceMessageLang.GuiMotPhanThanhCongFormat, success, requested, failed);
                    this.lblDescription.Text = Resources.ResourceMessageLang.DescriptionGuiMotPhanThanhCong;
                    RenderFailureDetails();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RenderFailureDetails()
        {
            try
            {
                if (this.result.Details == null) return;

                var sb = new StringBuilder();
                foreach (var item in this.result.Details.Where(o => o != null && !o.IsSuccess))
                {
                    sb.AppendFormat("• {0} ({1}): {2}",
                        item.TreatmentCode ?? string.Empty,
                        item.PatientName ?? string.Empty,
                        item.ErrorMessage ?? string.Empty)
                      .AppendLine();
                }
                this.memoFailures.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Events
        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
