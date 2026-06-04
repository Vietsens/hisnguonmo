/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.TreatmentAppointment.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentAppointment.SelectZaloTemplate
{
    public partial class frmSelectZaloTemplate : FormBase
    {
        #region Constants
        private const string CONFIG_KEY_ZALO_ENABLE = "MOS.SMS.ZALO_ENABLE";
        private const string PLACEHOLDER_PATTERN = @"\{\{\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*\}\}";
        #endregion

        #region Declare
        private List<TreatmentAppointmentADO> selectedTreatments;
        private List<ZaloTemplateADO> listTemplate;
        private ZaloTemplateADO selectedTemplate;
        private Dictionary<string, string> sampleDataMap;
        private string sampleHeaderText;
        #endregion

        #region Properties
        /// <summary>TemplateId user đã chọn — null nếu user hủy</summary>
        public string SelectedTemplateId { get; private set; }
        #endregion

        #region Construct
        public frmSelectZaloTemplate(List<TreatmentAppointmentADO> selectedTreatments)
        {
            try
            {
                InitializeComponent();
                this.selectedTreatments = selectedTreatments ?? new List<TreatmentAppointmentADO>();
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

        private void frmSelectZaloTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                BuildSampleDataMap();
                UpdateHeaderInfo();
                UpdateConfirmButtonText();
                LoadTemplates();
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
                    typeof(frmSelectZaloTemplate).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmSelectZaloTemplate.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lblPatientCountCaption.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.lblPatientCountCaption.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lblGatewayCaption.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.lblGatewayCaption.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciTemplate.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.lciTemplate.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lblNote.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.lblNote.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnConfirm.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.btnConfirm.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnCancel.Text = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.btnCancel.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Build map placeholder → giá trị thực từ BN đầu tiên trong danh sách đã tích chọn.
        /// Convention placeholder theo MOS.SMS.ZALO_TEMPLATE_PARAMS: ho_ten, ma_benh_nhan, ngay_tai_kham, khoa_kham.
        /// </summary>
        private void BuildSampleDataMap()
        {
            this.sampleDataMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (this.selectedTreatments == null || this.selectedTreatments.Count == 0)
                {
                    this.sampleHeaderText = string.Empty;
                    return;
                }

                var sample = this.selectedTreatments[0];
                this.sampleDataMap["ho_ten"] = sample.TDL_PATIENT_NAME ?? string.Empty;
                this.sampleDataMap["ma_benh_nhan"] = sample.TDL_PATIENT_CODE ?? string.Empty;
                this.sampleDataMap["ngay_tai_kham"] = FormatAppointmentDate(sample.APPOINTMENT_TIME);
                this.sampleDataMap["khoa_kham"] = LookupRoomNames(sample.APPOINTMENT_EXAM_ROOM_IDS);

                this.sampleHeaderText = string.Format(
                    Resources.ResourceMessageLang.NoiDungXemTruocVoiBenhNhanFormat,
                    sample.TDL_PATIENT_NAME ?? string.Empty,
                    sample.TDL_PATIENT_CODE ?? string.Empty);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static string FormatAppointmentDate(long? appointmentTime)
        {
            try
            {
                if (!appointmentTime.HasValue || appointmentTime.Value == 0) return string.Empty;
                return Inventec.Common.DateTime.Convert.TimeNumberToDateString(appointmentTime.Value) ?? string.Empty;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Lookup tên phòng từ chuỗi APPOINTMENT_EXAM_ROOM_IDS dạng "12,34,56".
        /// Ghép tên các phòng cách dấu phẩy. Dùng cho preview Zalo template
        /// (HIS_TREATMENT.APPOINTMENT_EXAM_ROOM_IDS là string multi-ID).
        /// </summary>
        private static string LookupRoomNames(string roomIdsCsv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomIdsCsv)) return string.Empty;

                var roomDict = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>()
                    .Where(o => o != null)
                    .GroupBy(o => o.ROOM_ID)
                    .ToDictionary(g => g.Key, g => g.First().EXECUTE_ROOM_NAME ?? string.Empty);

                var names = new List<string>();
                foreach (var token in roomIdsCsv.Split(','))
                {
                    long roomId;
                    if (!long.TryParse(token.Trim(), out roomId)) continue;
                    string name;
                    if (roomDict.TryGetValue(roomId, out name) && !string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
                return string.Join(", ", names);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        private void UpdateHeaderInfo()
        {
            try
            {
                this.lblPatientCountValue.Text = string.Format(
                    Resources.ResourceMessageLang.NBenhNhanFormat,
                    this.selectedTreatments.Count);

                this.lblGatewayValue.Text = GetGatewayDisplayName();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string GetGatewayDisplayName()
        {
            try
            {
                string raw = HisConfigs.Get<string>(CONFIG_KEY_ZALO_ENABLE);
                int value;
                if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw.Trim(), out value))
                {
                    if (value == (int)EnumZaloEnable.OneSms)
                        return Resources.ResourceMessageLang.GatewayOneSms;
                    if (value == (int)EnumZaloEnable.FnsZns)
                        return Resources.ResourceMessageLang.GatewayFnsZns;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return "—";
        }

        private void UpdateConfirmButtonText()
        {
            try
            {
                string baseText = Inventec.Common.Resource.Get.Value(
                    "frmSelectZaloTemplate.btnConfirm.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnConfirm.Text = string.Format("{0} ({1})", baseText, this.selectedTreatments.Count);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadTemplates()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                var apiResult = new BackendAdapter(param).Get<List<ZaloTemplateADO>>(
                    HisRequestUriStore.MOSHIS_HIS_TREATMENT_GET_ZALO_TEMPLATES,
                    ApiConsumers.MosConsumer, null, param);
                WaitingManager.Hide();

                this.listTemplate = apiResult ?? new List<ZaloTemplateADO>();

                this.cboTemplate.Properties.DataSource = this.listTemplate;
                this.cboTemplate.Properties.DisplayMember = "TemplateName";
                this.cboTemplate.Properties.ValueMember = "TemplateId";
                this.cboTemplate.Properties.PopulateColumns();
                if (this.cboTemplate.Properties.Columns.Count > 0)
                {
                    foreach (DevExpress.XtraEditors.Controls.LookUpColumnInfo col in this.cboTemplate.Properties.Columns)
                    {
                        col.Visible = col.FieldName == "TemplateName";
                    }
                }
                this.cboTemplate.Properties.NullText = "";
                this.cboTemplate.Properties.ShowHeader = false;
                this.cboTemplate.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;

                if (this.listTemplate.Count > 0)
                {
                    this.cboTemplate.EditValue = this.listTemplate[0].TemplateId;
                    ApplyTemplateSelection(this.listTemplate[0]);
                }
                else
                {
                    this.selectedTemplate = null;
                    this.btnConfirm.Enabled = false;
                    this.rtxtPreview.Clear();
                    this.lblQualityBadge.Text = "—";
                    this.lblPreviewHeader.Text = Resources.ResourceMessageLang.NoiDungXemTruoc;
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void ApplyTemplateSelection(ZaloTemplateADO template)
        {
            try
            {
                this.selectedTemplate = template;
                UpdateQualityBadge(template != null ? template.Quality : null);
                UpdatePreviewHeader();
                FillPreviewContent(template != null ? (template.PreviewContent ?? string.Empty) : string.Empty);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateQualityBadge(string quality)
        {
            try
            {
                string q = (quality ?? string.Empty).Trim().ToUpperInvariant();
                Color color;
                string text;
                string tooltip;
                if (q == "HIGH")
                {
                    text = "●●● HIGH";
                    color = Color.FromArgb(34, 139, 34);
                    tooltip = Resources.ResourceMessageLang.QualityHighTooltip;
                }
                else if (q == "MEDIUM")
                {
                    text = "●●○ MEDIUM";
                    color = Color.FromArgb(255, 140, 0);
                    tooltip = Resources.ResourceMessageLang.QualityMediumTooltip;
                }
                else if (q == "LOW")
                {
                    text = "●○○ LOW";
                    color = Color.FromArgb(220, 20, 60);
                    tooltip = Resources.ResourceMessageLang.QualityLowTooltip;
                }
                else
                {
                    text = "—";
                    color = Color.Gray;
                    tooltip = string.Empty;
                }

                this.lblQualityBadge.Text = text;
                this.lblQualityBadge.Appearance.ForeColor = color;
                this.lblQualityBadge.Appearance.Options.UseForeColor = true;
                this.lblQualityBadge.ToolTip = tooltip;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdatePreviewHeader()
        {
            try
            {
                this.lblPreviewHeader.Text = !string.IsNullOrEmpty(this.sampleHeaderText)
                    ? this.sampleHeaderText
                    : Resources.ResourceMessageLang.NoiDungXemTruoc;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Fill placeholder `{{key}}` trong template bằng giá trị thực từ BN mẫu,
        /// đồng thời highlight vùng được fill bằng màu vàng trong RichTextBox.
        /// </summary>
        private void FillPreviewContent(string templateContent)
        {
            try
            {
                this.rtxtPreview.Clear();

                if (string.IsNullOrEmpty(templateContent))
                {
                    return;
                }

                var regex = new Regex(PLACEHOLDER_PATTERN);
                int lastIndex = 0;

                foreach (Match match in regex.Matches(templateContent))
                {
                    if (match.Index > lastIndex)
                    {
                        AppendPlainSegment(templateContent.Substring(lastIndex, match.Index - lastIndex));
                    }

                    string key = match.Groups[1].Value;
                    string replacement;
                    if (this.sampleDataMap != null && this.sampleDataMap.TryGetValue(key, out replacement)
                        && !string.IsNullOrEmpty(replacement))
                    {
                        AppendHighlightedSegment(replacement);
                    }
                    else
                    {
                        // Không có data sample — giữ nguyên placeholder để user biết field thiếu
                        AppendHighlightedSegment(match.Value);
                    }

                    lastIndex = match.Index + match.Length;
                }

                if (lastIndex < templateContent.Length)
                {
                    AppendPlainSegment(templateContent.Substring(lastIndex));
                }

                this.rtxtPreview.SelectionStart = 0;
                this.rtxtPreview.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void AppendPlainSegment(string text)
        {
            int start = this.rtxtPreview.TextLength;
            this.rtxtPreview.AppendText(text);
            this.rtxtPreview.Select(start, text.Length);
            this.rtxtPreview.SelectionBackColor = Color.White;
            this.rtxtPreview.SelectionColor = Color.Black;
        }

        private void AppendHighlightedSegment(string text)
        {
            int start = this.rtxtPreview.TextLength;
            this.rtxtPreview.AppendText(text);
            this.rtxtPreview.Select(start, text.Length);
            this.rtxtPreview.SelectionBackColor = Color.FromArgb(255, 235, 153); // vàng nhạt
            this.rtxtPreview.SelectionColor = Color.FromArgb(128, 86, 0);
            this.rtxtPreview.SelectionFont = new Font(this.rtxtPreview.Font, FontStyle.Bold);
        }
        #endregion

        #region Events
        private void cboTemplate_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.listTemplate == null || this.listTemplate.Count == 0) return;
                var key = this.cboTemplate.EditValue as string;
                if (string.IsNullOrEmpty(key)) return;

                var template = this.listTemplate.FirstOrDefault(o => o != null && o.TemplateId == key);
                if (template != null)
                {
                    ApplyTemplateSelection(template);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.selectedTemplate == null || string.IsNullOrWhiteSpace(this.selectedTemplate.TemplateId))
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessageLang.VuiLongChonTemplate,
                        MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.SelectedTemplateId = this.selectedTemplate.TemplateId;
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
                this.SelectedTemplateId = null;
                this.DialogResult = DialogResult.Cancel;
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
