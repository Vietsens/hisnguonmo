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
namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>
    /// Bản ghi HIS_KSK_PRO_DRIVER (tab "Ksk lái xe Mẫu 5" — Mẫu 5 TT 36/2024/TT-BYT).
    /// TẠM THỜI thay cho entity MOS.EFMODEL: bảng mới tạo 11/11/2026, EFMODEL chưa có class. Tên/kiểu
    /// thuộc tính sinh đúng theo cột DB (USER_TAB_COLUMNS) nên gửi/nhận JSON với api/HisKskProDriver/*
    /// y như entity. Khi EFMODEL có HIS_KSK_PRO_DRIVER thì đổi sang entity và xóa lớp này.
    /// </summary>
    public class KskProDriverADO
    {
        public long ID { get; set; }
        public long? CREATE_TIME { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }
        public long SERVICE_REQ_ID { get; set; }
        public long? TDL_TREATMENT_ID { get; set; }
        public long? TDL_PATIENT_ID { get; set; }
        public long? DHST_ID { get; set; }
        public string KSK_PRO_DRIVER_CODE { get; set; }
        public long? LICENSE_CLASS_ID { get; set; }
        public string WORK_COMMUNE_NAME { get; set; }
        public string KSK_PATIENT_TYPES { get; set; }
        public short? KSK_PAY_SOURCE { get; set; }
        public short? EXAM_PLACE_TYPE { get; set; }
        public short? IS_FAMILY_HISTORY { get; set; }
        public string FAMILY_HISTORY { get; set; }
        public string OTHER_DISEASE { get; set; }
        public short? IS_TREATING { get; set; }
        public string TREATING_MEDICINE { get; set; }
        public short? IS_MATERNITY_HISTORY { get; set; }
        public string MATERNITY_HISTORY { get; set; }
        public long? DHST_RANK { get; set; }
        public string DHST_CONCLUDE { get; set; }
        public short? EXAM_CIRCULATION_IS_NORMAL { get; set; }
        public string EXAM_CIRCULATION_PRE_ICD_CODE { get; set; }
        public string EXAM_CIRCULATION_PRE_ICD_NAME { get; set; }
        public string EXAM_CIRCULATION_ICD_CODE { get; set; }
        public string EXAM_CIRCULATION_ICD_NAME { get; set; }
        public long? EXAM_CIRCULATION_RANK { get; set; }
        public string EXAM_CIRCULATION_CONCLUDE { get; set; }
        public string EXAM_CIRCULATION_LOGINNAME { get; set; }
        public short? EXAM_RESPIRATORY_IS_NORMAL { get; set; }
        public string EXAM_RESPIRATORY_PRE_ICD_CODE { get; set; }
        public string EXAM_RESPIRATORY_PRE_ICD_NAME { get; set; }
        public string EXAM_RESPIRATORY_ICD_CODE { get; set; }
        public string EXAM_RESPIRATORY_ICD_NAME { get; set; }
        public long? EXAM_RESPIRATORY_RANK { get; set; }
        public string EXAM_RESPIRATORY_CONCLUDE { get; set; }
        public string EXAM_RESPIRATORY_LOGINNAME { get; set; }
        public short? EXAM_DIGESTION_IS_NORMAL { get; set; }
        public string EXAM_DIGESTION_PRE_ICD_CODE { get; set; }
        public string EXAM_DIGESTION_PRE_ICD_NAME { get; set; }
        public string EXAM_DIGESTION_ICD_CODE { get; set; }
        public string EXAM_DIGESTION_ICD_NAME { get; set; }
        public long? EXAM_DIGESTION_RANK { get; set; }
        public string EXAM_DIGESTION_CONCLUDE { get; set; }
        public string EXAM_DIGESTION_LOGINNAME { get; set; }
        public short? EXAM_KIDNEY_URO_IS_NORMAL { get; set; }
        public string EXAM_KIDNEY_URO_PRE_ICD_CODE { get; set; }
        public string EXAM_KIDNEY_URO_PRE_ICD_NAME { get; set; }
        public string EXAM_KIDNEY_URO_ICD_CODE { get; set; }
        public string EXAM_KIDNEY_URO_ICD_NAME { get; set; }
        public long? EXAM_KIDNEY_URO_RANK { get; set; }
        public string EXAM_KIDNEY_URO_CONCLUDE { get; set; }
        public string EXAM_KIDNEY_URO_LOGINNAME { get; set; }
        public short? EXAM_OEND_IS_NORMAL { get; set; }
        public string EXAM_OEND_PRE_ICD_CODE { get; set; }
        public string EXAM_OEND_PRE_ICD_NAME { get; set; }
        public string EXAM_OEND_ICD_CODE { get; set; }
        public string EXAM_OEND_ICD_NAME { get; set; }
        public long? EXAM_OEND_RANK { get; set; }
        public string EXAM_OEND_CONCLUDE { get; set; }
        public string EXAM_OEND_LOGINNAME { get; set; }
        public short? EXAM_MUSCLE_BONE_IS_NORMAL { get; set; }
        public string EXAM_MUSCLE_BONE_PRE_ICD_CODE { get; set; }
        public string EXAM_MUSCLE_BONE_PRE_ICD_NAME { get; set; }
        public string EXAM_MUSCLE_BONE_ICD_CODE { get; set; }
        public string EXAM_MUSCLE_BONE_ICD_NAME { get; set; }
        public long? EXAM_MUSCLE_BONE_RANK { get; set; }
        public string EXAM_MUSCLE_BONE_CONCLUDE { get; set; }
        public string EXAM_MUSCLE_BONE_LOGINNAME { get; set; }
        public short? EXAM_NEUROLOGICAL_IS_NORMAL { get; set; }
        public string EXAM_NEUROLOGICAL_PRE_ICD_CODE { get; set; }
        public string EXAM_NEUROLOGICAL_PRE_ICD_NAME { get; set; }
        public string EXAM_NEUROLOGICAL_ICD_CODE { get; set; }
        public string EXAM_NEUROLOGICAL_ICD_NAME { get; set; }
        public long? EXAM_NEUROLOGICAL_RANK { get; set; }
        public string EXAM_NEUROLOGICAL_CONCLUDE { get; set; }
        public string EXAM_NEUROLOGICAL_LOGINNAME { get; set; }
        public short? EXAM_MENTAL_IS_NORMAL { get; set; }
        public string EXAM_MENTAL_PRE_ICD_CODE { get; set; }
        public string EXAM_MENTAL_PRE_ICD_NAME { get; set; }
        public string EXAM_MENTAL_ICD_CODE { get; set; }
        public string EXAM_MENTAL_ICD_NAME { get; set; }
        public long? EXAM_MENTAL_RANK { get; set; }
        public string EXAM_MENTAL_CONCLUDE { get; set; }
        public string EXAM_MENTAL_LOGINNAME { get; set; }
        public short? EXAM_SURGERY_IS_NORMAL { get; set; }
        public string EXAM_SURGERY_PRE_ICD_CODE { get; set; }
        public string EXAM_SURGERY_PRE_ICD_NAME { get; set; }
        public string EXAM_SURGERY_ICD_CODE { get; set; }
        public string EXAM_SURGERY_ICD_NAME { get; set; }
        public long? EXAM_SURGERY_RANK { get; set; }
        public string EXAM_SURGERY_CONCLUDE { get; set; }
        public string EXAM_SURGERY_LOGINNAME { get; set; }
        public short? EXAM_DERMATOLOGY_IS_NORMAL { get; set; }
        public string EXAM_DERMATOLOGY_PRE_ICD_CODE { get; set; }
        public string EXAM_DERMATOLOGY_PRE_ICD_NAME { get; set; }
        public string EXAM_DERMATOLOGY_ICD_CODE { get; set; }
        public string EXAM_DERMATOLOGY_ICD_NAME { get; set; }
        public long? EXAM_DERMATOLOGY_RANK { get; set; }
        public string EXAM_DERMATOLOGY_CONCLUDE { get; set; }
        public string EXAM_DERMATOLOGY_LOGINNAME { get; set; }
        public short? EXAM_OBSTETRIC_IS_NORMAL { get; set; }
        public string EXAM_OBSTETRIC_PRE_ICD_CODE { get; set; }
        public string EXAM_OBSTETRIC_PRE_ICD_NAME { get; set; }
        public string EXAM_OBSTETRIC_ICD_CODE { get; set; }
        public string EXAM_OBSTETRIC_ICD_NAME { get; set; }
        public long? EXAM_OBSTETRIC_RANK { get; set; }
        public string EXAM_OBSTETRIC_CONCLUDE { get; set; }
        public string EXAM_OBSTETRIC_LOGINNAME { get; set; }
        public short? EXAM_EYE_IS_NORMAL { get; set; }
        public string EXAM_EYE_PRE_ICD_CODE { get; set; }
        public string EXAM_EYE_PRE_ICD_NAME { get; set; }
        public string EXAM_EYE_ICD_CODE { get; set; }
        public string EXAM_EYE_ICD_NAME { get; set; }
        public long? EXAM_EYE_RANK { get; set; }
        public string EXAM_EYE_CONCLUDE { get; set; }
        public string EXAM_EYE_LOGINNAME { get; set; }
        public short? EXAM_ENT_IS_NORMAL { get; set; }
        public string EXAM_ENT_PRE_ICD_CODE { get; set; }
        public string EXAM_ENT_PRE_ICD_NAME { get; set; }
        public string EXAM_ENT_ICD_CODE { get; set; }
        public string EXAM_ENT_ICD_NAME { get; set; }
        public long? EXAM_ENT_RANK { get; set; }
        public string EXAM_ENT_CONCLUDE { get; set; }
        public string EXAM_ENT_LOGINNAME { get; set; }
        public short? EXAM_STOMATOLOGY_IS_NORMAL { get; set; }
        public string EXAM_STOMATOLOGY_PRE_ICD_CODE { get; set; }
        public string EXAM_STOMATOLOGY_PRE_ICD_NAME { get; set; }
        public string EXAM_STOMATOLOGY_ICD_CODE { get; set; }
        public string EXAM_STOMATOLOGY_ICD_NAME { get; set; }
        public long? EXAM_STOMATOLOGY_RANK { get; set; }
        public string EXAM_STOMATOLOGY_CONCLUDE { get; set; }
        public string EXAM_STOMATOLOGY_LOGINNAME { get; set; }
        public short? IS_REFUSE_OBSTETRIC { get; set; }
        public string EXAM_EYESIGHT_RIGHT { get; set; }
        public string EXAM_EYESIGHT_LEFT { get; set; }
        public string EXAM_EYESIGHT_PINHOLE_RIGHT { get; set; }
        public string EXAM_EYESIGHT_PINHOLE_LEFT { get; set; }
        public string EXAM_EYESIGHT_GLASS_RIGHT { get; set; }
        public string EXAM_EYESIGHT_GLASS_LEFT { get; set; }
        public string EXAM_EYE_SPHERE_RIGHT { get; set; }
        public string EXAM_EYE_SPHERE_LEFT { get; set; }
        public string EXAM_EYE_CYLINDER_RIGHT { get; set; }
        public string EXAM_EYE_CYLINDER_LEFT { get; set; }
        public string EXAM_EYE_AXIS_RIGHT { get; set; }
        public string EXAM_EYE_AXIS_LEFT { get; set; }
        public string EXAM_ENT_LEFT_NORMAL { get; set; }
        public string EXAM_ENT_LEFT_WHISPER { get; set; }
        public string EXAM_ENT_RIGHT_NORMAL { get; set; }
        public string EXAM_ENT_RIGHT_WHISPER { get; set; }
        public short? TOOTH_18 { get; set; }
        public short? TOOTH_17 { get; set; }
        public short? TOOTH_16 { get; set; }
        public short? TOOTH_15 { get; set; }
        public short? TOOTH_14 { get; set; }
        public short? TOOTH_13 { get; set; }
        public short? TOOTH_12 { get; set; }
        public short? TOOTH_11 { get; set; }
        public short? TOOTH_21 { get; set; }
        public short? TOOTH_22 { get; set; }
        public short? TOOTH_23 { get; set; }
        public short? TOOTH_24 { get; set; }
        public short? TOOTH_25 { get; set; }
        public short? TOOTH_26 { get; set; }
        public short? TOOTH_27 { get; set; }
        public short? TOOTH_28 { get; set; }
        public short? TOOTH_48 { get; set; }
        public short? TOOTH_47 { get; set; }
        public short? TOOTH_46 { get; set; }
        public short? TOOTH_45 { get; set; }
        public short? TOOTH_44 { get; set; }
        public short? TOOTH_43 { get; set; }
        public short? TOOTH_42 { get; set; }
        public short? TOOTH_41 { get; set; }
        public short? TOOTH_31 { get; set; }
        public short? TOOTH_32 { get; set; }
        public short? TOOTH_33 { get; set; }
        public short? TOOTH_34 { get; set; }
        public short? TOOTH_35 { get; set; }
        public short? TOOTH_36 { get; set; }
        public short? TOOTH_37 { get; set; }
        public short? TOOTH_38 { get; set; }
        public string TEST_AMPHETAMIN { get; set; }
        public string TEST_MARIJUANA { get; set; }
        public string TEST_MORPHIN { get; set; }
        public string TEST_CODEIN { get; set; }
        public string TEST_HEROIN { get; set; }
        public string TEST_ALCOHOL { get; set; }
        public string TEST_CERVICAL { get; set; }
        public string TEST_BREAST { get; set; }
        public string TEST_BLOOD_HC { get; set; }
        public string TEST_BLOOD_HGB { get; set; }
        public string TEST_BLOOD_HCT { get; set; }
        public string TEST_BLOOD_MCV { get; set; }
        public string TEST_BLOOD_MCH { get; set; }
        public string TEST_BLOOD_MCHC { get; set; }
        public string TEST_BLOOD_RDW { get; set; }
        public string TEST_BLOOD_BC { get; set; }
        public string TEST_BLOOD_NEUT { get; set; }
        public string TEST_BLOOD_LYMPH { get; set; }
        public string TEST_BLOOD_MONO { get; set; }
        public string TEST_BLOOD_EOS { get; set; }
        public string TEST_BLOOD_BASO { get; set; }
        public string TEST_BLOOD_TC { get; set; }
        public string TEST_BLOOD_GLUCO { get; set; }
        public string TEST_BLOOD_CREATININ { get; set; }
        public string TEST_BLOOD_ASAT { get; set; }
        public string TEST_BLOOD_ALAT { get; set; }
        public string TEST_URINE_SG { get; set; }
        public string TEST_URINE_PH { get; set; }
        public string TEST_URINE_LEU { get; set; }
        public string TEST_URINE_ERY { get; set; }
        public string TEST_URINE_NITRIT { get; set; }
        public string TEST_URINE_PROTEIN { get; set; }
        public string TEST_URINE_GLUCO { get; set; }
        public string TEST_URINE_KETONE { get; set; }
        public string TEST_URINE_BILIRUBIN { get; set; }
        public string TEST_URINE_UBG { get; set; }
        public string TEST_URINE_OTHER { get; set; }
        public string RESULT_DIIM { get; set; }
        public string RESULT_OTHER_TEST { get; set; }
        public string TEST_DRUG_LOGINNAME { get; set; }
        public string TEST_FEMALE_LOGINNAME { get; set; }
        public string TEST_BLOOD_LOGINNAME { get; set; }
        public string TEST_URINE_LOGINNAME { get; set; }
        public string DIIM_LOGINNAME { get; set; }
        public string OTHER_TEST_LOGINNAME { get; set; }
        public short? DRIVER_CONCLUSION_TYPE { get; set; }
        public string REEXAM_NOTE { get; set; }
        public short? CONCLUSION_IS_NORMAL { get; set; }
        public string CONCLUSION_PRE_ICD_CODE { get; set; }
        public string CONCLUSION_PRE_ICD_NAME { get; set; }
        public string CONCLUSION_ICD_CODE { get; set; }
        public string CONCLUSION_ICD_NAME { get; set; }
        public long? HEALTH_EXAM_RANK_ID { get; set; }
        public short? SUGGEST_TYPE { get; set; }
        public string SUGGEST_NOTE { get; set; }

        // ----- Cột Mẫu 5 đã đề nghị BE bổ sung (chưa có trên DB 11/11/2026): BE thêm đúng tên thì tự lưu/nạp/in -----
        public short? LICENSE_ISSUE_TYPE { get; set; }
        public string EXAM_TWO_EYESIGHT { get; set; }
        public string EXAM_TWO_EYESIGHT_GLASS { get; set; }
        public string EXAM_EYEFIELD_HORI_NORMAL { get; set; }
        public string EXAM_EYEFIELD_HORI_LIMIT { get; set; }
        public string EXAM_EYEFIELD_VERT_NORMAL { get; set; }
        public string EXAM_EYEFIELD_VERT_LIMIT { get; set; }
        public short? EXAM_EYECOLOR_IS_NORMAL { get; set; }
        public short? EXAM_EYECOLOR_IS_BLIND { get; set; }
        public short? EXAM_EYECOLOR_IS_BLIND_RED { get; set; }
        public short? EXAM_EYECOLOR_IS_BLIND_GREEN { get; set; }
        public short? EXAM_EYECOLOR_IS_BLIND_YELOW { get; set; }
        public long? CONCLUSION_TIME { get; set; }
    }
}
