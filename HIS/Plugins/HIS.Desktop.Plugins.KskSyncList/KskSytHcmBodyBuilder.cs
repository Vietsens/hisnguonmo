/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * Dựng TOÀN BỘ bản tin mẫu M3 gửi Nền tảng KSK Sở Y tế TP.HCM — 6 khối:
 *      tthc · tien_su · kham_the_luc · kham_lam_san · can_lam_san · ket_luan
 *
 * TÊN TRƯỜNG lấy theo VÍ DỤ BODY (mục 2.2 của đặc tả), KHÔNG theo bảng chi tiết (mục 2.3).
 * Hai mục này đặt tên khác nhau ở 10 trường (doi_tuong_kham/doituongkham, ward_id/wardId,
 * chieucao/theluc_chieucao, de_nghi/KetLuan_DeNghi...). Chữ ký băm trên chính bản tin nên phải
 * chọn một bộ; ví dụ body là bản Sở đã chạy thật nên tin bộ đó hơn.
 *
 * QUY ĐỔI DANH MỤC: trường nào cổng có danh mục thì gửi **Id của cổng**, không gửi mã của HIS.
 * Ba cách quy đổi, xem vùng "Quy đổi danh mục":
 *      1. Đã là Id của cổng   -> gửi thẳng (đối tượng khám, nguồn chi trả, địa điểm, tình trạng răng)
 *      2. Theo cấp độ         -> phân loại sức khỏe I–V
 *      3. Theo tên            -> giới tính, dân tộc, nhóm máu, xã/phường, nghề nghiệp
 *      4. Theo mã ICD         -> tách mã ở đầu tên mục trong danh mục ICD của cổng
 *
 * TIỀN SỬ BỆNH BẢN THÂN: 21 câu hỏi lấy từ bảng câu trả lời tiền sử (HIS_PERIOD_DRIVER_DITY),
 * mỗi bệnh một dòng với IS_YES_NO = "1"/"0" — đúng dạng 1-0 mà cổng nhận. Ghép câu hỏi với danh
 * mục bệnh của HIS theo TỪ KHÓA TRONG TÊN, xem vùng "Tiền sử".
 */
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace HIS.Desktop.Plugins.KskSyncList
{
    /// <summary>Dữ liệu nguồn của một hồ sơ — nơi gọi đổ vào từ những gì đã tải sẵn.</summary>
    internal class KskSytHcmSource
    {
        public HIS_PATIENT Patient { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public HIS_SERVICE_REQ ServiceReq { get; set; }
        public HIS_KSK_OVER_EIGHTEEN Over18 { get; set; }
        public HIS_KSK_GENERAL General { get; set; }
        public HIS_DHST Dhst { get; set; }
        /// <summary>Bảng dữ liệu riêng của mẫu M3 (có thể null nếu hồ sơ chưa nhập tab HCM).</summary>
        public HIS_KSK_SYT_HCM SytHcm { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> HisRanks { get; set; }

        /// <summary>
        /// Câu trả lời tiền sử bệnh của hồ sơ — mỗi bệnh một dòng, `IS_YES_NO` = "1" (có) / "0" (không).
        /// Lọc sẵn theo hồ sơ đang đẩy (`KSK_OVER_EIGHTEEN_ID`).
        /// </summary>
        public List<HIS_PERIOD_DRIVER_DITY> Ditys { get; set; }

        /// <summary>Danh mục bệnh của HIS — để biết mỗi dòng trả lời ứng với bệnh nào.</summary>
        public List<HIS_DISEASE_TYPE> DiseaseTypes { get; set; }

        /// <summary>
        /// Kết quả chỉ số xét nghiệm của CHÍNH đợt điều trị này — nguồn của khối cận lâm sàng.
        /// Lọc sẵn theo đợt điều trị để không lấy lẫn kết quả của bệnh nhân khác trong cùng lượt đẩy.
        /// </summary>
        public List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN> ClsTeins { get; set; }

        /// <summary>
        /// Bảng khai báo nối chỉ số cận lâm sàng đã lưu ở màn hình đồng bộ (dạng JSON).
        /// Rỗng = viện chưa khai báo -> khối cận lâm sàng gửi rỗng.
        /// </summary>
        public string ClsMapJson { get; set; }
    }

    internal static class KskSytHcmBodyBuilder
    {
        #region ===== Mã danh mục của cổng =====

        private const string CAT__GIOI_TINH = "GioiTinh";
        private const string CAT__DAN_TOC = "DanToc";
        private const string CAT__NHOM_MAU = "NhomMau";
        private const string CAT__YEU_TO_NHOM_MAU = "YeuToNhomMau";
        private const string CAT__XA_PHUONG = "DiaChiHienTai_XaPhuong";
        private const string CAT__NGHE_NGHIEP = "NgheNghiepId";
        private const string CAT__NOI_CONG_TAC = "NoiCongTacHocTap";
        private const string CAT__KET_LUAN_DE_NGHI = "KetLuan_DeNghi";
        private const string CAT__ICD = "ICD";
        private const string CAT__YES_NO = "Yes_No";
        private const string CAT__YES_NO_M4 = "YesNo";

        /// <summary>
        /// Các câu hỏi có/không gửi 1/0 hay gửi Id danh mục (264 = Có, 265 = Không)?
        ///
        /// Đặc tả tự nói hai kiểu: bảng chi tiết ghi "Danh mục có không", còn ví dụ body gửi `1`.
        /// Đang theo ví dụ body. Sở xác nhận là Id thì đổi cờ này thành true, không phải sửa chỗ khác.
        /// </summary>
        private const bool YES_NO_AS_CATALOG_ID = false;

        #endregion

        #region ===== Dựng bản tin =====

        /// <summary>
        /// Dựng bản tin đầy đủ 6 khối.
        ///
        /// `useFakeParaclinical` = true thì RIÊNG khối **cận lâm sàng** dùng dữ liệu giả, vì phần
        /// đọc theo bảng nối 34 chỉ số chưa gắn. Năm khối còn lại LUÔN lấy dữ liệu thật.
        ///
        /// Đặt false khi gắn xong khối cận lâm sàng, rồi xóa KskSytHcmFakeData.
        /// </summary>
        internal static object Build(KskSytHcmSource src, bool useFakeParaclinical)
        {
            if (src == null) return null;
            var body = new
            {
                tthc = BuildTthc(src),
                tien_su = BuildTienSu(src),
                kham_the_luc = BuildKhamTheLuc(src),
                kham_lam_san = BuildKhamLamSan(src),
                can_lam_san = useFakeParaclinical
                    ? KskSytHcmFakeData.BuildCanLamSan()
                    : BuildCanLamSan(src),
                ket_luan = BuildKetLuan(src)
            };


            // Gửi kèm tên của BẢNG CHI TIẾT bên cạnh tên của VÍ DỤ BODY — xem AddAliases.
            AddAliases(body.tthc, TTHC_ALIASES);
            AddAliases(body.kham_the_luc, THE_LUC_ALIASES);
            AddAliases(body.tien_su, TIEN_SU_ALIASES);
            AddAliases(body.kham_lam_san, LAM_SAN_ALIASES);
            AddAliases(body.ket_luan, KET_LUAN_ALIASES);
            return body;
        }

        /// <summary>I. Thông tin hành chính — 23 chỉ tiêu.</summary>
        private static Dictionary<string, object> BuildTthc(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            HIS_PATIENT p = src.Patient;
            HIS_KSK_OVER_EIGHTEEN o = src.Over18;
            HIS_KSK_SYT_HCM h = src.SytHcm;

            b["ngay_kham"] = ToDateString(FirstLong(src.General, "CONCLUSION_TIME")
                                       ?? FirstLong(src.Treatment, "IN_TIME"));

            // Đối tượng khám và nguồn chi trả: bảng riêng của mẫu M3 đã lưu Id CỦA CỔNG -> gửi thẳng.
            // Đặc tả nhận danh sách cách nhau bằng dấu phẩy, HIS lưu nối bằng ";" -> chỉ đổi dấu.
            b["doi_tuong_kham"] = ReplaceSeparator(GetStr(h, "SYT_PATIENT_TYPES"));
            b["dia_diem_kham"] = GetLong(h, "SYT_EXAM_PLACE_ID");

            b["dinh_danh_ca_nhan"] = FirstStr(p, "CCCD_NUMBER", "CMND_NUMBER");
            b["ho_ten"] = FirstStr(p, "VIR_PATIENT_NAME", "PATIENT_NAME");
            b["ngay_sinh"] = ToDateString(GetLong(p, "DOB"));
            b["gioi_tinh"] = MapByName(CAT__GIOI_TINH, GenderName(src));
            b["the_bhyt"] = GetStr(p, "TDL_HEIN_CARD_NUMBER");
            // DÂN TỘC nằm ở ETHNIC_NAME. NATIONAL_NAME là QUỐC TỊCH ("Việt Nam") — lấy cột đó thì
            // không bao giờ khớp danh mục dân tộc (Kinh, Tày, Thái...).
            b["dan_toc_id"] = MapByName(CAT__DAN_TOC, FirstStr(p, "ETHNIC_NAME", "VIR_ETHNIC_NAME"));
            b["sdt"] = GetStr(p, "PHONE");
            b["nhom_mau_id"] = MapByName(CAT__NHOM_MAU, GetStr(p, "BLOOD_ABO_CODE"));
            b["yeu_to_nhom_mau_id"] = MapByName(CAT__YEU_TO_NHOM_MAU, GetStr(p, "BLOOD_RH_CODE"));
            b["dia_chi_hien_tai"] = FirstStr(p, "VIR_HT_ADDRESS", "HT_ADDRESS", "VIR_ADDRESS", "ADDRESS");

            // Id VÀ MÃ phải lấy CÙNG MỘT MỤC trong danh mục của Sở. Lấy mã của HIS thì cổng báo
            // "NgheNghiepCode không tồn tại trong danh mục nghề nghiệp" — hai hệ mã khác nhau.
            KskSytCatalogItem ward = MapByNameItem(CAT__XA_PHUONG,
                FirstStr(p, "HT_COMMUNE_NAME", "COMMUNE_NAME"));
            b["ward_id"] = ToLongOrNull(ward != null ? ward.Id : null);
            b["ward_code"] = (ward != null) ? ward.Code : null;

            KskSytCatalogItem career = MapByNameItem(CAT__NGHE_NGHIEP, GetStr(p, "CAREER_NAME"));
            if (career != null)
            {
                b["nghenghiep_id"] = ToLongOrNull(career.Id);
                b["nghenghiep_code"] = career.Code;
            }
            else
            {
                // TẠM THỜI, ĐANG CHỜ SỞ TRẢ LỜI — danh mục nghề nghiệp của cổng có 792 mục theo
                // phân loại khác hẳn cách HIS đặt tên ("Nông dân" không có trong danh sách) nên
                // chưa nối được, mà cổng thì bắt buộc trường này. Gửi tạm một mục CÓ THẬT.
                b["nghenghiep_id"] = TAM_NGHE_NGHIEP_ID;
                b["nghenghiep_code"] = TAM_NGHE_NGHIEP_CODE;
            }

            // NƠI CÔNG TÁC là Id trong danh mục của cổng, KHÔNG phải chữ. Bảng chi tiết ghi kiểu
            // chuỗi 2000 ký tự nhưng thực tế cổng chỉ nhận Id — gửi chữ là bị từ chối.
            long? noiCongTac = MapByName(CAT__NOI_CONG_TAC,
                FirstStr(o, "WORK_PLACE", "WORKING_PLACE"));
            // TẠM THỜI, CÙNG LOẠI VẤN ĐỀ VỚI NGHỀ NGHIỆP — danh mục nơi công tác của cổng có 5431
            // mục là tên cơ sở cụ thể, HIS không có dữ liệu tương ứng để nối, mà cổng bắt buộc.
            b["noi_cong_tac"] = noiCongTac.HasValue ? (object)noiCongTac.Value : TAM_NOI_CONG_TAC_ID;
            b["noi_cong_tac_xa_phuong"] = TAM_NOI_CONG_TAC_XA_PHUONG;

            b["hinh_thuc_chi_tra_khamsk"] = GetLong(h, "SYT_PAYSOURCE_ID");
            b["hinh_thuc_chi_tra_khamsk_chi_tiet"] = GetLong(h, "SYT_PAY_SOURCE_DETAIL_ID");
            b["nguonkhac_ghiro"] = GetStr(h, "PAY_SOURCE_OTHER");
            b["ly_do_kham"] = GetStr(src.ServiceReq, "HOSPITALIZATION_REASON");
            return b;
        }

        /// <summary>II. Tiền sử.</summary>
        private static Dictionary<string, object> BuildTienSu(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            HIS_KSK_OVER_EIGHTEEN o = src.Over18;

            string family = GetStr(o, "PATHOLOGICAL_HISTORY_FAMILY");
            b["giadinh_macbenh"] = YesNo(!string.IsNullOrWhiteSpace(family));
            b["giadinh_danhsachbenh"] = null;       // HIS lưu dạng chữ, không có danh sách mã bệnh
            b["giadinh_danhsachbenh_icd"] = null;
            b["giadinh_macbenh_tenbenh"] = family;

            b["ds_benh_ban_than"] = BuildDsBenhBanThan(src);

            string treating = GetStr(o, "PATHOLOGICAL_HISTORY");
            b["dieu_tri_benh_co_khong"] = YesNo(!string.IsNullOrWhiteSpace(treating));
            b["dieu_tri_benh_liet_ke"] = treating;

            string maternity = GetStr(o, "MATERNITY_HISTORY");
            b["thai_san_co_khong"] = YesNo(!string.IsNullOrWhiteSpace(maternity));
            b["thai_san_liet_ke"] = maternity;
            return b;
        }

        /// <summary>Một câu hỏi tiền sử: khóa trường của cổng và từ khóa nhận biết trong danh mục HIS.</summary>
        private class DiseaseQuestion
        {
            public string Field { get; set; }
            public string[] Must { get; set; }
            public string[] Not { get; set; }
        }

        /// <summary>
        /// 21 câu hỏi tiền sử bệnh, tật của bản thân.
        ///
        /// NGUỒN DỮ LIỆU: bảng câu trả lời tiền sử (`HIS_PERIOD_DRIVER_DITY`) — mỗi bệnh một dòng,
        /// `IS_YES_NO` = "1" (có) / "0" (không) / trống (chưa trả lời). Đúng dạng 1-0 mà cổng nhận.
        ///
        /// GHÉP CÂU HỎI VỚI DANH MỤC BỆNH CỦA HIS theo TỪ KHÓA TRONG TÊN, không theo mã và không
        /// theo thứ tự dòng: mã do từng viện tự đặt, còn thứ tự đổi khi viện thêm/bớt bệnh.
        /// Từ khóa GIỮ NGUYÊN DẤU — bỏ dấu thì "thận" và "thần kinh" trùng nhau ("than"), ghép sai.
        /// </summary>
        private static readonly List<DiseaseQuestion> DS_BENH_BAN_THAN = new List<DiseaseQuestion>
        {
            Q("benh_5nam",              new[] { "5 năm" }),
            Q("benh_than_kinh",         new[] { "thần kinh" }),
            Q("benh_mat",               new[] { "mắt" }),
            Q("benh_tai",               new[] { "tai" }, new[] { "tai biến" }),
            Q("benh_tim",               new[] { "tim" }, new[] { "phẫu thuật" }),
            Q("pt_tim_mach",            new[] { "phẫu thuật", "tim" }),
            Q("tang_ha",                new[] { "huyết áp" }),
            Q("kho_tho",                new[] { "khó thở" }),
            Q("benh_phoi",              new[] { "phổi" }),
            Q("benh_than",              new[] { "thận" }),
            Q("nghien_ruou_bia",        new[] { "nghiện" }),
            Q("dai_thao_duong",         new[] { "đái tháo đường" }),
            Q("benh_tam_than",          new[] { "tâm thần" }),
            Q("mat_y_thuc",             new[] { "ý thức" }),
            Q("ngat_chong_mat",         new[] { "ngất" }),
            Q("benh_tieu_hoa",          new[] { "tiêu hóa" }),
            Q("roi_loan_giac_ngu",      new[] { "giấc ngủ" }),
            Q("tai_bien_mach_mau_nao",  new[] { "tai biến" }),
            Q("cot_song",               new[] { "cột sống" }),
            Q("su_dung_ruou_bia",       new[] { "rượu" }, new[] { "nghiện" }),
            Q("su_dung_ma_tuy",         new[] { "ma túy" })
        };

        private static DiseaseQuestion Q(string field, string[] must)
        {
            return new DiseaseQuestion { Field = field, Must = must, Not = new string[0] };
        }

        private static DiseaseQuestion Q(string field, string[] must, string[] not)
        {
            return new DiseaseQuestion { Field = field, Must = must, Not = not };
        }

        private static bool warnedDsBenh = false;

        private static Dictionary<string, object> BuildDsBenhBanThan(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            try
            {
                // Mã bệnh -> câu trả lời của hồ sơ này.
                var answerByDiseaseId = new Dictionary<long, string>();
                if (src.Ditys != null)
                {
                    foreach (var d in src.Ditys)
                    {
                        if (d == null) continue;
                        answerByDiseaseId[d.DISEASE_TYPE_ID] = d.IS_YES_NO;
                    }
                }

                var unmatchedFields = new List<string>();
                var matchedDiseaseIds = new List<long>();

                foreach (DiseaseQuestion q in DS_BENH_BAN_THAN)
                {
                    HIS_DISEASE_TYPE dt = FindDiseaseType(src.DiseaseTypes, q);
                    if (dt == null)
                    {
                        b[q.Field] = 0;
                        unmatchedFields.Add(q.Field);
                        continue;
                    }
                    matchedDiseaseIds.Add(dt.ID);

                    string ans;
                    // Chưa trả lời thì gửi 0 — cổng chỉ nhận 1 hoặc 0, không có trạng thái "chưa hỏi".
                    b[q.Field] = (answerByDiseaseId.TryGetValue(dt.ID, out ans) && ans == "1") ? 1 : 0;
                }

                b["benh_khac"] = GetStr(src.Over18, "DISEASES");

                if (!warnedDsBenh)
                {
                    warnedDsBenh = true;
                    if (unmatchedFields.Count > 0)
                        Inventec.Common.Logging.LogSystem.Warn(
                            "SytHcm: KHONG ghep duoc " + unmatchedFields.Count + "/" + DS_BENH_BAN_THAN.Count
                            + " cau hoi tien su voi danh muc benh cua HIS -> gui 0. Cac cau: "
                            + string.Join(", ", unmatchedFields.ToArray())
                            + ". Xem danh sach ten benh trong danh muc ngay duoi de sua tu khoa.");
                    LogDiseaseCatalog(src.DiseaseTypes, matchedDiseaseIds);
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return b;
        }

        /// <summary>Tìm dòng danh mục bệnh khớp câu hỏi. Nhiều dòng khớp thì lấy dòng có tên NGẮN NHẤT
        /// (tên ngắn thường là mục chính, tên dài là mục con chi tiết hơn).</summary>
        private static HIS_DISEASE_TYPE FindDiseaseType(List<HIS_DISEASE_TYPE> all, DiseaseQuestion q)
        {
            try
            {
                if (all == null) return null;
                HIS_DISEASE_TYPE best = null;
                foreach (var dt in all)
                {
                    if (dt == null || string.IsNullOrWhiteSpace(dt.DISEASE_TYPE_NAME)) continue;
                    string name = dt.DISEASE_TYPE_NAME.ToLowerInvariant();

                    bool ok = true;
                    foreach (string kw in q.Must)
                        if (!name.Contains(kw)) { ok = false; break; }
                    if (ok && q.Not != null)
                        foreach (string kw in q.Not)
                            if (name.Contains(kw)) { ok = false; break; }
                    if (!ok) continue;

                    if (best == null || dt.DISEASE_TYPE_NAME.Length < best.DISEASE_TYPE_NAME.Length)
                        best = dt;
                }
                return best;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Ghi danh mục bệnh của HIS ra nhật ký MỘT LẦN, đánh dấu dòng nào đã ghép được câu hỏi nào.
        /// Nhờ đó ghép sai chỉ cần đọc nhật ký là biết sửa từ khóa nào, không phải tra cơ sở dữ liệu.
        /// </summary>
        private static void LogDiseaseCatalog(List<HIS_DISEASE_TYPE> all, List<long> matchedIds)
        {
            try
            {
                if (all == null || all.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: danh muc benh cua HIS RONG -> 21 cau hoi tien su deu gui 0");
                    return;
                }
                var sb = new System.Text.StringBuilder();
                sb.Append("SytHcm: danh muc benh cua HIS (dau + = da ghep duoc cau hoi):");
                foreach (var dt in all)
                {
                    if (dt == null) continue;
                    sb.Append("\r\n  ").Append(matchedIds.Contains(dt.ID) ? "+ " : "  ")
                      .Append(dt.ID).Append(" | ").Append(dt.DISEASE_TYPE_CODE)
                      .Append(" | ").Append(dt.DISEASE_TYPE_NAME);
                }
                Inventec.Common.Logging.LogSystem.Info(sb.ToString());
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>III. Khám thể lực.</summary>
        private static Dictionary<string, object> BuildKhamTheLuc(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            HIS_DHST d = src.Dhst;
            if (d == null)
                Inventec.Common.Logging.LogSystem.Warn(
                    "SytHcm: ho so CHUA CO ban ghi sinh hieu (chieu cao, can nang, mach, huyet ap) "
                    + "-> ca khoi kham the luc gui trong. Cong bat buoc phai co huyet ap tam thu.");

            // Số tròn gửi dạng SỐ NGUYÊN: kiểu decimal bị ghi thành "120.0", cổng đọc kiểu
            // số nguyên sẽ coi như không có giá trị.
            b["chieucao"] = Num(GetDecimal(d, "HEIGHT"));
            b["cannang"] = Num(GetDecimal(d, "WEIGHT"));
            b["bmi"] = Num(GetDecimal(d, "VIR_BMI"));
            b["mach"] = Num(GetDecimal(d, "PULSE"));
            b["huyetaptamthu"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MAX"));
            b["huyetaptamtruong"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MIN"));
            b["nhiptho"] = Num(GetDecimal(d, "BREATH_RATE"));
            b["phanloai"] = KskSytHcmPayload.ResolveSytRankId(
                GetLong(src.Over18, "DHST_RANK"), src.HisRanks);
            return b;
        }

        /// <summary>IV. Khám lâm sàng — 15 mục + trị số mắt/tai + sơ đồ răng.</summary>
        private static Dictionary<string, object> BuildKhamLamSan(KskSytHcmSource src)
        {
            // 15 mục khám (ô tích, 2 chẩn đoán, phân loại) — xem KskSytHcmPayload.
            var b = KskSytHcmPayload.BuildClinicalExam(src.SytHcm, src.Over18, src.HisRanks);

            HIS_KSK_SYT_HCM h = src.SytHcm;
            HIS_KSK_OVER_EIGHTEEN o = src.Over18;

            // Thị lực: 4 trị số cũ ở bảng KSK, 8 trị số mới ở bảng mẫu M3.
            b["mat_khongkinh_mp"] = Num(ToNum(GetStr(o, "EXAM_EYESIGHT_RIGHT")));
            b["mat_khongkinh_mt"] = Num(ToNum(GetStr(o, "EXAM_EYESIGHT_LEFT")));
            b["mat_cokinh_mp"] = Num(ToNum(GetStr(o, "EXAM_EYESIGHT_GLASS_RIGHT")));
            b["mat_cokinh_mt"] = Num(ToNum(GetStr(o, "EXAM_EYESIGHT_GLASS_LEFT")));
            b["mat_kinhlo_mp"] = Num(ToNum(GetStr(h, "EXAM_EYESIGHT_PINHOLE_RIGHT")));
            b["mat_kinhlo_mt"] = Num(ToNum(GetStr(h, "EXAM_EYESIGHT_PINHOLE_LEFT")));
            b["mat_docau_mp"] = Num(ToNum(GetStr(h, "EXAM_EYE_SPHERE_RIGHT")));
            b["mat_docau_mt"] = Num(ToNum(GetStr(h, "EXAM_EYE_SPHERE_LEFT")));
            b["mat_dotru_mp"] = Num(ToNum(GetStr(h, "EXAM_EYE_CYLINDER_RIGHT")));
            b["mat_dotru_mt"] = Num(ToNum(GetStr(h, "EXAM_EYE_CYLINDER_LEFT")));
            b["mat_truc_mp"] = Num(ToNum(GetStr(h, "EXAM_EYE_AXIS_RIGHT")));
            b["mat_truc_mt"] = Num(ToNum(GetStr(h, "EXAM_EYE_AXIS_LEFT")));

            // Thính lực — cột sẵn có ở bảng KSK.
            b["tmh_taitrai_noithuong"] = Num(ToNum(GetStr(o, "EXAM_ENT_LEFT_NORMAL")));
            b["tmh_taitrai_noitham"] = Num(ToNum(GetStr(o, "EXAM_ENT_LEFT_WHISPER")));
            b["tmh_taiphai_noithuong"] = Num(ToNum(GetStr(o, "EXAM_ENT_RIGHT_NORMAL")));
            b["tmh_taiphai_noitham"] = Num(ToNum(GetStr(o, "EXAM_ENT_RIGHT_WHISPER")));

            b["chi_tiet_kham_rang"] = BuildChiTietKhamRang(h);
            return b;
        }

        /// <summary>
        /// Sơ đồ răng: cặp "số răng" -> Id tình trạng răng của cổng.
        /// CHỈ gửi những chiếc CÓ dữ liệu — răng chưa ghi nhận thì bỏ hẳn khỏi bản tin (quy tắc R15).
        /// </summary>
        private static Dictionary<string, object> BuildChiTietKhamRang(HIS_KSK_SYT_HCM h)
        {
            var teeth = new Dictionary<string, object>();
            try
            {
                if (h == null) return teeth;
                string[] all = new string[]
                {
                    "18","17","16","15","14","13","12","11", "21","22","23","24","25","26","27","28",
                    "48","47","46","45","44","43","42","41", "31","32","33","34","35","36","37","38"
                };
                foreach (string no in all)
                {
                    long? v = GetLong(h, "TOOTH_" + no);
                    if (v.HasValue && v.Value > 0) teeth[no] = v.Value;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return teeth;
        }

        /// <summary>
        /// V. Khám cận lâm sàng — KẾT QUẢ THẬT, lấy từ chỉ số của dịch vụ đã nối ở bảng khai báo
        /// (nút "Nối chỉ số cận lâm sàng" trên màn hình đồng bộ).
        ///
        /// Cách chạy: bảng khai báo cho biết mỗi chỉ tiêu của cổng ứng với MÃ CHỈ SỐ nào của HIS;
        /// tra mã đó trong kết quả xét nghiệm của chính đợt điều trị rồi lấy giá trị.
        ///
        /// Chỉ tiêu CHƯA NỐI, hoặc đã nối mà bệnh nhân không làm dịch vụ đó, thì KHÔNG GỬI —
        /// không gửi 0 hay chuỗi rỗng, vì cổng sẽ hiểu là "đã làm và kết quả bằng 0".
        /// </summary>
        private static Dictionary<string, object> BuildCanLamSan(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            try
            {
                // 3 chỉ tiêu này KHÔNG lấy từ bảng nối chỉ số mà lấy thẳng từ ô nhập của màn hình
                // khám sức khỏe, nên phải chạy TRƯỚC và chạy cả khi viện chưa khai báo nối chỉ số.
                AddClsFromScreen(b, src);

                Dictionary<string, string> map = ParseClsMap(src.ClsMapJson);
                Dictionary<string, string> valueByIndexCode = IndexTeinValues(src.ClsTeins);

                // CHAN DOAN: ba con so nay chi ro hong o dau — chua noi chi so / ho so khong co ket
                // qua / noi roi ma ma chi so khong khop. Khong co chung thi chi biet "gui rong".
                Inventec.Common.Logging.LogSystem.Warn("SytHcm/CLS: bang khai bao "
                    + (string.IsNullOrWhiteSpace(src.ClsMapJson) ? "RONG" : src.ClsMapJson.Length + " ky tu")
                    + ", so cap da noi=" + map.Count
                    + ", so dong ket qua xet nghiem=" + ((src.ClsTeins != null) ? src.ClsTeins.Count : 0)
                    + ", so ma chi so co ket qua=" + valueByIndexCode.Count);

                if (valueByIndexCode.Count > 0)
                {
                    var codes = new List<string>(valueByIndexCode.Keys);
                    if (codes.Count > 40) codes.RemoveRange(40, codes.Count - 40);
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm/CLS: ma chi so CO ket qua -> "
                        + string.Join(", ", codes.ToArray()));
                }

                if (map.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: CHUA noi chi so nao o bang khai bao"
                        + " -> 34 chi tieu xet nghiem gui rong");
                }
                else if (valueByIndexCode.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: ho so khong co ket qua xet nghiem nao"
                        + " -> 34 chi tieu xet nghiem gui rong");
                }
                else
                {
                    int filled = 0, noResult = 0;
                    foreach (var kv in map)
                    {
                        string fieldCode = kv.Key;
                        string indexCode = kv.Value;

                        string raw;
                        if (!valueByIndexCode.TryGetValue(NormCode(indexCode), out raw)
                            || string.IsNullOrWhiteSpace(raw))
                        {
                            // Ghi ro ma MUON ma khong thay -> doi chieu voi danh sach ma CO ket qua
                            // o dong log tren la biet ngay lech ma hay benh nhan khong lam dich vu do.
                            Inventec.Common.Logging.LogSystem.Warn("SytHcm/CLS: " + fieldCode
                                + " <- ma chi so \"" + indexCode + "\" KHONG co ket qua");
                            noResult++;
                            continue;
                        }

                        Inventec.Common.Logging.LogSystem.Debug("SytHcm/CLS: " + fieldCode
                            + " <- " + indexCode + " = " + raw);

                        object val = ConvertClsValue(fieldCode, raw.Trim());
                        if (val == null) { noResult++; continue; }

                        b[fieldCode] = val;
                        filled++;
                    }

                    Inventec.Common.Logging.LogSystem.Info("SytHcm: can lam san — noi " + map.Count
                        + " chi tieu, lay duoc " + filled + ", khong co ket qua " + noResult);
                }

                // Đặc tả có HAI bộ chỉ tiêu trùng nhau: bộ thường và bộ khám định kỳ (tiền tố kskdk_).
                // HIS không có cờ phân biệt hồ sơ thuộc bộ nào nên điền cả hai bộ giống nhau —
                // giữ đúng cách đã đẩy thành công trước đó. Bỏ khi Sở chốt quy tắc phân biệt.
                foreach (var kv in new Dictionary<string, object>(b))
                {
                    if (kv.Key.StartsWith("xnm_") || kv.Key.StartsWith("shm_")
                        || kv.Key.StartsWith("xnnt_") || kv.Key == "chuan_doan_hinh_anh")
                    {
                        b["kskdk_" + kv.Key] = kv.Value;
                    }
                }

            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return b;
        }

        /// <summary>
        /// 3 chỉ tiêu lấy thẳng từ ô nhập của tab "Khám cận lâm sàng" trên màn hình khám sức khỏe,
        /// KHÔNG qua bảng nối chỉ số:
        ///
        ///   chuan_doan_hinh_anh        <- ô "3. Chẩn đoán hình ảnh"
        ///   can_lam_sang_khac          <- 1 khi ô "4. Kết quả khám cận lâm sàng khác" có nhập, 0 nếu không
        ///   can_lam_sang_khac_chi_tiet <- nội dung ô 4
        ///
        /// Cờ `can_lam_sang_khac` GỬI CẢ KHI BẰNG 0 — đây là câu trả lời "có làm cận lâm sàng khác
        /// hay không", bỏ trống thì cổng coi như chưa trả lời.
        /// </summary>
        private static void AddClsFromScreen(Dictionary<string, object> b, KskSytHcmSource src)
        {
            try
            {
                string diim = GetStr(src.Over18, "RESULT_DIIM");
                if (!string.IsNullOrWhiteSpace(diim)) b["chuan_doan_hinh_anh"] = diim.Trim();

                string other = GetStr(src.Over18, "OTHER_CLS_RESULT");
                bool hasOther = !string.IsNullOrWhiteSpace(other);
                b["can_lam_sang_khac"] = hasOther ? 1 : 0;
                b["can_lam_sang_khac_chi_tiet"] = hasOther ? other.Trim() : "";
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>Bảng khai báo đã lưu -> cặp (chỉ tiêu của cổng, mã chỉ số của HIS).</summary>
        private static Dictionary<string, string> ParseClsMap(string json)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return rs;
                var file = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<ADO.KskSytClsMapFileADO>(json);
                if (file == null || file.Items == null) return rs;
                foreach (var it in file.Items)
                {
                    if (it == null) continue;
                    if (string.IsNullOrWhiteSpace(it.FieldCode)) continue;
                    if (string.IsNullOrWhiteSpace(it.TestIndexCode)) continue;
                    rs[it.FieldCode.Trim()] = it.TestIndexCode.Trim();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        /// <summary>
        /// Kết quả xét nghiệm của hồ sơ -> tra theo MÃ CHỈ SỐ.
        ///
        /// Một mã có thể xuất hiện nhiều lần (làm lại xét nghiệm). Giữ giá trị KHÔNG RỖNG ĐẦU TIÊN:
        /// khung nhìn không có cột thời điểm trả kết quả nên không chọn được "bản mới nhất" một cách
        /// chắc chắn, mà lấy bừa bản sau thì có thể lấy đúng bản chưa nhập.
        /// </summary>
        private static Dictionary<string, string> IndexTeinValues(
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_TEIN> teins)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                if (teins == null) return rs;
                foreach (var t in teins)
                {
                    if (t == null || string.IsNullOrWhiteSpace(t.TEST_INDEX_CODE)) continue;
                    if (string.IsNullOrWhiteSpace(t.VALUE)) continue;
                    string k = NormCode(t.TEST_INDEX_CODE);
                    if (!rs.ContainsKey(k)) rs[k] = t.VALUE;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        private static string NormCode(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "" : s.Trim().ToUpperInvariant();
        }

        /// <summary>Danh mục Âm tính / Dương tính của cổng — chỉ tiêu Nitrit nước tiểu gửi Id.</summary>
        private const string CAT__AM_DUONG_TINH = "AmTinh_DuongTinh";

        /// <summary>
        /// 14 chỉ tiêu xét nghiệm máu — ví dụ body của đặc tả gửi SỐ.
        /// </summary>
        private static readonly HashSet<string> CLS_NUMBER_FIELDS = new HashSet<string>
        {
            "xnm_slhc", "xnm_huyetsacto", "xnm_hematocrit", "xnm_mcv", "xnm_mch", "xnm_mchc",
            "xnm_rdw", "xnm_slbc", "xnm_slbc_trungtinh", "xnm_slbc_lympho", "xnm_slbc_donnhan",
            "xnm_slbc_aitoan", "xnm_slbc_aikiem", "xnm_sltc"
        };

        /// <summary>
        /// 5 chỉ tiêu sinh hóa máu + chỉ số khác + 4 chỉ tiêu tầm soát nữ — gửi CHUỖI.
        /// Sinh hóa: ví dụ body của đặc tả gửi chuỗi dù giá trị là số.
        /// Tầm soát nữ: kết quả là lời mô tả ("Không thấy bất thường").
        /// </summary>
        private static readonly HashSet<string> CLS_TEXT_FIELDS = new HashSet<string>
        {
            "shm_duongmau", "shm_ure", "shm_creatinin", "shm_asat_got", "shm_alat_gpt",
            "xnnt_khac",
            "xet_nghiem_te_bao_co_tu_cung", "xet_nghiem_hpv", "xquang_nhu", "sieu_am_2_tuyen_vu"
        };

        /// <summary>
        /// Đổi kết quả của HIS sang đúng kiểu mà cổng nhận cho từng chỉ tiêu.
        /// Trả null = không gửi chỉ tiêu này.
        /// </summary>
        private static object ConvertClsValue(string fieldCode, string raw)
        {
            try
            {
                // Nitrit: chỉ tiêu DUY NHẤT của nhóm nước tiểu nhận Id danh mục (5120 Âm / 5119 Dương).
                if (fieldCode == "xnnt_nitrit") return MapAmDuongTinh(raw);

                if (CLS_TEXT_FIELDS.Contains(fieldCode)) return raw;

                if (CLS_NUMBER_FIELDS.Contains(fieldCode))
                {
                    decimal? n = ParseClsNumber(raw);
                    if (n.HasValue) return Num(n.Value);
                    // Chỉ tiêu khai là số mà kết quả lại là chữ ("không làm", "thiếu mẫu") -> bỏ,
                    // gửi chữ vào trường số thì cổng từ chối cả hồ sơ.
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: chi tieu " + fieldCode
                        + " can SO nhung ket qua khong doc duoc thanh so -> bo qua chi tieu nay");
                    return null;
                }

                // 9 chỉ tiêu nước tiểu còn lại. Hai mục của đặc tả ghi khác nhau (ví dụ body là SỐ,
                // bảng chi tiết là chuỗi 20 ký tự) nên nhận cả hai dạng, theo thứ tự:
                //   1. đọc được thành số            -> gửi SỐ
                //   2. mang nghĩa âm tính/bình thường (hoặc dương tính) -> gửi Id danh mục
                //   3. còn lại (ví dụ "vết")        -> gửi nguyên chữ
                decimal? u = ParseClsNumber(raw);
                if (u.HasValue) return Num(u.Value);
                if (IsAmDuongTinhWord(raw)) return MapAmDuongTinh(raw);
                return raw;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Các cách ghi kết quả mang nghĩa ÂM TÍNH / BÌNH THƯỜNG.
        ///
        /// Máy xét nghiệm và người nhập mỗi nơi ghi một kiểu, nên liệt kê cả bốn dạng hay gặp:
        /// tiếng Việt có dấu, tiếng Việt không dấu, viết tắt, và tiếng Anh của máy.
        /// So sánh sau khi đã bỏ khoảng trắng thừa và đưa về chữ thường.
        /// </summary>
        private static readonly HashSet<string> AM_TINH_WORDS = new HashSet<string>
        {
            "âm tính", "am tinh", "âm", "am", "at",
            "-", "(-)", "( - )", "0",
            "neg", "neg.", "negative", "neg-trace", "neg trace", "negtrace",
            "norm", "norm.", "normal",
            "bình thường", "binh thuong", "bt",
            "trong giới hạn bình thường", "trong gioi han binh thuong",
            "không phát hiện", "khong phat hien", "kpt", "kph",
            "không có", "khong co"
        };

        /// <summary>Các cách ghi kết quả mang nghĩa DƯƠNG TÍNH.</summary>
        private static readonly HashSet<string> DUONG_TINH_WORDS = new HashSet<string>
        {
            "dương tính", "duong tinh", "dương", "duong", "dt",
            "+", "(+)", "( + )", "++", "+++", "1",
            "pos", "pos.", "positive",
            "bất thường", "bat thuong",
            "có", "co"
        };

        /// <summary>
        /// Kết quả dạng chữ -> Id danh mục Âm/Dương tính của cổng.
        /// Không nhận ra thì trả null = KHÔNG gửi chỉ tiêu, thay vì đoán bừa một trong hai.
        /// </summary>
        private static object MapAmDuongTinh(string raw)
        {
            string v = Norm(raw);
            if (AM_TINH_WORDS.Contains(v)) v = "âm tính";
            else if (DUONG_TINH_WORDS.Contains(v)) v = "dương tính";

            long? id = MapByName(CAT__AM_DUONG_TINH, v);
            if (id.HasValue) return id.Value;

            Inventec.Common.Logging.LogSystem.Warn("SytHcm: ket qua \"" + raw
                + "\" khong quy doi duoc sang Am/Duong tinh -> bo qua chi tieu nay");
            return null;
        }

        /// <summary>Kết quả có mang nghĩa âm tính / dương tính không.</summary>
        private static bool IsAmDuongTinhWord(string raw)
        {
            string v = Norm(raw);
            return AM_TINH_WORDS.Contains(v) || DUONG_TINH_WORDS.Contains(v);
        }

        /// <summary>
        /// Đọc số từ kết quả của HIS. Kết quả hay kèm dấu so sánh hoặc đơn vị ("&lt; 0.2", "5,1 mmol/l")
        /// nên chỉ giữ phần số; dấu phẩy thập phân đổi thành dấu chấm.
        /// </summary>
        private static decimal? ParseClsNumber(string raw)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(raw)) return null;
                var sb = new System.Text.StringBuilder();
                foreach (char ch in raw)
                {
                    if (char.IsDigit(ch)) sb.Append(ch);
                    else if (ch == '.' || ch == ',') sb.Append('.');
                    else if (ch == '-' && sb.Length == 0) sb.Append(ch);
                    else if (sb.Length > 0) break;   // hết phần số -> phần sau là đơn vị
                }
                decimal d;
                string t = sb.ToString().TrimEnd('.');
                if (t.Length == 0 || t == "-") return null;
                return decimal.TryParse(t, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out d) ? (decimal?)d : null;
            }
            catch { return null; }
        }

        /// <summary>
        /// VI. Kết luận — chỉ tiêu `de_nghi`.
        ///
        /// Ưu tiên cụm "Đề nghị" ở tab Kết luận (bảng mẫu M3):
        ///   1. chọn "Khác" và có ghi rõ  -> gửi nội dung ghi rõ;
        ///   2. chọn mục có sẵn           -> gửi TÊN của mục đó trong danh mục của cổng;
        ///   3. chưa chọn gì              -> quay về kết luận của hồ sơ KSK như trước.
        /// Đặc tả để chỉ tiêu này kiểu CHUỖI 500 ký tự nên gửi tên mục, không gửi mã.
        /// </summary>
        private static Dictionary<string, object> BuildKetLuan(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            try
            {
                long? suggestId = GetLong(src.SytHcm, "SYT_SUGGEST_ID");

                // Trường DANH MỤC — gửi MÃ ĐỊNH DANH của mục đã chọn, không gửi tên. Tên trường là
                // `danh_muc_de_nghi` (xác nhận của người yêu cầu), không phải `KetLuan_DeNghi` như
                // trước — `KetLuan_DeNghi` chỉ là mã của danh mục khi gọi dịch vụ tra danh mục.
                if (suggestId.HasValue && suggestId.Value > 0)
                    b["danh_muc_de_nghi"] = suggestId.Value;

                // Trường CHỮ — ưu tiên nội dung "Đề nghị khác" người dùng tự nhập; không nhập thì
                // gửi tên của mục đã chọn để bản tin vẫn đọc được bằng mắt.
                string other = GetStr(src.SytHcm, "SUGGEST_OTHER");
                if (!string.IsNullOrWhiteSpace(other))
                {
                    b["de_nghi"] = other.Trim();
                    return b;
                }

                string name = NameFromCatalog(CAT__KET_LUAN_DE_NGHI, suggestId);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    b["de_nghi"] = name;
                    return b;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            b["de_nghi"] = FirstStr(src.General, "CONCLUSION", "SUGGEST");
            return b;
        }

        /// <summary>Tên của một mục trong danh mục của cổng, tra theo mã định danh.</summary>
        private static string NameFromCatalog(string catalogCode, long? id)
        {
            try
            {
                if (!id.HasValue || id.Value <= 0) return null;
                var items = KskSytHcmPayload.ReadCachedCatalog(catalogCode);
                if (items == null) return null;
                string key = id.Value.ToString();
                foreach (var it in items)
                    if (it != null && it.Id == key) return it.Name;

                Inventec.Common.Logging.LogSystem.Warn("SytHcm: ma " + id.Value
                    + " khong co trong danh muc " + catalogCode + " -> khong lay duoc ten");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Nơi công tác dùng tạm khi chưa nối được danh mục 5431 mục của cổng — Id 1, đúng giá trị
        /// mà ví dụ body của Sở dùng. Bỏ khi Sở trả lời cách nối.
        /// </summary>
        private const int TAM_NOI_CONG_TAC_ID = 1;
        private const int TAM_NOI_CONG_TAC_XA_PHUONG = 1;

        /// <summary>
        /// Nghề nghiệp dùng tạm khi chưa nối được danh mục 792 mục của cổng.
        /// Chọn "Nông nghiệp, lâm nghiệp và thủy sản" — gần nghĩa với nghề của bệnh nhân đang thử
        /// (HIS ghi "Nông dân"). Id và mã lấy đúng CÙNG một mục trong danh mục của Sở.
        /// </summary>
        private const int TAM_NGHE_NGHIEP_ID = 286;
        private const string TAM_NGHE_NGHIEP_CODE = "17210";

        #endregion

        #region ===== Gửi kèm tên trường của bảng chi tiết =====

        /// <summary>
        /// Đặc tả đặt tên trường KHÁC NHAU ở hai mục: ví dụ body (mục 2.2) và bảng chi tiết (mục 2.3).
        /// Ban đầu chọn theo ví dụ body, nhưng thực tế cổng đòi theo BẢNG CHI TIẾT — gửi đủ
        /// `huyetaptamthu` mà cổng vẫn báo "Vui lòng truyền huyetaptamthu!", tức là nó tìm
        /// `theluc_huyetaptamthu`.
        ///
        /// Vì không biết chắc cổng đọc mục nào cho TỪNG trường, gửi CẢ HAI TÊN cho mọi trường lệch.
        /// Trường thừa thì cổng bỏ qua (bản tin trước đã có `bmi`, `nhiptho` thừa mà không bị chê),
        /// còn thiếu tên đúng thì hỏng cả hồ sơ. Bỏ dần khi Sở xác nhận tên chuẩn.
        ///
        /// Mỗi dòng: tên đang gửi -> tên thêm vào.
        /// </summary>
        private static readonly string[][] TTHC_ALIASES = new string[][]
        {
            new[] { "doi_tuong_kham", "doituongkham" },
            new[] { "dia_diem_kham",  "diadiemkham"  },
            new[] { "ward_id",        "wardId"       },
            new[] { "ward_code",      "wardCode"     },
            new[] { "ly_do_kham",     "lydokham"     }
        };

        private static readonly string[][] THE_LUC_ALIASES = new string[][]
        {
            new[] { "chieucao",         "theluc_chieucao"         },
            new[] { "cannang",          "theluc_cannang"          },
            new[] { "mach",             "theluc_mach"             },
            new[] { "nhiptho",          "theluc_nhiptho"          },
            new[] { "huyetaptamthu",    "theluc_huyetaptamthu"    },
            new[] { "huyetaptamtruong", "theluc_huyetaptamtruong" },
            new[] { "phanloai",         "theluc_phanloai"         },
            new[] { "bmi",              "theluc_bmi"              }
        };

        private static readonly string[][] TIEN_SU_ALIASES = new string[][]
        {
            new[] { "giadinh_danhsachbenh",     "ts_giadinh_danhsachbenh"     },
            new[] { "giadinh_danhsachbenh_icd", "ts_giadinh_danhsachbenh_icd" }
        };

        private static readonly string[][] LAM_SAN_ALIASES = new string[][]
        {
            new[] { "chi_tiet_kham_rang", "ChiTietKhamRang" }
        };

        /// <summary>
        /// Khối kết luận KHÔNG cần gửi kèm tên nào nữa: trường danh mục có tên riêng là
        /// `danh_muc_de_nghi`, đã gửi thẳng trong BuildKetLuan.
        /// </summary>
        private static readonly string[][] KET_LUAN_ALIASES = new string[][] { };

        /// <summary>Thêm bản sao của một trường dưới tên khác, giữ nguyên giá trị.</summary>
        private static void AddAliases(Dictionary<string, object> block, string[][] aliases)
        {
            try
            {
                if (block == null || aliases == null) return;
                foreach (string[] a in aliases)
                {
                    object v;
                    if (!block.TryGetValue(a[0], out v)) continue;
                    if (!block.ContainsKey(a[1])) block[a[1]] = v;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Kiểm tra trước khi gửi =====

        /// <summary>
        /// Trường bắt buộc, mỗi dòng: tên trường của cổng · khối chứa nó · nơi lấy dữ liệu ở HIS.
        ///
        /// Lấy theo dấu "x" ở bảng chi tiết của đặc tả, CỘNG các trường mà cổng thực tế đòi dù đặc tả
        /// không đánh dấu (huyết áp tâm thu là một ví dụ đã gặp) — phát hiện thêm thì bổ sung vào đây.
        /// </summary>
        private static readonly string[][] REQUIRED_FIELDS = new string[][]
        {
            new[] { "ngay_kham",            "tthc",         "ngày kết luận của hồ sơ KSK" },
            new[] { "doi_tuong_kham",       "tthc",         "ô Đối tượng khám ở màn hình nhập KSK" },
            new[] { "dia_diem_kham",        "tthc",         "ô Địa điểm khám ở màn hình nhập KSK" },
            new[] { "dinh_danh_ca_nhan",    "tthc",         "số CCCD của bệnh nhân" },
            new[] { "ho_ten",               "tthc",         "tên bệnh nhân" },
            new[] { "ngay_sinh",            "tthc",         "ngày sinh của bệnh nhân" },
            new[] { "gioi_tinh",            "tthc",         "giới tính của bệnh nhân" },
            new[] { "sdt",                  "tthc",         "số điện thoại của bệnh nhân" },
            new[] { "dia_chi_hien_tai",     "tthc",         "địa chỉ hiện tại của bệnh nhân" },
            new[] { "ward_id",              "tthc",         "xã/phường của bệnh nhân — phải khớp danh mục của cổng" },
            new[] { "nghenghiep_id",        "tthc",         "nghề nghiệp của bệnh nhân — phải khớp danh mục của cổng" },
            new[] { "hinh_thuc_chi_tra_khamsk",            "tthc", "ô Nguồn chi trả ở màn hình nhập KSK" },
            new[] { "hinh_thuc_chi_tra_khamsk_chi_tiet",   "tthc", "ô Hình thức chi trả ở màn hình nhập KSK" },
            new[] { "huyetaptamthu",        "kham_the_luc", "huyết áp tâm thu ở phần sinh hiệu" },
            new[] { "huyetaptamtruong",     "kham_the_luc", "huyết áp tâm trương ở phần sinh hiệu" },
            new[] { "chieucao",             "kham_the_luc", "chiều cao ở phần sinh hiệu" },
            new[] { "cannang",              "kham_the_luc", "cân nặng ở phần sinh hiệu" },
            new[] { "phanloai",             "kham_the_luc", "phân loại thể lực ở phần sinh hiệu" }
        };

        /// <summary>
        /// Liệt kê MỘT LƯỢT mọi trường bắt buộc còn trống, kèm nơi phải nhập.
        /// Cổng chỉ báo từng trường một mỗi lần gửi nên nếu không kiểm trước thì phải đẩy rất nhiều lần
        /// mới biết hết. Ngoài ra kiểm cả 15 phân loại của khối khám lâm sàng.
        /// </summary>
        internal static List<string> DescribeMissingRequired(object body)
        {
            var missing = new List<string>();
            try
            {
                if (body == null) return missing;

                foreach (string[] f in REQUIRED_FIELDS)
                {
                    Dictionary<string, object> block = GetBlock(body, f[1]);
                    if (block == null) continue;
                    object v;
                    if (!block.TryGetValue(f[0], out v) || IsEmpty(v))
                        missing.Add(f[0] + " (" + f[2] + ")");
                }

                // 15 phân loại của khối khám lâm sàng — đặc tả đánh dấu bắt buộc cho từng mục.
                Dictionary<string, object> ls = GetBlock(body, "kham_lam_san");
                if (ls != null)
                {
                    var noRank = new List<string>();
                    foreach (KeyValuePair<string, string> sec in SYT_HCM_SECTION_LABELS)
                    {
                        object v;
                        if (!ls.TryGetValue(sec.Key + "_phanloai", out v) || IsEmpty(v))
                            noRank.Add(sec.Value);
                    }
                    if (noRank.Count > 0)
                        missing.Add("phân loại của " + noRank.Count + " mục khám ("
                            + string.Join(", ", noRank.ToArray()) + ")");
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return missing;
        }

        /// <summary>Nhãn tiếng Việt của 15 mục khám — chỉ dùng để viết thông báo cho dễ đọc.</summary>
        private static readonly Dictionary<string, string> SYT_HCM_SECTION_LABELS
            = new Dictionary<string, string>
            {
                { "noikhoa", "Tuần hoàn" }, { "hohap", "Hô hấp" }, { "tieuhoa", "Tiêu hóa" },
                { "thantietnieu", "Thận - Tiết niệu" }, { "noitiet", "Nội tiết" },
                { "coxuongkhop", "Cơ - xương - khớp" }, { "thankinh", "Thần kinh" },
                { "tamthan", "Tâm thần" }, { "ngoaikhoa", "Ngoại khoa" }, { "dalieu", "Da liễu" },
                { "sankhoa", "Sản khoa" }, { "phukhoa", "Phụ khoa" }, { "mat", "Mắt" },
                { "tmh", "Tai - Mũi - Họng" }, { "rhm", "Răng - Hàm - Mặt" }
            };

        /// <summary>Lấy một khối của bản tin theo tên (tthc, kham_the_luc, kham_lam_san...).</summary>
        private static Dictionary<string, object> GetBlock(object body, string name)
        {
            try
            {
                PropertyInfo pi = body.GetType().GetProperty(name);
                return (pi != null) ? pi.GetValue(body, null) as Dictionary<string, object> : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static bool IsEmpty(object v)
        {
            if (v == null) return true;
            string s = v.ToString();
            return string.IsNullOrWhiteSpace(s);
        }

        #endregion

        #region ===== Quy đổi danh mục =====

        /// <summary>Bảng tra "tên đã chuẩn hóa" -> Id, dựng một lần cho mỗi danh mục.</summary>
        private static readonly Dictionary<string, Dictionary<string, long>> byNameCache
            = new Dictionary<string, Dictionary<string, long>>();

        /// <summary>
        /// Quy đổi theo TÊN nhưng trả về CẢ MỤC danh mục, để nơi gọi lấy được Id và mã của cùng
        /// một mục. Danh mục nghề nghiệp và xã/phường của Sở có mã riêng, gửi lẫn mã của HIS là
        /// cổng từ chối.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, KskSytCatalogItem>> byNameItemCache
            = new Dictionary<string, Dictionary<string, KskSytCatalogItem>>();

        internal static KskSytCatalogItem MapByNameItem(string catalogCode, string hisName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hisName)) return null;

                // Dựng BẢNG TRA một lần cho mỗi danh mục, như MapByName và MapIcdCodeToId ở dưới.
                // Trước đây quét lần lượt và chuẩn hóa lại tên của TỪNG mục ở mỗi lần gọi — xã/phường
                // 3.321 mục, nghề nghiệp 792 mục, nhân cho số hồ sơ của cả đợt đẩy.
                Dictionary<string, KskSytCatalogItem> map;
                if (!byNameItemCache.TryGetValue(catalogCode, out map))
                {
                    map = new Dictionary<string, KskSytCatalogItem>();
                    var items = KskSytHcmPayload.ReadCachedCatalog(catalogCode);
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (it == null) continue;
                            // Mục đầu tiên thắng — giữ đúng thứ tự ưu tiên của cách quét lần lượt cũ.
                            string k = Norm(it.Name);
                            if (k.Length > 0 && !map.ContainsKey(k)) map[k] = it;
                            string c = Norm(it.Code);
                            if (c.Length > 0 && !map.ContainsKey(c)) map[c] = it;
                        }
                    }
                    byNameItemCache[catalogCode] = map;
                }

                KskSytCatalogItem found;
                if (map.TryGetValue(Norm(hisName), out found)) return found;

                Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong tim duoc \"" + hisName
                    + "\" trong danh muc " + catalogCode + " -> gui trong ca Id va ma");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static long? ToLongOrNull(string v)
        {
            long r;
            return (!string.IsNullOrWhiteSpace(v) && long.TryParse(v, out r)) ? (long?)r : null;
        }

        /// <summary>
        /// Quy đổi theo TÊN: chuẩn hóa cả hai bên (bỏ dấu cách thừa, về chữ thường) rồi so.
        /// Không tìm được thì trả null và ghi cảnh báo — thà để trống còn hơn gửi Id sai.
        /// </summary>
        internal static long? MapByName(string catalogCode, string hisName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hisName)) return null;

                Dictionary<string, long> map;
                if (!byNameCache.TryGetValue(catalogCode, out map))
                {
                    map = new Dictionary<string, long>();
                    var items = KskSytHcmPayload.ReadCachedCatalog(catalogCode);
                    if ((items == null || items.Count == 0) && catalogCode == CAT__YES_NO)
                        items = KskSytHcmPayload.ReadCachedCatalog(CAT__YES_NO_M4);
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (it == null) continue;
                            long id;
                            if (!long.TryParse(it.Id, out id)) continue;
                            string k = Norm(it.Name);
                            if (k.Length > 0 && !map.ContainsKey(k)) map[k] = id;
                            // Nhiều danh mục còn trả mã chữ -> cho tra theo mã luôn.
                            string c = Norm(it.Code);
                            if (c.Length > 0 && !map.ContainsKey(c)) map[c] = id;
                        }
                    }
                    byNameCache[catalogCode] = map;
                }

                long found;
                if (map.TryGetValue(Norm(hisName), out found)) return found;

                Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong quy doi duoc \"" + hisName
                    + "\" sang Id trong danh muc " + catalogCode + " -> gui trong");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Quy đổi MÃ ICD sang Id trong danh mục ICD của cổng.
        ///
        /// Danh mục ICD của cổng để trống trường mã, mã nằm ở ĐẦU TÊN theo dạng
        /// "K65.9 -- Viêm phúc mạc...". Nên phải tách phần trước dấu "--" để so mã.
        /// Danh mục có hơn 11.000 mục nên bảng tra dựng một lần rồi dùng lại.
        /// </summary>
        private static Dictionary<string, long> icdByCode;

        internal static long? MapIcdCodeToId(string icdCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(icdCode)) return null;

                if (icdByCode == null)
                {
                    icdByCode = new Dictionary<string, long>();
                    var items = KskSytHcmPayload.ReadCachedCatalog(CAT__ICD);
                    if (items != null)
                    {
                        foreach (var it in items)
                        {
                            if (it == null) continue;
                            long id;
                            if (!long.TryParse(it.Id, out id)) continue;
                            string code = ExtractIcdCode(it.Code, it.Name);
                            if (code.Length > 0 && !icdByCode.ContainsKey(code)) icdByCode[code] = id;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Info(
                        "SytHcm: bang tra ma benh cua cong co " + icdByCode.Count + " ma");
                }

                long found;
                if (icdByCode.TryGetValue(Norm(icdCode), out found)) return found;

                Inventec.Common.Logging.LogSystem.Warn(
                    "SytHcm: ma benh \"" + icdCode + "\" khong co trong danh muc ICD cua cong -> gui trong");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Lấy mã bệnh: ưu tiên trường mã, không có thì tách phần trước "--" của tên.</summary>
        private static string ExtractIcdCode(string code, string name)
        {
            if (!string.IsNullOrWhiteSpace(code)) return Norm(CleanIcdCode(code));
            if (string.IsNullOrWhiteSpace(name)) return "";
            int i = name.IndexOf("--", StringComparison.Ordinal);
            return Norm(CleanIcdCode((i > 0) ? name.Substring(0, i) : name));
        }

        /// <summary>
        /// Bỏ dấu thập tự / hoa thị khỏi mã bệnh của cổng.
        ///
        /// 1.226 mục trong danh mục của cổng có mã kèm dấu thập tự ("A06.5†") đánh dấu cặp bệnh
        /// nguyên nhân / biểu hiện. Mã bệnh của HIS không có dấu này, nên không bỏ thì tra không ra.
        /// Màn hình nhập KSK bỏ y như vậy khi đổ danh mục — hai bên phải đọc giống nhau.
        /// </summary>
        private static string CleanIcdCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return "";
            return code.Replace("†", "").Replace("*", "").Trim();
        }

        /// <summary>Quy đổi danh sách mã bệnh (nối ";") sang danh sách Id nối bằng ",".</summary>
        internal static string MapIcdListToIds(string icdCodes)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(icdCodes)) return null;
                var ids = new List<string>();
                foreach (string one in icdCodes.Split(';', ','))
                {
                    string t = (one ?? "").Trim();
                    if (t.Length == 0) continue;

                    // Màn hình nhập KSK nay lưu THẲNG Id của cổng vào cột này, nên số thuần thì dùng
                    // nguyên. Vẫn giữ đường quy đổi mã -> Id cho hồ sơ lưu TRƯỚC khi đổi ô chọn bệnh,
                    // nếu không những hồ sơ đó đẩy lên sẽ mất chẩn đoán.
                    long already;
                    if (long.TryParse(t, out already))
                    {
                        if (already > 0) ids.Add(already.ToString());
                        continue;
                    }

                    long? id = MapIcdCodeToId(t);
                    if (id.HasValue) ids.Add(id.Value.ToString());
                }
                return (ids.Count > 0) ? string.Join(",", ids.ToArray()) : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Giá trị cho câu hỏi có/không.</summary>
        private static object YesNo(bool value)
        {
            if (!YES_NO_AS_CATALOG_ID) return value ? 1 : 0;
            long? id = MapByName(CAT__YES_NO, value ? "Có" : "Không");
            return id.HasValue ? (object)id.Value : (value ? 1 : 0);
        }

        private static string Norm(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            return s.Trim().ToLowerInvariant();
        }

        #endregion

        #region ===== Đọc dữ liệu, đổi định dạng =====

        private static readonly Dictionary<string, PropertyInfo> propCache
            = new Dictionary<string, PropertyInfo>();

        private static PropertyInfo Prop(object o, string name)
        {
            if (o == null) return null;
            string key = o.GetType().Name + "." + name;
            PropertyInfo pi;
            if (propCache.TryGetValue(key, out pi)) return pi;
            pi = o.GetType().GetProperty(name);
            propCache[key] = pi;
            return pi;
        }

        private static string GetStr(object o, string name)
        {
            PropertyInfo pi = Prop(o, name);
            if (pi == null) return null;
            object v = pi.GetValue(o, null);
            return (v != null) ? v.ToString() : null;
        }

        /// <summary>Lấy giá trị đầu tiên có dữ liệu — dùng khi các phiên bản bảng đặt tên cột khác nhau.</summary>
        private static string FirstStr(object o, params string[] names)
        {
            foreach (string n in names)
            {
                string v = GetStr(o, n);
                if (!string.IsNullOrWhiteSpace(v)) return v;
            }
            return null;
        }

        private static long? GetLong(object o, string name)
        {
            PropertyInfo pi = Prop(o, name);
            if (pi == null) return null;
            object v = pi.GetValue(o, null);
            if (v == null) return null;
            long r;
            return long.TryParse(v.ToString(), out r) ? (long?)r : null;
        }

        private static long? FirstLong(object o, params string[] names)
        {
            foreach (string n in names)
            {
                long? v = GetLong(o, n);
                if (v.HasValue && v.Value > 0) return v;
            }
            return null;
        }

        private static decimal? GetDecimal(object o, string name)
        {
            PropertyInfo pi = Prop(o, name);
            if (pi == null) return null;
            object v = pi.GetValue(o, null);
            if (v == null) return null;
            decimal r;
            return decimal.TryParse(v.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out r)
                ? (decimal?)r : null;
        }

        /// <summary>
        /// Số tròn -> gửi dạng SỐ NGUYÊN, số lẻ -> giữ nguyên.
        ///
        /// Kiểu decimal bị ghi ra JSON kèm phần thập phân ("120.0"). Chỗ nào đặc tả để kiểu số
        /// nguyên mà nhận "120.0" thì có thể đọc không ra và coi như không có giá trị.
        /// </summary>
        private static object Num(decimal? v)
        {
            if (!v.HasValue) return null;
            if (v.Value == decimal.Truncate(v.Value)) return (long)v.Value;
            return v.Value;
        }

        /// <summary>Trị số đo lưu dạng chữ ở HIS -> số. Không phải số thì trả null, không gửi chữ.</summary>
        private static decimal? ToNum(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            decimal r;
            return decimal.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Any,
                CultureInfo.InvariantCulture, out r) ? (decimal?)r : null;
        }

        /// <summary>Thời điểm HIS (yyyyMMddHHmmss) -> chuỗi ngày yyyy-MM-dd theo đặc tả.</summary>
        private static string ToDateString(long? hisTime)
        {
            try
            {
                if (!hisTime.HasValue || hisTime.Value <= 0) return null;
                string s = hisTime.Value.ToString();
                if (s.Length < 8) return null;
                return s.Substring(0, 4) + "-" + s.Substring(4, 2) + "-" + s.Substring(6, 2);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static string ReplaceSeparator(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s.Replace(';', ',');
        }

        /// <summary>
        /// Tên giới tính để tra danh mục của cổng.
        ///
        /// SO VỚI HẰNG SỐ CỦA HỆ THỐNG, KHÔNG VIẾT CỨNG SỐ. Trước đây hàm này suy ra bằng
        /// `(id == 1) ? "Nam" : "Nữ"` — mã định danh giới tính là hằng số của hệ thống, không có gì
        /// bảo đảm 1 là Nam, nên viết cứng số là đoán và đã đẩy sai (hồ sơ Nữ lên cổng thành Nam).
        /// </summary>
        private static string GenderName(KskSytHcmSource src)
        {
            HIS_PATIENT p = (src != null) ? src.Patient : null;

            // 1. Tên có sẵn trên hồ sơ bệnh nhân.
            string name = GetStr(p, "GENDER_NAME");
            if (!string.IsNullOrWhiteSpace(name)) return name.Trim();

            // 2. Tên có sẵn trên đợt điều trị — bảng đợt điều trị chép sẵn tên giới tính.
            name = GetStr((src != null) ? (object)src.Treatment : null, "TDL_PATIENT_GENDER_NAME");
            if (!string.IsNullOrWhiteSpace(name)) return name.Trim();

            // 3. Cuối cùng mới suy từ mã định danh, và so với HẰNG SỐ của hệ thống.
            long? id = GetLong(p, "GENDER_ID");
            if (!id.HasValue)
                id = GetLong((src != null) ? (object)src.Treatment : null, "TDL_PATIENT_GENDER_ID");
            if (!id.HasValue) return null;

            // 3a. TRA DANH MỤC GIỚI TÍNH CỦA HIS — nguồn đúng nhất, vì tên lấy ra chính là tên
            //     viện đang dùng, đem map sang danh mục của Sở là khớp.
            try
            {
                // Vòng lặp thường: tệp này không dùng LINQ, thêm using chỉ vì một dòng là không đáng.
                var genders = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_GENDER>();
                if (genders != null)
                {
                    foreach (HIS_GENDER g in genders)
                    {
                        if (g == null || g.ID != id.Value) continue;
                        if (!string.IsNullOrWhiteSpace(g.GENDER_NAME)) return g.GENDER_NAME.Trim();
                        break;
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

            // 3b. Danh mục chưa nạp được thì mới so với hằng số của hệ thống.
            if (id.Value == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE) return "Nam";
            if (id.Value == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE) return "Nữ";

            Inventec.Common.Logging.LogSystem.Warn("SytHcm: ma gioi tinh " + id.Value
                + " khong tra duoc trong danh muc gioi tinh cua HIS -> khong gui gioi tinh");
            return null;
        }

        #endregion
    }
}
