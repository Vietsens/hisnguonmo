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
 *  - Mục KẾT LUẬN của 3 tab (trên 18, dưới 18, trẻ dưới 6 tuổi): "Phân loại" và "Người khám"
 *    là MỘT CẶP — bắt buộc đủ cả hai khi:
 *      + Tab trên 18 / dưới 18 tuổi (mục kết luận nằm trong sub-tab "Kết luận"):
 *        ĐANG MỞ sub-tab "Kết luận", HOẶC đã nhập MỘT thông tin kết luận của tab
 *        TRONG PHIÊN NÀY (HasConclusionInput — so với snapshot chụp lúc nạp tab, nội dung
 *        nạp sẵn từ bản ghi cũ KHÔNG tính). Các sub-tab còn lại (Khám thể lực / Khám lâm sàng /
 *        Khám cận lâm sàng) KHÔNG bắt nhập — "Phân loại" thuộc mục kết luận, không liên quan.
 *      + Tab trẻ em dưới 6 tuổi (không có sub-tab): đã nhập MỘT thông tin kết luận, HOẶC
 *        đã nhập MỘT trong hai ô của chính cặp đó (nhập Phân loại thì phải có Người khám
 *        và ngược lại).
 *    -> xem IsConclusionPairRequired / IsConclusionSubTabSelected / ValidateRequiredConclusion.
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

        /// <summary>Ký tự ngăn cách giữa các ô trong chữ ký nội dung kết luận (không xuất hiện trong dữ liệu nhập).</summary>
        private const char CONCLUSION_SIG_SEPARATOR_CHAR = '';
        private const string CONCLUSION_SIG_SEPARATOR = "";

        /// <summary>
        /// Chữ ký nội dung mục KẾT LUẬN của từng tab (1 / 2 / 7) CHỤP NGAY SAU KHI NẠP DỮ LIỆU.
        /// Dùng để phân biệt "kết luận có sẵn trong bản ghi cũ" với "người dùng vừa nhập trong phiên"
        /// — xem HasConclusionInput.
        /// </summary>
        private readonly Dictionary<int, string> conclusionInputSnapshot = new Dictionary<int, string>();

        /// <summary>
        /// Chữ ký của CHÍNH CẶP "Phân loại" + "Người khám" từng tab, chụp cùng lúc với
        /// <see cref="conclusionInputSnapshot"/>. Dùng cho tab trẻ dưới 6 tuổi (không có sub-tab
        /// "Kết luận" để làm mốc): chỉ khi người dùng TỰ chọn một trong hai ô trong phiên thì mới
        /// bắt nhập nốt ô còn lại — giá trị nạp sẵn từ bản ghi cũ KHÔNG tính.
        /// </summary>
        private readonly Dictionary<int, string> conclusionPairSnapshot = new Dictionary<int, string>();

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
                // Mục kết luận — Phân loại + Người kết luận (tab lái xe chỉ có Người kết luận).
                WireRequiredClearEvent(cboHealthExamRank2);
                WireRequiredClearEvent(cboConcluderLoginName2);
                WireRequiredClearEvent(cboHealthExamRank3);
                WireRequiredClearEvent(cboConcluderLoginName3);
                WireRequiredClearEvent(cboConcluderLoginName4);
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
                // Đổi SUB-TAB của tab trên 18 / dưới 18 tuổi -> tính lại điều kiện bắt buộc của
                // mục kết luận (chỉ sub-tab "Kết luận" mới bắt nhập Phân loại + Người khám).
                if (this.xtraTabControl2 != null)
                    this.xtraTabControl2.SelectedPageChanged
                        += new DevExpress.XtraTab.TabPageChangedEventHandler(RequiredValidation_TabChanged);
                if (this.xtraTabControl3 != null)
                    this.xtraTabControl3.SelectedPageChanged
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
        /// "Phân loại" tab trên 18 tuổi (layoutControlItem185) TRƯỚC ĐÂY bắt buộc VÔ ĐIỀU KIỆN
        /// (Maroon cứng trong Designer); nay theo cùng điều kiện với cặp kết luận nên được tô
        /// ĐỘNG ở đây — Designer vẫn giữ ForeColor Maroon, chỉ bật/tắt UseForeColor.
        /// </summary>
        private void UpdateRequiredHighlight()
        {
            try
            {
                bool conc1 = IsConclusionPairRequired(1);
                bool conc2 = IsConclusionPairRequired(2);
                bool conc7 = IsConclusionPairRequired(7);

                SetCaptionRequired(layoutControlItem185, conc1);   // "Phân loại" tab trên 18 tuổi
                SetCaptionRequired(lciKskConcluder1, conc1);
                SetCaptionRequired(lciHealthExamRank3, conc2);
                SetCaptionRequired(lciKskConcluder2, conc2);
                SetCaptionRequired(lciHealthRank8, conc7);
                SetCaptionRequired(lciConcluder8, conc7);
                // Nhóm "Kết luận về sức khỏe" (tab trẻ <6): trước đây Maroon cứng trong Designer vì
                // bắt buộc vô điều kiện; nay theo cùng điều kiện với mục kết luận -> tô động.
                SetGroupCaptionRequired(lcgKetLuanSub8, conc7);

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

        /// <summary>
        /// Bật/tắt màu Maroon ở caption của 1 LayoutControlGroup (nhóm mục), tương tự SetCaptionRequired.
        /// </summary>
        private void SetGroupCaptionRequired(DevExpress.XtraLayout.LayoutControlGroup group, bool required)
        {
            try
            {
                if (group == null) return;
                if (required) group.AppearanceGroup.ForeColor = System.Drawing.Color.Maroon;
                group.AppearanceGroup.Options.UseForeColor = required;
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
                // "Phân loại" (HEALTH_EXAM_RANK_ID) của tab trên 18 tuổi KHÔNG còn bắt buộc VÔ
                // ĐIỀU KIỆN: ô này thuộc mục kết luận nên chỉ bắt nhập theo ValidateRequiredConclusion
                // (đang mở sub-tab "Kết luận" hoặc đã nhập nội dung kết luận).
                if (tabIndex == 1) // KSK trên 18 tuổi
                    ValidateObjectAndPaySource(cboObject, GetKskObjectValue(), cboPaymentSource, messages);
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
        /// Tab trẻ em dưới 6 tuổi — các trường bắt buộc:
        ///  - "Họ tên người đi cùng trẻ"  (txtAccompanyPersonName8 — mục I. HÀNH CHÍNH): LUÔN bắt buộc.
        ///  - "Mối quan hệ với trẻ"       (rdoAccompanyRelationship8 — mục I. HÀNH CHÍNH): LUÔN bắt buộc.
        ///  - "Kết luận về sức khỏe"      (rdoConclusionHealth8 — mục VII. KẾT LUẬN VÀ TƯ VẤN):
        ///    CHỈ bắt buộc khi mục kết luận đang bắt buộc (IsConclusionPairRequired(7) — tức người dùng
        ///    đã nhập/sửa thông tin kết luận trong phiên này). Tab này KHÔNG có sub-tab để lấy làm mốc
        ///    như tab ≥18 / dưới 18, nên trước đây bắt VÔ ĐIỀU KIỆN: người dùng chỉ nhập phần khám thể
        ///    lực rồi Lưu cũng bị chặn bởi ô kết luận — không liên quan việc họ đang làm.
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
            bool conclusionRequired = IsConclusionPairRequired(7);
            if (!conclusionRequired)
            {
                LogKskConclusion("ValidateRequiredUnderSix: chua nhap/sua gi o muc ket luan trong phien"
                    + " -> KHONG bat buoc \"Ket luan ve suc khoe\"");
            }
            else if (!HasRadioValue(rdoConclusionHealth8))
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
                if (!IsConclusionPairRequired(tabIndex))
                {
                    LogKskConclusion("ValidateRequiredConclusion(tab=" + tabIndex
                        + "): KHONG bat buoc -> bo qua kiem tra Phan loai / Nguoi kham");
                    return;
                }
                LogKskConclusion("ValidateRequiredConclusion(tab=" + tabIndex + "): dang bat buoc"
                    + " -> Phan loai da chon=" + hasRank + ", Nguoi kham da chon=" + hasConcluder
                    + (hasRank && hasConcluder ? " -> hop le" : " -> CHAN LUU"));

                // Nêu rõ LÝ DO bắt buộc: do ô còn lại của cặp đã nhập, do đã nhập nội dung kết luận,
                // hay do đang mở sub-tab "Kết luận" (tab trên 18 / dưới 18 tuổi).
                string reason = HasConclusionInput(tabIndex)
                    ? " khi đã nhập thông tin kết luận."
                    : " ở mục kết luận.";
                if (!hasRank)
                    AddRequiredError(cboRank, hasConcluder
                        ? "Phân loại bắt buộc chọn khi đã nhập Người khám."
                        : "Phân loại bắt buộc chọn" + reason, messages);
                if (!hasConcluder)
                    AddRequiredError(cboConcluder, hasRank
                        ? "Người khám bắt buộc chọn khi đã nhập Phân loại."
                        : "Người khám bắt buộc chọn" + reason, messages);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Cặp "Phân loại" + "Người khám" của tab đang bắt buộc đủ cả hai chưa.
        ///
        /// Tab trên 18 / dưới 18 tuổi (mục kết luận nằm trong sub-tab "Kết luận") — đúng khi:
        /// ĐANG MỞ sub-tab "Kết luận", HOẶC đã nhập một thông tin kết luận của tab.
        /// KHÔNG xét việc cặp đã có sẵn giá trị: "Phân loại" / "Người khám" được NẠP TỪ BẢN GHI CŨ
        /// lúc Load (cboHealthExamRank2/3 = HEALTH_EXAM_RANK_ID, cboConcluderLoginName2/3 =
        /// HIS_KSK_GENERAL.CONCLUDER_LOGINNAME) nên nếu xét sẽ bắt nhập nhầm khi người dùng đang
        /// làm việc ở sub-tab Khám thể lực / Khám lâm sàng / Khám cận lâm sàng.
        ///
        /// Tab trẻ em dưới 6 tuổi (mục kết luận nằm ngay trên tab, không có sub-tab) — GIỮ NGUYÊN:
        /// đã nhập một thông tin kết luận, HOẶC đã nhập một trong hai ô của chính cặp
        /// (đã chọn Phân loại thì phải có Người khám và ngược lại).
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

                if (tabIndex == 1 || tabIndex == 2)
                {
                    bool subTab = IsConclusionSubTabSelected(tabIndex);
                    bool input = HasConclusionInput(tabIndex);
                    bool required = subTab || input;
                    LogKskConclusion("IsConclusionPairRequired(tab=" + tabIndex + "): dang mo sub-tab Ket luan="
                        + subTab + ", da nhap ket luan trong phien=" + input
                        + " -> " + (required ? "BAT BUOC" : "khong bat buoc") + " Phan loai + Nguoi kham"
                        + " [Phan loai da chon=" + HasLookUpValue(cboRank)
                        + ", Nguoi kham da chon=" + HasLookUpValue(cboConcluder) + "]");
                    return required;
                }

                bool inputUnderSix = HasConclusionInput(tabIndex);
                bool pairChanged = HasConclusionPairChanged(tabIndex);
                bool requiredUnderSix = inputUnderSix || pairChanged;
                LogKskConclusion("IsConclusionPairRequired(tab=" + tabIndex + ", tre duoi 6 tuoi): da nhap ket luan trong phien="
                    + inputUnderSix + ", tu chon Phan loai/Nguoi kham trong phien=" + pairChanged
                    + " -> " + (requiredUnderSix ? "BAT BUOC" : "khong bat buoc") + " Phan loai + Nguoi kham"
                    + " [Phan loai da chon=" + HasLookUpValue(cboRank)
                    + ", Nguoi kham da chon=" + HasLookUpValue(cboConcluder) + "]");
                return requiredUnderSix;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Đang mở sub-tab "Kết luận" của tab trên 18 / dưới 18 tuổi hay không.
        /// Mục kết luận của 2 tab này nằm trong một XtraTabControl lồng bên trong:
        ///   tab 1 (trên 18 tuổi)  -> xtraTabControl2, sub-tab "Kết luận" = xtraTabPage12
        ///   tab 2 (dưới 18 tuổi) -> xtraTabControl3, sub-tab "Kết luận" = xtraTabPage16
        /// Các tab khác (kể cả trẻ em dưới 6 tuổi) không có sub-tab -> luôn false.
        /// </summary>
        private bool IsConclusionSubTabSelected(int tabIndex)
        {
            try
            {
                if (tabIndex == 1)
                    return xtraTabControl2 != null && xtraTabControl2.SelectedTabPage == xtraTabPage12;
                if (tabIndex == 2)
                    return xtraTabControl3 != null && xtraTabControl3.SelectedTabPage == xtraTabPage16;
                return false;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Tab đã nhập thông tin kết luận TRONG PHIÊN NÀY chưa.
        ///
        /// KHÔNG được xuất phát từ "ô kết luận đang có chữ": Mô tả / Bệnh tật / ICD kết luận đều được
        /// NẠP SẴN từ bản ghi cũ lúc Load (FillDataPage* đổ DISEASES / HEALTH_EXAM_RANK_DESCRIPTION,
        /// UcKskConclusionIcd.LoadFromGeneral đổ ICD) — mà HIS_KSK_GENERAL dùng CHUNG cho cả lượt khám,
        /// nên chỉ cần ai đó đã ghi kết luận trước đó là mọi lần Lưu sau đều bị bắt nhập "Phân loại",
        /// dù người dùng đang ở sub-tab Khám thể lực / Lâm sàng / Cận lâm sàng và không đụng gì tới kết luận.
        ///
        /// Vì vậy: so nội dung HIỆN TẠI với SNAPSHOT chụp ngay sau khi nạp tab
        /// (CaptureConclusionInputSnapshot gọi cuối EnsureTabLoaded). GIỐNG snapshot = dữ liệu cũ,
        /// người dùng chưa nhập gì -> KHÔNG bắt buộc. KHÁC snapshot = đã nhập/sửa kết luận -> bắt buộc đủ cặp.
        ///
        /// Dùng chung cho cả việc chặn Lưu và việc tô màu động — MỘT nguồn sự thật.
        /// </summary>
        private bool HasConclusionInput(int tabIndex)
        {
            try
            {
                if (!HasConclusionContent(tabIndex)) return false;

                string snapshot;
                if (conclusionInputSnapshot.TryGetValue(tabIndex, out snapshot))
                {
                    string now = GetConclusionInputSignature(tabIndex);
                    if (string.Equals(now, snapshot, StringComparison.Ordinal))
                    {
                        LogKskConclusion("HasConclusionInput(tab=" + tabIndex
                            + "): noi dung ket luan GIU NGUYEN nhu luc nap (" + DescribeSignature(now)
                            + ") -> coi nhu CHUA nhap trong phien -> KHONG bat buoc");
                        return false;
                    }
                    LogKskConclusion("HasConclusionInput(tab=" + tabIndex
                        + "): noi dung ket luan DA DOI so voi luc nap (luc nap: " + DescribeSignature(snapshot)
                        + " | hien tai: " + DescribeSignature(now) + ") -> BAT BUOC Phan loai + Nguoi kham");
                    return true;
                }

                // Chưa chụp snapshot (tab chưa nạp xong) -> giữ hành vi cũ.
                LogKskConclusion("HasConclusionInput(tab=" + tabIndex
                    + "): CHUA co snapshot (tab chua nap xong) -> giu hanh vi cu: co noi dung = coi nhu da nhap");
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Ô kết luận của tab ĐANG CÓ nội dung hay không (không phân biệt dữ liệu cũ hay người dùng vừa nhập).
        ///
        ///  - Trên 18 tuổi (1): Mô tả, Bệnh tật nếu có, ICD-10 kết luận.
        ///  - Dưới 18 tuổi (2): Sức khỏe, Các vấn đề khác, ICD-10 kết luận.
        ///  - Trẻ dưới 6 tuổi (7): Kết luận về sức khỏe, ICD-10, Ghi rõ, Tư vấn và hẹn khám lần sau, Chuyển cơ sở.
        /// </summary>
        private bool HasConclusionContent(int tabIndex)
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

        /// <summary>
        /// Chụp "chữ ký" nội dung kết luận của tab ngay sau khi nạp xong dữ liệu (cuối EnsureTabLoaded).
        /// Đây là MỐC so sánh để biết người dùng có thực sự nhập/sửa kết luận trong phiên hay không.
        /// Chỉ 3 tab có mục kết luận (1 / 2 / 7) mới cần.
        /// </summary>
        private void CaptureConclusionInputSnapshot(int tabIndex)
        {
            try
            {
                if (tabIndex != 1 && tabIndex != 2 && tabIndex != 7) return;
                string sig = GetConclusionInputSignature(tabIndex);
                conclusionInputSnapshot[tabIndex] = sig;
                string pairSig = GetConclusionPairSignature(tabIndex);
                conclusionPairSnapshot[tabIndex] = pairSig;
                LogKskConclusion("CaptureConclusionInputSnapshot(tab=" + tabIndex + "): noi dung "
                    + DescribeSignature(sig) + " | cap Phan loai-Nguoi kham luc nap=\"" + pairSig + "\"");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Chuỗi đại diện nội dung kết luận của tab — CHỈ dùng để SO SÁNH, không ghi thẳng ra log
        /// (xem DescribeSignature).
        /// </summary>
        private string GetConclusionInputSignature(int tabIndex)
        {
            try
            {
                string icd = GetIcdConclusionSignature(tabIndex);
                if (tabIndex == 1)
                    return GetEditText(txtHealthExamRankDescription2) + CONCLUSION_SIG_SEPARATOR
                         + GetEditText(txtDiseases2) + CONCLUSION_SIG_SEPARATOR + icd;
                if (tabIndex == 2)
                    return GetEditText(txtNormalHealth3) + CONCLUSION_SIG_SEPARATOR
                         + GetEditText(txtProblemHealth3) + CONCLUSION_SIG_SEPARATOR + icd;
                if (tabIndex == 7)
                    return GetRadioText(rdoConclusionHealth8) + CONCLUSION_SIG_SEPARATOR
                         + GetEditText(memConclusionDetail8) + CONCLUSION_SIG_SEPARATOR
                         + GetEditText(memAdviceNextExam8) + CONCLUSION_SIG_SEPARATOR
                         + ((checkEdit1 != null && checkEdit1.Checked) ? "1" : "0")
                         + CONCLUSION_SIG_SEPARATOR + icd;
                return "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); return ""; }
        }

        /// <summary>
        /// Người dùng có TỰ chọn "Phân loại" / "Người khám" của mục kết luận trong phiên này không
        /// (so với lúc nạp tab). Dùng cho tab trẻ dưới 6 tuổi — thay cho việc xét thẳng
        /// HasLookUpValue, vốn luôn true với hồ sơ cũ đã có sẵn một trong hai ô.
        /// </summary>
        private bool HasConclusionPairChanged(int tabIndex)
        {
            try
            {
                string snapshot;
                if (!conclusionPairSnapshot.TryGetValue(tabIndex, out snapshot)) return false;
                string now = GetConclusionPairSignature(tabIndex);
                if (string.Equals(now, snapshot, StringComparison.Ordinal)) return false;
                LogKskConclusion("HasConclusionPairChanged(tab=" + tabIndex + "): cap Phan loai-Nguoi kham DA DOI"
                    + " (luc nap=\"" + snapshot + "\", hien tai=\"" + now + "\")");
                return true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return false; }
        }

        /// <summary>
        /// Chữ ký của cặp "Phân loại" + "Người khám" mục kết luận. Đây là ID phân loại + loginname
        /// (KHÔNG phải dữ liệu bệnh nhân) nên ghi log được nguyên văn.
        /// </summary>
        private string GetConclusionPairSignature(int tabIndex)
        {
            try
            {
                GridLookUpEdit cboRank;
                GridLookUpEdit cboConcluder;
                GetConclusionControls(tabIndex, out cboRank, out cboConcluder);
                string rank = (cboRank == null || cboRank.EditValue == null || cboRank.EditValue == DBNull.Value)
                    ? "" : cboRank.EditValue.ToString();
                string concluder = (cboConcluder == null || cboConcluder.EditValue == null || cboConcluder.EditValue == DBNull.Value)
                    ? "" : cboConcluder.EditValue.ToString();
                return rank + "/" + concluder;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return ""; }
        }

        /// <summary>Chữ ký của ICD-10 kết luận (loại chẩn đoán + mã) trong UC của tab.</summary>
        private string GetIcdConclusionSignature(int tabIndex)
        {
            try
            {
                if (dicIcdConclusionUc == null
                    || !dicIcdConclusionUc.ContainsKey(tabIndex)
                    || dicIcdConclusionUc[tabIndex] == null) return "";
                UcKskConclusionIcd uc = dicIcdConclusionUc[tabIndex];
                long? type = uc.GetConclusionIcdType();
                return (type.HasValue ? type.Value.ToString() : "") + "#" + (uc.GetConclusionIcdCode() ?? "");
            }
            catch (Exception ex) { LogSystem.Warn(ex); return ""; }
        }

        private static string GetEditText(BaseEdit edit)
        {
            return (edit == null || edit.Text == null) ? "" : edit.Text.Trim();
        }

        private static string GetRadioText(RadioGroup rdo)
        {
            if (rdo == null || rdo.EditValue == null || rdo.EditValue == DBNull.Value) return "";
            return rdo.EditValue.ToString();
        }

        /// <summary>
        /// Mô tả chữ ký để GHI LOG: CHỈ độ dài từng ô + mã băm — KHÔNG ghi nội dung chẩn đoán /
        /// bệnh tật của bệnh nhân ra log (quy định bảo mật dữ liệu bệnh nhân).
        /// </summary>
        private static string DescribeSignature(string sig)
        {
            try
            {
                if (sig == null) return "null";
                string[] parts = sig.Split(CONCLUSION_SIG_SEPARATOR_CHAR);
                string lens = "";
                for (int i = 0; i < parts.Length; i++)
                    lens += (i > 0 ? "/" : "") + parts[i].Length;
                return "do dai o[" + lens + "] hash=" + sig.GetHashCode();
            }
            catch { return "?"; }
        }

        /// <summary>Log riêng cho luật bắt buộc mục kết luận — tìm trong log bằng từ khóa "KskConclusion".</summary>
        private static void LogKskConclusion(string message)
        {
            try { LogSystem.Debug("KskConclusion: " + message); }
            catch { }
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
