/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * BẢN ĐỒ MỤC KHÁM theo từng mẫu khám (việc 57621) — dùng cho Xuất mẫu / Nhập mẫu Excel.
 *
 * Mỗi dòng = 1 mục khám trên tờ Giấy khám sức khỏe, gồm ô Kết quả (điền "Bình thường") và
 * ô Phân loại (điền "Loại I"). Mục chỉ có một trong hai thì tham số kia để null.
 *
 * >>> VÌ SAO LIỆT KÊ TAY CHỨ KHÔNG DUYỆT CÂY CONTROL <<<
 * Duyệt động rồi lọc bằng luật đặt tên đã thử và HỎNG: tên control ở màn hình này không bám theo
 * tab (tab nghề nghiệp có ô mang đuôi 8 và 2), kiểu control không suy được từ tiền tố (9 ô số đo
 * của tab trẻ dưới 6 tuổi mang tiền tố spn nhưng là TextEdit), và có typo hệ thống
 * (txtExamDernatology, txtExamEntRightNomal). Liệt kê tay thì đọc được, rà được, không có bẫy ngầm.
 *
 * Danh sách dưới đây lấy từ tờ Giấy khám sức khỏe thật của bệnh viện, đã đối chiếu từng ô với
 * Designer.
 *
 * CÓ trong danh sách nhưng giá trị mặc định KHÁC "Bình thường" (tham số thứ 4 của Add):
 *   thị lực không kính "10" (ô trên form đã có sẵn chữ "/10" nên chỉ điền phần tử số),
 *   thính lực nói thường "5", nói thầm "0,5" — đúng như tờ giấy.
 *   Hai ô thị lực CÓ KÍNH để trống, vì trên giấy cũng trống (chỉ người đeo kính mới điền).
 *
 * KHÔNG có trong danh sách (cố ý): số đo thể lực (chiều cao, cân nặng, mạch, huyết áp…),
 * ô "Các bệnh về…", toàn bộ cận lâm sàng, thông tin hành chính khác, ngày tháng,
 * và các ô đang bị ẩn (10 ô Phân loại của mẫu khám dưới 18 tuổi, 10 ô BS khám của trẻ dưới 6 tuổi).
 *
 * CÓ thêm (việc nâng cấp sau 57621):
 *  - 3 dòng đầu file: Lý do khám (mọi mẫu), Đối tượng + Nguồn chi trả (mẫu trên 18, dưới 18, dưới 6 tuổi).
 *  - Người khám từng chuyên khoa: gắn vào DÒNG ĐẦU của chuyên khoa (nhiều dòng dùng chung 1 ô người khám,
 *    VD Mắt có 5 dòng). Ô người khám nào không có trên mẫu khám thì không gắn.
 *    Riêng Răng hàm mặt gắn vào dòng Phân loại (dòng mang tên chuyên khoa, giống Mắt/TMH).
 */
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    /// <summary>Bản đồ mục khám của từng mẫu khám, phục vụ xuất/nhập file Excel mẫu.</summary>
    public static class KskExcelMap
    {
        /// <summary>Giá trị điền sẵn cho ô Kết quả khi xuất file.</summary>
        public const string DEFAULT_RESULT = "Bình thường";

        /// <summary>Mức phân loại điền sẵn khi xuất file (1 = Loại I).</summary>
        public const int DEFAULT_RANK_LEVEL = 1;

        /// <summary>Ô Lý do khám — ô chung của form, nằm NGOÀI vùng tab.</summary>
        public const string KEY_REASON = "txtLyDoKham";

        /// <summary>Dấu ngăn cách nhiều Đối tượng trong 1 ô (không dùng dấu phẩy vì tên đối tượng có dấu phẩy).</summary>
        public const char OBJECT_SEPARATOR = ';';

        /// <summary>
        /// Các mục khám của một mẫu khám, theo thứ tự hiện trên form.
        /// Trả danh sách rỗng (khác null) với mẫu khám không có mục nào — VD "KSK khác".
        ///
        /// <paramref name="isFemale"/>: bệnh nhân có phải nữ không. Mục Thai sản của 2 mẫu khám lái
        /// xe chỉ xuất khi nữ — xuất "Bình thường" cho bệnh nhân nam là ghi dữ liệu vô nghĩa.
        /// </summary>
        public static List<KskExcelRowADO> GetRows(int tabIndex, bool isFemale)
        {
            var rows = new List<KskExcelRowADO>();
            try
            {
            switch (tabIndex)
            {
                case 0:   // Ksk định kỳ (13 ô kết quả, 14 ô phân loại)
                    Add(rows, "a.Tuần hoàn", "txtExamCirculation", "cboExamCirculationRank");
                    Add(rows, "b. Hô hấp", "txtExamRespiratory", "cboExamRespiratoryRank");
                    Add(rows, "c. Tiêu hóa", "txtExamDigestion", "cboExamDigestionRank");
                    Add(rows, "d. Thận - TN", "txtExamKidneyUrology", "cboExamKidneyUrologyRank");
                    Add(rows, "đ. Nội tiết", "txtExamOend", "cboExamOendRank");
                    Add(rows, "e. CXK", "txtExamMuscleBone", "cboExamMuscleBoneRank");
                    Add(rows, "g. Thần kinh", "txtExamNeurological", "cboExamNeurologicalRank");
                    Add(rows, "h. Tâm thần", "txtExamMental", "cboExamMentalRank");
                    Add(rows, "Kết quả: (Ngoại khoa)", "txtExamSurgery", "cboExamSurgeryRank");
                    Add(rows, "Kết quả: (Da liễu)", "txtExamDernatology", "cboExamDernatologyRank");
                    Add(rows, "Kết quả: (Sản phụ khoa)", "txtExamObstetric", "cboExamObstetricRank");
                    Add(rows, "Răng hàm mặt - Hàm trên", "txtExamStomatologyUpper", null);
                    Add(rows, "Răng hàm mặt - Hàm dưới", "txtExamStomatologyLower", null);
                    Add(rows, "Phân loại (Mắt)", null, "cboExamEyeRank");
                    Add(rows, "Phân loại (Tai mũi họng)", null, "cboExamEntDiseaseRank");
                    Add(rows, "Phân loại (Răng hàm mặt)", null, "cboExamStomatologyRank");
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper", null, "0,5");
                    DocStd(rows, "Circulation", "");
                    DocStd(rows, "Respiratory", "");
                    DocStd(rows, "Digestion", "");
                    DocStd(rows, "KidneyUrology", "");
                    DocStd(rows, "Oend", "");
                    DocStd(rows, "MuscleBone", "");
                    DocStd(rows, "Neurological", "");
                    DocStd(rows, "Mental", "");
                    DocStd(rows, "Surgery", "");
                    DocStd(rows, "Obstetric", "");
                    Doc(rows, "cboExamDermatologyLoginName", "txtExamDernatology", "cboExamDernatologyRank");
                    DocStomatology(rows, "");
                    DocEye(rows, "");
                    DocEnt(rows, "");
                    break;
                case 1:   // Ksk trên 18 tuổi (13 ô kết quả, 14 ô phân loại)
                    Add(rows, "Tuần hoàn", "txtExamCirculation2", "cboExamCirculationRank2");
                    Add(rows, "Hô hấp", "txtExamRespiratory2", "cboExamRespiratoryRank2");
                    Add(rows, "Tiêu hóa", "txtExamDigestion2", "cboExamDigestionRank2");
                    Add(rows, "Thận - Tiết niệu", "txtExamKidneyUrology2", "cboExamKidneyUrologyRank2");
                    Add(rows, "Nội tiết", "txtExamOend2", "cboExamOend2");
                    Add(rows, "Cơ xương khớp", "txtExamMuscleBone2", "cboExamMuscleBoneRank2");
                    Add(rows, "Thần kinh", "txtExamNeurological2", "cboExamNeurologicalRank2");
                    Add(rows, "Tâm thần", "txtExamMental2", "cboExamMentalRank2");
                    Add(rows, "Ngoại khoa", "txtExamSurgery2", "cboExamSurgeryRank2");
                    Add(rows, "Da liễu", "txtExamDernatology2", "cboExamDernatologyRank2");
                    Add(rows, "Sản phụ khoa", "txtExamObstetric2", "cboExamObstetricRank2");
                    Add(rows, "Răng hàm mặt - Hàm trên", "txtExamStomatologyUpper2", null);
                    Add(rows, "Răng hàm mặt - Hàm dưới", "txtExamStomatologyLower2", null);
                    Add(rows, "Mắt - Phân loại", null, "cboExamEyeRank2");
                    Add(rows, "Tai mũi họng - Phân loại", null, "cboExamEntDiseaseRank2");
                    Add(rows, "Răng hàm mặt - Phân loại", null, "cboExamStomatologyRank2");
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight2", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft2", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight2", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft2", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal2", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper2", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal2", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper2", null, "0,5");
                    DocStd(rows, "Circulation", "2");
                    DocStd(rows, "Respiratory", "2");
                    DocStd(rows, "Digestion", "2");
                    DocStd(rows, "KidneyUrology", "2");
                    Doc(rows, "cboExamOendLoginName2", "txtExamOend2", "cboExamOend2");
                    DocStd(rows, "MuscleBone", "2");
                    DocStd(rows, "Neurological", "2");
                    DocStd(rows, "Mental", "2");
                    DocStd(rows, "Surgery", "2");
                    DocStd(rows, "Obstetric", "2");
                    Doc(rows, "cboExamDermatologyLoginName2", "txtExamDernatology2", "cboExamDernatologyRank2");
                    DocStomatology(rows, "2");
                    DocEye(rows, "2");
                    DocEnt(rows, "2");
                    break;
                case 2:   // Ksk dưới 18 tuổi (9 ô kết quả, 0 ô phân loại)
                    Add(rows, "a. Tuần hoàn", "txtExamCirculation3", null);
                    Add(rows, "b. Hô hấp", "txtExamRespiratory3", null);
                    Add(rows, "c. Tiêu hóa", "txtExamDigestion3", null);
                    Add(rows, "d. Thận - TN", "txtExamKidneyUrology3", null);
                    Add(rows, "đ. Thần kinh", "txtExamNeuroMental3", null);
                    Add(rows, "e. Tâm thần", "txtExamMental3", null);
                    Add(rows, "g. LS khác", "txtExamClinicalOther3", null);
                    Add(rows, "Răng hàm mặt - Hàm trên", "txtExamStomatologyUpper3", null);
                    Add(rows, "Răng hàm mặt - Hàm dưới", "txtExamStomatologyLower3", null);
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight3", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft3", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight3", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft3", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal3", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper3", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal3", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper3", null, "0,5");
                    DocStd(rows, "Circulation", "3");
                    DocStd(rows, "Respiratory", "3");
                    DocStd(rows, "Digestion", "3");
                    DocStd(rows, "KidneyUrology", "3");
                    DocStd(rows, "NeuroMental", "3");
                    DocStd(rows, "Mental", "3");
                    DocStd(rows, "ClinicalOther", "3");
                    DocStomatology(rows, "3");
                    DocEye(rows, "3");
                    DocEnt(rows, "3");
                    break;
                case 3:   // Ksk lái xe (16 ô kết quả, 9 ô phân loại)
                    Add(rows, "1. Tâm thần", "txtExamMental4", "txtExamMentalConclude4");
                    Add(rows, "1. Tâm thần - Phân loại", null, "cboExamMentalRank4");
                    Add(rows, "2. Thần kinh", "txtExamNeurological4", "txtNeurologicalConclude4");
                    Add(rows, "2. Thần kinh - Phân loại", null, "cboNeurologicalRank4");
                    Add(rows, "3. Mắt - Kết luận", null, "txtExamEyeConclude4");
                    Add(rows, "3. Mắt - Phân loại", null, "cboExamEyeRank4");
                    Add(rows, "4. Tai mũi họng - Kết luận", null, "txtExamEntConclude4");
                    Add(rows, "4. Tai mũi họng - Phân loại", null, "cboExamEntDiseaseRank4");
                    Add(rows, "5. Tim mạch", "txtExamCardiovascular4", "txtExamCardiovascularConclude4");
                    Add(rows, "5. Tim mạch - Phân loại", null, "cboExamCardiovascularRank4");
                    Add(rows, "6. Hô hấp", "txtExamRespiratory4", "txtExamRespiratoryConclude4");
                    Add(rows, "6. Hô hấp - Phân loại", null, "cboExamRespiratoryRank4");
                    Add(rows, "7. Cơ xương khớp", "txtExamMuscleBone4", "txtExamMuscleBoneConclude4");
                    Add(rows, "7. Cơ xương khớp - Phân loại", null, "cboExamMuscleBoneRank4");
                    Add(rows, "8. Nội tiết", "txtExamOend4", "txtExamOendConclude4");
                    Add(rows, "8. Nội tiết - Phân loại", null, "cboExamOendRank4");
                    Add(rows, "9. Thai sản", "txtExamMaternity4", "txtExamMaternityConclude4");
                    Add(rows, "9. Thai sản - Phân loại", null, "cboExamMaternityRank4");
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight4", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft4", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight4", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft4", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal4", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper4", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal4", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper4", null, "0,5");
                    DocStd(rows, "Mental", "4");
                    DocStd(rows, "Neurological", "4");
                    DocEye(rows, "4");
                    DocEnt(rows, "4");
                    DocStd(rows, "Cardiovascular", "4");
                    DocStd(rows, "Respiratory", "4");
                    DocStd(rows, "MuscleBone", "4");
                    DocStd(rows, "Oend", "4");
                    DocStd(rows, "Maternity", "4");
                    break;
                case 4:   // Ksk lái xe ô tô (7 ô kết quả, 9 ô phân loại)
                    Add(rows, "1. Tâm thần", "txtExamMental5", null);
                    Add(rows, "1. Tâm thần - Phân loại", null, "cboExamMentalRank5");
                    Add(rows, "2. Thần kinh", "txtExamNeurological5", null);
                    Add(rows, "2. Thần kinh - Phân loại", null, "cboExamNeurologicalRank5");
                    Add(rows, "3. Mắt - Phân loại", null, "cboExamEyeRank5");
                    Add(rows, "4. Tai mũi họng - Phân loại", null, "cboExamEntDiseaseRank5");
                    Add(rows, "5. Tim mạch", "txtExamCardiovascular5", null);
                    Add(rows, "5. Tim mạch - Phân loại", null, "cboExamCardiovascularRank5");
                    Add(rows, "6. Hô hấp", "txtExamRespiratory5", null);
                    Add(rows, "6. Hô hấp - Phân loại", null, "cboExamRespiratoryRank5");
                    Add(rows, "7. Cơ xương khớp", "txtExamMuscleBone5", null);
                    Add(rows, "7. Cơ xương khớp - Phân loại", null, "cboExamMuscleBoneRank5");
                    Add(rows, "8. Nội tiết", "txtExamOend5", null);
                    Add(rows, "8. Nội tiết - Phân loại", null, "cboExamOendRank5");
                    Add(rows, "9. Thai sản", "txtExamMaternity5", null);
                    Add(rows, "9. Thai sản - Phân loại", null, "cboExamMaternityRank5");
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight5", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft5", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight5", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft5", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal5", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper5", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal5", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper5", null, "0,5");
                    // Mẫu lái xe ô tô chỉ có ô người khám cho Tim mạch, Mắt, Tai mũi họng.
                    DocStd(rows, "Cardiovascular", "5");
                    DocEye(rows, "5");
                    DocEnt(rows, "5");
                    break;
                case 5:   // KSK khác (0 ô kết quả, 0 ô phân loại)
                    break;
                case 6:   // Ksk nghề nghiệp (13 ô kết quả, 14 ô phân loại)
                    Add(rows, "Tuần hoàn", "txtExamCirculation7", "cboExamCirculationRank7");
                    Add(rows, "Hô hấp", "txtExamRespiratory7", "cboExamRespiratoryRank7");
                    Add(rows, "Tiêu hóa", "txtExamDigestion7", "cboExamDigestionRank7");
                    Add(rows, "Thận - TN", "txtExamKidneyUrology7", "cboExamKidneyUrologyRank7");
                    Add(rows, "Nội tiết", "txtExamOend7", "cboExamOendRank7");
                    Add(rows, "Cơ-Xương-Khớp", "txtExamMuscleBone7", "cboExamMuscleBoneRank7");
                    Add(rows, "Thần kinh", "txtExamNeurological7", "cboExamNeurologicalRank7");
                    Add(rows, "Tâm thần", "txtExamMental7", "cboExamMentalRank7");
                    Add(rows, "3. Ngoại khoa — Kết quả", "txtExamSurgery7", "cboExamSurgeryRank7");
                    Add(rows, "7. Da liễu (ô kết quả, layoutControlItem547 TextVisible=false)", "txtExamDernatology7", "cboExamDernatologyRank7");
                    Add(rows, "8. Sản phụ khoa — Kết quả", "txtExamObstetric7", "cboExamObstetricRank7");
                    Add(rows, "Răng hàm mặt - Hàm trên", "txtExamStomatologyUpper7", null);
                    Add(rows, "Răng hàm mặt - Hàm dưới", "txtExamStomatologyLower7", null);
                    Add(rows, "Phân loại: (4. Mắt)", null, "cboExamEyeRank7");
                    Add(rows, "Phân loại: (5. Tai - Mũi - Họng)", null, "cboExamEntDiseaseRank7");
                    Add(rows, "Phân loại: (6. Răng - Hàm - Mặt)", null, "cboExamStomatologyRank7");
                    Add(rows, "Mắt - Thị lực không kính, mắt phải", "txtExamEyeSightRight7", null, "10");
                    Add(rows, "Mắt - Thị lực không kính, mắt trái", "txtExamEyeSightLeft7", null, "10");
                    Add(rows, "Mắt - Thị lực có kính, mắt phải", "txtExamEyeSightGlassRight7", null, "");
                    Add(rows, "Mắt - Thị lực có kính, mắt trái", "txtExamEyeSightGlassLeft7", null, "");
                    Add(rows, "Tai mũi họng - Tai trái, nói thường", "txtExamEntLeftNormal7", null, "5");
                    Add(rows, "Tai mũi họng - Tai trái, nói thầm", "txtExamEntLeftWhisper7", null, "0,5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thường", "txtExamEntRightNomal7", null, "5");
                    Add(rows, "Tai mũi họng - Tai phải, nói thầm", "txtExamEntRightWhisper7", null, "0,5");
                    DocStd(rows, "Circulation", "7");
                    DocStd(rows, "Respiratory", "7");
                    DocStd(rows, "Digestion", "7");
                    DocStd(rows, "KidneyUrology", "7");
                    DocStd(rows, "Oend", "7");
                    DocStd(rows, "MuscleBone", "7");
                    DocStd(rows, "Neurological", "7");
                    DocStd(rows, "Mental", "7");
                    DocStd(rows, "Surgery", "7");
                    DocStd(rows, "Obstetric", "7");
                    Doc(rows, "cboExamDermatologyLoginName7", "txtExamDernatology7", "cboExamDermatologyRank7");
                    // Ô người khám RHM của mẫu này tên "Loginname" (n thường), khác các mẫu khác.
                    DocStomatology(rows, "7", "cboExamStomatologyLoginname7");
                    DocEye(rows, "7");
                    DocEnt(rows, "7");
                    break;
                case 7:   // Trẻ em dưới 6 tuổi (25 ô kết quả, 1 ô phân loại)
                    Add(rows, "Lòng bàn tay", "rdoPalmEval8", null);
                    Add(rows, "Thóp (trẻ nhỏ còn thóp)", "rdoFontanel8", null);
                    Add(rows, "Kích thước, hình dạng đầu", "rdoHeadShape8", null);
                    Add(rows, "Vận động cổ", "rdoNeckMotion8", null);
                    Add(rows, "Vị trí 2 mắt", "rdoEyePosition8", null);
                    Add(rows, "Mí mắt và kết mạc", "rdoEyelidConjunctiva8", null);
                    Add(rows, "Đồng tử (kích thước, phản xạ)", "rdoPupil8", null);
                    Add(rows, "Tai và màng nhĩ", "rdoEarEardrum8", null);
                    Add(rows, "Đáp ứng với âm thanh", "rdoSoundResponse8", null);
                    Add(rows, "Hình dạng mũi", "rdoNoseShape8", null);
                    Add(rows, "Họng", "rdoThroat8", null);
                    Add(rows, "Hình dạng miệng", "rdoMouthShape8", null);
                    Add(rows, "Hình dạng lưỡi", "rdoTongueShape8", null);
                    Add(rows, "Nghe phổi", "rdoLungAuscultation8", null);
                    Add(rows, "Vị trí mỏm tim", "rdoApexPosition8", null);
                    Add(rows, "Nghe tim (loạn nhịp, tiếng thổi)", "rdoHeartAuscultation8", null);
                    Add(rows, "Hình dáng bụng, rốn", "rdoAbdomenNavel8", null);
                    Add(rows, "Lỗ hậu môn", "rdoAnus8", null);
                    Add(rows, "Cơ quan sinh dục ngoài", "rdoGenitalia8", null);
                    Add(rows, "Trương lực cơ", "rdoMuscleTone8", null);
                    Add(rows, "Khớp háng", "rdoHipJoint8", null);
                    Add(rows, "Phản xạ cơ", "rdoMuscleReflex8", null);
                    Add(rows, "Kiểm tra lưng, cột sống", "rdoSpineCheck8", null);
                    Add(rows, "Khám tứ chi và khớp", "rdoLimbsJoints8", null);
                    Add(rows, "Quan sát dáng đi", "rdoGait8", null);
                    Add(rows, "Phân loại sức khỏe", null, "cboHealthExamRank8");
                    break;
            }
                // Mẫu khám không có mục nào (KSK khác) thì cũng không xuất 3 dòng hành chính, để nút vẫn mờ.
                if (rows.Count > 0) rows.InsertRange(0, GetAdminRows(tabIndex));
                // Mục Thai sản chỉ có nghĩa với bệnh nhân nữ.
                if (!isFemale) rows.RemoveAll(o => IsMaternity(o));
            }
            catch (Exception ex) { LogSystem.Error(ex); }
            return rows;
        }

        /// <summary>
        /// Thêm 1 mục khám. Tên ô để null nghĩa là mục này không có ô đó.
        /// <paramref name="fixedResult"/>: giá trị mặc định RIÊNG cho ô Kết quả, dùng cho các ô
        /// không phải "Bình thường" — thị lực ("10"), thính lực ("5", "0,5"). Để null thì lấy
        /// giá trị chung <see cref="DEFAULT_RESULT"/>; để chuỗi rỗng thì xuất ra ô trống
        /// (ô thị lực có kính: trên tờ giấy để trống, chỉ người đeo kính mới điền).
        /// </summary>
        private static void Add(List<KskExcelRowADO> rows, string caption, string resultControl,
                                string rankControl, string fixedResult = null)
        {
            rows.Add(new KskExcelRowADO()
            {
                MUC_KHAM = caption,
                MA_O = resultControl,
                MA_O_PHAN_LOAI = rankControl,
                GIA_TRI_RIENG = fixedResult
            });
        }

        /// <summary>
        /// 3 dòng hành chính đầu file. Lý do khám là ô chung mọi mẫu khám; Đối tượng + Nguồn chi trả chỉ có
        /// ở mẫu trên 18, dưới 18 và trẻ dưới 6 tuổi.
        /// </summary>
        private static List<KskExcelRowADO> GetAdminRows(int tabIndex)
        {
            var head = new List<KskExcelRowADO>();
            Add(head, "Lý do khám", KEY_REASON, null);
            string suffix = null;
            switch (tabIndex)
            {
                case 1: suffix = ""; break;
                case 2: suffix = "3"; break;
                case 7: suffix = "8"; break;
            }
            if (suffix != null)
            {
                Add(head, "Đối tượng (nhiều đối tượng cách nhau bằng dấu ;)", "cboObject" + suffix, null);
                Add(head, "Nguồn chi trả", "cboPaymentSource" + suffix, null);
            }
            return head;
        }

        /// <summary>Ô thuộc 3 dòng hành chính (Lý do khám, Đối tượng, Nguồn chi trả).</summary>
        public static bool IsAdminKey(string controlName)
        {
            if (string.IsNullOrEmpty(controlName)) return false;
            return controlName == KEY_REASON
                || controlName.StartsWith("cboObject", StringComparison.Ordinal)
                || controlName.StartsWith("cboPaymentSource", StringComparison.Ordinal);
        }

        /// <summary>
        /// Gắn ô người khám vào DÒNG ĐẦU (theo thứ tự trên file) có ô Kết quả hoặc Phân loại thuộc
        /// <paramref name="members"/>, nếu ô người khám này chưa gắn vào dòng nào. Mọi dòng khớp đều được
        /// ghi nhóm người khám. Không có dòng nào khớp (VD mục Thai sản của bệnh nhân nam) thì bỏ qua.
        /// </summary>
        private static bool Doc(List<KskExcelRowADO> rows, string doctorControl, params string[] members)
        {
            if (rows == null || members == null) return false;
            bool tagged = rows.Any(o => o != null && o.MA_O_NGUOI_KHAM == doctorControl);
            bool matched = false;
            foreach (var row in rows)
            {
                if (row == null) continue;
                if (!members.Contains(row.MA_O) && !members.Contains(row.MA_O_PHAN_LOAI)) continue;
                matched = true;
                if (string.IsNullOrEmpty(row.NHOM_NGUOI_KHAM)) row.NHOM_NGUOI_KHAM = doctorControl;
                if (!tagged && string.IsNullOrEmpty(row.MA_O_NGUOI_KHAM))
                {
                    row.MA_O_NGUOI_KHAM = doctorControl;
                    tagged = true;
                }
            }
            return matched;
        }

        /// <summary>
        /// Tra ô Kết quả / Phân loại -> ô Người khám của chuyên khoa, theo bản đồ của mẫu khám.
        /// Dùng khi nhập: dòng trong file không mang khóa người khám nhưng vẫn ghi người khám.
        /// </summary>
        public static Dictionary<string, string> GetDoctorGroups(int tabIndex)
        {
            var groups = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in GetRows(tabIndex, true))
            {
                if (row == null || string.IsNullOrEmpty(row.NHOM_NGUOI_KHAM)) continue;
                if (!string.IsNullOrEmpty(row.MA_O) && !groups.ContainsKey(row.MA_O)) groups.Add(row.MA_O, row.NHOM_NGUOI_KHAM);
                if (!string.IsNullOrEmpty(row.MA_O_PHAN_LOAI) && !groups.ContainsKey(row.MA_O_PHAN_LOAI)) groups.Add(row.MA_O_PHAN_LOAI, row.NHOM_NGUOI_KHAM);
            }
            return groups;
        }

        /// <summary>Chuyên khoa đặt tên chuẩn: cboExam{X}LoginName{s} cho txtExam{X}{s} + cboExam{X}Rank{s}.</summary>
        private static void DocStd(List<KskExcelRowADO> rows, string part, string suffix)
        {
            Doc(rows, "cboExam" + part + "LoginName" + suffix, "txtExam" + part + suffix, "cboExam" + part + "Rank" + suffix);
        }

        /// <summary>
        /// Răng hàm mặt: gắn vào dòng Phân loại vì đó là dòng mang tên chuyên khoa (2 dòng Hàm trên/Hàm dưới
        /// đứng trước, người dùng không nhận ra là của RHM). Mẫu không có dòng phân loại RHM (dưới 18) thì
        /// gắn vào dòng Hàm trên.
        /// </summary>
        private static void DocStomatology(List<KskExcelRowADO> rows, string s, string doctorControl = null)
        {
            string doctor = doctorControl ?? ("cboExamStomatologyLoginName" + s);
            // Gọi riêng dòng Phân loại trước để ô người khám rơi vào dòng đó; lượt sau chỉ ghi nhóm.
            Doc(rows, doctor, "cboExamStomatologyRank" + s);
            Doc(rows, doctor, "txtExamStomatologyUpper" + s, "txtExamStomatologyLower" + s);
        }

        private static void DocEye(List<KskExcelRowADO> rows, string s)
        {
            Doc(rows, "cboExamEyeLoginName" + s,
                "cboExamEyeRank" + s, "txtExamEyeConclude" + s,
                "txtExamEyeSightRight" + s, "txtExamEyeSightLeft" + s,
                "txtExamEyeSightGlassRight" + s, "txtExamEyeSightGlassLeft" + s);
        }

        private static void DocEnt(List<KskExcelRowADO> rows, string s)
        {
            Doc(rows, "cboExamEntLoginName" + s,
                "cboExamEntDiseaseRank" + s, "txtExamEntConclude" + s,
                "txtExamEntLeftNormal" + s, "txtExamEntLeftWhisper" + s,
                "txtExamEntRightNomal" + s, "txtExamEntRightWhisper" + s);
        }

        /// <summary>Mục Thai sản, nhận theo tên ô (2 mẫu khám lái xe dùng tên Maternity).</summary>
        private static bool IsMaternity(KskExcelRowADO row)
        {
            if (row == null) return false;
            string a = (row.MA_O ?? "").ToLowerInvariant();
            string b = (row.MA_O_PHAN_LOAI ?? "").ToLowerInvariant();
            return a.Contains("maternity") || b.Contains("maternity");
        }
    }
}
