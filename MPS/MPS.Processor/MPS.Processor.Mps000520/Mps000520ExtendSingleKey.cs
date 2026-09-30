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

namespace MPS.Processor.Mps000520
{
    /// <summary>
    /// Key bổ sung cho template Mps000520 (Giấy khám sức khỏe tâm thần — Mẫu 04 TT 25/2026/TT-BYT),
    /// ngoài key thô của HIS_KSK_MENTAL ({GENERAL_MANIFESTATION}, {THOUGHT_FORM}...) và V_HIS_SERVICE_REQ
    /// ({TDL_PATIENT_NAME}, {HOSPITALIZATION_REASON}...). Ô đánh dấu Có/Không: key *_X = "X" hoặc "".
    /// </summary>
    class Mps000520ExtendSingleKey : CommonKey
    {
        // ----- Đầu mẫu -----
        internal const string KSK_NUMBER = "KSK_NUMBER";

        // ----- I. Hành chính -----
        internal const string DOB_STR = "DOB_STR";
        internal const string AGE = "AGE";
        internal const string GENDER_MALE_X = "GENDER_MALE_X";
        internal const string GENDER_FEMALE_X = "GENDER_FEMALE_X";
        internal const string ID_NUMBER = "ID_NUMBER";
        internal const string ID_DATE_STR = "ID_DATE_STR";
        internal const string ID_PLACE = "ID_PLACE";
        internal const string ETHNIC_NAME = "ETHNIC_NAME";
        internal const string KSK_PATIENT_TYPES_NAME = "KSK_PATIENT_TYPES_NAME";
        internal const string KSK_PAY_SOURCE_NAME = "KSK_PAY_SOURCE_NAME";
        internal const string BLOOD_TYPE = "BLOOD_TYPE";
        internal const string PROVINCE_NAME = "PROVINCE_NAME";
        internal const string COMMUNE_NAME = "COMMUNE_NAME";
        internal const string ADDRESS_DETAIL = "ADDRESS_DETAIL";
        internal const string PHONE = "PHONE";
        internal const string EXAM_REASON = "EXAM_REASON";

        // ----- II. Tiền sử tâm thần -----
        internal const string FAMILY_HISTORY_NO_X = "FAMILY_HISTORY_NO_X";
        internal const string FAMILY_HISTORY_YES_X = "FAMILY_HISTORY_YES_X";
        internal const string PERSONAL_HISTORY_NO_X = "PERSONAL_HISTORY_NO_X";
        internal const string PERSONAL_HISTORY_YES_X = "PERSONAL_HISTORY_YES_X";

        // ----- III.1 Thể lực (HIS_DHST) — số đã bỏ phần thập phân thừa -----
        internal const string PULSE_STR = "PULSE_STR";
        internal const string BLOOD_PRESSURE_MAX_STR = "BLOOD_PRESSURE_MAX_STR";
        internal const string BLOOD_PRESSURE_MIN_STR = "BLOOD_PRESSURE_MIN_STR";
        internal const string HEIGHT_STR = "HEIGHT_STR";
        internal const string WEIGHT_STR = "WEIGHT_STR";

        // ----- V. Kết luận -----
        internal const string MENTAL_DISORDER_NO_X = "MENTAL_DISORDER_NO_X";
        internal const string MENTAL_DISORDER_YES_X = "MENTAL_DISORDER_YES_X";
        /// <summary>"F20.0 - Tâm thần phân liệt thể paranoid; F32 - ..." (ghép mã + tên theo thứ tự).</summary>
        internal const string MENTAL_DISORDER_ICD_TEXT = "MENTAL_DISORDER_ICD_TEXT";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
        internal const string CONCLUDER_LOGINNAME = "CONCLUDER_LOGINNAME";
        internal const string CONCLUSION_TIME_STR = "CONCLUSION_TIME_STR";
        internal const string CONCLUSION_DAY = "CONCLUSION_DAY";
        internal const string CONCLUSION_MONTH = "CONCLUSION_MONTH";
        internal const string CONCLUSION_YEAR = "CONCLUSION_YEAR";
    }
}
