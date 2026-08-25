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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000315.ADO
{
    /// <summary>
    /// Phần khám KSK gộp từ 4 mẫu (trên-18 / dưới-18 / dưới-6 / khám chung) về MỘT bộ tên field chuẩn
    /// (tên theo HIS_KSK_GENERAL) để biểu mẫu chỉ cần dùng 1 bộ key duy nhất.
    /// Thứ tự ưu tiên: HIS_KSK_OVER_EIGHTEEN → HIS_KSK_UNDER_EIGHTEEN → HIS_KSK_UNDER_SIX → HIS_KSK_GENERAL.
    /// Mẫu nào không có field tương ứng thì rơi về HIS_KSK_GENERAL (không ghi đè bằng null).
    /// </summary>
    public class KskExamAdo
    {
        /// <summary>Tên mẫu KSK đang lấy dữ liệu (để biểu mẫu in kèm nếu cần).</summary>
        public string KSK_FORM_NAME { get; set; }

        public string EXAM_CIRCULATION { get; set; }
        public long? EXAM_CIRCULATION_RANK { get; set; }
        public string EXAM_CIRCULATION_LOGINNAME { get; set; }

        public string EXAM_RESPIRATORY { get; set; }
        public long? EXAM_RESPIRATORY_RANK { get; set; }
        public string EXAM_RESPIRATORY_LOGINNAME { get; set; }

        public string EXAM_DIGESTION { get; set; }
        public long? EXAM_DIGESTION_RANK { get; set; }
        public string EXAM_DIGESTION_LOGINNAME { get; set; }

        public string EXAM_KIDNEY_UROLOGY { get; set; }
        public long? EXAM_KIDNEY_UROLOGY_RANK { get; set; }
        public string EXAM_KIDNEY_UROLOGY_LOGINNAME { get; set; }

        public string EXAM_NEUROLOGICAL { get; set; }
        public long? EXAM_NEUROLOGICAL_RANK { get; set; }
        public string EXAM_NEUROLOGICAL_LOGINNAME { get; set; }

        public string EXAM_MENTAL { get; set; }
        public long? EXAM_MENTAL_RANK { get; set; }
        public string EXAM_MENTAL_LOGINNAME { get; set; }

        public string EXAM_MUSCLE_BONE { get; set; }
        public long? EXAM_MUSCLE_BONE_RANK { get; set; }
        public string EXAM_MUSCLE_BONE_LOGINNAME { get; set; }

        public string EXAM_DERMATOLOGY { get; set; }
        public long? EXAM_DERMATOLOGY_RANK { get; set; }
        public string EXAM_DERMATOLOGY_LOGINNAME { get; set; }

        public string EXAM_SURGERY { get; set; }
        public long? EXAM_SURGERY_RANK { get; set; }
        public string EXAM_SURGERY_LOGINNAME { get; set; }

        public string EXAM_OBSTETRIC { get; set; }
        public long? EXAM_OBSTETRIC_RANK { get; set; }
        public string EXAM_OBSTETRIC_LOGINNAME { get; set; }

        public string EXAM_OEND { get; set; }
        public long? EXAM_OEND_RANK { get; set; }
        public string EXAM_OEND_LOGINNAME { get; set; }

        public string EXAM_EYE { get; set; }
        public string EXAM_EYE_DISEASE { get; set; }
        public long? EXAM_EYE_RANK { get; set; }
        public string EXAM_EYE_LOGINNAME { get; set; }
        public string EXAM_EYESIGHT_LEFT { get; set; }
        public string EXAM_EYESIGHT_RIGHT { get; set; }
        public string EXAM_EYESIGHT_GLASS_LEFT { get; set; }
        public string EXAM_EYESIGHT_GLASS_RIGHT { get; set; }

        public string EXAM_ENT { get; set; }
        public string EXAM_ENT_DISEASE { get; set; }
        public long? EXAM_ENT_RANK { get; set; }
        public string EXAM_ENT_LOGINNAME { get; set; }
        public string EXAM_ENT_LEFT_NORMAL { get; set; }
        public string EXAM_ENT_LEFT_WHISPER { get; set; }
        public string EXAM_ENT_RIGHT_NORMAL { get; set; }
        public string EXAM_ENT_RIGHT_WHISPER { get; set; }

        public string EXAM_STOMATOLOGY { get; set; }
        public string EXAM_STOMATOLOGY_DISEASE { get; set; }
        public long? EXAM_STOMATOLOGY_RANK { get; set; }
        public string EXAM_STOMATOLOGY_LOGINNAME { get; set; }
        public string EXAM_STOMATOLOGY_UPPER { get; set; }
        public string EXAM_STOMATOLOGY_LOWER { get; set; }

        public string DISEASES { get; set; }
        public string TREATMENT_INSTRUCTION { get; set; }
        public string RESULT_SUBCLINICAL { get; set; }
        public string PATHOLOGICAL_HISTORY { get; set; }
        public string PATHOLOGICAL_HISTORY_FAMILY { get; set; }

        public long? HEALTH_EXAM_RANK_ID { get; set; }
        public string HEALTH_EXAM_RANK_CODE { get; set; }
        public string HEALTH_EXAM_RANK_NAME { get; set; }

        public long? DHST_ID { get; set; }
        public long? DHST_RANK { get; set; }

        public KskExamAdo() { }

        /// <summary>
        /// Gộp phần khám theo thứ tự ưu tiên trên-18 → dưới-18 → dưới-6 → khám chung.
        /// Trả null khi cả 4 mẫu đều không có bản ghi.
        /// </summary>
        public static KskExamAdo Build(HIS_KSK_OVER_EIGHTEEN overEighteen,
            HIS_KSK_UNDER_EIGHTEEN underEighteen,
            HIS_KSK_UNDER_SIX underSix,
            HIS_KSK_GENERAL general,
            List<HIS_HEALTH_EXAM_RANK> healthExamRanks)
        {
            try
            {
                object primary = (object)overEighteen ?? (object)underEighteen ?? (object)underSix ?? (object)general;
                if (primary == null) return null;

                KskExamAdo ado = new KskExamAdo();
                if (overEighteen != null) ado.KSK_FORM_NAME = "Khám sức khỏe trên 18 tuổi";
                else if (underEighteen != null) ado.KSK_FORM_NAME = "Khám sức khỏe dưới 18 tuổi";
                else if (underSix != null) ado.KSK_FORM_NAME = "Khám sức khỏe trẻ dưới 6 tuổi";
                else ado.KSK_FORM_NAME = "Khám sức khỏe chung";

                // Mỗi mẫu đặt tên field một khác nên truyền kèm tên thay thế: dưới-18 gộp thần kinh–tâm thần
                // (EXAM_NEURO_MENTAL), dưới-6 (QĐ1551) không có cột EXAM_* mà tách theo cơ quan.
                ado.EXAM_CIRCULATION = Str(primary, general, "EXAM_CIRCULATION", "HEART_AUSCULTATION", "CARDIO_NOTE");
                ado.EXAM_CIRCULATION_RANK = Lng(primary, general, "EXAM_CIRCULATION_RANK");
                ado.EXAM_CIRCULATION_LOGINNAME = Str(primary, general, "EXAM_CIRCULATION_LOGINNAME", "CARDIO_LOGINNAME");

                ado.EXAM_RESPIRATORY = Str(primary, general, "EXAM_RESPIRATORY", "LUNG_AUSCULTATION", "RESP_NOTE");
                ado.EXAM_RESPIRATORY_RANK = Lng(primary, general, "EXAM_RESPIRATORY_RANK");
                ado.EXAM_RESPIRATORY_LOGINNAME = Str(primary, general, "EXAM_RESPIRATORY_LOGINNAME", "RESP_LOGINNAME");

                ado.EXAM_DIGESTION = Str(primary, general, "EXAM_DIGESTION", "ABDOMEN_NOTE");
                ado.EXAM_DIGESTION_RANK = Lng(primary, general, "EXAM_DIGESTION_RANK");
                ado.EXAM_DIGESTION_LOGINNAME = Str(primary, general, "EXAM_DIGESTION_LOGINNAME", "ABDOMEN_LOGINNAME");

                ado.EXAM_KIDNEY_UROLOGY = Str(primary, general, "EXAM_KIDNEY_UROLOGY");
                ado.EXAM_KIDNEY_UROLOGY_RANK = Lng(primary, general, "EXAM_KIDNEY_UROLOGY_RANK");
                ado.EXAM_KIDNEY_UROLOGY_LOGINNAME = Str(primary, general, "EXAM_KIDNEY_UROLOGY_LOGINNAME");

                ado.EXAM_NEUROLOGICAL = Str(primary, general, "EXAM_NEUROLOGICAL", "EXAM_NEURO_MENTAL");
                ado.EXAM_NEUROLOGICAL_RANK = Lng(primary, general, "EXAM_NEUROLOGICAL_RANK", "EXAM_NEURO_MENTAL_RANK");
                ado.EXAM_NEUROLOGICAL_LOGINNAME = Str(primary, general, "EXAM_NEUROLOGICAL_LOGINNAME", "EXAM_NEURO_MENTAL_LOGINNAME");

                ado.EXAM_MENTAL = Str(primary, general, "EXAM_MENTAL");
                ado.EXAM_MENTAL_RANK = Lng(primary, general, "EXAM_MENTAL_RANK");
                ado.EXAM_MENTAL_LOGINNAME = Str(primary, general, "EXAM_MENTAL_LOGINNAME");

                ado.EXAM_MUSCLE_BONE = Str(primary, general, "EXAM_MUSCLE_BONE", "MUSCULOSKELETAL_NOTE");
                ado.EXAM_MUSCLE_BONE_RANK = Lng(primary, general, "EXAM_MUSCLE_BONE_RANK");
                ado.EXAM_MUSCLE_BONE_LOGINNAME = Str(primary, general, "EXAM_MUSCLE_BONE_LOGINNAME", "MUSCULOSKELETAL_LOGINNAME");

                ado.EXAM_DERMATOLOGY = Str(primary, general, "EXAM_DERMATOLOGY", "SKIN_NOTE");
                ado.EXAM_DERMATOLOGY_RANK = Lng(primary, general, "EXAM_DERMATOLOGY_RANK");
                ado.EXAM_DERMATOLOGY_LOGINNAME = Str(primary, general, "EXAM_DERMATOLOGY_LOGINNAME", "SKIN_LOGINNAME");

                ado.EXAM_SURGERY = Str(primary, general, "EXAM_SURGERY");
                ado.EXAM_SURGERY_RANK = Lng(primary, general, "EXAM_SURGERY_RANK");
                ado.EXAM_SURGERY_LOGINNAME = Str(primary, general, "EXAM_SURGERY_LOGINNAME");

                ado.EXAM_OBSTETRIC = Str(primary, general, "EXAM_OBSTETRIC");
                ado.EXAM_OBSTETRIC_RANK = Lng(primary, general, "EXAM_OBSTETRIC_RANK");
                ado.EXAM_OBSTETRIC_LOGINNAME = Str(primary, general, "EXAM_OBSTETRIC_LOGINNAME");

                ado.EXAM_OEND = Str(primary, general, "EXAM_OEND");
                ado.EXAM_OEND_RANK = Lng(primary, general, "EXAM_OEND_RANK");
                ado.EXAM_OEND_LOGINNAME = Str(primary, general, "EXAM_OEND_LOGINNAME");

                ado.EXAM_EYE = Str(primary, general, "EXAM_EYE", "EYE_NOTE");
                ado.EXAM_EYE_DISEASE = Str(primary, general, "EXAM_EYE_DISEASE", "EYE_NOTE");
                ado.EXAM_EYE_RANK = Lng(primary, general, "EXAM_EYE_RANK");
                ado.EXAM_EYE_LOGINNAME = Str(primary, general, "EXAM_EYE_LOGINNAME", "EYE_LOGINNAME");
                ado.EXAM_EYESIGHT_LEFT = Str(primary, general, "EXAM_EYESIGHT_LEFT");
                ado.EXAM_EYESIGHT_RIGHT = Str(primary, general, "EXAM_EYESIGHT_RIGHT");
                ado.EXAM_EYESIGHT_GLASS_LEFT = Str(primary, general, "EXAM_EYESIGHT_GLASS_LEFT");
                ado.EXAM_EYESIGHT_GLASS_RIGHT = Str(primary, general, "EXAM_EYESIGHT_GLASS_RIGHT");

                ado.EXAM_ENT = Str(primary, general, "EXAM_ENT", "NOSETHROAT_NOTE", "EAR_NOTE");
                ado.EXAM_ENT_DISEASE = Str(primary, general, "EXAM_ENT_DISEASE", "NOSETHROAT_NOTE", "EAR_NOTE");
                ado.EXAM_ENT_RANK = Lng(primary, general, "EXAM_ENT_RANK");
                ado.EXAM_ENT_LOGINNAME = Str(primary, general, "EXAM_ENT_LOGINNAME", "NOSETHROAT_LOGINNAME", "EAR_LOGINNAME");
                ado.EXAM_ENT_LEFT_NORMAL = Str(primary, general, "EXAM_ENT_LEFT_NORMAL");
                ado.EXAM_ENT_LEFT_WHISPER = Str(primary, general, "EXAM_ENT_LEFT_WHISPER");
                ado.EXAM_ENT_RIGHT_NORMAL = Str(primary, general, "EXAM_ENT_RIGHT_NORMAL");
                ado.EXAM_ENT_RIGHT_WHISPER = Str(primary, general, "EXAM_ENT_RIGHT_WHISPER");

                ado.EXAM_STOMATOLOGY = Str(primary, general, "EXAM_STOMATOLOGY", "MOUTHTEETH_NOTE");
                ado.EXAM_STOMATOLOGY_DISEASE = Str(primary, general, "EXAM_STOMATOLOGY_DISEASE", "MOUTHTEETH_NOTE");
                ado.EXAM_STOMATOLOGY_RANK = Lng(primary, general, "EXAM_STOMATOLOGY_RANK");
                ado.EXAM_STOMATOLOGY_LOGINNAME = Str(primary, general, "EXAM_STOMATOLOGY_LOGINNAME", "MOUTHTEETH_LOGINNAME");
                ado.EXAM_STOMATOLOGY_UPPER = Str(primary, general, "EXAM_STOMATOLOGY_UPPER");
                ado.EXAM_STOMATOLOGY_LOWER = Str(primary, general, "EXAM_STOMATOLOGY_LOWER");

                // Bệnh tật khác: dưới-18 ghi ở PROBLEM_HEALTH, dưới-6 ghi ở CLINICAL_OBSERVATION.
                ado.DISEASES = Str(primary, general, "DISEASES", "PROBLEM_HEALTH", "CLINICAL_OBSERVATION");
                ado.TREATMENT_INSTRUCTION = Str(primary, general, "TREATMENT_INSTRUCTION");
                ado.RESULT_SUBCLINICAL = Str(primary, general, "RESULT_SUBCLINICAL", "RESULT_DIIM");
                ado.PATHOLOGICAL_HISTORY = Str(primary, general, "PATHOLOGICAL_HISTORY", "HISTORY_PERSONAL");
                ado.PATHOLOGICAL_HISTORY_FAMILY = Str(primary, general, "PATHOLOGICAL_HISTORY_FAMILY", "HISTORY_FAMILY");

                ado.HEALTH_EXAM_RANK_ID = Lng(primary, general, "HEALTH_EXAM_RANK_ID");
                if (ado.HEALTH_EXAM_RANK_ID != null && healthExamRanks != null && healthExamRanks.Count > 0)
                {
                    var rank = healthExamRanks.FirstOrDefault(o => o.ID == ado.HEALTH_EXAM_RANK_ID);
                    if (rank != null)
                    {
                        ado.HEALTH_EXAM_RANK_CODE = rank.HEALTH_EXAM_RANK_CODE;
                        ado.HEALTH_EXAM_RANK_NAME = rank.HEALTH_EXAM_RANK_NAME;
                    }
                }

                ado.DHST_ID = Lng(primary, general, "DHST_ID");
                ado.DHST_RANK = Lng(primary, general, "DHST_RANK");

                return ado;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        /// <summary>Đọc chuỗi theo danh sách tên field (thử lần lượt trên bản ghi ưu tiên, hết mới sang khám chung).</summary>
        private static string Str(object primary, object fallback, params string[] names)
        {
            string value = GetStr(primary, names);
            return !String.IsNullOrEmpty(value) ? value : GetStr(fallback, names);
        }

        /// <summary>Đọc số theo danh sách tên field (thử lần lượt trên bản ghi ưu tiên, hết mới sang khám chung).</summary>
        private static long? Lng(object primary, object fallback, params string[] names)
        {
            long? value = GetLng(primary, names);
            return value ?? GetLng(fallback, names);
        }

        private static string GetStr(object data, params string[] names)
        {
            if (data == null || names == null) return null;
            foreach (var name in names)
            {
                try
                {
                    var pi = data.GetType().GetProperty(name);
                    if (pi == null) continue;
                    var value = pi.GetValue(data, null);
                    if (value == null) continue;
                    string text = value.ToString();
                    if (!String.IsNullOrEmpty(text)) return text;
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }
            return null;
        }

        private static long? GetLng(object data, params string[] names)
        {
            if (data == null || names == null) return null;
            foreach (var name in names)
            {
                try
                {
                    var pi = data.GetType().GetProperty(name);
                    if (pi == null) continue;
                    var value = pi.GetValue(data, null);
                    if (value != null) return Convert.ToInt64(value);
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }
            return null;
        }
    }
}
