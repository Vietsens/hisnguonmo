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

namespace MPS.Processor.Mps000523
{
    /// <summary>
    /// Keys of template Mps000523. Check boxes are keys ending with "_X" ("X" or "").
    /// Families built by pattern (see processor):
    /// - ID_1..ID_12, HEIN_1..HEIN_15: one character per printed box.
    /// - PH_01..PH_22 + _CO_X/_KHONG_X/_NAME: personal history rows.
    /// - B1_1..B1_7_X, D1_1..D1_14_X, D1_n_PLACE: family factors, chronic diseases.
    /// - D2_n, D3_n, D4_n, D5_n, D8_1_n, D8_2_n, D8_3_2/4, D8_4_1/2/4, D8_5_1..3 + _KHONG_X/_CO_X.
    /// - D6_n/D7_n + _L1_X.._L4_X: frequency (Hầu như không .. Gần như mọi ngày).
    /// - {NOI|NGOAI|DALIEU|SAN|MAT|TMH|RHM}_NORMAL_X/_PRE_X/_PRE_ICD/_FINAL_X/_FINAL_ICD/_RANK_1.._5_X/_DOCTOR.
    /// - TOOTH_18.._48_STR: tooth status code 0-9 of the form.
    /// - PHYSIQUE_1.._5_X, HEALTH_RANK_1.._5_X, SUGGEST_1.._5_X.
    /// Raw columns are also exposed with prefixes SREQ_, PATIENT_, DHST_, KSK_ (over-18), HCM_ (M4), GENERAL_, TREATMENT_.
    /// </summary>
    class Mps000523ExtendSingleKey : CommonKey
    {
        internal const string KSK_NUMBER = "KSK_NUMBER";

        // ----- Thông tin hành chính -----
        internal const string PATIENT_NAME_UPPER = "PATIENT_NAME_UPPER";
        internal const string GENDER_MALE_X = "GENDER_MALE_X";
        internal const string GENDER_FEMALE_X = "GENDER_FEMALE_X";
        internal const string DOB_STR = "DOB_STR";
        internal const string AGE = "AGE";
        internal const string ETHNIC_NAME = "ETHNIC_NAME";
        internal const string BLOOD_TYPE = "BLOOD_TYPE";
        internal const string ID_NUMBER = "ID_NUMBER";
        /// <summary>Filled only when the id does not fit the 12 boxes (passport...).</summary>
        internal const string ID_NUMBER_LONG = "ID_NUMBER_LONG";
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        /// <summary>Filled only when the card number is longer than the 15 boxes (new 17-character card); shorter fills boxes from the left.</summary>
        internal const string HEIN_CARD_NUMBER_LONG = "HEIN_CARD_NUMBER_LONG";
        internal const string ADDRESS_DETAIL = "ADDRESS_DETAIL";
        internal const string ADDRESS_COMMUNE = "ADDRESS_COMMUNE";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string WORK_PLACE_COMMUNE = "WORK_PLACE_COMMUNE";
        internal const string PHONE = "PHONE";
        internal const string EXAM_REASON = "EXAM_REASON";

        // ----- Tiền sử bệnh -----
        internal const string FH_NO_X = "FH_NO_X";
        internal const string FH_YES_X = "FH_YES_X";
        internal const string FH_TEXT = "FH_TEXT";
        internal const string PH_22_TEXT = "PH_22_TEXT";
        internal const string TREATING_NO_X = "TREATING_NO_X";
        internal const string TREATING_YES_X = "TREATING_YES_X";
        internal const string TREATING_TEXT = "TREATING_TEXT";
        internal const string MATERNITY_NO_X = "MATERNITY_NO_X";
        internal const string MATERNITY_YES_X = "MATERNITY_YES_X";
        internal const string MATERNITY_TEXT = "MATERNITY_TEXT";

        // ----- Đối tượng - chi trả -----
        internal const string SYT_PATIENT_TYPE_TEXT = "SYT_PATIENT_TYPE_TEXT";
        internal const string PAY_NSNN_X = "PAY_NSNN_X";
        internal const string PAY_CONTRACT_X = "PAY_CONTRACT_X";
        internal const string PAY_NON_BORDER_X = "PAY_NON_BORDER_X";
        internal const string PAY_SELF_X = "PAY_SELF_X";
        internal const string PAY_OTHER_X = "PAY_OTHER_X";
        internal const string PAY_OTHER_TEXT = "PAY_OTHER_TEXT";
        internal const string PLACE_FACILITY_X = "PLACE_FACILITY_X";
        internal const string PLACE_MOBILE_X = "PLACE_MOBILE_X";
        internal const string PLACE_STATION_X = "PLACE_STATION_X";
        internal const string PLACE_HOME_X = "PLACE_HOME_X";

