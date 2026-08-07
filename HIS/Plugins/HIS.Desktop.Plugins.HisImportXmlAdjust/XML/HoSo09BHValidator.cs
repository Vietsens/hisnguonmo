using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HIS.Desktop.Plugins.HisImportXmlAdjust.XML
{
    /// <summary>
    /// Kiểm tra nội dung hồ sơ điều chỉnh mẫu 09/BH TRƯỚC KHI đẩy cổng, theo đúng các quy tắc cổng kiểm
    /// (tài liệu MoTaAPI_GuiHoSoDieuChinh09BH mục 5-9 và danh mục mã lỗi mục II.4):
    ///   - 123: MAU_SO không phải 09/BH, thiếu thẻ bắt buộc, mã CSKCB trong XML không khớp tài khoản
    ///   - 124: TRANGTHAI, KY_QT, NGAY_RA &lt; NGAY_VAO, thiếu LYDO_DIEUCHINH, SOBANG_XML không hợp lệ
    ///   - 202: NGAY_VAO/NGAY_RA sai định dạng (12 ký tự yyyyMMddHHmm)
    ///   - 204: hồ sơ không có nội dung điều chỉnh
    /// Bắt lỗi tại máy trạm giúp người dùng biết ngay dòng Excel nào phải sửa, thay vì chờ cổng trả mã lỗi chung.
    /// </summary>
    public static class HoSo09BHValidator
    {
        private const string MAU_SO_09BH = "09/BH";
        private const string TIME_FORMAT_12 = "yyyyMMddHHmm";

        /// <summary>
        /// Kiểm tra 01 hồ sơ. Trả về null nếu hợp lệ, ngược lại là chuỗi mô tả các lỗi (ngăn cách bằng "; ").
        /// </summary>
        /// <param name="xml">Hồ sơ dựng từ dữ liệu Excel</param>
        /// <param name="maCsKcbBody">Mã CSKCB gửi ở body - phải trùng MA_CSKCB trong XML (tài liệu mục 6)</param>
        public static string Validate(XmlHoSoDieuChinhGD xml, string maCsKcbBody)
        {
            var errors = new List<string>();
            try
            {
                if (xml == null || xml.TT_HOSO == null || xml.TT_HOSO.Count == 0 || xml.TT_HOSO[0] == null)
                    return "Hồ sơ rỗng, không có thẻ TT_HOSO.";

                XmlTTHoSo hoSo = xml.TT_HOSO[0];

                // Reference của chữ ký trỏ vào TT_HOSO/@Id -> thiếu Id là chắc chắn lỗi chữ ký (mã 125)
                if (string.IsNullOrEmpty(hoSo.Id))
                    errors.Add("TT_HOSO thiếu thuộc tính Id (chữ ký số không trỏ được vào TT_HOSO)");

                ValidateTtMau(hoSo.TT_MAU, maCsKcbBody, errors);
                ValidateTtXml1(hoSo.TT_XML1, errors);
                ValidateTtDieuChinh(hoSo.TT_DIEUCHINH, errors);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                errors.Add("Lỗi kiểm tra dữ liệu hồ sơ: " + ex.Message);
            }

            return errors.Count > 0 ? string.Join("; ", errors.ToArray()) : null;
        }

        /// <summary>Mục 6 - TT_MAU (thông tin biểu).</summary>
        private static void ValidateTtMau(XmlTTMau ttMau, string maCsKcbBody, List<string> errors)
        {
            if (ttMau == null)
            {
                errors.Add("Thiếu thẻ TT_MAU");
                return;
            }

            if (!string.Equals(ttMau.MAU_SO, MAU_SO_09BH, StringComparison.OrdinalIgnoreCase))
                errors.Add("MAU_SO phải là " + MAU_SO_09BH);

            if (string.IsNullOrEmpty(ttMau.MA_CSKCB))
                errors.Add("Thiếu MA_CSKCB (kiểm tra mã cơ sở KCB của chi nhánh đang làm việc)");
            else if (!string.IsNullOrEmpty(maCsKcbBody) && ttMau.MA_CSKCB.Trim() != maCsKcbBody.Trim())
                errors.Add(string.Format("MA_CSKCB trong XML ({0}) không khớp mã gửi lên cổng ({1})", ttMau.MA_CSKCB, maCsKcbBody));

            if (!string.IsNullOrEmpty(ttMau.NGAYTHANGNAM) && !IsDate(ttMau.NGAYTHANGNAM, "yyyyMMdd"))
                errors.Add("NGAYTHANGNAM phải 8 ký tự yyyyMMdd");
        }

        /// <summary>Mục 7 - TT_XML1 (hồ sơ XML1 gốc bị điều chỉnh).</summary>
        private static void ValidateTtXml1(XmlTTXml1 ttXml1, List<string> errors)
        {
            if (ttXml1 == null)
            {
                errors.Add("Thiếu thẻ TT_XML1");
                return;
            }

            if (string.IsNullOrEmpty(ttXml1.MA_LK))
                errors.Add("Thiếu MA_LK (mã liên kết lượt KCB)");

            if (string.IsNullOrEmpty(ttXml1.KY_QT))
                errors.Add("Thiếu KY_QT (kỳ quyết toán)");
            else if (!IsKyQt(ttXml1.KY_QT))
                errors.Add("KY_QT phải 6 ký tự yyyyMM, tháng 01-12 (hiện tại: " + ttXml1.KY_QT + ")");

            if (string.IsNullOrEmpty(ttXml1.TRANGTHAI))
                errors.Add("Thiếu TRANGTHAI của hồ sơ (nhận giá trị 1 hoặc 2)");
            else if (ttXml1.TRANGTHAI != "1" && ttXml1.TRANGTHAI != "2")
                errors.Add("TRANGTHAI của hồ sơ chỉ nhận 1 hoặc 2 (hiện tại: " + ttXml1.TRANGTHAI + ")");

            bool inTimeOk = true, outTimeOk = true;
            if (!string.IsNullOrEmpty(ttXml1.NGAY_VAO) && !IsDate(ttXml1.NGAY_VAO, TIME_FORMAT_12))
            {
                errors.Add("NGAY_VAO phải 12 ký tự yyyyMMddHHmm (hiện tại: " + ttXml1.NGAY_VAO + ")");
                inTimeOk = false;
            }
            if (!string.IsNullOrEmpty(ttXml1.NGAY_RA) && !IsDate(ttXml1.NGAY_RA, TIME_FORMAT_12))
            {
                errors.Add("NGAY_RA phải 12 ký tự yyyyMMddHHmm (hiện tại: " + ttXml1.NGAY_RA + ")");
                outTimeOk = false;
            }

            // NGAY_RA phải >= NGAY_VAO (mã lỗi 124)
            if (inTimeOk && outTimeOk
                && !string.IsNullOrEmpty(ttXml1.NGAY_VAO) && !string.IsNullOrEmpty(ttXml1.NGAY_RA)
                && string.Compare(ttXml1.NGAY_RA, ttXml1.NGAY_VAO, StringComparison.Ordinal) < 0)
            {
                errors.Add("NGAY_RA nhỏ hơn NGAY_VAO");
            }
        }

        /// <summary>Mục 8, 9 - TT_DIEUCHINH (điều chỉnh hành chính và điều chỉnh/huỷ chi phí).</summary>
        private static void ValidateTtDieuChinh(XmlTTDieuChinh ttDieuChinh, List<string> errors)
        {
            int soDongHanhChinh = 0, soDongChiPhi = 0;

            if (ttDieuChinh != null)
            {
                if (ttDieuChinh.DS_XML1_DIEUCHINH != null && ttDieuChinh.DS_XML1_DIEUCHINH.Items != null)
                {
                    soDongHanhChinh = ttDieuChinh.DS_XML1_DIEUCHINH.Items.Count;
                    foreach (var dc in ttDieuChinh.DS_XML1_DIEUCHINH.Items)
                    {
                        if (dc == null) continue;
                        // Mục 8: LYDO_DIEUCHINH bắt buộc khi TT_DIEUCHINH có giá trị
                        if (!string.IsNullOrEmpty(dc.TT_DIEUCHINH) && string.IsNullOrEmpty(dc.LYDO_DIEUCHINH))
                            errors.Add(string.Format("Dòng điều chỉnh hành chính STT {0}: có TT_DIEUCHINH nhưng thiếu LYDO_DIEUCHINH", NullToEmpty(dc.STT)));
                    }
                }

                if (ttDieuChinh.DSCP_DIEUCHINH != null && ttDieuChinh.DSCP_DIEUCHINH.Items != null)
                {
                    soDongChiPhi = ttDieuChinh.DSCP_DIEUCHINH.Items.Count;
                    foreach (var cp in ttDieuChinh.DSCP_DIEUCHINH.Items)
                    {
                        if (cp == null) continue;
                        string stt = NullToEmpty(cp.STT);

                        // Mục 9: SOBANG_XML bắt buộc, chỉ nhận 2 hoặc 3
                        if (string.IsNullOrEmpty(cp.SOBANG_XML))
                            errors.Add(string.Format("Dòng chi phí STT {0}: thiếu SOBANG_XML (chỉ nhận 2 hoặc 3)", stt));
                        else if (cp.SOBANG_XML != "2" && cp.SOBANG_XML != "3")
                            errors.Add(string.Format("Dòng chi phí STT {0}: SOBANG_XML chỉ nhận 2 hoặc 3 (hiện tại: {1})", stt, cp.SOBANG_XML));

                        // Mục 9: TRANGTHAI bắt buộc, nhận 1 hoặc 2
                        if (string.IsNullOrEmpty(cp.TRANGTHAI))
                            errors.Add(string.Format("Dòng chi phí STT {0}: thiếu TRANGTHAI (nhận giá trị 1 hoặc 2)", stt));
                        else if (cp.TRANGTHAI != "1" && cp.TRANGTHAI != "2")
                            errors.Add(string.Format("Dòng chi phí STT {0}: TRANGTHAI chỉ nhận 1 hoặc 2 (hiện tại: {1})", stt, cp.TRANGTHAI));

                        // Mục 9: LYDO_DIEUCHINH bắt buộc khi có TRUONG_TT_DIEUCHINH hoặc TT_DIEUCHINH
                        if ((!string.IsNullOrEmpty(cp.TRUONG_TT_DIEUCHINH) || !string.IsNullOrEmpty(cp.TT_DIEUCHINH))
                            && string.IsNullOrEmpty(cp.LYDO_DIEUCHINH))
                            errors.Add(string.Format("Dòng chi phí STT {0}: có nội dung điều chỉnh nhưng thiếu LYDO_DIEUCHINH", stt));

                        if (!string.IsNullOrEmpty(cp.NGAY_YL) && !IsDate(cp.NGAY_YL, TIME_FORMAT_12))
                            errors.Add(string.Format("Dòng chi phí STT {0}: NGAY_YL phải 12 ký tự yyyyMMddHHmm (hiện tại: {1})", stt, cp.NGAY_YL));
                    }
                }
            }

            // Mã lỗi 204: hồ sơ không có nội dung điều chỉnh
            if (soDongHanhChinh == 0 && soDongChiPhi == 0)
                errors.Add("Hồ sơ không có nội dung điều chỉnh (cả DS_XML1_DIEUCHINH và DSCP_DIEUCHINH đều rỗng)");
        }

        /// <summary>KY_QT: 6 ký tự yyyyMM, tháng 01-12 (mục 7).</summary>
        private static bool IsKyQt(string value)
        {
            return IsDate(value, "yyyyMM");
        }

        private static bool IsDate(string value, string format)
        {
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Trim();
            if (value.Length != format.Length) return false;
            DateTime parsed;
            return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }

        private static string NullToEmpty(string s)
        {
            return s ?? "";
        }
    }
}
