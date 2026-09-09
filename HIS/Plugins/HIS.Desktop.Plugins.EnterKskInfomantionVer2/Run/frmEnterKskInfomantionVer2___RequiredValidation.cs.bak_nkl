/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Kiểm tra trường bắt buộc nhập khi Lưu — hiển thị icon cảnh báo (tam giác) kèm nội dung lỗi
 * ngay tại control theo ui_rules (caption Maroon đặt ở LayoutControlItem trong Designer).
 *
 * Phạm vi:
 *  - "Đối tượng" + "Nguồn chi trả" của 3 tab: trên 18 tuổi (cboObject/cboPaymentSource),
 *    dưới 18 tuổi (cboObject3/cboPaymentSource3), trẻ em dưới 6 tuổi (cboObject8/cboPaymentSource8).
 *    Chỉ kiểm tra cặp combo của TAB ĐANG LƯU (mỗi lần Lưu chỉ gửi dữ liệu 1 tab).
 *  - "Lý do khám" (txtLyDoKham): bắt buộc nhập ở MỨC FORM (mọi tab) + giới hạn độ dài.
 *  - Tab trên 18 tuổi (index 1) thêm "Phân loại sức khỏe" (cboHealthExamRank2 —
 *    ô "Phân loại:" ở mục kết luận, cột HIS_KSK_OVER_EIGHTEEN.HEALTH_EXAM_RANK_ID).
 *  - Mục KẾT LUẬN của 3 tab (trên 18, dưới 18, trẻ dưới 6 tuổi): "Phân loại" và "Người khám"
 *    là MỘT CẶP — bắt buộc đủ cả hai khi:
 *      + đã nhập MỘT thông tin kết luận của tab (HasConclusionInput), HOẬC
 *      + đã nhập MỘT trong hai ô của chính cặp đó (nhập Phân loại thì phải có Người khám
 *        và ngược lại)
 *    -> xem IsConclusionPairRequired / ValidateRequiredConclusion.
 *  - Huyết áp (các tab có sinh hiệu): đã nhập 1 ô thì phải nhập đủ CẢ tâm thu + tâm trương
 *    -> xem ValidateBloodPressure.
 *
 * TÔ MÀU: trường bắt buộc VÔ ĐIỀU KIỆN được tô Maroon sẵn trong Designer; trường bắt buộc
 * CÓ ĐIỀU KIỆN (Phân loại / Người kết luận / Huyết áp) được tô ĐỘNG lúc chạy — xem
 * UpdateRequiredHighlight: điều kiện phát sinh thì caption đổi Maroon, hết điều kiện thì trả lại
 * màu mặc định (không bắt người dùng đoán ô nào đang bắt buộc).
 *  - Tab trẻ em dưới 6 tuổi (index 7) thêm 3 trường: "Họ tên người đi cùng trẻ"
 *    (txtAccompanyPersonName8), "Mối quan hệ với trẻ" (rdoAccompanyRelationship8) và
 *    "Kết luận về sức khỏe" (rdoConclusionHealth8 — mục VII. KẾT LUẬN VÀ TƯ VẤN).
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Độ dài tối đa của Lý do khám (HIS_TREATMENT.HOSPITALIZATION_REASON).</summary>
        private const int LY_DO_KHAM_MAX_LENGTH = 500;

        private DXErrorProvider dxErrorProviderRequired;

        /// <summary>Danh sách control đang báo lỗi của lần kiểm tra gần nhất (để focus control đầu tiên).</summary>
        private readonly List<Control> requiredInvalidControls = new List<Control>();

        #region Khởi tạo

        /// <summary>
        /// Tạo DXErrorProvider + gắn sự kiện tự xóa cảnh báo khi người dùng đã nhập lại giá trị
        /// (ui_rules: phải Clear icon/ErrorText khi đã hợp lệ). Gọi 1 lần lúc Load form.
        /// </summary>
        private void InitRequiredValidation()
        {
            try
            {
                if (dxErrorProviderRequired != null) return;
                // KHÔNG dùng new DXErrorProvider(this.components): Designer của form này khai
                // `components = null` và KHÔNG bao giờ khởi tạo Container, nên ctor nhận IContainer
                // ném NullReferenceException ngay (DXErrorProvider..ctor gọi container.Add) → cả cụm
                // cảnh báo trường bắt buộc chết âm thầm (chỉ thấy 1 dòng WARN trong log).
                dxErrorProviderRequired = new DXErrorProvider();
                dxErrorProviderRequired.ContainerControl = this;

                WireRequiredClearEvent(cboObject);
                WireRequiredClearEvent(cboPaymentSource);
                WireRequiredClearEvent(cboObject3);
                WireRequiredClearEvent(cboPaymentSource3);
                WireRequiredClearEvent(cboObject8);
                WireRequiredClearEvent(cboPaymentSource8);
                WireRequiredClearEvent(txtLyDoKham);
                // Mục kết luận 3 tab — Phân loại + Người kết luận.
                WireRequiredClearEvent(cboHealthExamRank2);
                WireRequiredClearEvent(cboConcluderLoginName2);
                WireRequiredClearEvent(cboHealthExamRank3);
                WireRequiredClearEvent(cboConcluderLoginName3);
                WireRequiredClearEvent(cboHealthExamRank8);
                WireRequiredClearEvent(cboConcluder8);
                // Huyết áp 4 tab: định kỳ / trên 18 / dưới 18 / nghề nghiệp.
                WireRequiredClearEvent(spnBloodPressureMax);
                WireRequiredClearEvent(spnBloodPressureMin);
                WireRequiredClearEvent(spnBloodPressureMax2);
                WireRequiredClearEvent(spnBloodPressureMin2);
                WireRequiredClearEvent(spnBloodPressureMax3);
                WireRequiredClearEvent(spnBloodPressureMin3);
                WireRequiredClearEvent(spnBloodPressureMax7);
                WireRequiredClearEvent(spnBloodPressureMin7);

                // Ô "trigger" của mục kết luận: đổi nội dung -> tính lại màu bắt buộc.
                WireRequiredHighlightEvent(txtHealthExamRankDescription2);
                WireRequiredHighlightEvent(txtDiseases2);
                WireRequiredHighlightEvent(txtNormalHealth3);
                WireRequiredHighlightEvent(txtProblemHealth3);
                WireRequiredHighlightEvent(rdoConclusionHealth8);
                WireRequiredHighlightEvent(memConclusionDetail8);
                WireRequiredHighlightEvent(memAdviceNextExam8);
                WireRequiredHighlightEvent(checkEdit1);
                // Tab trẻ em dưới 6 tuổi — 3 trường bắt buộc bổ sung.
                WireRequiredClearEvent(txtAccompanyPersonName8);
                WireRequiredClearEvent(rdoAccompanyRelationship8);
                WireRequiredClearEvent(rdoConclusionHealth8);

                // Đổi tab -> bỏ cảnh báo của tab cũ (cảnh báo chỉ có nghĩa với tab đang lưu).
                this.xtraTabControl1.SelectedPageChanged
                    += new DevExpress.XtraTab.TabPageChangedEventHandler(RequiredValidation_TabChanged);

                UpdateRequiredHighlight();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void WireRequiredClearEvent(BaseEdit edit)
        {
            if (edit == null) return;
            // GridLookUpEdit chọn nhiều (Đối tượng) chỉ đổi Text khi tick checkbox, không đổi EditValue
            // -> bắt cả 2 sự kiện.
            edit.EditValueChanged -= RequiredEdit_ValueChanged;
            edit.EditValueChanged += RequiredEdit_ValueChanged;
            edit.TextChanged -= RequiredEdit_ValueChanged;
            edit.TextChanged += RequiredEdit_ValueChanged;
        }

        private void RequiredEdit_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                if (ctrl == null || dxErrorProviderRequired == null) return;
                // RadioGroup: Text không phản ánh việc đã chọn -> xét EditValue.
                RadioGroup rdo = ctrl as RadioGroup;
                bool hasValue = (rdo != null)
                    ? (rdo.EditValue != null && rdo.EditValue != DBNull.Value)
                    : !string.IsNullOrWhiteSpace(ctrl.Text);
                if (hasValue) ClearRequiredError(ctrl);
                UpdateRequiredHighlight();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RequiredValidation_TabChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try { ClearAllRequiredErrors(); UpdateRequiredHighlight(); }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Set / Clear cảnh báo

        private void SetRequiredError(Control ctrl, string message)
        {
            if (ctrl == null || dxErrorProviderRequired == null) return;
            dxErrorProviderRequired.SetError(ctrl, message, ErrorType.Warning);
            if (!requiredInvalidControls.Contains(ctrl)) requiredInvalidControls.Add(ctrl);
        }

        private void ClearRequiredError(Control ctrl)
        {
            if (ctrl == null || dxErrorProviderRequired == null) return;
            dxErrorProviderRequired.SetError(ctrl, "", ErrorType.None);
            requiredInvalidControls.Remove(ctrl);
        }

        private void ClearAllRequiredErrors()
        {
            if (dxErrorProviderRequired != null) dxErrorProviderRequired.ClearErrors();
            requiredInvalidControls.Clear();
        }

        #endregion

        #region Tô màu động cho trường bắt buộc CÓ ĐIỀU KIỆN

        /// <summary>
        /// Tính lại màu caption của các trường bắt buộc CÓ ĐIỀU KIỆN:
        ///  - "Phân loại" + "Người khám" của tab trên 18 / dưới 18 / trẻ dưới 6 tuổi:
        ///    đỏ khi cặp đang bắt buộc (IsConclusionPairRequired) — tức đã nhập nội dung kết luận
        ///    HOẬC đã nhập một trong hai ô của cặp.
        ///  - Huyết áp: ô còn trống đỏ khi ô còn lại đã nhập.
        ///
        /// Tính cho CẢ 3 tab (không chỉ tab đang xem) vì người dùng chuyển tab liên tục; caption tab
        /// ẩn không vẽ nên không tốn gì.
        ///
        /// "Phân loại" tab trên 18 tuổi (layoutControlItem185) bắt buộc VÔ ĐIỀU KIỆN nên
        /// đã Maroon sẵn trong Designer — KHÔNG đổi ở đây.
        /// </summary>
        private void UpdateRequiredHighlight()
        {
            try
            {
                bool conc1 = IsConclusionPairRequired(1);
                bool conc2 = IsConclusionPairRequired(2);
                bool conc7 = IsConclusionPairRequired(7);

                SetCaptionRequired(lciKskConcluder1, conc1);
                SetCaptionRequired(lciHealthExamRank3, conc2);
                SetCaptionRequired(lciKskConcluder2, conc2);
                SetCaptionRequired(lciHealthRank8, conc7);
                SetCaptionRequired(lciConcluder8, conc7);

                // Huyết áp 4 tab (hậu tố control không trùng chỉ số tab — xem ValidateBloodPressure).
                UpdateBloodPressureHighlight(layoutControlItem45, spnBloodPressureMax, spnBloodPressureMin);
                UpdateBloodPressureHighlight(layoutControlItem117, spnBloodPressureMax2, spnBloodPressureMin2);
                UpdateBloodPressureHighlight(layoutControlItem206, spnBloodPressureMax3, spnBloodPressureMin3);
                UpdateBloodPressureHighlight(layoutControlItem498, spnBloodPressureMax7, spnBloodPressureMin7);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Caption "Huyết áp:" đỏ khi CHỬ nhập MỘT trong hai ô (nhập 1 phải đủ 2).
        ///
        /// Hai ô tâm thu / tâm trương dùng CHUNG một caption: LayoutControlItem của ô tâm trương
        /// để TextVisible = false nên tô màu nó không hiện gì — chỉ tô caption của ô tâm thu.
        /// Vị trí ô còn thiếu vẫn chỉ đúng bằng icon cảnh báo lúc Lưu (ValidateBloodPressure).
        /// </summary>
        private void UpdateBloodPressureHighlight(DevExpress.XtraLayout.LayoutControlItem lciCaption,
            SpinEdit spnMax, SpinEdit spnMin)
        {
            bool hasMax = HasSpinValue(spnMax);
            bool hasMin = HasSpinValue(spnMin);
            SetCaptionRequired(lciCaption, hasMax != hasMin);
        }

        /// <summary>
        /// Bật/tắt màu Maroon ở caption của 1 LayoutControlItem.
        /// Tắt = trả UseForeColor về false để lấy lại màu theo skin, KHÔNG hard-code màu đen.
        /// </summary>
        private void SetCaptionRequired(DevExpress.XtraLayout.LayoutControlItem item, bool required)
        {
            try
            {
                if (item == null) return;
                if (required) item.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                item.AppearanceItemCaption.Options.UseForeColor = required;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>SpinEdit đã nhập giá trị hay chưa (giống điều kiện lưu vào HIS_DHST).</summary>
        private bool HasSpinValue(SpinEdit spn)
        {
            return spn != null && spn.EditValue != null && spn.EditValue != DBNull.Value;
        }

        /// <summary>Gắn sự kiện để ô "trigger" đổi nội dung thì tính lại màu bắt buộc.</summary>
        private void WireRequiredHighlightEvent(BaseEdit edit)
        {
            if (edit == null) return;
            edit.EditValueChanged -= RequiredHighlight_ValueChanged;
            edit.EditValueChanged += RequiredHighlight_ValueChanged;
            edit.TextChanged -= RequiredHighlight_ValueChanged;
            edit.TextChanged += RequiredHighlight_ValueChanged;
        }

        private void RequiredHighlight_ValueChanged(object sender, EventArgs e)
        {
            UpdateRequiredHighlight();
        }

        #endregion

        #region Kiểm tra khi Lưu

        /// <summary>
        /// Kiểm tra các trường bắt buộc trước khi Lưu. Trả về true nếu hợp lệ.
        /// Không hợp lệ: gắn icon cảnh báo + nội dung lỗi tại từng control, hiện thông báo tổng hợp
        /// và focus control lỗi đầu tiên.
        /// </summary>
        private bool ValidateRequiredBeforeSave()
        {
            try
            {
                ClearAllRequiredErrors();
                List<string> messages = new List<string>();

                // 1. Lý do khám — bắt buộc ở mức FORM (áp dụng cho mọi tab), kèm giới hạn độ dài.
                string lyDoKham = txtLyDoKham.Text;
                if (string.IsNullOrWhiteSpace(lyDoKham))
                {
                    string msg = "Lý do khám bắt buộc nhập.";
                    SetRequiredError(txtLyDoKham, msg);
                    messages.Add(msg);
                }
                else if (lyDoKham.Length > LY_DO_KHAM_MAX_LENGTH)
                {
                    string msg = "Lý do khám tối đa " + LY_DO_KHAM_MAX_LENGTH + " ký tự.";
                    SetRequiredError(txtLyDoKham, msg);
                    messages.Add(msg);
                }

                // 2. Đối tượng + Nguồn chi trả của tab đang lưu.
                int tabIndex = xtraTabControl1.SelectedTabPageIndex;
                if (tabIndex == 1) // KSK trên 18 tuổi
                {
                    ValidateObjectAndPaySource(cboObject, GetKskObjectValue(), cboPaymentSource, messages);
                    // "Phân loại sức khỏe" (mục kết luận) — HEALTH_EXAM_RANK_ID.
                    if (cboHealthExamRank2 != null
                        && (cboHealthExamRank2.EditValue == null || cboHealthExamRank2.EditValue == DBNull.Value))
                    {
                        string msgRank = "Phân loại sức khỏe bắt buộc chọn.";
                        SetRequiredError(cboHealthExamRank2, msgRank);
                        messages.Add(msgRank);
                    }
                }
                else if (tabIndex == 2) // KSK dưới 18 tuổi
                    ValidateObjectAndPaySource(cboObject3, GetObjectValueExt(cboObject3), cboPaymentSource3, messages);
                else if (tabIndex == 7) // Trẻ em dưới 6 tuổi
                {
                    ValidateObjectAndPaySource(cboObject8, GetObjectValueExt(cboObject8), cboPaymentSource8, messages);
                    ValidateRequiredUnderSix(messages);
                }

                // 3. Mục kết luận: đã nhập thông tin kết luận -> bắt buộc Phân loại + Người kết luận.
                ValidateRequiredConclusion(tabIndex, messages);

                // 4. Huyết áp: đã nhập 1 ô thì phải nhập đủ cả tâm thu và tâm trương.
                ValidateBloodPressure(tabIndex, messages);

                if (messages.Count == 0) return true;

                XtraMessageBox.Show(
                    "Vui lòng kiểm tra lại các trường bắt buộc:\r\n\r\n- " + string.Join("\r\n- ", messages.ToArray()),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (requiredInvalidControls.Count > 0)
                {
                    try { requiredInvalidControls[0].Focus(); }
                    catch (Exception exFocus) { LogSystem.Warn(exFocus); }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return true; // lỗi kiểm tra không được chặn nghiệp vụ Lưu
            }
        }

        /// <summary>
        /// Tab trẻ em dưới 6 tuổi: 3 trường bắt buộc (caption đã tô Maroon trong Designer)
        ///  - "Họ tên người đi cùng trẻ"  (txtAccompanyPersonName8 — mục I. HÀNH CHÍNH)
        ///  - "Mối quan hệ với trẻ"       (rdoAccompanyRelationship8 — mục I. HÀNH CHÍNH)
        ///  - "Kết luận về sức khỏe"      (rdoConclusionHealth8 — mục VII. KẾT LUẬN VÀ TƯ VẤN)
        /// </summary>
        private void ValidateRequiredUnderSix(List<string> messages)
        {
            if (txtAccompanyPersonName8 != null && string.IsNullOrWhiteSpace(txtAccompanyPersonName8.Text))
            {
                string msg = "Họ tên người đi cùng trẻ bắt buộc nhập.";
                SetRequiredError(txtAccompanyPersonName8, msg);
                messages.Add(msg);
            }
            if (!HasRadioValue(rdoAccompanyRelationship8))
            {
                string msg = "Mối quan hệ với trẻ bắt buộc chọn.";
                SetRequiredError(rdoAccompanyRelationship8, msg);
                messages.Add(msg);
            }
            if (!HasRadioValue(rdoConclusionHealth8))
            {
                string msg = "Kết luận về sức khỏe bắt buộc chọn.";
                SetRequiredError(rdoConclusionHealth8, msg);
                messages.Add(msg);
            }
        }

        /// <summary>RadioGroup đã chọn 1 mục hay chưa.</summary>
        private bool HasRadioValue(RadioGroup rdo)
        {
            return rdo != null && rdo.EditValue != null && rdo.EditValue != DBNull.Value;
        }

        /// <summary>
        /// Mục KẾT LUẬN của 3 tab: chỉ cần nhập MỘT trong các thông tin kết luận của tab thì
        /// BẮT BUỘC nhập "Phân loại" + "Người kết luận".
        ///
        /// Thông tin kết luận tính là "đã nhập" theo từng tab:
        ///  - Trên 18 tuổi (1): Mô tả (txtHealthExamRankDescription2), Bệnh tật nếu có (txtDiseases2),
        ///    Chẩn đoán bệnh theo ICD-10 (UC dicIcdConclusionUc[1]).
        ///  - Dưới 18 tuổi (2): Sức khỏe (txtNormalHealth3), Các vấn đề khác (txtProblemHealth3),
        ///    Chẩn đoán bệnh theo ICD-10 (UC dicIcdConclusionUc[2]).
        ///  - Trẻ em dưới 6 tuổi (7): Kết luận về sức khỏe (rdoConclusionHealth8), ICD-10
        ///    (UC dicIcdConclusionUc[7]), Ghi rõ (memConclusionDetail8), Tư vấn và hẹn khám lần sau
        ///    (memAdviceNextExam8), Chuyển cơ sở khám bệnh - chữa bệnh (checkEdit1).
        ///
        /// Các tab còn lại (định kỳ, lái xe, nghề nghiệp...) KHÔNG áp quy tắc này.
        /// </summary>
        private void ValidateRequiredConclusion(int tabIndex, List<string> messages)
        {
            try
            {
                GridLookUpEdit cboRank;
                GridLookUpEdit cboConcluder;
                GetConclusionControls(tabIndex, out cboRank, out cboConcluder);
                if (cboRank == null && cboConcluder == null) return;

                bool hasRank = HasLookUpValue(cboRank);
                bool hasConcluder = HasLookUpValue(cboConcluder);
                if (!IsConclusionPairRequired(tabIndex)) return;

                // Nêu rõ LÝ DO bắt buộc: do ô còn lại của cặp đã nhập, hay do đã nhập nội dung kết luận.
                if (!hasRank)
                    AddRequiredError(cboRank, hasConcluder
                        ? "Phân loại bắt buộc chọn khi đã nhập Người khám."
                        : "Phân loại bắt buộc chọn khi đã nhập thông tin kết luận.", messages);
                if (!hasConcluder)
                    AddRequiredError(cboConcluder, hasRank
                        ? "Người khám bắt buộc chọn khi đã nhập Phân loại."
                        : "Người khám bắt buộc chọn khi đã nhập thông tin kết luận.", messages);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Cặp "Phân loại" + "Người khám" của tab đang bắt buộc đủ cả hai chưa.
        ///
        /// Đúng khi: đã nhập một thông tin kết luận của tab, HOẬC đã nhập một trong hai ô của
        /// chính cặp (đã chọn Phân loại thì phải có Người khám và ngược lại).
        ///
        /// Dùng CHUNG cho việc chặn Lưu và việc tô màu động — màu và cảnh báo không thể lệch nhau.
        /// </summary>
        private bool IsConclusionPairRequired(int tabIndex)
        {
            try
            {
                GridLookUpEdit cboRank;
                GridLookUpEdit cboConcluder;
                GetConclusionControls(tabIndex, out cboRank, out cboConcluder);
                if (cboRank == null && cboConcluder == null) return false;

                return HasConclusionInput(tabIndex)
                    || HasLookUpValue(cboRank)
                    || HasLookUpValue(cboConcluder);
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Tab đã nhập ít nhất MỘT thông tin kết luận chưa. Dùng chung cho cả việc chặn Lưu
        /// và việc tô màu động — MỘT nguồn sự thật, tránh lệch giữa màu và cảnh báo.
        /// </summary>
        private bool HasConclusionInput(int tabIndex)
        {
            try
            {
                if (tabIndex == 1)          // KSK trên 18 tuổi
                    return HasText(txtHealthExamRankDescription2)
                        || HasText(txtDiseases2)
                        || HasIcdConclusion(1);
                if (tabIndex == 2)          // KSK dưới 18 tuổi
                    return HasText(txtNormalHealth3)
                        || HasText(txtProblemHealth3)
                        || HasIcdConclusion(2);
                if (tabIndex == 7)          // Trẻ em dưới 6 tuổi
                    return HasRadioValue(rdoConclusionHealth8)
                        || HasIcdConclusion(7)
                        || HasText(memConclusionDetail8)
                        || HasText(memAdviceNextExam8)
                        || (checkEdit1 != null && checkEdit1.Checked);
                return false;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>Cặp combo "Phân loại" + "Người kết luận" của tab (null nếu tab không áp quy tắc).</summary>
        private void GetConclusionControls(int tabIndex, out GridLookUpEdit cboRank, out GridLookUpEdit cboConcluder)
        {
            cboRank = null;
            cboConcluder = null;
            if (tabIndex == 1)      { cboRank = cboHealthExamRank2; cboConcluder = cboConcluderLoginName2; }
            else if (tabIndex == 2) { cboRank = cboHealthExamRank3; cboConcluder = cboConcluderLoginName3; }
            else if (tabIndex == 7) { cboRank = cboHealthExamRank8; cboConcluder = cboConcluder8; }
        }

        /// <summary>
        /// Huyết áp phải nhập ĐỦ CẢ HAI ô tâm thu (tối đa) và tâm trương (tối thiểu):
        /// chỉ nhập 1 ô -> chặn Lưu và cảnh báo ngay tại ô còn trống. Cả hai trống -> hợp lệ
        /// (không đo huyết áp).
        ///
        /// 4 tab có ô huyết áp (hậu tố control KHÔNG trùng chỉ số tab):
        ///   tab 0 định kỳ        -> spnBloodPressureMax  / spnBloodPressureMin
        ///   tab 1 trên 18 tuổi   -> spnBloodPressureMax2 / spnBloodPressureMin2
        ///   tab 2 dưới 18 tuổi  -> spnBloodPressureMax3 / spnBloodPressureMin3
        ///   tab 6 nghề nghiệp    -> spnBloodPressureMax7 / spnBloodPressureMin7
        /// Các tab còn lại (lái xe, KSK khác, trẻ dưới 6 tuổi) KHÔNG có ô huyết áp.
        /// </summary>
        private void ValidateBloodPressure(int tabIndex, List<string> messages)
        {
            try
            {
                SpinEdit spnMax;
                SpinEdit spnMin;
                if (tabIndex == 0)      { spnMax = spnBloodPressureMax;  spnMin = spnBloodPressureMin; }
                else if (tabIndex == 1) { spnMax = spnBloodPressureMax2; spnMin = spnBloodPressureMin2; }
                else if (tabIndex == 2) { spnMax = spnBloodPressureMax3; spnMin = spnBloodPressureMin3; }
                else if (tabIndex == 6) { spnMax = spnBloodPressureMax7; spnMin = spnBloodPressureMin7; }
                else return;
                if (spnMax == null || spnMin == null) return;

                // "Đã nhập" xét theo EditValue — giống điều kiện lưu BLOOD_PRESSURE_MAX/MIN vào HIS_DHST.
                bool hasMax = spnMax.EditValue != null && spnMax.EditValue != DBNull.Value;
                bool hasMin = spnMin.EditValue != null && spnMin.EditValue != DBNull.Value;
                if (hasMax == hasMin) return;   // cả hai trống hoặc cả hai đã nhập -> hợp lệ

                if (!hasMax)
                    AddRequiredError(spnMax,
                        "Huyết áp tâm thu bắt buộc nhập khi đã nhập huyết áp tâm trương.", messages);
                else
                    AddRequiredError(spnMin,
                        "Huyết áp tâm trương bắt buộc nhập khi đã nhập huyết áp tâm thu.", messages);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đã nhập "Chẩn đoán bệnh theo ICD-10" ở UC kết luận của tab hay chưa.</summary>
        private bool HasIcdConclusion(int tabIndex)
        {
            try
            {
                if (dicIcdConclusionUc == null
                    || !dicIcdConclusionUc.ContainsKey(tabIndex)
                    || dicIcdConclusionUc[tabIndex] == null) return false;
                UcKskConclusionIcd uc = dicIcdConclusionUc[tabIndex];
                return uc.GetConclusionIcdType() != null
                    || !string.IsNullOrWhiteSpace(uc.GetConclusionIcdCode());
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>GridLookUpEdit đã chọn giá trị hay chưa.</summary>
        private bool HasLookUpValue(GridLookUpEdit cbo)
        {
            return cbo != null && cbo.EditValue != null && cbo.EditValue != DBNull.Value;
        }

        /// <summary>
        /// Gắn cảnh báo + thêm message. BỎ QUA nếu control đã bị báo lỗi ở bước kiểm tra trước
        /// (vd "Phân loại" của tab trên 18 tuổi đã bắt buộc vô điều kiện) — tránh message trùng.
        /// </summary>
        private void AddRequiredError(Control ctrl, string message, List<string> messages)
        {
            if (ctrl == null || requiredInvalidControls.Contains(ctrl)) return;
            SetRequiredError(ctrl, message);
            messages.Add(message);
        }

        /// <summary>Cặp combo Đối tượng (chọn nhiều) + Nguồn chi trả (chọn 1) của 1 tab đều bắt buộc.</summary>
        private void ValidateObjectAndPaySource(GridLookUpEdit cboObj, string objectValue,
            GridLookUpEdit cboPay, List<string> messages)
        {
            if (cboObj != null && string.IsNullOrWhiteSpace(objectValue))
            {
                string msg = "Đối tượng bắt buộc nhập.";
                SetRequiredError(cboObj, msg);
                messages.Add(msg);
            }
            if (cboPay != null && (cboPay.EditValue == null || cboPay.EditValue == DBNull.Value))
            {
                string msg = "Nguồn chi trả bắt buộc nhập.";
                SetRequiredError(cboPay, msg);
                messages.Add(msg);
            }
        }

        #endregion
    }
}
