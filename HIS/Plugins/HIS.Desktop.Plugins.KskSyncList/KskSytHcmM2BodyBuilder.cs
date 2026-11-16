/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - message of form M2 (Mau 02, children from 6 to under 18) of the KSK portal of
 * So Y te TP.HCM: hin-api-service/create-mau-phieu-m2. Same token / signature / server as M3, M4.
 *
 * Sources:
 *   - HIS_KSK_UNDER_EIGHTEEN : vital-sign rank, specialty ranks, eyesight, hearing, guardian name +
 *                              relationship (D8), medicine in use, family history ids (D9)
 *   - HIS_KSK_SYT_HCM_M2     : everything else of Mau 02 (portal ids already)
 *   - HIS_PATIENT / HIS_TREATMENT / HIS_SERVICE_REQ / HIS_DHST : administration + vital signs (as M3)
 *
 * Answers of vaccination / B1 / B2 are stored as INTERNAL levels (see EnterKskInfomantionVer2
 * KskM2Definition) and converted here to the portal catalog ids by the item NAME.
 * Not sent (D7 / spec): noitiet_*, vaccination dates and the "other vaccine" row.
 */
using System;
using System.Collections.Generic;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json.Linq;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>Data of one under-18 record to push as M2.</summary>
    internal class KskSytHcmM2Source
    {
        public HIS_PATIENT Patient { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public HIS_SERVICE_REQ ServiceReq { get; set; }
        public HIS_KSK_UNDER_EIGHTEEN Under18 { get; set; }
        public HIS_KSK_GENERAL General { get; set; }
        public HIS_DHST Dhst { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> HisRanks { get; set; }
        public HIS_KSK_SYT_HCM_M2 M2 { get; set; }
    }

    internal static partial class KskSytHcmBodyBuilder
    {
        #region ===== M2 definitions (same order as the M2 spec) =====

        private const string CAT__M2_TINH_TRANG_TIEM = "Tinh_trang_tiem";
        private const string CAT__M2_ADHD = "RoiLoanGiamChuY_6_18_tuoi";
        private const string CAT__M2_ASD = "PhoTuKy_6_11_tuoi";
        private const string CAT__M2_QUAN_HE = "MoiQuanHeVoiTre";

        private static readonly string[] M2_VACCINE_FIELDS = new string[]
        {
            "tiem_lao_m1", "tiem_viemganb_m1", "tiem_viemganb_m2", "tiem_viemganb_m3", "tiem_viemganb_m4",
            "tiem_bachhau_uonvan_hoga_m1", "tiem_bachhau_uonvan_hoga_m2", "tiem_bachhau_uonvan_hoga_m3",
            "tiem_bachhau_uonvan_hoga_m4", "tiem_bailiet_l1", "tiem_bailiet_l2", "tiem_bailiet_l3",
            "tiem_bailiet_m1", "tiem_bailiet_m2", "tiem_hib_m1", "tiem_hib_m2", "tiem_hib_m3",
            "tiem_soi_m1", "tiem_soi_m2", "tiem_soi_rubella_m1", "tiem_viemnao_nhatban_m1",
            "tiem_viemnao_nhatban_m2", "tiem_viemnao_nhatban_m3"
        };

        private static readonly string[] M2_ADHD_FIELDS = new string[]
        {
            "adhd_thieu_can_than", "adhd_kho_duy_tri_chu_y", "adhd_khong_lang_nghe", "adhd_khong_theo_huong_dan",
            "adhd_kho_to_chuc", "adhd_ne_tranh_tap_trung", "adhd_danh_mat_do_dung", "adhd_de_bi_xao_lang",
            "adhd_hay_quen", "adhd_bon_chon_tay_chan", "adhd_roi_khoi_cho", "adhd_chay_nhay_lang_xang",
            "adhd_kho_choi_yen_lang", "adhd_ban_ron_hoat_dong", "adhd_noi_qua_nhieu", "adhd_noi_bat_cau_tra_loi",
            "adhd_kho_cho_doi", "adhd_ngat_loi_xen_ngang"
        };

        private static readonly string[] M2_ASD_FIELDS = new string[]
        {
            "asd_chu_y_am_thanh_nho", "asd_tap_trung_toan_canh", "asd_theo_doi_hoi_thoai",
            "asd_de_dang_thay_doi_hoat_dong", "asd_kho_duy_tri_tro_chuyen", "asd_gioi_tan_gau",
            "asd_kho_hieu_tam_ly_nhan_vat", "asd_thich_choi_gia_vo", "asd_the_hien_cam_xuc_mat",
            "asd_kho_khan_ket_ban"
        };

        /// <summary>Clinical section: field prefix of M2 -> column prefix of the M2 table -> rank column of the under-18 record.</summary>
        private static readonly string[][] M2_SECTIONS = new string[][]
        {
            new[] { "noikhoa", "EXAM_CIRCULATION", "EXAM_CIRCULATION_RANK", "Tuần hoàn" },
            new[] { "hohap", "EXAM_RESPIRATORY", "EXAM_RESPIRATORY_RANK", "Hô hấp" },
            new[] { "tieuhoa", "EXAM_DIGESTION", "EXAM_DIGESTION_RANK", "Tiêu hóa" },
            new[] { "thantietnieu", "EXAM_KIDNEY_URO", "EXAM_KIDNEY_UROLOGY_RANK", "Thận - Tiết niệu" },
            new[] { "thankinh", "EXAM_NEUROLOGICAL", "EXAM_NEURO_MENTAL_RANK", "Thần kinh" },
            new[] { "tamthan", "EXAM_MENTAL", "EXAM_MENTAL_RANK", "Tâm thần" },
            new[] { "mat", "EXAM_EYE", "EXAM_EYE_RANK", "Mắt" },
            new[] { "tmh", "EXAM_ENT", "EXAM_ENT_RANK", "Tai - Mũi - Họng" },
            new[] { "rhm", "EXAM_STOMATOLOGY", "EXAM_STOMATOLOGY_RANK", "Răng - Hàm - Mặt" }
        };

        private static readonly string[] M2_TEETH = new string[]
        {
            "18","17","16","15","14","13","12","11","21","22","23","24","25","26","27","28",
            "55","54","53","52","51","61","62","63","64","65",
            "85","84","83","82","81","71","72","73","74","75",
            "48","47","46","45","44","43","42","41","31","32","33","34","35","36","37","38"
        };

        #endregion

        #region ===== Build =====

        /// <summary>Builds the M2 message (null when there is no M2 record).</summary>
        internal static Dictionary<string, object> BuildM2(KskSytHcmM2Source src)
        {
            if (src == null || src.M2 == null || src.Under18 == null) return null;
            var body = new Dictionary<string, object>();
            body["tthc"] = BuildM2Tthc(src);
            body["tiem_chung"] = BuildM2TiemChung(src.M2);
            body["tien_su"] = BuildM2TienSu(src);
            body["kham_thuc_the"] = BuildM2KhamThucThe(src);
            body["adhd"] = BuildM2Adhd(src.M2);
            if (IsM2AsdAge(src)) body["asd"] = BuildM2Asd(src.M2);
            body["kham_lam_san"] = BuildM2KhamLamSan(src);
            body["can_lam_san"] = BuildM2CanLamSan(src.M2);
            body["ket_luan"] = BuildM2KetLuan(src.M2);
            return body;
        }

        private static Dictionary<string, object> BuildM2Tthc(KskSytHcmM2Source src)
        {
            var b = new Dictionary<string, object>();
            HIS_PATIENT p = src.Patient;
            HIS_KSK_SYT_HCM_M2 m = src.M2;
            HIS_KSK_UNDER_EIGHTEEN u = src.Under18;

            // Exam date = date of the KSK order (PTTK 4.1.3), fallback conclusion date.
            b["ngay_kham"] = ToDateString(FirstLong(src.ServiceReq, "INTRUCTION_TIME")
                                       ?? FirstLong(src.General, "CONCLUSION_TIME"));
            b["doi_tuong_kham"] = ReplaceSeparator(m.SYT_PATIENT_TYPES);
            b["dia_diem_kham"] = m.SYT_EXAM_PLACE_ID;
            b["dinh_danh_ca_nhan"] = FirstStr(p, "CCCD_NUMBER", "CMND_NUMBER") ?? "";
            b["ho_ten"] = FirstStr(p, "VIR_PATIENT_NAME", "PATIENT_NAME") ?? "";
            b["ngay_sinh"] = ToDateString(GetLong(p, "DOB"));
            b["gioi_tinh"] = MapByName(CAT__GIOI_TINH, GenderName(new KskSytHcmSource { Patient = p, Treatment = src.Treatment }));
            b["the_bhyt"] = GetStr(p, "TDL_HEIN_CARD_NUMBER") ?? "";
            b["dan_toc_id"] = MapByName(CAT__DAN_TOC, FirstStr(p, "ETHNIC_NAME", "VIR_ETHNIC_NAME"));
            b["sdt"] = GetStr(p, "PHONE") ?? "";
            b["nhom_mau_id"] = MapByName(CAT__NHOM_MAU, GetStr(p, "BLOOD_ABO_CODE"));
            b["yeu_to_nhom_mau_id"] = MapRhFactor(GetStr(p, "BLOOD_RH_CODE"));
            b["dia_chi_hien_tai"] = FirstStr(p, "VIR_HT_ADDRESS", "HT_ADDRESS", "VIR_ADDRESS", "ADDRESS") ?? "";

            KskSytCatalogItem city = MapCityItem(FirstStr(p, "HT_PROVINCE_CODE", "PROVINCE_CODE"),
                FirstStr(p, "HT_PROVINCE_NAME", "PROVINCE_NAME"));
            b["city_id"] = ToLongOrNull(city != null ? city.Id : null);
            KskSytCatalogItem ward = MapByNameItem(CAT__XA_PHUONG, FirstStr(p, "HT_COMMUNE_NAME", "COMMUNE_NAME"));
            b["ward_id"] = ToLongOrNull(ward != null ? ward.Id : null);
            b["ward_code"] = (ward != null) ? ward.Code : null;

            b["tre_co_di_hoc"] = (m.IS_GO_TO_SCHOOL == 1) ? 1 : 0;
            b["truong_id"] = (m.IS_GO_TO_SCHOOL == 1) ? m.SCHOOL_ID : null;
            b["ward_truonghoc"] = (m.IS_GO_TO_SCHOOL == 1) ? m.SCHOOL_WARD_ID : null;
            b["lop"] = (m.IS_GO_TO_SCHOOL == 1) ? (m.CLASS_NAME ?? "") : "";
            b["nguoi_giam_ho"] = u.ACCOMPANY_PERSON_NAME ?? "";
            b["cccd_nguoi_giam_ho"] = m.GUARDIAN_CCCD ?? "";
            b["moi_quan_he_voi_tre"] = MapM2Relationship(u.ACCOMPANY_RELATIONSHIP);
            b["sdt_nguoi_giam_ho"] = m.GUARDIAN_PHONE ?? "";
            b["hinh_thuc_chi_tra_khamsk"] = m.SYT_PAYSOURCE_ID;
            b["hinh_thuc_chi_tra_khamsk_chi_tiet"] = m.SYT_PAY_SOURCE_DETAIL_ID;
            b["nguonkhac_ghiro"] = m.PAY_SOURCE_OTHER ?? "";
            // Q6: spec table says "lydokham", sample message + Postman say "ly_do_kham" -> sample.
            b["ly_do_kham"] = GetStr(src.ServiceReq, "HOSPITALIZATION_REASON") ?? "";
            return b;
        }

        private static Dictionary<string, object> BuildM2TiemChung(HIS_KSK_SYT_HCM_M2 m)
        {
            var b = new Dictionary<string, object>();
            JObject o = ParseM2Json(m.VACCINATION_JSON);
            foreach (string f in M2_VACCINE_FIELDS)
            {
                JObject e = (o != null) ? o[f] as JObject : null;
                int level = (e != null) ? (e.Value<int?>("s") ?? 0) : 0;
                b[f] = MapM2Vaccine(level);
            }
            return b;
        }

        private static Dictionary<string, object> BuildM2TienSu(KskSytHcmM2Source src)
        {
            var b = new Dictionary<string, object>();
            HIS_KSK_SYT_HCM_M2 m = src.M2;
            HIS_KSK_UNDER_EIGHTEEN u = src.Under18;

            // D9: family history ids are in the old column of the under-18 record.
            string family = (m.FAMILY_HISTORY_HAS == 1) ? IdListOrNull(u.PATHOLOGICAL_HISTORY_FAMILY) : null;
            b["giadinh_macbenh"] = family ?? "";
            b["giadinh_danhsachbenh_icd"] = (m.FAMILY_HISTORY_HAS == 1)
                ? (IdListOrNull(GetStr(src.General, "FAMILY_HISTORY_ICD_CODE")) ?? "") : "";

            b["dieu_tri_benh_co_khong"] = (m.PERSONAL_HISTORY_HAS == 1) ? 1 : 0;
            b["tien_su_benh"] = (m.PERSONAL_HISTORY_HAS == 1) ? (m.PERSONAL_HISTORY_IDS ?? "") : "";
            b["benh_khac"] = (m.PERSONAL_HISTORY_HAS == 1) ? (m.PERSONAL_HISTORY_ICD ?? "") : "";

            string medicine = (m.IS_TREATING == 1) ? (u.MEDICINE_USING ?? "") : "";
            b["benh_dangdieutri"] = medicine;
            b["dieu_tri_benh_liet_ke"] = medicine;

            b["sankhoa_chuaphathienbatthuong"] = (m.OBSTETRIC_IS_NORMAL == 1) ? 1 : 0;
            b["sankhoa_ghichu"] = (m.OBSTETRIC_IS_NORMAL == 0) ? (m.OBSTETRIC_ABNORMAL_IDS ?? "") : "";
            b["mebibenh"] = m.MOTHER_DISEASE ?? "";
            return b;
        }

        private static Dictionary<string, object> BuildM2KhamThucThe(KskSytHcmM2Source src)
        {
            HIS_DHST d = src.Dhst;
            var tl = new Dictionary<string, object>();
            tl["chieucao"] = Num(GetDecimal(d, "HEIGHT"));
            tl["cannang"] = Num(GetDecimal(d, "WEIGHT"));
            tl["mach"] = Num(GetDecimal(d, "PULSE"));
            tl["huyetaptamthu"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MAX"));
            tl["huyetaptamtruong"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MIN"));
            tl["nhiptho"] = Num(GetDecimal(d, "BREATH_RATE"));

            var b = new Dictionary<string, object>();
            b["kham_the_luc"] = tl;
            b["phanloai"] = KskSytHcmPayload.ResolveSytRankId(GetLong(src.Under18, "DHST_RANK"), src.HisRanks);
            return b;
        }

        private static Dictionary<string, object> BuildM2Adhd(HIS_KSK_SYT_HCM_M2 m)
        {
            var b = new Dictionary<string, object>();
            b["adhd_ngay_danh_gia"] = ToDateString(m.ADHD_DATE);
            JObject o = ParseM2Json(m.ADHD_JSON);
            foreach (string f in M2_ADHD_FIELDS)
                b[f] = MapM2Adhd((o != null) ? (o.Value<int?>(f) ?? 0) : 0);
            b["adhd_ket_qua"] = m.ADHD_RESULT ?? "";
            b["adhd_ghi_chu"] = m.ADHD_NOTE ?? "";
            return b;
        }

        private static Dictionary<string, object> BuildM2Asd(HIS_KSK_SYT_HCM_M2 m)
        {
            var b = new Dictionary<string, object>();
            b["asd_ngay_danh_gia"] = ToDateString(m.ASD_DATE);
            JObject o = ParseM2Json(m.ASD_JSON);
            foreach (string f in M2_ASD_FIELDS)
                b[f] = MapM2Asd((o != null) ? (o.Value<int?>(f) ?? 0) : 0);
            b["asd_ket_qua"] = m.ASD_RESULT ?? "";
            b["asd_ghi_chu"] = m.ASD_NOTE ?? "";
            return b;
        }

        private static Dictionary<string, object> BuildM2KhamLamSan(KskSytHcmM2Source src)
        {
            var b = new Dictionary<string, object>();
            HIS_KSK_SYT_HCM_M2 m = src.M2;
            HIS_KSK_UNDER_EIGHTEEN u = src.Under18;

            foreach (string[] s in M2_SECTIONS)
            {
                AddM2Diagnosis(b, s[0], m, s[1]);
                long? hisRank = GetLong(u, s[2]);
                if (s[0] == "thankinh" && !hisRank.HasValue) hisRank = GetLong(u, "EXAM_NEUROLOGICAL_RANK");
                b[s[0] + "_phanloai"] = KskSytHcmPayload.ResolveSytRankId(hisRank, src.HisRanks);

                if (s[0] == "mat")
                {
                    b["mat_khongkinh_mp"] = GetStr(u, "EXAM_EYESIGHT_RIGHT") ?? "";
                    b["mat_khongkinh_mt"] = GetStr(u, "EXAM_EYESIGHT_LEFT") ?? "";
                    b["mat_cokinh_mp"] = GetStr(u, "EXAM_EYESIGHT_GLASS_RIGHT") ?? "";
                    b["mat_cokinh_mt"] = GetStr(u, "EXAM_EYESIGHT_GLASS_LEFT") ?? "";
                }
                else if (s[0] == "tmh")
                {
                    b["tmh_taitrai_noithuong"] = GetStr(u, "EXAM_ENT_LEFT_NORMAL") ?? "";
                    b["tmh_taiphai_noithuong"] = GetStr(u, "EXAM_ENT_RIGHT_NORMAL") ?? "";
                    b["tmh_taitrai_noitham"] = GetStr(u, "EXAM_ENT_LEFT_WHISPER") ?? "";
                    b["tmh_taiphai_noitham"] = GetStr(u, "EXAM_ENT_RIGHT_WHISPER") ?? "";
                }
            }

            // Teeth: the M2 table already stores portal ids -> send as is, only teeth with a status.
            var teeth = new Dictionary<string, object>();
            foreach (string no in M2_TEETH)
            {
                long? v = GetLong(m, "TOOTH_" + no);
                if (v.HasValue && v.Value > 0) teeth[no] = v.Value;
            }
            b["chi_tiet_kham_rang"] = teeth;

            b["CotSong"] = m.SPINE_ID;
            b["CongCotSong"] = m.SPINE_KYPHOSIS_ID;
            b["VeoCotSong"] = m.SPINE_SCOLIOSIS_ID;
            b["MucDoVeoCotSong"] = m.SPINE_SCOLIOSIS_LEVEL_ID;
            AddM2Diagnosis(b, "cotsong_clskhac", m, "EXAM_OTHER");
            b["khamcan_lamsang_khac"] = u.EXAM_CLINICAL_OTHER ?? "";
            return b;
        }

        private static void AddM2Diagnosis(Dictionary<string, object> b, string field, HIS_KSK_SYT_HCM_M2 m, string prefix)
        {
            long? isNormal = GetLong(m, prefix + "_IS_NORMAL");
            b[field + "_chuaphathienbatthuong"] = (isNormal == 1) ? 1 : 0;
            b[field + "_chandoansobo_icd"] = GetStr(m, prefix + "_PRE_ICD_CODE") ?? "";
            b[field + "_chandoanxacdinh_icd"] = GetStr(m, prefix + "_ICD_CODE") ?? "";
        }

        private static Dictionary<string, object> BuildM2CanLamSan(HIS_KSK_SYT_HCM_M2 m)
        {
            var b = new Dictionary<string, object>();
            b["ngay_xetnghiem"] = ToDateString(m.TEST_DATE);
            b["donvi_xetnghiem"] = m.TEST_UNIT_NAME ?? "";
            b["kq_xetnghiem"] = m.TEST_RESULT ?? "";
            b["kq_cdha"] = m.IMAGING_RESULT ?? "";
            return b;
        }

        private static Dictionary<string, object> BuildM2KetLuan(HIS_KSK_SYT_HCM_M2 m)
        {
            var b = new Dictionary<string, object>();
            b["de_nghi"] = m.SUGGEST_OTHER ?? "";
            b["danh_muc_de_nghi"] = m.SYT_SUGGEST_ID;
            return b;
        }

        #endregion

        #region ===== Required fields (R19) =====

        /// <summary>Lists every missing required field of the M2 message in one pass.</summary>
        internal static List<string> DescribeMissingRequiredM2(Dictionary<string, object> body)
        {
            var missing = new List<string>();
            try
            {
                if (body == null) { missing.Add("hồ sơ chưa nhập dữ liệu Mẫu 02"); return missing; }
                var t = body["tthc"] as Dictionary<string, object>;
                RequireM2(t, "ngay_kham", "Ngày khám", missing);
                RequireM2(t, "doi_tuong_kham", "Đối tượng khám (tab Mẫu 02)", missing);
                RequireM2(t, "dia_diem_kham", "Địa điểm khám (tab Mẫu 02)", missing);
                RequireM2(t, "ho_ten", "Họ tên", missing);
                RequireM2(t, "ngay_sinh", "Ngày sinh", missing);
                RequireM2(t, "gioi_tinh", "Giới tính", missing);
                RequireM2(t, "sdt", "Số điện thoại của trẻ", missing);
                RequireM2(t, "dia_chi_hien_tai", "Chỗ ở hiện tại", missing);
                RequireM2(t, "city_id", "Tỉnh/Thành phố (phải khớp danh mục của cổng)", missing);
                RequireM2(t, "ward_id", "Xã/Phường (phải khớp danh mục của cổng)", missing);
                RequireM2(t, "ward_code", "Mã Xã/Phường", missing);
                if (Convert.ToInt32(t["tre_co_di_hoc"] ?? 0) == 1)
                {
                    RequireM2(t, "truong_id", "Trường", missing);
                    RequireM2(t, "ward_truonghoc", "Xã/Phường của trường", missing);
                    RequireM2(t, "lop", "Lớp", missing);
                }
                RequireM2(t, "nguoi_giam_ho", "Họ tên mẹ hoặc người giám hộ", missing);
                RequireM2(t, "moi_quan_he_voi_tre", "Mối quan hệ với trẻ", missing);
                RequireM2(t, "sdt_nguoi_giam_ho", "Điện thoại người giám hộ", missing);
                RequireM2(t, "hinh_thuc_chi_tra_khamsk", "Hình thức chi trả (tab Mẫu 02)", missing);
                RequireM2(t, "hinh_thuc_chi_tra_khamsk_chi_tiet", "Hình thức chi trả chi tiết (tab Mẫu 02)", missing);

                var tt = body["kham_thuc_the"] as Dictionary<string, object>;
                RequireM2(tt, "phanloai", "Phân loại thể lực", missing);

                var ls = body["kham_lam_san"] as Dictionary<string, object>;
                var noRank = new List<string>();
                foreach (string[] s in M2_SECTIONS)
                {
                    object v;
                    if (ls == null || !ls.TryGetValue(s[0] + "_phanloai", out v) || v == null) noRank.Add(s[3]);
                }
                if (noRank.Count > 0)
                    missing.Add("Phân loại của " + noRank.Count + " chuyên khoa (" + string.Join(", ", noRank.ToArray()) + ")");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return missing;
        }

        private static void RequireM2(Dictionary<string, object> block, string field, string label, List<string> missing)
        {
            object v;
            if (block == null || !block.TryGetValue(field, out v) || v == null
                || (v is string && string.IsNullOrWhiteSpace((string)v)))
                missing.Add(label);
        }

        #endregion

        #region ===== Helpers =====

        /// <summary>B2 is sent only for children 6..11 years old at the exam date (R12).</summary>
        private static bool IsM2AsdAge(KskSytHcmM2Source src)
        {
            try
            {
                long? dob = GetLong(src.Patient, "DOB");
                long? exam = FirstLong(src.ServiceReq, "INTRUCTION_TIME");
                if (!dob.HasValue || dob.Value <= 0) return false;
                DateTime? d = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob.Value);
                DateTime? e = exam.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(exam.Value) : null;
                if (!d.HasValue) return false;
                DateTime at = e ?? DateTime.Now;
                int age = at.Year - d.Value.Year;
                if (at.Month < d.Value.Month || (at.Month == d.Value.Month && at.Day < d.Value.Day)) age--;
                return age >= 6 && age <= 11;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }

        private static JObject ParseM2Json(string json)
        {
            try { return string.IsNullOrWhiteSpace(json) ? null : JObject.Parse(json); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn("SytHcm/M2: JSON khong doc duoc", ex); return null; }
        }

        private static string IdListOrNull(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            string t = s.Trim().Replace(';', ',').Replace(" ", "");
            foreach (char c in t)
            {
                if (!char.IsDigit(c) && c != ',') return null;
            }
            return t;
        }

        /// <summary>Lower case without Vietnamese accents.</summary>
        private static string NormNoAccent(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            string t = s.Trim().ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();
            foreach (char c in t)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sb.Append(c == 'đ' ? 'd' : c);
            }
            return sb.ToString();
        }

        /// <summary>Id of the first catalog item whose accent-free name satisfies the test.</summary>
        private static long? FindM2CatalogId(string catalogCode, Func<string, bool> test)
        {
            var items = KskSytHcmPayload.ReadCachedCatalog(catalogCode);
            if (items == null || items.Count == 0)
            {
                Inventec.Common.Logging.LogSystem.Warn("SytHcm/M2: chua co danh muc " + catalogCode + " tai may -> gui trong");
                return null;
            }
            foreach (var it in items)
            {
                if (it == null) continue;
                if (test(NormNoAccent(it.Name))) return ToLongOrNull(it.Id);
            }
            Inventec.Common.Logging.LogSystem.Warn("SytHcm/M2: khong nhan dien duoc muc can tim trong danh muc " + catalogCode);
            return null;
        }

        /// <summary>1 = Da tiem, 2 = Chua tiem, 3 = Khong nho ro.</summary>
        private static long? MapM2Vaccine(int level)
        {
            switch (level)
            {
                case 1: return FindM2CatalogId(CAT__M2_TINH_TRANG_TIEM, n => n.Contains("da tiem") || n.Contains("da uong"));
                case 2: return FindM2CatalogId(CAT__M2_TINH_TRANG_TIEM, n => n.Contains("chua tiem") || n.Contains("chua uong"));
                case 3: return FindM2CatalogId(CAT__M2_TINH_TRANG_TIEM, n => n.Contains("khong nho"));
                default: return null;
            }
        }

        /// <summary>1 = Khong co, 2 = Thinh thoang, 3 = Thuong xuyen.</summary>
        private static long? MapM2Adhd(int level)
        {
            switch (level)
            {
                case 1: return FindM2CatalogId(CAT__M2_ADHD, n => n.Contains("khong") && !n.Contains("thinh") && !n.Contains("thuong"));
                case 2: return FindM2CatalogId(CAT__M2_ADHD, n => n.Contains("thinh thoang"));
                case 3: return FindM2CatalogId(CAT__M2_ADHD, n => n.Contains("thuong xuyen"));
                default: return null;
            }
        }

        /// <summary>1 = Hoan toan dong y, 2 = Co chut dong y, 3 = Co chut khong dong y, 4 = Hoan toan khong dong y.</summary>
        private static long? MapM2Asd(int level)
        {
            switch (level)
            {
                case 1: return FindM2CatalogId(CAT__M2_ASD, n => n.Contains("hoan toan") && !n.Contains("khong"));
                case 2: return FindM2CatalogId(CAT__M2_ASD, n => n.Contains("chut") && !n.Contains("khong"));
                case 3: return FindM2CatalogId(CAT__M2_ASD, n => n.Contains("chut") && n.Contains("khong"));
                case 4: return FindM2CatalogId(CAT__M2_ASD, n => n.Contains("hoan toan") && n.Contains("khong"));
                default: return null;
            }
        }

        /// <summary>D8: relationship of the under-18 record (1 Cha .. 6 Khac) -> catalog MoiQuanHeVoiTre by name.</summary>
        private static long? MapM2Relationship(long? rel)
        {
            if (!rel.HasValue) return null;
            switch (rel.Value)
            {
                case 1: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n == "cha" || n.StartsWith("cha ") || n.Contains("bo"));
                case 2: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n == "me" || n.StartsWith("me "));
                case 3: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n.StartsWith("ong"));
                case 4: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n.Contains("anh") || n.Contains("chi"));
                case 5: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n.Contains("ho hang"));
                case 6: return FindM2CatalogId(CAT__M2_QUAN_HE, n => n.Contains("khac"));
                default: return null;
            }
        }

        #endregion
    }
}