        // ----- B, C -----
        internal const string B1_NO_X = "B1_NO_X";
        internal const string B1_YES_X = "B1_YES_X";
        internal const string HEIGHT_STR = "HEIGHT_STR";
        internal const string WEIGHT_STR = "WEIGHT_STR";
        internal const string WEIGHT_LAST_YEAR_STR = "WEIGHT_LAST_YEAR_STR";
        internal const string WAIST_STR = "WAIST_STR";
        internal const string PULSE_STR = "PULSE_STR";
        internal const string BP_MAX_STR = "BP_MAX_STR";
        internal const string BP_MIN_STR = "BP_MIN_STR";
        internal const string BREATH_RATE_STR = "BREATH_RATE_STR";

        // ----- D1 -----
        internal const string D1_NO_X = "D1_NO_X";
        internal const string D1_YES_X = "D1_YES_X";
        internal const string D1_OTHER_X = "D1_OTHER_X";
        internal const string D1_OTHER_TEXT = "D1_OTHER_TEXT";
        internal const string D8_3_1_A_X = "D8_3_1_A_X";
        internal const string D8_3_1_B_X = "D8_3_1_B_X";

        // ----- Khám chuyên khoa -----
        internal const string SAN_REFUSE_X = "SAN_REFUSE_X";
        internal const string EYE_NOGLASS_R = "EYE_NOGLASS_R";
        internal const string EYE_NOGLASS_L = "EYE_NOGLASS_L";
        internal const string EYE_PINHOLE_R = "EYE_PINHOLE_R";
        internal const string EYE_PINHOLE_L = "EYE_PINHOLE_L";
        internal const string EYE_GLASS_R = "EYE_GLASS_R";
        internal const string EYE_GLASS_L = "EYE_GLASS_L";
        internal const string EYE_REFRACTION_R = "EYE_REFRACTION_R";
        internal const string EYE_REFRACTION_L = "EYE_REFRACTION_L";
        internal const string ENT_L_NORMAL = "ENT_L_NORMAL";
        internal const string ENT_L_WHISPER = "ENT_L_WHISPER";
        internal const string ENT_R_NORMAL = "ENT_R_NORMAL";
        internal const string ENT_R_WHISPER = "ENT_R_WHISPER";

        // ----- Cận lâm sàng -----
        internal const string CLS_BLOOD_GENERAL = "CLS_BLOOD_GENERAL";
        internal const string CLS_GLU_RANDOM = "CLS_GLU_RANDOM";
        internal const string CLS_URINE_GENERAL = "CLS_URINE_GENERAL";
        internal const string CLS_DIIM_RESULT = "CLS_DIIM_RESULT";
        internal const string CLS_OTHER_RESULT = "CLS_OTHER_RESULT";
        internal const string CLS_BLOOD_DOCTOR = "CLS_BLOOD_DOCTOR";
        internal const string CLS_URINE_DOCTOR = "CLS_URINE_DOCTOR";
        internal const string CLS_DIIM_DOCTOR = "CLS_DIIM_DOCTOR";
        internal const string CLS_OTHER_DOCTOR = "CLS_OTHER_DOCTOR";

        // ----- Kết luận -----
        internal const string CONCL_NORMAL_X = "CONCL_NORMAL_X";
        internal const string CONCL_PRE_X = "CONCL_PRE_X";
        internal const string CONCL_PRE_ICD = "CONCL_PRE_ICD";
        internal const string CONCL_FINAL_X = "CONCL_FINAL_X";
        internal const string CONCL_FINAL_ICD = "CONCL_FINAL_ICD";
        internal const string SUGGEST_OTHER_TEXT = "SUGGEST_OTHER_TEXT";
        internal const string CONCLUSION_DAY = "CONCLUSION_DAY";
        internal const string CONCLUSION_MONTH = "CONCLUSION_MONTH";
        internal const string CONCLUSION_YEAR = "CONCLUSION_YEAR";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
    }
}
