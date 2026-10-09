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
        private const string CONFIG_KEY_ZALO_TEMPLATE_PARAMS = "MOS.SMS.ZALO_TEMPLATE_PARAMS";
        /// <summary>Mã điều trị — chỉ gửi khi được ánh xạ (nhánh không ánh xạ giữ 5 tên cũ), giống Backend.</summary>
        private const string LOGICAL_KEY_MA_DIEU_TRI = "ma_dieu_tri";
        // Placeholder trong nội dung template tùy nơi hiển thị: Zalo OA dùng <ten_param>,
        // cổng Zenify (msghub) dùng {{ten_param}}, OneSMS trả về danh sách dạng [ten_param].
        // Bắt cả 3 dạng để nội dung viện dán vào cấu hình nào cũng preview được.
        private const string PLACEHOLDER_PATTERN =
            @"<\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*>|\{\{\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*\}\}|\[\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*\]";
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
        /// Dựng map tên tham số → giá trị cho phần xem trước, từ BN đầu tiên trong danh sách đã tích,
        /// GIỐNG HỆT cách Backend dựng template_data khi gửi nhắc tái khám
        /// (ZaloAppointmentReminderService: available + BuildTemplateData), để xem trước phản ánh đúng tin sẽ gửi:
        ///   - 5 giá trị logical: ho_ten (cắt 30 ký tự), so_dien_thoai (cắt 15, ưu tiên di động BN → điện thoại BN →
        ///     di động người thân → điện thoại người thân), ma_benh_nhan, ngay_tai_kham (APPOINTMENT_DATE, rỗng thì ngày
        ///     hôm nay như Backend), khoa_kham (TÊN KHOA của các phòng hẹn, cách nhau dấu phẩy).
        ///   - ma_dieu_tri (TREATMENT_CODE, việc 57005 mẫu 600882): CHỈ dùng khi có trong ánh xạ, giống Backend.
        ///   - Có MOS.SMS.ZALO_TEMPLATE_PARAMS: CHỈ có các tên tham số thật đã ánh xạ (Backend chỉ gửi các tên này);
        ///     không có: 5 tên logical đầu (không có ma_dieu_tri).
        ///   - Tên tham số phân biệt hoa thường (Zalo/Zenify so khớp đúng chữ).
        /// Chỗ điền không có trong map (hoặc giá trị rỗng) được giữ nguyên trong xem trước = tin thật sẽ thiếu tham số đó.
        /// </summary>
        private void BuildSampleDataMap()
        {
            this.sampleDataMap = new Dictionary<string, string>(StringComparer.Ordinal);
            try
            {
                if (this.selectedTreatments == null || this.selectedTreatments.Count == 0)
                {
                    this.sampleHeaderText = string.Empty;
                    return;
                }

                var sample = this.selectedTreatments[0];

                Dictionary<string, string> available = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                available["ho_ten"] = Truncate(sample.TDL_PATIENT_NAME, 30);
                available["so_dien_thoai"] = Truncate(PickPhone(sample), 15);
                available["ma_benh_nhan"] = sample.TDL_PATIENT_CODE ?? string.Empty;
                available["ngay_tai_kham"] = FormatAppointmentDate(sample.APPOINTMENT_DATE);
                available["khoa_kham"] = LookupDepartmentNames(sample.APPOINTMENT_EXAM_ROOM_IDS);
                available[LOGICAL_KEY_MA_DIEU_TRI] = sample.TREATMENT_CODE ?? string.Empty;

                List<KeyValuePair<string, string>> mapping = ReadTemplateParamMapping();
                if (mapping.Count > 0)
                {
                    foreach (KeyValuePair<string, string> kv in mapping)
                    {
                        string value;
                        available.TryGetValue(kv.Key, out value);
                        this.sampleDataMap[kv.Value] = value ?? string.Empty;
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, string> kv in available)
                    {
                        if (string.Equals(kv.Key, LOGICAL_KEY_MA_DIEU_TRI, StringComparison.OrdinalIgnoreCase)) continue;
                        this.sampleDataMap[kv.Key] = kv.Value ?? string.Empty;
                    }
                }

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

        /// <summary>
        /// Đọc MOS.SMS.ZALO_TEMPLATE_PARAMS dạng "logical=ten_param_that|...". Tách theo cách an toàn mà thiết kế
        /// việc 57005 yêu cầu Backend dùng (cắt tối đa 2 phần, bỏ khoảng trắng, bỏ cặp sai). Giống Backend, ánh xạ
        /// lưu theo khóa LOGICAL: 2 cặp cùng khóa logical thì cặp sau thắng (giữ vị trí cặp đầu).
        /// Cấu hình sai định dạng chỉ bỏ qua cặp đó, không làm hỏng popup.
        /// </summary>
        private static List<KeyValuePair<string, string>> ReadTemplateParamMapping()
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            try
            {
                string raw = HisConfigs.Get<string>(CONFIG_KEY_ZALO_TEMPLATE_PARAMS);
                if (string.IsNullOrWhiteSpace(raw)) return result;

                Dictionary<string, int> indexByLogicalKey = new Dictionary<string, int>(StringComparer.Ordinal);
                foreach (string entry in raw.Split('|'))
                {
                    if (string.IsNullOrWhiteSpace(entry)) continue;

                    int idx = entry.IndexOf('=');
                    if (idx <= 0) continue;

                    string logicalKey = entry.Substring(0, idx).Trim();
                    string realName = entry.Substring(idx + 1).Trim();
                    if (logicalKey.Length == 0 || realName.Length == 0) continue;

                    int existing;
                    if (indexByLogicalKey.TryGetValue(logicalKey, out existing))
                    {
                        result[existing] = new KeyValuePair<string, string>(logicalKey, realName);
                    }
                    else
                    {
                        indexByLogicalKey[logicalKey] = result.Count;
                        result.Add(new KeyValuePair<string, string>(logicalKey, realName));
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Ngày hẹn giống Backend (ResolveAppointmentDate): lấy APPOINTMENT_DATE, rỗng hoặc không đổi được thì ngày hôm nay.</summary>
        private static string FormatAppointmentDate(long? appointmentDate)
        {
            try
            {
                DateTime? date = null;
                if (appointmentDate.HasValue)
                {
                    date = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentDate.Value);
                }
                return Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(date ?? DateTime.Today) ?? string.Empty;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Tên KHOA của các phòng hẹn khám (APPOINTMENT_EXAM_ROOM_IDS dạng "12,34,56"), giống Backend
        /// (ResolveDepartmentName): lấy V_HIS_ROOM có ID thuộc danh sách, DEPARTMENT_NAME khác rỗng, bỏ trùng, nối bằng ",".
        /// </summary>
        private static string LookupDepartmentNames(string roomIdsCsv)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomIdsCsv)) return string.Empty;

                List<long> roomIds = new List<long>();
                foreach (string token in roomIdsCsv.Split(','))
                {
                    long roomId;
                    if (long.TryParse(token.Trim(), out roomId) && roomId > 0) roomIds.Add(roomId);
                }
                if (roomIds.Count == 0) return string.Empty;

                List<V_HIS_ROOM> rooms = BackendDataWorker.Get<V_HIS_ROOM>();
                if (rooms == null) return string.Empty;

                return string.Join(",", rooms
                    .Where(r => r != null && roomIds.Contains(r.ID) && !string.IsNullOrWhiteSpace(r.DEPARTMENT_NAME))
                    .Select(r => r.DEPARTMENT_NAME)
                    .Distinct()
                    .ToArray());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return string.Empty;
            }
        }

        /// <summary>Số điện thoại giống Backend: di động BN → điện thoại BN → di động người thân → điện thoại người thân.</summary>
        private static string PickPhone(TreatmentAppointmentADO t)
        {
            if (t == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_MOBILE)) return t.TDL_PATIENT_MOBILE.Trim();
            if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_PHONE)) return t.TDL_PATIENT_PHONE.Trim();
            if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_RELATIVE_MOBILE)) return t.TDL_PATIENT_RELATIVE_MOBILE.Trim();
            if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_RELATIVE_PHONE)) return t.TDL_PATIENT_RELATIVE_PHONE.Trim();
            return string.Empty;
        }

        /// <summary>Cắt độ dài giống Backend (Truncate): bỏ khoảng trắng hai đầu rồi cắt tối đa maxLength ký tự.</summary>
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            string trimmed = value.Trim();
            return trimmed.Length > maxLength ? trimmed.Substring(0, maxLength) : trimmed;
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
                    if (value == (int)EnumZaloEnable.ZenifyZns)
                        return Resources.ResourceMessageLang.GatewayZenifyZns;
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
                    this.btnConfirm.Enabled = true;
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
                    ShowNoTemplateReason(param);
                }

                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Không lấy được template nào: nói rõ lý do cho người dùng thay vì để combo rỗng im lặng.
        /// Ưu tiên thông báo do Backend trả về (chưa bật Zalo, thiếu cấu hình gateway, chưa khai danh sách
        /// template với gateway Zenify vì Zenify không có API liệt kê template).
        /// </summary>
        private void ShowNoTemplateReason(CommonParam param)
        {
            try
            {
                string reason = null;
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    reason = string.Join(Environment.NewLine, param.Messages.ToArray());
                }
                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = Resources.ResourceMessageLang.KhongLayDuocDanhSachTemplateZalo;
                }

                this.lblNote.Text = reason;
                XtraMessageBox.Show(
                    reason,
                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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

                    // Group 1 = <ten_param>, group 2 = {{ten_param}}, group 3 = [ten_param]
                    string key = match.Groups[1].Success
                        ? match.Groups[1].Value
                        : (match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value);
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
