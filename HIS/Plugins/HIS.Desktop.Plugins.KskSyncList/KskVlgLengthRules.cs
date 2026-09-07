/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>
    /// Kiem tra DO DAI CHUOI cua ban tin KHAMSUCKHOE theo cot "Kich thuoc toi da" trong Phu luc
    /// QD 2062/QD-BYT (07/07/2026, sua doi QD 1551) TRUOC khi day len Cong tiep nhan KDLYT Vinh Long.
    /// Bang duoi day trich tu ca 3 mau phieu (duoi 6 tuoi / 6-duoi 18 / 18 tuoi tro len); the nao xuat
    /// hien o nhieu mau voi gioi han khac nhau thi lay gia tri LON NHAT (tranh chan nham ho so hop le);
    /// the co kich thuoc "n" (khong gioi han: LY_DO_VV, SO_CCCD, KET_LUAN, CKS_*...) KHONG kiem tra.
    /// Kiem tra tren XML DA DUNG XONG (dung gia tri thuc su gui di, ke ca gia tri thu vien tu sinh).
    /// CHI ap dung nhanh VLG — cac cong khac giu nguyen.
    /// </summary>
    internal static class KskVlgLengthRules
    {
        private const int MAX_REPORT_ITEMS = 8;

        /// <summary>Ten the XML -> so ky tu toi da (theo QD 2062).</summary>
        private static readonly Dictionary<string, int> MaxLengths = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            { "BINH_THUONG", 1 },
            { "CAM_NHO_TUT_VE_SAU", 1 },
            { "CAN_NANG", 6 },
            { "CAN_NANG_TUOI_SD", 10 },
            { "CHAY_MU_NUOC_TAI", 1 },
            { "CHAY_NUOC_MUI", 1 },
            { "CHIEU_CAO", 10 },
            { "CHIEU_DAI", 10 },
            { "CHIEU_DAI_TUOI_SD", 10 },
            { "CHI_SO_BMI", 10 },
            { "CHUYEN_CSKCB", 1 },
            { "CHU_VI_VONG_CANH_TAY", 10 },
            { "CO_KINH_MAT_PHAI", 5 },
            { "CO_KINH_MAT_TRAI", 5 },
            { "CO_QUAN_SINH_DUC_NGOAI", 1 },
            { "DAP_UNG_AM_THANH", 1 },
            { "DGDD_BINH_THUONG", 1 },
            { "DGDD_COI_XUONG", 1 },
            { "DGDD_THIEU_MAU", 1 },
            { "DGDHST_MACH", 1 },
            { "DGDHST_NHIET_DO", 1 },
            { "DGDHST_NHIP_THO", 1 },
            { "DG_VONG_DAU", 1 },
            { "DIA_CHI", 1024 },
            { "DIEN_THOAI", 15 },
            { "DIEN_THOAI_NGH", 15 },
            { "DIEN_THOAI_NGUOI_DI_CUNG", 15 },
            { "DINH_THANG_LUOI", 1 },
            { "DOI_TUONG", 50 },
            { "DONG_TU", 1 },
            { "DON_VI_DO", 50 },
            { "GAN_LACH_TO", 1 },
            { "GIA_TRI", 255 },
            { "GIOI_TINH", 1 },
            { "HEN_KHAM_LAN_SAU", 1 },
            { "HINH_DANG_BUNG_RON", 1 },
            { "HINH_DANG_DAU", 1 },
            { "HINH_DANG_LUOI", 1 },
            { "HINH_DANG_MIENG", 1 },
            { "HINH_DANG_MUI", 1 },
            { "HONG", 1 },
            { "HO_TEN", 255 },
            { "HO_TEN_NGUOI_DI_CUNG", 255 },
            { "HUYET_AP", 100 },
            { "KET_LUAN_BENH", 255 },
            { "KHAM_DA_LIEU_PL", 1 },
            { "KHAM_MAT_PL", 1 },
            { "KHAM_NGOAI_KHOA_PL", 1 },
            { "KHAM_RANG_HAM_MAT_PL", 1 },
            { "KHAM_SAN_PHU_KHOA_PL", 1 },
            { "KHAM_TAI_MUI_HONG_PL", 1 },
            { "KHAM_THE_LUC_PL", 1 },
            { "KHOI_BAT_THUONG", 1 },
            { "KHOI_BAT_THUONG_DAU_CO", 1 },
            { "KHOI_SUNG_SAU_TAI", 1 },
            { "KHONG_KINH_MAT_PHAI", 5 },
            { "KHONG_KINH_MAT_TRAI", 5 },
            { "KHOP_HANG", 1 },
            { "KIEM_TRA_LUNG_COT_SONG", 1 },
            { "LAC_MAT", 1 },
            { "LONG_BAN_TAY", 1 },
            { "LO_HAU_MON", 1 },
            { "MACH", 100 },
            { "MACH_NGOAI_VI", 1 },
            { "MATINH_CU_TRU", 3 },
            { "MAU_SAC_DA", 1 },
            { "MAXA_CU_TRU", 5 },
            { "MA_BENH_SAN_KHOA_KHONG_BT", 255 },
            { "MA_CHI_SO", 255 },
            { "MA_CSKCB", 5 },
            { "MA_DAN_TOC", 2 },
            { "MA_DICH_VU", 50 },
            { "MA_GTIN_CSKCB", 13 },
            { "MA_LK", 100 },
            { "MA_LOAI_KCB", 2 },
            { "MA_NGHE_NGHIEP", 2 },
            { "MI_MAT_KET_MAC", 1 },
            { "MOI_QUAN_HE_VOI_TRE", 1 },
            { "NAM_MIENG", 1 },
            { "NGAYCAP_CCCD", 8 },
            { "NGAY_SINH", 12 },
            { "NGAY_VAO", 12 },
            { "NGHET_MUI", 1 },
            { "NGHE_PHOI", 1 },
            { "NGUOI_GIAM_HO", 255 },
            { "NGUON_CHI_TRA", 1 },
            { "NGUY_CO_MAC_LAO", 1 },
            { "NGUY_CO_TU_KY", 1 },
            { "NHIET_DO", 10 },
            { "NHIP_THO", 10 },
            { "NHIP_THO_KHONG_DEU", 1 },
            { "NHOM_MAU", 5 },
            { "NOICAP_CCCD", 1024 },
            { "NOI_KHOA_CO_XUONG_KHOP_PL", 1 },
            { "NOI_KHOA_HO_HAP_PL", 1 },
            { "NOI_KHOA_NOI_TIET_PL", 1 },
            { "NOI_KHOA_TAM_THAN_PL", 1 },
            { "NOI_KHOA_THAN_KINH_PL", 1 },
            { "NOI_KHOA_THAN_TN_SD_PL", 1 },
            { "NOI_KHOA_TIEU_HOA_PL", 1 },
            { "NOI_KHOA_TUAN_HOAN_PL", 1 },
            { "NOI_LAM_VIEC_HOC_TAP", 1024 },
            { "PHAN_LOAI_SK", 1 },
            { "PHAN_XA_BU", 1 },
            { "PHAN_XA_CO", 1 },
            { "PHAN_XA_MORO", 1 },
            { "PHAN_XA_NAM", 1 },
            { "PHU_DINH_DUONG", 1 },
            { "PT_TTBT_THEO_DO_TUOI", 1 },
            { "PT_VDBT_THEO_DO_TUOI", 1 },
            { "QUAN_SAT_DANG_DI", 1 },
            { "RANG_SUA_SO_SINH", 1 },
            { "SAN_KHOA", 1 },
            { "SAN_KHOA_KHONG_BT", 1 },
            { "SAU_MANG_BAM_LO", 1 },
            { "SINH_NON", 1 },
            { "SUY_DINH_DUONG", 1 },
            { "SUY_HO_HAP", 1 },
            { "TAI_MANG_NHI", 1 },
            { "TAI_PHAI_NOI_THAM", 10 },
            { "TAI_PHAI_NOI_THUONG", 10 },
            { "TAI_TRAI_NOI_THAM", 10 },
            { "TAI_TRAI_NOI_THUONG", 10 },
            { "THOP", 1 },
            { "THO_RUT_LOM_LONG_NGUC", 1 },
            { "THUA_CAN_BEO_PHI", 1 },
            { "TIEM_CHUNG_BAI_LIET", 2 },
            { "TIEM_CHUNG_BCG", 2 },
            { "TIEM_CHUNG_BCG_SS", 1 },
            { "TIEM_CHUNG_BH_HG_UV", 2 },
            { "TIEM_CHUNG_CAC_LOAI_KHAC", 1 },
            { "TIEM_CHUNG_DAY_DU_THEO_DO_TUOI", 1 },
            { "TIEM_CHUNG_SOI", 2 },
            { "TIEM_CHUNG_VGB", 2 },
            { "TIEM_CHUNG_VGB_SS_MUI1", 1 },
            { "TIEM_CHUNG_VNNB_B", 2 },
            { "TIENG_THO_BAT_THUONG", 1 },
            { "TIENG_TIM", 1 },
            { "TRUONG_LUC_CO", 1 },
            { "TSBT_BENH_COT_SONG", 1 },
            { "TSBT_BENH_KHAC", 1 },
            { "TSBT_BENH_MAT", 1 },
            { "TSBT_BENH_PHOI", 1 },
            { "TSBT_BENH_TAI", 1 },
            { "TSBT_BENH_TAM_THAN", 1 },
            { "TSBT_BENH_THAN", 1 },
            { "TSBT_BENH_THAN_KINH", 1 },
            { "TSBT_BENH_TIEU_HOA", 1 },
            { "TSBT_BENH_TIM", 1 },
            { "TSBT_BENH_TRONG_5_NAM_QUA", 1 },
            { "TSBT_DAI_THAO_DUONG", 1 },
            { "TSBT_DANG_DIEU_TRI_BENH", 1 },
            { "TSBT_KHO_THO", 1 },
            { "TSBT_MAC_BENH", 1 },
            { "TSBT_MAT_Y_THUC", 1 },
            { "TSBT_MA_BENH", 255 },
            { "TSBT_MA_BENH_KHAC", 255 },
            { "TSBT_MA_BENH_THAI_SAN", 255 },
            { "TSBT_MA_TUY", 1 },
            { "TSBT_NGAT", 1 },
            { "TSBT_NGHIEN_RUOU", 1 },
            { "TSBT_PHAU_THUAT_TIM", 1 },
            { "TSBT_ROI_LOAN_GIAC_NGU", 1 },
            { "TSBT_RUOU_THUONG_XUYEN", 1 },
            { "TSBT_TAI_BIEN", 1 },
            { "TSBT_TANG_HUYET_AP", 1 },
            { "TSBT_TEN_THUOC_LIEU_LUONG", 1024 },
            { "TSBT_TEN_THUOC_THAI_SAN", 1024 },
            { "TSBT_THAI_SAN", 1 },
            { "TSGD_MAC_BENH", 1 },
            { "TSGD_MA_BENH", 255 },
            { "TS_TIEP_XUC_LAO", 1 },
            { "TUAN_THAI", 2 },
            { "TU_CHI_KHOP", 1 },
            { "VAN_DE_SUC_KHOE", 1 },
            { "VAN_DONG_CO", 1 },
            { "VAN_DONG_KHONG_DOI_XUNG", 1 },
            { "VI_TRI_HAI_MAT", 1 },
            { "VI_TRI_MOM_TIM", 1 },
            { "VONG_DAU", 10 },
        };

        /// <summary>
        /// Duyet moi the la (khong co the con) trong ban tin; the nao co trong bang ma gia tri dai hon
        /// gioi han -> gom vao thong bao. NOIDUNGFILE base64 (neu co) duoc giai ma va kiem tra tiep ben trong.
        /// Tra null khi hop le / khong parse duoc (khong chan nham vi loi ky thuat cua chinh bo kiem tra).
        /// </summary>
        /// <summary>Thẻ BẮT BUỘC (theo cột "Bắt buộc" QĐ 2062) mà HIS hay bỏ trống, phải chặn -> tên hiển thị.</summary>
        private static readonly Dictionary<string, string> RequiredNonEmpty = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "DOI_TUONG", "Đối tượng" },
            { "NGUON_CHI_TRA", "Nguồn chi trả" },
        };

        /// <summary>
        /// Kiểm tra các thẻ BẮT BUỘC không được rỗng (Đối tượng, Nguồn chi trả — theo QĐ 2062). Thẻ vắng
        /// hoặc rỗng đều tính là thiếu. Tra null khi đủ; ngược lại trả thông báo để chặn không đẩy.
        /// (KSK lái xe hiện không nhập 2 trường này nhưng cổng vẫn nhận — phải chặn từ HIS.)
        /// </summary>
        internal static string ValidateRequired(string xml)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml)) return null;

                // Ban tin KSK 2062 chia nhieu khoi (XML1..XML12), moi khoi co the boc base64 trong
                // NOIDUNGFILE (cong tra loi o path THONG_TIN_CHUNG_VE_LAN_KHAM/DOI_TUONG, "Loai ho so XML2").
                // -> DUYET SAU + GIAI base64 moi thay the that su gui di (giong Validate do dai).
                var seen = new HashSet<string>(StringComparer.Ordinal);
                CollectNonEmptyNames(xml, seen, 0);
                var missing = new List<string>();
                foreach (var kv in RequiredNonEmpty)
                    if (!seen.Contains(kv.Key)) missing.Add(kv.Value + " (" + kv.Key + ")");
                if (missing.Count == 0) return null;
                return "VLG: KHÔNG đẩy hồ sơ — thiếu trường bắt buộc theo QĐ 2062: "
                    + string.Join(", ", missing.ToArray())
                    + " — bổ sung khi tiếp đón/nhập phiếu KSK rồi đẩy lại.";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("KskVlgLengthRules.ValidateRequired: bo qua (van day): " + ex.Message);
                return null;
            }
        }

        internal static string Validate(string xml)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml)) return null;
                var violations = new List<string>();
                CollectViolations(xml, violations, 0);
                if (violations.Count == 0) return null;

                var sb = new StringBuilder();
                sb.Append("VLG: KHÔNG đẩy hồ sơ — vượt độ dài theo QĐ 2062/QĐ-BYT: ");
                int shown = Math.Min(MAX_REPORT_ITEMS, violations.Count);
                for (int i = 0; i < shown; i++)
                {
                    if (i > 0) sb.Append("; ");
                    sb.Append(violations[i]);
                }
                if (violations.Count > shown)
                    sb.Append("; … và ").Append(violations.Count - shown).Append(" trường khác");
                sb.Append(" — rút ngắn dữ liệu trên HIS rồi đẩy lại.");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("KskVlgLengthRules: khong kiem tra duoc do dai (bo qua, van day): " + ex.Message);
                return null;
            }
        }

        private static void CollectViolations(string xml, List<string> violations, int depth)
        {
            if (depth > 2) return;   // chan de quy vo han (base64 long nhau bat thuong)
            var doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(xml);
            if (doc.DocumentElement == null) return;
            Walk(doc.DocumentElement, violations, depth);
        }

        private static void Walk(XmlElement el, List<string> violations, int depth)
        {
            bool hasChildElement = false;
            foreach (XmlNode child in el.ChildNodes)
            {
                var ce = child as XmlElement;
                if (ce == null) continue;
                hasChildElement = true;
                Walk(ce, violations, depth);
            }
            if (hasChildElement) return;

            string name = el.LocalName;
            string value = el.InnerText ?? "";
            if (value.Length == 0) return;

            // Khoi ho so ma hoa base64 (dang GIAMDINHHS/FILEHOSO) -> giai ma va kiem tra ben trong.
            if (string.Equals(name, "NOIDUNGFILE", StringComparison.Ordinal))
            {
                string inner = TryDecodeBase64Xml(value);
                if (inner != null)
                {
                    try { CollectViolations(inner, violations, depth + 1); }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }
                return;
            }

            int max;
            if (MaxLengths.TryGetValue(name, out max) && value.Length > max)
                violations.Add(name + " dài " + value.Length + " (tối đa " + max + ")");
        }

        /// <summary>
        /// Duyet DE QUY ban tin, thu thap ten cac the LA (leaf) co gia tri KHONG rong vao "names"
        /// (giai NOIDUNGFILE base64 de soi cac khoi XML1..XML12 ben trong). Dung cho ValidateRequired.
        /// </summary>
        private static void CollectNonEmptyNames(string xml, HashSet<string> names, int depth)
        {
            if (depth > 2 || string.IsNullOrWhiteSpace(xml)) return;
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(xml);
            if (doc.DocumentElement == null) return;
            WalkNames(doc.DocumentElement, names, depth);
        }

        private static void WalkNames(XmlElement el, HashSet<string> names, int depth)
        {
            bool hasChildElement = false;
            foreach (XmlNode child in el.ChildNodes)
            {
                XmlElement ce = child as XmlElement;
                if (ce == null) continue;
                hasChildElement = true;
                WalkNames(ce, names, depth);
            }
            if (hasChildElement) return;

            string name = el.LocalName;
            string value = el.InnerText ?? "";
            if (value.Length == 0) return;

            if (string.Equals(name, "NOIDUNGFILE", StringComparison.Ordinal))
            {
                string inner = TryDecodeBase64Xml(value);
                if (inner != null)
                {
                    try { CollectNonEmptyNames(inner, names, depth + 1); }
                    catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }
                return;
            }
            if (!string.IsNullOrWhiteSpace(value)) names.Add(name);
        }

        private static string TryDecodeBase64Xml(string value)
        {
            try
            {
                string s = value.Trim();
                if (s.Length < 8 || s.StartsWith("<")) return null;
                byte[] bytes = Convert.FromBase64String(s);
                string text = Encoding.UTF8.GetString(bytes).TrimStart((char)0xFEFF, (char)32, (char)13, (char)10, (char)9);
                return text.StartsWith("<") ? text : null;
            }
            catch { return null; }
        }
    }
}
