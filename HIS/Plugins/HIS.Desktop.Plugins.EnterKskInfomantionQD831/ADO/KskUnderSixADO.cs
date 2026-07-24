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

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.ADO
{
    /// <summary>
    /// Model cục bộ (local) cho mẫu Khám sức khỏe trẻ em dưới 6 tuổi.
    /// TODO(backend): khi backend ra EFMODEL HIS_KSK_UNDER_SIX + Filter + SDO + endpoint,
    /// thay class này bằng EFMODEL thật (hoặc map 1-1 sang EFMODEL trong tầng lưu).
    /// Quy ước giá trị: tham chiếu spec cột A–P (xem comment từng trường).
    /// </summary>
    public class KskUnderSixADO
    {
        #region Liên kết & hành chính bổ sung (A)
        public long? ID { get; set; }
        public long? SERVICE_REQ_ID { get; set; }            // 1 - Liên kết yêu cầu khám
        public long? TDL_TREATMENT_ID { get; set; }          // 2 - Liên kết lượt điều trị
        public long? TDL_PATIENT_ID { get; set; }            // 3 - Liên kết bệnh nhân
        public long? DHST_ID { get; set; }                   // 4 - Liên kết chỉ số sinh tồn HIS_DHST
        public long? IS_PREMATURE_BIRTH { get; set; }        // 5 - Sinh non: 1=Có, 0=Không
        public string ETHNIC { get; set; }                   // 6 - Dân tộc
        public string RESIDENCE { get; set; }                // 7 - Nơi ở
        public string ACCOMPANY_PERSON_NAME { get; set; }    // 8 - Họ tên người đi cùng trẻ
        public long? ACCOMPANY_RELATIONSHIP { get; set; }    // 9 - 1=Cha,2=Mẹ,3=Ông/bà,4=Anh/chị,5=Họ hàng,6=Khác
        public string ACCOMPANY_RELATIONSHIP_OTHER { get; set; } // 10 - Ghi rõ khi "Khác"
        public string HISTORY_PERSONAL { get; set; }         // 11 - Tiền sử bản thân
        public string HISTORY_FAMILY { get; set; }           // 12 - Tiền sử gia đình
        public long? IS_TB_CONTACT { get; set; }             // 13 - Tiền sử tiếp xúc người bệnh lao: 1=Có, 0=Không
        #endregion

        #region Dấu hiệu sinh tồn (B) — số đo VARCHAR2(500) nhập tự do (theo DDL); *_EVAL = NUMBER
        public string TEMPERATURE { get; set; }              // Nhiệt độ (nhập tự do, kèm đơn vị nếu cần)
        public long? TEMPERATURE_EVAL { get; set; }          // 1=Bình thường, 2=Sốt, 3=Hạ thân nhiệt
        public string PULSE { get; set; }                    // Mạch (nhập tự do)
        public long? PULSE_EVAL { get; set; }                // 1=Bình thường, 2=Nhanh
        public string RESPIRATORY_RATE { get; set; }         // Nhịp thở (nhập tự do)
        public long? RESPIRATORY_EVAL { get; set; }          // 1=Bình thường, 2=Thở nhanh, 3=Thở chậm
        #endregion

        #region Đánh giá dinh dưỡng (C) — số đo VARCHAR2(500) nhập tự do
        public string BODY_LENGTH { get; set; }              // Chiều dài (nhập tự do)
        public string BODY_LENGTH_AGE_SD { get; set; }       // Chiều dài/Tuổi (SD) (nhập tự do)
        public string WEIGHT { get; set; }                   // Cân nặng (nhập tự do)
        public string WEIGHT_AGE_SD { get; set; }            // Cân nặng/Tuổi (SD) (nhập tự do)
        public string HEAD_CIRCUMFERENCE { get; set; }       // Vòng đầu (nhập tự do)
        public long? HEAD_CIRC_EVAL { get; set; }            // 1=Bình thường, 2=Đầu to, 3=Đầu nhỏ
        public string ARM_CIRCUMFERENCE { get; set; }        // Chu vi vòng cánh tay (nhập tự do)
        public long? IS_NUTRITIONAL_EDEMA { get; set; }      // Phù dinh dưỡng: 1=Có
        public long? IS_ANEMIA_SIGN { get; set; }            // Dấu hiệu thiếu máu: 1=Có
        public long? IS_RICKETS_SIGN { get; set; }           // Dấu hiệu còi xương: 1=Có
        public long? IS_MALNUTRITION { get; set; }           // Suy dinh dưỡng: 1=Có
        public long? IS_OVERWEIGHT { get; set; }             // Thừa cân/béo phì: 1=Có
        #endregion

        #region Phát triển tinh thần - vận động (D)
        public long? MENTAL_DEV_NORMAL { get; set; }         // 1=Có, 0=Không
        public long? MOTOR_DEV_NORMAL { get; set; }          // 1=Có, 0=Không
        public long? AUTISM_RISK { get; set; }               // 1=Có, 0=Không (16–30 tháng)
        #endregion

        #region Tiêm chủng (E)
        public long? VACCINE_TB { get; set; }                // Lao (sơ sinh): 1=Có, 0=Không
        public long? VACCINE_HEPB1 { get; set; }             // Viêm gan B mũi 1: 1=Có, 0=Không
        public long? VACCINE_FULL_BY_AGE { get; set; }       // Đầy đủ theo độ tuổi: 1=Có, 0=Không
        #endregion

        #region Khám lâm sàng — Quan sát chung & Da (F)
        public string CLINICAL_OBSERVATION { get; set; }     // Quan sát chung
        public long? SKIN_COLOR { get; set; }                // 1=Hồng hào,2=Nhợt,3=Tím,4=Vàng,5=Sạm da
        public long? PALM_EVAL { get; set; }                 // 1=Bình thường(không nhợt), 2=Không bình thường(nhợt)
        public string SKIN_NOTE { get; set; }                // Ghi chú mục Da
        #endregion

        #region Đầu - cổ (G)
        public long? FONTANEL { get; set; }                  // 1=Bình thường,2=Rộng,3=Hẹp,4=Thóp phồng
        public long? HEAD_SHAPE { get; set; }                // 1=Bình thường, 2=Không bình thường
        public long? NECK_MOTION { get; set; }               // 1=Bình thường, 2=Giới hạn
        public long? HEAD_ABNORMAL_MASS { get; set; }        // Khối bất thường: 1=Có, 0=Không
        public string HEADNECK_NOTE { get; set; }            // Ghi chú mục Đầu - cổ
        #endregion

        #region Mắt (H)
        public long? EYE_POSITION { get; set; }              // 1=Bình thường, 2=Hai mắt xa nhau
        public long? EYELID_CONJUNCTIVA { get; set; }        // 1=Bình thường, 2=Sưng/đỏ, 3=Chảy ghèn/mủ
        public long? PUPIL { get; set; }                     // 1=Bình thường, 2=Không bình thường
        public long? STRABISMUS { get; set; }                // Lác mắt: 1=Có, 0=Không
        public string EYE_NOTE { get; set; }                 // Ghi chú mục Mắt
        #endregion

        #region Tai (I)
        public long? EAR_EARDRUM { get; set; }               // 1=Bình thường, 2=Không bình thường
        public long? SOUND_RESPONSE { get; set; }            // 1=Bình thường, 2=Không bình thường
        public long? EAR_SWELLING { get; set; }              // Khối sưng sau tai: 1=Có, 0=Không
        public long? EAR_DISCHARGE { get; set; }             // Chảy mủ/nước tai: 1=Có, 0=Không
        public string EAR_NOTE { get; set; }                 // Ghi chú mục Tai
        #endregion

        #region Mũi - họng (J)
        public long? NOSE_SHAPE { get; set; }                // 1=Bình thường, 2=Mũi to/dày, 3=Bất sản xương mũi
        public long? RUNNY_NOSE { get; set; }                // Chảy nước mũi: 1=Có, 0=Không
        public long? STUFFY_NOSE { get; set; }               // Nghẹt mũi: 1=Có, 0=Không
        public long? THROAT { get; set; }                    // 1=Bình thường, 2=Không bình thường
        public string NOSETHROAT_NOTE { get; set; }          // Ghi chú mục Mũi - họng
        #endregion

        #region Miệng, răng (K)
        public long? MOUTH_SHAPE { get; set; }               // 1=Bình thường, 2=Sứt môi, chẻ vòm
        public long? NEONATAL_TEETH { get; set; }            // Răng sữa sơ sinh: 0=Bình thường, 1=Có
        public long? TONGUE_SHAPE { get; set; }              // 1=Bình thường, 2=Lưỡi to bè
        public long? TONGUE_TIE { get; set; }                // Dính thắng lưỡi: 1=Có, 0=Không
        public long? ORAL_THRUSH { get; set; }               // Nấm miệng: 1=Có, 0=Không
        public long? SMALL_CHIN { get; set; }                // Cằm nhỏ, tụt về sau: 1=Có, 0=Không
        public long? TOOTH_DECAY { get; set; }               // Vết sâu, mảng bám, lỗ trên răng: 1=Có, 0=Không
        public string MOUTHTEETH_NOTE { get; set; }          // Ghi chú mục Miệng, răng
        #endregion

        #region Hô hấp (L)
        public long? IRREGULAR_BREATH { get; set; }          // Nhịp thở không đều/ngưng thở >5s: 1=Có, 0=Không
        public long? CHEST_RETRACTION { get; set; }          // Thở rút lõm lồng ngực: 1=Có, 0=Không
        public long? ABNORMAL_BREATH_SOUND { get; set; }     // Tiếng thở bất thường: 1=Có, 0=Không
        public long? RESP_FAILURE_SIGN { get; set; }         // Dấu hiệu suy hô hấp: 1=Có, 0=Không
        public long? LUNG_AUSCULTATION { get; set; }         // Nghe phổi: 1=Bình thường, 2=Không bình thường
        public string RESP_NOTE { get; set; }                // Ghi chú mục Hô hấp
        #endregion

        #region Tim mạch (M)
        public long? APEX_POSITION { get; set; }             // Vị trí mỏm tim: 1=Bình thường, 2=Không bình thường
        public long? PERIPHERAL_PULSE { get; set; }          // Mạch ngoại vi: 1=Bắt rõ, 2=Mạch nhẹ, 3=Không bắt được
        public long? HEART_AUSCULTATION { get; set; }        // Nghe tim: 1=Bình thường, 2=Không bình thường
        public string CARDIO_NOTE { get; set; }              // Ghi chú mục Tim mạch
        #endregion

        #region Bụng và cơ quan sinh dục (N)
        public long? ABDOMEN_NAVEL { get; set; }             // Hình dáng bụng, rốn: 1=Bình thường, 2=Không bình thường
        public long? HEPATOSPLENOMEGALY { get; set; }        // Gan, lách to: 1=Có, 0=Không
        public long? ABDOMEN_MASS { get; set; }              // Khối bất thường: 1=Có, 0=Không
        public long? ANUS { get; set; }                      // Lỗ hậu môn: 1=Bình thường, 2=Không bình thường
        public long? GENITALIA { get; set; }                 // Cơ quan sinh dục ngoài: 1=Bình thường, 2=Không bình thường
        public string ABDOMEN_NOTE { get; set; }             // Ghi chú mục Bụng và cơ quan sinh dục
        #endregion

        #region Cơ xương và thần kinh (O)
        public long? ASYMMETRIC_MOVEMENT { get; set; }       // Vận động không đối xứng: 1=Có, 0=Không
        public long? SUCKING_REFLEX { get; set; }            // Phản xạ bú: 1=Có, 0=Không
        public long? GRASP_REFLEX { get; set; }              // Phản xạ nắm: 1=Có, 0=Không
        public long? MORO_REFLEX { get; set; }               // Phản xạ Moro: 1=Có, 0=Không
        public long? MUSCLE_TONE { get; set; }               // Trương lực cơ: 1=Bình thường, 2=Tăng, 3=Giảm
        public long? HIP_JOINT { get; set; }                 // Khớp háng: 1=Bình thường, 2=Trật khớp háng
        public long? MUSCLE_REFLEX { get; set; }             // Phản xạ cơ: 1=Bình thường, 2=Không bình thường
        public long? SPINE_CHECK { get; set; }               // Kiểm tra lưng, cột sống: 1=Bình thường, 2=Không bình thường
        public long? LIMBS_JOINTS { get; set; }              // Khám tứ chi và khớp: 1=Bình thường, 2=Không bình thường
        public long? GAIT { get; set; }                      // Quan sát dáng đi: 1=Bình thường, 2=Không bình thường
        public long? RICKETS_SIGN_NEURO { get; set; }        // Dấu hiệu còi xương: 1=Có, 0=Không
        public string MUSCULOSKELETAL_NOTE { get; set; }     // Ghi chú mục Cơ xương và thần kinh
        #endregion

        #region Kết luận và tư vấn (P) — LƯU SANG HIS_KSK_GENERAL (cùng SERVICE_REQ_ID), không lưu ở bảng này
        public long? HEALTH_CONCLUSION_TYPE { get; set; }    // GENERAL.HEALTH_CONCLUSION_TYPE: 1=Bình thường, 2=Có nguy cơ mắc lao, 3=Có vấn đề về sức khỏe
        public string DISEASES { get; set; }                 // GENERAL.DISEASES — "Ghi rõ" (chi tiết kết luận)
        public string TREATMENT_INSTRUCTION { get; set; }    // GENERAL.TREATMENT_INSTRUCTION — Tư vấn và hẹn khám lần sau
        public long? HEALTH_EXAM_RANK_ID { get; set; }       // GENERAL.HEALTH_EXAM_RANK_ID — Xếp loại tình trạng sức khỏe chung
        public string CONCLUDER_LOGINNAME { get; set; }      // GENERAL.CONCLUDER_LOGINNAME — Bác sĩ kết luận (tài khoản)
        public string CONCLUDER_USERNAME { get; set; }       // GENERAL.CONCLUDER_USERNAME — Bác sĩ kết luận (họ tên)
        public long? CONCLUSION_TIME { get; set; }           // GENERAL.CONCLUSION_TIME — Thời gian kết luận (yyyyMMddHHmmss)
        #endregion

        #region Kết luận theo bệnh (ICD-10) — LƯU SANG HIS_KSK_GENERAL
        public long? CONCLUSION_ICD_TYPE { get; set; }       // GENERAL.CONCLUSION_ICD_TYPE: 1=Chưa phát hiện bất thường, 2=Chẩn đoán sơ bộ (ICD), 3=Chẩn đoán xác định (ICD)
        public string CONCLUSION_ICD_CODE { get; set; }      // GENERAL.CONCLUSION_ICD_CODE — mã ICD ghép bằng ';'
        public string CONCLUSION_ICD_NAME { get; set; }      // GENERAL.CONCLUSION_ICD_NAME — tên ICD ghép bằng ';'
        #endregion
    }
}
