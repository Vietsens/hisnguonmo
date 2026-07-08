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
using System.Drawing;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Popup
{
    /// <summary>
    /// Cửa sổ hiển thị kết quả gửi dữ liệu qua EMRTOOLKIT:
    /// trạng thái thành công/thất bại + JSON đã gửi + JSON nhận về (có nút sao chép).
    /// Thuần UI — KHÔNG gọi API.
    /// </summary>
    public partial class frmEmrToolkitImportResult : HIS.Desktop.Utility.FormBase
    {
        private readonly EmrToolkitImportResult result;

        public frmEmrToolkitImportResult(EmrToolkitImportResult result)
        {
            InitializeComponent();
            this.result = result ?? new EmrToolkitImportResult();
            SetIcon();
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

        private void frmEmrToolkitImportResult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                FillResultToForm();
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
                    typeof(frmEmrToolkitImportResult).Assembly);

                this.Text = GetLang("frmEmrToolkitImportResult.Text");
                this.tabSent.Text = GetLang("frmEmrToolkitImportResult.lcgRequest.Text");
                this.tabReceived.Text = GetLang("frmEmrToolkitImportResult.lcgResponse.Text");
                this.btnCopy.Text = GetLang("frmEmrToolkitImportResult.btnCopy.Text");
                this.btnClose.Text = GetLang("frmEmrToolkitImportResult.btnClose.Text");
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

        private void FillResultToForm()
        {
            try
            {
                // Trạng thái + icon + màu
                if (result.Success)
                {
                    lblStatus.Text = Resources.ResourceMessage.GuiDuLieuThanhCong;
                    lblStatus.Appearance.ForeColor = Color.Green;
                    picStatus.Image = SystemIcons.Information.ToBitmap();
                }
                else
                {
                    lblStatus.Text = Resources.ResourceMessage.GuiDuLieuThatBai;
                    lblStatus.Appearance.ForeColor = Color.Firebrick;
                    picStatus.Image = SystemIcons.Error.ToBitmap();
                }

                // Thông tin tóm tắt
                StringBuilder info = new StringBuilder();
                info.Append("Bước: ").Append(result.Step.ToString());
                if (!string.IsNullOrEmpty(result.Message))
                    info.Append("    |    Thông báo: ").Append(result.Message);
                memInfo.Text = info.ToString();

                // JSON đã gửi
                memSent.Text = result.RawRequestJson ?? "";

                // JSON nhận về (cố gắng format đẹp)
                memReceived.Text = FormatJson(result.RawResponseJson);

                // Nếu thành công, mặc định mở tab JSON nhận về
                tabResult.SelectedTabPage = result.Success ? tabReceived : tabSent;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Format JSON cho dễ đọc; trả về nguyên bản nếu không parse được.</summary>
        private string FormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "";
            try
            {
                return JToken.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                return json;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                // Sao chép JSON của tab đang xem
                string content = tabResult.SelectedTabPage == tabSent ? memSent.Text : memReceived.Text;
                if (!string.IsNullOrEmpty(content))
                {
                    Clipboard.SetText(content);
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.DaSaoChepJson,
                        this.Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
