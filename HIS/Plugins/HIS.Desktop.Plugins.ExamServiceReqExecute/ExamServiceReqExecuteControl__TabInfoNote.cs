/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab Info — Ghi chú KCB (HIS_PATIENT.NOTE)
 *
 * Thiết kế PTTK:
 *   - Nhãn "Ghi chú KCB" (Label) — tiêu đề
 *   - Ô Ghi chú KCB (Textbox đa dòng, max 4000 ký tự)
 *   - Nút "Lưu" (Button) — lưu độc lập với thao tác lưu thông tin khám
 *
 * Hành vi xử lý:
 *   1. Khi mở màn hình / chọn BN → tải HIS_PATIENT.NOTE vào MemoEdit
 *   2. Khi nhấn "Lưu" → PUT /api/HisPatient/Update + thông báo
 *   3. Khi chuyển BN → tải lại theo BN mới (plugin re-instance)
 *   4. Khi nhập quá 4000 ký tự → MemoEdit.MaxLength = 4000 chặn
 *   5. Khi thoát chưa Lưu → hủy nội dung, không động DB
 *
 * Controls (khai báo trong ExamServiceReqExecuteControl.designer.cs):
 *   - pnlNoteKcb (PanelControl, Dock Top, 220px)
 *   - lblNoteKcbCaption (LabelControl, "Ghi chú KCB")
 *   - memoNoteKcb (MemoEdit, MaxLength=4000)
 *   - btnSaveNoteKcb (SimpleButton, "Lưu (Ctrl S)")
 */
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Windows.Forms;
using MessageUtil = HIS.Desktop.LibraryMessage.MessageUtil;
using LibMessage = HIS.Desktop.LibraryMessage.Message;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl
    {
        private const int NOTE_KCB_MAX_LENGTH = 4000;

        /// <summary>
        /// Cập nhật text label + button theo ngôn ngữ hiện tại.
        /// Controls đã được khai báo trong Designer.cs với text mặc định "Ghi chú KCB" / "Lưu (Ctrl S)" (Tiếng Việt).
        /// </summary>
        private void ApplyNoteKcbLanguage()
        {
            try
            {
                if (this.lblNoteKcbCaption != null)
                {
                    string caption = ResourceMessage.GhiChuKCB;
                    if (!string.IsNullOrEmpty(caption)) this.lblNoteKcbCaption.Text = caption;
                }
                if (this.btnSaveNoteKcb != null)
                {
                    string text = ResourceMessage.Luu;
                    if (!string.IsNullOrEmpty(text)) this.btnSaveNoteKcb.Text = text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tải nội dung Ghi chú KCB từ CurrentPatient vào MemoEdit.
        /// Gọi sau LoadCurrentPatient().
        /// </summary>
        private void LoadNoteKcbFromCurrentPatient()
        {
            try
            {
                if (this.memoNoteKcb == null) return;
                this.memoNoteKcb.Text = (this.CurrentPatient != null && this.CurrentPatient.PT_MEDICAL_NOTE != null)
                    ? this.CurrentPatient.PT_MEDICAL_NOTE
                    : string.Empty;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveNoteKcb_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CurrentPatient == null)
                {
                    XtraMessageBox.Show(
                        ResourceMessage.GhiChuKcbChuaCoBenhNhan,
                        MessageUtil.GetMessage(LibMessage.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string note = this.memoNoteKcb != null ? (this.memoNoteKcb.Text ?? string.Empty) : string.Empty;
                if (note.Length > NOTE_KCB_MAX_LENGTH)
                {
                    note = note.Substring(0, NOTE_KCB_MAX_LENGTH);
                }

                HIS_PATIENT updateDto = new HIS_PATIENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(updateDto, this.CurrentPatient);
                updateDto.PT_MEDICAL_NOTE = note;

                CommonParam paramUpdate = new CommonParam();
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug(
                    Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDto), updateDto));

                HIS_PATIENT result = new BackendAdapter(paramUpdate).Post<HIS_PATIENT>(
                    HisRequestUriStore.HIS_PATIENT_UPDATE,
                    ApiConsumers.MosConsumer,
                    updateDto,
                    paramUpdate);

                WaitingManager.Hide();

                bool success = (result != null);
                if (success)
                {
                    this.CurrentPatient.PT_MEDICAL_NOTE = result.PT_MEDICAL_NOTE;
                    if (this.memoNoteKcb != null) this.memoNoteKcb.Text = result.PT_MEDICAL_NOTE ?? string.Empty;

                    XtraMessageBox.Show(
                        ResourceMessage.LuuGhiChuKcbThanhCong,
                        MessageUtil.GetMessage(LibMessage.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show(
                        ResourceMessage.LuuGhiChuKcbThatBai,
                        MessageUtil.GetMessage(LibMessage.Enum.TieuDeCuaSoThongBaoLaLoi),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramUpdate);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
