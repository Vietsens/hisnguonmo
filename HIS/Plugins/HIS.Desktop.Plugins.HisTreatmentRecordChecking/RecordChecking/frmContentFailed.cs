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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    /// <summary>
    /// Hop thoai "Noi dung khong dat": Luu tam (viec 57750) / Dong y (tu choi chinh thuc) / Bo qua.
    /// </summary>
    public partial class frmContentFailed : FormBase
    {
        /// <summary>Key prefix cua cac nhan thuoc hop thoai nay trong Lang.*.resx.</summary>
        private const string LANG_PREFIX = "frmContentFailed.";

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? treatmentId;
        private Action<bool> dlgIsSuccess;

        /// <summary>Ban tam hien co cua ho so, do man chinh truyen vao. Khong bao gio null.</summary>
        private RecordDraftADO currentDraft;

        /// <summary>Goi sau khi luu / xoa ban tam thanh cong de man chinh cap nhat dau hieu "co ban tam".</summary>
        private Action<RecordDraftADO> dlgDraftChanged;

        /// <summary>Noi dung luc mo hop thoai (= ban tam). Khac noi dung hien tai => "da sua, chua luu".</summary>
        private string baselineText = "";

        /// <summary>True sau khi Dong y / Luu tam thanh cong hoac nguoi dung da xac nhan bo qua => dong khong hoi lai.</summary>
        private bool closeConfirmed = false;

        /// <summary>Provider hiển thị cảnh báo validate ngay tại control lý do (form không có sẵn error provider).</summary>
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProviderReason =
            new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();

        public frmContentFailed(Inventec.Desktop.Common.Modules.Module _moduleData, long? treatmentId, Action<bool> dlgIsSuccess)
            : this(_moduleData, treatmentId, dlgIsSuccess, null, null)
        {
        }

        /// <param name="currentDraft">Ban tam hien co cua ho so (null = chua co).</param>
        /// <param name="dlgDraftChanged">Callback khi ban tam thay doi (luu / xoa) - khong doi trang thai ho so.</param>
        internal frmContentFailed(Inventec.Desktop.Common.Modules.Module _moduleData, long? treatmentId, Action<bool> dlgIsSuccess,
            RecordDraftADO currentDraft, Action<RecordDraftADO> dlgDraftChanged)
            : base(_moduleData)
        {
            InitializeComponent();
            this.moduleData = _moduleData;
            this.treatmentId = treatmentId;
            this.dlgIsSuccess = dlgIsSuccess;
            this.currentDraft = currentDraft ?? new RecordDraftADO();
            this.dlgDraftChanged = dlgDraftChanged;
            SetIcon();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmContentFailed_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                SetCaptionByLanguageKey();

                // Nap san ban tam (neu co), con tro dat o cuoi de ghi tiep
                txtRejectReason.Text = this.currentDraft.HasDraft ? this.currentDraft.Content : "";
                this.baselineText = txtRejectReason.Text;
                ShowDraftInfo();
                UpdateCounter(txtRejectReason.Text);

                this.ActiveControl = txtRejectReason;
                txtRejectReason.SelectionStart = txtRejectReason.Text.Length;
                txtRejectReason.SelectionLength = 0;
                txtRejectReason.Focus();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Language

        private string GetLangValue(string keyWithoutPrefix)
        {
            try
            {
                if (Resources.ResourceLanguageManager.LanguageResource == null)
                {
                    Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                        "HIS.Desktop.Plugins.HisTreatmentRecordChecking.Resources.Lang",
                        typeof(frmContentFailed).Assembly);
                }

                return Inventec.Common.Resource.Get.Value(
                    LANG_PREFIX + keyWithoutPrefix,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                string text = GetLangValue("btnSaveDraft.Text");
                if (!String.IsNullOrEmpty(text)) this.btnSaveDraft.Text = text;
                this.btnSaveDraft.ToolTip = GetLangValue("btnSaveDraft.ToolTip");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Draft display

        /// <summary>Dong chu thich duoi o nhap: "Ban tam luu luc ... boi ..." hoac "Chua co ban tam".</summary>
        private void ShowDraftInfo()
        {
            try
            {
                if (this.currentDraft.HasDraft)
                {
                    string time = this.currentDraft.Time.HasValue
                        ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentDraft.Time.Value)
                        : "";
                    string user = !String.IsNullOrEmpty(this.currentDraft.Username)
                        ? this.currentDraft.Username
                        : this.currentDraft.Loginname;
                    lblDraftInfo.Text = String.Format(GetLangValue("lblDraftInfo.Format"), time, user);
                }
                else
                {
                    lblDraftInfo.Text = GetLangValue("lblDraftInfo.NoDraft");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Bo dem "da dung / toi da byte"; do mau canh bao khi vuot gioi han.</summary>
        private void UpdateCounter(string text)
        {
            try
            {
                int bytes = RecordDraftADO.CountBytes(text);
                lblCounter.Text = String.Format(GetLangValue("lblCounter.Format"),
                    bytes.ToString("N0"), RecordDraftADO.MAX_BYTES.ToString("N0"));
                lblCounter.Appearance.ForeColor = bytes > RecordDraftADO.MAX_BYTES ? Color.Red : SystemColors.GrayText;
                lblCounter.Appearance.Options.UseForeColor = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRejectReason_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            try
            {
                UpdateCounter(e.NewValue != null ? e.NewValue.ToString() : "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate / confirm

        /// <summary>Chan khi noi dung vuot gioi han cua cot tren may chu (4000 byte UTF-8).</summary>
        private bool IsValidLength(string text)
        {
            if (RecordDraftADO.CountBytes(text) <= RecordDraftADO.MAX_BYTES) return true;

            dxErrorProviderReason.SetError(txtRejectReason,
                String.Format(Resources.ResourceMessage.NoiDungKhongDatVuotDoDai, RecordDraftADO.MAX_BYTES.ToString("N0")),
                DevExpress.XtraEditors.DXErrorProvider.ErrorType.Critical);
            txtRejectReason.Focus();
            return false;
        }

        private string GetTitleThongBao()
        {
            return HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
        }

        /// <summary>
        /// Noi dung khac luc mo (da sua, chua luu) => hoi "bo qua?". Tra ve true khi duoc phep dong.
        /// </summary>
        private bool ConfirmDiscardChanges()
        {
            string current = (txtRejectReason.Text ?? "").Trim();
            string baseline = (this.baselineText ?? "").Trim();
            if (current == baseline) return true;

            return XtraMessageBox.Show(
                Resources.ResourceMessage.NoiDungChuaDuocLuuBoQua,
                GetTitleThongBao(),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        #endregion

        /// <summary>
        /// Luu tam: ghi noi dung len may chu lam ban tam cua ho so. KHONG doi trang thai ho so.
        /// Noi dung trong + dang co ban tam => hoi roi xoa ban tam. Thanh cong => dong hop thoai.
        /// </summary>
        private void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                dxErrorProviderReason.ClearErrors();
                string text = (txtRejectReason.Text ?? "").Trim();

                if (text.Length == 0)
                {
                    if (!this.currentDraft.HasDraft)
                    {
                        dxErrorProviderReason.SetError(txtRejectReason,
                            Resources.ResourceMessage.ChuaNhapNoiDungKhongDat,
                            DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                        txtRejectReason.Focus();
                        return;
                    }
                    if (XtraMessageBox.Show(
                            Resources.ResourceMessage.XoaBanTamNoiDungKhongDat,
                            GetTitleThongBao(),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                else if (!IsValidLength(text))
                {
                    return;
                }

                CommonParam param = new CommonParam();
                HisTreatmentRejectStoreDraftSDO data = new HisTreatmentRejectStoreDraftSDO();
                data.TreatmentId = (long)this.treatmentId;
                data.Draft = text.Length == 0 ? null : text;

                Inventec.Common.Logging.LogSystem.Debug("SaveRejectStoreDraft INPUT____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT>(
                    RecordCheckingUriStore.MOSHIS_HIS_TREATMENT_SAVE_REJECT_STORE_DRAFT, ApiConsumers.MosConsumer, data, param);

                bool success = resultData != null;
                MessageManager.Show(this, param, success);

                // That bai (mat ket noi, may chu tu choi...) => giu hop thoai va noi dung dang nhap
                if (!success) return;

                // Lay dung gia tri may chu da ghi (noi dung, thoi diem, nguoi luu); xoa => cac cot null
                this.currentDraft = RecordDraftADO.FromTreatment(resultData);

                if (this.dlgDraftChanged != null)
                {
                    this.dlgDraftChanged(this.currentDraft);
                }

                this.closeConfirmed = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAgree_Click(object sender, EventArgs e)
        {
            try
            {
                dxErrorProviderReason.ClearErrors();

                // Bắt buộc nhập lý do không đạt — tránh truyền rỗng sang backend
                if (string.IsNullOrWhiteSpace(txtRejectReason.Text))
                {
                    dxErrorProviderReason.SetError(txtRejectReason,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.ThieuTruongDuLieuBatBuoc),
                        DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                    txtRejectReason.Focus();
                    return;
                }

                // Cot ly do tu choi tren may chu gioi han 4000 byte
                if (!IsValidLength(txtRejectReason.Text.Trim()))
                {
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = false;

                HisTreatmentRejectStoreSDO RejectStoreSDO = new HisTreatmentRejectStoreSDO();
                RejectStoreSDO.RejectReason = txtRejectReason.Text.Trim();
                RejectStoreSDO.TreatmentId = (long)this.treatmentId;

                Inventec.Common.Logging.LogSystem.Debug("RejectStore INPUT____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => RejectStoreSDO), RejectStoreSDO));

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/RejectStore", ApiConsumers.MosConsumer, RejectStoreSDO, param);

                if (resultData != null)
                {
                    success = true;
                }

                MessageManager.Show(this, param, success);

                // Chỉ trả kết quả + đóng form khi lưu thành công; thất bại giữ popup để nhập lại
                if (success)
                {
                    // May chu da xoa ban tam khi tu choi chinh thuc => man chinh nap lai ho so qua dlgIsSuccess
                    if (dlgIsSuccess != null)
                    {
                        dlgIsSuccess(success);
                    }
                    this.closeConfirmed = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnToIgnore_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ConfirmDiscardChanges()) return;
                this.closeConfirmed = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dong bang nut X cua cua so: xu ly nhu Bo qua.</summary>
        private void frmContentFailed_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.closeConfirmed) return;
                if (e.CloseReason != CloseReason.UserClosing) return;
                if (!ConfirmDiscardChanges())
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
