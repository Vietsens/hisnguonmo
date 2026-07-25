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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000516.ADO;
using MPS.Processor.Mps000516.PDO;
using MPS.ProcessorBase.Core;
namespace MPS.Processor.Mps000516
{
    /// <summary>
    /// Phiếu Khám sức khỏe trẻ em dưới 6 tuổi (HIS_KSK_UNDER_SIX).
    /// Khuôn theo Mps000453 (Under 18). Điểm khác biệt: bảng UNDER_SIX lưu MÃ SỐ cho cột
    /// "1 trong nhiều" và 1/0 cho cờ, nên ngoài key thô còn phải MAP mã → text + cờ → "x".
    /// </summary>
    public class Mps000516Processor : AbstractProcessor
    {
        Mps000516PDO rdo;
        TreatmentAdo TreatmentAdos { get; set; }

        public Mps000516Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000516PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                Inventec.Common.FlexCellExport.ProcessBarCodeTag barCodeTag = new Inventec.Common.FlexCellExport.ProcessBarCodeTag();

                TreatmentAdos = new TreatmentAdo();
                if (rdo.treatment != null)
                {
                    TreatmentAdo ado = new TreatmentAdo();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentAdo>(ado, rdo.treatment);
                    TreatmentAdos = ado;
                }
                SetImageKey();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                objectTag.AddObjectData(store, "KskUnderSix", new List<HIS_KSK_UNDER_SIX>() { rdo.HisKskUnderSix });
                objectTag.AddObjectData(store, "Treatment", new List<TreatmentAdo>() { TreatmentAdos });
                objectTag.AddObjectData(store, "Dhst", new List<HIS_DHST>() { rdo.HisDhst });
                // Kết luận & tư vấn (mục P) nằm ở HIS_KSK_GENERAL → template dùng {KskGeneral.DISEASES}, {KskGeneral.CONCLUDER_USERNAME}...
                objectTag.AddObjectData(store, "KskGeneral", new List<HIS_KSK_GENERAL>() { rdo.HisKskGeneral ?? new HIS_KSK_GENERAL() });
                objectTag.AddRelationship(store, "KskUnderSix", "Dhst", "DHST_ID", "ID");

                SetSingleKey();
                SetSignatureKeyImageByCFG();

                objectTag.AddObjectData(store, "ExamRank", rdo.examRank ?? new List<HIS_HEALTH_EXAM_RANK>());

