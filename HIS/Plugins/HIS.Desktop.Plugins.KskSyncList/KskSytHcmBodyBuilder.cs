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
using Newtonsoft.Json.Linq;
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
        /// Dịch vụ của chính đợt điều trị — nguồn cho những chỉ tiêu nối vào DỊCH VỤ (siêu âm,
        /// phẫu thuật - thủ thuật) thay vì vào chỉ số xét nghiệm. Kết quả lấy ở cột kết luận,
        /// không có thì lấy mô tả.
        /// </summary>
        public List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_2> ClsSereServs { get; set; }

        /// <summary>
        /// Kết quả của dịch vụ chẩn đoán hình ảnh / siêu âm — nằm ở bảng `HIS_SERE_SERV_EXT`
        /// (mô tả + kết luận), nối với dịch vụ qua `SERE_SERV_ID`.
        /// </summary>
        public List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> ClsExts { get; set; }

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
        private const string CAT__TINH = "DiaChiHienTai_Tinh";
        private const string CAT__NGHE_NGHIEP = "NgheNghiepId";
        private const string CAT__NOI_CONG_TAC = "NoiCongTacHocTap";
        private const string CAT__KET_LUAN_DE_NGHI = "KetLuan_DeNghi";
        private const string CAT__ICD = "ICD";
        private const string CAT__YES_NO = "Yes_No";
        private const string CAT__YES_NO_M4 = "YesNo";
        private const string CAT__DOI_TUONG_KHAM = "M3_DoiTuongKham";

        /// <summary>Nguồn chi trả: mẫu M3 dùng danh mục này (Id 4039–4042)...</summary>
        private const string CAT__CHI_TRA_M3 = "ChiTra";

        /// <summary>...còn mẫu M4 (người cao tuổi) dùng danh mục riêng, ít mục hơn (Id 5052–5054).</summary>
        private const string CAT__CHI_TRA_M4 = "NCT_HTChiTraKhamSucKhoe";

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


            // Khối hỏi bệnh chỉ có ở mẫu M4 — hồ sơ M3 giữ nguyên hình dạng bản tin như cũ.
            if (!IsElderlyForm(src)) return body;

            JObject hb = ReadInterviewJson(src);
            if (hb == null)
            {
                // Hồ sơ người cao tuổi mà tab hỏi bệnh còn trống. KHÔNG gửi khối toàn số 0: như
                // vậy là khai thay bệnh nhân rằng họ trả lời "Không" cho mọi câu.
                Inventec.Common.Logging.LogSystem.Warn(
                    "SytHcm: ho so NGUOI CAO TUOI nhung tab Hoi benh lam sang HCM chua nhap"
                    + " -> ban tin M4 THIEU khoi kham_thuc_the va hoi_benh_kham_lam_sang");
                return body;
            }

            JObject m3 = JObject.FromObject(body);
            ReshapeTienSuToM4(m3);

            // Dựng LẠI theo ĐÚNG THỨ TỰ KHỐI của đặc tả M4, thay vì gắn thêm vào cuối bản tin M3.
            JObject full = new JObject();
            full["tthc"] = m3["tthc"];
            full["tien_su"] = m3["tien_su"];
            full["kham_thuc_the"] = JObject.FromObject(BuildKhamThucThe(src, hb));
            full["hoi_benh_kham_lam_sang"] = JObject.FromObject(BuildHoiBenh(src, hb));
            full["kham_lam_san"] = RenameEarFieldsForM4(m3["kham_lam_san"] as JObject);
            full["can_lam_san"] = m3["can_lam_san"];
            full["ket_luan"] = m3["ket_luan"];

            // Trường chữ bỏ trống: gửi "" chứ KHÔNG gửi null — xem chú thích của hàm.
            NullToEmptyForTextFields(full);
            return full;
        }



        #region ===== V. Khám thực thể — mục B và C của Mẫu 4 (RIÊNG mẫu M4) =====

        /// <summary>
        /// Tiền sử gia đình theo MẪU M4 — 7 bệnh, khác hẳn danh mục của mẫu M3.
        ///
        /// KHÔNG suy từ ô tích tiền sử gia đình của mẫu M3: danh mục bên đó là Truyền nhiễm, Lao,
        /// Động kinh, Rối loạn tâm thần... thiếu hẳn Tăng huyết áp, Phổi tắc nghẽn mạn tính,
        /// Trầm cảm-lo âu; còn "Tim mạch" bên đó KHÔNG phải "Tim mạch sớm" có kèm mốc tuổi của mẫu
        /// này. Vì vậy mẫu M4 có ô nhập riêng, đọc từ INTERVIEW_JSON.
        /// </summary>
        private static readonly string[] HB__TIEN_SU_GIA_DINH = new string[]
        {
            "tsb_giadinh",
            "tsbgd_tanghuyetap", "tsbgd_daithaoduong", "tsbgd_phoi", "tsbgd_henphequan",
            "tsbgd_tramcam", "tsbgd_ungthu", "tsbgd_timmach"
        };

        /// <summary>Đọc cột INTERVIEW_JSON. Trả null khi hồ sơ chưa nhập tab hỏi bệnh.</summary>
        private static JObject ReadInterviewJson(KskSytHcmSource src)
        {
            try
            {
                if (src == null || src.SytHcm == null) return null;

                string raw = GetStr(src.SytHcm, "INTERVIEW_JSON");
                if (string.IsNullOrWhiteSpace(raw)) return null;

                return JObject.Parse(raw);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(
                    "SytHcm: cot INTERVIEW_JSON khong doc duoc -> KHONG day cac khoi rieng cua M4");
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Khối kham_thuc_the của mẫu M4:
        ///   ├ dieu_tri_benh_* / thai_san_*   (dùng lại nguồn của khối tiền sử)
        ///   ├ tien_su_benh_gia_dinh          mục B — nhập ở tab Hỏi bệnh lâm sàng HCM
        ///   ├ kham_the_luc                   mục C — trị số lấy từ bản ghi sinh hiệu
        ///   └ phanloai                       phân loại thể lực
        ///
        /// LƯU Ý HÌNH DẠNG: mẫu M4 nhét kham_the_luc VÀO TRONG kham_thuc_the và để phanloai ra
        /// ngoài nó, còn mẫu M3 để kham_the_luc ở tầng ngoài cùng và phanloai nằm bên trong. Hai
        /// mẫu không dùng chung một khối được, nên dựng riêng thay vì dùng lại BuildKhamTheLuc.
        /// </summary>
        private static Dictionary<string, object> BuildKhamThucThe(KskSytHcmSource src, JObject hb)
        {
            var b = new Dictionary<string, object>();
            HIS_KSK_OVER_EIGHTEEN ov = src.Over18;
            HIS_DHST d = src.Dhst;

            string treating = GetStr(ov, "PATHOLOGICAL_HISTORY");
            b["dieu_tri_benh_co_khong"] = YesNo(!string.IsNullOrWhiteSpace(treating));
            b["dieu_tri_benh_liet_ke"] = treating;

            string maternity = GetStr(ov, "MATERNITY_HISTORY");
            b["thai_san_co_khong"] = YesNo(!string.IsNullOrWhiteSpace(maternity));
            b["thai_san_liet_ke"] = maternity;

            b["tien_su_benh_gia_dinh"] = HoiBenhFlags(hb, HB__TIEN_SU_GIA_DINH);

            // Trị số thể lực lấy từ BẢN GHI SINH HIỆU, không nhập lại ở tab hỏi bệnh — để hai nơi
            // khỏi lệch nhau. Riêng cân nặng một năm trước là câu hỏi bệnh nhân, HIS không có chỗ
            // nào lưu nên nằm trong INTERVIEW_JSON.
            var tl = new Dictionary<string, object>();
            tl["chieucao"] = Num(GetDecimal(d, "HEIGHT"));
            tl["vongbung"] = Num(GetDecimal(d, "BELLY"));
            tl["cannang"] = Num(GetDecimal(d, "WEIGHT"));
            tl["cannang_namtruoc"] = ReadHoiBenhNum(hb, "cannang_namtruoc");
            tl["mach"] = Num(GetDecimal(d, "PULSE"));
            tl["huyetaptamthu"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MAX"));
            tl["huyetaptamtruong"] = Num(GetDecimal(d, "BLOOD_PRESSURE_MIN"));
            tl["nhiptho"] = Num(GetDecimal(d, "BREATH_RATE"));
            b["kham_the_luc"] = tl;

            b["phanloai"] = KskSytHcmPayload.ResolveSytRankId(
                GetLong(src.Over18, "DHST_RANK"), src.HisRanks);
            return b;
        }

        /// <summary>Trị số nhập tay trong INTERVIEW_JSON. Chưa nhập thì null, không gửi số 0.</summary>
        private static object ReadHoiBenhNum(JObject o, string field)
        {
            try
            {
                JToken t = (o != null) ? o[field] : null;
                if (t == null) return null;

                decimal v;
                if (!decimal.TryParse(t.ToString(), System.Globalization.NumberStyles.Any,
                        CultureInfo.InvariantCulture, out v) || v <= 0) return null;
                return Num(v);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        #endregion

        #region ===== VI. Hỏi bệnh và khám lâm sàng (mục D của Mẫu 4 — TT25) =====

        /// <summary>
        /// Ô tích của từng nhóm — cổng cần ĐỦ MẶT: không trả lời nghĩa là "Không", tức 0, chứ không
        /// phải vắng mặt. Cơ sở dữ liệu lưu gọn bằng cách bỏ khoá, bản tin thì bù lại.
        ///
        /// Danh sách phải khớp với các ô ở tab Hỏi bệnh lâm sàng HCM. Thêm câu trên giao diện mà
        /// quên thêm vào đây thì câu đó không lên cổng.
        ///
        /// 3 câu tầm soát phổi tắc nghẽn mạn tính nằm ở nhóm riêng `benh_phoi_tac_nghen`, xem
        /// HB__PHOI_TAC_NGHEN bên dưới.
        /// </summary>
        private static readonly string[] HB__THONG_TIN_BENH = new string[]
        {
            "benh_tanghuyetap", "benh_daithaoduong", "benh_phoi", "benh_hen", "benh_ungthu",
            "benh_suytim", "benh_khop", "benh_thanmam", "benh_timthieumau", "benh_nhoimau",
            "benh_dotqui", "benh_roiloan_tramcam", "benh_roiloan_loau", "benh_sasut_tritue"
        };

        /// <summary>
        /// Ô chọn nơi đang điều trị. KHÔNG có benh_nhoimau_cskcb và benh_dotqui_cskcb: đặc tả của
        /// cổng có hai trường này nhưng biểu mẫu in KHÔNG có ô nơi điều trị cho Nhồi máu cơ tim và
        /// Đột quỵ, nên giao diện không thu thập — không có dữ liệu để gửi.
        /// </summary>
        private static readonly string[] HB__THONG_TIN_BENH_CSKCB = new string[]
        {
            "benh_tanghuyetap_cskcb", "benh_daithaoduong_cskcb", "benh_phoi_cskcb",
            "benh_hen_cskcb", "benh_ungthu_cskcb", "benh_suytim_cskcb", "benh_khop_cskcb",
            "benh_thanmam_cskcb", "benh_timthieumau_cskcb", "benh_roiloan_tramcam_cskcb",
            "benh_roiloan_loau_cskcb", "benh_sasut_tritue_cskcb"
        };

        private static readonly string[] HB__DAI_THAO_DUONG = new string[]
        { "dtd_maudoi", "dtd_khatnuoc", "dtd_ditieunhieu", "dtd_sutcan", "dtd_vetthuong" };

        /// <summary>
        /// D3 — tầm soát bệnh phổi tắc nghẽn mạn tính. Cặp { tên trường của cổng, khoá trong
        /// INTERVIEW_JSON }.
        ///
        /// ĐÂY LÀ NHÓM DUY NHẤT tên hai bên KHÁC NHAU: giao diện dựng theo biểu mẫu in (tiền tố
        /// `bpq_`) từ trước khi có tài liệu của cổng, cổng lại gọi là `ptnmt_`. Đổi tên khoá trong
        /// cơ sở dữ liệu thì hồ sơ đã lưu mất dữ liệu, nên quy đổi ở đây.
        ///
        /// Cổng BẮT BUỘC cả ba (400 `required` nếu thiếu), nên luôn gửi đủ mặt, không trả lời là 0.
        /// </summary>
        private static readonly string[][] HB__PHOI_TAC_NGHEN = new string[][]
        {
            new[] { "ptnmt_ho",      "bpq_ho_hangngay" },
            new[] { "ptnmt_khacdom", "bpq_khacdam"     },
            new[] { "ptnmt_khotho",  "bpq_khotho"      }
        };

        private static readonly string[] HB__HEN_PHE_QUAN = new string[]
        {
            "hpq_khokhe", "hpq_ho_demkhuya", "hpq_ho_thucgiac", "hpq_ho_vandong",
            "hpq_hohap_theomua", "hpq_ho_chatkichthich", "hpq_dotcamlanh", "hpq_trieuchung_caithien"
        };

        private static readonly string[] HB__UNG_THU = new string[]
        {
            "ut_vetloet", "ut_hodai", "ut_ankhongtieu", "ut_thaydoi_thoiquen_ruotbongdai",
            "ut_cucu", "ut_notruoi", "ut_hachto", "ut_utai_nghetmui", "ut_sutcan",
            "ut_chaymau_dauvu", "ut_chaymau_amdao"
        };

        /// <summary>
        /// Trầm cảm VÀ lo âu nằm CHUNG một khối của cổng, dù biểu mẫu in tách thành D6 và D7.
        /// Trả lời bằng mã mức tần suất, chưa chọn thì không gửi.
        /// </summary>
        private static readonly string[] HB__ROI_LOAN_TRAM_CAM = new string[]
        {
            "rltc_ithungthu", "rltc_channan", "rltc_khongu", "rltc_metmoi", "rltc_khongngonmieng",
            "rltc_camthayte", "rltc_khotaptrung", "rltc_chamchap", "rltc_ynghi",
            "rlla_canthang", "rlla_kiemsoat_lolang", "rlla_lolang_nhieuthu", "rlla_khothugian",
            "rlla_bucrut", "rlla_bucboi", "rlla_lolang"
        };

        private static readonly string[] HB__HOAT_DONG_SONG = new string[]
        {
            "hds_tutam", "hds_tumacquanao", "hds_tudivesinh", "hds_tudichuyen_khoigiuong",
            "hds_kiemsoat_tieutieu", "hds_tuanuong"
        };

        private static readonly string[] HB__HOAT_DONG_HANG_NGAY = new string[]
        {
            "hdhn_nghedt", "hdhn_tumua_vatdung", "hdhn_tunauan", "hdhn_tulamviecnha",
            "hdhn_tugiatquanao", "hdhn_tulaixe_batxe", "hdhn_tuchia_uongthuoc", "hdhn_tugiutien"
        };

        private static readonly string[] HB__DANH_GIA_TE_NGA = new string[]
        { "dgtn_bite", "dgtn_loso_binga", "dgtn_didung" };

        private static readonly string[] HB__GIAM_NHAN_THUC = new string[]
        { "gnt_trinho_bigiam", "gnt_ghinho", "gnt_nho_noilai" };

        /// <summary>
        /// Dựng khối hỏi bệnh từ cột INTERVIEW_JSON.
        ///
        /// KHỐI NÀY LỒNG NHAU, KHÔNG PHẲNG — đúng theo đặc tả của Sở:
        ///   hoi_benh_kham_lam_sang
        ///     ├ macbenh
        ///     ├ thong_tin_benh          (14 bệnh + nơi điều trị + bệnh khác)
        ///     ├ dai_thao_duong / hen_phe_quan / ung_thu / roi_loan_tram_cam
        ///     ├ tinh_than_van_dong
        ///     │   └ hoat_dong_song / hoat_dong_hang_ngay / tinh_trang_suy_yeu
        ///     │     / danh_gia_te_nga / giam_nhan_thuc
        ///     └ dauhieu_khac
        /// Cơ sở dữ liệu vẫn lưu PHẲNG một mức trong INTERVIEW_JSON: gói lồng là hình dạng của bản
        /// tin, không phải hình dạng của dữ liệu — lưu phẳng thì đọc/ghi trên giao diện gọn hơn.
        ///
        /// Trả null khi hồ sơ chưa nhập tab này.
        /// </summary>
        private static Dictionary<string, object> BuildHoiBenh(KskSytHcmSource src, JObject o)
        {
            HIS_KSK_SYT_HCM h = src.SytHcm;

            var thongTinBenh = HoiBenhFlags(o, HB__THONG_TIN_BENH);
            HoiBenhAddIds(thongTinBenh, o, HB__THONG_TIN_BENH_CSKCB);
            thongTinBenh["benh_khac_hoibenh"] = GetStr(h, "INTERVIEW_OTHER_DISEASE");

            var suyYeu = HoiBenhFlags(o, new string[] { "ttsy_khokhan_leothang", "ttsy_khokhan_dibo" });
            HoiBenhAddIds(suyYeu, o, new string[] { "ttsy_metmoi" });

            var tinhThanVanDong = new Dictionary<string, object>();
            tinhThanVanDong["hoat_dong_song"] = HoiBenhFlags(o, HB__HOAT_DONG_SONG);
            tinhThanVanDong["hoat_dong_hang_ngay"] = HoiBenhFlags(o, HB__HOAT_DONG_HANG_NGAY);
            tinhThanVanDong["tinh_trang_suy_yeu"] = suyYeu;
            tinhThanVanDong["danh_gia_te_nga"] = HoiBenhFlags(o, HB__DANH_GIA_TE_NGA);
            tinhThanVanDong["giam_nhan_thuc"] = HoiBenhFlags(o, HB__GIAM_NHAN_THUC);

            var b = new Dictionary<string, object>();
            b["macbenh"] = ReadHoiBenhFlag(o, "macbenh");
            b["thong_tin_benh"] = thongTinBenh;
            b["dai_thao_duong"] = HoiBenhFlags(o, HB__DAI_THAO_DUONG);
            b["benh_phoi_tac_nghen"] = HoiBenhFlagsRenamed(o, HB__PHOI_TAC_NGHEN);
            b["hen_phe_quan"] = HoiBenhFlags(o, HB__HEN_PHE_QUAN);
            b["ung_thu"] = HoiBenhFlags(o, HB__UNG_THU);
            b["roi_loan_tram_cam"] = HoiBenhIds(o, HB__ROI_LOAN_TRAM_CAM);
            b["tinh_than_van_dong"] = tinhThanVanDong;
            b["dauhieu_khac"] = GetStr(h, "INTERVIEW_OTHER_SIGN");

            Inventec.Common.Logging.LogSystem.Warn("SytHcm/HoiBenh: da dung khoi hoi benh long nhau");
            return b;
        }

        /// <summary>Một nhóm ô tích: gửi đủ mặt, không trả lời thì 0.</summary>
        private static Dictionary<string, object> HoiBenhFlags(JObject o, string[] fields)
        {
            var b = new Dictionary<string, object>();
            foreach (string f in fields) b[f] = ReadHoiBenhFlag(o, f);
            return b;
        }

        /// <summary>Như HoiBenhFlags nhưng tên trường của cổng khác khoá lưu trong cơ sở dữ liệu.</summary>
        private static Dictionary<string, object> HoiBenhFlagsRenamed(JObject o, string[][] pairs)
        {
            var b = new Dictionary<string, object>();
            foreach (string[] p in pairs) b[p[0]] = ReadHoiBenhFlag(o, p[1]);
            return b;
        }

        /// <summary>Một nhóm ô chọn: chưa chọn thì KHÔNG gửi — 0 ở đây là một mã danh mục có thật.</summary>
        private static Dictionary<string, object> HoiBenhIds(JObject o, string[] fields)
        {
            var b = new Dictionary<string, object>();
            HoiBenhAddIds(b, o, fields);
            return b;
        }

        private static void HoiBenhAddIds(Dictionary<string, object> b, JObject o, string[] fields)
        {
            foreach (string f in fields)
            {
                long? v = ReadHoiBenhId(o, f);
                if (v.HasValue) b[f] = v.Value;
            }
        }

        /// <summary>Ô tích: có khoá và bằng 1 thì 1, còn lại là 0.</summary>
        /// <summary>
        /// Đọc một Id (kiểu số) từ chuỗi JSON hỏi bệnh. Không có khoá, hoặc giá trị không phải số
        /// dương, thì trả null — chỗ gọi hiểu là KHÔNG GỬI trường đó.
        /// </summary>
        private static long? ReadInterviewLong(JObject o, string field)
        {
            try
            {
                if (o == null) return null;
                JToken t = o[field];
                if (t == null || t.Type == JTokenType.Null) return null;
                long v;
                if (!long.TryParse(t.ToString(), out v) || v <= 0) return null;
                return v;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private static int ReadHoiBenhFlag(JObject o, string field)
        {
            try
            {
                JToken t = o[field];
                if (t == null) return 0;
                int v;
                return (int.TryParse(t.ToString(), out v) && v == 1) ? 1 : 0;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return 0; }
        }

        /// <summary>Ô chọn: trả mã đã chọn, chưa chọn thì null.</summary>
        private static long? ReadHoiBenhId(JObject o, string field)
        {
            try
            {
                JToken t = o[field];
                if (t == null) return null;
                long v;
                return (long.TryParse(t.ToString(), out v) && v > 0) ? (long?)v : null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        #endregion


        /// <summary>
        /// Hồ sơ này đẩy theo MẪU M4 (khám sức khoẻ người cao tuổi) hay M3.
        ///
        /// CĂN CỨ LÀ "ĐỐI TƯỢNG KHÁM", KHÔNG PHẢI TUỔI. Ba lý do:
        ///   - Cổng đã mô hình hoá sẵn: "Người cao tuổi" là một mục của danh mục M3_DoiTuongKham,
        ///     người dùng chọn trên màn nhập và đã lưu vào cột SYT_PATIENT_TYPES.
        ///   - Đủ 60 tuổi không có nghĩa là khám theo chương trình người cao tuổi: một người 65
        ///     tuổi còn đi làm, khám định kỳ theo diện người lao động thì vẫn là M3.
        ///   - Trừ tuổi không đáng tin với đúng nhóm này: HIS có cờ IS_HAS_NOT_DAY_DOB cho bệnh
        ///     nhân không nhớ ngày tháng sinh, mà người cao tuổi lại là nhóm hay chỉ có năm sinh.
        ///
        /// KHÔNG VIẾT CỨNG MÃ ĐỊNH DANH: tra tên trong danh mục của cổng, như phần giới tính.
        /// Danh mục chưa tải về thì trả false — đẩy M3 như cũ, kèm cảnh báo, hơn là đoán.
        /// </summary>
        internal static bool IsElderlyForm(KskSytHcmSource src)
        {
            try
            {
                if (src == null || src.SytHcm == null) return false;

                string types = GetStr(src.SytHcm, "SYT_PATIENT_TYPES");
                if (string.IsNullOrWhiteSpace(types)) return false;

                long? nctId = MapByName(CAT__DOI_TUONG_KHAM, "Người cao tuổi");
                if (!nctId.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "SytHcm: chua tra duoc ma cua doi tuong kham 'Nguoi cao tuoi' trong danh muc "
                        + CAT__DOI_TUONG_KHAM + " -> day theo mau M3");
                    return false;
                }

                foreach (string part in types.Split(';', ','))
                {
                    long v;
                    if (long.TryParse(part.Trim(), out v) && v == nctId.Value) return true;
                }
                return false;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return false; }
        }


        /// <summary>
        /// Nắn riêng khối `tien_su` từ hình dạng M3 sang M4.
        ///
        /// Làm ở một chỗ như thế này thay vì sửa BuildTienSu, để LUỒNG M3 KHÔNG BỊ ĐỘNG VÀO — bốn
        /// viện đang chạy M3 không phải kiểm tra lại.
        ///
        /// Khối `tien_su` của hai mẫu khác hẳn nhau:
        ///       M3: giadinh_macbenh = cờ 0/1, danh sách mã nằm ở giadinh_danhsachbenh,
        ///           kèm giadinh_macbenh_tenbenh.
        ///       M4: giadinh_macbenh CHÍNH LÀ chuỗi danh sách mã ("1051,1053");
        ///           KHÔNG có giadinh_danhsachbenh, KHÔNG có giadinh_macbenh_tenbenh.
        /// </summary>
        private static void ReshapeTienSuToM4(JObject full)
        {
            try
            {
                if (full == null) return;

                JObject tienSu = full["tien_su"] as JObject;
                if (tienSu == null) return;

                JToken ids = tienSu["giadinh_danhsachbenh"];
                bool coMa = (ids != null && ids.Type != JTokenType.Null
                             && !string.IsNullOrWhiteSpace(ids.ToString()));

                tienSu["giadinh_macbenh"] = coMa ? ids.ToString() : "";
                tienSu.Remove("giadinh_danhsachbenh");
                tienSu.Remove("giadinh_macbenh_tenbenh");
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
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
            b["yeu_to_nhom_mau_id"] = MapRhFactor(GetStr(p, "BLOOD_RH_CODE"));
            b["dia_chi_hien_tai"] = FirstStr(p, "VIR_HT_ADDRESS", "HT_ADDRESS", "VIR_ADDRESS", "ADDRESS");

            // Id VÀ MÃ phải lấy CÙNG MỘT MỤC trong danh mục của Sở. Lấy mã của HIS thì cổng báo
            // "NgheNghiepCode không tồn tại trong danh mục nghề nghiệp" — hai hệ mã khác nhau.
            KskSytCatalogItem ward = MapByNameItem(CAT__XA_PHUONG,
                FirstStr(p, "HT_COMMUNE_NAME", "COMMUNE_NAME"));
            b["ward_id"] = ToLongOrNull(ward != null ? ward.Id : null);
            b["ward_code"] = (ward != null) ? ward.Code : null;

            // TỈNH / THÀNH PHỐ — tra theo MÃ trước, theo TÊN sau, cùng cách với nghề nghiệp.
            //
            // Mã tỉnh trong danh mục của cổng có 3 chữ số kèm số 0 ở đầu ("079"), còn HIS có nơi lưu
            // "79" — nên thử cả dạng đã thêm số 0 cho đủ 3 chữ số, nếu không sẽ trượt hết.
            KskSytCatalogItem city = MapCityItem(
                FirstStr(p, "HT_PROVINCE_CODE", "PROVINCE_CODE"),
                FirstStr(p, "HT_PROVINCE_NAME", "PROVINCE_NAME"));
            b["city_id"] = ToLongOrNull(city != null ? city.Id : null);
            b["city_code"] = (city != null) ? city.Code : null;

            // NGHỀ NGHIỆP — tra theo MÃ trước, theo TÊN sau.
            //
            // Danh mục của cổng có mã cho cả 792 mục, dạng 5 chữ số ("10110", "17210") — đúng bộ mã
            // nghề nghiệp chuẩn mà danh mục của viện cũng dùng. Bằng chứng: cổng QĐ 1551 lấy 2 ký tự
            // ĐẦU của `TDL_PATIENT_CAREER_CODE` làm mã nhóm nghề, tức mã của HIS dài hơn 2 ký tự và
            // cùng hệ.
            //
            // Tra theo mã chắc hơn hẳn tra theo tên: tên cùng một nghề mỗi nơi ghi một kiểu, chỉ lệch
            // một dấu cách hay một chữ viết hoa là trượt.
            string careerCode = FirstStr(src.Treatment, "TDL_PATIENT_CAREER_CODE");
            KskSytCatalogItem career = MapByNameItem(CAT__NGHE_NGHIEP, careerCode, false);
            if (career == null)
                career = MapByNameItem(CAT__NGHE_NGHIEP, GetStr(p, "CAREER_NAME"), false);

            if (career != null)
            {
                // Id VÀ mã lấy CÙNG MỘT MỤC của danh mục cổng — ghép lẫn là cổng từ chối.
                b["nghenghiep_id"] = ToLongOrNull(career.Id);
                b["nghenghiep_code"] = career.Code;
            }
            else
            {
                // KHÔNG gửi giá trị bịa. Trước đây tra không ra thì gửi cứng một nghề có thật cho
                // qua cửa cổng — nhưng như vậy mọi bệnh nhân tra hụt đều thành cùng một nghề, trên
                // cổng nhìn y như dữ liệu thật nên không ai phát hiện (cùng lý do đã bỏ nơi công tác).
                Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong tra duoc nghe nghiep — ma HIS=\""
                    + (careerCode ?? "") + "\", ten=\"" + (GetStr(p, "CAREER_NAME") ?? "")
                    + "\" -> KHONG gui nghe nghiep");
            }

            // NƠI CÔNG TÁC là Id trong danh mục của cổng, KHÔNG phải chữ. Bảng chi tiết ghi kiểu
            // chuỗi 2000 ký tự nhưng thực tế cổng chỉ nhận Id — gửi chữ là bị từ chối.
            //
            // TRA ĐƯỢC THÌ GỬI, KHÔNG THÌ ĐỂ TRỐNG. Trước đây không tra được thì gửi Id 1 cho khỏi
            // bị cổng chê thiếu — nhưng như vậy MỌI bệnh nhân của viện đều thành cùng một nơi làm
            // việc, nhìn trên cổng y như dữ liệu thật nên không ai phát hiện ra. Thà để trống và bị
            // cổng chê, còn hơn gửi dữ liệu bịa (quyết định của người yêu cầu).
            //
            // Danh mục nơi công tác của cổng có 5431 mục là tên cơ sở cụ thể, còn HIS lưu nơi công
            // tác dạng chữ tự do nên phần lớn hồ sơ sẽ không tra ra. Muốn gửi đúng thì phải bổ sung
            // ô chọn nơi công tác lấy thẳng danh mục của cổng, như đã làm cho ô chọn bệnh.
            // Người nhập CHỌN THẲNG trong danh mục của cổng ở màn hình nhập KSK, giá trị lưu
            // trong cột INTERVIEW_JSON. Bỏ hẳn cách tra theo tên: danh mục của cổng hơn 5.000 mục
            // tên cơ sở cụ thể, còn HIS lưu nơi làm việc dạng chữ tự do nên tra tên gần như luôn
            // trượt — và trước đó còn đọc nhầm sang bảng KSK (cột WORK_PLACE nằm ở HIS_PATIENT).
            //
            // `noi_cong_tac_xa_phuong` dùng chung danh mục xã/phường với địa chỉ bệnh nhân: bộ
            // Postman của Sở liệt kê 26 danh mục và không có danh mục riêng cho xã/phường nơi
            // công tác.
            JObject hbTthc = ReadInterviewJson(src);
            long? noiCongTac = ReadInterviewLong(hbTthc, "noi_cong_tac");
            if (noiCongTac.HasValue) b["noi_cong_tac"] = noiCongTac.Value;

            long? noiCongTacXa = ReadInterviewLong(hbTthc, "noi_cong_tac_xa_phuong");
            if (noiCongTacXa.HasValue) b["noi_cong_tac_xa_phuong"] = noiCongTacXa.Value;

            b["hinh_thuc_chi_tra_khamsk"] = MapPaySourceForForm(GetLong(h, "SYT_PAYSOURCE_ID"), src);
            b["hinh_thuc_chi_tra_khamsk_chi_tiet"] = GetLong(h, "SYT_PAY_SOURCE_DETAIL_ID");
            b["nguonkhac_ghiro"] = GetStr(h, "PAY_SOURCE_OTHER");
            b["ly_do_kham"] = GetStr(src.ServiceReq, "HOSPITALIZATION_REASON");
            return b;
        }

        /// <summary>
        /// Quy đổi Nguồn chi trả sang danh mục ĐÚNG CỦA TỪNG MẪU.
        ///
        /// VÌ SAO: ô "Nguồn chi trả" ở màn hình nhập luôn đổ theo danh mục `ChiTra` (mẫu M3, Id
        /// 4039–4042). Dịch vụ M4 lại đối chiếu danh mục riêng của người cao tuổi
        /// `NCT_HTChiTraKhamSucKhoe` (Id 5052–5054) — tuy thông báo lỗi vẫn gọi là "danh mục
        /// ChiTra". Gửi thẳng Id của M3 thì cổng trả 400 `category_not_found`.
        ///
        /// CÁCH LÀM: tra TÊN của mục đã chọn trong danh mục M3, rồi tìm đúng tên đó ở danh mục M4.
        /// Không viết cứng cặp Id, vì Id do cổng cấp và có thể đổi.
        ///
        /// Mẫu M3 giữ nguyên hành vi cũ: trả lại đúng Id đã lưu.
        /// </summary>
        private static long? MapPaySourceForForm(long? paySourceId, KskSytHcmSource src)
        {
            try
            {
                if (!paySourceId.HasValue || !IsElderlyForm(src)) return paySourceId;

                string name = null;
                var items = KskSytHcmPayload.ReadCachedCatalog(CAT__CHI_TRA_M3);
                if (items != null)
                {
                    foreach (var it in items)
                    {
                        if (it == null) continue;
                        if (ToLongOrNull(it.Id) == paySourceId) { name = it.Name; break; }
                    }
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong tra duoc ten cua nguon chi tra Id="
                        + paySourceId.Value + " trong danh muc " + CAT__CHI_TRA_M3
                        + " -> mau M4 gui trong o nguon chi tra");
                    return null;
                }

                long? m4Id = MapByName(CAT__CHI_TRA_M4, name);
                if (!m4Id.HasValue)
                {
                    // Danh mục của người cao tuổi ÍT MỤC HƠN (không có "Người sử dụng lao động chi
                    // trả"). Chọn phải mục đó thì không có gì để quy đổi -> để trống và báo rõ, hơn
                    // là tự đổi sang mục khác làm sai thông tin hành chính.
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: nguon chi tra \"" + name
                        + "\" KHONG co trong danh muc " + CAT__CHI_TRA_M4 + " cua mau M4"
                        + " -> gui trong; can chon lai nguon chi tra khac cho ho so nguoi cao tuoi");
                    return null;
                }

                Inventec.Common.Logging.LogSystem.Info("SytHcm: mau M4 quy doi nguon chi tra \"" + name
                    + "\": " + paySourceId.Value + " (" + CAT__CHI_TRA_M3 + ") -> " + m4Id.Value
                    + " (" + CAT__CHI_TRA_M4 + ")");
                return m4Id;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return paySourceId; }
        }

        /// <summary>II. Tiền sử.</summary>
        private static Dictionary<string, object> BuildTienSu(KskSytHcmSource src)
        {
            var b = new Dictionary<string, object>();
            HIS_KSK_OVER_EIGHTEEN o = src.Over18;

            string family = GetStr(o, "PATHOLOGICAL_HISTORY_FAMILY");
            // TIỀN SỬ GIA ĐÌNH — 4 chỉ tiêu, tất cả đã có chỗ lưu trong HIS kể từ khi màn hình
            // nhập đổi ô chữ thành danh sách ô tích lấy từ danh mục của cổng.
            //
            //   giadinh_danhsachbenh      <- danh sách mã định danh bệnh trong danh mục của cổng
            //   giadinh_danhsachbenh_icd  <- danh sách mã định danh trong danh mục ICD của cổng
            //   giadinh_macbenh_tenbenh   <- tên các bệnh đã chọn ở ô mã ICD
            //   giadinh_macbenh           <- có/không, suy ra từ ba chỉ tiêu trên
            string famIds = ReplaceSeparator(family);
            string famIcdIds = ReplaceSeparator(GetStr(src.General, "FAMILY_HISTORY_ICD_CODE"));
            string famIcdNames = GetStr(src.General, "FAMILY_HISTORY_ICD_NAME");

            b["giadinh_danhsachbenh"] = famIds;
            b["giadinh_danhsachbenh_icd"] = famIcdIds;
            b["giadinh_macbenh_tenbenh"] = famIcdNames;
            b["giadinh_macbenh"] = YesNo(!string.IsNullOrWhiteSpace(famIds)
                || !string.IsNullOrWhiteSpace(famIcdIds)
                || !string.IsNullOrWhiteSpace(famIcdNames));

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

            // Từ chối khám sản khoa / phụ khoa — cả bản mẫu M3 lẫn M4 đều có hai chỉ tiêu này.
            // Chưa tích thì gửi 0: cổng cần biết "có khám, không từ chối", khác hẳn với vắng mặt.
            b["sankhoa_tuchoikham"] = (GetLong(h, "IS_REFUSE_OBSTETRIC") == 1) ? 1 : 0;
            b["phukhoa_tuchoikham"] = (GetLong(h, "IS_REFUSE_GYNECOLOGY") == 1) ? 1 : 0;

            b["chi_tiet_kham_rang"] = BuildChiTietKhamRang(h);
            return b;
        }

        /// <summary>
        /// Tên các trường KIỂU CHỮ của mẫu M4. Bỏ trống thì gửi chuỗi rỗng, KHÔNG gửi null.
        ///
        /// VÌ SAO: bản mẫu của Sở có 52 trường kiểu chữ và KHÔNG trường nào là null — không có dữ
        /// liệu thì để "". Null chỉ xuất hiện ở trường kiểu số/Id.
        ///
        /// Đối chiếu hai bản tin thật cho thấy đúng chỗ này: bản cổng NHẬN có `sdt`,
        /// `dieu_tri_benh_liet_ke`, `thai_san_liet_ke`, `benh_khac` đều là chuỗi; bản cổng ném lỗi
        /// `internal_error` "Tạo phiếu thất bại" thì cả sáu trường đó đều null, còn bộ khóa hai bên
        /// giống hệt nhau (299 = 299). Cổng báo `field: null` — tức không chết ở tầng kiểm tra
        /// từng trường mà chết lúc xử lý, đúng kiểu vấp phải chuỗi null.
        ///
        /// CHỈ ÁP DỤNG CHO M4. Mẫu M3 vẫn gửi null như cũ và vẫn chạy được — không đụng vào để
        /// khỏi phải kiểm tra hồi quy phần đang chạy.
        /// </summary>
        private static readonly HashSet<string> M4__TRUONG_KIEU_CHU = new HashSet<string>(
            new string[]
            {
                // I. Thông tin hành chính
                "sdt", "the_bhyt", "nguonkhac_ghiro", "ly_do_kham", "dia_chi_hien_tai",
                "ho_ten", "dinh_danh_ca_nhan", "ngay_kham", "ngay_sinh",
                "nghenghiep_code", "city_code", "ward_code",
                // II. Tiền sử + III. Khám thực thể
                "dieu_tri_benh_liet_ke", "thai_san_liet_ke", "benh_khac",
                "giadinh_danhsachbenh_icd", "giadinh_macbenh",
                // IV. Hỏi bệnh
                "dauhieu_khac", "benh_khac_hoibenh",
                // V. Cận lâm sàng + VI. Kết luận
                "xet_nghiem_hpv", "xet_nghiem_te_bao_co_tu_cung",
                "can_lam_sang_khac_chi_tiet", "de_nghi"
            });

        /// <summary>
        /// Duyệt cả bản tin, trường nào thuộc nhóm kiểu chữ mà đang null thì đổi thành "".
        /// Mọi trường chẩn đoán (`*_icd`) cũng là kiểu chữ nên gộp bằng phần đuôi tên.
        /// </summary>
        private static void NullToEmptyForTextFields(JToken node)
        {
            try
            {
                JObject o = node as JObject;
                if (o == null) return;

                foreach (JProperty pr in o.Properties())
                {
                    if (pr.Value is JObject) { NullToEmptyForTextFields(pr.Value); continue; }
                    if (pr.Value == null || pr.Value.Type != JTokenType.Null) continue;

                    if (M4__TRUONG_KIEU_CHU.Contains(pr.Name)
                        || pr.Name.EndsWith("_icd", StringComparison.Ordinal))
                    {
                        pr.Value = new JValue(string.Empty);
                    }
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Bốn chỉ tiêu thính lực: mẫu M3 gọi `tmh_taitrai_noithuong`, mẫu M4 gọi `taitrai_noithuong`
        /// — KHÔNG có tiền tố. Đối chiếu hai bản mẫu của Sở thì đúng là hai tên khác nhau.
        ///
        /// Khối `kham_lam_san` của M4 lấy lại từ thân bản tin M3, nên phải đổi tên bốn khoá này;
        /// để nguyên thì cổng M4 không nhận được thính lực mà cũng không báo lỗi.
        /// </summary>
        private static JToken RenameEarFieldsForM4(JObject khamLamSan)
        {
            try
            {
                if (khamLamSan == null) return khamLamSan;
                string[] names = new string[]
                { "taitrai_noithuong", "taitrai_noitham", "taiphai_noithuong", "taiphai_noitham" };

                foreach (string n in names)
                {
                    JToken v = khamLamSan["tmh_" + n];
                    if (v == null) continue;
                    khamLamSan[n] = v;
                    khamLamSan.Remove("tmh_" + n);
                }
                return khamLamSan;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return khamLamSan; }
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
                    if (!v.HasValue || v.Value <= 0) continue;

                    long? id = MapToothStatusToCatalogId(v.Value);
                    if (id.HasValue) teeth[no] = id.Value;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return teeth;
        }

        /// <summary>Danh mục tình trạng răng của cổng.</summary>
        private const string CAT__TINH_TRANG_RANG = "NenTangKSK_TinhTrangRang";

        /// <summary>
        /// Bảng mã trạng thái răng CŨ của HIS (màn hình nhập dùng khi cổng chưa cấp danh mục).
        /// Chỉ dùng để ĐỌC LẠI hồ sơ lưu bằng bản cũ — không phải bảng mã chính thức.
        /// </summary>
        private static readonly Dictionary<long, string> TOOTH_LEGACY_NAME = new Dictionary<long, string>
        {
            { 1,  "Bình thường"       },
            { 2,  "Sâu"               },
            { 3,  "Trám sâu lại"      },
            { 4,  "Trám tốt"          },
            { 5,  "Mất do sâu"        },
            { 6,  "Mất lý do khác"    },
            { 7,  "Bít hố rãnh"       },
            { 8,  "Trụ, cầu, implant" },
            { 9,  "Chưa mọc"          },
            { 10, "Không ghi nhận"    }
        };

        /// <summary>
        /// Đưa trạng thái răng đã lưu về ĐÚNG Id của danh mục cổng.
        ///
        /// VÌ SAO: màn hình nhập bản cũ ghi mã nội bộ của HIS (1..10) thay vì Id của cổng
        /// (191..221), nên cổng trả 400 "Trạng thái của các răng sau không tồn tại trong danh mục".
        /// Bản mới đã ghi đúng Id, nhưng hồ sơ lưu trước đó vẫn còn mã cũ trong cơ sở dữ liệu —
        /// quy đổi THEO TÊN ngay lúc đẩy để không phải mở lại và lưu lại từng hồ sơ.
        ///
        /// Không quy đổi được thì KHÔNG GỬI chiếc răng đó, hơn là gửi mã cổng không hiểu.
        /// </summary>
        private static long? MapToothStatusToCatalogId(long stored)
        {
            try
            {
                var items = KskSytHcmPayload.ReadCachedCatalog(CAT__TINH_TRANG_RANG);
                if (items == null || items.Count == 0)
                {
                    // Chưa tải được danh mục -> giữ nguyên như trước, đừng tự ý bỏ dữ liệu.
                    return stored;
                }

                foreach (var it in items)
                {
                    if (it != null && ToLongOrNull(it.Id) == stored) return stored;   // đã là Id đúng
                }

                string name;
                if (!TOOTH_LEGACY_NAME.TryGetValue(stored, out name))
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: trang thai rang " + stored
                        + " khong co trong danh muc " + CAT__TINH_TRANG_RANG
                        + " va cung khong phai ma cu cua HIS -> KHONG gui chiec rang nay");
                    return null;
                }

                long? id = MapByName(CAT__TINH_TRANG_RANG, name);
                if (!id.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: trang thai rang cu \"" + name
                        + "\" khong tra duoc trong danh muc " + CAT__TINH_TRANG_RANG
                        + " -> KHONG gui chiec rang nay");
                    return null;
                }

                Inventec.Common.Logging.LogSystem.Info("SytHcm: quy doi trang thai rang \"" + name
                    + "\": " + stored + " (ma cu cua HIS) -> " + id.Value + " (Id cua cong)");
                return id;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return stored; }
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
                Dictionary<string, string> valueByServiceCode =
                    ServiceResultValues(src.ClsSereServs, src.ClsExts);

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
                else if (valueByIndexCode.Count == 0 && valueByServiceCode.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: ho so khong co ket qua xet nghiem"
                        + " va cung khong co ket qua dich vu nao -> 34 chi tieu xet nghiem gui rong");
                }
                else
                {
                    int filled = 0, noResult = 0;
                    foreach (var kv in map)
                    {
                        string fieldCode = kv.Key;

                        // Giá trị bảng khai báo có dạng "XN|<mã>" hoặc "DV|<mã>".
                        string kind = ADO.KskSytClsFieldStore.SOURCE_KIND__TEST_INDEX;
                        string indexCode = kv.Value ?? "";
                        int vach = indexCode.IndexOf('|');
                        if (vach > 0)
                        {
                            kind = indexCode.Substring(0, vach);
                            indexCode = indexCode.Substring(vach + 1);
                        }
                        bool laDichVu = (kind == ADO.KskSytClsFieldStore.SOURCE_KIND__SERVICE);
                        string tenNguon = laDichVu ? "ma dich vu" : "ma chi so";

                        string raw;
                        bool coKetQua = laDichVu
                            ? valueByServiceCode.TryGetValue(NormCode(indexCode), out raw)
                            : valueByIndexCode.TryGetValue(NormCode(indexCode), out raw);
                        if (!coKetQua || string.IsNullOrWhiteSpace(raw))
                        {
                            // Ghi ro ma MUON ma khong thay -> doi chieu voi danh sach ma CO ket qua
                            // o dong log tren la biet ngay lech ma hay benh nhan khong lam dich vu do.
                            Inventec.Common.Logging.LogSystem.Warn("SytHcm/CLS: " + fieldCode
                                + " <- " + tenNguon + " \"" + indexCode + "\" KHONG co ket qua");
                            noResult++;
                            continue;
                        }

                        Inventec.Common.Logging.LogSystem.Debug("SytHcm/CLS: " + fieldCode
                            + " <- " + tenNguon + " " + indexCode + " = " + raw);

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

                    // Giá trị mang theo LOẠI NGUỒN: "XN|<mã chỉ số>" hoặc "DV|<mã dịch vụ>".
                    // Bản khai báo cũ không ghi loại -> hiểu là chỉ số xét nghiệm, như trước.
                    string kind = string.IsNullOrWhiteSpace(it.SourceKind)
                        ? ADO.KskSytClsFieldStore.SOURCE_KIND__TEST_INDEX
                        : it.SourceKind.Trim();
                    rs[it.FieldCode.Trim()] = kind + "|" + it.TestIndexCode.Trim();
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            return rs;
        }

        /// <summary>
        /// Kết quả của DỊCH VỤ trong hồ sơ -> tra theo MÃ DỊCH VỤ.
        ///
        /// Ưu tiên cột kết luận, trống thì lấy mô tả — hai chỉ tiêu "X-quang nhũ" và "Siêu âm 02
        /// tuyến vú" của cổng nhận văn bản tự do nên kết luận là thứ sát nghĩa nhất.
        ///
        /// Một mã có thể xuất hiện nhiều lần (làm lại dịch vụ): giữ giá trị KHÔNG RỖNG ĐẦU TIÊN,
        /// cùng quy tắc với phần chỉ số xét nghiệm.
        /// </summary>
        private static Dictionary<string, string> ServiceResultValues(
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_2> sereServs,
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT> exts)
        {
            var rs = new Dictionary<string, string>();
            try
            {
                if (sereServs == null || exts == null) return rs;

                // Bảng kết quả tra theo mã dịch vụ đã chỉ định, KHÔNG phải theo mã dịch vụ.
                var extBySereServ = new Dictionary<long, MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>();
                foreach (var ex in exts)
                {
                    if (ex == null) continue;
                    if (!extBySereServ.ContainsKey(ex.SERE_SERV_ID)) extBySereServ[ex.SERE_SERV_ID] = ex;
                }

                foreach (var ss in sereServs)
                {
                    if (ss == null || string.IsNullOrWhiteSpace(ss.TDL_SERVICE_CODE)) continue;

                    MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT ext;
                    if (!extBySereServ.TryGetValue(ss.ID, out ext) || ext == null) continue;

                    // Kết luận là thứ sát nghĩa nhất với ô văn bản của cổng; trống thì lấy mô tả.
                    string val = ext.CONCLUDE;
                    if (string.IsNullOrWhiteSpace(val)) val = ext.DESCRIPTION;
                    if (string.IsNullOrWhiteSpace(val)) continue;

                    string key = NormCode(ss.TDL_SERVICE_CODE);
                    if (!rs.ContainsKey(key)) rs[key] = val;
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
            new[] { "huyetaptamthu",    "kham_the_luc", "huyết áp tâm thu ở phần sinh hiệu" },
            new[] { "huyetaptamtruong", "kham_the_luc", "huyết áp tâm trương ở phần sinh hiệu" },
            new[] { "chieucao",         "kham_the_luc", "chiều cao ở phần sinh hiệu" },
            new[] { "cannang",          "kham_the_luc", "cân nặng ở phần sinh hiệu" },
            new[] { "phanloai",         "kham_the_luc", "phân loại thể lực ở phần sinh hiệu" }
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
        /// Yeu to Rh cua HIS -> Id trong danh muc cua cong.
        ///
        /// HIS luu dau "+" / "-" (xac nhan cua nguoi yeu cau), con danh muc cua cong ghi "Rh+" /
        /// "Rh-" nen so thang la truot, va truot thi ho so len cong bi thieu yeu to Rh ma khong co
        /// canh bao nao. Nhan them cac cach viet thuong gap khac de khoi phai sua lai khi gap vien
        /// luu kieu khac.
        /// </summary>
        private static object MapRhFactor(string code)
        {
            try
            {
                string v = Norm(code);
                if (v.Length == 0) return null;

                if (v == "+" || v == "rh+" || v == "pos" || v == "positive" || v == "duong" || v == "1")
                    v = "rh+";
                else if (v == "-" || v == "rh-" || v == "neg" || v == "negative" || v == "am" || v == "0")
                    v = "rh-";

                long? id = MapByName(CAT__YEU_TO_NHOM_MAU, v);
                if (id.HasValue) return id.Value;

                Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong quy doi duoc yeu to Rh \""
                    + (code ?? "") + "\" -> KHONG gui yeu_to_nhom_mau_id");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Tra tinh / thanh pho trong danh muc cua cong: theo MA truoc, theo TEN sau.
        ///
        /// Ma trong danh muc cua cong la 3 chu so co so 0 o dau ("079" = TP.HCM), con HIS tuy noi
        /// luu "79" hoac "079" — nen thu ca dang da them so 0 cho du 3 chu so.
        /// Tra theo ten de du phong, vi ten tinh moi noi ghi mot kieu nen kem chac hon tra theo ma.
        /// </summary>
        private static KskSytCatalogItem MapCityItem(string code, string name)
        {
            try
            {
                string c = (code ?? "").Trim();
                if (c.Length > 0)
                {
                    KskSytCatalogItem hit = MapByNameItem(CAT__TINH, c, false);
                    if (hit == null && c.Length < 3)
                        hit = MapByNameItem(CAT__TINH, c.PadLeft(3, '0'), false);
                    if (hit == null && c.Length > 1)
                        hit = MapByNameItem(CAT__TINH, c.TrimStart('0'), false);
                    if (hit != null) return hit;
                }

                KskSytCatalogItem byName = MapByNameItem(CAT__TINH, name, false);
                if (byName != null) return byName;

                Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong tra duoc tinh/thanh pho - ma HIS=\""
                    + c + "\", ten=\"" + (name ?? "") + "\" -> KHONG gui city_id/city_code");
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Quy đổi theo TÊN nhưng trả về CẢ MỤC danh mục, để nơi gọi lấy được Id và mã của cùng
        /// một mục. Danh mục nghề nghiệp và xã/phường của Sở có mã riêng, gửi lẫn mã của HIS là
        /// cổng từ chối.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, KskSytCatalogItem>> byNameItemCache
            = new Dictionary<string, Dictionary<string, KskSytCatalogItem>>();

        internal static KskSytCatalogItem MapByNameItem(string catalogCode, string hisName)
        {
            return MapByNameItem(catalogCode, hisName, true);
        }

        /// <summary>
        /// <paramref name="warnWhenMissing"/> = false khi đây mới là MỘT TRONG NHIỀU cách tra và còn
        /// cách khác phía sau (ví dụ tra mã "91" hụt thì thử tiếp "091"). Chỗ gọi tự ghi cảnh báo
        /// khi đã thử hết mà vẫn không ra.
        ///
        /// VÌ SAO: trước đây mỗi lần thử hụt đều ghi một dòng cảnh báo, nên nhật ký báo "không tìm
        /// được 91 trong danh mục tỉnh" NGAY CẢ KHI lần thử sau đó đã ra đúng tỉnh. Đọc nhật ký
        /// tưởng gửi thiếu tỉnh, mà bản tin thật ra gửi đủ.
        /// </summary>
        internal static KskSytCatalogItem MapByNameItem(string catalogCode, string hisName,
            bool warnWhenMissing)
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

                if (warnWhenMissing)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SytHcm: khong tim duoc \"" + hisName
                        + "\" trong danh muc " + catalogCode + " -> gui trong ca Id va ma");
                }
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

        /// <summary>
        /// Đọc một trị số từ bản ghi.
        ///
        /// KHÔNG đi vòng qua chuỗi. Trước đây hàm này gọi `v.ToString()` rồi đọc lại bằng ngôn ngữ
        /// bất biến: máy đặt ngôn ngữ Việt ghi số 140.0 thành chuỗi "140,0", đọc lại thì dấu phẩy bị
        /// hiểu là DẤU NGĂN NGHÌN (vì `NumberStyles.Any` cho phép) nên ra 1400 — sai gấp mười lần.
        /// Chiều cao 140,0 cm đẩy lên cổng thành 1400.
        /// </summary>
        private static decimal? GetDecimal(object o, string name)
        {
            PropertyInfo pi = Prop(o, name);
            if (pi == null) return null;
            object v = pi.GetValue(o, null);
            if (v == null) return null;

            // Cột số thì đổi thẳng, không qua chuỗi.
            if (v is decimal) return (decimal)v;
            if (v is double) return (decimal)(double)v;
            if (v is float) return (decimal)(float)v;
            if (v is int || v is long || v is short || v is byte)
                return Convert.ToDecimal(v, CultureInfo.InvariantCulture);

            // Cột chữ thì mới phải đọc, và dùng lại đúng cách đọc của ToNum.
            return ToNum(v.ToString());
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
            // NumberStyles.Float, KHÔNG dùng Any: Any cho phép dấu ngăn nghìn nên "1.234" thành 1234.
            return decimal.TryParse(s.Trim().Replace(',', '.'), NumberStyles.Float,
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
