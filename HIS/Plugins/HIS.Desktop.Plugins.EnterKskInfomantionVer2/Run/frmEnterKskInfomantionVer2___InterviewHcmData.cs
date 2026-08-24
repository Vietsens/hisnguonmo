/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * LƯU / NẠP tab "Hỏi bệnh lâm sàng HCM" (mục D của Mẫu 4 — TT25).
 *
 * VÌ SAO GÓI VÀO MỘT CỘT JSON THAY VÌ 94 CỘT: bộ câu hỏi của Sở còn thay đổi — mục D3 đã có trên
 * biểu mẫu in mà bản tin của cổng chưa có trường tương ứng. Mỗi lần Sở thêm/bớt câu, nếu là cột
 * thật thì phải chạy lại vòng nâng cấp cơ sở dữ liệu -> sinh lại thực thể -> sửa đối tượng truyền
 * -> phát hành. Còn 94 chỉ tiêu này chỉ nhập một lần, in ra giấy rồi đẩy cổng, HIS không bao giờ
 * tra cứu riêng từng câu. Trong MOS đã có tiền lệ: BANK_JSON_DATA, POS_RESULT_JSON, QR_CONFIG_JSON.
 *
 * BA CỘT:
 *   INTERVIEW_JSON            VARCHAR2(4000 BYTE)  — các câu trả lời
 *   INTERVIEW_OTHER_DISEASE   VARCHAR2(2000 BYTE)  — ô "Bệnh khác, ghi rõ"
 *   INTERVIEW_OTHER_SIGN      VARCHAR2(2000 BYTE)  — ô "Dấu hiệu khác, ghi rõ"
 * Hai ô chữ để RIÊNG chứ không nhét vào JSON: chúng là tiếng Việt, dài không lường trước được,
 * để chung thì rất dễ tràn 4000 byte và làm hỏng cả bản ghi.
 *
 * QUY ƯỚC GHI:
 *   - Ô tích có  -> ghi 1. Ô tích không -> KHÔNG ghi khoá. Đọc lại thấy vắng mặt thì hiểu là
 *     "Không", đúng nghĩa của biểu mẫu, mà bản ghi gọn đi rất nhiều.
 *   - Ô chọn -> ghi mã của cổng. Chưa chọn -> KHÔNG ghi. Khác hẳn ô tích: "chưa chọn mức tần suất"
 *     không đồng nghĩa với mức thấp nhất, nên không được suy ra giá trị.
 *   - Mục bị bỏ qua theo luật của mẫu đã bị xoá trắng ngay trên giao diện, nên tự khắc vắng mặt.
 *   - `_v` là số phiên bản bộ câu hỏi, để sau này Sở đổi mẫu còn biết hồ sơ cũ ghi theo bản nào.
 *
 * KHI ĐỌC: khoá lạ thì bỏ qua, khoá thiếu thì để trống — hồ sơ nhập hôm nay vẫn mở được sau khi
 * mẫu đổi, và ngược lại.
 *
 * BA CỘT NÀY ĐỌC/GHI QUA PHẢN CHIẾU (SetSytHcmValue / GetSytHcmStr) như cụm "Đề nghị" đang làm:
 * thư viện thực thể chỉ có thuộc tính sau khi sinh lại, chưa có thì các hàm đó ghi cảnh báo
 * "KHONG co cot" rồi bỏ qua, không làm hỏng luồng lưu.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        #region ===== Hằng số =====

        private const int IHCM_JSON_VERSION = 1;

        private const string IHCM_COL__JSON = "INTERVIEW_JSON";
        private const string IHCM_COL__OTHER_DISEASE = "INTERVIEW_OTHER_DISEASE";
        private const string IHCM_COL__OTHER_SIGN = "INTERVIEW_OTHER_SIGN";

        private const string IHCM_FIELD__OTHER_DISEASE = "benh_khac_hoibenh";
        private const string IHCM_FIELD__OTHER_SIGN = "dauhieu_khac";

        /// <summary>Ngưỡng cảnh báo: cột chứa được 4000 byte, kêu sớm để còn kịp xử lý.</summary>
        private const int IHCM_JSON_WARN_BYTES = 3800;

        #endregion

        /// <summary>Đang đổ dữ liệu vào ô — tạm ngưng áp bảng luật, xem ApplyInterviewHcmData.</summary>
        private bool ihcmLoading;

        /// <summary>Hồ sơ nạp TRƯỚC khi tab kịp dựng thì giữ tạm ở đây, dựng xong đổ vào.</summary>
        private bool ihcmHasPending;
        private string ihcmPendingJson;
        private string ihcmPendingOtherDisease;
        private string ihcmPendingOtherSign;

        #region ===== LƯU =====

        /// <summary>
        /// Ghi 3 cột của tab hỏi bệnh vào thực thể sắp lưu.
        ///
        /// Tab CHƯA DỰNG thì không ghi gì: viện chưa khai báo cấu hình cổng, hoặc người dùng lưu hồ
        /// sơ mà chưa từng mở tab. Ghi vào lúc đó là ghi đè dữ liệu cũ bằng bản rỗng — đúng cái lỗi
        /// đã mắc ở phần khám lâm sàng HCM.
        /// </summary>
        private void SetInterviewHcmToEntity(HIS_KSK_SYT_HCM d)
        {
            try
            {
                if (d == null) return;
                if (tabInterviewHcm == null) return;

                string json = BuildInterviewHcmJson();
                int bytes = Encoding.UTF8.GetByteCount(json ?? "");
                if (bytes > IHCM_JSON_WARN_BYTES)
                    LogSystem.Warn("SytHcm/HoiBenh: chuoi JSON dai " + bytes
                        + " byte, sat nguong 4000 cua cot INTERVIEW_JSON — can xem lai");

                SetSytHcmValue(d, IHCM_COL__JSON, json);
                SetSytHcmValue(d, IHCM_COL__OTHER_DISEASE, GetIhcmTextValue(IHCM_FIELD__OTHER_DISEASE));
                SetSytHcmValue(d, IHCM_COL__OTHER_SIGN, GetIhcmTextValue(IHCM_FIELD__OTHER_SIGN));

                LogSystem.Warn("SytHcm/HoiBenh: LUU " + bytes + " byte JSON");
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Gom mọi ô đã trả lời thành một chuỗi JSON viết liền.</summary>
        private string BuildInterviewHcmJson()
        {
            JObject o = new JObject();
            try
            {
                o["_v"] = IHCM_JSON_VERSION;

                foreach (KeyValuePair<string, Control> kv in dicInterviewHcm)
                {
                    // Hai ô chữ nằm ở cột riêng của chúng.
                    if (kv.Key == IHCM_FIELD__OTHER_DISEASE) continue;
                    if (kv.Key == IHCM_FIELD__OTHER_SIGN) continue;

                    CheckEdit chk = kv.Value as CheckEdit;
                    if (chk != null)
                    {
                        if (chk.Checked) o[kv.Key] = 1;
                        continue;
                    }

                    GridLookUpEdit cbo = kv.Value as GridLookUpEdit;
                    if (cbo != null && cbo.EditValue != null)
                    {
                        long v;
                        if (long.TryParse(cbo.EditValue.ToString(), out v) && v > 0) o[kv.Key] = v;
                        continue;
                    }

                    SpinEdit spin = kv.Value as SpinEdit;
                    if (spin != null && spin.EditValue != null && spin.Value > 0)
                        o[kv.Key] = spin.Value;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }

            return o.ToString(Newtonsoft.Json.Formatting.None);
        }

        private string GetIhcmTextValue(string field)
        {
            try
            {
                Control c;
                if (!dicInterviewHcm.TryGetValue(field, out c)) return null;
                MemoEdit txt = c as MemoEdit;
                if (txt == null) return null;
                return !string.IsNullOrWhiteSpace(txt.Text) ? txt.Text.Trim() : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        #endregion

        #region ===== NẠP =====

        /// <summary>Đọc 3 cột từ hồ sơ vừa nạp và đổ lên tab.</summary>
        private void FillInterviewHcmFromEntity(HIS_KSK_SYT_HCM d)
        {
            try
            {
                if (d == null) return;

                string json = GetSytHcmStr(d, IHCM_COL__JSON);
                string otherDisease = GetSytHcmStr(d, IHCM_COL__OTHER_DISEASE);
                string otherSign = GetSytHcmStr(d, IHCM_COL__OTHER_SIGN);

                // Hồ sơ thường nạp TRƯỚC khi tab dựng xong -> giữ lại, dựng xong đổ sau.
                if (tabInterviewHcm == null)
                {
                    ihcmHasPending = true;
                    ihcmPendingJson = json;
                    ihcmPendingOtherDisease = otherDisease;
                    ihcmPendingOtherSign = otherSign;
                    return;
                }

                ApplyInterviewHcmData(json, otherDisease, otherSign);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Đổ dữ liệu đã giữ tạm, gọi ngay sau khi dựng xong tab.</summary>
        private void ApplyPendingInterviewHcm()
        {
            try
            {
                if (!ihcmHasPending) return;
                ihcmHasPending = false;
                ApplyInterviewHcmData(ihcmPendingJson, ihcmPendingOtherDisease, ihcmPendingOtherSign);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ giá trị lên ô.
        ///
        /// Phải TẠM NGƯNG bảng luật trong lúc đổ: đổ theo thứ tự khoá trong JSON nên câu con có thể
        /// tới trước câu điều kiện của nó, luật chạy giữa chừng sẽ xoá trắng đúng cái vừa đổ vào.
        /// Đổ xong mới áp luật một lượt — lúc đó mọi ô điều kiện đã có giá trị đúng.
        /// </summary>
        private void ApplyInterviewHcmData(string json, string otherDisease, string otherSign)
        {
            try
            {
                ihcmLoading = true;
                ClearInterviewHcmControls();

                SetIhcmTextValue(IHCM_FIELD__OTHER_DISEASE, otherDisease);
                SetIhcmTextValue(IHCM_FIELD__OTHER_SIGN, otherSign);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject o = JObject.Parse(json);
                    int unknown = 0;
                    int filled = 0;

                    foreach (JProperty pr in o.Properties())
                    {
                        if (pr.Name == "_v") continue;

                        Control c;
                        if (!dicInterviewHcm.TryGetValue(pr.Name, out c)) { unknown++; continue; }

                        CheckEdit chk = c as CheckEdit;
                        if (chk != null)
                        {
                            chk.Checked = ToIhcmInt(pr.Value) == 1;
                            filled++;
                            continue;
                        }

                        SpinEdit spin = c as SpinEdit;
                        if (spin != null)
                        {
                            // JSON luôn dùng dấu chấm thập phân; đọc theo văn hoá của máy thì
                            // "55.5" ở máy đặt tiếng Việt sẽ thành 555.
                            decimal dv;
                            if (decimal.TryParse(pr.Value.ToString(), NumberStyles.Any,
                                    CultureInfo.InvariantCulture, out dv) && dv > 0)
                            {
                                spin.EditValue = dv;
                                filled++;
                            }
                            continue;
                        }

                        GridLookUpEdit cbo = c as GridLookUpEdit;
                        if (cbo != null)
                        {
                            // Mã trong danh mục là kiểu int; gán kiểu long thì ô chọn không khớp
                            // được dòng nào và hiện rỗng dù dữ liệu vẫn đúng.
                            int v = ToIhcmInt(pr.Value);
                            if (v > 0) { cbo.EditValue = v; filled++; }
                        }
                    }

                    if (unknown > 0)
                        LogSystem.Warn("SytHcm/HoiBenh: ho so co " + unknown
                            + " khoa khong con tren giao dien -> bo qua (mau cua So da doi?)");
                    LogSystem.Warn("SytHcm/HoiBenh: NAP " + filled + " o tu ho so");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            finally { ihcmLoading = false; }

            UpdateInterviewHcmEnabled();
        }

        private void ClearInterviewHcmControls()
        {
            try
            {
                foreach (KeyValuePair<string, Control> kv in dicInterviewHcm) IhcmClearValue(kv.Value);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void SetIhcmTextValue(string field, string value)
        {
            try
            {
                Control c;
                if (!dicInterviewHcm.TryGetValue(field, out c)) return;
                MemoEdit txt = c as MemoEdit;
                if (txt != null) txt.Text = value ?? "";
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static int ToIhcmInt(JToken t)
        {
            try
            {
                if (t == null) return 0;
                int v;
                return int.TryParse(t.ToString(), out v) ? v : 0;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return 0; }
        }

        #endregion
    }
}
