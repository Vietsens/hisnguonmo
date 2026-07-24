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

namespace MPS.Processor.Mps000519
{
    /// <summary>
    /// Các key bổ sung (ngoài cột thô của HIS_KSK_UNDER_SIX) cho template Mps000519.
    /// Quy ước phái sinh tự động trong Processor:
    ///   - Cột "1 trong nhiều" (code NUMBER):  &lt;COLUMN&gt;_STR  → text hiển thị tiếng Việt
    ///   - Cột cờ Có/Không (1/0):              &lt;COLUMN&gt;_X    → "x" nếu =1, "" nếu khác
    /// Ví dụ template dùng: {SKIN_COLOR_STR}, {VACCINE_TB_X}, {IS_PREMATURE_BIRTH_X}...
    /// </summary>
    class Mps000519ExtendSingleKey : CommonKey
    {
        internal const string STR_SUFFIX = "_STR";
        internal const string FLAG_SUFFIX = "_X";

        // Key phái sinh không trùng tên cột
        internal const string IMG_AVATAR = "IMG_AVATAR";
        internal const string DHST_LOGINNAME = "DHST_LOGINNAME";
        internal const string HEALTH_EXAM_RANK_NAME = "HEALTH_EXAM_RANK_NAME";
        internal const string CONCLUSION_TIME_STR = "CONCLUSION_TIME_STR";
        internal const string ACCOMPANY_RELATIONSHIP_FULL = "ACCOMPANY_RELATIONSHIP_FULL";

        // ----- Kết luận & tư vấn (mục P) — lấy từ HIS_KSK_GENERAL -----
        // Kết luận sức khỏe (GENERAL.HEALTH_CONCLUSION_TYPE: 1/2/3) → 3 cờ "x"
        internal const string CONCLUSION_NORMAL_X = "CONCLUSION_NORMAL_X";
        internal const string CONCLUSION_TB_RISK_X = "CONCLUSION_TB_RISK_X";
        internal const string CONCLUSION_HEALTH_ISSUE_X = "CONCLUSION_HEALTH_ISSUE_X";
        // Kết luận theo bệnh ICD-10 (GENERAL.CONCLUSION_ICD_TYPE: 1/2/3) → 3 cờ "x" + mã/tên
        internal const string CONCLUSION_ICD_NONE_X = "CONCLUSION_ICD_NONE_X";
        internal const string CONCLUSION_ICD_PRELIM_X = "CONCLUSION_ICD_PRELIM_X";
        internal const string CONCLUSION_ICD_FINAL_X = "CONCLUSION_ICD_FINAL_X";
        internal const string CONCLUSION_ICD_CODE = "CONCLUSION_ICD_CODE";
        internal const string CONCLUSION_ICD_NAME = "CONCLUSION_ICD_NAME";
        // Văn bản kết luận (GENERAL)
        internal const string DISEASES = "DISEASES";
        internal const string TREATMENT_INSTRUCTION = "TREATMENT_INSTRUCTION";
        internal const string CONCLUDER_USERNAME = "CONCLUDER_USERNAME";
        internal const string CONCLUDER_LOGINNAME = "CONCLUDER_LOGINNAME";

        // ----- Mục 3 (và 4/6) Tiền sử bệnh tật, dị ứng — checklist HIS_DISEASE_DETAIL_RESULT -----
        // Sinh single key theo danh mục (ổn định qua DISEASE_TYPE_CODE + HIS_DISEASE_DETAIL.NUM_ORDER):
        //   {DISEASE_KEY_PREFIX}{typeCode}_{numOrder}{FLAG_SUFFIX}  -> "x" nếu IS_CHECK = 1, "" nếu không
        //   {DISEASE_KEY_PREFIX}{typeCode}_{numOrder}{DISEASE_TEXT_SUFFIX} -> nội dung OTHER (ghi rõ / mô tả + người mắc)
        // Mục 3 = Bệnh tật bản thân (code 49) + Dị ứng bản thân (code 50).
        // Ví dụ: {TS_D49_1_X} (Bệnh tim mạch), {TS_D49_13_TXT} (Ung thư — ghi rõ), {TS_D50_1_TXT} (Dị ứng Thuốc).
        internal const string DISEASE_KEY_PREFIX = "TS_D";
        internal const string DISEASE_TEXT_SUFFIX = "_TXT";
        // Tách OTHER (dạng "mô tả | Người mắc: X" ở mục 6 gia đình) thành 2 key riêng cho template 2 cột.
        internal const string DISEASE_DESC_SUFFIX = "_MOTA";      // phần mô tả (trước "Người mắc:")
        internal const string DISEASE_PERSON_SUFFIX = "_NM";      // phần người mắc/quan hệ (sau "Người mắc:")
    }
}
