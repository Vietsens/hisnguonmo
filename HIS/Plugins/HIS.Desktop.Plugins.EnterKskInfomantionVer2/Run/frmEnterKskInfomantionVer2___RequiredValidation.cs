/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Kiểm tra trường bắt buộc — hiển thị icon cảnh báo (tam giác) kèm nội dung lỗi ngay tại control
 * theo ui_rules (caption Maroon đặt ở LayoutControlItem trong Designer).
 *
 * KHI LƯU (ValidateRequiredBeforeSave):
 *  - "Đối tượng" + "Nguồn chi trả" của 3 tab: trên 18 tuổi (cboObject/cboPaymentSource),
 *    dưới 18 tuổi (cboObject3/cboPaymentSource3), trẻ em dưới 6 tuổi (cboObject8/cboPaymentSource8).
 *    Chỉ kiểm tra cặp combo của TAB ĐANG LƯU (mỗi lần Lưu chỉ gửi dữ liệu 1 tab).
 *  - "Lý do khám" (txtLyDoKham): bắt buộc nhập ở MỨC FORM (mọi tab) + giới hạn độ dài.
 *  - Tab trẻ em dưới 6 tuổi: "Họ tên người đi cùng trẻ" + "Mối quan hệ với trẻ".
 *  - Huyết áp (các tab có sinh hiệu): đã nhập 1 ô thì phải nhập đủ CẢ tâm thu + tâm trương
 *    -> xem ValidateBloodPressure.
 *  - Độ dài theo QĐ 2062 (chi nhánh có liên thông cổng 2062) -> xem ___Qd2062Length.
 *
 * KHI KẾT THÚC Y LỆNH KHÁM (ValidateConclusionBeforeFinish — nút "Kết thúc y lệnh khám" và
 * "Tự động kết thúc" sau Lưu):
 *  - Mục KẾT LUẬN của 3 tab (trên 18, dưới 18, trẻ dưới 6 tuổi): "Phân loại" + "Người khám"
 *    (tab trẻ <6: "Xếp loại tình trạng sức khỏe chung" + "Bác sĩ khám" + "Kết luận về sức khỏe").
 *    BV Nguyễn Đình Chiểu (25/09/2026): nhập khám, cận lâm sàng xong mới nhập kết luận — trước đây
 *    mục kết luận chặn Lưu nên phải nhập kết luận TRƯỚC mới lưu được phần khám (bị ngược). Nay Lưu
 *    không bắt mục kết luận nữa, chỉ bắt khi kết thúc khám.
 *
 * TÔ MÀU: caption Maroon = trường bắt buộc. Mục kết luận luôn Maroon (bắt buộc để kết thúc khám,
 * tooltip ghi rõ); huyết áp tô ĐỘNG — ô còn trống đỏ khi ô còn lại đã nhập (UpdateRequiredHighlight).
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

        /// <summary>Tooltip các ô mục kết luận — giải thích vì sao Maroon mà Lưu vẫn không bị chặn.</summary>
        private const string CONCLUSION_REQUIRED_AT_FINISH_TOOLTIP =
            "Bắt buộc nhập trước khi kết thúc y lệnh khám (Lưu phần khám / cận lâm sàng không bắt buộc).";

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
                // Mục kết luận — Phân loại + Người kết luận (bắt buộc khi kết thúc khám).
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
                // Tab trẻ em dưới 6 tuổi — trường bắt buộc bổ sung.
                WireRequiredClearEvent(txtAccompanyPersonName8);
                WireRequiredClearEvent(rdoAccompanyRelationship8);
                WireRequiredClearEvent(rdoConclusionHealth8);

                // Đổi tab -> bỏ cảnh báo của tab cũ (cảnh báo chỉ có nghĩa với tab đang lưu).
                this.xtraTabControl1.SelectedPageChanged
                    += new DevExpress.XtraTab.TabPageChangedEventHandler(RequiredValidation_TabChanged);

                InitConclusionRequiredAtFinishCaption();
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
                // Ô còn vượt độ dài QĐ 2062 thì giữ cảnh báo (ô vừa bắt buộc vừa giới hạn độ dài).
                if (hasValue && !IsQd2062TooLong(ctrl)) ClearRequiredError(ctrl);
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
            qd2062Recheck.Remove(ctrl);
        }

        private void ClearAllRequiredErrors()
        {
            if (dxErrorProviderRequired != null) dxErrorProviderRequired.ClearErrors();
            requiredInvalidControls.Clear();
            qd2062Recheck.Clear();
        }

        /// <summary>
        /// Focus control lỗi đầu tiên. Control nằm trong sub-tab CHƯA mở (tab trên 18 / dưới 18 tuổi chia
        /// Khám thể lực / Lâm sàng / Cận lâm sàng / Kết luận) thì Focus() thất bại âm thầm và icon nằm ở
        /// trang ẩn -> mở các sub-tab chứa nó trước. KHÔNG đổi tab ngoài cùng (xtraTabControl1): đó là tab
        /// đang lưu, và đổi tab đó sẽ xóa hết cảnh báo (RequiredValidation_TabChanged).
        /// </summary>
        private void FocusFirstInvalidControl()
        {
            if (requiredInvalidControls.Count == 0) return;
            try
            {
                Control ctrl = requiredInvalidControls[0];
                for (Control p = ctrl.Parent; p != null; p = p.Parent)
                {
                    DevExpress.XtraTab.XtraTabPage page = p as DevExpress.XtraTab.XtraTabPage;
                    if (page == null || page.TabControl == null || page.TabControl == xtraTabControl1) continue;
                    if (page.TabControl.SelectedTabPage != page) page.TabControl.SelectedTabPage = page;
                }
                ctrl.Focus();
            }
            catch (Exception exFocus) { LogSystem.Warn(exFocus); }
        }

        #endregion

        #region Tô màu trường bắt buộc

        /// <summary>
        /// Caption mục kết luận của 3 tab (trên 18 / dưới 18 / trẻ dưới 6 tuổi) luôn Maroon vì bắt buộc
        /// để kết thúc khám; tooltip nói rõ là KHÔNG bắt buộc khi Lưu. Gọi 1 lần lúc Load.
        /// (Trước đây tô động theo điều kiện "đã nhập kết luận trong phiên" — điều kiện đó đã bỏ.)
        /// </summary>
        private void InitConclusionRequiredAtFinishCaption()
        {
            try
            {
                DevExpress.XtraLayout.LayoutControlItem[] items = new DevExpress.XtraLayout.LayoutControlItem[]
                {
                    layoutControlItem185, lciKskConcluder1,   // tab trên 18 tuổi
                    lciHealthExamRank3, lciKskConcluder2,     // tab dưới 18 tuổi
                    lciHealthRank8, lciConcluder8             // tab trẻ em dưới 6 tuổi
                };
                foreach (DevExpress.XtraLayout.LayoutControlItem item in items)
                {
                    if (item == null) continue;
                    SetCaptionRequired(item, true);
                    item.OptionsToolTip.ToolTip = CONCLUSION_REQUIRED_AT_FINISH_TOOLTIP;
                }
                SetGroupCaptionRequired(lcgKetLuanSub8, true);
                if (lcgKetLuanSub8 != null) lcgKetLuanSub8.OptionsToolTip.ToolTip = CONCLUSION_REQUIRED_AT_FINISH_TOOLTIP;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tính lại màu caption của trường bắt buộc CÓ ĐIỀU KIỆN — Huyết áp: ô còn trống đỏ khi ô còn
        /// lại đã nhập. (Mục kết luận tô cố định ở InitConclusionRequiredAtFinishCaption.)
        /// </summary>
        private void UpdateRequiredHighlight()
        {
            try
            {
                // Huyết áp 4 tab (hậu tố control không trùng chỉ số tab — xem ValidateBloodPressure).
                UpdateBloodPressureHighlight(layoutControlItem45, spnBloodPressureMax, spnBloodPressureMin);
                UpdateBloodPressureHighlight(layoutControlItem117, spnBloodPressureMax2, spnBloodPressureMin2);
                UpdateBloodPressureHighlight(layoutControlItem206, spnBloodPressureMax3, spnBloodPressureMin3);
                UpdateBloodPressureHighlight(layoutControlItem498, spnBloodPressureMax7, spnBloodPressureMin7);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Caption "Huyết áp:" đỏ khi CHỈ nhập MỘT trong hai ô (nhập 1 phải đủ 2).
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

        #endregion

        #region Kiểm tra khi Lưu

        /// <summary>
        /// Kiểm tra các trường bắt buộc trước khi Lưu. Trả về true nếu hợp lệ.
        /// Không hợp lệ: gắn icon cảnh báo + nội dung lỗi tại từng control, hiện thông báo tổng hợp
        /// và focus control lỗi đầu tiên.
        /// Mục kết luận (Phân loại / Người khám / Kết luận về sức khỏe) KHÔNG kiểm tra ở đây — chỉ bắt
        /// buộc khi kết thúc y lệnh khám (ValidateConclusionBeforeFinish).
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
                    ValidateObjectAndPaySource(cboObject, GetKskObjectValue(), cboPaymentSource, messages);
                else if (tabIndex == 2) // KSK dưới 18 tuổi
                    ValidateObjectAndPaySource(cboObject3, GetObjectValueExt(cboObject3), cboPaymentSource3, messages);
                else if (tabIndex == 7) // Trẻ em dưới 6 tuổi
                {
                    ValidateObjectAndPaySource(cboObject8, GetObjectValueExt(cboObject8), cboPaymentSource8, messages);
                    ValidateRequiredUnderSix(messages);
                }

                // 3. Huyết áp: đã nhập 1 ô thì phải nhập đủ cả tâm thu và tâm trương.
                ValidateBloodPressure(tabIndex, messages);

                if (messages.Count == 0) return true;

                XtraMessageBox.Show(
                    "Vui lòng kiểm tra lại các trường bắt buộc:\r\n\r\n- " + string.Join("\r\n- ", messages.ToArray()),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstInvalidControl();
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return true; // lỗi kiểm tra không được chặn nghiệp vụ Lưu
            }
        }

        /// <summary>
        /// Tab trẻ em dưới 6 tuổi — trường bắt buộc khi Lưu (mục I. HÀNH CHÍNH):
        ///  - "Họ tên người đi cùng trẻ"  (txtAccompanyPersonName8).
        ///  - "Mối quan hệ với trẻ"       (rdoAccompanyRelationship8).
        /// "Kết luận về sức khỏe" (rdoConclusionHealth8) thuộc mục kết luận -> chỉ bắt buộc khi kết thúc
        /// y lệnh khám (ValidateConclusionBeforeFinish).
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
        }

        /// <summary>RadioGroup đã chọn 1 mục hay chưa.</summary>
        private bool HasRadioValue(RadioGroup rdo)
        {
            return rdo != null && rdo.EditValue != null && rdo.EditValue != DBNull.Value;
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

        #endregion

        #region Kiểm tra khi Kết thúc y lệnh khám

        /// <summary>
        /// Mục KẾT LUẬN phải đủ trước khi kết thúc y lệnh khám — áp cho hồ sơ KSK trên 18 / dưới 18 /
        /// trẻ dưới 6 tuổi (các loại khác không có quy tắc này):
        ///  - Trên 18 / dưới 18 tuổi: "Phân loại" + "Người khám".
        ///  - Trẻ em dưới 6 tuổi: "Xếp loại tình trạng sức khỏe chung" + "Bác sĩ khám" + "Kết luận về sức khỏe".
        ///
        /// Loại hồ sơ xét theo BẢN GHI (ResolveConclusionRuleTab), không theo tab đang mở: nút Kết thúc nằm
        /// ngoài tab nên đứng ở tab "Ksk định kỳ" bấm Kết thúc cũng phải kiểm hồ sơ trên 18 tuổi.
        ///
        /// Mỗi ô phải có trong bản ghi ĐÃ LƯU: nút Kết thúc không lưu dữ liệu. Đang đứng đúng tab của hồ sơ thì
        /// xét thêm màn hình: chọn Phân loại rồi bấm Kết thúc ngay (chưa Lưu) -> báo "đã chọn nhưng chưa Lưu".
        ///
        /// "Tự động kết thúc" sau Lưu (isAutoAfterSave): mục kết luận chưa lưu ô nào = người dùng còn đang
        /// ở bước khám / cận lâm sàng -> lặng lẽ KHÔNG kết thúc (không bật hộp thoại mỗi lần Lưu);
        /// đã lưu một phần mục kết luận mà thiếu ô khác -> cảnh báo như khi bấm nút.
        /// </summary>
        /// <returns>true = cho phép kết thúc.</returns>
        private bool ValidateConclusionBeforeFinish(bool isAutoAfterSave)
        {
            try
            {
                int selectedTab = xtraTabControl1.SelectedTabPageIndex;
                int ruleTab = ResolveConclusionRuleTab(selectedTab);
                if (ruleTab < 0)
                {
                    LogKskConclusion("ValidateConclusionBeforeFinish(tab=" + selectedTab + "): ho so khong phai KSK tren 18"
                        + " / duoi 18 / tre duoi 6 tuoi -> khong ap quy tac muc ket luan");
                    return true;
                }
                bool onRuleTab = (ruleTab == selectedTab);
                bool underSix = (ruleTab == TAB_UNDER_SIX);
                GridLookUpEdit cboRank;
                GridLookUpEdit cboConcluder;
                GetConclusionControls(ruleTab, out cboRank, out cboConcluder);

                bool savedRank, savedConcluder, savedHealth;
                GetSavedConclusionState(ruleTab, out savedRank, out savedConcluder, out savedHealth);
                if (!underSix) savedHealth = true;
                // Màn hình chỉ có nghĩa khi đang đứng đúng tab của hồ sơ (tab khác: ô có thể chưa nạp / còn cũ).
                bool screenRank = !onRuleTab || cboRank == null || HasLookUpValue(cboRank);
                bool screenConcluder = !onRuleTab || cboConcluder == null || HasLookUpValue(cboConcluder);
                bool screenHealth = !onRuleTab || !underSix || HasRadioValue(rdoConclusionHealth8);

                bool okRank = savedRank && screenRank;
                bool okConcluder = savedConcluder && screenConcluder;
                bool okHealth = savedHealth && screenHealth;
                bool ok = okRank && okConcluder && okHealth;
                bool anySaved = savedRank || savedConcluder || (underSix && savedHealth);
                LogKskConclusion("ValidateConclusionBeforeFinish(tab dang mo=" + selectedTab + ", tab ho so=" + ruleTab
                    + ", auto=" + isAutoAfterSave + "): Phan loai man hinh/da luu=" + screenRank + "/" + savedRank
                    + ", Nguoi kham=" + screenConcluder + "/" + savedConcluder
                    + (underSix ? ", Ket luan ve suc khoe=" + screenHealth + "/" + savedHealth : "")
                    + (ok ? " -> cho ket thuc" : " -> CHAN KET THUC"));
                if (ok) return true;

                if (isAutoAfterSave && !anySaved)
                {
                    LogKskConclusion("ValidateConclusionBeforeFinish: tu dong ket thuc - muc ket luan chua luu o nao"
                        + " (dang o buoc kham/CLS) -> bo qua ket thuc, khong hien thong bao");
                    return false;
                }

                ClearAllRequiredErrors();
                // Đúng tab hồ sơ: mở sub-tab "Kết luận" TRƯỚC khi gắn icon để người dùng thấy ngay ô còn thiếu.
                // Tab khác: không đổi tab (đổi sang tab trẻ <6 còn bật hỏi xác nhận tuổi) — chỉ báo mở tab nào.
                if (onRuleTab) ShowConclusionSubTab(ruleTab);
                Control iconRank = onRuleTab ? cboRank : null;
                Control iconConcluder = onRuleTab ? cboConcluder : null;
                Control iconHealth = onRuleTab ? rdoConclusionHealth8 : null;
                List<string> messages = new List<string>();
                if (!okRank)
                    AddConclusionFinishError(iconRank, ConclusionFinishMessage(
                        underSix ? "Xếp loại tình trạng sức khỏe chung" : "Phân loại", savedRank, screenRank, onRuleTab), messages);
                if (!okConcluder)
                    AddConclusionFinishError(iconConcluder, ConclusionFinishMessage(
                        underSix ? "Bác sĩ khám" : "Người khám", savedConcluder, screenConcluder, onRuleTab), messages);
                if (!okHealth)
                    AddConclusionFinishError(iconHealth, ConclusionFinishMessage(
                        "Kết luận về sức khỏe", savedHealth, screenHealth, onRuleTab), messages);

                string tabName = GetMainTabCaption(ruleTab);
                string header = isAutoAfterSave
                    ? "Đã lưu hồ sơ nhưng CHƯA tự động kết thúc y lệnh khám vì mục kết luận chưa đủ:"
                    : onRuleTab
                        ? "Chưa thể kết thúc y lệnh khám vì mục kết luận chưa đủ:"
                        : "Chưa thể kết thúc y lệnh khám vì hồ sơ \"" + tabName + "\" chưa đủ mục kết luận:";
                string footer = onRuleTab
                    ? "Nhập đủ mục kết luận, bấm Lưu rồi kết thúc y lệnh khám."
                    : "Mở tab \"" + tabName + "\", nhập đủ mục kết luận, bấm Lưu rồi kết thúc y lệnh khám.";
                XtraMessageBox.Show(
                    header + "\r\n\r\n- " + string.Join("\r\n- ", messages.ToArray()) + "\r\n\r\n" + footer,
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusFirstInvalidControl();
                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return true; // lỗi kiểm tra không được chặn nghiệp vụ kết thúc
            }
        }

        /// <summary>
        /// Loại hồ sơ KSK (tab) phải áp quy tắc mục kết luận khi kết thúc khám — theo BẢN GHI của y lệnh:
        ///  - Tab đang mở là tab trên 18 / dưới 18 / trẻ <6 và có bản ghi riêng của tab đó -> tab đó.
        ///  - Có bản ghi riêng trên 18 / dưới 18 / trẻ <6 -> tab đó (nhiều loại thì ưu tiên loại khớp tuổi).
        ///  - Chỉ có bản ghi loại khác (định kỳ = chỉ HIS_KSK_GENERAL, lái xe, nghề nghiệp, KSK khác) -> -1.
        ///  - Chưa lưu gì -> tab đang mở nếu là tab trên 18 / dưới 18 / trẻ <6, không thì -1.
        /// Bản ghi = current* (đã nạp tab / vừa Lưu) hoặc pre* (tải sẵn lúc mở y lệnh, tab chưa nạp).
        /// </summary>
        private int ResolveConclusionRuleTab(int selectedTab)
        {
            try
            {
                bool over18 = currentKskOverEight != null || Cnt(preKskOverEighteens) > 0;
                bool under18 = currentKskUnderEight != null || Cnt(preKskUnderEighteens) > 0;
                bool underSix = currentKskUnderSixEf != null || Cnt(preKskUnderSixes) > 0;
                if ((selectedTab == TAB_OVER_EIGHTEEN && over18)
                    || (selectedTab == TAB_UNDER_EIGHTEEN && under18)
                    || (selectedTab == TAB_UNDER_SIX && underSix))
                    return selectedTab;
                if (over18 || under18 || underSix)
                {
                    int byAge = ResolveDefaultTabByAge();
                    if ((byAge == TAB_OVER_EIGHTEEN && over18) || (byAge == TAB_UNDER_EIGHTEEN && under18)
                        || (byAge == TAB_UNDER_SIX && underSix))
                        return byAge;
                    return over18 ? TAB_OVER_EIGHTEEN : under18 ? TAB_UNDER_EIGHTEEN : TAB_UNDER_SIX;
                }
                bool otherRecord = currentKskGeneral != null || Cnt(preKskGenerals) > 0
                    || currentKskPeriodDriver != null || Cnt(preKskPeriodDrivers) > 0
                    || currentKskDriverCar != null || Cnt(preKskDriverCars) > 0
                    || currentKskOther != null || Cnt(preKskOthers) > 0
                    || currentKsKOccupational != null || Cnt(preKskOccupationals) > 0;
                if (otherRecord) return -1;
                return (selectedTab == TAB_OVER_EIGHTEEN || selectedTab == TAB_UNDER_EIGHTEEN
                    || selectedTab == TAB_UNDER_SIX) ? selectedTab : -1;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return -1; }
        }

        /// <summary>Thêm message (luôn) + icon tại ô (nếu có ô — tức đang đứng đúng tab hồ sơ).</summary>
        private void AddConclusionFinishError(Control ctrl, string message, List<string> messages)
        {
            messages.Add(message);
            if (ctrl != null && !requiredInvalidControls.Contains(ctrl)) SetRequiredError(ctrl, message);
        }

        /// <summary>Nội dung cảnh báo 1 ô mục kết luận khi kết thúc khám.</summary>
        private static string ConclusionFinishMessage(string label, bool saved, bool hasOnScreen, bool onRuleTab)
        {
            if (!onRuleTab) return label + " chưa có (chưa nhập hoặc chưa Lưu).";
            if (saved) return label + " đang để trống — chọn lại rồi Lưu."; // đã lưu nhưng vừa xóa trên màn hình
            return hasOnScreen
                ? label + " đã chọn nhưng chưa Lưu."
                : label + " bắt buộc chọn trước khi kết thúc khám.";
        }

        /// <summary>Tên tab ngoài cùng (để báo "mở tab ...").</summary>
        private string GetMainTabCaption(int tabIndex)
        {
            try
            {
                if (xtraTabControl1 != null && tabIndex >= 0 && tabIndex < xtraTabControl1.TabPages.Count)
                    return xtraTabControl1.TabPages[tabIndex].Text;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return "";
        }

        /// <summary>
        /// Mục kết luận trong bản ghi ĐÃ LƯU của hồ sơ — current* (đã nạp tab / trả về sau lần Lưu gần
        /// nhất), tab chưa nạp thì lấy pre* (tải sẵn lúc mở y lệnh):
        ///  - Trên 18:  Phân loại = HIS_KSK_OVER_EIGHTEEN.HEALTH_EXAM_RANK_ID.
        ///  - Dưới 18:  Phân loại = HIS_KSK_UNDER_EIGHTEEN.HEALTH_EXAM_RANK_ID.
        ///  - Trẻ <6:   Phân loại = HIS_KSK_GENERAL.HEALTH_EXAM_RANK_ID,
        ///              Kết luận về sức khỏe = HIS_KSK_GENERAL.HEALTH_CONCLUSION_TYPE.
        ///  - Người khám của cả 3 loại = HIS_KSK_GENERAL.CONCLUDER_LOGINNAME.
        /// </summary>
        private void GetSavedConclusionState(int tabIndex, out bool rank, out bool concluder, out bool health)
        {
            rank = false;
            concluder = false;
            health = false;
            try
            {
                MOS.EFMODEL.DataModels.HIS_KSK_GENERAL g = currentKskGeneral ?? FirstOrNull(preKskGenerals);
                concluder = g != null && !string.IsNullOrWhiteSpace(g.CONCLUDER_LOGINNAME);
                if (tabIndex == TAB_OVER_EIGHTEEN)
                {
                    MOS.EFMODEL.DataModels.HIS_KSK_OVER_EIGHTEEN o = currentKskOverEight ?? FirstOrNull(preKskOverEighteens);
                    rank = o != null && o.HEALTH_EXAM_RANK_ID.HasValue;
                }
                else if (tabIndex == TAB_UNDER_EIGHTEEN)
                {
                    MOS.EFMODEL.DataModels.HIS_KSK_UNDER_EIGHTEEN u = currentKskUnderEight ?? FirstOrNull(preKskUnderEighteens);
                    rank = u != null && u.HEALTH_EXAM_RANK_ID.HasValue;
                }
                else if (tabIndex == TAB_UNDER_SIX)
                {
                    rank = g != null && g.HEALTH_EXAM_RANK_ID.HasValue;
                    health = g != null && g.HEALTH_CONCLUSION_TYPE.HasValue;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static T FirstOrNull<T>(List<T> list) where T : class
        {
            return (list != null && list.Count > 0) ? list[0] : null;
        }

        /// <summary>
        /// Mở sub-tab "Kết luận" của tab trên 18 tuổi (xtraTabControl2 -> xtraTabPage12) / dưới 18 tuổi
        /// (xtraTabControl3 -> xtraTabPage16). Tab trẻ dưới 6 tuổi không có sub-tab.
        /// </summary>
        private void ShowConclusionSubTab(int tabIndex)
        {
            try
            {
                if (tabIndex == 1 && xtraTabControl2 != null && xtraTabPage12 != null)
                    xtraTabControl2.SelectedTabPage = xtraTabPage12;
                else if (tabIndex == 2 && xtraTabControl3 != null && xtraTabPage16 != null)
                    xtraTabControl3.SelectedTabPage = xtraTabPage16;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Log riêng cho luật bắt buộc mục kết luận — tìm trong log bằng từ khóa "KskConclusion".</summary>
        private static void LogKskConclusion(string message)
        {
            try { LogSystem.Debug("KskConclusion: " + message); }
            catch (Exception ex) { LogSystem.Warn(ex); }
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

        #endregion

        #region Tiện ích

        /// <summary>GridLookUpEdit đã chọn giá trị hay chưa.</summary>
        private bool HasLookUpValue(GridLookUpEdit cbo)
        {
            return cbo != null && cbo.EditValue != null && cbo.EditValue != DBNull.Value;
        }

        /// <summary>
        /// Gắn cảnh báo + thêm message. BỎ QUA nếu control đã bị báo lỗi ở bước kiểm tra trước
        /// — tránh message trùng.
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