                singleTag.ProcessData(store, singleValueDictionary);
                barCodeTag.ProcessData(store, dicImage);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                // Y lệnh KSK (entity HIS_SERVICE_REQ) + bệnh nhân (HIS_PATIENT) — key prefix SREQ_ / PATIENT_
                if (rdo.KskServiceReq != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<HIS_SERVICE_REQ>(rdo.KskServiceReq, "SREQ_", false);
                }
                if (rdo.KskPatient != null)
                {
                    AddObjectKeyIntoListkeyWithPrefix<HIS_PATIENT>(rdo.KskPatient, "PATIENT_", false);
                }
                if (rdo.HisKskUnderSix != null)
                {
                    AddObjectKeyIntoListkey<HIS_KSK_UNDER_SIX>(rdo.HisKskUnderSix, false);
                }
                // Số thứ tự KSK -> key chung {KSK_NUMBER} (ngoài key thô {KSK_UNDER_SIX_CODE})
                SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.KSK_NUMBER,
                    rdo.HisKskUnderSix != null ? rdo.HisKskUnderSix.KSK_UNDER_SIX_CODE : ""));
                if (rdo.HisServiceReq != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(rdo.HisServiceReq, false);
                }
                if (rdo.HisDhst != null)
                {
                    // KHÔNG AddObjectKeyIntoListkey<HIS_DHST> để tránh đụng key với HIS_KSK_UNDER_SIX
                    // (WEIGHT/PULSE/TEMPERATURE... có ở cả 2 bảng). Form dưới-6-tuổi lấy số đo từ bảng KSK.
                    // Vẫn giữ DHST qua object-tag {Dhst.x} + quan hệ DHST_ID nếu template cần.
                    SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.DHST_LOGINNAME, rdo.HisDhst.EXECUTE_LOGINNAME));
                }

                SetMappedKeys();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sinh các key phái sinh: code "1 trong nhiều" → text (&lt;COLUMN&gt;_STR),
        /// cờ 1/0 → "x"/"" (&lt;COLUMN&gt;_X), kết luận, xếp loại, thời gian kết luận.
        /// </summary>
        private void SetMappedKeys()
        {
            var k = rdo.HisKskUnderSix;
            if (k == null) return;

            // ---- A. Hành chính ----
            SetFlag("IS_PREMATURE_BIRTH", k.IS_PREMATURE_BIRTH);
            SetFlag("IS_TB_CONTACT", k.IS_TB_CONTACT);
            SetCodeStr("ACCOMPANY_RELATIONSHIP", k.ACCOMPANY_RELATIONSHIP, MAP_RELATION);
            // Mối quan hệ đầy đủ: nếu "Khác" thì ghép phần ghi rõ
            string relFull = MapCode(k.ACCOMPANY_RELATIONSHIP, MAP_RELATION);
            if (N(k.ACCOMPANY_RELATIONSHIP) == 6 && !string.IsNullOrEmpty(k.ACCOMPANY_RELATIONSHIP_OTHER))
                relFull = string.IsNullOrEmpty(relFull) ? k.ACCOMPANY_RELATIONSHIP_OTHER : relFull + ": " + k.ACCOMPANY_RELATIONSHIP_OTHER;
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.ACCOMPANY_RELATIONSHIP_FULL, relFull));

            // ---- B. Dấu hiệu sinh tồn ----
            SetCodeStr("TEMPERATURE_EVAL", k.TEMPERATURE_EVAL, MAP_TEMPERATURE);
            SetCodeStr("PULSE_EVAL", k.PULSE_EVAL, MAP_PULSE);
            SetCodeStr("RESPIRATORY_EVAL", k.RESPIRATORY_EVAL, MAP_RESPIRATORY);

            // ---- C. Dinh dưỡng ----
            SetCodeStr("HEAD_CIRC_EVAL", k.HEAD_CIRC_EVAL, MAP_HEAD_CIRC);
            SetFlag("IS_NUTRITIONAL_EDEMA", k.IS_NUTRITIONAL_EDEMA);
            SetFlag("IS_ANEMIA_SIGN", k.IS_ANEMIA_SIGN);
            SetFlag("IS_RICKETS_SIGN", k.IS_RICKETS_SIGN);
            SetFlag("IS_MALNUTRITION", k.IS_MALNUTRITION);
            SetFlag("IS_OVERWEIGHT", k.IS_OVERWEIGHT);

            // ---- D. Phát triển tinh thần - vận động ----
            SetFlag("MENTAL_DEV_NORMAL", k.MENTAL_DEV_NORMAL);
            SetFlag("MOTOR_DEV_NORMAL", k.MOTOR_DEV_NORMAL);
            SetFlag("AUTISM_RISK", k.AUTISM_RISK);

            // ---- E. Tiêm chủng ----
            SetFlag("VACCINE_TB", k.VACCINE_TB);
            SetFlag("VACCINE_HEPB1", k.VACCINE_HEPB1);
            SetFlag("VACCINE_FULL_BY_AGE", k.VACCINE_FULL_BY_AGE);

            // ---- F. Quan sát chung & Da ----
            SetCodeStr("SKIN_COLOR", k.SKIN_COLOR, MAP_SKIN_COLOR);
            SetCodeStr("PALM_EVAL", k.PALM_EVAL, MAP_PALM);

            // ---- G. Đầu - cổ ----
            SetCodeStr("FONTANEL", k.FONTANEL, MAP_FONTANEL);
            SetCodeStr("HEAD_SHAPE", k.HEAD_SHAPE, MAP_NORMAL2);
            SetCodeStr("NECK_MOTION", k.NECK_MOTION, MAP_NECK_MOTION);
            SetFlag("HEAD_ABNORMAL_MASS", k.HEAD_ABNORMAL_MASS);

            // ---- H. Mắt ----
            SetCodeStr("EYE_POSITION", k.EYE_POSITION, MAP_EYE_POSITION);
            SetCodeStr("EYELID_CONJUNCTIVA", k.EYELID_CONJUNCTIVA, MAP_EYELID);
            SetCodeStr("PUPIL", k.PUPIL, MAP_NORMAL2);
            SetFlag("STRABISMUS", k.STRABISMUS);

            // ---- I. Tai ----
            SetCodeStr("EAR_EARDRUM", k.EAR_EARDRUM, MAP_NORMAL2);
            SetCodeStr("SOUND_RESPONSE", k.SOUND_RESPONSE, MAP_NORMAL2);
            SetFlag("EAR_SWELLING", k.EAR_SWELLING);
            SetFlag("EAR_DISCHARGE", k.EAR_DISCHARGE);

            // ---- J. Mũi - họng ----
            SetCodeStr("NOSE_SHAPE", k.NOSE_SHAPE, MAP_NOSE_SHAPE);
            SetFlag("RUNNY_NOSE", k.RUNNY_NOSE);
            SetFlag("STUFFY_NOSE", k.STUFFY_NOSE);
            SetCodeStr("THROAT", k.THROAT, MAP_NORMAL2);

            // ---- K. Miệng, răng ----
            SetCodeStr("MOUTH_SHAPE", k.MOUTH_SHAPE, MAP_MOUTH_SHAPE);
            SetFlag("NEONATAL_TEETH", k.NEONATAL_TEETH);
            SetCodeStr("TONGUE_SHAPE", k.TONGUE_SHAPE, MAP_TONGUE_SHAPE);
            SetFlag("TONGUE_TIE", k.TONGUE_TIE);
            SetFlag("ORAL_THRUSH", k.ORAL_THRUSH);
            SetFlag("SMALL_CHIN", k.SMALL_CHIN);
            SetFlag("TOOTH_DECAY", k.TOOTH_DECAY);

            // ---- L. Hô hấp ----
            SetFlag("IRREGULAR_BREATH", k.IRREGULAR_BREATH);
            SetFlag("CHEST_RETRACTION", k.CHEST_RETRACTION);
            SetFlag("ABNORMAL_BREATH_SOUND", k.ABNORMAL_BREATH_SOUND);
            SetFlag("RESP_FAILURE_SIGN", k.RESP_FAILURE_SIGN);
            SetCodeStr("LUNG_AUSCULTATION", k.LUNG_AUSCULTATION, MAP_NORMAL2);

            // ---- M. Tim mạch ----
            SetCodeStr("APEX_POSITION", k.APEX_POSITION, MAP_NORMAL2);
            SetCodeStr("PERIPHERAL_PULSE", k.PERIPHERAL_PULSE, MAP_PERIPHERAL_PULSE);
            SetCodeStr("HEART_AUSCULTATION", k.HEART_AUSCULTATION, MAP_NORMAL2);

            // ---- N. Bụng và cơ quan sinh dục ----
            SetCodeStr("ABDOMEN_NAVEL", k.ABDOMEN_NAVEL, MAP_NORMAL2);
            SetFlag("HEPATOSPLENOMEGALY", k.HEPATOSPLENOMEGALY);
            SetFlag("ABDOMEN_MASS", k.ABDOMEN_MASS);
            SetCodeStr("ANUS", k.ANUS, MAP_NORMAL2);
            SetCodeStr("GENITALIA", k.GENITALIA, MAP_NORMAL2);

            // ---- O. Cơ xương và thần kinh ----
            SetFlag("ASYMMETRIC_MOVEMENT", k.ASYMMETRIC_MOVEMENT);
            SetFlag("SUCKING_REFLEX", k.SUCKING_REFLEX);
            SetFlag("GRASP_REFLEX", k.GRASP_REFLEX);
            SetFlag("MORO_REFLEX", k.MORO_REFLEX);
            SetCodeStr("MUSCLE_TONE", k.MUSCLE_TONE, MAP_MUSCLE_TONE);
            SetCodeStr("HIP_JOINT", k.HIP_JOINT, MAP_HIP_JOINT);
            SetCodeStr("MUSCLE_REFLEX", k.MUSCLE_REFLEX, MAP_NORMAL2);
            SetCodeStr("SPINE_CHECK", k.SPINE_CHECK, MAP_NORMAL2);
            SetCodeStr("LIMBS_JOINTS", k.LIMBS_JOINTS, MAP_NORMAL2);
            SetCodeStr("GAIT", k.GAIT, MAP_NORMAL2);
            SetFlag("RICKETS_SIGN_NEURO", k.RICKETS_SIGN_NEURO);

            // ---- P. Kết luận & tư vấn (lưu ở HIS_KSK_GENERAL, KHÔNG ở HIS_KSK_UNDER_SIX) ----
            SetConclusionKeysFromGeneral();
        }

        /// <summary>
        /// Sinh key kết luận từ HIS_KSK_GENERAL (cùng SERVICE_REQ_ID):
        /// Kết luận sức khỏe (HEALTH_CONCLUSION_TYPE 1/2/3), Kết luận theo bệnh ICD (CONCLUSION_ICD_TYPE 1/2/3 + mã/tên),
        /// Ghi rõ (DISEASES), Tư vấn (TREATMENT_INSTRUCTION), bác sĩ kết luận, thời gian, xếp loại.
        /// </summary>
        private void SetConclusionKeysFromGeneral()
        {
            var g = rdo.HisKskGeneral;

            // Kết luận sức khỏe: 1=Bình thường, 2=Có nguy cơ mắc lao, 3=Có vấn đề về sức khỏe
            long? hc = g != null ? N(g.HEALTH_CONCLUSION_TYPE) : null;
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_NORMAL_X, hc == 1 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_TB_RISK_X, hc == 2 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_HEALTH_ISSUE_X, hc == 3 ? "x" : ""));

            // Kết luận theo bệnh ICD-10: 1=Chưa phát hiện, 2=Chẩn đoán sơ bộ, 3=Chẩn đoán xác định
            long? ic = g != null ? N(g.CONCLUSION_ICD_TYPE) : null;
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_ICD_NONE_X, ic == 1 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_ICD_PRELIM_X, ic == 2 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_ICD_FINAL_X, ic == 3 ? "x" : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_ICD_CODE, g != null ? (g.CONCLUSION_ICD_CODE ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_ICD_NAME, g != null ? (g.CONCLUSION_ICD_NAME ?? "") : ""));

            // Văn bản kết luận
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.DISEASES, g != null ? (g.DISEASES ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.TREATMENT_INSTRUCTION, g != null ? (g.TREATMENT_INSTRUCTION ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUDER_USERNAME, g != null ? (g.CONCLUDER_USERNAME ?? "") : ""));
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUDER_LOGINNAME, g != null ? (g.CONCLUDER_LOGINNAME ?? "") : ""));

            // Thời gian kết luận yyyyMMddHHmmss → chuỗi ngày
            long? concTime = g != null ? N(g.CONCLUSION_TIME) : null;
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.CONCLUSION_TIME_STR,
                (concTime.HasValue && concTime.Value > 0)
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(concTime.Value)
                    : ""));

            // Xếp loại sức khỏe chung → tên xếp loại
            SetSingleKey(new KeyValue(Mps000516ExtendSingleKey.HEALTH_EXAM_RANK_NAME,
                GetRankName(g != null ? N(g.HEALTH_EXAM_RANK_ID) : null)));
        }

        private string GetRankName(long? rankId)
        {
            try
            {
                if (!rankId.HasValue || rdo.examRank == null) return "";
                var rank = rdo.examRank.FirstOrDefault(o => o.ID == rankId.Value);
                return rank != null ? rank.HEALTH_EXAM_RANK_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        // ===== Helpers =====

        /// <summary>Đọc giá trị số bất kể EF sinh ra short?/long?/decimal? → long?.</summary>
        private static long? N(object v)
        {
            if (v == null) return null;
            long r;
            return long.TryParse(v.ToString(), out r) ? (long?)r : (long?)null;
        }

        private static string MapCode(object value, Dictionary<long, string> map)
        {
            long? n = N(value);
            return (n.HasValue && map.ContainsKey(n.Value)) ? map[n.Value] : "";
        }

        private void SetCodeStr(string column, object value, Dictionary<long, string> map)
        {
            SetSingleKey(new KeyValue(column + Mps000516ExtendSingleKey.STR_SUFFIX, MapCode(value, map)));
        }

        private void SetFlag(string column, object value)
        {
            SetSingleKey(new KeyValue(column + Mps000516ExtendSingleKey.FLAG_SUFFIX, N(value) == 1 ? "x" : ""));
        }

        internal void SetImageKey()
        {
            try
            {
                // Avatar gắn vào object Treatment (template dùng {Treatment.AVATAR})
                if (TreatmentAdos != null && !string.IsNullOrEmpty(TreatmentAdos.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(TreatmentAdos, TreatmentAdos.TDL_PATIENT_AVATAR_URL);
                }

                // Avatar single-key {IMG_AVATAR} lấy từ service_req (giống Mps000452/Mps000454).
                // Đây là URL mà plugin EnterKskInfomantionVer2 đổ vào qua EnsurePatientAvatarUrlForPrint().
                if (rdo.HisServiceReq != null && !string.IsNullOrEmpty(rdo.HisServiceReq.TDL_PATIENT_AVATAR_URL))
                {
                    SetSingleImage(Mps000516ExtendSingleKey.IMG_AVATAR, rdo.HisServiceReq.TDL_PATIENT_AVATAR_URL);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(TreatmentAdo key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                key.AVATAR = stream != null ? stream.ToArray() : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetSingleImage(string key, string imageUrl)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(imageUrl);
                if (stream != null)
                {
                    SetSingleKey(new KeyValue(key, stream.ToArray()));
                }
                else
                {
                    SetSingleKey(new KeyValue(key, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // ===== Bảng map mã → text (theo đặc tả HIS_KSK_UNDER_SIX) =====

        private static readonly Dictionary<long, string> MAP_NORMAL2 = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Không bình thường" } };

        private static readonly Dictionary<long, string> MAP_RELATION = new Dictionary<long, string>
        { { 1, "Cha" }, { 2, "Mẹ" }, { 3, "Ông/bà" }, { 4, "Anh/chị" }, { 5, "Họ hàng" }, { 6, "Khác" } };

        private static readonly Dictionary<long, string> MAP_TEMPERATURE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Sốt" }, { 3, "Hạ thân nhiệt" } };

        private static readonly Dictionary<long, string> MAP_PULSE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Nhanh" } };

        private static readonly Dictionary<long, string> MAP_RESPIRATORY = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Thở nhanh" }, { 3, "Thở chậm" } };

        private static readonly Dictionary<long, string> MAP_HEAD_CIRC = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Đầu to" }, { 3, "Đầu nhỏ" } };

        private static readonly Dictionary<long, string> MAP_SKIN_COLOR = new Dictionary<long, string>
        { { 1, "Hồng hào" }, { 2, "Nhợt" }, { 3, "Tím" }, { 4, "Vàng" }, { 5, "Sạm da" } };

        private static readonly Dictionary<long, string> MAP_PALM = new Dictionary<long, string>
        { { 1, "Bình thường (không nhợt)" }, { 2, "Không bình thường (nhợt)" } };

        private static readonly Dictionary<long, string> MAP_FONTANEL = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Rộng" }, { 3, "Hẹp" }, { 4, "Thóp phồng" } };

        private static readonly Dictionary<long, string> MAP_NECK_MOTION = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Giới hạn" } };

        private static readonly Dictionary<long, string> MAP_EYE_POSITION = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Hai mắt xa nhau" } };

        private static readonly Dictionary<long, string> MAP_EYELID = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Sưng/đỏ" }, { 3, "Chảy ghèn/mủ" } };

        private static readonly Dictionary<long, string> MAP_NOSE_SHAPE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Mũi to/dày" }, { 3, "Bất sản xương mũi" } };

        private static readonly Dictionary<long, string> MAP_MOUTH_SHAPE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Sứt môi, chẻ vòm" } };

        private static readonly Dictionary<long, string> MAP_TONGUE_SHAPE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Lưỡi to bè" } };

        private static readonly Dictionary<long, string> MAP_PERIPHERAL_PULSE = new Dictionary<long, string>
        { { 1, "Bắt rõ" }, { 2, "Mạch nhẹ" }, { 3, "Không bắt được" } };

        private static readonly Dictionary<long, string> MAP_MUSCLE_TONE = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Tăng" }, { 3, "Giảm" } };

        private static readonly Dictionary<long, string> MAP_HIP_JOINT = new Dictionary<long, string>
        { { 1, "Bình thường" }, { 2, "Trật khớp háng" } };
    }
}
