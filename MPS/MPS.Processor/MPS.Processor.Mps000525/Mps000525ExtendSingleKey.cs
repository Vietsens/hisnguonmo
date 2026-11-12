/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000525
{
    /// <summary>
    /// Key bổ sung cho template Mps000525 (Giấy KSK định kỳ người hành nghề lái xe ô tô — Mẫu 06 TT 25/2026),
    /// ngoài key thô của HIS_KSK_PRO_DRIVER ({EXAM_EYE_CONCLUDE}, {TOOTH_18}, {TEST_CODEIN}...) và V_HIS_SERVICE_REQ.
    /// Ô đánh dấu: key *_X = "X" hoặc "".
    ///
    /// Key theo TỪNG MỤC KHÁM (tiền tố = tiền tố cột, VD EXAM_CIRCULATION, EXAM_EYE, EXAM_STOMATOLOGY):
    ///   {P}_NORMAL_X        ô "Chưa phát hiện bất thường"
    ///   {P}_PRE_X           ô "Chẩn đoán sơ bộ"   + {P}_PRE_ICD_TEXT   "Mã - Tên; Mã - Tên"
    ///   {P}_FINAL_X         ô "Chẩn đoán xác định" + {P}_FINAL_ICD_TEXT
    ///   {P}_RANK_1_X .. {P}_RANK_5_X   ô "Loại I" .. "Loại V"
    ///   {P}_DOCTOR          họ tên bác sĩ khám mục đó
    /// Câu tiền sử 1..21: DISEASE_{n}_NAME, DISEASE_{n}_YES_X, DISEASE_{n}_NO_X; câu 22: DISEASE_22_YES_X + {OTHER_DISEASE}.
    /// Phân loại thể lực: DHST_RANK_1_X..5_X. Phân loại sức khỏe: HEALTH_RANK_1_X..5_X.
    /// Kết luận lái xe: DRIVER_CONCLUSION_1_X..5_X; hạng GPLX in ở dòng đã chọn: CONCLUSION_LICENSE_1..3.
    /// Đề nghị: SUGGEST_1_X..5_X. Người thực hiện CLS: TEST_DRUG_DOCTOR, TEST_FEMALE_DOCTOR, TEST_BLOOD_DOCTOR,
    /// TEST_URINE_DOCTOR, DIIM_DOCTOR, OTHER_TEST_DOCTOR.
    /// </summary>
    class Mps000525ExtendSingleKey : CommonKey
    {
        // ----- Đầu mẫu -----
        internal const string KSK_NUMBER = "KSK_NUMBER";

        // ----- Hành chính -----
        internal const string DOB_STR = "DOB_STR";
        internal const string AGE = "AGE";
        internal const string GENDER_MALE_X = "GENDER_MALE_X";
        internal const string GENDER_FEMALE_X = "GENDER_FEMALE_X";
        internal const string ID_NUMBER = "ID_NUMBER";
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string ETHNIC_NAME = "ETHNIC_NAME";
        internal const string BLOOD_TYPE = "BLOOD_TYPE";
        internal const string ADDRESS_DETAIL = "ADDRESS_DETAIL";
        internal const string COMMUNE_NAME = "COMMUNE_NAME";
        internal const string PROVINCE_NAME = "PROVINCE_NAME";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string PHONE = "PHONE";
        internal const string EXAM_REASON = "EXAM_REASON";
        internal const string LICENSE_CLASS_NAME = "LICENSE_CLASS_NAME";

        // ----- Đối tượng - chi trả -----
        internal const string KSK_PATIENT_TYPES_NAME = "KSK_PATIENT_TYPES_NAME";
        internal const string KSK_PAY_SOURCE_NAME = "KSK_PAY_SOURCE_NAME";
        internal const string PAY_SOURCE_EMPLOYER_X = "PAY_SOURCE_EMPLOYER_X";
        internal const string PAY_SOURCE_OTHER_X = "PAY_SOURCE_OTHER_X";
        internal const string PAY_SOURCE_OTHER_NAME = "PAY_SOURCE_OTHER_NAME";
        internal const string EXAM_PLACE_FACILITY_X = "EXAM_PLACE_FACILITY_X";
        internal const string EXAM_PLACE_MOBILE_X = "EXAM_PLACE_MOBILE_X";

        // ----- Tiền sử -----
        internal const string FAMILY_HISTORY_NO_X = "FAMILY_HISTORY_NO_X";
        internal const string FAMILY_HISTORY_YES_X = "FAMILY_HISTORY_YES_X";
        internal const string TREATING_NO_X = "TREATING_NO_X";
        internal const string TREATING_YES_X = "TREATING_YES_X";
        internal const string MATERNITY_NO_X = "MATERNITY_NO_X";
        internal const string MATERNITY_YES_X = "MATERNITY_YES_X";
        internal const string DISEASE_22_YES_X = "DISEASE_22_YES_X";

        // ----- Thể lực (HIS_DHST) -----
        internal const string HEIGHT_STR = "HEIGHT_STR";
        internal const string WEIGHT_STR = "WEIGHT_STR";
        internal const string BMI_STR = "BMI_STR";
        internal const string PULSE_STR = "PULSE_STR";
        internal const string BLOOD_PRESSURE_STR = "BLOOD_PRESSURE_STR";
        internal const string BREATH_RATE_STR = "BREATH_RATE_STR";

        // ----- Khám lâm sàng -----
        internal const string EXAM_OBSTETRIC_REFUSE_X = "EXAM_OBSTETRIC_REFUSE_X";

        // ----- Kết luận -----
        internal const string CONCLUSION_NORMAL_X = "CONCLUSION_NORMAL_X";
        internal const string CONCLUSION_PRE_X = "CONCLUSION_PRE_X";
        internal const string CONCLUSION_PRE_ICD_TEXT = "CONCLUSION_PRE_ICD_TEXT";
        internal const string CONCLUSION_FINAL_X = "CONCLUSION_FINAL_X";
        internal const string CONCLUSION_FINAL_ICD_TEXT = "CONCLUSION_FINAL_ICD_TEXT";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
        internal const string CONCLUDER_LOGINNAME = "CONCLUDER_LOGINNAME";
        internal const string CONCLUSION_TIME_STR = "CONCLUSION_TIME_STR";
        internal const string CONCLUSION_DAY = "CONCLUSION_DAY";
        internal const string CONCLUSION_MONTH = "CONCLUSION_MONTH";
        internal const string CONCLUSION_YEAR = "CONCLUSION_YEAR";
    }
}
