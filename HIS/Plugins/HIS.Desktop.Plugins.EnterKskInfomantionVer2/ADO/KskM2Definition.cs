/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * vCong61141 - Mau 02 (TT 25/2026/TT-BYT), tre tu du 6 den duoi 18 tuoi, cong KSK So Y te TP.HCM (M2).
 *
 * Static definitions shared by the input tab, the print keys and the stored JSON:
 *   - 23 vaccination rows (field code = field name of the M2 message, order = order on the form)
 *   - 18 ADHD questions (B1) and 10 ASD questions (B2)
 *   - 10 clinical sections (field prefix of the M2 message -> column prefix of HIS_KSK_SYT_HCM_M2)
 *
 * Stored answers use INTERNAL levels (not portal catalog ids) so data entry and printing still work
 * when the workstation has not downloaded the portal catalogs yet; ids are resolved when pushing.
 */
using System;
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>One vaccination dose row of Mau 02.</summary>
    public class KskM2VaccineDef
    {
        public string Field { get; set; }
        public string Disease { get; set; }
        public string Schedule { get; set; }
        public string Dose { get; set; }

        public KskM2VaccineDef(string field, string disease, string schedule, string dose)
        {
            Field = field; Disease = disease; Schedule = schedule; Dose = dose;
        }
    }

    /// <summary>One screening question (B1 or B2).</summary>
    public class KskM2QuestionDef
    {
        public int No { get; set; }
        public string Field { get; set; }
        public string Text { get; set; }

        public KskM2QuestionDef(int no, string field, string text)
        {
            No = no; Field = field; Text = text;
        }
    }

    /// <summary>One clinical section of Mau 02 part "Kham lam sang".</summary>
    public class KskM2SectionDef
    {
        /// <summary>Field prefix of the M2 message (noikhoa, hohap, ... cotsong_clskhac).</summary>
        public string Key { get; set; }
        /// <summary>Column prefix of HIS_KSK_SYT_HCM_M2 (EXAM_CIRCULATION ...).</summary>
        public string ColumnPrefix { get; set; }
        public string Caption { get; set; }

        public KskM2SectionDef(string key, string columnPrefix, string caption)
        {
            Key = key; ColumnPrefix = columnPrefix; Caption = caption;
        }
    }

    /// <summary>Internal vaccination status stored in VACCINATION_JSON.</summary>
    public enum KskM2VaccineStatus
    {
        /// <summary>Not answered.</summary>
        None = 0,
        /// <summary>Da tiem.</summary>
        Done = 1,
        /// <summary>Chua tiem.</summary>
        NotDone = 2,
        /// <summary>Khong nho ro.</summary>
        Unknown = 3
    }

    /// <summary>Internal B1 answer stored in ADHD_JSON.</summary>
    public enum KskM2AdhdLevel
    {
        /// <summary>Not answered.</summary>
        None = 0,
        /// <summary>Khong co.</summary>
        Never = 1,
        /// <summary>Thinh thoang.</summary>
        Sometimes = 2,
        /// <summary>Thuong xuyen.</summary>
        Often = 3
    }

    /// <summary>Internal B2 answer stored in ASD_JSON.</summary>
    public enum KskM2AsdLevel
    {
        /// <summary>Not answered.</summary>
        None = 0,
        /// <summary>Hoan toan dong y.</summary>
        StronglyAgree = 1,
        /// <summary>Co chut dong y.</summary>
        SlightlyAgree = 2,
        /// <summary>Co chut khong dong y.</summary>
        SlightlyDisagree = 3,
        /// <summary>Hoan toan khong dong y.</summary>
        StronglyDisagree = 4
    }

    public static class KskM2Definition
    {
        /// <summary>Key of the free "Cac loai khac" vaccination row in VACCINATION_JSON.</summary>
        public const string VACCINE_OTHER_KEY = "khac";

        /// <summary>B1 threshold: from this many "Thinh thoang"/"Thuong xuyen" answers in a group.</summary>
        public const int ADHD_THRESHOLD = 6;

        /// <summary>B2 threshold of total score.</summary>
        public const int ASD_THRESHOLD = 6;

        /// <summary>B2 applies to children from 6 to 11 years old (age at exam date).</summary>
        public const int ASD_MIN_AGE = 6;
        public const int ASD_MAX_AGE = 11;

        public const string ADHD_RESULT_NORMAL = "Bình thường";
        public const string ADHD_RESULT_ATTENTION = "Cần đưa trẻ đến cơ sở khám bệnh, chữa bệnh chuyên khoa để được đánh giá lại tình trạng giảm chú ý";
        public const string ADHD_RESULT_HYPER = "Cần đưa trẻ đến cơ sở khám bệnh, chữa bệnh chuyên khoa để được đánh giá lại tình trạng tăng động";
        public const string ASD_RESULT_NORMAL = "Bình thường";
        public const string ASD_RESULT_REFER = "Cân nhắc giới thiệu trẻ đến khám tại cơ sở khám bệnh, chữa bệnh chuyên khoa để được đánh giá chẩn đoán phù hợp";

        public static readonly List<KskM2VaccineDef> VACCINES = new List<KskM2VaccineDef>
        {
            new KskM2VaccineDef("tiem_lao_m1", "Lao", "Sơ sinh", "Mũi 1"),
            new KskM2VaccineDef("tiem_viemganb_m1", "Viêm gan siêu vi B", "Trong vòng 24 giờ sau sinh", "Mũi 1"),
            new KskM2VaccineDef("tiem_viemganb_m2", "Viêm gan siêu vi B", "02 tháng", "Mũi 2"),
            new KskM2VaccineDef("tiem_viemganb_m3", "Viêm gan siêu vi B", "03 tháng", "Mũi 3"),
            new KskM2VaccineDef("tiem_viemganb_m4", "Viêm gan siêu vi B", "04 tháng", "Mũi 4"),
            new KskM2VaccineDef("tiem_bachhau_uonvan_hoga_m1", "Bạch hầu, Uốn ván, Ho gà", "02 tháng", "Mũi 1"),
            new KskM2VaccineDef("tiem_bachhau_uonvan_hoga_m2", "Bạch hầu, Uốn ván, Ho gà", "03 tháng", "Mũi 2"),
            new KskM2VaccineDef("tiem_bachhau_uonvan_hoga_m3", "Bạch hầu, Uốn ván, Ho gà", "04 tháng", "Mũi 3"),
            new KskM2VaccineDef("tiem_bachhau_uonvan_hoga_m4", "Bạch hầu, Uốn ván, Ho gà", "18 tháng", "Mũi 4"),
            new KskM2VaccineDef("tiem_bailiet_l1", "Bại liệt", "02 tháng (uống)", "Liều 1"),
            new KskM2VaccineDef("tiem_bailiet_l2", "Bại liệt", "03 tháng (uống)", "Liều 2"),
            new KskM2VaccineDef("tiem_bailiet_l3", "Bại liệt", "04 tháng (uống)", "Liều 3"),
            new KskM2VaccineDef("tiem_bailiet_m1", "Bại liệt", "05 tháng (tiêm)", "Mũi 1"),
            new KskM2VaccineDef("tiem_bailiet_m2", "Bại liệt", "09 tháng (tiêm)", "Mũi 2"),
            new KskM2VaccineDef("tiem_hib_m1", "Viêm phổi, viêm màng não do Hib", "02 tháng", "Mũi 1"),
            new KskM2VaccineDef("tiem_hib_m2", "Viêm phổi, viêm màng não do Hib", "03 tháng", "Mũi 2"),
            new KskM2VaccineDef("tiem_hib_m3", "Viêm phổi, viêm màng não do Hib", "04 tháng", "Mũi 3"),
            new KskM2VaccineDef("tiem_soi_m1", "Sởi", "09 tháng", "Mũi 1"),
            new KskM2VaccineDef("tiem_soi_m2", "Sởi", "18 tháng", "Mũi 2"),
            new KskM2VaccineDef("tiem_soi_rubella_m1", "Sởi - Rubella", "18 tháng", "Mũi 1"),
            new KskM2VaccineDef("tiem_viemnao_nhatban_m1", "Viêm não Nhật Bản", "12 tháng", "Mũi 1"),
            new KskM2VaccineDef("tiem_viemnao_nhatban_m2", "Viêm não Nhật Bản", "Cách 1-2 tuần sau mũi 1", "Mũi 2"),
            new KskM2VaccineDef("tiem_viemnao_nhatban_m3", "Viêm não Nhật Bản", "24 tháng", "Mũi 3")
        };

        public static readonly List<KskM2QuestionDef> ADHD_QUESTIONS = new List<KskM2QuestionDef>
        {
            new KskM2QuestionDef(1, "adhd_thieu_can_than", "Thường không chú ý đến chi tiết hoặc mắc lỗi do thiếu cẩn thận trong học tập, công việc hoặc các hoạt động khác."),
            new KskM2QuestionDef(2, "adhd_kho_duy_tri_chu_y", "Thường gặp khó khăn khi duy trì sự chú ý trong các hoạt động hằng ngày hoặc hoạt động vui chơi."),
            new KskM2QuestionDef(3, "adhd_khong_lang_nghe", "Thường có vẻ như không chú ý lắng nghe khi nói chuyện trực tiếp."),
            new KskM2QuestionDef(4, "adhd_khong_theo_huong_dan", "Thường không thực hiện được các hướng dẫn và không thể hoàn tất việc học ở trường (không phải do không hiểu được các hướng dẫn)."),
            new KskM2QuestionDef(5, "adhd_kho_to_chuc", "Thường gặp khó khăn khi thực hiện các nhiệm vụ và các hoạt động."),
            new KskM2QuestionDef(6, "adhd_ne_tranh_tap_trung", "Thường né tránh, không thích, hoặc miễn cưỡng tham gia các nhiệm vụ cần nỗ lực tập trung chú ý kéo dài (ví dụ, làm bài tập ở trường hoặc làm bài tập ở nhà)."),
            new KskM2QuestionDef(7, "adhd_danh_mat_do_dung", "Thường đánh mất những vật dụng cần thiết cho các hoạt động (ví dụ, bài tập ở trường, bút chì hoặc sách, dụng cụ học tập khác)."),
            new KskM2QuestionDef(8, "adhd_de_bi_xao_lang", "Thường dễ bị xao lãng bởi các kích thích bên ngoài."),
            new KskM2QuestionDef(9, "adhd_hay_quen", "Thường hay quên trong các hoạt động hằng ngày."),
            new KskM2QuestionDef(10, "adhd_bon_chon_tay_chan", "Thường có vẻ bồn chồn, tay chân không yên hoặc uốn éo vặn vẹo người khi ngồi."),
            new KskM2QuestionDef(11, "adhd_roi_khoi_cho", "Thường rời khỏi chỗ ngồi trong lớp học hoặc trong những tình huống khác mà trẻ cần phải ngồi tại chỗ."),
            new KskM2QuestionDef(12, "adhd_chay_nhay_lang_xang", "Thường chạy tới lui, lăng xăng hoặc leo trèo quá mức trong những tình huống không phù hợp (ở thanh thiếu niên, có thể chỉ là cảm giác bồn chồn, không yên)."),
            new KskM2QuestionDef(13, "adhd_kho_choi_yen_lang", "Thường khó khăn khi cần giữ yên lặng trong lúc chơi hoặc khó khăn khi tham gia các hoạt động thư giãn, yên tĩnh."),
            new KskM2QuestionDef(14, "adhd_ban_ron_hoat_dong", "Thường \"bận rộn hoạt động\" hoặc hoạt động như thể không biết mệt mỏi."),
            new KskM2QuestionDef(15, "adhd_noi_qua_nhieu", "Thường nói quá nhiều."),
            new KskM2QuestionDef(16, "adhd_noi_bat_cau_tra_loi", "Thường nói bật ra câu trả lời khi câu hỏi chưa được nói xong."),
            new KskM2QuestionDef(17, "adhd_kho_cho_doi", "Thường gặp khó khăn khi phải chờ đợi tới lượt."),
            new KskM2QuestionDef(18, "adhd_ngat_loi_xen_ngang", "Thường ngắt lời người khác hoặc chen ngang (ví dụ, chen ngang vào cuộc nói chuyện hoặc trò chơi của người khác).")
        };

        public static readonly List<KskM2QuestionDef> ASD_QUESTIONS = new List<KskM2QuestionDef>
        {
            new KskM2QuestionDef(1, "asd_chu_y_am_thanh_nho", "Trẻ thường chú ý đến những âm thanh nhỏ khi người khác không chú ý đến."),
            new KskM2QuestionDef(2, "asd_tap_trung_toan_canh", "Trẻ thường tập trung chú ý vào toàn bộ bức tranh hơn là những chi tiết nhỏ."),
            new KskM2QuestionDef(3, "asd_theo_doi_hoi_thoai", "Trong một nhóm xã hội, trẻ có thể dễ dàng theo dõi cuộc trò chuyện của nhiều người khác nhau."),
            new KskM2QuestionDef(4, "asd_de_dang_thay_doi_hoat_dong", "Trẻ thấy dễ dàng khi thay đổi tới lui giữa các hoạt động khác nhau."),
            new KskM2QuestionDef(5, "asd_kho_duy_tri_tro_chuyen", "Trẻ không biết cách duy trì cuộc trò chuyện với các bạn đồng trang lứa của mình."),
            new KskM2QuestionDef(6, "asd_gioi_tan_gau", "Trẻ giỏi trong các cuộc tán gẫu, nói chuyện phiếm xã hội."),
            new KskM2QuestionDef(7, "asd_kho_hieu_tam_ly_nhan_vat", "Khi trẻ đang đọc một câu chuyện, trẻ cảm thấy khó khăn khi trình bày ý muốn và cảm xúc của nhân vật."),
            new KskM2QuestionDef(8, "asd_thich_choi_gia_vo", "Trước khi trẻ vào học lớp 1, trẻ thường thích chơi các trò chơi liên quan đến sắm vai giả vờ với trẻ khác."),
            new KskM2QuestionDef(9, "asd_the_hien_cam_xuc_mat", "Trẻ thấy dễ dàng khi trình bày những gì trẻ đang suy nghĩ hoặc cảm thấy bằng cách nhìn vào gương mặt của trẻ."),
            new KskM2QuestionDef(10, "asd_kho_khan_ket_ban", "Trẻ thấy khó khăn khi kết bạn mới.")
        };

        /// <summary>B2 questions scored when the answer is an AGREE option (1, 5, 7, 10).</summary>
        public static readonly HashSet<int> ASD_AGREE_SCORED = new HashSet<int> { 1, 5, 7, 10 };

        public static readonly List<KskM2SectionDef> SECTIONS = new List<KskM2SectionDef>
        {
            new KskM2SectionDef("noikhoa", "EXAM_CIRCULATION", "1a. Tuần hoàn"),
            new KskM2SectionDef("hohap", "EXAM_RESPIRATORY", "1b. Hô hấp"),
            new KskM2SectionDef("tieuhoa", "EXAM_DIGESTION", "1c. Tiêu hóa"),
            new KskM2SectionDef("thantietnieu", "EXAM_KIDNEY_URO", "1d. Thận - Tiết niệu"),
            new KskM2SectionDef("thankinh", "EXAM_NEUROLOGICAL", "1đ. Thần kinh"),
            new KskM2SectionDef("tamthan", "EXAM_MENTAL", "1e. Tâm thần"),
            new KskM2SectionDef("cotsong_clskhac", "EXAM_OTHER", "1g. Khám lâm sàng khác"),
            new KskM2SectionDef("mat", "EXAM_EYE", "2. Mắt - Các bệnh về mắt"),
            new KskM2SectionDef("tmh", "EXAM_ENT", "3. Tai - Mũi - Họng - Các bệnh về tai mũi họng"),
            new KskM2SectionDef("rhm", "EXAM_STOMATOLOGY", "4. Răng - Hàm - Mặt - Các bệnh về răng hàm mặt")
        };

        /// <summary>Permanent teeth (upper row 18..28, lower row 48..38) — same order as the form.</summary>
        public static readonly string[] TEETH_UPPER = { "18", "17", "16", "15", "14", "13", "12", "11", "21", "22", "23", "24", "25", "26", "27", "28" };
        public static readonly string[] TEETH_LOWER = { "48", "47", "46", "45", "44", "43", "42", "41", "31", "32", "33", "34", "35", "36", "37", "38" };

        /// <summary>Primary teeth (upper row 55..65, lower row 85..75).</summary>
        public static readonly string[] TEETH_PRIMARY_UPPER = { "55", "54", "53", "52", "51", "61", "62", "63", "64", "65" };
        public static readonly string[] TEETH_PRIMARY_LOWER = { "85", "84", "83", "82", "81", "71", "72", "73", "74", "75" };

        /// <summary>All 52 teeth.</summary>
        public static IEnumerable<string> AllTeeth()
        {
            foreach (string t in TEETH_UPPER) yield return t;
            foreach (string t in TEETH_PRIMARY_UPPER) yield return t;
            foreach (string t in TEETH_PRIMARY_LOWER) yield return t;
            foreach (string t in TEETH_LOWER) yield return t;
        }

        /// <summary>Count answers "Thinh thoang"/"Thuong xuyen" of questions [from..to].</summary>
        public static int CountAdhdPositive(IDictionary<string, int> answers, int from, int to)
        {
            int n = 0;
            if (answers == null) return 0;
            foreach (KskM2QuestionDef q in ADHD_QUESTIONS)
            {
                if (q.No < from || q.No > to) continue;
                int v;
                if (answers.TryGetValue(q.Field, out v)
                    && (v == (int)KskM2AdhdLevel.Sometimes || v == (int)KskM2AdhdLevel.Often)) n++;
            }
            return n;
        }

        /// <summary>Suggested B1 result (rules of Mau 02). Null when nothing is answered.</summary>
        public static string SuggestAdhdResult(IDictionary<string, int> answers)
        {
            if (answers == null || answers.Count == 0) return null;
            bool attention = CountAdhdPositive(answers, 1, 9) >= ADHD_THRESHOLD;
            bool hyper = CountAdhdPositive(answers, 10, 18) >= ADHD_THRESHOLD;
            if (attention && hyper) return ADHD_RESULT_ATTENTION + "; " + ADHD_RESULT_HYPER;
            if (attention) return ADHD_RESULT_ATTENTION;
            if (hyper) return ADHD_RESULT_HYPER;
            return ADHD_RESULT_NORMAL;
        }

        /// <summary>B2 total score (rules of Mau 02).</summary>
        public static int ScoreAsd(IDictionary<string, int> answers)
        {
            int score = 0;
            if (answers == null) return 0;
            foreach (KskM2QuestionDef q in ASD_QUESTIONS)
            {
                int v;
                if (!answers.TryGetValue(q.Field, out v)) continue;
                bool agree = v == (int)KskM2AsdLevel.StronglyAgree || v == (int)KskM2AsdLevel.SlightlyAgree;
                bool disagree = v == (int)KskM2AsdLevel.SlightlyDisagree || v == (int)KskM2AsdLevel.StronglyDisagree;
                if (ASD_AGREE_SCORED.Contains(q.No) ? agree : disagree) score++;
            }
            return score;
        }

        /// <summary>Suggested B2 result. Null when nothing is answered.</summary>
        public static string SuggestAsdResult(IDictionary<string, int> answers)
        {
            if (answers == null || answers.Count == 0) return null;
            return ScoreAsd(answers) >= ASD_THRESHOLD ? ASD_RESULT_REFER : ASD_RESULT_NORMAL;
        }

        /// <summary>Age in full years at the given date.</summary>
        public static int AgeAt(DateTime dob, DateTime at)
        {
            int age = at.Year - dob.Year;
            if (at.Month < dob.Month || (at.Month == dob.Month && at.Day < dob.Day)) age--;
            return age;
        }
    }
}
