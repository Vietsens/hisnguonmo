/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Giới hạn độ dài theo QĐ 2062/QĐ-BYT ngay lúc NHẬP (BV Nguyễn Đình Chiểu, 25/09/2026): trước đây
 * nhập dài quá chỉ bị phát hiện ở màn "Đồng bộ KSK" ("VLG: KHÔNG đẩy hồ sơ — vượt độ dài theo QĐ 2062:
 * KHONG_KINH_MAT_PHAI dài 6 (tối đa 5)") nên phải quay lại sửa từng hồ sơ.
 *
 * CHỈ BẬT khi chi nhánh đang làm việc có liên thông cổng 2062 (HisConfigCFG.IsQd2062LengthCheck) —
 * viện không đẩy cổng thì không bị giới hạn.
 *
 * 2 lớp:
 *  1. InitQd2062Length (lúc Load): đặt Properties.MaxLength cho các ô chữ tự do -> không gõ quá được.
 *  2. ValidateQd2062LengthBeforePost (lúc Lưu, NGAY TRƯỚC khi gọi api/HisServiceReq/KskExecuteV2):
 *     đo trên CHÍNH dữ liệu sắp gửi (entity trong SDO), không đo trên ô nhập — MaxLength chỉ chặn gõ
 *     phím, còn giá trị đổ bằng code (nhập mẫu Excel, thư viện văn bản mắt, lấy sẵn từ y lệnh khám
 *     PART_EXAM_*, dữ liệu cũ) và giá trị ghép (mã ICD nối ";", Đối tượng nối ";", số cân nặng) thì
 *     không qua MaxLength.
 *
 * Bảng cột -> thẻ -> độ dài đối chiếu thực nghiệm với thư viện His.Ksk.QD2062 (bản dùng ở KskSyncList)
 * và KskSyncList.KskVlgLengthRules: thư viện chép NGUYÊN VĂN chuỗi (không Trim) và in số thập phân
 * đủ chữ số (65.50 -> "65.50") nên độ dài đo ở đây = độ dài thẻ trong bản tin.
 * NGOẠI LỆ: 4 ô tai (TAI_*_NOI_THUONG / NOI_THAM) thư viện tự CẮT còn 10 ký tự nên cổng không bao giờ
 * từ chối -> chỉ giới hạn lúc gõ (MaxLength 10), KHÔNG chặn Lưu (dữ liệu cũ dài hơn vẫn lưu được).
 * Các cột mã radio (1 ký tự) / số nguyên lớn (MACH, HUYET_AP tối đa 100) không thể vượt nên không kiểm.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        // Độ dài tối đa theo QĐ 2062 (cùng số với KskSyncList.KskVlgLengthRules).
        private const int QD2062_MAX_THI_LUC = 5;       // KHONG_KINH_MAT_*, CO_KINH_MAT_*
        private const int QD2062_MAX_TAI = 10;          // TAI_*_NOI_THUONG / NOI_THAM (thư viện tự cắt — chỉ MaxLength)
        private const int QD2062_MAX_CAN_NANG = 6;      // CAN_NANG
        private const int QD2062_MAX_SO_DO = 10;        // CHIEU_CAO, CHIEU_DAI, VONG_DAU, CHU_VI_VONG_CANH_TAY, NHIET_DO, NHIP_THO, *_TUOI_SD
        private const int QD2062_MAX_MACH = 100;        // MACH
        private const int QD2062_MAX_DIEN_THOAI = 15;   // DIEN_THOAI_NGUOI_DI_CUNG
        private const int QD2062_MAX_DOI_TUONG = 50;    // DOI_TUONG
        private const int QD2062_MAX_MA_BENH = 255;     // KET_LUAN_BENH, TSGD_MA_BENH, TSBT_MA_BENH, TSBT_MA_BENH_THAI_SAN, TSBT_MA_BENH_KHAC
        private const int QD2062_MAX_HO_TEN = 255;      // HO_TEN_NGUOI_DI_CUNG
        private const int QD2062_MAX_TEN_THUOC = 1024;  // TSBT_TEN_THUOC_LIEU_LUONG, TSBT_TEN_THUOC_THAI_SAN

        /// <summary>Ô chữ đã đặt MaxLength theo QĐ 2062 -> độ dài tối đa (để tự xóa cảnh báo khi đã sửa ngắn lại).</summary>
        private readonly Dictionary<Control, int> qd2062TextLimits = new Dictionary<Control, int>();

        /// <summary>
        /// Ô đang báo vượt độ dài KHÔNG phải ô chữ (ô số DHST, combo Đối tượng, UC mã ICD) -> hàm "còn vượt
        /// không" để tự xóa icon khi đã sửa đúng (ui_rules) và giữ icon khi vẫn còn vượt.
        /// Xóa cùng icon ở ClearRequiredError / ClearAllRequiredErrors.
        /// </summary>
        private readonly Dictionary<Control, Func<bool>> qd2062Recheck = new Dictionary<Control, Func<bool>>();

        #region Lớp 1 — MaxLength lúc Load

        /// <summary>
        /// Đặt MaxLength theo QĐ 2062 cho các ô chữ tự do có đẩy lên cổng. Gọi 1 lần lúc Load
        /// (sau InitRequiredValidation). Chi nhánh không liên thông cổng 2062 -> không làm gì.
        /// </summary>
        private void InitQd2062Length()
        {
            try
            {
                if (!Config.HisConfigCFG.IsQd2062LengthCheck) return;

                // Tab KSK trên 18 tuổi (hậu tố 2).
                ApplyQd2062MaxLength(txtExamEyeSightRight2, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightLeft2, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightGlassRight2, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightGlassLeft2, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEntLeftNormal2, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntLeftWhisper2, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntRightNomal2, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntRightWhisper2, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtMedicineUsing, QD2062_MAX_TEN_THUOC);
                ApplyQd2062MaxLength(txtMaternityHistory, QD2062_MAX_TEN_THUOC);
                ApplyQd2062MaxLength(txtPathologicalHistory2, QD2062_MAX_MA_BENH);

                // Tab KSK dưới 18 tuổi (hậu tố 3).
                ApplyQd2062MaxLength(txtExamEyeSightRight3, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightLeft3, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightGlassRight3, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEyeSightGlassLeft3, QD2062_MAX_THI_LUC);
                ApplyQd2062MaxLength(txtExamEntLeftNormal3, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntLeftWhisper3, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntRightNomal3, QD2062_MAX_TAI);
                ApplyQd2062MaxLength(txtExamEntRightWhisper3, QD2062_MAX_TAI);

                // Tab trẻ em dưới 6 tuổi (hậu tố 8) — các ô số đo là TextEdit lưu nguyên chuỗi.
                ApplyQd2062MaxLength(txtAccompanyPersonName8, QD2062_MAX_HO_TEN);
                ApplyQd2062MaxLength(txtAccompanyPhone8, QD2062_MAX_DIEN_THOAI);
                ApplyQd2062MaxLength(spnTemperature8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnPulse8, QD2062_MAX_MACH);
                ApplyQd2062MaxLength(spnRespiratoryRate8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnBodyLength8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnBodyLengthAgeSd8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnWeight8, QD2062_MAX_CAN_NANG);
                ApplyQd2062MaxLength(spnWeightAgeSd8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnHeadCircumference8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(spnArmCircumference8, QD2062_MAX_SO_DO);
                ApplyQd2062MaxLength(memConclusionDetail8, QD2062_MAX_MA_BENH);

                // Tab KSK định kỳ — "Bệnh tật" (HIS_KSK_GENERAL.DISEASES -> KET_LUAN_BENH).
                ApplyQd2062MaxLength(txtDiseases, QD2062_MAX_MA_BENH);

                // Ô số DHST (SpinEdit): không đặt MaxLength (ảnh hưởng nhập số); sửa giá trị -> tính lại icon.
                SpinEdit[] dhstSpins = new SpinEdit[]
                {
                    spnWeight, spnHeight, spnWeight2, spnHeight2, spnWeight3, spnHeight3, spnWeight7, spnHeight7
                };
                foreach (SpinEdit spn in dhstSpins)
                {
                    if (spn == null) continue;
                    spn.EditValueChanged -= Qd2062Recheck_Changed;
                    spn.EditValueChanged += Qd2062Recheck_Changed;
                }

                LogSystem.Info("KskQd2062Length: bat gioi han do dai QD 2062 cho " + qd2062TextLimits.Count
                    + " o nhap (chi nhanh lien thong cong " + Config.HisConfigCFG.Qd2062ConnectionKey + ")");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void ApplyQd2062MaxLength(TextEdit edit, int maxLength)
        {
            try
            {
                if (edit == null) return;
                // Giữ giới hạn sẵn có nếu đã chặt hơn.
                int current = edit.Properties.MaxLength;
                if (current <= 0 || current > maxLength) edit.Properties.MaxLength = maxLength;
                qd2062TextLimits[edit] = maxLength;
                edit.TextChanged -= Qd2062Text_TextChanged;
                edit.TextChanged += Qd2062Text_TextChanged;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đã sửa ngắn lại trong giới hạn -> xóa icon cảnh báo độ dài của ô.</summary>
        private void Qd2062Text_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                if (ctrl == null || !requiredInvalidControls.Contains(ctrl)) return;
                if (IsQd2062TooLong(ctrl)) return;
                // Ô vừa bắt buộc vừa giới hạn độ dài (Họ tên người đi cùng trẻ): xóa trắng thì để
                // RequiredEdit_ValueChanged / lần Lưu sau xử lý, không xóa icon ở đây.
                if (string.IsNullOrWhiteSpace(ctrl.Text) && ctrl == txtAccompanyPersonName8) return;
                ClearRequiredError(ctrl);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ô số / UC ICD đổi giá trị -> xóa icon các ô đã hết vượt độ dài.</summary>
        private void Qd2062Recheck_Changed(object sender, EventArgs e)
        {
            ReevaluateQd2062Icons();
        }

        private void Qd2062HistoryIcd_Changed(KskHistoryGroup group, string codes, string names)
        {
            ReevaluateQd2062Icons();
        }

        private void ReevaluateQd2062Icons()
        {
            try
            {
                if (qd2062Recheck.Count == 0) return;
                foreach (KeyValuePair<Control, Func<bool>> kv in qd2062Recheck.ToList())
                {
                    bool stillTooLong = true;
                    try { stillTooLong = kv.Value(); }
                    catch (Exception exCheck) { LogSystem.Warn(exCheck); }
                    if (!stillTooLong) ClearRequiredError(kv.Key);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Ô đang vượt giới hạn QĐ 2062 (ô chữ theo MaxLength đã đặt; ô khác theo qd2062Recheck).</summary>
        private bool IsQd2062TooLong(Control ctrl)
        {
            try
            {
                if (ctrl == null) return false;
                int max;
                if (qd2062TextLimits.TryGetValue(ctrl, out max))
                    return ctrl.Text != null && ctrl.Text.Length > max;
                Func<bool> recheck;
                if (qd2062Recheck.TryGetValue(ctrl, out recheck)) return recheck();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return false;
        }

        #endregion

        #region Lớp 2 — kiểm tra trên dữ liệu sắp gửi

        /// <summary>
        /// Kiểm tra độ dài theo QĐ 2062 trên dữ liệu SẮP GỬI (SDO đã dựng xong). Gọi trong btnSave_Click
        /// ngay trước khi POST. Vượt -> icon cảnh báo tại ô (nếu xác định được ô), thông báo tổng hợp,
        /// trả false để KHÔNG lưu (đã tự WaitingManager.Hide trước khi hiện hộp thoại).
        /// Chi nhánh không liên thông cổng 2062 -> luôn true.
        /// </summary>
        private bool ValidateQd2062LengthBeforePost(HisServiceReqKskExecuteV2SDO sdo)
        {
            try
            {
                if (!Config.HisConfigCFG.IsQd2062LengthCheck || sdo == null) return true;
                int tab = xtraTabControl1.SelectedTabPageIndex;
                ClearAllRequiredErrors();
                List<string> messages = new List<string>();
                List<string> logTags = new List<string>();

                if (sdo.KskOverEighteen != null)
                    CheckQd2062OverEighteen(sdo.KskOverEighteen.HisKskOverEighteen, messages, logTags);
                if (sdo.KskUnderEighteen != null)
                    CheckQd2062UnderEighteen(sdo.KskUnderEighteen.HisKskUnderEighteen, messages, logTags);
                if (sdo.KskUnderSix != null)
                {
                    CheckQd2062UnderSix(sdo.KskUnderSix.HisKskUnderSix, messages, logTags);
                    // SĐT người đi cùng không nằm trong SDO: lưu riêng vào HIS_PATIENT.RELATIVE_PHONE sau khi
                    // Lưu (SaveAccompanyPatientUnderSix, Trim) rồi lên cổng qua DIEN_THOAI_NGUOI_DI_CUNG.
                    string phone = (txtAccompanyPhone8 != null && txtAccompanyPhone8.Text != null) ? txtAccompanyPhone8.Text.Trim() : null;
                    CheckQd2062(phone, QD2062_MAX_DIEN_THOAI, "Số điện thoại người đi cùng", "DIEN_THOAI_NGUOI_DI_CUNG",
                        txtAccompanyPhone8, null, messages, logTags);
                }
                if (sdo.KskGeneral != null)
                    CheckQd2062General(sdo.KskGeneral.HisKskGeneral, tab, messages, logTags);

                // Sinh hiệu (HIS_DHST) — mỗi lần Lưu chỉ gửi DHST của tab đang lưu. Tab trẻ <6 KHÔNG xét:
                // mẫu trẻ em lấy cân nặng / chiều dài từ HIS_KSK_UNDER_SIX (chuỗi, đã kiểm ở trên), DHST
                // chỉ là bản số parse lại từ cùng ô nhập.
                HIS_DHST dhst = null;
                if (sdo.KskGeneral != null && sdo.KskGeneral.HisDhst != null) dhst = sdo.KskGeneral.HisDhst;
                else if (sdo.KskOverEighteen != null && sdo.KskOverEighteen.HisDhst != null) dhst = sdo.KskOverEighteen.HisDhst;
                else if (sdo.KskUnderEighteen != null && sdo.KskUnderEighteen.HisDhst != null) dhst = sdo.KskUnderEighteen.HisDhst;
                else if (sdo.KskOccupationalV2 != null && sdo.KskOccupationalV2.HisDhst != null) dhst = sdo.KskOccupationalV2.HisDhst;
                if (tab != TAB_UNDER_SIX) CheckQd2062Dhst(dhst, tab, messages, logTags);

                if (messages.Count == 0) return true;

                // Chỉ log tên thẻ + độ dài — KHÔNG log nội dung (dữ liệu bệnh nhân).
                LogSystem.Info("KskQd2062Length: CHAN LUU (tab=" + tab + ", service_req="
                    + sdo.ServiceReqId + "): " + string.Join("; ", logTags.ToArray()));
                WaitingManager.Hide(); // btnSave_Click đang hiện màn chờ lúc dựng SDO
                XtraMessageBox.Show(
                    "Các trường sau vượt độ dài tối đa theo QĐ 2062/QĐ-BYT (hồ sơ sẽ không đẩy được lên cổng):\r\n\r\n- "
                    + string.Join("\r\n- ", messages.ToArray())
                    + "\r\n\r\nVui lòng sửa ngắn lại rồi Lưu.",
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

        private void CheckQd2062OverEighteen(HIS_KSK_OVER_EIGHTEEN o, List<string> messages, List<string> logTags)
        {
            if (o == null) return;
            CheckQd2062(o.EXAM_EYESIGHT_RIGHT, QD2062_MAX_THI_LUC, "Thị lực không kính - mắt phải", "KHONG_KINH_MAT_PHAI", txtExamEyeSightRight2, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_LEFT, QD2062_MAX_THI_LUC, "Thị lực không kính - mắt trái", "KHONG_KINH_MAT_TRAI", txtExamEyeSightLeft2, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_GLASS_RIGHT, QD2062_MAX_THI_LUC, "Thị lực có kính - mắt phải", "CO_KINH_MAT_PHAI", txtExamEyeSightGlassRight2, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_GLASS_LEFT, QD2062_MAX_THI_LUC, "Thị lực có kính - mắt trái", "CO_KINH_MAT_TRAI", txtExamEyeSightGlassLeft2, null, messages, logTags);
            CheckQd2062(o.MEDICINE_USING, QD2062_MAX_TEN_THUOC, "Thuốc đang dùng", "TSBT_TEN_THUOC_LIEU_LUONG", txtMedicineUsing, null, messages, logTags);
            CheckQd2062(o.MATERNITY_HISTORY, QD2062_MAX_TEN_THUOC, "Tiền sử thai sản", "TSBT_TEN_THUOC_THAI_SAN", txtMaternityHistory, null, messages, logTags);
            CheckQd2062(o.PATHOLOGICAL_HISTORY, QD2062_MAX_MA_BENH, "Tiền sử bệnh", "TSBT_MA_BENH_KHAC", txtPathologicalHistory2, null, messages, logTags);
            CheckQd2062(o.KSK_PATIENT_TYPES, QD2062_MAX_DOI_TUONG, "Đối tượng (chọn quá nhiều)", "DOI_TUONG", cboObject,
                () => GetKskObjectValue(), messages, logTags);
        }

        private void CheckQd2062UnderEighteen(HIS_KSK_UNDER_EIGHTEEN o, List<string> messages, List<string> logTags)
        {
            if (o == null) return;
            CheckQd2062(o.EXAM_EYESIGHT_RIGHT, QD2062_MAX_THI_LUC, "Thị lực không kính - mắt phải", "KHONG_KINH_MAT_PHAI", txtExamEyeSightRight3, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_LEFT, QD2062_MAX_THI_LUC, "Thị lực không kính - mắt trái", "KHONG_KINH_MAT_TRAI", txtExamEyeSightLeft3, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_GLASS_RIGHT, QD2062_MAX_THI_LUC, "Thị lực có kính - mắt phải", "CO_KINH_MAT_PHAI", txtExamEyeSightGlassRight3, null, messages, logTags);
            CheckQd2062(o.EXAM_EYESIGHT_GLASS_LEFT, QD2062_MAX_THI_LUC, "Thị lực có kính - mắt trái", "CO_KINH_MAT_TRAI", txtExamEyeSightGlassLeft3, null, messages, logTags);
            CheckQd2062(o.KSK_PATIENT_TYPES, QD2062_MAX_DOI_TUONG, "Đối tượng (chọn quá nhiều)", "DOI_TUONG", cboObject3,
                () => GetObjectValueExt(cboObject3), messages, logTags);
        }

        private void CheckQd2062UnderSix(HIS_KSK_UNDER_SIX o, List<string> messages, List<string> logTags)
        {
            if (o == null) return;
            CheckQd2062(o.ACCOMPANY_PERSON_NAME, QD2062_MAX_HO_TEN, "Họ tên người đi cùng trẻ", "HO_TEN_NGUOI_DI_CUNG", txtAccompanyPersonName8, null, messages, logTags);
            CheckQd2062(o.TEMPERATURE, QD2062_MAX_SO_DO, "Nhiệt độ", "NHIET_DO", spnTemperature8, null, messages, logTags);
            CheckQd2062(o.PULSE, QD2062_MAX_MACH, "Mạch", "MACH", spnPulse8, null, messages, logTags);
            CheckQd2062(o.RESPIRATORY_RATE, QD2062_MAX_SO_DO, "Nhịp thở", "NHIP_THO", spnRespiratoryRate8, null, messages, logTags);
            CheckQd2062(o.BODY_LENGTH, QD2062_MAX_SO_DO, "Chiều dài/chiều cao", "CHIEU_DAI", spnBodyLength8, null, messages, logTags);
            CheckQd2062(o.BODY_LENGTH_AGE_SD, QD2062_MAX_SO_DO, "Chiều dài/tuổi (SD)", "CHIEU_DAI_TUOI_SD", spnBodyLengthAgeSd8, null, messages, logTags);
            CheckQd2062(o.WEIGHT, QD2062_MAX_CAN_NANG, "Cân nặng", "CAN_NANG", spnWeight8, null, messages, logTags);
            CheckQd2062(o.WEIGHT_AGE_SD, QD2062_MAX_SO_DO, "Cân nặng/tuổi (SD)", "CAN_NANG_TUOI_SD", spnWeightAgeSd8, null, messages, logTags);
            CheckQd2062(o.HEAD_CIRCUMFERENCE, QD2062_MAX_SO_DO, "Vòng đầu", "VONG_DAU", spnHeadCircumference8, null, messages, logTags);
            CheckQd2062(o.ARM_CIRCUMFERENCE, QD2062_MAX_SO_DO, "Chu vi vòng cánh tay", "CHU_VI_VONG_CANH_TAY", spnArmCircumference8, null, messages, logTags);
            CheckQd2062(o.KSK_PATIENT_TYPES, QD2062_MAX_DOI_TUONG, "Đối tượng (chọn quá nhiều)", "DOI_TUONG", cboObject8,
                () => GetObjectValueExt(cboObject8), messages, logTags);
        }

        /// <summary>
        /// HIS_KSK_GENERAL: kết luận bệnh + các mã ICD (tiền sử / kết luận) — gửi ở MỌI tab.
        /// Ô gắn cảnh báo: "Bệnh tật" tab định kỳ / "Ghi rõ" tab trẻ <6; UC ICD kết luận và UC ICD tiền sử
        /// của tab đang lưu (mã ICD là chuỗi ghép ";" chọn từ danh sách nên không chặn được lúc gõ).
        /// </summary>
        private void CheckQd2062General(HIS_KSK_GENERAL g, int tab, List<string> messages, List<string> logTags)
        {
            if (g == null) return;
            Control diseasesCtrl = (tab == 0) ? (Control)txtDiseases : (tab == TAB_UNDER_SIX) ? (Control)memConclusionDetail8 : null;
            CheckQd2062(g.DISEASES, QD2062_MAX_MA_BENH, "Kết luận bệnh", "KET_LUAN_BENH", diseasesCtrl, null, messages, logTags);

            UcKskConclusionIcd icdUc = null;
            if (dicIcdConclusionUc != null && dicIcdConclusionUc.ContainsKey(tab)) icdUc = dicIcdConclusionUc[tab];
            if (icdUc != null) { icdUc.Leave -= Qd2062Recheck_Changed; icdUc.Leave += Qd2062Recheck_Changed; }
            CheckQd2062(g.CONCLUSION_ICD_CODE, QD2062_MAX_MA_BENH, "Mã ICD-10 kết luận (chọn quá nhiều mã)", "KET_LUAN_BENH", icdUc,
                icdUc == null ? (Func<string>)null : () => icdUc.GetConclusionIcdCode(), messages, logTags);

            CheckQd2062HistoryIcd(g.FAMILY_HISTORY_ICD_CODE, KskHistoryGroup.Family, "Mã ICD tiền sử gia đình (chọn quá nhiều mã)", "TSGD_MA_BENH", messages, logTags);
            CheckQd2062HistoryIcd(g.PERSONAL_HISTORY_ICD_CODE, KskHistoryGroup.Personal, "Mã ICD tiền sử bản thân (chọn quá nhiều mã)", "TSBT_MA_BENH", messages, logTags);
            CheckQd2062HistoryIcd(g.OBSTETRIC_DISEASE_ICD_CODE, KskHistoryGroup.Obstetric, "Mã ICD tiền sử thai sản (chọn quá nhiều mã)", "TSBT_MA_BENH_THAI_SAN", messages, logTags);
        }

        /// <summary>1 nhóm ICD tiền sử: icon tại UC của nhóm trên tab đang mở; đổi mã -> tính lại icon.</summary>
        private void CheckQd2062HistoryIcd(string value, KskHistoryGroup group, string label, string tag,
            List<string> messages, List<string> logTags)
        {
            UcKskHistoryIcd uc = FindHistoryIcdUcOnCurrentTab(group);
            if (uc != null) { uc.IcdChanged -= Qd2062HistoryIcd_Changed; uc.IcdChanged += Qd2062HistoryIcd_Changed; }
            Func<string> current = () =>
            {
                string ids, names;
                if (group == KskHistoryGroup.Family && GetSytFamilyIcdValue(out ids, out names)) return ids;
                return GetHistoryIcdCode(group);
            };
            CheckQd2062(value, QD2062_MAX_MA_BENH, label, tag, uc, current, messages, logTags);
        }

        /// <summary>
        /// HIS_DHST: cân nặng / chiều cao là SỐ — thư viện in đủ chữ số thập phân (InvariantCulture,
        /// vd 65.50 -> "65.50") nên đo độ dài chuỗi đó. Ô số đo theo tab (hậu tố không trùng chỉ số tab).
        /// </summary>
        private void CheckQd2062Dhst(HIS_DHST dhst, int tab, List<string> messages, List<string> logTags)
        {
            if (dhst == null) return;
            SpinEdit weightCtrl = null, heightCtrl = null;
            if (tab == 0) { weightCtrl = spnWeight; heightCtrl = spnHeight; }
            else if (tab == TAB_OVER_EIGHTEEN) { weightCtrl = spnWeight2; heightCtrl = spnHeight2; }
            else if (tab == TAB_UNDER_EIGHTEEN) { weightCtrl = spnWeight3; heightCtrl = spnHeight3; }
            else if (tab == 6) { weightCtrl = spnWeight7; heightCtrl = spnHeight7; }
            CheckQd2062(FormatQd2062Number(dhst.WEIGHT), QD2062_MAX_CAN_NANG, "Cân nặng (tối đa 999.99)", "CAN_NANG", weightCtrl,
                weightCtrl == null ? (Func<string>)null : () => FormatQd2062Number(GetSpinRounded(weightCtrl)), messages, logTags);
            CheckQd2062(FormatQd2062Number(dhst.HEIGHT), QD2062_MAX_SO_DO, "Chiều cao", "CHIEU_CAO", heightCtrl,
                heightCtrl == null ? (Func<string>)null : () => FormatQd2062Number(GetSpinRounded(heightCtrl)), messages, logTags);
        }

        private static string FormatQd2062Number(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        /// <summary>Giá trị ô số làm tròn 2 chữ số như lúc lưu HIS_DHST (RoundCurrency(v, 2)).</summary>
        private static decimal? GetSpinRounded(SpinEdit spn)
        {
            if (spn == null || spn.EditValue == null || spn.EditValue == DBNull.Value) return null;
            return Inventec.Common.Number.Get.RoundCurrency(spn.Value, 2);
        }

        /// <summary>
        /// 1 trường: vượt độ dài -> message "Nhãn dài N ký tự (tối đa M — THẺ)" + icon tại ô (nếu có ô và ô
        /// chưa bị báo lỗi). Message luôn được thêm (kể cả không xác định được ô), không trùng lặp.
        /// currentValue: cách đọc lại giá trị hiện tại của ô KHÔNG phải ô chữ (để tự xóa icon khi đã sửa).
        /// </summary>
        private void CheckQd2062(string value, int max, string label, string tag, Control ctrl, Func<string> currentValue,
            List<string> messages, List<string> logTags)
        {
            if (value == null || value.Length <= max) return;
            string msg = label + " dài " + value.Length + " ký tự (tối đa " + max + " — " + tag + ")";
            if (messages.Contains(msg)) return;
            messages.Add(msg);
            logTags.Add(tag + " dai " + value.Length + "/" + max);
            if (ctrl == null || requiredInvalidControls.Contains(ctrl)) return;
            SetRequiredError(ctrl, msg);
            if (currentValue != null && !qd2062TextLimits.ContainsKey(ctrl))
            {
                qd2062Recheck[ctrl] = () =>
                {
                    string now = currentValue();
                    return now != null && now.Length > max;
                };
            }
        }

        /// <summary>UC ICD tiền sử của nhóm nằm trên tab đang mở (mỗi nhóm có 1 UC trên nhiều tab).</summary>
        private UcKskHistoryIcd FindHistoryIcdUcOnCurrentTab(KskHistoryGroup group)
        {
            try
            {
                List<UcKskHistoryIcd> list;
                if (!dicHistoryIcdUc.TryGetValue(group, out list) || list == null) return null;
                Control page = xtraTabControl1.SelectedTabPage;
                if (page == null) return null;
                foreach (UcKskHistoryIcd uc in list)
                {
                    if (uc == null) continue;
                    for (Control p = uc.Parent; p != null; p = p.Parent)
                        if (p == page) return uc;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        #endregion
    }
}
