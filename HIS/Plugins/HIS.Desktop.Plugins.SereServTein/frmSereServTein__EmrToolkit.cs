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
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.Library.EmrToolkitImport;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Config;
using HIS.Desktop.Plugins.Library.EmrToolkitImport.Models.LabResult;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.SereServTein
{
    /// <summary>
    /// Tab xem kết quả cận lâm sàng do cơ sở khác chia sẻ trên cổng EMRToolkit.
    /// Chỉ ĐỌC — việc đẩy kết quả của viện lên cổng do tiến trình nền trong MOS đảm nhiệm.
    /// </summary>
    public partial class frmSereServTein
    {
        #region Declare

        /// <summary>Số định danh bệnh nhân dùng để tra cứu trên cổng (CCCD, thiếu thì CMND)</summary>
        private string emrPatientIdentifier;

        /// <summary>Các bản ghi còn hiệu lực trên cổng — dữ liệu chỉ đọc</summary>
        private List<LabResultValidityADO> listEmrValidity;

        /// <summary>Tab chỉ gọi API ở lần mở đầu tiên (lazy-load)</summary>
        private bool isEmrToolkitLoaded;

        /// <summary>
        /// Cổng chặn xin OTP nhanh hơn 5 giây một lần cho cùng bệnh nhân (HTTP 429)
        /// nên khóa nút trong khoảng đó thay vì để cổng từ chối.
        /// </summary>
        private DateTime emrLastRequestViewTime = DateTime.MinValue;
        private const int EMR_REQUEST_VIEW_INTERVAL_SECOND = 5;

        #endregion

        /// <summary>
        /// Khởi tạo tab. Gọi trong Load, SAU khi đã có currentServiceReq và currentTreatment.
        /// Chưa cấu hình liên thông thì ẩn hẳn tab, người dùng không thấy chức năng.
        /// </summary>
        private void InitEmrToolkitTab()
        {
            try
            {
                if (!EmrToolkitSubclinicalProcessor.IsViewEnable(EmrToolkitSubclinicalType.TEST))
                {
                    this.xtraTabControl1.TabPages.Remove(this.xtraTabPageEmrToolkit);
                    return;
                }

                SetCaptionEmrToolkitByLanguageKey();

                this.gridViewEmrValidity.BeginUpdate();
                try
                {
                    this.grdEmrValidity.DataSource = null;
                }
                finally
                {
                    this.gridViewEmrValidity.EndUpdate();
                }

                this.xtraTabControl1.SelectedPageChanged += xtraTabControl1_SelectedPageChanged_EmrToolkit;
                this.FormClosed += frmSereServTein_FormClosed_EmrToolkit;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionEmrToolkitByLanguageKey()
        {
            try
            {
                this.xtraTabPageEmrToolkit.Text = GetLangEmrToolkit("frmSereServTein.xtraTabPageEmrToolkit.Text");
                this.lciEmrPatientId.Text = GetLangEmrToolkit("frmSereServTein.lciEmrPatientId.Text");
                this.btnViewEmrToolkit.Text = GetLangEmrToolkit("frmSereServTein.btnViewEmrToolkit.Text");
                this.gcEmrMaPhieuXN.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrMaPhieuXN.Caption");
                this.gcEmrMaDichVu.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrMaDichVu.Caption");
                this.gcEmrLoaiXetNghiem.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrLoaiXetNghiem.Caption");
                this.gcEmrMaCskcb.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrMaCskcb.Caption");
                this.gcEmrValidUntil.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrValidUntil.Caption");
                this.gcEmrQcDatChuan.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrQcDatChuan.Caption");
                this.gcEmrNgayTao.Caption = GetLangEmrToolkit("frmSereServTein.gcEmrNgayTao.Caption");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string GetLangEmrToolkit(string key)
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
                return key;
            }
        }

        /// <summary>Chỉ tra cứu cổng khi người dùng thực sự mở tab</summary>
        private void xtraTabControl1_SelectedPageChanged_EmrToolkit(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page != this.xtraTabPageEmrToolkit || this.isEmrToolkitLoaded) return;

                this.isEmrToolkitLoaded = true;
                LoadEmrValidity();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Tra cứu các bản ghi còn hiệu lực của bệnh nhân trên cổng</summary>
        private void LoadEmrValidity()
        {
            CommonParam param = new CommonParam();
            try
            {
                this.emrPatientIdentifier = GetEmrPatientIdentifier();
                this.lblEmrPatientId.Text = this.emrPatientIdentifier;

                if (string.IsNullOrWhiteSpace(this.emrPatientIdentifier))
                {
                    this.btnViewEmrToolkit.Enabled = false;
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.BenhNhanChuaCoSoDinhDanh,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                WaitingManager.Show();
                string message;
                this.listEmrValidity = new EmrToolkitSubclinicalProcessor().CheckValidity(this.emrPatientIdentifier, out message);
                WaitingManager.Hide();

                FillEmrValidityToGrid();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    XtraMessageBox.Show(message,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (this.listEmrValidity.Count == 0)
                {
                    // Không còn kết quả hiệu lực là trạng thái bình thường, không phải lỗi
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongCoKetQuaConHieuLuc,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void FillEmrValidityToGrid()
        {
            try
            {
                this.gridViewEmrValidity.BeginUpdate();
                try
                {
                    this.grdEmrValidity.DataSource = this.listEmrValidity;
                }
                finally
                {
                    this.gridViewEmrValidity.EndUpdate();
                }

                bool hasData = this.listEmrValidity != null && this.listEmrValidity.Count > 0;
                this.btnViewEmrToolkit.Enabled = hasData;
                if (hasData) this.gridViewEmrValidity.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Số định danh bệnh nhân: ưu tiên CCCD, thiếu thì CMND</summary>
        private string GetEmrPatientIdentifier()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (this.currentTreatment == null) return null;

                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = this.currentTreatment.PATIENT_ID;
                List<HIS_PATIENT> patients = new BackendAdapter(param)
                    .Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param);

                HIS_PATIENT patient = patients != null ? patients.FirstOrDefault() : null;
                if (patient == null) return null;

                if (!string.IsNullOrWhiteSpace(patient.CCCD_NUMBER)) return patient.CCCD_NUMBER.Trim();
                if (!string.IsNullOrWhiteSpace(patient.CMND_NUMBER)) return patient.CMND_NUMBER.Trim();
                return null;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }

        private void btnViewEmrToolkit_Click(object sender, EventArgs e)
        {
            try
            {
                ViewEmrResultProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewEmrValidity_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ViewEmrResultProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xin OTP cho bản ghi đang chọn rồi tải phiếu kết quả về hiển thị.
        /// Cổng tiêu thụ phiên OTP ngay lần tải đầu tiên nên muốn xem lại phải xin mã mới.
        /// </summary>
        private void ViewEmrResultProcess()
        {
            try
            {
                LabResultValidityADO selected = this.gridViewEmrValidity.GetFocusedRow() as LabResultValidityADO;
                if (selected == null)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonBanGhiDeXem,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                double elapsed = (DateTime.Now - this.emrLastRequestViewTime).TotalSeconds;
                if (elapsed < EMR_REQUEST_VIEW_INTERVAL_SECOND)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.XinMaOtpQuaNhanh,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                this.btnViewEmrToolkit.Enabled = false;
                try
                {
                    this.emrLastRequestViewTime = DateTime.Now;

                    LabResultPdfResultADO result = new EmrToolkitSubclinicalProcessor()
                        .RequestViewAndDownloadPdf(this.emrPatientIdentifier, selected.MaPhieuXN, null, this);

                    if (result == null || !result.Success)
                    {
                        // Không có thông báo nghĩa là người dùng tự hủy ở cửa sổ nhập OTP
                        if (result != null && !string.IsNullOrWhiteSpace(result.Message))
                        {
                            XtraMessageBox.Show(result.Message,
                                MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    ShowEmrPdf(result);
                    WriteEmrViewLogAction(selected);
                }
                finally
                {
                    this.btnViewEmrToolkit.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Hiển thị phiếu đầu tiên; lô nhiều mẫu phiếu thì báo số tệp đã tải về</summary>
        private void ShowEmrPdf(LabResultPdfResultADO result)
        {
            try
            {
                if (result.PdfFilePaths.Count == 0) return;

                this.pdfViewerEmr.DetachStreamAfterLoadComplete = true;
                this.pdfViewerEmr.LoadDocument(result.PdfFilePaths[0]);

                if (result.PdfFilePaths.Count > 1)
                {
                    XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.DaTaiNhieuPhieuKetQua,
                            result.PdfFilePaths.Count, result.OutputFolder),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>Truy vết ai đã xem dữ liệu bệnh nhân từ cơ sở khác — KHÔNG ghi số định danh</summary>
        private void WriteEmrViewLogAction(LabResultValidityADO selected)
        {
            try
            {
                Inventec.Common.Logging.LogAction.Info(string.Format("{0}____{1}____{2}____{3}",
                    this.moduleLink,
                    "ViewEmrToolkitSubclinical",
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    selected != null ? selected.MaPhieuXN : ""));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>Giải phóng tài liệu PDF khi đóng form để không giữ tệp tạm</summary>
        private void frmSereServTein_FormClosed_EmrToolkit(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.pdfViewerEmr != null) this.pdfViewerEmr.CloseDocument();
                this.listEmrValidity = null;
                this.emrPatientIdentifier = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
