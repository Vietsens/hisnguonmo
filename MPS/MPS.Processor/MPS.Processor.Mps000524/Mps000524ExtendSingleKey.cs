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

namespace MPS.Processor.Mps000524
{
    /// <summary>
    /// Key của mẫu Mps000524. Ô tích là key kết thúc "_X" ("X" hoặc "").
    /// Họ key sinh theo mẫu (xem processor):
    /// - PH_01..PH_22 + _CO_X/_KHONG_X/_NAME: tiền sử bản thân.
    /// - {MENTAL|NEURO|EYE|ENT|CARDIO|RESP|MUSCLE|OEND} + _NORMAL_X/_PRE_X/_PRE_ICD/_FINAL_X/_FINAL_ICD/_CONCLUDE/_DOCTOR/_RANK_1.._5_X.
    /// - DRUG_AMPHETAMIN/MARIJUANA/MORPHIN/CODEIN/HEROIN, ALCOHOL, CLS_* (huyết học, sinh hóa, nước tiểu, CĐHA, XN khác).
    /// - SUGGEST_1.._5_X, DRIVER_CONCL_1.._5_X, DRIVER_CONCL_1.._3_CLASS, PHYSIQUE_1.._5_X, HEALTH_RANK_1.._5_X.
    /// Cột gốc: tiền tố PRO_ (HIS_KSK_PRO_DRIVER), SREQ_, PATIENT_, DHST_, GENERAL_, TREATMENT_ + tên cột.
    /// </summary>
    class Mps000524ExtendSingleKey : CommonKey
    {
        internal const string KSK_NUMBER = "KSK_NUMBER";
        internal const string IMG_AVATAR = "IMG_AVATAR";

        // ----- Thông tin hành chính -----
        internal const string PATIENT_NAME_UPPER = "PATIENT_NAME_UPPER";
        internal const string GENDER_MALE_X = "GENDER_MALE_X";
        internal const string GENDER_FEMALE_X = "GENDER_FEMALE_X";
        internal const string DOB_STR = "DOB_STR";
        internal const string ETHNIC_NAME = "ETHNIC_NAME";
        internal const string BLOOD_TYPE = "BLOOD_TYPE";
        internal const string ID_NUMBER = "ID_NUMBER";
        internal const string ID_DATE = "ID_DATE";
        internal const string ID_PLACE = "ID_PLACE";
        internal const string HEIN_CARD_NUMBER = "HEIN_CARD_NUMBER";
        internal const string ADDRESS_DETAIL = "ADDRESS_DETAIL";
        internal const string ADDRESS_COMMUNE = "ADDRESS_COMMUNE";
        internal const string PHONE = "PHONE";
        internal const string LICENSE_CLASS_NAME = "LICENSE_CLASS_NAME";
        internal const string LICENSE_NEW_X = "LICENSE_NEW_X";
        internal const string LICENSE_CHANGE_X = "LICENSE_CHANGE_X";
        internal const string LICENSE_RENEW_X = "LICENSE_RENEW_X";
        internal const string LICENSE_REQUEST_TEXT = "LICENSE_REQUEST_TEXT";

        // ----- Tiền sử -----
        internal const string FH_TEXT = "FH_TEXT";
        internal const string PH_22_TEXT = "PH_22_TEXT";
        internal const string PH_DETAIL_TEXT = "PH_DETAIL_TEXT";
        internal const string TREATING_TEXT = "TREATING_TEXT";
        internal const string MATERNITY_TEXT = "MATERNITY_TEXT";

        // ----- Khám thể lực -----
        internal const string HEIGHT_STR = "HEIGHT_STR";
        internal const string WEIGHT_STR = "WEIGHT_STR";
        internal const string BMI_STR = "BMI_STR";
        internal const string PULSE_STR = "PULSE_STR";
        internal const string BP_MAX_STR = "BP_MAX_STR";
        internal const string BP_MIN_STR = "BP_MIN_STR";
        internal const string BREATH_RATE_STR = "BREATH_RATE_STR";

        // ----- Mắt -----
        internal const string EYE_NOGLASS_R = "EYE_NOGLASS_R";
        internal const string EYE_NOGLASS_L = "EYE_NOGLASS_L";
        internal const string EYE_PINHOLE_R = "EYE_PINHOLE_R";
        internal const string EYE_PINHOLE_L = "EYE_PINHOLE_L";
        internal const string EYE_GLASS_R = "EYE_GLASS_R";
        internal const string EYE_GLASS_L = "EYE_GLASS_L";
        internal const string EYE_TWO_NOGLASS = "EYE_TWO_NOGLASS";
        internal const string EYE_TWO_GLASS = "EYE_TWO_GLASS";
        internal const string EYEFIELD_HORI_NORMAL_X = "EYEFIELD_HORI_NORMAL_X";
        internal const string EYEFIELD_HORI_LIMIT_X = "EYEFIELD_HORI_LIMIT_X";
        internal const string EYEFIELD_VERT_NORMAL_X = "EYEFIELD_VERT_NORMAL_X";
        internal const string EYEFIELD_VERT_LIMIT_X = "EYEFIELD_VERT_LIMIT_X";
        internal const string EYECOLOR_NORMAL_X = "EYECOLOR_NORMAL_X";
        internal const string EYECOLOR_BLIND_X = "EYECOLOR_BLIND_X";
        internal const string EYECOLOR_RED_X = "EYECOLOR_RED_X";
        internal const string EYECOLOR_GREEN_X = "EYECOLOR_GREEN_X";
        internal const string EYECOLOR_YELLOW_X = "EYECOLOR_YELLOW_X";

        // ----- Tai - Mũi - Họng -----
        internal const string ENT_L_NORMAL = "ENT_L_NORMAL";
        internal const string ENT_L_WHISPER = "ENT_L_WHISPER";
        internal const string ENT_R_NORMAL = "ENT_R_NORMAL";
        internal const string ENT_R_WHISPER = "ENT_R_WHISPER";

        // ----- Cận lâm sàng: bác sĩ/CN XN -----
        internal const string CLS_DRUG_DOCTOR = "CLS_DRUG_DOCTOR";
        internal const string CLS_BLOOD_DOCTOR = "CLS_BLOOD_DOCTOR";
        internal const string CLS_URINE_DOCTOR = "CLS_URINE_DOCTOR";
        internal const string CLS_DIIM_DOCTOR = "CLS_DIIM_DOCTOR";
        internal const string CLS_OTHER_DOCTOR = "CLS_OTHER_DOCTOR";

        // ----- IV. Bệnh tật, đề nghị -----
        internal const string CONCL_NORMAL_X = "CONCL_NORMAL_X";
        internal const string CONCL_PRE_X = "CONCL_PRE_X";
        internal const string CONCL_PRE_ICD = "CONCL_PRE_ICD";
        internal const string CONCL_FINAL_X = "CONCL_FINAL_X";
        internal const string CONCL_FINAL_ICD = "CONCL_FINAL_ICD";
        internal const string SUGGEST_TRANSFER_TEXT = "SUGGEST_TRANSFER_TEXT";
        internal const string SUGGEST_OTHER_TEXT = "SUGGEST_OTHER_TEXT";

        // ----- V. Kết luận -----
        internal const string REEXAM_NOTE = "REEXAM_NOTE";
        internal const string CONCLUSION_DAY = "CONCLUSION_DAY";
        internal const string CONCLUSION_MONTH = "CONCLUSION_MONTH";
        internal const string CONCLUSION_YEAR = "CONCLUSION_YEAR";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
    }
}
