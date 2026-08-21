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
                // Tab trẻ em dưới 6 tuổi — 3 trường bắt buộc bổ sung.
                WireRequiredClearEvent(txtAccompanyPersonName8);
                WireRequiredClearEvent(rdoAccompanyRelationship8);
                WireRequiredClearEvent(rdoConclusionHealth8);

                // Đổi tab -> bỏ cảnh báo của tab cũ (cảnh báo chỉ có nghĩa với tab đang lưu).
                this.xtraTabControl1.SelectedPageChanged
                    += new DevExpress.XtraTab.TabPageChangedEventHandler(RequiredValidation_TabChanged);
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
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RequiredValidation_TabChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try { ClearAllRequiredErrors(); }
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
                    ValidateObjectAndPaySource(cboObject, GetKskObjectValue(), cboPaymentSource, messages);
                else if (tabIndex == 2) // KSK dưới 18 tuổi
                    ValidateObjectAndPaySource(cboObject3, GetObjectValueExt(cboObject3), cboPaymentSource3, messages);
                else if (tabIndex == 7) // Trẻ em dưới 6 tuổi
                {
                    ValidateObjectAndPaySource(cboObject8, GetObjectValueExt(cboObject8), cboPaymentSource8, messages);
                    ValidateRequiredUnderSix(messages);
                }

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
