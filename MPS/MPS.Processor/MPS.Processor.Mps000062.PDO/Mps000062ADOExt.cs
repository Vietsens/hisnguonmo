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
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000062.PDO
{
    public class Mps000062ADOExt
    {
        public Mps000062ADOExt() { }

        public List<Mps000062ExtADO> Mps000062ADOs { get; set; }
        public Mps000062SingleKey WorkPlaceSDO { get; set; }
        public Dictionary<string, object> SingleValueDictionary { get; set; }
    }

    public class NumberDate
    {
        public long INTRUCTION_DATE { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public long MEDICINE_GROUP_ID { get; set; }
        public int num { get; set; }
        public short Num_Order { get; set; }
        public long? MIXED_INFUSION { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
    }

    public class SingleKeyTracking
    {
        public string TRACKING_DATE_STR { get; set; }
        public string Y_LENH { get; set; }
    }


    public class Mps000062ADO : V_HIS_TRACKING
    {
        public string TRACKING_TIME_STR { get; set; }

        public string TRACKING_DATE_STR { get; set; }

        public string TRACKING_DATE_SEPARATE_STR { get; set; }

        public long? REMEDY_COUNT { get; set; }

        public string ICD_NAME_TRACKING { get; set; }

        public string IS_SHOW_MEDICINE { get; set; }

        public long? SUM_MEDI_MATE { get; set; }
        public string IS_T7_OR_CN { get; set; }
        public decimal? BELLY { get; set; }
        public long? BLOOD_PRESSURE_MAX { get; set; }
        public long? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public decimal? CHEST { get; set; }
        public decimal? HEIGHT { get; set; }
        public long? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? VIR_BODY_SURFACE_AREA { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? SPO2 { get; set; }
        public decimal? CAPILLARY_BLOOD_GLUCOSE { get; set; }
        public string TRACKING_USERNAME { get; set; }
        public string NOTE { get; set; }

        public string PART_EXAM_EYE_TENSION_LEFT { get; set; }
        public string PART_EXAM_EYE_TENSION_RIGHT { get; set; }
        public string PART_EXAM_EYESIGHT_LEFT { get; set; }
        public string PART_EXAM_EYESIGHT_RIGHT { get; set; }
        public string PART_EXAM_EYESIGHT_GLASS_LEFT { get; set; }
        public string PART_EXAM_EYESIGHT_GLASS_RIGHT { get; set; }

        public string ICD_TEXT_TRACKING { get; set; }

        public long NUMBER_DAYS_TREATMENT { get; set; }
    }

    public class Mps000062ExtADO : V_HIS_TRACKING
    {
        public Mps000062ExtADO() { }
        public Mps000062ExtADO(Mps000062ADO mps000062ADO)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<Mps000062ExtADO>(this, mps000062ADO);
            this.TRACKING_TIME_STR = mps000062ADO.TRACKING_TIME_STR;
            this.IS_T7_OR_CN = mps000062ADO.IS_T7_OR_CN;
            this.TRACKING_DATE_STR = mps000062ADO.TRACKING_DATE_STR;
            this.TRACKING_DATE_SEPARATE_STR = mps000062ADO.TRACKING_DATE_SEPARATE_STR;
            this.REMEDY_COUNT = mps000062ADO.REMEDY_COUNT;
            this.ICD_NAME_TRACKING = mps000062ADO.ICD_NAME_TRACKING;
            this.IS_SHOW_MEDICINE = mps000062ADO.IS_SHOW_MEDICINE;
            this.SUM_MEDI_MATE = mps000062ADO.SUM_MEDI_MATE;
            this.BELLY = mps000062ADO.BELLY;
            this.BLOOD_PRESSURE_MAX = mps000062ADO.BLOOD_PRESSURE_MAX;
            this.BLOOD_PRESSURE_MIN = mps000062ADO.BLOOD_PRESSURE_MIN;
            this.BREATH_RATE = mps000062ADO.BREATH_RATE;
            this.CHEST = mps000062ADO.CHEST;
            this.HEIGHT = mps000062ADO.HEIGHT;
            this.PULSE = mps000062ADO.PULSE;
            this.TEMPERATURE = mps000062ADO.TEMPERATURE;
            this.NOTE = mps000062ADO.NOTE;

            this.VIR_BMI = mps000062ADO.VIR_BMI;
            this.VIR_BODY_SURFACE_AREA = mps000062ADO.VIR_BODY_SURFACE_AREA;
            this.WEIGHT = mps000062ADO.WEIGHT;
            this.SPO2 = mps000062ADO.SPO2;
            this.CAPILLARY_BLOOD_GLUCOSE = mps000062ADO.CAPILLARY_BLOOD_GLUCOSE;

            this.AWARENESS_BEHAVIOR = mps000062ADO.AWARENESS_BEHAVIOR;
            this.CARDIOVASCULAR = mps000062ADO.CARDIOVASCULAR;
            this.CARE_INSTRUCTION = mps000062ADO.CARE_INSTRUCTION;
            this.CONCENTRATION = mps000062ADO.CONCENTRATION;
            this.CONTENT = mps000062ADO.CONTENT;
            this.CONTENT_OF_THINKING = mps000062ADO.CONTENT_OF_THINKING;
            this.CREATE_TIME = mps000062ADO.CREATE_TIME;
            this.CREATOR = mps000062ADO.CREATOR;
            this.DEPARTMENT_ID = mps000062ADO.DEPARTMENT_ID;
            this.EMOTION = mps000062ADO.EMOTION;
            this.FORM_OF_THINKING = mps000062ADO.FORM_OF_THINKING;
            this.GENERAL_EXPRESSION = mps000062ADO.GENERAL_EXPRESSION;
            this.ICD_CODE = mps000062ADO.ICD_CODE;
            this.ICD_NAME = mps000062ADO.ICD_NAME;
            this.ICD_SUB_CODE = mps000062ADO.ICD_SUB_CODE;
            this.ICD_TEXT = mps000062ADO.ICD_TEXT;
            this.ID = mps000062ADO.ID;
            this.INSTINCTIVELY_BEHAVIOR = mps000062ADO.INSTINCTIVELY_BEHAVIOR;
            this.INTELLECTUAL = mps000062ADO.INTELLECTUAL;
            this.IS_ACTIVE = mps000062ADO.IS_ACTIVE;
            this.IS_DELETE = mps000062ADO.IS_DELETE;
            this.MEDICAL_INSTRUCTION = mps000062ADO.MEDICAL_INSTRUCTION;
            this.MEMORY = mps000062ADO.MEMORY;
            this.MODIFIER = mps000062ADO.MODIFIER;
            this.MODIFY_TIME = mps000062ADO.MODIFY_TIME;
            this.ORIENTATION_CAPACITY = mps000062ADO.ORIENTATION_CAPACITY;
            this.PERCEPTION = mps000062ADO.PERCEPTION;
            this.RESPIRATORY = mps000062ADO.RESPIRATORY;
            this.SUBCLINICAL_PROCESSES = mps000062ADO.SUBCLINICAL_PROCESSES;
            this.TRACKING_TIME = mps000062ADO.TRACKING_TIME;
            this.TREATMENT_ID = mps000062ADO.TREATMENT_ID;

            this.PART_EXAM_EYE_TENSION_LEFT = mps000062ADO.PART_EXAM_EYE_TENSION_LEFT;
            this.PART_EXAM_EYE_TENSION_RIGHT = mps000062ADO.PART_EXAM_EYE_TENSION_RIGHT;
            this.PART_EXAM_EYESIGHT_LEFT = mps000062ADO.PART_EXAM_EYESIGHT_LEFT;
            this.PART_EXAM_EYESIGHT_RIGHT = mps000062ADO.PART_EXAM_EYESIGHT_RIGHT;
            this.PART_EXAM_EYESIGHT_GLASS_LEFT = mps000062ADO.PART_EXAM_EYESIGHT_GLASS_LEFT;
            this.PART_EXAM_EYESIGHT_GLASS_RIGHT = mps000062ADO.PART_EXAM_EYESIGHT_GLASS_RIGHT;

            this.ICD_TEXT_TRACKING = mps000062ADO.ICD_TEXT_TRACKING;
            this.NUMBER_DAYS_TREATMENT = mps000062ADO.NUMBER_DAYS_TREATMENT;
        }
        public string IS_T7_OR_CN { get; set; }
        public string TRACKING_TIME_STR { get; set; }
        public string TRACKING_DATE_STR { get; set; }
        public string TRACKING_DATE_SEPARATE_STR { get; set; }
        public string PRIVIOUS_TRACKING_DATE_STR { get; set; }
        public string NEXT_TRACKING_DATE_STR { get; set; }
        public long? REMEDY_COUNT { get; set; }
        public string ICD_NAME_TRACKING { get; set; }
        public string IS_SHOW_MEDICINE { get; set; }
        public long? SUM_MEDI_MATE { get; set; }
        public decimal? BELLY { get; set; }
        public long? BLOOD_PRESSURE_MAX { get; set; }
        public long? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public decimal? CHEST { get; set; }
        public decimal? HEIGHT { get; set; }
        public long? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? VIR_BODY_SURFACE_AREA { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? SPO2 { get; set; }
        public decimal? CAPILLARY_BLOOD_GLUCOSE { get; set; }
        public string NOTE { get; set; }

        public string ICD_TEXT_TRACKING { get; set; }

        //New
        public string MATERIAL___DATA { get; set; }
        public string MATERIAL_HTU___DATA { get; set; }
        public string MATERIAL_DuTru___DATA { get; set; } // vật tư dự trù
        public string MATERIAL_DuTru_HTU___DATA { get; set; }
        public string MATERIAL_THDT___DATA { get; set; } // vật tư thực hiện dự trù
        public string MATERIAL_THDT_HTU___DATA { get; set; } // vật tư thực hiện dự trù
        /// <summary>
        /// Liều 1 thang x " + DAY_COUNT+ TUTORIAL_REMEDY
        /// </summary>
        public string REMEDY_COUNT___DATA { get; set; }
        /// <summary>
        /// Đơn {0} thang: {1}
        /// </summary>
        public string REMEDY_COUNT___DATA1 { get; set; }
        public string MEDICINE_LINE___DATA { get; set; }
        /// <summary>
        /// Dữ liệu hiển thị dạng:
        /// '- <#Medicines.NUMBER_H_N;><#if(<#Medicines.REMEDY_COUNT;> = 0;; <#Medicines.MEDICINE_TYPE_NAME;>)> <#if(<#Medicines.REMEDY_COUNT;> >0;; <#Medicines.MEDICINE_TYPE_NAME;>)> <#if(<#Medicines.MEDICINE_line_id;> =2;;<#Medicines.CONCENTRA;>)> <#if(<#Medicines.REMEDY_COUNT;> = 0;<#if(<#Medicines.AMOUNT;> = 0;;)>;)>
        ///<#if(<#Medicines.REMEDY_COUNT;> >0;; <#Medicines.TUTORIAL;>)> <#Row Height(Autofit)>
        ///<#if(<#Medicines.REMEDY_COUNT;> = 0;;<#Medicines.Amount_By_Remedy_Count;><#Medicines.SERVICE_UNIT_NAME;> )> <#if(<#Medicines.REMEDY_COUNT;> = 0;<#if(<#Medicines.AMOUNT;> = 0;;   <#Medicines.AMOUNT;>  <#Medicines.SERVICE_UNIT_NAME;>)>;)> <#Row Height(Autofit)>
        /// </summary>
        public string MEDICINES___DATA { get; set; }

        public string MEDICINES_HTU___DATA { get; set; }
        /// <summary>
        /// <#Medicines.NUMBER_H_N;><#Medicines.MEDICINE_TYPE_NAME;> <#Medicines.CONCENTRA;> <#if(<#Medicines.REMEDY_COUNT;> = 0;<#if(<#Medicines.AMOUNT;> = 0;;   x<#Medicines.AMOUNT;>  <#Medicines.SERVICE_UNIT_NAME;>   <#Medicines.MEDICINE_USE_FORM_NAME;> )>;<#Medicines.Amount_By_Remedy_Count;> <#Medicines.SERVICE_UNIT_NAME;>)><#if(<#Medicines.PRESCRIPTION_TYPE_ID;> =2;;
        ///<#Medicines.TUTORIAL;>)><#Row Height(Autofit)>
        /// </summary>
        public string MEDICINES___DATA1 { get; set; }
        /// <summary>
        /// <#Medicines.NUMBER_H_N;>--<#Medicines.MEDICINE_TYPE_NAME;> <#if(<#Medicines.REMEDY_COUNT;> = 0;<#if(<#Medicines.AMOUNT;> = 0;;   x<#Medicines.AMOUNT;>  <#Medicines.SERVICE_UNIT_NAME;>   <#Medicines.MEDICINE_USE_FORM_NAME;> )>;<#Medicines.Amount_By_Remedy_Count;> (<#Medicines.REMEDY_COUNT;> thang)  <#Medicines.SERVICE_UNIT_NAME;>)>
        ///<#Medicines.TUTORIAL;><#Row Height(Autofit)>
        /// </summary>
        public string MEDICINES___DATA2 { get; set; }

        public string MEDICINES___DATA3 { get; set; }

        public string MEDICINES_TAY___DATA { get; set; } //thuốc tây y
        public string MEDICINES_DONG___DATA { get; set; } //thuốc đông y
        public string MEDICINES_DONG_DETAIL___DATA { get; set; } //Chi tiết thuốc đông y

        public string MEDICINES_DONG_HTU___DATA { get; set; } //Chi tiết thuốc đông y
        public string MEDICINES_NO_CONCENTRA__DATA { get; set; } //Bỏ hàm lượng khỏi tên thuốc 
        public string MEDI_DUTRU_NO_CONCENTRA { get; set; } //Bỏ hàm lượng khỏi tên thuốc đối với thuốc dự trù
        public string MEDI_THDT_NO_CONCENTRA { get; set; } //Bỏ hàm lượng khỏi tên thuốc đối với thuốc thực hiện dự trù

        public string IMP_MEST_MEDICINE___DATA { get; set; }
        public string IMP_MEST_MATERIAL___DATA { get; set; }
        public string TT_SERVICE___DATA { get; set; }
        public string SERVICE_CLS___DATA { get; set; }
        public string SERVICE_CLS_X01___DATA { get; set; }
        public string SERVICE_CLS_BOLD___DATA { get; set; }
        public string SERVICE_CLS_BOLD_X01___DATA { get; set; }
        /// <summary>
        /// 6. Thuốc Hoàn trả
        ///- Thêm tiêu đề “Thuốc hoàn trả”
        ///- Số lượng đưa sang phải gồm: Số lượng + ĐVT
        /// </summary>
        public string MOBA_IMP_MEST_MEDICINE__DATA { get; set; }
        public string MOBA_IMP_MEST_MATERIAL__DATA { get; set; }
        public string MOBA_IMP_MEST_BLOOD__DATA { get; set; }
        /// <summary>
        /// hiển thị thuốc và HDSD của các thuốc kê trước đó nhưng có ngày dùng chứa ngày của tờ điều trị
        /// </summary>
        public string PRE_MEDICINE { get; set; }
        public string TRACKING_USERNAME { get; set; }
        public string SERVICE_REQ_METY___DATA { get; set; }
        public string SERVICE_REQ_MATY___DATA { get; set; }
        public string CARE___DATA { get; set; }
        public string CARE_DETAIL___DATA { get; set; }
        public string BLOOD___DATA { get; set; }
        public string MEDICAL_INSTRUCTION___DATA { get; set; }
        public string MEDICINES_INFUSION___DATA { get; set; }
        public string MEDICINES_INFUSION_HTU___DATA { get; set; }
        public string MEDICINES_INFUSION_DATA_WITH_BOLD_NAME { get; set; } // thuốc pha truyền có in đậm tên thuốc
        public string MEDICINES_DuTru___DATA { get; set; } // hiển thị các thuốc dự trù
        public string MEDICINES_DuTru_HTU___DATA { get; set; }
        public string MEDICINES_THDT___DATA { get; set; } // hiển thị các thuốc thực hiện dự trù
        public string MEDICINES_THDT_HTU___DATA { get; set; }
        public string MEDICINES_INFUSION_DuTru___DATA { get; set; } // thuốc pha truyền dự trù
        public string MEDICINES_INFUSION_DuTru_HTU___DATA { get; set; }
        public string MEDICINES_INFUSION_THDT___DATA { get; set; } // thuốc pha truyền thực hiện dự trù
        public string MEDICINES_INFUSION_THDT_HTU___DATA { get; set; }
        public string MEDICINES_OutStock_DuTru__DATA { get; set; } // thuốc mua ngoài dự trù
        public string MEDICINES_OutStock_DuTru_HTU__DATA { get; set; }
        public string MEDICINES_OutStock_THDT__DATA { get; set; } // thuốc mua ngoài thực hiện dự trù
        public string MEDICINES_OutStock_THDT_HTU__DATA { get; set; }

        public string MEDICINES_MERGE___DATA { get; set; } // thuốc pha truyền và thuốc không pha truyền
        public string MEDICINES_MERGE_HTU___DATA { get; set; } 
        public string MEDICINES_MERGE_DUTRU___DATA { get; set; } // thuốc dự trù pha truyền và thuốc dự trù không pha truyền
        public string MEDICINES_MERGE_DUTRU_HTU___DATA { get; set; }
        /// <summary>
        /// Tổng hợp chi tiết của MEDICINES___DATA, MEDICINES_DuTru___DATA,  MEDICINES_THDT___DATA
        /// </summary>
        public string MEDICINES_MERGE_DETAIL___DATA { get; set; }
        public string MEDICINES_MERGE_DETAIL_HTU___DATA { get; set; }
        /// <summary>
        /// Key tạo dữ liệu gồm dịch vụ CLS và dịch vụ TT các trường hợp không dự trù, dự trù, thực hiện dự trù
        /// </summary>
        public string SERVICE_MERGE_X01___DATA { get; set; }
        public string SERVICE_CLS_DuTru___DATA { get; set; }
        public string MEDICINES_MERGE_DAY___DATA { get; set; }
        public string MEDICINES_MERGE_DAY_HTU___DATA { get; set; }
        public string SERVICE_CLS_DuTru_X01___DATA { get; set; }
        public string SERVICE_CLS_THDT___DATA { get; set; }
        public string SERVICE_CLS_THDT_X01___DATA { get; set; }
        public string TT_SERVICE_DuTru___DATA { get; set; } 
        public string TT_SERVICE_THDT___DATA { get; set; } 
        public string SERVICE_REQ_ADVISE { get; set; }
        public string PARENT_ORGANIZATION_NAME { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string DEPARTMENT_NAME { get; set; }

        public string AGE { get; set; }
        public string AGE_TRACKING { get; set; }
        public string PHONE { get; set; }
        public string ROOM_NAME { get; set; }
        public string BED_NAME { get; set; }
        public string ICD_NAME_FULL { get; set; }
        public string ICD_CODE_BY_TRACKING { get; set; }
        public string ICD_NAME_BY_TRACKING { get; set; }
        public string ICD_SUB_CODE_BY_TRACKING { get; set; }
        public string ICD_TEXT_BY_TRACKING { get; set; }
        public string EXECUTE_TIME_DHST { get; set; }
        public string NGAY_1 { get; set; }
        public string NGAY_2 { get; set; }
        public string NGAY_3 { get; set; }
        public string NGAY_4 { get; set; }
        public string Y_LENH_1 { get; set; }
        public string Y_LENH_2 { get; set; }
        public string Y_LENH_3 { get; set; }
        public string Y_LENH_4 { get; set; }

        public string ADVISE { get; set; }
        public string APPOINTMENT_CODE { get; set; }
        public string APPOINTMENT_DESC { get; set; }
        public string APPOINTMENT_EXAM_ROOM_IDS { get; set; }
        public string APPOINTMENT_SURGERY { get; set; }
        public long? APPOINTMENT_TIME { get; set; }
        public decimal? AUTO_DISCOUNT_RATIO { get; set; }
        public long BRANCH_ID { get; set; }
        public long? CLINICAL_IN_TIME { get; set; }
        public string CLINICAL_NOTE { get; set; }
        public string CO_DEPARTMENT_IDS { get; set; }
        public string COLLINEAR_XML4210_DESC { get; set; }
        public long? COLLINEAR_XML4210_RESULT { get; set; }
        public string COLLINEAR_XML4210_URL { get; set; }
        public long? DATA_STORE_ID { get; set; }
        public long? DEATH_CAUSE_ID { get; set; }
        public long? DEATH_DOCUMENT_DATE { get; set; }
        public string DEATH_DOCUMENT_NUMBER { get; set; }
        public string DEATH_DOCUMENT_PLACE { get; set; }
        public string DEATH_DOCUMENT_TYPE { get; set; }
        public string DEATH_PLACE { get; set; }
        public long? DEATH_TIME { get; set; }
        public long? DEATH_WITHIN_ID { get; set; }
        public string DEPARTMENT_IDS { get; set; }
        public string DOCTOR_LOGINNAME { get; set; }
        public string DOCTOR_USERNAME { get; set; }
        public long? EMERGENCY_WTIME_ID { get; set; }
        public string END_CODE { get; set; }
        public long? END_DEPARTMENT_ID { get; set; }
        public string END_LOGINNAME { get; set; }
        public long? END_ROOM_ID { get; set; }
        public string END_USERNAME { get; set; }
        public string EXTRA_END_CODE { get; set; }
        public long? FEE_LOCK_DEPARTMENT_ID { get; set; }
        public long? FEE_LOCK_ORDER { get; set; }
        public long? FEE_LOCK_ROOM_ID { get; set; }
        public long? FEE_LOCK_TIME { get; set; }
        public decimal? FUND_BUDGET { get; set; }
        public string FUND_COMPANY_NAME { get; set; }
        public string FUND_CUSTOMER_NAME { get; set; }
        public long? FUND_FROM_TIME { get; set; }
        public long? FUND_ID { get; set; }
        public long? FUND_ISSUE_TIME { get; set; }
        public string FUND_NUMBER { get; set; }
        public long? FUND_PAY_TIME { get; set; }
        public long? FUND_SEND_FILE_TIME { get; set; }
        public long? FUND_TO_TIME { get; set; }
        public string FUND_TYPE_NAME { get; set; }
        public string HOSPITALIZATION_REASON { get; set; }
        public string HRM_KSK_CODE { get; set; }
        public string ICD_CAUSE_CODE { get; set; }
        public string ICD_CAUSE_NAME { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string IN_CODE { get; set; }
        public long IN_DATE { get; set; }
        public long? IN_DEPARTMENT_ID { get; set; }
        public string IN_ICD_CODE { get; set; }
        public string IN_ICD_NAME { get; set; }
        public string IN_ICD_SUB_CODE { get; set; }
        public string IN_ICD_TEXT { get; set; }
        public string IN_LOGINNAME { get; set; }
        public long? IN_ROOM_ID { get; set; }
        public long IN_TIME { get; set; }
        public long? IN_TREATMENT_TYPE_ID { get; set; }
        public string IN_USERNAME { get; set; }
        public short? IS_AUTO_DISCOUNT { get; set; }
        public short? IS_CHRONIC { get; set; }
        public short? IS_EMERGENCY { get; set; }
        public short? IS_END_CODE_REQUEST { get; set; }
        public short? IS_HAS_AUPOPSY { get; set; }
        public short? IS_HOLD_BHYT_CARD { get; set; }
        public short? IS_IN_CODE_REQUEST { get; set; }
        public short? IS_INTEGRATE_HIS_SENT { get; set; }
        public short? IS_LOCK_FEE { get; set; }
        public short? IS_LOCK_HEIN { get; set; }
        public short? IS_NOT_CHECK_LHMP { get; set; }
        public short? IS_NOT_CHECK_LHSP { get; set; }
        public short? IS_OUT_CODE_REQUEST { get; set; }
        public short? IS_PAUSE { get; set; }
        public short? IS_REMOTE { get; set; }
        public short? IS_SYNC_EMR { get; set; }
        public short? IS_TEMPORARY_LOCK { get; set; }
        public short? IS_TRANSFER_IN { get; set; }
        public short? IS_YDT_UPLOAD { get; set; }
        public string JSON_FORM_ID { get; set; }
        public string JSON_PRINT_ID { get; set; }
        public long? KSK_ORDER { get; set; }
        public long? LAST_DEPARTMENT_ID { get; set; }
        public string MAIN_CAUSE { get; set; }
        public string MEDI_ORG_CODE { get; set; }
        public string MEDI_ORG_NAME { get; set; }
        public long? MEDI_RECORD_ID { get; set; }
        public long? MEDI_RECORD_TYPE_ID { get; set; }
        public short? NEED_SICK_LEAVE_CERT { get; set; }
        public string OUT_CODE { get; set; }
        public long? OUT_DATE { get; set; }
        public long? OUT_TIME { get; set; }
        public long? OWE_MODIFY_TIME { get; set; }
        public long? OWE_TYPE_ID { get; set; }
        public string PATIENT_CONDITION { get; set; }
        public long PATIENT_ID { get; set; }
        public long? PROGRAM_ID { get; set; }
        public string PROVISIONAL_DIAGNOSIS { get; set; }
        public string SICK_HEIN_CARD_NUMBER { get; set; }
        public decimal? SICK_LEAVE_DAY { get; set; }
        public long? SICK_LEAVE_FROM { get; set; }
        public long? SICK_LEAVE_TO { get; set; }
        public string STORE_CODE { get; set; }
        public long? STORE_TIME { get; set; }
        public string SUBCLINICAL_RESULT { get; set; }
        public string SURGERY { get; set; }
        public long? SURGERY_APPOINTMENT_TIME { get; set; }
        public long? TDL_FIRST_EXAM_ROOM_ID { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string TDL_HEIN_MEDI_ORG_CODE { get; set; }
        public string TDL_HEIN_MEDI_ORG_NAME { get; set; }
        public long? TDL_KSK_CONTRACT_ID { get; set; }
        public string TDL_PATIENT_ACCOUNT_NUMBER { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_AVATAR_URL { get; set; }
        public string TDL_PATIENT_CAREER_NAME { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public long TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public string TDL_PATIENT_MILITARY_RANK_NAME { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_RELATIVE_NAME { get; set; }
        public string TDL_PATIENT_RELATIVE_TYPE { get; set; }
        public string TDL_PATIENT_TAX_CODE { get; set; }
        public long? TDL_PATIENT_TYPE_ID { get; set; }
        public string TDL_PATIENT_WORK_PLACE { get; set; }
        public string TDL_PATIENT_WORK_PLACE_NAME { get; set; }
        public long? TDL_TREATMENT_TYPE_ID { get; set; }
        public long? TRAN_PATI_FORM_ID { get; set; }
        public long? TRAN_PATI_REASON_ID { get; set; }
        public long? TRAN_PATI_TECH_ID { get; set; }
        public long? TRANSFER_IN_CMKT { get; set; }
        public string TRANSFER_IN_CODE { get; set; }
        public long? TRANSFER_IN_FORM_ID { get; set; }
        public string TRANSFER_IN_ICD_CODE { get; set; }
        public long? TRANSFER_IN_ICD_ID__DELETE { get; set; }
        public string TRANSFER_IN_ICD_NAME { get; set; }
        public string TRANSFER_IN_MEDI_ORG_CODE { get; set; }
        public string TRANSFER_IN_MEDI_ORG_NAME { get; set; }
        public long? TRANSFER_IN_REASON_ID { get; set; }
        public long? TRANSFER_IN_TIME_FROM { get; set; }
        public long? TRANSFER_IN_TIME_TO { get; set; }
        public string TRANSPORT_VEHICLE { get; set; }
        public string TRANSPORTER { get; set; }
        public string TREATMENT_CODE { get; set; }
        public decimal? TREATMENT_DAY_COUNT { get; set; }
        public string TREATMENT_DIRECTION { get; set; }
        public long? TREATMENT_END_TYPE_EXT_ID { get; set; }
        public long? TREATMENT_END_TYPE_ID { get; set; }
        public string TREATMENT_METHOD { get; set; }
        public long? TREATMENT_ORDER { get; set; }
        public long? TREATMENT_RESULT_ID { get; set; }
        public long? TREATMENT_STT_ID { get; set; }
        public string USED_MEDICINE { get; set; }
        public string XML4210_DESC { get; set; }
        public long? XML4210_RESULT { get; set; }
        public string XML4210_URL { get; set; }

        public string PART_EXAM_EYE_TENSION_LEFT { get; set; }
        public string PART_EXAM_EYE_TENSION_RIGHT { get; set; }
        public string PART_EXAM_EYESIGHT_LEFT { get; set; }
        public string PART_EXAM_EYESIGHT_RIGHT { get; set; }
        public string PART_EXAM_EYESIGHT_GLASS_LEFT { get; set; }
        public string PART_EXAM_EYESIGHT_GLASS_RIGHT { get; set; }

        public string ADD_LOGINNAME { get; set; }
        public long ADD_TIME { get; set; }
        public string ADD_USERNAME { get; set; }
        public string BED_CODE { get; set; }
        public long? BED_ID { get; set; }
        public string BED_ROOM_CODE { get; set; }
        public long BED_ROOM_ID { get; set; }
        public string BED_ROOM_NAME { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }

        public string ORDER_SHEET { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long NUMBER_DAYS_TREATMENT { get; set; }
        public long? NUM_ORDER_MEDICINE_LINE { get; set; }
        public long? NUM_ORDER_MEDICINE_USE_FORM { get; set; }
        public long? NUM_ORDER_MEDICINE_GROUP { get; set; }
        public long? NUM_ORDER_DOSAGE_FORM { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string DOSAGE_FORM_NAME { get; set; }

        public List<ExpMestMetyReqADO> ExpMestMetyReqADOs { get; set; }// thuốc thường
        public List<ExpMestMetyReqADO> MedicinesInfusion { get; set; } //thuốc pha truyền
        public List<ExpMestMetyReqADO> MedicinesDuTru { get; set; } //thuốc dự trù
        public List<ExpMestMetyReqADO> MedicinesTHDT { get; set; } //thuốc thực hiện dự trù
        public List<ExpMestMetyReqADO> MediInfusionDutru { get; set; } //thuốc pha truyền dự trù
        public List<ExpMestMetyReqADO> MediInfusionTHDT { get; set; } //thuốc pha truyền thực hiện dự trù
        public List<HIS_SERVICE_REQ> ServiceReq { get; set; } //y lệnh
        public List<HIS_SERVICE_REQ> ServiceReqDuTru { get; set; } //y lệnh dự trù
        public List<HIS_SERVICE_REQ> ServiceReqTHDT { get; set; } //y lệnh thực hiện dự trù
        public List<ServiceCLS> ServiceCLSOrder { get; set; }
        public List<ServiceCLS> ServiceCLSSplitXN { get; set; }
        public List<ServiceCLS> ServiceCLSs { get; set; }
        public List<ServiceCLS> Bloods { get; set; }
        public List<ServiceCLS> ExamServices { get; set; }
        public List<ServiceCLS> TTServices { get; set; }
        public List<RemedyCountADO> RemedyCountADOs { get; set; }
        public List<ExpMestMatyReqADO> ExpMestMatyReqADOs { get; set; } //vật tư
        public List<ExpMestMatyReqADO> MaterialsDuTru { get; set; } //vật tư dự trù
        public List<ExpMestMatyReqADO> MaterialsTHDT { get; set; } //vật tư thực hiện dự trù
        public List<ServiceReqMetyADO> ServiceReqMetyADOs { get; set; }
        public List<ServiceReqMatyADO> ServiceReqMatyADOs { get; set; }
        public List<ImpMestMedicineADO> ImpMestMedicineADOs { get; set; }
        public List<ImpMestMaterialADO> ImpMestMaterialADOs { get; set; }
        public List<MedicalInstruction> MedicalInstructions { get; set; }
        public List<HIS_CARE> Cares { get; set; } //chăm sóc
        public List<V_HIS_CARE_DETAIL> CareDetails { get; set; }
        public List<MedicineLineADO> MedicineLineADOs { get; set; }
        public List<ImpMestBloodADO> ImpMestBlood { get; set; }
        public List<SereServRationADO> Ration { get; set; } // suất ăn
    }

    public class MedicineLineADO
    {
        public long? ID { get; set; }
        public String MEDICINE_LINE_NAME { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? TRACKING_ID { get; set; }
    }

    public class RemedyCountADO
    {
        public long TRACKING_ID { get; set; }
        public long EXP_MEST_ID { get; set; } //exp_mest_id
        public long? REMEDY_COUNT { get; set; }
        public short PRESCRIPTION_TYPE_ID { get; set; }
        public string TUTORIAL_REMEDY { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }
        public long DAY_COUNT { get; set; }
    }

    public class ExpMestMetyReqADO : HIS_EXP_MEST_MEDICINE
    {
        public long TRACKING_ID { get; set; }
        public long? REMEDY_COUNT { get; set; }
        public decimal? Amount_By_Remedy_Count { get; set; }
        public long? NUMBER_H_N { get; set; }
        public long? NUMBER_BY_TYPE { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long? MEDICINE_USE_FORM_ID { get; set; }
        public long NUM_ORDER_BY_USE_FORM { get; set; }
        public string TUTORIAL_REMEDY { get; set; }
        public string CONCENTRA { get; set; }
        public string GROUP_BHYT { get; set; }
        public string AMOUNT_STR { get; set; }
        public short PRESCRIPTION_TYPE_ID { get; set; }
        public long? MEDICINE_LINE_ID { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_LINE_CODE { get; set; }
        public long? MEDICINE_LINE_NUM_ORDER { get; set; }

        public string DESCRIPTION_INTRUCTION_DAYS { get; set; }
        public long? INTRUCTION_DAYS { get; set; }
        public long INTRUCTION_DATE { get; set; }
        public long INTRUCTION_TIME { get; set; }

        public long TYPE_ID { get; set; }
        public string NUMBER_INTRUCTION_DATE { get; set; }

        public long Num_Order_by_group { get; set; }

        public long? MEDICINE_GROUP_NUM_ORDER { get; set; }

        public long INTRUCTION_TIME_STR { get; set; }

        public long? USING_COUNT_NUMBER { get; set; } //số lần sử dụng

        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public long? NUMBER_USE_AND_ACTIVE { get; set; } //số lần sử dụng theo hoạt chất, đường dùng

        public long? NUMBER_ACTIVE_INGR { get; set; } //số lần sử dụng theo hoạt chất và không phân biệt đường dùng (cùng 1 ngày 2 thuốc khác nhau nhưng có hoạt chất giống nhau thì đánh cùng số ngày sử dụng thuốc)
        public long? NUMBER_USE_DAY { get; set; } //số ngày sử dụng

        public short? IS_NUMBERED_TRACKING { get; set; }

        public short? IS_NOT_TREATMENT_DAY_COUNT { get; set; }

        public long? MEDICINE_GROUP_ID { get; set; }

        public string TUTORIAL_WITH_ENTER { get; set; } //Hưỡng dẫn sử dụng co enter

        public string NUMBER_OF_USE_IN_DAY { get; set; } // Số lần sử dụng trong ngày

        public long? NUMBER_BY_GROUP { get; set; } //Đánh số cả thuốc trong kho và ngoài kho theo checkbox đáng số thứ tự

        public decimal AMOUNTDuTru { get; set; } //Số lượng dự trù
        public decimal AMOUNTTHDT { get; set; } //Số lượng thực hiện dự trù

        public long? NUMBER_BY_TYPE_IN_OUT { get; set; } // Đánh số theo loại thuốc đối với thuốc trong kho và ngoài kho

        public long? USED_FOR_TRACKING_ID { get; set; }

        public long? USE_TIME { get; set; }
        public long SERVICE_REQ_TYPE_ID { get; set; }

        public long? USE_TIME_AND_INTRUCTION_TIME { get; set; }
        public string TH_AMOUNT_STR { get; set; }
        public string AMOUNT_TH { get; set; }
        public string AMOUNT_TEXT { get; set; }

        public decimal? CONVERT_RATIO { get; set; }
        public string CONVERT_UNIT_CODE { get; set; }
        public string CONVERT_UNIT_NAME { get; set; }
        public decimal? CONVERT_AMOUNT { get; set; }

        public long? NUM_ORDER_MEDICINE_LINE { get; set; }
        public long? NUM_ORDER_MEDICINE_USE_FORM { get; set; }
        public long? NUM_ORDER_MEDICINE_GROUP { get; set; }
        public long? NUM_ORDER_DOSAGE_FORM { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string DOSAGE_FORM_NAME { get; set; }
        public string DATA_REPX { get; set; }
        public string DATA_DAY_REPX { get; set; }
        public string DATA_DAY_HTU_REPX { get; set; }
        public long? ASSIGN_TIME_TO { get; set; }
        public string ADVISE { get; set; }
        public ExpMestMetyReqADO() { }
        public ExpMestMetyReqADO(HIS_EXP_MEST_MEDICINE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMetyReqADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExpMestMatyReqADO : HIS_EXP_MEST_MATERIAL
    {
        public long TRACKING_ID { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public string AMOUNT_STR { get; set; }
        public long INTRUCTION_TIME { get; set; }
        public long INTRUCTION_TIME_STR { get; set; }
        public long? USED_FOR_TRACKING_ID { get; set; }
        public long? USE_TIME { get; set; }

        public decimal? CONVERT_RATIO { get; set; }
        public string CONVERT_UNIT_CODE { get; set; }
        public string CONVERT_UNIT_NAME { get; set; }
        public decimal? CONVERT_AMOUNT { get; set; }

        public ExpMestMatyReqADO() { }
        public ExpMestMatyReqADO(HIS_EXP_MEST_MATERIAL data)
        {
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMatyReqADO>(this, data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ServiceReqMetyADO : HIS_SERVICE_REQ_METY
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long INTRUCTION_TIME_STR { get; set; }
        public long? NUMBER_BY_GROUP { get; set; } //Đánh số cả thuốc trong kho và ngoài kho theo checkbox đáng số thứ tự
        public long? NUMBER_BY_TYPE_IN_OUT { get; set; } // Đánh số theo loại thuốc đối với thuốc trong kho và ngoài kho
        public long? USE_TIME { get; set; }
        public long? USED_FOR_TRACKING_ID { get; set; }
        public long? NUM_ORDER_MEDICINE_LINE { get; set; }
        public long? NUM_ORDER_MEDICINE_USE_FORM { get; set; }
        public long? NUM_ORDER_MEDICINE_GROUP { get; set; }
        public long? NUM_ORDER_DOSAGE_FORM { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string DOSAGE_FORM_NAME { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string CONCENTRA { get; set; }
        public ServiceReqMetyADO() { }
        public ServiceReqMetyADO(HIS_SERVICE_REQ_METY data, long trackingId, List<HIS_SERVICE_REQ> ServiceReqs)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqMetyADO>(this, data);
                    this.SERVICE_UNIT_NAME = data.UNIT_NAME;
                    this.TRACKING_ID = trackingId;

                    if (ServiceReqs != null && ServiceReqs.Count > 0)
                    {
                        var serviceReq = ServiceReqs.FirstOrDefault(o => o.ID == data.SERVICE_REQ_ID);
                        this.INTRUCTION_TIME_STR = serviceReq != null ? serviceReq.INTRUCTION_DATE : 0;
                        this.USE_TIME = serviceReq != null ? serviceReq.USE_TIME : null;
                        this.USED_FOR_TRACKING_ID = serviceReq != null ? serviceReq.USED_FOR_TRACKING_ID : null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ServiceReqMatyADO : HIS_SERVICE_REQ_MATY
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public long INTRUCTION_TIME_STR { get; set; }

        public ServiceReqMatyADO() { }
        public ServiceReqMatyADO(HIS_SERVICE_REQ_MATY data, long trackingId, List<HIS_SERVICE_REQ> ServiceReqs)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqMatyADO>(this, data);
                    this.SERVICE_UNIT_NAME = data.UNIT_NAME;
                    this.TRACKING_ID = trackingId;

                    if (ServiceReqs != null && ServiceReqs.Count > 0)
                    {
                        this.INTRUCTION_TIME_STR = ServiceReqs.FirstOrDefault(o => o.ID == data.SERVICE_REQ_ID).INTRUCTION_DATE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class MedicalInstruction
    {
        public long TRACKING_ID { get; set; }
        public string MEDICAL_INSTRUCTION { get; set; }
    }

    public class ServiceCLS : HIS_SERE_SERV
    {
        public long TRACKING_ID { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public long INTRUCTION_DATE_STR { get; set; }

        public string SERVICE_CONDITION_CODE { get; set; }
        public string SERVICE_CONDITION_NAME { get; set; }
        public long TYPE_ID { get; set; }
        public string TEST_TYPE_CODE { get; set; }
        public long? TEST_TYPE_ID { get; set; }
        public string TEST_TYPE_NAME { get; set; }

        //máu
        public string BLOOD_ABO_CODE { get; set; }
        public long? BLOOD_ABO_ID { get; set; }
        public string BLOOD_RH_CODE { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public long BLOOD_TYPE_ID { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public decimal? DD_AMOUNT { get; set; }
        public long EXP_MEST_ID { get; set; }
        public string EXP_MEST_SUB_CODE { get; set; }
        public string EXP_MEST_SUB_CODE_2 { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? REQ_AMOUNT { get; set; }
        public long? REQ_BLOOD_ABO_ID { get; set; }
        public long? REQ_BLOOD_RH_ID { get; set; }
        public long? REQ_BLOOD_TYPE_ID { get; set; }
        public long? USE_TIME { get; set; }
        public long? NUM_ORDER_SERVICE_TYPE { get; set; }
        public bool IsGoupService { get; set; }
        public long? SERVICE_PARENT_ID { get; set; }
        public long? SERVICE_NUM_ORDER { get; set; }
        public List<ServiceCLS> serviceSplits { get; set; }
        public ServiceCLS() { }

        public ServiceCLS(HIS_SERE_SERV data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceCLS>(this, data);
                    serviceSplits = new List<ServiceCLS>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public ServiceCLS(V_HIS_EXP_MEST_BLTY_REQ_2 data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ServiceCLS>(this, data);
                    this.TDL_SERVICE_DESCRIPTION = data.DESCRIPTION;
                    this.TDL_INTRUCTION_DATE = data.INTRUCTION_DATE;
                    this.TDL_INTRUCTION_TIME = data.INTRUCTION_TIME;
                    this.TDL_REQUEST_DEPARTMENT_ID = data.REQUEST_DEPARTMENT_ID;
                    this.TDL_REQUEST_LOGINNAME = data.REQUEST_LOGINNAME;
                    this.TDL_REQUEST_ROOM_ID = data.REQUEST_ROOM_ID;
                    this.PARENT_ID = data.SERE_SERV_PARENT_ID;
                    this.TRACKING_ID = data.TRACKING_ID ?? 0;
                    this.BLOOD_ABO_CODE = data.BLOOD_ABO_CODE;
                    this.BLOOD_ABO_ID = data.BLOOD_ABO_ID;
                    this.BLOOD_RH_CODE = data.BLOOD_RH_CODE;
                    this.BLOOD_RH_ID = data.BLOOD_RH_ID;
                    this.BLOOD_TYPE_CODE = data.BLOOD_TYPE_CODE;
                    this.BLOOD_TYPE_ID = data.BLOOD_TYPE_ID;
                    this.BLOOD_TYPE_NAME = data.BLOOD_TYPE_NAME;
                    this.DD_AMOUNT = data.DD_AMOUNT;
                    this.EXP_MEST_ID = data.EXP_MEST_ID;
                    this.EXP_MEST_SUB_CODE = data.EXP_MEST_SUB_CODE;
                    this.EXP_MEST_SUB_CODE_2 = data.EXP_MEST_SUB_CODE_2;
                    this.NUM_ORDER = data.NUM_ORDER;
                    this.REQ_AMOUNT = data.REQ_AMOUNT;
                    this.REQ_BLOOD_ABO_ID = data.REQ_BLOOD_ABO_ID;
                    this.REQ_BLOOD_RH_ID = data.REQ_BLOOD_RH_ID;
                    this.REQ_BLOOD_TYPE_ID = data.REQ_BLOOD_TYPE_ID;
                    this.TDL_SERVICE_CODE = data.SERVICE_CODE;
                    this.TDL_SERVICE_NAME = data.SERVICE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ImpMestMedicineADO : V_HIS_IMP_MEST_MEDICINE
    {
        public long TRACKING_ID { get; set; }

        public long? USE_TIME { get; set; }
        public long? INTRUCTION_TIME { get; set; }

        public long? USE_TIME_OLD { get; set; }
        public long? INTRUCTION_TIME_OLD { get; set; }
        public long? NUM_ORDER_MEDICINE_LINE { get; set; }
        public long? NUM_ORDER_MEDICINE_USE_FORM { get; set; }
        public long? NUM_ORDER_MEDICINE_GROUP { get; set; }
        public long? NUM_ORDER_DOSAGE_FORM { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string DOSAGE_FORM_NAME { get; set; }
        public ImpMestMedicineADO() { }

        public ImpMestMedicineADO(V_HIS_IMP_MEST_MEDICINE data, HIS_SERVICE_REQ ServiceReq)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMedicineADO>(this, data);

                    if (ServiceReq != null)
                    {
                        this.USE_TIME = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                        this.USE_TIME_OLD = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME_OLD = ServiceReq.INTRUCTION_TIME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ImpMestMaterialADO : V_HIS_IMP_MEST_MATERIAL
    {
        public long TRACKING_ID { get; set; }
        public long? USE_TIME { get; set; }
        public long? INTRUCTION_TIME { get; set; }
        public long? USE_TIME_OLD { get; set; }
        public long? INTRUCTION_TIME_OLD { get; set; }

        public ImpMestMaterialADO() { }

        public ImpMestMaterialADO(V_HIS_IMP_MEST_MATERIAL data, HIS_SERVICE_REQ ServiceReq)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestMaterialADO>(this, data);

                    if (ServiceReq != null)
                    {
                        this.USE_TIME = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                        this.USE_TIME_OLD = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME_OLD = ServiceReq.INTRUCTION_TIME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SereServRationADO : V_HIS_SERE_SERV_RATION 
    {
        public string RATION_INFO { get; set; }

        public SereServRationADO() { }

        public SereServRationADO(V_HIS_SERE_SERV_RATION data) 
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServRationADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }

    public class ExpMestADO : HIS_EXP_MEST 
    {
        public long? USE_TIME { get; set; }
        public long? USE_TIME_AND_INTRUCTION_TIME { get; set; }

        public ExpMestADO(HIS_EXP_MEST data, HIS_SERVICE_REQ ServiceReq) 
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestADO>(this, data);
                    this.USE_TIME = ServiceReq != null ? ServiceReq.USE_TIME : null;
                    USE_TIME_AND_INTRUCTION_TIME = ServiceReq.USE_TIME.HasValue ? (ServiceReq.USE_TIME / 1000000) : (ServiceReq.INTRUCTION_TIME / 1000000);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }

    public class ImpMestBloodADO : V_HIS_IMP_MEST_BLOOD
    {
        public long? USE_TIME { get; set; }
        public long? INTRUCTION_TIME { get; set; }
        public long TRACKING_ID { get; set; }
        public long? USE_TIME_OLD { get; set; }
        public long? INTRUCTION_TIME_OLD { get; set; }

        public ImpMestBloodADO() { }
        public ImpMestBloodADO(V_HIS_IMP_MEST_BLOOD data, HIS_SERVICE_REQ ServiceReq)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ImpMestBloodADO>(this, data);
                    if (ServiceReq != null)
                    {
                        this.USE_TIME = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME = ServiceReq.INTRUCTION_TIME;
                        this.USE_TIME_OLD = ServiceReq.USE_TIME;
                        this.INTRUCTION_TIME_OLD = ServiceReq.INTRUCTION_TIME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class ExpMestMedicineADO : HIS_EXP_MEST_MEDICINE
    {
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public long? MEDICINE_GROUP_ID { get; set; }
        public long? NUM_ORDER_MEDICINE_LINE { get; set; }
        public long? NUM_ORDER_MEDICINE_USE_FORM { get; set; }
        public long? NUM_ORDER_MEDICINE_GROUP { get; set; }
        public long? NUM_ORDER_DOSAGE_FORM { get; set; }
        public string MEDICINE_USE_FORM_NAME { get; set; }
        public string MEDICINE_LINE_NAME { get; set; }
        public string MEDICINE_GROUP_NAME { get; set; }
        public string DOSAGE_FORM_NAME { get; set; }
        public ExpMestMedicineADO() { }
        public ExpMestMedicineADO(HIS_EXP_MEST_MEDICINE data, List<V_HIS_MEDICINE_TYPE> MedicineTypes)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMedicineADO>(this, data);
                    if (MedicineTypes != null && MedicineTypes.Count > 0)
                    {
                        var check = MedicineTypes.FirstOrDefault(p => p.ID == data.TDL_MEDICINE_TYPE_ID);
                        if (check != null)
                        {
                            this.ACTIVE_INGR_BHYT_CODE = check.ACTIVE_INGR_BHYT_CODE;
                            this.ACTIVE_INGR_BHYT_NAME = check.ACTIVE_INGR_BHYT_NAME;
                            this.MEDICINE_GROUP_ID = check.MEDICINE_GROUP_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
